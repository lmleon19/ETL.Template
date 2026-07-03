using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class SistemaArchivosTests
{
    [Fact]
    public void BuscarPrimerArchivo_CuandoExisteArchivoConPatron_RetornaRuta()
    {
        string carpeta = CrearCarpetaTemporal();

        try
        {
            string rutaArchivo = Path.Combine(carpeta, "datos.csv");
            File.WriteAllText(rutaArchivo, "id;nombre");

            SistemaArchivos servicio = new();

            string resultado = servicio.BuscarPrimerArchivo(carpeta, "*.csv");

            Assert.Equal(rutaArchivo, resultado);
        }
        finally
        {
            Directory.Delete(carpeta, recursive: true);
        }
    }

    [Fact]
    public void LimpiarCarpeta_CuandoCarpetaTieneArchivos_LaDejaVacia()
    {
        string carpeta = CrearCarpetaTemporal();

        try
        {
            File.WriteAllText(Path.Combine(carpeta, "archivo.txt"), "contenido");

            SistemaArchivos servicio = new();

            servicio.LimpiarCarpeta(carpeta);

            Assert.True(Directory.Exists(carpeta));
            Assert.Empty(Directory.EnumerateFiles(carpeta));
        }
        finally
        {
            Directory.Delete(carpeta, recursive: true);
        }
    }

    private static string CrearCarpetaTemporal()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(carpeta);
        return carpeta;
    }
}
