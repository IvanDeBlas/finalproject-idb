# Estrategia de Testing: cp-wallet-comisiones (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/admin
**Cobertura Objetivo:** 80%

---

> **FUERA DE SCOPE MVP — Plan preparatorio**
>
> Segun la feature-spec de US-CP-06, el proyecto Admin no tiene responsabilidad en esta iteracion:
> _"Sin responsabilidad en MVP. Futuro: vista de administrador para ver wallets de promotores y marcar transacciones como Procesada/Pagada/Cancelada."_
>
> Este documento define la estrategia de testing que se aplicara cuando Admin implemente la gestion de wallets. Sirve como referencia para el equipo en la iteracion futura, y documenta los patrones, mocks y casos de test que deberan implementarse.

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 22 | 85% |
| Integration Tests | 18 | 80% |
| Total | 40 | 80%+ |

### Alcance futuro de Admin

Cuando Admin implemente la gestion de wallets, las responsabilidades seran:

1. **Listado de wallets de promotores**: vista tabular con saldo disponible, total ganado, total retirado y moneda por promotor
2. **Detalle de transacciones por promotor**: historial paginado con filtros por tipo (credito/debito), estado y rango de fechas
3. **Cambio de estado de transacciones**: marcar una transaccion como Procesada, Pagada o Cancelada con confirmacion
4. **Panel de resumen de wallets**: KPIs globales de wallets del sistema (total pendiente de pagar, total pagado, etc.)

---

## 2. Estructura de Tests

```
src/admin/src/
├── __mocks__/
│   └── cp-wallet-comisiones.mock.ts            NUEVO
├── components/
│   └── crowdpromotion/
│       └── wallets/
│           ├── WalletResumenCard.tsx            (componente futuro)
│           ├── TransaccionesTable.tsx           (componente futuro)
│           ├── TransaccionEstadoBadge.tsx       (componente futuro)
│           ├── ActualizarEstadoDialog.tsx       (componente futuro)
│           ├── WalletFiltros.tsx                (componente futuro)
│           └── __tests__/
│               ├── WalletResumenCard.test.tsx
│               ├── TransaccionesTable.test.tsx
│               ├── TransaccionEstadoBadge.test.tsx
│               ├── ActualizarEstadoDialog.test.tsx
│               └── WalletFiltros.test.tsx
├── hooks/
│   ├── use-wallet-admin.ts                      (hook futuro)
│   ├── use-transacciones-admin.ts               (hook futuro)
│   ├── use-actualizar-estado-transaccion.ts     (hook futuro)
│   └── __tests__/
│       ├── use-wallet-admin.test.ts
│       ├── use-transacciones-admin.test.ts
│       └── use-actualizar-estado-transaccion.test.ts
└── services/
    ├── wallet-admin.service.ts                  (servicio futuro)
    └── __tests__/
        └── wallet-admin.service.test.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/cp-wallet-comisiones.mock.ts`

Este archivo sigue el mismo patron que `cp-tracking-metricas.mock.ts` existente: constantes de IDs, datos por escenario (con datos, vacio, con error), y una funcion builder para generar variantes.

