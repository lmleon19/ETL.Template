CREATE OR ALTER PROCEDURE dbo.ETL_CrearTablasFinalesAnuales
AS
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID(N'dbo.DatosAbiertos_Licitaciones_Stage', N'U') IS NULL
    BEGIN
        THROW 50001, 'Debe existir dbo.DatosAbiertos_Licitaciones_Stage antes de crear la tabla final anual.', 1;
    END;

    IF OBJECT_ID(N'dbo.DatosAbiertos_OC_Stage', N'U') IS NULL
    BEGIN
        THROW 50002, 'Debe existir dbo.DatosAbiertos_OC_Stage antes de crear la tabla final anual.', 1;
    END;

    DECLARE @Periodos TABLE (Anio int NOT NULL PRIMARY KEY);

    INSERT INTO @Periodos (Anio)
    SELECT DISTINCT TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4))
    FROM dbo.DatosAbiertos_Licitaciones_Stage
    WHERE Archivo IS NOT NULL
      AND PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4)) IS NOT NULL
    UNION
    SELECT DISTINCT TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4))
    FROM dbo.DatosAbiertos_OC_Stage
    WHERE Archivo IS NOT NULL
      AND PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4)) IS NOT NULL;

    DECLARE @Anio int;
    DECLARE @SufijoAnio char(2);
    DECLARE @TablaLicitaciones sysname;
    DECLARE @TablaOC sysname;
    DECLARE @Sql nvarchar(max);

    DECLARE Periodos CURSOR LOCAL FAST_FORWARD FOR
        SELECT Anio FROM @Periodos ORDER BY Anio;

    OPEN Periodos;
    FETCH NEXT FROM Periodos INTO @Anio;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SufijoAnio = RIGHT(CONVERT(char(4), @Anio), 2);
        SET @TablaLicitaciones = N'DatosAbiertos_Licitaciones_' + @SufijoAnio;
        SET @TablaOC = N'DatosAbiertos_OC_' + @SufijoAnio;

        IF OBJECT_ID(N'dbo.' + @TablaLicitaciones, N'U') IS NULL
        BEGIN
            SET @Sql = N'SELECT TOP (0) * INTO ' + QUOTENAME(N'dbo') + N'.' + QUOTENAME(@TablaLicitaciones) + N'
FROM dbo.DatosAbiertos_Licitaciones_Stage;';

            EXEC sys.sp_executesql @Sql;
        END;

        IF OBJECT_ID(N'dbo.' + @TablaOC, N'U') IS NULL
        BEGIN
            SET @Sql = N'SELECT TOP (0) * INTO ' + QUOTENAME(N'dbo') + N'.' + QUOTENAME(@TablaOC) + N'
FROM dbo.DatosAbiertos_OC_Stage;';

            EXEC sys.sp_executesql @Sql;
        END;

        FETCH NEXT FROM Periodos INTO @Anio;
    END;

    CLOSE Periodos;
    DEALLOCATE Periodos;
END;
GO

-- Ejemplo de uso manual:
-- EXEC dbo.ETL_CrearTablasFinalesAnuales;
