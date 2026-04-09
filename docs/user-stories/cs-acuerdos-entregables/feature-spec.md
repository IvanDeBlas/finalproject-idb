# Feature: Acuerdos, Milestones y Entregables

> **ID:** cs-acuerdos-entregables
> **User Story:** US-CS-04
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature completa el ciclo de vida del modulo crowdsourcing gestionando todo lo que ocurre despues de que un artista selecciona un profesional: la formalizacion del acuerdo, la organizacion del trabajo en milestones, la entrega y revision de resultados, y el cierre del compromiso. Es la pieza central que convierte una propuesta aceptada en un proyecto de trabajo estructurado y rastreable.

El flujo comienza cuando el artista acepta una propuesta (operacion transaccional que crea el acuerdo, rechaza las demas propuestas, avanza el estado de la necesidad y abre un canal de comunicacion automatico). A partir de ahi, el artista puede definir milestones con importes parciales y fechas limite, el profesional puede subir entregables con URLs externas (Dropbox, Drive, WeTransfer), y el artista puede aprobar o rechazar cada entregable con comentarios. El ciclo culmina cuando el artista completa el acuerdo o cuando cualquiera de las dos partes lo cancela.

Esta feature es la continuacion directa de US-CS-03 (explorar propuestas) y es prerequisito para las valoraciones mutuas entre artista y profesional. Al centralizarse toda la interaccion en la Landing, no hay impacto en Admin. La operacion de aceptar propuesta es la mas compleja del modulo (6 efectos secundarios en una sola transaccion) y requiere especial atencion en la implementacion del backend.

---

## User Story

**Como** artista o profesional participante de una contratacion,
**Quiero** gestionar el ciclo de vida completo del acuerdo de trabajo: aceptar/rechazar propuestas, definir milestones, subir y revisar entregables, y completar o cancelar el acuerdo,
**Para** llevar un control estructurado del trabajo contratado desde la formalizacion hasta la entrega final.

---

## Flujo Principal

### 1. Aceptar Propuesta y Crear Acuerdo

1. Artista navega a `/crowdsourcing/necesidades/{id}` o a la lista de propuestas de su necesidad
2. Artista selecciona una propuesta en estado `Pendiente` y hace click en "Aceptar propuesta"
3. El sistema muestra un formulario-resumen del acuerdo a crear con los datos derivados de la propuesta:
   - `TituloInterno` (editable, default: titulo de la necesidad)
   - `FechaInicio` (editable, default: hoy)
   - `FechaFinPrevista` (editable, default: hoy + DiasEstimados de la propuesta)
   - `ImporteTotalPactado` (readonly, del precio propuesto)
   - Partes involucradas: nombre del artista y nombre del profesional
4. Artista puede ajustar TituloInterno, FechaInicio y FechaFinPrevista
5. Artista hace click en "Confirmar acuerdo"
6. El sistema ejecuta `AceptarPropuestaCommand` de forma transaccional:
   - Crea `AcuerdoCrowdsourcing` con estado `Activo`
   - Propuesta aceptada pasa a estado `Aceptada`
   - Demas propuestas `Pendientes` de la misma necesidad pasan a `Rechazada` con motivo "Otra propuesta fue aceptada"
   - Necesidad pasa a estado `En Progreso`
   - Se crea automaticamente una `ConversacionCrowdsourcing` vinculada al acuerdo
7. El sistema muestra toast: "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."
8. Artista es redirigido a `/crowdsourcing/acuerdos/{acuerdoId}`

### 2. Ver Detalle de Acuerdo

1. Participante (artista o profesional) navega a `/crowdsourcing/acuerdos/{id}`
2. El sistema ejecuta `GetAcuerdoByIdQuery` verificando que el usuario es participante del acuerdo
3. Si el usuario no es participante, retorna 403 Forbidden
4. La vista muestra todas las secciones del acuerdo:
   - Cabecera: titulo, badge de estado, nombres de las partes, importe total, moneda, fechas, campo `miRol`
   - Milestones: listado ordenado con titulo, importe, fecha limite, estado, barra de progreso visual con "Asignado: X de Y EUR (Z%)"
   - Entregables: agrupados por milestone (los sin milestone al final), con titulo, estado, URL, fecha
   - Conversacion: boton/link para ir al chat vinculado al acuerdo
   - Valoraciones: reviews (si completado) o CTA para valorar
   - Timeline: historial de actividad reciente del acuerdo
5. Las acciones disponibles se renderizan segun `miRol` y el estado del acuerdo

### 3. Definir y Gestionar Milestones

1. Artista hace click en "+ Agregar milestone" desde el detalle del acuerdo (acuerdo en estado `Activo`)
2. El sistema muestra formulario de milestone con los campos: Titulo, Descripcion, Importe parcial, Fecha limite
3. El formulario muestra en tiempo real el indicador: "Asignado: X de Y EUR (Z% del total)"
4. Si la suma de importes supera el ImporteTotalPactado, se muestra error inmediato
5. Artista envia el formulario y el sistema ejecuta `CreateMilestoneCommand`
6. El milestone se crea con `FechaCompletado = null` (estado pendiente)
7. El artista puede editar un milestone existente siempre que no este marcado como completado
8. El artista puede eliminar un milestone siempre que no tenga entregables asociados

### 4. Subir Entregable

1. Profesional hace click en "Subir entregable" desde el detalle del acuerdo (acuerdo en estado `Activo`)
2. El sistema muestra formulario con: Titulo, Descripcion, URL del recurso, Milestone asociado (select opcional)
3. Profesional completa el formulario y envia
4. El sistema ejecuta `CreateEntregableCommand`, crea el entregable con estado `Entregado`
5. El sistema muestra toast: "Entregable subido correctamente. El artista sera notificado."
6. El entregable aparece inmediatamente en la vista de detalle, agrupado bajo el milestone seleccionado

### 5. Aprobar Entregable

1. Artista revisa un entregable en estado `Entregado` en el detalle del acuerdo
2. Artista hace click en "Aprobar"
3. El sistema muestra campo para comentario opcional (max 500 chars)
4. Artista confirma la aprobacion
5. El sistema ejecuta `AprobarEntregableCommand`, entregable pasa a estado `Aprobado` con `FechaAprobacion`
6. Si todos los entregables del milestone quedan aprobados, el sistema sugiere marcar el milestone como completado

### 6. Rechazar Entregable

1. Artista revisa un entregable en estado `Entregado` en el detalle del acuerdo
2. Artista hace click en "Rechazar"
3. El sistema solicita comentario obligatorio (min 10 chars) explicando que debe corregirse
4. El sistema ejecuta `RechazarEntregableCommand`, entregable pasa a estado `Rechazado`
5. El profesional puede subir una nueva version (nuevo entregable vinculado al mismo milestone)