```typescript
import type {
    PromotorWallet,
    WalletTransaccionItem,
    WalletTransaccionesPagedResponse,
} from "@shared/types/crowdpromotion"

// ─── Identificadores ──────────────────────────────────────────────────────────

export const WALLET_ID = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
export const PROMOTOR_ID = "a1b2c3d4-1234-5678-abcd-ef0123456789"
export const TRANSACCION_ID_1 = "c1d2e3f4-5678-9012-bcde-f01234567890"
export const TRANSACCION_ID_2 = "d2e3f4a5-6789-0123-cdef-012345678901"

// ─── Wallet resumen ──────────────────────────────────────────────────────────

export const mockWalletConSaldo: PromotorWallet = {
    walletId: WALLET_ID,
    monedaId: 1,
    monedaNombre: "EUR",
    saldoDisponible: 150.50,
    saldoPendiente: 0.00,
    totalGanado: 200.00,
    totalRetirado: 49.50,
    minimoRetiro: 10.00,
}

export const mockWalletVacio: PromotorWallet = {
    walletId: WALLET_ID,
    monedaId: 1,
    monedaNombre: "EUR",
    saldoDisponible: 0.00,
    saldoPendiente: 0.00,
    totalGanado: 0.00,
    totalRetirado: 0.00,
    minimoRetiro: 10.00,
}

// ─── Transacciones ────────────────────────────────────────────────────────────

export const mockTransaccionCredito: WalletTransaccionItem = {
    id: TRANSACCION_ID_1,
    esCredito: true,
    importe: 10.00,
    descripcion: "Comision por backing referido",
    concepto: null,
    estadoTransaccionId: 1,
    estadoTransaccionNombre: "Pendiente",
    tipoRewardId: 1,
    tipoRewardNombre: "Dinero",
    promoEventoId: "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    fechaCreacion: "2026-03-20T14:30:00Z",
    fechaProcesado: null,
}

export const mockTransaccionDebito: WalletTransaccionItem = {
    id: TRANSACCION_ID_2,
    esCredito: false,
    importe: 49.50,
    descripcion: "Retiro mensual",
    concepto: null,
    estadoTransaccionId: 1,
    estadoTransaccionNombre: "Pendiente",
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: "2026-03-21T10:00:00Z",
    fechaProcesado: null,
}

export const mockTransaccionPagada: WalletTransaccionItem = {
    ...mockTransaccionDebito,
    id: "e3f4a5b6-7890-1234-def0-123456789012",
    estadoTransaccionId: 3,
    estadoTransaccionNombre: "Pagada",
    fechaProcesado: "2026-03-22T09:00:00Z",
}

export const mockTransaccionesList: WalletTransaccionItem[] = [
    mockTransaccionCredito,
    mockTransaccionDebito,
]

// ─── Respuesta paginada ───────────────────────────────────────────────────────

export const mockTransaccionesPagedResponse: WalletTransaccionesPagedResponse = {
    items: mockTransaccionesList,
    totalCount: 15,
    page: 1,
    pageSize: 10,
    totalPages: 2,
}

export const mockTransaccionesPagedVacia: WalletTransaccionesPagedResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

// ─── Builder helpers ──────────────────────────────────────────────────────────

export function buildTransaccion(
    overrides: Partial<WalletTransaccionItem> = {}
): WalletTransaccionItem {
    return { ...mockTransaccionCredito, ...overrides }
}

export function buildWallet(
    overrides: Partial<PromotorWallet> = {}
): PromotorWallet {
    return { ...mockWalletConSaldo, ...overrides }
}
```

### 3.2 Mock del Servicio

El patron de Admin mocka el service directamente con `vi.mock`, igual que en los tests de `cp-tracking-metricas`. No se usa MSW en Admin para estos tests unitarios e de integracion de componente.

```typescript
vi.mock("@/services/wallet-admin.service", () => ({
    walletAdminService: {
        getWalletByPromotor: vi.fn(),
        getTransacciones: vi.fn(),
        actualizarEstadoTransaccion: vi.fn(),
    },
}))
```

### 3.3 Mock de next/navigation

Todos los componentes Admin que usen filtros via URL deben mockear `next/navigation` siguiendo el patron establecido en `ProgramaMetricasTab.test.tsx`:

```typescript
const mockReplace = vi.fn()
const mockGet = vi.fn(() => null)

vi.mock("next/navigation", () => ({
    useSearchParams: vi.fn(() => ({
        get: mockGet,
        toString: vi.fn(() => ""),
    })),
    useRouter: vi.fn(() => ({
        replace: mockReplace,
        push: vi.fn(),
    })),
}))
```

### 3.4 Test Utilities

Usar el wrapper de `@/test-utils` ya existente en Admin (el mismo que usan `ProgramaMetricasTab.test.tsx` y `KpiCard.test.tsx`). Este wrapper incluye `QueryClientProvider` con `retry: false`.

Para hooks, usar el patron `createWrapper()` establecido en `use-programa-metricas.test.ts`:

```typescript
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

### 4.1 Components

#### 4.1.1 WalletResumenCard.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/__tests__/WalletResumenCard.test.tsx`

