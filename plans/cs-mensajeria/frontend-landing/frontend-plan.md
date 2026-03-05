# Plan Frontend: cs-mensajeria (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)
**Target:** src/web (Vite + React 18, TypeScript, Tailwind, shadcn/ui)

---

## 1. Resumen

- Pantallas: 2 (MensajesPage + ConversacionChatPage)
- Componentes nuevos: 11
- Componentes modificados: 2 (Header + ConversacionLink)
- Hooks de query: 3 (useConversaciones, useMensajes, useNoLeidosCount)
- Hooks de mutation: 3 (useCreateConversacion, useEnviarMensaje, useMarcarLeidos)
- API services (archivos): 2 (conversacion.api.ts, mensaje.api.ts)
- Rutas nuevas: 2 (`/crowdsourcing/mensajes`, `/crowdsourcing/mensajes/:id`)

---

## 2. Estructura de Carpetas

La feature se integra dentro de `src/web/src/features/crowdsourcing/` siguiendo el patron
de la feature existente. El subdirectorio `mensajeria/` organiza todos los artefactos de US-CS-05.

```
src/web/src/
├── app/
│   └── router.tsx                                  [MODIFICAR] Agregar 2 rutas nuevas
│
├── components/
│   └── layout/
│       └── Header.tsx                              [MODIFICAR] Integrar NavbarMensajesIcon
│
└── features/
    └── crowdsourcing/
        ├── application/
        │   ├── hooks/
        │   │   ├── useConversaciones.ts            [NUEVO] Query hook - lista de conversaciones
        │   │   ├── useMensajes.ts                  [NUEVO] Query hook - mensajes de una conversacion
        │   │   ├── useNoLeidosCount.ts             [NUEVO] Query hook - badge navbar
        │   │   ├── useCreateConversacion.ts        [NUEVO] Mutation hook - crear conversacion
        │   │   ├── useEnviarMensaje.ts             [NUEVO] Mutation hook - enviar mensaje
        │   │   └── useMarcarLeidos.ts              [NUEVO] Mutation hook - marcar leidos al abrir chat
        │   └── index.ts                            [MODIFICAR] Agregar exports de los 6 hooks nuevos
        │
        ├── infrastructure/
        │   ├── api/
        │   │   ├── conversacion.api.ts             [NUEVO] API calls para conversaciones
        │   │   └── mensaje.api.ts                  [NUEVO] API calls para mensajes
        │   └── index.ts                            [MODIFICAR] Agregar exports de los 2 apis nuevos
        │
        └── presentation/
            ├── components/
            │   ├── NavbarMensajesIcon.tsx          [NUEVO] Icono con badge para Header
            │   ├── ConversacionList.tsx            [NUEVO] Lista paginada con tabs y empty state
            │   ├── ConversacionRow.tsx             [NUEVO] Fila individual de conversacion
            │   ├── ChatHeader.tsx                  [NUEVO] Cabecera de la vista de chat
            │   ├── ChatMessageList.tsx             [NUEVO] Contenedor de burbujas con scroll
            │   ├── MessageBubble.tsx               [NUEVO] Burbuja de mensaje individual
            │   ├── DateSeparator.tsx               [NUEVO] Separador de fecha entre mensajes
            │   ├── MessageInput.tsx                [NUEVO] Textarea + adjunto + boton enviar
            │   ├── IniciarConversacionDialog.tsx   [NUEVO] Dialog para crear conversacion
            │   ├── IniciarConversacionButton.tsx   [NUEVO] Boton contextual en necesidad/acuerdo
            │   ├── ConversacionLink.tsx            [MODIFICAR] Pasar a ser enlace real (Link)
            │   └── index.ts                        [MODIFICAR] Agregar exports nuevos
            │
            └── pages/
                ├── MensajesPage.tsx                [NUEVO] /crowdsourcing/mensajes
                ├── ConversacionChatPage.tsx         [NUEVO] /crowdsourcing/mensajes/:id
                └── index.ts                        [MODIFICAR] Agregar exports nuevos
```

---

## 3. Componentes

