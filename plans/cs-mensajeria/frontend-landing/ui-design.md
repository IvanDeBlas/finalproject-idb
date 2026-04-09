# Diseno UI: cs-mensajeria (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)
**Target:** src/web (Vite + React 18, puerto 3000)

---

## 1. Resumen

- Componentes shadcn instalados utilizados: 9 (Badge, Avatar, Tabs, Dialog, Alert, Button, Input, Textarea, Skeleton)
- Componente shadcn NO instalado y requerido: **ScrollArea** (debe instalarse con `npx shadcn@latest add scroll-area`)
- Composiciones custom: 10
- Responsive breakpoints: mobile (< 768px), tablet (768px - 1024px), desktop (> 1024px)
- Patron de dark theme establecido en el proyecto: bg `#0f1729`, texto `#ffffff`/`#94a3b8`, acento `#a855f7`

### Componentes shadcn instalados verificados en `src/web/src/components/ui/`

| Componente | Archivo | Estado |
|------------|---------|--------|
| `Badge` | `badge.tsx` | Instalado |
| `Avatar`, `AvatarImage`, `AvatarFallback` | `avatar.tsx` | Instalado |
| `Tabs`, `TabsList`, `TabsTrigger`, `TabsContent` | `tabs.tsx` | Instalado |
| `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter` | `dialog.tsx` | Instalado |
| `Alert`, `AlertTitle`, `AlertDescription` | `alert.tsx` | Instalado |
| `Button` | `button.tsx` | Instalado |
| `Input` | `input.tsx` | Instalado |
| `Textarea` | `textarea.tsx` | Instalado |
| `Skeleton` | `skeleton.tsx` | Instalado |
| `Label` | `label.tsx` | Instalado |
| `ScrollArea` | `scroll-area.tsx` | **NO INSTALADO - requiere: `npx shadcn@latest add scroll-area`** |

---

## 2. Paleta de Colores

| Uso | Variable CSS / Valor hex | Ejemplo de aplicacion |
|-----|--------------------------|-----------------------|
| Fondo primario | `#1a1a2e` | Fondo de pagina |
| Fondo secundario | `#16213e` | Burbuja mensaje ajeno, seccion contexto dialog |
| Fondo card | `#0f1729` | Cards, cabecera chat, area de mensajes, area entrada |
| Fondo card hover | `#1e2a42` | Fondo fila no-leida, hover en filas |
| Fondo hover alternativo | `#243347` | Hover especifico en filas no-leidas |
| Acento primario | `#a855f7` | Texto contexto, borde focus, titulos de contexto |
| Gradiente primario | `from-[#ec4899] to-[#a855f7]` | Burbujas propias, boton enviar, boton iniciar |
| Gradiente hover | `from-[#f472b6] to-[#c084fc]` | Hover del gradiente primario |
| Texto principal | `#ffffff` | Titulos, nombre de la otra parte en conv. no-leida |
| Texto secundario | `#e2e8f0` | Texto en burbujas ajenas |
| Texto label | `#94a3b8` | Nombres (conv. leida), textos secundarios, placeholder hover |
| Texto muted | `#64748b` | Timestamps, texto muted, placeholder inputs |
| Texto contexto | `#cbd5e1` | Labels de formulario |
| Badge no-leidos | `#ec4899` | Badge numerico de mensajes no leidos, badge navbar |
| Borde principal | `#334155` | Borders de cards, inputs, separadores |
| Borde focus | `#a855f7` | Ring focus en inputs |
| Texto error | `#ef4444` (alias `red-400`) | Mensajes de error, bordes en error |
| Exito | `#10b981` | Toasts de exito |
| Info | `#3b82f6` | Icono contexto tipo acuerdo |

---

## 3. Componentes por Pantalla

### 3.1 Pantalla 1: Lista de Conversaciones (`/crowdsourcing/mensajes`)

**Componente contenedor:** `MensajesPage.tsx`

#### Layout

```
Mobile (< 768px)               Desktop (> 1024px)
┌─────────────────────────┐    ┌────────────────────────────────────────┐
│ NAVBAR [Msgs badge]     │    │ NAVBAR                    [Msgs (3)]  │
├─────────────────────────┤    ├────────────────────────────────────────┤
│ px-3 py-4               │    │ max-w-3xl mx-auto px-4 py-8            │
│                         │    │                                        │
│ Mensajes   [badge 3]    │    │ Mensajes                  [badge 3]    │
│                         │    │                                        │
│ [Todas][Nec.][Acuerdos] │    │ [Todas]  [Necesidades]  [Acuerdos]     │
│                         │    │                                        │
│ ┌─────────────────────┐ │    │ ┌────────────────────────────────────┐ │
│ │[Av] Studio Mix [2]  │ │    │ │[Avatar] Studio Mix Pro   [2] 15:30 │ │
│ │     Mezcla EP       │ │    │ │         Mezcla de pistas para EP   │ │
│ │     "Perfecto, te.."│ │    │ │         "Perfecto, te envio los.." │ │
│ └─────────────────────┘ │    │ └────────────────────────────────────┘ │
│                         │    │                                        │
│ [Cargar mas]            │    │ [Cargar mas conversaciones]            │
└─────────────────────────┘    └────────────────────────────────────────┘
```

#### Componentes

**Contenedor de pagina y cabecera**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Wrapper pagina | `<div>` (nativo) | `max-w-3xl mx-auto px-4 py-8` (desktop) / `px-3 py-4` (mobile) |
| Fila de cabecera | `<div>` (nativo) | `flex items-center justify-between mb-6` |
| Titulo "Mensajes" | `<h1>` (nativo) | `text-2xl font-bold text-white` |
| Badge total no-leidos | `<Badge>` | `bg-[#ec4899] text-white rounded-full px-2 py-0.5 text-xs font-semibold` - clase `hidden` cuando `totalNoLeidos === 0` |

**Tabs de filtro**

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|-------------------------|
| Contenedor tabs | `<Tabs>` | `defaultValue="todas" className="mb-4"` |
| Lista de tabs | `<TabsList>` | `bg-[#1e2a42] border border-[#334155]` |
| Tab "Todas" | `<TabsTrigger value="todas">` | `data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm` |
| Tab "Necesidades" | `<TabsTrigger value="necesidades">` | Mismas clases que anterior |
| Tab "Acuerdos" | `<TabsTrigger value="acuerdos">` | Mismas clases que anterior |

