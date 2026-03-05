# Plan Frontend: cp-wallet-comisiones (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/admin (Next.js 14, App Router)

---

## AVISO IMPORTANTE: FUERA DE SCOPE EN MVP

> La feature-spec (seccion "Proyectos Involucrados") establece explicitamente:
>
> **Admin — Impacto: BAJO**
> _"Sin responsabilidad en MVP. Futuro: vista de administrador para ver wallets de promotores y marcar transacciones como Procesada/Pagada/Cancelada."_

**Este plan documenta la arquitectura futura para cuando se implemente la funcionalidad de administracion de wallets. No se debe implementar nada de este plan en el sprint actual (US-CP-06 MVP).**

La implementacion completa del MVP de cp-wallet-comisiones esta en:
- **Backend:** `plans/cp-wallet-comisiones/backend/`
- **Landing:** `plans/cp-wallet-comisiones/frontend-landing/`
- **Shared:** `plans/cp-wallet-comisiones/shared/contracts-plan.md`

---

## 1. Resumen

- **Estado:** Fuera de scope MVP — plan preparatorio para implementacion futura
- **Screens futuras:** 2 (lista de wallets de promotores + detalle de wallet individual)
- **Componentes futuros:** 8
- **Hooks futuros:** 3
- **Services futuros:** 1 (nuevo) + 1 extension de promotorService existente
- **Endpoints admin-especificos requeridos:** 3 (no existen aun en backend)

### Contexto de Admin en MVP

En MVP el administrador no necesita gestionar wallets porque:
1. Las transacciones de credito (comisiones) se crean automaticamente en el backend al validar tareas (US-CP-04) o registrar conversiones (US-CP-05).
2. El cambio de estado de transacciones (Pendiente -> Procesada -> Pagada) es una operacion manual que en MVP se describe como "fuera de scope".
3. El promotor gestiona su propia wallet desde la Landing (`/promotor/wallet`).

### Lo que existira en MVP en Admin (sin cambios requeridos)

La informacion de wallets estara disponible implicitamente en los promotores ya visibles en:
- `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaPromotoresTab.tsx` — lista de promotores aprobados por programa
- `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaAprobadosTab.tsx` — detalle de aprobados

Ningun cambio es necesario en estos componentes para MVP.

---

## 2. Estructura de Carpetas Futura Propuesta

La siguiente estructura debe crearse cuando se implemente la funcionalidad de administracion de wallets. Sigue el patron establecido en el codebase de admin para crowdpromotion.

```
src/admin/src/
│
├── app/(dashboard)/crowdpromotion/
│   │
│   ├── wallets/                                     NUEVO directorio (futuro)
│   │   ├── page.tsx                                 Pagina principal: lista de wallets de todos los promotores
│   │   └── [promotorId]/
│   │       └── page.tsx                             Detalle de wallet de un promotor especifico
│   │
│   └── components/
│       └── wallets/                                 NUEVO directorio de componentes de lista (futuro)
│           ├── WalletListClient.tsx                 Client component: tabla de wallets con filtros
│           ├── WalletFilters.tsx                    Filtros de busqueda de la lista
│           └── WalletStatusBadge.tsx                Badge del estado de la wallet (activa/sin saldo)
│
├── components/crowdpromotion/
│   └── wallets/                                     NUEVO directorio de componentes reutilizables (futuro)
│       ├── WalletResumenCard.tsx                    Card con saldo disponible, pendiente, total ganado, retirado
│       ├── TransaccionesTable.tsx                   Tabla paginada de transacciones con filtros
│       ├── TransaccionEstadoBadge.tsx               Badge de estado (Pendiente/Procesada/Pagada/Cancelada)
│       ├── TransaccionTipoBadge.tsx                 Badge de tipo (Credito/Debito con colores)
│       ├── CambiarEstadoTransaccionDialog.tsx       Dialog para marcar transacciones como Procesada/Pagada/Cancelada
│       └── WalletEmptyState.tsx                     Empty state cuando el promotor no tiene transacciones
│
├── hooks/
│   ├── use-wallets-promotores.ts                    NUEVO: lista paginada de wallets (futuro)
│   ├── use-wallet-detalle.ts                        NUEVO: wallet individual con transacciones (futuro)
│   └── use-wallet-transaccion-mutation.ts           NUEVO: cambiar estado de transaccion (futuro)
│
└── services/
    └── wallet-admin.service.ts                      NUEVO: endpoints admin-especificos de wallet (futuro)
```

### Relacion con la estructura existente

```
src/admin/src/app/(dashboard)/crowdpromotion/
├── programas/               YA EXISTE - gestion de programas
│   ├── page.tsx             YA EXISTE
│   └── [id]/                YA EXISTE
│       └── page.tsx         YA EXISTE
├── wallets/                 FUTURO - gestion de wallets de promotores
│   ├── page.tsx             FUTURO
│   └── [promotorId]/        FUTURO
│       └── page.tsx         FUTURO
└── components/
    ├── list/                YA EXISTE - componentes de lista de programas
    ├── wizard/              YA EXISTE - wizard de creacion de programas
    └── wallets/             FUTURO - componentes de lista de wallets
```

