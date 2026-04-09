# Plan Frontend: Crear Campania (Landing)

**Fecha:** 2026-02-12
**Feature:** crear-campania
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

- Screens: 2 (Explorar Campanias, Detalle Campania Publica)
- Componentes: 8 (CampaniaCard, CampaniaList, CampaniaDetail, ProgressBar, RewardCard, ArtistCard, CampaniaHero, CampaniaStats)
- Hooks: 2 (useCampanias, useCampania)
- Services: 1 (campaniaService con nuevos endpoints)
- Pages: 2 (ExplorarPage, CampaniaDetailPage)

---

## 2. Estructura de Carpetas

```
src/web/src/features/campanias/
├── domain/
│   ├── types.ts                    # Re-export de shared + domain models
│   └── index.ts
├── application/
│   ├── hooks/
│   │   ├── useCampanias.ts        # ACTUALIZAR - agregar filtros y paginacion
│   │   ├── useCampania.ts         # ACTUALIZAR - query individual
│   │   └── index.ts
│   └── index.ts
├── infrastructure/
│   ├── api/
│   │   └── campania.api.ts        # ACTUALIZAR - agregar metodos de filtrado
│   ├── dtos.ts                    # ACTUALIZAR - alinear con contracts.md
│   ├── mappers.ts                 # ACTUALIZAR - mappers para nuevos campos
│   └── index.ts
└── presentation/
    ├── components/
    │   ├── CampaniaCard.tsx           # ACTUALIZAR - usar nuevos campos
    │   ├── CampaniaList.tsx           # ACTUALIZAR - paginacion y filtros
    │   ├── CampaniaHero.tsx           # NUEVO - hero image + video
    │   ├── CampaniaStats.tsx          # NUEVO - stats bar (backers, dias, progreso)
    │   ├── CampaniaProgress.tsx       # NUEVO - progress bar detallado
    │   ├── RewardCard.tsx             # NUEVO - reward con stock y precio
    │   ├── RewardsList.tsx            # NUEVO - sidebar de rewards
    │   ├── ArtistCard.tsx             # NUEVO - mini card del artista
    │   ├── CampaniaTabs.tsx           # NUEVO - tabs Historia/Actualizaciones/etc
    │   ├── CampaniaDescription.tsx    # NUEVO - render rich text HTML
    │   ├── CampaniaListSkeleton.tsx   # NUEVO - loading state
    │   ├── CampaniaDetailSkeleton.tsx # NUEVO - loading state detalle
    │   └── index.ts
    └── pages/
        ├── ExplorarPage.tsx           # NUEVO - /explorar con filtros
        ├── CampaniaDetailPage.tsx     # ACTUALIZAR - layout completo krowd
        └── index.ts
```

---

## 3. Componentes

### 3.1 CampaniaCard (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campania | CampaniaListItem | Si | Datos de la campania |
| variant | 'default' \| 'compact' | No | Variante del card (default: 'default') |
| className | string | No | Clases CSS adicionales |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Hooks: Ninguno
- Componentes UI: Card, CardContent, CardHeader, CardTitle, CardFooter, Progress, Badge, Button
- Utils: formatCurrencyWithSymbol (de shared), calcularDiasRestantes

**Responsabilidad:**
Mostrar una campania en formato card con imagen, titulo, descripcion corta, progress bar, stats (porcentaje, backers, dias restantes) y boton "Ver campania". Variante compact para listas densas.

**Cambios respecto a version actual:**
- Usar `imagenPrincipalUrl` en lugar de `imagenUrl`
- Usar `descripcionCorta` en lugar de `descripcion`
- Agregar badge de estado (estadoCampaniaId)
- Agregar formato de moneda con `monedaId`
- Agregar backers count (importePledgedActual / importeObjetivo)
- Responsive: mobile (w-full), tablet (2 cols), desktop (3 cols)

---

### 3.2 CampaniaList (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaList.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campanias | CampaniaListItem[] | Si | Array de campanias |
| isLoading | boolean | No | Estado de carga |
| error | Error \| null | No | Error si hay |
| variant | 'grid' \| 'list' | No | Layout (default: 'grid') |

**Estado Local:**
- Ninguno

**Dependencias:**
- Hooks: Ninguno
- Componentes: CampaniaCard, CampaniaListSkeleton, Alert (para error)