### 7. Completar Acuerdo

1. Artista hace click en "Completar acuerdo" en el detalle del acuerdo (estado `Activo`)
2. El sistema muestra resumen: milestones completados, entregables aprobados, importe total
3. Si hay entregables pendientes de revision, el sistema muestra aviso (no bloqueante)
4. Artista confirma la accion
5. El sistema ejecuta `CompletarAcuerdoCommand`:
   - Acuerdo pasa a estado `Completado`
   - Se registra `FechaFinReal`
   - Necesidad pasa a estado `Cerrada`
   - Se habilitan las valoraciones para ambas partes
6. El sistema muestra toast: "Acuerdo completado. Puedes dejar una valoracion al profesional."

### 8. Cancelar Acuerdo

1. Participante (artista o profesional) hace click en "Cancelar acuerdo" en el detalle del acuerdo
2. El sistema muestra dialogo de advertencia: "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas."
3. Se solicita motivo obligatorio (min 20, max 1000 chars)
4. Participante confirma la accion
5. El sistema ejecuta `CancelarAcuerdoCommand`:
   - Acuerdo pasa a estado `Cancelado`
   - Se registra `FechaFinReal = ahora`
   - Se registra quien cancelo (`CanceladoPor`) y el motivo
   - Necesidad vuelve a estado `Abierta` (el artista puede buscar otro profesional)
   - Milestones y entregables se conservan como historial (no modificables)
6. El sistema muestra toast: "Acuerdo cancelado."

### 9. Rechazar Propuesta Individual

