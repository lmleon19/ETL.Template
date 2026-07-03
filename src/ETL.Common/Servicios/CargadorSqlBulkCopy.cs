using System.Data;
using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para cargar registros a tablas Stage mediante SqlBulkCopy.
/// </summary>
public sealed class CargadorSqlBulkCopy
{
    private readonly ILogger<CargadorSqlBulkCopy> logger;

    public CargadorSqlBulkCopy(ILogger<CargadorSqlBulkCopy> logger) => this.logger = logger;

    /// <summary>
    /// Carga registros en una tabla destino usando los nombres de columnas del diccionario.
    /// </summary>
    /// <param name="cadenaConexion">Cadena de conexión a SQL Server.</param>
    /// <param name="tablaDestino">Nombre de la tabla Stage destino.</param>
    /// <param name="registros">Registros que se cargarán en la tabla destino.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Resultado de la carga masiva.</returns>
    public async Task<ResultadoOperacion> CargarAsync(string cadenaConexion, string tablaDestino, IEnumerable<Dictionary<string, string>> registros, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cadenaConexion);
            ArgumentException.ThrowIfNullOrWhiteSpace(tablaDestino);
            ArgumentNullException.ThrowIfNull(registros);

            DataTable tabla = CrearDataTable(registros);

            if (tabla.Rows.Count == 0)
            {
                return ResultadoOperacion.Correcto("No existen registros para cargar.");
            }

            logger.LogInformation("Iniciando carga masiva hacia {TablaDestino}. Registros: {CantidadRegistros}.", tablaDestino, tabla.Rows.Count);

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            using Microsoft.Data.SqlClient.SqlBulkCopy bulkCopy = new(conexion)
            {
                DestinationTableName = tablaDestino,
                BatchSize = 5000,
                BulkCopyTimeout = 0,
                EnableStreaming = true
            };

            foreach (DataColumn columna in tabla.Columns)
            {
                bulkCopy.ColumnMappings.Add(columna.ColumnName, columna.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(tabla, cancellationToken);

            logger.LogInformation("Carga masiva finalizada hacia {TablaDestino}.", tablaDestino);

            return ResultadoOperacion.Correcto("Carga masiva finalizada correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error durante la carga masiva hacia {TablaDestino}.", tablaDestino);
            return ResultadoOperacion.Error($"No fue posible cargar registros en la tabla '{tablaDestino}'.", ex);
        }
    }

    private static DataTable CrearDataTable(IEnumerable<Dictionary<string, string>> registros)
    {
        DataTable tabla = new();
        Dictionary<string, string>[] registrosMaterializados = registros.ToArray();

        if (registrosMaterializados.Length == 0)
        {
            return tabla;
        }

        string[] columnas = registrosMaterializados[0].Keys.ToArray();

        foreach (string columna in columnas)
        {
            tabla.Columns.Add(columna, typeof(string));
        }

        foreach (Dictionary<string, string> registro in registrosMaterializados)
        {
            DataRow fila = tabla.NewRow();

            foreach (string columna in columnas)
            {
                fila[columna] = registro.TryGetValue(columna, out string? valor) ? valor : DBNull.Value;
            }

            tabla.Rows.Add(fila);
        }

        return tabla;
    }
}
