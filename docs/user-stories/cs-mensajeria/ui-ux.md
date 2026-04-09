# UI/UX: Mensajeria entre Partes

> **Feature:** cs-mensajeria
> **Ultima actualizacion:** 2026-02-18

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Detail Campaign (referencia layout de detalle y sidebar) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |
| Explore Campaigns (referencia cards y listados) | [WPR_2-Excplore-Campaigns.png](../../ui-images/WPR_2-Excplore-Campaigns.png) | Landing |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue el wireframe textual definido en US-CS-05 y los patrones visuales de la landing publica existente (cards, dialogs, listas con badges de notificacion).

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Lista de conversaciones, vista de chat, dialog de nueva conversacion | `references/templates/krowd/` |

**Nota:** Esta feature pertenece exclusivamente a la Landing publica (`src/web`, Vite + React, puerto 3000), NO al dashboard admin.

---

## Design Tokens

### Paleta de Colores

```css
:root {
  /* Background - Dark theme */
  --bg-primary: #1a1a2e;
  --bg-secondary: #16213e;
  --bg-card: #0f1729;
  --bg-card-hover: #1e2a42;
  --bg-overlay: rgba(0, 0, 0, 0.6);

  /* Primary - Gradient pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;

  /* Status generales */
  --status-success: #10b981;
  --status-error: #ef4444;
  --status-warning: #f59e0b;
  --status-info: #3b82f6;

  /* Mensajeria - Burbujas */
  --msg-own-bg: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);  /* Propio: gradiente primario */
  --msg-other-bg: #16213e;                                           /* Otro: fondo de card secundario */
  --msg-own-text: #ffffff;
  --msg-other-text: #e2e8f0;

  /* Mensajeria - Indicadores */
  --unread-badge-bg: #ec4899;       /* Rosa primario para badge de no leidos */
  --unread-badge-text: #ffffff;
  --unread-row-highlight: #1e2a42;  /* Fondo ligeramente diferenciado para fila con no leidos */
  --unread-dot: #ec4899;            /* Punto de indicador no leido */

  /* Mensajeria - Timestamp */
  --timestamp-text: #64748b;        /* Gris muted para timestamps */

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Input */
  --input-bg: #0f1729;
  --input-bg-disabled: #1e293b;
  --input-border: #334155;
  --input-placeholder: #64748b;

  /* Destructive */
  --destructive: #dc2626;
  --destructive-hover: #b91c1c;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Font Sizes */
  --text-xs: 0.75rem;      /* 12px */
  --text-sm: 0.875rem;     /* 14px */
  --text-base: 1rem;       /* 16px */
  --text-lg: 1.125rem;     /* 18px */
  --text-xl: 1.25rem;      /* 20px */
  --text-2xl: 1.5rem;      /* 24px */

  /* Font Weights */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Line Heights */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
  --leading-relaxed: 1.75;
}
```

### Espaciado

```css
:root {
  --space-1: 0.25rem;   /* 4px */
  --space-2: 0.5rem;    /* 8px */
  --space-3: 0.75rem;   /* 12px */
  --space-4: 1rem;      /* 16px */
  --space-5: 1.25rem;   /* 20px */
  --space-6: 1.5rem;    /* 24px */
  --space-8: 2rem;      /* 32px */
  --space-10: 2.5rem;   /* 40px */
  --space-12: 3rem;     /* 48px */
  --space-16: 4rem;     /* 64px */
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;   /* 6px */
  --radius-md: 0.5rem;     /* 8px */
  --radius-lg: 0.75rem;    /* 12px */
  --radius-xl: 1rem;       /* 16px */
  --radius-full: 9999px;

  /* Burbuja de mensaje: esquina aplastada en el lado del remitente */
  --radius-bubble-own: 1rem 1rem 0.25rem 1rem;    /* top-left top-right bottom-right bottom-left */
  --radius-bubble-other: 1rem 1rem 1rem 0.25rem;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.3);
  --shadow-card-hover: 0 8px 24px rgba(0, 0, 0, 0.4);
}
```

---