Componente que muestra el resumen del wallet de un promotor: saldo disponible, saldo pendiente, total ganado, total retirado y moneda. Es el equivalente admin de la tarjeta de saldo de la Landing.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders saldoDisponible | Unit | Muestra el saldo disponible con formato monetario (ej: "150,50 EUR") |
| renders totalGanado | Unit | Muestra el total ganado historico |
| renders totalRetirado | Unit | Muestra el total retirado historico |
| renders monedaNombre | Unit | Muestra el codigo de moneda junto al saldo |
| renders saldoPendiente when non-zero | Unit | Muestra el campo de saldo pendiente si es mayor a 0 |
| hides saldoPendiente when zero | Unit | No muestra saldo pendiente si es 0.00 |
| renders zero saldo correctly | Unit | Saldo 0.00 se muestra correctamente sin crashes |

**Casos Detallados:**

```markdown
1. renders saldoDisponible
   - Render con mockWalletConSaldo
   - Assert: texto "150,50" visible en documento
   - Assert: texto "EUR" visible en documento

2. renders zero saldo correctly
   - Render con mockWalletVacio
   - Assert: texto "0,00" o "0" visible en documento
   - Assert: no crashes, componente en DOM
```

#### 4.1.2 TransaccionEstadoBadge.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/__tests__/TransaccionEstadoBadge.test.tsx`

Componente Badge visual que representa el estado de una transaccion (Pendiente, Procesada, Pagada, Cancelada) con colores diferenciados. Equivale a `KpiCard` en simplicidad: es un componente de presentacion puro sin efectos.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders "Pendiente" for estadoId 1 | Unit | Muestra texto "Pendiente" para estado 1 |
| renders "Procesada" for estadoId 2 | Unit | Muestra texto "Procesada" para estado 2 |
| renders "Pagada" for estadoId 3 | Unit | Muestra texto "Pagada" para estado 3 |
| renders "Cancelada" for estadoId 4 | Unit | Muestra texto "Cancelada" para estado 4 |
| applies correct variant for Pendiente | Unit | Aplica variant "secondary" (gris) para estado 1 |
| applies correct variant for Pagada | Unit | Aplica variant "success" (verde) para estado 3 |
| applies correct variant for Cancelada | Unit | Aplica variant "destructive" (rojo) para estado 4 |
| handles unknown estadoId gracefully | Unit | No lanza error para IDs desconocidos, muestra fallback |

#### 4.1.3 TransaccionesTable.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/__tests__/TransaccionesTable.test.tsx`

Tabla paginada de transacciones. Recibe la lista de items, totalCount y callbacks de paginacion como props. Muestra importe con signo (+/-), tipo (credito/debito), estado con badge y fecha.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders transaccion rows | Unit | Muestra una fila por cada transaccion |
| renders importe positivo with + prefix for credito | Unit | Credito muestra "+10,00 EUR" (verde) |
| renders importe with - prefix for debito | Unit | Debito muestra "-49,50 EUR" (rojo) |
| renders estado badge per row | Unit | Cada fila tiene su badge de estado |
| renders descripcion text | Unit | Muestra el campo descripcion de cada transaccion |
| renders fechaCreacion formatted | Unit | Fecha formateada en formato legible |
| renders empty state when items is empty | Unit | Muestra mensaje de "Sin transacciones" cuando lista vacia |
| calls onPageChange when paginating | Integration | Click en boton de siguiente pagina llama callback |
| renders totalCount in pagination | Unit | Muestra el total de registros para la paginacion |
| handles null descripcion | Unit | No crashea cuando descripcion es null |
| handles null fechaProcesado | Unit | Columna fecha procesado muestra "-" cuando es null |

**Casos Detallados:**

```markdown
1. renders importe positivo with + prefix for credito
   - Render con [mockTransaccionCredito]
   - Assert: texto "+10,00 EUR" o "+10,00" visible y con clase texto verde

2. renders empty state when items is empty
   - Render con items=[] totalCount=0
   - Assert: texto de empty state visible (ej: "Sin transacciones en este periodo")
   - Assert: tabla o lista no muestra filas

3. calls onPageChange when paginating
   - Render con totalCount=15, pageSize=10, currentPage=1
   - Click en boton de pagina siguiente
   - Assert: onPageChange llamado con 2
```

