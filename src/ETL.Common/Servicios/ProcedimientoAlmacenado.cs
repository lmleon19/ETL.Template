using System.Data;
using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para ejecutar procedimientos almacenados de SQL Server.
/// </summary>
public sealed class ProcedimientoAlmacenado
{
    private readonly ILogger<ProcedimientoAlmacenado> logger;

    public ProcedimientoAlmacenado(ILogger<ProcedimientoAlmacenado> logger) => this.logger = logger;

    /// <summary>
    /// Ejecuta un procedimiento almacenado sin parámetros.
    /// </summary>
    /// <param name="cadenaConexion">Cadena de conexión a SQL Server.</param>
    /// <param name="nombreProcedimiento">Nombre del procedimiento almacenado.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Resultado de la ejecución.</returns>
    public async Task<ResultadoOperacion> EjecutarAsync(string cadenaConexion, string nombreProcedimiento, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cadenaConexion);
            ArgumentException.ThrowIfNullOrWhiteSpace(nombreProcedimiento);

            logger.LogInformation("Ejecutando procedimiento almacenado {NombreProcedimiento}.", nombreProcedimiento);

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            await using SqlCommand comando = new(nombreProcedimiento, conexion)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 0
            };

            await comando.ExecuteNonQueryAsync(cancellationToken);

            logger.LogInformation("Procedimiento almacenado {NombreProcedimiento} ejecutado correctamente.", nombreProcedimiento);

            return ResultadoOperacion.Correcto("Procedimiento almacenado ejecutado correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al ejecutar procedimiento almacenado {NombreProcedimiento}.", nombreProcedimiento);
            return ResultadoOperacion.Error($"No fue posible ejecutar el procedimiento almacenado '{nombreProcedimiento}'.", ex);
        }
    }
}
