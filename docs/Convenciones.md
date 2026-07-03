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

## Carpetas

Los proyectos ETL específicos deben usar una carpeta `Model` para clases que representen estructuras de datos del proceso.

No usar `Modelos`.

`Model` debe contener modelos propios del ETL, por ejemplo registros Stage, estructuras de tablas y metadatos de columnas.

Las clases de `Model` que representen tablas Stage o estructuras de base de datos deben ser la fuente maestra para el nombre real de columna y el tipo SQL cuando corresponda.

## Program.cs

`Program.cs` representa el flujo principal del ETL.

Debe poder leerse de arriba hacia abajo y permitir comprender el proceso completo sin revisar todas las clases.

No debe contener lógica de negocio extensa.

Durante el desarrollo, cada paso puede controlarse mediante `if (true)`.
Para omitir temporalmente un paso, cambiar solo ese `true` por `false`.

```csharp
if (true)
{
    // Calcula los meses cerrados que se deben procesar.
    CalcularPeriodosProceso();
}
```

Cada bloque del flujo debe incluir un comentario corto que explique su objetivo.

Estos `if` deben activar o desactivar pasos completos. No deben usarse para esconder lógica de negocio dentro de `Program.cs`.

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
