# Estrategia de Testing: PromoPrograma (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-programas-promocion (US-CP-02)
**Target:** `src/admin`
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Prioridad | Cobertura estimada |
|------|----------|-----------|-------------------|
| Unit Tests (schemas Zod) | 42 | P0 | 95% logica de validacion |
| Unit Tests (componentes puros) | 31 | P1 | 85% renderizado |
| Integration Tests (wizard) | 18 | P0 | 90% flujos criticos |
| Integration Tests (hooks) | 29 | P0 | 85% |
| Integration Tests (service) | 20 | P1 | 90% |
| **Total** | **140** | | **80%+** |

### Criterios de aceptacion cubiertos por los tests

| AC | Criterio | Suite responsable |
|----|----------|------------------|
| AC-CP02-1 | Wizard 4 pasos crea programa | WizardContainer.test, useCreatePromoPrograma.test |
| AC-CP02-4 | Al menos una comision | createPromoProgramaSchema.test, Paso2Comisiones.test |
| AC-CP02-5 | Listado paginado con filtro | MisProgramasPage.test, usePromoProgramas.test |
| AC-CP02-6 | Edicion con aviso promotores | ProgramaEditPage.test |
| AC-CP02-8 | CodigoTrackingBase unico (backend) | useCreatePromoPrograma.test (error 1024) |
| AC-CP02-9 | EsRepetible requiere MaxRepeticiones | createPromoTareaSchema.test |
| AC-CP02-10 | Programa sin campana valido | Paso1DatosBasicos.test |
| FA-02 | Programa sin tareas valido | WizardContainer.test, createPromoProgramaSchema.test |
| FA-03 | FechaFin >= FechaInicio | createPromoProgramaSchema.test |
| FA-06 | EsRepetible sin MaxRepeticiones | PromoTareaFormItem.test |
| FA-08 | Error de red mantiene estado wizard | WizardContainer.test |

---

## 2. Estructura de Tests

```
src/admin/src/
├── __mocks__/
│   └── promo-programa.mock.ts               <- Fixtures centralizados
│
├── app/(dashboard)/crowdpromotion/programas/
│   ├── __tests__/
│   │   └── schemas/
│   │       ├── createPromoProgramaSchema.test.ts
│   │       └── createPromoTareaSchema.test.ts
│   │
│   ├── nuevo/
│   │   └── components/
│   │       └── __tests__/
│   │           ├── WizardContainer.test.tsx
│   │           ├── WizardStepper.test.tsx
│   │           ├── Paso1DatosBasicos.test.tsx
│   │           ├── Paso2Comisiones.test.tsx
│   │           ├── Paso3Tareas.test.tsx
│   │           ├── Paso4Revision.test.tsx
│   │           ├── PromoTareaFormItem.test.tsx
│   │           └── AbandonarWizardDialog.test.tsx
│   │
│   ├── components/
│   │   └── __tests__/
│   │       ├── MisProgramasPage.test.tsx
│   │       ├── PromoProgramaCard.test.tsx
│   │       ├── PromoProgramaStatusBadge.test.tsx
│   │       ├── PromoProgramaFilters.test.tsx
│   │       └── EmptyStateProgramas.test.tsx
│   │
│   └── [id]/
│       └── components/
│           └── __tests__/
│               ├── PromoProgramaDetailPage.test.tsx
│               ├── PromoProgramaKPICards.test.tsx
│               ├── PromoTareasSection.test.tsx
│               ├── PromotoresSection.test.tsx
│               └── DesactivarProgramaDialog.test.tsx
│
├── hooks/
│   └── __tests__/
│       ├── use-promo-programas.test.ts
│       ├── use-promo-programa-detail.test.ts
│       ├── use-promo-programa-mutations.test.ts
│       └── use-wizard-promo-state.test.ts
│
└── services/
    └── __tests__/
        └── promo-programa.service.test.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data Central

**Archivo:** `src/admin/src/__mocks__/promo-programa.mock.ts`

Este archivo centraliza todos los fixtures de la feature. Patron heredado de `promotor.mock.ts`.

**Exports requeridos:**

```typescript
// --- PromoProgramaListItem fixtures ---
export const mockProgramaActivo: PromoProgramaListItem
// Programa activo, comision porcentaje, con campana vinculada, 3 tareas, 5 promotores

export const mockProgramaInactivo: PromoProgramaListItem
// Programa inactivo, comision fija, sin campana, 0 promotores

export const mockProgramaSinTareas: PromoProgramaListItem
// Programa activo, sin tareas (numeroTareas: 0), para testear FA-02

export const mockProgramaListResult: PromoProgramaListResult
// items: [mockProgramaActivo, mockProgramaInactivo], totalCount: 2, page: 1, pageSize: 10

// --- PromoProgramaDetail fixtures ---
export const mockProgramaDetail: PromoProgramaDetail
// Detalle completo con 2 tareas activas, 2 promotores, resumen KPIs

export const mockProgramaDetailInactivo: PromoProgramaDetail
// Igual pero esActivo: false, para testear boton desactivar oculto

export const mockProgramaDetailSinTareas: PromoProgramaDetail
// Detalle con tareas: [], para testear empty state de tareas

// --- PromoTareaDetail fixtures ---
export const mockTareaShare: PromoTareaDetail
// tipoEventoPromoNombre: 'Share', tipoRewardNombre: 'Dinero', esRepetible: true

export const mockTareaBacking: PromoTareaDetail
// tipoEventoPromoNombre: 'Backing', tipoRewardNombre: 'Puntos', esRepetible: false

// --- Result fixtures ---
export const mockCreatedResult: PromoProgramaCreatedResult
// id: 'prog-123', tareasCreadas: 2

export const mockUpdatedResult: PromoProgramaUpdatedResult
// id: 'prog-123', fechaActualizacion: '2026-02-25T10:00:00Z'

export const mockDesactivadoResult: PromoProgramaDesactivadoResult
// id: 'prog-123', esActivo: false, tareasDesactivadas: 2

// --- Wizard form data fixtures ---
export const mockWizardStep1Data: CreatePromoProgramaFormData (partial)
// titulo, tipoPromoId, monedaId, fechaInicio, fechaFin

export const mockWizardStep2Data: CreatePromoProgramaFormData (partial)
// importeComisionPorcentaje: 10.00

