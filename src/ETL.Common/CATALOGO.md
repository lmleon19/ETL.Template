# Catálogo ETL.Common

| Utilidad | Estado | Responsabilidad |
| --- | --- | --- |
| `ResultadoOperacion` | Funcional | Representar resultados exitosos o fallidos, conservando excepción original cuando corresponde. |
| `DescargaHttp` | Funcional | Descargar archivos HTTP/HTTPS a una ruta local. |
| `Zip` | Funcional | Descomprimir archivos ZIP con validación de rutas de salida. |
| `DetectorEncoding` | Funcional | Detectar encoding por BOM y usar fallback configurable cuando no existe BOM. |
| `Csv` | Funcional | Leer archivos CSV con encabezado, delimitador configurable, calificador de texto, campos multilinea y tolerancia parametrizable para registros invalidos. |
| `HashArchivo` | Funcional | Calcular hash SHA-256 para trazabilidad e identificación de archivos procesados. |
| `FechaProceso` | Funcional | Obtener y formatear fecha de proceso con formatos institucionales simples. |
| `RutaTrabajo` | Funcional | Construir rutas de archivos y carpetas de trabajo por fecha. |
| `NormalizadorTexto` | Funcional | Normalizar espacios, acentos y nombres de columna para comparaciones genéricas. |
| `NormalizadorRutChile` | Funcional | Separar estructura de RUT chileno y validar RUT completo con dígito verificador. |
| `MensajesProceso` | Funcional | Construir mensajes estándar de inicio, fin, pasos correctos y errores. |
| `LoggerArchivo` | Funcional | Escribir logs de `ILogger` en un archivo historico acumulativo y otro del ultimo proceso. |
| `ValidadorArchivo` | Funcional | Validar existencia, contenido, extensión y tamaño máximo de archivos. |
| `ValidadorCsv` | Funcional | Validar estructura basica, columnas obligatorias y porcentaje maximo parametrizable de registros invalidos en archivos CSV. |
| `CargadorSqlBulkCopy` | Funcional | Cargar registros a tablas Stage mediante `SqlBulkCopy`. |
| `ProcedimientoAlmacenado` | Funcional | Ejecutar stored procedures sin parámetros. |
| `SistemaArchivos` | Funcional | Centralizar operaciones de archivos y carpetas. |
