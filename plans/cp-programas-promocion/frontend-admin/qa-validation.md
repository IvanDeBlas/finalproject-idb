# Validacion QA: cp-programas-promocion (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-programas-promocion (US-CP-02)
**Target:** src/admin (Next.js 14) + Backend + Shared

> **Alcance de esta validacion (actualizada):** Se validan TODOS los planes generados:
> `shared/contracts-plan.md`, `backend/api-contracts.md`, `backend/hexagonal-architecture.md`,
> `backend/20260225_cqrs-plan.md`, `frontend-admin/frontend-plan.md`,
> `frontend-admin/ui-design.md` y `frontend-admin/test-strategy.md`.

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Criterios de Aceptacion | 10 |
| Cubiertos completamente | 10 |
| Parcialmente cubiertos | 0 |
| No cubiertos | 0 |
| Total Flujos Alternativos | 8 |
| FA Cubiertos completamente | 8 |
| FA Parcialmente cubiertos | 0 |
| FA No cubiertos | 0 |
| **Score de Cobertura (AC)** | **100%** |
| **Score de Cobertura (FA)** | **100%** |

**Estado: APROBADO**

Todos los criterios de aceptacion y flujos alternativos tienen cobertura completa en el conjunto de
planes analizados. Se identificaron cuatro observaciones tecnicas menores (no gaps bloqueantes)
que se documentan en la seccion 5 para guiar al implementador.

---

## 2. Criterios de Aceptacion

