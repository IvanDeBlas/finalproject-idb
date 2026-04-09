# Estrategia de Testing: Dashboard de Artista (Admin)

**Fecha:** 2026-02-14
**Feature:** dashboard-artista
**Target:** src/admin (Next.js 14)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 28 | 85% |
| Integration Tests | 12 | 75% |
| Accessibility Tests | 6 | 100% |
| **Total** | **46** | **80%+** |

**Tiempo estimado de ejecucion:** < 30 segundos
**Testing stack:** Vitest + Testing Library + MSW (opcional)

---

## 2. Estructura de Tests

```
src/admin/src/
├── app/(dashboard)/dashboard/
│   └── __tests__/
│       ├── DashboardPage.test.tsx
│       └── components/
│           ├── StatsOverview.test.tsx
│           ├── CampaignCard.test.tsx
│           ├── CampaignsGrid.test.tsx
│           └── EmptyDashboardState.test.tsx
│
├── components/dashboard/
│   └── __tests__/
│       ├── StatsCard.test.tsx
│       ├── ProgressBar.test.tsx
│       └── RecentBackings.test.tsx
│
├── hooks/
│   └── __tests__/
│       ├── useDashboardResumen.test.ts
│       ├── useMisCampanias.test.ts
│       ├── useCampaniaBackings.test.ts
│       └── useCampaniaStats.test.ts
│
└── __mocks__/
    ├── dashboard.mock.ts
    └── handlers.ts (opcional)
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/dashboard.mock.ts`

```typescript
import type {
    DashboardResumen,
    MiCampaniaListItem,
    CampaniaBackingList,
    CampaniaStatsDetail,
} from "@shared/types"

// Dashboard Resumen Mock
export const mockDashboardResumen: DashboardResumen = {
    artistaId: "artista-123",
    nombreArtistico: "Usuario 1 Music Actualizado",
    totalRecaudado: 15340.50,
    totalBackers: 487,
    campaniasActivas: 2,
    campaniasCompletadas: 3,
    totalCampanias: 5,
    monedaSimbolo: "EUR",
    fechaUltimoAporte: "2026-02-14T10:30:00Z",
}

export const mockDashboardResumenEmpty: DashboardResumen = {
    artistaId: "artista-123",
    nombreArtistico: "Usuario 1 Music Actualizado",
    totalRecaudado: 0,
    totalBackers: 0,
    campaniasActivas: 0,
    campaniasCompletadas: 0,
    totalCampanias: 0,
    monedaSimbolo: "EUR",
    fechaUltimoAporte: undefined,
}

// Mis Campanias Mock
export const mockMisCampanias: MiCampaniaListItem[] = [
    {
        id: "campania-1",
        titulo: "Mi Album Debut",
        imagenPrincipalUrl: "https://example.com/album.jpg",
        estadoCampaniaId: 2,
        estadoCampaniaNombre: "Publicada",
        importeObjetivo: 5000.00,
        importeRecaudado: 2340.50,
        porcentajeProgreso: 46.81,
        numBackers: 78,
        diasRestantes: 46,
        fechaFin: "2026-03-31T23:59:59Z",
        fechaCreacion: "2026-01-15T12:00:00Z",
    },
    {
        id: "campania-2",
        titulo: "Gira Nacional 2026",
        imagenPrincipalUrl: "https://example.com/gira.jpg",
        estadoCampaniaId: 2,
        estadoCampaniaNombre: "Publicada",
        importeObjetivo: 10000.00,
        importeRecaudado: 7890.00,
        porcentajeProgreso: 78.90,
        numBackers: 234,
        diasRestantes: 15,
        fechaFin: "2026-03-01T23:59:59Z",
        fechaCreacion: "2026-01-10T09:00:00Z",
    },
    {
        id: "campania-3",
        titulo: "EP Acustico",
        imagenPrincipalUrl: null,
        estadoCampaniaId: 1,
        estadoCampaniaNombre: "Borrador",
        importeObjetivo: 3000.00,
        importeRecaudado: 0.00,
        porcentajeProgreso: 0.00,
        numBackers: 0,
        diasRestantes: null,
        fechaFin: null,
        fechaCreacion: "2026-02-10T09:00:00Z",
    },
]

// Campania Backings Mock
export const mockCampaniaBackings: CampaniaBackingList = {
    campaniaId: "campania-1",
    campaniaTitulo: "Mi Album Debut",
    stats: {
        totalRecaudado: 2340.50,
        backingPromedio: 30.01,
        totalBackers: 78,
        rewardMasPopular: "CD Fisico Firmado",
        ultimoBacking: {
            nombreBacker: "Maria Lopez",
            monto: 25.00,
            fechaCreacion: "2026-02-14T10:30:00Z",
        },
    },
    backings: {
        items: [
            {
                id: "backing-1",
                nombreBacker: "Maria Lopez",
                email: "maria.lopez@example.com",
                monto: 25.00,
                rewardNombre: "CD Fisico Firmado",
                mensaje: "Mucha suerte con el proyecto!",
                esAnonimo: false,
                estadoPedido: "Completado",
                fechaCreacion: "2026-02-14T10:30:00Z",
            },
            {
                id: "backing-2",
                nombreBacker: "Anonimo",
                email: null,
                monto: 50.00,
                rewardNombre: null,
                mensaje: null,
                esAnonimo: true,
                estadoPedido: "Completado",
                fechaCreacion: "2026-02-13T09:15:00Z",
            },
            {
                id: "backing-3",
                nombreBacker: "Carlos Sanchez",
                email: "carlos@example.com",
                monto: 100.00,
                rewardNombre: "Vinilo Limitado",
                mensaje: null,
                esAnonimo: false,
                estadoPedido: "Completado",
                fechaCreacion: "2026-02-12T15:45:00Z",
            },
        ],
        totalCount: 78,
        page: 1,
        pageSize: 20,
        totalPages: 4,
    },
}

// Campania Stats Mock
export const mockCampaniaStats: CampaniaStatsDetail = {
    campaniaId: "campania-1",
    campaniaTitulo: "Mi Album Debut",
    importeObjetivo: 5000.00,
    importeRecaudado: 2340.50,
    porcentajeProgreso: 46.81,
    numBackers: 78,
    backingPromedio: 30.01,
    diasRestantes: 46,
    diasTranscurridos: 14,
    totalDiasCampania: 60,
    proyeccionFinal: 10029.64,
    velocidadDiaria: 167.18,
    rewardStats: [
        {
            rewardId: "reward-1",
            rewardNombre: "Descarga Digital",
            cantidadVendida: 45,
            totalRecaudado: 450.00,
            porcentajeDelTotal: 19.23,
        },
        {
            rewardId: "reward-2",
            rewardNombre: "CD Fisico Firmado",
            cantidadVendida: 33,
            totalRecaudado: 825.00,
            porcentajeDelTotal: 35.25,
        },
        {
            rewardId: null,
            rewardNombre: "Sin recompensa",
            cantidadVendida: 0,
            totalRecaudado: 1065.50,
            porcentajeDelTotal: 45.52,
        },
    ],
    progressoPorDia: [
        {
            fecha: "2026-02-01",
            numBackings: 12,
            totalRecaudado: 340.00,
            acumulado: 340.00,
        },
        {
            fecha: "2026-02-02",
            numBackings: 8,
            totalRecaudado: 210.00,
            acumulado: 550.00,
        },
    ],
}
```

