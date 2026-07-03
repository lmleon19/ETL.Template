namespace ETL.ChileCompra.Carga;

public sealed class OpcionesChileCompra
{
    public int MesesCerradosACargar { get; set; } = 4;
    public string DelimitadorCsv { get; set; } = ";";
    public string EncodingFallback { get; set; } = "windows-1252";
    public bool EjecutarLimpiezaTablasStage { get; set; }
    public OpcionesUrlsChileCompra Urls { get; set; } = new();
    public OpcionesCarpetasChileCompra Carpetas { get; set; } = new();
    public OpcionesTablasChileCompra Tablas { get; set; } = new();
    public OpcionesProcedimientosChileCompra Procedimientos { get; set; } = new();
}

public sealed class OpcionesUrlsChileCompra
{
    public string OrdenesCompra { get; set; } = string.Empty;
    public string Licitaciones { get; set; } = string.Empty;
    public string ParidadMoneda { get; set; } = string.Empty;
}

public sealed class OpcionesCarpetasChileCompra
{
    public string BaseTrabajo { get; set; } = "datos/trabajo";
    public string BaseFinal { get; set; } = "datos/final";
    public string Licitaciones { get; set; } = "LIC";
    public string OrdenesCompra { get; set; } = "OC";
    public string Zip { get; set; } = "zip";
    public string Extraidos { get; set; } = "extraidos";
}

public sealed class OpcionesTablasChileCompra
{
    public string StageLicitaciones { get; set; } = "dbo.DatosAbiertos_Licitaciones_Stage";
    public string StageOrdenesCompra { get; set; } = "dbo.DatosAbiertos_OC_Stage";
    public string PrefijoLicitaciones { get; set; } = "dbo.DatosAbiertos_Licitaciones_";
    public string PrefijoOrdenesCompra { get; set; } = "dbo.DatosAbiertos_OC_";
}

public sealed class OpcionesProcedimientosChileCompra
{
    public string TraspasoFinal { get; set; } = string.Empty;
}
