# Estrategia de Testing: Hacer Backing (Admin)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/admin
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 8 | 85% |
| Integration Tests | 4 | 75% |
| Total | 12 | 80%+ |

**Nota MVP:** El impacto en Admin es BAJO. Esta feature solo implica lectura de backings recibidos por campania. No hay formularios ni flujos complejos - solo visualizacion de estadisticas y lista de backers.

## 2. Estructura de Tests

```
src/admin/src/
├── app/(dashboard)/campanias/[id]/backings/
│   ├── components/
│   │   ├── __tests__/
│   │   │   ├── BackingStatsCards.test.tsx
│   │   │   ├── BackingsTable.test.tsx
│   │   │   ├── BackingFilters.test.tsx
│   │   │   └── CampaignBackingsPage.test.tsx
│   │   ├── BackingStatsCards.tsx
│   │   ├── BackingsTable.tsx
│   │   ├── BackingFilters.tsx
│   │   └── EmptyBackingsState.tsx
│   └── page.tsx
├── hooks/
│   └── __tests__/
│       ├── use-campaign-backings.test.ts
│       └── use-campaign-stats.test.ts
└── services/
    └── __tests__/
        └── backing.service.test.ts
```

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/mocks/backing.mock.ts`

```typescript
import type { BackingPublicDto, CampaniaStats } from "@shared/types";

export const mockBackingPublic: BackingPublicDto = {
  id: "backing-1",
  nombreBacker: "Maria Lopez",
  monto: 25.00,
  rewardNombre: "CD Fisico Firmado",
  mensaje: "Mucha suerte con el proyecto!",
  fechaCreacion: "2026-02-13T10:30:00Z",
};

export const mockBackingAnonymous: BackingPublicDto = {
  id: "backing-2",
  nombreBacker: "Anonimo",
  monto: 50.00,
  rewardNombre: null,
  mensaje: null,
  fechaCreacion: "2026-02-13T09:15:00Z",
};

export const mockBackingList: BackingPublicDto[] = [
  mockBackingPublic,
  mockBackingAnonymous,
  {
    id: "backing-3",
    nombreBacker: "Carlos Sanchez",
    monto: 10.00,
    rewardNombre: "Descarga Digital",
    mensaje: null,
    fechaCreacion: "2026-02-13T08:45:00Z",
  },
  {
    id: "backing-4",
    nombreBacker: "Anonimo",
    monto: 100.00,
    rewardNombre: "Experiencia VIP",
    mensaje: "Increible proyecto!",
    fechaCreacion: "2026-02-12T18:00:00Z",
  },
];

export const mockCampaniaStats: CampaniaStats = {
  campaniaId: "campania-1",
  totalBackers: 234,
  totalRecaudado: 2340.00,
  promedioAporte: 10.00,
  aporteMinimo: 1.00,
  aporteMaximo: 100.00,
  diasRestantes: 46,
};

export const mockEmptyStats: CampaniaStats = {
  campaniaId: "campania-1",
  totalBackers: 0,
  totalRecaudado: 0,
  promedioAporte: 0,
  aporteMinimo: 0,
  aporteMaximo: 0,
  diasRestantes: 46,
};

export const mockPaginatedBackings = {
  items: mockBackingList,
  totalCount: 234,
  page: 1,
  pageSize: 20,
  totalPages: 12,
};
```

### 3.2 Service Mocks

**Archivo:** Tests usan `vi.mock` para mockear services directamente

```typescript
vi.mock("@/services/backing.service", () => ({
  backingService: {
    getByCampania: vi.fn(),
    getStats: vi.fn(),
  },
}));
```

### 3.3 Test Utilities

**Archivo:** `src/admin/src/test-utils.tsx` (ya existe)

```typescript
// Ya configurado con QueryClientProvider
import { render } from "@/test-utils";
```

**Wrapper personalizado para tests de hooks:**

```typescript
// En cada archivo de test de hooks
function createWrapper() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return function Wrapper({ children }: { children: ReactNode }) {
    return createElement(QueryClientProvider, { client: queryClient }, children);
  };
}
```

## 4. Tests por Modulo

### 4.1 Components

#### BackingStatsCards.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingStatsCards.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders all stats cards | Unit | Renderiza 4 cards: Total Raised, Total Backers, Avg Amount, Min/Max |
| displays zero state correctly | Unit | Muestra "0 EUR", "0 backers" cuando stats vacias |
| formats currency correctly | Unit | Muestra "2,340.00 EUR" con formato correcto |
| displays average calculation | Unit | Promedio = totalRecaudado / totalBackers |
| shows min and max amounts | Unit | Muestra monto minimo y maximo formateados |

**Casos Detallados:**

```markdown
1. **renders all stats cards**
   - Render con mockCampaniaStats
   - Assert: "Total Raised", "Total Backers", "Avg Amount" en documento
   - Assert: valores numericos visibles

