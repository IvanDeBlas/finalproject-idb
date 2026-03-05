# Plan Frontend: Inscripcion en Programa (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/admin (Next.js 14 App Router)

---

## 1. Resumen

Esta feature extiende la pantalla de detalle de programa existente (`/dashboard/crowdpromotion/programas/[id]`) con tres nuevas pestanas de gestion de inscripciones. El artista podra ver solicitudes pendientes, aprobar/rechazar/bloquear promotores, ver la lista de aprobados con accion de baja, y consultar el registro de bloqueados.

- **Screens modificadas:** 1 (detalle de programa - se extienden los tabs existentes)
- **Componentes nuevos:** 11
- **Componentes modificados:** 2 (`PromoProgramaDetailClient`, `PromoProgramaKpiCards`)
- **Hooks nuevos:** 2
- **Services modificados:** 1 (`promo-programa.service.ts`)

### Alcance del Admin (US-CP-03)

El admin gestiona exclusivamente las pantallas 4, 5 y 6 del spec ui-ux:

| Pantalla | Descripcion | Implementacion |
|----------|-------------|----------------|
| Pantalla 4 | Tab "Solicitudes" - Aprobar/Rechazar/Bloquear | Tab nuevo en detalle programa |
| Pantalla 5 | Tab "Aprobados" - Lista con "Dar de baja" | Tab nuevo en detalle programa |
| Pantalla 6 | Tab "Bloqueados" - Solo lectura | Tab nuevo en detalle programa |

---

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/(dashboard)/crowdpromotion/programas/[id]/
│   ├── page.tsx                                          <- SIN CAMBIOS
│   └── components/
│       ├── PromoProgramaDetailClient.tsx                 <- MODIFICAR: agregar 3 tabs nuevos
│       ├── PromoProgramaKpiCards.tsx                     <- MODIFICAR: agregar KPI "Bloqueados"
│       │
│       │   ── Componentes nuevos: Solicitudes (Tab 4) ──
│       ├── PromoProgramaSolicitudesTab.tsx               <- NUEVO: contenedor del tab solicitudes
│       ├── SolicitudCard.tsx                             <- NUEVO: card de una solicitud pendiente
│       ├── AprobarInscripcionDialog.tsx                  <- NUEVO: no hay dialogo (accion directa con loading)
│       ├── RechazarInscripcionDialog.tsx                 <- NUEVO: AlertDialog de confirmacion
│       ├── BloquearInscripcionDialog.tsx                 <- NUEVO: AlertDialog destructivo
│       │
│       │   ── Componentes nuevos: Aprobados (Tab 5) ──
│       ├── PromoProgramaAprobadosTab.tsx                 <- NUEVO: contenedor del tab aprobados
│       ├── AprobadoCard.tsx                              <- NUEVO: card de promotor aprobado
│       ├── DarDeBajaDialog.tsx                           <- NUEVO: AlertDialog destructivo
│       │
│       │   ── Componentes nuevos: Bloqueados (Tab 6) ──
│       └── PromoProgramaBloqueadosTab.tsx                <- NUEVO: contenedor del tab bloqueados + BloqueadoCard inline
│
├── hooks/
│   ├── use-promo-programas.ts                            <- SIN CAMBIOS
│   ├── use-promo-programas-mutations.ts                  <- SIN CAMBIOS
│   ├── use-inscripciones.ts                              <- NUEVO: queries de inscripciones
│   ├── use-inscripciones-mutations.ts                    <- NUEVO: mutaciones aprobar/rechazar/bloquear/baja
│   └── index.ts                                          <- MODIFICAR: agregar exports nuevos
│
└── services/
    ├── promo-programa.service.ts                         <- SIN CAMBIOS
    ├── inscripcion.service.ts                            <- NUEVO: API calls de inscripciones
    └── index.ts                                          <- MODIFICAR: agregar export nuevo
```

---

## 3. Componentes

### 3.1 PromoProgramaDetailClient (MODIFICAR)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx`

**Cambio:** Agregar tres nuevas pestanas al `<Tabs>` existente y consumir `useInscripcionesResumen` para mostrar el contador de pendientes en el badge del tab "Solicitudes".

**Estado local adicional:**
- Ninguno nuevo (los dialogos de confirmacion se gestionan dentro de los componentes hijos)

**Nuevos tabs a agregar al `<TabsList>`:**

| Tab value | Trigger label | Badge |
|-----------|---------------|-------|
| `"solicitudes"` | `"Solicitudes"` con `<Badge>` contador si pendientes > 0 | Contador de pendientes (amber) |
| `"aprobados"` | `"Aprobados"` | Sin badge |
| `"bloqueados"` | `"Bloqueados"` | Sin badge |

**Cambio en KPI:** El componente `PromoProgramaKpiCards` recibe el mismo `resumen` de siempre. El KPI de "Bloqueados" se agrega en `PromoProgramaKpiCards` (ver 3.2).

