using System.Text;
using ETL.Common.Resultados;
using ETL.Common.Servicios;

namespace ETL.Common.Validacion;

/// <summary>
/// Validador común para estructura mínima de archivos CSV.
/// </summary>
public sealed class ValidadorCsv
{
    /// <summary>
    /// Valida la estructura básica de un archivo CSV antes de leerlo o cargarlo.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo CSV.</param>
    /// <param name="encoding">Encoding que se usará para leer el archivo.</param>
    /// <param name="delimitador">Caracter delimitador del archivo.</param>
    /// <param name="calificadorTexto">Caracter usado para delimitar textos que pueden contener separadores o saltos de línea.</param>
    /// <param name="cantidadMinimaFilas">Cantidad mínima de filas de datos requeridas. No cuenta el encabezado.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Resultado de la validación estructural.</returns>
    public async Task<ResultadoOperacion> ValidarEstructuraBasicaAsync(
        string rutaArchivo,
        Encoding encoding,
        char delimitador,
        char calificadorTexto = '"',
        int cantidadMinimaFilas = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
            ArgumentNullException.ThrowIfNull(encoding);

            if (cantidadMinimaFilas < 0)
            {
                return ResultadoOperacion.Error("La cantidad mínima de filas no puede ser negativa.");
            }

            if (!File.Exists(rutaArchivo))
            {
                return ResultadoOperacion.Error($"No se encontró el archivo CSV: {rutaArchivo}");
            }

            using StreamReader reader = new(rutaArchivo, encoding);
            string? registroEncabezado = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken);

            if (string.IsNullOrWhiteSpace(registroEncabezado))
            {
                return ResultadoOperacion.Error("El archivo CSV no contiene encabezado.");
            }

            IReadOnlyList<string> encabezados = CsvLineParser.SepararColumnas(registroEncabezado, delimitador, calificadorTexto);
            ResultadoOperacion validacionEncabezados = ValidarEncabezados(encabezados);

            if (!validacionEncabezados.Exitoso)
            {
                return validacionEncabezados;
            }

            int cantidadFilasDatos = 0;
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

                cantidadFilasDatos++;
                IReadOnlyList<string> valores = CsvLineParser.SepararColumnas(registroCsv, delimitador, calificadorTexto);

                if (valores.Count != encabezados.Count)
                {
                    return ResultadoOperacion.Error($"El registro {numeroRegistro} contiene {valores.Count} columnas, pero el encabezado contiene {encabezados.Count}.");
                }
            }

            if (cantidadFilasDatos < cantidadMinimaFilas)
            {
                return ResultadoOperacion.Error($"El archivo CSV contiene {cantidadFilasDatos} filas de datos, pero se requieren al menos {cantidadMinimaFilas}.");
            }

            return ResultadoOperacion.Correcto("La estructura básica del CSV es válida.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ResultadoOperacion.Error("No fue posible validar la estructura básica del CSV.", ex);
        }
    }

    /// <summary>
    /// Valida que un archivo CSV contenga las columnas obligatorias requeridas por el proceso ETL.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo CSV.</param>
    /// <param name="encoding">Encoding que se usará para leer el encabezado.</param>
    /// <param name="delimitador">Caracter delimitador del archivo.</param>
    /// <param name="calificadorTexto">Caracter usado para delimitar textos que pueden contener separadores o saltos de línea.</param>
    /// <param name="columnasObligatorias">Columnas que deben existir en el encabezado.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Resultado de la validación.</returns>
    public async Task<ResultadoOperacion> ValidarColumnasObligatoriasAsync(
        string rutaArchivo,
        Encoding encoding,
        char delimitador,
        IEnumerable<string> columnasObligatorias,
        char calificadorTexto = '"',
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
            ArgumentNullException.ThrowIfNull(encoding);
            ArgumentNullException.ThrowIfNull(columnasObligatorias);

            if (!File.Exists(rutaArchivo))
            {
                return ResultadoOperacion.Error($"No se encontró el archivo CSV: {rutaArchivo}");
            }

            string[] columnasRequeridas = columnasObligatorias
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (columnasRequeridas.Length == 0)
            {
                return ResultadoOperacion.Correcto("No se informaron columnas obligatorias para validar.");
            }

            using StreamReader reader = new(rutaArchivo, encoding);
            string? registroEncabezado = await CsvLineParser.LeerRegistroAsync(reader, calificadorTexto, cancellationToken);

            if (string.IsNullOrWhiteSpace(registroEncabezado))
            {
                return ResultadoOperacion.Error("El archivo CSV no contiene encabezado.");
            }

            IReadOnlyList<string> encabezados = CsvLineParser.SepararColumnas(registroEncabezado, delimitador, calificadorTexto);
            HashSet<string> columnasArchivo = encabezados
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            string[] columnasFaltantes = columnasRequeridas
                .Where(c => !columnasArchivo.Contains(c))
                .ToArray();

            if (columnasFaltantes.Length > 0)
            {
                return ResultadoOperacion.Error($"El archivo CSV no contiene las columnas obligatorias: {string.Join(", ", columnasFaltantes)}.");
            }

            return ResultadoOperacion.Correcto("El archivo CSV contiene las columnas obligatorias.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return ResultadoOperacion.Error("No fue posible validar las columnas obligatorias del CSV.", ex);
        }
    }

    private static ResultadoOperacion ValidarEncabezados(IReadOnlyList<string> encabezados)
    {
        if (encabezados.Count == 0)
        {
            return ResultadoOperacion.Error("El archivo CSV no contiene columnas en el encabezado.");
        }

        if (encabezados.Any(string.IsNullOrWhiteSpace))
        {
            return ResultadoOperacion.Error("El archivo CSV contiene columnas vacías en el encabezado.");
        }

        string[] columnasDuplicadas = encabezados
            .GroupBy(e => e.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (columnasDuplicadas.Length > 0)
        {
            return ResultadoOperacion.Error($"El archivo CSV contiene columnas duplicadas en el encabezado: {string.Join(", ", columnasDuplicadas)}.");
        }

        return ResultadoOperacion.Correcto("Encabezado CSV válido.");
    }
}