### 3.1 NavbarMensajesIcon

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/NavbarMensajesIcon.tsx`

**Proposito:** Icono de mensajeria en el navbar con badge numerico de mensajes no leidos. Visible
solo cuando el usuario esta autenticado. Navega a `/crowdsourcing/mensajes`.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `totalNoLeidos` | `number` | Si | Total de mensajes no leidos. Badge oculto si es 0 |

**Estado Local:** Ninguno. El componente es puramente de presentacion.

**Dependencias:**
- Hooks: Ninguno (recibe data via props desde `Header`)
- Componentes UI: `Link` (react-router-dom), `MessageSquare`, `span` para badge
- shadcn/ui: ninguno adicional

**Comportamiento:**
- Badge oculto con `hidden` cuando `totalNoLeidos === 0`
- Muestra "99+" cuando `totalNoLeidos > 99`
- `aria-label` en el badge: `"{n} mensajes no leidos"`
- Styles: `relative flex items-center gap-1.5 text-[#94a3b8] hover:text-white transition-colors text-sm font-medium`
- Badge styles: `absolute -top-1.5 -right-1.5 min-w-[1.125rem] h-[1.125rem] rounded-full bg-[#ec4899] text-white text-[10px] font-bold flex items-center justify-center px-1 leading-none`

---

### 3.2 ConversacionList

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ConversacionList.tsx`

**Proposito:** Lista completa de conversaciones con tabs de filtro, estado de carga, empty state
y boton "Cargar mas". Orquesta `useConversaciones` y renderiza `ConversacionRow` items.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `filtro` | `FiltroConversacion` | Si | Filtro activo: `'todas' \| 'necesidades' \| 'acuerdos'` |
| `onFiltroChange` | `(filtro: FiltroConversacion) => void` | Si | Callback al cambiar tab |

**Estado Local:**
- `page: number` - Pagina actual para paginacion (reseteada a 1 al cambiar filtro)
- `conversaciones: ConversacionListItem[]` - Acumulacion de items de todas las paginas cargadas

**Dependencias:**
- Hooks: `useConversaciones(filtro, page)`
- Componentes UI: `Tabs`, `TabsList`, `TabsTrigger` de shadcn/ui, `Skeleton`, `Alert`, `Button`
- Lucide: `MessageSquare` (empty state), `Loader2` (cargar mas), `AlertCircle` (error)
- Componentes propios: `ConversacionRow`

**Estados de UI:**
- Loading: 5 `Skeleton` de altura `h-20` con `space-y-3`
- Error: `Alert` con icono `AlertCircle` y boton "Reintentar"
- Empty: Icono `MessageSquare` opacidad 30% + texto "No tienes conversaciones activas"
- Default: Lista de `ConversacionRow` + boton "Cargar mas conversaciones" si `totalCount > items.length`

**Interacciones:**
- Click en tab: llama `onFiltroChange`, resetea `page` a 1 y limpia `conversaciones` acumuladas
- Click "Cargar mas": incrementa `page`, agrega resultados al array `conversaciones` (append, no replace)
- Polling: configurado en `useConversaciones` con `refetchInterval` (30 segundos segun ui-ux.md)

---

### 3.3 ConversacionRow

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ConversacionRow.tsx`

**Proposito:** Fila de conversacion individual en el listado. Muestra avatar, nombre de la otra parte,
contexto, preview del ultimo mensaje, timestamp relativo y badge de no leidos.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `conversacion` | `ConversacionListItem` | Si | Datos de la conversacion |
| `onClick` | `() => void` | Si | Callback al hacer click en la fila |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes UI: `Avatar`, `AvatarImage`, `AvatarFallback`, `Badge` de shadcn/ui
- Lucide: Ninguno
- Utilidades: Funcion `formatTimestamp(fechaUltimoMensaje?: string): string` - calcula tiempo relativo

**Comportamiento:**
- Fondo diferenciado: `bg-[#1e2a42]` si `mensajesNoLeidos > 0`, `bg-[#0f1729]` si 0
- Nombre de la otra parte: `font-semibold` si tiene no leidos, `font-medium` si no
- Badge de no leidos: visible solo si `mensajesNoLeidos > 0`
- Avatar fallback: primeras 2 iniciales del nombre en mayuscula, `bg-[#334155]`
- Titulo del contexto: texto en `text-[#a855f7]` con el nombre de la necesidad/acuerdo
- Preview del ultimo mensaje: truncado a 80 chars en UI si no lo hace el backend, `text-[#94a3b8]`
- Timestamp: `text-[#64748b]`, relativo (hoy: hora, ayer: "ayer", mas: fecha corta)
- Hover: `hover:bg-[#243347]` si tiene no leidos, `hover:bg-[#1e2a42]` si no

---

### 3.4 ChatHeader

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ChatHeader.tsx`

**Proposito:** Cabecera de la vista de chat. Muestra avatar y nombre de la otra parte, asunto de la
conversacion y contexto (necesidad o acuerdo) vinculado.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `nombreOtraParte` | `string` | Si | Nombre de la otra parte |
| `imagenOtraParte` | `string \| undefined` | No | URL de avatar de la otra parte |
| `asunto` | `string` | Si | Asunto de la conversacion |
| `contextoTipo` | `ContextoConversacion` | Si | `'necesidad' \| 'acuerdo'` |
| `contextoTitulo` | `string` | Si | Titulo de la necesidad o acuerdo |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes UI: `Avatar`, `AvatarImage`, `AvatarFallback` de shadcn/ui
- Lucide: Ninguno

**Comportamiento:**
- Card styles: `bg-[#0f1729] border border-[#334155] rounded-xl p-4 mb-4 flex items-start gap-3`
- Etiqueta de contexto: `text-xs text-[#64748b] uppercase tracking-wider` + "Necesidad:" o "Acuerdo:"
- Titulo del contexto: `text-sm text-[#a855f7] font-medium`
- Asunto: `text-sm text-[#94a3b8] italic truncate mt-0.5` con comillas alrededor

---

### 3.5 ChatMessageList

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ChatMessageList.tsx`

**Proposito:** Contenedor del area de mensajes. Maneja el scroll, auto-scroll al ultimo mensaje,
carga de paginas anteriores, separadores de fecha y renderiza `MessageBubble` items. Recibe
mensajes ya cargados desde el padre (`ConversacionChatPage`).

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `mensajes` | `Mensaje[]` | Si | Array de mensajes ordenados cronologicamente (ASC) |
| `isLoading` | `boolean` | Si | Estado de carga inicial |
| `hasMore` | `boolean` | Si | Hay paginas anteriores disponibles |
| `isLoadingMore` | `boolean` | Si | Se esta cargando pagina anterior |
| `onLoadMore` | `() => void` | Si | Callback para cargar pagina anterior |

**Estado Local:**
- `bottomRef: RefObject<HTMLDivElement>` - Referencia al div ancla al final del scroll
- Logica interna de auto-scroll: scroll al `bottomRef` cuando llegan mensajes nuevos via polling
  y el usuario esta cerca del final (scrollTop > scrollHeight - clientHeight - 100)

**Dependencias:**
- Componentes UI: `ScrollArea` de shadcn/ui, `Button`, `Loader2` (lucide)
- Componentes propios: `MessageBubble`, `DateSeparator`

**Comportamiento:**
- `aria-live="polite"` en el contenedor de mensajes para accesibilidad
- `aria-busy="true"` durante la carga inicial
- Altura: `h-[calc(100vh-320px)] min-h-[300px]` (desktop), `h-[calc(100vh-200px)]` (mobile)
- Auto-scroll al montar y cuando llegan mensajes nuevos (si usuario esta en el fondo)
- Boton "Cargar mensajes anteriores" visible en el top si `hasMore === true`
- Spinner cuando `isLoadingMore === true`
- Empty state: "Aun no hay mensajes. Escribe el primero." si `mensajes.length === 0 && !isLoading`
- Skeleton loading: 5 burbujas alternando izquierda/derecha si `isLoading === true`
- Nuevos mensajes aparecen con `animate-in fade-in-0 slide-in-from-bottom-2 duration-200`

---

### 3.6 MessageBubble

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MessageBubble.tsx`

**Proposito:** Burbuja de mensaje individual. Alineacion a la derecha para mensajes propios
(gradiente rosa-purpura) y a la izquierda para mensajes del interlocutor (fondo neutro).

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `mensaje` | `Mensaje` | Si | Datos completos del mensaje |
| `isOptimistic` | `boolean` | No | true si es un mensaje enviado en proceso (optimistic) |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes UI: `Avatar`, `AvatarImage`, `AvatarFallback` de shadcn/ui
- Lucide: `Paperclip` (icono adjunto), `AlertCircle` (error optimista)
- Utilidades: Funcion `formatMessageTime(fechaCreacion: string): string`

**Comportamiento:**
- Contenedor: `flex items-start gap-2 mb-3` + `flex-row-reverse` si `esPropio`
- Avatar: `w-8 h-8` con iniciales fallback
- Burbuja propio: `bg-gradient-to-br from-[#ec4899] to-[#a855f7] text-white text-sm px-4 py-3 rounded-[1rem_1rem_0.25rem_1rem]`
- Burbuja otro: `bg-[#16213e] border border-[#334155] text-[#e2e8f0] text-sm px-4 py-3 rounded-[1rem_1rem_1rem_0.25rem]`
- Nombre remitente (solo para mensajes ajenos): `text-xs text-[#64748b] mb-1 ml-1`
- Contenido: `<p className="break-words whitespace-pre-wrap">`
- URL adjunta: `<a>` con `target="_blank" rel="noopener noreferrer"`, icono `Paperclip`
- Timestamp: `text-xs text-[#64748b] mt-1`
- Si `isOptimistic === true`: mostrar "Enviando..." en lugar del timestamp
- Si `isOptimistic === true` y hubo error: mostrar icono `AlertCircle` rojo

---

### 3.7 DateSeparator

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/DateSeparator.tsx`

**Proposito:** Separador visual de fecha entre grupos de mensajes de dias diferentes.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `fecha` | `Date` | Si | Fecha del separador |

**Estado Local:** Ninguno.

**Dependencias:** Ninguna de shadcn/ui.

**Comportamiento:**
- Layout: `flex items-center gap-3 my-4`
- Lineas: `flex-1 h-px bg-[#334155]`
- Texto: `text-xs text-[#64748b] whitespace-nowrap`
- Formato: "1 de marzo de 2026" usando `toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })`
- Logica de insercion: el padre `ChatMessageList` inserta `DateSeparator` entre mensajes de fechas distintas

---

### 3.8 MessageInput

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MessageInput.tsx`

**Proposito:** Area de entrada de mensaje con textarea auto-resize, campo opcional de URL adjunta
y boton enviar. Maneja el formulario con React Hook Form + Zod (`createMensajeSchema`).

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `conversacionId` | `string` | Si | ID de la conversacion activa |
| `onMensajeEnviado` | `(mensaje: Mensaje) => void` | Si | Callback con el mensaje enviado (para actualizar lista optimisticamente) |

**Estado Local:**
- `showUrlInput: boolean` - Controla visibilidad del campo URL adjunta
- `charCount: number` - Numero de caracteres actuales (para mostrar contador si > 4000)

**Dependencias:**
- Hooks: `useEnviarMensaje(conversacionId)` (mutation hook)
- Formulario: `useForm<CreateMensajeFormData>` con `zodResolver(createMensajeSchema)` de `@shared/schemas`
- Componentes UI: `Textarea`, `Input`, `Button` de shadcn/ui
- Lucide: `Send`, `Paperclip`, `Loader2`

**Comportamiento:**
- Textarea: `resize-none min-h-[44px] max-h-[160px]`, auto-resize via `onInput` con `style.height`
- Enter (sin Shift) envia, Shift+Enter inserta salto de linea
- Boton Enviar: disabled si textarea vacio o `isPending === true`
- Boton Enviar en loading: `<Loader2 className="animate-spin w-4 h-4" />`
- Toggle URL: boton ghost con `Paperclip`, cambia texto a "Quitar adjunto" si visible
- Contador caracteres: visible solo si `charCount > 4000`, formato "{n}/5000"
- Error URL: `text-xs text-red-400 mt-1` debajo del input
- Al enviar exitoso: resetear form con `reset()`, ocultar campo URL, auto-focus en textarea
- Al enviar: llamar `onMensajeEnviado` con el mensaje retornado del backend

---

### 3.9 IniciarConversacionDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/IniciarConversacionDialog.tsx`

**Proposito:** Dialog modal para crear una nueva conversacion. Muestra informacion del destinatario
y del contexto (necesidad o acuerdo). Campo de asunto con validacion en tiempo real.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `isOpen` | `boolean` | Si | Controla visibilidad del dialog |
| `onClose` | `() => void` | Si | Callback para cerrar el dialog |
| `destinatarioId` | `string` | Si | UserId del destinatario |
| `destinatarioNombre` | `string` | Si | Nombre del destinatario para mostrar |
| `contextoId` | `string` | Si | ID de la necesidad o acuerdo |
| `contextoTipo` | `ContextoConversacion` | Si | `'necesidad' \| 'acuerdo'` |
| `contextoTitulo` | `string` | Si | Titulo de la necesidad o acuerdo |

**Estado Local:**
- `charCount: number` - Numero de caracteres del campo asunto para el contador en tiempo real

**Dependencias:**
- Hooks: `useCreateConversacion()` (mutation hook), `useNavigate` (react-router-dom)
- Formulario: `useForm<CreateConversacionFormData>` con `zodResolver(createConversacionSchema)` de `@shared/schemas`
- Componentes UI: `Dialog`, `DialogContent`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Input`, `Label`, `Button` de shadcn/ui
- Lucide: `Briefcase` (icono necesidad), `FileText` (icono acuerdo), `Loader2`

**Comportamiento:**
- Foco automatico en el campo asunto al abrir el dialog (`autoFocus`)
- Contador: "{n} / 200 caracteres" actualizado en tiempo real
- Boton "Iniciar conversacion": disabled durante submit
- Al submit exitoso: `toast.success`, cerrar dialog, navegar a `/crowdsourcing/mensajes/{id}`
- Error 4015 (conversacion duplicada): `toast.info` con CONVERSACION_DUPLICADA message, navegar a `/crowdsourcing/mensajes`
- Error 4016 (sin relacion): `toast.error` con CONVERSACION_NO_RELACION message, dialog permanece abierto
- Error generico: `toast.error`, dialog permanece abierto
- Al cerrar: resetear form con `reset()`
- El `CreateConversacionRequest` construido usa `necesidadId` o `acuerdoId` segun `contextoTipo`

---

### 3.10 IniciarConversacionButton

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/IniciarConversacionButton.tsx`

**Proposito:** Boton contextual que aparece en el detalle de necesidad o acuerdo. Muestra
"Iniciar conversacion" si no existe conversacion aun, o "Ver conversacion" si ya existe.
Encapsula `IniciarConversacionDialog` internamente.

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `destinatarioId` | `string` | Si | UserId del destinatario |
| `destinatarioNombre` | `string` | Si | Nombre del destinatario |
| `contextoId` | `string` | Si | ID de la necesidad o acuerdo |
| `contextoTipo` | `ContextoConversacion` | Si | `'necesidad' \| 'acuerdo'` |
| `contextoTitulo` | `string` | Si | Titulo del contexto |
| `conversacionExistenteId` | `string \| undefined` | No | Si existe, mostrar enlace en vez de boton |
| `mensajesNoLeidos` | `number \| undefined` | No | Para badge junto al enlace (desde acuerdo) |

**Estado Local:**
- `dialogOpen: boolean` - Controla apertura del `IniciarConversacionDialog`

**Dependencias:**
- Componentes propios: `IniciarConversacionDialog`
- Componentes UI: `Button` de shadcn/ui, `Link` (react-router-dom), `Badge` de shadcn/ui
- Lucide: `MessageSquare`

**Comportamiento:**
- Si `conversacionExistenteId` es definido: renderizar `Link` a `/crowdsourcing/mensajes/{id}` + badge si `mensajesNoLeidos > 0`
- Si `conversacionExistenteId` es undefined: renderizar boton que abre `IniciarConversacionDialog`
- Styles boton: `flex items-center gap-2 border border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white bg-transparent h-9 px-3 text-sm rounded-lg transition-colors`
- Styles enlace: `flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm transition-colors`

---

### 3.11 MensajesPage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/MensajesPage.tsx`

**Proposito:** Pagina del listado de conversaciones. Ruta: `/crowdsourcing/mensajes`.
Requiere autenticacion. Orquesta el filtro activo como estado local y renderiza `ConversacionList`.

**Props:** Ninguno (pagina raiz)

**Estado Local:**
- `filtroActivo: FiltroConversacion` - Estado del tab activo, default `FILTRO_CONVERSACION.TODAS`

**Dependencias:**
- Hooks: Ninguno directamente (los consume `ConversacionList`)
- Componentes propios: `ConversacionList`
- Componentes UI: `Badge` de shadcn/ui (para el total de no leidos en el encabezado)
- Hooks extra: `useConversaciones(filtroActivo, 1)` - solo para leer `totalNoLeidos` del encabezado,
  reutiliza el mismo query key que `ConversacionList` (sin refetch duplicado)

**Comportamiento:**
- Layout: `max-w-3xl mx-auto px-4 py-8` (desktop), `px-3 py-4` (mobile)
- Titulo "Mensajes" con `Badge` rosa del total de no leidos (oculto si 0)
- Redirigir a `/auth/login` si no autenticado (via guard en router o verificacion local)

---

### 3.12 ConversacionChatPage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/ConversacionChatPage.tsx`

**Proposito:** Pagina del chat de una conversacion especifica. Ruta: `/crowdsourcing/mensajes/:id`.
Requiere autenticacion. Orquesta carga de mensajes, polling, marcar leidos al montar, y acumula
mensajes de multiples paginas.

**Props:** Ninguno (recibe `id` via `useParams`)

**Estado Local:**
- `page: number` - Pagina actual (se incrementa al cargar mensajes anteriores)
- `mensajesAcumulados: Mensaje[]` - Array union de mensajes de todas las paginas cargadas (mas reciente = al final)
- `mensajesOptimistas: Mensaje[]` - Mensajes enviados pero pendientes de confirmacion del backend

**Dependencias:**
- Hooks: `useMensajes(id, page)`, `useMarcarLeidos(id)` (se llama al montar), `useNavigate`
- Params: `useParams<{ id: string }>()`
- Componentes propios: `ChatHeader`, `ChatMessageList`, `MessageInput`
- Componentes UI: `Link`, `ChevronLeft` (lucide) para "Volver a mensajes"

**Comportamiento:**
- Al montar: invocar `useMarcarLeidos` automaticamente (mutation sin UI de confirmacion)
- Al recibir nuevos mensajes del polling: agregar al final si el `id` no existe en `mensajesAcumulados`
- Al cargar pagina anterior: agregar los mensajes AL INICIO del array `mensajesAcumulados`
- Al enviar mensaje (via `onMensajeEnviado` de `MessageInput`): agregar a `mensajesAcumulados`
- Error 403: mostrar mensaje "No tienes acceso a esta conversacion" con link volver
- Error 404: mostrar mensaje "Conversacion no encontrada" con link volver
- `ChatHeader` recibe datos de la primera pagina de mensajes o de la conversacion del listado

---

### 3.13 ConversacionLink (modificacion)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ConversacionLink.tsx`

**Modificacion:** Actualmente es un `Button` disabled que no navega. Debe convertirse en un `Link`
real que navega a `/crowdsourcing/mensajes/{conversacionId}` cuando `conversacionId` esta definido.

**Props (sin cambio):**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `conversacionId` | `string \| undefined` | Si | ID de la conversacion |
| `className` | `string` | No | Clases adicionales |

**Cambio:** Si `conversacionId` es definido, renderizar `<Link to={...}>` con estilos de boton.
Si `conversacionId` es undefined, mantener el boton disabled actual.

---

## 4. Hooks

### 4.1 useConversaciones

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useConversaciones.ts`

**Tipo:** Query Hook (TanStack Query - `useQuery`)

**Descripcion:** Obtiene la lista paginada de conversaciones del usuario autenticado con filtro de
contexto. Implementa polling cada 30 segundos (segun ui-ux.md para el listado).

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `filtro` | `FiltroConversacion` | Filtro de contexto. Default `'todas'` |
| `page` | `number` | Numero de pagina. Default `1` |
| `pageSize` | `number` | Elementos por pagina. Default `20` |

**Retorna (campos de `useQuery`):**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `ConversacionListResponse \| undefined` | Lista paginada con `items`, `totalCount`, `totalNoLeidos` |
| `isLoading` | `boolean` | Carga inicial (sin datos previos en cache) |
| `isFetching` | `boolean` | Cualquier fetch en curso (incluido polling) |
| `error` | `Error \| null` | Error si la peticion falla |
| `refetch` | `function` | Forzar refetch manual |

**Query Key:** `QUERY_KEYS.crowdsourcing.conversaciones.lista(filtro)`

**Configuracion:**
- `queryFn`: `conversacionApi.getAll({ contexto: filtro, page, pageSize })`
- `staleTime`: `0` (siempre refrescar para reflejar badges actualizados)
- `refetchInterval`: `30_000` (30 segundos - para el listado segun ui-ux.md)
- `enabled`: Solo si el usuario esta autenticado (verificar con `useAuthStore`)

**Nota de invalidacion:** Este query es invalidado por `useMarcarLeidos` y por `useEnviarMensaje`.

---

### 4.2 useMensajes

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useMensajes.ts`

**Tipo:** Query Hook (TanStack Query - `useQuery`)

**Descripcion:** Obtiene los mensajes de una conversacion especifica, paginados cronologicamente
ascendente. Implementa polling cada 10 segundos (`MENSAJERIA_POLLING_INTERVAL_MS`).

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `conversacionId` | `string` | ID de la conversacion. Hook disabled si esta vacio |
| `page` | `number` | Pagina a cargar. Default `1` |
| `pageSize` | `number` | Elementos por pagina. Default `50` |

**Retorna (campos de `useQuery`):**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `MensajeListResponse \| undefined` | Lista paginada con `items`, `totalCount`, `page`, `pageSize` |
| `isLoading` | `boolean` | Carga inicial |
| `isFetching` | `boolean` | Fetch en curso (polling incluido) |
| `error` | `Error \| null` | Error (403, 404, 500) |

**Query Key:** `QUERY_KEYS.crowdsourcing.conversaciones.mensajes(conversacionId, page)`

**Configuracion:**
- `queryFn`: `mensajeApi.getByConversacion(conversacionId, { page, pageSize })`
- `staleTime`: `0`
- `refetchInterval`: `MENSAJERIA_POLLING_INTERVAL_MS` (10 segundos)
- `enabled`: `!!conversacionId`
- `retry`: `false` (para no reintentar errores 403/404)

---

### 4.3 useNoLeidosCount

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useNoLeidosCount.ts`

**Tipo:** Query Hook (TanStack Query - `useQuery`)

**Descripcion:** Obtiene el total de mensajes no leidos del usuario para el badge del navbar.
Endpoint ligero dedicado. Se usa exclusivamente desde `Header.tsx`.

**Parametros:** Ninguno

**Retorna (campos de `useQuery`):**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `NoLeidosCountResponse \| undefined` | Objeto con `totalNoLeidos: number` |
| `isLoading` | `boolean` | Carga inicial |

**Query Key:** `QUERY_KEYS.crowdsourcing.conversaciones.noLeidos`

**Configuracion:**
- `queryFn`: `conversacionApi.getNoLeidosCount()`
- `staleTime`: `0`
- `refetchInterval`: `60_000` (60 segundos - badge del navbar segun ui-ux.md)
- `enabled`: Solo si usuario autenticado (verificar con `useAuthStore`)

**Nota:** Invalidado por `useMarcarLeidos` al abrir un chat.

---

### 4.4 useCreateConversacion

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCreateConversacion.ts`

**Tipo:** Mutation Hook (TanStack Query - `useMutation`)

**Descripcion:** Crea una nueva conversacion. Maneja el caso especial del error 4015
(conversacion duplicada) redirigiendo al listado en lugar de mostrar error.

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreateConversacionRequest` | Body del POST con `necesidadId \| acuerdoId`, `userIdDestinatario`, `asunto` |

**Retorna (campos de `useMutation`):**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `function` | Ejecutar la mutacion |
| `mutateAsync` | `function` | Ejecutar y awaitar |
| `isPending` | `boolean` | Peticion en curso |
| `isError` | `boolean` | La mutacion fallo |
| `error` | `Error \| null` | Error de la peticion |

**Acciones `onSuccess`:**
- No invalida queries directamente (el componente hace la navegacion)
- Retorna `CreateConversacionResult` para que el componente use el `id`

**Acciones `onError`:**
- Si `error.message === '4015'`: NO mostrar toast de error; el componente (`IniciarConversacionDialog`)
  debe interpretar este error y mostrar `toast.info` + navegar al listado
- Otros errores: el componente muestra `toast.error` con `getMensajeriaErrorMessage(errorCode)`

**Nota de implementacion:** La mutation NO usa `onSuccess`/`onError` callbacks propios para toasts;
los callbacks de manejo de UI estan en `IniciarConversacionDialog` para mayor control.

---

### 4.5 useEnviarMensaje

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useEnviarMensaje.ts`

**Tipo:** Mutation Hook (TanStack Query - `useMutation`)

**Descripcion:** Envia un nuevo mensaje en una conversacion. Invalida las queries de mensajes
y del listado de conversaciones para reflejar el nuevo `FechaUltimoMensaje`.

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `conversacionId` | `string` | ID de la conversacion (parte de la URL, no del body) |
| `data` | `CreateMensajeRequest` | Body con `contenido` y `urlAdjunto?` |

**Retorna:** `Mensaje` (el mensaje creado con `esPropio: true`)

**Acciones `onSuccess`:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.conversaciones.mensajes(conversacionId, 1) })`
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.conversaciones.lista() })` - invalida todos los filtros
- No muestra toast (la UI actualiza visualmente con el mensaje retornado)

**Acciones `onError`:**
- `toast.error('No se pudo enviar el mensaje. Intenta de nuevo.')`
- El componente `ConversacionChatPage` retira el mensaje optimista de `mensajesOptimistas`

---

### 4.6 useMarcarLeidos

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useMarcarLeidos.ts`

**Tipo:** Mutation Hook (TanStack Query - `useMutation`)

**Descripcion:** Marca como leidos todos los mensajes no leidos del interlocutor en una conversacion.
Se invoca automaticamente al montar `ConversacionChatPage`. No muestra UI de confirmacion.

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `conversacionId` | `string` | ID de la conversacion |

**Retorna:** `MarcarLeidosResponse` con `mensajesMarcados: number`

**Acciones `onSuccess`:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.conversaciones.noLeidos })`
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.conversaciones.lista() })`

**Acciones `onError`:** No mostrar toast (operacion silenciosa; si falla no impacta el UX principal)

---

## 5. Services (API Layer)

### 5.1 conversacionApi

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/conversacion.api.ts`

