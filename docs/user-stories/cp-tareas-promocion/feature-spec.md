# Feature: Ejecucion de Tareas de Promocion y Validacion

> **ID:** cp-tareas-promocion
> **User Story:** US-CP-04
> **Status:** proposed
> **Priority:** Media
> **Sprint:** TBD

---

## Descripcion

Esta feature implementa el ciclo central de trabajo del modulo Crowdpromotion: el promotor ejecuta acciones de difusion (compartir en redes, escribir resenas, publicar videos) y el artista verifica y recompensa esas acciones. Una vez que un promotor esta aprobado en un programa, puede ver las tareas activas definidas por el artista, enviar prueba de que las realizo mediante una URL, y quedar en estado "pendiente de validacion". El artista, por su parte, revisa las pruebas presentadas y decide si las valida o rechaza. Al validar, el sistema acredita automaticamente la recompensa en la wallet del promotor, cerrando el loop de incentivos.

El modelo de datos de esta feature esta disenado para soportar tareas repetibles: una misma tarea puede completarse multiples veces hasta un limite configurado por el artista (`MaxRepeticiones`). El diseno actual de la entidad `PromoTareaPromotor` modela cada completado como un registro independiente, lo cual permite trazabilidad completa de cada envio individual con su propia URL de prueba, comentarios y fechas. Los campos de agregacion (`VecesCompletada`, `FechaPrimeraCompletada`, `FechaUltimaCompletada`) que la User Story menciona en las vistas son datos calculados o derivados a partir del conjunto de registros por tarea y promotor, o requieren campos adicionales en la entidad segun la estrategia de implementacion elegida (ver seccion Cambios de Modelo de Dominio).

La feature tiene impacto en cuatro proyectos: el Backend implementa los endpoints CQRS con validaciones de negocio y la logica transaccional de acreditacion de recompensa; la Landing expone la vista del promotor para ver y completar tareas; el Admin expone la vista del artista para revisar y validar completados; y Shared define los tipos y schemas compartidos por ambos frontends.

---

## User Story

**Como** promotor aprobado en un programa de promocion
**Quiero** ver las tareas asignadas, marcarlas como completadas aportando una URL de prueba, y que el artista valide mi trabajo
**Para** acumular recompensas (dinero o puntos) por cada tarea verificada

---

## Actores

| Actor | Descripcion |
|-------|-------------|
| Promotor | Usuario con perfil de promotor activo, aprobado en el programa (EsAprobado = true, EsBloqueado = false) |
| Artista | Propietario del programa de promocion; valida o rechaza los completados presentados por los promotores |

---

## Precondiciones

- Promotor esta aprobado en el programa (EsAprobado = true, EsBloqueado = false en PromoProgramaPromotor)
- El programa tiene al menos una tarea activa (PromoTarea con EsActivo = true)
- La tabla maestra `Maestra_EstadoTareaPromo` tiene los datos seed cargados (Pendiente, Completada, Validada, Rechazada)
- El promotor tiene una PromotorWallet activa en la moneda de la recompensa (para acreditacion al validar)

---

## Flujo Principal: Ver y Completar Tareas (Promotor)

```
1. Promotor autenticado accede al detalle de su inscripcion aprobada en la Landing
2. Sistema muestra el listado de tareas activas del programa con el estado personal del promotor en cada una
3. Para cada tarea, el sistema muestra: nombre, descripcion, instrucciones (URL), tipo de evento, recompensa (importe/moneda o puntos), si es repetible y cuantas veces ya fue completada
4. Promotor selecciona una tarea que no tenga el maximo de completados alcanzado
5. Sistema muestra el formulario de completado: instrucciones, campo URL de prueba (obligatorio) y campo comentario (opcional)
6. Promotor ingresa la URL de prueba donde realizo la accion externa (story de Instagram, video de TikTok, URL del blog, etc.)
7. Promotor envia el formulario
8. Backend valida: promotor aprobado en el programa, programa activo, tarea activa, tarea no excede MaxRepeticiones, URL con formato valido
9. Si la tarea no tiene completados previos: Backend crea un nuevo PromoTareaPromotor con EstadoTareaId = Completada
10. Si la tarea ya tiene completados previos (tarea repetible): Backend crea un nuevo registro PromoTareaPromotor para este completado adicional
11. Frontend muestra toast: "Tarea enviada para validacion"
12. La tarea actualiza su estado visible en el listado del promotor
```

---

## Flujo Secundario: Validar Tareas (Artista)