**Responsabilidad:**
Renderizar lista de campanias en grid o list layout. Manejar estados de loading, error y empty state.

**Cambios respecto a version actual:**
- Agregar paginacion (props adicionales)
- Agregar variant 'list' para vista alternativa
- Agregar empty state con ilustracion y CTA
- Responsive grid: mobile (1 col), tablet (2 cols), desktop (3 cols)

---

### 3.3 CampaniaHero (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaHero.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| imagenUrl | string | Si | URL de imagen principal |
| videoUrl | string | No | URL de video (YouTube/Vimeo) |
| titulo | string | Si | Titulo de la campania |
| alt | string | Si | Alt text para imagen |

**Estado Local:**
- `showVideo: boolean` - Toggle entre imagen y video

**Dependencias:**
- Hooks: useState
- Componentes UI: Button (play icon)
- Utils: extractVideoId (para embed URLs)

**Responsabilidad:**
Mostrar hero image full width con opcion de reproducir video si existe. Hero con height 384px (h-96) en desktop, 256px (h-64) en mobile.

---

### 3.4 CampaniaStats (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaStats.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| importeObjetivo | number | Si | Meta financiera |
| importePledgedActual | number | Si | Monto recaudado |
| monedaId | number | Si | ID de moneda |
| fechaFin | string | Si | Fecha fin ISO 8601 |
| backersCount | number | No | Numero de backers (calcular o dummy) |

**Estado Local:**
- Ninguno

**Dependencias:**
- Utils: formatCurrencyWithSymbol, calcularDiasRestantes, calcularPorcentaje

**Responsabilidad:**
Mostrar stats row con: monto recaudado/objetivo, porcentaje, backers count, dias restantes. Layout horizontal en desktop, wrap en mobile.

---

### 3.5 CampaniaProgress (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaProgress.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| importeObjetivo | number | Si | Meta financiera |
| importePledgedActual | number | Si | Monto recaudado |
| monedaId | number | Si | ID de moneda |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: Progress
- Utils: calcularPorcentaje, formatCurrencyWithSymbol

**Responsabilidad:**
Progress bar detallado con gradient (pink to purple), porcentaje animado, monto recaudado vs objetivo.

---

### 3.6 RewardCard (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/RewardCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| reward | RewardDto | Si | Datos de la recompensa |
| monedaId | number | Si | ID de moneda para formato |
| onSelect | (rewardId: string) => void | No | Callback al seleccionar |
| disabled | boolean | No | Deshabilitar seleccion |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: Card, Badge, Button
- Utils: formatCurrencyWithSymbol

**Responsabilidad:**
Mostrar reward card con precio, titulo, descripcion, stock disponible y boton "Seleccionar". Badge "Mas popular" si aplica. Badge "Agotado" si stock = 0.

---

### 3.7 RewardsList (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/RewardsList.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| rewards | RewardDto[] | Si | Array de recompensas |
| monedaId | number | Si | ID de moneda |
| campaniaId | string | Si | ID de campania |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes: RewardCard
- Utils: Ninguno

**Responsabilidad:**
Renderizar lista de rewards en sidebar sticky. Incluir titulo "Recompensas", info de pago seguro y entrega estimada. Empty state si no hay rewards.

---

### 3.8 ArtistCard (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/ArtistCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artistaId | string | Si | ID del artista |
| nombre | string | Si | Nombre artistico |
| avatar | string | No | URL de avatar |
| bio | string | No | Biografia corta |
| verified | boolean | No | Artista verificado |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: Card, Avatar, Badge, Button
- Router: Link (para perfil artista)

**Responsabilidad:**
Mostrar mini card del artista con avatar, nombre, verificado badge, bio corta y boton "Ver perfil". Se usa en sidebar del detalle.

---

### 3.9 CampaniaTabs (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaTabs.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| descripcion | string | Si | Descripcion HTML rich text |
| defaultTab | string | No | Tab inicial (default: 'historia') |

**Estado Local:**
- `activeTab: string` - Tab actualmente activo

**Dependencias:**
- Componentes UI: Tabs, TabsList, TabsTrigger, TabsContent
- Componentes: CampaniaDescription

**Responsabilidad:**
Tabs navigation (Historia, Actualizaciones, Comentarios, FAQ). En MVP solo Historia tiene contenido, el resto muestra "Proximamente".

