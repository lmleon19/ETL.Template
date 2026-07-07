using ETL.Common.Resultados;
using ETL.Common.Servicios;
using ETL.InfoTransparencia.CargaPortal.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

OpcionesLoggerArchivo opcionesLoggerArchivo = builder.Configuration
    .GetSection("InfoTransparenciaCargaPortal:Logs")
    .Get<OpcionesLoggerArchivo>() ?? new OpcionesLoggerArchivo();

builder.Logging.AddLoggerArchivo(opcionesLoggerArchivo);
builder.Services.AddServiciosCargaPortal(builder.Configuration);

using IHost host = builder.Build();

ProcesoCargaPortal proceso = host.Services.GetRequiredService<ProcesoCargaPortal>();
ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.InfoTransparencia.CargaPortal");

try
{
    proceso.RegistrarInicioProceso();

    if (true)
    {
        // Valida que la configuracion base del ETL este disponible.
        ResultadoOperacion resultado = proceso.ValidarConfiguracionBase();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga PORTAL_Organismos desde BDC_Datamart hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarPortalOrganismosAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga PORTAL_Solicitantes desde BDC_Datamart hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarPortalSolicitantesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga PORTAL_Solicitudes desde BDC_Datamart hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarPortalSolicitudesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga PORTAL_Temas_solicitud desde BDC_Datamart hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarPortalTemasSolicitudAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga PORTAL_TrazaEstadoSolicitud desde BDC_Datamart hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarPortalTrazaEstadoSolicitudAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    proceso.RegistrarFinProceso();
    return 0;
}
catch (Exception ex)
{
    logger.LogError(ex, "Error no controlado durante la ejecucion del ETL InfoTransparencia CargaPortal.");
    return 1;
}
