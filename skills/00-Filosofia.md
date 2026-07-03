# Filosofía de Desarrollo ETL

## Objetivo

Esta guía define los principios que deben seguir todos los procesos ETL desarrollados sobre ETL.Template.

Su objetivo es mantener un estándar único de desarrollo, facilitando la lectura, el mantenimiento y la evolución de todos los ETL de la organización.

Las reglas definidas aquí tienen prioridad sobre cualquier preferencia personal de programación.

---

# Principios

## Simplicidad

Siempre debe preferirse la solución más simple que resuelva correctamente el problema.

Evitar complejidad innecesaria, patrones de diseño injustificados o sobrearquitectura.

---

## Legibilidad

El código se escribe para ser leído.

Debe ser posible comprender el funcionamiento del ETL sin necesidad de recorrer múltiples clases o realizar un análisis profundo del código.

La claridad siempre tiene prioridad sobre reducir líneas de código.

---

## Flujo evidente

El flujo principal del proceso debe ser visible inmediatamente.

Una persona debe poder abrir Program.cs y comprender el proceso completo leyendo únicamente el flujo principal.

Los detalles de implementación pertenecen a métodos específicos.

---

## Responsabilidad única

Cada clase debe tener una única responsabilidad.

Cada método debe realizar una única tarea.

Si un método realiza múltiples tareas, debe dividirse.

---

## Métodos pequeños

Preferir varios métodos pequeños antes que pocos métodos extensos.

Los métodos deben tener nombres suficientemente descriptivos para que su propósito sea evidente.

---

## Nombres claros

Todos los nombres deben expresar claramente su finalidad.

Evitar abreviaciones innecesarias.

Evitar nombres genéricos.

Un nombre debe permitir comprender el propósito del código sin necesidad de leer su implementación.

---

## Reutilización

Antes de escribir código nuevo, reutilizar las funcionalidades existentes de ETL.Common cuando sea posible.

No duplicar código.

---

## Estabilidad del Template

El Template representa el estándar institucional.

Durante el desarrollo normal de un ETL no debe modificarse la arquitectura base ni ETL.Common.

Toda lógica específica pertenece únicamente al proyecto del ETL.

---

## Consistencia

Todos los ETL deben compartir la misma estructura, organización y estilo de programación.

Un desarrollador debe sentirse familiarizado con cualquier ETL del proyecto.

---

## Separación de responsabilidades

El proyecto específico contiene únicamente la lógica propia del proceso.

ETL.Common contiene únicamente funcionalidades reutilizables y genéricas.

Nunca mezclar lógica de negocio con utilidades generales.

---

## Logging

Todo proceso importante debe registrar información suficiente para permitir conocer:

- cuándo comenzó
- qué está realizando
- cuándo terminó
- si ocurrió un error
- cuánto tiempo demoró

El objetivo del log es facilitar la operación y el diagnóstico del proceso.

---

## Validaciones

Toda entrada debe validarse antes de ser procesada.

Es preferible detectar un problema al inicio del proceso que durante la carga de datos.

Las validaciones deben entregar mensajes claros y útiles.

---

## Mantenibilidad

El código debe poder mantenerse durante años.

Siempre pensar que el próximo desarrollador podría no ser quien escribió originalmente el proceso.

El código debe ser fácil de modificar sin introducir errores.

---

## Calidad

Todo cambio debe dejar el código igual o mejor de como se encontró.

Evitar deuda técnica innecesaria.

---

## Inteligencia Artificial

Las herramientas de IA deben respetar esta filosofía.

No deben modificar la arquitectura del Template.

No deben modificar ETL.Common.

No deben inventar nuevas estructuras de carpetas.

No deben incorporar dependencias innecesarias.

Toda propuesta debe mantener la consistencia con el estándar definido por este proyecto.

---

# Objetivo Final

El propósito de este proyecto no es únicamente desarrollar ETL.

El objetivo es construir una plataforma donde cualquier desarrollador o herramienta de IA pueda crear procesos consistentes, legibles, mantenibles y fáciles de evolucionar, siguiendo un único estándar institucional.