1. Artista hace click en "Rechazar" sobre una propuesta en estado `Pendiente`
2. El sistema solicita motivo opcional (max 500 chars)
3. Artista confirma
4. El sistema ejecuta `RechazarPropuestaCommand`, propuesta pasa a estado `Rechazada`
5. El profesional puede ver el nuevo estado pero NO el motivo de rechazo

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Artista intenta aceptar propuesta y ya hay acuerdo activo para esa necesidad | Error 400: "Ya existe un acuerdo activo para esta necesidad" |
| FA-02 | Profesional intenta subir entregable en acuerdo cancelado o completado | Error 400: "El acuerdo no esta activo" |
| FA-03 | Artista intenta eliminar milestone con entregables asociados | Error 400: "No se puede eliminar un milestone con entregables. Elimina primero los entregables." |
| FA-04 | Suma de importes de milestones supera el ImporteTotalPactado | Error en tiempo real en el formulario: "El importe asignado supera el total pactado" |
| FA-05 | Artista completa acuerdo sin milestones definidos | Permitido. Los milestones son opcionales. Se muestra resumen con 0 milestones. |
| FA-06 | Entregable rechazado: profesional sube nueva version | Nuevo entregable vinculado al mismo milestone. El anterior queda como historial en estado Rechazado. |
| FA-07 | Usuario no participante intenta acceder al detalle del acuerdo | Retorna 403 Forbidden. La pagina muestra mensaje de acceso denegado. |
| FA-08 | Artista intenta aprobar entregable que no esta en estado Entregado | Boton no visible en UI. Backend retorna 400 si se llama directamente. |
| FA-09 | Artista intenta editar milestone ya marcado como completado | Boton no visible en UI. Backend retorna 400 si se llama directamente. |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto | Metodo de Prueba |
|----|----------|----------|------------------|
| AC-CS04-1 | Al aceptar una propuesta, se crea un `AcuerdoCrowdsourcing` con estado `Activo` y con todos los datos derivados: NecesidadId, PropuestaId, ArtistaId, UserIdProveedor, ImporteTotalPactado, MonedaId, TituloInterno, FechaInicio, FechaFinPrevista | Backend + Landing | Aceptar propuesta. Verificar en BD que el acuerdo existe con todos los campos correctamente asignados. Verificar estado=Activo. |
| AC-CS04-2 | La propuesta aceptada pasa a estado `Aceptada` y las demas propuestas en estado `Pendiente` de la misma necesidad pasan automaticamente a `Rechazada` con motivo "Otra propuesta fue aceptada". Esta operacion es atomica (transaccional). | Backend | Crear necesidad con 3 propuestas Pendientes. Aceptar una. Verificar que la aceptada tiene estado=Aceptada y las otras dos tienen estado=Rechazada con el motivo correcto. Si falla en cualquier paso, verificar rollback completo. |
| AC-CS04-3 | La necesidad pasa a estado `En Progreso` y se crea automaticamente una `ConversacionCrowdsourcing` vinculada al acuerdo. El toast muestra: "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." El artista es redirigido a `/crowdsourcing/acuerdos/{id}`. | Backend + Landing | Verificar estado de la necesidad en BD. Verificar existencia de ConversacionCrowdsourcing con AcuerdoId correcto. Verificar toast con texto exacto. Verificar redirect a la URL del acuerdo. |
| AC-CS04-4 | Solo los dos participantes del acuerdo (artista y profesional) pueden acceder al detalle via GET `/api/crowdsourcing/acuerdos/{id}`. Cualquier otro usuario autenticado recibe 403 Forbidden. | Backend | Crear acuerdo entre usuario A y usuario B. Autenticarse como usuario C (tercero). GET `/acuerdos/{id}`. Verificar 403. |
| AC-CS04-5 | La vista de detalle renderiza todas las secciones: cabecera (titulo, estado, partes, importe, fechas), milestones con barra de progreso, entregables agrupados por milestone, boton de acceso a conversacion, y timeline de actividad reciente. | Landing | Crear acuerdo con milestones y entregables. Navegar al detalle. Verificar presencia de cada seccion. Verificar que los entregables se agrupan correctamente bajo su milestone. Verificar que entregables sin milestone aparecen al final. |
| AC-CS04-6 | Solo el artista participante puede crear milestones. La suma de importes parciales debe ser <= ImporteTotalPactado. El indicador "Asignado: X de Y EUR (Z%)" se actualiza en tiempo real al modificar el importe. Cada milestone se crea con FechaCompletado = null. | Backend + Landing | Crear milestone como artista: verificar. Intentar como profesional: verificar 403. Crear milestone que supera el total: verificar error en UI y backend. Verificar FechaCompletado=null en BD. |
| AC-CS04-7 | Solo el profesional participante puede subir entregables usando URL externa. Los entregables se crean con estado `Entregado`. El toast muestra: "Entregable subido correctamente. El artista sera notificado." | Backend + Landing | Subir entregable como profesional: verificar estado=Entregado en BD y toast. Intentar subir como artista: verificar 403. Subir con URL invalida: verificar error de validacion. |
| AC-CS04-8 | Solo el artista participante puede aprobar o rechazar entregables en estado `Entregado`. Al aprobar, el comentario es opcional (max 500 chars). Al rechazar, el comentario es obligatorio (min 10 chars). Se registra FechaAprobacion al aprobar. | Backend + Landing | Aprobar entregable sin comentario: verificar exito. Aprobar con comentario > 500 chars: verificar error. Rechazar sin comentario: verificar error de validacion. Rechazar con 9 chars: verificar error. Rechazar con 10+ chars: verificar exito. |
| AC-CS04-9 | Un entregable rechazado permite al profesional subir una nueva version (nuevo entregable). El entregable rechazado queda como historial en estado `Rechazado` y no se elimina. | Backend + Landing | Rechazar entregable. Subir nueva version como profesional. Verificar que existen dos entregables en BD: uno Rechazado y uno Entregado. |
| AC-CS04-10 | Al completar un acuerdo, se registra FechaFinReal con el timestamp actual, la necesidad pasa a estado `Cerrada`, se habilitan las valoraciones para ambas partes. El toast muestra: "Acuerdo completado. Puedes dejar una valoracion al profesional." | Backend + Landing | Completar acuerdo. Verificar estado=Completado en BD. Verificar FechaFinReal != null. Verificar necesidad.Estado=Cerrada. Verificar toast con texto exacto. |
| AC-CS04-11 | Al cancelar un acuerdo, se muestra dialogo con texto "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas.", se requiere motivo obligatorio (min 20, max 1000 chars). Tras confirmar: acuerdo pasa a `Cancelado`, se registra FechaFinReal y CanceladoPor, la necesidad vuelve a `Abierta`. | Backend + Landing | Cancelar sin motivo: verificar error. Cancelar con motivo < 20 chars: verificar error. Cancelar con motivo valido como artista: verificar acuerdo=Cancelado, necesidad=Abierta, CanceladoPor=userId del artista. Repetir como profesional. |
| AC-CS04-12 | Solo el artista propietario de la necesidad puede rechazar una propuesta individual en estado `Pendiente`. El motivo es opcional (max 500 chars). El profesional puede ver que su propuesta fue rechazada pero NO puede ver el motivo. | Backend + Landing | Rechazar propuesta. Verificar estado=Rechazada en BD. Autenticarse como el profesional. GET mis-propuestas: verificar que el campo MotivoRechazo no aparece en la respuesta. Intentar rechazar como profesional: verificar 403. |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar Commands CQRS: `AceptarPropuestaCommand` (transaccional, 6 efectos), `RechazarPropuestaCommand`, `CreateMilestoneCommand`, `UpdateMilestoneCommand`, `DeleteMilestoneCommand`, `CreateEntregableCommand`, `AprobarEntregableCommand`, `RechazarEntregableCommand`, `CompletarAcuerdoCommand`, `CancelarAcuerdoCommand`. Queries: `GetAcuerdoByIdQuery`. Validators para cada command. Nuevo `IAcuerdoCrowdsourcingService` con logica de negocio. Nuevo `IAcuerdoCrowdsourcingMilestoneService`. Nuevo `IAcuerdoCrowdsourcingEntregableService`. Nuevas migraciones para las 3 entidades nuevas. Logica de autorizacion por participante en cada endpoint. Calculo de `porcentajeParcial` y `importeAsignado` en tiempo real. Generacion del timeline desde cambios de estado y fechas de creacion. | ALTO |
| **Shared** | Definir TypeScript types para DTOs: `AcuerdoDetalleDto`, `MilestoneDto`, `EntregableDto`, `TimelineItemDto`, `AcuerdoResumenDto`. Tipos para requests: `AceptarPropuestaRequest`, `CreateMilestoneRequest`, `UpdateMilestoneRequest`, `CreateEntregableRequest`, `AprobarEntregableRequest`, `RechazarEntregableRequest`, `CancelarAcuerdoRequest`. Schemas Zod: `aceptarPropuestaSchema`, `createMilestoneSchema`, `createEntregableSchema`, `rechazarEntregableSchema`, `cancelarAcuerdoSchema`. Constants: `ESTADO_ACUERDO` (Activo, Completado, Cancelado), `ESTADO_ENTREGABLE` (Entregado, Aprobado, Rechazado), `BADGE_COLORS_ACUERDO`, `BADGE_COLORS_ENTREGABLE`. Nuevas `QUERY_KEYS` y `API_ROUTES` para los endpoints de esta feature. | MEDIO |
| **Landing** | Implementar pagina `AcuerdoDetallePage` en `/crowdsourcing/acuerdos/:id`. Componentes: `AcuerdoCabecera`, `MilestonesSection` (con barra de progreso), `MilestoneCard` (con lista de entregables), `EntregableItem` (con botones aprobar/rechazar segun rol), `AgregarMilestoneForm` (modal o drawer, con indicador tiempo real), `SubirEntregableForm` (modal), `AprobarEntregableDialog`, `RechazarEntregableDialog`, `CompletarAcuerdoDialog` (resumen + confirmacion), `CancelarAcuerdoDialog` (advertencia + motivo), `AcuerdoTimeline`, `ConversacionLink`. Hooks: `useAcuerdoDetalle`, `useCreateMilestone`, `useUpdateMilestone`, `useDeleteMilestone`, `useCreateEntregable`, `useAprobarEntregable`, `useRechazarEntregable`, `useCompletarAcuerdo`, `useCancelarAcuerdo`, `useAceptarPropuesta`, `useRechazarPropuesta`. Services: `acuerdo.service.ts`, `milestone.service.ts`, `entregable.service.ts`. Logica de renderizado condicional de acciones segun `miRol` y estado del acuerdo. | ALTO |
| **Admin** | No involucrado. Toda la interaccion de acuerdos, milestones y entregables ocurre en la Landing. | BAJO |

---

## Entidades Involucradas

### AcuerdoCrowdsourcing (nueva)

Entidad principal que formaliza el contrato de trabajo entre artista y profesional.

