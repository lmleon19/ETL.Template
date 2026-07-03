using ETL.Common.Validacion;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class ValidadorArchivoTests
{
    [Fact]
    public void ValidarExiste_CuandoArchivoExiste_RetornaCorrecto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            ValidadorArchivo validador = new();

            var resultado = validador.ValidarExiste(rutaArchivo);

            Assert.True(resultado.Exitoso);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public void ValidarNoVacio_CuandoArchivoEstaVacio_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            ValidadorArchivo validador = new();

            var resultado = validador.ValidarNoVacio(rutaArchivo);

            Assert.False(resultado.Exitoso);
            Assert.Contains("vacío", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public void ValidarExtension_CuandoExtensionEstaPermitida_RetornaCorrecto()
    {
        ValidadorArchivo validador = new();

        var resultado = validador.ValidarExtension("archivo.csv", "csv", ".txt");

        Assert.True(resultado.Exitoso);
    }

    [Fact]
    public void ValidarExtension_CuandoExtensionNoEstaPermitida_RetornaError()
    {
        ValidadorArchivo validador = new();

        var resultado = validador.ValidarExtension("archivo.xlsx", ".csv", ".txt");

        Assert.False(resultado.Exitoso);
        Assert.Contains(".xlsx", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidarTamanoMaximo_CuandoArchivoSuperaMaximo_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            File.WriteAllText(rutaArchivo, "contenido");
            ValidadorArchivo validador = new();

            var resultado = validador.ValidarTamanoMaximo(rutaArchivo, bytesMaximos: 1);

            Assert.False(resultado.Exitoso);
            Assert.Contains("supera", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }
}