## Pantalla 1: Lista de Conversaciones (/crowdsourcing/mensajes)

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/mensajes`
**Template base:** Krowd - listado con cards

### Layout Desktop

```
┌────────────────────────────────────────────────────────────┐
│  NAVBAR (global)                               [Mensajes 3]│
├────────────────────────────────────────────────────────────┤
│                                                            │
│  Mensajes                              [3 no leidos]       │
│                                                            │
│  [Todas]  [Necesidades]  [Acuerdos]                        │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ [Avatar]  Studio Mix Pro            [2]      15:30   │  │
│  │           Mezcla de pistas para EP                   │  │
│  │           "Perfecto, te envio los stems ma..."       │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ [Avatar]  Diseno Grafico Pro        [1]      10:00   │  │
│  │           Portada del album                          │  │
│  │           "He preparado 3 bocetos para que..."       │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ [Avatar]  Fotografo Madrid                    ayer   │  │
│  │           Sesion de fotos                            │  │
│  │           "Perfecto, confirmamos el sabado"          │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  [Cargar mas conversaciones]                               │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────────┐
│  NAVBAR                      [Msgs (3)] │
├─────────────────────────────────────────┤
│                                         │
│  Mensajes              [3 no leidos]    │
│                                         │
│  [Todas] [Necesidades] [Acuerdos]       │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │ [Av]  Studio Mix Pro     [2] 15:30│  │
│  │       Mezcla de pistas para EP    │  │
│  │       "Perfecto, te envio los..." │  │
│  └───────────────────────────────────┘  │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │ [Av]  Diseno Grafico Pro  [1] 10:0│  │
│  │       Portada del album           │  │
│  │       "He preparado 3 bocetos..." │  │
│  └───────────────────────────────────┘  │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │ [Av]  Fotografo Madrid      ayer  │  │
│  │       Sesion de fotos             │  │
│  │       "Confirmamos el sabado"     │  │
│  └───────────────────────────────────┘  │
│                                         │
│  [Cargar mas]                           │
│                                         │
└─────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de pagina | `<div>` | `max-w-3xl mx-auto px-4 py-8` |
| Encabezado de seccion | `<div>` | `flex items-center justify-between mb-6` |
| Titulo "Mensajes" | `<h1>` | `text-2xl font-bold text-white` |
| Badge total no leidos | `<Badge>` | `bg-[#ec4899] text-white rounded-full px-2 py-0.5 text-xs font-semibold` - oculto si totalNoLeidos === 0 |
| Tabs de filtro | `<Tabs>` | `defaultValue="todas" className="mb-4"` |
| Tab "Todas" | `<TabsTrigger value="todas">` | `data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8]` |
| Tab "Necesidades" | `<TabsTrigger value="necesidades">` | igual que anterior con `value="necesidades"` |
| Tab "Acuerdos" | `<TabsTrigger value="acuerdos">` | igual que anterior con `value="acuerdos"` |
| Lista de conversaciones | `<div>` | `space-y-3` |
| Fila de conversacion (con no leidos) | `<button>` | `w-full bg-[#1e2a42] border border-[#334155] rounded-xl p-4 flex items-start gap-3 hover:bg-[#243347] transition-colors cursor-pointer text-left` |
| Fila de conversacion (leida) | `<button>` | `w-full bg-[#0f1729] border border-[#334155] rounded-xl p-4 flex items-start gap-3 hover:bg-[#1e2a42] transition-colors cursor-pointer text-left` |
| Avatar de la otra parte | `<Avatar>` | `w-12 h-12 flex-shrink-0` |
| Avatar fallback (iniciales) | `<AvatarFallback>` | `bg-[#334155] text-white text-sm font-semibold` |
| Contenedor de texto de la fila | `<div>` | `flex-1 min-w-0` |
| Fila superior (nombre + badge + tiempo) | `<div>` | `flex items-center justify-between gap-2 mb-0.5` |
| Nombre de la otra parte (con no leidos) | `<span>` | `text-sm font-semibold text-white truncate` |
| Nombre de la otra parte (sin no leidos) | `<span>` | `text-sm font-medium text-[#e2e8f0] truncate` |
| Badge de no leidos | `<Badge>` | `bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold flex-shrink-0` - oculto si mensajesNoLeidos === 0 |
| Timestamp | `<span>` | `text-xs text-[#64748b] flex-shrink-0` |
| Titulo del contexto | `<p>` | `text-xs text-[#a855f7] font-medium truncate mb-0.5` - nombre de necesidad o acuerdo |
| Preview del ultimo mensaje | `<p>` | `text-sm text-[#94a3b8] truncate` - max 80 chars, sin texto si FechaUltimoMensaje es null |
| Estado empty | `<div>` | `text-center py-16 text-[#64748b]` |
| Icono empty | `<MessageSquare>` | Lucide, `w-12 h-12 mx-auto mb-3 opacity-30` |
| Texto empty | `<p>` | `text-base text-[#64748b]` "No tienes conversaciones activas" |
| Boton cargar mas | `<Button variant="outline">` | `w-full mt-4 border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42]` "Cargar mas conversaciones" - oculto si no hay mas paginas |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton de 5 filas: cada fila tiene `<Skeleton className="h-20 rounded-xl w-full bg-[#1e2a42]" />` con `space-y-3` |
| **Error API** | `<Alert>` con borde rojo, icono `<AlertCircle>`, texto "No se pudieron cargar las conversaciones. Intenta de nuevo." y boton "Reintentar" |
| **Empty** | Icono `<MessageSquare>` opacidad 30% + "No tienes conversaciones activas" centrado verticalmente en la lista |
| **Default** | Lista de conversaciones ordenada por FechaUltimoMensaje DESC. Conversaciones con mensajesNoLeidos > 0 tienen fondo `#1e2a42` |
| **Loading more** | Spinner `<Loader2 className="animate-spin" />` centrado debajo de la lista mientras se carga la siguiente pagina |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Click en fila de conversacion | Navegar a `/crowdsourcing/mensajes/{id}` |
| Cambio de tab (filtro) | Recargar lista con parametro `contexto` correspondiente. Resetear paginacion a pagina 1 |
| Click "Cargar mas" | Incrementar pagina, hacer GET paginado, agregar resultados al final de la lista (no reemplazar) |
| Hover en fila | Transicion de fondo en 150ms |
| Polling automatico | Refetch del listado cada 30 segundos para actualizar badges de no leidos y posicion en la lista |

