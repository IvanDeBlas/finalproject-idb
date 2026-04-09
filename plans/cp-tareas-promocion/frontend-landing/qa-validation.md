# Validacion QA: cp-tareas-promocion (Landing)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos evaluados (Landing) | 14 AC + 5 RNF + 8 flujos alternativos = 27 items |
| AC con impacto directo en Landing | 6 (AC-CP04-1, AC-CP04-2, AC-CP04-9, AC-CP04-14, parcialmente AC-CP04-4, AC-CP04-6) |
| AC cubiertos por documentacion existente | 4 |
| AC parcialmente cubiertos | 2 |
| AC no cubiertos por falta de plan frontend | 0 (los exclusivos del backend no se evaluan aqui) |
| **Plans Landing existentes** | **0 de 3 requeridos** |
| **Score de Cobertura (docs de spec vs plan frontend)** | **0% — No existen planes frontend** |

**Estado: RECHAZADO**

**Razon del rechazo:** El directorio `plans/cp-tareas-promocion/frontend-landing/` no contiene ningun plan de implementacion. Los archivos `frontend-plan.md`, `ui-design.md` y `test-strategy.md` no existen. No es posible validar cobertura de implementacion sin planes. La documentacion de spec (feature-spec.md, contracts.md, ui-ux.md) esta completa y bien definida, lo que facilita la creacion de los planes pendientes.

---

## 2. Criterios de Aceptacion

### Fuente: docs/user-stories/cp-tareas-promocion/feature-spec.md

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|---------|
| AC-CP04-1 | El promotor aprobado puede ver el listado de tareas activas con nombre, descripcion, tipo de evento, importe de recompensa, si es repetible y su estado personal en cada tarea | Funcional | Backend + **Landing** |
| AC-CP04-2 | El promotor puede enviar el completado de una tarea con URL de prueba valida (formato URL); el campo comentario es opcional con max 500 caracteres | Funcional | Backend + **Landing** |
| AC-CP04-3 | Al completar una tarea, se crea PromoTareaPromotor con datos correctos | Funcional | Backend (solo) |
| AC-CP04-4 | Para tareas repetibles, el promotor puede enviar completados sucesivos hasta MaxRepeticiones | Funcional | Backend + **Landing** (UI condicional) |
| AC-CP04-5 | Tarea no repetible ya completada/validada retorna 400 con mensaje | Funcional | Backend (solo) |
| AC-CP04-6 | Tarea repetible con MaxRepeticiones alcanzado retorna 400 con mensaje | Funcional | Backend + **Landing** (UI condicional) |
| AC-CP04-7 | El artista puede ver listado paginado de completados pendientes | Funcional | Backend + Admin (solo) |
| AC-CP04-8 | Al validar, backend actualiza estado y crea transaccion de wallet en una transaccion | Funcional | Backend (solo) |
| AC-CP04-9 | Al rechazar, backend actualiza estado y persiste ComentarioValidacion; el promotor puede ver el motivo en su vista | Funcional | Backend + **Landing** + Admin |
| AC-CP04-10 | Promotor con completado rechazado puede re-enviar con nueva URL | Funcional | Backend (solo) |
| AC-CP04-11 | No se puede completar tarea de programa inactivo; backend retorna 400 | Funcional | Backend (solo) |
| AC-CP04-12 | Promotor no aprobado recibe 403 Forbidden | Funcional | Backend (solo) |
| AC-CP04-13 | Wallet se crea automaticamente si no existe al validar | Funcional | Backend (solo) |
| AC-CP04-14 | Los tipos TypeScript de tareas y completados estan definidos en shared y son reutilizados por Landing y Admin | Funcional | **Shared** + Landing + Admin |

### Flujos Alternativos con Impacto en Landing

