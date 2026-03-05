# Plan Frontend: cp-tareas-promocion (Admin)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/admin

---

## 1. Resumen

- **Screens:** 1 (nueva tab "Pendientes" en pagina de detalle de programa existente)
- **Componentes:** 6 (1 tab container, 1 list, 1 card, 2 dialogs, 1 paginacion)
- **Hooks:** 3 (1 query, 2 mutations)
- **Services:** 1 (tareas.service.ts)

### Objetivo del Admin

El artista propietario del programa ve una nueva pestana "Pendientes (N)" en la pagina de detalle del programa (`/crowdpromotion/programas/[id]`). Desde ahi puede revisar completados pendientes de validacion, abrir un dialog para validar con comentario opcional, y abrir un dialog para rechazar con motivo obligatorio. Tras cada accion, la card desaparece de la lista con animacion y se muestra un toast.

### Integracion con pagina existente

La pagina de detalle del programa ya existe en:
`src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx`

Este archivo ya contiene el componente `<Tabs>` con pestanas: Info general, Tareas, Promotores, Solicitudes, Aprobados, Bloqueados, Resumen. El plan agrega una pestana nueva "Pendientes" a ese `<Tabs>`, de forma identica a como se agregaron las pestanas existentes.

---

## 2. Estructura de Carpetas

```
src/admin/src/
│
├── app/(dashboard)/crowdpromotion/programas/[id]/
│   └── components/
│       ├── PromoProgramaDetailClient.tsx         MODIFICAR: agregar tab "Pendientes"
│       ├── PromoProgramaPendientesTab.tsx         NUEVO
│       ├── TareaPendienteCard.tsx                 NUEVO
│       ├── ValidarTareaDialog.tsx                 NUEVO
│       └── RechazarTareaDialog.tsx                NUEVO
│
├── hooks/
│   ├── use-tareas-pendientes.ts                   NUEVO (query hook)
│   ├── use-validar-tarea.ts                       NUEVO (mutation hook)
│   └── use-rechazar-tarea.ts                      NUEVO (mutation hook)
│
└── services/
    └── tareas.service.ts                          NUEVO
```

### Notas de organizacion

- Los hooks siguen el patron de archivo-por-hook del proyecto (`use-inscripciones.ts`, `use-inscripciones-mutations.ts`). Las dos mutations van en archivos separados por claridad.
- El service sigue el patron de clase instanciada de los services existentes (`campaniaService`, `rewardService`, `inscripcionService`).
- No se crea una carpeta `crowdpromotion/` nueva en `services/` ni en `hooks/` porque el proyecto usa la raiz de esas carpetas (sin sub-carpetas por dominio).

---

## 3. Componentes

### 3.1 PromoProgramaDetailClient (MODIFICAR)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx`

**Tipo de cambio:** Agregar una nueva `<TabsTrigger>` y `<TabsContent>` al `<Tabs>` existente.

**Cambios a realizar:**

1. Importar `PromoProgramaPendientesTab` desde `./PromoProgramaPendientesTab`.
2. Agregar nueva `<TabsTrigger value="pendientes">` en `<TabsList>` con badge de count.
3. Agregar nueva `<TabsContent value="pendientes">` con `<PromoProgramaPendientesTab programaId={programa.id} />`.

**Posicion del tab en la lista:** Despues de "Tareas" y antes de "Solicitudes", para agrupar contenido operativo del programa.

**Badge del count:**

El count de tareas pendientes no viene del objeto `programa` actual (que no tiene ese campo). Se recomienda obtenerlo de la misma query `useTareasPendientes` dentro de `PromoProgramaPendientesTab` y elevarlo al `PromoProgramaDetailClient` via prop o usando el hook directamente en el `PromoProgramaDetailClient`.

**Estrategia recomendada:** Llamar `useTareasPendientes` en `PromoProgramaDetailClient` solo para obtener `totalCount` y mostrarlo en el badge del tab. La pestana `PromoProgramaPendientesTab` usara el mismo hook (TanStack Query deduplica la llamada por queryKey identico). El hook solo se ejecuta cuando el `programaId` esta disponible.

**Nueva interface a agregar:**

```typescript
// No hay cambio de interface; programaId ya esta disponible como prop
```

**Modificacion del TabsList:**

```
TabsTrigger "pendientes" con:
- Label: "Pendientes"
- Badge rojo si totalCount > 0: bg-red-500/20 text-red-400 border border-red-500/30
- El badge muestra el numero (totalCount)
```

---

### 3.2 PromoProgramaPendientesTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaPendientesTab.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa para fetch de tareas pendientes |

**Estado local:**

| Variable | Tipo | Descripcion |
|----------|------|-------------|
| `selectedItem` | `TareaPendienteItem \| null` | Completado seleccionado para validar o rechazar |
| `dialogMode` | `'validar' \| 'rechazar' \| null` | Controla que dialog esta abierto |
| `page` | `number` | Pagina actual para paginacion (inicia en 1) |

