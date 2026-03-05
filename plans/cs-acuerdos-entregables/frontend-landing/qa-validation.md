# Validacion QA: Acuerdos, Milestones y Entregables (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Target:** src/web (Landing publica - Vite + React)
**Fuente de verdad:** `docs/user-stories/cs-acuerdos-entregables/feature-spec.md`
**Planes evaluados:**
- `plans/cs-acuerdos-entregables/shared/contracts-plan.md`
- `docs/user-stories/cs-acuerdos-entregables/contracts.md`

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Criterios de Aceptacion | 12 |
| Cubiertos completamente | 9 |
| Parcialmente cubiertos | 3 |
| No cubiertos | 0 |
| **Score de Cobertura** | **87.5%** |

> Calculo: (9 completos + 3 parciales * 0.5) / 12 = (9 + 1.5) / 12 = 10.5 / 12 = **87.5%**

**Estado: REQUIERE CAMBIOS**

Los tres gaps mayores identificados corresponden a comportamientos de UX que estan definidos en `feature-spec.md` pero que el `contracts-plan.md` no especifica como implementar en detalle: la sugerencia post-aprobacion de milestone, la agrupacion de entregables sin milestone en la vista de detalle, y la validacion de privacidad del motivo de rechazo en el endpoint de mis-propuestas del profesional. Estos deben resolverse antes de iniciar la implementacion de la Landing.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md (seccion "Criterios de Aceptacion")

| ID | Criterio (resumen) | Tipo |
|----|--------------------|------|
| AC-CS04-1 | Aceptar propuesta crea AcuerdoCrowdsourcing con estado Activo y todos los campos derivados correctamente asignados | Funcional |
| AC-CS04-2 | Operacion de aceptar propuesta es atomica: propuesta->Aceptada, otras propuestas->Rechazada con motivo, transaccion completa o rollback | Funcional (Transaccional) |
| AC-CS04-3 | Necesidad pasa a En Progreso, se crea ConversacionCrowdsourcing, toast con texto exacto, redirect a /crowdsourcing/acuerdos/{id} | Funcional (UX) |
| AC-CS04-4 | Solo los dos participantes pueden acceder al GET del detalle; terceros reciben 403 Forbidden | Seguridad / Autorizacion |
| AC-CS04-5 | Vista de detalle renderiza todas las secciones: cabecera, milestones con barra de progreso, entregables agrupados por milestone (sin milestone al final), link a conversacion, timeline | Funcional (UI) |
| AC-CS04-6 | Solo artista crea milestones; suma de importes <= total pactado; indicador en tiempo real; FechaCompletado=null al crear | Funcional + Validacion |
| AC-CS04-7 | Solo profesional sube entregables con URL externa; estado inicial Entregado; toast con texto exacto | Funcional + Validacion |
| AC-CS04-8 | Solo artista aprueba/rechaza; aprobar: comentario opcional max 500; rechazar: comentario obligatorio min 10; se registra FechaAprobacion | Funcional + Validacion |
| AC-CS04-9 | Entregable rechazado permite nueva version del profesional; el rechazado queda como historial (no se elimina) | Funcional |
| AC-CS04-10 | Completar acuerdo: FechaFinReal registrada, necesidad->Cerrada, valoraciones habilitadas, toast con texto exacto | Funcional (UX) |
| AC-CS04-11 | Cancelar acuerdo: dialogo con texto exacto de advertencia, motivo obligatorio min 20 max 1000, acuerdo->Cancelado, FechaFinReal, CanceladoPor, necesidad->Abierta | Funcional (UX) + Validacion |
| AC-CS04-12 | Solo artista rechaza propuesta individual; motivo opcional max 500; profesional ve el estado Rechazada pero NO el motivo | Seguridad + Privacidad |

---

## 3. Matriz de Trazabilidad

### 3.1 Criterios Funcionales Principales

