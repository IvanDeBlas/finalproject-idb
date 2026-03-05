# Diseno UI: Hacer Backing (Admin)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/admin (Dashboard artista)

## 1. Resumen

- Componentes shadcn: 9 (Card, Table, Badge, Input, Select, Button, Skeleton, Alert, Separator)
- Composiciones custom: 4 (StatsGrid, FiltersRow, BackingsTable, EmptyState)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)
- Pagina unica: Backings Management (Artist View)

## 2. Paleta de Colores (del proyecto Admin)

El proyecto Admin usa el dark theme de shadcn/ui con CSS variables.

| Uso | Variable CSS | Aplicacion |
|-----|--------------|------------|
| Background principal | `bg-background` | hsl(224 71% 4%) - #0a0f1a |
| Background cards | `bg-card` | hsl(224 71% 8%) - #0f1729 |
| Background hover | `bg-muted` | hsl(217 33% 17%) - #1e293b |
| Text principal | `text-foreground` | hsl(213 31% 91%) - #e2e8f0 |
| Text secundario | `text-muted-foreground` | hsl(215 20% 65%) - #94a3b8 |
| Primary (acciones) | `bg-primary` | hsl(262 83% 58%) - #a855f7 (purple) |
| Primary hover | `hover:bg-primary/90` | Opacity 90% |
| Border normal | `border-border` | hsl(215 28% 17%) - #1e293b |
| Border primary | `border-primary` | hsl(262 83% 58%) - #a855f7 |
| Success | `bg-green-500/20` | Green con opacity 20% |
| Warning | `bg-yellow-500/20` | Yellow con opacity 20% |
| Destructive | `bg-destructive` | hsl(0 84% 60%) - #ef4444 (red) |

**Nota:** Se reutilizan los colores existentes del dashboard. No se requieren colores especificos de backing porque es vista simplificada.

## 3. Componentes por Screen

### 3.1 Backings Management (Artist View)

**Ruta:** `/dashboard/campanias/{id}/backings`

#### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]      │  [HEADER]                                       │
│                 ├─────────────────────────────────────────────────┤
│  Dashboard      │  ← Volver a Campania                            │
│  Campanias      │                                                  │
│  Perfil         │  Backings: "Nuevo Album Debut"                  │
│                 │  Aportes recibidos de tus fans                  │
│  [Avatar]       │                                                  │
│  Juan Perez     │  ┌──────────┐  ┌──────────┐  ┌──────────┐       │
│  artista        │  │ Total    │  │ Total    │  │ Promedio │       │
│                 │  │ Raised   │  │ Backers  │  │ Amount   │       │
│                 │  │ €8,450   │  │ 127      │  │ €66.53   │       │
│                 │  └──────────┘  └──────────┘  └──────────┘       │
│                 │                                                  │
│                 │  [Search]  [Filter by Reward ▼]  [Export CSV]   │
│                 │                                                  │
│                 │  ┌────────────────────────────────────────────┐  │
│                 │  │ Backer    Amount  Reward         Date   ... │  │
│                 │  ├────────────────────────────────────────────┤  │
│                 │  │ Carlos M. €75     Disco firmado  15 Ene ... │  │
│                 │  │ Ana G.    €50     Vinilo limited 14 Ene ... │  │
│                 │  │ [Anonimo] €25     CD Fisico      13 Ene ... │  │
│                 │  │ ...                                        │  │
│                 │  └────────────────────────────────────────────┘  │
│                 │                                                  │
│                 │  [← Anterior]   1  2  3  [Siguiente →]          │
│                 │                                                  │
└────────────────────────────────────────────────────────────────────┘
```

#### Componentes

**PageHeader**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Breadcrumb Back Button | Button variant="ghost" | size="sm", gap-2 con ArrowLeft icon |
| Page Title | h1 | text-2xl md:text-3xl font-bold text-foreground |
| Subtitle | p | text-sm text-muted-foreground mt-1 |

**Composicion:**
```tsx
<div className="mb-6">
  <Button
    variant="ghost"
    onClick={() => router.push(`/dashboard/campanias/${campaniaId}`)}
    className="flex items-center gap-2 text-muted-foreground hover:text-foreground mb-4 transition"
  >
    <ArrowLeft className="w-4 h-4" />
    Volver a Campania
  </Button>
  <h1 className="text-2xl md:text-3xl font-bold text-foreground">
    Backings: {campaniaTitulo}
  </h1>
  <p className="text-sm text-muted-foreground mt-1">
    Aportes recibidos de tus fans
  </p>
