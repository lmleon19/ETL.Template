using ETL.ChileCompra.Carga.Servicios;
using ETL.Common.Resultados;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServiciosChileCompra(builder.Configuration);

using IHost host = builder.Build();

ProcesoChileCompraCarga proceso = host.Services.GetRequiredService<ProcesoChileCompraCarga>();
ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.ChileCompra.Carga");

try
{
    proceso.RegistrarInicioProceso();

    if (true)
    {
        // Limpia carpetas de trabajo y tablas intermedias antes de iniciar la carga.
        ResultadoOperacion resultado = await proceso.PrepararEjecucionAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Calcula los ultimos cuatro meses cerrados y los anios involucrados.
        ResultadoOperacion resultado = proceso.CalcularPeriodosProceso();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Crea las tablas anuales de licitaciones y ordenes de compra si no existen.
        ResultadoOperacion resultado = await proceso.CrearTablasAnualesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Descarga y prepara las conversiones de moneda necesarias para los periodos.
        ResultadoOperacion resultado = await proceso.PrepararConversionMonedaAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Descarga los archivos ZIP de licitaciones.
        ResultadoOperacion resultado = await proceso.DescargarLicitacionesAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Descarga los archivos ZIP de ordenes de compra.
        ResultadoOperacion resultado = await proceso.DescargarOrdenesCompraAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Descomprime los ZIP descargados en carpetas separadas.
        ResultadoOperacion resultado = await proceso.DescomprimirArchivosAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Valida la estructura de los CSV extraidos.
        ResultadoOperacion resultado = await proceso.ValidarArchivosCsvAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Lee los CSV, agrega metadatos y carga las tablas intermedias.
        ResultadoOperacion resultado = await proceso.CargarTablasIntermediasAsync();
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Elimina periodos existentes y traspasa datos desde tablas intermedias a tablas reales.
        ResultadoOperacion resultado = await proceso.EjecutarTraspasoFinalAsync();
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
