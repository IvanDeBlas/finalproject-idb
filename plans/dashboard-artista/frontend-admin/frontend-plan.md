# Plan Frontend: Dashboard Artista (Admin)

**Fecha:** 2026-02-14
**Feature:** dashboard-artista (US-05)
**Target:** src/admin (Next.js 14, App Router)

---

## 1. Resumen

- **Screens:** 3 (Dashboard enhanced, Campania Detail Stats, Backings List Full)
- **Componentes nuevos:** 8
- **Componentes a modificar:** 2 (Dashboard page, StatsCard)
- **Hooks:** 4 nuevos (useQuery)
- **Services:** 1 nuevo (dashboard.service.ts)
- **Pages a crear:** 0 (las rutas ya existen, se mejoran)
- **Pages a modificar:** 3

---

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       ├── dashboard/
│       │   ├── page.tsx                        # MODIFICAR - Enhanced stats
│       │   └── components/                     # NUEVO
│       │       ├── MisCampaniasCard.tsx       # NUEVO - Enhanced campaigns list
│       │       └── CampaniaProgressItem.tsx    # NUEVO - Campaign item con avatar
│       │
│       └── campanias/
│           └── [id]/
│               ├── page.tsx                    # MODIFICAR - Stats view
│               └── backings/
│                   ├── page.tsx                # MODIFICAR - Full list con filtros
│                   └── components/
│                       ├── BackingStatsGrid.tsx    # YA EXISTE - usar
│                       ├── BackingsTable.tsx       # YA EXISTE - mejorar
│                       ├── BackingFilters.tsx      # NUEVO
│                       └── BackingExportButton.tsx # NUEVO
│
├── components/
│   ├── dashboard/
│   │   ├── stats-card.tsx                      # YA EXISTE - mejorar
│   │   ├── recent-backings.tsx                 # YA EXISTE - mejorar avatars
│   │   ├── complete-profile-banner.tsx         # YA EXISTE - no tocar
│   │   ├── ProgressBar.tsx                     # NUEVO
│   │   └── EmptyStatePlaceholder.tsx           # NUEVO
│   │
│   └── ui/
│       ├── avatar.tsx                          # INSTALAR shadcn
│       └── tooltip.tsx                         # INSTALAR shadcn
│
├── hooks/
│   ├── use-campanias.ts                        # YA EXISTE - no tocar
│   ├── use-artista.ts                          # YA EXISTE - no tocar
│   ├── use-dashboard-stats.ts                  # NUEVO
│   ├── use-campania-stats.ts                   # NUEVO
│   ├── use-campania-backings.ts                # NUEVO
│   └── use-recent-backings.ts                  # NUEVO
│
└── services/
    └── dashboard.service.ts                    # NUEVO
