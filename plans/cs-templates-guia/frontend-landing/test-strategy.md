# Estrategia de Testing: cs-templates-guia (Landing)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia
**Target:** src/web (Landing)
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests - Components | 18 | 85% |
| Unit Tests - Hooks | 8 | 90% |
| Unit Tests - Services | 4 | 95% |
| Integration Tests | 6 | 80% |
| Total | 36 | 85%+ |

## 2. Estructura de Tests

```
src/web/src/features/crowdsourcing/
├── __tests__/
│   ├── components/
│   │   ├── TemplateCard.test.tsx
│   │   ├── TemplateGallery.test.tsx
│   │   ├── WizardStepper.test.tsx
│   │   ├── NecesidadItem.test.tsx
│   │   ├── NecesidadList.test.tsx
│   │   ├── BudgetSummary.test.tsx
│   │   ├── RolProfesionalTooltip.test.tsx
│   │   ├── ConfirmationSummary.test.tsx
│   │   └── NuevoProyectoPage.test.tsx
│   ├── hooks/
│   │   ├── useTemplates.test.ts
│   │   ├── useTemplateDetail.test.ts
│   │   ├── useGenerarNecesidades.test.ts
│   │   └── useWizardState.test.ts
│   ├── services/
│   │   └── crowdsourcing.service.test.ts
│   └── integration/
│       ├── wizard-flow.test.tsx
│       ├── budget-calculations.test.tsx
│       ├── navigation.test.tsx
│       └── validation.test.tsx
└── __mocks__/
    ├── crowdsourcing.mock.ts
    ├── handlers.ts
    └── test-utils.tsx
```

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/crowdsourcing.mock.ts`

```typescript
import {
  PlantillaProyectoList,
  PlantillaProyecto,
  PlantillaProyectoNecesidad,
  GenerarNecesidadesResult
} from '@/shared/types/crowdsourcing';

// Mock para lista de templates
export const mockTemplatesList: PlantillaProyectoList[] = [
  {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    nombre: 'Producción de EP',
    descripcion: 'Plantilla completa para producir un EP de 4-6 canciones',
    icono: '🎵',
    orden: 1,
    precioMinTotal: 3000.00,
    precioMaxTotal: 8000.00,
    moneda: 1,
    cantidadNecesidades: 8,
    fases: ['Preproducción', 'Grabación', 'Mezcla y Master', 'Promoción']
  },
  {
    id: '7cb95f12-9301-4872-a1de-3f984e22bbc1',
    nombre: 'Producción de Álbum Completo',
    descripcion: 'Plantilla para producir un álbum de 10-12 canciones',
    icono: '💿',
    orden: 2,
    precioMinTotal: 8000.00,
    precioMaxTotal: 25000.00,
    moneda: 1,
    cantidadNecesidades: 12,
    fases: ['Preproducción', 'Grabación', 'Mezcla y Master', 'Diseño', 'Promoción']
  },
  {
    id: '9ab12c45-7821-4963-c2ef-1d873f55aac3',
    nombre: 'Producción de Sencillo',
    descripcion: 'Plantilla ágil para producir 1-2 canciones',
    icono: '🎤',
    orden: 3,
    precioMinTotal: 1000.00,
    precioMaxTotal: 3500.00,
    moneda: 1,
    cantidadNecesidades: 5,
    fases: ['Grabación', 'Mezcla y Master', 'Promoción']
  }
];

// Mock para necesidad individual
export const mockNecesidad: PlantillaProyectoNecesidad = {
  id: '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d',
  fase: 'Preproducción',
  titulo: 'Productor Musical',
  descripcion: 'Desarrollo de arreglos, estructura y dirección musical',
  rolProfesional: {
    id: 1,
    nombre: 'Productor Musical',
    descripcion: 'Profesional que guía el proceso creativo y técnico',
    categoriaRolId: 1,
    modalidadCobro: 'Por proyecto'
  },
  precioMinOrientativo: 500.00,
  precioMaxOrientativo: 1500.00,
  moneda: 1,
  prioridad: 'Alta',
  orden: 1
};