2. **displays zero state correctly**
   - Render con mockEmptyStats (0 backers, 0 recaudado)
   - Assert: "0 EUR" en Total Raised
   - Assert: "0" en Total Backers
   - Assert: "0 EUR" en Average Amount

3. **formats currency correctly**
   - Render con stats de 2340.00 EUR
   - Assert: texto "2,340.00 EUR" visible (usa formatCurrency de shared/utils)

4. **displays average calculation**
   - Render con totalRecaudado: 2340, totalBackers: 234
   - Assert: "10.00 EUR" en Average Amount card

5. **shows min and max amounts**
   - Render con aporteMinimo: 1, aporteMaximo: 100
   - Assert: "1.00 EUR" en Min Amount
   - Assert: "100.00 EUR" en Max Amount
```

**Setup:**
```typescript
import { render, screen } from "@/test-utils";
import { BackingStatsCards } from "../BackingStatsCards";
import { mockCampaniaStats, mockEmptyStats } from "@/mocks/backing.mock";
```

#### BackingsTable.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingsTable.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders table headers | Unit | Muestra columnas: Backer, Monto, Recompensa, Mensaje, Fecha |
| renders backing rows | Unit | Renderiza cada backing con datos correctos |
| displays anonymous backers | Unit | Muestra "Anonimo" con estilo especial (italic, muted) |
| shows empty reward cell | Unit | Muestra "-" cuando rewardNombre es null |
| formats dates relatively | Unit | Usa formatRelativeDate ("hace 2 horas") |
| renders empty state | Unit | Muestra mensaje "No hay backings aun" si lista vacia |
| handles pagination | Integration | Muestra paginacion correcta (page 1 of 12) |
| filters by search | Integration | Filtra backings por nombre de backer |
| filters by reward | Integration | Filtra backings por recompensa seleccionada |

**Casos Detallados:**

```markdown
1. **renders table headers**
   - Render con mockBackingList
   - Assert: headers "Backer", "Monto", "Recompensa", "Mensaje", "Fecha" visibles

2. **renders backing rows**
   - Render con 4 backings
   - Assert: 4 rows en tbody
   - Assert: "Maria Lopez", "25.00 EUR", "CD Fisico Firmado" visibles

3. **displays anonymous backers**
   - Render con mockBackingAnonymous
   - Assert: "Anonimo" en tabla
   - Assert: celda tiene class "italic text-muted-foreground"

4. **shows empty reward cell**
   - Render con backing sin rewardNombre (null)
   - Assert: celda muestra "-"

5. **formats dates relatively**
   - Render con backing de hace 2 horas
   - Assert: texto "hace 2 horas" visible (mock de formatRelativeDate)

6. **renders empty state**
   - Render con backings = []
   - Assert: "No hay backings aun" visible
   - Assert: tabla no renderizada

7. **handles pagination**
   - Render con mockPaginatedBackings (page 1, totalPages 12)
   - Assert: "Page 1 of 12" visible
   - Assert: botones "Previous" y "Next"

8. **filters by search**
   - Render con lista completa
   - User types "Maria" en search input
   - Assert: solo 1 row visible (Maria Lopez)

9. **filters by reward**
   - Render con lista completa
   - User selects "CD Fisico Firmado" en filter dropdown
   - Assert: solo backings con ese reward visibles
