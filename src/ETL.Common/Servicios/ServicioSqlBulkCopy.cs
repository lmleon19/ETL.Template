using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

public sealed class ServicioSqlBulkCopy
{
    private readonly ILogger<ServicioSqlBulkCopy> logger;

    public ServicioSqlBulkCopy(ILogger<ServicioSqlBulkCopy> logger) => this.logger = logger;

    public Task<ResultadoOperacion> CargarAsync(string cadenaConexion, string tablaDestino, IEnumerable<Dictionary<string, string>> registros, CancellationToken cancellationToken = default)
    {
        _ = cadenaConexion;
        _ = registros;
        _ = cancellationToken;
        logger.LogInformation("Carga Stage pendiente de implementar para {TablaDestino}.", tablaDestino);
        return Task.FromResult(ResultadoOperacion.Error("Carga Stage pendiente de implementar."));
    }
}
