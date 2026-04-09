# Estrategia de Testing: cp-tareas-promocion (Admin)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/admin
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 18 | 75% |
| Integration Tests | 16 | 88% |
| Total | 34 | 80%+ |

### Distribucion por Modulo

| Modulo | Tipo | Tests |
|--------|------|-------|
| TareasPendientesTab | Integration | 6 |
| TareaPendienteCard | Unit | 7 |
| ValidarTareaDialog | Integration | 8 |
| RechazarTareaDialog | Integration | 8 |
| useTareasPendientes | Unit | 5 |
| useValidarTarea | Unit | 4 |
| useRechazarTarea | Unit | 4 |
| tareas-pendientes.service | Unit | 6 |
| **Total** | | **48** |

> Nota: el conteo final es 48 casos individuales distribuidos en los 7 archivos de test. El resumen de arriba agrupa por categoria (unit vs integration) contando los archivos, no los casos.

---

## 2. Estructura de Tests

```
src/admin/src/
└── app/
    └── (dashboard)/
        └── crowdpromotion/
            └── programas/
                └── [programaId]/
                    └── tareas-pendientes/
                        ├── components/
                        │   ├── __tests__/
                        │   │   ├── TareasPendientesTab.test.tsx
                        │   │   ├── TareaPendienteCard.test.tsx
                        │   │   ├── ValidarTareaDialog.test.tsx
                        │   │   └── RechazarTareaDialog.test.tsx
                        │   ├── TareasPendientesTab.tsx
                        │   ├── TareaPendienteCard.tsx
                        │   ├── ValidarTareaDialog.tsx
                        │   └── RechazarTareaDialog.tsx
                        ├── hooks/
                        │   ├── __tests__/
                        │   │   ├── use-tareas-pendientes.test.ts
                        │   │   ├── use-validar-tarea.test.ts
                        │   │   └── use-rechazar-tarea.test.ts
                        │   ├── use-tareas-pendientes.ts
                        │   ├── use-validar-tarea.ts
                        │   └── use-rechazar-tarea.ts
                        └── services/
                            ├── __tests__/
                            │   └── tareas-pendientes.service.test.ts
                            └── tareas-pendientes.service.ts

src/admin/src/
└── __mocks__/
    └── cp-tareas-promocion.mock.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/cp-tareas-promocion.mock.ts`

```typescript
import type {
    TareaPendienteItem,
    TareasPendientesResponse,
    ValidarTareaResponse,
    RechazarTareaResponse,
} from "@shared/types"

// ─── Fixtures base ────────────────────────────────────────────────────────────

export const PROGRAMA_ID = "3fa85f64-5717-4562-b3fc-2c963f66afa6"

export const mockTareaPendienteItem: TareaPendienteItem = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    tareaId: "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    tareaNombre: "Comparte en Instagram Stories",
    promotorId: "p1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    promotorNombre: "Maria Lopez",
    promotorTipoNombre: "Influencer",
    urlPruebaCompletado: "https://instagram.com/stories/maria_promo_abc123",
    comentarioPromotor: "Comparto mi story con la campa\u00f1a tal como se indico",
    vecesCompletada: 1,
    fechaUltimaCompletada: "2026-03-20T15:30:00Z",
}

export const mockTareaPendienteItemSinComentario: TareaPendienteItem = {
    ...mockTareaPendienteItem,
    tareaPromotorId: "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
    tareaId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    tareaNombre: "Publica un TikTok",
    promotorId: "p2c3d4e5-f6a7-8901-bc23-de45fa678901",
    promotorNombre: "Carlos Ruiz",
    promotorTipoNombre: undefined,
    urlPruebaCompletado: "https://tiktok.com/@carlosruiz/video/123456789",
    comentarioPromotor: undefined,
    vecesCompletada: 3,
    fechaUltimaCompletada: "2026-03-19T10:00:00Z",
}

export const mockTareaPendienteItemSinUrl: TareaPendienteItem = {
    ...mockTareaPendienteItem,
    tareaPromotorId: "d4e5f6a7-b8c9-0123-de45-fa6789012345",
    urlPruebaCompletado: undefined,
    comentarioPromotor: undefined,
}

export const mockTareasPendientesResponse: TareasPendientesResponse = {
    items: [mockTareaPendienteItem, mockTareaPendienteItemSinComentario],
    totalCount: 2,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}

export const mockTareasPendientesResponsePaginada: TareasPendientesResponse = {
    items: Array.from({ length: 10 }, (_, i) => ({
        ...mockTareaPendienteItem,
        tareaPromotorId: `tarea-promotor-${i + 1}`,
        promotorNombre: `Promotor ${i + 1}`,
    })),
    totalCount: 25,
    page: 1,
    pageSize: 10,
    totalPages: 3,
}

export const mockTareasPendientesResponseVacia: TareasPendientesResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

export const mockValidarTareaResponse: ValidarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 3,
    estadoTareaNombre: "Validada",
    recompensaAcreditada: 5.00,
    monedaNombre: "EUR",
    puntosAcreditados: undefined,
}

export const mockValidarTareaResponseSinRecompensa: ValidarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 3,
    estadoTareaNombre: "Validada",
    recompensaAcreditada: undefined,
    monedaNombre: undefined,
    puntosAcreditados: undefined,
}

export const mockRechazarTareaResponse: RechazarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 4,
    estadoTareaNombre: "Rechazada",
}

// ─── Helpers ──────────────────────────────────────────────────────────────────

export function buildTareaPendiente(
    overrides: Partial<TareaPendienteItem> = {}
): TareaPendienteItem {
    return { ...mockTareaPendienteItem, ...overrides }
}

export function buildTareasPendientesResponse(
    count: number,
    page = 1,
    pageSize = 10
): TareasPendientesResponse {
    return {
        items: Array.from({ length: Math.min(count, pageSize) }, (_, i) =>
            buildTareaPendiente({
                tareaPromotorId: `tarea-promotor-${(page - 1) * pageSize + i + 1}`,
                promotorNombre: `Promotor ${(page - 1) * pageSize + i + 1}`,
            })
        ),
        totalCount: count,
        page,
        pageSize,
        totalPages: Math.ceil(count / pageSize),
    }
}
```