**Dependencias:**

- Hooks: `useTareasPendientes(programaId, page)`
- Componentes internos: `TareaPendienteCard`, `ValidarTareaDialog`, `RechazarTareaDialog`
- Componentes UI: `Skeleton`, `Button`, `Badge`
- Iconos: `CheckCircle`, `AlertCircle`, `ChevronLeft`, `ChevronRight`, `Inbox`

**Responsabilidad:**

Orquesta la tab completa de tareas pendientes. Hace la query de datos paginados, renderiza la lista de `TareaPendienteCard`, gestiona el estado de seleccion para abrir `ValidarTareaDialog` o `RechazarTareaDialog`, y muestra controles de paginacion.

**Estados de UI a cubrir:**

| Estado | Componente/Elemento |
|--------|---------------------|
| Loading inicial | 3 `<Skeleton>` con `h-[120px] rounded-xl bg-[#1e1e38] mb-4 animate-pulse` |
| Error de carga | Card con `<AlertCircle>` rojo + boton "Reintentar" que llama `refetch()` |
| Sin pendientes (empty) | Icono `<CheckCircle>` verde + textos "No hay tareas pendientes de validacion" |
| Con items | Lista de `TareaPendienteCard` + controles de paginacion |

**Layout de paginacion:**

```
div: flex items-center justify-between mt-6
  p: "Mostrando {pageSize} de {totalCount} pendientes"  (text-sm text-[#64748b])
  div: flex items-center gap-2
    Button anterior (disabled si page === 1)
    span: "Pagina {page} de {totalPages}"
    Button siguiente (disabled si page === totalPages)
```

---

### 3.3 TareaPendienteCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/TareaPendienteCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `item` | `TareaPendienteItem` | Si | Datos del completado pendiente a mostrar |
| `onValidar` | `(item: TareaPendienteItem) => void` | Si | Callback al hacer click en "Validar" |
| `onRechazar` | `(item: TareaPendienteItem) => void` | Si | Callback al hacer click en "Rechazar" |
| `isProcessing` | `boolean` | No | Deshabilita botones mientras hay una mutacion en curso para este item |

**Estado local:**

- `isHiding: boolean` - Animacion de desaparicion al procesar exitosamente. Se activa desde el padre via `isProcessing` o al recibir un evento de exito. Implementar con `opacity-0 max-h-0 overflow-hidden` + `transition-all duration-400`.

**Nota sobre `isHiding`:** El padre (`PromoProgramaPendientesTab`) gestiona el estado `selectedItem`. Al confirmar exito en los dialogs, se invalida la query y TanStack Query refresca la lista. La card desaparece naturalmente del re-render. No se necesita estado `isHiding` explicito en el card; la animacion de fade-out la maneja el dialog al cerrarse antes de que la lista refresque. Simplificar a solo mostrar/ocultar via clase CSS controlada por el padre si fuera necesario.

**Dependencias:**

- Componentes UI: `Card`, `Button`, `Avatar`, `AvatarFallback`, `Separator`
- Iconos: `Check`, `X`, `ExternalLink`, `Clock`
- Utils: `formatRelativeDate` (funcion local del mismo archivo, identica a la de `SolicitudCard.tsx`)

**Responsabilidad:**

Renderiza una fila de completado pendiente con: nombre y tipo del promotor, nombre de la tarea con numero de ejecucion ordinal, URL de prueba como link externo, comentario del promotor (o texto "(sin comentario)"), fecha relativa, y botones Validar/Rechazar.

**Especificaciones visuales:**

```
Card: bg-[#151525] border-[#334155] p-5 mb-4
  div: flex items-start justify-between gap-4
    div: flex-1 min-w-0  (columna izquierda)
      p: nombre promotor (text-base font-semibold text-white)
        span: tipo promotor (text-xs text-[#64748b] ml-2 font-normal)
      p: "tareaNombre (Nta vez)" (text-sm text-[#94a3b8] mt-0.5)
      div: URL de prueba (flex items-center gap-1.5 mt-2)
        span: "Prueba:" (text-xs text-[#64748b] shrink-0)
        a: link truncado con <ExternalLink> (text-sm text-[#a855f7])
      p: comentario en italica con borde izquierdo (o "(sin comentario)")
      p: fecha relativa con <Clock>
    div: flex items-center gap-2 shrink-0  (columna derecha)
      Button "Validar": bg-green-950/50 text-green-400 border border-green-800/50
      Button "Rechazar": border-red-800/50 text-red-400
```

**Funcion helper `formatOrdinal(n: number): string`:**

Devuelve "1ra", "2da", "3ra", "4ta", ... vez. Logica:
- 1 -> "1ra vez"
- 2 -> "2da vez"
- 3 -> "3ra vez"
- n >= 4 -> `${n}ta vez`

