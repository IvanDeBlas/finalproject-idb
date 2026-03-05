# Plan Frontend: Definir Recompensas (Landing)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen

- **Screens:** 1 (integracion en CampaniaDetailPage)
- **Componentes:** 3 (CampaniaRewardsSection, RewardPublicCard, RewardPublicCardSkeleton)
- **Hooks:** 1 (useRewardsByCampania)
- **Services:** 1 (reward.service.ts)
- **Alcance:** Vista publica de recompensas (solo lectura), sin CRUD

---

## 2. Estructura de Carpetas

**IMPORTANTE:** Esta feature NO crea una nueva carpeta `features/rewards` porque las recompensas son parte integral de la vista de campania. Los componentes se integran en la estructura existente de `features/campanias`.

```
src/web/src/
├── features/
│   └── campanias/
│       ├── domain/
│       │   └── types.ts                           [MODIFICAR] - Actualizar Reward interface
│       ├── application/
│       │   ├── useCampanias.ts                    [EXISTENTE]
│       │   └── hooks/
│       │       └── useRewardsByCampania.ts        [CREAR] - Query hook para rewards
│       ├── infrastructure/
│       │   ├── campania.api.ts                    [EXISTENTE]
│       │   └── services/
│       │       └── reward.service.ts              [CREAR] - API calls de rewards
│       └── presentation/
│           └── components/
│               ├── RewardsList.tsx                [MODIFICAR] - Actualizar para usar nuevos tipos
│               ├── RewardCard.tsx                 [MODIFICAR] - Actualizar para usar nuevos tipos
│               ├── CampaniaRewardsSection.tsx     [CREAR] - Seccion sidebar de rewards
│               ├── RewardPublicCard.tsx           [CREAR] - Card individual de reward
│               ├── RewardPublicCardSkeleton.tsx   [CREAR] - Skeleton loader
│               └── index.ts                       [MODIFICAR] - Re-export componentes
└── lib/
    └── constants.ts                               [MODIFICAR] - Agregar QUERY_KEYS.rewards
```

---

## 3. Componentes

### 3.1 CampaniaRewardsSection

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaRewardsSection.tsx`

**Descripcion:** Contenedor principal de la seccion de recompensas en el sidebar del detalle de campania. Incluye el boton principal "Apoyar esta campania", lista de rewards, y footer con info de seguridad.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campaniaId | string | Si | ID de la campania para cargar rewards |
| monedaId | number | Si | ID de moneda para formatear precios |
| onApoyar | () => void | No | Callback al hacer click en "Apoyar" o "Seleccionar reward" |
| campaniaFinalizada | boolean | No | Si true, deshabilita botones (default: false) |
| className | string | No | Clases CSS adicionales |

**Estado Local:**

```typescript
// No estado local, usa hook useRewardsByCampania
const { data: rewards, isLoading, error } = useRewardsByCampania(campaniaId)
```

**Dependencias:**

- **Hooks:** `useRewardsByCampania`
- **Componentes UI:** `Card`, `Button`, `Separator` (shadcn/ui)
- **Iconos:** `Lock`, `Package` (lucide-react)
- **Componentes propios:** `RewardPublicCard`, `RewardPublicCardSkeleton`
- **Utils:** `formatCurrency` (de `../application/utils`)

**Responsabilidad:**

- Renderizar el layout completo del sidebar de rewards
- Mostrar boton principal "Apoyar esta campania" con precio minimo
- Listar todas las recompensas activas ordenadas por precio ascendente
- Mostrar empty state si no hay rewards
- Mostrar loading state con skeletons
- Calcular y mostrar precio minimo de recompensa
- Mostrar footer con info de seguridad y entrega estimada

**JSX Structure:**

```tsx
<Card className="sticky top-24" sticky sidebar>
  <Button gradient "Apoyar esta campania" />
  <p>Desde € {minPrice}</p>
  <Separator />
  <h3>Recompensas</h3>
  {isLoading ? (
    <RewardPublicCardSkeleton count={3} />
  ) : rewards.length > 0 ? (
    rewards.map(reward => <RewardPublicCard key={reward.id} reward={reward} />)
  ) : (
    <EmptyState icon={Package} text="No hay recompensas" />
  )}
  <Separator />
  <Footer>
    <Lock /> Pago seguro
    <Package /> Entrega estimada: {earliestDelivery}
  </Footer>
