# Diseño UI: Registro de Artista (Landing)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/web (Landing - Vite + React)

## 1. Resumen

- Componentes shadcn: 8 componentes base (Card, Avatar, Badge, Button, Skeleton, Separator, Tabs, Alert)
- Composiciones custom: 5 (ArtistaProfilePage, ArtistaHero, ArtistaBio, CampaniasPreview, ReviewsList)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px), xl (1280px)
- Alcance: Solo perfil público de artista (`/artistas/{id}`) - Sin formularios de registro/creación

## 2. Paleta de Colores (del proyecto)

| Uso | Variable CSS | Tailwind Class | Ejemplo |
|-----|--------------|----------------|---------|
| Primary | `--primary-color` | `text-primary` `border-primary` | Gradient pink/purple, botones principales |
| Background Primary | `--bg-primary` | `bg-[#1a1a2e]` | Fondo principal dark |
| Background Secondary | `--bg-secondary` | `bg-[#16213e]` | Fondo alternativo |
| Background Card | `--bg-card` | `bg-[#0f1729]` | Cards de contenido |
| Background Card Hover | `--bg-card-hover` | `bg-[#1e2a42]` | Hover en cards |
| Text Primary | `--text-primary` | `text-white` | Títulos, textos principales |
| Text Secondary | `--text-secondary` | `text-[#94a3b8]` | Subtítulos, descripciones |
| Text Muted | `--text-muted` | `text-[#64748b]` | Textos auxiliares, placeholders |
| Border Primary | `--border-primary` | `border-[#334155]` | Bordes de cards, separadores |
| Gradient Primary | `--primary-gradient` | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones, badges activos |
| Success | `--status-success` | `bg-green-500` `text-green-400` | Badge "Activa" |
| Info | `--status-info` | `bg-blue-500` `text-blue-400` | Badge genérico |

## 3. Componentes por Screen

### 3.1 ArtistaProfilePage (`/artistas/{id}`)

