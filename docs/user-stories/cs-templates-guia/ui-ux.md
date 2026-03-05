# UI/UX: Templates y Guia para Artistas Noveles

> **Feature:** cs-templates-guia
> **Ultima actualizacion:** 2026-02-15

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Landing general | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing |
| Dashboard artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |
| Crear campana (referencia de wizard) | [WPR_6-Create-Campaign.png](../../ui-images/WPR_6-Create-Campaign.png) | Admin |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue los patrones de dashboard y wizard existentes.

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Landing pages, cards de proyecto | `references/templates/krowd/` |
| **Dashtail** | Dashboard artista, wizard de pasos, formularios | `references/templates/dashtail/` |

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

  /* Primary - Gradient pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;

  /* Status */
  --status-success: #10b981;
  --status-error: #ef4444;
  --status-warning: #f59e0b;
  --status-info: #3b82f6;

  /* Priority Colors */
  --priority-esencial: #ef4444;       /* Red - must have */
  --priority-recomendado: #f59e0b;   /* Yellow - should have */
  --priority-opcional: #64748b;      /* Gray - nice to have */

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
}
```

---

## Pantalla 1: Galeria de Templates (Paso 1)

**Proyecto:** Landing (web)
**Ruta:** `/crowdsourcing/nuevo-proyecto`
**Template base:** `krowd/` (card layouts)

### Layout

```
┌──────────────────────────────────────────────────────────┐
│  [HEADER - Landing navigation]                           │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  ┌────────────────────────────────────────────────────┐  │
│  │  [Progress Bar]                                    │  │
│  │  1. Seleccionar → 2. Personalizar → 3. Confirmar  │  │
│  │  ████████────────────────  (Step 1 of 3)          │  │
│  └────────────────────────────────────────────────────┘  │
│                                                          │
│  Selecciona tu tipo de proyecto                         │
│  Elige la plantilla que mejor se adapte a tus objetivos │
│                                                          │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────┐ │
│  │ [music icon]   │  │ [video icon]   │  │ [route]    │ │
│  │                │  │                │  │            │ │
│  │ Grabar Album/  │  │ Produccion de  │  │ Organizar  │ │
│  │ EP             │  │ Videoclip      │  │ Gira/Tour  │ │
│  │                │  │                │  │            │ │
│  │ 11 necesidades │  │ 11 necesidades │  │ 11 neces.  │ │
│  │ 1,920-10,400 € │  │ 2,000-20,000 € │  │ 5k-10k €   │ │
│  │                │  │                │  │            │ │
│  │ [Seleccionar]  │  │ [Seleccionar]  │  │ [Selec.]   │ │
│  └────────────────┘  └────────────────┘  └────────────┘ │
│                                                          │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────┐ │
│  │ [speaker icon] │  │ [music note]   │  │ [palette]  │ │
│  │                │  │                │  │            │ │
│  │ Marketing y    │  │ Lanzamiento    │  │ Crear      │ │
│  │ Promocion      │  │ de Single      │  │ Presencia  │ │
│  │                │  │                │  │ Online     │ │
│  │ 9 necesidades  │  │ 8 necesidades  │  │ 5 neces.   │ │
│  │ 2,000-20,000 € │  │ 1,500-5,000 €  │  │ 1k-5k €    │ │
│  │                │  │                │  │            │ │
│  │ [Seleccionar]  │  │ [Seleccionar]  │  │ [Selec.]   │ │
│  └────────────────┘  └────────────────┘  └────────────┘ │
│                                                          │
│  [Crear plantilla personalizada] (link alternativo)     │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Progress Bar Container | `<div>` | `max-w-4xl mx-auto mb-8` |
| Progress Steps | Custom component `<WizardStepper>` | `steps={3} currentStep={1}` |
| Page Title | `<h1>` | `text-4xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Templates Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6` |
| Template Card | `<Card>` | `bg-[#0f1729] border-[#334155] hover:bg-[#1e2a42] hover:border-primary transition-all duration-200 cursor-pointer` |
| Card Icon | Lucide icon | `w-16 h-16 text-primary mb-4` (music, video, route, speaker, music-note, palette) |
| Card Title | `<h3>` | `text-xl font-semibold text-white mb-2` |
| Card Stats | `<div>` | `flex flex-col gap-1 text-sm text-[#94a3b8] mb-4` |
| Needs Count | `<span>` | `flex items-center gap-1` con icono |
| Price Range | `<span>` | `flex items-center gap-1 font-medium text-primary` |
| Select Button | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Alternative Link | `<Link>` | `text-primary hover:underline text-center block mt-6` |