---

## Pantalla 2: Vista de Chat (/crowdsourcing/mensajes/{id})

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/mensajes/:conversacionId`
**Template base:** Krowd - vista de detalle con layout fijo

### Layout Desktop

```
┌────────────────────────────────────────────────────────────┐
│  NAVBAR                                                    │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [<] Volver a mensajes                                     │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ CABECERA DE CONVERSACION                             │  │
│  │ [Avatar]  Studio Mix Pro                             │  │
│  │           "Consulta sobre la mezcla de pistas"       │  │
│  │           Necesidad: Mezcla de pistas para EP        │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ AREA DE MENSAJES (ScrollArea, altura fija)           │  │
│  │                                                      │  │
│  │  --- 1 de marzo de 2026 ---                          │  │
│  │                                                      │  │
│  │  [Av] Los Rockeros                        10:05      │  │
│  │  ┌────────────────────────────────────┐              │  │
│  │  │ Hola, me interesa tu propuesta.   │              │  │
│  │  │ Podrias contarme mas sobre tu     │              │  │
│  │  │ experiencia?                      │              │  │
│  │  └────────────────────────────────────┘              │  │
│  │                                                      │  │
│  │                 Studio Mix Pro [Av]  10:15           │  │
│  │         ┌────────────────────────────────────┐       │  │
│  │         │ Claro! He trabajado con bandas...  │       │  │
│  │         │ [Adjunto: drive.google.com/port...]│       │  │
│  │         └────────────────────────────────────┘       │  │
│  │                                                      │  │
│  │  [Av] Los Rockeros                        11:00      │  │
│  │  ┌────────────────────────────────────┐              │  │
│  │  │ Genial, me convence. Cuando        │              │  │
│  │  │ podrias empezar?                   │              │  │
│  │  └────────────────────────────────────┘              │  │
│  │                                                      │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ AREA DE ENTRADA                                      │  │
│  │ [Escribe un mensaje...                    ] [Enviar] │  │
│  │ [+ Adjuntar URL]                                     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────────┐
│  [<] Studio Mix Pro                     │
│  "Consulta sobre mezcla de pistas"      │
│  Necesidad: Mezcla de pistas para EP    │
├─────────────────────────────────────────┤
│  --- 1 de marzo de 2026 ---             │
│                                         │
│ [Av] Los Rockeros             10:05     │
│ ┌─────────────────────────────────────┐ │
│ │ Hola, me interesa tu propuesta.    │ │
│ │ Podrias contarme mas...            │ │
│ └─────────────────────────────────────┘ │
│                                         │
│         Studio Mix Pro [Av]    10:15    │
│   ┌─────────────────────────────────────┐│
│   │ Claro! He trabajado con bandas...  ││
│   │ [Adjunto: drive.google.com...]     ││
│   └─────────────────────────────────────┘│
│                                         │
├─────────────────────────────────────────┤
│ [Escribe un mensaje...       ] [Enviar] │
│ [+ Adjuntar URL]                        │
└─────────────────────────────────────────┘
```

### Especificaciones

#### Cabecera de Conversacion

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Enlace volver | `<Link>` | `flex items-center gap-2 text-[#94a3b8] hover:text-white text-sm mb-4 transition-colors` con `<ChevronLeft className="w-4 h-4" />` + "Volver a mensajes" |
| Card cabecera | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-4 mb-4 flex items-start gap-3` |
| Avatar de la otra parte | `<Avatar>` | `w-12 h-12 flex-shrink-0` |
| Avatar fallback | `<AvatarFallback>` | `bg-[#334155] text-white text-sm font-semibold` |
| Contenedor de info | `<div>` | `flex-1 min-w-0` |
| Nombre de la otra parte | `<h2>` | `text-lg font-semibold text-white truncate` |
| Asunto de la conversacion | `<p>` | `text-sm text-[#94a3b8] italic truncate mt-0.5` entre comillas |
| Etiqueta tipo de contexto | `<span>` | `text-xs text-[#64748b] uppercase tracking-wider` "Necesidad:" o "Acuerdo:" |
| Titulo de contexto | `<span>` | `text-sm text-[#a855f7] font-medium` - nombre de la necesidad o acuerdo |

#### Area de Mensajes

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de mensajes | `<ScrollArea>` | `h-[calc(100vh-320px)] min-h-[300px] bg-[#0f1729] border border-[#334155] rounded-xl p-4` |
| Boton "cargar mensajes anteriores" | `<Button variant="ghost">` | `w-full text-xs text-[#64748b] hover:text-[#94a3b8] mb-4` "Cargar mensajes anteriores" - visible solo si hay mas paginas |
| Loading mensajes anteriores | `<div>` | `flex justify-center mb-4` con `<Loader2 className="animate-spin w-4 h-4 text-[#64748b]" />` |
| Separador de fecha | `<div>` | `flex items-center gap-3 my-4` con `<div className="flex-1 h-px bg-[#334155]" />` + `<span className="text-xs text-[#64748b] whitespace-nowrap">` + `<div className="flex-1 h-px bg-[#334155]" />` |
| Grupo de mensaje (otro usuario) | `<div>` | `flex items-start gap-2 mb-3` |
| Grupo de mensaje (propio) | `<div>` | `flex items-start gap-2 mb-3 flex-row-reverse` |
| Avatar en mensaje | `<Avatar>` | `w-8 h-8 flex-shrink-0 mt-0.5` |
| Contenedor de burbuja + meta | `<div>` | `flex flex-col max-w-[70%]` - para otro: `items-start`, para propio: `items-end` |
| Nombre en burbuja (otro) | `<span>` | `text-xs text-[#64748b] mb-1 ml-1` |
| Burbuja mensaje (otro) | `<div>` | `bg-[#16213e] border border-[#334155] text-[#e2e8f0] text-sm px-4 py-3 rounded-[1rem_1rem_1rem_0.25rem] leading-relaxed` |
| Burbuja mensaje (propio) | `<div>` | `bg-gradient-to-br from-[#ec4899] to-[#a855f7] text-white text-sm px-4 py-3 rounded-[1rem_1rem_0.25rem_1rem] leading-relaxed` |
| Contenido del mensaje | `<p>` | `break-words whitespace-pre-wrap` |
| URL adjunta | `<a>` | `block mt-2 text-xs underline opacity-80 hover:opacity-100 truncate max-w-[200px]` href={urlAdjunto} target="_blank" rel="noopener noreferrer" - para propio: `text-white/80 hover:text-white`, para otro: `text-[#a855f7] hover:text-[#c084fc]` |
| Icono adjunto | `<Paperclip>` | Lucide, `w-3 h-3 inline mr-1` |
| Timestamp del mensaje | `<span>` | `text-xs text-[#64748b] mt-1` - para propio: `mr-1`, para otro: `ml-1` |
| Indicador "enviando" | `<span>` | `text-xs text-[#64748b]/60 mt-1 mr-1` "Enviando..." - solo para mensajes optimistas |
| Area de scroll bottom anchor | `<div ref={bottomRef}>` | `h-1` - elemento invisible al final de la lista para auto-scroll |

#### Area de Entrada de Mensaje

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor entrada | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-4 mt-4` |
| Fila principal de input | `<div>` | `flex items-end gap-3` |
| Textarea de mensaje | `<Textarea>` | `bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] resize-none min-h-[44px] max-h-[160px] focus:border-[#a855f7] flex-1 text-sm leading-relaxed` placeholder="Escribe un mensaje..." `rows={1}` con auto-resize |
| Boton enviar (activo) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-11 px-4 flex-shrink-0 transition-opacity` con `<Send className="w-4 h-4" />` aria-label="Enviar mensaje" |
| Boton enviar (disabled) | `<Button disabled>` | `opacity-40 cursor-not-allowed` - cuando textarea esta vacio o enviando |
| Boton enviar (enviando) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4" />` |
| Toggle URL adjunto | `<Button variant="ghost">` | `text-xs text-[#64748b] hover:text-[#94a3b8] h-8 px-2 mt-2 flex items-center gap-1` con `<Paperclip className="w-3.5 h-3.5" />` "Adjuntar URL" o "Quitar adjunto" segun estado |
| Campo URL (visible si toggle activo) | `<Input>` | `mt-2 bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] text-sm h-9 focus:border-[#a855f7]` placeholder="https://..." `type="url"` |
| Error URL | `<p>` | `text-xs text-red-400 mt-1` "Debe ser una URL valida (ej: https://...)" |
| Contador de caracteres | `<span>` | `text-xs text-[#64748b] text-right block mt-1` "{n}/5000" - visible solo cuando n > 4000 |
| Error max chars | `<p>` | `text-xs text-red-400 mt-1` "Maximo 5000 caracteres" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | Skeleton de cabecera (una fila `<Skeleton className="h-20 rounded-xl" />`) + skeleton de 5 burbujas alternando izquierda/derecha en el area de mensajes + area de entrada disabled |
| **Error carga** | `<Alert>` con borde rojo "No se pudo cargar la conversacion." con boton "Reintentar" |
| **Sin mensajes** | Area de mensajes con texto centrado "Aun no hay mensajes. Escribe el primero." en `text-[#64748b]` |
| **Default con mensajes** | Mensajes visibles, auto-scroll al ultimo, textarea habilitado |
| **Enviando mensaje** | Mensaje optimista aparece inmediatamente en la lista con indicador "Enviando...", boton enviar con spinner, textarea disabled |
| **Error al enviar** | Toast error "No se pudo enviar el mensaje. Intenta de nuevo." El mensaje optimista se retira o muestra icono de error `<AlertCircle className="w-3 h-3 text-red-400" />` junto al timestamp |
| **Mensaje enviado** | Indicador "Enviando..." cambia a timestamp real, campo se limpia |
| **Polling activo** | Sin indicador visual. Nuevos mensajes del otro usuario aparecen al final con transicion `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Contenido** | Minimo 1 caracter (sin espacios iniciales y finales) | El boton Enviar permanece disabled si el campo esta vacio o solo tiene espacios |
| **Contenido** | Maximo 5000 caracteres | "Maximo 5000 caracteres" mostrado debajo del textarea. Textarea con borde rojo `border-red-500` |
| **URL adjunta** | Si no esta vacia: debe pasar validacion de URL (startsWith http:// o https://) | "Debe ser una URL valida (ej: https://...)" debajo del campo |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Click "< Volver a mensajes" | Navegar a `/crowdsourcing/mensajes` |
| Escribir en textarea | Auto-resize del textarea hasta max-h-[160px], actualizar contador si > 4000 chars |
| Tecla Enter (sin Shift) en textarea | Enviar mensaje (equivalente a click en Enviar) |
| Tecla Shift+Enter en textarea | Insertar salto de linea, NO enviar |
| Click boton Enviar | Validar, mostrar mensaje optimista, llamar API, limpiar textarea |
| Click "Adjuntar URL" | Mostrar campo URL debajo del textarea |
| Click "Quitar adjunto" | Ocultar campo URL y limpiar su valor |
| Pegado de URL en campo URL | Validar en tiempo real al perder el foco (`onBlur`) |
| Abrir la pagina | PATCH automatico a `/marcar-leidos` al montar el componente, badges de no leidos en navbar se actualizan |
| Scroll hacia arriba | Cuando el usuario esta en el top del ScrollArea y hay mas paginas: mostrar boton "Cargar mensajes anteriores" |
| Click "Cargar mensajes anteriores" | Fetch de pagina anterior, agregar mensajes AL INICIO de la lista, mantener posicion de scroll actual |
| Polling automatico | Cada 10 segundos: GET mensajes pagina 1, si hay mensajes nuevos (id no conocido): agregar al final y auto-scroll si el usuario esta cerca del final (scroll < 100px desde el bottom) |
| Nuevo mensaje recibido via polling | `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |

---

## Pantalla 3: Badge de Mensajeria en Navbar (componente global)

**Proyecto:** Landing (`src/web`)
**Componente:** Actualizacion del Navbar existente
**Contexto:** Icono de mensajeria visible en la barra de navegacion principal cuando el usuario esta autenticado

### Layout

```
┌────────────────────────────────────────────────────────────┐
│  [WePlayRises]  [Campanias] [Crowdsourcing]  [Msgs (3)] [Perfil] │
└────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Enlace mensajeria en navbar | `<Link to="/crowdsourcing/mensajes">` | `relative flex items-center gap-1.5 text-[#94a3b8] hover:text-white transition-colors text-sm font-medium` |
| Icono de mensajeria | `<MessageSquare>` | Lucide, `w-5 h-5` |
| Badge de no leidos | `<span>` | `absolute -top-1.5 -right-1.5 min-w-[1.125rem] h-[1.125rem] rounded-full bg-[#ec4899] text-white text-[10px] font-bold flex items-center justify-center px-1 leading-none` - oculto con `hidden` cuando totalNoLeidos === 0 |
| Texto del badge | texto directo | Numero entero. Si totalNoLeidos > 99 mostrar "99+" |

### Estado del Badge

| Condicion | Visualizacion |
|-----------|--------------|
| totalNoLeidos === 0 | Badge oculto (`hidden`) |
| 1 - 99 no leidos | Badge rosa con el numero |
| > 99 no leidos | Badge rosa con "99+" |
| Usuario no autenticado | Icono no se muestra |

### Actualizacion del Conteo

El badge se actualiza mediante:
1. Fetch inicial al montar el navbar (cuando el usuario esta autenticado): GET `/api/crowdsourcing/conversaciones?pageSize=1` extrae `totalNoLeidos` de la respuesta
2. Refetch automatico cada 60 segundos mediante `useQuery` con `refetchInterval: 60000`
3. Invalidacion inmediata del query key `['conversaciones', 'noLeidos']` tras marcar como leidos al abrir una conversacion

---

## Pantalla 4: Dialog para Iniciar Conversacion

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay que aparece desde el detalle de una necesidad o desde un acuerdo
**Ruta de activacion:** Boton "Iniciar conversacion" en detalle de necesidad o acuerdo

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Iniciar conversacion                          [×]   │
│  Con: Studio Mix Pro                                 │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Contexto                                            │
│  ┌──────────────────────────────────────────────┐    │
│  │ [Icono]  Mezcla de pistas para EP            │    │
│  │          Necesidad de crowdsourcing          │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Asunto *                                            │
│  ┌──────────────────────────────────────────────┐    │
│  │  Consulta sobre la propuesta de mezcla       │    │
│  └──────────────────────────────────────────────┘    │
│  Breve descripcion del motivo de la conversacion     │
│  0 / 200 caracteres                                  │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                    [Iniciar conversacion]│
└──────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  Iniciar conversacion         [×]   │
│  Con: Studio Mix Pro                │
├─────────────────────────────────────┤
│  Contexto                           │
│  Mezcla de pistas para EP           │
│  Necesidad de crowdsourcing         │
│                                     │
│  Asunto *                           │
│  [Consulta sobre la propuesta...  ] │
│  0 / 200 caracteres                 │
│                                     │
│  [Cancelar] [Iniciar conversacion]  │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Container | `<Dialog>` | `open={isOpen} onOpenChange={onClose}` |
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Iniciar conversacion" |
| Subtitulo (nombre destinatario) | `<DialogDescription>` | `text-sm text-[#94a3b8] mt-0.5` "Con: {nombreDestinatario}" |
| Seccion contexto | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-3 mb-5 flex items-start gap-3` |
| Icono contexto (necesidad) | `<Briefcase>` | Lucide, `w-5 h-5 text-[#a855f7] flex-shrink-0 mt-0.5` |
| Icono contexto (acuerdo) | `<FileText>` | Lucide, `w-5 h-5 text-[#3b82f6] flex-shrink-0 mt-0.5` |
| Titulo del contexto | `<p>` | `text-sm font-medium text-white` |
| Tipo del contexto | `<p>` | `text-xs text-[#64748b] mt-0.5` "Necesidad de crowdsourcing" o "Acuerdo de crowdsourcing" |
| Label asunto | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Asterisco requerido | `<span>` | `text-red-400 ml-1` aria-hidden="true" |
| Input asunto | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7] placeholder:text-[#64748b]` placeholder="Breve descripcion del motivo..." |
| Hint asunto | `<p>` | `text-xs text-[#64748b] mt-1` "Breve descripcion del motivo de la conversacion" |
| Contador asunto | `<span>` | `text-xs text-[#64748b] text-right block` "{n} / 200 caracteres" |
| Error campo | `<p>` | `text-sm text-red-400 mt-1` con `role="alert"` |
| Dialog Footer | `<DialogFooter>` | `pt-4 border-t border-[#334155] flex justify-between gap-3` |
| Boton Cancelar | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Boton Iniciar | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` "Iniciar conversacion" |
| Boton Iniciar (loading) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4 mr-2" />` "Iniciando..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Campo asunto vacio, contador en 0/200, boton Iniciar habilitado |
| **Validation Error** | Borde rojo en campo asunto, mensaje "El asunto es obligatorio" o "Maximo 200 caracteres", boton disabled |
| **Submitting** | Spinner en boton + "Iniciando...", todos los inputs disabled |
| **Success** | Dialog cierra, toast success "Conversacion iniciada correctamente.", navegar a `/crowdsourcing/mensajes/{conversacionId}` |
| **Error - Conversacion ya existe (FA-01)** | Toast info "Ya existe una conversacion con esta persona para este contexto." + navegar a la conversacion existente |
| **Error - Sin relacion (FA-02)** | Toast error "No puedes iniciar una conversacion con este usuario sin una relacion previa." Dialog permanece abierto |
| **Error API generico** | Toast error "No se pudo iniciar la conversacion. Intenta de nuevo." Dialog permanece abierto |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Asunto** | Obligatorio, minimo 1 char (sin espacios) | "El asunto es obligatorio" |
| **Asunto** | Maximo 200 caracteres | "Maximo 200 caracteres" - borde rojo + mensaje |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Dialog abre | Campo asunto vacio con foco automatico |
| Click [×] o Cancelar | Cierra dialog, campo se resetea |
| Press Escape | Cierra dialog |
| Escribir en asunto | Actualizar contador en tiempo real |
| Click "Iniciar conversacion" | Validar, POST a API, navegar si exitoso |
| Tecla Enter en input | Equivalente a click en "Iniciar conversacion" |

---

## Pantalla 5: Puntos de Entrada - Boton "Iniciar Conversacion"

**Contexto:** Componentes que incluyen el boton o enlace para iniciar o ver una conversacion. Estos son elementos que se agregan a pantallas existentes (detalle de necesidad, detalle de acuerdo).

### Especificaciones

#### Desde Detalle de Necesidad (propuesta enviada)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Boton iniciar conversacion | `<Button>` | `flex items-center gap-2 border border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white bg-transparent h-9 px-3 text-sm rounded-lg transition-colors` con `<MessageSquare className="w-4 h-4" />` "Iniciar conversacion" |
| Enlace a conversacion existente | `<Link>` | `flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm transition-colors` con `<MessageSquare className="w-4 h-4" />` "Ver conversacion" |

**Regla de visualizacion:**
- Si el profesional no ha enviado propuesta: boton no visible
- Si el profesional envio propuesta y no hay conversacion: boton "Iniciar conversacion" abre el Dialog de Pantalla 4
- Si ya existe conversacion: enlace "Ver conversacion" navega directamente a `/crowdsourcing/mensajes/{id}`

#### Desde Detalle de Acuerdo

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Enlace de conversacion del acuerdo | `<Link>` | `flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm font-medium transition-colors` con `<MessageSquare className="w-4 h-4" />` "Ver conversacion del acuerdo" |
| Badge de no leidos (acuerdo) | `<Badge>` | `bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold ml-1` - oculto si no hay no leidos |

**Regla de visualizacion:**
- La conversacion de un acuerdo se crea automaticamente al aceptar la propuesta (US-CS-04)
- El enlace siempre esta visible para ambas partes del acuerdo

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 768px | Lista de conversaciones: padding reducido `px-3 py-4`, fila de conversacion compacta con nombre truncado mas agresivo. Vista de chat: cabecera condensada con solo nombre y flecha volver, area de mensajes ocupa casi toda la pantalla `h-[calc(100vh-200px)]`, textarea fijo en la parte inferior de la pantalla |
| **Tablet** | 768px - 1024px | Lista de conversaciones: `max-w-2xl mx-auto`. Vista de chat: `max-w-2xl mx-auto`, area de mensajes `h-[calc(100vh-280px)]` |
| **Desktop** | > 1024px | Lista de conversaciones: `max-w-3xl mx-auto`. Vista de chat: `max-w-3xl mx-auto`, area de mensajes `h-[calc(100vh-320px)]`. MVP: navegacion en una sola vista (no layout de dos columnas) |

**Nota MVP:** Para el MVP se mantiene navegacion de una sola vista en todos los breakpoints (lista separada de chat). El layout de dos columnas (lista + chat en paralelo) queda fuera del alcance del MVP.

---

## Animaciones

| Elemento | Animacion | Duracion | Clases Tailwind |
|----------|-----------|----------|-----------------|
| Hover en fila de conversacion | Cambio de fondo suave | 150ms | `transition-colors duration-150` |
| Hover en boton Enviar | Cambio de gradiente | 200ms | `transition-all duration-200` |
| Nuevo mensaje (polling) | Fade in + slide desde abajo | 200ms | `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |
| Dialog apertura | Fade in + scale | 150ms | Manejado por shadcn/ui Dialog por defecto |
| Badge de no leidos (aparicion) | Sin animacion especial | - | Solo visibilidad condicional |
| Skeleton loading | Pulso | 2s | `animate-pulse` (parte de shadcn Skeleton) |
| Spinner en botones | Rotacion continua | 1s | `animate-spin` |
| Textarea auto-resize | Sin transicion | - | Cambio de altura via JS, sin CSS transition para evitar saltos visuales |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de texto** | Texto principal `#ffffff` sobre `#0f1729`: ratio > 10:1. Texto secundario `#94a3b8` sobre `#0f1729`: ratio > 4.5:1. Texto en burbujas propias `#ffffff` sobre gradiente rosa-purpura: ratio > 4.5:1 |
| **Focus ring** | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` en todos los elementos interactivos |
| **Labels en inputs** | Todos los campos de formulario tienen `<Label>` asociado con `htmlFor` y el `id` del input correspondiente |
| **ARIA en badge no leidos (navbar)** | `<span aria-label="{n} mensajes no leidos" />` - el contenido numerico va dentro del span visible |
| **ARIA en badge de fila** | El badge numerico incluye `aria-label="{n} mensajes no leidos"` |
| **ARIA live region para mensajes nuevos** | El contenedor del area de mensajes tiene `aria-live="polite"` para que lectores de pantalla anuncien mensajes nuevos |
| **Role alert para errores** | Mensajes de error de validacion y toasts con `role="alert"` |
| **Boton enviar accesible** | `<Button aria-label="Enviar mensaje">` cuando el boton solo contiene icono |
| **Navegacion con teclado** | Tab entre elementos interactivos en orden logico. Enter en textarea envia, Shift+Enter inserta salto de linea |
| **Alt text en avatares** | `<AvatarImage alt="{nombre de la persona}" />` |
| **Indicador de estado de carga** | `aria-busy="true"` en el area de mensajes durante la carga inicial |

---

## Componentes Personalizados Necesarios

Los siguientes componentes personalizados deben crearse en `src/web/src/features/mensajeria/`:

| Componente | Descripcion | Props Interface |
|------------|-------------|-----------------|
| `ConversacionList` | Lista completa con filtros, paginacion y empty state | `{ filtro: 'todas' \| 'necesidades' \| 'acuerdos', onFiltroChange }` |
| `ConversacionRow` | Fila individual de conversacion | `{ conversacion: ConversacionListItemDto, onClick: () => void }` |
| `ChatView` | Vista completa de chat con scroll, polling y entrada | `{ conversacionId: string }` |
| `ChatHeader` | Cabecera de la vista de chat | `{ conversacion: ConversacionDetalleDto }` |
| `MessageBubble` | Burbuja individual de mensaje | `{ mensaje: MensajeDto, esPropio: boolean }` |
| `DateSeparator` | Separador de fecha entre mensajes | `{ fecha: Date }` |
| `MessageInput` | Area de entrada con textarea, URL adjunto y boton enviar | `{ conversacionId: string, onMensajeEnviado: (mensaje: MensajeDto) => void }` |
| `NavbarMensajesIcon` | Icono con badge para el navbar | `{ totalNoLeidos: number }` |
| `IniciarConversacionDialog` | Dialog para crear nueva conversacion | `{ isOpen: boolean, onClose: () => void, destinatarioId: string, destinatarioNombre: string, contextoId: string, contextoTipo: 'necesidad' \| 'acuerdo', contextoTitulo: string }` |
| `IniciarConversacionButton` | Boton/enlace contextual (desde necesidad o acuerdo) | `{ destinatarioId: string, destinatarioNombre: string, contextoId: string, contextoTipo: 'necesidad' \| 'acuerdo', contextoTitulo: string, conversacionExistenteId?: string }` |

---

## Tipos TypeScript de Referencia

```typescript
interface ConversacionListItemDto {
  id: string;
  asunto: string;
  nombreOtraParte: string;
  imagenOtraParte: string | null;
  contextoTipo: 'necesidad' | 'acuerdo';
  contextoTitulo: string;
  ultimoMensaje: string | null;
  fechaUltimoMensaje: string | null;  // ISO 8601
  mensajesNoLeidos: number;
}

interface MensajeDto {
  id: string;
  contenido: string;
  urlAdjunto: string | null;
  remitenteNombre: string;
  esPropio: boolean;
  leido: boolean;
  fechaCreacion: string;  // ISO 8601
}

interface ConversacionDetalleDto {
  id: string;
  asunto: string;
  nombreOtraParte: string;
  imagenOtraParte: string | null;
  contextoTipo: 'necesidad' | 'acuerdo';
  contextoTitulo: string;
}
```

---

## Checklist UI/UX

### Pantalla 1: Lista de Conversaciones
- [ ] Layout con `max-w-3xl mx-auto` implementado
- [ ] Tabs de filtro (Todas / Necesidades / Acuerdos) funcionales
- [ ] Fila de conversacion con avatar, nombre, contexto, preview y timestamp
- [ ] Badge de no leidos numerico en la fila
- [ ] Badge total de no leidos en el encabezado
- [ ] Fondo diferenciado (`#1e2a42`) para conversaciones con no leidos
- [ ] Timestamp relativo (hace 5 min, ayer, fecha) correctamente calculado
- [ ] Estado loading con skeleton de filas
- [ ] Estado error con boton reintentar
- [ ] Estado empty con icono y texto
- [ ] Paginacion con boton "Cargar mas"
- [ ] Polling cada 30 segundos implementado
- [ ] Responsive: compacto en mobile, padding ajustado
- [ ] Click en fila navega a la vista de chat

### Pantalla 2: Vista de Chat
- [ ] Cabecera con nombre, asunto y contexto
- [ ] Enlace "Volver a mensajes" funcional
- [ ] ScrollArea con altura calculada
- [ ] Mensajes propios a la derecha con gradiente rosa-purpura
- [ ] Mensajes del otro a la izquierda con fondo `#16213e`
- [ ] Separadores de fecha entre dias diferentes
- [ ] URL adjunta renderizada como enlace externo
- [ ] Avatar en cada mensaje
- [ ] Timestamp en cada mensaje
- [ ] Auto-scroll al ultimo mensaje al abrir y al enviar
- [ ] Textarea con auto-resize
- [ ] Enter envia, Shift+Enter inserta salto de linea
- [ ] Boton Enviar disabled cuando el textarea esta vacio
- [ ] Toggle de campo URL adjunta
- [ ] Validacion de URL en tiempo real
- [ ] Contador de caracteres visible cuando > 4000
- [ ] Mensaje optimista al enviar
- [ ] Estado de error al enviar con posibilidad de reintentar
- [ ] PATCH marcar-leidos al montar el componente
- [ ] Polling cada 10 segundos para nuevos mensajes
- [ ] "Cargar mensajes anteriores" con scroll hacia arriba
- [ ] Estado loading inicial con skeleton
- [ ] Responsive: full-width en mobile, area de mensajes ajustada

### Pantalla 3: Badge en Navbar
- [ ] Icono `<MessageSquare>` visible cuando usuario autenticado
- [ ] Badge rosa con numero cuando totalNoLeidos > 0
- [ ] Badge oculto cuando totalNoLeidos === 0
- [ ] "99+" cuando supera 99
- [ ] Refetch automatico cada 60 segundos
- [ ] Invalidacion inmediata tras marcar como leidos

### Pantalla 4: Dialog Iniciar Conversacion
- [ ] Dialog con informacion del destinatario y contexto
- [ ] Input de asunto con contador 0/200
- [ ] Validacion en tiempo real del asunto
- [ ] Estado submitting con spinner
- [ ] Manejo de conversacion ya existente (redireccion)
- [ ] Manejo de sin relacion (error informativo)
- [ ] Cierre con Escape y boton Cancelar
- [ ] Foco automatico en el campo asunto al abrir

### Pantalla 5: Puntos de Entrada
- [ ] Boton "Iniciar conversacion" visible en detalle de necesidad (cuando hay propuesta)
- [ ] Enlace "Ver conversacion" cuando ya existe una
- [ ] Enlace "Ver conversacion del acuerdo" en detalle de acuerdo
- [ ] Badge de no leidos junto al enlace de acuerdo
- [ ] Logica de visibilidad correcta segun estado de la relacion

### Accesibilidad General
- [ ] Todos los inputs tienen `<Label>` asociado
- [ ] Focus ring visible en todos los elementos interactivos
- [ ] `aria-live="polite"` en area de mensajes
- [ ] `role="alert"` en mensajes de error
- [ ] `aria-label` en badge de no leidos
- [ ] Alt text en avatares
- [ ] Contraste minimo 4.5:1 verificado