---

### 3.10 CampaniaDescription (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaDescription.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| htmlContent | string | Si | HTML sanitized de descripcion |

**Estado Local:**
- Ninguno

**Dependencias:**
- Utils: sanitizeHtml (crear o usar DOMPurify)

**Responsabilidad:**
Renderizar rich text HTML de la descripcion de forma segura (XSS protection). Aplicar estilos prose prose-invert de Tailwind.

---

### 3.11 CampaniaListSkeleton (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaListSkeleton.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| count | number | No | Numero de skeletons (default: 6) |

**Responsabilidad:**
Skeleton loader para lista de campanias (cards con shimmer effect).

---

### 3.12 CampaniaDetailSkeleton (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/components/CampaniaDetailSkeleton.tsx`

**Props:**
Ninguna

**Responsabilidad:**
Skeleton loader para detalle de campania (hero, stats, descripcion, sidebar).

---

## 4. Hooks

### 4.1 useCampanias (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/application/hooks/useCampanias.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| filters | CampaniaFilters | Filtros opcionales (searchTerm, artistaId, estadoCampaniaId) |
| pagination | PaginationParams | Paginacion (pageNumber, pageSize) |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | CampaniaListItem[] | Array de campanias |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |
| refetch | () => void | Funcion para refetch |

**Query Key:**
```typescript
['campanias', 'filtered', { ...filters, ...pagination }]
```

**Cambios respecto a version actual:**
- Agregar soporte para filtros (searchTerm, artistaId, estadoCampaniaId)
- Agregar paginacion (pageNumber, pageSize)
- Por defecto filtrar solo campanias publicadas (estadoCampaniaId = 2)
- Usar endpoint GET /api/campanias con query params

---

### 4.2 useCampania (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/application/hooks/useCampania.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID de la campania |
| options | UseQueryOptions | Opciones de useQuery |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | Campania \| null | Datos completos de la campania |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |
| refetch | () => void | Funcion para refetch |

**Query Key:**
```typescript
['campanias', id]
```

**Cambios respecto a version actual:**
- Manejar 404 correctamente (retornar null si no existe)
- Incluir enabled flag (no ejecutar si id vacio)
- Usar Campania type completo (no CampaniaListItem)

---

## 5. Services

### 5.1 campaniaService (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/infrastructure/api/campania.api.ts`

**Cambios a realizar:**
1. Renombrar archivo de `campania.service.ts` a `campania.api.ts` (consistencia con patron)
2. Actualizar DTOs segun contracts.md
3. Agregar metodos de filtrado

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getAll | CampaniaFilters?, PaginationParams? | ServiceResponse<CampaniaListItem[]> | GET /api/campanias?searchTerm=...&estadoCampaniaId=2&pageNumber=1&pageSize=10 |
| getById | id: string | ServiceResponse<Campania> | GET /api/campanias/{id} |
| getByArtistaId | artistaId: string | ServiceResponse<CampaniaListItem[]> | GET /api/campanias?artistaId={id} |

**Ejemplo de implementacion getAll con filtros:**

```typescript
async getAll(
  filters?: CampaniaFilters,
  pagination?: PaginationParams
): Promise<CampaniaListItem[]> {
  const params = new URLSearchParams();

  // Filtros
  if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
  if (filters?.artistaId) params.append('artistaId', filters.artistaId);
  if (filters?.estadoCampaniaId !== undefined) {
    params.append('estadoCampaniaId', filters.estadoCampaniaId.toString());
  } else {
    // Por defecto solo campanias publicadas
    params.append('estadoCampaniaId', CAMPANIA_ESTADOS.PUBLICADA.toString());
  }

  // Paginacion
  if (pagination?.pageNumber) params.append('pageNumber', pagination.pageNumber.toString());
  if (pagination?.pageSize) params.append('pageSize', pagination.pageSize.toString());

  const url = `${this.baseUrl}?${params.toString()}`;
  const response = await apiFetch<ServiceResponse<CampaniaDto[]>>(url);

  if (!response.data.isSuccess) {
    throw new Error(response.data.messages[0]?.message || 'Error al obtener campanias');
  }

  return response.data.data!.map(mapCampaniaDtoToDomain);
}
```

**Types adicionales:**

