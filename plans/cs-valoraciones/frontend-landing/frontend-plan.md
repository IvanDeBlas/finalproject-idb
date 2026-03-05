# Plan Frontend: cs-valoraciones (Landing)

**Fecha:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)
**Target:** `src/web` (Vite + React 18 + TypeScript + Tailwind + shadcn/ui)

---

## 1. Resumen

- **Screens modificadas:** 1 (AcuerdoDetallePage - ya existe)
- **Screens nuevas:** 1 (PerfilProfesionalPage - nueva, o insercion en perfil existente)
- **Componentes nuevos:** 9
- **Componentes modificados:** 2 (AcuerdoDetallePage, PropuestaCard)
- **Hooks nuevos:** 2
- **API services nuevos:** 1 (valoracion.api.ts como clase)
- **Tipos de dominio:** re-export de 6 nuevas interfaces desde `@shared`

---

## 2. Estructura de Carpetas

La feature se ubica dentro de la feature `crowdsourcing` existente, siguiendo la misma convencion arquitectonica ya establecida en el proyecto.

```
src/web/src/features/crowdsourcing/
├── domain/
│   └── types.ts                            [MODIFICAR] - agregar re-exports de tipos de valoraciones
├── infrastructure/
│   ├── api/
│   │   └── valoracion.api.ts               [NUEVO] - clase ValoracionApiService
│   └── index.ts                            [MODIFICAR] - exportar valoracionApi
├── application/
│   └── hooks/
│       ├── useCreateValoracion.ts          [NUEVO] - useMutation POST
│       └── useValoracionesUsuario.ts       [NUEVO] - useQuery GET paginado
└── presentation/
    ├── components/
    │   ├── StarRating.tsx                  [NUEVO] - input interactivo 1-5 con hover
    │   ├── StarDisplay.tsx                 [NUEVO] - read-only con medias estrellas
    │   ├── RatingHistogram.tsx             [NUEVO] - barras de distribucion
    │   ├── ValoracionForm.tsx              [NUEVO] - card formulario completo
    │   ├── ValoracionReadOnly.tsx          [NUEVO] - card post-envio / ya valorado
    │   ├── ValoracionesSection.tsx         [NUEVO] - seccion completa en perfil
    │   ├── ValoracionListItem.tsx          [NUEVO] - item individual en lista
    │   ├── RatingBadge.tsx                 [NUEVO] - badge compacto/medium
    │   ├── EmptyValoraciones.tsx           [NUEVO] - estado vacio
    │   └── PropuestaCard.tsx               [MODIFICAR] - agregar RatingBadge
    └── pages/
        ├── AcuerdoDetallePage.tsx          [MODIFICAR] - agregar seccion de valoracion
        └── PerfilProfesionalPage.tsx       [NUEVO] - pagina de perfil de usuario con valoraciones

src/web/src/app/
└── router.tsx                              [MODIFICAR] - agregar ruta /crowdsourcing/profesionales/:userId
```

---

## 3. Componentes

