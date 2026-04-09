# UI/UX: Dashboard de Artista

> **Feature:** dashboard-artista (US-05)
> **Ultima actualizacion:** 2026-02-14

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Dashboard Principal | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Dashtail** | Dashboard, panel artista, stats cards, charts | `references/templates/dashtail/` |

---

## Design Tokens

### Paleta de Colores (Extraida de Mockup)

```css
:root {
  /* Background - Dark Theme */
  --bg-primary: #0a0b14;        /* Fondo principal oscuro */
  --bg-secondary: #1a1d2e;      /* Cards y superficies elevadas */
  --bg-tertiary: #252837;       /* Hover states */
  --bg-sidebar: #0f1117;        /* Sidebar background */

  /* Primary - Pink/Purple Gradient */
  --primary-pink: #e91e8c;      /* Rosa principal */
  --primary-purple: #a855f7;    /* Morado principal */
  --primary-gradient: linear-gradient(135deg, #e91e8c 0%, #a855f7 100%);

  /* Accent Colors */
  --accent-green: #10b981;      /* Success, stats positivos */
  --accent-cyan: #06b6d4;       /* Stats secundarios */
  --accent-blue: #3b82f6;       /* Info */
  --accent-orange: #f97316;     /* Warning */
  --accent-red: #ef4444;        /* Error */

  /* Text */
  --text-primary: #ffffff;      /* Titulos, texto principal */
  --text-secondary: #94a3b8;    /* Subtitulos, descriptions */
  --text-muted: #64748b;        /* Labels, placeholders */

  /* Status Colors (para badges) */
  --status-active: #10b981;     /* Campana activa */
  --status-draft: #eab308;      /* Borrador */
  --status-finalized: #3b82f6;  /* Finalizada */
  --status-canceled: #ef4444;   /* Cancelada */

  /* Borders */
  --border-default: #252837;
  --border-hover: #3b3f54;
}
```

### Tipografia

```css
--font-sans: 'Inter', system-ui, -apple-system, sans-serif;

/* Font Sizes */
--text-xs: 0.75rem;    /* 12px - timestamps, badges */
--text-sm: 0.875rem;   /* 14px - descriptions */
--text-base: 1rem;     /* 16px - body text */
--text-lg: 1.125rem;   /* 18px - card titles */
--text-xl: 1.25rem;    /* 20px - section headers */
--text-2xl: 1.5rem;    /* 24px - page title */
--text-3xl: 1.875rem;  /* 30px - stat values */

/* Font Weights */
--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
```

### Espaciado

```css
--space-1: 0.25rem;   /* 4px */
--space-2: 0.5rem;    /* 8px */
--space-3: 0.75rem;   /* 12px */
--space-4: 1rem;      /* 16px */
--space-5: 1.25rem;   /* 20px */
--space-6: 1.5rem;    /* 24px */
--space-8: 2rem;      /* 32px */
--space-10: 2.5rem;   /* 40px */
--space-12: 3rem;     /* 48px */
```

### Shadows

```css
--shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.3);
--shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.4);
--shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.5);
--shadow-xl: 0 20px 25px -5px rgba(0, 0, 0, 0.6);
```

---

## Pantalla 1: Dashboard Principal (Enhanced)

**Mockup:** WPR_5-Dashboard-Artist.png
**Proyecto:** Admin
**Ruta:** `/dashboard`
**Template base:** `dashtail/pages/dashboard.html`

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (250px)  │  HEADER                          [+ Nueva]      │
│                  ├────────────────────────────────────────────────┤
│ WePlay Rises     │  Hola, {Nombre Artista}                        │
│ [Avatar]         │  {Fecha actual}                                │
│ {Nombre}         │                                                │
│ Artista          │  ┌──────────────────────────────────────────┐  │
│                  │  │ CompleteProfileBanner (if no artista)    │  │
│ ─ Dashboard      │  └──────────────────────────────────────────┘  │
│ ─ Mis Campañas   │                                                │
│ ─ Crear Campaña  │  STATS CARDS (Grid 4 cols)                    │
│ ─ Mis Backers    │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐             │
│ ─ Configuración  │  │$    │ │👥   │ │🎵   │ │📊   │             │
│                  │  │24.5K│ │1,247│ │  3  │ │ 78% │             │
│ ─ Ver perfil     │  │Total│ │Back.│ │Camp.│ │Éxito│             │
│ ─ Logout         │  │+12% │ │+8.2%│ │Activ│ │+5.1%│             │
│                  │  └─────┘ └─────┘ └─────┘ └─────┘             │
│                  │                                                │
│                  │  CHART: Recaudación últimos 30 días           │
│                  │  ┌───────────────────────────────────────────┐│
│                  │  │ [Line chart mostrando tendencia creciente]││
│                  │  │ $0 ──────────────────────> $6,000         ││
│                  │  │    1 Ene        11 Ene        21 Ene      ││
│                  │  └───────────────────────────────────────────┘│
│                  │                                                │
│                  │  ┌──────────────────┐  ┌──────────────────┐   │
│                  │  │ MIS CAMPAÑAS     │  │ ÚLTIMOS BACKINGS │   │
│                  │  │ [Ver todas]      │  │ [Ver todos]      │   │
│                  │  │                  │  │                  │   │
│                  │  │ ┌──────────────┐ │  │ ┌──────────────┐ │   │
│                  │  │ │[IMG] Midnight│ │  │ │[👤] María G. │ │   │
│                  │  │ │ Activa 85%   │ │  │ │ Vinyl  $45   │ │   │
│                  │  │ │ 420 backers  │ │  │ │ Hace 2h      │ │   │
│                  │  │ └──────────────┘ │  │ └──────────────┘ │   │
│                  │  │ ┌──────────────┐ │  │ ┌──────────────┐ │   │
│                  │  │ │[IMG] EP Acús.│ │  │ │[👤] Carlos R │ │   │
│                  │  │ │ Borrador 0%  │ │  │ │ Digital $15  │ │   │
│                  │  │ │ 0 backers    │ │  │ │ Hace 5h      │ │   │
│                  │  │ └──────────────┘ │  │ └──────────────┘ │   │
│                  │  └──────────────────┘  └──────────────────┘   │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones de Componentes

#### 1. Header Section

| Elemento | Componente | Estilos/Props | Datos |
|----------|------------|---------------|-------|
| Saludo | `<h1>` | `text-3xl font-bold text-white` | `Hola, {artista.nombreArtistico}` |
| Fecha | `<p>` | `text-sm text-muted-foreground` | `formatDate(new Date())` |
| Botón Nueva | `<Button>` | `variant="default" className="bg-gradient-to-r from-pink-500 to-purple-600"` | Link a `/campanias/nueva` |

#### 2. Stats Cards (Grid)

**Container:**
```tsx
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
```

**Componente:** `<StatsCard>` (ALREADY EXISTS)

**Props Interface:**
```tsx
interface StatsCardProps {
  title: string
  value: string
  icon: LucideIcon
  description?: string
  trend?: {
    value: number
    isPositive: boolean
  }
  className?: string
}
```

**Cards Specification:**

