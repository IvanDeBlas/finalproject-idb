# Estrategia de Testing: cp-wallet-comisiones (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/web
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 47 | ~85% |
| Integration Tests | 15 | ~90% |
| Total | 62 | 80%+ |

### Archivos de test a crear

| Archivo | Tipo | Modulo |
|---------|------|--------|
| `__tests__/components/WalletSaldoPanel.test.tsx` | Unit | Componente |
| `__tests__/components/TransaccionesHistorial.test.tsx` | Integration | Componente |
| `__tests__/components/TransaccionRow.test.tsx` | Unit | Componente |
| `__tests__/components/TransaccionFilters.test.tsx` | Integration | Componente |
| `__tests__/components/SolicitarCobroDialog.test.tsx` | Integration | Componente |
| `__tests__/components/WalletEmptyState.test.tsx` | Unit | Componente |
| `__tests__/components/EstadoBadge.test.tsx` | Unit | Componente |
| `__tests__/hooks/usePromotorWallet.test.ts` | Unit | Hook |
| `__tests__/hooks/useWalletTransacciones.test.ts` | Unit | Hook |
| `__tests__/hooks/useSolicitarCobro.test.ts` | Unit | Hook |
| `__tests__/services/wallet.service.test.ts` | Unit | Service |
| `__tests__/pages/PromotorWalletPage.test.tsx` | Integration | Pagina |
| `__tests__/utils/wallet.mappers.test.ts` | Unit | Utilidades |
| `__mocks__/wallet.mock.ts` | - | Fixtures |

---

## 2. Estructura de Tests

```
src/web/src/features/crowdpromotion/
├── __mocks__/
│   ├── tracking.mock.ts          (existente)
│   └── wallet.mock.ts            (NUEVO)
└── __tests__/
    ├── components/
    │   ├── WalletSaldoPanel.test.tsx
    │   ├── TransaccionesHistorial.test.tsx
    │   ├── TransaccionRow.test.tsx
    │   ├── TransaccionFilters.test.tsx
    │   ├── SolicitarCobroDialog.test.tsx
    │   ├── WalletEmptyState.test.tsx
    │   └── EstadoBadge.test.tsx
    ├── hooks/
    │   ├── usePromotorWallet.test.ts
    │   ├── useWalletTransacciones.test.ts
    │   └── useSolicitarCobro.test.ts
    ├── services/
    │   └── wallet.service.test.ts
    ├── pages/
    │   └── PromotorWalletPage.test.tsx
    └── utils/
        └── wallet.mappers.test.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Archivo de Mocks

**Archivo:** `src/web/src/features/crowdpromotion/__mocks__/wallet.mock.ts`

Este archivo sigue exactamente el mismo patron que `tracking.mock.ts` existente: exports nombrados con datos tipados desde `@shared/types/crowdpromotion`.

```typescript
import type {
    PromotorWallet,
    WalletTransaccionItem,
    WalletTransaccionesPagedResponse,
    SolicitarCobroResponse,
    WalletTransaccionesFilters,
} from '@shared/types/crowdpromotion'

// ========== Wallet Resumen ==========

export const mockPromotorWallet: PromotorWallet = {
    walletId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    monedaId: 1,
    monedaNombre: 'EUR',
    saldoDisponible: 150.50,
    saldoPendiente: 0.00,
    totalGanado: 200.00,
    totalRetirado: 49.50,
    minimoRetiro: 10.00,
}

export const mockPromotorWallet_SaldoCero: PromotorWallet = {
    walletId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    monedaId: 1,
    monedaNombre: 'EUR',
    saldoDisponible: 0.00,
    saldoPendiente: 0.00,
    totalGanado: 0.00,
    totalRetirado: 0.00,
    minimoRetiro: 10.00,
}

export const mockPromotorWallet_BajoMinimo: PromotorWallet = {
    walletId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    monedaId: 1,
    monedaNombre: 'EUR',
    saldoDisponible: 5.00,
    saldoPendiente: 0.00,
    totalGanado: 5.00,
    totalRetirado: 0.00,
    minimoRetiro: 10.00,
}

// ========== Transacciones Individuales ==========

export const mockTransaccion_Credito_Pendiente: WalletTransaccionItem = {
    id: 'txn-001',
    esCredito: true,
    importe: 10.00,
    descripcion: 'Comision por backing referido',
    concepto: null,
    estadoTransaccionId: 1,
    estadoTransaccionNombre: 'Pendiente',
    tipoRewardId: 1,
    tipoRewardNombre: 'Dinero',
    promoEventoId: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
    fechaCreacion: '2026-03-20T14:30:00Z',
    fechaProcesado: null,
}

export const mockTransaccion_Credito_Procesada: WalletTransaccionItem = {
    id: 'txn-002',
    esCredito: true,
    importe: 25.00,
    descripcion: 'Comision por tarea validada',
    concepto: null,
    estadoTransaccionId: 2,
    estadoTransaccionNombre: 'Procesada',
    tipoRewardId: 1,
    tipoRewardNombre: 'Dinero',
    promoEventoId: 'ab1c2d3e-4f56-7890-abcd-ef1234567890',
    fechaCreacion: '2026-03-19T10:00:00Z',
    fechaProcesado: '2026-03-21T10:00:00Z',
}

export const mockTransaccion_Debito_Pendiente: WalletTransaccionItem = {
    id: 'txn-003',
    esCredito: false,
    importe: 49.50,
    descripcion: 'Retiro mensual',
    concepto: null,
    estadoTransaccionId: 1,
    estadoTransaccionNombre: 'Pendiente',
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: '2026-03-15T09:00:00Z',
    fechaProcesado: null,
}

export const mockTransaccion_Credito_Pagada: WalletTransaccionItem = {
    id: 'txn-004',
    esCredito: true,
    importe: 15.00,
    descripcion: 'Comision por backing referido',
    concepto: null,
    estadoTransaccionId: 3,
    estadoTransaccionNombre: 'Pagada',
    tipoRewardId: 1,
    tipoRewardNombre: 'Dinero',
    promoEventoId: 'cc1d2e3f-4a5b-6789-bcde-fa1234567890',
    fechaCreacion: '2026-03-10T08:00:00Z',
    fechaProcesado: '2026-03-12T09:30:00Z',
}

