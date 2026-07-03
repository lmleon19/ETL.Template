# ETL.Common

## Objetivo

`ETL.Common` centraliza utilidades genéricas reutilizables por todos los ETL institucionales.

No contiene lógica de negocio.

## Utilidades disponibles

| Utilidad | Responsabilidad |
| --- | --- |
| `ResultadoOperacion` | Representar resultados exitosos o fallidos. |
| `DescargaHttp` | Descargar archivos HTTP/HTTPS. |
| `SistemaArchivos` | Operaciones comunes de archivos y carpetas. |
| `Zip` | Descomprimir ZIP con protección de rutas de salida. |
| `DetectorEncoding` | Detectar encoding por BOM con fallback configurable. |
| `Csv` | Leer CSV con encabezado, delimitador y campos multilinea. |
| `ValidadorCsv` | Validar estructura y columnas obligatorias de CSV. |
| `ValidadorArchivo` | Validar existencia, contenido, extensión y tamaño. |
| `CargadorSqlBulkCopy` | Cargar registros a tablas Stage. |
| `ProcedimientoAlmacenado` | Ejecutar Stored Procedures sin parámetros. |
| `HashArchivo` | Calcular SHA-256 de archivos. |
| `FechaProceso` | Obtener y formatear fecha de proceso. |
| `RutaTrabajo` | Construir rutas estándar. |
| `NormalizadorTexto` | Normalizar espacios, acentos y nombres de columna. |
| `MensajesProceso` | Construir mensajes operativos estándar. |

## Restricción

Durante el desarrollo de un ETL específico no se modifica `ETL.Common`.

Si una funcionalidad común necesita evolucionar, debe tratarse como cambio del template y revisarse como decisión de arquitectura.

## Prohibido

No agregar lógica específica como:

- `DescargarChileCompra`.
- `ProcesarLobby`.
- `ActualizarSAIGlobal`.

Toda lógica de negocio pertenece al proyecto específico del ETL.
