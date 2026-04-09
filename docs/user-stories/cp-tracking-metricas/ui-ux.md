# UI/UX: Tracking de Eventos, Conversiones y Dashboard de Metricas

> **Feature:** cp-tracking-metricas
> **User Story:** US-CP-05
> **Ultima actualizacion:** 2026-03-02

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto y los diagramas ASCII de US-CP-05.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron sidebar + KPI cards + dark theme |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing | Header publico, paleta de colores pink/purple |

**Observaciones del mockup WPR_5-Dashboard-Artist.png aplicables a esta feature:**
- KPI cards con valor numerico grande (`text-3xl font-bold`), etiqueta de KPI en `text-sm text-[#94a3b8]`
- Sidebar oscuro `bg-[#0d0d1a]` con navegacion lateral y avatar de artista
- Tabs de seccion dentro de una pagina de detalle (patron aplicable a la pestana Metricas dentro del detalle de programa)
- Tablas con filas con hover `bg-[#1e1e38]`, texto en columnas con colores de estado

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Dashtail** | Dashboard metricas del programa (artista, admin) | `references/templates/dashtail/` |
| **Krowd** | Dashboard metricas del promotor (landing publica autenticada) | `references/templates/krowd/` |

**Componentes de referencia clave:**
- Layout dashboard: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/layout.tsx`
- KPI cards: patron de cards con icono + valor numerico grande del dashboard Dashtail
- Tablas de datos: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/`
- Selector de programa (promotor): patron de `<Select>` de shadcn con lista de programas activos

---

## Design Tokens

### Paleta de Colores

Reutilizados de US-CP-01 a US-CP-04. Se agregan tokens especificos para tipos de eventos de tracking.

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

  /* Tipos de evento de tracking (NUEVOS para US-CP-05) */
  --event-click-bg: rgba(59, 130, 246, 0.1);     /* Azul - clicks */
  --event-click-text: #3b82f6;
  --event-click-border: rgba(59, 130, 246, 0.3);

  --event-pageview-bg: rgba(100, 116, 139, 0.1);  /* Gris - page views */
  --event-pageview-text: #94a3b8;
  --event-pageview-border: rgba(100, 116, 139, 0.3);

  --event-signup-bg: rgba(16, 185, 129, 0.1);     /* Verde - signups */
  --event-signup-text: #10b981;
  --event-signup-border: rgba(16, 185, 129, 0.3);

  --event-backing-bg: rgba(168, 85, 247, 0.1);    /* Purpura - backings (conversiones) */
  --event-backing-text: #a855f7;
  --event-backing-border: rgba(168, 85, 247, 0.3);

  --event-share-bg: rgba(245, 158, 11, 0.1);      /* Ambar - shares */
  --event-share-text: #f59e0b;
  --event-share-border: rgba(245, 158, 11, 0.3);

  /* KPI values - colores de metricas */
  --kpi-clicks: #3b82f6;        /* Azul para clicks */
  --kpi-signups: #10b981;       /* Verde para signups */
  --kpi-conversiones: #a855f7;  /* Purpura para conversiones */
  --kpi-valor: #f59e0b;         /* Ambar para valor monetario */
  --kpi-comision: #10b981;      /* Verde para comisiones */
  --kpi-tasa: #94a3b8;          /* Gris para tasa de conversion */

  /* Estado general */
  --status-success: #10b981;
  --status-warning: #f59e0b;
  --status-error: #ef4444;
  --status-info: #3b82f6;

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Colores de lineas del grafico temporal */
  --chart-clicks: #3b82f6;
  --chart-pageviews: #64748b;
  --chart-signups: #10b981;
  --chart-conversiones: #a855f7;
  --chart-grid: rgba(51, 65, 85, 0.5);
  --chart-tooltip-bg: #1e1e38;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - etiquetas auxiliares, hints, fechas */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario, celdas de tabla */
  --text-base: 1rem;     /* 16px - texto de inputs, contenido de cards */
  --text-lg: 1.125rem;   /* 18px - subtitulos de seccion */
  --text-xl: 1.25rem;    /* 20px - titulos de card */
  --text-2xl: 1.5rem;    /* 24px - titulo de pagina landing */
  --text-3xl: 1.875rem;  /* 30px - valor de KPI grande */
  --text-4xl: 2.25rem;   /* 36px - KPI hero (valor destacado) */

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

## Componente Invisible: Tracking Interceptor

**Proyecto:** Landing (Vite + React 18)
**Tipo:** Hook sin renderizado (`useTrackingInterceptor`)
**Montaje:** En el componente raiz `App.tsx`, ejecuta una sola vez al cargar la SPA

Este componente no tiene interfaz visual. Se ejecuta en background al cargar la Landing para detectar parametros de tracking en la URL, persistirlos en sesion y reportar el evento Click al API.

