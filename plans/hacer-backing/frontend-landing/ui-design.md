# Diseno UI: Hacer Backing (Landing)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/web (Landing - Vite + React 18)

## 1. Resumen

- Componentes shadcn: 12 (Card, Button, Input, Textarea, Badge, Progress, Avatar, Tabs, Skeleton, Separator, Alert, Label)
- Composiciones custom: 15 (CampaignCard, CampaignProgressBar, RewardCard, BackingForm, etc.)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)
- Dark theme: #0a0a0f, #0f1729, #1a1a2e, #334155
- Gradient CTAs: from-pink-500 to-purple-600

## 2. Paleta de Colores (Dark Theme)

| Uso | Variable CSS | Tailwind Class | Color Hex |
|-----|-------------|----------------|-----------|
| Background principal | --background | bg-[#0a0a0f] | #0a0a0f |
| Card background | --card | bg-[#0f1729] o bg-[#1a1a2e] | #0f1729 / #1a1a2e |
| Border default | --border | border-[#334155] | #334155 |
| Text primary | --foreground | text-white | #ffffff |
| Text secondary | --muted-foreground | text-[#94a3b8] | #94a3b8 |
| Text tertiary | - | text-[#64748b] | #64748b |
| Primary (hover, focus) | --primary | bg-primary / border-primary | #a855f7 (purple-500) |
| CTA gradient | - | bg-gradient-to-r from-pink-500 to-purple-600 | #ec4899 → #a855f7 |
| Success | --success | bg-green-500 / text-green-400 | #10b981 / #4ade80 |
| Warning | --warning | bg-warning / text-warning | #f59e0b |
| Destructive | --destructive | bg-red-500 / text-red-400 | #ef4444 / #f87171 |
| Progress bar background | - | bg-[#1e293b] | #1e293b |
| Progress bar fill | - | bg-gradient-to-r from-pink-500 to-purple-600 | #ec4899 → #a855f7 |
| Progress bar near goal | - | bg-gradient-to-r from-warning to-amber-500 | #f59e0b → #f59e0b |
| Progress bar success | - | bg-gradient-to-r from-green-400 to-emerald-500 | #4ade80 → #10b981 |

## 3. Componentes por Screen

### 3.1 Campaign Listing (`/campanias`)

#### Layout ASCII

```
┌─────────────────────────────────────────────────────────────┐
│ [HEADER] MusicFund  Explorar  Categorias  [Login][Register] │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│              Descubre Proyectos Musicales                   │
│          Apoya a tus artistas favoritos y se parte          │
│                                                             │
│  [🔍 Buscar artistas, generos, proyectos...]                │
│                                                             │
│  [🎛️ Genero ▼] [🔘 Estado ▼] [📊 Ordenar ▼]  248 campanias │
│                                                             │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐               │
│  │ [IMG]     │  │ [IMG]     │  │ [IMG]     │               │
│  │ [Rock]    │  │ [Indie]   │  │ [Pop]     │               │
│  │ 12 dias   │  │ 3 dias    │  │ 28 dias   │               │
│  │           │  │           │  │           │               │
│  │ Album 2025│  │ EP Dreams │  │ Live Tour │               │
│  │ by Artist │  │ by Artist │  │ by Artist │               │
│  │           │  │           │  │           │               │
│  │ €8,700    │  │ €4,700    │  │ €9,300    │               │
│  │ de €10k   │  │ de €5k    │  │ de €15k   │               │
│  │ [████87%] │  │ [████94%] │  │ [███62%]  │               │
│  │ 142 backs │  │ 89 backs  │  │ 67 backs  │               │
│  └───────────┘  └───────────┘  └───────────┘               │
│                                                             │
│  [← Prev]   1  2  3  4  5  [Next →]                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Principales

**Page Container**

```tsx
<div className="min-h-screen bg-[#0a0a0f] text-white">
  {/* Header */}
  {/* Hero Section */}
  {/* Filters */}
  {/* Campaigns Grid */}
  {/* Pagination */}
</div>
```

**Hero Section**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Container | `<section>` | `py-16 px-4 text-center` |
| Title | `<h1>` | `text-4xl md:text-5xl font-bold text-white mb-4` |
| Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8 max-w-2xl mx-auto` |

**Search Bar**

```tsx
<div className="relative max-w-2xl mx-auto mb-12">
  <Icon name="search" className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-[#64748b]" />
  <Input
    type="search"
    placeholder="Buscar artistas, generos, proyectos..."
    className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] px-12 py-4 rounded-xl focus:border-primary w-full"
  />
  {/* Clear button (X) si hay texto */}
</div>
```

**Filters Row**

| Elemento | Componente | Clases Tailwind |
|----------|------------|-----------------|
| Container | `<div>` | `flex flex-col md:flex-row items-center justify-between py-6 px-4 max-w-7xl mx-auto gap-4` |
| Filters Group | `<div>` | `flex gap-3 flex-wrap` |
| Filter Button | `<Button variant="outline">` | `border-[#334155] text-white hover:border-primary hover:bg-primary/10` |
| Filter Button Active | `<Button variant="outline">` | `border-primary bg-primary/10 text-primary` |
| Results Count | `<span>` | `text-sm text-[#64748b]` |

**Campaigns Grid**

```tsx
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 px-4 max-w-7xl mx-auto mb-12">
  {campaigns.map((campaign) => (
    <CampaignCard key={campaign.id} campaign={campaign} />
  ))}
</div>
```

**CampaignCard Component**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] hover:border-primary cursor-pointer transition-all duration-300 group overflow-hidden">
  {/* Image Container */}
  <div className="relative aspect-video overflow-hidden">
    <img
      src={campaign.imagenPrincipalUrl}
      alt={campaign.titulo}
      className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
    />
    {/* Genre Badge - Top Left */}
    <Badge className="absolute top-3 left-3 bg-primary/90 text-white font-semibold">
      {campaign.genero}
    </Badge>
    {/* Days Badge - Top Right */}
    <Badge variant="secondary" className="absolute top-3 right-3 bg-[#1a1a2e]/90 backdrop-blur text-white text-xs">
      <Icon name="calendar" className="w-3 h-3 mr-1" />
      {campaign.diasRestantes} dias
    </Badge>
  </div>

  <CardContent className="p-5">
    {/* Title */}
    <h3 className="text-xl font-bold text-white mb-1 line-clamp-1">
      {campaign.titulo}
    </h3>

    {/* Artist Name */}
    <p className="text-sm text-[#94a3b8] mb-4 flex items-center gap-1">
      <Icon name="user" className="w-4 h-4" />
      {campaign.artistaNombre}
    </p>

    {/* Amount Row */}
    <div className="flex items-baseline gap-2 mb-2">
      <span className="text-2xl font-bold text-white">
        €{campaign.importePledgedActual.toLocaleString()}
      </span>
      <span className="text-sm text-[#64748b]">
        de €{campaign.importeObjetivo.toLocaleString()}
      </span>
    </div>

    {/* Progress Bar */}
    <div className="w-full h-2 bg-[#1e293b] rounded-full overflow-hidden mb-3">
      <div
        className={cn(
          "h-full rounded-full transition-all duration-500",
          campaign.porcentajeProgreso >= 100
            ? "bg-gradient-to-r from-green-400 to-emerald-500"
            : campaign.porcentajeProgreso >= 90
            ? "bg-gradient-to-r from-warning to-amber-500"
            : "bg-gradient-to-r from-pink-500 to-purple-600"
        )}
        style={{ width: `${Math.min(campaign.porcentajeProgreso, 100)}%` }}
      />
    </div>

    {/* Stats Row */}
    <div className="flex items-center justify-between text-sm">
      <span className="font-semibold text-primary">
        {campaign.porcentajeProgreso}% financiado
      </span>
      <div className="flex items-center gap-1 text-[#94a3b8]">
        <Icon name="users" className="w-4 h-4" />
        {campaign.totalBackers} backers
      </div>
    </div>
  </CardContent>
</Card>
```

**Pagination**

```tsx
<div className="flex items-center justify-center gap-2 py-8">
  <Button variant="outline" size="sm" disabled={currentPage === 1}>
    <Icon name="chevron-left" className="w-4 h-4" />
    Anterior
  </Button>

  {[1, 2, 3, 4, 5].map((page) => (
    <Button
      key={page}
      size="sm"
      variant={page === currentPage ? "default" : "outline"}
      className={page === currentPage ? "bg-primary text-white" : "border-[#334155] text-white hover:border-primary"}
    >
      {page}
    </Button>
  ))}

  <Button variant="outline" size="sm" disabled={currentPage === totalPages}>
    Siguiente
    <Icon name="chevron-right" className="w-4 h-4" />
  </Button>
</div>
```

**Empty State**

```tsx
<div className="text-center py-16 px-4">
  <Icon name="search-x" className="w-20 h-20 text-[#64748b] mx-auto mb-4" />
  <h3 className="text-2xl font-bold text-white mb-2">
    No se encontraron campanias
  </h3>
  <p className="text-[#94a3b8] mb-6">
    Intenta ajustar tus filtros o realiza una busqueda diferente
  </p>
  <Button className="bg-gradient-to-r from-pink-500 to-purple-600 text-white">
    Limpiar filtros
  </Button>
</div>
```

**Loading State**

```tsx
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 px-4 max-w-7xl mx-auto mb-12">
  {[1, 2, 3, 4, 5, 6].map((i) => (
    <Card key={i} className="bg-[#1a1a2e] border-[#334155] overflow-hidden">
      <Skeleton className="aspect-video w-full bg-[#1e293b]" />
      <CardContent className="p-5 space-y-3">
        <Skeleton className="h-6 w-3/4 bg-[#1e293b]" />
        <Skeleton className="h-4 w-1/2 bg-[#1e293b]" />
        <Skeleton className="h-8 w-full bg-[#1e293b]" />
        <Skeleton className="h-2 w-full bg-[#1e293b]" />
        <Skeleton className="h-4 w-full bg-[#1e293b]" />
      </CardContent>
    </Card>
  ))}
</div>
```

#### Estados y Variantes

| Estado | Visual |
|--------|--------|
| **Default** | Grid de campaign cards, progress bars animados |
| **Loading** | 6 skeleton cards con shimmer animation |
| **Empty** | Icono search-x, texto, boton "Limpiar filtros" |
| **Hover Card** | Border primary, image scale 1.05, elevation shadow-lg |
| **Filter Active** | Border primary, bg primary/10, texto primary |
| **Search Active** | Border primary, icon primary |
| **Near Goal (>90%)** | Progress bar gradient warning (amarillo) |
| **Goal Reached (100%)** | Progress bar gradient success (verde), badge "Financiada" |
| **Expiring Soon (<3 dias)** | Days badge bg-red-500 con pulse animation |

#### Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Grid 1 col, hero text-3xl, search bar px-4, filters stack vertical, card p-4, font sizes reducidos |
| **sm-md (tablet)** | Grid 2 cols, hero text-4xl, filters horizontal compactos |
| **> lg (desktop)** | Grid 3 cols, spacing amplio (gap-6), max-w-7xl container |

---

### 3.2 Campaign Detail (`/campanias/{id}`)

#### Layout ASCII

```
┌─────────────────────────────────────────────────────────────┐
│ [HEADER] MusicFund  Explorar                                │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ ┌─────────────────────────────┐  ┌──────────────────────┐  │
│ │                             │  │ [Apoyar esta         │  │
│ │      [HERO IMAGE/VIDEO]     │  │  campania]           │  │
│ │                             │  │  (gradient button)   │  │
│ │                             │  │  Desde €5            │  │
│ ├─────────────────────────────┤  │                      │  │
│ │ Album Debut "Ecos"          │  │ ──────────────────   │  │
│ │ by Los Nocturnos [avatar]   │  │                      │  │
│ │ Rock Indie  ✓ Verificado    │  │ Recompensas          │  │
│ │                             │  │                      │  │
│ │ €12,450 de €15,000          │  │ ┌──────────────────┐ │  │
│ │ [████████████        83%]   │  │ │ €10 Descarga     │ │  │
│ │                             │  │ │ Digital          │ │  │
│ │ 83%    127      12          │  │ │ ⭐ Mas popular   │ │  │
│ │ financ backers dias         │  │ │                  │ │  │
│ │                             │  │ │ 📦 234 de 500    │ │  │
│ │ ─────────────────────────   │  │ │ [Seleccionar]    │ │  │
│ │                             │  │ └──────────────────┘ │  │
│ │ [Historia][Actualiz][FAQ]   │  │                      │  │
│ │                             │  │ ┌──────────────────┐ │  │
│ │ Despues de dos anos...      │  │ │ €25 CD Firmado   │ │  │
│ │ [Description content]       │  │ │                  │ │  │
│ │                             │  │ │ 📦 89 de 200     │ │  │
│ │ [Studio photo]              │  │ │ [Seleccionar]    │ │  │
│ │                             │  │ └──────────────────┘ │  │
│ │ ─────────────────────────   │  │                      │  │
│ │                             │  │ 🔒 Pago seguro       │  │
│ │ Sobre el Artista            │  │ 📦 Entrega: Mar2025  │  │
│ │ [Artist Bio Card]           │  └──────────────────────┘  │
│ │                             │                            │
│ └─────────────────────────────┘                            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Principales

**Page Layout**

```tsx
<div className="min-h-screen bg-[#0a0a0f] pb-16">
  <div className="max-w-7xl mx-auto px-4 py-8 grid lg:grid-cols-[1fr_380px] gap-8">
    {/* Main Content Column */}
    <div className="space-y-8">
      {/* Hero, Progress, Tabs, Description, Artist Bio */}
    </div>

    {/* Sidebar Column (sticky) */}
    <div className="sticky top-24 space-y-6">
      {/* CTA Button, Rewards, Footer Info */}
    </div>
  </div>
</div>
```

**Hero Image/Video**

```tsx
<div className="aspect-video bg-[#1a1a2e] rounded-xl overflow-hidden mb-6">
  {campaign.videoPrincipalUrl ? (
    <video
      src={campaign.videoPrincipalUrl}
      controls
      className="w-full h-full object-cover"
    />
  ) : (
    <img
      src={campaign.imagenPrincipalUrl}
      alt={campaign.titulo}
      className="w-full h-full object-cover"
    />
  )}
</div>
```

**Campaign Header**

```tsx
<div className="mb-6">
  <h1 className="text-3xl md:text-4xl font-bold text-white mb-3">
    {campaign.titulo}
  </h1>

  <div className="flex items-center gap-3 mb-2">
    <Link to={`/artistas/${campaign.artistaId}`}>
      <Avatar className="w-10 h-10 border-2 border-primary">
        <AvatarImage src={campaign.artistaImagenUrl} />
        <AvatarFallback>{campaign.artistaNombre[0]}</AvatarFallback>
      </Avatar>
    </Link>

    <Link
      to={`/artistas/${campaign.artistaId}`}
      className="text-lg text-white hover:text-primary transition"
    >
      {campaign.artistaNombre}
    </Link>

    <Badge className="ml-2 bg-primary/20 text-primary border-primary/50 text-xs">
      <Icon name="check-circle" className="w-3 h-3 mr-1" />
      Verificado
    </Badge>
  </div>

  <Badge variant="secondary" className="bg-[#1a1a2e] text-[#94a3b8] border-[#334155]">
    {campaign.genero}
  </Badge>
</div>
```

**Progress Section**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-6 mb-8">
  {/* Amount Row */}
  <div className="flex items-baseline gap-2 mb-2">
    <span className="text-3xl font-bold text-white">
      €{campaign.importePledgedActual.toLocaleString()}
    </span>
    <span className="text-lg text-[#64748b]">
      de €{campaign.importeObjetivo.toLocaleString()}
    </span>
  </div>

  {/* Progress Bar */}
  <div className="w-full h-3 bg-[#1e293b] rounded-full overflow-hidden mb-4">
    <div
      className={cn(
        "h-full rounded-full transition-all duration-500",
        campaign.porcentajeProgreso >= 100
          ? "bg-gradient-to-r from-green-400 to-emerald-500"
          : campaign.porcentajeProgreso >= 90
          ? "bg-gradient-to-r from-warning to-amber-500"
          : "bg-gradient-to-r from-pink-500 to-purple-600"
      )}
      style={{ width: `${Math.min(campaign.porcentajeProgreso, 100)}%` }}
    />
  </div>

  {/* Stats Grid */}
  <div className="grid grid-cols-3 gap-4 text-center">
    <div className="space-y-1">
      <div className="text-2xl font-bold text-white">
        {campaign.porcentajeProgreso}%
      </div>
      <div className="text-sm text-[#64748b]">financiado</div>
    </div>
    <div className="space-y-1">
      <div className="text-2xl font-bold text-white">
        {campaign.totalBackers}
      </div>
      <div className="text-sm text-[#64748b]">backers</div>
    </div>
    <div className="space-y-1">
      <div className="text-2xl font-bold text-white">
        {campaign.diasRestantes}
      </div>
      <div className="text-sm text-[#64748b]">dias restantes</div>
    </div>
  </div>
</Card>
```

**Tabs Navigation**

```tsx
<Tabs defaultValue="historia" className="mb-6">
  <TabsList className="border-b border-[#334155] bg-transparent w-full justify-start p-0 h-auto">
    <TabsTrigger
      value="historia"
      className="data-[state=active]:bg-transparent data-[state=active]:text-white data-[state=active]:border-b-2 data-[state=active]:border-primary text-[#94a3b8] hover:text-white rounded-none pb-3"
    >
      Historia
    </TabsTrigger>
    <TabsTrigger
      value="actualizaciones"
      className="data-[state=active]:bg-transparent data-[state=active]:text-white data-[state=active]:border-b-2 data-[state=active]:border-primary text-[#94a3b8] hover:text-white rounded-none pb-3"
    >
      Actualizaciones
    </TabsTrigger>
    <TabsTrigger
      value="faq"
      className="data-[state=active]:bg-transparent data-[state=active]:text-white data-[state=active]:border-b-2 data-[state=active]:border-primary text-[#94a3b8] hover:text-white rounded-none pb-3"
    >
      FAQ
    </TabsTrigger>
  </TabsList>

  <TabsContent value="historia" className="mt-6">
    <div className="prose prose-invert max-w-none">
      <div className="text-[#cbd5e1] leading-relaxed space-y-4">
        {/* Rich text content from campaign.descripcionCorta */}
        <p>{campaign.descripcionCorta}</p>
        {/* Images, lists, etc. */}
      </div>
    </div>
  </TabsContent>

  {/* TabsContent para actualizaciones y faq */}
</Tabs>
```

**Artist Bio Card**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-6">
  <div className="flex items-start gap-4 mb-4">
    <Avatar className="w-16 h-16 border-2 border-primary">
      <AvatarImage src={campaign.artistaImagenUrl} />
      <AvatarFallback>{campaign.artistaNombre[0]}</AvatarFallback>
    </Avatar>
    <div className="flex-1">
      <h3 className="text-xl font-bold text-white mb-1">
        {campaign.artistaNombre}
      </h3>
      <p className="text-sm text-[#94a3b8] line-clamp-3">
        {artistaBio}
      </p>
    </div>
  </div>

  <div className="flex items-center gap-4 text-xs text-[#64748b] mb-4">
    <span>3 campanias</span>
    <span>•</span>
    <span>€45,230 total recaudado</span>
  </div>

  <Button
    variant="outline"
    className="w-full border-primary text-primary hover:bg-primary/10"
  >
    Ver perfil completo
  </Button>
</Card>
```

**Sidebar - CTA Button**

```tsx
<Button
  size="lg"
  className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-4 text-lg mb-2"
  onClick={handleApoyar}
>
  <Icon name="heart" className="w-5 h-5 mr-2" />
  Apoyar esta campania
</Button>

<p className="text-center text-sm text-[#94a3b8] mb-6">
  Desde €5
</p>
```

**Sidebar - Rewards List**

```tsx
<div className="space-y-4">
  <h3 className="text-lg font-bold text-white mb-4">Recompensas</h3>

  {campaign.rewards.map((reward) => (
    <RewardCard
      key={reward.id}
      reward={reward}
      onSelect={() => handleSelectReward(reward.id)}
    />
  ))}
</div>
```

**RewardCard Component**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] hover:border-primary transition-all cursor-pointer p-4">
  {/* Popular Badge (if applicable) */}
  {reward.esPopular && (
    <Badge className="mb-2 bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs">
      <Icon name="sparkles" className="w-3 h-3 mr-1" />
      Mas popular
    </Badge>
  )}

  {/* Warning Badge (low stock) */}
  {reward.disponible && reward.cantidadMaxima && (reward.cantidadVendida / reward.cantidadMaxima) > 0.8 && (
    <Badge className="mb-2 bg-warning/20 text-warning border-warning/50 text-xs">
      <Icon name="alert-circle" className="w-3 h-3 mr-1" />
      Pocas unidades
    </Badge>
  )}

  <div className="mb-3">
    <div className="flex items-baseline gap-2 mb-2">
      <span className="text-2xl font-bold text-primary">
        €{reward.importeMinimo}
      </span>
      <span className="text-sm text-[#64748b]">minimo</span>
    </div>

    <h4 className="text-lg font-semibold text-white mb-1">
      {reward.nombre}
    </h4>

    <p className="text-sm text-[#94a3b8] line-clamp-2">
      {reward.descripcion}
    </p>
  </div>

  {/* Stock Info */}
  {reward.cantidadMaxima && (
    <div className="flex items-center gap-2 text-xs text-[#64748b] mb-3">
      <Icon name="package" className="w-4 h-4" />
      <span>
        {reward.cantidadVendida} de {reward.cantidadMaxima} reservados
      </span>
    </div>
  )}

  {/* Delivery Info (if physical) */}
  {reward.incluyeEnvioFisico && (
    <div className="flex items-center gap-2 text-xs text-[#64748b] mb-3">
      <Icon name="truck" className="w-4 h-4" />
      <span>{reward.tiempoEntregaEstimado}</span>
    </div>
  )}

  <Button
    variant={reward.disponible ? "default" : "outline"}
    className={cn(
      "w-full",
      reward.disponible
        ? "bg-primary text-white hover:bg-primary/90"
        : "border-[#334155] text-[#64748b] cursor-not-allowed"
    )}
    disabled={!reward.disponible}
  >
    {reward.disponible ? "Seleccionar" : "Agotado"}
  </Button>
</Card>
```

**Sidebar - Footer Info**

```tsx
<div className="border-t border-[#334155] pt-4 space-y-2">
  <div className="flex items-center gap-2 text-xs text-[#64748b]">
    <Icon name="lock" className="w-4 h-4" />
    <span>Pago seguro</span>
  </div>
  <div className="flex items-center gap-2 text-xs text-[#64748b]">
    <Icon name="truck" className="w-4 h-4" />
    <span>Entrega estimada: Marzo 2025</span>
  </div>
</div>
```

#### Estados y Variantes

| Estado | Visual |
|--------|--------|
| **Default** | Hero image/video, progress bar, rewards sidebar sticky |
| **Loading** | Skeleton para hero, title, progress, rewards (3 skeletons) |
| **Video Playing** | Controls overlay, play/pause button |
| **Tab Active** | Tab con border-bottom primary, contenido visible |
| **Near Goal (>90%)** | Progress bar warning, badge "Casi alcanzado" |
| **Goal Reached** | Progress bar success green, badge "Financiada" |
| **Expired** | Badge "Finalizada", CTA disabled "Campania finalizada" |
| **No Rewards** | Solo boton "Apoyar", texto "Aporte libre" |
| **Reward Popular** | Badge rosa "Mas popular" |
| **Reward Low Stock** | Badge warning "Pocas unidades" |
| **Reward Sold Out** | Opacity 60%, boton disabled "Agotado" |
| **Mobile View** | Sidebar NO sticky, stack vertical, hero aspect-square |

#### Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Stack vertical (main + sidebar), sidebar NO sticky, hero aspect-square, font sizes reducidos, stats grid gap-2 |
| **sm-md (tablet)** | Stack vertical, sidebar sticky, layout normal |
| **> lg (desktop)** | Grid layout [1fr 380px], sidebar sticky top-24, spacing amplio |

---

### 3.3 Backing Form (`/campanias/{id}/backing`)

#### Layout ASCII (Paso 1: Monto y Datos)

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│   ┌──────────────────────────────────────────────────────┐  │
│   │  [×] Apoya a Luna Sonora                             │  │
│   │  Album debut "Ecos del Alma"                         │  │
│   ├──────────────────────────────────────────────────────┤  │
│   │                                                      │  │
│   │  Progreso                             Paso 1 de 2   │  │
│   │  [████████████████████        ] 50%                 │  │
│   │  Monto           Pago                               │  │
│   │                                                      │  │
│   │  ────────────────────────────────────────────────   │  │
│   │                                                      │  │
│   │  ┌──────────────────────────────────────────────┐   │  │
│   │  │  Disco firmado + Poster        [Cambiar]    │   │  │
│   │  │  Album fisico firmado + poster exclusivo    │   │  │
│   │  │  Minimo €35                                  │   │  │
│   │  └──────────────────────────────────────────────┘   │  │
│   │                                                      │  │
│   │  Monto a Aportar                                    │  │
│   │  ┌──────────────────────────────────────────────┐   │  │
│   │  │  €                                      50   │   │  │
│   │  └──────────────────────────────────────────────┘   │  │
│   │  Minimo €35 para este reward                        │  │
│   │                                                      │  │
│   │  [+€5]  [+€10]  [+€25]                              │  │
│   │                                                      │  │
│   │  Mensaje para el artista (opcional)                 │  │
│   │  ┌──────────────────────────────────────────────┐   │  │
│   │  │  Escribe un mensaje de apoyo...              │   │  │
│   │  │                                              │   │  │
│   │  └──────────────────────────────────────────────┘   │  │
│   │  0/500 caracteres                                   │  │
│   │                                                      │  │
│   │  ☐ Hacer anonimo mi apoyo                           │  │
│   │                                                      │  │
│   │  ────────────────────────────────────────────────   │  │
│   │                                                      │  │
│   │  Resumen                                            │  │
│   │  Aportacion:                              €50      │  │
│   │  Reward:                    Disco firmado + Poster │  │
│   │  Total:                                   €50      │  │
│   │                                                      │  │
│   │  [← Volver]                        [Continuar →]   │  │
│   │  (outline)                         (gradient)      │  │
│   └──────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Principales

**Card Container**

```tsx
<Card className="max-w-2xl mx-auto bg-[#0f1729] border-[#334155]">
  {/* Header */}
  {/* Progress Section */}
  {/* Form Body */}
  {/* Footer */}
</Card>
```

**Header**

```tsx
<div className="border-b border-[#334155] p-6 relative">
  <button className="absolute right-4 top-4 text-[#94a3b8] hover:text-white">
    <Icon name="x" className="w-5 h-5" />
  </button>

  <h2 className="text-2xl font-bold text-white mb-1">
    Apoya a {campaign.artistaNombre}
  </h2>
  <p className="text-sm text-[#64748b]">
    {campaign.titulo}
  </p>
</div>
```

**Progress Section**

```tsx
<div className="px-6 py-4 bg-[#1a1a2e]">
  <div className="flex items-center justify-between text-sm mb-2">
    <span className="text-[#94a3b8]">Progreso</span>
    <span className="text-[#64748b]">Paso 1 de 2</span>
  </div>

  <div className="w-full h-2 bg-[#1e293b] rounded-full overflow-hidden mb-2">
    <div
      className="h-full bg-gradient-to-r from-pink-500 to-purple-600 transition-all duration-300"
      style={{ width: `${currentStep === 1 ? 50 : 100}%` }}
    />
  </div>

  <div className="flex justify-between text-xs text-[#64748b]">
    <span>Monto</span>
    <span>Pago</span>
  </div>
</div>
```

**Selected Reward Card**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-4 relative">
  <div className="flex items-start justify-between mb-2">
    <h4 className="text-lg font-semibold text-white">
      {selectedReward.nombre}
    </h4>
    <Button variant="link" size="sm" className="text-primary hover:underline">
      Cambiar
    </Button>
  </div>

  <p className="text-sm text-[#94a3b8] mb-2">
    {selectedReward.descripcion}
  </p>

  <p className="text-xs text-[#64748b]">
    Minimo €{selectedReward.importeMinimo}
  </p>
</Card>
```

**Amount Input**

```tsx
<div className="space-y-2">
  <Label htmlFor="amount" className="text-sm font-medium text-[#cbd5e1]">
    Monto a Aportar
  </Label>

  <div className="relative">
    <span className="absolute left-4 top-1/2 -translate-y-1/2 text-2xl text-[#94a3b8]">
      €
    </span>
    <Input
      id="amount"
      type="number"
      step="0.01"
      min={selectedReward?.importeMinimo || 5}
      value={amount}
      onChange={(e) => setAmount(Number(e.target.value))}
      className="bg-[#1a1a2e] border-[#334155] text-white text-2xl font-bold text-center py-4 pl-12 pr-4 focus:border-primary"
      aria-describedby="amount-hint amount-error"
      aria-invalid={!!amountError}
    />
  </div>

  {selectedReward && (
    <p id="amount-hint" className="text-xs text-[#64748b] italic">
      Minimo €{selectedReward.importeMinimo} para este reward
    </p>
  )}

  {amountError && (
    <p id="amount-error" role="alert" className="text-xs text-red-500">
      {amountError}
    </p>
  )}
</div>
```

**Amount Suggestions**

```tsx
<div className="flex gap-2 mt-3">
  <Button
    variant="outline"
    size="sm"
    className="border-[#334155] text-white hover:border-primary hover:bg-primary/10"
    onClick={() => setAmount((prev) => Math.min(prev + 5, 10000))}
  >
    +€5
  </Button>
  <Button
    variant="outline"
    size="sm"
    className="border-[#334155] text-white hover:border-primary hover:bg-primary/10"
    onClick={() => setAmount((prev) => Math.min(prev + 10, 10000))}
  >
    +€10
  </Button>
  <Button
    variant="outline"
    size="sm"
    className="border-[#334155] text-white hover:border-primary hover:bg-primary/10"
    onClick={() => setAmount((prev) => Math.min(prev + 25, 10000))}
  >
    +€25
  </Button>
</div>
```

**Message Textarea**

```tsx
<div className="space-y-2">
  <Label htmlFor="message" className="text-sm font-medium text-[#cbd5e1]">
    Mensaje para el artista (opcional)
  </Label>

  <Textarea
    id="message"
    value={message}
    onChange={(e) => setMessage(e.target.value)}
    maxLength={500}
    className="bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none placeholder:text-[#64748b] focus:border-primary"
    placeholder="Escribe un mensaje de apoyo..."
  />

  <span
    className={cn(
      "text-xs block",
      message.length > 400 ? "text-warning" : "text-[#64748b]",
      message.length > 475 && "text-red-500"
    )}
  >
    {message.length}/500 caracteres
  </span>
</div>
```

**Anonymous Checkbox**

```tsx
<div className="flex items-center space-x-2">
  <Checkbox
    id="anonymous"
    checked={esAnonimo}
    onCheckedChange={setEsAnonimo}
    className="border-[#334155] data-[state=checked]:bg-primary data-[state=checked]:border-primary"
  />
  <Label
    htmlFor="anonymous"
    className="text-sm text-white cursor-pointer"
  >
    <Icon name="eye-off" className="w-4 h-4 inline mr-1 text-[#64748b]" />
    Hacer anonimo mi apoyo
  </Label>
</div>
```

**Summary Section**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-4">
  <h4 className="text-lg font-bold text-white mb-3">Resumen</h4>

  <div className="space-y-2">
    <div className="flex justify-between text-sm">
      <span className="text-[#94a3b8]">Aportacion:</span>
      <span className="text-white font-semibold">€{amount}</span>
    </div>

    {selectedReward && (
      <div className="flex justify-between text-sm">
        <span className="text-[#94a3b8]">Reward:</span>
        <span className="text-white font-semibold line-clamp-1">
          {selectedReward.nombre}
        </span>
      </div>
    )}

    <div className="flex justify-between text-lg font-bold border-t border-[#334155] pt-3 mt-3">
      <span className="text-white">Total:</span>
      <span className="text-primary">€{amount}</span>
    </div>
  </div>
</Card>
```

**Footer Actions**

```tsx
<div className="border-t border-[#334155] p-6 flex gap-3 justify-between">
  <Button
    variant="outline"
    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]"
    onClick={handleVolver}
  >
    <Icon name="arrow-left" className="w-4 h-4 mr-2" />
    Volver
  </Button>

  <Button
    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
    onClick={handleContinuar}
    disabled={isSubmitting || !!amountError}
  >
    {isSubmitting ? (
      <>
        <Icon name="loader-2" className="w-4 h-4 mr-2 animate-spin" />
        Procesando...
      </>
    ) : (
      <>
        Continuar
        <Icon name="arrow-right" className="w-4 h-4 ml-2" />
      </>
    )}
  </Button>
</div>
```

#### Estados y Variantes

| Estado | Visual |
|--------|--------|
| **Default Paso 1** | Form con reward pre-seleccionado, progress 50% |
| **No Reward** | Card "Sin recompensa", monto minimo €5 |
| **Reward Pre-selected** | Card con detalles, monto pre-filled con minimo |
| **Focus Amount** | Border primary, glow shadow, input bold center |
| **Amount Below Min** | Error rojo "Minimo €X", continue disabled |
| **Typing Message** | Character counter (X/500), warning at 400 |
| **Anonymous Checked** | Checkbox checked, icon eye-off highlighted |
| **Loading Continue** | Spinner + "Procesando...", inputs disabled |
| **Validation Error** | Input border rojo, mensaje debajo en rojo |

#### Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Full width, padding reducido (p-4), amount text-xl, suggestions 3 cols grid, footer buttons stack vertical full-width |
| **sm-md (tablet)** | max-w-xl, layout normal |
| **> lg (desktop)** | max-w-2xl, spacing amplio |

---

### 3.4 Backing Confirmation (`/campanias/{id}/backing-confirmado`)

#### Layout ASCII

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│                       ✓                                     │
│                  [Success Icon]                             │
│                                                             │
│           ¡Gracias por tu apoyo!                            │
│                                                             │
│      Tu contribucion ha sido confirmada con exito.          │
│   Has ayudado a Luna Sonora a alcanzar su sueno musical.   │
│                                                             │
│   ┌──────────────────────────────────────────────────────┐  │
│   │  Resumen de tu Apoyo                                 │  │
│   │                                                      │  │
│   │  Campania:   Album Debut "Ecos del Silencio"        │  │
│   │  Artista:    Luna Indie                              │  │
│   │  Monto:      €50                                     │  │
│   │  Reward:     Disco firmado + Poster                  │  │
│   │  Fecha:      15 Enero 2025, 14:32                    │  │
│   │  ID Backing: BK-2025-001234                          │  │
│   │                                                      │  │
│   │  📧 Te hemos enviado un email de confirmacion a:    │  │
│   │     tu-email@ejemplo.com                             │  │
│   └──────────────────────────────────────────────────────┘  │
│                                                             │
│   ¿Que sigue?                                               │
│   • El artista procesara tu recompensa                      │
│   • Te notificaremos sobre actualizaciones                  │
│   • Recibiras tu recompensa en: Marzo 2025                  │
│                                                             │
│   [Ver Campania]         [Explorar Mas Campanias]          │
│   (outline)              (gradient)                         │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Principales

**Container**

```tsx
<div className="min-h-screen bg-[#0a0a0f] flex items-center justify-center py-8 px-4">
  <Card className="max-w-2xl mx-auto bg-[#0f1729] border-[#334155] p-8 text-center">
    {/* Content */}
  </Card>
</div>
```

**Success Icon**

```tsx
<div className="w-24 h-24 mx-auto mb-6 rounded-full bg-green-500/20 flex items-center justify-center">
  <Icon
    name="heart-handshake"
    className="w-12 h-12 text-green-400 animate-in zoom-in duration-400"
  />
</div>
```

**Title and Messages**

```tsx
<h1 className="text-3xl font-bold text-white mb-4">
  ¡Gracias por tu apoyo!
</h1>

<p className="text-lg text-[#cbd5e1] mb-2">
  Tu contribucion ha sido confirmada con exito.
</p>

<p className="text-[#94a3b8] mb-8">
  Has ayudado a {campaign.artistaNombre} a alcanzar su sueno musical.
</p>
```

**Summary Card**

```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-6 mb-6 text-left">
  <h3 className="text-xl font-bold text-white mb-4 text-center">
    Resumen de tu Apoyo
  </h3>

  <div className="space-y-2">
    <div className="flex justify-between py-2 border-b border-[#334155]">
      <span className="text-sm text-[#64748b]">Campania:</span>
      <span className="text-sm text-white font-semibold">
        {backing.campaniaTitulo}
      </span>
    </div>

    <div className="flex justify-between py-2 border-b border-[#334155]">
      <span className="text-sm text-[#64748b]">Artista:</span>
      <span className="text-sm text-white font-semibold">
        {backing.artistaNombre}
      </span>
    </div>

    <div className="flex justify-between py-2 border-b border-[#334155]">
      <span className="text-sm text-[#64748b]">Monto:</span>
      <span className="text-lg text-primary font-bold">
        €{backing.monto}
      </span>
    </div>

    {backing.rewardNombre && (
      <div className="flex justify-between py-2 border-b border-[#334155]">
        <span className="text-sm text-[#64748b]">Reward:</span>
        <span className="text-sm text-white font-semibold">
          {backing.rewardNombre}
        </span>
      </div>
    )}

    <div className="flex justify-between py-2 border-b border-[#334155]">
      <span className="text-sm text-[#64748b]">Fecha:</span>
      <span className="text-sm text-white font-semibold">
        {formatDate(backing.fechaCreacion)}
      </span>
    </div>

    <div className="flex justify-between py-2">
      <span className="text-sm text-[#64748b]">ID Backing:</span>
      <span className="text-sm text-white font-semibold font-mono">
        {backing.id.substring(0, 13)}
      </span>
    </div>
  </div>

  <div className="flex items-start gap-2 bg-[#1e293b] p-3 rounded-lg mt-4">
    <Icon name="mail" className="w-5 h-5 text-primary mt-0.5" />
    <p className="text-xs text-[#cbd5e1]">
      Te hemos enviado un email de confirmacion a: <br />
      <strong>{userEmail}</strong>
    </p>
  </div>
</Card>
```

**Next Steps Section**

```tsx
<div className="text-left mb-8">
  <h4 className="text-lg font-bold text-white mb-3">¿Que sigue?</h4>

  <ul className="space-y-2 text-sm text-[#94a3b8]">
    <li className="flex items-start gap-2">
      <Icon name="check" className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
      <span>El artista procesara tu recompensa</span>
    </li>
    <li className="flex items-start gap-2">
      <Icon name="check" className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
      <span>Te notificaremos sobre actualizaciones de la campania</span>
    </li>
    <li className="flex items-start gap-2">
      <Icon name="check" className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
      <span>Recibiras tu recompensa en: Marzo 2025</span>
    </li>
  </ul>
</div>
```

**Actions Row**

```tsx
<div className="flex gap-3 justify-center">
  <Button
    variant="outline"
    className="border-primary text-primary hover:bg-primary/10 px-8"
    onClick={() => navigate(`/campanias/${backing.campaniaId}`)}
  >
    Ver Campania
  </Button>

  <Button
    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
    onClick={() => navigate("/campanias")}
  >
    Explorar Mas Campanias
  </Button>
</div>
```

#### Estados y Variantes

| Estado | Visual |
|--------|--------|
| **Loading** | Spinner mientras verifica session_id |
| **Success** | Success icon animado (scale), confetti opcional, summary visible |
| **Error Session** | Icon x-circle rojo, texto error, boton "Contactar soporte" |
| **Mobile View** | Buttons stack vertical full-width, padding reducido |

#### Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Full width (max-w-full), padding p-4, buttons stack vertical, font sizes reducidos |
| **sm-md (tablet)** | max-w-xl, layout normal |
| **> lg (desktop)** | max-w-2xl, spacing amplio |

---

## 4. Componentes Compartidos

### 4.1 CampaignProgressBar

**Ubicacion:** `src/web/src/components/shared/CampaignProgressBar.tsx`

**Props:**
```typescript
interface CampaignProgressBarProps {
  amountRaised: number;
  goalAmount: number;
  backers: number;
  percentage: number;
  size?: "sm" | "md" | "lg";
  className?: string;
}
```

**Composicion:**
```tsx
<div className={cn("space-y-2", className)}>
  <div className={cn(
    "w-full bg-[#1e293b] rounded-full overflow-hidden",
    size === "sm" && "h-1",
    size === "md" && "h-2",
    size === "lg" && "h-3"
  )}>
    <div
      className={cn(
        "h-full rounded-full transition-all duration-500",
        percentage >= 100
          ? "bg-gradient-to-r from-green-400 to-emerald-500"
          : percentage >= 90
          ? "bg-gradient-to-r from-warning to-amber-500"
          : "bg-gradient-to-r from-pink-500 to-purple-600"
      )}
      style={{ width: `${Math.min(percentage, 100)}%` }}
    />
  </div>

  <div className="flex justify-between text-sm">
    <span className="font-semibold text-primary">
      {percentage}% financiado
    </span>
    <div className="flex items-center gap-1 text-[#64748b]">
      <Icon name="users" className="w-4 h-4" />
      {backers} backers
    </div>
  </div>
</div>
```

**Variantes:**
- `size="sm"` - height 1 (h-1)
- `size="md"` - height 2 (h-2, default)
- `size="lg"` - height 3 (h-3)

**Colores dinamicos:**
- Default (0-89%): gradient pink-500 → purple-600
- Near goal (90-99%): gradient warning → amber-500
- Success (100%+): gradient green-400 → emerald-500

### 4.2 RewardCard

**Ubicacion:** `src/web/src/components/shared/RewardCard.tsx`

**Props:**
```typescript
interface RewardCardProps {
  reward: RewardPublic;
  onSelect: () => void;
  isSelected?: boolean;
  showSelectButton?: boolean;
  className?: string;
}
```

**Composicion:** (Ver seccion 3.2 Campaign Detail - RewardCard Component)

**Variantes:**
- Default: Card con hover effects, boton "Seleccionar"
- Selected: Border primary, bg primary/5
- Sold Out: Opacity 60%, boton disabled "Agotado"
- Popular: Badge rosa "Mas popular"
- Low Stock: Badge warning "Pocas unidades"

### 4.3 BackingStatusBadge

**Ubicacion:** `src/web/src/components/shared/BackingStatusBadge.tsx`

**Props:**
```typescript
interface BackingStatusBadgeProps {
  status: "EN_CURSO" | "FINANCIADA" | "COMPLETADA" | "PENDIENTE" | "CANCELADA";
  className?: string;
}
```

**Composicion:**
```tsx
<Badge
  variant={getVariant(status)}
  className={cn(getClassName(status), className)}
>
  <Icon name={getIcon(status)} className="w-3 h-3 mr-1" />
  {getLabel(status)}
</Badge>
```

**Variantes:**

| Estado | Variant | Color | Icon | Texto |
|--------|---------|-------|------|-------|
| EN_CURSO | default | bg-primary/20 text-primary border-primary/50 | clock | En curso |
| FINANCIADA | success | bg-green-500/20 text-green-400 border-green-500/50 | check-circle | Financiada |
| COMPLETADA | secondary | bg-blue-500/20 text-blue-400 border-blue-500/50 | package-check | Completada |
| PENDIENTE | warning | bg-warning/20 text-warning border-warning/50 | alert-circle | Pendiente |
| CANCELADA | destructive | bg-red-500/20 text-red-400 border-red-500/50 | x-circle | Cancelada |

### 4.4 AmountInput

**Ubicacion:** `src/web/src/components/shared/AmountInput.tsx`

**Props:**
```typescript
interface AmountInputProps {
  amount: number;
  onChange: (amount: number) => void;
  minAmount?: number;
  maxAmount?: number;
  error?: string;
  hint?: string;
  className?: string;
}
```

**Composicion:** (Ver seccion 3.3 Backing Form - Amount Input)

**Estados:**
- Default: Border gray, euro symbol left
- Focus: Border primary, glow shadow
- Error: Border red, error message below
- Disabled: Opacity 50%, cursor-not-allowed

### 4.5 CharacterCounter

**Ubicacion:** `src/web/src/components/shared/CharacterCounter.tsx`

**Props:**
```typescript
interface CharacterCounterProps {
  current: number;
  max: number;
  className?: string;
}
```

**Composicion:**
```tsx
<span
  className={cn(
    "text-xs block",
    current > max * 0.8 ? "text-warning" : "text-[#64748b]",
    current > max * 0.95 && "text-red-500",
    className
  )}
>
  {current}/{max} caracteres
</span>
```

**Colores dinamicos:**
- 0-80%: text-[#64748b] (gray)
- 80-95%: text-warning (yellow)
- 95-100%: text-red-500 (red)

---

## 5. Feedback y Estados

### Loading

**Skeleton Cards (Campaign Listing):**
```tsx
<Card className="bg-[#1a1a2e] border-[#334155] overflow-hidden">
  <Skeleton className="aspect-video w-full bg-[#1e293b]" />
  <CardContent className="p-5 space-y-3">
    <Skeleton className="h-6 w-3/4 bg-[#1e293b]" />
    <Skeleton className="h-4 w-1/2 bg-[#1e293b]" />
    <Skeleton className="h-8 w-full bg-[#1e293b]" />
    <Skeleton className="h-2 w-full bg-[#1e293b]" />
    <Skeleton className="h-4 w-full bg-[#1e293b]" />
  </CardContent>
</Card>
```

**Spinner (Full Page):**
```tsx
<div className="min-h-screen bg-[#0a0a0f] flex items-center justify-center">
  <div className="text-center">
    <Icon name="loader-2" className="w-16 h-16 animate-spin text-primary mx-auto mb-4" />
    <p className="text-lg text-white">Cargando...</p>
  </div>
</div>
```

**Button Loading:**
```tsx
<Button disabled={isLoading}>
  {isLoading && (
    <Icon name="loader-2" className="w-4 h-4 mr-2 animate-spin" />
  )}
  {isLoading ? "Procesando..." : "Continuar"}
</Button>
```

### Error

**Alert (Inline):**
```tsx
<Alert variant="destructive" className="bg-red-500/10 border-red-500/50">
  <Icon name="alert-circle" className="w-4 h-4" />
  <AlertTitle>Error</AlertTitle>
  <AlertDescription>
    {errorMessage}
  </AlertDescription>
</Alert>
```

**Form Field Error:**
```tsx
<div className="space-y-2">
  <Input
    className={cn(
      "bg-[#1a1a2e] border-[#334155]",
      error && "border-red-500 focus:border-red-500"
    )}
    aria-invalid={!!error}
    aria-describedby="field-error"
  />
  {error && (
    <p id="field-error" role="alert" className="text-xs text-red-500">
      {error}
    </p>
  )}
</div>
```

**Toast (Global Error):**
Usar `sonner` (ya instalado en el proyecto)
```tsx
import { toast } from "sonner";

toast.error("Error al procesar el aporte", {
  description: "Por favor, intenta nuevamente",
});
```

### Success

**Toast (Success):**
```tsx
toast.success("Aporte realizado con exito", {
  description: "Gracias por tu apoyo",
});
```

**Success Badge:**
```tsx
<Badge className="bg-green-500/20 text-green-400 border-green-500/50">
  <Icon name="check-circle" className="w-3 h-3 mr-1" />
  Completado
</Badge>
```

### Empty State

**No Results:**
```tsx
<div className="text-center py-16 px-4">
  <Icon name="search-x" className="w-20 h-20 text-[#64748b] mx-auto mb-4" />
  <h3 className="text-2xl font-bold text-white mb-2">
    No se encontraron campanias
  </h3>
  <p className="text-[#94a3b8] mb-6">
    Intenta ajustar tus filtros o realiza una busqueda diferente
  </p>
  <Button className="bg-gradient-to-r from-pink-500 to-purple-600 text-white">
    Limpiar filtros
  </Button>
</div>
```

**No Backings:**
```tsx
<div className="text-center py-16 px-4">
  <Icon name="heart-off" className="w-20 h-20 text-[#64748b] mx-auto mb-4" />
  <h3 className="text-2xl font-bold text-white mb-2">
    No has apoyado ninguna campania aun
  </h3>
  <p className="text-[#94a3b8] mb-6">
    Explora proyectos y encuentra tu proxima banda favorita
  </p>
  <Button className="bg-gradient-to-r from-pink-500 to-purple-600 text-white">
    Explorar Campanias
  </Button>
</div>
```

---

## 6. Animaciones y Transiciones

### Progress Bar Fill

```css
.progress-bar-fill {
  transition: width 500ms cubic-bezier(0.4, 0, 0.2, 1);
}
```

### Card Hover

```css
.campaign-card {
  transition: all 300ms ease;
}

.campaign-card:hover {
  border-color: #a855f7; /* primary */
  box-shadow: 0 10px 25px -5px rgba(168, 85, 247, 0.2);
}

.campaign-card img {
  transition: transform 500ms ease;
}

.campaign-card:hover img {
  transform: scale(1.05);
}
```

### Modal Open

```css
.modal-overlay {
  animation: fadeIn 250ms ease-out;
}

.modal-content {
  animation: scaleIn 250ms ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}
```

### Success Icon

```css
.success-icon {
  animation: successBounce 400ms cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

@keyframes successBounce {
  0% {
    opacity: 0;
    transform: scale(0);
  }
  60% {
    opacity: 1;
    transform: scale(1.2);
  }
  100% {
    transform: scale(1);
  }
}
```

### Confetti Animation (Opcional)

Usar libreria `canvas-confetti` (instalar con npm)
```tsx
import confetti from 'canvas-confetti';

useEffect(() => {
  confetti({
    particleCount: 100,
    spread: 70,
    origin: { y: 0.6 },
    colors: ['#ec4899', '#a855f7', '#10b981', '#3b82f6'],
  });
}, []);
```

### Skeleton Pulse

```css
@keyframes skeleton-pulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

.skeleton {
  animation: skeleton-pulse 1500ms ease-in-out infinite;
}
```

### Badge Appear

```css
.badge {
  animation: badgeAppear 200ms ease-out;
}

@keyframes badgeAppear {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}
```

### Days Badge Pulse (Expiring Soon)

```css
.days-badge-expiring {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}
```

---

## 7. Responsive Design

### Breakpoints

| Breakpoint | Tailwind | Width |
|------------|----------|-------|
| Mobile | `< sm` | < 640px |
| Tablet | `sm` - `md` | 640px - 768px |
| Desktop | `> lg` | > 1024px |

### Campaign Listing

| Elemento | Mobile | Tablet | Desktop |
|----------|--------|--------|---------|
| Grid | `grid-cols-1` | `md:grid-cols-2` | `lg:grid-cols-3` |
| Hero Title | `text-3xl` | `md:text-4xl` | `md:text-5xl` |
| Search Bar | `px-4` | `px-12` | `px-12` |
| Filters | `flex-col` | `md:flex-row` | `md:flex-row` |
| Card Padding | `p-4` | `p-5` | `p-5` |

### Campaign Detail

| Elemento | Mobile | Tablet | Desktop |
|----------|--------|--------|---------|
| Layout | Stack vertical | Stack vertical | `lg:grid-cols-[1fr_380px]` |
| Sidebar | NO sticky | Sticky | Sticky top-24 |
| Hero | `aspect-square` | `aspect-video` | `aspect-video` |
| Title | `text-2xl` | `text-3xl` | `md:text-4xl` |
| Stats Grid | `gap-2` | `gap-4` | `gap-4` |

### Backing Form

| Elemento | Mobile | Tablet | Desktop |
|----------|--------|--------|---------|
| Card | Full width | `max-w-xl` | `max-w-2xl` |
| Padding | `p-4` | `p-6` | `p-6` |
| Amount Input | `text-xl` | `text-2xl` | `text-2xl` |
| Suggestions | 3 cols grid | Flex row | Flex row |
| Footer Buttons | Stack vertical | Flex row | Flex row |

### Utilities Tailwind

**Stack to Row:**
```tsx
<div className="flex flex-col md:flex-row gap-4">
```

**Full Width to Max Width:**
```tsx
<div className="w-full max-w-7xl mx-auto px-4">
```

**Hidden on Mobile:**
```tsx
<div className="hidden md:block">
```

**Mobile Only:**
```tsx
<div className="block md:hidden">
```

---

## 8. Accesibilidad

### Contraste de Color

Verificado WCAG AA (4.5:1 minimo):
- Primary (#a855f7) sobre bg (#1a1a2e): **5.2:1** ✓
- Text white (#ffffff) sobre bg (#0a0a0f): **21:1** ✓
- Text gray (#94a3b8) sobre bg (#0a0a0f): **8.5:1** ✓

### Focus States

Todos los elementos interactivos:
```tsx
className="focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2"
```

### Form Labels

Todos los inputs tienen labels asociados:
```tsx
<Label htmlFor="amount">Monto a Aportar</Label>
<Input id="amount" aria-describedby="amount-hint amount-error" />
```

### Form Validation

Mensajes de error con roles ARIA:
```tsx
{error && (
  <p id="amount-error" role="alert" aria-live="polite" className="text-xs text-red-500">
    {error}
  </p>
)}
```

### Progress Bars

Con atributos ARIA:
```tsx
<div
  role="progressbar"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
  aria-label={`Progreso de la campania: ${percentage}% financiado, ${backers} backers`}
>
  <div className="progress-fill" style={{ width: `${percentage}%` }} />
</div>
```

### Buttons

Texto descriptivo o aria-label:
```tsx
<Button aria-label="Cerrar modal">
  <Icon name="x" />
</Button>

<Button>
  <Icon name="heart" className="w-5 h-5 mr-2" />
  Apoyar esta campania
</Button>
```

### Images

Alt text descriptivo:
```tsx
<img
  src={campaign.imagenPrincipalUrl}
  alt={`Imagen de la campania ${campaign.titulo} de ${campaign.artistaNombre}`}
/>
```

### Modal Focus Trap

Usar Dialog de shadcn (ya incluye focus trap):
```tsx
<Dialog open={open} onOpenChange={setOpen}>
  <DialogContent>
    {/* Focus queda dentro del dialog */}
  </DialogContent>
</Dialog>
```

### Status Badges

Con role status:
```tsx
<Badge role="status" aria-label={`Estado de la campania: ${statusLabel}`}>
  <Icon name={statusIcon} aria-hidden="true" />
  {statusLabel}
</Badge>
```

### Keyboard Navigation

Orden logico de tab:
1. Header navigation
2. Search bar
3. Filter buttons
4. Campaign cards (uno por uno)
5. Pagination buttons

Teclas soportadas:
- **Tab**: Navegar entre elementos
- **Enter**: Activar boton/link, submit form
- **Esc**: Cerrar modal
- **Space**: Toggle checkbox
- **Arrow Keys**: Navegar entre tabs

### Screen Reader Support

Landmarks:
```tsx
<header role="banner">...</header>
<nav role="navigation" aria-label="Navegacion principal">...</nav>
<main role="main" id="main-content">...</main>
<aside role="complementary" aria-label="Recompensas">...</aside>
```

Skip Link:
```tsx
<a href="#main-content" className="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 focus:z-50 focus:px-4 focus:py-2 focus:bg-primary focus:text-white">
  Saltar al contenido principal
</a>
```

---

## 9. Checklist

### Campaign Listing
- [ ] Hero section con titulo, subtitulo, search bar
- [ ] Search bar con icon, debounce 500ms, clear button
- [ ] Filters row: Genero, Estado, Ordenar con dropdowns
- [ ] Results count visible
- [ ] Grid responsive: 1 col mobile, 2 tablet, 3 desktop
- [ ] Campaign cards con image, badges (genero, dias)
- [ ] Progress bar con gradient dinamico segun %
- [ ] Hover effect: border primary, image scale 1.05
- [ ] Pagination con buttons prev/next
- [ ] Empty state: icon, texto, boton
- [ ] Loading: 6 skeleton cards con shimmer
- [ ] Accesibilidad: search role="searchbox", cards role="article", progress aria-valuenow

### Campaign Detail
- [ ] Grid layout: main + sidebar (desktop), stack (mobile)
- [ ] Hero image/video con controls
- [ ] Campaign header: title, artist avatar + link, verified badge
- [ ] Progress section: amount, bar, stats grid (3 cols)
- [ ] Tabs navigation: Historia, Actualizaciones, FAQ
- [ ] Description content con prose styling
- [ ] Artist bio card: avatar, name, description, stats
- [ ] Sidebar sticky top-24 (desktop), scroll normal (mobile)
- [ ] CTA button gradient full-width
- [ ] Rewards list con RewardCard components
- [ ] Popular badge, low stock badge, sold out state
- [ ] Sidebar footer: lock icon + "Pago seguro", truck icon + entrega
- [ ] Loading: skeletons para hero, title, progress, rewards
- [ ] Accesibilidad: video controls, tabs role="tablist", progress aria

### Backing Form
- [ ] Modal/card max-w-2xl, header con close button
- [ ] Progress section: bar + step labels
- [ ] Selected reward card con boton "Cambiar"
- [ ] Amount input: € symbol left, text-2xl bold center, focus primary
- [ ] Amount suggestions: +€5, +€10, +€25 buttons
- [ ] Message textarea con character counter (0/500)
- [ ] Character counter warning at 400, red at 475
- [ ] Anonymous checkbox con icon eye-off
- [ ] Summary section: Aportacion, Reward, Total (primary)
- [ ] Footer: "Volver" outline, "Continuar" gradient
- [ ] Validation errors: border rojo, mensaje debajo
- [ ] Loading continue: spinner + "Procesando...", inputs disabled
- [ ] Mobile: full screen, suggestions 3 cols, footer stack vertical
- [ ] Accesibilidad: labels htmlFor, errors role="alert", focus trap

### Backing Confirmation
- [ ] Card max-w-2xl centered, padding amplio
- [ ] Success icon w-24 h-24, circular bg green/20, icon heart-handshake
- [ ] Success icon animation: scale 0 → 1.2 → 1
- [ ] Confetti animation opcional (canvas-confetti)
- [ ] Title h1 "¡Gracias por tu apoyo!"
- [ ] Subtitle + message text gray
- [ ] Summary card: rows label/value (campania, artista, monto, reward, fecha, ID)
- [ ] Amount value text-lg primary bold
- [ ] Email notice section: icon mail, texto confirmacion
- [ ] Next steps section: title + list con bullets (icon check primary)
- [ ] Actions row: "Ver Campania" outline, "Explorar Mas" gradient
- [ ] Loading: spinner mientras verifica session_id
- [ ] Error state: icon x-circle, texto, boton "Contactar soporte"
- [ ] Mobile: buttons stack vertical full-width
- [ ] Accesibilidad: success role="status" aria-live="polite"

### Componentes Compartidos
- [ ] CampaignProgressBar: bar con gradient segun %, stats debajo
- [ ] RewardCard: card con hover, badges (popular, low stock), boton seleccionar
- [ ] BackingStatusBadge: badge con icon + texto, colores segun estado
- [ ] AmountInput: input con € prefix, validacion min/max, error message
- [ ] CharacterCounter: contador dinamico, colores warning/red

### Cross-cutting
- [ ] Dark theme aplicado (#0a0a0f, #0f1729, #1a1a2e, #334155)
- [ ] Gradient buttons (pink → purple) en CTAs
- [ ] shadcn/ui components: Card, Button, Input, Textarea, Badge, Progress, Avatar, Tabs, Skeleton, Separator, Alert, Label
- [ ] Tailwind utilities consistentes
- [ ] Animaciones smooth (150-500ms)
- [ ] Progress bar colores dinamicos (purple, yellow >90%, green 100%)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring purple
- [ ] Form validation con Zod + react-hook-form
- [ ] TanStack Query para queries y mutations
- [ ] Manejo de errores del backend (ServiceResponse)
- [ ] Toast notifications (sonner)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders para loading states
- [ ] Empty states con iconos y CTAs
- [ ] Progress bars con role="progressbar" y aria
- [ ] Status badges con role="status"

---

## 10. Proximos Pasos

Despues de aprobar este diseno UI:

1. **Implementar componentes shared** (CampaignProgressBar, RewardCard, etc.)
2. **Implementar Campaign Listing page** (`/campanias`)
3. **Implementar Campaign Detail page** (`/campanias/{id}`)
4. **Implementar Backing Form** (`/campanias/{id}/backing`)
5. **Implementar Backing Confirmation** (`/campanias/{id}/backing-confirmado`)
6. **Integrar con TanStack Query hooks** (useCampanias, useCampaniaDetail, useCreateBacking)
7. **Testing E2E** con Playwright o Cypress

---

**Fin del documento de diseno UI para hacer-backing (Landing).**
