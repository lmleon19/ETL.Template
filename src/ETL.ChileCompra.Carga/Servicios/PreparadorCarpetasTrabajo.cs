using ETL.ChileCompra.Carga.Model;
using ETL.Common.Servicios;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

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
        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaZipOrdenesCompra());
        sistemaArchivos.LimpiarCarpeta(ObtenerCarpetaExtraidosOrdenesCompra());

        sistemaArchivos.CrearCarpetaSiNoExiste(ObtenerCarpetaFinalLicitaciones());
        sistemaArchivos.CrearCarpetaSiNoExiste(ObtenerCarpetaFinalOrdenesCompra());
    }

    public string ObtenerRutaZipLicitaciones(PeriodoProceso periodo) =>
        Path.Combine(ObtenerCarpetaZipLicitaciones(), periodo.NombreArchivoZip);

    public string ObtenerRutaZipOrdenesCompra(PeriodoProceso periodo) =>
        Path.Combine(ObtenerCarpetaZipOrdenesCompra(), periodo.NombreArchivoZip);

    public string ObtenerCarpetaZipLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.Licitaciones, opciones.Carpetas.Zip);

    public string ObtenerCarpetaExtraidosLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.Licitaciones, opciones.Carpetas.Extraidos);

    public string ObtenerCarpetaZipOrdenesCompra() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.OrdenesCompra, opciones.Carpetas.Zip);

    public string ObtenerCarpetaExtraidosOrdenesCompra() =>
        Path.Combine(opciones.Carpetas.BaseTrabajo, opciones.Carpetas.OrdenesCompra, opciones.Carpetas.Extraidos);

    public string ObtenerCarpetaFinalLicitaciones() =>
        Path.Combine(opciones.Carpetas.BaseFinal, opciones.Carpetas.Licitaciones);

    public string ObtenerCarpetaFinalOrdenesCompra() =>
        Path.Combine(opciones.Carpetas.BaseFinal, opciones.Carpetas.OrdenesCompra);
}