### 3.2 MSW Handlers (Opcional)

**Archivo:** `src/admin/src/__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from "msw"
import {
    mockDashboardResumen,
    mockMisCampanias,
    mockCampaniaBackings,
    mockCampaniaStats,
} from "./dashboard.mock"

const API_BASE_URL = "http://localhost:5001/api"

export const dashboardHandlers = [
    // GET /api/dashboard/resumen
    http.get(`${API_BASE_URL}/dashboard/resumen`, () => {
        return HttpResponse.json({
            data: mockDashboardResumen,
            messages: [
                {
                    message: "Resumen obtenido",
                    errorCode: "0000",
                },
            ],
        })
    }),

    // GET /api/campanias/mis-campanias
    http.get(`${API_BASE_URL}/campanias/mis-campanias`, () => {
        return HttpResponse.json({
            data: {
                items: mockMisCampanias,
                totalCount: mockMisCampanias.length,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            messages: [
                {
                    message: "Campanias encontradas",
                    errorCode: "0000",
                },
            ],
        })
    }),

    // GET /api/campanias/:id/backings
    http.get(`${API_BASE_URL}/campanias/:id/backings`, ({ params }) => {
        return HttpResponse.json({
            data: mockCampaniaBackings,
            messages: [
                {
                    message: "Aportes encontrados",
                    errorCode: "0000",
                },
            ],
        })
    }),

    // GET /api/campanias/:id/stats
    http.get(`${API_BASE_URL}/campanias/:id/stats`, () => {
        return HttpResponse.json({
            data: mockCampaniaStats,
            messages: [
                {
                    message: "Estadisticas obtenidas",
                    errorCode: "0000",
                },
            ],
        })
    }),

    // Error handler - 401 Unauthorized
    http.get(`${API_BASE_URL}/dashboard/resumen-unauthorized`, () => {
        return HttpResponse.json(
            {
                data: null,
                messages: [
                    {
                        message: "Token no valido o expirado",
                        errorCode: "3001",
                    },
                ],
            },
            { status: 401 }
        )
    }),

    // Error handler - 403 Forbidden
    http.get(`${API_BASE_URL}/campanias/:id/backings-forbidden`, () => {
        return HttpResponse.json(
            {
                data: null,
                messages: [
                    {
                        message: "No tienes permiso para ver estos aportes",
                        errorCode: "3002",
                    },
                ],
            },
            { status: 403 }
        )
    }),
]
```

### 3.3 Test Utilities

Ya existe en `src/admin/src/test-utils.tsx`:

```typescript
import { render, type RenderOptions } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactElement, ReactNode } from "react"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
}

function AllProviders({ children }: { children: ReactNode }) {
    const queryClient = createTestQueryClient()
    return (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    )
}

function customRender(
    ui: ReactElement,
    options?: Omit<RenderOptions, "wrapper">
) {
    return render(ui, { wrapper: AllProviders, ...options })
}

export * from "@testing-library/react"
export { customRender as render }
```

---

## 4. Tests por Modulo

### 4.1 Components - StatsCard

