using ETL.ChileCompra.Carga.Servicios;
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
        Assert.True(resultado.EsValido);
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
