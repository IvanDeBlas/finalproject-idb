# Validacion QA: Templates y Guia para Artistas Noveles (Landing)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** src/web (Landing Application)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 9 |
| Cubiertos por Specs | 9 |
| Cubiertos por Plans | 3 (solo contracts-plan.md existe) |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 6 |
| **Score de Cobertura** | **33%** |

**Estado:** ❌ REQUIERE CAMBIOS

**Veredicto:** La feature esta completamente especificada en `feature-spec.md`, `contracts.md` y `ui-ux.md`, pero **NO EXISTE frontend-plan.md, ui-design.md ni test-strategy.md** para la aplicacion Landing. Solo se ha generado `contracts-plan.md` en shared. Se requiere crear los planes de implementacion faltantes antes de proceder.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo |
|----|----------|------|
| AC-CS01-1 | Galeria de 6 templates con cards interactivos mostrando icono, nombre, descripcion, rango de precio total (min-max EUR) y cantidad de necesidades | Funcional - UI |
| AC-CS01-2 | Desglose por fases con cada necesidad mostrando titulo, descripcion, rol profesional, precio orientativo, prioridad (Esencial/Recomendado/Opcional) | Funcional - UI |
| AC-CS01-3 | Checkboxes para marcar/desmarcar necesidades. Necesidades "Esencial" pre-seleccionadas, "Recomendado" y "Opcional" deseleccionadas | Funcional - Interaccion |
| AC-CS01-4 | Ajuste de presupuesto (min y max) por necesidad. Validacion max >= min en frontend y backend | Funcional - Validacion |
| AC-CS01-5 | Creacion automatica de N registros de NecesidadCrowdsourcing en estado "Abierta" pre-rellenados con datos del template | Funcional - Backend |
| AC-CS01-6 | Todas las necesidades vinculadas al mismo ProyectoArtisticoId. Si no existe proyecto, se crea automaticamente | Funcional - Backend |
| AC-CS01-7 | Resumen pre-confirmacion mostrando template, cantidad seleccionadas, lista de necesidades, coste total estimado (min/max) | Funcional - UI |
| AC-CS01-8 | 6 plantillas pre-cargadas en DB (Grabar Album/EP, Videoclip, Gira/Tour, Marketing, Single, Branding) con ~55 necesidades y ~35 roles profesionales | Datos - Seed |
| AC-CS01-9 | Tooltips en roles profesionales mostrando descripcion al hacer hover. Descripcion viene de MaestraRolProfesional.Descripcion | Funcional - UX |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | feature-spec | contracts | ui-ux | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|--------------|-----------|-------|---------------|-----------|---------------|--------|
| AC-CS01-1 | Galeria de 6 templates con cards interactivos | ✅ Linea 72 | ✅ GET /templates | ✅ Pantalla 1 | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |
| AC-CS01-2 | Desglose por fases con necesidades completas | ✅ Linea 73 | ✅ GET /templates/{id} | ✅ Pantalla 2 | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |
| AC-CS01-3 | Checkboxes con pre-seleccion por prioridad | ✅ Linea 74 | ✅ prioridad: string | ✅ NecesidadItem | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |
| AC-CS01-4 | Ajuste de presupuesto con validacion | ✅ Linea 75 | ✅ Zod schema | ✅ Budget inputs | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |
| AC-CS01-5 | Creacion automatica de NecesidadCrowdsourcing | ✅ Linea 76 | ✅ POST /generar | N/A (backend) | N/A | N/A | ❌ NO EXISTE | **PARCIAL (backend)** |
| AC-CS01-6 | Vinculacion a ProyectoArtisticoId | ✅ Linea 77 | ✅ Request DTO | N/A (backend) | N/A | N/A | ❌ NO EXISTE | **PARCIAL (backend)** |
| AC-CS01-7 | Resumen pre-confirmacion con totales | ✅ Linea 78 | ✅ Response DTO | ✅ Pantalla 3 | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |
| AC-CS01-8 | 6 plantillas pre-cargadas en DB | ✅ Linea 79 | ✅ Seed data | ✅ Template data | N/A (backend) | N/A | N/A | **CUBIERTO (backend)** |
| AC-CS01-9 | Tooltips en roles profesionales | ✅ Linea 80 | ✅ rolDescripcion | ✅ Tooltips | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | **NO CUBIERTO** |

