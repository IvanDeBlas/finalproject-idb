# Diseno UI: cs-explorar-propuestas (Landing)

**Fecha:** 2026-02-17
**Feature:** cs-explorar-propuestas
**Target:** src/web (Vite + React 18 + Tailwind CSS + shadcn/ui)

---

## 1. Resumen

- Componentes shadcn/ui ya instalados usados: Card, CardHeader, CardContent, CardFooter, CardTitle, CardDescription, Button, Input, Label, Textarea, Select, SelectTrigger, SelectContent, SelectItem, Badge, Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter, Alert, AlertDescription, Avatar, AvatarImage, AvatarFallback, Skeleton, Separator
- Componentes shadcn/ui faltantes por instalar: Pagination, Collapsible
- Composiciones custom: NecesidadCard, NecesidadCardSkeleton, PropuestaCard, PropuestaCardSkeleton, EstadoFilterChip, FiltrosSidebar, BudgetRangeInfo, CharacterCounter
- Responsive breakpoints: mobile (< 640px), tablet (640px - 1024px), desktop (> 1024px)
- Tema: dark theme consistente con el existente en `TemplateCard.tsx`

---

## 2. Componentes Faltantes - Instalar

Los siguientes componentes shadcn/ui no estan instalados y deben agregarse:

```bash
npx shadcn-ui@latest add pagination
npx shadcn-ui@latest add collapsible
```

| Componente | Uso |
|------------|-----|
| `Pagination` | Paginacion de listado de necesidades y mis propuestas |
| `Collapsible` | Filtros colapsables en mobile |

Los siguientes componentes YA estan instalados y se reutilizan directamente:

| Componente | Archivo |
|------------|---------|
| Card, CardHeader, CardContent, CardFooter | `components/ui/card.tsx` |
| Button | `components/ui/button.tsx` |
| Input | `components/ui/input.tsx` |
| Label | `components/ui/label.tsx` |
| Textarea | `components/ui/textarea.tsx` |
| Select, SelectTrigger, SelectContent, SelectItem | `components/ui/select.tsx` |
| Badge | `components/ui/badge.tsx` |
| Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter | `components/ui/dialog.tsx` |
| Alert, AlertDescription | `components/ui/alert.tsx` |
| Avatar, AvatarImage, AvatarFallback | `components/ui/avatar.tsx` |
| Skeleton | `components/ui/skeleton.tsx` |
| Separator | `components/ui/separator.tsx` |

---

## 2. Paleta de Colores

El proyecto usa dark theme con CSS custom properties. Las clases Tailwind a usar son valores literales (no variables CSS) siguiendo el patron establecido en `TemplateCard.tsx`.

| Uso | Valor Hex | Clase Tailwind |
|-----|-----------|----------------|
| Background pagina | `#1a1a2e` | `bg-[#1a1a2e]` |
| Background secundario | `#16213e` | `bg-[#16213e]` |
| Background card | `#0f1729` | `bg-[#0f1729]` |
| Background card hover | `#1e2a42` | `bg-[#1e2a42]` |
| Primary (purple) | `#a855f7` | `text-[#a855f7]` / `border-[#a855f7]` |
| Gradient primary | pink-500 to purple-600 | `bg-gradient-to-r from-pink-500 to-purple-600` |
| Gradient hover | pink-600 to purple-700 | `hover:from-pink-600 hover:to-purple-700` |
| Texto primario | `#ffffff` | `text-white` |
| Texto secundario | `#94a3b8` | `text-[#94a3b8]` |
| Texto muted | `#64748b` | `text-[#64748b]` |
| Texto label | `#cbd5e1` | `text-[#cbd5e1]` |
| Border default | `#334155` | `border-[#334155]` |
| Border focus | `#a855f7` | `focus:border-[#a855f7]` |
| Input background | `#0f1729` | `bg-[#0f1729]` |
| Input background (fields) | `#1a1a2e` | `bg-[#1a1a2e]` |

### Colores de Estado de Propuesta

| Estado | Background | Border | Texto | Clases Tailwind |
|--------|-----------|--------|-------|-----------------|
| Pendiente | amber-900/30 | amber-600 | amber-300 | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aceptada | green-900/30 | green-600 | green-300 | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazada | red-900/30 | red-600 | red-300 | `bg-red-900/30 border-red-600 text-red-300` |
| Retirada | gray-900/30 | gray-600 | gray-400 | `bg-gray-900/30 border-gray-600 text-gray-400` |

### Colores de Estado de Necesidad / Urgencia

| Uso | Clase Tailwind |
|-----|----------------|
| Necesidad Abierta badge | `bg-emerald-900/30 border-emerald-700 text-emerald-300` |
| Urgencia badge | `bg-red-900/30 border-red-700 text-red-300` |
| Fecha limite normal (> 7 dias) | `text-[#94a3b8]` |
| Fecha limite proxima (3-7 dias) | `text-amber-400` |
| Fecha limite critica (< 3 dias) | `text-red-400` |

### Colores de Info de Precio

| Situacion | Background | Border | Texto |
|-----------|-----------|--------|-------|
| Dentro del rango | `bg-green-900/20` | `border-green-700/50` | `text-green-300` |
| Por encima del rango | `bg-amber-900/20` | `border-amber-700/50` | `text-amber-300` |
| Por debajo del rango | `bg-blue-900/20` | `border-blue-700/50` | `text-blue-300` |

---

## 3. Design Tokens Adicionales para Tailwind Config

Las siguientes animaciones deben agregarse a `tailwind.config.ts` en la seccion `keyframes` y `animation`:

```typescript
// Agregar en theme.extend.keyframes:
"urgency-pulse": {
  "0%, 100%": { opacity: "1" },
  "50%": { opacity: "0.7" },
},
"shake": {
  "0%, 100%": { transform: "translateX(0)" },
  "25%": { transform: "translateX(-4px)" },
  "75%": { transform: "translateX(4px)" },
},

// Agregar en theme.extend.animation:
"urgency-pulse": "urgency-pulse 2000ms ease-in-out infinite",
"shake": "shake 400ms ease",
```

---

## 4. Componentes por Pantalla

---

### 4.1 Pantalla: Explorar Necesidades (Listado)

**Ruta:** `/crowdsourcing/necesidades`
**Archivo:** `src/web/src/features/crowdsourcing/necesidades/presentation/pages/NecesidadesPublicasPage.tsx`