export const mockTransaccion_Cancelada: WalletTransaccionItem = {
    id: 'txn-005',
    esCredito: false,
    importe: 20.00,
    descripcion: null,
    concepto: null,
    estadoTransaccionId: 4,
    estadoTransaccionNombre: 'Cancelada',
    tipoRewardId: null,
    tipoRewardNombre: null,
    promoEventoId: null,
    fechaCreacion: '2026-03-05T11:00:00Z',
    fechaProcesado: null,
}

// ========== Respuestas Paginadas ==========

export const mockWalletTransaccionesPaginadas: WalletTransaccionesPagedResponse = {
    items: [
        mockTransaccion_Credito_Pendiente,
        mockTransaccion_Credito_Procesada,
        mockTransaccion_Debito_Pendiente,
    ],
    totalCount: 15,
    page: 1,
    pageSize: 10,
    totalPages: 2,
}

export const mockWalletTransacciones_Pagina2: WalletTransaccionesPagedResponse = {
    items: [
        mockTransaccion_Credito_Pagada,
        mockTransaccion_Cancelada,
    ],
    totalCount: 15,
    page: 2,
    pageSize: 10,
    totalPages: 2,
}

export const mockWalletTransacciones_SoloCreditos: WalletTransaccionesPagedResponse = {
    items: [mockTransaccion_Credito_Pendiente, mockTransaccion_Credito_Procesada],
    totalCount: 2,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}

export const mockWalletTransacciones_Vacia: WalletTransaccionesPagedResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

// ========== Response de Solicitar Cobro ==========

export const mockSolicitarCobroResponse: SolicitarCobroResponse = {
    transaccionId: 'txn-new-001',
    importe: 50.00,
    monedaNombre: 'EUR',
    estadoTransaccionNombre: 'Pendiente',
    saldoRestante: 100.50,
    fechaCreacion: '2026-03-20T15:00:00Z',
}

// ========== Filtros de ejemplo ==========

export const mockFiltros_SoloCreditos: WalletTransaccionesFilters = {
    esCredito: true,
    page: 1,
    pageSize: 10,
}

export const mockFiltros_ConFechas: WalletTransaccionesFilters = {
    fechaDesde: '2026-03-01',
    fechaHasta: '2026-03-31',
    page: 1,
    pageSize: 10,
}
```

### 3.2 Estrategia de Mocking de Servicios

El proyecto mocka el service completo con `vi.mock()`, igual que en `tracking.service.test.ts`. **No se usa MSW** porque el proyecto ya tiene un patron consolidado de mockear el `apiFetch` de bajo nivel o el service directamente.

**Patron para tests de componentes y hooks:**
```typescript
// Mockear el service a nivel del modulo, antes de los imports
vi.mock('../../wallet/infrastructure/wallet.service', () => ({
    walletService: {
        getResumen: vi.fn(),
        getTransacciones: vi.fn(),
        solicitarCobro: vi.fn(),
    },
}))

// Importar DESPUES del vi.mock()
import { walletService } from '../../wallet/infrastructure/wallet.service'
```

**Patron para tests de service (wallet.service.test.ts):**
```typescript
// Mockear apiFetch de bajo nivel, igual que en tracking.service.test.ts
vi.mock('@/lib/api-client', () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api-client'
```

**Toast mock (para SolicitarCobroDialog y PromotorWalletPage):**
```typescript
vi.mock('sonner', () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))
```

### 3.3 Setup de QueryClient para Tests

Reutilizar el mismo patron de `createWrapper()` establecido en `usePromotorMetricas.test.ts`:

```typescript
function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children)
    }
}
```

**Para tests de pagina completa** (PromotorWalletPage), usar `renderPage()` con MemoryRouter igual que en `PromotorMetricasPage.test.tsx`:

```typescript
function renderPage() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={['/promotor/wallet']}>
                <Routes>
                    <Route path="/promotor/wallet" element={<PromotorWalletPage />} />
                    <Route
                        path="/promotor/mis-programas"
                        element={<div>Mis Programas Page</div>}
                    />
                </Routes>
            </MemoryRouter>
        </QueryClientProvider>
    )
}
```

---

## 4. Tests por Modulo

### 4.1 Mocks Compartidos - wallet.mock.ts

**Archivo:** `__mocks__/wallet.mock.ts`

Ver seccion 3.1 para el contenido completo. Exports clave:
- `mockPromotorWallet` - wallet con saldo 150.50 EUR
- `mockPromotorWallet_SaldoCero` - wallet sin saldo
- `mockPromotorWallet_BajoMinimo` - wallet con 5.00 EUR (bajo el minimo de 10.00)
- `mockTransaccion_Credito_Pendiente` - credito en Pendiente con PromoEventoId
- `mockTransaccion_Credito_Procesada` - credito en Procesada con fechaProcesado
- `mockTransaccion_Debito_Pendiente` - debito/retiro en Pendiente sin PromoEventoId
- `mockTransaccion_Credito_Pagada` - credito en Pagada
- `mockTransaccion_Cancelada` - transaccion Cancelada
- `mockWalletTransaccionesPaginadas` - pagina 1 con 3 items, totalCount 15
- `mockWalletTransacciones_Vacia` - respuesta vacia para empty state
- `mockSolicitarCobroResponse` - respuesta 201 exitosa del POST /cobro

---

### 4.2 Components

#### WalletSaldoPanel.test.tsx

**Archivo:** `__tests__/components/WalletSaldoPanel.test.tsx`
**Tipo predominante:** Unit
**Dependencias:** Solo props, sin hooks ni queries

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders saldo disponible destacado | Unit | Muestra `150,50 €` o similar con el importe del saldo |
| 2 | renders total ganado | Unit | Muestra `totalGanado` con formato de moneda |
| 3 | renders total retirado | Unit | Muestra `totalRetirado` con formato de moneda |
| 4 | renders moneda del wallet | Unit | Muestra `monedaNombre` (EUR) junto a los importes |
| 5 | button "Solicitar cobro" habilitado cuando saldo >= minimoRetiro | Unit | Con `mockPromotorWallet` (150.50 >= 10), el boton no tiene `disabled` |
| 6 | button "Solicitar cobro" deshabilitado cuando saldo < minimoRetiro | Unit | Con `mockPromotorWallet_BajoMinimo` (5.00 < 10.00), el boton tiene `disabled` |
| 7 | button "Solicitar cobro" deshabilitado cuando saldo es cero | Unit | Con `mockPromotorWallet_SaldoCero`, el boton tiene `disabled` |
| 8 | muestra aviso de saldo minimo cuando boton esta deshabilitado | Unit | Texto informativo con el valor del minimo (10.00 EUR) visible cuando saldo insuficiente |
| 9 | llama onSolicitarCobro al hacer click en boton habilitado | Unit | Mock del callback, click, verificar que se llamo una vez |
| 10 | no llama onSolicitarCobro cuando boton esta deshabilitado | Unit | Click en boton deshabilitado, verificar que callback no fue llamado |

**Casos Detallados:**

```markdown
5. button "Solicitar cobro" habilitado cuando saldo >= minimoRetiro
   - Render WalletSaldoPanel con mockPromotorWallet (saldoDisponible=150.50, minimoRetiro=10.00)
   - Assert: boton con texto "Solicitar cobro" NO tiene atributo disabled