### 3.2 Service Mock

Los tests de componentes y hooks mockean el service directamente con `vi.mock`. No se usa MSW en esta feature porque el patron establecido en el proyecto es mockear el service layer con `vi.mock`.

```typescript
// Patron de mock de service (usado en tests de hooks)
vi.mock("@/services/tareas-pendientes.service", () => ({
    tareasPendientesService: {
        getTareasPendientes: vi.fn(),
        validarTarea: vi.fn(),
        rechazarTarea: vi.fn(),
    },
}))
```

### 3.3 Mock de hooks (usado en tests de componentes)

```typescript
// Patron para TareasPendientesTab y dialogs
const mockValidarMutate = vi.fn()
const mockRechazarMutate = vi.fn()

vi.mock("@/hooks/use-validar-tarea", () => ({
    useValidarTarea: () => ({
        mutate: mockValidarMutate,
        isPending: false,
    }),
}))

vi.mock("@/hooks/use-rechazar-tarea", () => ({
    useRechazarTarea: () => ({
        mutate: mockRechazarMutate,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))
```

### 3.4 Test Utilities (existente en el proyecto)

El proyecto ya tiene `src/admin/src/test-utils.tsx` con `renderWithProviders` (exportado como `render`) que envuelve en `QueryClientProvider`. Se reutiliza sin modificaciones.

```typescript
// Patron para tests de hooks con QueryClient
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

---

### 4.1 TareasPendientesTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/components/__tests__/TareasPendientesTab.test.tsx`

**Tipo:** Integration (renderiza con QueryClient, mockea el hook de datos)

**Mocks necesarios:**
- `vi.mock("@/hooks/use-tareas-pendientes")` - mock del hook de datos
- `vi.mock("@/hooks/use-validar-tarea")` - para que ValidarTareaDialog funcione
- `vi.mock("@/hooks/use-rechazar-tarea")` - para que RechazarTareaDialog funcione
- `vi.mock("sonner")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders loading skeleton | Integration | Cuando `isLoading: true`, muestra skeleton y no lista |
| 2 | renders empty state | Integration | Cuando `items: []` y `totalCount: 0`, muestra mensaje "No hay tareas pendientes" |
| 3 | renders list of pending tasks | Integration | Muestra una TareaPendienteCard por cada item del response |
| 4 | shows correct count badge in tab | Integration | El badge del tab muestra `totalCount` del response |
| 5 | shows pagination controls when totalPages > 1 | Integration | Controles de paginacion visibles con 3 paginas |
| 6 | handles error state | Integration | Cuando `isError: true`, muestra mensaje de error |

**Casos Detallados:**

```
1. renders loading skeleton
   Arrange: mock useTareasPendientes -> { isLoading: true, data: undefined }
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  pantalla muestra skeleton (role="status" o data-testid="skeleton")
   Assert:  NO muestra ninguna card de tarea

2. renders empty state
   Arrange: mock useTareasPendientes -> { isLoading: false, isSuccess: true,
            data: mockTareasPendientesResponseVacia }
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  screen.getByText(/No hay tareas pendientes/) o similar

3. renders list of pending tasks
   Arrange: mock useTareasPendientes -> { isLoading: false, isSuccess: true,
            data: mockTareasPendientesResponse }   // 2 items
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  screen.getAllByRole("article") o similar tiene length 2
   Assert:  screen.getByText("Maria Lopez") esta en el documento
   Assert:  screen.getByText("Carlos Ruiz") esta en el documento

4. shows correct count badge in tab
   Arrange: mock useTareasPendientes -> { data: { ...response, totalCount: 7 } }
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  screen.getByText("7") esta visible (badge del tab)

