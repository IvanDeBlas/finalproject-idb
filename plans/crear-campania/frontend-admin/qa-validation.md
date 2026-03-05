# Validacion QA: Crear Campania (Admin Dashboard)

**Fecha:** 2026-02-12
**Feature:** crear-campania (US-02)
**Target:** src/admin (Next.js 14)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos Identificados | 13 |
| Cubiertos | 0 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 13 |
| **Score de Cobertura** | **0%** |

**Estado:** ❌ **RECHAZADO - PLANES NO EXISTEN**

**Razon Critica:** No existen planes de implementacion para validar. Los siguientes archivos estan ausentes:
- `plans/crear-campania/frontend-admin/frontend-plan.md`
- `plans/crear-campania/frontend-admin/ui-design.md`
- `plans/crear-campania/frontend-admin/test-strategy.md`

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Relevante para Admin |
|----|----------|------|---------------------|
| AC-02-1 | Campana creada en estado BORRADOR | Funcional | ✅ Si |
| AC-02-2 | Meta financiera > 0 validacion frontend | Funcional | ✅ Si |
| AC-02-3 | Fecha fin minimo 7 dias | Funcional | ✅ Si |
| AC-02-4 | Fecha fin maximo 60 dias | Funcional | ✅ Si |
| AC-02-5 | Solo artista dueno puede editar | Funcional | ✅ Si (validacion UI) |
| AC-02-6 | Publicar cambia estado a PUBLICADA | Funcional | ✅ Si (boton publicar) |
| AC-02-7 | Borrador NO visible en landing | Funcional | ❌ No (responsabilidad backend) |
| AC-02-8 | Publicada visible en landing | Funcional | ❌ No (responsabilidad landing) |
| AC-02-9 | Artista puede editar borrador | Funcional | ✅ Si (flujo editar) |
| AC-02-10 | FechaInicio null al crear | Funcional | ❌ No (responsabilidad backend) |
| AC-02-11 | Wizard con stepper (1/4 a 4/4) | Funcional | ✅ Si |
| AC-02-12 | Rich text editor en descripcion | Funcional | ✅ Si |
| AC-02-13 | Schemas Zod en shared reutilizados | Tecnico | ✅ Si |

**Total Criterios Aplicables a Admin:** 10

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|---------------|-----------|---------------|--------|
| AC-02-1 | Campana en BORRADOR al crear | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-2 | Validacion meta > 0 | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-3 | Fecha fin >= hoy + 7 dias | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-4 | Fecha fin <= hoy + 60 dias | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-5 | Solo dueno edita (UI) | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-6 | Boton Publicar | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-9 | Editar borrador | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-11 | Wizard stepper 1/4 a 4/4 | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-12 | Rich text editor | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO CUBIERTO |
| AC-02-13 | Schemas Zod shared | ✅ contracts-plan | ❌ NO EXISTE | ❌ NO EXISTE | 🟡 PARCIAL |

**Leyenda:**
- ❌ NO CUBIERTO: Requisito no mencionado en planes (porque no existen)
- 🟡 PARCIAL: Requisito parcialmente cubierto (solo en shared)
- ✅ CUBIERTO: Requisito completamente implementado en plan (ninguno)

