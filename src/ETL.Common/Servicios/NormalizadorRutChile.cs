using System.Text;

namespace ETL.Common.Servicios;

public sealed class NormalizadorRutChile
{
    public static int LimpiarNumero(string? rut) => ValidarEstructuraInterno(rut).Numero;

    public ValorRut Limpiar(string? rut) => ValidarRutCompleto(rut);

    public ValorRut ValidarEstructura(string? rut) => ValidarEstructuraInterno(rut);

    public ValorRut ValidarRutCompleto(string? rut)
    {
        ValorRut rutEstructurado = ValidarEstructuraInterno(rut);
        bool esValido = rutEstructurado.EstructuraValida &&
            EsRutValido(rutEstructurado.Numero, rutEstructurado.DigitoVerificador);

        return rutEstructurado with { EsValido = esValido };
    }

    private static ValorRut ValidarEstructuraInterno(string? rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
        {
            return new ValorRut(0, null, rut, false, false);
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
            return new ValorRut(0, null, rut, false, false);
        }

        (string numeroTexto, string? digitoVerificador) = SepararRut(texto);

        if (!int.TryParse(numeroTexto, out int numero))
        {
            return new ValorRut(0, digitoVerificador, rut, false, false);
        }

        digitoVerificador = string.IsNullOrWhiteSpace(digitoVerificador)
            ? null
            : digitoVerificador;
        bool estructuraValida = numero > 0 &&
            !string.IsNullOrWhiteSpace(digitoVerificador) &&
            digitoVerificador.Length == 1;

        return new ValorRut(numero, digitoVerificador, rut, estructuraValida, false);
    }

    private static (string Numero, string? DigitoVerificador) SepararRut(string texto)
    {
        string[] partes = texto
            .Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (partes.Length >= 2)
        {
            string digitoVerificador = partes[^1].ToUpperInvariant();
            string numeroTexto = UnirPartesNumericas(partes, 0, partes.Length - 1);

            if (DebeOmitirPrefijoSucursal(partes, numeroTexto))
            {
                numeroTexto = UnirPartesNumericas(partes, 1, partes.Length - 1);
            }

            return (numeroTexto, digitoVerificador);
        }

        StringBuilder digitos = new();

        foreach (char caracter in texto)
        {
            if (char.IsDigit(caracter))
            {
                digitos.Append(caracter);
            }
        }

        return (digitos.ToString(), null);
    }

    private static bool DebeOmitirPrefijoSucursal(string[] partes, string numeroTexto)
    {
        if (partes.Length < 3 || numeroTexto.Length <= 9)
        {
            return false;
        }

        string numeroSinPrimeraParte = UnirPartesNumericas(partes, 1, partes.Length - 1);

        return partes[0].Length <= 4 && numeroSinPrimeraParte.Length is >= 7 and <= 9;
    }

    private static string UnirPartesNumericas(string[] partes, int inicio, int finExclusivo)
    {
        StringBuilder digitos = new();

        for (int indice = inicio; indice < finExclusivo; indice++)
        {
            foreach (char caracter in partes[indice])
            {
                if (char.IsDigit(caracter))
                {
                    digitos.Append(caracter);
                }
            }
        }

        return digitos.ToString();
    }

    private static bool EsRutValido(int numero, string? digitoVerificador)
    {
        if (numero <= 0 || string.IsNullOrWhiteSpace(digitoVerificador) || digitoVerificador.Length != 1)
        {
            return false;
        }

        char digitoEsperado = CalcularDigitoVerificador(numero);

        return char.ToUpperInvariant(digitoVerificador[0]) == digitoEsperado;
    }

    private static char CalcularDigitoVerificador(int numero)
    {
        int suma = 0;
        int multiplicador = 2;

        while (numero > 0)
        {
            suma += numero % 10 * multiplicador;
            numero /= 10;
            multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
        }

        int resultado = 11 - suma % 11;

        return resultado switch
        {
            11 => '0',
            10 => 'K',
            _ => resultado.ToString()[0]
        };
    }
}