**Patron:** Clase instanciada (igual que `acuerdoApi`, `necesidadPublicaApi`, etc.)

**Interface interna `ServiceResponse<T>`:** Importada o redeclarada localmente siguiendo el patron
del proyecto (`{ data: T; messages: Array<{ message: string; errorCode: string }> }`).

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getAll(params)` | `{ contexto?: string; page?: number; pageSize?: number }` | `Promise<ConversacionListResponse>` | `GET /api/crowdsourcing/conversaciones` |
| `create(data)` | `CreateConversacionRequest` | `Promise<CreateConversacionResult>` | `POST /api/crowdsourcing/conversaciones` |
| `getNoLeidosCount()` | - | `Promise<NoLeidosCountResponse>` | `GET /api/crowdsourcing/conversaciones/no-leidos` |

**Manejo de errores:**
- Si `!response.data`: leer `response.messages?.[0]?.errorCode ?? '5000'` y hacer `throw new Error(errorCode)`
- El hook captura el error y el componente lo interpreta segun el codigo

**Imports:**
```typescript
import { apiFetch } from '@/lib/api-client'
import { API_ROUTES } from '@shared/constants'
import type {
    CreateConversacionRequest,
    CreateConversacionResult,
    ConversacionListResponse,
    NoLeidosCountResponse,
} from '@shared/types/crowdsourcing'
```

---

### 5.2 mensajeApi

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/mensaje.api.ts`