// Mock para template detallado
export const mockTemplateDetail: PlantillaProyecto = {
  id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
  nombre: 'Producción de EP',
  descripcion: 'Plantilla completa para producir un EP de 4-6 canciones',
  icono: '🎵',
  orden: 1,
  necesidades: [
    mockNecesidad,
    {
      id: '2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e',
      fase: 'Grabación',
      titulo: 'Ingeniero de Grabación',
      descripcion: 'Grabación profesional de instrumentos y voces',
      rolProfesional: {
        id: 2,
        nombre: 'Ingeniero de Grabación',
        descripcion: 'Técnico especializado en captura de audio',
        categoriaRolId: 2,
        modalidadCobro: 'Por día'
      },
      precioMinOrientativo: 800.00,
      precioMaxOrientativo: 2000.00,
      moneda: 1,
      prioridad: 'Alta',
      orden: 2
    },
    {
      id: '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f',
      fase: 'Mezcla y Master',
      titulo: 'Ingeniero de Mezcla',
      descripcion: 'Mezcla profesional del EP',
      rolProfesional: {
        id: 3,
        nombre: 'Ingeniero de Mezcla',
        descripcion: 'Especialista en balance y procesamiento de audio',
        categoriaRolId: 2,
        modalidadCobro: 'Por canción'
      },
      precioMinOrientativo: 600.00,
      precioMaxOrientativo: 1800.00,
      moneda: 1,
      prioridad: 'Media',
      orden: 3
    }
  ],
  resumen: {
    precioMinTotal: 3700.00,
    precioMaxTotal: 11400.00,
    moneda: 1,
    cantidadNecesidadesAlta: 2,
    cantidadNecesidadesMedia: 1,
    cantidadNecesidadesBaja: 0
  }
};

// Mock para respuesta de generación
export const mockGenerarNecesidadesResponse: GenerarNecesidadesResult = {
  necesidadesCreadas: 3,
  necesidadIds: [
    'a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d',
    'b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e',
    'c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f'
  ],
  presupuestoTotalMin: 1900.00,
  presupuestoTotalMax: 5300.00,
  moneda: 1
};
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from 'msw';
import {
  mockTemplatesList,
  mockTemplateDetail,
  mockGenerarNecesidadesResponse
} from './crowdsourcing.mock';

const API_BASE = 'http://localhost:5001';

export const crowdsourcingHandlers = [
  // GET /api/crowdsourcing/templates
  rest.get(`${API_BASE}/api/crowdsourcing/templates`, (req, res, ctx) => {
    return res(
      ctx.status(200),
      ctx.json({
        data: { items: mockTemplatesList },
        messages: [{ message: 'Plantillas obtenidas exitosamente', errorCode: '0000' }]
      })
    );
  }),

  // GET /api/crowdsourcing/templates/:id
  rest.get(`${API_BASE}/api/crowdsourcing/templates/:id`, (req, res, ctx) => {
    const { id } = req.params;

    if (id === '404-not-found') {
      return res(
        ctx.status(404),
        ctx.json({
          data: null,
          messages: [{ message: 'Plantilla no encontrada', errorCode: '2006' }]
        })
      );
    }

    return res(
      ctx.status(200),
      ctx.json({
        data: mockTemplateDetail,
        messages: [{ message: 'Plantilla obtenida exitosamente', errorCode: '0000' }]
      })
    );
  }),

  // POST /api/crowdsourcing/templates/:id/generar
  rest.post(`${API_BASE}/api/crowdsourcing/templates/:id/generar`, async (req, res, ctx) => {
    const body = await req.json();

    // Validación: al menos 1 necesidad
    if (!body.necesidadesSeleccionadas || body.necesidadesSeleccionadas.length === 0) {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'Debe seleccionar al menos una necesidad', errorCode: '1001' }]
        })
      );
    }

    // Validación: presupuesto max >= min
    const invalidBudget = body.necesidadesSeleccionadas.some(
      (n: any) => n.presupuestoMax < n.presupuestoMin
    );

    if (invalidBudget) {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{
            message: 'El presupuesto máximo debe ser mayor o igual al mínimo',
            errorCode: '1009'
          }]
        })
      );
    }

    return res(
      ctx.status(201),
      ctx.json({
        data: mockGenerarNecesidadesResponse,
        messages: [{
          message: 'Necesidades generadas exitosamente',
          errorCode: '0001'
        }]
      })
    );
  })
];
```

### 3.3 Test Utilities

**Archivo:** `__mocks__/test-utils.tsx`

```typescript
import { ReactElement, ReactNode } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';