5. shows pagination controls when totalPages > 1
   Arrange: mock useTareasPendientes -> { data: mockTareasPendientesResponsePaginada }
            // totalCount: 25, totalPages: 3
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  controles de paginacion son visibles (Anterior / Siguiente o similar)

6. handles error state
   Arrange: mock useTareasPendientes -> { isLoading: false, isError: true, error: new Error() }
   Act:     render(<TareasPendientesTab programaId={PROGRAMA_ID} />)
   Assert:  screen muestra algun mensaje de error ("Error al cargar" o similar)
```

---

### 4.2 TareaPendienteCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/components/__tests__/TareaPendienteCard.test.tsx`

**Tipo:** Unit (componente presentacional, sin queries propias)

**Mocks necesarios:** Ninguno (componente puro presentacional con handlers pasados por props)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders promotor name and tarea name | Unit | Muestra `promotorNombre` y `tareaNombre` correctamente |
| 2 | renders proof URL as external link | Unit | `urlPruebaCompletado` es un `<a target="_blank" rel="noopener noreferrer">` |
| 3 | renders promotor comment when present | Unit | Muestra `comentarioPromotor` si existe |
| 4 | does not render comment section when absent | Unit | No muestra seccion de comentario cuando `comentarioPromotor` es undefined |
| 5 | renders vecesCompletada correctly | Unit | Muestra el numero de veces completada |
| 6 | calls onValidar with tareaPromotorId when Validar clicked | Unit | Handler `onValidar` es llamado con el id correcto |
| 7 | calls onRechazar with tareaPromotorId when Rechazar clicked | Unit | Handler `onRechazar` es llamado con el id correcto |

**Casos Detallados:**

```
1. renders promotor name and tarea name
   Arrange: item = mockTareaPendienteItem
   Act:     render(<TareaPendienteCard item={item} onValidar={vi.fn()} onRechazar={vi.fn()} />)
   Assert:  screen.getByText("Maria Lopez") esta en el documento
   Assert:  screen.getByText("Comparte en Instagram Stories") esta en el documento

2. renders proof URL as external link
   Arrange: item = mockTareaPendienteItem
   Act:     render(<TareaPendienteCard item={item} onValidar={vi.fn()} onRechazar={vi.fn()} />)
   Assert:  el link con href="https://instagram.com/stories/maria_promo_abc123" existe
   Assert:  el link tiene attribute target="_blank"
   Assert:  el link tiene attribute rel que incluye "noopener"

3. renders promotor comment when present
   Arrange: item = mockTareaPendienteItem  // tiene comentarioPromotor
   Act:     render(...)
   Assert:  screen.getByText("Comparto mi story con la campaña tal como se indico")

4. does not render comment section when absent
   Arrange: item = mockTareaPendienteItemSinComentario  // comentarioPromotor: undefined
   Act:     render(...)
   Assert:  "Comparto" no esta en el documento (ninguna mencion al comentario)

5. renders vecesCompletada correctly
   Arrange: item = mockTareaPendienteItemSinComentario  // vecesCompletada: 3
   Act:     render(...)
   Assert:  screen.getByText(/3/) esta en el documento (en el contexto de repeticiones)

6. calls onValidar with tareaPromotorId when Validar clicked
   Arrange: onValidar = vi.fn()
            item = mockTareaPendienteItem  // tareaPromotorId: "b2c3d4e5-..."
   Act:     await user.click(screen.getByRole("button", { name: /Validar/i }))
   Assert:  onValidar toHaveBeenCalledWith("b2c3d4e5-f6a7-8901-bc23-de45fa678901")

7. calls onRechazar with tareaPromotorId when Rechazar clicked
   Arrange: onRechazar = vi.fn()
            item = mockTareaPendienteItem
   Act:     await user.click(screen.getByRole("button", { name: /Rechazar/i }))
   Assert:  onRechazar toHaveBeenCalledWith("b2c3d4e5-f6a7-8901-bc23-de45fa678901")
```

---

### 4.3 ValidarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/components/__tests__/ValidarTareaDialog.test.tsx`

**Tipo:** Integration (interaccion con formulario + mutacion + toast)

**Mocks necesarios:**
- `vi.mock("@/hooks/use-validar-tarea")` con `mockValidarMutate`
- `vi.mock("sonner")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders dialog with item data read-only | Integration | Muestra nombre del promotor y nombre de la tarea en modo lectura |
| 2 | renders proof URL as clickable link | Integration | URL de prueba es un link externo dentro del dialog |
| 3 | shows reward accrual info | Integration | Muestra info de recompensa a acreditar (cuando el item la tiene) |
| 4 | comment field is optional - submits without it | Integration | El formulario se envia exitosamente sin completar `comentarioValidacion` |
| 5 | submits with optional comment | Integration | Al escribir comentario y enviar, la mutacion recibe `{ comentarioValidacion }` |
| 6 | calls onClose after successful mutation | Integration | `onClose` se llama cuando `onSuccess` del mutate es ejecutado |
| 7 | shows success toast on success | Integration | `toast.success` con mensaje de validacion y recompensa |
| 8 | shows error toast on mutation error | Integration | `toast.error` cuando `onError` del mutate es ejecutado |

**Casos Detallados:**

```
1. renders dialog with item data read-only
   Arrange: item = mockTareaPendienteItem, isOpen = true
   Act:     render(<ValidarTareaDialog isOpen={true} onClose={vi.fn()}
                   item={item} programaId={PROGRAMA_ID} />)
   Assert:  screen.getByText("Maria Lopez") visible
   Assert:  screen.getByText("Comparte en Instagram Stories") visible
   Assert:  NO existe input editable con el nombre del promotor (es texto, no input)