**Archivo:** `src/admin/src/components/dashboard/__tests__/StatsCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with basic props | Unit | Renderiza titulo y valor correctamente |
| displays icon correctly | Unit | Muestra el icono pasado como prop |
| shows trend when provided | Unit | Muestra trend positivo con color verde |
| shows negative trend | Unit | Muestra trend negativo con color rojo |
| shows description when provided | Unit | Muestra descripcion adicional |
| applies custom className | Unit | Aplica className customizado |
| renders without trend | Unit | Funciona sin prop trend |
| renders without description | Unit | Funciona sin prop description |

**Casos Detallados:**

```typescript
describe("StatsCard", () => {
    it("renders with basic props", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15,340.50 EUR"
                icon={Euro}
            />
        )

        expect(screen.getByText("Total Recaudado")).toBeInTheDocument()
        expect(screen.getByText("15,340.50 EUR")).toBeInTheDocument()
    })

    it("displays icon correctly", () => {
        const { container } = render(
            <StatsCard
                title="Total Recaudado"
                value="15,340.50 EUR"
                icon={Euro}
            />
        )

        const icon = container.querySelector("svg")
        expect(icon).toBeInTheDocument()
        expect(icon).toHaveClass("h-4", "w-4")
    })

    it("shows trend when provided", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15,340.50 EUR"
                icon={Euro}
                trend={{ value: 12.5, isPositive: true }}
            />
        )

        const trendText = screen.getByText(/\+12.5%/)
        expect(trendText).toBeInTheDocument()
        expect(trendText).toHaveClass("text-green-600")
    })

    it("shows negative trend", () => {
        render(
            <StatsCard
                title="Backers"
                value="487"
                icon={Users}
                trend={{ value: 5.3, isPositive: false }}
            />
        )

        const trendText = screen.getByText(/-5.3%/)
        expect(trendText).toBeInTheDocument()
        expect(trendText).toHaveClass("text-red-600")
    })

    it("shows description when provided", () => {
        render(
            <StatsCard
                title="Campanias Activas"
                value="2"
                icon={TrendingUp}
                description="Campanias publicadas actualmente"
            />
        )

        expect(
            screen.getByText("Campanias publicadas actualmente")
        ).toBeInTheDocument()
    })

    it("applies custom className", () => {
        const { container } = render(
            <StatsCard
                title="Test"
                value="123"
                icon={Euro}
                className="custom-class"
            />
        )

        const card = container.firstChild
        expect(card).toHaveClass("custom-class")
    })

    it("renders without trend", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15,340.50 EUR"
                icon={Euro}
            />
        )

        expect(screen.queryByText(/%/)).not.toBeInTheDocument()
    })

    it("renders without description", () => {
        const { container } = render(
            <StatsCard
                title="Total Recaudado"
                value="15,340.50 EUR"
                icon={Euro}
            />
        )

        const descriptions = container.querySelectorAll(
            ".text-muted-foreground"
        )
        // Solo el titulo debe tener text-muted-foreground
        expect(descriptions.length).toBe(1)
    })
})
```

---

### 4.2 Components - ProgressBar

**Archivo:** `src/admin/src/components/dashboard/__tests__/ProgressBar.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with percentage | Unit | Muestra porcentaje calculado correctamente |
| applies success color when >= 100% | Unit | Verde cuando progreso es 100% o mas |
| applies warning color when >= 50% | Unit | Amarillo cuando progreso entre 50-99% |
| applies danger color when < 50% | Unit | Rojo cuando progreso menor a 50% |
| handles 0% progress | Unit | Maneja correctamente progreso en 0% |
| handles > 100% progress | Unit | Maneja correctamente progreso mayor a 100% |
| shows percentage label when enabled | Unit | Muestra label de porcentaje |
| hides percentage label when disabled | Unit | Oculta label si showPercentage=false |
| renders with custom className | Unit | Aplica className customizado |

**Casos Detallados:**

```typescript
describe("ProgressBar", () => {
    it("renders with percentage", () => {
        render(<ProgressBar current={2340.50} goal={5000.00} />)

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toBeInTheDocument()
        expect(progressBar).toHaveAttribute("aria-valuenow", "46.81")
        expect(progressBar).toHaveAttribute("aria-valuemin", "0")
        expect(progressBar).toHaveAttribute("aria-valuemax", "100")
    })

    it("applies success color when >= 100%", () => {
        render(
            <ProgressBar current={5500} goal={5000} showPercentage={true} />
        )

        const progressFill = screen.getByRole("progressbar").firstChild
        expect(progressFill).toHaveClass("bg-green-500")
    })

    it("applies warning color when >= 50%", () => {
        render(<ProgressBar current={3000} goal={5000} />)

        const progressFill = screen.getByRole("progressbar").firstChild
        expect(progressFill).toHaveClass("bg-yellow-500")
    })

    it("applies danger color when < 50%", () => {
        render(<ProgressBar current={1000} goal={5000} />)

        const progressFill = screen.getByRole("progressbar").firstChild
        expect(progressFill).toHaveClass("bg-red-500")
    })

    it("handles 0% progress", () => {
        render(<ProgressBar current={0} goal={5000} showPercentage={true} />)

        expect(screen.getByText("0.00%")).toBeInTheDocument()
    })

    it("handles > 100% progress", () => {
        render(<ProgressBar current={7500} goal={5000} showPercentage={true} />)

        expect(screen.getByText("150.00%")).toBeInTheDocument()
    })

    it("shows percentage label when enabled", () => {
        render(
            <ProgressBar current={2340.50} goal={5000} showPercentage={true} />
        )

        expect(screen.getByText("46.81%")).toBeInTheDocument()
    })

    it("hides percentage label when disabled", () => {
        render(
            <ProgressBar current={2340.50} goal={5000} showPercentage={false} />
        )

        expect(screen.queryByText("46.81%")).not.toBeInTheDocument()
    })

    it("renders with custom className", () => {
        const { container } = render(
            <ProgressBar
                current={2340.50}
                goal={5000}
                className="custom-progress"
            />
        )

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toHaveClass("custom-progress")
    })
})
```

---

### 4.3 Components - CampaignCard

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/components/CampaignCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders campania title | Unit | Muestra titulo de campania |
| displays campania image | Unit | Muestra imagen principal |
| shows fallback image when null | Unit | Muestra placeholder si no hay imagen |
| renders estado badge | Unit | Muestra badge de estado correctamente |
| displays progress bar | Unit | Muestra barra de progreso |
| shows porcentaje | Unit | Muestra porcentaje de progreso |
| displays numBackers | Unit | Muestra numero de backers |
| shows diasRestantes | Unit | Muestra dias restantes si aplica |
| hides diasRestantes for borradores | Unit | No muestra dias para borradores |
| calls onSelect with id on click | Unit | Llama handler al hacer click |
| renders with keyboard navigation | Accessibility | Navegable con teclado (Enter) |

**Casos Detallados:**

