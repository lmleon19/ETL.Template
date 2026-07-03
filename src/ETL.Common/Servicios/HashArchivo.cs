using System.Security.Cryptography;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para calcular identificadores hash de archivos procesados por un ETL.
/// </summary>
public sealed class HashArchivo
{
    /// <summary>
    /// Calcula el hash SHA-256 de un archivo.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo que se desea identificar.</param>
    /// <param name="cancellationToken">Token de cancelación de la operación.</param>
    /// <returns>Hash SHA-256 en formato hexadecimal.</returns>
    public async Task<string> CalcularSha256Async(string rutaArchivo, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("No se encontró el archivo para calcular hash.", rutaArchivo);
        }

        await using FileStream stream = File.OpenRead(rutaArchivo);
        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
