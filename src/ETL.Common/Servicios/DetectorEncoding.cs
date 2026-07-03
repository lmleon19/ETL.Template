using System.Text;

namespace ETL.Common.Servicios;

public sealed class DetectorEncoding
{
    public Encoding DetectarEncoding(string rutaArchivo)
    {
        _ = rutaArchivo;
        return Encoding.UTF8;
    }
}
