using System.IO.Compression;
using ETL.Common.Resultados;
using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para descomprimir archivos ZIP usados como entrada de procesos ETL.
/// </summary>
public sealed class Zip
{
    private readonly ILogger<Zip> logger;

    public Zip(ILogger<Zip> logger) => this.logger = logger;

    /// <summary>
    /// Descomprime un archivo ZIP en una carpeta destino.
    /// </summary>
    /// <param name="rutaZip">Ruta del archivo ZIP origen.</param>
    /// <param name="carpetaDestino">Carpeta donde se extraerán los archivos.</param>
    /// <returns>Resultado con la lista de archivos extraídos.</returns>
    public ResultadoOperacion<IReadOnlyList<string>> DescomprimirArchivo(string rutaZip, string carpetaDestino)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rutaZip);
            ArgumentException.ThrowIfNullOrWhiteSpace(carpetaDestino);

            if (!File.Exists(rutaZip))
            {
                return ResultadoOperacion<IReadOnlyList<string>>.Error($"No se encontró el archivo ZIP: {rutaZip}");
            }

            Directory.CreateDirectory(carpetaDestino);

            string carpetaDestinoNormalizada = Path.GetFullPath(carpetaDestino);

            if (!carpetaDestinoNormalizada.EndsWith(Path.DirectorySeparatorChar))
            {
                carpetaDestinoNormalizada += Path.DirectorySeparatorChar;
            }

            List<string> archivosExtraidos = [];

            logger.LogInformation("Iniciando descompresión de {RutaZip}.", rutaZip);

            using ZipArchive archivoZip = ZipFile.OpenRead(rutaZip);

            foreach (ZipArchiveEntry entrada in archivoZip.Entries)
            {
                string rutaSalida = Path.GetFullPath(Path.Combine(carpetaDestinoNormalizada, entrada.FullName));

                // Evita que una entrada mal formada del ZIP escriba fuera de la carpeta destino.
                if (!rutaSalida.StartsWith(carpetaDestinoNormalizada, StringComparison.OrdinalIgnoreCase))
                {
                    return ResultadoOperacion<IReadOnlyList<string>>.Error($"El ZIP contiene una ruta inválida: {entrada.FullName}");
                }

                if (string.IsNullOrEmpty(entrada.Name))
                {
                    Directory.CreateDirectory(rutaSalida);
                    continue;
                }

                string? carpetaArchivo = Path.GetDirectoryName(rutaSalida);

                if (!string.IsNullOrWhiteSpace(carpetaArchivo))
                {
                    Directory.CreateDirectory(carpetaArchivo);
                }

                entrada.ExtractToFile(rutaSalida, overwrite: true);
                archivosExtraidos.Add(rutaSalida);
            }

            logger.LogInformation("Descompresión completada. Archivos extraídos: {CantidadArchivos}.", archivosExtraidos.Count);

            return ResultadoOperacion<IReadOnlyList<string>>.Correcto(archivosExtraidos, "Archivo ZIP descomprimido correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al descomprimir {RutaZip}.", rutaZip);
            return ResultadoOperacion<IReadOnlyList<string>>.Error($"No fue posible descomprimir el archivo '{rutaZip}'.", ex);
        }
    }
}
