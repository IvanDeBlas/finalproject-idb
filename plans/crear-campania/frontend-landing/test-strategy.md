# Estrategia de Testing: Crear Campania (Landing)

**Fecha:** 2026-02-12
**Feature:** crear-campania
**Target:** src/web (Vite + React Landing)
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 18 | 85% |
| Integration Tests | 8 | 75% |
| Total | 26 | 80%+ |

**Tiempo estimado ejecucion:** < 45 segundos

**Herramientas:**
- Vitest (test runner)
- @testing-library/react (component testing)
- @testing-library/user-event (user interactions)
- @testing-library/react-hooks (hook testing)
- msw (API mocking)
- @tanstack/react-query (query testing utilities)

## 2. Estructura de Tests

```
src/web/src/features/campanias/
├── __tests__/
│   ├── components/
│   │   ├── CampaniaCard.test.tsx
│   │   ├── CampaniaList.test.tsx
│   │   ├── CampaniaDetail.test.tsx
│   │   ├── ProgressBar.test.tsx
│   │   └── RewardCard.test.tsx
│   ├── hooks/
│   │   ├── useCampanias.test.ts
│   │   ├── useCampania.test.ts
│   │   └── useMisCampanias.test.ts
│   └── infrastructure/
│       ├── campania.service.test.ts
│       └── mappers.test.ts
├── __mocks__/
│   ├── campanias.mock.ts
│   ├── handlers.ts
│   └── test-utils.tsx
└── vitest.setup.ts
```

## 3. Configuracion Inicial

### 3.1 Dependencias a Instalar

```bash
npm install -D vitest @testing-library/react @testing-library/user-event @testing-library/react-hooks @vitest/ui jsdom msw
```

### 3.2 Configuracion Vitest

**Archivo:** `src/web/vite.config.ts`

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@shared': path.resolve(__dirname, '../shared'),
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/features/campanias/__tests__/vitest.setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: ['node_modules/', 'src/features/campanias/__tests__/', 'src/features/campanias/__mocks__/'],
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 80,
        statements: 80,
      },
    },
  },
})
```

### 3.3 Setup File

**Archivo:** `src/features/campanias/__tests__/vitest.setup.ts`

```typescript
import '@testing-library/jest-dom'
import { cleanup } from '@testing-library/react'
import { afterEach, beforeAll, afterAll } from 'vitest'
import { server } from '../__mocks__/handlers'

// Cleanup after each test
afterEach(() => {
  cleanup()
})

// MSW Setup
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }))
afterEach(() => server.resetHandlers())
afterAll(() => server.close())
```

## 4. Mocks y Fixtures

### 4.1 Mock Data

**Archivo:** `__mocks__/campanias.mock.ts`

```typescript
import type { Campania } from '../domain'
import type { CampaniaDto } from '../infrastructure/dtos'

export const mockCampania: Campania = {
  id: '550e8400-e29b-41d4-a716-446655440000',
  artistaId: '660e8400-e29b-41d4-a716-446655440001',
  titulo: 'Mi Primer Album - Rock Alternativo',
  descripcion: 'Proyecto musical que necesita tu apoyo para grabar nuestro primer album de rock alternativo con influencias indie',
  importeObjetivo: 5000,
  importePledgedActual: 1250.5,
  fechaInicio: new Date('2026-03-01T00:00:00Z'),
  fechaFin: new Date('2026-04-30T23:59:59Z'),
  estado: 'activa',
  imagenUrl: 'https://example.com/imagen.jpg',
  createdAt: new Date('2026-02-12T10:30:00Z'),
  updatedAt: new Date('2026-02-12T10:30:00Z'),
}

export const mockCampaniaBorrador: Campania = {
  ...mockCampania,
  id: '550e8400-e29b-41d4-a716-446655440002',
  titulo: 'Campania en Borrador',
  estado: 'borrador',
  importePledgedActual: 0,
}

export const mockCampaniaFinalizada: Campania = {
  ...mockCampania,
  id: '550e8400-e29b-41d4-a716-446655440003',
  titulo: 'Campania Finalizada Exitosa',
  estado: 'finalizada',
  importePledgedActual: 6000,
  fechaFin: new Date('2026-01-30T23:59:59Z'),
}