```

---

## 3. Componentes

### 3.1 Dashboard Enhanced Page

**Archivo:** `app/(dashboard)/dashboard/page.tsx` (MODIFICAR)

**Props:** Ninguna (page component)

**Estado Local:**
- Ninguno (usa hooks de React Query)

**Dependencias:**
- Hooks: `useMisCampanias()`, `useMyArtistProfile()`, `useDashboardStats()`
- Componentes: `StatsCard`, `CompleteProfileBanner`, `MisCampaniasCard`, `RecentBackings`

**Responsabilidad:**
- Layout principal del dashboard con 4 stats cards
- Renderizar resumen de campanias y backings recientes
- Mostrar CompleteProfileBanner si artista no tiene perfil completo
- Boton CTA "Nueva campania"

**Cambios vs actual:**
- Agregar hook `useDashboardStats()` para obtener metricas del backend
- Reemplazar calculo client-side de `totalRecaudado` con dato de API
- Agregar stat card "Backers Totales" con dato real
- Mejorar "Mis Campanias" card usando componente `MisCampaniasCard`

---

### 3.2 MisCampaniasCard

**Archivo:** `app/(dashboard)/dashboard/components/MisCampaniasCard.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campanias | `MiCampaniaListItem[]` | Si | Lista de campanias del artista |
| isLoading | `boolean` | No | Estado de carga |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Card`, `CardHeader`, `CardTitle`, `CardContent`, `Button`, `Progress`
- Utils: `formatCurrency`, `calculatePercentage`, `CAMPANIA_ESTADOS_LABELS`
- Next: `Link`

**Responsabilidad:**
- Card con header "Mis Campanias" y boton "Ver todas"
- Lista de hasta 5 campanias con `CampaniaProgressItem`
- EmptyState cuando no hay campanias
- Loading skeleton con 3 items

**Estructura:**
```tsx
export function MisCampaniasCard({ campanias, isLoading }: Props) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle>Mis Campanias</CardTitle>
        <Link href="/campanias">
          <Button variant="ghost" size="sm">Ver todas</Button>
        </Link>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <LoadingSkeleton />
        ) : campanias.length > 0 ? (
          <div className="space-y-4">
            {campanias.slice(0, 5).map(c => (
              <CampaniaProgressItem key={c.id} campania={c} />
            ))}
          </div>
        ) : (
          <EmptyState />
        )}
      </CardContent>
    </Card>
  )
}
```

---

### 3.3 CampaniaProgressItem

**Archivo:** `app/(dashboard)/dashboard/components/CampaniaProgressItem.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campania | `MiCampaniaListItem` | Si | Datos de campania con metricas |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Progress`, `Badge`
- Utils: `calculatePercentage`, `CAMPANIA_ESTADOS_LABELS`, `CAMPANIA_ESTADO_COLORS`
- Next: `Link`

**Responsabilidad:**
- Item clickeable de campania con imagen/avatar gradient
- Titulo, estado badge, progress bar, porcentaje, num backers
- Color de fondo gradient segun estado (borrador=orange, activa=pink, etc.)

**Estructura:**
```tsx
export function CampaniaProgressItem({ campania }: Props) {
  const porcentaje = calculatePercentage(
    campania.importeRecaudado,
    campania.importeObjetivo
  )

  const gradientClass = getGradientByEstado(campania.estadoCampaniaId)

  return (
    <Link href={`/campanias/${campania.id}`}>
      <div className="flex items-center gap-3 rounded-lg border p-3 hover:bg-card transition-colors">
        {/* Imagen gradient */}
        <div className={cn("w-12 h-12 rounded-lg", gradientClass)} />

        {/* Info */}
        <div className="flex-1 min-w-0">
          <p className="font-medium truncate">{campania.titulo}</p>
          <div className="flex items-center gap-2 mt-1">
            <Progress value={porcentaje} className="h-2 flex-1" />
            <span className="text-sm text-muted-foreground">{porcentaje}%</span>
          </div>
          <p className="text-xs text-muted-foreground mt-1">
            {campania.numBackers} backers
          </p>
        </div>

        {/* Badge */}
        <Badge variant={getVariantByEstado(campania.estadoCampaniaId)}>
          {CAMPANIA_ESTADOS_LABELS[campania.estadoCampaniaId]}
        </Badge>
      </div>
    </Link>
  )
}
```

---

### 3.4 ProgressBar (Nuevo componente generico)

**Archivo:** `components/dashboard/ProgressBar.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| current | `number` | Si | Valor actual (recaudado) |
| goal | `number` | Si | Valor objetivo |
| showPercentage | `boolean` | No | Mostrar % al lado |
| showAmount | `boolean` | No | Mostrar "X / Y EUR" |
| className | `string` | No | Clase adicional |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Progress`, `Badge`
- Utils: `formatCurrency`, `calculatePercentage`

**Responsabilidad:**
- Barra de progreso con label de monto y porcentaje
- Badge "Meta alcanzada" si >= 100%
- Color de barra segun porcentaje (verde >= 100%, amarillo >= 50%, rojo < 50%)

**Estructura:**
```tsx
export function ProgressBar({ current, goal, showPercentage, showAmount }: Props) {
  const percentage = calculatePercentage(current, goal)
  const colorClass = percentage >= 100 ? 'text-green-500' : percentage >= 50 ? 'text-yellow-500' : 'text-red-500'

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between text-sm">
        {showAmount && (
          <span className="text-muted-foreground">
            {formatCurrency(current)} de {formatCurrency(goal)}
          </span>
        )}
        {showPercentage && (
          <span className={cn("font-semibold", colorClass)}>
            {percentage}%
          </span>
        )}
      </div>
      <Progress value={percentage} className="h-3" />
      {percentage >= 100 && (
        <Badge variant="success" className="bg-green-500/20 text-green-400">
          <CheckCircle className="mr-1 h-3 w-3" />
          Meta alcanzada
        </Badge>
      )}
    </div>
  )
}
```

---

### 3.5 Campania Stats Page (Enhanced)

**Archivo:** `app/(dashboard)/campanias/[id]/page.tsx` (MODIFICAR)

**Props:** Ninguna (page component, usa `useParams`)

**Estado Local:**
- Ninguno

**Dependencias:**
- Hooks: `useCampania()`, `useCampaniaStats()`
- Componentes: `StatsCard`, `ProgressBar`, `BackingsTable` (existing)
- UI: `Card`, `Button`, `Skeleton`

**Responsabilidad:**
- Header con breadcrumb y botones "Volver", "Ver publico", "Editar"
- 4 stats cards: Recaudado, Backers, Dias Restantes, Promedio
- Progress bar full width con meta alcanzada badge
- Backings recientes (5 items) con link "Ver todos"
- Loading y empty states

**Cambios vs actual:**
- Agregar stats cards arriba
- Agregar `ProgressBar` component full width
- Usar `useCampaniaStats()` para obtener metricas del backend
- Mejorar tabla de backings recientes con avatars

---

### 3.6 BackingsTable Enhanced

**Archivo:** `app/(dashboard)/campanias/[id]/backings/components/BackingsTable.tsx` (MEJORAR)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| backings | `CampaniaBackingItem[]` | Si | Lista de backings |
| isLoading | `boolean` | No | Estado de carga |
| totalCount | `number` | Si | Total de items |
| currentPage | `number` | Si | Pagina actual |
| onPageChange | `(page: number) => void` | Si | Callback de cambio pagina |

**Estado Local:**
- Ninguno (controlado desde page)

**Dependencias:**
- Componentes UI: `Table`, `Avatar`, `Badge`, `Tooltip`
- Utils: `formatCurrency`, `formatRelativeDate`

**Responsabilidad:**
- Tabla con columnas: Backer (avatar + nombre), Reward, Monto, Mensaje (tooltip), Fecha
- Badge "Anonimo" si `esAnonimo = true`
- Tooltip en mensajes truncados (max 50 chars)
- Paginacion en footer con numeros de pagina

**Mejoras vs actual:**
- Agregar columna Avatar con `<Avatar>` component
- Agregar Badge "Anonimo" en nombre
- Agregar Tooltip en mensajes
- Mejorar estilos hover

---

### 3.7 BackingFilters

**Archivo:** `app/(dashboard)/campanias/[id]/backings/components/BackingFilters.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| onSearchChange | `(query: string) => void` | Si | Callback de busqueda |
| onFilterChange | `(filter: FilterType) => void` | Si | Callback de filtro |
| currentFilter | `FilterType` | Si | Filtro activo |