**Patron:** Clase instanciada

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getByConversacion(id, params)` | `id: string`, `{ page?: number; pageSize?: number }` | `Promise<MensajeListResponse>` | `GET /api/crowdsourcing/conversaciones/{id}/mensajes` |
| `create(id, data)` | `id: string`, `CreateMensajeRequest` | `Promise<Mensaje>` | `POST /api/crowdsourcing/conversaciones/{id}/mensajes` |
| `marcarLeidos(id)` | `id: string` | `Promise<MarcarLeidosResponse>` | `PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos` |

**Imports:**
```typescript
import { apiFetch } from '@/lib/api-client'
import { API_ROUTES } from '@shared/constants'
import type {
    CreateMensajeRequest,
    Mensaje,
    MensajeListResponse,
    MarcarLeidosResponse,
} from '@shared/types/crowdsourcing'
```

---

## 6. Flujo de Datos

### 6.1 Flujo: Listado de Conversaciones

```
MensajesPage
    |
    ├── [estado local] filtroActivo
    |
    └── ConversacionList (recibe filtro)
            |
            ├── useConversaciones(filtro, page)
            |       |
            |       ├── conversacionApi.getAll(params)
            |       |       └── GET /api/crowdsourcing/conversaciones?contexto=...
            |       |
            |       └── polling: 30 segundos
            |
            └── [render] ConversacionRow[] + paginacion