**Nuevo `<TabsContent>` a agregar:**
- `value="solicitudes"`: renderiza `<PromoProgramaSolicitudesTab programaId={programaId} />`
- `value="aprobados"`: renderiza `<PromoProgramaAprobadosTab programaId={programaId} />`
- `value="bloqueados"`: renderiza `<PromoProgramaBloqueadosTab programaId={programaId} />`

**Nota de implementacion:** El numero de pendientes para el badge del tab "Solicitudes" se obtiene del campo `resumen.totalPromotoresPendientes` ya presente en `PromoProgramaResumen`. No se hace fetch adicional.

---

### 3.2 PromoProgramaKpiCards (MODIFICAR)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaKpiCards.tsx`

**Cambio:** Agregar un quinto KPI card "Promotores bloqueados" al array `kpis`. El tipo `PromoProgramaResumen` debe ser extendido con `totalPromotoresBloqueados: number` en shared.

**Nueva entrada en el array `kpis`:**

| key | label | icon | color | bgColor | getValue |
|-----|-------|------|-------|---------|----------|
| `"bloqueados"` | `"Promotores bloqueados"` | `Ban` | `text-red-400` | `bg-red-500/10` | `r.totalPromotoresBloqueados` |

**Nota:** El campo `totalPromotoresBloqueados` debe estar disponible en `PromoProgramaResumen` de shared. El plan shared (contracts-plan.md) ya documenta la existencia del resumen. Verificar si el campo necesita agregarse o ya existe como parte de US-CP-03. Si `PromoProgramaResumen` no lo tiene, este KPI puede derivarse de la query de inscripciones filtrada por estado "Bloqueado".

---

### 3.3 PromoProgramaSolicitudesTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaSolicitudesTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa para filtrar inscripciones |

**Estado local:**
- Ninguno (los dialogos se gestionan dentro de `SolicitudCard`)

**Dependencias:**
- Hooks: `useInscripcionesPendientes(programaId)`
- Componentes UI: `Card`, `Separator`, `Avatar`, `AvatarFallback`
- Componentes propios: `SolicitudCard`
- Iconos: `InboxIcon`

**Responsabilidad:**
Contenedor del tab "Solicitudes pendientes". Gestiona los estados de carga (skeleton), error y empty state. Renderiza la lista de `SolicitudCard` para cada inscripcion con estado "Pendiente". Muestra el titulo y subtitulo del tab con el contador de solicitudes.

**Estructura de UI:**
```
Cabecera: "Solicitudes pendientes (N)" + subtitulo
Loading: 3 skeletons de 120px
Empty: InboxIcon + "No hay solicitudes pendientes"
Error: Texto de error + boton "Reintentar"
Default: lista de <SolicitudCard>
```

---

### 3.4 SolicitudCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/SolicitudCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `inscripcion` | `InscripcionListItem` | Si | Datos de la inscripcion pendiente con perfil del promotor |
| `programaId` | `string` | Si | ID del programa, necesario para las mutaciones |
| `onAccionExitosa` | `() => void` | Si | Callback tras aprobar/rechazar/bloquear exitosamente para re-render |

**Estado local:**
- `showRechazarDialog: boolean` - controla visibilidad del `<AlertDialog>` de rechazo
- `showBloquearDialog: boolean` - controla visibilidad del `<AlertDialog>` de bloqueo
- `isAprobando: boolean` - estado de loading especifico del boton "Aprobar" (derivado de `aprobarMutation.isPending && aprobarMutation.variables?.inscripcionId === inscripcion.id`)

**Dependencias:**
- Hooks: `useAprobarInscripcion`, `useRechazarInscripcion`, `useBloquearInscripcion`
- Componentes UI: `Card`, `Avatar`, `AvatarFallback`, `Badge`, `Button`, `Separator`
- Componentes propios: `RechazarInscripcionDialog`, `BloquearInscripcionDialog`
- Iconos: `UserCheck`, `UserX`, `Ban`, `Instagram`, `Globe`, `AtSign`
- Utilidades: `formatRelativeDate` (fecha relativa tipo "hace 2 dias") de `@/lib/utils` o implementacion inline

**Responsabilidad:**
Renderiza la card de una solicitud pendiente con:
- Avatar con iniciales del promotor (2 primeras letras)
- Nombre, tipo, fecha relativa de solicitud
- Redes sociales del promotor (Instagram, TikTok, sitio web) como links
- Tres botones de accion: Aprobar (gradient), Rechazar (outline), Bloquear (outline destructivo)
- Los dialogos de confirmacion para rechazar y bloquear se abren desde esta card
- El boton "Aprobar" dispara la mutacion directamente (sin dialogo, con loading state)
- Animacion fade-out del item tras accion exitosa (clase CSS `opacity-0 transition-opacity duration-200`)

