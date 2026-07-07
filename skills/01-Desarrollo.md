# Desarrollo ETL

## Objetivo

Desarrollar ETL en .NET 10 siguiendo el estándar institucional definido por el proyecto ETL.Template.

La arquitectura del proyecto ya existe y no debe ser modificada.

---

# Principios

## Modo plan antes de ejecutar

Antes de crear o modificar un ETL, se debe trabajar primero en modo plan.

En modo plan se permite:

- leer archivos del repositorio;
- revisar scripts, modelos, configuracion y documentacion existente;
- identificar informacion faltante;
- proponer cambios concretos;
- explicar archivos que se modificarian;
- indicar pruebas o comandos de validacion recomendados.

En modo plan no se debe:

- crear archivos;
- modificar codigo;
- modificar scripts SQL;
- modificar configuracion;
- modificar documentacion;
- ejecutar acciones que cambien datos o estructura.

Solo se debe pasar de plan a ejecucion cuando el usuario lo indique explicitamente con una instruccion como `ejecuta`, `implementa`, `aplica`, `haz los cambios` o equivalente.

Si el usuario entrega una solicitud ambigua, primero se debe responder con el plan y pedir confirmacion antes de editar.


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

Todo ETL debe configurar el logger a archivos provisto por `ETL.Common` antes de construir el host.

Ejemplo:

```csharp
OpcionesLoggerArchivo opcionesLoggerArchivo = builder.Configuration
    .GetSection("NombreProceso:Logs")
    .Get<OpcionesLoggerArchivo>() ?? new OpcionesLoggerArchivo();

builder.Logging.AddLoggerArchivo(opcionesLoggerArchivo);
```

El `appsettings.json` del ETL debe incluir una seccion `Logs` dentro de la seccion propia del proyecto ETL, con carpeta, archivo historico y archivo del ultimo proceso.

Ejemplo:

```json
"NombreProceso": {
  "Logs": {
    "Carpeta": "logs",
    "Historico": "NombreProceso-historico.txt",
    "UltimoProceso": "NombreProceso-ultimo.txt"
  }
}
```

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
    await DescargarOCAsync();
}
```

Estos `if` deben usarse solo para controlar la ejecucion de pasos completos, no para ocultar logica de negocio.

Antes de cerrar un cambio, los `if (true)` / `if (false)` deben quedar coherentes con el flujo esperado para la ejecucion del ETL. Si un paso queda desactivado intencionalmente, debe existir una razon clara en el codigo o en la documentacion del flujo.

Cada bloque del flujo debe incluir un comentario corto que explique que hace el paso.

Cuando un proceso tenga etapas naturalmente separables, deben quedar como pasos separados en el flujo principal.

Ejemplo:

```csharp
if (true)
{
    // Carga Stage de licitaciones.
    await CargarStageLicitacionesAsync();
}

if (true)
{
    // Carga Stage de OC.
    await CargarStageOCAsync();
}

if (true)
{
    // Traspasa licitaciones a tablas finales.
    await EjecutarTraspasoFinalLicitacionesAsync();
}