### 3.1 StarRating

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/StarRating.tsx`

**Descripcion:** Selector interactivo de 1 a 5 estrellas con efecto hover. Implementado con iconos Lucide `<Star>` y estado local de hover. No existe en shadcn/ui; se implementa custom con Tailwind.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `value` | `number` | Si | Valor actual seleccionado (0 = sin seleccion, 1-5 = seleccionado) |
| `onChange` | `(value: number) => void` | Si | Callback al seleccionar una estrella |
| `size` | `'sm' \| 'md' \| 'lg'` | No (default: `'lg'`) | Tamano del icono. `sm`: w-3.5 h-3.5; `md`: w-8 h-8; `lg`: w-10 h-10 |
| `disabled` | `boolean` | No (default: `false`) | Si true, deshabilita la interaccion (estado submitting) |
| `className` | `string` | No | Clase CSS adicional para el contenedor |

**Estado Local:**
- `hoverValue: number` - valor de la estrella sobre la que esta el cursor (0 = sin hover)
- Derivado: `isActive(i) = hoverValue > 0 ? i <= hoverValue : i <= value`

**Accesibilidad:**
- Contenedor: `role="radiogroup"` con `aria-label="Puntuacion de 1 a 5 estrellas"`
- Cada boton de estrella: `aria-label="{n} estrella{s}"` y `aria-pressed={value === n}`
- Navegacion teclado: `Tab` entre estrellas, `Enter`/`Space` para seleccionar, flechas izquierda/derecha para decrementar/incrementar
- Cuando `disabled`: `aria-disabled="true"` y `tabIndex={-1}`

**Dependencias:**
- Lucide: `Star`
- Tailwind utilities para transiciones y escalado

**Clases CSS por estado de estrella:**
- Activa + hover: `text-[#fbbf24] scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)] transition-transform duration-100`
- Activa sin hover: `text-[#f59e0b] transition-transform duration-100`
- Inactiva: `text-[#334155] transition-transform duration-100`
- En click: `active:scale-125`

---

### 3.2 StarDisplay

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/StarDisplay.tsx`

**Descripcion:** Visualizacion read-only de puntuacion. Soporta medias estrellas mediante opacidad reducida para la parte fraccionaria (estrategia MVP: redondear a 0.5 mas cercano y mostrar la estrella del umbral con `opacity-50`).

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `value` | `number` | Si | Puntuacion a mostrar. Acepta decimales (ej: 4.5) |
| `size` | `'sm' \| 'md' \| 'lg'` | No (default: `'md'`) | `sm`: w-3.5 h-3.5; `md`: w-4 h-4; `lg`: w-5 h-5 |
| `showNumeric` | `boolean` | No (default: `false`) | Si true, muestra "{value} / 5" a la derecha |
| `className` | `string` | No | Clase adicional para el contenedor |

**Estado Local:** Ninguno. Componente puramente de presentacion.

**Accesibilidad:**
- Contenedor: `role="img"` con `aria-label="Puntuacion: {value} de 5 estrellas"`
- Cada `<Star>`: `aria-hidden="true"`

**Logica de medias estrellas:**
- `fullStars = Math.floor(value)` - estrellas completamente llenas
- `hasHalf = (value - fullStars) >= 0.25 && (value - fullStars) < 0.75` - si hay media estrella
- `emptyStars = 5 - fullStars - (hasHalf ? 1 : 0)` - estrellas vacias
- La media estrella se renderiza con `opacity-50` sobre color ambar `#f59e0b`

**Dependencias:**
- Lucide: `Star`

---

### 3.3 RatingHistogram

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RatingHistogram.tsx`

**Descripcion:** Barras de distribucion de valoraciones por nivel de estrella (5 a 1). Incluye animacion de expansion al montar.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `distribucion` | `{ 5: number; 4: number; 3: number; 2: number; 1: number }` | Si | Conteo por nivel de estrella |
| `total` | `number` | Si | Total de valoraciones (usado para calcular porcentajes) |

**Estado Local:** Ninguno. El porcentaje se calcula: `total > 0 ? (distribucion[n] / total) * 100 : 0`

**Estructura de cada fila (niveles 5 a 1):**
- Etiqueta del nivel con icono estrella decorativo pequeno
- Track de la barra: `h-2 bg-[#334155] rounded-full overflow-hidden`
- Relleno con gradiente ambar: `h-full bg-gradient-to-r from-[#f59e0b] to-[#fbbf24] rounded-full transition-all duration-500`
  - El width se aplica con `style={{ width: '{porcentaje}%' }}`, iniciando en 0 y expandiendo al montar (via `useEffect` que aplica el width real tras el primer render)
- Contador numerico a la derecha

**Dependencias:**
- Lucide: `Star`

---

### 3.4 ValoracionForm

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ValoracionForm.tsx`

**Descripcion:** Card completo con el formulario de valoracion. Contiene `StarRating` para seleccion de puntuacion y `Textarea` para comentario opcional. Usa React Hook Form + Zod. Gestiona el estado de envio con `useCreateValoracion`.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdoId` | `string` | Si | ID del acuerdo sobre el que se valora |
| `userIdValorado` | `string` | Si | ID del usuario que recibe la valoracion (necesario para invalidar cache) |
| `onSuccess` | `(valoracion: ValoracionCreatedResult) => void` | Si | Callback tras envio exitoso; la pagina padre usa esto para mostrar `ValoracionReadOnly` |

**Estado Local:**
- `form`: instancia de `useForm<CreateValoracionFormData>` con `zodResolver(createValoracionSchema)`
- El campo `puntuacion` se registra manualmente via `form.setValue('puntuacion', n)` cuando el usuario hace click en `StarRating`
- `comentarioLength: number` - longitud actual del comentario (derivado de `form.watch('comentario')`)

**Logica de estados del contador:**
- 0-900 chars: `text-[#64748b]`
- 901-1000 chars: `text-[#f59e0b]`
- > 1000 chars: `text-[#ef4444]` y boton submit disabled

**Dependencias:**
- shadcn/ui: `Button`, `Textarea`, `Label`, `Skeleton`
- Lucide: `Star`, `Loader2`
- Hooks: `useCreateValoracion`
- Componentes: `StarRating`
- Shared: `createValoracionSchema`, `CreateValoracionFormData`, `ValoracionCreatedResult`
- `react-hook-form`, `@hookform/resolvers/zod`

**Render condicional del boton:**
- Disabled si: `puntuacion === 0` o `comentarioLength > 1000` o `isPending`
- Con spinner si: `isPending`
- El formulario completo tiene `aria-busy={isPending}`

---

### 3.5 ValoracionReadOnly

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ValoracionReadOnly.tsx`

**Descripcion:** Card de solo lectura que muestra la valoracion ya enviada. Aparece en dos contextos: (a) inmediatamente despues de enviar, (b) al cargar la pagina si el usuario ya valoro previamente.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `valoracion` | `ValoracionCreatedResult` | Si | Datos de la valoracion existente |

**Estado Local:** Ninguno.

**Especificaciones visuales:**
- Card contenedor: `bg-[#0f1729] border border-[#10b981]/40 rounded-xl p-6 mt-6`
- Header: titulo "Tu valoracion" + icono `<CheckCircle2>` verde `text-[#10b981]`
- `<StarDisplay value={valoracion.puntuacion} size="lg" showNumeric />`
- Comentario (si existe): `text-sm text-[#e2e8f0] leading-relaxed mt-3 italic border-l-2 border-[#334155] pl-3`
- Fecha: `text-xs text-[#64748b] mt-3` formateada con `toLocaleDateString('es-ES', { day: 'numeric', month: 'short', year: 'numeric' })`

**Dependencias:**
- Lucide: `CheckCircle2`
- Componentes: `StarDisplay`
- Shared: `ValoracionCreatedResult`

---

### 3.6 ValoracionesSection

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ValoracionesSection.tsx`

**Descripcion:** Seccion completa de valoraciones para la pagina de perfil de usuario. Orquesta la carga de datos, estados de loading/error/empty y la paginacion. Es el componente de nivel mas alto para el perfil.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `userId` | `string` | Si | Identity User ID del usuario cuyas valoraciones se muestran |

**Estado Local:**
- `currentPage: number` - pagina actual del listado (default: 1)
- `sectionRef: RefObject<HTMLElement>` - ref para el scroll al cambiar pagina

**Lógica de paginacion:**
- Al cambiar `currentPage`, se hace scroll suave a `sectionRef.current` con `scrollIntoView({ behavior: 'smooth' })`
- Calcula `totalPages = Math.ceil(totalCount / pageSize)`
- Botones de pagina: hasta 5 numeros visibles en desktop, solo anterior/actual/siguiente en mobile

**Estados de render:**
1. `isLoading`: skeletons (2 cards de 36 + lista de 3 skeletons de 28 de alto)
2. `isError`: `<Alert>` con boton "Reintentar" que llama `refetch()`
3. `totalValoraciones === 0`: `<EmptyValoraciones />`
4. Default: resumen + histograma + lista + paginacion

**Dependencias:**
- shadcn/ui: `Skeleton`, `Alert`, `AlertDescription`, `Button`
- Lucide: `AlertCircle`, `Loader2`, `ChevronLeft`, `ChevronRight`
- Hooks: `useValoracionesUsuario`
- Componentes: `ValoracionListItem`, `RatingHistogram`, `StarDisplay`, `EmptyValoraciones`

---

### 3.7 ValoracionListItem

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ValoracionListItem.tsx`

**Descripcion:** Card individual de una valoracion en el listado del perfil. Muestra estrellas, avatar del autor, nombre, fecha, titulo del acuerdo y comentario.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `valoracion` | `ValoracionListItem` | Si | Datos del item de valoracion recibida |

**Estado Local:** Ninguno.

**Estructura visual:**
- Card: `bg-[#0f1729] border border-[#334155] rounded-xl p-4 hover:border-[#475569] transition-colors duration-150`
- Fila superior: `StarDisplay size="sm"` + `Avatar` (w-6 h-6) + nombre del autor + fecha
- Fecha: `new Date(valoracion.fechaCreacion).toLocaleDateString('es-ES', { day: 'numeric', month: 'short', year: 'numeric' })`
- Contexto del acuerdo: `text-xs text-[#a855f7] font-medium mb-2`
- Comentario: solo renderizado si `valoracion.comentario` tiene valor; `text-sm text-[#e2e8f0] leading-relaxed italic`
- AvatarFallback: iniciales del autor extraidas con `autorNombre.split(' ').map(p => p[0]).join('').slice(0, 2).toUpperCase()`

**Dependencias:**
- shadcn/ui: `Avatar`, `AvatarImage`, `AvatarFallback`
- Componentes: `StarDisplay`
- Shared: `ValoracionListItem` type

---

### 3.8 RatingBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RatingBadge.tsx`

**Descripcion:** Badge de puntuacion media reutilizable. Dos variantes: `compact` para cards y listados (solo estrella + numero + total) y `medium` para cabecera de perfil (estrellas + numero + total con `StarDisplay`).

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `puntuacionMedia` | `number` | Si | Puntuacion media con 1 decimal |
| `totalValoraciones` | `number` | Si | Total de valoraciones recibidas |
| `variant` | `'compact' \| 'medium'` | No (default: `'compact'`) | Variante visual |
| `className` | `string` | No | Clase CSS adicional |

**Render condicional:**
- Si `totalValoraciones === 0`: no renderizar nada (`return null`)
- Si `totalValoraciones >= 1`: renderizar el badge

**Variante `compact`:**
- `flex items-center gap-1`
- `<Star className="w-3.5 h-3.5 text-[#f59e0b]" fill="currentColor" />`
- `<span className="text-sm font-semibold text-white">{puntuacionMedia.toFixed(1)}</span>`
- `<span className="text-xs text-[#64748b]">({totalValoraciones})</span>`

**Variante `medium`:**
- `flex items-center gap-2`
- `<StarDisplay value={puntuacionMedia} size="sm" />`
- `<span className="text-base font-semibold text-white">{puntuacionMedia.toFixed(1)}</span>`
- `<span className="text-sm text-[#94a3b8]">({totalValoraciones} valoraciones)</span>`

**Dependencias:**
- Lucide: `Star`
- Componentes: `StarDisplay` (solo para variante `medium`)

---

### 3.9 EmptyValoraciones

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EmptyValoraciones.tsx`

**Descripcion:** Estado vacio de la seccion de valoraciones en el perfil. Componente sin props (informacion fija).

**Props:** Ninguna.

**Estado Local:** Ninguno.

**Estructura visual:**
- Contenedor centrado con padding
- `<Star className="w-10 h-10 text-[#334155] mx-auto mb-3" />`
- `<p className="text-[#94a3b8] text-center text-sm">"Este usuario aun no tiene valoraciones"</p>`
- `<p className="text-[#64748b] text-center text-xs mt-1">"Completa un acuerdo para recibir tu primera valoracion"</p>`

**Dependencias:**
- Lucide: `Star`

---

## 4. Hooks

### 4.1 useCreateValoracion

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCreateValoracion.ts`

**Tipo:** Mutation Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo sobre el que se crea la valoracion |
| `userIdValorado` | `string` | ID del usuario que recibe la valoracion (para invalidar cache de su perfil) |
| `onSuccess` | `(valoracion: ValoracionCreatedResult) => void \| undefined` | Callback opcional tras exito |

**Firma:**
```
useCreateValoracion(acuerdoId: string, userIdValorado: string, onSuccess?: (valoracion: ValoracionCreatedResult) => void)
```

**Retorna:** objeto de `useMutation` de TanStack Query

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(data: CreateValoracionRequest) => void` | Ejecutar la mutacion |
| `isPending` | `boolean` | True mientras se ejecuta el POST |
| `isError` | `boolean` | True si hubo error |
| `error` | `Error \| null` | Error si lo hay |

**`mutationFn`:** `(data: CreateValoracionRequest) => valoracionApi.create(acuerdoId, data)`

**`onSuccess`:**
1. Invalidar `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` - refresca el detalle del acuerdo
2. Invalidar `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userIdValorado)` - refresca las valoraciones del usuario valorado
3. Toast success usando el mensaje del servidor: `toast.success(result.messages?.[0]?.message ?? "Valoracion enviada. Gracias por tu feedback.")`
   - **Nota:** La API de valoracion devuelve el resultado como `ValoracionCreatedResult` desde `data`. El mensaje del servidor viene en `messages[0].message`. Requiere que `valoracionApi.create` devuelva el objeto completo `ServiceResponse` o que el hook tenga acceso a `messages`. Ver nota en seccion de services.
4. Llamar `onSuccess(valoracion)` si esta definido

**`onError`:** `toast.error(getValoracionErrorMessage(error.message))` usando la funcion especifica de valoraciones del shared utils (que maneja el override del codigo 4007)

**Imports requeridos:**
- `useMutation`, `useQueryClient` de `@tanstack/react-query`
- `toast` de `sonner`
- `QUERY_KEYS` de `@shared/constants`
- `getValoracionErrorMessage` de `@shared/utils/error-messages`
- `valoracionApi` de `../../infrastructure`
- Types: `CreateValoracionRequest`, `ValoracionCreatedResult` de `../../domain`

---

### 4.2 useValoracionesUsuario

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useValoracionesUsuario.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `userId` | `string` | Identity User ID del usuario cuyas valoraciones se consultan |
| `page` | `number` | Numero de pagina (default: 1) |
| `pageSize` | `number` | Resultados por pagina (default: 10) |

**Firma:**
```
useValoracionesUsuario(userId: string, page: number = 1, pageSize: number = 10)
```

**Retorna:** objeto de `useQuery`

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `ValoracionesUsuario \| undefined` | Resumen + listado paginado |
| `isLoading` | `boolean` | True en la primera carga |
| `isFetching` | `boolean` | True al cambiar de pagina (carga adicional) |
| `isError` | `boolean` | True si hubo error |
| `refetch` | `() => void` | Funcion para reintentar |

**Query Key:** `[...QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId), page, pageSize]`

**Nota sobre query key:** La clave base `byUserId(userId)` del shared es `['crowdsourcing', 'valoraciones', userId]`. Al agregar `page` y `pageSize` se distingue cada pagina en cache. Esto permite que TanStack Query mantenga en cache las distintas paginas consultadas durante la sesion.

**Configuracion:**
```
queryKey: [...QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId), page, pageSize],
queryFn: () => valoracionApi.getByUser(userId, { page, pageSize }),
staleTime: 60_000,
enabled: !!userId,
```

**Imports requeridos:**
- `useQuery` de `@tanstack/react-query`
- `QUERY_KEYS` de `@shared/constants`
- `valoracionApi` de `../../infrastructure`
- Type: `ValoracionesUsuario` de `../../domain`

---

## 5. Services (API Layer)

### 5.1 valoracionApi (ValoracionApiService)

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/valoracion.api.ts`