Definir como funcion local en el archivo.

---

### 3.4 ValidarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/ValidarTareaDialog.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `isOpen` | `boolean` | Si | Controla visibilidad del dialog |
| `onClose` | `() => void` | Si | Callback al cerrar sin accion |
| `item` | `TareaPendienteItem \| null` | Si | Datos del completado a validar. Si es null, no renderizar contenido |
| `programaId` | `string` | Si | Necesario para la mutacion |

**Estado local:**

| Variable | Tipo | Descripcion |
|----------|------|-------------|
| `comentarioValidacion` | `string` | Valor del campo textarea, controlado con `useState` |

**Dependencias:**

- Hooks: `useValidarTarea` (mutation)
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`, `Textarea`, `Button`, `Label`, `Separator`
- Iconos: `Check`, `Loader2`, `ExternalLink`, `DollarSign`, `X`

**Responsabilidad:**

Muestra los datos del completado seleccionado en modo solo lectura (tarea, promotor, numero de ejecucion, URL de prueba, comentario del promotor). Muestra el bloque verde de recompensa a acreditar si `item` tuviera ese dato disponible. Proporciona un campo de comentario opcional y el boton "Validar tarea". Al confirmar llama `useValidarTarea.mutate`. Al exito, muestra toast de exito y llama `onClose`.

**Nota sobre recompensa a acreditar:** El objeto `TareaPendienteItem` del listado NO incluye el importe de la recompensa (ver contrato: la response de GET tareas-pendientes no expone `importeRecompensa`). El bloque verde de recompensa definido en el mockup (`Pantalla 4`) requiere ese dato, que solo se conoce al completarse la accion (en el `ValidarTareaResponse.recompensaAcreditada`). Estrategia: mostrar el bloque verde de recompensa en el toast de exito (despues de la llamada), no en el dialog de confirmacion previo. El dialog muestra solo los datos disponibles en `TareaPendienteItem`. Esta decision simplifica el plan y evita una segunda llamada extra para obtener el importe antes de validar.

**Layout del dialog:**

```
Dialog: open={isOpen} onOpenChange={onClose}
  DialogContent: bg-[#151525] border-[#334155] max-w-md rounded-xl p-0
    div header: px-6 pt-6 pb-4 border-b border-[#334155]
      DialogTitle: "Validar tarea completada"
    div datos: px-6 py-4 flex flex-col gap-3
      fila "Tarea:": label + valor (tareaNombre)
      fila "Promotor:": label + valor (promotorNombre + tipo)
      fila "Ejecucion:": label + valor (ordinal)
      fila "Prueba:": label + link <a> con ExternalLink
      div comentario promotor: bg-[#0f0f1f] border rounded-lg p-3 italic (o "(sin comentario)")
    Separator: bg-[#334155]
    div formulario: px-6 py-4
      Label: "Comentario de validacion (opcional)"
      Textarea: controlado, bg-[#0f0f1f] min-h-[72px] resize-none
    DialogFooter: px-6 pb-6 pt-2 border-t
      Button "Cancelar": variant="ghost"
      Button "Validar tarea": bg-green-600 hover:bg-green-700 + Loader2 si isPending
```

**Logica de submit:**

```
handleValidar():
  mutate({ programaId, tareaPromotorId: item.tareaPromotorId, comentarioValidacion })
  onSuccess(response):
    const msg = response.recompensaAcreditada
      ? `Tarea validada. Se acreditaron ${response.recompensaAcreditada} ${response.monedaNombre} en la wallet del promotor.`
      : "Tarea validada exitosamente."
    toast.success(msg)
    onClose()
  onError:
    toast.error("No se pudo validar la tarea. Intentalo de nuevo.")
```

**Comportamiento de cierre:** `onOpenChange={onClose}` en `Dialog` pero bloquear cierre si `isPending` usando `onOpenChange={(open) => { if (!open && !isPending) onClose() }}`. Limpiar `comentarioValidacion` al abrir (via `useEffect([isOpen])`).

---

### 3.5 RechazarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/RechazarTareaDialog.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `isOpen` | `boolean` | Si | Controla visibilidad del dialog |
| `onClose` | `() => void` | Si | Callback al cerrar sin accion |
| `item` | `TareaPendienteItem \| null` | Si | Datos del completado a rechazar |
| `programaId` | `string` | Si | Necesario para la mutacion |

**Estado local:**

| Variable | Tipo | Descripcion |
|----------|------|-------------|
| `comentarioValidacion` | `string` | Valor del textarea, controlado (required) |
| `touched` | `boolean` | Para mostrar validacion solo despues de primer submit o blur |

**Alternativa con React Hook Form + Zod:**

Usar `useForm<RechazarTareaFormData>` con `zodResolver(rechazarTareaSchema)` para ser consistente con el patron del proyecto. El schema `rechazarTareaSchema` ya esta definido en `src/shared/schemas/crowdpromotion.schema.ts` (ver contracts-plan.md seccion 3.2). Esto elimina la necesidad de `touched` manual.

| Campo del form | Schema | Descripcion |
|----------------|--------|-------------|
| `comentarioValidacion` | `z.string().min(1).max(500)` | Obligatorio, min 1 char en schema, pero el ui-ux.md indica min 10 chars para la validacion de frontend |

**Nota sobre min chars:** El contracts.md indica `min(1)` en backend pero el ui-ux.md indica `min 10 chars` en frontend para dar feedback al artista sobre el nivel de detalle esperado. El schema de shared tiene `min(1)`. Crear una regla de validacion local `min(10, 'El motivo debe tener al menos 10 caracteres')` en el resolver del formulario de este componente especificamente, o ampliar el schema. Opcion recomendada: sobrescribir el schema localmente con `.superRefine` o usar una variante extendida solo en el Admin.

**Dependencias:**

- Hooks: `useRechazarTarea` (mutation), `useForm` de `react-hook-form`, `zodResolver`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`, `Textarea`, `Button`, `Label`, `Separator`
- Schemas: `rechazarTareaSchema` de `@shared/schemas/crowdpromotion.schema`
- Iconos: `X`, `Loader2`, `ExternalLink`, `AlertTriangle`

**Responsabilidad:**

Similar a `ValidarTareaDialog` pero con flujo de rechazo. Muestra mismo bloque de datos readonly. Agrega bloque de aviso amarillo: "El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba." Campo de motivo obligatorio con contador de caracteres (0/500). Boton rojo "Rechazar tarea".

**Layout del dialog:**

```
Dialog: open={isOpen} onOpenChange={...}
  DialogContent: bg-[#151525] border-[#334155] max-w-md rounded-xl p-0
    div header: px-6 pt-6 pb-4 border-b border-[#334155]
      DialogTitle: "Rechazar tarea"
    div datos: px-6 py-4 flex flex-col gap-3
      (mismas filas que ValidarTareaDialog: Tarea, Promotor, Ejecucion, URL, comentario)
    Separator: bg-[#334155]
    div aviso: px-6 py-3 bg-amber-950/20 border-y border-amber-900/30 flex items-start gap-2.5
      AlertTriangle: w-4 h-4 text-amber-400 shrink-0 mt-0.5
      p: "El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba." (text-xs text-amber-300/80)
    div formulario: px-6 py-4
      form: onSubmit={handleSubmit(onSubmit)}
        Label: "Motivo del rechazo" + span rojo "*"
        Textarea: {...register('comentarioValidacion')}, min-h-[80px] focus:border-red-500
        p: contador "{n} / 500 caracteres" (text-right text-xs text-[#64748b])
        FormMessage con error si campo invalido
    DialogFooter: px-6 pb-6 pt-2 border-t
      Button "Cancelar": variant="ghost"
      Button "Rechazar tarea": bg-red-600/80 border-red-700/50 + Loader2 si isPending
```

**Logica de submit:**

```
onSubmit(formData):
  mutate({ programaId, tareaPromotorId: item.tareaPromotorId, comentarioValidacion: formData.comentarioValidacion })
  onSuccess:
    toast("Tarea rechazada. El promotor podra re-enviar con nueva prueba.")
    onClose()
  onError:
    toast.error("No se pudo rechazar la tarea. Intentalo de nuevo.")
```

---

## 4. Hooks

### 4.1 useTareasPendientes

**Archivo:** `src/admin/src/hooks/use-tareas-pendientes.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa a consultar |
| `page` | `number` | Numero de pagina (default 1) |
| `pageSize` | `number` | Items por pagina (default `TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE` = 10) |

**Retorna:** El objeto completo de `useQuery` de TanStack Query con:

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `TareasPendientesResponse \| undefined` | Response paginado del endpoint |
| `data.items` | `TareaPendienteItem[]` | Lista de completados pendientes |
| `data.totalCount` | `number` | Total de items para paginacion |
| `data.page` | `number` | Pagina actual |
| `data.pageSize` | `number` | Items por pagina |
| `data.totalPages` | `number` | Total de paginas |
| `isLoading` | `boolean` | True en el primer fetch sin datos en cache |
| `isError` | `boolean` | True si la query fallo |
| `refetch` | `function` | Para el boton "Reintentar" en estado de error |

**Query Key:** `QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId, { page, pageSize })`

**Configuracion:**

```
queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId, { page, pageSize })
queryFn: () => tareasService.getTareasPendientes(programaId, { page, pageSize })
enabled: !!programaId
staleTime: 0               // Siempre refetch para mostrar datos frescos
retry: false               // Patron del proyecto para queries de listas de admin
```

**Importaciones:**

```typescript
import { useQuery } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS, TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE } from "@shared/constants"
import type { TareasPendientesResponse } from "@shared/types"
```

---

### 4.2 useValidarTarea

**Archivo:** `src/admin/src/hooks/use-validar-tarea.ts`

**Tipo:** Mutation Hook

**Parametros del mutate:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `tareaPromotorId` | `string` | ID del completado (PromoTareaPromotor.Id) |
| `comentarioValidacion` | `string \| undefined` | Comentario opcional del artista |

**Tipo interno de params:**

```typescript
interface ValidarTareaParams {
    programaId: string
    tareaPromotorId: string
    comentarioValidacion?: string
}
```

**Retorna:** El objeto completo de `useMutation` con:

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(params: ValidarTareaParams, options?) => void` | Ejecutar la mutacion |
| `mutateAsync` | `(params: ValidarTareaParams) => Promise<ValidarTareaResponse>` | Version async |
| `isPending` | `boolean` | True mientras la llamada esta en curso |

**Acciones en onSuccess:**

- Invalidar `QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId)` para refrescar la lista
- NO mostrar toast aqui: el toast se muestra en el componente `ValidarTareaDialog` para tener acceso al `response.recompensaAcreditada` y construir el mensaje dinamico

**Acciones en onError:**

- No mostrar toast aqui (el componente lo maneja para mensajes contextuales)

**Nota sobre onSuccess/onError:** A diferencia del patron de `useAprobarInscripcion` que pone el toast en el hook, este hook deja el toast al componente. Esto es necesario porque el mensaje de validacion incluye el importe acreditado del response. El hook solo invalida queries en `onSuccess`. Los toasts los manejan los dialogs via callbacks `onSuccess`/`onError` del `mutate`.

**Importaciones:**

```typescript
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS } from "@shared/constants"
import type { ValidarTareaRequest, ValidarTareaResponse } from "@shared/types"
```

---

### 4.3 useRechazarTarea

**Archivo:** `src/admin/src/hooks/use-rechazar-tarea.ts`

**Tipo:** Mutation Hook

**Parametros del mutate:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `tareaPromotorId` | `string` | ID del completado |
| `comentarioValidacion` | `string` | Motivo del rechazo (obligatorio) |

**Tipo interno de params:**

```typescript
interface RechazarTareaParams {
    programaId: string
    tareaPromotorId: string
    comentarioValidacion: string
}
```

**Retorna:** El objeto completo de `useMutation`.

**Acciones en onSuccess:**

- Invalidar `QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId)` para refrescar la lista

**Acciones en onError:**

- No mostrar toast aqui (el componente lo maneja)

**Importaciones:**

```typescript
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS } from "@shared/constants"
import type { RechazarTareaRequest, RechazarTareaResponse } from "@shared/types"
```

---

## 5. Services

### 5.1 tareasService

**Archivo:** `src/admin/src/services/tareas.service.ts`

**Patron:** Clase instanciada (identico a `campaniaService`, `rewardService`, `inscripcionService`).

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getTareasPendientes` | `programaId: string, params?: { page?: number; pageSize?: number }` | `Promise<TareasPendientesResponse>` | `GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes` |
| `validarTarea` | `programaId: string, tareaPromotorId: string, data: ValidarTareaRequest` | `Promise<ValidarTareaResponse>` | `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar` |
| `rechazarTarea` | `programaId: string, tareaPromotorId: string, data: RechazarTareaRequest` | `Promise<RechazarTareaResponse>` | `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar` |

**Detalle de cada metodo:**

**`getTareasPendientes`:**

```
Construir URLSearchParams con page y pageSize si existen.
URL base: API_ROUTES.crowdpromotion.programas.tareasPendientes(programaId)
Adjuntar querystring si hay params.
apiFetch<ServiceResponse<TareasPendientesResponse>>(url)
Verificar errores en response.messages (misma logica que inscripcionService).
Si error, throw new Error(getTareaPromocionErrorMessage(errorCode)).
Retornar response.data.
```

**`validarTarea`:**

```
URL: API_ROUTES.crowdpromotion.programas.validarTarea(programaId, tareaPromotorId)
apiFetch<ServiceResponse<ValidarTareaResponse>>(url, { method: "PATCH", data })
Verificar errores en response.messages.
Si error, throw new Error(getTareaPromocionErrorMessage(errorCode)).
Retornar response.data.
```

**`rechazarTarea`:**

```
URL: API_ROUTES.crowdpromotion.programas.rechazarTarea(programaId, tareaPromotorId)
apiFetch<ServiceResponse<RechazarTareaResponse>>(url, { method: "PATCH", data })
Verificar errores en response.messages.
Si error, throw new Error(getTareaPromocionErrorMessage(errorCode)).
Retornar response.data.
```

**Importaciones del service:**

```typescript
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getTareaPromocionErrorMessage } from "@shared/utils/error-messages"
import type {
    TareasPendientesResponse,
    ValidarTareaRequest,
    ValidarTareaResponse,
    RechazarTareaRequest,
    RechazarTareaResponse,
    ServiceResponse,
} from "@shared/types"
```

**Export:**

```typescript
export const tareasService = new TareasService()
```

---

## 6. Flujo de Datos

### Flujo principal: Ver tareas pendientes

```
PromoProgramaDetailClient monta
    |
    +--> useTareasPendientes(programaId, page=1)
    |        |
    |        +--> tareasService.getTareasPendientes(programaId, { page: 1, pageSize: 10 })
    |                 |
    |                 +--> apiFetch GET /api/crowdpromotion/programas/{id}/tareas-pendientes?page=1&pageSize=10
    |                 |        |
    |                 |        +--> Backend verifica ArtistaId del JWT vs PromoPrograma.ArtistaId
    |                 |        +--> Retorna TareasPendientesResponse paginado
    |                 |
    |                 +--> Verifica errores en response.messages
    |                 +--> Retorna response.data
    |
    +--> PromoProgramaDetailClient muestra count en badge del tab "Pendientes"
    +--> PromoProgramaPendientesTab renderiza TareaPendienteCard[] + paginacion
```

### Flujo: Validar tarea

```
Usuario hace click en "Validar" en TareaPendienteCard
    |
    +--> PromoProgramaPendientesTab: setSelectedItem(item), setDialogMode('validar')
    |
    +--> ValidarTareaDialog abre con item y programaId
    |
    [Usuario revisa datos, opcionalmente escribe comentario, click "Validar tarea"]
    |
    +--> ValidarTareaDialog: useValidarTarea.mutate({ programaId, tareaPromotorId, comentarioValidacion })
    |        |
    |        +--> tareasService.validarTarea(programaId, tareaPromotorId, { comentarioValidacion })
    |                 |
    |                 +--> apiFetch PATCH /api/.../tareas-promotor/{tareaPromotorId}/validar
    |                 |        |
    |                 |        +--> Backend: actualiza estado a Validada, crea WalletTransaccion
    |                 |        +--> Retorna ValidarTareaResponse
    |                 |
    |                 +--> Retorna response.data
    |
    +--> onSuccess en ValidarTareaDialog:
    |        +--> toast.success("Tarea validada. Se acreditaron X EUR...")
    |        +--> onClose() -> cierra dialog, limpia selectedItem
    |
    +--> useValidarTarea.onSuccess (hook):
             +--> queryClient.invalidateQueries(QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId))
             +--> TanStack Query refetch -> lista actualizada -> card desaparece