| Card | title | value | icon | trend | datos |
|------|-------|-------|------|-------|-------|
| Total Recaudado | "Total Recaudado" | `formatCurrency(stats.totalRecaudado)` | `TrendingUp` | `{ value: 12, isPositive: true }` | API: Suma de `campanias[].importePledgedActual` |
| Backers Totales | "Backers Totales" | `stats.totalBackers.toString()` | `Users` | `{ value: 8.2, isPositive: true }` | API: GET /api/stats/backers (NEW endpoint needed) |
| Campañas Activas | "Campañas Activas" | `stats.campaniaActivas.toString()` | `Music` | - | Filtrar `campanias` donde `estadoCampaniaId === 2` |
| Tasa de Éxito | "Tasa de Éxito" | `stats.tasaExito + "%"` | `BarChart3` | `{ value: 5.1, isPositive: true }` | Cálculo: `(campanias finalizadas con meta alcanzada / total finalizadas) * 100` |

**Tailwind Classes:**
```tsx
// Card container (shadcn Card component)
<Card className="bg-secondary border-border hover:border-primary/50 transition-colors">

// Title
<CardTitle className="text-sm font-medium text-muted-foreground">

// Value
<div className="text-2xl font-bold text-white">

// Trend (positive)
<p className="text-xs text-green-600">+{trend.value}% desde el mes pasado</p>

// Trend (negative)
<p className="text-xs text-red-600">-{trend.value}% desde el mes pasado</p>

// Icon
<Icon className="h-4 w-4 text-muted-foreground" />
```

#### 3. Chart: Recaudación últimos 30 días

**Componente:** `<Card>` con chart library (recharts o chart.js)

**Para MVP:** Placeholder con mensaje "Próximamente" o datos mockeados

**Estructura:**
```tsx
<Card className="lg:col-span-4">
  <CardHeader>
    <CardTitle>Recaudación últimos 30 días</CardTitle>
  </CardHeader>
  <CardContent>
    {/* Line chart aquí */}
    <div className="h-[300px] flex items-center justify-center text-muted-foreground">
      Gráfica próximamente
    </div>
  </CardContent>
</Card>
```

**Chart Config (future):**
```tsx
// Datos de ejemplo del mockup
const chartData = [
  { date: '1 Ene', amount: 1800 },
  { date: '3 Ene', amount: 2200 },
  { date: '5 Ene', amount: 2600 },
  // ...
  { date: '21 Ene', amount: 6000 },
]

// Color: Purple gradient (#a855f7)
```

#### 4. Mis Campañas Card (Enhanced)

**Container:**
```tsx
<Card className="lg:col-span-1">
  <CardHeader className="flex flex-row items-center justify-between">
    <CardTitle>Mis Campañas</CardTitle>
    <Link href="/campanias">
      <Button variant="ghost" size="sm">Ver todas</Button>
    </Link>
  </CardHeader>
  <CardContent>
```

**Campaña Item (Enhanced):**

| Elemento | Componente | Estilos | Datos |
|----------|------------|---------|-------|
| Container | `<div>` | `rounded-lg border border-border bg-card/50 p-4 hover:bg-card transition-colors` | - |
| Imagen | `<div>` con bg color | `w-12 h-12 rounded-lg bg-gradient-to-br` (color por estado) | Color según estado |
| Título | `<p>` | `font-medium text-white truncate` | `campania.titulo` |
| Estado Badge | `<Badge>` | `variant según estado` | Ver tabla de estados abajo |
| Porcentaje | `<span>` | `text-sm font-medium` con color según valor | `calculatePercentage(pledged, objetivo)` |
| Backers | `<span>` | `text-xs text-muted-foreground` | `{count} backers` |

**Estado Badges:**

| Estado | Variant | Color Classes | Label |
|--------|---------|---------------|-------|
| Publicada (2) | - | `bg-green-500/20 text-green-400 border-green-500/30` | "Activa" |
| Borrador (1) | - | `bg-yellow-500/20 text-yellow-400 border-yellow-500/30` | "Borrador" |
| Finalizada (3) | - | `bg-blue-500/20 text-blue-400 border-blue-500/30` | "Finalizada" |
| Cancelada (4) | - | `bg-red-500/20 text-red-400 border-red-500/30` | "Cancelada" |

**Imagen Color por Estado:**
```tsx
// Borrador: Orange gradient
bg-gradient-to-br from-orange-500 to-orange-600

// Activa: Pink gradient
bg-gradient-to-br from-pink-500 to-purple-600

// Finalizada: Blue gradient
bg-gradient-to-br from-blue-500 to-cyan-600

// Cancelada: Gray
bg-gradient-to-br from-gray-600 to-gray-700
```

#### 5. Últimos Backings Card (Enhanced)

**Componente:** `<RecentBackings>` (ALREADY EXISTS, needs enhancement)

**Enhancement needed:**
- Show backer avatar (or default icon)
- Show reward name below backer name
- Relative time formatting
- Link to full backings list

**Backing Item Structure:**

| Elemento | Componente | Estilos | Datos |
|----------|------------|---------|-------|
| Avatar | `<Avatar>` (shadcn) | `h-10 w-10` | User avatar o fallback |
| Nombre | `<p>` | `text-sm font-medium text-white` | `userName` o "Anónimo" |
| Reward | `<p>` | `text-xs text-muted-foreground` | `rewardNombre` |
| Monto | `<p>` | `text-sm font-medium text-white` | `formatCurrency(monto)` |
| Tiempo | `<p>` | `text-xs text-muted-foreground` | `formatRelativeDate(createdAt)` |

### Estados de UI

#### Loading State

```tsx
// Stats cards skeleton
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
  {[1, 2, 3, 4].map((i) => (
    <Skeleton key={i} className="h-32 rounded-lg" />
  ))}
</div>

// Campañas skeleton
<div className="space-y-4">
  {[1, 2, 3].map((i) => (
    <Skeleton key={i} className="h-20 rounded-lg" />
  ))}
</div>
```

#### Empty State

**No hay campañas:**
```tsx
<div className="text-center py-12">
  <Music className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
  <p className="text-lg font-medium text-white mb-2">
    No tienes campañas aún
  </p>
  <p className="text-sm text-muted-foreground mb-6">
    Crea tu primera campaña y empieza a recaudar fondos
  </p>
  <Link href="/campanias/nueva">
    <Button className="bg-gradient-to-r from-pink-500 to-purple-600">
      <PlusCircle className="mr-2 h-4 w-4" />
      Crear campaña
    </Button>
  </Link>
</div>
```

#### Error State

```tsx
<Alert variant="destructive">
  <AlertCircle className="h-4 w-4" />
  <AlertTitle>Error al cargar datos</AlertTitle>
  <AlertDescription>
    No pudimos cargar tu dashboard. Por favor, intenta nuevamente.
  </AlertDescription>
</Alert>
```

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| Click en "Nueva campaña" | Navigate to `/campanias/nueva` |
| Click en "Ver todas" (campañas) | Navigate to `/campanias` |
| Click en campaña item | Navigate to `/campanias/[id]` (detail view) |
| Click en "Ver todos" (backings) | Navigate to `/backings` o mostrar modal |
| Hover en stat card | Border cambia a `border-primary/50`, transition 200ms |
| Hover en campaña item | Background cambia a `bg-card`, transition 200ms |

