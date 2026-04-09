# Diseno UI: cp-wallet-comisiones (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06)
**Target:** src/admin

---

> ## FUERA DE SCOPE MVP — Plan Preparatorio
>
> Este documento es un plan de diseno UI **preparatorio**. El modulo Admin NO implementa ninguna
> vista de wallet en el MVP (US-CP-06). Segun la feature-spec y el documento ui-ux.md:
>
> > "Sin responsabilidad en MVP. Futuro: vista de administrador para ver wallets de promotores
> > y marcar transacciones como Procesada/Pagada/Cancelada."
>
> La gestion de transacciones en MVP se realiza directamente en base de datos.
> Este plan documenta la arquitectura de componentes UI que se necesitaria en una iteracion futura.

---

## 1. Resumen

- Componentes shadcn/ui: 18 distintos
- Composiciones custom: 7
- Pantallas futuras: 3 (Lista wallets, Detalle promotor, Procesador de cobros)
- Responsive breakpoints: md (768px), lg (1024px)
- Reutilizacion desde Landing: `TransaccionEstadoBadge` y mappers de `@shared/utils`

### Alcance Futuro Planificado

| Pantalla | Ruta Admin | Descripcion |
|----------|------------|-------------|
| Lista de wallets | `/admin/crowdpromotion/wallets` | Todos los promotores con saldo y estado |
| Detalle de wallet | `/admin/crowdpromotion/wallets/[promotorId]` | Historial completo + cambio de estado |
| Procesador de cobros | `/admin/crowdpromotion/cobros/pendientes` | Cola de transacciones pendientes |

---

## 2. Paleta de Colores

El admin utiliza el design system dark-theme del proyecto. Los tokens son identicos a los de la
landing (src/web), ya que ambas apps comparten el mismo lenguaje visual.

| Uso | Valor | Ejemplo de uso |
|-----|-------|----------------|
| Fondo de pagina | `bg-[#1a1a2e]` | Contenido principal |
| Fondo de cards | `bg-[#151525]` | Todas las Cards de datos |
| Fondo de hover | `bg-[#1e1e38]` | Filas de tabla hover |
| Fondo de inputs | `bg-[#0f0f1f]` | Inputs y selects en filtros |
| Borde default | `border-zinc-800` | Cards y tablas |
| Borde enfocado | `border-[#a855f7]` | Focus en inputs |
| Texto primario | `text-white` | Valores principales |
| Texto secundario | `text-[#94a3b8]` | Labels y metadatos |
| Texto muted | `text-[#64748b]` | Hints y fechas |
| Accion primaria | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones CTA |
| Saldo disponible | `text-[#f59e0b]` | Valor de saldo destacado |
| Credito / ingreso | `text-[#10b981]` | Importes positivos |
| Debito / retiro | `text-[#ef4444]` | Importes negativos |

### Badges de Estado de Transaccion

Reutilizados directamente de `ESTADO_WALLET_TRANSACCION_BADGES` en `@shared/constants`:

| Estado | ID | Clases Tailwind |
|--------|----|-----------------|
| Pendiente | 1 | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50` |
| Procesada | 2 | `bg-green-950/50 text-[#10b981] border border-green-800/50` |
| Pagada | 3 | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50` |
| Cancelada | 4 | `bg-slate-900/50 text-[#64748b] border border-slate-700/50` |

---

## 3. Componentes por Pantalla

### 3.1 Pantalla: Lista de Wallets

**Ruta futura:** `/admin/crowdpromotion/wallets`
**Proposito:** Vista centralizada de todos los wallets de promotores con sus saldos.

#### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR Admin (bg-[#0d0d1a])                                        │
├─────────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT (bg-[#1a1a2e])                                         │
│                                                                     │
│  CABECERA                                                           │
│  Wallets de Promotores                    [Exportar CSV]            │
│                                                                     │
│  KPI SUMMARY CARDS (grid 4 columnas)                                │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌───────────┐  │
│  │ Total wallets│ │Saldo total   │ │Pendientes    │ │Pagados    │  │
│  │ activos      │ │en plataforma │ │de procesar   │ │este mes   │  │
│  └──────────────┘ └──────────────┘ └──────────────┘ └───────────┘  │
│                                                                     │
│  FILTROS Y BUSQUEDA                                                 │
│  [Buscar promotor...____] [Saldo min ___] [Saldo max ___] [Limpiar] │
│                                                                     │
│  TABLA DE WALLETS                                                   │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ Promotor       │ Email         │ Saldo disp. │ Pendiente │ Acc│   │
│  ├──────────────────────────────────────────────────────────────┤   │
│  │ [Avatar] Juan  │ juan@mail.com │ 150.50 EUR  │ 0.00 EUR  │ [>]│   │
│  │ [Avatar] Maria │ maria@mail.com│  22.00 EUR  │ 50.00 EUR │ [>]│   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  < [1] [2] [3] >  Mostrando 1-20 de 45 promotores                  │
└─────────────────────────────────────────────────────────────────────┘
```

#### Componentes

**WalletsResumenKpiGrid (nuevo, patron identico a KpiCardsGrid de metricas)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor grid | `div` | `grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6` |
| Card total wallets | `KpiCard` (existente) | `icon=Wallet iconBgClass="bg-purple-900/30"` |
| Card saldo total | `KpiCard` (existente) | `icon=TrendingUp iconBgClass="bg-green-900/30" valueColorClass="text-[#f59e0b]"` |
| Card transac. pendientes | `KpiCard` (existente) | `icon=Clock iconBgClass="bg-amber-900/30" valueColorClass="text-[#f59e0b]"` |
| Card total pagado | `KpiCard` (existente) | `icon=CheckCircle iconBgClass="bg-blue-900/30"` |

Reutiliza el componente `KpiCard` existente en
`src/admin/src/components/crowdpromotion/metricas/KpiCard.tsx` sin modificacion.

**WalletsBuscadorFiltros (nuevo)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `Card` | `bg-[#151525] border-zinc-800 p-4 mb-4` |
| Input busqueda | `Input` | `bg-[#0f0f1f] border-zinc-700 pl-9 text-white` con icono `Search` absoluto |
| Input saldo min | `Input type="number"` | `bg-[#0f0f1f] border-zinc-700 text-white w-32` |
| Input saldo max | `Input type="number"` | Mismos estilos que saldo min |
| Boton limpiar | `Button variant="ghost" size="sm"` | `text-[#64748b] hover:text-[#94a3b8]` con icono `X` |

```tsx
<Card className="bg-[#151525] border-zinc-800 p-4 mb-4">
    <div className="flex flex-wrap items-center gap-3">
        <div className="relative flex-1 min-w-[200px]">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-[#64748b]" />
            <Input
                placeholder="Buscar promotor por nombre o email..."
                className="bg-[#0f0f1f] border-zinc-700 text-white pl-9 focus-visible:ring-[#a855f7]"
                aria-label="Buscar promotor"
            />
        </div>
        <Input
            type="number"
            placeholder="Saldo min"
            className="bg-[#0f0f1f] border-zinc-700 text-white w-28"
            aria-label="Saldo minimo"
        />
        <Input
            type="number"
            placeholder="Saldo max"
            className="bg-[#0f0f1f] border-zinc-700 text-white w-28"
            aria-label="Saldo maximo"
        />
        <Button
            variant="ghost"
            size="sm"
            className="text-[#64748b] hover:text-[#94a3b8]"
        >
            <X className="w-3.5 h-3.5 mr-1" />
            Limpiar
        </Button>
    </div>
</Card>
```

**WalletsTable (nuevo)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `Card` | `bg-[#151525] border-zinc-800` |
| Tabla | `Table` | `aria-label="Lista de wallets de promotores"` |
| Cabecera tabla | `TableHeader` | `border-zinc-800` |
| Fila header | `TableRow` | `hover:bg-transparent border-zinc-800` |
| Celda header | `TableHead` | `text-[#64748b]` |
| Cuerpo tabla | `TableBody` | - |
| Fila datos | `TableRow` | `border-zinc-800 hover:bg-[#1e1e38] transition-colors cursor-pointer` |
| Avatar promotor | `Avatar` + `AvatarFallback` | `w-8 h-8 bg-[#1e1e38] border border-zinc-700 text-[#94a3b8] text-xs` |
| Nombre promotor | `TableCell` | `text-white text-sm font-medium` |
| Email | `TableCell` | `text-[#64748b] text-sm hidden md:table-cell` |
| Saldo disponible | `TableCell` | `text-[#f59e0b] text-sm font-semibold tabular-nums` |
| Saldo pendiente | `TableCell` | `text-[#94a3b8] text-sm tabular-nums hidden lg:table-cell` |
| Total ganado | `TableCell` | `text-[#10b981] text-sm tabular-nums hidden lg:table-cell` |
| Boton detalle | `Button variant="ghost" size="sm"` | `text-[#a855f7] hover:text-[#c084fc]` con icono `ChevronRight` |

Columnas de la tabla:

| Header | Width | Alineacion | Sortable | Visible en |
|--------|-------|------------|----------|------------|
| Promotor | flex-1 | izquierda | Si | todos |
| Email | 200px | izquierda | No | md+ |
| Saldo disponible | 130px | derecha | Si | todos |
| Saldo pendiente | 120px | derecha | Si | lg+ |
| Total ganado | 120px | derecha | Si | lg+ |
| Acciones | 80px | centro | No | todos |

Reutiliza el hook `useSortableTable` existente en `src/admin/src/hooks/use-sortable-table.ts`
y el patron de iconos de ordenamiento de `RankingPromotoresTable`.

**WalletsPaginacion (nuevo)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `div` | `flex items-center justify-between py-4 border-t border-zinc-800` |
| Texto informativo | `p` | `text-xs text-[#64748b]` — "Mostrando 1-20 de 45 promotores" |
| Controles | `Pagination` | Estilos dark-theme: activa en `bg-[#a855f7]`, inactiva en `bg-transparent border-zinc-700` |

---

### 3.2 Pantalla: Detalle de Wallet de Promotor

**Ruta futura:** `/admin/crowdpromotion/wallets/[promotorId]`
**Proposito:** Vista completa del wallet de un promotor con historial y acciones de cambio de estado.

#### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ SIDEBAR Admin                                                      │
├────────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT                                                       │
│                                                                    │
│  BREADCRUMB: Wallets > Juan Garcia                                 │
│                                                                    │
│  CABECERA                                                          │
│  [Avatar] Juan Garcia          [Volver a wallets]                  │
│  juan@mail.com                                                     │
│                                                                    │
│  KPI CARDS (2 columnas en md, 4 en lg)                             │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────┐  │
│  │ Saldo disp.  │ │Saldo pend.   │ │Total ganado  │ │Total ret.│  │
│  │ 150.50 EUR   │ │   0.00 EUR   │ │ 200.00 EUR   │ │49.50 EUR │  │
│  └──────────────┘ └──────────────┘ └──────────────┘ └──────────┘  │
│                                                                    │
│  TABS                                                              │
│  [Historial] [Pendientes de cobro]                                 │
│                                                                    │
│  TAB: Historial                                                    │
│  FILTROS: [Tipo v] [Estado v] [Desde____] [Hasta____] [Limpiar]   │
│                                                                    │
│  TABLA DE TRANSACCIONES                                            │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Tipo │ Importe     │ Descripcion           │ Estado  │ Acc.  │  │
│  ├──────────────────────────────────────────────────────────────┤  │
│  │ [+]  │ +10.00 EUR  │ Comision por backing  │[PROCESADA]│ [v] │  │
│  │ [-]  │ -49.50 EUR  │ Retiro mensual        │[PENDIENTE]│ [v] │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  < [1] [2] >  Mostrando 1-10 de 15 transacciones                  │
└────────────────────────────────────────────────────────────────────┘
```

#### Componentes

**PromoWalletKpiCards (nuevo, reutiliza KpiCard)**

| KPI | Icono | Color icono | Color valor |
|-----|-------|-------------|-------------|
| Saldo disponible | `Wallet` | `text-[#f59e0b] bg-amber-900/30` | `text-[#f59e0b]` |
| Saldo pendiente | `Clock` | `text-[#a855f7] bg-purple-900/30` | `text-white` |
| Total ganado | `TrendingUp` | `text-[#10b981] bg-green-900/30` | `text-[#10b981]` |
| Total retirado | `ArrowDownCircle` | `text-[#94a3b8] bg-slate-800/50` | `text-[#94a3b8]` |

Composicion usando `KpiCard` existente con `layout="default"`:

```tsx
<div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
    <KpiCard
        label="Saldo Disponible"
        value="150,50 EUR"
        icon={Wallet}
        iconBgClass="bg-amber-900/30"
        iconColorClass="text-[#f59e0b]"
        valueColorClass="text-[#f59e0b]"
    />
    <KpiCard
        label="Saldo Pendiente"
        value="0,00 EUR"
        icon={Clock}
        iconBgClass="bg-purple-900/30"
        iconColorClass="text-[#a855f7]"
    />
    {/* Total Ganado y Total Retirado */}
</div>
```

**TransaccionesAdminFiltros (nuevo)**

Extiende el patron de `FiltroFechas` existente con dos selectores adicionales (tipo y estado).

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `div` | `flex flex-wrap items-center gap-3 mb-4` |
| Select tipo | `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | `w-36 bg-[#0f0f1f] border-zinc-700 text-sm text-white` |
| Select estado | `Select` + `SelectContent` + `SelectItem` | `w-40 bg-[#0f0f1f] border-zinc-700 text-sm text-white` |
| Input fecha desde | `Input type="date"` | `w-36 bg-[#0f0f1f] border-zinc-700 text-white [color-scheme:dark]` |
| Input fecha hasta | `Input type="date"` | Mismos estilos |
| Boton limpiar | `Button variant="ghost" size="sm"` | Solo visible cuando hay filtros activos |

Opciones del Select "Tipo":

| Valor | Etiqueta |
|-------|----------|
| `` (todos) | "Todos los tipos" |
| `true` | "Ingresos (creditos)" |
| `false` | "Retiros (debitos)" |

Opciones del Select "Estado":

| Valor | Etiqueta |
|-------|----------|
| `` (todos) | "Todos los estados" |
| `1` | "Pendiente" |
| `2` | "Procesada" |
| `3` | "Pagada" |
| `4` | "Cancelada" |

**TransaccionesAdminTable (nuevo, columna de acciones exclusiva de Admin)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `Card` | `bg-[#151525] border-zinc-800` |
| Tabla | `Table` | `aria-label="Historial de transacciones del promotor"` |
| Fila | `TableRow` | `border-zinc-800 hover:bg-[#1e1e38] transition-colors` |
| Icono tipo | `div` (custom) | `w-8 h-8 rounded-full` verde o rojo segun `esCredito` |
| Importe | `TableCell` | `font-semibold tabular-nums` verde si credito, rojo si debito |
| Descripcion | `TableCell` | `text-sm text-white truncate max-w-[200px]` con `Tooltip` para texto completo |
| Fecha | `TableCell` | `text-xs text-[#64748b] hidden md:table-cell` |
| Badge estado | `TransaccionEstadoBadge` (reutilizado de Landing) | Ver seccion 6 |
| Celda acciones | `TableCell` | `text-right` con `DropdownMenu` |

Columnas de la tabla:

| Header | Width | Alineacion | Sortable | Visible en |
|--------|-------|------------|----------|------------|
| Tipo | 60px | centro | No | todos |
| Importe | 130px | derecha | Si | todos |
| Descripcion | flex-1 | izquierda | No | todos |
| Fecha creacion | 130px | izquierda | Si | md+ |
| Estado | 120px | centro | Si | todos |
| Acciones | 80px | centro | No | todos |

**Columna de acciones - DropdownMenu para cambio de estado:**

El `DropdownMenu` aparece al hacer click en el icono de tres puntos `MoreVertical`.
Las opciones disponibles dependen del estado actual de la transaccion:

| Estado actual | Opciones disponibles |
|---------------|---------------------|
| Pendiente (1) | Marcar como Procesada, Cancelar |
| Procesada (2) | Marcar como Pagada, Cancelar |
| Pagada (3) | (sin acciones - estado final) |
| Cancelada (4) | (sin acciones - estado final) |

```tsx
<DropdownMenu>
    <DropdownMenuTrigger asChild>
        <Button
            variant="ghost"
            size="sm"
            className="text-[#64748b] hover:text-white focus-visible:ring-[#a855f7]"
            aria-label="Acciones de transaccion"
        >
            <MoreVertical className="w-4 h-4" />
        </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent
        align="end"
        className="bg-[#151525] border-zinc-700"
    >
        {estadoId === 1 && (
            <>
                <DropdownMenuItem
                    className="text-[#10b981] focus:bg-[#1e1e38] focus:text-[#10b981]"
                    onClick={() => onCambiarEstado(transaccionId, 2)}
                >
                    <CheckCircle className="w-4 h-4 mr-2" />
                    Marcar como Procesada
                </DropdownMenuItem>
                <DropdownMenuSeparator className="bg-zinc-700" />
                <DropdownMenuItem
                    className="text-[#ef4444] focus:bg-[#1e1e38] focus:text-[#ef4444]"
                    onClick={() => onCancelar(transaccionId)}
                >
                    <XCircle className="w-4 h-4 mr-2" />
                    Cancelar transaccion
                </DropdownMenuItem>
            </>
        )}
        {estadoId === 2 && (
            <>
                <DropdownMenuItem
                    className="text-[#3b82f6] focus:bg-[#1e1e38] focus:text-[#3b82f6]"
                    onClick={() => onCambiarEstado(transaccionId, 3)}
                >
                    <CreditCard className="w-4 h-4 mr-2" />
                    Marcar como Pagada
                </DropdownMenuItem>
                <DropdownMenuSeparator className="bg-zinc-700" />
                <DropdownMenuItem
                    className="text-[#ef4444] focus:bg-[#1e1e38] focus:text-[#ef4444]"
                    onClick={() => onCancelar(transaccionId)}
                >
                    <XCircle className="w-4 h-4 mr-2" />
                    Cancelar transaccion
                </DropdownMenuItem>
            </>
        )}
        {(estadoId === 3 || estadoId === 4) && (
            <DropdownMenuItem disabled className="text-[#64748b]">
                Sin acciones disponibles
            </DropdownMenuItem>
        )}
    </DropdownMenuContent>
</DropdownMenu>
```

---

### 3.3 Pantalla: Procesador de Cobros Pendientes

**Ruta futura:** `/admin/crowdpromotion/cobros/pendientes`
**Proposito:** Cola de todas las transacciones de retiro en estado Pendiente para procesamiento masivo.

#### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ MAIN CONTENT                                                       │
│                                                                    │
│  Cobros Pendientes                     [Marcar todas procesadas]   │
│                                                                    │
│  KPI HEADER                                                        │
│  ┌────────────────┐ ┌────────────────┐ ┌────────────────────────┐  │
│  │ 12 pendientes  │ │ 234.50 EUR     │ │ Mas antiguo: 3 dias    │  │
│  └────────────────┘ └────────────────┘ └────────────────────────┘  │
│                                                                    │
│  TABLA DE COBROS PENDIENTES                                        │
│  [ ] Promotor     │ Email    │ Importe     │ Fecha sol.  │ Acciones│
│  [ ] Juan Garcia  │ ...      │  50.00 EUR  │ hace 3 dias │ [Proc.] │
│  [ ] Maria Lopez  │ ...      │  22.00 EUR  │ hace 1 dia  │ [Proc.] │
│                                                                    │
│  [Marcar seleccionadas como Procesadas] (activo cuando hay sel.)  │
└────────────────────────────────────────────────────────────────────┘
```

#### Componentes

**CobrosPendientesTable (nuevo)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Checkbox de seleccion | `Checkbox` | `border-zinc-700 data-[state=checked]:bg-[#a855f7]` |
| Checkbox "seleccionar todos" | `Checkbox` en `TableHead` | Mismos estilos |
| Importe | `TableCell` | `text-[#ef4444] font-semibold tabular-nums` (siempre debito) |
| Tiempo transcurrido | `TableCell` | `text-[#f59e0b] text-xs` si supera 48h |
| Boton accion individual | `Button size="sm"` | `bg-[#10b981]/10 text-[#10b981] border border-green-800/50 hover:bg-[#10b981]/20` |
| Boton bulk action | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` — solo visible con seleccion |
| Alert de confirmacion | `Alert` | `bg-amber-950/30 border-amber-800/50 text-[#f59e0b]` — antes de accion masiva |

---

## 4. Dialogos y Modales

### 4.1 CambiarEstadoTransaccionDialog

Dialogo de confirmacion antes de cambiar el estado de una transaccion. Se muestra tanto para
acciones individuales desde el `DropdownMenu` como para acciones masivas.

**Trigger:** Click en "Marcar como Procesada" o "Cancelar transaccion" del `DropdownMenu`.

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Dialog | `Dialog` | Controlado por el padre |
| Content | `DialogContent` | `sm:max-w-md bg-[#0f1729] border-[#334155]` |
| Titulo | `DialogTitle` | `text-lg font-semibold text-white` |
| Descripcion | `DialogDescription` | `text-sm text-[#94a3b8]` — resumen de la transaccion a cambiar |
| Info de transaccion | `div` | `bg-[#151525] border border-[#334155] rounded-lg p-4 my-4` |
| Promotor | `p` | `text-sm text-white font-medium` |
| Importe | `p` | `text-lg font-bold text-[#ef4444]` (debito) |
| Estado actual badge | `TransaccionEstadoBadge` | Reutilizado |
| Flecha de cambio | `ArrowRight` icono | `text-[#64748b] mx-2` |
| Estado nuevo badge | `TransaccionEstadoBadge` | Reutilizado con el nuevo estado |
| Separador | `Separator` | `bg-[#334155]` |
| Boton cancelar | `Button variant="outline"` | `border-[#334155] text-[#94a3b8] hover:text-white` |
| Boton confirmar | `Button` | Segun la accion: verde para Procesada/Pagada, rojo para Cancelar |
| Estado loading | `Button disabled` | Con `Loader2 animate-spin` |

```tsx
<Dialog open={open} onOpenChange={onOpenChange}>
    <DialogContent className="sm:max-w-md bg-[#0f1729] border-[#334155]">
        <DialogHeader>
            <DialogTitle className="text-white">
                Cambiar estado de transaccion
            </DialogTitle>
            <DialogDescription className="text-[#94a3b8]">
                Esta accion cambiara el estado de la transaccion seleccionada.
            </DialogDescription>
        </DialogHeader>

        <div className="bg-[#151525] border border-[#334155] rounded-lg p-4 my-4">
            <p className="text-xs text-[#64748b] mb-1">Promotor</p>
            <p className="text-sm text-white font-medium mb-3">{promotorNombre}</p>
            <p className="text-xs text-[#64748b] mb-1">Importe</p>
            <p className="text-lg font-bold text-[#ef4444] tabular-nums mb-3">
                -{formatWalletImporte(importe, monedaNombre)}
            </p>
            <div className="flex items-center gap-2">
                <TransaccionEstadoBadge estadoId={estadoActual} />
                <ArrowRight className="w-4 h-4 text-[#64748b]" />
                <TransaccionEstadoBadge estadoId={estadoNuevo} />
            </div>
        </div>

        <Separator className="bg-[#334155]" />

        <DialogFooter className="flex gap-3 mt-4">
            <Button
                variant="outline"
                onClick={onClose}
                className="border-[#334155] text-[#94a3b8] hover:text-white"
            >
                Cancelar
            </Button>
            <Button
                onClick={onConfirmar}
                disabled={isLoading}
                className={accionEsCancelar
                    ? "bg-red-600 hover:bg-red-700 text-white"
                    : "bg-[#10b981] hover:bg-[#059669] text-white"
                }
            >
                {isLoading && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
                {accionEsCancelar ? "Confirmar cancelacion" : "Confirmar cambio"}
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

### 4.2 ConfirmarAccionMasivaDialog

Variante del dialog anterior para cuando se seleccionan multiples transacciones.

| Elemento adicional | Descripcion |
|--------------------|-------------|
| `Alert variant="warning"` | "Vas a modificar N transacciones. Esta accion no se puede deshacer." |
| Lista de importes | `ul` con los primeros 3 promotores afectados + "y N mas..." |

---

## 5. Tablas

### Resumen de Tablas Admin para Wallets

| Tabla | Filas | Columnas clave | Sortable | Paginacion |
|-------|-------|----------------|----------|------------|
| WalletsTable | Promotores | Nombre, Email, Saldo disp., Saldo pend. | Si | Si (20/pag) |
| TransaccionesAdminTable | Transacciones | Tipo, Importe, Descripcion, Estado, Acciones | Si | Si (10/pag) |
| CobrosPendientesTable | Solo debitos pendientes | Promotor, Importe, Fecha solicitud | No | Si (20/pag) |

---

## 6. Reutilizacion de Componentes de Landing

### TransaccionEstadoBadge

Componente que existe en la Landing (`src/web/src/features/crowdpromotion/wallet/presentation/components/TransaccionEstadoBadge.tsx`) y que el Admin puede reutilizar directamente, ya que ambas apps consumen `@shared/constants`.

El Admin no necesita reescribir este componente. Puede importar la logica desde `@shared/constants`:

```tsx
// En Admin: componente local que replica la logica compartida
// src/admin/src/components/crowdpromotion/wallets/TransaccionEstadoBadge.tsx

import { Badge } from "@/components/ui/badge"
import { ESTADO_WALLET_TRANSACCION_LABELS } from "@shared/constants"

interface TransaccionEstadoBadgeProps {
    estadoId: number
    className?: string
}

// Estilos mapeados segun ESTADO_WALLET_TRANSACCION_BADGES de @shared/constants
const BADGE_STYLES: Record<number, string> = {
    1: "bg-amber-950/50 text-[#f59e0b] border border-amber-800/50",
    2: "bg-green-950/50 text-[#10b981] border border-green-800/50",
    3: "bg-blue-950/50 text-[#3b82f6] border border-blue-800/50",
    4: "bg-slate-900/50 text-[#64748b] border border-slate-700/50",
}

export function TransaccionEstadoBadge({ estadoId, className }: TransaccionEstadoBadgeProps) {
    return (
        <Badge
            className={`${BADGE_STYLES[estadoId] ?? BADGE_STYLES[4]} text-xs font-medium ${className ?? ""}`}
        >
            {ESTADO_WALLET_TRANSACCION_LABELS[estadoId] ?? "Desconocido"}
        </Badge>
    )
}
```

### Mappers y Formatters compartidos

El Admin consume los mismos utilitarios del paquete `@shared/utils`:

| Funcion | Uso en Admin |
|---------|-------------|
| `formatWalletImporte(importe, monedaNombre)` | Formato de saldos en KPI cards |
| `formatTransaccionImporte(importe, esCredito, monedaNombre)` | Importes con signo en tabla |
| `mapEstadoWalletTransaccionToBadge(estadoId)` | Variante para Badge (no necesario si se usan las clases directas) |
| `mapTransaccionTipoToDisplayProps(esCredito)` | Icono y color de la columna "Tipo" |

---

## 7. Feedback y Estados

### Loading

| Elemento | Skeleton |
|----------|---------|
| WalletsResumenKpiGrid | 4 `Skeleton h-24 bg-[#1e1e38] rounded-xl animate-pulse` en grid |
| WalletsTable | 5 filas con `Skeleton h-10 w-full bg-[#1e1e38] animate-pulse` |
| PromoWalletKpiCards | 4 `Skeleton h-24` identico al anterior |
| TransaccionesAdminTable | 10 filas skeleton con iconos redondos + barras de texto |

Patron identico al `ProgramaMetricasTab.tsx` existente:

```tsx
// Estado loading en WalletsPage
if (isLoading) {
    return (
        <div className="space-y-6" aria-busy="true">
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                {[1, 2, 3, 4].map((i) => (
                    <Skeleton key={i} className="h-24 bg-[#1e1e38] rounded-xl animate-pulse" />
                ))}
            </div>
            <Skeleton className="h-[400px] bg-[#1e1e38] rounded-lg animate-pulse" />
        </div>
    )
}
```

### Error

| Escenario | Componente | Contenido |
|-----------|-----------|-----------|
| Error al cargar lista wallets | `Card role="alert"` | `AlertCircle text-red-400` + mensaje + `Button "Reintentar"` |
| Error al cargar detalle | `Card role="alert"` | Patron identico al `ProgramaMetricasTab` existente |
| Error al cambiar estado | `Alert variant="destructive"` dentro del dialog | Mensaje del backend |

Patron de error reutilizado del `ProgramaMetricasTab.tsx`:

```tsx
<Card
    className="bg-[#151525] border-zinc-800 p-8 flex flex-col items-center justify-center text-center"
    role="alert"
>
    <AlertCircle className="w-10 h-10 text-red-400 mb-3" />
    <p className="text-sm text-red-400 mb-4">
        No se pudieron cargar los wallets
    </p>
    <Button
        variant="outline"
        size="sm"
        onClick={() => refetch()}
        className="border-zinc-700 text-[#94a3b8] hover:text-white focus-visible:ring-2 focus-visible:ring-[#a855f7]"
    >
        <RotateCcw className="w-4 h-4 mr-1.5" />
        Reintentar
    </Button>
</Card>
```

### Success (Toast)

| Accion | Tipo toast | Mensaje |
|--------|-----------|---------|
| Estado cambiado exitosamente | Success | "Estado actualizado a {estadoNuevo}" |
| Accion masiva completada | Success | "N transacciones marcadas como Procesadas" |
| Error al cambiar estado | Destructive | Mensaje del backend o error generico |

### Empty State

| Contexto | Componente | Mensaje |
|----------|-----------|---------|
| Sin wallets | `div` centrado | Icono `Wallet` + "Ningun promotor tiene wallet activo todavia" |
| Sin transacciones | `TableRow` colspan | "Sin transacciones en este periodo" (identico al `RankingPromotoresTable`) |
| Sin cobros pendientes | `div` centrado | Icono `CheckCircle text-[#10b981]` + "No hay cobros pendientes de procesar" |

```tsx
// Empty state: sin cobros pendientes (caso positivo para admin)
<div className="flex flex-col items-center justify-center py-16 text-center">
    <div className="w-16 h-16 rounded-full bg-green-900/30 border border-green-800/50 flex items-center justify-center mb-4">
        <CheckCircle className="w-7 h-7 text-[#10b981]" />
    </div>
    <h3 className="text-base font-semibold text-white mb-2">
        Todo al dia
    </h3>
    <p className="text-sm text-[#64748b]">
        No hay cobros pendientes de procesar en este momento.
    </p>
</div>
```

---

## 8. Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| < md (mobile, < 768px) | KPI grid 2 columnas. Tabla con columnas email/fechas ocultas (`hidden md:table-cell`). Filtros en columna vertical. DropdownMenu en lugar de botones inline. |
| md-lg (tablet, 768-1024px) | KPI grid 2 columnas. Tabla con email visible, columnas de saldos secundarios ocultas. Filtros en fila horizontal. |
| > lg (desktop, 1024px+) | Layout completo: KPI grid 4 columnas. Todas las columnas de tabla visibles. Sidebar fijo. |

Clases Tailwind para la tabla de wallets:

```tsx
// Columna email: visible desde md
className="hidden md:table-cell text-[#64748b] text-sm"

// Columna saldo pendiente: visible desde lg
className="hidden lg:table-cell text-[#94a3b8] text-sm tabular-nums"

// Columna total ganado: visible desde lg
className="hidden lg:table-cell text-[#10b981] text-sm tabular-nums"
```

---

## 9. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Labels de tabla | `TableHead` con `scope="col"`. `Table` con `aria-label` descriptivo |
| Ordenamiento | `aria-sort="ascending|descending|none"` en headers sortables (patron de `RankingPromotoresTable`) |
| Checkboxes de seleccion | `aria-label="Seleccionar transaccion de {promotorNombre} por {importe}"` |
| Checkbox "seleccionar todos" | `aria-label="Seleccionar todas las transacciones pendientes"` |
| DropdownMenu | `aria-label="Acciones para transaccion de {promotorNombre}"` en el trigger |
| Dialog | `DialogTitle` siempre visible. Radix gestiona `aria-modal="true"` y focus-trap |
| Estados de carga | `aria-busy="true"` en el contenedor mientras carga |
| Mensajes de error | `role="alert"` en los contenedores de error |
| Badges de estado | Texto visible siempre; no dependen solo del color |
| Botones de accion destructiva | Color rojo con texto explicito "Cancelar transaccion" (no solo icono) |
| Focus ring | `focus-visible:ring-2 focus-visible:ring-[#a855f7]` en todos los interactivos |
| Contraste | `#f59e0b` sobre `#151525` = 5.74:1 (AA). `#10b981` sobre `#151525` = 4.61:1 (AA) |
| Paginacion | `aria-label` en botones: "Ir a pagina N", "Pagina anterior", "Pagina siguiente". `aria-current="page"` en pagina activa |

---

## 10. Estructura de Archivos Propuesta (Futuro)

```
src/admin/src/
├── app/(dashboard)/
│   └── crowdpromotion/
│       └── wallets/
│           ├── page.tsx                        <- WalletsPage (lista)
│           ├── loading.tsx                     <- Skeleton de la lista
│           ├── [promotorId]/
│           │   └── page.tsx                   <- WalletDetallePage
│           └── cobros/
│               └── pendientes/
│                   └── page.tsx               <- CobrosPendientesPage
│
└── components/crowdpromotion/
    └── wallets/
        ├── WalletsResumenKpiGrid.tsx           <- Grid de 4 KPIs de resumen global
        ├── WalletsBuscadorFiltros.tsx          <- Input busqueda + filtros saldo
        ├── WalletsTable.tsx                   <- Tabla con sortable y paginacion
        ├── PromoWalletKpiCards.tsx            <- 4 KPIs del wallet de un promotor
        ├── TransaccionesAdminFiltros.tsx      <- Tipo + Estado + Fechas
        ├── TransaccionesAdminTable.tsx        <- Tabla con DropdownMenu de acciones
        ├── TransaccionEstadoBadge.tsx         <- Badge de estado (patron compartido)
        ├── CambiarEstadoTransaccionDialog.tsx <- Dialog de confirmacion individual
        ├── ConfirmarAccionMasivaDialog.tsx    <- Dialog de confirmacion masiva
        └── CobrosPendientesTable.tsx          <- Tabla con checkboxes y bulk action
```

### Hooks necesarios (Admin)

| Hook | Descripcion | Patron de referencia |
|------|-------------|---------------------|
| `useAdminWallets(filtros, pagina)` | Lista paginada de wallets | `useProgramaMetricas` |
| `useAdminWalletDetalle(promotorId)` | Resumen del wallet de un promotor | `usePromoPrograma` |
| `useAdminWalletTransacciones(promotorId, filtros, pagina)` | Historial paginado con filtros | `useProgramaMetricas` |
| `useCambiarEstadoTransaccion()` | Mutation para cambiar estado | `useDesactivarPromoPrograma` |
| `useCobrosPendientes(pagina)` | Lista de retiros en estado Pendiente | `useProgramaMetricas` |

### Endpoints de API que se necesitaran (no existentes en MVP)

| Endpoint | Descripcion |
|----------|-------------|
| `GET /api/admin/crowdpromotion/wallets` | Lista paginada de todos los wallets |
| `GET /api/admin/crowdpromotion/wallets/{promotorId}` | Resumen de wallet de un promotor |
| `GET /api/admin/crowdpromotion/wallets/{promotorId}/transacciones` | Historial con filtros |
| `PATCH /api/admin/crowdpromotion/transacciones/{id}/estado` | Cambiar estado de transaccion |
| `POST /api/admin/crowdpromotion/transacciones/bulk-estado` | Cambio masivo de estado |

Estos endpoints **no existen** en MVP y requeriran nuevos Commands/Queries en el backend.

---

## 11. Estimacion de Complejidad

| Pantalla / Componente | Complejidad | Justificacion |
|-----------------------|-------------|---------------|
| WalletsResumenKpiGrid | Baja | Reutiliza `KpiCard` sin modificar |
| WalletsBuscadorFiltros | Baja | Patron de `BackingFilters` existente |
| WalletsTable | Media | Sortable + paginacion + Avatar. Patron de `RankingPromotoresTable` |
| TransaccionEstadoBadge | Baja | Logica en `@shared/constants`, solo presentacion |
| TransaccionesAdminFiltros | Baja-Media | Extiende `FiltroFechas` con 2 selects adicionales |
| TransaccionesAdminTable | Media-Alta | `DropdownMenu` con logica condicional por estado + `Tooltip` |
| CambiarEstadoTransaccionDialog | Media | Patron de `DesactivarPromoProgramaDialog` adaptado |
| ConfirmarAccionMasivaDialog | Media | Variante del anterior + `Alert` de advertencia |
| CobrosPendientesTable | Alta | Checkboxes de seleccion multiple + bulk actions |
| PromoWalletKpiCards | Baja | Reutiliza `KpiCard` directamente |
| WalletsPage (integracion) | Media | Composicion de todos los anteriores + hooks |
| WalletDetallePage + Tabs | Media | `Tabs` de shadcn con 2 panels + navegacion |
| CobrosPendientesPage | Media-Alta | Estado de seleccion multiple + confirmacion masiva |

**Estimacion total de implementacion futura:** 3-4 dias de desarrollo (aprox. 24-32 horas).
El mayor factor de complejidad es la API del Admin que debe implementarse desde cero.

---

## 12. Checklist

- [ ] La nota "FUERA DE SCOPE MVP" es clara al inicio del documento
- [ ] `KpiCard` existente se reutiliza sin modificar para todos los KPI grids
- [ ] `RankingPromotoresTable` sirve como patron directo para `WalletsTable`
- [ ] `useSortableTable` existente se reutiliza para ordenamiento de tablas
- [ ] `TransaccionEstadoBadge` consume `ESTADO_WALLET_TRANSACCION_LABELS` de `@shared/constants`
- [ ] `formatWalletImporte` y `formatTransaccionImporte` de `@shared/utils` se usan en Admin
- [ ] `DropdownMenu` tiene `aria-label` en el trigger
- [ ] Acciones destructivas (Cancelar) tienen color rojo y texto explicito
- [ ] Todos los badges de estado tienen texto visible (no solo color)
- [ ] Tablas tienen `aria-sort` en headers sortables
- [ ] Checkboxes tienen `aria-label` descriptivo
- [ ] `Dialog` tiene `DialogTitle` visible siempre
- [ ] Columnas secundarias usan `hidden md:table-cell` y `hidden lg:table-cell`
- [ ] Estados loading usan `aria-busy="true"` y `Skeleton` con `animate-pulse`
- [ ] Estados de error usan `role="alert"` y boton "Reintentar"
- [ ] Empty state de cobros pendientes usa icono positivo (CheckCircle verde)
- [ ] Estimacion de complejidad documentada por componente
- [ ] Endpoints de API futura documentados como "no existentes en MVP"
