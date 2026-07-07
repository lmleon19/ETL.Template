namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record OrdenCompraDetalleTransparencia(
    long? ID,
    string? Codigo,
    int? Correlativo,
    decimal? Cantidad,
    string? CodigoProducto,
    string? Producto,
    string? Categoria,
    string? CodigoCategoria,
    string? MonedaNombre,
    decimal? PrecioNeto,
    decimal? ImpuestoTotal,
    decimal? Descuentos,
    decimal? Total,
    decimal? TotalClp,
    DateTime? FechaEnvio);