---

## 3. Componentes Futuros

### 3.1 WalletListClient

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/wallets/components/WalletListClient.tsx`

**Responsabilidad:** Client component principal de la pagina de lista de wallets. Renderiza la tabla de wallets de todos los promotores con paginacion server-side y filtros basicos (busqueda por nombre de promotor, filtro por programa). Cada fila tiene un enlace al detalle de wallet del promotor.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| (ninguna) | - | - | Componente auto-contenido que gestiona su propio estado de filtros |

**Estado local:**
- `filtros: { busqueda: string; programaId?: string; page: number }` - filtros de la lista
- Gestionado via URL query params con `useSearchParams` y `useRouter` de Next.js

**Dependencias:**
- Hooks: `useWalletsPromotores`
- Componentes: `WalletFilters`, `WalletStatusBadge`
- shadcn/ui: `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableHead`, `TableCell`, `Button`, `Skeleton`
- lucide-react: `Wallet`, `ArrowRight`
- next/navigation: `useSearchParams`, `useRouter`, `Link`

**Directiva:** `"use client"`

---

### 3.2 WalletFilters

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/wallets/components/WalletFilters.tsx`

**Responsabilidad:** Barra de filtros de la lista de wallets. Incluye campo de busqueda por nombre de promotor (con debounce) y selector de programa (opcional). Sigue el patron de `PromoProgramaFilters` existente.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `busqueda` | `string` | Si | Valor actual del campo de busqueda (controlado) |
| `onBusquedaChange` | `(value: string) => void` | Si | Callback al cambiar busqueda |
| `onLimpiar` | `() => void` | Si | Callback para limpiar todos los filtros |
| `tieneFiltroPeriodo` | `boolean` | Si | Controla visibilidad del boton "Limpiar" |

**Estado local:**
- `inputValue: string` - valor del input antes del debounce (750ms)

**shadcn/ui:** `Input`, `Button`
**lucide-react:** `Search`, `X`
**Directiva:** `"use client"`

---

### 3.3 WalletResumenCard

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/WalletResumenCard.tsx`

**Responsabilidad:** Card que muestra el resumen financiero de la wallet de un promotor. Muestra cuatro metricas en grid: saldo disponible (destacado), saldo pendiente, total ganado y total retirado. Incluye la moneda y el minimo de retiro. Sigue el patron de `PromoProgramaKpiCards` existente.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `wallet` | `PromotorWallet` | Si | Datos del wallet del promotor (type de shared) |

**Estado local:** Ninguno

**Dependencias:**
- Importa de shared: `PromotorWallet`
- Importa de shared/utils: `formatWalletImporte`
- shadcn/ui: `Card`, `CardContent`, `CardHeader`, `CardTitle`
- lucide-react: `Wallet`, `TrendingUp`, `TrendingDown`, `Clock`

**Directiva:** Componente servidor (sin hooks de cliente)

---

### 3.4 TransaccionesTable

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/TransaccionesTable.tsx`

**Responsabilidad:** Tabla paginada del historial de transacciones de la wallet de un promotor. Muestra las columnas: fecha, tipo (credito/debito con color), importe con signo, descripcion/concepto, estado con badge, y columna de acciones (boton "Cambiar estado" solo para transacciones en estado Pendiente o Procesada). Incluye filtros de tipo y estado.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `transacciones` | `WalletTransaccionItem[]` | Si | Array de transacciones de la pagina actual (types de shared) |
| `totalCount` | `number` | Si | Total de transacciones para la paginacion |
| `page` | `number` | Si | Pagina actual |
| `pageSize` | `number` | Si | Elementos por pagina |
| `onPageChange` | `(page: number) => void` | Si | Callback al cambiar pagina |
| `onFiltroChange` | `(filtros: WalletTransaccionesFilters) => void` | Si | Callback al cambiar filtros |
| `filtros` | `WalletTransaccionesFilters` | Si | Filtros activos actualmente |
| `onCambiarEstado` | `(transaccionId: string) => void` | Si | Callback al click en "Cambiar estado" — abre el dialog |
| `isLoading` | `boolean` | Si | Estado de carga para mostrar skeletons |

**Columnas:**

| Columna | Campo | Descripcion |
|---------|-------|-------------|
| Fecha | `fechaCreacion` | Formateada como DD/MM/YYYY HH:mm |
| Tipo | `esCredito` | Badge via `TransaccionTipoBadge` |
| Importe | `importe` + `esCredito` | Formateado con signo via `formatTransaccionImporte` |
| Descripcion | `descripcion` o `concepto` | Texto truncado a 60 chars con title completo |
| Estado | `estadoTransaccionNombre` | Badge via `TransaccionEstadoBadge` |
| Acciones | - | Boton "Cambiar estado" si estado es Pendiente (1) o Procesada (2) |

