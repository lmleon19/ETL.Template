CREATE OR ALTER PROCEDURE dbo.ETL_CargaPortal_TrazaEstadoSolicitud
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    MERGE INTO dbo.PORTAL_TrazaEstadoSolicitud WITH (HOLDLOCK) AS DN
    USING BDC_Datamart.dbo.PORTAL_TrazaEstadoSolicitud AS D
        ON D.Codigo_Solicitud = DN.Codigo_Solicitud
        AND D.Fecha_Entrada_Fase = DN.Fecha_Entrada_Fase
        AND D.Estado = DN.Estado
    WHEN MATCHED AND EXISTS (
        SELECT D.Etapa
        EXCEPT
        SELECT DN.Etapa)
    THEN UPDATE SET
        DN.Etapa = D.Etapa
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Codigo_Solicitud,
            Fecha_Entrada_Fase,
            Estado,
            Etapa)
        VALUES (
            D.Codigo_Solicitud,
            D.Fecha_Entrada_Fase,
            D.Estado,
            D.Etapa)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    COMMIT TRANSACTION;
END;
