# SqlBulkCopy

```csharp
CargadorSqlBulkCopy cargadorSqlBulkCopy = servicios.GetRequiredService<CargadorSqlBulkCopy>();

ResultadoOperacion resultado = await cargadorSqlBulkCopy.CargarAsync(
    cadenaConexion,
    opciones.TablaStage,
    registros,
    cancellationToken);

if (!resultado.Exitoso)
{
    logger.LogError("Error al cargar tabla Stage: {Mensaje}", resultado.Mensaje);
    return 1;
}
```