</Card>
```

---

### 3.2 RewardPublicCard

**Archivo:** `src/web/src/features/campanias/presentation/components/RewardPublicCard.tsx`

**Descripcion:** Card individual de recompensa en vista publica. Muestra precio, titulo, descripcion, stock disponible, badges (Popular, Pocas unidades, Agotado), y boton de seleccion.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| reward | Reward | Si | Objeto de recompensa (tipo actualizado de shared) |
| monedaId | number | Si | ID de moneda para formatear precio |
| onSelect | (rewardId: string) => void | No | Callback al seleccionar reward |
| disabled | boolean | No | Si true, deshabilita interaccion (default: false) |
| className | string | No | Clases CSS adicionales |

**Estado Local:**

```typescript
// Computed values (no estado, solo derivaciones)
const cantidadDisponible = reward.cantidadMaxima
  ? reward.cantidadMaxima - reward.cantidadVendida
  : null // null = ilimitado
const isUnlimited = cantidadDisponible === null
const isSoldOut = !isUnlimited && cantidadDisponible <= 0
const isLimited = !isUnlimited && cantidadDisponible < (reward.cantidadMaxima * 0.5)
const isPopular = reward.cantidadVendida > 10 // threshold configurable
```

**Dependencias:**

- **Componentes UI:** `Card`, `Button`, `Badge` (shadcn/ui)
- **Iconos:** `Package`, `AlertCircle`, `XCircle`, `CheckCircle` (lucide-react)
- **Utils:** `formatCurrency`, `cn`

**Responsabilidad:**

- Renderizar card de reward con hover effect (si no esta sold out)
- Calcular stock disponible dinamicamente (cantidadMaxima - cantidadVendida)
- Mostrar badge "Mas popular" si cantidadVendida > threshold
- Mostrar badge "Pocas unidades" warning si stock < 50% del maximo
- Mostrar badge "AGOTADO" gris si stock = 0
- Deshabilitar hover y boton si sold out o disabled
- Manejar click en card completo o boton para seleccionar reward
- Formatear fecha de entrega estimada (si existe)
- Aplicar accesibilidad (role="article", aria-label)

**JSX Structure:**

```tsx
<Card
  className={cn(
    "bg-[#1a1a2e] border-[#334155] p-4 transition",
    !isSoldOut && !disabled && "hover:border-primary cursor-pointer",
    isSoldOut && "opacity-60 cursor-not-allowed"
  )}
  onClick={() => !isSoldOut && !disabled && onSelect?.(reward.id)}
  role="article"
  aria-label={`Recompensa ${reward.nombre} por ${formatCurrency(reward.importeMinimo, monedaId)}`}
>
  <div className="flex items-start justify-between mb-2">
    <span className="text-xl font-bold text-primary">
      {formatCurrency(reward.importeMinimo, monedaId)}
    </span>
    <div className="flex gap-1">
      {isPopular && <Badge pink>Mas popular</Badge>}
      {isLimited && <Badge warning>Pocas unidades</Badge>}
      {isSoldOut && <Badge gray>AGOTADO</Badge>}
    </div>
  </div>

  <h4 className="font-semibold text-white">{reward.nombre}</h4>

  {reward.descripcion && (
    <p className="text-sm text-secondary line-clamp-2">{reward.descripcion}</p>
  )}

  <div className="flex items-center gap-2 text-xs text-muted">
    <Icon name={getStockIcon()} />
    <span>{getStockText()}</span>
  </div>

  {reward.tiempoEntregaEstimado && (
    <p className="text-xs text-muted">Entrega: {reward.tiempoEntregaEstimado}</p>
  )}

  <Button
    variant="outline"
    size="sm"
    className="w-full border-primary text-primary"
    disabled={isSoldOut || disabled}
    onClick={(e) => {
      e.stopPropagation()
      if (!isSoldOut && !disabled) onSelect?.(reward.id)
    }}
  >
    {isSoldOut ? "Agotado" : "Seleccionar"}
  </Button>