export const mockCampaniaList: Campania[] = [
  mockCampania,
  {
    ...mockCampania,
    id: '550e8400-e29b-41d4-a716-446655440004',
    titulo: 'Segundo Album - Jazz Fusion',
    importeObjetivo: 8000,
    importePledgedActual: 2400,
    imagenUrl: 'https://example.com/imagen2.jpg',
  },
  {
    ...mockCampania,
    id: '550e8400-e29b-41d4-a716-446655440005',
    titulo: 'Tour Nacional 2026',
    importeObjetivo: 15000,
    importePledgedActual: 500,
    imagenUrl: undefined, // Test sin imagen
  },
]

export const mockCampaniaDto: CampaniaDto = {
  id: '550e8400-e29b-41d4-a716-446655440000',
  artistaId: '660e8400-e29b-41d4-a716-446655440001',
  titulo: 'Mi Primer Album - Rock Alternativo',
  descripcionCorta: 'Proyecto musical que necesita tu apoyo',
  importeObjetivo: 5000,
  importePledgedActual: 1250.5,
  fechaInicio: '2026-03-01T00:00:00Z',
  fechaFin: '2026-04-30T23:59:59Z',
  estadoCampaniaId: 2, // Publicada
  imagenPrincipalUrl: 'https://example.com/imagen.jpg',
  fechaCreacion: '2026-02-12T10:30:00Z',
  fechaActualizacion: '2026-02-12T10:30:00Z',
}

export const mockReward = {
  id: '770e8400-e29b-41d4-a716-446655440000',
  campaniaId: mockCampania.id,
  nombre: 'Acceso Early Bird',
  descripcion: 'Copia digital del album antes del lanzamiento oficial',
  importeMinimo: 15,
  stockLimitado: 50,
  stockDisponible: 30,
}

export const mockRewardSoldOut = {
  ...mockReward,
  id: '770e8400-e29b-41d4-a716-446655440001',
  nombre: 'Meet & Greet VIP',
  importeMinimo: 150,
  stockLimitado: 10,
  stockDisponible: 0,
}

export const mockRewardList = [mockReward, mockRewardSoldOut]
```

### 4.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw'
import { setupServer } from 'msw/node'
import { mockCampaniaDto, mockCampaniaList } from './campanias.mock'
import { mapCampaniaDtoToDomain } from '../infrastructure/mappers'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

export const handlers = [
  // GET /api/campanias - Lista todas las campanias
  http.get(`${API_URL}/campanias`, () => {
    return HttpResponse.json({
      data: mockCampaniaList.map(c => ({
        ...mockCampaniaDto,
        id: c.id,
        titulo: c.titulo,
        importeObjetivo: c.importeObjetivo,
        importePledgedActual: c.importePledgedActual,
      })),
      messages: [{ message: 'Campanias encontradas', errorCode: '0000' }],
    })
  }),

  // GET /api/campanias/:id - Obtener campania por ID
  http.get(`${API_URL}/campanias/:id`, ({ params }) => {
    const { id } = params
    const campania = mockCampaniaList.find(c => c.id === id)

    if (!campania) {
      return HttpResponse.json(
        {
          data: null,
          messages: [{ message: 'Campania no encontrada', errorCode: '2003' }],
        },
        { status: 404 }
      )
    }

    return HttpResponse.json({
      data: mockCampaniaDto,
      messages: [{ message: 'Campania encontrada', errorCode: '0000' }],
    })
  }),

  // POST /api/campanias - Crear campania
  http.post(`${API_URL}/campanias`, async ({ request }) => {
    const body = await request.json()
    return HttpResponse.json({
      data: {
        ...mockCampaniaDto,
        id: crypto.randomUUID(),
        titulo: body.titulo,
        estadoCampaniaId: 1, // Borrador
      },
      messages: [{ message: 'Campania creada correctamente', errorCode: '0001' }],
    })
  }),

  // PUT /api/campanias/:id - Actualizar campania
  http.put(`${API_URL}/campanias/:id`, async ({ request }) => {
    return HttpResponse.json({
      data: true,
      messages: [{ message: 'Campania actualizada correctamente', errorCode: '0002' }],
    })
  }),

  // POST /api/campanias/:id/publicar - Publicar campania
  http.post(`${API_URL}/campanias/:id/publicar`, ({ params }) => {
    const { id } = params
    return HttpResponse.json({
      data: {
        id,
        estadoCampaniaId: 2,
        fechaPublicacion: new Date().toISOString(),
        message: 'Campania publicada exitosamente',
      },
      messages: [{ message: 'Campania publicada exitosamente', errorCode: '0000' }],
    })
  }),
]

export const server = setupServer(...handlers)
```

