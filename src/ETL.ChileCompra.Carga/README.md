# ETL.ChileCompra.Carga

Proceso ETL para descargar, preparar y cargar datos abiertos de ChileCompra.

## Alcance

- Limpia carpetas de trabajo.
- Calcula los ultimos cuatro meses cerrados.
- Descarga archivos ZIP de licitaciones y ordenes de compra.
- Descarga archivo de paridad de moneda.
- Descomprime archivos descargados.
- Valida CSV extraidos.
- Carga tablas Stage.
- Ejecuta traspaso final hacia tablas anuales.
- Mueve ZIP procesados a carpeta final.

## Tablas

- `dbo.DatosAbiertos_Licitaciones_Stage`
- `dbo.DatosAbiertos_OC_Stage`
- `dbo.DatosAbiertos_Licitaciones_YY`
- `dbo.DatosAbiertos_OC_YY`

## Configuracion

Las URLs y nombres de tablas viven en `appsettings.json`.

La credencial de base de datos esta configurada temporalmente para pruebas locales y debe moverse a configuracion no versionada antes de publicar.

