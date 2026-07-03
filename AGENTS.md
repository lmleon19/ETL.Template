# Instrucciones para IA

Este repositorio define el template institucional para procesos ETL en .NET.

Antes de crear o modificar un ETL especifico, leer y respetar estos documentos:

1. `skills/00-Filosofia.md`
2. `skills/01-Desarrollo.md`
3. `skills/02-UsarCommon.md`
4. `skills/03-Calidad.md`
5. `skills/04-Documentar.md`

Reglas obligatorias:

- No modificar `ETL.Common` durante el desarrollo normal de un ETL especifico.
- No cambiar la arquitectura base del template sin una solicitud explicita.
- Implementar la logica de negocio solo dentro del proyecto del ETL.
- Mantener `Program.cs` como flujo principal legible, sin logica extensa.
- Reutilizar `ETL.Common` antes de crear utilidades nuevas.
- Mantener nombres en espanol, claros y autoexplicativos.
- Actualizar documentacion cuando el cambio lo requiera.
- Ejecutar pruebas antes de finalizar cuando sea posible.