```

### 6.2 Flujo: Vista de Chat

```
ConversacionChatPage (monta)
    |
    ├── useMarcarLeidos(id) → automaticamente al montar
    |       └── mensajeApi.marcarLeidos(id)
    |               └── PATCH /api/.../marcar-leidos
    |               └── onSuccess: invalida noLeidos + lista
    |
    ├── useMensajes(id, page=1)
    |       └── mensajeApi.getByConversacion(id)
    |               └── GET /api/.../mensajes
    |               └── polling: 10 segundos (MENSAJERIA_POLLING_INTERVAL_MS)
    |
    ├── ChatHeader (datos de la primera respuesta de mensajes o conversacion)
    |
    ├── ChatMessageList (mensajesAcumulados)
    |       └── MessageBubble[] + DateSeparator[]
    |
    └── MessageInput
            └── useEnviarMensaje(id)
                    └── mensajeApi.create(id, data)
                            └── POST /api/.../mensajes
                            └── onSuccess: invalida mensajes + lista
```

### 6.3 Flujo: Badge Navbar

```
Header (renderiza si isAuthenticated)
    |
    └── useNoLeidosCount()
            |
            ├── conversacionApi.getNoLeidosCount()
            |       └── GET /api/crowdsourcing/conversaciones/no-leidos
            |
            ├── polling: 60 segundos
            |
            └── NavbarMensajesIcon (totalNoLeidos)