---

## Pantalla 2: Campaña Detail (Stats View)

**Proyecto:** Admin
**Ruta:** `/campanias/[id]`
**Template base:** `dashtail/pages/analytics.html`

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ SIDEBAR  │  ← Volver       {Título de Campaña}                     │
│          ├────────────────────────────────────────────────────────┤
│          │                                                         │
│          │  STATS OVERVIEW (Grid 4 cols)                          │
│          │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐      │
│          │  │ $1,250  │ │   15    │ │ 45 días │ │  $83    │      │
│          │  │Recaudado│ │ Backers │ │Restantes│ │Promedio │      │
│          │  └─────────┘ └─────────┘ └─────────┘ └─────────┘      │
│          │                                                         │
│          │  PROGRESO DE META                                      │
│          │  ┌─────────────────────────────────────────────────┐   │
│          │  │ [===========================>    ] 25%          │   │
│          │  │ $1,250 / $5,000                                 │   │
│          │  └─────────────────────────────────────────────────┘   │
│          │                                                         │
│          │  ┌──────────────────────────────────────────────────┐  │
│          │  │ BACKINGS RECIENTES               [Ver página    │  │
│          │  │                                   pública] [Edit]│  │
│          │  │ ┌─────────────────────────────────────────────┐ │  │
│          │  │ │ [👤] Juan G.   │ CD Firmado │ $25 │ 2h ago  │ │  │
│          │  │ │ [👤] Anónimo   │ Digital    │ $10 │ 5h ago  │ │  │
│          │  │ │ [👤] María L.  │ Vinilo     │ $50 │ 1d ago  │ │  │
│          │  │ └─────────────────────────────────────────────┘ │  │
│          │  │                                                  │  │
│          │  │ [Ver todos los backers]                          │  │
│          │  └──────────────────────────────────────────────────┘  │
│          │                                                         │
│          │  ┌──────────────────────────────────────────────────┐  │
│          │  │ REWARDS STATS                                    │  │
│          │  │                                                  │  │
│          │  │ CD Firmado       ██████████░░░░░  15 vendidos   │  │
│          │  │ Digital          ████░░░░░░░░░░░   5 vendidos   │  │
│          │  │ Vinilo           ████████░░░░░░░  10 vendidos   │  │
│          │  └──────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones de Componentes

#### 1. Header con Navegación

| Elemento | Componente | Estilos | Comportamiento |
|----------|------------|---------|----------------|
| Botón Volver | `<Button>` | `variant="ghost" size="sm"` con `<ArrowLeft>` icon | Navigate back o to `/campanias` |
| Título | `<h1>` | `text-3xl font-bold text-white` | `campania.titulo` |
| Actions Group | `<div>` flex gap | `flex items-center gap-2` | - |
| Ver Público | `<Button>` | `variant="outline"` con `<ExternalLink>` icon | Open landing URL en new tab |
| Editar | `<Button>` | `variant="default"` con `<Edit>` icon | Navigate to `/campanias/[id]/editar` |

#### 2. Stats Overview Cards

**Container:**
```tsx
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
```

**Cards:**

| Card | title | value | icon | datos |
|------|-------|-------|------|-------|
| Recaudado | "Recaudado" | `formatCurrency(campania.importePledgedActual)` | `DollarSign` | `campania.importePledgedActual` |
| Backers | "Backers" | `stats.totalBackers.toString()` | `Users` | API: GET `/api/campanias/[id]/stats` |
| Días Restantes | "Días Restantes" | `stats.diasRestantes.toString()` | `Calendar` | `getDaysRemaining(campania.fechaFin)` |
| Backing Promedio | "Backing Promedio" | `formatCurrency(stats.promedioAporte)` | `TrendingUp` | `totalRecaudado / totalBackers` |

#### 3. Progress Bar (Full Width)

**Componente:**
```tsx
<Card>
  <CardHeader>
    <CardTitle>Progreso de Meta</CardTitle>
  </CardHeader>
  <CardContent className="space-y-4">
    <div className="space-y-2">
      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">
          {formatCurrency(importePledgedActual)} de {formatCurrency(importeObjetivo)}
        </span>
        <span className="font-semibold text-white">
          {porcentaje}%
        </span>
      </div>
      <Progress value={porcentaje} className="h-3" />
    </div>

    {/* Meta alcanzada badge */}
    {porcentaje >= 100 && (
      <Badge variant="success" className="bg-green-500/20 text-green-400">
        <CheckCircle className="mr-1 h-3 w-3" />
        Meta alcanzada
      </Badge>
    )}
  </CardContent>
</Card>
```

**Progress Component Styling:**
```tsx
// Progress bar container
<Progress className="h-3 bg-muted/20" />

// Progress fill (gradient)
// En globals.css o component styles
.progress-bar {
  background: linear-gradient(135deg, #e91e8c 0%, #a855f7 100%);
}
```

#### 4. Backings Recientes Table

**Componente:** `<Table>` (shadcn)

**Estructura:**
```tsx
<Card>
  <CardHeader className="flex flex-row items-center justify-between">
    <CardTitle>Backings Recientes</CardTitle>
    <div className="flex gap-2">
      <Link href={`/campanias/${id}/public`} target="_blank">
        <Button variant="outline" size="sm">
          <ExternalLink className="mr-2 h-4 w-4" />
          Ver página pública
        </Button>
      </Link>
      <Link href={`/campanias/${id}/editar`}>
        <Button variant="outline" size="sm">
          <Edit className="mr-2 h-4 w-4" />
          Editar
        </Button>
      </Link>
    </div>
  </CardHeader>
  <CardContent>
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Backer</TableHead>
          <TableHead>Reward</TableHead>
          <TableHead>Monto</TableHead>
          <TableHead className="text-right">Fecha</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {backings.map((backing) => (
          <TableRow key={backing.id}>
            <TableCell>
              <div className="flex items-center gap-2">
                <Avatar className="h-8 w-8">
                  <AvatarFallback>{backing.nombreBacker[0]}</AvatarFallback>
                </Avatar>
                <span className="font-medium">{backing.nombreBacker}</span>
              </div>
            </TableCell>
            <TableCell className="text-muted-foreground">
              {backing.rewardNombre || "Sin recompensa"}
            </TableCell>
            <TableCell className="font-medium">
              {formatCurrency(backing.monto)}
            </TableCell>
            <TableCell className="text-right text-muted-foreground">
              {formatRelativeDate(backing.fechaCreacion)}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>

    <div className="mt-4 text-center">
      <Link href={`/campanias/${id}/backings`}>
        <Button variant="ghost">
          Ver todos los backers
          <ChevronRight className="ml-2 h-4 w-4" />
        </Button>
      </Link>
    </div>
  </CardContent>
</Card>
```

**Table Styling:**
```tsx
// TableRow hover
<TableRow className="hover:bg-muted/50 transition-colors">

// Avatar fallback
<AvatarFallback className="bg-primary/20 text-primary-foreground">
```

