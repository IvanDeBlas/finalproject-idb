# Estrategia de Testing: Registro de Artista (Landing)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/web (Landing - Vite + React)
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 8 | 40% |
| Integration Tests | 5 | 40% |
| Total | 13 | 80%+ |

**Alcance Landing:** Visualización de perfil público de artista en `/artistas/{id}`. Sin formularios ni autenticación en esta app.

**Prioridad Alta:**
- Hook `useArtista` (data fetching core)
- Page component `ArtistaProfilePage` (flujo principal)
- Error states (NOT_FOUND, network errors)

## 2. Estructura de Tests

```
src/web/src/features/artistas/
├── __tests__/
│   ├── ArtistaProfilePage.test.tsx      # Integration - Full page
│   ├── components/
│   │   ├── ArtistaAvatar.test.tsx       # Unit - Avatar display
│   │   ├── ArtistaBio.test.tsx          # Unit - Bio section
│   │   ├── ArtistaStats.test.tsx        # Unit - Stats display
│   │   └── ArtistaLocation.test.tsx     # Unit - Location display
│   ├── hooks/
│   │   └── useArtista.test.ts           # Integration - Query hook
│   └── services/
│       └── artista.service.test.ts      # Unit - API service
└── __mocks__/
    ├── artista.mock.ts                   # Mock data
    └── handlers.ts                       # MSW handlers
```

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/artista.mock.ts`

```typescript
import { Artista } from '@shared/types/artista';

export const mockArtistaCompleto: Artista = {
  id: '123e4567-e89b-12d3-a456-426614174000',
  userId: 'user-123',
  nombreArtistico: 'Luna Volcánica',
  descripcion: 'Banda indie rock formada en Madrid. Fusionamos rock alternativo con electrónica experimental.',
  pais: 'España',
  ciudad: 'Madrid',
  imagenUrl: 'https://images.unsplash.com/photo-artist-123?w=800',
  fechaCreacion: '2026-01-15T10:30:00Z',
  fechaActualizacion: '2026-01-20T14:45:00Z',
};

export const mockArtistaSinDescripcion: Artista = {
  id: '223e4567-e89b-12d3-a456-426614174001',
  userId: 'user-456',
  nombreArtistico: 'DJ Pixel',
  descripcion: undefined,
  pais: undefined,
  ciudad: undefined,
  imagenUrl: undefined,
  fechaCreacion: '2026-01-10T08:00:00Z',
};

export const mockArtistaSinImagen: Artista = {
  id: '323e4567-e89b-12d3-a456-426614174002',
  userId: 'user-789',
  nombreArtistico: 'Coros del Sur',
  descripcion: 'Coro polifónico de música folclórica.',
  pais: 'Argentina',
  ciudad: 'Buenos Aires',
  imagenUrl: undefined,
  fechaCreacion: '2026-01-12T12:00:00Z',
};

export const mockArtistaList: Artista[] = [
  mockArtistaCompleto,
  mockArtistaSinDescripcion,
  mockArtistaSinImagen,
];
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from 'msw';
import { mockArtistaCompleto, mockArtistaSinImagen } from './artista.mock';
import { API_ROUTES } from '@shared/constants/api-routes';

export const artistaHandlers = [
  // GET /api/artistas/:id - Success
  rest.get(`${API_ROUTES.artistas.byId(':id')}`, (req, res, ctx) => {
    const { id } = req.params;

    // Simular diferentes casos segun ID
    if (id === '123e4567-e89b-12d3-a456-426614174000') {
      return res(
        ctx.status(200),
        ctx.json({
          data: mockArtistaCompleto,
          isSuccess: true,
        })
      );
    }

    if (id === '323e4567-e89b-12d3-a456-426614174002') {
      return res(
        ctx.status(200),
        ctx.json({
          data: mockArtistaSinImagen,
          isSuccess: true,
        })
      );
    }

    // Not Found
    if (id === 'non-existent-id') {
      return res(
        ctx.status(404),
        ctx.json({
          data: null,
          isSuccess: false,
          messages: [
            {
              message: 'Artista no encontrado',
              errorCode: 'ARTISTA_NOT_FOUND',
            },
          ],
        })
      );
    }

    // Network error simulation
    if (id === 'error-500') {
      return res(ctx.status(500));
    }

    // Default: return completo
    return res(
      ctx.status(200),
      ctx.json({
        data: mockArtistaCompleto,
        isSuccess: true,
      })
    );
  }),
];
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import { ReactElement } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';