#### Layout Desktop

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR - reutilizado del landing]                                   │
├──────────────────────────────────────────────────────────────────────┤
│  bg-[#16213e] py-12 px-4  (Page Hero)                                │
│  <h1> Explorar necesidades                                            │
│  <p>  Encuentra oportunidades de trabajo en la industria musical      │
│                                                                      │
│  <div max-w-2xl mx-auto> [Search Input]  [Buscar Button]             │
├──────────────────────────────────────────────────────────────────────┤
│  max-w-7xl mx-auto px-4 py-8                                         │
│  ┌──────────────────┐  ┌────────────────────────────────────────────┐ │
│  │ <aside w-64>     │  │ <main flex-1>                              │ │
│  │ FiltrosSidebar   │  │  [Ordenar Select]  [N necesidades]         │ │
│  │ (Card sticky)    │  │                                            │ │
│  │                  │  │  grid grid-cols-2 gap-4                    │ │
│  │                  │  │  [NecesidadCard] [NecesidadCard]           │ │
│  │                  │  │  [NecesidadCard] [NecesidadCard]           │ │
│  │                  │  │  ...                                       │ │
│  │                  │  │                                            │ │
│  │                  │  │  [Pagination]                              │ │
│  └──────────────────┘  └────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────────┘
```

#### Layout Mobile (< 640px)

```
┌─────────────────────────────────────┐
│  [NAVBAR hamburger]                 │
├─────────────────────────────────────┤
│  bg-[#16213e] py-8 px-4             │
│  <h1> Explorar necesidades          │
│  [Search Input full-width]          │
│  [Buscar Button full-width]         │
│                                     │
│  [Collapsible: Filtros accordion]   │
│  [Ordenar Select full-width]        │
│                                     │
│  grid grid-cols-1 gap-4             │
│  [NecesidadCard full-width]         │
│  [NecesidadCard full-width]         │
│  ...                                │
│  [Pagination]                       │
└─────────────────────────────────────┘
```

#### Componentes de la Pantalla

**Hero Section**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Hero container | `<div>` | `bg-[#16213e] py-12 px-4` |
| Page title | `<h1>` | `text-3xl font-bold text-white mb-2 text-center` |
| Page subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8 text-center` |
| Search container | `<div>` | `max-w-2xl mx-auto flex gap-2` |
| Search input | `Input` shadcn | `flex-1 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] h-12 text-base focus:border-[#a855f7]` |
| Search button | `Button` shadcn | `h-12 px-6 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold` |

**Composicion Hero:**
```tsx
<div className="bg-[#16213e] py-12 px-4">
    <h1 className="text-3xl font-bold text-white mb-2 text-center">
        Explorar necesidades
    </h1>
    <p className="text-lg text-[#94a3b8] mb-8 text-center">
        Encuentra oportunidades de trabajo en la industria musical
    </p>
    <div className="max-w-2xl mx-auto flex gap-2">
        <Input
            placeholder="Buscar en titulo y descripcion..."
            className="flex-1 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] h-12 text-base"
        />
        <Button className="h-12 px-6 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
            <Search className="w-4 h-4 mr-2" aria-hidden="true" />
            Buscar
        </Button>
    </div>
</div>
```

**Sidebar de Filtros (Desktop)**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Aside container | `<aside>` | `w-64 flex-shrink-0 hidden lg:block` |
| Card contenedor | `Card` shadcn | `bg-[#0f1729] border-[#334155] p-5 sticky top-24` |
| Titulo filtros | `<h2>` | `text-base font-semibold text-white mb-4` |
| Section label | `<h3>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` |
| Chips container Tipo | `<div>` | `flex flex-wrap gap-2 mb-5` |
| Chip no seleccionado | `<button>` | `px-3 py-1 rounded-full text-sm border border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-white transition-all duration-150` |
| Chip seleccionado | `<button>` | `px-3 py-1 rounded-full text-sm bg-purple-600/20 border border-[#a855f7] text-white` |
| Modalidad Select | `Select` shadcn | `w-full mb-5` con trigger: `bg-[#0f1729] border-[#334155] text-white` |
| Presupuesto range row | `<div>` | `flex gap-2 items-center mb-5` |
| Presupuesto input (min/max) | `Input type="number"` | `bg-[#1a1a2e] border-[#334155] text-white text-sm` |
| Pais Select | `Select` shadcn | `w-full mb-3` con trigger: `bg-[#0f1729] border-[#334155] text-white` |
| Ciudad input | `Input` shadcn | `bg-[#1a1a2e] border-[#334155] text-white text-sm w-full mb-5` (visible solo si hay pais) |
| Separator | `Separator` shadcn | `my-4 bg-[#334155]` |
| Limpiar button | `Button variant="ghost"` | `w-full text-[#94a3b8] hover:text-white hover:bg-[#1e2a42] mt-2` |

**Composicion Sidebar Filtros:**
```tsx
<aside className="w-64 flex-shrink-0 hidden lg:block">
    <Card className="bg-[#0f1729] border-[#334155] p-5 sticky top-24">
        <h2 className="text-base font-semibold text-white mb-4">Filtros</h2>

        <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">Tipo</h3>
        <div className="flex flex-wrap gap-2 mb-5" role="group" aria-label="Filtrar por tipo de necesidad">
            {tipos.map(tipo => (
                <button
                    key={tipo.id}
                    aria-pressed={selectedTipos.includes(tipo.id)}
                    aria-label={`Filtrar por tipo: ${tipo.nombre}`}
                    className={isSelected ? "px-3 py-1 rounded-full text-sm bg-purple-600/20 border border-[#a855f7] text-white" : "px-3 py-1 rounded-full text-sm border border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-white transition-all duration-150"}
                >
                    {tipo.nombre}
                </button>
            ))}
        </div>

        <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">Modalidad</h3>
        <Select>
            <SelectTrigger className="bg-[#0f1729] border-[#334155] text-white mb-5 w-full">
                <SelectValue placeholder="Todas" />
            </SelectTrigger>
            <SelectContent className="bg-[#0f1729] border-[#334155]">
                <SelectItem value="all">Todas</SelectItem>
                <SelectItem value="2">Remoto</SelectItem>
                <SelectItem value="1">Presencial</SelectItem>
                <SelectItem value="3">Hibrido</SelectItem>
            </SelectContent>
        </Select>

        <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">Presupuesto (EUR)</h3>
        <div className="flex gap-2 items-center mb-5">
            <Input type="number" placeholder="Min" className="bg-[#1a1a2e] border-[#334155] text-white text-sm" />
            <span className="text-[#64748b] text-sm">-</span>
            <Input type="number" placeholder="Max" className="bg-[#1a1a2e] border-[#334155] text-white text-sm" />
        </div>

        <Separator className="my-4 bg-[#334155]" />

        <Button variant="ghost" className="w-full text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]">
            Limpiar filtros
        </Button>
    </Card>
</aside>
```

**Filtros Mobile (Collapsible)**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Toggle button | `Button variant="outline"` | `lg:hidden mb-4 border-[#334155] text-white w-full flex items-center justify-between` |
| Toggle icon | `SlidersHorizontal` Lucide | `w-4 h-4` |
| Collapsible root | `Collapsible` shadcn | `lg:hidden mb-4` |
| Collapsible content | `CollapsibleContent` | `bg-[#0f1729] border border-[#334155] rounded-lg p-4 space-y-4` |

**Content Area**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Main container | `<main>` | `flex-1 min-w-0` |
| Header row | `<div>` | `flex items-center justify-between mb-6` |
| Results count | `<p>` | `text-sm text-[#94a3b8]` |
| Ordenar Select | `Select` shadcn | `w-48` con trigger: `bg-[#0f1729] border-[#334155] text-white` |
| Cards grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4 mb-8` |
| Pagination | `Pagination` shadcn | `flex justify-center mt-8` |

---

### 4.2 Componente Reutilizable: NecesidadCard

**Archivo:** `src/web/src/features/crowdsourcing/necesidades/presentation/components/NecesidadCard.tsx`

#### Layout interno

```
┌──────────────────────────────────────────────────────┐
│  [TIPO-BADGE]  [MODALIDAD-BADGE]      [!URGENTE!]    │
│                                                      │
│  Titulo de la necesidad (font-bold text-white)       │
│                                                      │
│  Descripcion truncada a 150 chars con "..."          │
│  (text-sm text-[#94a3b8] line-clamp-3)               │
│                                                      │
│  [ArtistaNombre]          [icono] Min - Max EUR      │
│                                                      │
│  [Clock] hace 2 dias   [Users] 3 propuestas          │
│  [Calendar] Limite: 15 mar 2026                      │
└──────────────────────────────────────────────────────┘
```

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Card container | `Card` shadcn | `bg-[#0f1729] border border-[#334155] rounded-xl p-5 cursor-pointer transition-all duration-200 ease-out hover:bg-[#1e2a42] hover:border-purple-500/50 hover:shadow-[0_8px_24px_rgba(0,0,0,0.4)] hover:scale-[1.01]` |
| Badges row | `<div>` | `flex items-center gap-2 mb-3 flex-wrap` |
| Tipo badge | `Badge` shadcn custom | `bg-purple-900/30 text-purple-300 border border-purple-700 text-xs font-medium` |
| Modalidad badge | `Badge variant="outline"` | `border-[#334155] text-[#94a3b8] text-xs flex items-center gap-1` |
| Urgencia badge | `Badge` shadcn custom | `bg-red-900/30 text-red-300 border border-red-700 text-xs flex items-center gap-1 animate-urgency-pulse` |
| Titulo | `<h3>` | `text-base font-bold text-white mb-2 leading-tight` |
| Descripcion | `<p>` | `text-sm text-[#94a3b8] leading-relaxed mb-4 line-clamp-3` |
| Info row (artista + precio) | `<div>` | `flex items-center justify-between text-sm mb-3` |
| Artista nombre | `<span>` | `text-[#94a3b8] font-medium` |
| Presupuesto | `<span>` | `text-[#a855f7] font-semibold flex items-center gap-1` |
| Stats row (fecha + propuestas) | `<div>` | `flex items-center gap-4 text-xs text-[#64748b] mb-2` |
| Stat item | `<span>` | `flex items-center gap-1` |
| Fecha limite | `<span>` | Color dinamico segun urgencia: `text-[#94a3b8]` / `text-amber-400` / `text-red-400` |

**Composicion NecesidadCard:**
```tsx
<Card
    role="article"
    tabIndex={0}
    aria-label={`${necesidad.titulo}. Tipo: ${necesidad.tipoNecesidadNombre}. Modalidad: ${necesidad.modalidadTrabajoNombre}. Presupuesto ${necesidad.presupuestoMin} a ${necesidad.presupuestoMax} ${necesidad.monedaNombre}. ${necesidad.numeroPropuestas} propuestas.`}
    onClick={onClick}
    onKeyDown={(e) => (e.key === 'Enter' || e.key === ' ') && onClick?.()}
    className="bg-[#0f1729] border border-[#334155] rounded-xl p-5 cursor-pointer transition-all duration-200 ease-out hover:bg-[#1e2a42] hover:border-purple-500/50 hover:shadow-[0_8px_24px_rgba(0,0,0,0.4)] hover:scale-[1.01]"
>
    <div className="flex items-center gap-2 mb-3 flex-wrap">
        <Badge className="bg-purple-900/30 text-purple-300 border border-purple-700 text-xs">
            {necesidad.tipoNecesidadNombre}
        </Badge>
        <Badge variant="outline" className="border-[#334155] text-[#94a3b8] text-xs flex items-center gap-1">
            <ModalidadIcon className="w-3 h-3" aria-hidden="true" />
            {necesidad.modalidadTrabajoNombre}
        </Badge>
        {necesidad.esUrgente && (
            <Badge
                className="bg-red-900/30 text-red-300 border border-red-700 text-xs flex items-center gap-1 animate-urgency-pulse"
                aria-label="Urgente: menos de 3 dias para el cierre"
            >
                <Clock className="w-3 h-3" aria-hidden="true" />
                URGENTE
            </Badge>
        )}
    </div>

    <h3 className="text-base font-bold text-white mb-2 leading-tight">
        {necesidad.titulo}
    </h3>
    <p className="text-sm text-[#94a3b8] leading-relaxed mb-4 line-clamp-3">
        {necesidad.descripcion}
    </p>

    <div className="flex items-center justify-between text-sm mb-3">
        <span className="text-[#94a3b8] font-medium">{necesidad.artistaNombre}</span>
        <span className="text-[#a855f7] font-semibold flex items-center gap-1">
            <DollarSign className="w-3 h-3" aria-hidden="true" />
            {necesidad.presupuestoMin} - {necesidad.presupuestoMax} {necesidad.monedaNombre}
        </span>
    </div>

    <div className="flex items-center gap-4 text-xs text-[#64748b] mb-2">
        <span className="flex items-center gap-1">
            <Clock className="w-3 h-3" aria-hidden="true" />
            {necesidad.fechaRelativa}
        </span>
        <span className="flex items-center gap-1">
            <Users className="w-3 h-3" aria-hidden="true" />
            {necesidad.numeroPropuestas > 0 ? `${necesidad.numeroPropuestas} propuesta(s)` : 'Sin propuestas aun'}
        </span>
    </div>

    {necesidad.fechaLimitePropuestas && (
        <div className={`flex items-center gap-1 text-xs ${fechaLimiteStyle}`}>
            <Calendar className="w-3 h-3" aria-hidden="true" />
            <span aria-label={`Fecha limite: ${fechaFormateada}. ${diasRestantes} dias restantes.`}>
                Limite: {fechaFormateada}
            </span>
        </div>
    )}
</Card>
```

---

### 4.3 Componente Reutilizable: NecesidadCardSkeleton

**Archivo:** `src/web/src/features/crowdsourcing/necesidades/presentation/components/NecesidadCardSkeleton.tsx`

```tsx
<Card className="bg-[#0f1729] border-[#334155] rounded-xl p-5">
    <div role="status" aria-label="Cargando necesidad...">
        <div className="flex items-center gap-2 mb-3">
            <Skeleton className="h-5 w-28 rounded-full" />
            <Skeleton className="h-5 w-20 rounded-full" />
        </div>
        <Skeleton className="h-6 w-4/5 mb-2" />
        <Skeleton className="h-4 w-full mb-1" />
        <Skeleton className="h-4 w-3/4 mb-4" />
        <div className="flex justify-between mb-3">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-4 w-28" />
        </div>
        <div className="flex gap-4">
            <Skeleton className="h-3 w-24" />
            <Skeleton className="h-3 w-24" />
        </div>
    </div>
</Card>
```

---

### 4.4 Pantalla: Detalle de Necesidad

**Ruta:** `/crowdsourcing/necesidades/:id`
**Archivo:** `src/web/src/features/crowdsourcing/necesidades/presentation/pages/NecesidadPublicaDetailPage.tsx`

#### Layout Desktop

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                            │
├──────────────────────────────────────────────────────────────────────┤
│  max-w-6xl mx-auto px-4 py-8                                         │
│  [< Explorar necesidades] (back link)                                │
│                                                                      │
│  ┌──────────────────────────────────────┐  ┌──────────────────────┐  │
│  │ <main flex-1 min-w-0>               │  │ <aside w-80          │  │
│  │                                     │  │  hidden md:block     │  │
│  │ [Header Card]                       │  │  sticky top-24>      │  │
│  │  - Badges row (tipo, modalidad,     │  │                      │  │
│  │    urgencia)                        │  │ [Action Card]        │  │
│  │  - Titulo h1                        │  │  - Presupuesto label │  │
│  │  - Artista row (Avatar + nombre)    │  │  - Presupuesto value │  │
│  │                                     │  │  - [CTA Button]      │  │
│  │ [Description Card]                  │  │  - Separator         │  │
│  │  - "Descripcion" h2                 │  │  - Stats (propuestas,│  │
│  │  - Descripcion completa             │  │    publicado,        │  │
│  │                                     │  │    fecha limite,     │  │
│  │ [Details Card]                      │  │    inicio previsto)  │  │
│  │  - "Detalles" h2                    │  └──────────────────────┘  │
│  │  - Grid 2 cols (tipo, modalidad,    │                            │
│  │    ubicacion, fechas)               │                            │
│  └──────────────────────────────────────┘                           │
└──────────────────────────────────────────────────────────────────────┘
```

#### Componentes del Detalle

**Back Link**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Back link | `<Link>` React Router | `flex items-center gap-2 text-[#94a3b8] hover:text-white transition-colors mb-6` |
| Arrow icon | `ArrowLeft` Lucide | `w-4 h-4` |

**Header Card**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Card container | `Card` shadcn | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Badges row | `<div>` | `flex items-center gap-2 mb-3 flex-wrap` |
| Tipo badge | `Badge` custom | `bg-purple-900/30 text-purple-300 border border-purple-700` |
| Modalidad badge | `Badge variant="outline"` | `border-[#334155] text-[#94a3b8] flex items-center gap-1` |
| Urgencia badge | `Badge` custom | `bg-red-900/30 text-red-300 border border-red-700 flex items-center gap-1 animate-urgency-pulse` |
| Titulo | `<h1>` | `text-2xl md:text-3xl font-bold text-white mb-4 leading-tight` |
| Artista row | `<div>` | `flex items-center gap-3` |
| Avatar | `Avatar` shadcn | `w-8 h-8` |
| AvatarImage | `AvatarImage` | `alt="{nombre} avatar"` |
| AvatarFallback | `AvatarFallback` | `bg-[#334155] text-[#94a3b8] text-xs` |
| Artista nombre | `<span>` | `text-sm font-medium text-[#94a3b8]` |

**Description Card**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Card container | `Card` shadcn | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section title | `<h2>` | `text-base font-semibold text-white mb-3` |
| Descripcion | `<p>` | `text-base text-[#94a3b8] leading-relaxed whitespace-pre-line` |

**Details Card**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Card container | `Card` shadcn | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section title | `<h2>` | `text-base font-semibold text-white mb-4` |
| Details grid | `<div>` | `grid grid-cols-2 gap-4` (1 col en mobile) |
| Detail item | `<div>` | `flex flex-col gap-1` |
| Detail label | `<span>` | `text-xs text-[#64748b] uppercase tracking-wide` |
| Detail value | `<span>` | `text-sm font-medium text-white` |

**Action Sidebar Card**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Aside container | `<aside>` | `w-80 flex-shrink-0 hidden md:block sticky top-24` |
| Card container | `Card` shadcn | `bg-[#0f1729] border-[#334155] p-6` |
| Budget label | `<p>` | `text-xs text-[#64748b] uppercase tracking-wide mb-1` |
| Budget value | `<p>` | `text-2xl font-bold text-white mb-4` |
| Separator | `Separator` | `my-4 bg-[#334155]` |
| Stats container | `<div>` | `space-y-3` |
| Stat row | `<div>` | `flex items-center justify-between` |
| Stat label | `<span>` | `text-xs text-[#64748b] flex items-center gap-1` |
| Stat value | `<span>` | `text-sm font-medium text-white` |
| Fecha limite (urgente) | `<span>` | `text-sm font-medium text-amber-400` o `text-red-400` segun dias |

**CTA States en Action Card**

| Estado | Composicion |
|--------|------------|
| Sin autenticar | `Button w-full h-12 bg-gradient-to-r from-pink-500 to-purple-600` con texto "Inicia sesion para enviar propuesta" |
| Sin PerfilProfesional | `<div bg-[#1e2a42] border border-[#334155] rounded-lg p-4>` con texto info + `Button variant="outline" w-full border-purple-500 text-purple-300` |
| Con perfil, sin propuesta | `Button w-full h-12 bg-gradient-to-r from-pink-500 to-purple-600` + texto "Enviar propuesta" que abre Dialog |
| Ya propuso | `<div bg-green-900/20 border border-green-700 rounded-lg p-4>` con `CheckCircle` icono verde + texto |
| Es propietario | `<div bg-[#1e2a42] border border-[#334155] rounded-lg p-4>` con `Info` icono + texto |
| Necesidad expirada | `Alert bg-amber-900/20 border-amber-700` con texto de advertencia |

**Composicion CTA "Con perfil, sin propuesta":**
```tsx
<Button
    className="w-full h-12 text-base font-semibold bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 mb-4"
    onClick={onEnviarPropuesta}
    aria-label="Enviar propuesta para esta necesidad"
>
    Enviar propuesta
</Button>
```

**Composicion CTA "Ya propuso":**
```tsx
<div className="bg-green-900/20 border border-green-700 rounded-lg p-4 mb-4 flex items-center gap-2">
    <CheckCircle className="w-5 h-5 text-green-400 flex-shrink-0" aria-hidden="true" />
    <p className="text-sm text-green-300">Ya enviaste una propuesta para esta necesidad</p>
</div>
```

**Mobile CTA Sticky Bottom:**
```tsx
<div className="fixed bottom-0 left-0 right-0 p-4 bg-[#0f1729] border-t border-[#334155] md:hidden z-50">
    {/* CTA segun estado del usuario */}
</div>
```

---

### 4.5 Pantalla: Enviar Propuesta (Modal Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/propuestas/presentation/components/EnviarPropuestaDialog.tsx`
**Contexto:** Se abre desde el detalle de la necesidad

#### Layout del Dialog

```
┌─────────────────────────────────────────────────────┐
│  DialogHeader                                       │
│  "Enviar propuesta"                          [×]    │
│  "Para: {necesidad.titulo}"                         │
│  "Presupuesto del artista: 150 - 800 EUR"           │
├─────────────────────────────────────────────────────┤
│  DialogBody (py-6 space-y-5)                        │
│                                                     │
│  [Label* Precio]     [Label* Moneda]                │
│  [Input number]      [Select EUR/USD]               │
│  [BudgetRangeInfo - verde/amber/azul]               │
│                                                     │
│  [Label Dias estimados]                             │
│  [Input number w-40]                                │
│  [Hint: Opcional - cuantos dias necesitas]          │
│                                                     │
│  [Label* Mensaje de propuesta]                      │
│  [Textarea min-h-[140px]]                           │
│  [CharacterCounter: 245 / 2000 (min 20)]            │
│                                                     │
├─────────────────────────────────────────────────────┤
│  DialogFooter                                       │
│  [Cancelar - outline]    [Enviar propuesta - grad]  │
└─────────────────────────────────────────────────────┘
```

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Dialog | `Dialog` shadcn | `open={isOpen} onOpenChange={onClose}` |
| DialogContent | `DialogContent` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| DialogHeader | `DialogHeader` | `pb-4 border-b border-[#334155]` |
| DialogTitle | `DialogTitle` | `text-xl font-semibold text-white` |
| DialogDescription (para) | `DialogDescription` | `text-sm text-[#94a3b8] mt-1` |
| Presupuesto referencia | `<p>` | `text-xs text-[#64748b] mt-1` |
| Form body | `<div>` | `py-6 space-y-5` |
| Precio + Moneda row | `<div>` | `grid grid-cols-2 gap-3` |
| Label con asterisco | `Label` shadcn | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Required mark | `<span aria-hidden="true">` | `text-red-400 ml-1` |
| Precio Input | `Input type="number"` | `bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7]` |
| Moneda Select trigger | `SelectTrigger` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| BudgetRangeInfo | componente custom | Ver composicion abajo |
| Dias Label | `Label` | igual al precio label |
| Dias Input | `Input type="number"` | `bg-[#1a1a2e] border-[#334155] text-white h-11 w-40` |
| Dias hint | `<p>` | `text-xs text-[#64748b] mt-1` |
| Mensaje Textarea | `Textarea` shadcn | `bg-[#1a1a2e] border-[#334155] text-white min-h-[140px] resize-none focus:border-[#a855f7]` |
| CharacterCounter | `<div>` | `flex justify-between mt-1` |
| Counter normal | `<span aria-live="polite" aria-atomic="true">` | `text-xs text-[#64748b]` |
| Counter al limite | `<span>` | `text-xs text-red-400` |
| Error message | `<p role="alert">` | `text-sm text-red-400 mt-1` |
| DialogFooter | `DialogFooter` | `pt-4 border-t border-[#334155] flex justify-between` |
| Cancelar button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit button | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50` |
| Submit loading | `Button disabled` | con `Loader2 animate-spin w-4 h-4 mr-2` + "Enviando..." |

**Composicion BudgetRangeInfo:**
```tsx
// Dentro del rango (verde)
<div className="bg-green-900/20 border border-green-700/50 rounded-md p-3 flex items-center gap-2">
    <CheckCircle className="w-4 h-4 text-green-400 flex-shrink-0" aria-hidden="true" />
    <p className="text-sm text-green-300">Tu precio esta dentro del rango (150-800 EUR)</p>
</div>

// Por encima (amber)
<div className="bg-amber-900/20 border border-amber-700/50 rounded-md p-3 flex items-center gap-2">
    <AlertTriangle className="w-4 h-4 text-amber-400 flex-shrink-0" aria-hidden="true" />
    <p className="text-sm text-amber-300">Tu precio esta por encima del presupuesto indicado (max 800 EUR)</p>
</div>

// Por debajo (azul)
<div className="bg-blue-900/20 border border-blue-700/50 rounded-md p-3 flex items-center gap-2">
    <Info className="w-4 h-4 text-blue-400 flex-shrink-0" aria-hidden="true" />
    <p className="text-sm text-blue-300">Tu precio esta por debajo del presupuesto indicado (min 150 EUR)</p>
</div>
```

**Composicion completa Dialog:**
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen}>
    <DialogContent
        className="max-w-lg bg-[#0f1729] border-[#334155] text-white"
        aria-labelledby="enviar-propuesta-title"
        aria-describedby="enviar-propuesta-desc"
    >
        <DialogHeader className="pb-4 border-b border-[#334155]">
            <DialogTitle id="enviar-propuesta-title" className="text-xl font-semibold text-white">
                Enviar propuesta
            </DialogTitle>
            <DialogDescription id="enviar-propuesta-desc" className="text-sm text-[#94a3b8] mt-1">
                Para: {necesidad.titulo}
            </DialogDescription>
            <p className="text-xs text-[#64748b] mt-1">
                Presupuesto del artista: {necesidad.presupuestoMin} - {necesidad.presupuestoMax} {necesidad.monedaNombre}
            </p>
        </DialogHeader>

        <div className="py-6 space-y-5">
            <div className="grid grid-cols-2 gap-3">
                <div>
                    <Label htmlFor="precioPropuesto" className="text-sm font-medium text-[#cbd5e1] mb-1.5 block">
                        Precio propuesto
                        <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                    </Label>
                    <Input
                        id="precioPropuesto"
                        type="number"
                        aria-required="true"
                        aria-describedby="precio-error"
                        className="bg-[#1a1a2e] border-[#334155] text-white h-11"
                    />
                    <p id="precio-error" role="alert" className="text-sm text-red-400 mt-1 animate-shake">
                        {errors.precioPropuesto?.message}
                    </p>
                </div>
                <div>
                    <Label htmlFor="monedaId" className="text-sm font-medium text-[#cbd5e1] mb-1.5 block">
                        Moneda
                        <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                    </Label>
                    <Select>
                        <SelectTrigger id="monedaId" className="bg-[#1a1a2e] border-[#334155] text-white h-11" aria-required="true">
                            <SelectValue placeholder="Seleccionar" />
                        </SelectTrigger>
                        <SelectContent className="bg-[#0f1729] border-[#334155]">
                            <SelectItem value="1">EUR</SelectItem>
                            <SelectItem value="2">USD</SelectItem>
                        </SelectContent>
                    </Select>
                </div>
            </div>

            {/* BudgetRangeInfo - condicional */}

            <div>
                <Label htmlFor="diasEstimados" className="text-sm font-medium text-[#cbd5e1] mb-1.5 block">
                    Tiempo estimado (dias)
                </Label>
                <Input
                    id="diasEstimados"
                    type="number"
                    className="bg-[#1a1a2e] border-[#334155] text-white h-11 w-40"
                />
                <p className="text-xs text-[#64748b] mt-1">
                    Opcional - cuantos dias necesitas para completar
                </p>
            </div>

            <div>
                <Label htmlFor="mensajePropuesta" className="text-sm font-medium text-[#cbd5e1] mb-1.5 block">
                    Mensaje de propuesta
                    <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                </Label>
                <Textarea
                    id="mensajePropuesta"
                    aria-required="true"
                    aria-describedby="mensaje-counter mensaje-error"
                    className="bg-[#1a1a2e] border-[#334155] text-white min-h-[140px] resize-none"
                />
                <div className="flex justify-between mt-1">
                    <p id="mensaje-error" role="alert" className="text-sm text-red-400">
                        {errors.mensajePropuesta?.message}
                    </p>
                    <span
                        id="mensaje-counter"
                        aria-live="polite"
                        aria-atomic="true"
                        className={charCount > 1800 ? "text-xs text-red-400" : charCount > 1400 ? "text-xs text-amber-400" : "text-xs text-[#64748b]"}
                    >
                        {charCount} / 2000 caracteres (min 20)
                    </span>
                </div>
            </div>
        </div>

        <DialogFooter className="pt-4 border-t border-[#334155] flex justify-between">
            <Button variant="outline" className="border-[#334155] text-white hover:bg-[#1e2a42]" onClick={onClose}>
                Cancelar
            </Button>
            <Button
                type="submit"
                disabled={isPending || !isValid}
                aria-busy={isPending}
                aria-label={isPending ? "Enviando propuesta..." : "Enviar propuesta"}
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50"
            >
                {isPending && <Loader2 className="animate-spin w-4 h-4 mr-2" aria-hidden="true" />}
                {isPending ? "Enviando..." : "Enviar propuesta"}
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

### 4.6 Pantalla: Mis Propuestas (Listado)

**Ruta:** `/crowdsourcing/mis-propuestas`
**Archivo:** `src/web/src/features/crowdsourcing/propuestas/presentation/pages/MisPropuestasPage.tsx`

#### Layout

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                            │
├──────────────────────────────────────────────────────────────────────┤
│  max-w-4xl mx-auto px-4 py-8                                         │
│                                                                      │
│  <h1> Mis propuestas                                                 │
│  <p>  Seguimiento de las propuestas que has enviado                  │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │ Filter bar - flex flex-wrap gap-2                                │ │
│  │ [Todas] [Pendiente] [Aceptada] [Rechazada] [Retirada]           │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  <p> 8 propuestas                                                    │
│                                                                      │
│  space-y-4 mb-8                                                      │
│  [PropuestaCard]                                                     │
│  [PropuestaCard]                                                     │
│  ...                                                                 │
│  [Pagination]                                                        │
└──────────────────────────────────────────────────────────────────────┘
```

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Page container | `<div>` | `max-w-4xl mx-auto px-4 py-8` |
| Page title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page subtitle | `<p>` | `text-base text-[#94a3b8] mb-6` |
| Filter bar | `<div>` | `flex flex-wrap gap-2 mb-6 p-4 bg-[#0f1729] border border-[#334155] rounded-lg overflow-x-auto` |
| Filter label | `<span>` | `text-sm text-[#94a3b8] mr-2 self-center flex-shrink-0` |
| Results count | `<p>` | `text-sm text-[#94a3b8] mb-4` |
| Cards list | `<div>` | `space-y-4 mb-8` |
| Pagination | `Pagination` shadcn | `flex justify-center mt-8` |

**EstadoFilterChip - Estilos por Estado:**

| Estado | No seleccionado | Seleccionado |
|--------|----------------|--------------|
| Todas | `bg-[#1e2a42] border-[#334155] text-[#94a3b8]` | `bg-white/10 border-white text-white` |
| Pendiente | (igual no seleccionado) | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aceptada | (igual no seleccionado) | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazada | (igual no seleccionado) | `bg-red-900/30 border-red-600 text-red-300` |
| Retirada | (igual no seleccionado) | `bg-gray-900/30 border-gray-600 text-gray-300` |

**Clases base de todos los chips:** `px-4 py-1.5 rounded-full text-sm border transition-all cursor-pointer focus:outline-none focus:ring-2 focus:ring-[#a855f7] focus:ring-offset-2 focus:ring-offset-[#1a1a2e]`

**Composicion EstadoFilterChip:**
```tsx
<button
    aria-pressed={isSelected}
    aria-label={`Filtrar por estado: ${label}`}
    className={`px-4 py-1.5 rounded-full text-sm border transition-all cursor-pointer focus:outline-none focus:ring-2 focus:ring-[#a855f7] focus:ring-offset-2 focus:ring-offset-[#1a1a2e] ${selectedClasses}`}
    onClick={() => onSelect(estado)}
>
    {label}
</button>
```

---

### 4.7 Componente Reutilizable: PropuestaCard

**Archivo:** `src/web/src/features/crowdsourcing/propuestas/presentation/components/PropuestaCard.tsx`

#### Layout interno

```
┌──────────────────────────────────────────────────────┐
│  [Link] Titulo de la necesidad   [ESTADO-BADGE]      │
│  Artista: {artistaNombre}                            │
│                                                      │
│  Mi precio: {precio} {moneda}                        │
│  Enviada: {fechaCreacion}                            │
│  Respuesta: {fechaActualizacion} (si existe)         │
│                                                      │
│                          [Accion segun estado]       │
└──────────────────────────────────────────────────────┘
```

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Card container | `Card` shadcn | `bg-[#0f1729] border border-[#334155] rounded-xl p-5` |
| Header row | `<div>` | `flex items-start justify-between mb-3` |
| Titulo (link) | `<Link>` React Router | `text-base font-semibold text-white hover:text-[#a855f7] transition-colors flex-1 mr-4` |
| Estado badge | `Badge` custom | estilos segun estado (ver tabla colores) |
| Info section | `<div>` | `space-y-1 mb-4` |
| Artista | `<p>` | `text-sm text-[#94a3b8]` |
| Precio | `<p>` | `text-sm text-[#94a3b8]` con valor en `text-white font-medium` |
| Fecha enviada | `<p>` | `text-sm text-[#94a3b8]` |
| Fecha respuesta | `<p>` | `text-sm text-[#94a3b8]` (solo si existe) |
| Actions row | `<div>` | `flex justify-end` |

**Actions por Estado:**

| Estado | Componente y estilos |
|--------|---------------------|
| Pendiente | `Button variant="outline" size="sm"` con `border-red-400/50 text-red-400 hover:bg-red-900/20 hover:text-red-300` |
| Aceptada | `Button size="sm"` con `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 flex items-center gap-1` + `ArrowRight w-3 h-3` |
| Rechazada | Sin boton de accion. Solo lectura |
| Retirada | Sin boton de accion. Solo lectura |

**Estado Badges (clase base: `border text-xs font-medium px-2.5 py-0.5 rounded-full`):**

| Estado | Clases adicionales |
|--------|--------------------|
| Pendiente | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aceptada | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazada | `bg-red-900/30 border-red-600 text-red-300` |
| Retirada | `bg-gray-900/30 border-gray-600 text-gray-400` |

**Composicion PropuestaCard:**
```tsx
<Card className="bg-[#0f1729] border border-[#334155] rounded-xl p-5">
    <div className="flex items-start justify-between mb-3">
        <Link
            to={`/crowdsourcing/necesidades/${propuesta.necesidadId}`}
            className="text-base font-semibold text-white hover:text-[#a855f7] transition-colors flex-1 mr-4"
        >
            {propuesta.necesidadTitulo}
        </Link>
        <Badge
            className={`border text-xs font-medium px-2.5 py-0.5 rounded-full flex-shrink-0 ${estadoBadgeClasses}`}
            aria-label={`Estado: ${propuesta.estadoPropuestaNombre}`}
        >
            {propuesta.estadoPropuestaNombre}
        </Badge>
    </div>

    <div className="space-y-1 mb-4">
        <p className="text-sm text-[#94a3b8]">
            Artista: <span className="text-white font-medium">{propuesta.artistaNombre}</span>
        </p>
        <p className="text-sm text-[#94a3b8]">
            Mi precio: <span className="text-white font-medium">{propuesta.precioPropuesto} {propuesta.monedaNombre}</span>
        </p>
        <p className="text-sm text-[#94a3b8]">
            Enviada: <span className="text-white">{formatDate(propuesta.fechaCreacion)}</span>
        </p>
        {propuesta.fechaActualizacion && (
            <p className="text-sm text-[#94a3b8]">
                Respuesta: <span className="text-white">{formatDate(propuesta.fechaActualizacion)}</span>
            </p>
        )}
    </div>

    <div className="flex justify-end">
        {propuesta.estadoPropuestaNombre === 'Pendiente' && (
            <Button
                variant="outline"
                size="sm"
                className="border-red-400/50 text-red-400 hover:bg-red-900/20 hover:text-red-300"
                onClick={() => onRetirar?.(propuesta.id)}
            >
                Retirar propuesta
            </Button>
        )}
        {propuesta.estadoPropuestaNombre === 'Aceptada' && propuesta.acuerdoId && (
            <Button
                size="sm"
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 flex items-center gap-1"
            >
                Ver acuerdo
                <ArrowRight className="w-3 h-3" aria-hidden="true" />
            </Button>
        )}
    </div>
</Card>
```

---

### 4.8 Componente Reutilizable: PropuestaCardSkeleton

```tsx
<Card className="bg-[#0f1729] border-[#334155] rounded-xl p-5">
    <div role="status" aria-label="Cargando propuesta...">
        <div className="flex items-start justify-between mb-3">
            <div className="flex-1">
                <Skeleton className="h-5 w-3/4 mb-2" />
                <Skeleton className="h-4 w-40" />
            </div>
            <Skeleton className="h-6 w-24 rounded-full flex-shrink-0" />
        </div>
        <div className="space-y-2 mb-4">
            <Skeleton className="h-4 w-28" />
            <Skeleton className="h-4 w-36" />
        </div>
        <div className="flex justify-end">
            <Skeleton className="h-8 w-32" />
        </div>
    </div>
</Card>
```

---

### 4.9 Pantalla: Confirmar Retirar Propuesta (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/propuestas/presentation/components/RetirarPropuestaDialog.tsx`
**Contexto:** Se abre desde el boton "Retirar propuesta" en PropuestaCard

#### Layout del Dialog

```
┌──────────────────────────────────────────────────────┐
│  "Retirar propuesta"                           [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  [Alert destructivo: Esta accion no se puede        │
│   deshacer]                                          │
│                                                      │
│  <p> Vas a retirar tu propuesta para:               │
│  <p> "{necesidadTitulo}"                            │
│  <p> Una vez retirada, el artista ya no podra       │
│      ver ni aceptar tu propuesta...                 │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar - outline]    [Confirmar retirada - red]  │
└──────────────────────────────────────────────────────┘
```

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Dialog | `Dialog` shadcn | `open={isOpen} onOpenChange={onClose}` |
| DialogContent | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155]` |
| DialogTitle | `DialogTitle` | `text-xl font-semibold text-white` |
| Warning Alert | `Alert` shadcn custom | `bg-red-900/20 border-red-700/50 mb-4` |
| Warning icon | `AlertTriangle` Lucide | `w-5 h-5 text-red-400 flex-shrink-0` |
| AlertDescription | `AlertDescription` | `text-sm text-red-300` |
| Description lead | `<p>` | `text-sm text-[#94a3b8]` |
| Necesidad titulo | `<p>` | `text-sm font-semibold text-white` |
| Explanation | `<p>` | `text-sm text-[#94a3b8]` |
| DialogFooter | `DialogFooter` | `flex justify-between gap-3` |
| Cancelar button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Confirmar button | `Button` | `bg-red-600 hover:bg-red-700 text-white` |
| Confirmar loading | `Button disabled` | con `Loader2 animate-spin w-4 h-4 mr-2` + "Retirando..." |

**Composicion completa Dialog de Confirmacion:**
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen}>
    <DialogContent
        className="max-w-md bg-[#0f1729] border-[#334155]"
        aria-labelledby="retirar-dialog-title"
        aria-describedby="retirar-dialog-desc"
    >
        <DialogHeader>
            <DialogTitle id="retirar-dialog-title" className="text-xl font-semibold text-white">
                Retirar propuesta
            </DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-2">
            <Alert className="bg-red-900/20 border-red-700/50">
                <AlertTriangle className="w-5 h-5 text-red-400" aria-hidden="true" />
                <AlertDescription className="text-sm text-red-300">
                    Esta accion no se puede deshacer
                </AlertDescription>
            </Alert>

            <div id="retirar-dialog-desc" className="space-y-3">
                <p className="text-sm text-[#94a3b8]">Vas a retirar tu propuesta para:</p>
                <p className="text-sm font-semibold text-white">"{necesidadTitulo}"</p>
                <p className="text-sm text-[#94a3b8]">
                    Una vez retirada, el artista ya no podra ver ni aceptar tu propuesta.
                    Podras enviar una nueva propuesta si lo deseas.
                </p>
            </div>
        </div>

        <DialogFooter className="flex justify-between gap-3">
            <Button
                variant="outline"
                className="border-[#334155] text-white hover:bg-[#1e2a42]"
                onClick={onClose}
                disabled={isPending}
            >
                Cancelar
            </Button>
            <Button
                className="bg-red-600 hover:bg-red-700 text-white"
                onClick={onConfirm}
                disabled={isPending}
                aria-busy={isPending}
                aria-label={isPending ? "Retirando propuesta..." : "Confirmar retirada de propuesta"}
            >
                {isPending && <Loader2 className="animate-spin w-4 h-4 mr-2" aria-hidden="true" />}
                {isPending ? "Retirando..." : "Confirmar retirada"}
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

## 5. Formularios

### 5.1 EnviarPropuestaForm (dentro del Dialog)

**Campos:**

| Campo | Componente shadcn | Tipo | Obligatorio | Validacion Visual |
|-------|-------------------|------|-------------|-------------------|
| precioPropuesto | `Input` + `Label` | number | Si | `FormMessage` con error rojo en blur |
| monedaId | `Select` + `Label` | select | Si | `FormMessage` con error rojo |
| diasEstimados | `Input` + `Label` | number | No | `FormMessage` con error si fuera de rango |
| mensajePropuesta | `Textarea` + `Label` | textarea | Si | `FormMessage` + CharacterCounter |

**Layout:**
- Stack vertical `space-y-5`
- Labels encima del input con asterisco en obligatorios
- Errores debajo del input con `role="alert"`
- Precio + Moneda en grid 2 columnas

**Estados de inputs:**

| Estado | Clases Tailwind |
|--------|----------------|
| Default | `border-[#334155]` |
| Focus | `focus:border-[#a855f7] focus:ring-[#a855f7]` |
| Error (blur) | `border-red-500 focus:border-red-500` |
| Disabled (loading) | `opacity-50 cursor-not-allowed` |

**Logica BudgetRangeInfo:**
- Se muestra solo si `precioPropuesto > 0`
- Se actualiza en tiempo real con cada cambio del campo precio
- Desaparece con `precio === 0` o vacio
- Transicion: `transition-opacity duration-200`

---

## 6. Estados de UI

### 6.1 Loading States

| Pantalla | Implementacion |
|----------|----------------|
| Listado Necesidades | `Array.from({ length: 6 }).map(() => <NecesidadCardSkeleton />)` en grid 2 cols |
| Detalle Necesidad | 3 `Card` con `Skeleton` simulando header, description y details |
| Mis Propuestas | `Array.from({ length: 4 }).map(() => <PropuestaCardSkeleton />)` en lista vertical |
| Sidebar Action Detalle | Card con `Skeleton` simulando presupuesto y stats |

Todos los contenedores de skeleton llevan `role="status" aria-label="Cargando..."`.

### 6.2 Error States

**Componente de Error Generico:**

| Estado | Icono | Clases contenedor | Boton CTA |
|--------|-------|-------------------|-----------|
| Error de red | `WifiOff` Lucide | `text-center py-12` | `Button variant="outline"` "Reintentar" |
| 404 Not Found | `SearchX` | `text-center py-12` | `Button variant="outline"` "Volver al listado" |
| Forbidden / Sin auth | `Lock` | `text-center py-12` | `Button gradient` "Iniciar sesion" |

```tsx
<div role="status" className="text-center py-12">
    <WifiOff className="w-12 h-12 text-[#334155] mx-auto mb-4" aria-hidden="true" />
    <h3 className="text-lg font-semibold text-white mb-2">Error de conexion</h3>
    <p className="text-sm text-[#94a3b8] mb-6">No se pudo cargar el contenido</p>
    <Button variant="outline" className="border-[#334155] text-white hover:bg-[#1e2a42]">
        Reintentar
    </Button>
</div>
```

### 6.3 Empty States

| Pantalla / Contexto | Icono | Titulo | CTA |
|--------------------|-------|--------|-----|
| Listado sin filtros activos | `Inbox` (w-12 text-[#334155]) | "Sin necesidades abiertas" | - |
| Listado con filtros activos | `SearchX` | "Sin resultados" | Button "Limpiar filtros" |
| Mis Propuestas (ninguna) | `FileText` | "Sin propuestas enviadas" | Button gradient "Explorar necesidades" |
| Mis Propuestas (filtro activo) | `Filter` | "Sin propuestas {estado}" | - |

Todos llevan `role="status"` en el contenedor.

### 6.4 Necesidad Expirada (Banner)

```tsx
<Alert className="bg-amber-900/20 border-amber-700/50 mb-6">
    <AlertTriangle className="w-5 h-5 text-amber-400" aria-hidden="true" />
    <AlertDescription className="text-sm text-amber-300">
        Esta necesidad ya no admite propuestas. La fecha limite ha sido superada.
    </AlertDescription>
</Alert>
```

### 6.5 Toast Notifications

Usar `sonner` (ya configurado en el landing). Mapeo de acciones a tipos de toast:

| Accion | Tipo | Clase de sonner |
|--------|------|-----------------|
| Propuesta enviada OK | success | `toast.success(...)` |
| Propuesta ya enviada | error | `toast.error(...)` |
| Sin perfil profesional | warning | `toast.warning(...)` |
| Propietario de necesidad | warning | `toast.warning(...)` |
| Error general API | error | `toast.error(...)` |
| Propuesta retirada OK | success | `toast.success(...)` |
| Error al retirar | error | `toast.error(...)` |
| Error carga de datos | error | `toast.error(...)` |

---

## 7. Responsive Design

| Breakpoint | Width | Cambios clave |
|------------|-------|---------------|
| Mobile | < 640px | Filtros en `Collapsible`, grid 1 col, CTA sticky bottom en detalle, filter chips con `overflow-x-auto` |
| Tablet | 640px - 1024px | Grid 2 cols cards, sidebar accion visible (no fixed bottom), filtros sidebar visible |
| Desktop | > 1024px | Sidebar filtros fijo izquierda (w-64), grid 2 cols en content area, sidebar accion sticky (w-80) |

**Clases responsive clave:**

| Elemento | Clases Responsive |
|----------|------------------|
| Cards grid | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Page layout | `flex flex-col lg:flex-row gap-8` |
| Sidebar filtros | `hidden lg:block` |
| Sidebar accion | `hidden md:block` |
| Mobile CTA sticky | `fixed bottom-0 left-0 right-0 md:hidden` |
| Filter chips mobile | `flex flex-nowrap overflow-x-auto` (mobile) / `flex flex-wrap` (desktop) |
| Details grid | `grid grid-cols-1 sm:grid-cols-2 gap-4` |
| Dialog max-width | `max-w-lg` (adapta automaticamente en mobile a full-width) |

---

## 8. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Focus visible | `focus:ring-2 focus:ring-[#a855f7] focus:ring-offset-2 focus:ring-offset-[#1a1a2e]` en todos los elementos interactivos |
| Labels en inputs | `Label` con `htmlFor` apuntando al `id` del `Input`/`Select`/`Textarea` |
| Required fields | `aria-required="true"` en el input + asterisco visual con `<span aria-hidden="true">` |
| Errores form | `role="alert"` en `<p>` de error + `aria-describedby` en el input |
| Loading states | `aria-busy="true"` en botones durante submit. Skeleton con `role="status" aria-label="Cargando..."` |
| Cards navegables | `tabIndex={0}`, `role="article"`, manejo de `onKeyDown` para Enter/Space |
| Estado badges | `aria-label="Estado: {estado}"` |
| Dialog | `aria-labelledby`, `aria-describedby`, focus trap nativo de Radix |
| Fecha limite urgente | `aria-label="Fecha limite: {fecha}. {N} dias restantes."` |
| Contador caracteres | `aria-live="polite" aria-atomic="true"` |
| Boton loading | `aria-busy={isPending} aria-label={isPending ? "Accion en progreso..." : "Accion"}` |
| Filter chips | `aria-pressed="true/false"` |
| Paginacion | `aria-label="Paginacion"` en nav, `aria-current="page"` en pagina activa |
| Empty states | `role="status"` en el contenedor |
| Avatares | `alt="{nombre} avatar"` en AvatarImage |
| Iconos decorativos | `aria-hidden="true"` en todos los iconos de Lucide que son decorativos |
| Contraste | Todos los textos verificados: white (#fff) sobre #0f1729 = ~15:1 (WCAG AAA). amber-300 sobre #0f1729 verificar >= 4.5:1 |

---

## 9. Animaciones

Las animaciones estan definidas en `tailwind.config.ts` (requieren agregar keyframes listados en seccion 3):

| Elemento | Clase Tailwind | Duracion |
|----------|---------------|----------|
| NecesidadCard hover scale | `hover:scale-[1.01]` | 200ms via `transition-all duration-200 ease-out` |
| Badge urgencia pulse | `animate-urgency-pulse` | 2000ms infinite |
| Error input shake | `animate-shake` | 400ms (aplicar cuando hay error de validacion) |
| Dialog open/close | Manejado automaticamente por Radix/shadcn con `tailwindcss-animate` | 200ms |
| Button spinner | `animate-spin` (clase de Tailwind built-in) | Infinite |
| Skeleton shimmer | `animate-pulse` (clase de Tailwind built-in) | Infinite |
| BudgetRangeInfo appear | `transition-opacity duration-200` | 200ms |
| Filter chip toggle | `transition-all duration-150` | 150ms |
| Input focus glow | `transition-colors duration-200` | 200ms |

---

## 10. Estructura de Archivos Sugerida

```
src/web/src/features/crowdsourcing/
├── necesidades/
│   ├── presentation/
│   │   ├── pages/
│   │   │   └── NecesidadesPublicasPage.tsx
│   │   │   └── NecesidadPublicaDetailPage.tsx
│   │   └── components/
│   │       ├── NecesidadCard.tsx
│   │       ├── NecesidadCardSkeleton.tsx
│   │       └── FiltrosSidebar.tsx
├── propuestas/
│   ├── presentation/
│   │   ├── pages/
│   │   │   └── MisPropuestasPage.tsx
│   │   └── components/
│   │       ├── PropuestaCard.tsx
│   │       ├── PropuestaCardSkeleton.tsx
│   │       ├── EnviarPropuestaDialog.tsx
│   │       └── RetirarPropuestaDialog.tsx
└── shared/
    └── components/
        ├── EstadoFilterChip.tsx
        ├── BudgetRangeInfo.tsx
        ├── CharacterCounter.tsx
        └── UrgenciaBadge.tsx
```

---

## 11. Checklist

- [ ] Componentes shadcn instalados: Pagination, Collapsible (ejecutar comandos de instalacion)
- [ ] NecesidadCard: todos los elementos visuales, hover effects, aria-label completo
- [ ] NecesidadCardSkeleton: shimmer en 2 lineas descripcion + badges + stats
- [ ] PropuestaCard: badge de estado con color correcto, acciones condicionales por estado
- [ ] PropuestaCardSkeleton: estructura correcta con badge y boton
- [ ] FiltrosSidebar: chips toggle con aria-pressed, Select modalidad, inputs presupuesto, pais, ciudad condicional, limpiar
- [ ] Collapsible mobile filtros con trigger SlidersHorizontal
- [ ] EstadoFilterChip: 5 estados con estilos correctos + aria-pressed
- [ ] BudgetRangeInfo: 3 variantes (dentro/encima/debajo) con fade-in
- [ ] CharacterCounter: color progresivo (gris -> amber -> rojo) + aria-live
- [ ] UrgenciaBadge: animate-urgency-pulse con aria-label
- [ ] EnviarPropuestaDialog: React Hook Form + Zod, validacion blur, loading state, focus trap
- [ ] RetirarPropuestaDialog: Alert destructivo, boton rojo, loading state, aria-modal
- [ ] CTA detalle: 5 estados (sin auth, sin perfil, con perfil, ya propuso, propietario)
- [ ] Mobile CTA sticky bottom en detalle (fixed bottom-0 md:hidden)
- [ ] Loading states con role="status" aria-label en todos los skeletons
- [ ] Empty states: 4 variantes con iconos Lucide apropiados y role="status"
- [ ] Error states: 3 variantes (red, 404, forbidden)
- [ ] Banner necesidad expirada: Alert amber
- [ ] Toast notifications: mapeo completo de acciones a tipos
- [ ] Keyframes urgency-pulse y shake agregados a tailwind.config.ts
- [ ] Focus visible ring purple en todos los elementos interactivos
- [ ] aria-hidden="true" en todos los iconos Lucide decorativos
- [ ] Contraste verificado para textos amber/green/red sobre fondos oscuros
- [ ] Responsive: mobile 1 col, tablet 2 cols, desktop sidebar + 2 cols
- [ ] Dialog max-w-lg adaptado a mobile con full-width
- [ ] URL sync filtros con useSearchParams (React Router)
- [ ] Dark theme consistente con TemplateCard.tsx existente