```

### Flujo: Rechazar tarea

```
Usuario hace click en "Rechazar" en TareaPendienteCard
    |
    +--> PromoProgramaPendientesTab: setSelectedItem(item), setDialogMode('rechazar')
    |
    +--> RechazarTareaDialog abre con item y programaId
    |
    [Usuario escribe motivo de rechazo (obligatorio, min 10 chars), click "Rechazar tarea"]
    |
    +--> RechazarTareaDialog: handleSubmit valida con Zod -> si valido:
    |        useRechazarTarea.mutate({ programaId, tareaPromotorId, comentarioValidacion })
    |            |
    |            +--> tareasService.rechazarTarea(...) -> PATCH .../rechazar
    |            +--> Backend: actualiza estado a Rechazada, guarda ComentarioValidacion
    |
    +--> onSuccess en RechazarTareaDialog:
    |        +--> toast("Tarea rechazada. El promotor podra re-enviar con nueva prueba.")
    |        +--> onClose()
    |
    +--> useRechazarTarea.onSuccess (hook):
             +--> queryClient.invalidateQueries(QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId))
```

### Flujo: Cambio de pagina

```
Usuario hace click en "Siguiente" en controles de paginacion
    |
    +--> PromoProgramaPendientesTab: setPage(page + 1)
    |
    +--> useTareasPendientes(programaId, page + 1) -> query key cambia
    |        |
    |        +--> TanStack Query fetch nueva pagina
    |        +--> Muestra skeletons durante loading
    +--> Lista actualiza con items de nueva pagina
