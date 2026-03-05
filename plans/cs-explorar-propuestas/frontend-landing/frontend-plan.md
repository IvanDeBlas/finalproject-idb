# Plan Frontend: cs-explorar-propuestas (Landing)

**Fecha:** 2026-02-17
**Feature:** cs-explorar-propuestas (US-CS-03)
**Target:** src/web (Vite + React 18 + TypeScript + Tailwind + shadcn/ui)

---

## 1. Resumen

- Screens: 3 pages + 2 dialogs modales
- Componentes: 14
- Hooks: 5
- Services/APIs: 2

### Pantallas

| ID | Ruta | Componente | Auth |
|----|------|------------|------|
| P1 | `/crowdsourcing/necesidades` | `ExplorarNecesidadesPage` | JWT requerido |
| P2 | `/crowdsourcing/necesidades/:id` | `NecesidadDetallePage` | JWT requerido |
| P3 | `/crowdsourcing/mis-propuestas` | `MisPropuestasPage` | JWT requerido |
| D1 | Modal sobre P2 | `EnviarPropuestaForm` | JWT + PerfilProfesional |
| D2 | Modal sobre P3 | `RetirarPropuestaDialog` | JWT, propietario propuesta |

---

## 2. Estructura de Carpetas

La feature se integra en el feature `crowdsourcing` existente bajo `src/web/src/features/crowdsourcing/`. Se agregan sub-carpetas de dominio especificas dentro de la arquitectura hexagonal ya establecida.

```
src/web/src/features/crowdsourcing/
├── domain/
│   ├── types.ts                          MODIFICAR - agregar re-exports de nuevos types
│   └── index.ts                          MODIFICAR - re-exportar nuevos types
│
├── application/
│   ├── hooks/
│   │   ├── useTemplates.ts               EXISTENTE - sin cambios
│   │   ├── useTemplateDetail.ts          EXISTENTE - sin cambios
│   │   ├── useGenerarNecesidades.ts      EXISTENTE - sin cambios
│   │   ├── useWizardState.ts             EXISTENTE - sin cambios
│   │   ├── useRolesProfesionales.ts      EXISTENTE - sin cambios
│   │   ├── useNecesidadesPublicas.ts     NUEVO
│   │   ├── useNecesidadPublica.ts        NUEVO
│   │   ├── useCreatePropuesta.ts         NUEVO
│   │   ├── useMisPropuestas.ts           NUEVO
│   │   └── useRetirarPropuesta.ts        NUEVO
│   ├── schemas.ts                        EXISTENTE - sin cambios (schemas en @shared)
│   └── index.ts                          MODIFICAR - exportar nuevos hooks
│
├── infrastructure/
│   ├── api/
│   │   ├── crowdsourcing.api.ts          EXISTENTE - sin cambios (wizard)
│   │   ├── necesidadPublica.api.ts       NUEVO - API calls necesidades publicas
│   │   └── propuesta.api.ts              NUEVO - API calls propuestas profesional
│   └── index.ts                          MODIFICAR - exportar nuevas clases API
│
└── presentation/
    ├── components/
    │   ├── WizardStepper.tsx             EXISTENTE - sin cambios
    │   ├── TemplateCard.tsx              EXISTENTE - sin cambios
    │   ├── (... otros existentes ...)
    │   ├── NecesidadCard.tsx             NUEVO
    │   ├── NecesidadCardSkeleton.tsx     NUEVO
    │   ├── NecesidadFilters.tsx          NUEVO
    │   ├── PropuestaCard.tsx             NUEVO
    │   ├── PropuestaCardSkeleton.tsx     NUEVO
    │   ├── EstadoPropuestaBadge.tsx      NUEVO
    │   ├── UrgenciaBadge.tsx             NUEVO
    │   ├── EnviarPropuestaForm.tsx       NUEVO
    │   ├── RetirarPropuestaDialog.tsx    NUEVO
    │   ├── EmptyStateNecesidades.tsx     NUEVO
    │   ├── EmptyStatePropuestas.tsx      NUEVO
    │   ├── PerfilProfesionalCTA.tsx      NUEVO
    │   └── index.ts                      MODIFICAR - exportar nuevos componentes
    ├── pages/
    │   ├── NuevoProyectoPage.tsx         EXISTENTE - sin cambios
    │   ├── ExplorarNecesidadesPage.tsx   NUEVO
    │   ├── NecesidadDetallePage.tsx      NUEVO
    │   ├── MisPropuestasPage.tsx         NUEVO
    │   └── index.ts                      MODIFICAR - exportar nuevas pages
    └── index.ts                          sin cambios
```

### Modificaciones en shared (ya planificadas en contracts-plan.md)

```
src/shared/
├── types/crowdsourcing.ts        MODIFICAR - agregar 6 interfaces + 2 union types
├── schemas/crowdsourcing.schema.ts MODIFICAR - agregar createPropuestaSchema, filterNecesidadesSchema
├── constants/index.ts            MODIFICAR - agregar ESTADO_PROPUESTA, QUERY_KEYS, API_ROUTES, APP_ROUTES
└── utils/error-messages.ts       MODIFICAR - agregar PROPUESTA_ERROR_MESSAGES, getPropuestaErrorMessage
```

### Modificacion en router

```
src/web/src/app/router.tsx        MODIFICAR - agregar 3 rutas protegidas
```

---

## 3. Componentes

### 3.1 ExplorarNecesidadesPage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/ExplorarNecesidadesPage.tsx`

**Props:** Ninguna (es una page, recibe datos via hooks y router)

**Estado Local:**
- `searchParams` / `setSearchParams` (React Router) - sincroniza filtros con URL
- `searchInput: string` - valor en tiempo real del input de busqueda (antes del debounce)
- `presupuestoMinInput: string` - valor en tiempo real (antes del debounce)
- `presupuestoMaxInput: string` - valor en tiempo real (antes del debounce)
- `ciudadInput: string` - valor en tiempo real (antes del debounce)

**Dependencias:**
- Hooks: `useNecesidadesPublicas`, `useDebounce` (hook utilitario local o de shared)
- Componentes UI: `NecesidadCard`, `NecesidadCardSkeleton`, `NecesidadFilters`, `EmptyStateNecesidades`, `UrgenciaBadge`
- shadcn: `Pagination`, `Select`, `Input`, `Button`
- React Router: `useSearchParams`

**Responsabilidad:**
Pantalla principal de exploración de necesidades abiertas. Gestiona el estado de filtros sincronizando con URL via `useSearchParams`. Aplica debounce de 300ms a la búsqueda por texto y 500ms a los inputs de presupuesto y ciudad. Muestra la grilla de NecesidadCard (2 cols en md+, 1 col mobile), paginación, y el sidebar de filtros colapsable en mobile. Delega todo el render de filtros a `NecesidadFilters` y el render de cada card a `NecesidadCard`.

**Logica de filtros desde URL:**
Leer de `searchParams`: `search`, `tipo`, `modalidad`, `presupuestoMin`, `presupuestoMax`, `pais`, `ciudad`, `orderBy`, `page`.
Escribir a `searchParams` al cambiar cualquier filtro, reseteando `page` a 1 si cambia un filtro (no la paginacion).

