# Uso de ETL.Common

## Objetivo

ETL.Common contiene funcionalidades reutilizables para todos los ETL.

Debe utilizarse tal como fue entregado por el template.

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

# Objetivo final

Todos los ETL deben reutilizar ETL.Common sin modificarlo.