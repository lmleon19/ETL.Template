# Flujo ETL

## Flujo estándar

Todo ETL debe seguir un flujo claro y repetible:

1. Registrar inicio.
2. Leer configuración.
3. Preparar carpetas de trabajo.
4. Obtener archivo origen.
5. Validar existencia, extensión y tamaño.
6. Descomprimir si corresponde.
7. Detectar encoding.
8. Validar estructura del archivo.
9. Leer registros.
10. Cargar tabla Stage.
11. Ejecutar Stored Procedure final.
12. Registrar fin.

## Manejo de errores

Cada paso debe validar su resultado antes de continuar.

Si un paso falla:

- Registrar el error.
- No ejecutar los pasos posteriores.
- Finalizar con código de salida distinto de cero.

## Program.cs esperado

`Program.cs` debe mostrar el proceso completo:

```csharp
RegistrarInicioProceso();

PrepararCarpetasTrabajo();

ResultadoOperacion descarga = await DescargarArchivoAsync();
if (!descarga.Exitoso)
{
    RegistrarError(descarga);
    return 1;
}

ResultadoOperacion validacion = await ValidarArchivoAsync();
if (!validacion.Exitoso)
{
    RegistrarError(validacion);
    return 1;
}

await CargarTablaStageAsync();
await EjecutarProcedimientoFinalAsync();

RegistrarFinProceso();
return 0;
```

El ejemplo es orientativo. Cada proceso puede tener pasos propios, pero debe conservar la misma forma general.
