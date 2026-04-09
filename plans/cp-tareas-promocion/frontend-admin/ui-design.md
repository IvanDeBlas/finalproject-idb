# Diseno UI: cp-tareas-promocion (Admin)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/admin
**Ruta afectada:** `/crowdpromotion/programas/[id]` - nueva tab "Pendientes"

---

## 1. Resumen

- **Componentes shadcn nuevos:** Tabs (extension), Card, Badge, Button, Dialog, Form, Textarea, Separator, Skeleton, Alert
- **Composiciones custom nuevas:** 4 (TareasPendientesTab, TareaPendienteCard, ValidarTareaDialog, RechazarTareaDialog)
- **Punto de integracion:** `PromoProgramaDetailClient.tsx` - agregar tab "Pendientes" al `<Tabs>` existente
- **Responsive breakpoints:** mobile (< 768px), tablet (768px-1024px), desktop (> 1024px)
- **Tema:** Dark theme exclusivo (Admin usa fondo `#0d0d1a` / `#151525`)

---

## 2. Paleta de Colores

| Uso | Valor CSS | Ejemplo de aplicacion |
|-----|-----------|----------------------|
| Fondo base sidebar/header | `#0d0d1a` | Sidebar, TopBar |
| Fondo de pagina | `#1a1a2e` | Area de contenido principal |
| Fondo de cards | `#151525` | TareaPendienteCard, DialogContent |
| Fondo hover de card | `#1e1e38` | Skeleton, hover states, TabsList bg |
| Fondo de inputs | `#0f0f1f` | Textarea en dialogs |
| Primario (purple) | `#a855f7` | Focus rings, links, tab activo border |
| Texto principal | `#ffffff` | Nombres, titulos |
| Texto secundario | `#94a3b8` | Subtitulos, labels secundarios |
| Texto muted | `#64748b` | Hints, fechas, labels readonly |
| Borde default | `#334155` | Bordes de cards, separadores, inputs |
| Borde focus | `#a855f7` | Focus en inputs |
| Estado: Completada/Warning | `#f59e0b` / amber | Badge pendiente de validacion |
| Estado: Validada/Success | `#10b981` / green | Boton Validar, bloque recompensa |
| Estado: Rechazada/Error | `#ef4444` / red | Boton Rechazar, badge destructive |

---

## 3. Componentes por Screen

### 3.1 Integracion en PromoProgramaDetailClient

La nueva tab "Pendientes" se agrega al `<Tabs>` existente en `PromoProgramaDetailClient.tsx`. Este componente ya usa el patron de tabs de shadcn con customizacion del proyecto.

#### Layout global (sin cambios)

```
┌───────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a], border-r border-[#334155])               │
├───────────────────────────────────────────────────────────────────────┤
│ TOPBAR (h-14, bg-[#0d0d1a], border-b border-[#334155])                │
├───────────────────────────────────────────────────────────────────────┤
│ CONTENIDO (p-6)                                                        │
│                                                                        │
│  PromoProgramaHeader                                                   │
│  PromoProgramaKpiCards                                                 │
│                                                                        │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │ TABS (overflow-x-auto)                                           │  │
│  │ [Info general]  [Tareas (N)]  [Promotores (N)]  [Solicitudes]   │  │
│  │ [Aprobados]  [Bloqueados]  [Pendientes (N)]  [Resumen]          │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                                                                        │
│  TAB CONTENT: TareasPendientesTab                                      │
│                                                                        │
└───────────────────────────────────────────────────────────────────────┘
```

#### TabsTrigger "Pendientes" - Modificacion al componente existente

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Tab trigger | `TabsTrigger` | `value="pendientes"` con `flex items-center gap-1.5` |
| Badge con count | `Badge` | `bg-red-500/20 text-red-400 border border-red-500/30 text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0` |
| Visibilidad badge | Condicional | Solo visible cuando `totalCount > 0` |

**Composicion del trigger:**
```tsx
<TabsTrigger value="pendientes" className="flex items-center gap-1.5">
    Pendientes
    {totalPendientes > 0 && (
        <Badge
            className="bg-red-500/20 text-red-400 border border-red-500/30 text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0"
            aria-label={`${totalPendientes} tareas pendientes de validacion`}
        >
            {totalPendientes}
        </Badge>
    )}
</TabsTrigger>
```

**Nota sobre `totalPendientes`:** El valor proviene de la primera carga de la tab (query `tareas-pendientes` con `page=1`), que incluye `totalCount` en el response. El tab trigger recibe este valor como prop desde `TareasPendientesTab` via callback o desde el query cache.

---

### 3.2 TareasPendientesTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/TareasPendientesTab.tsx`

**API:** `GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes?page={page}&pageSize=10`

**Props:**
```tsx
interface TareasPendientesTabProps {
    programaId: string
}
```

#### Layout del tab

```
┌──────────────────────────────────────────────────────────────────────┐
│ TAB CONTENT (pt-4)                                                    │
│                                                                        │
│  [Estado: Loading]                                                     │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ Skeleton card 1 (h-[120px] animate-pulse rounded-lg bg-[#1e1e38])│  │
│  └─────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ Skeleton card 2                                                  │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ Skeleton card 3                                                  │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│                                                                        │
│  [Estado: Default - con pendientes]                                    │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ TareaPendienteCard (item 1)                                      │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ TareaPendienteCard (item 2)                                      │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│                                                                        │
│  PAGINACION                                                            │
│  Mostrando 10 de 24 pendientes    [< Anterior]  Pagina 1 de 3  [Sig >]│
│                                                                        │
│  [Estado: Empty]                                                       │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │         [CheckCircle icon verde, w-12 h-12]                     │  │
│  │         No hay tareas pendientes de validacion                  │  │
│  │         Todas las tareas completadas han sido revisadas          │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│                                                                        │
│  [Estado: Error]                                                       │
│  ┌─────────────────────────────────────────────────────────────────┐  │
│  │ [AlertCircle rojo]  Error al cargar las tareas pendientes        │  │
│  │                                      [Reintentar]               │  │
│  └─────────────────────────────────────────────────────────────────┘  │
│                                                                        │
└──────────────────────────────────────────────────────────────────────┘
```