| ID | Criterio (resumen) | Endpoint(s) cubiertos | Componentes UI cubiertos | Tests planificados | Estado |
|----|--------------------|-----------------------|--------------------------|--------------------|--------|
| AC-CS04-1 | Creacion de acuerdo con todos los campos | `POST /propuestas/{id}/aceptar` (contracts.md) - request/response con todos los campos: NecesidadId, PropuestaId, ArtistaId, UserIdProveedor, ImporteTotalPactado, MonedaId, TituloInterno, FechaInicio, FechaFinPrevista documentados en DTOs | `AceptarPropuestaRequest`, `AceptarPropuestaResult` (contracts-plan.md §2.2, §2.3), `aceptarPropuestaSchema` (§3.1) | `AceptarPropuestaCommandHandlerTests` (feature-spec.md "Testing") | CUBIERTO |
| AC-CS04-2 | Transaccion atomica de 6 efectos | `POST /propuestas/{id}/aceptar` - respuesta incluye `propuestasRechazadas: number`; errores 400 con rollback documentados | No aplica directamente a UI - es logica backend | `AceptarPropuestaCommandHandlerTests` (rollback ante fallo) - mencionado en feature-spec.md | CUBIERTO |
| AC-CS04-3 | Necesidad->En Progreso, Conversacion creada, toast exacto, redirect | `POST /propuestas/{id}/aceptar` - respuesta incluye `conversacionId`; mensaje exacto en response `messages[0]` | `useAceptarPropuesta` hook - invalida propuestas.mis, navega al detalle (contracts-plan.md §8.3 nota); toast desde `messages[0].message` (contracts.md notas de implementacion frontend §4 punto 4) | E2E: "Aceptar propuesta (toast + redirect al detalle)" (feature-spec.md) | CUBIERTO |
| AC-CS04-4 | Acceso restringido a participantes; 403 para terceros | `GET /acuerdos/{id}` - error 403 documentado (contracts.md); autorizacion por `ArtistaId` o `UserIdProveedor` vs `UserId` del token | Ruta `/crowdsourcing/acuerdos/:id` protegida con `Bearer JWT` / redirect a `/auth/login` (contracts.md "Proteccion de Rutas Frontend") | Test integracion: "Verificar que usuario tercero recibe 403 en todos los endpoints" (feature-spec.md) | CUBIERTO |
| AC-CS04-5 | Vista de detalle con todas las secciones y agrupacion de entregables | `GET /acuerdos/{id}` - response incluye `milestones[].entregables[]` y campo `entregablesSinMilestone` mencionado en feature-spec.md; **GAP: `entregablesSinMilestone` no aparece en el response del contracts.md ni en los types del contracts-plan.md** | `AcuerdoDetailPage`, `AcuerdoHeader`, `MilestonesSection`, `MilestoneCard`, `EntregablesSection`, `AcuerdoTimeline`, `ImporteAsignadoBar`, `ConversacionLink` - todos listados en contracts-plan.md §2 componentes | E2E: "Crear acuerdo con milestones y entregables. Navegar al detalle." (feature-spec.md AC-CS04-5) | PARCIAL |
| AC-CS04-6 | Milestones: solo artista, suma <= total, indicador tiempo real, FechaCompletado=null | `POST /milestones` (403 para profesional), `PUT /milestones/{id}` (400 milestone completado), `DELETE /milestones/{id}`; validaciones `4009`, `4011`, `4012` documentadas | `MilestoneFormDialog` con `createMilestoneSchema` (importeParcial > 0, suma en tiempo real usando `watch` de RHF); `useCreateMilestone`, `useUpdateMilestone`, `useDeleteMilestone` hooks (contracts-plan.md §8.3) | `CreateMilestoneCommandHandlerTests`, `UpdateMilestone/DeleteMilestone` validators; E2E: "Crear milestone con indicador de importe en tiempo real" | CUBIERTO |
| AC-CS04-7 | Profesional sube entregables; estado Entregado; toast exacto | `POST /entregables` (403 para artista, 400 URL invalida); response con `estadoEntregableNombre: 'Entregado'`; mensaje exacto en response | `SubirEntregableDialog` con `createEntregableSchema` (url validation, milestoneId opcional); `useCreateEntregable` hook invalidando `acuerdos.byId` | `CreateEntregableCommandHandlerTests`; E2E: "Subir entregable y verlo en la vista del artista" | CUBIERTO |
| AC-CS04-8 | Aprobar/rechazar entregable: validaciones de comentario y FechaAprobacion | `PATCH /entregables/{id}/aprobar` (comentario max 500, registra `fechaAprobacion`); `PATCH /entregables/{id}/rechazar` (comentario min 10 max 500 obligatorio); errores 400 documentados | `AprobarEntregableDialog` con `aprobarEntregableSchema`; `RechazarEntregableDialog` con `rechazarEntregableSchema`; hooks `useAprobarEntregable`, `useRechazarEntregable`; `EntregableItem` con botones condicionales por `miRol` | `AprobarEntregableCommandHandlerTests`, `RechazarEntregableCommandHandlerTests`; E2E: "Aprobar y rechazar entregable" | CUBIERTO |
| AC-CS04-9 | Entregable rechazado: nueva version posible, historial conservado | `POST /entregables` en acuerdo activo (profesional puede subir nuevo); entregable rechazado persiste con `estadoEntregableId: 3`; no hay DELETE de entregables en API | `EntregableItem` muestra entregables en estado Rechazado con badge rojo; profesional ve boton "Subir entregable" disponible si acuerdo Activo; **GAP: no esta documentado en contracts-plan.md como se muestra visualmente el historial de versiones rechazadas vinculadas al mismo milestone** | `RechazarEntregableCommandHandlerTests`; test integracion: "Rechazar entregable. Subir nueva version." (feature-spec.md AC-CS04-9) | PARCIAL |
| AC-CS04-10 | Completar acuerdo: FechaFinReal, necesidad->Cerrada, toast exacto | `PATCH /acuerdos/{id}/completar`; response con `fechaFinReal`; mensaje exacto en response `messages[0]`; necesidad->Cerrada verificable en BD | `CompletarAcuerdoDialog` con aviso de entregables pendientes (contracts.md notas frontend §4 tercer punto); `useCompletarAcuerdo` hook | `CompletarAcuerdoCommandHandlerTests`; E2E: "Completar acuerdo con aviso de entregables pendientes" | CUBIERTO |
| AC-CS04-11 | Cancelar acuerdo: dialogo con texto exacto, motivo min 20, CanceladoPor registrado | `PATCH /acuerdos/{id}/cancelar`; request con `motivo`, response con `necesidadEstadoNombre: 'Abierta'`; `cancelarAcuerdoSchema` (min 20, max 1000); texto de advertencia en dialogo documentado en contracts.md §4 nota | `CancelarAcuerdoDialog` con texto de advertencia exacto "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas."; `useCancelarAcuerdo` hook | `CancelarAcuerdoCommandHandlerTests`; E2E: "Cancelar acuerdo con motivo obligatorio" | CUBIERTO |
| AC-CS04-12 | Solo artista rechaza propuesta; motivo no visible para el profesional | `PATCH /propuestas/{id}/rechazar` (403 para profesional); `rechazarPropuestaSchema` (motivo opcional max 500); contracts.md especifica que el campo `MotivoRechazo` no debe exponerse en el endpoint de mis-propuestas; **GAP: el contracts-plan.md no documenta explicitamente como se oculta el motivo en la respuesta del endpoint `GET /propuestas/mis-propuestas` desde la perspectiva del frontend** | `RechazarPropuestaRequest`, `RechazarPropuestaResult` (solo devuelve `estadoPropuestaNombre`); `useRechazarPropuesta` hook | Test integracion: "Autenticarse como el profesional. GET mis-propuestas: verificar que MotivoRechazo no aparece" (feature-spec.md AC-CS04-12) | PARCIAL |

**Leyenda:**
- CUBIERTO: El criterio esta completamente especificado en los planes (contratos, tipos, componentes y tests)
- PARCIAL: El criterio esta parcialmente cubierto; falta especificacion en algun aspecto
- NO CUBIERTO: El criterio no tiene ninguna mencion en los planes

---

### 3.2 Requisitos No Funcionales