**Estado Local:**
- `searchQuery: string` (debounced)

**Dependencias:**
- Componentes UI: `Input`, `Button`, `Card`
- Utils: `debounce` (custom hook o lodash)

**Responsabilidad:**
- Input de busqueda por nombre/email (debounced 300ms)
- Botones de filtro: "Todos", "Con Reward", "Anonimos"
- Boton "Exportar CSV"

**Estructura:**
```tsx
type FilterType = 'all' | 'with-reward' | 'anonymous'

export function BackingFilters({ onSearchChange, onFilterChange, currentFilter }: Props) {
  const [searchQuery, setSearchQuery] = useState('')

  const debouncedSearch = useDebouncedCallback((value: string) => {
    onSearchChange(value)
  }, 300)

  const handleSearchChange = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value
    setSearchQuery(value)
    debouncedSearch(value)
  }

  return (
    <Card className="mb-6">
      <CardContent className="pt-6">
        <div className="flex flex-col md:flex-row gap-4">
          <Input
            placeholder="Buscar por nombre o email..."
            value={searchQuery}
            onChange={handleSearchChange}
            className="max-w-sm"
          />

          <div className="flex gap-2">
            <Button
              variant={currentFilter === 'all' ? 'default' : 'outline'}
              onClick={() => onFilterChange('all')}
              size="sm"
            >
              Todos
            </Button>
            {/* ... otros filtros */}
          </div>

          <BackingExportButton campaniaId={...} />
        </div>
      </CardContent>
    </Card>
  )
}
```

---

### 3.8 BackingExportButton