```typescript
interface CampaniaFilters {
  searchTerm?: string;
  artistaId?: string;
  estadoCampaniaId?: number;
}

interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
}
```

---

## 6. Pages

### 6.1 ExplorarPage (NUEVO)

**Archivo:** `src/web/src/features/campanias/presentation/pages/ExplorarPage.tsx`

**Ruta:** `/explorar`

**Layout:**
```
Header (sticky)
  |
  v
Filtros (searchTerm input + estados dropdown)
  |
  v
CampaniaList (grid 3 cols)
  |
  v
Paginacion
  |
  v
Footer
```

**Estado Local:**
- `filters: CampaniaFilters` - Filtros activos
- `pagination: PaginationParams` - Paginacion actual

**Dependencias:**
- Hooks: useCampanias (con filtros y paginacion), useDebounce (para searchTerm)
- Componentes: CampaniaList, Input (busqueda), Select (filtro estado)

**Responsabilidad:**
Pagina de listado de campanias publicas con filtros y paginacion. Solo muestra campanias en estado PUBLICADA por defecto.

**SEO:**
- Title: "Explorar Campanas - WePlay Rises"
- Description: "Descubre proyectos musicales increibles y apoya a tus artistas favoritos"

---

### 6.2 CampaniaDetailPage (ACTUALIZAR)

**Archivo:** `src/web/src/features/campanias/presentation/pages/CampaniaDetailPage.tsx`

**Ruta:** `/campanias/:id`

**Layout:**
```
Header (sticky)
  |
  v
CampaniaHero (full width)
  |
  v
Grid 2 columnas (content | sidebar)
  |-- Column 1 (content):
  |     - Titulo + Artista Info
  |     - CampaniaStats
  |     - CampaniaProgress
  |     - CampaniaTabs (Historia/etc)
  |     - ArtistCard (about artist)
  |
  |-- Column 2 (sidebar sticky):
        - Button "Apoyar esta campania"
        - Starting price
        - RewardsList
        - Secure payment + delivery info
  |
  v
Footer
```

**Estado Local:**
- Ninguno (todo viene de query)

**Dependencias:**
- Hooks: useCampania (id desde params)
- Componentes: CampaniaHero, CampaniaStats, CampaniaProgress, CampaniaTabs, RewardsList, ArtistCard, CampaniaDetailSkeleton
- Router: useParams (para obtener id)

**Responsabilidad:**
Pagina de detalle completo de una campania publica. Layout segun mockup krowd. Maneja estados: loading, error 404, success.

**Cambios respecto a version actual:**
- Layout completo de 2 columnas (content + sidebar sticky)
- Agregar CampaniaHero con imagen/video
- Agregar stats detallados
- Agregar sidebar con rewards
- Agregar tabs navigation
- Agregar artist card
- Responsive: mobile (columna unica), tablet/desktop (2 cols)

**Error Handling:**
- Si campania no existe (404): Mostrar mensaje "Campania no encontrada" + link a /explorar
- Si campania es borrador y usuario no es owner: Mostrar 404
- Si error de red: Mostrar mensaje + boton retry

**SEO:**
- Title: "{titulo} - {artistaNombre} - WePlay Rises"
- Description: "{descripcionCorta}"

---

## 7. Flujo de Datos

```
User navega a /explorar
    ↓
ExplorarPage (presentation)
    ↓
useCampanias(filters, pagination) (application)
    ↓
campaniaService.getAll(filters, pagination) (infrastructure)
    ↓
apiFetch GET /api/campanias?estadoCampaniaId=2&pageNumber=1 (infrastructure)
    ↓
Backend retorna ServiceResponse<CampaniaDto[]>
    ↓
Mappers: mapCampaniaDtoToDomain[]
    ↓
TanStack Query cache: ['campanias', 'filtered', { ... }]
    ↓
ExplorarPage recibe data
    ↓
CampaniaList renderiza CampaniaCard[]
```

**Flujo detalle:**
```
User navega a /campanias/{id}
    ↓
CampaniaDetailPage (useParams para id)
    ↓
useCampania(id)
    ↓
campaniaService.getById(id)
    ↓
apiFetch GET /api/campanias/{id}
    ↓
Backend retorna ServiceResponse<Campania>
    ↓
Mapper: mapCampaniaDtoToDomain
    ↓
TanStack Query cache: ['campanias', id]
    ↓
CampaniaDetailPage recibe data
    ↓
Render: Hero, Stats, Progress, Tabs, Rewards, Artist
```