---

### 3.2 NecesidadDetallePage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/NecesidadDetallePage.tsx`

**Props:** Ninguna (usa `useParams` para leer `:id`)

**Estado Local:**
- `isFormOpen: boolean` - controla apertura del dialog `EnviarPropuestaForm`

**Dependencias:**
- Hooks: `useNecesidadPublica`
- Componentes propios: `EnviarPropuestaForm`, `UrgenciaBadge`, `PerfilProfesionalCTA`
- shadcn: `Card`, `CardContent`, `CardHeader`, `Badge`, `Button`, `Avatar`, `AvatarFallback`, `AvatarImage`, `Skeleton`
- Lucide: `ArrowLeft`, `Wifi`, `MapPin`, `GitBranch`, `Clock`, `Users`, `CheckCircle`, `Info`
- React Router: `useParams`, `Link`

**Responsabilidad:**
Muestra el detalle completo de una necesidad pública. Implementa layout de 2 columnas en desktop (contenido principal + sidebar de acción) y 1 columna en mobile con CTA sticky bottom. El CTA del sidebar es dinámico segun `yaPropuso`, `esPropietario` y `tienePerfilProfesional` retornados por el backend. Gestiona la apertura del dialog `EnviarPropuestaForm`. Maneja el estado de loading con skeletons y el estado de error 404.

**Logica del CTA (sidebar accion):**
```
si isLoading     -> skeleton
si error (404)   -> pantalla de error con link volver
si data.esPropietario      -> bloque "Esta es tu necesidad"
si data.yaPropuso          -> bloque verde "Ya enviaste una propuesta"
si !data.tienePerfilProfesional -> <PerfilProfesionalCTA />
default                    -> <Button> "Enviar propuesta" que setea isFormOpen=true
```

---

### 3.3 MisPropuestasPage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/MisPropuestasPage.tsx`

**Props:** Ninguna (es una page protegida)

**Estado Local:**
- `searchParams` / `setSearchParams` (React Router) - sincroniza filtro de estado con URL `?estado=N`
- `propuestaARetirar: MiPropuestaList | null` - propuesta seleccionada para retirar (controla el dialog)

**Dependencias:**
- Hooks: `useMisPropuestas`, `useRetirarPropuesta`
- Componentes propios: `PropuestaCard`, `PropuestaCardSkeleton`, `EstadoPropuestaBadge`, `RetirarPropuestaDialog`, `EmptyStatePropuestas`
- shadcn: `Pagination`, `Button`
- React Router: `useSearchParams`

**Responsabilidad:**
Lista paginada de propuestas enviadas por el usuario autenticado. Incluye chips de filtro de estado (`Todas / Pendiente / Aceptada / Rechazada / Retirada`) sincronizados con URL. Gestiona el estado de la dialog de confirmación `RetirarPropuestaDialog` guardando la propuesta seleccionada en `propuestaARetirar`. Muestra el empty state correspondiente (sin propuestas en general, o sin propuestas del estado filtrado).

---

### 3.4 NecesidadCard

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/NecesidadCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `necesidad` | `NecesidadPublicaList` | Si | DTO con todos los campos del listado publico |
| `onClick` | `() => void` | No | Callback de click, navega al detalle |
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes propios: `UrgenciaBadge`
- shadcn: `Card`, `Badge`
- Lucide: `Wifi`, `MapPin`, `GitBranch`, `Clock`, `Users`, `Calendar`
- Utilitarios: `formatCurrency` de `@shared/utils`, `formatDistanceToNow` de `date-fns` o funcion propia

**Responsabilidad:**
Card clickeable del listado de necesidades. Muestra: badge de tipo de necesidad (estilo purple), badge de modalidad con icono Lucide correspondiente (Wifi/MapPin/GitBranch), badge de urgencia condicionado a `esUrgente`, titulo, descripcion truncada, nombre de artista, rango de presupuesto, fecha relativa, contador de propuestas, y fecha límite con color semantico segun días restantes (calculado localmente con `URGENCIA_DIAS_UMBRAL`). Implementa hover effect con transicion (bg + border + scale(1.01) + shadow). Accesible: `role="article"`, `tabIndex={0}`, `aria-label` descriptivo, soporte Enter/Space.

**Calculo de color fecha limite (local):**
```
diasRestantes < 3  -> text-red-400
diasRestantes < 7  -> text-amber-400
default            -> text-[#94a3b8]
```

---

### 3.5 NecesidadCardSkeleton

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/NecesidadCardSkeleton.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Card`, `Skeleton`

**Responsabilidad:**
Versión skeleton con shimmer del NecesidadCard. Replica la estructura visual con placeholders: 2 badges de ancho fijo, línea de título, 2 líneas de descripción, fila de stats. Usado en el estado de carga del listado (6 instancias). Accesible: contenedor padre con `role="status" aria-label="Cargando necesidades..."`.

---

### 3.6 NecesidadFilters

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/NecesidadFilters.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `filters` | `NecesidadesPublicasFilter` | Si | Estado actual de los filtros (leido de URL) |
| `onFilterChange` | `(partial: Partial<NecesidadesPublicasFilter>) => void` | Si | Callback para actualizar un filtro parcial |
| `onClear` | `() => void` | Si | Resetea todos los filtros |
| `tiposNecesidad` | `Array<{ id: number; nombre: string }>` | Si | Maestras de tipo para renderizar chips |
| `isMobile` | `boolean` | No | Si true, renderiza en modo accordion colapsable |

**Estado Local:**
- `isOpen: boolean` - para el accordion mobile (si `isMobile=true`)

**Dependencias:**
- shadcn: `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`, `Input`, `Button`, `Collapsible`, `CollapsibleContent`, `CollapsibleTrigger`
- Lucide: `SlidersHorizontal`, `X`
- Constantes: `ORDER_BY_NECESIDADES_LABELS` de `@shared/constants`

**Responsabilidad:**
Panel de filtros. Contiene: chips multi-select de tipo de necesidad (toggle individual, `aria-pressed`), select de modalidad, inputs de presupuesto min/max (el debounce se maneja en el padre `ExplorarNecesidadesPage`), select de país, input de ciudad (visible solo si hay país seleccionado), botón "Limpiar filtros". En mobile (`isMobile=true`), todo se envuelve en un `Collapsible` de shadcn con botón de toggle "[Filtros]". En desktop, se renderiza directamente como sidebar sticky.

**Nota:** El componente NO maneja debounce interno; delega los valores en tiempo real al padre, que aplica debounce antes de actualizar la URL.

---

### 3.7 PropuestaCard

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/PropuestaCard.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `propuesta` | `MiPropuestaList` | Si | DTO de una propuesta del profesional |
| `onRetirar` | `(propuesta: MiPropuestaList) => void` | No | Callback al click en "Retirar propuesta" |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes propios: `EstadoPropuestaBadge`
- shadcn: `Card`, `CardContent`, `Button`
- Lucide: `ArrowRight`
- Utilitarios: `formatDate` de `@shared/utils`
- React Router: `Link` (para titulo de necesidad y boton "Ver acuerdo")
- Constantes: `ESTADO_PROPUESTA` de `@shared/constants`

**Responsabilidad:**
Card de una propuesta en la pantalla "Mis Propuestas". Muestra: titulo de la necesidad como link a `/crowdsourcing/necesidades/{necesidadId}` (nota: el `necesidadId` no está en `MiPropuestaList`; se navega al listado o se omite si no disponible), nombre del artista, precio + moneda, badge de estado via `EstadoPropuestaBadge`, fecha de envio, fecha de respuesta (si existe). La acción condicional por estado:
- `estadoPropuestaId === ESTADO_PROPUESTA.PENDIENTE` -> botón "Retirar propuesta" (outline, rojo) que llama `onRetirar(propuesta)`
- `estadoPropuestaId === ESTADO_PROPUESTA.ACEPTADA && acuerdoId` -> botón "Ver acuerdo" (gradiente) con icono ArrowRight, navega a `/crowdsourcing/acuerdos/{acuerdoId}`
- otros estados -> solo lectura, sin acciones

---

### 3.8 PropuestaCardSkeleton

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/PropuestaCardSkeleton.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Card`, `Skeleton`