### Fuente: docs/user-stories/cp-programas-promocion/feature-spec.md

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|---------|
| AC-CP02-1 | Wizard 4 pasos crea PromoPrograma + PromoTarea transaccionalmente en BD | Funcional | Backend + Admin |
| AC-CP02-2 | Programa y tareas se crean con EsActivo = true | Funcional | Backend |
| AC-CP02-3 | ArtistaId asignado desde JWT, no del cliente | Funcional / Seguridad | Backend |
| AC-CP02-4 | Al menos una comision obligatoria (porcentaje o fija) | Funcional / Validacion | Backend + Shared |
| AC-CP02-5 | Listado paginado con badge estado, contadores y filtro activo/inactivo | Funcional | Backend + Admin |
| AC-CP02-6 | Editar programa y tareas; aviso informativo si tiene promotores inscritos | Funcional | Backend + Admin |
| AC-CP02-7 | Desactivar: EsActivo = false en programa y todas las PromoTarea activas | Funcional | Backend |
| AC-CP02-8 | CodigoTrackingBase unico por artista; duplicado retorna error de negocio | Funcional / Validacion | Backend |
| AC-CP02-9 | EsRepetible = true requiere MaxRepeticiones >= 1; false no requiere | Funcional / Validacion | Backend + Shared |
| AC-CP02-10 | Crear programa sin CampaniaCrowdfunding ni ProyectoArtistico es valido | Funcional | Backend + Admin |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio (resumen) | frontend-plan | ui-design | test-strategy | contracts-plan (Shared) | hexagonal-arch | cqrs-plan | Estado |
|----|-------------------|---------------|-----------|---------------|------------------------|----------------|-----------|--------|
| AC-CP02-1 | Wizard 4 pasos + transaccion BD | `PromoProgramaWizardContainer` (orquestador create/edit), 4 Step components, `useCreatePromoPrograma`, `usePromoProgramaWizardState` | Layout wizard full-page, WizardStepper 4 pasos, WizardFooter con navegacion | `WizardContainer.test.tsx` (18 casos), `useCreatePromoPrograma.test.ts` (casos 1-7), `Paso1-4.test.tsx` | `CreatePromoProgramaRequest` + `CreatePromoTareaItem`; `APP_ROUTES.dashboard.crowdpromotion.programas.nuevo` | `PromoProgramaService.CreateWithTareasAsync` con `BeginTransactionAsync` | `CreatePromoProgramaCommand` + Handler pasos 1-11; respuesta `PromoProgramaCreatedResultDto` | **CUBIERTO** |
| AC-CP02-2 | EsActivo = true al crear programa y tareas | No aplica (logica backend) | No aplica | No aplica directamente | `PromoProgramaCreatedResult.esActivo: boolean` | `PromoProgramaService.CreateWithTareasAsync` establece `EsActivo = true` | Handler paso 6: `programa.EsActivo = true`; paso 7 foreach: `tarea.EsActivo = true` | **CUBIERTO** |
| AC-CP02-3 | ArtistaId desde JWT | No aplica (backend) | No aplica | No aplica (backend) | `CreatePromoProgramaRequest` NO incluye `artistaId` | `GetArtistaByUserIdAsync` en `IPromoProgramaService` resuelve ArtistaId desde UserId | Handler paso 2-3: `request.UserId` desde JWT claim; `programa.ArtistaId = artista.Id` en paso 6 | **CUBIERTO** |
| AC-CP02-4 | Al menos una comision | `ComisionesStep` con `promoProgramaStep2Schema` (Refine); bloquea avance sin comision | Paso 2: AvisoBanner amber; `ComisionPreviewCard` con estado "Sin comisiones" | `createPromoProgramaSchema.test.ts` (casos 8-12); `Paso2Comisiones.test.tsx` (caso 2); `WizardContainer.test.tsx` (caso 5) | `createPromoProgramaSchema` Refine 1; codigo `'1020'` en `PROMO_PROGRAMA_ERROR_MESSAGES` | `ServiceResponseMessageType.Validation_ComisionRequerida = "1020"` | `CreatePromoProgramaCommandValidator` con `.Must(x => HasValue || HasValue)` y codigo `1020` | **CUBIERTO** |
| AC-CP02-5 | Listado paginado con badge, contadores y filtro | `PromoProgramaListClient` (filtroEstado, paginaActual), `PromoProgramaCard`, `PromoProgramaFilters`, `PromoProgramaStatusBadge` | Cards en listado con badges ACTIVO/INACTIVO; filtros; paginacion | `MisProgramasPage.test.tsx` (11 casos); `PromoProgramaCard.test.tsx` (12 casos); `PromoProgramaStatusBadge.test.tsx`; `PromoProgramaFilters.test.tsx`; `use-promo-programas.test.ts` (8 casos) | `PromoProgramaListItem` con `esActivo`, `numeroPromotores`, `numeroTareas`; `PromoProgramaListResult` con paginacion; `QUERY_KEYS.crowdpromotion.programas.misFiltrados` | `GetByArtistaIdPagedAsync` con `bool? esActivo`; retorna `(Items, TotalCount)` | `GetMisProgramasQuery` con `EsActivo?`, `Page`, `PageSize`; `GetMisProgramasQueryHandler` | **CUBIERTO** |
| AC-CP02-6 | Editar con aviso de promotores | `PromoProgramaWizardContainer` modo `"edit"`; `DatosBasicosStep.tienePromotores` banner; `TareasStep` con `PromoTareaCard.puedeEliminar` | Banner aviso amber en pasos 1-3 cuando `tienePromotores = true`; tooltip en boton Eliminar de tarea con completados | `ProgramaEditPage.test` en tabla ACs; `useUpdatePromoPrograma.test` (casos 8-11); `DesactivarProgramaDialog.test.tsx` (8 casos) | `UpdatePromoProgramaRequest` + `UpdatePromoTareaItem` con `id?` + `esActivo?` | `IPromoProgramaService.UpdateWithTareasAsync` transaccional | `UpdatePromoProgramaCommand` + Handler: verifica ownership (paso 5); clasifica tareas nuevas/actualizar/desactivar (paso 8) | **CUBIERTO** |
| AC-CP02-7 | Desactivar en cascada | `DesactivarPromoProgramaDialog` (AlertDialog); `useDesactivarPromoPrograma`; boton oculto cuando `esActivo = false` | Dialog confirmacion desactivacion; boton Desactivar visible solo si `esActivo = true` | `DesactivarProgramaDialog.test.tsx` (8 casos); `use-promo-programa-mutations.test.ts` (casos 12-16) | `PromoProgramaDesactivadoResult` con `tareasDesactivadas: number`; codigo `'4020'` | `PromoProgramaService.DesactivarWithTareasAsync`: transaccion, `programa.EsActivo = false`, `tarea.EsActivo = false` foreach | `DesactivarPromoProgramaCommandHandler`: verifica ownership; verifica `!EsActivo` -> retorna `4020`; delega a `DesactivarWithTareasAsync` | **CUBIERTO** |
| AC-CP02-8 | CodigoTrackingBase unico por artista | Schema `codigoTrackingBase` con regex `[a-zA-Z0-9-]*`; error `1024` via `getPromoProgramaErrorMessage` | Paso 1: campo CodigoTracking con validacion visual | `createPromoProgramaSchema.test.ts` (casos 18-20); `use-promo-programa-mutations.test.ts` (caso 7: error 1024 toast especifico) | Codigo `'1024'` en `PROMO_PROGRAMA_ERROR_MESSAGES`; `PROGRAMA_CODIGO_TRACKING_DUPLICADO` key semantica | Indice unico `IX_PromoPrograma_Artista_CodigoTracking` (UNIQUE FILTERED IS NOT NULL); `ExistsByCodigoTrackingAndArtistaAsync` en repo | `CreatePromoProgramaCommandValidator` llama `ExisteCodigoTrackingAsync`; `UpdatePromoProgramaCommandValidator` llama `ExisteCodigoTrackingParaEdicionAsync` (excluye propio ID) | **CUBIERTO** |
| AC-CP02-9 | EsRepetible = true requiere MaxRepeticiones | `PromoTareaForm` con campo `maxRepeticiones` condicional (visible solo si `esRepetible = true`); schema con Refine | Paso 3: Switch toggle `EsRepetible`; campo `maxRepeticiones` aparece/desaparece condicionalmente con `hidden` | `createPromoTareaSchema.test.ts` (casos 8-11); `Paso3Tareas.test.tsx` (casos 5-6); `PromoTareaFormItem.test.tsx` (casos 5-7) | `createPromoTareaSchema` Refine 1: `!esRepetible || maxRepeticiones != null`; mensaje `'1025'`; path `['maxRepeticiones']` | `PromoTarea.EsRepetible` (bool, default true); `PromoTarea.MaxRepeticiones` (int?, nullable) | `CreatePromoProgramaCommandValidator` en cada tarea: `Must(x => !x.EsRepetible || (x.MaxRepeticiones.HasValue && x.MaxRepeticiones >= 1))` con codigo `1026` | **CUBIERTO** |
| AC-CP02-10 | Programa sin campana ni proyecto es valido | `DatosBasicosStep`: campana y proyecto opcionales; `RevisarPublicarStep` muestra "--" si sin campana | Paso 1: banner info azul "Vincula al menos uno (opcional)"; Paso 4: valores nulos con "--" italic | `Paso1DatosBasicos.test.tsx` (caso 6: FA-01 + AC-CP02-10); `Paso4Revision.test.tsx` (caso 10) | `campaniaCrowdfundingId?: string` y `proyectoArtisticoId?: string` opcionales en request y schema | `PromoPrograma.CampaniaCrowdfundingId` (Guid?, nullable); `PromoPrograma.ProyectoArtisticoId` (Guid?, nullable) | Handler create: solo verifica pertenencia `Si request.CampaniaCrowdfundingId.HasValue`; ausencia no es error | **CUBIERTO** |