**Archivo:** `app/(dashboard)/campanias/[id]/backings/components/BackingExportButton.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campaniaId | `string` | Si | ID de campania |
| disabled | `boolean` | No | Deshabilitar boton |

**Estado Local:**
- Ninguno

**Dependencias:**
- Services: `dashboardService.exportBackingsCSV()`
- UI: `Button`
- Icons: `Download`

**Responsabilidad:**
- Boton que descarga CSV de todos los backings
- Loading state mientras exporta
- Toast success/error

**Estructura:**
```tsx
export function BackingExportButton({ campaniaId, disabled }: Props) {
  const handleExport = async () => {
    try {
      const blob = await dashboardService.exportBackingsCSV(campaniaId)
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `backings-${campaniaId}.csv`
      a.click()
      toast.success('CSV descargado')
    } catch (error) {
      toast.error('Error al exportar CSV')
    }
  }

  return (
    <Button variant="outline" size="sm" onClick={handleExport} disabled={disabled}>
      <Download className="mr-2 h-4 w-4" />
      Exportar CSV
    </Button>
  )
}
```

---

### 3.9 EmptyStatePlaceholder

**Archivo:** `components/dashboard/EmptyStatePlaceholder.tsx` (NUEVO)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| icon | `LucideIcon` | Si | Icono a mostrar |
| title | `string` | Si | Titulo del empty state |
| description | `string` | Si | Descripcion |
| action | `{ label: string, href: string }` | No | CTA opcional |

**Estado Local:**
- Ninguno

**Dependencias:**
- UI: `Button`
- Next: `Link`

**Responsabilidad:**
- Placeholder generico para empty states
- Icono, titulo, descripcion, boton CTA opcional

---

## 4. Hooks

### 4.1 useDashboardStats

**Archivo:** `hooks/use-dashboard-stats.ts` (NUEVO)

**Tipo:** Query Hook

**Parametros:** Ninguno (usa userId del token via service)

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `DashboardResumen \| undefined` | Metricas del dashboard |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.dashboard.resumen`

**Implementacion:**
```typescript
import { useQuery } from '@tanstack/react-query'
import { QUERY_KEYS } from '@shared/constants'
import { dashboardService } from '@/services/dashboard.service'

export function useDashboardStats() {
  return useQuery({
    queryKey: QUERY_KEYS.dashboard.resumen,
    queryFn: () => dashboardService.getResumen(),
    staleTime: 30_000, // 30s cache
  })
}
```

---

### 4.2 useCampaniaStats

**Archivo:** `hooks/use-campania-stats.ts` (NUEVO)

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| campaniaId | `string` | ID de campania |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `CampaniaStatsDetail \| undefined` | Stats detalladas |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.dashboard.campaniaStats(campaniaId)`

**Implementacion:**
```typescript
export function useCampaniaStats(campaniaId: string) {
  return useQuery({
    queryKey: QUERY_KEYS.dashboard.campaniaStats(campaniaId),
    queryFn: () => dashboardService.getCampaniaStats(campaniaId),
    enabled: !!campaniaId,
    staleTime: 60_000, // 1min cache
  })
}
```

---

### 4.3 useCampaniaBackings

**Archivo:** `hooks/use-campania-backings.ts` (NUEVO)

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| campaniaId | `string` | ID de campania |
| params | `BackingsQueryParams` | Paginacion (page, pageSize) |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `CampaniaBackingList \| undefined` | Lista paginada con stats |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.dashboard.campaniaBackings(campaniaId, params)`

**Implementacion:**
```typescript
export function useCampaniaBackings(
  campaniaId: string,
  params: BackingsQueryParams
) {
  return useQuery({
    queryKey: QUERY_KEYS.dashboard.campaniaBackings(campaniaId, params),
    queryFn: () => dashboardService.getCampaniaBackings(campaniaId, params),
    enabled: !!campaniaId,
    keepPreviousData: true, // Para paginacion suave
  })
}
```

---

### 4.4 useRecentBackings

**Archivo:** `hooks/use-recent-backings.ts` (NUEVO)

**Tipo:** Query Hook

**Parametros:** Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `CampaniaBackingItem[] \| undefined` | Ultimos 5 backings |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.backings.recent`

**Implementacion:**
```typescript
export function useRecentBackings() {
  return useQuery({
    queryKey: QUERY_KEYS.backings.recent,
    queryFn: () => dashboardService.getRecentBackings(),
    staleTime: 30_000, // 30s cache
  })
}
```

**Nota:** Este endpoint aun no existe en backend. Ver "Endpoints faltantes" abajo.

---

## 5. Services

### 5.1 dashboardService

**Archivo:** `services/dashboard.service.ts` (NUEVO)

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getResumen | - | `Promise<DashboardResumen>` | GET /api/dashboard/resumen |
| getCampaniaStats | `campaniaId: string` | `Promise<CampaniaStatsDetail>` | GET /api/campanias/{id}/stats |
| getCampaniaBackings | `campaniaId: string, params: BackingsQueryParams` | `Promise<CampaniaBackingList>` | GET /api/campanias/{id}/backings |
| getRecentBackings | - | `Promise<CampaniaBackingItem[]>` | GET /api/backings/recent |
| exportBackingsCSV | `campaniaId: string` | `Promise<Blob>` | GET /api/campanias/{id}/backings/export |

**Implementacion:**
```typescript
import { apiClient } from '@/lib/api-client'
import { API_ROUTES } from '@shared/constants'
import {
  DashboardResumen,
  CampaniaStatsDetail,
  CampaniaBackingList,
  CampaniaBackingItem,
  BackingsQueryParams
} from '@shared/types'

