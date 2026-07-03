using System.Text;
using ETL.Common.Validacion;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class ValidadorCsvDetalleTests
{
    [Fact]
    public async Task ValidarEstructuraBasicaAsync_CuandoCsvEsValido_RetornaCorrecto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana\n2;Luis");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarEstructuraBasicaAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                cantidadMinimaFilas: 2);

            Assert.True(resultado.Exitoso);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarEstructuraBasicaAsync_CuandoEncabezadoTieneColumnasDuplicadas_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre;id\n1;Ana;2");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarEstructuraBasicaAsync(rutaArchivo, Encoding.UTF8, ';');

            Assert.False(resultado.Exitoso);
            Assert.Contains("duplicadas", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarEstructuraBasicaAsync_CuandoFilaTieneCantidadColumnasDistinta_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana;Extra");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarEstructuraBasicaAsync(rutaArchivo, Encoding.UTF8, ';');

            Assert.False(resultado.Exitoso);
            Assert.Contains("registro 2", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarEstructuraBasicaAsync_CuandoCampoTieneSaltoLineaEntreComillas_RetornaCorrecto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Descripcion\n1;\"Texto con\nsalto de línea\"");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarEstructuraBasicaAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                cantidadMinimaFilas: 1);

            Assert.True(resultado.Exitoso);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarEstructuraBasicaAsync_CuandoNoCumpleMinimoFilas_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarEstructuraBasicaAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                cantidadMinimaFilas: 2);

            Assert.False(resultado.Exitoso);
            Assert.Contains("requieren al menos 2", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarColumnasObligatoriasAsync_CuandoFaltanColumnas_RetornaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarColumnasObligatoriasAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                ["Id", "Fecha"]);

            Assert.False(resultado.Exitoso);
            Assert.Contains("Fecha", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task ValidarColumnasObligatoriasAsync_CuandoExistenColumnas_RetornaCorrecto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana");

            ValidadorCsv validador = new();

            var resultado = await validador.ValidarColumnasObligatoriasAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                ["Id", "Nombre"]);

            Assert.True(resultado.Exitoso);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }
}