**Logica de fade-out:**
El componente gestiona un estado local `isHiding: boolean`. Cuando `onAccionExitosa` se llama, primero se activa `isHiding = true` (aplica `opacity-0`), y tras 200ms se llama al callback padre para que refetche/invalide la query. El padre `PromoProgramaSolicitudesTab` re-renderiza sin este item al invalidar la cache.

---

### 3.5 RechazarInscripcionDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/RechazarInscripcionDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad del dialogo |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback para cerrar el dialogo |
| `inscripcionId` | `string` | Si | ID de la inscripcion a rechazar |
| `programaId` | `string` | Si | ID del programa |
| `promotorNombre` | `string` | Si | Nombre del promotor para mostrar en el mensaje de confirmacion |
| `onExito` | `() => void` | Si | Callback ejecutado tras rechazo exitoso |

**Estado local:**
- Ninguno (el estado de loading se lee de `useRechazarInscripcion`)

**Dependencias:**
- Hooks: `useRechazarInscripcion`
- Componentes UI: `AlertDialog`, `AlertDialogAction`, `AlertDialogCancel`, `AlertDialogContent`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogHeader`, `AlertDialogTitle`

**Responsabilidad:**
Dialogo de confirmacion para rechazar una solicitud pendiente. Estilo neutro (no destructivo: el rechazo elimina el registro pero el promotor puede volver a solicitar). Deshabilita ambos botones durante el loading. Llama a `onExito` en caso de exito y cierra el dialogo.

---

### 3.6 BloquearInscripcionDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/BloquearInscripcionDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad del dialogo |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback para cerrar el dialogo |
| `inscripcionId` | `string` | Si | ID de la inscripcion a bloquear |
| `programaId` | `string` | Si | ID del programa |
| `promotorNombre` | `string` | Si | Nombre del promotor para el mensaje de confirmacion |
| `onExito` | `() => void` | Si | Callback ejecutado tras bloqueo exitoso |

**Estado local:**
- Ninguno

**Dependencias:**
- Hooks: `useBloquearInscripcion`
- Componentes UI: `AlertDialog` y subcomponentes

**Responsabilidad:**
Dialogo de confirmacion destructivo para bloquear un promotor. Estilo destructivo (fondo rojo oscuro en el boton de confirmar). Mensaje: "¿Seguro que deseas bloquear a {nombre}? No podra volver a solicitar inscripcion en este programa." Deshabilita botones durante loading.

---

### 3.7 PromoProgramaAprobadosTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaAprobadosTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa |

**Estado local:**
- Ninguno

**Dependencias:**
- Hooks: `useInscripcionesAprobadas(programaId)`
- Componentes UI: `Card`, `Avatar`, `AvatarFallback`, `Button`
- Componentes propios: `AprobadoCard`
- Iconos: `Users`

**Responsabilidad:**
Contenedor del tab "Promotores aprobados". Gestiona estados de carga (skeleton), error y empty state. Renderiza la lista de `AprobadoCard`. Muestra el titulo "Promotores aprobados (N)".

---

### 3.8 AprobadoCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/AprobadoCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `inscripcion` | `InscripcionListItem` | Si | Datos del promotor aprobado (estado "Aprobado") |
| `programaId` | `string` | Si | ID del programa para la mutacion de baja |

**Estado local:**
- `showDarDeBajaDialog: boolean` - controla visibilidad del dialogo de confirmacion

**Dependencias:**
- Hooks: ninguno directo (la mutacion se pasa via `DarDeBajaDialog`)
- Componentes UI: `Card`, `Avatar`, `AvatarFallback`, `Button`, `Badge`
- Componentes propios: `DarDeBajaDialog`
- Iconos: `UserMinus`, `Calendar`

**Responsabilidad:**
Renderiza la card de un promotor aprobado con:
- Avatar con iniciales
- Nombre, tipo, fecha de aprobacion
- Codigo referido en fuente monospace y color purple
- Estadisticas MVP: "Clics: 0 | Conv.: 0" (valores en 0 para MVP)
- Boton "Dar de baja" (outline destructivo) que abre `DarDeBajaDialog`
- Animacion fade-out tras baja exitosa (mismo patron que `SolicitudCard`)

---

### 3.9 DarDeBajaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/DarDeBajaDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback para cerrar |
| `inscripcionId` | `string` | Si | ID de la inscripcion a dar de baja |
| `programaId` | `string` | Si | ID del programa |
| `promotorNombre` | `string` | Si | Nombre del promotor para el mensaje |
| `onExito` | `() => void` | Si | Callback tras baja exitosa |

**Estado local:**
- Ninguno

**Dependencias:**
- Hooks: `useDarDeBajaInscripcion`
- Componentes UI: `AlertDialog` y subcomponentes

**Responsabilidad:**
Dialogo de confirmacion destructivo para dar de baja a un promotor aprobado. Mensaje: "¿Seguro que deseas dar de baja a {nombre}? Su codigo referido quedara desactivado." Boton confirmar con estilo destructivo (fondo rojo oscuro). Deshabilita botones durante loading.

---

### 3.10 PromoProgramaBloqueadosTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaBloqueadosTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa |

**Estado local:**
- Ninguno (tab de solo lectura)

**Dependencias:**
- Hooks: `useInscripcionesBloqueadas(programaId)`
- Componentes UI: `Card`, `Avatar`, `AvatarFallback`, `Badge`
- Iconos: `Ban`, `Calendar`, `ShieldCheck`, `Info`

**Responsabilidad:**
Contenedor del tab "Promotores bloqueados". Tab de solo lectura, sin botones de accion. Muestra:
- Aviso informativo azul: "Los promotores bloqueados no pueden re-solicitar inscripcion en este programa."
- Lista de cards de bloqueados (cada una inline sin componente separado por simplicidad)
- Cada card: avatar con fondo rojo/oscuro, nombre, tipo, badge "BLOQUEADO", fecha de bloqueo
- Estados: loading (skeleton), empty (ShieldCheck icon + mensaje), error

---

## 4. Hooks

### 4.1 useInscripcionesPendientes

**Archivo:** `src/admin/src/hooks/use-inscripciones.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa de promocion |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `InscripcionesListResponse \| undefined` | Lista paginada de inscripciones con estado "Pendiente" |
| `isLoading` | `boolean` | Estado de carga inicial |
| `isError` | `boolean` | Si hay error en la peticion |
| `refetch` | `function` | Refetch manual |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Pendiente' })`