</div>
```

**StatsGrid**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Grid Container | div | grid grid-cols-1 md:grid-cols-3 gap-4 mb-6 |
| Stat Card | Card | bg-card border-border |
| Stat Header | CardHeader | flex-row items-center justify-between pb-2 |
| Stat Title | CardTitle | text-sm font-medium text-muted-foreground |
| Stat Icon | Icon (Lucide) | h-4 w-4 text-muted-foreground |
| Stat Value | div | text-2xl font-bold text-foreground |

**Composicion:**
```tsx
<div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
  <Card>
    <CardHeader className="flex flex-row items-center justify-between pb-2">
      <CardTitle className="text-sm font-medium text-muted-foreground">
        Total Recaudado
      </CardTitle>
      <EuroIcon className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold text-foreground">€{totalRecaudado}</div>
    </CardContent>
  </Card>

  <Card>
    <CardHeader className="flex flex-row items-center justify-between pb-2">
      <CardTitle className="text-sm font-medium text-muted-foreground">
        Total Backers
      </CardTitle>
      <Users className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold text-foreground">{totalBackers}</div>
    </CardContent>
  </Card>

  <Card>
    <CardHeader className="flex flex-row items-center justify-between pb-2">
      <CardTitle className="text-sm font-medium text-muted-foreground">
        Promedio por Aporte
      </CardTitle>
      <TrendingUp className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold text-foreground">€{promedioAporte}</div>
    </CardContent>
  </Card>
</div>
```

**Nota:** Reutilizar componente `StatsCard` existente en `src/admin/src/components/dashboard/stats-card.tsx`.

**FiltersRow**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | div | flex flex-col md:flex-row gap-3 mb-4 |
| Search Input | Input | placeholder="Buscar por nombre...", type="search" |
| Search Icon | Icon (Search) | absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground |
| Filter Select | Select | placeholder="Todas las recompensas" |
| Export Button | Button variant="outline" | border-primary text-primary hover:bg-primary/10 |
| Export Icon | Icon (Download) | w-4 h-4 mr-2 |

**Composicion:**
```tsx
<div className="flex flex-col md:flex-row gap-3 mb-4">
  {/* Search Input */}
  <div className="relative flex-1 md:max-w-xs">
    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
    <Input
      type="search"
      placeholder="Buscar por nombre..."
      value={searchTerm}
      onChange={(e) => setSearchTerm(e.target.value)}
      className="pl-9 bg-card border-border text-foreground"
    />
  </div>

  {/* Filter by Reward */}
  <Select value={rewardFilter} onValueChange={setRewardFilter}>
    <SelectTrigger className="w-full md:w-[200px] bg-card border-border text-foreground">
      <SelectValue placeholder="Todas las recompensas" />
    </SelectTrigger>
    <SelectContent>
      <SelectItem value="all">Todas las recompensas</SelectItem>
      {rewards.map(reward => (
        <SelectItem key={reward.id} value={reward.id}>
          {reward.nombre}
        </SelectItem>
      ))}
    </SelectContent>
  </Select>

  {/* Export CSV Button */}
  <Button
    variant="outline"
    onClick={handleExportCSV}
    className="border-primary text-primary hover:bg-primary/10"
  >
    <Download className="w-4 h-4 mr-2" />
    Export CSV
  </Button>
