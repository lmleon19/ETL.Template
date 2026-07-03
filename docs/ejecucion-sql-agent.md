# Ejecución desde SQL Server Agent

Este documento resume la ejecución operativa. El estándar completo está en `SQLAgent.md`.

## Enfoque

Cada ETL se publica como una aplicación de consola .NET independiente.

La ejecución desde SQL Server Agent debe realizarse con un Job Step de tipo:

```text
Operating system (CmdExec)
```

## Código de salida

- `0`: ejecución correcta.
- Distinto de `0`: ejecución fallida.

El proceso debe registrar el error antes de finalizar con código distinto de cero.

## Configuración mínima

Cada ETL debe definir:

- Ruta del ejecutable publicado.
- Carpeta de trabajo.
- Archivo `appsettings.json` correspondiente al ambiente.
- Cuenta de ejecución del Job.
- Permisos sobre carpetas, archivos y base de datos.
- Política de retención de logs.