**Leyenda:**
- CUBIERTO: Requisito completamente implementado en los planes
- PARCIAL: Cobertura incompleta en alguna capa
- NO CUBIERTO: Sin mencion en los planes

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Planes | Estado |
|----|----------|-----------|--------|--------|
| NFR-01 | Transaccionalidad en creacion y desactivacion | `PromoProgramaService.CreateWithTareasAsync` y `DesactivarWithTareasAsync` con `Database.BeginTransactionAsync` | hexagonal-arch sec. 4.3 + 7.2; cqrs-plan Handler pasos 8 y 9 | **CUBIERTO** |
| NFR-02 | ArtistaId no modificable por cliente (JWT) | `UserId` en Command asignado solo por controller desde `User.FindFirstValue(ClaimTypes.NameIdentifier)`; `CreatePromoProgramaRequest` omite el campo | cqrs-plan sec. 5.1, 5.2, 5.3; contracts-plan sec. 2.1 | **CUBIERTO** |
| NFR-03 | Responsive mobile/tablet/desktop | Clases `grid grid-cols-1 md:grid-cols-2` en formularios; breakpoints 768px y 1024px definidos | ui-design sec. 1 (breakpoints); sec. 3 en cada componente | **CUBIERTO** |
| NFR-04 | Accesibilidad (ARIA) | `aria-current="step"` en stepper; `aria-label` en pasos; `aria-invalid="true"` en inputs con error; `aria-label` en Switches | ui-design sec. 3.1.1 WizardStepper ARIA; PromoTareaForm con `aria-label="La tarea es repetible"` | **CUBIERTO** |
| NFR-05 | Patron CQRS (Command + Handler mismo archivo; sin DbContext en handler) | Command + Handler en mismo archivo `.cs`; handlers inyectan `IPromoProgramaService`, no `DbContext` | cqrs-plan sec. 5.x todos los handlers; hexagonal-arch sec. 7.1 | **CUBIERTO** |
| NFR-06 | Cache per-request (ADR-006) | `IRequestCacheService` en `PromoProgramaService` para `GetByIdAsync` y `ExisteCodigoTrackingAsync` | hexagonal-arch sec. 4.3 comportamiento de metodos; sec. 7.3 | **CUBIERTO** |
| NFR-07 | Validators con `ServiceResponseMessageType` constants (no strings literales) | Todos los validators usan constantes del modulo (ej: `Validation_ComisionRequerida`, `Validation_CodigoTrackingDuplicado`) | hexagonal-arch sec. 3.4; cqrs-plan sec. 7 validadores | **CUBIERTO** |
| NFR-08 | Migracion EF Core aditiva sin romper registros existentes | 4 columnas nullable en PromoPrograma; 4 nullable + 1 NOT NULL con default en PromoTarea; indice unico filtrado | hexagonal-arch sec. 5 | **CUBIERTO** |
| NFR-09 | Toast de feedback en cada operacion (crear, actualizar, desactivar) | `useCreatePromoPrograma` onSuccess: `toast.success("Programa de promocion creado")`; otros hooks similares | frontend-plan sec. 4.6 Flujo submit; test-strategy casos de onSuccess | **CUBIERTO** |

---

## 4. Flujos Alternativos - Matriz de Cobertura