6. button "Solicitar cobro" deshabilitado cuando saldo < minimoRetiro
   - Render WalletSaldoPanel con mockPromotorWallet_BajoMinimo (saldoDisponible=5.00, minimoRetiro=10.00)
   - Assert: boton con texto "Solicitar cobro" tiene atributo disabled o aria-disabled
   - Assert: texto con "10" o "10,00" visible (descripcion del minimo)

9. llama onSolicitarCobro al hacer click en boton habilitado
   - const onSolicitarCobro = vi.fn()
   - Render con mockPromotorWallet y prop onSolicitarCobro
   - userEvent.click en boton "Solicitar cobro"
   - expect(onSolicitarCobro).toHaveBeenCalledTimes(1)
```

---

#### TransaccionRow.test.tsx

**Archivo:** `__tests__/components/TransaccionRow.test.tsx`
**Tipo predominante:** Unit
**Dependencias:** Solo props (transaccion: WalletTransaccionItem)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders importe con signo positivo para credito | Unit | Credito muestra "+10,00" o "+10.00" con algun formato positivo |
| 2 | renders importe con signo negativo para debito | Unit | Debito muestra "-49,50" o "-49.50" con signo negativo |
| 3 | aplica clase de color verde para creditos | Unit | Elemento del importe tiene clase CSS de color verde (text-green-*) |
| 4 | aplica clase de color rojo para debitos | Unit | Elemento del importe tiene clase CSS de color rojo (text-red-*) |
| 5 | renders descripcion cuando existe | Unit | `mockTransaccion_Credito_Pendiente.descripcion` visible en pantalla |
| 6 | renders EstadoBadge con nombre de estado correcto | Unit | Badge con texto "Pendiente" para estadoTransaccionId=1 |
| 7 | renders fecha de creacion formateada | Unit | Fecha de `fechaCreacion` visible en pantalla |
| 8 | renders tipoRewardNombre cuando existe | Unit | "Dinero" visible para credito con tipoRewardId |
| 9 | no muestra tipoRewardNombre para debitos | Unit | Con `mockTransaccion_Debito_Pendiente`, "Dinero" no aparece |
| 10 | renders correctamente cuando descripcion es null | Unit | Sin texto de descripcion visible, no hay crash |

**Casos Detallados:**

```markdown
1. renders importe con signo positivo para credito
   - Render TransaccionRow con mockTransaccion_Credito_Pendiente (esCredito=true, importe=10.00)
   - Assert: elemento con texto que contenga "+" y "10"

2. renders importe con signo negativo para debito
   - Render TransaccionRow con mockTransaccion_Debito_Pendiente (esCredito=false, importe=49.50)
   - Assert: elemento con texto que contenga "-" y "49"

6. renders EstadoBadge con nombre de estado correcto
   - Render TransaccionRow con mockTransaccion_Credito_Pendiente (estadoTransaccionNombre="Pendiente")
   - Assert: screen.getByText("Pendiente") en documento
