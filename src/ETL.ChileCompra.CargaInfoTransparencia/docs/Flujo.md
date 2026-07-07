# Flujo ETL ChileCompra CargaInfoTransparencia

Este documento describe el flujo real del ETL `ETL.ChileCompra.CargaInfoTransparencia`.

Debe actualizarse cada vez que cambie el orden, la responsabilidad o el comportamiento de los pasos del proceso.

## Flujo actual

1. Configura el logger a archivos mediante `ChileCompraCargaInfoTransparencia:Logs`.
2. Registra el inicio del proceso.
3. Ejecuta `ValidarConfiguracionBase`.
4. Calcula en `Program.cs` los ultimos meses cerrados configurados en `MesesCerradosACargar`, sin considerar el mes actual.
5. Reutiliza los mismos periodos calculados para licitaciones y OC.
6. Ejecuta `CargarLicitacionesAsync(periodos)`.
7. Para cada periodo, arma el archivo mensual esperado con formato `lic_yyyy-M.csv`.
8. Lee desde la conexion `ChileCompra` la tabla anual configurada con `PrefijoLicitacionesOrigen` y `FormatoSufijoTablaAnual`.
9. Excluye licitaciones presentes en `dbo.Excluir_Licitaciones`, comparando por `CodigoExterno`.
10. Obtiene cabecera agregada de licitaciones adjudicadas seleccionadas.
11. Obtiene detalle de items adjudicados seleccionados.
12. Abre la conexion destino `InfoTransparencia`.
13. Elimina en `dbo.EX_MP_LicitacionDetalle` los registros con `Fecha_publicacion >= FechaDesde AND Fecha_publicacion < FechaHasta`.
14. Elimina en `dbo.EX_MP_Licitacion` los registros con `Fecha_publicacion >= FechaDesde AND Fecha_publicacion < FechaHasta`.
15. Carga cabecera y detalle con `SqlBulkCopy`.
16. Ejecuta `CargarOCAsync(periodos)`.
17. Para cada periodo, arma el archivo mensual esperado con formato `yyyy-M.csv`.
18. Lee desde la conexion `ChileCompra` la tabla anual configurada con `PrefijoOCOrigen` y `FormatoSufijoTablaAnual`.
19. Excluye OC presentes en `dbo.Excluir_OC`, comparando por `Codigo`.
20. Obtiene cabecera agregada de OC.
21. Obtiene detalle de items de OC.
22. Abre la conexion destino `InfoTransparencia`.
23. Elimina en `dbo.EX_MP_OrdenCompraDetalle` los registros con `Fecha_Envio >= FechaDesde AND Fecha_Envio < FechaHasta`.
24. Elimina en `dbo.EX_MP_OrdenCompra` los registros con `FechaEnvio >= FechaDesde AND FechaEnvio < FechaHasta`.
25. Carga cabecera y detalle de OC con `SqlBulkCopy`.
26. Registra el fin del proceso.

## Pendiente de definicion

Antes de implementar la carga real se debe definir:

- cadena de conexion destino `InfoTransparencia`;
- confirmacion de la estructura exacta de `dbo.EX_MP_Licitacion`;
- confirmacion de la estructura exacta de `dbo.EX_MP_LicitacionDetalle`;
- confirmacion de la estructura exacta de `dbo.EX_MP_OrdenCompra`;
- confirmacion de la estructura exacta de `dbo.EX_MP_OrdenCompraDetalle`.
