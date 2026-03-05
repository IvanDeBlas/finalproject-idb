# Estrategia de Testing: Registro de Artista (Landing)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/web (Landing - Vite + React)
**Cobertura Objetivo:** 80%+

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 12 | 85% |
| Integration Tests | 4 | 75% |
| Total | 16 | 80%+ |

**Alcance:**
- Componente de perfil publico de artista (`/artistas/{id}`)
- Hook `useArtista` (query para obtener artista por ID)
- Service `artistaService.getById()`
- Componentes de presentacion: `ArtistaHero`, `ArtistaBio`
- Estados: loading, error, success

**Fuera de alcance:**
- Formularios de registro (viven en `/admin`, no en landing)
- Crear/editar perfil (viven en `/admin`)
- Tests E2E (se realizaran por separado)

---

## 2. Estructura de Tests

```
src/web/src/features/artistas/
├── __tests__/
│   ├── components/
│   │   ├── ArtistaHero.test.tsx
│   │   ├── ArtistaBio.test.tsx
│   │   └── ArtistaPublicProfilePage.test.tsx
│   ├── hooks/
│   │   └── useArtista.test.ts
│   └── infrastructure/
│       └── artista.service.test.ts
├── __mocks__/
│   ├── artista.mock.ts
│   └── handlers.ts
├── presentation/
│   ├── components/
│   │   ├── ArtistaHero.tsx
│   │   ├── ArtistaBio.tsx
│   │   └── index.ts
│   └── pages/
│       └── ArtistaPublicProfilePage.tsx
├── application/
│   └── hooks/
│       └── useArtista.ts
├── infrastructure/
│   └── artista.service.ts
└── domain/
    └── types.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/artista.mock.ts`