```

---

#### EstadoBadge.test.tsx

**Archivo:** `__tests__/components/EstadoBadge.test.tsx`
**Tipo predominante:** Unit
**Dependencias:** Solo props (estadoTransaccionId: number, estadoTransaccionNombre: string)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders "Pendiente" para estadoId 1 | Unit | Texto "Pendiente" visible |
| 2 | renders "Procesada" para estadoId 2 | Unit | Texto "Procesada" visible |
| 3 | renders "Pagada" para estadoId 3 | Unit | Texto "Pagada" visible |
| 4 | renders "Cancelada" para estadoId 4 | Unit | Texto "Cancelada" visible |
| 5 | aplica variant "secondary" para Pendiente (estadoId=1) | Unit | Badge component recibe variant "secondary" |
| 6 | aplica variant "default" para Procesada (estadoId=2) | Unit | Badge component recibe variant "default" |
| 7 | aplica variant "success" para Pagada (estadoId=3) | Unit | Badge component recibe variant "success" |
| 8 | aplica variant "destructive" para Cancelada (estadoId=4) | Unit | Badge component recibe variant "destructive" |
| 9 | usa estadoTransaccionNombre recibido como texto del badge | Unit | El componente muestra el nombre que recibe por prop, no hardcodeado |

**Nota de implementacion:** Si `EstadoBadge` recibe ambas props (`estadoTransaccionId` para derivar el variant y `estadoTransaccionNombre` para el texto), los tests de variante pueden verificar la clase CSS del elemento o el atributo `data-variant` del componente Badge de shadcn/ui.

---

#### WalletEmptyState.test.tsx

**Archivo:** `__tests__/components/WalletEmptyState.test.tsx`
**Tipo predominante:** Unit
**Dependencias:** Solo props opcionales

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders mensaje de empty state | Unit | Texto explicativo sobre wallet vacia visible |
| 2 | renders boton/link "Explorar programas" | Unit | Elemento de navegacion a programas disponibles |
| 3 | link "Explorar programas" apunta a la ruta correcta | Unit | href del link contiene "/promotor/mis-programas" o equivalente |
| 4 | renders icono de wallet (o contenedor del icono) | Unit | Elemento visual indicador de empty state presente |

---

#### TransaccionFilters.test.tsx

**Archivo:** `__tests__/components/TransaccionFilters.test.tsx`
**Tipo predominante:** Integration (interaccion con formulario)
**Dependencias:** Callbacks, sin queries

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders selector de tipo (credito/debito/todos) | Unit | El elemento de filtro por tipo esta en pantalla |
| 2 | renders selector de estado | Unit | El elemento de filtro por estado esta en pantalla |
| 3 | renders inputs de fecha desde y fecha hasta | Unit | Ambos campos de fecha presentes |
| 4 | llama onFilterChange con esCredito=true al seleccionar "Creditos" | Integration | userEvent en selector, verificar callback con { esCredito: true } |
| 5 | llama onFilterChange con esCredito=false al seleccionar "Debitos" | Integration | userEvent en selector, verificar callback con { esCredito: false } |
| 6 | llama onFilterChange con esCredito=undefined al seleccionar "Todos" | Integration | userEvent en selector, verificar que esCredito no esta en el payload |
| 7 | llama onFilterChange con estadoTransaccionId correcto al seleccionar estado | Integration | Seleccionar "Pendiente", verificar { estadoTransaccionId: 1 } |
| 8 | llama onFilterChange con fechas cuando se ingresan fechas validas | Integration | userEvent en fechaDesde y fechaHasta, verificar callback |
| 9 | no llama onFilterChange cuando fechaDesde > fechaHasta (validacion) | Integration | Ingresar fechas invalidas, callback no debe ser llamado con fechas invalidas |
| 10 | boton de limpiar filtros resetea el estado | Integration | Click en "Limpiar filtros", verificar que onFilterChange recibe filtros vacios |

---

#### TransaccionesHistorial.test.tsx

**Archivo:** `__tests__/components/TransaccionesHistorial.test.tsx`
**Tipo predominante:** Integration
**Dependencias:** `useWalletTransacciones` hook (mocked)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | muestra skeleton durante la carga | Unit | Estado isLoading=true muestra indicadores de carga |
| 2 | renderiza lista de transacciones tras carga exitosa | Integration | Con datos de mock, items visibles en pantalla |
| 3 | renderiza tantas filas como items en la respuesta | Integration | 3 filas para `mockWalletTransaccionesPaginadas.items` |
| 4 | muestra WalletEmptyState cuando items es vacio | Integration | Con `mockWalletTransacciones_Vacia`, empty state visible |
| 5 | muestra mensaje de error cuando la query falla | Integration | Error state visible con boton Reintentar |
| 6 | muestra informacion de paginacion (pagina X de Y) | Integration | Texto de pagina actual y total de paginas visible |
| 7 | boton "Siguiente" llama a cambio de pagina | Integration | userEvent click en siguiente, verificar cambio de pagina |
| 8 | boton "Anterior" deshabilitado en pagina 1 | Integration | Boton "Anterior" tiene disabled en pagina 1 |
| 9 | boton "Siguiente" deshabilitado en ultima pagina | Integration | Con totalPages=1 y page=1, boton "Siguiente" tiene disabled |
| 10 | aplica filtros y relanza la query | Integration | Cambio de filtros en TransaccionFilters dispara nueva query |

---

#### SolicitarCobroDialog.test.tsx

**Archivo:** `__tests__/components/SolicitarCobroDialog.test.tsx`
**Tipo predominante:** Integration (formulario + mutation)
**Dependencias:** `useSolicitarCobro` hook (mocked), props `wallet: PromotorWallet`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders el dialogo cuando isOpen=true | Unit | Dialogo visible con titulo y campos |
| 2 | no renders contenido del dialogo cuando isOpen=false | Unit | Contenido del dialogo no en pantalla cuando cerrado |
| 3 | muestra saldo disponible actual en el dialogo | Unit | `150,50` o similar visible dentro del dialogo |
| 4 | muestra el minimo de retiro requerido | Unit | "10,00" o "10.00" como referencia del minimo |
| 5 | campo importe requerido - muestra error si se envia vacio | Integration | Submit sin importe muestra "El importe es obligatorio" |
| 6 | muestra error si importe es 0 o negativo | Integration | Ingresar "0" y submit, mensaje "mayor que cero" visible |
| 7 | muestra error si importe excede saldo disponible (soft check) | Integration | Ingresar importe mayor al saldo, mensaje de error visible antes de submit |
| 8 | campo descripcion acepta texto opcional (max 500) | Integration | Ingresar texto en descripcion, sin error |
| 9 | submit valido llama a la mutation con importe y descripcion | Integration | userEvent en campos, submit, verificar llamada a mutate con datos correctos |
| 10 | muestra toast de exito tras solicitud exitosa | Integration | Mutation exitosa, `toast.success` llamado con mensaje de confirmacion |
| 11 | muestra error 4040 (saldo insuficiente) del backend en el dialogo | Integration | Mutation rechaza con errorCode 4040, mensaje de error visible en el formulario |
| 12 | muestra error 4041 (bajo minimo) del backend en el dialogo | Integration | Mutation rechaza con errorCode 4041, mensaje especifico visible |
| 13 | muestra error 4042 (cobro concurrente) del backend como toast | Integration | Mutation rechaza con errorCode 4042, toast.error llamado |
| 14 | cierra el dialogo tras exito y llama onSuccess | Integration | Mutation exitosa, dialogo se cierra y prop onSuccess llamado |
| 15 | boton Cancelar cierra el dialogo sin llamar a mutation | Integration | userEvent click en Cancelar, mutation no fue llamada |

**Setup especifico:**

```markdown
El componente recibe: isOpen, onClose, onSuccess, wallet: PromotorWallet

