using System.Text;
using ETL.Common.Servicios;
using Xunit;

namespace ETL.NombreProceso.Tests;

public sealed class CsvTests
{
    [Fact]
    public async Task LeerCsvAsync_CuandoArchivoTieneEncabezado_RetornaRegistros()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre;Glosa\n1;Ana;\"Texto; con delimitador\"");

            Csv servicio = new();

            IReadOnlyList<Dictionary<string, string>> registros = await servicio.LeerCsvAsync(rutaArchivo, Encoding.UTF8, ';');

            Assert.Single(registros);
            Assert.Equal("1", registros[0]["Id"]);
            Assert.Equal("Ana", registros[0]["Nombre"]);
            Assert.Equal("Texto; con delimitador", registros[0]["Glosa"]);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task LeerCsvAsync_CuandoCampoTieneSaltoLineaEntreComillas_RetornaRegistroCompleto()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Descripcion\n1;\"Texto con\nsalto de línea\"\n2;Normal");

            Csv servicio = new();

            IReadOnlyList<Dictionary<string, string>> registros = await servicio.LeerCsvAsync(rutaArchivo, Encoding.UTF8, ';');

            Assert.Equal(2, registros.Count);
            Assert.Equal($"Texto con{Environment.NewLine}salto de línea", registros[0]["Descripcion"]);
            Assert.Equal("Normal", registros[1]["Descripcion"]);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task LeerCsvAsync_CuandoUsaCalificadorTextoPersonalizado_RetornaRegistros()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Descripcion\n1;'Texto; con separador'");

            Csv servicio = new();

            IReadOnlyList<Dictionary<string, string>> registros = await servicio.LeerCsvAsync(
                rutaArchivo,
                Encoding.UTF8,
                ';',
                calificadorTexto: '\'');

            Assert.Single(registros);
            Assert.Equal("Texto; con separador", registros[0]["Descripcion"]);
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }

    [Fact]
    public async Task LeerCsvAsync_CuandoFilaTieneCantidadColumnasDistinta_LanzaError()
    {
        string rutaArchivo = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(rutaArchivo, "Id;Nombre\n1;Ana;Extra");

            Csv servicio = new();

            await Assert.ThrowsAsync<InvalidDataException>(() => servicio.LeerCsvAsync(rutaArchivo, Encoding.UTF8, ';'));
        }
        finally
        {
            File.Delete(rutaArchivo);
        }
    }
}
