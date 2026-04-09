# Plan Frontend: Hacer Backing (Admin/Dashboard)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/admin (Next.js 14 App Router)
**Rol:** Artista
**Impacto:** BAJO - Solo lectura de backings recibidos

---

## 1. Resumen

**Alcance MVP:** Vista simplificada de backings recibidos para que el artista pueda:
1. Ver estadísticas básicas de una campaña (Total Raised, Total Backers, Avg Amount)
2. Listar backings recibidos en tabla
3. Filtrar por búsqueda y reward
4. Exportar CSV (opcional)

**No incluido en MVP:**
- Gestión de entregas
- Mensajería con backers
- Dashboard avanzado de analytics
- Notificaciones de nuevos backings

**Componentes a crear:**
- Página: `CampaignBackingsPage` (1 archivo)
- Componentes: `BackingStatsGrid` (3 stat cards), `BackingsTable` (tabla shadcn/ui)
- Hooks: `useCampaignBackings`, `useCampaignStats` (2 hooks)
- Service: Extender `campaniaService` (métodos nuevos)

**Total archivos nuevos:** ~8
- 1 página (page.tsx)
- 3 componentes
- 2 hooks
- 1 service modificado
- 1 test (opcional)

---

## 2. Estructura de Carpetas

### 2.1 Vista General

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── campanias/
│           └── [id]/
│               ├── backings/           # NUEVA carpeta
│               │   ├── page.tsx        # NUEVO - Página principal
│               │   └── components/     # NUEVO
│               │       ├── BackingStatsGrid.tsx
│               │       ├── BackingsTable.tsx
│               │       └── __tests__/
│               │           ├── BackingStatsGrid.test.tsx
│               │           └── BackingsTable.test.tsx
│               ├── recompensas/        # EXISTENTE
│               ├── editar/             # EXISTENTE
│               └── page.tsx            # EXISTENTE
│
├── hooks/
│   └── use-backings.ts                 # NUEVO
│
└── services/
    └── campania.service.ts             # MODIFICAR (agregar métodos)
```

### 2.2 Archivos Existentes a Modificar

| Archivo | Cambios |
|---------|---------|
| `src/admin/src/services/campania.service.ts` | Agregar `getBackings()` y `getStats()` |
| `src/admin/src/app/(dashboard)/campanias/[id]/page.tsx` | Agregar link a "Ver apoyos" (opcional) |

### 2.3 Archivos a Crear

| Archivo | Tipo | Descripción |
|---------|------|-------------|
| `app/(dashboard)/campanias/[id]/backings/page.tsx` | Página | Página principal de backings |
| `app/(dashboard)/campanias/[id]/backings/components/BackingStatsGrid.tsx` | Componente | Grid con 3 stat cards |
| `app/(dashboard)/campanias/[id]/backings/components/BackingsTable.tsx` | Componente | Tabla de backings con filtros |
| `hooks/use-backings.ts` | Hook | Hooks de TanStack Query para backings |

---

## 3. Componentes

### 3.1 Página: `CampaignBackingsPage`

**Archivo:** `app/(dashboard)/campanias/[id]/backings/page.tsx`

**Responsabilidad:**
- Página raíz de la ruta `/dashboard/campanias/{id}/backings`
- Coordina la carga de datos de estadísticas y backings
- Layout con breadcrumb, stats grid y tabla

**Dependencias:**
- Hooks: `useCampaignBackings`, `useCampaignStats`
- Componentes: `BackingStatsGrid`, `BackingsTable`
- shadcn/ui: `Card`, `Skeleton`, `Button`

**Props:** Ninguno (usa `useParams` de Next.js)

**Estado Local:**
- `searchTerm: string` - Para filtro de búsqueda
- `selectedReward: string | null` - Para filtro por reward

**Estructura JSX:**

```tsx
<div className="space-y-6">
  {/* Breadcrumb */}
  <div className="flex items-center gap-2 text-sm">
    <Link href="/campanias">Campañas</Link>
    <span>/</span>
    <Link href={`/campanias/${id}`}>{campaniaTitulo}</Link>
    <span>/</span>
    <span className="text-muted-foreground">Apoyos</span>
  </div>

  {/* Page Header */}
  <div className="flex items-center justify-between">
    <div>
      <h1 className="text-3xl font-bold">Apoyos Recibidos</h1>
      <p className="text-muted-foreground">
        {campaniaTitulo}
      </p>
    </div>
    <Button variant="outline" onClick={handleExport}>
      Exportar CSV
    </Button>
  </div>

  {/* Stats Grid */}
  <BackingStatsGrid stats={stats} isLoading={statsLoading} />

  {/* Table */}
  <BackingsTable
    backings={backings}
    isLoading={backingsLoading}
    searchTerm={searchTerm}
    onSearchChange={setSearchTerm}
    selectedReward={selectedReward}
    onRewardFilterChange={setSelectedReward}
    rewards={rewards}
  />
