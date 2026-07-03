# Convenciones

## Lenguaje

Todos los nombres de clases, métodos, variables, parámetros, mensajes operativos y documentación deben estar en español.

## Nombres

- Clases: PascalCase.
- Métodos: PascalCase.
- Variables locales: camelCase.
- Parámetros: camelCase.
- Métodos async: terminar en `Async`.

Usar nombres autoexplicativos. Evitar nombres como `data`, `tmp`, `obj`, `hacer` o `procesar` cuando no expliquen la intención.

## Program.cs

`Program.cs` representa el flujo principal del ETL.

Debe poder leerse de arriba hacia abajo y permitir comprender el proceso completo sin revisar todas las clases.

No debe contener lógica de negocio extensa.

## Logging

Usar `ILogger`.

Registrar:

- Inicio.
- Fin.
- Advertencias importantes.
- Errores.
- Duración de pasos relevantes cuando corresponda.

No registrar información innecesaria.

## SQL

Preferir el flujo:

```text
Archivo
Validación
Tabla Stage
Stored Procedure final
```

Evitar distribuir lógica SQL compleja por múltiples clases de C#.

## LINQ

Usar LINQ cuando simplifique la lectura del código.

Preferir expresiones claras:

```csharp
var registrosValidos = registros
    .Where(r => r.Codigo != null)
    .ToList();
```

## Errores

Los errores deben conservar contexto suficiente:

- Proceso.
- Paso.
- Archivo involucrado.
- Mensaje claro.
- Excepción original cuando exista.
