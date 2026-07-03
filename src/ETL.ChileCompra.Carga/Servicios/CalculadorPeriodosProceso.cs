using ETL.ChileCompra.Carga.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class CalculadorPeriodosProceso
{
    private readonly OpcionesChileCompra opciones;
    private readonly ILogger<CalculadorPeriodosProceso> logger;

    public CalculadorPeriodosProceso(IOptions<OpcionesChileCompra> opciones, ILogger<CalculadorPeriodosProceso> logger)
    {
        this.opciones = opciones.Value;
        this.logger = logger;
    }

    public IReadOnlyList<PeriodoProceso> Calcular(DateOnly fechaReferencia)
    {
        if (opciones.MesesCerradosACargar <= 0)
        {
            throw new InvalidOperationException("La cantidad de meses cerrados a cargar debe ser mayor que cero.");
        }

        DateOnly primerDiaMesActual = new(fechaReferencia.Year, fechaReferencia.Month, 1);

        PeriodoProceso[] periodos = Enumerable
            .Range(1, opciones.MesesCerradosACargar)
            .Select(desplazamiento => primerDiaMesActual.AddMonths(-desplazamiento))
            .OrderBy(fecha => fecha)
            .Select(fecha => new PeriodoProceso(fecha.Year, fecha.Month))
            .ToArray();

        logger.LogInformation(
            "Periodos calculados: {Periodos}.",
            string.Join(", ", periodos.Select(p => $"{p.Anio}-{p.Mes:00}")));

        return periodos;
    }
}