**Campos clave:**
- `Id` (Guid, PK)
- `NecesidadId` (Guid, FK -> NecesidadCrowdsourcing) - Necesidad de origen
- `PropuestaId` (Guid, FK -> PropuestaCrowdsourcing) - Propuesta aceptada
- `ArtistaId` (Guid, FK -> Artista) - Artista contratante
- `UserIdProveedor` (string, FK -> Identity.User) - Usuario del profesional
- `PerfilProfesionalId` (Guid, FK -> PerfilProfesional) - Perfil del profesional
- `ImporteTotalPactado` (decimal, NOT NULL, del PrecioPropuesto)
- `MonedaId` (int, FK -> MaestraMoneda)
- `EstadoAcuerdoId` (int, FK -> MaestraEstadoAcuerdo) - Activo/Completado/Cancelado
- `TituloInterno` (string, NOT NULL, max 200, editable al crear)
- `FechaInicio` (DateTime, editable al crear, default: hoy)
- `FechaFinPrevista` (DateTime, nullable, editable al crear, default: hoy + DiasEstimados)
- `FechaFinReal` (DateTime, nullable, se registra al completar o cancelar)
- `MotivoCancelacion` (string, nullable, min 20 max 1000, obligatorio al cancelar)
- `CanceladoPor` (string, nullable, UserId de quien cancelo)
- `FechaCreacion` (DateTime, NOT NULL)
- `FechaActualizacion` (DateTime, nullable)

**Relaciones:**
- `Necesidad` (navigation property -> NecesidadCrowdsourcing)
- `Propuesta` (navigation property -> PropuestaCrowdsourcing)
- `Milestones` (collection -> AcuerdoCrowdsourcingMilestone)
- `Entregables` (collection -> AcuerdoCrowdsourcingEntregable, sin milestone incluidos)
- `Conversacion` (navigation property -> ConversacionCrowdsourcing, nullable)
- `Valoraciones` (collection -> ValoracionCrowdsourcing)

**Campos calculados (no en BD):**
- `importeAsignado`: SUM(Milestones.ImporteParcial)
- `porcentajeAsignado`: importeAsignado / ImporteTotalPactado * 100
- `miRol`: "Artista" o "Profesional" segun el usuario autenticado

### AcuerdoCrowdsourcingMilestone (nueva)

Representa una etapa del trabajo con importe parcial asociado.

**Campos clave:**
- `Id` (Guid, PK)
- `AcuerdoId` (Guid, FK -> AcuerdoCrowdsourcing)
- `Titulo` (string, NOT NULL, min 3, max 200)
- `Descripcion` (string, nullable, max 1000)
- `Orden` (int, secuencial, editable con drag and drop)
- `ImporteParcial` (decimal, NOT NULL, > 0)
- `FechaLimite` (DateTime, nullable, >= FechaInicio del acuerdo)
- `FechaCompletado` (DateTime, nullable, null = pendiente, set = completado)
- `FechaCreacion` (DateTime, NOT NULL)

**Relaciones:**
- `Acuerdo` (navigation property -> AcuerdoCrowdsourcing)
- `Entregables` (collection -> AcuerdoCrowdsourcingEntregable)

**Campos calculados (no en BD):**
- `porcentajeParcial`: ImporteParcial / AcuerdoCrowdsourcing.ImporteTotalPactado * 100

**Reglas de negocio:**
- La suma de todos los ImporteParcial del acuerdo no puede superar ImporteTotalPactado
- No se puede editar si FechaCompletado != null
- No se puede eliminar si tiene entregables asociados

### AcuerdoCrowdsourcingEntregable (nueva)

Representa un resultado de trabajo entregado por el profesional.

**Campos clave:**
- `Id` (Guid, PK)
- `AcuerdoId` (Guid, FK -> AcuerdoCrowdsourcing)
- `MilestoneId` (Guid, nullable, FK -> AcuerdoCrowdsourcingMilestone) - Agrupacion opcional
- `Titulo` (string, NOT NULL, min 3, max 200)
- `Descripcion` (string, nullable, max 1000)
- `UrlRecurso` (string, nullable, URL valida) - MVP: solo URLs externas
- `EstadoEntregableId` (int, FK -> MaestraEstadoEntregable) - Entregado/Aprobado/Rechazado
- `ComentarioAprobacion` (string, nullable, max 500) - Opcional al aprobar
- `ComentarioRechazo` (string, nullable, min 10, obligatorio al rechazar)
- `FechaAprobacion` (DateTime, nullable, se registra al aprobar)
- `FechaCreacion` (DateTime, NOT NULL)
- `FechaActualizacion` (DateTime, nullable)

**Relaciones:**
- `Acuerdo` (navigation property -> AcuerdoCrowdsourcing)
- `Milestone` (navigation property -> AcuerdoCrowdsourcingMilestone, nullable)

**Ciclo de estados:**
- `Entregado` (estado inicial al subir) -> `Aprobado` (artista aprueba) o `Rechazado` (artista rechaza)
- `Rechazado` -> el profesional puede subir nuevo entregable al mismo milestone (nuevo registro, no mutacion)

### PropuestaCrowdsourcing (existente, modificada)

Entidad ya introducida en US-CS-03. En esta feature el artista puede:
- Aceptarla (PATCH /propuestas/{id}/aceptar) -> estado `Aceptada`
- Rechazarla individualmente (PATCH /propuestas/{id}/rechazar) -> estado `Rechazada`

**Campo adicionado en esta feature:**
- `AcuerdoId` (Guid, nullable, FK -> AcuerdoCrowdsourcing) - Navegacion hacia el acuerdo creado

### NecesidadCrowdsourcing (existente, transiciones de estado)

Su estado cambia como efecto secundario de las operaciones de esta feature:
- Al aceptar propuesta: `Abierta` -> `En Progreso`
- Al completar acuerdo: `En Progreso` -> `Cerrada`
- Al cancelar acuerdo: `En Progreso` -> `Abierta`

---

## API Endpoints

### POST /api/crowdsourcing/propuestas/{id}/aceptar

Aceptar propuesta y crear acuerdo. Operacion transaccional con 6 efectos secundarios.

**Auth:** Artista (propietario de la necesidad relacionada a la propuesta)

