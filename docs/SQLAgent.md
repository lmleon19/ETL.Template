# SQL Server Agent

## Objetivo

Definir el estándar de ejecución de ETL desde SQL Server Agent.

## Tipo de Job Step

Usar un Job Step de tipo:

```text
Operating system (CmdExec)
```

El comando debe ejecutar el binario publicado del ETL.

## Códigos de salida

Todo ETL debe retornar:

- `0`: ejecución correcta.
- Distinto de `0`: ejecución fallida.

SQL Server Agent debe usar el código de salida para determinar el resultado del Job Step.

## Configuración

Cada ETL debe mantener su configuración en `appsettings.json` y, cuando corresponda, en archivos de configuración por ambiente.

No guardar secretos en el repositorio.

## Cuenta de ejecución

La cuenta usada por SQL Server Agent debe tener permisos mínimos necesarios:

- Lectura/escritura en carpetas de trabajo.
- Acceso a archivos origen cuando aplique.
- Permisos SQL requeridos para tablas Stage y Stored Procedures.

## Logs

Los logs deben permitir identificar:

- Proceso ejecutado.
- Fecha y hora.
- Paso en ejecución.
- Error ocurrido.
- Duración cuando corresponda.

## Publicación

Cada ETL se publica como aplicación independiente de consola .NET.

El estándar de carpetas de publicación debe definirse por ambiente antes de pasar a producción.