#### 4.1.4 ActualizarEstadoDialog.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/__tests__/ActualizarEstadoDialog.test.tsx`

Dialogo de confirmacion para cambiar el estado de una transaccion. Muestra el estado actual, un selector con los estados posibles (Procesada, Pagada, Cancelada) y botones Confirmar/Cancelar. Es el componente de mayor complejidad de interaccion en el modulo Admin de wallets.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with current estado name | Unit | Muestra el nombre del estado actual de la transaccion |
| renders estado selector options | Unit | El selector tiene opciones: Procesada, Pagada, Cancelada |
| Pendiente is not an option in selector | Unit | El estado "Pendiente" no aparece como opcion (no se puede volver atras) |
| clicking Cancelar closes dialog | Integration | Click en "Cancelar" cierra el dialogo sin llamar al servicio |
| clicking Confirmar calls onConfirm with new estado | Integration | Seleccionar "Pagada" y click en "Confirmar" llama callback con estadoId=3 |
| shows loading state during mutation | Integration | Boton "Confirmar" muestra estado loading mientras muta |
| shows success toast after confirmar | Integration | Toast de exito visible tras confirmacion exitosa |
| shows error message on service failure | Integration | Mensaje de error visible si la mutacion falla |
| Confirmar is disabled when no estado selected | Unit | Boton "Confirmar" deshabilitado si no se ha elegido un nuevo estado |

**Casos Detallados:**

```markdown
1. clicking Confirmar calls onConfirm with new estado
   - Render dialog con transaccion en estado Pendiente (id=1)
   - Seleccionar "Pagada" del selector (estadoId=3)
   - Click en boton "Confirmar"
   - Assert: onConfirm llamado con { transaccionId, nuevoEstadoId: 3 }

2. shows loading state during mutation
   - Mock del mutation que devuelve Promise que no resuelve
   - Click en Confirmar
   - Assert: boton "Confirmar" tiene atributo disabled o spinner visible

3. shows error message on service failure
   - Mock del mutation que rechaza con error "4042"
   - Click en Confirmar
   - Assert: mensaje de error visible (ej: "Ya existe una solicitud de cobro pendiente")
```

#### 4.1.5 WalletFiltros.test.tsx

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/__tests__/WalletFiltros.test.tsx`

Panel de filtros para el historial de transacciones admin: tipo (Credito/Debito/Todos), estado y rango de fechas. Sigue el patron de `FiltroFechas.test.tsx` existente.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders tipo filter options | Unit | Muestra opciones: Todos, Credito, Debito |
| renders estado filter options | Unit | Muestra opciones de estado (Pendiente, Procesada, Pagada, Cancelada) |
| renders fecha inputs | Unit | Muestra inputs fechaDesde y fechaHasta |
| clicking Aplicar calls onFilter with selected values | Integration | Seleccionar filtros y click en Aplicar llama onFilter con los valores |
| clicking Limpiar resets all filters | Integration | Click en Limpiar limpia todos los filtros y llama onFilter con objeto vacio |
| fechaHasta before fechaDesde shows validation error | Integration | Si fechaHasta < fechaDesde, muestra error de validacion |

---

### 4.2 Hooks

#### 4.2.1 use-wallet-admin.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-wallet-admin.test.ts`

Hook que encapsula `useQuery` para obtener el resumen del wallet de un promotor especifico. Sigue el patron de `useProgramaMetricas`.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns wallet data on success | Unit | Retorna `PromotorWallet` cuando el servicio resuelve correctamente |
| handles isLoading state | Unit | `isLoading` es `true` mientras la query esta en vuelo |
| handles isError state | Unit | `isError` es `true` cuando el servicio rechaza |
| does not fetch when promotorId is empty | Unit | `fetchStatus` es `"idle"` cuando promotorId es string vacio |
| passes promotorId to service | Unit | Llama a `walletAdminService.getWalletByPromotor` con el promotorId correcto |
| does not retry on error | Unit | El servicio se llama exactamente 1 vez al fallar (retry: false) |
| exposes refetch function | Unit | `refetch` es una funcion en el resultado del hook |

