# Estrategia de Testing: cs-acuerdos-entregables (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Target:** `src/web` (Vite + React 18, puerto 3000)
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 42 | ~45% |
| Integration Tests | 28 | ~35% |
| Total | 70 | 80%+ |

La feature tiene alta densidad de logica condicional por `miRol` y por `estadoAcuerdoId`, lo que hace que los tests de integracion (componentes con estado real de datos) sean el tier mas critico. Los tests unitarios cubren badges, formularios aislados y calculos de importe en tiempo real.

---

## 2. Estructura de Tests

```
src/web/src/features/crowdsourcing/
├── __tests__/
│   ├── components/
│   │   ├── EstadoAcuerdoBadge.test.tsx
│   │   ├── EstadoEntregableBadge.test.tsx
│   │   ├── AcuerdoCabecera.test.tsx
│   │   ├── ImporteAsignadoBar.test.tsx
│   │   ├── MilestoneCard.test.tsx
│   │   ├── EntregableItem.test.tsx
│   │   ├── AcuerdoTimeline.test.tsx
│   │   ├── MilestoneFormDialog.test.tsx
│   │   ├── SubirEntregableDialog.test.tsx
│   │   ├── AprobarEntregableDialog.test.tsx
│   │   ├── RechazarEntregableDialog.test.tsx
│   │   ├── CompletarAcuerdoDialog.test.tsx
│   │   ├── CancelarAcuerdoDialog.test.tsx
│   │   ├── MilestonesSection.test.tsx
│   │   └── AcuerdoDetallePage.test.tsx
│   ├── hooks/
│   │   ├── useAcuerdo.test.ts
│   │   ├── useAceptarPropuesta.test.ts
│   │   ├── useRechazarPropuesta.test.ts
│   │   ├── useCreateMilestone.test.ts
│   │   ├── useUpdateMilestone.test.ts
│   │   ├── useDeleteMilestone.test.ts
│   │   ├── useCreateEntregable.test.ts
│   │   ├── useAprobarEntregable.test.ts
│   │   ├── useRechazarEntregable.test.ts
│   │   ├── useCompletarAcuerdo.test.ts
│   │   └── useCancelarAcuerdo.test.ts
│   └── services/
│       ├── acuerdo.service.test.ts
│       ├── milestone.service.test.ts
│       └── entregable.service.test.ts
└── __mocks__/
    └── acuerdo.mock.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/web/src/features/crowdsourcing/__mocks__/acuerdo.mock.ts`

Este archivo extiende los mocks existentes del modulo (`propuesta.mock.ts`, `necesidad.mock.ts`). Se deben cubrir todos los estados posibles del acuerdo y de los entregables para testar el renderizado condicional.

```typescript
// Entregables en todos sus estados
export const mockEntregableEntregado: Entregable = {
    id: 'entr-001-entregado',
    titulo: 'Mezcla cancion 1 - v1',
    descripcion: 'Primera version de la mezcla',
    urlRecurso: 'https://drive.google.com/file/xyz',
    estadoEntregableId: 1,
    estadoEntregableNombre: 'Entregado',
    comentarioAprobacion: undefined,
    comentarioRechazo: undefined,
    fechaAprobacion: undefined,
    fechaCreacion: '2026-03-05T14:00:00Z',
}

export const mockEntregableAprobado: Entregable = {
    ...mockEntregableEntregado,
    id: 'entr-002-aprobado',
    titulo: 'Mezcla cancion 1 - v2',
    estadoEntregableId: 2,
    estadoEntregableNombre: 'Aprobado',
    comentarioAprobacion: 'Excelente resultado',
    fechaAprobacion: '2026-03-06T10:00:00Z',
}

export const mockEntregableRechazado: Entregable = {
    ...mockEntregableEntregado,
    id: 'entr-003-rechazado',
    titulo: 'Mezcla cancion 2 - v1',
    estadoEntregableId: 3,
    estadoEntregableNombre: 'Rechazado',
    comentarioRechazo: 'La voz esta demasiado baja en el coro, necesita mas presencia.',
}

// Milestones
export const mockMilestoneConEntregables: Milestone = {
    id: 'mile-001',
    titulo: 'Mezcla de pistas 1-3',
    descripcion: 'Mezcla de las primeras 3 canciones del EP',
    orden: 1,
    importeParcial: 270.00,
    porcentajeParcial: 60.00,
    fechaLimite: '2026-03-08T00:00:00Z',
    fechaCompletado: undefined,
    entregables: [mockEntregableEntregado],
}

export const mockMilestoneCompletado: Milestone = {
    ...mockMilestoneConEntregables,
    id: 'mile-002',
    titulo: 'Mezcla de pistas 4-5',
    fechaCompletado: '2026-03-07T18:00:00Z',
    entregables: [mockEntregableAprobado],
}

export const mockMilestoneSinEntregables: Milestone = {
    id: 'mile-003',
    titulo: 'Masterizacion final',
    descripcion: undefined,
    orden: 2,
    importeParcial: 180.00,
    porcentajeParcial: 40.00,
    fechaLimite: undefined,
    fechaCompletado: undefined,
    entregables: [],
}

// Timeline
export const mockTimeline: AcuerdoTimelineEvento[] = [
    {
        accion: 'Acuerdo creado',
        fecha: '2026-03-01T10:00:00Z',
        actor: 'Los Rockeros',
    },
    {
        accion: 'Milestone agregado: Mezcla de pistas 1-3',
        fecha: '2026-03-02T09:00:00Z',
        actor: 'Los Rockeros',
    },
    {
        accion: 'Entregable subido: Mezcla cancion 1 - v1',
        fecha: '2026-03-05T14:00:00Z',
        actor: 'Studio Mix Pro',
    },
]

// Acuerdo base (miRol: Artista, estado: Activo)
export const mockAcuerdoActivo: Acuerdo = {
    id: 'e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b',
    tituloInterno: 'Mezcla EP Los Rockeros',
    estadoAcuerdoId: 1,
    estadoAcuerdoNombre: 'Activo',
    importeTotalPactado: 450.00,
    monedaNombre: 'EUR',
    fechaInicio: '2026-03-01T00:00:00Z',
    fechaFinPrevista: '2026-03-15T00:00:00Z',
    fechaFinReal: undefined,
    artista: { id: 'artista-001', nombreArtistico: 'Los Rockeros' },
    profesional: {
        userId: 'profesional-user-001',
        perfilProfesionalId: 'perfil-001',
        nombre: 'Studio Mix Pro',
    },
    necesidad: { id: 'necesidad-001', titulo: 'Mezcla de pistas para EP' },
    conversacionId: 'conv-001',
    milestones: [mockMilestoneConEntregables],
    importeAsignado: 270.00,
    porcentajeAsignado: 60.00,
    miRol: 'Artista',
    timeline: mockTimeline,
}

// Variantes de rol y estado para tests de renderizado condicional
export const mockAcuerdoActivoProfesional: Acuerdo = {
    ...mockAcuerdoActivo,
    miRol: 'Profesional',
}

export const mockAcuerdoCompletado: Acuerdo = {
    ...mockAcuerdoActivo,
    estadoAcuerdoId: 2,
    estadoAcuerdoNombre: 'Completado',
    fechaFinReal: '2026-03-14T16:00:00Z',
}

export const mockAcuerdoCancelado: Acuerdo = {
    ...mockAcuerdoActivo,
    estadoAcuerdoId: 3,
    estadoAcuerdoNombre: 'Cancelado',
    fechaFinReal: '2026-03-10T12:00:00Z',
}

// Acuerdo sin milestones (FA-05: milestones opcionales)
export const mockAcuerdoSinMilestones: Acuerdo = {
    ...mockAcuerdoActivo,
    milestones: [],
    importeAsignado: 0,
    porcentajeAsignado: 0,
}

// Acuerdo con entregables pendientes (para test de aviso en CompletarAcuerdoDialog)
export const mockAcuerdoConEntregablesPendientes: Acuerdo = {
    ...mockAcuerdoActivo,
    milestones: [
        {
            ...mockMilestoneConEntregables,
            entregables: [mockEntregableEntregado, mockEntregableRechazado],
        },
    ],
}

// Results de mutaciones
export const mockAceptarPropuestaResult: AceptarPropuestaResult = {
    acuerdoId: 'e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b',
    tituloInterno: 'Mezcla EP Los Rockeros',
    estadoAcuerdoNombre: 'Activo',
    importeTotalPactado: 450.00,
    monedaNombre: 'EUR',
    conversacionId: 'conv-001',
    propuestasRechazadas: 2,
}

export const mockMilestoneCreatedResult: MilestoneCreatedResult = {
    id: 'mile-004-new',
    titulo: 'Nuevo Milestone',
    orden: 2,
    importeParcial: 180.00,
    porcentajeParcial: 40.00,
    importeAsignadoTotal: 450.00,
}

export const mockEntregableCreatedResult: EntregableCreatedResult = {
    id: 'entr-004-new',
    titulo: 'Nuevo Entregable',
    estadoEntregableNombre: 'Entregado',
    fechaCreacion: '2026-03-06T10:00:00Z',
}

export const mockAprobarEntregableResult: AprobarEntregableResult = {
    id: mockEntregableEntregado.id,
    estadoEntregableNombre: 'Aprobado',
    fechaAprobacion: '2026-03-06T10:00:00Z',
    todosAprobadosEnMilestone: true,
}

export const mockAprobarEntregableResultParcial: AprobarEntregableResult = {
    ...mockAprobarEntregableResult,
    todosAprobadosEnMilestone: false,
}
```

