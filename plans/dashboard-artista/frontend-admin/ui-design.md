# Diseno UI: Dashboard Artista (Admin)

**Fecha:** 2026-02-14
**Feature:** dashboard-artista
**Target:** src/admin (Next.js 14)

---

## 1. Resumen

- **Componentes shadcn:** 12 (existentes + 2 nuevos a instalar)
- **Composiciones custom:** 7 nuevos componentes
- **Responsive breakpoints:** sm (640px), md (768px), lg (1024px), xl (1280px)
- **Tema:** Dark theme con gradiente pink/purple
- **Animaciones:** Hover effects, transitions, loading states

---

## 2. Paleta de Colores (Dark Theme)

### Variables CSS Personalizadas

```css
/* Agregar a src/admin/src/styles/globals.css */
:root {
  /* Background - Dark Theme */
  --bg-primary: #0a0b14;        /* Fondo principal oscuro */
  --bg-secondary: #1a1d2e;      /* Cards y superficies elevadas */
  --bg-tertiary: #252837;       /* Hover states */
  --bg-sidebar: #0f1117;        /* Sidebar background */

  /* Primary - Pink/Purple Gradient */
  --primary-pink: #e91e8c;      /* Rosa principal */
  --primary-purple: #a855f7;    /* Morado principal */

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

  /* Status Colors (badges) */
  --status-active: #10b981;     /* Campana activa */
  --status-draft: #eab308;      /* Borrador */
  --status-finalized: #3b82f6;  /* Finalizada */
  --status-canceled: #ef4444;   /* Cancelada */

  /* Borders */
  --border-default: #252837;
  --border-hover: #3b3f54;
}

/* Gradient utility */
.gradient-primary {
  background: linear-gradient(135deg, #e91e8c 0%, #a855f7 100%);
}
```

### Mapeo a Tailwind Classes

| Uso | Tailwind Class | Valor |
|-----|----------------|-------|
| Primary Button | `bg-gradient-to-r from-pink-500 to-purple-600` | Gradiente pink/purple |
| Success Badge | `bg-green-500/20 text-green-400 border-green-500/30` | Verde transparente |
| Warning Badge | `bg-yellow-500/20 text-yellow-400 border-yellow-500/30` | Amarillo transparente |
| Error Badge | `bg-red-500/20 text-red-400 border-red-500/30` | Rojo transparente |
| Card Background | `bg-secondary border-border` | Card oscuro |
| Text Primary | `text-white` | Blanco |
| Text Secondary | `text-muted-foreground` | Gris claro |

---

## 3. Componentes por Screen

### 3.1 Dashboard Principal (`/dashboard`)

#### Layout Base

```
┌─────────────────────────────────────────────────────────────────┐
│ SIDEBAR (250px)  │  HEADER                      [+ Nueva Camp] │
│                  ├─────────────────────────────────────────────┤
│ WePlay Rises     │  Hola, {Nombre Artista}                     │
│ [Avatar]         │  {Fecha actual}                             │
│ {Nombre}         │                                             │
│ Artista          │  [CompleteProfileBanner] (if no artista)    │
│                  │                                             │
│ - Dashboard      │  STATS CARDS (Grid 4 cols)                 │
│ - Mis Campanias  │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐          │
│ - Crear Campania │  │ $   │ │ 👥  │ │ 🎵  │ │ 📊  │          │
│ - Configuracion  │  │24.5K│ │1,247│ │  3  │ │ 78% │          │
│                  │  │Total│ │Back.│ │Camp.│ │Exito│          │
│ - Ver perfil     │  │+12% │ │+8.2%│ │Activ│ │+5.1%│          │
│ - Logout         │  └─────┘ └─────┘ └─────┘ └─────┘          │
│                  │                                             │
│                  │  CHART: Recaudacion ultimos 30 dias         │
│                  │  ┌──────────────────────────────────────┐   │
│                  │  │ [Placeholder "Proximamente"]         │   │
│                  │  └──────────────────────────────────────┘   │
│                  │                                             │
│                  │  ┌───────────────┐  ┌──────────────────┐   │
│                  │  │ MIS CAMPANIAS │  │ ULTIMOS BACKINGS │   │
│                  │  │ [Ver todas]   │  │ [Ver todos]      │   │
│                  │  │               │  │                  │   │
│                  │  │ [CampCards]   │  │ [BackingCards]   │   │
│                  │  └───────────────┘  └──────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

#### Componentes Principales

**1. Stats Cards Grid**

| Componente | shadcn Base | Customizacion |
|------------|-------------|---------------|
| Container | `<div>` | `grid gap-4 md:grid-cols-2 lg:grid-cols-4` |
| StatsCard | `Card`, `CardHeader`, `CardContent` | Ya existe, sin cambios necesarios |

**Props Interface:**
```typescript
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

**Composicion Ejemplo:**
```tsx
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
  <StatsCard
    title="Total Recaudado"
    value={formatCurrency(totalRecaudado)}
    icon={TrendingUp}
    trend={{ value: 12, isPositive: true }}
  />
  <StatsCard
    title="Backers Totales"
    value={totalBackers.toString()}
    icon={Users}
    trend={{ value: 8.2, isPositive: true }}
  />
  <StatsCard
    title="Campanias Activas"
    value={campaniasActivas.toString()}
    icon={Music}
  />
  <StatsCard
    title="Tasa de Exito"
    value={`${tasaExito}%`}
    icon={BarChart3}
    trend={{ value: 5.1, isPositive: true }}
  />
</div>
```