// Query client para tests (sin retry)
const createTestQueryClient = () => new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      cacheTime: 0,
    },
    mutations: {
      retry: false,
    },
  },
});

// Wrapper con providers
interface AllProvidersProps {
  children: ReactNode;
}

const AllProviders = ({ children }: AllProvidersProps) => {
  const queryClient = createTestQueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </QueryClientProvider>
  );
};

// Render personalizado con providers
export const renderWithProviders = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => {
  return render(ui, { wrapper: AllProviders, ...options });
};

// Re-export everything
export * from '@testing-library/react';
export { renderWithProviders as render };
```

## 4. Tests por Modulo

### 4.1 Components

#### TemplateCard.test.tsx

**Archivo:** `__tests__/components/TemplateCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders template data correctly | Unit | Renderiza nombre, descripcion, icono, precio, cantidad necesidades |
| displays correct price range | Unit | Formatea precios min-max correctamente con EUR |
| shows phases count | Unit | Muestra cantidad de fases del template |
| calls onSelect with template id on click | Unit | Ejecuta callback con ID correcto al hacer click |
| applies hover styles | Unit | Cambia estilos al hacer hover (border, background) |
| is keyboard accessible | Unit | Puede activarse con Enter y Space |
| has correct aria labels | Unit | Contiene aria-label descriptivo |

**Casos Detallados:**

```markdown
1. **renders template data correctly**
   - Render <TemplateCard template={mockTemplatesList[0]} onSelect={vi.fn()} />
   - Assert: nombre "Producción de EP" visible
   - Assert: descripción visible
   - Assert: icono 🎵 renderizado
   - Assert: "8 necesidades" visible

2. **displays correct price range**
   - Render con template
   - Assert: "3,000 EUR - 8,000 EUR" visible (formateado)

3. **calls onSelect with template id on click**
   - Render con mock onSelect
   - Click en card
   - Assert: onSelect llamado con id correcto

4. **is keyboard accessible**
   - Render card
   - Focus en card con Tab
   - Press Enter
   - Assert: onSelect ejecutado
```

---

#### TemplateGallery.test.tsx

**Archivo:** `__tests__/components/TemplateGallery.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders loading skeletons | Unit | Muestra 6 skeletons mientras carga |
| renders grid of template cards | Unit | Renderiza 3 cards en grid responsive |
| displays empty state when no templates | Unit | Muestra mensaje "No hay plantillas disponibles" |
| handles template selection | Unit | Navega a paso 2 al seleccionar template |
| shows error message on fetch error | Unit | Muestra toast de error si falla API |
| renders alternative link | Unit | Muestra link "Crear plantilla personalizada" |

**Casos Detallados:**

```markdown
1. **renders loading skeletons**
   - Mock useTemplates con isLoading: true
   - Render <TemplateGallery />
   - Assert: 6 skeleton loaders visibles

2. **renders grid of template cards**
   - Mock useTemplates con data: mockTemplatesList
   - Render gallery
   - Assert: 3 TemplateCard renderizados
   - Assert: grid tiene clase "grid-cols-1 md:grid-cols-2 lg:grid-cols-3"

3. **displays empty state when no templates**
   - Mock useTemplates con data: []
   - Render gallery
   - Assert: "No hay plantillas disponibles" visible
   - Assert: boton CTA "Crear plantilla personalizada" visible
```

---

#### WizardStepper.test.tsx

**Archivo:** `__tests__/components/WizardStepper.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders correct number of steps | Unit | Renderiza 3 pasos con labels correctos |
| highlights current step | Unit | Paso actual tiene estilos especiales |
| marks completed steps | Unit | Pasos anteriores muestran checkmark |
| shows future steps as inactive | Unit | Pasos futuros tienen estilos muted |
| displays progress percentage | Unit | Barra de progreso muestra 33%, 66%, 100% |
| has correct aria labels | Unit | role="navigation" y aria-current="step" |