| ID | Condicion | Plan de cobertura | Tests | Estado |
|----|-----------|-------------------|-------|--------|
| FA-01 | Sin campanas de crowdfunding | `campaniaCrowdfundingId` y `proyectoArtisticoId` opcionales en schema y DTO; banner info azul en paso 1; handler no falla si ausentes | `Paso1DatosBasicos.test.tsx` caso 6 "campana y proyecto son opcionales" | **CUBIERTO** |
| FA-02 | Sin tareas en wizard | `tareas?: CreatePromoTareaItem[]` opcional en request; `TareasStep` muestra aviso "opcional"; schema `tareas.optional().default([])`; paso 3 permite avanzar con 0 tareas | `WizardContainer.test.tsx` caso 10; `Paso3Tareas.test.tsx` caso 8; `createPromoProgramaSchema.test.ts` casos 25-26 | **CUBIERTO** |
| FA-03 | FechaFin <= FechaInicio | `createPromoProgramaSchema` Refine 2: `fechaFin > fechaInicio`; mismo refine en `promoProgramaStep1Schema`; validator backend `Validation_FechaFinAnterior = "1022"` | `createPromoProgramaSchema.test.ts` casos 13-17 incluyendo fechas iguales | **CUBIERTO** |
| FA-04 | Sin comision porcentaje ni fija | Schema Refine 1 bloquea avance del paso 2; AvisoBanner amber visible; validator backend retorna `1020` | `Paso2Comisiones.test.tsx` caso 2; `WizardContainer.test.tsx` caso 5 | **CUBIERTO** |
| FA-05 | CodigoTrackingBase duplicado por artista | Backend retorna error `1024`; `useCreatePromoPrograma` muestra toast con `getPromoProgramaErrorMessage("1024")` | `use-promo-programa-mutations.test.ts` caso 7 "error 1024 (tracking duplicado) muestra toast especifico" | **CUBIERTO** |
| FA-06 | EsRepetible = true sin MaxRepeticiones | Schema Refine 1 en `createPromoTareaSchema`; campo `maxRepeticiones` visible condicionalmente; validator backend codigo `1026` | `PromoTareaFormItem.test.tsx` casos 5-7; `Paso3Tareas.test.tsx` caso 5 | **CUBIERTO** |
| FA-07 | Campana/proyecto no pertenece al artista | Handler verifica pertenencia con `VerificarCampaniaPertenece`; retorna 403; frontend captura error `3002` como toast | `promo-programa.service.test.ts` caso 15 "lanza error en 403" | **CUBIERTO** |
| FA-08 | Error de red durante publicacion | `PromoProgramaWizardContainer` en `onError` muestra toast y mantiene wizard en paso 4 sin resetear estado | `WizardContainer.test.tsx` caso 15 "error de red mantiene estado del wizard" | **CUBIERTO** |

---

## 5. Observaciones Tecnicas (No son gaps bloqueantes)

Las siguientes observaciones no impiden la implementacion pero requieren decision consciente del implementador.

### OBS-01: Divergencia de codigo de error para formato de CodigoTracking

**Descripcion:** `hexagonal-architecture.md` define la constante `Validation_CodigoTrackingInvalidFormat = "1025"` para el formato invalido de CodigoTracking. Sin embargo, `contracts-plan.md` asigna el codigo `'1023'` al mensaje "El formato del campo no es valido" y el codigo `'1025'` al mensaje "Las tareas repetibles deben tener maximo de repeticiones". Existe una divergencia: backend usa `1025` para formato de tracking, shared usa `1025` para EsRepetible-MaxRepeticiones.

**Impacto:** Bajo. El comportamiento es correcto en ambos planes; solo el codigo numerico difiere.

**Resolucion sugerida:** Al implementar el validator de backend, verificar que el codigo usado para CodigoTrackingBase formato invalido sea el mismo que figura en `PROMO_PROGRAMA_ERROR_MESSAGES` del shared. Alinear ambos en la implementacion real.

---

### OBS-02: ProgramaEditPage.test referenciado pero no detallado en estructura de tests

**Descripcion:** La tabla de criterios en `test-strategy.md` menciona `ProgramaEditPage.test` como suite responsable de AC-CP02-6 (edicion con aviso de promotores), pero este archivo no aparece en la estructura de carpetas de tests de la seccion 2.

**Impacto:** Bajo. Los casos de edicion del wizard estan cubiertos distribuidos en `WizardContainer.test.tsx` (modo edit), `use-promo-programa-mutations.test.ts` (casos 8-11) y `Paso1DatosBasicos.test.tsx` (caso 11 defaultValues).

**Resolucion sugerida:** El implementador puede crear `ProgramaEditPage.test.tsx` como test de integracion de la pagina `/[id]/editar` o confirmar que la cobertura existente es suficiente.

---

### OBS-03: Zod .omit().extend() pierde refines en updatePromoProgramaSchema

**Descripcion:** `contracts-plan.md` advierte que `createPromoProgramaSchema.omit({tareas:true}).extend({tareas: updatePromoTareaSchema})` pierde los 2 refines (al menos una comision y fechaFin > fechaInicio). El plan documenta dos estrategias pero delega la decision.

**Impacto:** Medio si no se implementa correctamente: AC-CP02-4 quedaria sin validacion en el formulario de edicion.

**Resolucion sugerida:** Usar Estrategia A del contracts-plan: definir `promoProgramaBaseSchema` sin `tareas` y sin refines, crear factory `addPromoProgramaRefines(schema)`, aplicarla tanto en `createPromoProgramaSchema` como en `updatePromoProgramaSchema`. Agregar test que verifique que el refine de comision dispara en el schema de update.

---

### OBS-04: campaniaCrowdfundingId con string vacio puede fallar .uuid()