| ID | Criterio (fuente: feature-spec.md seccion NFR) | Cobertura en planes | Estado |
|----|------------------------------------------------|---------------------|--------|
| NFR-01 | GET detalle del acuerdo < 500ms incluyendo milestones, entregables y timeline | contracts-plan.md §8.3 nota: "Usar `AsNoTracking()`; Include(Milestones).ThenInclude(Entregables)"; feature-spec.md: "evitar N+1" | CUBIERTO |
| NFR-02 | Calculo de importeAsignado via SUM en SQL, no en memoria | contracts-plan.md §8.3 punto 9: "Calcular ImporteAsignado, PorcentajeAsignado y PorcentajeParcial en la proyeccion" | CUBIERTO |
| NFR-03 | Indicador de importe en tiempo real en formulario de milestone sin llamadas al backend | contracts-plan.md §8.3: "barra de progreso usando `importeAsignadoTotal` del servidor (invalidar query tras cada mutacion)"; contracts.md notas frontend: "usa `watch` de React Hook Form" | CUBIERTO |
| NFR-04 | `IRequestCacheService` para compartir datos entre validator y handler | contracts.md notas backend punto 10: "Usar IRequestCacheService para la resolucion de ArtistaId y carga del AcuerdoCrowdsourcing" | CUBIERTO |
| NFR-05 | Validar en backend que el usuario es participante en TODOS los endpoints (no confiar en frontend) | contracts.md seccion "Autorizacion" - tabla resumen con todos los endpoints y sus reglas de autorizacion documentadas | CUBIERTO |
| NFR-06 | `miRol` calculado en backend (no aceptar del cliente) | contracts.md: "`miRol`: 'Artista' si UserId == artista.userId, 'Profesional' si UserId == profesional.userId"; contracts-plan.md §8.4: "el frontend no necesita comparar IDs, solo lee `miRol`" | CUBIERTO |
| NFR-07 | Motivo de rechazo de propuesta no expuesto al profesional | contracts.md: "El profesional NO puede verlo" en nota del endpoint PATCH /propuestas/{id}/rechazar; `RechazarPropuestaResult` solo devuelve `estadoPropuestaNombre` | PARCIAL (ver gap GAP-02) |
| NFR-08 | AceptarPropuestaCommand en unica transaccion de BD | contracts.md notas backend punto 1: "Dentro de una transaccion de base de datos" con los 5 pasos documentados | CUBIERTO |
| NFR-09 | Barra de progreso de milestones visible en tiempo real en detalle | contracts.md notas frontend §4 punto 1: "La barra de progreso se actualiza usando el valor de `importeAsignadoTotal` del servidor"; `ImporteAsignadoBar` componente en contracts-plan.md | CUBIERTO |
| NFR-10 | Dialogo de cancelacion con texto de advertencia explicito | contracts.md notas frontend §4 cuarto punto: texto exacto documentado | CUBIERTO |
| NFR-11 | Toast en TODAS las acciones exitosas | contracts.md notas frontend §4 ultimo punto: "Toast tras cada accion exitosa con el mensaje del campo `messages[0].message`" | CUBIERTO |
| NFR-12 | Acciones renderizadas condicionalmente segun `miRol` y estado del acuerdo | contracts.md notas frontend §4 segundo punto: tabla artista/profesional documentada | CUBIERTO |
| NFR-13 | Sugerencia de marcar milestone como completado cuando todos los entregables estan aprobados | `AprobarEntregableResult.todosAprobadosEnMilestone: boolean` en contracts-plan.md; hook `useAprobarEntregable`: "si `todosAprobadosEnMilestone == true`, mostrar sugerencia" | CUBIERTO |
| NFR-14 | Aviso (no bloqueante) al completar si hay entregables pendientes | contracts.md notas frontend §4: "si hay entregables con `estadoEntregableNombre == 'Entregado'`, mostrar aviso antes de la confirmacion"; `CompletarAcuerdoDialog` | CUBIERTO |
| NFR-15 | Timeline limitado a 20 eventos mas recientes | contracts.md: "`timeline`: ultimos 20 eventos de actividad del acuerdo, ordenados de mas reciente a mas antiguo" | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Mayores (cobertura parcial con impacto en implementacion)

| ID Gap | Criterio Afectado | Descripcion del Gap | Impacto | Recomendacion |
|--------|-------------------|---------------------|---------|---------------|
| GAP-01 | AC-CS04-5 | El tipo `Acuerdo` en `contracts-plan.md` (y en `contracts.md`) no incluye el campo `entregablesSinMilestone` que menciona `feature-spec.md` en el criterio: "entregables sin milestone aparecen al final". El response del GET detalle define `milestones[].entregables[]` pero no existe un array de nivel raiz para entregables sin milestone. | Medio - El componente `EntregablesSection` no tendra los datos necesarios para renderizar entregables sin milestone si no se agrega el campo al contrato y al type `Acuerdo`. | Agregar `entregablesSinMilestone: Entregable[]` al type `Acuerdo` en `contracts-plan.md §2.1` y al response del `GET /acuerdos/{id}` en `contracts.md`. Actualizar `AcuerdoDto.cs` en backend para incluir este campo. |
| GAP-02 | AC-CS04-12 | El contracts-plan.md documenta `RechazarPropuestaResult` con solo `estadoPropuestaNombre` (correcto para la respuesta de rechazo), pero no especifica explicitamente que el endpoint `GET /propuestas/mis-propuestas` (de US-CS-03) debe omitir el campo `MotivoRechazo` en su response para el profesional. Desde la perspectiva de la Landing, no hay una prueba de que el type de la propuesta en el contexto del profesional no exponga el motivo. | Medio - Sin verificacion explicita en el contrato compartido, el frontend podria accidentalmente renderizar un campo que llega del backend aunque no deberia. | Agregar una nota explicita en `contracts-plan.md` indicando que el type de `PropuestaResumen` (o equivalente de US-CS-03) no debe incluir `motivoRechazo` cuando el usuario autenticado es el profesional proponente. Verificar en el plan de US-CS-03 que la respuesta de `GET /mis-propuestas` para el profesional no incluye ese campo. |
| GAP-03 | AC-CS04-9 | El contracts-plan.md no especifica como el componente `EntregableItem` o `MilestoneCard` debe presentar visualmente el historial de versiones rechazadas de un entregable. El criterio AC-CS04-9 requiere que "el entregable rechazado queda como historial en estado Rechazado", lo que implica que el profesional debe poder ver ambas versiones en la UI. El plan de componentes no documenta como se relacionan visualmente entregables rechazados con su version nueva en el mismo milestone. | Medio - Sin esta especificacion, el desarrollador puede implementar la lista de entregables de forma que el historial quede poco claro para el usuario, o que el boton de "Subir entregable" no este contextualizado con el entregable rechazado anterior. | Agregar en `contracts-plan.md §2 componentes` una nota en `EntregableItem` y `MilestoneCard` indicando que los entregables se muestran en lista cronologica dentro del milestone, con badges de estado diferenciados (rojo para Rechazado, amarillo para Entregado, verde para Aprobado), sin ocultar los rechazados. Documentar que el boton "Subir entregable" siempre esta disponible para el profesional cuando el acuerdo esta Activo, independientemente de si hay entregables rechazados previos. |

### 4.2 Gaps Menores (falta de detalle documental)

