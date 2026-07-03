# Arquitectura

## Objetivo

Definir una arquitectura estándar para procesos ETL institucionales en .NET 10.

Cada ETL debe ser una aplicación de consola independiente, mantenible y preparada para ejecución desde SQL Server Agent.

## Principios

- Simplicidad.
- Legibilidad.
- Mantenibilidad.
- Consistencia entre procesos.
- Separación entre lógica común y lógica específica.

## Componentes

### Proyecto específico

Cada ETL tiene su propio proyecto:

- `ETL.ChileCompra`
- `ETL.Lobby`
- `ETL.SAIGlobal`
- `ETL.NombreProceso`

Este proyecto contiene únicamente lógica propia del proceso.

La estructura interna del proyecto específico debe incluir una carpeta `Model` cuando existan clases que representen estructuras de datos propias del ETL.

`Model` contiene, por ejemplo:

- registros de tablas Stage
- estructuras de tablas de base de datos
- modelos de archivos del proceso
- metadatos de mapeo de columnas

No se debe usar una carpeta llamada `Modelos`.

Cuando una clase represente una tabla Stage o una estructura de base de datos, esa clase debe actuar como fuente maestra para nombres de columna, tipos SQL y origen de los valores cuando corresponda.

### ETL.Common

`ETL.Common` contiene utilidades reutilizables y genéricas. No contiene reglas de negocio ni código asociado a un dominio específico.

### Tests

Cada repositorio debe mantener pruebas unitarias para la lógica propia y para cualquier comportamiento relevante del proceso.

### Docs

Cada ETL debe documentar objetivo, origen, destino, flujo, configuración, SQL utilizado, ejecución y manejo de errores.

## Ejecución

Los ETL se publican como aplicaciones de consola y se ejecutan mediante SQL Server Agent con un Job Step de tipo CmdExec.

## Repositorios

Cada ETL debe vivir en su propio repositorio de Azure DevOps Server. No se usará monorepo inicialmente.

Cada repositorio contiene su propia copia de `ETL.Common` proveniente de este template.
