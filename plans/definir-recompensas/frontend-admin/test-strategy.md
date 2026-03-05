# Estrategia de Testing: Definir Recompensas (Admin)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/admin
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 12 | 75% |
| Integration Tests | 8 | 85% |
| Total | 20 | 80%+ |

## 2. Estructura de Tests

```
src/admin/src/
└── app/
    └── (dashboard)/
        └── campanias/
            └── [id]/
                └── recompensas/
                    ├── components/
                    │   ├── __tests__/
                    │   │   ├── RewardsListPage.test.tsx
                    │   │   ├── RewardCard.test.tsx
                    │   │   ├── RewardFormModal.test.tsx
                    │   │   ├── RewardDeleteDialog.test.tsx
                    │   │   ├── RewardStatsCard.test.tsx
                    │   │   └── EmptyRewardsState.test.tsx
                    │   ├── RewardsListPage.tsx
                    │   ├── RewardCard.tsx
                    │   ├── RewardFormModal.tsx
                    │   ├── RewardDeleteDialog.tsx
                    │   ├── RewardStatsCard.tsx
                    │   └── EmptyRewardsState.tsx
                    ├── hooks/
                    │   ├── __tests__/
                    │   │   ├── useRewards.test.ts
                    │   │   ├── useCreateReward.test.ts
                    │   │   ├── useUpdateReward.test.ts
                    │   │   ├── useDeleteReward.test.ts
                    │   │   └── useReorderRewards.test.ts
                    │   ├── useRewards.ts
                    │   ├── useCreateReward.ts
                    │   ├── useUpdateReward.ts
                    │   ├── useDeleteReward.ts
                    │   └── useReorderRewards.ts
                    └── __mocks__/
                        └── rewards.mock.ts
```

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/rewards.mock.ts`

```typescript
import type { Reward, RewardListItem } from "@shared/types"

export const mockReward: Reward = {
    id: "reward-1",
    campaniaId: "campania-1",
    tipoRewardId: 1, // Digital
    nombre: "Descarga Digital",
    descripcion: "Acceso anticipado al album completo en formato digital FLAC + MP3",
    importeMinimo: 10.00,
    monedaId: 1, // EUR
    esAddOn: false,
    cantidadMaxima: null, // Ilimitado
    cantidadPorBacker: 1,
    incluyeEnvioFisico: false,
    tiempoEntregaEstimado: "Inmediato tras finalizar campania",
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-02-13T10:00:00Z",
    fechaActualizacion: null,
}

export const mockRewardLimited: Reward = {
    ...mockReward,
    id: "reward-2",
    nombre: "CD Fisico",
    descripcion: "CD firmado + descarga digital + booklet dedicado",
    importeMinimo: 25.00,
    cantidadMaxima: 200,
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: "30 dias",
    orden: 2,
}

export const mockRewardSoldOut: Reward = {
    ...mockReward,
    id: "reward-3",
    nombre: "Vinilo Limitado",
    descripcion: "Vinilo en color especial + poster exclusivo",
    importeMinimo: 50.00,
    cantidadMaxima: 100,
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: "60 dias",
    orden: 3,
}

export const mockRewardWithBackings: Reward = {
    ...mockReward,
    id: "reward-4",
    nombre: "Paquete VIP",
    descripcion: "Meet & greet privado + vinilo + mercancia exclusiva",
    importeMinimo: 100.00,
    cantidadMaxima: 50,
    orden: 4,
}

export const mockRewardList: RewardListItem[] = [
    {
        id: "reward-1",
        campaniaId: "campania-1",
        nombre: "Descarga Digital",
        descripcion: "Acceso anticipado al album completo",
        importeMinimo: 10.00,
        esAddOn: false,
        cantidadMaxima: null,
        incluyeEnvioFisico: false,
        orden: 1,
        esActivo: true,
    },
    {
        id: "reward-2",
        campaniaId: "campania-1",
        nombre: "CD Fisico",
        descripcion: "CD firmado + descarga digital",
        importeMinimo: 25.00,
        esAddOn: false,
        cantidadMaxima: 200,
        incluyeEnvioFisico: true,
        orden: 2,
        esActivo: true,
    },
    {
        id: "reward-3",
        campaniaId: "campania-1",
        nombre: "Vinilo Limitado",
        descripcion: "Vinilo en color especial + poster",
        importeMinimo: 50.00,
        esAddOn: false,
        cantidadMaxima: 100,
        incluyeEnvioFisico: true,
        orden: 3,
        esActivo: true,
    },
]

export const mockCreateRewardRequest = {
    campaniaId: "campania-1",
    tipoRewardId: 1,
    nombre: "Nueva Recompensa",
    descripcion: "Descripcion de la recompensa",
    importeMinimo: 15.00,
    monedaId: 1,
    esAddOn: false,
    cantidadMaxima: null,
    cantidadPorBacker: 1,
    incluyeEnvioFisico: false,
    tiempoEntregaEstimado: "",
    orden: 4,
}

export const mockUpdateRewardRequest = {
    id: "reward-1",
    nombre: "Descarga Digital Actualizada",
    descripcion: "Descripcion actualizada",
    importeMinimo: 12.00,
}

export const mockReorderRequest = {
    campaniaId: "campania-1",
    rewardOrders: [
        { rewardId: "reward-3", orden: 1 },
        { rewardId: "reward-1", orden: 2 },
        { rewardId: "reward-2", orden: 3 },
    ],
}

// Helper para construir rewards customizados
export function buildReward(overrides: Partial<Reward> = {}): Reward {
    return {
        ...mockReward,
        ...overrides,
    }
}