export const mockWizardStep3Data: CreatePromoProgramaFormData (partial)
// tareas: [{ titulo, tipoEventoPromoId, tipoRewardId, esRepetible, maxRepeticiones }]

export const mockWizardCompleteData: CreatePromoProgramaFormData
// Todos los campos del wizard combinados (para Paso4 y submit)
```

**Valores concretos para campos clave:**

```typescript
// mockProgramaActivo
{
    id: 'prog-001',
    titulo: 'Promociona mi nuevo album',
    tipoPromoId: 1,
    tipoPromoNombre: 'Referral',
    campaniaTitulo: 'Mi Album Debut',
    esActivo: true,
    importeComisionPorcentaje: 10.00,
    importeComisionFija: null,
    monedaNombre: 'EUR',
    numeroPromotores: 5,
    numeroTareas: 3,
    fechaInicio: '2026-03-01',
    fechaFin: '2026-06-01',
    fechaCreacion: '2026-02-25T10:00:00Z',
}

// mockProgramaDetail.resumen
{
    totalPromotoresAprobados: 5,
    totalPromotoresPendientes: 2,
    totalEventos: 150,
    totalConversiones: 12,
    valorTotalGenerado: 1200.00,
}
```

### 3.2 Mocks de Hooks y Services

Los tests de componentes mockean hooks completos (patron del proyecto). Los tests de hooks mockean el service directamente. Los tests de service mockean `apiFetch`.

**Patron para tests de componentes:**

```typescript
// Mock del hook de query
vi.mock('@/hooks/use-promo-programas', () => ({
    usePromoProgramas: () => ({
        data: mockProgramaListResult,
        isLoading: false,
        isError: false,
    }),
}))

// Mock del hook de mutacion
const mockMutate = vi.fn()
vi.mock('@/hooks/use-promo-programa-mutations', () => ({
    useCreatePromoPrograma: () => ({
        mutate: mockMutate,
        isPending: false,
    }),
    useDesactivarPromoPrograma: () => ({
        mutate: mockMutate,
        isPending: false,
    }),
}))

// Mock de router (Next.js 14)
const mockPush = vi.fn()
vi.mock('next/navigation', () => ({
    useRouter: () => ({ push: mockPush, back: vi.fn() }),
    useParams: () => ({ id: 'prog-001' }),
}))

// Mock de sonner
vi.mock('sonner', () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))
```

**Patron para tests de hooks:**

```typescript
vi.mock('@/services/promo-programa.service', () => ({
    promoProgramaService: {
        getMisProgramas: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}))
```

**Patron para tests de service:**

```typescript
vi.mock('@/lib/api-client', () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))
```

### 3.3 createWrapper (reutilizable)

El wrapper es el mismo que existe en el proyecto. Todos los tests de hooks usan `createWrapper()` para envolver con `QueryClientProvider` con `retry: false`.

---

## 4. Tests por Modulo

### 4.1 Schemas Zod (P0)

#### createPromoTareaSchema.test.ts

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/__tests__/schemas/createPromoTareaSchema.test.ts`
**Importa de:** `@shared/schemas/crowdpromotion.schema`

Estos son tests unitarios puros de logica de validacion. Son P0 porque los refines son la logica de negocio mas critica de la feature.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | valida datos minimos validos | Unit | titulo y campos obligatorios, esRepetible false |
| 2 | valida datos completos validos | Unit | todos los campos opcionales incluidos |
| 3 | titulo requerido | Unit | falla con titulo vacio |
| 4 | titulo min 3 caracteres | Unit | falla con 'Ab', pasa con 'Abc' |
| 5 | titulo max 200 caracteres | Unit | falla con 201 chars |
| 6 | tipoEventoPromoId requerido | Unit | falla sin el campo, falla con 0 |
| 7 | tipoRewardId requerido | Unit | falla sin el campo, falla con 0 |
| 8 | **Refine: esRepetible=true requiere maxRepeticiones** | Unit | falla si esRepetible:true y maxRepeticiones:undefined |
| 9 | **Refine: esRepetible=false no requiere maxRepeticiones** | Unit | pasa si esRepetible:false y maxRepeticiones:undefined |
| 10 | **Refine: esRepetible=true con maxRepeticiones valido pasa** | Unit | esRepetible:true, maxRepeticiones:5 -> valido |
| 11 | **Refine: maxRepeticiones path correcto en error** | Unit | error en path ['maxRepeticiones'] |
| 12 | **Refine: tipoRewardId=1 (Dinero) requiere importeRecompensa** | Unit | falla sin importeRecompensa |
| 13 | **Refine: tipoRewardId=3 (Mixto) requiere importeRecompensa** | Unit | falla sin importeRecompensa |
| 14 | **Refine: tipoRewardId=2 (Puntos) requiere puntosRecompensa** | Unit | falla sin puntosRecompensa |
| 15 | **Refine: tipoRewardId=2 no requiere importeRecompensa** | Unit | pasa sin importeRecompensa cuando tipoRewardId=2 |
| 16 | urlInstrucciones acepta string vacio | Unit | pasa con '' |
| 17 | urlInstrucciones valida formato URL | Unit | falla con 'no-es-url', pasa con URL valida |
| 18 | urlInstrucciones max 500 caracteres | Unit | falla con 501 chars |
| 19 | descripcion max 4000 caracteres | Unit | falla con 4001 chars |
| 20 | importeRecompensa no puede ser negativo | Unit | falla con -1 |
| 21 | puntosRecompensa no puede ser negativo | Unit | falla con -1 |

