# Plan Frontend: cp-tareas-promocion (Landing)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

- **Screens:** 1 pagina + 1 dialog modal
- **Componentes nuevos:** 5 (`MisTareasPage`, `TareaCard`, `CompletarTareaDialog`, `TareaStatusBadge`, `TareaRewardInfo`)
- **Hooks nuevos:** 2 (`useMisTareas`, `useCompletarTarea`)
- **Services nuevos:** 1 (`tareas.service.ts`)
- **Cambios en archivos existentes:** Router (`router.tsx`), shared types/constants/schemas (gestionados en contracts-plan.md)

### Contexto de integracion

Esta feature se integra dentro de la feature `crowdpromotion` existente en `src/web/src/features/crowdpromotion/`. Todos los archivos nuevos se crean dentro de un subdirectorio `tareas/` de esa feature. La pagina se registra como ruta protegida en `DashboardLayout` (requiere auth), siguiendo el patron de `MisProgramasPage` que usa el mismo layout.

---

## 2. Estructura de Carpetas

```
src/web/src/features/crowdpromotion/
├── tareas/                                          NUEVO subdirectorio
│   ├── domain/
│   │   └── index.ts                                 Re-exports de shared types
│   ├── infrastructure/
│   │   └── tareas.service.ts                        API calls HTTP
│   ├── application/
│   │   └── hooks/
│   │       ├── useMisTareas.ts                      Query hook GET mis-tareas
│   │       └── useCompletarTarea.ts                 Mutation hook POST completar
│   └── presentation/
│       ├── components/
│       │   ├── TareaCard.tsx                        Card individual de tarea
│       │   ├── CompletarTareaDialog.tsx              Dialog formulario de completado
│       │   ├── TareaStatusBadge.tsx                 Badge de estado de tarea
│       │   ├── TareaRewardInfo.tsx                  Bloque de recompensa y repeticiones
│       │   └── index.ts                             Barrel exports
│       └── pages/
│           └── MisTareasPage.tsx                    Pagina principal

src/web/src/app/
└── router.tsx                                       MODIFICAR: agregar ruta nueva
```

---

## 3. Componentes

### 3.1 MisTareasPage

**Archivo:** `src/web/src/features/crowdpromotion/tareas/presentation/pages/MisTareasPage.tsx`

**Tipo:** Pagina (default export, lazy-loaded)

**Props:** Ninguna (lee `programaId` desde `useParams`)

**Estado Local:**
- `tareaSeleccionada: MisTareasItem | null` - Tarea actualmente seleccionada para el dialog
- `dialogOpen: boolean` - Controla la visibilidad del `CompletarTareaDialog`
- `esReenvio: boolean` - Indica si la accion es un re-envio por rechazo previo (cambia el titulo del dialog)

**Dependencias:**
- Hooks: `useMisTareas`, `useParams`, `useNavigate`
- Componentes: `TareaCard`, `CompletarTareaDialog`, `Skeleton` (shadcn), `Button` (shadcn), `AlertCircle` (lucide), `ChevronRight` (lucide), `ClipboardX` (lucide)
- Shared: `APP_ROUTES`, `ESTADO_TAREA_PROMO`, `puedeCompletarTarea`

**Responsabilidad:**
Pagina principal de tareas del promotor. Carga las tareas del programa mediante `useMisTareas(programaId)`. Renderiza el breadcrumb de navegacion, la cabecera con el titulo del programa, y la lista de `TareaCard` en todos sus estados (loading: 3 skeletons, empty, error, datos). Gestiona el estado del dialog de completado: cuando el usuario clickea cualquier boton de accion en una card, esta pagina abre el `CompletarTareaDialog` pasandole la tarea seleccionada y el modo (primera vez, repeticion, re-envio). Cierra el dialog al completar con exito.

**Logica de ordenamiento de tareas:** Antes de renderizar, ordena el array `items` del response con la siguiente prioridad: (1) accionables primero - tareas donde `puedeCompletarTarea(item)` es `true` o `item.miEstado?.estadoTareaId === 4`, (2) pendientes de validacion (`estadoTareaId === 2`), (3) validadas (`estadoTareaId === 3`). Dentro de cada grupo, ordenar por `orden` ascendente.

