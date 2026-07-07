using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class RepositorioOCChileCompra(
    IConfiguration configuration,
    IOptions<OpcionesCargaInfoTransparencia> opciones,
    ILogger<RepositorioOCChileCompra> logger)
{
    public async Task<DatosOCPeriodo> ObtenerAsync(PeriodoCarga periodo, CancellationToken cancellationToken = default)
    {
        string cadenaConexion = ObtenerCadenaConexion("ChileCompra");
        string tablaOrigen = ObtenerTablaOrigen(periodo);

        logger.LogInformation("Leyendo OC desde {TablaOrigen} para {Periodo}.csv.", tablaOrigen, periodo.PeriodoArchivo);

        await using SqlConnection conexion = new(cadenaConexion);
        await conexion.OpenAsync(cancellationToken);

        IReadOnlyCollection<OrdenCompraTransparencia> ordenesCompra = await LeerOrdenesCompraAsync(conexion, tablaOrigen, periodo, cancellationToken);
        IReadOnlyCollection<OrdenCompraDetalleTransparencia> detalles = await LeerDetallesAsync(conexion, tablaOrigen, periodo, cancellationToken);

        logger.LogInformation(
            "Lectura de OC finalizada para {Periodo}. Cabeceras: {Cabeceras}. Detalles: {Detalles}.",
            periodo.PeriodoArchivo,
            ordenesCompra.Count,
            detalles.Count);

        return new DatosOCPeriodo(periodo, ordenesCompra, detalles);
    }

    private async Task<IReadOnlyCollection<OrdenCompraTransparencia>> LeerOrdenesCompraAsync(
        SqlConnection conexion,
        string tablaOrigen,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        string sql = $"""
SELECT
    ID,
    [Codigo],
    Link,
    Nombre,
    ISNULL([Descripcion/Obervaciones], '') AS Descripcion,
    CodigoTipo,
    CodigoAbreviadoTipoOC AS [TipoDescripcion],
    codigoEstado AS [EstadoCodigo],
    Estado AS [EstadoDescripcion],
    codigoEstadoProveedor AS [EstadoProveedorCodigo],
    EstadoProveedor AS [EstadoProveedorDescripcion],
    FechaCreacion AS Fecha,
    FechaEnvio,
    FechaAceptacion,
    MontoTotalOC AS MontoTotal_MonedaOrigen,
    TipoMonedaOC AS [MonedaNombre],
    SUM(Total_CLP) AS MontoTotal_CLP,
    Impuestos,
    Descuentos,
    TotalNetoOC AS MontoTotal_Neto,
    RutUnidadCompra AS [InstitucionRut],
    InstitucionRut_Numero,
    InstitucionRut_DV,
    UnidadCompra,
    [RutSucursal],
    ProveedorRut_Numero,
    ProveedorRut_DV,
    Sucursal AS ProveedorSucursal,
    NombreProveedor AS ProveedorNombre,
    CodigoLicitacion,
    ISNULL(IdOrPortal, '#') AS IdOrPortal,
    3 AS [Origen]
FROM
    {tablaOrigen} AS D
WHERE
    D.Archivo = @Archivo
    AND (D.Codigo <> 'NA')
    AND (D.IDItem IS NOT NULL)
    AND (D.ProveedorRut_Numero IS NOT NULL)
    AND (D.ProveedorRut_Numero <> -1)
    AND (D.ProveedorRut_Numero <> 1234567)
    AND (D.codigoCategoria IS NOT NULL)
    AND (D.Categoria IS NOT NULL)
    AND (D.Total_CLP IS NOT NULL)
    AND (D.codigoProductoONU IS NOT NULL)
    AND (D.NombreroductoGenerico IS NOT NULL)
    AND (D.codigoEstado = 6 OR D.codigoEstado = 12)
    AND NOT EXISTS (
        SELECT 1
        FROM dbo.Excluir_OC AS E
        WHERE E.Codigo = D.Codigo
    )
GROUP BY
    ID,
    [Codigo],
    Link,
    Nombre,
    [Descripcion/Obervaciones],
    CodigoTipo,
    CodigoAbreviadoTipoOC,
    codigoEstado,
    Estado,
    codigoEstadoProveedor,
    EstadoProveedor,
    FechaCreacion,
    FechaEnvio,
    FechaAceptacion,
    MontoTotalOC,
    TipoMonedaOC,
    Impuestos,
    Descuentos,
    TotalNetoOC,
    RutUnidadCompra,
    InstitucionRut_Numero,
    InstitucionRut_DV,
    UnidadCompra,
    [RutSucursal],
    ProveedorRut_Numero,
    ProveedorRut_DV,
    Sucursal,
    NombreProveedor,
    CodigoLicitacion,
    IdOrPortal
ORDER BY
    Codigo;
""";

        await using SqlCommand comando = CrearComando(conexion, sql, periodo);
        await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

        List<OrdenCompraTransparencia> registros = [];

        while (await reader.ReadAsync(cancellationToken))
        {
            registros.Add(new OrdenCompraTransparencia(
                LeerInt64(reader, "ID"),
                LeerString(reader, "Codigo"),
                LeerString(reader, "Link"),
                LeerString(reader, "Nombre"),
                LeerString(reader, "Descripcion"),
                LeerDecimal(reader, "CodigoTipo"),
                LeerString(reader, "TipoDescripcion"),
                LeerDecimal(reader, "EstadoCodigo"),
                LeerString(reader, "EstadoDescripcion"),
                LeerDecimal(reader, "EstadoProveedorCodigo"),
                LeerString(reader, "EstadoProveedorDescripcion"),
                LeerDateTime(reader, "Fecha"),
                LeerDateTime(reader, "FechaEnvio"),
                LeerDateTime(reader, "FechaAceptacion"),
                LeerDecimal(reader, "MontoTotal_MonedaOrigen"),
                LeerString(reader, "MonedaNombre"),
                LeerDecimal(reader, "MontoTotal_CLP"),
                LeerDecimal(reader, "Impuestos"),
                LeerDecimal(reader, "Descuentos"),
                LeerDecimal(reader, "MontoTotal_Neto"),
                LeerString(reader, "InstitucionRut"),
                LeerInt32(reader, "InstitucionRut_Numero"),
                LeerString(reader, "InstitucionRut_DV"),
                LeerString(reader, "UnidadCompra"),
                LeerString(reader, "RutSucursal"),
                LeerInt32(reader, "ProveedorRut_Numero"),
                LeerString(reader, "ProveedorRut_DV"),
                LeerString(reader, "ProveedorSucursal"),
                LeerString(reader, "ProveedorNombre"),
                LeerString(reader, "CodigoLicitacion"),
                LeerString(reader, "IdOrPortal"),
                LeerInt32(reader, "Origen") ?? 3));
        }

        return registros;
    }

    private async Task<IReadOnlyCollection<OrdenCompraDetalleTransparencia>> LeerDetallesAsync(
        SqlConnection conexion,
        string tablaOrigen,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        string sql = $"""
SELECT
    [ID],
    Codigo AS Codigo,
    IDItem AS Correlativo,
    cantidad AS cantidad,
    codigoProductoONU AS CodigoProducto,
    NombreroductoGenerico AS Producto,
    Categoria AS Categoria,
    codigoCategoria AS codigoCategoria,
    monedaItem AS MonedaNombre,
    precioNeto,
    ISNULL(totalImpuestos, 0) AS ImpuestoTotal,
    ISNULL(Descuentos, 0) AS Descuentos,
    totalLineaNeto AS Total,
    Total_CLP,
    FechaEnvio
FROM
    {tablaOrigen} AS D
WHERE
    D.Archivo = @Archivo
    AND (D.Codigo <> 'NA')
    AND (D.IDItem IS NOT NULL)
    AND (D.ProveedorRut_Numero IS NOT NULL)
    AND (D.ProveedorRut_Numero <> -1)
    AND (D.ProveedorRut_Numero <> 1234567)
    AND (D.codigoCategoria IS NOT NULL)
    AND (D.Categoria IS NOT NULL)
    AND (D.Total_CLP IS NOT NULL)
    AND (D.codigoProductoONU IS NOT NULL)
    AND (D.NombreroductoGenerico IS NOT NULL)
    AND (D.codigoEstado = 6 OR D.codigoEstado = 12)
    AND NOT EXISTS (
        SELECT 1
        FROM dbo.Excluir_OC AS E
        WHERE E.Codigo = D.Codigo
    )
ORDER BY
    D.Codigo;
""";

        await using SqlCommand comando = CrearComando(conexion, sql, periodo);
        await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

        List<OrdenCompraDetalleTransparencia> registros = [];

        while (await reader.ReadAsync(cancellationToken))
        {
            registros.Add(new OrdenCompraDetalleTransparencia(
                LeerInt64(reader, "ID"),
                LeerString(reader, "Codigo"),
                LeerInt32(reader, "Correlativo"),
                LeerDecimal(reader, "cantidad"),
                LeerString(reader, "CodigoProducto"),
                LeerString(reader, "Producto"),
                LeerString(reader, "Categoria"),
                LeerString(reader, "codigoCategoria"),
                LeerString(reader, "MonedaNombre"),
                LeerDecimal(reader, "precioNeto"),
                LeerDecimal(reader, "ImpuestoTotal"),
                LeerDecimal(reader, "Descuentos"),
                LeerDecimal(reader, "Total"),
                LeerDecimal(reader, "Total_CLP"),
                LeerDateTime(reader, "FechaEnvio")));
        }

        return registros;
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

    private string ObtenerTablaOrigen(PeriodoCarga periodo)
    {
        string prefijo = opciones.Value.Tablas.PrefijoOCOrigen;
        string tabla = prefijo + periodo.SufijoTablaAnual;

        if (!EsNombreTablaPermitido(tabla))
        {
            throw new InvalidOperationException($"La tabla origen '{tabla}' no tiene un nombre permitido.");
        }

        return tabla;
    }

    private static bool EsNombreTablaPermitido(string tabla) =>
        tabla.Split('.').All(parte => parte.Length > 0 && parte.All(c => char.IsLetterOrDigit(c) || c == '_'));

    private static SqlCommand CrearComando(SqlConnection conexion, string sql, PeriodoCarga periodo)
    {
        SqlCommand comando = conexion.CreateCommand();
        comando.CommandText = sql;
        comando.CommandTimeout = 0;
        comando.Parameters.AddWithValue("@Archivo", $"{periodo.PeriodoArchivo}.csv");
        return comando;
    }

    private static string? LeerString(SqlDataReader reader, string columna)
    {
        int ordinal = reader.GetOrdinal(columna);
        return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal).ToString();
    }

    private static int? LeerInt32(SqlDataReader reader, string columna)
    {
        int ordinal = reader.GetOrdinal(columna);
        return reader.IsDBNull(ordinal) ? null : Convert.ToInt32(reader.GetValue(ordinal));
    }

    private static long? LeerInt64(SqlDataReader reader, string columna)
    {
        int ordinal = reader.GetOrdinal(columna);
        return reader.IsDBNull(ordinal) ? null : Convert.ToInt64(reader.GetValue(ordinal));
    }

    private static decimal? LeerDecimal(SqlDataReader reader, string columna)
    {
        int ordinal = reader.GetOrdinal(columna);
        return reader.IsDBNull(ordinal) ? null : Convert.ToDecimal(reader.GetValue(ordinal));
    }

    private static DateTime? LeerDateTime(SqlDataReader reader, string columna)
    {
        int ordinal = reader.GetOrdinal(columna);
        return reader.IsDBNull(ordinal) ? null : Convert.ToDateTime(reader.GetValue(ordinal));
    }
}