#### createPromoProgramaSchema.test.ts

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/__tests__/schemas/createPromoProgramaSchema.test.ts`
**Importa de:** `@shared/schemas/crowdpromotion.schema`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | valida datos minimos validos | Unit | titulo, tipoPromoId, monedaId, importeComisionPorcentaje |
| 2 | valida datos completos validos | Unit | todos los campos incluyendo tareas |
| 3 | titulo requerido | Unit | falla con titulo vacio |
| 4 | titulo min 5 caracteres | Unit | falla con 'Hola', pasa con 'Hola!' |
| 5 | titulo max 200 caracteres | Unit | falla con 201 chars |
| 6 | tipoPromoId requerido | Unit | falla sin el campo |
| 7 | monedaId requerido | Unit | falla sin el campo |
| 8 | **Refine: ambas comisiones null falla** | Unit | falla con importeComisionPorcentaje:undefined y importeComisionFija:undefined |
| 9 | **Refine: solo comision porcentaje definida pasa** | Unit | importeComisionPorcentaje:10, importeComisionFija:undefined |
| 10 | **Refine: solo comision fija definida pasa** | Unit | importeComisionPorcentaje:undefined, importeComisionFija:50 |
| 11 | **Refine: ambas comisiones definidas pasa** | Unit | importeComisionPorcentaje:10, importeComisionFija:50 |
| 12 | **Refine: error en path ['importeComisionPorcentaje']** | Unit | el path del error es correcto para ubicar en el form |
| 13 | **Refine: fechaFin posterior a fechaInicio** | Unit | falla si fechaFin <= fechaInicio |
| 14 | **Refine: fechaFin igual a fechaInicio falla** | Unit | '2026-03-01' > '2026-03-01' = false |
| 15 | **Refine: fechas iguales falla** | Unit | falla, message 'La fecha fin debe ser posterior...' |
| 16 | **Refine: sin fechas es valido (ambas opcionales)** | Unit | pasa si ambas fechas son undefined |
| 17 | **Refine: solo fechaFin sin fechaInicio es valido** | Unit | pasa (no hay fechaInicio para comparar) |
| 18 | codigoTrackingBase acepta alfanumerico y guiones | Unit | pasa con 'album-2026', falla con 'album_2026' (guion bajo) |
| 19 | codigoTrackingBase acepta string vacio | Unit | pasa con '' |
| 20 | codigoTrackingBase max 50 caracteres | Unit | falla con 51 chars |
| 21 | campaniaCrowdfundingId acepta UUID valido | Unit | pasa con UUID formato correcto |
| 22 | campaniaCrowdfundingId acepta undefined | Unit | pasa sin el campo |
| 23 | urlLanding acepta string vacio | Unit | pasa con '' |
| 24 | urlLanding valida formato URL | Unit | falla con 'no-es-url' |
| 25 | tareas puede ser array vacio | Unit | pasa con tareas: [] |
| 26 | tareas puede estar ausente | Unit | pasa sin el campo tareas |
| 27 | importeComisionPorcentaje no puede superar 100 | Unit | falla con 101 |
| 28 | importeComisionPorcentaje no puede ser negativo | Unit | falla con -1 |
| 29 | importeComisionFija no puede ser negativa | Unit | falla con -1 |

---

### 4.2 Wizard - Crear Programa (P0)

#### WizardContainer.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/WizardContainer.test.tsx`

Test de integracion del flujo completo. Mockea los hooks de mutacion y el router.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza en Paso 1 por defecto | Integration | el stepper muestra paso 1 activo |
| 2 | muestra boton Anterior deshabilitado en Paso 1 | Integration | primer paso no tiene vuelta atras |
| 3 | avanza a Paso 2 al completar Paso 1 valido | Integration | fill Paso1 + click Siguiente -> paso 2 visible |
| 4 | no avanza si Paso 1 tiene errores de validacion | Integration | titulo vacio -> no avanza, muestra errores |
| 5 | no avanza en Paso 2 si ninguna comision definida | Integration | AC-CP02-4: ambas comisiones vacias bloquea avance |
| 6 | avanza de Paso 2 a Paso 3 con comision valida | Integration | importeComisionPorcentaje:10 -> avanza |
| 7 | retrocede de Paso 2 a Paso 1 | Integration | click Anterior desde paso 2 -> paso 1 visible |
| 8 | puede agregar tarea en Paso 3 | Integration | click "Agregar tarea" -> formulario inline visible |
| 9 | puede eliminar tarea antes de avanzar | Integration | tarea agregada -> icono papelera -> tarea removida |
| 10 | Paso 3 sin tareas permite avanzar (FA-02) | Integration | puede avanzar al Paso 4 con 0 tareas |
| 11 | Paso 4 muestra resumen de datos | Integration | titulo y tipo de programa visible en revision |
| 12 | Paso 4 muestra tareas en resumen | Integration | tareas del paso 3 listadas en revision |
| 13 | submit llama a useCreatePromoPrograma.mutate con datos correctos | Integration | datos del wizard enviados al servicio |
| 14 | toast exito y redireccion al detalle tras submit exitoso | Integration | mockMutate onSuccess -> toast + push('/programas/prog-123') |
| 15 | **error de red mantiene estado del wizard (FA-08)** | Integration | mutacion falla -> wizard sigue en Paso 4 con datos intactos |
| 16 | click X abre AbandonarWizardDialog | Integration | boton X -> dialog de abandono visible |
| 17 | confirmar abandono redirige al listado | Integration | confirmar dialog -> push('/programas') |
| 18 | cancelar abandono cierra dialog y mantiene wizard | Integration | cancelar dialog -> wizard sigue visible |

#### WizardStepper.test.tsx (Promo)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/WizardStepper.test.tsx`

Nota: Si el wizard de PromoPrograma reutiliza el `WizardStepper` existente de campanias, estas pruebas pueden omitirse y referenciarse al test existente. Si es un componente nuevo adaptado, se incluyen los siguientes casos.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza 4 pasos con etiquetas correctas | Unit | 'Datos basicos', 'Comisiones', 'Tareas', 'Revisar' |
| 2 | paso activo con aria-current="step" | Unit | paso 1 activo identificado correctamente |
| 3 | pasos completados muestran checkmark | Unit | paso 1 completado -> icono Check en lugar de '1' |
| 4 | pasos futuros estan deshabilitados | Unit | paso 3 y 4 disabled cuando currentStep=1 |
| 5 | click en paso completado navega | Unit | paso 1 completado -> click -> onStepClick(1) |

#### Paso1DatosBasicos.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/Paso1DatosBasicos.test.tsx`

