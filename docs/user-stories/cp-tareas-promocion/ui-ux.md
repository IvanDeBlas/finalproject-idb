# UI/UX: Tareas de Promocion y Validacion

> **Feature:** cp-tareas-promocion
> **User Story:** US-CP-04
> **Ultima actualizacion:** 2026-03-01

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron sidebar + dark theme + cards de lista con acciones por fila |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing | Header publico, cards sobre fondo oscuro, paleta de colores pink/purple |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Mis Tareas (vista promotor, landing publica) | `references/templates/krowd/` |
| **Dashtail** | Tareas Pendientes de Validacion (dashboard artista) | `references/templates/dashtail/` |

**Componentes de referencia clave:**
- Layout dashboard: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/layout.tsx`
- Dialog de confirmacion: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/delete-confirmation-dialog.tsx`
- Tabs del dashboard: patron de WPR_5-Dashboard-Artist.png (pestanas de seccion dentro de una pagina)

---

## Design Tokens

### Paleta de Colores

Reutilizados integramente de US-CP-01, US-CP-02 y US-CP-03. Se agregan tokens especificos para los estados de tareas.

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

  /* Estados de tarea (NUEVOS para US-CP-04) */
  --status-no-completada-bg: rgba(100, 116, 139, 0.1);   /* Fondo badge No completada */
  --status-no-completada-text: #94a3b8;                   /* Texto badge No completada */
  --status-no-completada-border: rgba(100, 116, 139, 0.3);

  --status-completada-bg: rgba(245, 158, 11, 0.1);        /* Fondo badge Completada (pendiente validacion) */
  --status-completada-text: #f59e0b;                       /* Texto badge Completada */
  --status-completada-border: rgba(245, 158, 11, 0.3);

  --status-validada-bg: rgba(16, 185, 129, 0.1);          /* Fondo badge Validada */
  --status-validada-text: #10b981;                         /* Texto badge Validada */
  --status-validada-border: rgba(16, 185, 129, 0.3);

  --status-rechazada-bg: rgba(239, 68, 68, 0.1);          /* Fondo badge Rechazada */
  --status-rechazada-text: #ef4444;                        /* Texto badge Rechazada */
  --status-rechazada-border: rgba(239, 68, 68, 0.3);

  /* Estado general */
  --status-active: #10b981;
  --status-warning: #f59e0b;
  --status-error: #ef4444;
  --status-info: #3b82f6;

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - etiquetas auxiliares, hints, fechas relativas */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario, cuerpo de cards */
  --text-base: 1rem;     /* 16px - texto de inputs, contenido de dialogs */
  --text-lg: 1.125rem;   /* 18px - subtitulos de seccion */
  --text-xl: 1.25rem;    /* 20px - titulos de card / dialog */
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

## Pantalla 1: Mis Tareas (Promotor)