export const dashboardService = {
  async getResumen(): Promise<DashboardResumen> {
    const response = await apiClient.get<ServiceResponse<DashboardResumen>>(
      API_ROUTES.dashboard.resumen
    )
    if (!response.data.isSuccess) {
      throw new Error(response.data.messages[0]?.message || 'Error obteniendo resumen')
    }
    return response.data.data!
  },

  async getCampaniaStats(campaniaId: string): Promise<CampaniaStatsDetail> {
    const response = await apiClient.get<ServiceResponse<CampaniaStatsDetail>>(
      API_ROUTES.dashboard.campaniaStats(campaniaId)
    )
    if (!response.data.isSuccess) {
      throw new Error(response.data.messages[0]?.message || 'Error obteniendo stats')
    }
    return response.data.data!
  },

  async getCampaniaBackings(
    campaniaId: string,
    params: BackingsQueryParams
  ): Promise<CampaniaBackingList> {
    const response = await apiClient.get<ServiceResponse<CampaniaBackingList>>(
      API_ROUTES.dashboard.campaniaBackings(campaniaId),
      { params }
    )
    if (!response.data.isSuccess) {
      throw new Error(response.data.messages[0]?.message || 'Error obteniendo backings')
    }
    return response.data.data!
  },

  async getRecentBackings(): Promise<CampaniaBackingItem[]> {
    // NOTA: Endpoint aun no existe en backend
    const response = await apiClient.get<ServiceResponse<CampaniaBackingItem[]>>(
      '/api/backings/recent'
    )
    if (!response.data.isSuccess) {
      throw new Error(response.data.messages[0]?.message || 'Error obteniendo backings recientes')
    }
    return response.data.data!
  },

  async exportBackingsCSV(campaniaId: string): Promise<Blob> {
    const response = await apiClient.get(
      `/api/campanias/${campaniaId}/backings/export`,
      { responseType: 'blob' }
    )
    return response.data
  },
}
```

---

## 6. Flujo de Datos

```
User Action (Dashboard page load)
    ↓
Component (DashboardPage)
    ↓
Hook (useDashboardStats, useMisCampanias)
    ↓
Service (dashboardService.getResumen)
    ↓
API Client (GET /api/dashboard/resumen)
    ↓
Backend (GetDashboardResumenQuery Handler)
    ↓
Response (DashboardResumen DTO)
    ↓
React Query Cache (staleTime: 30s)
    ↓
Component Re-render con datos
```

**Flujo de navegacion:**
```
/dashboard (main)
    ↓ Click en campania
/campanias/{id} (stats view)
    ↓ Click "Ver todos los backers"
/campanias/{id}/backings (full list)
```

**Flujo de paginacion:**
```
User clicks "Pagina 2"
    ↓
Page component: setPage(2)
    ↓
Hook: useCampaniaBackings(id, { page: 2, pageSize: 20 })
    ↓
Query Key cambia → Fetch automatico
    ↓