**Empty state:** Cuando `transacciones.length === 0`, renderiza `WalletEmptyState`

**Dependencias:**
- Importa de shared: `WalletTransaccionItem`, `WalletTransaccionesFilters`
- Importa de shared/utils: `formatTransaccionImporte`
- Importa de shared/constants: `ESTADO_WALLET_TRANSACCION`
- Componentes: `TransaccionEstadoBadge`, `TransaccionTipoBadge`, `WalletEmptyState`
- shadcn/ui: `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableHead`, `TableCell`, `Button`, `Select`, `Skeleton`
- lucide-react: `ChevronLeft`, `ChevronRight`, `Settings`

**Directiva:** `"use client"` (filtros y paginacion interactiva)

---

### 3.5 TransaccionEstadoBadge

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/TransaccionEstadoBadge.tsx`

**Responsabilidad:** Badge visual para el estado de una transaccion de wallet. Muestra el nombre del estado con el color correspondiente segun `ESTADO_WALLET_TRANSACCION_BADGES` de shared. Sigue el patron de `PromoProgramaStatusBadge` existente en el codebase.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoId` | `number` | Si | ID del estado (1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada) |
| `estadoNombre` | `string` | Si | Nombre del estado para mostrar en el badge |
| `className` | `string` | No | Clase adicional para el badge |

**Mapeo de variantes:**

| Estado ID | Nombre | Variante shadcn | Color visual |
|-----------|--------|-----------------|--------------|
| 1 | Pendiente | `secondary` | Gris/neutro |
| 2 | Procesada | `default` | Azul |
| 3 | Pagada | `success` | Verde |
| 4 | Cancelada | `destructive` | Rojo |

**Dependencias:**
- Importa de shared/constants: `ESTADO_WALLET_TRANSACCION_BADGES`
- shadcn/ui: `Badge`

**Directiva:** Componente servidor

---

### 3.6 TransaccionTipoBadge

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/TransaccionTipoBadge.tsx`

**Responsabilidad:** Badge visual para el tipo de transaccion (Credito/Debito). Muestra icono de flecha y color verde para creditos, rojo para debitos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `esCredito` | `boolean` | Si | true=Credito (ingreso), false=Debito (retiro) |
| `className` | `string` | No | Clase adicional |

**Variantes:**

| esCredito | Texto | Color texto | Icono |
|-----------|-------|-------------|-------|
| `true` | Credito | `text-green-500` | `ArrowDownLeft` |
| `false` | Debito | `text-red-500` | `ArrowUpRight` |

**shadcn/ui:** `Badge`
**lucide-react:** `ArrowDownLeft`, `ArrowUpRight`
**Directiva:** Componente servidor

---

### 3.7 CambiarEstadoTransaccionDialog

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/CambiarEstadoTransaccionDialog.tsx`

**Responsabilidad:** Dialog para que el administrador cambie manualmente el estado de una transaccion de wallet. Permite transiciones validas: Pendiente -> Procesada, Pendiente -> Cancelada, Procesada -> Pagada, Procesada -> Cancelada. Muestra el estado actual, el selector de nuevo estado (con solo las transiciones validas disponibles), un campo de notas opcional y los botones Cancelar/Confirmar. Al confirmar, llama al mutation hook y muestra toast de resultado.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Controla visibilidad del dialog |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback para abrir/cerrar |
| `transaccionId` | `string` | Si | ID de la transaccion a modificar |
| `estadoActualId` | `number` | Si | Estado actual (para calcular transiciones validas) |
| `estadoActualNombre` | `string` | Si | Nombre del estado actual para mostrar |
| `importe` | `number` | Si | Importe de la transaccion (informativo en el dialog) |
| `esCredito` | `boolean` | Si | Tipo de transaccion (informativo) |
| `monedaNombre` | `string` | Si | Moneda del wallet (informativo) |

**Estado local:**
- `nuevoEstadoId: number | null` - estado seleccionado en el selector
- Gestionado via `useForm` de React Hook Form + `cambiarEstadoTransaccionSchema` (schema Zod futuro en shared)

**Transiciones validas por estado:**

| Estado actual | Transiciones disponibles |
|---------------|-------------------------|
| Pendiente (1) | Procesada (2), Cancelada (4) |
| Procesada (2) | Pagada (3), Cancelada (4) |
| Pagada (3) | Sin transiciones (estado final) |
| Cancelada (4) | Sin transiciones (estado final) |