</div>
```

**Estados de UI:**
| Estado | Visual |
|--------|--------|
| Loading | Skeleton para stats (3 cards) + tabla (5 rows) |
| Empty | Mensaje "No hay apoyos aún" con icono |
| Error | Alert rojo con mensaje de error |
| Success | Stats cards + tabla con datos |

---

### 3.2 Componente: `BackingStatsGrid`

**Archivo:** `app/(dashboard)/campanias/[id]/backings/components/BackingStatsGrid.tsx`

**Responsabilidad:**
- Renderizar grid de 3 stat cards (Total Raised, Total Backers, Avg Amount)
- Mostrar loading states con skeletons

**Props:**

| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| stats | `CampaniaStats \| null` | Sí | Datos de estadísticas |
| isLoading | `boolean` | Sí | Estado de carga |

**Dependencias:**
- shadcn/ui: `Card`, `CardHeader`, `CardTitle`, `CardContent`, `Skeleton`
- Iconos: `Users`, `DollarSign`, `TrendingUp` (lucide-react)
- Utils: `formatCurrency` de @shared/utils

**Estructura JSX:**

```tsx
<div className="grid grid-cols-1 md:grid-cols-3 gap-4">
  {/* Card 1: Total Raised */}
  <Card>
    <CardHeader className="flex flex-row items-center justify-between">
      <CardTitle className="text-sm font-medium">
        Total Recaudado
      </CardTitle>
      <DollarSign className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold">
        {formatCurrency(stats.totalRecaudado)}
      </div>
    </CardContent>
  </Card>

  {/* Card 2: Total Backers */}
  <Card>
    <CardHeader className="flex flex-row items-center justify-between">
      <CardTitle className="text-sm font-medium">
        Total Apoyos
      </CardTitle>
      <Users className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold">
        {stats.totalBackers}
      </div>
    </CardContent>
  </Card>

  {/* Card 3: Avg Amount */}
  <Card>
    <CardHeader className="flex flex-row items-center justify-between">
      <CardTitle className="text-sm font-medium">
        Promedio
      </CardTitle>
      <TrendingUp className="h-4 w-4 text-muted-foreground" />
    </CardHeader>
    <CardContent>
      <div className="text-2xl font-bold">
        {formatCurrency(stats.promedioAporte)}
      </div>
    </CardContent>
  </Card>