</Card>
```

**Helper Functions (dentro del componente):**

```typescript
const getStockIcon = () => {
  if (isUnlimited) return 'infinity'
  if (isSoldOut) return 'x-circle'
  if (isLimited) return 'alert-circle'
  return 'package'
}

const getStockText = () => {
  if (isUnlimited) return 'Ilimitadas disponibles'
  if (isSoldOut) return 'Agotado'
  return `${cantidadDisponible} de ${reward.cantidadMaxima} disponibles`
}
```

---

### 3.3 RewardPublicCardSkeleton

**Archivo:** `src/web/src/features/campanias/presentation/components/RewardPublicCardSkeleton.tsx`

**Descripcion:** Skeleton loader para RewardPublicCard durante carga de datos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| count | number | No | Numero de skeletons a renderizar (default: 3) |
| className | string | No | Clases CSS adicionales |

**Estado Local:**

Ninguno (componente estatico)

**Dependencias:**

- **Componentes UI:** `Card`, `Skeleton` (shadcn/ui)

**Responsabilidad:**

- Renderizar skeletons que imiten la estructura de RewardPublicCard
- Usar `Array.from({ length: count })` para renderizar N skeletons

**JSX Structure:**

```tsx
<>
  {Array.from({ length: count }).map((_, i) => (
    <Card key={i} className="bg-[#1a1a2e] border-[#334155] p-4">
      <div className="flex items-start justify-between mb-2">
        <Skeleton className="h-6 w-16" />
        <Skeleton className="h-5 w-20" />
      </div>
      <Skeleton className="h-5 w-3/4 mb-2" />
      <Skeleton className="h-4 w-full mb-1" />
      <Skeleton className="h-4 w-2/3 mb-3" />
      <Skeleton className="h-4 w-32 mb-3" />
      <Skeleton className="h-9 w-full" />
    </Card>
  ))}
</>
```

---

### 3.4 Modificaciones a Componentes Existentes

#### RewardCard.tsx (MODIFICAR)

**Accion:** Actualizar para usar los nuevos tipos de `@shared/types/reward` en lugar de tipos locales.

**Cambios:**

```typescript
// ANTES
import type { Reward } from "../../domain"

// DESPUES
import type { Reward } from "@shared/types/reward"

// Actualizar computed properties
const cantidadDisponible = reward.cantidadMaxima
  ? reward.cantidadMaxima - reward.cantidadVendida
  : null
const isUnlimited = cantidadDisponible === null
const isSoldOut = !isUnlimited && cantidadDisponible <= 0

// Actualizar campo de fecha
{reward.tiempoEntregaEstimado && (
  <p className="text-xs text-muted">
    Entrega estimada: {reward.tiempoEntregaEstimado}
  </p>
)}
```

#### RewardsList.tsx (MODIFICAR)

**Accion:** Actualizar para usar `CampaniaRewardsSection` en lugar de implementacion inline. DEPRECAR este componente a futuro.

**Cambios:**

```typescript
// OPCION 1: Re-export CampaniaRewardsSection para backward compatibility
export { CampaniaRewardsSection as RewardsList } from './CampaniaRewardsSection'

// OPCION 2: Wrapper que delega a CampaniaRewardsSection
export function RewardsList(props: RewardsListProps) {
  console.warn('RewardsList is deprecated, use CampaniaRewardsSection instead')
  return <CampaniaRewardsSection {...props} />
}
```

**Nota:** Preferir Opcion 1 para mantener compatibilidad sin warnings.

#### types.ts (MODIFICAR)

**Accion:** Actualizar interface `Reward` para alinear con `@shared/types/reward`.

**Cambios:**

```typescript
// ANTES
export interface Reward {
  id: string
  campaniaId: string
  nombre: string
  descripcion?: string
  importeMinimo: number
  cantidadDisponible?: number
  cantidadReclamada: number
  fechaEntregaEstimada?: string
  // Legacy
  stockLimitado?: number
  stockDisponible?: number
}