**Redireccion:** Si `useMisTareas` retorna error con `errorCode === '4026'` (no aprobado) o `'4024'` (programa inactivo), navegar a `/promotor/mis-programas` con `toast.error(...)`.

**Registro en router:** Se agrega como ruta protegida dentro del `DashboardLayout` con path `/promotor/programas/:programaId/tareas`. Requiere que el usuario este autenticado (ya garantizado por `DashboardLayout`).

---

### 3.2 TareaCard

**Archivo:** `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tarea` | `MisTareasItem` | Si | Datos de la tarea con estado personal del promotor |
| `onCompletar` | `(tarea: MisTareasItem, esReenvio: boolean) => void` | Si | Callback cuando el usuario clickea cualquier boton de accion que abre el dialog |
| `isProgramaActivo` | `boolean` | Si | Si el programa esta activo. Cuando false, deshabilita todas las acciones |
| `isSubmitting` | `boolean` | No | Cuando true, deshabilita los botones de accion de esta card (mutable durante envio del dialog) |
| `className` | `string` | No | Clases CSS adicionales |

**Estado Local:** Ninguno (componente puramente presentacional).

**Dependencias:**
- Componentes shadcn/ui: `Card`, `Badge`, `Button`, `Separator`
- Componentes propios: `TareaStatusBadge`, `TareaRewardInfo`
- Lucide icons: `RefreshCw`, `Play`, `RotateCcw`, `ExternalLink`, `Clock`, `AlertCircle`, `Lock`, `Calendar`
- Shared: `ESTADO_TAREA_PROMO`, `puedeCompletarTarea`, `ESTADO_TAREA_PROMO_LABELS`
- Utils: `cn` de `@/lib/utils`

**Responsabilidad:**
Renderiza la card de una tarea individual del promotor. Muestra el badge de tipo de evento, el badge de repetible (si aplica), el badge de estado (`TareaStatusBadge`), el nombre, la descripcion (maxima 3 lineas con `line-clamp-3`), el separador, el bloque de recompensa (`TareaRewardInfo`), y el bloque de accion condicional.

El bloque de accion (esquina inferior derecha) tiene 6 variantes mutuamente excluyentes determinadas por el estado de `tarea.miEstado`:

1. **Sin estado (null) o primera vez:** Boton "Completar tarea" con gradiente pink-purple. Llama `onCompletar(tarea, false)`.
2. **Repetible con cupo disponible (`puedeCompletarTarea` = true y `miEstado != null`):** Boton "Completar de nuevo" mismo gradiente. Llama `onCompletar(tarea, false)`.
3. **Rechazada (`estadoTareaId === 4`):** Boton outline "Re-enviar con nueva prueba". Llama `onCompletar(tarea, true)`.
4. **Completada/pendiente validacion (`estadoTareaId === 2`):** Boton ghost "Ver prueba enviada". Abre `tarea.miEstado.urlPruebaCompletado` en nueva tab.
5. **Max repeticiones alcanzado (`puedeCompletarTarea` = false y repetible):** Texto con icono Lock "Maximo de repeticiones alcanzado".
6. **No repetible ya validada (`estadoTareaId === 3` y `!esRepetible`):** Sin boton (solo badge de estado).

Cuando `isProgramaActivo` es false, todos los botones de accion se reemplazan por el texto con icono Lock "Programa inactivo".

Cuando `isSubmitting` es true, los botones de accion estan `disabled`.

El bloque de motivo de rechazo (panel amber) es visible cuando `miEstado?.estadoTareaId === 4` y `miEstado.comentarioValidacion` no es undefined. El bloque de prueba enviada (panel amber informativo) es visible cuando `miEstado?.estadoTareaId === 2`.

Hover: `border-[#334155]` pasa a `border-[#a855f7]/30` con `transition-colors duration-200`.

---

### 3.3 CompletarTareaDialog