</div>
```

**BackingsTable**

**Nota:** shadcn/ui Table component NO está instalado. Se debe instalar primero con:
```bash
npx shadcn@latest add table
```

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Table Container | Table | - |
| Table Header | TableHeader | bg-card border-b border-border |
| Table Header Cell | TableHead | text-muted-foreground font-medium text-xs uppercase |
| Table Body | TableBody | - |
| Table Row | TableRow | border-b border-border hover:bg-muted/50 transition |
| Table Cell | TableCell | text-foreground text-sm py-3 |
| Anonymous Text | span | text-muted-foreground italic |
| Amount Text | span | font-semibold text-foreground |
| Status Badge | Badge | variant="secondary" bg-green-500/20 text-green-400 |
| Pagination Container | div | flex items-center justify-center gap-2 mt-4 |
| Pagination Button | Button | variant="outline" size="sm" |

**Composicion:**
```tsx
<div className="border border-border rounded-lg overflow-hidden">
  <Table>
    <TableHeader>
      <TableRow className="bg-card border-b border-border">
        <TableHead className="text-muted-foreground font-medium text-xs uppercase">
          Backer
        </TableHead>
        <TableHead className="text-muted-foreground font-medium text-xs uppercase">
          Amount
        </TableHead>
        <TableHead className="text-muted-foreground font-medium text-xs uppercase">
          Reward
        </TableHead>
        <TableHead className="text-muted-foreground font-medium text-xs uppercase">
          Date
        </TableHead>
        <TableHead className="text-muted-foreground font-medium text-xs uppercase text-center">
          Status
        </TableHead>
      </TableRow>
    </TableHeader>
    <TableBody>
      {backings.map(backing => (
        <TableRow
          key={backing.id}
          className="border-b border-border hover:bg-muted/50 transition cursor-pointer"
          onClick={() => handleRowClick(backing.id)}
        >
          <TableCell className="text-foreground text-sm py-3">
            {backing.esAnonimo ? (
              <span className="text-muted-foreground italic">Anonimo</span>
            ) : (
              backing.nombreBacker
            )}
          </TableCell>
          <TableCell className="text-foreground text-sm py-3">
            <span className="font-semibold">€{backing.monto.toFixed(2)}</span>
          </TableCell>
          <TableCell className="text-foreground text-sm py-3">
            {backing.rewardNombre || "-"}
          </TableCell>
          <TableCell className="text-foreground text-sm py-3">
            {formatDate(backing.fechaCreacion)}
          </TableCell>
          <TableCell className="text-center py-3">
            <Badge variant="secondary" className="bg-green-500/20 text-green-400 border-green-500/50">
              Confirmado
            </Badge>
          </TableCell>
        </TableRow>
      ))}
    </TableBody>
  </Table>
</div>

{/* Pagination */}
<div className="flex items-center justify-center gap-2 mt-4">
  <Button
    variant="outline"
    size="sm"
    onClick={() => setPage(page - 1)}
    disabled={page === 1}
  >
    Anterior
  </Button>
  <span className="text-sm text-muted-foreground">
    Pagina {page} de {totalPages}
  </span>
  <Button
    variant="outline"
    size="sm"
    onClick={() => setPage(page + 1)}
    disabled={page === totalPages}
  >
    Siguiente
  </Button>
</div>
```

**EmptyState**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | Card | p-12 text-center |
| Icon Container | div | mx-auto mb-4 |
| Icon | Icon (PackageOpen) | w-20 h-20 text-muted-foreground |
| Title | h3 | text-xl font-bold text-foreground mb-2 |
| Description | p | text-muted-foreground mb-6 |
| CTA Button | Button | bg-primary text-white hover:bg-primary/90 |

**Composicion:**
```tsx
<Card className="p-12 text-center">
  <div className="mx-auto mb-4">
    <PackageOpen className="w-20 h-20 text-muted-foreground mx-auto" />
  </div>
  <h3 className="text-xl font-bold text-foreground mb-2">
    No hay backings aun
  </h3>
  <p className="text-muted-foreground mb-6">
    Cuando recibas tus primeros aportes, apareceran aqui.
  </p>
  <Button onClick={() => router.push(`/dashboard/campanias/${campaniaId}`)}>
    Volver a Campania
  </Button>