### 6 Templates con Datos

| Icon | Titulo | Necesidades | Precio Min | Precio Max |
|------|--------|-------------|------------|------------|
| music | Grabar un Album / EP | 11 | 1,920 EUR | 10,400 EUR |
| video | Produccion de Videoclip | 11 | 2,000 EUR | 20,000 EUR |
| route | Organizar una Gira / Tour | 11 | 5,000 EUR | 10,000 EUR |
| speaker | Marketing y Promocion | 9 | 2,000 EUR | 20,000 EUR |
| music-note | Lanzamiento de Single | 8 | 1,500 EUR | 5,000 EUR |
| palette | Crear Presencia Online | 5 | 1,000 EUR | 5,000 EUR |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders en lugar de cards (6 cards) |
| **Default** | Grid de 6 cards con hover effect |
| **Hover Card** | Background cambia a `--bg-card-hover`, border a `--primary-color`, shadow glow |
| **Click Card** | Navega a paso 2 con el template seleccionado |
| **Empty** | "No hay plantillas disponibles" con boton para crear personalizada |
| **Error** | Toast notification: "Error cargando plantillas. Intenta de nuevo." |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Hover en card** | Cambia background, border y shadow |
| **Click en "Seleccionar"** | Navega a `/crowdsourcing/nuevo-proyecto?step=2&template={id}` |
| **Click en "Crear plantilla personalizada"** | Navega a `/crowdsourcing/nueva-necesidad` (formulario manual) |

---

## Pantalla 2: Personalizar Necesidades (Paso 2)

