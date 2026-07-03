using ETL.ChileCompra.Carga.Model;
using ETL.Common.Servicios;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class RepositorioParidadMoneda
{
    private readonly OpcionesChileCompra opciones;
    private readonly Csv csv;
    private readonly DetectorEncoding detectorEncoding;
    private readonly ConvertidorValoresChileCompra convertidorValores;
    private readonly ILogger<RepositorioParidadMoneda> logger;
    private Dictionary<(int Anio, int Mes, string Moneda), decimal> paridades = [];

    public RepositorioParidadMoneda(
        IOptions<OpcionesChileCompra> opciones,
        Csv csv,
        DetectorEncoding detectorEncoding,
        ConvertidorValoresChileCompra convertidorValores,
        ILogger<RepositorioParidadMoneda> logger)
    {
        this.opciones = opciones.Value;
        this.csv = csv;
        this.detectorEncoding = detectorEncoding;
        this.convertidorValores = convertidorValores;
        this.logger = logger;
    }

    public async Task CargarAsync(string rutaArchivo, CancellationToken cancellationToken = default)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encodingFallback = Encoding.GetEncoding(opciones.EncodingFallback);
        Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, encodingFallback);
        char delimitador = ObtenerDelimitador();

        IReadOnlyList<Dictionary<string, string>> registros = await csv.LeerCsvAsync(rutaArchivo, encoding, delimitador, cancellationToken: cancellationToken);
        Dictionary<(int Anio, int Mes, string Moneda), decimal> resultado = [];

        foreach (Dictionary<string, string> registro in registros)
        {
            ParidadMoneda? paridad = MapearParidad(registro);

            if (paridad is not null)
            {
                resultado[(paridad.Anio, paridad.Mes, NormalizarMoneda(paridad.Moneda))] = paridad.Valor;
            }
        }

        paridades = resultado;
        logger.LogInformation("Paridades de moneda cargadas: {Cantidad}.", paridades.Count);
    }

    public long? ConvertirAClp(decimal? monto, string? moneda, DateTime? fecha)
    {
        if (!monto.HasValue)
        {
            return null;
        }

        string monedaNormalizada = NormalizarMoneda(moneda);

        if (EsPesoChileno(monedaNormalizada))
        {
            return Convert.ToInt64(Math.Round(monto.Value, 0, MidpointRounding.AwayFromZero));
        }

        if (!fecha.HasValue)
        {
            return null;
        }

        return ObtenerFactorClp(fecha.Value, monedaNormalizada, out decimal factor)
            ? Convert.ToInt64(Math.Round(monto.Value * factor, 0, MidpointRounding.AwayFromZero))
            : null;
    }

    private ParidadMoneda? MapearParidad(Dictionary<string, string> registro)
    {
        int? anio = ObtenerInt(registro, "YEAR", "Anio", "Año", "Year");
        int? mes = ObtenerInt(registro, "MONTH", "Mes", "Month");
        string? moneda = ObtenerTexto(registro, "Moneda", "CodigoMoneda", "TipoMoneda", "Codigo");
        decimal? valor = ObtenerDecimal(registro, "VMCLP", "Valor", "Paridad", "ValorCLP", "ValorPesos", "Pesos", "CLP");

        if (anio.HasValue && mes.HasValue && !string.IsNullOrWhiteSpace(moneda) && valor.HasValue)
        {
            return new ParidadMoneda(anio.Value, mes.Value, moneda, valor.Value);
        }

        DateTime? fecha = ObtenerFecha(registro, "Fecha", "Periodo");

        return fecha.HasValue && !string.IsNullOrWhiteSpace(moneda) && valor.HasValue
            ? new ParidadMoneda(fecha.Value.Year, fecha.Value.Month, moneda, valor.Value)
            : null;
    }

    private int? ObtenerInt(Dictionary<string, string> registro, params string[] columnas)
    {
        string? valor = ObtenerTexto(registro, columnas);
        return convertidorValores.ConvertirInt(valor);
    }

    private decimal? ObtenerDecimal(Dictionary<string, string> registro, params string[] columnas)
    {
        string? valor = ObtenerTexto(registro, columnas);
        return convertidorValores.ConvertirDecimal(valor);
    }

    private DateTime? ObtenerFecha(Dictionary<string, string> registro, params string[] columnas)
    {
        string? valor = ObtenerTexto(registro, columnas);
        return convertidorValores.ConvertirFecha(valor);
    }

    private static string? ObtenerTexto(Dictionary<string, string> registro, params string[] columnas)
    {
        foreach (string columna in columnas)
        {
            if (registro.TryGetValue(columna, out string? valor))
            {
                return valor;
            }
        }

        return null;
    }

    private char ObtenerDelimitador() => string.IsNullOrEmpty(opciones.DelimitadorCsv) ? ';' : opciones.DelimitadorCsv[0];

    private static bool EsPesoChileno(string moneda) =>
        moneda is "CLP" or "LP" or "PESOCHILENO" or "PESOSCHILENOS" or "PESO" or "PESOS";

    private bool ObtenerFactorClp(DateTime fecha, string moneda, out decimal factor)
    {
        DateTime periodo = new(fecha.Year, fecha.Month, 1);

        while (periodo >= new DateTime(2000, 1, 1))
        {
            if (paridades.TryGetValue((periodo.Year, periodo.Month, moneda), out factor))
            {
                return true;
            }

            periodo = periodo.AddMonths(-1);
        }

        factor = 0;
        return false;
    }

    private static string NormalizarMoneda(string? moneda)
    {
        if (string.IsNullOrWhiteSpace(moneda))
        {
            return string.Empty;
        }

        string monedaNormalizada = moneda
            .Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("\"", string.Empty, StringComparison.Ordinal)
            .Replace("'", string.Empty, StringComparison.Ordinal)
            .Replace("¨", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        return monedaNormalizada switch
        {
            "UF" => "CLF",
            _ => monedaNormalizada
        };
    }
}