---

## 8. Dependencias de Shared

**Importar de `@shared/` (cuando se implemente):**

- Types:
  - `Campania`
  - `CampaniaListItem`
  - `EstadoCampania` (enum)
  - `TipoFinanciacion` (enum)

- Constants:
  - `CAMPANIA_ESTADOS`
  - `CAMPANIA_ESTADOS_LABELS`
  - `TIPO_FINANCIACION_LABELS`
  - `MONEDA_SYMBOLS`
  - `MONEDA_CODES`
  - `QUERY_KEYS.campanias.*`
  - `API_ROUTES.campanias.*`

- Utils:
  - `formatCurrencyWithSymbol(amount, monedaId)`
  - `getCurrencySymbol(monedaId)`
  - `getErrorMessage(errorCode)`

**IMPORTANTE:** Hasta que shared/ se implemente, mantener tipos locales y luego refactorizar.

---

## 9. Utilidades Locales

**Crear archivo:** `src/web/src/features/campanias/application/utils.ts`

```typescript
import { differenceInDays } from 'date-fns';

/**
 * Calcula dias restantes desde hoy hasta fecha fin
 */
export function calcularDiasRestantes(fechaFin: string | Date): number {
  const fin = typeof fechaFin === 'string' ? new Date(fechaFin) : fechaFin;
  const hoy = new Date();
  const dias = differenceInDays(fin, hoy);
  return Math.max(0, dias);
}

/**
 * Calcula porcentaje financiado
 */
export function calcularPorcentaje(pledged: number, objetivo: number): number {
  if (objetivo === 0) return 0;
  return Math.min(Math.round((pledged / objetivo) * 100), 100);
}

/**
 * Extrae video ID de URL de YouTube o Vimeo
 */
export function extractVideoId(url: string): { type: 'youtube' | 'vimeo' | null; id: string } {
  // YouTube
  const youtubeRegex = /(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&]+)/;
  const youtubeMatch = url.match(youtubeRegex);
  if (youtubeMatch) {
    return { type: 'youtube', id: youtubeMatch[1] };
  }

  // Vimeo
  const vimeoRegex = /vimeo\.com\/(\d+)/;
  const vimeoMatch = url.match(vimeoRegex);
  if (vimeoMatch) {
    return { type: 'vimeo', id: vimeoMatch[1] };
  }

  return { type: null, id: '' };
}

/**
 * Sanitiza HTML para evitar XSS
 * NOTA: En produccion usar DOMPurify o similar
 */
export function sanitizeHtml(html: string): string {
  // Implementacion basica, mejorar con DOMPurify
  const div = document.createElement('div');
  div.textContent = html;
  return div.innerHTML;
}
```

**Dependencia adicional:** `date-fns` (ya instalado en package.json)

---

## 10. Actualizaciones a Archivos Existentes

### 10.1 domain/types.ts

**Accion:** ACTUALIZAR

```typescript
// src/web/src/features/campanias/domain/types.ts

// Re-export types from shared (cuando este disponible)
// import type { Campania, CampaniaListItem, EstadoCampania, TipoFinanciacion } from '@shared/types/campania';
// export type { Campania, CampaniaListItem, EstadoCampania, TipoFinanciacion };

// Mientras tanto, mantener types locales alineados con contracts.md
export interface Campania {
  id: string;
  artistaId: string;
  proyectoArtisticoId?: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  videoPrincipalUrl?: string;
  imagenPrincipalUrl?: string;
  monedaId: number;
  importeObjetivo: number;
  importeMinimo?: number;
  importePledgedActual: number;
  tipoFinanciacionId: number;
  estadoCampaniaId: number;
  permiteAportacionesAnonimas: boolean;
  permitePropinas: boolean;
  porcentajeComisionPlataforma?: number;
  fechaInicio?: string;
  fechaFin?: string;
  fechaPublicacion?: string;
  fechaCierre?: string;
  fechaCreacion: string;
  fechaActualizacion?: string;
}

export interface CampaniaListItem {
  id: string;
  artistaId: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  imagenPrincipalUrl?: string;
  importeObjetivo: number;
  importePledgedActual: number;
  estadoCampaniaId: number;
  fechaInicio?: string;
  fechaFin?: string;
  fechaCreacion: string;
}

export enum EstadoCampania {
  Borrador = 1,
  Publicada = 2,
  Finalizada = 3,
  Cancelada = 4,
}

export enum TipoFinanciacion {
  TodoONada = 1,
  FlexibleGoal = 2,
}

// Tipos auxiliares
export interface CampaniaFilters {
  searchTerm?: string;
  artistaId?: string;
  estadoCampaniaId?: number;
}

export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
}

export interface RewardDto {
  id: string;
  campaniaId: string;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  cantidadDisponible?: number; // null = ilimitado
  cantidadReclamada: number;
  fechaEntregaEstimada?: string;
}
```