```typescript
import type { Artista } from "../domain/types"

export const mockArtistaCompleto: Artista = {
  id: "550e8400-e29b-41d4-a716-446655440000",
  userId: "user-123",
  nombreArtistico: "Los Rockeros del Test",
  descripcion: "Banda de rock alternativo de Madrid con 10 años de trayectoria. Nuestro sonido combina influencias del rock clasico con elementos modernos.",
  pais: "España",
  ciudad: "Madrid",
  imagenUrl: "https://example.com/artista-test.jpg",
  generoMusical: "Rock Alternativo",
  createdAt: new Date("2026-01-15T10:00:00Z"),
  updatedAt: new Date("2026-01-20T15:30:00Z"),
}

export const mockArtistaMinimo: Artista = {
  id: "550e8400-e29b-41d4-a716-446655440001",
  userId: "user-456",
  nombreArtistico: "Artista Minimal",
  descripcion: undefined,
  pais: undefined,
  ciudad: undefined,
  imagenUrl: undefined,
  generoMusical: undefined,
  createdAt: new Date("2026-02-01T08:00:00Z"),
  updatedAt: new Date("2026-02-01T08:00:00Z"),
}

export const mockArtistaConUbicacion: Artista = {
  id: "550e8400-e29b-41d4-a716-446655440002",
  userId: "user-789",
  nombreArtistico: "Banda Internacional",
  descripcion: "Artistas viajeros del mundo",
  pais: "Argentina",
  ciudad: "Buenos Aires",
  imagenUrl: "https://example.com/banda-intl.jpg",
  generoMusical: "Jazz Fusion",
  createdAt: new Date("2026-01-10T12:00:00Z"),
  updatedAt: new Date("2026-01-25T18:00:00Z"),
}

export const mockArtistaList: Artista[] = [
  mockArtistaCompleto,
  mockArtistaMinimo,
  mockArtistaConUbicacion,
]
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from "msw"
import { mockArtistaCompleto, mockArtistaMinimo } from "./artista.mock"

const API_BASE = "/api"

export const artistaHandlers = [
  // GET /api/artistas/:id - Success
  http.get(`${API_BASE}/artistas/:id`, ({ params }) => {
    const { id } = params

    // Caso: Artista completo
    if (id === mockArtistaCompleto.id) {
      return HttpResponse.json({
        data: {
          id: mockArtistaCompleto.id,
          userId: mockArtistaCompleto.userId,
          nombreArtistico: mockArtistaCompleto.nombreArtistico,
          descripcion: mockArtistaCompleto.descripcion,
          pais: mockArtistaCompleto.pais,
          ciudad: mockArtistaCompleto.ciudad,
          imagenUrl: mockArtistaCompleto.imagenUrl,
          generoMusical: mockArtistaCompleto.generoMusical,
          createdAt: mockArtistaCompleto.createdAt.toISOString(),
          updatedAt: mockArtistaCompleto.updatedAt.toISOString(),
        },
        messages: [
          { message: "Artista encontrado", errorCode: "SUCCESS" },
        ],
      })
    }

    // Caso: Artista minimo
    if (id === mockArtistaMinimo.id) {
      return HttpResponse.json({
        data: {
          id: mockArtistaMinimo.id,
          userId: mockArtistaMinimo.userId,
          nombreArtistico: mockArtistaMinimo.nombreArtistico,
          createdAt: mockArtistaMinimo.createdAt.toISOString(),
          updatedAt: mockArtistaMinimo.updatedAt.toISOString(),
        },
        messages: [
          { message: "Artista encontrado", errorCode: "SUCCESS" },
        ],
      })
    }

    // Caso: Not Found
    return HttpResponse.json(
      {
        data: null,
        messages: [
          { message: "Artista no encontrado", errorCode: "ARTISTA_NOT_FOUND" },
        ],
      },
      { status: 404 }
    )
  }),

  // GET /api/artistas/:id - Network Error (para simular errores de red)
  http.get(`${API_BASE}/artistas/network-error`, () => {
    return HttpResponse.error()
  }),

  // GET /api/artistas/:id - Server Error
  http.get(`${API_BASE}/artistas/server-error`, () => {
    return HttpResponse.json(
      {
        data: null,
        messages: [
          { message: "Error interno del servidor", errorCode: "ERROR_UNEXPECTED" },
        ],
      },
      { status: 500 }
    )
  }),
]
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import { render, RenderOptions } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { ReactElement, ReactNode } from "react"

// Create test query client with disabled retries for faster tests
export function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        cacheTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  })
}

interface AllProvidersProps {
  children: ReactNode
  initialRoute?: string
}

export function AllProviders({ children, initialRoute = "/" }: AllProvidersProps) {
  const testQueryClient = createTestQueryClient()

  return (
    <QueryClientProvider client={testQueryClient}>
      <MemoryRouter initialEntries={[initialRoute]}>
        {children}
      </MemoryRouter>
    </QueryClientProvider>
  )
}

interface RenderWithProvidersOptions extends Omit<RenderOptions, "wrapper"> {
  initialRoute?: string
}

export function renderWithProviders(
  ui: ReactElement,
  options?: RenderWithProvidersOptions
) {
  const { initialRoute, ...renderOptions } = options || {}

  return render(ui, {
    wrapper: ({ children }) => (
      <AllProviders initialRoute={initialRoute}>{children}</AllProviders>
    ),
    ...renderOptions,
  })
}
```

### 3.4 MSW Setup

**Archivo:** `vitest.setup.ts`

```typescript
import { afterAll, afterEach, beforeAll } from "vitest"
import { setupServer } from "msw/node"
import { artistaHandlers } from "./src/features/artistas/__mocks__/handlers"
import "@testing-library/jest-dom/vitest"

// Setup MSW server
export const server = setupServer(...artistaHandlers)

beforeAll(() => {
  server.listen({ onUnhandledRequest: "warn" })
})

afterEach(() => {
  server.resetHandlers()
})

afterAll(() => {
  server.close()
})
```

---

## 4. Tests por Modulo

### 4.1 Components

#### ArtistaHero.test.tsx

**Archivo:** `__tests__/components/ArtistaHero.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders artista name | Unit | Renderiza el nombre artistico correctamente |
| renders avatar with image | Unit | Muestra avatar con imagen cuando imagenUrl existe |
| renders avatar fallback | Unit | Muestra fallback (icono User) cuando no hay imagenUrl |
| renders genero musical | Unit | Muestra genero musical cuando existe |
| hides genero when undefined | Unit | No renderiza seccion de genero si es undefined |
| applies gradient background | Unit | Aplica el gradiente de fondo correctamente |

**Casos Detallados:**

```markdown
1. **renders artista name**
   - Render ArtistaHero con mockArtistaCompleto
   - Assert: "Los Rockeros del Test" visible en h1