### 4.3 Test Utilities

**Archivo:** `__mocks__/test-utils.tsx`

```typescript
import { ReactElement, ReactNode } from 'react'
import { render, RenderOptions } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'

// Query client con config para tests (sin retries, sin cache)
const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        staleTime: 0,
        gcTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  })

interface AllProvidersProps {
  children: ReactNode
}

const AllProviders = ({ children }: AllProvidersProps) => {
  const queryClient = createTestQueryClient()

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>{children}</BrowserRouter>
    </QueryClientProvider>
  )
}

const customRender = (ui: ReactElement, options?: Omit<RenderOptions, 'wrapper'>) =>
  render(ui, { wrapper: AllProviders, ...options })

export * from '@testing-library/react'
export { customRender as render, createTestQueryClient }
```

## 5. Tests por Modulo

### 5.1 Components

#### CampaniaCard.test.tsx

**Archivo:** `__tests__/components/CampaniaCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders correctly | Unit | Renderiza sin errores con props minimas |
| displays title | Unit | Muestra titulo de la campania |
| displays funding amount | Unit | Muestra importe actual y objetivo |
| calculates percentage correctly | Unit | Calcula porcentaje correcto (25% en mock) |
| calculates days remaining | Unit | Calcula dias restantes correctamente |
| displays image when provided | Unit | Muestra imagen si imagenUrl existe |
| hides image when not provided | Unit | No renderiza imagen si imagenUrl es undefined |
| limits percentage to 100 | Unit | Muestra maximo 100% si se supera objetivo |
| renders link to detail | Integration | Link redirige a /campanias/:id |
| truncates long title | Unit | Trunca titulo largo con line-clamp-2 |

**Casos Detallados:**

```typescript
import { render, screen } from '../__mocks__/test-utils'
import { CampaniaCard } from '../../presentation/components/CampaniaCard'
import { mockCampania } from '../__mocks__/campanias.mock'

describe('CampaniaCard', () => {
  it('renders correctly', () => {
    render(<CampaniaCard campania={mockCampania} />)
    expect(screen.getByRole('img')).toBeInTheDocument()
  })

  it('displays title', () => {
    render(<CampaniaCard campania={mockCampania} />)
    expect(screen.getByText(mockCampania.titulo)).toBeInTheDocument()
  })

  it('calculates percentage correctly', () => {
    // 1250.5 / 5000 * 100 = 25%
    render(<CampaniaCard campania={mockCampania} />)
    expect(screen.getByText('25% financiado')).toBeInTheDocument()
  })

  it('limits percentage to 100', () => {
    const campaniaOverfunded = {
      ...mockCampania,
      importePledgedActual: 8000, // Supera objetivo de 5000
    }
    render(<CampaniaCard campania={campaniaOverfunded} />)
    expect(screen.getByText('100% financiado')).toBeInTheDocument()
  })

  it('hides image when not provided', () => {
    const campaniaNoImage = { ...mockCampania, imagenUrl: undefined }
    render(<CampaniaCard campania={campaniaNoImage} />)
    expect(screen.queryByRole('img')).not.toBeInTheDocument()
  })
})
```

#### CampaniaList.test.tsx

**Archivo:** `__tests__/components/CampaniaList.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders list of campanias | Unit | Renderiza array de campanias |
| displays loading state | Unit | Muestra skeletons cuando isLoading=true |
| displays empty state | Unit | Muestra mensaje cuando campanias array vacio |
| renders correct number of cards | Unit | Renderiza N cards segun array.length |
| applies grid layout | Unit | Aplica clases CSS de grid responsive |
| shows 6 skeletons on loading | Unit | Muestra exactamente 6 skeletons |

**Casos Detallados:**

```typescript
import { render, screen } from '../__mocks__/test-utils'
import { CampaniaList } from '../../presentation/components/CampaniaList'
import { mockCampaniaList } from '../__mocks__/campanias.mock'

describe('CampaniaList', () => {
  it('renders list of campanias', () => {
    render(<CampaniaList campanias={mockCampaniaList} />)
    expect(screen.getByText(mockCampaniaList[0].titulo)).toBeInTheDocument()
  })

  it('displays loading state', () => {
    render(<CampaniaList campanias={[]} isLoading={true} />)
    const skeletons = screen.getAllByTestId('skeleton')
    expect(skeletons).toHaveLength(6)
  })

  it('displays empty state', () => {
    render(<CampaniaList campanias={[]} isLoading={false} />)
    expect(screen.getByText('No hay campanias disponibles')).toBeInTheDocument()
  })

  it('renders correct number of cards', () => {
    render(<CampaniaList campanias={mockCampaniaList} />)
    const cards = screen.getAllByRole('link', { name: /ver campania/i })
    expect(cards).toHaveLength(mockCampaniaList.length)
  })
})
```