**Nota:** AC-02-13 tiene cobertura parcial porque los schemas Zod estan planificados en `contracts-plan.md` del shared, pero no hay plan de frontend que documente su uso.

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile (breakpoints) | ❌ NO EXISTE | ❌ NO CUBIERTO |
| NFR-02 | Accesibilidad WCAG AA | ❌ NO EXISTE | ❌ NO CUBIERTO |
| NFR-03 | Validacion instantanea (UX) | ❌ NO EXISTE | ❌ NO CUBIERTO |
| NFR-04 | Loading states | ❌ NO EXISTE | ❌ NO CUBIERTO |
| NFR-05 | Error handling | ❌ NO EXISTE | ❌ NO CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-001 | TODOS los criterios | No existe plan de implementacion frontend | CRITICO | Generar `frontend-plan.md` con arquitectura de wizard, componentes, hooks, services |
| GAP-002 | AC-02-11 | Wizard con stepper no planificado | CRITICO | Documentar componente Wizard, Stepper, navegacion entre pasos, persistencia de datos |
| GAP-003 | AC-02-12 | Rich text editor no planificado | CRITICO | Especificar libreria (TipTap/Quill), integracion, sanitizacion HTML |
| GAP-004 | AC-02-2, AC-02-3, AC-02-4 | Validaciones Zod no documentadas | CRITICO | Documentar uso de schemas shared, integracion react-hook-form |
| GAP-005 | AC-02-6, AC-02-9 | Flujo publicar/editar no planificado | CRITICO | Documentar componente Vista Previa, modal confirmacion, estados borrador/publicada |
| GAP-006 | TODOS | No existe ui-design.md | CRITICO | Generar especificacion UI con mockups, componentes shadcn, estilos Tailwind |
| GAP-007 | TODOS | No existe test-strategy.md | CRITICO | Definir estrategia testing: unit tests, integration tests, cobertura objetivo |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-008 | AC-02-1 | Estado BORRADOR UI | ALTO | Documentar badge/banner indicador de borrador en preview |
| GAP-009 | AC-02-5 | Validacion ownership | ALTO | Documentar manejo de 403 Forbidden, redirect, mensajes error |
| GAP-010 | NFR-01 | Responsive design | ALTO | Documentar breakpoints, comportamiento mobile del wizard |
| GAP-011 | NFR-02 | Accesibilidad | ALTO | Documentar ARIA labels, keyboard navigation, focus management |
| GAP-012 | NFR-03 | Validacion real-time | ALTO | Documentar debouncing, character counters, error messages inline |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-013 | - | No hay estructura de carpetas | MEDIO | Documentar organizacion de archivos (components, hooks, services) |
| GAP-014 | - | No hay definicion de rutas | MEDIO | Documentar routing `/dashboard/campanias/nueva?step=N` |
| GAP-015 | - | No hay estrategia cache | BAJO | Documentar localStorage para progreso wizard (opcional) |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

**NO APLICABLE** - No existe plan de testing.

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-02-2 | - | - | ❌ NO PLANIFICADO |
| AC-02-3 | - | - | ❌ NO PLANIFICADO |
| AC-02-4 | - | - | ❌ NO PLANIFICADO |
| AC-02-11 | - | - | ❌ NO PLANIFICADO |
| AC-02-12 | - | - | ❌ NO PLANIFICADO |

### Tests Requeridos Pero No Planificados

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-02-2 | test-validation-meta-objetivo | Validar que meta <= 0 muestra error |
| AC-02-3 | test-validation-fecha-minima | Validar que fecha < hoy+7 muestra error |
| AC-02-4 | test-validation-fecha-maxima | Validar que fecha > hoy+60 muestra error |
| AC-02-11 | test-wizard-navigation | Validar navegacion paso a paso mantiene datos |
| AC-02-12 | test-rich-editor-format | Validar que rich editor aplica formatos |
| AC-02-6 | test-publicar-campana | Validar que publicar cambia estado y redirige |
| AC-02-9 | test-editar-borrador | Validar que editar carga datos correctos |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| Wizard Paso 1: Info Basica | ❌ NO | - | ❌ NO CUBIERTO |
| Wizard Paso 2: Meta Financiera | ❌ NO | - | ❌ NO CUBIERTO |
| Wizard Paso 3: Duracion | ❌ NO | - | ❌ NO CUBIERTO |
| Wizard Paso 4: Multimedia | ❌ NO | - | ❌ NO CUBIERTO |
| Vista Previa Borrador | ❌ NO | - | ❌ NO CUBIERTO |
| Listado Mis Campanias | ❌ NO | - | ❌ NO CUBIERTO |
| Editar Campana | ❌ NO | - | ❌ NO CUBIERTO |

**Referencia UI:** El archivo `ui-ux.md` define mockups detallados (WPR_6-Create-Campaign.png), pero no hay plan de implementacion que los traduzca a componentes React.

### Estados de UI Requeridos

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading (crear campana) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |
| Error (validacion) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |
| Success (campana creada) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |
| Empty (sin campanias) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |
| Draft banner (borrador) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |
| 403 Forbidden (no owner) | ✅ Si | ❌ NO | ❌ NO CUBIERTO |

---

## 7. Validacion de Contratos

### Integracion con Shared

