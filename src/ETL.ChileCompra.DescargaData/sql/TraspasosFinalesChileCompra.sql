CREATE OR ALTER PROCEDURE dbo.ETL_TraspasarLicitaciones
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Columnas nvarchar(max);

    SELECT @Columnas = STRING_AGG(CONVERT(nvarchar(max), QUOTENAME(name)), N',') WITHIN GROUP (ORDER BY column_id)
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.DatosAbiertos_Licitaciones_Stage');

    IF @Columnas IS NULL
    BEGIN
        THROW 50000, 'No existe dbo.DatosAbiertos_Licitaciones_Stage o no tiene columnas.', 1;
    END;

    DECLARE @Periodos TABLE (Anio int NOT NULL PRIMARY KEY);

    INSERT INTO @Periodos (Anio)
    SELECT DISTINCT TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4))
    FROM dbo.DatosAbiertos_Licitaciones_Stage
    WHERE Archivo IS NOT NULL
      AND PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4)) IS NOT NULL;

    DECLARE @Anio int;
    DECLARE @NombreTablaFinal sysname;
    DECLARE @TablaFinal nvarchar(300);
    DECLARE @Sql nvarchar(max);

    DECLARE Periodos CURSOR LOCAL FAST_FORWARD FOR
        SELECT Anio FROM @Periodos ORDER BY Anio;

    OPEN Periodos;
    FETCH NEXT FROM Periodos INTO @Anio;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @NombreTablaFinal = N'DatosAbiertos_Licitaciones_' + RIGHT(CONVERT(varchar(4), @Anio), 2);
        SET @TablaFinal = QUOTENAME(N'dbo') + N'.' + QUOTENAME(@NombreTablaFinal);

        IF OBJECT_ID(N'dbo.' + @NombreTablaFinal, N'U') IS NULL
        BEGIN
            CLOSE Periodos;
            DEALLOCATE Periodos;
            THROW 50001, 'No existe tabla final anual de licitaciones.', 1;
        END;

        SET @Sql = N'
DELETE destino
FROM ' + @TablaFinal + N' AS destino
WHERE EXISTS
(
    SELECT 1
    FROM dbo.DatosAbiertos_Licitaciones_Stage AS origen
    WHERE origen.Archivo = destino.Archivo
      AND origen.Archivo IS NOT NULL
      AND PATINDEX(''%[12][0-9][0-9][0-9]-%'', origen.Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(origen.Archivo, PATINDEX(''%[12][0-9][0-9][0-9]-%'', origen.Archivo), 4)) = @Anio
);

INSERT INTO ' + @TablaFinal + N' (' + @Columnas + N')
SELECT ' + @Columnas + N'
FROM dbo.DatosAbiertos_Licitaciones_Stage
WHERE Archivo IS NOT NULL
  AND PATINDEX(''%[12][0-9][0-9][0-9]-%'', Archivo) > 0
  AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX(''%[12][0-9][0-9][0-9]-%'', Archivo), 4)) = @Anio;';

        EXEC sys.sp_executesql @Sql, N'@Anio int', @Anio = @Anio;

        FETCH NEXT FROM Periodos INTO @Anio;
    END;

    CLOSE Periodos;
    DEALLOCATE Periodos;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ETL_TraspasarOC
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Columnas nvarchar(max);

    SELECT @Columnas = STRING_AGG(CONVERT(nvarchar(max), QUOTENAME(name)), N',') WITHIN GROUP (ORDER BY column_id)
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.DatosAbiertos_OC_Stage');

    IF @Columnas IS NULL
    BEGIN
        THROW 50010, 'No existe dbo.DatosAbiertos_OC_Stage o no tiene columnas.', 1;
    END;

    DECLARE @Periodos TABLE (Anio int NOT NULL PRIMARY KEY);

    INSERT INTO @Periodos (Anio)
    SELECT DISTINCT TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4))
    FROM dbo.DatosAbiertos_OC_Stage
    WHERE Archivo IS NOT NULL
      AND PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX('%[12][0-9][0-9][0-9]-%', Archivo), 4)) IS NOT NULL;

    DECLARE @Anio int;
    DECLARE @NombreTablaFinal sysname;
    DECLARE @TablaFinal nvarchar(300);
    DECLARE @Sql nvarchar(max);

    DECLARE Periodos CURSOR LOCAL FAST_FORWARD FOR
        SELECT Anio FROM @Periodos ORDER BY Anio;

    OPEN Periodos;
    FETCH NEXT FROM Periodos INTO @Anio;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @NombreTablaFinal = N'DatosAbiertos_OC_' + RIGHT(CONVERT(varchar(4), @Anio), 2);
        SET @TablaFinal = QUOTENAME(N'dbo') + N'.' + QUOTENAME(@NombreTablaFinal);

        IF OBJECT_ID(N'dbo.' + @NombreTablaFinal, N'U') IS NULL
        BEGIN
            CLOSE Periodos;
            DEALLOCATE Periodos;
            THROW 50011, 'No existe tabla final anual de OC.', 1;
        END;

        SET @Sql = N'
DELETE destino
FROM ' + @TablaFinal + N' AS destino
WHERE EXISTS
(
    SELECT 1
    FROM dbo.DatosAbiertos_OC_Stage AS origen
    WHERE origen.Archivo = destino.Archivo
      AND origen.Archivo IS NOT NULL
      AND PATINDEX(''%[12][0-9][0-9][0-9]-%'', origen.Archivo) > 0
      AND TRY_CONVERT(int, SUBSTRING(origen.Archivo, PATINDEX(''%[12][0-9][0-9][0-9]-%'', origen.Archivo), 4)) = @Anio
);

INSERT INTO ' + @TablaFinal + N' (' + @Columnas + N')
SELECT ' + @Columnas + N'
FROM dbo.DatosAbiertos_OC_Stage
WHERE Archivo IS NOT NULL
  AND PATINDEX(''%[12][0-9][0-9][0-9]-%'', Archivo) > 0
  AND TRY_CONVERT(int, SUBSTRING(Archivo, PATINDEX(''%[12][0-9][0-9][0-9]-%'', Archivo), 4)) = @Anio;';

        EXEC sys.sp_executesql @Sql, N'@Anio int', @Anio = @Anio;

        FETCH NEXT FROM Periodos INTO @Anio;
    END;

    CLOSE Periodos;
    DEALLOCATE Periodos;
END;
GO
