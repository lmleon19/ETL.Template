# Uso de ETL.Common

## Objetivo

ETL.Common contiene funcionalidades reutilizables para todos los ETL.

Debe utilizarse tal como fue entregado por el template.

Todo ETL nuevo debe reutilizar `LoggerArchivo` de `ETL.Common` para persistir logs en archivos TXT, salvo que exista una solicitud explicita de usar otro mecanismo.

La configuracion estandar debe vivir en la seccion `Logs` dentro de la seccion propia del ETL en `appsettings.json`.

Ejemplo:

```json
"NombreProceso": {
  "Logs": {
    "Carpeta": "logs",
    "Historico": "NombreProceso-historico.txt",
    "UltimoProceso": "NombreProceso-ultimo.txt"
  }
}
```

---

# Antes de programar

Antes de crear una nueva función:

1. Revisar ETL.Common.
2. Buscar una función equivalente.
3. Reutilizarla si existe.

---

# Restricciones

Durante el desarrollo del ETL NO está permitido:

- modificar ETL.Common
- agregar clases
- agregar métodos
- eliminar métodos
- cambiar firmas de métodos

ETL.Common es una librería estable.

---

# Si una función no existe

Si una funcionalidad no existe dentro de ETL.Common:

Implementarla únicamente dentro del proyecto específico.

No modificar ETL.Common.

La evolución de ETL.Common es una decisión de arquitectura y no forma parte del desarrollo normal de un ETL.

---

# Lógica de negocio

Nunca agregar a ETL.Common lógica específica como:

DescargarChileCompra()

ProcesarLobby()

ActualizarSAI()

Toda lógica de negocio pertenece únicamente al proyecto correspondiente.

---

# Responsabilidad de ETL.Common

ETL.Common puede contener herramientas genericas reutilizables.

Ejemplos correctos:

- leer CSV y devolver registros invalidos
- calcular porcentaje de registros invalidos
- validar estructura basica
- escribir logs a archivos
- normalizar texto generico

ETL.Common no debe decidir reglas propias de un negocio o proceso especifico.

Ejemplos de decisiones que pertenecen al ETL especifico:

- porcentaje maximo aceptado de filas invalidas
- si una fila invalida se omite o detiene el proceso
- que campos calculados se dejan en `null`
- que datos se consideran obligatorios para una carga concreta
- nombres de tablas, carpetas, URLs o stored procedures del proceso

Si una utilidad comun necesita un umbral o una regla variable, debe recibirla como parametro o mediante una opcion generica. No debe quedar hardcodeada dentro de ETL.Common.

---

# CSV con errores

Cuando un ETL permita tolerancia de filas CSV invalidas:

- el porcentaje maximo debe venir desde configuracion o parametro
- las filas omitidas deben quedar registradas en log
- el log debe incluir archivo, numero de registro, columnas esperadas, columnas encontradas y una muestra
- si el porcentaje supera el maximo permitido, el proceso debe fallar con mensaje claro

La utilidad comun puede detectar y reportar filas invalidas, pero el ETL especifico decide el umbral y la accion.

---

# Objetivo final

Todos los ETL deben reutilizar ETL.Common sin modificarlo.
