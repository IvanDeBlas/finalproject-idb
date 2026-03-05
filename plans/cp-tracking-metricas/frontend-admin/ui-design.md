# Diseno UI: cp-tracking-metricas (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/admin (Next.js 14, puerto 3001)
**Ruta principal:** `/campanias/{campaniaId}/crowdpromotion/programas/{programaId}/metricas`

---

## 1. Resumen

- **Componentes shadcn/ui utilizados:** Card, CardHeader, CardContent, CardTitle, CardFooter, Table, TableHeader, TableBody, TableRow, TableHead, TableCell, Badge, Avatar, AvatarFallback, Input, Button, Skeleton, Alert, AlertTitle, AlertDescription, Separator
- **Composiciones custom nuevas:** 8 (ProgramaMetricasTab, FiltroFechas, KpiCardsGrid, RankingPromotoresTable, DesgloseEventosPanel, GraficoTemporal, MetricasLoadingSkeleton, MetricasEmptyState)
- **Libreria de graficos:** Recharts (LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer)
- **Iconos Lucide:** MousePointerClick, UserPlus, ShoppingCart, Euro, TrendingUp, Wallet, ArrowUpDown, AlertCircle, Loader2, X, ChevronRight, BarChart2
- **Responsive breakpoints:** md (768px), lg (1024px)
- **Patron de integracion:** Pestana "Metricas" dentro de la pagina de detalle del programa de promocion

---

## 2. Paleta de Colores

| Uso | Token CSS | Valor Hex | Ejemplo |
|-----|-----------|-----------|---------|
| Fondo base | `--bg-primary` | `#0d0d1a` | Sidebar, header |
| Fondo pagina | `--bg-secondary` | `#1a1a2e` | Main content area |
| Fondo card | `--bg-card` | `#151525` | Todas las cards de metricas |
| Fondo hover/skeleton | `--bg-card-hover` | `#1e1e38` | Hover filas tabla, skeletons |
| Fondo input | `--bg-input` | `#0f0f1f` | Inputs de fecha |
| Primario gradiente | `--primary-gradient` | `#ec4899 -> #a855f7` | Boton Aplicar |
| Texto primario | `--text-primary` | `#ffffff` | Valores KPI, nombres |
| Texto secundario | `--text-secondary` | `#94a3b8` | Labels de KPI, cabeceras tabla |
| Texto muted | `--text-muted` | `#64748b` | Fechas, notas, posicion ranking |
| Borde default | `--border-default` | `#334155` | Bordes de cards, inputs, tabla |
| Borde hover/accent | `--border-focus` | `#a855f7` | Cabeceras activas de tabla |
| KPI clicks | `--kpi-clicks` | `#3b82f6` | Icono, valor clicks |
| KPI signups | `--kpi-signups` | `#10b981` | Icono, valor signups |
| KPI conversiones | `--kpi-conversiones` | `#a855f7` | Icono, valor backings |
| KPI valor monetario | `--kpi-valor` | `#f59e0b` | Icono, valor EUR |
| KPI comision | `--kpi-comision` | `#10b981` | Icono, valor comision |
| Chart clicks | `--chart-clicks` | `#3b82f6` | Linea clicks en grafico |
| Chart signups | `--chart-signups` | `#10b981` | Linea signups en grafico |
| Chart conversiones | `--chart-conversiones` | `#a855f7` | Linea conversiones en grafico |
| Error | `--status-error` | `#ef4444` | Alertas de error, validacion |

---

