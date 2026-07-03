namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para construir rutas de trabajo usadas por procesos ETL.
/// </summary>
public sealed class RutaTrabajo
{
    /// <summary>
    /// Combina partes de una ruta eliminando segmentos vacíos.
    /// </summary>
    /// <param name="partes">Partes de la ruta.</param>
    /// <returns>Ruta combinada.</returns>
    public string CombinarRuta(params string[] partes)
    {
        ArgumentNullException.ThrowIfNull(partes);

        string[] partesValidas = partes
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .ToArray();

        if (partesValidas.Length == 0)
        {
            throw new ArgumentException("Debe informar al menos una parte de ruta válida.", nameof(partes));
        }

        return Path.Combine(partesValidas);
    }

    /// <summary>
    /// Construye la ruta completa de un archivo dentro de una carpeta.
    /// </summary>
    /// <param name="carpeta">Carpeta base.</param>
    /// <param name="nombreArchivo">Nombre del archivo.</param>
    /// <returns>Ruta completa del archivo.</returns>
    public string CrearRutaArchivo(string carpeta, string nombreArchivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(carpeta);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreArchivo);

        return Path.Combine(carpeta, nombreArchivo);
    }

    /// <summary>
    /// Construye una carpeta de trabajo usando la fecha de proceso en formato yyyyMMdd.
    /// </summary>
    /// <param name="carpetaBase">Carpeta base del ETL.</param>
    /// <param name="fechaProceso">Fecha de proceso.</param>
    /// <returns>Ruta de carpeta por fecha.</returns>
    public string CrearCarpetaPorFecha(string carpetaBase, DateOnly fechaProceso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(carpetaBase);

        string fecha = fechaProceso.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        return Path.Combine(carpetaBase, fecha);
    }
}