```typescript
describe("CampaignCard", () => {
    const mockCampania: MiCampaniaListItem = {
        id: "campania-1",
        titulo: "Mi Album Debut",
        imagenPrincipalUrl: "https://example.com/album.jpg",
        estadoCampaniaId: 2,
        estadoCampaniaNombre: "Publicada",
        importeObjetivo: 5000.00,
        importeRecaudado: 2340.50,
        porcentajeProgreso: 46.81,
        numBackers: 78,
        diasRestantes: 46,
        fechaFin: "2026-03-31T23:59:59Z",
        fechaCreacion: "2026-01-15T12:00:00Z",
    }

    it("renders campania title", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("displays campania image", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        const img = screen.getByAltText("Mi Album Debut")
        expect(img).toHaveAttribute("src", "https://example.com/album.jpg")
    })

    it("shows fallback image when null", () => {
        const campaniaNoImage = { ...mockCampania, imagenPrincipalUrl: null }
        render(<CampaignCard campania={campaniaNoImage} onSelect={vi.fn()} />)

        const img = screen.getByAltText("Mi Album Debut")
        expect(img).toHaveAttribute("src", "/placeholder-campaign.jpg")
    })

    it("renders estado badge", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        expect(screen.getByText("Publicada")).toBeInTheDocument()
    })

    it("displays progress bar", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toBeInTheDocument()
    })

    it("shows porcentaje", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        expect(screen.getByText("46.81%")).toBeInTheDocument()
    })

    it("displays numBackers", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        expect(screen.getByText("78 backers")).toBeInTheDocument()
    })

    it("shows diasRestantes", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        expect(screen.getByText("46 dias restantes")).toBeInTheDocument()
    })

    it("hides diasRestantes for borradores", () => {
        const borrador = {
            ...mockCampania,
            estadoCampaniaId: 1,
            estadoCampaniaNombre: "Borrador",
            diasRestantes: null,
        }
        render(<CampaignCard campania={borrador} onSelect={vi.fn()} />)

        expect(screen.queryByText(/dias restantes/)).not.toBeInTheDocument()
    })

    it("calls onSelect with id on click", () => {
        const handleSelect = vi.fn()
        render(<CampaignCard campania={mockCampania} onSelect={handleSelect} />)

        const card = screen.getByText("Mi Album Debut").closest("div")
        fireEvent.click(card!)

        expect(handleSelect).toHaveBeenCalledWith("campania-1")
    })

    it("renders with keyboard navigation", () => {
        const handleSelect = vi.fn()
        render(<CampaignCard campania={mockCampania} onSelect={handleSelect} />)

        const card = screen.getByText("Mi Album Debut").closest("div")
        card?.focus()
        fireEvent.keyDown(card!, { key: "Enter", code: "Enter" })

        expect(handleSelect).toHaveBeenCalledWith("campania-1")
    })
})
```

---

### 4.4 Components - BackingTable (Ya existe)

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingsTable.test.tsx`

**Nota:** Este componente ya tiene tests completos. Revisarlos y agregar solo casos faltantes:

| Test Case Adicional | Tipo | Descripcion |
|---------------------|------|-------------|
| pagination controls visible | Unit | Muestra controles de paginacion |
| handles page change | Unit | Navega entre paginas correctamente |
| displays total count | Unit | Muestra total de backings |
| keyboard navigation for table rows | Accessibility | Navegacion con Tab y Enter |
| aria-labels on interactive elements | Accessibility | Labels ARIA en inputs y selects |

**Casos a Agregar:**

```typescript
describe("BackingsTable - Additional Tests", () => {
    it("pagination controls visible when totalPages > 1", () => {
        const propsWithPagination = {
            ...defaultProps,
            totalCount: 100,
            currentPage: 1,
        }
        render(<BackingsTable {...propsWithPagination} />)

        expect(screen.getByText("Pagina 1 de 5")).toBeInTheDocument()
        expect(screen.getByLabelText("Siguiente pagina")).toBeInTheDocument()
    })

    it("handles page change", () => {
        const onPageChange = vi.fn()
        const propsWithPagination = {
            ...defaultProps,
            totalCount: 100,
            currentPage: 1,
            onPageChange,
        }
        render(<BackingsTable {...propsWithPagination} />)

        const nextButton = screen.getByLabelText("Siguiente pagina")
        fireEvent.click(nextButton)

        expect(onPageChange).toHaveBeenCalledWith(2)
    })

    it("displays total count", () => {
        const propsWithTotal = { ...defaultProps, totalCount: 78 }
        render(<BackingsTable {...propsWithTotal} />)

        expect(screen.getByText("78 apoyos totales")).toBeInTheDocument()
    })

    it("keyboard navigation for table rows", () => {
        render(<BackingsTable {...defaultProps} />)

        const firstRow = screen
            .getAllByRole("row")
            .find((row) => row.textContent?.includes("Maria Lopez"))
        firstRow?.focus()

        expect(document.activeElement).toBe(firstRow)
    })

    it("aria-labels on interactive elements", () => {
        render(<BackingsTable {...defaultProps} />)

        expect(
            screen.getByLabelText("Buscar backings por nombre de backer")
        ).toBeInTheDocument()

        expect(
            screen.getByLabelText("Filtrar por recompensa")
        ).toBeInTheDocument()
    })
})
```

---

### 4.5 Components - EmptyState

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/components/EmptyDashboardState.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders empty state title | Unit | Muestra titulo de empty state |
| renders description | Unit | Muestra descripcion |
| displays action button | Unit | Muestra boton de accion |
| calls action handler on click | Unit | Llama handler al hacer click en CTA |
| renders icon when provided | Unit | Muestra icono |
| renders without icon | Unit | Funciona sin icono |
| renders without action | Unit | Funciona sin boton de accion |

**Casos Detallados:**

```typescript
describe("EmptyDashboardState", () => {
    it("renders empty state title", () => {
        render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
            />
        )

        expect(screen.getByText("No tienes campanias aun")).toBeInTheDocument()
    })

    it("renders description", () => {
        render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
            />
        )

        expect(
            screen.getByText("Crea tu primera campania para comenzar")
        ).toBeInTheDocument()
    })

    it("displays action button", () => {
        render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
                action={{
                    label: "Crear Campania",
                    href: "/dashboard/campanias/nueva",
                }}
            />
        )

        const button = screen.getByRole("link", { name: "Crear Campania" })
        expect(button).toBeInTheDocument()
        expect(button).toHaveAttribute("href", "/dashboard/campanias/nueva")
    })

    it("calls action handler on click", () => {
        const handleClick = vi.fn()
        render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
                action={{
                    label: "Crear Campania",
                    onClick: handleClick,
                }}
            />
        )

        const button = screen.getByRole("button", { name: "Crear Campania" })
        fireEvent.click(button)

        expect(handleClick).toHaveBeenCalled()
    })

    it("renders icon when provided", () => {
        const { container } = render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
                icon={<MusicIcon className="h-12 w-12" />}
            />
        )

        const icon = container.querySelector("svg")
        expect(icon).toBeInTheDocument()
        expect(icon).toHaveClass("h-12", "w-12")
    })

    it("renders without icon", () => {
        const { container } = render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
            />
        )

        const icon = container.querySelector("svg")
        expect(icon).not.toBeInTheDocument()
    })

    it("renders without action", () => {
        render(
            <EmptyDashboardState
                title="No tienes campanias aun"
                description="Crea tu primera campania para comenzar"
            />
        )

        expect(
            screen.queryByRole("button")
        ).not.toBeInTheDocument()
        expect(
            screen.queryByRole("link")
        ).not.toBeInTheDocument()
    })
})
```

---

### 4.6 Hooks - useDashboardResumen

**Archivo:** `src/admin/src/hooks/__tests__/useDashboardResumen.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches dashboard resumen successfully | Integration | Obtiene resumen correctamente |
| returns data with correct structure | Integration | Estructura de datos correcta |
| handles loading state | Integration | isLoading true inicialmente |
| handles error state | Integration | error cuando API falla |
| retries on network error | Integration | Reintenta en error de red |
| caches data with correct query key | Integration | Usa query key correcto |

