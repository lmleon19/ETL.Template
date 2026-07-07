using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public static class ConfiguracionServiciosCargaInfoTransparencia
{
    public static IServiceCollection AddServiciosCargaInfoTransparencia(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpcionesCargaInfoTransparencia>(
            configuration.GetSection("ChileCompraCargaInfoTransparencia"));

        services.AddSingleton<ProcesoCargaInfoTransparencia>();
        services.AddSingleton<CalculadorPeriodosCargaInfoTransparencia>();
        services.AddSingleton<RepositorioLicitacionesChileCompra>();
        services.AddSingleton<CargadorLicitacionesInfoTransparencia>();
        services.AddSingleton<RepositorioOCChileCompra>();
        services.AddSingleton<CargadorOCInfoTransparencia>();

        return services;
    }
}
