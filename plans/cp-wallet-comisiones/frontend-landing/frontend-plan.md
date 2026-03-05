# Plan Frontend: cp-wallet-comisiones (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/web (Vite + React 18 + TypeScript)

---

## 1. Resumen

- Screens: 1 (`/promotor/wallet` con dialog embebido)
- Componentes: 8 (6 de presentacion + 1 dialog + 1 pagina)
- Hooks: 3 (2 query + 1 mutation)
- Services: 1 (`wallet.service.ts`)
- Archivos nuevos: 16 (sin contar tests ni mocks)
- Archivos modificados: 1 (`router.tsx`)

---

## 2. Estructura de Carpetas

```
src/web/src/features/crowdpromotion/
└── wallet/
    ├── domain/
    │   └── index.ts
    │       Re-export de types desde @shared/types/crowdpromotion
    │
    ├── application/
    │   └── hooks/
    │       ├── usePromotorWallet.ts
    │       ├── useWalletTransacciones.ts
    │       └── useSolicitarCobro.ts
    │
    ├── infrastructure/
    │   └── wallet.service.ts
    │
    └── presentation/
        ├── components/
        │   ├── WalletSaldoCard.tsx
        │   ├── FiltrosHistorial.tsx
        │   ├── TransaccionItem.tsx
        │   ├── TransaccionesList.tsx
        │   ├── PaginacionWallet.tsx
        │   ├── WalletEmptyState.tsx
        │   └── SolicitarCobroDialog.tsx
        └── pages/
            └── PromotorWalletPage.tsx
```

**Tests y mocks** (siguiendo el patron de la feature metricas):

```
src/web/src/features/crowdpromotion/
├── __mocks__/
│   └── wallet.mock.ts
└── __tests__/
    ├── services/
    │   └── wallet.service.test.ts
    ├── hooks/
    │   ├── usePromotorWallet.test.ts
    │   ├── useWalletTransacciones.test.ts
    │   └── useSolicitarCobro.test.ts
    └── components/
        ├── WalletSaldoCard.test.tsx
        ├── TransaccionItem.test.tsx
        ├── FiltrosHistorial.test.tsx
        ├── SolicitarCobroDialog.test.tsx
        └── PromotorWalletPage.test.tsx
```

---

## 3. Componentes