**Casos Detallados:**

```typescript
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useDashboardResumen } from "../useDashboardResumen"
import { mockDashboardResumen } from "@/mocks/dashboard.mock"
import * as dashboardService from "@/services/dashboard.service"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    })
    return ({ children }) => (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    )
}

describe("useDashboardResumen", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches dashboard resumen successfully", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })

        const { result } = renderHook(() => useDashboardResumen(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockDashboardResumen)
        expect(dashboardService.getDashboardResumen).toHaveBeenCalled()
    })

    it("returns data with correct structure", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })

        const { result } = renderHook(() => useDashboardResumen(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveProperty("artistaId")
        expect(result.current.data).toHaveProperty("totalRecaudado")
        expect(result.current.data).toHaveProperty("totalBackers")
        expect(result.current.data).toHaveProperty("campaniasActivas")
    })

    it("handles loading state", () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(() => useDashboardResumen(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useDashboardResumen(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
    })

    it("retries on network error", async () => {
        const getDashboardResumenSpy = vi
            .spyOn(dashboardService, "getDashboardResumen")
            .mockRejectedValueOnce(new Error("Network error"))
            .mockResolvedValueOnce({
                data: mockDashboardResumen,
                isSuccess: true,
            })

        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: 1 } },
        })

        const wrapper = ({ children }) => (
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        )

        const { result } = renderHook(() => useDashboardResumen(), { wrapper })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(getDashboardResumenSpy).toHaveBeenCalledTimes(2)
    })

    it("caches data with correct query key", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })

        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false } },
        })

        const wrapper = ({ children }) => (
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        )

        renderHook(() => useDashboardResumen(), { wrapper })

        await waitFor(() => {
            const cachedData = queryClient.getQueryData([
                "dashboard",
                "resumen",
            ])
            expect(cachedData).toBeDefined()
        })
    })
})
```

---

### 4.7 Hooks - useMisCampanias

**Archivo:** `src/admin/src/hooks/__tests__/useMisCampanias.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches mis campanias successfully | Integration | Obtiene lista de campanias |
| handles pagination params | Integration | Pasa parametros de paginacion |
| filters by estadoCampaniaId | Integration | Filtra por estado correctamente |
| handles empty list | Integration | Maneja lista vacia |
| invalidates on mutation | Integration | Invalida cache al crear campania |
| handles loading state | Integration | isLoading true inicialmente |
| handles error state | Integration | error cuando API falla |

**Casos Detallados:**

```typescript
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useMisCampanias } from "../useMisCampanias"
import { mockMisCampanias } from "@/mocks/dashboard.mock"
import * as dashboardService from "@/services/dashboard.service"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    })
    return ({ children }) => (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    )
}

describe("useMisCampanias", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches mis campanias successfully", async () => {
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: mockMisCampanias.length,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        const { result } = renderHook(() => useMisCampanias({ page: 1 }), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(3)
        expect(result.current.data?.items).toEqual(mockMisCampanias)
    })

    it("handles pagination params", async () => {
        const getMisCampaniasSpy = vi
            .spyOn(dashboardService, "getMisCampanias")
            .mockResolvedValue({
                data: {
                    items: [],
                    totalCount: 0,
                    page: 2,
                    pageSize: 10,
                    totalPages: 1,
                },
                isSuccess: true,
            })

        renderHook(() => useMisCampanias({ page: 2, pageSize: 10 }), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(getMisCampaniasSpy).toHaveBeenCalledWith({
                page: 2,
                pageSize: 10,
            })
        })
    })

    it("filters by estadoCampaniaId", async () => {
        const getMisCampaniasSpy = vi
            .spyOn(dashboardService, "getMisCampanias")
            .mockResolvedValue({
                data: {
                    items: [mockMisCampanias[0]],
                    totalCount: 1,
                    page: 1,
                    pageSize: 10,
                    totalPages: 1,
                },
                isSuccess: true,
            })

        renderHook(
            () => useMisCampanias({ page: 1, estadoCampaniaId: 2 }),
            {
                wrapper: createWrapper(),
            }
        )

        await waitFor(() => {
            expect(getMisCampaniasSpy).toHaveBeenCalledWith({
                page: 1,
                estadoCampaniaId: 2,
            })
        })
    })

    it("handles empty list", async () => {
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: [],
                totalCount: 0,
                page: 1,
                pageSize: 10,
                totalPages: 0,
            },
            isSuccess: true,
        })

        const { result } = renderHook(() => useMisCampanias({ page: 1 }), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(0)
        expect(result.current.data?.totalCount).toBe(0)
    })

    it("invalidates on mutation", async () => {
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: mockMisCampanias.length,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false } },
        })

        const wrapper = ({ children }) => (
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        )

        renderHook(() => useMisCampanias({ page: 1 }), { wrapper })

        await waitFor(() => {
            const cachedData = queryClient.getQueryData([
                "dashboard",
                "mis-campanias",
                { page: 1 },
            ])
            expect(cachedData).toBeDefined()
        })

        // Simular invalidacion
        queryClient.invalidateQueries({ queryKey: ["dashboard", "mis-campanias"] })

        const cachedDataAfter = queryClient.getQueryState([
            "dashboard",
            "mis-campanias",
            { page: 1 },
        ])
        expect(cachedDataAfter?.isInvalidated).toBe(true)
    })

    it("handles loading state", () => {
        vi.spyOn(dashboardService, "getMisCampanias").mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(() => useMisCampanias({ page: 1 }), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.spyOn(dashboardService, "getMisCampanias").mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useMisCampanias({ page: 1 }), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
    })
})
```

---

### 4.8 Integration Tests - DashboardPage

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/DashboardPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders dashboard with stats | Integration | Renderiza stats cards con datos |
| displays campanias grid | Integration | Muestra grid de campanias |
| shows empty state when no campanias | Integration | Muestra empty state si no hay campanias |
| handles loading state | Integration | Muestra skeletons mientras carga |
| handles error state | Integration | Muestra mensaje de error |
| navigates to campania detail on click | Integration | Navega al hacer click en card |
| shows create campania button | Integration | Muestra boton de crear campania |
| displays correct number of stats cards | Integration | Muestra 4 stats cards |
| handles pagination in campanias grid | Integration | Maneja paginacion correctamente |

**Casos Detallados:**

```typescript
import { render, screen, waitFor, fireEvent } from "@/test-utils"
import { DashboardPage } from "../page"
import { mockDashboardResumen, mockMisCampanias } from "@/mocks/dashboard.mock"
import * as dashboardService from "@/services/dashboard.service"
import { useRouter } from "next/navigation"