**Archivo:** `src/web/src/features/crowdpromotion/tareas/presentation/components/CompletarTareaDialog.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Controla visibilidad del dialog |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback de cambio de estado del dialog |
| `tarea` | `MisTareasItem \| null` | Si | Tarea a completar. Null cuando el dialog esta cerrado |
| `programaId` | `string` | Si | ID del programa, necesario para el endpoint de completar |
| `esReenvio` | `boolean` | Si | Si es un re-envio por rechazo previo. Cambia titulo y muestra nota de contexto |
| `onSuccess` | `(response: CompletarTareaResponse) => void` | Si | Callback al completar exitosamente. La pagina padre actualiza el estado |

**Estado Local:**
- `watchedComentario: string` - Valor actual del campo comentario para el contador de caracteres (accedido via `watch('comentarioPromotor')` de React Hook Form)

**Dependencias:**
- Hooks: `useCompletarTarea`, `useForm` (react-hook-form), `zodResolver` (@hookform/resolvers/zod)
- Componentes shadcn/ui: `Dialog`, `DialogContent`, `DialogTitle`, `DialogClose`, `Button`, `Input`, `Textarea`, `Form`, `FormField`, `FormItem`, `FormLabel`, `FormControl`, `FormMessage`, `Separator`
- Lucide icons: `X`, `Info`, `ExternalLink`, `Loader2`
- Shared: `completarTareaSchema`, `CompletarTareaFormData`, `VALIDATION`

**Responsabilidad:**
Dialog modal para que el promotor envie la prueba de completado de una tarea. Contiene el formulario con React Hook Form + Zod resolver usando `completarTareaSchema` de shared.

Estructura interna:
1. **Header:** Titulo dinamico: "Completar: {tarea.nombre}" (primera vez), "Completar de nuevo: {tarea.nombre}" (repeticion), o "Re-enviar: {tarea.nombre}" (re-envio).
2. **Nota de re-envio:** Panel amber visible solo cuando `esReenvio === true`.
3. **Seccion instrucciones:** Label "INSTRUCCIONES" + texto de `tarea.descripcion`. Si `tarea.instruccionesUrl` no es undefined, muestra link "Ver instrucciones completas" con `target="_blank"`.
4. **Separador**
5. **Formulario** (`id="form-completar-tarea"`):
   - Campo `urlPruebaCompletado`: Input tipo "url", validacion onBlur, placeholder `https://...`. Muestra `FormMessage` para errores Zod.
   - Campo `comentarioPromotor`: Textarea `min-h-[80px]`, resize-none. Contador de caracteres en tiempo real: `{n} / 500 caracteres`.
6. **Footer:** Boton Cancelar + Boton "Enviar prueba" (type="submit" form="form-completar-tarea"). Durante submit: spinner + "Enviando..." + disabled. Cancelar tambien disabled durante submit.

Al abrir el dialog (`open` cambia a `true`), resetear el formulario con `form.reset()`.

Al cerrar (sin envio), solo cerrar. No llamar a la API.

Durante envio (`isPending` del mutation): `onOpenChange` del Dialog usa `disabled` (prop `onInteractOutside` y `onEscapeKeyDown` bloqueados cuando `isPending`).

Al exito: llamar `onSuccess(response)`, luego cerrar el dialog.

Al error de API: el dialog permanece abierto, el toast de error es manejado por `useCompletarTarea.onError`.

---

### 3.4 TareaStatusBadge

**Archivo:** `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaStatusBadge.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoTareaId` | `EstadoTareaPromo \| undefined` | Si | ID del estado. `undefined` significa "no completada" (nunca enviada) |
| `className` | `string` | No | Clases CSS adicionales |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes shadcn/ui: `Badge`
- Lucide icons: `Clock`, `CheckCircle`, `XCircle`
- Shared: `ESTADO_TAREA_PROMO`, `ESTADO_TAREA_PROMO_LABELS`, `EstadoTareaPromo`
- Utils: `cn` de `@/lib/utils`

**Responsabilidad:**
Renderiza el badge de estado en la esquina superior derecha de la `TareaCard`. Encapsula la logica de estilos por estado en una tabla de configuracion interna `BADGE_CONFIG`:

