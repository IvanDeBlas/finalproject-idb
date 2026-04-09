# Diseno UI: cp-tareas-promocion (Landing)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/web (Vite + React 18)
**Ruta de pantalla:** `/promotor/programas/:programaId/tareas`

---

## 1. Resumen

- Componentes shadcn utilizados: Card, Badge, Button, Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogClose, Form, FormField, FormItem, FormLabel, FormControl, FormMessage, Input, Textarea, Separator, Skeleton, Alert, AlertTitle, AlertDescription
- Composiciones custom: TareaCard, EstadoTareaBadge, TipoEventoBadge, RechazoBlock, PruebaEnviadaBlock, AccionTarea, CompletarTareaDialog, MisTareasPage, TareasSkeleton, EmptyStateTareas, ErrorStateTareas
- Responsive breakpoints: mobile (< 640px), tablet (640px - 1024px), desktop (> 1024px)
- Feature folder: `src/web/src/features/crowdpromotion/presentation/`

---

## 2. Paleta de Colores

| Uso | Valor | Ejemplo |
|-----|-------|---------|
| Fondo pagina | `#1a1a2e` | Fondo de la pagina de tareas |
| Fondo card | `#151525` | Fondo de TareaCard |
| Fondo card hover | `#1e1e38` | Hover en TareaCard |
| Fondo input | `#0f0f1f` | Inputs del dialog |
| CTA principal | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones "Completar tarea" |
| CTA hover | `hover:from-pink-600 hover:to-purple-700` | Hover en botones CTA |
| Border default | `#334155` | Bordes de cards, separadores |
| Border focus | `#a855f7` | Ring de foco en inputs |
| Texto primario | `#ffffff` | Titulos, nombres de tarea |
| Texto secundario | `#94a3b8` | Descripciones, subtitulos |
| Texto muted | `#64748b` | Labels auxiliares, fechas |
| Texto label | `#cbd5e1` | Labels de formulario |
| Link / accent | `#a855f7` | Links y elementos accent |
| Estado: no completada | `bg-[#1e1e38] text-[#94a3b8] border-[#334155]` | Badge sin completar |
| Estado: completada/pendiente | `bg-amber-950/50 text-amber-400 border-amber-800/50` | Badge pendiente validacion |
| Estado: validada | `bg-green-950/50 text-green-400 border-green-800/50` | Badge validada |
| Estado: rechazada | `bg-red-950/50 text-red-400 border-red-800/50` | Badge rechazada |
| Recompensa valor | `text-[#10b981]` | Importe de recompensa |

---

## 3. Componentes por Screen

### 3.1 MisTareasPage

**Ruta:** `/promotor/programas/:programaId/tareas`
**Componente archivo:** `pages/MisTareasPage.tsx`

#### Layout General