| ID Gap | Criterio Afectado | Descripcion del Gap | Impacto | Recomendacion |
|--------|-------------------|---------------------|---------|---------------|
| GAP-04 | AC-CS04-5 | El contracts-plan.md lista `ConversacionLink` como componente pero no especifica a que ruta navega (el `conversacionId` esta disponible en `Acuerdo.conversacionId`). La feature de conversacion no esta implementada en el MVP segun out-of-scope, por lo que el comportamiento del link no esta definido. | Bajo - El desarrollador debera tomar una decision de implementacion sin guia: deshabilitar el link, mostrar un CTA de "proximamente", o navegar a una ruta que no existe. | Agregar en `contracts-plan.md` una nota en el componente `ConversacionLink` indicando el comportamiento MVP: el link debe mostrarse pero navegar a `/crowdsourcing/conversaciones/{conversacionId}` (ruta futura), y si la feature no existe aun, mostrar el boton deshabilitado con tooltip "Proximamente". |
| GAP-05 | AC-CS04-6 | El contracts-plan.md menciona `EditarMilestoneForm` como componente separado en la lista de `feature-spec.md` pero en su propia documentacion de componentes usa `MilestoneFormDialog` (un solo dialogo para crear y editar). Esta inconsistencia puede causar confusion al implementador. | Bajo - Confusion de nomenclatura solamente. El comportamiento esta correcto. | Consolidar la nomenclatura en `contracts-plan.md`: usar `MilestoneFormDialog` consistentemente, con una nota de que se usa para ambas operaciones (crear con campos vacios, editar con datos precargados). |
| GAP-06 | AC-CS04-10 | El contracts-plan.md menciona que al completar el acuerdo "se habilitan las valoraciones para ambas partes" pero no especifica como la UI lo refleja mas alla del toast. El criterio AC-CS04-10 menciona "se habilitan las valoraciones" como efecto. No hay componente de valoraciones en el plan ni seccion de "CTA para valorar" documentada. | Bajo - Es out-of-scope segun `feature-spec.md` ("Valoraciones mutuas entre artista y profesional (habilitadas al completar pero gestionadas en US futura)"), pero el criterio AC-CS04-10 menciona explicitamente que "se habilitan las valoraciones" como parte del AC. | Agregar una nota en `contracts-plan.md` aclarando que "se habilitan las valoraciones" es un efecto de backend (campo/flag en la entidad) que se implementara en la UI en una US futura. En el MVP, el toast es suficiente evidencia para el criterio. Actualizar el metodo de prueba de AC-CS04-10 para indicar que la verificacion de "valoraciones habilitadas" se hace a nivel de BD (campo), no de UI. |
| GAP-07 | NFR-03 | El contracts-plan.md en §8.3 nota sobre el indicador de importe en tiempo real afirma que "la barra de progreso se actualiza usando el valor de `importeAsignadoTotal` del servidor (invalidar query tras cada mutacion)". Esto contradice la especificacion en `contracts.md` (notas frontend) y `feature-spec.md` que dicen explicitamente que el indicador del formulario de milestone se calcula en frontend usando `watch` de RHF, SIN llamadas al backend. | Bajo - Es una inconsistencia documental entre los dos documentos del plan. No es un error de implementacion pero puede confundir al desarrollador. | Corregir `contracts-plan.md §8.3`: el indicador "Asignado: X de Y EUR (Z%)" en el formulario de milestone (modal) se calcula en frontend con `watch('importeParcial')` de React Hook Form y los datos ya cargados del acuerdo, SIN llamadas al backend. La barra en la vista de detalle del acuerdo (fuera del formulario) se actualiza tras cada mutacion exitosa invalidando el `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`. |

---

## 5. Validacion de Tests

### 5.1 Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Fuente | Estado |
|----------|-----------------|------|--------|--------|
| AC-CS04-1 | `AceptarPropuestaCommandHandlerTests` - transaccion exitosa | Unit (Backend) | feature-spec.md "Testing - Unit tests Backend" | CUBIERTO |
| AC-CS04-2 | `AceptarPropuestaCommandHandlerTests` - propuesta no pendiente, acuerdo ya existente, rollback ante fallo | Unit (Backend) | feature-spec.md "Testing - Unit tests Backend" | CUBIERTO |
| AC-CS04-2 | Flujo completo: "aceptar propuesta -> crear milestone -> subir entregable -> aprobar -> completar acuerdo" | Integration | feature-spec.md "Testing - Integration tests" | CUBIERTO |
| AC-CS04-3 | E2E: "Aceptar propuesta (toast + redirect al detalle)" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-4 | Integration: "Verificar que usuario tercero recibe 403 en todos los endpoints" | Integration | feature-spec.md "Testing - Integration tests" | CUBIERTO |
| AC-CS04-5 | E2E: "Crear acuerdo con milestones y entregables. Navegar al detalle. Verificar secciones." | E2E | feature-spec.md AC-CS04-5 "Metodo de Prueba" | CUBIERTO |
| AC-CS04-5 (gap) | Test de agrupacion de entregables sin milestone | E2E / Unit | No documentado en plans | NO CUBIERTO |
| AC-CS04-6 | `CreateMilestoneCommandHandlerTests` - validacion de suma de importes, orden correcto | Unit (Backend) | feature-spec.md "Testing" | CUBIERTO |
| AC-CS04-6 | E2E: "Crear milestone con indicador de importe en tiempo real" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-7 | `CreateEntregableCommandHandlerTests` | Unit (Backend) | feature-spec.md "Testing" (implicito) | CUBIERTO |
| AC-CS04-7 | E2E: "Subir entregable y verlo en la vista del artista" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-8 | `AprobarEntregableCommandHandlerTests`, `RechazarEntregableCommandHandlerTests` - validaciones | Unit (Backend) | feature-spec.md "Testing - Unit tests Backend" | CUBIERTO |
| AC-CS04-8 | E2E: "Aprobar y rechazar entregable" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-9 | Integration: "Rechazar entregable. Subir nueva version. Verificar dos entregables en BD." | Integration | feature-spec.md AC-CS04-9 "Metodo de Prueba" | CUBIERTO |
| AC-CS04-10 | `CompletarAcuerdoCommandHandlerTests` - transicion de estados, FechaFinReal | Unit (Backend) | feature-spec.md "Testing" | CUBIERTO |
| AC-CS04-10 | E2E: "Completar acuerdo con aviso de entregables pendientes" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-11 | `CancelarAcuerdoCommandHandlerTests` - transicion de estados, registro CanceladoPor | Unit (Backend) | feature-spec.md "Testing" | CUBIERTO |
| AC-CS04-11 | E2E: "Cancelar acuerdo con motivo obligatorio" | E2E | feature-spec.md "Testing - E2E tests" | CUBIERTO |
| AC-CS04-12 | Integration: "GET mis-propuestas como profesional: verificar que MotivoRechazo no aparece" | Integration | feature-spec.md AC-CS04-12 "Metodo de Prueba" | CUBIERTO |
| AC-CS04-12 | Test frontend: verificar que el type `PropuestaResumen` no incluye `motivoRechazo` en la respuesta | Unit (Frontend) | No documentado en plans | NO CUBIERTO |

### 5.2 Tests Faltantes

| Criterio | Test Requerido | Tipo | Razon |
|----------|----------------|------|-------|
| AC-CS04-5 | Test de que `entregablesSinMilestone` se renderiza al final de la lista de milestones | E2E o Component Test | El campo no esta en el contrato actual; requiere GAP-01 resuelto primero |
| AC-CS04-12 | Test unitario del componente de lista de propuestas (profesional) que verifica que el campo `motivoRechazo` no se renderiza aunque exista en el objeto | Unit (Frontend) | La privacidad del motivo debe ser doble: backend no lo expone Y frontend tampoco lo renderiza accidentalmente |
| AC-CS04-6 (FA-09) | Test frontend: el boton "Editar milestone" no es visible cuando `milestone.fechaCompletado != null` | Unit / Component Test | El flujo alternativo FA-09 no tiene test documentado para la capa UI |
| AC-CS04-8 (FA-08) | Test frontend: los botones "Aprobar" y "Rechazar" no son visibles cuando el entregable no esta en estado Entregado | Unit / Component Test | El flujo alternativo FA-08 no tiene test documentado para la capa UI |