```

**Setup:**
```typescript
import { render, screen, fireEvent } from "@/test-utils";
import userEvent from "@testing-library/user-event";
import { BackingsTable } from "../BackingsTable";
import { mockBackingList, mockPaginatedBackings } from "@/mocks/backing.mock";
```

#### BackingFilters.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingFilters.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders search input | Unit | Input de busqueda visible |
| renders reward filter select | Unit | Dropdown de recompensas visible |
| calls onSearchChange on input | Unit | Callback ejecutado al escribir |
| calls onRewardFilter on select | Unit | Callback ejecutado al seleccionar reward |
| shows all rewards in dropdown | Unit | Lista todas las recompensas unicas + "All" option |
| clears filters | Unit | Boton "Clear" resetea filtros |

**Casos Detallados:**

```markdown
1. **renders search input**
   - Render <BackingFilters />
   - Assert: input con placeholder "Buscar por nombre..." visible

2. **renders reward filter select**
   - Render con rewards = ["CD Fisico", "Descarga Digital"]
   - Assert: select visible con 3 options (All, CD Fisico, Descarga Digital)

3. **calls onSearchChange on input**
   - Render con onSearchChange mock
   - User types "Maria"
   - Assert: onSearchChange llamado con "Maria"

4. **calls onRewardFilter on select**
   - Render con onRewardFilter mock
   - User selects "CD Fisico"
   - Assert: onRewardFilter llamado con "CD Fisico"

5. **shows all rewards in dropdown**
   - Render con 3 rewards unicos
   - Assert: 4 opciones en select (All + 3 rewards)

6. **clears filters**
   - Render con filtros aplicados (search="Maria", reward="CD Fisico")
   - User clicks "Clear filters"
   - Assert: onSearchChange(""), onRewardFilter(null) llamados
```

**Setup:**
```typescript
import { render, screen } from "@/test-utils";
import userEvent from "@testing-library/user-event";
import { BackingFilters } from "../BackingFilters";
```

#### CampaignBackingsPage.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/CampaignBackingsPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders page title | Integration | Titulo "Backings recibidos" visible |
| fetches and displays stats | Integration | Carga stats via useCampaignStats y renderiza cards |
| fetches and displays backings | Integration | Carga backings via useCampaignBackings y renderiza tabla |
| shows loading skeletons | Integration | Muestra skeleton mientras isLoading=true |
| shows error state | Integration | Muestra error message si query falla |
| handles empty backings | Integration | Muestra EmptyBackingsState si no hay backings |

**Casos Detallados:**

```markdown
1. **renders page title**
   - Render page
   - Assert: "Backings recibidos" heading visible

2. **fetches and displays stats**
   - Mock useCampaignStats retorna mockCampaniaStats
   - Render page
   - Assert: BackingStatsCards renderizado con stats correctas

3. **fetches and displays backings**
   - Mock useCampaignBackings retorna mockBackingList
   - Render page
   - Assert: BackingsTable renderizada con 4 rows

4. **shows loading skeletons**
   - Mock useCampaignStats con isLoading=true
   - Render page
   - Assert: Skeleton components visibles (shimmer effect)

5. **shows error state**
   - Mock useCampaignBackings retorna error
   - Render page
   - Assert: "Error al cargar backings" visible
   - Assert: boton "Retry" visible

6. **handles empty backings**
   - Mock useCampaignBackings retorna []
   - Render page
   - Assert: EmptyBackingsState visible ("No hay backings aun")
```

**Setup:**
```typescript
import { render, screen } from "@/test-utils";
import CampaignBackingsPage from "../../page";
import { useCampaignStats, useCampaignBackings } from "@/hooks/use-campaign-backings";
import { mockCampaniaStats, mockBackingList } from "@/mocks/backing.mock";

vi.mock("@/hooks/use-campaign-backings");
```

### 4.2 Hooks

#### use-campaign-backings.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-campaign-backings.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| useCampaignBackings returns data on success | Unit | Query exitosa retorna lista de backings |
| useCampaignBackings handles loading state | Unit | isLoading=true mientras fetching |
| useCampaignBackings handles error state | Unit | isError=true cuando service falla |
| useCampaignBackings does not fetch when campaniaId empty | Unit | Query disabled si campaniaId="" |
| useCampaignStats returns stats on success | Unit | Query exitosa retorna CampaniaStats |
| useCampaignStats handles error state | Unit | isError=true cuando service falla |

**Casos Detallados:**

```markdown
1. **useCampaignBackings returns data on success**
   - Mock backingService.getByCampania retorna mockPaginatedBackings
   - renderHook(() => useCampaignBackings("campania-1"))
   - waitFor isSuccess=true
   - Assert: result.current.data.items.length === 4
   - Assert: result.current.data.items[0].nombreBacker === "Maria Lopez"