| ID | Condicion | Comportamiento requerido en Landing |
|----|-----------|-------------------------------------|
| FA-01 | Tarea no repetible ya tiene completado Completada/Validada | No mostrar boton de completar; mostrar estado actual |
| FA-02 | Tarea repetible con MaxRepeticiones alcanzado | Mostrar aviso "Limite de repeticiones alcanzado (X/X)" |
| FA-03 | Tarea fuera de fechas de vigencia (FechaFin pasada) | Mostrar aviso "Esta tarea ya no esta disponible" |
| FA-04 | Tarea desactivada (EsActivo = false) | No mostrar la tarea en el listado |
| FA-05 | Promotor tiene completado Rechazada | Permitir re-envio con nueva URL (boton "Re-enviar con nueva prueba") |
| FA-06 | Programa desactivado (EsActivo = false) | Mostrar tareas como solo lectura; aviso "Programa inactivo" |

### Requisitos No Funcionales con Impacto en Landing

| ID | Requisito | Impacto en Landing |
|----|-----------|-------------------|
| RNF-02 | GET mis-tareas debe responder en menos de 500ms | El frontend debe mostrar skeleton loading y no bloquear la UI |
| RNF-04 | URLs de prueba se almacenan tal como son; solo validar formato URL | El schema Zod debe usar `.url()` sin validacion de disponibilidad |
| RNF-05 | Completados y validaciones son registros inmutables; no se eliminan | La UI no debe ofrecer opcion de eliminar completados al promotor |

---

## 3. Matriz de Trazabilidad

> **NOTA CRITICA:** Los planes de implementacion para Landing (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) NO EXISTEN. La columna "Plan Frontend" se marca como "AUSENTE" para todos los criterios con impacto en Landing. La columna "Shared Plan" refleja el unico plan disponible: `plans/cp-tareas-promocion/shared/contracts-plan.md`.

### 3.1 Requisitos Funcionales — Criterios con impacto en Landing

| ID | Criterio (Landing) | Shared Plan | Frontend Plan | UI Design | Test Strategy | Estado |
|----|-------------------|-------------|---------------|-----------|---------------|--------|
| AC-CP04-1 | Listado de tareas activas con todos los campos | `MisTareasItem`, `MiEstadoTarea`, `QUERY_KEYS.tareas.mis` | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| AC-CP04-2 | Formulario de completado con URL obligatoria y comentario opcional (max 500) | `completarTareaSchema`, `CompletarTareaRequest` | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| AC-CP04-4 | UI condicional para tareas repetibles (boton "Completar de nuevo") | `puedeCompletarTarea`, `MisTareasItem.esRepetible` | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| AC-CP04-6 | UI de bloqueo cuando MaxRepeticiones alcanzado | `puedeCompletarTarea` (logica de negocio) | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| AC-CP04-9 | Vista del motivo de rechazo en la card del promotor + boton re-envio | `MiEstadoTarea.comentarioValidacion` | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| AC-CP04-14 | Tipos TypeScript en shared reutilizados por Landing | Completamente cubierto en `contracts-plan.md` (8 interfaces, 3 schemas, constantes) | AUSENTE (no se documenta el consumo desde Landing) | AUSENTE | AUSENTE | PARCIAL |

### 3.2 Requisitos No Funcionales — Impacto en Landing

| ID | Criterio | Shared Plan | Frontend Plan | UI Design | Test Strategy | Estado |
|----|----------|-------------|---------------|-----------|---------------|--------|
| RNF-02 | Skeleton loading durante GET mis-tareas | N/A | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| RNF-04 | Solo validar formato URL, no disponibilidad | `completarTareaSchema` usa `.url()` sin disponibilidad | AUSENTE | AUSENTE | AUSENTE | PARCIAL |
| RNF-05 | No ofrecer eliminacion de completados en UI | N/A | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |

### 3.3 Flujos Alternativos — Impacto en Landing