export function buildRewardList(count: number): RewardListItem[] {
    return Array.from({ length: count }, (_, i) => ({
        id: `reward-${i + 1}`,
        campaniaId: "campania-1",
        nombre: `Recompensa ${i + 1}`,
        descripcion: `Descripcion de recompensa ${i + 1}`,
        importeMinimo: (i + 1) * 10,
        esAddOn: false,
        cantidadMaxima: i % 2 === 0 ? 100 : null,
        incluyeEnvioFisico: i % 2 === 0,
        orden: i + 1,
        esActivo: true,
    }))
}
```

### 3.2 MSW Handlers (para integration tests)

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from "msw"
import { API_ROUTES } from "@shared/constants"
import { mockRewardList, mockReward } from "./rewards.mock"

export const rewardHandlers = [
    // GET /api/rewards?campaniaId=X
    rest.get(API_ROUTES.rewards.base, (req, res, ctx) => {
        const campaniaId = req.url.searchParams.get("campaniaId")
        const esActivo = req.url.searchParams.get("esActivo")

        let filteredRewards = mockRewardList

        if (esActivo === "true") {
            filteredRewards = filteredRewards.filter((r) => r.esActivo)
        }

        return res(
            ctx.status(200),
            ctx.json({
                data: filteredRewards,
                messages: [{ message: "Rewards encontrados", errorCode: "0000" }],
            })
        )
    }),

    // GET /api/rewards/{id}
    rest.get(`${API_ROUTES.rewards.base}/:id`, (req, res, ctx) => {
        const { id } = req.params
        return res(
            ctx.status(200),
            ctx.json({
                data: { ...mockReward, id: id as string },
                messages: [{ message: "Reward encontrado", errorCode: "0000" }],
            })
        )
    }),

    // POST /api/rewards
    rest.post(API_ROUTES.rewards.base, async (req, res, ctx) => {
        const body = await req.json()
        return res(
            ctx.status(200),
            ctx.json({
                data: { ...mockReward, ...body, id: "new-reward-id" },
                messages: [{ message: "Reward creado correctamente", errorCode: "0001" }],
            })
        )
    }),

    // PUT /api/rewards/{id}
    rest.put(`${API_ROUTES.rewards.base}/:id`, async (req, res, ctx) => {
        const body = await req.json()
        return res(
            ctx.status(200),
            ctx.json({
                data: true,
                messages: [{ message: "Reward actualizado correctamente", errorCode: "0002" }],
            })
        )
    }),

    // DELETE /api/rewards/{id}
    rest.delete(`${API_ROUTES.rewards.base}/:id`, (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json({
                data: true,
                messages: [{ message: "Reward eliminado correctamente", errorCode: "0003" }],
            })
        )
    }),

    // PUT /api/rewards/reorder
    rest.put(API_ROUTES.rewards.reorder, async (req, res, ctx) => {
        const body = await req.json()
        return res(
            ctx.status(200),
            ctx.json({
                data: true,
                messages: [{ message: "Recompensas reordenadas correctamente", errorCode: "0002" }],
            })
        )
    }),

    // DELETE error - reward con backings
    rest.delete(`${API_ROUTES.rewards.base}/reward-with-backings`, (req, res, ctx) => {
        return res(
            ctx.status(409),
            ctx.json({
                data: null,
                messages: [
                    {
                        message: "No se puede eliminar una recompensa con aportes existentes",
                        errorCode: "4010",
                    },
                ],
            })
        )
    }),
]
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx` (ya existe en src/admin/src/)

```typescript
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

// Test query client sin retries
export const createTestQueryClient = () =>
    new QueryClient({
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

export const AllProviders = ({ children }: { children: React.ReactNode }) => {
    const testQueryClient = createTestQueryClient()

    return (
        <QueryClientProvider client={testQueryClient}>
            {children}
        </QueryClientProvider>
    )
}

// Ya exportado en test-utils existente
export { render, screen } from "@testing-library/react"
```

## 4. Tests por Modulo

### 4.1 Components

#### RewardsListPage.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/__tests__/RewardsListPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders correctly with rewards | Integration | Renderiza lista de rewards correctamente |
| shows empty state when no rewards | Unit | Muestra EmptyRewardsState cuando no hay datos |
| displays stats card with correct totals | Unit | Calcula y muestra totales (activas, stock, backers) |
| shows skeletons in loading state | Unit | Muestra skeleton loaders mientras carga |
| opens modal when "Agregar recompensa" clicked | Unit | Abre RewardFormModal en modo create |
| opens modal in edit mode when Edit clicked | Unit | Abre RewardFormModal con datos del reward |
| opens delete dialog when Delete clicked | Unit | Abre RewardDeleteDialog con reward seleccionado |
| updates list after successful create | Integration | Lista refresh despues de crear reward |
| updates list after successful edit | Integration | Lista refresh despues de editar reward |
| removes reward from list after delete | Integration | Reward desaparece de lista tras eliminar |

**Casos Detallados:**