#### Componentes del tab

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor tab | `TabsContent` | `value="pendientes" className="mt-4 space-y-0"` |
| Skeleton de loading | `Skeleton` | `h-[120px] rounded-lg bg-[#1e1e38] animate-pulse mb-3` (x3) |
| Lista de cards | `<div>` nativo | `space-y-3` |
| Card de item | `TareaPendienteCard` | Composicion custom (ver 3.3) |
| Empty state container | `<div>` nativo | `bg-[#1e1e38]/50 rounded-xl py-10 text-center` |
| Empty state icon | `CheckCircle` (lucide) | `w-12 h-12 text-green-400 mx-auto mb-4` |
| Empty state titulo | `<p>` nativo | `text-base font-medium text-white` |
| Empty state subtitulo | `<p>` nativo | `text-sm text-[#64748b] mt-1` |
| Error container | `Alert` | `variant="destructive"` con `className="border-red-900/50 bg-red-950/20"` |
| Error titulo | `AlertTitle` | `text-red-400` |
| Error boton retry | `Button` | `variant="outline" size="sm" className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] mt-3"` |

#### Paginacion

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor paginacion | `<div>` nativo | `flex items-center justify-between mt-6 pt-4 border-t border-[#334155]` |
| Info contador | `<p>` nativo | `text-sm text-[#64748b]` / "Mostrando {min(pageSize, remaining)} de {totalCount} pendientes" |
| Grupo botones | `<div>` nativo | `flex items-center gap-2` |
| Boton anterior | `Button` | `variant="outline" size="sm" className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-3" disabled={page <= 1}` |
| Icono anterior | `ChevronLeft` (lucide) | `w-4 h-4` con `aria-hidden="true"` |
| Indicador pagina | `<span>` nativo | `text-sm text-white px-3` / "Pagina {page} de {totalPages}" |
| Boton siguiente | `Button` | mismo estilo que anterior, `disabled={page >= totalPages}` |
| Icono siguiente | `ChevronRight` (lucide) | `w-4 h-4` con `aria-hidden="true"` |

**Composicion de la paginacion:**
```tsx
<div className="flex items-center justify-between mt-6 pt-4 border-t border-[#334155]">
    <p className="text-sm text-[#64748b]">
        Mostrando {Math.min(pageSize, totalCount - (page - 1) * pageSize)} de {totalCount} pendientes
    </p>
    <div className="flex items-center gap-2">
        <Button
            variant="outline"
            size="sm"
            className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-3"
            onClick={() => setPage(p => p - 1)}
            disabled={page <= 1}
            aria-label="Pagina anterior"
        >
            <ChevronLeft className="w-4 h-4" aria-hidden="true" />
        </Button>
        <span className="text-sm text-white px-3">Pagina {page} de {totalPages}</span>
        <Button
            variant="outline"
            size="sm"
            className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-3"
            onClick={() => setPage(p => p + 1)}
            disabled={page >= totalPages}
            aria-label="Pagina siguiente"
        >
            <ChevronRight className="w-4 h-4" aria-hidden="true" />
        </Button>
    </div>
</div>
```

---

### 3.3 TareaPendienteCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/TareaPendienteCard.tsx`

**Patron de referencia:** `SolicitudCard.tsx` - mismo patron dark theme + card con acciones + fade-out al procesar.

**Props:**
```tsx
interface TareaPendienteCardProps {
    item: TareaPendienteItem
    programaId: string
    onValidar: (item: TareaPendienteItem) => void
    onRechazar: (item: TareaPendienteItem) => void
}
```

#### Layout de la card

```
┌────────────────────────────────────────────────────────────────────┐
│ CARD (bg-[#0f0f1f], border-[#334155], p-4, mb-3)                   │
│                                                                      │
│  ┌──────────────────────────────────────────┐ ┌──────────────────┐ │
│  │ COLUMNA IZQUIERDA (flex-1)               │ │ ACCIONES (shrink)│ │
│  │                                          │ │                  │ │
│  │ DJ Marketing Pro (Influencer)            │ │ [Validar]        │ │
│  │ Comparte en IG Stories (4ta vez)         │ │ [Rechazar]       │ │
│  │                                          │ │                  │ │
│  │ Prueba: [instagram.com/... ->]           │ │                  │ │
│  │                                          │ │                  │ │
│  │ "Story publicada con mencion a..."       │ │                  │ │
│  │  (blockquote style)                      │ │                  │ │
│  │                                          │ │                  │ │
│  │ [Clock] Hace 2 horas                     │ │                  │ │
│  └──────────────────────────────────────────┘ └──────────────────┘ │
│                                                                      │
└────────────────────────────────────────────────────────────────────┘
```

#### Componentes de la card

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|-----------------------|
| Card raiz | `Card` | `bg-[#0f0f1f] border border-[#334155] p-4 mb-3 transition-all duration-400` + estado `isHiding`: `opacity-0 max-h-0 overflow-hidden` |
| Layout fila | `<div>` nativo | `flex items-start justify-between gap-4` |
| Columna info | `<div>` nativo | `flex-1 min-w-0` |
| Nombre promotor | `<p>` nativo | `text-base font-semibold text-white` |
| Tipo promotor | `<span>` nativo | `text-xs text-[#64748b] ml-2 font-normal` / "(Influencer)" entre parentesis |
| Nombre tarea + ejecucion | `<p>` nativo | `text-sm text-[#94a3b8] mt-0.5` / "Comparte en IG Stories (4ta vez)" |
| Fila URL prueba | `<div>` nativo | `flex items-center gap-1.5 mt-2` |
| Label "Prueba:" | `<span>` nativo | `text-xs text-[#64748b] shrink-0` |
| Link URL | `<a>` nativo | `text-sm text-[#a855f7] hover:text-purple-400 truncate max-w-[300px] inline-flex items-center gap-1 transition-colors` / `target="_blank" rel="noopener noreferrer"` |
| Icono link externo | `ExternalLink` (lucide) | `w-3 h-3 shrink-0 aria-hidden="true"` |
| Comentario promotor (si existe) | `<p>` nativo | `text-sm text-[#94a3b8] italic mt-2 pl-2 border-l-2 border-[#334155]` |
| Placeholder sin comentario | `<p>` nativo | `text-xs text-[#64748b] italic mt-2` / "(sin comentario)" |
| Fecha relativa | `<p>` nativo con `<time>` | `text-xs text-[#64748b] mt-2 flex items-center gap-1` |
| Icono fecha | `Clock` (lucide) | `w-3 h-3 aria-hidden="true"` |
| Elemento time | `<time dateTime="{ISO}">` | Texto "Hace 2 horas" - accesibilidad de fechas |
| Columna acciones | `<div>` nativo | `flex flex-col sm:flex-row items-end sm:items-center gap-2 shrink-0` |
| Boton Validar | `Button` | `size="sm"` + `bg-green-950/50 text-green-400 border border-green-800/50 hover:bg-green-900/50 hover:border-green-700/50 h-9 px-4 text-sm font-medium transition-colors` |
| Icono Validar | `Check` (lucide) | `w-3.5 h-3.5 mr-1.5 aria-hidden="true"` |
| Boton Rechazar | `Button` | `size="sm" variant="outline"` + `border-red-800/50 text-red-400 hover:bg-red-950/50 hover:border-red-700/50 h-9 px-4 text-sm font-medium transition-colors` |
| Icono Rechazar | `X` (lucide) | `w-3.5 h-3.5 mr-1.5 aria-hidden="true"` |

