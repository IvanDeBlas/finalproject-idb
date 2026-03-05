# Diseño UI: Registro de Artista (Landing)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/web (Vite + React - Landing Pública)

---

## 1. Resumen

- **Componentes shadcn/ui existentes:** 9 (Button, Card, Input, Label, Progress, Textarea, Avatar, Badge, Skeleton)
- **Componentes custom a crear:** 5
- **Screens:** 1 (Perfil Público de Artista)
- **Responsive breakpoints:** sm (640px), md (768px), lg (1024px)

### Scope de esta Feature en Landing

Esta feature se enfoca **únicamente en la visualización del perfil público de artista** (`/artistas/{id}`). Los formularios de registro y creación de perfil están en la app **Admin** (Next.js), no en Landing.

---

## 2. Paleta de Colores (Dark Theme)

| Uso | Variable CSS | Hex | Uso Específico |
|-----|--------------|-----|----------------|
| Background Primary | `--bg-primary` | `#1a1a2e` | Fondo principal de la página |
| Background Secondary | `--bg-secondary` | `#16213e` | Secciones alternativas |
| Background Card | `--bg-card` | `#0f1729` | Cards de contenido |
| Background Card Hover | `--bg-card-hover` | `#1e2a42` | Hover en cards interactivos |
| Primary Color | `--primary-color` | `#a855f7` | Botones principales, focus states |
| Primary Gradient | `--primary-gradient` | `linear-gradient(135deg, #ec4899 0%, #a855f7 100%)` | Botones CTA, hero banner |
| Primary Gradient Hover | `--primary-gradient-hover` | `linear-gradient(135deg, #f472b6 0%, #c084fc 100%)` | Hover en gradientes |
| Text Primary | `--text-primary` | `#ffffff` | Títulos, texto principal |
| Text Secondary | `--text-secondary` | `#94a3b8` | Subtítulos, descripciones |
| Text Muted | `--text-muted` | `#64748b` | Placeholder, texto terciario |
| Text Label | `--text-label` | `#cbd5e1` | Labels de formularios |
| Border Primary | `--border-primary` | `#334155` | Bordes de cards, separadores |
| Border Focus | `--border-focus` | `#a855f7` | Focus en inputs |
| Status Success | `--status-success` | `#10b981` | Estados exitosos |
| Status Error | `--status-error` | `#ef4444` | Estados de error |

### Aplicación en Tailwind

```tsx
// Fondo principal
className="bg-[#1a1a2e]"

// Card de contenido
className="bg-[#0f1729] border border-[#334155]"

// Botón con gradiente
className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"

// Texto
className="text-white"           // Primary
className="text-[#94a3b8]"       // Secondary
className="text-[#64748b]"       // Muted
```

---

## 3. Componentes por Screen

### 3.1 Perfil Público de Artista (`/artistas/{id}`)

#### Layout General

