CREATE OR ALTER PROCEDURE dbo.ETL_CargaPortal_Solicitudes
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    MERGE INTO dbo.PORTAL_Solicitudes WITH (HOLDLOCK) AS DN
    USING BDC_Datamart.dbo.PORTAL_Solicitudes AS D
        ON D.Codigo_Solicitud = DN.Codigo_Solicitud
    WHEN MATCHED AND EXISTS (
        SELECT
            D.Fecha_Ingreso,
            D.Mes_Ingreso,
            D.Ano_Ingreso,
            D.Fecha_Respuesta,
            D.Mes_Respuesta,
            D.Ano_Respuesta,
            D.Fecha_Anulacion,
            D.Fecha_Ultimo_Estado,
            D.Etapa,
            D.Estado,
            D.DiasCorridoTiempoRespuesta,
            D.DiasHabilesTiempoRespuesta,
            D.IDORPortal,
            D.Organismo,
            D.region,
            D.GruposInstituciones,
            D.OrganismoInteropera
        EXCEPT
        SELECT
            DN.Fecha_Ingreso,
            DN.Mes_Ingreso,
            DN.Ano_Ingreso,
            DN.Fecha_Respuesta,
            DN.Mes_Respuesta,
            DN.Ano_Respuesta,
            DN.Fecha_Anulacion,
            DN.Fecha_Ultimo_Estado,
            DN.Etapa,
            DN.Estado,
            DN.DiasCorridoTiempoRespuesta,
            DN.DiasHabilesTiempoRespuesta,
            DN.IDORPortal,
            DN.Organismo,
            DN.region,
            DN.GruposInstituciones,
            DN.OrganismoInteropera)
    THEN UPDATE SET
        DN.Fecha_Ingreso = D.Fecha_Ingreso,
        DN.Mes_Ingreso = D.Mes_Ingreso,
        DN.Ano_Ingreso = D.Ano_Ingreso,
        DN.Fecha_Respuesta = D.Fecha_Respuesta,
        DN.Mes_Respuesta = D.Mes_Respuesta,
        DN.Ano_Respuesta = D.Ano_Respuesta,
        DN.Fecha_Anulacion = D.Fecha_Anulacion,
        DN.Fecha_Ultimo_Estado = D.Fecha_Ultimo_Estado,
        DN.Etapa = D.Etapa,
        DN.Estado = D.Estado,
        DN.DiasCorridoTiempoRespuesta = D.DiasCorridoTiempoRespuesta,
        DN.DiasHabilesTiempoRespuesta = D.DiasHabilesTiempoRespuesta,
        DN.IDORPortal = D.IDORPortal,
        DN.Organismo = D.Organismo,
        DN.region = D.region,
        DN.GruposInstituciones = D.GruposInstituciones,
        DN.OrganismoInteropera = D.OrganismoInteropera
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Codigo_Solicitud,
            Fecha_Ingreso,
            Mes_Ingreso,
            Ano_Ingreso,
            Fecha_Respuesta,
            Mes_Respuesta,
            Ano_Respuesta,
            Fecha_Anulacion,
            Fecha_Ultimo_Estado,
            Etapa,
            Estado,
            DiasCorridoTiempoRespuesta,
            DiasHabilesTiempoRespuesta,
            IDORPortal,
            Organismo,
            region,
            GruposInstituciones,
            OrganismoInteropera)
        VALUES (
            D.Codigo_Solicitud,
            D.Fecha_Ingreso,
            D.Mes_Ingreso,
            D.Ano_Ingreso,
            D.Fecha_Respuesta,
            D.Mes_Respuesta,
            D.Ano_Respuesta,
            D.Fecha_Anulacion,
            D.Fecha_Ultimo_Estado,
            D.Etapa,
            D.Estado,
            D.DiasCorridoTiempoRespuesta,
            D.DiasHabilesTiempoRespuesta,
            D.IDORPortal,
            D.Organismo,
            D.region,
            D.GruposInstituciones,
            D.OrganismoInteropera)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    COMMIT TRANSACTION;
END;