| Estado | Clases | Icono | Label |
|--------|--------|-------|-------|
| `undefined` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155]` | Ninguno | "No completada" |
| `2` (Completada) | `bg-amber-950/50 text-amber-400 border border-amber-800/50` | `Clock` | "PENDIENTE" |
| `3` (Validada) | `bg-green-950/50 text-green-400 border border-green-800/50` | `CheckCircle` | "VALIDADA" |
| `4` (Rechazada) | `bg-red-950/50 text-red-400 border border-red-800/50` | `XCircle` | "RECHAZADA" |

Patron identico al `InscripcionEstadoBadge` existente en la feature.

---

### 3.5 TareaRewardInfo

**Archivo:** `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaRewardInfo.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tarea` | `MisTareasItem` | Si | Datos de la tarea para calcular texto de recompensa y progreso de repeticiones |

**Estado Local:** Ninguno.

**Dependencias:**
- Lucide icons: `DollarSign`, `Award`
- Shared: `ESTADO_TAREA_PROMO`
- Utils: `cn`

**Responsabilidad:**
Bloque informativo que se coloca despues del separador en `TareaCard`. Muestra:

1. **Fila de recompensa:** Icono `DollarSign` + label "Recompensa:" + valor formateado. Logica de formato:
   - Si `importeRecompensa` no es undefined: `"{importeRecompensa} {monedaNombre ?? 'EUR'} por ejecucion"`
   - Si `puntosRecompensa` no es undefined: `"{puntosRecompensa} puntos"`
   - Si ambos son undefined: `"Sin recompensa definida"` en color muted

2. **Fila de progreso de repeticiones** (visible solo si `esRepetible === true` Y `miEstado` no es undefined Y `miEstado.vecesCompletada > 0`):
   - Texto: `"Completada: {vecesCompletada} de {maxRepeticiones ?? 'N'} veces"`
   - Separador " | "
   - Texto: `"Ultima: {formatDate(miEstado.fechaUltimaCompletada)}"` donde `formatDate` usa `toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })`

---

## 4. Hooks

### 4.1 useMisTareas

**Archivo:** `src/web/src/features/crowdpromotion/tareas/application/hooks/useMisTareas.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa. La query se deshabilita si el valor es falsy |

**Retorna:** `UseQueryResult<MisTareasResponse, Error>` de TanStack Query

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `MisTareasResponse \| undefined` | Response completo con `programaId`, `programaTitulo`, `items[]` |
| `isLoading` | `boolean` | `true` durante la primera carga |
| `isFetching` | `boolean` | `true` en cualquier carga incluyendo re-fetches |
| `isError` | `boolean` | `true` si la query fallo |
| `error` | `Error & { errorCode?: string } \| null` | Error con `errorCode` extraido del response del backend |
| `refetch` | `() => void` | Funcion para reintentar manualmente |

**Query Key:** `QUERY_KEYS.crowdpromotion.tareas.mis(programaId)`
(nueva entrada en shared constants segun contracts-plan.md seccion 4.2)

**Configuracion:**
- `enabled`: `!!programaId`
- `staleTime`: `30 * 1000` (30 segundos, consistente con `useMisProgramas`)
- `retry`: `1` (consistente con el patron del proyecto)

**Notas de implementacion:**
El error extraido del service debe incluir el `errorCode` del backend para que `MisTareasPage` pueda redirigir en los casos `4026` (no autorizado) y `4024` (programa inactivo). Seguir el patron `extractError` de `InscripcionService`.

---

### 4.2 useCompletarTarea

**Archivo:** `src/web/src/features/crowdpromotion/tareas/application/hooks/useCompletarTarea.ts`

**Tipo:** Mutation Hook

**Parametros del mutationFn:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa (path param) |
| `tareaId` | `string` | ID de la tarea a completar (path param) |
| `data` | `CompletarTareaRequest` | Body del POST: `{ urlPruebaCompletado, comentarioPromotor? }` |

El mutationFn recibe un objeto: `{ programaId: string; tareaId: string; data: CompletarTareaRequest }`.

