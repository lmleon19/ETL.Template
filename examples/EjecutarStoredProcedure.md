# Ejecutar Stored Procedure

```csharp
ProcedimientoAlmacenado procedimientoAlmacenado = servicios.GetRequiredService<ProcedimientoAlmacenado>();

ResultadoOperacion resultado = await procedimientoAlmacenado.EjecutarAsync(
    cadenaConexion,
    opciones.ProcedimientoFinal,
    cancellationToken);

if (!resultado.Exitoso)
{
    logger.LogError("Error al ejecutar procedimiento final: {Mensaje}", resultado.Mensaje);
    return 1;
}
```