if (true)
{
    // Traspasa OC a tablas finales.
    await EjecutarTraspasoFinalOCAsync();
}
```

Evitar agrupar en un solo paso operaciones que deban poder diagnosticarse o reejecutarse por separado.

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

Cuando un paso procese una coleccion de periodos, debe registrar inicio y fin de cada periodo.
El log debe indicar al menos el periodo y el paso ejecutado, para poder seguir el avance y detectar en que periodo fallo o quedo detenido el proceso.

Ejemplo:

```csharp
foreach (PeriodoProceso periodo in periodos)
{
    logger.LogInformation("Inicio periodo {Anio}-{Mes:00} para descarga de OC.", periodo.Anio, periodo.Mes);

    await DescargarOCPeriodoAsync(periodo);

    logger.LogInformation("Fin periodo {Anio}-{Mes:00} para descarga de OC.", periodo.Anio, periodo.Mes);
}
```

---

# Organización

## Nombre del proyecto

Todo proyecto ETL especifico debe usar un nombre claro y consistente con el formato:

```text
ETL.<Dominio>.<Accion><Objeto>
```

Ejemplos:

- `ETL.ChileCompra.DescargaData`
- `ETL.ChileCompra.CargaInfoTransparencia`
- `ETL.InfoTransparencia.CargaPortal`

El nombre elegido debe mantenerse alineado en:

- carpeta del proyecto dentro de `src`;
- archivo `.csproj`;
- namespace base;
- seccion propia de configuracion en `appsettings.json`;
- nombres de logs historico y ultimo proceso;
- `README.md` y `docs/Flujo.md` del ETL.

El dominio debe representar la fuente, destino o area funcional principal del proceso. La accion y objeto deben ser autoexplicativos y estar en espanol cuando corresponda al dominio del repositorio.


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

# Configuracion

Todo valor operativo debe venir desde `appsettings.json` o desde configuracion equivalente.

Ejemplos:

- cantidad de meses a procesar
- rutas y carpetas
- URLs
- nombres de tablas
- nombres de stored procedures
- tolerancias de errores
- delimitadores y encoding

El codigo puede definir valores por defecto razonables para evitar `null`, pero no debe esconder reglas operativas fijas.

Si un valor puede cambiar entre ambientes o ejecuciones, no debe quedar hardcodeado en la logica.

---

# Contratos de datos

No inventar contratos de datos ni estructuras tecnicas.

Antes de definir una tabla, un modelo Stage, un mapeo, una columna, un tipo de dato, una base de datos, un schema, una URL o una regla de negocio, se debe usar una fuente real entregada por el usuario o existente en el repositorio.

Si esa informacion no existe o no es suficiente, se debe preguntar al usuario antes de continuar.

Ejemplos de cosas que requieren confirmacion o fuente real:

- estructura de tablas
- columnas esperadas de archivos CSV, Excel, JSON o APIs
- tipos de datos SQL
- nombres de tablas finales o Stage
- nombre de base de datos
- nombre de schema
- reglas de normalizacion
- reglas de conversion
- reglas para aceptar, rechazar o corregir datos

Es valido inferir detalles menores solo cuando no cambian el contrato de datos ni el comportamiento del proceso.

---

# Siglas de dominio

Si una sigla es parte del dominio y ya se usa como estandar, debe mantenerse de forma consistente.

Ejemplo:

- Usar `OC` en nombres de configuracion, metodos y procedimientos si el proceso adopto esa sigla.
- No mezclar `OC`, `OrdenesCompra`, `OrdenCompra` y `Ordenes` para referirse a lo mismo.

No abreviar por comodidad. Solo usar siglas cuando sean claras para el dominio o para la organizacion.

---

# Validacion antes de transformacion

Antes de calcular campos derivados, validar que el dato fuente sea semanticamente correcto.

Ejemplo:

- No basta con limpiar un RUT.
- Primero debe validarse que sea un RUT valido.
- Solo si es valido se debe calcular numero y digito verificador.
- Si no es valido, el campo calculado debe quedar `null` o debe rechazarse segun la regla del ETL.

Evitar transformar texto invalido en datos aparentemente validos.

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

El log debe persistirse usando `LoggerArchivo` de `ETL.Common`, manteniendo:

- Un archivo historico acumulativo.
- Un archivo del ultimo proceso, recreado al inicio de cada ejecucion.

---

# SQL

Preferir siempre:

Archivo

↓

Tabla Stage

↓

Stored Procedure

Evitar lógica SQL distribuida por todo el código.

Cuando un ETL necesite objetos de base de datos, como tablas Stage, tablas finales o stored procedures, el proyecto debe entregar los scripts SQL dentro de una carpeta `sql` del proyecto específico.

Ejemplo:

```text
src/ETL.NombreProceso/sql
```

El ETL no debe crear automaticamente objetos permanentes de base de datos armando SQL de estructura desde C#.

La aplicación debe indicar al usuario u operador que los scripts SQL deben ejecutarse previamente en la base de datos correspondiente.

El codigo del ETL puede limpiar o cargar tablas existentes cuando sea parte del proceso.

El codigo tambien puede ejecutar stored procedures existentes y configurados para preparar estructura necesaria del proceso, por ejemplo crear tablas finales anuales faltantes.

La regla es que la definicion de estructura debe vivir en scripts SQL versionados; C# solo llama procedimientos existentes y configurados.

Los stored procedures tecnicos de ETL deben tener nombres consistentes.

Si la base de datos ya identifica el dominio, no repetir el dominio innecesariamente en el nombre del procedimiento.

Ejemplo recomendado cuando las tablas viven en `dbo`:

```sql
dbo.ETL_TraspasarLicitaciones
dbo.ETL_TraspasarOC
```

Usar un schema distinto, como `etl`, solo si el estandar de la base de datos ya contempla separar objetos por schema.

Si las tablas del proceso viven en `dbo`, mantener los procedimientos ETL en `dbo` salvo que exista una decision explicita de arquitectura.

Los scripts de stored procedures deben usar `CREATE OR ALTER PROCEDURE` cuando corresponda.

Los scripts de tablas deben evitar operaciones destructivas. No deben eliminar tablas ni datos salvo que exista una instrucción explícita.

El traspaso desde Stage hacia tablas finales debe ser reejecutable cuando sea posible. Si se reprocesa un archivo o período, el procedimiento debe reemplazar ese conjunto antes de volver a insertarlo.

Si se usa SQL dinámico, debe existir una razón clara, por ejemplo tablas anuales dinámicas. En ese caso se deben proteger nombres de objetos con `QUOTENAME` y ejecutar con `sp_executesql` cuando corresponda.

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

La carpeta `docs` de la raiz debe mantenerse acotada a la documentacion de `ETL.Common`.

Cada ETL creado debe tener su propio documento `docs/Flujo.md` dentro del proyecto especifico, con el flujo real del codigo, punto a punto.

La carpeta `docs` del proyecto ETL y el archivo `Flujo.md` deben crearse al momento de crear el ETL, no al final.

Cada vez que cambie `Program.cs`, el orden de ejecucion o la responsabilidad de un paso, debe actualizarse `docs/Flujo.md` en la misma modificacion.

Los ejemplos pequeños reutilizables viven en examples.

---

# Encoding

Los archivos nuevos del template, del ETL y de documentacion deben crearse en UTF-8.

No usar encodings antiguos para codigo o documentacion.

Solo se debe usar un encoding distinto cuando el archivo fuente externo lo exija, por ejemplo un CSV de origen que venga en otro encoding.

---

# Artefactos generados

No deben quedar versionados artefactos generados por compilacion, ejecucion o descarga.

Ejemplos:

- `bin`
- `obj`
- logs
- archivos ZIP descargados
- CSV grandes de trabajo
- carpetas temporales
- datos de prueba pesados

Los archivos de prueba pequenos y controlados pueden mantenerse cuando sean necesarios para validar una regla concreta.