**Patron:** Clase con instancia exportada. Mismo patron que `acuerdoApi`, `conversacionApi`, etc.

**Interfaz `ServiceResponse<T>`:** Definida localmente en el archivo (igual que los otros api services del modulo):
```
interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}
```

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP Method |
|--------|-------|--------|----------|-------------|
| `create(acuerdoId, data)` | `acuerdoId: string`, `data: CreateValoracionRequest` | `CreateValoracionResult` | `API_ROUTES.crowdsourcing.valoraciones.create(acuerdoId)` | POST |
| `getByUser(userId, params)` | `userId: string`, `params: { page?: number; pageSize?: number }` | `ValoracionesUsuario` | `API_ROUTES.crowdsourcing.valoraciones.byUser(userId)` + query params | GET |

**Detalle de `create`:**
- Ejecuta `apiFetch<ServiceResponse<CreateValoracionResult>>(url, { method: 'POST', data })`
- Si `!response.data`: lanza `new Error(response.messages?.[0]?.errorCode ?? '5000')`
- **Nota especial:** El hook `useCreateValoracion` necesita el `message` del servidor para el toast. La solucion es que el service devuelva un objeto combinado `{ result: CreateValoracionResult, message: string }`, o alternativamente que el hook use el mensaje hardcodeado "Valoracion enviada. Gracias por tu feedback." si la respuesta exitosa siempre lleva el mismo mensaje (segun el contrato, el mensaje `messages[0].message` en el 201 es siempre ese texto). La opcion recomendada para mantener DRY y consistencia es que el service retorne solo `response.data` (el `CreateValoracionResult`) y el hook use el texto hardcodeado del contrato como fallback de toast. Esto alinea con como los otros hooks del modulo usan mensajes hardcodeados de toast.

