using ETL.ChileCompra.Carga.Servicios;
using ETL.Common.Resultados;
using ETL.Common.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

OpcionesLoggerArchivo opcionesLoggerArchivo = builder.Configuration
    .GetSection("ChileCompra:Logs")
    .Get<OpcionesLoggerArchivo>() ?? new OpcionesLoggerArchivo();

builder.Logging.AddLoggerArchivo(opcionesLoggerArchivo);
builder.Services.AddServiciosChileCompra(builder.Configuration);

using IHost host = builder.Build();

ProcesoChileCompraCarga proceso = host.Services.GetRequiredService<ProcesoChileCompraCarga>();
ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.ChileCompra.Carga");

try
{
    proceso.RegistrarInicioProceso();

    if (false)
    {
        // Limpia carpetas de trabajo y tablas intermedias antes de iniciar la carga.
        ResultadoOperacion resultado = await proceso.PrepararEjecucionAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Calcula los ultimos meses cerrados configurados y los anios involucrados.
        ResultadoOperacion resultado = proceso.CalcularPeriodosProceso();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Descarga y prepara las conversiones de moneda necesarias para los periodos.
        ResultadoOperacion resultado = await proceso.PrepararConversionMonedaAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Descarga los archivos ZIP de licitaciones.
        ResultadoOperacion resultado = await proceso.DescargarLicitacionesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Descarga los archivos ZIP de OC.
        ResultadoOperacion resultado = await proceso.DescargarOCAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Descomprime los ZIP descargados en carpetas separadas.
        ResultadoOperacion resultado = await proceso.DescomprimirArchivosAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Valida la estructura de los CSV extraidos.
        ResultadoOperacion resultado = await proceso.ValidarArchivosCsvAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Lee los CSV de licitaciones, agrega metadatos y carga el Stage.
        ResultadoOperacion resultado = await proceso.CargarStageLicitacionesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (false)
    {
        // Lee los CSV de OC, agrega metadatos y carga el Stage.
        ResultadoOperacion resultado = await proceso.CargarStageOCAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Crea tablas finales anuales faltantes segun los anios presentes en Stage.
        ResultadoOperacion resultado = await proceso.EjecutarCreacionTablasFinalesAnualesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Traspasa licitaciones desde Stage hacia tablas finales.
        ResultadoOperacion resultado = await proceso.EjecutarTraspasoFinalLicitacionesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Traspasa OC desde Stage hacia tablas finales.
        ResultadoOperacion resultado = await proceso.EjecutarTraspasoFinalOCAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Mueve los ZIP procesados a la carpeta final reemplazando archivos existentes.
        ResultadoOperacion resultado = proceso.ArchivarZipFinales();
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
    logger.LogError(ex, "Error no controlado durante la ejecucion del ETL ChileCompra.");
    return 1;
}