---

## 6. Validacion de UI/UX

### 6.1 Screens Requeridas vs Planificadas

| Screen Requerida (feature-spec.md) | Planificada (contracts-plan.md) | Componentes principales | Estado |
|------------------------------------|---------------------------------|------------------------|--------|
| Formulario de aceptar propuesta (modal/dialog con campos editables) | Si - `AceptarPropuestaRequest` + `aceptarPropuestaSchema` en shared; referenciado en notas de `useAceptarPropuesta` | No tiene un componente nombrado explicitamente en contracts-plan.md para el formulario-resumen pre-aceptacion | PARCIAL |
| Detalle de acuerdo (`/crowdsourcing/acuerdos/:id`) | Si - `AcuerdoDetailPage`, ruta en `APP_ROUTES.landing.crowdsourcing.acuerdoDetail` | `AcuerdoDetailPage`, `AcuerdoHeader`, `MilestonesSection`, `ImporteAsignadoBar`, `AcuerdoTimeline` | CUBIERTO |
| Formulario crear/editar milestone (modal/drawer) | Si - `MilestoneFormDialog` | `MilestoneFormDialog` con `createMilestoneSchema`, indicador de importe en tiempo real | CUBIERTO |
| Formulario subir entregable (modal) | Si - `SubirEntregableDialog` | `SubirEntregableDialog` con `createEntregableSchema`, select de milestones | CUBIERTO |
| Dialog aprobar entregable | Si - `AprobarEntregableDialog` | `AprobarEntregableDialog` con comentario opcional | CUBIERTO |
| Dialog rechazar entregable | Si - `RechazarEntregableDialog` | `RechazarEntregableDialog` con comentario obligatorio min 10 | CUBIERTO |
| Dialog completar acuerdo (con resumen + aviso pendientes) | Si - `CompletarAcuerdoDialog` | `CompletarAcuerdoDialog` con listado de pendientes | CUBIERTO |
| Dialog cancelar acuerdo (texto advertencia + motivo) | Si - `CancelarAcuerdoDialog` | `CancelarAcuerdoDialog` con texto exacto de advertencia | CUBIERTO |

### 6.2 Estados de UI

| Estado de UI | Requerido (feature-spec.md) | Planificado | Detalle |
|-------------|------------------------------|-------------|---------|
| Badge Activo (azul) en cabecera del acuerdo | Si | Si | `ESTADO_ACUERDO_BADGES[1]: 'blue'` en constants-plan.md §4.1 |
| Badge Completado (verde) | Si | Si | `ESTADO_ACUERDO_BADGES[2]: 'green'` |
| Badge Cancelado (gris) | Si | Si | `ESTADO_ACUERDO_BADGES[3]: 'gray'` |
| Badge Entregado (amarillo) para entregables | Si | Si | `ESTADO_ENTREGABLE_BADGES[1]: 'yellow'` |
| Badge Aprobado (verde) para entregables | Si | Si | `ESTADO_ENTREGABLE_BADGES[2]: 'green'` |
| Badge Rechazado (rojo) para entregables | Si | Si | `ESTADO_ENTREGABLE_BADGES[3]: 'red'` |
| Barra de progreso de importes asignados | Si | Si | `ImporteAsignadoBar` componente |
| Indicador "Asignado: X de Y EUR (Z%)" en formulario milestone | Si | Si | Usando `watch` de React Hook Form |
| Loading state al cargar detalle del acuerdo | Si (implicito) | Parcial | No mencionado en contracts-plan.md; TanStack Query provee `isLoading` |
| Error state (403 Forbidden) con mensaje de acceso denegado | Si (FA-07) | Parcial | No hay componente de error de acceso documentado en contracts-plan.md |
| Estado vacio (acuerdo sin milestones) | Si (FA-05: milestones opcionales) | No documentado | contracts-plan.md no especifica estado vacio de `MilestonesSection` |
| Acciones visibles solo para el rol correcto | Si | Si | Renderizado condicional basado en `miRol` documentado en contracts.md |

### 6.3 Validaciones de Formulario

| Campo / Formulario | Validacion Requerida | Zod Schema Planificado | Mensaje de Error | Estado |
|--------------------|---------------------|------------------------|-----------------|--------|
| TituloInterno (aceptar propuesta) | min 1, max 200 | `aceptarPropuestaSchema.tituloInterno` | 'El titulo interno es obligatorio' / 'El titulo no puede superar los 200 caracteres' | CUBIERTO |
| FechaFinPrevista > FechaInicio | Refine cross-field | `aceptarPropuestaSchema` refine | 'La fecha de fin prevista debe ser posterior a la fecha de inicio' | CUBIERTO |
| Motivo (rechazar propuesta) | max 500 si presente | `rechazarPropuestaSchema.motivo` | 'El motivo no puede superar los 500 caracteres' | CUBIERTO |
| Titulo milestone | min 3, max 200 | `createMilestoneSchema.titulo` | 'El titulo debe tener al menos 3 caracteres' | CUBIERTO |
| ImporteParcial | > 0 | `createMilestoneSchema.importeParcial` | 'El importe parcial debe ser mayor a 0' | CUBIERTO |
| Suma importes <= total | Validacion en componente con contexto del acuerdo | No en schema (requiere contexto) | Mensaje en tiempo real en el formulario | CUBIERTO |
| FechaLimite >= FechaInicio acuerdo | Validacion en componente | No en schema (requiere contexto) | Nota en contracts-plan.md §8.2 | CUBIERTO |
| Titulo entregable | min 3, max 200 | `createEntregableSchema.titulo` | 'El titulo debe tener al menos 3 caracteres' | CUBIERTO |
| UrlRecurso | URL valida si presente, acepta string vacio | `createEntregableSchema.urlRecurso` con `.optional().or(z.literal(''))` | 'Debe ser una URL valida' | CUBIERTO |
| Comentario aprobar | max 500 si presente | `aprobarEntregableSchema.comentario` | 'El comentario no puede superar los 500 caracteres' | CUBIERTO |
| Comentario rechazar | obligatorio, min 10, max 500 | `rechazarEntregableSchema.comentario` | 'El comentario es obligatorio al rechazar' / 'Minimo 10 caracteres' | CUBIERTO |
| Motivo cancelar | obligatorio, min 20, max 1000 | `cancelarAcuerdoSchema.motivo` | 'El motivo es obligatorio' / 'El motivo debe tener al menos 20 caracteres' | CUBIERTO |

