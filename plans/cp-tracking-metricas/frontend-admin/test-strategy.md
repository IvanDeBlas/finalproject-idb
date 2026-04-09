# Estrategia de Testing: cp-tracking-metricas (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/admin (Next.js 14)
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 44 | 85% |
| Integration Tests | 18 | 78% |
| Total | 62 | 80%+ |

**Tiempo estimado de ejecucion:** < 55 segundos

**Herramientas:**
- Vitest (test runner, `vitest.config.ts` en `src/admin/`)
- @testing-library/react (component testing)
- @testing-library/user-event (interacciones de usuario)
- @tanstack/react-query (`renderHook` + `QueryClientProvider` para hooks)
- `@/test-utils` (wrapper custom con `QueryClientProvider`, `render` reexportado)

**Nota sobre mocking:** El proyecto no usa MSW. El patron establecido es:
- Service tests: `vi.mock("@/lib/api-client")` -> mock de `apiFetch`
- Hook tests: `vi.mock("@/services/metricas.service")` -> mock del service
- Component tests: `vi.mock("@/services/metricas.service")` via el hook que el componente invoca
- Toast: `vi.mock("sonner")` cuando el componente muestra toasts

---

## 2. Estructura de Tests

```
src/admin/src/
├── __mocks__/
│   └── cp-tracking-metricas.mock.ts                            NUEVO
│
├── components/crowdpromotion/metricas/
│   └── __tests__/
│       ├── KpiCard.test.tsx                                    NUEVO
│       ├── KpiCardsGrid.test.tsx                               NUEVO
│       ├── FiltroFechas.test.tsx                               NUEVO
│       ├── RankingPromotoresTable.test.tsx                     NUEVO
│       ├── DesgloseEventosPanel.test.tsx                       NUEVO
│       ├── GraficoTemporal.test.tsx                            NUEVO
│       └── ProgramaMetricasTab.test.tsx                        NUEVO
│
├── hooks/
│   └── __tests__/
│       ├── use-programa-metricas.test.ts                       NUEVO
│       └── use-sortable-table.test.ts                          NUEVO
│
└── services/
    └── __tests__/
        └── metricas.service.test.ts                            NUEVO
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/cp-tracking-metricas.mock.ts`

**Convencion seguida:** Mismo patron que `cp-tareas-promocion.mock.ts` y `inscripcion.mock.ts`.
Tipos importados desde `@shared/types/crowdpromotion`.