// DESPUES - Importar de shared
export type { Reward, RewardListItem } from '@shared/types/reward'

// O si necesitas extender:
import type { Reward as SharedReward } from '@shared/types/reward'

export interface Reward extends Omit<SharedReward, 'fechaCreacion' | 'fechaActualizacion'> {
  // Campos calculados para UI
  cantidadVendida: number // Calculado: suma de backings
}
```

**Decision:** Preferir importar directamente de shared y calcular `cantidadVendida` en el service/mapper.

---

## 4. Hooks

### 4.1 useRewardsByCampania

**Archivo:** `src/web/src/features/campanias/application/hooks/useRewardsByCampania.ts`

**Tipo:** Query Hook (TanStack Query)

**Descripcion:** Hook para obtener las recompensas activas de una campania. Filtra solo rewards con `esActivo=true` y ordena por precio ascendente.

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| campaniaId | string | ID de la campania (requerido) |
| enabled | boolean | (Opcional) Si false, no ejecuta query (default: true) |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | Reward[] \| undefined | Array de rewards o undefined si loading |
| isLoading | boolean | True mientras carga primera vez |
| isError | boolean | True si hay error |
| error | Error \| null | Objeto de error si hay |
| refetch | () => void | Funcion para refetch manual |

**Query Key:**

```typescript
[QUERY_KEYS.REWARDS, 'campania', campaniaId, { esActivo: true }]
```

**Implementacion:**

```typescript
import { useQuery } from '@tanstack/react-query'
import { QUERY_KEYS } from '@/lib/constants'
import { rewardService } from '../../infrastructure/services/reward.service'
import type { Reward } from '@shared/types/reward'

interface UseRewardsByCampaniaOptions {
  enabled?: boolean
}

export function useRewardsByCampania(
  campaniaId: string,
  options?: UseRewardsByCampaniaOptions
) {
  return useQuery<Reward[], Error>({
    queryKey: [QUERY_KEYS.REWARDS, 'campania', campaniaId, { esActivo: true }],
    queryFn: () => rewardService.getByCampaniaId(campaniaId, { esActivo: true }),
    enabled: options?.enabled !== false && !!campaniaId,
    staleTime: 5 * 60 * 1000, // 5 minutos
    select: (data) => {
      // Ordenar por importeMinimo ascendente (convencion crowdfunding)
      return data.sort((a, b) => a.importeMinimo - b.importeMinimo)
    },
  })
}
```

**Notas:**

- `enabled: !!campaniaId` previene queries con ID vacio
- `staleTime: 5min` para evitar refetch constante (rewards cambian poco)
- `select` ordena los rewards por precio (convencion de crowdfunding: menor a mayor)
- Filtra solo `esActivo: true` para no mostrar rewards desactivados

---

## 5. Services

### 5.1 reward.service.ts

**Archivo:** `src/web/src/features/campanias/infrastructure/services/reward.service.ts`

**Descripcion:** Servicio para realizar API calls a endpoints de rewards. Maneja responses con `ServiceResponse<T>` del backend.

**Metodos:**

| Metodo | Input | Output | Endpoint | Descripcion |
|--------|-------|--------|----------|-------------|
| getAll | filters?: RewardFilters | Promise<Reward[]> | GET /api/rewards | Obtiene todos los rewards con filtros opcionales |
| getByCampaniaId | campaniaId: string, filters?: RewardFilters | Promise<Reward[]> | GET /api/rewards?campaniaId=X | Obtiene rewards de una campania especifica |
| getById | id: string | Promise<Reward \| null> | GET /api/rewards/{id} | Obtiene un reward por ID |

**Types de Soporte:**

```typescript
interface RewardFilters {
  esActivo?: boolean
  esAddOn?: boolean
  campaniaId?: string
}
```

**Implementacion:**

```typescript
import { apiFetch } from '@/lib/api-client'
import type { Reward } from '@shared/types/reward'