**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/programas/{programaId}/tareas`
**Template base:** Krowd (paginas publicas con header de landing)

**Precondicion:** Usuario autenticado con perfil de promotor aprobado en el programa. Si no esta aprobado, redirigir a `/promotor/mis-programas`. Si el programa no existe, mostrar 404.

**API call al montar:** `GET /api/crowdpromotion/programas/{programaId}/mis-tareas`

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER LANDING (h-16, bg-[#0d0d1a], border-b border-[#334155])      │
│ [Logo WePlay Rises]   Explorar  Artistas  [CrowdPromotion]  [Avatar]│
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  BREADCRUMB (max-w-3xl mx-auto px-4, pt-6)                          │
│  Mis programas > Promociona mi nuevo album > Tareas                  │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CABECERA DE PAGINA (max-w-3xl mx-auto px-4, py-6)                  │
│  Tareas: Promociona mi nuevo album                                   │
│  Completa las tareas y acumula recompensas                           │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  LISTA DE TAREAS (max-w-3xl mx-auto px-4 pb-10)                     │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ [Share badge]  [Repetible badge]            [VALIDADA badge] │   │
│  │ Comparte en Instagram Stories                                │   │
│  │ Sube una story mencionando la campana...                     │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ Recompensa: 5 EUR por ejecucion                              │   │
│  │ Completada: 3 de 10 veces  |  Ultima: 20 Mar 2026            │   │
│  │                                    [Completar de nuevo ->]   │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ [Post badge]                          [No completada badge]  │   │
│  │ Publica un TikTok                                            │   │
│  │ Crea un TikTok de al menos 30 segundos sobre la campana...   │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ Recompensa: 10 EUR                                           │   │
│  │                                             [Completar ->]   │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ [Post badge]                           [RECHAZADA badge]     │   │
│  │ Escribe una resena en tu blog                                │   │
│  │ Publica una resena de al menos 300 palabras...               │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ Recompensa: 15 EUR                                           │   │
│  │ Motivo rechazo: "La resena no menciona la campana ni..."     │   │
│  │                               [Re-enviar con nueva prueba]   │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ [Share badge]                     [COMPLETADA badge]         │   │
│  │ Comparte en Facebook                                         │   │
│  │ Publica una publicacion en tu pagina de Facebook...          │   │
│  │ ──────────────────────────────────────────────────────────── │   │
│  │ Recompensa: 3 EUR                                            │   │
│  │ Enviada: 25 Mar 2026 - Pendiente de validacion del artista   │   │
│  │                                     [Ver prueba enviada]     │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 1

**Breadcrumb**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor breadcrumb | `<nav>` | `flex items-center gap-1.5 text-xs text-[#64748b] mb-2` |
| Link "Mis programas" | `<Link>` | `hover:text-[#94a3b8] transition-colors` / Navega a `/promotor/mis-programas` |
| Separador | `<ChevronRight w-3 h-3>` | `text-[#334155]` |
| Nombre del programa | `<span>` | `hover:text-[#94a3b8] cursor-pointer transition-colors` (truncado a 24 chars con ellipsis) |
| Separador | `<ChevronRight w-3 h-3>` | `text-[#334155]` |
| "Tareas" (activo) | `<span>` | `text-[#94a3b8]` |

**Cabecera de pagina**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo | `<h1>` | `text-2xl font-bold text-white leading-tight` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` / "Completa las tareas y acumula recompensas" |
| Contenedor | `<div>` | `mb-6` |

**Card de tarea**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] p-5 mb-4 flex flex-col gap-3` |
| Fila superior (badges + estado) | `<div>` | `flex items-start justify-between gap-2` |
| Grupo badges izquierda | `<div>` | `flex items-center gap-2 flex-wrap` |
| Badge tipo evento (Share/Post/etc.) | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium` |
| Badge "Repetible" | `<Badge>` | `bg-[#1e1e38] text-[#64748b] border border-[#334155] text-xs` con icono `<RefreshCw w-3 h-3 mr-1>` |
| Nombre de la tarea | `<h3>` | `text-base font-semibold text-white leading-tight` |
| Descripcion corta | `<p>` | `text-sm text-[#94a3b8] leading-relaxed` (3 lineas max, truncado con "ver mas" si supera) |
| Separador | `<Separator>` | `bg-[#334155]` |
| Fila recompensa | `<div>` | `flex items-center gap-2` |
| Icono recompensa | `<DollarSign w-3.5 h-3.5>` | `text-[#64748b] shrink-0` |
| Label recompensa | `<span>` | `text-xs text-[#64748b]` / "Recompensa:" |
| Valor recompensa | `<span>` | `text-sm font-semibold text-[#10b981]` / "5 EUR por ejecucion" |
| Fila progreso repeticiones | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` (visible solo si `esRepetible = true` y tarea completada al menos una vez) |
| Texto progreso | `<span>` | `text-xs text-[#64748b]` / "Completada: 3 de 10 veces" |
| Separador punto | `<span>` | `text-[#334155]` / " | " |
| Fecha ultima | `<span>` | `text-xs text-[#64748b]` / "Ultima: 20 Mar 2026" |
| Fila de accion | `<div>` | `flex items-center justify-end mt-1` |

**Badges de estado (esquina superior derecha de cada card)**

| Estado | Componente | Estilos / Props |
|--------|------------|-----------------|
| No completada | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs` / Sin icono |
| Completada (pendiente) | `<Badge>` | `bg-amber-950/50 text-amber-400 border border-amber-800/50 text-xs` con icono `<Clock w-3 h-3 mr-1>` / "PENDIENTE" |
| Validada | `<Badge>` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs` con icono `<CheckCircle w-3 h-3 mr-1>` / "VALIDADA" |
| Rechazada | `<Badge>` | `bg-red-950/50 text-red-400 border border-red-800/50 text-xs` con icono `<XCircle w-3 h-3 mr-1>` / "RECHAZADA" |

**Bloque de motivo de rechazo** (visible solo cuando estado = "Rechazada")

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor motivo | `<div>` | `bg-red-950/20 border border-red-900/40 rounded-lg p-3 flex items-start gap-2.5` |
| Icono | `<AlertCircle w-4 h-4>` | `text-red-400 shrink-0 mt-0.5` |
| Contenido | `<div>` | `flex flex-col gap-0.5` |
| Label | `<p>` | `text-xs font-medium text-red-400` / "Motivo del rechazo:" |
| Texto motivo | `<p>` | `text-sm text-red-300/80 leading-relaxed` |

**Bloque de prueba enviada** (visible solo cuando estado = "Completada")

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor | `<div>` | `bg-amber-950/20 border border-amber-900/40 rounded-lg p-3 flex items-center gap-2.5` |
| Icono | `<Clock w-4 h-4>` | `text-amber-400 shrink-0` |
| Texto | `<p>` | `text-xs text-amber-300/80` / "Enviada: {fecha} - Pendiente de validacion del artista" |

**Botones de accion por variante**

| Variante | Componente | Estilos / Props |
|----------|------------|-----------------|
| No completada | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-9 px-4 text-sm` con icono `<Play w-3.5 h-3.5 mr-1.5>` / Texto: "Completar tarea" |
| Repetible con ejecuciones disponibles | `<Button size="sm">` | Mismo estilo gradient / Texto: "Completar de nuevo" con icono `<RefreshCw w-3.5 h-3.5 mr-1.5>` |
| Rechazada (re-enviar) | `<Button variant="outline" size="sm">` | `border-[#a855f7]/50 text-[#a855f7] hover:bg-[#a855f7]/10 hover:border-[#a855f7] h-9 px-4 text-sm` con icono `<RotateCcw w-3.5 h-3.5 mr-1.5>` / Texto: "Re-enviar con nueva prueba" |
| Completada (pendiente validacion) | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-[#94a3b8] h-9 px-4 text-sm` con icono `<ExternalLink w-3.5 h-3.5 mr-1.5>` / Texto: "Ver prueba enviada" / Abre URL en nueva tab |
| No repetible ya validada | Sin boton | Solo mostrar el badge de estado y fecha de validacion |
| Max repeticiones alcanzado | `<div>` | `text-xs text-[#64748b] flex items-center gap-1.5` con icono `<Lock w-3.5 h-3.5>` / Texto: "Maximo de repeticiones alcanzado" |
| Programa inactivo | `<div>` | `text-xs text-[#64748b] flex items-center gap-1.5` con icono `<Lock w-3.5 h-3.5>` / Texto: "Programa inactivo" |
| Tarea con fecha fin pasada | `<div>` | `text-xs text-[#64748b] flex items-center gap-1.5` con icono `<Calendar w-3.5 h-3.5>` / Texto: "Plazo de esta tarea finalizado" |

### Estados de UI Pantalla 1

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | 3 cards skeleton con `animate-pulse`: cada una `bg-[#1e1e38] rounded-xl h-[180px] mb-4` |
| **Default con tareas** | Lista de cards ordenadas por: primero las no completadas/rechazadas (accionables), luego completadas (pendiente), luego validadas |
| **Empty state (sin tareas activas)** | Icono `<ClipboardX w-12 h-12>` centrado con gradiente en contenedor `bg-[#1e1e38] rounded-xl`, titulo "Este programa no tiene tareas activas", subtitulo "El artista aun no ha configurado tareas de promocion" |
| **Error de carga** | Card centrado con icono `<AlertCircle w-8 h-8 text-red-400>` + texto "No se pudieron cargar las tareas" + boton "Reintentar" `<Button variant="outline" size="sm">` |
| **Enviando completar tarea** | Boton de la card pulsada muestra `<Loader2 w-4 h-4 animate-spin>` + "Enviando..." y queda deshabilitado. Resto de botones deshabilitados |
| **Tarea enviada con exito** | Badge de la card pasa de "No completada" a "PENDIENTE" con animacion `transition-all duration-300`. Boton cambia a "Ver prueba enviada". Toast de exito |
| **Error al enviar** | Toast destructivo. Boton vuelve a su estado original |
| **Acceso no autorizado (403)** | Redirigir a `/promotor/mis-programas` con toast: "No estas aprobado en este programa" |

### Toast Messages Pantalla 1

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Tarea completada exitosamente | Success (verde) | "Tarea enviada. El artista revisara tu prueba." |
| Tarea re-enviada (rechazo previo) | Success (verde) | "Prueba re-enviada para validacion." |
| Tarea no repetible ya completada (400) | Destructive (rojo) | "Esta tarea ya fue completada y no es repetible." |
| Max repeticiones alcanzado (400) | Destructive (rojo) | "Alcanzaste el maximo de repeticiones para esta tarea." |
| Programa inactivo (400) | Destructive (rojo) | "Este programa no esta activo. No puedes completar tareas." |
| Error inesperado (500) | Destructive (rojo) | "Ocurrio un error. Intentalo de nuevo." |

### Interacciones Pantalla 1

| Accion | Comportamiento |
|--------|----------------|
| Click "Completar tarea" | Abrir `<Dialog>` de completar tarea con datos de la tarea seleccionada |
| Click "Completar de nuevo" | Abrir `<Dialog>` de completar tarea (mismo dialog, texto del titulo cambia a "Completar de nuevo") |
| Click "Re-enviar con nueva prueba" | Abrir `<Dialog>` de completar tarea con nota de contexto: "Estas re-enviando esta tarea que fue rechazada." |
| Click "Ver prueba enviada" | Abrir URL de prueba en nueva tab (`target="_blank" rel="noopener noreferrer"`) |
| Hover sobre card | `border-[#334155]` pasa a `border-[#a855f7]/30` con `transition-colors duration-200` |
| Click en breadcrumb "Mis programas" | Navegar a `/promotor/mis-programas` |

---

## Pantalla 2: Dialog Completar Tarea

**Proyecto:** Landing (Vite + React 18)
**Trigger:** Botones "Completar tarea", "Completar de nuevo" o "Re-enviar con nueva prueba" en Pantalla 1
**Componente:** `<Dialog>` de shadcn/ui (modal centrado, backdrop oscuro)

**API call al confirmar:** `POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar`

### Layout

```
┌─────────────────────────────────────────────────────────┐
│  [X cerrar]                                             │
│                                                         │
│  Completar: Comparte en Instagram Stories               │
│                                                         │
│  ─────────────────────────────────────────────────────  │
│                                                         │
│  [Nota re-envio - solo si es re-envio]                  │
│  Estas re-enviando esta tarea rechazada.                │
│  Nueva prueba reemplazara la anterior.                  │
│                                                         │
│  Instrucciones                                          │
│  Sube una story de Instagram Stories de al menos        │
│  15 segundos mencionando la campana y etiquetando       │
│  @weplay_rises. La story debe estar en tu perfil        │
│  publico durante al menos 24 horas.                     │
│  [Ver instrucciones completas ->]  (si hay URL externa) │
│                                                         │
│  ─────────────────────────────────────────────────────  │
│                                                         │
│  URL de prueba *                                        │
│  [ https://instagram.com/stories/xxx          ]         │
│                                                         │
│  Comentario (opcional)                                  │
│  [ Story publicada con mencion y etiqueta...  ]         │
│  0 / 500 caracteres                                     │
│                                                         │
│  ─────────────────────────────────────────────────────  │
│                                                         │
│             [Cancelar]       [Enviar prueba]            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Especificaciones Dialog Completar Tarea

**Estructura del Dialog**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialog raiz | `<Dialog>` | `open={isOpen} onOpenChange={setIsOpen}` |
| Overlay | `<DialogOverlay>` | `bg-black/70 backdrop-blur-sm` |
| Contenedor | `<DialogContent>` | `bg-[#151525] border border-[#334155] text-white max-w-lg w-full rounded-xl p-0 overflow-hidden` |
| Boton cerrar | `<DialogClose>` | `absolute top-4 right-4 text-[#64748b] hover:text-white transition-colors` con icono `<X w-5 h-5>` |

**Header del dialog**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor header | `<div>` | `px-6 pt-6 pb-4 border-b border-[#334155]` |
| Titulo | `<DialogTitle>` | `text-xl font-bold text-white` / "Completar: {nombreTarea}" (o "Completar de nuevo: {nombre}" segun contexto) |
| Separador | El border-b del contenedor header actua como separador |

**Nota de re-envio** (visible solo si es re-envio por rechazo previo)

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor nota | `<div>` | `mx-6 mt-4 bg-amber-950/30 border border-amber-800/50 rounded-lg p-3 flex items-start gap-2.5` |
| Icono | `<Info w-4 h-4>` | `text-amber-400 shrink-0 mt-0.5` |
| Texto principal | `<p>` | `text-sm text-amber-300 font-medium` / "Estas re-enviando esta tarea que fue rechazada." |
| Texto secundario | `<p>` | `text-xs text-amber-300/70 mt-0.5` / "Tu nueva prueba reemplazara la anterior." |

**Seccion de instrucciones**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor instrucciones | `<div>` | `px-6 py-4` |
| Label | `<p>` | `text-xs font-semibold text-[#94a3b8] uppercase tracking-wide mb-2` / "Instrucciones" |
| Texto instrucciones | `<p>` | `text-sm text-[#cbd5e1] leading-relaxed` |
| Link instrucciones externas | `<a>` | `inline-flex items-center gap-1 text-xs text-[#a855f7] hover:text-purple-400 mt-2 transition-colors` con icono `<ExternalLink w-3 h-3>` / "Ver instrucciones completas" / `target="_blank"` (visible solo si `urlInstrucciones != null`) |

**Formulario**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor formulario | `<Form>` | shadcn Form con `react-hook-form` + Zod resolver |
| Separador antes form | `<Separator>` | `bg-[#334155] mx-6` |
| Contenedor campos | `<div>` | `px-6 py-4 flex flex-col gap-5` |
| `<FormField>` URL de prueba | `<FormField name="urlPruebaCompletado">` | |
| Label URL | `<FormLabel>` | `text-sm font-medium text-[#cbd5e1]` / "URL de prueba" con `<span className="text-red-400 ml-1">*</span>` |
| Input URL | `<FormControl><Input>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] h-10 focus:border-[#a855f7] focus:ring-1 focus:ring-[#a855f7]/30` / `placeholder="https://instagram.com/stories/..."` / `type="url"` |
| Mensaje de error URL | `<FormMessage>` | `text-xs text-red-400 mt-1` |
| `<FormField>` Comentario | `<FormField name="comentarioPromotor">` | |
| Label comentario | `<FormLabel>` | `text-sm font-medium text-[#cbd5e1]` / "Comentario (opcional)" |
| Textarea comentario | `<FormControl><Textarea>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[80px] resize-none focus:border-[#a855f7] focus:ring-1 focus:ring-[#a855f7]/30` / `placeholder="Describe brevemente la accion realizada..."` |
| Contador caracteres | `<p>` | `text-right text-xs text-[#64748b] mt-1` / "{n} / 500 caracteres" |
| Mensaje de error comentario | `<FormMessage>` | `text-xs text-red-400 mt-1` |

**Footer del dialog**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor footer | `<div>` | `px-6 pb-6 pt-2 border-t border-[#334155] flex items-center justify-end gap-3` |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` / Cierra el dialog sin accion |
| Boton Enviar | `<Button type="submit" form="form-completar-tarea">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-6` / Texto: "Enviar prueba" |
| Boton Enviar (loading) | `<Button disabled>` | Mismo estilo + `<Loader2 w-4 h-4 animate-spin mr-2>` + texto "Enviando..." |

### Validacion en Tiempo Real Dialog Completar Tarea

| Campo | Regla | Mensaje de error |
|-------|-------|-----------------|
| URL de prueba | `z.string().min(1)` | "La URL de prueba es obligatoria" |
| URL de prueba | `z.string().url()` | "Introduce una URL valida (debe comenzar con http:// o https://)" |
| Comentario | `z.string().max(500).optional()` | "El comentario no puede superar 500 caracteres" |

La validacion se ejecuta `onBlur` para el campo URL (despues de que el usuario sale del campo). El contador de caracteres del comentario se actualiza `onChange`.

### Estados de UI Dialog Completar Tarea

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (abierto)** | Dialog visible, campos vacios o con datos previos si es re-envio, boton Enviar habilitado |
| **Campo URL invalido** | Borde del input cambia a `border-red-500`, mensaje de error aparece debajo con animacion `fade-in` |
| **Enviando** | Boton "Enviar prueba" muestra spinner. Dialog no se puede cerrar (click en overlay no cierra). Boton Cancelar deshabilitado |
| **Exito** | Dialog se cierra automaticamente. Toast de exito aparece en la pagina. Card de tarea actualiza su estado |
| **Error de API** | Dialog permanece abierto. Toast destructivo aparece. Boton vuelve a estado normal |

### Interacciones Dialog Completar Tarea

| Accion | Comportamiento |
|--------|----------------|
| Click en [X] o Cancelar | Cerrar dialog sin llamada a API. Limpiar estado del formulario |
| Click fuera del dialog (overlay) | Solo cerrar si no hay envio en curso. Misma accion que Cancelar |
| Tecla Escape | Cerrar dialog si no hay envio en curso |
| Submit con datos validos | `POST /api/.../completar`. Loading en boton. Cerrar al exito y actualizar estado de tarea en la lista |
| Typing en comentario | Actualizar contador de caracteres en tiempo real |

---

## Pantalla 3: Tareas Pendientes de Validacion (Artista - Tab en programa)

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/crowdpromotion/programas/{programaId}` - nueva pestana "Pendientes"
**Tab label:** "Pendientes ({count})" - el count es el total de items pendientes
**Template base:** Dashtail (dashboard con sidebar)

**Precondicion:** Usuario autenticado como Artista propietario del programa. Si no es propietario, redirigir a `/dashboard`.

**API call al activar tab:** `GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes?page=1&pageSize=10`

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a], border-r border-[#334155])             │
│ [Logo]  Dashboard  Campanas  CrowdPromotion  Configuracion          │
├─────────────────────────────────────────────────────────────────────┤
│ TOPBAR (h-14, bg-[#0d0d1a], border-b)                               │
│ [Hamburger]  Programas > Promociona mi nuevo album     [Avatar]      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CONTENIDO (p-6)                                                    │
│                                                                     │
│  Promociona mi nuevo album                [Editar programa]         │
│  Programa activo | 3 promotores aprobados | 8 tareas pendientes     │
│                                                                     │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ TABS                                                          │  │
│  │ [Resumen]  [Promotores (3)]  [Tareas]  [Pendientes (8)]       │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  TAB ACTIVO: PENDIENTES (8)                                         │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ DJ Marketing Pro                                             │   │
│  │ Comparte en Instagram Stories (4ta vez)                      │   │
│  │ Prueba: [instagram.com/stories/xxx ->]                       │   │
│  │ "Story publicada con mencion a la campana"                   │   │
│  │ Hace 2 horas                  [Validar] [Rechazar]           │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │ Luna Nova Fan                                                │   │
│  │ Publica un TikTok (1a vez)                                   │   │
│  │ Prueba: [tiktok.com/@user/video/xxx ->]                      │   │
│  │ (sin comentario)                                             │   │
│  │ Hace 5 horas                  [Validar] [Rechazar]           │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  PAGINACION: [< Anterior]  1  2  [Siguiente >]                      │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 3

**Tabs del programa**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor tabs | `<Tabs>` | `defaultValue="resumen"` |
| Lista de tabs | `<TabsList>` | `bg-[#0d0d1a] border-b border-[#334155] w-full justify-start rounded-none px-0 h-auto` |
| Tab individual (inactivo) | `<TabsTrigger>` | `px-4 py-3 text-sm text-[#64748b] hover:text-[#94a3b8] border-b-2 border-transparent rounded-none transition-colors data-[state=inactive]:bg-transparent` |
| Tab activo | `<TabsTrigger data-state="active">` | `text-white border-b-2 border-[#a855f7] data-[state=active]:bg-transparent data-[state=active]:shadow-none` |
| Badge count en tab "Pendientes" | `<Badge>` | `ml-1.5 bg-red-500/20 text-red-400 border border-red-500/30 text-xs px-1.5 py-0.5 rounded-full` / (solo visible si count > 0) |
| Contenido de tab | `<TabsContent>` | `pt-5` |

**Card de completado pendiente**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] p-5 mb-4` |
| Fila principal | `<div>` | `flex items-start justify-between gap-4` |
| Columna izquierda (info) | `<div>` | `flex-1 min-w-0` |
| Nombre promotor | `<p>` | `text-base font-semibold text-white` |
| Tipo promotor | `<span>` | `text-xs text-[#64748b] ml-2 font-normal` / "(Influencer)" |
| Nombre tarea y numero de ejecucion | `<p>` | `text-sm text-[#94a3b8] mt-0.5` / "Comparte en Instagram Stories (4ta vez)" |
| Fila URL de prueba | `<div>` | `flex items-center gap-1.5 mt-2` |
| Label URL | `<span>` | `text-xs text-[#64748b] shrink-0` / "Prueba:" |
| Link URL | `<a>` | `text-sm text-[#a855f7] hover:text-purple-400 truncate max-w-[300px] inline-flex items-center gap-1 transition-colors` con icono `<ExternalLink w-3 h-3 shrink-0>` / `target="_blank" rel="noopener noreferrer"` |
| Comentario del promotor | `<p>` | `text-sm text-[#94a3b8] italic mt-2 pl-2 border-l-2 border-[#334155]` (visible si no es null) / (texto: "{comentario}") |
| Sin comentario | `<p>` | `text-xs text-[#64748b] mt-2` / "(sin comentario)" |
| Fecha relativa | `<p>` | `text-xs text-[#64748b] mt-2 flex items-center gap-1` con icono `<Clock w-3 h-3>` / "Hace 2 horas" |
| Columna derecha (acciones) | `<div>` | `flex items-center gap-2 shrink-0` |
| Boton Validar | `<Button size="sm">` | `bg-green-950/50 text-green-400 border border-green-800/50 hover:bg-green-900/50 hover:border-green-700/50 h-9 px-4 text-sm font-medium transition-colors` con icono `<Check w-3.5 h-3.5 mr-1.5>` |
| Boton Rechazar | `<Button variant="outline" size="sm">` | `border-red-800/50 text-red-400 hover:bg-red-950/50 hover:border-red-700/50 h-9 px-4 text-sm font-medium transition-colors` con icono `<X w-3.5 h-3.5 mr-1.5>` |

**Paginacion**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor | `<div>` | `flex items-center justify-between mt-6` |
| Info contador | `<p>` | `text-sm text-[#64748b]` / "Mostrando 10 de 8 pendientes" |
| Controles paginacion | `<div>` | `flex items-center gap-2` |
| Boton pagina anterior | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-3` con icono `<ChevronLeft w-4 h-4>` |
| Boton pagina siguiente | `<Button variant="outline" size="sm">` | Mismo estilo con icono `<ChevronRight w-4 h-4>` |
| Numero de pagina actual | `<span>` | `text-sm text-white px-3` / "Pagina 1 de 1" |

### Estados de UI Pantalla 3

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | 3 cards skeleton con `animate-pulse`: `bg-[#1e1e38] rounded-xl h-[120px] mb-4` |
| **Default con pendientes** | Lista de cards ordenadas por `fechaUltimaCompletada` descendente (mas recientes primero) |
| **Empty state (sin pendientes)** | Icono `<CheckCircle w-12 h-12 text-green-400>` centrado en contenedor `bg-[#1e1e38]/50 rounded-xl py-10`, titulo "No hay tareas pendientes de validacion", subtitulo "Todas las tareas completadas han sido revisadas" |
| **Error de carga** | Card de error con `<AlertCircle w-8 h-8 text-red-400>` + boton "Reintentar" |
| **Validando** | Botones de la fila muestran estado loading. La card desaparece de la lista con animacion `fade-out` al completarse (actualiza lista localmente) |
| **Rechazando** | Dialog de rechazo se abre. La card espera hasta que el dialog confirme |

### Toast Messages Pantalla 3

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Tarea validada exitosamente | Success (verde) | "Tarea validada. Se acreditaron {importe} {moneda} en la wallet del promotor." |
| Tarea rechazada | Default (neutro) | "Tarea rechazada. El promotor podra re-enviar con nueva prueba." |
| Error al validar (500) | Destructive (rojo) | "No se pudo validar la tarea. Intentalo de nuevo." |
| Error al rechazar (500) | Destructive (rojo) | "No se pudo rechazar la tarea. Intentalo de nuevo." |

### Interacciones Pantalla 3

| Accion | Comportamiento |
|--------|----------------|
| Click "Validar" | Abrir `<Dialog>` de validar tarea con datos del completado |
| Click "Rechazar" | Abrir `<Dialog>` de rechazar tarea con campos de motivo |
| Click en URL de prueba | Abrir en nueva tab (`target="_blank"`) |
| Cambio de pagina | Fetch nueva pagina con `page` actualizado. Scroll to top del listado |

---

## Pantalla 4: Dialog Validar Tarea (Artista)

**Proyecto:** Admin (Next.js 14)
**Trigger:** Boton "Validar" en Pantalla 3
**Componente:** `<Dialog>` de shadcn/ui

**API call al confirmar:** `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar`

### Layout

```
┌────────────────────────────────────────┐
│  [X]                                   │
│                                        │
│  Validar tarea completada              │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│  Tarea:    Comparte en IG Stories      │
│  Promotor: DJ Marketing Pro            │
│  Ejecucion: 4ta vez completada         │
│                                        │
│  Prueba enviada:                       │
│  [instagram.com/stories/xxx ->]        │
│                                        │
│  Comentario del promotor:              │
│  "Story publicada con mencion a la     │
│   campana y etiqueta de artista"       │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│  Recompensa a acreditar:               │
│  5 EUR en wallet del promotor          │
│                                        │
│  Comentario de validacion (opcional)   │
│  [ Verificado correctamente...      ]  │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│       [Cancelar]    [Validar tarea]    │
│                                        │
└────────────────────────────────────────┘
```

### Especificaciones Dialog Validar Tarea

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialog raiz | `<Dialog>` | `open={isOpen} onOpenChange={setIsOpen}` |
| Contenedor | `<DialogContent>` | `bg-[#151525] border border-[#334155] text-white max-w-md w-full rounded-xl p-0 overflow-hidden` |
| Header | `<div>` | `px-6 pt-6 pb-4 border-b border-[#334155]` |
| Titulo | `<DialogTitle>` | `text-xl font-bold text-white` / "Validar tarea completada" |
| Seccion datos (solo lectura) | `<div>` | `px-6 py-4 flex flex-col gap-3` |
| Fila de dato | `<div>` | `flex items-start gap-2` |
| Label dato | `<span>` | `text-xs font-medium text-[#64748b] w-24 shrink-0` |
| Valor dato | `<span>` | `text-sm text-white` |
| Link URL prueba | `<a>` | `text-sm text-[#a855f7] hover:text-purple-400 flex items-center gap-1 break-all` con icono `<ExternalLink w-3 h-3 shrink-0>` / `target="_blank"` |
| Bloque comentario promotor | `<div>` | `bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 text-sm text-[#cbd5e1] italic` |
| Separador | `<Separator>` | `bg-[#334155] mx-0` |
| Bloque recompensa | `<div>` | `px-6 py-3 bg-green-950/20 border-y border-green-900/30 flex items-center gap-3` |
| Icono recompensa | `<DollarSign w-5 h-5>` | `text-green-400 shrink-0` |
| Texto recompensa | `<div>` | `<p className="text-xs text-[#64748b]">Recompensa a acreditar:</p><p className="text-sm font-semibold text-green-400">{importe} {moneda} en wallet del promotor</p>` |
| Seccion formulario | `<div>` | `px-6 py-4` |
| Label comentario | `<FormLabel>` | `text-sm font-medium text-[#cbd5e1]` / "Comentario de validacion (opcional)" |
| Textarea comentario | `<Textarea>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[72px] resize-none mt-2 focus:border-[#a855f7]` / `placeholder="Ej: Verificado correctamente, contenido segun instrucciones"` |
| Footer | `<div>` | `px-6 pb-6 pt-2 border-t border-[#334155] flex items-center justify-end gap-3` |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` |
| Boton Validar | `<Button>` | `bg-green-600 hover:bg-green-700 text-white font-semibold h-10 px-6 transition-colors` con icono `<Check w-4 h-4 mr-2>` / "Validar tarea" |
| Boton Validar (loading) | `<Button disabled>` | Mismo estilo + `<Loader2 w-4 h-4 animate-spin mr-2>` + "Validando..." |

### Estados de UI Dialog Validar Tarea

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Muestra datos del completado y campo de comentario vacio |
| **Enviando** | Boton "Validar tarea" muestra spinner. Dialog no se puede cerrar |
| **Exito** | Dialog se cierra. Card desaparece de la lista en Pantalla 3. Toast con importe acreditado |
| **Error de API** | Dialog permanece abierto. Toast destructivo |

---

## Pantalla 5: Dialog Rechazar Tarea (Artista)

**Proyecto:** Admin (Next.js 14)
**Trigger:** Boton "Rechazar" en Pantalla 3
**Componente:** `<Dialog>` de shadcn/ui

**API call al confirmar:** `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar`

### Layout

```
┌────────────────────────────────────────┐
│  [X]                                   │
│                                        │
│  Rechazar tarea                        │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│  Tarea:    Comparte en IG Stories      │
│  Promotor: DJ Marketing Pro            │
│  Ejecucion: 4ta vez completada         │
│                                        │
│  Prueba enviada:                       │
│  [instagram.com/stories/xxx ->]        │
│                                        │
│  Comentario del promotor:              │
│  "Story publicada con mencion..."      │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│  [!] Aviso                             │
│  El promotor podra ver este motivo     │
│  y podrá re-enviar la tarea.           │
│                                        │
│  Motivo del rechazo *                  │
│  [ La story no menciona la campana. ]  │
│  0 / 500 caracteres                    │
│                                        │
│  ─────────────────────────────────── │
│                                        │
│       [Cancelar]    [Rechazar tarea]   │
│                                        │
└────────────────────────────────────────┘
```

### Especificaciones Dialog Rechazar Tarea

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialog raiz | `<Dialog>` | `open={isOpen} onOpenChange={setIsOpen}` |
| Contenedor | `<DialogContent>` | `bg-[#151525] border border-[#334155] text-white max-w-md w-full rounded-xl p-0 overflow-hidden` |
| Titulo | `<DialogTitle>` | `text-xl font-bold text-white` / "Rechazar tarea" |
| Seccion datos (igual que Dialog Validar) | `<div>` | `px-6 py-4 flex flex-col gap-3` (mismas filas de Tarea, Promotor, Ejecucion, URL, Comentario) |
| Separador | `<Separator>` | `bg-[#334155]` |
| Bloque aviso | `<div>` | `px-6 py-3 bg-amber-950/20 border-y border-amber-900/30 flex items-start gap-2.5` |
| Icono aviso | `<AlertTriangle w-4 h-4>` | `text-amber-400 shrink-0 mt-0.5` |
| Texto aviso | `<p>` | `text-xs text-amber-300/80` / "El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba." |
| Seccion formulario | `<div>` | `px-6 py-4` |
| Label motivo | `<FormLabel>` | `text-sm font-medium text-[#cbd5e1]` / "Motivo del rechazo" con `<span className="text-red-400 ml-1">*</span>` |
| Textarea motivo | `<Textarea>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[80px] resize-none mt-2 focus:border-red-500` / `placeholder="Explica por que no se acepta esta prueba..."` |
| Contador caracteres | `<p>` | `text-right text-xs text-[#64748b] mt-1` / "{n} / 500 caracteres" |
| Mensaje error motivo | `<FormMessage>` | `text-xs text-red-400 mt-1` |
| Footer | `<div>` | `px-6 pb-6 pt-2 border-t border-[#334155] flex items-center justify-end gap-3` |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5` |
| Boton Rechazar | `<Button>` | `bg-red-600/80 hover:bg-red-600 text-white font-semibold h-10 px-6 border border-red-700/50 transition-colors` con icono `<X w-4 h-4 mr-2>` / "Rechazar tarea" |
| Boton Rechazar (loading) | `<Button disabled>` | Mismo estilo + `<Loader2 w-4 h-4 animate-spin mr-2>` + "Rechazando..." |

### Validacion en Tiempo Real Dialog Rechazar Tarea

| Campo | Regla | Mensaje de error |
|-------|-------|-----------------|
| Motivo del rechazo | `z.string().min(10)` | "El motivo debe tener al menos 10 caracteres" |
| Motivo del rechazo | `z.string().max(500)` | "El motivo no puede superar 500 caracteres" |

### Estados de UI Dialog Rechazar Tarea

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Campo motivo vacio, boton Rechazar habilitado |
| **Campo motivo vacio (blur)** | Borde cambia a `border-red-500`, mensaje de error aparece |
| **Enviando** | Boton muestra spinner y texto "Rechazando...". No se puede cerrar el dialog |
| **Exito** | Dialog se cierra. Card desaparece de la lista en Pantalla 3. Toast con mensaje de rechazo |
| **Error de API** | Dialog permanece abierto. Toast destructivo |

### Interacciones Dialogs Validar y Rechazar

| Accion | Comportamiento |
|--------|----------------|
| Click en [X] o Cancelar | Cerrar dialog sin accion. Limpiar estado del formulario |
| Click fuera del dialog | No cerrar si hay envio en curso. Si no hay envio, cerrar |
| Tecla Escape | Cerrar si no hay envio en curso |
| Submit (Validar) | `PATCH /api/.../validar`. Loading. Al exito, cerrar y remover card de la lista |
| Submit (Rechazar) sin motivo | Mostrar error de validacion, no enviar |
| Submit (Rechazar) con motivo valido | `PATCH /api/.../rechazar`. Loading. Al exito, cerrar y remover card de la lista |

---

## Responsive Breakpoints

### Pantalla 1: Mis Tareas (Landing - Promotor)

| Breakpoint | Width | Cambios de layout |
|------------|-------|-------------------|
| Mobile | < 640px | Contenedor full width con `px-4`. Botones de accion full width (`w-full`). Fila superior de card en columna (badges encima del nombre). Fecha relativa debajo de progreso |
| Tablet | 640px - 1024px | `max-w-2xl mx-auto`. Cards con layout actual. Botones en esquina derecha |
| Desktop | > 1024px | `max-w-3xl mx-auto`. Layout optimo |

### Pantalla 2: Dialog Completar Tarea (Landing)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Dialog ocupa ancho completo (`max-w-none mx-0`) con bordes redondeados solo arriba (`rounded-t-xl`). Se "desliza" desde abajo (bottom sheet). Height maximo `85vh` con scroll interno |
| Tablet/Desktop | >= 640px | Dialog centrado `max-w-lg`. Sin cambios de comportamiento |

### Pantalla 3: Tareas Pendientes (Admin - Artista)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 768px | Sidebar colapsada (hamburger menu). Cards en columna unica. Botones Validar/Rechazar se mueven debajo de la info (columna, full width). URL de prueba truncada mas agresivamente (`max-w-[180px]`) |
| Tablet | 768px - 1024px | Sidebar visible colapsada (solo iconos, 64px de ancho). Cards con layout de dos columnas (info + acciones) |
| Desktop | > 1024px | Sidebar expandida (256px). Layout completo como el diagrama |

### Dialogs 4 y 5: Validar y Rechazar (Admin)

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Dialog ancho completo como bottom sheet (mismo patron que Dialog Completar Tarea) |
| Desktop | >= 640px | Dialog centrado `max-w-md` |

---

## Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| Apertura de dialog | `fade-in` + `scale-in` desde 95% | 200ms ease-out | Click en boton de accion |
| Cierre de dialog | `fade-out` + `scale-out` | 150ms ease-in | Click cancelar / exito |
| Badge de estado en card (cambio) | `transition-all` (color, fondo) | 300ms | API success |
| Card que desaparece de lista Pendientes | `opacity-0` + `height-0` + `margin-0` | 400ms ease-out | API success validar/rechazar |
| Hover sobre card de tarea | `border-color` | 200ms | Mouse enter/leave |
| Hover sobre boton Validar | `background-color` | 150ms | Mouse enter/leave |
| Hover sobre boton Rechazar | `background-color` | 150ms | Mouse enter/leave |
| Loading spinner boton | `animate-spin` | infinito 1000ms | Durante envio de formulario |
| Toast aparicion | Slide desde esquina inferior derecha | 300ms ease-out | API response |
| Mensaje de error de validacion (campo) | `fade-in` + `slide-down` 4px | 150ms | onBlur con error |
| Skeleton loading cards | `animate-pulse` | 2s ciclo infinito | Durante fetch inicial |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1 para texto normal, 3:1 para texto grande. Blanco `#ffffff` sobre `#151525` = ratio 12.6:1. Texto secundario `#94a3b8` sobre `#151525` = ratio 5.1:1 |
| Focus visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0d0d1a]` en todos los elementos interactivos. No usar `outline: none` sin reemplazo |
| Navegacion por teclado | Todos los botones, links e inputs accesibles con Tab. Dialog atrapa el foco dentro mientras esta abierto (`focus-trap`). Escape cierra el dialog |
| Labels de formulario | Todos los `<Input>` y `<Textarea>` tienen `<FormLabel>` asociado con `htmlFor` o usando `<FormControl>` de shadcn que los vincula automaticamente |
| Estados de carga | Botones en loading tienen `aria-busy="true"` y `aria-disabled="true"`. El spinner tiene `aria-label="Cargando"` |
| Dialogs | `<Dialog>` de shadcn maneja `role="dialog"`, `aria-modal="true"` y `aria-labelledby` automaticamente con `<DialogTitle>` |
| Estados de error | Mensajes de error tienen `role="alert"` para anunciarse a lectores de pantalla. Los inputs con error tienen `aria-invalid="true"` y `aria-describedby` apuntando al mensaje |
| Badges de estado | Los badges de estado incluyen texto legible completo (no solo color). Ej: "VALIDADA", "RECHAZADA". No depender unicamente del color |
| Botones de iconos | Botones que solo muestran icono (ej: cerrar dialog) tienen `aria-label` descriptivo: `<Button aria-label="Cerrar dialog">` |
| Fechas relativas | Fechas como "Hace 2 horas" tienen `<time dateTime="2026-03-20T15:30:00Z">` como elemento HTML para accesibilidad |
| Textos de motivo rechazo | El motivo de rechazo en la card del promotor esta en texto plano legible, no en tooltip o icono |
| Skip to content | El layout de landing incluye `<a href="#main-content" className="sr-only focus:not-sr-only">Ir al contenido principal</a>` |

---

## Checklist UI/UX

### Pantalla 1: Mis Tareas (Promotor - Landing)
- [ ] Header de landing con navegacion correcta
- [ ] Breadcrumb con links funcionales
- [ ] Cards de tareas renderizadas para todos los estados (no completada, completada, validada, rechazada)
- [ ] Badge de tipo de evento por tarea
- [ ] Badge "Repetible" visible en tareas repetibles
- [ ] Progreso de repeticiones mostrado (X de Y veces)
- [ ] Bloque de motivo de rechazo visible solo en estado "Rechazada"
- [ ] Bloque de prueba enviada visible solo en estado "Completada"
- [ ] Boton correcto segun estado y condicion (ver tabla de variantes)
- [ ] Estados disabled para max repeticiones, programa inactivo, fecha fin pasada
- [ ] Estado loading con skeletons
- [ ] Empty state cuando no hay tareas activas
- [ ] Estado de error con reintentar
- [ ] Responsive: mobile, tablet, desktop
- [ ] Toast messages en todos los escenarios
- [ ] Animaciones de hover y transicion de estado

### Pantalla 2: Dialog Completar Tarea
- [ ] Dialog abre correctamente desde los tres botones de accion
- [ ] Titulo cambia segun contexto (completar / de nuevo / re-enviar)
- [ ] Nota de re-envio visible solo cuando hay rechazo previo
- [ ] Instrucciones de la tarea visibles
- [ ] Link a instrucciones externas solo si URL existe
- [ ] Campo URL de prueba con validacion de formato URL
- [ ] Campo comentario con contador de caracteres (max 500)
- [ ] Validacion se ejecuta en onBlur (URL) y onChange (contador comentario)
- [ ] Estado loading: spinner en boton, dialog no cierra
- [ ] Cierre en exito: dialog cierra, estado de card actualiza, toast aparece
- [ ] Cierre en error: dialog permanece, toast destructivo
- [ ] Comportamiento bottom sheet en mobile
- [ ] Focus trap dentro del dialog
- [ ] Escape cierra el dialog cuando no hay envio en curso

### Pantalla 3: Tareas Pendientes de Validacion (Artista - Admin)
- [ ] Tab "Pendientes (N)" visible en programa con counter correcto
- [ ] Badge rojo en tab cuando hay pendientes
- [ ] Cards de completados con todos los campos
- [ ] URL de prueba clickable en nueva tab
- [ ] Numero de ejecucion en el nombre de tarea (1ra, 2da, etc.)
- [ ] Comentario del promotor con estilo blockquote o "(sin comentario)"
- [ ] Fecha relativa calculada correctamente
- [ ] Botones Validar y Rechazar por fila
- [ ] Paginacion funcional
- [ ] Estado loading con skeletons
- [ ] Empty state cuando no hay pendientes (check verde)
- [ ] Card desaparece de lista tras validar/rechazar con animacion
- [ ] Responsive: sidebar colapsada en mobile, layout de dos columnas en tablet

### Pantalla 4: Dialog Validar Tarea
- [ ] Datos del completado mostrados en modo solo lectura
- [ ] URL de prueba clickable
- [ ] Bloque verde de recompensa a acreditar con importe y moneda
- [ ] Campo comentario de validacion opcional
- [ ] Boton Validar en verde
- [ ] Estado loading durante llamada API
- [ ] Exito: dialog cierra, card eliminada de lista, toast con importe acreditado
- [ ] Error: dialog permanece, toast destructivo

### Pantalla 5: Dialog Rechazar Tarea
- [ ] Datos del completado mostrados en modo solo lectura
- [ ] URL de prueba clickable
- [ ] Aviso en amarillo informando que el promotor puede re-enviar
- [ ] Campo motivo del rechazo obligatorio (min 10 chars)
- [ ] Contador de caracteres visible
- [ ] Validacion activa antes de enviar
- [ ] Boton Rechazar en rojo
- [ ] Estado loading durante llamada API
- [ ] Exito: dialog cierra, card eliminada de lista, toast neutro
- [ ] Error: dialog permanece, toast destructivo