```

### 6.4 Flujo: Crear Conversacion

```
IniciarConversacionButton
    |
    └── [estado] dialogOpen
            |
            └── IniciarConversacionDialog
                    |
                    ├── useCreateConversacion()
                    |
                    └── onSubmit → conversacionApi.create(data)
                                    └── POST /api/crowdsourcing/conversaciones
                                    |
                                    ├── onSuccess → navigate(/crowdsourcing/mensajes/{id})
                                    ├── error 4015 → toast.info + navigate(/crowdsourcing/mensajes)
                                    └── otros errores → toast.error
```

---

## 7. Integracion con Navbar Existente (Badge)

**Archivo a modificar:** `src/web/src/components/layout/Header.tsx`

**Cambios necesarios:**

1. Importar el hook `useNoLeidosCount` de la feature:
   ```typescript
   import { useNoLeidosCount } from '@/features/crowdsourcing/application/hooks/useNoLeidosCount'
   ```

2. Importar el componente `NavbarMensajesIcon`:
   ```typescript
   import { NavbarMensajesIcon } from '@/features/crowdsourcing/presentation/components/NavbarMensajesIcon'
   ```

3. Dentro del componente `Header`, llamar al hook condicionalmente:
   - Solo ejecutar el hook si `isAuthenticated === true` para no hacer peticiones sin token
   - Usar el resultado `data?.totalNoLeidos ?? 0`

4. Agregar `<NavbarMensajesIcon totalNoLeidos={...} />` dentro del bloque `isAuthenticated` del `<nav>`,
   antes de los botones de Dashboard y Cerrar sesion.

**Consideracion:** El hook `useNoLeidosCount` ya maneja `enabled: isAuthenticated` internamente,
pero el componente solo se renderiza dentro del bloque `{isAuthenticated && ...}`.

---

## 8. Rutas del Router a Agregar

**Archivo:** `src/web/src/app/router.tsx`

**Nuevas paginas a importar con `lazy()`:**
```typescript
const MensajesPage = lazy(() =>
    import('@/features/crowdsourcing/presentation/pages/MensajesPage')
)
const ConversacionChatPage = lazy(() =>
    import('@/features/crowdsourcing/presentation/pages/ConversacionChatPage')
)
```

**Nuevas `<Route>` a agregar:**

Las rutas de mensajeria requieren autenticacion. Actualmente el router no tiene un componente de
ruta protegida generico (las rutas de dashboard usan `DashboardLayout`). Las rutas de mensajeria
deben ir bajo `PublicLayout` con una guarda de autenticacion interna en la pagina (verificar
`isAuthenticated` de `useAuthStore` y redirigir con `useNavigate` si no autenticado).

```typescript
// Dentro del bloque <Route element={<PublicLayout />}>
<Route
    path="/crowdsourcing/mensajes"
    element={<MensajesPage />}
/>
<Route
    path="/crowdsourcing/mensajes/:id"
    element={<ConversacionChatPage />}
/>
```

**Constantes de rutas:** Agregar al archivo `src/web/src/lib/constants.ts` si existe un objeto
`ROUTES` (verificar si usa `ROUTES` de ese archivo o `APP_ROUTES` de `@shared`):
- El router actualmente usa `ROUTES` de `@/lib/constants` para las rutas conocidas
- Las nuevas rutas pueden usarse como strings literales o agregarse a `ROUTES` de `@/lib/constants`
- Tambien estan disponibles en `APP_ROUTES.landing.crowdsourcing.mensajes` de `@shared/constants`

---

## 9. Dependencias de Shared

**Importar de `@shared/types/crowdsourcing`:**
- `ConversacionListItem`, `ConversacionListResponse`
- `Mensaje`, `MensajeListResponse`
- `CreateConversacionRequest`, `CreateConversacionResult`
- `CreateMensajeRequest`
- `MarcarLeidosResponse`, `NoLeidosCountResponse`
- `ContextoConversacion`, `FiltroConversacion`

**Importar de `@shared/schemas/crowdsourcing.schema`:**
- `createConversacionSchema`, `CreateConversacionFormData`
- `createMensajeSchema`, `CreateMensajeFormData`

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdsourcing.conversaciones` (lista, mensajes, noLeidos)
- `API_ROUTES.crowdsourcing.conversaciones` (base, noLeidos, mensajes, marcarLeidos)
- `APP_ROUTES.landing.crowdsourcing.mensajes`, `mensajeDetail`
- `MENSAJERIA_POLLING_INTERVAL_MS`
- `FILTRO_CONVERSACION`