// Query client sin retries para tests rapidos
const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        cacheTime: 0,
        staleTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  });

interface AllProvidersProps {
  children: React.ReactNode;
}

const AllProviders = ({ children }: AllProvidersProps) => {
  const testQueryClient = createTestQueryClient();

  return (
    <QueryClientProvider client={testQueryClient}>
      <BrowserRouter>{children}</BrowserRouter>
    </QueryClientProvider>
  );
};

const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllProviders, ...options });

export * from '@testing-library/react';
export { customRender as render };
```

### 3.4 MSW Setup

**Archivo:** `setupTests.ts` (Vitest config)

```typescript
import { beforeAll, afterEach, afterAll } from 'vitest';
import { setupServer } from 'msw/node';
import { artistaHandlers } from './features/artistas/__mocks__/handlers';

export const server = setupServer(...artistaHandlers);

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

## 4. Tests por Modulo

### 4.1 Services

#### artista.service.test.ts

**Archivo:** `__tests__/services/artista.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getById returns artista on success | Unit | Retorna artista completo cuando existe |
| getById returns null on 404 | Unit | Retorna null cuando ID no existe |
| getById throws error on network failure | Unit | Lanza error en fallo de red (500) |
| getById handles timeout | Unit | Maneja timeout de request |

**Casos Detallados:**

```markdown
1. **getById returns artista on success**
   - Setup: MSW handler retorna mockArtistaCompleto para ID valido
   - Call: `artistaService.getById('123e4567-e89b-12d3-a456-426614174000')`
   - Assert: Resultado es Artista con nombreArtistico 'Luna Volcánica'
   - Assert: Todos los campos opcionales presentes

2. **getById returns null on 404**
   - Setup: MSW handler retorna 404 para 'non-existent-id'
   - Call: `artistaService.getById('non-existent-id')`
   - Assert: Resultado es null
   - Assert: No lanza excepcion

3. **getById throws error on network failure**
   - Setup: MSW handler retorna 500 para 'error-500'
   - Call: `artistaService.getById('error-500')`
   - Assert: Lanza error con mensaje 'Network Error'

4. **getById handles timeout**
   - Setup: MSW handler con delay de 5000ms
   - Call: `artistaService.getById('timeout-id')` con timeout de 1000ms
   - Assert: Lanza error de timeout
```

### 4.2 Hooks

#### useArtista.test.ts

**Archivo:** `__tests__/hooks/useArtista.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns data on success | Integration | Retorna artista cuando existe |
| handles loading state | Integration | isLoading true inicialmente, false al completar |
| handles not found error | Integration | isError true cuando artista no existe |
| handles network error | Integration | error state en fallo de red |
| caches data correctly | Integration | No refetch en segundo render |
| invalidates on manual refetch | Integration | Refetch manual actualiza datos |

**Setup:**
```typescript
const wrapper = ({ children }) => (
  <QueryClientProvider client={testQueryClient}>
    {children}
  </QueryClientProvider>
);

const { result } = renderHook(() => useArtista('123'), { wrapper });
```

**Casos Detallados:**