### 3.2 Mocking Strategy

El proyecto sigue el patron de mockear el modulo de infrastructure/api directamente con `vi.mock()`, sin usar MSW. Este patron ya esta establecido en los tests existentes de hooks (`useTemplates.test.ts`).

**Para componentes** que reciben datos por props: no se necesita mock de API, solo props con mock data.

**Para hooks** que llaman a services: usar `vi.mock('../../infrastructure')` o `vi.mock('../../infrastructure/acuerdo.service')` segun la organizacion de archivos.

**Para tests de integracion de paginas**: usar `vi.mock` en el hook de nivel superior (`useAcuerdo`) para controlar el estado de carga/error/datos desde fuera.

**Para el router**: usar `BrowserRouter` del mismo modo que en `PropuestaCard.test.tsx`. Para tests que necesiten verificar navegacion, usar `MemoryRouter` con `initialEntries`.

**Para el auth context**: si `AcuerdoDetallePage` consume un contexto de autenticacion, crear un `AuthProvider` de test que devuelva el usuario mockeado. Si el `miRol` viene directamente del response del backend (que es el caso segun contracts.md), no hace falta mockear auth en la mayoria de tests de componentes.

```typescript
// Wrapper estandar para hooks con QueryClient
function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

// Wrapper para componentes que necesitan router
function renderWithRouter(ui: React.ReactElement) {
    return render(<BrowserRouter>{ui}</BrowserRouter>)
}

// Wrapper completo para tests de pagina
function renderWithProviders(ui: React.ReactElement) {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, gcTime: 0 } },
    })
    return render(
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>{ui}</BrowserRouter>
        </QueryClientProvider>
    )
}
```

---

## 4. Tests por Modulo

### 4.1 Components - Tests Unitarios

#### EstadoAcuerdoBadge.test.tsx

**Archivo:** `__tests__/components/EstadoAcuerdoBadge.test.tsx`

Componente puramente visual que recibe `estadoAcuerdoId: number` y renderiza el badge de color correcto.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders badge Activo con texto correcto | Unit | estadoAcuerdoId=1 muestra "Activo" |
| 2 | renders badge Completado con texto correcto | Unit | estadoAcuerdoId=2 muestra "Completado" |
| 3 | renders badge Cancelado con texto correcto | Unit | estadoAcuerdoId=3 muestra "Cancelado" |
| 4 | aplica clase/color blue para Activo | Unit | Verifica clase CSS o atributo data correspondiente al color azul |
| 5 | aplica clase/color green para Completado | Unit | Verifica clase CSS o atributo data correspondiente al color verde |
| 6 | aplica clase/color red para Cancelado | Unit | Verifica clase CSS o atributo data correspondiente al color rojo |

---

#### EstadoEntregableBadge.test.tsx