```

---

## 7. Dependencias de Shared

**Importar de `@shared/types`:**

| Type | Descripcion |
|------|-------------|
| `TareaPendienteItem` | DTO de un completado pendiente (para card y dialogs) |
| `TareasPendientesResponse` | Response paginado del GET |
| `ValidarTareaRequest` | Body del PATCH validar |
| `ValidarTareaResponse` | Response del PATCH validar (contiene `recompensaAcreditada`) |
| `RechazarTareaRequest` | Body del PATCH rechazar |
| `RechazarTareaResponse` | Response del PATCH rechazar |
| `ServiceResponse` | Wrapper generico de response del backend |

**Importar de `@shared/constants`:**

| Constante | Uso |
|-----------|-----|
| `QUERY_KEYS.crowdpromotion.tareas.pendientes` | Query key para fetch y invalidacion |
| `API_ROUTES.crowdpromotion.programas.tareasPendientes` | Endpoint GET |
| `API_ROUTES.crowdpromotion.programas.validarTarea` | Endpoint PATCH validar |
| `API_ROUTES.crowdpromotion.programas.rechazarTarea` | Endpoint PATCH rechazar |
| `TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE` | Tamano de pagina por defecto (10) |

**Importar de `@shared/schemas/crowdpromotion.schema`:**

| Schema | Uso |
|--------|-----|
| `rechazarTareaSchema` | Validacion del formulario en `RechazarTareaDialog` |
| `RechazarTareaFormData` | Tipo inferido del schema para `useForm` |
| `validarTareaSchema` | Opcional: si se decide usar React Hook Form en `ValidarTareaDialog` |
| `ValidarTareaFormData` | Tipo inferido para el form de validacion |

**Importar de `@shared/utils/error-messages`:**

| Funcion | Uso |
|---------|-----|
| `getTareaPromocionErrorMessage` | Para convertir error codes del backend a mensajes legibles en el service |

---

## 8. Detalle de Props de Comunicacion entre Componentes

### Jerarquia de componentes y props

```
PromoProgramaDetailClient (programaId: string)
    |
    +--> [usa] useTareasPendientes(programaId, 1) -> { data.totalCount } para badge del tab
    |
    +--> PromoProgramaPendientesTab (programaId: string)
              |
              +--> [usa] useTareasPendientes(programaId, page)
              |
              +--> TareaPendienteCard[]
              |       props: item, onValidar, onRechazar
              |
              +--> ValidarTareaDialog
              |       props: isOpen, onClose, item, programaId
              |       [usa] useValidarTarea
              |
              +--> RechazarTareaDialog
                      props: isOpen, onClose, item, programaId
                      [usa] useRechazarTarea
