# Diseno UI: Crear Campania (Landing)

**Fecha:** 2026-02-12
**Feature:** crear-campania
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen

Este documento define la arquitectura de componentes UI para las pantallas publicas de campanas en la landing app:
- **Explorar Campanas** - Grid de campaign cards con filtros
- **Detalle Campana** - Layout 2+1 columnas con hero, contenido y sidebar de rewards

**Componentes shadcn utilizados:** 18
**Composiciones custom:** 12
**Responsive breakpoints:** mobile (< 640px), tablet (640-1024px), desktop (> 1024px)
**Dark theme:** Si (paleta oscura #1a1a2e, #0f1729, gradient pink-purple)

---

## 2. Paleta de Colores (Dark Theme)

| Uso | CSS Variable | Valor Hex | Ejemplo |
|-----|--------------|-----------|---------|
| Background Primary | `--bg-primary` | #1a1a2e | Fondo principal de pagina |
| Background Secondary | `--bg-secondary` | #16213e | Fondo de secciones alternadas |
| Background Card | `--bg-card` | #0f1729 | Tarjetas de campana, modales |
| Background Card Hover | `--bg-card-hover` | #1e2a42 | Hover state en cards |
| Primary Gradient | `--primary-gradient` | linear-gradient(135deg, #ec4899 0%, #a855f7 100%) | Botones CTA principales |
| Primary Gradient Hover | `--primary-gradient-hover` | linear-gradient(135deg, #f472b6 0%, #c084fc 100%) | Hover en CTAs |
| Text Primary | `--text-primary` | #ffffff | Titulos, textos destacados |
| Text Secondary | `--text-secondary` | #94a3b8 | Descripciones, subtitulos |
| Text Muted | `--text-muted` | #64748b | Placeholders, hints |
| Text Label | `--text-label` | #cbd5e1 | Labels de formularios |
| Border Primary | `--border-primary` | #334155 | Bordes de cards, inputs |
| Border Focus | `--border-focus` | #a855f7 | Estado focus en inputs |
| Status Success | `--status-success` | #10b981 | Badge "Activa", progress completado |
| Status Error | `--status-error` | #ef4444 | Errores, estado cancelado |
| Status Warning | `--status-warning` | #f59e0b | Alertas, advertencias |
| Status Info | `--status-info` | #3b82f6 | Badge "Finalizada" |
| Badge Draft | `--badge-draft` | #64748b | Estado borrador (no visible en landing) |
| Badge Active | `--badge-active` | #10b981 | Campana activa |
| Badge Completed | `--badge-completed` | #3b82f6 | Campana finalizada |
| Badge Cancelled | `--badge-cancelled` | #ef4444 | Campana cancelada |

**Nota:** Las campanas en estado "Borrador" NO se muestran en la landing publica.

---

## 3. Componentes por Screen

### 3.1 Explorar Campanas (`/explorar`)

#### Layout

```
┌──────────────────────────────────────────────────────┐
│ Header Nav (sticky)                                  │
│ [Logo] Explorar Categorias Cómo funciona  [Login]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│ Descubre proyectos musicales                        │
│ [Search input] [Filtro Genero v] [Filtro Estado v]  │
│                                                      │
│ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐         │
│ │ Card 1 │ │ Card 2 │ │ Card 3 │ │ Card 4 │         │
│ └────────┘ └────────┘ └────────┘ └────────┘         │
│                                                      │
│ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐         │
│ │ Card 5 │ │ Card 6 │ │ Card 7 │ │ Card 8 │         │
│ └────────┘ └────────┘ └────────┘ └────────┘         │
│                                                      │
│ [Load More]                                          │
│                                                      │
└──────────────────────────────────────────────────────┘
```

#### Componentes

**HeaderNav**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<header>` (HTML) | `bg-[#1a1a2e] border-b border-[#334155] sticky top-0 z-50` |
| Logo Link | `<Link>` + Icon | `flex items-center gap-2 text-white font-bold text-xl` |
| Nav Links | `<nav>` con Links | `flex gap-6 text-[#94a3b8] hover:text-white transition` |
| Login Button | `Button variant="outline"` | `border-[#334155] text-[#94a3b8] hover:text-white` |
| Create Button | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |

**Composicion HeaderNav:**
```tsx
<header className="bg-[#1a1a2e] border-b border-[#334155] sticky top-0 z-50">
  <div className="max-w-7xl mx-auto px-6 py-4 flex items-center justify-between">
    <Link to="/" className="flex items-center gap-2 text-white font-bold text-xl">
      <MusicIcon className="w-6 h-6" />
      <span>MusicFund</span>
    </Link>

    <nav className="hidden md:flex gap-6">
      <Link to="/explorar" className="text-[#94a3b8] hover:text-white transition">
        Explorar
      </Link>
      <Link to="/categorias" className="text-[#94a3b8] hover:text-white transition">
        Categorías
      </Link>
      <Link to="/como-funciona" className="text-[#94a3b8] hover:text-white transition">
        Cómo funciona
      </Link>
    </nav>

    <div className="flex items-center gap-3">
      <Button variant="outline" className="border-[#334155] text-[#94a3b8]">
        Iniciar Sesión
      </Button>
      <Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
        Crear Campaña
      </Button>
    </div>
  </div>
</header>
```

**SearchAndFilters**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<div>` | `max-w-7xl mx-auto px-6 py-8` |
| Title | `<h1>` | `text-4xl font-bold text-white mb-2` |
| Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Search Input | `Input` | `bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b]` |
| Filter Select | `Select` (futuro) | `bg-[#0f1729] border-[#334155]` (no implementado en MVP) |

**Composicion SearchAndFilters:**
```tsx
<div className="max-w-7xl mx-auto px-6 py-8">
  <h1 className="text-4xl font-bold text-white mb-2">
    Descubre proyectos musicales
  </h1>
  <p className="text-lg text-[#94a3b8] mb-8">
    Apoya a tus artistas favoritos y haz realidad sus sueños
  </p>

  <div className="flex gap-4 mb-8">
    <div className="flex-1">
      <Input
        type="search"
        placeholder="Buscar campañas..."
        className="bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7]"
      />
    </div>
    {/* Filtros fuera de MVP - reservar espacio */}
  </div>
</div>
```

**CampaniaCard** (Grid Item)

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `Card` | `bg-[#0f1729] border-[#334155] hover:border-[#a855f7] transition cursor-pointer` |
| Image | `<img>` | `w-full h-48 object-cover rounded-t-lg` |
| Header | `CardHeader` | `p-4` |
| Title | `CardTitle` | `text-lg font-bold text-white line-clamp-2` |
| Artist | `<Link>` con Avatar | `flex items-center gap-2 text-sm text-[#94a3b8] hover:text-primary` |
| Content | `CardContent` | `p-4 pt-0 space-y-3` |
| Progress Bar | `Progress` | `h-2 bg-[#334155]` con indicator `bg-gradient-to-r from-pink-500 to-purple-600` |
| Stats Row | `<div>` | `flex items-center justify-between text-sm` |
| Amount | `<span>` | `text-white font-semibold` |
| Backers | `<span>` | `text-[#64748b] flex items-center gap-1` |
| Status Badge | `Badge` | `variant="outline"` con custom colors segun estado |

**Composicion CampaniaCard:**
```tsx
<Card
  className="bg-[#0f1729] border-[#334155] hover:border-[#a855f7] transition cursor-pointer overflow-hidden"
  onClick={() => navigate(`/campanias/${campania.id}`)}
>
  <img
    src={campania.imagenPrincipalUrl || '/placeholder.jpg'}
    alt={campania.titulo}
    className="w-full h-48 object-cover"
  />

  <CardHeader className="p-4">
    <div className="flex items-center justify-between mb-2">
      <Badge
        variant="outline"
        className={cn(
          "text-xs",
          campania.estadoCampaniaId === 2 && "bg-green-500/20 text-green-400 border-green-500/50",
          campania.estadoCampaniaId === 3 && "bg-blue-500/20 text-blue-400 border-blue-500/50"
        )}
      >
        {campania.estadoCampaniaId === 2 ? 'Activa' : 'Finalizada'}
      </Badge>
    </div>

    <CardTitle className="text-lg font-bold text-white line-clamp-2">
      {campania.titulo}
    </CardTitle>

    <Link
      to={`/artistas/${campania.artistaId}`}
      className="flex items-center gap-2 text-sm text-[#94a3b8] hover:text-primary mt-2"
      onClick={(e) => e.stopPropagation()}
    >
      <Avatar className="w-5 h-5">
        <AvatarImage src={artista.avatar} />
        <AvatarFallback>{artista.nombre[0]}</AvatarFallback>
      </Avatar>
      <span>{artista.nombre}</span>
    </Link>
  </CardHeader>

  <CardContent className="p-4 pt-0 space-y-3">
    <Progress
      value={(campania.importePledgedActual / campania.importeObjetivo) * 100}
      className="h-2"
    />

    <div className="flex items-center justify-between">
      <div className="space-y-1">
        <p className="text-white font-semibold">
          {formatCurrency(campania.importePledgedActual, campania.monedaId)}
        </p>
        <p className="text-xs text-[#64748b]">
          de {formatCurrency(campania.importeObjetivo, campania.monedaId)}
        </p>
      </div>

      <div className="flex items-center gap-1 text-[#64748b] text-sm">
        <UsersIcon className="w-4 h-4" />
        <span>{campania.backersCount || 0}</span>
      </div>
    </div>

    {campania.fechaFin && (
      <p className="text-xs text-[#64748b]">
        {getDaysRemaining(campania.fechaFin)} días restantes
      </p>
    )}
  </CardContent>
</Card>
```

**CampaniaGrid**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<div>` | `max-w-7xl mx-auto px-6 pb-12` |
| Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6` |
| Load More Button | `Button variant="outline"` | `mx-auto block mt-8` |

**Composicion CampaniaGrid:**
```tsx
<div className="max-w-7xl mx-auto px-6 pb-12">
  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
    {campanias.map((campania) => (
      <CampaniaCard key={campania.id} campania={campania} />
    ))}
  </div>

  {hasMore && (
    <Button
      variant="outline"
      className="mx-auto block mt-8 border-[#334155] text-[#94a3b8] hover:text-white"
      onClick={loadMore}
      disabled={isLoadingMore}
    >
      {isLoadingMore ? 'Cargando...' : 'Cargar más'}
    </Button>
  )}
</div>
```

**Estados UI - Explorar Campanas**

| Estado | Componente Visual |
|--------|------------------|
| Loading Initial | 8x Skeleton cards en grid (usando `Skeleton` de shadcn) |
| Loading More | Load More button con spinner + texto "Cargando..." |
| Empty State | Card centrado con icono FolderOpen + "No se encontraron campañas" + link "Ver todas" |
| Error State | Alert destructive con mensaje de error + botón "Reintentar" |
| Hover Card | Border color cambia a `#a855f7`, elevation sutil (shadow-lg) |
| Search Active | Input con border focus `#a855f7`, icono search animado |

---

### 3.2 Detalle Campana Publica (`/campanias/{id}`)

#### Layout

```
┌────────────────────────────────────────────────────────────────┐
│ Header Nav (sticky)                                            │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│ ┌────────────────────────────────────────────────────────────┐ │
│ │ Hero Image (full width, h-96)                             │ │
│ └────────────────────────────────────────────────────────────┘ │
│                                                                │
│ ┌─────────────────────────────┐  ┌───────────────────────────┐ │
│ │ Content (col-span-2)        │  │ Sidebar (col-span-1)      │ │
│ │                             │  │                           │ │
│ │ Titulo de Campaña           │  │ [Apoyar esta campaña]     │ │
│ │ Artista • Genre • Badge     │  │ Desde € 5                 │ │
│ │                             │  │                           │ │
│ │ € 12,450 de € 15,000        │  │ ────────────────────      │ │
│ │ ████████████░░░ 83%         │  │                           │ │
│ │ 127 backers • 12 días       │  │ Recompensas               │ │
│ │                             │  │                           │ │
│ │ ──────────────────          │  │ ┌───────────────────────┐ │ │
│ │                             │  │ │ € 10 Descarga Digital │ │ │
│ │ [Tabs: Historia | Updates | │  │ │ Más popular          │ │ │
│ │  Comentarios | FAQ]         │  │ │ 234 de 500 dispon.   │ │ │
│ │                             │  │ │ [Seleccionar]        │ │ │
│ │ Rich text description...    │  │ └───────────────────────┘ │ │
│ │ [Images in content]         │  │                           │ │
│ │                             │  │ ┌───────────────────────┐ │ │
│ │ ✓ Masterización             │  │ │ € 25 CD Físico       │ │ │
│ │ ✓ Producción vinilos        │  │ │ CD + extras          │ │ │
│ │ ✓ 3 videoclips              │  │ │ 89 de 200 dispon.    │ │ │
│ │                             │  │ │ [Seleccionar]        │ │ │
│ │ ──────────────────          │  │ └───────────────────────┘ │ │
│ │                             │  │                           │ │
│ │ Sobre el Artista            │  │ 🔒 Pago seguro            │ │
│ │ [Avatar + Bio + Stats]      │  │ 📦 Entrega: Marzo 2025    │ │
│ │                             │  │                           │ │
│ └─────────────────────────────┘  └───────────────────────────┘ │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

#### Componentes

**HeroImage**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<div>` | `w-full h-96 relative overflow-hidden` |
| Image | `<img>` | `w-full h-full object-cover` |
| Skeleton | `Skeleton` | `w-full h-96` (loading state) |

**Composicion HeroImage:**
```tsx
<div className="w-full h-96 relative overflow-hidden rounded-lg mb-8">
  {isLoading ? (
    <Skeleton className="w-full h-full" />
  ) : (
    <img
      src={campania.imagenPrincipalUrl}
      alt={campania.titulo}
      className="w-full h-full object-cover"
    />
  )}
</div>
```

**CampaniaHeader**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Title | `<h1>` | `text-4xl font-bold text-white mb-4` |
| Artist Info Row | `<div>` | `flex items-center gap-3 mb-6` |
| Artist Avatar | `Avatar` | `w-10 h-10` |
| Artist Name Link | `<Link>` | `text-primary hover:underline font-medium` |
| Verified Badge | `Badge` | `bg-primary/20 text-primary border-primary/50` |
| Genre Badge | `Badge variant="outline"` | `bg-[#2d1b4e] text-purple-300 border-purple-500/50` |
| Status Badge | `Badge` | Dynamic color based on `estadoCampaniaId` |

**Composicion CampaniaHeader:**
```tsx
<div className="mb-8">
  <h1 className="text-4xl font-bold text-white mb-4">
    {campania.titulo}
  </h1>

  <div className="flex items-center gap-3 flex-wrap">
    <Link
      to={`/artistas/${campania.artistaId}`}
      className="flex items-center gap-2 text-primary hover:underline font-medium"
    >
      <Avatar className="w-10 h-10">
        <AvatarImage src={artista.avatar} />
        <AvatarFallback>{artista.nombre[0]}</AvatarFallback>
      </Avatar>
      <span>{artista.nombre}</span>
    </Link>

    {artista.verificado && (
      <Badge className="bg-primary/20 text-primary border-primary/50">
        <CheckCircleIcon className="w-3 h-3 mr-1" />
        Verificado
      </Badge>
    )}

    <Badge
      variant="outline"
      className="bg-[#2d1b4e] text-purple-300 border-purple-500/50"
    >
      {genero.nombre}
    </Badge>

    <Badge
      className={cn(
        campania.estadoCampaniaId === 2 && "bg-green-500/20 text-green-400 border-green-500/50",
        campania.estadoCampaniaId === 3 && "bg-blue-500/20 text-blue-400 border-blue-500/50"
      )}
    >
      {CAMPANIA_ESTADOS_LABELS[campania.estadoCampaniaId]}
    </Badge>
  </div>
</div>
```

**FundingStats**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<div>` | `space-y-3 mb-8` |
| Amount Row | `<div>` | `flex items-baseline gap-2` |
| Current Amount | `<span>` | `text-3xl font-bold text-white` |
| Goal Amount | `<span>` | `text-xl text-[#94a3b8]` |
| Progress Bar | `Progress` | `h-3 bg-[#334155] mb-3` con indicator gradient |
| Percentage | `<p>` | `text-sm font-semibold text-[#94a3b8]` |
| Stats Row | `<div>` | `flex items-center gap-6 text-[#94a3b8]` |
| Stat Item | `<div>` | `flex items-center gap-2` |
| Stat Icon | Icon component | `w-4 h-4` |
| Stat Value | `<span>` | `font-semibold` |

**Composicion FundingStats:**
```tsx
<div className="space-y-3 mb-8">
  <div className="flex items-baseline gap-2">
    <span className="text-3xl font-bold text-white">
      {formatCurrency(campania.importePledgedActual, campania.monedaId)}
    </span>
    <span className="text-xl text-[#94a3b8]">
      de {formatCurrency(campania.importeObjetivo, campania.monedaId)}
    </span>
  </div>

  <Progress
    value={(campania.importePledgedActual / campania.importeObjetivo) * 100}
    className="h-3"
  />

  <p className="text-sm font-semibold text-[#94a3b8]">
    {Math.round((campania.importePledgedActual / campania.importeObjetivo) * 100)}% financiado
  </p>

  <div className="flex items-center gap-6 text-[#94a3b8] flex-wrap">
    <div className="flex items-center gap-2">
      <UsersIcon className="w-4 h-4" />
      <span className="font-semibold">{campania.backersCount || 0}</span>
      <span className="text-sm">backers</span>
    </div>

    {campania.fechaFin && (
      <div className="flex items-center gap-2">
        <ClockIcon className="w-4 h-4" />
        <span className="font-semibold">{getDaysRemaining(campania.fechaFin)}</span>
        <span className="text-sm">días restantes</span>
      </div>
    )}
  </div>
</div>
```

**TabsNavigation**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Tabs Root | `Tabs` | `defaultValue="historia"` |
| Tabs List | `TabsList` | `bg-transparent border-b border-[#334155] rounded-none w-full justify-start` |
| Tabs Trigger | `TabsTrigger` | `text-[#94a3b8] data-[state=active]:text-primary data-[state=active]:border-b-2 data-[state=active]:border-primary` |
| Tabs Content | `TabsContent` | `py-6` |

**Composicion TabsNavigation:**
```tsx
<Tabs defaultValue="historia" className="mb-8">
  <TabsList className="bg-transparent border-b border-[#334155] rounded-none w-full justify-start h-auto p-0">
    <TabsTrigger
      value="historia"
      className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:text-primary text-[#94a3b8] px-4 py-3"
    >
      Historia
    </TabsTrigger>
    <TabsTrigger
      value="actualizaciones"
      className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:text-primary text-[#94a3b8] px-4 py-3"
    >
      Actualizaciones
    </TabsTrigger>
    <TabsTrigger
      value="comentarios"
      className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:text-primary text-[#94a3b8] px-4 py-3"
    >
      Comentarios
    </TabsTrigger>
    <TabsTrigger
      value="faq"
      className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:text-primary text-[#94a3b8] px-4 py-3"
    >
      FAQ
    </TabsTrigger>
  </TabsList>

  <TabsContent value="historia" className="py-6">
    <RichTextContent html={campania.descripcionCompleta} />
  </TabsContent>

  {/* Otros tabs fuera de MVP */}
</Tabs>
```

**RichTextContent**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `<div>` | `prose prose-invert max-w-none text-[#94a3b8]` |
| Images | `<img>` | `rounded-lg my-6 w-full` |
| Checklist Section | `<div>` | `bg-[#0f1729] border-l-4 border-primary p-6 rounded-r-lg my-6` |
| Checklist Item | `<div>` | `flex items-start gap-3 mb-3` |
| Check Icon | Icon | `w-5 h-5 text-primary flex-shrink-0 mt-0.5` |

**Composicion RichTextContent:**
```tsx
<div className="prose prose-invert max-w-none text-[#94a3b8]">
  <div dangerouslySetInnerHTML={{ __html: sanitizedHtml }} />

  {/* Checklist personalizado */}
  <div className="bg-[#0f1729] border-l-4 border-primary p-6 rounded-r-lg my-6">
    <h3 className="text-white font-bold mb-4">¿Por qué necesitamos tu apoyo?</h3>
    <div className="space-y-3">
      {items.map((item, index) => (
        <div key={index} className="flex items-start gap-3">
          <CheckCircleIcon className="w-5 h-5 text-primary flex-shrink-0 mt-0.5" />
          <p className="text-[#94a3b8]">{item}</p>
        </div>
      ))}
    </div>
  </div>
</div>
```

**AboutArtistCard**

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `Card` | `bg-[#0f1729] border-[#334155] p-6 mt-8` |
| Header | `<div>` | `flex items-start gap-4 mb-4` |
| Avatar | `Avatar` | `w-16 h-16` |
| Name | `<h3>` | `text-lg font-bold text-white` |
| Bio | `<p>` | `text-sm text-[#94a3b8] line-clamp-3` |
| Stats Row | `<div>` | `grid grid-cols-2 gap-4 text-sm text-[#64748b] mb-4` |
| View Profile Button | `Button variant="outline" size="sm"` | `w-full border-[#334155] text-[#94a3b8]` |

**Composicion AboutArtistCard:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mt-8">
  <h3 className="text-lg font-bold text-white mb-4">Sobre el Artista</h3>

  <div className="flex items-start gap-4 mb-4">
    <Avatar className="w-16 h-16">
      <AvatarImage src={artista.avatar} />
      <AvatarFallback>{artista.nombre[0]}</AvatarFallback>
    </Avatar>

    <div className="flex-1">
      <h4 className="font-semibold text-white mb-1">{artista.nombre}</h4>
      <p className="text-sm text-[#94a3b8] line-clamp-3">
        {artista.bio}
      </p>
    </div>
  </div>

  <div className="grid grid-cols-2 gap-4 text-sm text-[#64748b] mb-4">
    <div>
      <span className="block font-semibold text-white">{artista.campaniasCount}</span>
      <span>campañas</span>
    </div>
    <div>
      <span className="block font-semibold text-white">
        {formatCurrency(artista.totalRecaudado)}
      </span>
      <span>recaudado</span>
    </div>
  </div>

  <Button
    variant="outline"
    size="sm"
    className="w-full border-[#334155] text-[#94a3b8] hover:text-white"
    asChild
  >
    <Link to={`/artistas/${artista.id}`}>Ver perfil</Link>
  </Button>
</Card>
```

**SidebarSupport** (Sidebar sticky)

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `Card` | `bg-[#0f1729] border-[#334155] p-6 sticky top-24` |
| Support Button | `Button` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2` |
| Starting Price | `<p>` | `text-center text-sm text-[#94a3b8] mb-6` |
| Rewards Title | `<h3>` | `text-lg font-bold text-white mb-4` |
| Separator | `Separator` | `my-6` |
| Security Info Row | `<div>` | `flex items-center gap-2 text-xs text-[#64748b] mb-2` |

**Composicion SidebarSupport:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 sticky top-24">
  <Button
    className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2"
    onClick={handleApoyar}
  >
    Apoyar esta campaña
  </Button>

  <p className="text-center text-sm text-[#94a3b8] mb-6">
    Desde {formatCurrency(minRewardAmount, campania.monedaId)}
  </p>

  <Separator className="my-6" />

  <h3 className="text-lg font-bold text-white mb-4">Recompensas</h3>

  <div className="space-y-3">
    {rewards.map((reward) => (
      <RewardCard key={reward.id} reward={reward} />
    ))}
  </div>

  {rewards.length === 0 && (
    <p className="text-sm text-[#64748b] text-center py-4">
      Esta campaña no tiene recompensas específicas
    </p>
  )}

  <Separator className="my-6" />

  <div className="space-y-2 text-xs text-[#64748b]">
    <div className="flex items-center gap-2">
      <LockIcon className="w-4 h-4" />
      <span>Pago seguro</span>
    </div>
    <div className="flex items-center gap-2">
      <PackageIcon className="w-4 h-4" />
      <span>Entrega: {reward.fechaEntregaEstimada}</span>
    </div>
  </div>
</Card>
```

**RewardCard** (Inside Sidebar)

| Elemento | Componente shadcn | Props/Customizacion |
|----------|-------------------|---------------------|
| Container | `Card` | `bg-[#1a1a2e] border-[#334155] p-4 hover:border-primary cursor-pointer transition` |
| Price | `<div>` | `text-xl font-bold text-primary mb-2` |
| Title | `<h4>` | `text-white font-semibold mb-1` |
| Popular Badge | `Badge` | `bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs` |
| Description | `<p>` | `text-sm text-[#94a3b8] mb-2` |
| Stock Info | `<p>` | `text-xs text-[#64748b]` |
| Select Button | `Button variant="outline" size="sm"` | `w-full border-primary text-primary hover:bg-primary/10` |
| Sold Out Overlay | `<div>` | `opacity-50 cursor-not-allowed` |

**Composicion RewardCard:**
```tsx
<Card
  className={cn(
    "bg-[#1a1a2e] border-[#334155] p-4 hover:border-primary cursor-pointer transition",
    reward.agotado && "opacity-50 cursor-not-allowed"
  )}
  onClick={() => !reward.agotado && handleSelectReward(reward.id)}
>
  <div className="flex items-start justify-between mb-2">
    <div className="text-xl font-bold text-primary">
      {formatCurrency(reward.precio, campania.monedaId)}
    </div>
    {reward.popular && (
      <Badge className="bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs">
        Más popular
      </Badge>
    )}
  </div>

  <h4 className="text-white font-semibold mb-1">{reward.titulo}</h4>

  <p className="text-sm text-[#94a3b8] mb-2 line-clamp-2">
    {reward.descripcion}
  </p>

  <p className="text-xs text-[#64748b] mb-3">
    {reward.cantidadDisponible === null
      ? 'Ilimitadas disponibles'
      : `${reward.cantidadDisponible - reward.cantidadReservada} de ${reward.cantidadDisponible} disponibles`
    }
  </p>

  <Button
    variant="outline"
    size="sm"
    className="w-full border-primary text-primary hover:bg-primary/10"
    disabled={reward.agotado}
  >
    {reward.agotado ? 'Agotado' : 'Seleccionar'}
  </Button>
</Card>
```

**Estados UI - Detalle Campana**

| Estado | Componente Visual |
|--------|------------------|
| Loading Initial | Skeleton para hero image (h-96), title (2 lines), stats (3 bars), content (8 lines), sidebar (4 cards) |
| Loading Rewards | Skeleton cards en sidebar |
| Error 404 | Card centrado con icono SearchX + "Campaña no encontrada" + link "Explorar campañas" |
| Campaign Ended | Status badge "Finalizada" azul, support button disabled con texto "Campaña finalizada" |
| Campaign 100% Funded | Progress bar lleno verde, confetti animation (opcional), badge "Financiada" verde |
| No Rewards | Mensaje en sidebar "Esta campaña no tiene recompensas específicas" |
| Reward Sold Out | Card con opacity 50%, badge "Agotado" gris, button disabled |
| Tab Active | Border bottom primary (2px), text color primary |
| Hover Reward Card | Border color primary, elevation aumenta sutilmente |
| Sticky Sidebar | Position sticky con top-24, scroll independiente |

---

## 4. Variantes y Estados de Componentes

### Button

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| `default` | CTA principal, Apoyar campaña | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| `outline` | Acciones secundarias, Ver perfil, Load More | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| `ghost` | Nav links, icon buttons | `hover:bg-accent hover:text-accent-foreground` |
| `link` | Links inline, "Ver más" | `text-primary hover:underline` |

**Estados Button:**
- `default` - Estado normal
- `hover` - Gradient más brillante (from-pink-600 to-purple-700), scale 1.02
- `active` - Scale 0.98
- `disabled` - Opacity 50%, cursor not-allowed, no hover effects
- `loading` - Spinner icon (Loader2) animado, texto "Cargando...", disabled

### Badge

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| `default` (primary) | Destacados, Popular | `bg-primary/20 text-primary border-primary/50` |
| `outline` | Neutral, Genre | `border-foreground text-foreground` |
| Status Active | Campaña activa | `bg-green-500/20 text-green-400 border-green-500/50` |
| Status Completed | Campaña finalizada | `bg-blue-500/20 text-blue-400 border-blue-500/50` |
| Status Cancelled | Campaña cancelada | `bg-red-500/20 text-red-400 border-red-500/50` |
| Verified | Artista verificado | `bg-primary/20 text-primary border-primary/50` |

### Card

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| Default | Tarjetas de campaña, about artist | `bg-[#0f1729] border-[#334155]` |
| Hover | Card interactivo (clickable) | `hover:border-[#a855f7] transition cursor-pointer` |
| Sidebar | Sidebar sticky con padding mayor | `bg-[#0f1729] border-[#334155] p-6 sticky top-24` |
| Reward | Card dentro de sidebar | `bg-[#1a1a2e] border-[#334155] hover:border-primary` |

### Progress

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| Default | Progress bar de funding | `h-3 bg-[#334155]` con indicator `bg-gradient-to-r from-pink-500 to-purple-600` |
| Mini | Progress en cards de grid | `h-2 bg-[#334155]` |
| Success (100%) | Campaña financiada | Indicator `bg-gradient-to-r from-green-400 to-green-600` |

### Avatar

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| Small | En cards de grid | `w-5 h-5` |
| Medium | Header de detalle | `w-10 h-10` |
| Large | About artist card | `w-16 h-16` |

### Skeleton

| Variante | Uso | Clases Tailwind |
|----------|-----|-----------------|
| Hero Image | Loading hero | `w-full h-96` |
| Card | Loading campaign card | `w-full h-80` |
| Line | Loading text | `h-4 w-full`, `h-4 w-3/4`, etc. |
| Circle | Loading avatar | `w-10 h-10 rounded-full` |

---

## 5. Responsive Design

| Breakpoint | Width | Cambios Clave |
|------------|-------|---------------|
| **Mobile** | < 640px | Nav colapsado a hamburger, Grid 1 columna, Hero h-64, Sidebar abajo de content, Stats en grid 2x2, Font sizes -2px |
| **Tablet** | 640-1024px | Grid 2 columnas, Hero h-80, Sidebar visible en columna derecha (grid 3 cols → 2+1), Nav visible pero compacto |
| **Desktop** | > 1024px | Grid 4 columnas, Hero h-96, Layout completo 2+1, Max-width containers (max-w-7xl), Spacing amplio |

### Detalle Campana Responsive

**Mobile (< 640px):**
```tsx
<div className="max-w-7xl mx-auto px-4 py-6">
  {/* Hero */}
  <div className="w-full h-64 mb-6">...</div>

  {/* Layout columna unica */}
  <div className="space-y-6">
    {/* Content first */}
    <div>
      <CampaniaHeader />
      <FundingStats />
      <TabsNavigation />
      <RichTextContent />
      <AboutArtistCard />
    </div>

    {/* Sidebar después */}
    <div>
      <SidebarSupport />
    </div>
  </div>
</div>
```

**Desktop (> 1024px):**
```tsx
<div className="max-w-7xl mx-auto px-8 py-8">
  {/* Hero */}
  <div className="w-full h-96 mb-8">...</div>

  {/* Grid 3 columnas (2+1) */}
  <div className="grid grid-cols-3 gap-8">
    {/* Content - 2 columnas */}
    <div className="col-span-2">
      <CampaniaHeader />
      <FundingStats />
      <TabsNavigation />
      <RichTextContent />
      <AboutArtistCard />
    </div>

    {/* Sidebar - 1 columna sticky */}
    <div className="col-span-1">
      <SidebarSupport />
    </div>
  </div>
</div>
```

### Explorar Campanas Responsive

**Grid responsivo:**
```tsx
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
  {/* Mobile: 1 columna */}
  {/* Tablet: 2 columnas */}
  {/* Desktop: 4 columnas */}
</div>
```

---

## 6. Accesibilidad (ARIA)

### Requisitos WCAG

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de color | Minimo 4.5:1 para texto normal. white (#fff) / bg-primary (#1a1a2e) = 15.8:1 ✓ |
| Focus visible | Ring 2px `ring-[#a855f7]` con offset 2px en todos los interactivos |
| Labels semanticos | Todos los links/buttons con aria-label descriptivo |
| Images | `alt` text descriptivo: "Imagen de portada de {titulo}" |
| Progress bars | `role="progressbar"` con `aria-valuenow`, `aria-valuemin`, `aria-valuemax` |
| Tabs | `role="tablist"`, `role="tab"`, `aria-selected` en tab activo |
| Keyboard navigation | Tab order logico, Enter para select/submit, Esc para cerrar modals |
| Screen reader | Anuncios de cambios con `aria-live="polite"` |

### ARIA Labels Especificos

**Progress Bar (Funding):**
```tsx
<Progress
  value={percentage}
  className="h-3"
  role="progressbar"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
  aria-label={`Progreso de financiación: ${percentage}% alcanzado`}
/>
```

**Status Badge:**
```tsx
<Badge
  role="status"
  aria-label={`Estado de campaña: ${estadoLabel}`}
>
  {estadoLabel}
</Badge>
```

**Reward Card:**
```tsx
<Card
  role="article"
  aria-label={`Recompensa: ${reward.titulo} por ${formatCurrency(reward.precio)}`}
  onClick={handleSelect}
  tabIndex={0}
  onKeyDown={(e) => e.key === 'Enter' && handleSelect()}
>
  {/* ... */}
</Card>
```

**Button Loading State:**
```tsx
<Button
  disabled={isPending}
  aria-busy={isPending}
  aria-label={isPending ? 'Cargando contenido' : 'Apoyar esta campaña'}
>
  {isPending && <Loader2 className="animate-spin mr-2" aria-hidden="true" />}
  {isPending ? 'Cargando...' : 'Apoyar esta campaña'}
</Button>
```

**Tabs Navigation:**
```tsx
<Tabs defaultValue="historia">
  <TabsList role="tablist" aria-label="Secciones de la campaña">
    <TabsTrigger
      value="historia"
      role="tab"
      aria-selected={activeTab === 'historia'}
      aria-controls="panel-historia"
    >
      Historia
    </TabsTrigger>
    {/* ... */}
  </TabsList>

  <TabsContent
    value="historia"
    role="tabpanel"
    id="panel-historia"
    aria-labelledby="tab-historia"
  >
    {/* ... */}
  </TabsContent>
</Tabs>
```

---

## 7. Animaciones y Transiciones

| Elemento | Animacion | Duracion | Easing |
|----------|-----------|----------|--------|
| Button hover | Scale 1.02 + shadow glow | 150ms | ease |
| Card hover | Border color change + elevation | 200ms | ease |
| Page transition | Fade in opacity 0 → 1 | 300ms | ease-in-out |
| Progress bar fill | Width transition animated | 600ms | ease-out |
| Skeleton pulse | Opacity 0.5 → 1 → 0.5 loop | 1500ms | ease-in-out |
| Badge appearance | Fade in + scale 0.9 → 1 | 200ms | ease |
| Tab change | Content fade out → fade in | 200ms | ease-in-out |
| Image load | Fade in + scale 0.95 → 1 | 300ms | ease |
| Modal open | Scale 0.95 → 1 + fade in | 200ms | ease-out |
| Confetti (100% funded) | Particles fall from top | 3000ms | ease-out |

### CSS Animations

```css
/* Button hover glow effect */
@keyframes shadowGlow {
  from { box-shadow: 0 0 10px rgba(168, 85, 247, 0.3); }
  to { box-shadow: 0 0 20px rgba(168, 85, 247, 0.5); }
}

.button-gradient:hover {
  animation: shadowGlow 150ms ease forwards;
}

/* Skeleton pulse */
@keyframes pulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

.skeleton {
  animation: pulse 1500ms ease-in-out infinite;
}

/* Confetti (opcional para 100% funded) */
@keyframes confetti-fall {
  from {
    transform: translateY(-100vh) rotate(0deg);
    opacity: 1;
  }
  to {
    transform: translateY(100vh) rotate(360deg);
    opacity: 0;
  }
}
```

---

## 8. Estados de Carga (Loading States)

### Skeleton Loaders

**CampaniaCard Skeleton:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-0 overflow-hidden">
  <Skeleton className="w-full h-48" />
  <div className="p-4 space-y-3">
    <Skeleton className="h-6 w-3/4" />
    <div className="flex items-center gap-2">
      <Skeleton className="w-5 h-5 rounded-full" />
      <Skeleton className="h-4 w-24" />
    </div>
    <Skeleton className="h-2 w-full" />
    <div className="flex justify-between">
      <Skeleton className="h-4 w-20" />
      <Skeleton className="h-4 w-16" />
    </div>
  </div>
</Card>
```

**Detalle Campana Skeleton:**
```tsx
<div className="max-w-7xl mx-auto px-6 py-8">
  {/* Hero skeleton */}
  <Skeleton className="w-full h-96 mb-8" />

  <div className="grid grid-cols-3 gap-8">
    <div className="col-span-2 space-y-6">
      {/* Title */}
      <Skeleton className="h-10 w-3/4" />

      {/* Artist info */}
      <div className="flex items-center gap-3">
        <Skeleton className="w-10 h-10 rounded-full" />
        <Skeleton className="h-4 w-32" />
        <Skeleton className="h-5 w-20" />
      </div>

      {/* Stats */}
      <div className="space-y-3">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-3 w-full" />
        <div className="flex gap-6">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-4 w-32" />
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-4 border-b border-[#334155] pb-2">
        <Skeleton className="h-6 w-20" />
        <Skeleton className="h-6 w-32" />
        <Skeleton className="h-6 w-28" />
      </div>

      {/* Content lines */}
      <div className="space-y-3">
        {[...Array(8)].map((_, i) => (
          <Skeleton key={i} className="h-4 w-full" />
        ))}
      </div>
    </div>

    <div className="col-span-1 space-y-4">
      {/* Sidebar skeleton */}
      <Skeleton className="h-12 w-full" />
      <Skeleton className="h-4 w-24 mx-auto" />
      <Skeleton className="h-32 w-full" />
      <Skeleton className="h-32 w-full" />
    </div>
  </div>
</div>
```

### Empty States

**No Campanias Found:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-12 text-center">
  <FolderOpenIcon className="w-16 h-16 text-[#64748b] mx-auto mb-4" />
  <h3 className="text-xl font-bold text-white mb-2">
    No se encontraron campañas
  </h3>
  <p className="text-[#94a3b8] mb-6">
    Intenta ajustar tus filtros o busca otros términos
  </p>
  <Button
    variant="outline"
    onClick={clearFilters}
    className="border-[#334155] text-[#94a3b8]"
  >
    Ver todas las campañas
  </Button>
</Card>
```

**Campania Not Found (404):**
```tsx
<div className="max-w-2xl mx-auto px-6 py-24 text-center">
  <SearchXIcon className="w-24 h-24 text-[#64748b] mx-auto mb-6" />
  <h1 className="text-3xl font-bold text-white mb-4">
    Campaña no encontrada
  </h1>
  <p className="text-lg text-[#94a3b8] mb-8">
    Esta campaña no existe o fue eliminada
  </p>
  <Button
    className="bg-gradient-to-r from-pink-500 to-purple-600"
    asChild
  >
    <Link to="/explorar">Explorar campañas</Link>
  </Button>
</div>
```

**No Rewards:**
```tsx
<div className="text-center py-8">
  <PackageIcon className="w-12 h-12 text-[#64748b] mx-auto mb-3" />
  <p className="text-sm text-[#64748b]">
    Esta campaña no tiene recompensas específicas
  </p>
</div>
```

### Error States

**Error Fetching Campanias:**
```tsx
<Alert variant="destructive" className="max-w-2xl mx-auto">
  <AlertCircleIcon className="h-4 w-4" />
  <AlertTitle>Error al cargar campañas</AlertTitle>
  <AlertDescription>
    Hubo un problema al cargar las campañas. Por favor, intenta nuevamente.
  </AlertDescription>
  <Button
    variant="outline"
    size="sm"
    onClick={retry}
    className="mt-4"
  >
    Reintentar
  </Button>
</Alert>
```

---

## 9. Props Interfaces

### CampaniaCard

```typescript
interface CampaniaCardProps {
  campania: CampaniaListItem;
  onClick?: (id: string) => void;
  className?: string;
}
```

### CampaniaHeader

```typescript
interface CampaniaHeaderProps {
  campania: Campania;
  artista: {
    id: string;
    nombre: string;
    avatar?: string;
    verificado: boolean;
  };
  genero: {
    id: number;
    nombre: string;
  };
}
```

### FundingStats

```typescript
interface FundingStatsProps {
  importePledgedActual: number;
  importeObjetivo: number;
  monedaId: number;
  backersCount: number;
  fechaFin?: string;
  className?: string;
}
```

### RewardCard

```typescript
interface RewardCardProps {
  reward: {
    id: string;
    precio: number;
    titulo: string;
    descripcion: string;
    cantidadDisponible: number | null;
    cantidadReservada: number;
    popular?: boolean;
    agotado: boolean;
  };
  monedaId: number;
  onSelect: (rewardId: string) => void;
  className?: string;
}
```

### SidebarSupport

```typescript
interface SidebarSupportProps {
  campaniaId: string;
  rewards: Reward[];
  monedaId: number;
  minRewardAmount: number;
  onApoyar: () => void;
  className?: string;
}
```

### AboutArtistCard

```typescript
interface AboutArtistCardProps {
  artista: {
    id: string;
    nombre: string;
    avatar?: string;
    bio: string;
    campaniasCount: number;
    totalRecaudado: number;
  };
  className?: string;
}
```

---

## 10. Customizaciones de Tema Oscuro

### Tailwind Config Extensions

```javascript
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        'bg-primary': '#1a1a2e',
        'bg-secondary': '#16213e',
        'bg-card': '#0f1729',
        'bg-card-hover': '#1e2a42',
        'text-primary': '#ffffff',
        'text-secondary': '#94a3b8',
        'text-muted': '#64748b',
        'text-label': '#cbd5e1',
        'border-primary': '#334155',
        'border-focus': '#a855f7',
      },
      backgroundImage: {
        'gradient-primary': 'linear-gradient(135deg, #ec4899 0%, #a855f7 100%)',
        'gradient-primary-hover': 'linear-gradient(135deg, #f472b6 0%, #c084fc 100%)',
      },
    },
  },
  // ...
}
```

### CSS Variables Override

```css
/* globals.css */
:root {
  /* Shadcn overrides for dark theme */
  --background: #1a1a2e;
  --foreground: #ffffff;
  --card: #0f1729;
  --card-foreground: #ffffff;
  --popover: #0f1729;
  --popover-foreground: #ffffff;
  --primary: #a855f7;
  --primary-foreground: #ffffff;
  --secondary: #334155;
  --secondary-foreground: #ffffff;
  --muted: #64748b;
  --muted-foreground: #94a3b8;
  --accent: #1e2a42;
  --accent-foreground: #ffffff;
  --destructive: #ef4444;
  --destructive-foreground: #ffffff;
  --border: #334155;
  --input: #334155;
  --ring: #a855f7;
  --radius: 0.5rem;
}

/* Progress gradient customization */
.progress-indicator {
  background: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
}

.progress-indicator.success {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
}
```

---

## 11. Checklist de Implementacion

### Explorar Campanas
- [ ] HeaderNav sticky con logo, links, buttons
- [ ] SearchAndFilters con Input de busqueda
- [ ] CampaniaCard con imagen, title, artist, progress, stats
- [ ] CampaniaGrid responsive (1-2-4 columnas)
- [ ] Load More button con loading state
- [ ] Skeleton loaders (8 cards)
- [ ] Empty state con icono y CTA
- [ ] Error state con Alert destructive
- [ ] Hover effects en cards (border primary, elevation)
- [ ] Click en card navega a detalle
- [ ] Click en artista navega a perfil (stopPropagation)
- [ ] Progress bar con gradient pink-purple
- [ ] Badges de estado con colores correctos
- [ ] Avatar con fallback inicial
- [ ] Responsive mobile (1 col), tablet (2 cols), desktop (4 cols)
- [ ] Accesibilidad: ARIA labels, keyboard navigation

### Detalle Campana
- [ ] HeroImage full width (h-96) con skeleton loader
- [ ] CampaniaHeader con titulo, artista, badges
- [ ] FundingStats con amount, progress bar, stats row
- [ ] TabsNavigation (Historia, Actualizaciones, Comentarios, FAQ)
- [ ] RichTextContent con prose-invert, imagenes, checklist
- [ ] AboutArtistCard con avatar, bio, stats, link perfil
- [ ] SidebarSupport sticky (top-24)
- [ ] Support button gradient con hover effect
- [ ] RewardCard en sidebar con precio, title, stock, select button
- [ ] Popular badge en reward destacado
- [ ] Sold out state en rewards agotados
- [ ] No rewards message cuando aplique
- [ ] Security info en footer sidebar (lock, package icons)
- [ ] Progress bar con aria-valuenow
- [ ] Tabs con role tablist y aria-selected
- [ ] Reward cards con role article y aria-label
- [ ] Layout grid 3 columnas (2+1) en desktop
- [ ] Responsive mobile (columna unica, sidebar abajo)
- [ ] Skeleton loaders para hero, title, content, sidebar
- [ ] Error 404 con mensaje y link explorar
- [ ] Campaign ended state (button disabled)
- [ ] 100% funded state (progress green, confetti opcional)
- [ ] Hover effects en rewards (border primary)
- [ ] Sticky header nav funcional
- [ ] All images con alt text descriptivo

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Gradient buttons (pink → purple) en CTAs principales
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Componentes shadcn correctos importados
- [ ] cn() utility para merge classNames
- [ ] Animaciones smooth (150-600ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Keyboard navigation (Tab, Enter, Esc)
- [ ] Focus states con ring purple visible
- [ ] Form validation (futuro para filtros)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders para loading states
- [ ] Empty states con iconos y CTAs
- [ ] Error boundaries (nivel app)
- [ ] Toast notifications para acciones
- [ ] React Router navigation configurado
- [ ] TanStack Query para fetching
- [ ] TypeScript strict mode
- [ ] Props interfaces definidas

---

## 12. Notas Tecnicas

### Utilidades Compartidas

Estas utilidades deben existir en `src/shared/utils/`:

```typescript
// formatters.ts
export function formatCurrency(amount: number, monedaId: number): string {
  const symbol = MONEDA_SYMBOLS[monedaId] || '€';
  return `${symbol} ${amount.toLocaleString('es-ES', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`;
}

export function getDaysRemaining(fechaFin: string): number {
  const now = new Date();
  const end = new Date(fechaFin);
  const diff = end.getTime() - now.getTime();
  return Math.ceil(diff / (1000 * 60 * 60 * 24));
}

// cn.ts (ya existe)
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
```

### Componentes No Implementados en MVP

Los siguientes componentes están fuera del MVP pero se reserva espacio en el UI:

- **Filtros avanzados** (Select por género, estado) - Espacio reservado en SearchAndFilters
- **Tabs de Actualizaciones, Comentarios, FAQ** - Solo Historia implementado
- **Video embed** (si `videoPrincipalUrl` existe) - Se muestra link o se omite
- **Share buttons** - Futuro
- **Favoritos/Bookmark** - Futuro

### Integracion con Backend

**Query Keys (TanStack Query):**
```typescript
// src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  campanias: {
    all: ['campanias'] as const,
    byId: (id: string) => ['campanias', id] as const,
    list: (filters: any) => ['campanias', 'list', filters] as const,
  },
  artistas: {
    byId: (id: string) => ['artistas', id] as const,
  },
  rewards: {
    byCampaniaId: (campaniaId: string) => ['rewards', 'campania', campaniaId] as const,
  },
} as const;
```

**Services (API calls):**
```typescript
// src/features/campanias/services/campania.service.ts
import { API_ROUTES } from '@/shared/constants/api-routes';
import { apiClient } from '@/lib/api-client';

export const campaniaService = {
  getAll: (params?: { searchTerm?: string; pageNumber?: number; pageSize?: number }) =>
    apiClient.get<ServiceResponse<CampaniaListItem[]>>(API_ROUTES.campanias.base, { params }),

  getById: (id: string) =>
    apiClient.get<ServiceResponse<Campania>>(API_ROUTES.campanias.byId(id)),
};
```

### Optimizaciones de Rendimiento

1. **Image lazy loading**: Usar `loading="lazy"` en todas las imagenes
2. **Virtualization**: Si la lista de campañas es muy larga (>100), considerar `react-virtual`
3. **Debounce search**: Input de busqueda con debounce de 300ms
4. **Infinite scroll**: Como alternativa a "Load More" button
5. **Memoization**: Usar `React.memo` en CampaniaCard para evitar re-renders

---

## Resumen Final

Este plan de diseño UI define:
- **18 componentes shadcn/ui** utilizados (Button, Card, Badge, Progress, Avatar, Skeleton, Input, Label, Textarea, Separator, Tabs, Alert)
- **12 composiciones custom** (CampaniaCard, CampaniaHeader, FundingStats, RewardCard, SidebarSupport, AboutArtistCard, etc.)
- **Dark theme completo** con paleta oscura (#1a1a2e, #0f1729, gradient pink-purple)
- **Responsive en 3 breakpoints** (mobile < 640px, tablet 640-1024px, desktop > 1024px)
- **Accesibilidad WCAG AA** con ARIA labels, contraste 4.5:1, keyboard navigation
- **Estados completos**: loading (skeletons), empty, error, hover, disabled, 100% funded
- **Props interfaces TypeScript** para todos los componentes custom
- **Animaciones y transiciones** smooth (150-600ms)

**Archivos generados:**
- `C:\Repos\WePlay_Rises\plans\crear-campania\frontend-landing\ui-design.md`

**Componentes shadcn requeridos:**
- Button, Card (Header, Title, Content, Footer), Badge, Progress, Avatar (Image, Fallback), Skeleton, Input, Label, Textarea, Separator, Tabs (List, Trigger, Content), Alert (Title, Description)

**Consideraciones de accesibilidad:**
- Todos los elementos interactivos con focus visible (ring purple)
- Progress bars con role="progressbar" y aria-valuenow
- Tabs con role="tablist" y aria-selected
- Buttons con aria-busy en loading states
- Images con alt text descriptivo
- Contraste minimo 4.5:1 en todos los textos