**Importar de `@shared/utils/error-messages`:**
- `getMensajeriaErrorMessage`
- `MENSAJERIA_ERROR_MESSAGES`

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdsourcing/infrastructure/api/conversacion.api.ts` | API Service | `conversacionApi` con `getAll`, `create`, `getNoLeidosCount` |
| `src/web/src/features/crowdsourcing/infrastructure/api/mensaje.api.ts` | API Service | `mensajeApi` con `getByConversacion`, `create`, `marcarLeidos` |
| `src/web/src/features/crowdsourcing/application/hooks/useConversaciones.ts` | Query Hook | Lista de conversaciones con polling 30s |
| `src/web/src/features/crowdsourcing/application/hooks/useMensajes.ts` | Query Hook | Mensajes de una conversacion con polling 10s |
| `src/web/src/features/crowdsourcing/application/hooks/useNoLeidosCount.ts` | Query Hook | Badge navbar con polling 60s |
| `src/web/src/features/crowdsourcing/application/hooks/useCreateConversacion.ts` | Mutation Hook | Crear conversacion, maneja error 4015 |
| `src/web/src/features/crowdsourcing/application/hooks/useEnviarMensaje.ts` | Mutation Hook | Enviar mensaje, invalida queries |
| `src/web/src/features/crowdsourcing/application/hooks/useMarcarLeidos.ts` | Mutation Hook | Marcar leidos, invalida noLeidos + lista |
| `src/web/src/features/crowdsourcing/presentation/components/NavbarMensajesIcon.tsx` | Component | Icono + badge para el navbar |
| `src/web/src/features/crowdsourcing/presentation/components/ConversacionList.tsx` | Component | Lista de conversaciones con tabs y paginacion |
| `src/web/src/features/crowdsourcing/presentation/components/ConversacionRow.tsx` | Component | Fila individual del listado |
| `src/web/src/features/crowdsourcing/presentation/components/ChatHeader.tsx` | Component | Cabecera del chat |
| `src/web/src/features/crowdsourcing/presentation/components/ChatMessageList.tsx` | Component | Area de mensajes con scroll |
| `src/web/src/features/crowdsourcing/presentation/components/MessageBubble.tsx` | Component | Burbuja de mensaje |
| `src/web/src/features/crowdsourcing/presentation/components/DateSeparator.tsx` | Component | Separador de fecha |
| `src/web/src/features/crowdsourcing/presentation/components/MessageInput.tsx` | Component | Input de mensaje con adjunto |
| `src/web/src/features/crowdsourcing/presentation/components/IniciarConversacionDialog.tsx` | Component | Dialog de crear conversacion |
| `src/web/src/features/crowdsourcing/presentation/components/IniciarConversacionButton.tsx` | Component | Boton contextual para necesidad/acuerdo |
| `src/web/src/features/crowdsourcing/presentation/pages/MensajesPage.tsx` | Page | `/crowdsourcing/mensajes` |
| `src/web/src/features/crowdsourcing/presentation/pages/ConversacionChatPage.tsx` | Page | `/crowdsourcing/mensajes/:id` |

---

## 11. Archivos a Modificar

| Archivo | Cambio | Descripcion |
|---------|--------|-------------|
| `src/web/src/app/router.tsx` | Agregar 2 rutas + 2 lazy imports | Rutas `/crowdsourcing/mensajes` y `/crowdsourcing/mensajes/:id` |
| `src/web/src/components/layout/Header.tsx` | Agregar `useNoLeidosCount` + `NavbarMensajesIcon` | Badge de mensajeria en el nav cuando autenticado |
| `src/web/src/features/crowdsourcing/infrastructure/index.ts` | Agregar 2 exports | `export { conversacionApi }` y `export { mensajeApi }` |
| `src/web/src/features/crowdsourcing/application/index.ts` | Agregar 6 exports | Los 6 hooks nuevos de mensajeria |
| `src/web/src/features/crowdsourcing/presentation/components/index.ts` | Agregar exports | Los 9 componentes nuevos + `IniciarConversacionButton` |
| `src/web/src/features/crowdsourcing/presentation/pages/index.ts` | Agregar 2 exports | `MensajesPage` y `ConversacionChatPage` |
| `src/web/src/features/crowdsourcing/presentation/components/ConversacionLink.tsx` | Convertir a `Link` real | Navegar a `/crowdsourcing/mensajes/{id}` cuando tiene `conversacionId` |

---

## 12. Componentes shadcn/ui Requeridos

Los siguientes componentes de shadcn/ui ya deben estar instalados (verificar en `src/web/src/components/ui/`):

| Componente | Archivo | Usado en |
|------------|---------|----------|
| `Tabs`, `TabsList`, `TabsTrigger` | `tabs.tsx` | `ConversacionList` (tabs de filtro) |
| `Avatar`, `AvatarImage`, `AvatarFallback` | `avatar.tsx` | `ConversacionRow`, `ChatHeader`, `MessageBubble` |
| `Badge` | `badge.tsx` | `ConversacionRow` (no leidos), `MensajesPage` (total), `IniciarConversacionButton` |
| `Skeleton` | `skeleton.tsx` | `ConversacionList`, `ConversacionChatPage` (loading states) |
| `Alert`, `AlertTitle`, `AlertDescription` | `alert.tsx` | `ConversacionList`, `ConversacionChatPage` (error states) |
| `Dialog`, `DialogContent`, `DialogTitle`, `DialogDescription`, `DialogFooter` | `dialog.tsx` | `IniciarConversacionDialog` |
| `Input` | `input.tsx` | `IniciarConversacionDialog` (asunto), `MessageInput` (URL adjunta) |
| `Textarea` | `textarea.tsx` | `MessageInput` |
| `Label` | `label.tsx` | `IniciarConversacionDialog`, `MessageInput` |
| `Button` | `button.tsx` | Todos los componentes interactivos |
| `ScrollArea` | *(verificar si existe)* | `ChatMessageList` |

**Nota:** Si `ScrollArea` no esta instalado: `npx shadcn-ui@latest add scroll-area`

---

## 13. Consideraciones de Implementacion

### Polling y Rendimiento

- `useConversaciones`: `refetchInterval: 30_000` (30 segundos para el listado, actualiza badges)
- `useMensajes`: `refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS` (10 segundos para mensajes activos)
- `useNoLeidosCount`: `refetchInterval: 60_000` (60 segundos para el badge del navbar)
- Todos los hooks de polling deben usar `refetchIntervalInBackground: false` para pausar el polling
  cuando la pestaña no esta activa y mejorar el rendimiento

### Mensajes Optimistas

`ConversacionChatPage` implementa un array `mensajesOptimistas: Mensaje[]` para mostrar el mensaje
inmediatamente al hacer click en Enviar. Si el servidor retorna error, el mensaje optimista se elimina
y se muestra `toast.error`. Si tiene exito, el mensaje real del servidor reemplaza al optimista
(la invalidacion del query trae la lista actualizada del servidor).

### Acumulacion de Mensajes (Paginacion)

Los mensajes se acumulan en `ConversacionChatPage` en un array local porque TanStack Query
no mantiene paginas anteriores acumuladas en un `useQuery` simple. La alternativa seria
`useInfiniteQuery`, pero dado el MVP y la complejidad de scroll inverso, se usa estado local.

Al cargar paginas anteriores (scroll hacia arriba):
1. Fetch de la pagina siguiente (numero mayor = mensajes mas antiguos)
2. Los mensajes obtenidos se PREPENDEN al inicio del array `mensajesAcumulados`
3. La posicion de scroll se mantiene con `scrollTop` ajustado antes y despues de agregar

### Formato de Timestamp Relativo

Funcion local `formatTimestamp(fechaUltimoMensaje?: string): string`:
- Undefined / null: string vacio
- Hoy: hora en formato HH:MM
- Ayer: "ayer"
- Mas antiguo: dd/MM (dia y mes abreviado)

Funcion local `formatMessageTime(fechaCreacion: string): string`:
- Siempre hora en formato HH:MM

Ambas funciones pueden vivir en un archivo de utilidades local de la feature:
`src/web/src/features/crowdsourcing/presentation/components/mensajeria.utils.ts`

### Autenticacion y Guards

Las paginas `MensajesPage` y `ConversacionChatPage` deben verificar autenticacion al montar:
```typescript
const { isAuthenticated } = useAuthStore()
const navigate = useNavigate()