```typescript
import type {
    ProgramaMetricasKpis,
    ProgramaMetricasResponse,
    RankingPromotorItem,
    EventosPorDiaItem,
    FiltroFechas,
} from "@shared/types/crowdpromotion"

// ─── Constante de identificadores ────────────────────────────────────────────

export const PROGRAMA_ID = "3fa85f64-5717-4562-b3fc-2c963f66afa6"

// ─── KPIs con datos reales ────────────────────────────────────────────────────

export const mockKpisConDatos: ProgramaMetricasKpis = {
    totalClicks: 1250,
    totalPageViews: 890,
    totalSignups: 45,
    totalConversiones: 12,
    valorTotalGenerado: 1200,
    monedaNombre: "EUR",
    tasaConversion: 0.96,
    comisionesTotales: 120,
}

export const mockKpisVacios: ProgramaMetricasKpis = {
    totalClicks: 0,
    totalPageViews: 0,
    totalSignups: 0,
    totalConversiones: 0,
    valorTotalGenerado: 0,
    monedaNombre: null,
    tasaConversion: 0,
    comisionesTotales: 0,
}

// ─── Ranking promotores ────────────────────────────────────────────────────────

export const mockRankingItem1: RankingPromotorItem = {
    promotorId: "promotor-1",
    promotorNombre: "DJ Mark",
    tipoPromotorNombre: "Influencer",
    clicks: 450,
    pageViews: 320,
    signups: 20,
    conversiones: 5,
    valorGenerado: 500,
    comisionAcumulada: 50,
}

export const mockRankingItem2: RankingPromotorItem = {
    promotorId: "promotor-2",
    promotorNombre: "MusicBlog.es",
    tipoPromotorNombre: "Medio / Blog",
    clicks: 380,
    pageViews: 280,
    signups: 15,
    conversiones: 4,
    valorGenerado: 400,
    comisionAcumulada: 40,
}

export const mockRankingItem3: RankingPromotorItem = {
    promotorId: "promotor-3",
    promotorNombre: "FanLuna",
    tipoPromotorNombre: null,
    clicks: 300,
    pageViews: 200,
    signups: 8,
    conversiones: 3,
    valorGenerado: 300,
    comisionAcumulada: 30,
}

export const mockRankingItems: RankingPromotorItem[] = [
    mockRankingItem1,
    mockRankingItem2,
    mockRankingItem3,
]

// ─── Eventos por dia (serie temporal) ────────────────────────────────────────

export const mockEventosPorDia: EventosPorDiaItem[] = [
    { fecha: "2026-03-01", clicks: 45, pageViews: 32, signups: 5, conversiones: 1 },
    { fecha: "2026-03-02", clicks: 78, pageViews: 55, signups: 8, conversiones: 2 },
    { fecha: "2026-03-03", clicks: 62, pageViews: 41, signups: 6, conversiones: 1 },
    { fecha: "2026-03-04", clicks: 91, pageViews: 67, signups: 11, conversiones: 3 },
]

// ─── Response completo ─────────────────────────────────────────────────────────

export const mockProgramaMetricasResponse: ProgramaMetricasResponse = {
    programaId: PROGRAMA_ID,
    programaTitulo: "Promociona mi nuevo album",
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
    kpis: mockKpisConDatos,
    rankingPromotores: mockRankingItems,
    eventosPorDia: mockEventosPorDia,
}

export const mockProgramaMetricasResponseVacia: ProgramaMetricasResponse = {
    programaId: PROGRAMA_ID,
    programaTitulo: "Promociona mi nuevo album",
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
    kpis: mockKpisVacios,
    rankingPromotores: [],
    eventosPorDia: [],
}

// ─── Filtros ──────────────────────────────────────────────────────────────────

export const mockFiltroConFechas: FiltroFechas = {
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
}

export const mockFiltroVacio: FiltroFechas = {}

// ─── Builder helper ───────────────────────────────────────────────────────────

export function buildRankingItem(
    overrides: Partial<RankingPromotorItem> = {}
): RankingPromotorItem {
    return { ...mockRankingItem1, ...overrides }
}
```

### 3.2 Mock del Service (`metricasService`)

Pattern estandar del proyecto: `vi.mock("@/services/metricas.service")`.

```typescript
vi.mock("@/services/metricas.service", () => ({
    metricasService: {
        getProgramaMetricas: vi.fn(),
    },
}))
```

### 3.3 Mock de Recharts

`GraficoTemporal` usa Recharts, que requiere DOM y `ResizeObserver`. Dado que el grafico en si es una caja negra de terceros, se mockea Recharts en los tests del componente para verificar que el contenedor y el empty state se renderizan correctamente sin ejecutar el render real del canvas/SVG.

```typescript
vi.mock("recharts", () => ({
    ResponsiveContainer: ({ children }: { children: React.ReactNode }) => children,
    LineChart: ({ children }: { children: React.ReactNode }) =>
        React.createElement("div", { "data-testid": "line-chart" }, children),
    Line: () => null,
    XAxis: () => null,
    YAxis: () => null,
    CartesianGrid: () => null,
    Tooltip: () => null,
    Legend: () => null,
}))
```

### 3.4 Test Utilities

Los tests de componentes usan `render` y `screen` de `@/test-utils` (que ya incluye el `QueryClientProvider`).
Los tests de hooks usan `renderHook` + `createWrapper()` con un `QueryClient` fresco por test (patron de `use-inscripciones.test.ts`).

```typescript
// Para hooks - patron existente del proyecto
function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}
```

---

## 4. Tests por Modulo

### 4.1 metricas.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/metricas.service.test.ts`