```typescript
describe("RewardsListPage", () => {
    const campaniaId = "campania-1"

    beforeEach(() => {
        server.use(...rewardHandlers)
    })

    it("renders correctly with rewards", async () => {
        render(<RewardsListPage campaniaId={campaniaId} />)

        // Wait for data to load
        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        expect(screen.getByText("CD Fisico")).toBeInTheDocument()
        expect(screen.getByText("Vinilo Limitado")).toBeInTheDocument()
    })

    it("shows empty state when no rewards", async () => {
        server.use(
            rest.get(API_ROUTES.rewards.base, (req, res, ctx) => {
                return res(ctx.status(200), ctx.json({ data: [], messages: [] }))
            })
        )

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText(/No has creado recompensas aun/)).toBeInTheDocument()
        })

        expect(screen.getByRole("button", { name: /Crear primera recompensa/ })).toBeInTheDocument()
    })

    it("displays stats card with correct totals", async () => {
        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText(/Total: 3/)).toBeInTheDocument()
        })

        expect(screen.getByText(/Activas: 3/)).toBeInTheDocument()
        // Stock calculado: reward-2 (200) + reward-3 (100) = 300
        expect(screen.getByText(/Stock: 300 unid/)).toBeInTheDocument()
    })

    it("shows skeletons in loading state", () => {
        render(<RewardsListPage campaniaId={campaniaId} />)

        // Antes de que resuelva la query, debe haber skeletons
        expect(screen.getAllByTestId("reward-skeleton")).toHaveLength(3)
    })

    it("opens modal when Agregar recompensa clicked", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        const addButton = screen.getByRole("button", { name: /Agregar recompensa/ })
        await user.click(addButton)

        expect(screen.getByText("Nueva Recompensa")).toBeInTheDocument() // Modal title
        expect(screen.getByLabelText(/Nombre de la recompensa/)).toBeInTheDocument()
    })

    it("opens modal in edit mode when Edit clicked", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        const editButtons = screen.getAllByRole("button", { name: /Editar/ })
        await user.click(editButtons[0])

        expect(screen.getByText("Editar Recompensa")).toBeInTheDocument() // Modal title
        // Form pre-completado
        expect(screen.getByDisplayValue("Descarga Digital")).toBeInTheDocument()
    })

    it("opens delete dialog when Delete clicked", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        const deleteButtons = screen.getAllByRole("button", { name: /Eliminar/ })
        await user.click(deleteButtons[0])

        expect(screen.getByText(/Eliminar Recompensa/)).toBeInTheDocument()
        expect(screen.getByText(/Esta accion no se puede deshacer/)).toBeInTheDocument()
    })

    it("updates list after successful create", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        // Click Agregar
        await user.click(screen.getByRole("button", { name: /Agregar recompensa/ }))

        // Fill form
        await user.type(screen.getByLabelText(/Nombre/), "Nueva Reward")
        await user.type(screen.getByLabelText(/Descripcion/), "Descripcion test")
        await user.type(screen.getByLabelText(/Importe minimo/), "20")

        // Submit
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        // Toast exito
        await waitFor(() => {
            expect(screen.getByText(/Recompensa creada/)).toBeInTheDocument()
        })

        // Lista refresh (nueva reward aparece)
        expect(screen.getByText("Nueva Reward")).toBeInTheDocument()
    })

    it("updates list after successful edit", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        // Click Edit
        const editButtons = screen.getAllByRole("button", { name: /Editar/ })
        await user.click(editButtons[0])

        // Update nombre
        const nombreInput = screen.getByDisplayValue("Descarga Digital")
        await user.clear(nombreInput)
        await user.type(nombreInput, "Descarga Digital Actualizada")

        // Submit
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        // Toast exito
        await waitFor(() => {
            expect(screen.getByText(/Recompensa actualizada/)).toBeInTheDocument()
        })

        // Lista refresh
        expect(screen.getByText("Descarga Digital Actualizada")).toBeInTheDocument()
    })

    it("removes reward from list after delete", async () => {
        const user = userEvent.setup()

        render(<RewardsListPage campaniaId={campaniaId} />)

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        // Click Delete
        const deleteButtons = screen.getAllByRole("button", { name: /Eliminar/ })
        await user.click(deleteButtons[0])

        // Confirm delete
        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        // Toast exito
        await waitFor(() => {
            expect(screen.getByText(/Recompensa eliminada/)).toBeInTheDocument()
        })

        // Reward desaparece
        expect(screen.queryByText("Descarga Digital")).not.toBeInTheDocument()
    })
})
```

#### RewardCard.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/__tests__/RewardCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders precio, titulo, descripcion | Unit | Muestra datos basicos del reward |
| shows drag handle | Unit | Drag handle visible en card |
| shows unlimited stock indicator | Unit | Icono infinity + texto "Ilimitadas" cuando cantidadMaxima null |
| shows limited stock indicator | Unit | Package icon + texto "X de Y disponibles" |
| shows sold out indicator | Unit | X-circle icon + texto "Agotado" cuando stock = 0 |
| shows stock warning badge when < 50% | Unit | Badge "50% vendido" amarillo cuando quedan < 50% |
| renders Edit and Delete buttons | Unit | Botones Edit y Delete presentes |
| calls onEdit when Edit clicked | Unit | Callback onEdit llamado con rewardId |
| calls onDelete when Delete clicked | Unit | Callback onDelete llamado con rewardId |
| shows backers count | Unit | Muestra numero de backers |

**Casos Detallados:**

