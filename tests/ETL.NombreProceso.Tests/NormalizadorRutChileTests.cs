using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class NormalizadorRutChileTests
{
    [Theory]
    [InlineData("77.337.544-5", 77337544, "5")]
    public void Limpiar_CuandoRutTieneFormatoConGuiones_RetornaNumeroYDigitoVerificadorCorrectos(
        string rut,
        int numeroEsperado,
        string digitoVerificadorEsperado)
    {
        NormalizadorRutChile normalizador = new();

        var resultado = normalizador.Limpiar(rut);

        Assert.Equal(numeroEsperado, resultado.Numero);
        Assert.Equal(digitoVerificadorEsperado, resultado.DigitoVerificador);
        Assert.Equal(rut, resultado.TextoOriginal);
        Assert.True(resultado.EstructuraValida);
        Assert.True(resultado.EsValido);
    }

    [Fact]
    public void ValidarEstructura_CuandoRutTieneDigitoVerificadorIncorrecto_RetornaNumeroYDigito()
    {
        NormalizadorRutChile normalizador = new();

        var resultado = normalizador.ValidarEstructura("70.934.000-7");

        Assert.Equal(70934000, resultado.Numero);
        Assert.Equal("7", resultado.DigitoVerificador);
        Assert.True(resultado.EstructuraValida);
        Assert.False(resultado.EsValido);
    }

    [Fact]
    public void ValidarRutCompleto_CuandoRutTieneDigitoVerificadorIncorrecto_RetornaInvalido()
    {
        NormalizadorRutChile normalizador = new();

        var resultado = normalizador.ValidarRutCompleto("70.934.000-7");

        Assert.Equal(70934000, resultado.Numero);
        Assert.Equal("7", resultado.DigitoVerificador);
        Assert.True(resultado.EstructuraValida);
        Assert.False(resultado.EsValido);
    }

    [Theory]
    [InlineData("77.337.544-4")]
    [InlineData("090425-118-3")]
    [InlineData("0623-090425-118-2")]
    [InlineData("0623-090425-118-3")]
    [InlineData("NO-ES-RUT")]
    public void Limpiar_CuandoNoEsRutValido_RetornaMarcadoComoInvalido(string rut)
    {
        NormalizadorRutChile normalizador = new();

        var resultado = normalizador.Limpiar(rut);

        Assert.False(resultado.EsValido);
    }
}