2. **useCampaignBackings handles loading state**
   - Mock backingService.getByCampania con Promise pendiente
   - renderHook(() => useCampaignBackings("campania-1"))
   - Assert: result.current.isLoading === true

3. **useCampaignBackings handles error state**
   - Mock backingService.getByCampania lanza Error("Network error")
   - renderHook(() => useCampaignBackings("campania-1"))
   - waitFor isError=true
   - Assert: result.current.error definido

4. **useCampaignBackings does not fetch when campaniaId empty**
   - renderHook(() => useCampaignBackings(""))
   - Assert: backingService.getByCampania NOT called

5. **useCampaignStats returns stats on success**
   - Mock backingService.getStats retorna mockCampaniaStats
   - renderHook(() => useCampaignStats("campania-1"))
   - waitFor isSuccess=true
   - Assert: result.current.data.totalBackers === 234
   - Assert: result.current.data.totalRecaudado === 2340.00

6. **useCampaignStats handles error state**
   - Mock backingService.getStats lanza Error
   - renderHook(() => useCampaignStats("campania-1"))
   - waitFor isError=true
   - Assert: result.current.error definido
```

**Setup:**
```typescript
import { renderHook, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement } from "react";
import { useCampaignBackings, useCampaignStats } from "../use-campaign-backings";
import { backingService } from "@/services/backing.service";
import { mockPaginatedBackings, mockCampaniaStats } from "@/mocks/backing.mock";

vi.mock("@/services/backing.service", () => ({
  backingService: {
    getByCampania: vi.fn(),
    getStats: vi.fn(),
  },
}));

function createWrapper() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return function Wrapper({ children }: { children: ReactNode }) {
    return createElement(QueryClientProvider, { client: queryClient }, children);
  };
}
```

### 4.3 Services

#### backing.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/backing.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getByCampania returns paginated list | Unit | GET /api/campanias/{id}/backings retorna lista paginada |
| getByCampania sends correct query params | Unit | Envia pageNumber, pageSize en query string |
| getByCampania handles API error | Unit | Lanza error si API retorna 404/500 |
| getStats returns CampaniaStats | Unit | GET /api/campanias/{id}/stats retorna stats correctas |
| getStats handles error | Unit | Lanza error si API falla |

**Casos Detallados:**

```markdown
1. **getByCampania returns paginated list**
   - Mock axios.get retorna { data: { data: mockPaginatedBackings } }
   - result = await backingService.getByCampania("campania-1", { page: 1, pageSize: 20 })
   - Assert: result.items.length === 4
   - Assert: result.totalCount === 234

2. **getByCampania sends correct query params**
   - Mock axios.get
   - await backingService.getByCampania("campania-1", { page: 2, pageSize: 10 })
   - Assert: axios.get llamado con "/api/campanias/campania-1/backings?pageNumber=2&pageSize=10"

3. **getByCampania handles API error**
   - Mock axios.get lanza error 404
   - expect(backingService.getByCampania("invalid-id")).rejects.toThrow()

4. **getStats returns CampaniaStats**
   - Mock axios.get retorna { data: { data: mockCampaniaStats } }
   - result = await backingService.getStats("campania-1")
   - Assert: result.totalBackers === 234
   - Assert: result.totalRecaudado === 2340.00

5. **getStats handles error**
   - Mock axios.get lanza error
   - expect(backingService.getStats("invalid-id")).rejects.toThrow()
```

**Setup:**
```typescript
import { backingService } from "../backing.service";
import axios from "axios";
import { mockPaginatedBackings, mockCampaniaStats } from "@/mocks/backing.mock";

