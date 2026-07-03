# Ejecución desde SQL Server Agent

Documento inicial para preparar la futura ejecución de ETL desde SQL Server Agent.

## Enfoque previsto

Cada ETL se publicará como aplicación de consola .NET y se ejecutará desde un Job Step de tipo **Operating system (CmdExec)**.

## Pendiente

- Definir estándar de publicación.
- Definir cuenta de servicio.
- Definir ubicación de binarios y logs.
- Definir manejo de códigos de salida.
