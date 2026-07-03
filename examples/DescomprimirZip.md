# Descomprimir ZIP

```csharp
Zip zip = servicios.GetRequiredService<Zip>();

ResultadoOperacion<IReadOnlyList<string>> resultado = zip.DescomprimirArchivo(
    rutaZip,
    carpetaExtraccion);

if (!resultado.Exitoso)
{
    logger.LogError("Error al descomprimir archivo: {Mensaje}", resultado.Mensaje);
    return 1;
}
```
