# UI/UX: Inscripcion en Programa

> **Feature:** cp-inscripcion-programa
> **User Story:** US-CP-03
> **Ultima actualizacion:** 2026-02-25

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron sidebar + KPI cards + tabs + tabla de promotores |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing | Header publico, cards de campana, paleta de colores |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Explorar Programas (landing publica), Mis Programas (promotor) | `references/templates/krowd/` |
| **Dashtail** | Solicitudes y promotores del programa (dashboard artista) | `references/templates/dashtail/` |

**Componentes de referencia clave:**
- Layout dashboard: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/layout.tsx`
- Dialogo de confirmacion: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/delete-confirmation-dialog.tsx`
- Cards de campana (patron visual para cards de programa): patron de WPR_1-Landing.png

---

## Design Tokens

### Paleta de Colores

Reutilizados integramente de US-CP-01 y US-CP-02. Se agregan tokens especificos para los estados de inscripcion.

```css
:root {
  /* Backgrounds */
  --bg-primary: #0d0d1a;        /* Fondo base (sidebar, header) */
  --bg-secondary: #1a1a2e;      /* Fondo de paginas y contenedores */
  --bg-card: #151525;           /* Fondo de cards */
  --bg-card-hover: #1e1e38;     /* Card hover state */
  --bg-input: #0f0f1f;          /* Fondo de inputs */
  --bg-sidebar: #0d0d1a;        /* Sidebar del dashboard */

  /* Colores primarios - Gradiente pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);
  --primary-active: #9333ea;

  /* Texto */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;
  --text-link: #a855f7;

  /* Estados de inscripcion (NUEVOS para US-CP-03) */
  --status-pendiente-bg: rgba(245, 158, 11, 0.1);   /* Fondo badge Pendiente */
  --status-pendiente-text: #f59e0b;                  /* Texto badge Pendiente */
  --status-pendiente-border: rgba(245, 158, 11, 0.3);
  --status-aprobado-bg: rgba(16, 185, 129, 0.1);    /* Fondo badge Aprobado */
  --status-aprobado-text: #10b981;                   /* Texto badge Aprobado */
  --status-aprobado-border: rgba(16, 185, 129, 0.3);
  --status-bloqueado-bg: rgba(239, 68, 68, 0.1);    /* Fondo badge Bloqueado */
  --status-bloqueado-text: #ef4444;                  /* Texto badge Bloqueado */
  --status-bloqueado-border: rgba(239, 68, 68, 0.3);

  /* Estado general */
  --status-active: #10b981;
  --status-warning: #f59e0b;
  --status-error: #ef4444;
  --status-info: #3b82f6;

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Copy to clipboard highlight */
  --copy-highlight-bg: rgba(168, 85, 247, 0.08);    /* Fondo del bloque de codigo referido */
  --copy-highlight-border: rgba(168, 85, 247, 0.3); /* Borde del bloque de codigo referido */
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - etiquetas auxiliares, hints */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario */
  --text-base: 1rem;     /* 16px - texto de inputs, contenido */
  --text-lg: 1.125rem;   /* 18px - subtitulos de seccion */
  --text-xl: 1.25rem;    /* 20px - titulos de card */
  --text-2xl: 1.5rem;    /* 24px - titulo de pagina landing */
  --text-3xl: 1.875rem;  /* 30px - titulos de pagina dashboard */

  /* Pesos */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Interlineado */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
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
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;  /* 6px */
  --radius-md: 0.5rem;    /* 8px */
  --radius-lg: 0.75rem;   /* 12px */
  --radius-xl: 1rem;      /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.35);
}
```

---

## Pantalla 1: Explorar Programas (Promotor)

**Mockup:** Patron visual de WPR_1-Landing.png (cards en grid sobre fondo oscuro)
**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/crowdpromotion/explorar`
**Template base:** Krowd (paginas publicas con header de landing)

**Precondicion:** Usuario autenticado con perfil de promotor activo. Si no esta autenticado, redirigir a `/login?redirect=/crowdpromotion/explorar`. Si no tiene perfil de promotor, mostrar banner de invitacion a crear perfil.

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (h-16, bg-[#0d0d1a], border-b border-[#334155])      │
│ [Logo WePlay Rises]   Explorar  Artistas  [CrowdPromotion]  [Avatar]│
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  HERO SECTION (py-10, bg-[#0d0d1a])                                 │
│  Programas de promocion disponibles                                  │
│  Encuentra programas y empieza a ganar comisiones                    │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  FILTROS (max-w-5xl mx-auto px-4, mt-6)                              │
│  [Buscar artista...] [Tipo de programa v] [Rango comision v]         │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  GRID DE CARDS (max-w-5xl mx-auto px-4, mt-6)                        │
│                                                                     │
│  ┌──────────────────────┐  ┌──────────────────────┐                 │
│  │ Card programa 1      │  │ Card programa 2      │                 │
│  │ [Referral] [3 tareas]│  │ [Afiliado] [5 tareas]│                 │
│  │ Titulo programa      │  │ Titulo programa      │                 │
│  │ Artista: Luna Nova   │  │ Artista: The Waves   │                 │
│  │ 10% comision         │  │ 5 EUR/conversion     │                 │
│  │ Mi Album | Mar-Jun   │  │ Gira Verano | Abr-Sep│                 │
│  │ [Solicitar inscr.]   │  │ [PENDIENTE]          │                 │
│  └──────────────────────┘  └──────────────────────┘                 │
│                                                                     │
│  ┌──────────────────────┐  ┌──────────────────────┐                 │
│  │ Card programa 3      │  │ Card programa 4      │                 │
│  │ ...                  │  │ [APROBADO]           │                 │
│  └──────────────────────┘  └──────────────────────┘                 │
│                                                                     │
│  PAGINACION: [< Anterior]  1  2  3  [Siguiente >]                    │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Card de Programa (Explorar)

```
┌─────────────────────────────────────────────────────────┐
│  [Referral badge]  [3 tareas badge]                     │
│                                                         │
│  Promociona mi nuevo album                              │
│  Luna Nova                                              │
│                                                         │
│  ─────────────────────────────────────────────────────  │
│                                                         │
│  Comision:  10% por backing referido                    │
│  Campana:   Mi Album Debut                              │
│  Vigencia:  01 Mar 2026 - 01 Jun 2026                   │
│  Moneda:    EUR                                         │
│                                                         │
│  ─────────────────────────────────────────────────────  │
│                                                         │
│  [Solicitar inscripcion ->]                             │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Variante con estado PENDIENTE:**
```
┌─────────────────────────────────────────────────────────┐
│  [Afiliado badge]  [5 tareas badge]                     │
│                                                         │
│  Difunde mi gira de verano                              │
│  The Waves                                              │
│                                                         │
│  Comision:  5 EUR/conversion                            │
│  Campana:   Gira Verano 2026                            │
│  Vigencia:  01 Abr 2026 - 30 Sep 2026                   │
│                                                         │
│  Esperando aprobacion del artista                       │
│  [PENDIENTE DE APROBACION]                              │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Variante con estado APROBADO:**
```
┌─────────────────────────────────────────────────────────┐
│  [Influencer badge]  [2 tareas badge]                   │
│                                                         │
│  Campana Influencers                                    │
│  Luna Nova                                              │
│                                                         │
│  Comision:  15% + 10 EUR/conversion                     │
│  Campana:   Mi Album Debut                              │
│                                                         │
│  [APROBADO] Ver mis datos ->                            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**Variante con estado BLOQUEADO:**
```
┌─────────────────────────────────────────────────────────┐
│  [Referral badge]  [3 tareas badge]          BLOQUEADO  │
│                                                         │
│  Programa X                                             │
│  Artista Y                         (card con opacidad   │
│                                     reducida: 60%)      │
│  No puedes inscribirte en este programa                 │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 1

**Hero section**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo de pagina | `<h1>` | `text-2xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Contenedor hero | `<section>` | `py-10 bg-[#0d0d1a] border-b border-[#334155]` |
| Contenedor inner | `<div>` | `max-w-5xl mx-auto px-4` |

