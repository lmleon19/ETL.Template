using ETL.Common.Resultados;

namespace ETL.Common.Validacion;

/// <summary>
/// Validador común para comprobaciones genéricas de archivos antes de procesarlos en un ETL.
/// </summary>
public sealed class ValidadorArchivo
{
    /// <summary>
    /// Valida que un archivo exista.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo requerido.</param>
    /// <returns>Resultado de la validación.</returns>
    public ResultadoOperacion ValidarExiste(string rutaArchivo)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);

            return File.Exists(rutaArchivo)
                ? ResultadoOperacion.Correcto("El archivo existe.")
                : ResultadoOperacion.Error($"No se encontró el archivo: {rutaArchivo}");
        }
        catch (Exception ex)
        {
            return ResultadoOperacion.Error("No fue posible validar la existencia del archivo.", ex);
        }
    }

    /// <summary>
    /// Valida que un archivo exista y tenga contenido.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo requerido.</param>
    /// <returns>Resultado de la validación.</returns>
    public ResultadoOperacion ValidarNoVacio(string rutaArchivo)
    {
        try
        {
            ResultadoOperacion validacionExiste = ValidarExiste(rutaArchivo);

            if (!validacionExiste.Exitoso)
            {
                return validacionExiste;
            }

            FileInfo archivo = new(rutaArchivo);

            return archivo.Length > 0
                ? ResultadoOperacion.Correcto("El archivo tiene contenido.")
                : ResultadoOperacion.Error($"El archivo está vacío: {rutaArchivo}");
        }
        catch (Exception ex)
        {
            return ResultadoOperacion.Error("No fue posible validar que el archivo tenga contenido.", ex);
        }
    }

    /// <summary>
    /// Valida que un archivo tenga una extensión permitida.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo requerido.</param>
    /// <param name="extensionesPermitidas">Extensiones permitidas, con o sin punto inicial.</param>
    /// <returns>Resultado de la validación.</returns>
    public ResultadoOperacion ValidarExtension(string rutaArchivo, params string[] extensionesPermitidas)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);
            ArgumentNullException.ThrowIfNull(extensionesPermitidas);

            string[] extensionesNormalizadas = extensionesPermitidas
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(NormalizarExtension)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (extensionesNormalizadas.Length == 0)
            {
                return ResultadoOperacion.Error("Debe informar al menos una extensión permitida.");
            }

            string extensionArchivo = Path.GetExtension(rutaArchivo);

            if (extensionesNormalizadas.Contains(extensionArchivo, StringComparer.OrdinalIgnoreCase))
            {
                return ResultadoOperacion.Correcto("La extensión del archivo es válida.");
            }

            return ResultadoOperacion.Error($"La extensión '{extensionArchivo}' no está permitida. Extensiones permitidas: {string.Join(", ", extensionesNormalizadas)}.");
        }
        catch (Exception ex)
        {
            return ResultadoOperacion.Error("No fue posible validar la extensión del archivo.", ex);
        }
    }

    /// <summary>
    /// Valida que un archivo no supere un tamaño máximo.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo requerido.</param>
    /// <param name="bytesMaximos">Tamaño máximo permitido en bytes.</param>
    /// <returns>Resultado de la validación.</returns>
    public ResultadoOperacion ValidarTamanoMaximo(string rutaArchivo, long bytesMaximos)
    {
        try
        {
            if (bytesMaximos < 0)
            {
                return ResultadoOperacion.Error("El tamaño máximo no puede ser negativo.");
            }

            ResultadoOperacion validacionExiste = ValidarExiste(rutaArchivo);

            if (!validacionExiste.Exitoso)
            {
                return validacionExiste;
            }

            FileInfo archivo = new(rutaArchivo);

            return archivo.Length <= bytesMaximos
                ? ResultadoOperacion.Correcto("El tamaño del archivo está dentro del máximo permitido.")
                : ResultadoOperacion.Error($"El archivo supera el tamaño máximo permitido. Tamaño actual: {archivo.Length} bytes. Máximo: {bytesMaximos} bytes.");
        }
        catch (Exception ex)
        {
            return ResultadoOperacion.Error("No fue posible validar el tamaño máximo del archivo.", ex);
        }
    }

    private static string NormalizarExtension(string extension)
    {
        string extensionNormalizada = extension.Trim();

        // El estándar acepta extensiones con o sin punto para facilitar el uso desde configuración.
        return extensionNormalizada.StartsWith('.')
            ? extensionNormalizada
            : $".{extensionNormalizada}";
    }
}