Necesita wrapper con QueryClientProvider para la mutation hook.
El mock de useSolicitarCobro se hace a nivel del modulo del hook:

vi.mock('../../wallet/application/hooks/useSolicitarCobro', () => ({
    useSolicitarCobro: () => ({
        mutate: mockMutate,
        isPending: false,
        isError: false,
        error: null,
    }),
}))

Alternativa: mockear walletService.solicitarCobro directamente
y dejar que useSolicitarCobro sea real (mas integration).
```

**Casos Detallados:**

```markdown
9. submit valido llama a la mutation con importe y descripcion
   - Render dialog con isOpen=true y mockPromotorWallet
   - userEvent.type en campo importe con "50"
   - userEvent.type en campo descripcion con "Retiro mensual"
   - userEvent.click en boton "Confirmar"
   - expect(mutate o walletService.solicitarCobro).toHaveBeenCalledWith({
       importe: 50,
       descripcion: "Retiro mensual"
     })

10. muestra toast de exito tras solicitud exitosa
   - Mock mutation que resuelve con mockSolicitarCobroResponse
   - Submit formulario valido
   - expect(toast.success).toHaveBeenCalledWith(
       expect.stringContaining("registrada")
     )
```

---

### 4.3 Hooks

#### usePromotorWallet.test.ts

**Archivo:** `__tests__/hooks/usePromotorWallet.test.ts`
**Tipo predominante:** Unit
**Mock:** `walletService.getResumen` via `vi.mock`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna data correctamente en exito | Unit | `result.current.data` igual a `mockPromotorWallet` tras `waitFor isSuccess` |
| 2 | isLoading es true inicialmente | Unit | `result.current.isLoading` es `true` antes de resolver |
| 3 | isError es true cuando el servicio falla | Unit | `mockRejectedValue`, `waitFor isError` |
| 4 | llama al servicio sin parametros adicionales | Unit | `walletService.getResumen` llamado sin argumentos |
| 5 | retorna saldo cero correctamente | Unit | Con `mockPromotorWallet_SaldoCero`, `data.saldoDisponible` es 0 |
| 6 | retorna error 404/2030 cuando wallet no existe | Unit | `mockRejectedValue` con errorCode "2030", `isError` true |
| 7 | retorna error 403/2015 cuando no tiene perfil de promotor | Unit | `mockRejectedValue` con errorCode "2015", `isError` true |

**Setup:**

```typescript
vi.mock('../../wallet/infrastructure/wallet.service', () => ({
    walletService: {
        getResumen: vi.fn(),
        getTransacciones: vi.fn(),
        solicitarCobro: vi.fn(),
    },
}))

// createWrapper() igual al patron existente en usePromotorMetricas.test.ts
```

---

#### useWalletTransacciones.test.ts

**Archivo:** `__tests__/hooks/useWalletTransacciones.test.ts`
**Tipo predominante:** Unit
**Mock:** `walletService.getTransacciones` via `vi.mock`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna datos correctamente en exito | Unit | `result.current.data` igual a `mockWalletTransaccionesPaginadas` |
| 2 | isLoading es true inicialmente | Unit | Estado loading antes de resolver |
| 3 | isError es true cuando el servicio falla | Unit | `mockRejectedValue`, isError true |
| 4 | llama al servicio con filtros por defecto (page=1, pageSize=10) | Unit | Verificar que la llamada incluye pagina y pageSize por defecto |
| 5 | llama al servicio con filtros esCredito=true | Unit | Pasar `{ esCredito: true }` como filtros, verificar llamada al servicio |
| 6 | llama al servicio con estadoTransaccionId correcto | Unit | Pasar filtro de estado, verificar que llega al servicio |
| 7 | llama al servicio con rango de fechas cuando se pasan filtros | Unit | `{ fechaDesde: '2026-03-01', fechaHasta: '2026-03-31' }` en la llamada |
| 8 | retorna lista vacia correctamente | Unit | Con `mockWalletTransacciones_Vacia`, `data.items` es array vacio |
| 9 | retorna totalCount y totalPages correctamente | Unit | Verificar metadatos de paginacion en `data` |
| 10 | re-fetch cuando cambian los filtros | Unit | Cambiar filtros, verificar que el servicio es llamado nuevamente |

---

#### useSolicitarCobro.test.ts

**Archivo:** `__tests__/hooks/useSolicitarCobro.test.ts`
**Tipo predominante:** Unit
**Mock:** `walletService.solicitarCobro` via `vi.mock`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna mutate function | Unit | `result.current.mutate` es una funcion |
| 2 | isPending es false inicialmente | Unit | `result.current.isPending` es false antes de llamar |
| 3 | llama al servicio con importe y descripcion correctos | Unit | `mutate({ importe: 50, descripcion: "test" })`, verificar llamada |
| 4 | llama al servicio con solo importe (descripcion opcional) | Unit | `mutate({ importe: 50 })`, servicio llamado sin crash |
| 5 | onSuccess invalida la query del wallet tras mutation exitosa | Unit | Verificar que `queryClient.invalidateQueries` es llamado con la key del wallet |
| 6 | onSuccess invalida la query de transacciones tras mutation exitosa | Unit | Verificar invalidacion de la key de transacciones |
| 7 | isError es true cuando el servicio retorna error | Unit | `mockRejectedValue`, `waitFor isError` |
| 8 | el error del hook tiene el errorCode del backend | Unit | Error con `errorCode: "4040"` propagado correctamente |

**Caso detallado para invalidacion de queries:**

```markdown
5. onSuccess invalida la query del wallet tras mutation exitosa
   - Crear queryClient de test con spy en invalidateQueries
   - Mock walletService.solicitarCobro que resuelve con mockSolicitarCobroResponse
   - renderHook con wrapper que incluye QueryClientProvider
   - Llamar result.current.mutate({ importe: 50 })
   - waitFor(() => expect(queryClient.invalidateQueries).toHaveBeenCalledWith(
       expect.objectContaining({ queryKey: expect.arrayContaining(['crowdpromotion', 'wallet', 'resumen']) })
     ))