**Archivo:** `__tests__/components/EstadoEntregableBadge.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders badge Entregado con texto correcto | Unit | estadoEntregableId=1 muestra "Entregado" |
| 2 | renders badge Aprobado con texto correcto | Unit | estadoEntregableId=2 muestra "Aprobado" |
| 3 | renders badge Rechazado con texto correcto | Unit | estadoEntregableId=3 muestra "Rechazado" |
| 4 | aplica color yellow para Entregado | Unit | Pendiente de revision: amarillo |
| 5 | aplica color green para Aprobado | Unit | Aprobado: verde |
| 6 | aplica color red para Rechazado | Unit | Rechazado: rojo |

---

#### AcuerdoCabecera.test.tsx

**Archivo:** `__tests__/components/AcuerdoCabecera.test.tsx`

Componente que muestra la cabecera del acuerdo: titulo, badge de estado, partes involucradas, importe total, moneda, fechas.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders titulo interno del acuerdo | Unit | Muestra `tituloInterno` del mock |
| 2 | renders nombre del artista | Unit | Muestra `artista.nombreArtistico` |
| 3 | renders nombre del profesional | Unit | Muestra `profesional.nombre` |
| 4 | renders importe total y moneda | Unit | Muestra "450,00 EUR" o formato equivalente |
| 5 | renders fecha de inicio | Unit | Muestra `fechaInicio` formateada |
| 6 | renders fecha fin prevista cuando existe | Unit | Muestra `fechaFinPrevista` formateada |
| 7 | no renders fecha fin real cuando es null | Unit | `fechaFinReal` ausente -> no aparece en DOM |
| 8 | renders fecha fin real cuando acuerdo completado | Unit | Muestra `fechaFinReal` con mockAcuerdoCompletado |
| 9 | renders badge de estado | Unit | Delega en EstadoAcuerdoBadge, verifica texto de estado presente |
| 10 | renders link a conversacion cuando existe conversacionId | Unit | Muestra enlace o boton de chat |
| 11 | no renders link a conversacion cuando falta conversacionId | Unit | Sin conversacionId: no aparece boton de chat |

---

#### ImporteAsignadoBar.test.tsx

**Archivo:** `__tests__/components/ImporteAsignadoBar.test.tsx`

Componente de barra de progreso con texto "Asignado: X de Y EUR (Z%)". Logica de calculo importante.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders texto con importe asignado y total | Unit | "Asignado: 270,00 de 450,00 EUR (60%)" |
| 2 | renders barra de progreso con porcentaje correcto | Unit | width o aria-valuenow = 60 |
| 3 | renders correctamente cuando importe asignado es 0 | Unit | "Asignado: 0,00 de 450,00 EUR (0%)" |
| 4 | renders correctamente cuando importe asignado es igual al total | Unit | "Asignado: 450,00 de 450,00 EUR (100%)" |
| 5 | acepta importeAsignadoActual prop para mostrar valor en tiempo real | Unit | Usado en formulario de milestone para preview sin llamada API |

---

#### MilestoneCard.test.tsx

**Archivo:** `__tests__/components/MilestoneCard.test.tsx`

Card que muestra un milestone con su lista de entregables anidados y botones de accion condicionales.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders titulo del milestone | Unit | Muestra `titulo` |
| 2 | renders importe parcial | Unit | Muestra `importeParcial` formateado |
| 3 | renders porcentaje parcial | Unit | Muestra "60%" o similar |
| 4 | renders fecha limite cuando existe | Unit | Muestra `fechaLimite` formateada |
| 5 | no renders fecha limite cuando es null | Unit | Sin `fechaLimite`: no aparece en DOM |
| 6 | renders badge de pendiente cuando fechaCompletado es null | Unit | Estado visual "Pendiente" |
| 7 | renders badge de completado cuando fechaCompletado existe | Unit | Estado visual "Completado" con mockMilestoneCompletado |
| 8 | renders lista de entregables del milestone | Unit | Tantos items como `entregables.length` |
| 9 | renders mensaje vacio cuando no hay entregables | Unit | Texto informativo cuando `entregables = []` |
| 10 | muestra boton editar para artista en acuerdo activo y milestone no completado | Unit | miRol=Artista + estadoAcuerdoId=1 + fechaCompletado=null -> boton editar visible |
| 11 | no muestra boton editar para milestone ya completado | Unit | fechaCompletado != null -> boton no visible (FA-09) |
| 12 | no muestra boton editar para profesional | Unit | miRol=Profesional -> boton no visible |
| 13 | no muestra boton editar en acuerdo completado o cancelado | Unit | estadoAcuerdoId=2 o 3 -> botones no visibles |
| 14 | muestra boton eliminar para artista en acuerdo activo y milestone sin entregables | Unit | Boton visible con mockMilestoneSinEntregables |
| 15 | no muestra boton eliminar cuando milestone tiene entregables | Unit | Boton no visible (FA-03) |
| 16 | llama onEdit al hacer click en editar | Unit | Callback con milestone como argumento |
| 17 | llama onDelete al hacer click en eliminar | Unit | Callback con milestone.id como argumento |

---

#### EntregableItem.test.tsx

**Archivo:** `__tests__/components/EntregableItem.test.tsx`

Item de entregable con acciones condicionales segun `miRol` y estado del entregable.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders titulo del entregable | Unit | Muestra `titulo` |
| 2 | renders badge de estado | Unit | Delega en EstadoEntregableBadge |
| 3 | renders enlace URL cuando existe urlRecurso | Unit | Link clicable a la URL externa |
| 4 | no renders enlace URL cuando urlRecurso es null | Unit | Sin URL: no aparece link |
| 5 | renders fecha de creacion | Unit | Muestra `fechaCreacion` formateada |
| 6 | renders comentario de aprobacion cuando existe | Unit | Muestra `comentarioAprobacion` con mockEntregableAprobado |
| 7 | renders comentario de rechazo cuando existe | Unit | Muestra `comentarioRechazo` con mockEntregableRechazado |
| 8 | muestra botones aprobar/rechazar para artista con entregable en estado Entregado | Unit | miRol=Artista + estadoEntregableId=1 -> ambos botones visibles |
| 9 | no muestra botones aprobar/rechazar para entregable ya aprobado | Unit | estadoEntregableId=2 -> sin botones (FA-08) |
| 10 | no muestra botones aprobar/rechazar para entregable ya rechazado | Unit | estadoEntregableId=3 -> sin botones |
| 11 | no muestra botones aprobar/rechazar para profesional | Unit | miRol=Profesional -> sin botones |
| 12 | no muestra botones en acuerdo completado o cancelado | Unit | estadoAcuerdoId != 1 -> sin botones de revision |
| 13 | llama onAprobar al hacer click en aprobar | Unit | Callback con entregable.id |
| 14 | llama onRechazar al hacer click en rechazar | Unit | Callback con entregable.id |

---

#### AcuerdoTimeline.test.tsx

**Archivo:** `__tests__/components/AcuerdoTimeline.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders lista de eventos del timeline | Unit | Tantos items como `timeline.length` |
| 2 | renders accion y actor de cada evento | Unit | Muestra "Acuerdo creado" y "Los Rockeros" |
| 3 | renders fecha formateada de cada evento | Unit | Fecha legible en cada item |
| 4 | renders mensaje vacio cuando timeline es vacio | Unit | Texto informativo con array vacio |

---

#### MilestoneFormDialog.test.tsx

**Archivo:** `__tests__/components/MilestoneFormDialog.test.tsx`

Dialogo modal con formulario para crear y editar milestones. Incluye validaciones Zod y el indicador de importe en tiempo real.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | no renderiza cuando isOpen es false | Unit | Dialog cerrado: campos no en DOM |
| 2 | renderiza campos del formulario cuando isOpen es true | Unit | Titulo, importe, descripcion, fecha limite visibles |
| 3 | muestra titulo "Agregar milestone" en modo creacion | Unit | Sin defaultValues -> titulo de creacion |
| 4 | muestra titulo "Editar milestone" en modo edicion | Unit | Con defaultValues -> titulo de edicion |
| 5 | pre-carga campos con defaultValues en modo edicion | Unit | Inputs con valores del milestone existente |
| 6 | muestra error de validacion cuando titulo esta vacio | Integration | Submit sin titulo -> mensaje de error visible |
| 7 | muestra error cuando titulo es menor a 3 caracteres | Integration | "ab" -> error min 3 chars |
| 8 | muestra error cuando titulo supera 200 caracteres | Integration | Titulo 201 chars -> error max |
| 9 | muestra error cuando importe es 0 o negativo | Integration | importeParcial <= 0 -> error |
| 10 | actualiza indicador de importe en tiempo real al escribir | Integration | Escribir 180 en importe -> "Asignado: 270 + 180 = 450 EUR (100%)" |
| 11 | muestra error cuando suma de importes supera el total pactado | Integration | importeParcial que haria sum > importeTotalPactado -> error visual |
| 12 | llama onSubmit con datos correctos cuando formulario es valido | Integration | Submit valido -> callback con CreateMilestoneFormData |
| 13 | llama onClose al cancelar | Unit | Click en Cancelar -> callback onClose |
| 14 | deshabilita boton submit mientras isLoading es true | Unit | prop isLoading=true -> boton deshabilitado |

---

#### SubirEntregableDialog.test.tsx

