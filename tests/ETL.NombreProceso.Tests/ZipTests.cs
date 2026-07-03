using System.IO.Compression;
using ETL.Common.Servicios;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class ZipTests
{
    [Fact]
    public void DescomprimirArchivo_CuandoZipEsValido_ExtraeArchivos()
    {
        string carpetaTrabajo = CrearCarpetaTemporal();
        string carpetaDestino = Path.Combine(carpetaTrabajo, "extraido");
        string rutaZip = Path.Combine(carpetaTrabajo, "archivo.zip");

        try
        {
            string archivoOrigen = Path.Combine(carpetaTrabajo, "datos.csv");
            File.WriteAllText(archivoOrigen, "id;nombre");

            using (ZipArchive zip = ZipFile.Open(rutaZip, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(archivoOrigen, "datos.csv");
            }

            Zip servicio = new(NullLogger<Zip>.Instance);

            var resultado = servicio.DescomprimirArchivo(rutaZip, carpetaDestino);

            Assert.True(resultado.Exitoso);
            Assert.NotNull(resultado.Valor);
            Assert.Single(resultado.Valor);
            Assert.True(File.Exists(Path.Combine(carpetaDestino, "datos.csv")));
        }
        finally
        {
            Directory.Delete(carpetaTrabajo, recursive: true);
        }
    }

    private static string CrearCarpetaTemporal()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(carpeta);
        return carpeta;
    }
}