```

---

### 4.4 Services

#### wallet.service.test.ts

**Archivo:** `__tests__/services/wallet.service.test.ts`
**Tipo predominante:** Unit
**Mock:** `apiFetch` de `@/lib/api-client` (igual que en `tracking.service.test.ts`)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| **getResumen** | | |
| 1 | retorna PromotorWallet en exito | Unit | `apiFetch` resuelve con `{ data: mockPromotorWallet }`, resultado igual |
| 2 | llama al endpoint correcto GET /promotor/wallet | Unit | `apiFetch` llamado con URL que contiene "/promotor/wallet" |
| 3 | lanza error con errorCode 2030 cuando wallet no existe (404) | Unit | Mock retorna `{ data: null, messages: [{ errorCode: "2030" }] }`, catch errorCode |
| 4 | lanza error con errorCode 2015 cuando no tiene perfil promotor | Unit | Mock retorna errorCode "2015", catch |
| 5 | lanza error con errorCode 3001 cuando no autenticado | Unit | Mock retorna errorCode "3001", catch |
| 6 | lanza en error de red | Unit | `mockRejectedValue(new Error("Network error"))`, rejects |
| **getTransacciones** | | |
| 7 | retorna WalletTransaccionesPagedResponse en exito | Unit | `apiFetch` resuelve con datos paginados, resultado correcto |
| 8 | llama al endpoint correcto con parametros de paginacion | Unit | URL contiene "page=1" y "pageSize=10" |
| 9 | incluye esCredito=true en la URL cuando se pasa el filtro | Unit | URL contiene "esCredito=true" |
| 10 | incluye estadoTransaccionId en la URL cuando se pasa | Unit | URL contiene "estadoTransaccionId=1" |
| 11 | incluye fechaDesde y fechaHasta en la URL cuando se pasan | Unit | URL contiene las fechas codificadas correctamente |
| 12 | no incluye filtros opcionales cuando son undefined | Unit | Sin filtros, URL no contiene "esCredito" ni "estadoTransaccionId" |
| 13 | lanza error con errorCode 2030 cuando wallet no existe | Unit | Mock retorna errorCode "2030", catch |
| 14 | lanza en error de red | Unit | `mockRejectedValue`, rejects |
| **solicitarCobro** | | |
| 15 | retorna SolicitarCobroResponse con status 201 en exito | Unit | `apiFetch` resuelve con `mockSolicitarCobroResponse`, resultado correcto |
| 16 | llama al endpoint POST /promotor/wallet/cobro | Unit | `apiFetch` llamado con URL correcta y method POST |
| 17 | envia importe y descripcion en el body | Unit | `apiFetch` llamado con `data: { importe: 50, descripcion: "Retiro" }` |
| 18 | envia solo importe cuando descripcion no se proporciona | Unit | `apiFetch` llamado con `data: { importe: 50 }` sin descripcion |
| 19 | lanza error con errorCode 4040 (saldo insuficiente) | Unit | Mock retorna errorCode "4040", catch |
| 20 | lanza error con errorCode 4041 (saldo bajo minimo) | Unit | Mock retorna errorCode "4041", catch |
| 21 | lanza error con errorCode 4042 (cobro concurrente) | Unit | Mock retorna errorCode "4042", catch |
| 22 | lanza en error de red | Unit | `mockRejectedValue`, rejects |

---

### 4.5 Utilidades Shared

#### wallet.mappers.test.ts

**Archivo:** `__tests__/utils/wallet.mappers.test.ts`
**Tipo predominante:** Unit (funciones puras)
**Importa de:** `@shared/utils/mappers` y `@shared/utils/format`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| **mapEstadoWalletTransaccionToBadge** | | |
| 1 | retorna "secondary" para estadoId 1 (Pendiente) | Unit | `mapEstadoWalletTransaccionToBadge(1)` === "secondary" |
| 2 | retorna "default" para estadoId 2 (Procesada) | Unit | `mapEstadoWalletTransaccionToBadge(2)` === "default" |
| 3 | retorna "success" para estadoId 3 (Pagada) | Unit | `mapEstadoWalletTransaccionToBadge(3)` === "success" |
| 4 | retorna "destructive" para estadoId 4 (Cancelada) | Unit | `mapEstadoWalletTransaccionToBadge(4)` === "destructive" |
| 5 | retorna "secondary" como fallback para estadoId desconocido | Unit | `mapEstadoWalletTransaccionToBadge(99)` === "secondary" (fallback) |
| **mapTransaccionTipoToDisplayProps** | | |
| 6 | retorna tipo "credit" para esCredito=true | Unit | `.tipo` === "credit" |
| 7 | retorna signo "+" para esCredito=true | Unit | `.signo` === "+" |
| 8 | retorna clase CSS verde para esCredito=true | Unit | `.colorClass` contiene "green" |
| 9 | retorna tipo "debit" para esCredito=false | Unit | `.tipo` === "debit" |
| 10 | retorna signo "-" para esCredito=false | Unit | `.signo` === "-" |
| 11 | retorna clase CSS roja para esCredito=false | Unit | `.colorClass` contiene "red" |
| **formatWalletImporte** | | |
| 12 | formatea 150.50 con EUR como "150,50 €" o similar | Unit | Resultado contiene "150" y "50" y "€" |
| 13 | formatea 0 con EUR correctamente mostrando dos decimales | Unit | Resultado contiene "0,00" o "0.00" |
| 14 | formatea con moneda EUR por defecto cuando no se pasa moneda | Unit | Llamar sin segundo argumento, resultado formateado en EUR |
| **formatTransaccionImporte** | | |
| 15 | formatea credito de 10.00 EUR como "+10,00 €" o similar | Unit | Resultado empieza con "+" o contiene "+" seguido del importe |
| 16 | formatea debito de 49.50 EUR como "-49,50 €" o similar | Unit | Resultado empieza con "-" o contiene "-" seguido del importe |

---

### 4.6 Pagina Completa (Integration)

#### PromotorWalletPage.test.tsx

**Archivo:** `__tests__/pages/PromotorWalletPage.test.tsx`
**Tipo predominante:** Integration (flujo completo de usuario)
**Mocks:** `walletService` completo + `toast` de sonner
**Setup:** `renderPage()` con MemoryRouter y QueryClientProvider

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renderiza el titulo de la pagina | Unit | "Mi Wallet" o titulo equivalente visible en pantalla |
| 2 | muestra skeletons/loading durante la carga inicial | Unit | Indicadores de carga presentes mientras queries pendientes |
| 3 | muestra el panel de saldo tras carga exitosa | Integration | `150,50` o saldo del mock visible en el panel |
| 4 | muestra el historial de transacciones tras carga exitosa | Integration | Filas de transacciones visibles con descripciones |
| 5 | muestra WalletEmptyState cuando no hay transacciones | Integration | Con `mockWalletTransacciones_Vacia`, empty state visible |
| 6 | muestra boton "Solicitar cobro" habilitado con saldo suficiente | Integration | Boton no disabled con `mockPromotorWallet` |
| 7 | click en "Solicitar cobro" abre el dialogo SolicitarCobroDialog | Integration | userEvent click, dialogo visible en pantalla |
| 8 | flujo completo: abrir dialogo, ingresar importe, confirmar, toast exito | Integration | Flujo end-to-end del cobro con todos los steps |
| 9 | muestra error de red en la pagina con boton Reintentar | Integration | Servicio falla, mensaje de error y boton Reintentar visibles |
| 10 | click en Reintentar hace refetch del wallet | Integration | Mock falla primero, luego exito, verificar datos visibles tras retry |
| 11 | WalletEmptyState es visible cuando wallet no existe (404/2030) | Integration | Servicio retorna 404, empty state o mensaje apropiado visible |
| 12 | aplicar filtro de tipo en historial relanza la query con filtros | Integration | Seleccionar "Solo creditos", verificar que getTransacciones recibe esCredito=true |
| 13 | cambiar pagina en el historial llama al servicio con page correcto | Integration | Click en "Siguiente", verificar llamada con page=2 |
| 14 | boton "Solicitar cobro" deshabilitado cuando saldo bajo minimo | Integration | Con `mockPromotorWallet_BajoMinimo`, boton disabled sin click posible |
| 15 | el dialogo de cobro se cierra y actualiza el saldo tras exito | Integration | Tras mutation exitosa, saldo nuevo visible en panel (query invalidada) |

**Caso detallado para flujo completo (test 8):**

```markdown
8. flujo completo: abrir dialogo, ingresar importe, confirmar, toast exito
   - Mock walletService.getResumen que resuelve con mockPromotorWallet
   - Mock walletService.getTransacciones que resuelve con mockWalletTransaccionesPaginadas
   - Mock walletService.solicitarCobro que resuelve con mockSolicitarCobroResponse
   - renderPage()
   - waitFor(() => saldo visible en pantalla)
   - userEvent.click en boton "Solicitar cobro"
   - waitFor(() => dialogo visible)
   - userEvent.type en campo importe con "50"
   - userEvent.click en boton "Confirmar" (o "Solicitar")
   - waitFor(() => {
       expect(walletService.solicitarCobro).toHaveBeenCalledWith({ importe: 50 })
       expect(toast.success).toHaveBeenCalled()
     })
