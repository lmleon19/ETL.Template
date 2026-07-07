# ETL.ChileCompra.CargaInfoTransparencia

Proyecto ETL para cargar informacion de ChileCompra hacia InfoTransparencia.

## Estado

Implementacion inicial de licitaciones y OC.

Pendiente de definicion:

- conexion destino `InfoTransparencia`;
- confirmacion de estructura exacta de las tablas destino;
- confirmacion de estructura exacta de las tablas destino OC.

## Flujo Licitaciones

El proceso toma los ultimos meses cerrados configurados, sin incluir el mes actual.

Para cada periodo:

1. Lee datos desde la base `ChileCompra`.
2. Consulta la tabla anual de licitaciones segun el periodo.
3. Excluye codigos externos presentes en `dbo.Excluir_Licitaciones`.
4. Obtiene cabecera agregada de licitaciones.
5. Obtiene detalle de items adjudicados.
6. Elimina en destino el rango `Fecha_publicacion >= FechaDesde AND Fecha_publicacion < FechaHasta`.
7. Carga nuevamente cabecera y detalle en la base destino.

## Flujo OC

El proceso usa los mismos periodos cerrados que licitaciones.

Para cada periodo:

1. Lee datos desde la base `ChileCompra`.
2. Consulta la tabla anual de OC segun el periodo.
3. Excluye codigos presentes en `dbo.Excluir_OC`.
4. Obtiene cabecera agregada de OC.
5. Obtiene detalle de items de OC.
6. Elimina en destino el rango `Fecha_Envio >= FechaDesde AND Fecha_Envio < FechaHasta` para detalle.
7. Elimina en destino el rango `FechaEnvio >= FechaDesde AND FechaEnvio < FechaHasta` para cabecera.
8. Carga nuevamente cabecera y detalle en la base destino.

## Configuracion

La configuracion propia del proceso vive en:

```json
"ConnectionStrings": {
  "ChileCompra": "",
  "InfoTransparencia": ""
},
"ChileCompraCargaInfoTransparencia": {
  "MesesCerradosACargar": 4,
  "FormatoSufijoTablaAnual": "yy",
  "Tablas": {
    "PrefijoLicitacionesOrigen": "dbo.DatosAbiertos_Licitaciones_",
    "PrefijoOCOrigen": "dbo.DatosAbiertos_OC_",
    "LicitacionDestino": "dbo.EX_MP_Licitacion",
    "LicitacionDetalleDestino": "dbo.EX_MP_LicitacionDetalle",
    "OCDestino": "dbo.EX_MP_OrdenCompra",
    "OCDetalleDestino": "dbo.EX_MP_OrdenCompraDetalle"
  },
  "Logs": {
    "Carpeta": "logs",
    "Historico": "ChileCompra-CargaInfoTransparencia-historico.txt",
    "UltimoProceso": "ChileCompra-CargaInfoTransparencia-ultimo.txt"
  }
}
```

## Logs

El proceso usa `LoggerArchivo` de `ETL.Common`:

- log historico acumulativo;
- log del ultimo proceso recreado en cada ejecucion.

## SQL

Las consultas base de licitaciones estan documentadas en:

```text
sql/ConsultasLicitacionesInfoTransparencia.sql
sql/ConsultasOCInfoTransparencia.sql
```

El proceso no usa linked server. Por eso C# coordina la lectura desde `ChileCompra` y la escritura hacia `InfoTransparencia` con dos conexiones distintas.

Si mas adelante se decide crear stored procedures, deben quedar versionados en la carpeta `sql` del proyecto y configurados en `appsettings.json`.
