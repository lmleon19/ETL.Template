namespace ETL.Common.Resultados;

public sealed class ResultadoOperacion
{
    private ResultadoOperacion(bool exitoso, string mensaje)
    {
        Exitoso = exitoso;
        Mensaje = mensaje;
    }

    public bool Exitoso { get; }
    public string Mensaje { get; }

    public static ResultadoOperacion Correcto(string mensaje = "Operación completada.") => new(true, mensaje);

    public static ResultadoOperacion Error(string mensaje) => new(false, mensaje);
}

public sealed class ResultadoOperacion<T>
{
    private ResultadoOperacion(bool exitoso, string mensaje, T? valor)
    {
        Exitoso = exitoso;
        Mensaje = mensaje;
        Valor = valor;
    }

    public bool Exitoso { get; }
    public string Mensaje { get; }
    public T? Valor { get; }

    public static ResultadoOperacion<T> Correcto(T valor, string mensaje = "Operación completada.") => new(true, mensaje, valor);

    public static ResultadoOperacion<T> Error(string mensaje) => new(false, mensaje, default);
}