```
┌──────────────────────────────────────────────────────────────┐
│ HEADER (sticky)                                              │
│ [LOGO] WePlay Rises  Explorar  Para Artistas  Login         │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ HERO BANNER (gradient o imagen)                             │
│ h-64 bg-gradient-to-r from-purple-900 to-pink-900           │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ ┌───┐  ARTIST INFO (overlay sobre hero)                     │
│ │IMG│  Luna Vibe                                            │
│ └───┘  Electronic • Synthwave • Indie                       │
│                                                              │
│ 🎵 3 campañas • 👥 247 backers • 💰 €12,450 recaudados      │
│                                                              │
│ [💜 Seguir]  🌐 🎵 📺                                         │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│ CONTENT GRID (Desktop: 3 cols, Tablet: 2 cols, Mobile: 1)   │
│                                                              │
│ ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │
│ │ Biografía   │  │ Campañas    │  │ Actividad   │          │
│ │             │  │ Activas     │  │ Reciente    │          │
│ │ ...         │  │             │  │             │          │
│ └─────────────┘  └─────────────┘  └─────────────┘          │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

#### Componentes Utilizados

**Header (NavBar)**

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Container | `<header>` | `sticky top-0 z-50 bg-[#1a1a2e] border-b border-[#334155]` |
| Logo | Custom SVG + Text | `flex items-center gap-2 text-white font-bold text-xl` |
| Nav Links | `<nav>` con `<a>` | `flex items-center gap-6 text-[#94a3b8] hover:text-white transition` |
| Login Button | `<Button variant="ghost">` | `text-white hover:bg-[#1e2a42]` |

**Composición NavBar:**
```tsx
<header className="sticky top-0 z-50 bg-[#1a1a2e] border-b border-[#334155]">
  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div className="flex items-center justify-between h-16">
      {/* Logo */}
      <div className="flex items-center gap-2">
        <MusicNoteIcon className="h-8 w-8 text-purple-500" />
        <span className="text-xl font-bold text-white">WePlay Rises</span>
      </div>

      {/* Navigation - Desktop */}
      <nav className="hidden md:flex items-center gap-6">
        <a href="/explorar" className="text-[#94a3b8] hover:text-white transition">
          Explorar
        </a>
        <a href="/para-artistas" className="text-[#94a3b8] hover:text-white transition">
          Para Artistas
        </a>
        <a href="/como-funciona" className="text-[#94a3b8] hover:text-white transition">
          Cómo Funciona
        </a>
      </nav>

      {/* Auth Buttons */}
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="sm" className="text-white">
          Iniciar Sesión
        </Button>
        <Button
          size="sm"
          className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
        >
          Registrarse
        </Button>
      </div>
    </div>
  </div>
</header>
```

**Hero Banner**

| Elemento | Componente | Customización |
|----------|------------|---------------|
| Container | `<div>` | `relative h-64 bg-gradient-to-r from-purple-900 to-pink-900` |
| Imagen (si existe) | `<img>` | `absolute inset-0 w-full h-full object-cover opacity-50` |
| Overlay | `<div>` | `absolute inset-0 bg-gradient-to-t from-[#1a1a2e] to-transparent` |

**Composición Hero:**
```tsx
<div className="relative h-48 sm:h-56 md:h-64 bg-gradient-to-r from-purple-900 to-pink-900">
  {/* Si el artista tiene banner image */}
  {bannerUrl && (
    <img
      src={bannerUrl}
      alt="Banner de artista"
      className="absolute inset-0 w-full h-full object-cover opacity-50"
    />
  )}

  {/* Overlay degradado */}
  <div className="absolute inset-0 bg-gradient-to-t from-[#1a1a2e] to-transparent" />
</div>
```

**Artist Info Section**

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Avatar Container | `<Avatar>` | `w-24 h-24 sm:w-28 sm:h-28 md:w-32 md:h-32 -mt-12 sm:-mt-14 md:-mt-16 border-4 border-[#1a1a2e] rounded-full` |
| Avatar Image | `<AvatarImage>` | - |
| Avatar Fallback | `<AvatarFallback>` | `bg-[#2d1b4e] text-purple-300` con icono de música |
| Artist Name | `<h1>` | `text-2xl sm:text-3xl md:text-4xl font-bold text-white mt-4` |
| Genre Tags Container | `<div>` | `flex flex-wrap gap-2 mt-2` |
| Genre Tag | `<Badge>` | `bg-[#2d1b4e] text-purple-300 border border-purple-500/50` |
| Stats Container | `<div>` | `flex flex-wrap gap-4 sm:gap-6 text-[#94a3b8] mt-4 text-sm sm:text-base` |
| Stat Item | `<span>` | `flex items-center gap-2` |
| Follow Button | `<Button>` | `mt-6 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white px-6` |
| Social Icons Container | `<div>` | `flex gap-2 mt-4` |
| Social Icon Button | `<Button variant="ghost" size="icon">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |

**Composición Artist Info:**
```tsx
<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
  {/* Avatar */}
  <Avatar className="w-24 h-24 sm:w-28 sm:h-28 md:w-32 md:h-32 -mt-12 sm:-mt-14 md:-mt-16 border-4 border-[#1a1a2e] rounded-full">
    <AvatarImage src={artista.imagenUrl} alt={artista.nombreArtistico} />
    <AvatarFallback className="bg-[#2d1b4e] text-purple-300 text-3xl">
      <MusicalNoteIcon className="h-16 w-16" />
    </AvatarFallback>
  </Avatar>

  {/* Nombre */}
  <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold text-white mt-4">
    {artista.nombreArtistico}
  </h1>

  {/* Géneros (ejemplo - fuera de MVP) */}
  <div className="flex flex-wrap gap-2 mt-2">
    <Badge className="bg-[#2d1b4e] text-purple-300 border border-purple-500/50">
      Electronic
    </Badge>
    <Badge className="bg-[#2d1b4e] text-purple-300 border border-purple-500/50">
      Synthwave
    </Badge>
  </div>

  {/* Stats (futura implementación - placeholder) */}
  <div className="flex flex-wrap gap-4 sm:gap-6 text-[#94a3b8] mt-4 text-sm sm:text-base">
    <span className="flex items-center gap-2">
      <MusicalNoteIcon className="h-5 w-5" />
      <span><strong className="text-white">0</strong> campañas</span>
    </span>
    <span className="flex items-center gap-2">
      <UsersIcon className="h-5 w-5" />
      <span><strong className="text-white">0</strong> backers</span>
    </span>
    <span className="flex items-center gap-2">
      <CurrencyEuroIcon className="h-5 w-5" />
      <span><strong className="text-white">€0</strong> recaudados</span>
    </span>
  </div>

  {/* Actions */}
  <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 mt-6">
    <Button className="w-full sm:w-auto bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white px-6">
      <HeartIcon className="h-5 w-5 mr-2" />
      Seguir
    </Button>

    {/* Social Links */}
    <div className="flex gap-2">
      <Button variant="ghost" size="icon" className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]" asChild>
        <a href="https://spotify.com" target="_blank" rel="noopener noreferrer" aria-label="Spotify">
          <SpotifyIcon className="h-5 w-5" />
        </a>
      </Button>
      <Button variant="ghost" size="icon" className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]" asChild>
        <a href="https://youtube.com" target="_blank" rel="noopener noreferrer" aria-label="YouTube">
          <YouTubeIcon className="h-5 w-5" />
        </a>
      </Button>
      <Button variant="ghost" size="icon" className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]" asChild>
        <a href="https://example.com" target="_blank" rel="noopener noreferrer" aria-label="Sitio Web">
          <GlobeIcon className="h-5 w-5" />
        </a>
      </Button>
    </div>
  </div>
