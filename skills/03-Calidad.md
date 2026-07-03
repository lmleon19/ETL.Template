# Revisión de Calidad

Antes de finalizar el desarrollo verificar el siguiente checklist.

---

## Arquitectura

□ No se modificó la estructura del template.

□ No se modificó ETL.Common.

□ El proyecto mantiene la arquitectura definida.

---

## Program.cs

□ Program.cs representa claramente el flujo.

□ No contiene lógica extensa.

□ Puede leerse de arriba hacia abajo.

□ Cada paso valida si puede continuar.

□ El proceso retorna 0 cuando termina correctamente.

□ El proceso retorna distinto de 0 cuando falla.

---

## Métodos

□ Todos los métodos tienen una única responsabilidad.

□ Los nombres son autoexplicativos.

□ No existen métodos excesivamente largos.

---

## Código

□ No existe código duplicado.

□ Se reutilizó ETL.Common cuando fue posible.

□ Se utilizó LINQ cuando simplifica el código.

□ Se utilizan métodos Async.

---

## Logging

□ Se registra inicio.

□ Se registra fin.

□ Se registran errores.

□ Se registran advertencias importantes.

---

## SQL

□ Existe tabla Stage.

□ La carga masiva utiliza SqlBulkCopy cuando corresponde.

□ El procesamiento final se realiza mediante Stored Procedure.

---

## Errores

□ Todas las excepciones son registradas.

□ No existen catch vacíos.

□ No se pierde información del error.

□ Un error en un paso impide ejecutar los pasos posteriores.

---

## Legibilidad

□ El código puede entenderse sin explicaciones adicionales.

□ Los nombres son claros.

□ El flujo del proceso es evidente.

---

## Documentación

□ README.md está actualizado.

□ docs contiene la documentación institucional vigente.

□ examples contiene solo ejemplos pequeños, no ETL completos.

Si algún punto no se cumple, debe corregirse antes de finalizar el desarrollo.