**2. Chart Placeholder**

| Elemento | Componente | Estilos |
|----------|------------|---------|
| Container | `Card` | `lg:col-span-4` |
| Header | `CardHeader` | - |
| Title | `CardTitle` | `text-xl font-bold text-white` |
| Placeholder | `<div>` | `h-[300px] flex items-center justify-center text-muted-foreground` |

**Composicion:**
```tsx
<Card className="lg:col-span-4">
  <CardHeader>
    <CardTitle>Recaudacion ultimos 30 dias</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="h-[300px] flex items-center justify-center text-muted-foreground">
      <div className="text-center">
        <BarChart3 className="h-12 w-12 mx-auto mb-4 opacity-50" />
        <p className="text-lg font-medium">Grafica proximamente</p>
      </div>
    </div>
  </CardContent>
</Card>
```

**3. CampaignCard (NUEVO)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `rounded-lg border border-border bg-card/50 p-4 hover:bg-card transition-colors cursor-pointer` |
| Imagen | `<div>` | `w-12 h-12 rounded-lg bg-gradient-to-br` (color segun estado) |
| Content Wrapper | `<div>` | `flex items-center gap-3` |
| Titulo | `<p>` | `font-medium text-white truncate` |
| Estado Badge | `Badge` | Variante segun estado |
| Progreso | `Progress` | `h-2` con color dinamico |
| Metricas Row | `<div>` | `flex items-center justify-between text-xs text-muted-foreground mt-2` |

**Props Interface:**
```typescript
interface CampaignCardProps {
  campania: {
    id: string
    titulo: string
    estadoCampaniaId: number
    estadoCampaniaNombre: string
    porcentajeProgreso: number
    numBackers: number
    importeRecaudado: number
    importeObjetivo: number
  }
  onSelect?: (id: string) => void
  className?: string
}
```

**Composicion:**
```tsx
<div
  onClick={() => onSelect?.(campania.id)}
  className="rounded-lg border border-border bg-card/50 p-4 hover:bg-card transition-colors cursor-pointer"
>
  <div className="flex items-start gap-3">
    {/* Icono color segun estado */}
    <div className={cn(
      "w-12 h-12 rounded-lg flex-shrink-0",
      getEstadoGradient(campania.estadoCampaniaId)
    )} />

    <div className="flex-1 min-w-0">
      <p className="font-medium text-white truncate mb-1">
        {campania.titulo}
      </p>

      <div className="flex items-center gap-2 mb-2">
        <Badge className={getEstadoBadgeClass(campania.estadoCampaniaId)}>
          {campania.estadoCampaniaNombre}
        </Badge>
        <span className={cn(
          "text-sm font-medium",
          getPercentageColor(campania.porcentajeProgreso)
        )}>
          {campania.porcentajeProgreso}%
        </span>
      </div>

      <Progress
        value={campania.porcentajeProgreso}
        className="h-2 mb-2"
      />

      <div className="flex items-center justify-between text-xs text-muted-foreground">
        <span>{campania.numBackers} backers</span>
        <span>
          {formatCurrency(campania.importeRecaudado)} / {formatCurrency(campania.importeObjetivo)}
        </span>
      </div>
    </div>
  </div>
</div>
```

**Helper Functions:**
```typescript
function getEstadoGradient(estadoId: number): string {
  const gradients = {
    1: "bg-gradient-to-br from-orange-500 to-orange-600", // Borrador
    2: "bg-gradient-to-br from-pink-500 to-purple-600",   // Activa
    3: "bg-gradient-to-br from-blue-500 to-cyan-600",     // Finalizada
    4: "bg-gradient-to-br from-gray-600 to-gray-700",     // Cancelada
  }
  return gradients[estadoId as keyof typeof gradients] || gradients[1]
}

function getEstadoBadgeClass(estadoId: number): string {
  const classes = {
    1: "bg-yellow-500/20 text-yellow-400 border-yellow-500/30",
    2: "bg-green-500/20 text-green-400 border-green-500/30",
    3: "bg-blue-500/20 text-blue-400 border-blue-500/30",
    4: "bg-red-500/20 text-red-400 border-red-500/30",
  }
  return classes[estadoId as keyof typeof classes] || classes[1]
}

function getPercentageColor(percentage: number): string {
  if (percentage >= 100) return "text-green-400"
  if (percentage >= 50) return "text-yellow-400"
  return "text-red-400"
}
```

**4. RecentBackingCard (MEJORAR EXISTENTE)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `rounded-lg border border-border bg-card/50 p-3 hover:bg-card transition-colors` |
| Avatar | `Avatar` (NUEVO) | `h-10 w-10` |
| AvatarFallback | `AvatarFallback` (NUEVO) | `bg-primary/20 text-primary` |
| Nombre | `<p>` | `text-sm font-medium text-white` |
| Reward | `<p>` | `text-xs text-muted-foreground` |
| Monto | `<p>` | `text-sm font-medium text-white` |
| Tiempo | `<p>` | `text-xs text-muted-foreground` |

