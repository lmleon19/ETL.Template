using ETL.Common.Resultados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class ProveedorIdOrPortalInstitucion
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ProveedorIdOrPortalInstitucion> logger;
    private Dictionary<int, string> idOrPortalPorRut = [];

    public ProveedorIdOrPortalInstitucion(IConfiguration configuration, ILogger<ProveedorIdOrPortalInstitucion> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
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
                MST.PDT_Idor AS IdorMST,
                CRM.Rut
            FROM CRM.dbo.Instituciones CRM
            LEFT JOIN Maestro.dbo.tblOrganismo MST 
                ON CRM.ID = MST.CRM_Id
            WHERE CRM.Rut IS NOT NULL
              AND (CRM.IDORPortal <> '' OR MST.PDT_Idor IS NOT NULL)
              AND CRM.Habilitada = 0;
            """;

        try
        {
            Dictionary<int, string> resultado = [];

            await using SqlConnection conexion = new(cadenaConexion);
            await conexion.OpenAsync(cancellationToken);

            await using SqlCommand comando = new(sql, conexion);
            comando.CommandTimeout = 0;

            await using SqlDataReader reader = await comando.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                string? rutTexto = reader["Rut"]?.ToString();
                string? idorMst = reader["IdorMST"]?.ToString();
                string? idorCrm = reader["IdorCRM"]?.ToString();
                string? idOrPortal = !string.IsNullOrWhiteSpace(idorMst) ? idorMst : idorCrm;
                int rut = NormalizadorRutChile.LimpiarNumero(rutTexto);

                if (rut > 0 && !string.IsNullOrWhiteSpace(idOrPortal))
                {
                    resultado[rut] = idOrPortal;
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

    public string? Obtener(int rutInstitucion) =>
        idOrPortalPorRut.TryGetValue(rutInstitucion, out string? idOrPortal) ? idOrPortal : null;
}
