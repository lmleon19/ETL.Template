# ETL.Template

Template institucional para crear procesos ETL en .NET 10 como reemplazo progresivo de SSIS.

El objetivo del repositorio no es implementar un ETL específico. El objetivo es definir una base común de arquitectura, programación, documentación y uso de `ETL.Common` para que todos los futuros ETL se vean y operen de forma consistente.

## Propósito

- Estandarizar el desarrollo de ETL on-premise.
- Mantener una arquitectura simple y legible.
- Facilitar mantenimiento durante años.
- Permitir que desarrolladores e IA generen código con el mismo estilo.
- Preparar cada ETL para ejecución desde SQL Server Agent mediante CmdExec.

## Tecnología

- .NET 10.
- Console App con HostBuilder.
- Dependency Injection.
- `ILogger`.
- `appsettings.json`.
- SQL Server.
- SQL Server Agent.
- Azure DevOps Server 2022.

No se usa MVC, Minimal API, Web API, SSIS, Azure Data Factory ni Microsoft Fabric.

## Estructura

```text
ETL.Template/
├── ETL.Template.slnx
├── src/
│   ├── ETL.Common/
│   └── ETL.NombreProceso/
├── tests/
│   └── ETL.NombreProceso.Tests/
├── docs/
├── skills/
├── examples/
└── README.md
```

## Proyectos

- `ETL.Common`: librería de utilidades reutilizables y genéricas.
- `ETL.NombreProceso`: proyecto base casi vacío para crear un ETL específico.
- `ETL.NombreProceso.Tests`: pruebas unitarias del template y de las utilidades comunes.

## Creación de un ETL específico

Para crear un nuevo ETL desde este template:

1. Copiar el template a un nuevo repositorio.
2. Renombrar `ETL.NombreProceso` con el nombre real del proceso, por ejemplo `ETL.ChileCompra`.
3. Leer y aplicar las reglas de `skills/`.
4. Definir el flujo principal en `Program.cs`.
5. Implementar la lógica específica solo dentro del proyecto del ETL.
6. Reutilizar `ETL.Common` sin modificarlo.
7. Actualizar la documentación del ETL.
8. Ejecutar las pruebas.

## Flujo estándar

Todo ETL debe seguir el flujo general:

1. Registrar inicio.
2. Preparar carpetas de trabajo.
3. Descargar u obtener archivo origen.
4. Validar archivo.
5. Descomprimir cuando corresponda.
6. Detectar encoding.
7. Validar estructura del archivo.
8. Leer datos.
9. Cargar tabla Stage.
10. Ejecutar Stored Procedure final.
11. Registrar fin.

Cuando un paso falla, el proceso debe registrar el error y finalizar con código de salida distinto de cero.

## ETL.Common

`ETL.Common` contiene funcionalidades reutilizables:

- Descarga HTTP.
- Manejo de archivos y carpetas.
- Descompresión ZIP segura.
- Lectura y validación CSV.
- Detección de encoding por BOM.
- Hash SHA-256.
- Rutas de trabajo.
- Normalización de texto.
- Mensajes estándar.
- `SqlBulkCopy`.
- Ejecución de Stored Procedures.
- Resultados controlados mediante `ResultadoOperacion`.

`ETL.Common` no contiene lógica de negocio. No debe incluir nombres ni reglas específicas de ChileCompra, Lobby, SAI Global u otros dominios.

## Regla de estabilidad

Durante el desarrollo normal de un ETL específico no se modifica `ETL.Common`.

Si un ETL necesita una función que no existe, se implementa dentro del proyecto específico. La evolución de `ETL.Common` es una decisión de arquitectura del template, no una decisión local de un ETL.

## Documentación

La documentación institucional vive en `docs/`:

- `Arquitectura.md`
- `Convenciones.md`
- `FlujoETL.md`
- `SQLAgent.md`
- `ETL.Common.md`
- `ejecucion-sql-agent.md`

Las reglas operativas para IA viven en `skills/`.

## Ejemplos

La carpeta `examples/` contiene ejemplos pequeños de uso de utilidades comunes. No son ETL completos.

## Validación

Ejecutar las pruebas con:

```bash
dotnet test
```
