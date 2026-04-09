# Plan Frontend: cp-inscripcion-programa (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

- **Screens:** 2 (ExplorarProgramasPage, MisProgramasPage)
- **Componentes:** 12
- **Hooks:** 4 (2 query, 2 mutation)
- **Services:** 1 (inscripcionService)
- **Rutas nuevas:** 2 (`/crowdpromotion/explorar`, `/promotor/mis-programas`)
- **Feature folder:** `src/web/src/features/crowdpromotion/`

La feature se integra en la carpeta `crowdpromotion` (que ya existe como raiz vacia). Sigue el mismo patron hexagonal usado en `promotor`, `crowdsourcing` y `campanias`: `domain/`, `application/`, `infrastructure/`, `presentation/`.

---

## 2. Estructura de Carpetas

```
src/web/src/features/crowdpromotion/
├── domain/
│   └── index.ts                    <- Re-export de tipos desde @shared
│
├── application/
│   └── hooks/
│       ├── useExplorarProgramas.ts     <- Query hook: GET /explorar con filtros y paginacion
│       ├── useMisProgramas.ts          <- Query hook: GET /promotor/mis-programas con paginacion
│       ├── useSolicitarInscripcion.ts  <- Mutation hook: POST /programas/{id}/inscripcion
│       └── index.ts                   <- Re-export de todos los hooks
│
├── infrastructure/
│   └── inscripcion.service.ts         <- Servicio con llamadas API HTTP
│
└── presentation/
    ├── components/
    │   ├── ProgramaCard.tsx            <- Card de programa en el catalogo (Explorar)
    │   ├── ProgramaCardSkeleton.tsx    <- Skeleton de carga para ProgramaCard
    │   ├── ProgramaFiltros.tsx         <- Barra de filtros (busqueda artista + tipo)
    │   ├── InscripcionItem.tsx         <- Item expandible de inscripcion (Mis Programas)
    │   ├── InscripcionItemSkeleton.tsx <- Skeleton para InscripcionItem
    │   ├── CodigoReferidoBlock.tsx     <- Bloque de codigo referido con boton copiar
    │   ├── UrlTrackingBlock.tsx        <- Bloque URL de tracking con tooltip y boton copiar
    │   ├── TareaItem.tsx               <- Item de tarea del programa (seccion expandida)
    │   ├── InscripcionEstadoBadge.tsx  <- Badge reutilizable de estado de inscripcion
    │   ├── EmptyStateProgramas.tsx     <- Empty state para Explorar sin programas
    │   ├── EmptyStateMisProgramas.tsx  <- Empty state para Mis Programas sin inscripciones
    │   └── index.ts                   <- Re-export de todos los componentes
    │
    └── pages/
        ├── ExplorarProgramasPage.tsx   <- /crowdpromotion/explorar
        └── MisProgramasPage.tsx        <- /promotor/mis-programas
```

---

## 3. Componentes