**Responsabilidad:**
Versión skeleton de `PropuestaCard`. Replica la estructura: línea de título + badge de estado a la derecha, línea de artista, línea de precio + fecha, botón de acción. Usado en el estado de carga de MisPropuestasPage (4 instancias).

---

### 3.9 EstadoPropuestaBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EstadoPropuestaBadge.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoId` | `number` | Si | ID numerico del estado (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) |
| `estadoNombre` | `string` | Si | Nombre legible del estado (para mostrar y para aria-label) |
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Badge`
- Constantes: `ESTADO_PROPUESTA` de `@shared/constants`

**Responsabilidad:**
Badge con color semantico según el estado de la propuesta. Mapeo de estilos:
- PENDIENTE (1): `bg-amber-900/30 border-amber-600 text-amber-300`
- ACEPTADA (2): `bg-green-900/30 border-green-600 text-green-300`
- RECHAZADA (3): `bg-red-900/30 border-red-600 text-red-300`
- RETIRADA (4): `bg-gray-900/30 border-gray-600 text-gray-400`

Accesible: `aria-label="Estado: {estadoNombre}"`.

---

### 3.10 UrgenciaBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/UrgenciaBadge.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `fechaLimite` | `string` | Si | ISO date string de fecha limite |
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Badge`
- Lucide: `Clock`
- Constantes: `URGENCIA_DIAS_UMBRAL` de `@shared/constants`

**Responsabilidad:**
Badge de urgencia que se muestra solo cuando los días restantes hasta `fechaLimite` son menores al umbral (3 dias). Calcula `diasRestantes` localmente con `Math.ceil((new Date(fechaLimite).getTime() - Date.now()) / 86400000)`. Estilos: `bg-red-900/30 text-red-300 border-red-700`. Animación CSS `urgency-pulse` (opacity 1 -> 0.7 -> 1, 2s infinite). Accesible: `aria-label="Urgente: menos de {URGENCIA_DIAS_UMBRAL} dias para el cierre"`.

---

### 3.11 EnviarPropuestaForm

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EnviarPropuestaForm.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `necesidadId` | `string` | Si | ID de la necesidad a la que se envia la propuesta |
| `necesidadTitulo` | `string` | Si | Titulo de la necesidad (para mostrar en el header del dialog) |
| `presupuestoMin` | `number \| undefined` | No | Presupuesto minimo del artista (para nota informativa de rango) |
| `presupuestoMax` | `number \| undefined` | No | Presupuesto maximo del artista (para nota informativa) |
| `monedaNombre` | `string \| undefined` | No | Nombre de la moneda de referencia del artista (para nota informativa) |
| `isOpen` | `boolean` | Si | Controla la apertura del dialog |
| `onClose` | `() => void` | Si | Callback para cerrar el dialog |

**Estado Local:**
- `form` (React Hook Form instance) - gestiona campo `precioPropuesto`, `monedaId`, `diasEstimados`, `mensajePropuesta`
- `precioInfo: 'dentro' | 'por-encima' | 'por-debajo' | null` - calculado en tiempo real al cambiar el precio

**Dependencias:**
- Hooks: `useCreatePropuesta`
- shadcn: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Input`, `Textarea`, `Label`, `Button`, `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`, `Alert`, `AlertDescription`
- Lucide: `CheckCircle`, `AlertTriangle`, `Loader2`, `Info`
- React Hook Form: `useForm`, `Controller`
- Zod: `zodResolver`, `createPropuestaSchema` de `@shared/schemas/crowdsourcing.schema`
- Tipos: `CreatePropuestaFormData` de `@shared/schemas/crowdsourcing.schema`

**Responsabilidad:**
Dialog modal con formulario de propuesta. Campos: precio propuesto (number input), moneda (select), dias estimados (number input opcional), mensaje de propuesta (textarea con contador de caracteres). Calcula en tiempo real la nota informativa de rango (dentro/por encima/por debajo del presupuesto del artista). Muestra errores de validacion bajo cada campo con `role="alert"`. En estado de submit: spinner + "Enviando...", inputs disabled, botón submit disabled. Al exito: cierra dialog, muestra toast de éxito. Al error de API: muestra toast de error, dialog permanece abierto. Al cancelar o Escape: cierra y resetea el form. Focus trap integrado via shadcn Dialog.

**Logica nota informativa de rango:**
```
si precioPropuesto > 0 && presupuestoMax definido:
    si precio >= presupuestoMin && precio <= presupuestoMax -> 'dentro'
    si precio > presupuestoMax                             -> 'por-encima'
    si precio < presupuestoMin                             -> 'por-debajo'
sino: null (ocultar nota)
```

**Monedas disponibles:** Se obtienen de una maestra hardcodeada o de un hook de maestras existente. Si hay un endpoint de maestras de monedas, usar ese; sino, definir lista estatica `[{ id: 1, nombre: 'EUR' }, { id: 2, nombre: 'USD' }]`.

---

### 3.12 RetirarPropuestaDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RetirarPropuestaDialog.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `propuesta` | `MiPropuestaList \| null` | Si | Propuesta a retirar; null = dialog cerrado |
| `isOpen` | `boolean` | Si | Controla apertura del dialog |
| `onClose` | `() => void` | Si | Callback para cerrar el dialog |

**Estado Local:** Ninguno (estado de la mutacion viene del hook)

**Dependencias:**
- Hooks: `useRetirarPropuesta`
- shadcn: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Button`, `Alert`, `AlertDescription`
- Lucide: `AlertTriangle`, `Loader2`

**Responsabilidad:**
Dialog de confirmacion antes de retirar una propuesta. Muestra: icono de warning + alerta roja "Esta accion no se puede deshacer", nombre de la necesidad entre comillas, texto explicativo de consecuencias. Botones: "Cancelar" (outline) y "Confirmar retirada" (destructive rojo). En estado de submit: ambos botones disabled, spinner + "Retirando..." en boton confirmar. Al exito: dialog cierra, toast success, la lista de propuestas se invalida via query. Al error: toast error, dialog cierra. Escape y [x] cierran sin cambios. Accesible: `aria-modal`, `aria-labelledby`, `aria-describedby`.