</div>
```

**Estados de UI:**
| Estado | Visual |
|--------|--------|
| Loading | Skeleton en lugar de valores (3 skeletons) |
| No data | Cards con "€0" o "0" |

---

### 3.3 Componente: `BackingsTable`

**Archivo:** `app/(dashboard)/campanias/[id]/backings/components/BackingsTable.tsx`

**Responsabilidad:**
- Renderizar tabla de backings con columnas: Backer, Amount, Reward, Date, Status
- Filtros: Search input (por nombre), Select (por reward)
- Paginación (si hay muchos backings)

**Props:**

| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| backings | `BackingPublicDto[]` | Sí | Lista de backings |
| isLoading | `boolean` | Sí | Estado de carga |
| searchTerm | `string` | Sí | Término de búsqueda |
| onSearchChange | `(term: string) => void` | Sí | Handler cambio búsqueda |
| selectedReward | `string \| null` | Sí | ID del reward seleccionado |
| onRewardFilterChange | `(id: string \| null) => void` | Sí | Handler cambio filtro |
| rewards | `Reward[]` | Sí | Lista de rewards para filtro |

**Dependencias:**
- shadcn/ui: `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableCell`, `Input`, `Select`, `Badge`
- Iconos: `Search`, `Filter` (lucide-react)
- Utils: `formatCurrency`, `formatDate` de @shared/utils

**Estructura JSX:**

```tsx
<Card>
  <CardHeader>
    <div className="flex items-center gap-4">
      {/* Search Input */}
      <div className="relative flex-1">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Buscar por nombre..."
          value={searchTerm}
          onChange={(e) => onSearchChange(e.target.value)}
          className="pl-10"
        />
      </div>

      {/* Reward Filter */}
      <Select value={selectedReward ?? ""} onValueChange={onRewardFilterChange}>
        <SelectTrigger className="w-[200px]">
          <Filter className="h-4 w-4 mr-2" />
          <SelectValue placeholder="Todas las recompensas" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="">Todas</SelectItem>
          {rewards.map(reward => (
            <SelectItem key={reward.id} value={reward.id}>
              {reward.nombre}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  </CardHeader>

  <CardContent>
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Backer</TableHead>
          <TableHead>Monto</TableHead>
          <TableHead>Recompensa</TableHead>
          <TableHead>Fecha</TableHead>
          <TableHead>Estado</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {filteredBackings.map(backing => (
          <TableRow key={backing.id}>
            <TableCell>
              {backing.nombreBacker}
            </TableCell>
            <TableCell className="font-semibold">
              {formatCurrency(backing.monto)}
            </TableCell>
            <TableCell className="text-muted-foreground">
              {backing.rewardNombre ?? "Sin recompensa"}
            </TableCell>
            <TableCell className="text-muted-foreground">
              {formatDate(backing.fechaCreacion, "short")}
            </TableCell>
            <TableCell>
              <Badge variant="success">Confirmado</Badge>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>

    {filteredBackings.length === 0 && (
      <div className="text-center py-8">
        <p className="text-muted-foreground">
          No se encontraron apoyos
        </p>
      </div>
    )}
  </CardContent>
</Card>
```

**Estados de UI:**
| Estado | Visual |
|--------|--------|
| Loading | Skeleton rows (5 rows) |
| Empty | Mensaje "No hay apoyos aún" centrado |
| No results | Mensaje "No se encontraron apoyos" (post-filtro) |

**Lógica de Filtrado (client-side):**

```typescript
const filteredBackings = backings.filter(backing => {
  // Filtro por búsqueda
  const matchesSearch = backing.nombreBacker
    .toLowerCase()
    .includes(searchTerm.toLowerCase())

  // Filtro por reward
  const matchesReward = selectedReward
    ? backing.rewardId === selectedReward
    : true

  return matchesSearch && matchesReward
})
```

---

## 4. Hooks

### 4.1 Hook: `useCampaignBackings`

**Archivo:** `hooks/use-backings.ts`

**Tipo:** Query Hook (TanStack Query)

**Responsabilidad:**
- Cargar lista de backings de una campaña paginados
- Invalidar cache cuando se crea un nuevo backing (desde landing)

**Parámetros:**

| Param | Tipo | Descripción |
|-------|------|-------------|
| campaniaId | `string` | ID de la campaña |
| params | `BackingsQueryParams` (opcional) | Filtros de paginación |

**Retorna:**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | `PaginatedResponse<BackingPublicDto>` | Lista de backings con meta |
| isLoading | `boolean` | Estado de carga inicial |
| isFetching | `boolean` | Estado de carga (incluye refetch) |
| error | `Error \| null` | Error si hay |
| refetch | `() => void` | Función para refrescar datos |

**Query Key:** `QUERY_KEYS.backings.byCampania(campaniaId)`

**Implementación:**

```typescript
export function useCampaignBackings(
  campaniaId: string,
  params?: BackingsQueryParams
) {
  return useQuery({
    queryKey: QUERY_KEYS.backings.byCampania(campaniaId),
    queryFn: () => campaniaService.getBackings(campaniaId, params),
    enabled: !!campaniaId,
  })
}
```

---

### 4.2 Hook: `useCampaignStats`

**Archivo:** `hooks/use-backings.ts`

**Tipo:** Query Hook (TanStack Query)

**Responsabilidad:**
- Cargar estadísticas agregadas de backings de una campaña
- Refrescar automáticamente cada 30 segundos (opcional)

**Parámetros:**

| Param | Tipo | Descripción |
|-------|------|-------------|
| campaniaId | `string` | ID de la campaña |

**Retorna:**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | `CampaniaStats \| null` | Estadísticas de la campaña |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.campanias.stats(campaniaId)`

**Implementación:**

```typescript
export function useCampaignStats(campaniaId: string) {
  return useQuery({
    queryKey: QUERY_KEYS.campanias.stats(campaniaId),
    queryFn: () => campaniaService.getStats(campaniaId),
    enabled: !!campaniaId,
    // Opcional: refetch cada 30 segundos
    refetchInterval: 30000,
    refetchIntervalInBackground: false,
  })
}
```

---

## 5. Services

### 5.1 Modificar: `campaniaService`

**Archivo:** `services/campania.service.ts`

**Métodos a Agregar:**

#### 5.1.1 `getBackings`

| Campo | Valor |
|-------|-------|
| **Método** | `getBackings(campaniaId, params?)` |
| **Input** | `campaniaId: string, params?: BackingsQueryParams` |
| **Output** | `Promise<PaginatedResponse<BackingPublicDto>>` |
| **Endpoint** | `GET /api/campanias/{id}/backings` |

**Implementación:**

```typescript
async getBackings(
  campaniaId: string,
  params?: BackingsQueryParams
): Promise<PaginatedResponse<BackingPublicDto>> {
  const queryParams = new URLSearchParams()
  if (params?.pageNumber) queryParams.set("pageNumber", String(params.pageNumber))
  if (params?.pageSize) queryParams.set("pageSize", String(params.pageSize))

  const response = await apiFetch<ServiceResponse<PaginatedResponse<BackingPublicDto>>>(
    `${this.baseUrl}/${campaniaId}/backings?${queryParams.toString()}`
  )
  return response.data
}
```

#### 5.1.2 `getStats`

| Campo | Valor |
|-------|-------|
| **Método** | `getStats(campaniaId)` |
| **Input** | `campaniaId: string` |
| **Output** | `Promise<CampaniaStats>` |
| **Endpoint** | `GET /api/campanias/{id}/stats` |

**Implementación:**

```typescript
async getStats(campaniaId: string): Promise<CampaniaStats> {
  const response = await apiFetch<ServiceResponse<CampaniaStats>>(
    `${this.baseUrl}/${campaniaId}/stats`
  )
  return response.data
}
```

**Clase Completa (fragmento):**

```typescript
class CampaniaService {
  private readonly baseUrl = "/campanias"

  // ... métodos existentes (getAll, getById, create, update, etc.)

  // NUEVO - Obtener backings de una campaña
  async getBackings(
    campaniaId: string,
    params?: BackingsQueryParams
  ): Promise<PaginatedResponse<BackingPublicDto>> {
    const queryParams = new URLSearchParams()
    if (params?.pageNumber) queryParams.set("pageNumber", String(params.pageNumber))
    if (params?.pageSize) queryParams.set("pageSize", String(params.pageSize))

    const response = await apiFetch<ServiceResponse<PaginatedResponse<BackingPublicDto>>>(
      `${this.baseUrl}/${campaniaId}/backings?${queryParams.toString()}`
    )
    return response.data
  }

  // NUEVO - Obtener estadísticas de backings
  async getStats(campaniaId: string): Promise<CampaniaStats> {
    const response = await apiFetch<ServiceResponse<CampaniaStats>>(
      `${this.baseUrl}/${campaniaId}/stats`
    )
    return response.data
  }
}

export const campaniaService = new CampaniaService()
```

---

## 6. Flujo de Datos

### 6.1 Diagrama de Flujo

```
User Navigation (/campanias/{id}/backings)
    ↓
CampaignBackingsPage (page.tsx)
    ↓
useParams() → campaniaId
    ↓
┌────────────────────────────────────┐
│ Hooks (parallel queries)           │
│ - useCampaignStats(campaniaId)     │
│ - useCampaignBackings(campaniaId)  │
│ - useCampania(campaniaId)          │ (para título)
└────────────────────────────────────┘
    ↓
Services
    ↓
┌────────────────────────────────────┐
│ campaniaService.getStats()         │
│ → GET /api/campanias/{id}/stats   │
│                                    │
│ campaniaService.getBackings()      │
│ → GET /api/campanias/{id}/backings│
└────────────────────────────────────┘
    ↓
Backend (ASP.NET Core)
    ↓
GetCampaniaStatsQuery / GetCampaniaBackingsQuery
    ↓
Database (SQL Server)
    ↓
Response (ServiceResponse<T>)
    ↓
TanStack Query Cache
    ↓
React State
    ↓
Components Render
```

### 6.2 Query Keys Strategy

| Query Key | Invalidación | Refetch |
|-----------|--------------|---------|
| `['campanias', id, 'stats']` | Cuando se crea un backing (landing) | Cada 30s |
| `['backings', 'campania', id]` | Cuando se crea un backing (landing) | Manual |
| `['campanias', id]` | Cuando se actualiza campaña | On focus |

**Invalidación desde Landing:**
Cuando un fan crea un backing en la landing, debe invalidar:
```typescript
queryClient.invalidateQueries({ queryKey: ['campanias', campaniaId, 'stats'] })
queryClient.invalidateQueries({ queryKey: ['backings', 'campania', campaniaId] })
```

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

### 7.1 Types

```typescript
import type {
  BackingPublicDto,
  CampaniaStats,
  PaginatedResponse,
} from "@shared/types"
```

### 7.2 Constants

```typescript
import {
  QUERY_KEYS,
  API_ROUTES,
} from "@shared/constants"
```

### 7.3 Utils

```typescript
import {
  formatCurrency,
  formatDate,
} from "@shared/utils"
```

### 7.4 Schemas

No se requieren schemas Zod para Admin (solo lectura).

---

## 8. Archivos a Crear/Modificar

### 8.1 Resumen

| Archivo | Tipo | Acción | Descripción |
|---------|------|--------|-------------|
| `app/(dashboard)/campanias/[id]/backings/page.tsx` | Página | CREAR | Página principal de backings |
| `app/(dashboard)/campanias/[id]/backings/components/BackingStatsGrid.tsx` | Componente | CREAR | Grid con 3 stat cards |
| `app/(dashboard)/campanias/[id]/backings/components/BackingsTable.tsx` | Componente | CREAR | Tabla de backings con filtros |
| `hooks/use-backings.ts` | Hook | CREAR | Hooks de TanStack Query |
| `services/campania.service.ts` | Service | MODIFICAR | Agregar `getBackings()` y `getStats()` |

### 8.2 Archivos Opcionales (Post-MVP)

| Archivo | Descripción |
|---------|-------------|
| `app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingStatsGrid.test.tsx` | Test del componente stats |
| `app/(dashboard)/campanias/[id]/backings/components/__tests__/BackingsTable.test.tsx` | Test de la tabla |
| `hooks/__tests__/use-backings.test.ts` | Test de hooks |

---

## 9. Routing (Next.js App Router)

### 9.1 Nueva Ruta

| Ruta | Archivo | Auth | Descripción |
|------|---------|------|-------------|
| `/dashboard/campanias/[id]/backings` | `app/(dashboard)/campanias/[id]/backings/page.tsx` | Sí (Artista) | Vista de backings de una campaña |

### 9.2 Navegación

**Agregar link en:** `app/(dashboard)/campanias/[id]/page.tsx`

```tsx
<Button
  variant="outline"
  onClick={() => router.push(`/campanias/${id}/backings`)}
>
  Ver Apoyos Recibidos
</Button>
```

**Breadcrumb en nueva página:**
```
Dashboard > Campañas > {CampaniaTitulo} > Apoyos
```

---

## 10. Estados de UI y Validación

### 10.1 Estados de Carga

| Componente | Loading State |
|------------|---------------|
| `BackingStatsGrid` | 3 Skeleton cards |
| `BackingsTable` | 5 Skeleton rows |
| Page | Skeleton grid + table skeleton |

### 10.2 Estados Vacíos

| Componente | Empty State |
|------------|-------------|
| `BackingsTable` | "No hay apoyos aún. Cuando recibas tu primer apoyo, aparecerá aquí." (con icono) |
| `BackingStatsGrid` | Cards con valores "€0" / "0" |

### 10.3 Estados de Error

| Error | Mensaje | Acción |
|-------|---------|--------|
| 404 Campaña no encontrada | "Esta campaña no existe o fue eliminada" | Redirect a `/campanias` |
| 403 No autorizado | "No tienes permiso para ver los apoyos de esta campaña" | Redirect a `/campanias` |
| 500 Error servidor | "Error al cargar los apoyos. Intenta nuevamente." | Botón "Reintentar" |

### 10.4 Validación

**No aplica** - Esta es una página de solo lectura. No hay formularios ni validación necesaria.

---

## 11. Testing (Opcional para MVP)

### 11.1 Tests de Componentes (Vitest + Testing Library)

**BackingStatsGrid.test.tsx:**
```typescript
describe("BackingStatsGrid", () => {
  it("renders stats correctly", () => {
    const stats: CampaniaStats = {
      totalRecaudado: 5000,
      totalBackers: 42,
      promedioAporte: 119.05,
      // ...
    }

    render(<BackingStatsGrid stats={stats} isLoading={false} />)

    expect(screen.getByText("€5,000.00")).toBeInTheDocument()
    expect(screen.getByText("42")).toBeInTheDocument()
    expect(screen.getByText("€119.05")).toBeInTheDocument()
  })

  it("renders skeletons when loading", () => {
    render(<BackingStatsGrid stats={null} isLoading={true} />)

    const skeletons = screen.getAllByTestId("skeleton")
    expect(skeletons).toHaveLength(3)
  })
})
```

**BackingsTable.test.tsx:**
```typescript
describe("BackingsTable", () => {
  it("filters backings by search term", () => {
    const backings: BackingPublicDto[] = [
      { id: "1", nombreBacker: "Juan Perez", monto: 50, ... },
      { id: "2", nombreBacker: "Ana Lopez", monto: 100, ... },
    ]

    const { rerender } = render(
      <BackingsTable
        backings={backings}
        searchTerm=""
        onSearchChange={jest.fn()}
        // ...
      />
    )

    expect(screen.getByText("Juan Perez")).toBeInTheDocument()
    expect(screen.getByText("Ana Lopez")).toBeInTheDocument()

    rerender(
      <BackingsTable
        backings={backings}
        searchTerm="juan"
        onSearchChange={jest.fn()}
        // ...
      />
    )

    expect(screen.getByText("Juan Perez")).toBeInTheDocument()
    expect(screen.queryByText("Ana Lopez")).not.toBeInTheDocument()
  })

  it("shows empty state when no backings", () => {
    render(<BackingsTable backings={[]} isLoading={false} {...otherProps} />)

    expect(screen.getByText(/no hay apoyos aún/i)).toBeInTheDocument()
  })
})
```

### 11.2 Tests de Hooks (Vitest + React Query Testing)

**use-backings.test.ts:**
```typescript
describe("useCampaignBackings", () => {
  it("fetches backings successfully", async () => {
    const queryClient = new QueryClient()
    const wrapper = createWrapper(queryClient)

    const { result } = renderHook(
      () => useCampaignBackings("campania-123"),
      { wrapper }
    )

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data?.items).toHaveLength(10)
  })
})

describe("useCampaignStats", () => {
  it("fetches stats successfully", async () => {
    const queryClient = new QueryClient()
    const wrapper = createWrapper(queryClient)

    const { result } = renderHook(
      () => useCampaignStats("campania-123"),
      { wrapper }
    )

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data?.totalBackers).toBeGreaterThan(0)
  })
})
```

---

## 12. Funcionalidades Post-MVP

**Mejoras futuras (NO incluidas en este plan):**

| Funcionalidad | Descripción | Complejidad |
|---------------|-------------|-------------|
| **Export CSV** | Botón "Exportar CSV" que genera archivo descargable | Baja |
| **Paginación** | Pagination component para manejar >100 backings | Media |
| **Modal detalle** | Click en row → modal con mensaje del backer, detalles completos | Baja |
| **Filtro por fecha** | Date range picker para filtrar backings por rango de fechas | Media |
| **Notificaciones** | Toast cuando se recibe un nuevo backing (SignalR) | Alta |
| **Gestión de entregas** | Marcar rewards como "enviado", "entregado" | Alta |
| **Mensajería** | Enviar mensaje a backer individual o masivo | Alta |
| **Analytics dashboard** | Gráficos de evolución de backings (Chart.js) | Media |

---

## 13. Checklist de Implementación

### 13.1 Fase 1: Estructura (15 min)
- [ ] Crear carpeta `app/(dashboard)/campanias/[id]/backings/`
- [ ] Crear carpeta `app/(dashboard)/campanias/[id]/backings/components/`
- [ ] Crear archivo `hooks/use-backings.ts`

### 13.2 Fase 2: Services (10 min)
- [ ] Modificar `services/campania.service.ts`
- [ ] Agregar método `getBackings(campaniaId, params?)`
- [ ] Agregar método `getStats(campaniaId)`
- [ ] Verificar que compila sin errores

### 13.3 Fase 3: Hooks (10 min)
- [ ] Implementar `useCampaignBackings` en `use-backings.ts`
- [ ] Implementar `useCampaignStats` en `use-backings.ts`
- [ ] Exportar hooks en `hooks/index.ts`

### 13.4 Fase 4: Componentes (30 min)
- [ ] Crear `BackingStatsGrid.tsx`
  - [ ] Grid de 3 cards
  - [ ] Loading states con Skeleton
  - [ ] Iconos Lucide (DollarSign, Users, TrendingUp)
- [ ] Crear `BackingsTable.tsx`
  - [ ] shadcn/ui Table
  - [ ] Search Input
  - [ ] Reward Filter (Select)
  - [ ] Loading states (Skeleton rows)
  - [ ] Empty state
  - [ ] Lógica de filtrado client-side

### 13.5 Fase 5: Página (20 min)
- [ ] Crear `page.tsx`
- [ ] Breadcrumb navigation
- [ ] useParams para obtener `id`
- [ ] Cargar datos con hooks (stats, backings, campania)
- [ ] Renderizar `BackingStatsGrid`
- [ ] Renderizar `BackingsTable`
- [ ] Loading states
- [ ] Error states

### 13.6 Fase 6: Navegación (5 min)
- [ ] Agregar link "Ver Apoyos" en `campanias/[id]/page.tsx`
- [ ] Verificar routing funciona correctamente

### 13.7 Fase 7: Testing (Opcional - 30 min)
- [ ] Test `BackingStatsGrid.test.tsx`
- [ ] Test `BackingsTable.test.tsx`
- [ ] Test `use-backings.test.ts`
- [ ] Ejecutar `npm run test`

### 13.8 Fase 8: Refinamiento (10 min)
- [ ] Revisar estilos (responsive, dark mode)
- [ ] Verificar accesibilidad (ARIA labels, keyboard nav)
- [ ] Verificar que formatos de moneda y fecha funcionan
- [ ] Probar con datos vacíos
- [ ] Probar con datos grandes (>50 backings)

---

## 14. Estimación de Tiempo

| Fase | Tiempo Estimado | Prioridad |
|------|----------------|-----------|
| Estructura | 15 min | Alta |
| Services | 10 min | Alta |
| Hooks | 10 min | Alta |
| Componentes | 30 min | Alta |
| Página | 20 min | Alta |
| Navegación | 5 min | Media |
| Testing | 30 min | Baja |
| Refinamiento | 10 min | Media |
| **TOTAL** | **2 horas** | - |

**Tiempo mínimo MVP (sin tests):** ~1.5 horas
**Tiempo completo (con tests):** ~2 horas

---

## 15. Notas Técnicas

### 15.1 Paginación

**MVP:** No implementar paginación. Cargar todos los backings (default backend: 20 items).

**Post-MVP:** Si se requiere paginación, agregar:
```typescript
// Page state
const [page, setPage] = useState(1)
const [pageSize] = useState(20)

// Hook con params
const { data } = useCampaignBackings(campaniaId, { pageNumber: page, pageSize })

// Componente Pagination
<Pagination
  currentPage={data.page}
  totalPages={data.totalPages}
  onPageChange={setPage}
/>
```

### 15.2 Export CSV

**MVP:** Botón placeholder (no funcional).

**Post-MVP:** Implementación:
```typescript
const handleExport = () => {
  const csvContent = [
    ["Backer", "Monto", "Recompensa", "Fecha"],
    ...backings.map(b => [b.nombreBacker, b.monto, b.rewardNombre, b.fechaCreacion])
  ]
    .map(row => row.join(","))
    .join("\n")

  const blob = new Blob([csvContent], { type: "text/csv" })
  const url = URL.createObjectURL(blob)
  const link = document.createElement("a")
  link.href = url
  link.download = `backings-${campaniaId}.csv`
  link.click()
}
```

### 15.3 Formateo de Datos

**Fechas:**
```typescript
import { formatDate } from "@shared/utils"

// "hace 2 días"
formatDate(backing.fechaCreacion, "relative")

// "15 Ene 2025"
formatDate(backing.fechaCreacion, "short")

// "15 de Enero de 2025, 14:32"
formatDate(backing.fechaCreacion, "long")
```

**Moneda:**
```typescript
import { formatCurrency } from "@shared/utils"

// "€50.00"
formatCurrency(backing.monto)

// "$50.00"
formatCurrency(backing.monto, "USD")
```

### 15.4 Filtrado de Backings

**Client-side (MVP):**
- Búsqueda por nombre (case-insensitive)
- Filtro por reward (select)
- Máximo ~100 backings en memoria

**Server-side (Post-MVP):**
Si hay >100 backings, mover filtros al backend:
```typescript
// Agregar params a endpoint
GET /api/campanias/{id}/backings?search=juan&rewardId=123
```

### 15.5 Badges de Estado

**MVP:** Solo badge "Confirmado" (todos los backings tienen estado COMPLETADO).

**Post-MVP:** Agregar estados:
```typescript
const getBadgeVariant = (estadoPedidoId: number) => {
  switch (estadoPedidoId) {
    case ESTADO_PEDIDO.PENDIENTE:
      return "warning"
    case ESTADO_PEDIDO.COMPLETADO:
      return "success"
    case ESTADO_PEDIDO.FALLIDO:
      return "destructive"
    default:
      return "secondary"
  }
}
```

### 15.6 Refresh Data

**Auto-refresh stats:**
```typescript
// En useCampaignStats
refetchInterval: 30000, // 30 segundos
```

**Manual refresh:**
```typescript
const { refetch } = useCampaignBackings(campaniaId)

<Button variant="outline" onClick={() => refetch()}>
  Actualizar
</Button>
```

---

## 16. Consideraciones de Seguridad

### 16.1 Autorización

**Validar en Backend:** Solo el artista propietario de la campaña puede ver sus backings.

**Frontend:** Confiar en el backend. Si el backend retorna 403, mostrar mensaje de error.

### 16.2 Datos Sensibles

**No exponer:**
- Emails de backers
- Métodos de pago
- Direcciones de envío

**Mostrar solo:**
- Nombre público (o "Anónimo")
- Monto
- Recompensa seleccionada
- Fecha
- Estado

### 16.3 XSS Prevention

**Sanitizar nombres de backers:**
- El backend debe sanitizar antes de guardar
- Frontend usa React (auto-escapes por defecto)
- No usar `dangerouslySetInnerHTML` para datos de backers

---

## 17. Accesibilidad

### 17.1 Semantic HTML

```tsx
<table role="table" aria-label="Lista de apoyos recibidos">
  <thead>
    <tr>
      <th scope="col">Backer</th>
      <th scope="col">Monto</th>
      {/* ... */}
    </tr>
  </thead>
  {/* ... */}
</table>
```

### 17.2 Keyboard Navigation

- Tab order lógico (Search → Filter → Table)
- Enter en row (futuro: abrir modal detalle)
- Arrow keys en Select (reward filter)

### 17.3 Screen Readers

```tsx
<Button
  variant="outline"
  onClick={handleExport}
  aria-label="Exportar lista de apoyos en formato CSV"
>
  Exportar CSV
</Button>

<Input
  placeholder="Buscar por nombre..."
  aria-label="Buscar apoyos por nombre del backer"
  value={searchTerm}
  onChange={...}
/>
```

### 17.4 Color Contrast

- Badges: verificar contraste 4.5:1 mínimo
- Stats values: text-2xl bold con contraste alto
- Table text: usar `text-muted-foreground` para campos secundarios

---

## 18. Responsive Design

### 18.1 Breakpoints

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Stats grid 1 col, tabla scroll horizontal, filtros stack vertical |
| **Tablet (640-1024px)** | Stats grid 2 cols, tabla scroll horizontal |
| **Desktop (> 1024px)** | Stats grid 3 cols, tabla full width sin scroll |

### 18.2 Table Responsiveness

**Mobile:**
```tsx
<div className="overflow-x-auto">
  <Table className="min-w-[600px]">
    {/* ... */}
  </Table>
</div>
```

**Desktop:**
```tsx
<Table>
  {/* Full width, no scroll */}
</Table>
```

---

## 19. Próximos Pasos

Después de implementar este plan:

1. **Backend:** Ejecutar agente de backend para crear:
   - `GetCampaniaBackingsQuery.cs` + Handler
   - `GetCampaniaStatsQuery.cs` + Handler
   - Endpoints en `CampaniasController`

2. **Landing:** Implementar flujo de backings públicos (US-04 completa)

3. **Testing E2E:** Crear test de flujo completo:
   - Fan crea backing en landing
   - Artista ve backing en dashboard/backings
   - Stats se actualizan correctamente

4. **Monitoring:** Agregar logs/analytics:
   - Track cuántos artistas acceden a `/backings`
   - Track errores de carga de datos

---

## 20. Conclusión

Este plan cubre el impacto BAJO del Admin para la feature "hacer-backing". Se enfoca en:

✅ Vista de solo lectura de backings recibidos
✅ Estadísticas básicas (Total Raised, Total Backers, Avg Amount)
✅ Tabla simple con filtros básicos
✅ Reutilización de arquitectura existente (hooks, services, componentes)

❌ NO incluye:
- Gestión de entregas
- Mensajería con backers
- Analytics avanzados
- Notificaciones en tiempo real

**Tiempo de implementación estimado:** 2 horas (1.5h sin tests)

**Archivos totales:** 8 archivos (4 nuevos componentes, 1 página, 1 hook file, 1 service modificado, 1 test opcional)

**Dependencias:** Todas disponibles en shared (types, constants, utils)

**Riesgo:** Bajo - Arquitectura probada, solo lectura, sin lógica de negocio compleja

---

**Fin del Plan Frontend Admin - Hacer Backing**
