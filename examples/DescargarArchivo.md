# Descargar Archivo

```csharp
DescargaHttp descargaHttp = servicios.GetRequiredService<DescargaHttp>();

ResultadoOperacion<string> resultado = await descargaHttp.DescargarArchivoAsync(
    opciones.UrlDescarga,
    rutaDestino,
    cancellationToken);

if (!resultado.Exitoso)
{
    logger.LogError("Error al descargar archivo: {Mensaje}", resultado.Mensaje);
    return 1;
}
```