```

### Gestion de estado de dialogs en PromoProgramaPendientesTab

```typescript
// Estado en PromoProgramaPendientesTab
const [selectedItem, setSelectedItem] = useState<TareaPendienteItem | null>(null)
const [dialogMode, setDialogMode] = useState<'validar' | 'rechazar' | null>(null)
const [page, setPage] = useState(1)

// Handlers
const handleValidar = (item: TareaPendienteItem) => {
    setSelectedItem(item)
    setDialogMode('validar')
}

const handleRechazar = (item: TareaPendienteItem) => {
    setSelectedItem(item)
    setDialogMode('rechazar')
}

const handleClose = () => {
    setSelectedItem(null)
    setDialogMode(null)
}

// Render de dialogs
<ValidarTareaDialog
    isOpen={dialogMode === 'validar'}
    onClose={handleClose}
    item={selectedItem}
    programaId={programaId}
/>
<RechazarTareaDialog
    isOpen={dialogMode === 'rechazar'}
    onClose={handleClose}
    item={selectedItem}
    programaId={programaId}
/>
```

---

## 9. Directivas "use client"

Todos los nuevos componentes requieren `"use client"` al inicio del archivo:

| Archivo | Razon |
|---------|-------|
| `PromoProgramaPendientesTab.tsx` | Usa hooks de React y TanStack Query |
| `TareaPendienteCard.tsx` | Handlers de click, estado local |
| `ValidarTareaDialog.tsx` | Estado local, hooks de mutation |
| `RechazarTareaDialog.tsx` | Estado local, react-hook-form, hooks de mutation |

Los hooks (`use-tareas-pendientes.ts`, `use-validar-tarea.ts`, `use-rechazar-tarea.ts`) y el service (`tareas.service.ts`) son modulos puros de TypeScript; no llevan `"use client"` pero solo se importan desde Client Components.

La modificacion de `PromoProgramaDetailClient.tsx` ya tiene `"use client"` en la primera linea.

---

## 10. Archivos a Crear o Modificar

| Archivo | Accion | Descripcion |
|---------|--------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx` | MODIFICAR | Agregar tab "Pendientes" con badge, importar `PromoProgramaPendientesTab`, llamar `useTareasPendientes` para el count del badge |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaPendientesTab.tsx` | CREAR | Contenedor de la tab; gestiona paginacion y estado de dialogs |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/TareaPendienteCard.tsx` | CREAR | Card individual de completado pendiente con botones Validar/Rechazar |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/ValidarTareaDialog.tsx` | CREAR | Dialog de confirmacion de validacion con campo de comentario opcional |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/RechazarTareaDialog.tsx` | CREAR | Dialog de rechazo con campo de motivo obligatorio y validacion |
| `src/admin/src/hooks/use-tareas-pendientes.ts` | CREAR | Query hook para GET tareas-pendientes paginado |
| `src/admin/src/hooks/use-validar-tarea.ts` | CREAR | Mutation hook para PATCH validar |
| `src/admin/src/hooks/use-rechazar-tarea.ts` | CREAR | Mutation hook para PATCH rechazar |
| `src/admin/src/services/tareas.service.ts` | CREAR | Service con metodos getTareasPendientes, validarTarea, rechazarTarea |

