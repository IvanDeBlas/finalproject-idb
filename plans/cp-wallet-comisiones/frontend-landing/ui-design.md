# Diseno UI: cp-wallet-comisiones (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/web (Landing - Vite + React 18)
**Ruta principal:** `/promotor/wallet`

---

## 1. Resumen

- Componentes shadcn/ui utilizados: Card, Badge, Button, Dialog, Input, Textarea, Select, Separator, Alert, Skeleton, Pagination
- Composiciones custom: WalletSaldoCard, FiltrosHistorial, TransaccionItem, TransaccionEstadoBadge, TransaccionesList, TransaccionesListSkeleton, WalletEmptyState, WalletEmptyFiltered, WalletErrorState, PaginacionWallet, SolicitarCobroDialog
- Pagina contenedora: PromotorWalletPage
- Responsive breakpoints: mobile (< 640px), tablet (640–1024px), desktop (> 1024px)
- Patron visual: hereda de PromotorMetricasPage (MetricasKpiCard, EventosRecientesList)
- Layout: DashboardLayout (sidebar fijo + main content scrollable)

---

## 2. Paleta de Colores

Todos los tokens son inline Tailwind / valores CSS fijos. No hay variables CSS globales nuevas. Se reutilizan los tokens ya establecidos en US-CP-01 a US-CP-05.

| Rol | Valor hex / clase | Uso |
|-----|-------------------|-----|
| Fondo pagina | `#1a1a2e` | `bg-[#1a1a2e]` en el contenedor principal |
| Fondo sidebar | `#0d0d1a` | Existente en DashboardLayout |
| Fondo card | `#151525` | `bg-[#151525]` en todas las Card |
| Fondo card hover | `#1e1e38` | `hover:bg-[#1e1e38]` en filas |
| Fondo input | `#0f0f1f` | `bg-[#0f0f1f]` en Input, Select, Textarea |
| Fondo dialog | `#0f1729` | Default del componente Dialog existente |
| Borde default | `#334155` | `border-[#334155]` en cards, inputs |
| Borde foco | `#a855f7` | `focus:border-[#a855f7]` en inputs/selects |
| Borde error | `#ef4444` | `border-[#ef4444]` en inputs con error |
| Texto primario | `#ffffff` | `text-white` |
| Texto secundario | `#94a3b8` | `text-[#94a3b8]` |
| Texto muted | `#64748b` | `text-[#64748b]` |
| Texto label | `#cbd5e1` | `text-[#cbd5e1]` en labels de formulario |
| Texto link / accent | `#a855f7` | `text-[#a855f7]` |
| Saldo disponible (dorado) | `#f59e0b` | `text-[#f59e0b]` valor principal del saldo |
| Credito (verde) | `#10b981` | Importe +, icono ArrowUp, Total Ganado |
| Debito (rojo) | `#ef4444` | Importe -, icono ArrowDown |
| Boton primario | gradiente pink->purple | `bg-gradient-to-r from-pink-500 to-purple-600` |
| Estado Pendiente | `#f59e0b` texto, `bg-amber-950/50` fondo | Badge Pendiente |
| Estado Procesada | `#10b981` texto, `bg-green-950/50` fondo | Badge Procesada |
| Estado Pagada | `#3b82f6` texto, `bg-blue-950/50` fondo | Badge Pagada |
| Estado Cancelada | `#64748b` texto, `bg-slate-900/50` fondo | Badge Cancelada |

---

## 3. Estructura de Archivos

```
src/web/src/features/crowdpromotion/wallet/
  domain/
    index.ts
  application/
    hooks/
      useWallet.ts
      useWalletTransacciones.ts
      useSolicitarCobro.ts
  infrastructure/
    wallet.service.ts
  presentation/
    pages/
      PromotorWalletPage.tsx
    components/
      WalletSaldoCard.tsx
      FiltrosHistorial.tsx
      TransaccionItem.tsx
      TransaccionEstadoBadge.tsx
      TransaccionesList.tsx
      TransaccionesListSkeleton.tsx
      WalletEmptyState.tsx
      PaginacionWallet.tsx
      SolicitarCobroDialog.tsx
      index.ts
```

---

## 4. Pantalla 1: PromotorWalletPage (/promotor/wallet)

### 4.1 Layout General

```
┌──────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a], border-r border-[#334155], fixed)   │
│  [Logo]                                                          │
│  Mis programas                                                   │
│  Mis tareas                                                      │
│  Metricas                                                        │
│  Mi Wallet  [activo]   <-- item nuevo con icono <Wallet w-4 h-4> │
├──────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT (bg-[#1a1a2e], min-h-screen)                        │
│                                                                  │
│  ┌ max-w-3xl mx-auto px-4 sm:px-6 pt-8 pb-10 ──────────────┐    │
│  │                                                          │    │
│  │  h1: "Mi Wallet"  (text-xl sm:text-2xl font-bold)        │    │
│  │                                                          │    │
│  │  [WalletSaldoCard]      <- card de saldo y boton cobro   │    │
│  │                                                          │    │
│  │  h2: "Historial de transacciones"                        │    │
│  │  [FiltrosHistorial]     <- selects + date inputs         │    │
│  │                                                          │    │
│  │  [TransaccionesListSkeleton]  <- mientras isLoading      │    │
│  │  [WalletErrorState]           <- si isError              │    │
│  │  [WalletEmptyState]           <- si totalCount === 0     │    │
│  │  [WalletEmptyFiltered]        <- filtros sin resultados  │    │
│  │  [TransaccionesList]          <- lista con datos         │    │
│  │    TransaccionItem x n                                   │    │
│  │    [PaginacionWallet]                                    │    │
│  └──────────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────────┘
```

### 4.2 Composicion PromotorWalletPage

```tsx
<div className="min-h-screen bg-[#1a1a2e]">
    <div
        className="max-w-3xl mx-auto px-4 sm:px-6 pt-8 pb-10"
        role="main"
        aria-busy={isWalletLoading}
    >
        {/* Cabecera */}
        <h1 className="text-xl sm:text-2xl font-bold text-white mb-6">
            Mi Wallet
        </h1>

        {/* Saldo Card (siempre visible, en skeleton si carga) */}
        <WalletSaldoCard
            wallet={walletData}
            isLoading={isWalletLoading}
            isError={isWalletError}
            onSolicitarCobro={() => setDialogOpen(true)}
        />

        {/* Dialog de cobro (controlado por PromotorWalletPage) */}
        <SolicitarCobroDialog
            open={dialogOpen}
            onOpenChange={setDialogOpen}
            saldoDisponible={walletData?.saldoDisponible ?? 0}
            minimoRetiro={walletData?.minimoRetiro ?? 10}
            monedaNombre={walletData?.monedaNombre ?? 'EUR'}
            onSuccess={handleCobroSuccess}
        />

        {/* Historial - solo visible si wallet cargo OK */}
        {!isWalletError && (
            <>
                <h2 className="text-base font-semibold text-white mb-4">
                    Historial de transacciones
                </h2>

                {/* Filtros - ocultos en empty state sin filtros */}
                {(hasTransacciones || hayFiltrosActivos) && (
                    <FiltrosHistorial
                        filters={filters}
                        onFiltersChange={handleFiltersChange}
                        onClear={handleClearFilters}
                        hayFiltrosActivos={hayFiltrosActivos}
                    />
                )}

                {/* Estados condicionados */}
                {isTransaccionesLoading && <TransaccionesListSkeleton />}
                {isTransaccionesError && !isTransaccionesLoading && (
                    <WalletErrorState onRetry={refetchTransacciones} />
                )}
                {!isTransaccionesLoading && !isTransaccionesError && totalCount === 0 && !hayFiltrosActivos && (
                    <WalletEmptyState />
                )}
                {!isTransaccionesLoading && !isTransaccionesError && totalCount === 0 && hayFiltrosActivos && (
                    <WalletEmptyFiltered onClear={handleClearFilters} />
                )}
                {!isTransaccionesLoading && !isTransaccionesError && totalCount > 0 && (
                    <TransaccionesList
                        items={transacciones}
                        monedaNombre={walletData?.monedaNombre ?? 'EUR'}
                        totalCount={totalCount}
                        page={filters.page ?? 1}
                        pageSize={filters.pageSize ?? 10}
                        totalPages={totalPages}
                        onPageChange={handlePageChange}
                    />
                )}
            </>
        )}
    </div>
</div>
```