| ID | Condicion | Shared Plan | Frontend Plan | UI Design | Test Strategy | Estado |
|----|-----------|-------------|---------------|-----------|---------------|--------|
| FA-01 | No repetible ya completada/validada: no boton completar | `puedeCompletarTarea` retorna false | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| FA-02 | MaxRepeticiones alcanzado: aviso X/X | `puedeCompletarTarea` retorna false | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| FA-03 | Tarea fuera de fecha: aviso "ya no disponible" | N/A en shared | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| FA-04 | Tarea inactiva: no aparece en listado | N/A en shared (filtrado en backend) | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| FA-05 | Completado rechazado: boton "Re-enviar" | `puedeCompletarTarea` (estadoTareaId == 4) | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |
| FA-06 | Programa inactivo: tareas solo lectura + aviso | `TAREA_PROMOCION_ERROR_MESSAGES['4024']` | AUSENTE | AUSENTE | AUSENTE | NO CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-01 | AC-CP04-1 (Listado de tareas) | No existe `frontend-plan.md` para Landing. No hay componentes, hooks ni servicios planificados para la pagina `/promotor/programas/{programaId}/tareas` | Alto: sin plan no hay implementacion | Crear `plans/cp-tareas-promocion/frontend-landing/frontend-plan.md` con: pagina `MisTareasPage`, componente `TareaCard`, hook `useMisTareas`, servicio `crowdpromotion.service.ts` |
| GAP-02 | AC-CP04-2 (Formulario de completado) | No existe plan para el componente Dialog de completado con validacion Zod | Alto: flujo principal del promotor no planificado | Incluir en `frontend-plan.md`: componente `CompletarTareaDialog`, formulario con `react-hook-form` + `completarTareaSchema`, hook `useCompletarTarea` |
| GAP-03 | AC-CP04-9 (Motivo de rechazo visible) | No hay plan para mostrar `comentarioValidacion` en la card del promotor ni para el boton de re-envio condicional | Alto: requisito de visibilidad del rechazo no planificado | Incluir en `TareaCard` los estados de UI para `estadoTareaId == 4` con bloque de motivo de rechazo |
| GAP-04 | FA-01, FA-02, FA-03, FA-06 | No hay plan de componentes para los estados de UI condicionales (boton deshabilitado, avisos de limite, programa inactivo, fecha expirada) | Alto: sin estos estados, la UX viola las reglas de negocio RN-04, RN-05 | Documentar en `frontend-plan.md` la logica condicional de `TareaCard` segun `puedeCompletarTarea()` y los campos de estado |
| GAP-05 | TODOS | No existe `ui-design.md` para Landing | Alto: sin UI design planificado el implementador no tiene referencia de componentes especificos para este target | Crear `plans/cp-tareas-promocion/frontend-landing/ui-design.md` consolidando las especificaciones de Pantalla 1 y Pantalla 2 del `docs/user-stories/cp-tareas-promocion/ui-ux.md` |
| GAP-06 | TODOS | No existe `test-strategy.md` para Landing | Alto: sin estrategia de tests no se puede verificar la cobertura del 80% requerido por el proyecto | Crear `plans/cp-tareas-promocion/frontend-landing/test-strategy.md` con tests unitarios, de integracion y E2E para los flujos del promotor |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-07 | AC-CP04-14 (Tipos shared) | El `contracts-plan.md` es exhaustivo pero no documenta como Landing consumira los tipos. No hay plan de servicio API ni de hooks | Medio: el implementador de Landing debe inferir las dependencias de shared | En `frontend-plan.md`, documentar explicitamente los imports desde `src/shared`: `MisTareasItem`, `MiEstadoTarea`, `CompletarTareaRequest`, `completarTareaSchema`, `QUERY_KEYS.crowdpromotion.tareas.mis`, `API_ROUTES.crowdpromotion.programas.misTareas` |
| GAP-08 | RNF-02 (Performance < 500ms) | No hay plan para el manejo de skeleton loading y estados de carga en Landing | Medio: puede impactar la experiencia de usuario si no se implementa correctamente | En `frontend-plan.md`, documentar el patron de skeleton: 3 cards `animate-pulse` de 180px mientras `isLoading = true` en `useMisTareas` |
| GAP-09 | FA-05 (Re-envio tras rechazo) | No hay plan para el contexto del dialog cuando se abre desde "Re-enviar con nueva prueba" (nota visual diferente, titulo diferente) | Medio: el Dialog de completado tiene 3 variantes de apertura segun el contexto | Documentar en `frontend-plan.md` que `CompletarTareaDialog` recibe una prop `contexto: 'completar' | 'repetir' | 'reenviar'` que modifica titulo y nota contextual |
| GAP-10 | AC-CP04-1 (Orden de tareas) | El `ui-ux.md` especifica ordenar tareas (no completadas/rechazadas primero, luego completadas, luego validadas) pero no hay plan para implementar esta logica de ordenacion en el frontend | Medio: el backend no garantiza este orden en la respuesta | En `frontend-plan.md`, documentar que `useMisTareas` aplica ordenacion local del array de items tras recibirlos del API |
| GAP-11 | Toast messages (AC-CP04-2, AC-CP04-9) | Los mensajes de toast del `ui-ux.md` tienen variantes especificas segun el codigo de error (4027, 4028, 4024), pero no hay plan para como el hook de mutation mapea los error codes a los mensajes correctos | Medio: sin este mapeo los mensajes de error seran genericos | Documentar en `frontend-plan.md` que `useCompletarTarea` usa `getTareaPromocionErrorMessage(errorCode)` del shared para obtener el mensaje correcto del toast destructivo |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-12 | Breadcrumb (Pantalla 1) | No hay plan para el componente de breadcrumb en la ruta `/promotor/programas/{programaId}/tareas` | Bajo: detalle de navegacion | Incluir en `ui-design.md` la navegacion breadcrumb con links a `/promotor/mis-programas` |
| GAP-13 | Empty state (sin tareas activas) | No hay plan para el estado vacio cuando el programa no tiene tareas con `EsActivo = true` | Bajo: estado valido pero infrecuente | Incluir en `frontend-plan.md` el empty state con `ClipboardX` y mensajes especificados en `ui-ux.md` |
| GAP-14 | Error state (fallo de API) | No hay plan para el estado de error con boton "Reintentar" en la carga inicial | Bajo: ya cubierto en patron del proyecto | Documentar en `frontend-plan.md` que se usa el patron `error` de `useQuery` con reintentar via `refetch()` |
| GAP-15 | Responsive breakpoints | El `ui-ux.md` especifica 3 breakpoints para Pantalla 1 y comportamiento bottom sheet para Dialog en mobile, pero no hay plan que confirme la estrategia de implementacion | Bajo: las specs existen en `ui-ux.md` | Referenciar explicitamente en `ui-design.md` la tabla de breakpoints del `ui-ux.md` |
| GAP-16 | Accesibilidad | El `ui-ux.md` tiene una seccion completa de accesibilidad (WCAG, focus trap, aria labels) pero no hay plan que confirme su implementacion | Bajo: es un requisito implicito del proyecto | En `ui-design.md`, incluir checklist de accesibilidad para Pantalla 1 y Dialog Completar |
| GAP-17 | Animaciones de estado | Las animaciones de transicion de badge (300ms) y hover de card (200ms) estan especificadas en `ui-ux.md` pero no hay plan de implementacion | Bajo: mejora UX pero no es bloqueante | Incluir en `ui-design.md` las clases Tailwind de transicion para cada elemento animado |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