**Casos Detallados:**

```markdown
1. **renders correct number of steps**
   - Render <WizardStepper steps={3} currentStep={1} />
   - Assert: 3 step indicators visibles
   - Assert: labels "Seleccionar", "Personalizar", "Confirmar"

2. **highlights current step**
   - Render con currentStep={2}
   - Assert: paso 2 tiene border-primary y bg-primary
   - Assert: paso 1 tiene checkmark (completado)
   - Assert: paso 3 tiene opacity-50 (futuro)

3. **displays progress percentage**
   - Render con currentStep={2}
   - Assert: barra de progreso tiene width="66%"
```

---

#### NecesidadItem.test.tsx

**Archivo:** `__tests__/components/NecesidadItem.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders necesidad data | Unit | Muestra titulo, rol, precio, prioridad |
| checkbox checked by default if Esencial | Unit | Checkbox pre-seleccionado si prioridad=Alta |
| checkbox unchecked by default if not Esencial | Unit | Checkbox sin seleccionar si prioridad=Media/Baja |
| toggles checkbox on click | Unit | Cambia estado al hacer click |
| shows budget inputs when selected | Unit | Inputs visibles solo si checkbox activo |
| hides budget inputs when unselected | Unit | Inputs ocultos si checkbox inactivo |
| validates budget min < max | Unit | Muestra error si max < min |
| calls onBudgetChange with correct values | Unit | Ejecuta callback al cambiar presupuesto |
| displays priority badge with correct color | Unit | Badge rojo (Alta), amarillo (Media), gris (Baja) |
| shows tooltip on role hover | Unit | Tooltip con descripción del rol visible al hover |

**Casos Detallados:**

```markdown
1. **checkbox checked by default if Esencial**
   - Render <NecesidadItem necesidad={mockNecesidad} isSelected={true} />
   - Assert: checkbox.checked = true
   - Assert: badge "Alta" con color rojo

2. **shows budget inputs when selected**
   - Render con isSelected={true}
   - Assert: input presupuestoMin visible con value=500
   - Assert: input presupuestoMax visible con value=1500

3. **validates budget min < max**
   - Render con isSelected={true}
   - Change presupuestoMin to 2000
   - Change presupuestoMax to 1000
   - Assert: error message "El máximo debe ser mayor que el mínimo"
   - Assert: input tiene border-red-500

4. **shows tooltip on role hover**
   - Render necesidad
   - Hover sobre icono de info junto a rol
   - Assert: tooltip con "Profesional que guía el proceso creativo" visible
```

---

#### BudgetSummary.test.tsx

**Archivo:** `__tests__/components/BudgetSummary.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| displays min and max totals | Unit | Muestra totales formateados correctamente |
| shows selected count | Unit | "X de Y necesidades seleccionadas" |
| calculates average budget | Unit | Promedio = (min + max) / 2 |
| updates totals in real-time | Unit | Recalcula al cambiar props |
| shows progress bar with correct percentage | Unit | Barra = (selected / total) * 100% |
| displays warning if no selections | Unit | Mensaje "Debe seleccionar al menos una" si count=0 |
| animates number changes | Unit | Count-up animation al cambiar totales |

**Casos Detallados:**

```markdown
1. **displays min and max totals**
   - Render <BudgetSummary minTotal={3000} maxTotal={8000} selectedCount={5} totalCount={8} />
   - Assert: "3,000 EUR" visible
   - Assert: "8,000 EUR" visible

2. **calculates average budget**
   - Render con min=3000, max=8000
   - Assert: "~5,500 EUR" (promedio) visible

3. **shows progress bar with correct percentage**
   - Render con selectedCount={5}, totalCount={8}
   - Assert: progress bar width="62.5%"

4. **displays warning if no selections**
   - Render con selectedCount={0}
   - Assert: mensaje "Debe seleccionar al menos una necesidad" visible
   - Assert: icono de warning visible
```

---

#### NuevoProyectoPage.test.tsx (Integration Component)

