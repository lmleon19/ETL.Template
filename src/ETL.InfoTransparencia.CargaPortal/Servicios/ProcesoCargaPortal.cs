using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.InfoTransparencia.CargaPortal.Servicios;

public sealed class ProcesoCargaPortal(
    IOptions<OpcionesCargaPortal> opciones,
    EjecutorCargaPortal ejecutorCargaPortal,
    ILogger<ProcesoCargaPortal> logger)
{
    private const string NombreCargaPortalOrganismos = "PORTAL_Organismos";
    private const string NombreCargaPortalSolicitantes = "PORTAL_Solicitantes";
    private const string NombreCargaPortalSolicitudes = "PORTAL_Solicitudes";
    private const string NombreCargaPortalTemasSolicitud = "PORTAL_Temas_solicitud";
    private const string NombreCargaPortalTrazaEstadoSolicitud = "PORTAL_TrazaEstadoSolicitud";

    public void RegistrarInicioProceso() => logger.LogInformation("Inicio ETL InfoTransparencia CargaPortal.");

    public void RegistrarFinProceso() => logger.LogInformation("Fin ETL InfoTransparencia CargaPortal.");

    public ResultadoOperacion ValidarConfiguracionBase()
    {
        OpcionesCargaPortal opcionesProceso = opciones.Value;

        if (string.IsNullOrWhiteSpace(opcionesProceso.NombreConexionDestino))
        {
            return ResultadoOperacion.Error("Debe configurar InfoTransparenciaCargaPortal:NombreConexionDestino.");
        }

        if (opcionesProceso.TimeoutComandoSegundos < 0)
        {
            return ResultadoOperacion.Error("InfoTransparenciaCargaPortal:TimeoutComandoSegundos no puede ser negativo.");
        }

        if (opcionesProceso.Cargas.Count == 0)
        {
            return ResultadoOperacion.Error("Debe configurar al menos una carga en InfoTransparenciaCargaPortal:Cargas.");
        }

        foreach (OpcionesCargaTablaPortal carga in opcionesProceso.Cargas)
        {
            if (string.IsNullOrWhiteSpace(carga.Nombre))
            {
                return ResultadoOperacion.Error("Todas las cargas deben tener Nombre.");
            }

            if (string.IsNullOrWhiteSpace(carga.Procedimiento))
            {
                return ResultadoOperacion.Error($"La carga {carga.Nombre} debe tener Procedimiento.");
            }
        }

        logger.LogInformation("Configuracion base de CargaPortal validada.");

        return ResultadoOperacion.Correcto("Configuracion base valida.");
    }

    public Task<ResultadoOperacion> CargarPortalOrganismosAsync(CancellationToken cancellationToken = default) =>
        EjecutarCargaConfiguradaAsync(NombreCargaPortalOrganismos, cancellationToken);

    public Task<ResultadoOperacion> CargarPortalSolicitantesAsync(CancellationToken cancellationToken = default) =>
        EjecutarCargaConfiguradaAsync(NombreCargaPortalSolicitantes, cancellationToken);

    public Task<ResultadoOperacion> CargarPortalSolicitudesAsync(CancellationToken cancellationToken = default) =>
        EjecutarCargaConfiguradaAsync(NombreCargaPortalSolicitudes, cancellationToken);

    public Task<ResultadoOperacion> CargarPortalTemasSolicitudAsync(CancellationToken cancellationToken = default) =>
        EjecutarCargaConfiguradaAsync(NombreCargaPortalTemasSolicitud, cancellationToken);

    public Task<ResultadoOperacion> CargarPortalTrazaEstadoSolicitudAsync(CancellationToken cancellationToken = default) =>
        EjecutarCargaConfiguradaAsync(NombreCargaPortalTrazaEstadoSolicitud, cancellationToken);

    private async Task<ResultadoOperacion> EjecutarCargaConfiguradaAsync(
        string nombreCarga,
        CancellationToken cancellationToken = default)
    {
        try
        {
            OpcionesCargaTablaPortal? carga = opciones.Value.Cargas
                .FirstOrDefault(c => string.Equals(c.Nombre, nombreCarga, StringComparison.OrdinalIgnoreCase));

            if (carga is null)
            {
                return ResultadoOperacion.Error($"No existe configuracion para la carga {nombreCarga}.");
            }

            if (!carga.Activo)
            {
                logger.LogInformation("Carga {Carga} omitida porque esta desactivada por configuracion.", nombreCarga);
                return ResultadoOperacion.Correcto($"Carga {nombreCarga} omitida por configuracion.");
            }

            await ejecutorCargaPortal.EjecutarAsync(carga, cancellationToken);

            return ResultadoOperacion.Correcto($"Carga {nombreCarga} finalizada correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error durante la carga {Carga} hacia InfoTransparencia.", nombreCarga);
            return ResultadoOperacion.Error($"No fue posible ejecutar la carga {nombreCarga} en InfoTransparencia.", ex);
        }
    }
}
