using System.Text;
using ETL.ChileCompra.Carga.Model;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class NormalizadorRutChile
{
    public static int LimpiarNumero(string? rut) => LimpiarInterno(rut).Numero;

    public ValorRut Limpiar(string? rut) => LimpiarInterno(rut);

    private static ValorRut LimpiarInterno(string? rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
        {
            return new ValorRut(0, null, rut);
        }

        string texto = rut
            .Trim()
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace('\u2010', '-')
            .Replace('\u2011', '-')
            .Replace('\u2012', '-')
            .Replace('\u2013', '-')
            .Replace('\u2014', '-');

        if (texto.Equals("#", StringComparison.OrdinalIgnoreCase) ||
            texto.Equals("NA", StringComparison.OrdinalIgnoreCase))
        {
            return new ValorRut(0, null, rut);
        }

        string numeroTexto;
        string? digitoVerificador = null;
        int posicionGuion = texto.IndexOf('-', StringComparison.Ordinal);

        if (posicionGuion >= 0)
        {
            numeroTexto = texto[..posicionGuion];
            digitoVerificador = texto[(posicionGuion + 1)..].Trim().ToUpperInvariant();
        }
        else
        {
            StringBuilder digitos = new();

            foreach (char caracter in texto)
            {
                if (char.IsDigit(caracter))
                {
                    digitos.Append(caracter);
                }
            }

            numeroTexto = digitos.ToString();
        }

        return int.TryParse(numeroTexto, out int numero)
            ? new ValorRut(numero, string.IsNullOrWhiteSpace(digitoVerificador) ? null : digitoVerificador, rut)
            : new ValorRut(0, digitoVerificador, rut);
    }
}