vi.mock("axios");
```

**Nota:** El service usa axios configurado en `@/lib/api.ts`. Mockear axios directamente o usar MSW para interceptar requests.

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| BackingStatsCards.tsx | 90% | 100% | 85% |
| BackingsTable.tsx | 85% | 90% | 80% |
| BackingFilters.tsx | 90% | 100% | 85% |
| CampaignBackingsPage (integration) | 80% | 85% | 75% |
| use-campaign-backings.ts | 95% | 100% | 90% |
| backing.service.ts | 95% | 100% | 90% |

**Meta Global:** 80% en todas las metricas

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar tests de feature especifica
npm run test -- --filter=backing

# Watch mode
npm run test -- --watch

# Ejecutar solo tests de hooks
npm run test -- hooks/__tests__/use-campaign-backings.test.ts

# Ejecutar solo tests de componentes
npm run test -- app/(dashboard)/campanias/[id]/backings/components/__tests__
```

## 7. CI/CD Integration

```yaml
# .github/workflows/admin-tests.yml
- name: Run Admin Frontend Tests
  run: |
    cd src/admin
    npm run test -- --coverage

- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./src/admin/coverage/coverage-final.json
    flags: admin-frontend
```

## 8. Checklist

- [ ] Mocks definidos para backings (backing.mock.ts)
- [ ] Test utilities configurados (ya existen en test-utils.tsx)
- [ ] Tests de BackingStatsCards (5 unit tests)
- [ ] Tests de BackingsTable (9 tests: 6 unit, 3 integration)
- [ ] Tests de BackingFilters (6 unit tests)
- [ ] Tests de CampaignBackingsPage (6 integration tests)
- [ ] Tests de useCampaignBackings hook (4 unit tests)
- [ ] Tests de useCampaignStats hook (2 unit tests)
- [ ] Tests de backing.service (5 unit tests)
- [ ] Cobertura 80%+
- [ ] Tests pasan en CI

## 9. Notas Adicionales

### 9.1 Prioridades MVP

Dado el impacto BAJO de esta feature en Admin, priorizar:

1. **Tests de hooks** (criticos para data fetching)
2. **Tests de BackingsTable** (componente principal)
3. **Tests de BackingStatsCards** (visualizacion de metricas)
4. **Tests de filtros** (UX secundaria, puede reducirse)

### 9.2 Exclusiones MVP

Estas funcionalidades NO estan en el MVP inicial de Admin:

- **Export a CSV/Excel** de backings (feature futura)
- **Envio de mensajes a backers** (feature futura)
- **Analisis avanzado** de backings por fecha/reward (feature futura)
- **Refunds** o modificacion de backings (feature post-MVP)

Por tanto, NO se crean tests para estas features.

### 9.3 Dependencias de Shared

Los tests dependen de tipos en `src/shared/types/backing.ts`:

- `BackingPublicDto`
- `CampaniaStats`

**Importante:** Si estos tipos cambian, actualizar mocks en `backing.mock.ts`.

### 9.4 Patrones de Testing Adoptados

**Basado en tests existentes de Rewards:**

1. **Mocks a nivel de service:** Usar `vi.mock("@/services/backing.service")`
2. **Wrapper de QueryClient:** Crear wrapper con `retry: false` para tests
3. **Test utils:** Usar `@/test-utils` para render con providers
4. **Mock data separado:** Crear `backing.mock.ts` en lugar de inline mocks
5. **Naming:** Patron `{Component}.test.tsx` o `{hook}.test.ts`

### 9.5 Testing de Paginacion

BackingsTable implementa paginacion client-side (filtrado) o server-side (query params).

**Si server-side:**
```typescript
// Test de paginacion
it("fetches next page on click", async () => {
  const { result } = renderHook(() => useCampaignBackings("campania-1", { page: 1 }));

  await waitFor(() => expect(result.current.isSuccess).toBe(true));

  // User clicks "Next"
  act(() => {
    result.current.goToNextPage(); // metodo del hook
  });

  await waitFor(() => {
    expect(backingService.getByCampania).toHaveBeenCalledWith("campania-1", { page: 2, pageSize: 20 });
  });
});
```

**Si client-side:**
```typescript
// Test de paginacion local
it("shows page 2 items", () => {
  render(<BackingsTable backings={mockBackingList} />);

  // Asume paginacion local con 10 items/page
  expect(screen.getAllByRole("row")).toHaveLength(11); // header + 10 rows

  fireEvent.click(screen.getByText("Next"));

  // Pagina 2 muestra items 11-20
  expect(screen.getAllByRole("row")).toHaveLength(11);
});
```