2. **renders avatar with image**
   - Render con artista que tiene imagenUrl
   - Assert: elemento img con src === imagenUrl
   - Assert: alt === nombreArtistico

3. **renders avatar fallback**
   - Render con mockArtistaMinimo (sin imagenUrl)
   - Assert: icono User visible
   - Assert: no hay elemento img

4. **renders genero musical**
   - Render con mockArtistaCompleto (tiene generoMusical)
   - Assert: texto "Rock Alternativo" visible

5. **hides genero when undefined**
   - Render con mockArtistaMinimo (sin generoMusical)
   - Assert: texto de genero no existe en documento

6. **applies gradient background**
   - Render con cualquier artista
   - Assert: elemento con clase "bg-gradient-to-r from-purple-900..."
```

**Coverage esperado:** 90% (lineas), 100% (funciones)

---

#### ArtistaBio.test.tsx

**Archivo:** `__tests__/components/ArtistaBio.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders descripcion | Unit | Muestra descripcion cuando existe |
| renders placeholder when no descripcion | Unit | Muestra "No hay descripcion disponible" cuando es undefined |
| renders ubicacion completa | Unit | Muestra ciudad y pais cuando ambos existen |
| renders solo ciudad | Unit | Muestra solo ciudad cuando pais es undefined |
| renders solo pais | Unit | Muestra solo pais cuando ciudad es undefined |
| hides ubicacion when both undefined | Unit | No muestra seccion de ubicacion si no hay datos |
| renders genero musical | Unit | Muestra genero cuando existe |
| hides genero when undefined | Unit | No muestra genero si es undefined |
| renders social networks placeholder | Unit | Muestra mensaje "Proximamente" para redes sociales |

**Casos Detallados:**

```markdown
1. **renders descripcion**
   - Render con mockArtistaCompleto
   - Assert: descripcion completa visible

2. **renders placeholder when no descripcion**
   - Render con mockArtistaMinimo (sin descripcion)
   - Assert: texto "No hay descripcion disponible" visible
   - Assert: texto tiene clase "italic"

3. **renders ubicacion completa**
   - Render con mockArtistaConUbicacion
   - Assert: texto "Buenos Aires, Argentina" visible
   - Assert: icono MapPin visible

4. **renders solo ciudad**
   - Render con artista { ciudad: "Barcelona", pais: undefined }
   - Assert: texto "Barcelona" visible (sin coma ni pais)

5. **renders solo pais**
   - Render con artista { ciudad: undefined, pais: "Mexico" }
   - Assert: texto "Mexico" visible

6. **hides ubicacion when both undefined**
   - Render con mockArtistaMinimo (sin ubicacion)
   - Assert: icono MapPin no existe
   - Assert: no hay texto de ubicacion

7. **renders genero musical**
   - Render con mockArtistaConUbicacion (tiene genero)
   - Assert: texto "Jazz Fusion" visible
   - Assert: icono Music visible

8. **hides genero when undefined**
   - Render con mockArtistaMinimo
   - Assert: icono Music no existe

9. **renders social networks placeholder**
   - Render con cualquier artista
   - Assert: titulo "Redes Sociales" visible
   - Assert: texto "Proximamente" visible
```

**Coverage esperado:** 95% (lineas), 100% (funciones)

---

#### ArtistaPublicProfilePage.test.tsx

**Archivo:** `__tests__/components/ArtistaPublicProfilePage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders skeleton on loading | Integration | Muestra skeleton mientras carga |
| renders artista profile on success | Integration | Renderiza perfil completo cuando carga exitosamente |
| renders ArtistaHero component | Integration | Renderiza componente ArtistaHero con artista |
| renders ArtistaBio component | Integration | Renderiza componente ArtistaBio con artista |
| renders estadisticas placeholder | Integration | Muestra seccion de estadisticas con placeholder |
| renders not found on error | Integration | Muestra mensaje "Artista no encontrado" en error |
| renders not found when artista is null | Integration | Muestra not found cuando data es null |
| not found has back button | Integration | Boton "Volver al inicio" presente y funcional |

**Casos Detallados:**

```markdown
1. **renders skeleton on loading**
   - Mock useArtista para retornar { isLoading: true }
   - Render ArtistaPublicProfilePage
   - Assert: multiples Skeleton components visibles