### Logica del Hook

```
Al montar la aplicacion (useEffect una vez):
  1. Leer window.location.search
  2. Extraer: ref, utm_source, utm_medium, utm_campaign
  3. Si existe param "ref":
     a. Guardar en sessionStorage: { ref, utm_source, utm_medium, utm_campaign, campaniaCrowdfundingId }
     b. Guardar en cookie "wp_ref" (TTL 30 min, SameSite=Lax)
     c. Llamar POST /api/crowdpromotion/tracking/evento con:
        - tipoEventoPromoId = 1 (Click)
        - codigoReferido = ref
        - urlOrigen = document.referrer || window.location.href
        - urlReferer = document.referrer
        - utmSource, utmMedium, utmCampaign
  4. Si "ref" NO existe pero hay datos en sessionStorage/cookie:
     - Recuperar ref de sesion para atribucion de eventos posteriores
  5. Ignorar errores silenciosamente (no mostrar nada al usuario)
     - Si API devuelve 429, no reintentar
     - Si API devuelve 400, descartar y limpiar sessionStorage
```

### Persistencia de Datos de Sesion

| Dato | Almacenamiento | Clave | TTL |
|------|----------------|-------|-----|
| Codigo referido | `sessionStorage` + Cookie | `wp_ref` | Cookie: 30 min, sessionStorage: hasta cerrar tab |
| UTM source | `sessionStorage` | `wp_utm_source` | sesion |
| UTM medium | `sessionStorage` | `wp_utm_medium` | sesion |
| UTM campaign | `sessionStorage` | `wp_utm_campaign` | sesion |
| ID campana | `sessionStorage` | `wp_campania_id` | sesion |

### Especificaciones del Hook

| Elemento | Detalle |
|----------|---------|
| Nombre del hook | `useTrackingInterceptor` |
| Ubicacion | `src/web/src/features/tracking/hooks/useTrackingInterceptor.ts` |
| Parametros de entrada | Ninguno (lee directamente de `window.location.search`) |
| Retorno | `void` |
| Dependencias externas | `trackingService.registrarEvento()` |
| Manejo de errores | Silencioso (try/catch sin UI) |
| Rate limit cliente | No reintentar en la misma sesion si recibe 429 |

---

## Pantalla 1: Dashboard de Metricas del Promotor