2. renders proof URL as clickable link
   Arrange: item = mockTareaPendienteItem  // urlPruebaCompletado definida
   Act:     render(...)
   Assert:  link con href="https://instagram.com/stories/maria_promo_abc123" existe
   Assert:  link tiene target="_blank"

3. shows reward accrual info
   Arrange: item = buildTareaPendiente()  // la card no tiene importeRecompensa directamente
            Nota: el dialog puede mostrar info derivada del response de validacion
            o puede mostrar datos del item si la implementacion los incluye.
            Si el componente no tiene datos de recompensa pre-carga, este test
            verifica que el area de "Recompensa a acreditar" sea visible en la UI.
   Act:     render(...)
   Assert:  screen.getByText(/Recompensa/i) o seccion informativa visible

4. comment field is optional - submits without it
   Arrange: mockValidarMutate implementado normalmente (no llama callbacks auto)
   Act:     render(...)
            await user.click(screen.getByRole("button", { name: /Confirmar|Validar/i }))
   Assert:  mockValidarMutate toHaveBeenCalledWith(
              expect.objectContaining({
                  tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                  programaId: PROGRAMA_ID,
                  data: { comentarioValidacion: undefined }
              }),
              expect.objectContaining({ onSuccess: expect.any(Function) })
            )

5. submits with optional comment
   Arrange: render(...)
   Act:     await user.type(screen.getByRole("textbox"), "Buen trabajo")
            await user.click(screen.getByRole("button", { name: /Confirmar|Validar/i }))
   Assert:  mockValidarMutate toHaveBeenCalledWith(
              expect.objectContaining({ data: { comentarioValidacion: "Buen trabajo" } }),
              expect.any(Object)
            )

6. calls onClose after successful mutation
   Arrange: const onClose = vi.fn()
            mockValidarMutate implementado para llamar onSuccess automaticamente:
            mockValidarMutate.mockImplementation((_args, { onSuccess }) => onSuccess(mockValidarTareaResponse))
   Act:     render(<ValidarTareaDialog ... onClose={onClose} />)
            await user.click(screen.getByRole("button", { name: /Confirmar|Validar/i }))
   Assert:  onClose toHaveBeenCalled()

7. shows success toast on success
   Arrange: mockValidarMutate llama onSuccess automaticamente
   Act:     click Confirmar
   Assert:  toast.success toHaveBeenCalledWith(
              expect.stringContaining("validada")
            )
   Nota:    el mensaje incluye la recompensa acreditada si `recompensaAcreditada` esta en el response

8. shows error toast on mutation error
   Arrange: mockValidarMutate.mockImplementation((_args, { onError }) =>
              onError(new Error("Server error")))
   Act:     click Confirmar
   Assert:  toast.error toHaveBeenCalledWith(expect.stringContaining("Error"))
   Assert:  onClose NO toHaveBeenCalled()
```

---

### 4.4 RechazarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/components/__tests__/RechazarTareaDialog.test.tsx`

**Tipo:** Integration (formulario con validacion requerida + mutacion + toast)

**Mocks necesarios:**
- `vi.mock("@/hooks/use-rechazar-tarea")` con `mockRechazarMutate`
- `vi.mock("sonner")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders dialog with item info | Integration | Muestra nombre del promotor y nombre de la tarea |
| 2 | shows rejection reason textarea | Integration | Existe textarea para `comentarioValidacion` (obligatorio) |
| 3 | validates reason is required on submit | Integration | Sin motivo, muestra error de validacion al intentar enviar |
| 4 | shows character counter | Integration | Counter actualiza al escribir en el textarea |
| 5 | shows warning message about rejection | Integration | Mensaje de advertencia sobre el impacto del rechazo |
| 6 | submits with rejection reason | Integration | Mutacion recibe `comentarioValidacion` con el texto ingresado |
| 7 | calls onClose after successful rejection | Integration | `onClose` se llama cuando `onSuccess` del mutate ejecuta |
| 8 | shows error toast on mutation error | Integration | `toast.error` cuando la mutacion falla |

**Casos Detallados:**

```
1. renders dialog with item info
   Arrange: item = mockTareaPendienteItem, isOpen = true
   Act:     render(<RechazarTareaDialog isOpen={true} onClose={vi.fn()}
                   item={item} programaId={PROGRAMA_ID} />)
   Assert:  screen.getByText("Maria Lopez") visible
   Assert:  screen.getByText("Comparte en Instagram Stories") visible