**Dependencias:**
- Hooks: `useWalletTransaccionMutation`
- Importa de shared/constants: `ESTADO_WALLET_TRANSACCION`, `ESTADO_WALLET_TRANSACCION_LABELS`
- shadcn/ui: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`, `Button`, `Select`, `SelectItem`, `Textarea`, `Label`
- lucide-react: `AlertTriangle`

**Directiva:** `"use client"`

---

### 3.8 WalletEmptyState

**Archivo:** `src/admin/src/components/crowdpromotion/wallets/WalletEmptyState.tsx`

**Responsabilidad:** Componente de estado vacio para la tabla de transacciones cuando no hay resultados (ya sea porque el promotor no tiene transacciones o porque los filtros activos no tienen resultados). Muestra un icono, mensaje descriptivo y, si hay filtros activos, un boton "Limpiar filtros".

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tieneFiltroPeriodo` | `boolean` | Si | Si hay filtros activos (cambia el mensaje y muestra boton Limpiar) |
| `onLimpiarFiltros` | `() => void` | No | Callback del boton "Limpiar filtros" (solo visible si `tieneFiltroPeriodo=true`) |

**Variantes de mensaje:**

| tieneFiltroPeriodo | Icono | Mensaje |
|--------------------|-------|---------|
| `false` | `Wallet` | "Este promotor aun no tiene transacciones en su wallet." |
| `true` | `SearchX` | "No se encontraron transacciones con los filtros activos." |

**shadcn/ui:** `Button`
**lucide-react:** `Wallet`, `SearchX`
**Directiva:** Componente servidor

---

## 4. Hooks Futuros

### 4.1 useWalletsPromotores

**Archivo:** `src/admin/src/hooks/use-wallets-promotores.ts`

**Tipo:** Query Hook (lectura — lista paginada)

**Responsabilidad:** Wrapper de `useQuery` para el futuro endpoint admin `GET /api/crowdpromotion/admin/wallets`. Retorna la lista paginada de wallets de todos los promotores de los programas del artista autenticado, con filtro opcional por nombre de promotor y programaId.

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `filtros` | `AdminWalletsFilters` | Filtros de busqueda (busqueda, programaId, page, pageSize) |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `AdminWalletsPagedResponse \| undefined` | Lista paginada de wallets |
| `isLoading` | `boolean` | Estado de carga inicial |
| `isFetching` | `boolean` | Estado de carga en refetch |
| `isError` | `boolean` | Error en el fetch |
| `error` | `Error \| null` | Objeto de error |

**Query Key:** `QUERY_KEYS.crowdpromotion.adminWallets(filtros)` (constante futura en shared)

**Configuracion:**
- `staleTime: 30_000` - 30 segundos de cache (datos de admin menos criticos que los de promotor)
- `retry: false`

---

### 4.2 useWalletDetalle

**Archivo:** `src/admin/src/hooks/use-wallet-detalle.ts`

**Tipo:** Query Hook compuesto (dos queries: resumen + transacciones paginadas)

**Responsabilidad:** Coordina dos queries para la pagina de detalle de wallet de un promotor: el resumen del wallet y el historial de transacciones paginado con filtros. Encapsula la logica de ambas queries para simplificar el componente de detalle.

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `promotorId` | `string` | ID del promotor cuyo wallet se consulta |
| `filtrosTransacciones` | `WalletTransaccionesFilters` | Filtros del historial de transacciones |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `walletData` | `PromotorWallet \| undefined` | Resumen del wallet (type de shared) |
| `walletLoading` | `boolean` | Carga del resumen |
| `transaccionesData` | `WalletTransaccionesPagedResponse \| undefined` | Historial paginado (type de shared) |
| `transaccionesLoading` | `boolean` | Carga del historial |
| `isError` | `boolean` | Cualquier error en cualquiera de las queries |
| `refetchAll` | `() => void` | Refetch de ambas queries |

**Nota de implementacion:** Este hook usa internamente `useQuery` de TanStack Query para cada endpoint por separado. No usa `useQueries` para mantener el patron simple del codebase existente.

**Endpoints consultados:**
- `GET /api/crowdpromotion/admin/wallets/{promotorId}` — resumen del wallet
- `GET /api/crowdpromotion/admin/wallets/{promotorId}/transacciones` — historial paginado con filtros

**Query Keys:**
- `QUERY_KEYS.crowdpromotion.adminWalletDetalle(promotorId)` (futura en shared)
- `QUERY_KEYS.crowdpromotion.adminWalletTransacciones(promotorId, filtros)` (futura en shared)

---

### 4.3 useWalletTransaccionMutation

**Archivo:** `src/admin/src/hooks/use-wallet-transaccion-mutation.ts`

**Tipo:** Mutation Hook (escritura)

**Responsabilidad:** Wrapper de `useMutation` para el futuro endpoint admin `PATCH /api/crowdpromotion/admin/wallets/transacciones/{transaccionId}/estado`. Permite cambiar el estado de una transaccion de Pendiente a Procesada/Cancelada, o de Procesada a Pagada/Cancelada. En `onSuccess` invalida las queries de detalle de wallet y muestra toast de confirmacion. En `onError` muestra toast de error.

**Parametros del mutationFn:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `transaccionId` | `string` | ID de la transaccion a modificar |
| `nuevoEstadoId` | `number` | Nuevo estado (2=Procesada, 3=Pagada, 4=Cancelada) |
| `notas` | `string \| undefined` | Notas opcionales del administrador |

