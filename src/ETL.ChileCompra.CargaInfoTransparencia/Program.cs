using ETL.ChileCompra.CargaInfoTransparencia.Model;
using ETL.ChileCompra.CargaInfoTransparencia.Servicios;
using ETL.Common.Resultados;
using ETL.Common.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

OpcionesLoggerArchivo opcionesLoggerArchivo = builder.Configuration
    .GetSection("ChileCompraCargaInfoTransparencia:Logs")
    .Get<OpcionesLoggerArchivo>() ?? new OpcionesLoggerArchivo();

builder.Logging.AddLoggerArchivo(opcionesLoggerArchivo);
builder.Services.AddServiciosCargaInfoTransparencia(builder.Configuration);

using IHost host = builder.Build();

ProcesoCargaInfoTransparencia proceso = host.Services.GetRequiredService<ProcesoCargaInfoTransparencia>();
CalculadorPeriodosCargaInfoTransparencia calculadorPeriodos = host.Services.GetRequiredService<CalculadorPeriodosCargaInfoTransparencia>();
ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.ChileCompra.CargaInfoTransparencia");

try
{
    proceso.RegistrarInicioProceso();
    IReadOnlyCollection<PeriodoCarga> periodos = [];

    if (false)
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
        // Calcula los ultimos meses cerrados que usaran licitaciones y OC.
        periodos = calculadorPeriodos.Calcular(DateOnly.FromDateTime(DateTime.Today));
        foreach (PeriodoCarga periodo in periodos.OrderBy(p => p.FechaDesde))
        {
            logger.LogInformation(
                "Periodo calculado: {Periodo}. Rango: {FechaDesde:yyyy-MM-dd} a {FechaHasta:yyyy-MM-dd}.",
                periodo.PeriodoArchivo,
                periodo.FechaDesde,
                periodo.FechaHasta);
        }
    }

    if (false)
    {
        // Carga licitaciones de los ultimos meses cerrados hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarLicitacionesAsync(periodos);
        if (!resultado.Exitoso)
        {
            logger.LogError("{Mensaje}", resultado.Mensaje);
            return 1;
        }
    }

    if (true)
    {
        // Carga OC de los ultimos meses cerrados hacia InfoTransparencia.
        ResultadoOperacion resultado = await proceso.CargarOCAsync(periodos);
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
    logger.LogError(ex, "Error no controlado durante la ejecucion del ETL ChileCompra CargaInfoTransparencia.");
    return 1;
}
