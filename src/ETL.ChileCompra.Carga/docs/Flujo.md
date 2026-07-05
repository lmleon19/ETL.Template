# Flujo ETL ChileCompra

Este documento describe el flujo real del ETL `ETL.ChileCompra.Carga`.

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
2. Prepara la ejecución:
   - crea o limpia las carpetas de trabajo necesarias;
   - limpia las tablas Stage.
3. Calcula los períodos a procesar:
   - toma la cantidad de meses cerrados desde `ChileCompra:MesesCerradosACargar`;
   - calcula los meses cerrados anteriores a la fecha actual;
   - identifica los años involucrados.
4. Crea las tablas finales anuales faltantes mediante el procedimiento configurado en `ChileCompra:Procedimientos:CrearTablasFinalesAnuales`:
   - licitaciones: `dbo.DatosAbiertos_Licitaciones_YY`;
   - OC: `dbo.DatosAbiertos_OC_YY`.
5. Prepara la conversión de moneda:
   - descarga el archivo de paridad;
   - carga las equivalencias necesarias para el mapeo.
6. Descarga los ZIP de licitaciones:
   - genera una URL por período;
   - guarda cada ZIP en la carpeta de trabajo de licitaciones.
7. Descarga los ZIP de OC:
   - genera una URL por período;
   - guarda cada ZIP en la carpeta de trabajo de OC.
8. Descomprime los ZIP descargados:
   - extrae licitaciones en su carpeta de extraídos;
   - extrae OC en su carpeta de extraídos.
9. Valida los CSV extraídos:
   - valida licitaciones contra el modelo Stage;
   - valida OC contra el modelo Stage;
   - permite filas inválidas solo hasta el porcentaje configurado en `ChileCompra:PorcentajeMaximoFilasInvalidas`;
   - registra en log las filas omitidas con archivo, número de registro, columnas esperadas, columnas encontradas y muestra.
10. Carga Stage de licitaciones:
    - carga el lookup `IdOrPortal` de instituciones;
    - lee los CSV de licitaciones;
    - mapea cada fila al modelo `RegistroLicitacionStage`;
    - valida RUT antes de separar número y dígito verificador;
    - deja los campos calculados de RUT en `null` cuando el valor no es un RUT válido;
    - carga los registros en `dbo.DatosAbiertos_Licitaciones_Stage`.
11. Carga Stage de OC:
    - carga el lookup `IdOrPortal` de instituciones;
    - lee los CSV de OC;
    - mapea cada fila al modelo `RegistroOrdenCompraStage`;
    - valida RUT antes de separar número y dígito verificador;
    - deja los campos calculados de RUT en `null` cuando el valor no es un RUT válido;
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
16. Retorna `0` si todo terminó correctamente.
17. Retorna `1` si algún paso falla o si ocurre un error no controlado.

## Reglas operativas relevantes

- El flujo debe poder leerse desde `Program.cs` sin revisar toda la implementación.
- Cada paso debe fallar rápido: si un resultado no es exitoso, no se ejecutan los pasos siguientes.
- Los valores operativos deben venir desde `appsettings.json`.
- Las siglas de dominio deben mantenerse consistentes; para órdenes de compra se usa `OC`.
- La tolerancia de filas CSV inválidas es configurable.
- Las filas inválidas aceptadas no desaparecen silenciosamente: quedan registradas en log.
- Los RUT deben validarse antes de transformarse en campos calculados.