**Leyenda:**
- ✅ CUBIERTO: Requisito completamente especificado/planificado
- ❌ NO CUBIERTO: Requisito no planificado en documentos de implementacion
- N/A: No aplica a este documento
- **PARCIAL**: Requisito solo cubierto en specs, falta plan de implementacion

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Galeria templates carga < 500ms | ui-ux: mencionado | ❌ NO PLANIFICADO (no existe frontend-plan) |
| NFR-02 | Wizard navegacion instantanea entre pasos | ui-ux: state en React | ❌ NO PLANIFICADO (no existe frontend-plan) |
| NFR-03 | Creacion necesidades < 2s (11 items) | feature-spec: mencionado | ❌ NO PLANIFICADO (backend, no test-strategy) |
| NFR-04 | Validar ownership de ProyectoArtistico | contracts: 403 Forbidden | ✅ CUBIERTO (contracts + backend) |
| NFR-05 | Rate limiting 10 req/h por usuario | feature-spec: mencionado | ❌ NO PLANIFICADO (backend, no implementacion) |
| NFR-06 | Wizard con indicador visual de paso | ui-ux: WizardStepper | ❌ NO PLANIFICADO (no existe ui-design) |
| NFR-07 | Navegacion sin perder datos | ui-ux: state en React | ❌ NO PLANIFICADO (no existe frontend-plan) |
| NFR-08 | Resumen presupuesto en tiempo real | ui-ux: BudgetSummary | ❌ NO PLANIFICADO (no existe ui-design) |
| NFR-09 | Tooltips educativos en todos los roles | ui-ux: RolProfesionalTooltip | ❌ NO PLANIFICADO (no existe ui-design) |
| NFR-10 | Responsive mobile (breakpoints) | ui-ux: Responsive section | ❌ NO PLANIFICADO (no existe ui-design) |
| NFR-11 | Accesibilidad WCAG AA | ui-ux: Accessibility section | ❌ NO PLANIFICADO (no existe ui-design) |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-001 | AC-CS01-1 | NO EXISTE frontend-plan.md para Landing | CRITICO | Crear `plans/cs-templates-guia/frontend-landing/frontend-plan.md` con arquitectura de componentes, hooks, services, routing |
| GAP-002 | AC-CS01-2 | NO EXISTE ui-design.md para componentes de wizard | CRITICO | Crear `plans/cs-templates-guia/frontend-landing/ui-design.md` con especificaciones de componentes: TemplateCard, NecesidadItem, BudgetSummary, WizardStepper |
| GAP-003 | AC-CS01-7 | NO EXISTE test-strategy.md para wizard E2E | CRITICO | Crear `plans/cs-templates-guia/frontend-landing/test-strategy.md` con casos de prueba para flujo completo (3 pasos) + validaciones |
| GAP-004 | NFR-01 | No hay plan de performance (cache, staleTime) | ALTO | En frontend-plan.md especificar estrategia de cache para templates (TanStack Query con staleTime 5min) |
| GAP-005 | NFR-07 | No hay plan de state management para wizard | ALTO | En frontend-plan.md definir state management: Zustand/Context + localStorage para persistencia |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-006 | AC-CS01-3 | Logica de pre-seleccion de checkboxes no planificada | MEDIO | En frontend-plan.md documentar logica: `defaultChecked = necesidad.prioridad === "Alta"` |
| GAP-007 | AC-CS01-4 | Validacion frontend de presupuestos no documentada | MEDIO | En frontend-plan.md referenciar `generarNecesidadesSchema` de contracts-plan.md |
| GAP-008 | NFR-08 | Recalculo de totales en tiempo real no especificado | MEDIO | En ui-design.md especificar `useEffect` para recalcular totales al cambiar seleccion/presupuestos |
| GAP-009 | NFR-10 | Breakpoints responsive no implementados | MEDIO | En ui-design.md documentar Tailwind breakpoints: `md:grid-cols-2 lg:grid-cols-3` |
| GAP-010 | NFR-11 | ARIA labels y keyboard navigation no planificados | MEDIO | En ui-design.md agregar seccion de accesibilidad con ejemplos de ARIA labels |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-011 | AC-CS01-9 | Tooltips accesibles con teclado no documentado | BAJO | En ui-design.md agregar nota: usar shadcn/ui Tooltip con soporte Enter/Esc |
| GAP-012 | NFR-06 | WizardStepper component sin specs detalladas | BAJO | En ui-design.md agregar props: `{ steps: 3, currentStep: number }` |
| GAP-013 | Flujo Alt FA-03 | Navegacion "< Atras" sin spec de preservacion de datos | BAJO | En frontend-plan.md documentar: usar URL params o state global |
| GAP-014 | Flujo Alt FA-02 | Boton disabled cuando count(checked) === 0 | BAJO | En ui-design.md agregar: `disabled={selectedCount === 0}` |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-CS01-1 | test-galeria-templates-render | E2E | ❌ NO PLANIFICADO (no existe test-strategy.md) |
| AC-CS01-1 | test-template-card-navigation | E2E | ❌ NO PLANIFICADO |
| AC-CS01-2 | test-desglose-necesidades-render | E2E | ❌ NO PLANIFICADO |
| AC-CS01-3 | test-checkbox-toggle | Unit | ❌ NO PLANIFICADO |
| AC-CS01-3 | test-prioridad-pre-seleccion | Unit | ❌ NO PLANIFICADO |
| AC-CS01-4 | test-presupuesto-validation | Unit | ❌ NO PLANIFICADO |
| AC-CS01-4 | test-presupuesto-max-menor-min | Unit | ❌ NO PLANIFICADO |
| AC-CS01-5 | test-generar-necesidades-mutation | Integration | ❌ NO PLANIFICADO |
| AC-CS01-7 | test-resumen-confirmacion | E2E | ❌ NO PLANIFICADO |
| AC-CS01-9 | test-tooltip-hover | E2E | ❌ NO PLANIFICADO |
| NFR-08 | test-resumen-tiempo-real | Integration | ❌ NO PLANIFICADO |

