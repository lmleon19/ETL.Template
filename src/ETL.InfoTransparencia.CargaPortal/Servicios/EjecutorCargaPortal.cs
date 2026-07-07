using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.InfoTransparencia.CargaPortal.Servicios;

public sealed class EjecutorCargaPortal(
    IConfiguration configuration,
    IOptions<OpcionesCargaPortal> opciones,
    ILogger<EjecutorCargaPortal> logger)
{
    public async Task EjecutarAsync(
        OpcionesCargaTablaPortal carga,
        CancellationToken cancellationToken = default)
    {
        OpcionesCargaPortal opcionesProceso = opciones.Value;
        string cadenaConexion = configuration.GetConnectionString(opcionesProceso.NombreConexionDestino) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                $"No se encontro la cadena de conexion {opcionesProceso.NombreConexionDestino}.");
        }

        logger.LogInformation("Iniciando carga {Carga} mediante {Procedimiento}.", carga.Nombre, carga.Procedimiento);

        await using SqlConnection conexion = new(cadenaConexion);
        await conexion.OpenAsync(cancellationToken);

        await using SqlCommand comando = conexion.CreateCommand();
        comando.CommandText = carga.Procedimiento;
        comando.CommandType = CommandType.StoredProcedure;
        comando.CommandTimeout = opcionesProceso.TimeoutComandoSegundos;

        int filasAfectadas = await comando.ExecuteNonQueryAsync(cancellationToken);

        logger.LogInformation(
            "Carga {Carga} finalizada. Filas afectadas reportadas por SQL Server: {FilasAfectadas}.",
            carga.Nombre,
            filasAfectadas);
    }
}