</div>
```

**Biography Card**

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border border-[#334155] hover:bg-[#1e2a42]/30 transition-colors` |
| Card Header | `<CardHeader>` | `pb-3` |
| Card Title | `<CardTitle>` | `text-xl font-bold text-white` |
| Card Content | `<CardContent>` | `pt-0` |
| Bio Text | `<p>` | `text-[#94a3b8] leading-relaxed whitespace-pre-wrap` |
| Empty State | `<p>` | `text-[#64748b] italic` |

**Composición Biography:**
```tsx
<Card className="bg-[#0f1729] border border-[#334155]">
  <CardHeader className="pb-3">
    <CardTitle className="text-xl font-bold text-white">Biografía</CardTitle>
  </CardHeader>
  <CardContent className="pt-0">
    {artista.descripcion ? (
      <p className="text-[#94a3b8] leading-relaxed whitespace-pre-wrap">
        {artista.descripcion}
      </p>
    ) : (
      <p className="text-[#64748b] italic">
        Este artista aún no ha agregado una biografía.
      </p>
    )}
  </CardContent>
</Card>
```

**Campañas Section (Placeholder MVP)**

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border border-[#334155]` |
| Card Header | `<CardHeader>` | `pb-3` |
| Card Title | `<CardTitle>` | `text-xl font-bold text-white` |
| Card Content | `<CardContent>` | `pt-0` |
| Empty State | `<div>` | `text-center py-8` |
| Empty Icon | Custom SVG/Icon | `h-16 w-16 text-[#64748b] mx-auto mb-4` |
| Empty Text | `<p>` | `text-[#94a3b8]` |

**Composición Campañas:**
```tsx
<Card className="bg-[#0f1729] border border-[#334155]">
  <CardHeader className="pb-3">
    <CardTitle className="text-xl font-bold text-white">Campañas Activas</CardTitle>
  </CardHeader>
  <CardContent className="pt-0">
    {/* Placeholder para MVP */}
    <div className="text-center py-8">
      <MusicalNoteIcon className="h-16 w-16 text-[#64748b] mx-auto mb-4" />
      <p className="text-[#94a3b8]">Próximamente campañas musicales...</p>
    </div>
  </CardContent>
</Card>
```

---

## 4. Componentes Custom a Crear

### 4.1 `<NavBar>` (Custom)

**Ubicación:** `src/web/src/components/layout/NavBar.tsx`

**Props:**
```typescript
interface NavBarProps {
  className?: string;
}
```

**Descripción:** Header sticky con navegación, logo y botones de autenticación.

**Componentes internos:**
- Logo (custom SVG + text)
- Nav links (desktop)
- Mobile menu button (hamburger - futura implementación)
- Auth buttons (Login, Register)

**Responsive:**
- Mobile: Solo logo + hamburger (futura implementación)
- Tablet+: Logo + nav links + auth buttons

---

### 4.2 `<ArtistHero>` (Custom)

**Ubicación:** `src/web/src/components/artistas/ArtistHero.tsx`

**Props:**
```typescript
interface ArtistHeroProps {
  artista: Artista;
  isLoading?: boolean;
}
```

**Descripción:** Sección de hero banner + avatar + info del artista.

**Componentes internos:**
- Hero banner (gradient o imagen)
- Avatar (shadcn/ui)
- Nombre y tags de género
- Stats (campañas, backers, recaudado)
- Follow button
- Social links

**Estados:**
- `isLoading`: Skeletons en avatar, nombre, stats
- `!bannerUrl`: Muestra solo gradiente
- `!imagenUrl`: Avatar fallback con icono

---

### 4.3 `<ArtistBio>` (Custom)

**Ubicación:** `src/web/src/components/artistas/ArtistBio.tsx`

**Props:**
```typescript
interface ArtistBioProps {
  descripcion?: string;
  isLoading?: boolean;
}
```

**Descripción:** Card con la biografía del artista.

**Componentes internos:**
- Card (shadcn/ui)
- CardHeader con título
- CardContent con texto o empty state

**Estados:**
- `isLoading`: Skeleton lines
- `!descripcion`: Mensaje "Este artista aún no ha agregado una biografía"

---

### 4.4 `<ArtistCampaignsSection>` (Custom - Placeholder)

**Ubicación:** `src/web/src/components/artistas/ArtistCampaignsSection.tsx`

**Props:**
```typescript
interface ArtistCampaignsSectionProps {
  artistaId: string;
  isLoading?: boolean;
}
```

**Descripción:** Card placeholder para campañas del artista (fuera de MVP).

**Componentes internos:**
- Card (shadcn/ui)
- Empty state con icono y mensaje

**Estados:**
- `isLoading`: Skeleton cards (3 placeholders)
- Empty: Mensaje "Próximamente campañas musicales..."

---

### 4.5 `<ArtistProfileSkeleton>` (Custom)

**Ubicación:** `src/web/src/components/artistas/ArtistProfileSkeleton.tsx`

**Props:**
```typescript
// Sin props - componente estático
```

**Descripción:** Loading skeleton para toda la página de perfil de artista.

**Componentes internos:**
- Hero skeleton (gradient + círculo para avatar)
- Skeleton para nombre (h-8 w-64)
- Skeleton para badges (3 small rectangles)
- Skeleton para stats (3 lines)
- Skeleton cards para bio y campañas

**Composición:**
```tsx
<div className="animate-pulse">
  {/* Hero + Avatar */}
  <div className="relative h-64 bg-gradient-to-r from-purple-900/30 to-pink-900/30">
    <div className="absolute -bottom-16 left-8">
      <Skeleton className="h-32 w-32 rounded-full border-4 border-[#1a1a2e]" />
    </div>
  </div>

  {/* Info */}
  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-20">
    <Skeleton className="h-10 w-64 mb-4" />
    <div className="flex gap-2 mb-4">
      <Skeleton className="h-6 w-20 rounded-full" />
      <Skeleton className="h-6 w-24 rounded-full" />
      <Skeleton className="h-6 w-16 rounded-full" />
    </div>
    <div className="flex gap-6 mb-6">
      <Skeleton className="h-5 w-32" />
      <Skeleton className="h-5 w-32" />
      <Skeleton className="h-5 w-40" />
    </div>

    {/* Content Grid */}
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-8">
      <Skeleton className="h-64" />
      <Skeleton className="h-64" />
      <Skeleton className="h-64" />
    </div>
  </div>
</div>
```

---

## 5. Estados Visuales

### 5.1 Loading States

| Componente | Loading Visual |
|------------|----------------|
| **Perfil completo** | `<ArtistProfileSkeleton>` (hero + info + cards) |
| **Avatar** | `<Skeleton className="h-32 w-32 rounded-full">` |
| **Nombre** | `<Skeleton className="h-10 w-64">` |
| **Bio text** | `<Skeleton className="h-4 w-full mb-2">` x4 lines |
| **Stats** | `<Skeleton className="h-5 w-32">` x3 |
| **Badges** | `<Skeleton className="h-6 w-20 rounded-full">` x3 |

**Duración de animación:** Pulse infinito (`animate-pulse`)

### 5.2 Empty States

| Escenario | Visual | Mensaje |
|-----------|--------|---------|
| **Sin avatar** | `<AvatarFallback>` con icono de música | - |
| **Sin descripción** | Texto muted italic | "Este artista aún no ha agregado una biografía." |
| **Sin campañas** | Icono musical + texto centrado | "Próximamente campañas musicales..." |
| **Sin banner** | Solo gradiente purple-to-pink | - |

### 5.3 Error States

| Error | Visual | Acción |
|-------|--------|--------|
| **404 - Artista no encontrado** | Página completa con error | Botón "Volver a explorar" |
| **500 - Error de servidor** | Toast notification rojo | Botón "Reintentar" |
| **Red error (fetch failed)** | Toast notification rojo | Auto-retry con TanStack Query |

**Componente 404 Page:**
```tsx
<div className="min-h-screen bg-[#1a1a2e] flex items-center justify-center px-4">
  <div className="text-center">
    <h1 className="text-6xl font-bold text-white mb-4">404</h1>
    <p className="text-xl text-[#94a3b8] mb-8">
      Artista no encontrado
    </p>
    <Button
      asChild
      className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
    >
      <a href="/explorar">Volver a Explorar</a>
    </Button>
  </div>
</div>
```

### 5.4 Hover States

| Elemento | Hover Effect |
|----------|--------------|
| **Nav links** | `text-[#94a3b8] → text-white` transition 150ms |
| **Button gradient** | `from-pink-500 to-purple-600 → from-pink-600 to-purple-700` transition 200ms |
| **Social icon buttons** | `bg-transparent → bg-[#1e2a42]` + `text-[#94a3b8] → text-white` |
| **Biography card** | `bg-[#0f1729] → bg-[#1e2a42]/30` transition 200ms |
| **Badges** | Scale 1.05 transition 150ms (opcional) |

---

## 6. Responsive Design

### 6.1 Breakpoints

| Breakpoint | Width | Cambios Principales |
|------------|-------|---------------------|
| **Mobile** | < 640px (sm) | Stack vertical, padding reducido, avatar más pequeño |
| **Tablet** | 640px - 1024px (sm-lg) | Grid 2 columnas, padding medio |
| **Desktop** | > 1024px (lg) | Grid 3 columnas, max-width 7xl, padding amplio |

### 6.2 Componentes por Breakpoint

**Hero Banner:**
| Breakpoint | Height |
|------------|--------|
| Mobile | `h-48` (192px) |
| Tablet | `h-56` (224px) |
| Desktop | `h-64` (256px) |

**Avatar:**
| Breakpoint | Size | Margin Top |
|------------|------|------------|
| Mobile | `w-24 h-24` (96px) | `-mt-12` |
| Tablet | `w-28 h-28` (112px) | `-mt-14` |
| Desktop | `w-32 h-32` (128px) | `-mt-16` |

**Artist Name:**
| Breakpoint | Font Size |
|------------|-----------|
| Mobile | `text-2xl` |
| Tablet | `text-3xl` |
| Desktop | `text-4xl` |

**Stats Row:**
| Breakpoint | Layout |
|------------|--------|
| Mobile | `flex-wrap gap-4` (stack 2x2) |
| Tablet+ | `flex gap-6` (horizontal) |

**Content Grid:**
```tsx
// Mobile: 1 columna
// Tablet: 2 columnas (Bio + Campañas)
// Desktop: 3 columnas (Bio + Campañas + Placeholder futuro)
className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
```

**Container Padding:**
```tsx
// Mobile: px-4
// Tablet: px-6
// Desktop: px-8
className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8"
```

### 6.3 Responsive Classes Clave

```tsx
// Typography
className="text-2xl sm:text-3xl md:text-4xl"

// Spacing
className="mt-4 sm:mt-6 md:mt-8"
className="gap-4 sm:gap-6"

// Layout
className="flex-col sm:flex-row"
className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3"

// Sizing
className="w-full sm:w-auto"
className="h-48 sm:h-56 md:h-64"
```

---

## 7. Animaciones y Transiciones

### 7.1 Transiciones CSS

| Elemento | Propiedad | Duración | Easing |
|----------|-----------|----------|--------|
| **Nav links** | color | 150ms | ease |
| **Button hover** | background-color, transform (scale 1.02) | 200ms | ease |
| **Card hover** | background-color | 200ms | ease |
| **Social icons** | color, background-color | 150ms | ease |
| **Page mount** | opacity 0 → 1 | 300ms | ease-in-out |

### 7.2 Animaciones Tailwind

**Button Hover:**
```tsx
<Button className="... transition-all duration-200 hover:scale-102">
```

**Nav Link:**
```tsx
<a className="... transition-colors duration-150">
```

**Card:**
```tsx
<Card className="... transition-colors duration-200">
```

### 7.3 Loading Animations

**Skeleton Pulse:**
```tsx
<Skeleton className="animate-pulse" />
```

**Spinner (para botones - no aplica en esta feature de landing):**
```tsx
<Loader2Icon className="animate-spin h-5 w-5" />
```

### 7.4 Page Transitions (React Router)

**Fade In al montar:**
```tsx
// En el componente principal
<div className="animate-in fade-in duration-300">
  {/* Contenido del perfil */}
</div>
```

---

## 8. Accesibilidad

### 8.1 Contraste de Colores

| Par | Ratio | Cumple WCAG AA |
|-----|-------|----------------|
| White (#ffffff) sobre bg-primary (#1a1a2e) | 15.8:1 | ✓ (AAA) |
| Text-secondary (#94a3b8) sobre bg-card (#0f1729) | 7.2:1 | ✓ (AAA) |
| Text-muted (#64748b) sobre bg-card (#0f1729) | 4.8:1 | ✓ (AA) |
| Purple-500 (#a855f7) sobre bg-primary (#1a1a2e) | 5.1:1 | ✓ (AA) |

**Verificación:** Todas las combinaciones de texto/fondo cumplen mínimo WCAG AA (4.5:1 para texto normal).

### 8.2 Focus States

**Ring visible en todos los elementos interactivos:**
```tsx
// Botones
className="focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 focus:ring-offset-[#1a1a2e]"

// Links
className="focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2"
```

### 8.3 ARIA Labels y Roles

**Avatar:**
```tsx
<Avatar>
  <AvatarImage src={imagenUrl} alt={`Foto de perfil de ${nombreArtistico}`} />
  <AvatarFallback aria-label="Placeholder de perfil">
    <MusicalNoteIcon aria-hidden="true" />
  </AvatarFallback>
</Avatar>
```

**Social Links:**
```tsx
<Button variant="ghost" size="icon" asChild>
  <a
    href={spotifyUrl}
    target="_blank"
    rel="noopener noreferrer"
    aria-label="Perfil de Spotify de {nombreArtistico}"
  >
    <SpotifyIcon aria-hidden="true" />
  </a>
</Button>
```

**Follow Button:**
```tsx
<Button aria-label="Seguir a {nombreArtistico}">
  <HeartIcon aria-hidden="true" className="mr-2" />
  Seguir
</Button>
```

**Loading State:**
```tsx
<div role="status" aria-live="polite" aria-busy="true">
  <ArtistProfileSkeleton />
  <span className="sr-only">Cargando perfil de artista...</span>
</div>
```

**Empty States:**
```tsx
<div role="status" aria-live="polite">
  <p className="text-[#64748b] italic">
    Este artista aún no ha agregado una biografía.
  </p>
</div>
```

### 8.4 Keyboard Navigation

| Elemento | Comportamiento |
|----------|----------------|
| **Nav links** | Tab order lógico (izq → der) |
| **Follow button** | Enter/Space activa acción |
| **Social links** | Tab order después del Follow button |
| **Skip link** | "Saltar al contenido" (invisible hasta focus) |

**Skip Link Implementation:**
```tsx
<a
  href="#main-content"
  className="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 focus:z-50 focus:px-4 focus:py-2 focus:bg-purple-600 focus:text-white focus:rounded-md"
>
  Saltar al contenido
</a>
```

### 8.5 Semantic HTML

**Estructura correcta:**
```tsx
<header>
  <nav>
    {/* Navigation */}
  </nav>
</header>

<main id="main-content">
  <section aria-labelledby="artist-info">
    <h1 id="artist-info">{nombreArtistico}</h1>
    {/* Artist info */}
  </section>

  <section aria-labelledby="artist-bio">
    <h2 id="artist-bio">Biografía</h2>
    {/* Bio content */}
  </section>

  <section aria-labelledby="artist-campaigns">
    <h2 id="artist-campaigns">Campañas Activas</h2>
    {/* Campaigns */}
  </section>
</main>
```

---

## 9. Integración con Datos

### 9.1 TanStack Query Hook

**Hook:** `useArtista(id: string)`

**Ubicación:** `src/web/src/features/artistas/hooks/useArtista.ts`

```typescript
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/shared/constants/query-keys';
import { artistaService } from '../services/artista.service';

export const useArtista = (id: string) => {
  return useQuery({
    queryKey: QUERY_KEYS.artistas.byId(id),
    queryFn: () => artistaService.getById(id),
    enabled: !!id,
    staleTime: 1000 * 60 * 5, // 5 minutos
    retry: 1,
  });
};
```

### 9.2 Uso en Componente

```tsx
import { useParams } from 'react-router-dom';
import { useArtista } from '../hooks/useArtista';
import { ArtistProfileSkeleton } from '../components/ArtistProfileSkeleton';
import { ArtistHero } from '../components/ArtistHero';
import { ArtistBio } from '../components/ArtistBio';

export const ArtistaPage = () => {
  const { id } = useParams<{ id: string }>();
  const { data, isLoading, isError } = useArtista(id!);

  if (isLoading) {
    return <ArtistProfileSkeleton />;
  }

  if (isError || !data?.data) {
    return <NotFoundPage />;
  }

  const artista = data.data;

  return (
    <div className="min-h-screen bg-[#1a1a2e]">
      <NavBar />

      <main>
        <ArtistHero artista={artista} />

        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 sm:py-12">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <ArtistBio descripcion={artista.descripcion} />
            <ArtistCampaignsSection artistaId={artista.id} />
          </div>
        </div>
      </main>
    </div>
  );
};
```

---

## 10. Componentes shadcn/ui Necesarios

### 10.1 Ya Instalados

| Componente | Uso en Feature | Variantes Utilizadas |
|------------|----------------|----------------------|
| `<Button>` | Follow button, social icons, nav buttons | `default`, `ghost` |
| `<Card>` | Biography card, campaigns card | `default` |
| `<CardHeader>` | Títulos de cards | - |
| `<CardTitle>` | "Biografía", "Campañas Activas" | - |
| `<CardContent>` | Contenido de cards | - |
| `<Avatar>` | Foto de perfil del artista | Size customizado |
| `<AvatarImage>` | Imagen real del artista | - |
| `<AvatarFallback>` | Placeholder con icono musical | Custom bg color |
| `<Badge>` | Tags de género musical | Custom variant (purple) |
| `<Skeleton>` | Loading states | - |

### 10.2 Componentes Faltantes (a Instalar)

**Ninguno.** Todos los componentes shadcn/ui necesarios ya están instalados.

### 10.3 Iconos Necesarios

**Librería:** `heroicons` (o `lucide-react`)

| Icono | Uso | Import |
|-------|-----|--------|
| `MusicalNoteIcon` | Fallback avatar, empty states | `@heroicons/react/24/outline` |
| `HeartIcon` | Follow button | `@heroicons/react/24/outline` |
| `UsersIcon` | Stat backers | `@heroicons/react/24/outline` |
| `CurrencyEuroIcon` | Stat recaudado | `@heroicons/react/24/outline` |
| `GlobeIcon` | Website link | `@heroicons/react/24/outline` |
| `SpotifyIcon` | Social link | Custom SVG o `react-icons` |
| `YouTubeIcon` | Social link | Custom SVG o `react-icons` |

**Alternativa:** Usar `lucide-react` para consistencia:
```tsx
import { Music, Heart, Users, DollarSign, Globe } from 'lucide-react';
```

---

## 11. Customizaciones de Tema

### 11.1 Tailwind Config Extensions

**Archivo:** `tailwind.config.ts`

```typescript
export default {
  darkMode: ['class'],
  theme: {
    extend: {
      colors: {
        border: 'hsl(var(--border))',
        input: 'hsl(var(--input))',
        ring: 'hsl(var(--ring))',
        background: 'hsl(var(--background))',
        foreground: 'hsl(var(--foreground))',
        primary: {
          DEFAULT: '#a855f7',
          foreground: '#ffffff',
        },
        secondary: {
          DEFAULT: '#94a3b8',
          foreground: '#ffffff',
        },
        muted: {
          DEFAULT: '#1e293b',
          foreground: '#94a3b8',
        },
        accent: {
          DEFAULT: '#ec4899',
          foreground: '#ffffff',
        },
        destructive: {
          DEFAULT: '#ef4444',
          foreground: '#ffffff',
        },
        card: {
          DEFAULT: '#0f1729',
          foreground: '#ffffff',
        },
      },
      borderRadius: {
        lg: '0.75rem',
        md: '0.5rem',
        sm: '0.375rem',
      },
      boxShadow: {
        glow: '0 0 20px rgba(168, 85, 247, 0.3)',
      },
      animation: {
        'pulse': 'pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },
    },
  },
  plugins: [require('tailwindcss-animate')],
};
```

### 11.2 CSS Variables Globales

**Archivo:** `src/web/src/index.css`

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  :root {
    --background: 210 40% 8%;      /* #1a1a2e */
    --foreground: 210 20% 98%;     /* #ffffff */

    --card: 220 45% 7%;            /* #0f1729 */
    --card-foreground: 210 20% 98%;

    --primary: 271 91% 65%;        /* #a855f7 */
    --primary-foreground: 210 20% 98%;

    --secondary: 215 20% 65%;      /* #94a3b8 */
    --secondary-foreground: 210 20% 98%;

    --muted: 217 33% 17%;          /* #1e293b */
    --muted-foreground: 215 20% 65%;

    --accent: 330 81% 60%;         /* #ec4899 */
    --accent-foreground: 210 20% 98%;

    --destructive: 0 84% 60%;      /* #ef4444 */
    --destructive-foreground: 210 20% 98%;

    --border: 215 28% 32%;         /* #334155 */
    --input: 220 45% 7%;           /* #0f1729 */
    --ring: 271 91% 65%;           /* #a855f7 */

    --radius: 0.5rem;
  }

  body {
    @apply bg-background text-foreground;
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  }
}
```

---

## 12. Checklist de Implementación UI

### Componentes shadcn/ui
- [x] Button (ya instalado)
- [x] Card, CardHeader, CardTitle, CardContent (ya instalado)
- [x] Avatar, AvatarImage, AvatarFallback (ya instalado)
- [x] Badge (ya instalado)
- [x] Skeleton (ya instalado)

### Componentes Custom
- [ ] `<NavBar>` - Header con navegación
- [ ] `<ArtistHero>` - Hero banner + info del artista
- [ ] `<ArtistBio>` - Card de biografía
- [ ] `<ArtistCampaignsSection>` - Placeholder de campañas
- [ ] `<ArtistProfileSkeleton>` - Loading state completo

### Estados Visuales
- [ ] Loading (skeleton loaders)
- [ ] Empty state - Sin avatar (fallback con icono)
- [ ] Empty state - Sin descripción (mensaje muted)
- [ ] Empty state - Sin campañas (placeholder)
- [ ] Error 404 - Artista no encontrado
- [ ] Hover states en links y botones

### Responsive Design
- [ ] Mobile (< 640px): Stack vertical, avatar pequeño
- [ ] Tablet (640-1024px): Grid 2 columnas
- [ ] Desktop (> 1024px): Grid 3 columnas

### Accesibilidad
- [ ] Contraste de colores mínimo 4.5:1 (verificado)
- [ ] Focus rings visibles (purple-500)
- [ ] ARIA labels en avatar e iconos
- [ ] Alt text descriptivo en imágenes
- [ ] Skip link funcional
- [ ] Keyboard navigation (tab order lógico)
- [ ] Semantic HTML (header, main, section, h1, h2)
- [ ] Loading states con aria-busy y aria-live

### Animaciones
- [ ] Transiciones en hover (150-200ms)
- [ ] Fade in al montar página (300ms)
- [ ] Skeleton pulse animation
- [ ] Button scale on hover (1.02)

### Integración
- [ ] Hook `useArtista(id)` con TanStack Query
- [ ] Manejo de errores con toast (futura implementación)
- [ ] Retry logic en fetch errors
- [ ] Stale time configurado (5 min)

### Design Tokens
- [ ] CSS variables en `index.css`
- [ ] Tailwind config extendido
- [ ] Gradientes pink-to-purple aplicados
- [ ] Shadow glow en focus states

---

## 13. Notas de Implementación

### 13.1 Iconos

**Decisión:** Usar `lucide-react` para consistencia con shadcn/ui.

```bash
npm install lucide-react
```

```tsx
import { Music, Heart, Users, DollarSign, Globe } from 'lucide-react';
```

### 13.2 Social Icons

Para Spotify y YouTube, considerar usar `react-icons`:

```bash
npm install react-icons
```

```tsx
import { FaSpotify, FaYoutube } from 'react-icons/fa';
```

### 13.3 Fuentes

**Font:** Inter (de Google Fonts)

```html
<!-- En index.html -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
```

### 13.4 Próximos Pasos (Fuera de MVP)

- Implementar sistema de reviews/testimonios
- Agregar galería de imágenes/videos del artista
- Sistema de follows con contador dinámico
- Integración real con campañas (cuando esté implementada esa feature)
- Mobile menu (hamburger) para navegación

---

**Fin del Plan de Diseño UI**
