namespace ETL.Common.Resultados;

/// <summary>
/// Representa el resultado de una operación que no retorna datos.
/// </summary>
public sealed class ResultadoOperacion
{
    private ResultadoOperacion(bool exitoso, string mensaje, Exception? excepcion)
    {
        Exitoso = exitoso;
        Mensaje = mensaje;
        Excepcion = excepcion;
    }

    /// <summary>
    /// Indica si la operación finalizó correctamente.
    /// </summary>
    public bool Exitoso { get; }

    /// <summary>
    /// Mensaje legible para operación, diagnóstico o registro de logs.
    /// </summary>
    public string Mensaje { get; }

    /// <summary>
    /// Excepción original asociada al error, cuando existe.
    /// </summary>
    public Exception? Excepcion { get; }

    /// <summary>
    /// Crea un resultado exitoso.
    /// </summary>
    /// <param name="mensaje">Mensaje asociado al resultado.</param>
    /// <returns>Resultado exitoso.</returns>
    public static ResultadoOperacion Correcto(string mensaje = "Operación completada.") => new(true, mensaje, null);

    /// <summary>
    /// Crea un resultado fallido.
    /// </summary>
    /// <param name="mensaje">Mensaje asociado al error.</param>
    /// <param name="excepcion">Excepción original, cuando existe.</param>
    /// <returns>Resultado fallido.</returns>
    public static ResultadoOperacion Error(string mensaje, Exception? excepcion = null) => new(false, mensaje, excepcion);
}

/// <summary>
/// Representa el resultado de una operación que retorna un valor.
/// </summary>
/// <typeparam name="T">Tipo del valor retornado por la operación.</typeparam>
public sealed class ResultadoOperacion<T>
{
    private ResultadoOperacion(bool exitoso, string mensaje, T? valor, Exception? excepcion)
    {
        Exitoso = exitoso;
        Mensaje = mensaje;
        Valor = valor;
        Excepcion = excepcion;
    }

    /// <summary>
    /// Indica si la operación finalizó correctamente.
    /// </summary>
    public bool Exitoso { get; }

    /// <summary>
    /// Mensaje legible para operación, diagnóstico o registro de logs.
    /// </summary>
    public string Mensaje { get; }

    /// <summary>
    /// Valor retornado por la operación cuando fue exitosa.
    /// </summary>
    public T? Valor { get; }

    /// <summary>
    /// Excepción original asociada al error, cuando existe.
    /// </summary>
    public Exception? Excepcion { get; }

    /// <summary>
    /// Crea un resultado exitoso con valor.
    /// </summary>
    /// <param name="valor">Valor retornado por la operación.</param>
    /// <param name="mensaje">Mensaje asociado al resultado.</param>
    /// <returns>Resultado exitoso con valor.</returns>
    public static ResultadoOperacion<T> Correcto(T valor, string mensaje = "Operación completada.") => new(true, mensaje, valor, null);

    /// <summary>
    /// Crea un resultado fallido sin valor.
    /// </summary>
    /// <param name="mensaje">Mensaje asociado al error.</param>
    /// <param name="excepcion">Excepción original, cuando existe.</param>
    /// <returns>Resultado fallido sin valor.</returns>
    public static ResultadoOperacion<T> Error(string mensaje, Exception? excepcion = null) => new(false, mensaje, default, excepcion);
}