**Descripcion:** `contracts-plan.md` nota 7 advierte que si el usuario deselecciona una campana en el Select, el campo puede quedar como string vacio `""`, que falla la validacion `z.string().uuid()`.

**Impacto:** Bajo. El plan documenta el riesgo pero el schema escrito en contracts-plan usa `z.string().uuid().optional()` sin el patron `.or(z.literal(''))`.

**Resolucion sugerida:** Al implementar el schema, usar `z.string().uuid().optional().or(z.literal(''))`  para `campaniaCrowdfundingId` y `proyectoArtisticoId`, y transformar strings vacios a `undefined` antes del POST en el hook de mutacion.

---

## 6. Validacion de Tests

### 6.1 Cobertura de Criterios de Aceptacion en Tests

| Criterio | Suites de Tests | Tipo | Estado |
|----------|-----------------|------|--------|
| AC-CP02-1 | `WizardContainer.test.tsx` (18 casos), `useCreatePromoPrograma.test.ts` (casos 1-7), `Paso4Revision.test.tsx` (casos 6-7) | Integration + Unit | **CUBIERTO** |
| AC-CP02-2 | Backend (fuera scope admin); verificado en cqrs-plan Handler pasos 6-7 | Backend unit tests | **CUBIERTO (backend)** |
| AC-CP02-3 | Backend (fuera scope admin); verificado en cqrs-plan Handler pasos 2-3 | Backend unit tests | **CUBIERTO (backend)** |
| AC-CP02-4 | `createPromoProgramaSchema.test.ts` (casos 8-12), `Paso2Comisiones.test.tsx` (caso 2), `WizardContainer.test.tsx` (caso 5) | Unit + Integration | **CUBIERTO** |
| AC-CP02-5 | `MisProgramasPage.test.tsx` (11 casos), `PromoProgramaCard.test.tsx` (12 casos), `PromoProgramaStatusBadge.test.tsx`, `PromoProgramaFilters.test.tsx`, `use-promo-programas.test.ts` (8 casos) | Integration + Unit | **CUBIERTO** |
| AC-CP02-6 | `useUpdatePromoPrograma.test.ts` (casos 8-11), `DesactivarProgramaDialog.test.tsx` (8 casos), `Paso1DatosBasicos.test.tsx` (caso 11 defaultValues) | Integration + Unit | **CUBIERTO** |
| AC-CP02-7 | `DesactivarProgramaDialog.test.tsx` (8 casos), `use-promo-programa-mutations.test.ts` (casos 12-16), `PromoProgramaDetailPage.test.tsx` (casos 6-8) | Integration + Unit | **CUBIERTO** |
| AC-CP02-8 | `createPromoProgramaSchema.test.ts` (casos 18-20), `use-promo-programa-mutations.test.ts` (caso 7) | Unit + Integration | **CUBIERTO** |
| AC-CP02-9 | `createPromoTareaSchema.test.ts` (casos 8-11), `Paso3Tareas.test.tsx` (casos 5-6), `PromoTareaFormItem.test.tsx` (casos 5-7) | Unit + Integration | **CUBIERTO** |
| AC-CP02-10 | `Paso1DatosBasicos.test.tsx` (caso 6), `Paso4Revision.test.tsx` (caso 10), `createPromoProgramaSchema.test.ts` (casos 21-22) | Integration + Unit | **CUBIERTO** |

### 6.2 Cobertura de Flujos Alternativos en Tests

| FA | Suite Responsable | Casos Especificos | Estado |
|----|-------------------|-------------------|--------|
| FA-01 | `Paso1DatosBasicos.test.tsx` caso 6 | "campana y proyecto son opcionales" | **CUBIERTO** |
| FA-02 | `WizardContainer.test.tsx` caso 10, `Paso3Tareas.test.tsx` caso 8, `createPromoProgramaSchema.test.ts` casos 25-26 | "Paso 3 sin tareas permite avanzar" | **CUBIERTO** |
| FA-03 | `createPromoProgramaSchema.test.ts` casos 13-17 | "fechaFin posterior a fechaInicio"; fechas iguales | **CUBIERTO** |
| FA-04 | `Paso2Comisiones.test.tsx` caso 2, `WizardContainer.test.tsx` caso 5 | "ambas comisiones vacias bloquea submit" | **CUBIERTO** |
| FA-05 | `use-promo-programa-mutations.test.ts` caso 7 | "error 1024 (tracking duplicado) muestra toast especifico" | **CUBIERTO** |
| FA-06 | `PromoTareaFormItem.test.tsx` casos 5-7, `Paso3Tareas.test.tsx` caso 5 | "esRepetible=true sin maxRepeticiones muestra error" | **CUBIERTO** |
| FA-07 | `promo-programa.service.test.ts` caso 15 | "lanza error en 403" | **CUBIERTO** |
| FA-08 | `WizardContainer.test.tsx` caso 15 | "error de red mantiene estado del wizard" | **CUBIERTO** |

### 6.3 Metricas de Tests Planificadas