| Contrato | Definido en shared | Uso planificado en Admin | Estado |
|----------|-------------------|--------------------------|--------|
| `Campania` type | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `CreateCampaniaRequest` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `UpdateCampaniaRequest` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `PublishCampaniaResponse` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `createCampaniaSchema` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `campaniaBasicInfoSchema` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `campaniaFundingSchema` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `campaniaDurationSchema` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `campaniaMediaSchema` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `QUERY_KEYS.campanias` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `API_ROUTES.campanias` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |
| `APP_ROUTES.dashboard` | ✅ contracts-plan | ❌ NO | 🟡 PARCIAL |

**Nota:** Los contratos estan definidos en shared, pero no hay documentacion de como Admin los consumira.

### Endpoints Backend vs Frontend Plan

| Endpoint | Definido en contracts.md | Service planificado | Hook planificado | Estado |
|----------|--------------------------|---------------------|------------------|--------|
| `POST /api/campanias` | ✅ Si | ❌ NO | ❌ NO | ❌ NO CUBIERTO |
| `GET /api/campanias/{id}` | ✅ Si | ❌ NO | ❌ NO | ❌ NO CUBIERTO |
| `PUT /api/campanias/{id}` | ✅ Si | ❌ NO | ❌ NO | ❌ NO CUBIERTO |
| `POST /api/campanias/{id}/publicar` | ✅ Si | ❌ NO | ❌ NO | ❌ NO CUBIERTO |
| `GET /api/campanias/mis-campanias` | ✅ Si | ❌ NO | ❌ NO | ❌ NO CUBIERTO |

---

## 8. Recomendaciones

### Acciones Requeridas (Critico - BLOQUEANTE)

Estos gaps DEBEN resolverse antes de proceder con implementacion:

#### 1. **Generar frontend-plan.md**
   - **Archivo:** `plans/crear-campania/frontend-admin/frontend-plan.md`
   - **Contenido requerido:**
     - Arquitectura de wizard (4 pasos)
     - Componentes React (WizardLayout, StepIndicator, FormSteps 1-4, PreviewCampana, ListCampanias)
     - Hooks custom (useWizardState, useCreateCampania, useMisCampanias, usePublicarCampania)
     - Services (campaniaService.ts con metodos create, update, publish, getMisCampanias)
     - State management (localStorage para progreso wizard, React Query para server state)
     - Routing (Next.js App Router: /dashboard/campanias/nueva?step=N)
     - Integracion con shared (imports de types, schemas, constants)
     - Manejo de errores (mapeo ServiceResponse, toasts, validacion)

#### 2. **Generar ui-design.md**
   - **Archivo:** `plans/crear-campania/frontend-admin/ui-design.md`
   - **Contenido requerido:**
     - Wireframes por pantalla (Wizard Step 1-4, Preview, Lista)
     - Componentes shadcn/ui (Button, Input, Select, Card, Badge, Alert, Calendar, Textarea)
     - Rich text editor (TipTap o Quill, configuracion, toolbar)
     - Estilos Tailwind (dark theme, gradient buttons, responsive)
     - Estados visuales (loading, error, success, empty, draft)
     - Animaciones (stepper progress, transitions)
     - Accesibilidad (ARIA labels, keyboard nav, focus management)

#### 3. **Generar test-strategy.md**
   - **Archivo:** `plans/crear-campania/frontend-admin/test-strategy.md`
   - **Contenido requerido:**
     - Unit tests (schemas Zod, utils, hooks custom)
     - Integration tests (wizard flow, create campana, publicar)
     - Component tests (WizardStep forms, validation, navigation)
     - E2E tests (flujo completo crear → preview → publicar)
     - Cobertura objetivo (80%+ para utils y hooks)
     - Mocking strategy (API calls, localStorage)

### Acciones Sugeridas (Mayor)

#### 4. **Documentar Flujo de Datos**
   - Wizard state management (como mantener datos entre pasos)
   - Validacion progresiva (step by step vs final)
   - Persistencia local (localStorage keys, serialization)
   - Sincronizacion con backend (when to POST)

#### 5. **Documentar Manejo de Errores**
   - Mapeo de ErrorCodes del backend
   - Toast notifications (sonner configuracion)
   - Validacion inline vs submit
   - 401/403 handling (redirect to login)

#### 6. **Documentar Rich Text Editor**
   - Libreria seleccionada (TipTap recomendado)
   - Configuracion toolbar (bold, italic, lists, links)
   - Sanitizacion HTML output
   - Preview rendering