Recibe `onNext(data)` y `defaultValues` como props.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza todos los campos del paso 1 | Unit | titulo, descripcion, tipoPromo, campana, proyecto, urlLanding, codigoTracking, fechas |
| 2 | titulo es obligatorio | Integration | submit sin titulo -> error visible |
| 3 | titulo min 5 caracteres | Integration | 'Hola' -> error 'al menos 5 caracteres' |
| 4 | tipo de programa es obligatorio | Integration | submit sin seleccionar tipo -> error |
| 5 | fechaFin posterior a fechaInicio | Integration | fechaFin=fechaInicio -> error en fechaFin |
| 6 | campana y proyecto son opcionales (FA-01, AC-CP02-10) | Integration | submit sin vincular ninguno -> onNext llamado |
| 7 | codigoTracking acepta solo alfanumerico y guiones | Integration | 'album_2026' -> error de formato |
| 8 | urlLanding acepta string vacio | Integration | URL vacia -> sin error |
| 9 | urlLanding invalida muestra error | Integration | 'no-es-url' -> error de formato URL |
| 10 | contador de caracteres en descripcion | Unit | al escribir en textarea, contador actualiza |
| 11 | pre-carga defaultValues cuando existen | Unit | defaultValues.titulo -> input pre-rellenado |
| 12 | onNext llamado con datos correctos al submit valido | Integration | fill todos campos -> click Siguiente -> onNext(datos) |

#### Paso2Comisiones.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/Paso2Comisiones.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza campos de comision porcentaje y fija | Unit | ambos inputs visibles |
| 2 | **ambas comisiones vacias bloquea submit (AC-CP02-4)** | Integration | ningun campo -> error 'Debe definir al menos una comision' |
| 3 | **solo comision porcentaje definida permite submit** | Integration | importeComisionPorcentaje:10 -> onNext llamado |
| 4 | **solo comision fija definida permite submit** | Integration | importeComisionFija:50 -> onNext llamado |
| 5 | comision porcentaje no puede superar 100 | Integration | 101 -> error de rango |
| 6 | comision porcentaje no puede ser negativa | Integration | -1 -> error |
| 7 | comision fija no puede ser negativa | Integration | -1 -> error |
| 8 | selector de moneda es requerido | Integration | sin moneda -> error 'La moneda es obligatoria' |
| 9 | pre-carga defaultValues | Unit | defaultValues con comision -> inputs pre-rellenados |

#### Paso3Tareas.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/Paso3Tareas.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | lista vacia con mensaje "Sin tareas" cuando no hay tareas | Unit | array vacio -> empty state visible |
| 2 | boton "Agregar tarea" visible siempre | Unit | boton presente independiente del estado |
| 3 | click "Agregar tarea" muestra formulario inline PromoTareaFormItem | Integration | click -> form item visible |
| 4 | agregar tarea valida la agrega a la lista | Integration | fill datos validos + guardar -> tarea en lista |
| 5 | **EsRepetible=true sin MaxRepeticiones muestra error (FA-06, AC-CP02-9)** | Integration | toggle esRepetible sin maxRepeticiones -> error |
| 6 | EsRepetible=false no requiere MaxRepeticiones | Integration | esRepetible false -> maxRepeticiones no requerido |
| 7 | eliminar tarea la remueve de la lista | Integration | icono eliminar -> tarea desaparece |
| 8 | **avanzar sin tareas es valido (FA-02)** | Integration | 0 tareas -> click Siguiente -> onNext llamado |
| 9 | puede agregar multiples tareas | Integration | agregar 3 tareas -> lista muestra 3 |
| 10 | editar tarea existente actualiza datos | Integration | click editar tarea -> form pre-rellenado -> guardar -> lista actualizada |

#### PromoTareaFormItem.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/PromoTareaFormItem.test.tsx`

Formulario inline para agregar/editar una tarea. Componente controlado que recibe `onSave(tarea)` y `onCancel`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza campos obligatorios | Unit | titulo, tipoEvento, tipoReward, esRepetible |
| 2 | titulo de tarea min 3 caracteres | Integration | 'Ab' -> error |
| 3 | tipoEventoPromoId requerido | Integration | sin seleccionar -> error |
| 4 | tipoRewardId requerido | Integration | sin seleccionar -> error |
| 5 | **esRepetible=true muestra campo MaxRepeticiones** | Unit | toggle on -> maxRepeticiones field visible |
| 6 | **esRepetible=false oculta campo MaxRepeticiones** | Unit | toggle off -> maxRepeticiones field oculto |
| 7 | **esRepetible=true, MaxRepeticiones vacio -> error** | Integration | toggle on + submit sin maxRepeticiones -> error en maxRepeticiones |
| 8 | tipoReward Dinero muestra campo importeRecompensa | Unit | seleccionar tipoRewardId=1 -> campo importe visible |
| 9 | tipoReward Puntos muestra campo puntosRecompensa | Unit | seleccionar tipoRewardId=2 -> campo puntos visible |
| 10 | tipoReward Mixto muestra ambos campos | Unit | seleccionar tipoRewardId=3 -> ambos campos visibles |
| 11 | urlInstrucciones invalida muestra error | Integration | 'no-url' -> error de formato |
| 12 | onSave llamado con datos correctos | Integration | fill valido + guardar -> onSave(datos) |
| 13 | onCancel cierra el form inline | Unit | click cancelar -> onCancel() |
| 14 | pre-carga datos en modo edicion | Unit | defaultValues -> campos pre-rellenados |

#### Paso4Revision.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/Paso4Revision.test.tsx`

Componente de solo lectura que recibe `formData` y `onSubmit` como props.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra titulo del programa | Unit | formData.titulo visible |
| 2 | muestra tipo de programa | Unit | 'Referral' visible |
| 3 | muestra comisiones definidas | Unit | '10%' visible, comision fija oculta si null |
| 4 | muestra tareas en lista | Unit | cada tarea con titulo y tipo de evento |
| 5 | muestra "Sin tareas" cuando array vacio | Unit | FA-02: 0 tareas -> mensaje informativo |
| 6 | boton publicar presente | Unit | 'Publicar programa' visible |
| 7 | click publicar llama onSubmit | Unit | click boton -> onSubmit() |
| 8 | boton deshabilitado durante isPending | Unit | isPending:true -> boton disabled |
| 9 | muestra campana vinculada si existe | Unit | campaniaTitulo en resumen |
| 10 | "Sin campana vinculada" si no hay campana | Unit | campaniaCrowdfundingId:null -> texto alternativo |

#### AbandonarWizardDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/components/__tests__/AbandonarWizardDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza cuando open=true | Unit | titulo del dialog visible |
| 2 | no renderiza cuando open=false | Unit | dialog no en DOM |
| 3 | click confirmar llama onConfirm | Unit | boton 'Si, abandonar' -> onConfirm() |
| 4 | click cancelar llama onOpenChange(false) | Unit | boton 'Cancelar' -> onOpenChange(false) |
| 5 | mensaje advierte que se perderan los datos | Unit | texto de aviso presente |

