using System.Globalization;

namespace ETL.Common.Servicios;

/// <summary>
/// Utilidad común para obtener y formatear la fecha de proceso de un ETL.
/// </summary>
public sealed class FechaProceso
{
    private static readonly string[] FormatosPermitidos =
    [
        "yyyy-MM-dd",
        "yyyyMMdd",
        "dd-MM-yyyy",
        "dd/MM/yyyy"
    ];

    /// <summary>
    /// Obtiene la fecha de proceso desde un texto o usa la fecha actual cuando no se informa valor.
    /// </summary>
    /// <param name="valor">Fecha informada como texto. Si viene vacía, se usa la fecha actual.</param>
    /// <returns>Fecha de proceso normalizada.</returns>
    public DateOnly ObtenerFechaProceso(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }

        if (DateOnly.TryParseExact(
            valor.Trim(),
            FormatosPermitidos,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly fechaProceso))
        {
            return fechaProceso;
        }

        throw new FormatException($"La fecha de proceso '{valor}' no tiene un formato válido.");
    }

    /// <summary>
    /// Formatea una fecha de proceso con el estándar compacto yyyyMMdd.
    /// </summary>
    /// <param name="fechaProceso">Fecha de proceso.</param>
    /// <returns>Fecha formateada como yyyyMMdd.</returns>
    public string FormatearFechaProceso(DateOnly fechaProceso) => fechaProceso.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
}
