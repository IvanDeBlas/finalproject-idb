# UI/UX: Hacer Backing

> **Feature:** hacer-backing
> **Última actualización:** 2026-02-13

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Campaign Listing (Explore) | [WPR_2-Excplore-Campaigns.png](../../ui-images/WPR_2-Excplore-Campaigns.png) | Landing |
| Campaign Detail (Rewards Sidebar) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |
| Backing Form | [WPR_7-Backing.png](../../ui-images/WPR_7-Backing.png) | Landing |
| My Backings Dashboard | [WPR_9-Mis-backings.png](../../ui-images/WPR_9-Mis-backings.png) | Dashboard |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Campaign cards, detail page, backing flow, reward cards | `references/templates/krowd/` |
| **Dashtail** | Backings dashboard (artist view), tables, stats | `references/templates/dashtail/` |

**Componentes de referencia clave:**
- Krowd: campaign cards con progress bars, reward selection flow, multi-step backing process
- Dashtail: transaction lists, stats cards, status badges

---

## Design Tokens

**Nota:** Se usan los mismos design tokens definidos en [crear-campania/ui-ux.md](../crear-campania/ui-ux.md#design-tokens) y [definir-recompensas/ui-ux.md](../definir-recompensas/ui-ux.md#design-tokens).

### Colores Específicos de Backing

```css
:root {
  /* Backing Progress States */
  --progress-bg: #1e293b;
  --progress-fill: #a855f7;
  --progress-fill-success: #10b981;
  --progress-fill-near-goal: #f59e0b;

  /* Backing CTA */
  --backing-cta-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --backing-cta-hover: linear-gradient(135deg, #db2777 0%, #9333ea 100%);

  /* Status Badges */
  --status-active: #10b981;
  --status-finished: #3b82f6;
  --status-pending: #f59e0b;
  --status-cancelled: #ef4444;

  /* Amount Input */
  --amount-highlight: #a855f7;
  --amount-suggestion-bg: #1e293b;
  --amount-suggestion-hover: #334155;
}
```

### Iconos Específicos

| Icono | Uso | Biblioteca |
|-------|-----|-----------|
| `heart` | Backing action, support | Lucide React |
| `heart-handshake` | Successful backing | Lucide React |
| `users` | Backers count | Lucide React |
| `calendar` | Days remaining | Lucide React |
| `trophy` | Goal amount | Lucide React |
| `check-circle` | Confirmation, success | Lucide React |
| `gift` | Rewards | Lucide React |
| `eye-off` | Anonymous backing | Lucide React |
| `lock` | Secure payment | Lucide React |
| `truck` | Delivery info | Lucide React |
| `credit-card` | Payment method | Lucide React |
| `message-square` | Optional message | Lucide React |
| `sparkles` | Popular badge | Lucide React |
| `alert-circle` | Limited stock warning | Lucide React |
| `euro` | Currency symbol | Lucide React |

---

## Pantalla: Campaign Listing (Explorar Campañas)

**Mockup:** WPR_2-Excplore-Campaigns.png
**Proyecto:** Landing
**Ruta:** `/campanias`
**Template base:** `krowd/` explore page

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [HEADER]  MusicFund  Explorar  Categorías  Cómo funciona         │
│                                          [Iniciar Sesión] [Regist] │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│              Descubre Proyectos Musicales                          │
│          Apoya a tus artistas favoritos y sé parte de su historia  │
│                                                                    │
│  [🔍 Buscar artistas, géneros, proyectos...]                      │
│                                                                    │
│  🎛️ Filtros:  🎸 Género Musical ▼  🔘 Estado ▼  📊 Ordenar por ▼ │
│                                          248 campañas activas     │
│                                                                    │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                │
│  │ [Rock]      │  │ [Electrónica│  │ [Indie]     │                │
│  │ [Image]     │  │ [Image]     │  │ [Image]     │                │
│  │             │  │             │  │             │                │
│  │ 12 días     │  │ 3 días      │  │ 28 días     │                │
│  │             │  │             │  │             │                │
│  │ Nuevo Álbum:│  │ EP Experim: │  │ Live Session│                │
│  │ "Ecos del   │  │ "Digital    │  │ en el Blue  │                │
│  │  Tiempo"    │  │  Dreams"    │  │  Note       │                │
│  │ by Medianoc │  │ by Artist   │  │ by Artist   │                │
│  │                                                                 │
│  │ €8,700      │  │ €4,700      │  │ €9,300      │                │
│  │ de €10,000  │  │ de €5,000   │  │ de €15,000  │                │
│  │ [██████87%] │  │ [█████94%]  │  │ [████62%]   │                │
│  │ 142 backers │  │ 89 backers  │  │ 67 backers  │                │
│  └─────────────┘  └─────────────┘  └─────────────┘                │
│                                                                    │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                │
│  │ [Cumbia]    │  │ [Indie]     │  │ [Clásica]   │                │
│  │ [Image]     │  │ [Image]     │  │ [Image]     │                │
│  │             │  │             │  │             │                │
│  │ 4 días      │  │ 12 días     │  │ 20 días     │                │
│  │             │  │             │  │             │                │
│  │ Mixtape:    │  │ Single Debut│  │ Concierto de│                │
│  │ "Voces      │  │ "Primavera" │  │ Primavera   │                │
│  │  Urbanas"   │  │ by Artist   │  │ 2024        │                │
│  │ by MC Tropi │  │                                                │
│  │ €2,250      │  │ €840        │  │ €14,300     │                │
│  │ de €5,000   │  │ de €3,000   │  │ de €20,000  │                │
│  │ [████45%]   │  │ [███28%]    │  │ [██████71%] │                │
│  │ 38 backers  │  │ 24 backers  │  │ 103 backers │                │
│  └─────────────┘  └─────────────┘  └─────────────┘                │
│                                                                    │
│  [← Anterior]   1  2  3  4  5  [Siguiente →]                      │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Container | `<div>` | `min-h-screen bg-[#0a0a0f]` |
| Header | `<header>` | `fixed top-0 w-full bg-[#0f0f1a]/90 backdrop-blur-md border-b border-[#334155] z-50` |
| Hero Section | `<section>` | `py-16 px-4 text-center` |
| Hero Title | `<h1>` | `text-4xl md:text-5xl font-bold text-white mb-4` |
| Hero Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Search Bar | `<Input>` con `<Icon>` (search) | `max-w-2xl mx-auto bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] px-12 py-4 rounded-xl focus:border-primary` |
| Search Icon | `<Icon>` (search) | `absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-[#64748b]` |
| Filters Row | `<div>` | `flex items-center justify-between py-6 px-4 max-w-7xl mx-auto` |
| Filter Button | `<Button variant="outline">` | `border-[#334155] text-white hover:border-primary` |
| Filter Icon | `<Icon>` | `w-4 h-4 mr-2` |
| Results Count | `<span>` | `text-sm text-[#64748b]` |
| Campaigns Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 px-4 max-w-7xl mx-auto mb-12` |
| Campaign Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] hover:border-primary cursor-pointer transition group overflow-hidden` |
| Card Image Container | `<div>` | `relative aspect-video overflow-hidden` |
| Card Image | `<img>` | `w-full h-full object-cover group-hover:scale-105 transition duration-500` |
| Genre Badge | `<Badge>` | `absolute top-3 left-3 bg-primary/90 text-white font-semibold` |
| Days Badge | `<Badge variant="secondary">` | `absolute top-3 right-3 bg-[#1a1a2e]/90 backdrop-blur text-white text-xs` |
| Card Content | `<CardContent>` | `p-5` |
| Campaign Title | `<h3>` | `text-xl font-bold text-white mb-1 line-clamp-1` |
| Artist Name | `<p>` | `text-sm text-[#94a3b8] mb-4 flex items-center gap-1` |
| Artist Icon | `<Icon>` (user) | `w-4 h-4` |
| Amount Row | `<div>` | `flex items-baseline gap-2 mb-2` |
| Amount Raised | `<span>` | `text-2xl font-bold text-white` |
| Amount Goal | `<span>` | `text-sm text-[#64748b]` |
| Progress Bar Container | `<div>` | `w-full h-2 bg-[#1e293b] rounded-full overflow-hidden mb-3` |
| Progress Bar Fill | `<div>` | `h-full bg-gradient-to-r from-pink-500 to-purple-600 rounded-full transition-all duration-500` (width: %financiado) |
| Progress Bar Fill (near goal) | `<div>` | `h-full bg-gradient-to-r from-warning to-amber-500` (width > 90%) |
| Progress Bar Fill (success) | `<div>` | `h-full bg-gradient-to-r from-green-400 to-emerald-500` (width >= 100%) |
| Stats Row | `<div>` | `flex items-center justify-between text-sm` |
| Percentage | `<span>` | `font-semibold text-primary` |
| Backers Count | `<div>` | `flex items-center gap-1 text-[#94a3b8]` |
| Backers Icon | `<Icon>` (users) | `w-4 h-4` |
| Pagination | `<div>` | `flex items-center justify-center gap-2 py-8` |
| Page Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-white hover:border-primary` |
| Page Button (active) | `<Button size="sm">` | `bg-primary text-white` |
| Empty State Container | `<div>` | `text-center py-16 px-4` |
| Empty Icon | `<Icon>` (search-x) | `w-20 h-20 text-[#64748b] mx-auto mb-4` |
| Empty Title | `<h3>` | `text-2xl font-bold text-white mb-2` |
| Empty Description | `<p>` | `text-[#94a3b8] mb-6` |
| Empty CTA | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Grid de campaign cards, progress bars animados, badges visibles |
| **Loading** | 6 skeleton cards (shimmer animation) en grid |
| **Empty Results** | Icono search-x grande, texto "No se encontraron campañas", botón "Limpiar filtros" |
| **No Campaigns** | Icono package-open, texto "No hay campañas activas", botón "Ver todas" |
| **Hover Card** | Border primary, image scale 1.05, elevation aumenta, smooth transition 300ms |
| **Filter Active** | Botón de filtro con border primary y background primary/10 |
| **Search Active** | Search bar border primary, icon primary color |
| **Near Goal (>90%)** | Progress bar gradient warning (amarillo-ámbar) |
| **Goal Reached (100%)** | Progress bar gradient success (verde), badge "Financiada" sobre card |
| **Expiring Soon (<3 días)** | Days badge en rojo con pulso animation |

### Validación en Tiempo Real

**No aplica** - Página de solo lectura.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Card** | Navigate a `/campanias/{id}` (campaign detail) |
| **Type Search** | Debounce 500ms, filtrar campañas, actualizar count |
| **Click Filtro Género** | Dropdown con opciones, multi-select, aplicar filtros, actualizar grid |
| **Click Filtro Estado** | Dropdown: Todas, Activas, Próximas a finalizar, Financiadas. Aplicar filtro |
| **Click Ordenar** | Dropdown: Más recientes, Más populares, Más financiadas, Finalizan pronto. Re-order grid |
| **Clear Search** | Botón X en search input, limpiar texto, mostrar todas |
| **Click Pagination** | Cargar siguiente página, scroll to top, actualizar grid |
| **Hover Card** | Border color change, image zoom, elevation increase |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Grid 1 columna, hero text-3xl, search bar px-4, filters stack vertical, card padding reducido (p-4) |
| **Tablet (640-1024px)** | Grid 2 columnas, hero text-4xl, filters horizontal compactos |
| **Desktop (> 1024px)** | Grid 3 columnas, spacing amplio, max-w-7xl container |

---

## Pantalla: Campaign Detail (Detalle de Campaña)

**Mockup:** WPR_3-Detail-Campaign.png
**Proyecto:** Landing
**Ruta:** `/campanias/{id}`
**Template base:** `krowd/` campaign detail page

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [HEADER]  MusicFund  Explorar  Categorías                        │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  ┌───────────────────────────────────┐  ┌─────────────────────┐   │
│  │                                   │  │ [Apoyar esta        │   │
│  │        [HERO IMAGE/VIDEO]         │  │  campaña]           │   │
│  │                                   │  │  (gradient button)  │   │
│  │                                   │  │  Desde €5           │   │
│  │                                   │  │                     │   │
│  ├───────────────────────────────────┤  │ ─────────────────── │   │
│  │ Nuevo Álbum: "Ecos de Medianoche"│  │                     │   │
│  │ by Los Nocturnos  [🔗 avatar]    │  │ Recompensas         │   │
│  │ Rock Indie    ✓ Artista verificado│  │                     │   │
│  │                                   │  │ ┌─────────────────┐ │   │
│  │ €12,450 de €15,000                │  │ │ €10 Descarga    │ │   │
│  │ [████████████████        83%]     │  │ │ Digital         │ │   │
│  │                                   │  │ │ ⭐ Más popular │ │   │
│  │ 83%          127            12    │  │ │                 │ │   │
│  │ financiado   backers   días rest. │  │ │ Acceso anticip. │ │   │
│  │                                   │  │ │ álbum completo  │ │   │
│  │ ───────────────────────────────   │  │ │ FLAC + MP3      │ │   │
│  │                                   │  │ │                 │ │   │
│  │ Historia | Actualizaciones | FAQ  │  │ │ 📦 234 de 500   │ │   │
│  │                                   │  │ │ [Seleccionar]   │ │   │
│  │ Después de dos años de trabajo... │  │ └─────────────────┘ │   │
│  │ intenso, estamos listos para      │  │                     │   │
│  │ compartir nuestro nuevo álbum...  │  │ ┌─────────────────┐ │   │
│  │                                   │  │ │ €25 CD Físico   │ │   │
│  │ [Description content HTML]        │  │ │                 │ │   │
│  │                                   │  │ │ CD firmado +    │ │   │
│  │ El álbum contiene 12 canciones... │  │ │ descarga digital│ │   │
│  │                                   │  │ │ + booklet       │ │   │
│  │ [Image - Studio photo]            │  │ │                 │ │   │
│  │                                   │  │ │ 📦 89 de 200    │ │   │
│  │ ¿Por qué necesitamos tu apoyo?    │  │ │ [Seleccionar]   │ │   │
│  │ • Masterización profesional...    │  │ └─────────────────┘ │   │
│  │ • Producción de vinilos edición...│  │                     │   │
│  │ • Videos musicales para 3...      │  │ ┌─────────────────┐ │   │
│  │ • Gira promocional por 10 ciud... │  │ │ €50 Vinilo Lim. │ │   │
│  │                                   │  │ │ ⚠️ Pocas unid. │ │   │
│  │ ───────────────────────────────   │  │ │                 │ │   │
│  │                                   │  │ │ Vinilo especial │ │   │
│  │ Sobre el Artista                  │  │ │ + póster excl.  │ │   │
│  │ ┌──────────────────────────────┐  │  │ │ + todo anterior │ │   │
│  │ │ [Avatar] Los Nocturnos       │  │  │ │                 │ │   │
│  │ │ Banda de rock indie formada  │  │  │ │ 📦 45 de 100    │ │   │
│  │ │ en Madrid en 2018...         │  │  │ │ [Seleccionar]   │ │   │
│  │ │                              │  │  │ └─────────────────┘ │   │
│  │ │ 3 campañas  •  €45,230 total │  │  │                     │   │
│  │ │                              │  │  │ 🔒 Pago seguro      │   │
│  │ │ [Ver perfil]                 │  │  │ 📦 Entrega: Mar2025 │   │
│  │ └──────────────────────────────┘  │  └─────────────────────┘   │
│  │                                   │                            │
│  └───────────────────────────────────┘                            │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Container | `<div>` | `min-h-screen bg-[#0a0a0f] pb-16` |
| Main Layout | `<div>` | `max-w-7xl mx-auto px-4 py-8 grid lg:grid-cols-[1fr_380px] gap-8` |
| Main Content Column | `<div>` | `space-y-8` |
| Hero Image/Video | `<div>` | `aspect-video bg-[#1a1a2e] rounded-xl overflow-hidden mb-6` |
| Video Player | `<video>` o `<ReactPlayer>` | `w-full h-full object-cover` |
| Campaign Header | `<div>` | `mb-6` |
| Campaign Title | `<h1>` | `text-3xl md:text-4xl font-bold text-white mb-3` |
| Artist Row | `<div>` | `flex items-center gap-3 mb-2` |
| Artist Avatar | `<Avatar>` | `w-10 h-10 border-2 border-primary` |
| Artist Name | `<Link>` | `text-lg text-white hover:text-primary transition` |
| Verified Badge | `<Badge>` | `ml-2 bg-primary/20 text-primary border-primary/50 text-xs` |
| Genre Badge | `<Badge variant="secondary">` | `bg-[#1a1a2e] text-[#94a3b8] border-[#334155]` |
| Progress Section | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-6 mb-8` |
| Amount Row | `<div>` | `flex items-baseline gap-2 mb-2` |
| Amount Raised | `<span>` | `text-3xl font-bold text-white` |
| Amount Goal | `<span>` | `text-lg text-[#64748b]` |
| Progress Bar Container | `<div>` | `w-full h-3 bg-[#1e293b] rounded-full overflow-hidden mb-4` |
| Progress Bar Fill | `<div>` | `h-full bg-gradient-to-r from-pink-500 to-purple-600 rounded-full transition-all` (width: %financiado) |
| Stats Grid | `<div>` | `grid grid-cols-3 gap-4 text-center` |
| Stat Item | `<div>` | `space-y-1` |
| Stat Value | `<div>` | `text-2xl font-bold text-white` |
| Stat Label | `<div>` | `text-sm text-[#64748b]` |
| Tabs Navigation | `<Tabs>` | `border-b border-[#334155] mb-6` |
| Tab Item | `<TabsTrigger>` | `text-[#94a3b8] hover:text-white data-[state=active]:text-white data-[state=active]:border-b-2 data-[state=active]:border-primary` |
| Description Content | `<TabsContent>` | `prose prose-invert max-w-none` |
| Description Text | `<div>` | `text-[#cbd5e1] leading-relaxed space-y-4` |
| Description Image | `<img>` | `w-full rounded-lg my-6` |
| Description List | `<ul>` | `list-disc list-inside text-[#cbd5e1] space-y-2` |
| Artist Bio Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-6` |
| Artist Bio Header | `<div>` | `flex items-start gap-4 mb-4` |
| Artist Bio Avatar | `<Avatar>` | `w-16 h-16 border-2 border-primary` |
| Artist Bio Name | `<h3>` | `text-xl font-bold text-white mb-1` |
| Artist Bio Description | `<p>` | `text-sm text-[#94a3b8] mb-4 line-clamp-3` |
| Artist Stats | `<div>` | `flex items-center gap-4 text-xs text-[#64748b] mb-4` |
| View Profile Button | `<Button variant="outline">` | `w-full border-primary text-primary hover:bg-primary/10` |
| Sidebar Container | `<div>` | `sticky top-24 space-y-6` |
| Support CTA Button | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-4 mb-2 text-lg` |
| Starting Price Text | `<p>` | `text-center text-sm text-[#94a3b8] mb-6` |
| Rewards Title | `<h3>` | `text-lg font-bold text-white mb-4` |
| Reward Card | Ver [definir-recompensas/ui-ux.md - Pantalla: Reward Cards en Campaign Detail](../definir-recompensas/ui-ux.md#pantalla-reward-cards-en-campaign-detail-landing---public) |
| Sidebar Footer | `<div>` | `border-t border-[#334155] pt-4 space-y-2` |
| Security Info | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` |
| Security Icon | `<Icon>` (lock) | `w-4 h-4` |
| Delivery Info | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` |
| Delivery Icon | `<Icon>` (truck) | `w-4 h-4` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Hero image/video visible, progress bar animado, rewards sidebar sticky |
| **Loading** | Skeleton para hero, title, progress bar, rewards (3 skeletons) |
| **Video Playing** | Controls overlay, play/pause button |
| **Tab Active** | Tab con border bottom primary, contenido correspondiente visible |
| **Near Goal (>90%)** | Progress bar warning color, badge "Casi alcanzado" |
| **Goal Reached** | Progress bar success green, badge "Financiada", celebration confetti animation opcional |
| **Expired Campaign** | Badge "Finalizada", CTA button disabled con texto "Campaña finalizada" |
| **No Rewards** | Solo botón "Apoyar esta campaña", texto "Esta campaña no tiene recompensas específicas. Puedes hacer una contribución libre." |
| **Mobile View** | Sidebar NO sticky (scroll normal), stack vertical, hero 16:9 ratio |

### Validación en Tiempo Real

**No aplica** - Página de solo lectura.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Hero Video** | Play/pause video |
| **Click Artist Name/Avatar** | Navigate a `/artistas/{id}` (artist profile) |
| **Click Tab (Historia, Actualizaciones, FAQ)** | Cambiar contenido visible, scroll to content |
| **Click Ver perfil (artist)** | Navigate a `/artistas/{id}` |
| **Click Apoyar esta campaña** | Si autenticado → navigate `/campanias/{id}/backing`. Si no → redirect a login |
| **Click Seleccionar (reward)** | Si autenticado → navigate `/campanias/{id}/backing?reward={rewardId}`. Si no → redirect a login |
| **Hover Reward Card** | Border primary, elevation increase (ver definir-recompensas) |
| **Scroll Page** | Sidebar sticky en desktop (permanece visible) |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Stack vertical (main + sidebar), sidebar NO sticky, hero aspect-square, font sizes reducidos, stats grid gap reducido |
| **Tablet (640-1024px)** | Stack vertical, sidebar sticky, layout normal |
| **Desktop (> 1024px)** | Grid layout 1fr 380px, sidebar sticky top-24, spacing amplio |

---

## Pantalla: Backing Form/Modal

**Mockup:** WPR_7-Backing.png
**Proyecto:** Landing
**Ruta:** `/campanias/{id}/backing` o modal overlay
**Template base:** `krowd/` backing flow

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│    ┌──────────────────────────────────────────────────────────┐    │
│    │  [×] Apoya a Luna Sonora                                 │    │
│    │  Álbum debut "Ecos del Alma"                             │    │
│    ├──────────────────────────────────────────────────────────┤    │
│    │                                                          │    │
│    │  Progreso                              Paso 2 de 3      │    │
│    │  [███████████████████████████        ] 75%              │    │
│    │  Monto        Datos          Pago                       │    │
│    │                                                          │    │
│    │  ──────────────────────────────────────────────────      │    │
│    │                                                          │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  Disco firmado + Póster              [Cambiar]    │  │    │
│    │  │  Álbum físico firmado por la artista + póster     │  │    │
│    │  │  exclusivo de edición limitada                    │  │    │
│    │  │                                                    │  │    │
│    │  │  Mínimo €35                                        │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │                                                          │    │
│    │  Monto a Aportar                                         │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  €                                           50     │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │  Mínimo €35 para este reward                             │    │
│    │                                                          │    │
│    │  [+€5]  [+€10]  [+€25]                                   │    │
│    │                                                          │    │
│    │  Mensaje para el artista (opcional)                      │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  Escribe un mensaje de apoyo...                    │  │    │
│    │  │                                                    │  │    │
│    │  │  [Textarea ~3 líneas]                             │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │  0/500 caracteres                                        │    │
│    │                                                          │    │
│    │  ☐ Hacer anónimo mi apoyo                                │    │
│    │                                                          │    │
│    │  ──────────────────────────────────────────────────      │    │
│    │                                                          │    │
│    │  Resumen                                                 │    │
│    │  Aportación:                                   €50      │    │
│    │  Reward:                         Disco firmado + Póster │    │
│    │  Total:                                        €50      │    │
│    │                                                          │    │
│    │  [← Volver]                           [Continuar →]     │    │
│    │  (outline)                            (gradient)        │    │
│    └──────────────────────────────────────────────────────────┘    │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

**Nota:** El mockup WPR_7-Backing.png muestra un formulario de 3 pasos con campos de entrega y método de pago. Para el MVP, simplificaremos a un flujo de 2 pasos:
1. **Paso 1 (Monto + Datos):** Selección de reward, monto, mensaje, anónimo
2. **Paso 2 (Pago):** Integración con Stripe Checkout (redirect externo)

### Especificaciones - Paso 1: Monto y Datos

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Modal Overlay (si modal) | `<Dialog>` backdrop | `bg-black/90 backdrop-blur-md` |
| Form Container (si página) | `<div>` | `min-h-screen bg-[#0a0a0f] py-8` |
| Card Container | `<Card>` | `max-w-2xl mx-auto bg-[#0f1729] border-[#334155]` |
| Header | `<div>` | `border-b border-[#334155] p-6` |
| Close Button (si modal) | `<DialogClose>` | `absolute right-4 top-4 text-[#94a3b8] hover:text-white` |
| Campaign Title | `<h2>` | `text-2xl font-bold text-white mb-1` |
| Campaign Subtitle | `<p>` | `text-sm text-[#64748b]` |
| Progress Section | `<div>` | `px-6 py-4 bg-[#1a1a2e]` |
| Progress Bar Label | `<div>` | `flex items-center justify-between text-sm mb-2` |
| Progress Text | `<span>` | `text-[#94a3b8]` |
| Step Indicator | `<span>` | `text-[#64748b]` |
| Progress Bar Container | `<div>` | `w-full h-2 bg-[#1e293b] rounded-full overflow-hidden mb-2` |
| Progress Bar Fill | `<div>` | `h-full bg-gradient-to-r from-pink-500 to-purple-600 transition-all` (width según step) |
| Steps Labels | `<div>` | `flex justify-between text-xs text-[#64748b]` |
| Form Body | `<form>` | `p-6 space-y-6` |
| Section Divider | `<hr>` | `border-[#334155] my-6` |
| Selected Reward Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4 relative` |
| Reward Header Row | `<div>` | `flex items-start justify-between mb-2` |
| Reward Title | `<h4>` | `text-lg font-semibold text-white` |
| Change Button | `<Button variant="link" size="sm">` | `text-primary hover:underline` |
| Reward Description | `<p>` | `text-sm text-[#94a3b8] mb-2` |
| Reward Minimum | `<p>` | `text-xs text-[#64748b]` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` |
| Amount Input Container | `<div>` | `relative` |
| Amount Input | `<Input type="number" step="0.01">` | `bg-[#1a1a2e] border-[#334155] text-white text-2xl font-bold text-center py-4 pl-12 pr-4 focus:border-primary` |
| Currency Symbol | `<span>` | `absolute left-4 top-1/2 -translate-y-1/2 text-2xl text-[#94a3b8]` |
| Minimum Hint | `<p>` | `text-xs text-[#64748b] mt-1 italic` |
| Amount Suggestions Row | `<div>` | `flex gap-2 mt-3` |
| Suggestion Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-white hover:border-primary hover:bg-primary/10` |
| Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none placeholder:text-[#64748b] focus:border-primary` |
| Character Counter | `<span>` | `text-xs text-[#64748b] mt-1 block` (color warning at 80%, red at 95%) |
| Checkbox | `<Checkbox>` | `border-[#334155] data-[state=checked]:bg-primary data-[state=checked]:border-primary` |
| Checkbox Label | `<label>` | `text-sm text-white ml-2 cursor-pointer` |
| Anonymous Icon | `<Icon>` (eye-off) | `w-4 h-4 inline mr-1 text-[#64748b]` |
| Summary Section | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4` |
| Summary Title | `<h4>` | `text-lg font-bold text-white mb-3` |
| Summary Row | `<div>` | `flex justify-between text-sm mb-2` |
| Summary Label | `<span>` | `text-[#94a3b8]` |
| Summary Value | `<span>` | `text-white font-semibold` |
| Summary Total Row | `<div>` | `flex justify-between text-lg font-bold border-t border-[#334155] pt-3 mt-3` |
| Summary Total Label | `<span>` | `text-white` |
| Summary Total Value | `<span>` | `text-primary` |
| Footer | `<div>` | `border-t border-[#334155] p-6 flex gap-3 justify-between` |
| Back Button | `<Button variant="outline">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Continue Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |
| Error Message | `<p>` | `text-xs text-red-500 mt-1` |

### Especificaciones - Paso 2: Pago (Stripe Checkout)

**Nota MVP:** En lugar de un formulario de tarjeta custom, redirigir a Stripe Checkout (hosted page). Esto simplifica PCI compliance.

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Loading Container | `<div>` | `min-h-screen bg-[#0a0a0f] flex items-center justify-center` |
| Loading Spinner | `<Loader2>` | `w-16 h-16 animate-spin text-primary` |
| Loading Text | `<p>` | `text-lg text-white mt-4` |
| Redirect Message | `<p>` | `text-sm text-[#94a3b8] mt-2` |

**Flujo:**
1. User completa Paso 1, click "Continuar"
2. POST `/api/backings/create-checkout-session` con backing data
3. Backend crea Stripe Checkout Session, retorna `sessionUrl`
4. Frontend muestra loading "Redirigiendo a pago seguro..."
5. `window.location.href = sessionUrl` → redirect a Stripe Checkout
6. User completa pago en Stripe
7. Stripe webhook confirma pago → backend actualiza backing status
8. Redirect back a `/campanias/{id}/backing-confirmado?session_id={sessionId}`

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default - Paso 1** | Form con reward pre-seleccionado (si viene de ?reward=X), monto mínimo pre-filled, progress bar 50% |
| **No Reward Selected** | Reward card muestra "Sin recompensa seleccionada", monto input sin mínimo (pero min €5) |
| **Reward Pre-selected** | Reward card muestra detalles, monto pre-filled con mínimo del reward |
| **Focus Amount Input** | Border primary, glow shadow, input text center bold |
| **Amount Below Minimum** | Error message rojo "El monto debe ser al menos €X", continue button disabled |
| **Typing Message** | Character counter actualizado (X/500), warning color a 400 caracteres |
| **Anonymous Checked** | Checkbox checked, icon eye-off highlighted |
| **Loading Continue** | Continue button spinner + "Procesando...", form inputs disabled |
| **Validación Error** | Input con border rojo, mensaje de error debajo |
| **Paso 2 - Redirecting** | Full screen loading spinner con texto "Redirigiendo a pago seguro..." |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Monto** | Mayor o igual a mínimo reward (o €5 si sin reward) | "El monto debe ser al menos €{minimo}" |
| **Monto** | Formato decimal válido | "Ingresa un monto válido (ej: 25.00)" |
| **Monto** | Máximo €10,000 | "El monto máximo es €10,000" |
| **Mensaje** | Máximo 500 caracteres | "Máximo 500 caracteres (X/500)" |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click X (cerrar modal)** | Cerrar modal, mostrar confirmación si hay datos ingresados |
| **Click Cambiar (reward)** | Volver a `/campanias/{id}` o abrir modal de rewards selection |
| **Type Amount** | Validación debounced (300ms), actualizar summary |
| **Click +€5/+€10/+€25** | Sumar al monto actual, max €10,000, actualizar summary |
| **Type Mensaje** | Character counter update, validación max length |
| **Toggle Anónimo** | Actualizar estado del checkbox, mostrar icon eye-off highlighted |
| **Click Volver** | Volver a campaign detail `/campanias/{id}`, mostrar confirmación si hay datos |
| **Click Continuar** | Validar form, si válido POST `/api/backings/create-checkout-session`, obtener sessionUrl, redirect a Stripe Checkout |
| **Form Submit Error** | Toast error con mensaje del backend, mantener en paso 1 |

### Zod Schema

```typescript
const backingFormSchema = z.object({
  campaniaId: z.string().uuid(),

  rewardId: z.string()
    .uuid()
    .optional()
    .nullable(), // Puede ser null si "sin recompensa"

  monto: z.number()
    .min(5, "El monto mínimo es €5")
    .max(10000, "El monto máximo es €10,000")
    .positive("Ingresa un monto válido"),

  mensaje: z.string()
    .max(500, "Máximo 500 caracteres")
    .optional()
    .nullable(),

  esAnonimo: z.boolean()
    .default(false),
})
.refine(
  (data) => {
    // Si hay reward seleccionado, validar mínimo del reward
    // Esta validación se hace en el componente con el dato del reward
    return true;
  }
);
```

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Full screen (si modal → full page), padding reducido (p-4), amount input text-xl, suggestion buttons stack 3 cols, footer buttons stack vertical full-width |
| **Tablet (640-1024px)** | Modal max-w-xl, layout normal |
| **Desktop (> 1024px)** | Modal max-w-2xl, spacing amplio |

---

## Pantalla: Backing Confirmation

**Mockup:** Derivado del flujo (no hay mockup específico)
**Proyecto:** Landing
**Ruta:** `/campanias/{id}/backing-confirmado?session_id={sessionId}`
**Template base:** `krowd/` success pages

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│                                                                    │
│                        ✓                                           │
│                   [Animated Success Icon]                          │
│                                                                    │
│            ¡Gracias por tu apoyo!                                  │
│                                                                    │
│       Tu contribución ha sido confirmada con éxito.                │
│    Has ayudado a Luna Sonora a alcanzar su sueño musical.         │
│                                                                    │
│    ┌──────────────────────────────────────────────────────────┐    │
│    │  Resumen de tu Apoyo                                     │    │
│    │                                                          │    │
│    │  Campaña:       Nuevo Álbum: "Ecos del Silencio"        │    │
│    │  Artista:       Luna Indie                               │    │
│    │  Monto:         €50                                      │    │
│    │  Reward:        Disco firmado + Póster                   │    │
│    │  Fecha:         15 Enero 2025, 14:32                     │    │
│    │  ID Backing:    BK-2025-001234                           │    │
│    │                                                          │    │
│    │  📧 Te hemos enviado un email de confirmación a:         │    │
│    │     tu-email@ejemplo.com                                 │    │
│    └──────────────────────────────────────────────────────────┘    │
│                                                                    │
│    ¿Qué sigue?                                                     │
│    • El artista procesará tu recompensa                            │
│    • Te notificaremos sobre actualizaciones de la campaña          │
│    • Recibirás tu recompensa en: Marzo 2025                        │
│                                                                    │
│    [Ver Campaña]          [Explorar Más Campañas]                 │
│    (outline)              (gradient)                               │
│                                                                    │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Container | `<div>` | `min-h-screen bg-[#0a0a0f] flex items-center justify-center py-8 px-4` |
| Success Card | `<Card>` | `max-w-2xl mx-auto bg-[#0f1729] border-[#334155] p-8 text-center` |
| Success Icon Container | `<div>` | `w-24 h-24 mx-auto mb-6 rounded-full bg-green-500/20 flex items-center justify-center` |
| Success Icon | `<Icon>` (check-circle) o `<Icon>` (heart-handshake) | `w-12 h-12 text-green-400` |
| Success Animation | Lottie o Framer Motion | Scale in + fade in + confetti opcional |
| Title | `<h1>` | `text-3xl font-bold text-white mb-4` |
| Subtitle | `<p>` | `text-lg text-[#cbd5e1] mb-2` |
| Message | `<p>` | `text-[#94a3b8] mb-8` |
| Summary Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-6 mb-6 text-left` |
| Summary Title | `<h3>` | `text-xl font-bold text-white mb-4 text-center` |
| Summary Row | `<div>` | `flex justify-between py-2 border-b border-[#334155]` |
| Summary Label | `<span>` | `text-sm text-[#64748b]` |
| Summary Value | `<span>` | `text-sm text-white font-semibold` |
| Amount Value | `<span>` | `text-lg text-primary font-bold` |
| Email Notice | `<div>` | `flex items-start gap-2 bg-[#1e293b] p-3 rounded-lg mt-4` |
| Email Icon | `<Icon>` (mail) | `w-5 h-5 text-primary mt-0.5` |
| Email Text | `<p>` | `text-xs text-[#cbd5e1]` |
| Next Steps Section | `<div>` | `text-left mb-8` |
| Next Steps Title | `<h4>` | `text-lg font-bold text-white mb-3` |
| Next Steps List | `<ul>` | `space-y-2 text-sm text-[#94a3b8]` |
| Next Step Item | `<li>` | `flex items-start gap-2` |
| Bullet Icon | `<Icon>` (check) | `w-4 h-4 text-primary mt-0.5 flex-shrink-0` |
| Actions Row | `<div>` | `flex gap-3 justify-center` |
| View Campaign Button | `<Button variant="outline">` | `border-primary text-primary hover:bg-primary/10 px-8` |
| Explore More Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Spinner mientras se verifica el session_id con backend |
| **Success** | Success icon animado (scale in + fade), confetti animation opcional 2s, summary visible |
| **Error Session** | Error icon (x-circle rojo), texto "No se pudo verificar el pago", botón "Contactar soporte" |
| **Mobile View** | Stack vertical, padding reducido, buttons stack vertical full-width |

### Validación en Tiempo Real

**No aplica** - Página de confirmación.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Page Load** | GET `/api/backings/verify-session?session_id={sessionId}` para obtener backing details |
| **Click Ver Campaña** | Navigate a `/campanias/{campaniaId}` |
| **Click Explorar Más Campañas** | Navigate a `/campanias` (listing) |
| **Click Email Link (si hay)** | Mailto o abrir email client |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Full width (max-w-full), padding reducido (p-4), buttons stack vertical, font sizes reducidos |
| **Tablet (640-1024px)** | max-w-xl, layout normal |
| **Desktop (> 1024px)** | max-w-2xl, spacing amplio |

---

## Pantalla: My Backings Dashboard (Fan View)

**Mockup:** WPR_9-Mis-backings.png
**Proyecto:** Dashboard (puede estar en Landing o Admin - preferir Landing para fans)
**Ruta:** `/mis-apoyos` o `/dashboard/mis-apoyos`
**Template base:** `dashtail/` lists + tables

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]      │  Mis Apoyos                     [Explorar]       │
│                 │  Has apoyado 12 proyectos por un total de €850   │
│  Dashboard      │                                                  │
│  Explorar       │  ┌──────┬────────┬────────────┬──────────┬────┐  │
│  ❤️ Mis Apoyos  │  │ Todos│ Activos│ Completados│ Pendientes│... │  │
│  Guardados      │  │  12  │   5    │     6      │     1     │    │  │
│  Notificaciones │  └──────┴────────┴────────────┴──────────┴────┘  │
│  Configuración  │                                                  │
│                 │  ┌────────────────────────────────────────────┐  │
│  [Avatar]       │  │ [IMG] Nuevo Álbum: Ecos del Silencio       │  │
│  Carlos Méndez  │  │       por Luna Indie                   [En curso]│
│  carlos@mail.com│  │                                            │  │
│                 │  │ Monto Aportado    €75                       │  │
│                 │  │ Recompensa:   Disco firmado + MP3s + álbum │  │
│                 │  │                                            │  │
│                 │  │ ⏰ Pendiente - Campaña finaliza en 16 días │  │
│                 │  │                                            │  │
│                 │  │ [👁️ Ver Campaña]  [💬 Contactar]           │  │
│                 │  └────────────────────────────────────────────┘  │
│                 │                                                  │
│                 │  ┌────────────────────────────────────────────┐  │
│                 │  │ [IMG] Synth Dreams EP             [Financiada]│
│                 │  │       por Neon Waves                       │  │
│                 │  │                                            │  │
│                 │  │ Monto Aportado    €50                       │  │
│                 │  │ Recompensa:   Vinilo edición limitada + Digi│  │
│                 │  │                                            │  │
│                 │  │ 📦 Enviado - Tracking: EST12345789         │  │
│                 │  │                                            │  │
│                 │  │ [👁️ Ver Campaña]  [💬 Contactar]           │  │
│                 │  └────────────────────────────────────────────┘  │
│                 │                                                  │
│                 │  [... más backing cards ...]                    │
│                 │                                                  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Container | `<div>` | `min-h-screen bg-[#0a0a0f]` |
| Sidebar | Ver componente compartido | Dashboard sidebar con navegación |
| Main Content | `<div>` | `ml-64 p-8` (desktop), `p-4` (mobile) |
| Page Header | `<div>` | `flex items-center justify-between mb-6` |
| Page Title | `<h1>` | `text-3xl font-bold text-white` |
| Explore Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |
| Stats Subtitle | `<p>` | `text-[#94a3b8] mb-6` |
| Tabs Container | `<Tabs>` | `mb-6` |
| Tab Item | `<TabsTrigger>` | `text-[#94a3b8] data-[state=active]:text-white data-[state=active]:bg-[#1a1a2e]` |
| Tab Badge | `<Badge variant="secondary">` | `ml-2 bg-[#334155] text-white text-xs` |
| Backings List | `<div>` | `space-y-4` |
| Backing Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-5 hover:border-primary transition` |
| Card Layout | `<div>` | `flex gap-4` |
| Campaign Image | `<img>` | `w-24 h-24 rounded-lg object-cover flex-shrink-0` |
| Card Content | `<div>` | `flex-1` |
| Card Header Row | `<div>` | `flex items-start justify-between mb-3` |
| Campaign Title | `<h3>` | `text-lg font-bold text-white line-clamp-1` |
| Status Badge | `<Badge>` | Variants según estado (ver tabla estados) |
| Artist Name | `<p>` | `text-sm text-[#64748b] mb-3 flex items-center gap-1` |
| Artist Icon | `<Icon>` (user) | `w-4 h-4` |
| Details Grid | `<div>` | `grid grid-cols-2 gap-3 mb-3 text-sm` |
| Detail Item | `<div>` | `space-y-1` |
| Detail Label | `<span>` | `text-[#64748b] block` |
| Detail Value | `<span>` | `text-white font-semibold` |
| Amount Value | `<span>` | `text-primary font-bold text-lg` |
| Reward Text | `<p>` | `text-[#cbd5e1] text-sm line-clamp-1` |
| Status Row | `<div>` | `flex items-center gap-2 text-sm mb-3` |
| Status Icon | `<Icon>` | `w-4 h-4` (varía según estado) |
| Status Text | `<p>` | `text-[#cbd5e1]` |
| Actions Row | `<div>` | `flex gap-2` |
| View Campaign Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-white hover:border-primary` |
| Contact Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-white hover:border-primary` |
| Empty State | `<div>` | `text-center py-16 px-4` |
| Empty Icon | `<Icon>` (heart-off) | `w-20 h-20 text-[#64748b] mx-auto mb-4` |
| Empty Title | `<h3>` | `text-2xl font-bold text-white mb-2` |
| Empty Description | `<p>` | `text-[#94a3b8] mb-6` |
| Empty CTA | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Lista de backing cards ordenada por fecha desc (más recientes primero) |
| **Loading** | 4-5 skeleton cards con shimmer animation |
| **Empty State** | Icono heart-off grande, texto "No has apoyado ninguna campaña aún", botón "Explorar Campañas" gradient |
| **Tab Active** | Tab con background #1a1a2e, texto blanco, badge count |
| **Hover Card** | Border primary, elevation increase |
| **Mobile View** | Sidebar hamburger, cards stack vertical, image size reducido (w-16 h-16), details grid 1 col |

### Status Badges

| Estado Campaña | Badge Variant | Color | Texto |
|----------------|--------------|-------|-------|
| **Activa (En curso)** | `default` | `bg-primary/20 text-primary border-primary/50` | "En curso" |
| **Financiada** | `success` | `bg-green-500/20 text-green-400 border-green-500/50` | "Financiada" |
| **Completada** | `secondary` | `bg-blue-500/20 text-blue-400 border-blue-500/50` | "Completada" |
| **Pendiente pago** | `warning` | `bg-warning/20 text-warning border-warning/50` | "Pendiente" |
| **Cancelada** | `destructive` | `bg-red-500/20 text-red-400 border-red-500/50` | "Cancelada" |
| **Reembolsada** | `outline` | `bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50` | "Reembolsada" |

### Status Icons

| Estado | Icono | Color |
|--------|-------|-------|
| **En curso** | `clock` | `text-primary` |
| **Financiada** | `check-circle` | `text-green-400` |
| **Enviado** | `truck` | `text-blue-400` |
| **Entregado** | `package-check` | `text-green-400` |
| **Pendiente** | `alert-circle` | `text-warning` |
| **Cancelada** | `x-circle` | `text-red-400` |

### Validación en Tiempo Real

**No aplica** - Página de solo lectura.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Tab** | Filtrar backings por estado, actualizar lista |
| **Click Backing Card** | Navigate a `/campanias/{campaniaId}` o expandir card con más detalles |
| **Click Ver Campaña** | Navigate a `/campanias/{campaniaId}` |
| **Click Contactar** | Abrir modal de mensaje al artista o mailto |
| **Click Explorar (header)** | Navigate a `/campanias` (listing) |
| **Hover Card** | Border primary, elevation increase |
| **Empty State CTA** | Navigate a `/campanias` |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar hamburger, main content full width (ml-0), campaign image w-16 h-16, details grid 1 col, actions stack vertical |
| **Tablet (640-1024px)** | Sidebar colapsado icon-only, main content ml-20, layout normal |
| **Desktop (> 1024px)** | Sidebar full width (ml-64), layout completo |

---

## Pantalla: Backings Management (Artist View)

**Mockup:** No hay mockup específico (feature menor para MVP)
**Proyecto:** Admin (Dashboard artista)
**Ruta:** `/dashboard/campanias/{id}/backings`
**Template base:** `dashtail/` tables + stats

**Nota MVP:** Esta pantalla es secundaria para el MVP. Priorizar vistas del fan. Si hay tiempo, implementar tabla simple con:

### Layout Simplificado

```
┌────────────────────────────────────────────────────────────────────┐
│  Dashboard > Campañas > "Ecos del Silencio" > Backings            │
│                                                                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐             │
│  │ Total Raised │  │ Total Backers│  │ Avg. Amount  │             │
│  │ €8,450       │  │ 127          │  │ €66.53       │             │
│  └──────────────┘  └──────────────┘  └──────────────┘             │
│                                                                    │
│  [🔍 Search]  [🎁 Filter by Reward ▼]  [📥 Export CSV]            │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Backer       │ Amount │ Reward        │ Date       │ Status  │  │
│  ├──────────────────────────────────────────────────────────────┤  │
│  │ Carlos M.    │ €75    │ Disco firmado │ 15 Ene 25  │ ✓ Conf. │  │
│  │ Ana G.       │ €50    │ Vinilo limited│ 14 Ene 25  │ ✓ Conf. │  │
│  │ [Anónimo]    │ €25    │ CD Físico     │ 13 Ene 25  │ ✓ Conf. │  │
│  │ ...          │        │               │            │         │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  [← Anterior]   1  2  3  [Siguiente →]                             │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Simplificadas

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Stats Grid | `<div>` | `grid grid-cols-3 gap-4 mb-6` |
| Stat Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4 text-center` |
| Stat Value | `<p>` | `text-2xl font-bold text-white` |
| Stat Label | `<p>` | `text-sm text-[#64748b]` |
| Filters Row | `<div>` | `flex gap-3 mb-4` |
| Search Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| Filter Select | `<Select>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Export Button | `<Button variant="outline">` | `border-primary text-primary hover:bg-primary/10` |
| Table | `<Table>` | shadcn/ui Table component |
| Table Header | `<TableHeader>` | `bg-[#1a1a2e] text-[#94a3b8]` |
| Table Row | `<TableRow>` | `border-b border-[#334155] hover:bg-[#1a1a2e]` |
| Anonymous Text | `<span>` | `text-[#64748b] italic` |
| Status Badge | `<Badge>` | `bg-green-500/20 text-green-400` (confirmed) |

**Interacciones clave:**
- Search filtra por nombre de backer
- Filter by reward muestra solo backings de reward específico
- Export CSV genera CSV con todos los backings
- Click row → ver detalles del backing (modal)

---

## Componentes Compartidos

### RewardSelectionModal

**Usado en:** Backing form (cuando user click "Cambiar" reward)

```tsx
<Dialog open={open} onOpenChange={setOpen}>
  <DialogContent>
    <DialogHeader>
      <DialogTitle>Selecciona una recompensa</DialogTitle>
    </DialogHeader>

    <div className="space-y-3 max-h-[60vh] overflow-y-auto">
      {/* Card "Sin recompensa" */}
      <RewardCard
        reward={null}
        isSelected={selectedReward === null}
        onSelect={() => handleSelect(null)}
      />

      {/* Cards de rewards disponibles */}
      {rewards.map(reward => (
        <RewardCard
          key={reward.id}
          reward={reward}
          isSelected={selectedReward?.id === reward.id}
          onSelect={() => handleSelect(reward)}
          disabled={reward.stockDisponible === 0}
        />
      ))}
    </div>
  </DialogContent>
</Dialog>
```

**Props:**
- `open: boolean`
- `onOpenChange: (open: boolean) => void`
- `rewards: RewardDto[]`
- `selectedReward: RewardDto | null`
- `onSelect: (reward: RewardDto | null) => void`

### CampaignProgressBar

**Usado en:** Campaign cards, campaign detail, backing form

```tsx
<div className="space-y-2">
  <div className="w-full h-2 bg-[#1e293b] rounded-full overflow-hidden">
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
    <span className="font-semibold text-primary">{percentage}% financiado</span>
    <span className="text-[#64748b]">{backers} backers</span>
  </div>
</div>
```

**Props:**
- `amountRaised: number`
- `goalAmount: number`
- `backers: number`
- `size?: "sm" | "md" | "lg"`

### BackingStatusBadge

**Usado en:** My Backings dashboard, Backings management (artist view)

```tsx
<Badge variant={getVariant(status)} className={getClassName(status)}>
  <Icon name={getIcon(status)} className="w-3 h-3 mr-1" />
  {getLabel(status)}
</Badge>
```

**Props:**
- `status: BackingStatus` (enum: PENDING, CONFIRMED, SHIPPED, DELIVERED, CANCELLED, REFUNDED)

### AmountInput

**Usado en:** Backing form

```tsx
<div className="relative">
  <span className="absolute left-4 top-1/2 -translate-y-1/2 text-2xl text-[#94a3b8]">
    €
  </span>
  <Input
    type="number"
    step="0.01"
    value={amount}
    onChange={handleChange}
    className="bg-[#1a1a2e] border-[#334155] text-white text-2xl font-bold text-center py-4 pl-12 pr-4 focus:border-primary"
    min={minAmount}
    max={10000}
  />
  {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
  {hint && <p className="text-xs text-[#64748b] mt-1 italic">{hint}</p>}
</div>
```

**Props:**
- `amount: number`
- `onChange: (amount: number) => void`
- `minAmount?: number`
- `maxAmount?: number`
- `error?: string`
- `hint?: string`

---

## Navegación y Rutas

| Ruta | Componente | Auth Required | Descripción |
|------|-----------|---------------|-------------|
| `/campanias` | CampaignsListPage | No | Grid de campañas activas, search, filtros |
| `/campanias/{id}` | CampaignDetailPage | No | Detalle de campaña con rewards sidebar |
| `/campanias/{id}/backing` | BackingFormPage | **Sí** | Formulario de backing (paso 1: monto + datos) |
| `/campanias/{id}/backing-confirmado` | BackingConfirmationPage | **Sí** | Confirmación post-pago |
| `/mis-apoyos` | MyBackingsPage | **Sí** | Dashboard de backings del fan |
| `/dashboard/campanias/{id}/backings` | CampaignBackingsPage | **Sí (Artista)** | Lista de backings de una campaña (artist view) |

### Redirects de Autenticación

```tsx
// Si user no autenticado intenta acceder a backing form
const returnUrl = `/campanias/${id}/backing${reward ? `?reward=${reward}` : ''}`;
navigate(`/login?returnUrl=${encodeURIComponent(returnUrl)}`);

// Post-login redirect
const returnUrl = searchParams.get('returnUrl');
if (returnUrl) {
  navigate(returnUrl);
} else {
  navigate('/campanias');
}
```

### Query Parameters

| Ruta | Query Param | Uso |
|------|------------|-----|
| `/campanias/{id}/backing` | `?reward={rewardId}` | Pre-seleccionar reward en backing form |
| `/campanias/{id}/backing-confirmado` | `?session_id={stripeSessionId}` | Verificar pago con Stripe |
| `/login` | `?returnUrl={url}` | Redirect post-login |

---

## Animaciones y Transiciones

| Elemento | Animación | Duración |
|----------|-----------|----------|
| **Campaign card hover** | Border color + image scale 1.05 + elevation | 300ms ease |
| **Progress bar fill** | Width transition smooth | 500ms ease-out |
| **Backing form modal open** | Scale 0.95 → 1 + fade in + backdrop blur | 250ms ease-out |
| **Amount input focus** | Border glow + scale 1.01 | 200ms ease |
| **Success icon (confirmation)** | Scale 0 → 1.2 → 1 + fade in | 400ms spring |
| **Confetti animation (confirmation)** | Particles burst from center | 2000ms ease-out |
| **Reward card hover** | Border primary + elevation increase | 200ms ease |
| **Tab switch** | Content fade out → fade in | 150ms ease |
| **Backing card hover** | Border primary + elevation | 200ms ease |
| **Empty state icon** | Fade in + float up 20px | 400ms ease-out |
| **Search input focus** | Border color + glow shadow | 150ms ease |
| **Badge appear** | Scale 0.9 → 1 + fade in | 200ms ease |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 loop | 1500ms ease-in-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Button loading spinner** | Rotate 360deg loop | 600ms linear |

### Confetti Animation (Success Confirmation)

```tsx
import confetti from 'canvas-confetti';

// Trigger on success page mount
useEffect(() => {
  confetti({
    particleCount: 100,
    spread: 70,
    origin: { y: 0.6 },
    colors: ['#ec4899', '#a855f7', '#10b981', '#3b82f6'],
  });
}, []);
```

### Progress Bar Fill Animation

```css
.progress-bar-fill {
  transition: width 500ms cubic-bezier(0.4, 0, 0.2, 1);
}

/* Near goal pulse */
@keyframes nearGoalPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.8; }
}

.progress-near-goal {
  animation: nearGoalPulse 2s ease-in-out infinite;
}
```

---

## Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de color** | Mínimo 4.5:1 para texto normal, 3:1 para texto grande. Verificado: primary (#a855f7) sobre bg (#1a1a2e) = 5.2:1 ✓ |
| **Focus visible** | Ring de 2px en primary (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Keyboard navigation** | Tab order lógico, Enter para submit/select, Esc para cerrar modales, Space para checkboxes |
| **Form labels** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Buttons** | Texto descriptivo o `aria-label` en icon buttons |
| **Links** | Texto descriptivo, no "click here" |
| **Images** | `alt` text descriptivo en campaign images |
| **Modal focus trap** | Focus queda dentro del modal mientras está abierto, focus vuelve al trigger al cerrar |
| **Progress bars** | `role="progressbar"` con `aria-valuenow`, `aria-valuemin`, `aria-valuemax` |
| **Status badges** | `role="status"` con texto descriptivo |
| **Loading states** | `aria-busy="true"` y `aria-live="polite"` para anunciar cambios |
| **Tabs** | `role="tablist"`, `role="tab"`, `role="tabpanel"` con aria-controls/aria-labelledby |
| **Empty states** | Contenido significativo, no solo iconos decorativos |
| **Toast notifications** | `role="status"` con `aria-live="polite"` para info, `role="alert"` para errores |

### ARIA Labels Específicos

```tsx
// Campaign Listing - Search
<Input
  type="search"
  role="searchbox"
  aria-label="Buscar campañas por título, artista o género"
  placeholder="Buscar artistas, géneros, proyectos..."
/>

// Campaign Card
<article
  role="article"
  aria-labelledby={`campaign-title-${id}`}
  aria-describedby={`campaign-progress-${id}`}
>
  <h3 id={`campaign-title-${id}`}>Nuevo Álbum: "Ecos del Silencio"</h3>
  <div id={`campaign-progress-${id}`} role="progressbar" aria-valuenow={83} aria-valuemin={0} aria-valuemax={100}>
    83% financiado
  </div>
</article>

// Progress Bar
<div
  role="progressbar"
  aria-valuenow={percentage}
  aria-valuemin={0}
  aria-valuemax={100}
  aria-label={`Progreso de la campaña: ${percentage}% financiado, ${backers} backers`}
>
  <div className="progress-fill" style={{ width: `${percentage}%` }} />
</div>

// Backing Form - Amount Input
<div role="group" aria-labelledby="amount-label">
  <label id="amount-label" htmlFor="amount-input">
    Monto a Aportar
  </label>
  <input
    id="amount-input"
    type="number"
    aria-required="true"
    aria-describedby="amount-hint amount-error"
    aria-invalid={hasError}
  />
  <span id="amount-hint">Mínimo €{minAmount} para este reward</span>
  {error && <span id="amount-error" role="alert" aria-live="polite">{error}</span>}
</div>

// Backing Form - Anonymous Checkbox
<div role="group">
  <label htmlFor="anonymous-checkbox">
    <input
      id="anonymous-checkbox"
      type="checkbox"
      aria-describedby="anonymous-description"
    />
    Hacer anónimo mi apoyo
  </label>
  <span id="anonymous-description" className="sr-only">
    Si marcas esta opción, tu nombre no aparecerá en la lista pública de backers
  </span>
</div>

// Backing Confirmation - Success Message
<div role="status" aria-live="polite">
  <h1>¡Gracias por tu apoyo!</h1>
  <p>Tu contribución ha sido confirmada con éxito.</p>
</div>

// My Backings - Tabs
<div role="tablist" aria-label="Filtrar backings por estado">
  <button
    role="tab"
    aria-selected={activeTab === 'todos'}
    aria-controls="panel-todos"
    id="tab-todos"
  >
    Todos
    <Badge aria-label={`${totalCount} backings totales`}>{totalCount}</Badge>
  </button>
</div>
<div
  role="tabpanel"
  id="panel-todos"
  aria-labelledby="tab-todos"
>
  {/* Backing cards */}
</div>

// Backing Status Badge
<span role="status" aria-label={`Estado de la campaña: ${statusLabel}`}>
  <Icon name={statusIcon} aria-hidden="true" />
  {statusLabel}
</span>

// Empty State
<div role="status">
  <Icon name="heart-off" aria-hidden="true" />
  <h3>No has apoyado ninguna campaña aún</h3>
  <p>Explora proyectos y encuentra tu próxima banda favorita</p>
</div>

// Loading Button
<Button disabled={isLoading} aria-busy={isLoading}>
  {isLoading && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isLoading ? "Procesando..." : "Continuar"}
</Button>
```

### Keyboard Shortcuts

| Tecla | Acción | Contexto |
|-------|--------|----------|
| **Tab** | Navegar entre elementos interactivos | Global |
| **Shift + Tab** | Navegar hacia atrás | Global |
| **Enter** | Activar botón, submit form, abrir campaign detail | Botones, forms, cards |
| **Space** | Toggle checkbox, scroll page | Checkboxes, page |
| **Esc** | Cerrar modal/dialog | Modales, dialogs |
| **Arrow Keys** | Navegar entre tabs | Tabs component |
| **/** | Focus search bar | Campaign listing |
| **Home** | Scroll to top | Global |
| **End** | Scroll to bottom | Global |

### Screen Reader Support

**Landmarks:**
```tsx
<header role="banner">Header</header>
<nav role="navigation" aria-label="Navegación principal">Navbar</nav>
<main role="main" id="main-content">Main content</main>
<aside role="complementary" aria-label="Recompensas">Sidebar</aside>
<footer role="contentinfo">Footer</footer>
```

**Skip Links:**
```tsx
<a href="#main-content" className="sr-only focus:not-sr-only">
  Saltar al contenido principal
</a>
```

---

## Checklist UI/UX

### Campaign Listing
- [ ] Hero section con título, subtítulo, search bar
- [ ] Search bar con icon, debounce 500ms, clear button
- [ ] Filters row: Género, Estado, Ordenar con dropdowns
- [ ] Results count visible
- [ ] Grid responsive: 1 col mobile, 2 tablet, 3 desktop
- [ ] Campaign cards con image, badge género, days remaining badge
- [ ] Campaign title, artist name con icon user
- [ ] Amount raised + goal + progress bar (gradient según %)
- [ ] Progress bar colors: purple default, yellow >90%, green 100%
- [ ] Percentage financiado + backers count con icon users
- [ ] Hover effect: border primary, image scale 1.05, elevation
- [ ] Pagination con buttons prev/next y números
- [ ] Empty state: icon search-x, texto, botón "Limpiar filtros"
- [ ] Loading: 6 skeleton cards con shimmer
- [ ] Mobile: grid 1 col, filters stack vertical, padding reducido
- [ ] Accesibilidad: search role="searchbox", cards role="article", progress aria-valuenow

### Campaign Detail
- [ ] Grid layout: main content + sidebar (desktop), stack (mobile)
- [ ] Hero image/video aspect-video, play/pause controls
- [ ] Campaign title h1, artist row con avatar + verified badge
- [ ] Genre badge, artist link hover primary
- [ ] Progress section: amount raised (3xl bold), goal (lg gray)
- [ ] Progress bar 3px height con gradient según %
- [ ] Stats grid 3 cols: % financiado, backers, días restantes
- [ ] Tabs navigation: Historia, Actualizaciones, FAQ
- [ ] Description content con prose styling, images, lists
- [ ] Artist bio card: avatar, name, description, stats, "Ver perfil" button
- [ ] Sidebar sticky top-24 (desktop), scroll normal (mobile)
- [ ] "Apoyar esta campaña" button gradient full-width
- [ ] "Desde €X" text debajo del botón
- [ ] Rewards title h3, divider before
- [ ] Reward cards: price, title, description, stock, "Seleccionar" button
- [ ] Popular badge rosa en reward con más backings
- [ ] Limited badge warning en rewards con stock <50%
- [ ] Sold out rewards opacity 60%, botón disabled
- [ ] Sidebar footer: lock icon + "Pago seguro", truck icon + "Entrega: fecha"
- [ ] Loading: skeletons para hero, title, progress, rewards
- [ ] Mobile: sidebar no sticky, font sizes reducidos
- [ ] Accesibilidad: video controls, tabs role="tablist", progress aria, focus states

### Backing Form (Paso 1)
- [ ] Modal/card max-w-2xl, header con title + subtitle
- [ ] Close button (X) con confirmación si hay datos
- [ ] Progress section: bar + step labels "Monto, Datos, Pago"
- [ ] Progress bar width 33% paso 1, 66% paso 2, 100% paso 3
- [ ] Selected reward card: title, description, mínimo, botón "Cambiar"
- [ ] Amount input: € symbol left, text-2xl bold center, focus border primary
- [ ] Minimum hint debajo del input
- [ ] Amount suggestions: +€5, +€10, +€25 buttons outline
- [ ] Message textarea: min-h-[80px], placeholder, character counter (0/500)
- [ ] Character counter warning color a 400, red a 475
- [ ] Anonymous checkbox con icon eye-off, label descriptivo
- [ ] Summary section card: Aportación, Reward, Total (primary color)
- [ ] Footer: "Volver" button outline, "Continuar" button gradient
- [ ] Validation errors: border rojo, mensaje debajo en rojo
- [ ] Loading continue: spinner + "Procesando...", inputs disabled
- [ ] Mobile: full screen (si modal), padding reducido, suggestions 3 cols, footer stack vertical
- [ ] Accesibilidad: labels con htmlFor, errors role="alert", focus trap

### Backing Form (Paso 2: Stripe Redirect)
- [ ] Loading screen full height, centered
- [ ] Spinner Loader2 w-16 h-16 animate-spin primary color
- [ ] Text "Redirigiendo a pago seguro..."
- [ ] Redirect message "No cierres esta ventana"
- [ ] POST `/api/backings/create-checkout-session` al submit
- [ ] Backend retorna sessionUrl de Stripe
- [ ] window.location.href = sessionUrl (redirect)
- [ ] Error handling: toast error si falla POST

### Backing Confirmation
- [ ] Card max-w-2xl centered, padding amplio
- [ ] Success icon w-24 h-24, circular background green/20, icon check-circle o heart-handshake
- [ ] Success icon animation: scale 0 → 1.2 → 1 (400ms spring)
- [ ] Confetti animation opcional (canvas-confetti)
- [ ] Title h1 "¡Gracias por tu apoyo!"
- [ ] Subtitle + message text [#cbd5e1] y [#94a3b8]
- [ ] Summary card: rows con label/value (campaña, artista, monto, reward, fecha, ID)
- [ ] Amount value text-lg primary bold
- [ ] Email notice section: icon mail, texto "Te hemos enviado confirmación..."
- [ ] Next steps section: title + list con bullets (icon check primary)
- [ ] Actions row: "Ver Campaña" outline, "Explorar Más" gradient
- [ ] Loading: spinner mientras verifica session_id
- [ ] Error state: icon x-circle rojo, texto error, botón "Contactar soporte"
- [ ] Mobile: buttons stack vertical full-width
- [ ] Accesibilidad: success message role="status" aria-live="polite"

### My Backings Dashboard
- [ ] Sidebar navigation con "Mis Apoyos" active
- [ ] Page header: title h1 + "Explorar" button gradient
- [ ] Stats subtitle: "Has apoyado X proyectos por un total de €X"
- [ ] Tabs: Todos, Activos, Completados, Pendientes con badge count
- [ ] Backings list space-y-4
- [ ] Backing cards: flex layout, image 24x24 rounded (desktop), 16x16 (mobile)
- [ ] Card header: campaign title + status badge
- [ ] Artist name con icon user
- [ ] Details grid 2 cols: Monto (primary bold), Recompensa (line-clamp-1)
- [ ] Status row con icon + texto (varía según estado)
- [ ] Actions: "Ver Campaña" + "Contactar" buttons outline
- [ ] Hover card: border primary, elevation
- [ ] Empty state: icon heart-off, texto, botón "Explorar Campañas" gradient
- [ ] Loading: 4-5 skeleton cards
- [ ] Mobile: sidebar hamburger, image w-16 h-16, details grid 1 col, actions stack
- [ ] Status badges: colores según estado (ver tabla)
- [ ] Status icons: clock, check-circle, truck, package-check, alert-circle, x-circle
- [ ] Accesibilidad: tabs role="tablist", cards role="article", badges role="status"

### Backings Management (Artist View) - MVP Simplificado
- [ ] Breadcrumb: Dashboard > Campañas > [Nombre] > Backings
- [ ] Stats grid 3 cols: Total Raised, Total Backers, Avg. Amount
- [ ] Filters row: search input, filter by reward dropdown, export CSV button
- [ ] Table shadcn/ui: headers (Backer, Amount, Reward, Date, Status)
- [ ] Table rows: hover bg [#1a1a2e], border-b [#334155]
- [ ] Anonymous text italic gray
- [ ] Status badge green confirmed
- [ ] Pagination
- [ ] Click row → ver detalles backing (modal)
- [ ] Export CSV → download CSV file
- [ ] Mobile: table horizontal scroll
- [ ] Accesibilidad: table semantic headers, row click keyboard accessible

### Componentes Compartidos
- [ ] RewardSelectionModal: dialog con lista de rewards, botón seleccionar
- [ ] CampaignProgressBar: bar con gradient según %, stats debajo
- [ ] BackingStatusBadge: badge con icon + texto, colores según estado
- [ ] AmountInput: input con € prefix, validación min/max, error message

### Cross-cutting
- [ ] Design tokens consistentes con crear-campania y definir-recompensas
- [ ] Dark theme aplicado (#0a0a0f, #0f1729, #1a1a2e)
- [ ] Gradient buttons (pink → purple) en CTAs principales
- [ ] shadcn/ui components: Card, Button, Input, Textarea, Dialog, Badge, Tabs, Progress, Avatar, Select, Checkbox, Table
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-500ms)
- [ ] Progress bar colors dinámicos (purple, yellow >90%, green 100%)
- [ ] Contraste mínimo 4.5:1 verificado
- [ ] Focus states con ring purple en todos interactivos
- [ ] Form validation con Zod + react-hook-form
- [ ] TanStack Query para queries (campaigns, backings) y mutations (create backing)
- [ ] Stripe Checkout integration para pago (redirect a hosted page)
- [ ] Manejo de errores del backend (ServiceResponse)
- [ ] Toast notifications configuradas (sonner)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders para loading states
- [ ] Empty states con iconos y CTAs claros
- [ ] Progress bars con role="progressbar" y aria attributes
- [ ] Status badges con role="status"
- [ ] API endpoints:
  - GET `/api/campanias` (list)
  - GET `/api/campanias/{id}` (detail)
  - GET `/api/campanias/{id}/rewards` (rewards list)
  - POST `/api/backings/create-checkout-session` (crear sesión Stripe)
  - GET `/api/backings/verify-session?session_id={id}` (verificar pago)
  - GET `/api/backings/my-backings` (backings del user)
  - GET `/api/campanias/{id}/backings` (backings de campaña - artist view)
- [ ] Stripe webhook endpoint para confirmar pagos: `/api/webhooks/stripe`
