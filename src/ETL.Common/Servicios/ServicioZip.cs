using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

public sealed class ServicioZip
{
    private readonly ILogger<ServicioZip> logger;

    public ServicioZip(ILogger<ServicioZip> logger) => this.logger = logger;

    public ResultadoOperacion<IReadOnlyList<string>> DescomprimirArchivo(string rutaZip, string carpetaDestino)
    {
        logger.LogInformation("Descompresión pendiente de implementar para {RutaZip} en {CarpetaDestino}.", rutaZip, carpetaDestino);
        return ResultadoOperacion<IReadOnlyList<string>>.Error("Descompresión pendiente de implementar.");
    }
}
