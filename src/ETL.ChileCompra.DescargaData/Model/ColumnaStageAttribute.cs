namespace ETL.ChileCompra.DescargaData.Model;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnaStageAttribute : Attribute
{
    public ColumnaStageAttribute(
        int posicion,
        string nombre,
        string tipoSql,
        OrigenColumnaStage origen,
        string? columnaCsv = null)
    {
        Posicion = posicion;
        Nombre = nombre;
        TipoSql = tipoSql;
        Origen = origen;
        ColumnaCsv = columnaCsv;
    }

    public int Posicion { get; }
    public string Nombre { get; }
    public string TipoSql { get; }
    public OrigenColumnaStage Origen { get; }
    public string? ColumnaCsv { get; }
    public string NombreCsv => string.IsNullOrWhiteSpace(ColumnaCsv) ? Nombre : ColumnaCsv;
}