---

### 3.13 EmptyStateNecesidades

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EmptyStateNecesidades.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `hasActiveFilters` | `boolean` | Si | true = empty con filtros activos, false = empty sin filtros |
| `onClearFilters` | `() => void` | No | Solo necesario si hasActiveFilters=true |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Button`
- Lucide: `Inbox`, `SearchX`

**Responsabilidad:**
Empty state del listado de necesidades. Dos variantes:
- Sin filtros activos: icono `Inbox` + "Sin necesidades abiertas" + "Vuelve pronto para ver nuevas oportunidades de trabajo"
- Con filtros activos: icono `SearchX` + "Sin resultados" + "No hay necesidades que coincidan con tus filtros. Intenta ampliar la busqueda." + botón "Limpiar filtros"

Accesible: contenedor con `role="status"`.

---

### 3.14 EmptyStatePropuestas

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EmptyStatePropuestas.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoFiltroActivo` | `number \| null` | Si | ID del estado filtrado activo; null = sin filtro |
| `estadoNombre` | `string \| undefined` | No | Nombre legible del estado para el mensaje |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Button`
- Lucide: `FileText`, `Filter`
- React Router: `Link`
- Constantes: `APP_ROUTES` de `@shared/constants`

**Responsabilidad:**
Empty state de Mis Propuestas. Dos variantes:
- Sin filtro de estado (estadoFiltroActivo=null): icono `FileText` + "Sin propuestas enviadas" + "Aun no has enviado propuestas a ningun artista" + link "Explorar necesidades" que navega a `/crowdsourcing/necesidades`
- Con filtro de estado activo: icono `Filter` + "Sin propuestas {estadoNombre}" + "No tienes propuestas en este estado"

---

### 3.15 PerfilProfesionalCTA

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/PerfilProfesionalCTA.tsx`

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `className` | `string` | No | Clase CSS adicional |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn: `Button`
- React Router: `Link`

**Responsabilidad:**
Bloque CTA para usuarios sin PerfilProfesional, visible en el sidebar de accion de `NecesidadDetallePage` cuando `tienePerfilProfesional=false`. Muestra: bloque con fondo `bg-[#1e2a42]`, texto "Necesitas un perfil profesional para enviar propuestas", botón outline "Crear perfil profesional" que navega al formulario de perfil. La ruta de destino se define como un TODO hasta que exista el formulario de perfil profesional en la landing.

---

## 4. Hooks

### 4.1 useNecesidadesPublicas

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useNecesidadesPublicas.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `filters` | `NecesidadesPublicasFilter` | Filtros del listado: search, tipo, modalidad, presupuesto, pais, orderBy, page, pageSize |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `PaginatedResponse<NecesidadPublicaList> \| undefined` | Datos paginados con items, totalCount, page, totalPages |
| `isLoading` | `boolean` | true durante la carga inicial (no hay datos previos en cache) |
| `isFetching` | `boolean` | true durante cualquier fetch (incluye refetch con datos previos) |
| `error` | `Error \| null` | Error si el request fallo |

**Query Key:** `[...QUERY_KEYS.crowdsourcing.necesidades.publicas, filters]`

**Opciones:**
- `staleTime: 30_000` (30 segundos)
- `placeholderData: keepPreviousData` - mantiene los datos anteriores durante la carga de nuevos filtros para evitar flash de skeleton mientras el usuario filtra
- `enabled: true` (siempre activo, el usuario debe estar autenticado via ProtectedRoute)

**Uso del servicio:** Llama a `necesidadPublicaApi.getAll(filters)` pasando los filtros como query params.

---

### 4.2 useNecesidadPublica

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useNecesidadPublica.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `id` | `string` | ID de la necesidad a consultar |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `NecesidadPublica \| undefined` | Detalle completo con campos calculados (yaPropuso, esPropietario, tienePerfilProfesional) |
| `isLoading` | `boolean` | Estado de carga inicial |
| `error` | `Error \| null` | Error (incluyendo 404) |

**Query Key:** `QUERY_KEYS.crowdsourcing.necesidades.publicaById(id)`

**Opciones:**
- `staleTime: 30_000`
- `enabled: !!id` - no ejecutar si id es falsy

**Uso del servicio:** Llama a `necesidadPublicaApi.getById(id)`.

---

### 4.3 useCreatePropuesta

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCreatePropuesta.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `necesidadId` | `string` | ID de la necesidad; necesario para invalidar la query del detalle |
| `onSuccess` | `() => void` | Callback post-exito (para cerrar el dialog desde el padre) |

**Parametros de mutate:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreatePropuestaRequest` | Body del POST: precioPropuesto, monedaId, diasEstimados?, mensajePropuesta |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(data: CreatePropuestaRequest) => void` | Ejecutar la mutacion |
| `isPending` | `boolean` | true mientras la request esta en curso |
| `error` | `Error \| null` | Error si fallo |

**Acciones:**
- `mutationFn`: llama a `propuestaApi.create(necesidadId, data)`
- `onSuccess`:
  1. Invalida `QUERY_KEYS.crowdsourcing.necesidades.publicaById(necesidadId)` - actualiza `yaPropuso=true` en detalle
  2. Invalida `QUERY_KEYS.crowdsourcing.propuestas.mis` - refresca "Mis Propuestas"
  3. Invalida `QUERY_KEYS.crowdsourcing.necesidades.publicas` - actualiza `numeroPropuestas` en listado
  4. `toast.success("Propuesta enviada correctamente. El artista sera notificado.")`
  5. Llama `onSuccess()` para que el padre cierre el dialog
- `onError`:
  - Detecta error code del mensaje de error y usa `getPropuestaErrorMessage` para toast descriptivo
  - `toast.error(getPropuestaErrorMessage(errorCode))`

**Uso del servicio:** `propuestaApi.create(necesidadId, data)`

---

### 4.4 useMisPropuestas

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useMisPropuestas.ts`

**Tipo:** Query Hook

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `filters` | `{ estado?: number; page?: number; pageSize?: number }` | Filtros opcionales de estado y paginacion |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `PaginatedResponse<MiPropuestaList> \| undefined` | Propuestas paginadas del usuario autenticado |
| `isLoading` | `boolean` | Estado de carga inicial |
| `isFetching` | `boolean` | Estado de cualquier fetch (para spinner de filtro) |
| `error` | `Error \| null` | Error si fallo |

**Query Key:** `[...QUERY_KEYS.crowdsourcing.propuestas.mis, filters]`

**Opciones:**
- `staleTime: 30_000`
- `placeholderData: keepPreviousData`

**Uso del servicio:** `propuestaApi.getMisPropuestas(filters)`

---

### 4.5 useRetirarPropuesta

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useRetirarPropuesta.ts`

**Tipo:** Mutation Hook

**Parametros de mutate:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `propuestaId` | `string` | ID de la propuesta a retirar |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `(propuestaId: string) => void` | Ejecutar la mutacion |
| `isPending` | `boolean` | true mientras la request esta en curso |