vi.mock("next/navigation", () => ({
    useRouter: vi.fn(),
}))

describe("DashboardPage", () => {
    const mockPush = vi.fn()

    beforeEach(() => {
        vi.clearAllMocks()
        ;(useRouter as any).mockReturnValue({ push: mockPush })
    })

    it("renders dashboard with stats", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: 3,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(screen.getByText("15,340.50 EUR")).toBeInTheDocument()
            expect(screen.getByText("487")).toBeInTheDocument()
            expect(screen.getByText("2")).toBeInTheDocument()
            expect(screen.getByText("3")).toBeInTheDocument()
        })
    })

    it("displays campanias grid", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: 3,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
            expect(screen.getByText("Gira Nacional 2026")).toBeInTheDocument()
            expect(screen.getByText("EP Acustico")).toBeInTheDocument()
        })
    })

    it("shows empty state when no campanias", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumenEmpty,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: [],
                totalCount: 0,
                page: 1,
                pageSize: 10,
                totalPages: 0,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(
                screen.getByText("No tienes campanias aun")
            ).toBeInTheDocument()
            expect(
                screen.getByText("Crea tu primera campania para comenzar")
            ).toBeInTheDocument()
        })
    })

    it("handles loading state", () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockImplementation(
            () => new Promise(() => {})
        )
        vi.spyOn(dashboardService, "getMisCampanias").mockImplementation(
            () => new Promise(() => {})
        )

        const { container } = render(<DashboardPage />)

        const skeletons = container.querySelectorAll('[class*="animate-pulse"]')
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("handles error state", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockRejectedValue(
            new Error("Network error")
        )
        vi.spyOn(dashboardService, "getMisCampanias").mockRejectedValue(
            new Error("Network error")
        )

        render(<DashboardPage />)

        await waitFor(() => {
            expect(
                screen.getByText(/Error al cargar el dashboard/)
            ).toBeInTheDocument()
        })
    })

    it("navigates to campania detail on click", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: 3,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
        })

        const card = screen.getByText("Mi Album Debut").closest("div")
        fireEvent.click(card!)

        expect(mockPush).toHaveBeenCalledWith("/dashboard/campanias/campania-1")
    })

    it("shows create campania button", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: 3,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(
                screen.getByRole("link", { name: /Nueva Campania/i })
            ).toBeInTheDocument()
        })
    })

    it("displays correct number of stats cards", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        vi.spyOn(dashboardService, "getMisCampanias").mockResolvedValue({
            data: {
                items: mockMisCampanias,
                totalCount: 3,
                page: 1,
                pageSize: 10,
                totalPages: 1,
            },
            isSuccess: true,
        })

        render(<DashboardPage />)

        await waitFor(() => {
            const statsCards = screen.getAllByRole("article")
            expect(statsCards).toHaveLength(4)
        })
    })

    it("handles pagination in campanias grid", async () => {
        vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
            data: mockDashboardResumen,
            isSuccess: true,
        })
        const getMisCampaniasSpy = vi
            .spyOn(dashboardService, "getMisCampanias")
            .mockResolvedValue({
                data: {
                    items: mockMisCampanias,
                    totalCount: 25,
                    page: 1,
                    pageSize: 10,
                    totalPages: 3,
                },
                isSuccess: true,
            })

        render(<DashboardPage />)

        await waitFor(() => {
            expect(screen.getByText("Pagina 1 de 3")).toBeInTheDocument()
        })

        const nextButton = screen.getByLabelText("Siguiente pagina")
        fireEvent.click(nextButton)

        await waitFor(() => {
            expect(getMisCampaniasSpy).toHaveBeenCalledWith({ page: 2 })
        })
    })
})
```

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Prioridad |
|---------|--------|-----------|----------|-----------|
| StatsCard.tsx | 90% | 100% | 85% | Alta |
| ProgressBar.tsx | 95% | 100% | 90% | Alta |
| CampaignCard.tsx | 85% | 90% | 80% | Alta |
| BackingsTable.tsx | 80% | 85% | 75% | Media |
| EmptyDashboardState.tsx | 90% | 100% | 85% | Media |
| useDashboardResumen.ts | 95% | 100% | 90% | Alta |
| useMisCampanias.ts | 95% | 100% | 90% | Alta |
| useCampaniaBackings.ts | 90% | 95% | 85% | Media |
| useCampaniaStats.ts | 90% | 95% | 85% | Media |
| DashboardPage.tsx | 75% | 80% | 70% | Alta |

**Meta Global:** 80% en todas las metricas

---

## 6. Estrategia de Mocking

### 6.1 API Services

**Opcion 1: Mock directo en tests**
```typescript
vi.spyOn(dashboardService, "getDashboardResumen").mockResolvedValue({
    data: mockDashboardResumen,
    isSuccess: true,
})
```

**Opcion 2: MSW (Mock Service Worker) - Recomendado para E2E**
```typescript
// En setup de tests
import { setupServer } from "msw/node"
import { dashboardHandlers } from "@/mocks/handlers"

