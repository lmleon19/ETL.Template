/*
Consultas base usadas por ETL.ChileCompra.CargaInfoTransparencia para licitaciones.

El codigo reemplaza:
- {TablaOrigen}: tabla anual de ChileCompra, por ejemplo dbo.DatosAbiertos_Licitaciones_26.
- @Archivo: archivo mensual, por ejemplo lic_2026-3.csv.
*/

WITH CTE_Items_Cantidad AS (
    SELECT
        Codigo,
        COUNT(*) AS Items_Cantidad
    FROM
        {TablaOrigen} AS L
    WHERE
        L.Archivo = @Archivo
        AND L.[Oferta seleccionada] = 'Seleccionada'
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
    {TablaOrigen} AS D
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
    {TablaOrigen} AS D
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
