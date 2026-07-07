# Flujo ETL ChileCompra

Este documento describe el flujo real del ETL `Etl.ChileCompra.DescargaData`.

Debe actualizarse cada vez que cambie el orden, la responsabilidad o el comportamiento de los pasos del proceso.

## Requisito previo SQL

Antes de ejecutar el ETL, el usuario u operador debe crear o actualizar los objetos de base de datos requeridos usando los scripts del proyecto:

- `sql/CrearTablasStageChileCompra.sql`
- `sql/CrearTablasFinalesAnualesChileCompra.sql`
- `sql/TraspasosFinalesChileCompra.sql`

El script `sql/CrearTablasFinalesAnualesChileCompra.sql` crea el procedimiento `dbo.ETL_CrearTablasFinalesAnuales`.

El ETL ejecuta ese procedimiento antes del traspaso final para crear las tablas anuales que falten segun los anios presentes en Stage.

```sql
EXEC dbo.ETL_CrearTablasFinalesAnuales;
```

## Flujo principal

1. Registra el inicio del proceso en los logs configurados.
2. Prepara la ejecuciÃ³n:
   - crea o limpia las carpetas de trabajo necesarias;
   - limpia las tablas Stage.
3. Calcula los perÃ­odos a procesar:
   - toma la cantidad de meses cerrados desde `ChileCompra:MesesCerradosACargar`;
   - calcula los meses cerrados anteriores a la fecha actual;
   - identifica los aÃ±os involucrados.
4. Crea las tablas finales anuales faltantes mediante el procedimiento configurado en `ChileCompra:Procedimientos:CrearTablasFinalesAnuales`:
   - licitaciones: `dbo.DatosAbiertos_Licitaciones_YY`;
   - OC: `dbo.DatosAbiertos_OC_YY`.
5. Prepara la conversiÃ³n de moneda:
   - descarga el archivo de paridad;
   - carga las equivalencias necesarias para el mapeo.
6. Descarga los ZIP de licitaciones:
   - genera una URL por perÃ­odo;
   - registra inicio y fin de descarga para cada perÃ­odo;
   - guarda cada ZIP en la carpeta de trabajo de licitaciones.
7. Descarga los ZIP de OC:
   - genera una URL por perÃ­odo;
   - registra inicio y fin de descarga para cada perÃ­odo;
   - guarda cada ZIP en la carpeta de trabajo de OC.
8. Descomprime los ZIP descargados:
   - extrae licitaciones en su carpeta de extraÃ­dos;
   - extrae OC en su carpeta de extraÃ­dos.
9. Valida los CSV extraÃ­dos:
   - valida licitaciones contra el modelo Stage;
   - valida OC contra el modelo Stage;
   - permite filas invÃ¡lidas solo hasta el porcentaje configurado en `ChileCompra:PorcentajeMaximoFilasInvalidas`;
   - registra en log las filas omitidas con archivo, nÃºmero de registro, columnas esperadas, columnas encontradas y muestra.
10. Carga Stage de licitaciones:
    - carga el lookup `IdOrPortal` de instituciones;
    - lee los CSV de licitaciones;
    - mapea cada fila al modelo `RegistroLicitacionStage`;
    - normaliza los RUT para separar nÃºmero y dÃ­gito verificador;
    - compara `IdOrPortal` con el RUT sin formato, usando `Numero + DV`;
    - deja los campos calculados de RUT de proveedor en `null` cuando el valor no es un RUT vÃ¡lido;
    - carga los registros en `dbo.DatosAbiertos_Licitaciones_Stage`.
11. Carga Stage de OC:
    - carga el lookup `IdOrPortal` de instituciones;
    - lee los CSV de OC;
    - mapea cada fila al modelo `RegistroOrdenCompraStage`;
    - para `RutUnidadCompra`, separa `InstitucionRut_Numero` e `InstitucionRut_DV` validando solo estructura;
    - compara `IdOrPortal` con el RUT sin formato, usando `InstitucionRut_Numero + InstitucionRut_DV`;
    - deja los campos calculados de RUT de proveedor en `null` cuando el valor no es un RUT vÃ¡lido;
    - carga los registros en `dbo.DatosAbiertos_OC_Stage`.
12. Traspasa licitaciones a tablas finales:
    - ejecuta el procedimiento configurado en `ChileCompra:Procedimientos:TraspasoFinalLicitaciones`;
    - actualmente usa `dbo.ETL_TraspasarLicitaciones`;
    - elimina en la tabla anual final los archivos presentes en Stage;
    - inserta los registros Stage en la tabla anual correspondiente.
13. Traspasa OC a tablas finales:
    - ejecuta el procedimiento configurado en `ChileCompra:Procedimientos:TraspasoFinalOC`;
    - actualmente usa `dbo.ETL_TraspasarOC`;
    - elimina en la tabla anual final los archivos presentes en Stage;
    - inserta los registros Stage en la tabla anual correspondiente.
14. Archiva los ZIP procesados:
    - mueve los ZIP de licitaciones a la carpeta final;
    - mueve los ZIP de OC a la carpeta final;
    - reemplaza archivos existentes con el mismo nombre.
15. Registra el fin del proceso.
16. Retorna `0` si todo terminÃ³ correctamente.
17. Retorna `1` si algÃºn paso falla o si ocurre un error no controlado.

## Reglas operativas relevantes

- El flujo debe poder leerse desde `Program.cs` sin revisar toda la implementaciÃ³n.
- Cada paso debe fallar rÃ¡pido: si un resultado no es exitoso, no se ejecutan los pasos siguientes.
- Los valores operativos deben venir desde `appsettings.json`.
- Las siglas de dominio deben mantenerse consistentes; para Ã³rdenes de compra se usa `OC`.
- La tolerancia de filas CSV invÃ¡lidas es configurable.
- Las filas invÃ¡lidas aceptadas no desaparecen silenciosamente: quedan registradas en log.
- Los RUT de proveedor se validan completamente antes de transformarse en campos calculados.
- Los RUT de instituciÃ³n usados para lookup se comparan por estructura normalizada, sin puntos ni guion.
