namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record DatosOCPeriodo(
    PeriodoCarga Periodo,
    IReadOnlyCollection<OrdenCompraTransparencia> OrdenesCompra,
    IReadOnlyCollection<OrdenCompraDetalleTransparencia> Detalles);
