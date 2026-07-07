CREATE OR ALTER PROCEDURE dbo.ETL_CargaPortal_TemasSolicitud
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    MERGE INTO dbo.PORTAL_Temas_solicitud WITH (HOLDLOCK) AS DN
    USING BDC_Datamart.dbo.PORTAL_Temas_solicitud AS D
        ON D.Codigo_Solicitud = DN.Codigo_Solicitud
        AND D.nombre = DN.nombre
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
            D.IDORPortal,
            D.Organismo,
            D.Region
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
            DN.IDORPortal,
            DN.Organismo,
            DN.Region)
    THEN UPDATE SET
        DN.Fecha_Ingreso = D.Fecha_Ingreso,
        DN.Mes_Ingreso = D.Mes_Ingreso,
        DN.Ano_Ingreso = D.Ano_Ingreso,
        DN.Fecha_Respuesta = D.Fecha_Respuesta,
        DN.Mes_Respuesta = D.Mes_Respuesta,
        DN.Ano_Respuesta = D.Ano_Respuesta,
        DN.Fecha_Anulacion = D.Fecha_Anulacion,
        DN.Fecha_Ultimo_Estado = D.Fecha_Ultimo_Estado,
        DN.IDORPortal = D.IDORPortal,
        DN.Organismo = D.Organismo,
        DN.Region = D.Region
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Codigo_Solicitud,
            nombre,
            Fecha_Ingreso,
            Mes_Ingreso,
            Ano_Ingreso,
            Fecha_Respuesta,
            Mes_Respuesta,
            Ano_Respuesta,
            Fecha_Anulacion,
            Fecha_Ultimo_Estado,
            IDORPortal,
            Organismo,
            Region)
        VALUES (
            D.Codigo_Solicitud,
            D.nombre,
            D.Fecha_Ingreso,
            D.Mes_Ingreso,
            D.Ano_Ingreso,
            D.Fecha_Respuesta,
            D.Mes_Respuesta,
            D.Ano_Respuesta,
            D.Fecha_Anulacion,
            D.Fecha_Ultimo_Estado,
            D.IDORPortal,
            D.Organismo,
            D.Region)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    COMMIT TRANSACTION;
END;