### 3.1 ProgramaCard

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/ProgramaCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programa` | `ProgramaExplorarItem` | Si | Datos del programa del catalogo |
| `isSolicitando` | `boolean` | Si | Si la mutacion de este programa esta en curso |
| `onSolicitar` | `(programaId: string) => void` | Si | Callback al hacer click en "Solicitar inscripcion" |
| `onVerMisDatos` | `(programaId: string) => void` | Si | Callback al hacer click en "Ver mis datos ->" (estado Aprobado) |
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno (el estado `isSolicitando` viene del padre via prop para que la pagina controle cual card esta en carga).

**Dependencias:**
- Hooks: ninguno (componente presentacional puro)
- Componentes UI: `Card`, `Badge`, `Button`, `Separator` de shadcn/ui
- Iconos: `UserPlus`, `CheckCircle`, `Ban`, `Music`, `Calendar`, `ClipboardList` de lucide-react
- Shared: `ProgramaExplorarItem`, `InscripcionEstado`, `INSCRIPCION_ESTADO`, `INSCRIPCION_ESTADO_LABELS`

**Responsabilidad:**
Renderiza un item del catalogo de programas. Muestra el tipo, numero de tareas, titulo, artista, comision, campana y vigencia. Segun el valor de `programa.miEstado`, muestra uno de cuatro estados de accion: boton "Solicitar inscripcion" (null), badge "Pendiente" con subtexto (Pendiente), badge "Aprobado" con enlace (Aprobado), o mensaje de bloqueo (Bloqueado). La card con estado Bloqueado aplica `opacity-60 cursor-not-allowed`. La funcion `formatComision` se implementa localmente para mostrar "10% por backing referido" o "5 EUR/conversion" segun si hay porcentaje o importe fijo.

**Variantes visuales por `miEstado`:**

| miEstado | Estilo card | Elemento de accion |
|----------|-------------|--------------------|
| `null` | `hover:border-[#a855f7]/50 transition-colors` | `<Button>` gradiente "Solicitar inscripcion" |
| `'Pendiente'` | `hover:border-[#a855f7]/50` | `<Badge>` amber "PENDIENTE DE APROBACION" + subtexto |
| `'Aprobado'` | `hover:border-green-800/50` | `<Badge>` verde "APROBADO" + `<Button variant="ghost">` "Ver mis datos ->" |
| `'Bloqueado'` | `opacity-60 cursor-not-allowed` | `<p>` rojo con icono `<Ban>` "No puedes inscribirte en este programa" |

---

### 3.2 ProgramaCardSkeleton

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/ProgramaCardSkeleton.tsx`

**Props:** Ninguno

**Dependencias:**
- Componentes UI: `Skeleton` de shadcn/ui

**Responsabilidad:**
Renderiza una card de placeholder con `animate-pulse` de altura `h-[240px]` para el estado de carga inicial del grid de Explorar Programas. Se renderizan 6 instancias durante la carga.

---

### 3.3 ProgramaFiltros

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/ProgramaFiltros.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `artistaNombre` | `string` | Si | Valor actual del input de busqueda por artista |
| `tipoPromoId` | `number \| undefined` | Si | Valor actual del select de tipo de programa |
| `tiposPromo` | `Array<{ id: number; nombre: string }>` | Si | Opciones del select tipo de programa |
| `hasActiveFilters` | `boolean` | Si | Si hay algun filtro activo (para mostrar boton Limpiar) |
| `onArtistaNombreChange` | `(value: string) => void` | Si | Callback al cambiar el input de busqueda |
| `onTipoPromoChange` | `(value: number \| undefined) => void` | Si | Callback al cambiar el select de tipo |
| `onClearFilters` | `() => void` | Si | Callback al hacer click en Limpiar filtros |

**Estado Local:** Ninguno (componente controlado completamente desde el padre).

**Dependencias:**
- Componentes UI: `Input`, `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`, `Button` de shadcn/ui
- Iconos: `Search`, `X` de lucide-react

**Responsabilidad:**
Renderiza la barra de filtros de la pagina ExplorarProgramasPage. Contiene: input de texto para buscar por nombre de artista (el debounce se gestiona en la pagina), select de tipo de programa, y boton "Limpiar filtros" visible solo cuando `hasActiveFilters` es true.

**Nota:** El filtro de rango de comision indicado en el mockup se implementa como filtro local (no API). El componente expone solo los filtros que disparan llamadas API (`tipoPromoId`). La busqueda por artista se debouncea en la pagina con `useDebounce` (ya existe en `src/web/src/hooks/useDebounce.ts`).

---

### 3.4 InscripcionEstadoBadge

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/InscripcionEstadoBadge.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estado` | `InscripcionEstado` | Si | Estado de la inscripcion |
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Badge` de shadcn/ui
- Iconos: `CheckCircle`, `Clock`, `Ban`, `UserX` de lucide-react
- Shared: `InscripcionEstado`, `INSCRIPCION_ESTADO`, `INSCRIPCION_ESTADO_LABELS`

**Responsabilidad:**
Componente presentacional que renderiza el badge de estado con el icono, color y texto correctos segun el estado. Usado tanto en `ProgramaCard` (variante compacta) como en `InscripcionItem` (variante mas grande). El mapeo de estado a clases CSS y icono se hace internamente usando `INSCRIPCION_ESTADO`.

| Estado | Clases badge | Icono |
|--------|--------------|-------|
| `'Pendiente'` | `bg-amber-950/50 text-amber-400 border border-amber-800/50` | `<Clock w-3 h-3>` |
| `'Aprobado'` | `bg-green-950/50 text-green-400 border border-green-800/50` | `<CheckCircle w-3 h-3>` |
| `'Bloqueado'` | `bg-red-950/50 text-red-400 border border-red-800/50` | `<Ban w-3 h-3>` |
| `'DadoDeBaja'` | `bg-[#1e1e38] text-[#64748b] border border-[#334155]` | `<UserX w-3 h-3>` |

---

### 3.5 CodigoReferidoBlock

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/CodigoReferidoBlock.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `codigo` | `string` | Si | Valor del codigo referido a mostrar y copiar |

**Estado Local:**
- `copied: boolean` - Controla si se muestra el icono `<Check>` en lugar de `<Copy>` (se resetea tras 1500ms)

**Dependencias:**
- Componentes UI: `Button` de shadcn/ui
- Iconos: `Copy`, `Check` de lucide-react
- Toast: `toast` de sonner

**Responsabilidad:**
Renderiza el bloque de codigo referido con fondo `bg-[#0f0f1f] border border-[#a855f7]/30` y fuente monospace. Implementa la logica de copiado al portapapeles con `navigator.clipboard.writeText`. Al copiar exitosamente, cambia el icono a `<Check>` verde durante 1500ms y muestra toast "Codigo referido copiado al portapapeles". Si el copiado falla (ej: contexto no seguro), muestra toast destructivo.

---

### 3.6 UrlTrackingBlock

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/UrlTrackingBlock.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `url` | `string` | Si | URL de tracking completa a mostrar (truncada) y copiar |

**Estado Local:**
- `copied: boolean` - Mismo patron que `CodigoReferidoBlock`

**Dependencias:**
- Componentes UI: `Button`, `Tooltip`, `TooltipContent`, `TooltipProvider`, `TooltipTrigger` de shadcn/ui
- Iconos: `Copy`, `Check` de lucide-react
- Toast: `toast` de sonner

**Responsabilidad:**
Similar a `CodigoReferidoBlock` pero para la URL de tracking. Muestra la URL truncada con `<span className="truncate">` y expone la URL completa en un `<Tooltip>` al hacer hover. El boton de copiar sigue el mismo patron de feedback visual. El contenedor usa `border-[#334155]` en lugar del borde purpura del codigo referido.

---

### 3.7 TareaItem

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/TareaItem.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tarea` | `TareaResumen` | Si | Datos de la tarea del programa |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Badge` de shadcn/ui
- Shared: `TareaResumen`

**Responsabilidad:**
Renderiza un item de tarea dentro de la seccion expandida de una inscripcion aprobada. Muestra el tipo de evento como badge, el titulo de la tarea, y la informacion de recompensa y repetibilidad en formato "Dinero: X.XX EUR | Repetible xN" o "No repetible". Los valores de recompensa se formatean con 2 decimales.

---

### 3.8 InscripcionItem

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/InscripcionItem.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `inscripcion` | `MiInscripcion` | Si | Datos completos de la inscripcion del promotor |
| `defaultExpanded` | `boolean` | No | Si la seccion de detalle empieza expandida (default: false) |

**Estado Local:**
- `isExpanded: boolean` - Controla si la seccion de tareas/estadisticas esta expandida (solo disponible para estado `'Aprobado'`)

**Dependencias:**
- Hooks: ninguno (componente presentacional)
- Componentes UI: `Card`, `Separator`, `Button` de shadcn/ui
- Iconos: `Music`, `Clock`, `Ban`, `UserX`, `ChevronDown`, `ChevronUp`, `Calendar`, `ClipboardList` de lucide-react
- Sub-componentes: `InscripcionEstadoBadge`, `CodigoReferidoBlock`, `UrlTrackingBlock`, `TareaItem`
- Shared: `MiInscripcion`, `INSCRIPCION_ESTADO`

**Responsabilidad:**
Renderiza un item completo de la lista de inscripciones del promotor. La estructura es una card con cabecera (titulo del programa, artista, badge de estado) y contenido condicional segun el estado:

- **Aprobado:** Muestra `CodigoReferidoBlock`, `UrlTrackingBlock` (si `urlTrackingPersonalizada` no es null), comision, fecha de inscripcion, y un boton toggle "Ver tareas y estadisticas" / "Ocultar detalles" que controla la expansion de la seccion de tareas.
- **Pendiente:** Mensaje con icono `<Clock>` amber "Esperando aprobacion del artista" y subtexto.
- **Bloqueado:** Mensaje con icono `<Ban>` rojo "Tu acceso a este programa ha sido bloqueado".
- **DadoDeBaja:** Similar al bloqueado pero con texto especifico de baja.

La seccion expandida (solo Aprobado) incluye mini KPI de estadisticas (valores en 0 para MVP) y la lista de `TareaItem`. La animacion de expansion usa transicion CSS sobre `max-height`.

---

### 3.9 InscripcionItemSkeleton

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/InscripcionItemSkeleton.tsx`

**Props:** Ninguno

**Dependencias:**
- Componentes UI: `Skeleton` de shadcn/ui

**Responsabilidad:**
Renderiza un placeholder de card de altura `h-[160px]` con `animate-pulse` para el estado de carga de la lista de Mis Programas. Se renderizan 3 instancias durante la carga.

---

### 3.10 EmptyStateProgramas

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/EmptyStateProgramas.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `hasActiveFilters` | `boolean` | Si | Si el empty state es por filtros activos o por falta de programas |
| `onClearFilters` | `() => void` | Si | Callback para limpiar filtros (usado cuando `hasActiveFilters` es true) |

**Dependencias:**
- Componentes UI: `Button` de shadcn/ui
- Iconos: `Megaphone`, `SearchX` de lucide-react

**Responsabilidad:**
Renderiza el empty state de la pagina ExplorarProgramasPage. Si `hasActiveFilters` es false: icono `<Megaphone>` con gradiente, titulo "No hay programas disponibles en este momento" y subtitulo. Si `hasActiveFilters` es true: icono `<SearchX>`, texto "No se encontraron programas con los filtros seleccionados" y boton "Limpiar filtros".

---

### 3.11 EmptyStateMisProgramas

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/EmptyStateMisProgramas.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `hasActiveEstadoFilter` | `boolean` | Si | Si el empty state es por filtro de estado activo o por falta de inscripciones |
| `onClearFilter` | `() => void` | Si | Callback para limpiar el filtro de estado activo |
| `onExplorar` | `() => void` | Si | Callback para navegar a explorar programas |

**Dependencias:**
- Componentes UI: `Button` de shadcn/ui
- Iconos: `Megaphone` de lucide-react

**Responsabilidad:**
Renderiza el empty state de la pagina MisProgramasPage. Si no hay ninguna inscripcion: icono `<Megaphone>` con gradiente, titulo "No estas inscrito en ningun programa", boton gradiente "Explorar programas". Si hay filtro de estado activo sin resultados: texto "No tienes inscripciones con este estado" y boton "Ver todas".

---

### 3.12 SinPerfilPromotorBanner

**Archivo:** `src/web/src/features/crowdpromotion/presentation/components/SinPerfilPromotorBanner.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `onCrearPerfil` | `() => void` | Si | Callback para navegar a /promotor/registro |

**Dependencias:**
- Componentes UI: `Button`, `Alert`, `AlertDescription` de shadcn/ui
- Iconos: `AlertCircle` de lucide-react

**Responsabilidad:**
Banner que se muestra en ExplorarProgramasPage cuando el promotor no tiene perfil (el query `usePromotor` retorna error 404 con code 2015). Fondo `bg-amber-950/30 border border-amber-800/50 rounded-lg p-4`. Texto "Necesitas un perfil de promotor para solicitar inscripciones." + boton "Crear perfil".

---

## 4. Hooks

### 4.1 useExplorarProgramas

**Archivo:** `src/web/src/features/crowdpromotion/application/hooks/useExplorarProgramas.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `filters` | `ExplorarProgramasFilters` | Filtros opcionales (artistaNombre, tipoPromoId, page, pageSize) |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `ProgramasExplorarResponse \| undefined` | Respuesta paginada con items y metadatos |
| `isLoading` | `boolean` | True durante la carga inicial (sin datos en cache) |
| `isFetching` | `boolean` | True durante cualquier carga (incluye revalidaciones) |
| `isError` | `boolean` | True si la llamada fallo |
| `error` | `Error \| null` | Objeto de error si hay fallo |
| `refetch` | `function` | Funcion para reintentar manualmente |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.explorar(filters)`

**Configuracion:**
- `staleTime: 1 * 60 * 1000` (1 minuto, los datos del catalogo cambian con poca frecuencia)
- `retry: 1`

**Notas:** Los filtros completos (incluyendo `page`, `pageSize`, `artistaNombre`, `tipoPromoId`) se pasan como parte de la query key para que cada combinacion de filtros tenga su propia entrada de cache.

---

### 4.2 useMisProgramas

**Archivo:** `src/web/src/features/crowdpromotion/application/hooks/useMisProgramas.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `page` | `number` | Pagina actual (default 1) |
| `pageSize` | `number` | Elementos por pagina (default 10) |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `MisProgramasResponse \| undefined` | Lista completa de inscripciones del promotor |
| `isLoading` | `boolean` | Estado de carga inicial |
| `isError` | `boolean` | Si la llamada fallo |
| `error` | `Error \| null` | Objeto de error |
| `refetch` | `function` | Funcion para reintentar |

**Query Key:** `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas(page)`

**Configuracion:**
- `staleTime: 30 * 1000` (30 segundos, datos del promotor cambian con mas frecuencia)
- `retry: 1`

**Notas:** La pagina recibe toda la lista sin filtros de estado (el filtro por estado es local, sobre `data.items`). El filtro por estado se aplica en la pagina mediante un `useMemo`.

---

### 4.3 useSolicitarInscripcion

**Archivo:** `src/web/src/features/crowdpromotion/application/hooks/useSolicitarInscripcion.ts`

**Tipo:** Mutation Hook

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(programaId: string) => void` | Dispara la mutacion de solicitud |
| `mutateAsync` | `(programaId: string) => Promise<InscripcionCreada>` | Version async para await |
| `isPending` | `boolean` | True mientras la peticion POST esta en vuelo |
| `variables` | `string \| undefined` | El `programaId` de la mutacion activa (para mapear loading a card especifica) |

**Acciones:**
- `mutationFn`: POST a `/api/crowdpromotion/programas/{programaId}/inscripcion`
- `onSuccess`: Invalida `QUERY_KEYS.crowdpromotion.programas.explorar` (para refrescar `miEstado` en el catalogo) e invalida `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas`. Muestra toast success "Solicitud enviada. El artista revisara tu perfil."
- `onError`: Lee `error.errorCode`. Para `'4021'` muestra toast info "Ya tienes una solicitud activa para este programa." Para `'4022'` muestra toast destructivo "No puedes inscribirte en este programa." Para cualquier otro codigo usa `getInscripcionErrorMessage(errorCode)`.

**Notas de invalidacion:** La invalidacion de `explorar` sin filtros especificos invalida todas las entradas de cache de explorar (el patron de `invalidateQueries` con clave parcial). Esto garantiza que `miEstado` se actualice en cualquier vista de filtros que el promotor tenga activa.

---

### 4.4 Hooks de aplicacion index

**Archivo:** `src/web/src/features/crowdpromotion/application/hooks/index.ts`

Re-exporta: `useExplorarProgramas`, `useMisProgramas`, `useSolicitarInscripcion`.

---

## 5. Services

### 5.1 inscripcionService

**Archivo:** `src/web/src/features/crowdpromotion/infrastructure/inscripcion.service.ts`

**Patron:** Sigue exactamente el mismo patron de `promotorService` en `src/web/src/features/promotor/infrastructure/promotor.service.ts`: clase con metodos async, `apiFetch` de `@/lib/api-client`, `API_ROUTES` de `@shared/constants`, y funcion privada `extractError` para procesar mensajes de error.

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `explorarProgramas` | `filters: ExplorarProgramasFilters` | `ProgramasExplorarResponse` | `API_ROUTES.crowdpromotion.programas.explorar` + query params | GET |
| `misProgramas` | `page: number, pageSize: number` | `MisProgramasResponse` | `API_ROUTES.crowdpromotion.promotorInscripciones.misProgramas` + query params | GET |
| `solicitarInscripcion` | `programaId: string` | `InscripcionCreada` | `API_ROUTES.crowdpromotion.programas.inscripcion(programaId)` | POST |

**Manejo de errores:**
Cada metodo verifica `response.messages` para detectar error codes que no empiecen con `'0'`, crea un `Error` con el mensaje y adjunta `errorCode` al objeto (mismo patron que `promotorService`). Los hooks consumen este `errorCode` para mostrar mensajes especificos.

**Construccion de query params para GET:**
Para `explorarProgramas`, construir `URLSearchParams` omitiendo parametros `undefined`:
```
const params = new URLSearchParams()
if (filters.artistaNombre) params.set('artistaNombre', filters.artistaNombre)
if (filters.tipoPromoId !== undefined) params.set('tipoPromoId', String(filters.tipoPromoId))
params.set('page', String(filters.page ?? 1))
params.set('pageSize', String(filters.pageSize ?? 10))
const url = `${API_ROUTES.crowdpromotion.programas.explorar}?${params.toString()}`
```

**Exportacion:** `export const inscripcionService = new InscripcionService()`

---

## 6. Paginas

### 6.1 ExplorarProgramasPage

**Archivo:** `src/web/src/features/crowdpromotion/presentation/pages/ExplorarProgramasPage.tsx`

**Ruta:** `/crowdpromotion/explorar`
**Layout:** `PublicLayout` (con header de landing)

**Estado Local:**
- `artistaNombreInput: string` - Valor del input de busqueda (debounceado antes de pasarlo al hook)
- `tipoPromoId: number | undefined` - Filtro por tipo de programa (dispara refetch)
- `page: number` - Pagina actual

**Hooks utilizados:**
- `usePromotor()` - Para verificar si el promotor tiene perfil activo
- `useDebounce(artistaNombreInput, 300)` - Debounce del input de busqueda
- `useExplorarProgramas({ artistaNombre: debouncedArtistaNombre, tipoPromoId, page, pageSize: 10 })`
- `useSolicitarInscripcion()` - La mutacion compartida para todos los botones de la pagina
- `useNavigate()` de react-router-dom

**Logica de la pagina:**

1. Verificar autenticacion: si el usuario no esta autenticado, redirigir a `/login?redirect=/crowdpromotion/explorar`. Usar el store de auth (`useAuthStore` o similar pattern existente).
2. Consultar `usePromotor()`. Si retorna error 404 (code `'2015'`), mostrar `SinPerfilPromotorBanner` (pero no bloquear la visualizacion del catalogo).
3. Cargar programas con `useExplorarProgramas`. Mientras carga: grid de 6 `ProgramaCardSkeleton`.
4. Si error de carga: card centrada con `<AlertCircle>` y boton "Reintentar".
5. Si sin resultados: `<EmptyStateProgramas>` con `hasActiveFilters` calculado.
6. Si hay datos: grid de `ProgramaCard` con paginacion.

**Calculo de `isSolicitandoForCard`:** Como `useSolicitarInscripcion` retorna `variables` (el `programaId` en vuelo), se puede pasar `isPending && variables === programa.id` como prop `isSolicitando` a cada `ProgramaCard`.

**Interaccion "Ver mis datos ->":** Llama a `navigate(APP_ROUTES.landing.crowdpromotion.misProgramas)`. El hash `#programa-{id}` es un nice-to-have que se puede omitir en MVP.

**Paginacion:** Al cambiar de pagina, hace scroll al top del grid (`document.querySelector('.programa-grid')?.scrollIntoView()`).

**Filtros:** El cambio de `tipoPromoId` resetea `page` a 1. El debounce del artista tambien resetea `page` a 1 via `useEffect`.

---

### 6.2 MisProgramasPage

**Archivo:** `src/web/src/features/crowdpromotion/presentation/pages/MisProgramasPage.tsx`

**Ruta:** `/promotor/mis-programas`
**Layout:** `DashboardLayout` (autenticado, sidebar del promotor)

**Estado Local:**
- `estadoFiltro: InscripcionEstado | 'Todos'` - Filtro de estado aplicado localmente sobre los items

**Hooks utilizados:**
- `usePromotor()` - Para verificar perfil activo y redirigir si no existe
- `useMisProgramas({ page: 1, pageSize: 50 })` - Carga con pageSize alto para filtrado local
- `useNavigate()` de react-router-dom

**Logica de la pagina:**

1. Si `usePromotor()` retorna error: redirigir a `/promotor/registro` (mismo patron que `PromotorPerfilPage`).
2. Mientras carga `useMisProgramas`: 3 `InscripcionItemSkeleton`.
3. Si error: card de error con boton "Reintentar".
4. Si sin inscripciones (items.length === 0): `<EmptyStateMisProgramas hasActiveEstadoFilter={false}>`.
5. Si hay inscripciones: mostrar filtros de estado (botones pill) y lista de `InscripcionItem`.

**Filtro local por estado:**
```typescript
const inscripcionesFiltradas = useMemo(() => {
    if (estadoFiltro === 'Todos') return data?.items ?? []
    return (data?.items ?? []).filter(i => i.estado === estadoFiltro)
}, [data?.items, estadoFiltro])
```

Si `inscripcionesFiltradas.length === 0` y `estadoFiltro !== 'Todos'`: mostrar `<EmptyStateMisProgramas hasActiveEstadoFilter={true}>`.

**Botones de filtro de estado:** "Todos", "Aprobado", "Pendiente", "Bloqueado". El boton activo cambia de estilo. Los botones usan `INSCRIPCION_ESTADO_LABELS` para los textos.

**Nota sobre paginacion:** Para MVP, con `pageSize: 50` se asume que los promotores no tienen mas de 50 inscripciones. Si se requiere paginacion real en el futuro, se agrega un parametro `page` controlado por la pagina.

---

## 7. Flujo de Datos

```
Usuario hace click "Solicitar inscripcion"
         |
         v
ExplorarProgramasPage.onSolicitar(programaId)
         |
         v
useSolicitarInscripcion.mutate(programaId)
         |
         v
inscripcionService.solicitarInscripcion(programaId)
         |
         v
apiFetch: POST /api/crowdpromotion/programas/{programaId}/inscripcion
         |
         v
Backend (handler -> service -> repository)
         |
    onSuccess                   onError
         |                          |
         v                          v
invalidar QUERY_KEYS        toast.error(getInscripcionErrorMessage)
explorar + misProgramas
         |
         v
useExplorarProgramas refetch automatico
         |
         v
ProgramaCard re-renderiza con nuevo miEstado = 'Pendiente'
```

```
Usuario abre /promotor/mis-programas
         |
         v
MisProgramasPage monta
         |
         v
useMisProgramas({ page: 1, pageSize: 50 })
         |
         v
inscripcionService.misProgramas(1, 50)
         |
         v
GET /api/crowdpromotion/promotor/mis-programas?page=1&pageSize=50
         |
         v
data.items -> filtrado local por estadoFiltro (useMemo)
         |
         v
Lista de InscripcionItem renderizada
         |
         v
InscripcionItem (estado Aprobado) muestra CodigoReferidoBlock + UrlTrackingBlock
         |
         v
Usuario click "Copiar" en CodigoReferidoBlock
         |
         v
navigator.clipboard.writeText(codigo)
         |
         v
toast.success("Codigo referido copiado al portapapeles")
         + icono <Check> durante 1500ms
```

---

## 8. Rutas a Agregar en AppRouter

**Archivo:** `src/web/src/app/router.tsx`

Se agregan 2 rutas nuevas con lazy loading:

**Imports lazy a agregar:**
```
const ExplorarProgramasPage = lazy(() => import("@/features/crowdpromotion/presentation/pages/ExplorarProgramasPage"))
const MisProgramasPage = lazy(() => import("@/features/crowdpromotion/presentation/pages/MisProgramasPage"))
```

**Ruta `ExplorarProgramasPage`:**
- Ubicacion: dentro del bloque `<Route element={<PublicLayout />}>` (junto a otras rutas publicas de crowdpromotion/crowdsourcing)
- Path: `/crowdpromotion/explorar`
- Nota: Aunque requiere autenticacion para solicitar, la vista del catalogo es "publica". La autenticacion se verifica en la pagina, no en el router.

**Ruta `MisProgramasPage`:**
- Ubicacion: dentro del bloque `<Route element={<DashboardLayout />}>` (junto a otras rutas protegidas del promotor)
- Path: `/promotor/mis-programas`

**APP_ROUTES requeridos (ya definidos en shared/contracts-plan.md):**
- `APP_ROUTES.landing.crowdpromotion.explorar` = `/crowdpromotion/explorar`
- `APP_ROUTES.landing.crowdpromotion.misProgramas` = `/promotor/mis-programas`

---

## 9. Dependencias de Shared

**Importar de `@shared/types/crowdpromotion`:**
- `InscripcionEstado` (union type)
- `ProgramaExplorarItem`
- `ProgramasExplorarResponse`
- `InscripcionCreada`
- `MiInscripcion`
- `MisProgramasResponse`
- `TareaResumen`
- `ExplorarProgramasFilters`

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdpromotion.programas.explorar`
- `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas`
- `API_ROUTES.crowdpromotion.programas.explorar`
- `API_ROUTES.crowdpromotion.programas.inscripcion`
- `API_ROUTES.crowdpromotion.promotorInscripciones.misProgramas`
- `APP_ROUTES.landing.crowdpromotion.explorar`
- `APP_ROUTES.landing.crowdpromotion.misProgramas`
- `APP_ROUTES.landing.promotor.registro`
- `INSCRIPCION_ESTADO`
- `INSCRIPCION_ESTADO_LABELS`

**Importar de `@shared/utils/error-messages`:**
- `getInscripcionErrorMessage`

**Importar de `@shared/utils/mappers`:**
- `getInscripcionBadgeVariant` (opcional, el componente puede mapear localmente)

**Hooks existentes reutilizados:**
- `useDebounce` de `@/hooks/useDebounce`
- `usePromotor` de `@/features/promotor/application`
- `apiFetch` de `@/lib/api-client`

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdpromotion/domain/index.ts` | Domain barrel | Re-exporta tipos de @shared necesarios para la feature |
| `src/web/src/features/crowdpromotion/application/hooks/useExplorarProgramas.ts` | Hook | Query hook para GET /explorar |
| `src/web/src/features/crowdpromotion/application/hooks/useMisProgramas.ts` | Hook | Query hook para GET /mis-programas |
| `src/web/src/features/crowdpromotion/application/hooks/useSolicitarInscripcion.ts` | Hook | Mutation hook para POST /inscripcion |
| `src/web/src/features/crowdpromotion/application/hooks/index.ts` | Barrel | Re-exporta hooks de la feature |
| `src/web/src/features/crowdpromotion/infrastructure/inscripcion.service.ts` | Service | Llamadas API HTTP para inscripciones (promotor) |
| `src/web/src/features/crowdpromotion/presentation/components/ProgramaCard.tsx` | Component | Card de programa en catalogo de explorar |
| `src/web/src/features/crowdpromotion/presentation/components/ProgramaCardSkeleton.tsx` | Component | Skeleton de carga para grid de explorar |
| `src/web/src/features/crowdpromotion/presentation/components/ProgramaFiltros.tsx` | Component | Barra de filtros de explorar programas |
| `src/web/src/features/crowdpromotion/presentation/components/InscripcionEstadoBadge.tsx` | Component | Badge reutilizable de estado de inscripcion |
| `src/web/src/features/crowdpromotion/presentation/components/CodigoReferidoBlock.tsx` | Component | Bloque de codigo referido con copiar |
| `src/web/src/features/crowdpromotion/presentation/components/UrlTrackingBlock.tsx` | Component | Bloque URL tracking con tooltip y copiar |
| `src/web/src/features/crowdpromotion/presentation/components/TareaItem.tsx` | Component | Item de tarea en seccion expandida |
| `src/web/src/features/crowdpromotion/presentation/components/InscripcionItem.tsx` | Component | Item expandible de inscripcion en Mis Programas |
| `src/web/src/features/crowdpromotion/presentation/components/InscripcionItemSkeleton.tsx` | Component | Skeleton de carga para lista Mis Programas |
| `src/web/src/features/crowdpromotion/presentation/components/EmptyStateProgramas.tsx` | Component | Empty state para Explorar sin resultados |
| `src/web/src/features/crowdpromotion/presentation/components/EmptyStateMisProgramas.tsx` | Component | Empty state para Mis Programas sin inscripciones |
| `src/web/src/features/crowdpromotion/presentation/components/SinPerfilPromotorBanner.tsx` | Component | Banner para promotor sin perfil en Explorar |
| `src/web/src/features/crowdpromotion/presentation/components/index.ts` | Barrel | Re-exporta componentes de la feature |
| `src/web/src/features/crowdpromotion/presentation/pages/ExplorarProgramasPage.tsx` | Page | Pagina /crowdpromotion/explorar |
| `src/web/src/features/crowdpromotion/presentation/pages/MisProgramasPage.tsx` | Page | Pagina /promotor/mis-programas |
| `src/web/src/features/crowdpromotion/index.ts` | Barrel | Re-exporta la feature completa |

**Archivo a modificar:**
| Archivo | Cambio |
|---------|--------|
| `src/web/src/app/router.tsx` | Agregar 2 imports lazy y 2 rutas (`/crowdpromotion/explorar` en PublicLayout, `/promotor/mis-programas` en DashboardLayout) |

---

## 11. Estados de UI por Pantalla

### ExplorarProgramasPage

| Estado | Condicion | Comportamiento |
|--------|-----------|----------------|
| Loading inicial | `isLoading === true` | Grid de 6 `ProgramaCardSkeleton` |
| Con datos | `isLoading === false && items.length > 0` | Grid de `ProgramaCard` + paginacion si `totalPages > 1` |
| Empty (sin filtros) | `!isLoading && items.length === 0 && !hasActiveFilters` | `<EmptyStateProgramas hasActiveFilters={false}>` |
| Empty (con filtros) | `!isLoading && items.length === 0 && hasActiveFilters` | `<EmptyStateProgramas hasActiveFilters={true}>` |
| Error | `isError === true` | Card con `<AlertCircle>` + boton "Reintentar" |
| Sin perfil promotor | `usePromotor().isError` | Banner `<SinPerfilPromotorBanner>` sobre el grid |
| Solicitando | `isPending === true` | Boton de la card especifica con `<Loader2>` + "Enviando..." |

### MisProgramasPage

| Estado | Condicion | Comportamiento |
|--------|-----------|----------------|
| Loading inicial | `isLoading === true` | 3 `InscripcionItemSkeleton` |
| Con datos | `!isLoading && items.length > 0` | Filtros de estado + lista de `InscripcionItem` |
| Empty (sin inscripciones) | `!isLoading && items.length === 0` | `<EmptyStateMisProgramas hasActiveEstadoFilter={false}>` |
| Empty (filtro sin resultados) | `!isLoading && inscripcionesFiltradas.length === 0 && estadoFiltro !== 'Todos'` | `<EmptyStateMisProgramas hasActiveEstadoFilter={true}>` |
| Error | `isError === true` | Card con `<AlertCircle>` + boton "Reintentar" |
| Expandido | `isExpanded === true` en `InscripcionItem` | Seccion tareas/stats visible |

---

## 12. Checklist

- [ ] Componentes usan shadcn/ui (`Card`, `Badge`, `Button`, `Separator`, `Input`, `Select`, `Tooltip`, `Skeleton`)
- [ ] Hooks siguen patron `useQuery` / `useMutation` de TanStack Query
- [ ] `inscripcionService` sigue el patron exacto de `promotorService` (clase, `apiFetch`, `extractError`)
- [ ] Types importados de `@shared/types/crowdpromotion` (no duplicados)
- [ ] Constantes importadas de `@shared/constants` (QUERY_KEYS, API_ROUTES, APP_ROUTES, INSCRIPCION_ESTADO)
- [ ] Error messages via `getInscripcionErrorMessage` de `@shared/utils/error-messages`
- [ ] No se usa `any` en TypeScript
- [ ] Toast via `sonner` (libreria ya instalada en el proyecto, usada en `useCreatePromotor`)
- [ ] Debounce del input de busqueda via `useDebounce` existente en `@/hooks/useDebounce`
- [ ] `useSolicitarInscripcion.variables` usado para identificar la card en carga (sin estado local por card)
- [ ] `CodigoReferidoBlock` y `UrlTrackingBlock` gestionan su propio `copied` local (unico estado local aceptable)
- [ ] `InscripcionItem` gestionan su propio `isExpanded` (estado de UI local de la card)
- [ ] Filtro de estado en `MisProgramasPage` es local (no dispara fetch)
- [ ] Filtro `tipoPromoId` en `ExplorarProgramasPage` dispara refetch (incluido en query key)
- [ ] Busqueda `artistaNombre` en `ExplorarProgramasPage` debounceada 300ms antes de entrar en query key
- [ ] `codigoReferido` y `urlTrackingPersonalizada` solo renderizados cuando `estado === 'Aprobado'`
- [ ] Las 2 rutas nuevas registradas en `router.tsx` con lazy loading
- [ ] `ExplorarProgramasPage` en `PublicLayout`, `MisProgramasPage` en `DashboardLayout`
- [ ] Archivos de barrel (`index.ts`) creados para `application/hooks/` y `presentation/components/`