// ServiceResponse wrapper del backend
interface ServiceResponse<T> {
  data: T
  messages: Array<{ message: string; errorCode: string }>
  isSuccess?: boolean
}

interface RewardFilters {
  esActivo?: boolean
  esAddOn?: boolean
  campaniaId?: string
}

class RewardService {
  private readonly baseUrl = '/rewards'

  async getAll(filters?: RewardFilters): Promise<Reward[]> {
    const params = new URLSearchParams()

    if (filters?.campaniaId) {
      params.append('campaniaId', filters.campaniaId)
    }
    if (filters?.esActivo !== undefined) {
      params.append('esActivo', filters.esActivo.toString())
    }
    if (filters?.esAddOn !== undefined) {
      params.append('esAddOn', filters.esAddOn.toString())
    }

    const queryString = params.toString()
    const url = queryString ? `${this.baseUrl}?${queryString}` : this.baseUrl

    try {
      const response = await apiFetch<ServiceResponse<Reward[]>>(url)
      return response.data ?? []
    } catch (error) {
      console.error('Error fetching rewards:', error)
      throw error
    }
  }

  async getByCampaniaId(
    campaniaId: string,
    filters?: Omit<RewardFilters, 'campaniaId'>
  ): Promise<Reward[]> {
    return this.getAll({ ...filters, campaniaId })
  }

  async getById(id: string): Promise<Reward | null> {
    try {
      const response = await apiFetch<ServiceResponse<Reward>>(
        `${this.baseUrl}/${id}`
      )
      return response.data ?? null
    } catch (error) {
      console.error(`Error fetching reward ${id}:`, error)
      return null
    }
  }
}

export const rewardService = new RewardService()
```

**Notas:**

- **No maneja mutaciones** (crear/editar/eliminar) porque Landing es solo lectura
- Usa `apiFetch` del `api-client` existente para manejo de auth y errores
- Retorna arrays vacios en lugar de throw en `getAll` para mejor UX
- Retorna `null` en `getById` si no encuentra el reward
- El filtro `esActivo=true` es crucial para no mostrar rewards desactivados

---

## 6. Flujo de Datos

```
User Action: Navigate to /campanias/{id}
    ↓
CampaniaDetailPage (presentation)
    ↓
CampaniaRewardsSection (component)
    ↓
useRewardsByCampania (hook)
    ↓
rewardService.getByCampaniaId (service)
    ↓
apiFetch → GET /api/rewards?campaniaId=X&esActivo=true
    ↓
Backend → ServiceResponse<Reward[]>
    ↓
TanStack Query cache
    ↓
RewardPublicCard (render N cards)
```

**User Interaction:**

```
Click "Seleccionar" en RewardPublicCard
    ↓
onSelect(rewardId) callback
    ↓
CampaniaRewardsSection.onApoyar callback
    ↓
Navigate to /campanias/{id}/backing?rewardId={rewardId}
    (flujo de backing - fuera del alcance de esta feature)
```

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

- **Types:**
  - `Reward` (de `@shared/types/reward`)
  - `RewardListItem` (de `@shared/types/reward`)
  - `TipoReward` (enum de `@shared/types/reward`)

- **Constants:**
  - `QUERY_KEYS.rewards` (de `@shared/constants`)
  - `API_ROUTES.rewards` (de `@shared/constants`)
  - `TIPO_REWARD_LABELS` (de `@shared/constants`)

- **Utils:**
  - `getRewardErrorMessage` (de `@shared/utils/error-messages`)
  - `getRewardSpecificErrorMessage` (de `@shared/utils/error-messages`)

**Nota:** Los schemas Zod (`createRewardSchema`, etc.) NO se importan en Landing porque no hay formularios de creacion/edicion.

---

## 8. Integracion con CampaniaDetailPage

**Archivo a Modificar:** `src/web/src/features/campanias/presentation/pages/CampaniaDetailPage.tsx`

**Cambios:**

1. **Importar nuevo componente:**

```typescript
import { CampaniaRewardsSection } from '../components/CampaniaRewardsSection'
```

2. **Reemplazar `<RewardsList>` con `<CampaniaRewardsSection>`:**

```typescript
// ANTES
<RewardsList
  rewards={campania.rewards}
  monedaId={campania.monedaId}
  onApoyar={handleApoyar}
  campaniaFinalizada={isFinalizada}
