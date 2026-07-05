using Microsoft.Extensions.Logging;

namespace ETL.Common.Servicios;

public sealed class OpcionesLoggerArchivo
{
    public string Carpeta { get; set; } = "logs";
    public string Historico { get; set; } = "historico.txt";
    public string UltimoProceso { get; set; } = "ultimo-proceso.txt";
}

public static class LoggerArchivo
{
    public static ILoggingBuilder AddLoggerArchivo(this ILoggingBuilder logging, OpcionesLoggerArchivo opciones)
    {
        ArgumentNullException.ThrowIfNull(logging);
        ArgumentNullException.ThrowIfNull(opciones);

        logging.AddProvider(new ProveedorLoggerArchivo(opciones));

        return logging;
    }
}

internal sealed class ProveedorLoggerArchivo : ILoggerProvider
{
    private readonly object bloqueo = new();
    private readonly string rutaHistorico;
    private readonly string rutaUltimoProceso;

    public ProveedorLoggerArchivo(OpcionesLoggerArchivo opciones)
    {
        ArgumentNullException.ThrowIfNull(opciones);

        string carpeta = ObtenerValorConfigurado(opciones.Carpeta, "logs");
        Directory.CreateDirectory(carpeta);

        rutaHistorico = Path.Combine(carpeta, ObtenerValorConfigurado(opciones.Historico, "historico.txt"));
        rutaUltimoProceso = Path.Combine(carpeta, ObtenerValorConfigurado(opciones.UltimoProceso, "ultimo-proceso.txt"));

        File.WriteAllText(rutaUltimoProceso, string.Empty);
    }

    public ILogger CreateLogger(string categoryName) =>
        new LoggerArchivoInterno(categoryName, rutaHistorico, rutaUltimoProceso, bloqueo);

    public void Dispose()
    {
    }

    private static string ObtenerValorConfigurado(string valorConfigurado, string valorPorDefecto) =>
        string.IsNullOrWhiteSpace(valorConfigurado)
            ? valorPorDefecto
            : valorConfigurado;
}

internal sealed class LoggerArchivoInterno : ILogger
{
    private readonly string categoria;
    private readonly string rutaHistorico;
    private readonly string rutaUltimoProceso;
    private readonly object bloqueo;

    public LoggerArchivoInterno(
        string categoria,
        string rutaHistorico,
        string rutaUltimoProceso,
        object bloqueo)
    {
        this.categoria = categoria;
        this.rutaHistorico = rutaHistorico;
        this.rutaUltimoProceso = rutaUltimoProceso;
        this.bloqueo = bloqueo;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        null;

    public bool IsEnabled(LogLevel logLevel) => logLevel is not LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        if (!IsEnabled(logLevel))
        {
            return;
        }

        string mensaje = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(mensaje) && exception is null)
        {
            return;
        }

        string linea = FormatearLinea(logLevel, eventId, mensaje, exception);

        lock (bloqueo)
        {
            File.AppendAllText(rutaHistorico, linea);
            File.AppendAllText(rutaUltimoProceso, linea);
        }
    }

    private string FormatearLinea(LogLevel logLevel, EventId eventId, string mensaje, Exception? exception)
    {
        string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string idEvento = eventId.Id == 0 ? string.Empty : $" Evento={eventId.Id}";
        string detalleExcepcion = exception is null
            ? string.Empty
            : $"{Environment.NewLine}{exception}";

        return $"{fecha} [{logLevel}] {categoria}{idEvento}: {mensaje}{detalleExcepcion}{Environment.NewLine}";
    }
}