**Acciones:**
- `mutationFn`: llama a `propuestaApi.retirar(propuestaId)`
- `onSuccess`:
  1. Invalida `QUERY_KEYS.crowdsourcing.propuestas.mis` - refresca la lista con el nuevo estado
  2. `toast.success("Propuesta retirada correctamente")`
- `onError`:
  - `toast.error(getPropuestaErrorMessage(errorCode))`

**Uso del servicio:** `propuestaApi.retirar(propuestaId)`

---

## 5. Services (API Clients)

### 5.1 necesidadPublicaApi

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/necesidadPublica.api.ts`

**Patron:** Clase instanciada como singleton (igual que `CrowdsourcingApiService`)

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getAll(filters)` | `NecesidadesPublicasFilter` | `Promise<PaginatedResponse<NecesidadPublicaList>>` | `GET /api/crowdsourcing/necesidades?{queryParams}` |
| `getById(id)` | `id: string` | `Promise<NecesidadPublica>` | `GET /api/crowdsourcing/necesidades/{id}` |

**Implementacion de query params en `getAll`:**
Construir `URLSearchParams` iterando sobre `filters`, omitiendo keys con valor `undefined`. Campos array (`tipoNecesidadId` si se hace multi-select en el futuro) se agregan como valores multiples. El endpoint actual acepta un solo `tipoNecesidadId`; si se extiende a multi-select, se repiten los params.

**Manejo de errores:**
- `getById`: si `response.data` es null o el backend retorna 404, lanzar `new Error(response.messages[0]?.message ?? 'Necesidad no encontrada')`. El error se captura en el hook con `error` de `useQuery` y el componente renderiza el estado de error 404.
- `getAll`: si `response.data` es null, retornar estructura vacia `{ items: [], totalCount: 0, page: 1, pageSize: 12, totalPages: 0 }`.

**Importaciones:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- Types de `../../domain` (re-exportados desde `@shared/types/crowdsourcing`)

---

### 5.2 propuestaApi

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/propuesta.api.ts`

**Patron:** Clase instanciada como singleton

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `create(necesidadId, data)` | `necesidadId: string, data: CreatePropuestaRequest` | `Promise<PropuestaCreatedResult>` | `POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas` |
| `getMisPropuestas(filters)` | `{ estado?: number; page?: number; pageSize?: number }` | `Promise<PaginatedResponse<MiPropuestaList>>` | `GET /api/crowdsourcing/propuestas/mis-propuestas?{queryParams}` |
| `retirar(propuestaId)` | `propuestaId: string` | `Promise<RetirarPropuestaResult>` | `PATCH /api/crowdsourcing/propuestas/{id}/retirar` |

**Manejo de errores:**
Todos los metodos verifican `response.data`. Si falla, extraen el errorCode del primer mensaje (`response.messages[0]?.errorCode`) y construyen el error como `new Error(errorCode)`. Esto permite al hook usar `getPropuestaErrorMessage(error.message)` para mostrar el toast correcto.

**Importaciones:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- Types de `../../domain`

---

## 6. Flujo de Datos

### Listado de Necesidades (ExplorarNecesidadesPage)

```
Usuario escribe en search input
    ↓
ExplorarNecesidadesPage (estado local: searchInput)
    ↓ debounce 300ms
setSearchParams({ search: debouncedSearch, page: 1 })
    ↓
useNecesidadesPublicas({ search, tipo, modalidad, ...otrosFiltros, page })
    ↓
necesidadPublicaApi.getAll(filters)
    ↓
apiFetch GET /api/crowdsourcing/necesidades?search=...&page=1
    ↓
Backend (con PaginatedResponse<NecesidadPublicaList>)
    ↓
TanStack Query cache
    ↓
Grid de NecesidadCard (render)
```

### Enviar Propuesta (NecesidadDetallePage -> EnviarPropuestaForm)

```
Usuario click "Enviar propuesta"
    ↓
NecesidadDetallePage setea isFormOpen=true
    ↓
EnviarPropuestaForm (Dialog abierto)
    ↓
Usuario completa form + submit
    ↓
React Hook Form valida con createPropuestaSchema (Zod)
    ↓ si valido
useCreatePropuesta.mutate({ precioPropuesto, monedaId, diasEstimados, mensajePropuesta })
    ↓
propuestaApi.create(necesidadId, data)
    ↓
apiFetch POST /api/crowdsourcing/necesidades/{id}/propuestas
    ↓
Backend (201 Created / 400/403/404)
    ↓ onSuccess
invalidateQueries [necesidadPublica, misPropuestas, necesidadesPublicas]
toast.success("Propuesta enviada correctamente. El artista sera notificado.")
onSuccess() -> NecesidadDetallePage setea isFormOpen=false
    ↓ onError
toast.error(getPropuestaErrorMessage(errorCode))
Dialog permanece abierto
```

### Retirar Propuesta (MisPropuestasPage -> RetirarPropuestaDialog)

```
Usuario click "Retirar propuesta" en PropuestaCard
    ↓
MisPropuestasPage: setPropuestaARetirar(propuesta)
    ↓
RetirarPropuestaDialog abierto con propuesta.necesidadTitulo
    ↓
Usuario click "Confirmar retirada"
    ↓
useRetirarPropuesta.mutate(propuesta.id)
    ↓
propuestaApi.retirar(propuestaId)
    ↓
apiFetch PATCH /api/crowdsourcing/propuestas/{id}/retirar
    ↓
Backend (200 OK / 400/403/404)
    ↓ onSuccess
invalidateQueries [misPropuestas]
toast.success("Propuesta retirada correctamente")
onClose() -> dialog cierra, propuesta en lista actualiza badge a RETIRADA
    ↓ onError
toast.error(mensaje)
onClose() -> dialog cierra
```

---

## 7. State Management - URL State para Filtros

### ExplorarNecesidadesPage - URL params

```
/crowdsourcing/necesidades?search=mezcla&tipo=3&modalidad=2&presupuestoMin=100&presupuestoMax=500&pais=ES&ciudad=Madrid&orderBy=recientes&page=2
```

**Lectura de URL -> estado:**
```typescript
const [searchParams, setSearchParams] = useSearchParams()

const filters: NecesidadesPublicasFilter = {
    search: searchParams.get('search') ?? undefined,
    tipoNecesidadId: searchParams.get('tipo') ? Number(searchParams.get('tipo')) : undefined,
    modalidad: searchParams.get('modalidad') ? Number(searchParams.get('modalidad')) : undefined,
    presupuestoMin: searchParams.get('presupuestoMin') ? Number(searchParams.get('presupuestoMin')) : undefined,
    presupuestoMax: searchParams.get('presupuestoMax') ? Number(searchParams.get('presupuestoMax')) : undefined,
    pais: searchParams.get('pais') ?? undefined,
    orderBy: (searchParams.get('orderBy') as OrderByNecesidades) ?? 'recientes',
    page: Number(searchParams.get('page') ?? '1'),
    pageSize: 12,
}
```

**Escritura de filtros -> URL:**
```typescript
const updateFilter = (partial: Partial<NecesidadesPublicasFilter>) => {
    setSearchParams(prev => {
        const next = new URLSearchParams(prev)
        // Aplicar cambios parciales
        // Resetear page a 1 si cambia cualquier filtro excepto page
        // Eliminar params con valor undefined/null
        return next
    })
}
```

**Estado local (inputs en tiempo real, antes del debounce):**
```typescript
const [searchInput, setSearchInput] = useState(filters.search ?? '')
const debouncedSearch = useDebounce(searchInput, 300)
// Al cambiar debouncedSearch, actualizar URL
useEffect(() => { updateFilter({ search: debouncedSearch, page: 1 }) }, [debouncedSearch])

