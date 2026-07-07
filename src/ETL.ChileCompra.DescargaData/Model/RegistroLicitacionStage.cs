namespace ETL.ChileCompra.DescargaData.Model;

public sealed class RegistroLicitacionStage
{
    [ColumnaStage(1, "Codigo", "float NULL", OrigenColumnaStage.Csv)]
    public double? Codigo { get; set; }

    [ColumnaStage(2, "Link", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Link { get; set; }

    [ColumnaStage(3, "CodigoExterno", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CodigoExterno { get; set; }

    [ColumnaStage(4, "Nombre", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Nombre { get; set; }

    [ColumnaStage(5, "Descripcion", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? Descripcion { get; set; }

    [ColumnaStage(6, "CriteriosRequisitosAmbientales", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CriteriosRequisitosAmbientales { get; set; }

    [ColumnaStage(7, "DescripcionCriteriosRequisitosAmbientales", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionCriteriosRequisitosAmbientales { get; set; }

    [ColumnaStage(8, "CriteriosRequisitosSociales", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CriteriosRequisitosSociales { get; set; }

    [ColumnaStage(9, "DescripcionCriteriosRequisitosSociales", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionCriteriosRequisitosSociales { get; set; }

    [ColumnaStage(10, "DescripcionCriteriosRequisitosSociales.1", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionCriteriosRequisitosSociales1 { get; set; }

    [ColumnaStage(11, "CriteriosEvaluacion", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? CriteriosEvaluacion { get; set; }

    [ColumnaStage(12, "Tipo de Adquisicion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? TipoAdquisicion { get; set; }

    [ColumnaStage(13, "CodigoEstado", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoEstado { get; set; }

    [ColumnaStage(14, "Estado", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Estado { get; set; }

    [ColumnaStage(15, "CodigoOrganismo", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoOrganismo { get; set; }

    [ColumnaStage(16, "NombreOrganismo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreOrganismo { get; set; }

    [ColumnaStage(17, "sector", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Sector { get; set; }

    [ColumnaStage(18, "RutUnidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RutUnidad { get; set; }

    [ColumnaStage(19, "CodigoUnidad", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoUnidad { get; set; }

    [ColumnaStage(20, "NombreUnidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreUnidad { get; set; }

    [ColumnaStage(21, "DireccionUnidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? DireccionUnidad { get; set; }

    [ColumnaStage(22, "ComunaUnidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? ComunaUnidad { get; set; }

    [ColumnaStage(23, "RegionUnidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RegionUnidad { get; set; }

    [ColumnaStage(24, "Informada", "float NULL", OrigenColumnaStage.Csv)]
    public double? Informada { get; set; }

    [ColumnaStage(25, "CodigoTipo", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoTipo { get; set; }

    [ColumnaStage(26, "Tipo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Tipo { get; set; }

    [ColumnaStage(27, "TipoConvocatoria", "float NULL", OrigenColumnaStage.Csv)]
    public double? TipoConvocatoria { get; set; }

    [ColumnaStage(28, "CodigoMoneda", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? CodigoMoneda { get; set; }

    [ColumnaStage(29, "Moneda Adquisicion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? MonedaAdquisicion { get; set; }

    [ColumnaStage(30, "Etapas", "float NULL", OrigenColumnaStage.Csv)]
    public double? Etapas { get; set; }

    [ColumnaStage(31, "EstadoEtapas", "float NULL", OrigenColumnaStage.Csv)]
    public double? EstadoEtapas { get; set; }

    [ColumnaStage(32, "TomaRazon", "float NULL", OrigenColumnaStage.Csv)]
    public double? TomaRazon { get; set; }

    [ColumnaStage(33, "EstadoPublicidadOfertas", "float NULL", OrigenColumnaStage.Csv)]
    public double? EstadoPublicidadOfertas { get; set; }

    [ColumnaStage(34, "JustificacionPublicidad", "nvarchar(600) NULL", OrigenColumnaStage.Csv)]
    public string? JustificacionPublicidad { get; set; }

    [ColumnaStage(35, "EstadoCS", "float NULL", OrigenColumnaStage.Csv)]
    public double? EstadoCS { get; set; }

    [ColumnaStage(36, "Contrato", "float NULL", OrigenColumnaStage.Csv)]
    public double? Contrato { get; set; }

    [ColumnaStage(37, "Obras", "float NULL", OrigenColumnaStage.Csv)]
    public double? Obras { get; set; }

    [ColumnaStage(38, "CantidadReclamos", "float NULL", OrigenColumnaStage.Csv)]
    public double? CantidadReclamos { get; set; }

    [ColumnaStage(39, "FechaCreacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaCreacion { get; set; }

    [ColumnaStage(40, "FechaCierre", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaCierre { get; set; }

    [ColumnaStage(41, "FechaInicio", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaInicio { get; set; }

    [ColumnaStage(42, "FechaFinal", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaFinal { get; set; }

    [ColumnaStage(43, "FechaPubRespuestas", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaPubRespuestas { get; set; }

    [ColumnaStage(44, "FechaActoAperturaTecnica", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaActoAperturaTecnica { get; set; }

    [ColumnaStage(45, "FechaActoAperturaEconomica", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaActoAperturaEconomica { get; set; }

    [ColumnaStage(46, "FechaPublicacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaPublicacion { get; set; }

    [ColumnaStage(47, "FechaAdjudicacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaAdjudicacion { get; set; }

    [ColumnaStage(48, "FechaEstimadaAdjudicacion", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaEstimadaAdjudicacion { get; set; }

    [ColumnaStage(49, "FechaSoporteFisico", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaSoporteFisico { get; set; }

    [ColumnaStage(50, "FechaTiempoEvaluacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? FechaTiempoEvaluacion { get; set; }

    [ColumnaStage(51, "UnidadTiempoEvaluacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? UnidadTiempoEvaluacion { get; set; }

    [ColumnaStage(52, "FechaEstimadaFirma", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaEstimadaFirma { get; set; }

    [ColumnaStage(53, "FechasUsuario", "float NULL", OrigenColumnaStage.Csv)]
    public double? FechasUsuario { get; set; }

    [ColumnaStage(54, "FechaVisitaTerreno", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaVisitaTerreno { get; set; }

    [ColumnaStage(55, "DireccionVisita", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? DireccionVisita { get; set; }

    [ColumnaStage(56, "FechaEntregaAntecedentes", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaEntregaAntecedentes { get; set; }

    [ColumnaStage(57, "DireccionEntrega", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? DireccionEntrega { get; set; }

    [ColumnaStage(58, "Estimacion", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? Estimacion { get; set; }

    [ColumnaStage(59, "FuenteFinanciamiento", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? FuenteFinanciamiento { get; set; }

    [ColumnaStage(60, "VisibilidadMonto", "float NULL", OrigenColumnaStage.Csv)]
    public double? VisibilidadMonto { get; set; }

    [ColumnaStage(61, "MontoEstimado", "float NULL", OrigenColumnaStage.Csv)]
    public double? MontoEstimado { get; set; }

    [ColumnaStage(62, "Tiempo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Tiempo { get; set; }

    [ColumnaStage(63, "UnidadTiempo", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? UnidadTiempo { get; set; }

    [ColumnaStage(64, "Modalidad", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Modalidad { get; set; }

    [ColumnaStage(65, "TipoPago", "float NULL", OrigenColumnaStage.Csv)]
    public double? TipoPago { get; set; }

    [ColumnaStage(66, "ProhibicionContratacion", "nvarchar(400) NULL", OrigenColumnaStage.Csv)]
    public string? ProhibicionContratacion { get; set; }

    [ColumnaStage(67, "SubContratacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? SubContratacion { get; set; }

    [ColumnaStage(68, "UnidadTiempoDuracionContrato", "float NULL", OrigenColumnaStage.Csv)]
    public double? UnidadTiempoDuracionContrato { get; set; }

    [ColumnaStage(69, "TiempoDuracionContrato", "float NULL", OrigenColumnaStage.Csv)]
    public double? TiempoDuracionContrato { get; set; }

    [ColumnaStage(70, "TipoDuracionContrato", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? TipoDuracionContrato { get; set; }

    [ColumnaStage(71, "JustificacionMontoEstimado", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? JustificacionMontoEstimado { get; set; }

    [ColumnaStage(72, "ObservacionContrato", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? ObservacionContrato { get; set; }

    [ColumnaStage(73, "ExtensionPlazo", "float NULL", OrigenColumnaStage.Csv)]
    public double? ExtensionPlazo { get; set; }

    [ColumnaStage(74, "EsBaseTipo", "float NULL", OrigenColumnaStage.Csv)]
    public double? EsBaseTipo { get; set; }

    [ColumnaStage(75, "UnidadTiempoContratoLicitacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? UnidadTiempoContratoLicitacion { get; set; }

    [ColumnaStage(76, "ValorTiempoRenovacion", "float NULL", OrigenColumnaStage.Pendiente)]
    public double? ValorTiempoRenovacion { get; set; }

    [ColumnaStage(77, "PeriodoTiempoRenovacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? PeriodoTiempoRenovacion { get; set; }

    [ColumnaStage(78, "EsRenovable", "float NULL", OrigenColumnaStage.Csv)]
    public double? EsRenovable { get; set; }

    [ColumnaStage(79, "TipoAprobacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? TipoAprobacion { get; set; }

    [ColumnaStage(80, "NumeroAprobacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NumeroAprobacion { get; set; }

    [ColumnaStage(81, "FechaAprobacion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? FechaAprobacion { get; set; }

    [ColumnaStage(82, "NumeroOferentes", "float NULL", OrigenColumnaStage.Csv)]
    public double? NumeroOferentes { get; set; }

    [ColumnaStage(83, "Correlativo", "float NULL", OrigenColumnaStage.Csv)]
    public double? Correlativo { get; set; }

    [ColumnaStage(84, "CodigoEstadoLicitacion", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoEstadoLicitacion { get; set; }

    [ColumnaStage(85, "Codigoitem", "float NULL", OrigenColumnaStage.Csv)]
    public double? Codigoitem { get; set; }

    [ColumnaStage(86, "CodigoProductoONU", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoProductoONU { get; set; }

    [ColumnaStage(87, "Rubro1", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Rubro1 { get; set; }

    [ColumnaStage(88, "Rubro2", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Rubro2 { get; set; }

    [ColumnaStage(89, "Rubro3", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? Rubro3 { get; set; }

    [ColumnaStage(90, "Nombre producto genrico", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreProductoGenerico { get; set; }

    [ColumnaStage(91, "Nombre linea Adquisicion", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreLineaAdquisicion { get; set; }

    [ColumnaStage(92, "Descripcion linea Adquisicion", "nvarchar(500) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionLineaAdquisicion { get; set; }

    [ColumnaStage(93, "UnidadMedida", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? UnidadMedida { get; set; }

    [ColumnaStage(94, "Cantidad", "float NULL", OrigenColumnaStage.Csv)]
    public double? Cantidad { get; set; }

    [ColumnaStage(95, "CodigoProveedor", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoProveedor { get; set; }

    [ColumnaStage(96, "CodigoSucursalProveedor", "float NULL", OrigenColumnaStage.Csv)]
    public double? CodigoSucursalProveedor { get; set; }

    [ColumnaStage(97, "RutProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RutProveedor { get; set; }

    [ColumnaStage(98, "NombreProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreProveedor { get; set; }

    [ColumnaStage(99, "RazonSocialProveedor", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? RazonSocialProveedor { get; set; }

    [ColumnaStage(100, "DescripcionProveedor", "nvarchar(max) NULL", OrigenColumnaStage.Csv)]
    public string? DescripcionProveedor { get; set; }

    [ColumnaStage(101, "Monto Estimado Adjudicado", "nvarchar(400) NULL", OrigenColumnaStage.Csv)]
    public string? MontoEstimadoAdjudicado { get; set; }

    [ColumnaStage(102, "Nombre de la Oferta", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? NombreOferta { get; set; }

    [ColumnaStage(103, "Estado Oferta", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? EstadoOferta { get; set; }

    [ColumnaStage(104, "Cantidad Ofertada", "float NULL", OrigenColumnaStage.Csv)]
    public double? CantidadOfertada { get; set; }

    [ColumnaStage(105, "Moneda de la Oferta", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? MonedaOferta { get; set; }

    [ColumnaStage(106, "MontoUnitarioOferta", "float NULL", OrigenColumnaStage.Csv)]
    public double? MontoUnitarioOferta { get; set; }

    [ColumnaStage(107, "Valor Total Ofertado", "float NULL", OrigenColumnaStage.Csv)]
    public double? ValorTotalOfertado { get; set; }

    [ColumnaStage(108, "CantidadAdjudicada", "float NULL", OrigenColumnaStage.Csv)]
    public double? CantidadAdjudicada { get; set; }

    [ColumnaStage(109, "MontoLineaAdjudica", "float NULL", OrigenColumnaStage.Csv)]
    public double? MontoLineaAdjudica { get; set; }

    [ColumnaStage(110, "FechaEnvioOferta", "datetime NULL", OrigenColumnaStage.Csv)]
    public DateTime? FechaEnvioOferta { get; set; }

    [ColumnaStage(111, "Oferta seleccionada", "nvarchar(255) NULL", OrigenColumnaStage.Csv)]
    public string? OfertaSeleccionada { get; set; }

    [ColumnaStage(112, "Archivo", "nvarchar(250) NULL", OrigenColumnaStage.Metadato)]
    public string? Archivo { get; set; }

    [ColumnaStage(113, "InstitucionIdOrPortal", "nvarchar(5) NULL", OrigenColumnaStage.Calculado)]
    public string? InstitucionIdOrPortal { get; set; }

    [ColumnaStage(114, "MonedaOrigen_Total_Adjudicado", "float NULL", OrigenColumnaStage.Calculado)]
    public double? MonedaOrigenTotalAdjudicado { get; set; }

    [ColumnaStage(115, "ProveedorRut_Numero", "int NULL", OrigenColumnaStage.Calculado)]
    public int? ProveedorRutNumero { get; set; }

    [ColumnaStage(116, "CLP_Total_Adjudicado", "bigint NULL", OrigenColumnaStage.Calculado)]
    public long? ClpTotalAdjudicado { get; set; }

    [ColumnaStage(117, "RutProveedor2", "nvarchar(255) NULL", OrigenColumnaStage.Calculado)]
    public string? RutProveedor2 { get; set; }
}