### Nice to Have (Menor)

#### 7. **Optimizaciones UX**
   - Auto-save wizard progress
   - Character counters live
   - Image preview optimizado
   - Drag & drop file upload

#### 8. **Telemetria**
   - Analytics de abandono por paso
   - Errores mas comunes de validacion
   - Tiempo promedio por paso

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- ❌ Todos los criterios de aceptacion tienen cobertura
- ❌ Flujos de usuario completos planificados
- ❌ Casos de error cubiertos

### UI/UX
- ❌ Todas las screens planificadas
- ❌ Estados de interaccion definidos
- ❌ Responsive design considerado
- ❌ Accesibilidad validada

### Testing
- ❌ Tests para criterios criticos
- ❌ Tests de integracion para flujos
- ❌ Cobertura objetivo definida

### Integracion
- 🟡 Contratos shared definidos (pero uso no documentado)
- ❌ Services API planificados
- ❌ Hooks React Query planificados
- ❌ Routing configurado

---

## 10. Dependencias Bloqueantes

### Para Empezar Implementacion

El frontend Admin NO PUEDE empezar implementacion hasta que:

1. ✅ **Contratos Shared completados** - `contracts-plan.md` existe
2. ❌ **Frontend Plan generado** - BLOQUEANTE
3. ❌ **UI Design generado** - BLOQUEANTE
4. ❌ **Test Strategy generado** - BLOQUEANTE

### Dependencias Externas

- ✅ Backend endpoints documentados en `contracts.md`
- ✅ UI/UX mockups disponibles en `ui-ux.md`
- 🟡 Shared types/schemas planificados (falta implementar)
- ❌ Backend endpoints implementados (desarrollo paralelo posible)

---

## 11. Comparacion con Requisitos del Spec

### Del feature-spec.md - Seccion "Proyectos Involucrados"

**Admin - Responsabilidades esperadas:**

> Implementar wizard de 4 pasos en `/dashboard/campanias/nueva` con stepper horizontal. Crear formularios para cada paso con validacion Zod. Implementar rich text editor para descripcion (TipTap o Quill). Implementar vista previa en `/dashboard/campanias/{id}/preview`. Crear pagina de listado de campanias del artista en `/dashboard/campanias` mostrando estado (BORRADOR / PUBLICADA). Implementar boton "Publicar" que llama a endpoint de publicacion. Gestionar estados de carga y errores con react-query.

**Cobertura actual:** 0%

**Elementos faltantes en planes:**
- ❌ Wizard de 4 pasos
- ❌ Stepper horizontal
- ❌ Formularios por paso
- ❌ Validacion Zod integrada
- ❌ Rich text editor
- ❌ Vista previa
- ❌ Listado campanias
- ❌ Boton publicar
- ❌ Estados de carga/error

---

## 12. Conclusion

**Score Final:** 0%

**Veredicto:** ❌ **RECHAZADO**

**Razon Principal:** No existen planes de implementacion para validar. No es posible evaluar cobertura de criterios de aceptacion sin documentacion de la solucion propuesta.

**Proximo Paso:**

### Accion Inmediata Requerida

1. **GENERAR `frontend-plan.md`** con:
   - Arquitectura de componentes
   - Hooks y services
   - Integracion con shared
   - State management
   - Routing

2. **GENERAR `ui-design.md`** con:
   - Especificacion de componentes UI
   - Estilos y temas
   - Estados visuales
   - Responsive design
   - Accesibilidad

3. **GENERAR `test-strategy.md`** con:
   - Estrategia de testing
   - Tipos de tests
   - Cobertura objetivo
   - Herramientas

### Una vez generados los planes

1. **Ejecutar validacion QA nuevamente** para evaluar cobertura
2. **Resolver gaps criticos y mayores** antes de implementar
3. **Proceder con implementacion** solo si score >= 70%

---

## 13. Informacion Adicional

### Recursos Disponibles

**Ya documentados:**
- ✅ `docs/user-stories/crear-campania/feature-spec.md` - Requisitos completos
- ✅ `docs/user-stories/crear-campania/contracts.md` - Contratos API
- ✅ `docs/user-stories/crear-campania/ui-ux.md` - Mockups y diseno
- ✅ `plans/crear-campania/shared/contracts-plan.md` - Plan shared

