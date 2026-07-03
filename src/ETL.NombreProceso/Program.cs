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
builder.Services.AddSingleton<ServicioDescargaHttp>();
builder.Services.AddSingleton<ServicioSistemaArchivos>();
builder.Services.AddSingleton<ServicioZip>();
builder.Services.AddSingleton<DetectorEncoding>();
builder.Services.AddSingleton<ValidadorCsv>();
builder.Services.AddSingleton<ServicioCsv>();
builder.Services.AddSingleton<ServicioSqlBulkCopy>();
builder.Services.AddSingleton<ServicioProcedimientoAlmacenado>();

using IHost host = builder.Build();

IServiceProvider servicios = host.Services;
ILogger logger = servicios.GetRequiredService<ILoggerFactory>().CreateLogger("ETL.NombreProceso");
OpcionesProceso opciones = servicios.GetRequiredService<IOptions<OpcionesProceso>>().Value;
IConfiguration configuration = servicios.GetRequiredService<IConfiguration>();

ServicioSistemaArchivos servicioSistemaArchivos = servicios.GetRequiredService<ServicioSistemaArchivos>();
ServicioDescargaHttp servicioDescargaHttp = servicios.GetRequiredService<ServicioDescargaHttp>();
ServicioZip servicioZip = servicios.GetRequiredService<ServicioZip>();
DetectorEncoding detectorEncoding = servicios.GetRequiredService<DetectorEncoding>();
ValidadorCsv validadorCsv = servicios.GetRequiredService<ValidadorCsv>();
ServicioCsv servicioCsv = servicios.GetRequiredService<ServicioCsv>();
ServicioSqlBulkCopy servicioSqlBulkCopy = servicios.GetRequiredService<ServicioSqlBulkCopy>();
ServicioProcedimientoAlmacenado servicioProcedimientoAlmacenado = servicios.GetRequiredService<ServicioProcedimientoAlmacenado>();

string rutaZip = Path.Combine(opciones.CarpetaTrabajo, opciones.NombreArchivoZip);
string carpetaExtraccion = Path.Combine(opciones.CarpetaTrabajo, "extraido");
string cadenaConexion = configuration.GetConnectionString("Principal") ?? string.Empty;

logger.LogInformation("Inicio ETL NombreProceso.");

// 1. Preparar carpetas de trabajo.
servicioSistemaArchivos.CrearCarpetaSiNoExiste(opciones.CarpetaTrabajo);
servicioSistemaArchivos.EliminarCarpetaSiExiste(carpetaExtraccion);

// 2. Descargar archivo.
ResultadoOperacion<string> descarga = await servicioDescargaHttp.DescargarArchivoAsync(opciones.UrlDescarga, rutaZip);

// 3. Descomprimir archivo.
ResultadoOperacion<IReadOnlyList<string>> descompresion = servicioZip.DescomprimirArchivo(rutaZip, carpetaExtraccion);

// 4. Detectar encoding.
string rutaCsv = servicioSistemaArchivos.BuscarPrimerArchivo(carpetaExtraccion, opciones.PatronCsv);
var encoding = detectorEncoding.DetectarEncoding(rutaCsv);

// 5. Validar CSV.
ResultadoOperacion validacion = await validadorCsv.ValidarColumnasObligatoriasAsync(rutaCsv, encoding, opciones.Delimitador, opciones.ColumnasObligatorias);

// 6. Leer CSV.
IReadOnlyList<Dictionary<string, string>> registros = await servicioCsv.LeerCsvAsync(rutaCsv, encoding, opciones.Delimitador);

// 7. Cargar tabla Stage.
ResultadoOperacion cargaStage = await servicioSqlBulkCopy.CargarAsync(cadenaConexion, opciones.TablaStage, registros);

// 8. Ejecutar procedimiento almacenado final.
ResultadoOperacion procedimientoFinal = await servicioProcedimientoAlmacenado.EjecutarAsync(cadenaConexion, opciones.ProcedimientoFinal);

logger.LogInformation(
    "Fin estructura base ETL. Descarga={Descarga}, Descompresión={Descompresion}, Validación={Validacion}, Carga={Carga}, Procedimiento={Procedimiento}.",
    descarga.Mensaje,
    descompresion.Mensaje,
    validacion.Mensaje,
    cargaStage.Mensaje,
    procedimientoFinal.Mensaje);

return 0;
