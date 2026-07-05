using System.Text;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para leer archivos CSV con encabezado.
/// </summary>
public sealed class Csv
{
    /// <summary>
    /// Lee un archivo CSV y retorna cada fila como diccionario usando los encabezados como claves.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo CSV.</param>
    /// <param name="encoding">Encoding que se usará para leer el archivo.</param>
    /// <param name="delimitador">Caracter delimitador del archivo.</param>
    /// <param name="calificadorTexto">Caracter usado para delimitar textos que pueden contener separadores o saltos de línea.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Lista de registros leídos desde el CSV.</returns>
    public async Task<IReadOnlyList<Dictionary<string, string>>> LeerCsvAsync(
        string rutaArchivo,
        Encoding encoding,
        char delimitador,
        char calificadorTexto = '"',
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
        ArgumentNullException.ThrowIfNull(encoding);

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("No se encontró el archivo CSV.", rutaArchivo);
        }

        using StreamReader reader = new(rutaArchivo, encoding);

        string? registroEncabezado = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken);

        if (string.IsNullOrWhiteSpace(registroEncabezado))
        {
            throw new InvalidDataException("El archivo CSV no contiene encabezado.");
        }

        IReadOnlyList<string> encabezados = CsvLineParser.SepararColumnas(registroEncabezado, delimitador, calificadorTexto);
        ValidarEncabezados(encabezados);

        List<Dictionary<string, string>> registros = [];
        int numeroRegistro = 1;
        string? registroCsv;

        while ((registroCsv = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            numeroRegistro++;

            if (string.IsNullOrWhiteSpace(registroCsv))
            {
                continue;
            }

            IReadOnlyList<string> valores = CsvLineParser.SepararColumnas(registroCsv, delimitador, calificadorTexto);

            if (valores.Count != encabezados.Count)
            {
                throw new InvalidDataException($"El registro {numeroRegistro} contiene {valores.Count} columnas, pero el encabezado contiene {encabezados.Count}.");
            }

            Dictionary<string, string> registro = new(StringComparer.OrdinalIgnoreCase);

            for (int indice = 0; indice < encabezados.Count; indice++)
            {
                registro[encabezados[indice]] = valores[indice];
            }

            registros.Add(registro);
        }

        return registros;
    }

    public async Task<ResultadoLecturaCsv> LeerCsvConErroresAsync(
        string rutaArchivo,
        Encoding encoding,
        char delimitador,
        char calificadorTexto = '"',
        decimal porcentajeMaximoRegistrosInvalidos = 100m,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
        ArgumentNullException.ThrowIfNull(encoding);

        if (porcentajeMaximoRegistrosInvalidos is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(porcentajeMaximoRegistrosInvalidos),
                "El porcentaje maximo de registros invalidos debe estar entre 0 y 100.");
        }

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("No se encontrÃ³ el archivo CSV.", rutaArchivo);
        }

        using StreamReader reader = new(rutaArchivo, encoding);

        string? registroEncabezado = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken);

        if (string.IsNullOrWhiteSpace(registroEncabezado))
        {
            throw new InvalidDataException("El archivo CSV no contiene encabezado.");
        }

        IReadOnlyList<string> encabezados = CsvLineParser.SepararColumnas(registroEncabezado, delimitador, calificadorTexto);
        ValidarEncabezados(encabezados);

        List<Dictionary<string, string>> registros = [];
        List<ErrorRegistroCsv> errores = [];
        int numeroRegistro = 1;
        int registrosDatos = 0;
        string? registroCsv;

        while ((registroCsv = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken)) is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            numeroRegistro++;

            if (string.IsNullOrWhiteSpace(registroCsv))
            {
                continue;
            }

            registrosDatos++;
            IReadOnlyList<string> valores = CsvLineParser.SepararColumnas(registroCsv, delimitador, calificadorTexto);

            if (valores.Count != encabezados.Count)
            {
                errores.Add(new ErrorRegistroCsv(
                    numeroRegistro,
                    encabezados.Count,
                    valores.Count,
                    ObtenerMuestra(registroCsv)));

                continue;
            }

            Dictionary<string, string> registro = new(StringComparer.OrdinalIgnoreCase);

            for (int indice = 0; indice < encabezados.Count; indice++)
            {
                registro[encabezados[indice]] = valores[indice];
            }

            registros.Add(registro);
        }

        return new ResultadoLecturaCsv(registros, errores, registrosDatos, porcentajeMaximoRegistrosInvalidos);
    }

    private static void ValidarEncabezados(IReadOnlyList<string> encabezados)
    {
        if (encabezados.Count == 0 || encabezados.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidDataException("El archivo CSV contiene encabezados vacíos.");
        }

        bool existenDuplicados = encabezados
            .GroupBy(e => e, StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1);

        if (existenDuplicados)
        {
            throw new InvalidDataException("El archivo CSV contiene encabezados duplicados.");
        }
    }

    private static string ObtenerMuestra(string registroCsv)
    {
        const int largoMaximo = 1000;
        string registroEnUnaLinea = registroCsv.ReplaceLineEndings(" ");

        return registroEnUnaLinea.Length <= largoMaximo
            ? registroEnUnaLinea
            : registroEnUnaLinea[..largoMaximo];
    }
}

public sealed record ErrorRegistroCsv(
    int NumeroRegistro,
    int ColumnasEsperadas,
    int ColumnasEncontradas,
    string Muestra);

public sealed record ResultadoLecturaCsv(
    IReadOnlyList<Dictionary<string, string>> Registros,
    IReadOnlyList<ErrorRegistroCsv> Errores,
    int TotalRegistrosDatos,
    decimal PorcentajeMaximoRegistrosInvalidos = 100m)
{
    public int RegistrosValidos => Registros.Count;
    public int RegistrosInvalidos => Errores.Count;
    public decimal PorcentajeRegistrosInvalidos =>
        TotalRegistrosDatos == 0
            ? 0
            : decimal.Round(RegistrosInvalidos * 100m / TotalRegistrosDatos, 4);

    public bool ExcedePorcentajeMaximo => PorcentajeRegistrosInvalidos > PorcentajeMaximoRegistrosInvalidos;
}