2. shows rejection reason textarea
   Arrange: render(...)
   Act:     -
   Assert:  screen.getByRole("textbox") visible (textarea para motivo)
   Assert:  el placeholder o label indica que es obligatorio o el campo "Motivo de rechazo"

3. validates reason is required on submit
   Arrange: render(...)
   Act:     await user.click(screen.getByRole("button", { name: /Rechazar|Confirmar/i }))
   Assert:  waitFor -> screen.getByText(/motivo.*obligatorio|El motivo es obligatorio/i) visible
   Assert:  mockRechazarMutate NOT toHaveBeenCalled()

4. shows character counter
   Arrange: render(...)
   Act:     await user.type(textarea, "URL invalida")  // 12 caracteres
   Assert:  screen.getByText(/12\/500/) visible
   Nota:    El limite de ComentarioValidacion es 500 caracteres (contracts-plan.md seccion 3.2)

5. shows warning message about rejection
   Arrange: render(...)
   Act:     -
   Assert:  screen.getByText(/no se puede deshacer|el promotor podra re-enviar/i) visible
   Nota:    El dialog debe informar que el promotor puede re-enviar tras el rechazo

6. submits with rejection reason
   Arrange: render(...)
   Act:     await user.type(textarea, "La URL no corresponde a la tarea solicitada")
            await user.click(screen.getByRole("button", { name: /Rechazar|Confirmar/i }))
   Assert:  mockRechazarMutate toHaveBeenCalledWith(
              expect.objectContaining({
                  tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                  programaId: PROGRAMA_ID,
                  data: { comentarioValidacion: "La URL no corresponde a la tarea solicitada" }
              }),
              expect.objectContaining({ onSuccess: expect.any(Function) })
            )

7. calls onClose after successful rejection
   Arrange: const onClose = vi.fn()
            mockRechazarMutate.mockImplementation((_args, { onSuccess }) =>
              onSuccess(mockRechazarTareaResponse))
   Act:     type motivo + click Rechazar
   Assert:  toast.success toHaveBeenCalledWith(expect.stringContaining("rechazada"))
   Assert:  onClose toHaveBeenCalled()

8. shows error toast on mutation error
   Arrange: mockRechazarMutate.mockImplementation((_args, { onError }) =>
              onError(new Error("Network error")))
   Act:     type motivo + click Rechazar
   Assert:  toast.error toHaveBeenCalledWith(expect.stringContaining("Error"))
   Assert:  onClose NOT toHaveBeenCalled()
```

---

### 4.5 useTareasPendientes

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/hooks/__tests__/use-tareas-pendientes.test.ts`

**Tipo:** Unit (hook de query)

**Mocks necesarios:**
- `vi.mock("@/services/tareas-pendientes.service")`

**Setup:**
```typescript
const wrapper = createWrapper()  // QueryClientProvider con retry: false

vi.mocked(tareasPendientesService.getTareasPendientes)
    .mockResolvedValue(mockTareasPendientesResponse)
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | returns paginated data on success | Unit | Retorna `items`, `totalCount`, `totalPages` correctamente |
| 2 | handles loading state initially | Unit | `isLoading: true` antes de que la query resuelva |
| 3 | handles error state when service fails | Unit | `isError: true` cuando el service rechaza |
| 4 | does not fetch when programaId is empty | Unit | El service no se llama si `programaId === ""` |
| 5 | calls service with correct params including pagination | Unit | El service recibe `page` y `pageSize` correctos |

**Casos Detallados:**

```
1. returns paginated data on success
   Arrange: vi.mocked(service.getTareasPendientes).mockResolvedValue(mockTareasPendientesResponse)
   Act:     const { result } = renderHook(() => useTareasPendientes(PROGRAMA_ID), { wrapper })
            await waitFor(() => expect(result.current.isSuccess).toBe(true))
   Assert:  result.current.data.items.length == 2
   Assert:  result.current.data.totalCount == 2
   Assert:  result.current.data.page == 1

2. handles loading state initially
   Arrange: vi.mocked(service.getTareasPendientes).mockReturnValue(new Promise(() => {}))
   Act:     const { result } = renderHook(() => useTareasPendientes(PROGRAMA_ID), { wrapper })
   Assert:  result.current.isLoading == true

3. handles error state when service fails
   Arrange: vi.mocked(service.getTareasPendientes).mockRejectedValue(new Error("Network error"))
   Act:     renderHook + waitFor isError
   Assert:  result.current.isError == true
   Assert:  result.current.error definido

4. does not fetch when programaId is empty
   Arrange: -
   Act:     renderHook(() => useTareasPendientes(""), { wrapper })
   Assert:  service.getTareasPendientes NOT toHaveBeenCalled()