```

---

## 5. Cobertura Esperada por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| `WalletSaldoPanel.tsx` | 90% | 100% | 85% |
| `TransaccionesHistorial.tsx` | 85% | 90% | 80% |
| `TransaccionRow.tsx` | 95% | 100% | 90% |
| `TransaccionFilters.tsx` | 85% | 90% | 80% |
| `SolicitarCobroDialog.tsx` | 85% | 90% | 85% |
| `WalletEmptyState.tsx` | 100% | 100% | 100% |
| `EstadoBadge.tsx` | 100% | 100% | 100% |
| `PromotorWalletPage.tsx` | 85% | 90% | 80% |
| `usePromotorWallet.ts` | 90% | 100% | 85% |
| `useWalletTransacciones.ts` | 90% | 100% | 85% |
| `useSolicitarCobro.ts` | 90% | 100% | 85% |
| `wallet.service.ts` | 95% | 100% | 90% |
| `mappers.ts` (funciones wallet) | 100% | 100% | 100% |
| `format.ts` (funciones wallet) | 100% | 100% | 100% |

**Meta Global:** 80% en todas las metricas

---

## 6. Patrones de Test del Proyecto (Referencia)

Los siguientes patrones estan consolidados en el proyecto y DEBEN aplicarse en esta feature:

### Orden de declaraciones en archivos de test

```typescript
// 1. Imports de test framework
import { describe, it, expect, vi, beforeEach } from "vitest"

// 2. Imports de testing library
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { renderHook } from "@testing-library/react"

// 3. vi.mock() de dependencias ANTES de importar el modulo bajo test
vi.mock('ruta/al/servicio', () => ({ ... }))
vi.mock('sonner', () => ({ toast: { success: vi.fn(), error: vi.fn() } }))

// 4. Imports del codigo bajo test (despues de vi.mock)
import { walletService } from '...'
import { ComponenteATestear } from '...'