#### CampaniaDetail.test.tsx

**Archivo:** `__tests__/components/CampaniaDetail.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders full detail | Integration | Renderiza todos los campos completos |
| displays tabs correctly | Unit | Renderiza tabs (Descripcion, Rewards, Actualizaciones) |
| shows progress bar | Unit | Muestra ProgressBar component |
| displays reward list | Integration | Muestra lista de rewards |
| handles video embed | Unit | Renderiza iframe YouTube/Vimeo si URL existe |
| displays artista info | Integration | Muestra info del artista |
| shows back CTA | Unit | Muestra boton "Apoyar Proyecto" |
| disables CTA when finalizada | Unit | Deshabilita boton si estado=finalizada |
| switches between tabs | Integration | Cambia contenido al hacer clic en tabs |

#### ProgressBar.test.tsx

**Archivo:** `__tests__/components/ProgressBar.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with correct percentage | Unit | Renderiza con aria-valuenow correcto |
| applies success color when 100% | Unit | Aplica clase verde cuando alcanza meta |
| applies warning color when < 50% | Unit | Aplica clase amarilla cuando < 50% |
| applies danger color when < 20% | Unit | Aplica clase roja cuando < 20% |
| handles 0% correctly | Unit | Renderiza 0% sin errores |
| handles values > 100% | Unit | Limita a 100% maximo |

#### RewardCard.test.tsx

**Archivo:** `__tests__/components/RewardCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders reward info | Unit | Muestra nombre, descripcion, precio |
| displays available stock | Unit | Muestra "30 de 50 disponibles" |
| shows sold out state | Unit | Muestra "Agotado" cuando stock=0 |
| disables select when sold out | Unit | Deshabilita boton cuando agotado |
| formats price correctly | Unit | Formatea precio con separador miles y EUR |
| renders unlimited stock | Unit | No muestra stock si no es limitado |

### 5.2 Hooks

#### useCampanias.test.ts

**Archivo:** `__tests__/hooks/useCampanias.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches campanias successfully | Integration | Retorna lista de campanias |
| handles loading state | Unit | isLoading=true inicialmente |
| handles error state | Unit | error cuando API retorna 500 |
| returns empty array on success | Unit | data=[] cuando backend retorna array vacio |
| uses correct query key | Unit | queryKey=['campanias'] |

**Setup:**

```typescript
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useCampanias } from '../../application/useCampanias'
import { createTestQueryClient } from '../__mocks__/test-utils'

describe('useCampanias', () => {
  const wrapper = ({ children }) => {
    const queryClient = createTestQueryClient()
    return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  }

  it('fetches campanias successfully', async () => {
    const { result } = renderHook(() => useCampanias(), { wrapper })

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toHaveLength(3)
  })

  it('handles loading state', () => {
    const { result } = renderHook(() => useCampanias(), { wrapper })
    expect(result.current.isLoading).toBe(true)
  })
})
```

#### useCampania.test.ts

**Archivo:** `__tests__/hooks/useCampania.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches single campania | Integration | Retorna campania por ID |
| returns null when not found | Integration | data=null cuando API retorna 404 |
| is disabled when id is empty | Unit | enabled=false cuando id='' |
| invalidates on mutation | Integration | Invalida query al actualizar |

#### useMisCampanias.test.ts

**Archivo:** `__tests__/hooks/useMisCampanias.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches artista campanias | Integration | Retorna campanias del artista |
| is disabled when artistaId empty | Unit | enabled=false cuando artistaId='' |
| filters by estado | Integration | Respeta filtro estadoCampaniaId |

### 5.3 Services

#### campania.service.test.ts