**Composicion completa de la card:**
```tsx
<Card
    className={`bg-[#0f0f1f] border border-[#334155] p-4 mb-3 transition-all duration-400 ${
        isHiding ? "opacity-0 max-h-0 overflow-hidden" : "opacity-100"
    }`}
>
    <div className="flex items-start justify-between gap-4">
        <div className="flex-1 min-w-0">
            <p className="text-base font-semibold text-white">
                {item.promotorNombre}
                {item.promotorTipoNombre && (
                    <span className="text-xs text-[#64748b] ml-2 font-normal">
                        ({item.promotorTipoNombre})
                    </span>
                )}
            </p>
            <p className="text-sm text-[#94a3b8] mt-0.5">
                {item.tareaNombre} ({formatEjecucion(item.vecesCompletada)})
            </p>
            {item.urlPruebaCompletado && (
                <div className="flex items-center gap-1.5 mt-2">
                    <span className="text-xs text-[#64748b] shrink-0">Prueba:</span>
                    <a
                        href={item.urlPruebaCompletado}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-[#a855f7] hover:text-purple-400 truncate max-w-[300px] inline-flex items-center gap-1 transition-colors"
                    >
                        {displayUrl(item.urlPruebaCompletado)}
                        <ExternalLink className="w-3 h-3 shrink-0" aria-hidden="true" />
                    </a>
                </div>
            )}
            {item.comentarioPromotor ? (
                <p className="text-sm text-[#94a3b8] italic mt-2 pl-2 border-l-2 border-[#334155]">
                    {item.comentarioPromotor}
                </p>
            ) : (
                <p className="text-xs text-[#64748b] italic mt-2">(sin comentario)</p>
            )}
            {item.fechaUltimaCompletada && (
                <p className="text-xs text-[#64748b] mt-2 flex items-center gap-1">
                    <Clock className="w-3 h-3" aria-hidden="true" />
                    <time dateTime={item.fechaUltimaCompletada}>
                        {formatRelativeDate(item.fechaUltimaCompletada)}
                    </time>
                </p>
            )}
        </div>
        <div className="flex flex-col sm:flex-row items-end sm:items-center gap-2 shrink-0">
            <Button
                size="sm"
                className="bg-green-950/50 text-green-400 border border-green-800/50 hover:bg-green-900/50 hover:border-green-700/50 h-9 px-4 text-sm font-medium transition-colors"
                onClick={() => onValidar(item)}
                disabled={isAnyPending}
            >
                <Check className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                Validar
            </Button>
            <Button
                size="sm"
                variant="outline"
                className="border-red-800/50 text-red-400 hover:bg-red-950/50 hover:border-red-700/50 h-9 px-4 text-sm font-medium transition-colors"
                onClick={() => onRechazar(item)}
                disabled={isAnyPending}
            >
                <X className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                Rechazar
            </Button>
        </div>
    </div>
</Card>
```

**Funcion auxiliar `formatEjecucion`:**

| vecesCompletada | Texto generado |
|-----------------|----------------|
| 1 | "1ra vez" |
| 2 | "2da vez" |
| 3 | "3ra vez" |
| 4+ | "{N}ta vez" |

**Funcion auxiliar `displayUrl`:**
Trunca la URL para mostrar solo el dominio + path abreviado. Ej: `instagram.com/stories/xxx...`

**Funcion auxiliar `formatRelativeDate`:**
Reutilizar la funcion existente de `SolicitudCard.tsx`. Se extrae a `utils/date.ts` para compartirla.

#### Estados de la TareaPendienteCard

| Estado | Visual |
|--------|--------|
| Default | Visible con opacidad 100%, botones habilitados |
| Validando | Boton Validar muestra `Loader2 animate-spin`, ambos botones `disabled` |
| Rechazando | Boton Rechazar muestra `Loader2 animate-spin`, ambos botones `disabled` |
| Procesado (fade-out) | `isHiding=true`: `opacity-0 max-h-0 overflow-hidden` con `transition-all duration-400` |

---

### 3.4 ValidarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/ValidarTareaDialog.tsx`

**API:** `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar`

**Props:**
```tsx
interface ValidarTareaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    item: TareaPendienteItem
    programaId: string
    recompensa?: {
        importe: number
        monedaNombre: string
    }
}
```

**Nota sobre `recompensa`:** El campo `importeRecompensa` y `monedaNombre` no estan en `TareaPendienteItem` (el endpoint tareas-pendientes no los incluye directamente segun los contracts). Se obtienen de los datos del programa ya cargados en `PromoProgramaDetailClient` (via `programa.tareas`). El componente los recibe como prop opcional; si `undefined`, no se muestra el bloque verde de recompensa.

#### Layout del dialog

```
┌────────────────────────────────────────┐
│  [X cerrar]                            │
│                                        │
│  Validar tarea completada              │
│  ─────────────────────────────────     │
│                                        │
│  Tarea:     Comparte en IG Stories     │
│  Promotor:  DJ Marketing Pro           │
│  Ejecucion: 4ta vez completada         │
│                                        │
│  Prueba enviada:                       │
│  [instagram.com/stories/xxx ->]        │
│                                        │
│  Comentario del promotor:              │
│  ┌─────────────────────────────────┐   │
│  │ "Story publicada con mencion..."|   │
│  └─────────────────────────────────┘   │
│                                        │
│  ─────────────────────────────────     │
│  [DollarSign] Recompensa a acreditar:  │
│               5 EUR en wallet          │
│  ─────────────────────────────────     │
│                                        │
│  Comentario de validacion (opcional)   │
│  [                                 ]   │
│                                        │
│  ─────────────────────────────────     │
│       [Cancelar]    [Validar tarea]    │
└────────────────────────────────────────┘
```