---

### 4.3 Listado Mis Programas (P1)

#### MisProgramasPage.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/components/__tests__/MisProgramasPage.test.tsx`

Test de integracion de la pagina de listado. Mockea `usePromoProgramas`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra skeleton durante carga | Integration | isLoading:true -> skeleton visible |
| 2 | muestra lista de programas | Integration | data con 2 programas -> 2 cards visibles |
| 3 | **muestra empty state cuando no hay programas (AC-CP02-5)** | Integration | data con items:[] -> EmptyStateProgramas visible |
| 4 | empty state tiene boton "Crear programa" | Integration | click boton -> push('/programas/nuevo') |
| 5 | filtro "Activos" filtra la lista | Integration | click filtro activos -> usePromoProgramas llamado con esActivo:true |
| 6 | filtro "Inactivos" filtra la lista | Integration | click filtro inactivos -> usePromoProgramas llamado con esActivo:false |
| 7 | filtro "Todos" limpia el filtro | Integration | click todos -> usePromoProgramas llamado sin esActivo |
| 8 | boton "Crear programa" en header navega | Integration | click -> push('/programas/nuevo') |
| 9 | click en card navega al detalle | Integration | click card -> push('/programas/prog-001') |
| 10 | paginacion renderiza cuando totalCount > pageSize | Integration | totalCount:15 -> paginacion visible |
| 11 | muestra mensaje de error cuando falla la API | Integration | isError:true -> mensaje de error visible |

#### PromoProgramaCard.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/components/__tests__/PromoProgramaCard.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra titulo del programa | Unit | titulo visible |
| 2 | muestra tipo de programa (badge/texto) | Unit | 'Referral' visible |
| 3 | muestra badge de estado activo | Unit | esActivo:true -> badge verde 'Activo' |
| 4 | muestra badge de estado inactivo | Unit | esActivo:false -> badge gris 'Inactivo' |
| 5 | muestra numero de promotores | Unit | '5 promotores' visible |
| 6 | muestra numero de tareas | Unit | '3 tareas' visible |
| 7 | muestra campana vinculada | Unit | campaniaTitulo visible |
| 8 | muestra "--" o alternativo cuando sin campana | Unit | campaniaTitulo:null -> texto alternativo |
| 9 | muestra comision porcentaje cuando aplica | Unit | '10%' visible |
| 10 | muestra comision fija cuando aplica | Unit | '50 EUR' visible |
| 11 | menu de acciones tiene opcion Ver detalle | Unit | item menu visible |
| 12 | click Ver detalle llama onView | Unit | click -> onView(id) |

#### PromoProgramaStatusBadge.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/components/__tests__/PromoProgramaStatusBadge.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | badge activo con color correcto | Unit | esActivo:true -> texto 'Activo', variant success |
| 2 | badge inactivo con color correcto | Unit | esActivo:false -> texto 'Inactivo', variant muted |

#### PromoProgramaFilters.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/components/__tests__/PromoProgramaFilters.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza los 3 botones de filtro | Unit | 'Todos', 'Activos', 'Inactivos' |
| 2 | filtro activo resaltado | Unit | valor seleccionado -> estilo diferenciado |
| 3 | click en filtro llama onFilterChange | Unit | click 'Activos' -> onFilterChange('activo') |
| 4 | click en Todos limpia filtro | Unit | click 'Todos' -> onFilterChange(undefined) |

#### EmptyStateProgramas.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/components/__tests__/EmptyStateProgramas.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra mensaje cuando no hay programas | Unit | texto descriptivo visible |
| 2 | boton Crear programa visible | Unit | CTA principal presente |
| 3 | click boton llama onCreateNew | Unit | click -> onCreateNew() |

---

### 4.4 Detalle Programa (P1)

#### PromoProgramaDetailPage.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/PromoProgramaDetailPage.test.tsx`

Test de integracion de la pagina de detalle. Mockea `usePromoProgramaDetail`, `useDesactivarPromoPrograma` y `useParams`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra skeleton durante carga | Integration | isLoading:true -> skeleton visible |
| 2 | muestra titulo del programa | Integration | mockProgramaDetail.titulo visible |
| 3 | muestra secciones de KPIs | Integration | tarjetas de promotores, conversiones, valor visible |
| 4 | muestra seccion de tareas | Integration | lista de tareas del programa visible |
| 5 | muestra seccion de promotores inscritos | Integration | lista de promotores visible |
| 6 | boton Desactivar visible cuando esActivo=true | Integration | mockProgramaDetail.esActivo:true -> boton presente |
| 7 | boton Desactivar oculto cuando esActivo=false | Integration | mockProgramaDetailInactivo -> boton ausente |
| 8 | click Desactivar abre DesactivarProgramaDialog | Integration | click boton -> dialog visible |
| 9 | boton Editar navega a /programas/[id]/editar | Integration | click editar -> push('/programas/prog-001/editar') |
| 10 | muestra error 404 cuando programa no existe | Integration | isError:true (2019) -> mensaje de error |
| 11 | muestra badge de estado correcto | Integration | esActivo:false -> badge inactivo visible |

#### PromoProgramaKPICards.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/PromoProgramaKPICards.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra total promotores aprobados | Unit | '5' en la card de promotores |
| 2 | muestra total promotores pendientes | Unit | '2' pendientes visible |
| 3 | muestra total conversiones | Unit | '12' conversiones |
| 4 | muestra valor total generado formateado | Unit | '1.200,00 EUR' formateado |
| 5 | KPIs en cero renderiza sin errores | Unit | resumen con todos los valores a 0 |

#### PromoTareasSection.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/PromoTareasSection.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | lista tareas activas del programa | Unit | mockTareaShare y mockTareaBacking visibles |
| 2 | muestra tipo de evento de cada tarea | Unit | 'Share', 'Backing' visibles |
| 3 | muestra recompensa de cada tarea | Unit | '5 EUR' o 'Puntos' visible |
| 4 | muestra completados por promotores | Unit | '23 completados' visible |
| 5 | muestra empty state cuando no hay tareas | Unit | tareas:[] -> "Sin tareas definidas" |
| 6 | muestra badge activo/inactivo por tarea | Unit | esActivo:false -> badge inactivo en tarea |

