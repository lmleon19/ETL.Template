using System.Globalization;
using System.Text;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para normalizar textos usados en archivos, encabezados y configuraciones ETL.
/// </summary>
public sealed class NormalizadorTexto
{
    /// <summary>
    /// Elimina espacios redundantes y deja un único espacio entre palabras.
    /// </summary>
    /// <param name="valor">Texto original.</param>
    /// <returns>Texto normalizado.</returns>
    public string NormalizarEspacios(string valor)
    {
        ArgumentNullException.ThrowIfNull(valor);

        string[] partes = valor
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(' ', partes);
    }

    /// <summary>
    /// Normaliza un nombre de columna para comparaciones independientes de espacios, acentos y mayúsculas.
    /// </summary>
    /// <param name="valor">Nombre de columna original.</param>
    /// <returns>Nombre de columna normalizado.</returns>
    public string NormalizarNombreColumna(string valor)
    {
        ArgumentNullException.ThrowIfNull(valor);

        string sinEspaciosExtra = NormalizarEspacios(valor);
        string sinAcentos = QuitarAcentos(sinEspaciosExtra);

        return sinAcentos.ToUpperInvariant();
    }

    private static string QuitarAcentos(string valor)
    {
        string normalizado = valor.Normalize(NormalizationForm.FormD);
        StringBuilder resultado = new(capacity: normalizado.Length);

        foreach (char caracter in normalizado)
        {
            UnicodeCategory categoria = CharUnicodeInfo.GetUnicodeCategory(caracter);

            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caracter);
            }
        }

        return resultado.ToString().Normalize(NormalizationForm.FormC);
    }
}