**Detalle de `getByUser`:**
- Construye query string con `URLSearchParams` a partir de `params.page` y `params.pageSize`
- Si ambos son valores por defecto (1 y 10), omite query string
- Ejecuta `apiFetch<ServiceResponse<ValoracionesUsuario>>(url)`
- Si `!response.data`: lanza `new Error(response.messages?.[0]?.errorCode ?? '5000')`
- Retorna `response.data`

**Patron de construccion de URL con query params:**
```
const searchParams = new URLSearchParams()
if (params?.page && params.page !== 1) searchParams.set('page', String(params.page))
if (params?.pageSize && params.pageSize !== 10) searchParams.set('pageSize', String(params.pageSize))
const queryString = searchParams.toString()
const url = queryString
    ? `${API_ROUTES.crowdsourcing.valoraciones.byUser(userId)}?${queryString}`
    : API_ROUTES.crowdsourcing.valoraciones.byUser(userId)
```

**Imports requeridos:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- Types: `CreateValoracionRequest`, `CreateValoracionResult`, `ValoracionesUsuario` de `../../domain`

**Exportacion:** `export const valoracionApi = new ValoracionApiService()`

---

## 6. Tipos de Dominio

### 6.1 Modificacion de domain/types.ts

**Archivo:** `src/web/src/features/crowdsourcing/domain/types.ts`