```typescript
describe("RewardCard", () => {
    const defaultHandlers = {
        onEdit: vi.fn(),
        onDelete: vi.fn(),
        onReorder: vi.fn(),
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders precio, titulo, descripcion", () => {
        render(
            <RewardCard
                reward={mockReward}
                {...defaultHandlers}
            />
        )

        expect(screen.getByText("€ 10.00")).toBeInTheDocument()
        expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        expect(screen.getByText(/Acceso anticipado/)).toBeInTheDocument()
    })

    it("shows drag handle", () => {
        render(<RewardCard reward={mockReward} {...defaultHandlers} />)

        const dragHandle = screen.getByTestId("drag-handle")
        expect(dragHandle).toBeInTheDocument()
    })

    it("shows unlimited stock indicator", () => {
        render(<RewardCard reward={mockReward} {...defaultHandlers} />)

        expect(screen.getByText(/Ilimitadas disponibles/)).toBeInTheDocument()
    })

    it("shows limited stock indicator", () => {
        render(<RewardCard reward={mockRewardLimited} {...defaultHandlers} />)

        // Asumiendo que cantidadVendida = 100
        expect(screen.getByText(/100 de 200 disponibles/)).toBeInTheDocument()
    })

    it("shows sold out indicator", () => {
        const soldOutReward = buildReward({
            cantidadMaxima: 100,
            // Mock cantidadVendida = 100 (via API response)
        })

        render(<RewardCard reward={soldOutReward} {...defaultHandlers} />)

        expect(screen.getByText(/Agotado/)).toBeInTheDocument()
    })

    it("shows stock warning badge when < 50%", () => {
        // Stock 45% vendido (90 de 200)
        const lowStockReward = buildReward({
            cantidadMaxima: 200,
            // Mock cantidadVendida = 110
        })

        render(<RewardCard reward={lowStockReward} {...defaultHandlers} />)

        expect(screen.getByText(/55% vendido/)).toBeInTheDocument() // Badge warning
    })

    it("renders Edit and Delete buttons", () => {
        render(<RewardCard reward={mockReward} {...defaultHandlers} />)

        expect(screen.getByRole("button", { name: /Editar/ })).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Eliminar/ })).toBeInTheDocument()
    })

    it("calls onEdit when Edit clicked", async () => {
        const onEdit = vi.fn()
        const user = userEvent.setup()

        render(<RewardCard reward={mockReward} {...defaultHandlers} onEdit={onEdit} />)

        await user.click(screen.getByRole("button", { name: /Editar/ }))

        expect(onEdit).toHaveBeenCalledWith("reward-1")
    })

    it("calls onDelete when Delete clicked", async () => {
        const onDelete = vi.fn()
        const user = userEvent.setup()

        render(<RewardCard reward={mockReward} {...defaultHandlers} onDelete={onDelete} />)

        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        expect(onDelete).toHaveBeenCalledWith("reward-1")
    })

    it("shows backers count", () => {
        // Mock reward con backers count
        const rewardWithBackers = buildReward({
            // Suponer que API retorna backersCount
        })

        render(<RewardCard reward={rewardWithBackers} {...defaultHandlers} />)

        expect(screen.getByText(/234 backers/)).toBeInTheDocument()
    })
})
```

#### RewardFormModal.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/__tests__/RewardFormModal.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders form vacio en create mode | Unit | Modal con titulo "Nueva Recompensa", form vacio |
| renders form pre-completado en edit mode | Unit | Modal con titulo "Editar Recompensa", datos prellenados |
| validates nombre requerido | Unit | Muestra error "El nombre es obligatorio" |
| validates nombre max 200 caracteres | Unit | Muestra error cuando supera 200 chars |
| validates descripcion max 2000 caracteres | Unit | Muestra error cuando supera 2000 chars |
| validates importe minimo > 0 | Unit | Muestra error "Debe ser mayor a 0" |
| updates character counter for nombre | Unit | Contador X/200 actualizado al escribir |
| updates character counter for descripcion | Unit | Contador X/2000 actualizado al escribir |
| shows/hides stock limitado section | Unit | Seccion condicional visible cuando checkbox checked |
| shows/hides envio fisico section | Unit | Seccion condicional visible cuando checkbox checked |
| calls create mutation with correct data | Integration | POST /api/rewards llamado con datos correctos |
| calls update mutation with correct data | Integration | PUT /api/rewards/{id} llamado con datos correctos |
| shows loading state during submit | Unit | Botones disabled, spinner visible |
| closes modal on success | Integration | Modal cierra despues de crear/editar exitoso |
| shows toast on success | Integration | Toast "Recompensa creada/actualizada" visible |
| shows toast on error | Integration | Toast con mensaje de error del backend |

**Casos Detallados:**