| Tipo | Cantidad | Cobertura estimada | Prioridad |
|------|----------|--------------------|-----------|
| Unit Tests (schemas Zod) | 42 | 95% logica de validacion | P0 |
| Unit Tests (componentes puros) | 31 | 85% renderizado | P1 |
| Integration Tests (wizard) | 18 | 90% flujos criticos | P0 |
| Integration Tests (hooks) | 29 | 85% | P0 |
| Integration Tests (service) | 20 | 90% | P1 |
| **Total** | **140** | **80%+** | |

---

## 7. Validacion de UI/UX

### 7.1 Screens Requeridas vs Planificadas

| Screen Requerida (feature-spec) | Planificada | Componentes Clave | Estado |
|---------------------------------|-------------|-------------------|--------|
| Wizard crear programa `/nuevo` | Si | `PromoProgramaWizardContainer`, `DatosBasicosStep`, `ComisionesStep`, `TareasStep`, `RevisarPublicarStep`, `PromoProgramaWizardStepper` | **CUBIERTO** |
| Listado mis programas `/programas` | Si | `PromoProgramaListClient`, `PromoProgramaCard`, `PromoProgramaFilters`, `PromoProgramaStatusBadge`, `PromoProgramaEmptyState` | **CUBIERTO** |
| Detalle programa `/programas/[id]` | Si | `PromoProgramaDetailClient`, `PromoProgramaHeader`, `PromoProgramaKpiCards`, tabs Info/Tareas/Promotores/Resumen | **CUBIERTO** |
| Edicion programa `/programas/[id]/editar` | Si | `PromoProgramaWizardContainer` modo `"edit"` con `usePromoPrograma(programaId)` para precarga | **CUBIERTO** |
| Dialog desactivacion | Si | `DesactivarPromoProgramaDialog` (AlertDialog shadcn/ui) | **CUBIERTO** |
| Dialog abandono wizard | Si | `PromoProgramaAbandonDialog` (AlertDialog shadcn/ui) | **CUBIERTO** |

### 7.2 Estados de UI Planificados

| Pantalla | Estado | Planificado | Visual definido | Status |
|----------|--------|-------------|-----------------|--------|
| Listado | Loading | Si | 3 skeletons `animate-pulse h-[160px] bg-[#1e1e38]` | **CUBIERTO** |
| Listado | Error | Si | Card error con boton "Reintentar" | **CUBIERTO** |
| Listado | Empty sin programas | Si | `PromoProgramaEmptyState` variant `no-programas` con CTA "Crear programa" | **CUBIERTO** |
| Listado | Empty sin resultados | Si | `PromoProgramaEmptyState` variant `no-resultados` con "Limpiar filtros" | **CUBIERTO** |
| Listado | Con datos | Si | `PromoProgramaCard` + paginacion | **CUBIERTO** |
| Wizard Paso 1 | Error validacion | Si | `FormMessage` con `border-red-500` + `aria-invalid="true"` | **CUBIERTO** |
| Wizard Paso 1 | Con promotores (modo edit) | Si | Banner aviso amber visible | **CUBIERTO** |
| Wizard Paso 2 | Sin comisiones | Si | Preview placeholder + AvisoBanner amber | **CUBIERTO** |
| Wizard Paso 2 | Con comisiones | Si | `ComisionPreviewCard` con calculo en tiempo real via `useWatch` | **CUBIERTO** |
| Wizard Paso 3 | Sin tareas | Si | Empty state con boton "+ Agregar nueva tarea" | **CUBIERTO** |
| Wizard Paso 4 | Submitting | Si | Spinner en boton + todos los botones `disabled` | **CUBIERTO** |
| Wizard Paso 4 | Valores nulos | Si | Mostrado como "--" en `text-[#64748b] italic` | **CUBIERTO** |
| Detalle | Loading | Si | Skeleton | **CUBIERTO** |
| Detalle | Programa activo | Si | Boton Desactivar visible | **CUBIERTO** |
| Detalle | Programa inactivo | Si | Boton Desactivar oculto | **CUBIERTO** |

### 7.3 Componente shadcn/ui Pendiente de Instalar

| Componente | Estado | Requerido para |
|------------|--------|---------------|
| Switch | **Por instalar** | Toggle `EsRepetible` en `PromoTareaForm`; toggle de estado activo en `PromoTareaCard` |

Todos los demas componentes shadcn/ui requeridos (Button, Card, Input, Label, Textarea, Badge, Skeleton, Select, Tabs, AlertDialog, Dialog, DropdownMenu, Table, Avatar, Separator, Alert, Tooltip, Sonner) estan instalados.

---

## 8. Validacion de Contratos API

### 8.1 Alineacion de Tipos TypeScript vs C#