**Retorna:** `UseMutationResult<CompletarTareaResponse, Error & { errorCode?: string }, ...>` de TanStack Query

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(vars) => void` | Dispara la mutacion de forma fire-and-forget |
| `mutateAsync` | `(vars) => Promise<CompletarTareaResponse>` | Dispara y retorna promesa. Usar en `onSubmit` del dialog |
| `isPending` | `boolean` | `true` mientras se ejecuta la mutacion |
| `isError` | `boolean` | `true` si la mutacion fallo |
| `reset` | `() => void` | Resetea el estado de la mutacion |

**onSuccess:**
1. `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.tareas.mis(programaId) })` para que el listado se refresque con el nuevo estado
2. `toast.success("Tarea enviada. El artista revisara tu prueba.")` - Para primera vez y repeticion
3. Si es re-envio (lo determina el componente padre que llama con contexto), el toast cambia a `"Prueba re-enviada para validacion."`. NOTA: el hook no sabe si es re-envio; el componente `CompletarTareaDialog` gestiona el toast via `onSuccess` callback, o el hook expone un parametro `esReenvio` opcional en el objeto de variables.

**Estrategia de toast segun contexto de re-envio:**
El objeto de variables incluye `esReenvio: boolean` para que el `onSuccess` pueda diferenciar el mensaje:
```
variables: { programaId, tareaId, data, esReenvio }
```

**onError:**
Extrae el `errorCode` del error:
- `errorCode === '4027'`: `toast.error("Esta tarea ya fue completada y no es repetible.")`
- `errorCode === '4028'`: `toast.error("Alcanzaste el maximo de repeticiones para esta tarea.")`
- `errorCode === '4024'`: `toast.error("Este programa no esta activo. No puedes completar tareas.")`
- Default: `toast.error(getTareaPromocionErrorMessage(errorCode))` usando la funcion de shared

---

## 5. Services

### 5.1 tareasService

**Archivo:** `src/web/src/features/crowdpromotion/tareas/infrastructure/tareas.service.ts`

**Patron:** Clase con instancia singleton exportada, identico a `InscripcionService`.

**Importaciones:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- Types de shared: `MisTareasResponse`, `CompletarTareaRequest`, `CompletarTareaResponse`

**Metodos:**

| Metodo | Input | Output (Promise) | Endpoint | HTTP |
|--------|-------|-----------------|----------|------|
| `misTareas` | `programaId: string` | `MisTareasResponse` | `API_ROUTES.crowdpromotion.programas.misTareas(programaId)` | GET |
| `completarTarea` | `programaId: string, tareaId: string, data: CompletarTareaRequest` | `CompletarTareaResponse` | `API_ROUTES.crowdpromotion.programas.completarTarea(programaId, tareaId)` | POST |

**Interface ServiceResponse interna** (privada al archivo):
```typescript
interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}
```

**Funcion extractError privada:**
Identica a la de `InscripcionService`:
```typescript
function extractError(messages: Array<{ message: string; errorCode: string }>): Error {
    const firstError = messages.find(m => m.errorCode && !m.errorCode.startsWith("0"))
    const error = new Error(firstError?.message || "Error desconocido")
    ;(error as Error & { errorCode: string }).errorCode = firstError?.errorCode ?? "5000"
    return error
}
```

**Detalle de implementacion `misTareas`:**
```
GET {API_ROUTES.crowdpromotion.programas.misTareas(programaId)}
Autorizacion: Bearer JWT (agregado automaticamente por el interceptor de apiFetch)
Response esperado: ServiceResponse<MisTareasResponse>
Si messages tiene error (errorCode no empieza por "0"): throw extractError(messages)
Retorna: response.data
```

**Detalle de implementacion `completarTarea`:**
```
POST {API_ROUTES.crowdpromotion.programas.completarTarea(programaId, tareaId)}
Body: data (CompletarTareaRequest)
Autorizacion: Bearer JWT
Response esperado: ServiceResponse<CompletarTareaResponse>
Si messages tiene error: throw extractError(messages)
Retorna: response.data
```

---

## 6. Domain Re-exports

**Archivo:** `src/web/src/features/crowdpromotion/tareas/domain/index.ts`

Exporta los tipos necesarios de shared para que los componentes internos de la feature los importen con path limpio:

```typescript
export type {
    EstadoTareaPromo,
    MiEstadoTarea,
    MisTareasItem,
    MisTareasResponse,
    CompletarTareaRequest,
    CompletarTareaResponse,
} from '@shared/types/crowdpromotion'
```

---

## 7. Flujo de Datos

### Flujo de carga (GET mis-tareas)

```
MisTareasPage monta
    ↓