2. **renders artista profile on success**
   - Mock useArtista para retornar { data: mockArtistaCompleto, isLoading: false, isError: false }
   - Render page
   - Assert: nombre artista visible
   - Assert: no hay skeleton

3. **renders ArtistaHero component**
   - Mock exitoso con mockArtistaCompleto
   - Assert: ArtistaHero renderizado (verificar avatar y nombre)

4. **renders ArtistaBio component**
   - Mock exitoso con mockArtistaCompleto
   - Assert: ArtistaBio renderizado (verificar titulo "Sobre el Artista")

5. **renders estadisticas placeholder**
   - Mock exitoso
   - Assert: titulo "Estadisticas" visible
   - Assert: texto "Proximamente" visible
   - Assert: items "Campanias", "Backers", "Fondos Recaudados" con valor "-"

6. **renders not found on error**
   - Mock useArtista para retornar { isError: true, isLoading: false }
   - Assert: titulo "Artista no encontrado" visible
   - Assert: icono AlertCircle visible
   - Assert: mensaje de error descriptivo

7. **renders not found when artista is null**
   - Mock useArtista: { data: null, isError: false, isLoading: false }
   - Assert: muestra componente ArtistaNotFound

8. **not found has back button**
   - Mock error state
   - Assert: boton con texto "Volver al inicio" existe
   - Assert: Link apunta a ROUTES.HOME
```

**Coverage esperado:** 85% (lineas), 90% (funciones)

---

### 4.2 Hooks

#### useArtista.test.ts

**Archivo:** `__tests__/hooks/useArtista.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns data on success | Unit | Retorna artista correctamente cuando la query tiene exito |
| isLoading true initially | Unit | isLoading es true al inicio |
| isError true on API error | Unit | isError es true cuando API falla |
| query disabled when id is empty | Unit | Query no se ejecuta si id es vacio/undefined |
| query key includes artista id | Unit | Query key contiene el ID del artista |
| refetch on query key change | Integration | Se vuelve a ejecutar query cuando cambia el ID |

**Setup:**
```typescript
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useArtista } from "../../application/hooks/useArtista"
import { server } from "../../../vitest.setup"
import { http, HttpResponse } from "msw"
import { mockArtistaCompleto } from "../../__mocks__/artista.mock"

const createWrapper = () => {
  const testQueryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
    },
  })

  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={testQueryClient}>
      {children}
    </QueryClientProvider>
  )
}
```

**Casos Detallados:**

```markdown
1. **returns data on success**
   - renderHook(() => useArtista(mockArtistaCompleto.id), { wrapper })
   - waitFor(() => expect(result.current.isSuccess).toBe(true))
   - Assert: result.current.data.nombreArtistico === "Los Rockeros del Test"
   - Assert: result.current.data.id === mockArtistaCompleto.id

2. **isLoading true initially**
   - renderHook(() => useArtista(mockArtistaCompleto.id), { wrapper })
   - Assert (sin waitFor): result.current.isLoading === true
   - Assert: result.current.data === undefined

3. **isError true on API error**
   - Override MSW handler para retornar 404
   - renderHook(() => useArtista("invalid-id"), { wrapper })
   - waitFor(() => expect(result.current.isError).toBe(true))
   - Assert: result.current.data === undefined

4. **query disabled when id is empty**
   - renderHook(() => useArtista(""), { wrapper })
   - Assert: result.current.fetchStatus === "idle"
   - Assert: query no se ejecuta (verificar mock no fue llamado)

5. **query key includes artista id**
   - renderHook(() => useArtista("test-id"), { wrapper })
   - Assert: result.current.dataUpdatedAt existe
   - Verificar queryKey en QueryClient cache

6. **refetch on query key change**
   - renderHook con initialProps: { id: mockArtistaCompleto.id }
   - waitFor success
   - rerender({ id: mockArtistaMinimo.id })
   - waitFor(() => data cambia a mockArtistaMinimo)
```

**Coverage esperado:** 100% (lineas), 100% (funciones)

---

### 4.3 Infrastructure

#### artista.service.test.ts