5. calls service with correct params including pagination
   Arrange: vi.mocked(service.getTareasPendientes).mockResolvedValue(mockTareasPendientesResponse)
   Act:     renderHook(() => useTareasPendientes(PROGRAMA_ID, { page: 2, pageSize: 5 }), { wrapper })
            await waitFor(isSuccess)
   Assert:  service.getTareasPendientes toHaveBeenCalledWith(PROGRAMA_ID, { page: 2, pageSize: 5 })
```

---

### 4.6 useValidarTarea

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/hooks/__tests__/use-validar-tarea.test.ts`

**Tipo:** Unit (hook de mutacion)

**Mocks necesarios:**
- `vi.mock("@/services/tareas-pendientes.service")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | calls validarTarea service on mutate | Unit | El service recibe `programaId`, `tareaPromotorId` y `data` correctamente |
| 2 | invalidates tareas-pendientes query on success | Unit | `queryClient.invalidateQueries` con la key correcta tras exito |
| 3 | returns success response data | Unit | `result.current.data` coincide con `mockValidarTareaResponse` |
| 4 | handles service error | Unit | `isError: true` cuando el service rechaza |

**Casos Detallados:**

```
1. calls validarTarea service on mutate
   Arrange: vi.mocked(service.validarTarea).mockResolvedValue(mockValidarTareaResponse)
   Act:     const { result } = renderHook(() => useValidarTarea(), { wrapper })
            await act(async () => {
                result.current.mutate({
                    programaId: PROGRAMA_ID,
                    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                    data: { comentarioValidacion: undefined }
                })
            })
            await waitFor(() => expect(result.current.isSuccess).toBe(true))
   Assert:  service.validarTarea toHaveBeenCalledWith(
              PROGRAMA_ID,
              "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
              { comentarioValidacion: undefined }
            )

2. invalidates tareas-pendientes query on success
   Arrange: Espiar queryClient.invalidateQueries via wrapper dedicado con queryClient accesible
            vi.mocked(service.validarTarea).mockResolvedValue(mockValidarTareaResponse)
   Act:     mutate + waitFor isSuccess
   Assert:  queryClient.invalidateQueries fue llamado con queryKey que incluye
            ["crowdpromotion", "tareas", PROGRAMA_ID, "pendientes"]

3. returns success response data
   Arrange: vi.mocked(service.validarTarea).mockResolvedValue(mockValidarTareaResponse)
   Act:     mutate + waitFor isSuccess
   Assert:  result.current.data.estadoTareaId == 3
   Assert:  result.current.data.recompensaAcreditada == 5.00

4. handles service error
   Arrange: vi.mocked(service.validarTarea).mockRejectedValue(new Error("Server error"))
   Act:     mutate + waitFor isError
   Assert:  result.current.isError == true
   Assert:  result.current.error definido
```

---

### 4.7 useRechazarTarea

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/hooks/__tests__/use-rechazar-tarea.test.ts`

**Tipo:** Unit (hook de mutacion)

**Mocks necesarios:**
- `vi.mock("@/services/tareas-pendientes.service")`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | calls rechazarTarea service on mutate | Unit | El service recibe los parametros correctos |
| 2 | invalidates tareas-pendientes query on success | Unit | Query key correcta invalidada tras rechazo exitoso |
| 3 | returns rechazada response | Unit | `data.estadoTareaId == 4` tras exito |
| 4 | handles service error | Unit | `isError: true` cuando el service rechaza |

**Casos Detallados:**

```
1. calls rechazarTarea service on mutate
   Arrange: vi.mocked(service.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)
   Act:     const { result } = renderHook(() => useRechazarTarea(), { wrapper })
            await act(async () => {
                result.current.mutate({
                    programaId: PROGRAMA_ID,
                    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                    data: { comentarioValidacion: "La URL no corresponde a la tarea" }
                })
            })
            await waitFor(isSuccess)
   Assert:  service.rechazarTarea toHaveBeenCalledWith(
              PROGRAMA_ID,
              "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
              { comentarioValidacion: "La URL no corresponde a la tarea" }
            )

2. invalidates tareas-pendientes query on success
   Arrange: vi.mocked(service.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)
   Act:     mutate + waitFor isSuccess
   Assert:  queryClient.invalidateQueries fue llamado con queryKey que incluye
            ["crowdpromotion", "tareas", PROGRAMA_ID, "pendientes"]

3. returns rechazada response
   Arrange: vi.mocked(service.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)
   Act:     mutate + waitFor isSuccess
   Assert:  result.current.data.estadoTareaId == 4
   Assert:  result.current.data.estadoTareaNombre == "Rechazada"

4. handles service error
   Arrange: vi.mocked(service.rechazarTarea).mockRejectedValue(new Error("Forbidden"))
   Act:     mutate + waitFor isError
   Assert:  result.current.isError == true
```

---