**Tipo:** Unit Tests
**Mock principal:** `vi.mock("@/lib/api-client")` -> mock de `apiFetch`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | calls correct URL without date filters | Unit | Verifica que la URL construida es `/api/crowdpromotion/programas/{id}/metricas` sin query string cuando `filtro` es `{}` o `undefined` |
| 2 | appends fechaDesde when provided | Unit | Verifica que `?fechaDesde=2026-03-01` aparece en la URL cuando se pasa solo `fechaDesde` |
| 3 | appends fechaHasta when provided | Unit | Verifica que `?fechaHasta=2026-03-31` aparece en la URL cuando se pasa solo `fechaHasta` |
| 4 | appends both date params when both provided | Unit | Verifica que la URL contiene ambos params cuando se pasa `mockFiltroConFechas` |
| 5 | returns ProgramaMetricasResponse on success | Unit | Verifica que retorna `mockProgramaMetricasResponse` cuando `apiFetch` resuelve con `{ data: mockProgramaMetricasResponse, messages: [] }` |
| 6 | throws on 401 error response | Unit | Verifica que lanza `Error` cuando `messages` contiene errorCode `"3001"` |
| 7 | throws on 403 error response | Unit | Verifica que lanza `Error` cuando `messages` contiene errorCode `"3002"` (artista no propietario del programa) |
| 8 | throws on 404 error response | Unit | Verifica que lanza `Error` cuando `messages` contiene errorCode que representa not found (programaId invalido) |
| 9 | throws on 500 error response | Unit | Verifica que lanza `Error` cuando `messages` contiene errorCode `"5000"` |
| 10 | propagates network error | Unit | Verifica que relanza el `Error("Network error")` cuando `apiFetch` rechaza con error de red |
| 11 | does not append empty string fecha params | Unit | Verifica que si `fechaDesde` es `""` (string vacio) NO se agrega a la query string |
| 12 | uses API_ROUTES.crowdpromotion.programaMetricas | Unit | Verifica que llama a la funcion generadora de ruta con el `programaId` correcto |

**Assertions clave:**
- Verificar la URL construida con `vi.mocked(apiFetch).mock.calls[0][0]` (string)
- Verificar `.toContain("fechaDesde=2026-03-01")` y `.not.toContain("fechaDesde=")` para el caso vacio
- Verificar `await expect(metricasService.getProgramaMetricas(...)).rejects.toThrow()` para errores

---

### 4.2 use-programa-metricas.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-programa-metricas.test.ts`

**Tipo:** Unit + Integration Tests
**Mock principal:** `vi.mock("@/services/metricas.service")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | returns data on success without filter | Unit | Hook retorna `mockProgramaMetricasResponse` cuando el service resuelve. Verifica `result.current.isSuccess === true` y `result.current.data.programaId === PROGRAMA_ID` |
| 2 | returns data on success with date filter | Unit | Hook retorna datos cuando se pasa `mockFiltroConFechas`. Verifica que el service se llama con programaId y el filtro |
| 3 | passes programaId and filtro to service | Unit | Verifica `metricasService.getProgramaMetricas` fue llamado con `(PROGRAMA_ID, mockFiltroConFechas)` |
| 4 | handles isLoading state | Unit | `result.current.isLoading === true` cuando el service no ha resuelto aun (`new Promise(() => {})`) |
| 5 | handles isError state | Unit | `result.current.isError === true` cuando el service rechaza con `new Error("Fail")` |
| 6 | does not fetch when programaId is empty | Unit | `result.current.fetchStatus === "idle"` y el service no fue llamado cuando `programaId = ""` |
| 7 | does not retry on error | Unit | El service fue llamado exactamente 1 vez aunque `isError === true` |
| 8 | isFetching is true on refetch | Integration | Despues de resolver, llamar `result.current.refetch()` muestra `isFetching === true` |
| 9 | uses correct query key with dates | Unit | Al cambiar el filtro de fechas entre re-renders, el hook vuelve a llamar al service (nueva query key) |

**Setup:**
```typescript
vi.mock("@/services/metricas.service", () => ({
    metricasService: {
        getProgramaMetricas: vi.fn(),
    },
}))

const { result } = renderHook(
    () => useProgramaMetricas(PROGRAMA_ID, mockFiltroConFechas),
    { wrapper: createWrapper() }
)
await waitFor(() => expect(result.current.isSuccess).toBe(true))
```

---

### 4.3 use-sortable-table.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-sortable-table.test.ts`