**QueryFn:** `inscripcionService.getInscripciones(programaId, { estado: 'Pendiente' })`

**Opciones:** `staleTime: 0` (siempre refetch al enfocar el tab, ya que las solicitudes cambian frecuentemente)

---

### 4.2 useInscripcionesAprobadas

**Archivo:** `src/admin/src/hooks/use-inscripciones.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa de promocion |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `InscripcionesListResponse \| undefined` | Lista paginada de inscripciones con estado "Aprobado" |
| `isLoading` | `boolean` | Estado de carga |
| `isError` | `boolean` | Si hay error |
| `refetch` | `function` | Refetch manual |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Aprobado' })`

**QueryFn:** `inscripcionService.getInscripciones(programaId, { estado: 'Aprobado' })`

**Opciones:** `staleTime: 30_000`

---

### 4.3 useInscripcionesBloqueadas

**Archivo:** `src/admin/src/hooks/use-inscripciones.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa de promocion |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `InscripcionesListResponse \| undefined` | Lista de inscripciones con estado "Bloqueado" |
| `isLoading` | `boolean` | Estado de carga |
| `isError` | `boolean` | Si hay error |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Bloqueado' })`

**QueryFn:** `inscripcionService.getInscripciones(programaId, { estado: 'Bloqueado' })`

**Opciones:** `staleTime: 60_000` (datos mas estables, bloqueados no cambian con frecuencia)

---

### 4.4 useAprobarInscripcion

**Archivo:** `src/admin/src/hooks/use-inscripciones-mutations.ts`

**Tipo:** Mutation Hook

**Parametros de mutationFn:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `inscripcionId` | `string` | ID de la inscripcion a aprobar |

**Acciones:**
- `mutate({ programaId, inscripcionId })` - Ejecutar PATCH /aprobar
- `onSuccess`:
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Pendiente' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Aprobado' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.byId(programaId)` (para actualizar KPI cards)
  - `toast.success("{promotorNombre} ha sido aprobado. Se ha generado su codigo referido.")`
- `onError`:
  - `toast.error("No se pudo aprobar al promotor. Intentalo de nuevo.")`

**Retorna:** `UseMutationResult` con `isPending`, `mutate`, `variables`

---

### 4.5 useRechazarInscripcion

**Archivo:** `src/admin/src/hooks/use-inscripciones-mutations.ts`

**Tipo:** Mutation Hook

**Parametros de mutationFn:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `inscripcionId` | `string` | ID de la inscripcion a rechazar |

**Acciones:**
- `mutate({ programaId, inscripcionId })` - Ejecutar PATCH /rechazar
- `onSuccess`:
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Pendiente' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.byId(programaId)`
  - `toast("Solicitud de {promotorNombre} rechazada.")` (toast default, no destructivo)
- `onError`:
  - `toast.error("No se pudo rechazar la solicitud. Intentalo de nuevo.")`

---

### 4.6 useBloquearInscripcion

**Archivo:** `src/admin/src/hooks/use-inscripciones-mutations.ts`

**Tipo:** Mutation Hook

**Parametros de mutationFn:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `inscripcionId` | `string` | ID de la inscripcion a bloquear |