```
1. Artista autenticado accede al detalle de un programa en el Admin
2. Sistema muestra la pestana "Tareas pendientes de validacion" con el conteo de completados sin revisar
3. Listado muestra por cada completado: nombre de la tarea, nombre del promotor, URL de prueba, comentario del promotor, cuantas veces ha completado esa tarea y fecha de completado
4. Artista hace click en un completado para revisarlo
5. Artista puede abrir la URL de prueba en nueva pestana para verificar la accion realizada
6. Artista elige una accion:
   a. Validar: Backend actualiza EstadoTareaId = Validada, registra FechaValidado, crea PromotorWalletTransaccion con el importe de la recompensa y actualiza SaldoDisponible y TotalGanado en PromotorWallet; Frontend muestra toast "Tarea validada y recompensa acreditada"
   b. Rechazar: Backend actualiza EstadoTareaId = Rechazada, guarda ComentarioValidacion del artista; Frontend muestra toast "Tarea rechazada"
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Tarea no repetible ya tiene un completado en estado Completada o Validada | No mostrar boton de completar; mostrar estado actual de la tarea |
| FA-02 | Tarea repetible con MaxRepeticiones alcanzado | No permitir mas envios; mostrar aviso "Limite de repeticiones alcanzado (X/X)" |
| FA-03 | Tarea fuera de fechas de vigencia (FechaFin pasada) | No permitir completar; mostrar aviso "Esta tarea ya no esta disponible" |
| FA-04 | Tarea desactivada por artista (EsActivo = false) | No mostrar la tarea en el listado activo del promotor |
| FA-05 | Promotor tiene un completado en estado Rechazada para esa tarea | Permitir re-envio con nueva URL de prueba; se crea un nuevo PromoTareaPromotor (el registro rechazado queda historico) |
| FA-06 | Programa desactivado por el artista | Mostrar tareas como solo lectura sin boton de completar; mostrar aviso "Programa inactivo" |
| FA-07 | Wallet del promotor no existe para la moneda de la recompensa | Backend crea la wallet automaticamente al momento de la primera acreditacion |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP04-1 | El promotor aprobado puede ver el listado de tareas activas de su programa con nombre, descripcion, tipo de evento, importe de recompensa, si es repetible y su estado personal en cada tarea | Backend + Landing |
| AC-CP04-2 | El promotor puede enviar el completado de una tarea proporcionando una URL de prueba valida (formato URL); el campo comentario es opcional con maximo 500 caracteres | Backend + Landing |
| AC-CP04-3 | Al completar una tarea, se crea un registro PromoTareaPromotor con EstadoTareaId = Completada (id 2), TareaId, ProgramaPromotorId, UrlPruebaCompletado y FechaCompletado correctamente persisted | Backend |
| AC-CP04-4 | Para tareas repetibles, el promotor puede enviar completados sucesivos hasta el limite MaxRepeticiones; cada completado crea un nuevo registro PromoTareaPromotor independiente | Backend |
| AC-CP04-5 | El intento de completar una tarea no repetible que ya tiene un completado en estado Completada o Validada retorna 400 Bad Request con mensaje descriptivo | Backend |
| AC-CP04-6 | El intento de completar una tarea repetible cuando el conteo de registros activos ya alcanzo MaxRepeticiones retorna 400 Bad Request con mensaje descriptivo | Backend |
| AC-CP04-7 | El artista propietario del programa puede ver el listado paginado de completados pendientes de validacion, con nombre de tarea, nombre del promotor, URL de prueba, comentario y fecha | Backend + Admin |
| AC-CP04-8 | Al validar un completado, el backend actualiza EstadoTareaId = Validada (id 3), registra FechaValidado, crea una PromotorWalletTransaccion con el importe de la tarea y actualiza el SaldoDisponible y TotalGanado del PromotorWallet en una sola transaccion de BD | Backend |
| AC-CP04-9 | Al rechazar un completado, el backend actualiza EstadoTareaId = Rechazada (id 4) y persiste el ComentarioValidacion del artista; el promotor puede ver el motivo de rechazo en su vista | Backend + Landing + Admin |
| AC-CP04-10 | Un promotor con un completado rechazado puede re-enviar la tarea con una nueva URL de prueba; el re-envio crea un nuevo PromoTareaPromotor y no modifica el registro rechazado existente | Backend |
| AC-CP04-11 | No se puede completar ninguna tarea de un programa con EsActivo = false; el backend retorna 400 con mensaje descriptivo | Backend |
| AC-CP04-12 | Un promotor no aprobado en el programa (EsAprobado = false o inexistente) que intenta enviar un completado recibe 403 Forbidden | Backend |
| AC-CP04-13 | Si la wallet del promotor no existe para la moneda de la recompensa al momento de validar, el sistema la crea automaticamente antes de acreditar | Backend |
| AC-CP04-14 | Los tipos TypeScript de tareas y completados estan definidos en shared y son reutilizados por Landing y Admin | Shared |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar los cuatro endpoints CQRS del modulo Crowdpromotion: GET mis-tareas (promotor), POST completar (promotor), GET tareas-pendientes (artista), PATCH validar (artista), PATCH rechazar (artista). Logica de negocio: validar aprobacion del promotor, verificar limites de repeticion, calcular conteos derivados. Logica transaccional de validacion: actualizar estado + crear PromotorWalletTransaccion + actualizar PromotorWallet en una sola transaccion. Creacion automatica de wallet si no existe. | ALTO |
| **Landing** | Implementar la vista de tareas del promotor dentro de la pantalla de detalle de su inscripcion aprobada: listado de tareas con estado personal, indicador de repeticiones, boton de completar condicional. Implementar el dialogo/formulario de completado con campo URL y comentario opcional. Mostrar motivo de rechazo cuando el completado esta en estado Rechazada con enlace a re-envio. | ALTO |
| **Admin** | Implementar la pestana "Tareas pendientes de validacion" en el detalle del programa del artista: listado paginado de completados, detalle de cada completado con URL de prueba clickeable, botones de Validar y Rechazar con dialogo de confirmacion para Rechazar (para ingresar comentario). Mostrar contadores de completados por estado. | ALTO |
| **Shared** | Definir tipos TypeScript para PromoTareaDto (tarea con estado personal del promotor), PromoTareaCompletadoDto (completado para vista artista), CompletarTareaRequest. Definir schemas Zod para la solicitud de completado. Agregar QUERY_KEYS para mis-tareas y tareas-pendientes. | MEDIO |

---

## Reglas de Negocio

| Regla | Descripcion |
|-------|-------------|
| RN-01 | Solo los promotores con EsAprobado = true y EsBloqueado = false en PromoProgramaPromotor pueden completar tareas del programa |
| RN-02 | Solo las tareas con EsActivo = true y dentro de sus fechas de vigencia (FechaInicio/FechaFin) pueden ser completadas |
| RN-03 | Solo los programas con EsActivo = true permiten envios de completados |
| RN-04 | Para tareas no repetibles (EsRepetible = false): el promotor no puede tener mas de un PromoTareaPromotor en estado Completada o Validada para esa tarea; un registro en estado Rechazada no cuenta como bloqueo |
| RN-05 | Para tareas repetibles (EsRepetible = true): el conteo de completados activos (estado Completada o Validada) debe ser menor que MaxRepeticiones; el promotor puede enviar un nuevo completado si hay cupo disponible |
| RN-06 | Cada envio de completado crea un nuevo registro PromoTareaPromotor independiente; los registros anteriores en estado Rechazada no se modifican al re-enviar |
| RN-07 | La acreditacion de la recompensa al validar es una operacion transaccional: si falla la creacion de PromotorWalletTransaccion o la actualizacion del saldo en PromotorWallet, el estado del PromoTareaPromotor no debe quedar en Validada |
| RN-08 | La recompensa acreditada corresponde al ImporteRecompensa y MonedaId definidos en la PromoTarea en el momento de la validacion, no en el momento del completado |
| RN-09 | Solo el artista propietario del programa puede validar o rechazar completados; otros artistas o el propio promotor no pueden realizar esta accion |
| RN-10 | El ComentarioValidacion es obligatorio al rechazar y opcional al validar |

---

## Cambios de Modelo de Dominio Requeridos

La entidad `PromoTareaPromotor` actual modela un registro por completado individual, con los campos: Id, TareaId, ProgramaPromotorId, EstadoTareaId, UrlPruebaCompletado, ComentarioPromotor, ComentarioValidacion, FechaCompletado, FechaValidado, FechaCreacion.

La User Story menciona los campos `VecesCompletada`, `FechaPrimeraCompletada` y `FechaUltimaCompletada` como datos a mostrar en las vistas. Estos NO existen en la entidad actual. Hay dos estrategias posibles:

**Estrategia A (recomendada para MVP): Calcular en query**
No agregar campos a PromoTareaPromotor. El backend calcula estos valores en la query agrupando registros por (TareaId, ProgramaPromotorId):
- `VecesCompletada` = COUNT de registros con EstadoTareaId en (Completada, Validada)
- `FechaPrimeraCompletada` = MIN(FechaCompletado) entre todos los registros
- `FechaUltimaCompletada` = MAX(FechaCompletado) entre todos los registros

Esta estrategia no requiere migracion de BD para PromoTareaPromotor y es consistente con el modelo de un registro por completado.

**Estrategia B: Agregar campos a la entidad**
Agregar `VecesCompletada (int)`, `FechaPrimeraCompletada (DateTime?)` y `FechaUltimaCompletada (DateTime?)` a PromoTareaPromotor. Estos se actualizarian en cada completado nuevo. Requiere migracion de EF Core y logica adicional de mantenimiento.

**Impacto en migraciones:**
- Si se elige Estrategia A: no se requiere migracion para PromoTareaPromotor; los calculos se hacen en la capa de aplicacion
- Si se elige Estrategia B: se requiere migracion aditiva con valores default (VecesCompletada = 1, fechas = FechaCompletado al crear)
- En ambos casos: PromotorWallet y PromotorWalletTransaccion ya existen en el dominio y no requieren cambios de esquema

**Nota sobre PromotorWalletTransaccion:** El campo `CampaniaPayoutId` existe en la entidad pero no aplica directamente a este flujo. Para las transacciones de recompensa por tarea, se usara `ReferenciaExterna` para almacenar el Id del PromoTareaPromotor validado como trazabilidad.

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity activo con JWT: el PromotorId y ArtistaId se extraen del claim del token en los handlers
- Entidad `PromoProgramaPromotor` con campos `EsAprobado` y `EsBloqueado` (agregados en US-CP-03; necesarios para validar acceso)
- Entidad `PromotorWallet` existente; el handler de validacion la consulta por (PromotorId, MonedaId) para acreditar
- Entidad `PromotorWalletTransaccion` existente; se crea un registro por cada validacion exitosa
- Datos seed de `Maestra_EstadoTareaPromo` cargados: id 1 = Pendiente, id 2 = Completada, id 3 = Validada, id 4 = Rechazada
- Operaciones de BD deben ejecutarse dentro de una transaccion explicita en el handler de validacion (acreditacion atomica)

### De otras features

- **cp-perfil-promotor (US-CP-01):** provee la entidad `Promotor` y el PromotorId necesario para vincular inscripciones y wallets
- **cp-programas-promocion (US-CP-02):** provee la entidad `PromoPrograma` con `EsActivo` y las entidades `PromoTarea` con las definiciones de tareas que esta feature consume; sin tareas definidas no hay que completar
- **cp-inscripcion-programa (US-CP-03):** provee la entidad `PromoProgramaPromotor` con los campos `EsAprobado` y `EsBloqueado`; esta feature es un prerequerito directo porque sin inscripcion aprobada el promotor no puede acceder a las tareas

---

## Requisitos No Funcionales

| ID | Requisito |
|----|-----------|
| RNF-01 | La acreditacion de recompensa al validar debe ser atomica: actualizar PromoTareaPromotor + crear PromotorWalletTransaccion + actualizar PromotorWallet en una sola transaccion de BD; si cualquier paso falla, todos los cambios deben revertirse |
| RNF-02 | El endpoint GET mis-tareas debe responder en menos de 500ms para programas con hasta 50 tareas activas y promotores con hasta 200 completados historicos |
| RNF-03 | El endpoint GET tareas-pendientes del artista debe soportar paginacion; el tamano de pagina por defecto es 10 items |
| RNF-04 | Las URLs de prueba enviadas por los promotores se almacenan tal como son ingresadas; el sistema no valida que la URL este activa en el momento del envio, solo que tenga formato URL valido |
| RNF-05 | Los completados y las validaciones generan registros inmutables; no se eliminan fisicamente en MVP para mantener trazabilidad de auditoria |

---

## Referencia

- User Story completa: [docs/product/US-CP-04-tareas-promocion.md](../../product/US-CP-04-tareas-promocion.md)
- Feature spec cp-inscripcion-programa: [docs/user-stories/cp-inscripcion-programa/feature-spec.md](../cp-inscripcion-programa/feature-spec.md)
- Feature spec cp-programas-promocion: [docs/user-stories/cp-programas-promocion/feature-spec.md](../cp-programas-promocion/feature-spec.md)
- Feature spec cp-perfil-promotor: [docs/user-stories/cp-perfil-promotor/feature-spec.md](../cp-perfil-promotor/feature-spec.md)