**Barra de filtros**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor filtros | `<div>` | `flex flex-wrap items-center gap-3 mb-6` |
| Input busqueda artista | `<div>` con `<Input>` | `relative flex-1 min-w-[200px] max-w-xs` / Input: `pl-9 bg-[#151525] border-[#334155] text-white h-10 placeholder:text-[#64748b]` / Icono `<Search w-4 h-4>`: `absolute left-3 top-3 text-[#64748b]` |
| Select tipo de programa | `<Select>` | `w-[180px] bg-[#151525] border-[#334155] text-white h-10` / Trigger con `<SelectTrigger>` / Opciones: "Todos los tipos", "Referral", "Afiliado", "Influencer", "Mixto" |
| Select rango comision | `<Select>` | `w-[180px] bg-[#151525] border-[#334155] text-white h-10` / Opciones: "Cualquier comision", "0-5%", "5-10%", "Mas de 10%", "Comision fija" |
| Boton limpiar filtros | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-white h-10 px-3 text-sm` con icono `<X w-3.5 h-3.5 mr-1.5>` (visible solo si hay filtros activos) |

**Card de programa (Explorar)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor (no inscrito) | `<Card>` | `bg-[#151525] border-[#334155] hover:border-[#a855f7]/50 transition-colors p-5 flex flex-col gap-3` |
| Card contenedor (bloqueado) | `<Card>` | `bg-[#151525] border-[#334155] p-5 flex flex-col gap-3 opacity-60 cursor-not-allowed` |
| Fila de badges superior | `<div>` | `flex items-center gap-2` |
| Badge tipo de programa | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium` |
| Badge numero de tareas | `<Badge>` | `bg-[#1e1e38] text-[#64748b] border border-[#334155] text-xs` con icono `<ClipboardList w-3 h-3 mr-1>` |
| Titulo del programa | `<h3>` | `text-base font-semibold text-white leading-tight` |
| Nombre del artista | `<p>` | `text-sm text-[#94a3b8]` con icono `<Music w-3.5 h-3.5 mr-1.5 inline text-[#64748b]>` |
| Separador | `<Separator>` | `bg-[#334155]` |
| Fila de metrica | `<div>` | `flex items-start gap-2` |
| Label metrica | `<span>` | `text-xs text-[#64748b] w-20 shrink-0` |
| Valor comision | `<span>` | `text-sm font-semibold text-[#a855f7]` |
| Valor campana | `<span>` | `text-sm text-white` |
| Valor vigencia | `<span>` | `text-xs text-[#64748b]` con icono `<Calendar w-3 h-3 mr-1 inline>` |
| Valor moneda | `<span>` | `text-xs text-[#64748b]` |

**Elemento de accion segun estado `miEstado`**

| `miEstado` | Componente | Estilos / Props |
|------------|------------|-----------------|
| `null` (no inscrito) | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10` con icono `<UserPlus w-4 h-4 mr-2>` / Texto: "Solicitar inscripcion" |
| `"Pendiente"` | `<div>` con `<Badge>` | Contenedor: `flex flex-col gap-1` / Badge: `bg-amber-950/50 text-amber-400 border border-amber-800/50 text-xs self-start px-3 py-1` / Texto: "PENDIENTE DE APROBACION" / Subtexto debajo: `<p className="text-xs text-[#64748b]">Esperando respuesta del artista</p>` |
| `"Aprobado"` | `<div>` con `<Badge>` y link | Badge: `bg-green-950/50 text-green-400 border border-green-800/50 text-xs self-start px-3 py-1` con icono `<CheckCircle w-3 h-3 mr-1>` / Texto: "APROBADO" / Boton debajo: `<Button variant="ghost" size="sm">` `text-[#a855f7] hover:text-purple-400 p-0 h-auto text-xs` / Texto boton: "Ver mis datos ->" |
| `"Bloqueado"` | `<div>` con texto | `<p className="text-xs text-red-400 flex items-center gap-1.5"><Ban w-3.5 h-3.5 /> No puedes inscribirte en este programa</p>` |

**Grid y paginacion**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid de cards | `<div>` | `grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5` |
| Contenedor paginacion | `<div>` | `flex items-center justify-center gap-2 mt-8 pb-10` |
| Boton pagina | `<Button variant="ghost" size="sm">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 w-9 p-0` |
| Pagina activa | `<Button size="sm">` | `bg-[#1e1e38] text-white h-9 w-9 p-0 font-semibold` |
| Boton anterior/siguiente | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-3` |
| Contador resultados | `<p>` | `text-sm text-[#64748b] mb-4` (ej: "Mostrando 10 de 24 programas") |

### Estados de UI Pantalla 1

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | Grid de 6 cards skeleton con `animate-pulse`. Cada skeleton: `bg-[#1e1e38] rounded-xl h-[240px]` |
| **Default con programas** | Grid de cards con datos reales. Paginacion visible si totalCount > pageSize (10) |
| **Solicitando inscripcion** | Boton "Solicitar inscripcion" del programa pulsado muestra spinner `<Loader2 w-4 h-4 animate-spin>` + "Enviando..." y queda deshabilitado durante la peticion |
| **Empty state (sin programas)** | Centrado en la pagina: icono `<Megaphone w-14 h-14>` con fondo gradient en contenedor redondeado, titulo "No hay programas disponibles en este momento", subtitulo "Vuelve mas tarde para encontrar nuevas oportunidades de promocion" |
| **Sin resultados (filtro activo)** | Icono `<SearchX w-10 h-10 text-[#64748b]>` centrado + texto "No se encontraron programas con los filtros seleccionados" + boton "Limpiar filtros" `<Button variant="outline" size="sm">` |
| **Error de carga** | Card centrado con icono `<AlertCircle w-8 h-8 text-red-400>`, texto "No se pudieron cargar los programas" + boton "Reintentar" |
| **Sin perfil de promotor** | Banner en la parte superior del contenido (debajo de filtros): `bg-amber-950/30 border border-amber-800/50 rounded-lg p-4` con texto "Necesitas un perfil de promotor para solicitar inscripciones." y boton "Crear perfil" que navega a `/promotor/registro` |

### Validacion antes de solicitar inscripcion

| Condicion | Resultado en UI |
|-----------|-----------------|
| Promotor con `EsActivo = false` | Toast destructivo: "Tu perfil de promotor esta inactivo. No puedes solicitar inscripciones." |
| Ya inscrito (EsAprobado = false, EsBloqueado = false) | Boton no aparece (estado Pendiente se muestra en su lugar desde el listado) |
| Promotor bloqueado en ese programa | Card con opacidad 60% + mensaje "No puedes inscribirte en este programa" sin boton |
| Programa fuera de vigencia o inactivo | No aparece en el listado |

### Toast Messages Pantalla 1

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Inscripcion exitosa | Success (verde) | "Solicitud enviada. El artista revisara tu perfil." |
| Ya inscrito (error 400) | Destructive (rojo) | "Ya tienes una solicitud activa para este programa." |
| Promotor bloqueado (403) | Destructive (rojo) | "No puedes inscribirte en este programa." |
| Error inesperado (500) | Destructive (rojo) | "Ocurrio un error. Intentalo de nuevo." |

### Interacciones Pantalla 1

| Accion | Comportamiento |
|--------|----------------|
| Typing en busqueda artista | Filtro local debounceado 300ms sobre los items cargados. No dispara nueva llamada API |
| Cambio en select Tipo | Refetch con parametro `tipoPromoId` |
| Cambio en select Rango comision | Filtro local sobre los items cargados |
| Click "Solicitar inscripcion" | POST a `/api/crowdpromotion/programas/{programaId}/inscripcion`. En success: actualizar `miEstado` de la card a "Pendiente" sin recargar la pagina. Mostrar toast success |
| Click "Ver mis datos ->" (aprobado) | Navegar a `/promotor/mis-programas` con hash `#programa-{id}` para hacer scroll al programa correspondiente |
| Click en paginacion | Fetch de la pagina correspondiente con `page` y `pageSize`. Scroll to top del grid |

---

## Pantalla 2: Mis Programas (Promotor)

**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/mis-programas`
**Template base:** Krowd / patron dashboard promotor

**Precondicion:** Usuario autenticado con perfil de promotor. Si no hay perfil, redirigir a `/promotor/registro`.

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (igual que Explorar Programas)                        │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CONTENIDO (max-w-4xl mx-auto px-4 py-8)                             │
│                                                                     │
│  Mis programas                 [Explorar programas ->]               │
│  Gestiona tus inscripciones                                          │
│                                                                     │
│  FILTRO DE ESTADO:                                                   │
│  [Todos] [Aprobado] [Pendiente] [Bloqueado]   (tabs o botones)      │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ ITEM 1: Aprobado                                             │   │
│  │ Titulo: Promociona mi nuevo album    [APROBADO]              │   │
│  │ Artista: Luna Nova                                           │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ Codigo referido:                                             │   │
│  │ ┌─────────────────────────────────────┐ [Copiar]            │   │
│  │ │ album-2026-x7k9m                    │                     │   │
│  │ └─────────────────────────────────────┘                     │   │
│  │                                                              │   │
│  │ URL de tracking:                                             │   │
│  │ ┌─────────────────────────────────────┐ [Copiar]            │   │
│  │ │ https://weplay.com/...ref=album-... │                     │   │
│  │ └─────────────────────────────────────┘                     │   │
│  │                                                              │   │
│  │ Comision: 10% por backing referido                           │   │
│  │ Inscrito: 05 Mar 2026                                        │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ ITEM 2: Pendiente                                            │   │
│  │ Titulo: Difunde mi gira de verano    [PENDIENTE]             │   │
│  │ Artista: The Waves                                           │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ [Clock] Esperando aprobacion del artista                     │   │
│  │ Inscrito: 10 Mar 2026                                        │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ ITEM 3: Bloqueado                                            │   │
│  │ Titulo: Promo sin acceso           [BLOQUEADO]               │   │
│  │ Artista: DJ Alpha                                            │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ [Ban] Tu acceso a este programa ha sido bloqueado            │   │
│  │ Inscrito: 01 Feb 2026                                        │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 2

**Cabecera de pagina**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor cabecera | `<div>` | `flex items-start justify-between mb-6` |
| Titulo | `<h1>` | `text-2xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Boton explorar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-4 text-sm` con icono `<Compass w-4 h-4 mr-2>` |

**Filtro por estado**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor filtros | `<div>` | `flex items-center gap-2 mb-5` |
| Boton "Todos" activo | `<Button size="sm">` | `bg-[#1e1e38] text-white border border-[#a855f7] h-8 px-4 text-sm rounded-full` |
| Boton "Todos" inactivo | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-white hover:bg-[#1e1e38] h-8 px-4 text-sm rounded-full` |
| Boton "Aprobado" activo | `<Button size="sm">` | `bg-green-950/50 text-green-400 border border-green-800/50 h-8 px-4 text-sm rounded-full` |
| Boton "Aprobado" inactivo | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-green-400/70 hover:bg-green-950/30 h-8 px-4 text-sm rounded-full` |
| Boton "Pendiente" activo | `<Button size="sm">` | `bg-amber-950/50 text-amber-400 border border-amber-800/50 h-8 px-4 text-sm rounded-full` |
| Boton "Bloqueado" activo | `<Button size="sm">` | `bg-red-950/50 text-red-400 border border-red-800/50 h-8 px-4 text-sm rounded-full` |

**Item de inscripcion (card expandida)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor item | `<Card>` | `bg-[#151525] border-[#334155] p-5 mb-4` |
| Cabecera item | `<div>` | `flex items-start justify-between` |
| Titulo programa | `<h3>` | `text-base font-semibold text-white` |
| Nombre artista | `<p>` | `text-sm text-[#94a3b8] mt-0.5` con icono `<Music w-3.5 h-3.5 mr-1.5 inline text-[#64748b]>` |
| Badge APROBADO | `<Badge>` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs px-2.5 py-1` con icono `<CheckCircle w-3 h-3 mr-1>` |
| Badge PENDIENTE | `<Badge>` | `bg-amber-950/50 text-amber-400 border border-amber-800/50 text-xs px-2.5 py-1` con icono `<Clock w-3 h-3 mr-1>` |
| Badge BLOQUEADO | `<Badge>` | `bg-red-950/50 text-red-400 border border-red-800/50 text-xs px-2.5 py-1` con icono `<Ban w-3 h-3 mr-1>` |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Label campo | `<p>` | `text-xs font-medium text-[#94a3b8] mb-1.5` |
| Fecha inscripcion | `<p>` | `text-xs text-[#64748b] mt-3` con icono `<Calendar w-3 h-3 mr-1 inline>` |

**Bloque codigo referido (solo si APROBADO)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor codigo | `<div>` | `flex items-center gap-2` |
| Campo codigo | `<div>` | `flex-1 flex items-center bg-[#0f0f1f] border border-[#a855f7]/30 rounded-lg px-3 h-10 font-mono text-sm text-[#a855f7] overflow-hidden` |
| Boton copiar codigo | `<Button variant="ghost" size="sm">` | `h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0` con icono `<Copy w-4 h-4>` |
| Icono copiado | `<Check w-4 h-4>` | `text-green-400` (aparece 1.5s tras copiar en lugar del icono Copy) |

**Bloque URL tracking (solo si APROBADO)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor URL | `<div>` | `flex items-center gap-2` |
| Campo URL | `<div>` | `flex-1 flex items-center bg-[#0f0f1f] border border-[#334155] rounded-lg px-3 h-10 text-sm text-white overflow-hidden` |
| Texto URL truncado | `<span>` | `truncate text-sm text-[#94a3b8]` (URL completa visible en tooltip al hover) |
| Boton copiar URL | `<Button variant="ghost" size="sm">` | Igual que boton copiar codigo |
| Tooltip URL completa | `<TooltipProvider>` + `<Tooltip>` + `<TooltipContent>` | `bg-[#0d0d1a] border-[#334155] text-white text-xs max-w-[400px] break-all` |

**Mensaje estado PENDIENTE**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor mensaje | `<div>` | `flex items-center gap-3 py-2` |
| Icono reloj | `<Clock w-5 h-5>` | `text-amber-400 shrink-0` |
| Texto principal | `<p>` | `text-sm text-amber-300` / "Esperando aprobacion del artista" |
| Texto secundario | `<p>` | `text-xs text-[#64748b] mt-0.5` / "Recibirás tu codigo referido cuando seas aprobado" |

**Mensaje estado BLOQUEADO**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor mensaje | `<div>` | `flex items-center gap-3 py-2` |
| Icono ban | `<Ban w-5 h-5>` | `text-red-400 shrink-0` |
| Texto | `<p>` | `text-sm text-red-300` / "Tu acceso a este programa ha sido bloqueado" |

### Estados de UI Pantalla 2

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton de 3 items: `bg-[#1e1e38] rounded-xl h-[160px] animate-pulse mb-4` |
| **Default** | Lista de items ordenados por `fechaAlta` descendente |
| **Empty state (ninguna inscripcion)** | Icono `<Megaphone w-12 h-12>` con gradiente, titulo "No estas inscrito en ningun programa", subtitulo "Explora los programas disponibles y empieza a ganar comisiones", boton "Explorar programas" gradient que navega a `/crowdpromotion/explorar` |
| **Filtro sin resultados** | Texto centrado: "No tienes inscripciones con este estado" + boton "Ver todas" |
| **Error de carga** | Card de error con icono `<AlertCircle>` + boton "Reintentar" |
| **Copiado** | Icono `<Copy>` del boton cambia a `<Check>` verde durante 1.5 segundos |

### Toast Messages Pantalla 2

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Codigo copiado | Default (fondo oscuro) | "Codigo referido copiado al portapapeles" |
| URL copiada | Default | "URL de tracking copiada al portapapeles" |
| Error al copiar | Destructive | "No se pudo copiar. Selecciona el texto manualmente." |

### Interacciones Pantalla 2

| Accion | Comportamiento |
|--------|----------------|
| Click boton filtro estado | Filtro local sobre los items cargados. El boton activo cambia estilos. No dispara nueva llamada API |
| Click icono copiar (codigo) | `navigator.clipboard.writeText(codigoReferido)`. Cambiar icono a `<Check>` durante 1.5s. Mostrar toast |
| Click icono copiar (URL) | `navigator.clipboard.writeText(urlTrackingPersonalizada)`. Mismo comportamiento |
| Hover sobre URL truncada | Mostrar tooltip con URL completa |
| Click "Explorar programas" (empty state) | Navegar a `/crowdpromotion/explorar` |

---

## Pantalla 3: Detalle Mi Programa Aprobado (Panel lateral o seccion expandida)

**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/mis-programas` (modal o seccion expandida inline, NO ruta separada)
**Trigger:** Click en el item aprobado en la lista de Mis Programas para ver el detalle con tareas y estadisticas

Esta vista se implementa como una **seccion expandida** dentro de la card del item aprobado. Al hacer click en "Ver tareas y estadisticas", la card se expande para mostrar el detalle.

### Layout (card expandida)

```
┌──────────────────────────────────────────────────────────────────┐
│ Promociona mi nuevo album                          [APROBADO]    │
│ Artista: Luna Nova                                               │
│ ─────────────────────────────────────────────────────────────── │
│                                                                  │
│ Tu codigo referido:                                              │
│ ┌─────────────────────────────────────────┐ [Copiar]            │
│ │ album-2026-x7k9m                        │                     │
│ └─────────────────────────────────────────┘                     │
│                                                                  │
│ Tu URL de tracking:                                              │
│ ┌─────────────────────────────────────────┐ [Copiar]            │
│ │ https://weplay.com/campanias/...        │                     │
│ └─────────────────────────────────────────┘                     │
│                                                                  │
│ Comision: 10% por cada backing referido                          │
│ Inscrito: 05 Mar 2026                                            │
│                                                                  │
│ ─────────────────────────────────────────────────────────────── │
│ [^ Ocultar detalles]                                             │
│ ─────────────────────────────────────────────────────────────── │
│                                                                  │
│ ESTADISTICAS (MVP: valores en 0 o N/A)                           │
│ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐            │
│ │  Clics       │  │  Conv.       │  │  Comision    │            │
│ │  [  0  ]     │  │  [  0  ]     │  │ [ 0.00 EUR ] │            │
│ └──────────────┘  └──────────────┘  └──────────────┘            │
│ Estadisticas en tiempo real (proximamente)                       │
│                                                                  │
│ ─────────────────────────────────────────────────────────────── │
│                                                                  │
│ TAREAS DEL PROGRAMA (3)                                          │
│                                                                  │
│ ┌──────────────────────────────────────────────────────────────┐ │
│ │ [Share] Comparte en Instagram Stories                        │ │
│ │ Dinero: 5.00 EUR | Repetible x10                             │ │
│ └──────────────────────────────────────────────────────────────┘ │
│ ┌──────────────────────────────────────────────────────────────┐ │
│ │ [Post] Publica un TikTok sobre la campana                    │ │
│ │ Dinero: 10.00 EUR | Repetible x5                             │ │
│ └──────────────────────────────────────────────────────────────┘ │
│ ┌──────────────────────────────────────────────────────────────┐ │
│ │ [Review] Escribe una resena en tu blog                       │ │
│ │ Dinero: 3.00 EUR | No repetible                              │ │
│ └──────────────────────────────────────────────────────────────┘ │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 3

**Mini KPI de estadisticas (MVP)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor KPI | `<div>` | `grid grid-cols-3 gap-3 my-4` |
| Card KPI | `<div>` | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-center` |
| Valor KPI | `<p>` | `text-xl font-bold text-white` |
| Label KPI | `<p>` | `text-xs text-[#64748b] mt-1` |
| Texto MVP | `<p>` | `text-xs text-[#64748b] italic text-center mt-2` / "Estadisticas en tiempo real (proximamente)" |

**Lista de tareas del programa**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo seccion | `<h4>` | `text-sm font-semibold text-[#cbd5e1] mb-3 flex items-center gap-2` con `<ClipboardList w-4 h-4 text-[#64748b]>` |
| Contenedor lista | `<div>` | `space-y-2` |
| Card tarea | `<div>` | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3` |
| Cabecera tarea | `<div>` | `flex items-start gap-2` |
| Badge tipo evento | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium shrink-0` |
| Nombre tarea | `<p>` | `text-sm text-white font-medium` |
| Detalles tarea | `<p>` | `text-xs text-[#64748b] mt-1` (ej: "Dinero: 5.00 EUR | Repetible x10") |
| Sin tareas | `<p>` | `text-sm text-[#64748b] italic py-2` / "Este programa no tiene tareas definidas" |

**Toggle expandir/colapsar**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Boton toggle | `<Button variant="ghost" size="sm">` | `w-full text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 text-xs gap-1.5` |
| Icono expandir | `<ChevronDown w-3.5 h-3.5>` | Visible cuando colapsado |
| Icono colapsar | `<ChevronUp w-3.5 h-3.5>` | Visible cuando expandido |
| Texto expandir | "Ver tareas y estadisticas" | Visible cuando colapsado |
| Texto colapsar | "Ocultar detalles" | Visible cuando expandido |

### Interacciones Pantalla 3

| Accion | Comportamiento |
|--------|----------------|
| Click "Ver tareas y estadisticas" | Expandir la seccion de detalle con animacion `max-h` suave (200ms ease-in-out). Scroll al item si es necesario |
| Click "Ocultar detalles" | Colapsar la seccion de detalle con animacion |

---

## Pantalla 4: Solicitudes Pendientes (Artista - Tab en Detalle de Programa)

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/{id}` (tab "Solicitudes" en la pantalla de detalle de US-CP-02)
**Template base:** Dashtail dashboard layout

Esta pantalla extiende la Pantalla 3 de US-CP-02 (Detalle del Programa). Agrega las tabs "Solicitudes", "Aprobados" y "Bloqueados" al bloque de Tabs existente. El tab activo por defecto sigue siendo "Info General", pero si hay solicitudes pendientes, el tab "Solicitudes" muestra un badge contador.

### Layout (Tabs extendidos)

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR     MAIN CONTENT                                             │
│             ┌───────────────────────────────────────────────────┐   │
│             │ Breadcrumb: CrowdPromotion / Mis Programas / {...} │   │
│             ├───────────────────────────────────────────────────┤   │
│             │                                                   │   │
│             │  {titulo del programa}       [Editar][Desactivar] │   │
│             │  [Referral] [ACTIVO]  Mi Album Debut              │   │
│             │                                                   │   │
│             │  ┌──────────┐ ┌──────────┐ ┌──────────┐          │   │
│             │  │Aprobados │ │Pendientes│ │Bloqueados│          │   │
│             │  │  [ 5 ]   │ │  [ 2 ]   │ │  [ 1 ]   │          │   │
│             │  └──────────┘ └──────────┘ └──────────┘          │   │
│             │                                                   │   │
│             │  [Info General][Tareas][Solicitudes (2)][Aprobados│   │
│             │                       ][Bloqueados]              │   │
│             │  ─────────────────────────────────────────────── │   │
│             │                                                   │   │
│             │  TAB SOLICITUDES PENDIENTES                       │   │
│             │  ┌─────────────────────────────────────────────┐  │   │
│             │  │ DJ Marketing Pro          hace 2 dias       │  │   │
│             │  │ Tipo: Influencer | IG: @djmarkpro (15k)     │  │   │
│             │  │ Web: djmarkpro.com                          │  │   │
│             │  │                  [Aprobar] [Rechazar][Bloq.] │  │   │
│             │  └─────────────────────────────────────────────┘  │   │
│             │  ┌─────────────────────────────────────────────┐  │   │
│             │  │ MusicBlog.es              hace 1 dia        │  │   │
│             │  │ Tipo: Medio/Blog | Web: musicblog.es        │  │   │
│             │  │                  [Aprobar] [Rechazar][Bloq.] │  │   │
│             │  └─────────────────────────────────────────────┘  │   │
│             └───────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Tab "Solicitudes Pendientes"

**Tabs actualizados (Pantalla 3 de US-CP-02 extendida)**

| Tab | Valor | Badge |
|-----|-------|-------|
| Info General | `"info"` | Sin badge |
| Tareas | `"tareas"` | Sin badge |
| Solicitudes | `"solicitudes"` | `<Badge>` con numero de pendientes (solo visible si pendientes > 0): `bg-amber-500 text-black text-xs rounded-full w-5 h-5 flex items-center justify-center ml-1.5 font-bold` |
| Aprobados | `"aprobados"` | Sin badge |
| Bloqueados | `"bloqueados"` | Sin badge |

**Cabecera del tab**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo | `<h3>` | `text-base font-semibold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-0.5` / "Revisa y gestiona las solicitudes de inscripcion" |
| Contenedor cabecera | `<div>` | `flex items-start justify-between mb-4` |

**Card de solicitud pendiente**

```
┌─────────────────────────────────────────────────────────────────┐
│  [Avatar iniciales]  DJ Marketing Pro            hace 2 dias    │
│                      Influencer                                 │
│ ─────────────────────────────────────────────────────────────── │
│ Instagram:  @djmarkpro (15k seguidores)                         │
│ TikTok:     @djmarkpro.official                                 │
│ Sitio web:  djmarkpro.com                                       │
│ ─────────────────────────────────────────────────────────────── │
│                     [Aprobar] [Rechazar] [Bloquear]             │
└─────────────────────────────────────────────────────────────────┘
```

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor card | `<Card>` | `bg-[#0f0f1f] border border-[#334155] p-4 mb-3 hover:border-[#334155]/80 transition-colors` |
| Cabecera card | `<div>` | `flex items-start justify-between` |
| Bloque info promotor | `<div>` | `flex items-center gap-3` |
| Avatar | `<Avatar>` | `w-10 h-10` con `<AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-sm font-semibold">` (2 primeras letras del nombre) |
| Nombre promotor | `<p>` | `text-sm font-semibold text-white` |
| Tipo promotor | `<p>` | `text-xs text-[#94a3b8]` |
| Fecha solicitud | `<p>` | `text-xs text-[#64748b] shrink-0` (formato relativo: "hace X dias") |
| Separador | `<Separator>` | `bg-[#334155] my-3` |
| Bloque redes sociales | `<div>` | `space-y-1.5` |
| Fila red social | `<div>` | `flex items-center gap-2` |
| Icono red | Icono Lucide o SVG brand | `w-3.5 h-3.5 text-[#64748b] shrink-0` |
| Label red | `<span>` | `text-xs text-[#64748b] w-20 shrink-0` |
| Valor red (handle/URL) | `<a>` o `<span>` | `text-xs text-[#94a3b8]` (si es URL: `text-[#a855f7] hover:underline`) |
| Sin redes | `<p>` | `text-xs text-[#64748b] italic` / "Sin redes sociales registradas" |
| Contenedor acciones | `<div>` | `flex justify-end gap-2 mt-3` |
| Boton Aprobar | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-8 px-4 text-xs` con icono `<UserCheck w-3.5 h-3.5 mr-1.5>` |
| Boton Rechazar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 px-3 text-xs` con icono `<UserX w-3.5 h-3.5 mr-1.5>` |
| Boton Bloquear | `<Button variant="outline" size="sm">` | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-8 px-3 text-xs` con icono `<Ban w-3.5 h-3.5 mr-1.5>` |

**Dialogo de confirmacion - Rechazar**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialogo | `<AlertDialog>` | Abre al click en "Rechazar" |
| Titulo | "Rechazar solicitud" | `text-white font-semibold` |
| Descripcion | `<AlertDialogDescription>` | `text-[#94a3b8]` / "¿Seguro que deseas rechazar la solicitud de {nombre}? Esta accion no se puede deshacer." |
| Boton cancelar | `<AlertDialogCancel>` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton confirmar | `<AlertDialogAction>` | `border-[#334155] text-white hover:bg-[#1e1e38]` (estilo neutro, no destructivo porque es solo rechazo) |

**Dialogo de confirmacion - Bloquear**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialogo | `<AlertDialog>` | Abre al click en "Bloquear" |
| Titulo | "Bloquear promotor" | `text-white font-semibold` |
| Descripcion | `<AlertDialogDescription>` | `text-[#94a3b8]` / "¿Seguro que deseas bloquear a {nombre}? No podra volver a solicitar inscripcion en este programa." |
| Boton cancelar | `<AlertDialogCancel>` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton confirmar | `<AlertDialogAction>` | `bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200` (estilo destructivo) |

### Estados de UI Tab Solicitudes

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton de 3 cards: `bg-[#1e1e38] rounded-xl h-[120px] animate-pulse mb-3` |
| **Default con solicitudes** | Lista de cards con datos. Ordenadas por `fechaAlta` ascendente (mas antiguas primero) |
| **Empty state** | Icono `<InboxIcon w-10 h-10 text-[#64748b]>` centrado + texto "No hay solicitudes pendientes" en `text-sm text-[#64748b]` |
| **Aprobando** | Boton "Aprobar" del item pulsado muestra `<Loader2 w-3.5 h-3.5 animate-spin>` + "Aprobando..." y se deshabilita. Los otros botones del mismo item tambien se deshabilitan |
| **Rechazando** | Boton "Rechazar" muestra loading tras confirmar en el dialogo. El item desaparece del listado con transicion fade-out `200ms` |
| **Bloqueando** | Similar a Rechazando: loading tras confirmar. El item desaparece del listado con fade-out |
| **Error en accion** | Toast destructivo. El item permanece en la lista para reintento |

### Toast Messages Tab Solicitudes

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Promotor aprobado | Success | "{nombre} ha sido aprobado. Se ha generado su codigo referido." |
| Promotor rechazado | Default | "Solicitud de {nombre} rechazada." |
| Promotor bloqueado | Default | "{nombre} ha sido bloqueado en este programa." |
| Error al aprobar | Destructive | "No se pudo aprobar al promotor. Intentalo de nuevo." |
| Error al rechazar | Destructive | "No se pudo rechazar la solicitud. Intentalo de nuevo." |
| Error al bloquear | Destructive | "No se pudo bloquear al promotor. Intentalo de nuevo." |

### Interacciones Tab Solicitudes

| Accion | Comportamiento |
|--------|----------------|
| Click "Aprobar" | PATCH a `/api/crowdpromotion/programas/{id}/inscripciones/{inscripcionId}/aprobar`. En success: remover card de la lista con fade-out, decrementar contador del badge de solicitudes, incrementar KPI "Aprobados" |
| Click "Rechazar" | Abrir `<AlertDialog>` de confirmacion. En confirmacion: PATCH a `.../rechazar`. En success: remover card con fade-out |
| Click "Bloquear" | Abrir `<AlertDialog>` de confirmacion. En confirmacion: PATCH a `.../bloquear`. En success: remover card con fade-out, incrementar KPI "Bloqueados" |
| Click "Cancelar" en dialogo | Cerrar dialogo. Ningun cambio |

---

## Pantalla 5: Promotores Aprobados (Artista - Tab en Detalle de Programa)

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/{id}` (tab "Aprobados")

### Layout del Tab

```
┌──────────────────────────────────────────────────────────────────┐
│ TAB APROBADOS                                                    │
│                                                                  │
│  Promotores aprobados (5)                                        │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ [Av] DJ Marketing Pro        album-2026-x7k9m            │   │
│  │      Influencer              Clics: 142 | Conv.: 8        │   │
│  │      Aprobado: 10 Mar 2026                 [Dar de baja] │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ [Av] MusicBlog.es            album-2026-k2mn8            │   │
│  │      Medio/Blog              Clics: 89  | Conv.: 3        │   │
│  │      Aprobado: 12 Mar 2026                 [Dar de baja] │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Especificaciones Tab "Promotores Aprobados"

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor card | `<Card>` | `bg-[#0f0f1f] border border-[#334155] p-4 mb-3` |
| Cabecera card | `<div>` | `flex items-start gap-3` |
| Avatar | `<Avatar>` | `w-9 h-9` con `<AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-xs font-semibold">` |
| Bloque info izq | `<div>` | `flex-1` |
| Nombre promotor | `<p>` | `text-sm font-semibold text-white` |
| Tipo promotor | `<p>` | `text-xs text-[#94a3b8]` |
| Fecha aprobacion | `<p>` | `text-xs text-[#64748b] mt-0.5` con icono `<Calendar w-3 h-3 mr-1 inline>` |
| Bloque info der | `<div>` | `text-right` |
| Codigo referido | `<p>` | `text-xs font-mono text-[#a855f7] font-medium` |
| Stats | `<p>` | `text-xs text-[#64748b] mt-0.5` / "Clics: X \| Conv.: Y" |
| Boton Dar de baja | `<Button variant="outline" size="sm">` | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-7 px-3 text-xs mt-2` con icono `<UserMinus w-3.5 h-3.5 mr-1>` |

**Dialogo de confirmacion - Dar de baja**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialogo | `<AlertDialog>` | Abre al click en "Dar de baja" |
| Titulo | "Dar de baja al promotor" | `text-white font-semibold` |
| Descripcion | `<AlertDialogDescription>` | `text-[#94a3b8]` / "¿Seguro que deseas dar de baja a {nombre}? Su codigo referido quedara desactivado." |
| Boton cancelar | `<AlertDialogCancel>` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton confirmar | `<AlertDialogAction>` | `bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200` |

### Estados de UI Tab Aprobados

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton de 3 items |
| **Default** | Lista de cards. Ordenada por fecha de aprobacion descendente |
| **Empty state** | Icono `<Users w-10 h-10 text-[#64748b]>` + "No hay promotores aprobados en este programa" |
| **Dando de baja** | Boton "Dar de baja" del item confirmado muestra loading. El item desaparece con fade-out tras respuesta exitosa |
| **Error** | Toast destructivo. Item permanece para reintento |

### Toast Messages Tab Aprobados

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Dado de baja exitoso | Default | "{nombre} ha sido dado de baja. Su codigo referido ha sido desactivado." |
| Error al dar de baja | Destructive | "No se pudo dar de baja al promotor. Intentalo de nuevo." |

### Interacciones Tab Aprobados

| Accion | Comportamiento |
|--------|----------------|
| Click "Dar de baja" | Abrir `<AlertDialog>` de confirmacion |
| Confirmar "Dar de baja" | PATCH a `/api/crowdpromotion/programas/{id}/inscripciones/{inscripcionId}/baja`. En success: remover card con fade-out, decrementar KPI "Aprobados" |

---

## Pantalla 6: Promotores Bloqueados (Artista - Tab en Detalle de Programa)

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/{id}` (tab "Bloqueados")

### Layout del Tab

```
┌──────────────────────────────────────────────────────────────────┐
│ TAB BLOQUEADOS                                                   │
│                                                                  │
│  Promotores bloqueados                                           │
│  Solo lectura. Los promotores bloqueados no pueden              │
│  re-solicitar inscripcion en este programa.                      │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ [Av] Spammer123                                          │   │
│  │      Fan Embajador                                       │   │
│  │      Bloqueado: 01 Feb 2026                              │   │
│  └──────────────────────────────────────────────────────────┘   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Especificaciones Tab "Promotores Bloqueados"

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Aviso informativo | `<div>` con icono `<Info>` | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mb-4` |
| Texto aviso | `<p>` | `text-sm text-blue-300` / "Los promotores bloqueados no pueden re-solicitar inscripcion en este programa." |
| Contenedor card | `<Card>` | `bg-[#0f0f1f] border border-[#334155] p-4 mb-3 opacity-80` |
| Cabecera card | `<div>` | `flex items-center gap-3` |
| Avatar | `<Avatar>` | `w-9 h-9` con `<AvatarFallback className="bg-red-950/50 text-red-400 text-xs font-semibold">` |
| Nombre promotor | `<p>` | `text-sm font-medium text-white` |
| Tipo promotor | `<p>` | `text-xs text-[#94a3b8]` |
| Badge bloqueado | `<Badge>` | `bg-red-950/50 text-red-400 border border-red-800/50 text-xs ml-auto` con icono `<Ban w-3 h-3 mr-1>` |
| Fecha bloqueo | `<p>` | `text-xs text-[#64748b] mt-1` con icono `<Calendar w-3 h-3 mr-1 inline>` |

### Estados de UI Tab Bloqueados

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton de 2 items |
| **Default** | Lista de cards de solo lectura. Sin botones de accion |
| **Empty state** | Icono `<ShieldCheck w-10 h-10 text-[#64748b]>` + "No hay promotores bloqueados" |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios en Landing (Explorar / Mis Programas) |
|------------|-------|----------------------------------------------|
| Mobile | < 640px | Grid de 1 columna. Filtros apilados verticalmente. Header colapsado (hamburger). Botones full-width |
| Tablet | 640-1024px | Grid de 2 columnas. Filtros en fila |
| Desktop | > 1024px | Grid de 3 columnas (Explorar). Filtros en fila con busqueda expandida |

| Breakpoint | Width | Cambios en Admin (Dashboard Artista) |
|------------|-------|--------------------------------------|
| Mobile | < 768px | Sidebar colapsado (drawer). Tabs scrollables horizontalmente. Cards de solicitud a columna completa. Botones de accion apilados |
| Tablet | 768-1024px | Sidebar visible (w-56). Tabs visibles sin scroll. Cards con layout normal |
| Desktop | > 1024px | Sidebar completo (w-64). Layout completo con KPI cards en fila |

**Detalles de adaptacion mobile - Pantalla 1 (Explorar):**
- Filtros: `flex-col gap-2` en mobile, `flex-row gap-3` en sm+
- Cada filtro/input: `w-full` en mobile
- Card de programa: igual en mobile y desktop (columna completa en mobile)
- Boton "Solicitar inscripcion": `h-11` en mobile (mas facil de tocar)

**Detalles de adaptacion mobile - Pantalla 4/5/6 (Tabs admin):**
- Tabs: `<ScrollArea horizontal>` para que los 5 tabs (Info, Tareas, Solicitudes, Aprobados, Bloqueados) sean scrollables en mobile
- Botones de accion en cards: `flex-col gap-2 w-full` en mobile, `flex-row justify-end` en sm+

---

## Animaciones

| Elemento | Animacion | Duracion | Easing |
|----------|-----------|----------|--------|
| Card hover (Explorar) | `border-color` cambia a `#a855f7/50` | 200ms | ease |
| Boton hover | Scale sutil (Tailwind `hover:scale-[1.01]`) | 150ms | ease |
| Item fade-out (rechazar/bloquear/baja) | `opacity: 1 -> 0` + `height: auto -> 0` | 200ms | ease-out |
| Seccion expandible (Pantalla 3) | `max-height` de 0 a contenido, `overflow-hidden` | 250ms | ease-in-out |
| Toast entrada | Slide desde abajo + fade-in | 200ms | ease-out |
| Toast salida | Fade-out | 150ms | ease-in |
| Icono copiado (Copy -> Check) | `opacity: 0 -> 1` | 100ms | ease |
| Badge contador (solicitudes) | Actualiza valor instantaneamente | - | - |
| Dialogo apertura | Fade + scale desde 95% | 150ms | ease-out |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1 para texto normal, 3:1 para texto grande. Verificado con colores definidos en tokens |
| Focus visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#151525]` en todos los elementos interactivos |
| Labels en formularios | Todos los `<Input>` y `<Select>` tienen `<Label>` asociado via `htmlFor` o `aria-label` |
| Botones con iconos | Botones icon-only (copiar, kebab) tienen `aria-label` descriptivo (ej: `aria-label="Copiar codigo referido"`) |
| Badges de estado | Texto visible dentro del badge (no solo color). Color no es el unico indicador |
| Dialogos de confirmacion | `<AlertDialog>` de shadcn gestiona focus trap, cierre con Escape, y `aria-labelledby` / `aria-describedby` automaticamente |
| Toast mensajes | `role="status"` para toasts informativos, `role="alert"` para toasts de error. Gestionado por sonner/shadcn Toast |
| Tablas | `<Table>` de shadcn incluye `<TableHead>` con semantica correcta |
| Loading states | Skeletons tienen `aria-busy="true"` en el contenedor padre. Spinners tienen `aria-label="Cargando"` |
| Navegacion por teclado | Tabs accesibles con flechas. Listas navegables con Tab/Enter |

---

## Zod Schemas

```typescript
// src/shared/schemas/inscripcion-programa.schema.ts
// (Solo validacion client-side, la logica de negocio es server-side)

// No hay formularios complejos en esta feature.
// La unica accion con input del usuario es el click en botones.
// Los schemas relevantes son los de respuesta de API para tipado.

export const miEstadoSchema = z.union([
  z.null(),
  z.literal("Pendiente"),
  z.literal("Aprobado"),
  z.literal("Bloqueado"),
]);

export type MiEstado = z.infer<typeof miEstadoSchema>;

export const programaExplorarItemSchema = z.object({
  id: z.string().uuid(),
  titulo: z.string(),
  artistaNombre: z.string(),
  tipoPromoNombre: z.string(),
  importeComisionPorcentaje: z.number().nullable(),
  importeComisionFija: z.number().nullable(),
  monedaNombre: z.string(),
  numeroTareas: z.number().int(),
  campaniaTitulo: z.string().nullable(),
  fechaInicio: z.string().nullable(),
  fechaFin: z.string().nullable(),
  miEstado: miEstadoSchema,
});

export const miInscripcionSchema = z.object({
  id: z.string().uuid(),
  programaId: z.string().uuid(),
  programaTitulo: z.string(),
  artistaNombre: z.string(),
  esAprobado: z.boolean(),
  esBloqueado: z.boolean(),
  codigoReferido: z.string().nullable(),
  urlTrackingPersonalizada: z.string().nullable(),
  fechaAlta: z.string(),
});

export type ProgramaExplorarItem = z.infer<typeof programaExplorarItemSchema>;
export type MiInscripcion = z.infer<typeof miInscripcionSchema>;
```

---

## Checklist UI/UX

### Pantalla 1: Explorar Programas (`/crowdpromotion/explorar`)
- [ ] Layout de grid responsivo (1/2/3 columnas)
- [ ] Filtros: busqueda artista, tipo, rango comision
- [ ] Card de programa con todos los datos especificados
- [ ] Elemento de accion segun estado (`miEstado`): boton / badge pendiente / badge aprobado / mensaje bloqueado
- [ ] Estado loading con skeletons
- [ ] Empty state sin programas
- [ ] Empty state con filtros sin resultado
- [ ] Banner si el promotor no tiene perfil activo
- [ ] Paginacion funcional
- [ ] Toast en solicitud exitosa / error
- [ ] Optimistic update del estado de la card tras solicitar

### Pantalla 2: Mis Programas (`/promotor/mis-programas`)
- [ ] Lista de inscripciones del promotor
- [ ] Filtro por estado (Todos / Aprobado / Pendiente / Bloqueado)
- [ ] Item aprobado: codigo referido + URL tracking con botones copiar
- [ ] Item pendiente: mensaje de espera
- [ ] Item bloqueado: mensaje de bloqueo
- [ ] Copy to clipboard con feedback visual (icono cambia a check)
- [ ] Empty state con boton "Explorar programas"
- [ ] Estado loading

### Pantalla 3: Detalle Mi Programa (seccion expandible)
- [ ] Toggle expandir/colapsar con animacion
- [ ] Mini KPIs (clics, conversiones, comision total) con valores 0 para MVP
- [ ] Lista de tareas del programa
- [ ] Mensaje "Estadisticas en tiempo real (proximamente)"

### Pantalla 4: Solicitudes Pendientes (Tab admin)
- [ ] Tab "Solicitudes" con badge contador de pendientes
- [ ] Card de solicitud con: avatar, nombre, tipo, redes sociales, fecha relativa
- [ ] Boton Aprobar con loading state y feedback
- [ ] Boton Rechazar con dialogo de confirmacion
- [ ] Boton Bloquear con dialogo de confirmacion destructivo
- [ ] Fade-out de item tras accion exitosa
- [ ] Actualizacion de KPI cards del programa
- [ ] Empty state sin solicitudes
- [ ] Toast para cada accion

### Pantalla 5: Promotores Aprobados (Tab admin)
- [ ] Lista de aprobados con: avatar, nombre, tipo, codigo referido, stats, fecha aprobacion
- [ ] Boton "Dar de baja" con dialogo de confirmacion destructivo
- [ ] Fade-out de item tras dar de baja exitoso
- [ ] Actualizacion de KPI "Aprobados"
- [ ] Empty state

### Pantalla 6: Promotores Bloqueados (Tab admin)
- [ ] Lista de bloqueados (solo lectura)
- [ ] Avatar con fondo rojo/oscuro
- [ ] Badge "BLOQUEADO" visible
- [ ] Fecha de bloqueo
- [ ] Aviso informativo sobre re-solicitud
- [ ] Empty state