**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/metricas`
**Template base:** Krowd (paginas publicas con header de landing)

**Precondicion:** Usuario autenticado con perfil de promotor aprobado en al menos un programa. Si no tiene programas, mostrar empty state con enlace a `/promotor/mis-programas`.

**API call al montar:** `GET /api/crowdpromotion/promotor/metricas?programaId={id}` (con el programa seleccionado)
**API call programas:** `GET /api/crowdpromotion/promotor/mis-programas` (para poblar el selector)

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (h-16, bg-[#0d0d1a], border-b border-[#334155])      │
│ [Logo WePlay Rises]   Explorar  Artistas  [CrowdPromotion]  [Avatar]│
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CABECERA (max-w-4xl mx-auto px-4, pt-8 pb-4)                       │
│  Mis metricas de promocion                                          │
│  Programa: [Select dropdown con programas activos        v]         │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  KPI CARDS (max-w-4xl mx-auto px-4, grid grid-cols-3 gap-4)         │
│                                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              │
│  │ Mis clicks   │  │ Conversiones │  │ Comision     │              │
│  │    450       │  │      5       │  │  50 EUR      │              │
│  │  [icono]     │  │  [icono]     │  │  [icono]     │              │
│  └──────────────┘  └──────────────┘  └──────────────┘              │
│                                                                     │
│  Tasa de conversion: 1.11%          (texto inline)                  │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ENLACE REFERIDO (max-w-4xl mx-auto px-4, mb-6)                     │
│                                                                     │
│  Mi enlace de promocion                                             │
│  ┌──────────────────────────────────────────────────┐ [Copiar]     │
│  │ https://weplay.com/campanias/xxx?ref=album-2026  │              │
│  └──────────────────────────────────────────────────┘              │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  EVENTOS RECIENTES (max-w-4xl mx-auto px-4, pb-10)                  │
│                                                                     │
│  Eventos recientes                              [Ver todos ->]      │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ [Backing badge]  100 EUR -> 10 EUR comision                  │   │
│  │ 20 mar 2026, 14:30                                           │   │
│  ├──────────────────────────────────────────────────────────────┤   │
│  │ [Click badge]  Click desde instagram.com                     │   │
│  │ 20 mar 2026, 12:15                                           │   │
│  ├──────────────────────────────────────────────────────────────┤   │
│  │ [PageView badge]  Visita a pagina de campana                 │   │
│  │ 19 mar 2026, 18:42                                           │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 1

**Cabecera de pagina**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo | `<h1>` | `text-2xl font-bold text-white` |
| Label selector | `<label>` | `text-sm text-[#94a3b8] mr-3` / "Programa:" |
| Selector de programa | `<Select>` | `w-64 bg-[#0f0f1f] border-[#334155] text-white text-sm focus:border-[#a855f7]` |
| Opcion en selector | `<SelectItem>` | `text-sm text-white` - muestra `programa.nombrePrograma` |
| Fila cabecera | `<div>` | `flex flex-col sm:flex-row sm:items-center gap-3 mb-8` |

**KPI Cards (grid de 3 columnas)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid contenedor | `<div>` | `grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6` |
| Card KPI individual | `<Card>` | `bg-[#151525] border-[#334155] p-5` |
| Icono del KPI | `<div>` | `w-10 h-10 rounded-lg flex items-center justify-center mb-3` con fondo de color especifico del KPI |
| Icono Click | `<MousePointerClick w-5 h-5>` | `text-[#3b82f6]` dentro de `bg-blue-950/50` |
| Icono Conversion | `<ShoppingCart w-5 h-5>` | `text-[#a855f7]` dentro de `bg-purple-950/50` |
| Icono Comision | `<Wallet w-5 h-5>` | `text-[#10b981]` dentro de `bg-green-950/50` |
| Valor KPI | `<p>` | `text-3xl font-bold text-white` |
| Label KPI | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Valor monetario KPI | `<p>` | `text-3xl font-bold text-[#f59e0b]` (solo para KPI de comision) |
| Sufijo moneda | `<span>` | `text-lg text-[#64748b] ml-1` |

**Tasa de conversion (inline bajo las KPI cards)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor | `<div>` | `flex items-center gap-2 mb-6 text-sm` |
| Icono | `<TrendingUp w-4 h-4>` | `text-[#94a3b8]` |
| Label | `<span>` | `text-[#94a3b8]` / "Tasa de conversion:" |
| Valor | `<span>` | `font-semibold text-white` / "1.11%" |
| Nota | `<span>` | `text-xs text-[#64748b]` / "(conversiones / clicks)" |

**Seccion enlace referido**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] p-5 mb-6` |
| Label titulo | `<p>` | `text-sm font-medium text-[#94a3b8] mb-3` / "Mi enlace de promocion" |
| Fila enlace + boton | `<div>` | `flex items-center gap-2` |
| Input URL (solo lectura) | `<Input readOnly>` | `bg-[#0f0f1f] border-[#334155] text-[#94a3b8] text-sm flex-1 font-mono cursor-default` / Muestra la URL completa con `?ref=...` |
| Boton copiar (estado normal) | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-[#a855f7] shrink-0` con icono `<Copy w-4 h-4 mr-1.5>` / "Copiar" |
| Boton copiar (estado copiado) | `<Button variant="outline" size="sm">` | `border-green-800/50 text-[#10b981]` con icono `<Check w-4 h-4 mr-1.5>` / "Copiado!" - Vuelve a estado normal tras 2000ms |

**Seccion eventos recientes**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155]` |
| Cabecera de seccion | `<div>` | `flex items-center justify-between p-5 border-b border-[#334155]` |
| Titulo "Eventos recientes" | `<h3>` | `text-base font-semibold text-white` |
| Link "Ver todos" | `<Button variant="ghost" size="sm">` | `text-[#a855f7] hover:text-[#c084fc] text-xs` con icono `<ChevronRight w-3.5 h-3.5>` a la derecha |
| Lista de eventos | `<div>` | `divide-y divide-[#1e1e38]` |
| Fila de evento | `<div>` | `flex items-center gap-3 px-5 py-3.5 hover:bg-[#1e1e38] transition-colors` |
| Badge tipo evento | `<Badge>` | Ver tabla de badges de evento mas abajo |
| Descripcion evento | `<p>` | `text-sm text-white flex-1` |
| Detalle conversion | `<p>` | `text-xs text-[#94a3b8]` / "100 EUR - 10 EUR comision" (solo para tipo Backing) |
| Fecha y hora | `<span>` | `text-xs text-[#64748b] shrink-0` / "20 mar 2026, 14:30" |

**Badges por tipo de evento**

| Tipo | Componente | Estilos / Props |
|------|------------|-----------------|
| Click | `<Badge>` | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs` / "Click" |
| PageView | `<Badge>` | `bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs` / "Vista" |
| Signup | `<Badge>` | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs` / "Registro" |
| Backing | `<Badge>` | `bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs` / "Backing" |
| Share | `<Badge>` | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs` / "Share" |

### Estados de UI Pantalla 1

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | 3 KPI cards skeleton `bg-[#1e1e38] rounded-xl h-24 animate-pulse`. Bloque enlace skeleton `h-16`. Lista de 3 filas skeleton `h-12` con `animate-pulse` |
| **Default con datos** | KPI cards con valores reales. Enlace generado visible. Lista de eventos recientes con ultimos 10 eventos |
| **Empty state (sin programas)** | Icono `<BarChart2 w-12 h-12>` centrado con gradiente, titulo "Sin programas activos", subtitulo "Inscribete en un programa de promocion para ver tus metricas", boton gradient `<Button>` / "Ver programas disponibles" que navega a `/promotor/mis-programas` |
| **Empty state (sin eventos en programa)** | Seccion eventos muestra icono `<Activity w-8 h-8 text-[#64748b]>` centrado, texto "Aun no hay eventos registrados. Comparte tu enlace de promocion para empezar." |
| **Cambio de programa** | Al seleccionar otro programa en el `<Select>`, los KPI cards y eventos muestran skeleton de carga mientras se obtienen nuevos datos |
| **Error de carga** | Card centrado con icono `<AlertCircle w-8 h-8 text-red-400>`, "No se pudieron cargar las metricas", boton "Reintentar" `<Button variant="outline" size="sm">` |
| **Copiando enlace** | Boton "Copiar" deshabilitado durante 300ms. Luego muestra "Copiado!" con icono check verde durante 2000ms. Sin toast (feedback en el boton) |

### Toast Messages Pantalla 1

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Enlace copiado (fallback si Clipboard API falla) | Success (verde) | "Enlace copiado al portapapeles" |
| Error al copiar (navegador sin soporte) | Destructive (rojo) | "No se pudo copiar. Copia el enlace manualmente." |

### Interacciones Pantalla 1

| Accion | Comportamiento |
|--------|----------------|
| Seleccionar programa en `<Select>` | Refetch de metricas del nuevo programa. Actualizar URL query param `?programaId=xxx` sin navegar (pushState) |
| Click "Copiar" en enlace | Llama `navigator.clipboard.writeText(url)`. Boton cambia a "Copiado!" durante 2000ms |
| Hover fila de evento | `bg-[#1e1e38]` con `transition-colors duration-150` |
| Click "Ver todos" | Navegar a `/promotor/eventos?programaId=xxx` (pagina de historial completo, fuera del scope MVP si no hay tiempo) |
| Scroll vertical | La pagina hace scroll completo. Header fijo en la parte superior |

---

## Pantalla 2: Dashboard de Metricas del Programa (Artista)

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/campanias/{campanaId}/crowdpromotion/programas/{programaId}/metricas`
**Template base:** Dashtail (dashboard artista con sidebar)
**Acceso:** Pestana "Metricas" dentro de la pagina de detalle del programa de promocion

**Precondicion:** Artista autenticado propietario del programa. Si no es el propietario, devolver 403.

**API call al montar:** `GET /api/crowdpromotion/programas/{programaId}/metricas`
**API call con filtro fechas:** `GET /api/crowdpromotion/programas/{programaId}/metricas?fechaDesde={date}&fechaHasta={date}`

### Layout

```
┌────────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a], border-r border-[#334155], fixed)         │
│ [Logo]                                                                 │
│ ─────                                                                  │
│ Dashboard     /dashboard                                               │
│ Campanias     /campanias       (activo, con sub-nav)                   │
│ Perfil        /perfil                                                  │
│ CrowdPromotion /campanias/.../crowdpromotion                           │
│ ─────                                                                  │
│ [Avatar artista]  Nombre artista                                       │
├────────────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT (ml-64, bg-[#1a1a2e], min-h-screen)                       │
│                                                                        │
│  BREADCRUMB + CABECERA (px-8 pt-8 pb-4)                                │
│  Campanias > Album 2026 > CrowdPromotion > Metricas                    │
│                                                                        │
│  Metricas: Promociona mi nuevo album                                   │
│                                                                        │
│  FILTRO FECHAS (px-8 mb-6)                                             │
│  Periodo:  [01/03/2026]  a  [31/05/2026]   [Aplicar]                  │
│                                                                        │
│  ────────────────────────────────────────────────────────────────────  │
│                                                                        │
│  KPI CARDS (px-8, grid grid-cols-4 gap-4 mb-6)                         │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐                 │
│  │  1,250   │ │    45    │ │    12    │ │ 1,200EUR │                 │
│  │  clicks  │ │ signups  │ │ backings │ │  valor   │                 │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘                 │
│                                                                        │
│  ┌─────────────────────────┐  ┌──────────────────────────────┐        │
│  │  Tasa de conversion     │  │  Comisiones pagadas          │        │
│  │        0.96%            │  │        120 EUR               │        │
│  └─────────────────────────┘  └──────────────────────────────┘        │
│                                                                        │
│  ────────────────────────────────────────────────────────────────────  │
│                                                                        │
│  RANKING PROMOTORES + GRAFICO (px-8, grid grid-cols-5 gap-6 mb-8)     │
│  ┌─────────────────────────────────────┐  ┌──────────────────────┐   │
│  │ RANKING (col-span-3)                │  │ DESGLOSE (col-span-2)│   │
│  │─────────────────────────────────────│  │──────────────────────│   │
│  │ # │ Promotor │ Clicks │ Conv │ Valor│  │ Clicks        1,250  │   │
│  │─────────────────────────────────────│  │ Page views      890  │   │
│  │ 1 │ DJ Mark  │   450  │   5  │500EUR│  │ Signups          45  │   │
│  │ 2 │ MusicBlog│   380  │   4  │400EUR│  │ Conversiones     12  │   │
│  │ 3 │ FanLuna  │   300  │   3  │300EUR│  │ Shares            8  │   │
│  └─────────────────────────────────────┘  └──────────────────────┘   │
│                                                                        │
│  GRAFICO TEMPORAL (px-8, mb-10)                                        │
│  Eventos por dia                                                       │
│  ┌──────────────────────────────────────────────────────────────────┐ │
│  │  |                                                               │ │
│  │  |    *                           (lineas de colores por tipo)   │ │
│  │  |   * *    *                                                    │ │
│  │  |  *   *  * *   *                                               │ │
│  │  | *     **   * * *                                              │ │
│  │  +──────+──────+──────+──────+──────+──────>                    │ │
│  │  1 Mar  8 Mar  15 Mar 22 Mar 29 Mar  5 Abr                      │ │
│  │  [Clicks] [Signups] [Conversiones]  (leyenda)                   │ │
│  └──────────────────────────────────────────────────────────────────┘ │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 2

**Breadcrumb**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor | `<nav>` | `flex items-center gap-1.5 text-xs text-[#64748b] mb-4` |
| Link "Campanias" | `<Link href="/campanias">` | `hover:text-[#94a3b8] transition-colors` |
| Separador | `<ChevronRight w-3 h-3>` | `text-[#334155]` |
| Link campana | `<Link href="/campanias/{id}">` | `hover:text-[#94a3b8] transition-colors` (truncado a 20 chars) |
| Separador | `<ChevronRight w-3 h-3>` | `text-[#334155]` |
| Link "CrowdPromotion" | `<Link href="...">` | `hover:text-[#94a3b8] transition-colors` |
| Separador | `<ChevronRight w-3 h-3>` | `text-[#334155]` |
| "Metricas" (activo) | `<span>` | `text-[#94a3b8]` |

**Cabecera y filtro de fechas**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo | `<h1>` | `text-2xl font-bold text-white mb-6` |
| Contenedor filtro | `<div>` | `flex items-center gap-3 mb-8 bg-[#151525] border border-[#334155] rounded-xl p-4` |
| Label "Periodo:" | `<label>` | `text-sm text-[#94a3b8] mr-1` |
| Input fecha desde | `<Input type="date">` | `bg-[#0f0f1f] border-[#334155] text-white text-sm w-36 [color-scheme:dark]` |
| Separador "a" | `<span>` | `text-sm text-[#64748b]` |
| Input fecha hasta | `<Input type="date">` | Mismos estilos que fecha desde |
| Boton "Aplicar" | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white text-sm px-4` |
| Boton "Limpiar" | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-[#94a3b8] text-sm` (visible solo cuando hay filtro activo) |

**KPI Cards - fila de 4**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid 4 columnas | `<div>` | `grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4` |
| Card KPI | `<Card>` | `bg-[#151525] border-[#334155] p-5` |
| Icono contenedor | `<div>` | `w-10 h-10 rounded-lg flex items-center justify-center mb-3` |
| Icono Clicks | `<MousePointerClick w-5 h-5>` | `text-[#3b82f6]` en `bg-blue-950/50` |
| Icono Signups | `<UserPlus w-5 h-5>` | `text-[#10b981]` en `bg-green-950/50` |
| Icono Backings | `<ShoppingCart w-5 h-5>` | `text-[#a855f7]` en `bg-purple-950/50` |
| Icono Valor | `<Euro w-5 h-5>` | `text-[#f59e0b]` en `bg-amber-950/50` |
| Valor KPI numerico | `<p>` | `text-3xl font-bold text-white` (Tailwind `tabular-nums`) |
| Valor KPI monetario | `<p>` | `text-3xl font-bold text-[#f59e0b]` (con sufijo `<span className="text-lg ml-1 text-[#64748b]">EUR</span>`) |
| Label KPI | `<p>` | `text-sm text-[#94a3b8] mt-1` |

**KPI Cards - fila de 2 (tasa + comisiones)**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid 2 columnas | `<div>` | `grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8` |
| Card Tasa conversion | `<Card>` | `bg-[#151525] border-[#334155] p-5 flex items-center gap-4` |
| Icono Tasa | `<TrendingUp w-5 h-5>` | `text-[#94a3b8]` en `bg-[#1e1e38] w-10 h-10 rounded-lg` |
| Label | `<p>` | `text-sm text-[#94a3b8]` / "Tasa de conversion" |
| Valor tasa | `<p>` | `text-2xl font-bold text-white` / "0.96%" |
| Nota | `<p>` | `text-xs text-[#64748b]` / "conversiones / clicks" |
| Card Comisiones | `<Card>` | Misma estructura con icono `<Wallet w-5 h-5>` |

**Tabla ranking de promotores**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] col-span-3` |
| Cabecera de card | `<CardHeader>` | `border-b border-[#334155] px-5 py-4` |
| Titulo | `<CardTitle>` | `text-base font-semibold text-white` / "Ranking de Promotores" |
| Tabla | `<Table>` | `w-full` |
| Cabecera tabla | `<TableHeader>` | `[&_tr]:border-b [&_tr]:border-[#334155]` |
| Celda cabecera | `<TableHead>` | `text-xs font-medium text-[#64748b] uppercase tracking-wide px-4 py-3` |
| Fila de tabla | `<TableRow>` | `border-b border-[#1e1e38] hover:bg-[#1e1e38] transition-colors` |
| Celda posicion | `<TableCell>` | `text-sm font-bold text-[#64748b] w-10 pl-4` / "#1", "#2", etc. |
| Avatar promotor | `<Avatar className="w-7 h-7">` | Con fallback inicial del nombre. `text-xs` |
| Nombre promotor | `<TableCell>` | `text-sm font-medium text-white` |
| Badge tipo promotor | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs ml-1.5` |
| Celda clicks | `<TableCell>` | `text-sm text-[#3b82f6] font-medium tabular-nums` |
| Celda conversiones | `<TableCell>` | `text-sm text-[#a855f7] font-medium tabular-nums` |
| Celda valor generado | `<TableCell>` | `text-sm text-[#f59e0b] font-medium tabular-nums` |
| Celda comision | `<TableCell>` | `text-sm text-[#10b981] font-medium tabular-nums` |

**Columnas de la tabla (cabeceras con ordenacion)**

| Columna | Tipo | Ordenable | Default |
|---------|------|-----------|---------|
| # | Posicion (1-N) | No | - |
| Promotor | Nombre + badge tipo | No | - |
| Clicks | Numero | Si | Desc |
| Conv. | Numero | Si | Desc |
| Valor | Monetario EUR | Si | Desc |
| Comision | Monetario EUR | Si | - |

Ordenacion: click en cabecera alterna ASC/DESC. Cabecera activa muestra icono `<ArrowUpDown w-3.5 h-3.5>` en color `#a855f7`.

**Panel de desglose por tipo de evento**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] col-span-2` |
| Titulo | `<CardTitle>` | `text-base font-semibold text-white px-5 pt-4` / "Desglose de Eventos" |
| Lista de tipos | `<div>` | `px-5 py-4 flex flex-col gap-3` |
| Fila tipo evento | `<div>` | `flex items-center justify-between` |
| Indicador de color | `<div>` | `w-3 h-3 rounded-full shrink-0` con color del tipo (`bg-[#3b82f6]`, etc.) |
| Nombre tipo | `<span>` | `text-sm text-[#94a3b8] flex-1 ml-2` |
| Barra de progreso | `<div>` | `flex-1 mx-3 h-1.5 bg-[#1e1e38] rounded-full overflow-hidden` |
| Relleno barra | `<div>` | `h-full rounded-full` con background del color del tipo y `width` calculado como porcentaje del total de eventos |
| Cantidad | `<span>` | `text-sm font-semibold text-white tabular-nums w-10 text-right` |

**Grafico temporal de eventos**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] p-5` |
| Titulo | `<h3>` | `text-base font-semibold text-white mb-4` / "Eventos por dia" |
| Contenedor del grafico | `<div>` | `h-64 w-full` |
| Libreria | `<LineChart>` de **Recharts** | `data={eventosPorDia}` |
| Eje X | `<XAxis dataKey="fecha">` | `tick={{ fill: '#64748b', fontSize: 11 }}`, formato `DD/MM` |
| Eje Y | `<YAxis>` | `tick={{ fill: '#64748b', fontSize: 11 }}` |
| Grid | `<CartesianGrid>` | `stroke="#334155" strokeDasharray="3 3" opacity={0.5}` |
| Linea Clicks | `<Line dataKey="clicks">` | `stroke="#3b82f6" strokeWidth={2} dot={false}` |
| Linea Signups | `<Line dataKey="signups">` | `stroke="#10b981" strokeWidth={2} dot={false}` |
| Linea Conversiones | `<Line dataKey="conversiones">` | `stroke="#a855f7" strokeWidth={2} dot={false}` |
| Tooltip | `<Tooltip>` | `contentStyle={{ background: '#1e1e38', border: '1px solid #334155', borderRadius: '8px', color: '#fff' }}` |
| Leyenda | `<Legend>` | `wrapperStyle={{ fontSize: '12px', color: '#94a3b8' }}` |

**Nota de implementacion del grafico:** PageViews no se muestra en el grafico por defecto (demasiado volumen vs los otros tipos). Se puede agregar con un toggle si el artista lo requiere.

### Estados de UI Pantalla 2

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | 4 KPI cards skeleton `h-24 bg-[#1e1e38] animate-pulse`. 2 KPI cards secundarias skeleton `h-16`. Tabla skeleton con 5 filas `h-10`. Grafico skeleton `h-64 bg-[#1e1e38] rounded-lg animate-pulse` |
| **Loading con filtro** | Mismos skeletons. El filtro de fechas permanece visible y editable. Boton "Aplicar" muestra `<Loader2 w-4 h-4 animate-spin>` |
| **Default con datos** | Todos los componentes rellenos con datos reales. Ranking ordenado por conversiones desc por defecto |
| **Empty state (sin eventos)** | KPI cards con valores en 0. Tabla ranking muestra "Sin promotores con actividad en este periodo". Grafico muestra ejes vacios con texto "No hay datos para el periodo seleccionado" centrado en el area del grafico |
| **Error de carga** | Card de error centrado con icono `<AlertCircle w-8 h-8 text-red-400>`, "No se pudieron cargar las metricas", boton "Reintentar" |
| **Filtro aplicado** | Badge visible junto al selector de fechas: "Filtrando: 01 Mar - 31 May 2026" con icono `<X w-3.5 h-3.5>` para limpiar filtro |
| **Ordenacion de tabla activa** | Columna activa con cabecera en `text-[#a855f7]` e icono de ordenacion visible |

### Validacion del Filtro de Fechas

| Campo | Validacion | Comportamiento |
|-------|-----------|----------------|
| Fecha desde | No puede ser mayor que fecha hasta | Si el usuario ingresa fecha desde > fecha hasta, deshabilitar boton "Aplicar" y mostrar mensaje de error inline `text-xs text-[#ef4444]` bajo el campo |
| Fecha hasta | No puede ser en el futuro (mas de hoy) | Limitar `max` del input a fecha de hoy |
| Rango maximo | No mas de 365 dias | Si supera, mostrar aviso informativo (no bloquear) |

### Interacciones Pantalla 2

| Accion | Comportamiento |
|--------|----------------|
| Click "Aplicar" en filtro fechas | Refetch de metricas con `?fechaDesde=&fechaHasta=`. Actualizar URL query params |
| Click en cabecera de columna del ranking | Alternar orden ASC/DESC de la columna. Solo una columna activa a la vez |
| Hover sobre punto del grafico | Tooltip con valores del dia para todos los tipos de evento |
| Click en fila del ranking | Expandir inline (o navegar) para ver el detalle completo del promotor (fuera de scope MVP - disabled) |
| Click "Limpiar" en filtro | Quitar fechaDesde y fechaHasta de la URL. Refetch sin filtro. Ocultar boton "Limpiar" |

---

## Responsive Breakpoints

### Pantalla 1: Dashboard Promotor (Landing)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | KPI cards en columna unica (`grid-cols-1`). Selector de programa ocupa ancho completo. Input de enlace referido se trunca con `truncate`. Cabecera de pagina con `text-xl` en lugar de `text-2xl` |
| Tablet | 640-1024px | KPI cards en 3 columnas (`grid-cols-3`). El enlace referido visible completo |
| Desktop | > 1024px | Layout completo. Contenido centrado con `max-w-4xl mx-auto` |

### Pantalla 2: Dashboard Artista (Admin)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 768px | Sidebar colapsado (hamburger menu). KPI cards en 2 columnas. Ranking y desglose apilados verticalmente (`grid-cols-1`). Grafico con altura reducida a `h-48`. Tabla de ranking muestra solo columnas: Promotor, Clicks, Conv |
| Tablet | 768-1024px | Sidebar fijo visible. KPI cards en 2 columnas superiores, 2 inferiores. Ranking y desglose en 2 columnas (`grid-cols-2` + `grid-cols-2` dentro) |
| Desktop | > 1024px | Layout completo: 4 KPI cards superiores, 2 secundarias, ranking 3 cols + desglose 2 cols, grafico completo |

---

## Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| KPI card al cargar datos | Fade in + slide up `translateY(8px) -> 0` | 300ms | Cuando datos llegan (reemplaza skeleton) |
| Boton "Copiar" -> "Copiado!" | Cambio de texto e icono | 150ms | Click en boton copiar |
| Cambio de orden en tabla | Las filas se re-ordenan con `transition-all duration-200` | 200ms | Click en cabecera ordenable |
| Barra de progreso en desglose | Animacion de ancho `0 -> valor` | 500ms con `ease-out` | Al montar el componente con datos |
| Skeleton -> contenido | Fade in `opacity-0 -> opacity-100` | 200ms | Al resolver la query |
| Hover sobre card | `border-color` transition | 150ms | Hover |
| Hover sobre fila de evento | `background-color` transition | 150ms | Hover |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1 para texto normal. Valores de KPI en blanco sobre `#151525` cumplen AAA |
| Contraste de badges | Colores de evento cumplen minimo 3:1 sobre fondo oscuro de la card |
| Focus ring | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en todos los elementos interactivos |
| Labels de formulario | Todos los `<Input>` y `<Select>` tienen `<label>` asociado via `htmlFor` o `aria-label` |
| Tabla de ranking | `<thead>` con `<th scope="col">`. Cabeceras ordenables tienen `aria-sort="ascending"/"descending"` |
| Grafico temporal | Alternativa textual en `<caption>` de la tabla o boton "Ver datos en tabla" que muestra los datos del grafico en formato de tabla accesible |
| Selector de programa | `<Select>` de shadcn incluye aria-label="Selecciona un programa de promocion" |
| Input readonly URL | `aria-label="Tu enlace de promocion"`, `aria-readonly="true"` |
| Boton copiar | `aria-label` dinamico: "Copiar enlace de promocion" / "Enlace copiado" |
| Skeletons | `aria-busy="true"` en contenedor mientras carga |
| Mensajes de error | `role="alert"` en contenedores de error para anuncio a lectores de pantalla |

---

## Checklist UI/UX

### Componente Tracking Interceptor (invisible)
- [ ] Hook `useTrackingInterceptor` implementado en `src/web/src/features/tracking/hooks/`
- [ ] Lee `ref`, `utm_source`, `utm_medium`, `utm_campaign` de la URL
- [ ] Persiste datos en `sessionStorage` y cookie `wp_ref`
- [ ] Llama a `POST /api/crowdpromotion/tracking/evento` con tipo Click
- [ ] Maneja 429 sin reintentar
- [ ] Maneja 400 limpiando sessionStorage
- [ ] No tiene interfaz visual ni bloquea el render
- [ ] Montado en `App.tsx` una sola vez

### Pantalla 1: Dashboard Promotor (Landing)
- [ ] Layout con header de landing implementado
- [ ] Selector de programa (`<Select>`) cargado dinamicamente
- [ ] 3 KPI cards: Clicks, Conversiones, Comision
- [ ] Tasa de conversion mostrada como texto inline
- [ ] Seccion enlace referido con boton copiar funcional
- [ ] Feedback de "Copiado!" durante 2000ms sin toast
- [ ] Lista de eventos recientes con badges por tipo
- [ ] Badges de colores correctos por tipo de evento
- [ ] Estado loading con skeletons
- [ ] Estado empty para sin programas
- [ ] Estado empty para sin eventos
- [ ] Estado error con boton reintentar
- [ ] Responsive: mobile (1 col), tablet/desktop (3 cols KPI)
- [ ] Accesibilidad: labels, focus rings, aria-label en boton copiar

### Pantalla 2: Dashboard Artista (Admin)
- [ ] Layout con sidebar de dashboard
- [ ] Breadcrumb completo hasta "Metricas"
- [ ] Filtro de fechas con inputs date y boton Aplicar
- [ ] Validacion de rango de fechas (desde <= hasta)
- [ ] 4 KPI cards superiores: Clicks, Signups, Backings, Valor
- [ ] 2 KPI cards secundarias: Tasa conversion, Comisiones
- [ ] Tabla de ranking con 6 columnas
- [ ] Ordenacion de tabla por columnas (clicks, conv, valor, comision)
- [ ] Avatar de promotor en tabla con fallback inicial
- [ ] Panel de desglose por tipo con barras de progreso animadas
- [ ] Grafico de lineas temporal (Recharts) con 3 series
- [ ] Tooltip del grafico con estilos dark
- [ ] Estado loading con skeletons para cada seccion
- [ ] Estado empty state para sin datos en el periodo
- [ ] Estado error con boton reintentar
- [ ] Badge de filtro activo con boton para limpiar
- [ ] Responsive: 2 cols en tablet, 4 cols en desktop
- [ ] Accesibilidad: tabla con scope, aria-sort, alternativa textual al grafico