```typescript
describe("RewardFormModal", () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        campaniaId: "campania-1",
        mode: "create" as const,
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders form vacio en create mode", () => {
        render(<RewardFormModal {...defaultProps} />)

        expect(screen.getByText("Nueva Recompensa")).toBeInTheDocument()
        expect(screen.getByLabelText(/Nombre/)).toHaveValue("")
        expect(screen.getByLabelText(/Descripcion/)).toHaveValue("")
        expect(screen.getByLabelText(/Importe minimo/)).toHaveValue(null)
    })

    it("renders form pre-completado en edit mode", () => {
        render(
            <RewardFormModal
                {...defaultProps}
                mode="edit"
                reward={mockReward}
            />
        )

        expect(screen.getByText("Editar Recompensa")).toBeInTheDocument()
        expect(screen.getByDisplayValue("Descarga Digital")).toBeInTheDocument()
        expect(screen.getByDisplayValue(/Acceso anticipado/)).toBeInTheDocument()
        expect(screen.getByDisplayValue("10")).toBeInTheDocument()
    })

    it("validates nombre requerido", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const submitButton = screen.getByRole("button", { name: /Guardar/ })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/El nombre es obligatorio/)).toBeInTheDocument()
        })
    })

    it("validates nombre max 200 caracteres", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/Nombre/)
        await user.type(nombreInput, "a".repeat(201))

        await waitFor(() => {
            expect(screen.getByText(/Maximo 200 caracteres/)).toBeInTheDocument()
        })
    })

    it("validates descripcion max 2000 caracteres", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const descripcionInput = screen.getByLabelText(/Descripcion/)
        await user.type(descripcionInput, "a".repeat(2001))

        await waitFor(() => {
            expect(screen.getByText(/Maximo 2000 caracteres/)).toBeInTheDocument()
        })
    })

    it("validates importe minimo > 0", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const importeInput = screen.getByLabelText(/Importe minimo/)
        await user.type(importeInput, "0")

        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            expect(screen.getByText(/Debe ser mayor a 0/)).toBeInTheDocument()
        })
    })

    it("updates character counter for nombre", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/Nombre/)
        await user.type(nombreInput, "Test")

        expect(screen.getByText("4/200")).toBeInTheDocument()
    })

    it("updates character counter for descripcion", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const descripcionInput = screen.getByLabelText(/Descripcion/)
        await user.type(descripcionInput, "Test descripcion")

        expect(screen.getByText("16/2000")).toBeInTheDocument()
    })

    it("shows/hides stock limitado section", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Section oculta por defecto
        expect(screen.queryByLabelText(/Cantidad maxima/)).not.toBeInTheDocument()

        // Check "Stock limitado"
        const checkbox = screen.getByLabelText(/Stock limitado/)
        await user.click(checkbox)

        // Section visible
        await waitFor(() => {
            expect(screen.getByLabelText(/Cantidad maxima/)).toBeInTheDocument()
        })
    })

    it("shows/hides envio fisico section", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Section oculta por defecto
        expect(screen.queryByLabelText(/Tiempo de entrega/)).not.toBeInTheDocument()

        // Check "Incluye envio fisico"
        const checkbox = screen.getByLabelText(/Incluye envio fisico/)
        await user.click(checkbox)

        // Section visible
        await waitFor(() => {
            expect(screen.getByLabelText(/Tiempo de entrega/)).toBeInTheDocument()
        })
    })

    it("calls create mutation with correct data", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Fill form
        await user.type(screen.getByLabelText(/Nombre/), "Nueva Reward")
        await user.type(screen.getByLabelText(/Descripcion/), "Descripcion test")
        await user.type(screen.getByLabelText(/Importe minimo/), "15")

        // Select tipo
        const tipoSelect = screen.getByLabelText(/Tipo de recompensa/)
        await user.selectOptions(tipoSelect, "1") // Digital

        // Submit
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            // Verify API call
            expect(fetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/rewards"),
                expect.objectContaining({
                    method: "POST",
                    body: expect.stringContaining("Nueva Reward"),
                })
            )
        })
    })

    it("calls update mutation with correct data", async () => {
        const user = userEvent.setup()

        render(
            <RewardFormModal
                {...defaultProps}
                mode="edit"
                reward={mockReward}
            />
        )

        // Update nombre
        const nombreInput = screen.getByDisplayValue("Descarga Digital")
        await user.clear(nombreInput)
        await user.type(nombreInput, "Descarga Digital Actualizada")

        // Submit
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            // Verify API call
            expect(fetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/rewards/reward-1"),
                expect.objectContaining({
                    method: "PUT",
                    body: expect.stringContaining("Actualizada"),
                })
            )
        })
    })

    it("shows loading state during submit", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Fill form minimamente
        await user.type(screen.getByLabelText(/Nombre/), "Test")
        await user.type(screen.getByLabelText(/Importe minimo/), "10")

        // Submit
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        // Loading state
        await waitFor(() => {
            expect(screen.getByText("Guardando...")).toBeInTheDocument()
        })

        // Buttons disabled
        expect(screen.getByRole("button", { name: /Guardando/ })).toBeDisabled()
        expect(screen.getByRole("button", { name: /Cancelar/ })).toBeDisabled()
    })

    it("closes modal on success", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} onClose={onClose} />)

        // Fill and submit
        await user.type(screen.getByLabelText(/Nombre/), "Test")
        await user.type(screen.getByLabelText(/Importe minimo/), "10")
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            expect(onClose).toHaveBeenCalled()
        })
    })

    it("shows toast on success", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Fill and submit
        await user.type(screen.getByLabelText(/Nombre/), "Test")
        await user.type(screen.getByLabelText(/Importe minimo/), "10")
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            expect(screen.getByText(/Recompensa creada/)).toBeInTheDocument()
        })
    })

    it("shows toast on error", async () => {
        const user = userEvent.setup()

        // Mock error response
        server.use(
            rest.post(API_ROUTES.rewards.base, (req, res, ctx) => {
                return res(
                    ctx.status(400),
                    ctx.json({
                        data: null,
                        messages: [
                            {
                                message: "El nombre es obligatorio",
                                errorCode: "1001",
                            },
                        ],
                    })
                )
            })
        )

        render(<RewardFormModal {...defaultProps} />)

        // Fill and submit
        await user.type(screen.getByLabelText(/Nombre/), "Test")
        await user.type(screen.getByLabelText(/Importe minimo/), "10")
        await user.click(screen.getByRole("button", { name: /Guardar/ }))

        await waitFor(() => {
            expect(screen.getByText(/El nombre es obligatorio/)).toBeInTheDocument()
        })
    })
})
```

#### RewardDeleteDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/__tests__/RewardDeleteDialog.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders variante eliminar (no backings) | Unit | Titulo "Eliminar Recompensa", boton "Eliminar" rojo |
| renders variante desactivar (has backings) | Unit | Titulo "No se puede eliminar", boton "Desactivar" amarillo |
| shows reward name in confirmation | Unit | Muestra nombre del reward a eliminar |
| shows backings count when has backings | Unit | Texto "tiene 23 backings confirmados" |
| calls delete mutation when eliminar clicked | Integration | DELETE /api/rewards/{id} llamado |
| calls deactivate mutation when desactivar clicked | Integration | PUT /api/rewards/{id}/deactivate llamado |
| shows loading state in buttons | Unit | Spinner + texto "Eliminando..." o "Desactivando..." |
| closes dialog on cancel | Unit | Dialog cierra al hacer click en Cancelar |
| closes dialog on success | Integration | Dialog cierra despues de eliminar/desactivar |
| shows toast on success | Integration | Toast "Recompensa eliminada/desactivada" |

**Casos Detallados:**

```typescript
describe("RewardDeleteDialog", () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        reward: mockReward,
        hasBackings: false,
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders variante eliminar (no backings)", () => {
        render(<RewardDeleteDialog {...defaultProps} hasBackings={false} />)

        expect(screen.getByText("Eliminar Recompensa")).toBeInTheDocument()
        expect(screen.getByText(/Esta accion no se puede deshacer/)).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Eliminar/ })).toBeInTheDocument()
    })

    it("renders variante desactivar (has backings)", () => {
        render(<RewardDeleteDialog {...defaultProps} hasBackings={true} />)

        expect(screen.getByText("No se puede eliminar")).toBeInTheDocument()
        expect(screen.getByText(/tiene aportes/)).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Desactivar/ })).toBeInTheDocument()
    })

    it("shows reward name in confirmation", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(screen.getByText(/Descarga Digital/)).toBeInTheDocument()
        expect(screen.getByText(/€10/)).toBeInTheDocument()
    })

    it("shows backings count when has backings", () => {
        render(
            <RewardDeleteDialog
                {...defaultProps}
                hasBackings={true}
                backingsCount={23}
            />
        )

        expect(screen.getByText(/23 backings confirmados/)).toBeInTheDocument()
    })

    it("calls delete mutation when eliminar clicked", async () => {
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} hasBackings={false} />)

        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        await waitFor(() => {
            expect(fetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/rewards/reward-1"),
                expect.objectContaining({ method: "DELETE" })
            )
        })
    })

    it("calls deactivate mutation when desactivar clicked", async () => {
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} hasBackings={true} />)

        await user.click(screen.getByRole("button", { name: /Desactivar/ }))

        await waitFor(() => {
            expect(fetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/rewards/reward-1"),
                expect.objectContaining({
                    method: "PUT",
                    body: expect.stringContaining("esActivo"),
                })
            )
        })
    })

    it("shows loading state in buttons", async () => {
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} hasBackings={false} />)

        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        await waitFor(() => {
            expect(screen.getByText("Eliminando...")).toBeInTheDocument()
        })
    })

    it("closes dialog on cancel", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} onClose={onClose} />)

        await user.click(screen.getByRole("button", { name: /Cancelar/ }))

        expect(onClose).toHaveBeenCalled()
    })

    it("closes dialog on success", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} onClose={onClose} />)

        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        await waitFor(() => {
            expect(onClose).toHaveBeenCalled()
        })
    })

    it("shows toast on success", async () => {
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} />)

        await user.click(screen.getByRole("button", { name: /Eliminar/ }))

        await waitFor(() => {
            expect(screen.getByText(/Recompensa eliminada/)).toBeInTheDocument()
        })
    })
})
```

### 4.2 Hooks

#### useRewards.test.ts

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/hooks/__tests__/useRewards.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns data on success | Unit | Retorna lista de rewards |
| handles loading state | Unit | isLoading true inicialmente |
| handles error state | Unit | error cuando API falla |
| filters by campaniaId | Unit | Query params incluyen campaniaId |
| filters by esActivo | Unit | Query params incluyen esActivo=true |

**Casos Detallados:**

```typescript
describe("useRewards", () => {
    const campaniaId = "campania-1"

    const wrapper = ({ children }: { children: React.ReactNode }) => (
        <AllProviders>{children}</AllProviders>
    )

    beforeEach(() => {
        server.use(...rewardHandlers)
    })

    it("returns data on success", async () => {
        const { result } = renderHook(() => useRewards(campaniaId), { wrapper })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toHaveLength(3)
        expect(result.current.data?.[0].nombre).toBe("Descarga Digital")
    })

    it("handles loading state", () => {
        const { result } = renderHook(() => useRewards(campaniaId), { wrapper })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        server.use(
            rest.get(API_ROUTES.rewards.base, (req, res, ctx) => {
                return res(ctx.status(500))
            })
        )

        const { result } = renderHook(() => useRewards(campaniaId), { wrapper })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(result.current.error).toBeDefined()
    })

    it("filters by campaniaId", async () => {
        const { result } = renderHook(() => useRewards(campaniaId), { wrapper })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        // Verify query params
        expect(fetch).toHaveBeenCalledWith(
            expect.stringContaining(`campaniaId=${campaniaId}`),
            expect.anything()
        )
    })

    it("filters by esActivo", async () => {
        const { result } = renderHook(
            () => useRewards(campaniaId, { esActivo: true }),
            { wrapper }
        )

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(fetch).toHaveBeenCalledWith(
            expect.stringContaining("esActivo=true"),
            expect.anything()
        )
    })
})
```

