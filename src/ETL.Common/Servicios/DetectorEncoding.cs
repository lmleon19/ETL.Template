using System.Text;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para detectar el encoding de archivos de texto usados por procesos ETL.
/// </summary>
public sealed class DetectorEncoding
{
    /// <summary>
    /// Detecta el encoding de un archivo usando BOM cuando está disponible.
    /// Si el archivo no contiene BOM, retorna el encoding por defecto informado o UTF-8.
    /// </summary>
    /// <param name="rutaArchivo">Ruta del archivo que se desea analizar.</param>
    /// <param name="encodingPorDefecto">Encoding usado cuando el archivo no contiene BOM. Si no se informa, se usa UTF-8.</param>
    /// <returns>Encoding detectado por BOM o encoding por defecto.</returns>
    public Encoding DetectarEncoding(string rutaArchivo, Encoding? encodingPorDefecto = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaArchivo);

        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("No se encontró el archivo para detectar encoding.", rutaArchivo);
        }

        Span<byte> buffer = stackalloc byte[4];

        using FileStream archivo = File.OpenRead(rutaArchivo);
        int bytesLeidos = archivo.Read(buffer);

        if (bytesLeidos >= 4)
        {
            if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
            {
                return Encoding.UTF32;
            }

            if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF)
            {
                return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            }
        }

        if (bytesLeidos >= 3 &&
            buffer[0] == 0xEF &&
            buffer[1] == 0xBB &&
            buffer[2] == 0xBF)
        {
            return Encoding.UTF8;
        }

        if (bytesLeidos >= 2)
        {
            if (buffer[0] == 0xFF && buffer[1] == 0xFE)
            {
                return Encoding.Unicode;
            }

            if (buffer[0] == 0xFE && buffer[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }
        }

        // Sin BOM no existe una forma completamente confiable de detectar encoding.
        // El template usa el encoding configurado por el ETL o UTF-8 como estándar.
        return encodingPorDefecto ?? Encoding.UTF8;
    }
}