### Tests Faltantes

| Criterio | Test Requerido | Razon | Prioridad |
|----------|----------------|-------|-----------|
| AC-CS01-1 | `tests/e2e/templates-gallery.spec.ts` | Validar render de 6 cards, click en card navega a paso 2 | ALTA |
| AC-CS01-2 | `tests/e2e/necesidades-desglose.spec.ts` | Validar agrupacion por fases, render de 11 necesidades | ALTA |
| AC-CS01-3 | `tests/unit/NecesidadItem.test.tsx` | Validar checkbox toggle y defaultChecked por prioridad | ALTA |
| AC-CS01-4 | `tests/unit/BudgetInput.test.tsx` | Validar validacion Zod, mensajes de error, max >= min | ALTA |
| AC-CS01-7 | `tests/e2e/wizard-confirmacion.spec.ts` | Validar resumen con totales correctos, lista completa | ALTA |
| AC-CS01-9 | `tests/e2e/tooltip-roles.spec.ts` | Validar hover muestra tooltip, contenido correcto | MEDIA |
| NFR-08 | `tests/integration/budget-summary.test.tsx` | Validar recalculo de totales al cambiar checkboxes/inputs | ALTA |
| Flujo completo | `tests/e2e/wizard-full-flow.spec.ts` | Validar paso 1 -> 2 -> 3 -> submit -> redirect | CRITICA |
| FA-02 | `tests/e2e/wizard-validations.spec.ts` | Validar boton disabled si count === 0 | MEDIA |
| FA-03 | `tests/e2e/wizard-navigation.spec.ts` | Validar "< Atras" preserva seleccion | MEDIA |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida (feature-spec) | Especificada (ui-ux) | Componentes Planificados | Estado |
|--------------------------------|----------------------|--------------------------|--------|
| Paso 1: Galeria de Templates | ✅ Pantalla 1 | ❌ TemplateGallery, TemplateCard | **NO PLANIFICADO** |
| Paso 2: Personalizar Necesidades | ✅ Pantalla 2 | ❌ NecesidadesForm, NecesidadItem | **NO PLANIFICADO** |
| Paso 3: Confirmar y Publicar | ✅ Pantalla 3 | ❌ ResumenConfirmacion | **NO PLANIFICADO** |
| WizardStepper (shared) | ✅ Componente | ❌ WizardStepper | **NO PLANIFICADO** |
| BudgetSummary (sticky) | ✅ Componente | ❌ BudgetSummary | **NO PLANIFICADO** |
| RolProfesionalTooltip | ✅ Componente | ❌ RolTooltip | **NO PLANIFICADO** |

