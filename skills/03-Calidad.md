# Revision de Calidad

Antes de finalizar el desarrollo verificar el siguiente checklist.

---

## Arquitectura

- Antes de modificar archivos, se presento un plan cuando correspondia.
- Se recibio instruccion explicita de ejecutar antes de crear o modificar codigo, SQL, configuracion o documentacion.

- No se modifico la estructura del template.
- No se modifico ETL.Common durante el desarrollo normal de un ETL especifico.
- El proyecto mantiene la arquitectura definida.
- El nombre del proyecto sigue el formato `ETL.<Dominio>.<Accion><Objeto>` y coincide con carpeta, `.csproj`, namespace, configuracion, logs y documentacion.

---

## Program.cs

- Program.cs representa claramente el flujo.
- No contiene logica extensa.
- Puede leerse de arriba hacia abajo.
- Los pasos pueden activarse o desactivarse durante desarrollo mediante `if (true)` / `if (false)`.
- Los `if (true)` / `if (false)` finales son coherentes con el flujo esperado del ETL.
- Si un paso queda desactivado intencionalmente, existe una razon clara en el codigo o en la documentacion del flujo.
- Cada paso del flujo tiene un comentario corto que explica su objetivo.
- Cada paso valida si puede continuar.
- Las etapas separables del proceso estan separadas en pasos distintos.
- El proceso retorna 0 cuando termina correctamente.
- El proceso retorna distinto de 0 cuando falla.

---

## Metodos

- Todos los metodos tienen una unica responsabilidad.
- Los nombres son autoexplicativos.
- No existen metodos excesivamente largos.

---

## Codigo

- No existe codigo duplicado.
- Se reutilizo ETL.Common cuando fue posible.
- Se utilizo LINQ cuando simplifica el codigo.
- Se utilizan metodos Async.
- No se inventaron estructuras, columnas, tipos de datos, nombres de base de datos, schemas, tablas, procedimientos ni reglas de negocio.
- Cuando falto informacion para definir un contrato de datos, se pregunto al usuario o se uso una fuente real del repositorio.
- Los archivos nuevos de codigo y documentacion estan en UTF-8.
- No se usaron encodings antiguos salvo que un archivo fuente externo lo exigiera.
- Los valores operativos vienen desde configuracion y no estan hardcodeados.
- Las siglas de dominio se usan de forma consistente.
- Los datos se validan semanticamente antes de transformarlos en campos calculados.
- Los datos invalidos no se convierten en datos aparentemente validos.

---

## Logging

- Se registra inicio.
- Se registra fin.
- Cuando se procesan periodos, se registra inicio y fin de cada periodo con el paso correspondiente.
- Se registran errores.
- Se registran advertencias importantes.
- Las filas o datos omitidos por validacion quedan trazables en log cuando tienen impacto operativo.
- Los logs se escriben en archivo historico y archivo del ultimo proceso.

---

## SQL

- Existe tabla Stage.
- Si el ETL requiere crear objetos de base de datos, los scripts estan en la carpeta `sql` del proyecto ETL.
- El ETL informa que esos scripts deben ejecutarse previamente por el usuario u operador.
- El ETL no arma SQL de estructura permanente directamente desde C#.
- Si el ETL debe preparar estructura durante el flujo, lo hace ejecutando stored procedures existentes y configurados.
- La carga masiva utiliza SqlBulkCopy cuando corresponde.
- El procesamiento final se realiza mediante Stored Procedure.
- Los scripts de stored procedures usan `CREATE OR ALTER PROCEDURE` cuando corresponde.
- Los stored procedures tecnicos usan una nomenclatura consistente, por ejemplo `dbo.ETL_...`.
- No se repite el nombre del dominio en el stored procedure si la base de datos ya identifica ese dominio.
- Si el estandar de la base usa `dbo`, no se crea un schema nuevo sin decision explicita.
- Los scripts de tablas evitan operaciones destructivas salvo instruccion explicita.
- El traspaso Stage a Final es reejecutable por archivo o periodo cuando corresponde.
- Si existe SQL dinamico, se justifica por una necesidad real y usa `QUOTENAME` / `sp_executesql` cuando corresponde.

---

## CSV

- La tolerancia de filas invalidas es parametrizable.
- Si se omiten filas invalidas, se registra archivo, registro, columnas esperadas, columnas encontradas y muestra.
- Si el porcentaje de filas invalidas supera el maximo permitido, el proceso falla.

---

## Errores

- Todas las excepciones son registradas.
- No existen catch vacios.
- No se pierde informacion del error.
- Un error en un paso impide ejecutar los pasos posteriores.

---

## Legibilidad

- El codigo puede entenderse sin explicaciones adicionales.
- Los nombres son claros.
- El flujo del proceso es evidente.

---

## Documentacion

- README.md esta actualizado.
- Si se agrego o modifico un script SQL, tambien se actualizaron `README.md` y `docs/Flujo.md` del proyecto ETL.
- docs contiene `ETL.Common.md`.
- La carpeta `docs` de la raiz no contiene documentacion especifica de un ETL.
- Cada proyecto ETL contiene su propio `docs/Flujo.md`.
- Al crear un ETL nuevo, se creo inmediatamente la carpeta `docs` del proyecto y el archivo `Flujo.md`.
- El documento de flujo del ETL describe punto a punto el orden real de `Program.cs`.
- Si cambio `Program.cs`, el orden de pasos o la responsabilidad de un paso, tambien cambio el documento de flujo correspondiente.
- examples contiene solo ejemplos pequenos, no ETL completos.

## Artefactos generados

- No quedaron versionados `bin` ni `obj`.
- No quedaron versionados logs de ejecucion.
- No quedaron versionados ZIP descargados ni CSV grandes de trabajo.
- No quedaron versionadas carpetas temporales.
- Los archivos de prueba versionados son pequenos, controlados y necesarios para validar una regla concreta.

Si algun punto no se cumple, debe corregirse antes de finalizar el desarrollo.