useParams() -> programaId
    ↓
useMisTareas(programaId)
    ↓
tareasService.misTareas(programaId)
    ↓
apiFetch GET /api/crowdpromotion/programas/{programaId}/mis-tareas
    ↓
Backend -> ServiceResponse<MisTareasResponse>
    ↓
tareasService: extractError o retornar data
    ↓
TanStack Query cachea en QUERY_KEYS.crowdpromotion.tareas.mis(programaId)
    ↓
MisTareasPage recibe { data, isLoading, isError }
    ↓
Ordena items por prioridad de accion
    ↓
Renderiza lista de TareaCard
```

### Flujo de completado (POST completar)

```
Usuario click boton accion en TareaCard
    ↓
TareaCard.onCompletar(tarea, esReenvio)
    ↓
MisTareasPage sets tareaSeleccionada + esReenvio + dialogOpen=true
    ↓
CompletarTareaDialog renderiza con tarea y programaId
    ↓
Usuario rellena form URL + comentario (validacion Zod onBlur)
    ↓
Submit form -> useCompletarTarea.mutateAsync({ programaId, tareaId, data, esReenvio })
    ↓
tareasService.completarTarea(programaId, tareaId, data)
    ↓
apiFetch POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar
    ↓
Backend -> ServiceResponse<CompletarTareaResponse>
    ↓
onSuccess:
  - invalidateQueries(QUERY_KEYS.crowdpromotion.tareas.mis(programaId))
  - toast.success(mensaje segun esReenvio)
    ↓
CompletarTareaDialog.onSuccess(response)
    ↓
MisTareasPage cierra dialog + resetea tareaSeleccionada
    ↓
TanStack Query re-fetcha mis-tareas -> TareaCard actualiza badge de estado
```

---

## 8. Cambios en Archivos Existentes

### 8.1 router.tsx

**Archivo:** `src/web/src/app/router.tsx`

**Cambio:** Agregar la importacion lazy y la ruta protegida dentro del bloque `DashboardLayout`:

```typescript
// Agregar import lazy:
const MisTareasPage = lazy(
    () => import("@/features/crowdpromotion/tareas/presentation/pages/MisTareasPage")
)