const server = setupServer(...dashboardHandlers)

beforeAll(() => server.listen())
afterEach(() => server.resetHandlers())
afterAll(() => server.close())
```

### 6.2 Next.js Router

```typescript
vi.mock("next/navigation", () => ({
    useRouter: vi.fn(() => ({
        push: vi.fn(),
        replace: vi.fn(),
        back: vi.fn(),
    })),
    usePathname: vi.fn(() => "/dashboard"),
}))
```

### 6.3 React Query

Ya configurado en `test-utils.tsx`:
```typescript
const queryClient = new QueryClient({
    defaultOptions: {
        queries: { retry: false },
        mutations: { retry: false },
    },
})
```

---

## 7. Tests de Accesibilidad

### 7.1 ARIA Labels

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/accessibility.test.tsx`

```typescript
describe("Dashboard Accessibility", () => {
    it("progress bars have correct ARIA attributes", async () => {
        render(<DashboardPage />)

        await waitFor(() => {
            const progressBars = screen.getAllByRole("progressbar")
            progressBars.forEach((bar) => {
                expect(bar).toHaveAttribute("aria-valuenow")
                expect(bar).toHaveAttribute("aria-valuemin", "0")
                expect(bar).toHaveAttribute("aria-valuemax", "100")
            })
        })
    })

    it("search input has accessible label", () => {
        render(<BackingsTable {...defaultProps} />)

        const searchInput = screen.getByLabelText(
            "Buscar backings por nombre de backer"
        )
        expect(searchInput).toBeInTheDocument()
    })

    it("buttons have descriptive text or aria-label", () => {
        render(<DashboardPage />)

        const buttons = screen.getAllByRole("button")
        buttons.forEach((button) => {
            const hasText = button.textContent && button.textContent.trim().length > 0
            const hasAriaLabel = button.getAttribute("aria-label")
            expect(hasText || hasAriaLabel).toBe(true)
        })
    })
})
```

### 7.2 Keyboard Navigation

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/keyboard-navigation.test.tsx`

```typescript
describe("Dashboard Keyboard Navigation", () => {
    it("campaign cards are keyboard navigable", async () => {
        render(<DashboardPage />)

        await waitFor(() => {
            expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
        })

        const card = screen.getByText("Mi Album Debut").closest("div")
        card?.focus()

        expect(document.activeElement).toBe(card)

        fireEvent.keyDown(card!, { key: "Enter", code: "Enter" })
        expect(mockPush).toHaveBeenCalledWith("/dashboard/campanias/campania-1")
    })

    it("table rows are keyboard navigable", () => {
        render(<BackingsTable {...defaultProps} />)

        const firstRow = screen.getAllByRole("row")[1] // Skip header row
        firstRow?.focus()

        fireEvent.keyDown(firstRow, { key: "Tab", code: "Tab" })
        // Assert focus moves to next element
    })

    it("pagination controls are keyboard accessible", () => {
        render(<BackingsTable {...defaultPropsWithPagination} />)

        const nextButton = screen.getByLabelText("Siguiente pagina")
        nextButton.focus()

        expect(document.activeElement).toBe(nextButton)

        fireEvent.keyDown(nextButton, { key: "Enter", code: "Enter" })
        expect(onPageChange).toHaveBeenCalledWith(2)
    })
})
```

### 7.3 Color Contrast

**Archivo:** `src/admin/src/app/(dashboard)/dashboard/__tests__/color-contrast.test.tsx`

```typescript
describe("Dashboard Color Contrast", () => {
    it("estado badges have sufficient contrast", () => {
        render(<CampaignCard campania={mockCampania} onSelect={vi.fn()} />)

        const badge = screen.getByText("Publicada")
        const styles = window.getComputedStyle(badge)

        // Verify background and text color meet WCAG AA (4.5:1)
        // This is a simplified example; use tools like axe-core for real tests
        expect(styles.backgroundColor).toBeDefined()
        expect(styles.color).toBeDefined()
    })

    it("progress bars have visible fill", () => {
        render(<ProgressBar current={2340.50} goal={5000} />)

        const progressFill = screen.getByRole("progressbar").firstChild
        const styles = window.getComputedStyle(progressFill!)

        expect(styles.backgroundColor).toBeDefined()
        expect(styles.backgroundColor).not.toBe("transparent")
    })
})
```

---

## 8. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar tests de dashboard especificamente
npm run test -- --filter=dashboard

# Watch mode
npm run test -- --watch

# UI mode (Vitest UI)
npx vitest --ui

# Ejecutar solo tests de accesibilidad
npm run test -- --filter=accessibility

# Ejecutar solo tests de hooks
npm run test -- hooks
```

---

## 9. CI/CD Integration

```yaml
# .github/workflows/admin-tests.yml
name: Admin Frontend Tests

on:
  push:
    branches: [master]
    paths:
      - "src/admin/**"
      - "src/shared/**"
  pull_request:
    branches: [master]
    paths:
      - "src/admin/**"
      - "src/shared/**"

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "18"

      - name: Install dependencies
        run: cd src/admin && npm ci

      - name: Run tests with coverage
        run: cd src/admin && npm run test -- --coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/admin/coverage/coverage-final.json
          flags: admin-frontend
          name: dashboard-artista

      - name: Check coverage threshold
        run: |
          cd src/admin
          npx vitest --coverage --coverage.thresholds.lines=80 \
            --coverage.thresholds.functions=80 \
            --coverage.thresholds.branches=80 \
            --coverage.thresholds.statements=80
```

---

## 10. Checklist de Testing

### Preparacion
- [ ] Mocks creados en `__mocks__/dashboard.mock.ts`
- [ ] MSW handlers configurados (opcional)
- [ ] Test utilities verificados (`test-utils.tsx`)
- [ ] Vitest config actualizado