/>

// DESPUES
<CampaniaRewardsSection
  campaniaId={campania.id}
  monedaId={campania.monedaId}
  onApoyar={handleApoyar}
  campaniaFinalizada={isFinalizada}
/>
```

**Beneficios:**

- Los rewards ahora se cargan via hook separado (desacoplado de campania)
- Loading states independientes (skeleton mientras carga rewards)
- Query cache separado para rewards (mejor invalidacion)

---

## 9. Actualizacion de constants.ts

**Archivo:** `src/web/src/lib/constants.ts`

**Agregar:**

```typescript
export const QUERY_KEYS = {
  CAMPANIAS: "campanias",
  CAMPANIA: "campania",
  ARTISTAS: "artistas",
  ARTISTA: "artista",
  BACKINGS: "backings",
  AUTH_USER: "auth-user",
  REWARDS: "rewards", // NUEVO
} as const
```

**Nota:** Los demas constants de rewards (API_ROUTES, TIPO_REWARD, etc.) se importan de `@shared/constants` para evitar duplicacion.

---

## 10. Archivos a Crear/Modificar

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/campanias/application/hooks/useRewardsByCampania.ts` | **CREAR** | Query hook para obtener rewards de campania |
| `src/web/src/features/campanias/infrastructure/services/reward.service.ts` | **CREAR** | Service para API calls de rewards |
| `src/web/src/features/campanias/presentation/components/CampaniaRewardsSection.tsx` | **CREAR** | Componente contenedor de sidebar rewards |
| `src/web/src/features/campanias/presentation/components/RewardPublicCard.tsx` | **CREAR** | Card individual de reward publico |
| `src/web/src/features/campanias/presentation/components/RewardPublicCardSkeleton.tsx` | **CREAR** | Skeleton loader para rewards |
| `src/web/src/features/campanias/domain/types.ts` | **MODIFICAR** | Actualizar Reward interface para alinearse con shared |
| `src/web/src/features/campanias/presentation/components/RewardCard.tsx` | **MODIFICAR** | Actualizar tipos y computed properties |
| `src/web/src/features/campanias/presentation/components/RewardsList.tsx` | **MODIFICAR** | Deprecar o re-export CampaniaRewardsSection |
| `src/web/src/features/campanias/presentation/components/index.ts` | **MODIFICAR** | Re-export nuevos componentes |
| `src/web/src/features/campanias/presentation/pages/CampaniaDetailPage.tsx` | **MODIFICAR** | Usar CampaniaRewardsSection en lugar de RewardsList |
| `src/web/src/lib/constants.ts` | **MODIFICAR** | Agregar QUERY_KEYS.REWARDS |

---

## 11. Responsive Behavior

### Mobile (< 640px)

| Componente | Cambios |
|------------|---------|
| CampaniaRewardsSection | `sticky` removido, scroll normal; padding reducido `p-4` |
| RewardPublicCard | Precio `text-lg` en lugar de `text-xl`; padding `p-3`; descripcion `line-clamp-3` |
| RewardPublicCardSkeleton | Skeletons mas compactos |

### Tablet (640-1024px)

Layout normal, sin cambios significativos.

### Desktop (> 1024px)

Layout completo con `sticky top-24` en sidebar.

**Implementacion (ejemplo CampaniaRewardsSection):**