// Agregar dentro de <Route element={<DashboardLayout />}>:
<Route path="/promotor/programas/:programaId/tareas" element={<MisTareasPage />} />
```

**Justificacion de usar DashboardLayout:**
La pagina requiere autenticacion (el promotor debe estar logueado) y sigue el mismo patron que `MisProgramasPage` que ya usa `DashboardLayout`. El layout aplica la verificacion de `isAuthenticated` y redirige a `/auth/login` si no hay sesion activa.

### 8.2 shared constants/types/schemas

Estos cambios estan documentados en `plans/cp-tareas-promocion/shared/contracts-plan.md` y no son responsabilidad de este plan de landing. Sin embargo, este plan asume que los siguientes simbolos ya existen en shared antes de implementar la landing:

**De `@shared/types/crowdpromotion`:**
- `EstadoTareaPromo`
- `MiEstadoTarea`
- `MisTareasItem`
- `MisTareasResponse`
- `CompletarTareaRequest`
- `CompletarTareaResponse`

**De `@shared/schemas/crowdpromotion.schema`:**
- `completarTareaSchema`
- `CompletarTareaFormData`

**De `@shared/constants`:**
- `QUERY_KEYS.crowdpromotion.tareas.mis(programaId)`
- `API_ROUTES.crowdpromotion.programas.misTareas(programaId)`
- `API_ROUTES.crowdpromotion.programas.completarTarea(programaId, tareaId)`
- `ESTADO_TAREA_PROMO`
- `ESTADO_TAREA_PROMO_LABELS`
- `VALIDATION.TAREA_COMENTARIO_PROMOTOR_MAX`

**De `@shared/utils/mappers`:**
- `puedeCompletarTarea(item: MisTareasItem): boolean`

**De `@shared/utils/error-messages`:**
- `getTareaPromocionErrorMessage(errorCode: string): string`

---

## 9. Dependencias de Shared

**Importar de `@shared/`:**

| Simbolo | Archivo origen | Uso |
|---------|---------------|-----|
| `EstadoTareaPromo` | `types/crowdpromotion` | Tipado del union type de estados |
| `MiEstadoTarea` | `types/crowdpromotion` | Tipado del estado personal del promotor en una tarea |
| `MisTareasItem` | `types/crowdpromotion` | Tipado de cada tarea en el listado |
| `MisTareasResponse` | `types/crowdpromotion` | Tipado del response completo de GET mis-tareas |
| `CompletarTareaRequest` | `types/crowdpromotion` | Tipado del body del POST completar |
| `CompletarTareaResponse` | `types/crowdpromotion` | Tipado del response del POST completar |
| `completarTareaSchema` | `schemas/crowdpromotion.schema` | Zod schema para validacion del formulario |
| `CompletarTareaFormData` | `schemas/crowdpromotion.schema` | Type inferido del schema Zod |
| `QUERY_KEYS.crowdpromotion.tareas.mis` | `constants` | Query key para useMisTareas |
| `API_ROUTES.crowdpromotion.programas.misTareas` | `constants` | Endpoint GET |
| `API_ROUTES.crowdpromotion.programas.completarTarea` | `constants` | Endpoint POST |
| `ESTADO_TAREA_PROMO` | `constants` | Constantes numericas de estados (2, 3, 4) |
| `ESTADO_TAREA_PROMO_LABELS` | `constants` | Labels de estado para aria-label |
| `VALIDATION.TAREA_COMENTARIO_PROMOTOR_MAX` | `constants` | Valor 500 para el contador del textarea |
| `puedeCompletarTarea` | `utils/mappers` | Logica de negocio RN-04 + RN-05 |
| `getTareaPromocionErrorMessage` | `utils/error-messages` | Mensajes de error localizados |

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdpromotion/tareas/domain/index.ts` | Re-exports | Re-exporta types de shared para imports limpios dentro de la feature |
| `src/web/src/features/crowdpromotion/tareas/infrastructure/tareas.service.ts` | Service | HTTP calls a GET mis-tareas y POST completar |
| `src/web/src/features/crowdpromotion/tareas/application/hooks/useMisTareas.ts` | Hook | TanStack Query hook de lectura |
| `src/web/src/features/crowdpromotion/tareas/application/hooks/useCompletarTarea.ts` | Hook | TanStack Query mutation hook de escritura |
| `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaStatusBadge.tsx` | Component | Badge de estado de tarea (sin estado local) |
| `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaRewardInfo.tsx` | Component | Bloque de recompensa y progreso repeticiones |
| `src/web/src/features/crowdpromotion/tareas/presentation/components/TareaCard.tsx` | Component | Card completa de una tarea individual |
| `src/web/src/features/crowdpromotion/tareas/presentation/components/CompletarTareaDialog.tsx` | Component | Dialog modal con formulario de completado |
| `src/web/src/features/crowdpromotion/tareas/presentation/components/index.ts` | Barrel | Re-exporta todos los componentes de la carpeta |
| `src/web/src/features/crowdpromotion/tareas/presentation/pages/MisTareasPage.tsx` | Page | Pagina principal de tareas del promotor (default export) |

## 11. Archivos a Modificar

| Archivo | Cambio |
|---------|--------|
| `src/web/src/app/router.tsx` | Agregar import lazy `MisTareasPage` y ruta `/promotor/programas/:programaId/tareas` en `DashboardLayout` |

---

## 12. Consideraciones de UX por Estado

### Estados de la pagina MisTareasPage