Component re-render con nuevos datos
```

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

**Types:**
- `DashboardResumen`
- `MiCampaniaListItem`
- `CampaniaBackingList`
- `CampaniaBackingStats`
- `UltimoBacking`
- `CampaniaBackingItem`
- `CampaniaStatsDetail`
- `RewardStat`
- `ProgressoDia`

**Schemas:**
- `misCampaniasQuerySchema`
- `backingsQuerySchema`
- `BackingsQueryParams`

**Constants:**
- `QUERY_KEYS.dashboard.*`
- `API_ROUTES.dashboard.*`
- `APP_ROUTES.dashboard.*`
- `CAMPANIA_ESTADOS`
- `CAMPANIA_ESTADOS_LABELS`
- `CAMPANIA_ESTADO_COLORS`
- `ESTADO_PEDIDO_LABELS`

**Utils:**
- `formatCurrency`
- `formatRelativeDate`
- `calculatePercentage`
- `calculateDiasRestantes`
- `calculateBackingPromedio`
- `getDashboardErrorMessage`

---

## 8. Routing (App Router)

### 8.1 Rutas Existentes

| Ruta | File | Status |
|------|------|--------|
| `/dashboard` | `app/(dashboard)/dashboard/page.tsx` | YA EXISTE - MODIFICAR |
| `/campanias` | `app/(dashboard)/campanias/page.tsx` | YA EXISTE - NO TOCAR |
| `/campanias/{id}` | `app/(dashboard)/campanias/[id]/page.tsx` | YA EXISTE - MODIFICAR |
| `/campanias/{id}/backings` | `app/(dashboard)/campanias/[id]/backings/page.tsx` | YA EXISTE - MODIFICAR |

**Nota:** Todas las rutas necesarias ya existen. Solo se mejoran.

### 8.2 Layout

**Archivo:** `app/(dashboard)/layout.tsx` (YA EXISTE - NO TOCAR)

- Sidebar navigation (funcional)
- Auth protection (middleware)
- Breadcrumbs automaticos

---

## 9. Componentes shadcn Necesarios

### 9.1 Ya Instalados

- Button
- Card, CardContent, CardHeader, CardTitle
- Input
- Progress
- Badge
- Skeleton
- Table, TableHeader, TableBody, TableRow, TableHead, TableCell
- Alert, AlertTitle, AlertDescription

### 9.2 A Instalar

```bash
cd src/admin
npx shadcn@latest add avatar tooltip
```

**Components a instalar:**
- `Avatar`, `AvatarFallback`, `AvatarImage`
- `Tooltip`, `TooltipProvider`, `TooltipTrigger`, `TooltipContent`

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `app/(dashboard)/dashboard/components/MisCampaniasCard.tsx` | Component | Card de lista de campanias con link "Ver todas" |
| `app/(dashboard)/dashboard/components/CampaniaProgressItem.tsx` | Component | Item de campania con avatar gradient, progress, badge |
| `components/dashboard/ProgressBar.tsx` | Component | Barra de progreso con labels y badge meta alcanzada |
| `components/dashboard/EmptyStatePlaceholder.tsx` | Component | Empty state generico con icono, titulo, descripcion, CTA |
| `app/(dashboard)/campanias/[id]/backings/components/BackingFilters.tsx` | Component | Filtros de busqueda y botones de filtro |
| `app/(dashboard)/campanias/[id]/backings/components/BackingExportButton.tsx` | Component | Boton de exportar CSV |
| `hooks/use-dashboard-stats.ts` | Hook | useQuery para resumen del dashboard |
| `hooks/use-campania-stats.ts` | Hook | useQuery para stats de campania |
| `hooks/use-campania-backings.ts` | Hook | useQuery para backings paginados |
| `hooks/use-recent-backings.ts` | Hook | useQuery para ultimos backings |
| `services/dashboard.service.ts` | Service | API calls para dashboard endpoints |

**Total:** 11 archivos nuevos

---

## 11. Archivos a Modificar

| Archivo | Cambios |
|---------|---------|
| `app/(dashboard)/dashboard/page.tsx` | Agregar `useDashboardStats`, usar `MisCampaniasCard`, mejorar stats cards |
| `app/(dashboard)/campanias/[id]/page.tsx` | Agregar `useCampaniaStats`, agregar stats cards arriba, agregar `ProgressBar` |
| `app/(dashboard)/campanias/[id]/backings/page.tsx` | Agregar `BackingFilters`, mejorar paginacion, integrar filtros client-side |
| `app/(dashboard)/campanias/[id]/backings/components/BackingsTable.tsx` | Agregar avatars, tooltips en mensajes, badge anonimo |
| `components/dashboard/stats-card.tsx` | Mejorar estilos hover, agregar prop `description` opcional |
| `components/dashboard/recent-backings.tsx` | Agregar avatars, usar `useRecentBackings` hook |

**Total:** 6 archivos modificados

---

## 12. Endpoints Faltantes (Backend)

**CRITICO:** Estos endpoints aun NO existen en backend y deben ser creados:

1. **GET `/api/dashboard/resumen`**
   - Response: `DashboardResumen`
   - Calcula total recaudado, backers unicos, campanias activas/completadas

2. **GET `/api/campanias/{id}/stats`**
   - Response: `CampaniaStatsDetail`
   - Calcula metricas avanzadas (proyeccion, velocidad diaria, reward stats)

3. **GET `/api/campanias/{id}/backings`** (verificar si existe)
   - Response: `CampaniaBackingList` (paginado)
   - Lista de backings con paginacion y stats agregadas

4. **GET `/api/backings/recent`** (opcional MVP)
   - Response: `CampaniaBackingItem[]`
   - Ultimos 5 backings de todas las campanias del artista

5. **GET `/api/campanias/{id}/backings/export`** (opcional MVP)
   - Response: CSV Blob
   - Exportar todos los backings a CSV

**Prioridad:**
- P0 (MVP): 1, 2, 3
- P1 (Nice to have): 4, 5

---

## 13. Estado y Data Flow

### 13.1 React Query Cache

**Configuracion:**
```typescript
// app/providers.tsx (YA EXISTE)
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000, // 30s por defecto
      retry: 1,
    },
  },
})
```

**Cache keys granulares:**
- `['dashboard', 'resumen']` - Invalidar al crear/actualizar campanias
- `['dashboard', 'campanias', id, 'stats']` - Invalidar al recibir backing nuevo
- `['dashboard', 'campanias', id, 'backings', params]` - Invalidar al recibir backing nuevo
- `['backings', 'recent']` - Invalidar al recibir backing nuevo

### 13.2 Invalidacion de Queries

**Cuando invalidar:**
```typescript
// Despues de crear campania
queryClient.invalidateQueries({ queryKey: ['dashboard', 'resumen'] })
queryClient.invalidateQueries({ queryKey: ['dashboard', 'mis-campanias'] })