#### useCreateReward.test.ts

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/hooks/__tests__/useCreateReward.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| mutation success invalidates query cache | Integration | Cache de useRewards invalidado |
| mutation error handles error | Unit | Error propagado correctamente |

**Casos Detallados:**

```typescript
describe("useCreateReward", () => {
    const wrapper = ({ children }: { children: React.ReactNode }) => (
        <AllProviders>{children}</AllProviders>
    )

    beforeEach(() => {
        server.use(...rewardHandlers)
    })

    it("mutation success invalidates query cache", async () => {
        const { result } = renderHook(() => useCreateReward(), { wrapper })

        await act(async () => {
            await result.current.mutateAsync(mockCreateRewardRequest)
        })

        expect(result.current.isSuccess).toBe(true)

        // Verify query invalidation
        // (requires mock QueryClient tracking)
    })

    it("mutation error handles error", async () => {
        server.use(
            rest.post(API_ROUTES.rewards.base, (req, res, ctx) => {
                return res(
                    ctx.status(400),
                    ctx.json({
                        data: null,
                        messages: [{ message: "Error", errorCode: "1001" }],
                    })
                )
            })
        )

        const { result } = renderHook(() => useCreateReward(), { wrapper })

        await act(async () => {
            try {
                await result.current.mutateAsync(mockCreateRewardRequest)
            } catch (error) {
                expect(error).toBeDefined()
            }
        })

        expect(result.current.isError).toBe(true)
    })
})
```

#### useUpdateReward.test.ts

Similar a `useCreateReward.test.ts` pero con PUT.

#### useDeleteReward.test.ts

Similar a `useCreateReward.test.ts` pero con DELETE.

#### useReorderRewards.test.ts

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/hooks/__tests__/useReorderRewards.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| optimistic update funciona | Integration | Lista reordenada inmediatamente antes de API response |
| rollback en error | Integration | Orden original restaurado si API falla |

**Casos Detallados:**

```typescript
describe("useReorderRewards", () => {
    const wrapper = ({ children }: { children: React.ReactNode }) => (
        <AllProviders>{children}</AllProviders>
    )

    beforeEach(() => {
        server.use(...rewardHandlers)
    })

    it("optimistic update funciona", async () => {
        const { result } = renderHook(() => useReorderRewards(), { wrapper })

        // Initial state
        const initialOrder = ["reward-1", "reward-2", "reward-3"]

        // Reorder
        await act(async () => {
            await result.current.mutateAsync(mockReorderRequest)
        })

        // Lista inmediatamente reordenada (optimistic)
        // Verificar que cache tiene nuevo orden antes de API response
    })

    it("rollback en error", async () => {
        server.use(
            rest.put(API_ROUTES.rewards.reorder, (req, res, ctx) => {
                return res(ctx.status(500))
            })
        )

        const { result } = renderHook(() => useReorderRewards(), { wrapper })

        await act(async () => {
            try {
                await result.current.mutateAsync(mockReorderRequest)
            } catch (error) {
                // Expected
            }
        })

        // Verificar que orden original restaurado
    })
})
```

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| RewardsListPage.tsx | 85% | 90% | 80% |
| RewardCard.tsx | 90% | 95% | 85% |
| RewardFormModal.tsx | 80% | 85% | 75% |
| RewardDeleteDialog.tsx | 90% | 95% | 85% |
| RewardStatsCard.tsx | 95% | 100% | 90% |
| EmptyRewardsState.tsx | 100% | 100% | 100% |
| useRewards.ts | 90% | 100% | 85% |
| useCreateReward.ts | 85% | 100% | 80% |
| useUpdateReward.ts | 85% | 100% | 80% |
| useDeleteReward.ts | 85% | 100% | 80% |
| useReorderRewards.ts | 80% | 95% | 75% |

**Meta Global:** 80% en todas las metricas

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- --filter=recompensas

# Watch mode
npm run test:watch

# Solo hooks
npm run test -- hooks/__tests__

# Solo components
npm run test -- components/__tests__
```

## 7. CI/CD Integration

```yaml
- name: Run Admin Frontend Tests
  run: |
    cd src/admin
    npm run test:coverage

- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./src/admin/coverage/coverage-final.json
    flags: admin
```

## 8. Setup y Configuracion

### 8.1 MSW Setup

**Archivo:** `src/admin/src/test-setup.ts` (ya existe, agregar rewardHandlers)

```typescript
import { setupServer } from "msw/node"
import { rewardHandlers } from "./app/(dashboard)/campanias/[id]/recompensas/__mocks__/handlers"

export const server = setupServer(...rewardHandlers)