| Estado | Comportamiento |
|--------|---------------|
| `isLoading` | 3 items `Skeleton` con `animate-pulse`, altura `h-[180px]`, `bg-[#1e1e38] rounded-xl mb-4` |
| `isError` | Card centrada con `AlertCircle` rojo, texto "No se pudieron cargar las tareas", boton "Reintentar" que llama `refetch()` |
| `data.items.length === 0` | Empty state con `ClipboardX` en contenedor `bg-[#1e1e38] rounded-xl`, titulo "Sin tareas activas", subtitulo "El artista aun no ha configurado tareas de promocion" |
| `data.items.length > 0` | Lista ordenada de `TareaCard` con `space-y-4` |

### Comportamiento del dialog durante envio

Cuando `useCompletarTarea.isPending` es `true`:
- El boton "Enviar prueba" muestra `<Loader2 className="animate-spin">` y texto "Enviando..."
- El boton "Cancelar" esta `disabled`
- `onInteractOutside` del Dialog retorna `event.preventDefault()` (no cierra al clickar fuera)
- `onEscapeKeyDown` del Dialog retorna `event.preventDefault()` (no cierra con Escape)
- El campo de URL y el textarea estan `disabled`

### Notificaciones toast (libreria `sonner`)

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Completado exitoso (primera vez o repeticion) | `toast.success` | "Tarea enviada. El artista revisara tu prueba." |
| Completado exitoso (re-envio por rechazo) | `toast.success` | "Prueba re-enviada para validacion." |
| Error 4027 (no repetible ya completada) | `toast.error` | "Esta tarea ya fue completada y no es repetible." |
| Error 4028 (max repeticiones) | `toast.error` | "Alcanzaste el maximo de repeticiones para esta tarea." |
| Error 4024 (programa inactivo) | `toast.error` | "Este programa no esta activo. No puedes completar tareas." |
| Error 4026 en carga (no aprobado) | `toast.error` + redirect | "No estas aprobado en este programa." -> navegar a `/promotor/mis-programas` |
| Error 5000 u otro | `toast.error` | Mensaje de `getTareaPromocionErrorMessage(errorCode)` |

---

## 13. Checklist de Implementacion

- [ ] Crear `src/web/src/features/crowdpromotion/tareas/domain/index.ts` con re-exports de shared
- [ ] Crear `tareas.service.ts` con metodos `misTareas` y `completarTarea`
- [ ] Verificar que `API_ROUTES.crowdpromotion.programas.misTareas` y `.completarTarea` existen en shared (prerequisito)
- [ ] Verificar que `QUERY_KEYS.crowdpromotion.tareas.mis` existe en shared (prerequisito)
- [ ] Crear `useMisTareas.ts` con `useQuery` y query key de shared
- [ ] Crear `useCompletarTarea.ts` con `useMutation`, invalidacion y toasts
- [ ] Crear `TareaStatusBadge.tsx` con tabla de configuracion por estado
- [ ] Crear `TareaRewardInfo.tsx` con logica de recompensa y progreso
- [ ] Crear `TareaCard.tsx` con todas las variantes de accion y bloques condicionales
- [ ] Crear `CompletarTareaDialog.tsx` con form React Hook Form + Zod + contador textarea
- [ ] Crear `index.ts` barrel en components
- [ ] Crear `MisTareasPage.tsx` con breadcrumb, cabecera, skeleton, empty, error y lista
- [ ] Verificar que `puedeCompletarTarea` de shared es correctamente importado y usado
- [ ] Agregar ruta en `router.tsx`
- [ ] Componentes usan shadcn/ui exclusivamente (no HTML nativo para botones/inputs)
- [ ] Hooks siguen patron `useQuery`/`useMutation` con query keys de shared
- [ ] Services manejan errores con `extractError` y no silencian errores
- [ ] Types importados de shared (no redefinidos en la feature)
- [ ] Formulario usa React Hook Form + `zodResolver(completarTareaSchema)` de shared
- [ ] No se usa `any` en ningun archivo TypeScript
- [ ] El dialog bloquea cierre durante `isPending`
- [ ] La lista de tareas se ordena por prioridad de accion antes de renderizar
