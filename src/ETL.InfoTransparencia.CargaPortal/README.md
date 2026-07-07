# ETL.InfoTransparencia.CargaPortal

Proyecto ETL para cargar tablas Portal desde `BDC_Datamart` hacia `InfoTransparenciaPROD`.

## Objetivo

Ejecutar secuencialmente cargas Portal mediante procedimientos almacenados en la base destino, dejando cada carga como un paso visible en `Program.cs`.

Las cargas implementadas sincronizan:

- origen: `BDC_Datamart.dbo.PORTAL_Organismos`;
- destino: `InfoTransparenciaPROD.dbo.PORTAL_Organismos`;
- procedimiento: `dbo.ETL_CargaPortal_Organismos`.
- origen: `BDC_Datamart.dbo.PORTAL_Solicitantes`;
- destino: `InfoTransparenciaPROD.dbo.PORTAL_Solicitantes`;
- procedimiento: `dbo.ETL_CargaPortal_Solicitantes`.
- origen: `BDC_Datamart.dbo.PORTAL_Solicitudes`;
- destino: `InfoTransparenciaPROD.dbo.PORTAL_Solicitudes`;
- procedimiento: `dbo.ETL_CargaPortal_Solicitudes`.
- origen: `BDC_Datamart.dbo.PORTAL_Temas_solicitud`;
- destino: `InfoTransparenciaPROD.dbo.PORTAL_Temas_solicitud`;
- procedimiento: `dbo.ETL_CargaPortal_TemasSolicitud`.
- origen: `BDC_Datamart.dbo.PORTAL_TrazaEstadoSolicitud`;
- destino: `InfoTransparenciaPROD.dbo.PORTAL_TrazaEstadoSolicitud`;
- procedimiento: `dbo.ETL_CargaPortal_TrazaEstadoSolicitud`.

## Configuracion

La configuracion propia del proceso vive en:

```json
"ConnectionStrings": {
  "InfoTransparencia": "",
  "BDC_Datamart": ""
},
"InfoTransparenciaCargaPortal": {
  "NombreConexionDestino": "InfoTransparencia",
  "TimeoutComandoSegundos": 0,
  "Cargas": [
    {
      "Nombre": "PORTAL_Organismos",
      "Procedimiento": "dbo.ETL_CargaPortal_Organismos",
      "Activo": true
    },
    {
      "Nombre": "PORTAL_Solicitantes",
      "Procedimiento": "dbo.ETL_CargaPortal_Solicitantes",
      "Activo": true
    },
    {
      "Nombre": "PORTAL_Solicitudes",
      "Procedimiento": "dbo.ETL_CargaPortal_Solicitudes",
      "Activo": true
    },
    {
      "Nombre": "PORTAL_Temas_solicitud",
      "Procedimiento": "dbo.ETL_CargaPortal_TemasSolicitud",
      "Activo": true
    },
    {
      "Nombre": "PORTAL_TrazaEstadoSolicitud",
      "Procedimiento": "dbo.ETL_CargaPortal_TrazaEstadoSolicitud",
      "Activo": true
    }
  ],
  "Logs": {
    "Carpeta": "logs",
    "Historico": "InfoTransparencia-CargaPortal-historico.txt",
    "UltimoProceso": "InfoTransparencia-CargaPortal-ultimo.txt"
  }
}
```

`TimeoutComandoSegundos` usa `0` para no limitar el tiempo de ejecucion del comando.

## SQL

Antes de ejecutar el ETL, el usuario u operador debe ejecutar en `InfoTransparenciaPROD`:

```text
sql/ETL_CargaPortal_Organismos.sql
sql/ETL_CargaPortal_Solicitantes.sql
sql/ETL_CargaPortal_Solicitudes.sql
sql/ETL_CargaPortal_TemasSolicitud.sql
sql/ETL_CargaPortal_TrazaEstadoSolicitud.sql
```

Los procedimientos no usan tabla temporal porque origen y destino estan en el mismo servidor SQL Server. Leen el origen con nombre de tres partes y sincronizan el destino dentro de una transaccion.

## Logs

El proceso usa `LoggerArchivo` de `ETL.Common`:

- log historico acumulativo;
- log del ultimo proceso recreado en cada ejecucion.

## Ejecucion

```powershell
dotnet run --project src/ETL.InfoTransparencia.CargaPortal/ETL.InfoTransparencia.CargaPortal.csproj
```

## Consideraciones

Para agregar nuevas tablas Portal, crear el script del procedimiento almacenado en la carpeta `sql`, ejecutarlo en la base destino, agregar una entrada activa en `InfoTransparenciaCargaPortal:Cargas` y agregar su paso propio en `Program.cs`.
