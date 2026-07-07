using ETL.Common.Resultados;
using ETL.Common.Servicios;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETL.ChileCompra.DescargaData.Servicios;

public sealed class ProveedorIdOrPortalInstitucion
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ProveedorIdOrPortalInstitucion> logger;
    private readonly NormalizadorRutChile normalizadorRut;
    private Dictionary<string, string> idOrPortalPorRut = [];

    public ProveedorIdOrPortalInstitucion(
        IConfiguration configuration,
        ILogger<ProveedorIdOrPortalInstitucion> logger,
        NormalizadorRutChile normalizadorRut)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.normalizadorRut = normalizadorRut;
    }

    public async Task<ResultadoOperacion> CargarAsync(CancellationToken cancellationToken = default)
    {
        string cadenaConexion = configuration.GetConnectionString("CRM") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            return ResultadoOperacion.Error("No existe cadena de conexion CRM.");
        }

        const string sql = """
            SELECT
                CRM.IDORPortal AS IdorCRM,
                CRM.Rut
            FROM CRM.dbo.Instituciones CRM
            WHERE CRM.Rut IS NOT NULL
              AND CRM.IDORPortal <> ''
              AND CRM.Habilitada = 0;
            """;

        try
        {
            Dictionary<string, string> resultado = [];

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            await using SqlCommand comando = new(sql, conexion);
            comando.CommandTimeout = 0;

            await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                string? rutTexto = reader["Rut"]?.ToString();
                string? idorCrm = reader["IdorCRM"]?.ToString();
                string? idOrPortal = !string.IsNullOrWhiteSpace(idorCrm) ? idorCrm : null;
                ValorRut rut = normalizadorRut.ValidarEstructura(rutTexto);
                string? claveRut = ObtenerClaveRut(rut);

                if (!string.IsNullOrWhiteSpace(claveRut) && !string.IsNullOrWhiteSpace(idOrPortal))
                {
                    resultado[claveRut] = idOrPortal;
                }
            }

            idOrPortalPorRut = resultado;
            logger.LogInformation("IdOrPortal cargados desde CRM/Maestro: {Cantidad}.", idOrPortalPorRut.Count);

            return ResultadoOperacion.Correcto("Lookup IdOrPortal cargado correctamente.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error al cargar lookup IdOrPortal.");
            return ResultadoOperacion.Error("No fue posible cargar el lookup IdOrPortal.", ex);
        }
    }

    public string? Obtener(ValorRut rutInstitucion)
    {
        string? claveRut = ObtenerClaveRut(rutInstitucion);

        return claveRut is not null && idOrPortalPorRut.TryGetValue(claveRut, out string? idOrPortal)
            ? idOrPortal
            : null;
    }

    private static string? ObtenerClaveRut(ValorRut rut) =>
        rut.EstructuraValida && !string.IsNullOrWhiteSpace(rut.DigitoVerificador)
            ? $"{rut.Numero}{rut.DigitoVerificador.ToUpperInvariant()}"
            : null;
}
