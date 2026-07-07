# Etl.ChileCompra.DescargaData

Proceso ETL para descargar, preparar y cargar datos abiertos de ChileCompra.

## Alcance

- Limpia carpetas de trabajo.
- Calcula los ultimos meses cerrados configurados.
- Descarga archivos ZIP de licitaciones y OC.
- Descarga archivo de paridad de moneda.
- Descomprime archivos descargados.
- Valida CSV extraidos.
- Carga tablas Stage.
- Ejecuta traspaso final de licitaciones y OC hacia tablas anuales.
- Mueve ZIP procesados a carpeta final.

## Tablas

- `dbo.DatosAbiertos_Licitaciones_Stage`
- `dbo.DatosAbiertos_OC_Stage`
- `dbo.DatosAbiertos_Licitaciones_YY`
- `dbo.DatosAbiertos_OC_YY`

## Scripts SQL

Los objetos de base de datos deben existir antes de ejecutar el ETL.

Los scripts se encuentran en:

- `sql/CrearTablasStageChileCompra.sql`
- `sql/CrearTablasFinalesAnualesChileCompra.sql`
- `sql/TraspasosFinalesChileCompra.sql`

Estos scripts debe ejecutarlos el usuario u operador en la base de datos correspondiente antes de correr el ETL.

El script `sql/CrearTablasFinalesAnualesChileCompra.sql` crea el procedimiento `dbo.ETL_CrearTablasFinalesAnuales`.

El ETL ejecuta ese procedimiento antes del traspaso final para crear las tablas anuales que falten segun los anios presentes en Stage.

```sql
EXEC dbo.ETL_CrearTablasFinalesAnuales;
```

## Configuracion

Las URLs, nombres de tablas y cantidad de meses cerrados a cargar viven en `appsettings.json`.

La cantidad de meses se configura en:

```json
"ChileCompra": {
  "MesesCerradosACargar": 4
}
```

La credencial de base de datos esta configurada temporalmente para pruebas locales y debe moverse a configuracion no versionada antes de publicar.

Los procedimientos de traspaso final se configuran por separado:

```json
"Procedimientos": {
  "CrearTablasFinalesAnuales": "dbo.ETL_CrearTablasFinalesAnuales",
  "TraspasoFinalLicitaciones": "dbo.ETL_TraspasarLicitaciones",
  "TraspasoFinalOC": "dbo.ETL_TraspasarOC"
}
```

## Logs

El proceso escribe logs en la carpeta configurada en `ChileCompra:Logs:Carpeta`.

- `ChileCompra-Carga-historico.txt`: acumula todas las ejecuciones.
- `ChileCompra-Carga-ultimo.txt`: se limpia al iniciar cada ejecucion y contiene solo el ultimo proceso.

## Filas CSV invalidas

El proceso permite omitir filas CSV con cantidad de columnas distinta al encabezado cuando no superan el porcentaje recibido por parametro. Si no se informa uno, usa el valor configurado en `ChileCompra:PorcentajeMaximoFilasInvalidas`.

Por defecto el maximo es `2`. Las filas omitidas se registran en el log con archivo, numero de registro, columnas esperadas, columnas encontradas y una muestra del contenido.