**Setup:**
```typescript
vi.mock("@/services/wallet-admin.service", () => ({
    walletAdminService: {
        getWalletByPromotor: vi.fn(),
    },
}))

const mockedGetWallet = vi.mocked(walletAdminService.getWalletByPromotor)
```

#### 4.2.2 use-transacciones-admin.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-transacciones-admin.test.ts`

Hook que encapsula `useQuery` para obtener el historial de transacciones paginado de un promotor con filtros. Los filtros incluyen `esCredito`, `estadoTransaccionId`, `fechaDesde`, `fechaHasta`, `page` y `pageSize`.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns paged data on success without filters | Unit | Retorna `WalletTransaccionesPagedResponse` sin filtros activos |
| returns data on success with all filters | Unit | Retorna datos cuando todos los filtros estan definidos |
| passes all filter params to service | Unit | Llama al servicio con los filtros exactos recibidos |
| handles isLoading state | Unit | `isLoading` es true mientras la query esta en vuelo |
| handles isError state | Unit | `isError` es true cuando el servicio rechaza |
| does not fetch when promotorId is empty | Unit | `fetchStatus` es `"idle"` cuando promotorId es vacio |
| re-fetches when filters change | Integration | Cambiar el filtro `esCredito` dispara nueva llamada al servicio |
| uses correct query key with filters | Unit | La query key incluye los filtros para invalidacion granular |
| does not retry on error | Unit | El servicio se llama 1 vez al fallar (retry: false) |

**Casos Detallados:**

```markdown
1. re-fetches when filters change
   - Render hook con filtro inicial {}
   - Esperar isSuccess
   - Rerender con filtro { esCredito: true }
   - Assert: servicio llamado 2 veces (una sin filtro, una con esCredito)

2. uses correct query key with filters
   - Render hook con filtros { esCredito: true, estadoTransaccionId: 1, page: 2 }
   - Assert: servicio recibio exactamente esos parametros
```

#### 4.2.3 use-actualizar-estado-transaccion.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-actualizar-estado-transaccion.test.ts`

Hook de mutacion que encapsula `useMutation` para cambiar el estado de una transaccion. Invalida las queries de transacciones tras el exito. Es el hook de mayor complejidad en este modulo.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| calls service with transaccionId and nuevoEstadoId | Integration | La mutacion llama al servicio con los parametros correctos |
| isLoading is true while mutating | Unit | `isPending` es true mientras la mutacion esta en vuelo |
| isSuccess after successful mutation | Integration | `isSuccess` es true tras mutacion exitosa |
| isError after failed mutation | Integration | `isError` es true si el servicio rechaza |
| invalidates transacciones query on success | Integration | Tras exito, `queryClient.invalidateQueries` es llamado con la key de transacciones |
| shows success toast on success | Integration | Toast de exito visible tras mutacion exitosa |
| shows error toast on failure | Integration | Toast de error visible si la mutacion falla |

**Casos Detallados:**

```markdown
1. invalidates transacciones query on success
   - Mock servicio para resolver exitosamente
   - Ejecutar mutacion con { transaccionId, nuevoEstadoId: 3 }
   - Esperar isSuccess
   - Assert: queryClient.invalidateQueries llamado con key que incluye 'wallet' y 'transacciones'

2. shows success toast on success
   - Mock servicio para resolver exitosamente
   - Ejecutar mutacion
   - Assert: toast con mensaje de exito visible en DOM

3. shows error toast on failure
   - Mock servicio para rechazar con error "4042"
   - Ejecutar mutacion
   - Assert: toast de error con mensaje legible visible en DOM
```

---

### 4.3 Service

#### 4.3.1 wallet-admin.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/wallet-admin.service.test.ts`

