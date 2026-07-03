using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class RutaTrabajoTests
{
    [Fact]
    public void CombinarRuta_IgnoraPartesVacias()
    {
        RutaTrabajo servicio = new();

        string ruta = servicio.CombinarRuta("base", "", "entrada", "archivo.csv");

        Assert.Equal(Path.Combine("base", "entrada", "archivo.csv"), ruta);
    }

    [Fact]
    public void CrearCarpetaPorFecha_UsaFormatoCompacto()
    {
        RutaTrabajo servicio = new();

        string ruta = servicio.CrearCarpetaPorFecha("base", new DateOnly(2026, 7, 3));

        Assert.Equal(Path.Combine("base", "20260703"), ruta);
    }
}