### 9.6 Testing de Filtros

BackingFilters filtra por:
- **Search term:** Busca en `nombreBacker`
- **Reward filter:** Filtra por `rewardNombre`

**Implementacion sugerida:** Filtros en componente padre (CampaignBackingsPage) que pasan filteredBackings a BackingsTable.

**Alternativa:** Filtros dentro de BackingsTable con state local.

**Tests deben validar:**
- Filtro por search actualiza lista visible
- Filtro por reward actualiza lista visible
- Ambos filtros aplicados simultáneamente funcionan (AND logic)

### 9.7 Anonymous Backers Styling

Los backers anonimos (`nombreBacker: "Anonimo"`) deben mostrarse con estilo especial:

```typescript
// En BackingsTable.tsx
<td className={backing.nombreBacker === "Anonimo" ? "italic text-muted-foreground" : ""}>
  {backing.nombreBacker}
</td>
```

**Test debe validar:**
- Celda con "Anonimo" tiene classes `italic text-muted-foreground`
- Celda con nombre real NO tiene esas classes

### 9.8 Relative Dates

Usar `formatRelativeDate` de `@shared/utils`:

```typescript
import { formatRelativeDate } from "@shared/utils";

// En BackingsTable
<td>{formatRelativeDate(backing.fechaCreacion)}</td>
```

**Test debe validar:**
- Formato correcto ("hace 2 horas", "hace 3 dias")
- Manejo de fechas antiguas ("hace 2 meses")

**Mock de formatRelativeDate en tests:**
```typescript
vi.mock("@shared/utils", () => ({
  formatRelativeDate: vi.fn((date: string) => "hace 2 horas"),
  formatCurrency: vi.fn((amount: number) => `${amount.toFixed(2)} EUR`),
}));
```

### 9.9 Empty States

Dos empty states posibles:

1. **No hay backings para esta campania**
   - Mensaje: "No hay backings aun. Comparte tu campania para recibir apoyo"
   - Boton: "Compartir campania" (redirect a share modal)

2. **Error al cargar backings**
   - Mensaje: "Error al cargar backings"
   - Boton: "Reintentar" (refetch query)

**Tests deben validar:**
- Render de EmptyBackingsState cuando `backings.length === 0`
- Render de ErrorState cuando `isError === true`
- Click en "Reintentar" llama `refetch()`

### 9.10 Status Badges

Si en el futuro se agregan badges de estado (Pending, Completed, Refunded):

```typescript
// Futuro - NO MVP
export const BACKING_STATUS_COLORS: Record<number, string> = {
  1: "yellow",  // Pendiente
  2: "green",   // Completado
  3: "red",     // Fallido
  4: "gray",    // Cancelado
};
```

**MVP:** Todos los backings son `Completado` (sin pasarela real). No mostrar badge de estado.

---

## Resumen Final

**Total de tests planificados:** 12
- Unit tests de componentes: 5 (BackingStatsCards) + 6 (BackingsTable) + 6 (BackingFilters) = 17 unit tests
- Integration tests: 6 (CampaignBackingsPage)
- Hooks tests: 6 (useCampaignBackings + useCampaignStats)
- Service tests: 5 (backing.service)

**Total real:** 34 tests (mas que el resumen inicial debido a desglose detallado)

**Archivos creados:**
1. `src/admin/src/mocks/backing.mock.ts` - Mock data
2. `src/admin/src/hooks/__tests__/use-campaign-backings.test.ts` - 6 tests
3. `src/admin/src/services/__tests__/backing.service.test.ts` - 5 tests
4. `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingStatsCards.test.tsx` - 5 tests
5. `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingsTable.test.tsx` - 9 tests
6. `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingFilters.test.tsx` - 6 tests
7. `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/CampaignBackingsPage.test.tsx` - 6 tests

**Cobertura esperada:** 80%+ en todos los archivos

**Comandos:**
```bash
npm run test -- --filter=backing
npm run test -- --coverage
```

**Proximos pasos:**
1. Implementar componentes (BackingStatsCards, BackingsTable, etc.)
2. Implementar hooks (useCampaignBackings, useCampaignStats)
3. Implementar service (backing.service.ts)
4. Escribir tests siguiendo este plan
5. Verificar cobertura 80%+