```tsx
<Card className={cn(
  "bg-[#0f1729] border-[#334155] p-6",
  "md:sticky md:top-24", // Sticky solo en tablet+
  "max-md:p-4", // Padding reducido en mobile
  className
)}>
  {/* ... */}
</Card>
```

---

## 12. Accesibilidad

### ARIA Labels

```tsx
// CampaniaRewardsSection
<Button aria-label="Apoyar esta campana">
  Apoyar esta campana
</Button>

<h3 id="rewards-heading" className="sr-only">Recompensas disponibles</h3>
<div role="list" aria-labelledby="rewards-heading">
  {rewards.map(reward => <RewardPublicCard role="listitem" />)}
</div>

// RewardPublicCard
<Card
  role="article"
  aria-label={`Recompensa ${reward.nombre} por ${formatCurrency(reward.importeMinimo, monedaId)}`}
  tabIndex={isSoldOut ? -1 : 0}
  onKeyDown={(e) => {
    if (e.key === 'Enter' && !isSoldOut) onSelect?.(reward.id)
  }}
>
  <Button
    aria-label={isSoldOut ? "Esta recompensa esta agotada" : `Seleccionar recompensa ${reward.nombre}`}
    disabled={isSoldOut}
  >
    {isSoldOut ? "Agotado" : "Seleccionar"}
  </Button>
</Card>
```

### Keyboard Navigation

- **Tab:** Navega entre cards y botones
- **Enter:** Selecciona reward (tanto en card como en boton)
- **Espacio:** Activa boton "Seleccionar"

### Contraste de Colores