// 5. Setup de fixtures y helpers de render
const mockDatos = { ... }
function renderPage() { ... }
function createWrapper() { ... }
```

### Convenciones de nombrado de tests

Seguir el patron del proyecto: mezcla de espanol e ingles segun contexto.
- Componentes: espanol ("renderiza el saldo", "muestra error")
- Hooks: ingles ("returns data correctly on success", "isLoading is true initially")
- Services: ingles ("returns WalletResponse on success", "calls correct endpoint")
- Utilidades: ingles o espanol ("retorna secondary para estadoId 1")

### beforeEach standard

```typescript
beforeEach(() => {
    vi.clearAllMocks()
})
```

---

## 7. Casos Edge y Escenarios de Error Criticos

Los siguientes casos tienen alta prioridad y cubren los flujos alternativos de la feature-spec:

| Escenario | Componente/Hook | Error Code | Caso de Test |
|-----------|-----------------|------------|--------------|
| FA-01: Wallet sin transacciones | TransaccionesHistorial, PromotorWalletPage | - | Empty state visible |
| FA-02: Saldo bajo minimo de retiro | WalletSaldoPanel, PromotorWalletPage | - | Boton disabled con aviso |
| FA-03: Importe > saldo en cobro | SolicitarCobroDialog | 4040 | Error en campo importe |
| FA-04: Cobro concurrente | SolicitarCobroDialog | 4042 | Toast de error con mensaje especifico |
| FA-05: Promotor desactivado | - | 2015 | Error en carga de wallet (404) |
| Wallet no creada aun | PromotorWalletPage | 2030 | Empty state o mensaje informativo |
| Sin perfil de promotor | PromotorWalletPage | 2015 | Error visible al cargar |
| Error de red | PromotorWalletPage | - | Estado de error con Reintentar |
| Paginacion en ultima pagina | TransaccionesHistorial | - | Boton Siguiente disabled |
| Descripcion con 501 chars | SolicitarCobroDialog | Zod | Mensaje de validacion visible |

---

## 8. Comandos de Ejecucion

```bash
# Todos los tests del proyecto landing
cd src/web && npm run test

# Con coverage
cd src/web && npm run test:coverage

# Solo los tests de la feature wallet
cd src/web && npm run test -- --reporter=verbose wallet

# Watch mode durante desarrollo
cd src/web && npm run test:watch

# Un archivo especifico
cd src/web && npm run test -- wallet.service.test.ts
```

---

## 9. CI/CD Integration

```yaml
- name: Run Landing Tests (cp-wallet-comisiones)
  working-directory: src/web
  run: npm run test:coverage

- name: Check Coverage Threshold
  run: |
    # Cobertura minima 80% en lineas, funciones y branches
    npm run test:coverage -- --coverage.thresholds.lines=80
```

---

## 10. Dependencias de Implementacion

Los tests de esta feature asumen que los siguientes artefactos del plan de shared y del plan de frontend ya existen:

| Dependencia | Origen | Necesaria para |
|-------------|--------|----------------|
| `PromotorWallet`, `WalletTransaccionItem`, etc. | `src/shared/types/crowdpromotion.ts` | wallet.mock.ts |
| `solicitarCobroSchema` | `src/shared/schemas/crowdpromotion.schema.ts` | SolicitarCobroDialog.test.tsx |
| `ESTADO_WALLET_TRANSACCION`, `ESTADO_WALLET_TRANSACCION_BADGES` | `src/shared/constants/index.ts` | wallet.mappers.test.ts |
| `mapEstadoWalletTransaccionToBadge`, `mapTransaccionTipoToDisplayProps` | `src/shared/utils/mappers.ts` | wallet.mappers.test.ts |
| `formatWalletImporte`, `formatTransaccionImporte` | `src/shared/utils/format.ts` | wallet.mappers.test.ts |
| `WALLET_ERROR_MESSAGES` | `src/shared/utils/error-messages.ts` | SolicitarCobroDialog.test.tsx |
| `walletService` | `src/web/src/features/crowdpromotion/wallet/infrastructure/wallet.service.ts` | Todos los tests de hooks |
| `usePromotorWallet` | `src/web/src/features/crowdpromotion/wallet/application/hooks/usePromotorWallet.ts` | PromotorWalletPage.test.tsx |
| `useSolicitarCobro` | `src/web/src/features/crowdpromotion/wallet/application/hooks/useSolicitarCobro.ts` | SolicitarCobroDialog.test.tsx |
| `useWalletTransacciones` | `src/web/src/features/crowdpromotion/wallet/application/hooks/useWalletTransacciones.ts` | TransaccionesHistorial.test.tsx |

---

## 11. Checklist

- [ ] `__mocks__/wallet.mock.ts` creado con todas las fixtures documentadas
- [ ] `wallet.service.test.ts` cubre los 3 metodos (getResumen, getTransacciones, solicitarCobro)
- [ ] `usePromotorWallet.test.ts` cubre estados loading/success/error y casos 404
- [ ] `useWalletTransacciones.test.ts` cubre filtros opcionales y paginacion
- [ ] `useSolicitarCobro.test.ts` verifica invalidacion de queries tras exito
- [ ] `WalletSaldoPanel.test.tsx` cubre logica de boton habilitado/deshabilitado segun saldo
- [ ] `EstadoBadge.test.tsx` cubre los 4 estados con sus variantes de color
- [ ] `TransaccionRow.test.tsx` cubre credito (verde/+) y debito (rojo/-)
- [ ] `SolicitarCobroDialog.test.tsx` cubre validacion client-side y errores 4040/4041/4042
- [ ] `PromotorWalletPage.test.tsx` cubre flujo completo de cobro (test 8)
- [ ] `wallet.mappers.test.ts` cubre las 4 funciones puras de shared
- [ ] Todos los tests usan `vi.clearAllMocks()` en `beforeEach`
- [ ] Los tests de mutation verifican invalidacion de queries
- [ ] Los tests de pagina usan MemoryRouter con las rutas necesarias
- [ ] Cobertura 80%+ verificada en CI antes de merge
