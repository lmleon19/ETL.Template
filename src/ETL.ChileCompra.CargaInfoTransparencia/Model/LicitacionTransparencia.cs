namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record LicitacionTransparencia(
    long? Codigo,
    string? CodigoExterno,
    string? Nombre,
    string? InstitucionRut,
    string? InstitucionNombre,
    string? InstitucionIdOrPortal,
    string? LicitacionTipo,
    string? LicitacionMoneda,
    int ItemsCantidad,
    string? AdjudicacionUrlActa,
    DateTime? AdjudicacionFecha,
    DateTime? FechaPublicacion,
    int? NumeroOferentes,
    decimal? ClpTotalAdjudicado,
    decimal? MonedaOrigenTotalAdjudicado);