#### Layout
```
┌────────────────────────────────────────────────────────────┐
│ Header (sticky)                                            │
│ [LOGO] MusicFund  Explorar  Para Artistas  Cómo funciona  │
│                             Iniciar Sesión  [Registro]     │
├────────────────────────────────────────────────────────────┤
│                                                            │
│ ┌────────────────────────────────────────────────────────┐ │
│ │        Hero Banner (gradient/image)                    │ │
│ │                                                        │ │
│ └────────────────────────────────────────────────────────┘ │
│                                                            │
│   [Avatar]  Luna Vibe                                      │
│             Electronic  Synthwave  Indie                   │
│                                                            │
│   🎵 3 campañas  👥 247 backers  💰 €12,450 recaudados      │
│                                                            │
│   [💜 Seguir]  🌐 🎵 📺                                      │
│                                                            │
│ ┌──────────────────────┐  ┌────────────────────────────┐  │
│ │ Biografía            │  │ Campañas Activas           │  │
│ │                      │  │                            │  │
│ │ Luna Vibe es...      │  │ ┌────────────────────────┐ │  │
│ │                      │  │ │ [IMG] Neon Dreams      │ │  │
│ │ Desde Barcelona...   │  │ │ €8,750 / €15,000      │ │  │
│ │                      │  │ │ 58% ████░░░          │ │  │
│ │ 🎵 Spotify           │  │ └────────────────────────┘ │  │
│ │ 📺 YouTube           │  │                            │  │
│ │ 🌐 Sitio oficial     │  │ [Ver todas]                │  │
│ └──────────────────────┘  └────────────────────────────┘  │
│                                                            │
│ ┌──────────────────────┐                                   │
│ │ Reseñas de Fans      │                                   │
│ │                      │                                   │
│ │ 👤 Carlos M. ⭐⭐⭐⭐⭐   │                                   │
│ │ Increíble artista... │                                   │
│ └──────────────────────┘                                   │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

#### Componentes

**Page Container**

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Layout principal | - (div) | `min-h-screen bg-[#1a1a2e]` |
| Content wrapper | - (div) | `max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6` |

**Composición general:**
```tsx
<div className="min-h-screen bg-[#1a1a2e]">
  {/* Header sticky (shared component fuera de scope) */}
  <ArtistaHero artista={artista} />

  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <ArtistaStats artista={artista} />
    <ArtistaActions artista={artista} />

    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-8">
      <div className="lg:col-span-1">
        <ArtistaBio artista={artista} />
      </div>
      <div className="lg:col-span-2">
        <CampaniasPreview artistaId={artista.id} />
      </div>
    </div>

    <div className="mt-8">
      <ReviewsList artistaId={artista.id} />
    </div>
  </div>
</div>
```

---

### 3.2 ArtistaHero Component

**Propósito:** Hero banner con gradiente/imagen y avatar del artista

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Hero container | - (div) | `relative h-48 sm:h-56 md:h-64 bg-gradient-to-r from-purple-900 via-purple-800 to-pink-900` |
| Backdrop blur overlay | - (div) | `absolute inset-0 bg-black/20 backdrop-blur-[2px]` (si hay imagen) |
| Avatar container | - (div) | `absolute -bottom-16 left-1/2 -translate-x-1/2 sm:left-8 sm:translate-x-0` |
| Avatar | Avatar, AvatarImage, AvatarFallback | `w-32 h-32 border-4 border-[#1a1a2e] shadow-xl` |
| Fallback icon | - (Icon) | `lucide-react Music icon, text-purple-300 w-12 h-12` |

**Composición:**
```tsx
<div className="relative h-48 sm:h-56 md:h-64 bg-gradient-to-r from-purple-900 via-purple-800 to-pink-900">
  {artista.imagenUrl && (
    <>
      <img
        src={artista.imagenUrl}
        alt=""
        className="absolute inset-0 w-full h-full object-cover"
        aria-hidden="true"
      />
      <div className="absolute inset-0 bg-black/20 backdrop-blur-[2px]" />
    </>
  )}

  <div className="absolute -bottom-16 left-1/2 -translate-x-1/2 sm:left-8 sm:translate-x-0">
    <Avatar className="w-32 h-32 border-4 border-[#1a1a2e] shadow-xl">
      <AvatarImage
        src={artista.imagenUrl}
        alt={`Foto de perfil de ${artista.nombreArtistico}`}
      />
      <AvatarFallback className="bg-purple-900/50">
        <Music className="w-12 h-12 text-purple-300" aria-hidden="true" />
      </AvatarFallback>
    </Avatar>
  </div>
</div>
```

**Estados:**
- **Default**: Gradient background + avatar con imagen
- **No Image**: Avatar muestra fallback con ícono Music
- **Loading**: Skeleton con mismo alto + Avatar skeleton

---

### 3.3 ArtistaStats Component

**Propósito:** Mostrar nombre, géneros y estadísticas

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Container | - (div) | `mt-20 sm:mt-8 sm:ml-44 text-center sm:text-left` |
| Nombre artístico | - (h1) | `text-3xl sm:text-4xl font-bold text-white mb-3` |
| Géneros container | - (div) | `flex flex-wrap gap-2 justify-center sm:justify-start mb-4` |
| Badge género | Badge | `bg-purple-900/50 text-purple-200 border-purple-500/30 hover:bg-purple-900/70` |
| Stats row | - (div) | `flex flex-wrap gap-4 sm:gap-6 justify-center sm:justify-start text-[#94a3b8] text-sm` |
| Stat item | - (span) | `flex items-center gap-2` |
| Stat icon | - (Icon) | `w-4 h-4` |

**Composición:**
```tsx
<div className="mt-20 sm:mt-8 sm:ml-44 text-center sm:text-left">
  <h1 className="text-3xl sm:text-4xl font-bold text-white mb-3">
    {artista.nombreArtistico}
  </h1>

  {/* Géneros - placeholder para MVP (no en DB) */}
  <div className="flex flex-wrap gap-2 justify-center sm:justify-start mb-4">
    <Badge variant="secondary" className="bg-purple-900/50 text-purple-200 border-purple-500/30">
      Electronic
    </Badge>
    <Badge variant="secondary" className="bg-purple-900/50 text-purple-200 border-purple-500/30">
      Synthwave
    </Badge>
  </div>

  {/* Stats - placeholder para MVP (no en DB aún) */}
  <div className="flex flex-wrap gap-4 sm:gap-6 justify-center sm:justify-start text-[#94a3b8] text-sm">
    <span className="flex items-center gap-2">
      <Music className="w-4 h-4" aria-hidden="true" />
      <span><strong className="text-white">0</strong> campañas</span>
    </span>
    <span className="flex items-center gap-2">
      <Users className="w-4 h-4" aria-hidden="true" />
      <span><strong className="text-white">0</strong> backers</span>
    </span>
    <span className="flex items-center gap-2">
      <Euro className="w-4 h-4" aria-hidden="true" />
      <span><strong className="text-white">€0</strong> recaudados</span>
    </span>
  </div>
</div>
```

**Estados:**
- **Loading**: Skeleton para nombre, badges y stats
- **Empty**: Mostrar "0 campañas" si no hay campañas aún

---

### 3.4 ArtistaActions Component

**Propósito:** Botón Seguir y enlaces sociales

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Container | - (div) | `mt-6 flex flex-wrap gap-3 justify-center sm:justify-start sm:ml-44` |
| Follow button | Button | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white px-6` |
| Social icons container | - (div) | `flex gap-2` |
| Social icon button | Button | `variant="ghost" size="icon" text-[#94a3b8] hover:text-white hover:bg-white/10` |

**Composición:**
```tsx
<div className="mt-6 flex flex-wrap gap-3 justify-center sm:justify-start sm:ml-44">
  <Button
    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white px-6"
    onClick={handleFollowClick}
  >
    <Heart className="w-4 h-4 mr-2" aria-hidden="true" />
    Seguir
  </Button>

  <div className="flex gap-2">
    <Button
      variant="ghost"
      size="icon"
      className="text-[#94a3b8] hover:text-white hover:bg-white/10"
      asChild
    >
      <a href="https://spotify.com" target="_blank" rel="noopener noreferrer" aria-label="Spotify">
        <Music className="w-4 h-4" />
      </a>
    </Button>
    <Button
      variant="ghost"
      size="icon"
      className="text-[#94a3b8] hover:text-white hover:bg-white/10"
      asChild
    >
      <a href="https://youtube.com" target="_blank" rel="noopener noreferrer" aria-label="YouTube">
        <Youtube className="w-4 h-4" />
      </a>
    </Button>
    <Button
      variant="ghost"
      size="icon"
      className="text-[#94a3b8] hover:text-white hover:bg-white/10"
      asChild
    >
      <a href="https://example.com" target="_blank" rel="noopener noreferrer" aria-label="Sitio web">
        <Globe className="w-4 h-4" />
      </a>
    </Button>
  </div>
</div>
```

**Estados:**
- **Follow button**: Placeholder (funcionalidad fuera de MVP)
- **Social links**: Placeholder con URLs de ejemplo
- **Hover**: Cambio de color a white + background subtle

---

### 3.5 ArtistaBio Component

**Propósito:** Card con biografía y enlaces sociales del artista

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Card container | Card | `bg-[#0f1729] border-[#334155]` |
| Card header | CardHeader | `pb-3` |
| Card title | CardTitle | `text-xl font-bold text-white` |
| Card content | CardContent | `pt-0` |
| Bio text | - (p) | `text-[#94a3b8] text-sm leading-relaxed whitespace-pre-wrap mb-6` |
| Social links list | - (div) | `space-y-3` |
| Social link | - (a) | `flex items-center gap-3 text-primary hover:text-purple-400 hover:underline transition-colors` |
| Social icon | - (Icon) | `w-5 h-5` |
| Empty state | - (p) | `text-[#64748b] italic text-sm` |
| Separator | Separator | `my-4 bg-[#334155]` |

**Composición:**
```tsx
<Card className="bg-[#0f1729] border-[#334155]">
  <CardHeader className="pb-3">
    <CardTitle className="text-xl font-bold text-white">
      Biografía
    </CardTitle>
  </CardHeader>
  <CardContent className="pt-0">
    {artista.descripcion ? (
      <p className="text-[#94a3b8] text-sm leading-relaxed whitespace-pre-wrap mb-6">
        {artista.descripcion}
      </p>
    ) : (
      <p className="text-[#64748b] italic text-sm mb-6">
        Este artista aún no ha agregado una biografía.
      </p>
    )}

    {/* Location */}
    {(artista.ciudad || artista.pais) && (
      <p className="text-[#94a3b8] text-sm mb-4 flex items-center gap-2">
        <MapPin className="w-4 h-4" aria-hidden="true" />
        <span>
          {[artista.ciudad, artista.pais].filter(Boolean).join(', ')}
        </span>
      </p>
    )}

    <Separator className="my-4 bg-[#334155]" />

    {/* Social links - placeholder para MVP */}
    <div className="space-y-3">
      <a
        href="https://spotify.com"
        target="_blank"
        rel="noopener noreferrer"
        className="flex items-center gap-3 text-primary hover:text-purple-400 hover:underline transition-colors"
      >
        <Music className="w-5 h-5" aria-hidden="true" />
        <span className="text-sm">Escuchar en Spotify</span>
      </a>
      <a
        href="https://youtube.com"
        target="_blank"
        rel="noopener noreferrer"
        className="flex items-center gap-3 text-primary hover:text-purple-400 hover:underline transition-colors"
      >
        <Youtube className="w-5 h-5" aria-hidden="true" />
        <span className="text-sm">Videos en YouTube</span>
      </a>
      <a
        href="https://example.com"
        target="_blank"
        rel="noopener noreferrer"
        className="flex items-center gap-3 text-primary hover:text-purple-400 hover:underline transition-colors"
      >
        <Globe className="w-5 h-5" aria-hidden="true" />
        <span className="text-sm">Sitio web oficial</span>
      </a>
    </div>
  </CardContent>
</Card>
```

**Estados:**
- **Default**: Mostrar descripción completa
- **No Description**: Mensaje muted "Este artista aún no ha agregado una biografía"
- **No Location**: Ocultar sección de ubicación
- **Loading**: Skeleton para texto

---

### 3.6 CampaniasPreview Component

**Propósito:** Tabs con campañas activas/pasadas (placeholder para MVP)

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Card container | Card | `bg-[#0f1729] border-[#334155]` |
| Tabs root | Tabs | `defaultValue="activas"` |
| Tabs list | TabsList | `bg-[#1a1a2e] border-b border-[#334155]` |
| Tab trigger | TabsTrigger | `data-[state=active]:bg-[#2d1b4e] data-[state=active]:text-primary` |
| Tab content | TabsContent | - |
| Empty state | Alert | `bg-[#1a1a2e] border-[#334155] text-[#94a3b8]` |
| Empty icon | - (Icon) | `Music w-12 h-12 text-[#64748b] mx-auto mb-3` |

**Composición:**
```tsx
<Card className="bg-[#0f1729] border-[#334155]">
  <Tabs defaultValue="activas" className="w-full">
    <TabsList className="w-full bg-[#1a1a2e] border-b border-[#334155] rounded-none justify-start">
      <TabsTrigger
        value="activas"
        className="data-[state=active]:bg-[#2d1b4e] data-[state=active]:text-primary"
      >
        Campañas Activas
      </TabsTrigger>
      <TabsTrigger
        value="pasadas"
        className="data-[state=active]:bg-[#2d1b4e] data-[state=active]:text-primary"
      >
        Campañas Pasadas
      </TabsTrigger>
    </TabsList>

    <TabsContent value="activas" className="p-6">
      {/* Empty state para MVP */}
      <Alert className="bg-[#1a1a2e] border-[#334155] text-center">
        <Music className="w-12 h-12 text-[#64748b] mx-auto mb-3" aria-hidden="true" />
        <AlertTitle className="text-white mb-2">Próximamente campañas</AlertTitle>
        <AlertDescription className="text-[#94a3b8]">
          Este artista aún no ha lanzado campañas. ¡Mantente atento!
        </AlertDescription>
      </Alert>
    </TabsContent>

    <TabsContent value="pasadas" className="p-6">
      <Alert className="bg-[#1a1a2e] border-[#334155] text-center">
        <Music className="w-12 h-12 text-[#64748b] mx-auto mb-3" aria-hidden="true" />
        <AlertTitle className="text-white mb-2">Sin campañas finalizadas</AlertTitle>
        <AlertDescription className="text-[#94a3b8]">
          Este artista no ha finalizado campañas aún.
        </AlertDescription>
      </Alert>
    </TabsContent>
  </Tabs>
</Card>
```

**Nota MVP:** Este componente muestra placeholder. La integración real con campañas será implementada en feature "crear-campania".

---

### 3.7 ReviewsList Component (Opcional - Fuera de MVP)

**Propósito:** Placeholder para reseñas de fans

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Card container | Card | `bg-[#0f1729] border-[#334155]` |
| Card header | CardHeader | `pb-3` |
| Card title | CardTitle | `text-xl font-bold text-white` |
| Empty state | - (p) | `text-[#64748b] italic text-sm text-center py-8` |

**Composición:**
```tsx
<Card className="bg-[#0f1729] border-[#334155]">
  <CardHeader className="pb-3">
    <CardTitle className="text-xl font-bold text-white">
      Reseñas de Fans
    </CardTitle>
  </CardHeader>
  <CardContent>
    <p className="text-[#64748b] italic text-sm text-center py-8">
      Las reseñas estarán disponibles próximamente.
    </p>
  </CardContent>
</Card>
```

**Nota:** Este componente es placeholder. Feature de reviews fuera del alcance del MVP.

---

## 4. Loading States

### 4.1 ArtistaProfilePage Loading

**Componente:** `ArtistaProfileSkeleton`

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Hero skeleton | Skeleton | `h-48 sm:h-56 md:h-64 w-full rounded-none` |
| Avatar skeleton | Skeleton | `w-32 h-32 rounded-full absolute -bottom-16 left-1/2 -translate-x-1/2 sm:left-8 sm:translate-x-0` |
| Title skeleton | Skeleton | `h-10 w-64 mx-auto sm:mx-0 sm:ml-44 mt-20 sm:mt-8` |
| Badges skeleton | Skeleton | `h-6 w-24` (3 instancias con gap) |
| Stats skeleton | Skeleton | `h-5 w-32` (3 instancias) |
| Card skeleton | Skeleton | `h-96 w-full rounded-lg` |

**Composición:**
```tsx
<div className="min-h-screen bg-[#1a1a2e]">
  <div className="relative">
    <Skeleton className="h-48 sm:h-56 md:h-64 w-full rounded-none" />
    <Skeleton className="w-32 h-32 rounded-full absolute -bottom-16 left-1/2 -translate-x-1/2 sm:left-8 sm:translate-x-0" />
  </div>

  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div className="mt-20 sm:mt-8 sm:ml-44 space-y-4">
      <Skeleton className="h-10 w-64 mx-auto sm:mx-0" />
      <div className="flex gap-2 justify-center sm:justify-start">
        <Skeleton className="h-6 w-24" />
        <Skeleton className="h-6 w-24" />
      </div>
      <div className="flex gap-6 justify-center sm:justify-start">
        <Skeleton className="h-5 w-32" />
        <Skeleton className="h-5 w-32" />
        <Skeleton className="h-5 w-32" />
      </div>
    </div>

    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-8">
      <Skeleton className="h-96 w-full rounded-lg" />
      <Skeleton className="lg:col-span-2 h-96 w-full rounded-lg" />
    </div>
  </div>
</div>
```

---

## 5. Error States

### 5.1 Artista Not Found (404)

**Componente:** `ArtistaNotFoundPage`

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Container | - (div) | `min-h-[calc(100vh-4rem)] flex items-center justify-center bg-[#1a1a2e] px-4` |
| Icon | - (Icon) | `Music w-24 h-24 text-[#64748b] mx-auto mb-6` |
| Title | - (h1) | `text-3xl font-bold text-white mb-3 text-center` |
| Description | - (p) | `text-[#94a3b8] text-center mb-8 max-w-md` |
| Button | Button | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |

**Composición:**
```tsx
<div className="min-h-[calc(100vh-4rem)] flex items-center justify-center bg-[#1a1a2e] px-4">
  <div className="text-center">
    <Music className="w-24 h-24 text-[#64748b] mx-auto mb-6" aria-hidden="true" />
    <h1 className="text-3xl font-bold text-white mb-3">
      Artista no encontrado
    </h1>
    <p className="text-[#94a3b8] text-center mb-8 max-w-md">
      El perfil que buscas no existe o ha sido eliminado.
    </p>
    <Button
      className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
      onClick={() => navigate('/explorar')}
    >
      Explorar artistas
    </Button>
  </div>
</div>
```

### 5.2 Error de Carga

**Componente:** `ArtistaErrorState`

| Elemento | Componente shadcn | Customización |
|----------|-------------------|---------------|
| Alert container | Alert | `bg-red-950/50 border-red-900 text-red-200` |
| Alert icon | - (Icon) | `AlertCircle w-5 h-5` |
| Alert title | AlertTitle | `text-red-100 mb-2` |
| Alert description | AlertDescription | `text-red-200/80` |
| Retry button | Button | `mt-4 bg-red-900 hover:bg-red-800` |

**Composición:**
```tsx
<div className="min-h-screen bg-[#1a1a2e] flex items-center justify-center px-4">
  <Alert className="max-w-md bg-red-950/50 border-red-900 text-red-200">
    <AlertCircle className="w-5 h-5" />
    <AlertTitle className="text-red-100 mb-2">Error al cargar perfil</AlertTitle>
    <AlertDescription className="text-red-200/80">
      No pudimos cargar el perfil del artista. Por favor, intenta nuevamente.
    </AlertDescription>
    <Button
      onClick={handleRetry}
      className="mt-4 w-full bg-red-900 hover:bg-red-800"
    >
      Reintentar
    </Button>
  </Alert>
</div>
```

---

## 6. Responsive Design

| Breakpoint | Width | Cambios Layout |
|------------|-------|----------------|
| **Mobile** | < 640px | - Hero h-48<br>- Avatar w-24 h-24, centrado<br>- Stats apilados 2x2 grid<br>- Bio y Campañas stack vertical<br>- Padding px-4 |
| **Small** | 640px - 767px | - Hero h-56<br>- Avatar w-28 h-28, left-8<br>- Stats en fila, wrap permitido<br>- Bio y Campañas stack vertical<br>- Padding px-6 |
| **Medium** | 768px - 1023px | - Hero h-64<br>- Avatar w-32 h-32<br>- Stats en fila sin wrap<br>- Bio y Campañas stack vertical<br>- Padding px-6 |
| **Large** | >= 1024px | - Hero h-64<br>- Avatar w-32 h-32<br>- Grid 3 columnas (1 col Bio, 2 cols Campañas)<br>- Reviews full width abajo<br>- Padding px-8 |

### Responsive Clases Detalladas

**Hero Banner:**
```tsx
className="h-48 sm:h-56 md:h-64"
```

**Avatar:**
```tsx
className="w-24 h-24 sm:w-28 sm:h-28 md:w-32 md:h-32 -bottom-12 sm:-bottom-14 md:-bottom-16 left-1/2 -translate-x-1/2 sm:left-8 sm:translate-x-0"
```

**Stats Container:**
```tsx
className="mt-16 sm:mt-20 md:mt-20 sm:ml-44 text-center sm:text-left"
```

**Stats Row:**
```tsx
className="flex flex-wrap gap-4 sm:gap-6 justify-center sm:justify-start"
```

**Main Grid:**
```tsx
className="grid grid-cols-1 lg:grid-cols-3 gap-6"
```

**Bio Column:**
```tsx
className="lg:col-span-1"
```

**Campañas Column:**
```tsx
className="lg:col-span-2"
```

---

## 7. Animaciones

| Elemento | Animación | Duración | Trigger |
|----------|-----------|----------|---------|
| Button hover | `scale-105` + shadow glow | 150ms | hover |
| Card hover | `bg-[#1e2a42]` (lighten) | 200ms | hover |
| Social icon hover | `text-white` + `bg-white/10` | 150ms | hover |
| Social link hover | `text-purple-400` + underline | 200ms | hover |
| Avatar image load | `animate-fadeIn` (opacity 0 → 1) | 300ms | onLoad |
| Page load | `animate-fadeIn` (opacity 0 → 1) | 300ms | mount |
| Skeleton pulse | `animate-pulse` | infinite | loading |
| Tab switch | `animate-fadeIn` | 200ms | tab change |

**Tailwind Animations (agregar a tailwind.config.js):**
```js
theme: {
  extend: {
    animation: {
      'fadeIn': 'fadeIn 300ms ease-in-out',
    },
    keyframes: {
      fadeIn: {
        '0%': { opacity: '0' },
        '100%': { opacity: '1' },
      },
    },
  },
}
```

---

## 8. Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de color** | - Text white (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 ✓<br>- Text secondary (#94a3b8) sobre bg-primary = 7.2:1 ✓<br>- Primary purple (#a855f7) sobre dark = 4.8:1 ✓ |
| **Focus visible** | - `focus:ring-2 focus:ring-primary focus:ring-offset-2 focus:ring-offset-[#1a1a2e]` en todos los botones y links |
| **Alt text en imágenes** | - Avatar: `alt="Foto de perfil de {nombreArtistico}"`<br>- Hero banner: `alt=""` (decorativa) con `aria-hidden="true"` |
| **ARIA labels en icon buttons** | - Social icons: `aria-label="Spotify"`, `aria-label="YouTube"`, etc. |
| **Heading hierarchy** | - h1: Nombre artístico<br>- h2: Secciones (Biografía, Campañas, Reseñas) |
| **Link external indicators** | - `target="_blank"` + `rel="noopener noreferrer"` en todos los enlaces sociales |
| **Loading states** | - `aria-busy="true"` en contenedores durante carga<br>- `aria-live="polite"` en mensajes de error |
| **Empty states** | - Mensajes descriptivos en lugar de "No data"<br>- Iconos con `aria-hidden="true"` |
| **Keyboard navigation** | - Tab order lógico: Follow → Social icons → Bio links → Campañas tabs<br>- Enter para activar botones y links<br>- Arrow keys para navegar tabs |
| **Screen reader** | - Textos descriptivos en fallbacks de Avatar<br>- `aria-label` en iconos decorativos |

### ARIA Labels Detallados

**Avatar:**
```tsx
<Avatar>
  <AvatarImage
    src={artista.imagenUrl}
    alt={`Foto de perfil de ${artista.nombreArtistico}`}
  />
  <AvatarFallback>
    <Music aria-hidden="true" />
    <span className="sr-only">Sin foto de perfil</span>
  </AvatarFallback>
</Avatar>
```

**Social Icon Buttons:**
```tsx
<Button aria-label="Visitar perfil en Spotify" asChild>
  <a href={spotifyUrl} target="_blank" rel="noopener noreferrer">
    <Music aria-hidden="true" />
  </a>
</Button>
```

**Stats Icons:**
```tsx
<Music className="w-4 h-4" aria-hidden="true" />
<span className="sr-only">Campañas:</span>
<span>3 campañas</span>
```

**Empty State:**
```tsx
<Alert role="status">
  <Music aria-hidden="true" />
  <AlertTitle>Próximamente campañas</AlertTitle>
  <AlertDescription>
    Este artista aún no ha lanzado campañas. ¡Mantente atento!
  </AlertDescription>
</Alert>
```

**Loading Skeleton:**
```tsx
<div role="status" aria-live="polite" aria-busy="true">
  <span className="sr-only">Cargando perfil del artista...</span>
  <Skeleton className="h-64 w-full" />
</div>
```

---

## 9. Props Interfaces

### ArtistaProfilePageProps
```typescript
interface ArtistaProfilePageProps {
  artistaId: string; // from route params
}
```

### ArtistaHeroProps
```typescript
import { Artista } from '@/shared/types/artista';

interface ArtistaHeroProps {
  artista: Artista;
}
```

### ArtistaStatsProps
```typescript
import { Artista } from '@/shared/types/artista';

interface ArtistaStatsProps {
  artista: Artista;
  campanasCount?: number; // Opcional para MVP
  backersCount?: number;
  totalRecaudado?: number;
}
```

### ArtistaActionsProps
```typescript
import { Artista } from '@/shared/types/artista';

interface ArtistaActionsProps {
  artista: Artista;
  isFollowing?: boolean; // Fuera de MVP
  onFollowClick?: () => void;
}
```

### ArtistaBioProps
```typescript
import { Artista } from '@/shared/types/artista';

interface ArtistaBioProps {
  artista: Artista;
}
```

### CampaniasPreviewProps
```typescript
interface CampaniasPreviewProps {
  artistaId: string;
  // Campañas vendrán de query separado en feature futura
}
```

### ArtistaNotFoundPageProps
```typescript
interface ArtistaNotFoundPageProps {
  onExplorarClick?: () => void;
}
```

### ArtistaErrorStateProps
```typescript
interface ArtistaErrorStateProps {
  error: Error;
  onRetry: () => void;
}
```

---

## 10. Componentes shadcn/ui Necesarios

| Componente | Instalado | Uso |
|------------|-----------|-----|
| `Card`, `CardHeader`, `CardTitle`, `CardContent` | Verificar | Contenedores de Bio, Campañas, Reviews |
| `Avatar`, `AvatarImage`, `AvatarFallback` | Verificar | Foto de perfil del artista |
| `Badge` | Verificar | Géneros musicales |
| `Button` | Verificar | Follow, social icons, CTAs |
| `Skeleton` | Verificar | Loading states |
| `Separator` | Verificar | Divisor en Bio card |
| `Tabs`, `TabsList`, `TabsTrigger`, `TabsContent` | Verificar | Campañas Activas/Pasadas |
| `Alert`, `AlertTitle`, `AlertDescription` | Verificar | Empty states, error states |

**Instalación (si falta alguno):**
```bash
npx shadcn-ui@latest add card avatar badge button skeleton separator tabs alert
```

---

## 11. Iconos (lucide-react)

| Icono | Uso |
|-------|-----|
| `Music` | Fallback avatar, icono campañas, empty states |
| `Users` | Stat backers |
| `Euro` | Stat recaudado |
| `Heart` | Follow button |
| `Globe` | Social link web |
| `Youtube` | Social link YouTube |
| `MapPin` | Ubicación (ciudad/país) |
| `AlertCircle` | Error states |

**Importación:**
```typescript
import {
  Music,
  Users,
  Euro,
  Heart,
  Globe,
  Youtube,
  MapPin,
  AlertCircle
} from 'lucide-react';
```

---

## 12. Integración con TanStack Query

### Query Hook: `useArtista`

```typescript
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/shared/constants/query-keys';
import { API_ROUTES } from '@/shared/constants/api-routes';
import { Artista } from '@/shared/types/artista';
import { api } from '@/services/api';

export const useArtista = (id: string) => {
  return useQuery({
    queryKey: QUERY_KEYS.artistas.byId(id),
    queryFn: async () => {
      const response = await api.get<Artista>(API_ROUTES.artistas.byId(id));
      return response.data;
    },
    enabled: !!id,
  });
};
```

### Uso en ArtistaProfilePage

```typescript
import { useParams } from 'react-router-dom';
import { useArtista } from '@/features/artistas/hooks/useArtista';

export const ArtistaProfilePage = () => {
  const { id } = useParams<{ id: string }>();
  const { data: artista, isLoading, error } = useArtista(id!);

  if (isLoading) return <ArtistaProfileSkeleton />;
  if (error) return <ArtistaErrorState error={error} onRetry={refetch} />;
  if (!artista) return <ArtistaNotFoundPage />;

  return (
    <div className="min-h-screen bg-[#1a1a2e]">
      <ArtistaHero artista={artista} />
      {/* ... resto de componentes */}
    </div>
  );
};
```

---

## 13. Estructura de Archivos

```
src/web/src/features/artistas/
├── components/
│   ├── ArtistaHero.tsx
│   ├── ArtistaStats.tsx
│   ├── ArtistaActions.tsx
│   ├── ArtistaBio.tsx
│   ├── CampaniasPreview.tsx
│   ├── ReviewsList.tsx (placeholder)
│   ├── ArtistaProfileSkeleton.tsx
│   ├── ArtistaNotFoundPage.tsx
│   └── ArtistaErrorState.tsx
├── hooks/
│   └── useArtista.ts
├── pages/
│   └── ArtistaProfilePage.tsx
└── types/
    └── artista-profile.types.ts (props interfaces)
```

---

## 14. Checklist de Implementación

### Layout y Estructura
- [ ] ArtistaProfilePage layout con grid responsive
- [ ] Hero banner con gradient/imagen de fondo
- [ ] Avatar posicionado con -mt-16 overlay
- [ ] Grid 1 col mobile, 3 cols desktop (Bio | Campañas x2)
- [ ] Max-width container (max-w-7xl) centrado

### Componentes
- [ ] ArtistaHero con Avatar y fallback Music icon
- [ ] ArtistaStats con nombre, géneros (Badge), stats row
- [ ] ArtistaActions con Follow button + social icons
- [ ] ArtistaBio con Card, descripción, ubicación, social links
- [ ] CampaniasPreview con Tabs y empty state
- [ ] ReviewsList placeholder con empty state

### Estados UI
- [ ] Loading: ArtistaProfileSkeleton con Skeleton components
- [ ] Error: ArtistaErrorState con Alert + retry button
- [ ] 404: ArtistaNotFoundPage con mensaje y CTA
- [ ] Empty bio: Mensaje muted en ArtistaBio
- [ ] Empty campañas: Alert con icono y mensaje

### Responsive
- [ ] Hero height: h-48 sm → h-56 md → h-64
- [ ] Avatar size: w-24 mobile → w-32 desktop
- [ ] Avatar position: centrado mobile → left-8 desktop
- [ ] Stats: grid 2x2 mobile → fila desktop
- [ ] Grid: stack vertical mobile → 3 cols desktop
- [ ] Padding: px-4 mobile → px-8 desktop

### Accesibilidad
- [ ] Alt text en Avatar: "Foto de perfil de {nombre}"
- [ ] ARIA labels en social icon buttons
- [ ] Heading hierarchy (h1 nombre, h2 secciones)
- [ ] Focus visible ring en buttons y links
- [ ] Social links con target="_blank" + rel="noopener noreferrer"
- [ ] Loading states con aria-busy="true"
- [ ] Empty states con role="status"
- [ ] Icons decorativos con aria-hidden="true"

### Animaciones
- [ ] Button hover: scale-105 + shadow (150ms)
- [ ] Social icon hover: color change (150ms)
- [ ] Social link hover: underline + color (200ms)
- [ ] Page load: fadeIn animation (300ms)
- [ ] Skeleton pulse durante loading

### Integración
- [ ] useArtista hook con TanStack Query
- [ ] QUERY_KEYS.artistas.byId(id)
- [ ] API_ROUTES.artistas.byId(id)
- [ ] Manejo de error con ServiceResponse
- [ ] Tipos Artista desde shared/types

### Placeholders MVP
- [ ] Géneros musicales (hardcoded "Electronic", "Synthwave")
- [ ] Stats (0 campañas, 0 backers, €0)
- [ ] Social links (URLs de ejemplo)
- [ ] Follow button sin funcionalidad
- [ ] Campañas empty state
- [ ] Reviews empty state

---

## 15. Notas Importantes

### Alcance MVP
Este plan cubre **solo el perfil público de artista** en Landing. Los formularios de registro y creación de perfil están en el plan de Admin (`frontend-admin/ui-design.md`).

### Datos Placeholder
Para el MVP, varios elementos mostrarán datos de placeholder:
- **Géneros**: Hardcoded en el componente (no hay campo en DB)
- **Stats**: Todos en 0 (campañas, backers, recaudado)
- **Social links**: URLs de ejemplo hasta que se agreguen campos en DB
- **Campañas**: Empty state con mensaje "Próximamente"
- **Reviews**: Empty state (feature fuera de MVP)

### Feature Flags
Considerar feature flags para ocultar secciones no implementadas:
```typescript
const FEATURE_FLAGS = {
  showCampanias: false, // hasta feature crear-campania
  showReviews: false,   // fuera de MVP
  showFollow: false,    // fuera de MVP
};
```

### Integración Futura
Al implementar feature "crear-campania":
- Reemplazar `CampaniasPreview` empty state con query real
- Agregar cards de campaña con progress bar y stats
- Link a `/campanias/{id}` desde cada card

---

**Siguiente paso:** Ejecutar agente de implementación frontend-landing con este plan como referencia.