**Retorna:** Resultado estandar de `useMutation` de TanStack Query

**Comportamiento onSuccess:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.adminWalletDetalle(promotorId) })`
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.adminWalletTransacciones(promotorId) })`
- `toast.success("Estado de transaccion actualizado correctamente")`

**Comportamiento onError:**
- `toast.error(getWalletErrorMessage(error.message))` (usando funcion de shared/utils)

---

## 5. Service Futuro

### 5.1 walletAdminService

**Archivo:** `src/admin/src/services/wallet-admin.service.ts`

**Responsabilidad:** Clase de servicio para los tres endpoints admin-especificos de wallet. Sigue el patron de clase con metodos async e instancia singleton exportada, identico a `metricasService`, `tareasService`, etc. Usa `apiFetch` para que el interceptor de axios agregue el token JWT del artista-administrador.

**Patron base:** Identico a `metricas.service.ts` — clase con constructor vacio, metodos async, `apiFetch`, verificacion de errores en `messages`, retorno tipado.

**Metodos futuros:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getWalletsPromotores` | `filtros: AdminWalletsFilters` | `Promise<AdminWalletsPagedResponse>` | `GET /api/crowdpromotion/admin/wallets` |
| `getWalletDetalle` | `promotorId: string` | `Promise<PromotorWallet>` | `GET /api/crowdpromotion/admin/wallets/{promotorId}` |
| `getWalletTransacciones` | `promotorId: string, filtros: WalletTransaccionesFilters` | `Promise<WalletTransaccionesPagedResponse>` | `GET /api/crowdpromotion/admin/wallets/{promotorId}/transacciones` |
| `cambiarEstadoTransaccion` | `transaccionId: string, nuevoEstadoId: number, notas?: string` | `Promise<void>` | `PATCH /api/crowdpromotion/admin/wallets/transacciones/{transaccionId}/estado` |

**Imports necesarios:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants` (constantes admin futuras)
- `PromotorWallet`, `WalletTransaccionesPagedResponse`, `WalletTransaccionesFilters` de `@shared/types/crowdpromotion`
- `getWalletErrorMessage` de `@shared/utils/error-messages`
- Tipos futuros: `AdminWalletsFilters`, `AdminWalletsPagedResponse` (nuevos en shared cuando se implemente)

**Export:** `export const walletAdminService = new WalletAdminService()`

---

## 6. Endpoints Backend Admin-Especificos Requeridos (Futuros)

Los tres endpoints de promotor ya definidos en `contracts.md` son privados del promotor y NO son accesibles por el administrador. Para la implementacion futura de Admin se requieren tres endpoints nuevos en el backend:

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| `GET` | `/api/crowdpromotion/admin/wallets` | Lista paginada de wallets de todos los promotores (de los programas del artista autenticado), con filtro por nombre y programaId | Bearer JWT + Rol Artista |
| `GET` | `/api/crowdpromotion/admin/wallets/{promotorId}` | Resumen del wallet de un promotor especifico (mismo DTO que `PromotorWalletDto` del promotor) | Bearer JWT + Rol Artista |
| `GET` | `/api/crowdpromotion/admin/wallets/{promotorId}/transacciones` | Historial paginado de transacciones de un promotor (mismo DTO que el endpoint del promotor, con los mismos filtros) | Bearer JWT + Rol Artista |
| `PATCH` | `/api/crowdpromotion/admin/wallets/transacciones/{transaccionId}/estado` | Cambia el estado de una transaccion (Pendiente->Procesada, Procesada->Pagada, cualquiera->Cancelada) | Bearer JWT + Rol Artista |

### Diferencias con los endpoints del promotor

| Aspecto | Endpoints del Promotor (MVP) | Endpoints Admin (Futuro) |
|---------|------------------------------|--------------------------|
| Autorizacion | JWT del promotor (extrae su propio PromotorId) | JWT del artista (accede a wallets de sus promotores) |
| Scope | Solo la wallet propia | Todos los promotores de sus programas |
| Operaciones | Read + solicitar cobro | Read + cambiar estado de transacciones |
| Ruta | `/promotor/wallet/...` | `/admin/wallets/...` |

### Tipos nuevos requeridos en shared para los endpoints admin

```typescript
// Agregar a src/shared/types/crowdpromotion.ts en la implementacion futura

export interface AdminWalletListItem {
    promotorId: string;
    promotorNombre: string;
    programaId: string;
    programaTitulo: string;
    walletId: string;
    monedaNombre: string;
    saldoDisponible: number;
    saldoPendiente: number;
    totalGanado: number;
    totalRetirado: number;
    transaccionesPendientes: number; // Cuenta de transacciones en estado Pendiente
}

export interface AdminWalletsPagedResponse {
    items: AdminWalletListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface AdminWalletsFilters {
    busqueda?: string;
    programaId?: string;
    page?: number;
    pageSize?: number;
}

export interface CambiarEstadoTransaccionRequest {
    nuevoEstadoId: number;
    notas?: string;
}
```