**Acciones:**
- `mutate({ programaId, inscripcionId })` - Ejecutar PATCH /bloquear
- `onSuccess`:
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Pendiente' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Bloqueado' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.byId(programaId)`
  - `toast("{promotorNombre} ha sido bloqueado en este programa.")` (toast default)
- `onError`:
  - `toast.error("No se pudo bloquear al promotor. Intentalo de nuevo.")`

---

### 4.7 useDarDeBajaInscripcion

**Archivo:** `src/admin/src/hooks/use-inscripciones-mutations.ts`

**Tipo:** Mutation Hook

**Parametros de mutationFn:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa |
| `inscripcionId` | `string` | ID de la inscripcion a dar de baja |

**Acciones:**
- `mutate({ programaId, inscripcionId })` - Ejecutar PATCH /dar-de-baja
- `onSuccess`:
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, { estado: 'Aprobado' })`
  - Invalidar `QUERY_KEYS.crowdpromotion.programas.byId(programaId)`
  - `toast("{promotorNombre} ha sido dado de baja. Su codigo referido ha sido desactivado.")` (toast default)
- `onError`:
  - `toast.error("No se pudo dar de baja al promotor. Intentalo de nuevo.")`

---

## 5. Services

### 5.1 inscripcionService (NUEVO)

**Archivo:** `src/admin/src/services/inscripcion.service.ts`

**Patron:** Clase singleton con metodos `async`, siguiendo exactamente el patron de `PromoProgramaService`.

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getInscripciones` | `programaId: string, filters?: InscripcionesFilters` | `Promise<InscripcionesListResponse>` | `GET /api/crowdpromotion/programas/{programaId}/inscripciones` |
| `aprobar` | `{ programaId: string, inscripcionId: string }` | `Promise<InscripcionAprobada>` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar` |
| `rechazar` | `{ programaId: string, inscripcionId: string }` | `Promise<InscripcionRechazada>` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/rechazar` |
| `bloquear` | `{ programaId: string, inscripcionId: string }` | `Promise<InscripcionBloqueada>` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/bloquear` |
| `darDeBaja` | `{ programaId: string, inscripcionId: string }` | `Promise<InscripcionDadaDeBaja>` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja` |

**Manejo de errores:** Mismo patron que `promoProgramaService`: leer `response.messages`, identificar el primer mensaje con `errorCode` que no empiece por `"0"`, y usar `getInscripcionErrorMessage(errorCode)` para construir el mensaje de error antes de throw.

**Importaciones necesarias:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- `getInscripcionErrorMessage` de `@shared/utils/error-messages`
- Types de `@shared/types`: `InscripcionesListResponse`, `InscripcionAprobada`, `InscripcionRechazada`, `InscripcionBloqueada`, `InscripcionDadaDeBaja`, `InscripcionesFilters`, `ServiceResponse`

**Detalle de `getInscripciones`:**
Construye los query params a partir de `filters` usando `URLSearchParams`. Si `filters.estado` existe, lo agrega. Si `filters.page` existe, lo agrega. Si `filters.pageSize` existe, lo agrega. El endpoint base es `API_ROUTES.crowdpromotion.programas.inscripciones(programaId)`.

**Detalle de los metodos PATCH:**
Todos los metodos de accion (aprobar, rechazar, bloquear, darDeBaja) llaman a `apiFetch` con `method: "PATCH"` y sin body (`data` no se pasa). El endpoint lo construye usando las funciones de `API_ROUTES.crowdpromotion.programas.aprobar(programaId, inscripcionId)`, etc.

---

## 6. Flujo de Datos

### Flujo: Artista aprueba una solicitud

```
Usuario (Artista)
    |
    | click "Aprobar"
    v
SolicitudCard
    | llama aprobarMutation.mutate({ programaId, inscripcionId })
    v
useAprobarInscripcion (useMutation)
    | mutationFn
    v
inscripcionService.aprobar({ programaId, inscripcionId })
    | apiFetch PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar
    v
Backend (WePlay API)
    | Response: InscripcionAprobada con codigoReferido generado
    v
useAprobarInscripcion.onSuccess
    | invalidate QUERY_KEYS.programas.inscripciones(programaId, { estado: 'Pendiente' })
    | invalidate QUERY_KEYS.programas.inscripciones(programaId, { estado: 'Aprobado' })
    | invalidate QUERY_KEYS.programas.byId(programaId)
    | toast.success(...)
    v
TanStack Query refetch automatico
    | PromoProgramaSolicitudesTab re-fetch -> item desaparece
    | PromoProgramaAprobadosTab re-fetch -> item aparece
    | PromoProgramaKpiCards re-render -> contadores actualizados
```

### Flujo: Artista da de baja a un promotor aprobado

```
Usuario (Artista)
    |
    | click "Dar de baja" -> abre DarDeBajaDialog
    | click "Confirmar"
    v
DarDeBajaDialog
    | llama darDeBajaMutation.mutate({ programaId, inscripcionId })
    v
