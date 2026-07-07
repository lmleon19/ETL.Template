using System.Text;
using ETL.Common.Resultados;
using ETL.Common.Servicios;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class LectorArchivosStage
{
    private readonly OpcionesChileCompra opciones;
    private readonly Csv csv;
    private readonly DetectorEncoding detectorEncoding;
    private readonly ILogger<LectorArchivosStage> logger;

    public LectorArchivosStage(
        IOptions<OpcionesChileCompra> opciones,
        Csv csv,
        DetectorEncoding detectorEncoding,
        ILogger<LectorArchivosStage> logger)
    {
        this.opciones = opciones.Value;
        this.csv = csv;
        this.detectorEncoding = detectorEncoding;
        this.logger = logger;
    }

    public async Task<ResultadoOperacion<IReadOnlyList<Dictionary<string, string>>>> LeerCsvAsync(
        string rutaArchivo,
        CancellationToken cancellationToken = default,
        decimal? porcentajeMaximoFilasInvalidas = null)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encodingFallback = Encoding.GetEncoding(opciones.EncodingFallback);
        Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, encodingFallback);
        char delimitador = string.IsNullOrEmpty(opciones.DelimitadorCsv) ? ';' : opciones.DelimitadorCsv[0];
        decimal porcentajeMaximoPermitido = porcentajeMaximoFilasInvalidas ?? opciones.PorcentajeMaximoFilasInvalidas;

        ResultadoLecturaCsv resultado = await csv.LeerCsvConErroresAsync(
            rutaArchivo,
            encoding,
            delimitador,
            porcentajeMaximoRegistrosInvalidos: porcentajeMaximoPermitido,
            cancellationToken: cancellationToken);
        decimal porcentajeInvalidos = resultado.PorcentajeRegistrosInvalidos;

        if (resultado.ExcedePorcentajeMaximo)
        {
            RegistrarFilasInvalidas(rutaArchivo, resultado);
            return ResultadoOperacion<IReadOnlyList<Dictionary<string, string>>>.Error(
                $"El archivo '{rutaArchivo}' contiene {resultado.RegistrosInvalidos} filas invalidas de {resultado.TotalRegistrosDatos} ({porcentajeInvalidos}%). El maximo permitido es {porcentajeMaximoPermitido}%.");
        }

        if (resultado.RegistrosInvalidos > 0)
        {
            RegistrarFilasInvalidas(rutaArchivo, resultado);
            logger.LogWarning(
                "Se omitieron {FilasInvalidas} filas invalidas de {TotalFilas} en {Archivo}. Porcentaje: {PorcentajeInvalidos}%. Maximo permitido: {PorcentajeMaximo}%.",
                resultado.RegistrosInvalidos,
                resultado.TotalRegistrosDatos,
                rutaArchivo,
                porcentajeInvalidos,
                porcentajeMaximoPermitido);
        }

        return ResultadoOperacion<IReadOnlyList<Dictionary<string, string>>>.Correcto(
            resultado.Registros,
            $"Archivo CSV leido correctamente. Registros validos: {resultado.RegistrosValidos}. Registros invalidos omitidos: {resultado.RegistrosInvalidos}.");
    }

    private void RegistrarFilasInvalidas(string rutaArchivo, ResultadoLecturaCsv resultado)
    {
        foreach (ErrorRegistroCsv error in resultado.Errores)
        {
            logger.LogWarning(
                "Fila CSV invalida omitida. Archivo: {Archivo}. Registro: {Registro}. Columnas esperadas: {ColumnasEsperadas}. Columnas encontradas: {ColumnasEncontradas}. Muestra: {Muestra}",
                rutaArchivo,
                error.NumeroRegistro,
                error.ColumnasEsperadas,
                error.ColumnasEncontradas,
                error.Muestra);
        }
    }
}

