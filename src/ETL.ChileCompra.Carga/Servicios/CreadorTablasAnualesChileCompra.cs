using ETL.ChileCompra.Carga.Model;
using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class CreadorTablasAnualesChileCompra
{
    private readonly OpcionesChileCompra opciones;
    private readonly IConfiguration configuration;
    private readonly GeneradorSqlTablasStage generadorSqlTablasStage;
    private readonly ILogger<CreadorTablasAnualesChileCompra> logger;

    public CreadorTablasAnualesChileCompra(
        IOptions<OpcionesChileCompra> opciones,
        IConfiguration configuration,
        GeneradorSqlTablasStage generadorSqlTablasStage,
        ILogger<CreadorTablasAnualesChileCompra> logger)
    {
        this.opciones = opciones.Value;
        this.configuration = configuration;
        this.generadorSqlTablasStage = generadorSqlTablasStage;
        this.logger = logger;
    }

    public async Task<ResultadoOperacion> CrearAsync(IEnumerable<PeriodoProceso> periodos, CancellationToken cancellationToken = default)
    {
        string cadenaConexion = configuration.GetConnectionString("ChileCompra") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            return ResultadoOperacion.Error("No existe cadena de conexion ChileCompra.");
        }

        try
        {
            string[] sufijos = periodos
                .Select(p => p.SufijoAnio)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            foreach (string sufijo in sufijos)
            {
                await CrearTablaSiNoExisteAsync<RegistroLicitacionStage>(
                    conexion,
                    $"{opciones.Tablas.PrefijoLicitaciones}{sufijo}",
                    cancellationToken);

                await CrearTablaSiNoExisteAsync<RegistroOrdenCompraStage>(
                    conexion,
                    $"{opciones.Tablas.PrefijoOrdenesCompra}{sufijo}",
                    cancellationToken);
            }

            return ResultadoOperacion.Correcto("Tablas anuales verificadas correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al crear tablas anuales ChileCompra.");
            return ResultadoOperacion.Error("No fue posible crear o verificar las tablas anuales.", ex);
        }
    }

    private async Task CrearTablaSiNoExisteAsync<TRegistroStage>(
        SqlConnection conexion,
        string nombreTabla,
        CancellationToken cancellationToken)
    {
        string esquema = ObtenerEsquema(nombreTabla);
        string tabla = ObtenerTabla(nombreTabla);
        string createTable = generadorSqlTablasStage.GenerarCreateTable<TRegistroStage>(DelimitarNombreCompleto(nombreTabla));
        string sql = $"""
            IF OBJECT_ID(N'{esquema}.{tabla}', N'U') IS NULL
            BEGIN
                {createTable}
            END
            """;

        logger.LogInformation("Verificando tabla anual {Tabla}.", nombreTabla);

        await using SqlCommand comando = new(sql, conexion)
        {
            CommandTimeout = 0
        };

        await comando.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string ObtenerEsquema(string nombreTabla)
    {
        string[] partes = nombreTabla.Split('.', 2);
        return partes.Length == 2 ? partes[0] : "dbo";
    }

    private static string ObtenerTabla(string nombreTabla)
    {
        string[] partes = nombreTabla.Split('.', 2);
        return partes.Length == 2 ? partes[1] : partes[0];
    }

    private static string DelimitarNombreCompleto(string nombreTabla)
    {
        string[] partes = nombreTabla.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return partes.Length == 2
            ? $"{Delimitar(partes[0])}.{Delimitar(partes[1])}"
            : Delimitar(nombreTabla);
    }

    private static string Delimitar(string identificador) => $"[{identificador.Replace("]", "]]", StringComparison.Ordinal)}]";
}