useDarDeBajaInscripcion (useMutation)
    | mutationFn
    v
inscripcionService.darDeBaja({ programaId, inscripcionId })
    | apiFetch PATCH .../dar-de-baja
    v
Backend
    | Response: InscripcionDadaDeBaja con fechaBaja
    v
useDarDeBajaInscripcion.onSuccess
    | invalidate QUERY_KEYS.programas.inscripciones(programaId, { estado: 'Aprobado' })
    | invalidate QUERY_KEYS.programas.byId(programaId)
    | onExito() -> cierra dialogo, activa fade-out en AprobadoCard
    | toast(...)
```

### Diagrama general del flujo de datos

```
Artista (Browser)
    |
    v
PromoProgramaDetailClient (Client Component)
    |-- usePromoPrograma(programaId) -> PromoProgramaKpiCards (KPIs actualizados)
    |
    |-- Tab "Solicitudes"
    |       v
    |   PromoProgramaSolicitudesTab
    |       |-- useInscripcionesPendientes(programaId)
    |       |       v
    |       |   inscripcionService.getInscripciones(programaId, { estado: 'Pendiente' })
    |       |       v
    |       |   GET /api/crowdpromotion/programas/{id}/inscripciones?estado=Pendiente
    |       |
    |       |-- SolicitudCard[]
    |               |-- useAprobarInscripcion -> PATCH .../aprobar
    |               |-- useRechazarInscripcion -> PATCH .../rechazar
    |               |-- useBloquearInscripcion -> PATCH .../bloquear
    |
    |-- Tab "Aprobados"
    |       v
    |   PromoProgramaAprobadosTab
    |       |-- useInscripcionesAprobadas(programaId)
    |       |       v
    |       |   GET .../inscripciones?estado=Aprobado
    |       |
    |       |-- AprobadoCard[]
    |               |-- useDarDeBajaInscripcion -> PATCH .../dar-de-baja
    |
    |-- Tab "Bloqueados"
            v
        PromoProgramaBloqueadosTab
            |-- useInscripcionesBloqueadas(programaId)
                    v
                GET .../inscripciones?estado=Bloqueado
```

---

## 7. Dependencias de Shared

**Importar de `@shared/types`:**
- `InscripcionListItem` - tipo de cada item en las listas de solicitudes/aprobados/bloqueados
- `InscripcionesListResponse` - respuesta paginada de GET /inscripciones
- `InscripcionAprobada` - respuesta de PATCH /aprobar
- `InscripcionRechazada` - respuesta de PATCH /rechazar
- `InscripcionBloqueada` - respuesta de PATCH /bloquear
- `InscripcionDadaDeBaja` - respuesta de PATCH /dar-de-baja
- `InscripcionesFilters` - query params para GET /inscripciones
- `InscripcionEstado` - union type `'Pendiente' | 'Aprobado' | 'Bloqueado' | 'DadoDeBaja'`
- `ServiceResponse` - wrapper de respuestas del backend (ya existente)

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdpromotion.programas.inscripciones` - nuevo key para las listas de inscripciones
- `QUERY_KEYS.crowdpromotion.programas.byId` - ya existente, para invalidacion
- `API_ROUTES.crowdpromotion.programas.inscripciones` - nuevo endpoint GET
- `API_ROUTES.crowdpromotion.programas.aprobar` - nuevo endpoint PATCH
- `API_ROUTES.crowdpromotion.programas.rechazar` - nuevo endpoint PATCH
- `API_ROUTES.crowdpromotion.programas.bloquear` - nuevo endpoint PATCH
- `API_ROUTES.crowdpromotion.programas.darDeBaja` - nuevo endpoint PATCH
- `INSCRIPCION_ESTADO` - constantes de estado para comparacion tipada

**Importar de `@shared/utils/error-messages`:**
- `getInscripcionErrorMessage` - getter de mensajes de error de inscripciones

**Nota sobre `PromoProgramaResumen` en shared:** El plan shared no menciona explicitamente el campo `totalPromotoresBloqueados` en `PromoProgramaResumen`. Si el backend no lo devuelve en el endpoint de detalle de programa, el KPI de bloqueados en `PromoProgramaKpiCards` puede derivarse del `data.totalCount` del hook `useInscripcionesBloqueadas`. En ese caso, `PromoProgramaKpiCards` necesitaria recibir ese valor como prop adicional desde `PromoProgramaDetailClient`.

---

## 8. Cambios a Archivos Existentes

### 8.1 PromoProgramaDetailClient.tsx (MODIFICAR)

**Tipo de cambio:** Agregar imports, agregar tabs al `<TabsList>` y agregar tres `<TabsContent>` nuevos.