**Archivo:** `__tests__/infrastructure/artista.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getById returns artista on success | Unit | Retorna artista correctamente en llamada exitosa |
| getById transforms DTO to domain | Unit | Transforma fechas ISO string a Date objects |
| getById throws on 404 | Unit | Lanza error cuando artista no existe |
| getById throws on network error | Unit | Lanza error en fallos de red |
| getById includes correct URL | Unit | Construye URL correcta con ID |

**Setup:**
```typescript
import { describe, it, expect, beforeAll, afterAll, afterEach } from "vitest"
import { artistaService } from "../../infrastructure/artista.service"
import { server } from "../../../vitest.setup"
import { http, HttpResponse } from "msw"
import { mockArtistaCompleto, mockArtistaMinimo } from "../../__mocks__/artista.mock"
```

**Casos Detallados:**

```markdown
1. **getById returns artista on success**
   - Llamar artistaService.getById(mockArtistaCompleto.id)
   - Assert: resultado.nombreArtistico === mockArtistaCompleto.nombreArtistico
   - Assert: resultado.id === mockArtistaCompleto.id
   - Assert: resultado.descripcion === mockArtistaCompleto.descripcion

2. **getById transforms DTO to domain**
   - Llamar artistaService.getById(mockArtistaCompleto.id)
   - Assert: resultado.createdAt instanceof Date
   - Assert: resultado.updatedAt instanceof Date
   - Assert: fechas coinciden con las del mock (valor, no referencia)

3. **getById throws on 404**
   - Llamar artistaService.getById("non-existent-id")
   - Assert: expect().rejects.toThrow()
   - Verificar error contiene informacion de 404

4. **getById throws on network error**
   - Llamar artistaService.getById("network-error")
   - Assert: expect().rejects.toThrow()
   - Verificar tipo de error de red

5. **getById includes correct URL**
   - Spy en apiFetch
   - Llamar artistaService.getById("test-id-123")
   - Assert: apiFetch fue llamado con URL "/artistas/test-id-123"
```

**Coverage esperado:** 90% (lineas), 100% (funciones)

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| ArtistaHero.tsx | 90% | 100% | 85% |
| ArtistaBio.tsx | 95% | 100% | 90% |
| ArtistaPublicProfilePage.tsx | 85% | 90% | 80% |
| useArtista.ts | 100% | 100% | 100% |
| artista.service.ts | 90% | 100% | 85% |

**Meta Global:** 80%+ en todas las metricas

**Archivos NO testeados (fuera de alcance):**
- `domain/types.ts` (solo tipos, no logica)
- `presentation/components/index.ts` (solo exports)
- `infrastructure/index.ts` (solo exports)

---

## 6. Configuracion de Vitest

### vitest.config.ts

```typescript
import { defineConfig } from "vitest/config"
import react from "@vitejs/plugin-react"
import path from "path"

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: "jsdom",
    setupFiles: "./vitest.setup.ts",
    coverage: {
      provider: "v8",
      reporter: ["text", "json", "html"],
      include: [
        "src/features/**/presentation/**/*.{ts,tsx}",
        "src/features/**/application/**/*.{ts,tsx}",
        "src/features/**/infrastructure/**/*.{ts,tsx}",
      ],
      exclude: [
        "**/__tests__/**",
        "**/__mocks__/**",
        "**/*.test.{ts,tsx}",
        "**/*.d.ts",
        "**/index.ts",
        "**/types.ts",
      ],
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 75,
        statements: 80,
      },
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
      "@shared": path.resolve(__dirname, "../shared"),
    },
  },
})
```

---

## 7. Comandos de Ejecucion

```bash
# Agregar dependencias de testing
npm install -D vitest @vitest/ui @testing-library/react @testing-library/user-event @testing-library/jest-dom jsdom msw

# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de artistas especificamente
npm run test -- artistas

# Watch mode (desarrollo)
npm run test:watch

# UI mode (visual)
npm run test:ui
```

### package.json scripts

```json
{
  "scripts": {
    "test": "vitest run",
    "test:watch": "vitest",
    "test:coverage": "vitest run --coverage",
    "test:ui": "vitest --ui"
  }
}
```

---

## 8. CI/CD Integration

### GitHub Actions Workflow

```yaml
name: Frontend Tests - Landing

