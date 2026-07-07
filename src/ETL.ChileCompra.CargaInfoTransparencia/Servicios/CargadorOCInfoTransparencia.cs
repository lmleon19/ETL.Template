using System.Data;
using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class CargadorOCInfoTransparencia(
    IConfiguration configuration,
    IOptions<OpcionesCargaInfoTransparencia> opciones,
    ILogger<CargadorOCInfoTransparencia> logger)
{
    public async Task CargarAsync(DatosOCPeriodo datos, CancellationToken cancellationToken = default)
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
                opciones.Value.Tablas.OCDetalleDestino,
                "Fecha_Envio",
                datos.Periodo,
                cancellationToken);

            await EliminarPeriodoAsync(
                conexion,
                transaccion,
                opciones.Value.Tablas.OCDestino,
                "FechaEnvio",
                datos.Periodo,
                cancellationToken);

            await CargarOrdenesCompraAsync(conexion, transaccion, datos.OrdenesCompra, cancellationToken);
            await CargarDetallesAsync(conexion, transaccion, datos.Detalles, cancellationToken);

            await transaccion.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Periodo {Periodo} cargado en InfoTransparencia. OC: {Cabeceras}. Detalles OC: {Detalles}.",
                datos.Periodo.PeriodoArchivo,
                datos.OrdenesCompra.Count,
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
        string columnaFecha,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        ValidarNombreTabla(tabla);
        ValidarIdentificador(columnaFecha);

        string sql = $"""
DELETE {tabla}
WHERE {columnaFecha} >= @FechaDesde
  AND {columnaFecha} < @FechaHasta;
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

    private async Task CargarOrdenesCompraAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        IReadOnlyCollection<OrdenCompraTransparencia> registros,
        CancellationToken cancellationToken)
    {
        if (registros.Count == 0)
        {
            return;
        }

        DataTable tabla = CrearTablaOrdenesCompra(registros);

        await CargarDataTableAsync(conexion, transaccion, opciones.Value.Tablas.OCDestino, tabla, cancellationToken);
    }

    private async Task CargarDetallesAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        IReadOnlyCollection<OrdenCompraDetalleTransparencia> registros,
        CancellationToken cancellationToken)
    {
        if (registros.Count == 0)
        {
            return;
        }

        DataTable tabla = CrearTablaDetalles(registros);

        await CargarDataTableAsync(conexion, transaccion, opciones.Value.Tablas.OCDetalleDestino, tabla, cancellationToken);
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

    private static DataTable CrearTablaOrdenesCompra(IReadOnlyCollection<OrdenCompraTransparencia> registros)
    {
        DataTable tabla = new();
        tabla.Columns.Add("Codigo", typeof(string));
        tabla.Columns.Add("Link", typeof(string));
        tabla.Columns.Add("Fecha", typeof(DateTime));
        tabla.Columns.Add("FechaEnvio", typeof(DateTime));
        tabla.Columns.Add("FechaAceptacion", typeof(DateTime));
        tabla.Columns.Add("Nombre", typeof(string));
        tabla.Columns.Add("Descripcion", typeof(string));
        tabla.Columns.Add("EstadoCodigo", typeof(decimal));
        tabla.Columns.Add("EstadoDescripcion", typeof(string));
        tabla.Columns.Add("EstadoProveedorCodigo", typeof(decimal));
        tabla.Columns.Add("EstadoProveedorDescripcion", typeof(string));
        tabla.Columns.Add("UnidadCompra", typeof(string));
        tabla.Columns.Add("InstitucionRut", typeof(string));
        tabla.Columns.Add("InstitucionRut_Numero", typeof(int));
        tabla.Columns.Add("InstitucionRut_DV", typeof(string));
        tabla.Columns.Add("IdOrPortal", typeof(string));
        tabla.Columns.Add("ProveedorSucursal", typeof(string));
        tabla.Columns.Add("ProveedorRut", typeof(string));
        tabla.Columns.Add("ProveedorRut_Numero", typeof(int));
        tabla.Columns.Add("ProveedorRut_DV", typeof(string));
        tabla.Columns.Add("ProveedorNombre", typeof(string));
        tabla.Columns.Add("CodigoLicitacion", typeof(string));
        tabla.Columns.Add("MonedaNombre", typeof(string));
        tabla.Columns.Add("Impuestos", typeof(decimal));
        tabla.Columns.Add("Descuentos", typeof(decimal));
        tabla.Columns.Add("MontoTotal_Neto", typeof(decimal));
        tabla.Columns.Add("MontoTotal_MonedaOrigen", typeof(decimal));
        tabla.Columns.Add("MontoTotal_CLP", typeof(decimal));
        tabla.Columns.Add("CodigoTipo", typeof(decimal));
        tabla.Columns.Add("TipoDescripcion", typeof(string));
        tabla.Columns.Add("Origen", typeof(int));

        foreach (OrdenCompraTransparencia registro in registros)
        {
            tabla.Rows.Add(
                Valor(registro.Codigo),
                Valor(registro.Link),
                Valor(registro.Fecha),
                Valor(registro.FechaEnvio),
                Valor(registro.FechaAceptacion),
                Valor(registro.Nombre),
                Valor(registro.Descripcion),
                Valor(registro.EstadoCodigo),
                Valor(registro.EstadoDescripcion),
                Valor(registro.EstadoProveedorCodigo),
                Valor(registro.EstadoProveedorDescripcion),
                Valor(registro.UnidadCompra),
                Valor(registro.InstitucionRut),
                Valor(registro.InstitucionRutNumero),
                Valor(registro.InstitucionRutDv),
                Valor(registro.IdOrPortal),
                Valor(registro.ProveedorSucursal),
                Valor(registro.RutSucursal),
                Valor(registro.ProveedorRutNumero),
                Valor(registro.ProveedorRutDv),
                Valor(registro.ProveedorNombre),
                Valor(registro.CodigoLicitacion),
                Valor(registro.MonedaNombre),
                Valor(registro.Impuestos),
                Valor(registro.Descuentos),
                Valor(registro.MontoTotalNeto),
                Valor(registro.MontoTotalMonedaOrigen),
                Valor(registro.MontoTotalClp),
                Valor(registro.CodigoTipo),
                Valor(registro.TipoDescripcion),
                registro.Origen);
        }

        return tabla;
    }

    private static DataTable CrearTablaDetalles(IReadOnlyCollection<OrdenCompraDetalleTransparencia> registros)
    {
        DataTable tabla = new();
        tabla.Columns.Add("Codigo", typeof(string));
        tabla.Columns.Add("Correlativo", typeof(int));
        tabla.Columns.Add("Cantidad", typeof(decimal));
        tabla.Columns.Add("CodigoProducto", typeof(string));
        tabla.Columns.Add("Producto", typeof(string));
        tabla.Columns.Add("Categoria", typeof(string));
        tabla.Columns.Add("CodigoCategoria", typeof(string));
        tabla.Columns.Add("MonedaNombre", typeof(string));
        tabla.Columns.Add("PrecioNeto", typeof(decimal));
        tabla.Columns.Add("ImpuestoTotal", typeof(decimal));
        tabla.Columns.Add("Descuentos", typeof(decimal));
        tabla.Columns.Add("Total", typeof(decimal));
        tabla.Columns.Add("Total_CLP", typeof(decimal));
        tabla.Columns.Add("Fecha_Envio", typeof(DateTime));

        foreach (OrdenCompraDetalleTransparencia registro in registros)
        {
            tabla.Rows.Add(
                Valor(registro.Codigo),
                Valor(registro.Correlativo),
                Valor(registro.Cantidad),
                Valor(registro.CodigoProducto),
                Valor(registro.Producto),
                Valor(registro.Categoria),
                Valor(registro.CodigoCategoria),
                Valor(registro.MonedaNombre),
                Valor(registro.PrecioNeto),
                Valor(registro.ImpuestoTotal),
                Valor(registro.Descuentos),
                Valor(registro.Total),
                Valor(registro.TotalClp),
                Valor(registro.FechaEnvio));
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

    private static void ValidarIdentificador(string identificador)
    {
        if (identificador.Length == 0 || !identificador.All(c => char.IsLetterOrDigit(c) || c == '_'))
        {
            throw new InvalidOperationException($"El identificador '{identificador}' no tiene un nombre permitido.");
        }
    }

    private static object Valor(string? valor) => valor ?? string.Empty;

    private static object Valor<T>(T? valor) => valor is null ? DBNull.Value : valor;
}
