using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class HashArchivoTests
{
    [Fact]
    public async Task CalcularSha256Async_CuandoArchivoExiste_RetornaHashEsperado()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "abc");

            HashArchivo servicio = new();

            string hash = await servicio.CalcularSha256Async(rutaArchivo);

            Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hash);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }
}