```markdown
1. **returns data on success**
   - Render hook: `useArtista('123e4567-e89b-12d3-a456-426614174000')`
   - Assert: isLoading true inicialmente
   - Wait for: isSuccess true
   - Assert: data es mockArtistaCompleto
   - Assert: data.nombreArtistico === 'Luna Volcánica'

2. **handles loading state**
   - Render hook: `useArtista('123e4567-e89b-12d3-a456-426614174000')`
   - Assert: isLoading true inmediatamente
   - Assert: data undefined mientras loading
   - Wait for: isLoading false
   - Assert: data definido

3. **handles not found error**
   - Render hook: `useArtista('non-existent-id')`
   - Wait for: isError true
   - Assert: error.response.status === 404
   - Assert: data undefined

4. **handles network error**
   - Render hook: `useArtista('error-500')`
   - Wait for: isError true
   - Assert: error.message contiene 'Network Error'

5. **caches data correctly**
   - First render: `useArtista('123')`
   - Wait for: data loaded
   - Unmount
   - Second render: mismo ID
   - Assert: data disponible inmediatamente (cache hit)
   - Assert: No segundo request a MSW

6. **invalidates on manual refetch**
   - Render hook: `useArtista('123')`
   - Wait for: data loaded
   - Mock data change en MSW
   - Call: `result.current.refetch()`
   - Wait for: new data loaded
   - Assert: data actualizado
```

### 4.3 Components

#### ArtistaAvatar.test.tsx

**Archivo:** `__tests__/components/ArtistaAvatar.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders image when imagenUrl provided | Unit | Muestra imagen con src correcto |
| shows placeholder when no imagenUrl | Unit | Muestra icono de placeholder |
| shows initials in placeholder | Unit | Muestra iniciales del nombre artistico |
| applies correct size classes | Unit | Aplica clases de tamaño (sm, md, lg) |
| handles image load error | Unit | Fallback a placeholder en error de carga |

**Casos Detallados:**

```markdown
1. **renders image when imagenUrl provided**
   - Render: `<ArtistaAvatar artista={mockArtistaCompleto} size="md" />`
   - Assert: img tag presente
   - Assert: src === mockArtistaCompleto.imagenUrl
   - Assert: alt === mockArtistaCompleto.nombreArtistico

2. **shows placeholder when no imagenUrl**
   - Render: `<ArtistaAvatar artista={mockArtistaSinImagen} size="md" />`
   - Assert: No img tag
   - Assert: Placeholder div visible
   - Assert: Icon visible (User icon)

3. **shows initials in placeholder**
   - Render: `<ArtistaAvatar artista={mockArtistaSinImagen} size="md" />`
   - Assert: Placeholder contiene 'CS' (Coros del Sur)
   - Assert: Text centered

4. **applies correct size classes**
   - Render: `<ArtistaAvatar artista={mockArtista} size="sm" />`
   - Assert: container tiene clase 'w-12 h-12'
   - Render: size="lg"
   - Assert: container tiene clase 'w-24 h-24'

5. **handles image load error**
   - Render: `<ArtistaAvatar artista={mockArtistaCompleto} />`
   - Simulate: img.onerror event
   - Assert: Placeholder mostrado
   - Assert: img tag removido
```

#### ArtistaBio.test.tsx

**Archivo:** `__tests__/components/ArtistaBio.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders description when provided | Unit | Muestra descripcion completa |
| shows fallback message when empty | Unit | Muestra 'Sin descripción' cuando undefined |
| shows location when provided | Unit | Muestra ciudad y pais |
| hides location section when not provided | Unit | No muestra seccion de ubicacion |
| renders multiline description correctly | Unit | Respeta saltos de linea |

**Casos Detallados:**

```markdown
1. **renders description when provided**
   - Render: `<ArtistaBio artista={mockArtistaCompleto} />`
   - Assert: Texto 'Banda indie rock...' visible
   - Assert: No mensaje de fallback

2. **shows fallback message when empty**
   - Render: `<ArtistaBio artista={mockArtistaSinDescripcion} />`
   - Assert: Texto 'Este artista aún no ha agregado una descripción' visible
   - Assert: Clase text-muted-foreground aplicada

3. **shows location when provided**
   - Render: `<ArtistaBio artista={mockArtistaCompleto} />`
   - Assert: Texto 'Madrid, España' visible
   - Assert: Location icon visible

4. **hides location section when not provided**
   - Render: `<ArtistaBio artista={mockArtistaSinDescripcion} />`
   - Assert: No location section
   - Assert: No location icon

5. **renders multiline description correctly**
   - Mock: artista con descripcion multilinea
   - Render: `<ArtistaBio artista={mockArtista} />`
   - Assert: Parrafos separados visibles
   - Assert: Whitespace preservado
```

