namespace ETL.Common.Servicios;

internal static class CsvLineParser
{
    public static IReadOnlyList<string> SepararColumnas(string registro, char delimitador, char calificadorTexto = '"')
    {
        List<string> columnas = [];
        bool dentroDeComillas = false;
        int inicioColumna = 0;
        int indice = 0;

        while (indice < registro.Length)
        {
            char caracter = registro[indice];

            if (caracter == calificadorTexto)
            {
                if (dentroDeComillas && indice + 1 < registro.Length && registro[indice + 1] == calificadorTexto)
                {
                    indice += 2;
                    continue;
                }

                dentroDeComillas = !dentroDeComillas;
            }
            else if (caracter == delimitador && !dentroDeComillas)
            {
                columnas.Add(NormalizarValor(registro[inicioColumna..indice], calificadorTexto));
                inicioColumna = indice + 1;
            }

            indice++;
        }

        columnas.Add(NormalizarValor(registro[inicioColumna..], calificadorTexto));

        if (dentroDeComillas)
        {
            throw new InvalidDataException("El registro CSV contiene calificador de texto sin cerrar.");
        }

        return columnas;
    }

    public static async Task<string?> LeerRegistroAsync(TextReader reader, char calificadorTexto = '"', CancellationToken cancellationToken = default)
    {
        string? primeraLinea = await reader.ReadLineAsync(cancellationToken);

        if (primeraLinea is null)
        {
            return null;
        }

        string registro = primeraLinea;

        // Un registro CSV puede ocupar varias líneas físicas cuando un campo viene entre comillas.
        while (TieneCalificadorAbierto(registro, calificadorTexto))
        {
            string? siguienteLinea = await reader.ReadLineAsync(cancellationToken);

            if (siguienteLinea is null)
            {
                throw new InvalidDataException("El registro CSV contiene calificador de texto sin cerrar.");
            }

            registro += Environment.NewLine + siguienteLinea;
        }

        return registro;
    }

    private static bool TieneCalificadorAbierto(string registro, char calificadorTexto)
    {
        bool dentroDeComillas = false;
        int indice = 0;

        while (indice < registro.Length)
        {
            if (registro[indice] == calificadorTexto)
            {
                if (dentroDeComillas && indice + 1 < registro.Length && registro[indice + 1] == calificadorTexto)
                {
                    indice += 2;
                    continue;
                }

                dentroDeComillas = !dentroDeComillas;
            }

            indice++;
        }

        return dentroDeComillas;
    }

    private static string NormalizarValor(string valor, char calificadorTexto)
    {
        string valorNormalizado = valor.Trim();

        if (valorNormalizado.Length >= 2 &&
            valorNormalizado[0] == calificadorTexto &&
            valorNormalizado[^1] == calificadorTexto)
        {
            string calificadorDuplicado = new(calificadorTexto, 2);
            valorNormalizado = valorNormalizado[1..^1].Replace(calificadorDuplicado, calificadorTexto.ToString(), StringComparison.Ordinal);
        }

        return valorNormalizado;
    }
}