---

## 5. Componente WalletSaldoCard

### 5.1 Props Interface

```typescript
interface WalletSaldoCardProps {
    wallet: PromotorWallet | undefined
    isLoading: boolean
    isError: boolean
    onSolicitarCobro: () => void
}
```

### 5.2 Layout

```
┌──────────────────────────────────────────────────────────┐
│  [gradiente sutil en top-left de la card]                │
│                                                          │
│  SALDO DISPONIBLE                                        │
│  (text-sm text-[#94a3b8] uppercase tracking-wide)        │
│                                                          │
│  150.50  EUR                                             │
│  (text-4xl font-bold text-[#f59e0b]) (text-2xl muted)   │
│                                                          │
│  ─────────────────────────────────────────────────────   │
│                                                          │
│  ┌─────────────────────┐  ┌─────────────────────┐        │
│  │  Total Ganado        │  │  Total Retirado      │       │
│  │  200.00 EUR          │  │  49.50 EUR           │       │
│  │  (verde #10b981)     │  │  (gris #94a3b8)      │       │
│  └─────────────────────┘  └─────────────────────┘        │
│                                                          │
│  [Solicitar cobro]      Minimo: 10.00 EUR                │
│  [Info] Necesitas al menos 10.00 EUR  <- si insuficiente │
└──────────────────────────────────────────────────────────┘
```