---

### 10.2 infrastructure/dtos.ts

**Accion:** ACTUALIZAR

Alinear con contracts.md, agregar todos los campos nuevos.

---

### 10.3 infrastructure/mappers.ts

**Accion:** ACTUALIZAR

Agregar mappers para campos nuevos (imagenPrincipalUrl, videoPrincipalUrl, descripcionCorta, monedaId, etc).

---

### 10.4 app/router.tsx

**Accion:** ACTUALIZAR

```typescript
// Agregar rutas
import { ExplorarPage, CampaniaDetailPage } from '@/features/campanias';

// En routes array
{
  path: '/explorar',
  element: <ExplorarPage />,
},
{
  path: '/campanias/:id',
  element: <CampaniaDetailPage />,
},
```

---

### 10.5 lib/constants.ts

**Accion:** ACTUALIZAR

```typescript
// Agregar constants alineados con shared
export const CAMPANIA_ESTADOS = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
} as const;

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
  1: 'Borrador',
  2: 'Publicada',
  3: 'Finalizada',
  4: 'Cancelada',
};

export const TIPO_FINANCIACION_LABELS: Record<number, string> = {
  1: 'Todo o Nada',
  2: 'Meta Flexible',
};

export const MONEDA_SYMBOLS: Record<number, string> = {
  1: '€',
  2: '$',
};

export const QUERY_KEYS = {
  // ... existentes
  CAMPANIAS: 'campanias',
  CAMPANIA: 'campania',
} as const;
```

---

## 11. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| presentation/pages/ExplorarPage.tsx | Page | Pagina de listado de campanias |
| presentation/components/CampaniaHero.tsx | Component | Hero con imagen/video |
| presentation/components/CampaniaStats.tsx | Component | Stats row (backers, dias, %) |
| presentation/components/CampaniaProgress.tsx | Component | Progress bar detallado |
| presentation/components/RewardCard.tsx | Component | Card de recompensa |
| presentation/components/RewardsList.tsx | Component | Lista de rewards sidebar |
| presentation/components/ArtistCard.tsx | Component | Mini card artista |
| presentation/components/CampaniaTabs.tsx | Component | Tabs navigation |
| presentation/components/CampaniaDescription.tsx | Component | Rich text renderer |
| presentation/components/CampaniaListSkeleton.tsx | Component | Skeleton loader lista |
| presentation/components/CampaniaDetailSkeleton.tsx | Component | Skeleton loader detalle |
| application/utils.ts | Utils | Helpers locales |

---

## 12. Archivos a Actualizar

| Archivo | Cambios |
|---------|---------|
| domain/types.ts | Agregar nuevos types alineados con contracts.md |
| infrastructure/dtos.ts | Actualizar DTOs con campos nuevos |
| infrastructure/mappers.ts | Actualizar mappers para campos nuevos |
| infrastructure/campania.api.ts | Renombrar de service, agregar metodos de filtrado |
| application/hooks/useCampanias.ts | Agregar filtros y paginacion |
| application/hooks/useCampania.ts | Ajustar tipos y error handling |
| presentation/components/CampaniaCard.tsx | Usar nuevos campos, agregar badges |
| presentation/components/CampaniaList.tsx | Agregar paginacion y variantes |
| presentation/pages/CampaniaDetailPage.tsx | Layout completo krowd con sidebar |
| app/router.tsx | Agregar rutas /explorar y /campanias/:id |
| lib/constants.ts | Agregar constantes de estados, tipos, monedas |

---

## 13. Responsive Behavior

### Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Layout columna unica, cards full width, sidebar abajo de content, hero h-64 |
| Tablet | 640-1024px | Grid 2 columnas en lista, detalle mantiene 2 cols (content + sidebar), hero h-80 |
| Desktop | > 1024px | Grid 3 columnas en lista, detalle 2 cols con sidebar sticky, hero h-96 |

### ExplorarPage Responsive

```tsx
// Mobile: 1 col
// Tablet: 2 cols
// Desktop: 3 cols
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  {campanias.map(c => <CampaniaCard key={c.id} campania={c} />)}
</div>
```

### CampaniaDetailPage Responsive

```tsx
// Mobile: stack vertical (hero, content, sidebar)
// Tablet/Desktop: grid 2 cols (content | sidebar sticky)
<div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
  {/* Content - 2 cols */}
  <div className="lg:col-span-2">
    {/* Titulo, stats, tabs, etc */}
  </div>

  {/* Sidebar - 1 col, sticky en desktop */}
  <div className="lg:col-span-1">
    <div className="sticky top-24">
      {/* Rewards, artist card */}
    </div>
  </div>
</div>
```

---

## 14. Loading States

### ExplorarPage
- Skeleton: CampaniaListSkeleton (6 cards por defecto)
- Duracion: Mientras useCampanias isLoading === true

### CampaniaDetailPage
- Skeleton: CampaniaDetailSkeleton (hero, stats, descripcion, sidebar)
- Duracion: Mientras useCampania isLoading === true

### Progresive Enhancement
- Hero image: placeholder con blur hasta que carga
- Rewards: skeleton individual si data no esta lista

---

## 15. Error States

### Error de Red
```tsx
<Alert variant="destructive">
  <AlertCircle className="h-4 w-4" />
  <AlertTitle>Error de conexion</AlertTitle>
  <AlertDescription>
    No se pudo cargar la informacion.
    <Button variant="link" onClick={refetch}>Intentar nuevamente</Button>
  </AlertDescription>
</Alert>
```

### 404 Campania No Encontrada
```tsx
<div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
  <FolderOpen className="h-16 w-16 text-muted-foreground" />
  <h2 className="text-2xl font-bold">Campania no encontrada</h2>
  <p className="text-muted-foreground">Esta campania no existe o ha sido eliminada</p>
  <Link to="/explorar">
    <Button>Explorar campanias</Button>
  </Link>
</div>
```

### Empty State (sin campanias)
```tsx
<div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
  <Music className="h-16 w-16 text-muted-foreground" />
  <h2 className="text-2xl font-bold">No hay campanias disponibles</h2>
  <p className="text-muted-foreground">Aun no hay proyectos musicales publicados</p>
</div>
```

---

## 16. Testing Considerations

### Componentes a Testear (prioridad)
1. CampaniaCard - props, estados, clicks
2. CampaniaList - empty, loading, error states
3. useCampanias - query, filtros, paginacion
4. useCampania - query, 404 handling
5. campaniaService - mappers, DTOs

### Tests Basicos (Vitest + Testing Library)

```typescript
// CampaniaCard.test.tsx
describe('CampaniaCard', () => {
  it('renders campania data correctly', () => {
    const mockCampania: CampaniaListItem = {
      id: '1',
      titulo: 'Mi Album',
      descripcionCorta: 'Rock alternativo',
      importeObjetivo: 5000,
      importePledgedActual: 4150,
      // ...
    };

    render(<CampaniaCard campania={mockCampania} />);

    expect(screen.getByText('Mi Album')).toBeInTheDocument();
    expect(screen.getByText(/83% financiado/)).toBeInTheDocument();
  });
});
```

**NOTA:** Tests opcionales para MVP, priorizar funcionalidad.

---

## 17. Performance Optimizations

### Imagenes
- Lazy loading: `<img loading="lazy" />`
- Placeholder blur mientras carga
- Responsive images: usar srcset si backend lo soporta

### Queries
- Stale time: 5 minutos para campanias (datos que cambian poco)
- Cache time: 10 minutos
- Prefetch en hover de CampaniaCard (opcional)

```typescript
export function useCampanias(filters?: CampaniaFilters) {
  return useQuery({
    queryKey: ['campanias', 'filtered', filters],
    queryFn: () => campaniaService.getAll(filters),
    staleTime: 5 * 60 * 1000, // 5 min
    cacheTime: 10 * 60 * 1000, // 10 min
  });
}
```

