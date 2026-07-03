namespace ETL.Common.Servicios;

public sealed class ServicioSistemaArchivos
{
    public void CrearCarpetaSiNoExiste(string rutaCarpeta) => _ = rutaCarpeta;

    public void EliminarCarpetaSiExiste(string rutaCarpeta) => _ = rutaCarpeta;

    public string BuscarPrimerArchivo(string rutaCarpeta, string patron)
    {
        _ = rutaCarpeta;
        _ = patron;
        return string.Empty;
    }
}