const [presupuestoMinInput, setPresupuestoMinInput] = useState(String(filters.presupuestoMin ?? ''))
const debouncedPresupuestoMin = useDebounce(presupuestoMinInput, 500)
// Idem para presupuestoMax y ciudad
```

**Nota importante:** Los selects (modalidad, tipo chips, orderBy, pais) actualizan la URL directamente sin debounce, ya que son selecciones discretas (no texto libre).

### MisPropuestasPage - URL param de estado

```
/crowdsourcing/mis-propuestas?estado=1
```

Leer `estado` de `searchParams` como `number | undefined`. Pasar como filtro a `useMisPropuestas`.

---

## 8. Routing - Cambios en router.tsx

Agregar 3 rutas nuevas en `src/web/src/app/router.tsx`. Las tres son rutas protegidas (usuario debe estar autenticado). El redirect a login se maneja via el componente `ProtectedRoute` existente o mediante la comprobacion en el layout.

**Importaciones lazy a agregar:**
```typescript
const ExplorarNecesidadesPage = lazy(() =>
    import("@/features/crowdsourcing/presentation/pages/ExplorarNecesidadesPage")
)
const NecesidadDetallePage = lazy(() =>
    import("@/features/crowdsourcing/presentation/pages/NecesidadDetallePage")
)
const MisPropuestasPage = lazy(() =>
    import("@/features/crowdsourcing/presentation/pages/MisPropuestasPage")
)
```

**Rutas a agregar (dentro del bloque `PublicLayout` con proteccion de auth):**
```tsx
<Route path="/crowdsourcing/necesidades" element={<ExplorarNecesidadesPage />} />
<Route path="/crowdsourcing/necesidades/:id" element={<NecesidadDetallePage />} />
<Route path="/crowdsourcing/mis-propuestas" element={<MisPropuestasPage />} />
```

**Nota de proteccion:** Las tres rutas requieren JWT. Si el `PublicLayout` ya redirige a login cuando no hay token, estas se agregan al mismo bloque. Si no, se debe agregar un `ProtectedRoute` wrapper o verificar el patron existente en el proyecto.

**ROUTES constants a agregar en `src/web/src/lib/constants.ts`:**
```typescript
CROWDSOURCING_NECESIDADES: '/crowdsourcing/necesidades',
CROWDSOURCING_NECESIDAD_DETAIL: '/crowdsourcing/necesidades/:id',
CROWDSOURCING_MIS_PROPUESTAS: '/crowdsourcing/mis-propuestas',
```

---

## 9. Dependencias de Shared

**Importar de `@shared/types/crowdsourcing`:**
- `NecesidadPublicaList` - DTO para NecesidadCard y listado
- `NecesidadPublica` - DTO para NecesidadDetallePage
- `ArtistaPublico` - Subtype embebido en NecesidadPublica
- `MiPropuestaList` - DTO para PropuestaCard y listado
- `CreatePropuestaRequest` - Body del POST enviar propuesta
- `PropuestaCreatedResult` - Response del POST
- `RetirarPropuestaResult` - Response del PATCH retirar
- `NecesidadesPublicasFilter` - Filtros del listado
- `EstadoPropuesta` - Union type string de estado
- `OrderByNecesidades` - Union type de opciones de orden

**Importar de `@shared/schemas/crowdsourcing.schema`:**
- `createPropuestaSchema` - Schema Zod para EnviarPropuestaForm
- `CreatePropuestaFormData` - Type inferido del schema

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdsourcing.necesidades.publicas`
- `QUERY_KEYS.crowdsourcing.necesidades.publicaById`
- `QUERY_KEYS.crowdsourcing.propuestas.mis`
- `API_ROUTES.crowdsourcing.necesidades.base`
- `API_ROUTES.crowdsourcing.necesidades.byId`
- `API_ROUTES.crowdsourcing.necesidades.propuestas`
- `API_ROUTES.crowdsourcing.propuestas.mis`
- `API_ROUTES.crowdsourcing.propuestas.retirar`
- `APP_ROUTES.landing.crowdsourcing.necesidades`
- `APP_ROUTES.landing.crowdsourcing.necesidadDetail`
- `APP_ROUTES.landing.crowdsourcing.misPropuestas`
- `ESTADO_PROPUESTA` - IDs numericos de estado
- `ESTADO_PROPUESTA_LABELS` - Etiquetas para UI
- `ESTADO_PROPUESTA_BADGES` - Colores semanticos
- `ORDER_BY_NECESIDADES_LABELS` - Etiquetas para select
- `URGENCIA_DIAS_UMBRAL` - Umbral de dias para badge urgencia

