# UI/UX: Gestionar Necesidades de Crowdsourcing

> **Feature:** cs-gestionar-necesidades
> **Ultima actualizacion:** 2026-02-16

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Dashboard artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |
| Crear campana (referencia de wizard) | [WPR_6-Create-Campaign.png](../../ui-images/WPR_6-Create-Campaign.png) | Admin |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue los patrones de dashboard y formularios existentes en el panel de artista.

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Landing pages, cards de proyecto | `references/templates/krowd/` |
| **Dashtail** | Dashboard artista, formularios, tables | `references/templates/dashtail/` |

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

  /* Estado Necesidad Colors */
  --estado-abierta: #10b981;        /* Green - open for proposals */
  --estado-en-progreso: #3b82f6;    /* Blue - work in progress */
  --estado-cerrada: #64748b;        /* Gray - closed */
  --estado-cancelada: #ef4444;      /* Red - cancelled */

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

## Pantalla 1: Mis Necesidades (Listado)

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/mis-necesidades`
**Template base:** `dashtail/` (dashboard layouts)

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │                                                  │
│             │  Mis Necesidades                                 │
│             │  Gestiona tus solicitudes de servicios          │
│             │                                                  │
│             │  [+ Nueva Necesidad] [Usar Plantilla]           │
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ Filtros:                                     ││
│             │  │ [Estado: Todos ▼] [Buscar necesidad...]      ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ [ABIERTA]  Mezcla de pistas                 ││
│             │  │                                              ││
│             │  │ Producción Musical • Remoto                  ││
│             │  │ 150 - 800 EUR                                ││
│             │  │                                              ││
│             │  │ 3 propuestas  📅 Hace 5 días                ││
│             │  │ Límite: 23 Feb 2026 (7 días)                ││
│             │  │                                              ││
│             │  │ [Ver Detalle] [Editar] [Cerrar]             ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ [EN PROGRESO]  Diseño de portada            ││
│             │  │                                              ││
│             │  │ Diseño Gráfico • Remoto                      ││
│             │  │ 200 - 500 EUR                                ││
│             │  │                                              ││
│             │  │ 1 propuesta aceptada  📅 Hace 12 días       ││
│             │  │                                              ││
│             │  │ [Ver Acuerdo]                                ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [< Anterior] Página 1 de 3 [Siguiente >]        │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Page Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-6` |
| Action Buttons Container | `<div>` | `flex gap-3 mb-6` |
| Nueva Necesidad Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Usar Plantilla Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Filters Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mb-6 flex gap-4 items-center` |
| Estado Filter | `<Select>` | shadcn/ui, options: "Todos", "Abierta", "En Progreso", "Cerrada", "Cancelada" |
| Search Input | `<Input>` | `flex-1 max-w-md bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| Necesidad Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-4 hover:bg-[#1e2a42] hover:border-primary transition-all cursor-pointer` |
| Estado Badge | `<Badge>` | `variant="outline"` con colores: Abierta=green, En Progreso=blue, Cerrada=gray, Cancelada=red |
| Titulo Necesidad | `<h3>` | `text-xl font-semibold text-white mb-3` |
| Meta Info Row | `<div>` | `flex items-center gap-3 text-sm text-[#94a3b8] mb-2` |
| Tipo Icon | Lucide icon | `w-4 h-4` (music, palette, video, megaphone según tipo) |
| Modalidad Badge | `<Badge variant="secondary" size="sm">` | text con icono (globe=Remoto, map-pin=Presencial, hybrid=Híbrido) |
| Budget Range | `<div>` | `text-base font-medium text-white mb-3` |
| Stats Row | `<div>` | `flex items-center gap-4 text-sm text-[#94a3b8] mb-4` |
| Propuestas Counter | `<div>` | `flex items-center gap-1` con icono `users` + badge numérico si > 0 |
| Date Info | `<div>` | `flex items-center gap-1` con icono `calendar` |
| Fecha Limite | `<span>` | Warning color si < 7 días, error si < 3 días |
| Actions Row | `<div>` | `flex gap-2 pt-3 border-t border-[#334155]` |
| Ver Detalle Button | `<Button variant="ghost" size="sm">` | `text-primary hover:text-primary/80` |
| Editar Button | `<Button variant="ghost" size="sm">` | `text-white hover:text-white/80` (solo si Abierta) |
| Cerrar Button | `<Button variant="ghost" size="sm">` | `text-red-400 hover:text-red-300` (si Abierta o En Progreso) |
| Ver Acuerdo Button | `<Button variant="default" size="sm">` | Solo si estado = En Progreso |
| Pagination | `<Pagination>` | shadcn/ui component, centered |
| Empty State Container | `<div>` | `flex flex-col items-center justify-center min-h-[400px] text-center p-8` |
| Empty Illustration | `<div>` | Icon `inbox` con `w-20 h-20 text-[#64748b] mb-4` |
| Empty Title | `<h3>` | `text-xl font-semibold text-white mb-2` |
| Empty Description | `<p>` | `text-base text-[#94a3b8] mb-6 max-w-md` |
| Empty Actions | `<div>` | `flex gap-3` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton cards (3-4 cards con shimmer effect) |
| **Empty (sin filtros)** | Ilustración + "No tienes necesidades publicadas" + "Publica lo que necesitas y recibe propuestas de profesionales" + botones [Publicar necesidad] [Usar plantilla] |
| **Empty (con filtros)** | Icono search + "No se encontraron necesidades" + "Intenta ajustar los filtros" + [Limpiar filtros] |
| **Populated** | Grid de cards con datos |
| **Card Hover** | Background cambia a `--bg-card-hover`, border a `--primary-color`, leve elevación (shadow-md) |
| **Fecha Limite < 7 días** | Badge amarillo "Cierra pronto" |
| **Fecha Limite < 3 días** | Badge rojo "Urgente - cierra en X días" |
| **Estado Badges** | Abierta (green background), En Progreso (blue), Cerrada (gray), Cancelada (red) |

### Validación en Tiempo Real

| Campo | Validacion | Mensaje |
|-------|-----------|---------|
| **Search input** | Debounce 300ms | Filtra en tiempo real |
| **Estado filter** | Inmediato | Actualiza listado sin delay |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "+ Nueva Necesidad"** | Navega a `/dashboard/crowdsourcing/necesidades/nueva` |
| **Click "Usar Plantilla"** | Navega a `/crowdsourcing/nuevo-proyecto` (wizard de templates, US-CS-01) |
| **Select estado filter** | Actualiza listado inmediatamente, preserva búsqueda |
| **Type en search** | Debounce 300ms, filtra por título y descripción |
| **Click en card (área general)** | Navega a detalle de necesidad |
| **Click "Ver Detalle"** | Navega a `/dashboard/crowdsourcing/necesidades/{id}` |
| **Click "Editar"** | Navega a `/dashboard/crowdsourcing/necesidades/{id}/editar` (solo si Abierta) |
| **Click "Cerrar"** | Abre diálogo de confirmación (ver Pantalla 5) |
| **Click "Ver Acuerdo"** | Navega a detalle del acuerdo creado (US-CS-04) |
| **Paginación** | Carga página correspondiente, scroll to top |

---

## Pantalla 2: Crear Necesidad (Form)

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/necesidades/nueva`
**Template base:** `dashtail/` (forms)

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │                                                  │
│             │  [< Volver a Mis Necesidades]                    │
│             │                                                  │
│             │  Publicar Nueva Necesidad                        │
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ INFORMACIÓN BÁSICA                           ││
│             │  │                                              ││
│             │  │ Título *                                     ││
│             │  │ [________________________________]           ││
│             │  │                                              ││
│             │  │ Descripción                                  ││
│             │  │ [                                      ]     ││
│             │  │ [        Textarea                      ]     ││
│             │  │ [                                      ]     ││
│             │  │                                              ││
│             │  │ ┌─────────────────┐  ┌──────────────────┐   ││
│             │  │ │ Tipo necesidad* │  │ Modalidad*       │   ││
│             │  │ │ [Select ▼     ] │  │ [Select ▼      ] │   ││
│             │  │ └─────────────────┘  └──────────────────┘   ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ PRESUPUESTO                                  ││
│             │  │                                              ││
│             │  │ Rango presupuestario (orientativo)           ││
│             │  │ ┌─────┐     ┌─────┐     ┌─────────┐         ││
│             │  │ │ Min │  -  │ Max │     │ Moneda  │         ││
│             │  │ │ [__]│     │ [__]│     │ [EUR ▼] │         ││
│             │  │ └─────┘     └─────┘     └─────────┘         ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ UBICACIÓN Y FECHAS                           ││
│             │  │                                              ││
│             │  │ Ubicación (si presencial/híbrido)            ││
│             │  │ ┌──────────────┐  ┌──────────────┐          ││
│             │  │ │ Ciudad       │  │ País         │          ││
│             │  │ │ [__________] │  │ [__________] │          ││
│             │  │ └──────────────┘  └──────────────┘          ││
│             │  │                                              ││
│             │  │ ┌──────────────┐  ┌──────────────┐          ││
│             │  │ │ Límite props │  │ Inicio prev. │          ││
│             │  │ │ [📅 ______] │  │ [📅 ______] │          ││
│             │  │ └──────────────┘  └──────────────┘          ││
│             │  │                                              ││
│             │  │ Proyecto Artístico *                         ││
│             │  │ [Select mi proyecto ▼]                       ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [Cancelar] [Publicar Necesidad]                 │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-primary hover:underline mb-4` con icono `arrow-left` |
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-6` |
| Section Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` |
| Required Indicator | `<span>` | `text-red-400 ml-1` "*" |
| Text Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px]` |
| Character Counter | `<span>` | `text-xs text-[#64748b] mt-1` "X / 4000 caracteres" |
| Select | `<Select>` | shadcn/ui, `bg-[#1a1a2e] border-[#334155] text-white` |
| Two Column Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Budget Input Container | `<div>` | `flex gap-3 items-center` |
| Budget Input | `<Input type="number">` | `w-32 bg-[#1a1a2e] border-[#334155] text-white` |
| Budget Separator | `<span>` | `text-[#64748b] font-medium` "-" |
| Currency Select | `<Select>` | `w-32` |
| DatePicker | `<Popover>` + `<Calendar>` | shadcn/ui datepicker con icono calendar |
| Conditional Location Inputs | Campos ciudad/país | `hidden` si modalidad = Remoto, `block` si Presencial/Híbrido |
| Error Message | `<p>` | `text-sm text-red-400 mt-1` con role="alert" |
| Form Actions | `<div>` | `flex justify-between mt-8` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Publicar Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Pristine** | Form vacío, todos los inputs en estado default |
| **Typing/Editing** | Character counter actualizado en tiempo real para textarea |
| **Validation Error** | Border rojo en input, mensaje de error debajo |
| **Modalidad = Remoto** | Campos ubicación (ciudad/país) ocultos |
| **Modalidad = Presencial/Híbrido** | Campos ubicación visibles y obligatorios |
| **Submitting** | Botón "Publicar" muestra spinner + texto "Publicando...", form disabled, cursor wait |
| **Success** | Toast verde: "Necesidad publicada correctamente", redirect a `/dashboard/crowdsourcing/mis-necesidades` |
| **Error** | Toast rojo: "Error al publicar. [Mensaje específico]", botón vuelve a enabled |

### Validación en Tiempo Real

| Campo | Validacion | Mensaje |
|-------|-----------|---------|
| **Título** | Min 5 chars, max 200, obligatorio | "El título debe tener al menos 5 caracteres" |
| **Descripción** | Max 4000 chars | "Máximo 4000 caracteres" |
| **Tipo Necesidad** | Obligatorio, debe existir en maestras | "Selecciona el tipo de necesidad" |
| **Modalidad** | Obligatorio | "Selecciona la modalidad de trabajo" |
| **Presupuesto Min** | >= 0 si se rellena | "El presupuesto debe ser mayor o igual a 0" |
| **Presupuesto Max** | > Presupuesto Min si ambos rellenados | "El presupuesto máximo debe ser mayor que el mínimo" |
| **Moneda** | Obligatorio si hay presupuesto | "Selecciona la moneda" |
| **Ciudad** | Obligatorio si modalidad != Remoto, max 100 | "La ciudad es obligatoria para trabajo presencial/híbrido" |
| **País** | Obligatorio si modalidad != Remoto, max 100 | "El país es obligatorio para trabajo presencial/híbrido" |
| **Fecha Límite** | >= hoy + 1 día si se rellena | "La fecha límite debe ser al menos mañana" |
| **Fecha Inicio** | >= hoy si se rellena | "La fecha de inicio no puede ser en el pasado" |
| **Proyecto** | Obligatorio, FK válido | "Selecciona un proyecto artístico" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "< Volver"** | Navega back (sin confirmación si form pristine, con confirmación si dirty) |
| **Select Modalidad** | Si cambia a/desde Remoto, muestra/oculta campos ubicación |
| **Type en Descripción** | Actualiza contador de caracteres en tiempo real |
| **Blur input** | Valida campo, muestra error si inválido |
| **Change Presupuesto Min/Max** | Valida que min < max en tiempo real |
| **Select Moneda** | Marca como seleccionada, actualiza formato de presupuesto |
| **Click DatePicker** | Abre calendario, solo fechas >= hoy seleccionables |
| **Click "Cancelar"** | Muestra diálogo: "¿Descartar cambios?" si dirty, navega a listado |
| **Click "Publicar Necesidad"** | Valida form completo, POST a API, muestra toast, redirect |

---

## Pantalla 3: Editar Necesidad (Form)

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}/editar`
**Template base:** `dashtail/` (forms)

### Layout

**Same layout as Crear Necesidad** pero:
- Page title: "Editar Necesidad"
- Campos pre-rellenados con datos existentes
- Campo "Tipo de Necesidad" es **DISABLED/readonly** (no editable)
- Si la necesidad tiene propuestas recibidas: mostrar **banner de advertencia** arriba del form

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| **Warning Banner** (si tiene propuestas) | `<Alert>` | shadcn/ui Alert, variant="warning", `bg-amber-900/20 border-amber-700 mb-6` |
| Banner Icon | `<AlertCircle>` | Lucide icon, `w-5 h-5 text-amber-500` |
| Banner Title | `<AlertTitle>` | `text-base font-semibold text-amber-200` |
| Banner Description | `<AlertDescription>` | `text-sm text-amber-300` "Esta necesidad ya tiene N propuestas. Los cambios serán visibles para los profesionales que ya enviaron propuestas." |
| Tipo Necesidad Select | `<Select disabled>` | `opacity-60 cursor-not-allowed` con tooltip explicativo |
| Save Button | `<Button>` | Texto: "Guardar Cambios" en lugar de "Publicar Necesidad" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading (fetch data)** | Skeleton del formulario |
| **Loaded** | Form pre-rellenado, tipo necesidad disabled |
| **Con Propuestas** | Banner warning visible arriba |
| **Sin Propuestas** | Sin banner, edición normal |
| **Estado != Abierta** | Redirect automático a detalle con toast error: "Solo puedes editar necesidades abiertas" |
| **Validation Error** | Igual que en crear |
| **Submitting** | Botón "Guardar Cambios" con spinner |
| **Success** | Toast verde: "Necesidad actualizada", redirect a detalle |
| **Error 400 (estado != Abierta)** | Toast rojo: "No se puede editar una necesidad cerrada o en progreso", redirect a listado |

### Validación en Tiempo Real

**Misma validación que en Crear Necesidad**, excepto:
- Tipo de Necesidad no se valida (disabled)

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Hover en Tipo Necesidad disabled** | Tooltip: "No se puede cambiar el tipo una vez publicada" |
| **Click "Guardar Cambios"** | PUT a `/api/crowdsourcing/necesidades/{id}`, actualiza FechaActualizacion, redirect |
| **Otras interacciones** | Igual que en Crear Necesidad |

---

## Pantalla 4: Detalle de Necesidad

**Proyecto:** Admin
**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}`
**Template base:** `dashtail/` (detail views)

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │                                                  │
│             │  [< Mis Necesidades]                             │
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ Mezcla de pistas          [ABIERTA] [⋮]     ││
│             │  │                                              ││
│             │  │ [Editar] [Cerrar Necesidad]                  ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ DETALLES                                     ││
│             │  │                                              ││
│             │  │ Descripción:                                 ││
│             │  │ Necesito un ingeniero de mezcla...           ││
│             │  │                                              ││
│             │  │ Tipo: Producción Musical                     ││
│             │  │ Modalidad: Remoto                            ││
│             │  │ Presupuesto: 150 - 800 EUR                   ││
│             │  │ Proyecto: Mi Primer EP                       ││
│             │  │                                              ││
│             │  │ Publicado: 16 Feb 2026 (hace 5 días)        ││
│             │  │ Última actualización: 16 Feb 2026            ││
│             │  │ Límite propuestas: 23 Feb 2026 (7 días)     ││
│             │  │ Inicio previsto: 1 Mar 2026                  ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ PROPUESTAS RECIBIDAS (3)                     ││
│             │  │                                              ││
│             │  │ ┌────────────────────────────────────────┐   ││
│             │  │ │ 👤 Juan Pérez                         │   ││
│             │  │ │    Ingeniero de mezcla • ⭐ 4.8 (12)  │   ││
│             │  │ │                                        │   ││
│             │  │ │    "Con 10 años de experiencia..."     │   ││
│             │  │ │                                        │   ││
│             │  │ │    Precio: 600 EUR • 7 días            │   ││
│             │  │ │    Enviado: 17 Feb 2026               │   ││
│             │  │ │                                        │   ││
│             │  │ │    [Ver Perfil] [Aceptar] [Rechazar]   │   ││
│             │  │ └────────────────────────────────────────┘   ││
│             │  │                                              ││
│             │  │ ... (más propuestas)                         ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-primary hover:underline mb-4` |
| Header Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Title Row | `<div>` | `flex items-center justify-between mb-4` |
| Title | `<h1>` | `text-2xl font-bold text-white flex-1` |
| Estado Badge | `<Badge>` | Con colores según estado |
| Actions Menu | `<DropdownMenu>` | shadcn/ui, icono `more-vertical` |
| Action Buttons | `<div>` | `flex gap-3` |
| Editar Button | `<Button variant="outline">` | Solo si estado = Abierta |
| Cerrar Button | `<Button variant="destructive">` | Si estado = Abierta o En Progreso |
| Details Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Description Text | `<p>` | `text-base text-[#94a3b8] mb-4 whitespace-pre-line` |
| Info Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Info Item | `<div>` | `flex flex-col gap-1` |
| Info Label | `<span>` | `text-sm text-[#64748b]` |
| Info Value | `<span>` | `text-base text-white` |
| Propuestas Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Propuestas Header | `<div>` | `flex items-center justify-between mb-4` |
| Propuestas Count | `<h2>` | `text-lg font-semibold text-white` "PROPUESTAS RECIBIDAS (N)" |
| Propuesta Item Card | `<Card>` | `bg-[#16213e] border-[#334155] p-4 mb-3` |
| Profesional Row | `<div>` | `flex items-center gap-3 mb-3` |
| Avatar | `<Avatar>` | shadcn/ui, 40x40px |
| Profesional Name | `<span>` | `text-base font-semibold text-white` |
| Profesional Meta | `<div>` | `text-sm text-[#94a3b8]` Rol + Rating |
| Rating Display | `<span>` | `flex items-center gap-1` icono star + score + count |
| Mensaje Propuesta | `<p>` | `text-sm text-[#94a3b8] mb-3` Truncado a 150 chars con "Leer más" |
| Propuesta Meta Row | `<div>` | `flex items-center gap-4 text-sm text-[#64748b] mb-3` |
| Precio Badge | `<Badge variant="secondary">` | `text-white font-semibold` |
| Dias Badge | `<Badge variant="outline">` | Con icono clock |
| Fecha Envio | `<span>` | Con icono calendar |
| Propuesta Actions | `<div>` | `flex gap-2 pt-3 border-t border-[#334155]` |
| Ver Perfil Button | `<Button variant="ghost" size="sm">` | `text-primary` |
| Aceptar Button | `<Button variant="default" size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600` |
| Rechazar Button | `<Button variant="ghost" size="sm">` | `text-red-400 hover:text-red-300` |
| Empty Propuestas | `<div>` | `text-center py-8` icono inbox + "Aún no hay propuestas para esta necesidad" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton en header, detalles y propuestas |
| **Estado = Abierta** | Botones [Editar] [Cerrar] visibles, propuestas con acciones [Aceptar] [Rechazar] |
| **Estado = En Progreso** | Solo botón [Ver Acuerdo], no [Editar], propuestas en readonly |
| **Estado = Cerrada/Cancelada** | Solo información, sin acciones |
| **Sin Propuestas** | Empty state con icono y mensaje |
| **Con Propuestas** | Lista de cards de propuestas |
| **Propuesta Fuera de Presupuesto** | Badge "Fuera del rango presupuestario" si precio < min o > max |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "< Mis Necesidades"** | Navega al listado |
| **Click "Editar"** | Navega a form de edición |
| **Click "Cerrar Necesidad"** | Abre diálogo de cierre (Pantalla 5) |
| **Click "Ver Perfil" (propuesta)** | Navega a perfil público del profesional |
| **Click "Aceptar" (propuesta)** | Muestra diálogo confirmación, crea acuerdo (US-CS-04), redirect |
| **Click "Rechazar" (propuesta)** | Muestra diálogo con textarea motivo opcional, rechaza propuesta |
| **Click "Leer más" (mensaje)** | Expande mensaje completo |

---

## Pantalla 5: Diálogo de Cierre

**Proyecto:** Admin
**Contexto:** Modal overlay sobre detalle o listado
**Template base:** shadcn/ui Dialog

### Layout

```
┌────────────────────────────────────────────┐
│  Cerrar Necesidad                     [×]  │
├────────────────────────────────────────────┤
│                                            │
│  ⚠️ Al cerrar esta necesidad, las          │
│     propuestas pendientes serán            │
│     rechazadas automáticamente.            │
│                                            │
│  Motivo del cierre (opcional)              │
│  ┌──────────────────────────────────────┐  │
│  │                                      │  │
│  │  [      Textarea      ]              │  │
│  │                                      │  │
│  └──────────────────────────────────────┘  │
│  0 / 500 caracteres                        │
│                                            │
├────────────────────────────────────────────┤
│              [Cancelar] [Cerrar Necesidad] │
└────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Container | `<Dialog>` | shadcn/ui component |
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155]` |
| Dialog Header | `<DialogHeader>` | Con título y botón close |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` |
| Warning Alert | `<Alert>` | `bg-amber-900/20 border-amber-700 mb-4` |
| Warning Icon | `<AlertCircle>` | `w-5 h-5 text-amber-500` |
| Warning Text | `<p>` | `text-sm text-amber-300` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` |
| Motivo Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[100px]` |
| Character Counter | `<span>` | `text-xs text-[#64748b]` |
| Dialog Footer | `<DialogFooter>` | `flex gap-3 justify-end` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white` |
| Cerrar Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Textarea vacío, botón "Cerrar" enabled |
| **Typing** | Contador de caracteres actualizado |
| **Max Chars** | Textarea no permite más input, contador en rojo |
| **Submitting** | Botón "Cerrar" con spinner, disabled, textarea disabled |
| **Success** | Toast verde: "Necesidad cerrada. Las propuestas pendientes han sido rechazadas.", dialog cierra, listado actualizado |
| **Error** | Toast rojo con mensaje, botón vuelve a enabled |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click "Cancelar"** | Cierra diálogo sin cambios |
| **Click [×]** | Cierra diálogo |
| **Click "Cerrar Necesidad"** | PATCH a `/api/crowdsourcing/necesidades/{id}/cerrar` con motivo, cierra diálogo, actualiza vista |
| **Type en textarea** | Actualiza contador, valida max 500 chars |

---

## Componentes Compartidos

### NecesidadCard

**Props:**
```typescript
interface NecesidadCardProps {
  necesidad: {
    id: string;
    titulo: string;
    estado: EstadoNecesidad;
    tipoNecesidad: string;
    modalidad: "Presencial" | "Remoto" | "Hibrido";
    presupuestoMin?: number;
    presupuestoMax?: number;
    moneda?: string;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    fechaCreacion: Date;
    fechaLimitePropuestas?: Date;
    propuestasCount: number;
  };
  onClick?: () => void;
}
```

**Renderizado:**
- Card con hover effect
- Badge de estado (color según estado)
- Título destacado
- Meta info: tipo + modalidad
- Presupuesto formateado
- Stats: propuestas + fecha
- Actions: [Ver] [Editar] [Cerrar] (condicionales)

### PropuestaCard

**Props:**
```typescript
interface PropuestaCardProps {
  propuesta: {
    id: string;
    profesionalNombre: string;
    profesionalAvatar?: string;
    profesionalRol: string;
    profesionalRating?: number;
    profesionalReviewsCount?: number;
    mensaje: string;
    precioPropuesto: number;
    moneda: string;
    diasEstimados?: number;
    fechaEnvio: Date;
  };
  onVerPerfil: () => void;
  onAceptar?: () => void;
  onRechazar?: () => void;
  readonly?: boolean;
}
```

**Renderizado:**
- Card con info del profesional
- Avatar + nombre + rol + rating
- Mensaje de propuesta (expandible)
- Precio + tiempo estimado + fecha
- Botones de acción (si no readonly)

### EstadoBadge

**Props:**
```typescript
interface EstadoBadgeProps {
  estado: "Abierta" | "En Progreso" | "Cerrada" | "Cancelada";
}
```

**Renderizado:**
- Badge de shadcn/ui
- Color según estado:
  - Abierta: `bg-green-900/20 text-green-400 border-green-700`
  - En Progreso: `bg-blue-900/20 text-blue-400 border-blue-700`
  - Cerrada: `bg-gray-900/20 text-gray-400 border-gray-700`
  - Cancelada: `bg-red-900/20 text-red-400 border-red-700`

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 768px | Cards full width stacked, filtros verticales, grid 1 col, form inputs full width, actions stack vertical, sidebar collapsa |
| **Tablet** | 768px - 1024px | Cards grid 2 cols, filtros horizontal, form grid 2 cols parcial, sidebar visible |
| **Desktop** | > 1024px | Cards grid 2-3 cols, sidebar fijo, form grid 2 cols completo, todos los espacios optimizados |

### Mobile (< 768px)

**Listado:**
- Cards apiladas verticalmente (1 columna)
- Filtros stack vertical: estado arriba, search abajo
- Actions dentro de cada card (no dropdown)
- Paginación con números reducidos

**Form Crear/Editar:**
- Todo en 1 columna
- Inputs full width
- Presupuesto: min y max verticales
- Location: ciudad y país verticales
- Actions: botones full width stacked

**Detalle:**
- Info grid 1 columna
- Propuestas cards full width
- Actions stack vertical

### Tablet (768px - 1024px)

- Cards grid 2 columnas
- Form grid parcial (algunos campos 2 cols)
- Sidebar colapsable pero visible

### Desktop (> 1024px)

- Layout completo con sidebar fijo
- Cards grid 3 columnas (si espacio suficiente)
- Form grid 2 columnas optimizado
- Hover effects completos

---

## Animaciones y Transiciones

| Elemento | Animacion | Duración |
|----------|-----------|----------|
| **Card hover** | Background + border color transition + scale 1.01 + shadow increase | 200ms ease-out |
| **Badge appear** | Fade in | 150ms ease |
| **Form validation error** | Shake animation (horizontal 4px) + border color to red | 400ms ease |
| **Button hover** | Background gradient shift + scale 1.02 | 150ms ease |
| **Input focus** | Border color to primary + glow shadow | 200ms ease |
| **Dialog open** | Backdrop fade + content scale 0.95 → 1 | 200ms ease-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Loading spinner** | Rotate 360deg infinite | 1000ms linear |
| **Skeleton shimmer** | Background position shift (shimmer effect) | 1500ms ease-in-out infinite |
| **Propuestas list** | Stagger animation (cada item +50ms delay) | 200ms ease-out |
| **Character counter update** | Color transition (gray → red at limit) | 200ms ease |

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

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de color** | Mínimo 4.5:1 para texto normal. White (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 ✓ |
| **Focus visible** | Ring de 2px en `--border-primary` (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Labels en inputs** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Required fields** | Marcados con "*" visual y `aria-required="true"` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Loading states** | `aria-busy="true"` durante submit |
| **Estado badges** | `aria-label="Estado: {estado}"` para screen readers |
| **Date pickers** | Navegables con teclado (arrows, enter, escape) |
| **Dialogs** | `role="dialog"`, `aria-labelledby`, `aria-describedby`, trap focus dentro |
| **Cards navegables** | `tabindex="0"`, `role="article"`, Enter/Space para activar |
| **Propuestas count** | `aria-label="{N} propuestas recibidas"` |
| **Fecha límite warning** | `aria-label="Fecha límite próxima: {fecha}"` si < 7 días |

### ARIA Labels Ejemplos

```tsx
// Necesidad card
<Card
  tabIndex={0}
  role="article"
  aria-label={`${necesidad.titulo}. Estado: ${necesidad.estado}. ${necesidad.propuestasCount} propuestas. Presupuesto ${formatCurrency(necesidad.presupuestoMin)} a ${formatCurrency(necesidad.presupuestoMax)}`}
  onClick={handleClick}
  onKeyDown={(e) => (e.key === 'Enter' || e.key === ' ') && handleClick()}
>

// Form input con error
<Input
  id="titulo"
  aria-required="true"
  aria-invalid={!!errors.titulo}
  aria-describedby={errors.titulo ? "titulo-error" : undefined}
/>
{errors.titulo && (
  <p id="titulo-error" role="alert" className="text-sm text-red-400 mt-1">
    {errors.titulo}
  </p>
)}

// Estado badge
<Badge aria-label={`Estado: ${estado}`}>
  {estado}
</Badge>

// Loading button
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin mr-2" aria-hidden="true" />}
  {isPending ? "Publicando..." : "Publicar Necesidad"}
</Button>

// Dialog
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent
    role="dialog"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
  >
    <DialogTitle id="dialog-title">Cerrar Necesidad</DialogTitle>
    <DialogDescription id="dialog-description">
      Al cerrar esta necesidad, las propuestas pendientes serán rechazadas automáticamente.
    </DialogDescription>
  </DialogContent>
</Dialog>

// Propuestas count
<div aria-label={`${count} propuestas recibidas`}>
  <Users className="w-4 h-4" aria-hidden="true" />
  <Badge>{count}</Badge>
</div>
```

---

## Toast Messages

| Acción | Tipo | Mensaje |
|--------|------|---------|
| **Crear necesidad - success** | Success | "Necesidad publicada correctamente" |
| **Crear necesidad - error** | Error | "Error al publicar la necesidad. Inténtalo de nuevo." |
| **Editar necesidad - success** | Success | "Necesidad actualizada correctamente" |
| **Editar necesidad - error 400** | Error | "No se puede editar una necesidad cerrada o en progreso" |
| **Editar necesidad - error general** | Error | "Error al actualizar. Inténtalo de nuevo." |
| **Cerrar necesidad - success** | Success | "Necesidad cerrada. Las propuestas pendientes han sido rechazadas." |
| **Cerrar necesidad - error** | Error | "Error al cerrar la necesidad. Inténtalo de nuevo." |
| **Aceptar propuesta - success** | Success | "Propuesta aceptada. Se ha creado el acuerdo de trabajo." |
| **Aceptar propuesta - error** | Error | "Error al aceptar la propuesta. Inténtalo de nuevo." |
| **Rechazar propuesta - success** | Success | "Propuesta rechazada" |
| **Rechazar propuesta - error** | Error | "Error al rechazar la propuesta. Inténtalo de nuevo." |
| **Acceso denegado** | Error | "No tienes permiso para acceder a esta necesidad" |
| **No encontrada** | Error | "Necesidad no encontrada" |

---

## Loading Skeletons

### Listado (NecesidadCard Skeleton)

```tsx
<Card className="p-6">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-6 w-32" /> {/* Badge */}
  </div>
  <Skeleton className="h-7 w-3/4 mb-3" /> {/* Título */}
  <div className="flex gap-3 mb-2">
    <Skeleton className="h-5 w-24" /> {/* Tipo */}
    <Skeleton className="h-5 w-20" /> {/* Modalidad */}
  </div>
  <Skeleton className="h-5 w-32 mb-3" /> {/* Presupuesto */}
  <div className="flex gap-4 mb-4">
    <Skeleton className="h-4 w-24" /> {/* Propuestas */}
    <Skeleton className="h-4 w-28" /> {/* Fecha */}
  </div>
  <div className="flex gap-2 pt-3 border-t">
    <Skeleton className="h-8 w-20" /> {/* Botón */}
    <Skeleton className="h-8 w-20" />
    <Skeleton className="h-8 w-20" />
  </div>
</Card>
```

### Form (Create/Edit)

```tsx
<Card className="p-6">
  <Skeleton className="h-6 w-48 mb-4" /> {/* Section title */}
  <div className="space-y-4">
    <div>
      <Skeleton className="h-4 w-16 mb-2" /> {/* Label */}
      <Skeleton className="h-10 w-full" /> {/* Input */}
    </div>
    <div>
      <Skeleton className="h-4 w-24 mb-2" />
      <Skeleton className="h-24 w-full" /> {/* Textarea */}
    </div>
    <div className="grid grid-cols-2 gap-4">
      <div>
        <Skeleton className="h-4 w-32 mb-2" />
        <Skeleton className="h-10 w-full" />
      </div>
      <div>
        <Skeleton className="h-4 w-24 mb-2" />
        <Skeleton className="h-10 w-full" />
      </div>
    </div>
  </div>
</Card>
```

### Detalle (Header + Details)

```tsx
{/* Header */}
<Card className="p-6 mb-6">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-8 w-1/2" /> {/* Título */}
    <Skeleton className="h-6 w-24" /> {/* Badge */}
  </div>
  <div className="flex gap-3">
    <Skeleton className="h-9 w-24" /> {/* Botones */}
    <Skeleton className="h-9 w-32" />
  </div>
</Card>

{/* Details */}
<Card className="p-6 mb-6">
  <Skeleton className="h-6 w-32 mb-4" />
  <Skeleton className="h-20 w-full mb-4" /> {/* Descripción */}
  <div className="grid grid-cols-2 gap-4">
    {[1, 2, 3, 4].map(i => (
      <div key={i}>
        <Skeleton className="h-4 w-20 mb-1" />
        <Skeleton className="h-5 w-32" />
      </div>
    ))}
  </div>
</Card>
```

---

## Checklist UI/UX

### Pantalla 1: Mis Necesidades (Listado)
- [ ] Layout de dashboard con sidebar
- [ ] Header con título + subtítulo + botones de acción
- [ ] Filtros: estado select + search input con debounce 300ms
- [ ] Grid responsive de cards (1/2/3 cols según breakpoint)
- [ ] Cada card muestra: estado badge, título, tipo, modalidad, presupuesto, propuestas count, fecha, fecha límite
- [ ] Estado badges con colores correctos (Abierta=green, En Progreso=blue, Cerrada=gray, Cancelada=red)
- [ ] Fecha límite con warning si < 7 días, error si < 3 días
- [ ] Actions condicionales: Editar (solo Abierta), Cerrar (Abierta o En Progreso), Ver Acuerdo (En Progreso)
- [ ] Card hover effect (background, border, shadow)
- [ ] Empty state con ilustración + mensaje + CTAs [Nueva Necesidad] [Usar Plantilla]
- [ ] Loading skeletons (3-4 cards)
- [ ] Paginación funcional
- [ ] Navegación: click card → detalle, click botones → acciones
- [ ] Responsive mobile: cards stack 1 col, filtros vertical
- [ ] ARIA labels en cards
- [ ] Focus states visibles

### Pantalla 2: Crear Necesidad
- [ ] Formulario completo con secciones: Info Básica, Presupuesto, Ubicación y Fechas
- [ ] Todos los inputs con labels + placeholders + validación
- [ ] Required fields marcados con "*"
- [ ] Campo Tipo Necesidad populated desde maestras
- [ ] Campo Modalidad con opciones: Presencial, Remoto, Híbrido
- [ ] Ubicación (ciudad/país) condicional: visible solo si Presencial/Híbrido, oculta si Remoto
- [ ] Presupuesto: min - max + moneda select
- [ ] Fecha Límite y Fecha Inicio con DatePicker (solo >= hoy)
- [ ] Proyecto Artístico select populated con proyectos del artista
- [ ] Validación en tiempo real: blur inputs, min < max budget, required fields
- [ ] Character counter en descripción (max 4000)
- [ ] Botones: [Cancelar] [Publicar Necesidad]
- [ ] Loading state en submit (spinner + disabled)
- [ ] Toast success: "Necesidad publicada correctamente"
- [ ] Redirect a listado tras success
- [ ] Dialog confirmación si form dirty al cancelar
- [ ] Responsive: 1 col mobile, 2 cols desktop
- [ ] Form validation con Zod schema
- [ ] ARIA labels, required, invalid states

### Pantalla 3: Editar Necesidad
- [ ] Same layout que Crear pero título "Editar Necesidad"
- [ ] Form pre-rellenado con datos existentes
- [ ] Campo Tipo Necesidad DISABLED con tooltip explicativo
- [ ] Banner warning si tiene propuestas: "Esta necesidad ya tiene N propuestas..."
- [ ] Validación igual que Crear (excepto Tipo)
- [ ] Botón "Guardar Cambios" (no "Publicar")
- [ ] Loading fetch data (skeleton)
- [ ] PUT a API con actualización FechaActualizacion
- [ ] Toast success: "Necesidad actualizada"
- [ ] Error 400 si estado != Abierta: toast + redirect
- [ ] Redirect a detalle tras success

### Pantalla 4: Detalle de Necesidad
- [ ] Header: back link + título + estado badge + actions menu
- [ ] Actions: [Editar] (solo Abierta), [Cerrar] (Abierta o En Progreso)
- [ ] Card de detalles: descripción + info grid (tipo, modalidad, presupuesto, proyecto, fechas)
- [ ] Card de propuestas: header con count + lista de propuestas
- [ ] Cada propuesta: avatar, nombre, rol, rating, mensaje, precio, días, fecha
- [ ] Actions por propuesta: [Ver Perfil] [Aceptar] [Rechazar] (solo si Abierta)
- [ ] Empty state propuestas: "Aún no hay propuestas para esta necesidad"
- [ ] Badge "Fuera del rango" si precio propuesta < min o > max
- [ ] Propuestas readonly si estado != Abierta
- [ ] Click Aceptar → dialog confirmación → crea acuerdo
- [ ] Click Rechazar → dialog con textarea motivo
- [ ] Responsive: info grid 1 col mobile, 2 cols desktop
- [ ] Loading skeletons
- [ ] ARIA labels

### Pantalla 5: Diálogo de Cierre
- [ ] Dialog de shadcn/ui
- [ ] Título: "Cerrar Necesidad"
- [ ] Warning alert: "Al cerrar esta necesidad, las propuestas pendientes serán rechazadas automáticamente."
- [ ] Textarea motivo (opcional, max 500 chars)
- [ ] Character counter
- [ ] Botones: [Cancelar] [Cerrar Necesidad] (destructive)
- [ ] Loading state en submit
- [ ] Toast success: "Necesidad cerrada. Las propuestas pendientes han sido rechazadas."
- [ ] Dialog cierra tras success
- [ ] Focus trap dentro del dialog
- [ ] Escape key cierra dialog

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Design tokens usados (no valores hardcoded)
- [ ] shadcn/ui components correctos
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste mínimo 4.5:1 verificado
- [ ] Focus states con ring purple
- [ ] ARIA labels, roles y live regions
- [ ] Keyboard navigation completa
- [ ] Loading states con aria-busy
- [ ] Mobile-first responsive design
- [ ] Toast notifications consistentes
- [ ] Form validation con Zod + React Hook Form
- [ ] TanStack Query para data fetching
- [ ] Error handling con toasts
- [ ] Success flows con redirects
- [ ] Debounce en search (300ms)