**Props Interface:**
```typescript
interface RecentBackingCardProps {
  backing: {
    id: string
    nombreBacker: string
    rewardNombre?: string
    monto: number
    fechaCreacion: string
    esAnonimo: boolean
  }
}
```

**Composicion:**
```tsx
<div className="rounded-lg border border-border bg-card/50 p-3 hover:bg-card transition-colors">
  <div className="flex items-center gap-3">
    <Avatar className="h-10 w-10">
      <AvatarFallback className="bg-primary/20 text-primary">
        {backing.nombreBacker[0]}
      </AvatarFallback>
    </Avatar>

    <div className="flex-1 min-w-0">
      <p className="text-sm font-medium text-white truncate">
        {backing.nombreBacker}
      </p>
      <p className="text-xs text-muted-foreground">
        {backing.rewardNombre || "Sin recompensa"}
      </p>
    </div>

    <div className="text-right">
      <p className="text-sm font-medium text-white">
        {formatCurrency(backing.monto)}
      </p>
      <p className="text-xs text-muted-foreground">
        {formatRelativeDate(backing.fechaCreacion)}
      </p>
    </div>
  </div>
</div>
```

**5. EmptyState (NUEVO COMPONENTE REUTILIZABLE)**

| Elemento | Componente | Estilos |
|----------|------------|---------|
| Container | `<div>` | `text-center py-12` |
| Icon | Lucide Icon | `h-12 w-12 text-muted-foreground mx-auto mb-4` |
| Title | `<p>` | `text-lg font-medium text-white mb-2` |
| Description | `<p>` | `text-sm text-muted-foreground mb-6` |
| Action Button | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600` |

**Props Interface:**
```typescript
interface EmptyStateProps {
  icon: LucideIcon
  title: string
  description: string
  action?: {
    label: string
    onClick: () => void
    icon?: LucideIcon
  }
}
```

**Composicion:**
```tsx
<div className="text-center py-12">
  <Icon className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
  <p className="text-lg font-medium text-white mb-2">
    {title}
  </p>
  <p className="text-sm text-muted-foreground mb-6 max-w-md mx-auto">
    {description}
  </p>
  {action && (
    <Button
      onClick={action.onClick}
      className="bg-gradient-to-r from-pink-500 to-purple-600"
    >
      {action.icon && <action.icon className="mr-2 h-4 w-4" />}
      {action.label}
    </Button>
  )}
</div>
```

---

### 3.2 Campana Detail (`/dashboard/campanias/[id]`)

#### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ [← Volver]  {Titulo Campana}      [Ver publico] [Editar]       │
├─────────────────────────────────────────────────────────────────┤
│ STATS CARDS (Grid 4 cols)                                       │
│ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐               │
│ │ $1,250  │ │   15    │ │ 45 dias │ │  $83    │               │
│ │Recaudado│ │ Backers │ │Restantes│ │Promedio │               │
│ └─────────┘ └─────────┘ └─────────┘ └─────────┘               │
│                                                                 │
│ PROGRESO DE META                                                │
│ ┌───────────────────────────────────────────────────────────┐   │
│ │ [===========================>    ] 25%                    │   │
│ │ $1,250 / $5,000                                           │   │
│ │ ✓ Meta alcanzada (si >= 100%)                            │   │
│ └───────────────────────────────────────────────────────────┘   │
│                                                                 │
│ BACKINGS RECIENTES                                              │
│ ┌───────────────────────────────────────────────────────────┐   │
│ │ Backer      │ Reward      │ Monto │ Fecha                │   │
│ ├───────────────────────────────────────────────────────────┤   │
│ │ [👤] Juan   │ CD Firmado  │ $25   │ hace 2 horas         │   │
│ │ [👤] Anonimo│ Digital     │ $10   │ hace 5 horas         │   │
│ │ [👤] Maria  │ Vinilo      │ $50   │ hace 1 dia           │   │
│ └───────────────────────────────────────────────────────────┘   │
│ [Ver todos los backers]                                         │
└─────────────────────────────────────────────────────────────────┘
```

#### Componentes Especificos

**1. Header con Navegacion**

| Elemento | Componente | Estilos | Props |
|----------|------------|---------|-------|
| Container | `<div>` | `flex items-center justify-between mb-6` | - |
| Back Button | `Button` | `variant="ghost" size="sm"` | `onClick={() => router.back()}` |
| Title | `<h1>` | `text-3xl font-bold text-white` | `{campania.titulo}` |
| Actions | `<div>` | `flex items-center gap-2` | - |
| Ver Publico | `Button` | `variant="outline"` | `target="_blank"` |
| Editar | `Button` | `variant="default" gradient-primary` | - |

