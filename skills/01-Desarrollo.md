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

---

# Organización

La lógica debe distribuirse mediante clases y servicios.

No crear clases enormes.

No crear métodos enormes.

Preferir muchos métodos pequeños.

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

---

# Arquitectura

No modificar la estructura del template.

Toda lógica específica pertenece únicamente al proyecto del ETL.

Toda lógica genérica pertenece a ETL.Common.