**Archivo:** `__tests__/components/SubirEntregableDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza campos del formulario | Unit | Titulo, descripcion, URL, select de milestone visibles |
| 2 | popula select de milestones con los del acuerdo | Unit | Muestra opciones "Ninguno" + milestones del acuerdo |
| 3 | muestra error cuando titulo esta vacio | Integration | Submit sin titulo -> error visible |
| 4 | muestra error cuando titulo es menor a 3 caracteres | Integration | "ab" -> error |
| 5 | muestra error cuando URL es invalida | Integration | "no-es-url" -> error de formato URL |
| 6 | acepta string vacio en URL (campo opcional) | Integration | URL vacia -> sin error |
| 7 | acepta URL valida de servicios externos | Integration | URL de Dropbox/Drive -> sin error |
| 8 | llama onSubmit con datos correctos incluyendo milestoneId | Integration | Submit con milestone seleccionado -> CreateEntregableFormData con milestoneId |
| 9 | llama onSubmit con milestoneId undefined cuando no se selecciona milestone | Integration | "Ninguno" seleccionado -> milestoneId undefined |
| 10 | deshabilita boton mientras isLoading | Unit | isLoading=true -> boton deshabilitado |

---

#### AprobarEntregableDialog.test.tsx

**Archivo:** `__tests__/components/AprobarEntregableDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra nombre del entregable a aprobar | Unit | Titulo del entregable visible en el dialog |
| 2 | campo de comentario es opcional: permite submit sin comentario | Integration | Submit vacio -> onConfirm llamado con comentario undefined |
| 3 | acepta comentario hasta 500 caracteres | Integration | 500 chars -> sin error |
| 4 | muestra error cuando comentario supera 500 caracteres | Integration | 501 chars -> error max length |
| 5 | llama onConfirm con comentario cuando se envia | Integration | Submit con texto -> onConfirm({ comentario: 'texto' }) |
| 6 | llama onClose al cancelar | Unit | Click Cancelar -> callback |
| 7 | deshabilita boton mientras isLoading | Unit | isLoading=true -> boton deshabilitado |

---

#### RechazarEntregableDialog.test.tsx

**Archivo:** `__tests__/components/RechazarEntregableDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra nombre del entregable a rechazar | Unit | Titulo visible |
| 2 | muestra error cuando comentario esta vacio | Integration | Submit vacio -> "El comentario es obligatorio" |
| 3 | muestra error cuando comentario tiene menos de 10 chars | Integration | "corto" (9 chars) -> error min 10 |
| 4 | muestra error cuando comentario supera 500 chars | Integration | 501 chars -> error max |
| 5 | acepta comentario de exactamente 10 caracteres | Integration | 10 chars exactos -> sin error |
| 6 | llama onConfirm con comentario obligatorio | Integration | Submit valido -> onConfirm({ comentario: '...' }) |
| 7 | llama onClose al cancelar | Unit | Click Cancelar -> callback |
| 8 | deshabilita boton mientras isLoading | Unit | isLoading=true -> deshabilitado |

---

#### CompletarAcuerdoDialog.test.tsx

**Archivo:** `__tests__/components/CompletarAcuerdoDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra resumen del acuerdo: milestones completados y entregables aprobados | Unit | Datos de resumen del mock visible |
| 2 | muestra aviso cuando hay entregables en estado Entregado (pendientes de revision) | Unit | mockAcuerdoConEntregablesPendientes -> aviso no bloqueante visible |
| 3 | no muestra aviso cuando todos los entregables estan aprobados | Unit | Sin entregables en Entregado -> sin aviso |
| 4 | permite confirmar incluso con entregables pendientes (no bloqueante) | Integration | Click confirmar con entregables pendientes -> onConfirm llamado |
| 5 | llama onConfirm al confirmar | Unit | Click Completar -> onConfirm() |
| 6 | llama onClose al cancelar | Unit | Click Cancelar -> callback |
| 7 | muestra milestone count en el resumen | Unit | "X milestones definidos" |

---

#### CancelarAcuerdoDialog.test.tsx

**Archivo:** `__tests__/components/CancelarAcuerdoDialog.test.tsx`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra texto de advertencia exacto | Unit | "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas." visible |
| 2 | muestra campo de motivo obligatorio | Unit | Input/textarea de motivo en DOM |
| 3 | muestra error cuando motivo esta vacio | Integration | Submit vacio -> error |
| 4 | muestra error cuando motivo tiene menos de 20 chars | Integration | 19 chars -> error min 20 |
| 5 | muestra error cuando motivo supera 1000 chars | Integration | 1001 chars -> error max |
| 6 | acepta motivo de exactamente 20 caracteres | Integration | 20 chars -> sin error, onConfirm llamado |
| 7 | llama onConfirm con motivo valido | Integration | Submit valido -> onConfirm({ motivo: '...' }) |
| 8 | llama onClose al cancelar | Unit | Click Cancelar -> callback |
| 9 | deshabilita boton mientras isLoading | Unit | isLoading=true -> deshabilitado |
| 10 | boton de confirmar es de tipo "destructivo" visualmente | Unit | Clase CSS o variante destructive aplicada |

---

#### MilestonesSection.test.tsx

**Archivo:** `__tests__/components/MilestonesSection.test.tsx`

Seccion que agrupa todos los milestones con la barra de progreso global y boton de agregar.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders barra de progreso global ImporteAsignadoBar | Unit | Componente visible con datos del acuerdo |
| 2 | renders un MilestoneCard por cada milestone | Unit | milestones.length cards en DOM |
| 3 | muestra estado vacio cuando no hay milestones | Unit | mockAcuerdoSinMilestones -> mensaje informativo |
| 4 | muestra boton "Agregar milestone" para artista en acuerdo activo | Unit | miRol=Artista + estadoAcuerdoId=1 -> boton visible |
| 5 | no muestra boton "Agregar milestone" para profesional | Unit | miRol=Profesional -> boton no visible |
| 6 | no muestra boton "Agregar milestone" en acuerdo completado | Unit | estadoAcuerdoId=2 -> boton no visible |
| 7 | no muestra boton "Agregar milestone" en acuerdo cancelado | Unit | estadoAcuerdoId=3 -> boton no visible |
| 8 | abre MilestoneFormDialog al hacer click en agregar | Integration | Click agregar -> dialog visible |

---

#### AcuerdoDetallePage.test.tsx

**Archivo:** `__tests__/components/AcuerdoDetallePage.test.tsx`

Test de integracion de la pagina completa. Mockea el hook `useAcuerdo` para controlar el estado.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra skeleton de carga mientras isLoading es true | Integration | useAcuerdo mockeado con isLoading=true -> skeleton visible |
| 2 | muestra mensaje de error cuando isError es true | Integration | useAcuerdo con isError=true -> error visible |
| 3 | muestra mensaje de acceso denegado para error 403 | Integration | useAcuerdo con error 403 -> mensaje especifico |
| 4 | renders todas las secciones cuando acuerdo carga exitosamente | Integration | useAcuerdo con mockAcuerdoActivo -> cabecera, milestones, timeline visibles |
| 5 | muestra acciones de artista en acuerdo activo (agregar milestone, completar, cancelar) | Integration | miRol=Artista + estadoAcuerdoId=1 -> botones de artista visibles |
| 6 | no muestra boton "Subir entregable" para artista | Integration | miRol=Artista -> boton de subir entregable no visible |
| 7 | muestra boton "Subir entregable" para profesional en acuerdo activo | Integration | miRol=Profesional + estadoAcuerdoId=1 -> boton visible |
| 8 | no muestra boton "Agregar milestone" para profesional | Integration | miRol=Profesional -> boton de agregar milestone no visible |
| 9 | no muestra botones de accion en acuerdo completado excepto timeline | Integration | mockAcuerdoCompletado -> sin botones de mutacion |
| 10 | no muestra botones de accion en acuerdo cancelado | Integration | mockAcuerdoCancelado -> sin botones de mutacion |
| 11 | muestra boton cancelar tanto para artista como para profesional en acuerdo activo | Integration | Ambos roles ven "Cancelar acuerdo" con estadoAcuerdoId=1 |
| 12 | abre CompletarAcuerdoDialog al hacer click en completar | Integration | Click "Completar acuerdo" -> dialog visible |
| 13 | abre CancelarAcuerdoDialog al hacer click en cancelar | Integration | Click "Cancelar acuerdo" -> dialog visible con texto de advertencia |
| 14 | abre SubirEntregableDialog para profesional | Integration | Click "Subir entregable" (como profesional) -> dialog visible |