#### ArtistaStats.test.tsx

**Archivo:** `__tests__/components/ArtistaStats.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| displays stats with correct labels | Unit | Muestra labels de stats |
| formats numbers correctly | Unit | Formatea numeros con separadores |
| shows zero values as placeholders | Unit | Muestra '0' o '--' para valores vacios |
| renders responsive grid layout | Unit | Grid layout correcto |

**Casos Detallados:**

```markdown
1. **displays stats with correct labels**
   - Mock: artista con stats { campanias: 3, backers: 150, totalRecaudado: 5000 }
   - Render: `<ArtistaStats stats={mockStats} />`
   - Assert: Label 'Campañas' visible
   - Assert: Label 'Apoyos' visible
   - Assert: Label 'Recaudado' visible

2. **formats numbers correctly**
   - Mock: stats con totalRecaudado: 15000
   - Render: `<ArtistaStats stats={mockStats} />`
   - Assert: '15.000 €' visible (separador de miles)
   - Mock: backers: 1250
   - Assert: '1.250' visible

3. **shows zero values as placeholders**
   - Mock: stats con campanias: 0
   - Render: `<ArtistaStats stats={mockStats} />`
   - Assert: '0 campañas' visible
   - Assert: Message 'Aún no ha lanzado campañas' (opcional)

4. **renders responsive grid layout**
   - Render: `<ArtistaStats stats={mockStats} />`
   - Assert: Grid container presente
   - Assert: grid-cols-3 en desktop
   - Assert: grid-cols-1 en mobile
```

#### ArtistaLocation.test.tsx

**Archivo:** `__tests__/components/ArtistaLocation.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders full location (ciudad, pais) | Unit | Muestra ciudad y pais |
| renders only pais when ciudad missing | Unit | Muestra solo pais |
| renders only ciudad when pais missing | Unit | Muestra solo ciudad |
| returns null when both missing | Unit | No renderiza componente |

**Casos Detallados:**

```markdown
1. **renders full location**
   - Render: `<ArtistaLocation ciudad="Madrid" pais="España" />`
   - Assert: Texto 'Madrid, España' visible
   - Assert: MapPin icon visible

2. **renders only pais when ciudad missing**
   - Render: `<ArtistaLocation pais="España" />`
   - Assert: Texto 'España' visible
   - Assert: Sin coma

3. **renders only ciudad when pais missing**
   - Render: `<ArtistaLocation ciudad="Madrid" />`
   - Assert: Texto 'Madrid' visible

4. **returns null when both missing**
   - Render: `<ArtistaLocation />`
   - Assert: Componente no en documento
   - Assert: No mapPin icon
```

### 4.4 Pages

#### ArtistaProfilePage.test.tsx

**Archivo:** `__tests__/ArtistaProfilePage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders artista profile successfully | Integration | Renderiza perfil completo con datos |
| shows loading skeleton while fetching | Integration | Muestra skeleton mientras carga |
| displays error message on not found | Integration | Muestra mensaje de error 404 |
| displays error message on network error | Integration | Muestra mensaje de error de red |
| renders all sections correctly | Integration | Todas las secciones visibles |
| navigates back to home on error | Integration | Boton volver a inicio funciona |

**Casos Detallados:**

```markdown
1. **renders artista profile successfully**
   - Setup: Route con params { id: '123e4567-e89b-12d3-a456-426614174000' }
   - Setup: MSW handler retorna mockArtistaCompleto
   - Render: `<ArtistaProfilePage />`
   - Wait for: Loading termina
   - Assert: Avatar visible
   - Assert: Nombre artistico 'Luna Volcánica' visible
   - Assert: Descripcion visible
   - Assert: Location 'Madrid, España' visible

2. **shows loading skeleton while fetching**
   - Setup: MSW handler con delay de 500ms
   - Render: `<ArtistaProfilePage />`
   - Assert: Skeleton avatar visible
   - Assert: Skeleton text lines visible
   - Assert: No datos reales visibles
   - Wait for: Skeleton desaparece
   - Assert: Datos reales mostrados