#### PromotoresSection.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/PromotoresSection.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | lista promotores inscritos | Unit | 'DJ Marketing Pro' visible |
| 2 | muestra tipo de promotor | Unit | 'Influencer' visible |
| 3 | muestra badge aprobado/pendiente | Unit | esAprobado:true -> badge 'Aprobado' |
| 4 | empty state cuando no hay promotores | Unit | promotores:[] -> "Sin promotores inscritos" |

#### DesactivarProgramaDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/DesactivarProgramaDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza cuando open=true | Unit | titulo del dialog visible |
| 2 | no renderiza cuando open=false | Unit | dialog no en DOM |
| 3 | muestra nombre del programa en el mensaje | Unit | 'Promociona mi nuevo album' en el texto de confirmacion |
| 4 | click confirmar llama mutate | Integration | click 'Desactivar' -> mockMutate() |
| 5 | click cancelar cierra dialog | Unit | click 'Cancelar' -> onOpenChange(false) |
| 6 | boton disabled durante isPending | Unit | isPending:true -> boton desactivar disabled |
| 7 | muestra aviso de cascada en tareas | Unit | texto sobre tareas que se desactivaran |
| 8 | toast exito y cierre tras mutacion exitosa | Integration | onSuccess -> toast.success + onOpenChange(false) |

---

### 4.5 Hooks (P0)

#### use-promo-programas.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-promo-programas.test.ts`

Hook de query para el listado. Mockea `promoProgramaService.getMisProgramas`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna lista de programas en exito | Unit | mockResolvedValue(mockProgramaListResult) -> data correcta |
| 2 | isLoading true inicialmente | Unit | promise pendiente -> isLoading:true |
| 3 | isError true cuando falla la API | Unit | mockRejectedValue -> isError:true |
| 4 | no reintenta en error (retry:false) | Unit | getMe llamado 1 sola vez |
| 5 | llama al servicio sin filtros por defecto | Unit | getMisProgramas llamado sin esActivo |
| 6 | pasa filtro esActivo:true cuando se especifica | Unit | hook({ esActivo: true }) -> servicio recibe parametro |
| 7 | pasa filtro esActivo:false cuando se especifica | Unit | hook({ esActivo: false }) -> servicio recibe parametro |
| 8 | pasa parametros de paginacion | Unit | hook({ page:2, pageSize:5 }) -> servicio recibe parametros |

#### use-promo-programa-detail.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-promo-programa-detail.test.ts`

Hook de query para el detalle. Mockea `promoProgramaService.getById`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna detalle en exito | Unit | mockResolvedValue(mockProgramaDetail) -> data correcta |
| 2 | isLoading true inicialmente | Unit | promise pendiente -> isLoading:true |
| 3 | isError true en 404 | Unit | error 2019 -> isError:true |
| 4 | llama al servicio con el id correcto | Unit | getById('prog-001') llamado |
| 5 | no reintenta en error | Unit | llamado 1 sola vez |

#### use-promo-programa-mutations.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-promo-programa-mutations.test.ts`

Tests para `useCreatePromoPrograma`, `useUpdatePromoPrograma` y `useDesactivarPromoPrograma`.

**useCreatePromoPrograma:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al servicio con datos correctos | Integration | mutate(mockWizardCompleteData) -> create llamado con payload |
| 2 | **transforma strings vacios de URLs a undefined** | Integration | urlLanding:'' -> servicio recibe urlLanding:undefined |
| 3 | **transforma campaniaCrowdfundingId vacio a undefined** | Integration | '' -> undefined en payload al servicio |
| 4 | invalida query 'mis' tras exito | Integration | onSuccess -> invalidateQueries con key correcto |
| 5 | toast exito tras creacion | Integration | onSuccess -> toast.success() |
| 6 | isError true cuando servicio lanza error | Integration | mockRejectedValue -> isError:true |
| 7 | **error 1024 (tracking duplicado) muestra toast con mensaje especifico** | Integration | error con code 1024 -> toast.error con 'codigo de tracking ya esta en uso' |

**useUpdatePromoPrograma:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 8 | llama al servicio con id y datos | Integration | mutate({ id, data }) -> update(id, data) llamado |
| 9 | invalida query byId y 'mis' tras exito | Integration | onSuccess -> ambas queries invalidadas |
| 10 | toast exito tras actualizacion | Integration | onSuccess -> toast.success() |
| 11 | **error 1029 (tarea con completados) muestra mensaje especifico** | Integration | code 1029 -> toast.error descriptivo |

**useDesactivarPromoPrograma:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 12 | llama al servicio con el id | Integration | mutate('prog-001') -> desactivar('prog-001') llamado |
| 13 | invalida queries tras exito | Integration | onSuccess -> queries invalidadas |
| 14 | toast exito tras desactivacion | Integration | onSuccess -> toast.success('Programa desactivado') |
| 15 | **error 4020 (ya desactivado) muestra mensaje especifico** | Integration | code 4020 -> toast.error('ya esta desactivado') |
| 16 | isError true en error de red | Integration | mockRejectedValue -> isError:true |

#### use-wizard-promo-state.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-wizard-promo-state.test.ts`

Hook de estado del wizard con useReducer. No usa QueryClient (no necesita wrapper).

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | inicializa en Paso 1 con formData vacio | Unit | currentStep:1, formData:{}, completedSteps:[] |
| 2 | updateFormData fusiona datos y marca paso completado | Unit | updateFormData({titulo:'Test'}) -> formData.titulo y completedSteps:[1] |
| 3 | goToStep avanza a paso alcanzable | Unit | paso 1 completado -> goToStep(2) -> currentStep:2 |
| 4 | goToStep rechaza paso no alcanzable | Unit | goToStep(3) sin paso 2 completado -> currentStep sin cambio |
| 5 | prevStep retrocede correctamente | Unit | currentStep:3 -> prevStep() -> currentStep:2 |
| 6 | prevStep no retrocede mas alla del paso 1 | Unit | currentStep:1 -> prevStep() -> currentStep:1 |
| 7 | resetWizard limpia todo el estado | Unit | despues de resetWizard -> estado inicial |
| 8 | updateFormData acumula datos entre llamadas | Unit | dos llamadas -> datos fusionados |
| 9 | paso 4 alcanzable solo con pasos 1-3 completados | Unit | completedSteps:[1,2,3] -> goToStep(4) pasa |

---

### 4.6 Service (P1)