### 4.8 tareas-pendientes.service

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[programaId]/tareas-pendientes/services/__tests__/tareas-pendientes.service.test.ts`

**Tipo:** Unit (service que llama apiFetch)

**Mocks necesarios:**
- `vi.mock("@/lib/api-client")` - mockear `apiFetch`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | getTareasPendientes calls correct URL without params | Unit | URL base sin query params |
| 2 | getTareasPendientes calls correct URL with pagination params | Unit | URL incluye `page` y `pageSize` como query params |
| 3 | getTareasPendientes returns items from response | Unit | Extrae `data.items` y `data.totalCount` del ServiceResponse |
| 4 | validarTarea calls PATCH with correct URL and body | Unit | Verifica metodo PATCH, URL y body enviado |
| 5 | rechazarTarea calls PATCH with correct URL and body | Unit | Verifica metodo PATCH, URL y body con `comentarioValidacion` |
| 6 | handles apiFetch error propagation | Unit | Re-lanza el error de apiFetch al caller |

**Casos Detallados:**

```
1. getTareasPendientes calls correct URL without params
   Arrange: vi.mocked(apiFetch).mockResolvedValue({
              data: mockTareasPendientesResponse, messages: []
            })
   Act:     await tareasPendientesService.getTareasPendientes(PROGRAMA_ID)
   Assert:  apiFetch toHaveBeenCalledWith(
              `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-pendientes`
            )

2. getTareasPendientes calls correct URL with pagination params
   Arrange: vi.mocked(apiFetch).mockResolvedValue({ data: mockTareasPendientesResponse, messages: [] })
   Act:     await tareasPendientesService.getTareasPendientes(PROGRAMA_ID, { page: 2, pageSize: 5 })
   Assert:  apiFetch toHaveBeenCalledWith(
              `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-pendientes?page=2&pageSize=5`
            )

3. getTareasPendientes returns items from response
   Arrange: vi.mocked(apiFetch).mockResolvedValue({
              data: mockTareasPendientesResponse, messages: []
            })
   Act:     const result = await tareasPendientesService.getTareasPendientes(PROGRAMA_ID)
   Assert:  result.items.length == 2
   Assert:  result.totalCount == 2
   Assert:  result.items[0].promotorNombre == "Maria Lopez"

4. validarTarea calls PATCH with correct URL and body
   Arrange: vi.mocked(apiFetch).mockResolvedValue({
              data: mockValidarTareaResponse, messages: []
            })
   Act:     await tareasPendientesService.validarTarea(
              PROGRAMA_ID,
              "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
              { comentarioValidacion: "Bien hecho" }
            )
   Assert:  apiFetch toHaveBeenCalledWith(
              `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-promotor/b2c3d4e5-f6a7-8901-bc23-de45fa678901/validar`,
              expect.objectContaining({ method: "PATCH", body: { comentarioValidacion: "Bien hecho" } })
            )

5. rechazarTarea calls PATCH with correct URL and body
   Arrange: vi.mocked(apiFetch).mockResolvedValue({
              data: mockRechazarTareaResponse, messages: []
            })
   Act:     await tareasPendientesService.rechazarTarea(
              PROGRAMA_ID,
              "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
              { comentarioValidacion: "La URL no corresponde" }
            )
   Assert:  apiFetch toHaveBeenCalledWith(
              `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-promotor/b2c3d4e5-f6a7-8901-bc23-de45fa678901/rechazar`,
              expect.objectContaining({
                  method: "PATCH",
                  body: { comentarioValidacion: "La URL no corresponde" }
              })
            )

6. handles apiFetch error propagation
   Arrange: vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))
   Act:     await expect(
              tareasPendientesService.getTareasPendientes(PROGRAMA_ID)
            ).rejects.toThrow("Network error")
```

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Descripcion |
|---------|--------|-----------|----------|-------------|
| TareasPendientesTab.tsx | 85% | 90% | 80% | Tab + paginacion + estado vacio |
| TareaPendienteCard.tsx | 95% | 100% | 90% | Componente presentacional |
| ValidarTareaDialog.tsx | 85% | 90% | 80% | Dialog con formulario opcional |
| RechazarTareaDialog.tsx | 88% | 90% | 85% | Dialog con validacion requerida |
| use-tareas-pendientes.ts | 90% | 100% | 85% | Hook de query paginada |
| use-validar-tarea.ts | 90% | 100% | 85% | Hook de mutacion PATCH |
| use-rechazar-tarea.ts | 90% | 100% | 85% | Hook de mutacion PATCH |
| tareas-pendientes.service.ts | 95% | 100% | 90% | Service capa de acceso API |

**Meta Global:** 80% en todas las metricas

---

## 6. Flujos Criticos a Cubrir (Integration)

Los siguientes flujos tienen prioridad maxima porque representan el valor de negocio central de la feature:

### Flujo A: Validar una tarea exitosamente

```
TareasPendientesTab muestra lista
  -> usuario hace click en "Validar" de una TareaPendienteCard (onValidar callback)
  -> TareasPendientesTab abre ValidarTareaDialog con el tareaPromotorId seleccionado
  -> usuario confirma (sin comentario)
  -> useValidarTarea.mutate se llama con los datos correctos
  -> onSuccess: toast.success visible, dialog se cierra, query se invalida
