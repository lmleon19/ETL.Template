# Leer CSV

```csharp
Csv csv = servicios.GetRequiredService<Csv>();

IReadOnlyList<Dictionary<string, string>> registros = await csv.LeerCsvAsync(
    rutaCsv,
    encoding,
    opciones.Delimitador,
    cancellationToken: cancellationToken);
```

El CSV debe tener encabezado. Cada registro se retorna como diccionario usando el nombre de columna como clave.