**Request:**
```json
{
  "tituloInterno": "Mezcla EP Los Rockeros",
  "fechaInicio": "2026-03-01",
  "fechaFinPrevista": "2026-03-15"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "acuerdoId": "guid",
    "tituloInterno": "Mezcla EP Los Rockeros",
    "estadoAcuerdoNombre": "Activo",
    "importeTotalPactado": 450.00,
    "monedaNombre": "EUR",
    "conversacionId": "guid",
    "propuestasRechazadas": 2
  },
  "messages": [
    { "message": "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional.", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Propuesta no esta en estado Pendiente, o ya existe un acuerdo activo para esa necesidad
- `403 Forbidden` - El usuario autenticado no es el propietario de la necesidad
- `404 Not Found` - Propuesta no existe

---

### PATCH /api/crowdsourcing/propuestas/{id}/rechazar

Rechazar propuesta individualmente.

**Auth:** Artista (propietario de la necesidad)

**Request:**
```json
{
  "motivo": "El presupuesto no se ajusta a nuestras posibilidades"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoPropuestaNombre": "Rechazada"
  },
  "messages": [
    { "message": "Propuesta rechazada", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Propuesta no esta en estado Pendiente
- `403 Forbidden` - El usuario no es el propietario de la necesidad
- `404 Not Found` - Propuesta no existe

---

### GET /api/crowdsourcing/acuerdos/{id}

Detalle del acuerdo con milestones, entregables anidados y timeline.

**Auth:** Participante del acuerdo (artista o profesional)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "tituloInterno": "Mezcla EP Los Rockeros",
    "estadoAcuerdoId": 1,
    "estadoAcuerdoNombre": "Activo",
    "importeTotalPactado": 450.00,
    "monedaNombre": "EUR",
    "fechaInicio": "2026-03-01",
    "fechaFinPrevista": "2026-03-15",
    "fechaFinReal": null,
    "artista": {
      "id": "guid",
      "nombreArtistico": "Los Rockeros"
    },
    "profesional": {
      "userId": "guid",
      "perfilProfesionalId": "guid",
      "nombre": "Studio Mix Pro"
    },
    "necesidad": {
      "id": "guid",
      "titulo": "Mezcla de pistas para EP"
    },
    "conversacionId": "guid",
    "importeAsignado": 270.00,
    "porcentajeAsignado": 60,
    "miRol": "Artista",
    "milestones": [
      {
        "id": "guid",
        "titulo": "Mezcla de pistas 1-3",
        "descripcion": "Mezcla de las primeras 3 canciones",
        "orden": 1,
        "importeParcial": 270.00,
        "porcentajeParcial": 60,
        "fechaLimite": "2026-03-08",
        "fechaCompletado": null,
        "entregables": [
          {
            "id": "guid",
            "titulo": "Mezcla cancion 1 - v1",
            "descripcion": "Primera version de la mezcla",
            "urlRecurso": "https://drive.google.com/...",
            "estadoEntregableId": 1,
            "estadoEntregableNombre": "Entregado",
            "comentarioAprobacion": null,
            "comentarioRechazo": null,
            "fechaAprobacion": null,
            "fechaCreacion": "2026-03-05T14:00:00Z"
          }
        ]
      }
    ],
    "entregablesSinMilestone": [],
    "timeline": [
      {
        "accion": "Acuerdo creado",
        "fecha": "2026-03-01T10:00:00Z",
        "actor": "Los Rockeros"
      },
      {
        "accion": "Milestone agregado: Mezcla de pistas 1-3",
        "fecha": "2026-03-02T09:00:00Z",
        "actor": "Los Rockeros"
      }
    ]
  },
  "messages": []
}
```

**Errores:**
- `403 Forbidden` - El usuario no es participante del acuerdo
- `404 Not Found` - Acuerdo no existe

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones

Crear milestone. Solo artista participante, acuerdo en estado Activo.

**Auth:** Artista (participante del acuerdo)

**Request:**
```json
{
  "titulo": "Mezcla de pistas 1-3",
  "descripcion": "Mezcla de las primeras 3 canciones del EP",
  "importeParcial": 270.00,
  "fechaLimite": "2026-03-08"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla de pistas 1-3",
    "orden": 1,
    "importeParcial": 270.00,
    "porcentajeParcial": 60,
    "importeAsignadoTotal": 270.00
  },
  "messages": [
    { "message": "Milestone creado", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Validacion fallida (titulo, importe) o suma de importes supera el total pactado
- `403 Forbidden` - No es el artista participante, o el acuerdo no esta Activo
- `404 Not Found` - Acuerdo no existe

---

### PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

Editar milestone. Solo si el milestone no esta completado.

**Auth:** Artista (participante del acuerdo)

**Request:** (mismos campos que POST)

**Response 200 OK:** (datos actualizados del milestone)

**Errores:**
- `400 Bad Request` - Milestone ya esta completado o suma de importes supera el total
- `403 Forbidden` - No es el artista participante
- `404 Not Found` - Milestone no existe o no pertenece al acuerdo

---

### DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

Eliminar milestone. Solo si no tiene entregables asociados.

**Auth:** Artista (participante del acuerdo)

**Response 204 No Content**

**Errores:**
- `400 Bad Request` - Milestone tiene entregables asociados o ya esta completado
- `403 Forbidden` - No es el artista participante
- `404 Not Found` - Milestone no existe o no pertenece al acuerdo

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/entregables

Subir entregable. Solo profesional participante, acuerdo en estado Activo.

**Auth:** Profesional (participante del acuerdo)

**Request:**
```json
{
  "titulo": "Mezcla cancion 1 - v1",
  "descripcion": "Primera version de la mezcla de la cancion 1",
  "urlRecurso": "https://drive.google.com/file/xyz",
  "milestoneId": "guid"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla cancion 1 - v1",
    "estadoEntregableNombre": "Entregado",
    "fechaCreacion": "2026-03-05T14:00:00Z"
  },
  "messages": [
    { "message": "Entregable subido correctamente. El artista sera notificado.", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Validacion fallida (titulo, URL invalida) o acuerdo no activo
- `403 Forbidden` - No es el profesional participante
- `404 Not Found` - Acuerdo no existe, o MilestoneId proporcionado no pertenece al acuerdo

---

### PATCH /api/crowdsourcing/entregables/{id}/aprobar

Aprobar entregable en estado Entregado.

**Auth:** Artista (participante del acuerdo)

**Request:**
```json
{
  "comentario": "Excelente mezcla, me encanta el resultado"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoEntregableNombre": "Aprobado",
    "fechaAprobacion": "2026-03-06T10:00:00Z",
    "todosAprobadosEnMilestone": true
  },
  "messages": [
    { "message": "Entregable aprobado", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Entregable no esta en estado Entregado o comentario supera 500 chars
- `403 Forbidden` - No es el artista participante del acuerdo
- `404 Not Found` - Entregable no existe

---

### PATCH /api/crowdsourcing/entregables/{id}/rechazar

Rechazar entregable en estado Entregado.

**Auth:** Artista (participante del acuerdo)

**Request:**
```json
{
  "comentario": "La voz esta demasiado baja en el coro, necesita mas presencia."
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoEntregableNombre": "Rechazado"
  },
  "messages": [
    { "message": "Entregable rechazado. El profesional sera notificado.", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Entregable no esta en estado Entregado, o comentario vacio/menor a 10 chars
- `403 Forbidden` - No es el artista participante del acuerdo
- `404 Not Found` - Entregable no existe

---

### PATCH /api/crowdsourcing/acuerdos/{id}/completar

Completar acuerdo. Solo artista participante, acuerdo en estado Activo.

**Auth:** Artista (participante del acuerdo)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoAcuerdoNombre": "Completado",
    "fechaFinReal": "2026-03-14T16:00:00Z"
  },
  "messages": [
    { "message": "Acuerdo completado. Puedes dejar una valoracion al profesional.", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Acuerdo no esta en estado Activo
- `403 Forbidden` - No es el artista participante
- `404 Not Found` - Acuerdo no existe

---

### PATCH /api/crowdsourcing/acuerdos/{id}/cancelar

Cancelar acuerdo. Tanto artista como profesional pueden cancelar.

**Auth:** Participante del acuerdo (artista o profesional)

**Request:**
```json
{
  "motivo": "No puedo continuar por motivos personales. Lamento las molestias causadas."
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoAcuerdoNombre": "Cancelado",
    "fechaFinReal": "2026-03-10T12:00:00Z",
    "necesidadEstadoNombre": "Abierta"
  },
  "messages": [
    { "message": "Acuerdo cancelado", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Acuerdo no esta en estado Activo o motivo no cumple validaciones
- `403 Forbidden` - No es participante del acuerdo
- `404 Not Found` - Acuerdo no existe

---

## Validaciones

### AceptarPropuestaValidator

```csharp
RuleFor(x => x.TituloInterno)
    .MaximumLength(200)
    .When(x => !string.IsNullOrEmpty(x.TituloInterno))
    .WithMessage("El titulo interno no puede superar los 200 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

// Business rule: propuesta debe estar en estado Pendiente
RuleFor(x => x.PropuestaId)
    .MustAsync(async (id, ct) => {
        var propuesta = await _propuestaService.GetByIdAsync(id, ct);
        return propuesta != null && propuesta.EstadoPropuestaId == EstadoPropuesta.Pendiente;
    })
    .WithMessage("La propuesta no existe o no esta en estado Pendiente")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_InvalidState);

// Business rule: no debe existir acuerdo activo para la necesidad
RuleFor(x => x)
    .MustAsync(async (command, ct) => {
        var tieneAcuerdo = await _acuerdoService.ExisteAcuerdoActivoParaNecesidadAsync(command.NecesidadId, ct);
        return !tieneAcuerdo;
    })
    .WithMessage("Ya existe un acuerdo activo para esta necesidad")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_ArtistaAlreadyExists);
```

### CreateMilestoneValidator

```csharp
RuleFor(x => x.Titulo)
    .NotEmpty()
    .WithMessage("El titulo es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MinimumLength(3)
    .WithMessage("Minimo 3 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
    .MaximumLength(200)
    .WithMessage("Maximo 200 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.Descripcion)
    .MaximumLength(1000)
    .When(x => !string.IsNullOrEmpty(x.Descripcion))
    .WithMessage("Maximo 1000 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.ImporteParcial)
    .GreaterThan(0)
    .WithMessage("El importe parcial debe ser mayor a 0")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);

// Suma de importes no puede superar el total pactado
RuleFor(x => x)
    .MustAsync(async (command, ct) => {
        var importeAsignado = await _milestoneService.GetImporteAsignadoAsync(command.AcuerdoId, ct);
        var acuerdo = await _acuerdoService.GetByIdAsync(command.AcuerdoId, ct);
        return (importeAsignado + command.ImporteParcial) <= acuerdo!.ImporteTotalPactado;
    })
    .WithMessage("La suma de importes parciales supera el importe total pactado")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_InvalidState);
```

### CreateEntregableValidator

```csharp
RuleFor(x => x.Titulo)
    .NotEmpty()
    .WithMessage("El titulo es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MinimumLength(3)
    .WithMessage("Minimo 3 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
    .MaximumLength(200)
    .WithMessage("Maximo 200 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.Descripcion)
    .MaximumLength(1000)
    .When(x => !string.IsNullOrEmpty(x.Descripcion))
    .WithMessage("Maximo 1000 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

RuleFor(x => x.UrlRecurso)
    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
    .When(x => !string.IsNullOrEmpty(x.UrlRecurso))
    .WithMessage("Debe ser una URL valida (ej: Dropbox, Drive, WeTransfer)")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
```

### RechazarEntregableValidator

```csharp
RuleFor(x => x.Comentario)
    .NotEmpty()
    .WithMessage("El comentario es obligatorio al rechazar un entregable")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MinimumLength(10)
    .WithMessage("Minimo 10 caracteres explicando que debe corregirse")
    .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
    .MaximumLength(500)
    .WithMessage("Maximo 500 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
```

### AprobarEntregableValidator

```csharp
RuleFor(x => x.Comentario)
    .MaximumLength(500)
    .When(x => !string.IsNullOrEmpty(x.Comentario))
    .WithMessage("El comentario no puede superar los 500 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
```

### CancelarAcuerdoValidator

```csharp
RuleFor(x => x.Motivo)
    .NotEmpty()
    .WithMessage("El motivo de cancelacion es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .MinimumLength(20)
    .WithMessage("Minimo 20 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
    .MaximumLength(1000)
    .WithMessage("Maximo 1000 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
```

---

## Requisitos No Funcionales

### Performance
- El GET de detalle del acuerdo debe retornar en < 500ms incluyendo milestones, entregables y timeline
- La query de detalle debe cargar milestones con sus entregables en una sola consulta (evitar N+1)
- El calculo de `importeAsignado` y `porcentajeAsignado` se hace a nivel de query SQL (SUM), no en memoria
- El indicador de importe en tiempo real del formulario de milestone se calcula en frontend (no request al backend)
- Usar `IRequestCacheService` para compartir datos entre validator y handler en la misma request (evitar queries duplicados de GetAcuerdoById)

### Seguridad
- Validar en backend que el usuario es participante del acuerdo en TODOS los endpoints de milestones, entregables, completar y cancelar (no confiar en frontend)
- Validar que el rol correcto ejecuta cada accion: artista para milestones/aprobar/completar, profesional para subir entregables
- El campo `miRol` debe calcularse en backend (no aceptar del cliente)
- El motivo de rechazo de propuesta no debe exponerse en el endpoint de mis-propuestas del profesional
- La operacion `AceptarPropuetaCommand` debe ejecutarse en una unica transaccion de base de datos

### UX
- Barra de progreso de milestones visible y actualizada en tiempo real en el detalle del acuerdo
- Indicador "Asignado: X de Y EUR (Z%)" en el formulario de milestone con actualizacion al escribir
- Dialogo de confirmacion antes de cancelar con texto de advertencia explicito
- Aviso (no bloqueante) al completar si hay entregables pendientes de revision
- Toast en todas las acciones: aceptar propuesta, subir entregable, aprobar, rechazar, completar, cancelar
- Acciones renderizadas de forma condicional segun `miRol` y estado del acuerdo (no mostrar botones que no corresponden)
- Sugerencia de marcar milestone como completado cuando todos sus entregables estan aprobados

### Mantenibilidad
- Maestras de estado en BD: `MaestraEstadoAcuerdo` (Activo=1, Completado=2, Cancelado=3) y `MaestraEstadoEntregable` (Entregado=1, Aprobado=2, Rechazado=3)
- Cada command en su propio archivo con Handler incluido (patron CQRS del proyecto)
- Timeline generado desde campos `FechaCreacion` y `FechaActualizacion` de las entidades (no tabla separada en MVP)
- Logs de auditoria en todos los handlers (quien ejecuto, que acuerdo, timestamp)

---

## Dependencias

### Tecnicas (dentro del proyecto)
- **Modulo Crowdsourcing**: `NecesidadCrowdsourcing` (transiciones de estado), `PropuestaCrowdsourcing` (aceptar/rechazar, introducida en US-CS-03)
- **Modulo UserAccess**: Entidad `Artista` (validar propietario), `PerfilProfesional` (datos del profesional en el detalle)
- **Nuevas maestras**: `MaestraEstadoAcuerdo` (Activo, Completado, Cancelado), `MaestraEstadoEntregable` (Entregado, Aprobado, Rechazado)
- **ConversacionCrowdsourcing**: Entidad creada automaticamente al aceptar propuesta. Debe existir el modelo y la tabla antes de esta US.
- **Maestras existentes**: `MaestraMoneda` (para ImporteTotalPactado)
- **Shared**: Types, schemas Zod, constants para estados acuerdo y entregable, QUERY_KEYS
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType`, `IRequestCacheService`

### De otras User Stories
- **US-CS-03** (Explorar Necesidades y Enviar Propuestas): Prerequisito directo. Sin propuestas en estado `Pendiente` no hay nada que aceptar. La entidad `PropuestaCrowdsourcing` y el endpoint `GET /mis-propuestas` con el enlace "Ver acuerdo" se introdujeron en US-CS-03.
- **US-CS-02** (Gestionar Necesidades): Prerequisito de prerequisito. La necesidad debe existir y estar en estado `Abierta` para que existan propuestas.

### Dependencias futuras (que esta US habilita)
- **Valoraciones (US-CS-05)**: El completado del acuerdo habilita las valoraciones mutuas. El campo `acuerdoId` en la valoracion trazabiliza el origen.

---

## Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Operacion de aceptar propuesta falla a mitad (estado inconsistente entre propuesta, necesidad y acuerdo) | Media | Alto | Implementar con transaccion de BD explicita en el handler. Si falla cualquier paso, rollback completo. Unit tests con mock de excepcion en cada paso. |
| Suma de importes de milestones supera el total por condicion de carrera (dos creates simultaneos) | Baja | Medio | Validar en validator con query actual a BD. Considerar optimistic concurrency: si la suma supera tras insertar, hacer rollback. Constraint a nivel de BD si es posible. |
| Profesional sube entregable a acuerdo que acaba de cancelarse (ventana de tiempo) | Baja | Bajo | Validar estado del acuerdo en el handler de `CreateEntregableCommand` (no solo en validator). Retornar 400 si estado != Activo. |
| Artista completa acuerdo accidentalmente sin revisar entregables | Media | Medio | Mostrar aviso en el dialogo de confirmacion listando cuantos entregables quedan pendientes. La accion no es bloqueante pero el aviso informa al usuario. |
| Inconsistencia en el estado de la necesidad si el handler de cancelar falla despues de cambiar el acuerdo | Baja | Alto | Toda la operacion de cancelar en una transaccion: actualizar acuerdo + actualizar necesidad de forma atomica. |
| Timeline con muchos items degrada la performance del GET de detalle | Baja | Bajo | MVP: construir el timeline desde fechas de creacion/actualizacion de las entidades existentes (sin tabla separada). Limitar a los 20 eventos mas recientes. |
| Profesional no ve los cambios del artista hasta recargar la pagina (sin notificaciones push) | Alta | Bajo | Documentado como limitacion del MVP. Los participantes ven los cambios al acceder al detalle. Priorizar para post-MVP con WebSockets o polling. |

---

## Notas de Implementacion

### Backend

- Crear Commands/Queries en `Modules/Crowdsourcing/Crowdsourcing.Application/Features/Acuerdos/`
  - Commands: `AceptarPropuestaCommand.cs`, `RechazarPropuestaCommand.cs`, `CreateMilestoneCommand.cs`, `UpdateMilestoneCommand.cs`, `DeleteMilestoneCommand.cs`, `CreateEntregableCommand.cs`, `AprobarEntregableCommand.cs`, `RechazarEntregableCommand.cs`, `CompletarAcuerdoCommand.cs`, `CancelarAcuerdoCommand.cs`
  - Queries: `GetAcuerdoByIdQuery.cs`
  - Validators: un archivo por cada Command
- Implementar Services:
  - `AcuerdoCrowdsourcingService : IAcuerdoCrowdsourcingService` con metodos: `GetByIdAsync`, `CreateAsync` (transaccional), `CompletarAsync`, `CancelarAsync`, `ExisteAcuerdoActivoParaNecesidadAsync`
  - `AcuerdoCrowdsourcingMilestoneService : IAcuerdoCrowdsourcingMilestoneService` con metodos: `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetImporteAsignadoAsync`
  - `AcuerdoCrowdsourcingEntregableService : IAcuerdoCrowdsourcingEntregableService` con metodos: `GetByIdAsync`, `CreateAsync`, `AprobarAsync`, `RechazarAsync`, `TodosAprobadosEnMilestoneAsync`
- `AceptarPropuestaCommand` es la operacion mas compleja. Usar `IDbContextTransaction` y ejecutar en orden:
  1. Verificar propuesta en estado Pendiente
  2. Verificar que no existe acuerdo activo para la necesidad
  3. Crear `AcuerdoCrowdsourcing`
  4. Actualizar propuesta a `Aceptada`
  5. Actualizar demas propuestas pendientes a `Rechazada`
  6. Actualizar necesidad a `En Progreso`
  7. Crear `ConversacionCrowdsourcing` vinculada al acuerdo
  8. Commit (o rollback si cualquier paso falla)
- AutoMapper Profiles: `AcuerdoProfile.cs`, `MilestoneProfile.cs`, `EntregableProfile.cs`
- Controller: `AcuerdosController.cs` con todos los endpoints. Ampliar `PropuestasController` con los endpoints de aceptar/rechazar.
- Nuevas migraciones: `AddAcuerdoCrowdsourcing`, `AddAcuerdoCrowdsourcingMilestone`, `AddAcuerdoCrowdsourcingEntregable`, `AddMaestrasEstadoAcuerdoEntregable`
- El campo `miRol` se calcula en `GetAcuerdoByIdQuery` comparando el UserId del JWT con `ArtistaId` y `UserIdProveedor`

### Frontend (Landing)

- Nueva pagina: `AcuerdoDetallePage.tsx` en ruta `/crowdsourcing/acuerdos/:id`
- Componentes reutilizables:
  - `AcuerdoCabecera.tsx` (estado badge, partes, importe, fechas)
  - `MilestonesSection.tsx` (listado + barra de progreso global)
  - `MilestoneCard.tsx` (titulo, importe, fecha, estado, lista de entregables, botones editar/eliminar)
  - `EntregableItem.tsx` (titulo, estado badge, URL, fecha, botones aprobar/rechazar segun `miRol`)
  - `AgregarMilestoneForm.tsx` (formulario en modal/drawer, indicador de importe en tiempo real)
  - `EditarMilestoneForm.tsx` (mismo formulario, pre-cargado con datos existentes)
  - `SubirEntregableForm.tsx` (formulario en modal, select de milestones del acuerdo)
  - `AprobarEntregableDialog.tsx` (dialog con campo comentario opcional)
  - `RechazarEntregableDialog.tsx` (dialog con campo comentario obligatorio)
  - `CompletarAcuerdoDialog.tsx` (resumen + aviso de entregables pendientes + confirmacion)
  - `CancelarAcuerdoDialog.tsx` (advertencia explicita + campo motivo + confirmacion)
  - `AcuerdoTimeline.tsx` (lista de eventos del timeline)
  - `ConversacionLink.tsx` (boton/link para ir al chat vinculado)
  - `EstadoAcuerdoBadge.tsx` (Activo=azul, Completado=verde, Cancelado=gris)
  - `EstadoEntregableBadge.tsx` (Entregado=amarillo, Aprobado=verde, Rechazado=rojo)
- Hooks:
  - `useAcuerdoDetalle.ts` (useQuery, invalida cache tras cualquier mutacion del acuerdo)
  - `useAceptarPropuesta.ts` (useMutation, redirige al detalle del acuerdo en exito)
  - `useRechazarPropuesta.ts` (useMutation)
  - `useCreateMilestone.ts`, `useUpdateMilestone.ts`, `useDeleteMilestone.ts` (useMutation, invalidan `useAcuerdoDetalle`)
  - `useCreateEntregable.ts` (useMutation, invalida `useAcuerdoDetalle`)
  - `useAprobarEntregable.ts`, `useRechazarEntregable.ts` (useMutation, invalidan `useAcuerdoDetalle`)
  - `useCompletarAcuerdo.ts`, `useCancelarAcuerdo.ts` (useMutation, invalidan `useAcuerdoDetalle`)
- Services:
  - `acuerdo.service.ts` (GET detalle, PATCH completar, PATCH cancelar)
  - `milestone.service.ts` (POST crear, PUT editar, DELETE eliminar)
  - `entregable.service.ts` (POST subir, PATCH aprobar, PATCH rechazar)
- La renderizacion condicional de acciones se basa en `miRol` y `estadoAcuerdoId` del response
- El indicador de importe del formulario de milestone usa `watch` de React Hook Form para calcular el nuevo total en tiempo real sin llamadas al backend

### Testing

- **Unit tests Backend:**
  - `AceptarPropuestaCommandHandlerTests` (transaccion exitosa, propuesta no pendiente, acuerdo ya existente, rollback ante fallo)
  - `CreateMilestoneCommandHandlerTests` (validacion de suma de importes, milestone con orden correcto)
  - `AprobarEntregableCommandHandlerTests`, `RechazarEntregableCommandHandlerTests` (validaciones de estado y comentario)
  - `CompletarAcuerdoCommandHandlerTests`, `CancelarAcuerdoCommandHandlerTests` (transicion de estados, registro de FechaFinReal)
  - Validators: uno por cada validator
- **Integration tests:**
  - Flujo completo: aceptar propuesta -> crear milestone -> subir entregable -> aprobar -> completar acuerdo
  - Flujo cancelacion: aceptar propuesta -> cancelar -> verificar necesidad=Abierta
  - Verificar que usuario tercero recibe 403 en todos los endpoints
- **E2E tests:**
  - Aceptar propuesta (toast + redirect al detalle)
  - Crear milestone con indicador de importe en tiempo real
  - Subir entregable y verlo en la vista del artista
  - Aprobar y rechazar entregable
  - Completar acuerdo con aviso de entregables pendientes
  - Cancelar acuerdo con motivo obligatorio

---

## Out of Scope (No incluido en esta feature)

- Notificaciones push o email cuando se sube un entregable, se aprueba/rechaza, o se cancela el acuerdo
- Upload de archivos directamente a la plataforma (MVP: solo URLs externas)
- Drag and drop para reordenar milestones (el campo Orden existe pero la UI de reordenamiento es post-MVP)
- Sistema de mensajeria en tiempo real (WebSockets) para ver actualizaciones sin recargar
- Pagos o transferencias monetarias vinculadas a milestones (la plataforma no gestiona pagos en el MVP)
- Disputas o sistema de arbitraje si las partes no llegan a acuerdo
- Historial de versiones de entregables (los rechazados se conservan pero no hay navegacion entre versiones)
- Valoraciones mutuas entre artista y profesional (habilitadas al completar pero gestionadas en US futura)
- Vista de lista de acuerdos del artista o del profesional (navegacion desde el detalle de la necesidad en US-CS-02 o desde mis-propuestas en US-CS-03)
- Moderacion de entregables por parte del Admin
- Exportacion o reporte del acuerdo en PDF

---

## Referencia

- User Story completa: [docs/product/US-CS-04-acuerdos-entregables.md](../../product/US-CS-04-acuerdos-entregables.md)
- US-CS-03 Explorar Propuestas: [docs/user-stories/cs-explorar-propuestas/feature-spec.md](../cs-explorar-propuestas/feature-spec.md)
- US-CS-02 Gestionar Necesidades: [docs/user-stories/cs-gestionar-necesidades/feature-spec.md](../cs-gestionar-necesidades/feature-spec.md)
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
- CLAUDE.md: Patron CQRS, reglas de validacion, ServiceResponse