on:
  pull_request:
    paths:
      - "src/web/**"
      - "src/shared/**"
  push:
    branches:
      - master

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"
          cache: "npm"
          cache-dependency-path: src/web/package-lock.json

      - name: Install dependencies
        working-directory: src/web
        run: npm ci

      - name: Run tests with coverage
        working-directory: src/web
        run: npm run test:coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: src/web/coverage/coverage-final.json
          flags: frontend-landing
          name: landing-coverage

      - name: Check coverage thresholds
        working-directory: src/web
        run: npm run test:coverage -- --reporter=json --outputFile=coverage-summary.json

      - name: Comment PR with coverage
        if: github.event_name == 'pull_request'
        uses: romeovs/lcov-reporter-action@v0.3.1
        with:
          lcov-file: src/web/coverage/lcov.info
          github-token: ${{ secrets.GITHUB_TOKEN }}
```

---

## 9. Estrategia de Mocking

### 9.1 Mock de API (MSW)

**Ventajas:**
- Intercepta requests a nivel de red (realista)
- No requiere modificar codigo de produccion
- Permite simular errores, delays, edge cases

**Uso:**
- Todos los endpoints GET `/api/artistas/:id`
- Respuestas exitosas, 404, 500, network errors

### 9.2 Mock de Hooks (vi.mock)

**Evitar:** No mockear `useArtista` en tests de componentes. Usar MSW para que el flujo completo funcione.

**Uso limitado:** Solo en tests de componentes que dependen de multiples hooks complejos.

### 9.3 Mock de Router

**react-router-dom:**
- Usar `MemoryRouter` en test-utils
- Simular params con `initialEntries`
- No mockear hooks de router directamente

---

## 10. Tests de Snapshot (Opcional)

**NO recomendado para este proyecto** debido a:
- MVP con cambios frecuentes en UI
- Snapshots generan ruido en PRs
- Dificultan refactoring

**Alternativa:** Tests de regression visual con Playwright (fuera de scope)

---

## 11. Performance Tests

### Tiempo de Ejecucion Esperado

| Suite | Tests | Tiempo |
|-------|-------|--------|
| ArtistaHero | 6 | ~500ms |
| ArtistaBio | 9 | ~800ms |
| ArtistaPublicProfilePage | 8 | ~2s |
| useArtista | 6 | ~1.5s |
| artista.service | 5 | ~700ms |
| **TOTAL** | **34** | **~5.5s** |

**Meta:** < 10 segundos total

---

## 12. Edge Cases y Escenarios Especiales

### 12.1 Estados de Datos

| Caso | Descripcion | Test Coverage |
|------|-------------|---------------|
| Artista completo | Todos los campos poblados | ArtistaHero, ArtistaBio |
| Artista minimo | Solo campos obligatorios (nombreArtistico) | ArtistaBio placeholder tests |
| Sin ubicacion | pais y ciudad undefined | ArtistaBio hide ubicacion |
| Solo ciudad | pais undefined | ArtistaBio render solo ciudad |
| Solo pais | ciudad undefined | ArtistaBio render solo pais |
| Sin imagen | imagenUrl undefined | ArtistaHero fallback |
| Sin genero | generoMusical undefined | ArtistaHero/Bio hide genero |

### 12.2 Estados de Query

| Estado | Descripcion | Test Coverage |
|--------|-------------|---------------|
| Loading | isLoading: true | ArtistaPublicProfilePage skeleton |
| Success | data presente | ArtistaPublicProfilePage renders |
| Error 404 | artista no existe | ArtistaPublicProfilePage not found |
| Error 500 | server error | useArtista isError |
| Network error | sin conexion | artista.service throws |
| Query disabled | id vacio | useArtista disabled query |

### 12.3 Casos de Navegacion

| Caso | Descripcion | Test Coverage |
|------|-------------|---------------|
| URL con ID valido | `/artistas/{valid-id}` | Integration test success |
| URL con ID invalido | `/artistas/fake-id` | Not found screen |
| Cambio de ID en URL | useParams cambia | useArtista refetch |
| Back button desde not found | Click "Volver al inicio" | Navigation test |

---

## 13. Accesibilidad (a11y)

**Tests basicos de accesibilidad:**

```typescript
// Ejemplo para ArtistaPublicProfilePage
it("has accessible heading structure", () => {
  renderWithProviders(<ArtistaPublicProfilePage />)

  const headings = screen.getAllByRole("heading")
  expect(headings[0]).toHaveAttribute("level", "1") // h1: nombre artista
})

