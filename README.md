# ETL.Template

Base institucional inicial para crear procesos ETL en .NET 10 como reemplazo progresivo de SSIS.

## Estado actual

Esta primera versión crea solo la estructura base: carpetas, proyectos, `Program.cs`, servicios vacíos con métodos iniciales y documentación mínima.

## Estructura

```text
ETL.Template/
├── src/
│   ├── ETL.Common/
│   └── ETL.NombreProceso/
├── tests/
│   └── ETL.NombreProceso.Tests/
├── docs/
├── skills/
├── README.md
└── ETL.Template.slnx
```

## Convenciones iniciales

- Cada ETL será un proyecto independiente.
- La lógica reutilizable debe vivir en `ETL.Common`.
- No se usa MVC, Minimal API ni Web API.
- Se usa Console App con HostBuilder, DI, `appsettings.json` e `ILogger`.
- Los servicios actuales son esqueletos y no contienen lógica final.