**Tipo:** Unit Tests
**Mock principal:** Ninguno (hook puro sin dependencias externas)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | returns data sorted by defaultSortKey DESC | Unit | Inicializa con `data=mockRankingItems`, `defaultSortKey="conversiones"`, `defaultDirection="desc"`. Verifica que `sortedData[0].conversiones >= sortedData[1].conversiones` |
| 2 | returns data sorted ASC when defaultDirection is asc | Unit | `defaultDirection="asc"` produce que el primer elemento sea el de menor valor |
| 3 | handleSort toggles direction on same key | Unit | Llamar `handleSort("conversiones")` cuando ya esta ordenado por `conversiones` desc cambia a asc |
| 4 | handleSort resets to desc on new key | Unit | Llamar `handleSort("clicks")` cuando la columna activa es `"conversiones"` cambia la columna activa y resetea a `"desc"` |
| 5 | getSortIcon returns "asc" for active column ascending | Unit | Despues de ordenar asc por `"clicks"`, `getSortIcon("clicks")` devuelve `"asc"` |
| 6 | getSortIcon returns "desc" for active column descending | Unit | `getSortIcon("conversiones")` devuelve `"desc"` cuando la columna activa es `"conversiones"` desc |
| 7 | getSortIcon returns "none" for inactive column | Unit | `getSortIcon("promotorNombre")` devuelve `"none"` cuando la columna activa es `"conversiones"` |
| 8 | null/undefined values sort to end regardless of direction | Unit | Un item con `valorGenerado: null` aparece al final tanto en ordenacion asc como desc |
| 9 | returns empty array for empty input | Unit | `data=[]` -> `sortedData = []` sin errores |
| 10 | re-sorts when data changes | Unit | Al actualizar `data` con un nuevo item, `sortedData` refleja el nuevo ordenamiento |

**Setup:**
```typescript
// Hook puro, usar renderHook sin wrapper de QueryClient
const { result } = renderHook(() =>
    useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
)
```

**Assertions clave:**
- `result.current.sortedData[0].conversiones` para verificar ordenacion
- `act(() => result.current.handleSort("clicks"))` para disparar cambios de estado
- `result.current.getSortIcon("clicks")` para verificar estado del icono

---

### 4.4 KpiCard.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/KpiCard.test.tsx`

**Tipo:** Unit Tests
**Mock principal:** Ninguno

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders label text | Unit | Verifica que `screen.getByText("Total Clicks")` esta en el documento |
| 2 | renders numeric value formatted with toLocaleString | Unit | Value `1250` se muestra como `"1.250"` (locale es-ES con separador de miles) |
| 3 | renders string value as-is | Unit | Value `"0.96%"` se muestra literalmente |
| 4 | renders suffix when provided | Unit | `suffix="EUR"` aparece en el documento |
| 5 | renders note when provided | Unit | `note="conversiones / clicks"` aparece en el documento |
| 6 | applies valueColorClass to value element | Unit | El elemento que contiene el valor tiene la clase `text-[#f59e0b]` cuando se pasa `valueColorClass="text-[#f59e0b]"` |
| 7 | does not render suffix when not provided | Unit | Sin prop `suffix`, no hay elemento con texto "EUR" en el documento |
| 8 | does not render note when not provided | Unit | Sin prop `note`, no hay elemento con texto "conversiones / clicks" |

**Props base para tests:**
```typescript
const defaultProps = {
    label: "Total Clicks",
    value: 1250,
    icon: MousePointerClick,
    iconBgClass: "bg-blue-950/50",
    iconColorClass: "text-[#3b82f6]",
}
```

---

### 4.5 KpiCardsGrid.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/KpiCardsGrid.test.tsx`

**Tipo:** Unit Tests
**Mock principal:** Ninguno (componente servidor sin hooks)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders 4 primary KPI labels | Unit | Verifica la presencia de "Clicks totales", "Registros", "Backings" y "Valor generado" |
| 2 | renders 2 secondary KPI labels | Unit | Verifica la presencia de "Tasa de conversion" y "Comisiones" (o los textos exactos del componente) |
| 3 | renders totalClicks formatted | Unit | `kpis.totalClicks = 1250` se muestra como `"1.250"` |
| 4 | renders totalConversiones | Unit | `kpis.totalConversiones = 12` aparece en el documento |
| 5 | renders valorTotalGenerado in amber color | Unit | El elemento que contiene `"1.200"` tiene clase con color ambar (`text-[#f59e0b]`) |
| 6 | renders monedaNombre as suffix | Unit | Cuando `monedaNombre = "EUR"`, aparece `"EUR"` en el documento |
| 7 | renders EUR fallback when monedaNombre is null | Unit | Cuando `monedaNombre = null`, sigue apareciendo `"EUR"` (fallback del componente) |
| 8 | renders tasaConversion formatted | Unit | `tasaConversion = 0.96` se muestra como `"0.96%"` (via `formatTasaConversion`) |
| 9 | renders comisionesTotales | Unit | `comisionesTotales = 120` aparece en el documento |
| 10 | renders zeros correctly for empty kpis | Unit | Con `mockKpisVacios`, los valores `"0"` y `"0.00%"` estan en el documento |

**Setup:**
```typescript
render(<KpiCardsGrid kpis={mockKpisConDatos} />)
```