---

### 4.2 Hooks - Tests Unitarios/Integracion

Todos los tests de hooks siguen el patron establecido en `useTemplates.test.ts`: `vi.mock` del modulo de infrastructure, `renderHook` con wrapper de `QueryClientProvider`, `waitFor` para estados async.

#### useAcuerdo.test.ts

**Archivo:** `__tests__/hooks/useAcuerdo.test.ts`

Hook de query principal. Usa `QUERY_KEYS.crowdsourcing.acuerdos.byId(id)`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna data correctamente cuando API responde con exito | Unit | mockAcuerdoActivo -> `result.current.data` igual al mock |
| 2 | isLoading es true inicialmente | Unit | Estado de carga antes de resolver |
| 3 | isError es true cuando API falla | Unit | Rechazar promesa -> `result.current.isError === true` |
| 4 | llama al service con el id correcto | Unit | Verificar que `acuerdoService.getById` se llamo con el id del hook |

---

#### useAceptarPropuesta.test.ts

**Archivo:** `__tests__/hooks/useAceptarPropuesta.test.ts`

Mutation que en exito debe redirigir al detalle del acuerdo usando el `acuerdoId` del response.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service de aceptar propuesta con los datos del form | Unit | `result.current.mutate(data)` -> service llamado con AceptarPropuestaRequest |
| 2 | en exito, invalida query de propuestas | Unit | `queryClient.invalidateQueries` llamado con key de propuestas |
| 3 | en exito, navega a la ruta del acuerdo usando acuerdoId del response | Unit | `useNavigate` mockeado -> llamado con `/crowdsourcing/acuerdos/{acuerdoId}` |
| 4 | isError es true cuando el service falla | Unit | Reject -> `result.current.isError === true` |

---

#### useRechazarPropuesta.test.ts

**Archivo:** `__tests__/hooks/useRechazarPropuesta.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con propuestaId y motivo opcional | Unit | Service llamado con params correctos |
| 2 | en exito, invalida queries de propuestas de la necesidad | Unit | Invalidacion de query correspondiente |
| 3 | maneja error del service | Unit | isError=true ante fallo |

---

#### useCreateMilestone.test.ts

**Archivo:** `__tests__/hooks/useCreateMilestone.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con acuerdoId y CreateMilestoneRequest | Unit | Verificar parametros correctos |
| 2 | en exito, invalida query del acuerdo byId | Unit | `invalidateQueries(QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId))` |
| 3 | maneja error 400 (importe excedido) | Unit | isError=true con error de negocio |
| 4 | maneja error 403 (no es artista) | Unit | isError=true con error de autorizacion |

---

#### useUpdateMilestone.test.ts

**Archivo:** `__tests__/hooks/useUpdateMilestone.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con acuerdoId, milestoneId y datos actualizados | Unit | Parametros correctos |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | maneja error 400 (milestone ya completado) | Unit | Error de negocio 4011 |

---

#### useDeleteMilestone.test.ts

**Archivo:** `__tests__/hooks/useDeleteMilestone.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service DELETE con acuerdoId y milestoneId | Unit | Parametros correctos |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | maneja error 400 cuando milestone tiene entregables | Unit | Error 4012 |

---

#### useCreateEntregable.test.ts

**Archivo:** `__tests__/hooks/useCreateEntregable.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con acuerdoId y CreateEntregableRequest | Unit | Parametros correctos incluido milestoneId opcional |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | maneja error 403 (no es profesional) | Unit | isError=true |
| 4 | maneja error 400 (acuerdo no activo) | Unit | Error 4010 |

---

#### useAprobarEntregable.test.ts

**Archivo:** `__tests__/hooks/useAprobarEntregable.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con entregableId y comentario opcional | Unit | Sin comentario: parametro omitido o undefined |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta con acuerdoId |
| 3 | cuando todosAprobadosEnMilestone es true, expone flag para sugerencia de UI | Unit | `result.current.data.todosAprobadosEnMilestone === true` accesible |
| 4 | maneja error 400 (entregable no en estado Entregado) | Unit | Error 4013 |

---

#### useRechazarEntregable.test.ts

**Archivo:** `__tests__/hooks/useRechazarEntregable.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con entregableId y comentario obligatorio | Unit | Comentario siempre presente |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | maneja error 400 (comentario vacio) | Unit | Error de validacion |

---

#### useCompletarAcuerdo.test.ts

**Archivo:** `__tests__/hooks/useCompletarAcuerdo.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service PATCH completar con acuerdoId | Unit | Parametro correcto, sin body |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | maneja error 403 (no es artista) | Unit | isError=true |
| 4 | maneja error 400 (acuerdo no activo) | Unit | Error 4010 |

---

#### useCancelarAcuerdo.test.ts

**Archivo:** `__tests__/hooks/useCancelarAcuerdo.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | llama al service con acuerdoId y motivo | Unit | Parametros correctos |
| 2 | en exito, invalida query del acuerdo | Unit | Invalidacion correcta |
| 3 | puede ser llamado tanto desde rol artista como profesional | Unit | Sin logica de rol en el hook, solo parametros |
| 4 | maneja error 400 (motivo invalido) | Unit | isError=true |

---

### 4.3 Services - Tests Unitarios

Los tests de services verifican que los metodos construyen correctamente las llamadas HTTP (usando mocks de `fetch` o del cliente HTTP del proyecto). Siguen el patron de los services existentes.

#### acuerdo.service.test.ts

**Archivo:** `__tests__/services/acuerdo.service.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | getById hace GET a la URL correcta con el id | Unit | `GET /api/crowdsourcing/acuerdos/{id}` |
| 2 | getById retorna datos mapeados correctamente | Unit | Response del mock -> AcuerdoDto tipado |
| 3 | completar hace PATCH a la URL correcta | Unit | `PATCH /api/crowdsourcing/acuerdos/{id}/completar` sin body |
| 4 | cancelar hace PATCH con motivo en el body | Unit | Body contiene `{ motivo: '...' }` |
| 5 | maneja error 403 lanzando excepcion | Unit | Response 403 -> throw con mensaje adecuado |
| 6 | maneja error 404 lanzando excepcion | Unit | Response 404 -> throw |

---

#### milestone.service.test.ts