**Composicion de Tabs:**

```tsx
<Tabs defaultValue="todas" className="mb-4">
    <TabsList className="bg-[#1e2a42] border border-[#334155]">
        <TabsTrigger
            value="todas"
            className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
        >
            Todas
        </TabsTrigger>
        <TabsTrigger
            value="necesidades"
            className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
        >
            Necesidades
        </TabsTrigger>
        <TabsTrigger
            value="acuerdos"
            className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
        >
            Acuerdos
        </TabsTrigger>
    </TabsList>
</Tabs>
```

**Lista de conversaciones (`ConversacionList`)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Lista | `<div>` (nativo) | `space-y-3` |

**Fila de conversacion (`ConversacionRow`)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Boton fila (no-leida) | `<button>` (nativo, `role="button"`) | `w-full bg-[#1e2a42] border border-[#334155] rounded-xl p-4 flex items-start gap-3 hover:bg-[#243347] transition-colors duration-150 cursor-pointer text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| Boton fila (leida) | `<button>` (nativo) | `w-full bg-[#0f1729] border border-[#334155] rounded-xl p-4 flex items-start gap-3 hover:bg-[#1e2a42] transition-colors duration-150 cursor-pointer text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| Avatar wrapper | `<Avatar>` | `w-12 h-12 flex-shrink-0` |
| Imagen avatar | `<AvatarImage>` | `alt="{nombreOtraParte}"` |
| Fallback avatar | `<AvatarFallback>` | `bg-[#334155] text-white text-sm font-semibold` - iniciales del nombre |
| Contenedor texto | `<div>` (nativo) | `flex-1 min-w-0` |
| Fila superior (nombre+badge+tiempo) | `<div>` (nativo) | `flex items-center justify-between gap-2 mb-0.5` |
| Nombre (no-leida) | `<span>` (nativo) | `text-sm font-semibold text-white truncate` |
| Nombre (leida) | `<span>` (nativo) | `text-sm font-medium text-[#e2e8f0] truncate` |
| Badge no-leidos fila | `<Badge>` | `bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold flex-shrink-0` - `aria-label="{n} mensajes no leidos"` - clase `hidden` cuando `mensajesNoLeidos === 0` |
| Timestamp | `<span>` (nativo) | `text-xs text-[#64748b] flex-shrink-0` |
| Titulo contexto | `<p>` (nativo) | `text-xs text-[#a855f7] font-medium truncate mb-0.5` |
| Preview ultimo mensaje | `<p>` (nativo) | `text-sm text-[#94a3b8] truncate` |

**Composicion de ConversacionRow:**

```tsx
<button
    className="w-full bg-[#1e2a42] border border-[#334155] rounded-xl p-4
               flex items-start gap-3 hover:bg-[#243347] transition-colors
               duration-150 cursor-pointer text-left
               focus-visible:outline-none focus-visible:ring-2
               focus-visible:ring-[#a855f7] focus-visible:ring-offset-2
               focus-visible:ring-offset-[#0f1729]"
    onClick={onClick}
>
    <Avatar className="w-12 h-12 flex-shrink-0">
        <AvatarImage src={conversacion.imagenOtraParte} alt={conversacion.nombreOtraParte} />
        <AvatarFallback className="bg-[#334155] text-white text-sm font-semibold">
            {conversacion.nombreOtraParte.slice(0, 2).toUpperCase()}
        </AvatarFallback>
    </Avatar>
    <div className="flex-1 min-w-0">
        <div className="flex items-center justify-between gap-2 mb-0.5">
            <span className="text-sm font-semibold text-white truncate">
                {conversacion.nombreOtraParte}
            </span>
            <div className="flex items-center gap-2 flex-shrink-0">
                {conversacion.mensajesNoLeidos > 0 && (
                    <Badge
                        className="bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold"
                        aria-label={`${conversacion.mensajesNoLeidos} mensajes no leidos`}
                    >
                        {conversacion.mensajesNoLeidos}
                    </Badge>
                )}
                <span className="text-xs text-[#64748b]">{timestampRelativo}</span>
            </div>
        </div>
        <p className="text-xs text-[#a855f7] font-medium truncate mb-0.5">
            {conversacion.contextoTitulo}
        </p>
        {conversacion.ultimoMensaje && (
            <p className="text-sm text-[#94a3b8] truncate">{conversacion.ultimoMensaje}</p>
        )}
    </div>
</button>
```

**Boton "Cargar mas"**

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|-------------------------|
| Boton paginacion | `<Button variant="outline">` | `w-full mt-4 border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white` - oculto si no hay mas paginas |

---

### 3.2 Pantalla 2: Vista de Chat (`/crowdsourcing/mensajes/:conversacionId`)

**Componente contenedor:** `ChatView.tsx`

#### Layout

```
Mobile (< 768px)               Desktop (> 1024px)
┌─────────────────────────┐    ┌────────────────────────────────────────┐
│ [<] Studio Mix Pro      │    │ [<] Volver a mensajes                  │
│ "Consulta sobre mezcla" │    │                                        │
│ Necesidad: Mezcla EP    │    │ ┌──────────────────────────────────┐   │
├─────────────────────────┤    │ │ [Avatar]  Studio Mix Pro         │   │
│ --- 1 mar 2026 ---      │    │ │           "Consulta sobre..."    │   │
│                         │    │ │  Necesidad: Mezcla EP            │   │
│ [Av] Los Rockeros 10:05 │    │ └──────────────────────────────────┘   │
│ ┌───────────────────┐   │    │                                        │
│ │ Hola, me interesa │   │    │ ┌──────────────────────────────────┐   │
│ └───────────────────┘   │    │ │ ScrollArea h-[calc(100vh-320px)] │   │
│                         │    │ │                                  │   │
│    Studio Mix [Av] 10:15│    │ │  --- 1 mar 2026 ---              │   │
│  ┌───────────────────┐  │    │ │                                  │   │
│  │ Claro! He trabaj. │  │    │ │  [Av] Los Rockeros       10:05   │   │
│  └───────────────────┘  │    │ │  ┌──────────────────────┐       │   │
├─────────────────────────┤    │ │  │ Hola, me interesa... │       │   │
│ [Escribe...    ][Enviar]│    │ │  └──────────────────────┘       │   │
│ [+ Adjuntar URL]        │    │ └──────────────────────────────────┘   │
└─────────────────────────┘    │                                        │
                               │ ┌──────────────────────────────────┐   │
                               │ │ [Escribe un mensaje...  ][Enviar]│   │
                               │ │ [+ Adjuntar URL]                 │   │
                               │ └──────────────────────────────────┘   │
                               └────────────────────────────────────────┘
```

#### Subcomponentes de la Vista de Chat

**Enlace volver (`ChatView` interior)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Enlace volver | `<Link>` (react-router-dom) | `flex items-center gap-2 text-[#94a3b8] hover:text-white text-sm mb-4 transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]` |
| Icono volver | `<ChevronLeft>` (Lucide) | `w-4 h-4` |

**Cabecera de conversacion (`ChatHeader`)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Card cabecera | `<div>` (nativo) | `bg-[#0f1729] border border-[#334155] rounded-xl p-4 mb-4 flex items-start gap-3` |
| Avatar | `<Avatar>` | `w-12 h-12 flex-shrink-0` |
| Imagen avatar | `<AvatarImage>` | `alt="{nombreOtraParte}"` |
| Fallback avatar | `<AvatarFallback>` | `bg-[#334155] text-white text-sm font-semibold` |
| Contenedor info | `<div>` (nativo) | `flex-1 min-w-0` |
| Nombre otra parte | `<h2>` (nativo) | `text-lg font-semibold text-white truncate` |
| Asunto entre comillas | `<p>` (nativo) | `text-sm text-[#94a3b8] italic truncate mt-0.5` |
| Fila contexto | `<div>` (nativo) | `flex items-center gap-1 mt-1` |
| Etiqueta tipo | `<span>` (nativo) | `text-xs text-[#64748b] uppercase tracking-wider` ("Necesidad:" o "Acuerdo:") |
| Titulo contexto | `<span>` (nativo) | `text-sm text-[#a855f7] font-medium` |

**Area de mensajes con scroll (`ScrollArea` - DEBE INSTALARSE)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| ScrollArea | `<ScrollArea>` | `h-[calc(100vh-320px)] min-h-[300px] bg-[#0f1729] border border-[#334155] rounded-xl p-4` - mobile: `h-[calc(100vh-200px)]`, tablet: `h-[calc(100vh-280px)]` |
| Boton cargar anteriores | `<Button variant="ghost">` | `w-full text-xs text-[#64748b] hover:text-[#94a3b8] mb-4` |
| Spinner cargando anteriores | `<Loader2>` (Lucide) | `animate-spin w-4 h-4 text-[#64748b]` dentro de `flex justify-center mb-4` |
| Anchor scroll bottom | `<div ref={bottomRef}>` | `h-1` (invisible) |
| Contenedor aria-live | `<div>` (nativo) | `aria-live="polite" aria-busy={isLoading}` |

**Separador de fecha (`DateSeparator`)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Contenedor | `<div>` (nativo) | `flex items-center gap-3 my-4` |
| Linea izquierda | `<div>` (nativo) | `flex-1 h-px bg-[#334155]` |
| Texto fecha | `<span>` (nativo) | `text-xs text-[#64748b] whitespace-nowrap` |
| Linea derecha | `<div>` (nativo) | `flex-1 h-px bg-[#334155]` |

**Burbuja de mensaje (`MessageBubble`)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Grupo (mensaje ajeno) | `<div>` (nativo) | `flex items-start gap-2 mb-3` |
| Grupo (mensaje propio) | `<div>` (nativo) | `flex items-start gap-2 mb-3 flex-row-reverse` |
| Avatar en mensaje | `<Avatar>` | `w-8 h-8 flex-shrink-0 mt-0.5` |
| Contenedor burbuja+meta | `<div>` (nativo) | `flex flex-col max-w-[70%] items-start` (ajeno) / `items-end` (propio) |
| Nombre remitente (ajeno) | `<span>` (nativo) | `text-xs text-[#64748b] mb-1 ml-1` |
| Burbuja (ajeno) | `<div>` (nativo) | `bg-[#16213e] border border-[#334155] text-[#e2e8f0] text-sm px-4 py-3 rounded-[1rem_1rem_1rem_0.25rem] leading-relaxed` |
| Burbuja (propio) | `<div>` (nativo) | `bg-gradient-to-br from-[#ec4899] to-[#a855f7] text-white text-sm px-4 py-3 rounded-[1rem_1rem_0.25rem_1rem] leading-relaxed` |
| Contenido del mensaje | `<p>` (nativo) | `break-words whitespace-pre-wrap` |
| URL adjunta (propio) | `<a>` (nativo) | `block mt-2 text-xs underline opacity-80 hover:opacity-100 truncate max-w-[200px] text-white/80 hover:text-white` con `target="_blank" rel="noopener noreferrer"` |
| URL adjunta (ajeno) | `<a>` (nativo) | `block mt-2 text-xs underline opacity-80 hover:opacity-100 truncate max-w-[200px] text-[#a855f7] hover:text-[#c084fc]` con `target="_blank" rel="noopener noreferrer"` |
| Icono adjunto | `<Paperclip>` (Lucide) | `w-3 h-3 inline mr-1` |
| Timestamp (propio) | `<span>` (nativo) | `text-xs text-[#64748b] mt-1 mr-1` |
| Timestamp (ajeno) | `<span>` (nativo) | `text-xs text-[#64748b] mt-1 ml-1` |
| Indicador "Enviando..." | `<span>` (nativo) | `text-xs text-[#64748b]/60 mt-1 mr-1` - solo para mensajes optimistas |
| Icono error envio | `<AlertCircle>` (Lucide) | `w-3 h-3 text-red-400 inline` junto al timestamp |

**Animacion para mensajes nuevos recibidos via polling:**

```
className="animate-in fade-in-0 slide-in-from-bottom-2 duration-200"
```

**Composicion de MessageBubble (ejemplo mensaje ajeno):**

```tsx
<div className="flex items-start gap-2 mb-3">
    <Avatar className="w-8 h-8 flex-shrink-0 mt-0.5">
        <AvatarImage src={imagenRemitente} alt={mensaje.remitenteNombre} />
        <AvatarFallback className="bg-[#334155] text-white text-xs font-semibold">
            {mensaje.remitenteNombre.slice(0, 2).toUpperCase()}
        </AvatarFallback>
    </Avatar>
    <div className="flex flex-col max-w-[70%] items-start">
        <span className="text-xs text-[#64748b] mb-1 ml-1">{mensaje.remitenteNombre}</span>
        <div className="bg-[#16213e] border border-[#334155] text-[#e2e8f0] text-sm px-4 py-3 rounded-[1rem_1rem_1rem_0.25rem] leading-relaxed">
            <p className="break-words whitespace-pre-wrap">{mensaje.contenido}</p>
            {mensaje.urlAdjunto && (
                <a
                    href={mensaje.urlAdjunto}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="block mt-2 text-xs underline opacity-80 hover:opacity-100 truncate max-w-[200px] text-[#a855f7] hover:text-[#c084fc]"
                >
                    <Paperclip className="w-3 h-3 inline mr-1" />
                    {mensaje.urlAdjunto}
                </a>
            )}
        </div>
        <span className="text-xs text-[#64748b] mt-1 ml-1">{timestampFormateado}</span>
    </div>
</div>
```

**Area de entrada de mensaje (`MessageInput`)**

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|-------------------------|
| Contenedor entrada | `<div>` (nativo) | `bg-[#0f1729] border border-[#334155] rounded-xl p-4 mt-4` |
| Fila principal | `<div>` (nativo) | `flex items-end gap-3` |
| Textarea | `<Textarea>` | `bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] resize-none min-h-[44px] max-h-[160px] focus:border-[#a855f7] focus-visible:ring-[#a855f7] flex-1 text-sm leading-relaxed` placeholder="Escribe un mensaje..." `rows={1}` |
| Boton enviar (activo) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-11 px-4 flex-shrink-0 transition-all duration-200` `aria-label="Enviar mensaje"` |
| Icono enviar | `<Send>` (Lucide) | `w-4 h-4` |
| Boton enviar (disabled) | `<Button disabled>` | clase `opacity-40 cursor-not-allowed` - cuando textarea vacio o enviando |
| Boton enviar (enviando) | `<Button disabled>` | `<Loader2 className="animate-spin w-4 h-4" />` |
| Toggle adjunto URL | `<Button variant="ghost">` | `text-xs text-[#64748b] hover:text-[#94a3b8] h-8 px-2 mt-2 flex items-center gap-1` |
| Icono toggle adjunto | `<Paperclip>` (Lucide) | `w-3.5 h-3.5` |
| Campo URL (condicional) | `<Input>` | `mt-2 bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] text-sm h-9 focus:border-[#a855f7] focus-visible:ring-[#a855f7]` `type="url"` placeholder="https://..." |
| Label URL (accesibilidad) | `<Label>` | clase `sr-only` (solo screen readers) |
| Error URL | `<p>` (nativo) | `text-xs text-red-400 mt-1` `role="alert"` |
| Contador caracteres | `<span>` (nativo) | `text-xs text-[#64748b] text-right block mt-1` - visible solo cuando `contenido.length > 4000` |
| Textarea error max | Estado de Textarea | `border-red-500 focus:border-red-500` cuando `contenido.length > 5000` |
| Error max chars | `<p>` (nativo) | `text-xs text-red-400 mt-1` `role="alert"` |

**Composicion de MessageInput:**

```tsx
<div className="bg-[#0f1729] border border-[#334155] rounded-xl p-4 mt-4">
    <div className="flex items-end gap-3">
        <Textarea
            id="mensaje-input"
            placeholder="Escribe un mensaje..."
            rows={1}
            className="bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b]
                       resize-none min-h-[44px] max-h-[160px] focus:border-[#a855f7]
                       focus-visible:ring-[#a855f7] flex-1 text-sm leading-relaxed"
            aria-label="Escribe un mensaje"
        />
        <Button
            aria-label="Enviar mensaje"
            disabled={contenidoVacio || enviando}
            className="bg-gradient-to-r from-pink-500 to-purple-600
                       hover:from-pink-600 hover:to-purple-700
                       h-11 px-4 flex-shrink-0 transition-all duration-200
                       disabled:opacity-40 disabled:cursor-not-allowed"
        >
            {enviando ? <Loader2 className="animate-spin w-4 h-4" /> : <Send className="w-4 h-4" />}
        </Button>
    </div>
    {mostrarUrlAdjunto && (
        <>
            <Label htmlFor="url-adjunto" className="sr-only">URL adjunta</Label>
            <Input
                id="url-adjunto"
                type="url"
                placeholder="https://..."
                className="mt-2 bg-[#16213e] border-[#334155] text-white
                           placeholder:text-[#64748b] text-sm h-9 focus:border-[#a855f7]
                           focus-visible:ring-[#a855f7]"
            />
            {errorUrl && (
                <p role="alert" className="text-xs text-red-400 mt-1">
                    Debe ser una URL valida (ej: https://...)
                </p>
            )}
        </>
    )}
    <Button
        variant="ghost"
        className="text-xs text-[#64748b] hover:text-[#94a3b8] h-8 px-2 mt-2 flex items-center gap-1"
    >
        <Paperclip className="w-3.5 h-3.5" />
        {mostrarUrlAdjunto ? 'Quitar adjunto' : 'Adjuntar URL'}
    </Button>
    {contenido.length > 4000 && (
        <span className="text-xs text-[#64748b] text-right block mt-1">
            {contenido.length}/5000
        </span>
    )}
</div>
```

---

### 3.3 Pantalla 3: Badge de Mensajeria en Navbar (`NavbarMensajesIcon`)

#### Layout

```
┌──────────────────────────────────────────────────────────┐
│ [WePlayRises] [Campanias] [Crowdsourcing] [Msgs (3)] [Perfil] │
└──────────────────────────────────────────────────────────┘
                                            ↑
                                   Icono con badge posicionado
                                   relativamente sobre el icono
```

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Enlace wrapper | `<Link to="/crowdsourcing/mensajes">` | `relative flex items-center gap-1.5 text-[#94a3b8] hover:text-white transition-colors text-sm font-medium focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]` |
| Icono mensajeria | `<MessageSquare>` (Lucide) | `w-5 h-5` |
| Badge no-leidos | `<span>` (nativo) | `absolute -top-1.5 -right-1.5 min-w-[1.125rem] h-[1.125rem] rounded-full bg-[#ec4899] text-white text-[10px] font-bold flex items-center justify-center px-1 leading-none` - clase `hidden` cuando `totalNoLeidos === 0` |

**Logica del texto del badge:**
- `totalNoLeidos === 0` → `hidden`
- `1 <= totalNoLeidos <= 99` → numero
- `totalNoLeidos > 99` → `"99+"`

**Composicion de NavbarMensajesIcon:**

```tsx
<Link
    to="/crowdsourcing/mensajes"
    className="relative flex items-center gap-1.5 text-[#94a3b8] hover:text-white
               transition-colors text-sm font-medium focus-visible:outline-none
               focus-visible:ring-2 focus-visible:ring-[#a855f7]"
>
    <MessageSquare className="w-5 h-5" />
    <span
        className={cn(
            "absolute -top-1.5 -right-1.5 min-w-[1.125rem] h-[1.125rem] rounded-full",
            "bg-[#ec4899] text-white text-[10px] font-bold",
            "flex items-center justify-center px-1 leading-none",
            totalNoLeidos === 0 && "hidden"
        )}
        aria-label={`${totalNoLeidos} mensajes no leidos`}
    >
        {totalNoLeidos > 99 ? "99+" : totalNoLeidos}
    </span>
</Link>
```

---

### 3.4 Pantalla 4: Dialog para Iniciar Conversacion (`IniciarConversacionDialog`)

#### Layout

```
┌──────────────────────────────────────────────────┐
│  Iniciar conversacion                       [x]  │
│  Con: Studio Mix Pro                             │
├──────────────────────────────────────────────────┤
│  Contexto                                        │
│  ┌────────────────────────────────────────────┐  │
│  │ [Briefcase/FileText]  Mezcla de pistas EP  │  │
│  │                       Necesidad de CS      │  │
│  └────────────────────────────────────────────┘  │
│                                                  │
│  Asunto *                                        │
│  ┌────────────────────────────────────────────┐  │
│  │  Consulta sobre la propuesta...            │  │
│  └────────────────────────────────────────────┘  │
│  Breve descripcion del motivo                    │
│  0 / 200                                         │
├──────────────────────────────────────────────────┤
│  [Cancelar]              [Iniciar conversacion]  │
└──────────────────────────────────────────────────┘
```

| Elemento | Componente shadcn | Clases Tailwind / Props |
|----------|-------------------|-------------------------|
| Dialog | `<Dialog>` | `open={isOpen} onOpenChange={onClose}` |
| Contenido | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Titulo | `<DialogTitle>` | `text-xl font-semibold text-white` |
| Descripcion (nombre destinatario) | `<DialogDescription>` | `text-sm text-[#94a3b8] mt-0.5` "Con: {nombreDestinatario}" |
| Seccion contexto | `<div>` (nativo) | `bg-[#16213e] border border-[#334155] rounded-lg p-3 mb-5 flex items-start gap-3` |
| Icono necesidad | `<Briefcase>` (Lucide) | `w-5 h-5 text-[#a855f7] flex-shrink-0 mt-0.5` |
| Icono acuerdo | `<FileText>` (Lucide) | `w-5 h-5 text-[#3b82f6] flex-shrink-0 mt-0.5` |
| Titulo contexto | `<p>` (nativo) | `text-sm font-medium text-white` |
| Tipo contexto | `<p>` (nativo) | `text-xs text-[#64748b] mt-0.5` |
| Label asunto | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` `htmlFor="asunto-input"` |
| Asterisco requerido | `<span>` (nativo) | `text-red-400 ml-1` `aria-hidden="true"` |
| Input asunto | `<Input>` | `id="asunto-input" bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7] focus-visible:ring-[#a855f7] placeholder:text-[#64748b]` `autoFocus` placeholder="Breve descripcion del motivo..." |
| Input asunto (error) | `<Input>` | Agrega `border-red-500 focus:border-red-500` |
| Hint asunto | `<p>` (nativo) | `text-xs text-[#64748b] mt-1` |
| Contador | `<span>` (nativo) | `text-xs text-[#64748b] text-right block` "{n} / 200 caracteres" |
| Mensaje de error | `<p>` (nativo) | `text-sm text-red-400 mt-1` `role="alert"` |
| Footer | `<DialogFooter>` | `pt-4 border-t border-[#334155] flex justify-between gap-3` |
| Boton cancelar | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Boton iniciar | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Boton iniciar (submitting) | `<Button disabled>` | `<Loader2 className="animate-spin w-4 h-4 mr-2" />` "Iniciando..." |

**Composicion de IniciarConversacionDialog:**

```tsx
<Dialog open={isOpen} onOpenChange={onClose}>
    <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
        <DialogHeader>
            <DialogTitle className="text-xl font-semibold text-white">
                Iniciar conversacion
            </DialogTitle>
            <DialogDescription className="text-sm text-[#94a3b8] mt-0.5">
                Con: {destinatarioNombre}
            </DialogDescription>
        </DialogHeader>

        {/* Bloque contexto */}
        <div className="bg-[#16213e] border border-[#334155] rounded-lg p-3 mb-5 flex items-start gap-3">
            {contextoTipo === 'necesidad'
                ? <Briefcase className="w-5 h-5 text-[#a855f7] flex-shrink-0 mt-0.5" />
                : <FileText className="w-5 h-5 text-[#3b82f6] flex-shrink-0 mt-0.5" />
            }
            <div>
                <p className="text-sm font-medium text-white">{contextoTitulo}</p>
                <p className="text-xs text-[#64748b] mt-0.5">
                    {contextoTipo === 'necesidad' ? 'Necesidad de crowdsourcing' : 'Acuerdo de crowdsourcing'}
                </p>
            </div>
        </div>

        {/* Campo asunto */}
        <div>
            <Label htmlFor="asunto-input" className="text-sm font-medium text-[#cbd5e1] mb-1.5 block">
                Asunto
                <span aria-hidden="true" className="text-red-400 ml-1">*</span>
            </Label>
            <Input
                id="asunto-input"
                autoFocus
                placeholder="Breve descripcion del motivo..."
                className="bg-[#1a1a2e] border-[#334155] text-white h-11
                           focus:border-[#a855f7] focus-visible:ring-[#a855f7]
                           placeholder:text-[#64748b]"
            />
            <div className="flex justify-between mt-1">
                <p className="text-xs text-[#64748b]">Breve descripcion del motivo de la conversacion</p>
                <span className="text-xs text-[#64748b]">{asuntoValue.length} / 200</span>
            </div>
            {errorAsunto && (
                <p role="alert" className="text-sm text-red-400 mt-1">{errorAsunto}</p>
            )}
        </div>

        <DialogFooter className="pt-4 border-t border-[#334155] flex justify-between gap-3">
            <Button
                variant="outline"
                onClick={onClose}
                className="border-[#334155] text-white hover:bg-[#1e2a42]"
            >
                Cancelar
            </Button>
            <Button
                disabled={submitting || !!errorAsunto}
                className="bg-gradient-to-r from-pink-500 to-purple-600
                           hover:from-pink-600 hover:to-purple-700"
            >
                {submitting
                    ? <><Loader2 className="animate-spin w-4 h-4 mr-2" /> Iniciando...</>
                    : 'Iniciar conversacion'
                }
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

### 3.5 Pantalla 5: Puntos de Entrada (`IniciarConversacionButton`)

**Desde detalle de necesidad (propuesta enviada)**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Boton iniciar conversacion | `<Button variant="ghost">` | `flex items-center gap-2 border border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white bg-transparent h-9 px-3 text-sm rounded-lg transition-colors` |
| Icono | `<MessageSquare>` (Lucide) | `w-4 h-4` |
| Enlace a conversacion existente | `<Link>` (react-router-dom) | `flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]` |

**Desde detalle de acuerdo**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Enlace conversacion del acuerdo | `<Link>` (react-router-dom) | `flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]` |
| Badge no-leidos | `<Badge>` | `bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold ml-1` - clase `hidden` cuando no hay no-leidos |

**Logica de visibilidad:**
- Sin propuesta enviada: componente no se renderiza
- Propuesta enviada, sin conversacion: renderizar boton "Iniciar conversacion" que abre `IniciarConversacionDialog`
- Conversacion existente (desde necesidad): renderizar enlace "Ver conversacion"
- Desde acuerdo: siempre visible el enlace "Ver conversacion del acuerdo" con badge opcional

**Composicion de IniciarConversacionButton:**

```tsx
{/* Caso: no hay conversacion previa */}
<Button
    variant="ghost"
    onClick={() => setDialogOpen(true)}
    className="flex items-center gap-2 border border-[#334155] text-[#94a3b8]
               hover:bg-[#1e2a42] hover:text-white bg-transparent h-9 px-3
               text-sm rounded-lg transition-colors"
>
    <MessageSquare className="w-4 h-4" />
    Iniciar conversacion
</Button>

{/* Caso: conversacion existente */}
<Link
    to={`/crowdsourcing/mensajes/${conversacionExistenteId}`}
    className="flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc]
               text-sm transition-colors focus-visible:outline-none
               focus-visible:ring-2 focus-visible:ring-[#a855f7]"
>
    <MessageSquare className="w-4 h-4" />
    Ver conversacion
</Link>
```

---

## 4. Formularios

### 4.1 Formulario IniciarConversacionDialog

**Validacion via react-hook-form + Zod (`createConversacionSchema`)**

| Campo | Componente | Validacion Zod | Estado visual de error |
|-------|------------|----------------|------------------------|
| `asunto` | `<Input>` | `min(1)`, `max(200)` | `border-red-500`, `<p role="alert">` debajo |

**Layout:** Campo unico en columna vertical, label arriba, hint y contador debajo, error debajo del contador.

**Estados del campo asunto:**

| Estado | Visual |
|--------|--------|
| Default | `border-[#334155]` |
| Focus | `border-[#a855f7]` + ring purple |
| Error | `border-red-500` + mensaje rojo debajo |
| Disabled (submitting) | `opacity-50 cursor-not-allowed` |

### 4.2 Area de Entrada de Mensaje en Chat

**Validacion inline (sin react-hook-form, estado local)**

| Campo | Validacion | Estado visual |
|-------|-----------|---------------|
| `contenido` | `length >= 1` (sin espacios) | Boton Enviar disabled si vacio |
| `contenido` | `length <= 5000` | `border-red-500` en Textarea, texto rojo, contador visible |
| `urlAdjunto` | URL valida o vacio (`onBlur`) | Campo URL con `border-red-500`, texto rojo debajo |

---

## 5. Estados de UI por Componente

### 5.1 Lista de Conversaciones (MensajesPage / ConversacionList)

| Estado | Elementos UI |
|--------|-------------|
| **Loading** | 5x `<Skeleton className="h-20 rounded-xl w-full bg-[#1e2a42]" />` con `space-y-3` |
| **Error API** | `<Alert>` con `variant="destructive"`, borde rojo, `<AlertCircle>` Lucide, texto "No se pudieron cargar las conversaciones. Intenta de nuevo.", `<Button>` "Reintentar" |
| **Empty** | `<MessageSquare className="w-12 h-12 mx-auto mb-3 opacity-30 text-[#64748b]" />` + `<p className="text-base text-[#64748b] text-center">No tienes conversaciones activas</p>` dentro de `py-16` |
| **Default** | Lista de `ConversacionRow`, ordenada por `fechaUltimoMensaje DESC` |
| **Loading more** | `<Loader2 className="animate-spin w-5 h-5 text-[#64748b] mx-auto" />` debajo de la lista |

**Composicion del estado Empty:**

```tsx
<div className="text-center py-16">
    <MessageSquare className="w-12 h-12 mx-auto mb-3 opacity-30 text-[#64748b]" />
    <p className="text-base text-[#64748b]">No tienes conversaciones activas</p>
</div>
```

**Composicion del estado Error:**

```tsx
<Alert variant="destructive" className="border-red-500/50 bg-red-500/10">
    <AlertCircle className="h-4 w-4 text-red-400" />
    <AlertTitle className="text-red-400">Error al cargar conversaciones</AlertTitle>
    <AlertDescription className="text-red-300">
        No se pudieron cargar las conversaciones. Intenta de nuevo.
    </AlertDescription>
    <Button
        variant="outline"
        size="sm"
        className="mt-3 border-red-500/50 text-red-400 hover:bg-red-500/10"
        onClick={onRetry}
    >
        Reintentar
    </Button>
</Alert>
```

### 5.2 Vista de Chat (ChatView / ScrollArea)

| Estado | Elementos UI |
|--------|-------------|
| **Loading inicial** | `<Skeleton className="h-20 rounded-xl mb-4" />` (cabecera) + 5x skeletons de burbujas alternando left/right en ScrollArea + MessageInput `disabled` |
| **Error carga** | `<Alert>` rojo "No se pudo cargar la conversacion." + boton "Reintentar" |
| **Sin mensajes** | Texto centrado en ScrollArea: `<p className="text-center text-[#64748b] py-8">Aun no hay mensajes. Escribe el primero.</p>` |
| **Default con mensajes** | Mensajes visibles, auto-scroll al final |
| **Enviando mensaje** | Burbuja optimista con "Enviando..." en gris, boton con spinner, Textarea `disabled` |
| **Error al enviar** | Toast error + icono `<AlertCircle className="w-3 h-3 text-red-400" />` junto al timestamp de la burbuja fallida |
| **Mensaje enviado** | "Enviando..." reemplazado por timestamp real, textarea limpio |
| **Nuevos mensajes (polling)** | Burbujas con `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |

**Skeleton de burbuja (loading inicial):**

```tsx
{/* Burbuja ajena (izquierda) */}
<div className="flex items-start gap-2 mb-3">
    <Skeleton className="w-8 h-8 rounded-full bg-[#1e2a42]" />
    <Skeleton className="h-16 w-48 rounded-[1rem_1rem_1rem_0.25rem] bg-[#1e2a42]" />
</div>
{/* Burbuja propia (derecha) */}
<div className="flex items-start gap-2 mb-3 flex-row-reverse">
    <Skeleton className="w-8 h-8 rounded-full bg-[#1e2a42]" />
    <Skeleton className="h-12 w-40 rounded-[1rem_1rem_0.25rem_1rem] bg-[#1e2a42]" />
</div>
```

### 5.3 Dialog IniciarConversacionDialog

| Estado | Visual |
|--------|--------|
| **Default** | Campo asunto vacio con `autoFocus`, contador 0/200, boton "Iniciar conversacion" habilitado |
| **Validation Error** | Borde rojo en Input, `<p role="alert">` con mensaje, boton disabled |
| **Submitting** | Spinner en boton + "Iniciando...", Input y boton Cancelar disabled |
| **Error 4015 (duplicada)** | Toast `info`: "Ya existe una conversacion..." Dialog cierra + navegar a lista de mensajes |
| **Error 4016 (sin relacion)** | Toast `error`: "No puedes iniciar una conversacion..." Dialog permanece abierto |
| **Error API generico** | Toast `error`: "No se pudo iniciar la conversacion. Intenta de nuevo." Dialog abierto |
| **Success** | Dialog cierra, toast `success`: "Conversacion iniciada.", navegar a `/crowdsourcing/mensajes/{id}` |

---

## 6. Toasts (via Sonner)

| Evento | Tipo toast | Mensaje |
|--------|-----------|---------|
| Conversacion creada exitosamente | `toast.success` | "Conversacion iniciada correctamente." |
| Error al crear (generico) | `toast.error` | "No se pudo iniciar la conversacion. Intenta de nuevo." |
| Conversacion ya existe (4015) | `toast.info` | "Ya existe una conversacion con esta persona para este contexto." |
| Sin relacion (4016) | `toast.error` | "No puedes iniciar una conversacion con este usuario sin una relacion previa." |
| Error al enviar mensaje | `toast.error` | "No se pudo enviar el mensaje. Intenta de nuevo." |
| Error al cargar mensajes | `toast.error` | "No se pudieron cargar los mensajes." |

---

## 7. Responsive Design

| Breakpoint | Elementos afectados | Cambio |
|------------|---------------------|--------|
| `< 768px` (mobile) | Pagina lista | `px-3 py-4` en lugar de `px-4 py-8` |
| `< 768px` (mobile) | `ConversacionRow` | Nombre truncado mas agresivo, badge compacto |
| `< 768px` (mobile) | Vista chat | Cabecera condensada (nombre + flecha) |
| `< 768px` (mobile) | `ScrollArea` chat | `h-[calc(100vh-200px)]` |
| `< 768px` (mobile) | `MessageInput` | Textarea y boton full-width, fijados al fondo de la pantalla |
| `768px - 1024px` (tablet) | Lista conversaciones | `max-w-2xl mx-auto` |
| `768px - 1024px` (tablet) | Vista chat | `max-w-2xl mx-auto`, `ScrollArea h-[calc(100vh-280px)]` |
| `> 1024px` (desktop) | Lista conversaciones | `max-w-3xl mx-auto` |
| `> 1024px` (desktop) | Vista chat | `max-w-3xl mx-auto`, `ScrollArea h-[calc(100vh-320px)]` |

**Clases Tailwind responsive clave:**

```tsx
{/* Pagina lista - contenedor */}
className="max-w-3xl mx-auto px-4 py-8 md:max-w-2xl lg:max-w-3xl px-3 md:px-4"

{/* ScrollArea chat - altura variable por breakpoint */}
className="h-[calc(100vh-200px)] md:h-[calc(100vh-280px)] lg:h-[calc(100vh-320px)] min-h-[300px]"
```

**Nota MVP:** No se implementa layout de dos columnas (lista + chat en paralelo). Navegacion unica: lista separada de chat en todos los breakpoints.

---

## 8. Animaciones y Transiciones

| Elemento | Animacion | Clases Tailwind |
|----------|-----------|-----------------|
| Hover en fila de conversacion | Cambio de fondo | `transition-colors duration-150` |
| Hover en boton Enviar | Cambio de gradiente | `transition-all duration-200` |
| Hover en enlaces | Color | `transition-colors` |
| Nuevo mensaje (polling) | Fade + slide desde abajo | `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |
| Dialog apertura/cierre | Fade + scale | Manejado por shadcn/ui Dialog internamente |
| Skeleton loading | Pulso | `animate-pulse` (incluido en `<Skeleton>` de shadcn) |
| Spinner en botones | Rotacion continua | `animate-spin` |
| Textarea auto-resize | Sin transicion CSS | Cambio via JS para evitar saltos visuales |

---

## 9. Accesibilidad (ARIA)

| Requisito | Componente afectado | Implementacion |
|-----------|---------------------|----------------|
| Labels asociados a inputs | `MessageInput`, `IniciarConversacionDialog` | `<Label htmlFor="...">` + `id` en input correspondiente. URL adjunta: `<Label className="sr-only">` |
| Errores accesibles | Todos los campos con validacion | `role="alert"` en `<p>` de error para anuncio inmediato por screen readers |
| Focus ring visible | Todos los elementos interactivos | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| ARIA en badge navbar | `NavbarMensajesIcon` | `<span aria-label="{n} mensajes no leidos">` |
| ARIA en badge de fila | `ConversacionRow` | `<Badge aria-label="{n} mensajes no leidos">` |
| ARIA live region mensajes | `ChatView` ScrollArea | `aria-live="polite"` en el div contenedor de mensajes |
| ARIA busy carga | `ChatView` ScrollArea | `aria-busy={isLoading}` durante la carga inicial |
| Boton enviar sin texto | `MessageInput` | `aria-label="Enviar mensaje"` en `<Button>` que solo contiene icono |
| Alt text avatares | `Avatar` en todos los componentes | `<AvatarImage alt="{nombre de la persona}" />` |
| Asterisco requerido | `IniciarConversacionDialog` | `<span aria-hidden="true">*</span>` (decorativo) + label ya lo indica |
| Navegacion por teclado | `ConversacionRow` | `<button>` nativo (Tab + Enter/Space) - no usar `<div>` clickeable |
| Escape para cerrar dialog | `IniciarConversacionDialog` | Manejado por `<Dialog onOpenChange>` de Radix UI |
| Skip a contenido | `ChatView` | Enlace "Volver a mensajes" es el primer elemento enfocable |

**Contrastes verificados:**

| Combinacion | Ratio estimado | WCAG |
|-------------|----------------|------|
| `#ffffff` sobre `#0f1729` | > 10:1 | AAA |
| `#94a3b8` sobre `#0f1729` | > 4.5:1 | AA |
| `#ffffff` sobre gradiente `#ec4899→#a855f7` | > 4.5:1 | AA |
| `#e2e8f0` sobre `#16213e` | > 7:1 | AAA |
| `#64748b` sobre `#0f1729` | ~ 3.5:1 | Solo para texto no interactivo (timestamps) |

---

## 10. Componentes Personalizados - Estructura de Archivos

```
src/web/src/features/crowdsourcing/presentation/
├── pages/
│   ├── MensajesPage.tsx              # /crowdsourcing/mensajes - lista de conversaciones
│   └── ConversacionChatPage.tsx      # /crowdsourcing/mensajes/:id - vista de chat
└── components/
    ├── ConversacionList.tsx           # Lista con filtros tabs, skeleton, error, empty, paginacion
    ├── ConversacionRow.tsx            # Fila individual con avatar, badge, preview
    ├── ChatView.tsx                   # Contenedor principal del chat
    ├── ChatHeader.tsx                 # Cabecera: avatar, nombre, asunto, contexto
    ├── MessageBubble.tsx              # Burbuja individual (propio/ajeno)
    ├── DateSeparator.tsx              # Separador de fecha entre grupos de mensajes
    ├── MessageInput.tsx               # Textarea + boton enviar + campo URL adjunto
    ├── NavbarMensajesIcon.tsx         # Icono con badge para insertar en Navbar
    ├── IniciarConversacionDialog.tsx  # Dialog overlay con formulario de asunto
    └── IniciarConversacionButton.tsx  # Boton/enlace contextual (necesidad o acuerdo)
```

**Nota:** `ConversacionLink.tsx` ya existe en `src/web/src/features/crowdsourcing/presentation/components/ConversacionLink.tsx`. Este componente puede reemplazarse o extenderse por `IniciarConversacionButton.tsx` que cubre el mismo caso de uso con la logica completa de visibilidad y el dialog integrado.

---

## 11. Instalacion de Componente Faltante

`ScrollArea` no esta instalado en `src/web/src/components/ui/`. Debe instalarse antes de implementar la vista de chat:

```bash
cd src/web
npx shadcn@latest add scroll-area
```

Esto genera `src/web/src/components/ui/scroll-area.tsx` con `ScrollArea` y `ScrollBar`.

---

## 12. Checklist

- [ ] `ScrollArea` instalado (`npx shadcn@latest add scroll-area` en `src/web`)
- [ ] Todos los inputs tienen `<Label>` asociado con `htmlFor`
- [ ] Errores de formulario con `role="alert"`
- [ ] Badge de no-leidos con `aria-label` en navbar y en filas
- [ ] `aria-live="polite"` en contenedor de mensajes del chat
- [ ] `aria-label="Enviar mensaje"` en boton de envio (solo icono)
- [ ] Alt text en todos los `<AvatarImage>`
- [ ] Focus ring visible en todos los elementos interactivos con `focus-visible:ring-[#a855f7]`
- [ ] `<button>` nativo para `ConversacionRow` (no div clickeable)
- [ ] `autoFocus` en Input de asunto al abrir `IniciarConversacionDialog`
- [ ] Estados loading/error/empty cubiertos en lista y chat
- [ ] Mensajes optimistas con indicador "Enviando..."
- [ ] Boton enviar disabled cuando textarea vacio o mientras se envia
- [ ] Animacion `animate-in fade-in-0 slide-in-from-bottom-2` para mensajes nuevos via polling
- [ ] Responsive: `h-[calc(100vh-Xpx)]` ajustado por breakpoint en ScrollArea del chat
- [ ] Gradiente `from-[#ec4899] to-[#a855f7]` en burbujas propias y boton enviar
- [ ] Radios especiales de burbuja: `rounded-[1rem_1rem_1rem_0.25rem]` (ajeno) / `rounded-[1rem_1rem_0.25rem_1rem]` (propio)
- [ ] Timestamp relativo correcto (hace X min, ayer, fecha completa)
- [ ] Contador de caracteres visible solo cuando `contenido.length > 4000`
- [ ] `IniciarConversacionButton` con logica condicional: sin propuesta = oculto, con propuesta sin conv = dialog, con conv existente = enlace
- [ ] Badge acuerdo con no-leidos en enlace "Ver conversacion del acuerdo"
- [ ] Polling 30s en lista de conversaciones (`refetchInterval: 30000`)
- [ ] Polling 10s en vista de chat y badge navbar (`refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS`)
- [ ] `ConversacionLink.tsx` existente evaluado para reemplazo o extension por `IniciarConversacionButton`
