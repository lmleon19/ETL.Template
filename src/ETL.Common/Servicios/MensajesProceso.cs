namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para construir mensajes operativos estándar de procesos ETL.
/// </summary>
public sealed class MensajesProceso
{
    /// <summary>
    /// Crea un mensaje estándar de inicio de proceso.
    /// </summary>
    /// <param name="nombreProceso">Nombre del proceso ETL.</param>
    /// <returns>Mensaje de inicio.</returns>
    public string InicioProceso(string nombreProceso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreProceso);

        return $"Inicio proceso ETL: {nombreProceso}.";
    }

    /// <summary>
    /// Crea un mensaje estándar de fin de proceso.
    /// </summary>
    /// <param name="nombreProceso">Nombre del proceso ETL.</param>
    /// <param name="duracion">Duración total del proceso.</param>
    /// <returns>Mensaje de fin.</returns>
    public string FinProceso(string nombreProceso, TimeSpan duracion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreProceso);

        return $"Fin proceso ETL: {nombreProceso}. Duración: {FormatearDuracion(duracion)}.";
    }

    /// <summary>
    /// Crea un mensaje estándar de inicio de paso.
    /// </summary>
    /// <param name="nombrePaso">Nombre del paso del proceso.</param>
    /// <returns>Mensaje de inicio de paso.</returns>
    public string InicioPaso(string nombrePaso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombrePaso);

        return $"Inicio paso: {nombrePaso}.";
    }

    /// <summary>
    /// Crea un mensaje estándar de paso completado correctamente.
    /// </summary>
    /// <param name="nombrePaso">Nombre del paso del proceso.</param>
    /// <returns>Mensaje de paso correcto.</returns>
    public string PasoCorrecto(string nombrePaso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombrePaso);

        return $"Paso completado correctamente: {nombrePaso}.";
    }

    /// <summary>
    /// Crea un mensaje estándar de error de proceso o paso.
    /// </summary>
    /// <param name="contexto">Proceso o paso donde ocurrió el error.</param>
    /// <param name="mensajeError">Detalle del error.</param>
    /// <returns>Mensaje de error.</returns>
    public string Error(string contexto, string mensajeError)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contexto);
        ArgumentException.ThrowIfNullOrWhiteSpace(mensajeError);

        return $"Error en {contexto}: {mensajeError}";
    }

    private static string FormatearDuracion(TimeSpan duracion)
    {
        // Se usa un formato fijo para facilitar búsquedas y comparación en logs.
        return duracion.ToString(@"hh\:mm\:ss");
    }
}