**Archivo:** `__tests__/components/NuevoProyectoPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders wizard on correct step | Integration | Muestra paso correcto según URL |
| handles step navigation | Integration | Navega entre pasos sin perder datos |
| preserves wizard state on back | Integration | Datos persisten al volver atrás |
| validates before advancing to next step | Integration | No avanza si validación falla |
| shows confirmation page before submit | Integration | Muestra resumen en paso 3 |
| redirects after successful submission | Integration | Navega a /crowdsourcing/mis-necesidades tras éxito |

### 4.2 Hooks

#### useTemplates.test.ts

**Archivo:** `__tests__/hooks/useTemplates.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches templates successfully | Unit | Retorna lista de templates |
| sets isLoading to true initially | Unit | isLoading=true mientras carga |
| sets isLoading to false after fetch | Unit | isLoading=false al completar |
| handles error state | Unit | error set si API falla |
| caches data for subsequent calls | Unit | No re-fetch si data en cache |

**Setup:**
```typescript
const wrapper = ({ children }) => (
  <QueryClientProvider client={testQueryClient}>
    {children}
  </QueryClientProvider>
);

const { result } = renderHook(() => useTemplates(), { wrapper });
```

**Casos Detallados:**

```markdown
1. **fetches templates successfully**
   - Setup MSW handler con mockTemplatesList
   - renderHook(() => useTemplates())
   - await waitFor(() => expect(result.current.isSuccess).toBe(true))
   - Assert: result.current.data.length === 3
   - Assert: data[0].nombre === 'Producción de EP'

2. **handles error state**
   - Setup MSW handler que retorna 500 error
   - renderHook(() => useTemplates())
   - await waitFor(() => expect(result.current.isError).toBe(true))
   - Assert: result.current.error !== null

3. **caches data for subsequent calls**
   - renderHook 1: cargar datos
   - await waitFor isSuccess
   - renderHook 2: usar mismo query client
   - Assert: no nueva request (mock llamado solo 1 vez)
```

---

#### useTemplateDetail.test.ts

**Archivo:** `__tests__/hooks/useTemplateDetail.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches template by id successfully | Unit | Retorna template con necesidades |
| handles 404 when template not found | Unit | error.status === 404 si no existe |
| returns null when id is empty | Unit | No hace fetch si id es vacío |
| includes necesidades in response | Unit | data.necesidades tiene items |
| includes resumen with budget totals | Unit | data.resumen con precioMinTotal/Max |

**Casos Detallados:**

```markdown
1. **fetches template by id successfully**
   - Setup MSW handler con mockTemplateDetail
   - renderHook(() => useTemplateDetail('3fa85f64-5717-4562-b3fc-2c963f66afa6'))
   - await waitFor isSuccess
   - Assert: data.nombre === 'Producción de EP'
   - Assert: data.necesidades.length === 3

2. **handles 404 when template not found**
   - Setup MSW handler que retorna 404 para id específico
   - renderHook(() => useTemplateDetail('404-not-found'))
   - await waitFor isError
   - Assert: error.response.status === 404
   - Assert: error mensaje === 'Plantilla no encontrada'

3. **returns null when id is empty**
   - renderHook(() => useTemplateDetail(''))
   - Assert: result.current.data === undefined
   - Assert: fetch no ejecutado (MSW no llamado)
```

---

#### useGenerarNecesidades.test.ts

**Archivo:** `__tests__/hooks/useGenerarNecesidades.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| generates necesidades successfully | Unit | POST exitoso retorna IDs generados |
| invalidates templates query on success | Unit | Invalida cache de templates |
| handles validation error (no selections) | Unit | Error 400 si necesidadesSeleccionadas vacío |
| handles validation error (max < min) | Unit | Error 400 si presupuesto inválido |
| calls onSuccess callback | Unit | Ejecuta callback con result |
| sets isPending to true during mutation | Unit | isPending=true mientras POST |

**Casos Detallados:**

