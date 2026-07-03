using System.Text;
using ETL.Common.Servicios;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class LectorArchivosStage
{
    private readonly OpcionesChileCompra opciones;
    private readonly Csv csv;
    private readonly DetectorEncoding detectorEncoding;

    public LectorArchivosStage(IOptions<OpcionesChileCompra> opciones, Csv csv, DetectorEncoding detectorEncoding)
    {
        this.opciones = opciones.Value;
        this.csv = csv;
        this.detectorEncoding = detectorEncoding;
    }

    public async Task<IReadOnlyList<Dictionary<string, string>>> LeerCsvAsync(string rutaArchivo, CancellationToken cancellationToken = default)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encodingFallback = Encoding.GetEncoding(opciones.EncodingFallback);
        Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, encodingFallback);
        char delimitador = string.IsNullOrEmpty(opciones.DelimitadorCsv) ? ';' : opciones.DelimitadorCsv[0];

        return await csv.LeerCsvAsync(rutaArchivo, encoding, delimitador, cancellationToken: cancellationToken);
    }
}

