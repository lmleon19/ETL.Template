using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class RepositorioLicitacionesChileCompra(
    IConfiguration configuration,
    IOptions<OpcionesCargaInfoTransparencia> opciones,
    ILogger<RepositorioLicitacionesChileCompra> logger)
{
    public async Task<DatosLicitacionPeriodo> ObtenerAsync(PeriodoCarga periodo, CancellationToken cancellationToken = default)
    {
        string cadenaConexion = ObtenerCadenaConexion("ChileCompra");
        string tablaOrigen = ObtenerTablaOrigen(periodo);

        logger.LogInformation("Leyendo licitaciones desde {TablaOrigen} para lic_{Periodo}.csv.", tablaOrigen, periodo.PeriodoArchivo);

        await using SqlConnection conexion = new(cadenaConexion);
        await conexion.OpenAsync(cancellationToken);

        IReadOnlyCollection<LicitacionTransparencia> licitaciones = await LeerLicitacionesAsync(conexion, tablaOrigen, periodo, cancellationToken);
        IReadOnlyCollection<LicitacionDetalleTransparencia> detalles = await LeerDetallesAsync(conexion, tablaOrigen, periodo, cancellationToken);

        logger.LogInformation(
            "Lectura de licitaciones finalizada para {Periodo}. Cabeceras: {Cabeceras}. Detalles: {Detalles}.",
            periodo.PeriodoArchivo,
            licitaciones.Count,
            detalles.Count);

        return new DatosLicitacionPeriodo(periodo, licitaciones, detalles);
    }

    private async Task<IReadOnlyCollection<LicitacionTransparencia>> LeerLicitacionesAsync(
        SqlConnection conexion,
        string tablaOrigen,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        string sql = $"""
WITH CTE_Items_Cantidad AS (
    SELECT
        Codigo,
        COUNT(*) AS Items_Cantidad
    FROM
        {tablaOrigen} AS L
    WHERE
        L.Archivo = @Archivo AND L.[Oferta seleccionada] = 'Seleccionada'
        AND NOT EXISTS (
            SELECT 1
            FROM dbo.Excluir_Licitaciones AS E
            WHERE E.CodigoExterno = L.CodigoExterno
        )
    GROUP BY
        L.Codigo
)
SELECT
    D.Codigo,
    D.CodigoExterno,
    D.Nombre,
    D.RutUnidad AS InstitucionRut,
    D.NombreOrganismo AS InstitucionNombre,
    D.InstitucionIdOrPortal,
    D.Tipo AS LicitacionTipo,
    D.CodigoMoneda AS LicitacionMoneda,
    ISNULL(I.Items_Cantidad, 0) AS Items_Cantidad,
    D.Link AS Adjudicacion_UrlActa,
    D.FechaAdjudicacion AS Adjudicacion_Fecha,
    D.FechaPublicacion AS Fecha_Publicacion,
    D.NumeroOferentes,
    SUM(ISNULL(D.CLP_total_Adjudicado, 0)) AS CLP_total_Adjudicado,
    SUM(ISNULL(D.MontoLineaAdjudica, 0)) AS MonedaOrigen_Total_Adjudicado
FROM
    {tablaOrigen} AS D
LEFT JOIN
    CTE_Items_Cantidad AS I ON D.Codigo = I.Codigo
WHERE
    Archivo = @Archivo
    AND D.[Oferta seleccionada] = 'Seleccionada'
    AND D.NombreProveedor IS NOT NULL
    AND D.[Monto Estimado Adjudicado] IS NOT NULL
    AND D.ProveedorRut_Numero IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM dbo.Excluir_Licitaciones AS E
        WHERE E.CodigoExterno = D.CodigoExterno
    )
GROUP BY
    D.Codigo,
    D.CodigoExterno,
    D.Nombre,
    D.RutUnidad,
    D.NombreOrganismo,
    D.InstitucionIdOrPortal,
    D.Tipo,
    D.CodigoMoneda,
    D.Link,
    D.FechaAdjudicacion,
    D.FechaPublicacion,
    D.NumeroOferentes,
    I.Items_Cantidad
ORDER BY
    CodigoExterno;
""";

        await using SqlCommand comando = CrearComando(conexion, sql, periodo);
        await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

        List<LicitacionTransparencia> registros = [];

        while (await reader.ReadAsync(cancellationToken))
        {
            registros.Add(new LicitacionTransparencia(
                LeerInt64(reader, "Codigo"),
                LeerString(reader, "CodigoExterno"),
                LeerString(reader, "Nombre"),
                LeerString(reader, "InstitucionRut"),
                LeerString(reader, "InstitucionNombre"),
                LeerString(reader, "InstitucionIdOrPortal"),
                LeerString(reader, "LicitacionTipo"),
                LeerString(reader, "LicitacionMoneda"),
                LeerInt32(reader, "Items_Cantidad") ?? 0,
                LeerString(reader, "Adjudicacion_UrlActa"),
                LeerDateTime(reader, "Adjudicacion_Fecha"),
                LeerDateTime(reader, "Fecha_Publicacion"),
                LeerInt32(reader, "NumeroOferentes"),
                LeerDecimal(reader, "CLP_total_Adjudicado"),
                LeerDecimal(reader, "MonedaOrigen_Total_Adjudicado")));
        }

        return registros;
    }

    private async Task<IReadOnlyCollection<LicitacionDetalleTransparencia>> LeerDetallesAsync(
        SqlConnection conexion,
        string tablaOrigen,
        PeriodoCarga periodo,
        CancellationToken cancellationToken)
    {
        string sql = $"""
SELECT
    Codigo,
    CodigoExterno,
    Correlativo,
    CantidadAdjudicada AS Cantidad,
    MontoUnitarioOferta AS MontoUnitario,
    CAST(CodigoProductoONU AS NVARCHAR(32)) AS ProductoCodigo,
    [Nombre producto genrico] AS ProductoNombre,
    CAST(CodigoProductoONU AS NVARCHAR(32)) AS CategoriaCodigo,
    Rubro1 + ' / ' + Rubro2 + ' / ' + Rubro3 AS CategoriaNombre,
    RutProveedor AS ProveedorRut,
    ProveedorRut_Numero,
    RazonSocialProveedor AS ProveedorNombre,
    MontoLineaAdjudica AS TotalAdjudicado,
    CLP_total_Adjudicado,
    FechaPublicacion
FROM
    {tablaOrigen} AS D
WHERE
    D.Archivo = @Archivo
    AND D.[Oferta seleccionada] = 'Seleccionada'
    AND D.NombreProveedor IS NOT NULL
    AND D.[Monto Estimado Adjudicado] IS NOT NULL
    AND D.ProveedorRut_Numero IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM dbo.Excluir_Licitaciones AS E
        WHERE E.CodigoExterno = D.CodigoExterno
    )
ORDER BY
    D.CodigoExterno;
""";

        await using SqlCommand comando = CrearComando(conexion, sql, periodo);
        await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

        List<LicitacionDetalleTransparencia> registros = [];

        while (await reader.ReadAsync(cancellationToken))
        {
            registros.Add(new LicitacionDetalleTransparencia(
                LeerInt64(reader, "Codigo"),
                LeerString(reader, "CodigoExterno"),
                LeerInt32(reader, "Correlativo"),
                LeerDecimal(reader, "Cantidad"),
                LeerDecimal(reader, "MontoUnitario"),
                LeerString(reader, "ProductoCodigo"),
                LeerString(reader, "ProductoNombre"),
                LeerString(reader, "CategoriaCodigo"),
                LeerString(reader, "CategoriaNombre"),
                LeerString(reader, "ProveedorRut"),
                LeerInt32(reader, "ProveedorRut_Numero"),
                LeerString(reader, "ProveedorNombre"),
                LeerDecimal(reader, "TotalAdjudicado"),
                LeerDecimal(reader, "CLP_total_Adjudicado"),
                LeerDateTime(reader, "FechaPublicacion")));
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
        string prefijo = opciones.Value.Tablas.PrefijoLicitacionesOrigen;
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
        comando.Parameters.AddWithValue("@Archivo", $"lic_{periodo.PeriodoArchivo}.csv");
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