**Proyecto:** Landing (web)
**Ruta:** `/crowdsourcing/nuevo-proyecto?step=2&template={id}`
**Template base:** `dashtail/` (formularios y wizards)

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│  [HEADER]                                                    │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  [Progress Bar] Step 2 of 3                            │  │
│  │  1. Seleccionar → 2. Personalizar → 3. Confirmar      │  │
│  │  ████████████████────────  (66%)                       │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  Grabar un Album / EP                                        │
│  Personaliza las necesidades de tu proyecto                  │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  PRE-PRODUCCION                                        │  │
│  │                                                        │  │
│  │  [x] Composicion y arreglos musicales   [ESENCIAL]    │  │
│  │      Rol: Arreglista / Compositor [?]                 │  │
│  │      Presupuesto: [200] - [1500] EUR                  │  │
│  │                                                        │  │
│  │  [x] Produccion musical                  [ESENCIAL]    │  │
│  │      Rol: Productor musical [?]                       │  │
│  │      Presupuesto: [500] - [3000] EUR                  │  │
│  │                                                        │  │
│  │  GRABACION                                             │  │
│  │                                                        │  │
│  │  [x] Alquiler de estudio                 [ESENCIAL]    │  │
│  │  [ ] Musicos de sesion                   [OPCIONAL]    │  │
│  │                                                        │  │
│  │  POST-PRODUCCION                                       │  │
│  │  ...                                                   │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  RESUMEN PRESUPUESTARIO                                │  │
│  │  Min total: 1,920 EUR  -  Max total: 10,400 EUR       │  │
│  │  Necesidades seleccionadas: 8 de 11                   │  │
│  │                                                        │  │
│  │  ████████████████████░░░░ 75%                          │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  [< Atras]                            [Siguiente >]          │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Template Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Template Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Phases Container | `<div>` | `space-y-8` |
| Phase Header | `<h2>` | `text-xl font-semibold text-white mb-4 uppercase tracking-wide` |
| Need Item Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mb-3` |
| Checkbox | `<Checkbox>` | shadcn/ui, `defaultChecked` si prioridad = Esencial |
| Need Title | `<Label>` | `text-base font-medium text-white flex items-center gap-2` |
| Priority Badge | `<Badge>` | `variant="outline"` con colores: Esencial=red, Recomendado=yellow, Opcional=gray |
| Role Label | `<span>` | `text-sm text-[#94a3b8] flex items-center gap-1` |
| Role Tooltip | `<Tooltip>` | shadcn/ui con icono `info-circle`, contenido: descripcion del rol |
| Budget Inputs Container | `<div>` | `flex gap-2 items-center mt-2` |
| Min Budget Input | `<Input type="number">` | `w-24 bg-[#1a1a2e] border-[#334155] text-white` |
| Separator | `<span>` | `text-[#64748b]` "-" |
| Max Budget Input | `<Input type="number">` | `w-24 bg-[#1a1a2e] border-[#334155] text-white` |
| Currency Label | `<span>` | `text-sm text-[#94a3b8]` "EUR" |
| Summary Card | `<Card>` | `bg-gradient-to-r from-purple-900/20 to-pink-900/20 border-primary p-6 sticky top-4` |
| Summary Title | `<h3>` | `text-lg font-semibold text-white mb-4` |
| Budget Range | `<div>` | `text-2xl font-bold text-white mb-2` |
| Selected Count | `<p>` | `text-sm text-[#94a3b8] mb-3` |
| Progress Bar | `<Progress>` | shadcn/ui, `value={selectedCount/totalCount * 100}` |
| Navigation Buttons | `<div>` | `flex justify-between mt-8` |
| Back Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Next Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders en fases y necesidades |
| **Default** | Lista agrupada por fases con checkboxes, Esenciales pre-seleccionados |
| **Checkbox Toggle** | Actualiza resumen de presupuesto en tiempo real |
| **Budget Input Change** | Valida min < max, actualiza totales en resumen |
| **Validation Error** | Border rojo en input, mensaje debajo: "El minimo debe ser menor que el maximo" |
| **All Unchecked** | Boton "Siguiente" disabled con tooltip: "Selecciona al menos una necesidad" |
| **Hover Need Item** | Fondo ligeramente mas claro |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje |
|-------|-----------|---------|
| **Presupuesto Min** | >= 0 | "El presupuesto debe ser mayor o igual a 0" |
| **Presupuesto Max** | > Presupuesto Min | "El maximo debe ser mayor que el minimo" |
| **Al menos 1 seleccionado** | count(checked) >= 1 | "Selecciona al menos una necesidad" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Toggle checkbox** | Recalcula totales min/max, actualiza contador "X de Y seleccionadas" |
| **Cambio en input presupuesto** | Debounce 300ms, valida, actualiza totales |
| **Hover en icono de info (rol)** | Muestra tooltip con descripcion del rol profesional |
| **Click "Atras"** | Navega a paso 1 sin perder seleccion (guardar en state/URL params) |
| **Click "Siguiente"** | Valida que al menos 1 necesidad este seleccionada, navega a paso 3 |

### Tooltips de Roles (Ejemplos)

| Rol | Descripcion en Tooltip |
|-----|------------------------|
| Productor musical | "Dirige la vision sonora del proyecto completo. Decide el enfoque artistico, selecciona sonidos y supervisa mezcla/master." |
| Arreglista | "Crea arreglos instrumentales y vocales a partir de una composicion basica. Decide que instrumentos suenan y como interactuan." |
| Ingeniero de mezcla | "Equilibra todas las pistas grabadas en un mix estereo cohesivo usando EQ, compresion y efectos." |
| Ingeniero de mastering | "Aplica el procesamiento final para optimizar la escucha en streaming, vinilo y radio." |

---

## Pantalla 3: Confirmar y Publicar (Paso 3)

**Proyecto:** Landing (web)
**Ruta:** `/crowdsourcing/nuevo-proyecto?step=3&template={id}`
**Template base:** `dashtail/` (formularios y confirmacion)

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│  [HEADER]                                                    │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  [Progress Bar] Step 3 of 3                            │  │
│  │  1. Seleccionar → 2. Personalizar → 3. Confirmar      │  │
│  │  ████████████████████████  (100%)                      │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  Resumen de tu proyecto                                      │
│  Revisa los detalles antes de publicar                       │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Template: Grabar un Album / EP                        │  │
│  │  Necesidades seleccionadas: 8 de 11                   │  │
│  │                                                        │  │
│  │  ┌──────────────────────────────────────────────────┐  │  │
│  │  │  RESUMEN PRESUPUESTARIO                          │  │  │
│  │  │                                                  │  │  │
│  │  │  Min total: 1,920 EUR                            │  │  │
│  │  │  Max total: 10,400 EUR                           │  │  │
│  │  │                                                  │  │  │
│  │  │  Rango promedio: ~6,160 EUR                      │  │  │
│  │  └──────────────────────────────────────────────────┘  │  │
│  │                                                        │  │
│  │  NECESIDADES SELECCIONADAS:                            │  │
│  │                                                        │  │
│  │  PRE-PRODUCCION                                        │  │
│  │  • Composicion y arreglos (200-1,500 EUR) [ESENCIAL]  │  │
│  │  • Produccion musical (500-3,000 EUR) [ESENCIAL]      │  │
│  │                                                        │  │
│  │  GRABACION                                             │  │
│  │  • Alquiler de estudio (200-800 EUR) [ESENCIAL]       │  │
│  │  • Ingeniero de grabacion (200-600 EUR) [ESENCIAL]    │  │
│  │                                                        │  │
│  │  POST-PRODUCCION                                       │  │
│  │  • Mezcla de pistas (150-800 EUR) [ESENCIAL]          │  │
│  │  • Mastering final (50-150 EUR) [ESENCIAL]            │  │
│  │  ...                                                   │  │
│  │                                                        │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  PROYECTO ARTISTICO (opcional)                         │  │
│  │  [Select proyecto] o [Crear nuevo proyecto]           │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  [< Atras]                   [Confirmar y publicar]          │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Summary Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-8` |
| Template Name | `<h2>` | `text-2xl font-semibold text-white mb-1` |
| Selected Count | `<p>` | `text-base text-[#94a3b8] mb-6` |
| Budget Summary Box | `<div>` | `bg-gradient-to-r from-purple-900/30 to-pink-900/30 border border-primary rounded-lg p-6 mb-6` |
| Budget Title | `<h3>` | `text-lg font-semibold text-white mb-3` |
| Budget Min | `<div>` | `text-xl font-bold text-white` |
| Budget Max | `<div>` | `text-xl font-bold text-white` |
| Budget Average | `<div>` | `text-base text-[#94a3b8] mt-2` |
| Needs List Title | `<h3>` | `text-lg font-semibold text-white mb-4` |
| Phase Group | `<div>` | `mb-4` |
| Phase Name | `<h4>` | `text-sm font-semibold text-[#94a3b8] uppercase tracking-wide mb-2` |
| Need List Item | `<li>` | `text-base text-white mb-2 flex items-center gap-2` |
| Need Bullet | `<span>` | `text-primary` "•" |
| Need Text | `<span>` | Titulo + presupuesto + badge prioridad |
| Priority Badge | `<Badge>` | `variant="outline" size="sm"` colores segun prioridad |
| Project Select Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mt-6` |
| Project Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Project Select | `<Select>` | shadcn/ui, lista proyectos del artista + opcion "Crear nuevo" |
| Navigation Buttons | `<div>` | `flex justify-between mt-8` |
| Back Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Confirm Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8 py-3` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders en resumen y lista |
| **Default** | Resumen completo con lista agrupada por fases |
| **No Project Selected** | Boton "Confirmar" enabled (crea proyecto automatico o pregunta) |
| **Loading Submit** | Boton muestra spinner + texto "Publicando necesidades...", disabled |
| **Success** | Toast verde: "8 necesidades publicadas correctamente", redirect a `/crowdsourcing/mis-necesidades` |
| **Error** | Toast rojo: "Error al publicar necesidades. Intenta de nuevo.", boton vuelve a enabled |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "Atras"** | Navega a paso 2 con seleccion preservada |
| **Select proyecto** | Actualiza el proyecto al que se vincularan las necesidades |
| **Click "Confirmar y publicar"** | POST a `/api/crowdsourcing/templates/{id}/generar` con `{ necesidadesSeleccionadas, presupuestos, proyectoId }`, crea N necesidades, muestra toast, redirect |

---

## Pantalla 4: Gestion de Templates (Admin)

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/templates`
**Template base:** `dashtail/` (tables, forms, CRUD)

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │                                                  │
│             │  Templates de Proyecto                           │
│             │  Gestiona las plantillas para artistas noveles  │
│             │                                                  │
│             │  [+ Nuevo Template]                              │
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ Filtros:                                     ││
│             │  │ [Buscar...] [Activos / Todos]                ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ Nombre          | Neces. | Precio  | Estado  ││
│             │  ├────────────────────────────────────────────────│
│             │  │ Grabar Album/EP   11      1.9k-10k  Activo  ││
│             │  │ [Editar] [Ver necesidades] [Desactivar]     ││
│             │  ├────────────────────────────────────────────────│
│             │  │ Videoclip         11      2k-20k    Activo  ││
│             │  │ [Editar] [Ver necesidades] [Desactivar]     ││
│             │  ├────────────────────────────────────────────────│
│             │  │ Gira/Tour         11      5k-10k    Activo  ││
│             │  │ [Editar] [Ver necesidades] [Desactivar]     ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [< Anterior] Pagina 1 de 2 [Siguiente >]        │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-6` |
| New Template Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 mb-4` |
| Filters Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mb-4 flex gap-4` |
| Search Input | `<Input>` | `w-64 bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| Status Filter | `<Select>` | shadcn/ui, options: "Todos", "Activos", "Inactivos" |
| Templates Table | `<Table>` | shadcn/ui, `bg-[#0f1729] border-[#334155]` |
| Table Header | `<TableHeader>` | `bg-[#16213e]` |
| Table Row | `<TableRow>` | `hover:bg-[#1e2a42] transition-colors` |
| Template Name | `<TableCell>` | `font-medium text-white` |
| Needs Count | `<TableCell>` | `text-[#94a3b8]` |
| Price Range | `<TableCell>` | `text-[#94a3b8]` formatCurrency |
| Status Badge | `<Badge>` | `variant="success"` si Activo, `variant="secondary"` si Inactivo |
| Actions Cell | `<TableCell>` | `flex gap-2` |
| Edit Button | `<Button variant="ghost" size="sm">` | `text-primary hover:text-primary/80` |
| View Needs Button | `<Button variant="ghost" size="sm">` | `text-[#94a3b8] hover:text-white` |
| Toggle Status Button | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-white` |
| Pagination | `<Pagination>` | shadcn/ui, centrado, botones prev/next |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton rows en tabla |
| **Empty** | "No hay templates disponibles. Crea el primero." con icono |
| **Filtered No Results** | "No se encontraron templates con esos criterios. [Limpiar filtros]" |
| **Success Create** | Toast verde: "Template creado correctamente" |
| **Success Edit** | Toast verde: "Template actualizado" |
| **Success Toggle Status** | Toast: "Template [activado/desactivado]" |
| **Error** | Toast rojo con mensaje de error |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "+ Nuevo Template"** | Navega a `/dashboard/crowdsourcing/templates/nuevo` (formulario) |
| **Search input** | Debounce 300ms, filtra por nombre |
| **Select status filter** | Filtra templates activos/inactivos |
| **Click "Editar"** | Navega a `/dashboard/crowdsourcing/templates/{id}/editar` |
| **Click "Ver necesidades"** | Abre modal o navega a vista detalle con lista de necesidades del template |
| **Click "Desactivar/Activar"** | Dialogo de confirmacion, PATCH a `/api/crowdsourcing/templates/{id}`, actualiza `Activo` boolean |
| **Click paginacion** | Carga siguiente/anterior pagina |

---

## Pantalla 5: Crear/Editar Template (Admin)

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/templates/nuevo` o `/dashboard/crowdsourcing/templates/{id}/editar`
**Template base:** `dashtail/` (formularios complejos)

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │                                                  │
│             │  [< Volver a Templates]                          │
│             │                                                  │
│             │  Nuevo Template                                  │
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ DATOS GENERALES                              ││
│             │  │                                              ││
│             │  │ Nombre *                                     ││
│             │  │ [_____________________________________]      ││
│             │  │                                              ││
│             │  │ Descripcion                                  ││
│             │  │ [                                      ]     ││
│             │  │ [        Textarea                      ]     ││
│             │  │                                              ││
│             │  │ Icono *                                      ││
│             │  │ [Select: music, video, route, ...]          ││
│             │  │                                              ││
│             │  │ Orden                                        ││
│             │  │ [____] (posicion en la galeria)             ││
│             │  │                                              ││
│             │  │ [x] Activo                                   ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ NECESIDADES                                  ││
│             │  │                                              ││
│             │  │ [+ Agregar necesidad]                        ││
│             │  │                                              ││
│             │  │ ┌──────────────────────────────────────────┐ ││
│             │  │ │ Fase: PRE-PRODUCCION                     │ ││
│             │  │ │ Titulo: Composicion y arreglos *         │ ││
│             │  │ │ Rol: [Select rol profesional]            │ ││
│             │  │ │ Precio: [200] - [1500] EUR               │ ││
│             │  │ │ Prioridad: [Select Esencial]             │ ││
│             │  │ │ [Eliminar]                               │ ││
│             │  │ └──────────────────────────────────────────┘ ││
│             │  │ ...                                          ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [Cancelar] [Guardar template]                   │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-primary hover:underline mb-4` |
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-6` |
| Section Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-xl font-semibold text-white mb-4` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Text Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px]` |
| Icon Select | `<Select>` | shadcn/ui, preview del icono seleccionado al lado |
| Order Input | `<Input type="number">` | `w-24` |
| Active Checkbox | `<Checkbox>` | shadcn/ui |
| Add Need Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Need Item Card | `<Card>` | `bg-[#16213e] border-[#334155] p-4 mb-3 relative` |
| Need Phase Input | `<Input>` | placeholder: "PRE-PRODUCCION" |
| Need Title Input | `<Input>` | placeholder: "Composicion y arreglos" |
| Role Select | `<Select>` | shadcn/ui, opciones desde MaestraRolProfesional |
| Price Inputs | `<div>` con 2 `<Input type="number">` | separados por "-" |
| Priority Select | `<Select>` | opciones: "Esencial", "Recomendado", "Opcional" |
| Delete Need Button | `<Button variant="ghost" size="sm">` | `absolute top-2 right-2 text-red-500 hover:text-red-400` |
| Save Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Cancel Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje |
|-------|-----------|---------|
| **Nombre** | No vacio, max 200 | "El nombre es obligatorio" |
| **Icono** | Debe seleccionar uno | "Selecciona un icono" |
| **Necesidad - Titulo** | No vacio, max 200 | "El titulo es obligatorio" |
| **Necesidad - Rol** | Debe seleccionar uno | "Selecciona un rol profesional" |
| **Necesidad - Precio Min** | >= 0 | "Debe ser mayor o igual a 0" |
| **Necesidad - Precio Max** | > Precio Min | "Debe ser mayor que el minimo" |
| **Necesidad - Prioridad** | Debe seleccionar una | "Selecciona la prioridad" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading (edit mode)** | Skeleton en formulario |
| **Default** | Form vacio (nuevo) o pre-rellenado (editar) |
| **Add Need** | Agrega nueva card de necesidad vacia al final |
| **Delete Need** | Dialogo de confirmacion, elimina card (solo visual si no esta guardado) |
| **Validation Error** | Inputs con error muestran border rojo y mensaje debajo |
| **Loading Submit** | Boton "Guardar" muestra spinner + "Guardando...", form disabled |
| **Success** | Toast verde "Template guardado", redirect a listado |
| **Error** | Toast rojo con mensaje de error |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "+ Agregar necesidad"** | Agrega nuevo item vacio a la lista |
| **Select icono** | Actualiza preview del icono |
| **Click "Eliminar" en necesidad** | Dialogo: "Eliminar necesidad?" -> Si: elimina, No: cierra |
| **Click "Cancelar"** | Dialogo de confirmacion si hay cambios, navega a listado |
| **Click "Guardar template"** | Valida form completo, POST/PUT a `/api/crowdsourcing/templates`, redirect |

---

## Componentes Compartidos

### WizardStepper

**Props:**
```typescript
interface WizardStepperProps {
  steps: number;           // Total de pasos (3)
  currentStep: number;     // Paso actual (1-3)
  stepLabels?: string[];   // Opcional: ["Seleccionar", "Personalizar", "Confirmar"]
}
```

**Renderizado:**
- Barra horizontal con N pasos
- Pasos completados: circulo con check, linea solid
- Paso actual: circulo con numero, linea solid hasta ahi
- Pasos futuros: circulo vacio, linea dashed
- Color: primary para completados/actual, muted para futuros

### TemplateCard

**Props:**
```typescript
interface TemplateCardProps {
  template: {
    id: string;
    nombre: string;
    descripcion?: string;
    icono: string;           // "music", "video", etc.
    necesidadesCount: number;
    precioMin: number;
    precioMax: number;
  };
  onSelect: (id: string) => void;
}
```

**Renderizado:**
- Card con icono grande arriba
- Titulo, descripcion truncada
- Stats: necesidades + precio
- Boton "Seleccionar"
- Hover effect

### NecesidadItem

**Props:**
```typescript
interface NecesidadItemProps {
  necesidad: {
    id: string;
    titulo: string;
    fase: string;
    rolProfesional: string;
    rolDescripcion?: string;
    precioMin: number;
    precioMax: number;
    prioridad: "Esencial" | "Recomendado" | "Opcional";
  };
  isSelected: boolean;
  onToggle: (id: string) => void;
  onBudgetChange: (id: string, min: number, max: number) => void;
}
```

**Renderizado:**
- Checkbox + titulo + badge prioridad
- Rol con tooltip
- Inputs de presupuesto (solo visibles si seleccionado)
- Colores segun prioridad

### BudgetSummary

**Props:**
```typescript
interface BudgetSummaryProps {
  minTotal: number;
  maxTotal: number;
  selectedCount: number;
  totalCount: number;
}
```

**Renderizado:**
- Card sticky con resumen
- Rangos min/max en grande
- Promedio calculado
- Contador "X de Y seleccionadas"
- Barra de progreso

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 640px | Templates grid 1 col, wizard vertical, summary no sticky, inputs presupuesto stack vertical, fase headers mas pequenos |
| **Tablet** | 640-1024px | Templates grid 2 cols, wizard mas compacto, summary sticky con width reducido |
| **Desktop** | > 1024px | Templates grid 3 cols, wizard full width, summary sticky a la derecha, layout 2 columnas (lista needs | summary) |

### Paso 1 (Mobile)
- Grid 1 columna
- Cards full width
- Font sizes reducidos (-2px)
- Padding reducido (p-4)

### Paso 2 (Mobile)
- Summary card al final (no sticky)
- Budget inputs verticales (min arriba, max abajo)
- Tooltips adaptados para touch (click en lugar de hover)

### Paso 3 (Mobile)
- Lista de necesidades sin agrupacion visual de fases (solo texto separador)
- Budget summary card sin gradient (fondo solido)

### Admin (Mobile)
- Tabla se convierte en cards apiladas
- Filtros stack vertical
- Acciones dentro de cada card en lugar de columna

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de color** | Minimo 4.5:1 para texto normal. White (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 ✓ |
| **Focus visible** | Ring de 2px en `border-primary` (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Labels en inputs** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Checkboxes** | `aria-label` descriptivo: "Seleccionar Composicion y arreglos musicales" |
| **Tooltips** | Accesibles con teclado: Enter/Space para abrir, Esc para cerrar |
| **Wizard stepper** | `aria-label="Paso {current} de {total}"` en cada paso |
| **Loading states** | `aria-busy="true"` y `aria-live="polite"` durante submit |
| **Template cards** | `role="button"` y `tabindex="0"` para navegacion con teclado |
| **Budget inputs** | `aria-label="Presupuesto minimo"` y `aria-label="Presupuesto maximo"` |
| **Priority badges** | `aria-label="Prioridad: {prioridad}"` para screen readers |

### ARIA Labels Ejemplos

```tsx
// Template card
<Card
  role="button"
  tabindex="0"
  aria-label={`Template: ${template.nombre}. ${template.necesidadesCount} necesidades. Presupuesto estimado ${formatCurrency(template.precioMin)} a ${formatCurrency(template.precioMax)}`}
  onClick={() => onSelect(template.id)}
  onKeyDown={(e) => e.key === 'Enter' && onSelect(template.id)}
>

// Necesidad checkbox
<Checkbox
  id={necesidad.id}
  checked={isSelected}
  aria-label={`Seleccionar ${necesidad.titulo}. Prioridad: ${necesidad.prioridad}`}
/>

// Budget input
<Input
  type="number"
  aria-label="Presupuesto minimo en euros"
  aria-describedby={`budget-help-${necesidad.id}`}
/>
<span id={`budget-help-${necesidad.id}`} className="sr-only">
  Presupuesto orientativo: {formatCurrency(necesidad.precioMin)} a {formatCurrency(necesidad.precioMax)}
</span>

// Wizard stepper
<div
  role="navigation"
  aria-label="Progreso del wizard"
>
  <span aria-current={currentStep === 1 ? "step" : undefined}>
    Paso 1: Seleccionar template
  </span>
</div>

// Loading button
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isPending ? "Publicando necesidades..." : "Confirmar y publicar"}
</Button>
```

---

## Animaciones y Transiciones

| Elemento | Animacion | Duración |
|----------|-----------|----------|
| **Template card hover** | Scale 1.02 + border glow + shadow increase | 200ms ease-out |
| **Card click** | Scale 0.98 momentaneo | 100ms ease-in |
| **Wizard step transition** | Fade out anterior + fade in siguiente | 300ms ease-in-out |
| **Progress bar fill** | Width transition smooth | 400ms ease-out |
| **Checkbox toggle** | Scale 1.1 momentaneo | 150ms ease |
| **Budget update (summary)** | Number count-up animation | 500ms ease-out |
| **Tooltip show** | Fade in + translate Y -4px | 200ms ease-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Button hover** | Background gradient shift + scale 1.02 | 150ms ease |
| **Input focus** | Border color transition + glow shadow | 200ms ease |
| **Validation error shake** | Shake animation (4px horizontal) | 400ms ease |
| **Loading spinner** | Rotate 360deg infinite | 1000ms linear |
| **Need item expand** | Height auto + fade in budget inputs | 300ms ease-out |
| **Table row hover** | Background color lighten | 150ms ease |
| **Modal open** | Backdrop fade + content scale 0.95 → 1 | 200ms ease-out |

### Shake Animation (Validation Error)

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

### Count-up Animation (Budget Summary)

```typescript
// Hook personalizado para animar numeros
const useCountUp = (end: number, duration: number = 500) => {
  const [count, setCount] = useState(0);

  useEffect(() => {
    let startTime: number;
    const step = (timestamp: number) => {
      if (!startTime) startTime = timestamp;
      const progress = Math.min((timestamp - startTime) / duration, 1);
      setCount(Math.floor(progress * end));
      if (progress < 1) {
        requestAnimationFrame(step);
      }
    };
    requestAnimationFrame(step);
  }, [end, duration]);

  return count;
};
```

---

## Checklist UI/UX

### Paso 1: Galeria de Templates
- [ ] Layout de grid responsive (1/2/3 cols)
- [ ] 6 template cards con datos correctos
- [ ] Iconos Lucide apropiados por tipo de proyecto
- [ ] Hover effect en cards (bg, border, shadow)
- [ ] WizardStepper component mostrando paso 1/3
- [ ] Link alternativo "Crear plantilla personalizada" funcional
- [ ] Loading skeletons implementados
- [ ] Empty state con mensaje y CTA
- [ ] Cards navegables con teclado (Tab + Enter)
- [ ] ARIA labels en cards
- [ ] Animacion de hover suave

### Paso 2: Personalizar Necesidades
- [ ] Layout agrupado por fases
- [ ] Checkboxes funcionales con estado inicial (Esencial pre-checked)
- [ ] Priority badges con colores correctos (rojo/amarillo/gris)
- [ ] Tooltips en roles profesionales con descripcion
- [ ] Inputs de presupuesto con validacion min < max
- [ ] Summary card sticky (desktop) con totales en tiempo real
- [ ] Progress bar actualizado al toggle checkboxes
- [ ] Contador "X de Y seleccionadas"
- [ ] Botones navegacion prev/next funcionales
- [ ] Validacion: al menos 1 necesidad seleccionada
- [ ] Debounce en budget inputs (300ms)
- [ ] Tooltips accesibles con teclado
- [ ] Responsive: summary al final en mobile

### Paso 3: Confirmar y Publicar
- [ ] Resumen completo del template
- [ ] Budget summary box destacado
- [ ] Lista agrupada por fases
- [ ] Priority badges en cada necesidad
- [ ] Select de proyecto artistico
- [ ] Opcion "Crear nuevo proyecto"
- [ ] Botones navegacion prev/confirm
- [ ] Loading state en boton confirm
- [ ] Toast notification de exito
- [ ] Redirect a /crowdsourcing/mis-necesidades tras exito
- [ ] Manejo de errores con toast
- [ ] Responsive: lista sin grupo visual en mobile

### Admin: Listado de Templates
- [ ] Table con datos correctos
- [ ] Filtros: search + status
- [ ] Debounce en search (300ms)
- [ ] Status badges (Activo/Inactivo)
- [ ] Botones: Editar, Ver necesidades, Toggle status
- [ ] Paginacion funcional
- [ ] Empty state
- [ ] Loading skeletons
- [ ] Dialogo confirmacion toggle status
- [ ] Toast notifications
- [ ] Responsive: cards en mobile

### Admin: Crear/Editar Template
- [ ] Formulario completo con validacion
- [ ] Select de icono con preview
- [ ] Lista dinamica de necesidades
- [ ] Boton "+ Agregar necesidad"
- [ ] Boton eliminar necesidad con confirmacion
- [ ] Select de rol profesional poblado
- [ ] Select de prioridad
- [ ] Validacion de campos obligatorios
- [ ] Loading state en submit
- [ ] Toast de exito/error
- [ ] Redirect tras guardar
- [ ] Dialogo confirmacion si hay cambios al cancelar

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Design tokens usados (no valores hardcoded)
- [ ] shadcn/ui components correctos
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring purple
- [ ] ARIA labels y roles
- [ ] Keyboard navigation completa
- [ ] Loading states con aria-busy
- [ ] Mobile-first responsive design
- [ ] Tooltips accesibles
- [ ] Form validation con Zod
- [ ] React Hook Form integrado
- [ ] TanStack Query para data fetching
- [ ] Error boundaries implementados