### Debounce en SearchTerm
```typescript
const [searchTerm, setSearchTerm] = useState('');
const debouncedSearch = useDebounce(searchTerm, 300);

const { data } = useCampanias({ searchTerm: debouncedSearch });
```

---

## 18. Accessibility

### Keyboard Navigation
- Todos los botones y links navegables con Tab
- Enter para activar botones
- Focus visible en todos los elementos interactivos

### ARIA Labels
```tsx
// Progress bar
<Progress value={83} aria-label="Progreso de financiacion: 83%" />

// Reward card
<Card role="article" aria-label="Recompensa: Descarga Digital por 10 euros">
  {/* ... */}
</Card>

// Image
<img src={url} alt={`Imagen de portada de ${titulo}`} />
```

### Contraste
- Verificar contraste minimo 4.5:1 en todos los textos
- Badge colors usar variants de shadcn/ui (mantienen contraste)

---

## 19. Checklist

### Componentes
- [ ] CampaniaCard actualizado con nuevos campos
- [ ] CampaniaList actualizado con paginacion
- [ ] CampaniaHero creado
- [ ] CampaniaStats creado
- [ ] CampaniaProgress creado
- [ ] RewardCard creado
- [ ] RewardsList creado
- [ ] ArtistCard creado
- [ ] CampaniaTabs creado
- [ ] CampaniaDescription creado
- [ ] CampaniaListSkeleton creado
- [ ] CampaniaDetailSkeleton creado

### Hooks
- [ ] useCampanias actualizado con filtros y paginacion
- [ ] useCampania actualizado con error handling

### Services
- [ ] campaniaService.getAll con filtros
- [ ] campaniaService.getById con error handling
- [ ] DTOs actualizados segun contracts.md
- [ ] Mappers actualizados para campos nuevos

### Pages
- [ ] ExplorarPage creado (/explorar)
- [ ] CampaniaDetailPage actualizado con layout completo

### Utilities
- [ ] calcularDiasRestantes
- [ ] calcularPorcentaje
- [ ] extractVideoId
- [ ] sanitizeHtml

### Routing
- [ ] Ruta /explorar configurada
- [ ] Ruta /campanias/:id configurada

### Constants
- [ ] CAMPANIA_ESTADOS agregado
- [ ] CAMPANIA_ESTADOS_LABELS agregado
- [ ] TIPO_FINANCIACION_LABELS agregado
- [ ] MONEDA_SYMBOLS agregado

### UI/UX
- [ ] Responsive en mobile, tablet, desktop
- [ ] Loading states (skeletons)
- [ ] Error states (404, network error, empty)
- [ ] Hover states en cards y botones
- [ ] Sticky sidebar en detalle
- [ ] Tabs navigation funcional
- [ ] Progress bar animado con gradient

### Accessibility
- [ ] ARIA labels en progress bars
- [ ] Alt text en imagenes
- [ ] Keyboard navigation funcional
- [ ] Focus states visibles
- [ ] Contraste verificado

### Integration
- [ ] Importar types de shared cuando este disponible
- [ ] Importar constants de shared cuando este disponible
- [ ] Importar utils de shared cuando este disponible

---

## 20. Siguiente Paso Sugerido

1. **Implementar shared/types y shared/constants** (bloqueante)
   - Ejecutar plan de contracts shared primero
   - Esto desbloquea tanto web como admin

2. **Actualizar tipos y constants locales** (temporal)
   - Mientras shared/ se implementa, usar tipos locales
   - Luego refactorizar para importar de shared

3. **Crear componentes base** (en paralelo)
   - CampaniaCard (actualizar)
   - CampaniaHero (nuevo)
   - CampaniaProgress (nuevo)
   - RewardCard (nuevo)

4. **Actualizar hooks y services** (despues de componentes)
   - useCampanias con filtros
   - campaniaService.getAll con query params

5. **Crear pages** (integracion final)
   - ExplorarPage
   - CampaniaDetailPage (actualizar)

6. **Testing y refinamiento** (opcional para MVP)
   - Tests basicos de componentes
   - Ajustes de responsive
   - Performance optimizations

---

**Fin del Plan Frontend Landing**

Este plan esta listo para ser implementado una vez que el plan de shared/contracts este completo. Los componentes pueden desarrollarse en paralelo una vez definidos los tipos base.
