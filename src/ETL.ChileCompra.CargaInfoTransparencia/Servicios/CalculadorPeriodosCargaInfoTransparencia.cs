using System.Globalization;
using ETL.ChileCompra.CargaInfoTransparencia.Model;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.CargaInfoTransparencia.Servicios;

public sealed class CalculadorPeriodosCargaInfoTransparencia(IOptions<OpcionesCargaInfoTransparencia> opciones)
{
    public IReadOnlyCollection<PeriodoCarga> Calcular(DateOnly fechaProceso)
    {
        int meses = opciones.Value.MesesCerradosACargar;

        if (meses <= 0)
        {
            throw new InvalidOperationException("MesesCerradosACargar debe ser mayor que cero.");
        }

        DateOnly primerDiaMesActual = new(fechaProceso.Year, fechaProceso.Month, 1);
        List<PeriodoCarga> periodos = [];

        for (int indice = 1; indice <= meses; indice++)
        {
            DateOnly inicioPeriodo = primerDiaMesActual.AddMonths(-indice);
            DateOnly finPeriodo = inicioPeriodo.AddMonths(1);

            periodos.Add(new PeriodoCarga(
                inicioPeriodo.Year,
                inicioPeriodo.Month,
                FormatearPeriodoArchivo(inicioPeriodo),
                FormatearSufijoTabla(inicioPeriodo.Year),
                inicioPeriodo.ToDateTime(TimeOnly.MinValue),
                finPeriodo.ToDateTime(TimeOnly.MinValue)));
        }

        return periodos;
    }

    private string FormatearSufijoTabla(int anio)
    {
        string formato = opciones.Value.FormatoSufijoTablaAnual.Trim();

        return formato switch
        {
            "yy" => (anio % 100).ToString("00", CultureInfo.InvariantCulture),
            "yyyy" => anio.ToString("0000", CultureInfo.InvariantCulture),
            _ => throw new InvalidOperationException("FormatoSufijoTablaAnual debe ser 'yy' o 'yyyy'.")
        };
    }

    private static string FormatearPeriodoArchivo(DateOnly periodo) =>
        string.Create(CultureInfo.InvariantCulture, $"{periodo.Year}-{periodo.Month}");
}
