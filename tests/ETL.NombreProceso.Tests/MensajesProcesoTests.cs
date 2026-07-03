using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class MensajesProcesoTests
{
    [Fact]
    public void InicioProceso_RetornaMensajeEstandar()
    {
        MensajesProceso servicio = new();

        string mensaje = servicio.InicioProceso("ETL.Prueba");

        Assert.Equal("Inicio proceso ETL: ETL.Prueba.", mensaje);
    }

    [Fact]
    public void FinProceso_IncluyeDuracionFormateada()
    {
        MensajesProceso servicio = new();

        string mensaje = servicio.FinProceso("ETL.Prueba", TimeSpan.FromSeconds(65));

        Assert.Equal("Fin proceso ETL: ETL.Prueba. Duración: 00:01:05.", mensaje);
    }

    [Fact]
    public void Error_RetornaMensajeEstandar()
    {
        MensajesProceso servicio = new();

        string mensaje = servicio.Error("Descarga", "No se encontró archivo.");

        Assert.Equal("Error en Descarga: No se encontró archivo.", mensaje);
    }
}