```
┌──────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (h-16, bg-[#0d0d1a], border-b border-[#334155])       │
│ [Logo WePlay Rises]  Explorar  Artistas  CrowdPromotion  [Avatar]    │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  BREADCRUMB (max-w-3xl mx-auto px-4, pt-6)                           │
│  Mis programas > Promociona mi nuevo album > Tareas                  │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  CABECERA (max-w-3xl mx-auto px-4, py-6)                             │
│  Tareas: Promociona mi nuevo album                                   │
│  Completa las tareas y acumula recompensas                           │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  LISTA DE TAREAS (max-w-3xl mx-auto px-4 pb-10)                      │
│                                                                      │
│  [ TareaCard: VALIDADA ]                                             │
│  [ TareaCard: No completada ]                                        │
│  [ TareaCard: RECHAZADA ]                                            │
│  [ TareaCard: PENDIENTE ]                                            │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

#### Seccion Breadcrumb

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor nav | `<nav>` | `flex items-center gap-1.5 text-xs text-[#64748b] mb-2` con `aria-label="Breadcrumb"` |
| Link "Mis programas" | `<Link>` de react-router-dom | `hover:text-[#94a3b8] transition-colors` - navega a `/promotor/mis-programas` |
| Separador | `<ChevronRight>` de lucide-react | `w-3 h-3 text-[#334155]` con `aria-hidden="true"` |
| Nombre del programa | `<span>` | `hover:text-[#94a3b8] cursor-pointer transition-colors` - truncado a 24 chars con `max-w-[12rem] truncate` |
| Separador | `<ChevronRight>` de lucide-react | `w-3 h-3 text-[#334155]` con `aria-hidden="true"` |
| "Tareas" (activo) | `<span>` | `text-[#94a3b8]` con `aria-current="page"` |

**Composicion:**

```tsx
<nav aria-label="Breadcrumb" className="flex items-center gap-1.5 text-xs text-[#64748b] mb-2">
    <Link to="/promotor/mis-programas" className="hover:text-[#94a3b8] transition-colors">
        Mis programas
    </Link>
    <ChevronRight className="w-3 h-3 text-[#334155]" aria-hidden="true" />
    <span className="max-w-[12rem] truncate hover:text-[#94a3b8] cursor-pointer transition-colors">
        {programaTitulo}
    </span>
    <ChevronRight className="w-3 h-3 text-[#334155]" aria-hidden="true" />
    <span className="text-[#94a3b8]" aria-current="page">Tareas</span>
</nav>
```

#### Seccion Cabecera de Pagina

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `mb-6` |
| Titulo | `<h1>` | `text-2xl font-bold text-white leading-tight` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |

**Composicion:**

```tsx
<div className="mb-6">
    <h1 className="text-2xl font-bold text-white leading-tight">
        Tareas: {programaTitulo}
    </h1>
    <p className="text-sm text-[#94a3b8] mt-1">
        Completa las tareas y acumula recompensas
    </p>
</div>
```

#### Seccion Lista de Tareas

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor lista | `<div>` | `flex flex-col gap-4` con `role="list"` |
| Item de lista | `<TareaCard>` | Ver seccion 3.2 |

---

### 3.2 TareaCard

**Archivo:** `components/TareaCard.tsx`
**Props:** `item: MisTareasItem`, `programaId: string`, `onCompletarClick: (item: MisTareasItem) => void`

#### Layout de la Card

```
┌──────────────────────────────────────────────────────────────────┐
│ [Badge tipo: Share]  [Badge: Repetible]     [Badge: VALIDADA]    │
│                                                                  │
│ Comparte en Instagram Stories                                    │
│ Sube una story de al menos 15 segundos mencionando...            │
│                                                                  │
│ ─────────────────────────────────────────────────────────────    │
│                                                                  │
│ Recompensa: 5 EUR por ejecucion                                  │
│ Completada: 3 de 10 veces  |  Ultima: 20 Mar 2026               │
│                                                                  │
│ [ Bloque de motivo de rechazo ]   (solo si RECHAZADA)           │
│ [ Bloque de prueba enviada ]      (solo si PENDIENTE)           │
│                                                                  │
│                              [Boton de accion segun estado]      │
└──────────────────────────────────────────────────────────────────┘
```

#### Elementos de la Card

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Card contenedor | `<Card>` | `bg-[#151525] border border-[#334155] p-5 flex flex-col gap-3 transition-colors duration-200 hover:border-[#a855f7]/30 cursor-default` con `role="listitem"` |
| Fila superior | `<div>` | `flex items-start justify-between gap-2 flex-wrap` |
| Grupo badges izquierda | `<div>` | `flex items-center gap-2 flex-wrap` |
| Badge tipo evento | `<TipoEventoBadge>` | Ver seccion 4.1 |
| Badge repetible | `<Badge>` | `bg-[#1e1e38] text-[#64748b] border border-[#334155] text-xs font-medium inline-flex items-center gap-1` - solo si `esRepetible === true` |
| Icono badge repetible | `<RefreshCw>` de lucide | `w-3 h-3` con `aria-hidden="true"` |
| Badge estado | `<EstadoTareaBadge>` | Ver seccion 4.2 |
| Nombre tarea | `<h3>` | `text-base font-semibold text-white leading-tight` |
| Descripcion | `<p>` | `text-sm text-[#94a3b8] leading-relaxed line-clamp-3` |
| Separador | `<Separator>` | `bg-[#334155]` |
| Fila recompensa | `<div>` | `flex items-center gap-2` - visible solo si `importeRecompensa` o `puntosRecompensa` existen |
| Icono recompensa | `<DollarSign>` o `<Star>` de lucide | `w-3.5 h-3.5 text-[#64748b] shrink-0` con `aria-hidden="true"` |
| Label recompensa | `<span>` | `text-xs text-[#64748b]` |
| Valor recompensa | `<span>` | `text-sm font-semibold text-[#10b981]` |
| Fila progreso | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` - visible si `esRepetible && miEstado?.vecesCompletada > 0` |
| Texto progreso | `<span>` | `text-xs text-[#64748b]` |
| Separador punto | `<span>` | `text-[#334155]` |
| Fecha ultima | `<time>` | `text-xs text-[#64748b]` con `dateTime={fechaIso}` |
| Bloque rechazo | `<RechazoBlock>` | Ver seccion 4.3 - visible si estado == 4 (RECHAZADA) |
| Bloque prueba enviada | `<PruebaEnviadaBlock>` | Ver seccion 4.4 - visible si estado == 2 (COMPLETADA/pendiente) |
| Fila de accion | `<div>` | `flex items-center justify-end mt-1` |
| Boton de accion | `<AccionTarea>` | Ver seccion 4.5 |

**Composicion:**

```tsx
<Card
    className="bg-[#151525] border border-[#334155] p-5 flex flex-col gap-3 transition-colors duration-200 hover:border-[#a855f7]/30"
    role="listitem"
>
    {/* Fila superior: badges + estado */}
    <div className="flex items-start justify-between gap-2 flex-wrap">
        <div className="flex items-center gap-2 flex-wrap">
            <TipoEventoBadge tipo={item.tipoEventoPromoNombre} />
            {item.esRepetible && (
                <Badge className="bg-[#1e1e38] text-[#64748b] border border-[#334155] text-xs font-medium inline-flex items-center gap-1">
                    <RefreshCw className="w-3 h-3" aria-hidden="true" />
                    Repetible
                </Badge>
            )}
        </div>
        <EstadoTareaBadge miEstado={item.miEstado} />
    </div>

    {/* Nombre y descripcion */}
    <div>
        <h3 className="text-base font-semibold text-white leading-tight">{item.nombre}</h3>
        {item.descripcion && (
            <p className="text-sm text-[#94a3b8] leading-relaxed line-clamp-3 mt-1">
                {item.descripcion}
            </p>
        )}
    </div>

    <Separator className="bg-[#334155]" />

    {/* Recompensa */}
    {(item.importeRecompensa || item.puntosRecompensa) && (
        <div className="flex items-center gap-2">
            <DollarSign className="w-3.5 h-3.5 text-[#64748b] shrink-0" aria-hidden="true" />
            <span className="text-xs text-[#64748b]">Recompensa:</span>
            <span className="text-sm font-semibold text-[#10b981]">
                {formatRecompensa(item)}
            </span>
        </div>
    )}

    {/* Progreso de repeticiones */}
    {item.esRepetible && item.miEstado && item.miEstado.vecesCompletada > 0 && (
        <div className="flex items-center gap-2 text-xs text-[#64748b]">
            <span>Completada: {item.miEstado.vecesCompletada} de {item.maxRepeticiones ?? "N"} veces</span>
            {item.miEstado.fechaUltimaCompletada && (
                <>
                    <span className="text-[#334155]">|</span>
                    <span>Ultima: </span>
                    <time dateTime={item.miEstado.fechaUltimaCompletada}>
                        {formatFechaRelativa(item.miEstado.fechaUltimaCompletada)}
                    </time>
                </>
            )}
        </div>
    )}

    {/* Bloques condicionales */}
    {item.miEstado?.estadoTareaId === 4 && (
        <RechazoBlock comentario={item.miEstado.comentarioValidacion} />
    )}
    {item.miEstado?.estadoTareaId === 2 && (
        <PruebaEnviadaBlock
            urlPrueba={item.miEstado.urlPruebaCompletado}
            fecha={item.miEstado.fechaUltimaCompletada}
        />
    )}

    {/* Accion */}
    <div className="flex items-center justify-end mt-1">
        <AccionTarea item={item} onCompletarClick={onCompletarClick} />
    </div>
</Card>
```

---

## 4. Componentes Atomicos

### 4.1 TipoEventoBadge

**Archivo:** `components/TipoEventoBadge.tsx`
**Props:** `tipo: string`

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Badge | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium` |

**Composicion:**

```tsx
<Badge
    variant="outline"
    className="bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium"
>
    {tipo}
</Badge>
```

---

### 4.2 EstadoTareaBadge

**Archivo:** `components/EstadoTareaBadge.tsx`
**Props:** `miEstado: MiEstadoTarea | undefined`

Cuando `miEstado` es `undefined` (tarea nunca intentada), muestra "No completada".

| Estado | estadoTareaId | Icono | Clases del Badge |
|--------|--------------|-------|------------------|
| No completada | `undefined` | - | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155]` |
| Completada (pendiente) | `2` | `<Clock>` | `bg-amber-950/50 text-amber-400 border border-amber-800/50` |
| Validada | `3` | `<CheckCircle>` | `bg-green-950/50 text-green-400 border border-green-800/50` |
| Rechazada | `4` | `<XCircle>` | `bg-red-950/50 text-red-400 border border-red-800/50` |

**Nota ARIA:** Cada badge tiene `role="status"` y `aria-label="Estado: {label}"`. El icono tiene `aria-hidden="true"`.

**Composicion:**

```tsx
const ESTADO_BADGE_CONFIG: Record<number, { label: string; classes: string; Icon?: LucideIcon }> = {
    2: { label: "PENDIENTE",    classes: "bg-amber-950/50 text-amber-400 border border-amber-800/50", Icon: Clock },
    3: { label: "VALIDADA",     classes: "bg-green-950/50 text-green-400 border border-green-800/50", Icon: CheckCircle },
    4: { label: "RECHAZADA",    classes: "bg-red-950/50 text-red-400 border border-red-800/50",       Icon: XCircle },
}
const DEFAULT_CONFIG = { label: "No completada", classes: "bg-[#1e1e38] text-[#94a3b8] border border-[#334155]" }

const config = miEstado ? (ESTADO_BADGE_CONFIG[miEstado.estadoTareaId] ?? DEFAULT_CONFIG) : DEFAULT_CONFIG

<Badge
    className={cn("text-xs font-medium inline-flex items-center gap-1 px-2.5 py-1", config.classes)}
    role="status"
    aria-label={`Estado: ${config.label}`}
>
    {config.Icon && <config.Icon className="w-3 h-3" aria-hidden="true" />}
    {config.label}
</Badge>
```

---

### 4.3 RechazoBlock

**Archivo:** `components/RechazoBlock.tsx`
**Props:** `comentario: string | undefined`

Visible solo cuando `estadoTareaId === 4`. Muestra el motivo de rechazo del artista en texto plano para maxima accesibilidad.

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `bg-red-950/20 border border-red-900/40 rounded-lg p-3 flex items-start gap-2.5` con `role="alert"` |
| Icono | `<AlertCircle>` | `w-4 h-4 text-red-400 shrink-0 mt-0.5` con `aria-hidden="true"` |
| Wrapper texto | `<div>` | `flex flex-col gap-0.5` |
| Label | `<p>` | `text-xs font-medium text-red-400` |
| Texto motivo | `<p>` | `text-sm text-red-300/80 leading-relaxed` |

**Composicion:**

```tsx
<div
    className="bg-red-950/20 border border-red-900/40 rounded-lg p-3 flex items-start gap-2.5"
    role="alert"
>
    <AlertCircle className="w-4 h-4 text-red-400 shrink-0 mt-0.5" aria-hidden="true" />
    <div className="flex flex-col gap-0.5">
        <p className="text-xs font-medium text-red-400">Motivo del rechazo:</p>
        <p className="text-sm text-red-300/80 leading-relaxed">
            {comentario ?? "Sin motivo especificado"}
        </p>
    </div>
</div>
```

---

### 4.4 PruebaEnviadaBlock

**Archivo:** `components/PruebaEnviadaBlock.tsx`
**Props:** `urlPrueba: string | undefined`, `fecha: string | undefined`

Visible solo cuando `estadoTareaId === 2` (Completada, pendiente de validacion).

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `bg-amber-950/20 border border-amber-900/40 rounded-lg p-3 flex items-center gap-2.5` |
| Icono | `<Clock>` | `w-4 h-4 text-amber-400 shrink-0` con `aria-hidden="true"` |
| Texto | `<p>` | `text-xs text-amber-300/80` |
| Fecha | `<time>` | `dateTime={fechaIso}` con formato legible |

**Composicion:**

```tsx
<div className="bg-amber-950/20 border border-amber-900/40 rounded-lg p-3 flex items-center gap-2.5">
    <Clock className="w-4 h-4 text-amber-400 shrink-0" aria-hidden="true" />
    <p className="text-xs text-amber-300/80">
        Enviada:{" "}
        {fecha ? (
            <time dateTime={fecha}>{formatFechaLegible(fecha)}</time>
        ) : "—"}
        {" "}- Pendiente de validacion del artista
    </p>
</div>
```

---

### 4.5 AccionTarea

**Archivo:** `components/AccionTarea.tsx`
**Props:** `item: MisTareasItem`, `onCompletarClick: (item: MisTareasItem) => void`, `isSubmitting?: boolean`

Este componente encapsula la logica de que boton o texto mostrar segun el estado y las condiciones de negocio. Usa `puedeCompletarTarea(item)` del shared utils.

| Variante de estado | Condicion | Elemento | Estilos / Notas |
|-------------------|-----------|----------|-----------------|
| Nunca completada | `!miEstado` | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-9 px-4 text-sm` - Icono `<Play>` - Texto "Completar tarea" |
| Repetible, puede volver | `puedeCompletarTarea(item) && esRepetible` | `<Button>` | Mismo estilo gradient - Icono `<RefreshCw>` - Texto "Completar de nuevo" |
| Rechazada (re-enviar) | `estadoTareaId === 4` | `<Button variant="outline">` | `border-[#a855f7]/50 text-[#a855f7] hover:bg-[#a855f7]/10 hover:border-[#a855f7] h-9 px-4 text-sm` - Icono `<RotateCcw>` - Texto "Re-enviar con nueva prueba" |
| Pendiente validacion | `estadoTareaId === 2` | `<Button variant="ghost">` | `text-[#64748b] hover:text-[#94a3b8] h-9 px-4 text-sm` - Icono `<ExternalLink>` - Texto "Ver prueba enviada" - Abre URL en nueva tab |
| Validada (no repetible) | `estadoTareaId === 3 && !esRepetible` | `null` | Sin boton - solo el badge de estado |
| Max repeticiones | `!puedeCompletarTarea(item) && vecesCompletada >= maxRepeticiones` | `<div>` | `text-xs text-[#64748b] flex items-center gap-1.5` - Icono `<Lock>` - Texto "Maximo de repeticiones alcanzado" |

**Estado loading:** Todos los botones de completar muestran `<Loader2 className="w-4 h-4 animate-spin">` con texto "Enviando..." y `disabled` cuando `isSubmitting === true`.

**Nota ARIA:** Boton en loading tiene `aria-busy="true"` y el spinner tiene `aria-label="Cargando"`.

**Composicion (fragmento logica):**

```tsx
// Prioridad de estados: rechazada > pendiente > validada sin repeticion > puede completar > bloqueado
if (miEstado?.estadoTareaId === 4) {
    return <Button variant="outline" size="sm" className="border-[#a855f7]/50 text-[#a855f7] ..." onClick={...}>
        <RotateCcw className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
        Re-enviar con nueva prueba
    </Button>
}
if (miEstado?.estadoTareaId === 2) {
    return <Button variant="ghost" size="sm" className="text-[#64748b] ..." asChild>
        <a href={miEstado.urlPruebaCompletado} target="_blank" rel="noopener noreferrer">
            <ExternalLink className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
            Ver prueba enviada
        </a>
    </Button>
}
if (!puedeCompletarTarea(item)) {
    return <div className="text-xs text-[#64748b] flex items-center gap-1.5">
        <Lock className="w-3.5 h-3.5" aria-hidden="true" />
        Maximo de repeticiones alcanzado
    </div>
}
const label = item.miEstado ? "Completar de nuevo" : "Completar tarea"
const Icon = item.miEstado ? RefreshCw : Play
return <Button size="sm" className="bg-gradient-to-r from-pink-500 to-purple-600 ..." onClick={...} disabled={isSubmitting} aria-busy={isSubmitting}>
    {isSubmitting ? <Loader2 className="w-4 h-4 animate-spin" aria-label="Cargando" /> : <Icon className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />}
    {isSubmitting ? "Enviando..." : label}
</Button>
```

---

## 5. Formularios

### 5.1 CompletarTareaDialog

**Archivo:** `components/CompletarTareaDialog.tsx`
**Props:** `open: boolean`, `onOpenChange: (open: boolean) => void`, `tarea: MisTareasItem`, `isReenvio: boolean`, `isSubmitting: boolean`, `onSubmit: (data: CompletarTareaFormData) => void`

#### Layout del Dialog

```
┌──────────────────────────────────────────────────────────┐
│  [X]                                                     │
│                                                          │
│  Completar: Comparte en Instagram Stories                │
│  (linea separadora border-b)                             │
│                                                          │
│  [Nota de re-envio - solo si isReenvio=true]             │
│                                                          │
│  INSTRUCCIONES                                           │
│  Sube una story de al menos 15 segundos...               │
│  [Ver instrucciones completas ->]  (si hay URL)          │
│                                                          │
│  (linea separadora)                                      │
│                                                          │
│  URL de prueba *                                         │
│  [ https://instagram.com/stories/...          ]         │
│  (mensaje de error si aplica)                            │
│                                                          │
│  Comentario (opcional)                                   │
│  [ Describe brevemente la accion...           ]         │
│  0 / 500 caracteres                                      │
│                                                          │
│  (linea separadora border-t)                             │
│  [Cancelar]                    [Enviar prueba]           │
└──────────────────────────────────────────────────────────┘
```

#### Campos del Formulario

| Campo | Componente | Validacion | Notas |
|-------|------------|-----------|-------|
| urlPruebaCompletado | `<Input type="url">` | `completarTareaSchema.urlPruebaCompletado` | Requerido, max 2048, formato URL. Validacion `onBlur` |
| comentarioPromotor | `<Textarea>` | `completarTareaSchema.comentarioPromotor` | Opcional, max 500. Contador `onChange` |

#### Estructura de Componentes Dialog

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Raiz dialog | `<Dialog>` | `open={open} onOpenChange={isSubmitting ? undefined : onOpenChange}` - bloquear cierre durante envio |
| Contenido | `<DialogContent>` | `bg-[#151525] border border-[#334155] text-white max-w-lg w-full rounded-xl p-0 overflow-hidden` |
| Boton cerrar X | `<DialogClose>` | `absolute top-4 right-4 text-[#64748b] hover:text-white transition-colors rounded-sm focus-visible:ring-2 focus-visible:ring-[#a855f7]` - `aria-label="Cerrar dialog"` |
| Header | `<div>` | `px-6 pt-6 pb-4 border-b border-[#334155]` |
| Titulo | `<DialogTitle>` | `text-xl font-bold text-white` - texto dinamico segun contexto |
| Nota re-envio | `<div>` | `mx-6 mt-4 bg-amber-950/30 border border-amber-800/50 rounded-lg p-3 flex items-start gap-2.5` - solo si `isReenvio` |
| Icono nota | `<Info>` | `w-4 h-4 text-amber-400 shrink-0 mt-0.5` con `aria-hidden="true"` |
| Texto nota | `<div>` | `<p className="text-sm text-amber-300 font-medium">` + `<p className="text-xs text-amber-300/70 mt-0.5">` |
| Seccion instrucciones | `<div>` | `px-6 py-4` |
| Label instrucciones | `<p>` | `text-xs font-semibold text-[#94a3b8] uppercase tracking-wide mb-2` |
| Texto instrucciones | `<p>` | `text-sm text-[#cbd5e1] leading-relaxed` |
| Link instrucciones ext. | `<a>` | `inline-flex items-center gap-1 text-xs text-[#a855f7] hover:text-purple-400 mt-2 transition-colors` con `target="_blank" rel="noopener noreferrer"` - solo si `instruccionesUrl` existe |
| Separador form | `<Separator>` | `bg-[#334155] mx-6` |
| Contenedor campos | `<div>` | `px-6 py-4 flex flex-col gap-5` |
| Campo URL | `<FormField name="urlPruebaCompletado">` | Ver detalle abajo |
| Campo comentario | `<FormField name="comentarioPromotor">` | Ver detalle abajo |
| Footer | `<div>` | `px-6 pb-6 pt-4 border-t border-[#334155] flex items-center justify-end gap-3` |
| Boton cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` - `disabled={isSubmitting}` |
| Boton enviar | `<Button type="submit">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-6` - `disabled={isSubmitting}` |

#### Detalle FormField URL de Prueba

```tsx
<FormField
    control={form.control}
    name="urlPruebaCompletado"
    render={({ field }) => (
        <FormItem>
            <FormLabel className="text-sm font-medium text-[#cbd5e1]">
                URL de prueba
                <span className="text-red-400 ml-1" aria-hidden="true">*</span>
                <span className="sr-only">(requerido)</span>
            </FormLabel>
            <FormControl>
                <Input
                    {...field}
                    type="url"
                    placeholder="https://instagram.com/stories/..."
                    className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] h-10 focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30"
                    aria-required="true"
                    aria-describedby="url-prueba-error"
                />
            </FormControl>
            <FormMessage id="url-prueba-error" className="text-xs text-red-400 mt-1" />
        </FormItem>
    )}
/>
```

#### Detalle FormField Comentario

```tsx
<FormField
    control={form.control}
    name="comentarioPromotor"
    render={({ field }) => (
        <FormItem>
            <FormLabel className="text-sm font-medium text-[#cbd5e1]">
                Comentario <span className="text-[#64748b] font-normal">(opcional)</span>
            </FormLabel>
            <FormControl>
                <Textarea
                    {...field}
                    placeholder="Describe brevemente la accion realizada..."
                    className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[80px] resize-none focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30"
                    maxLength={500}
                    aria-describedby="comentario-counter comentario-error"
                />
            </FormControl>
            <div className="flex justify-between items-center mt-1">
                <FormMessage id="comentario-error" className="text-xs text-red-400" />
                <p
                    id="comentario-counter"
                    className="text-right text-xs text-[#64748b] ml-auto"
                    aria-live="polite"
                    aria-atomic="true"
                >
                    {field.value?.length ?? 0} / 500 caracteres
                </p>
            </div>
        </FormItem>
    )}
/>
```

#### Estados del Dialog

| Estado | Visual |
|--------|--------|
| Default (abierto) | Campos vacios, boton "Enviar prueba" habilitado |
| Campo URL invalido (blur) | Border `border-red-500`, `<FormMessage>` con animacion `fade-in` visible debajo |
| Enviando | Boton "Enviar prueba" con `<Loader2 animate-spin>` + "Enviando...", `disabled`. Boton Cancelar `disabled`. Click en overlay no cierra |
| Exito | Dialog se cierra via `onOpenChange(false)`. Toast de exito en pagina. Card actualiza estado |
| Error de API | Dialog permanece abierto. Toast destructivo. Boton vuelve al estado normal |

---

## 6. Estados de UI Globales

### 6.1 Estado Loading

**Archivo:** `components/TareasSkeleton.tsx`

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `flex flex-col gap-4` |
| Card skeleton (x3) | `<Skeleton>` | `bg-[#1e1e38] rounded-xl h-[180px]` con `animate-pulse` |

**Composicion:**

```tsx
<div className="flex flex-col gap-4" aria-busy="true" aria-label="Cargando tareas">
    {Array.from({ length: 3 }).map((_, i) => (
        <Skeleton key={i} className="bg-[#1e1e38] rounded-xl h-[180px]" />
    ))}
</div>
```

---

### 6.2 Empty State (sin tareas)

**Archivo:** `components/EmptyStateTareas.tsx`

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center` |
| Icono | `<ClipboardX>` | `w-12 h-12 text-[#64748b]` con `aria-hidden="true"` |
| Titulo | `<p>` | `text-base font-semibold text-[#94a3b8]` |
| Subtitulo | `<p>` | `text-sm text-[#64748b]` |

**Composicion:**

```tsx
<div className="bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center">
    <ClipboardX className="w-12 h-12 text-[#64748b]" aria-hidden="true" />
    <p className="text-base font-semibold text-[#94a3b8]">
        Este programa no tiene tareas activas
    </p>
    <p className="text-sm text-[#64748b]">
        El artista aun no ha configurado tareas de promocion
    </p>
</div>
```

---

### 6.3 Error State

**Archivo:** `components/ErrorStateTareas.tsx`

| Elemento | Componente | Tailwind / Notas |
|----------|------------|------------------|
| Contenedor | `<div>` | `bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center` |
| Icono | `<AlertCircle>` | `w-8 h-8 text-red-400` con `aria-hidden="true"` |
| Texto | `<p>` | `text-sm text-[#94a3b8]` |
| Boton reintentar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] mt-2` |

**Composicion:**

```tsx
<div
    className="bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center"
    role="alert"
>
    <AlertCircle className="w-8 h-8 text-red-400" aria-hidden="true" />
    <p className="text-sm text-[#94a3b8]">No se pudieron cargar las tareas</p>
    <Button
        variant="outline"
        size="sm"
        className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] mt-2"
        onClick={onRetry}
    >
        Reintentar
    </Button>
</div>
```

---

### 6.4 Toast Messages

Usar `sonner` (biblioteca ya disponible en el proyecto) mediante la funcion `toast`.

| Evento | Variante | Mensaje |
|--------|----------|---------|
| Tarea completada con exito | `toast.success` | "Tarea enviada. El artista revisara tu prueba." |
| Tarea re-enviada | `toast.success` | "Prueba re-enviada para validacion." |
| Tarea no repetible ya completada (4027) | `toast.error` | "Esta tarea ya fue completada y no es repetible." |
| Max repeticiones alcanzado (4028) | `toast.error` | "Alcanzaste el maximo de repeticiones para esta tarea." |
| Programa inactivo (4024) | `toast.error` | "Este programa no esta activo. No puedes completar tareas." |
| Tarea no disponible (4029) | `toast.error` | "Esta tarea ya no esta disponible." |
| Plazo finalizado (4030) | `toast.error` | "El plazo para completar esta tarea ha finalizado." |
| Error inesperado (5000) | `toast.error` | "Ocurrio un error inesperado. Intentalo de nuevo." |

---

## 7. Responsive Design

### 7.1 MisTareasPage

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Contenedor full width `px-4`. Fila superior de card en columna: badges arriba, estado abajo. Boton de accion ocupa todo el ancho (`w-full`). Breadcrumb con nombre de programa truncado mas agresivamente (`max-w-[8rem]`) |
| Tablet | 640px - 1024px | `max-w-2xl mx-auto`. Cards con layout de fila. Botones en esquina derecha |
| Desktop | > 1024px | `max-w-3xl mx-auto`. Layout optimo con toda la informacion visible |

**Clases responsivas card:**

```tsx
className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2"
// Fila superior en mobile: columna. En sm+: fila con justify-between
```

**Clases responsivas boton de accion:**

```tsx
className="w-full sm:w-auto"
// En mobile: full width. En sm+: ancho automatico
```

### 7.2 CompletarTareaDialog (Responsive)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Dialog como bottom sheet: `fixed bottom-0 left-0 right-0 max-w-none mx-0 rounded-t-xl rounded-b-none`. Se desliza desde abajo. Height maximo `max-h-[85vh]` con scroll interno en el cuerpo del dialog |
| Tablet / Desktop | >= 640px | Dialog centrado `max-w-lg` en el viewport. Comportamiento estandar de modal |

**Implementacion bottom sheet en mobile:**

```tsx
<DialogContent
    className={cn(
        "bg-[#151525] border border-[#334155] text-white p-0 overflow-hidden",
        "sm:max-w-lg sm:rounded-xl",
        // Mobile bottom sheet
        "max-sm:fixed max-sm:bottom-0 max-sm:left-0 max-sm:right-0 max-sm:max-w-none max-sm:w-full max-sm:rounded-t-xl max-sm:rounded-b-none max-sm:translate-x-0 max-sm:translate-y-0 max-sm:top-auto"
    )}
>
    {/* Handle visual para bottom sheet */}
    <div className="sm:hidden flex justify-center pt-3 pb-1">
        <div className="w-10 h-1 bg-[#334155] rounded-full" aria-hidden="true" />
    </div>
    {/* Scroll area en mobile */}
    <div className="max-sm:max-h-[85vh] max-sm:overflow-y-auto">
        {/* Contenido del dialog */}
    </div>
</DialogContent>
```

---

## 8. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Blanco `#ffffff` sobre `#151525` = 12.6:1 (WCAG AAA). Texto `#94a3b8` sobre `#151525` = 5.1:1 (WCAG AA). Ambos cumplen AA |
| Focus visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0d0d1a]` en todos los interactivos. Nunca `outline: none` sin reemplazo |
| Dialog focus trap | `<Dialog>` de shadcn/Radix atrapa el foco automaticamente con `aria-modal="true"`. Escape cierra cuando no hay envio en curso |
| Labels de formulario | Cada `<Input>` y `<Textarea>` vinculado a `<FormLabel>` via `<FormControl>` de shadcn. `aria-required="true"` en campos obligatorios |
| Errores accesibles | `<FormMessage>` tiene `role="alert"` implicito. Inputs con error tienen `aria-invalid="true"` y `aria-describedby` apuntando al ID del mensaje |
| Badges de estado | Texto completo legible: "VALIDADA", "RECHAZADA", no solo color. Badge con `role="status"` y `aria-label` descriptivo |
| Iconos decorativos | Todos los iconos tienen `aria-hidden="true"`. Iconos funcionales tienen `aria-label` explicito |
| Botones de icono solo | Boton X del dialog tiene `aria-label="Cerrar dialog"` y `<span className="sr-only">Cerrar</span>` |
| Fechas relativas | Usar `<time dateTime={isoString}>` con texto legible como hijo |
| Estados de carga | Botones en loading: `aria-busy="true"`, `aria-disabled="true"`. Spinner: `aria-label="Cargando"` |
| Listas de tareas | Contenedor con `role="list"`, cada card con `role="listitem"` |
| Breadcrumb | `<nav aria-label="Breadcrumb">` con `aria-current="page"` en el item activo |
| RechazoBlock | `role="alert"` para que lectores de pantalla lo anuncien inmediatamente |
| Contador de caracteres | `aria-live="polite"` y `aria-atomic="true"` para anunciar cambios sin interrumpir |
| Skip link | El layout de landing debe incluir `<a href="#main-content" className="sr-only focus:not-sr-only">Ir al contenido principal</a>` |

---

## 9. Animaciones y Transiciones

| Elemento | Animacion | Clases Tailwind | Trigger |
|----------|-----------|-----------------|---------|
| Apertura dialog | fade-in + zoom-in-95 | Gestionado por Radix Dialog (`data-[state=open]:animate-in data-[state=open]:fade-in-0 data-[state=open]:zoom-in-95`) | Click en boton de accion |
| Cierre dialog | fade-out + zoom-out-95 | `data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95` | Cancelar / exito |
| Hover card | Border color | `transition-colors duration-200` | Mouse enter/leave |
| Hover botones | Background | `transition-colors` (ya incluido en `buttonVariants`) | Mouse enter/leave |
| Badge de estado (cambio) | Color + background | `transition-all duration-300` | Tras API success |
| Spinner de loading | Rotacion continua | `animate-spin` | Durante envio |
| Mensaje error campo | Aparicion | `animate-in fade-in-0 slide-in-from-top-1` | onBlur con error |
| Skeleton | Pulse | `animate-pulse` (2s ciclo) | Durante fetch inicial |
| Bottom sheet mobile (apertura) | Slide desde abajo | `data-[state=open]:slide-in-from-bottom` | Click boton |

---

## 10. Estructura de Archivos Planificada

```
src/web/src/features/crowdpromotion/
  presentation/
    components/
      TareaCard.tsx                  # Card principal de tarea del promotor
      TipoEventoBadge.tsx            # Badge para tipo de evento (Share, Post, etc.)
      EstadoTareaBadge.tsx           # Badge de estado de tarea (4 variantes)
      RechazoBlock.tsx               # Bloque de motivo de rechazo (estado 4)
      PruebaEnviadaBlock.tsx         # Bloque de prueba enviada pendiente (estado 2)
      AccionTarea.tsx                # Boton de accion dinamico segun estado
      CompletarTareaDialog.tsx       # Dialog de formulario completar tarea
      TareasSkeleton.tsx             # Skeleton de 3 cards para loading
      EmptyStateTareas.tsx           # Empty state cuando no hay tareas
      ErrorStateTareas.tsx           # Error state con boton reintentar
    pages/
      MisTareasPage.tsx              # Pagina principal de tareas del promotor
```

---

## 11. Checklist UI

- [ ] Breadcrumb con links funcionales y `aria-current="page"` en item activo
- [ ] Titulo de pagina con nombre del programa del API response
- [ ] Lista de tareas con `role="list"` y cards con `role="listitem"`
- [ ] TareaCard renderizada para los 4 estados: no completada, pendiente (2), validada (3), rechazada (4)
- [ ] TipoEventoBadge visible en todas las cards
- [ ] Badge "Repetible" visible solo si `esRepetible === true`
- [ ] EstadoTareaBadge en esquina superior derecha con icono + texto correcto por estado
- [ ] Seccion de recompensa visible solo si existe importe o puntos
- [ ] Progreso de repeticiones visible solo en repetibles con al menos una ejecucion
- [ ] RechazoBlock visible solo en estado 4 (Rechazada) con motivo del artista
- [ ] PruebaEnviadaBlock visible solo en estado 2 (Completada/pendiente) con fecha
- [ ] AccionTarea muestra el boton correcto por cada combinacion de estado y condicion
- [ ] Botones deshabilitados + texto "Maximo de repeticiones alcanzado" cuando corresponde
- [ ] TareasSkeleton mostrado durante fetch inicial (3 placeholders)
- [ ] EmptyStateTareas mostrado cuando `items.length === 0`
- [ ] ErrorStateTareas mostrado en error de fetch con boton "Reintentar" funcional
- [ ] CompletarTareaDialog abre desde los tres botones de accion (Completar / De nuevo / Re-enviar)
- [ ] Titulo del dialog cambia segun contexto de apertura
- [ ] Nota de re-envio visible solo cuando `isReenvio === true` (estado previo era rechazada)
- [ ] Instrucciones de tarea mostradas en el dialog
- [ ] Link a instrucciones externas visible solo si `instruccionesUrl` existe
- [ ] Campo URL con validacion `onBlur` y mensaje de error accesible
- [ ] Campo comentario con contador de caracteres actualizado `onChange`
- [ ] Boton Enviar en loading: spinner + "Enviando..." + `disabled` + `aria-busy="true"`
- [ ] Dialog no se puede cerrar durante envio (Escape, click overlay y X bloqueados)
- [ ] Toast de exito tras completar tarea
- [ ] Toast de error si la API rechaza la peticion
- [ ] Todos los inputs tienen `<FormLabel>` asociado
- [ ] Errores de formulario con `role="alert"` y `aria-describedby` en inputs
- [ ] Focus visible con ring purple en todos los elementos interactivos
- [ ] Iconos decorativos con `aria-hidden="true"`
- [ ] Boton X del dialog con `aria-label="Cerrar dialog"`
- [ ] Fechas con `<time dateTime={isoString}>`
- [ ] Responsive mobile: botones full width, fila de badges en columna, bottom sheet
- [ ] Responsive tablet: `max-w-2xl` centrado
- [ ] Responsive desktop: `max-w-3xl` centrado
- [ ] Dialog bottom sheet en mobile con handle visual y scroll interno
- [ ] Contraste de colores cumple WCAG AA en todos los elementos de texto