### 5.3 Componentes shadcn utilizados

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Contenedor | `Card` | `bg-[#151525] border-[#334155] p-6 mb-6 relative overflow-hidden` |
| Efecto fondo | `div` decorativo | `absolute inset-0 bg-gradient-to-br from-green-950/20 via-transparent to-transparent pointer-events-none` |
| Label "SALDO DISPONIBLE" | `p` | `text-sm font-medium text-[#94a3b8] mb-1 uppercase tracking-wide` |
| Valor principal | `p` | `text-4xl font-bold text-[#f59e0b] tabular-nums mb-1` |
| Sufijo moneda | `span` | `text-2xl text-[#64748b] ml-2` |
| Separador horizontal | `Separator` | `bg-[#334155] my-5` |
| Grid secundario | `div` | `grid grid-cols-2 gap-4` |
| Label stat | `p` | `text-xs text-[#64748b] mb-1` |
| Valor Total Ganado | `p` | `text-lg font-semibold text-[#10b981] tabular-nums` |
| Valor Total Retirado | `p` | `text-lg font-semibold text-[#94a3b8] tabular-nums` |
| Sufijo moneda stat | `span` | `text-sm text-[#64748b] ml-1` |
| Fila accion | `div` | `flex items-center justify-between flex-wrap gap-3 mt-4` |
| Boton cobro habilitado | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-400 hover:to-purple-500 transition-all duration-150 font-medium` |
| Boton cobro deshabilitado | `Button` con `disabled` | `opacity-50 cursor-not-allowed bg-[#1e1e38] text-[#64748b] border border-[#334155]` |
| Texto minimo retiro | `p` | `text-xs text-[#64748b]` |
| Aviso saldo insuficiente | `div` | `flex items-center gap-1.5 text-xs text-[#f59e0b] mt-2` con icono `Info w-3.5 h-3.5` |

### 5.4 Composicion

```tsx
<Card className="bg-[#151525] border-[#334155] p-6 mb-6 relative overflow-hidden">
    {/* Efecto de fondo */}
    <div className="absolute inset-0 bg-gradient-to-br from-green-950/20 via-transparent to-transparent pointer-events-none" />

    <p className="text-sm font-medium text-[#94a3b8] mb-1 uppercase tracking-wide">
        Saldo disponible
    </p>
    <p className="text-4xl font-bold text-[#f59e0b] tabular-nums mb-1">
        {formatWalletImporte(wallet.saldoDisponible, wallet.monedaNombre)}
    </p>

    <Separator className="bg-[#334155] my-5" />

    <div className="grid grid-cols-2 gap-4">
        {/* Total Ganado */}
        <div>
            <p className="text-xs text-[#64748b] mb-1">Total Ganado</p>
            <p className="text-lg font-semibold text-[#10b981] tabular-nums">
                {formatWalletImporte(wallet.totalGanado, wallet.monedaNombre)}
            </p>
        </div>
        {/* Total Retirado */}
        <div>
            <p className="text-xs text-[#64748b] mb-1">Total Retirado</p>
            <p className="text-lg font-semibold text-[#94a3b8] tabular-nums">
                {formatWalletImporte(wallet.totalRetirado, wallet.monedaNombre)}
            </p>
        </div>
    </div>

    <div className="flex items-center justify-between flex-wrap gap-3 mt-4">
        <Button
            onClick={onSolicitarCobro}
            disabled={wallet.saldoDisponible < wallet.minimoRetiro}
            aria-describedby={saldoInsuficiente ? 'aviso-saldo-insuficiente' : undefined}
            className="bg-gradient-to-r from-pink-500 to-purple-600 text-white
                       hover:from-pink-400 hover:to-purple-500 transition-all duration-150 font-medium
                       disabled:opacity-50 disabled:cursor-not-allowed disabled:bg-[#1e1e38]
                       disabled:text-[#64748b] disabled:border disabled:border-[#334155]
                       focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
        >
            Solicitar cobro
        </Button>
        <p className="text-xs text-[#64748b]">
            Minimo: {formatWalletImporte(wallet.minimoRetiro, wallet.monedaNombre)}
        </p>
    </div>

    {saldoInsuficiente && (
        <div
            id="aviso-saldo-insuficiente"
            role="status"
            className="flex items-center gap-1.5 text-xs text-[#f59e0b] mt-2"
        >
            <Info className="w-3.5 h-3.5" />
            <span>
                Necesitas al menos {formatWalletImporte(wallet.minimoRetiro, wallet.monedaNombre)} para solicitar un cobro
            </span>
        </div>
    )}
</Card>
```

### 5.5 Skeleton de WalletSaldoCard

```tsx
{/* Estado loading - estructura identica al card real pero con Skeleton */}
<div className="bg-[#151525] border border-[#334155] rounded-lg p-6 mb-6">
    <Skeleton className="w-32 h-4 mb-3 bg-[#1e1e38] animate-pulse" />
    <Skeleton className="w-48 h-10 mb-4 bg-[#1e1e38] animate-pulse" />
    <div className="border-t border-[#334155] pt-4">
        <div className="grid grid-cols-2 gap-4">
            <div>
                <Skeleton className="w-20 h-4 mb-1 bg-[#1e1e38] animate-pulse" />
                <Skeleton className="w-28 h-6 bg-[#1e1e38] animate-pulse" />
            </div>
            <div>
                <Skeleton className="w-20 h-4 mb-1 bg-[#1e1e38] animate-pulse" />
                <Skeleton className="w-28 h-6 bg-[#1e1e38] animate-pulse" />
            </div>
        </div>
    </div>
    <div className="mt-4">
        <Skeleton className="w-36 h-9 rounded-lg bg-[#1e1e38] animate-pulse" />
    </div>
</div>
```

### 5.6 Estados

| Estado | Visual |
|--------|--------|
| Loading | Skeleton con `animate-pulse` (ver estructura arriba) |
| Error wallet | No se muestra WalletSaldoCard; se muestra WalletErrorState en su lugar |
| Saldo >= minimo | Boton "Solicitar cobro" con gradiente habilitado |
| Saldo < minimo | Boton deshabilitado con `opacity-50`, aviso amarillo con `<Info>` |
| Saldo 0 | Boton deshabilitado, aviso visible, stats muestran 0.00 |
| Entrada de animacion | `opacity-0 -> opacity-100` + `translateY(8px) -> 0`, 300ms ease-out |

---

## 6. Componente FiltrosHistorial

### 6.1 Props Interface

```typescript
interface FiltrosHistorialProps {
    filters: WalletTransaccionesFilters
    onFiltersChange: (filters: Partial<WalletTransaccionesFilters>) => void
    onClear: () => void
    hayFiltrosActivos: boolean
    isLoading?: boolean
}
```

### 6.2 Layout

```
[Tipo v]   [Estado v]   [Desde____]   [Hasta____]   [x Limpiar filtros]
(w-36)     (w-40)       (w-36)         (w-36)         (ghost, visible si activos)
```

### 6.3 Componentes shadcn utilizados

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Contenedor | `div` | `flex flex-wrap items-center gap-3 mb-4` |
| Select tipo | `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | Trigger: `w-36 bg-[#0f0f1f] border-[#334155] text-sm text-white focus:border-[#a855f7] focus:ring-[#a855f7]` |
| Select estado | `Select` + variantes | `w-40 bg-[#0f0f1f] border-[#334155] text-sm text-white focus:border-[#a855f7] focus:ring-[#a855f7]` |
| Input fecha desde | `Input` tipo `date` | `w-36 bg-[#0f0f1f] border-[#334155] text-white text-sm [color-scheme:dark] focus:border-[#a855f7]` |
| Input fecha hasta | `Input` tipo `date` | Mismos estilos |
| Input fecha con error | `Input` | Agrega `border-[#ef4444] focus:border-[#ef4444]` |
| Boton limpiar | `Button` variant `ghost` size `sm` | `text-[#64748b] hover:text-[#94a3b8] text-xs focus-visible:ring-[#a855f7]` con icono `X w-3.5 h-3.5 mr-1` |

### 6.4 Composicion

```tsx
<div>
    {/* Opciones del Select tipo */}
    {/* value: '' = todos, 'true' = creditos, 'false' = debitos */}
    <Select
        value={tipoValue}
        onValueChange={handleTipoChange}
    >
        <SelectTrigger
            className="w-36 bg-[#0f0f1f] border-[#334155] text-white text-sm
                       focus:border-[#a855f7] focus:ring-[#a855f7]
                       focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
            aria-label="Filtrar por tipo de transaccion"
        >
            <SelectValue placeholder="Tipo" />
        </SelectTrigger>
        <SelectContent>
            <SelectItem value="">Todos</SelectItem>
            <SelectItem value="true">Creditos</SelectItem>
            <SelectItem value="false">Debitos</SelectItem>
        </SelectContent>
    </Select>

    {/* Select estado */}
    <Select
        value={estadoValue}
        onValueChange={handleEstadoChange}
    >
        <SelectTrigger
            className="w-40 bg-[#0f0f1f] border-[#334155] text-white text-sm
                       focus:border-[#a855f7] focus:ring-[#a855f7]
                       focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
            aria-label="Filtrar por estado de transaccion"
        >
            <SelectValue placeholder="Estado" />
        </SelectTrigger>
        <SelectContent>
            <SelectItem value="">Todos los estados</SelectItem>
            <SelectItem value="1">Pendiente</SelectItem>
            <SelectItem value="2">Procesada</SelectItem>
            <SelectItem value="3">Pagada</SelectItem>
            <SelectItem value="4">Cancelada</SelectItem>
        </SelectContent>
    </Select>

    {/* Input fecha desde */}
    <Input
        type="date"
        id="fecha-desde"
        aria-label="Fecha desde"
        max={hoy}
        value={filters.fechaDesde ?? ''}
        onChange={handleFechaDesdeChange}
        className="w-36 bg-[#0f0f1f] border-[#334155] text-white text-sm
                   [color-scheme:dark] focus:border-[#a855f7] focus:ring-[#a855f7]
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
    />

    {/* Input fecha hasta */}
    <Input
        type="date"
        id="fecha-hasta"
        aria-label="Fecha hasta"
        max={hoy}
        min={filters.fechaDesde}
        value={filters.fechaHasta ?? ''}
        onChange={handleFechaHastaChange}
        className="w-36 bg-[#0f0f1f] border-[#334155] text-white text-sm
                   [color-scheme:dark] focus:border-[#a855f7] focus:ring-[#a855f7]
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
    />

    {/* Boton limpiar - visible solo con filtros activos */}
    {hayFiltrosActivos && (
        <Button
            variant="ghost"
            size="sm"
            onClick={onClear}
            className="text-[#64748b] hover:text-[#94a3b8] text-xs
                       focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
        >
            <X className="w-3.5 h-3.5 mr-1" />
            Limpiar filtros
        </Button>
    )}
</div>
```

### 6.5 Estados y Variantes

| Estado | Comportamiento |
|--------|----------------|
| Default | Select en "Todos", fechas vacias, boton limpiar oculto |
| Filtros activos | Boton "Limpiar filtros" visible |
| Error fecha | Input con `border-[#ef4444]`, mensaje de error `text-xs text-[#ef4444]` debajo |
| Loading | Selects e inputs presentes pero el listado de debajo muestra skeleton |

---

## 7. Componente TransaccionEstadoBadge

### 7.1 Props Interface

```typescript
interface TransaccionEstadoBadgeProps {
    estadoTransaccionId: number
    estadoTransaccionNombre: string
    className?: string
}
```

### 7.2 Variantes por Estado

| Estado ID | Nombre | Clases del Badge |
|-----------|--------|------------------|
| 1 | Pendiente | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs font-medium` |
| 2 | Procesada | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs font-medium` |
| 3 | Pagada | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs font-medium` |
| 4 | Cancelada | `bg-slate-900/50 text-[#64748b] border border-slate-700/50 text-xs font-medium` |

### 7.3 Composicion

```tsx
<Badge
    variant="outline"
    className={cn(ESTADO_BADGE_CLASSES[estadoTransaccionId], className)}
>
    {estadoTransaccionNombre}
</Badge>
```

El mapa `ESTADO_BADGE_CLASSES` es local al componente:
```typescript
const ESTADO_BADGE_CLASSES: Record<number, string> = {
    1: 'bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs font-medium',
    2: 'bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs font-medium',
    3: 'bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs font-medium',
    4: 'bg-slate-900/50 text-[#64748b] border border-slate-700/50 text-xs font-medium',
}
```

Fallback: `'secondary'` (estado desconocido usa estilo de badge secundario de shadcn).

---

## 8. Componente TransaccionItem

### 8.1 Props Interface

```typescript
interface TransaccionItemProps {
    transaccion: WalletTransaccionItem
    monedaNombre: string
}
```

### 8.2 Layout

```
┌──────────────────────────────────────────────────────────────────┐
│  [O]   Concepto de la transaccion          + 10.00 EUR           │
│  ArrowUp   tipo reward o contexto          [PROCESADA]           │
│  (verde)   20 mar 2026                                           │
└──────────────────────────────────────────────────────────────────┘

[O] = icono circular:
  credito -> bg-green-950/50 border-green-800/30 + ArrowUp text-[#10b981]
  debito  -> bg-red-950/50 border-red-800/30 + ArrowDown text-[#ef4444]
```

### 8.3 Componentes shadcn utilizados

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Contenedor fila | `li` / `div` | `flex items-start gap-3 px-5 py-4 hover:bg-[#1e1e38] transition-colors duration-150 cursor-default` |
| Icono credito | `div` | `w-8 h-8 rounded-full flex items-center justify-center shrink-0 bg-green-950/50 border border-green-800/30 mt-0.5` |
| Icono debito | `div` | `w-8 h-8 rounded-full flex items-center justify-center shrink-0 bg-red-950/50 border border-red-800/30 mt-0.5` |
| Icono ArrowUp | `ArrowUp` (lucide) | `w-4 h-4 text-[#10b981]` |
| Icono ArrowDown | `ArrowDown` (lucide) | `w-4 h-4 text-[#ef4444]` |
| Columna central | `div` | `flex-1 min-w-0` |
| Texto concepto | `p` | `text-sm font-medium text-white truncate` |
| Texto contexto | `p` | `text-xs text-[#64748b] mt-0.5 truncate hidden sm:block` |
| Fecha | `time` | `text-xs text-[#64748b] mt-1 block` con `dateTime={transaccion.fechaCreacion}` |
| Columna derecha | `div` | `flex flex-col items-end gap-1.5 shrink-0` |
| Importe credito | `p` | `text-sm font-semibold text-[#10b981] tabular-nums` |
| Importe debito | `p` | `text-sm font-semibold text-[#ef4444] tabular-nums` |
| Badge estado | `TransaccionEstadoBadge` | componente propio |

### 8.4 Composicion

```tsx
<li
    className="flex items-start gap-3 px-5 py-4 hover:bg-[#1e1e38] transition-colors duration-150 cursor-default"
    role="listitem"
>
    {/* Icono direccion */}
    <div
        className={cn(
            'w-8 h-8 rounded-full flex items-center justify-center shrink-0 mt-0.5',
            transaccion.esCredito
                ? 'bg-green-950/50 border border-green-800/30'
                : 'bg-red-950/50 border border-red-800/30'
        )}
        aria-hidden="true"
    >
        {transaccion.esCredito
            ? <ArrowUp className="w-4 h-4 text-[#10b981]" />
            : <ArrowDown className="w-4 h-4 text-[#ef4444]" />
        }
    </div>

    {/* Columna central */}
    <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-white truncate">
            {transaccion.descripcion ?? transaccion.concepto ?? 'Transaccion'}
        </p>
        {transaccion.tipoRewardNombre && (
            <p className="text-xs text-[#64748b] mt-0.5 truncate hidden sm:block">
                {transaccion.tipoRewardNombre}
            </p>
        )}
        <time
            dateTime={transaccion.fechaCreacion}
            className="text-xs text-[#64748b] mt-1 block"
        >
            {formatFechaTransaccion(transaccion.fechaCreacion)}
        </time>
    </div>

    {/* Columna derecha */}
    <div className="flex flex-col items-end gap-1.5 shrink-0">
        <p
            className={cn(
                'text-sm font-semibold tabular-nums',
                transaccion.esCredito ? 'text-[#10b981]' : 'text-[#ef4444]'
            )}
        >
            {formatTransaccionImporte(transaccion.importe, transaccion.esCredito, monedaNombre)}
        </p>
        <TransaccionEstadoBadge
            estadoTransaccionId={transaccion.estadoTransaccionId}
            estadoTransaccionNombre={transaccion.estadoTransaccionNombre}
        />
    </div>
</li>
```

### 8.5 Formato de Datos

| Campo | Formato |
|-------|---------|
| importe credito | `+150,50 EUR` - texto con signo, `text-[#10b981]` |
| importe debito | `-49,50 EUR` - texto con signo, `text-[#ef4444]` |
| fechaCreacion | `20 mar 2026` via `toLocaleDateString('es-ES', { day: 'numeric', month: 'short', year: 'numeric' })` |
| descripcion | Mostrar `descripcion` si no es null, fallback a `concepto`, fallback a "Transaccion" |
| contexto | Mostrar `tipoRewardNombre` si no es null. Oculto con `hidden sm:block` en mobile |

### 8.6 Animacion de Entrada

Las filas aparecen con fade-in escalonado al resolver la query:
- Item 0: `animation-delay: 0ms`
- Item 1: `animation-delay: 50ms`
- Item 2: `animation-delay: 100ms`
- etc. (incremento de 50ms por item, maximo 400ms)
- Duracion: 200ms ease-out por item

---

## 9. Componente TransaccionesList

### 9.1 Props Interface

```typescript
interface TransaccionesListProps {
    items: WalletTransaccionItem[]
    monedaNombre: string
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
    onPageChange: (page: number) => void
}
```

### 9.2 Composicion

```tsx
<Card className="bg-[#151525] border-[#334155]">
    <ul
        className="divide-y divide-[#1e1e38]"
        role="list"
        aria-label="Historial de transacciones"
    >
        {items.map((item, index) => (
            <TransaccionItem
                key={item.id}
                transaccion={item}
                monedaNombre={monedaNombre}
                style={{ animationDelay: `${Math.min(index * 50, 400)}ms` }}
                className="animate-fade-in-up"
            />
        ))}
    </ul>

    <PaginacionWallet
        totalCount={totalCount}
        page={page}
        pageSize={pageSize}
        totalPages={totalPages}
        onPageChange={onPageChange}
    />
</Card>
```

---

## 10. Componente TransaccionesListSkeleton

### 10.1 Composicion

```tsx
<div className="bg-[#151525] border border-[#334155] rounded-lg" aria-hidden="true">
    <div className="divide-y divide-[#334155]">
        {[0, 1, 2, 3, 4].map(i => (
            <div key={i} className="flex items-start gap-3 px-5 py-4">
                {/* Icono */}
                <Skeleton className="w-8 h-8 rounded-full bg-[#1e1e38] shrink-0 animate-pulse" />
                {/* Columna central */}
                <div className="flex-1">
                    <Skeleton className="w-3/4 h-4 mb-2 bg-[#1e1e38] animate-pulse" />
                    <Skeleton className="w-1/2 h-3 bg-[#1e1e38] animate-pulse" />
                </div>
                {/* Columna derecha */}
                <div className="flex flex-col items-end gap-2">
                    <Skeleton className="w-20 h-4 bg-[#1e1e38] animate-pulse" />
                    <Skeleton className="w-16 h-5 rounded-full bg-[#1e1e38] animate-pulse" />
                </div>
            </div>
        ))}
    </div>
</div>
```

---

## 11. Componente PaginacionWallet

### 11.1 Props Interface

```typescript
interface PaginacionWalletProps {
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
    onPageChange: (page: number) => void
}
```

### 11.2 Layout

```
─────────────────────────────────────────────────
 Mostrando 1-10 de 25 transacciones    < [1] [2] [3] >
```

### 11.3 Componentes shadcn utilizados

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|------------------------|
| Contenedor | `div` | `flex items-center justify-between px-5 py-4 border-t border-[#334155]` |
| Texto informativo | `p` | `text-xs text-[#64748b]` |
| Paginacion raiz | `Pagination` | - |
| Lista de paginas | `PaginationContent` | - |
| Item pagina | `PaginationItem` | - |
| Link pagina activa | `PaginationLink` con `isActive` | `bg-[#a855f7] text-white border-[#a855f7]` |
| Link pagina inactiva | `PaginationLink` | `bg-transparent text-[#94a3b8] border-[#334155] hover:bg-[#1e1e38] hover:text-white` |
| Boton anterior | `PaginationPrevious` | Deshabilitado con `opacity-40 pointer-events-none` en pagina 1 |
| Boton siguiente | `PaginationNext` | Deshabilitado con `opacity-40 pointer-events-none` en ultima pagina |
| Ellipsis | `PaginationEllipsis` | Cuando hay mas de 5 paginas |

### 11.4 Composicion

```tsx
<div className="flex items-center justify-between px-5 py-4 border-t border-[#334155]">
    <p className="text-xs text-[#64748b]">
        Mostrando {inicio}-{fin} de {totalCount} transacciones
    </p>
    <Pagination>
        <PaginationContent>
            <PaginationItem>
                <PaginationPrevious
                    onClick={() => onPageChange(page - 1)}
                    aria-label="Pagina anterior"
                    className={cn(
                        'bg-transparent text-[#94a3b8] border-[#334155]',
                        'hover:bg-[#1e1e38] hover:text-white',
                        'focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]',
                        page === 1 && 'opacity-40 pointer-events-none'
                    )}
                />
            </PaginationItem>
            {/* paginas visibles (max 5) con ellipsis */}
            {paginasVisibles.map((p, i) =>
                p === '...' ? (
                    <PaginationItem key={`ellipsis-${i}`}>
                        <PaginationEllipsis />
                    </PaginationItem>
                ) : (
                    <PaginationItem key={p}>
                        <PaginationLink
                            isActive={p === page}
                            onClick={() => onPageChange(p)}
                            aria-label={`Ir a la pagina ${p}`}
                            aria-current={p === page ? 'page' : undefined}
                            className={cn(
                                p === page
                                    ? 'bg-[#a855f7] text-white border-[#a855f7]'
                                    : 'bg-transparent text-[#94a3b8] border-[#334155] hover:bg-[#1e1e38] hover:text-white',
                                'focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]'
                            )}
                        >
                            {p}
                        </PaginationLink>
                    </PaginationItem>
                )
            )}
            <PaginationItem>
                <PaginationNext
                    onClick={() => onPageChange(page + 1)}
                    aria-label="Pagina siguiente"
                    className={cn(
                        'bg-transparent text-[#94a3b8] border-[#334155]',
                        'hover:bg-[#1e1e38] hover:text-white',
                        'focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]',
                        page === totalPages && 'opacity-40 pointer-events-none'
                    )}
                />
            </PaginationItem>
        </PaginationContent>
    </Pagination>
</div>
```

---

## 12. Componente SolicitarCobroDialog

### 12.1 Props Interface

```typescript
interface SolicitarCobroDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    saldoDisponible: number
    minimoRetiro: number
    monedaNombre: string
    onSuccess: (response: SolicitarCobroResponse) => void
}
```

### 12.2 Layout

```
┌──────────────────────────────────────────────┐
│  Solicitar cobro                        [X]  │
├──────────────────────────────────────────────┤
│                                              │
│  ┌──────────────────────────────────────┐    │
│  │  Tu saldo disponible                 │    │
│  │  150.50 EUR                          │    │
│  │  Minimo de retiro: 10.00 EUR         │    │
│  └──────────────────────────────────────┘    │
│                                              │
│  ────────────────────────────────────────    │
│                                              │
│  Importe a retirar *                         │
│  ┌──────────────────────────────┐ EUR        │
│  │  50.00                       │            │
│  └──────────────────────────────┘            │
│  Hasta 150.50 EUR disponibles                │
│  [AlertCircle] Mensaje de error              │  <- si invalido
│                                              │
│  Descripcion (opcional)                      │
│  ┌──────────────────────────────────────┐    │
│  │                                      │    │
│  │                                      │    │
│  └──────────────────────────────────────┘    │
│  0 / 500 caracteres                          │
│                                              │
│  [Alert destructive si error de API]         │
│                                              │
│  ┌───────────────┐  ┌───────────────────┐    │
│  │   Cancelar    │  │  Solicitar cobro  │    │
│  └───────────────┘  └───────────────────┘    │
└──────────────────────────────────────────────┘
```

### 12.3 Componentes shadcn utilizados

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|------------------------|
| Dialog raiz | `Dialog` | `open={open} onOpenChange={onOpenChange}` |
| Contenido | `DialogContent` | `max-w-md` (usa estilos del componente: `bg-[#0f1729] border-[#334155]`) |
| Titulo | `DialogTitle` | `text-lg font-semibold text-white` |
| Seccion saldo | `div` | `bg-[#151525] border border-[#334155] rounded-lg p-4 mb-5` |
| Label saldo | `p` | `text-xs text-[#64748b] mb-1` |
| Valor saldo | `p` | `text-2xl font-bold text-[#f59e0b] tabular-nums` |
| Texto minimo | `p` | `text-xs text-[#64748b] mt-2` |
| Separador | `Separator` | `bg-[#334155] my-4` |
| Form | `Form` (react-hook-form) | - |
| Label importe | `FormLabel` | `text-sm font-medium text-[#cbd5e1]` |
| Asterisco requerido | `span` | `text-red-400 ml-1` |
| Input importe | `Input` con `type="number"` | `bg-[#0f0f1f] border-[#334155] text-white pr-14 focus:border-[#a855f7] focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]` |
| Sufijo "EUR" | `span` | `absolute right-3 top-1/2 -translate-y-1/2 text-sm text-[#64748b] pointer-events-none` |
| Hint disponible | `p` | `text-xs text-[#64748b] mt-1` |
| Error importe | `FormMessage` / `p role="alert"` | `text-xs text-[#ef4444] mt-1 flex items-center gap-1` + `AlertCircle w-3 h-3` |
| Label descripcion | `FormLabel` | `text-sm font-medium text-[#cbd5e1] mt-4 block` |
| Textarea descripcion | `Textarea` | `bg-[#0f0f1f] border-[#334155] text-white resize-none h-20 focus:border-[#a855f7] placeholder:text-[#64748b] focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]` |
| Contador chars | `p` | `text-xs text-right mt-1` - color cambia segun cantidad |
| Alert error API | `Alert variant="destructive"` | Con `AlertTitle` y `AlertDescription` |
| Footer | `DialogFooter` | `flex items-center justify-end gap-3 mt-5` |
| Boton cancelar | `Button variant="outline"` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white focus-visible:ring-[#a855f7]` |
| Boton submit normal | `Button type="submit"` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-400 hover:to-purple-500` |
| Boton submit loading | `Button type="submit" disabled` | Spinner `Loader2 w-4 h-4 animate-spin mr-2` + "Procesando..." |

### 12.4 Composicion

```tsx
<Dialog open={open} onOpenChange={handleOpenChange}>
    <DialogContent className="max-w-md">
        <DialogHeader>
            <DialogTitle className="text-lg font-semibold text-white">
                Solicitar cobro
            </DialogTitle>
        </DialogHeader>

        {/* Informacion de saldo */}
        <div className="bg-[#151525] border border-[#334155] rounded-lg p-4 mb-5">
            <p className="text-xs text-[#64748b] mb-1">Tu saldo disponible</p>
            <p className="text-2xl font-bold text-[#f59e0b] tabular-nums">
                {formatWalletImporte(saldoDisponible, monedaNombre)}
            </p>
            <p className="text-xs text-[#64748b] mt-2">
                Minimo de retiro: {formatWalletImporte(minimoRetiro, monedaNombre)}
            </p>
        </div>

        <Separator className="bg-[#334155] my-4" />

        {/* Formulario con react-hook-form + Zod */}
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
                {/* Campo importe */}
                <FormField
                    control={form.control}
                    name="importe"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="text-sm font-medium text-[#cbd5e1]">
                                Importe a retirar
                                <span className="text-red-400 ml-1" aria-hidden="true">*</span>
                            </FormLabel>
                            <FormControl>
                                <div className="relative">
                                    <Input
                                        {...field}
                                        id="importe"
                                        type="number"
                                        min="0.01"
                                        step="0.01"
                                        autoFocus
                                        disabled={isSubmitting}
                                        placeholder="0.00"
                                        className="bg-[#0f0f1f] border-[#334155] text-white pr-14
                                                   focus:border-[#a855f7] focus-visible:ring-[#a855f7]
                                                   focus-visible:ring-offset-[#1a1a2e]"
                                        aria-describedby="importe-hint importe-error"
                                    />
                                    <span className="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-[#64748b] pointer-events-none">
                                        {monedaNombre}
                                    </span>
                                </div>
                            </FormControl>
                            <p id="importe-hint" className="text-xs text-[#64748b] mt-1">
                                Hasta {formatWalletImporte(saldoDisponible, monedaNombre)} disponibles
                            </p>
                            <FormMessage
                                id="importe-error"
                                className="text-xs text-[#ef4444] flex items-center gap-1"
                            />
                        </FormItem>
                    )}
                />

                {/* Campo descripcion */}
                <FormField
                    control={form.control}
                    name="descripcion"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel className="text-sm font-medium text-[#cbd5e1]">
                                Descripcion (opcional)
                            </FormLabel>
                            <FormControl>
                                <Textarea
                                    {...field}
                                    id="descripcion"
                                    disabled={isSubmitting}
                                    placeholder="Ej: Retiro mensual"
                                    className="bg-[#0f0f1f] border-[#334155] text-white resize-none h-20
                                               focus:border-[#a855f7] placeholder:text-[#64748b]
                                               focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                                    maxLength={500}
                                    aria-describedby="descripcion-counter"
                                />
                            </FormControl>
                            <p
                                id="descripcion-counter"
                                className={cn(
                                    'text-xs text-right mt-1',
                                    charCount > 400 && charCount < 500 && 'text-[#f59e0b]',
                                    charCount >= 500 ? 'text-[#ef4444]' : 'text-[#64748b]'
                                )}
                            >
                                {charCount} / 500 caracteres
                            </p>
                            <FormMessage className="text-xs text-[#ef4444]" />
                        </FormItem>
                    )}
                />

                {/* Error de API */}
                {apiError && (
                    <Alert variant="destructive" role="alert">
                        <AlertCircle className="w-4 h-4" />
                        <AlertTitle>Error al procesar la solicitud</AlertTitle>
                        <AlertDescription>{apiError}</AlertDescription>
                    </Alert>
                )}

                <DialogFooter className="flex items-center justify-end gap-3 mt-5">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={handleCancel}
                        disabled={isSubmitting}
                        className="border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white
                                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                    >
                        Cancelar
                    </Button>
                    <Button
                        type="submit"
                        disabled={isSubmitting || !form.formState.isValid}
                        className="bg-gradient-to-r from-pink-500 to-purple-600 text-white
                                   hover:from-pink-400 hover:to-purple-500 transition-all duration-150
                                   disabled:opacity-50 disabled:cursor-not-allowed
                                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                    >
                        {isSubmitting ? (
                            <>
                                <Loader2 className="w-4 h-4 animate-spin mr-2" />
                                Procesando...
                            </>
                        ) : (
                            'Solicitar cobro'
                        )}
                    </Button>
                </DialogFooter>
            </form>
        </Form>
    </DialogContent>
</Dialog>
```

### 12.5 Estados del Dialog

| Estado | Comportamiento Visual |
|--------|-----------------------|
| Default | Importe vacio con foco, descripcion vacia, boton submit deshabilitado |
| Importe valido | Boton submit habilitado con gradiente |
| Importe invalido | `FormMessage` en rojo, boton deshabilitado con `opacity-50` |
| Importe > saldo | Error "El importe no puede superar tu saldo disponible (X EUR)" |
| Loading (submit) | Spinner en boton, "Procesando...", todos los inputs disabled, dialog no cierra al click fuera |
| Error API | `Alert variant="destructive"` visible dentro del dialog, dialog permanece abierto |
| Exito | Dialog cierra, toast success, refetch wallet y transacciones en padre |
| Contador descripcion | `text-[#64748b]` hasta 400, `text-[#f59e0b]` de 401-499, `text-[#ef4444]` en 500 |

### 12.6 Validacion React Hook Form + Zod

```typescript
// Schema base del contrato shared
const schemaConSaldo = solicitarCobroSchema.refine(
    data => data.importe <= saldoDisponible,
    {
        message: `El importe no puede superar tu saldo disponible (${saldoDisponible.toFixed(2)} EUR)`,
        path: ['importe'],
    }
)

const form = useForm<SolicitarCobroFormData>({
    resolver: zodResolver(schemaConSaldo),
    defaultValues: { importe: undefined, descripcion: '' },
    mode: 'onChange',
})
```

---

## 13. Componente WalletEmptyState

### 13.1 Props Interface

```typescript
interface WalletEmptyStateProps {
    // sin props - navega a ruta fija
}
```

### 13.2 Composicion

```tsx
<div className="flex flex-col items-center justify-center py-20 text-center">
    <div className="w-16 h-16 rounded-full flex items-center justify-center mb-4 bg-[#151525] border border-[#334155]">
        <Wallet className="w-7 h-7 text-[#64748b]" />
    </div>
    <h3 className="text-base font-semibold text-white mb-2">
        Aun no tienes transacciones
    </h3>
    <p className="text-sm text-[#64748b] max-w-sm">
        Completa tareas de promocion o genera backings referidos para ganar comisiones.
    </p>
    <Button
        asChild
        className="mt-6 bg-gradient-to-r from-pink-500 to-purple-600 text-white
                   hover:from-pink-400 hover:to-purple-500
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
        aria-label="Explorar programas de promocion disponibles"
    >
        <Link to="/crowdpromotion/explorar">Explorar programas</Link>
    </Button>
</div>
```

### 13.3 WalletEmptyFiltered (variante sin resultados con filtros activos)

```tsx
<div className="flex flex-col items-center justify-center py-12 text-center">
    <SearchX className="w-8 h-8 text-[#64748b] mb-3" />
    <p className="text-sm text-[#64748b]">
        No hay transacciones que coincidan con los filtros seleccionados.
    </p>
    <Button
        variant="ghost"
        size="sm"
        onClick={onClear}
        className="text-[#a855f7] hover:text-[#c084fc] mt-2
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
    >
        Limpiar filtros
    </Button>
</div>
```

---

## 14. Componente WalletErrorState

### 14.1 Props Interface

```typescript
interface WalletErrorStateProps {
    onRetry: () => void
    message?: string
}
```

### 14.2 Composicion

```tsx
<div
    className="flex flex-col items-center justify-center py-16 text-center"
    role="alert"
>
    <AlertCircle className="w-10 h-10 text-red-400 mb-4" />
    <h2 className="text-lg font-semibold text-white mb-2">
        {message ?? 'No se pudo cargar tu wallet'}
    </h2>
    <p className="text-sm text-[#94a3b8] mb-6">
        Ocurrio un error al obtener tus datos. Intenta de nuevo.
    </p>
    <Button
        variant="outline"
        size="sm"
        onClick={onRetry}
        className="border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-[#a855f7]
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
    >
        Reintentar
    </Button>
</div>
```

---

## 15. Responsive Design

### 15.1 Breakpoints y Cambios

| Breakpoint | Width | Componente | Cambio |
|------------|-------|------------|--------|
| Mobile | < 640px | `WalletSaldoCard` | `text-3xl` en saldo (en vez de `text-4xl`), `grid-cols-1` para stats, boton "Solicitar cobro" `w-full`, texto minimo centrado debajo |
| Mobile | < 640px | `FiltrosHistorial` | `flex-col`, cada Select `w-full`, fechas en `grid grid-cols-2 gap-2`, boton limpiar `w-full` |
| Mobile | < 640px | `TransaccionItem` | `py-3` (menos padding), texto contexto secundario `hidden`, fecha debajo del concepto, importe y badge en columna vertical |
| Mobile | < 640px | `SolicitarCobroDialog` | `max-w-full` con bordes al ras (via `sm:rounded-lg` del componente base) |
| Tablet | 640–1024px | General | Sidebar visible `w-48`, stats en 2 cols, dialog `max-w-md` |
| Desktop | > 1024px | General | Sidebar `w-64`, contenido `max-w-3xl mx-auto`, dialog `max-w-md` |

### 15.2 Clases Tailwind por Componente (Mobile-First)

**WalletSaldoCard:**
```tsx
// Valor saldo
className="text-3xl sm:text-4xl font-bold text-[#f59e0b] tabular-nums mb-1"

// Grid stats
className="grid grid-cols-1 sm:grid-cols-2 gap-4"

// Fila accion
className="flex flex-col sm:flex-row sm:items-center gap-3 mt-4"

// Boton cobro
className="w-full sm:w-auto bg-gradient-to-r from-pink-500 to-purple-600 ..."
```

**FiltrosHistorial:**
```tsx
// Contenedor
className="flex flex-col sm:flex-wrap sm:flex-row items-stretch sm:items-center gap-3 mb-4"

// Selects
className="w-full sm:w-36 bg-[#0f0f1f] ..."  // tipo
className="w-full sm:w-40 bg-[#0f0f1f] ..."  // estado

// Fechas
className="grid grid-cols-2 gap-2 sm:contents"  // wrapper de las dos fechas en mobile

// Inputs fecha
className="w-full sm:w-36 ..."

// Boton limpiar
className="w-full sm:w-auto text-[#64748b] ..."
```

**TransaccionItem:**
```tsx
// Texto contexto
className="text-xs text-[#64748b] mt-0.5 truncate hidden sm:block"

// Columna derecha
className="flex flex-col sm:flex-row items-end sm:items-center gap-1.5 shrink-0"
```

---

## 16. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Labels de formulario | `FormLabel` con `htmlFor` en todos los inputs del Dialog. Selects de filtros con `aria-label` descriptivo |
| Errores de formulario | `FormMessage` con `id` referenciado por `aria-describedby` en el Input correspondiente. `role="alert"` en mensajes de error de API |
| Focus visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en todos los elementos interactivos |
| Contraste | Saldo `#f59e0b` sobre `#151525`: ratio ~5.5:1 cumple AA. Badges de estado: texto coloreado sobre fondos oscuros cumplen 3:1 minimo |
| Dialog accesible | Radix UI (`Dialog` de shadcn) gestiona focus-trap, `aria-modal="true"`, retorno de foco al trigger al cerrar. `DialogTitle` visible siempre |
| Estados de carga | `aria-busy="true"` en el contenedor principal durante la carga inicial. Skeletons con `aria-hidden="true"` |
| Boton deshabilitado | `disabled` semantico en boton "Solicitar cobro" cuando saldo insuficiente. Aviso de saldo asociado via `aria-describedby="aviso-saldo-insuficiente"` |
| Lista de transacciones | `role="list"` en el `ul` contenedor, `role="listitem"` en cada `li`. Etiqueta `aria-label="Historial de transacciones"` |
| Fechas | `<time dateTime={transaccion.fechaCreacion}>` en cada fila de transaccion |
| Importe con signo | El signo `+` / `-` es texto explicito antes del numero. No se depende solo del color; el icono `ArrowUp` / `ArrowDown` complementa con `aria-hidden="true"` |
| Badges de estado | Texto visible en el badge (no solo color). Leibles por lectores de pantalla |
| Paginacion | `aria-label="Ir a la pagina N"` en cada boton. `aria-current="page"` en la pagina activa. `aria-label="Pagina anterior"` y `aria-label="Pagina siguiente"` |
| Importe obligatorio | `aria-required="true"` en el input de importe del dialog. Asterisco con `aria-hidden="true"` |
| Toast | Notificaciones via `sonner` con semantica de `role="status"` para exito y `role="alert"` para errores |

---

## 17. Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| `WalletSaldoCard` al cargar | `opacity-0 -> opacity-100` + `translateY(8px) -> 0` | 300ms ease-out | Query wallet resuelve |
| `TransaccionItem` al cargar lista | Fade-in escalonado: `animation-delay` incremental de 50ms por item | 200ms ease-out por item | Query transacciones resuelve |
| Dialog open | `zoom-in-95` + `fade-in-0` (comportamiento default shadcn Radix) | 200ms | Click en trigger |
| Dialog close | `zoom-out-95` + `fade-out-0` (comportamiento default shadcn Radix) | 200ms | Cancelar / Escape / click fuera |
| Hover `TransaccionItem` | `background-color` transition | 150ms | Hover del mouse |
| Boton "Solicitar cobro" hover | Gradiente aclara tonos con `transition-all` | 150ms | Hover |
| Skeleton | `animate-pulse` | 1.5s infinite | Durante carga |
| Contador descripcion | Color cambia instantaneo al superar umbrales (400, 500 chars) | - | `onChange` textarea |

---

## 18. Iconos Lucide Utilizados

| Icono | Uso | Tamano |
|-------|-----|--------|
| `Wallet` | Item sidebar "Mi Wallet", empty state icono | `w-4 h-4` sidebar, `w-7 h-7` empty |
| `ArrowUp` | Icono de transaccion credito | `w-4 h-4` |
| `ArrowDown` | Icono de transaccion debito | `w-4 h-4` |
| `Info` | Aviso saldo insuficiente | `w-3.5 h-3.5` |
| `X` | Boton "Limpiar filtros" | `w-3.5 h-3.5` |
| `AlertCircle` | Error de formulario, estado error de pagina | `w-3 h-3` inline, `w-10 h-10` pagina |
| `SearchX` | Empty state con filtros sin resultados | `w-8 h-8` |
| `Loader2` | Spinner en boton submit del dialog | `w-4 h-4 animate-spin` |

---

## 19. Componentes shadcn Utilizados

| Componente | Archivo shadcn | Uso |
|-----------|----------------|-----|
| `Card` | `card` | Contenedor WalletSaldoCard, TransaccionesList |
| `Badge` | `badge` | `TransaccionEstadoBadge` con variante `outline` |
| `Button` | `button` | Solicitar cobro, Cancelar, Limpiar filtros, CTA empty states, Reintentar |
| `Dialog` / `DialogContent` / `DialogHeader` / `DialogTitle` / `DialogFooter` | `dialog` | `SolicitarCobroDialog` |
| `Form` / `FormField` / `FormItem` / `FormLabel` / `FormControl` / `FormMessage` | `form` | Formulario de cobro |
| `Input` | `input` | Campo importe (number), inputs de fecha en filtros |
| `Textarea` | `textarea` | Campo descripcion del cobro |
| `Select` / `SelectTrigger` / `SelectContent` / `SelectItem` / `SelectValue` | `select` | Filtros tipo y estado |
| `Separator` | `separator` | Linea divisora en el dialog |
| `Skeleton` | `skeleton` | Skeletons de carga en WalletSaldoCard y TransaccionesListSkeleton |
| `Alert` / `AlertTitle` / `AlertDescription` | `alert` | Error de API dentro del dialog |
| `Pagination` / `PaginationContent` / `PaginationItem` / `PaginationLink` / `PaginationPrevious` / `PaginationNext` / `PaginationEllipsis` | `pagination` | PaginacionWallet |

---

## 20. Checklist de Diseno UI

### Componentes
- [ ] `WalletSaldoCard` con saldo en `text-3xl sm:text-4xl font-bold text-[#f59e0b]`
- [ ] Stats secundarias (Total Ganado verde, Total Retirado gris) en grid
- [ ] Boton "Solicitar cobro" habilitado/deshabilitado segun `saldo >= minimoRetiro`
- [ ] Aviso saldo insuficiente con `<Info>` en amarillo, vinculado por `aria-describedby`
- [ ] `FiltrosHistorial` con Select tipo (Todos/Creditos/Debitos), Select estado (5 opciones), inputs fecha
- [ ] Boton "Limpiar filtros" visible solo cuando hay filtros activos
- [ ] `TransaccionItem` con icono circular coloreado (verde credito, rojo debito)
- [ ] Importe con signo `+` / `-` y color correspondiente
- [ ] `TransaccionEstadoBadge` con 4 variantes de color (amarillo, verde, azul, gris)
- [ ] Fecha formateada en espanol con `<time dateTime>`
- [ ] `WalletEmptyState` con CTA a `/crowdpromotion/explorar`
- [ ] `WalletEmptyFiltered` con boton "Limpiar filtros"
- [ ] `WalletErrorState` con `role="alert"` y boton "Reintentar"
- [ ] `WalletSaldoCard` skeleton con `animate-pulse` (5 skeletons)
- [ ] `TransaccionesListSkeleton` con 5 filas
- [ ] `PaginacionWallet` con texto "Mostrando X-Y de Z", pagina activa en purple
- [ ] `SolicitarCobroDialog` con saldo visible, input importe con sufijo EUR, textarea descripcion con contador
- [ ] Contador de caracteres en textarea: gris -> amarillo (>400) -> rojo (500)
- [ ] Spinner `Loader2 animate-spin` en boton submit durante submit
- [ ] `Alert variant="destructive"` para error de API dentro del dialog

### Responsivo
- [ ] `WalletSaldoCard`: `grid-cols-1 sm:grid-cols-2` en stats, boton `w-full sm:w-auto`
- [ ] `FiltrosHistorial`: `flex-col sm:flex-row`, selects `w-full sm:w-36/40`
- [ ] `TransaccionItem`: contexto secundario `hidden sm:block`

### Accesibilidad
- [ ] Focus ring visible en todos los elementos interactivos (`ring-[#a855f7]`)
- [ ] `aria-busy="true"` durante carga inicial en el contenedor principal
- [ ] `role="alert"` en mensajes de error de pagina y dialog
- [ ] `disabled` semantico en boton con saldo insuficiente
- [ ] `aria-describedby` en boton deshabilitado apuntando al aviso de saldo
- [ ] `role="list"` + `aria-label` en el contenedor de transacciones
- [ ] `<time dateTime>` en fechas de transacciones
- [ ] Signos `+` / `-` como texto explicito (no solo color)
- [ ] `aria-current="page"` en pagina activa de paginacion
- [ ] `aria-label` descriptivo en botones de paginacion
- [ ] `DialogTitle` visible en el dialog
- [ ] Labels con `htmlFor` en todos los inputs del dialog