**Archivo:** `__tests__/services/milestone.service.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | create hace POST a la URL correcta con acuerdoId | Unit | `POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones` |
| 2 | create envia body con los campos correctos | Unit | Body con titulo, importeParcial, descripcion, fechaLimite |
| 3 | update hace PUT a la URL correcta con acuerdoId y milestoneId | Unit | `PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` |
| 4 | delete hace DELETE a la URL correcta | Unit | `DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` |
| 5 | maneja error 400 (suma importe excedida) | Unit | Error 4009 |

---

#### entregable.service.test.ts

**Archivo:** `__tests__/services/entregable.service.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | create hace POST con acuerdoId y datos del entregable | Unit | URL correcta y body con titulo, urlRecurso, milestoneId opcional |
| 2 | aprobar hace PATCH a /entregables/{id}/aprobar | Unit | URL correcta, body con comentario opcional |
| 3 | rechazar hace PATCH a /entregables/{id}/rechazar | Unit | URL correcta, body con comentario obligatorio |
| 4 | maneja error 403 en aprobar (no es artista) | Unit | Error 403 |
| 5 | maneja error 400 en rechazar (comentario muy corto) | Unit | Error de validacion |

---

## 5. Tests de Integracion - Flujos Criticos

Estos tests cubren flujos completos de usuario que cruzan multiples componentes. Se implementan como tests de componentes que mockean el hook de nivel superior.

### 5.1 Flujo: Artista Acepta Propuesta

**Contexto:** En la pagina de propuestas de una necesidad, el artista hace click en "Aceptar propuesta", completa el formulario y confirma.

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | flujo completo de aceptar propuesta con datos validos | PropuestasNecesidadSection o AceptarPropuestaDialog | 1. Render con propuesta Pendiente. 2. Click "Aceptar". 3. Formulario precargado con titulo de necesidad y fechas. 4. Ajustar titulo. 5. Click Confirmar. | useAceptarPropuesta.mutate llamado con datos correctos. En exito: navegacion a `/crowdsourcing/acuerdos/{acuerdoId}`. Toast con texto exacto "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." |
| 2 | muestra errores de validacion en formulario de aceptar | AceptarPropuestaDialog | Submit con fechaFinPrevista <= fechaInicio | Error de validacion "La fecha de fin prevista debe ser posterior a la fecha de inicio" visible |
| 3 | error de backend: ya existe acuerdo activo (4008) | AceptarPropuestaDialog | Mutacion falla con error 4008 | Toast o mensaje de error "Ya existe un acuerdo activo para esta necesidad" |

---

### 5.2 Flujo: Artista Gestiona Milestones

**Contexto:** Vista de detalle del acuerdo activo como artista. Crear, editar y eliminar milestones.

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | artista crea milestone correctamente | AcuerdoDetallePage (mockAcuerdoActivo, miRol=Artista) | 1. Click "+ Agregar milestone". 2. Rellenar formulario con titulo y importe. 3. Confirmar. | useCreateMilestone.mutate llamado. Invalidacion de query acuerdo. Dialog cierra. Toast de exito. |
| 2 | indicador de importe se actualiza en tiempo real | MilestoneFormDialog (importeAsignado=270, total=450) | Escribir 100 en importeParcial | "Asignado: 270 + 100 = 370 de 450 EUR" visible antes del submit |
| 3 | error cuando suma de importes supera total | MilestoneFormDialog (importeAsignado=270, total=450) | Escribir 200 (suma = 470 > 450) | Error de validacion visible en tiempo real |
| 4 | artista edita milestone no completado | MilestoneCard (mockMilestoneConEntregables) | 1. Click editar. 2. Cambiar titulo. 3. Confirmar. | useUpdateMilestone.mutate llamado con datos actualizados. Dialog cierra. |
| 5 | artista no puede editar milestone completado | MilestoneCard (mockMilestoneCompletado) | Verificar boton editar | Boton editar NO en DOM para milestone con fechaCompletado != null |
| 6 | artista elimina milestone sin entregables | MilestoneCard (mockMilestoneSinEntregables) | Click eliminar y confirmar | useDeleteMilestone.mutate llamado. Invalidacion de query. |
| 7 | artista no puede eliminar milestone con entregables | MilestoneCard (mockMilestoneConEntregables) | Verificar boton eliminar | Boton eliminar NO en DOM (o deshabilitado) cuando entregables.length > 0 |

---

### 5.3 Flujo: Profesional Sube Entregable

**Contexto:** Vista de detalle del acuerdo activo como profesional.

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | profesional sube entregable con URL y milestone | AcuerdoDetallePage (mockAcuerdoActivoProfesional) | 1. Click "Subir entregable". 2. Rellenar titulo, URL de Drive, seleccionar milestone. 3. Confirmar. | useCreateEntregable.mutate llamado con CreateEntregableRequest correcto. Toast "Entregable subido correctamente. El artista sera notificado." |
| 2 | profesional sube entregable sin milestone (agrupacion libre) | SubirEntregableDialog | Seleccionar "Ninguno" en select de milestone | milestoneId: undefined en el request |
| 3 | error de validacion URL invalida | SubirEntregableDialog | Escribir "texto-sin-formato-url" en URL | Error "Debe ser una URL valida" visible sin submit |
| 4 | profesional no ve botones de artista | AcuerdoDetallePage (mockAcuerdoActivoProfesional) | Render y observar | Sin botones "Agregar milestone", "Aprobar", "Rechazar", "Completar acuerdo" en DOM |

---

### 5.4 Flujo: Artista Revisa Entregables

**Contexto:** Vista de detalle del acuerdo con entregable en estado Entregado.

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | artista aprueba entregable sin comentario | EntregableItem (mockEntregableEntregado, miRol=Artista) | Click "Aprobar" -> Dialog abre -> Click Confirmar sin comentario | useAprobarEntregable.mutate({ comentario: undefined }). Invalidacion de query. Toast de exito. |
| 2 | artista aprueba con comentario opcional | AprobarEntregableDialog | Escribir comentario y confirmar | onConfirm({ comentario: 'texto' }) |
| 3 | cuando todosAprobadosEnMilestone es true, se muestra sugerencia de marcar milestone como completado | AprobarEntregableDialog / AcuerdoDetallePage | Mock useAprobarEntregable retorna todosAprobadosEnMilestone=true | Texto sugerencia "Todos los entregables del milestone estan aprobados" visible |
| 4 | artista rechaza entregable con comentario obligatorio valido | RechazarEntregableDialog | Escribir 15 chars y confirmar | useRechazarEntregable.mutate({ comentario: '...' }). Toast de exito. |
| 5 | artista no puede rechazar sin comentario | RechazarEntregableDialog | Submit vacio | Error "El comentario es obligatorio al rechazar" visible |
| 6 | artista no puede rechazar con menos de 10 caracteres | RechazarEntregableDialog | Escribir "corto" | Error min 10 chars visible |
| 7 | botones aprobar/rechazar no visibles en entregable ya aprobado | EntregableItem (mockEntregableAprobado) | Render y observar | Sin botones de revision para entregable Aprobado |

---