---

## 7. Criterios Criticos con Atencion Especial

Los siguientes aspectos requieren atencion especial durante la implementacion por su complejidad o impacto en la integridad de datos.

### 7.1 Transaccion Atomica de AceptarPropuesta (CRITICO)

**Criterio:** AC-CS04-2

**Descripcion del riesgo:** La operacion de aceptar propuesta tiene 6 efectos secundarios en una sola transaccion. Si cualquiera de los pasos falla, el estado de la BD debe quedar exactamente igual que antes de la operacion (rollback completo). Un fallo a mitad deja el sistema en estado inconsistente (por ejemplo: acuerdo creado pero propuesta aun en Pendiente).

**Cobertura actual:** Documentada en `contracts.md` con los 5 pasos del handler y el uso de `IDbContextTransaction`. El test `AceptarPropuestaCommandHandlerTests` debe incluir casos de excepcion en cada paso individual.

**Verificacion QA especifica:**
- Simular fallo en el paso 3 (crear `ConversacionCrowdsourcing`) y verificar que ni el acuerdo ni los cambios de estado de propuesta/necesidad persisten.
- Verificar que `propuestasRechazadas` en el response coincide exactamente con el numero de propuestas que estaban en estado Pendiente (excluyendo la aceptada).
- Verificar que la propuesta rechazada automaticamente incluye el motivo exacto "Otra propuesta fue aceptada" en BD.

### 7.2 Autorizacion por Participante en Todos los Endpoints (CRITICO)

**Criterio:** AC-CS04-4, AC-CS04-6, AC-CS04-7, AC-CS04-8, AC-CS04-11

**Descripcion del riesgo:** Cada endpoint tiene una regla de autorizacion diferente (artista, profesional, o cualquier participante). El backend no puede confiar en el frontend para aplicar estas restricciones. Un error de implementacion en un solo endpoint expone operaciones criticas a usuarios no autorizados.

**Cobertura actual:** La tabla de autorizacion en `contracts.md` documenta todos los endpoints con su regla exacta. El test de integracion "Verificar que usuario tercero recibe 403 en todos los endpoints" esta planificado.

**Verificacion QA especifica:**
- Para cada endpoint, ejecutar la accion con tres tipos de usuario: el rol correcto (exito), el otro participante (403 o 400 segun corresponda), y un tercero autenticado (403).
- Caso especial: el endpoint de cancelar acepta tanto artista como profesional; verificar que ambos pueden cancelar pero que `CanceladoPor` registra el UserId correcto en cada caso.
- Caso especial: verificar que el artista NO puede subir entregables (403) y el profesional NO puede crear milestones (403).

### 7.3 Privacidad del Motivo de Rechazo de Propuesta (CRITICO)

**Criterio:** AC-CS04-12

**Descripcion del riesgo:** El motivo de rechazo se almacena en la BD pero no debe aparecer en el endpoint `GET /propuestas/mis-propuestas` cuando el consumidor es el profesional. Si el campo se incluye accidentalmente en el DTO, el profesional puede ver informacion que no le corresponde.

**Cobertura actual:** PARCIAL - `contracts.md` documenta la nota "El profesional NO puede verlo" y `RechazarPropuestaResult` solo devuelve el estado. Sin embargo, contracts-plan.md no especifica explicitamente como se garantiza esto en el endpoint `GET /mis-propuestas` de US-CS-03.

**Verificacion QA especifica:**
- Rechazar una propuesta con un motivo especifico (por ejemplo: "Budget issues").
- Autenticarse como el profesional cuya propuesta fue rechazada.
- Llamar a `GET /api/crowdsourcing/propuestas/mis-propuestas` y verificar que el campo `motivoRechazo` no existe en el objeto de la propuesta o devuelve `null`.
- Este test debe hacerse tanto a nivel de API (Swagger/Postman) como verificando el type TypeScript del componente de lista de propuestas del profesional.

### 7.4 Race Condition en Suma de Importes de Milestones (MAYOR)

**Criterio:** AC-CS04-6

**Descripcion del riesgo:** Si dos requests de `CreateMilestoneCommand` se ejecutan simultaneamente para el mismo acuerdo, ambos validators pueden leer la misma suma actual y ambos pasar la validacion, resultando en una suma total que supera el `ImporteTotalPactado`.

**Cobertura actual:** Documentado como riesgo en `feature-spec.md` (tabla de riesgos: "condicion de carrera (dos creates simultaneos)"). La mitigacion es "validar en validator con query actual a BD. Considerar optimistic concurrency". El contracts-plan.md no documenta una solucion especifica.

**Verificacion QA especifica:**
- Aunque este escenario es de baja probabilidad en el MVP, documentar en el plan de tests que la suma se valida tanto en el validator (pre-insert) como que se considera agregar un constraint a nivel de BD o aplicar `SELECT FOR UPDATE` en la query de suma.
- En la implementacion del handler, verificar que la suma se calcula dentro de la misma transaccion que el INSERT del milestone.

---

## 8. Checklist de Validacion QA Manual

### 8.1 Flujo de Aceptar Propuesta

- [ ] El formulario-resumen premuestra los valores por defecto correctos (TituloInterno = titulo necesidad, FechaInicio = hoy, FechaFinPrevista = hoy + DiasEstimados de propuesta)
- [ ] Los campos TituloInterno, FechaInicio y FechaFinPrevista son editables; ImporteTotalPactado es readonly
- [ ] La validacion `fechaFinPrevista > fechaInicio` funciona en tiempo real antes de enviar
- [ ] El toast muestra exactamente: "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."
- [ ] Tras confirmar, el artista es redirigido a `/crowdsourcing/acuerdos/{acuerdoId}` (usando el `acuerdoId` del response)
- [ ] Si ya existe un acuerdo activo para la necesidad, se muestra el mensaje de error correspondiente (errorCode 4008)
- [ ] El error 403 se muestra si el usuario autenticado no es el artista propietario de la necesidad

### 8.2 Vista de Detalle del Acuerdo

- [ ] La cabecera muestra: titulo, badge de estado con color correcto, nombres del artista y profesional, importe total con moneda, fechas de inicio y fin prevista
- [ ] La barra de progreso `ImporteAsignadoBar` muestra "Asignado: X de Y EUR (Z%)" con los valores correctos
- [ ] Los milestones se muestran ordenados por campo `orden`
- [ ] Cada milestone muestra: titulo, importe parcial, porcentaje del total, fecha limite, estado (completado o pendiente), y lista de entregables anidados
- [ ] Los entregables sin milestone aparecen en una seccion separada al final
- [ ] El link/boton de conversacion esta presente (aunque pueda estar deshabilitado en MVP)
- [ ] El timeline muestra los ultimos eventos con fecha y actor
- [ ] Un usuario que no es participante del acuerdo ve una pantalla de acceso denegado (no el detalle)

