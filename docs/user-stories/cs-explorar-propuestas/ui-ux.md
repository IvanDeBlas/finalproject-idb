# UI/UX: Explorar Necesidades y Enviar Propuestas

> **Feature:** cs-explorar-propuestas
> **Ultima actualizacion:** 2026-02-17

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Explorar campanias (referencia de grid y cards) | [WPR_2-Excplore-Campaigns.png](../../ui-images/WPR_2-Excplore-Campaigns.png) | Landing |
| Detalle campania (referencia de layout de detalle) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue los patrones de la landing publica existente (grid de cards, sidebars de filtros, modales de formulario).

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Listado de necesidades, cards de necesidad, detalle de necesidad | `references/templates/krowd/` |
| **Krowd** | Formulario de propuesta (modal/drawer), listado de propuestas | `references/templates/krowd/` |

**Nota:** Esta feature pertenece a la Landing publica (`src/web`, Vite + React, puerto 3000), NO al dashboard admin.

---

## Design Tokens

### Paleta de Colores

```css
:root {
  /* Background - Dark theme */
  --bg-primary: #1a1a2e;
  --bg-secondary: #16213e;
  --bg-card: #0f1729;
  --bg-card-hover: #1e2a42;
  --bg-overlay: rgba(0, 0, 0, 0.6);

  /* Primary - Gradient pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;

  /* Status generales */
  --status-success: #10b981;
  --status-error: #ef4444;
  --status-warning: #f59e0b;
  --status-info: #3b82f6;

  /* Estado Necesidad */
  --estado-abierta: #10b981;       /* Green - abierta para propuestas */

  /* Estado Propuesta */
  --estado-pendiente: #f59e0b;     /* Amber/Yellow */
  --estado-aceptada: #10b981;      /* Green */
  --estado-rechazada: #ef4444;     /* Red */
  --estado-retirada: #64748b;      /* Gray */

  /* Urgencia */
  --urgencia-badge-bg: #7f1d1d;
  --urgencia-badge-text: #fca5a5;
  --urgencia-badge-border: #991b1b;

  /* Info de presupuesto */
  --info-dentro-rango: #10b981;    /* Green - precio dentro del rango */
  --info-fuera-rango-alto: #f59e0b; /* Amber - precio por encima */
  --info-fuera-rango-bajo: #3b82f6; /* Blue - precio por debajo */

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Input */
  --input-bg: #0f1729;
  --input-bg-disabled: #1e293b;
  --input-border: #334155;
  --input-placeholder: #64748b;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Font Sizes */
  --text-xs: 0.75rem;      /* 12px */
  --text-sm: 0.875rem;     /* 14px */
  --text-base: 1rem;       /* 16px */
  --text-lg: 1.125rem;     /* 18px */
  --text-xl: 1.25rem;      /* 20px */
  --text-2xl: 1.5rem;      /* 24px */
  --text-3xl: 1.875rem;    /* 30px */
  --text-4xl: 2.25rem;     /* 36px */

  /* Font Weights */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Line Heights */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
  --leading-relaxed: 1.75;
}
```

### Espaciado

```css
:root {
  --space-1: 0.25rem;   /* 4px */
  --space-2: 0.5rem;    /* 8px */
  --space-3: 0.75rem;   /* 12px */
  --space-4: 1rem;      /* 16px */
  --space-5: 1.25rem;   /* 20px */
  --space-6: 1.5rem;    /* 24px */
  --space-8: 2rem;      /* 32px */
  --space-10: 2.5rem;   /* 40px */
  --space-12: 3rem;     /* 48px */
  --space-16: 4rem;     /* 64px */
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;   /* 6px */
  --radius-md: 0.5rem;     /* 8px */
  --radius-lg: 0.75rem;    /* 12px */
  --radius-xl: 1rem;       /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.3);
  --shadow-card-hover: 0 8px 24px rgba(0, 0, 0, 0.4);
}
```

---