**Archivo:** `__tests__/infrastructure/campania.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getAll returns list | Unit | Retorna lista de campanias |
| getById returns campania | Unit | Retorna campania por ID |
| getById returns null on 404 | Unit | Retorna null cuando no existe |
| create sends correct data | Unit | POST con body correcto |
| update sends correct data | Unit | PUT con body correcto |
| publicar calls correct endpoint | Unit | POST /campanias/:id/publicar |
| handles API error | Unit | Lanza error en fallo 500 |

**Setup:**

```typescript
import { beforeAll, afterEach, afterAll, describe, it, expect } from 'vitest'
import { server } from '../__mocks__/handlers'
import { campaniaService } from '../../infrastructure/campania.service'

describe('CampaniaService', () => {
  beforeAll(() => server.listen())
  afterEach(() => server.resetHandlers())
  afterAll(() => server.close())

  it('getAll returns list', async () => {
    const result = await campaniaService.getAll()
    expect(result).toBeInstanceOf(Array)
    expect(result.length).toBeGreaterThan(0)
  })

  it('getById returns null on 404', async () => {
    const result = await campaniaService.getById('invalid-id')
    expect(result).toBeNull()
  })

  it('create sends correct data', async () => {
    const newCampania = {
      titulo: 'Nueva Campania',
      descripcion: 'Descripcion test',
      importeObjetivo: 1000,
      fechaFin: new Date('2026-05-01'),
    }
    const result = await campaniaService.create(newCampania)
    expect(result.titulo).toBe(newCampania.titulo)
    expect(result.estado).toBe('borrador')
  })
})
```

#### mappers.test.ts

**Archivo:** `__tests__/infrastructure/mappers.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| maps DTO to domain correctly | Unit | Convierte CampaniaDto a Campania |
| maps dates to Date objects | Unit | Convierte strings ISO a Date |
| maps estadoCampaniaId to enum | Unit | Convierte 1->borrador, 2->activa |
| maps create data to DTO | Unit | Convierte CreateCampaniaData a request DTO |

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| CampaniaCard.tsx | 90% | 100% | 85% |
| CampaniaList.tsx | 95% | 100% | 90% |
| CampaniaDetail.tsx | 80% | 90% | 75% |
| ProgressBar.tsx | 95% | 100% | 90% |
| RewardCard.tsx | 90% | 100% | 85% |
| useCampanias.ts | 90% | 100% | 85% |
| useCampania.ts | 90% | 100% | 85% |
| useMisCampanias.ts | 85% | 100% | 80% |
| campania.service.ts | 95% | 100% | 90% |
| mappers.ts | 100% | 100% | 100% |

**Meta Global:** 80% en todas las metricas

## 7. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- campanias

# Watch mode
npm run test:watch

# UI mode
npm run test:ui
```

**Actualizar package.json:**

```json
{
  "scripts": {
    "test": "vitest run",
    "test:watch": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest run --coverage"
  }
}
```

## 8. CI/CD Integration

```yaml
name: Frontend Tests

on:
  push:
    branches: [master]
    paths:
      - 'src/web/**'
  pull_request:
    branches: [master]

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
          flags: frontend-landing
          fail_ci_if_error: true
```

## 9. Estrategia de Mocking

### API Mocking (MSW)

**Ventajas:**
- Intercepta requests a nivel de red (no mockea axios/fetch)
- Mismo codigo de produccion
- Handlers reutilizables entre tests

**Uso:**

```typescript
// Para test especifico que necesita error
import { server } from '../__mocks__/handlers'
import { http, HttpResponse } from 'msw'

it('handles API error', async () => {
  server.use(
    http.get('/api/campanias', () => {
      return new HttpResponse(null, { status: 500 })
    })
  )

  const { result } = renderHook(() => useCampanias(), { wrapper })
  await waitFor(() => expect(result.current.isError).toBe(true))
})
```

### Query Client Mocking

**No mockear QueryClient directamente.** Usar client real con config de testing (sin retries, sin cache).

## 10. Edge Cases Criticos

| Escenario | Componente | Test |
|-----------|------------|------|
| Campania sin imagen | CampaniaCard | No renderiza img tag |
| Porcentaje > 100% | CampaniaCard | Limita a 100% |
| Dias restantes negativos | CampaniaCard | Muestra 0 dias |
| Lista vacia | CampaniaList | Muestra empty state |
| Reward agotado | RewardCard | Deshabilita boton |
| API 404 | useCampania | Retorna null |
| API 500 | useCampanias | isError=true |
| Token expirado | Service | Lanza error 401 |

## 11. Testing Best Practices

### 11.1 Testing Library Queries (Priority)

1. **getByRole** (preferido) - Accesibilidad first
2. **getByLabelText** - Para forms
3. **getByPlaceholderText** - Para inputs
4. **getByText** - Para contenido
5. **getByTestId** - Ultimo recurso

### 11.2 User Interactions

```typescript
import userEvent from '@testing-library/user-event'

