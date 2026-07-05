using ETL.ChileCompra.Carga.Model;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class GeneradorUrlsChileCompra
{
    private readonly OpcionesChileCompra opciones;

    public GeneradorUrlsChileCompra(IOptions<OpcionesChileCompra> opciones) => this.opciones = opciones.Value;

    public string ObtenerUrlLicitaciones(PeriodoProceso periodo) => GenerarUrl(opciones.Urls.Licitaciones, periodo);

    public string ObtenerUrlOC(PeriodoProceso periodo) => GenerarUrl(opciones.Urls.OC, periodo);

    public string ObtenerUrlParidadMoneda() => opciones.Urls.ParidadMoneda;

    private static string GenerarUrl(string plantilla, PeriodoProceso periodo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plantilla);

        return plantilla
            .Replace("{anio}", periodo.Anio.ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{mes}", periodo.Mes.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