#### promo-programa.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/promo-programa.service.test.ts`

Mockea `apiFetch`. Patron identico a `promotor.service.test.ts`.

**promoProgramaService.getMisProgramas:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama a API_ROUTES.crowdpromotion.programas.mis | Unit | URL correcta |
| 2 | pasa esActivo como query param | Unit | ?esActivo=true en la URL |
| 3 | pasa page y pageSize | Unit | ?page=2&pageSize=5 |
| 4 | retorna PromoProgramaListResult en exito | Unit | data del mock correctamente mapeada |
| 5 | lanza error cuando response tiene errorCodes | Unit | messages con errorCode no-0xxx -> throw |

**promoProgramaService.getById:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 6 | llama a API_ROUTES.crowdpromotion.programas.byId(id) | Unit | URL con id correcto |
| 7 | retorna PromoProgramaDetail en exito | Unit | data mapeada |
| 8 | lanza error en 404 (2019) | Unit | errorCode 2019 -> throw |

**promoProgramaService.create:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 9 | llama POST a API_ROUTES.crowdpromotion.programas.base | Unit | method: POST |
| 10 | envia el body correcto | Unit | data coincide con el request |
| 11 | retorna PromoProgramaCreatedResult en 201 | Unit | data mapeada |
| 12 | lanza error en 400 con errorCode 1020 | Unit | sin comision -> throw con code |

**promoProgramaService.update:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 13 | llama PUT a byId(id) | Unit | method: PUT, URL correcta |
| 14 | retorna PromoProgramaUpdatedResult en 200 | Unit | data mapeada |
| 15 | lanza error en 403 | Unit | errorCode 3002 -> throw |

**promoProgramaService.desactivar:**

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 16 | llama PATCH a API_ROUTES.crowdpromotion.programas.desactivar(id) | Unit | method: PATCH, sin body |
| 17 | retorna PromoProgramaDesactivadoResult en 200 | Unit | esActivo:false, tareasDesactivadas:2 |
| 18 | lanza error en 400 con errorCode 4020 (ya inactivo) | Unit | throw con mensaje especifico |
| 19 | propaga error de red | Unit | mockRejectedValue -> throw error original |
| 20 | retorna tareasDesactivadas:0 cuando no habia tareas activas | Unit | tareasDesactivadas:0 en resultado |

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Prioridad |
|---------|--------|-----------|----------|-----------|
| `crowdpromotion.schema.ts` (schemas nuevos) | 95% | 100% | 95% | P0 |
| `use-wizard-promo-state.ts` | 90% | 100% | 85% | P0 |
| `use-promo-programa-mutations.ts` | 85% | 100% | 80% | P0 |
| `use-promo-programas.ts` | 90% | 100% | 85% | P0 |
| `use-promo-programa-detail.ts` | 90% | 100% | 85% | P0 |
| `promo-programa.service.ts` | 90% | 100% | 90% | P1 |
| `WizardContainer.tsx` | 85% | 90% | 80% | P0 |
| `Paso1DatosBasicos.tsx` | 80% | 90% | 75% | P0 |
| `Paso2Comisiones.tsx` | 85% | 90% | 85% | P0 |
| `Paso3Tareas.tsx` | 80% | 85% | 75% | P0 |
| `Paso4Revision.tsx` | 85% | 90% | 80% | P1 |
| `PromoTareaFormItem.tsx` | 80% | 85% | 75% | P0 |
| `AbandonarWizardDialog.tsx` | 85% | 100% | 85% | P1 |
| `MisProgramasPage.tsx` | 80% | 85% | 75% | P1 |
| `PromoProgramaCard.tsx` | 80% | 90% | 75% | P1 |
| `PromoProgramaStatusBadge.tsx` | 100% | 100% | 100% | P1 |
| `PromoProgramaFilters.tsx` | 85% | 100% | 80% | P1 |
| `EmptyStateProgramas.tsx` | 90% | 100% | 85% | P1 |
| `PromoProgramaDetailPage.tsx` | 80% | 85% | 75% | P1 |
| `PromoProgramaKPICards.tsx` | 80% | 90% | 75% | P1 |
| `PromoTareasSection.tsx` | 80% | 90% | 75% | P2 |
| `PromotoresSection.tsx` | 80% | 90% | 75% | P2 |
| `DesactivarProgramaDialog.tsx` | 85% | 100% | 80% | P0 |
| `WizardStepper.tsx` (promo) | 85% | 100% | 80% | P1 |

**Meta Global:** 80% en todas las metricas

---

## 6. Orden de Implementacion

Las suites deben implementarse en este orden para minimizar dependencias bloqueantes:

### Fase 1 - Fundamentos (bloquean todo lo demas)
1. `__mocks__/promo-programa.mock.ts` - Fixtures
2. `createPromoTareaSchema.test.ts` - Logica critica, sin dependencias
3. `createPromoProgramaSchema.test.ts` - Logica critica, sin dependencias

### Fase 2 - Service y Hooks (P0)
4. `promo-programa.service.test.ts` - Base de los hooks
5. `use-promo-programas.test.ts`
6. `use-promo-programa-detail.test.ts`
7. `use-wizard-promo-state.test.ts`
8. `use-promo-programa-mutations.test.ts`

### Fase 3 - Componentes Criticos del Wizard (P0)
9. `PromoTareaFormItem.test.tsx` - Componente hoja, sin dependencias de otros componentes
10. `Paso1DatosBasicos.test.tsx`
11. `Paso2Comisiones.test.tsx`
12. `Paso3Tareas.test.tsx`
13. `Paso4Revision.test.tsx`
14. `AbandonarWizardDialog.test.tsx`
15. `WizardContainer.test.tsx` - Requiere pasos 1-4 como referencia

### Fase 4 - Paginas y Componentes (P1)
16. `PromoProgramaStatusBadge.test.tsx`
17. `EmptyStateProgramas.test.tsx`
18. `PromoProgramaFilters.test.tsx`
19. `PromoProgramaCard.test.tsx`
20. `MisProgramasPage.test.tsx`

### Fase 5 - Detalle (P1-P2)
21. `DesactivarProgramaDialog.test.tsx`
22. `PromoProgramaKPICards.test.tsx`
23. `PromoTareasSection.test.tsx`
24. `PromotoresSection.test.tsx`
25. `PromoProgramaDetailPage.test.tsx`
26. `WizardStepper.test.tsx` (solo si es componente nuevo)