---

## 7. Paginas Futuras (Next.js App Router)

### 7.1 WalletsListPage

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/wallets/page.tsx`

**Tipo:** Server Component (fetching inicial en servidor, hidratado con Client Component)

**Responsabilidad:** Pagina de lista de wallets de todos los promotores. Lee los query params de la URL para el estado inicial de los filtros. Renderiza `WalletListClient` pasando los params iniciales.

**Ruta:** `/crowdpromotion/wallets`

**Breadcrumb:** `CrowdPromotion > Wallets`

**Consideraciones:**
- Esta pagina requiere rol Artista (autorizacion ya manejada por el layout `(dashboard)`)
- El sidebar de admin debe agregar un nuevo item "Wallets" bajo el grupo "CrowdPromotion" cuando se implemente

---

### 7.2 WalletDetallePage

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/wallets/[promotorId]/page.tsx`

**Tipo:** Server Component con Client Component interno

**Responsabilidad:** Pagina de detalle del wallet de un promotor especifico. Muestra el resumen financiero (`WalletResumenCard`) y la tabla completa de transacciones con filtros y paginacion (`TransaccionesTable`). El dialog de cambio de estado (`CambiarEstadoTransaccionDialog`) se monta en este nivel y recibe el ID de transaccion seleccionada como estado local.

**Ruta:** `/crowdpromotion/wallets/{promotorId}`

**Params:** `promotorId: string`

**Estado local (en client component interno):**
- `transaccionSeleccionada: { id: string; estadoId: number; estadoNombre: string; importe: number; esCredito: boolean; monedaNombre: string } | null`
- `filtros: WalletTransaccionesFilters` - filtros activos del historial

**Breadcrumb:** `CrowdPromotion > Wallets > {promotorNombre}`

---

## 8. Flujo de Datos Futuro

```
Artista accede a /crowdpromotion/wallets
    |
    v
WalletsListPage (Server Component)
    + Lee query params iniciales
    |
    v
WalletListClient (Client Component — "use client")
    + useWalletsPromotores(filtros)
    |
    v
useWalletsPromotores (Hook)
    + useQuery con QUERY_KEYS.crowdpromotion.adminWallets(filtros)
    |
    v
walletAdminService.getWalletsPromotores(filtros) (Service)
    + Construye URL con query params
    + apiFetch con token JWT del artista
    |
    v
GET /api/crowdpromotion/admin/wallets?busqueda=&programaId=&page=1
    |
    v (AdminWalletsPagedResponse)
    |
    v
WalletListClient renderiza tabla con filas enlazadas a /wallets/{promotorId}

---

Artista hace click en una fila -> /crowdpromotion/wallets/{promotorId}
    |
    v
WalletDetallePage (Server Component)
    |
    v
Client Component interno
    + useWalletDetalle(promotorId, filtros)
    |                    |
    |                    +-- useQuery GET /admin/wallets/{promotorId}         -> WalletResumenCard
    |                    +-- useQuery GET /admin/wallets/{promotorId}/transacciones?{filtros} -> TransaccionesTable
    |
    v
Artista hace click en "Cambiar estado" en una fila de TransaccionesTable
    |
    v
CambiarEstadoTransaccionDialog (modal)
    + useWalletTransaccionMutation()
    |
    v
walletAdminService.cambiarEstadoTransaccion(transaccionId, nuevoEstadoId, notas)
    |
    v
PATCH /api/crowdpromotion/admin/wallets/transacciones/{id}/estado
    |
    v (204 No Content o 200 OK)
    |
    v
onSuccess: invalidate queries -> refetch WalletResumenCard + TransaccionesTable
           + toast.success("Estado actualizado")
```

---

## 9. Dependencias de Shared

### Types de shared ya definidos en MVP (reutilizar sin cambios)

**Importar de `@shared/types/crowdpromotion`:**

| Tipo | Descripcion |
|------|-------------|
| `PromotorWallet` | Resumen del wallet (walletId, saldos, totales, minimoRetiro) |
| `WalletTransaccionItem` | Item individual del historial de transacciones |
| `WalletTransaccionesPagedResponse` | Respuesta paginada del historial |
| `WalletTransaccionesFilters` | Parametros de filtro del historial |

**Importar de `@shared/constants`:**

| Constante | Uso |
|-----------|-----|
| `ESTADO_WALLET_TRANSACCION` | Enum: `{ PENDIENTE: 1, PROCESADA: 2, PAGADA: 3, CANCELADA: 4 }` |
| `ESTADO_WALLET_TRANSACCION_LABELS` | Nombres para mostrar en UI |
| `ESTADO_WALLET_TRANSACCION_BADGES` | Variantes de Badge shadcn por estado |
| `WALLET_DEFAULT_PAGE_SIZE` | Tamano de pagina por defecto (10) |

**Importar de `@shared/utils`:**

