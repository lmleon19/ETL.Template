using System.Text;
using ETL.ChileCompra.Carga.Model;
using ETL.Common.Resultados;
using ETL.Common.Servicios;
using ETL.Common.Validacion;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class ValidadorArchivosStage
{
    private readonly OpcionesChileCompra opciones;
    private readonly MetadataStage metadataStage;
    private readonly DetectorEncoding detectorEncoding;
    private readonly ValidadorCsv validadorCsv;

    public ValidadorArchivosStage(
        IOptions<OpcionesChileCompra> opciones,
        MetadataStage metadataStage,
        DetectorEncoding detectorEncoding,
        ValidadorCsv validadorCsv)
    {
        this.opciones = opciones.Value;
        this.metadataStage = metadataStage;
        this.detectorEncoding = detectorEncoding;
        this.validadorCsv = validadorCsv;
    }

    public async Task<ResultadoOperacion> ValidarLicitacionesAsync(string rutaArchivo, CancellationToken cancellationToken = default) =>
        await ValidarAsync<RegistroLicitacionStage>(rutaArchivo, cancellationToken);

    public async Task<ResultadoOperacion> ValidarOrdenesCompraAsync(string rutaArchivo, CancellationToken cancellationToken = default) =>
        await ValidarAsync<RegistroOrdenCompraStage>(rutaArchivo, cancellationToken);

    private async Task<ResultadoOperacion> ValidarAsync<TRegistroStage>(string rutaArchivo, CancellationToken cancellationToken)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encodingFallback = Encoding.GetEncoding(opciones.EncodingFallback);
        Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, encodingFallback);
        char delimitador = string.IsNullOrEmpty(opciones.DelimitadorCsv) ? ';' : opciones.DelimitadorCsv[0];

        ResultadoOperacion estructura = await validadorCsv.ValidarEstructuraBasicaAsync(
            rutaArchivo,
            encoding,
            delimitador,
            cantidadMinimaFilas: 1,
            cancellationToken: cancellationToken);

        if (!estructura.Exitoso)
        {
            return estructura;
        }

        return await validadorCsv.ValidarColumnasObligatoriasAsync(
            rutaArchivo,
            encoding,
            delimitador,
            metadataStage.ObtenerColumnasCsv<TRegistroStage>(),
            cancellationToken: cancellationToken);
    }
}

