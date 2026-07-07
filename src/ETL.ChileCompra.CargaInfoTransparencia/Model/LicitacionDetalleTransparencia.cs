namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record LicitacionDetalleTransparencia(
    long? Codigo,
    string? CodigoExterno,
    int? Correlativo,
    decimal? Cantidad,
    decimal? MontoUnitario,
    string? ProductoCodigo,
    string? ProductoNombre,
    string? CategoriaCodigo,
    string? CategoriaNombre,
    string? ProveedorRut,
    int? ProveedorRutNumero,
    string? ProveedorNombre,
    decimal? TotalAdjudicado,
    decimal? ClpTotalAdjudicado,
    DateTime? FechaPublicacion);