> No existe `test-strategy.md`. La tabla a continuacion refleja los tests que DEBERIAN planificarse segun los criterios de aceptacion y flujos con impacto en Landing.

| Criterio | Test Requerido | Tipo | Estado |
|----------|----------------|------|--------|
| AC-CP04-1 | Render de `MisTareasPage` con lista de tareas (todos los estados de badge) | Unit/Integration | NO CUBIERTO |
| AC-CP04-1 | `TareaCard` muestra nombre, descripcion, tipo evento, importe y estado | Unit | NO CUBIERTO |
| AC-CP04-2 | `CompletarTareaDialog` valida URL obligatoria con formato correcto | Unit | NO CUBIERTO |
| AC-CP04-2 | `CompletarTareaDialog` valida que comentario no supera 500 chars | Unit | NO CUBIERTO |
| AC-CP04-2 | Submit exitoso cierra dialog y muestra toast de exito | Integration | NO CUBIERTO |
| AC-CP04-4 | `TareaCard` muestra boton "Completar de nuevo" para tareas repetibles con cupo | Unit | NO CUBIERTO |
| AC-CP04-4 | `TareaCard` muestra aviso de limite cuando vecesCompletada == maxRepeticiones | Unit | NO CUBIERTO |
| AC-CP04-6 | API error 4028 muestra toast "Alcanzaste el maximo de repeticiones" | Integration | NO CUBIERTO |
| AC-CP04-9 | `TareaCard` muestra bloque de motivo de rechazo cuando estadoTareaId == 4 | Unit | NO CUBIERTO |
| AC-CP04-9 | `TareaCard` muestra boton "Re-enviar con nueva prueba" cuando estado == Rechazada | Unit | NO CUBIERTO |
| FA-01 | `TareaCard` no muestra boton completar para tarea no repetible ya validada | Unit | NO CUBIERTO |
| FA-02 | `TareaCard` muestra icono de bloqueo y texto cuando limite alcanzado | Unit | NO CUBIERTO |
| FA-05 | Dialog abierto desde "Re-enviar" muestra nota contextual de re-envio | Unit | NO CUBIERTO |
| FA-06 | `TareaCard` muestra aviso "Programa inactivo" y deshabilita acciones | Unit | NO CUBIERTO |
| RNF-02 | `MisTareasPage` muestra 3 skeleton cards durante el estado `isLoading` | Unit | NO CUBIERTO |
| E2E | Promotor completa tarea exitosamente y ve badge "PENDIENTE" en card | E2E | NO CUBIERTO |

