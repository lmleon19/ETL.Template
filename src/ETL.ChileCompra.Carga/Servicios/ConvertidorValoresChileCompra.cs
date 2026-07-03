using System.Globalization;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class ConvertidorValoresChileCompra
{
    private static readonly string[] ValoresNulos = ["", "NA", "N/A", "NULL", "#"];

    public bool EsNulo(string? valor) =>
        valor is null || ValoresNulos.Contains(valor.Trim(), StringComparer.OrdinalIgnoreCase);

    public string? ConvertirTexto(string? valor)
    {
        if (EsNulo(valor))
        {
            return null;
        }

        return valor!.Trim();
    }

    public int? ConvertirInt(string? valor)
    {
        if (EsNulo(valor))
        {
            return null;
        }

        string normalizado = valor!.Trim().Replace(".", string.Empty, StringComparison.Ordinal);

        return int.TryParse(normalizado, NumberStyles.Integer, CultureInfo.InvariantCulture, out int resultado)
            ? resultado
            : null;
    }

    public long? ConvertirLong(string? valor)
    {
        if (EsNulo(valor))
        {
            return null;
        }

        decimal? numero = ConvertirDecimal(valor);

        return numero.HasValue ? Convert.ToInt64(Math.Round(numero.Value, 0, MidpointRounding.AwayFromZero)) : null;
    }

    public double? ConvertirDouble(string? valor)
    {
        decimal? numero = ConvertirDecimal(valor);
        return numero.HasValue ? Convert.ToDouble(numero.Value, CultureInfo.InvariantCulture) : null;
    }

    public decimal? ConvertirDecimal(string? valor)
    {
        if (EsNulo(valor))
        {
            return null;
        }

        string normalizado = valor!.Trim();

        if (normalizado.Contains(',', StringComparison.Ordinal) && normalizado.Contains('.', StringComparison.Ordinal))
        {
            normalizado = normalizado.Replace(".", string.Empty, StringComparison.Ordinal).Replace(",", ".", StringComparison.Ordinal);
        }
        else if (normalizado.Contains(',', StringComparison.Ordinal))
        {
            normalizado = normalizado.Replace(",", ".", StringComparison.Ordinal);
        }

        const NumberStyles estilos = NumberStyles.Float | NumberStyles.AllowThousands;

        return decimal.TryParse(normalizado, estilos, CultureInfo.InvariantCulture, out decimal resultado)
            ? resultado
            : null;
    }

    public DateTime? ConvertirFecha(string? valor)
    {
        if (EsNulo(valor))
        {
            return null;
        }

        string normalizado = valor!.Trim();
        string[] formatos =
        [
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "dd-MM-yyyy",
            "dd-MM-yyyy HH:mm:ss",
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss"
        ];

        if (DateTime.TryParseExact(normalizado, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaExacta))
        {
            return fechaExacta;
        }

        return DateTime.TryParse(normalizado, CultureInfo.GetCultureInfo("es-CL"), DateTimeStyles.None, out DateTime fecha)
            ? fecha
            : null;
    }
}
