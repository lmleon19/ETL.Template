using System.Text;

namespace ETL.Common.Servicios;

public sealed class ServicioCsv
{
    public Task<IReadOnlyList<Dictionary<string, string>>> LeerCsvAsync(string rutaArchivo, Encoding encoding, char delimitador, CancellationToken cancellationToken = default)
    {
        _ = rutaArchivo;
        _ = encoding;
        _ = delimitador;
        _ = cancellationToken;
        IReadOnlyList<Dictionary<string, string>> registros = [];
        return Task.FromResult(registros);
    }
}