```markdown
1. **generates necesidades successfully**
   - Setup MSW handler para POST
   - const { mutate } = useGenerarNecesidades()
   - mutate({ proyectoArtisticoId: 'xxx', necesidadesSeleccionadas: [...] })
   - await waitFor isPending=false
   - Assert: result.data.necesidadesCreadas === 3
   - Assert: result.data.necesidadIds.length === 3

2. **invalidates templates query on success**
   - Setup query client con spy
   - mutate con datos válidos
   - await waitFor success
   - Assert: queryClient.invalidateQueries(['crowdsourcing', 'templates']) llamado

3. **handles validation error (no selections)**
   - mutate({ proyectoArtisticoId: 'xxx', necesidadesSeleccionadas: [] })
   - await waitFor isError
   - Assert: error.response.status === 400
   - Assert: error mensaje === 'Debe seleccionar al menos una necesidad'
```

---

#### useWizardState.test.ts

**Archivo:** `__tests__/hooks/useWizardState.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| initializes with step 1 | Unit | currentStep === 1 por defecto |
| advances to next step | Unit | nextStep() incrementa currentStep |
| goes back to previous step | Unit | prevStep() decrementa currentStep |
| preserves selected necesidades | Unit | State persiste al cambiar pasos |
| calculates total budget correctly | Unit | Suma min/max de necesidades seleccionadas |
| toggles necesidad selection | Unit | toggleNecesidad agrega/quita de array |
| updates budget for necesidad | Unit | updateBudget cambia min/max de necesidad |
| validates at least 1 selected | Unit | canAdvance=false si selectedCount=0 |

**Casos Detallados:**

```markdown
1. **initializes with step 1**
   - const { result } = renderHook(() => useWizardState())
   - Assert: result.current.currentStep === 1
   - Assert: result.current.selectedNecesidades === []

2. **advances to next step**
   - renderHook
   - act(() => result.current.nextStep())
   - Assert: result.current.currentStep === 2

3. **preserves selected necesidades**
   - act(() => result.current.toggleNecesidad('id1'))
   - act(() => result.current.nextStep())
   - Assert: result.current.selectedNecesidades incluye 'id1'
   - act(() => result.current.prevStep())
   - Assert: selected sigue incluyendo 'id1' (persiste)

4. **calculates total budget correctly**
   - act(() => result.current.toggleNecesidad('id1'))
   - act(() => result.current.updateBudget('id1', 500, 1500))
   - act(() => result.current.toggleNecesidad('id2'))
   - act(() => result.current.updateBudget('id2', 800, 2000))
   - Assert: result.current.totalMinBudget === 1300
   - Assert: result.current.totalMaxBudget === 3500
```

### 4.3 Services

#### crowdsourcing.service.test.ts

**Archivo:** `__tests__/services/crowdsourcing.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getTemplates returns list | Unit | Retorna array de templates |
| getTemplateById returns template | Unit | Retorna template con ID correcto |
| generarNecesidades sends correct request | Unit | POST con body correcto |
| handles API error 500 | Unit | Lanza error en fallo de servidor |

**Setup:**
```typescript
beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

**Casos Detallados:**

```markdown
1. **getTemplates returns list**
   - await crowdsourcingService.getTemplates()
   - Assert: result.items.length === 3
   - Assert: result.items[0].nombre === 'Producción de EP'

2. **getTemplateById returns template**
   - await crowdsourcingService.getTemplateById('3fa85f64-5717-4562-b3fc-2c963f66afa6')
   - Assert: result.nombre === 'Producción de EP'
   - Assert: result.necesidades.length === 3

3. **generarNecesidades sends correct request**
   - const payload = { proyectoArtisticoId: 'xxx', necesidadesSeleccionadas: [...] }
   - await crowdsourcingService.generarNecesidades('templateId', payload)
   - Assert: MSW interceptó POST a /templates/{id}/generar
   - Assert: request body === payload

4. **handles API error 500**
   - Setup MSW handler que retorna 500
   - await expect(crowdsourcingService.getTemplates()).rejects.toThrow()
```

## 5. Integration Tests

### 5.1 wizard-flow.test.tsx

**Archivo:** `__tests__/integration/wizard-flow.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| completes full wizard flow | Integration | Paso 1 → 2 → 3 → submit exitoso |
| preserves data through navigation | Integration | Datos persisten al ir atrás y adelante |

**Casos Detallados:**