---

### 4.6 FiltroFechas.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/FiltroFechas.test.tsx`

**Tipo:** Integration Tests
**Mock principal:** Ninguno

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders both date inputs | Unit | Los inputs con `aria-label="Fecha de inicio"` y `aria-label="Fecha de fin"` estan en el documento |
| 2 | renders Aplicar button enabled by default | Unit | El boton con texto "Aplicar" esta en el documento y no tiene atributo `disabled` |
| 3 | hides Limpiar button when tieneFiltroPeriodo is false | Unit | Con `tieneFiltroPeriodo={false}`, no existe el boton "Limpiar" |
| 4 | shows Limpiar button when tieneFiltroPeriodo is true | Unit | Con `tieneFiltroPeriodo={true}`, el boton "Limpiar" es visible |
| 5 | shows active filter badge when tieneFiltroPeriodo is true | Unit | Con `tieneFiltroPeriodo={true}`, aparece el badge que contiene "Filtrando:" |
| 6 | calls onAplicar when Aplicar is clicked | Integration | `userEvent.click(getByRole("button", { name: /Aplicar/i }))` -> `onAplicar` fue llamado 1 vez |
| 7 | calls onLimpiar when Limpiar button is clicked | Integration | Con `tieneFiltroPeriodo={true}`, `userEvent.click` en "Limpiar" llama `onLimpiar` |
| 8 | calls onLimpiar when X in badge is clicked | Integration | Click en el boton `aria-label="Quitar filtro de fechas"` llama `onLimpiar` |
| 9 | shows validation error when fechaDesde > fechaHasta | Integration | Con `fechaDesde="2026-03-31"` y `fechaHasta="2026-03-01"`, aparece el mensaje de error y el boton "Aplicar" se deshabilita |
| 10 | disables Aplicar button when isLoading is true | Unit | Con `isLoading={true}`, el boton "Aplicar" tiene `disabled` |
| 11 | shows Loader2 spinner in Aplicar when isLoading | Unit | Con `isLoading={true}`, existe un elemento con clase `animate-spin` dentro del boton |
| 12 | calls onFechaDesdeChange on input change | Integration | `userEvent.type` en el input de fecha inicio llama `onFechaDesdeChange` con el nuevo valor |

**Props base:**
```typescript
const defaultProps = {
    fechaDesde: "",
    fechaHasta: "",
    onFechaDesdeChange: vi.fn(),
    onFechaHastaChange: vi.fn(),
    onAplicar: vi.fn(),
    onLimpiar: vi.fn(),
    tieneFiltroPeriodo: false,
    isLoading: false,
}
```

---

### 4.7 RankingPromotoresTable.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/RankingPromotoresTable.test.tsx`

**Tipo:** Unit + Integration Tests
**Mock principal:** Ninguno (componente cliente con estado local via `useSortableTable`)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders promotor names | Unit | Los nombres "DJ Mark", "MusicBlog.es" y "FanLuna" aparecen en la tabla |
| 2 | renders position numbers | Unit | Los textos "#1", "#2", "#3" aparecen como celdas de posicion |
| 3 | renders tipo promotor badge | Unit | "Influencer" aparece como badge |
| 4 | renders fallback for null tipoPromotorNombre | Unit | Cuando `tipoPromotorNombre = null`, no hay badge para ese promotor (o se renderiza un badge vacio segun diseno) |
| 5 | renders clicks value for each row | Unit | `"450"` (clicks de DJ Mark) aparece en una celda |
| 6 | renders conversiones value | Unit | `"5"` (conversiones de DJ Mark) aparece en el documento |
| 7 | renders valorGenerado with EUR | Unit | `"500"` y `"EUR"` (o el formato acordado) aparecen para la fila de DJ Mark |
| 8 | renders comisionAcumulada | Unit | `"50"` aparece en el documento para DJ Mark |
| 9 | renders Avatar fallback initials | Unit | El avatar de "DJ Mark" muestra la inicial "D" en el fallback |
| 10 | renders sortable column headers | Unit | Las cabeceras "Clicks", "Conv.", "Valor" y "Comision" tienen el atributo `aria-sort` |
| 11 | sorts by clicks DESC on header click | Integration | Click en cabecera "Clicks" -> el promotor con mas clicks aparece primero en el orden siguiente |
| 12 | toggles sort direction on second click of same column | Integration | Dos clicks en "Clicks" -> la primera fila cambia de ser la de mayor a la de menor clicks |
| 13 | sorts by conversiones DESC by default | Unit | Sin interaccion, el orden de filas sigue la ordenacion por `conversiones` desc (`mockRankingItem1` primero con 5 conversiones) |
| 14 | renders empty state when items is empty | Unit | Con `items={[]}`, aparece el texto "Sin promotores con actividad en este periodo" |
| 15 | empty state occupies full row width | Unit | La celda del empty state tiene `colSpan={6}` (verificar con `closest("td")`) |