#### 5. Rewards Stats (Optional MVP)

**Para MVP:** Puede ser "Próximamente" o versión simple con lista

**Estructura simple:**
```tsx
<Card>
  <CardHeader>
    <CardTitle>Recompensas Más Populares</CardTitle>
  </CardHeader>
  <CardContent className="space-y-4">
    {rewardStats.map((reward) => (
      <div key={reward.id} className="space-y-2">
        <div className="flex items-center justify-between text-sm">
          <span className="font-medium text-white">{reward.nombre}</span>
          <span className="text-muted-foreground">
            {reward.vendidos} vendidos
          </span>
        </div>
        <Progress
          value={(reward.vendidos / totalBackers) * 100}
          className="h-2"
        />
      </div>
    ))}
  </CardContent>
</Card>
```

### Estados de UI

#### Loading State
```tsx
<div className="space-y-6">
  <Skeleton className="h-12 w-64" /> {/* Title */}
  <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
    {[1, 2, 3, 4].map((i) => (
      <Skeleton key={i} className="h-28 rounded-lg" />
    ))}
  </div>
  <Skeleton className="h-40 rounded-lg" /> {/* Progress */}
  <Skeleton className="h-96 rounded-lg" /> {/* Table */}
</div>
```

#### Empty State (No backings)
```tsx
<div className="text-center py-12">
  <Users className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
  <p className="text-lg font-medium text-white mb-2">
    Aún no hay backings
  </p>
  <p className="text-sm text-muted-foreground mb-6">
    Comparte tu campaña para empezar a recibir apoyo
  </p>
  <Button variant="outline">
    <Share2 className="mr-2 h-4 w-4" />
    Compartir campaña
  </Button>
</div>
```

#### Error State
```tsx
<Alert variant="destructive">
  <AlertCircle className="h-4 w-4" />
  <AlertTitle>Error al cargar estadísticas</AlertTitle>
  <AlertDescription>
    No pudimos cargar las estadísticas de tu campaña.
    <Button variant="link" className="p-0 h-auto" onClick={retry}>
      Intentar nuevamente
    </Button>
  </AlertDescription>
</Alert>
```

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| Click en "Volver" | Navigate to `/campanias` o browser back |
| Click en "Ver página pública" | Open `/campanias/[id]` (landing) en nueva pestaña |
| Click en "Editar" | Navigate to `/campanias/[id]/editar` |
| Click en "Ver todos los backers" | Navigate to `/campanias/[id]/backings` |
| Click en fila de tabla | Highlight row (no action por ahora) |
| Hover en stat card | Border glow effect |

---

## Pantalla 3: Backings List (Full)

**Proyecto:** Admin
**Ruta:** `/campanias/[id]/backings`
**Template base:** `dashtail/pages/tables.html`

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│ SIDEBAR  │  ← Volver       Backers: {Título Campaña}               │
│          ├────────────────────────────────────────────────────────┤
│          │                                                         │
│          │  STATS HEADER (Grid 3 cols)                            │
│          │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐      │
│          │  │  $1,250     │ │    $83      │ │ CD Firmado  │      │
│          │  │Total Recaud.│ │  Promedio   │ │Más Popular  │      │
│          │  └─────────────┘ └─────────────┘ └─────────────┘      │
│          │                                                         │
│          │  ┌──────────────────────────────────────────────────┐  │
│          │  │ FILTROS Y BÚSQUEDA                    [Export CSV]│  │
│          │  │ [Buscar...] [Todos] [Con Reward] [Anónimos]      │  │
│          │  └──────────────────────────────────────────────────┘  │
│          │                                                         │
│          │  BACKINGS TABLE                                        │
│          │  ┌──────────────────────────────────────────────────┐  │
│          │  │ Backer     │ Reward      │ Monto │ Mensaje│Fecha│  │
│          │  ├──────────────────────────────────────────────────┤  │
│          │  │ [👤] Juan G│ CD Firmado  │ $25   │ "..." │2h ago│  │
│          │  │ [👤] Anónimo│ Digital    │ $10   │   -   │5h ago│  │
│          │  │ [👤] María L│ Vinilo     │ $50   │ "..." │1d ago│  │
│          │  │ ...                                              │  │
│          │  └──────────────────────────────────────────────────┘  │
│          │                                                         │
│          │  [Anterior] Página 1 de 3 [Siguiente]                  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones de Componentes

#### 1. Header con Navegación

| Elemento | Componente | Estilos | Datos |
|----------|------------|---------|-------|
| Botón Volver | `<Button>` | `variant="ghost" size="sm"` | Navigate to `/campanias/[id]` |
| Título | `<h1>` | `text-3xl font-bold text-white` | `Backers: {campania.titulo}` |

#### 2. Stats Header

**Container:**
```tsx
<div className="grid gap-4 md:grid-cols-3 mb-6">
```

**Cards:**

| Card | title | value | icon |
|------|-------|-------|------|
| Total Recaudado | "Total Recaudado" | `formatCurrency(stats.totalRecaudado)` | `DollarSign` |
| Backing Promedio | "Backing Promedio" | `formatCurrency(stats.promedioAporte)` | `TrendingUp` |
| Reward Más Popular | "Reward Más Popular" | `stats.rewardMasPopular` | `Award` |

#### 3. Filtros y Búsqueda

**Componente:**
```tsx
<Card className="mb-6">
  <CardContent className="pt-6">
    <div className="flex flex-col md:flex-row gap-4">
      {/* Search input */}
      <div className="flex-1">
        <Input
          placeholder="Buscar por nombre o email..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="max-w-sm"
        />
      </div>

      {/* Filters */}
      <div className="flex gap-2">
        <Button
          variant={filter === 'all' ? 'default' : 'outline'}
          onClick={() => setFilter('all')}
          size="sm"
        >
          Todos
        </Button>
        <Button
          variant={filter === 'with-reward' ? 'default' : 'outline'}
          onClick={() => setFilter('with-reward')}
          size="sm"
        >
          Con Reward
        </Button>
        <Button
          variant={filter === 'anonymous' ? 'default' : 'outline'}
          onClick={() => setFilter('anonymous')}
          size="sm"
        >
          Anónimos
        </Button>
      </div>

      {/* Export button */}
      <Button variant="outline" size="sm">
        <Download className="mr-2 h-4 w-4" />
        Exportar CSV
      </Button>
    </div>
  </CardContent>
</Card>
```

#### 4. Backings Table (Full)

**Componente:** `<Table>` (shadcn)

**Columnas:**

| Columna | Ancho | Contenido | Ordenable |
|---------|-------|-----------|-----------|
| Backer | 25% | Avatar + Nombre | Sí |
| Reward | 25% | Nombre del reward o "Sin recompensa" | Sí |
| Monto | 15% | Cantidad formateada | Sí |
| Mensaje | 25% | Truncated a 50 chars con tooltip | No |
| Fecha | 10% | Relative date | Sí (default desc) |

