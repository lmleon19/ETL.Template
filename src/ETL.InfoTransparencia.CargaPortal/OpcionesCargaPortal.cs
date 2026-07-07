namespace ETL.InfoTransparencia.CargaPortal;

public sealed class OpcionesCargaPortal
{
    public string NombreConexionDestino { get; set; } = "InfoTransparencia";
    public int TimeoutComandoSegundos { get; set; }
    public List<OpcionesCargaTablaPortal> Cargas { get; set; } = [];
}

public sealed class OpcionesCargaTablaPortal
{
    public string Nombre { get; set; } = string.Empty;
    public string Procedimiento { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