Agregar los siguientes re-exports al final del archivo, en una seccion nueva para US-CS-06:

```
// Valoraciones Bidireccionales (US-CS-06)
CreateValoracionRequest,
ValoracionCreatedResult,
ValoracionResumen,
ValoracionListItem,
PaginatedResult,
ValoracionesUsuario,
```

Estos types provienen de `@shared/types/crowdsourcing` donde seran agregados segun el plan de shared (`plans/cs-valoraciones/shared/contracts-plan.md`).

---

## 7. Integraciones con Paginas Existentes

### 7.1 AcuerdoDetallePage (MODIFICACION)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/AcuerdoDetallePage.tsx`

**Que cambia:**
- Importar `ESTADO_ACUERDO` (ya importado) - verificar que incluye `COMPLETADO`
- Importar `ValoracionForm`, `ValoracionReadOnly`
- Agregar estado local: `valoracionEnviada: ValoracionCreatedResult | null` (default: `null`)
- Agregar derivada: `isCompletado = acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO`

**Donde se inserta:**
Dentro del `<div className="lg:col-span-2 space-y-6">` (columna principal del grid), despues de `<AcuerdoTimeline>`:

```
{isCompletado && (
    valoracionEnviada !== null ? (
        <ValoracionReadOnly valoracion={valoracionEnviada} />
    ) : (
        <ValoracionForm
            acuerdoId={acuerdo.id}
            userIdValorado={
                acuerdo.miRol === 'Artista'
                    ? acuerdo.profesional.userId
                    : acuerdo.artista.userId
            }
            onSuccess={(valoracion) => setValoracionEnviada(valoracion)}
        />
    )
)}
```

**Logica de `userIdValorado`:**
- Si `acuerdo.miRol === 'Artista'`: el valorado es el profesional (`acuerdo.profesional.userId`)
- Si `acuerdo.miRol === 'Profesional'`: el valorado es el artista (`acuerdo.artista.userId`)
- Esta logica refleja quien es "la otra parte"

**Nota sobre "ya valorado" previo:**
El contrato especifica que el backend retorna 400 con codigo `4014` si se intenta una segunda valoracion. El MVP no requiere un campo `miValoracion` en la respuesta del acuerdo; el estado `valoracionEnviada` en el componente es null al cargar y se establece con `onSuccess`. Si el usuario recarga la pagina tras haber valorado, el formulario estara de nuevo visible pero el intento de submit retornara 400 y el hook mostrara un toast de error. Esta es la UX aceptable para MVP.

**Imports adicionales:**
- `ValoracionForm` desde `../components/ValoracionForm`
- `ValoracionReadOnly` desde `../components/ValoracionReadOnly`
- Type `ValoracionCreatedResult` desde `../../domain`

---

### 7.2 PropuestaCard (MODIFICACION)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/PropuestaCard.tsx`

**Que cambia:**
- Importar `RatingBadge`
- La `PropuestaCard` ya muestra datos de `MiPropuestaList`. El badge de puntuacion del profesional requiere `puntuacionMedia` y `totalValoraciones`.
- **Enfoque para MVP:** Agregar props opcionales `puntuacionMedia?: number` y `totalValoraciones?: number` a `PropuestaCardProps`. Si el padre no las provee, el badge no se renderiza (ya que `RatingBadge` retorna `null` cuando `totalValoraciones === 0`).
- Si los datos de valoracion no estan disponibles en la respuesta de `mis-propuestas`, el badge no aparece. Esto es aceptable para MVP; en el futuro, el backend puede enriquecer el DTO de propuesta con la puntuacion del profesional.

**Donde se inserta el badge:**
En la seccion de metadatos, junto al precio, en la fila inferior antes de los botones:
```
{puntuacionMedia !== undefined && totalValoraciones !== undefined && (
    <RatingBadge
        puntuacionMedia={puntuacionMedia}
        totalValoraciones={totalValoraciones}
        variant="compact"
    />
)}
```

---

### 7.3 PerfilProfesionalPage (NUEVA)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/PerfilProfesionalPage.tsx`

**Ruta nueva:** `/crowdsourcing/profesionales/:userId`

**Descripcion:** Pagina de perfil publico del usuario en el contexto crowdsourcing. Muestra informacion basica del profesional y la seccion completa de valoraciones. Esta pagina es nueva porque actualmente solo existe `ArtistaPublicProfilePage` (para artistas en el contexto de campanias).

**Props:** Ninguna (lee `userId` via `useParams`)

**Estructura de la pagina:**
1. Loading state: skeletons de cabecera y seccion de valoraciones
2. Error state: card con mensaje de error y boton volver
3. Contenido:
   - Cabecera con nombre del usuario (a obtener de la respuesta de valoraciones o de un endpoint de perfil)
   - `<RatingBadge variant="medium" />` en la cabecera (si tiene valoraciones)
   - `<ValoracionesSection userId={userId} />`

