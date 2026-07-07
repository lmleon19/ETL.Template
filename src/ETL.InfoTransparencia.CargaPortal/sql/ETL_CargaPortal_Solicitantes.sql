CREATE OR ALTER PROCEDURE dbo.ETL_CargaPortal_Solicitantes
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    MERGE INTO dbo.PORTAL_Solicitantes WITH (HOLDLOCK) AS DN
    USING BDC_Datamart.dbo.PORTAL_Solicitantes AS D
        ON D.Codigo_Solicitud = DN.Codigo_Solicitud
    WHEN MATCHED AND EXISTS (
        SELECT
            D.IDORPortal,
            D.Organismo,
            D.Fecha_Ingreso,
            D.Mes_Ingreso,
            D.Ano_Ingreso,
            D.Fecha_Respuesta,
            D.Mes_Respuesta,
            D.Ano_Respuesta,
            D.Fecha_Anulacion,
            D.Fecha_Ultimo_Estado,
            D.region,
            D.edad,
            D.sexo,
            D.nacionalidad,
            D.pais,
            D.comuna,
            D.nivel_educacional,
            D.TipoOcupacion,
            D.TipoOrganizacion
        EXCEPT
        SELECT
            DN.IDORPortal,
            DN.Organismo,
            DN.Fecha_Ingreso,
            DN.Mes_Ingreso,
            DN.Ano_Ingreso,
            DN.Fecha_Respuesta,
            DN.Mes_Respuesta,
            DN.Ano_Respuesta,
            DN.Fecha_Anulacion,
            DN.Fecha_Ultimo_Estado,
            DN.region,
            DN.edad,
            DN.sexo,
            DN.nacionalidad,
            DN.pais,
            DN.comuna,
            DN.nivel_educacional,
            DN.TipoOcupacion,
            DN.TipoOrganizacion)
    THEN UPDATE SET
        DN.IDORPortal = D.IDORPortal,
        DN.Organismo = D.Organismo,
        DN.Fecha_Ingreso = D.Fecha_Ingreso,
        DN.Mes_Ingreso = D.Mes_Ingreso,
        DN.Ano_Ingreso = D.Ano_Ingreso,
        DN.Fecha_Respuesta = D.Fecha_Respuesta,
        DN.Mes_Respuesta = D.Mes_Respuesta,
        DN.Ano_Respuesta = D.Ano_Respuesta,
        DN.Fecha_Anulacion = D.Fecha_Anulacion,
        DN.Fecha_Ultimo_Estado = D.Fecha_Ultimo_Estado,
        DN.region = D.region,
        DN.edad = D.edad,
        DN.sexo = D.sexo,
        DN.nacionalidad = D.nacionalidad,
        DN.pais = D.pais,
        DN.comuna = D.comuna,
        DN.nivel_educacional = D.nivel_educacional,
        DN.TipoOcupacion = D.TipoOcupacion,
        DN.TipoOrganizacion = D.TipoOrganizacion
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            Codigo_Solicitud,
            IDORPortal,
            Organismo,
            Fecha_Ingreso,
            Mes_Ingreso,
            Ano_Ingreso,
            Fecha_Respuesta,
            Mes_Respuesta,
            Ano_Respuesta,
            Fecha_Anulacion,
            Fecha_Ultimo_Estado,
            region,
            edad,
            sexo,
            nacionalidad,
            pais,
            comuna,
            nivel_educacional,
            TipoOcupacion,
            TipoOrganizacion)
        VALUES (
            D.Codigo_Solicitud,
            D.IDORPortal,
            D.Organismo,
            D.Fecha_Ingreso,
            D.Mes_Ingreso,
            D.Ano_Ingreso,
            D.Fecha_Respuesta,
            D.Mes_Respuesta,
            D.Ano_Respuesta,
            D.Fecha_Anulacion,
            D.Fecha_Ultimo_Estado,
            D.region,
            D.edad,
            D.sexo,
            D.nacionalidad,
            D.pais,
            D.comuna,
            D.nivel_educacional,
            D.TipoOcupacion,
            D.TipoOrganizacion)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;

    COMMIT TRANSACTION;
END;