it("avatar has alt text", () => {
  renderWithProviders(<ArtistaHero artista={mockArtistaCompleto} />)

  const avatar = screen.getByRole("img")
  expect(avatar).toHaveAttribute("alt", mockArtistaCompleto.nombreArtistico)
})
```

**Herramienta adicional (opcional):**
- `@axe-core/react` para auditorias automatizadas
- Ejecutar en tests de integracion

---

## 14. Checklist de Implementacion

### Fase 1: Setup (1h)
- [ ] Instalar dependencias: vitest, testing-library, msw, jsdom
- [ ] Crear `vitest.config.ts`
- [ ] Crear `vitest.setup.ts` con MSW setup
- [ ] Crear `test-utils.tsx` con providers
- [ ] Agregar scripts en `package.json`

### Fase 2: Mocks (1h)
- [ ] Crear `__mocks__/artista.mock.ts` con fixtures
- [ ] Crear `__mocks__/handlers.ts` con MSW handlers
- [ ] Validar handlers con test simple

### Fase 3: Tests Unitarios (3h)
- [ ] ArtistaHero.test.tsx (6 tests)
- [ ] ArtistaBio.test.tsx (9 tests)
- [ ] useArtista.test.ts (6 tests)
- [ ] artista.service.test.ts (5 tests)

### Fase 4: Tests de Integracion (2h)
- [ ] ArtistaPublicProfilePage.test.tsx (8 tests)
- [ ] Validar flujo completo: loading -> success
- [ ] Validar flujo de error: loading -> error -> not found

### Fase 5: Cobertura y Ajustes (1h)
- [ ] Ejecutar `npm run test:coverage`
- [ ] Verificar cobertura >= 80%
- [ ] Ajustar tests faltantes
- [ ] Documentar casos edge no cubiertos

### Fase 6: CI/CD (30min)
- [ ] Configurar workflow de GitHub Actions
- [ ] Validar tests pasan en CI
- [ ] Configurar Codecov (opcional)

**Tiempo Total Estimado:** 8.5 horas

---

## 15. Dependencias de Testing

### Instalacion Completa

```bash
cd src/web

npm install -D \
  vitest@^1.2.0 \
  @vitest/ui@^1.2.0 \
  @testing-library/react@^14.1.2 \
  @testing-library/user-event@^14.5.2 \
  @testing-library/jest-dom@^6.2.0 \
  jsdom@^24.0.0 \
  msw@^2.1.0 \
  @types/testing-library__jest-dom@^6.0.0
```

### Versiones Compatibles

| Dependencia | Version | Notas |
|-------------|---------|-------|
| vitest | ^1.2.0 | Test runner (compatible con Vite 5) |
| @vitest/ui | ^1.2.0 | UI visual para tests |
| @testing-library/react | ^14.1.2 | Compatible con React 18 |
| @testing-library/user-event | ^14.5.2 | Simular interacciones usuario |
| @testing-library/jest-dom | ^6.2.0 | Matchers custom (toBeInTheDocument) |
| jsdom | ^24.0.0 | DOM environment para Node |
| msw | ^2.1.0 | Mock Service Worker (API mocking) |

---

## 16. Notas Finales

### Limitaciones Conocidas

1. **No se testean tipos TypeScript:** Los archivos `types.ts` no tienen logica ejecutable.
2. **Snapshots no incluidos:** Por decision de proyecto (MVP en cambio).
3. **Tests E2E separados:** Se haran con Playwright en fase posterior.
4. **Animaciones deshabilitadas:** En tests, Tailwind animations se omiten.

### Mejoras Futuras

1. **Visual Regression Testing:** Playwright + Percy para detectar cambios visuales.
2. **Tests de Performance:** Medir tiempos de render con React Profiler.
3. **Tests de Accesibilidad:** Integracion con axe-core.
4. **Mutation Testing:** Stryker para validar calidad de tests.

### Recursos Adicionales

- [Vitest Documentation](https://vitest.dev/)
- [Testing Library React](https://testing-library.com/docs/react-testing-library/intro/)
- [MSW Documentation](https://mswjs.io/)
- [Kent C. Dodds Testing Best Practices](https://kentcdodds.com/blog/common-mistakes-with-react-testing-library)

---

**Fin del documento de estrategia de testing.**