---

## 11. Requisitos de Shared (Pre-requisitos antes de implementar Admin)

Los siguientes artefactos de `src/shared/` deben existir antes de implementar el Admin. Ver `plans/cp-tareas-promocion/shared/contracts-plan.md` para detalle completo.

| Artefacto | Archivo en shared |
|-----------|------------------|
| `TareaPendienteItem` interface | `src/shared/types/crowdpromotion.ts` |
| `TareasPendientesResponse` interface | `src/shared/types/crowdpromotion.ts` |
| `ValidarTareaRequest` interface | `src/shared/types/crowdpromotion.ts` |
| `ValidarTareaResponse` interface | `src/shared/types/crowdpromotion.ts` |
| `RechazarTareaRequest` interface | `src/shared/types/crowdpromotion.ts` |
| `RechazarTareaResponse` interface | `src/shared/types/crowdpromotion.ts` |
| `rechazarTareaSchema` + `RechazarTareaFormData` | `src/shared/schemas/crowdpromotion.schema.ts` |
| `validarTareaSchema` + `ValidarTareaFormData` | `src/shared/schemas/crowdpromotion.schema.ts` |
| `API_ROUTES.crowdpromotion.programas.tareasPendientes` | `src/shared/constants/index.ts` |
| `API_ROUTES.crowdpromotion.programas.validarTarea` | `src/shared/constants/index.ts` |
| `API_ROUTES.crowdpromotion.programas.rechazarTarea` | `src/shared/constants/index.ts` |
| `QUERY_KEYS.crowdpromotion.tareas.pendientes` | `src/shared/constants/index.ts` |
| `TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE` | `src/shared/constants/index.ts` |
| `getTareaPromocionErrorMessage` | `src/shared/utils/error-messages.ts` |