// Despues de recibir backing nuevo (via webhook o poll)
queryClient.invalidateQueries({ queryKey: ['dashboard', 'campanias', campaniaId, 'stats'] })
queryClient.invalidateQueries({ queryKey: ['dashboard', 'campanias', campaniaId, 'backings'] })
queryClient.invalidateQueries({ queryKey: ['backings', 'recent'] })
```

### 13.3 Loading States

**Jerarquia:**
1. Skeleton placeholders (para contenido estructurado)
2. Spinner (para acciones puntuales)
3. Disable buttons con loading icon

**Ejemplo:**
```tsx
{isLoading ? (
  <div className="space-y-4">
    {[1, 2, 3].map(i => <Skeleton key={i} className="h-16 w-full" />)}
  </div>
) : (
  <ComponenteConDatos />
)}
```

### 13.4 Error Handling

**Patron:**
```tsx
if (error) {
  return (
    <Alert variant="destructive">
      <AlertCircle className="h-4 w-4" />
      <AlertTitle>Error al cargar datos</AlertTitle>
      <AlertDescription>
        {getDashboardErrorMessage(error.code)}
        <Button variant="link" onClick={() => refetch()}>
          Reintentar
        </Button>
      </AlertDescription>
    </Alert>
  )
}
```

---

## 14. Responsive Design

### 14.1 Breakpoints

| Breakpoint | Width | Layout |
|------------|-------|--------|
| Mobile | < 640px | 1 col, sidebar collapse, horizontal scroll en tablas |
| Tablet | 640px - 1024px | 2 cols stats (2x2), cards stack |
| Desktop | > 1024px | 4 cols stats, 2 cols cards lado a lado |

### 14.2 Tailwind Classes

**Stats grid:**
```tsx
<div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
```

**Campanias + Backings grid:**
```tsx
<div className="grid gap-6 lg:grid-cols-2">
```

**Filtros responsive:**
```tsx
<div className="flex flex-col md:flex-row gap-4">
```

**Tabla con scroll horizontal:**
```tsx
<div className="overflow-x-auto">
  <Table className="min-w-[600px]">
```

---

## 15. Accesibilidad

### 15.1 ARIA Labels

```tsx
// Stats card
<Card aria-label={`Estadistica: ${title}`}>
  <div role="status" aria-live="polite">{value}</div>
</Card>

// Progress bar
<Progress
  value={percentage}
  aria-label="Progreso de meta"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
/>

// Table
<Table aria-label="Lista de backings de campania">
  <TableHeader>
    <TableRow>
      <TableHead scope="col">Backer</TableHead>
    </TableRow>
  </TableHeader>
</Table>

// Paginacion
<nav aria-label="Paginacion de backings">
  <Button aria-label={`Ir a pagina ${page}`}>{page}</Button>
</nav>
```

### 15.2 Keyboard Navigation

- Todos los botones accesibles con Tab
- Links y cards clickeables con Enter
- Focus visible con ring (outline)

### 15.3 Screen Reader Announcements

```tsx
// Cuando stats se actualizan
<div role="status" aria-live="polite" className="sr-only">
  Estadisticas actualizadas. Total recaudado: {formatCurrency(total)}
</div>

// Cuando tabla se filtra
<div role="status" aria-live="polite" className="sr-only">
  Mostrando {filteredCount} de {totalCount} backings