it('handles form submission', async () => {
  const user = userEvent.setup()
  render(<CampaniaForm />)

  await user.type(screen.getByLabelText('Titulo'), 'Mi Campania')
  await user.click(screen.getByRole('button', { name: /enviar/i }))

  await waitFor(() => {
    expect(screen.getByText('Campania creada')).toBeInTheDocument()
  })
})
```

### 11.3 Async Testing

```typescript
// CORRECTO - waitFor para assertions async
await waitFor(() => expect(result.current.isSuccess).toBe(true))

// INCORRECTO - No usar timeouts arbitrarios
await new Promise(resolve => setTimeout(resolve, 1000)) // EVITAR
```

### 11.4 Test Independence

Cada test DEBE ser independiente. No compartir estado entre tests.

```typescript
// CORRECTO
beforeEach(() => {
  // Setup fresh data per test
  mockData = { ...originalMock }
})

// INCORRECTO - Estado compartido
let sharedCampania = mockCampania // EVITAR
```

## 12. Checklist Pre-Commit

- [ ] Dependencias instaladas (vitest, @testing-library/react, msw)
- [ ] Configuracion vitest.config.ts creada
- [ ] Setup file con MSW configurado
- [ ] Mock data definido en campanias.mock.ts
- [ ] MSW handlers configurados
- [ ] Test utilities (render wrapper) creado
- [ ] Tests de CampaniaCard completos (10 tests)
- [ ] Tests de CampaniaList completos (6 tests)
- [ ] Tests de hooks completos (3 archivos)
- [ ] Tests de service completos (7 tests)
- [ ] Cobertura 80%+ alcanzada
- [ ] Tests pasan en < 60 segundos
- [ ] Scripts npm configurados
- [ ] CI/CD workflow configurado (opcional para MVP)

## 13. Prioridades de Implementacion

### Fase 1 - Critical Path (2 horas)
1. Configuracion inicial (vitest.config, setup file)
2. Mock data y handlers MSW
3. Test utilities
4. Tests de CampaniaCard (componente mas critico)
5. Tests de CampaniaList

### Fase 2 - Core Functionality (2 horas)
6. Tests de hooks (useCampanias, useCampania)
7. Tests de service
8. Tests de mappers

### Fase 3 - Polish (1 hora)
9. Tests de ProgressBar
10. Tests de RewardCard
11. Edge cases adicionales
12. Alcanzar 80% coverage

**Total estimado:** 5 horas

## 14. Deuda Tecnica Conocida

| Item | Impacto | Razon | Plan |
|------|---------|-------|------|
| No hay tests E2E | Medio | MVP con tiempo limitado | Agregar Playwright post-MVP |
| CampaniaDetail sin tests completos | Bajo | Componente complejo, requiere mas tiempo | Priorizar en Sprint 2 |
| No hay tests de accesibilidad | Bajo | axe-core no instalado | Agregar en hardening |
| Mock de video embed basico | Bajo | iframe mock simplificado | Mejorar si necesario |

## 15. Metricas de Exito

| Metrica | Objetivo | Actual |
|---------|----------|--------|
| Cobertura Lineas | 80% | TBD |
| Cobertura Funciones | 80% | TBD |
| Cobertura Branches | 80% | TBD |
| Tiempo Ejecucion | < 60s | TBD |
| Tests Totales | 26+ | 0 |
| Tests Fallando | 0 | TBD |
| Flaky Tests | 0 | TBD |

---

**Archivo creado:** `C:\Repos\WePlay_Rises\plans\crear-campania\frontend-landing\test-strategy.md`

**Total tests planificados:** 26 (18 unit + 8 integration)

**Cobertura objetivo:** 80%+ en todas las metricas

**Tiempo estimado implementacion:** 5 horas

**Proximos pasos:**
1. Instalar dependencias: `npm install -D vitest @testing-library/react @testing-library/user-event @testing-library/react-hooks @vitest/ui jsdom msw`
2. Configurar vitest.config.ts
3. Crear mock data y handlers MSW
4. Implementar tests segun prioridades (Fase 1 → Fase 2 → Fase 3)
5. Verificar cobertura con `npm run test:coverage`
