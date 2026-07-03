using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class NormalizadorTextoTests
{
    [Fact]
    public void NormalizarEspacios_CuandoTextoTieneEspaciosExtra_RetornaTextoLimpio()
    {
        NormalizadorTexto normalizador = new();

        string resultado = normalizador.NormalizarEspacios("  hola   mundo  ");

        Assert.Equal("hola mundo", resultado);
    }

    [Fact]
    public void NormalizarNombreColumna_QuitaAcentosYNormalizaMayusculas()
    {
        NormalizadorTexto normalizador = new();

        string resultado = normalizador.NormalizarNombreColumna("  Fecha Última Compra  ");

        Assert.Equal("FECHA ULTIMA COMPRA", resultado);
    }
}