</Card>
```

**Nota:** Reutilizar componente `EmptyState` existente en `src/admin/src/app/(dashboard)/campanias/components/shared/EmptyState.tsx`.

## 4. Formularios

**No aplica** - Esta pantalla es de solo lectura. No hay formularios.

## 5. Tablas (Admin)

### 5.1 BackingsTable

**Columnas:**

| Header | Width | Align | Sortable |
|--------|-------|-------|----------|
| Backer | flex-1 (min-w-[120px]) | left | No (futuro: Si) |
| Amount | w-[100px] | left | No (futuro: Si) |
| Reward | flex-1 (min-w-[150px]) | left | No (futuro: Si) |
| Date | w-[120px] | left | No (futuro: Si) |
| Status | w-[100px] | center | No |

**Componentes:**
- Table, TableHeader, TableBody, TableRow, TableCell, TableHead (shadcn/ui)
- Badge para status (variant="secondary" con custom bg/text colors)

**Estados de Status:**

| Status | Badge Variant | Custom Classes | Texto |
|--------|---------------|----------------|-------|
| Confirmado | secondary | bg-green-500/20 text-green-400 border-green-500/50 | Confirmado |

**Nota MVP:** Solo hay un estado "Confirmado" porque el MVP no procesa pagos reales (ver contracts.md). Todos los backings se crean con `EstadoPedidoId = 3` (COMPLETADO).

**Responsive Behavior:**

| Breakpoint | Cambios |
|------------|---------|
| < md (mobile) | Table scroll horizontal (overflow-x-auto) |
| >= md (desktop) | Table normal con columnas visibles |

## 6. Dialogos/Modales

**No aplica para MVP** - No hay modales de detalle de backing ni edicion.

**Futuro (post-MVP):** Considerar modal de detalle al hacer click en row:
- Mostrar datos completos del backing (mensaje, datos de envio si reward fisico, etc.)
- Acciones: Ver perfil del backer, marcar como enviado (si reward fisico)

## 7. Feedback y Estados

### Loading
- Skeleton cards (3 skeletons) para stats grid mientras carga
- Skeleton table (3-4 rows) mientras carga backings
- Button con spinner en Export CSV mientras procesa

### Error
- Alert variant="destructive" si falla carga de backings
- Toast error si falla export CSV

### Success
- Toast success "CSV descargado" tras export exitoso

### Empty State
- Card con icon PackageOpen, texto "No hay backings aun", boton "Volver a Campania"

## 8. Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| < sm (mobile) | Stats grid 1 col, filters stack vertical, table horizontal scroll |
| sm-md (tablet) | Stats grid 1 col, filters stack vertical, table horizontal scroll |
| >= md (desktop) | Stats grid 3 cols, filters horizontal (flex-row), table normal |
| >= lg (desktop grande) | Layout completo con sidebar visible |

**Clases Tailwind:**
```tsx
// Stats Grid
className="grid grid-cols-1 md:grid-cols-3 gap-4"

// Filters Row
className="flex flex-col md:flex-row gap-3"

// Search Input
className="w-full md:max-w-xs"

// Table Container (mobile scroll)
className="overflow-x-auto"