**Composicion:**
```tsx
<div className="flex items-center justify-between mb-6">
  <div className="flex items-center gap-4">
    <Button
      variant="ghost"
      size="sm"
      onClick={() => router.back()}
      aria-label="Volver a campanias"
    >
      <ArrowLeft className="h-4 w-4 mr-2" />
      Volver
    </Button>
    <h1 className="text-3xl font-bold text-white">
      {campania.titulo}
    </h1>
  </div>

  <div className="flex items-center gap-2">
    <Link href={`/campanias/${campania.id}`} target="_blank">
      <Button variant="outline" size="sm">
        <ExternalLink className="mr-2 h-4 w-4" />
        Ver pagina publica
      </Button>
    </Link>
    <Link href={`/campanias/${campania.id}/editar`}>
      <Button size="sm" className="bg-gradient-to-r from-pink-500 to-purple-600">
        <Edit className="mr-2 h-4 w-4" />
        Editar
      </Button>
    </Link>
  </div>
</div>
```

**2. Progress Card (NUEVO)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `Card` | `w-full` |
| Header | `CardHeader` | - |
| Title | `CardTitle` | `text-xl font-bold` |
| Content | `CardContent` | `space-y-4` |
| Amounts Row | `<div>` | `flex items-center justify-between text-sm` |
| Progress Bar | `Progress` | `h-3` con gradiente pink/purple |
| Success Badge | `Badge` | `bg-green-500/20 text-green-400` (si >= 100%) |

**Props Interface:**
```typescript
interface ProgressCardProps {
  importeObjetivo: number
  importeRecaudado: number
  className?: string
}
```

**Composicion:**
```tsx
<Card className={className}>
  <CardHeader>
    <CardTitle>Progreso de Meta</CardTitle>
  </CardHeader>
  <CardContent className="space-y-4">
    <div className="space-y-2">
      <div className="flex items-center justify-between text-sm">
        <span className="text-muted-foreground">
          {formatCurrency(importeRecaudado)} de {formatCurrency(importeObjetivo)}
        </span>
        <span className="font-semibold text-white">
          {calculatePercentage(importeRecaudado, importeObjetivo)}%
        </span>
      </div>

      <Progress
        value={calculatePercentage(importeRecaudado, importeObjetivo)}
        className="h-3 [&>div]:bg-gradient-to-r [&>div]:from-pink-500 [&>div]:to-purple-600"
      />
    </div>

    {calculatePercentage(importeRecaudado, importeObjetivo) >= 100 && (
      <Badge className="bg-green-500/20 text-green-400 border-green-500/30">
        <CheckCircle className="mr-1 h-3 w-3" />
        Meta alcanzada
      </Badge>
    )}
  </CardContent>
</Card>
```

**3. BackingsTable (NUEVO)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `Card` | - |
| Header | `CardHeader` | `flex flex-row items-center justify-between` |
| Title | `CardTitle` | - |
| Content | `CardContent` | `p-0` (sin padding para tabla completa) |
| Table | `Table` | shadcn table completa |
| Avatar | `Avatar` | `h-8 w-8` |
| Footer | `<div>` | `mt-4 text-center` |

**Props Interface:**
```typescript
interface BackingsTableProps {
  backings: Array<{
    id: string
    nombreBacker: string
    rewardNombre?: string
    monto: number
    fechaCreacion: string
    esAnonimo: boolean
  }>
  limit?: number
  showViewAll?: boolean
  onViewAll?: () => void
}
```

**Composicion:**
```tsx
<Card>
  <CardHeader className="flex flex-row items-center justify-between">
    <CardTitle>Backings Recientes</CardTitle>
  </CardHeader>
  <CardContent className="p-0">
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
        {backings.slice(0, limit).map((backing) => (
          <TableRow key={backing.id} className="hover:bg-muted/50 transition-colors">
            <TableCell>
              <div className="flex items-center gap-2">
                <Avatar className="h-8 w-8">
                  <AvatarFallback className="bg-primary/20 text-primary">
                    {backing.nombreBacker[0]}
                  </AvatarFallback>
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

    {showViewAll && (
      <div className="mt-4 text-center p-4">
        <Button variant="ghost" onClick={onViewAll}>
          Ver todos los backers
          <ChevronRight className="ml-2 h-4 w-4" />
        </Button>
      </div>
    )}
  </CardContent>
</Card>
```

---

### 3.3 Backings List Full (`/dashboard/campanias/[id]/backings`)

#### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ [← Volver]  Backers: {Titulo Campana}                          │
├─────────────────────────────────────────────────────────────────┤
│ STATS HEADER (Grid 3 cols)                                      │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐               │
│ │  $1,250     │ │    $83      │ │ CD Firmado  │               │
│ │Total Recaud.│ │  Promedio   │ │Mas Popular  │               │
│ └─────────────┘ └─────────────┘ └─────────────┘               │
│                                                                 │
│ FILTROS Y BUSQUEDA                              [Export CSV]    │
│ ┌───────────────────────────────────────────────────────────┐   │
│ │ [Buscar...] [Todos] [Con Reward] [Anonimos]              │   │
│ └───────────────────────────────────────────────────────────┘   │
│                                                                 │
│ BACKINGS TABLE                                                  │
│ ┌───────────────────────────────────────────────────────────┐   │
│ │ Backer ↓  │ Reward      │ Monto ↑ │ Mensaje │ Fecha     │   │
│ ├───────────────────────────────────────────────────────────┤   │
│ │ [👤] Juan │ CD Firmado  │ $25     │ "..."   │ hace 2h   │   │
│ │ [👤] Anon │ Digital     │ $10     │   -     │ hace 5h   │   │
│ └───────────────────────────────────────────────────────────┘   │
│                                                                 │
│ [Anterior] Pagina 1 de 3 [Siguiente]                           │
└─────────────────────────────────────────────────────────────────┘
```

#### Componentes Especificos

**1. Stats Header (Grid 3 cols)**

Reutilizar `StatsCard` con grid de 3 columnas:
```tsx
<div className="grid gap-4 md:grid-cols-3 mb-6">
  <StatsCard
    title="Total Recaudado"
    value={formatCurrency(stats.totalRecaudado)}
    icon={DollarSign}
  />
  <StatsCard
    title="Backing Promedio"
    value={formatCurrency(stats.backingPromedio)}
    icon={TrendingUp}
  />
  <StatsCard
    title="Reward Mas Popular"
    value={stats.rewardMasPopular || "N/A"}
    icon={Award}
  />