---

## 7. Notas de Implementacion

### 7.1 Next.js 14 App Router - Mock de navegacion

En todos los tests de componentes que navegan, mockear `next/navigation` completo:

```typescript
const mockPush = vi.fn()
const mockBack = vi.fn()
vi.mock('next/navigation', () => ({
    useRouter: () => ({ push: mockPush, back: mockBack }),
    useParams: () => ({ id: 'prog-001' }),
    usePathname: () => '/dashboard/crowdpromotion/programas',
}))
```

### 7.2 Transformacion de strings vacios antes del POST

El hook `useCreatePromoPrograma` debe transformar strings vacios (`''`) de campos opcionales de URL y UUID a `undefined` antes de enviar al servicio. Los tests en `use-promo-programa-mutations.test.ts` (casos #2 y #3) son los que verifican esta logica critica. Sin esta transformacion, la API rechazaria el request con errores de formato URL.

### 7.3 Refines de Zod y paths de error

Los tests de schemas verifican explicitamente que los errores de los refines caigan en el `path` correcto (`['maxRepeticiones']`, `['importeComisionPorcentaje']`, `['fechaFin']`). Esto garantiza que React Hook Form posicione el error en el campo correcto del formulario.

### 7.4 Estado del wizard con useReducer vs localStorage

A diferencia del wizard de campanias, el wizard de PromoPrograma NO persiste en localStorage (la UI/UX especifica que el estado se pierde al navegar fuera). Por tanto, el test `use-wizard-promo-state.test.ts` NO incluye casos de localStorage (diferencia con `use-wizard-state.test.ts` existente).

### 7.5 Mockear el hook de mutacion, no el servicio, en tests de componentes

Los tests de componentes (formularios, dialogs) mockean el hook completo (`useCreatePromoPrograma`, `useDesactivarPromoPrograma`), no el servicio directamente. Esto sigue el patron establecido en el proyecto (ver `PromotorProfilePage.test.tsx` y `DesactivarPromotorDialog.test.tsx`).

### 7.6 Sonner (toast) siempre mockeado en tests de componentes y hooks

```typescript
vi.mock('sonner', () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))
```

Este mock se incluye en cualquier test que renderize componentes con feedback de acciones o que ejecute mutaciones con side-effects de toast.

### 7.7 WizardStepper reutilizado vs nuevo

Si el componente `WizardStepper` de PromoPrograma reutiliza el mismo componente del wizard de campanias (`campanias/components/wizard/WizardStepper.tsx`), las etiquetas de los pasos seran pasadas como prop y el test en `WizardStepper.test.tsx` (campania) ya cubre la logica base. En ese caso, la suite `WizardStepper.test.tsx` (promo) se limita a verificar que las etiquetas especificas del promo se renderizan correctamente ('Datos basicos', 'Comisiones', 'Tareas', 'Revisar').

### 7.8 Test del aviso de promotores inscritos en edicion

El criterio AC-CP02-6 especifica mostrar un aviso cuando el programa tiene promotores inscritos al editar. Este caso se cubre en `PromoProgramaDetailPage.test.tsx` o en un test especifico del formulario de edicion. El aviso se muestra cuando `mockProgramaDetail.promotores.length > 0` y es informativo (no bloquea). No requiere una suite separada dado que la pagina de edicion reutiliza los mismos componentes de paso del wizard.

---

## 8. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del admin
cd src/admin && npm run test

# Ejecutar en modo watch
cd src/admin && npm run test:run -- --watch

# Ejecutar solo tests de esta feature
cd src/admin && npm run test:run -- --reporter=verbose src/app/\(dashboard\)/crowdpromotion

# Ejecutar solo schemas
cd src/admin && npm run test:run -- src/app/\(dashboard\)/crowdpromotion/programas/__tests__/schemas

# Ejecutar solo hooks
cd src/admin && npm run test:run -- src/hooks/__tests__/use-promo

# Ejecutar con cobertura
cd src/admin && npm run test:run -- --coverage
```

---

## 9. Checklist

- [ ] `__mocks__/promo-programa.mock.ts` creado con todos los fixtures
- [ ] `createPromoTareaSchema.test.ts` - 21 casos cubriendo todos los refines
- [ ] `createPromoProgramaSchema.test.ts` - 29 casos cubriendo todos los refines
- [ ] `promo-programa.service.test.ts` - 20 casos, todos los metodos
- [ ] `use-promo-programas.test.ts` - 8 casos incluyendo filtros y paginacion
- [ ] `use-promo-programa-detail.test.ts` - 5 casos
- [ ] `use-promo-programa-mutations.test.ts` - 16 casos, los 3 hooks con errores especificos
- [ ] `use-wizard-promo-state.test.ts` - 9 casos, sin localStorage
- [ ] `WizardContainer.test.tsx` - 18 casos incluyendo FA-08 (error mantiene estado)
- [ ] `Paso1DatosBasicos.test.tsx` - 12 casos con FA-01 y AC-CP02-10
- [ ] `Paso2Comisiones.test.tsx` - 9 casos con AC-CP02-4
- [ ] `Paso3Tareas.test.tsx` - 10 casos con FA-02 y FA-06
- [ ] `PromoTareaFormItem.test.tsx` - 14 casos con logica de recompensas
- [ ] `Paso4Revision.test.tsx` - 10 casos
- [ ] `AbandonarWizardDialog.test.tsx` - 5 casos
- [ ] `MisProgramasPage.test.tsx` - 11 casos con filtros y paginacion
- [ ] `PromoProgramaCard.test.tsx` - 12 casos
- [ ] `PromoProgramaStatusBadge.test.tsx` - 2 casos
- [ ] `PromoProgramaFilters.test.tsx` - 4 casos
- [ ] `EmptyStateProgramas.test.tsx` - 3 casos
- [ ] `PromoProgramaDetailPage.test.tsx` - 11 casos
- [ ] `PromoProgramaKPICards.test.tsx` - 5 casos
- [ ] `PromoTareasSection.test.tsx` - 6 casos
- [ ] `PromotoresSection.test.tsx` - 4 casos
- [ ] `DesactivarProgramaDialog.test.tsx` - 8 casos
- [ ] Cobertura global >= 80% verificada con `--coverage`
- [ ] Todos los tests pasan en < 60 segundos