**Importar de `@shared/utils/error-messages`:**
- `getPropuestaErrorMessage` - Para toasts de error en hooks de mutacion

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdsourcing/infrastructure/api/necesidadPublica.api.ts` | API service | GET listado publico y detalle de necesidades |
| `src/web/src/features/crowdsourcing/infrastructure/api/propuesta.api.ts` | API service | POST crear, GET mis propuestas, PATCH retirar |
| `src/web/src/features/crowdsourcing/application/hooks/useNecesidadesPublicas.ts` | Query hook | Listado paginado con filtros y keepPreviousData |
| `src/web/src/features/crowdsourcing/application/hooks/useNecesidadPublica.ts` | Query hook | Detalle con campos calculados |
| `src/web/src/features/crowdsourcing/application/hooks/useCreatePropuesta.ts` | Mutation hook | Enviar propuesta con invalidacion de queries |
| `src/web/src/features/crowdsourcing/application/hooks/useMisPropuestas.ts` | Query hook | Listado paginado propio con filtro de estado |
| `src/web/src/features/crowdsourcing/application/hooks/useRetirarPropuesta.ts` | Mutation hook | Retirar propuesta pendiente |
| `src/web/src/features/crowdsourcing/presentation/components/NecesidadCard.tsx` | Component | Card del listado con badges, hover, accesibilidad |
| `src/web/src/features/crowdsourcing/presentation/components/NecesidadCardSkeleton.tsx` | Component | Skeleton para loading del listado |
| `src/web/src/features/crowdsourcing/presentation/components/NecesidadFilters.tsx` | Component | Panel filtros (sidebar desktop + accordion mobile) |
| `src/web/src/features/crowdsourcing/presentation/components/PropuestaCard.tsx` | Component | Card de mis propuestas con acciones por estado |
| `src/web/src/features/crowdsourcing/presentation/components/PropuestaCardSkeleton.tsx` | Component | Skeleton para loading de mis propuestas |
| `src/web/src/features/crowdsourcing/presentation/components/EstadoPropuestaBadge.tsx` | Component | Badge semantico de estado con colores |
| `src/web/src/features/crowdsourcing/presentation/components/UrgenciaBadge.tsx` | Component | Badge urgencia con calculo de dias restantes |
| `src/web/src/features/crowdsourcing/presentation/components/EnviarPropuestaForm.tsx` | Component | Dialog form con react-hook-form + zod + nota de rango |
| `src/web/src/features/crowdsourcing/presentation/components/RetirarPropuestaDialog.tsx` | Component | Dialog de confirmacion destructive |
| `src/web/src/features/crowdsourcing/presentation/components/EmptyStateNecesidades.tsx` | Component | Empty state del listado (2 variantes) |
| `src/web/src/features/crowdsourcing/presentation/components/EmptyStatePropuestas.tsx` | Component | Empty state de mis propuestas (2 variantes) |
| `src/web/src/features/crowdsourcing/presentation/components/PerfilProfesionalCTA.tsx` | Component | CTA para crear perfil profesional |
| `src/web/src/features/crowdsourcing/presentation/pages/ExplorarNecesidadesPage.tsx` | Page | Listado paginado con sidebar filtros + URL state |
| `src/web/src/features/crowdsourcing/presentation/pages/NecesidadDetallePage.tsx` | Page | Detalle 2 columnas con CTA dinamico |
| `src/web/src/features/crowdsourcing/presentation/pages/MisPropuestasPage.tsx` | Page | Lista propuestas con filter chips + dialog retirar |

## Archivos a Modificar

| Archivo | Cambio |
|---------|--------|
| `src/web/src/features/crowdsourcing/domain/types.ts` | Re-exportar `NecesidadPublicaList`, `NecesidadPublica`, `ArtistaPublico`, `MiPropuestaList`, `CreatePropuestaRequest`, `PropuestaCreatedResult`, `RetirarPropuestaResult`, `NecesidadesPublicasFilter`, `EstadoPropuesta`, `OrderByNecesidades` desde `@shared/types/crowdsourcing` |
| `src/web/src/features/crowdsourcing/domain/index.ts` | Exportar nuevos types de `types.ts` |
| `src/web/src/features/crowdsourcing/infrastructure/index.ts` | Exportar `necesidadPublicaApi` y `propuestaApi` |
| `src/web/src/features/crowdsourcing/application/index.ts` | Exportar los 5 hooks nuevos |
| `src/web/src/features/crowdsourcing/presentation/components/index.ts` | Exportar los 12 componentes nuevos |
| `src/web/src/features/crowdsourcing/presentation/pages/index.ts` | Exportar las 3 pages nuevas |
| `src/web/src/app/router.tsx` | Agregar lazy imports y 3 rutas de crowdsourcing |
| `src/web/src/lib/constants.ts` | Agregar constantes de rutas en `ROUTES` |

---

## 11. Orden de Implementacion

El orden respeta las dependencias: los artefactos de capa inferior deben existir antes de ser importados por capas superiores.

```
FASE 1 - Shared (prerequisito de todo)
├── 1. src/shared/types/crowdsourcing.ts          (agregar 6 interfaces + 2 union types)
├── 2. src/shared/schemas/crowdsourcing.schema.ts (agregar createPropuestaSchema, filterNecesidadesSchema)
├── 3. src/shared/constants/index.ts              (agregar ESTADO_PROPUESTA, QUERY_KEYS, API_ROUTES, APP_ROUTES)
└── 4. src/shared/utils/error-messages.ts         (agregar PROPUESTA_ERROR_MESSAGES, getPropuestaErrorMessage)

FASE 2 - Domain (re-exports de shared)
└── 5. src/web/src/features/crowdsourcing/domain/types.ts  (agregar re-exports de nuevos types)

FASE 3 - Infrastructure (sin dependencias de presentation)
├── 6. necesidadPublica.api.ts   (depende de domain types y API_ROUTES)
└── 7. propuesta.api.ts          (depende de domain types y API_ROUTES)

FASE 4 - Application / Hooks (dependen de infrastructure)
├── 8.  useNecesidadesPublicas.ts   (depende de necesidadPublicaApi y QUERY_KEYS)
├── 9.  useNecesidadPublica.ts      (depende de necesidadPublicaApi y QUERY_KEYS)
├── 10. useCreatePropuesta.ts       (depende de propuestaApi, QUERY_KEYS, getPropuestaErrorMessage)
├── 11. useMisPropuestas.ts         (depende de propuestaApi y QUERY_KEYS)
└── 12. useRetirarPropuesta.ts      (depende de propuestaApi, QUERY_KEYS, getPropuestaErrorMessage)

FASE 5 - Componentes atomicos y de display (sin dependencias de hooks de datos)
├── 13. EstadoPropuestaBadge.tsx    (solo ESTADO_PROPUESTA constants + shadcn Badge)
├── 14. UrgenciaBadge.tsx           (solo URGENCIA_DIAS_UMBRAL + shadcn Badge + Lucide)
├── 15. NecesidadCardSkeleton.tsx   (solo shadcn Skeleton + Card)
├── 16. PropuestaCardSkeleton.tsx   (solo shadcn Skeleton + Card)
├── 17. EmptyStateNecesidades.tsx   (shadcn Button + Lucide)
├── 18. EmptyStatePropuestas.tsx    (shadcn Button + Lucide + Link)
└── 19. PerfilProfesionalCTA.tsx    (shadcn Button + Link)

FASE 6 - Componentes de negocio (dependen de tipos de datos y componentes atomicos)
├── 20. NecesidadCard.tsx           (depende de NecesidadPublicaList, UrgenciaBadge)
├── 21. PropuestaCard.tsx           (depende de MiPropuestaList, EstadoPropuestaBadge)
└── 22. NecesidadFilters.tsx        (depende de NecesidadesPublicasFilter, ORDER_BY constants)

FASE 7 - Componentes de formulario/dialog (dependen de hooks de mutacion)
├── 23. EnviarPropuestaForm.tsx     (depende de useCreatePropuesta, createPropuestaSchema)
└── 24. RetirarPropuestaDialog.tsx  (depende de useRetirarPropuesta)

FASE 8 - Pages (dependen de todo lo anterior)
├── 25. ExplorarNecesidadesPage.tsx (depende de useNecesidadesPublicas, NecesidadCard, NecesidadFilters, EmptyStateNecesidades, NecesidadCardSkeleton)
├── 26. NecesidadDetallePage.tsx    (depende de useNecesidadPublica, EnviarPropuestaForm, PerfilProfesionalCTA, UrgenciaBadge)
└── 27. MisPropuestasPage.tsx       (depende de useMisPropuestas, PropuestaCard, RetirarPropuestaDialog, EmptyStatePropuestas, PropuestaCardSkeleton)

