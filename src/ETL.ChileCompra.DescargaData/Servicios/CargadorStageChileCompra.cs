using System.Data;
using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class CargadorStageChileCompra
{
    private readonly IConfiguration configuration;
    private readonly MetadataStage metadataStage;
    private readonly ILogger<CargadorStageChileCompra> logger;

    public CargadorStageChileCompra(
        IConfiguration configuration,
        MetadataStage metadataStage,
        ILogger<CargadorStageChileCompra> logger)
    {
        this.configuration = configuration;
        this.metadataStage = metadataStage;
        this.logger = logger;
    }

    public async Task<ResultadoOperacion> CargarAsync<TRegistroStage>(
        string tablaDestino,
        IEnumerable<TRegistroStage> registros,
        CancellationToken cancellationToken = default)
    {
        string cadenaConexion = configuration.GetConnectionString("ChileCompra") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            return ResultadoOperacion.Error("No existe cadena de conexion ChileCompra.");
        }

        DataTable tabla = CrearDataTable(registros);

        if (tabla.Rows.Count == 0)
        {
            return ResultadoOperacion.Correcto($"No existen registros para cargar en {tablaDestino}.");
        }

        try
        {
            logger.LogInformation("Cargando {Cantidad} registros en {TablaDestino}.", tabla.Rows.Count, tablaDestino);

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            using SqlBulkCopy bulkCopy = new(conexion)
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

            return ResultadoOperacion.Correcto($"Carga Stage finalizada en {tablaDestino}.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al cargar Stage {TablaDestino}.", tablaDestino);
            return ResultadoOperacion.Error($"No fue posible cargar la tabla Stage '{tablaDestino}'.", ex);
        }
    }

    private DataTable CrearDataTable<TRegistroStage>(IEnumerable<TRegistroStage> registros)
    {
        var columnas = metadataStage.ObtenerColumnas<TRegistroStage>();
        DataTable tabla = new();

        foreach ((_, var columna) in columnas)
        {
            tabla.Columns.Add(columna.Nombre, typeof(object));
        }

        foreach (TRegistroStage registro in registros)
        {
            DataRow fila = tabla.NewRow();

            foreach ((var propiedad, var columna) in columnas)
            {
                fila[columna.Nombre] = propiedad.GetValue(registro) ?? DBNull.Value;
            }

            tabla.Rows.Add(fila);
        }

        return tabla;
    }
}