beforeAll(() => server.listen({ onUnhandledRequest: "error" }))
afterEach(() => server.resetHandlers())
afterAll(() => server.close())
```

### 8.2 Vitest Config

Ya configurado en `vitest.config.ts` existente. No requiere cambios.

### 8.3 Test Utilities

Reutilizar `src/admin/src/test-utils.tsx` existente con:
- `render` con providers
- `screen` de Testing Library
- `userEvent` de Testing Library
- `AllProviders` con QueryClientProvider

## 9. Consideraciones Especiales

### 9.1 Drag & Drop Testing

**@dnd-kit/core** no tiene soporte directo de Testing Library. Estrategias:

1. **Unit Tests:** Mockear eventos de drag & drop
2. **Integration Tests:** Testear botones de reordenar (↑↓) como alternativa accesible
3. **E2E:** Usar Playwright para testear drag & drop real

**Ejemplo mock drag & drop:**

```typescript
it("reorders rewards on drag & drop", async () => {
    const user = userEvent.setup()

    render(<RewardsListPage campaniaId="campania-1" />)

    await waitFor(() => {
        expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
    })

    // Mockear drag & drop event
    const dragHandle = screen.getAllByTestId("drag-handle")[0]

    // Simular drag start
    fireEvent.dragStart(dragHandle)

    // Simular drop
    fireEvent.drop(screen.getAllByTestId("reward-card")[2])

    // Verificar que mutation llamada
    await waitFor(() => {
        expect(fetch).toHaveBeenCalledWith(
            expect.stringContaining("/api/rewards/reorder"),
            expect.objectContaining({ method: "PUT" })
        )
    })
})
```

### 9.2 Form Validation Testing

Usar `react-hook-form` + `zodResolver` en tests:

```typescript
it("validates all fields before submit", async () => {
    const user = userEvent.setup()

    render(<RewardFormModal {...defaultProps} />)

    // Submit sin llenar nada
    await user.click(screen.getByRole("button", { name: /Guardar/ }))

    // Multiples errores mostrados
    await waitFor(() => {
        expect(screen.getByText(/El nombre es obligatorio/)).toBeInTheDocument()
        expect(screen.getByText(/El importe debe ser mayor a 0/)).toBeInTheDocument()
        expect(screen.getByText(/Selecciona un tipo/)).toBeInTheDocument()
    })
})
```

### 9.3 Query Cache Invalidation

Verificar que mutations invalidan queries correctamente:

```typescript
it("invalidates rewards query after create", async () => {
    const queryClient = createTestQueryClient()
    const spy = vi.spyOn(queryClient, "invalidateQueries")

    const wrapper = ({ children }: { children: React.ReactNode }) => (
        <QueryClientProvider client={queryClient}>
            {children}
        </QueryClientProvider>
    )

    const { result } = renderHook(() => useCreateReward(), { wrapper })

    await act(async () => {
        await result.current.mutateAsync(mockCreateRewardRequest)
    })

    expect(spy).toHaveBeenCalledWith({
        queryKey: ["rewards", "campania", "campania-1"],
    })
})
```

### 9.4 Toast Notifications

Verificar toasts usando `screen.getByText()` (requiere mock de sonner):

```typescript
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

it("shows toast on success", async () => {
    const user = userEvent.setup()

    render(<RewardFormModal {...defaultProps} />)

    // Fill and submit
    // ...

    await waitFor(() => {
        expect(toast.success).toHaveBeenCalledWith("Recompensa creada exitosamente")
    })
})
```

## 10. Checklist

- [ ] Mocks definidos para API (`__mocks__/rewards.mock.ts`)
- [ ] MSW handlers configurados (`__mocks__/handlers.ts`)
- [ ] Test utilities configurados (`test-utils.tsx`)
- [ ] Tests de RewardsListPage (10 casos)
- [ ] Tests de RewardCard (10 casos)
- [ ] Tests de RewardFormModal (16 casos)
- [ ] Tests de RewardDeleteDialog (10 casos)
- [ ] Tests de RewardStatsCard (5 casos)
- [ ] Tests de EmptyRewardsState (3 casos)
- [ ] Tests de useRewards (5 casos)
- [ ] Tests de useCreateReward (2 casos)
- [ ] Tests de useUpdateReward (2 casos)
- [ ] Tests de useDeleteReward (2 casos)
- [ ] Tests de useReorderRewards (2 casos)
- [ ] Cobertura 80%+ verificada
- [ ] Tests pasan en CI
- [ ] Drag & drop alternativa testeada (botones ↑↓)
- [ ] Validaciones Zod testeadas
- [ ] Query cache invalidation verificado
- [ ] Toast notifications mockeadas
- [ ] Loading states testeados
- [ ] Error states testeados
- [ ] Empty states testeados

## 11. Notas Adicionales

### 11.1 Patron de Nombrado

Seguir patron existente:
- `NombreComponente_Escenario_ResultadoEsperado`
- En ingles o español segun convenga (proyecto usa español)

### 11.2 AAA Pattern

Todos los tests siguen Arrange-Act-Assert:

```typescript
it("ejemplo test", async () => {
    // Arrange
    const user = userEvent.setup()
    render(<Component {...props} />)

    // Act
    await user.click(screen.getByRole("button"))

    // Assert
    expect(screen.getByText("Result")).toBeInTheDocument()
})
```

### 11.3 Helpers de Test

Crear helpers para reducir boilerplate:

```typescript
// test-helpers.ts
export const fillRewardForm = async (user: UserEvent, data: Partial<CreateRewardRequest>) => {
    if (data.nombre) {
        await user.type(screen.getByLabelText(/Nombre/), data.nombre)
    }
    if (data.descripcion) {
        await user.type(screen.getByLabelText(/Descripcion/), data.descripcion)
    }
    if (data.importeMinimo) {
        await user.type(screen.getByLabelText(/Importe/), data.importeMinimo.toString())
    }
}

export const submitForm = async (user: UserEvent) => {
    await user.click(screen.getByRole("button", { name: /Guardar/ }))
}
```

### 11.4 Timeouts

Usar `waitFor` con timeouts razonables:

```typescript
await waitFor(
    () => {
        expect(screen.getByText("Result")).toBeInTheDocument()
    },
    { timeout: 3000 } // 3 segundos max
)
```

### 11.5 Cleanup

Vitest hace cleanup automatico. No necesitar `afterEach(() => cleanup())`.

---

**Fin de la estrategia de testing frontend Admin para definir-recompensas.**