</div>
```

**2. Filtros y Busqueda (NUEVO)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `Card` | `mb-6` |
| Content | `CardContent` | `pt-6` |
| Layout | `<div>` | `flex flex-col md:flex-row gap-4` |
| Search Input | `Input` | `max-w-sm` con icon Search |
| Filters Container | `<div>` | `flex gap-2` |
| Filter Button | `Button` | `variant="default"` o `"outline"` segun estado |
| Export Button | `Button` | `variant="outline" size="sm"` |

**Props Interface:**
```typescript
interface BackingsFiltersProps {
  searchQuery: string
  onSearchChange: (query: string) => void
  activeFilter: 'all' | 'with-reward' | 'anonymous'
  onFilterChange: (filter: 'all' | 'with-reward' | 'anonymous') => void
  onExport: () => void
}
```

**Composicion:**
```tsx
<Card className="mb-6">
  <CardContent className="pt-6">
    <div className="flex flex-col md:flex-row gap-4">
      {/* Search Input */}
      <div className="flex-1">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Buscar por nombre o email..."
            value={searchQuery}
            onChange={(e) => onSearchChange(e.target.value)}
            className="pl-10 max-w-sm"
          />
        </div>
      </div>

      {/* Filters */}
      <div className="flex gap-2 flex-wrap">
        <Button
          variant={activeFilter === 'all' ? 'default' : 'outline'}
          onClick={() => onFilterChange('all')}
          size="sm"
        >
          Todos
        </Button>
        <Button
          variant={activeFilter === 'with-reward' ? 'default' : 'outline'}
          onClick={() => onFilterChange('with-reward')}
          size="sm"
        >
          Con Reward
        </Button>
        <Button
          variant={activeFilter === 'anonymous' ? 'default' : 'outline'}
          onClick={() => onFilterChange('anonymous')}
          size="sm"
        >
          Anonimos
        </Button>
      </div>

      {/* Export Button */}
      <Button variant="outline" size="sm" onClick={onExport}>
        <Download className="mr-2 h-4 w-4" />
        Exportar CSV
      </Button>
    </div>
  </CardContent>
</Card>
```

**3. Backings Full Table**

Extender `BackingsTable` con:
- Columnas adicionales (Mensaje con tooltip)
- Sortable headers
- Anonymous badge

**Composicion:**
```tsx
<Card>
  <CardContent className="p-0">
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[25%]">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleSort('backer')}
              className="hover:bg-transparent"
            >
              Backer
              {sortBy === 'backer' && <ChevronDown className="ml-2 h-4 w-4" />}
            </Button>
          </TableHead>
          <TableHead className="w-[25%]">Reward</TableHead>
          <TableHead className="w-[15%]">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleSort('monto')}
              className="hover:bg-transparent"
            >
              Monto
              {sortBy === 'monto' && <ChevronDown className="ml-2 h-4 w-4" />}
            </Button>
          </TableHead>
          <TableHead className="w-[25%]">Mensaje</TableHead>
          <TableHead className="w-[10%] text-right">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleSort('fecha')}
              className="hover:bg-transparent"
            >
              Fecha
              {sortBy === 'fecha' && <ChevronDown className="ml-2 h-4 w-4" />}
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
                    <Badge variant="secondary" className="text-xs mt-1">
                      Anonimo
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
                      <p className="text-sm text-muted-foreground truncate max-w-[200px] cursor-help">
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

**4. Pagination (NUEVO)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `flex items-center justify-between mt-6` |
| Info Text | `<p>` | `text-sm text-muted-foreground` |
| Buttons Container | `<div>` | `flex items-center gap-2` |
| Prev/Next Button | `Button` | `variant="outline" size="sm"` |
| Page Number | `Button` | `variant="default"` o `"outline"` |
| Ellipsis | `<span>` | `text-muted-foreground` |

**Props Interface:**
```typescript
interface PaginationProps {
  currentPage: number
  totalPages: number
  totalItems: number
  pageSize: number
  onPageChange: (page: number) => void
}
```

