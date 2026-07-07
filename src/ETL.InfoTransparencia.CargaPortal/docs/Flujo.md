# Flujo ETL InfoTransparencia CargaPortal

Este documento describe el flujo real del ETL `ETL.InfoTransparencia.CargaPortal`.

Debe actualizarse cada vez que cambie el orden, la responsabilidad o el comportamiento de los pasos del proceso.

## Flujo actual

1. Configura el logger a archivos mediante `InfoTransparenciaCargaPortal:Logs`.
2. Registra el inicio del proceso.
3. Ejecuta `ValidarConfiguracionBase`.
4. Valida que exista la conexion destino configurada por `NombreConexionDestino`.
5. Valida que exista al menos una carga configurada.
6. Ejecuta `CargarPortalOrganismosAsync`.
7. Para `PORTAL_Organismos`, ejecuta `dbo.ETL_CargaPortal_Organismos` en la base destino `InfoTransparenciaPROD`.
8. El procedimiento lee desde `BDC_Datamart.dbo.PORTAL_Organismos`.
9. El procedimiento sincroniza `dbo.PORTAL_Organismos` mediante `MERGE`: actualiza diferencias, inserta faltantes y elimina registros que ya no existen en origen.
10. Ejecuta `CargarPortalSolicitantesAsync`.
11. Para `PORTAL_Solicitantes`, ejecuta `dbo.ETL_CargaPortal_Solicitantes` en la base destino `InfoTransparenciaPROD`.
12. El procedimiento lee desde `BDC_Datamart.dbo.PORTAL_Solicitantes`.
13. El procedimiento sincroniza `dbo.PORTAL_Solicitantes` mediante `MERGE`: actualiza diferencias, inserta faltantes y elimina registros que ya no existen en origen.
14. Ejecuta `CargarPortalSolicitudesAsync`.
15. Para `PORTAL_Solicitudes`, ejecuta `dbo.ETL_CargaPortal_Solicitudes` en la base destino `InfoTransparenciaPROD`.
16. El procedimiento lee desde `BDC_Datamart.dbo.PORTAL_Solicitudes`.
17. El procedimiento sincroniza `dbo.PORTAL_Solicitudes` mediante `MERGE`: actualiza diferencias, inserta faltantes y elimina registros que ya no existen en origen.
18. Ejecuta `CargarPortalTemasSolicitudAsync`.
19. Para `PORTAL_Temas_solicitud`, ejecuta `dbo.ETL_CargaPortal_TemasSolicitud` en la base destino `InfoTransparenciaPROD`.
20. El procedimiento lee desde `BDC_Datamart.dbo.PORTAL_Temas_solicitud`.
21. El procedimiento sincroniza `dbo.PORTAL_Temas_solicitud` mediante `MERGE`: actualiza diferencias, inserta faltantes y elimina registros que ya no existen en origen.
22. Ejecuta `CargarPortalTrazaEstadoSolicitudAsync`.
23. Para `PORTAL_TrazaEstadoSolicitud`, ejecuta `dbo.ETL_CargaPortal_TrazaEstadoSolicitud` en la base destino `InfoTransparenciaPROD`.
24. El procedimiento lee desde `BDC_Datamart.dbo.PORTAL_TrazaEstadoSolicitud`.
25. El procedimiento sincroniza `dbo.PORTAL_TrazaEstadoSolicitud` mediante `MERGE`: actualiza `Etapa`, inserta faltantes y elimina registros que ya no existen en origen.
26. Si una carga falla, el proceso registra el error, no ejecuta las cargas siguientes y retorna codigo distinto de cero.
27. Si todas las cargas terminan correctamente, registra el fin del proceso y retorna cero.

## SQL

El script requerido para crear o actualizar el procedimiento almacenado esta en:

```text
sql/ETL_CargaPortal_Organismos.sql
sql/ETL_CargaPortal_Solicitantes.sql
sql/ETL_CargaPortal_Solicitudes.sql
sql/ETL_CargaPortal_TemasSolicitud.sql
sql/ETL_CargaPortal_TrazaEstadoSolicitud.sql
```

Debe ejecutarse previamente en la base destino `InfoTransparenciaPROD`.