## 3. Layout General de la Pantalla

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-[250px] fixed, bg-[#0d0d1a])                             │
│ [Logo WePlay] Dashboard / Campanias / Perfil / Promotor             │
├─────────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT (ml-[250px] bg-[#1a1a2e] min-h-screen px-8 pt-8)      │
│                                                                     │
│  [BREADCRUMB] Campanias > Album 2026 > CrowdPromotion > Metricas   │
│                                                                     │
│  [TITULO] Metricas: Promociona mi nuevo album                       │
│                                                                     │
│  ┌─ FiltroFechas ────────────────────────────────────────────────┐  │
│  │ Periodo: [date input] a [date input] [Aplicar] [Limpiar?]    │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌─ KpiCardsGrid (fila 4) ───────────────────────────────────────┐  │
│  │ [Clicks] [Signups] [Backings] [Valor EUR]                     │  │
│  └───────────────────────────────────────────────────────────────┘  │
│  ┌─ KpiCardsGrid (fila 2) ───────────────────────────────────────┐  │
│  │ [Tasa conversion] [Comisiones pagadas]                        │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌─ grid-cols-5 ─────────────────────────────────────────────────┐  │
│  │ ┌─ RankingPromotoresTable (col-span-3) ┐ ┌─ DesglosePanel ─┐ │  │
│  │ │ # | Promotor | Clicks | Conv | Valor │ │ Clicks    1,250 │ │  │
│  │ │ 1 | DJ Mark  |   450  |   5  | 500  │ │ Page views  890 │ │  │
│  │ └──────────────────────────────────────┘ └─────────────────┘ │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌─ GraficoTemporal ─────────────────────────────────────────────┐  │
│  │ Eventos por dia                                               │  │
│  │ [Recharts LineChart h-64]                                     │  │
│  └───────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 4. Componentes por Seccion

### 4.1 Breadcrumb

**Descripcion:** Navegacion contextual que muestra la ruta completa hasta la pestana Metricas. No es un componente shadcn sino un `<nav>` semantico.

**Props interface:**
```
interface BreadcrumbMetricasProps {
    campaniaId: string
    campaniaTitulo: string
    programaId: string
    programaTitulo: string
}
```

**Elementos:**

| Elemento | Etiqueta HTML | Clases Tailwind |
|----------|---------------|-----------------|
| Contenedor nav | `<nav aria-label="Breadcrumb">` | `flex items-center gap-1.5 text-xs text-[#64748b] mb-4` |
| Link "Campanias" | `<Link href="/campanias">` | `hover:text-[#94a3b8] transition-colors` |
| Separador | `<ChevronRight className="w-3 h-3">` | `text-[#334155]` |
| Link campana (truncado) | `<Link href="/campanias/{id}">` | `hover:text-[#94a3b8] transition-colors max-w-[120px] truncate` |
| Separador | `<ChevronRight className="w-3 h-3">` | `text-[#334155]` |
| Link "CrowdPromotion" | `<Link href="/campanias/{id}/crowdpromotion">` | `hover:text-[#94a3b8] transition-colors` |
| Separador | `<ChevronRight className="w-3 h-3">` | `text-[#334155]` |
| Span "Metricas" (activo) | `<span aria-current="page">` | `text-[#94a3b8]` |

**Accesibilidad:** `aria-label="Breadcrumb"` en el nav. `aria-current="page"` en el ultimo item.

---

### 4.2 FiltroFechas

**Descripcion:** Formulario inline con dos date inputs y boton de accion. Persiste el estado del filtro activo con un badge visible. Se integra con `react-hook-form` y `filtroFechasSchema`.

**Props interface:**
```
interface FiltroFechasProps {
    onAplicar: (filtro: FiltroFechasFormData) => void
    onLimpiar: () => void
    isLoading: boolean
    filtroActivo: FiltroFechasFormData | null
}
```

**Composicion:**
```tsx
<div
    className="flex flex-wrap items-start gap-3 mb-8 bg-[#151525] border border-[#334155] rounded-xl p-4"
    aria-label="Filtro de fechas"
>
    {/* Fila principal del filtro */}
    <div className="flex flex-wrap items-center gap-3">
        <label htmlFor="fechaDesde" className="text-sm text-[#94a3b8] shrink-0">
            Periodo:
        </label>
        <Input
            id="fechaDesde"
            type="date"
            aria-label="Fecha de inicio"
            className="bg-[#0f0f1f] border-[#334155] text-white text-sm w-36 [color-scheme:dark]"
            max={today}  {/* YYYY-MM-DD */}
        />
        <span className="text-sm text-[#64748b]">a</span>
        <Input
            id="fechaHasta"
            type="date"
            aria-label="Fecha de fin"
            className="bg-[#0f0f1f] border-[#334155] text-white text-sm w-36 [color-scheme:dark]"
            max={today}
        />
        <Button
            size="sm"
            type="submit"
            disabled={isLoading || !!dateError}
            className="bg-gradient-to-r from-pink-500 to-purple-600 text-white text-sm px-4 hover:from-pink-400 hover:to-purple-500"
        >
            {isLoading ? (
                <Loader2 className="w-4 h-4 animate-spin mr-1.5" />
            ) : null}
            Aplicar
        </Button>
        {/* Boton Limpiar - solo visible cuando filtroActivo != null */}
        {filtroActivo && (
            <Button
                variant="ghost"
                size="sm"
                onClick={onLimpiar}
                className="text-[#64748b] hover:text-[#94a3b8] text-sm"
            >
                Limpiar
            </Button>
        )}
    </div>

    {/* Error de validacion inline */}
    {dateError && (
        <p className="w-full text-xs text-[#ef4444] mt-1" role="alert">
            La fecha de inicio no puede ser posterior a la fecha fin
        </p>
    )}

    {/* Badge de filtro activo */}
    {filtroActivo && (
        <div className="flex items-center gap-1.5 mt-1">
            <Badge
                className="bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs flex items-center gap-1"
            >
                Filtrando: {formattedDateRange}
                <button
                    onClick={onLimpiar}
                    aria-label="Quitar filtro de fechas"
                    className="ml-0.5 hover:text-white transition-colors"
                >
                    <X className="w-3.5 h-3.5" />
                </button>
            </Badge>
        </div>
    )}
</div>
```

**Estados:**

| Estado | Visual |
|--------|--------|
| Default (sin filtro) | Inputs vacios, boton "Aplicar" habilitado, sin badge, sin boton "Limpiar" |
| Filtro activo | Badge con rango de fechas visible, boton "Limpiar" visible |
| Loading (aplicando) | Boton "Aplicar" con `<Loader2 animate-spin>` y deshabilitado |
| Error validacion (desde > hasta) | Mensaje rojo debajo de inputs, boton "Aplicar" deshabilitado |
| Error rango > 365 dias | Aviso informativo `text-[#f59e0b]` (no bloquea) |

**Validacion:** `fechaDesde <= fechaHasta` via `filtroFechasSchema`. `max` del input de fecha hasta = fecha de hoy.

---

### 4.3 KpiCardsGrid

**Descripcion:** Dos filas de KPI cards. La primera fila tiene 4 cards (Clicks, Signups, Backings, Valor). La segunda fila tiene 2 cards secundarias (Tasa de conversion, Comisiones).

**Props interface:**
```
interface KpiCardsGridProps {
    kpis: ProgramaMetricasKpis
    isLoading: boolean
}
```

**Composicion - Fila primaria (4 cards):**
```tsx
<div
    className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4"
    aria-label="Indicadores clave de rendimiento"
    aria-busy={isLoading}
>
    {/* Card Clicks */}
    <Card className="bg-[#151525] border-[#334155] p-5">
        <div className="w-10 h-10 rounded-lg bg-blue-950/50 flex items-center justify-center mb-3">
            <MousePointerClick className="w-5 h-5 text-[#3b82f6]" />
        </div>
        <p className="text-3xl font-bold text-white tabular-nums">
            {kpis.totalClicks.toLocaleString('es-ES')}
        </p>
        <p className="text-sm text-[#94a3b8] mt-1">Clicks totales</p>
    </Card>

    {/* Card Signups */}
    <Card className="bg-[#151525] border-[#334155] p-5">
        <div className="w-10 h-10 rounded-lg bg-green-950/50 flex items-center justify-center mb-3">
            <UserPlus className="w-5 h-5 text-[#10b981]" />
        </div>
        <p className="text-3xl font-bold text-white tabular-nums">
            {kpis.totalSignups.toLocaleString('es-ES')}
        </p>
        <p className="text-sm text-[#94a3b8] mt-1">Registros</p>
    </Card>

    {/* Card Backings */}
    <Card className="bg-[#151525] border-[#334155] p-5">
        <div className="w-10 h-10 rounded-lg bg-purple-950/50 flex items-center justify-center mb-3">
            <ShoppingCart className="w-5 h-5 text-[#a855f7]" />
        </div>
        <p className="text-3xl font-bold text-white tabular-nums">
            {kpis.totalConversiones.toLocaleString('es-ES')}
        </p>
        <p className="text-sm text-[#94a3b8] mt-1">Backings</p>
    </Card>

    {/* Card Valor generado */}
    <Card className="bg-[#151525] border-[#334155] p-5">
        <div className="w-10 h-10 rounded-lg bg-amber-950/50 flex items-center justify-center mb-3">
            <Euro className="w-5 h-5 text-[#f59e0b]" />
        </div>
        <p className="text-3xl font-bold text-[#f59e0b] tabular-nums">
            {kpis.valorTotalGenerado.toLocaleString('es-ES')}
            <span className="text-lg ml-1 text-[#64748b]">
                {kpis.monedaNombre ?? 'EUR'}
            </span>
        </p>
        <p className="text-sm text-[#94a3b8] mt-1">Valor generado</p>
    </Card>
</div>
```

**Composicion - Fila secundaria (2 cards):**
```tsx
<div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
    {/* Card Tasa conversion */}
    <Card className="bg-[#151525] border-[#334155] p-5 flex items-center gap-4">
        <div className="w-10 h-10 rounded-lg bg-[#1e1e38] flex items-center justify-center shrink-0">
            <TrendingUp className="w-5 h-5 text-[#94a3b8]" />
        </div>
        <div>
            <p className="text-sm text-[#94a3b8]">Tasa de conversion</p>
            <p className="text-2xl font-bold text-white">
                {formatTasaConversion(kpis.tasaConversion)}
            </p>
            <p className="text-xs text-[#64748b]">conversiones / clicks</p>
        </div>
    </Card>

    {/* Card Comisiones */}
    <Card className="bg-[#151525] border-[#334155] p-5 flex items-center gap-4">
        <div className="w-10 h-10 rounded-lg bg-green-950/50 flex items-center justify-center shrink-0">
            <Wallet className="w-5 h-5 text-[#10b981]" />
        </div>
        <div>
            <p className="text-sm text-[#94a3b8]">Comisiones pagadas</p>
            <p className="text-2xl font-bold text-white tabular-nums">
                {kpis.comisionesTotales.toLocaleString('es-ES')}
                <span className="text-lg ml-1 text-[#64748b]">
                    {kpis.monedaNombre ?? 'EUR'}
                </span>
            </p>
            <p className="text-xs text-[#64748b]">total acumulado</p>
        </div>
    </Card>
</div>
```

**Skeleton de KPI (loading):**
```tsx
{/* 4 skeletons fila primaria */}
<div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4" aria-busy="true">
    {Array.from({ length: 4 }).map((_, i) => (
        <Skeleton key={i} className="h-24 bg-[#1e1e38] rounded-xl" />
    ))}
</div>
{/* 2 skeletons fila secundaria */}
<div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
    {Array.from({ length: 2 }).map((_, i) => (
        <Skeleton key={i} className="h-16 bg-[#1e1e38] rounded-xl" />
    ))}
</div>
```

**Animacion al cargar:** Fade in + slide up. Clase: `animate-in fade-in slide-in-from-bottom-2 duration-300`.

---

### 4.4 RankingPromotoresTable

**Descripcion:** Tabla de promotores ordenable con columnas: posicion, promotor (nombre + avatar + badge tipo), clicks, conversiones, valor generado, comision. La ordenacion es local (client-side) sobre los datos ya cargados.

**Props interface:**
```
interface RankingPromotoresTableProps {
    promotores: RankingPromotorItem[]
    isLoading: boolean
    sortColumn: SortableColumn | null
    sortDirection: 'asc' | 'desc'
    onSort: (column: SortableColumn) => void
}

type SortableColumn = 'clicks' | 'conversiones' | 'valorGenerado' | 'comisionAcumulada'
```

**Composicion:**
```tsx
<Card className="bg-[#151525] border-[#334155] col-span-3">
    <CardHeader className="border-b border-[#334155] px-5 py-4">
        <CardTitle className="text-base font-semibold text-white">
            Ranking de Promotores
        </CardTitle>
    </CardHeader>
    <div className="overflow-x-auto">
        <Table aria-label="Ranking de promotores por actividad">
            <TableHeader className="[&_tr]:border-b [&_tr]:border-[#334155]">
                <TableRow>
                    {/* Columna posicion - no ordenable */}
                    <TableHead
                        scope="col"
                        className="text-xs font-medium text-[#64748b] uppercase tracking-wide px-4 py-3 w-10"
                    >
                        #
                    </TableHead>

                    {/* Columna promotor - no ordenable */}
                    <TableHead
                        scope="col"
                        className="text-xs font-medium text-[#64748b] uppercase tracking-wide px-4 py-3"
                    >
                        Promotor
                    </TableHead>

                    {/* Columnas ordenables: Clicks, Conv, Valor, Comision */}
                    {[
                        { key: 'clicks', label: 'Clicks' },
                        { key: 'conversiones', label: 'Conv.' },
                        { key: 'valorGenerado', label: 'Valor' },
                        { key: 'comisionAcumulada', label: 'Comision' },
                    ].map(({ key, label }) => (
                        <TableHead
                            key={key}
                            scope="col"
                            aria-sort={
                                sortColumn === key
                                    ? sortDirection === 'asc' ? 'ascending' : 'descending'
                                    : 'none'
                            }
                            className={cn(
                                "text-xs font-medium uppercase tracking-wide px-4 py-3 cursor-pointer select-none",
                                sortColumn === key
                                    ? "text-[#a855f7]"
                                    : "text-[#64748b] hover:text-[#94a3b8]",
                            )}
                            onClick={() => onSort(key as SortableColumn)}
                        >
                            <span className="flex items-center gap-1">
                                {label}
                                <ArrowUpDown
                                    className={cn(
                                        "w-3.5 h-3.5",
                                        sortColumn === key ? "text-[#a855f7]" : "text-[#334155]"
                                    )}
                                />
                            </span>
                        </TableHead>
                    ))}
                </TableRow>
            </TableHeader>

            <TableBody>
                {promotores.map((promotor, index) => (
                    <TableRow
                        key={promotor.promotorId}
                        className="border-b border-[#1e1e38] hover:bg-[#1e1e38] transition-all duration-200"
                    >
                        {/* Posicion */}
                        <TableCell className="text-sm font-bold text-[#64748b] w-10 pl-4">
                            #{index + 1}
                        </TableCell>

                        {/* Promotor: Avatar + Nombre + Badge tipo */}
                        <TableCell>
                            <div className="flex items-center gap-2">
                                <Avatar className="w-7 h-7">
                                    <AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-xs">
                                        {promotor.promotorNombre
                                            .split(' ')
                                            .map(n => n[0])
                                            .join('')
                                            .toUpperCase()
                                            .slice(0, 2)}
                                    </AvatarFallback>
                                </Avatar>
                                <span className="text-sm font-medium text-white">
                                    {promotor.promotorNombre}
                                </span>
                                {promotor.tipoPromotorNombre && (
                                    <Badge className="bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs ml-1.5">
                                        {promotor.tipoPromotorNombre}
                                    </Badge>
                                )}
                            </div>
                        </TableCell>

                        {/* Clicks */}
                        <TableCell className="text-sm text-[#3b82f6] font-medium tabular-nums">
                            {promotor.clicks.toLocaleString('es-ES')}
                        </TableCell>

                        {/* Conversiones */}
                        <TableCell className="text-sm text-[#a855f7] font-medium tabular-nums">
                            {promotor.conversiones.toLocaleString('es-ES')}
                        </TableCell>

                        {/* Valor generado */}
                        <TableCell className="text-sm text-[#f59e0b] font-medium tabular-nums">
                            {promotor.valorGenerado.toLocaleString('es-ES')} EUR
                        </TableCell>

                        {/* Comision */}
                        <TableCell className="text-sm text-[#10b981] font-medium tabular-nums">
                            {promotor.comisionAcumulada.toLocaleString('es-ES')} EUR
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    </div>
</Card>
```

**Comportamiento de ordenacion:**
- Click en cabecera alterna entre `asc` y `desc`.
- Solo una columna activa a la vez.
- Por defecto: ordenado por `conversiones desc`.
- Cabecera activa: `text-[#a855f7]`, icono `ArrowUpDown` visible en `text-[#a855f7]`.
- Cabecera inactiva: `text-[#64748b]`, icono en `text-[#334155]`.
- Las filas se re-ordenan con `transition-all duration-200`.

**Responsive (mobile):** En pantallas `< md`, mostrar solo columnas Promotor, Clicks, Conv. Ocultar Valor y Comision con `hidden md:table-cell`.

**Skeleton de tabla:**
```tsx
<Card className="bg-[#151525] border-[#334155] col-span-3">
    <CardHeader className="border-b border-[#334155] px-5 py-4">
        <Skeleton className="h-5 w-40 bg-[#1e1e38]" />
    </CardHeader>
    <div className="p-4 space-y-3" aria-busy="true">
        {Array.from({ length: 5 }).map((_, i) => (
            <div key={i} className="flex items-center gap-3">
                <Skeleton className="h-7 w-7 rounded-full bg-[#1e1e38]" />
                <Skeleton className="h-4 flex-1 bg-[#1e1e38]" />
                <Skeleton className="h-4 w-12 bg-[#1e1e38]" />
                <Skeleton className="h-4 w-12 bg-[#1e1e38]" />
                <Skeleton className="h-4 w-16 bg-[#1e1e38]" />
            </div>
        ))}
    </div>
</Card>
```

**Empty state (sin promotores):**
```tsx
<div className="text-center py-10 text-sm text-[#64748b]">
    Sin promotores con actividad en este periodo
</div>
```

---

### 4.5 DesgloseEventosPanel

**Descripcion:** Panel lateral con una lista de tipos de evento, cada uno con un indicador de color, nombre, barra de progreso animada y cantidad. Se calcula el porcentaje de cada tipo sobre el total de eventos del periodo.

**Props interface:**
```
interface DesgloseEventosPanelProps {
    kpis: ProgramaMetricasKpis
    isLoading: boolean
}

interface EventoDesglose {
    tipo: string
    cantidad: number
    color: string
    colorBg: string
}
```

**Logica de calculo:**
- Total de eventos = `totalClicks + totalPageViews + totalSignups + totalConversiones` (Shares no se muestra por separado en MVP)
- Porcentaje de cada tipo = `(cantidad / total) * 100`
- Si total = 0, todas las barras al 0%

**Composicion:**
```tsx
<Card className="bg-[#151525] border-[#334155] col-span-2">
    <CardHeader className="px-5 pt-4 pb-2">
        <CardTitle className="text-base font-semibold text-white">
            Desglose de Eventos
        </CardTitle>
    </CardHeader>
    <CardContent className="px-5 pb-4 flex flex-col gap-3">
        {eventos.map(({ tipo, cantidad, color, colorBg }) => {
            const porcentaje = total > 0 ? (cantidad / total) * 100 : 0
            return (
                <div key={tipo} className="flex items-center justify-between gap-2">
                    {/* Indicador de color circular */}
                    <div
                        className={`w-3 h-3 rounded-full shrink-0 ${colorBg}`}
                        aria-hidden="true"
                    />

                    {/* Nombre del tipo */}
                    <span className="text-sm text-[#94a3b8] flex-1 ml-2 min-w-0">
                        {tipo}
                    </span>

                    {/* Barra de progreso animada */}
                    <div
                        className="flex-1 mx-3 h-1.5 bg-[#1e1e38] rounded-full overflow-hidden"
                        role="progressbar"
                        aria-valuenow={Math.round(porcentaje)}
                        aria-valuemin={0}
                        aria-valuemax={100}
                        aria-label={`${tipo}: ${Math.round(porcentaje)}%`}
                    >
                        <div
                            className={`h-full rounded-full ${color} transition-[width] duration-500 ease-out`}
                            style={{ width: `${porcentaje}%` }}
                        />
                    </div>

                    {/* Cantidad */}
                    <span className="text-sm font-semibold text-white tabular-nums w-10 text-right">
                        {cantidad.toLocaleString('es-ES')}
                    </span>
                </div>
            )
        })}
    </CardContent>
</Card>
```

**Datos de cada tipo de evento:**

| Tipo | Propiedad kpis | Color indicador | Color barra |
|------|----------------|-----------------|-------------|
| Clicks | `totalClicks` | `bg-[#3b82f6]` | `bg-[#3b82f6]` |
| Page views | `totalPageViews` | `bg-[#94a3b8]` | `bg-[#94a3b8]` |
| Signups | `totalSignups` | `bg-[#10b981]` | `bg-[#10b981]` |
| Conversiones | `totalConversiones` | `bg-[#a855f7]` | `bg-[#a855f7]` |

**Animacion de barras:** Al montar el componente con datos, la transicion CSS `transition-[width] duration-500 ease-out` anima desde 0 hasta el valor calculado. Se controla con `useEffect` + `useState` para inicializar en 0 y luego setear al valor real.

**Skeleton:**
```tsx
<Card className="bg-[#151525] border-[#334155] col-span-2">
    <CardHeader className="px-5 pt-4 pb-2">
        <Skeleton className="h-5 w-36 bg-[#1e1e38]" />
    </CardHeader>
    <CardContent className="px-5 pb-4 flex flex-col gap-4" aria-busy="true">
        {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="flex items-center gap-2">
                <Skeleton className="h-3 w-3 rounded-full bg-[#1e1e38]" />
                <Skeleton className="h-3 flex-1 bg-[#1e1e38]" />
                <Skeleton className="h-3 w-10 bg-[#1e1e38]" />
            </div>
        ))}
    </CardContent>
</Card>
```

---

### 4.6 GraficoTemporal

**Descripcion:** Grafico de lineas (Recharts LineChart) con serie temporal de eventos por dia. Muestra 3 lineas: clicks (azul), signups (verde), conversiones (purpura). No muestra page views por defecto.

**Props interface:**
```
interface GraficoTemporalProps {
    eventosPorDia: EventosPorDiaItem[]
    isLoading: boolean
}
```

**Composicion:**
```tsx
<Card className="bg-[#151525] border-[#334155] p-5 mb-10">
    <h3 className="text-base font-semibold text-white mb-4">
        Eventos por dia
    </h3>
    {/* Alternativa textual accesible */}
    <p id="chart-description" className="sr-only">
        Grafico de lineas con la evolucion diaria de clicks, signups y conversiones
        en el periodo seleccionado. Datos disponibles en tabla de ranking superior.
    </p>
    <div
        className="h-64 w-full"
        aria-describedby="chart-description"
        role="img"
        aria-label="Grafico de eventos por dia"
    >
        <ResponsiveContainer width="100%" height="100%">
            <LineChart data={eventosPorDia} margin={{ top: 5, right: 10, left: 0, bottom: 5 }}>
                <CartesianGrid
                    stroke="#334155"
                    strokeDasharray="3 3"
                    opacity={0.5}
                />
                <XAxis
                    dataKey="fecha"
                    tick={{ fill: '#64748b', fontSize: 11 }}
                    tickFormatter={formatFechaEje}  {/* YYYY-MM-DD -> DD/MM */}
                    axisLine={{ stroke: '#334155' }}
                    tickLine={false}
                />
                <YAxis
                    tick={{ fill: '#64748b', fontSize: 11 }}
                    axisLine={false}
                    tickLine={false}
                />
                <Tooltip
                    contentStyle={{
                        background: '#1e1e38',
                        border: '1px solid #334155',
                        borderRadius: '8px',
                        color: '#fff',
                        fontSize: '12px',
                    }}
                    labelStyle={{ color: '#94a3b8', marginBottom: '4px' }}
                    labelFormatter={formatFechaEje}
                />
                <Legend
                    wrapperStyle={{ fontSize: '12px', color: '#94a3b8', paddingTop: '12px' }}
                />
                <Line
                    type="monotone"
                    dataKey="clicks"
                    name="Clicks"
                    stroke="#3b82f6"
                    strokeWidth={2}
                    dot={false}
                    activeDot={{ r: 4, fill: '#3b82f6' }}
                />
                <Line
                    type="monotone"
                    dataKey="signups"
                    name="Signups"
                    stroke="#10b981"
                    strokeWidth={2}
                    dot={false}
                    activeDot={{ r: 4, fill: '#10b981' }}
                />
                <Line
                    type="monotone"
                    dataKey="conversiones"
                    name="Conversiones"
                    stroke="#a855f7"
                    strokeWidth={2}
                    dot={false}
                    activeDot={{ r: 4, fill: '#a855f7' }}
                />
            </LineChart>
        </ResponsiveContainer>
    </div>
</Card>
```

**Funcion formatFechaEje:** Convierte `YYYY-MM-DD` a `DD/MM`.

**Skeleton del grafico:**
```tsx
<Card className="bg-[#151525] border-[#334155] p-5 mb-10">
    <Skeleton className="h-5 w-36 bg-[#1e1e38] mb-4" />
    <Skeleton className="h-64 w-full bg-[#1e1e38] rounded-lg" aria-busy="true" />
</Card>
```

**Empty state (sin datos en el periodo):**
```tsx
<div className="h-64 flex items-center justify-center">
    <p className="text-sm text-[#64748b] text-center">
        No hay datos para el periodo seleccionado
    </p>
</div>
```

**Responsive:** En mobile (`< md`), la altura del grafico se reduce a `h-48`.

---

### 4.7 ProgramaMetricasTab (Composicion Principal)

**Descripcion:** Componente raiz que orquesta todos los subcomponentes de la pestana Metricas. Maneja el estado del filtro de fechas, la query a la API y la distribucion del layout.

**Props interface:**
```
interface ProgramaMetricasTabProps {
    programaId: string
    campaniaId: string
    campaniaTitulo: string
    programaTitulo: string
}
```

**Layout de la composicion:**
```tsx
<div className="space-y-0">
    {/* Breadcrumb */}
    <BreadcrumbMetricas
        campaniaId={campaniaId}
        campaniaTitulo={campaniaTitulo}
        programaId={programaId}
        programaTitulo={programaTitulo}
    />

    {/* Titulo */}
    <h1 className="text-2xl font-bold text-white mb-6">
        Metricas: {programaTitulo}
    </h1>

    {/* Filtro de fechas */}
    <FiltroFechas
        onAplicar={handleAplicarFiltro}
        onLimpiar={handleLimpiarFiltro}
        isLoading={isFetching}
        filtroActivo={filtroActivo}
    />

    {/* Estado de error */}
    {isError && (
        <Alert
            variant="destructive"
            role="alert"
            className="bg-red-950/30 border-red-800/50 mb-6"
        >
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>No se pudieron cargar las metricas</AlertTitle>
            <AlertDescription className="flex items-center gap-3 mt-2">
                <span>Verifica tu conexion e intenta de nuevo.</span>
                <Button
                    variant="outline"
                    size="sm"
                    onClick={() => refetch()}
                    className="border-red-800/50 text-red-400 hover:bg-red-950/50"
                >
                    Reintentar
                </Button>
            </AlertDescription>
        </Alert>
    )}

    {/* KPI Cards */}
    {isLoading || !data ? (
        <KpiCardsGridSkeleton />
    ) : (
        <KpiCardsGrid kpis={data.kpis} isLoading={false} />
    )}

    {/* Ranking + Desglose */}
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
        <RankingPromotoresTable
            promotores={data?.rankingPromotores ?? []}
            isLoading={isLoading}
            sortColumn={sortColumn}
            sortDirection={sortDirection}
            onSort={handleSort}
        />
        <DesgloseEventosPanel
            kpis={data?.kpis ?? defaultKpis}
            isLoading={isLoading}
        />
    </div>

    {/* Grafico temporal */}
    <GraficoTemporal
        eventosPorDia={data?.eventosPorDia ?? []}
        isLoading={isLoading}
    />
</div>
```

---

## 5. Formularios

### 5.1 FiltroFechas

**Campos:**

| Campo | Componente shadcn | Tipo HTML | Validacion Zod | Visual de error |
|-------|-------------------|-----------|----------------|-----------------|
| `fechaDesde` | `<Input>` | `type="date"` | Requerido si `fechaHasta` presente; <= `fechaHasta` | `text-xs text-[#ef4444]` bajo input |
| `fechaHasta` | `<Input>` | `type="date"` | `max` = hoy; >= `fechaDesde` | Borde `border-[#ef4444]` cuando error |

**Integracion con react-hook-form:**
- Schema: `filtroFechasSchema` (de `@shared/schemas`)
- `resolver: zodResolver(filtroFechasSchema)`
- `onSubmit`: llama a `onAplicar(data)` del prop

**Estados de los inputs:**

| Estado | Clases del Input |
|--------|-----------------|
| Default | `bg-[#0f0f1f] border-[#334155] text-white` |
| Focus | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` |
| Error | `border-[#ef4444]` + mensaje `text-xs text-[#ef4444]` |
| Disabled | `opacity-50 cursor-not-allowed` |

---

## 6. Tabla de Ranking

### 6.1 RankingPromotoresTable - Columnas

| Header | Clave datos | Ancho | Alineacion | Ordenable | Color valor |
|--------|-------------|-------|------------|-----------|-------------|
| # | Posicion (1-N) | `w-10` | Left | No | `text-[#64748b]` |
| Promotor | `promotorNombre` + Avatar | `flex-1` | Left | No | `text-white` |
| Clicks | `clicks` | `w-20` | Left | Si | `text-[#3b82f6]` |
| Conv. | `conversiones` | `w-16` | Left | Si | `text-[#a855f7]` |
| Valor | `valorGenerado` | `w-24` | Left | Si | `text-[#f59e0b]` |
| Comision | `comisionAcumulada` | `w-24` | Left | Si | `text-[#10b981]` |

**Columnas ocultas en mobile:** Valor (`hidden md:table-cell`) y Comision (`hidden md:table-cell`).

---

## 7. Estados Globales de la Pantalla

### 7.1 Loading inicial

- Todos los skeletons activos (`aria-busy="true"`)
- FiltroFechas visible y editable (no en skeleton)
- Boton "Aplicar" habilitado (el usuario puede configurar el filtro antes de que carguen los datos)

### 7.2 Loading con filtro aplicado

- Skeletons activos en KPI cards, tabla, desglose y grafico
- FiltroFechas permanece visible
- Boton "Aplicar" muestra `<Loader2 className="w-4 h-4 animate-spin mr-1.5">`

### 7.3 Default con datos

- Todos los componentes rellenos con datos reales
- Animaciones de entrada: fade in + slide up en KPI cards
- Tabla ordenada por conversiones DESC por defecto

### 7.4 Empty state (sin datos en el periodo)

- KPI cards con valores en 0 (se renderizan igual, no skeleton)
- Tabla muestra mensaje centrado: "Sin promotores con actividad en este periodo"
- Desglose muestra todas las barras a 0%
- Grafico muestra ejes vacios con texto centrado: "No hay datos para el periodo seleccionado"

### 7.5 Error de carga

```tsx
<Alert
    variant="destructive"
    role="alert"
    className="bg-red-950/30 border-red-800/50 mb-6"
>
    <AlertCircle className="h-4 w-4 text-red-400" />
    <AlertTitle className="text-red-400">No se pudieron cargar las metricas</AlertTitle>
    <AlertDescription className="flex flex-wrap items-center gap-3 mt-2 text-red-300">
        <span>Verifica tu conexion e intenta de nuevo.</span>
        <Button
            variant="outline"
            size="sm"
            onClick={refetch}
            className="border-red-800/50 text-red-400 hover:bg-red-950/50"
        >
            Reintentar
        </Button>
    </AlertDescription>
</Alert>
```

### 7.6 Filtro activo

- Badge visible junto al filtro con el rango formateado: `"Filtrando: 01 Mar - 31 May 2026"`
- Boton "Limpiar" visible (ghost, `text-[#64748b]`)
- Al limpiar: quitar params de URL, refetch sin filtro, ocultar badge y boton

---

## 8. Responsive Design

| Breakpoint | Layout | Cambios especificos |
|------------|--------|---------------------|
| Mobile `< md (768px)` | Stack vertical full width | Sidebar colapsado (hamburger). KPI primarias: `grid-cols-2`. KPI secundarias: `grid-cols-1`. Ranking + Desglose: `grid-cols-1` (apilados). Grafico: `h-48`. Tabla ranking: ocultar columnas Valor y Comision. FiltroFechas: `flex-col` en mobile. |
| Tablet `md-lg (768-1024px)` | Sidebar visible, 2 columnas | KPI primarias: `grid-cols-2`. KPI secundarias: `grid-cols-2`. Ranking + Desglose: `grid-cols-2` (ranking toma mas espacio). Grafico: `h-64`. |
| Desktop `> lg (1024px)` | Layout completo | KPI primarias: `grid-cols-4`. Ranking + Desglose: `grid-cols-5` (ranking 3 cols, desglose 2 cols). Grafico: `h-64`. |

**Clases Tailwind para el grid de ranking + desglose:**
```tsx
className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8"
```

**Col-span del ranking en desktop:**
```tsx
className="col-span-1 md:col-span-1 lg:col-span-3"
```

**Col-span del desglose en desktop:**
```tsx
className="col-span-1 md:col-span-1 lg:col-span-2"
```

---

## 9. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste texto | Texto blanco `#fff` sobre fondo `#151525`: ratio ~14:1 (AAA). Labels grises `#94a3b8` sobre `#151525`: ratio ~5.4:1 (AA). |
| Contraste badges | Texto `#3b82f6` sobre `bg-blue-950`: ratio ~4.6:1 (AA). Resto de colores de badge cumplen minimo 3:1. |
| Focus ring | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en inputs, botones y links |
| Labels de formulario | `<Input id="fechaDesde">` vinculado a `<label htmlFor="fechaDesde">`. Inputs con `aria-label` cuando no hay label visible. |
| Tabla de ranking | `<thead>` con `<th scope="col">`. Columnas ordenables con `aria-sort="ascending"/"descending"/"none"`. `<Table aria-label="...">` |
| Grafico temporal | `role="img"` + `aria-label` en contenedor. `<p className="sr-only">` con descripcion textual alternativa. |
| Skeletons / Loading | `aria-busy="true"` en contenedores mientras cargan. |
| Mensajes de error | `role="alert"` en el `<Alert>` de error para anuncio inmediato a lectores de pantalla. |
| Barras de progreso | `role="progressbar"` + `aria-valuenow`, `aria-valuemin`, `aria-valuemax`, `aria-label` en cada barra de desglose. |
| Boton X del badge filtro | `aria-label="Quitar filtro de fechas"` en el boton `<X>` del badge. |
| Validacion de fechas | `role="alert"` en el mensaje de error inline bajo los inputs. |

---

## 10. Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| KPI cards (skeleton -> contenido) | `animate-in fade-in slide-in-from-bottom-2` | `300ms` | Al resolver la query y reemplazar skeleton |
| Barra de progreso en desglose | `transition-[width] ease-out` desde `0%` al valor real | `500ms` | Al montar el componente con datos (via `useEffect`) |
| Filas de tabla al reordenar | `transition-all` | `200ms` | Click en cabecera ordenable |
| Hover sobre fila de tabla | `bg-[#1e1e38]` via `transition-colors` | `150ms` | Hover |
| Badge filtro activo (aparicion) | `animate-in fade-in` | `150ms` | Al aplicar filtro |
| Boton "Aplicar" (loading) | `<Loader2 animate-spin>` icono giratorio | Continuo | Mientras `isFetching` |

---

## 11. Ubicacion de Archivos

```
src/admin/src/
└── app/
    └── (dashboard)/
        └── campanias/
            └── [id]/
                └── crowdpromotion/
                    └── programas/
                        └── [programaId]/
                            └── metricas/
                                └── page.tsx              {Page principal - Server Component wrapper}
                                └── components/
                                    ├── ProgramaMetricasTab.tsx    {Composicion principal "use client"}
                                    ├── FiltroFechas.tsx           {Formulario de filtro "use client"}
                                    ├── KpiCardsGrid.tsx           {Grid de KPI cards "use client"}
                                    ├── RankingPromotoresTable.tsx {Tabla ordenable "use client"}
                                    ├── DesgloseEventosPanel.tsx   {Panel con barras "use client"}
                                    └── GraficoTemporal.tsx        {Recharts LineChart "use client"}
```

**Hook de datos:**
```
src/admin/src/hooks/
└── use-programa-metricas.ts    {useQuery con QUERY_KEYS.crowdpromotion.metricas.programa}
```

---

## 12. Checklist UI

- [ ] Breadcrumb completo con `aria-current="page"` en "Metricas"
- [ ] FiltroFechas con inputs date integrados con `react-hook-form` + `filtroFechasSchema`
- [ ] Validacion visual: error inline cuando `fechaDesde > fechaHasta`
- [ ] Badge de filtro activo con boton X accesible
- [ ] Boton "Limpiar" visible solo cuando hay filtro activo
- [ ] Boton "Aplicar" con spinner `<Loader2>` durante `isFetching`
- [ ] 4 KPI cards primarias con iconos Lucide correctos y colores del design system
- [ ] 2 KPI cards secundarias (Tasa + Comisiones) con layout `flex items-center`
- [ ] Valor monetario en `text-[#f59e0b]` con sufijo EUR en `text-[#64748b]`
- [ ] Tabla con `<th scope="col">` y `aria-sort` en columnas ordenables
- [ ] Avatar con fallback de iniciales (max 2 chars) en cada fila del ranking
- [ ] Icono `<ArrowUpDown>` en cabeceras ordenables, color `#a855f7` cuando activo
- [ ] Columnas Valor y Comision ocultas en mobile (`hidden md:table-cell`)
- [ ] Barras de progreso con animacion `0 -> valor` en 500ms al montar
- [ ] `role="progressbar"` + `aria-valuenow` en cada barra de desglose
- [ ] Grafico Recharts con `<ResponsiveContainer>` y 3 lineas de colores correctos
- [ ] Tooltip del grafico con `contentStyle` dark (`background: '#1e1e38'`)
- [ ] Eje X formateado como `DD/MM` (conversion desde `YYYY-MM-DD`)
- [ ] Alternativa textual `sr-only` para el grafico
- [ ] Skeleton activo para cada seccion durante loading (`aria-busy="true"`)
- [ ] Empty state en tabla: texto "Sin promotores con actividad en este periodo"
- [ ] Empty state en grafico: texto centrado dentro del area `h-64`
- [ ] `<Alert variant="destructive">` con boton "Reintentar" en caso de error
- [ ] `role="alert"` en el alert de error y en mensajes de validacion
- [ ] Focus ring `ring-[#a855f7]` visible en todos los elementos interactivos
- [ ] KPI grid: `grid-cols-2 lg:grid-cols-4` (2 cols en mobile/tablet, 4 en desktop)
- [ ] Ranking + Desglose: `grid-cols-1 md:grid-cols-2 lg:grid-cols-5`
- [ ] Grafico: `h-48` en mobile, `h-64` en tablet/desktop