**Cambios especificos:**
1. Importar `PromoProgramaSolicitudesTab`, `PromoProgramaAprobadosTab`, `PromoProgramaBloqueadosTab`
2. En `<TabsList>`: agregar tres `<TabsTrigger>` nuevos (solicitudes con badge de pendientes, aprobados, bloqueados)
3. Agregar tres `<TabsContent>` correspondientes
4. El badge de solicitudes pendientes usa `programa.resumen.totalPromotoresPendientes` (ya disponible)

**Badge del tab solicitudes:**
```
value="solicitudes"
Label: "Solicitudes"
Badge: si programa.resumen.totalPromotoresPendientes > 0, mostrar
  <span className="ml-1.5 flex h-5 w-5 items-center justify-center rounded-full bg-amber-500 text-xs font-bold text-black">
    {programa.resumen.totalPromotoresPendientes}
  </span>
```

### 8.2 PromoProgramaKpiCards.tsx (MODIFICAR)

**Tipo de cambio:** Agregar KPI de bloqueados al array de KPIs.

**Prerequisito:** El tipo `PromoProgramaResumen` en `@shared/types` debe incluir `totalPromotoresBloqueados: number`. Si no existe, este cambio se posterga hasta que shared lo exponga.

### 8.3 hooks/index.ts (MODIFICAR)

Agregar al final:
```
export * from "./use-inscripciones"
export * from "./use-inscripciones-mutations"
```

### 8.4 services/index.ts (MODIFICAR)

Agregar al final:
```
export { inscripcionService } from "./inscripcion.service"
```

---

## 9. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaSolicitudesTab.tsx` | Component | Tab de solicitudes pendientes con lista de SolicitudCard |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/SolicitudCard.tsx` | Component | Card de una solicitud pendiente con botones de accion |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/RechazarInscripcionDialog.tsx` | Component | AlertDialog de confirmacion para rechazar |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/BloquearInscripcionDialog.tsx` | Component | AlertDialog destructivo para bloquear |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaAprobadosTab.tsx` | Component | Tab de promotores aprobados con lista de AprobadoCard |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/AprobadoCard.tsx` | Component | Card de promotor aprobado con accion "Dar de baja" |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/DarDeBajaDialog.tsx` | Component | AlertDialog destructivo para dar de baja |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaBloqueadosTab.tsx` | Component | Tab de bloqueados, solo lectura |
| `src/admin/src/hooks/use-inscripciones.ts` | Hook | Queries para listas de inscripciones por estado |
| `src/admin/src/hooks/use-inscripciones-mutations.ts` | Hook | Mutaciones de aprobar/rechazar/bloquear/dar-de-baja |
| `src/admin/src/services/inscripcion.service.ts` | Service | API calls para inscripciones |

---

## 10. Estados de UI Detallados

### Tab Solicitudes

| Estado | UI |
|--------|----|
| Loading | 3 skeletons `bg-[#1e1e38] rounded-xl h-[120px] animate-pulse mb-3` |
| Empty | `<InboxIcon className="h-10 w-10 text-zinc-400">` + "No hay solicitudes pendientes" en `text-sm text-zinc-400` centrado |
| Error | Texto de error + `<Button variant="outline" onClick={refetch}>Reintentar</Button>` |
| Default | Lista de `<SolicitudCard>` |
| Aprobando | Boton "Aprobar" del item activo: `<Loader2 className="h-3.5 w-3.5 animate-spin">` + "Aprobando..." deshabilitado. Otros botones del mismo item tambien deshabilitados (`disabled={aprobarMutation.isPending}`) |
| Rechazando/Bloqueando | Loading en boton de confirmar del dialogo. Item desaparece con fade-out 200ms tras exito |

### Tab Aprobados

| Estado | UI |
|--------|----|
| Loading | 3 skeletons `bg-[#1e1e38] rounded-xl h-[100px] animate-pulse mb-3` |
| Empty | `<Users className="h-10 w-10 text-zinc-400">` + "No hay promotores aprobados en este programa" |
| Error | Texto + boton reintentar |
| Default | Lista de `<AprobadoCard>` |
| Dando de baja | Boton confirmar del `DarDeBajaDialog` con loading. Item desaparece con fade-out tras exito |

### Tab Bloqueados

| Estado | UI |
|--------|----|
| Loading | 2 skeletons |
| Empty | `<ShieldCheck className="h-10 w-10 text-zinc-400">` + "No hay promotores bloqueados" |
| Default | Aviso informativo azul + lista de cards de solo lectura |

---

## 11. Dialogos de Confirmacion

### RechazarInscripcionDialog

- **Trigger:** Click en boton "Rechazar" en `SolicitudCard`
- **Componente base:** `<AlertDialog>` de shadcn/ui
- **Estilo:** Neutro (no destructivo). El rechazo elimina el registro pero el promotor puede re-solicitar.
- **Titulo:** "Rechazar solicitud"
- **Descripcion:** "¿Seguro que deseas rechazar la solicitud de {promotorNombre}? Esta accion no se puede deshacer."
- **Boton cancelar:** `<AlertDialogCancel>` con `border-zinc-800 text-zinc-400 hover:bg-[#1e1e38] hover:text-white`
- **Boton confirmar:** `<AlertDialogAction>` con `border-zinc-800 text-white hover:bg-[#1e1e38]` (estilo neutro)
- **Loading:** boton confirmar deshabilitado + texto "Rechazando..."