3. **displays error message on not found**
   - Setup: Route con params { id: 'non-existent-id' }
   - Setup: MSW handler retorna 404
   - Render: `<ArtistaProfilePage />`
   - Wait for: Error state
   - Assert: Mensaje 'Artista no encontrado' visible
   - Assert: Codigo error 'ARTISTA_NOT_FOUND' visible
   - Assert: Boton 'Volver al inicio' visible

4. **displays error message on network error**
   - Setup: Route con params { id: 'error-500' }
   - Setup: MSW handler retorna 500
   - Render: `<ArtistaProfilePage />`
   - Wait for: Error state
   - Assert: Mensaje de error generico visible
   - Assert: Boton 'Reintentar' visible

5. **renders all sections correctly**
   - Render: `<ArtistaProfilePage />` con mockArtistaCompleto
   - Wait for: Data loaded
   - Assert: Header section con avatar y nombre
   - Assert: Bio section con descripcion
   - Assert: Stats section (aunque sean 0)
   - Assert: Future campaigns section (placeholder MVP)

6. **navigates back to home on error**
   - Render: `<ArtistaProfilePage />` con error 404
   - Wait for: Error state
   - Click: Boton 'Volver al inicio'
   - Assert: Navigate to '/' llamado
```

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| ArtistaProfilePage.tsx | 85% | 90% | 80% |
| hooks/useArtista.ts | 95% | 100% | 90% |
| services/artista.service.ts | 90% | 100% | 85% |
| components/ArtistaAvatar.tsx | 80% | 85% | 75% |
| components/ArtistaBio.tsx | 85% | 90% | 80% |
| components/ArtistaStats.tsx | 80% | 85% | 75% |
| components/ArtistaLocation.tsx | 90% | 95% | 85% |

**Meta Global:** 80% en todas las metricas

**Prioridad Coverage:**
1. `useArtista` (95%+) - Hook critico de data fetching
2. `artista.service.ts` (90%+) - Service core
3. `ArtistaProfilePage` (85%+) - Flujo principal
4. Components (80%+) - Presentacion

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- --filter=artistas

# Watch mode
npm run test:watch

# Single file
npm run test -- ArtistaProfilePage.test.tsx

# UI mode (Vitest UI)
npm run test:ui
```

## 7. CI/CD Integration

```yaml
# .github/workflows/frontend-landing-tests.yml
name: Landing Tests

on:
  pull_request:
    paths:
      - 'src/web/**'
      - 'src/shared/**'

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        run: |
          cd src/web
          npm ci

      - name: Run Tests
        run: |
          cd src/web
          npm run test:coverage

      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./src/web/coverage/lcov.info
          flags: landing

      - name: Check Coverage Threshold
        run: |
          cd src/web
          npm run test:coverage:check -- --threshold 80
```

## 8. Edge Cases y Error States

### 8.1 Edge Cases Criticos

| Caso | Test | Comportamiento Esperado |
|------|------|------------------------|
| Artista sin descripcion | ArtistaBio.test | Muestra mensaje 'Sin descripción' |
| Artista sin imagen | ArtistaAvatar.test | Muestra placeholder con iniciales |
| Artista sin ubicacion | ArtistaLocation.test | No renderiza seccion de ubicacion |
| ID invalido en URL | ArtistaProfilePage.test | Muestra error 404 |
| Network timeout | artista.service.test | Lanza error de timeout |
| Token expirado (futuro) | useArtista.test | Redirige a login (no aplica en Landing MVP) |
| Datos parciales | ArtistaProfilePage.test | Renderiza con fallbacks |

### 8.2 Error States

| Error | Codigo | Test Coverage |
|-------|--------|---------------|
| Artista no existe | ARTISTA_NOT_FOUND | ArtistaProfilePage: displays error on not found |
| Error de red | ERROR_UNEXPECTED | ArtistaProfilePage: displays network error |
| Timeout | TIMEOUT | artista.service: handles timeout |
| 500 Server Error | ERROR_UNEXPECTED | ArtistaProfilePage: displays network error |

## 9. Testing Best Practices

### 9.1 Testing Library Queries Priority