Servicio HTTP del modulo Admin para acceder a los endpoints de wallets. Sigue el patron de `metricas.service.test.ts`: mockea `apiFetch` y verifica URLs, metodo HTTP, params y manejo de errores.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getWalletByPromotor calls correct URL | Unit | Llama a `API_ROUTES.crowdpromotion.promotorWallet.resumen` con el promotorId correcto |
| getWalletByPromotor returns PromotorWallet on success | Unit | Retorna el objeto `PromotorWallet` del campo `data` de la respuesta |
| getWalletByPromotor throws on 404 (wallet not found) | Unit | Lanza error con mensaje cuando errorCode es "2030" |
| getWalletByPromotor throws on 404 (promotor not found) | Unit | Lanza error con mensaje cuando errorCode es "2015" |
| getWalletByPromotor throws on 401 unauthorized | Unit | Lanza error cuando errorCode es "3001" |
| getWalletByPromotor throws on 500 internal error | Unit | Lanza error cuando errorCode es "5000" |
| getWalletByPromotor propagates network error | Unit | Lanza error de red si apiFetch rechaza |
| getTransacciones calls correct URL without filters | Unit | Llama a la URL de transacciones sin query params cuando no hay filtros |
| getTransacciones appends esCredito when true | Unit | Query param `esCredito=true` incluido cuando filtro activo |
| getTransacciones appends esCredito when false | Unit | Query param `esCredito=false` incluido (distinguir de no enviado) |
| getTransacciones appends estadoTransaccionId | Unit | Query param `estadoTransaccionId=1` incluido cuando filtro activo |
| getTransacciones appends fechaDesde | Unit | Query param `fechaDesde=2026-03-01` incluido |
| getTransacciones appends fechaHasta | Unit | Query param `fechaHasta=2026-03-31` incluido |
| getTransacciones appends page and pageSize | Unit | Query params `page=2&pageSize=10` incluidos |
| getTransacciones returns paged response on success | Unit | Retorna `WalletTransaccionesPagedResponse` correctamente |
| getTransacciones throws on 400 invalid date range | Unit | Lanza error cuando errorCode es "1036" |
| getTransacciones throws on 400 invalid pagination | Unit | Lanza error cuando errorCode es "1021" |
| actualizarEstadoTransaccion calls PATCH/PUT with correct body | Unit | Llama al endpoint de actualizacion con `{ transaccionId, nuevoEstadoId }` |
| actualizarEstadoTransaccion returns updated transaccion on success | Unit | Retorna el item de transaccion actualizado |
| actualizarEstadoTransaccion throws on 409 cobro concurrente | Unit | Lanza error cuando errorCode es "4042" |

**Casos Detallados:**

```markdown
1. getTransacciones appends esCredito when false
   - Llamar con filtro { esCredito: false }
   - Assert: URL contiene "esCredito=false"
   - Nota critica: esCredito=false es un filtro valido (mostrar solo debitos)
     y debe diferenciarse de "no enviar el param" (mostrar todos los tipos)

2. actualizarEstadoTransaccion calls PATCH/PUT with correct body
   - Llamar con { transaccionId: TRANSACCION_ID_1, nuevoEstadoId: 3 }
   - Assert: apiFetch llamado con metodo PATCH o PUT (segun implementation)
   - Assert: body contiene { nuevoEstadoId: 3 }
   - Assert: URL contiene el transaccionId
```

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| WalletResumenCard.tsx | 85% | 100% | 80% |
| TransaccionEstadoBadge.tsx | 90% | 100% | 90% |
| TransaccionesTable.tsx | 80% | 90% | 80% |
| ActualizarEstadoDialog.tsx | 80% | 85% | 80% |
| WalletFiltros.tsx | 80% | 90% | 80% |
| use-wallet-admin.ts | 90% | 100% | 85% |
| use-transacciones-admin.ts | 90% | 100% | 85% |
| use-actualizar-estado-transaccion.ts | 85% | 100% | 80% |
| wallet-admin.service.ts | 95% | 100% | 90% |

**Meta Global:** 80% en todas las metricas

---

## 6. Patrones de Test Establecidos en Admin

La implementacion futura debe seguir los patrones ya establecidos en `src/admin`:

### Pattern 1: Mock del servicio con vi.mock (no MSW)

Admin no usa MSW; todos los tests mockean el servicio directamente via `vi.mock`. Ver `use-programa-metricas.test.ts` y `metricas.service.test.ts`.