**Nota sobre datos de cabecera para MVP:**
El endpoint `GET /api/crowdsourcing/usuarios/{userId}/valoraciones` devuelve valoraciones pero no los datos del perfil del usuario (nombre, imagen, etc.). Para MVP, la cabecera puede mostrar solo el userId o un nombre generico hasta que exista un endpoint de perfil de usuario. La pagina se centra principalmente en `<ValoracionesSection>` que es lo critico de esta US.

**Imports:**
- `useParams` de `react-router-dom`
- `ValoracionesSection` desde `../components/ValoracionesSection`
- `useValoracionesUsuario` para leer el estado de carga inicial
- shadcn/ui: `Skeleton`

---

## 8. Modificaciones a Archivos de Infraestructura y Routing

### 8.1 infrastructure/index.ts

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/index.ts`

Agregar al final:

```
// US-CS-06: Valoraciones
export { valoracionApi } from "./api/valoracion.api"
```

### 8.2 presentation/pages/index.ts

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/index.ts`

Agregar:

```
// US-CS-06: Valoraciones
export { default as PerfilProfesionalPage } from "./PerfilProfesionalPage"
```

### 8.3 router.tsx

**Archivo:** `src/web/src/app/router.tsx`

Agregar lazy import:
```
const PerfilProfesionalPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/PerfilProfesionalPage"))
```

Agregar ruta dentro del bloque de rutas publicas (`PublicLayout`):
```
<Route path="/crowdsourcing/profesionales/:userId" element={<PerfilProfesionalPage />} />
```

---

## 9. Flujo de Datos

### 9.1 Flujo: Enviar Valoracion (AcuerdoDetallePage)

```
Usuario hace click en estrella N
    |
    v
StarRating.onChange(N)
    |
    v
ValoracionForm - form.setValue('puntuacion', N)
    |
Usuario hace click en "Enviar valoracion"
    |
    v
ValoracionForm.onSubmit(formData: CreateValoracionFormData)
    |
    v
useCreateValoracion.mutate({ puntuacion, comentario })
    |
    v
valoracionApi.create(acuerdoId, data)
    |
    v
apiFetch -> POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones
    |
    v (201 Created)
ValoracionCreatedResult
    |
    v
onSuccess:
    - invalidateQueries(QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId))
    - invalidateQueries(QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userIdValorado))
    - toast.success("Valoracion enviada. Gracias por tu feedback.")
    - onSuccess(valoracion) -> AcuerdoDetallePage.setValoracionEnviada(valoracion)
    |
    v
AcuerdoDetallePage re-renderiza:
    - valoracionEnviada !== null -> renderiza <ValoracionReadOnly>
    - <ValoracionForm> desaparece (sin reload de pagina)
```

### 9.2 Flujo: Ver Valoraciones en Perfil

```
Usuario navega a /crowdsourcing/profesionales/:userId
    |
    v
PerfilProfesionalPage
    |
    v
ValoracionesSection({ userId })
    |
    v
useValoracionesUsuario(userId, page, pageSize)
    |
    v
valoracionApi.getByUser(userId, { page, pageSize })
    |
    v
apiFetch -> GET /api/crowdsourcing/usuarios/{userId}/valoraciones?page=1&pageSize=10
    |
    v (200 OK)
ValoracionesUsuario { resumen, valoraciones }
    |
    v
ValoracionesSection re-renderiza:
    - totalValoraciones === 0 -> <EmptyValoraciones>
    - totalValoraciones > 0:
        - Grid: card con puntuacion media + <StarDisplay> + total
                + <RatingHistogram distribucion={resumen.distribucion} total={resumen.totalValoraciones}>
        - Lista: <ValoracionListItem> para cada item
        - Controles de paginacion

Usuario hace click en pagina 2
    |
    v
ValoracionesSection.setCurrentPage(2)
    + scroll suave a sectionRef.current
    |
    v
useValoracionesUsuario(userId, 2, 10) - nueva query con page=2
    |
    v
isFetching=true -> spinner en controles de paginacion, lista disabled
    |
    v (200 OK)
Nuevos items reemplazan la lista
```

### 9.3 Flujo: Badge de Puntuacion en Cards

```
NecesidadDetallePage o MisPropuestasPage carga lista de propuestas
    |
    v
PropuestaCard recibe props opcionales puntuacionMedia y totalValoraciones
    |
    v (si totalValoraciones > 0)
<RatingBadge variant="compact" puntuacionMedia={} totalValoraciones={} />
    -> Renderiza: [★] {media} ({total})

    (si totalValoraciones === 0 o props no disponibles)
    -> return null (nada visible)
```

---

## 10. Dependencias de Shared

**Importar de `@shared/types/crowdsourcing`:**
- `CreateValoracionRequest`
- `ValoracionCreatedResult`
- `ValoracionResumen`
- `ValoracionListItem`
- `PaginatedResult<T>`
- `ValoracionesUsuario`