**Setup:**
```typescript
import userEvent from "@testing-library/user-event"
render(<RankingPromotoresTable items={mockRankingItems} />)
```

---

### 4.8 DesgloseEventosPanel.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/DesgloseEventosPanel.test.tsx`

**Tipo:** Unit Tests
**Mock principal:** Ninguno (componente servidor sin hooks)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders Clicks label and count | Unit | "Clicks" y `"1.250"` (o `"1250"` segun locale) aparecen en el documento |
| 2 | renders Page Views label and count | Unit | "Page Views" y `"890"` aparecen |
| 3 | renders Signups label and count | Unit | "Signups" y `"45"` aparecen |
| 4 | renders Conversiones label and count | Unit | "Conversiones" y `"12"` aparecen |
| 5 | renders progress bars (one per event type) | Unit | Existen 4 elementos de barra de progreso en el documento |
| 6 | clicks bar has largest width | Unit | La barra de Clicks tiene el `style.width` mayor (ya que totalClicks=1250 es el mayor) |
| 7 | renders bars with zero width when all kpis are zero | Unit | Con `mockKpisVacios`, todas las barras tienen `style.width = "0%"` |
| 8 | calculates percentage correctly | Unit | Para kpis con total=2197 eventos, la barra de Clicks tiene `style.width` aproximadamente `"56.9%"` (1250/2197) |

**Calculo del total para tests:**
`totalClicks(1250) + totalPageViews(890) + totalSignups(45) + totalConversiones(12) = 2197`

---

### 4.9 GraficoTemporal.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/GraficoTemporal.test.tsx`

**Tipo:** Unit Tests
**Mock principal:** `vi.mock("recharts")` (ver seccion 3.3)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders LineChart container when datos has items | Unit | El elemento con `data-testid="line-chart"` esta en el documento cuando `datos = mockEventosPorDia` |
| 2 | renders empty state when datos is empty | Unit | Con `datos={[]}`, aparece el texto "No hay datos para el periodo seleccionado" |
| 3 | does not render empty state when datos has items | Unit | Con `datos = mockEventosPorDia`, el texto del empty state NO aparece |
| 4 | renders accessible sr-only description | Unit | Existe un elemento con clase `sr-only` que contiene texto descriptivo del grafico |
| 5 | renders section title | Unit | El titulo "Eventos por dia" aparece en el documento |

**Nota:** No se testean las propiedades individuales de los subcomponentes de Recharts (`<Line>`, `<XAxis>`, etc.) porque estan mockeados. Lo que se testea es la logica de renderizado condicional del componente en si.

---

### 4.10 ProgramaMetricasTab.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/__tests__/ProgramaMetricasTab.test.tsx`

**Tipo:** Integration Tests (principal)
**Mocks:**
- `vi.mock("@/services/metricas.service")` - para controlar la respuesta del hook
- `vi.mock("recharts")` - para evitar problemas con ResizeObserver en Recharts
- `vi.mock("next/navigation")` - para mockear `useSearchParams` y `useRouter`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders KPI values when data loads successfully | Integration | Con `metricasService.getProgramaMetricas` resolviendo `mockProgramaMetricasResponse`, aparecen los valores "1.250", "45", "12" y "1.200" en el documento |
| 2 | renders ranking promotor names | Integration | Los nombres "DJ Mark" y "MusicBlog.es" aparecen en la tabla de ranking |
| 3 | renders desglose panel labels | Integration | El texto "Clicks" del panel de desglose esta visible |
| 4 | renders grafico container | Integration | El `data-testid="line-chart"` del mock de Recharts esta en el documento |
| 5 | shows skeleton while loading | Integration | Mientras el service no ha resuelto, existen elementos con clase `animate-pulse` en el documento |
| 6 | shows error state on service failure | Integration | Cuando el service rechaza con `new Error("Fail")`, aparece el texto "No se pudieron cargar las metricas" o similar |
| 7 | shows retry button on error | Integration | Con error, existe un boton "Reintentar" clicable |
| 8 | renders FiltroFechas component | Integration | Los inputs con `aria-label="Fecha de inicio"` y `aria-label="Fecha de fin"` estan en el documento |
| 9 | clicking Aplicar updates URL search params | Integration | Al rellenar las fechas y hacer click en "Aplicar", `router.push` o `router.replace` fue llamado con los nuevos query params |
| 10 | clicking Limpiar removes date params from URL | Integration | Con `tieneFiltroPeriodo=true`, click en "Limpiar" llama al router para remover `fechaDesde` y `fechaHasta` |
| 11 | renders empty state for ranking when rankingPromotores is empty | Integration | Con `mockProgramaMetricasResponseVacia`, aparece "Sin promotores con actividad en este periodo" |
| 12 | renders empty state for grafico when eventosPorDia is empty | Integration | Con `mockProgramaMetricasResponseVacia`, aparece "No hay datos para el periodo seleccionado" |
| 13 | reads initial filter from URL search params | Integration | Si `useSearchParams` devuelve `fechaDesde=2026-03-01`, el service se llama con ese filtro |