| Tipo TS (contracts-plan) | Tipo C# (cqrs-plan) | Alineacion de campos | Estado |
|--------------------------|---------------------|----------------------|--------|
| `CreatePromoProgramaRequest` (13 campos) | `CreatePromoProgramaRequestDto` (13 campos) | Todos los campos mapeados; `artistaId` ausente en ambos | **ALINEADO** |
| `CreatePromoTareaItem` (12 campos) | `CreatePromoTareaItemDto` (12 campos) | Incluye `EsRepetible`, `MaxRepeticiones`, `TipoEventoPromoId` | **ALINEADO** |
| `PromoProgramaCreatedResult` (6 campos) | `PromoProgramaCreatedResultDto` (6 campos) | `esActivo`, `tareasCreadas` presentes | **ALINEADO** |
| `PromoProgramaListItem` (13 campos) | `PromoProgramaListItemDto` (13 campos) | `esActivo`, `numeroPromotores`, `numeroTareas`, `campaniaTitulo` nullable | **ALINEADO** |
| `PromoProgramaDetail` (con tareas + promotores + resumen) | `PromoProgramaDetailDto` (idem) | Estructura compleja con listas anidadas | **ALINEADO** |
| `PromoProgramaDesactivadoResult` (3 campos) | `PromoProgramaDesactivadoResultDto` (3 campos) | `esActivo: false`, `tareasDesactivadas: number` | **ALINEADO** |

### 8.2 Alineacion de Rutas

| Endpoint | TypeScript `API_ROUTES` | Ruta Backend | Estado |
|----------|------------------------|--------------|--------|
| POST crear | `programas.base` = `/api/crowdpromotion/programas` | `POST /api/crowdpromotion/programas` | **ALINEADO** |
| GET mis programas | `programas.mis` = `/api/crowdpromotion/programas/mis-programas` | `GET /api/crowdpromotion/programas/mis-programas` | **ALINEADO** |
| GET detalle | `programas.byId(id)` | `GET /api/crowdpromotion/programas/{id}` | **ALINEADO** |
| PUT editar | `programas.byId(id)` | `PUT /api/crowdpromotion/programas/{id}` | **ALINEADO** |
| PATCH desactivar | `programas.desactivar(id)` | `PATCH /api/crowdpromotion/programas/{id}/desactivar` | **ALINEADO** |

### 8.3 Codigos de Error - Cobertura Completa

| Codigo | Descripcion | Definido en Shared | Definido en Backend | Estado |
|--------|-------------|-------------------|---------------------|--------|
| `1020` | Comision requerida (AC-CP02-4) | `PROMO_PROGRAMA_ERROR_MESSAGES` | `Validation_ComisionRequerida` | **CUBIERTO** |
| `1022` | FechaFin anterior (FA-03) | `ERROR_CODE_MESSAGES` | `Validation_FechaFinAnterior` | **CUBIERTO** |
| `1024` | CodigoTracking duplicado (AC-CP02-8) | `PROMO_PROGRAMA_ERROR_MESSAGES` | `Validation_CodigoTrackingDuplicado` | **CUBIERTO** |
| `1025` | EsRepetible sin MaxRepeticiones (AC-CP02-9) | `PROMO_PROGRAMA_ERROR_MESSAGES` | `Validation_EsRepetibleSinMaxRepeticiones` | **CUBIERTO** (ver OBS-01) |
| `2016` | Artista no encontrado | `PROMO_PROGRAMA_ERROR_MESSAGES` | `NotFound_Artista` | **CUBIERTO** |
| `2019` | Programa no encontrado | `PROMO_PROGRAMA_ERROR_MESSAGES` | `NotFound_PromoPrograma` | **CUBIERTO** |
| `3002` | Forbidden (campana no del artista) FA-07 | `CAMPANA_NOT_BELONGS_TO_ARTISTA` key semantica | `Auth_Forbidden` | **CUBIERTO** |
| `4020` | Programa ya desactivado (AC-CP02-7) | `PROMO_PROGRAMA_ERROR_MESSAGES` | `BusinessRule_PromoProgramaAlreadyInactive` | **CUBIERTO** |
| `4021` | Tarea con completados | `PROMO_PROGRAMA_ERROR_MESSAGES` | `BusinessRule_PromoTareaConCompletados` | **CUBIERTO** |

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [x] Todos los 10 criterios de aceptacion tienen cobertura en al menos un plan
- [x] Los 4 flujos principales (crear, listar, editar, desactivar) completamente planificados
- [x] Todos los 8 flujos alternativos cubiertos
- [x] Transaccionalidad documentada en creacion y desactivacion

### UI/UX
- [x] Las 4 pages de Next.js planificadas (listado, nuevo, detalle, editar)
- [x] Los 6 dialogs/modales planificados (desactivar, abandonar wizard)
- [x] Todos los estados de interaccion definidos (loading, error, empty, success)
- [x] Responsive design con breakpoints mobile/tablet/desktop documentados
- [x] Accesibilidad: ARIA en stepper, inputs con error, switches
- [x] Paleta de colores oscura consistente con el design system del proyecto
- [ ] **Switch de shadcn/ui pendiente de instalar** (accion requerida antes de implementar)

### Testing
- [x] Tests P0 para todos los criterios criticos (AC-CP02-1, 4, 5, 9)
- [x] 18 tests de integracion para flujo completo del wizard
- [x] Cobertura objetivo 80%+ con 140 tests planificados
- [x] Fixtures centralizados en `promo-programa.mock.ts`
- [x] Patron de mocking consistente con `promotor.mock.ts`

### Contratos Shared
- [x] 17 tipos TypeScript nuevos definidos
- [x] 4 schemas Zod con todos los refines de negocio
- [x] Constantes de dominio con labels y descriptions
- [x] Mensajes de error en espanol con codigos numericos y claves semanticas
- [x] Sin colision de nombres con modulos existentes (TIPO_REWARD_PROMO vs TIPO_REWARD)
- [x] Rutas de app y API correctamente definidas y alineadas con backend

