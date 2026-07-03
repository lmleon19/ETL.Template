# Detectar Encoding

```csharp
DetectorEncoding detectorEncoding = servicios.GetRequiredService<DetectorEncoding>();

Encoding encoding = detectorEncoding.DetectarEncoding(rutaArchivo, Encoding.UTF8);
```

La detección usa BOM cuando existe. Si el archivo no tiene BOM, utiliza el encoding informado como fallback.
