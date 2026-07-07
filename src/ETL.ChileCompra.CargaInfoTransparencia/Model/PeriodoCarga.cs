namespace ETL.ChileCompra.CargaInfoTransparencia.Model;

public sealed record PeriodoCarga(
    int Anio,
    int Mes,
    string PeriodoArchivo,
    string SufijoTablaAnual,
    DateTime FechaDesde,
    DateTime FechaHasta);