### Pattern 2: createWrapper para hooks con React Query

```typescript
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

### Pattern 3: render de componentes usa @/test-utils

```typescript
import { render, screen, waitFor } from "@/test-utils"
```

El `@/test-utils` del proyecto Admin ya configura `QueryClientProvider` y otros providers necesarios. No crear wrappers custom en los tests de componentes.

### Pattern 4: Mock de next/navigation para componentes con filtros en URL

```typescript
vi.mock("next/navigation", () => ({
    useSearchParams: vi.fn(() => ({
        get: mockGet,
        toString: vi.fn(() => ""),
    })),
    useRouter: vi.fn(() => ({
        replace: mockReplace,
        push: vi.fn(),
    })),
}))
```

### Pattern 5: beforeEach con vi.clearAllMocks

Cada `describe` que use mocks de servicio debe incluir `beforeEach(() => { vi.clearAllMocks() })` para evitar contaminacion entre tests.

### Pattern 6: Builder helpers en mock file

El archivo de mocks expone funciones `build*` para generar variantes de los datos base:

```typescript
export function buildTransaccion(overrides: Partial<WalletTransaccionItem> = {}): WalletTransaccionItem {
    return { ...mockTransaccionCredito, ...overrides }
}
```

---

## 7. Casos Edge y de Error

Los siguientes casos son criticos para cubrir las reglas de negocio de US-CP-06 desde la perspectiva Admin:

| Escenario | Componente/Hook | Caso a testear |
|-----------|-----------------|----------------|
| Transaccion con `promoEventoId` null (debito) | TransaccionesTable | Columna "Origen" muestra "-" o "Retiro manual" |
| Transaccion con `fechaProcesado` null (pendiente) | TransaccionesTable | Columna fecha procesado muestra "-" |
| Wallet con saldo 0 | WalletResumenCard | Valores 0.00 se muestran sin errores |
| Estado "Cancelada" en la tabla | TransaccionEstadoBadge | Badge variant "destructive" |
| Cambio de estado a "Procesada" | ActualizarEstadoDialog | Selector incluye "Procesada" como opcion |
| Error 409 (cobro concurrente) al actualizar | use-actualizar-estado-transaccion | Toast de error con mensaje especifico "4042" |
| Lista de transacciones vacia | TransaccionesTable | Empty state visible con mensaje descriptivo |
| Paginacion en ultima pagina | TransaccionesTable | Boton "Siguiente" deshabilitado |
| Filtro esCredito=false (mostrar solo debitos) | use-transacciones-admin | Service recibe `esCredito=false` explicitamente |

---

## 8. Comandos de Ejecucion

```bash
# Desde src/admin

# Ejecutar todos los tests
npm run test

# Ejecutar con cobertura
npm run test:coverage

# Ejecutar solo los tests de wallets
npm run test -- --reporter=verbose wallet

# Watch mode durante desarrollo
npm run test:watch
```

---

## 9. Checklist de Implementacion (para la iteracion futura)

- [ ] Archivo `__mocks__/cp-wallet-comisiones.mock.ts` creado con todos los datos mock
- [ ] Tests de `WalletResumenCard` (7 casos)
- [ ] Tests de `TransaccionEstadoBadge` (8 casos)
- [ ] Tests de `TransaccionesTable` (11 casos)
- [ ] Tests de `ActualizarEstadoDialog` (9 casos — mayor complejidad de interaccion)
- [ ] Tests de `WalletFiltros` (6 casos)
- [ ] Tests de `use-wallet-admin` (7 casos)
- [ ] Tests de `use-transacciones-admin` (9 casos)
- [ ] Tests de `use-actualizar-estado-transaccion` (7 casos)
- [ ] Tests de `wallet-admin.service` (19 casos — mayor cobertura de errores HTTP)
- [ ] Todos los casos edge documentados en seccion 7 cubiertos
- [ ] vi.clearAllMocks() en beforeEach de cada describe con mocks
- [ ] Cobertura 80%+ verificada con `npm run test:coverage`
- [ ] Tests pasan en CI sin flakiness