**Estructura:**
```tsx
<Card>
  <CardContent className="p-0">
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[25%]">
            <Button variant="ghost" size="sm" onClick={() => handleSort('backer')}>
              Backer
              {sortBy === 'backer' && (
                <ChevronDown className="ml-2 h-4 w-4" />
              )}
            </Button>
          </TableHead>
          <TableHead className="w-[25%]">Reward</TableHead>
          <TableHead className="w-[15%]">
            <Button variant="ghost" size="sm" onClick={() => handleSort('monto')}>
              Monto
              {sortBy === 'monto' && (
                <ChevronDown className="ml-2 h-4 w-4" />
              )}
            </Button>
          </TableHead>
          <TableHead className="w-[25%]">Mensaje</TableHead>
          <TableHead className="w-[10%] text-right">
            <Button variant="ghost" size="sm" onClick={() => handleSort('fecha')}>
              Fecha
              {sortBy === 'fecha' && (
                <ChevronDown className="ml-2 h-4 w-4" />
              )}
            </Button>
          </TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {backings.map((backing) => (
          <TableRow key={backing.id} className="hover:bg-muted/50">
            <TableCell>
              <div className="flex items-center gap-3">
                <Avatar className="h-10 w-10">
                  <AvatarFallback className="bg-primary/20 text-primary">
                    {backing.nombreBacker[0]}
                  </AvatarFallback>
                </Avatar>
                <div>
                  <p className="font-medium text-white">
                    {backing.nombreBacker}
                  </p>
                  {backing.esAnonimo && (
                    <Badge variant="secondary" className="text-xs">
                      Anónimo
                    </Badge>
                  )}
                </div>
              </div>
            </TableCell>
            <TableCell className="text-muted-foreground">
              {backing.rewardNombre || (
                <span className="italic">Sin recompensa</span>
              )}
            </TableCell>
            <TableCell className="font-semibold text-white">
              {formatCurrency(backing.monto)}
            </TableCell>
            <TableCell>
              {backing.mensaje ? (
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <p className="text-sm text-muted-foreground truncate max-w-[200px]">
                        {backing.mensaje}
                      </p>
                    </TooltipTrigger>
                    <TooltipContent className="max-w-sm">
                      <p>{backing.mensaje}</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              ) : (
                <span className="text-muted-foreground italic text-sm">-</span>
              )}
            </TableCell>
            <TableCell className="text-right text-muted-foreground">
              {formatRelativeDate(backing.fechaCreacion)}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  </CardContent>
</Card>
```

#### 5. Paginación

**Componente:**
```tsx
<div className="flex items-center justify-between mt-6">
  <p className="text-sm text-muted-foreground">
    Mostrando {startIndex}-{endIndex} de {totalBackings} backings
  </p>

  <div className="flex items-center gap-2">
    <Button
      variant="outline"
      size="sm"
      onClick={() => setPage(page - 1)}
      disabled={page === 1}
    >
      <ChevronLeft className="h-4 w-4" />
      Anterior
    </Button>

    <div className="flex items-center gap-1">
      {Array.from({ length: totalPages }, (_, i) => i + 1)
        .filter((p) => {
          // Mostrar: primera, última, actual, ±1 alrededor de actual
          return p === 1 || p === totalPages || Math.abs(p - page) <= 1
        })
        .map((p, idx, arr) => (
          <>
            {idx > 0 && arr[idx - 1] !== p - 1 && (
              <span className="text-muted-foreground">...</span>
            )}
            <Button
              key={p}
              variant={p === page ? 'default' : 'outline'}
              size="sm"
              onClick={() => setPage(p)}
              className="min-w-[40px]"
            >
              {p}
            </Button>
          </>
        ))}
    </div>

    <Button
      variant="outline"
      size="sm"
      onClick={() => setPage(page + 1)}
      disabled={page === totalPages}
    >
      Siguiente
      <ChevronRight className="h-4 w-4" />
    </Button>
  </div>
</div>
```

### Estados de UI

#### Loading State
```tsx
<div className="space-y-4">
  {[1, 2, 3, 4, 5].map((i) => (
    <div key={i} className="flex items-center gap-4">
      <Skeleton className="h-10 w-10 rounded-full" />
      <Skeleton className="h-4 flex-1" />
      <Skeleton className="h-4 w-20" />
      <Skeleton className="h-4 w-24" />
    </div>
  ))}
</div>
```

#### Empty State (No backings)
```tsx
<div className="text-center py-16">
  <Users className="h-16 w-16 text-muted-foreground mx-auto mb-4" />
  <h3 className="text-xl font-semibold text-white mb-2">
    Aún no hay backings
  </h3>
  <p className="text-muted-foreground mb-6 max-w-md mx-auto">
    Cuando alguien apoye tu campaña, verás la información aquí.
  </p>
  <div className="flex gap-4 justify-center">
    <Link href={`/campanias/${id}`}>
      <Button variant="outline">
        Ver campaña
      </Button>
    </Link>
    <Button className="bg-gradient-to-r from-pink-500 to-purple-600">
      <Share2 className="mr-2 h-4 w-4" />
      Compartir campaña
    </Button>
  </div>
</div>
```

#### Empty State (Filtros sin resultados)
```tsx
<div className="text-center py-12">
  <Search className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
  <p className="text-lg font-medium text-white mb-2">
    No se encontraron resultados
  </p>
  <p className="text-sm text-muted-foreground">
    Intenta ajustar los filtros de búsqueda
  </p>
</div>
```

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| Click en header sortable | Toggle sort direction (asc/desc) |
| Click en filtro | Filtrar backings según categoría |
| Type en search | Debounced search (300ms) en nombre/email |
| Click en "Exportar CSV" | Download CSV con todos los backings |
| Click en número de página | Navigate to page |
| Hover en mensaje truncado | Mostrar tooltip con mensaje completo |
| Click en fila | No action (considerar modal con detalle en futuro) |

---

## Responsive Breakpoints

### Mobile (< 640px)

**Dashboard Principal:**
- Stats cards: 1 columna (stacked)
- Chart: height reducida a 200px
- Mis Campañas y Backings: full width stacked
- Sidebar: collapse a hamburger menu (ya implementado)

**Campaña Detail:**
- Stats cards: 2x2 grid
- Progress bar: full width
- Tabla backings: scroll horizontal con columnas mínimas (Backer, Monto, Fecha)

**Backings List:**
- Stats header: 1 columna
- Filtros: stack verticalmente
- Tabla: scroll horizontal
- Paginación: solo botones prev/next (sin números)

### Tablet (640px - 1024px)

**Dashboard Principal:**
- Stats cards: 2x2 grid
- Mis Campañas y Backings: 2 columnas lado a lado

**Campaña Detail:**
- Stats cards: 2x2 grid
- Tabla: full width con todas las columnas

**Backings List:**
- Stats header: 3 columnas
- Filtros: wrap si es necesario
- Tabla: full width con todas las columnas

### Desktop (> 1024px)

**Dashboard Principal:**
- Stats cards: 4 columnas
- Full layout como en mockup

**Campaña Detail:**
- Stats cards: 4 columnas
- Full layout

**Backings List:**
- Stats header: 3 columnas
- Full layout con filtros en línea