**Faltantes:**
- ❌ `plans/crear-campania/frontend-admin/frontend-plan.md`
- ❌ `plans/crear-campania/frontend-admin/ui-design.md`
- ❌ `plans/crear-campania/frontend-admin/test-strategy.md`

### Estimacion de Impacto

**Sin planes de implementacion:**
- ❌ Desarrolladores no pueden empezar a implementar
- ❌ No hay guia arquitectural
- ❌ No hay estimacion de esfuerzo
- ❌ Alto riesgo de implementacion incorrecta
- ❌ No hay estrategia de testing

**Tiempo estimado para generar planes:** 4-6 horas
**Tiempo estimado para implementacion:** 16-20 horas (una vez planes aprobados)

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-12
**Revision:** 1.0

---

## ANEXO A: Criterios de Aceptacion Detallados

### Criterios Relevantes para Admin Dashboard

#### AC-02-1: Campana creada en estado BORRADOR
- **Descripcion:** Al crear campana (POST /api/campanias), el backend debe establecer EstadoCampaniaId = 1
- **Validacion Frontend:** Badge/banner visual indicando "Borrador" en vista previa
- **Test:** Verificar que response.data.estadoCampaniaId === 1 despues de crear

#### AC-02-2: Meta financiera > 0 validacion frontend
- **Descripcion:** El campo importeObjetivo debe validarse como > 0 antes de permitir submit
- **Validacion Frontend:** Schema Zod con `.positive()` y mensaje de error
- **Test:** Ingresar 0 o negativo debe mostrar error "La meta debe ser mayor a cero"

#### AC-02-3: Fecha fin >= hoy + 7 dias
- **Descripcion:** La fecha fin debe ser al menos 7 dias desde hoy
- **Validacion Frontend:** Schema Zod con refine usando date-fns addDays
- **Test:** Seleccionar fecha < hoy+7 debe mostrar error "La campana debe durar al menos 7 dias"

#### AC-02-4: Fecha fin <= hoy + 60 dias
- **Descripcion:** La fecha fin no puede superar 60 dias desde hoy
- **Validacion Frontend:** Schema Zod con refine validando max date
- **Test:** Seleccionar fecha > hoy+60 debe mostrar error "La campana no puede durar mas de 60 dias"

#### AC-02-5: Solo artista dueno puede editar
- **Descripcion:** Backend retorna 403 si ArtistaId token != ArtistaId campana
- **Validacion Frontend:** Manejar error 403, mostrar toast "No tienes permiso", redirect
- **Test:** Simular 403, verificar mensaje y redirect

#### AC-02-6: Publicar cambia estado a PUBLICADA
- **Descripcion:** POST /api/campanias/{id}/publicar cambia EstadoCampaniaId a 2
- **Validacion Frontend:** Boton "Publicar Ahora" llama endpoint, maneja success/error
- **Test:** Click publicar debe cambiar estado y redirigir a /dashboard/campanias

#### AC-02-9: Artista puede editar borrador
- **Descripcion:** PUT /api/campanias/{id} permite actualizar campana si estado = BORRADOR
- **Validacion Frontend:** Ruta /dashboard/campanias/{id}/editar carga wizard pre-completado
- **Test:** Editar borrador debe pre-llenar formulario con datos existentes

#### AC-02-11: Wizard con stepper (1/4 a 4/4)
- **Descripcion:** UI debe mostrar indicador visual de progreso (paso actual de 4)
- **Validacion Frontend:** Componente Stepper con 4 pasos, highlight en paso actual
- **Test:** Navegacion paso a paso debe actualizar stepper visual

#### AC-02-12: Rich text editor en descripcion
- **Descripcion:** Campo descripcionCorta debe usar editor WYSIWYG con formatos basicos
- **Validacion Frontend:** Integracion TipTap/Quill con toolbar (bold, italic, lists, links)
- **Test:** Aplicar formato debe reflejarse en preview HTML

#### AC-02-13: Schemas Zod en shared reutilizados
- **Descripcion:** Los schemas de validacion deben importarse desde src/shared/schemas
- **Validacion Frontend:** Imports de campaniaBasicInfoSchema, campaniaFundingSchema, etc.
- **Test:** Validacion debe usar schemas compartidos (no duplicados)

---

**FIN DEL INFORME**
