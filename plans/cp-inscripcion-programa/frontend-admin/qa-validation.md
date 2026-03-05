# Validacion QA: cp-inscripcion-programa (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/admin

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos evaluados | 7 |
| Cubiertos (solo shared) | 2 |
| Parcialmente Cubiertos | 3 |
| No Cubiertos (Admin especifico) | 2 |
| **Score de Cobertura** | **29%** |

**Estado:** RECHAZADO

> **Nota critica de contexto:** El unico plan disponible en el momento de esta validacion es `plans/cp-inscripcion-programa/shared/contracts-plan.md`. No existen aun los planes especificos de Admin (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`). La validacion evalua los criterios Admin contra el contrato shared como base, identificando todo lo que aun debe planificarse.

---

## 2. Criterios de Aceptacion Evaluados

### Fuente: feature-spec.md (criterios con impacto en Admin)

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|----------|
| AC-CP03-7 | El artista ve la lista de solicitudes pendientes en el detalle de cada programa de su propiedad | Funcional | Backend + Admin |
| AC-CP03-8 | Al aprobar, el backend genera CodigoReferido con patron {CodigoTrackingBase}-{ShortId} y UrlTrackingPersonalizada con parametros UTM; ambos se persisten en BD | Funcional | Backend |
| AC-CP03-9 | El CodigoReferido generado es unico entre todas las inscripciones | Funcional | Backend |
| AC-CP03-11 | El artista puede rechazar una solicitud pendiente; el registro se elimina fisicamente de BD | Funcional | Backend + Admin |
| AC-CP03-12 | El artista puede bloquear un promotor; EsBloqueado = true; promotor no puede re-solicitar | Funcional | Backend + Admin |
| AC-CP03-13 | El artista puede dar de baja a un promotor aprobado; FechaBaja = now y EsAprobado = false | Funcional | Backend + Admin |
| AC-CP03-16 | Los tipos y schemas Zod de inscripcion estan definidos en shared y son reutilizados por landing y admin | Funcional | Shared |

### Criterios no funcionales con impacto en Admin

| ID | Criterio | Tipo |
|----|----------|------|
| RNF-03 | CodigoReferido y UrlTrackingPersonalizada solo deben devolverse en endpoints autenticados al promotor propietario o al artista del programa | No Funcional |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | contracts-plan (shared) | frontend-plan (Admin) | ui-design (Admin) | test-strategy (Admin) | Estado |
|----|----------|------------------------|-----------------------|-------------------|-----------------------|--------|
| AC-CP03-7 | Artista ve lista de solicitudes pendientes | `InscripcionListItem`, `InscripcionesListResponse`, `InscripcionesFilters`, `QUERY_KEYS.programas.inscripciones`, `API_ROUTES.programas.inscripciones`, `APP_ROUTES.dashboard.crowdpromotion.programas.inscripciones` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP03-8 | Backend genera CodigoReferido y UrlTrackingPersonalizada al aprobar | `InscripcionAprobada` (response type con `codigoReferido` y `urlTrackingPersonalizada`), `API_ROUTES.programas.aprobar` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP03-9 | CodigoReferido unico entre inscripciones | No aplica al frontend shared; ninguna cobertura en contratos | NO EXISTE | NO EXISTE | NO EXISTE | NO CUBIERTO |
| AC-CP03-11 | Artista rechaza solicitud; eliminacion fisica | `InscripcionRechazada` (response type), `API_ROUTES.programas.rechazar`, `INSCRIPCION_ERROR_MESSAGES['4025']` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP03-12 | Artista bloquea promotor; EsBloqueado = true | `InscripcionBloqueada` (response type), `API_ROUTES.programas.bloquear`, `INSCRIPCION_ESTADO.BLOQUEADO`, `INSCRIPCION_ESTADO_BADGE_VARIANT.Bloqueado` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP03-13 | Artista da de baja a promotor aprobado | `InscripcionDadaDeBaja` (response type), `API_ROUTES.programas.darDeBaja`, `INSCRIPCION_ESTADO.DADO_DE_BAJA`, `getInscripcionBadgeVariant` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP03-16 | Tipos y schemas Zod en shared reutilizados por admin | Completamente cubierto: 10 interfaces, 1 union type, 2 schemas Zod, constantes de dominio, mappers. Los tipos de Admin (`InscripcionListItem`, `InscripcionesListResponse`, `InscripcionesFilters`, `InscripcionAprobada`, `InscripcionRechazada`, `InscripcionBloqueada`, `InscripcionDadaDeBaja`) estan en shared | NO EXISTE | NO EXISTE | NO EXISTE | CUBIERTO |

**Leyenda:**
- CUBIERTO: Requisito completamente implementado en plan
- PARCIAL: Requisito parcialmente cubierto (shared da la base, falta plan Admin especifico)
- NO CUBIERTO: Requisito no mencionado en ningun plan

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| RNF-03 | CodigoReferido/UrlTracking solo en endpoints autenticados para propietario/artista | `INSCRIPCION_ERROR_MESSAGES['4026']` cubre el mensaje de error de no-propietario. La aplicacion de autenticacion en Admin es responsabilidad del `frontend-plan.md` (rutas protegidas, manejo del 403). Sin ese plan la cobertura es parcial. | PARCIAL |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-C01 | AC-CP03-7 | No existe `frontend-plan.md` para Admin. No hay componentes planificados para la pestana "Solicitudes pendientes" (lista `InscripcionListItem`, filtros por estado, cards de promotor con datos de contacto y redes sociales) | Alto: sin esto no hay guia de implementacion para el desarrollador | Crear `plans/cp-inscripcion-programa/frontend-admin/frontend-plan.md` con componentes: `InscripcionesTab`, `InscripcionListItem` card, `InscripcionEstadoBadge`, `InscripcionesFiltersBar`, hook `useInscripcionesPendientes` |
| GAP-C02 | AC-CP03-11, AC-CP03-12, AC-CP03-13 | No existe `ui-design.md` para Admin. Los flujos de confirmacion para acciones destructivas (rechazar = eliminacion fisica, bloquear = permanente, dar de baja = irreversible) no estan diseñados. La feature-spec exige dialogo de confirmacion para "dar de baja" (seccion "Flujo Secundario: Dar de Baja"). Las acciones de rechazar y bloquear tambien son destructivas y deberian tener confirmacion. | Alto: sin disenyo no se sabe que dialogo usar, que texto mostrar, si rechazar y bloquear tambien necesitan confirmacion, ni que toast mostrar en cada caso | Crear `plans/cp-inscripcion-programa/frontend-admin/ui-design.md` con: dialogo de confirmacion para cada accion destructiva (texto especifico por accion, variant de boton), estados del card de inscripcion segun estado, toasts de resultado |
| GAP-C03 | AC-CP03-7, AC-CP03-11, AC-CP03-12, AC-CP03-13 | No existe `test-strategy.md` para Admin. Ningun criterio Admin tiene tests planificados. | Alto: sin tests la feature puede entrar con regresiones. AC-CP03-11 (eliminacion fisica) y AC-CP03-12 (bloqueo permanente) son especialmente criticos | Crear `plans/cp-inscripcion-programa/frontend-admin/test-strategy.md` con tests de integracion para flujos de aprobacion, rechazo, bloqueo y baja, y tests unitarios para hooks de mutacion |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-M01 | AC-CP03-7 | El contrato `InscripcionListItem` incluye `promotorEmailContacto`, `promotorUrlInstagram`, `promotorUrlTikTok`, `promotorUrlSitioWeb` (datos del perfil del promotor), pero no existe plan de componente que defina como renderizarlos en la UI del artista. La feature-spec especifica: "Artista ve el perfil del promotor (nombre publico, tipo, redes sociales)" | Medio: el desarrollador puede implementarlo de forma inconsistente con el resto del admin si no hay guia | En `frontend-plan.md` Admin, definir componente `PromotorProfileCard` que renderice nombre, tipo, email de contacto y links a redes sociales con iconos |
| GAP-M02 | AC-CP03-8 | Los tipos `InscripcionAprobada.urlTrackingPersonalizada` se declaran `string | null` en shared porque `PromoPrograma.UrlLanding` puede ser null (FA-05). Pero no hay plan de como el Admin debe mostrar/comunicar al artista el resultado de la aprobacion, incluyendo este caso edge. | Medio: el artista podria confundirse si ve que la URL es null tras aprobar | En `ui-design.md` Admin, especificar el toast de aprobacion exitosa y el mensaje condicional cuando `urlTrackingPersonalizada` es null (ej: "Promotor aprobado. La URL de tracking estara disponible cuando el programa vuelva a estar activo") |
| GAP-M03 | AC-CP03-9 | La unicidad del CodigoReferido es responsabilidad del backend (RN-09), pero el frontend Admin no tiene plan de manejo de error si la generacion falla (caso extremadamente raro pero posible). El codigo de error que corresponderia tampoco esta en `INSCRIPCION_ERROR_MESSAGES`. | Medio: sin manejo, el artista veria un error generico poco descriptivo | Agregar codigo de error especifico para fallo de generacion de codigo (ej: `'5021'`) en `INSCRIPCION_ERROR_MESSAGES` y planificar el toast de error en el hook `useAprobarInscripcion` |
| GAP-M04 | AC-CP03-7 | El plan shared define `InscripcionesFilters` con campo `estado?: InscripcionEstado`. Pero el artista necesita ver dos vistas distintas: "Solicitudes pendientes" (estado Pendiente) y "Promotores aprobados" (estado Aprobado). No hay plan de como se estructura la pestana (tabs, filtro, o paginas separadas). | Medio: la navegacion entre las dos vistas es un detalle de diseno critico para la UX del artista | En `ui-design.md` Admin, definir si se usa un componente `<Tabs>` de shadcn con dos tabs ("Solicitudes" y "Aprobados") o un filtro por `estado` sobre una sola lista |
| GAP-M05 | RNF-03 | El plan shared define `APP_ROUTES.dashboard.crowdpromotion.programas.inscripciones` pero no hay plan de middleware/guard de autenticacion en Next.js para esta ruta. El Admin usa Next.js 14 App Router con grupos `(auth)` y `(dashboard)`. La ruta de inscripciones debe quedar bajo `(dashboard)` para heredar el guard. | Medio: si se olvida proteger la ruta, el endpoint de inscripciones podria ser accesible sin autenticacion en el frontend | En `frontend-plan.md` Admin, especificar que la pagina de inscripciones se ubica en `app/(dashboard)/crowdpromotion/programas/[id]/inscripciones/page.tsx` |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-m01 | AC-CP03-13 | La feature-spec define explicitamente el flujo de "Dar de baja": paso 3 es "Sistema muestra dialogo de confirmacion". El contracts-plan no documenta el texto del dialogo ni el label del boton de confirmacion. | Bajo: el texto puede decidirse en implementacion, pero es mejor definirlo antes para consistencia | En `ui-design.md` Admin, incluir tabla de textos para cada dialogo: titulo, descripcion, label boton confirmar, label boton cancelar |
| GAP-m02 | AC-CP03-11 | El tipo `InscripcionRechazada` incluye `inscripcionId` y `promotorNombre`. No hay plan de que informacion mostrar en el toast tras el rechazo exitoso (ej: "Solicitud de {promotorNombre} rechazada"). | Bajo: impacto en UX, no en funcionalidad | En `ui-design.md` Admin, especificar el mensaje del toast para cada accion usando los campos del response type |
| GAP-m03 | AC-CP03-16 | El contracts-plan define `InscripcionesFiltersData` como type inferido del schema. No documenta si el formulario de filtros en Admin debe usar `react-hook-form` + `zodResolver` o filtros controlados simples. Para un filtro de una sola select (estado), `react-hook-form` es overhead. | Bajo: decision de implementacion, no afecta requisito | En `frontend-plan.md` Admin, aclarar que el filtro de estado en la lista de inscripciones usa estado local simple (`useState`) en lugar de `react-hook-form` dado que es un unico campo |
| GAP-m04 | AC-CP03-7 | El contrato `QUERY_KEYS.programas.inscripciones` incluye `filters` en la cache key. Pero la tabla de invalidacion del contracts-plan (seccion 8) no documenta que tras aprobar/rechazar/bloquear/dar-de-baja se debe invalidar tambien `crowdpromotion.programas.byId(programaId)` (el summary de promotores aprobados en el detalle del programa). Esto si esta en la nota de la seccion 8 pero no en la invalidacion explicita para todos los casos. | Bajo: si se olvida la invalidacion de `byId`, el contador de promotores en el header del programa no se actualiza | En `frontend-plan.md` Admin, incluir tabla de invalidacion de queries por mutacion, siguiendo la nota de la seccion 8 del contracts-plan |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-CP03-7 | - | - | NO CUBIERTO |
| AC-CP03-8 | - | - | NO CUBIERTO |
| AC-CP03-9 | - | - | NO CUBIERTO |
| AC-CP03-11 | - | - | NO CUBIERTO |
| AC-CP03-12 | - | - | NO CUBIERTO |
| AC-CP03-13 | - | - | NO CUBIERTO |
| AC-CP03-16 | - | - | NO CUBIERTO |

**No existe `test-strategy.md` para Admin.** Todos los criterios carecen de cobertura de tests planificada.

### Tests Requeridos (a definir en test-strategy.md)

| Criterio | Test Requerido | Tipo | Razon |
|----------|----------------|------|-------|
| AC-CP03-7 | `useInscripciones - fetches pending list successfully` | Unit (hook) | Validar que el hook llama al endpoint correcto y mapea `InscripcionListItem[]` |
| AC-CP03-7 | `InscripcionesTab - renders promotor contact data` | Integration | Validar que nombre, tipo, email y redes sociales del promotor se muestran |
| AC-CP03-8 | `useAprobarInscripcion - on success shows codigoReferido` | Unit (hook) | Validar respuesta `InscripcionAprobada` con codigoReferido no-null |
| AC-CP03-8 | `useAprobarInscripcion - handles null urlTrackingPersonalizada` | Unit (hook) | Validar mensaje especifico cuando urlTracking es null (FA-05) |
| AC-CP03-11 | `useRechazarInscripcion - on success invalidates cache` | Unit (hook) | Validar invalidacion de `programas.inscripciones` y `programas.byId` |
| AC-CP03-11 | `RechazarDialog - confirms before submitting` | Integration | Validar que el dialogo de confirmacion bloquea la accion sin interaccion |
| AC-CP03-12 | `useBloquearInscripcion - on success badge shows Bloqueado` | Integration | Validar que el badge cambia a variant `destructive` tras bloquear |
| AC-CP03-12 | `BloquearDialog - shows permanent warning` | Unit | Validar texto que advierte que el bloqueo es permanente |
| AC-CP03-13 | `useDarDeBajaInscripcion - calls darDeBaja endpoint` | Unit (hook) | Validar la mutacion llama `API_ROUTES.programas.darDeBaja` |
| AC-CP03-13 | `DarDeBajaDialog - shows confirmation before submit` | Integration | Validar flujo completo del dialogo de confirmacion |
| AC-CP03-16 | `InscripcionEstadoBadge - uses shared getInscripcionBadgeVariant` | Unit | Validar que el componente delega en el mapper shared |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Especificacion | Planificada | Componentes | Estado |
|------------------|---------------|-------------|-------------|--------|
| Pestana "Solicitudes pendientes" en detalle de programa | feature-spec: Flujo Secundario Artista, paso 2 | No | - | NO CUBIERTO |
| Lista de promotores aprobados con accion "Dar de baja" | feature-spec: Flujo Dar de Baja, paso 1 | No | - | NO CUBIERTO |
| Dialogo de confirmacion "Dar de baja" | feature-spec: Flujo Dar de Baja, pasos 3-4 | No | - | NO CUBIERTO |
| Dialogo de confirmacion "Rechazar" | Accion destructiva (eliminacion fisica) | No | - | NO CUBIERTO |
| Dialogo de confirmacion "Bloquear" | Accion destructiva (bloqueo permanente) | No | - | NO CUBIERTO |
| Toast de resultado por accion | feature-spec: paso 5 en Flujo Artista | No | - | NO CUBIERTO |

### Estados de UI para la lista de inscripciones

| Estado | Requerido | Especificacion | Planificado | Estado |
|--------|-----------|----------------|-------------|--------|
| Loading (cargando inscripciones) | Si | UX estandar | No | NO CUBIERTO |
| Empty state "no hay solicitudes pendientes" | Si | UX estandar | No | NO CUBIERTO |
| Error (fallo de red o 403) | Si | RNF-03 implica 403 posible | No | NO CUBIERTO |
| Card de inscripcion con estado Pendiente | Si | Flujo Artista paso 3 | Parcial (tipos en shared) | PARCIAL |
| Card de inscripcion con estado Aprobado + boton Dar de Baja | Si | Flujo Dar de Baja paso 1 | Parcial (tipos en shared) | PARCIAL |
| Card de inscripcion con estado Bloqueado (solo visual) | Si | `INSCRIPCION_ESTADO_BADGE_VARIANT.Bloqueado = 'destructive'` en shared | Parcial (constante en shared) | PARCIAL |
| Badge con color por estado usando shadcn Badge variant | Si | `INSCRIPCION_ESTADO_BADGE_VARIANT` definido en shared | Cubierto en shared | CUBIERTO |
| Boton "Aprobar" activo solo para estado Pendiente | Si | Logica de negocio | No | NO CUBIERTO |
| Boton "Rechazar" activo solo para estado Pendiente | Si | Logica de negocio | No | NO CUBIERTO |
| Boton "Bloquear" activo solo para estado Pendiente | Si | Logica de negocio (RN-07) | No | NO CUBIERTO |
| Boton "Dar de baja" activo solo para estado Aprobado | Si | Flujo Dar de Baja | No | NO CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear frontend-plan.md para Admin**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/frontend-plan.md`
   - Cambio: Planificar los siguientes elementos:
     - Pagina `app/(dashboard)/crowdpromotion/programas/[id]/inscripciones/page.tsx` (Next.js App Router, ruta protegida bajo grupo `(dashboard)`)
     - Componente `InscripcionesTab` con dos secciones: "Solicitudes pendientes" y "Promotores aprobados" (tabs o filtro por estado usando `InscripcionesFilters`)
     - Componente `InscripcionListItemCard` que renderiza `InscripcionListItem`: nombre del promotor, tipo, contacto, redes sociales, badge de estado usando `getInscripcionBadgeVariant` de shared, y botones de accion condicionales por estado
     - Hook `useInscripciones(programaId, filters?)` usando `QUERY_KEYS.programas.inscripciones` y `API_ROUTES.programas.inscripciones`
     - Hook `useAprobarInscripcion(programaId)` con invalidacion de `programas.inscripciones(programaId)` y `programas.byId(programaId)`
     - Hook `useRechazarInscripcion(programaId)` con invalidacion identica
     - Hook `useBloquearInscripcion(programaId)` con invalidacion identica
     - Hook `useDarDeBajaInscripcion(programaId)` con invalidacion identica
     - Componente `ConfirmacionDialog` generico reutilizable o especificos por accion

2. **Crear ui-design.md para Admin**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/ui-design.md`
   - Cambio: Disenar los siguientes elementos:
     - Layout de la pestana de inscripciones dentro de la pagina de detalle del programa (donde se integra respecto a las otras pestanas)
     - Estructura visual del `InscripcionListItemCard`: avatar/icono, nombre, tipo, email, iconos de redes sociales como links, badge de estado, grupo de botones de accion
     - Dialogo de confirmacion para "Rechazar": titulo "Rechazar solicitud", descripcion "Esta accion eliminara la solicitud de {promotorNombre}. El promotor podra volver a solicitar en el futuro.", boton confirmar variant=`destructive` label="Rechazar", boton cancelar label="Cancelar"
     - Dialogo de confirmacion para "Bloquear": titulo "Bloquear promotor", descripcion "Esta accion bloqueara permanentemente a {promotorNombre} en este programa. No podra volver a solicitar inscripcion.", boton confirmar variant=`destructive` label="Bloquear", boton cancelar label="Cancelar"
     - Dialogo de confirmacion para "Dar de baja": titulo "Dar de baja", descripcion "Se desactivara el acceso de {promotorNombre} a este programa. El codigo referido quedara inactivo.", boton confirmar variant=`destructive` label="Dar de baja", boton cancelar label="Cancelar"
     - Toasts de resultado: mensaje especifico por accion usando `promotorNombre` del response type
     - Toast especial para aprobacion cuando `urlTrackingPersonalizada` es null (ver GAP-M02)
     - Estado empty para lista sin solicitudes pendientes
     - Estado de error (403 y error de red)

3. **Crear test-strategy.md para Admin**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/test-strategy.md`
   - Cambio: Planificar al menos los 11 tests listados en la seccion 5 de este informe. Especial atencion a tests de integracion para los dialogos de confirmacion de acciones destructivas (AC-CP03-11, AC-CP03-12, AC-CP03-13).

### Acciones Sugeridas (Mayor)

4. **Planificar componente PromotorProfileCard**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/frontend-plan.md`
   - Cambio: Agregar componente `PromotorProfileCard` o seccion dentro de `InscripcionListItemCard` que renderice los 4 campos de contacto del promotor (`promotorEmailContacto`, `promotorUrlInstagram`, `promotorUrlTikTok`, `promotorUrlSitioWeb`) con iconos de cada red social y manejo de null para campos opcionales (ver GAP-M01)

5. **Definir estructura de pestana (tabs vs filtro)**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/ui-design.md`
   - Cambio: Decidir y documentar si la vista de inscripciones del artista usa un componente `<Tabs>` de shadcn con "Solicitudes pendientes" y "Promotores aprobados" como tabs separados, o un unico listado con filtro por `estado` (ver GAP-M04). La opcion de tabs es mas clara para el artista segun el flujo descrito en feature-spec.

6. **Agregar codigo de error para fallo de generacion de CodigoReferido**
   - Archivo: `plans/cp-inscripcion-programa/shared/contracts-plan.md` o directamente en implementacion de shared
   - Cambio: Agregar codigo `'5021'` con mensaje "Error al generar el codigo referido. Intente de nuevo." en `INSCRIPCION_ERROR_MESSAGES` para cubrir el caso edge de colision de CodigoReferido (RN-09, ver GAP-M03)

7. **Documentar invalidacion completa de queries en frontend-plan.md Admin**
   - Archivo: `plans/cp-inscripcion-programa/frontend-admin/frontend-plan.md`
   - Cambio: Incluir tabla de invalidacion de queries por mutacion replicando y expandiendo la tabla de la seccion 8 del contracts-plan, asegurando que `programas.byId(programaId)` se invalida en todas las mutaciones de inscripcion (ver GAP-m04)

### Nice to Have (Menor)

8. **Definir textos exactos de todos los toasts en ui-design.md**
   - Especificar formato: "{NombrePromotor} aprobado. Codigo referido: {codigoReferido}" para aprobar; "{NombrePromotor} rechazado" para rechazar; "{NombrePromotor} bloqueado en este programa" para bloquear; "{NombrePromotor} dado de baja" para dar de baja.

9. **Aclarar uso de react-hook-form para filtros**
   - En `frontend-plan.md` Admin, documentar que el filtro de estado usa `useState` simple en lugar de `react-hook-form` dado que es un unico campo select (ver GAP-m03)

10. **Considerar estado de carga por item (loading por boton)**
    - En `ui-design.md` Admin, considerar mostrar estado de carga por accion individual (el boton "Aprobar" de un item especifico muestra spinner mientras la mutacion esta en vuelo) en lugar de deshabilitar toda la lista. Esto mejora la UX cuando hay multiples inscripciones.

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [x] Contratos shared (tipos, schemas, constantes, mappers) definidos para Admin
- [ ] Componentes Admin para lista de inscripciones planificados
- [ ] Hooks de mutacion (aprobar, rechazar, bloquear, dar-de-baja) planificados
- [ ] Flujos de usuario completos para el artista planificados
- [ ] Casos de error (403, 4025, red) cubiertos en plan de componentes

### UI/UX
- [ ] Pestana "Solicitudes pendientes" disenada
- [ ] Pestana o filtro "Promotores aprobados" disenado
- [ ] Dialogos de confirmacion para las 3 acciones destructivas disenados con textos especificos
- [ ] Estados de interaccion definidos (loading, error, empty, exito) para cada accion
- [x] Variantes de Badge por estado definidas en shared (`INSCRIPCION_ESTADO_BADGE_VARIANT`)
- [ ] Responsive design considerado para la lista de inscripciones en pantallas pequenas
- [ ] Accesibilidad de los dialogos de confirmacion validada (foco en boton de cancelar al abrir)

### Testing
- [ ] Tests para criterios criticos planificados (AC-CP03-11, AC-CP03-12, AC-CP03-13)
- [ ] Tests de integracion para flujos de confirmacion de dialogo
- [ ] Tests unitarios para hooks de mutacion con verificacion de invalidacion de cache
- [ ] Cobertura objetivo definida para modulo de inscripciones Admin

---

## 9. Analisis Detallado: Cobertura de Shared para Admin

El `contracts-plan.md` cubre correctamente la capa de contratos que el Admin necesita. A continuacion se detalla que esta disponible para reutilizar:

| Elemento Shared | Uso en Admin | Estado |
|-----------------|--------------|--------|
| `InscripcionEstado` union type | Tipar el estado en todos los componentes Admin | Disponible |
| `InscripcionListItem` | Tipo del item en la lista de solicitudes del artista | Disponible |
| `InscripcionesListResponse` | Tipo de respuesta del endpoint GET /inscripciones | Disponible |
| `InscripcionesFilters` | Query params para filtrar inscripciones por estado | Disponible |
| `InscripcionAprobada` | Tipo de respuesta al aprobar (incluye codigoReferido) | Disponible |
| `InscripcionRechazada` | Tipo de respuesta al rechazar | Disponible |
| `InscripcionBloqueada` | Tipo de respuesta al bloquear | Disponible |
| `InscripcionDadaDeBaja` | Tipo de respuesta al dar de baja | Disponible |
| `inscripcionesFiltersSchema` | Schema Zod para validar filtros si se usa react-hook-form | Disponible |
| `QUERY_KEYS.programas.inscripciones` | Cache key para lista de inscripciones del artista | Disponible |
| `API_ROUTES.programas.aprobar/rechazar/bloquear/darDeBaja` | URLs de los 4 endpoints de gestion | Disponible |
| `APP_ROUTES.dashboard.crowdpromotion.programas.inscripciones` | Ruta de navegacion al tab de inscripciones | Disponible |
| `INSCRIPCION_ESTADO` | Constante de valores de estado (evita strings hardcodeados) | Disponible |
| `INSCRIPCION_ESTADO_LABELS` | Labels en espanol para mostrar en UI | Disponible |
| `INSCRIPCION_ESTADO_BADGE_VARIANT` | Variantes de Badge shadcn por estado | Disponible |
| `INSCRIPCION_ERROR_MESSAGES` | Mensajes de error por codigo para toasts | Disponible |
| `getInscripcionErrorMessage` | Getter con fallback para manejo de errores en hooks | Disponible |
| `mapInscripcionEstado` | Mapper para actualizaciones optimistas en cache | Disponible |
| `getInscripcionBadgeVariant` | Getter del variant del Badge por estado | Disponible |

**Conclusion:** La capa shared esta bien preparada para el Admin. Los gaps son exclusivamente en la capa de planificacion especifica del Admin (componentes, diseno, tests), no en los contratos.

---

## 10. Conclusion

**Score Final: 29%**

Este score refleja que el plan shared (`contracts-plan.md`) cubre correctamente los contratos que el Admin necesita para implementar AC-CP03-16, y provee las bases para AC-CP03-7, AC-CP03-8, AC-CP03-11, AC-CP03-12 y AC-CP03-13. Sin embargo, los tres planes especificos de Admin (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) no existen, lo que deja sin cubrir la implementacion concreta de componentes, el diseno de los dialogos de confirmacion para acciones destructivas, y la estrategia de tests.

**Veredicto:** RECHAZADO

El score bajo no indica problemas en lo que ya existe (el contracts-plan.md es solido y completo para su alcance), sino que refleja la ausencia de los planes especificos de Admin. Esto es esperado en un flujo incremental donde shared se planifica primero. La condicion para pasar a implementacion es generar los tres planes faltantes.

**Proximo Paso:**

El plan esta RECHAZADO. Se deben crear los tres planes de Admin antes de implementar:

1. `plans/cp-inscripcion-programa/frontend-admin/frontend-plan.md` - Componentes, hooks y estructura de paginas
2. `plans/cp-inscripcion-programa/frontend-admin/ui-design.md` - Diseno de UI, dialogos de confirmacion con textos, estados de interaccion
3. `plans/cp-inscripcion-programa/frontend-admin/test-strategy.md` - Tests unitarios e integracion para los 4 flujos del artista

Una vez creados estos tres planes, re-ejecutar esta validacion. El score esperado tras esa iteracion es superior al 90%.

**Gaps a resolver antes del siguiente ciclo de validacion:**
- GAP-C01: frontend-plan.md Admin (Critico)
- GAP-C02: ui-design.md Admin con dialogos de confirmacion (Critico)
- GAP-C03: test-strategy.md Admin (Critico)
- GAP-M01 a GAP-M05: se resuelven como parte de los tres planes anteriores

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-25