useEffect(() => {
    if (!isAuthenticated) {
        navigate('/auth/login', { replace: true })
    }
}, [isAuthenticated, navigate])
```

Esto es consistente con el patron de otras paginas del proyecto que verifican autenticacion
localmente en lugar de usar un wrapper de ruta protegida.

---

## 14. Orden de Implementacion Recomendado

### Fase 1: Infraestructura (sin UI, sin hooks)
1. `conversacion.api.ts` - API calls de conversaciones
2. `mensaje.api.ts` - API calls de mensajes
3. Modificar `infrastructure/index.ts` para exportar los dos nuevos apis

### Fase 2: Hooks de Aplicacion
4. `useConversaciones.ts` - Query hook del listado
5. `useMensajes.ts` - Query hook del chat
6. `useNoLeidosCount.ts` - Query hook del badge
7. `useCreateConversacion.ts` - Mutation hook
8. `useEnviarMensaje.ts` - Mutation hook
9. `useMarcarLeidos.ts` - Mutation hook
10. Modificar `application/index.ts` para exportar los 6 hooks

### Fase 3: Componentes Atomicos (sin dependencias entre si)
11. `DateSeparator.tsx` - Mas simple, sin dependencias
12. `NavbarMensajesIcon.tsx` - Independiente, solo presentacion
13. `ConversacionRow.tsx` - Presentacion, recibe data via props
14. `MessageBubble.tsx` - Presentacion, recibe data via props
15. `ChatHeader.tsx` - Presentacion, recibe data via props

### Fase 4: Componentes Compuestos
16. `ChatMessageList.tsx` - Usa `MessageBubble` y `DateSeparator`
17. `MessageInput.tsx` - Usa `useEnviarMensaje`
18. `IniciarConversacionDialog.tsx` - Usa `useCreateConversacion`
19. `IniciarConversacionButton.tsx` - Usa `IniciarConversacionDialog`
20. `ConversacionList.tsx` - Usa `useConversaciones` y `ConversacionRow`

### Fase 5: Paginas
21. `MensajesPage.tsx` - Usa `ConversacionList`
22. `ConversacionChatPage.tsx` - Usa todos los componentes de chat

### Fase 6: Integraciones
23. Modificar `router.tsx` - Agregar las 2 rutas nuevas
24. Modificar `Header.tsx` - Agregar badge de mensajeria
25. Modificar `ConversacionLink.tsx` - Convertir a enlace funcional
26. Modificar `index.ts` de components y pages - Agregar exports
27. Verificar integracion con `NecesidadDetallePage` para `IniciarConversacionButton`

---

## 15. Checklist de Implementacion

### Componentes
- [ ] `NavbarMensajesIcon` usa `hidden` cuando `totalNoLeidos === 0`
- [ ] `NavbarMensajesIcon` muestra "99+" cuando `totalNoLeidos > 99`
- [ ] `ConversacionRow` diferencia visualmente filas con y sin mensajes no leidos
- [ ] `MessageBubble` alinea propios a la derecha y ajenos a la izquierda
- [ ] `MessageBubble` renderiza URL adjunta como enlace externo con `target="_blank"`
- [ ] `MessageInput` implementa Enter para enviar, Shift+Enter para salto de linea
- [ ] `MessageInput` muestra contador de caracteres solo cuando `charCount > 4000`
- [ ] `IniciarConversacionDialog` maneja error 4015 con redireccion (no toast de error)
- [ ] `IniciarConversacionButton` muestra enlace si `conversacionExistenteId` es definido
- [ ] `ChatMessageList` tiene `aria-live="polite"` para accesibilidad
- [ ] Todos los inputs tienen `<Label>` asociado con `htmlFor`

### Hooks
- [ ] `useConversaciones` usa `refetchInterval: 30_000`
- [ ] `useMensajes` usa `refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS`
- [ ] `useNoLeidosCount` usa `refetchInterval: 60_000`
- [ ] Todos los hooks de polling usan `refetchIntervalInBackground: false`
- [ ] `useMarcarLeidos` invalida `noLeidos` y `lista` en `onSuccess`
- [ ] `useEnviarMensaje` invalida `mensajes(conversacionId, 1)` y `lista()` en `onSuccess`
- [ ] Los hooks solo se habilitan cuando el usuario esta autenticado

### Services
- [ ] `conversacionApi` y `mensajeApi` siguen el patron de `acuerdoApi` (class instanciada)
- [ ] Errores lanzados como `new Error(errorCode)` con el codigo de error del backend
- [ ] Usar `API_ROUTES.crowdsourcing.conversaciones.*` para las URLs
- [ ] Importar types de `@shared/types/crowdsourcing`

### Router y Navegacion
- [ ] Rutas `/crowdsourcing/mensajes` y `/crowdsourcing/mensajes/:id` agregadas en `router.tsx`
- [ ] Ambas paginas tienen guard de autenticacion con redireccion a `/auth/login`
- [ ] `ConversacionLink.tsx` navega realmente cuando tiene `conversacionId`

### Shared
- [ ] Types importados de `@shared/types/crowdsourcing` (no duplicar en domain local)
- [ ] Schemas importados de `@shared/schemas/crowdsourcing.schema`
- [ ] Constants (`QUERY_KEYS`, `API_ROUTES`, `APP_ROUTES`) de `@shared/constants`
- [ ] `MENSAJERIA_POLLING_INTERVAL_MS` de `@shared/constants`
- [ ] `getMensajeriaErrorMessage` de `@shared/utils/error-messages`
- [ ] Sin uso de `any` en ningun archivo

### UX
- [ ] PATCH marcar-leidos se llama automaticamente al montar `ConversacionChatPage`
- [ ] Auto-scroll al ultimo mensaje al abrir el chat y al enviar un mensaje propio
- [ ] Nuevos mensajes por polling aparecen con `animate-in fade-in-0 slide-in-from-bottom-2`
- [ ] Empty state en el listado con icono `MessageSquare` opacidad 30%
- [ ] Loading skeleton en el listado (5 filas `h-20`)
- [ ] Loading skeleton en el chat (5 burbujas alternadas)
