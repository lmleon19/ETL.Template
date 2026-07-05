# Documentación ETL

Al finalizar el desarrollo mantener actualizada la documentación.

## Documentación en el proyecto del ETL

Cada ETL específico debe mantener su propio `README.md`.

Como mínimo debe contener:

- Objetivo del ETL.
- Origen de los datos.
- Destino de los datos.
- Configuración necesaria.
- Parámetros operativos configurables, como meses a procesar, tolerancias, rutas, tablas y stored procedures.
- Tablas utilizadas.
- Procedimientos almacenados utilizados.
- Scripts SQL que debe ejecutar el usuario u operador antes de correr el ETL.
- Archivos involucrados.
- Dependencias externas.
- Forma de ejecución.
- Configuración del SQL Server Agent si aplica.
- Consideraciones importantes.
- Manejo de errores conocido.
- Reglas de tolerancia o rechazo de datos inválidos.
- Convenciones de nombres relevantes del ETL, incluyendo siglas de dominio como `OC` si aplican.

## Documentación en docs raíz

La carpeta `docs` de la raíz debe mantenerse simple.

Debe contener:

- `ETL.Common.md`: documentación general de las utilidades comunes.

No se deben agregar documentos genéricos adicionales en `docs` salvo que exista una solicitud explícita.

## Documento de flujo por ETL

Cada ETL debe tener un documento de flujo dentro de su propio proyecto.

Ruta recomendada:

```text
src/ETL.NombreProceso/docs/Flujo.md
```

Al crear un ETL nuevo, debe crearse inmediatamente la carpeta `docs` dentro del proyecto ETL y el archivo `Flujo.md`.

El archivo `docs/Flujo.md` del proyecto ETL debe describir punto a punto qué hace el proceso.

El flujo debe seguir el orden real de `Program.cs`.

Cada paso debe indicar, cuando corresponda:

- qué método ejecuta el proceso;
- qué datos lee;
- qué datos escribe;
- qué tablas usa;
- qué stored procedure ejecuta;
- qué validaciones aplica;
- qué reglas de negocio importantes considera;
- qué ocurre si el paso falla.

Si cambia `Program.cs`, cambia el orden de los pasos o cambia la responsabilidad de algún paso, debe actualizarse el documento de flujo del ETL correspondiente en la misma modificación.

Si se agrega o modifica un script SQL, debe actualizarse el `README.md` del ETL y el archivo `docs/Flujo.md` del proyecto ETL.

La documentación debe permitir que cualquier desarrollador pueda comprender el funcionamiento del ETL sin necesidad de revisar todo el código fuente.

---

# Examples

La carpeta examples contiene ejemplos pequeños de uso.

No debe contener ETL completos.

Cada ejemplo debe mostrar una sola idea reutilizable.
