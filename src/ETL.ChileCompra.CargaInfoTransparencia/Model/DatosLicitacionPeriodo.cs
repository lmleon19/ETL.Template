namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record DatosLicitacionPeriodo(
    PeriodoCarga Periodo,
    IReadOnlyCollection<LicitacionTransparencia> Licitaciones,
    IReadOnlyCollection<LicitacionDetalleTransparencia> Detalles);