| Funcion | Uso |
|---------|-----|
| `formatWalletImporte` | Formatea importes con moneda y 2 decimales |
| `formatTransaccionImporte` | Formatea importe con signo +/- segun esCredito |
| `mapEstadoWalletTransaccionToBadge` | Obtiene variante de badge por estadoId |
| `getWalletErrorMessage` | Traduce codigos de error a mensajes en espanol |

### Tipos nuevos en shared (solo para implementacion futura de Admin)

Los siguientes tipos NO existen aun en shared y se deben agregar cuando se implemente esta feature:

| Tipo | Archivo | Descripcion |
|------|---------|-------------|
| `AdminWalletListItem` | `src/shared/types/crowdpromotion.ts` | Item de la lista de wallets vista desde admin |
| `AdminWalletsPagedResponse` | `src/shared/types/crowdpromotion.ts` | Lista paginada de wallets admin |
| `AdminWalletsFilters` | `src/shared/types/crowdpromotion.ts` | Filtros de la lista admin |
| `CambiarEstadoTransaccionRequest` | `src/shared/types/crowdpromotion.ts` | Request del PATCH de estado |

### Constantes nuevas en shared (solo para implementacion futura de Admin)

```typescript
// Agregar a src/shared/constants/index.ts en la implementacion futura
// Dentro de QUERY_KEYS.crowdpromotion:
adminWallets: (filtros?: AdminWalletsFilters) =>
    ['crowdpromotion', 'admin', 'wallets', filtros] as const,
adminWalletDetalle: (promotorId: string) =>
    ['crowdpromotion', 'admin', 'wallets', promotorId] as const,
adminWalletTransacciones: (promotorId: string, filtros?: WalletTransaccionesFilters) =>
    ['crowdpromotion', 'admin', 'wallets', promotorId, 'transacciones', filtros] as const,

// Dentro de API_ROUTES.crowdpromotion:
adminWallets: {
    lista: '/api/crowdpromotion/admin/wallets',
    detalle: (promotorId: string) => `/api/crowdpromotion/admin/wallets/${promotorId}`,
    transacciones: (promotorId: string) => `/api/crowdpromotion/admin/wallets/${promotorId}/transacciones`,
    cambiarEstado: (transaccionId: string) => `/api/crowdpromotion/admin/wallets/transacciones/${transaccionId}/estado`,
},

// Dentro de APP_ROUTES.admin:
wallets: {
    lista: '/crowdpromotion/wallets',
    detalle: (promotorId: string) => `/crowdpromotion/wallets/${promotorId}`,
},
```

---

## 10. Navegacion en Sidebar (Futuro)

Cuando se implemente esta feature, el sidebar de admin debe agregar un nuevo item bajo el grupo "CrowdPromotion":

**Archivo a modificar:** El componente de navegacion lateral del dashboard (identificar el archivo correcto en el codebase al momento de implementar).

**Item a agregar:**
```
Grupo: CrowdPromotion
  - Programas   (ya existe: /crowdpromotion/programas)
  - Wallets     (nuevo: /crowdpromotion/wallets)  ← agregar aqui
```

El item "Wallets" debe incluir un indicador de conteo si hay transacciones en estado Pendiente (badge rojo), similar al badge de solicitudes pendientes en la vista de programas.

---

## 11. Estimacion de Esfuerzo para Implementacion Futura

| Categoria | Archivos | Esfuerzo Estimado |
|-----------|----------|-------------------|
| Endpoints backend nuevos (4) | Nuevos Commands/Queries en Crowdpromotion | 8-12 horas |
| Types y constantes en shared | 4 types nuevos + constantes | 1-2 horas |
| Service `walletAdminService` | 1 archivo nuevo | 1-2 horas |
| Hooks (3) | `use-wallets-promotores`, `use-wallet-detalle`, `use-wallet-transaccion-mutation` | 2-3 horas |
| Componentes (8) | Ver seccion 3 | 6-10 horas |
| Paginas Next.js (2) | `wallets/page.tsx`, `wallets/[promotorId]/page.tsx` | 1-2 horas |
| Sidebar | Modificacion del layout | 0.5 horas |
| Tests | Minimo 80% cobertura | 4-6 horas |
| **Total estimado** | | **23-37 horas** |

**Nota:** Esta estimacion asume que los endpoints de promotor ya estan implementados (MVP), ya que los tipos, schemas y constantes de wallet ya existen en shared. Si el backend admin no existe, el esfuerzo de backend es el mas significativo.

---

## 12. Archivos a Crear o Modificar (Futuro)

