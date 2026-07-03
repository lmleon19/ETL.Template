using ETL.Common.Resultados;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class ResultadoOperacionTests
{
    [Fact]
    public void Error_CuandoIncluyeExcepcion_ConservaExcepcionOriginal()
    {
        InvalidOperationException excepcion = new("Error original.");

        ResultadoOperacion resultado = ResultadoOperacion.Error("Error controlado.", excepcion);

        Assert.False(resultado.Exitoso);
        Assert.Equal("Error controlado.", resultado.Mensaje);
        Assert.Same(excepcion, resultado.Excepcion);
    }
}