FASE 9 - Integracion en router
└── 28. router.tsx                  (agregar lazy imports y rutas, despues de que las pages existan)
```

---

## 12. Notas de Implementacion

### Debounce hook
El proyecto no tiene un `useDebounce` utilitario documentado. Se debe verificar si existe en `src/web/src/hooks/` o en `src/shared/`. Si no existe, crear un hook simple:
```typescript
// src/web/src/hooks/useDebounce.ts
export function useDebounce<T>(value: T, delay: number): T {
    const [debouncedValue, setDebouncedValue] = useState<T>(value)
    useEffect(() => {
        const handler = setTimeout(() => setDebouncedValue(value), delay)
        return () => clearTimeout(handler)
    }, [value, delay])
    return debouncedValue
}
```

### PaginatedResponse type
Verificar que `PaginatedResponse<T>` existe en `src/shared/types/api.ts` con estructura `{ items: T[], totalCount: number, page: number, pageSize: number, totalPages: number }`. El contracts-plan.md lo confirma como existente.

### Maestras de moneda para el form
El `EnviarPropuestaForm` necesita opciones de moneda para el select. Verificar si existe un endpoint de maestras de monedas en el API. Si existe (similar a `getRolesProfesionales`), crear un hook `useMonedasMaestra`. Si no existe, usar lista estática `[{ id: 1, nombre: 'EUR' }, { id: 2, nombre: 'USD' }]` como valor temporal.

### Proteccion de rutas
Verificar el mecanismo de proteccion de rutas existente en el proyecto. Si las rutas en `PublicLayout` ya estan protegidas por algun `AuthGuard`, usar el mismo patron. Si no, estas 3 rutas necesitan redirigir a `/login?returnUrl={currentPath}` cuando no hay token JWT en el store.

### Manejo del `necesidadId` en PropuestaCard
El DTO `MiPropuestaList` no incluye `necesidadId` segun el contrato definido. El link al detalle de la necesidad desde `PropuestaCard` no es posible sin ese campo. Opciones:
1. Pedir al backend que incluya `necesidadId` en el DTO (preferido)
2. Omitir el link y solo mostrar el titulo como texto
3. El click en el titulo navega a la pagina de detalle si se agrega `necesidadId` al DTO

Documentar como TODO pendiente de confirmacion con backend.

### Toast provider
El proyecto usa `sonner` para toasts (confirmado por `toast` de `sonner` en `useBackings.ts`). Los toasts de esta feature usaran el mismo patron: `toast.success(...)` y `toast.error(...)`.

### Formato de fechas
Usar las utilidades de formato de fecha existentes en `@shared/utils` (si existen) o de `date-fns`. Las fechas se muestran como:
- Fecha relativa en NecesidadCard: "hace 2 dias" (el backend retorna `fechaRelativa` calculada, o usar `formatDistanceToNow` de date-fns)
- Fecha limite: formato "15 mar 2026"
- Fechas en PropuestaCard: formato "20 feb 2026"

---

## 13. Checklist

- [ ] Shared types agregados: `NecesidadPublicaList`, `NecesidadPublica`, `ArtistaPublico`, `MiPropuestaList`, `CreatePropuestaRequest`, `PropuestaCreatedResult`, `RetirarPropuestaResult`, `NecesidadesPublicasFilter`, `EstadoPropuesta`, `OrderByNecesidades`
- [ ] Shared schemas agregados: `createPropuestaSchema`, `filterNecesidadesSchema`, `CreatePropuestaFormData`, `FilterNecesidadesFormData`
- [ ] Shared constants agregados: `ESTADO_PROPUESTA`, `ESTADO_PROPUESTA_LABELS`, `ESTADO_PROPUESTA_BADGES`, `ORDER_BY_NECESIDADES`, `ORDER_BY_NECESIDADES_LABELS`, `URGENCIA_DIAS_UMBRAL`
- [ ] Shared constants modificados: `QUERY_KEYS.crowdsourcing.necesidades.publicas`, `QUERY_KEYS.crowdsourcing.necesidades.publicaById`, `QUERY_KEYS.crowdsourcing.propuestas.mis`
- [ ] Shared constants modificados: `API_ROUTES.crowdsourcing.necesidades.propuestas`, `API_ROUTES.crowdsourcing.propuestas.mis`, `API_ROUTES.crowdsourcing.propuestas.retirar`
- [ ] Shared constants modificados: `APP_ROUTES.landing.crowdsourcing.necesidades`, `.necesidadDetail`, `.misPropuestas`
- [ ] `necesidadPublica.api.ts` - getAll con query params, getById con manejo 404
- [ ] `propuesta.api.ts` - create, getMisPropuestas, retirar con extraccion de errorCode
- [ ] `useNecesidadesPublicas` - useQuery con `keepPreviousData`, staleTime 30s
- [ ] `useNecesidadPublica` - useQuery con `enabled: !!id`
- [ ] `useCreatePropuesta` - useMutation con 3 invalidaciones + toast + callback onSuccess
- [ ] `useMisPropuestas` - useQuery con filtros de estado + `keepPreviousData`
- [ ] `useRetirarPropuesta` - useMutation con invalidacion + toast
- [ ] Todos los hooks usan `QUERY_KEYS` de shared (no strings literales)
- [ ] `EstadoPropuestaBadge` con colores semanticos por estado
- [ ] `UrgenciaBadge` con calculo local de dias restantes y animacion pulse
- [ ] `NecesidadCard` con hover effect, `role="article"`, `tabIndex={0}`, soporte Enter/Space
- [ ] `NecesidadFilters` con accordion colapsable para mobile
- [ ] `EnviarPropuestaForm` usa `createPropuestaSchema` + `zodResolver` + React Hook Form
- [ ] `EnviarPropuestaForm` calcula nota informativa de rango en tiempo real
- [ ] `EnviarPropuestaForm` muestra contador de caracteres en textarea
- [ ] `EnviarPropuestaForm` deshabilita inputs y boton submit durante `isPending`
- [ ] `RetirarPropuestaDialog` con variant destructive + alert roja + focus trap
- [ ] `ExplorarNecesidadesPage` sincroniza filtros con URL via `useSearchParams`
- [ ] `ExplorarNecesidadesPage` aplica debounce 300ms en search, 500ms en presupuesto/ciudad
- [ ] `ExplorarNecesidadesPage` usa `placeholderData: keepPreviousData` para UX suave
- [ ] `NecesidadDetallePage` renderiza CTA dinamico segun `yaPropuso`, `esPropietario`, `tienePerfilProfesional`
- [ ] `NecesidadDetallePage` layout 2 columnas desktop, 1 columna mobile con CTA sticky bottom
- [ ] `MisPropuestasPage` filtro de estado sincronizado con URL `?estado=N`
- [ ] `MisPropuestasPage` gestiona `propuestaARetirar` para el dialog de confirmacion
- [ ] 3 rutas agregadas en `router.tsx` con lazy imports
- [ ] Rutas protegidas (redirect a login si no autenticado)
- [ ] Todos los componentes usan shadcn/ui (no HTML nativo para UI)
- [ ] No se usa `any` en ninguna definicion TypeScript
- [ ] ARIA labels, `role`, `aria-pressed`, `aria-live` implementados en componentes interactivos
- [ ] Empty states con `role="status"` y mensajes descriptivos
- [ ] Toast de sonner para success y error en mutaciones
- [ ] Skeletons accesibles con `role="status" aria-label="Cargando..."`
- [ ] Dark theme consistente con tokens del proyecto (`bg-[#0f1729]`, `border-[#334155]`, etc.)