```

Este flujo completo esta cubierto por la combinacion de:
- TareaPendienteCard test #6 (onValidar callback)
- ValidarTareaDialog tests #4 y #6 (submit sin comentario + onClose)
- useValidarTarea test #2 (invalidacion de query)

### Flujo B: Rechazar con motivo y validacion

```
TareasPendientesTab muestra lista
  -> usuario hace click en "Rechazar" de una TareaPendienteCard (onRechazar callback)
  -> TareasPendientesTab abre RechazarTareaDialog
  -> usuario intenta enviar sin motivo -> error de validacion visible
  -> usuario escribe motivo -> counter actualiza
  -> usuario confirma -> useRechazarTarea.mutate se llama
  -> onSuccess: toast.success visible, dialog se cierra, query se invalida
```

Este flujo esta cubierto por la combinacion de:
- TareaPendienteCard test #7 (onRechazar callback)
- RechazarTareaDialog tests #3, #4, #6, #7 (validacion + counter + submit + onClose)
- useRechazarTarea test #2 (invalidacion de query)

---

## 7. Casos Edge a Cubrir

| Caso | Componente | Test File | Descripcion |
|------|-----------|-----------|-------------|
| Promotor sin tipo | TareaPendienteCard | TareaPendienteCard.test.tsx | `promotorTipoNombre: undefined` no rompe el render |
| URL de prueba ausente | TareaPendienteCard | TareaPendienteCard.test.tsx | `urlPruebaCompletado: undefined` no muestra link roto |
| Validacion sin recompensa | ValidarTareaDialog | ValidarTareaDialog.test.tsx | `recompensaAcreditada: undefined` en el response |
| Paginacion pagina 2 | useTareasPendientes | use-tareas-pendientes.test.ts | Hook llama service con `page: 2` |
| programaId vacio | useTareasPendientes | use-tareas-pendientes.test.ts | No hace fetch con string vacio |
| 1 sola tarea pendiente | TareasPendientesTab | TareasPendientesTab.test.tsx | `totalCount: 1` visible en badge |

---

## 8. Decisiones de Testing

### Por que NO se usa MSW en esta feature

El patron establecido en el proyecto admin es mockear el service layer con `vi.mock` (ver `use-rewards.test.ts`, `campania-backings.service.test.ts`). MSW no esta configurado en el proyecto admin. Los tests de service mockean `apiFetch` directamente, y los tests de hooks mockean el service completo. Se sigue el mismo patron.

### Por que ValidarTareaDialog y RechazarTareaDialog son Integration y no Unit

Ambos dialogs contienen logica de formulario (React Hook Form + Zod), llaman mutaciones y muestran toasts. Son suficientemente complejos para ser integration tests, siguiendo el patron de `RewardFormModal.test.tsx` y `RewardDeleteDialog.test.tsx` del proyecto.

### Por que TareaPendienteCard es Unit

Es un componente presentacional puro: recibe datos por props, renderiza la informacion y llama handlers pasados por props al interactuar. No tiene estado propio ni queries. Mismo patron que `CampaniaListCard.test.tsx`.

### Estrategia de mock de mutaciones en dialogs

Se sigue el patron de `RewardDeleteDialog.test.tsx`: la mutacion se mockea a nivel del hook (`vi.mock("@/hooks/use-validar-tarea")`). Para simular callbacks de exito/error se usa `mockImplementation` que llama `onSuccess` o `onError` sincrónicamente.

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del admin
cd src/admin && npm run test

# Ejecutar con cobertura
cd src/admin && npm run test:coverage

# Ejecutar solo tests de esta feature
cd src/admin && npm run test -- --reporter=verbose tareas-pendientes

# Watch mode durante desarrollo
cd src/admin && npm run test:watch

# Filtrar por archivo especifico
cd src/admin && npm run test -- TareaPendienteCard
```

---

## 10. Checklist

- [ ] Archivo de mocks creado: `src/admin/src/__mocks__/cp-tareas-promocion.mock.ts`
- [ ] Tests de `TareasPendientesTab` (6 casos, Integration)
- [ ] Tests de `TareaPendienteCard` (7 casos, Unit)
- [ ] Tests de `ValidarTareaDialog` (8 casos, Integration)
- [ ] Tests de `RechazarTareaDialog` (8 casos, Integration)
- [ ] Tests de `useTareasPendientes` (5 casos, Unit)
- [ ] Tests de `useValidarTarea` (4 casos, Unit)
- [ ] Tests de `useRechazarTarea` (4 casos, Unit)
- [ ] Tests de `tareas-pendientes.service` (6 casos, Unit)
- [ ] Cobertura global >= 80% verificada con `npm run test:coverage`
- [ ] Todos los tests pasan en < 60 segundos
- [ ] No hay `console.log` en los archivos de test
- [ ] No se usa `any` en los archivos de test