</div>
```

---

## 16. Testing

### 16.1 Unit Tests

**Componentes a testear:**
- `MisCampaniasCard.test.tsx`
  - Renders list of campanias correctly
  - Shows empty state when no campanias
  - Shows loading skeleton
  - "Ver todas" link navigates correctly

- `CampaniaProgressItem.test.tsx`
  - Displays campania info correctly
  - Badge shows correct estado
  - Progress bar shows correct percentage
  - Gradient color matches estado

- `ProgressBar.test.tsx`
  - Calculates percentage correctly
  - Shows "Meta alcanzada" when >= 100%
  - Displays amounts when showAmount=true

- `BackingFilters.test.tsx`
  - Search input debounces correctly
  - Filter buttons update state
  - Export button calls service

### 16.2 Integration Tests

**Hooks:**
- `useDashboardStats.test.ts`
  - Fetches resumen successfully
  - Caches data for 30s
  - Handles error correctly

- `useCampaniaBackings.test.ts`
  - Fetches backings with pagination
  - keepPreviousData works correctly
  - Query key changes on param change

### 16.3 E2E Tests (Playwright)

```typescript
// dashboard-artista.e2e.ts
test('Artista views dashboard and navigates to backings', async ({ page }) => {
  // Login as artista
  await page.goto('/auth/login')
  await page.fill('[name=email]', 'artista@test.com')
  await page.fill('[name=password]', 'password')
  await page.click('button[type=submit]')

  // Dashboard loads with stats
  await expect(page.locator('h1')).toContainText('Dashboard')
  await expect(page.locator('[aria-label*="Total Recaudado"]')).toBeVisible()

  // Click on campania
  await page.click('[href*="/campanias/"][data-testid="campania-item"]')

  // Stats view loads
  await expect(page.locator('[aria-label*="Recaudado"]')).toBeVisible()

  // Click "Ver todos los backers"
  await page.click('text=Ver todos los backers')

  // Backings list loads
  await expect(page.locator('[aria-label="Lista de backings"]')).toBeVisible()

  // Filter works
  await page.fill('[placeholder*="Buscar"]', 'Juan')
  await expect(page.locator('tbody tr')).toHaveCount(1)
})
```

---

## 17. Checklist

### 17.1 Arquitectura

- [ ] Componentes usan shadcn/ui (Avatar, Tooltip instalados)
- [ ] Hooks siguen patron useQuery (staleTime configurado)
- [ ] Services manejan errores correctamente (throw Error con mensaje)
- [ ] Types importados de shared (no duplicados)
- [ ] Constants importados de shared (query keys, API routes)

### 17.2 Data Flow

- [ ] Dashboard stats obtenidos de backend (no client-side calculation)
- [ ] React Query cache configurado (30s staleTime)
- [ ] Invalidacion de queries funcionando (al crear campania, recibir backing)
- [ ] Loading states con skeletons
- [ ] Error handling con Alert component

### 17.3 UI/UX

- [ ] Responsive design probado (mobile, tablet, desktop)
- [ ] Animaciones hover en cards
- [ ] Empty states implementados
- [ ] Avatars con fallback funcionando
- [ ] Tooltips en mensajes truncados
- [ ] Progress bar con colores segun porcentaje
- [ ] Badge "Meta alcanzada" cuando >= 100%

### 17.4 Accesibilidad

- [ ] ARIA labels en stats, progress bars, tables
- [ ] Keyboard navigation funcionando
- [ ] Focus visible en elementos interactivos
- [ ] Screen reader announcements implementados
- [ ] Semantic HTML (h1, h2, nav, main)

### 17.5 Performance

- [ ] Debounced search (300ms)
- [ ] keepPreviousData en paginacion
- [ ] Lazy loading si aplica
- [ ] No re-renders innecesarios

---

## 18. Siguiente Paso Sugerido

**Orden de implementacion:**

1. **Backend primero (no parte de este plan, pero critico):**
   - Crear endpoints: `/api/dashboard/resumen`, `/api/campanias/{id}/stats`, `/api/campanias/{id}/backings`
   - Probar en Swagger

2. **Shared (ejecutar agente shared primero):**
   - Crear types en `dashboard.ts`
   - Crear schemas en `dashboard.schema.ts`
   - Actualizar constants

3. **Frontend Admin (este plan):**
   - Instalar shadcn components: `npx shadcn@latest add avatar tooltip`
   - Crear service `dashboard.service.ts`
   - Crear hooks (4 hooks)
   - Crear componentes nuevos (8 componentes)
   - Modificar pages existentes (3 pages)
   - Mejorar componentes existentes (BackingsTable, RecentBackings, StatsCard)

4. **Testing:**
   - Unit tests para componentes
   - Integration tests para hooks
   - E2E test completo

**Tiempo estimado:** 12-16 horas

---

**Fin del Plan Frontend: Dashboard Artista (Admin)**