#### Componentes del dialog

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|-----------------------|
| Dialog raiz | `Dialog` | `open={open} onOpenChange={onOpenChange}` |
| Contenedor | `DialogContent` | `bg-[#151525] border border-[#334155] text-white max-w-md w-full rounded-xl p-0 overflow-hidden sm:max-w-md` |
| Header | `<div>` nativo | `px-6 pt-6 pb-4 border-b border-[#334155]` |
| Titulo | `DialogTitle` | `text-xl font-bold text-white` / "Validar tarea completada" |
| Descripcion accesible | `DialogDescription` | `sr-only` / "Formulario para validar la tarea completada por {promotorNombre}" |
| Seccion datos | `<div>` nativo | `px-6 py-4 flex flex-col gap-3` |
| Fila de dato | `<div>` nativo | `flex items-start gap-2` |
| Label de dato | `<span>` nativo | `text-xs font-medium text-[#64748b] w-24 shrink-0` |
| Valor de dato texto | `<span>` nativo | `text-sm text-white` |
| Link URL prueba | `<a>` nativo | `text-sm text-[#a855f7] hover:text-purple-400 inline-flex items-center gap-1 break-all transition-colors` + `target="_blank" rel="noopener noreferrer"` |
| Icono link | `ExternalLink` (lucide) | `w-3 h-3 shrink-0 aria-hidden="true"` |
| Bloque comentario promotor | `<div>` nativo | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-sm text-[#cbd5e1] italic` |
| Separador | `Separator` | `bg-[#334155]` |
| Bloque recompensa (si existe) | `<div>` nativo | `px-6 py-3 bg-green-950/20 border-y border-green-900/30 flex items-center gap-3` |
| Icono recompensa | `DollarSign` (lucide) | `w-5 h-5 text-green-400 shrink-0 aria-hidden="true"` |
| Label recompensa | `<p>` nativo | `text-xs text-[#64748b]` / "Recompensa a acreditar:" |
| Valor recompensa | `<p>` nativo | `text-sm font-semibold text-green-400` / "{importe} {moneda} en wallet del promotor" |
| Seccion formulario | `<div>` nativo | `px-6 py-4` |
| Label comentario | `Label` shadcn | `text-sm font-medium text-[#cbd5e1]` / "Comentario de validacion (opcional)" |
| Textarea comentario | `Textarea` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[72px] resize-none mt-2 focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30` |
| Footer | `DialogFooter` | `px-6 pb-6 pt-2 border-t border-[#334155]` / override para `flex flex-row justify-end gap-3` |
| Boton Cancelar | `Button` | `variant="ghost"` + `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` |
| Boton Validar (default) | `Button` | `bg-green-600 hover:bg-green-700 text-white font-semibold h-10 px-6 transition-colors` |
| Icono Validar | `Check` (lucide) | `w-4 h-4 mr-2 aria-hidden="true"` |
| Boton Validar (loading) | `Button` | Mismo estilo + `disabled` + `Loader2` spinner |
| Spinner loading | `Loader2` (lucide) | `w-4 h-4 animate-spin mr-2 aria-hidden="true"` |

**Composicion completa del dialog:**
```tsx
<Dialog open={open} onOpenChange={isPending ? undefined : onOpenChange}>
    <DialogContent className="bg-[#151525] border border-[#334155] text-white max-w-md w-full rounded-xl p-0 overflow-hidden">
        <div className="px-6 pt-6 pb-4 border-b border-[#334155]">
            <DialogTitle className="text-xl font-bold text-white">
                Validar tarea completada
            </DialogTitle>
            <DialogDescription className="sr-only">
                Formulario para validar la tarea completada por {item.promotorNombre}
            </DialogDescription>
        </div>

        {/* Seccion datos readonly */}
        <div className="px-6 py-4 flex flex-col gap-3">
            <div className="flex items-start gap-2">
                <span className="text-xs font-medium text-[#64748b] w-24 shrink-0">Tarea:</span>
                <span className="text-sm text-white">{item.tareaNombre}</span>
            </div>
            <div className="flex items-start gap-2">
                <span className="text-xs font-medium text-[#64748b] w-24 shrink-0">Promotor:</span>
                <span className="text-sm text-white">{item.promotorNombre}</span>
            </div>
            <div className="flex items-start gap-2">
                <span className="text-xs font-medium text-[#64748b] w-24 shrink-0">Ejecucion:</span>
                <span className="text-sm text-white">{formatEjecucion(item.vecesCompletada)} completada</span>
            </div>
            {item.urlPruebaCompletado && (
                <div className="flex items-start gap-2">
                    <span className="text-xs font-medium text-[#64748b] w-24 shrink-0">Prueba:</span>
                    <a
                        href={item.urlPruebaCompletado}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-[#a855f7] hover:text-purple-400 inline-flex items-center gap-1 break-all transition-colors"
                    >
                        {displayUrl(item.urlPruebaCompletado)}
                        <ExternalLink className="w-3 h-3 shrink-0" aria-hidden="true" />
                    </a>
                </div>
            )}
            {item.comentarioPromotor && (
                <div className="bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-sm text-[#cbd5e1] italic">
                    {item.comentarioPromotor}
                </div>
            )}
        </div>

        <Separator className="bg-[#334155]" />

        {/* Bloque recompensa (si aplica) */}
        {recompensa && (
            <div className="px-6 py-3 bg-green-950/20 border-y border-green-900/30 flex items-center gap-3">
                <DollarSign className="w-5 h-5 text-green-400 shrink-0" aria-hidden="true" />
                <div>
                    <p className="text-xs text-[#64748b]">Recompensa a acreditar:</p>
                    <p className="text-sm font-semibold text-green-400">
                        {recompensa.importe} {recompensa.monedaNombre} en wallet del promotor
                    </p>
                </div>
            </div>
        )}

        {/* Formulario */}
        <div className="px-6 py-4">
            <Label className="text-sm font-medium text-[#cbd5e1]">
                Comentario de validacion (opcional)
            </Label>
            <Textarea
                value={comentario}
                onChange={(e) => setComentario(e.target.value)}
                placeholder="Ej: Verificado correctamente, contenido segun instrucciones"
                className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[72px] resize-none mt-2 focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30"
                disabled={isPending}
                maxLength={500}
            />
        </div>

        <Separator className="bg-[#334155] mx-6" />

        <DialogFooter className="px-6 pb-6 pt-4 flex flex-row justify-end gap-3 border-t border-[#334155]">
            <Button
                variant="ghost"
                className="text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5"
                onClick={() => onOpenChange(false)}
                disabled={isPending}
            >
                Cancelar
            </Button>
            <Button
                className="bg-green-600 hover:bg-green-700 text-white font-semibold h-10 px-6 transition-colors"
                onClick={handleValidar}
                disabled={isPending}
                aria-busy={isPending}
            >
                {isPending ? (
                    <>
                        <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />
                        Validando...
                    </>
                ) : (
                    <>
                        <Check className="w-4 h-4 mr-2" aria-hidden="true" />
                        Validar tarea
                    </>
                )}
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

#### Estados del ValidarTareaDialog

| Estado | Visual |
|--------|--------|
| Default | Datos visibles, textarea vacio, boton verde habilitado |
| Enviando | `isPending=true`: Boton verde con spinner + "Validando...", `disabled` en ambos botones, `onOpenChange` bloqueado (overlay no cierra) |
| Exito | Dialog se cierra. Card desaparece de la lista. Toast: "Tarea validada. Se acreditaron {importe} {moneda} en la wallet del promotor." |
| Error de API | Dialog permanece abierto. Toast destructivo: "No se pudo validar la tarea. Intentalo de nuevo." |

---

### 3.5 RechazarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/RechazarTareaDialog.tsx`

**API:** `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar`

**Props:**
```tsx
interface RechazarTareaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    item: TareaPendienteItem
    programaId: string
}
```

#### Layout del dialog

```
┌────────────────────────────────────────┐
│  [X cerrar]                            │
│                                        │
│  Rechazar tarea                        │
│  ─────────────────────────────────     │
│                                        │
│  Tarea:     Comparte en IG Stories     │
│  Promotor:  DJ Marketing Pro           │
│  Ejecucion: 4ta vez completada         │
│                                        │
│  Prueba enviada:                       │
│  [instagram.com/stories/xxx ->]        │
│                                        │
│  Comentario del promotor:              │
│  ┌─────────────────────────────────┐   │
│  │ "Story publicada con mencion..."|   │
│  └─────────────────────────────────┘   │
│                                        │
│  ─────────────────────────────────     │
│  [!] El promotor vera este motivo y    │
│      podra re-enviar la tarea.         │
│  ─────────────────────────────────     │
│                                        │
│  Motivo del rechazo *                  │
│  [                                 ]   │
│  0 / 500 caracteres                    │
│                                        │
│  ─────────────────────────────────     │
│       [Cancelar]    [Rechazar tarea]   │
└────────────────────────────────────────┘
```

#### Componentes del dialog

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|-----------------------|
| Dialog raiz | `Dialog` | `open={open} onOpenChange={isPending ? undefined : onOpenChange}` |
| Contenedor | `DialogContent` | `bg-[#151525] border border-[#334155] text-white max-w-md w-full rounded-xl p-0 overflow-hidden` |
| Header | `<div>` nativo | `px-6 pt-6 pb-4 border-b border-[#334155]` |
| Titulo | `DialogTitle` | `text-xl font-bold text-white` / "Rechazar tarea" |
| Descripcion accesible | `DialogDescription` | `sr-only` / "Formulario para rechazar la tarea completada por {promotorNombre}" |
| Seccion datos readonly | `<div>` nativo | Identica a ValidarTareaDialog (mismas filas Tarea / Promotor / Ejecucion / Prueba / Comentario) |
| Separador | `Separator` | `bg-[#334155]` |
| Bloque aviso amber | `<div>` nativo | `px-6 py-3 bg-amber-950/20 border-y border-amber-900/30 flex items-start gap-2.5` |
| Icono aviso | `AlertTriangle` (lucide) | `w-4 h-4 text-amber-400 shrink-0 mt-0.5 aria-hidden="true"` |
| Texto aviso | `<p>` nativo | `text-xs text-amber-300/80` / "El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba." |
| Seccion formulario | `<div>` nativo | `px-6 py-4` |
| Label motivo | `Label` shadcn | `text-sm font-medium text-[#cbd5e1]` / "Motivo del rechazo" con `<span className="text-red-400 ml-1">*</span>` |
| Textarea motivo | `Textarea` | Estado default: `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[80px] resize-none mt-2 focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30` / Estado error: `border-red-500 focus:border-red-500` |
| Fila contador/error | `<div>` nativo | `flex items-center justify-between mt-1` |
| Mensaje error | `<p>` nativo | `text-xs text-red-400` / solo visible si hay error de validacion (min 10 chars) con `role="alert"` |
| Contador caracteres | `<p>` nativo | `text-right text-xs text-[#64748b] ml-auto` / "{n} / 500 caracteres" |
| Footer | `DialogFooter` | `px-6 pb-6 pt-4 flex flex-row justify-end gap-3 border-t border-[#334155]` |
| Boton Cancelar | `Button` | `variant="ghost"` + `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` |
| Boton Rechazar (default) | `Button` | `bg-red-600/80 hover:bg-red-600 text-white font-semibold h-10 px-6 border border-red-700/50 transition-colors` |
| Icono Rechazar | `X` (lucide) | `w-4 h-4 mr-2 aria-hidden="true"` |
| Boton Rechazar (loading) | `Button` | Mismo estilo + `disabled` + `Loader2` spinner |

**Composicion completa del dialog (seccion diferenciada respecto a Validar):**
```tsx
{/* Bloque aviso amber - diferencia respecto a ValidarTareaDialog */}
<div className="px-6 py-3 bg-amber-950/20 border-y border-amber-900/30 flex items-start gap-2.5">
    <AlertTriangle className="w-4 h-4 text-amber-400 shrink-0 mt-0.5" aria-hidden="true" />
    <p className="text-xs text-amber-300/80">
        El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba.
    </p>
</div>

{/* Formulario con validacion */}
<div className="px-6 py-4">
    <Label
        htmlFor="motivo-rechazo"
        className="text-sm font-medium text-[#cbd5e1]"
    >
        Motivo del rechazo
        <span className="text-red-400 ml-1" aria-hidden="true">*</span>
    </Label>
    <Textarea
        id="motivo-rechazo"
        value={motivo}
        onChange={(e) => {
            setMotivo(e.target.value)
            if (touched) validateMotivo(e.target.value)
        }}
        onBlur={() => {
            setTouched(true)
            validateMotivo(motivo)
        }}
        placeholder="Explica por que no se acepta esta prueba..."
        className={`bg-[#0f0f1f] text-white placeholder:text-[#64748b] min-h-[80px] resize-none mt-2 transition-colors ${
            error
                ? "border-red-500 focus:border-red-500 focus-visible:ring-red-500/30"
                : "border-[#334155] focus:border-[#a855f7] focus-visible:ring-[#a855f7]/30"
        }`}
        aria-required="true"
        aria-invalid={!!error}
        aria-describedby={error ? "motivo-error" : undefined}
        disabled={isPending}
        maxLength={500}
    />
    <div className="flex items-center justify-between mt-1">
        {error ? (
            <p id="motivo-error" className="text-xs text-red-400" role="alert">
                {error}
            </p>
        ) : (
            <span />
        )}
        <p className="text-right text-xs text-[#64748b]">
            {motivo.length} / 500 caracteres
        </p>
    </div>
</div>

<Separator className="bg-[#334155] mx-0" />

<DialogFooter className="px-6 pb-6 pt-4 flex flex-row justify-end gap-3 border-t border-[#334155]">
    <Button
        variant="ghost"
        className="text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5"
        onClick={() => onOpenChange(false)}
        disabled={isPending}
    >
        Cancelar
    </Button>
    <Button
        className="bg-red-600/80 hover:bg-red-600 text-white font-semibold h-10 px-6 border border-red-700/50 transition-colors"
        onClick={handleRechazar}
        disabled={isPending || !isValid}
        aria-busy={isPending}
    >
        {isPending ? (
            <>
                <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />
                Rechazando...
            </>
        ) : (
            <>
                <X className="w-4 h-4 mr-2" aria-hidden="true" />
                Rechazar tarea
            </>
        )}
    </Button>
</DialogFooter>
```

#### Estados del RechazarTareaDialog

| Estado | Visual |
|--------|--------|
| Default (sin tocar) | Campo vacio, boton Rechazar habilitado visualmente (la validacion es `onBlur`) |
| Campo blur con error | Borde rojo en textarea, mensaje de error con `role="alert"`, boton `disabled` si campo invalido |
| Campo valido (>= 10 chars) | Borde default, sin mensaje de error, boton habilitado |
| Enviando | `isPending=true`: spinner en boton + "Rechazando...", ambos botones `disabled`, overlay no cierra |
| Exito | Dialog cierra, card desaparece de lista, Toast neutro: "Tarea rechazada. El promotor podra re-enviar con nueva prueba." |
| Error de API | Dialog permanece, Toast destructivo: "No se pudo rechazar la tarea. Intentalo de nuevo." |

---

## 4. Formularios

### 4.1 ValidarTareaForm (inline en ValidarTareaDialog)

Este formulario es minimalista: un solo campo opcional. No usa React Hook Form (un campo, sin schema Zod necesario en el cliente para la validacion ya que es opcional). Usa estado local `useState`.

| Campo | Componente | Validacion |
|-------|------------|------------|
| `comentarioValidacion` | `Textarea` | Opcional, `maxLength={500}` via atributo HTML |

**Comportamiento:**
- No hay validacion de error en este formulario. El campo es libre y opcional.
- El contador de caracteres NO se muestra en este dialog (diferencia respecto a Rechazar) porque no hay constraint de minimo.
- Al abrir el dialog, el campo se resetea a cadena vacia.
- Al cerrar el dialog (cancelar o exito), el campo se resetea.

### 4.2 RechazarTareaForm (inline en RechazarTareaDialog)

| Campo | Componente | Validacion Visual |
|-------|------------|-------------------|
| `comentarioValidacion` (motivo) | `Textarea` | Borde rojo + `FormMessage` / `role="alert"` al perder foco con < 10 chars. Borde default al cumplir requisito |

**Reglas de validacion frontend:**

| Regla | Mensaje |
|-------|---------|
| `motivo.length < 10` | "El motivo debe tener al menos 10 caracteres" |
| `motivo.length > 500` | "El motivo no puede superar 500 caracteres" (prevenido por `maxLength` en HTML) |
| Campo vacio al submit | "El motivo del rechazo es obligatorio" |

**Comportamiento:**
- Validacion se ejecuta `onBlur` (primera vez que el usuario abandona el campo).
- Una vez tocado (`touched=true`), la validacion se ejecuta `onChange` para feedback inmediato.
- El boton "Rechazar tarea" queda `disabled` cuando `!isValid || isPending`.
- Contador de caracteres actualizado `onChange`.
- Al abrir, campo resetea a cadena vacia y `touched=false`.

**Layout del campo:**
- Label encima con asterisco rojo
- Textarea debajo con minimo 80px de alto
- Fila debajo con error a la izquierda y contador a la derecha (en misma fila `flex justify-between`)

---

## 5. Estructura de Archivos Planificada

```
src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/
  components/
    TareasPendientesTab.tsx          NUEVO - Tab container con query, lista, paginacion
    TareaPendienteCard.tsx           NUEVO - Card individual de completado pendiente
    ValidarTareaDialog.tsx           NUEVO - Dialog de validacion con form opcional
    RechazarTareaDialog.tsx          NUEVO - Dialog de rechazo con form obligatorio
    PromoProgramaDetailClient.tsx    MODIFICAR - agregar tab "Pendientes"
    ...                              (existentes sin cambios)
```

**Hooks planificados:**
```
src/admin/src/hooks/
  use-tareas-pendientes.ts           NUEVO - useQuery para GET tareas-pendientes
  use-tareas-validar.ts              NUEVO - useMutation para PATCH validar + rechazar
```

**Utils a extraer/crear:**
```
src/admin/src/utils/
  date.ts                            NUEVO o AMPLIAR - extraer formatRelativeDate de SolicitudCard
  tarea-format.ts                    NUEVO - formatEjecucion (1ra, 2da, 3ra, Nta), displayUrl
```

---

## 6. Responsive Design

### TareasPendientesTab

| Breakpoint | Cambios de layout |
|------------|-------------------|
| Mobile (< 768px) | Sidebar colapsada (hamburger). Cards en columna unica full width. Paginacion apilada verticalmente (contador arriba, botones abajo). |
| Tablet (768px - 1024px) | Sidebar visible colapsada (iconos, 64px). Cards con layout de dos columnas (info + acciones lado a lado). |
| Desktop (> 1024px) | Sidebar expandida (256px). Layout completo segun diagramas. |

**Clases responsive para la columna de acciones en TareaPendienteCard:**
```tsx
className="flex flex-col sm:flex-row items-end sm:items-center gap-2 shrink-0"
```
En mobile (`< sm`): botones en columna (uno encima del otro), alineados al final derecho.
En sm+: botones en fila horizontal.

**URL de prueba truncada:**
```tsx
className="text-sm text-[#a855f7] hover:text-purple-400 truncate max-w-[180px] sm:max-w-[300px] inline-flex items-center gap-1 transition-colors"
```
En mobile: max-w-[180px]. En sm+: max-w-[300px].

### Dialogs ValidarTarea y RechazarTarea

| Breakpoint | Comportamiento |
|------------|----------------|
| Mobile (< 640px) | Dialog ocupa ancho completo como bottom sheet. `max-w-none mx-0` en el DialogContent. Bordes redondeados solo arriba (`rounded-t-xl rounded-b-none`). Height maximo 85vh con scroll interno (`overflow-y-auto max-h-[85vh]`). Se "desliza" desde abajo. |
| Desktop (>= 640px) | Dialog centrado `max-w-md`. Comportamiento estandar de modal. |

**Clases responsive para DialogContent:**
```tsx
className="bg-[#151525] border border-[#334155] text-white
    w-full sm:max-w-md rounded-t-xl sm:rounded-xl
    p-0 overflow-hidden
    fixed bottom-0 sm:bottom-auto left-0 sm:left-auto
    translate-x-0 translate-y-0 sm:translate-x-[-50%] sm:translate-y-[-50%]
    sm:top-[50%] sm:left-[50%]"
```

---

## 7. Feedback y Estados

### Loading

| Contexto | Componente | Implementacion |
|----------|------------|----------------|
| Carga inicial de la tab | `Skeleton` x3 | `<div className="h-[120px] rounded-lg bg-[#1e1e38] animate-pulse mb-3" />` |
| Boton Validar en proceso | `Button disabled` + `Loader2` | `<Loader2 className="w-4 h-4 animate-spin mr-2" />Validando...` |
| Boton Rechazar en proceso | `Button disabled` + `Loader2` | `<Loader2 className="w-4 h-4 animate-spin mr-2" />Rechazando...` |
| Botones de card durante operacion | `Button disabled` en ambos | `disabled={isAnyPending}` en Validar y Rechazar de la card |

### Error

| Contexto | Componente | Implementacion |
|----------|------------|----------------|
| Error carga lista | `Alert variant="destructive"` | `border-red-900/50 bg-red-950/20` con icono `AlertCircle` + boton Reintentar |
| Error API validar | `toast.error()` (sonner) | "No se pudo validar la tarea. Intentalo de nuevo." |
| Error API rechazar | `toast.error()` (sonner) | "No se pudo rechazar la tarea. Intentalo de nuevo." |
| Validacion campo motivo | `<p role="alert">` inline | Texto rojo debajo del textarea |

### Success

| Contexto | Componente | Implementacion |
|----------|------------|----------------|
| Tarea validada | `toast.success()` (sonner) | "Tarea validada. Se acreditaron {importe} {moneda} en la wallet del promotor." |
| Tarea rechazada | `toast()` (sonner, sin tipo) | "Tarea rechazada. El promotor podra re-enviar con nueva prueba." |
| Card procesada | Animacion fade-out | `isHiding=true`: `opacity-0 max-h-0 overflow-hidden transition-all duration-400` |

### Empty State

| Contexto | Visual |
|----------|--------|
| Sin tareas pendientes | `<div className="bg-[#1e1e38]/50 rounded-xl py-10 text-center">` con `<CheckCircle className="w-12 h-12 text-green-400 mx-auto mb-4" />` |
| Titulo | `<p className="text-base font-medium text-white">No hay tareas pendientes de validacion</p>` |
| Subtitulo | `<p className="text-sm text-[#64748b] mt-1">Todas las tareas completadas han sido revisadas</p>` |
| Sin CTA | No aplica boton (el usuario no puede crear tareas pendientes manualmente) |

---

## 8. Animaciones

| Elemento | Animacion | Clases Tailwind |
|----------|-----------|-----------------|
| Card procesada (tras validar/rechazar) | Fade-out + colapso de altura | `transition-all duration-400 opacity-0 max-h-0 overflow-hidden` |
| Apertura de dialog | Fade-in + scale-in (shadcn default) | `data-[state=open]:animate-in data-[state=open]:fade-in-0 data-[state=open]:zoom-in-95` |
| Cierre de dialog | Fade-out + scale-out (shadcn default) | `data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95` |
| Hover boton Validar | Cambio de fondo | `hover:bg-green-900/50 hover:border-green-700/50 transition-colors` |
| Hover boton Rechazar | Cambio de fondo | `hover:bg-red-950/50 hover:border-red-700/50 transition-colors` |
| Mensaje de error (campo motivo) | Fade-in desde arriba 4px | `animate-in fade-in-0 slide-in-from-top-1 duration-150` |
| Spinner loading | Rotacion continua | `animate-spin` en `Loader2` |
| Skeleton de carga | Pulso | `animate-pulse` |

---

## 9. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Tab trigger con count badge | `aria-label={"{N} tareas pendientes de validacion"}` en el Badge del tab |
| Boton Validar | Texto descriptivo visible "Validar". En estado loading: `aria-busy="true"` |
| Boton Rechazar | Texto descriptivo visible "Rechazar". En estado loading: `aria-busy="true"` |
| Dialog con titulo | `<DialogTitle>` vinculado automaticamente por Radix via `aria-labelledby` |
| Dialog con descripcion | `<DialogDescription className="sr-only">` para lectores de pantalla |
| Dialog con focus trap | Gestionado por Radix Dialog. Tab cicla dentro del dialog mientras esta abierto |
| Escape para cerrar | Radix Dialog lo implementa. Se bloquea durante `isPending` via `onOpenChange={isPending ? undefined : handler}` |
| Links URL de prueba | `rel="noopener noreferrer"` + texto visible (no solo icono) |
| Icono ExternalLink | `aria-hidden="true"` en todos los iconos decorativos |
| Tiempo relativo | `<time dateTime="{ISO string}">Hace 2 horas</time>` para semantica correcta |
| Campo motivo obligatorio | `aria-required="true"` en el Textarea + asterisco visual con `aria-hidden="true"` |
| Campo motivo con error | `aria-invalid="true"` + `aria-describedby="motivo-error"` cuando hay error |
| Mensaje de error | `id="motivo-error"` + `role="alert"` para anunciarse a lectores de pantalla |
| Botones con estado deshabilitado | `disabled` nativo en Button. Radix/shadcn agrega `aria-disabled` automaticamente |
| Botones de paginacion | `aria-label="Pagina anterior"` y `aria-label="Pagina siguiente"` en botones de icono |
| Contraste de colores | `#ffffff` sobre `#151525` = 12.6:1 (WCAG AAA). `#94a3b8` sobre `#151525` = 5.1:1 (WCAG AA). `#10b981` texto verde sobre `#151525` = 4.7:1 (WCAG AA). |
| Navegacion por teclado | Todos los botones alcanzables con Tab. Orden: botones Validar/Rechazar en la card, luego controles de paginacion. |

---

## 10. Integracion con PromoProgramaDetailClient

### Cambios necesarios en PromoProgramaDetailClient.tsx

1. Agregar import: `import { PromoProgramaTareasPendientesTab } from "./TareasPendientesTab"` (nota: nombre de componente con prefijo de proyecto para consistencia).

2. Agregar estado para el count de pendientes:
   ```tsx
   const [totalPendientes, setTotalPendientes] = useState(0)
   const handlePendientesCountChange = (count: number) => setTotalPendientes(count)
   ```

3. Agregar `TabsTrigger` de Pendientes en el `<TabsList>`:
   ```tsx
   <TabsTrigger value="pendientes" className="flex items-center gap-1.5">
       Pendientes
       {totalPendientes > 0 && (
           <Badge
               className="bg-red-500/20 text-red-400 border border-red-500/30 text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0"
               aria-label={`${totalPendientes} tareas pendientes de validacion`}
           >
               {totalPendientes}
           </Badge>
       )}
   </TabsTrigger>
   ```

4. Agregar `TabsContent` para Pendientes:
   ```tsx
   <TabsContent value="pendientes" className="mt-4">
       <PromoProgramaTareasPendientesTab
           programaId={programa.id}
           onCountChange={handlePendientesCountChange}
           tareasConRecompensa={programa.tareas}
       />
   </TabsContent>
   ```

5. **Posicion del tab "Pendientes":** Se inserta entre "Bloqueados" y "Resumen" para mantener el flujo logico (flujo de gestion del promotor: Solicitudes -> Aprobados -> Bloqueados -> Pendientes de validacion -> Resumen).

---

## 11. Checklist de Implementacion UI

### TareasPendientesTab
- [ ] Query con `useQuery` para `GET tareas-pendientes?page={page}&pageSize=10`
- [ ] Estado de loading con 3 Skeleton cards de h-[120px]
- [ ] Lista de `TareaPendienteCard` cuando hay items
- [ ] Paginacion con contador "Mostrando X de Y" y botones anterior/siguiente
- [ ] Empty state con `CheckCircle` verde cuando `items.length === 0`
- [ ] Error state con `Alert` + boton Reintentar
- [ ] Callback `onCountChange` para actualizar el badge del tab trigger
- [ ] Scroll to top del listado al cambiar de pagina

### TareaPendienteCard
- [ ] Nombre promotor con tipo entre parentesis
- [ ] Nombre tarea con numero de ejecucion formateado (1ra, 2da, etc.)
- [ ] URL de prueba clickable con truncamiento responsive
- [ ] Comentario del promotor en blockquote style, o "(sin comentario)" si ausente
- [ ] Fecha relativa con `<time dateTime>` semantico
- [ ] Botones Validar (verde) y Rechazar (rojo) con iconos
- [ ] Estado de carga en botones durante operaciones
- [ ] Fade-out animation al procesar exitosamente
- [ ] Ambos botones deshabilitados durante cualquier operacion pendiente

### ValidarTareaDialog
- [ ] Datos del completado en modo solo lectura (Tarea, Promotor, Ejecucion, URL, Comentario)
- [ ] Bloque verde de recompensa visible solo cuando `recompensa` prop existe
- [ ] Textarea opcional para comentario de validacion
- [ ] Boton Validar en verde con icono Check
- [ ] Estado loading durante PATCH validar
- [ ] Cierre bloqueado (overlay) durante loading
- [ ] Reset de campo al abrir/cerrar
- [ ] Focus inicial en textarea al abrir dialog

### RechazarTareaDialog
- [ ] Datos del completado en modo solo lectura (identico a ValidarTareaDialog)
- [ ] Bloque aviso amber con `AlertTriangle`
- [ ] Textarea obligatorio para motivo con validacion min 10 chars
- [ ] Contador de caracteres visible en tiempo real
- [ ] Mensaje de error `role="alert"` al perder foco con error
- [ ] Borde rojo en textarea cuando hay error
- [ ] Boton Rechazar en rojo con icono X
- [ ] Boton deshabilitado cuando motivo invalido o `isPending`
- [ ] Estado loading durante PATCH rechazar
- [ ] Cierre bloqueado (overlay) durante loading
- [ ] Reset de campo y `touched=false` al abrir/cerrar

### Accesibilidad (todos los componentes)
- [ ] Badge del tab con `aria-label` descriptivo
- [ ] `aria-busy` en botones durante loading
- [ ] `aria-required` y `aria-invalid` en campo motivo
- [ ] `aria-describedby` apuntando a mensaje de error
- [ ] `role="alert"` en mensajes de error
- [ ] `aria-hidden="true"` en todos los iconos decorativos
- [ ] `<time dateTime>` en fechas relativas
- [ ] `aria-label` en botones de paginacion
- [ ] Focus trap correcto en dialogs (gestionado por Radix)
- [ ] Contraste verificado para todos los textos
