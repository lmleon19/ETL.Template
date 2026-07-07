using ETL.Common.Servicios;
using ETL.Common.Validacion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETL.ChileCompra.DescargaData.Servicios;

public static class ConfiguracionServiciosChileCompra
{
    public static IServiceCollection AddServiciosChileCompra(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcionesChileCompra>(configuration.GetSection("ChileCompra"));

        services.AddSingleton<DescargaHttp>();
        services.AddSingleton<SistemaArchivos>();
        services.AddSingleton<Zip>();
        services.AddSingleton<DetectorEncoding>();
        services.AddSingleton<ValidadorCsv>();
        services.AddSingleton<Csv>();
        services.AddSingleton<CargadorSqlBulkCopy>();
        services.AddSingleton<ProcedimientoAlmacenado>();

        services.AddSingleton<CalculadorPeriodosProceso>();
        services.AddSingleton<PreparadorCarpetasTrabajo>();
        services.AddSingleton<LimpiadorTablasStage>();
        services.AddSingleton<GeneradorUrlsChileCompra>();
        services.AddSingleton<MetadataStage>();
        services.AddSingleton<ConvertidorValoresChileCompra>();
        services.AddSingleton<NormalizadorRutChile>();
        services.AddSingleton<RepositorioParidadMoneda>();
        services.AddSingleton<ProveedorIdOrPortalInstitucion>();
        services.AddSingleton<MapeadorCsvStage>();
        services.AddSingleton<LectorArchivosStage>();
        services.AddSingleton<ValidadorArchivosStage>();
        services.AddSingleton<CargadorStageChileCompra>();
        services.AddSingleton<ProcesoChileCompraCarga>();

        return services;
    }
}
