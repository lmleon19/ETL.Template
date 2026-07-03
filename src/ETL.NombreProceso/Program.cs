using ETL.Common.Resultados;
using ETL.Common.Servicios;
using ETL.Common.Validacion;
using ETL.NombreProceso;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<OpcionesProceso>(builder.Configuration.GetSection("Proceso"));
builder.Services.AddSingleton<DescargaHttp>();
builder.Services.AddSingleton<SistemaArchivos>();
builder.Services.AddSingleton<Zip>();
builder.Services.AddSingleton<DetectorEncoding>();
builder.Services.AddSingleton<ValidadorCsv>();
builder.Services.AddSingleton<Csv>();
builder.Services.AddSingleton<CargadorSqlBulkCopy>();
builder.Services.AddSingleton<ProcedimientoAlmacenado>();

using IHost host = builder.Build();

IServiceProvider servicios = host.Services;
ILogger logger = servicios.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.NombreProceso");
OpcionesProceso opciones = servicios.GetRequiredService<IOptions<OpcionesProceso>>().Value;
IConfiguration configuration = servicios.GetRequiredService<IConfiguration>();

SistemaArchivos sistemaArchivos = servicios.GetRequiredService<SistemaArchivos>();
DescargaHttp descargaHttp = servicios.GetRequiredService<DescargaHttp>();
Zip zip = servicios.GetRequiredService<Zip>();
DetectorEncoding detectorEncoding = servicios.GetRequiredService<DetectorEncoding>();
ValidadorCsv validadorCsv = servicios.GetRequiredService<ValidadorCsv>();
Csv csv = servicios.GetRequiredService<Csv>();
CargadorSqlBulkCopy cargadorSqlBulkCopy = servicios.GetRequiredService<CargadorSqlBulkCopy>();
ProcedimientoAlmacenado procedimientoAlmacenado = servicios.GetRequiredService<ProcedimientoAlmacenado>();

string rutaZip = Path.Combine(opciones.CarpetaTrabajo, opciones.NombreArchivoZip);
string carpetaExtraccion = Path.Combine(opciones.CarpetaTrabajo, "extraido");
string cadenaConexion = configuration.GetConnectionString("Principal") ?? string.Empty;

try
{
    logger.LogInformation("Inicio ETL NombreProceso.");

    // 1. Preparar carpetas de trabajo.
    sistemaArchivos.CrearCarpetaSiNoExiste(opciones.CarpetaTrabajo);
    sistemaArchivos.EliminarCarpetaSiExiste(carpetaExtraccion);

    // 2. Descargar archivo.
    ResultadoOperacion<string> descarga = await descargaHttp.DescargarArchivoAsync(opciones.UrlDescarga, rutaZip);
    if (!descarga.Exitoso)
    {
        logger.LogError("Error en descarga: {Mensaje}", descarga.Mensaje);
        return 1;
    }

    // 3. Descomprimir archivo.
    ResultadoOperacion<IReadOnlyList<string>> descompresion = zip.DescomprimirArchivo(rutaZip, carpetaExtraccion);
    if (!descompresion.Exitoso)
    {
        logger.LogError("Error en descompresión: {Mensaje}", descompresion.Mensaje);
        return 1;
    }

    // 4. Detectar encoding.
    string rutaCsv = sistemaArchivos.BuscarPrimerArchivo(carpetaExtraccion, opciones.PatronCsv);
    var encoding = detectorEncoding.DetectarEncoding(rutaCsv);

    // 5. Validar CSV.
    ResultadoOperacion validacion = await validadorCsv.ValidarColumnasObligatoriasAsync(rutaCsv, encoding, opciones.Delimitador, opciones.ColumnasObligatorias);
    if (!validacion.Exitoso)
    {
        logger.LogError("Error en validación CSV: {Mensaje}", validacion.Mensaje);
        return 1;
    }

    // 6. Leer CSV.
    IReadOnlyList<Dictionary<string, string>> registros = await csv.LeerCsvAsync(rutaCsv, encoding, opciones.Delimitador);

    // 7. Cargar tabla Stage.
    ResultadoOperacion cargaStage = await cargadorSqlBulkCopy.CargarAsync(cadenaConexion, opciones.TablaStage, registros);
    if (!cargaStage.Exitoso)
    {
        logger.LogError("Error en carga Stage: {Mensaje}", cargaStage.Mensaje);
        return 1;
    }

    // 8. Ejecutar procedimiento almacenado final.
    ResultadoOperacion procedimientoFinal = await procedimientoAlmacenado.EjecutarAsync(cadenaConexion, opciones.ProcedimientoFinal);
    if (!procedimientoFinal.Exitoso)
    {
        logger.LogError("Error en procedimiento final: {Mensaje}", procedimientoFinal.Mensaje);
        return 1;
    }

    logger.LogInformation(
        "Fin ETL NombreProceso. Descarga={Descarga}, Descompresión={Descompresion}, Validación={Validacion}, Carga={Carga}, Procedimiento={Procedimiento}.",
        descarga.Mensaje,
        descompresion.Mensaje,
        validacion.Mensaje,
        cargaStage.Mensaje,
        procedimientoFinal.Mensaje);

    return 0;
}
catch (Exception ex)
{
    logger.LogError(ex, "Error no controlado durante la ejecución del ETL NombreProceso.");
    return 1;
}
