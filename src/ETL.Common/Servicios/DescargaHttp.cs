using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para descargar archivos desde direcciones HTTP o HTTPS.
/// </summary>
public sealed class DescargaHttp
{
    private readonly ILogger<DescargaHttp> logger;

    public DescargaHttp(ILogger<DescargaHttp> logger) => this.logger = logger;

    /// <summary>
    /// Descarga un archivo desde una URL y lo guarda en una ruta local.
    /// </summary>
    /// <param name="url">URL HTTP o HTTPS del archivo origen.</param>
    /// <param name="rutaDestino">Ruta local donde se guardará el archivo descargado.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Resultado con la ruta local del archivo descargado.</returns>
    public async Task<ResultadoOperacion<string>> DescargarArchivoAsync(string url, string rutaDestino, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url);
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaDestino);

            Uri uri = new(url, UriKind.Absolute);

            if (uri.Scheme is not ("http" or "https"))
            {
                return ResultadoOperacion<string>.Error("La URL de descarga debe usar HTTP o HTTPS.");
            }

            string? carpetaDestino = Path.GetDirectoryName(rutaDestino);

            if (!string.IsNullOrWhiteSpace(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            logger.LogInformation("Iniciando descarga desde {Url}.", url);

            using HttpClient httpClient = new();
            using HttpResponseMessage respuesta = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            respuesta.EnsureSuccessStatusCode();

            await using Stream streamOrigen = await respuesta.Content.ReadAsStreamAsync(cancellationToken);
            await using FileStream streamDestino = File.Create(rutaDestino);

            await streamOrigen.CopyToAsync(streamDestino, cancellationToken);

            logger.LogInformation("Archivo descargado en {RutaDestino}.", rutaDestino);

            return ResultadoOperacion<string>.Correcto(rutaDestino, "Archivo descargado correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al descargar archivo desde {Url}.", url);
            return ResultadoOperacion<string>.Error($"No fue posible descargar el archivo desde '{url}'.", ex);
        }
    }
}