// Page Container
className="container max-w-5xl mx-auto py-8 px-4 md:px-6"
```

## 9. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Search input label | aria-label="Buscar backings por nombre de backer" |
| Table semantic | Table con thead, tbody, th, td |
| Table headers scope | th scope="col" |
| Row clickable | role="button" tabIndex={0} onKeyPress handler |
| Status badge | role="status" aria-label="Estado: Confirmado" |
| Pagination buttons | aria-label="Pagina anterior/siguiente", disabled cuando no aplica |
| Focus visible | Ring de 2px en primary en todos interactivos |
| Color contrast | Verificado: text-foreground (#e2e8f0) sobre bg-card (#0f1729) = 12:1 (AAA) |
| Empty state | Heading hierarchy correcta (h1 > h3) |

## 10. Iconos (Lucide React)

| Icono | Uso |
|-------|-----|
| ArrowLeft | Back button |
| EuroIcon (o DollarSign) | Total Recaudado stat |
| Users | Total Backers stat |
| TrendingUp | Promedio Aporte stat |
| Search | Search input |
| Filter | Filter by reward (opcional) |
| Download | Export CSV button |
| PackageOpen | Empty state |
| ChevronLeft | Pagination previous |
| ChevronRight | Pagination next |

## 11. Checklist

- [ ] Table component de shadcn/ui instalado (`npx shadcn@latest add table`)
- [ ] Componente StatsCard reutilizado de dashboard existente
- [ ] Componente EmptyState reutilizado de campanias
- [ ] Search input con icon left y debounce 300ms
- [ ] Filter select carga rewards de la campania
- [ ] Export CSV genera archivo CSV con headers y data
- [ ] Table responsive con scroll horizontal en mobile
- [ ] Anonymous text en italic gray
- [ ] Status badge verde para "Confirmado"
- [ ] Pagination funcional con estado local
- [ ] Loading skeletons para stats y table
- [ ] Error handling con toast y alert
- [ ] Empty state cuando no hay backings
- [ ] Click row abre modal detalle (futuro)
- [ ] Focus states en todos interactivos
- [ ] ARIA labels en search y pagination

## 12. Dependencias de Componentes shadcn/ui

### Componentes ya instalados:
- [x] Card
- [x] Button
- [x] Input
- [x] Select
- [x] Badge
- [x] Skeleton
- [x] Alert

### Componentes a instalar:
- [ ] Table (REQUERIDO - ejecutar `npx shadcn@latest add table`)

## 13. Componentes Custom a Crear

| Componente | Ubicacion | Responsabilidad |
|------------|-----------|-----------------|
| BackingsPage | `app/(dashboard)/campanias/[id]/backings/page.tsx` | Container principal, layout, state management |
| StatsCard | (ya existe) `components/dashboard/stats-card.tsx` | Reutilizar |
| EmptyState | (ya existe) `campanias/components/shared/EmptyState.tsx` | Reutilizar |

**Nota:** NO crear componentes separados para tabla, filtros, etc. Todo inline en `page.tsx` para simplicidad del MVP.

## 14. Hooks a Usar

| Hook | Ubicacion | Proposito |
|------|-----------|-----------|
| `useCampaniaBackings` | `hooks/use-backings.ts` (CREAR) | GET /api/campanias/{id}/backings con paginacion |
| `useCampaniaStats` | `hooks/use-backings.ts` (CREAR) | GET /api/campanias/{id}/stats (opcional, calcular client-side) |
| `useDebounce` | `hooks/use-debounce.ts` (CREAR o reutilizar) | Debounce search input 300ms |
| `useState` | React | Estado local (search, filter, page) |
| `useRouter` | Next.js | Navegacion back to campania |
| `useParams` | Next.js | Obtener campaniaId de URL |

## 15. Estado Local (useState)

```tsx
const [searchTerm, setSearchTerm] = useState("")
const [rewardFilter, setRewardFilter] = useState<string>("all")
const [page, setPage] = useState(1)
const [pageSize] = useState(20)
```

**Nota:** No usar estado global (Zustand) para esta pantalla. Estado local es suficiente.

## 16. Queries (TanStack Query)

```tsx
const { data: backingsData, isLoading } = useQuery({
  queryKey: ["backings", "campania", campaniaId, searchTerm, rewardFilter, page],
  queryFn: () => getCampaniaBackings({
    campaniaId,
    searchTerm: searchTerm || undefined,
    rewardFilter: rewardFilter !== "all" ? rewardFilter : undefined,
    page,
    pageSize,
  }),
})

