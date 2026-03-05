# Diseno UI: cp-inscripcion-programa (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

- Componentes shadcn utilizados: Card, CardHeader, CardContent, Badge, Button, Input, Select, Separator, Skeleton, Tooltip, TooltipProvider, TooltipTrigger, TooltipContent, Collapsible, CollapsibleTrigger, CollapsibleContent, Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationPrevious, PaginationNext, Alert
- Composiciones custom: 9 (ProgramaCard, EstadoInscripcionBadge, ProgramaActionSlot, FiltroProgramas, ProgramaCardSkeleton, ProgramaGrid, MiProgramaItem, CodigoReferidoBlock, UrlTrackingBlock, ProgramaDetallExpandido, EmptyStateProgramas, BannerSinPerfilPromotor, KpiStatCard, TareaItem)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)
- Pantallas: Explorar Programas (/crowdpromotion/explorar), Mis Programas (/promotor/mis-programas)

---

## 2. Paleta de Colores

| Uso | Variable CSS | Valor | Ejemplo |
|-----|-------------|-------|---------|
| Fondo base | --bg-primary | `#0d0d1a` | Header, hero section |
| Fondo pagina | --bg-secondary | `#1a1a2e` | Fondo contenido principal |
| Fondo cards | --bg-card | `#151525` | Cards de programa |
| Card hover | --bg-card-hover | `#1e1e38` | Hover en cards |
| Fondo inputs | --bg-input | `#0f0f1f` | Campos codigo referido |
| Primario gradiente | --primary-gradient | `from-pink-500 to-purple-600` | Botones primarios CTA |
| Texto principal | --text-primary | `#ffffff` | Titulos |
| Texto secundario | --text-secondary | `#94a3b8` | Subtitulos, artista |
| Texto muted | --text-muted | `#64748b` | Labels, fechas, iconos |
| Borde default | --border-default | `#334155` | Bordes de cards e inputs |
| Estado Pendiente bg | --status-pendiente-bg | `rgba(245,158,11,0.1)` / `amber-950/50` | Badge Pendiente |
| Estado Pendiente text | --status-pendiente-text | `#f59e0b` / `amber-400` | Texto badge Pendiente |
| Estado Aprobado bg | --status-aprobado-bg | `rgba(16,185,129,0.1)` / `green-950/50` | Badge Aprobado |
| Estado Aprobado text | --status-aprobado-text | `#10b981` / `green-400` | Texto badge Aprobado |
| Estado Bloqueado bg | --status-bloqueado-bg | `rgba(239,68,68,0.1)` / `red-950/50` | Badge Bloqueado |
| Estado Bloqueado text | --status-bloqueado-text | `#ef4444` / `red-400` | Texto badge Bloqueado |
| Codigo referido highlight | --copy-highlight-border | `rgba(168,85,247,0.3)` | Borde bloque codigo |

---

## 3. Componentes por Pantalla

### 3.1 Pantalla: Explorar Programas (/crowdpromotion/explorar)

#### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (h-16, bg-[#0d0d1a], border-b border-[#334155])      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  HERO SECTION (py-10, bg-[#0d0d1a], border-b border-[#334155])      │
│  max-w-5xl mx-auto px-4                                              │
│  "Programas de promocion disponibles"  (h1)                          │
│  "Encuentra programas y empieza a ganar comisiones"  (p)             │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CONTENIDO (max-w-5xl mx-auto px-4, bg-[#1a1a2e])                   │
│                                                                     │
│  [BANNER SIN PERFIL - condicional]                                   │
│                                                                     │
│  FILTROS (flex flex-wrap gap-3 mt-6 mb-4)                            │
│  [Buscar artista...]  [Tipo de programa v]  [Rango comision v]       │
│  [x Limpiar filtros - condicional]                                   │
│                                                                     │
│  "Mostrando X de Y programas"  (p, text-sm text-[#64748b])           │
│                                                                     │
│  GRID (grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5)         │
│  ┌───────────────────┐ ┌───────────────────┐ ┌───────────────────┐  │
│  │ ProgramaCard      │ │ ProgramaCard      │ │ ProgramaCard      │  │
│  └───────────────────┘ └───────────────────┘ └───────────────────┘  │
│                                                                     │
│  PAGINACION (flex justify-center mt-8 pb-10)                         │
│  [< Anterior]  [1] [2] [3]  [Siguiente >]                            │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

#### 3.1.1 Hero Section

| Elemento | Tag/Componente | Clases Tailwind |
|----------|---------------|-----------------|
| Seccion contenedor | `<section>` | `py-10 bg-[#0d0d1a] border-b border-[#334155]` |
| Inner wrapper | `<div>` | `max-w-5xl mx-auto px-4` |
| Titulo principal | `<h1>` | `text-2xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |

#### 3.1.2 BannerSinPerfilPromotor (componente custom, condicional)

Se muestra debajo de los filtros si el usuario no tiene perfil de promotor registrado. Solo visible para usuarios autenticados sin `Promotor`.

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Contenedor banner | `<Alert>` o `<div>` | `bg-amber-950/30 border border-amber-800/50 rounded-lg p-4 flex items-center justify-between gap-4 mb-4` |
| Icono advertencia | `<AlertCircle w-5 h-5>` | `text-amber-400 shrink-0` |
| Texto informativo | `<p>` | `text-sm text-amber-300` |
| Boton crear perfil | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white text-xs shrink-0` |

Composicion:
```tsx
<div className="bg-amber-950/30 border border-amber-800/50 rounded-lg p-4 flex items-center justify-between gap-4 mb-4"
     role="alert"
     aria-live="polite">
    <div className="flex items-center gap-3">
        <AlertCircle className="w-5 h-5 text-amber-400 shrink-0" aria-hidden="true" />
        <p className="text-sm text-amber-300">
            Necesitas un perfil de promotor para solicitar inscripciones.
        </p>
    </div>
    <Button size="sm"
            className="bg-gradient-to-r from-pink-500 to-purple-600 ..."
            onClick={() => navigate('/promotor/registro')}>
        Crear perfil
    </Button>
</div>
```

#### 3.1.3 FiltroProgramas (componente custom)

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Contenedor | `<div>` | `flex flex-wrap items-center gap-3 mb-6` |
| Wrapper icono busqueda | `<div>` | `relative flex-1 min-w-[200px] max-w-xs` |
| Icono Search | `<Search w-4 h-4>` | `absolute left-3 top-3 text-[#64748b] pointer-events-none` |
| Input artista | `<Input>` | `pl-9 bg-[#151525] border-[#334155] text-white h-10 placeholder:text-[#64748b] focus-visible:ring-[#a855f7] focus:border-[#a855f7]` |
| Select tipo programa | `<Select>` + `<SelectTrigger>` + `<SelectContent>` | Trigger: `w-[180px] bg-[#151525] border-[#334155] text-white h-10` |
| Select rango comision | `<Select>` + `<SelectTrigger>` + `<SelectContent>` | Trigger: `w-[180px] bg-[#151525] border-[#334155] text-white h-10` |
| Boton limpiar (condicional) | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-white h-10 px-3 text-sm` |

Opciones Select Tipo de Programa:
- `SelectItem value=""` - "Todos los tipos"
- `SelectItem value="1"` - "Referral"
- `SelectItem value="2"` - "Afiliado"
- `SelectItem value="3"` - "Influencer"
- `SelectItem value="4"` - "Mixto"

Opciones Select Rango Comision (filtro local):
- `SelectItem value=""` - "Cualquier comision"
- `SelectItem value="0-5"` - "0-5%"
- `SelectItem value="5-10"` - "5-10%"
- `SelectItem value="10+"` - "Mas de 10%"
- `SelectItem value="fija"` - "Comision fija"

Composicion:
```tsx
<div className="flex flex-wrap items-center gap-3 mb-6" role="search" aria-label="Filtros de programas">
    <div className="relative flex-1 min-w-[200px] max-w-xs">
        <Search className="absolute left-3 top-3 w-4 h-4 text-[#64748b] pointer-events-none" aria-hidden="true" />
        <Input
            type="search"
            placeholder="Buscar artista..."
            value={artistaFiltro}
            onChange={handleArtistaChange}
            className="pl-9 bg-[#151525] border-[#334155] text-white h-10 placeholder:text-[#64748b]"
            aria-label="Buscar por nombre de artista"
        />
    </div>
    <Select value={tipoPromoId} onValueChange={handleTipoChange}>
        <SelectTrigger className="w-[180px] bg-[#151525] border-[#334155] text-white h-10" aria-label="Filtrar por tipo de programa">
            <SelectValue placeholder="Tipo de programa" />
        </SelectTrigger>
        <SelectContent className="bg-[#151525] border-[#334155]">
            <SelectItem value="">Todos los tipos</SelectItem>
            {/* ... items dinamicos desde API */}
        </SelectContent>
    </Select>
    <Select value={rangoComision} onValueChange={handleRangoChange}>
        <SelectTrigger className="w-[180px] bg-[#151525] border-[#334155] text-white h-10" aria-label="Filtrar por rango de comision">
            <SelectValue placeholder="Rango comision" />
        </SelectTrigger>
        <SelectContent className="bg-[#151525] border-[#334155]">
            <SelectItem value="">Cualquier comision</SelectItem>
            {/* ... */}
        </SelectContent>
    </Select>
    {hasActiveFilters && (
        <Button variant="ghost" size="sm"
                className="text-[#64748b] hover:text-white h-10 px-3 text-sm"
                onClick={onClearFilters}
                aria-label="Limpiar todos los filtros">
            <X className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
            Limpiar
        </Button>
    )}
</div>
```

#### 3.1.4 EstadoInscripcionBadge (componente custom reutilizable)

Badge que representa el estado personal de inscripcion del promotor en un programa. Patron analogo a `EstadoPropuestaBadge` existente.

| Estado (`miEstado`) | Clases Tailwind Badge | Icono | Texto |
|--------------------|----------------------|-------|-------|
| `null` (no inscrito) | No aplica badge en header | - | - |
| `"Pendiente"` | `bg-amber-950/50 text-amber-400 border border-amber-800/50 text-xs px-2.5 py-1` | `<Clock w-3 h-3>` | "PENDIENTE" |
| `"Aprobado"` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs px-2.5 py-1` | `<CheckCircle w-3 h-3>` | "APROBADO" |
| `"Bloqueado"` | `bg-red-950/50 text-red-400 border border-red-800/50 text-xs px-2.5 py-1` | `<Ban w-3 h-3>` | "BLOQUEADO" |

Composicion:
```tsx
<Badge
    className={cn("border text-xs font-medium inline-flex items-center gap-1", estadoStyles)}
    aria-label={`Estado de inscripcion: ${estadoTexto}`}
    role="status"
>
    <IconoEstado className="w-3 h-3" aria-hidden="true" />
    {estadoTexto}
</Badge>
```

#### 3.1.5 ProgramaActionSlot (composicion interna de ProgramaCard)

Zona de accion condicional en la parte inferior de la card, dependiente de `miEstado`.

**Variante `miEstado === null` (no inscrito):**

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Boton solicitar | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10` |
| Icono | `<UserPlus w-4 h-4>` | `mr-2` |
| Estado loading | `<Loader2 w-4 h-4 animate-spin>` | `mr-2` |
| Texto loading | `"Enviando..."` | (boton disabled durante peticion) |

**Variante `miEstado === "Pendiente"`:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `flex flex-col gap-1` |
| Badge pendiente | `<Badge>` | `bg-amber-950/50 text-amber-400 border border-amber-800/50 text-xs self-start px-3 py-1` |
| Texto explicativo | `<p>` | `text-xs text-[#64748b]` |

**Variante `miEstado === "Aprobado"`:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `flex flex-col gap-1.5` |
| Badge aprobado | `<Badge>` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs self-start px-3 py-1` con `<CheckCircle w-3 h-3 mr-1>` |
| Link a mis programas | `<Button variant="ghost" size="sm">` | `text-[#a855f7] hover:text-purple-400 p-0 h-auto text-xs` |

**Variante `miEstado === "Bloqueado"`:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Mensaje bloqueado | `<p>` | `text-xs text-red-400 flex items-center gap-1.5` |
| Icono | `<Ban w-3.5 h-3.5>` | `shrink-0` |

#### 3.1.6 ProgramaCard (componente custom principal)

Reutilizable para el grid de explorar. Dos variantes visuales: normal y bloqueado (opacidad 60%).

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Card base (no inscrito / aprobado / pendiente) | `<Card>` | `bg-[#151525] border-[#334155] hover:border-[#a855f7]/50 transition-colors p-5 flex flex-col gap-3` |
| Card base (bloqueado) | `<Card>` | `bg-[#151525] border-[#334155] p-5 flex flex-col gap-3 opacity-60 cursor-not-allowed` |
| Fila badges superior | `<div>` | `flex items-center gap-2` |
| Badge tipo programa | `<Badge variant="outline">` | `bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium` |
| Badge num tareas | `<Badge variant="outline">` | `bg-[#1e1e38] text-[#64748b] border-[#334155] text-xs` |
| Icono clipboard | `<ClipboardList w-3 h-3>` | `mr-1` |
| Titulo programa | `<h3>` | `text-base font-semibold text-white leading-tight` |
| Artista | `<p>` | `text-sm text-[#94a3b8]` |
| Icono artista | `<Music w-3.5 h-3.5>` | `mr-1.5 inline text-[#64748b]` |
| Separador | `<Separator>` | `bg-[#334155]` |
| Fila metrica (label) | `<span>` | `text-xs text-[#64748b] w-20 shrink-0` |
| Valor comision | `<span>` | `text-sm font-semibold text-[#a855f7]` |
| Valor campana | `<span>` | `text-sm text-white` |
| Valor vigencia | `<span>` | `text-xs text-[#64748b]` |
| Icono calendario | `<Calendar w-3 h-3>` | `mr-1 inline` |
| Zona accion | `<ProgramaActionSlot>` | (ver 3.1.5) |

Composicion completa:
```tsx
<Card
    className={cn(
        "bg-[#151525] border-[#334155] p-5 flex flex-col gap-3 transition-colors",
        esBloqueado
            ? "opacity-60 cursor-not-allowed"
            : "hover:border-[#a855f7]/50"
    )}
    role="article"
    aria-label={`Programa: ${titulo}. Estado: ${miEstado ?? 'Disponible'}`}
    aria-disabled={esBloqueado}
>
    {/* Fila badges */}
    <div className="flex items-center gap-2">
        <Badge variant="outline" className="bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium">
            {tipoPromoNombre}
        </Badge>
        <Badge variant="outline" className="bg-[#1e1e38] text-[#64748b] border-[#334155] text-xs">
            <ClipboardList className="w-3 h-3 mr-1" aria-hidden="true" />
            {numeroTareas} {numeroTareas === 1 ? 'tarea' : 'tareas'}
        </Badge>
    </div>

    {/* Titulo y artista */}
    <div>
        <h3 className="text-base font-semibold text-white leading-tight">{titulo}</h3>
        <p className="text-sm text-[#94a3b8] mt-0.5">
            <Music className="w-3.5 h-3.5 mr-1.5 inline text-[#64748b]" aria-hidden="true" />
            {artistaNombre}
        </p>
    </div>

    <Separator className="bg-[#334155]" />

    {/* Metricas */}
    <div className="space-y-1.5" aria-label="Detalles del programa">
        <div className="flex items-start gap-2">
            <span className="text-xs text-[#64748b] w-20 shrink-0">Comision:</span>
            <span className="text-sm font-semibold text-[#a855f7]">{comisionTexto}</span>
        </div>
        {campaniaTitulo && (
            <div className="flex items-start gap-2">
                <span className="text-xs text-[#64748b] w-20 shrink-0">Campana:</span>
                <span className="text-sm text-white">{campaniaTitulo}</span>
            </div>
        )}
        <div className="flex items-start gap-2">
            <span className="text-xs text-[#64748b] w-20 shrink-0">Vigencia:</span>
            <span className="text-xs text-[#64748b]">
                <Calendar className="w-3 h-3 mr-1 inline" aria-hidden="true" />
                {formatFechaVigencia(fechaInicio, fechaFin)}
            </span>
        </div>
    </div>

    <Separator className="bg-[#334155]" />

    {/* Zona accion */}
    <ProgramaActionSlot
        miEstado={miEstado}
        onSolicitar={handleSolicitar}
        isSolicitando={isSolicitando}
        programaId={id}
    />
</Card>
```

#### 3.1.7 ProgramaCardSkeleton

Skeleton para el estado de carga inicial del grid. 6 unidades.

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Card skeleton | `<Skeleton>` | `bg-[#1e1e38] rounded-xl h-[240px] animate-pulse` |

Composicion:
```tsx
<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5" aria-busy="true" aria-label="Cargando programas">
    {Array.from({ length: 6 }).map((_, i) => (
        <Skeleton key={i} className="bg-[#1e1e38] rounded-xl h-[240px]" />
    ))}
</div>
```

#### 3.1.8 EmptyStateProgramas (componente custom)

Dos variantes: sin programas disponibles, y sin resultados con filtro activo.

**Variante sin programas disponibles:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `text-center py-16` |
| Icono contenedor | `<div>` | `mx-auto w-20 h-20 rounded-full bg-gradient-to-br from-pink-500/20 to-purple-600/20 flex items-center justify-center mb-6` |
| Icono | `<Megaphone w-10 h-10>` | `text-[#a855f7]` |
| Titulo | `<h3>` | `text-lg font-semibold text-white mb-2` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8]` |

**Variante sin resultados con filtro activo:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `text-center py-12` |
| Icono | `<SearchX w-10 h-10>` | `text-[#64748b] mx-auto mb-4` |
| Texto | `<p>` | `text-sm text-[#94a3b8] mb-4` |
| Boton limpiar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |

**Variante error de carga:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `text-center py-12` |
| Icono | `<AlertCircle w-8 h-8>` | `text-red-400 mx-auto mb-3` |
| Texto | `<p>` | `text-sm text-[#94a3b8] mb-4` |
| Boton reintentar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |

Composicion:
```tsx
<div role="status" className="text-center py-16" aria-label={ariaLabel}>
    {variant === 'error' && (
        <>
            <AlertCircle className="w-8 h-8 text-red-400 mx-auto mb-3" aria-hidden="true" />
            <p className="text-sm text-[#94a3b8] mb-4">No se pudieron cargar los programas</p>
            <Button variant="outline" size="sm" onClick={onRetry} className="border-[#334155] ...">
                Reintentar
            </Button>
        </>
    )}
    {variant === 'empty' && (
        <>
            <div className="mx-auto w-20 h-20 rounded-full bg-gradient-to-br from-pink-500/20 to-purple-600/20 flex items-center justify-center mb-6">
                <Megaphone className="w-10 h-10 text-[#a855f7]" aria-hidden="true" />
            </div>
            <h3 className="text-lg font-semibold text-white mb-2">No hay programas disponibles</h3>
            <p className="text-sm text-[#94a3b8]">Vuelve mas tarde para encontrar nuevas oportunidades</p>
        </>
    )}
    {variant === 'no-results' && (
        <>
            <SearchX className="w-10 h-10 text-[#64748b] mx-auto mb-4" aria-hidden="true" />
            <p className="text-sm text-[#94a3b8] mb-4">No se encontraron programas con los filtros seleccionados</p>
            <Button variant="outline" size="sm" onClick={onClearFilters} className="border-[#334155] ...">
                Limpiar filtros
            </Button>
        </>
    )}
</div>
```

#### 3.1.9 Paginacion

Uso del componente `Pagination` existente en el proyecto.

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Contador resultados | `<p>` | `text-sm text-[#64748b] mb-4` |
| Nav paginacion | `<Pagination>` | `mt-8 pb-10` |
| Contenido | `<PaginationContent>` | (default) |
| Item | `<PaginationItem>` | (default) |
| Link pagina | `<PaginationLink>` | activo: `bg-[#1e1e38] text-white border-[#a855f7]` / inactivo: `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] border-[#334155]` |
| Anterior | `<PaginationPrevious>` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Siguiente | `<PaginationNext>` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |

Composicion:
```tsx
<>
    <p className="text-sm text-[#64748b] mb-4" aria-live="polite">
        Mostrando {Math.min(page * pageSize, totalCount)} de {totalCount} programas
    </p>
    <Pagination>
        <PaginationContent>
            <PaginationItem>
                <PaginationPrevious
                    href="#"
                    onClick={(e) => { e.preventDefault(); onPageChange(page - 1) }}
                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                    aria-disabled={page === 1}
                />
            </PaginationItem>
            {/* numeros de pagina */}
            <PaginationItem>
                <PaginationNext
                    href="#"
                    onClick={(e) => { e.preventDefault(); onPageChange(page + 1) }}
                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                    aria-disabled={page === totalPages}
                />
            </PaginationItem>
        </PaginationContent>
    </Pagination>
</>
```

---

### 3.2 Pantalla: Mis Programas (/promotor/mis-programas)

#### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (igual que Explorar Programas)                        │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CONTENIDO (max-w-4xl mx-auto px-4 py-8, bg-[#1a1a2e])              │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ Mis programas              [Explorar programas ->]           │   │
│  │ Gestiona tus inscripciones                                   │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  FILTRO ESTADO:                                                      │
│  [Todos] [Aprobado] [Pendiente] [Bloqueado]                          │
│                                                                     │
│  LISTA DE INSCRIPCIONES:                                             │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ MiProgramaItem (aprobado - expandido)                        │   │
│  │  titulo / artista / badge APROBADO                           │   │
│  │  ---- codigo referido ----                                   │   │
│  │  ---- url tracking ----                                      │   │
│  │  comision / fecha                                            │   │
│  │  [Ver tareas y estadisticas v] (toggle)                      │   │
│  │  ---- tareas + KPIs (expandido) ----                         │   │
│  └──────────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ MiProgramaItem (pendiente)                                   │   │
│  │  titulo / artista / badge PENDIENTE                          │   │
│  │  [Clock] Esperando aprobacion...                             │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

#### 3.2.1 Cabecera de Pagina

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Wrapper cabecera | `<div>` | `flex items-start justify-between mb-6` |
| Bloque titulo | `<div>` | (flex-col) |
| Titulo | `<h1>` | `text-2xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Boton explorar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-4 text-sm` |
| Icono compass | `<Compass w-4 h-4>` | `mr-2` |

#### 3.2.2 Filtros de Estado (chips de boton)

Los chips son `<Button>` que filtran localmente la lista cargada. No disparan nueva llamada API.

| Estado | Boton activo | Boton inactivo |
|--------|-------------|----------------|
| Todos | `bg-[#1e1e38] text-white border border-[#a855f7] h-8 px-4 text-sm rounded-full` | `variant="ghost"` + `text-[#64748b] hover:text-white hover:bg-[#1e1e38] h-8 px-4 text-sm rounded-full` |
| Aprobado | `bg-green-950/50 text-green-400 border border-green-800/50 h-8 px-4 text-sm rounded-full` | `variant="ghost"` + `text-[#64748b] hover:text-green-400/70 hover:bg-green-950/30 h-8 px-4 text-sm rounded-full` |
| Pendiente | `bg-amber-950/50 text-amber-400 border border-amber-800/50 h-8 px-4 text-sm rounded-full` | `variant="ghost"` + `text-[#64748b] hover:text-amber-400/70 hover:bg-amber-950/30 h-8 px-4 text-sm rounded-full` |
| Bloqueado | `bg-red-950/50 text-red-400 border border-red-800/50 h-8 px-4 text-sm rounded-full` | `variant="ghost"` + `text-[#64748b] hover:text-red-400/70 hover:bg-red-950/30 h-8 px-4 text-sm rounded-full` |

Composicion:
```tsx
<div className="flex items-center gap-2 mb-5" role="group" aria-label="Filtrar inscripciones por estado">
    {ESTADO_CHIPS.map((chip) => (
        <Button
            key={chip.value}
            size="sm"
            onClick={() => setEstadoFiltro(chip.value)}
            className={cn(
                "h-8 px-4 text-sm rounded-full",
                estadoActivo === chip.value ? chip.activeClasses : chip.inactiveClasses
            )}
            aria-pressed={estadoActivo === chip.value}
        >
            {chip.label}
        </Button>
    ))}
</div>
```

#### 3.2.3 CodigoReferidoBlock (componente custom)

Bloque de codigo referido con boton copiar. Solo visible cuando `estado === "Aprobado"`.

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Label | `<p>` | `text-xs font-medium text-[#94a3b8] mb-1.5` |
| Contenedor codigo + boton | `<div>` | `flex items-center gap-2` |
| Campo codigo (display) | `<div>` | `flex-1 flex items-center bg-[#0f0f1f] border border-[#a855f7]/30 rounded-lg px-3 h-10 font-mono text-sm text-[#a855f7] overflow-hidden` |
| Boton copiar | `<Button variant="ghost" size="icon">` | `h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0` |
| Icono Copy | `<Copy w-4 h-4>` | (estado normal) |
| Icono Check | `<Check w-4 h-4>` | `text-green-400` (1.5s tras copiar, luego vuelve a Copy) |

Composicion:
```tsx
<div>
    <p className="text-xs font-medium text-[#94a3b8] mb-1.5" id="codigo-label">
        Codigo referido:
    </p>
    <div className="flex items-center gap-2">
        <div
            className="flex-1 flex items-center bg-[#0f0f1f] border border-[#a855f7]/30 rounded-lg px-3 h-10 font-mono text-sm text-[#a855f7] overflow-hidden"
            aria-labelledby="codigo-label"
            role="textbox"
            aria-readonly="true"
        >
            <span className="truncate">{codigoReferido}</span>
        </div>
        <Button
            variant="ghost"
            size="icon"
            className="h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0"
            onClick={handleCopiarCodigo}
            aria-label={copiado ? "Codigo copiado" : "Copiar codigo referido"}
        >
            {copiado
                ? <Check className="w-4 h-4 text-green-400" aria-hidden="true" />
                : <Copy className="w-4 h-4" aria-hidden="true" />
            }
        </Button>
    </div>
</div>
```

#### 3.2.4 UrlTrackingBlock (componente custom)

Bloque URL de tracking con tooltip (URL truncada, completa en hover) y boton copiar.

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Label | `<p>` | `text-xs font-medium text-[#94a3b8] mb-1.5` |
| Contenedor URL + boton | `<div>` | `flex items-center gap-2` |
| Campo URL (display) | `<div>` | `flex-1 flex items-center bg-[#0f0f1f] border border-[#334155] rounded-lg px-3 h-10 text-sm overflow-hidden` |
| Texto URL truncado (trigger tooltip) | `<TooltipTrigger>` envolviendo `<span>` | `truncate text-sm text-[#94a3b8] cursor-default` |
| Tooltip contenido | `<TooltipContent>` | `bg-[#0d0d1a] border border-[#334155] text-white text-xs max-w-[400px] break-all` |
| Boton copiar | `<Button variant="ghost" size="icon">` | Igual que CodigoReferidoBlock |

Composicion:
```tsx
<div>
    <p className="text-xs font-medium text-[#94a3b8] mb-1.5" id="url-label">
        URL de tracking:
    </p>
    <div className="flex items-center gap-2">
        <div className="flex-1 flex items-center bg-[#0f0f1f] border border-[#334155] rounded-lg px-3 h-10 text-sm overflow-hidden">
            <TooltipProvider>
                <Tooltip>
                    <TooltipTrigger asChild>
                        <span
                            className="truncate text-sm text-[#94a3b8] cursor-default"
                            aria-label={`URL de tracking: ${urlTrackingPersonalizada}`}
                        >
                            {urlTrackingPersonalizada}
                        </span>
                    </TooltipTrigger>
                    <TooltipContent
                        className="bg-[#0d0d1a] border border-[#334155] text-white text-xs max-w-[400px] break-all"
                        side="top"
                    >
                        {urlTrackingPersonalizada}
                    </TooltipContent>
                </Tooltip>
            </TooltipProvider>
        </div>
        <Button
            variant="ghost"
            size="icon"
            className="h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0"
            onClick={handleCopiarUrl}
            aria-label={copiadoUrl ? "URL copiada" : "Copiar URL de tracking"}
        >
            {copiadoUrl
                ? <Check className="w-4 h-4 text-green-400" aria-hidden="true" />
                : <Copy className="w-4 h-4" aria-hidden="true" />
            }
        </Button>
    </div>
</div>
```

#### 3.2.5 KpiStatCard (componente custom - seccion expandida)

Mini cards de estadisticas MVP. Valores siempre en 0 para el MVP.

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Grid KPI | `<div>` | `grid grid-cols-3 gap-3 my-4` |
| Card KPI | `<div>` | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-center` |
| Valor | `<p>` | `text-xl font-bold text-white` |
| Label | `<p>` | `text-xs text-[#64748b] mt-1` |
| Texto MVP | `<p>` | `text-xs text-[#64748b] italic text-center mt-2 col-span-3` |

Composicion:
```tsx
<div className="grid grid-cols-3 gap-3 my-4" aria-label="Estadisticas de rendimiento">
    {[
        { valor: '0', label: 'Clics' },
        { valor: '0', label: 'Conversiones' },
        { valor: '0.00 EUR', label: 'Comision ganada' },
    ].map((kpi) => (
        <div key={kpi.label}
             className="bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-center">
            <p className="text-xl font-bold text-white">{kpi.valor}</p>
            <p className="text-xs text-[#64748b] mt-1">{kpi.label}</p>
        </div>
    ))}
    <p className="text-xs text-[#64748b] italic text-center mt-2 col-span-3">
        Estadisticas en tiempo real (proximamente)
    </p>
</div>
```

#### 3.2.6 TareaItem (componente custom - seccion expandida)

Lista de tareas del programa, visible en la seccion expandida de inscripciones aprobadas.

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Titulo seccion | `<h4>` | `text-sm font-semibold text-[#cbd5e1] mb-3 flex items-center gap-2` |
| Icono ClipboardList | `<ClipboardList w-4 h-4>` | `text-[#64748b]` |
| Contenedor lista | `<div>` | `space-y-2` |
| Card tarea | `<div>` | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3` |
| Cabecera tarea | `<div>` | `flex items-start gap-2` |
| Badge tipo evento | `<Badge variant="outline">` | `bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium shrink-0` |
| Nombre tarea | `<p>` | `text-sm text-white font-medium` |
| Detalles tarea | `<p>` | `text-xs text-[#64748b] mt-1` |
| Sin tareas | `<p>` | `text-sm text-[#64748b] italic py-2` |

#### 3.2.7 ProgramaDetallExpandido (Collapsible - solo aprobados)

Seccion colapsable de detalle con tareas y estadisticas, visible solo en inscripciones `Aprobado`.

| Elemento | Componente shadcn | Clases Tailwind |
|----------|------------------|-----------------|
| Raiz collapsible | `<Collapsible>` | (estado open controlado) |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Trigger boton toggle | `<CollapsibleTrigger asChild>` envolviendo `<Button variant="ghost" size="sm">` | `w-full text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 text-xs gap-1.5` |
| Icono expandir | `<ChevronDown w-3.5 h-3.5>` | (visible si colapsado) |
| Icono colapsar | `<ChevronUp w-3.5 h-3.5>` | (visible si expandido) |
| Contenido expandido | `<CollapsibleContent>` | `data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0` |

Composicion:
```tsx
<Collapsible open={isOpen} onOpenChange={setIsOpen}>
    <Separator className="bg-[#334155] my-4" />
    <CollapsibleTrigger asChild>
        <Button
            variant="ghost"
            size="sm"
            className="w-full text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 text-xs gap-1.5"
            aria-expanded={isOpen}
            aria-controls={`detalle-${programaId}`}
        >
            {isOpen
                ? <><ChevronUp className="w-3.5 h-3.5" aria-hidden="true" /> Ocultar detalles</>
                : <><ChevronDown className="w-3.5 h-3.5" aria-hidden="true" /> Ver tareas y estadisticas</>
            }
        </Button>
    </CollapsibleTrigger>
    <CollapsibleContent id={`detalle-${programaId}`}>
        <Separator className="bg-[#334155] my-4" />
        <KpiStatCard />
        <Separator className="bg-[#334155] my-4" />
        <h4 className="text-sm font-semibold text-[#cbd5e1] mb-3 flex items-center gap-2">
            <ClipboardList className="w-4 h-4 text-[#64748b]" aria-hidden="true" />
            Tareas del programa ({tareas.length})
        </h4>
        <div className="space-y-2">
            {tareas.length === 0
                ? <p className="text-sm text-[#64748b] italic py-2">Este programa no tiene tareas definidas</p>
                : tareas.map((tarea) => <TareaItem key={tarea.id} tarea={tarea} />)
            }
        </div>
    </CollapsibleContent>
</Collapsible>
```

#### 3.2.8 MiProgramaItem (componente card principal)

Card de inscripcion en la lista de Mis Programas. Variantes por estado.

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor card | `<Card>` | `bg-[#151525] border-[#334155] p-5 mb-4` |
| Cabecera | `<div>` | `flex items-start justify-between` |
| Titulo programa | `<h3>` | `text-base font-semibold text-white` |
| Artista | `<p>` | `text-sm text-[#94a3b8] mt-0.5` |
| Badge estado | `<EstadoInscripcionBadge>` | (ver 3.1.4) |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Comision info | `<p>` | `text-sm text-[#64748b]` |
| Fecha inscripcion | `<p>` | `text-xs text-[#64748b] mt-3` |
| Icono calendar | `<Calendar w-3 h-3>` | `mr-1 inline` |

**Bloque especifico para Aprobado:** `<CodigoReferidoBlock>` + `<UrlTrackingBlock>` + `<ProgramaDetallExpandido>`

**Bloque especifico para Pendiente:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `flex items-center gap-3 py-2` |
| Icono Clock | `<Clock w-5 h-5>` | `text-amber-400 shrink-0` |
| Texto principal | `<p>` | `text-sm text-amber-300` |
| Texto secundario | `<p>` | `text-xs text-[#64748b] mt-0.5` |

**Bloque especifico para Bloqueado:**

| Elemento | Componente | Clases Tailwind |
|----------|-----------|-----------------|
| Contenedor | `<div>` | `flex items-center gap-3 py-2` |
| Icono Ban | `<Ban w-5 h-5>` | `text-red-400 shrink-0` |
| Texto | `<p>` | `text-sm text-red-300` |

Composicion:
```tsx
<Card
    className="bg-[#151525] border-[#334155] p-5 mb-4"
    id={`programa-${programaId}`}
    role="article"
    aria-label={`Inscripcion en ${programaTitulo}: estado ${estado}`}
>
    <div className="flex items-start justify-between">
        <div>
            <h3 className="text-base font-semibold text-white">{programaTitulo}</h3>
            <p className="text-sm text-[#94a3b8] mt-0.5">
                <Music className="w-3.5 h-3.5 mr-1.5 inline text-[#64748b]" aria-hidden="true" />
                {artistaNombre}
            </p>
        </div>
        <EstadoInscripcionBadge estado={estado} />
    </div>

    <Separator className="bg-[#334155] my-4" />

    {estado === 'Aprobado' && (
        <>
            <CodigoReferidoBlock codigoReferido={codigoReferido} />
            <div className="mt-3">
                <UrlTrackingBlock urlTrackingPersonalizada={urlTrackingPersonalizada} />
            </div>
        </>
    )}

    {estado === 'Pendiente' && (
        <div className="flex items-center gap-3 py-2">
            <Clock className="w-5 h-5 text-amber-400 shrink-0" aria-hidden="true" />
            <div>
                <p className="text-sm text-amber-300">Esperando aprobacion del artista</p>
                <p className="text-xs text-[#64748b] mt-0.5">Recibiras tu codigo referido cuando seas aprobado</p>
            </div>
        </div>
    )}

    {estado === 'Bloqueado' && (
        <div className="flex items-center gap-3 py-2">
            <Ban className="w-5 h-5 text-red-400 shrink-0" aria-hidden="true" />
            <p className="text-sm text-red-300">Tu acceso a este programa ha sido bloqueado</p>
        </div>
    )}

    <p className="text-xs text-[#64748b] mt-3">
        <Calendar className="w-3 h-3 mr-1 inline" aria-hidden="true" />
        Inscrito: {formatFecha(fechaAlta)}
    </p>

    {estado === 'Aprobado' && (
        <ProgramaDetallExpandido
            programaId={programaId}
            tareas={tareas}
        />
    )}
</Card>
```

---

## 4. Estados de UI por Pantalla

### 4.1 Explorar Programas - Estados

| Estado | Comportamiento Visual |
|--------|----------------------|
| Loading inicial | Grid de 6 `<ProgramaCardSkeleton>` con `animate-pulse`. `aria-busy="true"` en el grid |
| Default con programas | Grid de cards reales + paginacion si `totalCount > pageSize` |
| Solicitando inscripcion | Boton "Solicitar inscripcion" de la card especifica: `<Loader2 animate-spin>` + "Enviando..." + `disabled`. Otros botones no afectados |
| Empty sin programas | `<EmptyStateProgramas variant="empty">` centrado en el area del grid |
| Sin resultados filtro | `<EmptyStateProgramas variant="no-results">` con boton limpiar |
| Error de carga | `<EmptyStateProgramas variant="error">` con boton reintentar |
| Sin perfil de promotor | `<BannerSinPerfilPromotor>` sobre los filtros, fondo amber oscuro |

### 4.2 Mis Programas - Estados

| Estado | Comportamiento Visual |
|--------|----------------------|
| Loading | 3 `<Skeleton>` de `h-[160px] bg-[#1e1e38] rounded-xl animate-pulse mb-4` |
| Default | Lista de `<MiProgramaItem>` ordenados por `fechaAlta` descendente |
| Empty (sin inscripciones) | Icono Megaphone con gradiente, titulo, boton "Explorar programas" gradient que navega a `/crowdpromotion/explorar` |
| Filtro sin resultados | `<p>` centrado "No tienes inscripciones con este estado" + boton "Ver todas" |
| Error de carga | Card de error con `<AlertCircle>` + boton "Reintentar" |
| Copiado | Icono `<Copy>` cambia a `<Check className="text-green-400">` durante 1.5s. Estado local por campo (codigo y URL independientes) |

---

## 5. Feedback: Toasts

Usar `sonner` (toast) existente en el proyecto. Todos los toasts se muestran en posicion bottom-right.

### Pantalla Explorar Programas

| Evento | Tipo toast | Mensaje |
|--------|-----------|---------|
| Inscripcion exitosa | Success (verde) | "Solicitud enviada. El artista revisara tu perfil." |
| Ya inscrito (400/4021) | Destructive (rojo) | "Ya tienes una solicitud activa para este programa." |
| Bloqueado (403/4022) | Destructive (rojo) | "No puedes inscribirte en este programa." |
| Promotor inactivo (400/4023) | Destructive (rojo) | "Tu perfil de promotor esta inactivo." |
| Error inesperado (500) | Destructive (rojo) | "Ocurrio un error. Intentalo de nuevo." |

### Pantalla Mis Programas

| Evento | Tipo toast | Mensaje |
|--------|-----------|---------|
| Codigo copiado | Default (fondo oscuro) | "Codigo referido copiado al portapapeles" |
| URL copiada | Default | "URL de tracking copiada al portapapeles" |
| Error al copiar | Destructive | "No se pudo copiar. Selecciona el texto manualmente." |

---

## 6. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Labels de region | `aria-label` en `<section>`, `role="search"` en filtros, `role="navigation"` en paginacion |
| Cards interactivas | `role="article"` + `aria-label` con titulo y estado en cada `<ProgramaCard>` y `<MiProgramaItem>` |
| Estado inhabilitado | `aria-disabled="true"` en cards de programas bloqueados, `disabled` en botones durante loading |
| Estado de boton toggle | `aria-pressed` en chips de filtro de estado (Mis Programas) |
| Collapsible | `aria-expanded` en el trigger, `aria-controls` apuntando al `id` del contenido |
| Badges de estado | `role="status"` + `aria-label="Estado de inscripcion: X"` en `<EstadoInscripcionBadge>` |
| Iconos decorativos | `aria-hidden="true"` en todos los iconos de Lucide que son decorativos |
| Botones de copiar | `aria-label` dinamico: "Copiar codigo referido" / "Codigo copiado" segun estado |
| Tooltip URL | Accesible via teclado con `<TooltipProvider>` de Radix UI |
| Loading states | `aria-busy="true"` en el grid durante carga inicial |
| Contador de resultados | `aria-live="polite"` en el parrafo de conteo de programas |
| Paginacion | `aria-current="page"` en el link de pagina activa (via `PaginationLink isActive`) |
| Codigo referido | `role="textbox" aria-readonly="true"` en el div de display del codigo |
| Contraste colores | Todos los colores de texto sobre fondos oscuros cumplen WCAG AA (ratio >= 4.5:1) |
| Focus visible | Hereda del sistema de Tailwind: `focus-visible:ring-1 focus-visible:ring-ring` en buttons e inputs |
| Navegacion teclado | Todos los elementos interactivos accesibles via Tab. Cards de programa clickeables via Enter |

---

## 7. Responsive Design

| Breakpoint | Pantalla Explorar | Pantalla Mis Programas |
|------------|-----------------|----------------------|
| Mobile (< 640px) | Grid 1 columna. Filtros en stack vertical. Paginacion simplificada | Lista full-width. Cabecera en stack. Chips de filtro en scroll horizontal |
| Tablet sm (640px+) | Grid 2 columnas | Lista con algo de padding lateral |
| Desktop lg (1024px+) | Grid 3 columnas. Filtros en una fila horizontal | max-w-4xl centrado |

Clases responsivas clave:
```
Grid explorar:    grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5
Filtros:          flex flex-wrap items-center gap-3
Input busqueda:   flex-1 min-w-[200px] max-w-xs
Select tipo:      w-[180px]
KPI stats:        grid-cols-3 (siempre, las cifras son cortas)
```

---

## 8. Estructura de Archivos Planificada

```
src/web/src/features/crowdpromotion/
  domain/
    types.ts                         <- ProgramaExplorarItem, MiInscripcionItem, InscripcionEstado
  application/
    useProgramasExplorar.ts          <- useQuery GET /api/crowdpromotion/programas/explorar
    useSolicitarInscripcion.ts       <- useMutation POST .../inscripcion
    useMisProgramas.ts               <- useQuery GET /api/crowdpromotion/promotor/mis-programas
  infrastructure/
    crowdpromotion.service.ts        <- llamadas API
  presentation/
    pages/
      ExplorarProgramasPage.tsx      <- Pantalla 1
      MisProgramasPage.tsx           <- Pantalla 2
    components/
      ProgramaCard.tsx               <- Card explorar (secciones 3.1.5 y 3.1.6)
      ProgramaCardSkeleton.tsx       <- Skeleton carga explorar
      ProgramaActionSlot.tsx         <- Zona accion condicional por estado
      EstadoInscripcionBadge.tsx     <- Badge reutilizable Pendiente/Aprobado/Bloqueado
      FiltroProgramas.tsx            <- Barra de filtros explorar
      EmptyStateProgramas.tsx        <- Empty/error states explorar
      BannerSinPerfilPromotor.tsx    <- Banner condicional sin perfil
      MiProgramaItem.tsx             <- Item inscripcion Mis Programas
      CodigoReferidoBlock.tsx        <- Bloque codigo con copiar
      UrlTrackingBlock.tsx           <- Bloque URL con tooltip + copiar
      ProgramaDetallExpandido.tsx    <- Collapsible detalle (tareas + KPIs)
      KpiStatCard.tsx                <- Mini cards KPI MVP
      TareaItem.tsx                  <- Item de tarea del programa
```

---

## 9. Componentes shadcn Utilizados

| Componente | Pantallas | Uso |
|-----------|----------|-----|
| `Card` | Explorar, Mis Programas | Container de ProgramaCard y MiProgramaItem |
| `Badge` | Ambas | EstadoInscripcionBadge, badge tipo programa, badge num tareas, badge tipo evento tarea |
| `Button` | Ambas | Solicitar inscripcion, limpiar filtros, copiar, ver datos, filtros estado, explorar programas |
| `Input` | Explorar | Campo busqueda de artista |
| `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | Explorar | Filtro tipo programa y rango comision |
| `Separator` | Ambas | Divisores visuales dentro de cards |
| `Skeleton` | Ambas | Loading states de cards y lista |
| `Tooltip` + `TooltipProvider` + `TooltipTrigger` + `TooltipContent` | Mis Programas | URL tracking truncada con hover tooltip |
| `Collapsible` + `CollapsibleTrigger` + `CollapsibleContent` | Mis Programas | Toggle detalle inscripcion aprobada |
| `Pagination` + `PaginationContent` + `PaginationItem` + `PaginationLink` + `PaginationPrevious` + `PaginationNext` | Explorar | Navegacion paginada del grid |

---

## 10. Checklist

- [ ] Todos los inputs tienen labels o aria-label
- [ ] Botones de accion tienen aria-label descriptivo en estados dinamicos (copiar/copiado)
- [ ] Badges de estado tienen role="status" y aria-label
- [ ] Cards clickeables tienen role="article" y aria-label con titulo y estado
- [ ] Iconos decorativos tienen aria-hidden="true"
- [ ] Boton Solicitar Inscripcion muestra spinner + texto "Enviando..." en loading
- [ ] Card bloqueada tiene aria-disabled="true" y opacity-60
- [ ] Collapsible usa aria-expanded y aria-controls correctamente
- [ ] Paginacion tiene aria-label="pagination" y pagina activa con aria-current="page"
- [ ] Contador de resultados tiene aria-live="polite"
- [ ] Estados de error tienen role="alert"
- [ ] Empty states tienen role="status"
- [ ] Grid en loading tiene aria-busy="true"
- [ ] Responsive funciona en 1, 2 y 3 columnas segun breakpoint
- [ ] Filtros accesibles via teclado (Tab + Enter/Space)
- [ ] Tooltip URL accesible via teclado
- [ ] Focus ring visible en todos los elementos interactivos
- [ ] Contraste de texto sobre fondos oscuros >= 4.5:1 (WCAG AA)
- [ ] Scroll to top del grid al cambiar pagina
- [ ] Hash navigation (`#programa-{id}`) funcional para deeplink desde Explorar a Mis Programas