### BloquearInscripcionDialog

- **Trigger:** Click en boton "Bloquear" en `SolicitudCard`
- **Estilo:** Destructivo. El bloqueo es permanente para ese programa.
- **Titulo:** "Bloquear promotor"
- **Descripcion:** "¿Seguro que deseas bloquear a {promotorNombre}? No podra volver a solicitar inscripcion en este programa."
- **Boton confirmar:** `bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200`
- **Loading:** boton confirmar deshabilitado + texto "Bloqueando..."

### DarDeBajaDialog

- **Trigger:** Click en boton "Dar de baja" en `AprobadoCard`
- **Estilo:** Destructivo. La baja desactiva el codigo referido.
- **Titulo:** "Dar de baja al promotor"
- **Descripcion:** "¿Seguro que deseas dar de baja a {promotorNombre}? Su codigo referido quedara desactivado."
- **Boton confirmar:** Mismo estilo destructivo que `BloquearInscripcionDialog`
- **Loading:** "Dando de baja..."

---

## 12. Invalidacion de Cache

| Mutacion | Query Keys a Invalidar |
|----------|------------------------|
| Aprobar inscripcion | `inscripciones(programaId, { estado: 'Pendiente' })`, `inscripciones(programaId, { estado: 'Aprobado' })`, `programas.byId(programaId)` |
| Rechazar inscripcion | `inscripciones(programaId, { estado: 'Pendiente' })`, `programas.byId(programaId)` |
| Bloquear inscripcion | `inscripciones(programaId, { estado: 'Pendiente' })`, `inscripciones(programaId, { estado: 'Bloqueado' })`, `programas.byId(programaId)` |
| Dar de baja | `inscripciones(programaId, { estado: 'Aprobado' })`, `programas.byId(programaId)` |

**Nota:** La invalidacion de `programas.byId(programaId)` fuerza el refetch del resumen del programa y actualiza los KPI cards. Esto asegura que los contadores de "Promotores aprobados" y "Promotores pendientes" se actualicen automaticamente tras cada accion.

---

## 13. Checklist

- [ ] `PromoProgramaDetailClient` modificado con 3 nuevos tabs y badge en "Solicitudes"
- [ ] `PromoProgramaKpiCards` modificado con KPI "Bloqueados" (si `PromoProgramaResumen` lo expone)
- [ ] `PromoProgramaSolicitudesTab` creado con estados loading/empty/error/default
- [ ] `SolicitudCard` creado con perfil del promotor (redes sociales como links), botones de accion y fade-out
- [ ] `RechazarInscripcionDialog` creado con estilo neutro
- [ ] `BloquearInscripcionDialog` creado con estilo destructivo
- [ ] `PromoProgramaAprobadosTab` creado con estados loading/empty/error/default
- [ ] `AprobadoCard` creado con codigo referido, stats MVP y boton "Dar de baja"
- [ ] `DarDeBajaDialog` creado con estilo destructivo
- [ ] `PromoProgramaBloqueadosTab` creado (solo lectura) con aviso informativo
- [ ] `use-inscripciones.ts` creado con 3 query hooks (pendientes/aprobadas/bloqueadas)
- [ ] `use-inscripciones-mutations.ts` creado con 4 mutation hooks
- [ ] `inscripcion.service.ts` creado con 5 metodos alineados con los endpoints del contrato
- [ ] `hooks/index.ts` actualizado con nuevos exports
- [ ] `services/index.ts` actualizado con nuevo export
- [ ] Types importados de `@shared/types` (no duplicados en admin)
- [ ] Constants importadas de `@shared/constants` (QUERY_KEYS y API_ROUTES)
- [ ] Error messages usando `getInscripcionErrorMessage` de `@shared/utils/error-messages`
- [ ] Todos los componentes usan shadcn/ui (Card, Badge, Button, AlertDialog, Avatar, Separator, Tabs)
- [ ] Hooks siguen patron `useMutation` / `useQuery` de TanStack Query
- [ ] Dialogos usan `<AlertDialog>` de shadcn (no modales custom)
- [ ] Toast messages alineados con los mensajes definidos en ui-ux.md
- [ ] Estados de loading, empty y error implementados en los 3 tabs contenedores
- [ ] Animacion fade-out de items tras acciones exitosas
- [ ] Boton "Aprobar" tiene loading state inline (sin dialogo)
- [ ] Botones de accion deshabilitados durante mutaciones en curso
- [ ] Responsive: tabs scrollables horizontalmente en mobile via `<ScrollArea horizontal>`
