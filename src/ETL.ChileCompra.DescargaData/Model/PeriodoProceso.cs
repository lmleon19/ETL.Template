namespace ETL.ChileCompra.DescargaData.Model;

public sealed record PeriodoProceso(int Anio, int Mes)
{
    public DateOnly FechaInicio => new(Anio, Mes, 1);
    public DateOnly FechaFinExclusiva => FechaInicio.AddMonths(1);
    public string SufijoAnio => (Anio % 100).ToString("00");
    public string NombreArchivoZip => $"{Anio}-{Mes}.zip";
}