### Tests Faltantes (Resumen)

| Tipo | Cantidad requerida | Planificada |
|------|--------------------|-------------|
| Unit (componentes) | 12 | 0 |
| Integration (hooks + API mock) | 4 | 0 |
| E2E (flujo completo) | 1 minimo | 0 |
| **Total** | **17** | **0** |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Especificada en ui-ux.md | Planificada en frontend | Estado |
|------------------|--------------------------|------------------------|--------|
| Pantalla 1: Mis Tareas (`/promotor/programas/{id}/tareas`) | Si (completa con layout, componentes, estados) | No existe `ui-design.md` | NO CUBIERTO |
| Pantalla 2: Dialog Completar Tarea | Si (completa con layout, validaciones, estados) | No existe `ui-design.md` | NO CUBIERTO |

> Las Pantallas 3-5 (Admin) no son responsabilidad de este plan de Landing.

### Estados de UI — Pantalla 1 (Mis Tareas)

| Estado | Requerido (ui-ux.md) | Planificado (frontend-plan) | Estado |
|--------|----------------------|-----------------------------|--------|
| Loading (3 skeleton cards) | Si | AUSENTE | NO CUBIERTO |
| Default con tareas (ordenadas) | Si | AUSENTE | NO CUBIERTO |
| Empty state (sin tareas activas) | Si | AUSENTE | NO CUBIERTO |
| Error de carga con reintentar | Si | AUSENTE | NO CUBIERTO |
| Enviando completado (spinner en boton) | Si | AUSENTE | NO CUBIERTO |
| Tarea enviada exitosamente (badge cambia a PENDIENTE) | Si | AUSENTE | NO CUBIERTO |
| Error al enviar (toast destructivo) | Si | AUSENTE | NO CUBIERTO |
| Acceso no autorizado 403 (redirect) | Si | AUSENTE | NO CUBIERTO |

### Estados de UI — Pantalla 2 (Dialog Completar Tarea)

