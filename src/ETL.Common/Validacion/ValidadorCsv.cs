using System.Text;
using ETL.Common.Resultados;

namespace ETL.Common.Validacion;

public sealed class ValidadorCsv
{
    public Task<ResultadoOperacion> ValidarColumnasObligatoriasAsync(string rutaArchivo, Encoding encoding, char delimitador, IEnumerable<string> columnasObligatorias, CancellationToken cancellationToken = default)
    {
        _ = rutaArchivo;
        _ = encoding;
        _ = delimitador;
        _ = columnasObligatorias;
        _ = cancellationToken;
        return Task.FromResult(ResultadoOperacion.Error("Validación CSV pendiente de implementar."));
    }
}