**Importar de `@shared/schemas/crowdsourcing.schema`:**
- `createValoracionSchema`
- `CreateValoracionFormData`

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdsourcing.valoraciones.byUserId`
- `API_ROUTES.crowdsourcing.valoraciones.create`
- `API_ROUTES.crowdsourcing.valoraciones.byUser`
- `ESTADO_ACUERDO.COMPLETADO` (ya existente en el proyecto)

**Importar de `@shared/utils/error-messages`:**
- `getValoracionErrorMessage` (funcion nueva agregada en el plan shared)

---

## 11. Archivos a Crear/Modificar

### Archivos Nuevos

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdsourcing/infrastructure/api/valoracion.api.ts` | Service class | Clase `ValoracionApiService` con metodos `create` y `getByUser` |
| `src/web/src/features/crowdsourcing/application/hooks/useCreateValoracion.ts` | Hook | Mutation hook para POST valoracion |
| `src/web/src/features/crowdsourcing/application/hooks/useValoracionesUsuario.ts` | Hook | Query hook paginado para GET valoraciones de usuario |
| `src/web/src/features/crowdsourcing/presentation/components/StarRating.tsx` | Componente | Input interactivo 1-5 estrellas con hover y accesibilidad |
| `src/web/src/features/crowdsourcing/presentation/components/StarDisplay.tsx` | Componente | Visualizacion read-only con soporte de medias estrellas |
| `src/web/src/features/crowdsourcing/presentation/components/RatingHistogram.tsx` | Componente | Barras de distribucion animadas |
| `src/web/src/features/crowdsourcing/presentation/components/ValoracionForm.tsx` | Componente | Card con formulario + React Hook Form + Zod |
| `src/web/src/features/crowdsourcing/presentation/components/ValoracionReadOnly.tsx` | Componente | Card read-only post-envio o ya-valorado |
| `src/web/src/features/crowdsourcing/presentation/components/ValoracionesSection.tsx` | Componente | Orquestador de perfil: resumen + histograma + lista + paginacion |
| `src/web/src/features/crowdsourcing/presentation/components/ValoracionListItem.tsx` | Componente | Item individual de valoracion en lista |
| `src/web/src/features/crowdsourcing/presentation/components/RatingBadge.tsx` | Componente | Badge compacto/medium de puntuacion media |
| `src/web/src/features/crowdsourcing/presentation/components/EmptyValoraciones.tsx` | Componente | Estado vacio con mensaje correcto |
| `src/web/src/features/crowdsourcing/presentation/pages/PerfilProfesionalPage.tsx` | Pagina | Pagina de perfil con seccion de valoraciones |

### Archivos Modificados

| Archivo | Cambio |
|---------|--------|
| `src/web/src/features/crowdsourcing/domain/types.ts` | Agregar re-exports de 6 tipos de valoraciones desde `@shared/types/crowdsourcing` |
| `src/web/src/features/crowdsourcing/infrastructure/index.ts` | Agregar `export { valoracionApi }` |
| `src/web/src/features/crowdsourcing/presentation/pages/index.ts` | Agregar export de `PerfilProfesionalPage` |
| `src/web/src/features/crowdsourcing/presentation/pages/AcuerdoDetallePage.tsx` | Agregar estado `valoracionEnviada`, derivada `isCompletado`, y seccion condicional con `ValoracionForm`/`ValoracionReadOnly` |
| `src/web/src/features/crowdsourcing/presentation/components/PropuestaCard.tsx` | Agregar props opcionales `puntuacionMedia?` y `totalValoraciones?` y renderizar `RatingBadge` |
| `src/web/src/app/router.tsx` | Agregar lazy import de `PerfilProfesionalPage` y ruta `/crowdsourcing/profesionales/:userId` |

---

## 12. Decisiones de Diseno

### 12.1 Ubicacion dentro de `crowdsourcing` (no feature separada)

Los componentes se crean dentro de `src/features/crowdsourcing/` en lugar de una feature propia `valoraciones/`, siguiendo el patron establecido en el proyecto donde todas las sub-features de crowdsourcing conviven en la misma feature raiz. Esto es consistente con como `mensajeria` (US-CS-05) y `acuerdos` (US-CS-04) se implementaron.

### 12.2 StarRating custom vs libreria

`shadcn/ui` no tiene un componente de rating. La decision es implementarlo custom con Lucide + Tailwind para evitar dependencias adicionales. El componente tiene el alcance correcto (solo 5 estrellas enteras para el formulario de envio) y los requisitos de accesibilidad son manejables. Si en el futuro se requiere mas funcionalidad (fracciones en input), se puede evaluar `react-rating-stars-component` o similar.

### 12.3 Estado "ya valorado" sin campo en el DTO de acuerdo

El MVP no agrega un campo `miValoracion` al DTO de acuerdo. El estado se maneja localmente en `AcuerdoDetallePage` via `useState`. Si el usuario recarga la pagina, el formulario estara visible pero el intento de submit sera rechazado por el backend con 400 + toast de error informativo. Esto es aceptable para MVP y evita complejidad adicional en el backend.

### 12.4 Toast message del servidor

El mensaje de toast de exito usa el texto exacto del contrato: `"Valoracion enviada. Gracias por tu feedback."` Este mensaje puede venir del campo `messages[0].message` de la respuesta 201 del backend (que siempre es ese texto segun el contrato), o puede hardcodearse en el hook. Se recomienda hardcodear para simplicidad y evitar dependencia de la estructura `messages` en el hook.

### 12.5 Invalidacion de cache

Al crear una valoracion exitosa, se invalidan dos query keys:
1. El detalle del acuerdo (`byId(acuerdoId)`): por si el acuerdo expone en el futuro un campo `miValoracion`
2. Las valoraciones del usuario valorado (`byUserId(userIdValorado)`): para que el perfil del usuario muestre la nueva valoracion si el visitor lo abre

### 12.6 `PerfilProfesionalPage` vs insercion en paginas existentes