### 8.3 Milestones (solo artista)

- [ ] El boton "+ Agregar milestone" solo es visible para el artista; el profesional no lo ve
- [ ] El formulario de milestone muestra el indicador de importe en tiempo real al escribir en el campo de importe
- [ ] Si la suma supera el total pactado, el error aparece inmediatamente (sin necesidad de enviar)
- [ ] El milestone recien creado aparece en la lista sin recargar la pagina
- [ ] Un milestone con `fechaCompletado != null` no muestra el boton "Editar"
- [ ] Un milestone con entregables asociados muestra el error al intentar eliminar (no aparece boton eliminar en UI)
- [ ] El boton "Editar" abre el formulario con los datos actuales precargados

### 8.4 Entregables (solo profesional para subir, solo artista para revisar)

- [ ] El boton "Subir entregable" solo es visible para el profesional; el artista no lo ve
- [ ] El formulario de entregable incluye un select con los milestones disponibles del acuerdo (campo opcional)
- [ ] La validacion de URL rechaza URLs malformadas antes de enviar
- [ ] El toast muestra exactamente: "Entregable subido correctamente. El artista sera notificado."
- [ ] El entregable aparece inmediatamente en la vista de detalle bajo el milestone seleccionado
- [ ] Los botones "Aprobar" y "Rechazar" solo son visibles para el artista en entregables en estado Entregado
- [ ] Al aprobar sin comentario, la operacion es exitosa
- [ ] Al aprobar con comentario de 501 caracteres, se muestra el error de validacion
- [ ] Al rechazar sin comentario, se muestra el error "El comentario es obligatorio al rechazar"
- [ ] Al rechazar con 9 caracteres, se muestra el error "Minimo 10 caracteres"
- [ ] Al rechazar con 10+ caracteres, la operacion es exitosa
- [ ] Tras aprobar todos los entregables de un milestone, aparece sugerencia de marcar milestone como completado

### 8.5 Completar y Cancelar Acuerdo

- [ ] El boton "Completar acuerdo" solo es visible para el artista
- [ ] El dialog de completar muestra el resumen: milestones completados, entregables aprobados, importe total
- [ ] Si hay entregables en estado Entregado, el dialog muestra un aviso (no bloqueante)
- [ ] El toast muestra exactamente: "Acuerdo completado. Puedes dejar una valoracion al profesional."
- [ ] El boton "Cancelar acuerdo" es visible para artista Y profesional
- [ ] El dialog de cancelar muestra exactamente el texto: "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas."
- [ ] Cancelar sin motivo muestra el error de validacion
- [ ] Cancelar con motivo de 19 caracteres muestra el error "El motivo debe tener al menos 20 caracteres"
- [ ] Cancelar con motivo de 20+ caracteres es exitoso
- [ ] El toast muestra: "Acuerdo cancelado."
- [ ] Tras completar o cancelar, los botones de edicion de milestones y subida de entregables desaparecen

### 8.6 Rechazar Propuesta Individual

- [ ] El artista ve el boton "Rechazar" solo en propuestas en estado Pendiente
- [ ] El campo de motivo es opcional (se puede confirmar sin escribir nada)
- [ ] Si se escribe un motivo de 501 caracteres, se muestra error de validacion
- [ ] Tras rechazar, la propuesta muestra el estado "Rechazada" en la vista del artista
- [ ] El profesional ve que su propuesta esta "Rechazada" pero NO ve el motivo escrito por el artista

---

## 9. Escenarios de Prueba Exploratorios Sugeridos

Estos escenarios cubren casos de borde y flujos combinados que no estan explicitamente documentados en los criterios de aceptacion pero que son criticos para la correcta operacion del sistema.

### ESC-01: Ciclo de vida completo del acuerdo (happy path)

**Objetivo:** Verificar que el flujo completo funciona end-to-end sin errores.

**Pasos:**
1. Artista A crea necesidad N con 3 propuestas de profesionales P1, P2, P3.
2. Artista A acepta la propuesta de P1. Verificar: acuerdo creado, P1=Aceptada, P2=Rechazada, P3=Rechazada, necesidad=En Progreso, conversacion creada.
3. Artista A crea 2 milestones. Verificar: suma de importes <= total, barra actualizada.
4. Profesional P1 sube entregable en milestone 1. Verificar: estado=Entregado, visible para artista.
5. Artista A rechaza el entregable con comentario de 15 chars. Verificar: estado=Rechazado, historial visible.
6. Profesional P1 sube nueva version. Verificar: dos entregables en la lista, uno Rechazado y uno Entregado.
7. Artista A aprueba la nueva version. Verificar: estado=Aprobado, FechaAprobacion registrada.
8. Artista A completa el acuerdo. Verificar: estado=Completado, necesidad=Cerrada, toast exacto.

**Resultado esperado:** Todos los estados transicionan correctamente, los toasts muestran los textos exactos, los datos en BD son consistentes.

### ESC-02: Cancelacion por el profesional

**Objetivo:** Verificar que el profesional puede cancelar y que `CanceladoPor` registra su UserId.

**Pasos:**
1. Crear acuerdo entre artista A y profesional P.
2. Profesional P cancela con motivo de 30 chars.
3. Verificar en BD: `CanceladoPor = UserId del profesional P`.
4. Verificar que la necesidad vuelve a estado Abierta.
5. Verificar que artista A ya no puede crear milestones (acuerdo Cancelado).

### ESC-03: Intento de operaciones sobre acuerdo no activo

**Objetivo:** Verificar que los guards de estado del acuerdo funcionan correctamente en todos los endpoints.

**Pasos:**
1. Completar un acuerdo (estado=Completado).
2. Intentar: subir entregable -> esperar error 400 (acuerdo no activo).
3. Intentar: crear milestone -> esperar error 400 (acuerdo no activo).
4. Intentar: cancelar -> esperar error 400 (acuerdo no activo).
5. Repetir pasos 2-4 con un acuerdo Cancelado.

### ESC-04: Validacion del limite de importes con actualizaciones de milestone

**Objetivo:** Verificar que la validacion de suma de importes funciona tanto en creacion como en edicion, incluyendo el caso de editar el ultimo milestone para reducir su importe.

**Pasos:**
1. Crear acuerdo con ImporteTotalPactado = 1000 EUR.
2. Crear milestone A con importeParcial = 600 EUR. (suma = 600)
3. Intentar crear milestone B con importeParcial = 500 EUR. Esperar error (1100 > 1000).
4. Crear milestone B con importeParcial = 400 EUR. (suma = 1000) - exito.
5. Intentar editar milestone A para subir a 700 EUR. Esperar error (1100 > 1000 con B=400).
6. Editar milestone A para bajar a 500 EUR. (suma = 900) - exito.
7. Crear milestone C con importeParcial = 100 EUR. (suma = 1000) - exito.
8. Intentar crear milestone D con importeParcial = 1 EUR. Esperar error (1001 > 1000).