```typescript
// PRIORIDAD (de mejor a peor):
// 1. getByRole
expect(screen.getByRole('heading', { name: 'Luna Volcánica' })).toBeInTheDocument();

// 2. getByLabelText (forms)
expect(screen.getByLabelText('Nombre Artístico')).toBeInTheDocument();

// 3. getByText
expect(screen.getByText('Madrid, España')).toBeInTheDocument();

// 4. getByTestId (ULTIMO RECURSO)
expect(screen.getByTestId('artista-avatar')).toBeInTheDocument();
```

### 9.2 Async Testing

```typescript
// CORRECTO - waitFor para queries asincronas
await waitFor(() => expect(screen.getByText('Luna Volcánica')).toBeInTheDocument());

// CORRECTO - findBy (async query)
expect(await screen.findByText('Luna Volcánica')).toBeInTheDocument();

// INCORRECTO - query sincrona para datos async
expect(screen.getByText('Luna Volcánica')).toBeInTheDocument(); // Falla
```

### 9.3 User Interactions

```typescript
// CORRECTO - userEvent (simula eventos reales)
import userEvent from '@testing-library/user-event';
await userEvent.click(screen.getByRole('button'));

// EVITAR - fireEvent (eventos sinteticos)
import { fireEvent } from '@testing-library/react';
fireEvent.click(screen.getByRole('button'));
```

## 10. Checklist

- [ ] Mocks definidos para API (`artista.mock.ts`)
- [ ] MSW handlers configurados (`handlers.ts`)
- [ ] Test utilities con QueryClient setup (`test-utils.tsx`)
- [ ] Tests de service (`artista.service.test.ts`)
- [ ] Tests de hook con providers (`useArtista.test.ts`)
- [ ] Tests de componentes UI (`ArtistaAvatar`, `ArtistaBio`, etc.)
- [ ] Tests de page component (`ArtistaProfilePage.test.tsx`)
- [ ] Error states cubiertos (404, 500, timeout)
- [ ] Loading states cubiertos (skeleton)
- [ ] Edge cases cubiertos (sin imagen, sin descripcion)
- [ ] Cobertura 80%+ en archivos core
- [ ] Tests pasan en CI
- [ ] Coverage report generado
- [ ] No console.errors en tests

## 11. Dependencias de Testing

```json
{
  "devDependencies": {
    "vitest": "^1.0.0",
    "@testing-library/react": "^14.0.0",
    "@testing-library/user-event": "^14.5.0",
    "@testing-library/jest-dom": "^6.1.0",
    "msw": "^2.0.0",
    "@vitest/ui": "^1.0.0",
    "jsdom": "^23.0.0"
  }
}
```

## 12. Vitest Config

**Archivo:** `vitest.config.ts`

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/setupTests.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: [
        'node_modules/',
        'src/setupTests.ts',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        '**/dist',
      ],
      statements: 80,
      branches: 80,
      functions: 80,
      lines: 80,
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@shared': path.resolve(__dirname, '../shared'),
    },
  },
});
```

## 13. Notas Finales

### 13.1 Restricciones MVP

- **No testing de autenticacion**: Landing es publica, sin auth
- **No testing de formularios**: Solo visualizacion de perfil
- **No testing de mutations**: Solo queries GET
- **Stats placeholder**: Stats mock con valores 0 (campanias futuras)

### 13.2 Optimizaciones Futuras (Post-MVP)

- Snapshot testing para componentes estables
- E2E tests con Playwright para flujo completo
- Visual regression testing con Percy/Chromatic
- Performance testing con Lighthouse CI
- Accessibility testing con axe-core

### 13.3 Test Execution Time Goal

- Total: < 10 segundos
- Unit tests: < 5 segundos
- Integration tests: < 5 segundos
- MSW overhead: minimo (solo 1 endpoint GET)

### 13.4 Mantenimiento

- Actualizar mocks cuando backend agregue campos
- Revisar handlers MSW si endpoints cambian
- Refactorizar tests si componentes cambian estructura
- Mantener coverage 80%+ en nuevos cambios

---

**Siguiente paso:** Implementar tests siguiendo este plan. Ejecutar `npm run test:coverage` para validar cobertura.