### 3.1 PromotorWalletPage

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/pages/PromotorWalletPage.tsx`

**Tipo:** Page component (default export para lazy loading)

**Props:** ninguna (es una page, recibe datos de hooks internos)

**Estado Local:**
- `isCobroDialogOpen: boolean` - controla visibilidad del `SolicitarCobroDialog`
- `filters: WalletTransaccionesFilters` - estado de los filtros activos, inicializado con `{ page: 1, pageSize: 10 }`

**Dependencias:**
- Hooks: `usePromotorWallet`, `useWalletTransacciones`
- Componentes internos: `WalletSaldoCard`, `FiltrosHistorial`, `TransaccionesList`, `PaginacionWallet`, `WalletEmptyState`, `SolicitarCobroDialog`
- Componentes UI: `Button`, `Skeleton` (shadcn/ui), `AlertCircle` (lucide-react)
- Router: `Link` de react-router-dom

**Responsabilidad:**
Orquesta la pagina completa de wallet. Monta ambas queries en paralelo. Gestiona el estado de los filtros y los propaga a `useWalletTransacciones`. Controla la apertura y cierre del `SolicitarCobroDialog`. Implementa los cuatro estados principales de la pantalla: loading, datos, empty state y error. Es el unico componente que conoce el estado global de la pagina.

**Logica de estados:**

| Condicion | Vista que se muestra |
|-----------|---------------------|
| `isWalletLoading` | `WalletSaldoCard` en skeleton + `TransaccionesListSkeleton` (5 filas) |
| `isWalletError` | Card de error centrada con boton "Reintentar" |
| `wallet` cargado + `transacciones.totalCount === 0` (sin filtros activos) | `WalletSaldoCard` con saldo 0 + ocultar `FiltrosHistorial` + `WalletEmptyState` |
| `wallet` cargado + `transacciones.items.length === 0` (con filtros activos) | `WalletSaldoCard` normal + `FiltrosHistorial` + empty state de filtros dentro de `TransaccionesList` |
| Default | `WalletSaldoCard` + `FiltrosHistorial` + `TransaccionesList` + `PaginacionWallet` |

**Handler de cambio de filtros:**
Cuando cambia cualquier filtro (tipo, estado o fecha), el manejador actualiza `filters` forzando `page: 1` para volver a la primera pagina. Al cambiar solo la paginacion, `page` se actualiza sin resetear los demas filtros.

---

### 3.2 WalletSaldoCard

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/WalletSaldoCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| wallet | `PromotorWallet \| undefined` | No | Datos del wallet; si es undefined, muestra skeleton |
| isLoading | `boolean` | Si | Mostrar skeleton de carga |
| onSolicitarCobro | `() => void` | Si | Callback al hacer click en "Solicitar cobro" |

**Estado Local:** ninguno

**Dependencias:**
- Componentes UI: `Card`, `Button`, `Skeleton` (shadcn/ui)
- Iconos: `Info` (lucide-react)
- Utils: `formatWalletImporte` de `@shared/utils/format`
- Constantes: `MIN_RETIRO_WALLET` de `@shared/constants`

**Responsabilidad:**
Muestra el panel de saldo principal. Cuando `isLoading=true` o `wallet=undefined` renderiza el skeleton. Con datos, muestra: saldo disponible en `text-4xl font-bold text-[#f59e0b]`, total ganado en verde `text-[#10b981]`, total retirado en gris `text-[#94a3b8]`, moneda del wallet, y el boton "Solicitar cobro". El boton se habilita cuando `wallet.saldoDisponible >= wallet.minimoRetiro`. Muestra aviso inline en amarillo con icono `Info` cuando el saldo esta por debajo del minimo.

**Condicion del boton:**
```
canSolicitar = wallet !== undefined && wallet.saldoDisponible >= wallet.minimoRetiro
```

---

### 3.3 FiltrosHistorial

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/FiltrosHistorial.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| filters | `WalletTransaccionesFilters` | Si | Estado actual de los filtros |
| onChange | `(filters: WalletTransaccionesFilters) => void` | Si | Callback al cambiar cualquier filtro |
| isLoading | `boolean` | No | Deshabilitar controles mientras carga |

**Estado Local:**
- `fechaDesde: string` - valor local del input de fecha desde (controlado con debounce)
- `fechaHasta: string` - valor local del input de fecha hasta (controlado con debounce)
- `fechaError: string | null` - error de validacion cuando fechaDesde > fechaHasta

**Dependencias:**
- Componentes UI: `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`, `Input`, `Button` (shadcn/ui)
- Iconos: `X` (lucide-react)
- Constantes: `ESTADO_WALLET_TRANSACCION`, `ESTADO_WALLET_TRANSACCION_LABELS` de `@shared/constants`
- Hook externo: `useDebounce` (si existe en el proyecto) o `setTimeout` interno

**Responsabilidad:**
Renderiza los controles de filtrado: Select de tipo (Todos/Creditos/Debitos), Select de estado (Todos los estados/Pendiente/Procesada/Pagada/Cancelada), dos inputs de fecha con validacion de rango, y boton "Limpiar filtros" (visible solo cuando hay filtros activos). Los selects disparan `onChange` inmediatamente. Los inputs de fecha aplican debounce de 500ms antes de llamar `onChange`. Al detectar `fechaDesde > fechaHasta`, muestra borde rojo en el input y no llama `onChange`. El boton "Limpiar filtros" aparece cuando `hasActiveFilters = filters.esCredito !== undefined || filters.estadoTransaccionId !== undefined || !!filters.fechaDesde || !!filters.fechaHasta`.

---

### 3.4 TransaccionItem

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/TransaccionItem.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| transaccion | `WalletTransaccionItem` | Si | Datos de la transaccion a renderizar |
| monedaNombre | `string` | No | Moneda del wallet (default "EUR") |

**Estado Local:** ninguno

**Dependencias:**
- Componentes UI: `Badge` (shadcn/ui)
- Iconos: `ArrowUp`, `ArrowDown` (lucide-react)
- Utils: `formatTransaccionImporte`, `formatWalletImporte` de `@shared/utils/format`
- Utils: `mapEstadoWalletTransaccionToBadge` de `@shared/utils/mappers`
- Constantes: `ESTADO_WALLET_TRANSACCION_LABELS` de `@shared/constants`

**Responsabilidad:**
Renderiza una fila de transaccion. Icono verde con `ArrowUp` para creditos; icono rojo con `ArrowDown` para debitos. Muestra concepto o descripcion como texto principal, tipo de reward (si aplica) como texto secundario y fecha formateada como "20 mar 2026". A la derecha: importe con signo y color (verde para credito, rojo para debito) y badge de estado con los colores definidos en el design system. Implementa `hover:bg-[#1e1e38] transition-colors duration-150`.

**Formato de fecha:**
```
new Date(transaccion.fechaCreacion).toLocaleDateString('es-ES', {
    day: 'numeric', month: 'short', year: 'numeric'
})
```

**Badges de estado:**

| estadoTransaccionId | Clases Tailwind |
|--------------------|----------------|
| 1 (Pendiente) | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs font-medium` |
| 2 (Procesada) | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs font-medium` |
| 3 (Pagada) | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs font-medium` |
| 4 (Cancelada) | `bg-slate-900/50 text-[#64748b] border border-slate-700/50 text-xs font-medium` |

---

### 3.5 TransaccionesList

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/TransaccionesList.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| transacciones | `WalletTransaccionItem[]` | Si | Lista de transacciones a mostrar |
| isLoading | `boolean` | No | Mostrar skeleton (default false) |
| hasActiveFilters | `boolean` | No | Determina variante del empty state |
| monedaNombre | `string` | No | Moneda para pasar a TransaccionItem (default "EUR") |
| onClearFilters | `() => void` | No | Callback boton "Limpiar filtros" del empty state de filtros |

**Estado Local:** ninguno

**Dependencias:**
- Componentes internos: `TransaccionItem`
- Componentes UI: `Card`, `Skeleton`, `Button` (shadcn/ui)
- Iconos: `SearchX` (lucide-react)

**Responsabilidad:**
Renderiza la lista de transacciones dentro de una `Card`. Cuando `isLoading=true`, muestra el skeleton de 5 filas (`TransaccionesListSkeleton` como componente interno). Cuando `transacciones.length === 0` y `hasActiveFilters=true`, muestra el empty state de filtros con icono `SearchX` y boton "Limpiar filtros". Cuando `transacciones.length === 0` y `hasActiveFilters=false`, el padre `PromotorWalletPage` decide mostrar `WalletEmptyState` en lugar de este componente. El componente `TransaccionesListSkeleton` es un sub-componente interno (no exportado), con 5 filas de skeletons que imitan la estructura de `TransaccionItem`.

---

### 3.6 PaginacionWallet

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/PaginacionWallet.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| page | `number` | Si | Pagina actual (base 1) |
| totalPages | `number` | Si | Total de paginas |
| totalCount | `number` | Si | Total de registros para texto informativo |
| pageSize | `number` | Si | Registros por pagina para calcular rango |
| onPageChange | `(page: number) => void` | Si | Callback al cambiar de pagina |

**Estado Local:** ninguno

**Dependencias:**
- Componentes UI: `Pagination`, `PaginationContent`, `PaginationEllipsis`, `PaginationItem`, `PaginationLink`, `PaginationNext`, `PaginationPrevious` (shadcn/ui)

**Responsabilidad:**
Renderiza controles de paginacion con texto informativo "Mostrando X-Y de Z transacciones". Muestra maximo 5 numeros de pagina; agrega `PaginationEllipsis` si `totalPages > 5`. Aplica `opacity-40 pointer-events-none` a los botones anterior/siguiente en los extremos. La pagina activa usa `bg-[#a855f7] text-white`. Solo se renderiza si `totalPages > 1`.

---

### 3.7 WalletEmptyState

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/WalletEmptyState.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| (ninguna) | - | - | Componente sin props externas |

**Estado Local:** ninguno

**Dependencias:**
- Componentes UI: `Button` (shadcn/ui)
- Iconos: `Wallet` (lucide-react)
- Router: `Link` de react-router-dom

**Responsabilidad:**
Muestra el empty state cuando el promotor no tiene ninguna transaccion registrada. Icono de wallet en contenedor circular `bg-[#151525] border border-[#334155]`, titulo "Aun no tienes transacciones", subtitulo explicativo y boton CTA "Explorar programas" que navega a `/crowdpromotion/explorar`. Sigue exactamente el mismo patron visual de los empty states existentes en `PromotorMetricasPage`.

---

### 3.8 SolicitarCobroDialog

**Archivo:** `src/web/src/features/crowdpromotion/wallet/presentation/components/SolicitarCobroDialog.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| open | `boolean` | Si | Controla la visibilidad del dialog (controlado desde el padre) |
| onOpenChange | `(open: boolean) => void` | Si | Callback para abrir/cerrar (permite cerrar con Escape y click fuera) |
| saldoDisponible | `number` | Si | Saldo actual del wallet para mostrar y validar |
| minimoRetiro | `number` | Si | Minimo de retiro para mostrar en el dialog |
| monedaNombre | `string` | No | Moneda del wallet (default "EUR") |
| onSuccess | `(saldoRestante: number) => void` | Si | Callback cuando el cobro se registra con exito; recibe el saldo restante para actualizacion optimista |

**Estado Local:**
- `apiError: string | null` - error de API para mostrar inline dentro del dialog (no como toast para errores de negocio como 4040, 4041, 4042)

**Dependencias:**
- Hook: `useSolicitarCobro`
- Formulario: `useForm` + `zodResolver` de react-hook-form/zod
- Schema: `solicitarCobroSchema` de `@shared/schemas/crowdpromotion.schema`
- Types: `SolicitarCobroFormData` de `@shared/schemas/crowdpromotion.schema`
- Componentes UI: `Dialog`, `DialogContent`, `DialogFooter`, `DialogHeader`, `DialogTitle`, `Button`, `Input`, `Label`, `Separator`, `Textarea`, `Alert`, `AlertDescription` (shadcn/ui)
- Iconos: `AlertCircle`, `Loader2` (lucide-react)
- Utils: `formatWalletImporte` de `@shared/utils/format`
- Utils: `getWalletErrorMessage` de `@shared/utils/error-messages`

**Responsabilidad:**
Dialog controlado de solicitud de cobro. Contiene el formulario con React Hook Form y validacion Zod. Aplica un refinement adicional en tiempo de ejecucion para validar `importe <= saldoDisponible` usando `setError` de React Hook Form (no en el schema global porque depende del prop `saldoDisponible`). Impide cerrar el dialog mientras `isSubmitting === true` pasando `onInteractOutside={(e) => { if (isSubmitting) e.preventDefault() }}` al `DialogContent`. Al submit exitoso llama `onSuccess(response.saldoRestante)` y cierra el dialog. En error de API con codigo 4042 (concurrencia) muestra toast destructive; para otros errores de negocio (4040, 4041) muestra `Alert` inline dentro del dialog. Mantiene un contador de caracteres en tiempo real para el campo descripcion.

**Validacion adicional en el componente:**
```
form.watch('importe') > saldoDisponible
  => form.setError('importe', { message: `El importe no puede superar tu saldo disponible (${saldoDisponible.toFixed(2)} EUR)` })
```

---

## 4. Hooks

### 4.1 usePromotorWallet

**Archivo:** `src/web/src/features/crowdpromotion/wallet/application/hooks/usePromotorWallet.ts`

**Tipo:** Query Hook

**Parametros:** ninguno (el PromotorId se resuelve en el backend via JWT)

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `PromotorWallet \| undefined` | Datos del wallet del promotor autenticado |
| isLoading | `boolean` | True durante la carga inicial |
| isError | `boolean` | True si hay error en la query |
| error | `Error \| null` | Error con `errorCode` adjunto si aplica |
| refetch | `() => void` | Funcion para reintentar la carga |

**Query Key:** `QUERY_KEYS.crowdpromotion.wallet.resumen` → `['crowdpromotion', 'wallet', 'resumen']`

**Configuracion:**
- `staleTime: 30 * 1000` - 30 segundos (el saldo puede cambiar tras un cobro, valor moderado)
- `retry: 1`
- No se necesita `enabled` ya que siempre se ejecuta al montar la pagina protegida

**Comportamiento de error:**
Si el backend retorna 404 con errorCode `2030` (wallet no encontrado), el error se propaga normalmente. El componente `PromotorWalletPage` distingue este caso para mostrar el empty state en lugar del error generico.

---

### 4.2 useWalletTransacciones

**Archivo:** `src/web/src/features/crowdpromotion/wallet/application/hooks/useWalletTransacciones.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| filters | `WalletTransaccionesFilters` | Filtros activos y parametros de paginacion |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `WalletTransaccionesPagedResponse \| undefined` | Respuesta paginada con items y metadatos |
| isLoading | `boolean` | True durante la carga inicial |
| isFetching | `boolean` | True durante refetch con datos previos (util para indicar carga de filtros) |
| isError | `boolean` | True si hay error |
| refetch | `() => void` | Reintentar carga |

**Query Key:** `QUERY_KEYS.crowdpromotion.wallet.transacciones(filters)` → `['crowdpromotion', 'wallet', 'transacciones', filters]`

**Configuracion:**
- `staleTime: 30 * 1000`
- `retry: 1`
- `keepPreviousData: true` - mantiene datos previos durante cambio de filtros/paginacion para evitar parpadeo

**Nota de implementacion:** El hook pasa el objeto `filters` completo a la query key. Cuando `filters` cambia (al aplicar filtros o cambiar pagina), TanStack Query detecta el cambio y ejecuta el refetch automaticamente.

---

### 4.3 useSolicitarCobro

**Archivo:** `src/web/src/features/crowdpromotion/wallet/application/hooks/useSolicitarCobro.ts`

**Tipo:** Mutation Hook

**Parametros del mutationFn:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| data | `SolicitarCobroRequest` | `{ importe: number, descripcion?: string }` |

**Retorna:** `UseMutationResult` de TanStack Query con:

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| mutateAsync | `(data: SolicitarCobroRequest) => Promise<SolicitarCobroResponse>` | Ejecutar mutation, retorna la respuesta |
| isPending | `boolean` | True mientras procesa la solicitud |
| isError | `boolean` | True si hubo error |
| error | `Error & { errorCode?: string } \| null` | Error con errorCode del backend |

**Acciones:**
- `mutateAsync(data)` - Llamar al servicio y retornar la respuesta (el componente maneja onSuccess/onError localmente)
- El hook NO implementa `onSuccess`/`onError` internamente porque el `SolicitarCobroDialog` necesita manejar el cierre del dialog, el toast y la invalidacion de forma coordinada
- La invalidacion de queries se hace en el componente consumidor (`SolicitarCobroDialog`), no en el hook, para mantener el control del flujo

**Queries a invalidar (en el consumidor tras exito):**
```
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.wallet.resumen })
queryClient.invalidateQueries({ queryKey: ['crowdpromotion', 'wallet', 'transacciones'] })
```

**Nota de diseno:** Se usa `mutateAsync` en el dialog para poder usar `try/catch` y manejar el estado del dialog de forma sincrona. Esto sigue el mismo patron que `useCompletarTarea` en la feature de tareas.

---

## 5. Services

### 5.1 walletService

**Archivo:** `src/web/src/features/crowdpromotion/wallet/infrastructure/wallet.service.ts`

**Patron:** Clase con metodos async, instancia singleton exportada (igual que `inscripcionService` y `trackingService`)

**Interfaz interna `ServiceResponse<T>`:**
```typescript
interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}
```

**Funcion helper `extractError`:** Igual que en los servicios existentes - busca el primer mensaje con errorCode que no empiece por "0" y construye un `Error` con `errorCode` adjunto en la instancia.

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getWalletResumen` | ninguno | `Promise<PromotorWallet>` | `GET API_ROUTES.crowdpromotion.promotorWallet.resumen` |
| `getTransacciones` | `filters: WalletTransaccionesFilters` | `Promise<WalletTransaccionesPagedResponse>` | `GET API_ROUTES.crowdpromotion.promotorWallet.transacciones?{params}` |
| `solicitarCobro` | `data: SolicitarCobroRequest` | `Promise<SolicitarCobroResponse>` | `POST API_ROUTES.crowdpromotion.promotorWallet.cobro` |

**Construccion de query params en `getTransacciones`:**
```
const params = new URLSearchParams()
if (filters.esCredito !== undefined) params.set('esCredito', String(filters.esCredito))
if (filters.estadoTransaccionId !== undefined) params.set('estadoTransaccionId', String(filters.estadoTransaccionId))
if (filters.fechaDesde) params.set('fechaDesde', filters.fechaDesde)
if (filters.fechaHasta) params.set('fechaHasta', filters.fechaHasta)
params.set('page', String(filters.page ?? 1))
params.set('pageSize', String(filters.pageSize ?? 10))
```

**Manejo de errores:** Si `response.messages` contiene algun mensaje con errorCode que no empieza por "0", lanzar el error extraido via `extractError`. Esto propaga el `errorCode` para que los hooks y componentes puedan manejarlo de forma especifica (ej: distinguir 2030 de 5000).

---

## 6. Domain Index

**Archivo:** `src/web/src/features/crowdpromotion/wallet/domain/index.ts`

Re-exporta todos los types necesarios desde `@shared/types/crowdpromotion`. Sigue el patron exacto de `metricas/domain/index.ts` y `tareas/domain/index.ts`.

**Tipos a re-exportar:**
```typescript
export type {
    PromotorWallet,
    WalletTransaccionItem,
    WalletTransaccionesPagedResponse,
    SolicitarCobroRequest,
    SolicitarCobroResponse,
    WalletTransaccionesFilters,
} from '@shared/types/crowdpromotion'
```

---

## 7. Integracion con Router

**Archivo a modificar:** `src/web/src/app/router.tsx`

**Cambio 1 - Import lazy:**
```typescript
const PromotorWalletPage = lazy(
    () => import('@/features/crowdpromotion/wallet/presentation/pages/PromotorWalletPage')
)
```

**Cambio 2 - Nueva ruta dentro del bloque de rutas protegidas** (`<Route element={<DashboardLayout />}>`):
```tsx
<Route path="/promotor/wallet" element={<PromotorWalletPage />} />
```

**Posicion en el bloque:** Despues de la ruta `/promotor/metricas`, siguiendo el orden de navegacion del sidebar.

**Nota de proteccion:** La ruta usa `DashboardLayout` que ya implementa la proteccion de autenticacion del proyecto. No se requiere un guard adicional para esta feature.

---

## 8. Flujo de Datos

```
Usuario accede a /promotor/wallet
    |
    v
PromotorWalletPage (monta)
    |
    +---> usePromotorWallet()
    |         |
    |         v
    |     walletService.getWalletResumen()
    |         |
    |         v
    |     GET /api/crowdpromotion/promotor/wallet (JWT Bearer)
    |         |
    |         v
    |     PromotorWallet (data)
    |         |
    |         v
    |     WalletSaldoCard (presenta saldo, habilita boton)
    |
    +---> useWalletTransacciones({ page: 1, pageSize: 10 })
              |
              v
          walletService.getTransacciones(filters)
              |
              v
          GET /api/crowdpromotion/promotor/wallet/transacciones?page=1&pageSize=10
              |
              v
          WalletTransaccionesPagedResponse (data)
              |
              v
          TransaccionesList -> TransaccionItem (presenta filas)
          PaginacionWallet (presenta controles de pagina)

Usuario aplica filtro
    |
    v
FiltrosHistorial.onChange(newFilters)
    |
    v
PromotorWalletPage actualiza estado filters (con page: 1 forzado)
    |
    v
useWalletTransacciones recibe nueva filters (query key cambia)
    |
    v
TanStack Query ejecuta refetch automatico
    |
    v
TransaccionesList actualiza con nuevos resultados

Usuario hace click en "Solicitar cobro"
    |
    v
PromotorWalletPage.setIsCobroDialogOpen(true)
    |
    v
SolicitarCobroDialog (open=true, saldoDisponible, minimoRetiro, monedaNombre)
    |
    v
Usuario completa formulario y confirma
    |
    v
SolicitarCobroDialog -> useSolicitarCobro.mutateAsync({ importe, descripcion })
    |
    v
walletService.solicitarCobro({ importe, descripcion })
    |
    v
POST /api/crowdpromotion/promotor/wallet/cobro (JWT Bearer)
    |
    +--- EXITO (201) ---+
    |                   |
    |                   v
    |               SolicitarCobroResponse (transaccionId, saldoRestante, ...)
    |                   |
    |                   v
    |               queryClient.invalidateQueries(wallet.resumen)
    |               queryClient.invalidateQueries(wallet.transacciones)
    |               onSuccess(saldoRestante)
    |               toast.success('Solicitud de cobro registrada...')
    |               dialog.close()
    |
    +--- ERROR (4040/4041) ---+
                              |
                              v
                          Alert inline en dialog (mantener abierto)

    +--- ERROR (4042) ---+
                         |
                         v
                     toast.error('Hubo un conflicto...')
                     dialog.close()
```

---

## 9. Dependencias de Shared

### 9.1 Types (`@shared/types/crowdpromotion`)

Importar los siguientes tipos (definidos en `contracts-plan.md` de shared):

| Type | Uso |
|------|-----|
| `PromotorWallet` | Respuesta de GET /wallet |
| `WalletTransaccionItem` | Item individual del historial |
| `WalletTransaccionesPagedResponse` | Respuesta paginada de GET /transacciones |
| `SolicitarCobroRequest` | Body del POST /cobro |
| `SolicitarCobroResponse` | Respuesta del POST /cobro |
| `WalletTransaccionesFilters` | Filtros y paginacion para el historial |

### 9.2 Schemas (`@shared/schemas/crowdpromotion.schema`)

| Schema / Type | Uso |
|---------------|-----|
| `solicitarCobroSchema` | Validacion del formulario en `SolicitarCobroDialog` |
| `SolicitarCobroFormData` | Tipo inferido del formulario (react-hook-form) |

### 9.3 Constantes (`@shared/constants`)

| Constante | Uso |
|-----------|-----|
| `QUERY_KEYS.crowdpromotion.wallet.resumen` | Query key para el resumen del wallet |
| `QUERY_KEYS.crowdpromotion.wallet.transacciones(filters)` | Query key para el historial |
| `API_ROUTES.crowdpromotion.promotorWallet.resumen` | Endpoint GET wallet |
| `API_ROUTES.crowdpromotion.promotorWallet.transacciones` | Endpoint GET transacciones |
| `API_ROUTES.crowdpromotion.promotorWallet.cobro` | Endpoint POST cobro |
| `ESTADO_WALLET_TRANSACCION` | IDs de estados (1-4) para los selects de filtros |
| `ESTADO_WALLET_TRANSACCION_LABELS` | Nombres de estados para mostrar en UI |
| `MIN_RETIRO_WALLET` | Minimo de retiro (10.00) para soft check client-side |
| `WALLET_DEFAULT_PAGE_SIZE` | Tamano de pagina por defecto (10) |

### 9.4 Utils (`@shared/utils`)

| Funcion | Archivo | Uso |
|---------|---------|-----|
| `formatWalletImporte` | `format.ts` | Formatear saldo disponible y totales en WalletSaldoCard |
| `formatTransaccionImporte` | `format.ts` | Formatear importe con signo en TransaccionItem |
| `mapEstadoWalletTransaccionToBadge` | `mappers.ts` | Obtener variant del badge en TransaccionItem |
| `getWalletErrorMessage` | `error-messages.ts` | Traducir errorCodes del backend a mensajes de usuario en hooks/dialog |

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdpromotion/wallet/domain/index.ts` | Domain re-export | Re-exporta 6 types desde @shared |
| `src/web/src/features/crowdpromotion/wallet/infrastructure/wallet.service.ts` | Service class | 3 metodos API: getWalletResumen, getTransacciones, solicitarCobro |
| `src/web/src/features/crowdpromotion/wallet/application/hooks/usePromotorWallet.ts` | Query hook | useQuery para GET /wallet |
| `src/web/src/features/crowdpromotion/wallet/application/hooks/useWalletTransacciones.ts` | Query hook | useQuery para GET /transacciones con filtros |
| `src/web/src/features/crowdpromotion/wallet/application/hooks/useSolicitarCobro.ts` | Mutation hook | useMutation para POST /cobro |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/WalletSaldoCard.tsx` | Presentacion | Panel de saldo con skeleton y boton condicional |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/FiltrosHistorial.tsx` | Presentacion | Barra de filtros con selects y fechas con debounce |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/TransaccionItem.tsx` | Presentacion | Fila de transaccion con icono, importe con signo y badge de estado |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/TransaccionesList.tsx` | Presentacion | Lista de transacciones con skeleton y empty states |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/PaginacionWallet.tsx` | Presentacion | Controles de paginacion con texto informativo |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/WalletEmptyState.tsx` | Presentacion | Empty state sin transacciones con CTA a explorar programas |
| `src/web/src/features/crowdpromotion/wallet/presentation/components/SolicitarCobroDialog.tsx` | Presentacion | Dialog con formulario RHF+Zod, validacion de importe vs saldo y manejo de errores inline |
| `src/web/src/features/crowdpromotion/wallet/presentation/pages/PromotorWalletPage.tsx` | Page | Orquestador de la pagina; gestiona estado de filtros y dialog |

**Archivos a modificar:**

| Archivo | Cambio |
|---------|--------|
| `src/web/src/app/router.tsx` | Agregar lazy import de PromotorWalletPage y ruta `/promotor/wallet` en bloque DashboardLayout |

**Archivos de test a crear:**

| Archivo | Tipo |
|---------|------|
| `src/web/src/features/crowdpromotion/__mocks__/wallet.mock.ts` | Mock data de PromotorWallet, WalletTransaccionItem, SolicitarCobroResponse |
| `src/web/src/features/crowdpromotion/__tests__/services/wallet.service.test.ts` | Tests del servicio (3 metodos) |
| `src/web/src/features/crowdpromotion/__tests__/hooks/usePromotorWallet.test.ts` | Tests del query hook |
| `src/web/src/features/crowdpromotion/__tests__/hooks/useWalletTransacciones.test.ts` | Tests del query hook con filtros |
| `src/web/src/features/crowdpromotion/__tests__/hooks/useSolicitarCobro.test.ts` | Tests del mutation hook |
| `src/web/src/features/crowdpromotion/__tests__/components/WalletSaldoCard.test.tsx` | Tests de presentacion y estados del boton |
| `src/web/src/features/crowdpromotion/__tests__/components/TransaccionItem.test.tsx` | Tests de renderizado de credito/debito y badges |
| `src/web/src/features/crowdpromotion/__tests__/components/FiltrosHistorial.test.tsx` | Tests de filtros y debounce |
| `src/web/src/features/crowdpromotion/__tests__/components/SolicitarCobroDialog.test.tsx` | Tests de formulario, validaciones y estados del dialog |
| `src/web/src/features/crowdpromotion/__tests__/pages/PromotorWalletPage.test.tsx` | Tests de integracion de la pagina |

---

## 11. Escenarios de UI Detallados

### Loading Inicial

```
PromotorWalletPage (isWalletLoading=true)
  |-- WalletSaldoCard (isLoading=true) --> renderiza skeleton
  |-- FiltrosHistorial deshabilitado (selects disabled, aria-busy)
  |-- TransaccionesList (isLoading=true) --> renderiza 5 filas skeleton
```

El titulo "Mi Wallet" se muestra siempre. El contenedor principal tiene `aria-busy="true"`.

### Error al Cargar Wallet

```
PromotorWalletPage (isWalletError=true)
  |-- Card centrada con AlertCircle + "No se pudo cargar tu wallet"
  |-- Button "Reintentar" (variant="outline") --> refetch()
  |-- NO se muestra FiltrosHistorial ni TransaccionesList
```

### Sin Transacciones (Primera Vez)

```
PromotorWalletPage (wallet.data cargado, transacciones.totalCount === 0, !hasActiveFilters)
  |-- WalletSaldoCard (saldoDisponible=0, boton deshabilitado)
  |-- NO se muestra FiltrosHistorial
  |-- WalletEmptyState (icono Wallet, "Aun no tienes transacciones", boton "Explorar programas")
```

### Con Datos y Filtros Activos Sin Resultados

```
PromotorWalletPage (wallet cargado, transacciones.items.length === 0, hasActiveFilters=true)
  |-- WalletSaldoCard (datos reales)
  |-- FiltrosHistorial (con filtros activos visibles)
  |-- TransaccionesList (items=[], hasActiveFilters=true)
        |-- Empty state de filtros: SearchX icon + "No hay transacciones que coincidan" + boton "Limpiar filtros"
```

### Post Cobro Exitoso

```
1. SolicitarCobroDialog.onSuccess(saldoRestante) dispara
2. queryClient.invalidateQueries(wallet.resumen) --> usePromotorWallet refetch automatico
3. queryClient.invalidateQueries(wallet.transacciones) --> useWalletTransacciones refetch
4. dialog se cierra
5. toast.success('Solicitud de cobro registrada. Procesaremos tu pago en breve.')
6. WalletSaldoCard se actualiza con el nuevo saldo al completar el refetch
7. La nueva transaccion de debito aparece al inicio de TransaccionesList
```

---

## 12. Notas de Implementacion

### Debounce en FiltrosHistorial

El componente `FiltrosHistorial` necesita debounce de 500ms para los inputs de fecha. Si el proyecto no tiene un hook `useDebounce`, implementarlo localmente con `useEffect` + `setTimeout`/`clearTimeout`. No instalar una libreria adicional.

### Validacion de Importe vs Saldo en SolicitarCobroDialog

El `solicitarCobroSchema` global no incluye la validacion `importe <= saldoDisponible` porque el schema no tiene acceso al prop. En el dialog, usar `watch('importe')` con un `useEffect` para llamar `setError('importe', { message })` cuando el importe supere el saldo. Alternativamente, crear el schema con `z.object(...).refine(...)` localmente en el componente pasando `saldoDisponible` como closure.

### keepPreviousData en useWalletTransacciones

Usar la opcion `placeholderData: keepPreviousData` (TanStack Query v5) o `keepPreviousData: true` (v4) para que al cambiar filtros o pagina, la lista anterior se mantenga visible con baja opacidad mientras carga la nueva pagina. Esto elimina el parpadeo entre peticiones. El estado `isFetching` (distinto de `isLoading`) se puede usar para mostrar un indicador sutil de recarga.

### Exportacion del Servicio

```typescript
// Al final de wallet.service.ts
export const walletService = new WalletService()
```

Singleton igual que `inscripcionService` y `trackingService`.

### Accesibilidad

- El area principal tiene `role="main"` y `aria-busy={isLoading}`
- Los errores de formulario tienen `role="alert"`
- Los botones deshabilitados tienen `aria-disabled="true"` ademas de `disabled`
- Las filas de transaccion tienen `role="listitem"` dentro de un contenedor con `role="list"`

---

## 13. Checklist

- [ ] Componentes usan shadcn/ui (`Card`, `Button`, `Input`, `Label`, `Textarea`, `Select`, `Dialog`, `Badge`, `Skeleton`, `Pagination`, `Alert`, `Separator`)
- [ ] Hooks siguen patron `useQuery` / `useMutation` de TanStack Query
- [ ] `useWalletTransacciones` usa `placeholderData: keepPreviousData` para evitar parpadeo al paginar
- [ ] `useSolicitarCobro` usa `mutateAsync` para control sincrono en el dialog
- [ ] Services usan `apiFetch` de `@/lib/api-client` y el patron `extractError` del proyecto
- [ ] Types importados de `@shared/types/crowdpromotion` (no duplicados)
- [ ] Schemas importados de `@shared/schemas/crowdpromotion.schema`
- [ ] Constantes importadas de `@shared/constants`
- [ ] Formulario de cobro usa React Hook Form + zodResolver(`solicitarCobroSchema`)
- [ ] Validacion de `importe <= saldoDisponible` implementada en el componente (no en el schema global)
- [ ] Boton "Solicitar cobro" deshabilitado cuando `saldoDisponible < minimoRetiro`
- [ ] Empty state con boton CTA a `/crowdpromotion/explorar` cuando no hay transacciones
- [ ] Paginacion con texto informativo "Mostrando X-Y de Z transacciones"
- [ ] Filtros de fecha con debounce de 500ms
- [ ] Validacion de rango de fechas (`fechaDesde <= fechaHasta`) con error visual
- [ ] Toast de exito tras cobro registrado usando `toast.success` de sonner
- [ ] Errores de negocio (4040, 4041) mostrados como Alert inline en el dialog
- [ ] Error de concurrencia (4042) mostrado como toast destructive
- [ ] Dialog no se cierra mientras `isSubmitting === true`
- [ ] Nueva ruta `/promotor/wallet` en `router.tsx` dentro del bloque `DashboardLayout`
- [ ] Lazy import de `PromotorWalletPage` en `router.tsx`
- [ ] Paleta de colores dark theme consistente con `PromotorMetricasPage` (#1a1a2e, #151525, #334155)
- [ ] Iconos `ArrowUp` (verde) para creditos y `ArrowDown` (rojo) para debitos
- [ ] `role="alert"` en contenedores de error, `aria-busy` en contenedor principal durante carga