**Tailwind Classes:**
```tsx
// Responsive grid para stats
<div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">

// Responsive layout mis campanias/backings
<div className="grid gap-6 lg:grid-cols-2">

// Responsive filtros
<div className="flex flex-col md:flex-row gap-4">

// Responsive tabla (scroll horizontal mobile)
<div className="overflow-x-auto">
  <Table className="min-w-[600px]">
```

---

## Animaciones

| Elemento | Animación | Duración | Easing |
|----------|-----------|----------|--------|
| Stat card hover | Border glow + scale(1.02) | 200ms | ease-in-out |
| Campaña card hover | Background fade in | 200ms | ease-in-out |
| Progress bar fill | Width transition | 500ms | ease-out |
| Page transition | Fade in opacity | 150ms | ease-in |
| Table row hover | Background fade | 150ms | ease-in-out |
| Button hover | Scale(1.05) + brightness | 200ms | ease-in-out |
| Skeleton pulse | Opacity 0.5-1 loop | 2s | ease-in-out |
| Toast notification | Slide in from top | 300ms | ease-out |

**Tailwind Classes:**
```tsx
// Card hover con border glow
className="transition-all duration-200 hover:border-primary/50 hover:scale-[1.02]"

// Background hover
className="transition-colors duration-200 hover:bg-card"

// Button hover
className="transition-all duration-200 hover:scale-105 hover:brightness-110"

// Progress bar animation
<Progress className="transition-all duration-500 ease-out" value={percentage} />

// Skeleton pulse
<Skeleton className="animate-pulse" />
```

**CSS Keyframes (en globals.css):**
```css
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 200ms ease-in-out;
}
```

---

## Accesibilidad

### Requisitos WCAG 2.1 AA

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de Color** | Ratio mínimo 4.5:1 para texto, 3:1 para elementos grandes |
| **Keyboard Navigation** | Todos los elementos interactivos accesibles con Tab |
| **Focus Visible** | Ring visible en todos los elementos enfocados |
| **Labels** | Todos los inputs con labels asociados |
| **Alt Text** | Avatars con fallback text |
| **ARIA Roles** | Roles semánticos en tablas, cards, dialogs |
| **Screen Reader** | Anuncios de estado y cambios dinámicos |

### Focus Styles

```css
/* Focus ring global (en globals.css) */
*:focus-visible {
  outline: 2px solid hsl(var(--primary));
  outline-offset: 2px;
}

/* Button focus */
.button:focus-visible {
  ring: 2;
  ring-primary;
  ring-offset: 2;
}

/* Card focus (si es clickeable) */
.card-interactive:focus-visible {
  border-color: hsl(var(--primary));
  box-shadow: 0 0 0 2px hsl(var(--primary) / 0.2);
}
```

### ARIA Labels

```tsx
// Stats card
<Card aria-label={`Estadística: ${title}`}>
  <div role="status" aria-live="polite">
    {value}
  </div>
</Card>

// Progress bar
<Progress
  value={percentage}
  aria-label="Progreso de meta"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
/>

// Table
<Table aria-label="Lista de backings">
  <TableHeader>
    <TableRow>
      <TableHead scope="col">Backer</TableHead>
      ...
    </TableRow>
  </TableHeader>
</Table>

// Botón con solo icono
<Button aria-label="Volver a campanias">
  <ArrowLeft className="h-4 w-4" />
</Button>

// Paginación
<nav aria-label="Paginación de backings">
  <Button aria-label={`Ir a página ${page}`}>
    {page}
  </Button>
</nav>
```

### Screen Reader Announcements

```tsx
// Cuando stats se actualizan
<div role="status" aria-live="polite" className="sr-only">
  Estadísticas actualizadas. Total recaudado: {formatCurrency(total)}
</div>

// Cuando tabla se filtra
<div role="status" aria-live="polite" className="sr-only">
  Mostrando {filteredCount} de {totalCount} backings
</div>

// Loading state
<div role="status" aria-live="polite">
  <span className="sr-only">Cargando estadísticas...</span>
  <Skeleton />
</div>
```

### Semantic HTML

```tsx
// Headers jerárquicos
<h1>Dashboard</h1>
  <h2>Mis Campañas</h2>
    <h3>{campania.titulo}</h3>

// Landmarks
<main className="flex-1 overflow-auto">
<nav aria-label="Sidebar navigation">
<aside aria-label="Stats sidebar">

// Lists
<ul role="list" className="space-y-2">
  {campanias.map(c => (
    <li key={c.id}>...</li>
  ))}
</ul>
```

---

## Data Mapping: API Endpoints → Components

### Dashboard Principal (`/dashboard`)

| Componente | API Endpoint | Response Field | Transformación |
|------------|--------------|----------------|----------------|
| Stats - Total Recaudado | GET `/api/campanias/mis-campanias` | `Campania[]` | `reduce((sum, c) => sum + c.importePledgedActual, 0)` |
| Stats - Campañas Activas | GET `/api/campanias/mis-campanias` | `Campania[]` | `filter(c => c.estadoCampaniaId === 2).length` |
| Stats - Total Campañas | GET `/api/campanias/mis-campanias` | `Campania[]` | `length` |
| Stats - Backers Totales | **NEW** GET `/api/stats/backers` | `{ totalBackers: number }` | Directo |
| Stats - Tasa de Éxito | GET `/api/campanias/mis-campanias` | `Campania[]` | Calcular: finalizadas exitosas / total finalizadas |
| Mis Campañas List | GET `/api/campanias/mis-campanias` | `Campania[]` | `slice(0, 5)` para preview |
| Recent Backings | **NEW** GET `/api/backings/recent` | `BackingPublicDto[]` | Directo |

### Campaña Detail (`/campanias/[id]`)

| Componente | API Endpoint | Response Field | Transformación |
|------------|--------------|----------------|----------------|
| Campaña Info | GET `/api/campanias/{id}` | `Campania` | Directo |
| Stats - Recaudado | GET `/api/campanias/{id}` | `importePledgedActual` | `formatCurrency()` |
| Stats - Backers | **NEW** GET `/api/campanias/{id}/stats` | `CampaniaStats.totalBackers` | Directo |
| Stats - Días Restantes | GET `/api/campanias/{id}` | `fechaFin` | `getDaysRemaining(fechaFin)` |
| Stats - Promedio | **NEW** GET `/api/campanias/{id}/stats` | `CampaniaStats.promedioAporte` | `formatCurrency()` |
| Progress Bar | GET `/api/campanias/{id}` | `importePledgedActual, importeObjetivo` | `calculatePercentage()` |
| Backings Recientes | GET `/api/campanias/{id}/backings?limit=5` | `BackingPublicDto[]` | Directo |
| Reward Stats | **NEW** GET `/api/campanias/{id}/rewards/stats` | `RewardStats[]` | Agrupar y contar |

### Backings List (`/campanias/[id]/backings`)