### 5.5 Flujo: Completar Acuerdo

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | artista completa acuerdo con aviso de entregables pendientes | AcuerdoDetallePage (mockAcuerdoConEntregablesPendientes, miRol=Artista) | Click "Completar acuerdo" -> Dialog abre | Aviso de entregables pendientes visible en el dialog (no bloqueante). Boton confirmar habilitado. |
| 2 | artista completa acuerdo sin entregables pendientes | CompletarAcuerdoDialog (sin entregables en Entregado) | Click Confirmar | Sin aviso de pendientes. useCompletarAcuerdo.mutate llamado. Toast "Acuerdo completado. Puedes dejar una valoracion al profesional." |
| 3 | artista completa acuerdo sin milestones definidos | mockAcuerdoSinMilestones | Click Completar -> Confirmar | Resumen muestra "0 milestones". Mutacion llamada exitosamente (FA-05). |
| 4 | profesional no puede ver ni usar el boton de completar | AcuerdoDetallePage (mockAcuerdoActivoProfesional) | Render y observar | Boton "Completar acuerdo" NO en DOM |

---

### 5.6 Flujo: Cancelar Acuerdo

| # | Test Case | Componente origen | Acciones | Assertions |
|---|-----------|-------------------|----------|------------|
| 1 | artista cancela acuerdo con motivo valido (>= 20 chars) | AcuerdoDetallePage (mockAcuerdoActivo, miRol=Artista) | Click "Cancelar" -> Dialog con advertencia -> Escribir motivo (25 chars) -> Confirmar | useCancelarAcuerdo.mutate({ motivo: '...' }). Toast "Acuerdo cancelado." |
| 2 | profesional cancela acuerdo (ambos pueden cancelar) | AcuerdoDetallePage (mockAcuerdoActivoProfesional) | Click "Cancelar" -> motivo valido -> Confirmar | useCancelarAcuerdo.mutate llamado correctamente. |
| 3 | texto de advertencia exacto visible en dialog | CancelarAcuerdoDialog | Render | "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas." en DOM |
| 4 | error cuando motivo tiene menos de 20 chars | CancelarAcuerdoDialog | Escribir 19 chars y confirmar | Error "El motivo debe tener al menos 20 caracteres" visible |
| 5 | boton cancelar NO visible en acuerdo ya completado | AcuerdoDetallePage (mockAcuerdoCompletado) | Render y observar | Boton "Cancelar acuerdo" NO en DOM |
| 6 | boton cancelar NO visible en acuerdo ya cancelado | AcuerdoDetallePage (mockAcuerdoCancelado) | Render y observar | Boton "Cancelar acuerdo" NO en DOM |

---

## 6. Escenarios de Edge Cases y Flujos Alternativos

| ID Spec | Escenario | Componente | Test File |
|---------|-----------|------------|-----------|
| FA-01 | Error 4008: ya existe acuerdo activo al aceptar propuesta | AceptarPropuestaDialog | AcuerdoDetallePage.test.tsx |
| FA-02 | Acuerdo no activo: profesional intenta subir entregable (boton no visible) | AcuerdoDetallePage | AcuerdoDetallePage.test.tsx |
| FA-03 | Milestone con entregables: boton eliminar no visible | MilestoneCard | MilestoneCard.test.tsx |
| FA-04 | Suma de importes supera total: error en tiempo real | MilestoneFormDialog | MilestoneFormDialog.test.tsx |
| FA-05 | Completar acuerdo sin milestones (resumen con 0 milestones) | CompletarAcuerdoDialog | CompletarAcuerdoDialog.test.tsx |
| FA-07 | 403 Forbidden: mensaje de acceso denegado en pagina | AcuerdoDetallePage | AcuerdoDetallePage.test.tsx |
| FA-08 | Entregable no en Entregado: botones de revision no visibles | EntregableItem | EntregableItem.test.tsx |
| FA-09 | Milestone completado: boton editar no visible | MilestoneCard | MilestoneCard.test.tsx |

---

## 7. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Prioridad |
|---------|--------|-----------|----------|-----------|
| `AcuerdoDetallePage.tsx` | 85% | 90% | 85% | Alta |
| `AcuerdoCabecera.tsx` | 90% | 100% | 85% | Alta |
| `MilestonesSection.tsx` | 85% | 95% | 85% | Alta |
| `MilestoneCard.tsx` | 90% | 100% | 90% | Alta |
| `EntregableItem.tsx` | 90% | 100% | 90% | Alta |
| `MilestoneFormDialog.tsx` | 85% | 90% | 80% | Alta |
| `SubirEntregableDialog.tsx` | 85% | 90% | 80% | Alta |
| `AprobarEntregableDialog.tsx` | 90% | 100% | 85% | Alta |
| `RechazarEntregableDialog.tsx` | 90% | 100% | 90% | Alta |
| `CompletarAcuerdoDialog.tsx` | 85% | 95% | 85% | Alta |
| `CancelarAcuerdoDialog.tsx` | 90% | 100% | 90% | Alta |
| `EstadoAcuerdoBadge.tsx` | 100% | 100% | 100% | Media |
| `EstadoEntregableBadge.tsx` | 100% | 100% | 100% | Media |
| `ImporteAsignadoBar.tsx` | 95% | 100% | 90% | Media |
| `AcuerdoTimeline.tsx` | 90% | 100% | 85% | Media |
| `useAcuerdo.ts` | 90% | 100% | 85% | Alta |
| `useAceptarPropuesta.ts` | 90% | 100% | 85% | Alta |
| `useCreateMilestone.ts` | 90% | 100% | 80% | Alta |
| `useUpdateMilestone.ts` | 85% | 100% | 80% | Media |
| `useDeleteMilestone.ts` | 85% | 100% | 80% | Media |
| `useCreateEntregable.ts` | 90% | 100% | 80% | Alta |
| `useAprobarEntregable.ts` | 90% | 100% | 85% | Alta |
| `useRechazarEntregable.ts` | 90% | 100% | 85% | Alta |
| `useCompletarAcuerdo.ts` | 90% | 100% | 85% | Alta |
| `useCancelarAcuerdo.ts` | 90% | 100% | 80% | Alta |
| `acuerdo.service.ts` | 95% | 100% | 90% | Alta |
| `milestone.service.ts` | 95% | 100% | 90% | Media |
| `entregable.service.ts` | 95% | 100% | 90% | Media |

**Meta Global:** 80%+ en todas las metricas.

---

## 8. Patron de Archivos: Ubicacion Definitiva

Siguiendo la convencion establecida en el proyecto (donde crowdsourcing tiene su propia carpeta de feature en `src/web/src/features/crowdsourcing/`), los tests de acuerdos se ubican en la misma feature, dentro de subdirectorios que reflejan la subdivision:

