namespace ETL.NombreProceso;

public sealed class OpcionesProceso
{
    public string UrlDescarga { get; set; } = string.Empty;
    public string CarpetaTrabajo { get; set; } = "datos";
    public string NombreArchivoZip { get; set; } = "origen.zip";
    public string PatronCsv { get; set; } = "*.csv";
    public char Delimitador { get; set; } = ';';
    public string TablaStage { get; set; } = string.Empty;
    public string ProcedimientoFinal { get; set; } = string.Empty;
    public string[] ColumnasObligatorias { get; set; } = [];
}