Se crea una nueva pagina en lugar de insertar directamente en `ArtistaPublicProfilePage` porque:
- `ArtistaPublicProfilePage` es para artistas del contexto de campanias (diferente dominio)
- Los profesionales del crowdsourcing pueden no ser artistas (pueden ser tecnicos de sonido, etc.)
- La ruta `/crowdsourcing/profesionales/:userId` es mas semantica

---

## 13. Notas sobre Accesibilidad

| Componente | Requisito | Implementacion |
|------------|-----------|----------------|
| `StarRating` | Navegacion teclado completa | `role="radiogroup"`, cada estrella como boton con `aria-label` y `aria-pressed`; handler de `onKeyDown` para flechas izquierda/derecha |
| `StarRating` | Estado disabled | `aria-disabled="true"` en el grupo; `tabIndex={-1}` en cada boton |
| `StarDisplay` | Solo lectura semantica | `role="img"` con `aria-label="Puntuacion: {value} de 5 estrellas"`; estrellas con `aria-hidden="true"` |
| `ValoracionForm` | Error de validacion | `role="alert"` en mensajes de error; `aria-invalid="true"` en input con error; `aria-describedby` apuntando al id del mensaje |
| `ValoracionForm` | Estado submitting | `aria-busy="true"` en el formulario; texto del boton cambia a "Enviando..." |
| `ValoracionListItem` | Avatares | `<AvatarImage alt={valoracion.autorNombre} />`; fallback muestra iniciales como texto |
| `ValoracionesSection` | Paginacion | `aria-label="Paginacion de valoraciones"` en contenedor; `aria-current="page"` en pagina activa; `aria-label="Pagina anterior"` y `aria-label="Pagina siguiente"` en botones nav |
| Todos | Focus ring | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` en todos los elementos interactivos |

---

## 14. Checklist

### Componentes
- [ ] `StarRating` con soporte de hover, click y teclado (radiogroup)
- [ ] `StarDisplay` con soporte de medias estrellas (fraccion 0.5 via opacidad)
- [ ] `RatingHistogram` con barras animadas al montar
- [ ] `ValoracionForm` encapsula React Hook Form + Zod + `useCreateValoracion`
- [ ] `ValoracionReadOnly` para estado post-envio con borde verde
- [ ] `ValoracionesSection` gestiona carga, error, empty y paginacion
- [ ] `ValoracionListItem` con avatar, nombre, fecha, contexto y comentario
- [ ] `RatingBadge` con variantes compact y medium; retorna null si totalValoraciones === 0
- [ ] `EmptyValoraciones` con mensaje exacto del criterio AC-CS06-9
- [ ] Todos usan `FC<Props>` con interface explicita; ninguno usa `any`

### Hooks
- [ ] `useCreateValoracion` invalida dos query keys tras exito
- [ ] `useCreateValoracion` usa `getValoracionErrorMessage` para manejo de errores contextual
- [ ] `useValoracionesUsuario` incluye `page` y `pageSize` en el query key para cache por pagina
- [ ] `useValoracionesUsuario` tiene `enabled: !!userId`

### Services
- [ ] `valoracionApi.create` lanza `Error(errorCode)` si `!response.data`
- [ ] `valoracionApi.getByUser` construye query string con URLSearchParams
- [ ] Patron de clase con instancia exportada (igual que otros apis del modulo)

### Integraciones
- [ ] `AcuerdoDetallePage` muestra seccion solo cuando `estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO`
- [ ] `AcuerdoDetallePage` calcula `userIdValorado` correctamente segun `acuerdo.miRol`
- [ ] `PropuestaCard` acepta props opcionales de valoracion y renderiza `RatingBadge`
- [ ] `PerfilProfesionalPage` creada y registrada en el router
- [ ] Ruta `/crowdsourcing/profesionales/:userId` agregada en `router.tsx`
- [ ] `infrastructure/index.ts` exporta `valoracionApi`
- [ ] `domain/types.ts` re-exporta los 6 nuevos tipos de valoraciones

### Shared
- [ ] Types importados de `@shared/types/crowdsourcing` (no duplicados)
- [ ] Schema `createValoracionSchema` importado de `@shared/schemas/crowdsourcing.schema`
- [ ] Constants `QUERY_KEYS` y `API_ROUTES` con claves de valoraciones
- [ ] `getValoracionErrorMessage` de `@shared/utils/error-messages`

### Formulario
- [ ] React Hook Form con `zodResolver(createValoracionSchema)`
- [ ] Contador de caracteres: gris hasta 900, ambar hasta 1000, rojo y boton disabled al superar
- [ ] Boton disabled si `puntuacion === 0` (antes de primera seleccion)
- [ ] Spinner y todo el formulario disabled durante `isPending`
- [ ] Toast con texto exacto: "Valoracion enviada. Gracias por tu feedback."
- [ ] Transicion de formulario a `ValoracionReadOnly` sin reload de pagina

### Accesibilidad
- [ ] `StarRating` implementa `role="radiogroup"` con navegacion por teclado
- [ ] `StarDisplay` tiene `role="img"` con `aria-label` descriptivo
- [ ] Mensajes de error con `role="alert"`
- [ ] Avatares con `alt` text correcto
- [ ] Paginacion con `aria-label` descriptivos
- [ ] Focus ring visible en todos los elementos interactivos

### Responsive
- [ ] `StarRating` usa `size="md"` (w-8 h-8) en mobile para facilitar el tap
- [ ] Resumen + histograma apilados verticalmente en mobile (grid-cols-1 md:grid-cols-2)
- [ ] Paginacion simplificada en mobile (anterior + numero actual + siguiente)
- [ ] `ValoracionListItem` fecha debajo del nombre del autor en mobile (< 768px)