```markdown
1. **completes full wizard flow**
   - Render <NuevoProyectoPage />
   - Step 1: Click en template "Producción de EP"
   - Assert: navegó a step=2
   - Step 2: Toggle 3 checkboxes
   - Step 2: Cambiar presupuesto de necesidad 1
   - Click "Siguiente"
   - Assert: navegó a step=3
   - Step 3: Verify resumen muestra 3 necesidades
   - Click "Confirmar y publicar"
   - await waitFor: POST exitoso
   - Assert: navegó a /crowdsourcing/mis-necesidades
   - Assert: toast "3 necesidades publicadas" visible

2. **preserves data through navigation**
   - Step 1: Select template
   - Step 2: Toggle necesidad A
   - Click "Siguiente" → Step 3
   - Click "Atras" → Step 2
   - Assert: checkbox de necesidad A sigue checked
   - Assert: presupuestos preservados
```

### 5.2 budget-calculations.test.tsx

**Archivo:** `__tests__/integration/budget-calculations.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| calculates total budget correctly | Integration | Suma min/max de necesidades seleccionadas |
| updates summary in real-time | Integration | BudgetSummary se actualiza al toggle checkbox |

**Casos Detallados:**

```markdown
1. **calculates total budget correctly**
   - Render wizard en step 2
   - Toggle necesidad 1 (min=500, max=1500)
   - Assert: BudgetSummary muestra "500 - 1,500 EUR"
   - Toggle necesidad 2 (min=800, max=2000)
   - Assert: BudgetSummary muestra "1,300 - 3,500 EUR"
   - Cambiar presupuesto de necesidad 1 a (600, 1600)
   - Assert: BudgetSummary muestra "1,400 - 3,600 EUR"

2. **updates summary in real-time**
   - Render step 2
   - Assert: selectedCount === 0
   - Toggle necesidad → Assert: selectedCount === 1 (sin delay)
   - Untoggle necesidad → Assert: selectedCount === 0 (inmediato)
```

### 5.3 navigation.test.tsx

**Archivo:** `__tests__/integration/navigation.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| navigates forward and backward | Integration | Botones prev/next funcionan correctamente |
| prevents advancing if validation fails | Integration | No avanza si 0 necesidades seleccionadas |

**Casos Detallados:**

```markdown
1. **navigates forward and backward**
   - Render wizard en step 1
   - Click template → navega a step 2
   - Assert: currentStep === 2
   - Click "Atras" → navega a step 1
   - Assert: currentStep === 1
   - Click mismo template → vuelve a step 2
   - Toggle al menos 1 necesidad
   - Click "Siguiente" → navega a step 3
   - Assert: currentStep === 3

2. **prevents advancing if validation fails**
   - Render step 2 con 0 necesidades seleccionadas
   - Assert: boton "Siguiente" disabled
   - Hover sobre boton → tooltip "Debe seleccionar al menos una"
   - Toggle 1 necesidad
   - Assert: boton "Siguiente" enabled
```

### 5.4 validation.test.tsx

**Archivo:** `__tests__/integration/validation.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| validates budget min < max | Integration | Error si max < min en input |
| requires at least 1 selection | Integration | No permite submit si selectedCount=0 |

**Casos Detallados:**

```markdown
1. **validates budget min < max**
   - Render step 2 con necesidad seleccionada
   - Change presupuestoMin to 2000
   - Change presupuestoMax to 1000
   - Assert: input con border-red-500
   - Assert: mensaje "El máximo debe ser mayor que el mínimo" visible
   - Click "Siguiente" → no avanza (boton disabled)
   - Change presupuestoMax to 2500
   - Assert: error desaparece
   - Click "Siguiente" → avanza normalmente

2. **requires at least 1 selection**
   - Render step 2
   - Toggle todas las necesidades OFF
   - Assert: boton "Siguiente" disabled
   - Assert: warning "Debe seleccionar al menos una" visible
   - Toggle 1 necesidad ON
   - Assert: boton enabled
   - Assert: warning desaparece