**Setup para mockear next/navigation:**
```typescript
vi.mock("next/navigation", () => ({
    useSearchParams: vi.fn(() => ({
        get: vi.fn(() => null),
        toString: vi.fn(() => ""),
    })),
    useRouter: vi.fn(() => ({
        replace: vi.fn(),
        push: vi.fn(),
    })),
}))
```

**Assertions clave:**
- Usar `await screen.findByText("1.250")` (async, espera resolucion de query)
- Verificar que los hijos se renderizan como orquestador

---

## 5. Plan de Cobertura por Archivo

| Archivo | Tests Asignados | Lineas Target | Funciones Target | Branches Target |
|---------|-----------------|---------------|-----------------|-----------------|
| `metricas.service.ts` | 12 unit | 95% | 100% | 90% |
| `use-programa-metricas.ts` | 9 unit/integration | 90% | 100% | 85% |
| `use-sortable-table.ts` | 10 unit | 95% | 100% | 90% |
| `KpiCard.tsx` | 8 unit | 85% | 90% | 80% |
| `KpiCardsGrid.tsx` | 10 unit | 85% | 90% | 80% |
| `FiltroFechas.tsx` | 12 integration | 85% | 90% | 85% |
| `RankingPromotoresTable.tsx` | 15 unit/integration | 85% | 90% | 80% |
| `DesgloseEventosPanel.tsx` | 8 unit | 85% | 90% | 80% |
| `GraficoTemporal.tsx` | 5 unit | 80% | 85% | 80% |
| `ProgramaMetricasTab.tsx` | 13 integration | 80% | 85% | 75% |

**Meta Global:** 80% en lineas, funciones y branches

---

## 6. Acceptance Criteria Cubiertos por Tests

| AC | Criterio | Test que lo cubre |
|----|----------|-------------------|
| AC-CP05-9 | Dashboard muestra KPIs globales (totalClicks, totalPageViews, totalSignups, totalConversiones, valorTotalGenerado, tasaConversion, comisionesTotales) | `KpiCardsGrid.test.tsx` casos 1-9, `ProgramaMetricasTab.test.tsx` caso 1 |
| AC-CP05-10 | Dashboard soporta filtrado por rango de fechas; KPIs y ranking recalculados al cambiar rango | `FiltroFechas.test.tsx` casos 6, 9; `ProgramaMetricasTab.test.tsx` casos 9, 10, 13; `metricas.service.test.ts` casos 2-4; `use-programa-metricas.test.ts` casos 2, 3, 9 |
| AC-CP05-11 | Ranking de promotores con nombre, tipo, clicks, conversiones, valor, comision; ordenado por conversiones desc | `RankingPromotoresTable.test.tsx` casos 1-10, 13; `ProgramaMetricasTab.test.tsx` caso 2 |

---

## 7. Casos Edge y Errores Planificados

### Edge Cases Especificos de Esta Feature

