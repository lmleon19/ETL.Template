using System.Reflection;
using ETL.ChileCompra.Carga.Model;

namespace ETL.ChileCompra.Carga.Servicios;

public sealed class MetadataStage
{
    public IReadOnlyList<(PropertyInfo Propiedad, ColumnaStageAttribute Columna)> ObtenerColumnas<TRegistroStage>() =>
        typeof(TRegistroStage)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(p => (Propiedad: p, Columna: p.GetCustomAttribute<ColumnaStageAttribute>()))
            .Where(x => x.Columna is not null)
            .OrderBy(x => x.Columna!.Posicion)
            .Select(x => (x.Propiedad, x.Columna!))
            .ToArray();

    public IReadOnlyList<string> ObtenerColumnasCsv<TRegistroStage>() =>
        ObtenerColumnas<TRegistroStage>()
            .Where(c => c.Columna.Origen == OrigenColumnaStage.Csv)
            .Select(c => c.Columna.NombreCsv)
            .ToArray();
}