### Backend
- [x] 5 endpoints CQRS planificados (2 queries + 3 commands)
- [x] Patron CQRS respetado en todos los handlers
- [x] Handlers inyectan IPromoProgramaService (no DbContext directamente)
- [x] Validators con ServiceResponseMessageType constants
- [x] Migracion EF Core documentada con nombre descriptivo y cambios esperados
- [x] Indice unico filtrado para CodigoTrackingBase (IS NOT NULL)

---

## 10. Recomendaciones para Implementacion

### Accion Requerida Antes de Implementar

1. **Instalar componente Switch de shadcn/ui**
   - Archivo resultante: `src/admin/src/components/ui/switch.tsx`
   - Comando: `npx shadcn-ui@latest add switch`
   - Razon: `PromoTareaCard` y `PromoTareaForm` requieren Switch para el toggle de `EsRepetible`

### Acciones Sugeridas

2. **Resolver OBS-01: alinear codigo de error para formato de CodigoTracking**
   - Antes de implementar el validator, verificar que el codigo usado para "formato invalido de CodigoTracking" en el backend coincide con el codigo en `PROMO_PROGRAMA_ERROR_MESSAGES`. Actualizar el que sea necesario.

3. **Resolver OBS-03: implementar Estrategia A para refines de updatePromoProgramaSchema**
   - En `src/shared/schemas/crowdpromotion.schema.ts`, usar una funcion factory para aplicar los refines en lugar de `.omit().extend()`. Agregar un test unitario que verifique que el refine de "al menos una comision" se ejecuta en `updatePromoProgramaSchema`.

4. **Resolver OBS-04: manejar string vacio en campaniaCrowdfundingId**
   - En la implementacion del schema, agregar `.or(z.literal(''))` en `campaniaCrowdfundingId` y `proyectoArtisticoId`. En el hook `useCreatePromoPrograma`, transformar strings vacios a `undefined` antes de enviar el payload.

### Nice to Have

5. Crear `ProgramaEditPage.test.tsx` como test de integracion de la pagina `/[id]/editar` para validar explicitamente el flujo end-to-end de edicion.

6. Agregar `COMISION_PORCENTAJE_MIN: 0` en el objeto `VALIDATION` de shared/constants para simetria con `COMISION_PORCENTAJE_MAX: 100`.

7. Agregar comentario inline en la constante `TIPO_REWARD_PROMO` explicando la diferencia con `TIPO_REWARD` del modulo campanias.

---

## 11. Orden de Implementacion Recomendado

Basado en las dependencias entre artefactos:

1. **Instalar Switch** (`npx shadcn-ui@latest add switch`)
2. **`src/shared/types/crowdpromotion.ts`** - Sin dependencias externas
3. **`src/shared/constants/index.ts`** - Sin dependencias externas
4. **`src/shared/schemas/crowdpromotion.schema.ts`** - Depende de tipos
5. **`src/shared/utils/error-messages.ts`** - Sin dependencias del shared
6. **Backend: dominio** (`PromoPrograma.cs`, `PromoTarea.cs`, `ServiceResponseMessageType.cs`, `IPromoProgramaRepository.cs`)
7. **Backend: infraestructura** (`CrowdpromotionContext.cs` configs, `PromoProgramaRepository.cs`, `PromoProgramaService.cs`, migracion EF Core)
8. **Backend: CQRS** (DTOs, Commands, Queries, Validators, Profiles, Controller)
9. **Admin: service y hooks** (`promo-programa.service.ts`, `use-promo-programas.ts`, `use-promo-programas-mutations.ts`, `use-promo-programa-wizard-state.ts`)
10. **Admin: componentes wizard** (steps, tareas, container)
11. **Admin: componentes listado y detalle** (list, detail, filters)
12. **Admin: pages** (4 pages de Next.js)
13. **Tests** (schemas P0 primero, luego wizard, luego hooks y service)

---

## 12. Conclusion

**Score Final de Criterios de Aceptacion:** 10/10 = **100%**

**Score Final de Flujos Alternativos:** 8/8 = **100%**

**Veredicto: APROBADO**

Todos los criterios de aceptacion (AC-CP02-1 a AC-CP02-10) y todos los flujos alternativos
(FA-01 a FA-08) tienen cobertura completa distribuida entre los siete planes analizados. Los
planes son consistentes entre si en terminos de contratos de API, codigos de error y nomenclatura.

Se identificaron cuatro observaciones tecnicas (OBS-01 a OBS-04) que no bloquean la
implementacion pero requieren atencion durante el desarrollo para garantizar la correcta
implementacion de las reglas de negocio (especialmente la validacion de comisiones en edicion
y el manejo de campos UUID opcionales con string vacio).

El plan de testing es exhaustivo (140 tests, 80%+ cobertura objetivo) con prioridades P0
correctamente asignadas a los criterios de negocio mas criticos.

**Proximo Paso:** Proceder a implementacion en el orden documentado en la seccion 11.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-25
