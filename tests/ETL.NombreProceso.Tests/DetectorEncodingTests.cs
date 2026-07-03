using System.Text;
using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class DetectorEncodingTests
{
    [Fact]
    public void DetectarEncoding_CuandoArchivoTieneBomUtf8_RetornaUtf8()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(rutaArchivo, [0xEF, 0xBB, 0xBF, 0x41]);

            DetectorEncoding detectorEncoding = new();

            Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, Encoding.Latin1);

            Assert.Equal(Encoding.UTF8.WebName, encoding.WebName);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public void DetectarEncoding_CuandoArchivoTieneBomUtf16LittleEndian_RetornaUnicode()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(rutaArchivo, [0xFF, 0xFE, 0x41, 0x00]);

            DetectorEncoding detectorEncoding = new();

            Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo);

            Assert.Equal(Encoding.Unicode.WebName, encoding.WebName);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public void DetectarEncoding_CuandoArchivoNoTieneBom_RetornaEncodingPorDefecto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(rutaArchivo, [0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x69, 0x64, 0x6F]);

            DetectorEncoding detectorEncoding = new();

            Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, Encoding.Latin1);

            Assert.Equal(Encoding.Latin1.WebName, encoding.WebName);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }
}
