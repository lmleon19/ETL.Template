namespace ETL.ChileCompra.CargaInfoTransparencia;

public sealed class OpcionesCargaInfoTransparencia
{
    public int MesesCerradosACargar { get; set; } = 4;
    public string FormatoSufijoTablaAnual { get; set; } = "yy";
    public OpcionesTablasCargaInfoTransparencia Tablas { get; set; } = new();
}

public sealed class OpcionesTablasCargaInfoTransparencia
{
    public string PrefijoLicitacionesOrigen { get; set; } = "dbo.DatosAbiertos_Licitaciones_";
    public string PrefijoOCOrigen { get; set; } = "dbo.DatosAbiertos_OC_";
    public string LicitacionDestino { get; set; } = "dbo.EX_MP_Licitacion";
    public string LicitacionDetalleDestino { get; set; } = "dbo.EX_MP_LicitacionDetalle";
    public string OCDestino { get; set; } = "dbo.EX_MP_OrdenCompra";
    public string OCDetalleDestino { get; set; } = "dbo.EX_MP_OrdenCompraDetalle";
}
