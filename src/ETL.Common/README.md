# ETL.Common

Librería local destinada a contener utilidades genéricas para ETL institucionales.

## Estado actual

La librería contiene una primera versión funcional de utilidades genéricas para procesos ETL.
Estas utilidades cubren operaciones comunes de archivos, ZIP, CSV, encoding, descarga HTTP y ejecución SQL.

## Principios

- No incluir reglas de negocio específicas.
- No conocer dominios como ChileCompra, Lobby o SAI.
- Mantener utilidades pequeñas, claras y reutilizables.
- Evitar sobrearquitectura.

## Utilidades disponibles

- `ResultadoOperacion`: resultado estándar para operaciones controladas.
- `SistemaArchivos`: creación, limpieza, búsqueda y validación de archivos/carpetas.
- `DescargaHttp`: descarga de archivos HTTP/HTTPS.
- `Zip`: extracción segura de archivos ZIP.
- `DetectorEncoding`: detección por BOM con fallback configurable.
- `Csv`: lectura de CSV con encabezado, delimitador configurable y campos multilinea delimitados por calificador de texto.
- `HashArchivo`: cálculo de hash SHA-256 para trazabilidad de archivos.
- `FechaProceso`: obtención y formateo estándar de fecha de proceso.
- `RutaTrabajo`: construcción de rutas y carpetas de trabajo.
- `NormalizadorTexto`: normalización genérica de espacios, acentos y nombres de columna.
- `MensajesProceso`: generación de mensajes estándar de inicio, fin, pasos y errores.
- `ValidadorArchivo`: validaciones genéricas de existencia, contenido, extensión y tamaño.
- `ValidadorCsv`: validación de estructura básica y columnas obligatorias, incluyendo registros multilinea.
- `CargadorSqlBulkCopy`: carga masiva a tablas Stage.
- `ProcedimientoAlmacenado`: ejecución de stored procedures.

## Restricción

Esta librería no debe modificarse durante el desarrollo normal de un ETL específico.
Si una funcionalidad no existe, debe implementarse primero en el proyecto del ETL y luego evaluarse como evolución arquitectónica de `ETL.Common`.