const { data: rewards } = useQuery({
  queryKey: ["rewards", campaniaId],
  queryFn: () => getRewards(campaniaId),
})
```

## 17. Export CSV Funcionalidad

**Implementacion:**

```tsx
const handleExportCSV = async () => {
  try {
    const data = await getCampaniaBackings({
      campaniaId,
      page: 1,
      pageSize: 9999, // Get all
    })

    const csv = [
      ["Backer", "Amount", "Reward", "Date", "Status"].join(","),
      ...data.items.map(b => [
        b.esAnonimo ? "Anonimo" : b.nombreBacker,
        b.monto.toFixed(2),
        b.rewardNombre || "-",
        formatDate(b.fechaCreacion),
        "Confirmado"
      ].join(","))
    ].join("\n")

    const blob = new Blob([csv], { type: "text/csv" })
    const url = URL.createObjectURL(blob)
    const a = document.createElement("a")
    a.href = url
    a.download = `backings-${campaniaId}-${Date.now()}.csv`
    a.click()
    URL.revokeObjectURL(url)

    toast.success("CSV descargado exitosamente")
  } catch (error) {
    toast.error("Error al exportar CSV")
  }
}
```

## 18. Notas de Implementacion

### 18.1 Search Debounce

```tsx
const debouncedSearch = useDebounce(searchTerm, 300)

// Use debouncedSearch in query key instead of searchTerm
queryKey: ["backings", "campania", campaniaId, debouncedSearch, ...]
```

### 18.2 Filter by Reward

- Si `rewardFilter === "all"` → no enviar filtro al backend
- Si `rewardFilter === "{rewardId}"` → enviar rewardId al backend

**Backend debe soportar:**
```
GET /api/campanias/{id}/backings?rewardId={guid}
```

### 18.3 Anonymous Display

```tsx
{backing.esAnonimo ? (
  <span className="text-muted-foreground italic">Anonimo</span>
) : (
  <span className="text-foreground">{backing.nombreBacker}</span>
)}
```

### 18.4 Date Formatting

```tsx
import { format } from "date-fns"
import { es } from "date-fns/locale"

const formatDate = (dateString: string) => {
  return format(new Date(dateString), "dd MMM yy", { locale: es })
}

// Output: "15 Ene 25"
```

### 18.5 Stats Calculation (Client-Side)

Si no hay endpoint `/stats`, calcular client-side:

```tsx
const totalRecaudado = backings.reduce((sum, b) => sum + b.monto, 0)
const totalBackers = backings.length
const promedioAporte = totalRecaudado / totalBackers || 0
```

**Nota:** Preferir endpoint backend para stats si esta disponible (evita cargar todos los backings).

## 19. Mejoras Post-MVP

**No implementar en MVP:**

1. **Sortable columns** - Click en header de tabla para ordenar
2. **Modal de detalle** - Click en row para ver mensaje, datos de envio
3. **Marcar como enviado** - Accion para rewards fisicos
4. **Graficas de stats** - Chart.js o Recharts para visualizaciones
5. **Filtro por fecha** - DateRangePicker para filtrar por rango
6. **Bulk actions** - Checkbox multi-select para acciones masivas
7. **Busqueda avanzada** - Filtros combinados (monto, fecha, reward, etc.)

## 20. Archivo de Prueba (Ejemplo de Data)

```tsx
// Mock data para desarrollo
const mockBackings: BackingPublicDto[] = [
  {
    id: "1",
    nombreBacker: "Carlos Mendez",
    monto: 75,
    rewardNombre: "Disco firmado + Poster",
    mensaje: "Mucha suerte con el proyecto!",
    fechaCreacion: "2026-02-13T10:30:00Z",
  },
  {
    id: "2",
    nombreBacker: "Ana Garcia",
    monto: 50,
    rewardNombre: "Vinilo edicion limitada",
    mensaje: null,
    fechaCreacion: "2026-02-13T09:15:00Z",
  },
  {
    id: "3",
    nombreBacker: "Anonimo",
    monto: 25,
    rewardNombre: "CD Fisico",
    mensaje: null,
    fechaCreacion: "2026-02-13T08:45:00Z",
  },
]
```

---

**Fin del plan de diseno UI para Backings Management (Admin).**