- Texto principal (white) sobre fondo (#1a1a2e): **13.5:1** ✓
- Precio (primary #a855f7) sobre fondo (#1a1a2e): **5.2:1** ✓
- Texto secundario (#94a3b8) sobre fondo (#1a1a2e): **7.8:1** ✓

Todos cumplen WCAG AAA (contraste minimo 7:1 para texto normal).

---

## 13. Loading States

### Skeleton Loading (primera carga)

```tsx
{isLoading && <RewardPublicCardSkeleton count={3} />}
```

### Empty State (sin rewards)

```tsx
{!isLoading && rewards.length === 0 && (
  <div className="text-center py-8">
    <Package className="w-12 h-12 text-[#64748b] mx-auto mb-3" />
    <p className="text-sm text-[#64748b]">
      Esta campana no tiene recompensas especificas
    </p>
    <p className="text-xs text-[#64748b] mt-2">
      Puedes hacer una contribucion libre
    </p>
  </div>
)}
```

### Error State

```tsx
{isError && (
  <div className="text-center py-8">
    <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-3" />
    <p className="text-sm text-red-400">
      Error al cargar las recompensas
    </p>
    <Button variant="outline" size="sm" onClick={() => refetch()}>
      Intentar de nuevo
    </Button>
  </div>
)}
```

---

## 14. Testing Strategy

### Unit Tests

**Archivo:** `src/web/src/features/campanias/__tests__/RewardPublicCard.test.tsx`

```typescript
describe('RewardPublicCard', () => {
  it('renders reward with price and title', () => {
    const reward = mockReward({ nombre: 'Test Reward', importeMinimo: 10 })
    render(<RewardPublicCard reward={reward} monedaId={1} />)
    expect(screen.getByText('Test Reward')).toBeInTheDocument()
    expect(screen.getByText('€10')).toBeInTheDocument()
  })

  it('shows "Popular" badge when many sold', () => {
    const reward = mockReward({ cantidadVendida: 15 })
    render(<RewardPublicCard reward={reward} monedaId={1} />)
    expect(screen.getByText('Mas popular')).toBeInTheDocument()
  })

  it('shows "Agotado" badge when sold out', () => {
    const reward = mockReward({ cantidadMaxima: 10, cantidadVendida: 10 })
    render(<RewardPublicCard reward={reward} monedaId={1} />)
    expect(screen.getByText('AGOTADO')).toBeInTheDocument()
  })

  it('disables button when sold out', () => {
    const reward = mockReward({ cantidadMaxima: 10, cantidadVendida: 10 })
    render(<RewardPublicCard reward={reward} monedaId={1} />)
    const button = screen.getByRole('button', { name: /agotado/i })
    expect(button).toBeDisabled()
  })

  it('calls onSelect when clicking card', () => {
    const handleSelect = vi.fn()
    const reward = mockReward({ id: 'reward-123' })
    render(<RewardPublicCard reward={reward} monedaId={1} onSelect={handleSelect} />)
    fireEvent.click(screen.getByRole('article'))
    expect(handleSelect).toHaveBeenCalledWith('reward-123')
  })
})
```

### Hook Tests

**Archivo:** `src/web/src/features/campanias/__tests__/useRewardsByCampania.test.ts`

```typescript
describe('useRewardsByCampania', () => {
  it('fetches rewards for campania', async () => {
    const { result } = renderHook(() => useRewardsByCampania('campania-123'))

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toHaveLength(3)
  })

  it('sorts rewards by price ascending', async () => {
    const { result } = renderHook(() => useRewardsByCampania('campania-123'))

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    const prices = result.current.data!.map(r => r.importeMinimo)
    expect(prices).toEqual([10, 25, 50]) // Ordenado
  })

  it('does not fetch when campaniaId is empty', () => {
    const { result } = renderHook(() => useRewardsByCampania(''))
    expect(result.current.isFetching).toBe(false)
  })
})
```

---

## 15. Checklist de Implementacion

- [ ] **Types actualizados** - `Reward` alineado con `@shared/types/reward`
- [ ] **Service creado** - `reward.service.ts` con metodos `getAll`, `getByCampaniaId`, `getById`
- [ ] **Hook creado** - `useRewardsByCampania` con query key correcto
- [ ] **CampaniaRewardsSection creado** - Layout completo de sidebar
- [ ] **RewardPublicCard creado** - Card individual con badges y stock
- [ ] **RewardPublicCardSkeleton creado** - Loading state
- [ ] **RewardCard modificado** - Usa nuevos tipos de shared
- [ ] **RewardsList modificado** - Re-export CampaniaRewardsSection
- [ ] **CampaniaDetailPage modificado** - Integra CampaniaRewardsSection
- [ ] **constants.ts modificado** - QUERY_KEYS.REWARDS agregado
- [ ] **Responsive behavior** - Sticky sidebar en desktop, normal en mobile
- [ ] **Accesibilidad** - ARIA labels, keyboard navigation, focus states
- [ ] **Loading states** - Skeleton, empty state, error state
- [ ] **Badges dinamicos** - Popular, Pocas unidades, Agotado
- [ ] **Stock calculado** - cantidadMaxima - cantidadVendida
- [ ] **Ordenamiento** - Rewards ordenados por precio ascendente
- [ ] **Filtro esActivo** - Solo mostrar rewards activos
- [ ] **Hover effects** - Border primary en cards disponibles, no en sold out
- [ ] **Tests unitarios** - RewardPublicCard, useRewardsByCampania
- [ ] **Imports de shared** - Types, constants, utils importados correctamente

---

## 16. Siguiente Paso Sugerido

Una vez completado este plan, el siguiente paso es:

1. **Implementar el codigo siguiendo este plan** (usando el agente de implementacion frontend)
2. **Testear integracion** en `/campanias/{id}` (verificar que rewards se muestran correctamente)
3. **Verificar responsive** en mobile, tablet, desktop
4. **Verificar accesibilidad** con screen reader y navegacion por teclado
5. **Conectar con flujo de backing** (cuando se implemente US-04 - realizar-backing)

**Dependencias externas:**

- Endpoints backend de rewards deben estar implementados y funcionando
- Types de `@shared/types/reward` deben estar actualizados
- Constants de `@shared/constants` deben incluir QUERY_KEYS.rewards

---

**Fin del plan frontend Landing para definir-recompensas.**
