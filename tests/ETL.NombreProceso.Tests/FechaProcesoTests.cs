using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class FechaProcesoTests
{
    [Theory]
    [InlineData("2026-07-03")]
    [InlineData("20260703")]
    [InlineData("03-07-2026")]
    [InlineData("03/07/2026")]
    public void ObtenerFechaProceso_CuandoFormatoEsValido_RetornaFecha(string valor)
    {
        FechaProceso servicio = new();

        DateOnly fecha = servicio.ObtenerFechaProceso(valor);

        Assert.Equal(new DateOnly(2026, 7, 3), fecha);
    }

    [Fact]
    public void FormatearFechaProceso_RetornaFormatoCompacto()
    {
        FechaProceso servicio = new();

        string resultado = servicio.FormatearFechaProceso(new DateOnly(2026, 7, 3));

        Assert.Equal("20260703", resultado);
    }
}