| Estado | Requerido (ui-ux.md) | Planificado (frontend-plan) | Estado |
|--------|----------------------|-----------------------------|--------|
| Default (abierto, campos vacios) | Si | AUSENTE | NO CUBIERTO |
| Campo URL invalido (borde rojo, mensaje error) | Si | AUSENTE | NO CUBIERTO |
| Enviando (spinner, dialog no cierra) | Si | AUSENTE | NO CUBIERTO |
| Exito (dialog cierra, card actualiza, toast) | Si | AUSENTE | NO CUBIERTO |
| Error de API (dialog permanece, toast) | Si | AUSENTE | NO CUBIERTO |

### Variantes de Boton en TareaCard

| Variante | Requerida (ui-ux.md) | Planificada | Estado |
|----------|----------------------|-------------|--------|
| No completada: "Completar tarea" (gradient) | Si | AUSENTE | NO CUBIERTO |
| Repetible con cupo: "Completar de nuevo" (gradient) | Si | AUSENTE | NO CUBIERTO |
| Rechazada: "Re-enviar con nueva prueba" (outline purple) | Si | AUSENTE | NO CUBIERTO |
| Completada pendiente: "Ver prueba enviada" (ghost, abre URL) | Si | AUSENTE | NO CUBIERTO |
| No repetible validada: Sin boton (solo badge) | Si | AUSENTE | NO CUBIERTO |
| Max repeticiones alcanzado: Icono Lock + texto | Si | AUSENTE | NO CUBIERTO |
| Programa inactivo: Icono Lock + texto | Si | AUSENTE | NO CUBIERTO |
| Tarea con fecha fin pasada: Icono Calendar + texto | Si | AUSENTE | NO CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear `frontend-plan.md` para Landing**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/frontend-plan.md`
   - Cambio: Documentar los siguientes elementos:
     - **Ruta nueva:** `/promotor/programas/:programaId/tareas` protegida por autenticacion de promotor
     - **Pagina:** `MisTareasPage` que consume hook `useMisTareas(programaId)`
     - **Hook `useMisTareas`:** `useQuery` con `QUERY_KEYS.crowdpromotion.tareas.mis(programaId)` llamando a `GET /api/crowdpromotion/programas/{programaId}/mis-tareas`. Manejo de error 403 con redirect a `/promotor/mis-programas`
     - **Hook `useCompletarTarea`:** `useMutation` llamando a `POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar`. Invalida `QUERY_KEYS.crowdpromotion.tareas.mis(programaId)` en exito. Usa `getTareaPromocionErrorMessage` para mensajes de error
     - **Componente `TareaCard`:** recibe `MisTareasItem`. Renderiza badge de estado, boton condicional segun `puedeCompletarTarea()`, bloque de motivo de rechazo si `estadoTareaId == 4`, bloque de prueba enviada si `estadoTareaId == 2`
     - **Componente `CompletarTareaDialog`:** prop `contexto: 'completar' | 'repetir' | 'reenviar'`. Formulario con `react-hook-form` + `completarTareaSchema`. Contador de caracteres para comentario. Estado loading bloquea cierre del dialog
     - **Logica de ordenacion local:** no completadas/rechazadas primero, luego completadas, luego validadas (implementar en `useMisTareas` o en el componente)

2. **Crear `ui-design.md` para Landing**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/ui-design.md`
   - Cambio: Consolidar y adaptar las especificaciones de Pantalla 1 y Pantalla 2 del documento `docs/user-stories/cp-tareas-promocion/ui-ux.md`. Incluir:
     - Componentes shadcn/ui utilizados: `Card`, `Badge`, `Button`, `Dialog`, `DialogContent`, `DialogTitle`, `Form`, `FormField`, `FormLabel`, `FormControl`, `FormMessage`, `Input`, `Textarea`, `Separator`
     - Tokens de color para estados de tarea: `--status-completada-*`, `--status-validada-*`, `--status-rechazada-*`, `--status-no-completada-*`
     - Las 8 variantes de boton de accion por estado de tarea
     - Responsive: mobile bottom sheet para dialog, full width para cards en mobile
     - Checklist de accesibilidad (WCAG AA, focus trap, aria labels)

