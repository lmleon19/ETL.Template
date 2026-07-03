using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

public sealed class ServicioProcedimientoAlmacenado
{
    private readonly ILogger<ServicioProcedimientoAlmacenado> logger;

    public ServicioProcedimientoAlmacenado(ILogger<ServicioProcedimientoAlmacenado> logger) => this.logger = logger;

    public Task<ResultadoOperacion> EjecutarAsync(string cadenaConexion, string nombreProcedimiento, CancellationToken cancellationToken = default)
    {
        _ = cadenaConexion;
        _ = cancellationToken;
        logger.LogInformation("Ejecución de procedimiento pendiente de implementar para {NombreProcedimiento}.", nombreProcedimiento);
        return Task.FromResult(ResultadoOperacion.Error("Ejecución de procedimiento almacenado pendiente de implementar."));
    }
}