### Estados de UI

| Estado | Requerido (ui-ux) | Planificado | Estado |
|--------|-------------------|-------------|--------|
| Loading (galeria) | ✅ Skeleton loaders | ❌ NO | **NO PLANIFICADO** |
| Empty (sin templates) | ✅ Empty state | ❌ NO | **NO PLANIFICADO** |
| Error (fetch failed) | ✅ Toast notification | ❌ NO | **NO PLANIFICADO** |
| Hover (template card) | ✅ bg + border + shadow | ❌ NO | **NO PLANIFICADO** |
| Checkbox toggle | ✅ Recalcula totales | ❌ NO | **NO PLANIFICADO** |
| Budget validation error | ✅ Border rojo + mensaje | ❌ NO | **NO PLANIFICADO** |
| All unchecked | ✅ Boton disabled | ❌ NO | **NO PLANIFICADO** |
| Loading submit | ✅ Spinner + "Publicando..." | ❌ NO | **NO PLANIFICADO** |
| Success submit | ✅ Toast + redirect | ❌ NO | **NO PLANIFICADO** |

---

## 7. Validacion de Flujos

### Flujo Principal

| Paso | Accion | Especificado | Planificado | Estado |
|------|--------|--------------|-------------|--------|
| 1 | Usuario accede a `/crowdsourcing/nuevo-proyecto` | ✅ feature-spec L31 | ❌ NO | **NO PLANIFICADO** |
| 2 | Ve galeria de 6 templates | ✅ AC-CS01-1 | ❌ NO | **NO PLANIFICADO** |
| 3 | Click en template -> Paso 2 | ✅ feature-spec L33 | ❌ NO | **NO PLANIFICADO** |
| 4 | Ve desglose por fases | ✅ AC-CS01-2 | ❌ NO | **NO PLANIFICADO** |
| 5 | Marca/desmarca necesidades | ✅ AC-CS01-3 | ❌ NO | **NO PLANIFICADO** |
| 6 | Ajusta presupuestos | ✅ AC-CS01-4 | ❌ NO | **NO PLANIFICADO** |
| 7 | Click "Siguiente" -> Paso 3 | ✅ feature-spec L43 | ❌ NO | **NO PLANIFICADO** |
| 8 | Ve resumen con totales | ✅ AC-CS01-7 | ❌ NO | **NO PLANIFICADO** |
| 9 | Click "Confirmar y publicar" | ✅ feature-spec L46 | ❌ NO | **NO PLANIFICADO** |
| 10 | POST a `/templates/{id}/generar` | ✅ contracts | ✅ contracts-plan | **CUBIERTO** |
| 11 | Redirect a `/crowdsourcing/mis-necesidades` | ✅ feature-spec L51 | ❌ NO | **NO PLANIFICADO** |

### Flujos Alternativos

| ID | Condicion | Accion | Especificado | Planificado | Estado |
|----|-----------|--------|--------------|-------------|--------|
| FA-01 | Sin ProyectoArtistico | Crea automaticamente o wizard | ✅ feature-spec L59 | ❌ NO | **NO PLANIFICADO** |
| FA-02 | Todas deseleccionadas | Boton disabled + mensaje | ✅ feature-spec L60 | ❌ NO | **NO PLANIFICADO** |
| FA-03 | Click "< Atras" | Navega sin perder datos | ✅ feature-spec L61 | ❌ NO | **NO PLANIFICADO** |
| FA-04 | Presupuesto muy alto | Solo mensaje informativo | ✅ feature-spec L62 | ❌ NO | **NO PLANIFICADO** |
| FA-05 | Template inactivo | No aparece en galeria | ✅ feature-spec L63 | N/A (backend) | **CUBIERTO (backend)** |
| FA-06 | Error creacion BD | Toast error + rollback | ✅ feature-spec L64 | ❌ NO | **NO PLANIFICADO** |