**Composicion:**
```tsx
<div className="flex items-center justify-between mt-6">
  <p className="text-sm text-muted-foreground">
    Mostrando {(currentPage - 1) * pageSize + 1}-
    {Math.min(currentPage * pageSize, totalItems)} de {totalItems} backings
  </p>

  <div className="flex items-center gap-2">
    <Button
      variant="outline"
      size="sm"
      onClick={() => onPageChange(currentPage - 1)}
      disabled={currentPage === 1}
    >
      <ChevronLeft className="h-4 w-4" />
      Anterior
    </Button>

    <div className="flex items-center gap-1">
      {getPageNumbers(currentPage, totalPages).map((page, idx, arr) => (
        <React.Fragment key={page}>
          {idx > 0 && arr[idx - 1] !== page - 1 && (
            <span className="text-muted-foreground px-2">...</span>
          )}
          <Button
            variant={page === currentPage ? 'default' : 'outline'}
            size="sm"
            onClick={() => onPageChange(page)}
            className="min-w-[40px]"
          >
            {page}
          </Button>
        </React.Fragment>
      ))}
    </div>

    <Button
      variant="outline"
      size="sm"
      onClick={() => onPageChange(currentPage + 1)}
      disabled={currentPage === totalPages}
    >
      Siguiente
      <ChevronRight className="h-4 w-4" />
    </Button>
  </div>
</div>

// Helper function
function getPageNumbers(current: number, total: number): number[] {
  const pages: number[] = []

  // Always show first page
  pages.push(1)

  // Show current page and adjacent pages
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
    pages.push(i)
  }

  // Always show last page
  if (total > 1) {
    pages.push(total)
  }

  return [...new Set(pages)].sort((a, b) => a - b)
}
```

---

## 4. Estados de UI

### Loading States

**Dashboard Principal:**
```tsx
// Stats Cards Skeleton
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
  {[1, 2, 3, 4].map((i) => (
    <Skeleton key={i} className="h-32 rounded-lg" />
  ))}
</div>

// Campanias Skeleton
<div className="space-y-4">
  {[1, 2, 3].map((i) => (
    <Skeleton key={i} className="h-20 rounded-lg" />
  ))}
</div>
```

**Backings Table Skeleton:**
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

### Empty States

**No Campanias:**
```tsx
<EmptyState
  icon={Music}
  title="No tienes campanias aun"
  description="Crea tu primera campania y empieza a recaudar fondos"
  action={{
    label: "Crear campania",
    onClick: () => router.push('/campanias/nueva'),
    icon: PlusCircle
  }}
/>
```

**No Backings:**
```tsx
<EmptyState
  icon={Users}
  title="Aun no hay backings"
  description="Cuando alguien apoye tu campania, veras la informacion aqui."
  action={{
    label: "Compartir campania",
    onClick: handleShare,
    icon: Share2
  }}
/>
```

**Filtros Sin Resultados:**
```tsx
<div className="text-center py-12">
  <Search className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
  <p className="text-lg font-medium text-white mb-2">
    No se encontraron resultados
  </p>
  <p className="text-sm text-muted-foreground">
    Intenta ajustar los filtros de busqueda
  </p>
</div>
```

### Error States

```tsx
<Alert variant="destructive">
  <AlertCircle className="h-4 w-4" />
  <AlertTitle>Error al cargar datos</AlertTitle>
  <AlertDescription>
    No pudimos cargar tu dashboard. Por favor, intenta nuevamente.
    <Button
      variant="link"
      className="p-0 h-auto ml-2"
      onClick={retry}
    >
      Reintentar
    </Button>
  </AlertDescription>
</Alert>
```

---

## 5. Responsive Design

### Breakpoints

| Breakpoint | Width | Layout Changes |
|------------|-------|----------------|
| Mobile (< 640px) | < 640px | 1 col stats, stacked cards, hamburger menu |
| Tablet (640px - 1024px) | 640px - 1024px | 2x2 stats grid, 2 col cards |
| Desktop (> 1024px) | > 1024px | 4 col stats, full layout |

### Responsive Classes

**Stats Cards:**
```tsx
<div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
```

**Campanias/Backings Layout:**
```tsx
<div className="grid gap-6 lg:grid-cols-2">
```

**Filtros:**
```tsx
<div className="flex flex-col md:flex-row gap-4">
```

**Table Scroll (Mobile):**
```tsx
<div className="overflow-x-auto">
  <Table className="min-w-[600px]">
```

**Pagination Mobile:**
```tsx
// Solo mostrar prev/next en mobile
<div className="hidden sm:flex items-center gap-1">
  {/* Page numbers */}
</div>
```

---

## 6. Animaciones

### Transition Classes

**Card Hover:**
```tsx
className="transition-all duration-200 hover:border-primary/50 hover:scale-[1.02]"
```

**Background Hover:**
```tsx
className="transition-colors duration-200 hover:bg-card"
```

**Button Hover:**
```tsx
className="transition-all duration-200 hover:scale-105 hover:brightness-110"
```

**Progress Bar:**
```tsx
<Progress className="transition-all duration-500 ease-out" value={percentage} />
```

**Skeleton Pulse:**
```tsx
<Skeleton className="animate-pulse" />
```

### CSS Keyframes

Agregar a `globals.css`:
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