| Componente | API Endpoint | Response Field | Transformación |
|------------|--------------|----------------|----------------|
| Stats Header | **NEW** GET `/api/campanias/{id}/stats` | `CampaniaStats` | Directo |
| Backings Table | GET `/api/campanias/{id}/backings` | `BackingPublicDto[]` | Paginación client-side o query params `?page=1&limit=20` |
| Filters | Client-side | - | `filter(backing => ...)` |
| Search | Client-side | - | `filter(b => b.nombreBacker.includes(query))` |
| Sort | Client-side | - | `sort((a, b) => ...)` |

### NEW Endpoints Needed (Backend)

**CRITICAL:** Estos endpoints aún NO existen y deben ser creados:

1. **GET `/api/stats/backers`**
   - Response: `{ totalBackers: number }`
   - Calcula total de backers únicos para artista actual

2. **GET `/api/campanias/{id}/stats`**
   - Response: `CampaniaStats` (ya existe en types)
   - Campos: `totalBackers, totalRecaudado, promedioAporte, aporteMinimo, aporteMaximo, diasRestantes`

3. **GET `/api/backings/recent`**
   - Response: `BackingPublicDto[]`
   - Últimos 5 backings de todas las campañas del artista

4. **GET `/api/campanias/{id}/rewards/stats`** (opcional MVP)
   - Response: `{ rewardId: string, nombre: string, vendidos: number }[]`
   - Stats de rewards agrupados

### Query Keys (TanStack Query)

```typescript
// En src/shared/constants/index.ts (ALREADY EXISTS, may need additions)

export const QUERY_KEYS = {
  // Existing
  campanias: {
    all: ["campanias"] as const,
    byId: (id: string) => ["campanias", id] as const,
    stats: (id: string) => ["campanias", id, "stats"] as const,
    misCampanias: ["campanias", "mis-campanias"] as const,
  },
  backings: {
    all: ["backings"] as const,
    byCampania: (campaniaId: string) => ["backings", "campania", campaniaId] as const,
    recent: ["backings", "recent"] as const, // NEW
  },
  stats: {
    backers: ["stats", "backers"] as const, // NEW
  },
}
```

### Custom Hooks

**Existing:**
- `useMisCampanias()` - Already exists
- `useCampania(id)` - Already exists

**NEW Hooks Needed:**

```tsx
// src/admin/src/hooks/use-stats.ts
export function useArtistStats() {
  return useQuery({
    queryKey: QUERY_KEYS.stats.backers,
    queryFn: () => statsService.getArtistBackers(),
  })
}

export function useCampaniaStats(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.campanias.stats(id),
    queryFn: () => campaniaService.getStats(id),
    enabled: !!id,
  })
}

// src/admin/src/hooks/use-backings.ts
export function useRecentBackings() {
  return useQuery({
    queryKey: QUERY_KEYS.backings.recent,
    queryFn: () => backingService.getRecent(),
  })
}

export function useCampaniaBackings(campaniaId: string) {
  return useQuery({
    queryKey: QUERY_KEYS.backings.byCampania(campaniaId),
    queryFn: () => backingService.getByCampania(campaniaId),
    enabled: !!campaniaId,
  })
}
```

---

## Componentes shadcn/ui Requeridos

### Already Installed (Verified)
- ✅ Button
- ✅ Card, CardContent, CardHeader, CardTitle
- ✅ Input
- ✅ Label
- ✅ Textarea
- ✅ Progress
- ✅ Badge
- ✅ Skeleton
- ✅ Sonner (toasts)
- ✅ DropdownMenu
- ✅ Dialog
- ✅ Alert, AlertTitle, AlertDescription
- ✅ Select
- ✅ Separator
- ✅ Popover
- ✅ Calendar
- ✅ Tabs
- ✅ Table, TableHeader, TableBody, TableRow, TableHead, TableCell
- ✅ AlertDialog
- ✅ Checkbox

### Need to Install
- Avatar, AvatarFallback, AvatarImage
- Tooltip, TooltipProvider, TooltipTrigger, TooltipContent

**Installation Command:**
```bash
cd src/admin
npx shadcn@latest add avatar tooltip
```

---

## Iconos Lucide

### Icons Needed

| Contexto | Icon | Import |
|----------|------|--------|
| Stats - Recaudado | TrendingUp | `lucide-react` |
| Stats - Backers | Users | `lucide-react` |
| Stats - Campañas | Music | `lucide-react` |
| Stats - Éxito | BarChart3 | `lucide-react` |
| Stats - Promedio | DollarSign | `lucide-react` |
| Stats - Días | Calendar | `lucide-react` |
| Stats - Award | Award | `lucide-react` |
| Actions - Nueva | PlusCircle | `lucide-react` |
| Actions - Editar | Edit | `lucide-react` |
| Actions - Ver | ExternalLink | `lucide-react` |
| Actions - Compartir | Share2 | `lucide-react` |
| Actions - Export | Download | `lucide-react` |
| Navigation - Volver | ArrowLeft | `lucide-react` |
| Navigation - Next | ChevronRight | `lucide-react` |
| Navigation - Prev | ChevronLeft | `lucide-react` |
| Navigation - Down | ChevronDown | `lucide-react` |
| Status - Success | CheckCircle | `lucide-react` |
| Status - Error | AlertCircle | `lucide-react` |
| Empty - Music | Music | `lucide-react` |
| Empty - Users | Users | `lucide-react` |
| Empty - Search | Search | `lucide-react` |

**All icons already available in `lucide-react` package (no installation needed).**

---

## Checklist UI/UX

### Dashboard Principal (`/dashboard`)
- [ ] Layout implementado con sidebar + header (ALREADY EXISTS)
- [ ] CompleteProfileBanner funcionando (ALREADY EXISTS)
- [ ] Stats cards grid (4 cols desktop, 2x2 tablet, 1 col mobile)
- [ ] StatsCard component con datos reales (no client-side calculation)
- [ ] Trend percentages funcionando
- [ ] Chart placeholder o implementado
- [ ] Mis Campañas card con campañas reales
- [ ] Campaña items con imagen, título, estado badge, porcentaje
- [ ] Estado badges con colores correctos
- [ ] Recent Backings card con avatars y datos reales
- [ ] Empty states para no campañas, no backings
- [ ] Loading skeletons funcionando
- [ ] Error handling con Alert component
- [ ] Responsive layout probado en mobile, tablet, desktop
- [ ] Animaciones hover en cards
- [ ] Links funcionando: Nueva campaña, Ver todas, etc.

### Campaña Detail (`/campanias/[id]`)
- [ ] Layout con header + back button
- [ ] Stats cards con datos de API (4 cards)
- [ ] Progress bar full-width con porcentaje y montos
- [ ] Badge "Meta alcanzada" cuando >= 100%
- [ ] Backings recientes table (5 items)
- [ ] Table con avatars, reward names, formateo correcto
- [ ] Botones: Volver, Ver público, Editar
- [ ] Link "Ver todos los backers" funcionando
- [ ] Reward stats card (opcional MVP)
- [ ] Empty state cuando no hay backings
- [ ] Loading state con skeletons
- [ ] Error handling
- [ ] Responsive layout
- [ ] Hover effects en table rows

