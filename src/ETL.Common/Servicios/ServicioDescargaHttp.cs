using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

public sealed class ServicioDescargaHttp
{
    private readonly ILogger<ServicioDescargaHttp> logger;

    public ServicioDescargaHttp(ILogger<ServicioDescargaHttp> logger) => this.logger = logger;

    public Task<ResultadoOperacion<string>> DescargarArchivoAsync(string url, string rutaDestino, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Descarga pendiente de implementar desde {Url} hacia {RutaDestino}.", url, rutaDestino);
        return Task.FromResult(ResultadoOperacion<string>.Error("Descarga pendiente de implementar."));
    }
}