```
src/web/src/features/crowdsourcing/
├── __mocks__/
│   ├── crowdsourcing.mock.ts    (existente)
│   ├── necesidad.mock.ts        (existente)
│   ├── propuesta.mock.ts        (existente)
│   └── acuerdo.mock.ts          (NUEVO - esta feature)
└── __tests__/
    ├── components/
    │   ├── [tests existentes de CS-01, CS-02, CS-03]
    │   ├── EstadoAcuerdoBadge.test.tsx      (NUEVO)
    │   ├── EstadoEntregableBadge.test.tsx   (NUEVO)
    │   ├── AcuerdoCabecera.test.tsx         (NUEVO)
    │   ├── ImporteAsignadoBar.test.tsx      (NUEVO)
    │   ├── MilestoneCard.test.tsx           (NUEVO)
    │   ├── EntregableItem.test.tsx          (NUEVO)
    │   ├── AcuerdoTimeline.test.tsx         (NUEVO)
    │   ├── MilestoneFormDialog.test.tsx     (NUEVO)
    │   ├── SubirEntregableDialog.test.tsx   (NUEVO)
    │   ├── AprobarEntregableDialog.test.tsx (NUEVO)
    │   ├── RechazarEntregableDialog.test.tsx (NUEVO)
    │   ├── CompletarAcuerdoDialog.test.tsx  (NUEVO)
    │   ├── CancelarAcuerdoDialog.test.tsx   (NUEVO)
    │   ├── MilestonesSection.test.tsx       (NUEVO)
    │   └── AcuerdoDetallePage.test.tsx      (NUEVO)
    ├── hooks/
    │   ├── [tests existentes de CS-01, CS-02, CS-03]
    │   ├── useAcuerdo.test.ts               (NUEVO)
    │   ├── useAceptarPropuesta.test.ts      (NUEVO)
    │   ├── useRechazarPropuesta.test.ts     (NUEVO)
    │   ├── useCreateMilestone.test.ts       (NUEVO)
    │   ├── useUpdateMilestone.test.ts       (NUEVO)
    │   ├── useDeleteMilestone.test.ts       (NUEVO)
    │   ├── useCreateEntregable.test.ts      (NUEVO)
    │   ├── useAprobarEntregable.test.ts     (NUEVO)
    │   ├── useRechazarEntregable.test.ts    (NUEVO)
    │   ├── useCompletarAcuerdo.test.ts      (NUEVO)
    │   └── useCancelarAcuerdo.test.ts       (NUEVO)
    └── services/
        ├── acuerdo.service.test.ts          (NUEVO)
        ├── milestone.service.test.ts        (NUEVO)
        └── entregable.service.test.ts       (NUEVO)
```

---

## 9. Notas Criticas de Implementacion

### 9.1 El campo miRol es la clave del renderizado condicional

El campo `miRol: 'Artista' | 'Profesional'` viene directamente del backend y es el unico mecanismo que determina que acciones se muestran en UI. No hay comparacion de IDs en el frontend. Todos los tests de renderizado condicional deben pasar exactamente este campo en el mock del acuerdo. No hace falta mockear un contexto de autenticacion para los tests de componentes: basta con pasar `mockAcuerdoActivo` (miRol=Artista) o `mockAcuerdoActivoProfesional` (miRol=Profesional).

### 9.2 Indicador de importe en tiempo real

El calculo del importe en tiempo real en `MilestoneFormDialog` usa `watch` de React Hook Form para observar el campo `importeParcial`. El componente recibe como prop `importeAsignadoActual` (suma de milestones ya existentes, sin contar el que se esta creando) y calcula `importeAsignadoActual + valorActualDelCampo`. El test debe verificar que al cambiar el valor del input, el texto del indicador se actualiza sin necesidad de submit. No hay llamada al backend en este calculo.

### 9.3 Sugerencia de milestone completado despues de aprobar

Cuando `useAprobarEntregable` retorna `todosAprobadosEnMilestone: true`, el componente `AcuerdoDetallePage` o `MilestoneCard` debe mostrar una sugerencia para marcar el milestone como completado. El test correspondiente debe mockear la mutacion para que retorne este valor y verificar que el texto de sugerencia aparece en pantalla.

### 9.4 Toast messages: texto exacto

Los criterios de aceptacion especifican textos de toast exactos. Los tests de integracion de flujos deben verificar estos textos exactos:
- Aceptar propuesta: `"Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."`
- Subir entregable: `"Entregable subido correctamente. El artista sera notificado."`
- Completar acuerdo: `"Acuerdo completado. Puedes dejar una valoracion al profesional."`
- Cancelar acuerdo: `"Acuerdo cancelado."`

Para testear toasts, usar `screen.getByText` con el texto exacto despues de la mutacion exitosa, asegurando que el sistema de toasts del proyecto (Sonner o similar) renderiza en el DOM durante los tests.

### 9.5 Flujo alternativo FA-07: acceso denegado

Cuando `useAcuerdo` devuelve un error con status 403, la pagina debe mostrar un mensaje de acceso denegado en lugar del detalle. El test de `AcuerdoDetallePage.test.tsx` debe incluir este caso mockeando el hook con `isError=true` y un error que tenga el codigo 403.

### 9.6 Validaciones Zod en formularios

Las validaciones Zod se testean a nivel de componente (integration tests en los dialogs), no de schema unitario. Este patron es consistente con los tests existentes en `BackingForm.test.tsx`. El schema en si es simple y su correctitud se verifica indirectamente a traves de los tests de formulario.

---

## 10. Comandos de Ejecucion

```bash
# Desde el directorio src/web/

# Ejecutar todos los tests de la feature
npm run test -- --reporter=verbose src/features/crowdsourcing/__tests__

# Ejecutar solo tests de acuerdos
npm run test -- --reporter=verbose src/features/crowdsourcing/__tests__/components/Acuerdo
npm run test -- --reporter=verbose src/features/crowdsourcing/__tests__/hooks/useAcuerdo

# Ejecutar con cobertura
npm run test:coverage -- src/features/crowdsourcing

# Watch mode para desarrollo
npm run test -- --watch src/features/crowdsourcing/__tests__
```

---

## 11. Checklist

- [ ] Mock data `acuerdo.mock.ts` cubre todos los estados (Activo/Completado/Cancelado, ambos roles, milestones con y sin entregables en todos sus estados)
- [ ] Tests de `EstadoAcuerdoBadge` y `EstadoEntregableBadge` cubren los 3 estados cada uno
- [ ] Tests de `EntregableItem` verifican renderizado condicional para los 3 estados de entregable x 2 roles x 2 estados de acuerdo
- [ ] Tests de `MilestoneCard` verifican que el boton editar no aparece para milestone completado (FA-09)
- [ ] Tests de `MilestoneCard` verifican que el boton eliminar no aparece cuando hay entregables (FA-03)
- [ ] `MilestoneFormDialog` testea el indicador de importe en tiempo real sin submit
- [ ] `MilestoneFormDialog` testea el error cuando la suma supera el total pactado (FA-04)
- [ ] `RechazarEntregableDialog` testea que el comentario es obligatorio y tiene minimo 10 chars (AC-CS04-8)
- [ ] `CancelarAcuerdoDialog` testea texto exacto de advertencia (AC-CS04-11)
- [ ] `CancelarAcuerdoDialog` testea minimo 20 chars en motivo (AC-CS04-11)
- [ ] `CompletarAcuerdoDialog` testea aviso no bloqueante de entregables pendientes (AC-CS04-10)
- [ ] Tests de hooks usan `vi.mock` del modulo de infrastructure, NO de fetch directo
- [ ] `useAceptarPropuesta` verifica navegacion a `/crowdsourcing/acuerdos/{acuerdoId}`
- [ ] `useAprobarEntregable` verifica acceso al flag `todosAprobadosEnMilestone`
- [ ] Tests de services verifican URLs de API usando `API_ROUTES` constants
- [ ] Tests de integracion de `AcuerdoDetallePage` cubren el acceso 403 (FA-07)
- [ ] Toast messages verificados con texto exacto segun criterios de aceptacion
- [ ] Cobertura global >= 80% en lineas, funciones y branches
- [ ] Tests pasan en < 60 segundos (meta de performance del proyecto)