@keyframes slideInFromTop {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slide-in {
  animation: slideInFromTop 300ms ease-out;
}
```

---

## 7. Accesibilidad

### ARIA Labels

**Stats Card:**
```tsx
<Card aria-label={`Estadistica: ${title}`}>
  <div role="status" aria-live="polite">
    {value}
  </div>
</Card>
```

**Progress Bar:**
```tsx
<Progress
  value={percentage}
  aria-label="Progreso de meta"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
/>
```

**Table:**
```tsx
<Table aria-label="Lista de backings">
  <TableHeader>
    <TableRow>
      <TableHead scope="col">Backer</TableHead>
    </TableRow>
  </TableHeader>
</Table>
```

**Boton Solo Icono:**
```tsx
<Button aria-label="Volver a campanias">
  <ArrowLeft className="h-4 w-4" />
</Button>
```

**Paginacion:**
```tsx
<nav aria-label="Paginacion de backings">
  <Button aria-label={`Ir a pagina ${page}`}>
    {page}
  </Button>
</nav>
```

### Screen Reader Announcements

```tsx
// Stats actualizados
<div role="status" aria-live="polite" className="sr-only">
  Estadisticas actualizadas. Total recaudado: {formatCurrency(total)}
</div>

// Tabla filtrada
<div role="status" aria-live="polite" className="sr-only">
  Mostrando {filteredCount} de {totalCount} backings
</div>

// Loading
<div role="status" aria-live="polite">
  <span className="sr-only">Cargando estadisticas...</span>
  <Skeleton />
</div>
```

### Focus Styles

```css
/* Agregar a globals.css */
*:focus-visible {
  outline: 2px solid hsl(var(--primary));
  outline-offset: 2px;
}

.button:focus-visible {
  ring: 2;
  ring-primary;
  ring-offset: 2;
}
```

---

## 8. Componentes shadcn Requeridos

### Ya Instalados
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

### Por Instalar
- ❌ Avatar, AvatarFallback, AvatarImage
- ❌ Tooltip, TooltipProvider, TooltipTrigger, TooltipContent

**Comando de Instalacion:**
```bash
cd src/admin
npx shadcn@latest add avatar tooltip
```

---

## 9. Iconos Lucide

### Iconos Necesarios

| Contexto | Icon | Import |
|----------|------|--------|
| Stats - Recaudado | `TrendingUp` | `lucide-react` |
| Stats - Backers | `Users` | `lucide-react` |
| Stats - Campanias | `Music` | `lucide-react` |
| Stats - Exito | `BarChart3` | `lucide-react` |
| Stats - Promedio | `DollarSign` | `lucide-react` |
| Stats - Dias | `Calendar` | `lucide-react` |
| Stats - Award | `Award` | `lucide-react` |
| Actions - Nueva | `PlusCircle` | `lucide-react` |
| Actions - Editar | `Edit` | `lucide-react` |
| Actions - Ver | `ExternalLink` | `lucide-react` |
| Actions - Compartir | `Share2` | `lucide-react` |
| Actions - Export | `Download` | `lucide-react` |
| Navigation - Volver | `ArrowLeft` | `lucide-react` |
| Navigation - Next | `ChevronRight` | `lucide-react` |
| Navigation - Prev | `ChevronLeft` | `lucide-react` |
| Navigation - Down | `ChevronDown` | `lucide-react` |
| Status - Success | `CheckCircle` | `lucide-react` |
| Status - Error | `AlertCircle` | `lucide-react` |
| Empty - Music | `Music` | `lucide-react` |
| Empty - Users | `Users` | `lucide-react` |
| Empty - Search | `Search` | `lucide-react` |

**Todos los iconos estan disponibles en el paquete `lucide-react` (ya instalado).**

---

## 10. Checklist de Implementacion

### Dashboard Principal (`/dashboard`)
- [ ] Layout con sidebar + header (ALREADY EXISTS)
- [ ] CompleteProfileBanner funcionando (ALREADY EXISTS)
- [ ] Stats cards grid (4 cols desktop, 2x2 tablet, 1 col mobile)
- [ ] StatsCard con datos reales de API (sin calculos client-side)
- [ ] Trend percentages funcionando
- [ ] Chart placeholder implementado
- [ ] Mis Campanias card con CampaignCard components
- [ ] CampaignCard con imagen gradient, titulo, estado badge, porcentaje
- [ ] Estado badges con colores correctos
- [ ] Recent Backings card con avatars (Avatar component)
- [ ] Empty states para no campanias, no backings
- [ ] Loading skeletons funcionando
- [ ] Error handling con Alert component
- [ ] Responsive layout probado en mobile, tablet, desktop
- [ ] Animaciones hover en cards
- [ ] Links funcionando: Nueva campania, Ver todas, etc.

### Campana Detail (`/campanias/[id]`)
- [ ] Layout con header + back button
- [ ] Stats cards con datos de API (4 cards)
- [ ] Progress bar full-width con porcentaje y montos
- [ ] Badge "Meta alcanzada" cuando >= 100%
- [ ] Backings recientes table (5 items)
- [ ] Table con avatars, reward names, formateo correcto
- [ ] Botones: Volver, Ver publico, Editar
- [ ] Link "Ver todos los backers" funcionando
- [ ] Empty state cuando no hay backings
- [ ] Loading state con skeletons
- [ ] Error handling
- [ ] Responsive layout
- [ ] Hover effects en table rows

### Backings List (`/campanias/[id]/backings`)
- [ ] Layout con header + back button
- [ ] Stats header (3 cards)
- [ ] Filtros: Search input funcionando (debounced)
- [ ] Filtros: Botones Todos, Con Reward, Anonimos
- [ ] Boton "Exportar CSV" implementado
- [ ] Table completa con todas las columnas
- [ ] Avatars con fallback funcionando
- [ ] Badges "Anonimo" en backings anonimos
- [ ] Tooltip en mensajes truncados (Tooltip component)
- [ ] Sortable headers (Backer, Monto, Fecha)
- [ ] Paginacion funcionando (20 per page)
- [ ] Numeros de pagina dinamicos
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

### Componentes Nuevos
- [ ] CampaignCard component creado
- [ ] EmptyState component reutilizable creado
- [ ] ProgressCard component creado
- [ ] BackingsTable component creado
- [ ] BackingsFilters component creado
- [ ] Pagination component creado
- [ ] RecentBackings mejorado con Avatar

### Instalacion de Dependencias
- [ ] Avatar component instalado (`npx shadcn@latest add avatar`)
- [ ] Tooltip component instalado (`npx shadcn@latest add tooltip`)
- [ ] Colores custom agregados a `globals.css`
- [ ] Keyframes de animacion agregados a `globals.css`

---

## 11. Notas de Implementacion

### Prioridad MVP

**MUST HAVE (P0):**
1. Dashboard principal con stats cards (datos API reales)
2. Mis Campanias list con CampaignCard
3. Recent Backings card con Avatar
4. Campana detail con stats y progress bar
5. Backings recientes table en detail
6. Empty states y loading states

**NICE TO HAVE (P1):**
1. Chart de recaudacion (placeholder "Proximamente" OK para MVP)
2. Filtros avanzados en backings list
3. Exportar CSV
4. Tooltips en mensajes
5. Sortable table headers

**FUTURE (P2):**
1. Graficas interactivas con drill-down
2. Dashboard comparativo (mes vs mes)
3. Notificaciones en tiempo real de nuevos backings

### Orden de Desarrollo Sugerido

1. **Instalar componentes faltantes**
   ```bash
   cd src/admin
   npx shadcn@latest add avatar tooltip
   ```

2. **Crear componentes base reutilizables**
   - EmptyState (usado en multiples pantallas)
   - CampaignCard
   - ProgressCard

3. **Mejorar Dashboard Principal**
   - Agregar CampaignCard en lugar de lista simple
   - Mejorar RecentBackings con Avatar
   - Chart placeholder

4. **Crear Campana Detail**
   - Layout base con stats cards
   - Progress bar
   - Backings table

5. **Crear Backings List Full**
   - Stats header
   - Filtros y busqueda
   - Table completa
   - Paginacion

6. **Polish**
   - Animaciones
   - Responsive
   - Accesibilidad

### Consideraciones de Datos

**Client-side vs Server-side:**
- ✅ **Server-side:** Total backers, stats aggregations (usar endpoints API)
- ❌ **Client-side:** Evitar calculos de stats en frontend (usar datos de API)

**React Query:**
- Configurar `staleTime: 30000` (30s) para evitar refetch excesivo
- Usar `refetchInterval` solo en dashboard principal (stats actualizados)
- No usar en listas con paginacion (refetch manual en mutaciones)

**Paginacion:**
- Client-side: OK para < 100 items
- Server-side: Necesario para > 100 items (implementar query params)

---

## 12. Referencias

### Templates Dashtail

Consultar para inspiracion de diseno:
```
references/templates/dashtail/
├── pages/
│   ├── dashboard.html         # Stats cards layout
│   ├── analytics.html         # Charts y metricas
│   ├── tables.html            # Table styles y paginacion
└── components/
    ├── cards.html             # Card variations
    └── buttons.html           # Button styles
```

**Adaptaciones:**
- Convertir HTML a React components
- Usar shadcn/ui en lugar de Tailwind custom
- Adaptar color scheme a pink/purple de WePlay Rises

### Shared Utils

Funciones ya existentes en `src/shared/utils/format.ts`:
- `formatCurrency(amount, currency?)`
- `formatRelativeDate(date)`
- `formatDate(date)`
- `calculatePercentage(current, total)`
- `getDaysRemaining(endDate)`
- `getBackerDisplayName(esAnonimo, userName?)`

### Componentes Existentes (Admin)

Reutilizar:
- `<StatsCard>` (ALREADY EXISTS en `src/admin/src/components/dashboard/stats-card.tsx`)
- `<CompleteProfileBanner>` (ALREADY EXISTS)
- Sidebar layout (ALREADY EXISTS)
- Header layout (ALREADY EXISTS)

---

**Fin del Plan de Diseno UI**

**Total de Componentes Nuevos:** 7
- CampaignCard
- EmptyState
- ProgressCard
- BackingsTable
- BackingsFilters
- Pagination
- RecentBackingCard (mejora del existente)

**Componentes shadcn a Instalar:** 2
- Avatar
- Tooltip

**Estimacion de Tiempo de Implementacion UI:** 8-10 horas (sin backend)
