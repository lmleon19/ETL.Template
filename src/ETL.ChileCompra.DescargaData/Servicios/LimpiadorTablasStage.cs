using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class LimpiadorTablasStage
{
    private readonly OpcionesChileCompra opciones;
    private readonly IConfiguration configuration;
    private readonly ILogger<LimpiadorTablasStage> logger;

    public LimpiadorTablasStage(
        IOptions<OpcionesChileCompra> opciones,
        IConfiguration configuration,
        ILogger<LimpiadorTablasStage> logger)
    {
        this.opciones = opciones.Value;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<ResultadoOperacion> LimpiarAsync(CancellationToken cancellationToken = default)
    {
        if (!opciones.EjecutarLimpiezaTablasStage)
        {
            logger.LogWarning("Limpieza de tablas Stage omitida por configuracion.");
            return ResultadoOperacion.Correcto("Limpieza de Stage omitida por configuracion.");
        }

        string cadenaConexion = configuration.GetConnectionString("ChileCompra") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            return ResultadoOperacion.Error("No existe cadena de conexion ChileCompra.");
        }

        try
        {
            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            string comandoSql = $"""
                TRUNCATE TABLE {opciones.Tablas.StageLicitaciones};
                TRUNCATE TABLE {opciones.Tablas.StageOC};
                """;

            await using SqlCommand comando = new(comandoSql, conexion)
            {
                CommandTimeout = 0
            };

            await comando.ExecuteNonQueryAsync(cancellationToken);

            return ResultadoOperacion.Correcto("Tablas Stage limpiadas correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al limpiar tablas Stage.");
            return ResultadoOperacion.Error("No fue posible limpiar las tablas Stage.", ex);
        }
    }
}