### Backings List (`/campanias/[id]/backings`)
- [ ] Layout con header + back button
- [ ] Stats header (3 cards)
- [ ] Filtros: Search input funcionando
- [ ] Filtros: Botones Todos, Con Reward, Anónimos
- [ ] Botón "Exportar CSV" implementado
- [ ] Table completa con todas las columnas
- [ ] Avatars con fallback funcionando
- [ ] Badges "Anónimo" en backings anónimos
- [ ] Tooltip en mensajes truncados
- [ ] Sortable headers (Backer, Monto, Fecha)
- [ ] Paginación funcionando (20 per page)
- [ ] Números de página dinámicos
- [ ] Empty state: No backings
- [ ] Empty state: Filtros sin resultados
- [ ] Loading state con skeletons
- [ ] Responsive: scroll horizontal en mobile
- [ ] Filtros stack vertical en mobile

### Accesibilidad
- [ ] Contraste de colores >= 4.5:1 verificado
- [ ] Focus visible en todos los elementos interactivos
- [ ] Keyboard navigation funcionando (Tab order correcto)
- [ ] ARIA labels en stats, progress bars, tables
- [ ] Screen reader announcements implementados
- [ ] Semantic HTML (h1, h2, nav, main, etc.)
- [ ] Alt text en avatars con fallbacks
- [ ] Buttons con solo icono tienen aria-label

### Performance
- [ ] React Query caching configurado correctamente
- [ ] Query keys granulares para invalidación eficiente
- [ ] Lazy loading de componentes pesados (charts)
- [ ] Debounced search (300ms)
- [ ] Optimistic updates en mutations (si aplica)
- [ ] Skeleton loaders en lugar de spinners
- [ ] No re-renders innecesarios (React.memo donde necesario)

### Data Integration
- [ ] Endpoints existentes funcionando: `/api/campanias/mis-campanias`
- [ ] NEW endpoint creado: `/api/stats/backers`
- [ ] NEW endpoint creado: `/api/campanias/{id}/stats`
- [ ] NEW endpoint creado: `/api/backings/recent`
- [ ] NEW endpoint creado: `/api/campanias/{id}/backings` (con paginación)
- [ ] Custom hooks creados: `useArtistStats`, `useCampaniaStats`, etc.
- [ ] Error handling en hooks con mensajes user-friendly
- [ ] Loading states manejados en todos los hooks
- [ ] Query invalidation funcionando en mutations

---

## Notas de Implementación

### Prioridad MVP

**MUST HAVE (P0):**
1. Dashboard principal con stats cards (datos reales, no client-side)
2. Mis Campañas list con estado badges
3. Recent Backings card
4. Campaña detail con stats y progress bar
5. Backings recientes table en detail
6. Empty states y loading states

**NICE TO HAVE (P1):**
1. Chart de recaudación (puede ser placeholder "Próximamente")
2. Reward stats en campaña detail
3. Filtros avanzados en backings list
4. Exportar CSV
5. Tooltips en mensajes

**FUTURE (P2):**
1. Gráficas interactivas con drill-down
2. Dashboard comparativo (mes vs mes)
3. Notificaciones en tiempo real de nuevos backings
4. Análisis de conversión

### Orden de Desarrollo Sugerido

1. **Backend First:** Crear endpoints NEW necesarios
   - `GET /api/stats/backers`
   - `GET /api/campanias/{id}/stats`
   - `GET /api/backings/recent`
   - `GET /api/campanias/{id}/backings` (verificar si existe, agregar paginación)

2. **Custom Hooks:** Crear hooks para consumir APIs
   - `useArtistStats()`
   - `useCampaniaStats(id)`
   - `useRecentBackings()`
   - `useCampaniaBackings(campaniaId)`

3. **Components:** Instalar componentes faltantes
   - `npx shadcn@latest add avatar tooltip`

4. **Dashboard Principal:** Enhance existing dashboard
   - Reemplazar client-side calculations con datos de API
   - Mejorar Mis Campañas cards (imágenes, estados)
   - Mejorar Recent Backings (avatars)

5. **Campaña Detail:** Nueva página
   - Layout base con stats cards
   - Progress bar
   - Backings table
   - Botones de acción

6. **Backings List:** Nueva página
   - Stats header
   - Filtros y búsqueda
   - Table completa
   - Paginación

7. **Polish:** Animaciones, responsive, accesibilidad
   - Hover effects
   - Mobile layouts
   - ARIA labels
   - Focus styles

### Diferencias con Mockup WPR_5

El mockup muestra un diseño general. Esta spec EXTIENDE el mockup con:
- **Campaña Detail view** (no mostrada en mockup original)
- **Backings List full page** (no mostrada en mockup original)
- **Filtros y búsqueda** (nuevas funcionalidades)
- **Reward stats** (opcional, no en mockup)
- **Chart de recaudación** (en mockup, puede ser placeholder MVP)

### Consideraciones de Datos

**Client-side vs Server-side:**
- ✅ **Server-side:** Total backers, stats aggregations (más preciso)
- ❌ **Client-side:** Suma de importes, conteo de campañas activas (suficiente para MVP si endpoints no existen)

**Para MVP rápido:** Mantener client-side calculations en dashboard principal si endpoints tardan. Migrar a server-side en iteración posterior.

**Paginación:**
- **Client-side:** OK para < 100 items
- **Server-side:** Necesario para > 100 items (implementar query params `?page=1&limit=20`)

---

## Referencias de Implementación

### Templates Dashtail

Consultar estos archivos para inspiración de diseño:

```
references/templates/dashtail/
├── pages/
│   ├── dashboard.html         # Stats cards layout
│   ├── analytics.html         # Charts y métricas
│   ├── tables.html            # Table styles y paginación
│   └── profile.html           # Profile cards
├── components/
│   ├── cards.html             # Card variations
│   ├── buttons.html           # Button styles
│   └── charts.html            # Chart examples
└── assets/
    ├── css/
    │   └── style.css          # Color tokens, spacing
    └── js/
        └── dashboard.js       # Chart.js config
```

**Adaptaciones necesarias:**
- Convertir HTML a React components
- Reemplazar Chart.js con Recharts (si se usa)
- Usar shadcn/ui en lugar de Tailwind custom
- Adaptar color scheme a WePlay Rises (pink/purple)

### Shared Utils

Usar funciones ya existentes en `src/shared/utils/format.ts`:
- `formatCurrency(amount, currency?)`
- `formatRelativeDate(date)`
- `formatDate(date)`
- `calculatePercentage(current, total)`
- `getDaysRemaining(endDate)`
- `getBackerDisplayName(esAnonimo, userName?)`

### Existing Components (Admin)

Reutilizar:
- `<StatsCard>` (ALREADY EXISTS en `src/admin/src/components/dashboard/stats-card.tsx`)
- `<CompleteProfileBanner>` (ALREADY EXISTS)
- Sidebar layout (ALREADY EXISTS)
- Header layout (ALREADY EXISTS)

---

**END OF SPECIFICATION**

**Total Screens:** 3
**Total Components (new/enhanced):** ~15
**API Endpoints (new):** 4
**Complexity:** Medium-High (data aggregation, tables, filters)
**Estimated Dev Time:** 16-20 hours (with backend endpoints)