```

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| TemplateCard.tsx | 90% | 95% | 85% |
| TemplateGallery.tsx | 85% | 90% | 80% |
| WizardStepper.tsx | 95% | 100% | 90% |
| NecesidadItem.tsx | 85% | 90% | 80% |
| NecesidadList.tsx | 80% | 85% | 75% |
| BudgetSummary.tsx | 90% | 95% | 85% |
| RolProfesionalTooltip.tsx | 85% | 90% | 80% |
| ConfirmationSummary.tsx | 85% | 90% | 80% |
| NuevoProyectoPage.tsx | 80% | 85% | 75% |
| useTemplates.ts | 95% | 100% | 90% |
| useTemplateDetail.ts | 95% | 100% | 90% |
| useGenerarNecesidades.ts | 90% | 95% | 85% |
| useWizardState.ts | 90% | 95% | 85% |
| crowdsourcing.service.ts | 95% | 100% | 90% |

**Meta Global:** 85% en todas las metricas

## 7. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de crowdsourcing feature
npm run test -- --filter=crowdsourcing

# Watch mode
npm run test:watch

# Ejecutar solo integration tests
npm run test -- __tests__/integration

# Ejecutar test específico
npm run test -- TemplateCard.test.tsx
```

## 8. CI/CD Integration

```yaml
# .github/workflows/test.yml
name: Frontend Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        working-directory: ./src/web
        run: npm ci

      - name: Run tests with coverage
        working-directory: ./src/web
        run: npm run test:coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/web/coverage/coverage-final.json
          flags: frontend-crowdsourcing

      - name: Check coverage threshold
        working-directory: ./src/web
        run: |
          COVERAGE=$(jq '.total.lines.pct' coverage/coverage-summary.json)
          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "Coverage $COVERAGE% is below 80% threshold"
            exit 1
          fi
```

## 9. Checklist

- [ ] Mocks definidos para API (`crowdsourcing.mock.ts`)
- [ ] MSW handlers configurados (`handlers.ts`)
- [ ] Test utilities con providers (`test-utils.tsx`)
- [ ] Tests de componentes principales (9 archivos)
- [ ] Tests de hooks custom (4 archivos)
- [ ] Tests de services (1 archivo)
- [ ] Integration tests (4 archivos)
- [ ] Cobertura 85%+
- [ ] Tests pasan en CI
- [ ] Documentación de casos edge en cada test
- [ ] Validaciones cubiertas (budget, selections)
- [ ] Navegación entre pasos testeada
- [ ] Error states cubiertos
- [ ] Loading states cubiertos
- [ ] Accessibility (aria labels) validados

## 10. Notas Adicionales

### 10.1 Testing Best Practices

1. **Nomenclatura AAA**: Arrange, Act, Assert en cada test
2. **Un concepto por test**: Cada test valida un solo comportamiento
3. **Tests descriptivos**: Nombres explican qué se testea y resultado esperado
4. **No hardcodear IDs**: Usar mocks y factories para datos
5. **Cleanup automático**: MSW resetea handlers después de cada test

### 10.2 Cobertura de Edge Cases

| Edge Case | Archivo de Test | Descripcion |
|-----------|-----------------|-------------|
| Budget max < min | NecesidadItem.test.tsx | Validación de presupuesto inválido |
| 0 necesidades seleccionadas | validation.test.tsx | No permite avanzar sin selecciones |
| Template 404 | useTemplateDetail.test.ts | Manejo de template inexistente |
| API 500 error | crowdsourcing.service.test.ts | Error de servidor |
| Navegación sin perder datos | wizard-flow.test.tsx | State preservation |
| Tooltips accesibles | NecesidadItem.test.tsx | Aria labels y keyboard navigation |

### 10.3 Performance Testing

**No incluido en MVP**, pero considerar para futuro:
- Rendering performance con 50+ necesidades
- Debounce efectivo en budget inputs
- Lazy loading de templates
- Virtual scrolling en listas largas

### 10.4 E2E Testing (Playwright)

**Fuera de scope de este plan** (este plan cubre unit + integration con Vitest).

Para E2E completo usar Playwright:
- Full wizard flow con API real
- Cross-browser testing
- Visual regression testing
- Mobile responsive testing

---

**Archivo creado:** `C:\Repos\WePlay_Rises\plans\cs-templates-guia\frontend-landing\test-strategy.md`
**Total tests planificados:** 36
**Cobertura objetivo:** 85%
**Stack:** Vitest + Testing Library + MSW + TanStack Query
