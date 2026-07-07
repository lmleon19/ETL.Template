using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETL.InfoTransparencia.CargaPortal.Servicios;

public static class ConfiguracionServiciosCargaPortal
{
    public static IServiceCollection AddServiciosCargaPortal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpcionesCargaPortal>(
            configuration.GetSection("InfoTransparenciaCargaPortal"));

        services.AddSingleton<ProcesoCargaPortal>();
        services.AddSingleton<EjecutorCargaPortal>();

        return services;
    }
}