---

## 12. Checklist

- [ ] `PromoProgramaDetailClient.tsx` modificado con nueva tab "Pendientes" y badge con count
- [ ] `PromoProgramaPendientesTab.tsx` creado con estados: loading, error, empty, con-items
- [ ] `PromoProgramaPendientesTab.tsx` implementa controles de paginacion (anterior/siguiente)
- [ ] `TareaPendienteCard.tsx` muestra: nombre promotor, tipo, nombre tarea + ordinal, URL clickable, comentario, fecha relativa
- [ ] `TareaPendienteCard.tsx` botones Validar (verde) y Rechazar (rojo) con variantes correctas
- [ ] `ValidarTareaDialog.tsx` muestra datos en modo solo lectura
- [ ] `ValidarTareaDialog.tsx` campo comentario opcional con Textarea
- [ ] `ValidarTareaDialog.tsx` boton "Validar tarea" en verde con loading state
- [ ] `ValidarTareaDialog.tsx` toast dinamico con importe acreditado del response
- [ ] `RechazarTareaDialog.tsx` muestra datos en modo solo lectura
- [ ] `RechazarTareaDialog.tsx` bloque de aviso amarillo visible
- [ ] `RechazarTareaDialog.tsx` campo motivo obligatorio con React Hook Form + zodResolver
- [ ] `RechazarTareaDialog.tsx` contador de caracteres 0/500
- [ ] `RechazarTareaDialog.tsx` boton "Rechazar tarea" en rojo con loading state
- [ ] `use-tareas-pendientes.ts` implementa query con `enabled: !!programaId` y `staleTime: 0`
- [ ] `use-validar-tarea.ts` invalida query `tareas.pendientes` en `onSuccess`
- [ ] `use-rechazar-tarea.ts` invalida query `tareas.pendientes` en `onSuccess`
- [ ] `tareas.service.ts` usa clase instanciada siguiendo patron del proyecto
- [ ] `tareas.service.ts` verifica `response.messages` para errores y hace throw con mensaje legible
- [ ] `tareas.service.ts` construye correctamente las URLs con `API_ROUTES`
- [ ] Todos los componentes client tienen `"use client"` en primera linea
- [ ] Todos los tipos importados de `@shared/types`, no definidos localmente
- [ ] Dialogs bloquean cierre durante `isPending`
- [ ] `PromoProgramaPendientesTab.tsx` cierra dialogs y limpia `selectedItem` en `handleClose`
- [ ] Tests unitarios planeados para: `TareaPendienteCard`, `ValidarTareaDialog`, `RechazarTareaDialog`, `PromoProgramaPendientesTab` (siguiendo patron de `__tests__/` dentro de `components/`)
