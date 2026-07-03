namespace ETL.Common.Servicios;

/// <summary>
/// Centraliza operaciones genéricas de carpetas y archivos usadas por procesos ETL.
/// </summary>
public sealed class SistemaArchivos
{
    /// <summary>
    /// Crea una carpeta cuando no existe.
    /// </summary>
    /// <param name="rutaCarpeta">Ruta de la carpeta requerida.</param>
    public void CrearCarpetaSiNoExiste(string rutaCarpeta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaCarpeta);

        Directory.CreateDirectory(rutaCarpeta);
    }

    /// <summary>
    /// Elimina una carpeta y todo su contenido cuando existe.
    /// </summary>
    /// <param name="rutaCarpeta">Ruta de la carpeta que se desea eliminar.</param>
    public void EliminarCarpetaSiExiste(string rutaCarpeta)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaCarpeta);

        if (Directory.Exists(rutaCarpeta))
        {
            Directory.Delete(rutaCarpeta, recursive: true);
        }
    }

    /// <summary>
    /// Busca el primer archivo que cumple un patrón dentro de una carpeta.
    /// </summary>
    /// <param name="rutaCarpeta">Carpeta donde se buscará el archivo.</param>
    /// <param name="patron">Patrón de búsqueda, por ejemplo *.csv.</param>
    /// <returns>Ruta completa del primer archivo encontrado.</returns>
    public string BuscarPrimerArchivo(string rutaCarpeta, string patron)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaCarpeta);
        ArgumentException.ThrowIfNullOrWhiteSpace(patron);

        if (!Directory.Exists(rutaCarpeta))
        {
            throw new DirectoryNotFoundException($"No se encontró la carpeta: {rutaCarpeta}");
        }

        string? rutaArchivo = Directory
            .EnumerateFiles(rutaCarpeta, patron, SearchOption.TopDirectoryOnly)
            .OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (rutaArchivo is null)
        {
            throw new FileNotFoundException($"No se encontró un archivo con el patrón '{patron}' en '{rutaCarpeta}'.");
        }

        return rutaArchivo;
    }

    /// <summary>
    /// Elimina y vuelve a crear una carpeta para dejarla disponible sin archivos previos.
    /// </summary>
    /// <param name="rutaCarpeta">Ruta de la carpeta que se desea limpiar.</param>
    public void LimpiarCarpeta(string rutaCarpeta)
    {
        EliminarCarpetaSiExiste(rutaCarpeta);
        CrearCarpetaSiNoExiste(rutaCarpeta);
    }

    /// <summary>
    /// Valida que un archivo exista antes de procesarlo.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo esperado.</param>
    public void ValidarExisteArchivo(string rutaArchivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("No se encontró el archivo requerido.", rutaArchivo);
        }
    }
}
