# ETL.Common

## Objetivo

`ETL.Common` centraliza utilidades genericas reutilizables por todos los ETL institucionales.

No contiene logica de negocio.

## Utilidades disponibles

| Utilidad | Responsabilidad |
| --- | --- |
| `ResultadoOperacion` | Representar resultados exitosos o fallidos. |
| `DescargaHttp` | Descargar archivos HTTP/HTTPS. |
| `SistemaArchivos` | Operaciones comunes de archivos y carpetas. |
| `Zip` | Descomprimir ZIP con proteccion de rutas de salida. |
| `DetectorEncoding` | Detectar encoding por BOM con fallback configurable. |
| `Csv` | Leer CSV con encabezado, delimitador, campos multilinea y tolerancia parametrizable para registros invalidos. |
| `ValidadorCsv` | Validar estructura, columnas obligatorias y porcentaje maximo parametrizable de registros invalidos en CSV. |
| `ValidadorArchivo` | Validar existencia, contenido, extension y tamano. |
| `CargadorSqlBulkCopy` | Cargar registros a tablas Stage. |
| `ProcedimientoAlmacenado` | Ejecutar Stored Procedures sin parametros. |
| `HashArchivo` | Calcular SHA-256 de archivos. |
| `FechaProceso` | Obtener y formatear fecha de proceso. |
| `RutaTrabajo` | Construir rutas estandar. |
| `NormalizadorTexto` | Normalizar espacios, acentos y nombres de columna. |
| `MensajesProceso` | Construir mensajes operativos estandar. |
| `LoggerArchivo` | Escribir logs en archivo historico y archivo del ultimo proceso. |

## Logging a archivos

`LoggerArchivo` permite agregar un proveedor de `ILogger` que escribe cada mensaje en dos archivos TXT:

- Historico: acumula todas las ejecuciones.
- Ultimo proceso: se limpia al iniciar el ETL y conserva solo la ejecucion actual.

Ejemplo de uso en `Program.cs`:

```csharp
OpcionesLoggerArchivo opcionesLoggerArchivo = builder.Configuration
    .GetSection("Logs")
    .Get<OpcionesLoggerArchivo>() ?? new OpcionesLoggerArchivo();

builder.Logging.AddLoggerArchivo(opcionesLoggerArchivo);
```

Ejemplo de configuracion:

```json
"Logs": {
  "Carpeta": "logs",
  "Historico": "historico.txt",
  "UltimoProceso": "ultimo-proceso.txt"
}
```

## Responsabilidad

`ETL.Common` entrega herramientas genericas, pero no decide reglas de negocio.

Ejemplos de responsabilidades de `ETL.Common`:

- leer CSV y reportar filas invalidas
- calcular porcentajes
- validar estructura basica
- escribir logs

Ejemplos de responsabilidades del ETL especifico:

- definir el porcentaje maximo aceptado de filas invalidas
- decidir si una fila se omite o detiene el proceso
- decidir que campos calculados quedan en `null`
- definir nombres de tablas, carpetas, URLs y stored procedures

## Restriccion

Durante el desarrollo de un ETL especifico no se modifica `ETL.Common`.

Si una funcionalidad comun necesita evolucionar, debe tratarse como cambio del template y revisarse como decision de arquitectura.

## Prohibido

No agregar logica especifica como:

- `DescargarChileCompra`.
- `ProcesarLobby`.
- `ActualizarSAIGlobal`.

Toda logica de negocio pertenece al proyecto especifico del ETL.