## Pantalla 1: Explorar Necesidades (Listado)

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/necesidades`
**Template base:** `references/templates/krowd/` (listing pages)

### Layout

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR - Logo + Nav links + Login/Mi cuenta]                       │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  Explorar necesidades                                                │
│  Encuentra oportunidades de trabajo en la industria musical         │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │ [BARRA DE BUSQUEDA]                                             │ │
│  │ Buscar en titulo y descripcion...                [Buscar]       │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  ┌────────────────┐  ┌─────────────────────────────────────────────┐ │
│  │  FILTROS       │  │                                             │ │
│  │                │  │  Ordenar por: [Mas recientes v]   25 items  │ │
│  │  Tipo          │  │                                             │ │
│  │  [Produccion]  │  │  ┌──────────────┐  ┌──────────────┐        │ │
│  │  [Diseno]      │  │  │ [ABIERTA]    │  │ [ABIERTA]    │        │ │
│  │  [Marketing]   │  │  │              │  │              │        │ │
│  │  [Fotografia]  │  │  │ Mezcla pistas│  │ Diseno de    │        │ │
│  │  [Video]       │  │  │ para EP      │  │ portada EP   │        │ │
│  │  [Otro]        │  │  │              │  │              │        │ │
│  │                │  │  │ Buscamos un  │  │ Necesitamos  │        │ │
│  │  Modalidad     │  │  │ ingeniero... │  │ disenador... │        │ │
│  │  [Todos    v]  │  │  │              │  │              │        │ │
│  │                │  │  │ Los Rockeros │  │ Indie Band   │        │ │
│  │  Presupuesto   │  │  │ 150-800 EUR  │  │ 200-500 EUR  │        │ │
│  │  Min [____]    │  │  │ Remoto       │  │ Remoto       │        │ │
│  │  Max [____]    │  │  │              │  │              │        │ │
│  │                │  │  │ hace 2 dias  │  │ !URGENTE!    │        │ │
│  │  Pais          │  │  │ 3 propuestas │  │ hace 5 dias  │        │ │
│  │  [Todos    v]  │  │  │ 15 mar       │  │ 1 propuesta  │        │ │
│  │                │  │  └──────────────┘  └──────────────┘        │ │
│  │  Ciudad        │  │                                             │ │
│  │  [________]    │  │  ┌──────────────┐  ┌──────────────┐        │ │
│  │  (solo si pais)│  │  │  ...         │  │  ...         │        │ │
│  │                │  │  └──────────────┘  └──────────────┘        │ │
│  │  [Limpiar]     │  │                                             │ │
│  └────────────────┘  │  [< 1] [2] [3 >]  (paginacion 12/pag)     │ │
│                       └─────────────────────────────────────────────┘ │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  [NAVBAR hamburger]                 │
├─────────────────────────────────────┤
│  Explorar necesidades               │
│  [Buscar...]                        │
│                                     │
│  [Filtros v] (collapsed accordion)  │
│  Ordenar: [Mas recientes v]         │
│                                     │
│  ┌─────────────────────────────────┐ │
│  │ [ABIERTA]                       │ │
│  │ Mezcla pistas para EP           │ │
│  │ Buscamos un ingeniero de...     │ │
│  │ Los Rockeros | Remoto           │ │
│  │ 150-800 EUR | hace 2 dias       │ │
│  │ 3 propuestas | Limite: 15 mar   │ │
│  └─────────────────────────────────┘ │
│                                     │
│  ┌─────────────────────────────────┐ │
│  │ [ABIERTA] [!URGENTE!]           │ │
│  │ Diseno de portada EP            │ │
│  │ ...                             │ │
│  └─────────────────────────────────┘ │
│                                     │
│  [< 1] [2] [3 >]                   │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Navbar | `<nav>` (landing navbar existente) | Reutilizar navbar del landing |
| Page Hero | `<div>` | `bg-[#16213e] py-12 px-4` |
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-6` |
| Search Bar Container | `<div>` | `max-w-2xl mx-auto mb-8 flex gap-2` |
| Search Input | `<Input>` | `flex-1 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] h-12 text-base` |
| Search Button | `<Button>` | `h-12 px-6 bg-gradient-to-r from-pink-500 to-purple-600` |
| Page Layout | `<div>` | `max-w-7xl mx-auto px-4 py-8 flex gap-8` |
| Sidebar Filtros | `<aside>` | `w-64 flex-shrink-0 hidden lg:block` |
| Sidebar Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-5 sticky top-24` |
| Filtros Title | `<h2>` | `text-base font-semibold text-white mb-4` |
| Filtro Section Label | `<h3>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` |
| Tipo Chips Container | `<div>` | `flex flex-wrap gap-2 mb-5` |
| Tipo Chip (unselected) | `<button>` | `px-3 py-1 rounded-full text-sm border border-[#334155] text-[#94a3b8] hover:border-purple-500 hover:text-white transition-all` |
| Tipo Chip (selected) | `<button>` | `px-3 py-1 rounded-full text-sm bg-purple-600/20 border border-purple-500 text-white` |
| Modalidad Select | `<Select>` | shadcn/ui, `bg-[#0f1729] border-[#334155] text-white w-full mb-5` |
| Presupuesto Label | `<label>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` |
| Presupuesto Range Container | `<div>` | `flex gap-2 items-center mb-5` |
| Presupuesto Min Input | `<Input type="number">` | `bg-[#1a1a2e] border-[#334155] text-white text-sm w-full` |
| Presupuesto Max Input | `<Input type="number">` | `bg-[#1a1a2e] border-[#334155] text-white text-sm w-full` |
| Pais Select | `<Select>` | shadcn/ui, `bg-[#0f1729] border-[#334155] text-white w-full mb-3` |
| Ciudad Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white text-sm w-full mb-5` + hidden si no hay pais seleccionado |
| Limpiar Filtros Button | `<Button variant="ghost">` | `w-full text-[#94a3b8] hover:text-white hover:bg-[#1e2a42] mt-2` |
| Mobile Filtros Toggle | `<Button variant="outline">` | `lg:hidden mb-4 border-[#334155] text-white` con icono `SlidersHorizontal` |
| Mobile Filtros Accordion | `<Collapsible>` | shadcn/ui, `lg:hidden mb-4` |
| Content Area | `<main>` | `flex-1 min-w-0` |
| Content Header Row | `<div>` | `flex items-center justify-between mb-6` |
| Results Count | `<p>` | `text-sm text-[#94a3b8]` "25 necesidades encontradas" |
| Ordenar Select | `<Select>` | `w-48 bg-[#0f1729] border-[#334155] text-white` |
| Cards Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4 mb-8` |
| NecesidadCard | Ver "Componentes Reutilizables" | Click navega a detalle |
| Pagination | `<Pagination>` | shadcn/ui, centered, `mt-8` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | 6 skeleton cards en grid 2 cols (shimmer effect) |
| **Cargando con filtros** | Overlay semitransparente sobre cards, spinner centrado |
| **Populated** | Grid de NecesidadCard con paginacion |
| **Empty (sin filtros activos)** | Icono `Search` grande + "No hay necesidades abiertas en este momento" + "Vuelve pronto para ver nuevas oportunidades" |
| **Empty (con filtros activos)** | Icono `SearchX` + "No hay necesidades que coincidan" + "Intenta ampliar tus filtros" + boton [Limpiar filtros] |
| **Error de red** | Icono `AlertCircle` + "Error al cargar necesidades" + boton [Reintentar] |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Type en Search** | Debounce 300ms, actualiza listado, preserva otros filtros |
| **Click chip Tipo** | Toggle seleccion, actualiza listado inmediatamente (debounce 300ms) |
| **Select Modalidad** | Actualiza listado inmediatamente |
| **Input Presupuesto Min/Max** | Debounce 500ms antes de hacer request |
| **Select Pais** | Muestra input Ciudad, actualiza listado |
| **Type Ciudad** | Debounce 500ms |
| **Select Ordenar por** | Actualiza listado inmediatamente |
| **Click card** | Navega a `/crowdsourcing/necesidades/{id}` |
| **Click Limpiar filtros** | Resetea todos los filtros al estado inicial, actualiza listado |
| **Click pagina** | Carga pagina, scroll to top de la lista |

---

## Pantalla 2: Detalle de Necesidad

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/necesidades/{id}`
**Template base:** `references/templates/krowd/` (detail pages)

### Layout Desktop

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                            │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  [< Explorar necesidades]                                            │
│                                                                      │
│  ┌────────────────────────────────────┐  ┌───────────────────────┐  │
│  │ CONTENIDO PRINCIPAL                │  │ SIDEBAR ACCION        │  │
│  │                                    │  │                       │  │
│  │ ┌──────────────────────────────┐   │  │ ┌───────────────────┐ │  │
│  │ │ [POST-PRODUCCION]  [REMOTO]  │   │  │ │ Presupuesto       │ │  │
│  │ │                              │   │  │ │ 150 - 800 EUR     │ │  │
│  │ │ Mezcla de pistas para        │   │  │ │                   │ │  │
│  │ │ EP de 5 canciones            │   │  │ │ [Enviar propuesta]│ │  │
│  │ │                              │   │  │ │   (gradient btn)  │ │  │
│  │ │ ● Los Rockeros               │   │  │ │                   │ │  │
│  │ │   [mini avatar]              │   │  │ │ 3 propuestas      │ │  │
│  │ └──────────────────────────────┘   │  │ │                   │ │  │
│  │                                    │  │ │ Publicado hace    │ │  │
│  │ ┌──────────────────────────────┐   │  │ │ 2 dias            │ │  │
│  │ │ DESCRIPCION                  │   │  │ │                   │ │  │
│  │ │                              │   │  │ │ Limite propuestas │ │  │
│  │ │ Buscamos un ingeniero de     │   │  │ │ 15 mar 2026       │ │  │
│  │ │ mezcla experimentado para    │   │  │ │ (7 dias restantes)│ │  │
│  │ │ un EP de 5 canciones de      │   │  │ │                   │ │  │
│  │ │ rock alternativo. Necesitamos│   │  │ │ Inicio previsto   │ │  │
│  │ │ experiencia en...            │   │  │ │ 1 abr 2026        │ │  │
│  │ └──────────────────────────────┘   │  │ └───────────────────┘ │  │
│  │                                    │  │                       │  │
│  │ ┌──────────────────────────────┐   │  └───────────────────────┘  │
│  │ │ DETALLES                     │   │                             │
│  │ │                              │   │                             │
│  │ │ Tipo: Post-produccion        │   │                             │
│  │ │ Modalidad: Remoto            │   │                             │
│  │ │ Ubicacion: -                 │   │                             │
│  │ └──────────────────────────────┘   │                             │
│  └────────────────────────────────────┘                             │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  [NAVBAR]                           │
├─────────────────────────────────────┤
│  [< Explorar necesidades]           │
│                                     │
│  [POST-PRODUCCION]  [REMOTO]        │
│  Mezcla de pistas para EP           │
│  ● Los Rockeros [mini avatar]       │
│                                     │
│  Presupuesto: 150 - 800 EUR         │
│  Limite: 15 mar 2026 (7 dias)       │
│  3 propuestas recibidas             │
│                                     │
│  [Enviar propuesta - full width]    │
│                                     │
│  DESCRIPCION                        │
│  Buscamos un ingeniero de mezcla... │
│                                     │
│  DETALLES                           │
│  Tipo / Modalidad / Fechas          │
│                                     │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-[#94a3b8] hover:text-white mb-6` con icono `ArrowLeft` |
| Page Layout | `<div>` | `max-w-6xl mx-auto px-4 py-8 flex gap-8 items-start` |
| Main Content | `<div>` | `flex-1 min-w-0` |
| Header Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Badges Row | `<div>` | `flex items-center gap-2 mb-3` |
| Tipo Badge | `<Badge>` | `bg-purple-900/30 text-purple-300 border-purple-700` |
| Modalidad Badge | `<Badge variant="outline">` | `border-[#334155] text-[#94a3b8]` con icono (Wifi=Remoto, MapPin=Presencial, GitBranch=Hibrido) |
| Urgencia Badge | `<Badge>` | `bg-red-900/30 text-red-300 border-red-700 flex items-center gap-1` con icono `Clock` - solo visible si < 3 dias |
| Titulo | `<h1>` | `text-2xl md:text-3xl font-bold text-white mb-4 leading-tight` |
| Artist Row | `<div>` | `flex items-center gap-3` |
| Artist Avatar | `<Avatar>` | shadcn/ui, `w-8 h-8` |
| Artist Name | `<span>` | `text-sm font-medium text-[#94a3b8]` |
| Description Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Description Title | `<h2>` | `text-base font-semibold text-white mb-3` |
| Description Text | `<p>` | `text-base text-[#94a3b8] leading-relaxed whitespace-pre-line` |
| Details Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Details Title | `<h2>` | `text-base font-semibold text-white mb-4` |
| Details Grid | `<div>` | `grid grid-cols-2 gap-4` |
| Detail Item | `<div>` | `flex flex-col gap-1` |
| Detail Label | `<span>` | `text-xs text-[#64748b] uppercase tracking-wide` |
| Detail Value | `<span>` | `text-sm font-medium text-white` |
| Action Sidebar | `<aside>` | `w-80 flex-shrink-0 hidden md:block sticky top-24` |
| Action Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Budget Label | `<p>` | `text-xs text-[#64748b] uppercase tracking-wide mb-1` |
| Budget Value | `<p>` | `text-2xl font-bold text-white mb-4` |
| CTA Button (can propose) | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-12 text-base font-semibold mb-4` |
| CTA No Profile | `<div>` | `bg-[#1e2a42] border border-[#334155] rounded-lg p-4 mb-4 text-center` |
| CTA No Profile Text | `<p>` | `text-sm text-[#94a3b8] mb-3` |
| CTA No Profile Link | `<Button variant="outline">` | `w-full border-purple-500 text-purple-300 hover:bg-purple-900/20` |
| CTA Ya Propuso | `<div>` | `bg-green-900/20 border border-green-700 rounded-lg p-4 mb-4 flex items-center gap-2` |
| CTA Ya Propuso Text | `<p>` | `text-sm text-green-300` |
| CTA Es Propietario | `<div>` | `bg-[#1e2a42] border border-[#334155] rounded-lg p-4 mb-4 flex items-center gap-2` |
| CTA Es Propietario Text | `<p>` | `text-sm text-[#94a3b8]` |
| Sidebar Stats | `<div>` | `border-t border-[#334155] pt-4 space-y-3` |
| Stat Row | `<div>` | `flex items-center justify-between` |
| Stat Label | `<span>` | `text-xs text-[#64748b]` |
| Stat Value | `<span>` | `text-sm font-medium text-white` |
| Fecha Limite Row | `<div>` | Color warning (amber) si 3-7 dias, error (red) si < 3 dias |
| Mobile CTA (sticky bottom) | `<div>` | `fixed bottom-0 left-0 right-0 p-4 bg-[#0f1729] border-t border-[#334155] md:hidden z-50` |

### CTA States (Area de Accion)

| Estado | Contenido Visual |
|--------|-----------------|
| **Sin autenticar** | Boton "Inicia sesion para enviar propuesta" -> redirige a login con returnUrl |
| **Autenticado sin PerfilProfesional** | Bloque info: "Necesitas un perfil profesional para enviar propuestas" + boton "Crear perfil profesional" que navega al formulario |
| **Autenticado con PerfilProfesional** | Boton "Enviar propuesta" (gradient) que abre modal |
| **Ya envio propuesta** | Bloque verde: icono `CheckCircle` + "Ya enviaste una propuesta para esta necesidad" |
| **Es propietario de la necesidad** | Bloque info: icono `Info` + "Esta es tu necesidad" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton: header card, description card, details card, action card |
| **Loaded** | Contenido completo con CTA segun contexto del usuario |
| **Error 404** | Icono `AlertCircle` + "Necesidad no encontrada" + link "Volver al listado" |
| **Necesidad expirada** | Banner amber: "Esta necesidad ya no admite propuestas (fecha limite superada)" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "< Explorar necesidades"** | Navega a `/crowdsourcing/necesidades` (preserva filtros si es posible) |
| **Click "Enviar propuesta"** | Abre modal/drawer de formulario de propuesta (Pantalla 3) |
| **Click "Crear perfil profesional"** | Navega a formulario de perfil profesional |
| **Click "Inicia sesion"** | Navega a `/login?returnUrl=/crowdsourcing/necesidades/{id}` |
| **Badge urgencia** | Solo visible, no clickeable |

---

## Pantalla 3: Enviar Propuesta (Modal)

**Proyecto:** Landing (`src/web`)
**Contexto:** Modal overlay sobre el detalle de necesidad
**Template base:** shadcn/ui Dialog

### Layout

```
┌───────────────────────────────────────────────────────┐
│  Enviar propuesta                              [×]    │
│  Para: Mezcla de pistas para EP de 5 canciones        │
│  Presupuesto del artista: 150 - 800 EUR               │
├───────────────────────────────────────────────────────┤
│                                                       │
│  Precio propuesto *                 Moneda *          │
│  ┌──────────────────────────┐  ┌──────────────────┐   │
│  │  450                     │  │  EUR          v  │   │
│  └──────────────────────────┘  └──────────────────┘   │
│                                                       │
│  ┌───────────────────────────────────────────────┐    │
│  │ (i) Tu precio esta dentro del rango (150-800) │    │
│  └───────────────────────────────────────────────┘    │
│                                                       │
│  Tiempo estimado (dias)                               │
│  ┌──────────────────────────┐                         │
│  │  14                      │                         │
│  └──────────────────────────┘                         │
│  Opcional - cuantos dias necesitas para completar     │
│                                                       │
│  Mensaje de propuesta *                               │
│  ┌───────────────────────────────────────────────┐    │
│  │                                               │    │
│  │  Soy ingeniero de mezcla con 10 anos de      │    │
│  │  experiencia en rock alternativo...           │    │
│  │                                               │    │
│  │                                               │    │
│  └───────────────────────────────────────────────┘    │
│  245 / 2000 caracteres (min 20)                       │
│                                                       │
├───────────────────────────────────────────────────────┤
│  [Cancelar]                   [Enviar propuesta]      │
└───────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Container | `<Dialog>` | shadcn/ui, `open={isOpen} onOpenChange={onClose}` |
| Dialog Content | `<DialogContent>` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog Header | `<DialogHeader>` | `pb-4 border-b border-[#334155]` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` |
| Dialog Subtitle | `<DialogDescription>` | `text-sm text-[#94a3b8] mt-1` "Para: {necesidad.titulo}" |
| Budget Reference | `<p>` | `text-xs text-[#64748b] mt-1` "Presupuesto del artista: {min} - {max} {moneda}" |
| Form Body | `<div>` | `py-6 space-y-5` |
| Precio + Moneda Row | `<div>` | `grid grid-cols-2 gap-3` |
| Precio Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Required Mark | `<span>` | `text-red-400 ml-1` |
| Precio Input | `<Input type="number">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Moneda Label | `<Label>` | igual al precio label |
| Moneda Select | `<Select>` | shadcn/ui, `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Info Precio (dentro rango) | `<div>` | `bg-green-900/20 border border-green-700/50 rounded-md p-3 flex items-center gap-2` |
| Info Precio Icon (dentro) | `<CheckCircle>` | Lucide, `w-4 h-4 text-green-400 flex-shrink-0` |
| Info Precio Text (dentro) | `<p>` | `text-sm text-green-300` "Tu precio esta dentro del rango (150-800 EUR)" |
| Info Precio (por encima) | `<div>` | `bg-amber-900/20 border border-amber-700/50 rounded-md p-3 flex items-center gap-2` |
| Info Precio (por encima) Text | `<p>` | `text-sm text-amber-300` "Tu precio esta por encima del presupuesto indicado (max 800 EUR)" |
| Info Precio (por debajo) | `<div>` | `bg-blue-900/20 border border-blue-700/50 rounded-md p-3 flex items-center gap-2` |
| Info Precio (por debajo) Text | `<p>` | `text-sm text-blue-300` "Tu precio esta por debajo del presupuesto indicado (min 150 EUR)" |
| Dias Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Dias Input | `<Input type="number">` | `bg-[#1a1a2e] border-[#334155] text-white h-11 w-40` |
| Dias Hint | `<p>` | `text-xs text-[#64748b] mt-1` "Opcional - cuantos dias necesitas para completar" |
| Mensaje Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Mensaje Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[140px] resize-none` |
| Mensaje Counter | `<div>` | `flex justify-between mt-1` |
| Counter Text | `<span>` | `text-xs text-[#64748b]` "{count} / 2000 caracteres (min 20)" |
| Counter Text (at limit) | `<span>` | `text-xs text-red-400` |
| Error Message | `<p>` | `text-sm text-red-400 mt-1` con `role="alert"` |
| Dialog Footer | `<DialogFooter>` | `pt-4 border-t border-[#334155] flex justify-between` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50` |
| Submit Button (loading) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4 mr-2" />` + "Enviando..." |

### Logica de Info Precio

La nota informativa se calcula en tiempo real al cambiar el precio:

```
si precio > 0 y presupuestoMax definido:
  si precio >= presupuestoMin y precio <= presupuestoMax:
    -> mostrar "dentro del rango" (green)
  si precio > presupuestoMax:
    -> mostrar "por encima del presupuesto indicado" (amber)
  si precio < presupuestoMin:
    -> mostrar "por debajo del presupuesto indicado" (blue)
si precio = 0 o vacio:
  -> ocultar nota informativa
```

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Form vacio, todos inputs en estado default |
| **Typing Precio** | Nota informativa actualizada en tiempo real |
| **Typing Mensaje** | Contador de caracteres actualizado en tiempo real |
| **Validation Error** | Border rojo en inputs invalidos, mensaje de error debajo, submit disabled |
| **Submitting** | Submit button con spinner + "Enviando...", todos los inputs disabled |
| **Success** | Dialog se cierra, toast success, usuario permanece en la pagina de detalle |
| **Error API** | Toast error especifico, dialog permanece abierto, form re-habilitado |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Precio propuesto** | Obligatorio, > 0, numerico | "El precio propuesto debe ser mayor a 0" |
| **Moneda** | Obligatorio, debe existir en maestras | "Selecciona la moneda" |
| **Dias estimados** | Opcional, si se ingresa: > 0 y <= 365 | "Los dias estimados deben estar entre 1 y 365" |
| **Mensaje propuesta** | Obligatorio, min 20 chars, max 2000 | "El mensaje debe tener al menos 20 caracteres" / "Maximo 2000 caracteres" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click [×] o Cancelar** | Cierra dialog sin cambios, form se resetea |
| **Press Escape** | Cierra dialog |
| **Change Precio** | Actualiza nota informativa de rango en tiempo real |
| **Blur input** | Valida campo, muestra error si invalido |
| **Type en Mensaje** | Actualiza contador de caracteres |
| **Click "Enviar propuesta"** | Valida form completo, POST a API, cierra dialog si exitoso, muestra toast |
| **Error API "Ya tienes propuesta"** | Toast error, dialog cierra, pagina actualiza CTA a "Ya enviaste una propuesta" |
| **Error API 403** | Toast error descriptivo, dialog cierra |

---

## Pantalla 4: Mis Propuestas (Listado)

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/mis-propuestas`
**Template base:** `references/templates/krowd/` (listing pages)

**Requisito:** Usuario debe estar autenticado. Si no lo esta, redirigir a `/login?returnUrl=/crowdsourcing/mis-propuestas`.

### Layout

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                            │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  Mis propuestas                                                      │
│  Seguimiento de las propuestas que has enviado a artistas           │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │ Filtrar por estado:                                             │ │
│  │ [Todas] [Pendiente] [Aceptada] [Rechazada] [Retirada]           │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  8 propuestas  (resultado del filtro)                                │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │                                                                 │ │
│  │  Mezcla de pistas para EP de 5 canciones     [PENDIENTE]       │ │
│  │  Los Rockeros                                                   │ │
│  │  Mi precio: 450 EUR  |  Enviada: 20 feb 2026                   │ │
│  │                              [Retirar propuesta]               │ │
│  │                                                                 │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │                                                                 │ │
│  │  Diseno de portada para EP                   [ACEPTADA]        │ │
│  │  Indie Band                                                     │ │
│  │  Mi precio: 300 EUR  |  Enviada: 15 feb | Resp: 18 feb 2026    │ │
│  │                                          [Ver acuerdo ->]      │ │
│  │                                                                 │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────────┐ │
│  │                                                                 │ │
│  │  Video clip para single                      [RECHAZADA]       │ │
│  │  Rock Masters                                                   │ │
│  │  Mi precio: 1200 EUR  |  Enviada: 10 feb | Resp: 12 feb 2026   │ │
│  │                        (sin acciones - solo lectura)           │ │
│  │                                                                 │ │
│  └─────────────────────────────────────────────────────────────────┘ │
│                                                                      │
│  [< 1] [2] [Siguiente >]                                            │
└──────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Container | `<div>` | `max-w-4xl mx-auto px-4 py-8` |
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-base text-[#94a3b8] mb-6` |
| Filter Bar | `<div>` | `flex flex-wrap gap-2 mb-6 p-4 bg-[#0f1729] border border-[#334155] rounded-lg` |
| Filter Label | `<span>` | `text-sm text-[#94a3b8] mr-2 self-center` "Filtrar por estado:" |
| Estado Filter Chip (Todas) | `<button>` | Variant "all": chip seleccionado o no (ver EstadoFilterChip) |
| Estado Filter Chip (Pendiente) | `<button>` | Amber cuando seleccionado |
| Estado Filter Chip (Aceptada) | `<button>` | Green cuando seleccionado |
| Estado Filter Chip (Rechazada) | `<button>` | Red cuando seleccionado |
| Estado Filter Chip (Retirada) | `<button>` | Gray cuando seleccionado |
| Results Count | `<p>` | `text-sm text-[#94a3b8] mb-4` "{N} propuestas" |
| Cards List | `<div>` | `space-y-4 mb-8` |
| PropuestaCard | Ver "Componentes Reutilizables" | Cada propuesta |
| Pagination | `<Pagination>` | shadcn/ui, centered |

#### EstadoFilterChip styles

```
Todas (unselected): bg-[#1e2a42] border-[#334155] text-[#94a3b8]
Todas (selected):   bg-white/10 border-white text-white

Pendiente (selected): bg-amber-900/30 border-amber-600 text-amber-300
Aceptada (selected):  bg-green-900/30 border-green-600 text-green-300
Rechazada (selected): bg-red-900/30 border-red-600 text-red-300
Retirada (selected):  bg-gray-900/30 border-gray-600 text-gray-300

Todos los chips: px-4 py-1.5 rounded-full text-sm border transition-all cursor-pointer
```

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | 4 skeleton cards (shimmer) |
| **Empty (todas)** | Icono `FileText` + "Aun no has enviado ninguna propuesta" + boton [Explorar necesidades] |
| **Empty (filtro activo)** | Icono `SearchX` + "No tienes propuestas en estado {estado}" |
| **Populated** | Lista de PropuestaCard |
| **Error de red** | Icono `AlertCircle` + "Error al cargar tus propuestas" + boton [Reintentar] |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click chip estado** | Filtra lista, actualiza URL query param `?estado=pendiente` |
| **Click "Retirar propuesta"** | Abre dialog de confirmacion (Pantalla 5) |
| **Click "Ver acuerdo"** | Navega a detalle del acuerdo (US-CS-04) |
| **Click en titulo de necesidad** | Navega a `/crowdsourcing/necesidades/{necesidadId}` |
| **Click pagina** | Carga pagina correspondiente |

---

## Pantalla 5: Confirmar Retirar Propuesta (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre Mis Propuestas
**Template base:** shadcn/ui Dialog

### Layout

```
┌──────────────────────────────────────────────────┐
│  Retirar propuesta                          [×]  │
├──────────────────────────────────────────────────┤
│                                                  │
│  ┌──────────────────────────────────────────┐    │
│  │  ⚠  Esta accion no se puede deshacer    │    │
│  └──────────────────────────────────────────┘    │
│                                                  │
│  Vas a retirar tu propuesta para:                │
│  "Mezcla de pistas para EP de 5 canciones"       │
│                                                  │
│  Una vez retirada, el artista ya no podra        │
│  ver ni aceptar tu propuesta. Podras enviar      │
│  una nueva propuesta si lo deseas.               │
│                                                  │
├──────────────────────────────────────────────────┤
│  [Cancelar]          [Confirmar retirada]        │
└──────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Container | `<Dialog>` | shadcn/ui |
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155]` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Retirar propuesta" |
| Warning Alert | `<Alert>` | `bg-red-900/20 border-red-700/50 mb-4` |
| Warning Icon | `<AlertTriangle>` | Lucide, `w-5 h-5 text-red-400 flex-shrink-0` |
| Warning Text | `<AlertDescription>` | `text-sm text-red-300` "Esta accion no se puede deshacer" |
| Dialog Description | `<div>` | `space-y-3` |
| Description Lead | `<p>` | `text-sm text-[#94a3b8]` "Vas a retirar tu propuesta para:" |
| Necesidad Title | `<p>` | `text-sm font-semibold text-white` entre comillas |
| Explanation | `<p>` | `text-sm text-[#94a3b8]` descripcion de consecuencias |
| Dialog Footer | `<DialogFooter>` | `flex justify-between gap-3` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Confirmar Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700 text-white` |
| Confirmar (loading) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4 mr-2" />` + "Retirando..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Dialog abierto, botones activos |
| **Submitting** | "Confirmar retirada" con spinner + "Retirando...", disabled, cancelar tambien disabled |
| **Success** | Dialog cierra, toast success: "Propuesta retirada correctamente", card actualiza badge a [RETIRADA], boton Retirar desaparece |
| **Error** | Toast error, dialog cierra, propuesta permanece Pendiente |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "Cancelar"** | Cierra dialog sin cambios |
| **Click [×]** | Cierra dialog sin cambios |
| **Press Escape** | Cierra dialog |
| **Click "Confirmar retirada"** | PATCH a API, cierra dialog, actualiza lista de propuestas |

---

## Componentes Reutilizables

### NecesidadCard (Listado)

**Archivo sugerido:** `src/web/src/features/crowdsourcing/components/NecesidadCard.tsx`

**Props:**
```typescript
interface NecesidadCardProps {
  necesidad: {
    id: string;
    titulo: string;
    descripcion: string;               // truncar a 150 chars
    tipoNecesidadNombre: string;
    presupuestoMin: number;
    presupuestoMax: number;
    monedaNombre: string;
    modalidadTrabajoNombre: "Presencial" | "Remoto" | "Hibrido";
    modalidadTrabajoIcono: string;     // nombre del icono Lucide
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    artistaNombre: string;
    fechaRelativa: string;             // "hace 2 dias" (calculado en backend)
    fechaLimitePropuestas: string;     // ISO date string
    esUrgente: boolean;                // < 3 dias para deadline
    numeroPropuestas: number;
  };
  onClick?: () => void;
}
```

**Layout interno:**

```
┌──────────────────────────────────────────────────────┐
│  [TIPO-BADGE]  [MODALIDAD-BADGE]      [!URGENTE!]    │
│                                                      │
│  Titulo de la necesidad                              │
│                                                      │
│  Descripcion truncada a 150 chars con "..."          │
│                                                      │
│  [ArtistaNombre]  ·  [icono] Presupuesto range       │
│                                                      │
│  [Reloj] fecha relativa  ·  [Users] N propuestas     │
│  [Calendar] Limite: {fecha}                          │
└──────────────────────────────────────────────────────┘
```

**Estilos:**
```
Card base: bg-[#0f1729] border border-[#334155] rounded-xl p-5
Card hover: bg-[#1e2a42] border-purple-500/50 shadow-[0_8px_24px_rgba(0,0,0,0.4)] scale-[1.01]
Transition: transition-all duration-200 ease-out cursor-pointer
```

**Estados:**
- `esUrgente=true`: Badge `[!URGENTE!]` rojo `bg-red-900/30 text-red-300 border-red-700` con icono `Clock`
- `numeroPropuestas=0`: Texto "Sin propuestas aun" en gris
- `numeroPropuestas>0`: "{N} propuesta(s)" con icono `Users`
- Fecha limite: texto normal si > 7 dias, amber si 3-7 dias, rojo si < 3 dias (siempre, ademas del badge urgente)

---

### PropuestaCard (Mis Propuestas)

**Archivo sugerido:** `src/web/src/features/crowdsourcing/components/PropuestaCard.tsx`

**Props:**
```typescript
interface PropuestaCardProps {
  propuesta: {
    id: string;
    necesidadId: string;
    necesidadTitulo: string;
    artistaNombre: string;
    precioPropuesto: number;
    monedaNombre: string;
    estadoPropuestaNombre: "Pendiente" | "Aceptada" | "Rechazada" | "Retirada";
    fechaCreacion: string;             // ISO date string
    fechaActualizacion?: string;       // ISO date string, si hay respuesta
    acuerdoId?: string;               // si estado=Aceptada
  };
  onRetirar?: (propuestaId: string) => void;
}
```

**Layout interno:**

```
┌──────────────────────────────────────────────────────┐
│  Titulo de la necesidad               [ESTADO-BADGE] │
│  Artista: {artistaNombre}                            │
│                                                      │
│  Mi precio: {precio} {moneda}                        │
│  Enviada: {fechaCreacion formateada}                 │
│  Respuesta: {fechaActualizacion} (si existe)         │
│                                                      │
│                          [Accion segun estado]       │
└──────────────────────────────────────────────────────┘
```

**Estilos:**
```
Card base: bg-[#0f1729] border border-[#334155] rounded-xl p-5
Sin hover activo (no clickeable directamente, acciones via botones)
```

**Actions por Estado:**

| Estado | Accion disponible |
|--------|-----------------|
| **Pendiente** | Boton `<Button variant="outline" size="sm">` con `text-red-400 border-red-400/50 hover:bg-red-900/20` "Retirar propuesta" |
| **Aceptada** | Boton `<Button variant="default" size="sm">` gradient "Ver acuerdo" con icono `ArrowRight` |
| **Rechazada** | Sin acciones. Solo lectura |
| **Retirada** | Sin acciones. Solo lectura |

**Estado Badges:**

| Estado | Estilos |
|--------|---------|
| Pendiente | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aceptada | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazada | `bg-red-900/30 border-red-600 text-red-300` |
| Retirada | `bg-gray-900/30 border-gray-600 text-gray-400` |

---

### NecesidadCardSkeleton

**Uso:** Loading state en listado de necesidades

```tsx
<Card className="bg-[#0f1729] border-[#334155] p-5">
  <div className="flex items-center gap-2 mb-3">
    <Skeleton className="h-5 w-28 rounded-full" />  {/* Tipo badge */}
    <Skeleton className="h-5 w-20 rounded-full" />  {/* Modalidad badge */}
  </div>
  <Skeleton className="h-6 w-4/5 mb-3" />          {/* Titulo */}
  <Skeleton className="h-4 w-full mb-1" />          {/* Descripcion line 1 */}
  <Skeleton className="h-4 w-3/4 mb-4" />          {/* Descripcion line 2 */}
  <div className="flex justify-between">
    <Skeleton className="h-4 w-32" />               {/* Artista + precio */}
    <Skeleton className="h-4 w-24" />               {/* Stats */}
  </div>
</Card>
```

---

### PropuestaCardSkeleton

**Uso:** Loading state en listado de mis propuestas

```tsx
<Card className="bg-[#0f1729] border-[#334155] p-5">
  <div className="flex items-start justify-between mb-3">
    <div className="flex-1">
      <Skeleton className="h-5 w-3/4 mb-2" />      {/* Titulo necesidad */}
      <Skeleton className="h-4 w-40" />             {/* Artista */}
    </div>
    <Skeleton className="h-6 w-24 rounded-full" /> {/* Badge estado */}
  </div>
  <div className="flex gap-4 mb-3">
    <Skeleton className="h-4 w-28" />               {/* Precio */}
    <Skeleton className="h-4 w-36" />               {/* Fecha */}
  </div>
  <div className="flex justify-end">
    <Skeleton className="h-8 w-32" />               {/* Boton accion */}
  </div>
</Card>
```

---

## Estados UI Globales

### Loading Skeletons

| Pantalla | Cantidad | Descripcion |
|----------|----------|-------------|
| Listado Necesidades | 6 skeletons | Grid 2 cols, NecesidadCardSkeleton |
| Detalle Necesidad | 3 cards skeleton | Header + Description + Details |
| Mis Propuestas | 4 skeletons | Lista vertical, PropuestaCardSkeleton |

### Error States

| Tipo | Icono | Titulo | Descripcion | Accion |
|------|-------|--------|-------------|--------|
| **Red error** | `WifiOff` | "Error de conexion" | "No se pudo cargar el contenido" | [Reintentar] |
| **Not Found** | `SearchX` | "No encontrado" | "La necesidad que buscas no existe o ya no esta disponible" | [Volver al listado] |
| **Forbidden** | `Lock` | "Acceso restringido" | "Necesitas iniciar sesion para ver este contenido" | [Iniciar sesion] |

### Empty States

| Pantalla | Icono | Titulo | Descripcion | Accion |
|----------|-------|--------|-------------|--------|
| Listado sin filtros | `Inbox` | "Sin necesidades abiertas" | "Vuelve pronto para ver nuevas oportunidades de trabajo" | - |
| Listado con filtros | `SearchX` | "Sin resultados" | "No hay necesidades que coincidan con tus filtros. Intenta ampliar la busqueda." | [Limpiar filtros] |
| Mis propuestas sin ninguna | `FileText` | "Sin propuestas enviadas" | "Aun no has enviado propuestas a ningun artista" | [Explorar necesidades] |
| Mis propuestas con filtro | `Filter` | "Sin propuestas {estado}" | "No tienes propuestas en este estado" | - |

### Toast Notifications

| Accion | Tipo | Mensaje |
|--------|------|---------|
| **Enviar propuesta - success** | Success (verde) | "Propuesta enviada correctamente. El artista sera notificado." |
| **Enviar propuesta - ya enviada** | Error (rojo) | "Ya enviaste una propuesta para esta necesidad" |
| **Enviar propuesta - sin perfil** | Warning (amber) | "Necesitas un perfil profesional para enviar propuestas" |
| **Enviar propuesta - propietario** | Warning (amber) | "No puedes enviar propuesta a tu propia necesidad" |
| **Enviar propuesta - error general** | Error (rojo) | "Error al enviar la propuesta. Intentalo de nuevo." |
| **Retirar propuesta - success** | Success (verde) | "Propuesta retirada correctamente" |
| **Retirar propuesta - error** | Error (rojo) | "Error al retirar la propuesta. Intentalo de nuevo." |
| **Error carga de datos** | Error (rojo) | "Error al cargar los datos. Intentalo de nuevo." |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios Principales |
|------------|-------|---------------------|
| **Mobile** | < 640px | Layout 1 columna, filtros en accordion colapsable, sidebar accion sticky bottom, cards full width |
| **Tablet** | 640px - 1024px | Grid 2 columnas para cards, filtros en sidebar visible, sidebar accion a la derecha |
| **Desktop** | > 1024px | Grid 2 columnas para cards, sidebar filtros fijo izquierda, sidebar accion fijo derecha, hover effects |

### Mobile (< 640px)

**Listado de Necesidades:**
- Filtros colapsados en accordion con boton "[Filtros] SlidersHorizontal" arriba
- Dentro del accordion: tipo chips en grid 2x, modalidad select, presupuesto, pais, ciudad, limpiar
- Search input full width encima del accordion
- Ordenar por: select full width debajo del accordion
- Cards en 1 columna, full width
- Paginacion con numeros reducidos (prev / 1 ... 3 / next)

**Detalle de Necesidad:**
- Back link arriba
- Badges + titulo + artista en seccion header
- CTA area (presupuesto + boton) inmediatamente debajo del header
- Boton "Enviar propuesta": fixed bottom bar (`fixed bottom-0 left-0 right-0 p-4`) para visibilidad
- Description card
- Details card (1 columna, no grid)

**Mis Propuestas:**
- Filter chips horizontales con scroll horizontal si no caben (`overflow-x-auto flex-nowrap`)
- Cards full width
- Dentro de card: acciones alineadas a la derecha

### Tablet (640px - 1024px)

**Listado:**
- Sidebar filtros visible (colapsado o como panel lateral delgado)
- Cards en grid 2 columnas

**Detalle:**
- Sidebar accion visible a la derecha (no fixed bottom)
- Layout 2 columnas (contenido | sidebar)

### Desktop (> 1024px)

**Listado:**
- Sidebar filtros fijo izquierda (w-64)
- Cards en grid 2 columnas en el area de contenido
- Hover effects activos

**Detalle:**
- Layout 2 columnas: contenido principal (flex-1) + sidebar accion (w-80)
- Sidebar accion sticky: `sticky top-24`

---

## Interacciones y Animaciones

| Elemento | Animacion | Duracion | Easing |
|----------|-----------|----------|--------|
| **NecesidadCard hover** | Background + border color + scale(1.01) + shadow | 200ms | ease-out |
| **Chip de filtro toggle** | Background + border + text color | 150ms | ease |
| **Dialog open** | Backdrop fade-in + content scale 0.95 -> 1 + translateY(4px -> 0) | 200ms | ease-out |
| **Dialog close** | Backdrop fade-out + content scale 1 -> 0.95 + opacity fade | 150ms | ease-in |
| **Button hover (primary)** | Gradient shift + scale(1.02) | 150ms | ease |
| **Button loading** | Spinner rotate 360deg infinite | 1000ms | linear |
| **Input focus** | Border color to purple + box-shadow glow | 200ms | ease |
| **Toast appear** | Slide-in from right + fade | 250ms | ease-out |
| **Toast disappear** | Slide-out to right + fade | 200ms | ease-in |
| **Skeleton shimmer** | Background position shift | 1500ms | ease-in-out infinite |
| **Badge urgencia** | Pulse suave (opacity 1 -> 0.7 -> 1) | 2000ms | ease-in-out infinite |
| **Info precio (nota range)** | Fade in | 200ms | ease |
| **Character counter color** | Text color gray -> amber (80%) -> red (100%) | 200ms | ease |
| **Page transition** | Fade in | 200ms | ease |
| **Filter accordion open/close** | Height 0 -> auto | 300ms | ease |

### Shake Animation (Error de Validacion)

```css
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}

.error-shake {
  animation: shake 400ms ease;
}
```

### Badge Urgencia Pulse

```css
@keyframes urgency-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.urgency-badge {
  animation: urgency-pulse 2000ms ease-in-out infinite;
}
```

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste minimo** | 4.5:1 para texto normal. White (#fff) sobre bg-primary (#1a1a2e) = 15.8:1. Amber text (#f59e0b) sobre fondo oscuro (#0f1729) verificar >= 4.5:1 |
| **Focus visible** | Ring de 2px solid `#a855f7` con offset de 2px en todos los elementos interactivos |
| **Labels en inputs** | Todos los inputs tienen `<Label>` con `htmlFor` apuntando al `id` del input |
| **Required fields** | Marcados con "*" visual (span aria-hidden) y `aria-required="true"` en el input |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="assertive"` |
| **Loading states** | `aria-busy="true"` en botones durante submit. Skeletons con `role="status" aria-label="Cargando..."` |
| **Cards navegables** | NecesidadCard: `tabindex="0"`, `role="article"`, Enter/Space para navegar al detalle |
| **Estado badges** | `aria-label="Estado: {estado}"` para screen readers |
| **Dialog** | `role="dialog"`, `aria-modal="true"`, `aria-labelledby`, `aria-describedby`, focus trap dentro del dialog, Escape para cerrar |
| **Fecha limite** | `aria-label="Fecha limite: {fecha}. {N} dias restantes."` cuando es urgente |
| **Contadores de caracteres** | `aria-live="polite"` para actualizaciones del contador |
| **Botones de accion** | `aria-label` descriptivo cuando el texto solo es un icono |
| **Filtros** | Chips de tipo con `aria-pressed="true/false"` para estado selected |
| **Paginacion** | `aria-label="Paginacion"` en el nav, `aria-current="page"` en la pagina activa |
| **Empty states** | `role="status"` en el contenedor del empty state |
| **Imagenes** | Avatares con `alt="{nombre} avatar"` |

### ARIA Labels Ejemplos

```tsx
// NecesidadCard
<Card
  tabIndex={0}
  role="article"
  aria-label={`${necesidad.titulo}. ${necesidad.tipoNecesidadNombre}. Modalidad: ${necesidad.modalidadTrabajoNombre}. Presupuesto ${necesidad.presupuestoMin} a ${necesidad.presupuestoMax} ${necesidad.monedaNombre}. ${necesidad.numeroPropuestas} propuestas.`}
  onClick={onClick}
  onKeyDown={(e) => (e.key === 'Enter' || e.key === ' ') && onClick?.()}
>

// Filter chip
<button
  aria-pressed={isSelected}
  aria-label={`Filtrar por tipo: ${tipoNombre}`}
  onClick={() => toggleTipo(tipoId)}
>

// Badge urgencia
<Badge aria-label={`Urgente: menos de 3 dias para el cierre`}>
  <Clock className="w-3 h-3 mr-1" aria-hidden="true" />
  URGENTE
</Badge>

// Loading button
<Button
  disabled={isPending}
  aria-busy={isPending}
  aria-label={isPending ? "Enviando propuesta..." : "Enviar propuesta"}
>
  {isPending && <Loader2 className="animate-spin w-4 h-4 mr-2" aria-hidden="true" />}
  {isPending ? "Enviando..." : "Enviar propuesta"}
</Button>

// Dialog confirmacion
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent
    role="dialog"
    aria-modal="true"
    aria-labelledby="retirar-dialog-title"
    aria-describedby="retirar-dialog-desc"
  >
    <DialogTitle id="retirar-dialog-title">Retirar propuesta</DialogTitle>
    <DialogDescription id="retirar-dialog-desc">
      Esta accion no se puede deshacer. La propuesta para "{necesidadTitulo}" sera retirada.
    </DialogDescription>
  </DialogContent>
</Dialog>

// Skeleton loading
<div role="status" aria-label="Cargando necesidades...">
  {Array.from({ length: 6 }).map((_, i) => (
    <NecesidadCardSkeleton key={i} />
  ))}
</div>

// Contador de caracteres
<span
  aria-live="polite"
  aria-atomic="true"
  className="text-xs text-[#64748b]"
>
  {count} / 2000 caracteres
</span>
```

---

## Notas de Implementacion

### Data Fetching (TanStack Query)

```typescript
// src/web/src/features/crowdsourcing/hooks/useNecesidades.ts
export const useNecesidades = (filtros: NecesidadFiltros) => {
  return useQuery({
    queryKey: ['necesidades', filtros],
    queryFn: () => crowdsourcingService.getNecesidades(filtros),
    staleTime: 30_000,   // 30 segundos
    placeholderData: keepPreviousData, // mantiene datos anteriores durante loading de filtros
  });
};

// src/web/src/features/crowdsourcing/hooks/useMisPropuestas.ts
export const useMisPropuestas = (estado?: string) => {
  return useQuery({
    queryKey: ['mis-propuestas', estado],
    queryFn: () => crowdsourcingService.getMisPropuestas(estado),
    staleTime: 30_000,
  });
};

// src/web/src/features/crowdsourcing/hooks/useEnviarPropuesta.ts
export const useEnviarPropuesta = (necesidadId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: EnviarPropuestaDto) =>
      crowdsourcingService.enviarPropuesta(necesidadId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['necesidades'] });
      queryClient.invalidateQueries({ queryKey: ['mis-propuestas'] });
      queryClient.invalidateQueries({ queryKey: ['necesidad-detalle', necesidadId] });
    },
  });
};

// src/web/src/features/crowdsourcing/hooks/useRetirarPropuesta.ts
export const useRetirarPropuesta = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (propuestaId: string) =>
      crowdsourcingService.retirarPropuesta(propuestaId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mis-propuestas'] });
    },
  });
};
```

### Debounce para Busqueda y Filtros

```typescript
// Busqueda por texto: debounce 300ms
const [searchTerm, setSearchTerm] = useState('');
const debouncedSearch = useDebounce(searchTerm, 300);

