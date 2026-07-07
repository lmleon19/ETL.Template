/*
Consultas base usadas por ETL.ChileCompra.CargaInfoTransparencia para ordenes de compra.

El codigo reemplaza:
- {TablaOrigen}: tabla anual de ChileCompra, por ejemplo dbo.DatosAbiertos_OC_26.
- @Archivo: archivo mensual, por ejemplo 2026-3.csv.
*/

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
    {TablaOrigen} AS D
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
    {TablaOrigen} AS D
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