3. **Crear `test-strategy.md` para Landing**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/test-strategy.md`
   - Cambio: Documentar al menos:
     - 12 tests unitarios para `TareaCard` (uno por cada estado/variante)
     - 4 tests de integracion para `useMisTareas` y `useCompletarTarea` con mocks de API
     - Validacion del schema `completarTareaSchema` (URL obligatoria, formato valido, comentario max 500)
     - 1 test E2E del flujo completo: promotor inicia sesion, ve lista de tareas, hace click en "Completar tarea", llena el formulario, envia y ve el badge cambiar a "PENDIENTE"

### Acciones Sugeridas (Mayor)

4. **Documentar consumo de shared en frontend-plan.md**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/frontend-plan.md`
   - Cambio: Agregar tabla de dependencias explicita de shared: importar `MisTareasItem`, `MiEstadoTarea`, `CompletarTareaRequest`, `CompletarTareaResponse`, `completarTareaSchema`, `puedeCompletarTarea`, `getTareaPromocionErrorMessage`, `QUERY_KEYS.crowdpromotion.tareas.mis`, `API_ROUTES.crowdpromotion.programas.misTareas`, `ESTADO_TAREA_PROMO`

5. **Planificar logica de toast por codigo de error**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/frontend-plan.md`
   - Cambio: Documentar en la seccion del hook `useCompletarTarea` el mapeo de `errorCode` a mensaje de toast usando `getTareaPromocionErrorMessage`. Los codigos criticos son: 4027 (no repetible), 4028 (max repeticiones), 4024 (programa inactivo), 403 (no aprobado)

6. **Documentar ordenacion del listado de tareas**
   - Archivo: `plans/cp-tareas-promocion/frontend-landing/frontend-plan.md`
   - Cambio: Especificar el algoritmo de ordenacion local del array `items` de `MisTareasResponse`. Orden: `estadoTareaId == null` (sin completar) o `== 4` (rechazadas) primero, luego `== 2` (pendientes), luego `== 3` (validadas). Tareas dentro del mismo grupo ordenadas por `orden` del campo de la tarea

### Nice to Have (Menor)

7. **Documentar animaciones en ui-design.md**
   - Referenciar la tabla de animaciones del `ui-ux.md` (transicion de badge 300ms, hover de card 200ms, bottom sheet en mobile)

8. **Planificar empty state y error state**
   - Documentar en `frontend-plan.md` los componentes de estado vacio (icono `ClipboardX`, texto especifico) y estado de error (icono `AlertCircle`, boton reintentar) para `MisTareasPage`

9. **Agregar nota sobre skip-to-content**
   - El `ui-ux.md` especifica `<a href="#main-content" className="sr-only focus:not-sr-only">` en el layout de landing. Documentar en `ui-design.md` que este elemento ya debe existir en el layout compartido o que debe agregarse para esta ruta

---

## 8. Checklist de Validacion

### Planes de Implementacion (Pre-requisito)

- [ ] `frontend-plan.md` creado con componentes, hooks y servicios
- [ ] `ui-design.md` creado con especificaciones de UI para Pantalla 1 y Pantalla 2
- [ ] `test-strategy.md` creado con cobertura de 80%+

### Requisitos Funcionales (pendientes de plan)

- [ ] Listado de tareas activas (AC-CP04-1) planificado como componente
- [ ] Formulario de completado (AC-CP04-2) planificado con validacion Zod
- [ ] UI condicional para repetibles (AC-CP04-4) planificada en TareaCard
- [ ] Bloqueo visual cuando MaxRepeticiones alcanzado (AC-CP04-6)
- [ ] Motivo de rechazo visible en card (AC-CP04-9)
- [ ] Boton re-envio para estado Rechazada (AC-CP04-9, FA-05)
- [ ] Consumo de tipos shared documentado (AC-CP04-14)

### UI/UX (pendientes de plan)

- [ ] Pantalla 1 (Mis Tareas) con todos los estados: loading, empty, error, default, enviando, exito
- [ ] Las 8 variantes de boton segun estado de tarea planificadas
- [ ] Pantalla 2 (Dialog Completar) con sus 5 estados de UI
- [ ] Responsive breakpoints planificados (mobile, tablet, desktop)
- [ ] Comportamiento bottom sheet del dialog en mobile documentado
- [ ] Accesibilidad WCAG AA planificada (focus trap, aria labels, contraste)

### Testing (pendiente de plan)

- [ ] Tests unitarios para TareaCard (min 12)
- [ ] Tests de integracion para hooks con mock API
- [ ] Validacion de schema Zod (URL formato, max chars)
- [ ] Test E2E del flujo completo del promotor
- [ ] Cobertura objetivo 80% planificada

---

## 9. Analisis de lo que SI esta Cubierto

A pesar de la ausencia de planes frontend, los documentos de especificacion estan bien definidos y facilitan la creacion de los planes pendientes. Se destaca:

| Elemento | Donde esta cubierto | Calidad |
|----------|---------------------|---------|
| Tipos TypeScript de Landing | `contracts-plan.md` seccion 2 | Completo: 8 interfaces, 1 union type, alineamiento con backend documentado |
| Schemas Zod de validacion | `contracts-plan.md` seccion 3 | Completo: `completarTareaSchema` con todas las reglas alineadas con backend |
| Endpoints y query keys | `contracts-plan.md` seccion 4 | Completo: `misTareas`, `completarTarea` en `API_ROUTES` y `QUERY_KEYS` |
| Logica de negocio `puedeCompletarTarea` | `contracts-plan.md` seccion 5 | Completo: encapsula RN-04 y RN-05 |
| Mensajes de error por codigo | `contracts-plan.md` seccion 5 | Completo: 7 codigos nuevos con mensajes en espanol |
| Especificacion UI de pantallas 1-2 | `ui-ux.md` secciones "Pantalla 1" y "Pantalla 2" | Muy completo: layout, componentes, estilos Tailwind, estados, toasts, interacciones, responsive |
| Contratos API (GET mis-tareas, POST completar) | `contracts.md` | Completo: request, response, errores, logica de negocio |

---

## 10. Conclusion

**Score Final: 0% de cobertura en planes de implementacion**

**Veredicto: RECHAZADO**

Los documentos de especificacion (feature-spec.md, contracts.md, ui-ux.md) estan completos y son de alta calidad. El plan compartido (contracts-plan.md) esta correctamente elaborado. Sin embargo, los tres planes de implementacion especificos para Landing no existen:

- `frontend-plan.md` — AUSENTE
- `ui-design.md` — AUSENTE
- `test-strategy.md` — AUSENTE

**Proximo Paso:** Crear los tres planes de Landing antes de iniciar la implementacion. Los gaps identificados en la seccion 7 proporcionan el contenido minimo requerido para cada plan. La documentacion de spec existente es suficientemente detallada para que el implementador pueda redactar los planes sin ambiguedades adicionales.

**Estimacion de esfuerzo para cerrar gaps criticos (GAP-01 a GAP-06):**

| Plan a crear | Complejidad | Referencia principal |
|--------------|-------------|---------------------|
| `frontend-plan.md` | Media (2 paginas/componentes, 2 hooks, 1 servicio) | `docs/user-stories/cp-tareas-promocion/contracts.md` + `contracts-plan.md` |
| `ui-design.md` | Baja (specs ya en `ui-ux.md`, solo consolidar) | `docs/user-stories/cp-tareas-promocion/ui-ux.md` Pantallas 1-2 |
| `test-strategy.md` | Media (17 tests a planificar) | `feature-spec.md` criterios de aceptacion + `ui-ux.md` estados de UI |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-01