// Presupuesto min/max: debounce 500ms (input numerico, mas lento)
const debouncedPresupuestoMin = useDebounce(presupuestoMin, 500);
const debouncedPresupuestoMax = useDebounce(presupuestoMax, 500);

// Ciudad: debounce 500ms
const debouncedCiudad = useDebounce(ciudad, 500);
```

### Calculo de Urgencia (Frontend)

```typescript
// El campo esUrgente viene del backend, pero la urgencia de fecha limite
// para estilos de texto se calcula tambien en el frontend
const getDiasRestantes = (fechaLimite: string): number => {
  const hoy = new Date();
  const limite = new Date(fechaLimite);
  return Math.ceil((limite.getTime() - hoy.getTime()) / (1000 * 60 * 60 * 24));
};

const getFechaLimiteStyle = (diasRestantes: number) => {
  if (diasRestantes < 3) return 'text-red-400';
  if (diasRestantes < 7) return 'text-amber-400';
  return 'text-[#94a3b8]';
};
```

### URL State para Filtros

Los filtros del listado de necesidades se sincronizan con la URL para permitir compartir busquedas:

```
/crowdsourcing/necesidades?tipo=produccion,diseno&modalidad=Remoto&page=2
/crowdsourcing/mis-propuestas?estado=pendiente
```

Usar `useSearchParams` (React Router) para leer y escribir los parametros de filtro.

### Formulario de Propuesta (React Hook Form + Zod)

```typescript
const propuestaSchema = z.object({
  precioPropuesto: z.number({ required_error: "El precio es obligatorio" })
    .positive("El precio debe ser mayor a 0"),
  monedaId: z.number({ required_error: "Selecciona la moneda" }),
  diasEstimados: z.number().int().min(1).max(365).optional(),
  mensajePropuesta: z.string()
    .min(20, "El mensaje debe tener al menos 20 caracteres")
    .max(2000, "Maximo 2000 caracteres"),
});
```

### Iconos por Modalidad (Lucide)

| Modalidad | Icono Lucide |
|-----------|-------------|
| Remoto | `Wifi` |
| Presencial | `MapPin` |
| Hibrido | `GitBranch` |

---

## Checklist UI/UX

### Pantalla 1: Explorar Necesidades (Listado)
- [ ] Navbar del landing reutilizado
- [ ] Hero section: titulo + subtitulo + search bar
- [ ] Sidebar de filtros (desktop): tipo chips, modalidad, presupuesto range, pais, ciudad, limpiar
- [ ] Filtros en accordion colapsable (mobile)
- [ ] Ordenar por select en content header
- [ ] Results count actualizado
- [ ] Grid 2 columnas (md+), 1 columna (mobile)
- [ ] NecesidadCard con todos los datos: tipo badge, modalidad badge, urgencia badge, titulo, descripcion truncada, artista, presupuesto, modalidad, fecha relativa, propuestas count, fecha limite
- [ ] Hover effect en NecesidadCard (bg + border + scale + shadow)
- [ ] Badge urgente (rojo + pulse) si < 3 dias
- [ ] Fecha limite con color segun urgencia (normal/amber/rojo)
- [ ] Empty state sin filtros (icono Inbox + mensaje)
- [ ] Empty state con filtros (icono SearchX + boton limpiar)
- [ ] Loading: 6 skeleton cards con shimmer
- [ ] Error de red con boton reintentar
- [ ] Paginacion (12 items/pagina)
- [ ] Debounce 300ms en search, 500ms en presupuesto/ciudad
- [ ] URL sync de filtros (useSearchParams)
- [ ] Click en card navega a detalle
- [ ] ARIA labels en cards (role="article", aria-label descriptivo)
- [ ] Chips con aria-pressed
- [ ] Focus ring visible en todos los interactivos

### Pantalla 2: Detalle de Necesidad
- [ ] Back link con icono ArrowLeft
- [ ] Layout 2 columnas desktop (contenido + sidebar)
- [ ] Layout 1 columna mobile con CTA sticky bottom
- [ ] Header: tipo badge, modalidad badge, urgencia badge (si aplica), titulo, artista con avatar mini
- [ ] Description card con texto completo
- [ ] Details card: tipo, modalidad, ubicacion, fechas formateadas
- [ ] Sidebar accion con presupuesto destacado y estado del dias restantes
- [ ] CTA dinamico segun estado del usuario:
  - [ ] Sin autenticar: boton "Inicia sesion"
  - [ ] Sin perfil profesional: bloque info + link "Crear perfil"
  - [ ] Con perfil, sin propuesta: boton "Enviar propuesta" (gradient)
  - [ ] Ya propuso: bloque verde "Ya enviaste una propuesta"
  - [ ] Es propietario: bloque info "Esta es tu necesidad"
- [ ] Sidebar stats: propuestas count, fecha publicacion, fecha limite, inicio previsto
- [ ] Loading skeletons (header + description + details + action)
- [ ] Error 404 con link volver
- [ ] Accesibilidad: focus trap en modal, labels, aria-live

### Pantalla 3: Enviar Propuesta (Modal)
- [ ] Dialog shadcn/ui con titulo + subtitulo + presupuesto referencia
- [ ] Campos: precio (number), moneda (select), dias estimados (number, opcional), mensaje (textarea)
- [ ] Labels con asterisco en obligatorios
- [ ] Nota informativa de rango dinamica (dentro/por encima/por debajo) con colores
- [ ] Nota informativa actualizada en tiempo real al cambiar precio
- [ ] Character counter para mensaje (color cambia al acercarse al limite)
- [ ] Validacion en tiempo real (blur): precio > 0, moneda seleccionada, mensaje min 20
- [ ] Mensajes de error bajo cada campo invalido (role="alert")
- [ ] Submit disabled si hay errores de validacion
- [ ] Loading state en submit (spinner + "Enviando..." + form disabled)
- [ ] Success: dialog cierra + toast
- [ ] Error API: toast + dialog permanece abierto
- [ ] Cancelar y Escape cierran sin cambios
- [ ] Focus trap dentro del dialog
- [ ] Zod schema + React Hook Form

### Pantalla 4: Mis Propuestas
- [ ] Ruta protegida (redirect a login si no autenticado)
- [ ] Titulo + subtitulo
- [ ] Filter chips para estado (Todas/Pendiente/Aceptada/Rechazada/Retirada) con estilos de color
- [ ] URL sync del filtro de estado
- [ ] Results count
- [ ] PropuestaCard con: titulo necesidad (link), artista, precio + moneda, estado badge, fecha envio, fecha respuesta (si existe)
- [ ] Acciones segun estado: Pendiente -> boton Retirar, Aceptada -> boton Ver acuerdo, otros -> solo lectura
- [ ] Loading: 4 skeleton cards
- [ ] Empty state (sin ninguna propuesta)
- [ ] Empty state (sin propuestas del estado filtrado)
- [ ] Paginacion
- [ ] Titulo necesidad es link clickeable a detalle

### Pantalla 5: Confirmar Retirar Propuesta
- [ ] Dialog shadcn/ui con titulo "Retirar propuesta"
- [ ] Warning alert roja: "Esta accion no se puede deshacer"
- [ ] Referencia al titulo de la necesidad
- [ ] Explicacion de consecuencias
- [ ] Boton "Cancelar" (outline)
- [ ] Boton "Confirmar retirada" (destructive rojo)
- [ ] Loading state: ambos botones disabled + spinner en confirmar
- [ ] Success: dialog cierra + toast + lista actualizada (badge cambia a Retirada, boton Retirar desaparece)
- [ ] Error: toast + dialog cierra
- [ ] Escape y [x] cierran sin cambios
- [ ] Focus trap dentro del dialog
- [ ] aria-modal, aria-labelledby, aria-describedby

### Componentes Reutilizables
- [ ] NecesidadCard con todas las props tipadas
- [ ] NecesidadCardSkeleton
- [ ] PropuestaCard con acciones condicionales por estado
- [ ] PropuestaCardSkeleton
- [ ] EstadoPropuestaBadge con colores correctos

### Cross-cutting
- [ ] Dark theme aplicado consistentemente en todas las pantallas
- [ ] Design tokens usados (bg, text, border vars via Tailwind)
- [ ] shadcn/ui components: Button, Card, Input, Select, Badge, Dialog, Textarea, Pagination, Avatar, Label, Alert
- [ ] Tailwind utilities sin valores hardcoded
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste minimo 4.5:1 verificado en todos los textos
- [ ] Focus states con ring purple en todos los interactivos
- [ ] ARIA labels, roles y live regions
- [ ] Keyboard navigation: Tab, Shift+Tab, Enter, Space, Escape
- [ ] Loading states con aria-busy
- [ ] Mobile-first responsive design
- [ ] Toast notifications con shadcn/ui Toaster
- [ ] Form validation con Zod + React Hook Form
- [ ] TanStack Query para data fetching con staleTime y invalidateQueries
- [ ] Debounce en filtros de texto (300ms search, 500ms numeros)
- [ ] URL state sync para filtros y paginacion
- [ ] Error handling con toasts descriptivos
- [ ] Protect route /crowdsourcing/mis-propuestas (redirect si no autenticado)