| Archivo | Accion | Descripcion |
|---------|--------|-------------|
| `src/shared/types/crowdpromotion.ts` | MODIFICAR | Agregar 4 tipos admin: `AdminWalletListItem`, `AdminWalletsPagedResponse`, `AdminWalletsFilters`, `CambiarEstadoTransaccionRequest` |
| `src/shared/constants/index.ts` | MODIFICAR | Agregar `QUERY_KEYS.crowdpromotion.adminWallets*`, `API_ROUTES.crowdpromotion.adminWallets`, `APP_ROUTES.admin.wallets` |
| `src/admin/src/app/(dashboard)/crowdpromotion/wallets/page.tsx` | CREAR | Pagina lista de wallets (Server Component) |
| `src/admin/src/app/(dashboard)/crowdpromotion/wallets/[promotorId]/page.tsx` | CREAR | Pagina detalle de wallet (Server Component con Client Component) |
| `src/admin/src/app/(dashboard)/crowdpromotion/wallets/components/WalletListClient.tsx` | CREAR | Client component de la lista con tabla y filtros |
| `src/admin/src/app/(dashboard)/crowdpromotion/wallets/components/WalletFilters.tsx` | CREAR | Barra de filtros de la lista |
| `src/admin/src/components/crowdpromotion/wallets/WalletResumenCard.tsx` | CREAR | Card de resumen financiero del wallet |
| `src/admin/src/components/crowdpromotion/wallets/TransaccionesTable.tsx` | CREAR | Tabla paginada de transacciones con filtros |
| `src/admin/src/components/crowdpromotion/wallets/TransaccionEstadoBadge.tsx` | CREAR | Badge de estado de transaccion |
| `src/admin/src/components/crowdpromotion/wallets/TransaccionTipoBadge.tsx` | CREAR | Badge de tipo credito/debito |
| `src/admin/src/components/crowdpromotion/wallets/CambiarEstadoTransaccionDialog.tsx` | CREAR | Dialog para cambio de estado |
| `src/admin/src/components/crowdpromotion/wallets/WalletEmptyState.tsx` | CREAR | Empty state de la tabla de transacciones |
| `src/admin/src/hooks/use-wallets-promotores.ts` | CREAR | Query hook lista de wallets |
| `src/admin/src/hooks/use-wallet-detalle.ts` | CREAR | Query hook detalle de wallet |
| `src/admin/src/hooks/use-wallet-transaccion-mutation.ts` | CREAR | Mutation hook cambio de estado |
| `src/admin/src/services/wallet-admin.service.ts` | CREAR | Service con 4 metodos para endpoints admin |
| `src/admin/src/hooks/index.ts` | MODIFICAR | Exportar los 3 hooks nuevos |
| `src/admin/src/services/index.ts` | MODIFICAR | Exportar `walletAdminService` |
| Layout de sidebar admin | MODIFICAR | Agregar item "Wallets" bajo grupo CrowdPromotion |

**Total: 16 archivos nuevos, 4 modificados**

---

## 13. Checklist de Implementacion Futura

### Backend Admin (prerequisito)
- [ ] Endpoint `GET /admin/wallets` implementado con autorizacion Artista
- [ ] Endpoint `GET /admin/wallets/{promotorId}` implementado
- [ ] Endpoint `GET /admin/wallets/{promotorId}/transacciones` implementado
- [ ] Endpoint `PATCH /admin/wallets/transacciones/{transaccionId}/estado` implementado
- [ ] Validacion de que el artista solo puede ver wallets de promotores de sus propios programas

### Shared (prerequisito)
- [ ] Types admin (`AdminWalletListItem`, `AdminWalletsPagedResponse`, etc.) agregados en shared
- [ ] Constantes de QUERY_KEYS, API_ROUTES y APP_ROUTES admin agregadas

### Frontend Admin
- [ ] Service `walletAdminService` usa `apiFetch` (con interceptor JWT)
- [ ] Service verifica errores en `messages` antes de retornar `data`
- [ ] Hooks usan `useQuery` / `useMutation` de TanStack Query
- [ ] Componentes usan shadcn/ui (no HTML nativo para UI)
- [ ] `CambiarEstadoTransaccionDialog` solo muestra transiciones validas segun estado actual
- [ ] `TransaccionEstadoBadge` usa `ESTADO_WALLET_TRANSACCION_BADGES` de shared
- [ ] `TransaccionesTable` muestra `WalletEmptyState` cuando no hay resultados
- [ ] Types importados de `@shared/types` (no duplicados en admin)
- [ ] Constants importados de `@shared/constants`
- [ ] Todos los hooks nuevos exportados desde `src/admin/src/hooks/index.ts`
- [ ] `walletAdminService` exportado desde `src/admin/src/services/index.ts`
- [ ] Sidebar actualizado con nuevo item "Wallets"

### Patrones de consistencia con codebase existente
- [ ] Estructura de pagina sigue el patron `programas/` existente
- [ ] Componentes de lista siguen patron `src/admin/src/app/(dashboard)/crowdpromotion/components/list/`
- [ ] Componentes reutilizables en `src/admin/src/components/crowdpromotion/wallets/`
- [ ] Service sigue patron identico a `metricas.service.ts`
- [ ] Query hooks siguen patron de `use-programa-metricas.ts`
- [ ] Mutation hook sigue patron de `use-validar-tarea.ts` o `use-rechazar-tarea.ts`
