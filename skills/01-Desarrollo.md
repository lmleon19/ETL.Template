# Desarrollo ETL

## Objetivo

Desarrollar ETL en .NET 10 siguiendo el estándar institucional definido por el proyecto ETL.Template.

La arquitectura del proyecto ya existe y no debe ser modificada.

---

# Principios

- El código debe ser simple.
- Debe privilegiarse la legibilidad antes que escribir menos líneas.
- Todo desarrollador debe poder comprender el flujo del ETL en pocos minutos.
- Cada método debe tener una única responsabilidad.
- No duplicar código innecesariamente.
- Programar pensando en el mantenimiento futuro.

---

# Program.cs

Program.cs representa únicamente el flujo del proceso.

No debe contener lógica de negocio.

Debe ser posible leer el proceso de arriba hacia abajo.

Program.cs no debe registrar manualmente todos los servicios con múltiples llamadas `AddSingleton`, `AddScoped` o equivalentes.

El registro de dependencias debe vivir en una clase de configuración o extensión del proyecto específico.

Ejemplo:

```csharp
builder.Services.AddServiciosNombreProceso(builder.Configuration);
```

La clase de configuración puede contener los registros de `ETL.Common` y de servicios propios del ETL.

Cada paso debe validar si puede continuar.

Durante el desarrollo, los pasos del flujo deben quedar activables o desactivables con `if (true)`.
Para omitir temporalmente un paso, cambiar solo ese `true` por `false`.

Ejemplo:

```csharp
if (true)
{
    // Calcula los meses cerrados que se deben descargar.
    CalcularPeriodosProceso();
}

if (true)
{
    // Descarga los archivos ZIP de licitaciones.
    await DescargarLicitacionesAsync();
}

if (true)
{
    // Descarga los archivos ZIP de ordenes de compra.
    await DescargarOrdenesCompraAsync();
}
```

Estos `if` deben usarse solo para controlar la ejecucion de pasos completos, no para ocultar logica de negocio.

Cada bloque del flujo debe incluir un comentario corto que explique que hace el paso.

Si un paso falla:

- registrar el error
- no ejecutar los pasos siguientes
- retornar un código de salida distinto de cero

Ejemplo:

RegistrarInicioProceso()

if (...)
    DescargarArchivoAsync()

if (...)
    DescomprimirArchivoAsync()

if (...)
    ValidarArchivoAsync()

if (...)
    LeerArchivoAsync()

if (...)
    CargarStageAsync()

if (...)
    EjecutarProcedimientoFinalAsync()

RegistrarFinProceso()

return 0

---

# Organización

La lógica debe distribuirse mediante clases y servicios.

No crear clases enormes.

No crear métodos enormes.

Preferir muchos métodos pequeños.

## Carpeta Model

Todo proyecto ETL específico debe usar una carpeta llamada `Model`.

No usar `Modelos`.

La carpeta `Model` contiene clases que representan estructuras de datos propias del proceso, especialmente:

- registros de tablas Stage
- estructuras de tablas de base de datos
- modelos de archivos del proceso
- atributos o metadatos necesarios para mapear columnas

Cuando una clase represente una tabla Stage o una estructura de base de datos, debe ser la fuente maestra de:

- nombre de columna en base de datos
- tipo de dato SQL cuando corresponda
- origen del valor cuando sea necesario distinguir CSV, metadato o cálculo

Estas clases pertenecen al proyecto específico del ETL. No deben agregarse a `ETL.Common`.

---

# Convenciones

Utilizar:

- Clases en PascalCase.
- Métodos en PascalCase.
- Variables locales en camelCase.
- Parámetros en camelCase.
- Métodos Async finalizados en Async.

Ejemplo:

DescargarArchivoAsync()

LeerCsvAsync()

CargarTablaStageAsync()

---

# Nombres

Todos los nombres deben ser autoexplicativos.

Correcto:

ValidarCantidadColumnas()

ObtenerRutaArchivoTemporal()

Incorrecto:

Procesar()

Datos()

Tmp()

Obj()

---

# LINQ

Utilizar LINQ cuando simplifique el código.

Preferir:

r => r.Propiedad

Ejemplo:

var registrosValidos = registros
    .Where(r => r.Codigo != null)
    .ToList();

---

# ILogger

Registrar los pasos importantes del proceso.

Registrar:

- Inicio
- Fin
- Errores
- Advertencias
- Duración de procesos importantes

No registrar información innecesaria.

---

# SQL

Preferir siempre:

Archivo

↓

Tabla Stage

↓

Stored Procedure

Evitar lógica SQL distribuida por todo el código.

---

# Excepciones

Toda excepción debe contener suficiente información para identificar:

- proceso
- archivo
- paso
- mensaje
- excepción original

Nunca ocultar excepciones.

Las excepciones no controladas del flujo principal deben registrarse antes de finalizar el proceso.

---

# Arquitectura

No modificar la estructura del template.

Toda lógica específica pertenece únicamente al proyecto del ETL.

Toda lógica genérica pertenece a ETL.Common.

La documentación institucional vive en docs.

Los ejemplos pequeños reutilizables viven en examples.
