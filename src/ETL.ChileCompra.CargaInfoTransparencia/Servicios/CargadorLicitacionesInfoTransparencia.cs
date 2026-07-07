using System.Data;
using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class CargadorLicitacionesInfoTransparencia(
    IConfiguration configuration,
    IOptions<OpcionesCargaInfoTransparencia> opciones,
    ILogger<CargadorLicitacionesInfoTransparencia> logger)
{
    public async Task CargarAsync(DatosLicitacionPeriodo datos, CancellationToken cancellationToken = default)
    {
        string cadenaConexion = ObtenerCadenaConexion("InfoTransparencia");

        await using SqlConnection conexion = new(cadenaConexion);
        await conexion.OpenAsync(cancellationToken);

        await using SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync(cancellationToken);

        try
        {
            await EliminarPeriodoAsync(
                conexion,
                transaccion,
                opciones.Value.Tablas.LicitacionDetalleDestino,
                datos.Periodo,
                cancellationToken);

            await EliminarPeriodoAsync(
                conexion,
                transaccion,
                opciones.Value.Tablas.LicitacionDestino,
                datos.Periodo,
                cancellationToken);

            await CargarLicitacionesAsync(conexion, transaccion, datos.Licitaciones, cancellationToken);
            await CargarDetallesAsync(conexion, transaccion, datos.Detalles, cancellationToken);

            await transaccion.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Periodo {Periodo} cargado en InfoTransparencia. Cabeceras: {Cabeceras}. Detalles: {Detalles}.",
                datos.Periodo.PeriodoArchivo,
                datos.Licitaciones.Count,
                datos.Detalles.Count);
        }
        catch
        {
            await RevertirTransaccionAsync(transaccion);
            throw;
        }
    }

    private async Task RevertirTransaccionAsync(SqlTransaction transaccion)
    {
        try
        {
            await transaccion.RollbackAsync(CancellationToken.None);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(
                ex,
                "No fue posible revertir la transaccion porque SQL Server ya la habia completado.");
        }
        catch (SqlException ex)
        {
            logger.LogWarning(
                ex,
                "No fue posible revertir la transaccion despues de un error de carga.");
        }
    }

    private async Task EliminarPeriodoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string tabla,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        ValidarNombreTabla(tabla);

        string sql = $"""
DELETE {tabla}
WHERE Fecha_publicacion >= @FechaDesde
  AND Fecha_publicacion < @FechaHasta;
""";

        await using SqlCommand comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText = sql;
        comando.CommandTimeout = 0;
        comando.Parameters.AddWithValue("@FechaDesde", periodo.FechaDesde);
        comando.Parameters.AddWithValue("@FechaHasta", periodo.FechaHasta);

        int eliminados = await comando.ExecuteNonQueryAsync(cancellationToken);

        logger.LogInformation(
            "Eliminados {Registros} registros desde {Tabla} para periodo {Periodo}.",
            eliminados,
            tabla,
            periodo.PeriodoArchivo);
    }

    private async Task CargarLicitacionesAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        IReadOnlyCollection<LicitacionTransparencia> registros,
        CancellationToken cancellationToken)
    {
        if (registros.Count == 0)
        {
            return;
        }

        DataTable tabla = CrearTablaLicitaciones(registros);

        await CargarDataTableAsync(conexion, transaccion, opciones.Value.Tablas.LicitacionDestino, tabla, cancellationToken);
    }

    private async Task CargarDetallesAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        IReadOnlyCollection<LicitacionDetalleTransparencia> registros,
        CancellationToken cancellationToken)
    {
        if (registros.Count == 0)
        {
            return;
        }

        DataTable tabla = CrearTablaDetalles(registros);

        await CargarDataTableAsync(conexion, transaccion, opciones.Value.Tablas.LicitacionDetalleDestino, tabla, cancellationToken);
    }

    private async Task CargarDataTableAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string tablaDestino,
        DataTable tabla,
        CancellationToken cancellationToken)
    {
        ValidarNombreTabla(tablaDestino);

        using SqlBulkCopy bulkCopy = new(conexion, SqlBulkCopyOptions.Default, transaccion)
        {
            DestinationTableName = tablaDestino,
            BatchSize = 5000,
            BulkCopyTimeout = 0,
            EnableStreaming = true
        };

        HashSet<string> columnasDestino = await ObtenerColumnasDestinoAsync(
            conexion,
            transaccion,
            tablaDestino,
            cancellationToken);

        List<string> columnasOmitidas = [];

        foreach (DataColumn columna in tabla.Columns)
        {
            if (columnasDestino.Contains(columna.ColumnName))
            {
                bulkCopy.ColumnMappings.Add(columna.ColumnName, columna.ColumnName);
            }
            else
            {
                columnasOmitidas.Add(columna.ColumnName);
            }
        }

        if (bulkCopy.ColumnMappings.Count == 0)
        {
            throw new InvalidOperationException($"No existen columnas comunes entre los datos origen y la tabla destino '{tablaDestino}'.");
        }

        if (columnasOmitidas.Count > 0)
        {
            logger.LogWarning(
                "La tabla destino {TablaDestino} no contiene estas columnas origen y seran omitidas: {Columnas}.",
                tablaDestino,
                string.Join(", ", columnasOmitidas));
        }

        await bulkCopy.WriteToServerAsync(tabla, cancellationToken);
    }

    private static async Task<HashSet<string>> ObtenerColumnasDestinoAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        string tablaDestino,
        CancellationToken cancellationToken)
    {
        string sql = $"SELECT TOP (0) * FROM {tablaDestino};";

        await using SqlCommand comando = conexion.CreateCommand();
        comando.Transaction = transaccion;
        comando.CommandText = sql;

        await using SqlDataReader reader = await comando.ExecuteReaderAsync(CommandBehavior.SchemaOnly, cancellationToken);
        DataTable? esquema = reader.GetSchemaTable();

        if (esquema is null)
        {
            throw new InvalidOperationException($"No fue posible obtener el esquema de la tabla destino '{tablaDestino}'.");
        }

        HashSet<string> columnas = new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow fila in esquema.Rows)
        {
            if (fila["ColumnName"] is string columna && !string.IsNullOrWhiteSpace(columna))
            {
                columnas.Add(columna);
            }
        }

        return columnas;
    }

    private static DataTable CrearTablaLicitaciones(IReadOnlyCollection<LicitacionTransparencia> registros)
    {
        DataTable tabla = new();
        tabla.Columns.Add("CodigoExterno", typeof(string));
        tabla.Columns.Add("Nombre", typeof(string));
        tabla.Columns.Add("InstitucionRut", typeof(string));
        tabla.Columns.Add("InstitucionNombre", typeof(string));
        tabla.Columns.Add("InstitucionIdOrPortal", typeof(string));
        tabla.Columns.Add("LicitacionTipo", typeof(string));
        tabla.Columns.Add("LicitacionMoneda", typeof(string));
        tabla.Columns.Add("Items_Cantidad", typeof(int));
        tabla.Columns.Add("Adjudicacion_UrlActa", typeof(string));
        tabla.Columns.Add("Adjudicacion_Fecha", typeof(DateTime));
        tabla.Columns.Add("Fechas_FechaAdjudicacion", typeof(DateTime));
        tabla.Columns.Add("NumeroOferentes", typeof(int));
        tabla.Columns.Add("CLP_total_Adjudicado", typeof(decimal));
        tabla.Columns.Add("MonedaOrigen_Total_Adjudicado", typeof(decimal));
        tabla.Columns.Add("Fecha_publicacion", typeof(DateTime));

        foreach (LicitacionTransparencia registro in registros)
        {
            tabla.Rows.Add(
                Valor(registro.CodigoExterno),
                Valor(registro.Nombre),
                Valor(registro.InstitucionRut),
                Valor(registro.InstitucionNombre),
                Valor(registro.InstitucionIdOrPortal),
                Valor(registro.LicitacionTipo),
                Valor(registro.LicitacionMoneda),
                registro.ItemsCantidad,
                Valor(registro.AdjudicacionUrlActa),
                Valor(registro.AdjudicacionFecha),
                Valor(registro.AdjudicacionFecha),
                Valor(registro.NumeroOferentes),
                Valor(registro.ClpTotalAdjudicado),
                Valor(registro.MonedaOrigenTotalAdjudicado),
                Valor(registro.FechaPublicacion));
        }

        return tabla;
    }

    private static DataTable CrearTablaDetalles(IReadOnlyCollection<LicitacionDetalleTransparencia> registros)
    {
        DataTable tabla = new();
        tabla.Columns.Add("CodigoExterno", typeof(string));
        tabla.Columns.Add("Correlativo", typeof(int));
        tabla.Columns.Add("Cantidad", typeof(decimal));
        tabla.Columns.Add("MontoUnitario", typeof(decimal));
        tabla.Columns.Add("ProductoCodigo", typeof(string));
        tabla.Columns.Add("ProductoNombre", typeof(string));
        tabla.Columns.Add("CategoriaCodigo", typeof(string));
        tabla.Columns.Add("CategoriaNombre", typeof(string));
        tabla.Columns.Add("ProveedorRut", typeof(string));
        tabla.Columns.Add("ProveedorRut_Numero", typeof(int));
        tabla.Columns.Add("ProveedorNombre", typeof(string));
        tabla.Columns.Add("TotalAdjudicado", typeof(decimal));
        tabla.Columns.Add("CLP_Total_Adjudicado", typeof(decimal));
        tabla.Columns.Add("Fecha_publicacion", typeof(DateTime));

        foreach (LicitacionDetalleTransparencia registro in registros)
        {
            tabla.Rows.Add(
                Valor(registro.CodigoExterno),
                Valor(registro.Correlativo),
                Valor(registro.Cantidad),
                Valor(registro.MontoUnitario),
                Valor(registro.ProductoCodigo),
                Valor(registro.ProductoNombre),
                Valor(registro.CategoriaCodigo),
                Valor(registro.CategoriaNombre),
                Valor(registro.ProveedorRut),
                Valor(registro.ProveedorRutNumero),
                Valor(registro.ProveedorNombre),
                Valor(registro.TotalAdjudicado),
                Valor(registro.ClpTotalAdjudicado),
                Valor(registro.FechaPublicacion));
        }

        return tabla;
    }

    private string ObtenerCadenaConexion(string nombre)
    {
        string? cadenaConexion = configuration.GetConnectionString(nombre);

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException($"No existe la cadena de conexion '{nombre}'.");
        }

        return cadenaConexion;
    }

    private static void ValidarNombreTabla(string tabla)
    {
        bool esValida = tabla
            .Split('.')
            .All(parte => parte.Length > 0 && parte.All(c => char.IsLetterOrDigit(c) || c == '_' || c is '[' or ']'));

        if (!esValida)
        {
            throw new InvalidOperationException($"La tabla '{tabla}' no tiene un nombre permitido.");
        }
    }

    private static object Valor(string? valor) => valor ?? string.Empty;

    private static object Valor<T>(T? valor) => valor is null ? DBNull.Value : valor;
}
