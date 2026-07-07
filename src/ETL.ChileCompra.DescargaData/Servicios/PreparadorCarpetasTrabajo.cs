using ETL.ChileCompra.DescargaData.Model;
using ETL.Common.Servicios;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class PreparadorCarpetasTrabajo
{
    private readonly OpcionesChileCompra opciones;
    private readonly SistemaArchivos sistemaArchivos;
    private readonly ILogger<PreparadorCarpetasTrabajo> logger;

    public PreparadorCarpetasTrabajo(
        IOptions<OpcionesChileCompra> opciones,
        SistemaArchivos sistemaArchivos,
        ILogger<PreparadorCarpetasTrabajo> logger)
    {
        this.opciones = opciones.Value;
        this.sistemaArchivos = sistemaArchivos;
        this.logger = logger;
    }

    public void Preparar()
    {
        logger.LogInformation("Limpiando carpetas de trabajo ChileCompra.");

        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaZipLicitaciones());
        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaExtraidosLicitaciones());
        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaZipOC());
        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaExtraidosOC());

        sistemaArchivos.CrearCarpetaSiNoExiste(ObtenerCarpetaFinalLicitaciones());
        sistemaArchivos.CrearCarpetaSiNoExiste(ObtenerCarpetaFinalOC());
    }

    public string ObtenerRutaZipLicitaciones(PeriodoProceso periodo) =>
        Path.Combine(ObtenerCarpetaZipLicitaciones(), periodo.NombreArchivoZip);

    public string ObtenerRutaZipOC(PeriodoProceso periodo) =>
        Path.Combine(ObtenerCarpetaZipOC(), periodo.NombreArchivoZip);

    public string ObtenerCarpetaZipLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.Licitaciones, opciones.Carpetas.Zip);

    public string ObtenerCarpetaExtraidosLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.Licitaciones, opciones.Carpetas.Extraidos);

    public string ObtenerCarpetaZipOC() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.OC, opciones.Carpetas.Zip);

    public string ObtenerCarpetaExtraidosOC() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.OC, opciones.Carpetas.Extraidos);

    public string ObtenerCarpetaFinalLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseFinal, opciones.Carpetas.Licitaciones);

    public string ObtenerCarpetaFinalOC() =>
        Path.Combine(opciones.Carpetas.BaseFinal, opciones.Carpetas.OC);
}