---

## 8. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear `frontend-plan.md`**
   - Archivo: `plans/cs-templates-guia/frontend-landing/frontend-plan.md`
   - Contenido:
     - Arquitectura de features/crowdsourcing/
     - Componentes: TemplateGallery, NecesidadesForm, ResumenConfirmacion, WizardStepper, NecesidadItem, BudgetSummary
     - Hooks: useTemplates, useTemplateDetail, useGenerarNecesidades
     - Services: templates.service.ts
     - State management: Zustand store para wizard state + localStorage persistence
     - Routing: `/crowdsourcing/nuevo-proyecto` con query params `?step=1|2|3&template={id}`

2. **Crear `ui-design.md`**
   - Archivo: `plans/cs-templates-guia/frontend-landing/ui-design.md`
   - Contenido:
     - Specs detalladas de cada componente (props, estilos, estados)
     - Referencia a design tokens de ui-ux.md
     - Componentes shared: WizardStepper, TemplateCard, NecesidadItem, BudgetSummary, RolTooltip
     - Responsive breakpoints: mobile (1 col), tablet (2 cols), desktop (3 cols)
     - Estados de UI: loading, empty, error, hover, validation errors
     - Accesibilidad: ARIA labels, keyboard navigation, focus states

3. **Crear `test-strategy.md`**
   - Archivo: `plans/cs-templates-guia/frontend-landing/test-strategy.md`
   - Contenido:
     - E2E tests: wizard full flow (3 pasos), navegacion, validaciones
     - Unit tests: NecesidadItem, BudgetInput, BudgetSummary, schemas Zod
     - Integration tests: useGenerarNecesidades mutation, budget recalculo
     - Coverage target: 80%+
     - Test data: mock de 1 template con 3 necesidades
     - Casos de prueba por criterio de aceptacion

4. **Agregar validacion de ownership en frontend-plan**
   - Validar que el proyecto seleccionado pertenece al artista antes de submit
   - Manejar 403 Forbidden del backend con toast: "No tienes permiso para modificar este proyecto"

5. **Especificar cache strategy**
   - TanStack Query con `staleTime: 5 * 60 * 1000` (5 min) para templates (datos maestros)
   - `staleTime: 10 * 60 * 1000` (10 min) para roles/categorias (cambios muy poco)
   - Invalidar cache de templates tras crear necesidades (opcional)

### Acciones Sugeridas (Mayor)

1. **Agregar manejo de errores robusto**
   - En frontend-plan.md: Error boundary component
   - Mapeo de errorCodes del backend a mensajes user-friendly (usar ERROR_MESSAGES de contracts-plan)
   - Toast notifications con actions (ej: "Reintentar")

2. **Implementar analytics tracking**
   - Track eventos: template_selected, necesidad_toggled, presupuesto_changed, wizard_completed
   - Datos utiles: template_id, cantidad_necesidades_seleccionadas, presupuesto_total

3. **Optimizar performance**
   - Lazy load de Paso 2 y Paso 3 (React.lazy + Suspense)
   - Debounce en budget inputs (300ms) para evitar re-renders excesivos
   - Virtualizacion de lista de necesidades si > 20 items

4. **Mejorar UX**
   - Animaciones smooth entre pasos (fade in/out)
   - Count-up animation en totales de presupuesto
   - Shake animation en inputs con error de validacion
   - Confetti animation al confirmar (opcional)

### Nice to Have (Menor)

1. **Agregar preview de template**
   - Modal con preview de todas las necesidades del template antes de seleccionar
   - Ayuda a artista a decidir mejor

