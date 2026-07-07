namespace ETL.ChileCompra.DescargaData.Model;

public sealed class RegistroOrdenCompraStage
{
    [ColumnaStage(1, "ID", "int NULL", OrigenColumnaStage.Csv)]
    public int? Id { get; set; }

    [ColumnaStage(2, "Codigo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Codigo { get; set; }

    [ColumnaStage(3, "Link", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Link { get; set; }

    [ColumnaStage(4, "Nombre", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Nombre { get; set; }

    [ColumnaStage(5, "Descripcion/Obervaciones", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionObervaciones { get; set; }

    [ColumnaStage(6, "Tipo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Tipo { get; set; }

    [ColumnaStage(7, "ProcedenciaOC", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? ProcedenciaOC { get; set; }

    [ColumnaStage(8, "EsTratoDirecto", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? EsTratoDirecto { get; set; }

    [ColumnaStage(9, "EsCompraAgil", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? EsCompraAgil { get; set; }

    [ColumnaStage(10, "CodigoTipo", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoTipo { get; set; }

    [ColumnaStage(11, "CodigoAbreviadoTipoOC", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CodigoAbreviadoTipoOC { get; set; }

    [ColumnaStage(12, "DescripcionTipoOC", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionTipoOC { get; set; }

    [ColumnaStage(13, "idPlanDeCompra", "nvarchar(255) NULL", OrigenColumnaStage.Pendiente)]
    public string? IdPlanDeCompra { get; set; }

    [ColumnaStage(14, "codigoEstado", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoEstado { get; set; }

    [ColumnaStage(15, "Estado", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Estado { get; set; }

    [ColumnaStage(16, "codigoEstadoProveedor", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoEstadoProveedor { get; set; }

    [ColumnaStage(17, "EstadoProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? EstadoProveedor { get; set; }

    [ColumnaStage(18, "FechaCreacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaCreacion { get; set; }

    [ColumnaStage(19, "FechaEnvio", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaEnvio { get; set; }

    [ColumnaStage(20, "FechaSolicitudCancelacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaSolicitudCancelacion { get; set; }

    [ColumnaStage(21, "fechaUltimaModificacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ColumnaStage(22, "FechaAceptacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaAceptacion { get; set; }

    [ColumnaStage(23, "FechaCancelacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaCancelacion { get; set; }

    [ColumnaStage(24, "tieneItems", "float NULL", OrigenColumnaStage.Csv)]
    public double? TieneItems { get; set; }

    [ColumnaStage(25, "PromedioCalificacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? PromedioCalificacion { get; set; }

    [ColumnaStage(26, "CantidadEvaluacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? CantidadEvaluacion { get; set; }

    [ColumnaStage(27, "MontoTotalOC", "float NULL", OrigenColumnaStage.Csv)]
    public double? MontoTotalOC { get; set; }

    [ColumnaStage(28, "TipoMonedaOC", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? TipoMonedaOC { get; set; }

    [ColumnaStage(29, "MontoTotalOC_PesosChilenos", "float NULL", OrigenColumnaStage.Csv)]
    public double? MontoTotalOCPesosChilenos { get; set; }

    [ColumnaStage(30, "Impuestos", "float NULL", OrigenColumnaStage.Csv)]
    public double? Impuestos { get; set; }

    [ColumnaStage(31, "TipoImpuesto", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? TipoImpuesto { get; set; }

    [ColumnaStage(32, "Descuentos", "float NULL", OrigenColumnaStage.Csv)]
    public double? Descuentos { get; set; }

    [ColumnaStage(33, "Cargos", "float NULL", OrigenColumnaStage.Csv)]
    public double? Cargos { get; set; }

    [ColumnaStage(34, "TotalNetoOC", "float NULL", OrigenColumnaStage.Csv)]
    public double? TotalNetoOC { get; set; }

    [ColumnaStage(35, "CodigoUnidadCompra", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoUnidadCompra { get; set; }

    [ColumnaStage(36, "RutUnidadCompra", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RutUnidadCompra { get; set; }

    [ColumnaStage(37, "UnidadCompra", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? UnidadCompra { get; set; }

    [ColumnaStage(38, "CodigoOrganismoPublico", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoOrganismoPublico { get; set; }

    [ColumnaStage(39, "OrganismoPublico", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? OrganismoPublico { get; set; }

    [ColumnaStage(40, "ActividadComprador", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? ActividadComprador { get; set; }

    [ColumnaStage(41, "CiudadUnidadCompra", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CiudadUnidadCompra { get; set; }

    [ColumnaStage(42, "RegionUnidadCompra", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RegionUnidadCompra { get; set; }

    [ColumnaStage(43, "PaisUnidadCompra", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? PaisUnidadCompra { get; set; }

    [ColumnaStage(44, "CodigoSucursal", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoSucursal { get; set; }

    [ColumnaStage(45, "RutSucursal", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RutSucursal { get; set; }

    [ColumnaStage(46, "Sucursal", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Sucursal { get; set; }

    [ColumnaStage(47, "CodigoProveedor", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoProveedor { get; set; }

    [ColumnaStage(48, "NombreProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreProveedor { get; set; }

    [ColumnaStage(49, "ActividadProveedor", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? ActividadProveedor { get; set; }

    [ColumnaStage(50, "ComunaProveedor", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? ComunaProveedor { get; set; }

    [ColumnaStage(51, "RegionProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RegionProveedor { get; set; }

    [ColumnaStage(52, "PaisProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? PaisProveedor { get; set; }

    [ColumnaStage(53, "Financiamiento", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Financiamiento { get; set; }

    [ColumnaStage(54, "PorcentajeIva", "float NULL", OrigenColumnaStage.Csv)]
    public double? PorcentajeIva { get; set; }

    [ColumnaStage(55, "Pais", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Pais { get; set; }

    [ColumnaStage(56, "TipoDespacho", "float NULL", OrigenColumnaStage.Csv)]
    public double? TipoDespacho { get; set; }

    [ColumnaStage(57, "FormaPago", "float NULL", OrigenColumnaStage.Csv)]
    public double? FormaPago { get; set; }

    [ColumnaStage(58, "CodigoLicitacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CodigoLicitacion { get; set; }

    [ColumnaStage(59, "IDItem", "float NULL", OrigenColumnaStage.Csv)]
    public double? IDItem { get; set; }

    [ColumnaStage(60, "codigoCategoria", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoCategoria { get; set; }

    [ColumnaStage(61, "Categoria", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? Categoria { get; set; }

    [ColumnaStage(62, "codigoProductoONU", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoProductoONU { get; set; }

    [ColumnaStage(63, "NombreroductoGenerico", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreProductoGenerico { get; set; }

    [ColumnaStage(64, "RubroN1", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RubroN1 { get; set; }

    [ColumnaStage(65, "RubroN2", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RubroN2 { get; set; }

    [ColumnaStage(66, "RubroN3", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RubroN3 { get; set; }

    [ColumnaStage(67, "EspecificacionComprador", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? EspecificacionComprador { get; set; }

    [ColumnaStage(68, "EspecificacionProveedor", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? EspecificacionProveedor { get; set; }

    [ColumnaStage(69, "cantidad", "float NULL", OrigenColumnaStage.Csv)]
    public double? Cantidad { get; set; }

    [ColumnaStage(70, "UnidadMedida", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? UnidadMedida { get; set; }

    [ColumnaStage(71, "monedaItem", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? MonedaItem { get; set; }

    [ColumnaStage(72, "precioNeto", "float NULL", OrigenColumnaStage.Csv)]
    public double? PrecioNeto { get; set; }

    [ColumnaStage(73, "totalCargos", "float NULL", OrigenColumnaStage.Csv)]
    public double? TotalCargos { get; set; }

    [ColumnaStage(74, "totalDescuentos", "float NULL", OrigenColumnaStage.Csv)]
    public double? TotalDescuentos { get; set; }

    [ColumnaStage(75, "totalImpuestos", "float NULL", OrigenColumnaStage.Csv)]
    public double? TotalImpuestos { get; set; }

    [ColumnaStage(76, "totalLineaNeto", "float NULL", OrigenColumnaStage.Csv)]
    public double? TotalLineaNeto { get; set; }

    [ColumnaStage(77, "Forma de Pago", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? FormaDePago { get; set; }

    [ColumnaStage(78, "Archivo", "nvarchar(250) NULL", OrigenColumnaStage.Metadato)]
    public string? Archivo { get; set; }

    [ColumnaStage(79, "ProveedorRut_Numero", "int NULL", OrigenColumnaStage.Calculado)]
    public int? ProveedorRutNumero { get; set; }

    [ColumnaStage(80, "ProveedorRut_DV", "nvarchar(10) NULL", OrigenColumnaStage.Calculado)]
    public string? ProveedorRutDv { get; set; }

    [ColumnaStage(81, "InstitucionRut_Numero", "int NULL", OrigenColumnaStage.Calculado)]
    public int? InstitucionRutNumero { get; set; }

    [ColumnaStage(82, "InstitucionRut_DV", "nvarchar(10) NULL", OrigenColumnaStage.Calculado)]
    public string? InstitucionRutDv { get; set; }

    [ColumnaStage(83, "IdOrPortal", "nvarchar(5) NULL", OrigenColumnaStage.Calculado)]
    public string? IdOrPortal { get; set; }

    [ColumnaStage(84, "Total_CLP", "bigint NULL", OrigenColumnaStage.Calculado)]
    public long? TotalClp { get; set; }

    [ColumnaStage(85, "Codigo_ConvenioMarco", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CodigoConvenioMarco { get; set; }

    [ColumnaStage(86, "sector", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Sector { get; set; }

    [ColumnaStage(87, "RutSucursal2", "nvarchar(255) NULL", OrigenColumnaStage.Calculado)]
    public string? RutSucursal2 { get; set; }
}