### ESC-05: Acceso de usuario tercero a todos los endpoints protegidos

**Objetivo:** Verificar que el usuario C (que no es participante del acuerdo entre A y P) recibe 403 en todos los endpoints.

**Pasos para cada endpoint:**
1. Autenticar como usuario C (tercero, no participante).
2. `GET /acuerdos/{id}` -> 403.
3. `POST /acuerdos/{id}/milestones` -> 403.
4. `PUT /acuerdos/{id}/milestones/{milestoneId}` -> 403.
5. `DELETE /acuerdos/{id}/milestones/{milestoneId}` -> 403.
6. `POST /acuerdos/{id}/entregables` -> 403.
7. `PATCH /entregables/{entregableId}/aprobar` -> 403.
8. `PATCH /entregables/{entregableId}/rechazar` -> 403.
9. `PATCH /acuerdos/{id}/completar` -> 403.
10. `PATCH /acuerdos/{id}/cancelar` -> 403.

### ESC-06: Toast y textos exactos

**Objetivo:** Verificar que los textos de los toasts son exactamente los especificados en feature-spec.md.

| Accion | Texto esperado (exacto) |
|--------|------------------------|
| Aceptar propuesta | "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." |
| Subir entregable | "Entregable subido correctamente. El artista sera notificado." |
| Completar acuerdo | "Acuerdo completado. Puedes dejar una valoracion al profesional." |
| Cancelar acuerdo | "Acuerdo cancelado." |

**Nota:** Estos textos deben tomarse directamente del campo `messages[0].message` de la response del backend (no hardcodeados en frontend). Verificar que el backend retorna exactamente esos strings.

---

## 10. Recomendaciones

### Acciones Requeridas antes de implementar (Gaps Mayores)

1. **Resolver GAP-01: Agregar `entregablesSinMilestone` al contrato**
   - Archivo: `docs/user-stories/cs-acuerdos-entregables/contracts.md`
   - Cambio: Agregar `"entregablesSinMilestone": []` al response JSON del `GET /acuerdos/{id}`. Agregar `entregablesSinMilestone: Entregable[]` al type `Acuerdo` en `contracts-plan.md §2.1` y al `AcuerdoDto.cs`.
   - Razon: Sin este campo, el componente `EntregablesSection` no puede renderizar entregables sin milestone, incumpliendo AC-CS04-5.

2. **Resolver GAP-02: Documentar explicitamente la privacidad del motivo en mis-propuestas**
   - Archivo: `plans/cs-acuerdos-entregables/shared/contracts-plan.md` o el plan de US-CS-03
   - Cambio: Agregar una nota en la seccion de utilidades (`error-messages.ts` o en una nota de implementacion) que indique que el type de `PropuestaResumen` (US-CS-03) no debe incluir el campo `motivoRechazo`. Verificar que el plan de backend de US-CS-03 excluye ese campo del DTO de respuesta del endpoint `GET /mis-propuestas`.
   - Razon: AC-CS04-12 es un criterio de seguridad/privacidad. Si el backend expone el campo y el frontend lo renderiza, el profesional vera informacion confidencial.

3. **Resolver GAP-03: Documentar la presentacion visual del historial de entregables rechazados**
   - Archivo: `plans/cs-acuerdos-entregables/shared/contracts-plan.md`
   - Cambio: Agregar en la seccion de componentes (`EntregableItem`, `MilestoneCard`) una nota indicando: los entregables se muestran en lista cronologica ordenada por `fechaCreacion` ASC. Los entregables rechazados no se ocultan. El boton "Subir entregable" del profesional siempre esta disponible para el profesional cuando el acuerdo esta Activo (independiente del estado de los entregables previos).
   - Razon: AC-CS04-9 requiere que el historial sea visible y que el profesional pueda subir nueva version.

### Acciones Sugeridas (Gaps Menores)

4. **Resolver GAP-04: Documentar comportamiento MVP del ConversacionLink**
   - Archivo: `plans/cs-acuerdos-entregables/shared/contracts-plan.md`
   - Cambio: Agregar nota en el componente `ConversacionLink`: en MVP, mostrar boton deshabilitado con tooltip "Mensajeria disponible proximamente" si la feature de conversacion no esta implementada.

5. **Resolver GAP-07: Corregir inconsistencia sobre el indicador de importe en tiempo real**
   - Archivo: `plans/cs-acuerdos-entregables/shared/contracts-plan.md §8.3`
   - Cambio: Separar claramente: (a) indicador en el formulario de milestone = calculo en frontend con `watch` de RHF, sin llamadas al backend; (b) barra en la vista de detalle = se actualiza al invalidar la query del acuerdo tras cada mutacion exitosa.

6. **Agregar tests de componente para flujos alternativos FA-08 y FA-09**
   - Archivo: Plan de tests de la Landing (no existe aun)
   - Cambio: Cuando se genere el plan de tests frontend, incluir tests de componente para `EntregableItem` (botones no visibles cuando entregable no es Entregado) y `MilestoneCard` (boton editar no visible cuando milestone completado).

---

## 11. Conclusion

**Score Final:** 87.5%

**Veredicto: REQUIERE CAMBIOS**

Los planes existentes (`contracts.md` y `contracts-plan.md`) cubren con alto detalle la mayoria de los criterios de aceptacion, incluyendo todos los endpoints, DTOs, schemas Zod, constantes de estado, hooks y servicios de la Landing. Los flujos transaccionales criticos estan documentados y los tests planificados cubren los escenarios principales.

Sin embargo, tres gaps mayores deben resolverse antes de iniciar la implementacion:

1. **GAP-01** (AC-CS04-5): El campo `entregablesSinMilestone` no existe en el contrato actual. Sin este campo, el requisito de mostrar entregables sin milestone al final de la lista no puede implementarse correctamente.
2. **GAP-02** (AC-CS04-12): La privacidad del motivo de rechazo de propuesta no esta explicitamente garantizada en el contrato compartido para el endpoint `GET /mis-propuestas`.
3. **GAP-03** (AC-CS04-9): La presentacion visual del historial de entregables rechazados no esta especificada en los planes de componentes.

Una vez resueltos estos tres gaps (estimado: actualizaciones menores a `contracts.md` y `contracts-plan.md`), el plan estara en condiciones de proceder a la implementacion.

**Proximo Paso:** Resolver GAP-01, GAP-02 y GAP-03, luego re-validar. Si los tres gaps se cierran correctamente, el score subiria a 97%+ y el estado pasaria a APROBADO.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-18