| Caso | Componente/Hook | Test Archivo | Test Case # |
|------|----------------|--------------|-------------|
| `monedaNombre = null` -> mostrar fallback "EUR" | `KpiCardsGrid` | `KpiCardsGrid.test.tsx` | 7 |
| `tipoPromotorNombre = null` -> no badge o badge vacio | `RankingPromotoresTable` | `RankingPromotoresTable.test.tsx` | 4 |
| `rankingPromotores = []` -> empty state en tabla | `RankingPromotoresTable` | `RankingPromotoresTable.test.tsx` | 14, 15 |
| `eventosPorDia = []` -> empty state en grafico | `GraficoTemporal` | `GraficoTemporal.test.tsx` | 2 |
| Todos los KPIs en 0 | `KpiCardsGrid`, `DesgloseEventosPanel` | `.test.tsx` casos con `mockKpisVacios` | `KpiCardsGrid` caso 10, `DesgloseEventosPanel` caso 7 |
| `fechaDesde > fechaHasta` -> boton Aplicar deshabilitado | `FiltroFechas` | `FiltroFechas.test.tsx` | 9 |
| `fechaDesde = ""` (vacio) -> no agregar al query string | `metricasService` | `metricas.service.test.ts` | 11 |
| `programaId = ""` -> query no ejecutada | `useProgramaMetricas` | `use-programa-metricas.test.ts` | 6 |
| 403 (artista no propietario) -> lanzar error | `metricasService` | `metricas.service.test.ts` | 7 |
| Error de red -> propagar error | `metricasService` | `metricas.service.test.ts` | 10 |
| Valores `null` en ordenacion -> siempre al final | `useSortableTable` | `use-sortable-table.test.ts` | 8 |
| Calculo de porcentaje con total=0 -> no dividir por 0 | `DesgloseEventosPanel` | `DesgloseEventosPanel.test.tsx` | 7 |

---

## 8. Integracion con Tests Existentes

### Tests de Regresion No Requeridos

Esta feature es **aditiva**: agrega una nueva pestana "Metricas" al `PromoProgramaDetailClient` existente. Los tests existentes de las otras pestanas (Solicitudes, Aprobados, Bloqueados, Tareas) no deben verse afectados siempre que la modificacion de `PromoProgramaDetailClient` no rompa el renderizado de las otras pestanas.

**Verificacion recomendada:** Ejecutar los tests existentes de `PromoProgramaDetailClient` y tabs adyacentes despues de agregar la pestana "Metricas" para confirmar que siguen pasando.

### Archivos Existentes que Pueden Verse Afectados

| Archivo Afectado | Causa | Accion |
|-----------------|-------|--------|
| `PromoProgramaHeader.test.tsx` | El header es hijo de `PromoProgramaDetailClient` | Verificar que sigue pasando sin cambios |
| `PromoProgramaSolicitudesTab.test.tsx` | La pestana "Solicitudes" sigue existiendo | Verificar que sigue pasando sin cambios |

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del admin
cd src/admin && npm run test

# Ejecutar con coverage
cd src/admin && npm run test:coverage

# Ejecutar solo los tests de la feature cp-tracking-metricas (admin)
cd src/admin && npm run test -- --reporter=verbose metricas

# Ejecutar solo los tests de un archivo especifico
cd src/admin && npm run test -- services/__tests__/metricas.service.test.ts

# Watch mode durante desarrollo
cd src/admin && npm run test:watch
```

---

## 10. Checklist

- [ ] Archivo `__mocks__/cp-tracking-metricas.mock.ts` creado con todos los fixtures
- [ ] `metricas.service.test.ts` - 12 tests (service con URL y error handling)
- [ ] `use-programa-metricas.test.ts` - 9 tests (query hook con filtros de fecha)
- [ ] `use-sortable-table.test.ts` - 10 tests (hook puro de ordenacion)
- [ ] `KpiCard.test.tsx` - 8 tests (card individual con variantes)
- [ ] `KpiCardsGrid.test.tsx` - 10 tests (grid de 6 KPIs con formato)
- [ ] `FiltroFechas.test.tsx` - 12 tests (formulario de fechas con validacion)
- [ ] `RankingPromotoresTable.test.tsx` - 15 tests (tabla con ordenacion interactiva)
- [ ] `DesgloseEventosPanel.test.tsx` - 8 tests (panel con barras de progreso)
- [ ] `GraficoTemporal.test.tsx` - 5 tests (mock de Recharts + empty state)
- [ ] `ProgramaMetricasTab.test.tsx` - 13 tests (orquestador con todos los estados)
- [ ] `vi.mock("recharts")` configurado en tests que usan `GraficoTemporal`
- [ ] `vi.mock("next/navigation")` configurado en `ProgramaMetricasTab.test.tsx`
- [ ] Cobertura 80%+ verificada con `npm run test:coverage`
- [ ] Todos los tests pasan en menos de 55 segundos