### Unit Tests
- [ ] StatsCard - 8 test cases
- [ ] ProgressBar - 9 test cases
- [ ] CampaignCard - 11 test cases
- [ ] BackingsTable - 5 test cases adicionales
- [ ] EmptyDashboardState - 7 test cases

### Integration Tests
- [ ] useDashboardResumen - 6 test cases
- [ ] useMisCampanias - 7 test cases
- [ ] useCampaniaBackings - 6 test cases (similar a useMisCampanias)
- [ ] useCampaniaStats - 5 test cases (similar a useDashboardResumen)
- [ ] DashboardPage - 9 test cases

### Accessibility Tests
- [ ] ARIA labels en todos los componentes interactivos
- [ ] Keyboard navigation funcionando
- [ ] Color contrast verificado
- [ ] Screen reader friendly

### Coverage
- [ ] Cobertura 80%+ en lineas
- [ ] Cobertura 80%+ en funciones
- [ ] Cobertura 75%+ en branches
- [ ] Tests pasan en CI

### Edge Cases
- [ ] Empty states cubiertos
- [ ] Loading states cubiertos
- [ ] Error states cubiertos
- [ ] Backings anonimos testeados
- [ ] Paginacion testeada
- [ ] Filtros testeados

---

## 11. Casos Edge Importantes

### 11.1 Backings Anonimos

```typescript
it("displays anonymous backers correctly", () => {
    render(<BackingsTable {...defaultProps} />)

    const anonimo = screen.getByText("Anonimo")
    expect(anonimo).toBeInTheDocument()
    expect(anonimo).toHaveClass("italic", "text-muted-foreground")

    // Email no debe estar visible
    expect(screen.queryByText("email@example.com")).not.toBeInTheDocument()
})
```

### 11.2 Campanias sin Fecha Fin (Borradores)

```typescript
it("handles campanias without fechaFin", () => {
    const borrador = {
        ...mockCampania,
        estadoCampaniaId: 1,
        diasRestantes: null,
        fechaFin: null,
    }
    render(<CampaignCard campania={borrador} onSelect={vi.fn()} />)

    expect(screen.queryByText(/dias restantes/)).not.toBeInTheDocument()
})
```

### 11.3 Progreso Mayor a 100%

```typescript
it("handles progress over 100%", () => {
    render(<ProgressBar current={7500} goal={5000} showPercentage={true} />)

    expect(screen.getByText("150.00%")).toBeInTheDocument()

    const progressFill = screen.getByRole("progressbar").firstChild
    expect(progressFill).toHaveClass("bg-green-500")
})
```

### 11.4 Sin Recompensa (Backing sin Reward)

```typescript
it("displays 'Sin recompensa' when rewardNombre is null", () => {
    render(<BackingsTable {...defaultProps} />)

    expect(screen.getByText("Sin recompensa")).toBeInTheDocument()
})
```

---

## 12. Notas Tecnicas

### 12.1 Fake Timers para Fechas

```typescript
import { vi } from "vitest"

beforeAll(() => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date("2026-02-14T12:00:00Z"))
})

afterAll(() => {
    vi.useRealTimers()
})

it("calculates dias restantes correctly", () => {
    const campania = {
        ...mockCampania,
        fechaFin: "2026-03-31T23:59:59Z",
    }
    // Dias restantes = 45 dias desde 2026-02-14
    render(<CampaignCard campania={campania} onSelect={vi.fn()} />)

    expect(screen.getByText("45 dias restantes")).toBeInTheDocument()
})
```

### 12.2 Testing React Query Invalidation

```typescript
it("invalidates queries on successful mutation", async () => {
    const queryClient = new QueryClient()
    const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

    const wrapper = ({ children }) => (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    )

    // Trigger mutation
    const { result } = renderHook(() => useCreateCampania(), { wrapper })

    act(() => {
        result.current.mutate({ titulo: "Nueva Campania" })
    })

    await waitFor(() => {
        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["dashboard", "mis-campanias"],
        })
    })
})
```

### 12.3 Testing Error Boundaries

```typescript
it("handles error boundary gracefully", () => {
    const ThrowError = () => {
        throw new Error("Test error")
    }

    render(
        <ErrorBoundary fallback={<div>Error ocurrido</div>}>
            <ThrowError />
        </ErrorBoundary>
    )

    expect(screen.getByText("Error ocurrido")).toBeInTheDocument()
})
```

---

## 13. Metricas de Exito

| Metrica | Objetivo | Metodo de Medicion |
|---------|----------|-------------------|
| Cobertura de lineas | 80%+ | `vitest --coverage` |
| Cobertura de funciones | 80%+ | `vitest --coverage` |
| Cobertura de branches | 75%+ | `vitest --coverage` |
| Tiempo de ejecucion | < 30s | `vitest run --reporter=verbose` |
| Tests que fallan | 0 | CI pipeline |
| Accesibilidad (axe-core) | 0 violaciones | `@axe-core/react` |

---

## 14. Recursos y Referencias

### Documentacion
- Vitest: https://vitest.dev/
- Testing Library: https://testing-library.com/docs/react-testing-library/intro/
- MSW: https://mswjs.io/docs/
- React Query Testing: https://tanstack.com/query/latest/docs/framework/react/guides/testing

### Ejemplos en el Codebase
- `src/admin/src/app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingsTable.test.tsx`
- `src/admin/src/test-utils.tsx`
- `src/admin/vitest.config.ts`

### Templates
- Unit Test Template: `.claude/templates/frontend/unit-test.template.tsx` (crear)
- Integration Test Template: `.claude/templates/frontend/integration-test.template.tsx` (crear)

---

**Fin del Plan de Testing**

**Resumen:**
- **46 test cases** planificados
- **Cobertura objetivo:** 80%+
- **Tiempo estimado:** < 30 segundos
- **Prioridad:** Tests de hooks y componentes criticos primero
- **Accesibilidad:** ARIA labels, keyboard navigation, color contrast

**Siguiente paso:** Implementar tests comenzando por componentes base (StatsCard, ProgressBar) y luego hooks (useDashboardResumen, useMisCampanias).