2. **Implementar "Guardar borrador"**
   - Persistir seleccion en localStorage
   - Permitir continuar wizard despues (recuperar state)

3. **Agregar comparador de templates**
   - Checkbox en cards de galeria
   - Tabla comparativa lado a lado (precio, necesidades, fases)

4. **Exportar resumen a PDF**
   - Boton en Paso 3 para descargar resumen del proyecto
   - Util para artista para compartir con equipo

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [ ] AC-CS01-1: Galeria de 6 templates planificada (TemplateGallery component)
- [ ] AC-CS01-2: Desglose por fases planificado (NecesidadesForm con agrupacion)
- [ ] AC-CS01-3: Checkboxes con pre-seleccion planificados (NecesidadItem)
- [ ] AC-CS01-4: Validacion presupuestos planificada (generarNecesidadesSchema)
- [x] AC-CS01-5: Endpoint generar necesidades cubierto (POST /generar)
- [x] AC-CS01-6: Vinculacion a proyecto cubierto (proyectoArtisticoId en request)
- [ ] AC-CS01-7: Resumen confirmacion planificado (ResumenConfirmacion component)
- [x] AC-CS01-8: Seed data cubierto (backend responsibility)
- [ ] AC-CS01-9: Tooltips roles planificados (RolTooltip component)

### UI/UX
- [ ] WizardStepper component especificado
- [ ] BudgetSummary component especificado
- [ ] Responsive breakpoints documentados
- [ ] Estados de UI completos (loading, error, empty, hover, validation)
- [ ] Accesibilidad: ARIA labels documentados
- [ ] Keyboard navigation planificada
- [ ] Animaciones especificadas (hover, transitions)
- [ ] Dark theme aplicado (design tokens)

### Testing
- [ ] E2E test para flujo completo (3 pasos)
- [ ] Unit tests para componentes criticos
- [ ] Integration tests para mutations
- [ ] Validacion de schemas Zod testeada
- [ ] Coverage objetivo >= 80% definido
- [ ] Test data/mocks especificados

### State Management
- [ ] Wizard state management definido (Zustand/Context)
- [ ] localStorage persistence especificada
- [ ] Navegacion entre pasos sin perder datos
- [ ] URL params para step y template definidos

### Performance
- [ ] Cache strategy especificada (staleTime TanStack Query)
- [ ] Lazy loading de steps planificado
- [ ] Debounce en inputs definido
- [ ] Optimizaciones documentadas

---

## 10. Conclusion

**Score Final:** 33% (3/9 criterios cubiertos por planes)

**Veredicto:** ❌ REQUIERE CAMBIOS

**Problema Principal:**
La feature esta **100% especificada** en los documentos de diseño (feature-spec.md, contracts.md, ui-ux.md) y el plan de contratos compartidos (contracts-plan.md) esta completo. Sin embargo, **NO EXISTEN los planes de implementacion para la aplicacion Landing**:

- ❌ `frontend-plan.md` - Arquitectura, componentes, hooks, routing
- ❌ `ui-design.md` - Especificaciones detalladas de UI
- ❌ `test-strategy.md` - Estrategia de testing E2E/unit/integration

**Gaps Criticos:**
- 6 de 9 criterios de aceptacion no tienen plan de implementacion
- Todos los requisitos no funcionales de frontend no planificados
- Flujo completo del wizard (3 pasos) sin arquitectura definida
- State management no especificado
- Testing strategy completamente ausente

**Proximo Paso:**
1. **BLOQUEAR IMPLEMENTACION** hasta que se generen los 3 planes faltantes
2. Crear `frontend-plan.md` con arquitectura completa
3. Crear `ui-design.md` con specs de componentes
4. Crear `test-strategy.md` con casos de prueba
5. Re-ejecutar esta validacion QA para verificar cobertura 90%+

**Nota:**
El backend esta bien especificado (contracts.md tiene todos los endpoints, DTOs, validaciones). El problema es exclusivamente del frontend Landing. Una vez creados los planes, la implementacion puede proceder en paralelo (backend + frontend).

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-15
**Estado:** BLOCKED - Requiere planes de implementacion frontend antes de proceder
