using System.Text;
using System.Reflection;
using ETL.ChileCompra.Carga.Model;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class GeneradorSqlTablasStage
{
    public string GenerarCreateTable<TRegistroStage>(string nombreTabla)
    {
        IReadOnlyList<ColumnaStageAttribute> columnas = ObtenerColumnas<TRegistroStage>();

        return GenerarCreateTable(nombreTabla, columnas);
    }

    public IReadOnlyList<ColumnaStageAttribute> ObtenerColumnas<TRegistroStage>() =>
        typeof(TRegistroStage)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(p => p.GetCustomAttribute<ColumnaStageAttribute>())
            .Where(a => a is not null)
            .OrderBy(a => a!.Posicion)
            .Select(a => a!)
            .ToArray();

    public string GenerarCreateTable(string nombreTabla, IEnumerable<ColumnaStageAttribute> columnas)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreTabla);
        ArgumentNullException.ThrowIfNull(columnas);

        ColumnaStageAttribute[] columnasMaterializadas = columnas.ToArray();

        if (columnasMaterializadas.Length == 0)
        {
            throw new ArgumentException("Debe existir al menos una columna para crear la tabla.", nameof(columnas));
        }

        StringBuilder sql = new();

        sql.AppendLine($"CREATE TABLE {nombreTabla}");
        sql.AppendLine("(");

        for (int indice = 0; indice < columnasMaterializadas.Length; indice++)
        {
            ColumnaStageAttribute columna = columnasMaterializadas[indice];
            string separador = indice == columnasMaterializadas.Length - 1 ? string.Empty : ",";

            sql.AppendLine($"    {DelimitarColumna(columna.Nombre)} {columna.TipoSql}{separador}");
        }

        sql.AppendLine(");");

        return sql.ToString();
    }

    private static string DelimitarColumna(string nombreColumna) => $"[{nombreColumna.Replace("]", "]]", StringComparison.Ordinal)}]";
}
