using ETL.Common.Resultados;
using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class ProcesoCargaInfoTransparencia(
    IOptions<OpcionesCargaInfoTransparencia> opciones,
    RepositorioLicitacionesChileCompra repositorioLicitaciones,
    CargadorLicitacionesInfoTransparencia cargadorLicitaciones,
    RepositorioOCChileCompra repositorioOC,
    CargadorOCInfoTransparencia cargadorOC,
    ILogger<ProcesoCargaInfoTransparencia> logger)
{
    public void RegistrarInicioProceso() => logger.LogInformation("Inicio ETL ChileCompra CargaInfoTransparencia.");

    public void RegistrarFinProceso() => logger.LogInformation("Fin ETL ChileCompra CargaInfoTransparencia.");

    public ResultadoOperacion ValidarConfiguracionBase()
    {
        OpcionesCargaInfoTransparencia opcionesProceso = opciones.Value;

        if (opcionesProceso.MesesCerradosACargar <= 0)
        {
            return ResultadoOperacion.Error("MesesCerradosACargar debe ser mayor que cero.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.PrefijoLicitacionesOrigen))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:PrefijoLicitacionesOrigen.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.LicitacionDestino))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:LicitacionDestino.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.LicitacionDetalleDestino))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:LicitacionDetalleDestino.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.PrefijoOCOrigen))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:PrefijoOCOrigen.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.OCDestino))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:OCDestino.");
        }

        if (string.IsNullOrWhiteSpace(opcionesProceso.Tablas.OCDetalleDestino))
        {
            return ResultadoOperacion.Error("Debe configurar ChileCompraCargaInfoTransparencia:Tablas:OCDetalleDestino.");
        }

        logger.LogInformation("Configuracion base de CargaInfoTransparencia validada.");

        return ResultadoOperacion.Correcto("Configuracion base valida.");
    }

    public async Task<ResultadoOperacion> CargarLicitacionesAsync(
        IReadOnlyCollection<PeriodoCarga> periodos,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (periodos.Count == 0)
            {
                return ResultadoOperacion.Error("No existen periodos calculados para cargar licitaciones.");
            }

            foreach (PeriodoCarga periodo in periodos.OrderBy(p => p.FechaDesde))
            {
                logger.LogInformation(
                    "Iniciando carga de licitaciones para periodo {Periodo}. Rango: {FechaDesde:yyyy-MM-dd} a {FechaHasta:yyyy-MM-dd}.",
                    periodo.PeriodoArchivo,
                    periodo.FechaDesde,
                    periodo.FechaHasta);

                DatosLicitacionPeriodo datos = await repositorioLicitaciones.ObtenerAsync(periodo, cancellationToken);
                await cargadorLicitaciones.CargarAsync(datos, cancellationToken);
            }

            return ResultadoOperacion.Correcto("Carga de licitaciones finalizada correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error durante la carga de licitaciones para InfoTransparencia.");
            return ResultadoOperacion.Error("No fue posible cargar licitaciones en InfoTransparencia.", ex);
        }
    }

    public async Task<ResultadoOperacion> CargarOCAsync(
        IReadOnlyCollection<PeriodoCarga> periodos,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (periodos.Count == 0)
            {
                return ResultadoOperacion.Error("No existen periodos calculados para cargar OC.");
            }

            foreach (PeriodoCarga periodo in periodos.OrderBy(p => p.FechaDesde))
            {
                logger.LogInformation(
                    "Iniciando carga de OC para periodo {Periodo}. Rango: {FechaDesde:yyyy-MM-dd} a {FechaHasta:yyyy-MM-dd}.",
                    periodo.PeriodoArchivo,
                    periodo.FechaDesde,
                    periodo.FechaHasta);

                DatosOCPeriodo datos = await repositorioOC.ObtenerAsync(periodo, cancellationToken);
                await cargadorOC.CargarAsync(datos, cancellationToken);
            }

            return ResultadoOperacion.Correcto("Carga de OC finalizada correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error durante la carga de OC para InfoTransparencia.");
            return ResultadoOperacion.Error("No fue posible cargar OC en InfoTransparencia.", ex);
        }
    }
}
