# UI/UX: Acuerdos, Milestones y Entregables

> **Feature:** cs-acuerdos-entregables
> **Ultima actualizacion:** 2026-02-18

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Explore Campaigns (referencia grid y cards) | [WPR_2-Excplore-Campaigns.png](../../ui-images/WPR_2-Excplore-Campaigns.png) | Landing |
| Detail Campaign (referencia layout de detalle) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue los patrones del wireframe textual definido en US-CS-04 y los patrones de la landing publica existente (cards, modales, secciones con sidebar).

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Detalle de acuerdo, formularios de milestone/entregable, dialogs de confirmacion | `references/templates/krowd/` |

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

  /* Estado Acuerdo */
  --estado-acuerdo-activo: #3b82f6;        /* Blue */
  --estado-acuerdo-activo-bg: #1e3a5f;
  --estado-acuerdo-completado: #10b981;    /* Green */
  --estado-acuerdo-completado-bg: #064e3b;
  --estado-acuerdo-cancelado: #ef4444;     /* Red */
  --estado-acuerdo-cancelado-bg: #450a0a;

  /* Estado Entregable */
  --estado-entregable-entregado: #f59e0b;  /* Amber/Yellow - pendiente de revision */
  --estado-entregable-entregado-bg: #451a03;
  --estado-entregable-aprobado: #10b981;   /* Green */
  --estado-entregable-aprobado-bg: #064e3b;
  --estado-entregable-rechazado: #ef4444;  /* Red */
  --estado-entregable-rechazado-bg: #450a0a;

  /* Milestone */
  --milestone-pendiente: #94a3b8;          /* Gray */
  --milestone-completado: #10b981;         /* Green */
  --milestone-progress-bg: #334155;
  --milestone-progress-fill: #a855f7;      /* Purple gradient */

  /* Destructive (cancelar) */
  --destructive: #dc2626;
  --destructive-hover: #b91c1c;
  --destructive-bg: #450a0a;

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
  --border-warning: #f59e0b;

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

## Pantalla 1: Aceptar Propuesta (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de necesidad (pantalla de US-CS-03)
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Aceptar propuesta                             [×]   │
│  Confirma los detalles del acuerdo a crear           │
├──────────────────────────────────────────────────────┤
│                                                      │
│  RESUMEN DE LA PROPUESTA                             │
│  ┌──────────────────────────────────────────────┐    │
│  │ Profesional: Studio Mix Pro                  │    │
│  │ Precio pactado: 450 EUR                      │    │
│  │ Mensaje: "Soy ingeniero de mezcla con 10..." │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Titulo interno del acuerdo *                        │
│  ┌──────────────────────────────────────────────┐    │
│  │  Mezcla de pistas para EP (default)          │    │
│  └──────────────────────────────────────────────┘    │
│  Nombre interno para identificar el acuerdo          │
│                                                      │
│  Fecha de inicio *        Fecha fin prevista         │
│  ┌─────────────────┐      ┌─────────────────────┐    │
│  │  2026-03-01     │      │  2026-03-15          │    │
│  └─────────────────┘      └─────────────────────┘    │
│  (default: hoy)           (default: hoy + 14 dias)   │
│                                                      │
│  ┌──────────────────────────────────────────────┐    │
│  │  (i) Al aceptar esta propuesta, las demas    │    │
│  │  propuestas pendientes seran rechazadas       │    │
│  │  automaticamente.                             │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                [Crear acuerdo]           │
└──────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  Aceptar propuesta            [×]   │
├─────────────────────────────────────┤
│  RESUMEN DE LA PROPUESTA            │
│  Profesional: Studio Mix Pro        │
│  Precio pactado: 450 EUR            │
│  Mensaje: "Soy ingeniero..."        │
│                                     │
│  Titulo interno *                   │
│  [Mezcla de pistas para EP      ]   │
│                                     │
│  Fecha de inicio *                  │
│  [2026-03-01                    ]   │
│                                     │
│  Fecha fin prevista                 │
│  [2026-03-15                    ]   │
│                                     │
│  (i) Al aceptar, las demas          │
│  propuestas seran rechazadas.       │
│                                     │
│  [Cancelar]   [Crear acuerdo]       │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Container | `<Dialog>` | `open={isOpen} onOpenChange={onClose}` |
| Dialog Content | `<DialogContent>` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Aceptar propuesta" |
| Dialog Subtitle | `<DialogDescription>` | `text-sm text-[#94a3b8] mt-1` "Confirma los detalles del acuerdo a crear" |
| Resumen Section | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-5` |
| Resumen Title | `<h3>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` "Resumen de la propuesta" |
| Resumen Profesional | `<p>` | `text-sm text-white font-medium` "Profesional: {nombre}" |
| Resumen Precio | `<p>` | `text-sm text-[#94a3b8]` "Precio pactado: {importe} {moneda}" |
| Resumen Mensaje | `<p>` | `text-sm text-[#94a3b8] italic truncate` Mensaje truncado a 80 chars |
| Form Body | `<div>` | `space-y-4` |
| Titulo Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Required Mark | `<span>` | `text-red-400 ml-1` aria-hidden="true" |
| Titulo Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7]` |
| Titulo Hint | `<p>` | `text-xs text-[#64748b] mt-1` |
| Fechas Row | `<div>` | `grid grid-cols-2 gap-3` |
| FechaInicio Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| FechaInicio Input | `<Input type="date">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| FechaFin Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| FechaFin Input | `<Input type="date">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Info Alert | `<Alert>` | `bg-blue-900/20 border-blue-700/50 mt-4` |
| Info Icon | `<Info>` | Lucide, `w-4 h-4 text-blue-400 flex-shrink-0` |
| Info Text | `<AlertDescription>` | `text-sm text-blue-300` |
| Error Message | `<p>` | `text-sm text-red-400 mt-1` con `role="alert"` |
| Dialog Footer | `<DialogFooter>` | `pt-4 border-t border-[#334155] flex justify-between gap-3` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Submit Button (loading) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4 mr-2" />` + "Creando acuerdo..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Form con valores predeterminados: titulo=nombre de necesidad, fechaInicio=hoy, fechaFin=hoy+diasEstimados |
| **Validation Error** | Border rojo en campos invalidos, mensaje de error debajo, submit disabled |
| **Submitting** | Spinner en boton + "Creando acuerdo...", todos los inputs disabled |
| **Success** | Dialog cierra, toast success "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional.", redirect a `/crowdsourcing/acuerdos/{acuerdoId}` |
| **Error API** | Toast error descriptivo, dialog permanece abierto, form re-habilitado |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Titulo interno** | Obligatorio, min 3, max 200 chars | "El titulo debe tener al menos 3 caracteres" |
| **Fecha de inicio** | Obligatorio | "La fecha de inicio es obligatoria" |
| **Fecha fin prevista** | Opcional, si se ingresa: >= fechaInicio | "La fecha fin no puede ser anterior a la fecha de inicio" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Dialog abre** | Campos prellenados con defaults: titulo=necesidad.titulo, fechaInicio=hoy, fechaFin=hoy+propuesta.diasEstimados |
| **Click [×] o Cancelar** | Cierra dialog sin cambios, form se resetea |
| **Press Escape** | Cierra dialog |
| **Click "Crear acuerdo"** | Valida form, POST a API, redirect a detalle si exitoso |

---

## Pantalla 2: Rechazar Propuesta (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de necesidad
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────┐
│  Rechazar propuesta                        [×]   │
├──────────────────────────────────────────────────┤
│                                                  │
│  ┌──────────────────────────────────────────┐    │
│  │  ⚠  Esta accion rechazara la propuesta   │    │
│  │  de Studio Mix Pro definitivamente.       │    │
│  └──────────────────────────────────────────┘    │
│                                                  │
│  Motivo (opcional)                               │
│  ┌──────────────────────────────────────────┐    │
│  │                                          │    │
│  │  El presupuesto no se ajusta a...        │    │
│  │                                          │    │
│  └──────────────────────────────────────────┘    │
│  0 / 500 caracteres                             │
│                                                  │
│  ┌──────────────────────────────────────────┐    │
│  │  (i) El profesional podra ver que su     │    │
│  │  propuesta fue rechazada pero NO el      │    │
│  │  motivo que escribas aqui.               │    │
│  └──────────────────────────────────────────┘    │
│                                                  │
├──────────────────────────────────────────────────┤
│  [Cancelar]               [Rechazar propuesta]   │
└──────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  Rechazar propuesta           [×]   │
├─────────────────────────────────────┤
│  ⚠ Esta accion rechazara la         │
│  propuesta definitivamente.         │
│                                     │
│  Motivo (opcional)                  │
│  [                              ]   │
│  [                              ]   │
│  0 / 500 caracteres                 │
│                                     │
│  (i) El profesional no vera         │
│  el motivo de rechazo.              │
│                                     │
│  [Cancelar] [Rechazar propuesta]    │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Rechazar propuesta" |
| Warning Alert | `<Alert>` | `bg-amber-900/20 border-amber-700/50 mb-4` |
| Warning Icon | `<AlertTriangle>` | Lucide, `w-4 h-4 text-amber-400 flex-shrink-0` |
| Warning Text | `<AlertDescription>` | `text-sm text-amber-300` "Esta accion rechazara la propuesta de {profesional} definitivamente." |
| Motivo Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Motivo (opcional)" |
| Motivo Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[100px] resize-none` |
| Motivo Counter | `<span>` | `text-xs text-[#64748b]` "{count} / 500 caracteres" |
| Info Alert | `<Alert>` | `bg-[#16213e] border-[#334155] mt-3` |
| Info Icon | `<Info>` | Lucide, `w-4 h-4 text-[#94a3b8] flex-shrink-0` |
| Info Text | `<AlertDescription>` | `text-sm text-[#94a3b8]` "El profesional podra ver que su propuesta fue rechazada pero NO el motivo que escribas aqui." |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Rechazar Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700 text-white` |
| Rechazar (loading) | `<Button disabled>` | Spinner + "Rechazando..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Textarea vacio, contador en 0 |
| **Typing** | Contador actualizado en tiempo real |
| **At limit (500)** | Contador en rojo, textarea no acepta mas caracteres |
| **Submitting** | Boton con spinner + "Rechazando...", inputs disabled |
| **Success** | Dialog cierra, toast: "Propuesta rechazada", card de propuesta actualiza badge a [RECHAZADA] |
| **Error** | Toast error, dialog cierra |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Type en motivo** | Actualiza contador, max 500 chars |
| **Click Cancelar o [×]** | Cierra sin cambios |
| **Press Escape** | Cierra sin cambios |
| **Click "Rechazar propuesta"** | PATCH a API con motivo opcional, cierra dialog, actualiza lista de propuestas |

---

## Pantalla 3: Detalle de Acuerdo

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/acuerdos/{id}`
**Template base:** `references/templates/krowd/` (detail pages)

### Layout Desktop

```
┌──────────────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                            │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  [< Mis acuerdos]                                                    │
│                                                                      │
│  ┌─────────────────────────────────────────────────────────────┐     │
│  │  HEADER DEL ACUERDO                                         │     │
│  │                                                             │     │
│  │  Mezcla EP Los Rockeros                          [ACTIVO]   │     │
│  │                                                             │     │
│  │  [Avatar] Los Rockeros  <->  [Avatar] Studio Mix Pro        │     │
│  │                                                             │     │
│  │  450 EUR  |  1 mar 2026 - 15 mar 2026                       │     │
│  │  Necesidad: "Mezcla de pistas para EP de 5 canciones"       │     │
│  └─────────────────────────────────────────────────────────────┘     │
│                                                                      │
│  ┌─────────────────────────────────────────────┐  ┌───────────────┐  │
│  │  SECCION MILESTONES                         │  │  SIDEBAR      │  │
│  │                                             │  │               │  │
│  │  Milestones              [+ Agregar]        │  │ Conversacion  │  │
│  │  (boton solo artista, acuerdo activo)       │  │               │  │
│  │  Asignado: 270 de 450 EUR (60%)             │  │ [Ir al chat   │  │
│  │  [==========>            ] (progress 60%)   │  │  con Studio   │  │
│  │                                             │  │  Mix Pro ->]  │  │
│  │  ┌─────────────────────────────────────┐    │  │               │  │
│  │  │ 1. Mezcla pistas 1-3    270 EUR     │    │  │ Detalles      │  │
│  │  │    Limite: 8 mar  [PENDIENTE]       │    │  │               │  │
│  │  │    Entregables:                     │    │  │ Inicio:       │  │
│  │  │    ┌────────────────────────────┐   │    │  │ 1 mar 2026    │  │
│  │  │    │ Mezcla cancion 1 v1        │   │    │  │               │  │
│  │  │    │ [ENTREGADO] | 5 mar        │   │    │  │ Fin previsto: │  │
│  │  │    │ [Aprobar] [Rechazar]       │   │    │  │ 15 mar 2026   │  │
│  │  │    │ (artista, acuerdo activo)  │   │    │  │               │  │
│  │  │    └────────────────────────────┘   │    │  │ Fin real:     │  │
│  │  │    [+ Subir entregable]             │    │  │ -             │  │
│  │  │    (profesional, acuerdo activo)    │    │  │               │  │
│  │  └─────────────────────────────────────┘    │  └───────────────┘  │
│  │                                             │                     │
│  │  ┌─────────────────────────────────────┐    │                     │
│  │  │ Sin milestone       (sin importe)   │    │                     │
│  │  │    ┌────────────────────────────┐   │    │                     │
│  │  │    │ Entregable sin milestone   │   │    │                     │
│  │  │    │ [APROBADO] | 3 mar         │   │    │                     │
│  │  │    └────────────────────────────┘   │    │                     │
│  │  └─────────────────────────────────────┘    │                     │
│  │                                             │                     │
│  │  TIMELINE                                   │                     │
│  │  - Entregable subido (hace 1 dia)           │                     │
│  │  - Milestone creado (hace 3 dias)           │                     │
│  │  - Acuerdo creado (hace 5 dias)             │                     │
│  │                                             │                     │
│  │  ┌─────────────────────────────────────┐    │                     │
│  │  │ [Completar acuerdo] [Cancelar acuerdo] │  │                     │
│  │  │ (artista solo)     (artista/profesional)│  │                     │
│  │  └─────────────────────────────────────┘    │                     │
│  └─────────────────────────────────────────────┘                     │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  [NAVBAR hamburger]                 │
├─────────────────────────────────────┤
│  [< Mis acuerdos]                   │
│                                     │
│  Mezcla EP Los Rockeros  [ACTIVO]   │
│  Los Rockeros <-> Studio Mix Pro    │
│  450 EUR | 1 mar - 15 mar 2026      │
│  Necesidad: Mezcla de pistas...     │
│                                     │
│  [Ir al chat con Studio Mix Pro ->] │
│                                     │
│  MILESTONES        [+ Agregar]      │
│  Asignado: 270 / 450 EUR (60%)      │
│  [=========>         ] 60%          │
│                                     │
│  1. Mezcla pistas 1-3   270 EUR     │
│     Limite: 8 mar | [PENDIENTE]     │
│                                     │
│     Mezcla cancion 1 v1             │
│     [ENTREGADO] 5 mar               │
│     [Aprobar] [Rechazar]            │
│                                     │
│     [+ Subir entregable]            │
│                                     │
│  TIMELINE                           │
│  - Entregable subido (hace 1 dia)   │
│  - Milestone creado (hace 3 dias)   │
│  - Acuerdo creado (hace 5 dias)     │
│                                     │
│  [Completar acuerdo]                │
│  [Cancelar acuerdo]                 │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-[#94a3b8] hover:text-white mb-6` con icono `ArrowLeft` |
| Page Layout | `<div>` | `max-w-6xl mx-auto px-4 py-8 flex gap-8 items-start` |
| Main Content | `<div>` | `flex-1 min-w-0` |
| Header Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Header Top Row | `<div>` | `flex items-start justify-between mb-4` |
| Titulo | `<h1>` | `text-2xl font-bold text-white leading-tight` |
| Estado Badge (Activo) | `<Badge>` | `bg-blue-900/30 text-blue-300 border-blue-700 text-sm px-3 py-1` |
| Estado Badge (Completado) | `<Badge>` | `bg-green-900/30 text-green-300 border-green-700 text-sm px-3 py-1` |
| Estado Badge (Cancelado) | `<Badge>` | `bg-red-900/30 text-red-300 border-red-700 text-sm px-3 py-1` |
| Parties Row | `<div>` | `flex items-center gap-4 mb-3` |
| Party Item | `<div>` | `flex items-center gap-2` |
| Party Avatar | `<Avatar>` | shadcn/ui, `w-8 h-8` |
| Party Name | `<span>` | `text-sm font-medium text-white` |
| Parties Separator | `<span>` | `text-[#64748b]` "<->" |
| Importe Row | `<div>` | `flex items-center gap-2 text-[#94a3b8] text-sm mb-2` |
| Importe Value | `<span>` | `text-lg font-bold text-white` |
| Dates Row | `<div>` | `text-sm text-[#94a3b8]` |
| Necesidad Link | `<Link>` | `text-sm text-[#a855f7] hover:text-purple-300 mt-2 block` |
| Milestones Section Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Milestones Header Row | `<div>` | `flex items-center justify-between mb-4` |
| Milestones Title | `<h2>` | `text-base font-semibold text-white` "Milestones" |
| Agregar Milestone Btn | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 text-sm` + icono `Plus` - solo artista + acuerdo activo |
| Progreso Label | `<p>` | `text-sm text-[#94a3b8] mb-2` "Asignado: {asignado} de {total} {moneda} ({pct}%)" |
| Progreso Bar | `<Progress>` | shadcn/ui, `h-2 bg-[#334155]` con indicator `bg-gradient-to-r from-pink-500 to-purple-600` |
| Milestones List | `<div>` | `space-y-4 mt-4` |
| MilestoneItem | Ver "Componentes Reutilizables" | |
| Sidebar | `<aside>` | `w-72 flex-shrink-0 hidden md:block sticky top-24 space-y-4` |
| Chat Link Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4` |
| Chat Link | `<Link>` | `flex items-center gap-2 text-[#a855f7] hover:text-purple-300 font-medium` con icono `MessageSquare` |
| Detalles Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4` |
| Detalle Row | `<div>` | `flex flex-col gap-1 mb-3` |
| Detalle Label | `<span>` | `text-xs text-[#64748b] uppercase tracking-wide` |
| Detalle Value | `<span>` | `text-sm font-medium text-white` |
| Timeline Section | `<div>` | `mt-6` |
| Timeline Title | `<h3>` | `text-sm font-semibold text-[#64748b] uppercase tracking-wide mb-3` |
| Timeline List | `<ul>` | `space-y-3` |
| Timeline Item | `<li>` | `flex items-start gap-3` |
| Timeline Dot | `<div>` | `w-2 h-2 rounded-full bg-[#334155] mt-1.5 flex-shrink-0` |
| Timeline Text | `<span>` | `text-sm text-[#94a3b8]` |
| Timeline Date | `<span>` | `text-xs text-[#64748b] ml-1` |
| Actions Row | `<div>` | `flex gap-3 mt-8 pt-6 border-t border-[#334155]` |
| Completar Btn | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600` + icono `CheckCircle` - solo artista + activo |
| Cancelar Btn | `<Button variant="outline">` | `border-red-700/50 text-red-400 hover:bg-red-900/20 hover:border-red-600` + icono `XCircle` - artista/profesional + activo |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton: header card (titulo + parties + importe), milestones section (3 milestone skeletons), sidebar |
| **Loaded (Activo)** | Contenido completo. Botones Agregar milestone (artista), Subir entregable (profesional), Aprobar/Rechazar (artista sobre entregados), Completar/Cancelar |
| **Loaded (Completado)** | Header badge verde. Botones Aprobar/Rechazar/Agregar/Subir NO visibles. Seccion valoraciones habilitada. Sin botones de accion al pie |
| **Loaded (Cancelado)** | Header badge rojo. Todos los botones de accion ocultos. Banner informativo con fecha de cancelacion y quien cancelo |
| **Error 403** | Icono `Lock` + "No tienes acceso a este acuerdo" + link "Volver al inicio" |
| **Error 404** | Icono `AlertCircle` + "Acuerdo no encontrado" + link "Volver al inicio" |
| **Milestone sugerencia completado** | Alert verde inline debajo del milestone: "Todos los entregables han sido aprobados. ¿Deseas marcar este milestone como completado?" con boton [Marcar completado] |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click [< Mis acuerdos]** | Navega a `/crowdsourcing/mis-acuerdos` |
| **Click [+ Agregar milestone]** | Abre Dialog Formulario de Milestone (Pantalla 4) |
| **Click icono editar milestone** | Abre Dialog Formulario de Milestone con datos prellenados |
| **Click icono eliminar milestone** | Abre Dialog confirmacion de eliminacion (simple) |
| **Click [+ Subir entregable]** | Abre Dialog Formulario de Entregable (Pantalla 5) |
| **Click [Aprobar]** | Abre Dialog Aprobar Entregable (Pantalla 6) |
| **Click [Rechazar]** | Abre Dialog Rechazar Entregable (Pantalla 7) |
| **Click URL del entregable** | Abre en nueva pestaña (`target="_blank" rel="noopener noreferrer"`) |
| **Click [Ir al chat]** | Navega a `/crowdsourcing/conversaciones/{conversacionId}` |
| **Click [Completar acuerdo]** | Abre Dialog Completar Acuerdo (Pantalla 8) |
| **Click [Cancelar acuerdo]** | Abre Dialog Cancelar Acuerdo (Pantalla 9) |
| **Click [Marcar completado] (milestone)** | PATCH milestone, actualiza estado inline, recalcula barra de progreso |

---

## Pantalla 4: Formulario de Milestone (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Nuevo milestone                               [×]   │
│  (o "Editar milestone" si es edicion)                │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Titulo *                                            │
│  ┌──────────────────────────────────────────────┐    │
│  │  Mezcla de pistas 1-3                        │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Descripcion                                         │
│  ┌──────────────────────────────────────────────┐    │
│  │                                              │    │
│  │  Mezcla de las primeras 3 canciones del EP   │    │
│  │                                              │    │
│  └──────────────────────────────────────────────┘    │
│  0 / 1000 caracteres                                 │
│                                                      │
│  Importe parcial *   EUR       Fecha limite          │
│  ┌──────────────┐               ┌───────────────┐    │
│  │  270.00      │               │  2026-03-08   │    │
│  └──────────────┘               └───────────────┘    │
│                                                      │
│  ┌──────────────────────────────────────────────┐    │
│  │ [=====>      ] Asignado: 270 de 450 EUR (60%)│    │
│  │  Disponible: 180 EUR                         │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                  [Crear milestone]       │
│                              (o "Guardar cambios")   │
└──────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Nuevo milestone" o "Editar milestone" |
| Titulo Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Titulo Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Descripcion Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Descripcion Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| Descripcion Counter | `<span>` | `text-xs text-[#64748b]` "{count} / 1000 caracteres" |
| Importe + Fecha Row | `<div>` | `grid grid-cols-2 gap-3` |
| Importe Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Importe parcial * (EUR)" |
| Importe Input | `<Input type="number">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` step="0.01" min="0.01" |
| Fecha Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Fecha limite" |
| Fecha Input | `<Input type="date">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Progreso Indicator | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-3 mt-4` |
| Progreso Bar | `<Progress>` | `h-2 mb-2` valor = (importeAsignadoTotal + importeActual) / importeTotal * 100 |
| Progreso Text | `<p>` | `text-sm text-[#94a3b8]` "Asignado: {total} de {pactado} EUR ({pct}%)" |
| Disponible Text | `<p>` | `text-xs text-[#64748b] mt-1` "Disponible: {disponible} EUR" |
| Progreso Error | `<p>` | `text-sm text-red-400` visible solo cuando suma supera total "El importe supera el total pactado ({total} EUR)" |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit Button | `<Button>` | gradient, disabled si importe invalido o suma excede total |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (nuevo)** | Campos vacios, indicador de progreso mostrando el estado actual de milestones |
| **Default (editar)** | Campos prellenados con datos del milestone, progreso excluyendo el importe actual del milestone editado |
| **Typing importe** | Indicador de progreso actualizado en tiempo real |
| **Supera total** | Barra de progreso en rojo, texto de error visible, submit disabled |
| **Validation Error** | Borders rojos en campos invalidos, mensajes de error |
| **Submitting** | Spinner + "Creando milestone..." o "Guardando...", inputs disabled |
| **Success** | Dialog cierra, milestone aparece en la lista, barra de progreso actualizada |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Titulo** | Obligatorio, min 3, max 200 chars | "El titulo debe tener al menos 3 caracteres" |
| **Importe parcial** | Obligatorio, > 0 | "El importe parcial debe ser mayor a 0" |
| **Importe parcial** | SUM(todos) <= total pactado | "El importe supera el total pactado ({total} EUR)" |
| **Descripcion** | Opcional, max 1000 chars | "Maximo 1000 caracteres" |
| **Fecha limite** | Opcional, si ingresada: >= fechaInicio del acuerdo | "La fecha limite no puede ser anterior a la fecha de inicio del acuerdo" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Change importe** | Recalcula indicador de progreso en tiempo real (sin debounce) |
| **Click Cancelar o [×]** | Cierra sin cambios |
| **Click "Crear milestone"** | POST a API, cierra dialog, actualiza lista de milestones y barra de progreso |
| **Click "Guardar cambios"** | PUT a API, cierra dialog, actualiza milestone editado |

---

## Pantalla 5: Formulario de Entregable (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Subir entregable                              [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Titulo *                                            │
│  ┌──────────────────────────────────────────────┐    │
│  │  Mezcla cancion 1 - v1                       │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Descripcion                                         │
│  ┌──────────────────────────────────────────────┐    │
│  │  Primera version de la mezcla de la...       │    │
│  └──────────────────────────────────────────────┘    │
│  0 / 1000 caracteres                                 │
│                                                      │
│  URL del recurso                                     │
│  ┌──────────────────────────────────────────────┐    │
│  │  https://drive.google.com/file/...           │    │
│  └──────────────────────────────────────────────┘    │
│  Enlace a Dropbox, Google Drive, WeTransfer, etc.    │
│                                                      │
│  Milestone asociado                                  │
│  ┌──────────────────────────────────────────────┐    │
│  │  Mezcla de pistas 1-3                     v  │    │
│  └──────────────────────────────────────────────┘    │
│  Opcional - vincula este entregable a un milestone   │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                     [Subir entregable]   │
└──────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Subir entregable" |
| Titulo Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Descripcion Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| URL Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "URL del recurso" |
| URL Input | `<Input type="url">` | `bg-[#1a1a2e] border-[#334155] text-white h-11` placeholder="https://..." |
| URL Hint | `<p>` | `text-xs text-[#64748b] mt-1` "Enlace a Dropbox, Google Drive, WeTransfer, etc." |
| URL Icon Row | Prefijo icono `Link2` en Input | `text-[#64748b]` dentro del input |
| URL Valid Icon | `<CheckCircle>` | `w-4 h-4 text-green-400` visible a la derecha del input cuando URL es valida |
| Milestone Select Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Milestone Select | `<Select>` | shadcn/ui, `bg-[#1a1a2e] border-[#334155] text-white` opciones: "Sin milestone" + lista de milestones del acuerdo |
| Milestone Hint | `<p>` | `text-xs text-[#64748b] mt-1` "Opcional - vincula este entregable a un milestone" |
| Submit Button | `<Button>` | gradient + icono `Upload` |
| Submit (loading) | `<Button disabled>` | Spinner + "Subiendo..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Campos vacios, Milestone select en "Sin milestone" |
| **URL valida** | Icono checkmark verde en el input de URL |
| **URL invalida (blur)** | Border rojo + mensaje de error |
| **Submitting** | Spinner + "Subiendo...", inputs disabled |
| **Success** | Dialog cierra, toast: "Entregable subido correctamente. El artista sera notificado.", entregable aparece en lista del milestone |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Titulo** | Obligatorio, min 3, max 200 | "El titulo debe tener al menos 3 caracteres" |
| **URL del recurso** | Opcional, si ingresada: URL valida | "Debe ser una URL valida (ej: https://drive.google.com/...)" |
| **Descripcion** | Opcional, max 1000 chars | "Maximo 1000 caracteres" |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Type en URL** | Valida formato URL al hacer blur, muestra icono verde si valida |
| **Select milestone** | Actualiza valor del select |
| **Click "Subir entregable"** | POST a API, cierra dialog, actualiza lista de entregables del milestone |

---

## Pantalla 6: Aprobar Entregable (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Aprobar entregable                            [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ENTREGABLE                                          │
│  ┌──────────────────────────────────────────────┐    │
│  │ Mezcla cancion 1 - v1                        │    │
│  │ Primera version de la mezcla de la cancion 1 │    │
│  │ [Enlace: https://drive.google.com/...]        │    │
│  │ Subido: 5 mar 2026                           │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Comentario (opcional)                               │
│  ┌──────────────────────────────────────────────┐    │
│  │  Excelente mezcla, me encanta el resultado   │    │
│  └──────────────────────────────────────────────┘    │
│  0 / 500 caracteres                                  │
│                                                      │
│  ┌──────────────────────────────────────────────┐    │
│  │ (i) Si todos los entregables de este         │    │
│  │ milestone quedan aprobados, podras marcar     │    │
│  │ el milestone como completado.                 │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                    [Aprobar entregable]  │
└──────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Aprobar entregable" |
| Entregable Info Card | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-4` |
| Entregable Titulo | `<p>` | `text-sm font-semibold text-white mb-1` |
| Entregable Desc | `<p>` | `text-sm text-[#94a3b8] mb-2` |
| Entregable URL | `<a>` | `text-sm text-[#a855f7] hover:text-purple-300 flex items-center gap-1` con icono `ExternalLink`, target="_blank" |
| Entregable Fecha | `<p>` | `text-xs text-[#64748b] mt-1` |
| Comentario Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Comentario (opcional)" |
| Comentario Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| Contador | `<span>` | `text-xs text-[#64748b]` |
| Info Alert | `<Alert>` | `bg-[#16213e] border-[#334155] mt-3` |
| Info Text | `<AlertDescription>` | `text-sm text-[#94a3b8]` (solo si el milestone tiene mas entregables) |
| Submit Button | `<Button>` | `bg-green-600 hover:bg-green-700 text-white` + icono `CheckCircle` |
| Submit (loading) | `<Button disabled>` | Spinner + "Aprobando..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Datos del entregable visibles, textarea vacio |
| **Submitting** | Spinner, inputs disabled |
| **Success** | Dialog cierra, toast "Entregable aprobado", badge del entregable cambia a [APROBADO], botones Aprobar/Rechazar desaparecen. Si todos los entregables del milestone aprobados: alerta inline sugiriendo marcar milestone como completado |
| **Success (todos aprobados en milestone)** | Adicionalmente aparece en la pagina: Alert verde "Todos los entregables han sido aprobados. ¿Marcar milestone como completado?" con boton de accion |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click cancelar o [×]** | Cierra sin cambios |
| **Click "Aprobar entregable"** | PATCH a API, cierra dialog, actualiza estado |
| **Click URL del entregable** | Abre en nueva pestana |

---

## Pantalla 7: Rechazar Entregable (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Rechazar entregable                           [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ENTREGABLE                                          │
│  ┌──────────────────────────────────────────────┐    │
│  │ Mezcla cancion 1 - v1                        │    │
│  │ [Enlace: https://drive.google.com/...]        │    │
│  │ Subido: 5 mar 2026                           │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  ┌──────────────────────────────────────────────┐    │
│  │  ⚠  El profesional podra ver este comentario │    │
│  │  y subir una nueva version del entregable.   │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Motivo del rechazo *                                │
│  ┌──────────────────────────────────────────────┐    │
│  │  La voz esta demasiado baja en el coro...    │    │
│  └──────────────────────────────────────────────┘    │
│  Minimo 10 caracteres. 45 / 500                      │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                   [Rechazar entregable]  │
└──────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Rechazar entregable" |
| Entregable Info Card | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-4` (igual que Pantalla 6) |
| Warning Alert | `<Alert>` | `bg-amber-900/20 border-amber-700/50 mb-4` |
| Warning Text | `<AlertDescription>` | `text-sm text-amber-300` "El profesional podra ver este comentario y subir una nueva version del entregable." |
| Motivo Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Motivo del rechazo *" |
| Motivo Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[100px] resize-none` placeholder="Explica que debe corregirse..." |
| Counter Row | `<div>` | `flex justify-between mt-1` |
| Counter Min Text | `<span>` | `text-xs text-[#64748b]` "Minimo 10 caracteres" |
| Counter Current | `<span>` | `text-xs text-[#64748b]` "{count} / 500" |
| Error Text | `<p>` | `text-sm text-red-400 mt-1` visible cuando < 10 chars y se intento submit |
| Submit Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700` + icono `XCircle` |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Motivo** | Obligatorio, min 10, max 500 | "El motivo debe tener al menos 10 caracteres explicando que debe corregirse" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Textarea vacio, submit disabled hasta que haya 10+ chars |
| **Insufficient chars** | Submit disabled, texto del contador en amber si < 10 chars y el usuario ha escrito algo |
| **Valid** | Submit habilitado |
| **Submitting** | Spinner + "Rechazando...", inputs disabled |
| **Success** | Dialog cierra, toast: "Entregable rechazado. El profesional sera notificado.", badge cambia a [RECHAZADO], botones Aprobar/Rechazar desaparecen |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Type en motivo** | Habilita submit cuando >= 10 chars, contador actualizado |
| **Click "Rechazar entregable"** | PATCH a API, cierra dialog, actualiza estado del entregable |

---

## Pantalla 8: Completar Acuerdo (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Completar acuerdo                             [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  RESUMEN DEL ACUERDO                                 │
│  ┌──────────────────────────────────────────────┐    │
│  │ Milestones completados: 1 de 1               │    │
│  │ Entregables aprobados: 1 de 1                │    │
│  │ Importe total pactado: 450 EUR               │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  (aviso solo si hay entregables pendientes)          │
│  ┌──────────────────────────────────────────────┐    │
│  │  ⚠  Hay 2 entregables sin revisar.           │    │
│  │  Puedes completar el acuerdo igualmente,     │    │
│  │  pero se recomienda revisar todos los        │    │
│  │  entregables antes de completar.             │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Al completar el acuerdo:                            │
│  - El acuerdo pasara a estado Completado             │
│  - La necesidad pasara a estado Cerrada              │
│  - Se habilitaran las valoraciones                   │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                      [Completar acuerdo] │
└──────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Completar acuerdo" |
| Resumen Card | `<div>` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-4` |
| Resumen Title | `<h3>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` |
| Resumen Stat Row | `<div>` | `flex items-center justify-between py-1 border-b border-[#334155] last:border-0` |
| Resumen Stat Label | `<span>` | `text-sm text-[#94a3b8]` |
| Resumen Stat Value | `<span>` | `text-sm font-semibold text-white` |
| Warning Alert (pendientes) | `<Alert>` | `bg-amber-900/20 border-amber-700/50 mb-4` - solo visible si hay entregables sin revisar |
| Warning Icon | `<AlertTriangle>` | `w-4 h-4 text-amber-400` |
| Warning Text | `<AlertDescription>` | `text-sm text-amber-300` |
| Efectos List | `<ul>` | `space-y-1 text-sm text-[#94a3b8] mt-3` |
| Efecto Item | `<li>` | `flex items-center gap-2` con icono `ChevronRight w-3 h-3 text-green-400` |
| Submit Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600` + icono `CheckCircle` |
| Submit (loading) | `<Button disabled>` | Spinner + "Completando..." |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Sin entregables pendientes** | Solo resumen y lista de efectos, sin warning |
| **Con entregables pendientes** | Warning amber visible con conteo de pendientes |
| **Submitting** | Spinner, botones disabled |
| **Success** | Dialog cierra, toast "Acuerdo completado. Puedes dejar una valoracion al profesional.", pagina se recarga/actualiza: badge cambia a [COMPLETADO], botones de accion desaparecen |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Click Cancelar o [×]** | Cierra sin cambios |
| **Click "Completar acuerdo"** | PATCH a API, cierra dialog, actualiza estado del acuerdo |

---

## Pantalla 9: Cancelar Acuerdo (Dialog)

**Proyecto:** Landing (`src/web`)
**Contexto:** Dialog overlay sobre el detalle de acuerdo
**Template base:** shadcn/ui Dialog

### Layout Desktop

```
┌──────────────────────────────────────────────────────┐
│  Cancelar acuerdo                              [×]   │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌──────────────────────────────────────────────┐    │
│  │  ⚠  Cancelar un acuerdo es una accion        │    │
│  │  irreversible. Ambas partes seran notificadas │    │
│  │  y la necesidad volvera a estar Abierta.      │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Motivo de la cancelacion *                          │
│  ┌──────────────────────────────────────────────┐    │
│  │                                              │    │
│  │  No puedo continuar por motivos personales.  │    │
│  │  Lamento las molestias causadas.             │    │
│  │                                              │    │
│  └──────────────────────────────────────────────┘    │
│  Minimo 20 caracteres. 58 / 1000                     │
│                                                      │
│  Al cancelar el acuerdo:                             │
│  - El acuerdo pasara a estado Cancelado              │
│  - La necesidad volvera a estado Abierto             │
│  - Los entregables y milestones se conservan         │
│    como historial                                    │
│                                                      │
├──────────────────────────────────────────────────────┤
│  [Volver]                        [Cancelar acuerdo]  │
│                                  (boton destructivo) │
└──────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────┐
│  Cancelar acuerdo             [×]   │
├─────────────────────────────────────┤
│  ⚠ Cancelar un acuerdo es una       │
│  accion irreversible. Ambas         │
│  partes seran notificadas.          │
│                                     │
│  Motivo de la cancelacion *         │
│  [                              ]   │
│  [                              ]   │
│  [                              ]   │
│  Minimo 20 caracteres. 0 / 1000     │
│                                     │
│  Al cancelar:                       │
│  - Acuerdo -> Cancelado             │
│  - Necesidad -> Abierta             │
│  - Historial conservado             │
│                                     │
│  [Volver]  [Cancelar acuerdo]       │
└─────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` "Cancelar acuerdo" |
| Danger Alert | `<Alert>` | `bg-red-900/20 border-red-700/50 mb-4` |
| Danger Icon | `<AlertTriangle>` | `w-5 h-5 text-red-400 flex-shrink-0` |
| Danger Text | `<AlertDescription>` | `text-sm text-red-300` "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas y la necesidad volvera a estar Abierta." |
| Motivo Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Motivo de la cancelacion *" |
| Motivo Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none` placeholder="Explica el motivo de la cancelacion..." |
| Counter Row | `<div>` | `flex justify-between mt-1` |
| Counter Min | `<span>` | `text-xs text-[#64748b]` "Minimo 20 caracteres" |
| Counter Current | `<span>` | `text-xs text-[#64748b]` (color amber si < 20 y usuario ha escrito) |
| Efectos List | `<ul>` | `space-y-1 text-sm text-[#94a3b8] mt-4 mb-1` |
| Efecto Item | `<li>` | `flex items-center gap-2` con icono `ChevronRight w-3 h-3 text-red-400` |
| Volver Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Cancelar Submit Btn | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700 text-white disabled:opacity-50` - disabled si < 20 chars |
| Submit (loading) | `<Button disabled>` | Spinner + "Cancelando..." |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Motivo** | Obligatorio, min 20, max 1000 | "El motivo debe tener al menos 20 caracteres" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Textarea vacio, submit disabled |
| **Chars < 20** | Contador en amber, submit disabled |
| **Chars >= 20** | Submit habilitado |
| **Submitting** | Spinner + "Cancelando...", inputs disabled, ambos botones disabled |
| **Success** | Dialog cierra, toast "Acuerdo cancelado", pagina actualiza badge a [CANCELADO], botones de accion desaparecen, banner informativo de cancelacion visible |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| **Type en motivo** | Actualiza contador, habilita submit cuando >= 20 chars |
| **Click Volver o [×]** | Cierra sin cambios |
| **Click "Cancelar acuerdo"** | PATCH a API, cierra dialog, actualiza toda la pagina de acuerdo |

---

## Componentes Reutilizables

### MilestoneItem

**Archivo sugerido:** `src/web/src/features/crowdsourcing/components/MilestoneItem.tsx`

**Props:**
```typescript
interface MilestoneItemProps {
  milestone: {
    id: string;
    titulo: string;
    descripcion?: string;
    orden: number;
    importeParcial: number;
    porcentajeParcial: number;
    fechaLimite?: string;
    fechaCompletado?: string;
    entregables: EntregableItem[];
  };
  acuerdoId: string;
  monedaNombre: string;
  acuerdoActivo: boolean;
  miRol: "Artista" | "Profesional";
  onEntregableAprobado?: (entregableId: string) => void;
  onEntregableRechazado?: (entregableId: string) => void;
}
```

**Layout interno:**

```
┌──────────────────────────────────────────────────────┐
│  N. {titulo}                 {importe} EUR  [icono editar] [icono eliminar]  │
│     Limite: {fecha} | [{ESTADO}]                     │
│                                                      │
│  (entregables agrupados aqui)                        │
│  ┌──────────────────────────────────────────────┐    │
│  │ EntregableRow                                │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  [+ Subir entregable] (solo profesional + activo)    │
└──────────────────────────────────────────────────────┘
```

**Estilos:**
```
Container: bg-[#16213e] border border-[#334155] rounded-lg p-4
Header row: flex items-start justify-between
Numero: text-sm font-bold text-[#a855f7] mr-2
Titulo: text-sm font-semibold text-white
Importe: text-sm font-medium text-white ml-auto
Estado Pendiente badge: bg-gray-900/30 border-gray-600 text-gray-300
Estado Completado badge: bg-green-900/30 border-green-600 text-green-300
Fecha limite: text-xs text-[#64748b]
Fecha limite (proximo): text-xs text-amber-400
Fecha limite (vencido): text-xs text-red-400
Edit/Delete icons: text-[#64748b] hover:text-white w-4 h-4 cursor-pointer
```

**Logica de acciones:**
- Boton editar (icono `Pencil`): visible solo si `miRol === "Artista"` y `acuerdoActivo` y `!milestone.fechaCompletado`
- Boton eliminar (icono `Trash2`): visible solo si `miRol === "Artista"` y `acuerdoActivo` y `!milestone.fechaCompletado` y `entregables.length === 0`
- Boton [+ Subir entregable]: visible solo si `miRol === "Profesional"` y `acuerdoActivo`

---

### EntregableRow

**Archivo sugerido:** `src/web/src/features/crowdsourcing/components/EntregableRow.tsx`

**Props:**
```typescript
interface EntregableRowProps {
  entregable: {
    id: string;
    titulo: string;
    descripcion?: string;
    urlRecurso?: string;
    estadoEntregableNombre: "Entregado" | "Aprobado" | "Rechazado";
    fechaCreacion: string;
    comentarioAprobacion?: string;
    comentarioRechazo?: string;
  };
  acuerdoActivo: boolean;
  miRol: "Artista" | "Profesional";
  onAprobar?: (id: string) => void;
  onRechazar?: (id: string) => void;
}
```

**Layout interno:**

```
┌──────────────────────────────────────────────────────┐
│  {titulo}                              [ESTADO]      │
│  {descripcion truncada 80 chars}                     │
│  [Enlace externo ->] | {fecha formateada}            │
│  (comentario si existe, en cursiva)                  │
│                                                      │
│  [Aprobar] [Rechazar]  (si ENTREGADO y es artista)   │
└──────────────────────────────────────────────────────┘
```

**Estilos:**
```
Container: pl-4 py-3 border-l-2 border-[#334155] mb-2 last:mb-0
Titulo: text-sm font-medium text-white
Descripcion: text-xs text-[#94a3b8] mt-0.5
URL Link: text-xs text-[#a855f7] hover:text-purple-300 flex items-center gap-1 mt-1
Fecha: text-xs text-[#64748b]
Comentario: text-xs text-[#64748b] italic mt-1
Acciones: flex gap-2 mt-2
```

**Estado Badges:**

| Estado | Estilos |
|--------|---------|
| Entregado | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aprobado | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazado | `bg-red-900/30 border-red-600 text-red-300` |

**Botones de accion (solo si estado=Entregado y miRol=Artista y acuerdoActivo):**

```tsx
<Button
  size="sm"
  className="h-7 px-3 text-xs bg-green-600 hover:bg-green-700"
  onClick={() => onAprobar(entregable.id)}
>
  <CheckCircle className="w-3 h-3 mr-1" />
  Aprobar
</Button>
<Button
  size="sm"
  variant="outline"
  className="h-7 px-3 text-xs border-red-700/50 text-red-400 hover:bg-red-900/20"
  onClick={() => onRechazar(entregable.id)}
>
  <XCircle className="w-3 h-3 mr-1" />
  Rechazar
</Button>
```

**Borde lateral segun estado:**
```
Entregado: border-l-2 border-amber-600/50
Aprobado:  border-l-2 border-green-600/50
Rechazado: border-l-2 border-red-600/50
Default:   border-l-2 border-[#334155]
```

---

### AcuerdoDetailSkeleton

**Uso:** Loading state de la pagina de detalle de acuerdo.

```tsx
// Header skeleton
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-8 w-2/3" />
    <Skeleton className="h-6 w-20 rounded-full" />
  </div>
  <div className="flex items-center gap-4 mb-3">
    <Skeleton className="h-8 w-8 rounded-full" />
    <Skeleton className="h-4 w-32" />
    <Skeleton className="h-4 w-8" />
    <Skeleton className="h-8 w-8 rounded-full" />
    <Skeleton className="h-4 w-32" />
  </div>
  <Skeleton className="h-4 w-48 mb-2" />
  <Skeleton className="h-4 w-64" />
</Card>

// Milestones skeleton
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <div className="flex justify-between mb-4">
    <Skeleton className="h-5 w-28" />
    <Skeleton className="h-8 w-32 rounded-md" />
  </div>
  <Skeleton className="h-2 w-full mb-4 rounded-full" />
  {[1, 2].map(i => (
    <div key={i} className="bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-3">
      <Skeleton className="h-5 w-3/4 mb-2" />
      <Skeleton className="h-4 w-24 mb-3" />
      <Skeleton className="h-12 w-full rounded" />
    </div>
  ))}
</Card>
```

---

### EstadoAcuerdoBadge

**Props:** `estado: "Activo" | "Completado" | "Cancelado"`

| Estado | Estilos |
|--------|---------|
| Activo | `bg-blue-900/30 border-blue-700 text-blue-300` |
| Completado | `bg-green-900/30 border-green-700 text-green-300` |
| Cancelado | `bg-red-900/30 border-red-700 text-red-300` |

```tsx
const estadoStyles = {
  Activo: "bg-blue-900/30 border-blue-700 text-blue-300",
  Completado: "bg-green-900/30 border-green-700 text-green-300",
  Cancelado: "bg-red-900/30 border-red-700 text-red-300",
};

<Badge className={`${estadoStyles[estado]} border text-sm px-3 py-1`}>
  {estado}
</Badge>
```

---

## Estados UI Globales

### Loading Skeletons

| Pantalla | Cantidad | Descripcion |
|----------|----------|-------------|
| Detalle de Acuerdo | 1 compuesto | Header + 2 milestones + sidebar |

### Error States

| Tipo | Icono | Titulo | Descripcion | Accion |
|------|-------|--------|-------------|--------|
| **Red error** | `WifiOff` | "Error de conexion" | "No se pudo cargar el acuerdo" | [Reintentar] |
| **Not Found** | `SearchX` | "Acuerdo no encontrado" | "El acuerdo que buscas no existe o ya no esta disponible" | [Volver al inicio] |
| **Forbidden** | `Lock` | "Acceso restringido" | "No tienes permiso para ver este acuerdo" | [Volver al inicio] |

### Empty States

| Pantalla | Icono | Titulo | Descripcion | Accion |
|----------|-------|--------|-------------|--------|
| Acuerdo sin milestones (artista) | `Milestone` | "Sin milestones definidos" | "Define los hitos de trabajo para estructurar el acuerdo." | [+ Agregar milestone] |
| Acuerdo sin milestones (profesional) | `Milestone` | "Sin milestones definidos" | "El artista aun no ha definido los hitos de trabajo." | - |
| Milestone sin entregables (profesional) | `Upload` | "Sin entregables" | "Sube tu primer entregable para este milestone." | [+ Subir entregable] |
| Milestone sin entregables (artista) | `FileCheck` | "Sin entregables" | "El profesional aun no ha subido entregables para este milestone." | - |

### Toast Notifications

| Accion | Tipo | Mensaje |
|--------|------|---------|
| **Aceptar propuesta - success** | Success (verde) | "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." |
| **Aceptar propuesta - ya existe acuerdo** | Error (rojo) | "Ya existe un acuerdo activo para esta necesidad" |
| **Rechazar propuesta - success** | Success (verde) | "Propuesta rechazada" |
| **Crear milestone - success** | Success (verde) | "Milestone creado" |
| **Editar milestone - success** | Success (verde) | "Milestone actualizado" |
| **Eliminar milestone - success** | Success (verde) | "Milestone eliminado" |
| **Eliminar milestone - error (tiene entregables)** | Error (rojo) | "No se puede eliminar un milestone con entregables asociados" |
| **Subir entregable - success** | Success (verde) | "Entregable subido correctamente. El artista sera notificado." |
| **Aprobar entregable - success** | Success (verde) | "Entregable aprobado" |
| **Rechazar entregable - success** | Warning (amber) | "Entregable rechazado. El profesional sera notificado." |
| **Marcar milestone completado - success** | Success (verde) | "Milestone marcado como completado" |
| **Completar acuerdo - success** | Success (verde) | "Acuerdo completado. Puedes dejar una valoracion al profesional." |
| **Cancelar acuerdo - success** | Warning (amber) | "Acuerdo cancelado" |
| **Error generico** | Error (rojo) | "Error inesperado. Intentalo de nuevo." |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios Principales |
|------------|-------|---------------------|
| **Mobile** | < 640px | Layout 1 columna, sidebar se convierte en seccion inline al inicio, dialogs full-width, acciones al pie de cada seccion |
| **Tablet** | 640px - 1024px | Layout 2 columnas (main + sidebar), dialogs con max-w-lg |
| **Desktop** | > 1024px | Layout 2 columnas (main flex-1 + sidebar w-72 sticky), hover effects activos |

### Mobile (< 640px)

**Detalle de Acuerdo:**
- Header card full-width con info condensada (titulo, estado, partes en columna, importe, fechas)
- Link al chat como banner full-width inmediatamente debajo del header
- Seccion milestones full-width, sin sidebar
- Detalles del acuerdo (fechas, etc.) colapsados en un accordion "Ver detalles" debajo del chat link
- Botones de accion (Completar, Cancelar) como botones full-width al pie de la pagina
- Entregables con botones Aprobar/Rechazar en columna (no fila) si pantalla muy estrecha

**Dialogs en mobile:**
- `DialogContent` con `sm:max-w-lg` y en pantallas < 640px: full width con `mx-4`
- Fecha y importe row cambia a columna: `grid grid-cols-1`

### Tablet (640px - 1024px)

- Layout 2 columnas: main (flex-1) + sidebar (w-64)
- Dialogs con max-w-lg centrados

### Desktop (> 1024px)

- Layout 2 columnas: main (flex-1 min-w-0) + sidebar (w-72 flex-shrink-0 sticky top-24)
- Hover effects en milestone items y entregable rows
- Tooltips en botones de icono (editar/eliminar milestone)

---

## Interacciones y Animaciones

| Elemento | Animacion | Duracion | Easing |
|----------|-----------|----------|--------|
| **Dialog open** | Backdrop fade-in + content scale 0.95 -> 1 + translateY(4px -> 0) | 200ms | ease-out |
| **Dialog close** | Backdrop fade-out + content scale 1 -> 0.95 | 150ms | ease-in |
| **Progress bar update** | Transition de width | 300ms | ease-out |
| **Milestone item hover** | Background change a #1e2a42 | 150ms | ease |
| **Button hover (primary gradient)** | Gradient shift + scale(1.02) | 150ms | ease |
| **Button hover (destructive)** | Background darkening | 150ms | ease |
| **Entregable row appear** | Fade-in + translateY(-4px -> 0) tras crear | 200ms | ease-out |
| **Badge estado change** | Fade-in del nuevo badge | 250ms | ease |
| **Alert/suggestion appear** | Fade-in + height expand | 300ms | ease-out |
| **Toast appear** | Slide-in from right + fade | 250ms | ease-out |
| **Toast disappear** | Slide-out to right + fade | 200ms | ease-in |
| **Button loading spinner** | Rotate 360deg infinite | 1000ms | linear |
| **Skeleton shimmer** | Background position shift | 1500ms | ease-in-out infinite |
| **Input focus** | Border color to purple + box-shadow glow | 200ms | ease |
| **Submit button disabled** | Opacity 50% transition | 150ms | ease |

### Shake Animation (Error de Validacion al Submit)

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
| **Contraste minimo** | 4.5:1 para texto normal. White (#fff) sobre bg-card (#0f1729) = 15.8:1. Amber (#f59e0b) sobre fondo oscuro verificar >= 4.5:1 |
| **Focus visible** | Ring de 2px solid `#a855f7` con offset de 2px en todos los elementos interactivos. `focus-visible:ring-2 focus-visible:ring-purple-500 focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| **Labels en inputs** | Todos los inputs tienen `<Label>` con `htmlFor` apuntando al `id` del input |
| **Required fields** | Marcados con "*" visual (span aria-hidden="true") y `aria-required="true"` en el input |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="assertive"` |
| **Loading states** | `aria-busy="true"` en botones durante submit. Skeletons con `role="status" aria-label="Cargando..."` |
| **Dialog** | `role="dialog"`, `aria-modal="true"`, `aria-labelledby`, `aria-describedby`, focus trap dentro del dialog, Escape para cerrar |
| **Estado badges** | `aria-label="Estado: {estado}"` para screen readers |
| **Progress bar** | `<Progress>` con `aria-label="Importe asignado: {pct}% del total"` aria-valuenow, aria-valuemin, aria-valuemax |
| **Entregable URL** | `aria-label="Ver entregable: {titulo} (abre en nueva pestana)"` |
| **Botones de icono** | `aria-label` descriptivo cuando el texto es solo icono (editar milestone, eliminar milestone) |
| **Timeline** | `role="list"` en la lista, `role="listitem"` en cada item |
| **Keyboard navigation** | Tab/Shift+Tab para navegar, Enter/Space para activar botones, Escape para cerrar dialogs |
| **Contadores de caracteres** | `aria-live="polite"` para actualizaciones del contador |
| **Acciones condicionales por rol** | Los botones no disponibles para el rol actual deben estar ausentes del DOM (no solo disabled), para no confundir a usuarios de screen reader |

### ARIA Labels Ejemplos

```tsx
// Progress bar de milestones
<Progress
  value={porcentajeAsignado}
  aria-label={`Importe asignado: ${porcentajeAsignado}% del total pactado`}
  aria-valuenow={porcentajeAsignado}
  aria-valuemin={0}
  aria-valuemax={100}
/>

// Boton editar milestone (solo icono)
<button
  aria-label={`Editar milestone: ${milestone.titulo}`}
  onClick={() => onEdit(milestone.id)}
>
  <Pencil className="w-4 h-4" aria-hidden="true" />
</button>

// Boton eliminar milestone (solo icono)
<button
  aria-label={`Eliminar milestone: ${milestone.titulo}`}
  onClick={() => onDelete(milestone.id)}
>
  <Trash2 className="w-4 h-4" aria-hidden="true" />
</button>

// URL de entregable
<a
  href={entregable.urlRecurso}
  target="_blank"
  rel="noopener noreferrer"
  aria-label={`Ver entregable: ${entregable.titulo} (abre en nueva pestana)`}
>
  <ExternalLink className="w-3 h-3 mr-1" aria-hidden="true" />
  Ver entregable
</a>

// Estado badge del acuerdo
<Badge aria-label={`Estado del acuerdo: ${estadoAcuerdoNombre}`}>
  {estadoAcuerdoNombre}
</Badge>

// Dialog de cancelar acuerdo
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent
    role="dialog"
    aria-modal="true"
    aria-labelledby="cancelar-acuerdo-title"
    aria-describedby="cancelar-acuerdo-desc"
  >
    <DialogTitle id="cancelar-acuerdo-title">Cancelar acuerdo</DialogTitle>
    <DialogDescription id="cancelar-acuerdo-desc">
      Esta accion es irreversible. El acuerdo pasara a estado Cancelado
      y la necesidad volvera a estar Abierta.
    </DialogDescription>
  </DialogContent>
</Dialog>

// Loading button
<Button
  disabled={isPending}
  aria-busy={isPending}
  aria-label={isPending ? "Completando acuerdo..." : "Completar acuerdo"}
>
  {isPending && <Loader2 className="animate-spin w-4 h-4 mr-2" aria-hidden="true" />}
  {isPending ? "Completando..." : "Completar acuerdo"}
</Button>

// Skeleton loading
<div role="status" aria-label="Cargando detalle del acuerdo...">
  <AcuerdoDetailSkeleton />
</div>

// Timeline
<ul role="list" aria-label="Historial de actividad">
  {timeline.map((item, i) => (
    <li key={i} role="listitem">
      <span className="sr-only">{item.fecha}: </span>
      {item.accion} por {item.actor}
    </li>
  ))}
</ul>
```

---

## Notas de Implementacion

### Data Fetching (TanStack Query)

```typescript
// src/web/src/features/crowdsourcing/hooks/useAcuerdo.ts
export const useAcuerdo = (acuerdoId: string) => {
  return useQuery({
    queryKey: ['acuerdo', acuerdoId],
    queryFn: () => crowdsourcingService.getAcuerdo(acuerdoId),
    staleTime: 15_000,   // 15 segundos (datos mas dinamicos que listados)
  });
};

// src/web/src/features/crowdsourcing/hooks/useAceptarPropuesta.ts
export const useAceptarPropuesta = (propuestaId: string) => {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  return useMutation({
    mutationFn: (data: AceptarPropuestaDto) =>
      crowdsourcingService.aceptarPropuesta(propuestaId, data),
    onSuccess: (response) => {
      // Redirect a la pagina del acuerdo recien creado
      navigate(`/crowdsourcing/acuerdos/${response.data.acuerdoId}`);
      queryClient.invalidateQueries({ queryKey: ['necesidades'] });
      queryClient.invalidateQueries({ queryKey: ['mis-propuestas'] });
    },
  });
};

// src/web/src/features/crowdsourcing/hooks/useCrearMilestone.ts
export const useCrearMilestone = (acuerdoId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CrearMilestoneDto) =>
      crowdsourcingService.crearMilestone(acuerdoId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['acuerdo', acuerdoId] });
    },
  });
};

// src/web/src/features/crowdsourcing/hooks/useSubirEntregable.ts
export const useSubirEntregable = (acuerdoId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: SubirEntregableDto) =>
      crowdsourcingService.subirEntregable(acuerdoId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['acuerdo', acuerdoId] });
    },
  });
};

// src/web/src/features/crowdsourcing/hooks/useAprobarEntregable.ts
export const useAprobarEntregable = (acuerdoId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ entregableId, comentario }: AprobarEntregableDto) =>
      crowdsourcingService.aprobarEntregable(entregableId, { comentario }),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ['acuerdo', acuerdoId] });
      // Si todosAprobadosEnMilestone=true, mostrar sugerencia en UI
    },
  });
};

// src/web/src/features/crowdsourcing/hooks/useCompletarAcuerdo.ts
export const useCompletarAcuerdo = (acuerdoId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => crowdsourcingService.completarAcuerdo(acuerdoId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['acuerdo', acuerdoId] });
    },
  });
};
```

### State Management (Dialog State)

Cada dialog se controla con estado local en la pagina de detalle:

```typescript
// src/web/src/features/crowdsourcing/pages/AcuerdoDetailPage.tsx

const [milestoneDialog, setMilestoneDialog] = useState<{
  open: boolean;
  milestone?: MilestoneDto;  // si es edicion
}>({ open: false });

const [entregableDialog, setEntregableDialog] = useState<{
  open: boolean;
  milestoneId?: string;  // milestone por defecto
}>({ open: false });

const [aprobarDialog, setAprobarDialog] = useState<{
  open: boolean;
  entregable?: EntregableDto;
}>({ open: false });

const [rechazarEntregableDialog, setRechazarEntregableDialog] = useState<{
  open: boolean;
  entregable?: EntregableDto;
}>({ open: false });

const [completarDialog, setCompletarDialog] = useState(false);
const [cancelarDialog, setCancelarDialog] = useState(false);
```

### Calculo de Progreso de Milestones

```typescript
// El campo importeAsignado viene del backend
// La barra se calcula:
const porcentajeAsignado = acuerdo.importeTotalPactado > 0
  ? Math.min(100, Math.round((acuerdo.importeAsignado / acuerdo.importeTotalPactado) * 100))
  : 0;

// Para el formulario de milestone (calculo en tiempo real):
const calcularProgresoConNuevoMilestone = (
  importeActual: number,
  importeExistente: number,  // suma de otros milestones (excluyendo el editado)
  totalPactado: number
): { porcentaje: number; superaTotal: boolean } => {
  const total = importeExistente + importeActual;
  return {
    porcentaje: Math.min(100, Math.round((total / totalPactado) * 100)),
    superaTotal: total > totalPactado,
  };
};
```

### Formularios (React Hook Form + Zod)

```typescript
// Schema para aceptar propuesta
const aceptarPropuestaSchema = z.object({
  tituloInterno: z.string()
    .min(3, "El titulo debe tener al menos 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  fechaInicio: z.string().min(1, "La fecha de inicio es obligatoria"),
  fechaFinPrevista: z.string().optional(),
});

// Schema para crear milestone
const milestoneSchema = z.object({
  titulo: z.string()
    .min(3, "Minimo 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  descripcion: z.string().max(1000, "Maximo 1000 caracteres").optional(),
  importeParcial: z.number({ required_error: "El importe es obligatorio" })
    .positive("El importe debe ser mayor a 0"),
  fechaLimite: z.string().optional(),
});

// Schema para subir entregable
const entregableSchema = z.object({
  titulo: z.string()
    .min(3, "Minimo 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  descripcion: z.string().max(1000, "Maximo 1000 caracteres").optional(),
  urlRecurso: z.string().url("Debe ser una URL valida").optional().or(z.literal("")),
  milestoneId: z.string().optional(),
});

// Schema para rechazar entregable
const rechazarEntregableSchema = z.object({
  comentario: z.string()
    .min(10, "Minimo 10 caracteres explicando que debe corregirse")
    .max(500, "Maximo 500 caracteres"),
});

// Schema para cancelar acuerdo
const cancelarAcuerdoSchema = z.object({
  motivo: z.string()
    .min(20, "Minimo 20 caracteres")
    .max(1000, "Maximo 1000 caracteres"),
});
```

### Ruta Protegida

La ruta `/crowdsourcing/acuerdos/:id` requiere autenticacion. Si el usuario no esta autenticado, redirigir a `/login?returnUrl=/crowdsourcing/acuerdos/{id}`.

Si el usuario esta autenticado pero no es participante del acuerdo, la API devuelve 403 y se muestra el estado de error Forbidden.

---

## Checklist UI/UX

### Pantalla 1: Aceptar Propuesta (Dialog)
- [ ] Dialog shadcn/ui con titulo "Aceptar propuesta"
- [ ] Resumen de la propuesta: nombre profesional, precio, mensaje truncado
- [ ] Campo titulo interno (prellenado con nombre de necesidad)
- [ ] Campos fecha inicio (prellenado con hoy) y fecha fin prevista (prellenado con hoy+diasEstimados)
- [ ] Info alert sobre rechazo automatico de otras propuestas
- [ ] Validacion: titulo min 3 chars, fechaInicio obligatoria, fechaFin >= fechaInicio
- [ ] Submit gradient "Crear acuerdo"
- [ ] Loading state: spinner + "Creando acuerdo...", inputs disabled
- [ ] Success: dialog cierra + toast + redirect a detalle del acuerdo
- [ ] Error API: toast + dialog permanece abierto
- [ ] Escape y [×] cierran sin cambios
- [ ] Focus trap dentro del dialog
- [ ] aria-modal, aria-labelledby, aria-describedby

### Pantalla 2: Rechazar Propuesta (Dialog)
- [ ] Dialog con titulo "Rechazar propuesta"
- [ ] Warning alert amber sobre accion definitiva
- [ ] Textarea motivo opcional (max 500 chars)
- [ ] Contador de caracteres
- [ ] Info: "El profesional no vera el motivo"
- [ ] Boton destructivo "Rechazar propuesta" (rojo)
- [ ] Loading state: spinner + "Rechazando..."
- [ ] Success: dialog cierra + toast + badge actualizado a [RECHAZADA]
- [ ] Escape y [×] cierran sin cambios

### Pantalla 3: Detalle de Acuerdo
- [ ] Back link "< Mis acuerdos" con ArrowLeft
- [ ] Layout 2 columnas desktop (main + sidebar), 1 columna mobile
- [ ] Header: titulo, estado badge (color correcto segun estado), parties (avatar + nombre), importe, fechas, link a necesidad
- [ ] Seccion milestones: titulo "Milestones", boton "[+ Agregar]" (solo artista + activo)
- [ ] Indicador progreso: texto "Asignado: X de Y EUR (Z%)" + barra Progress
- [ ] Lista de milestones con MilestoneItem
- [ ] Cada MilestoneItem: numero + titulo + importe + estado badge + fecha limite + entregables agrupados
- [ ] Botones editar/eliminar milestone (iconos, solo artista + activo + no completado)
- [ ] Entregables con EntregableRow: titulo + estado badge + fecha + URL link
- [ ] Botones [Aprobar] [Rechazar] en entregables con estado=Entregado (solo artista + activo)
- [ ] Boton [+ Subir entregable] por milestone (solo profesional + activo)
- [ ] Grupo "Sin milestone" para entregables sin milestone asignado
- [ ] Sidebar: link al chat, detalles del acuerdo (fechas, estado necesidad)
- [ ] Timeline: lista de actividad reciente con fecha relativa
- [ ] Botones de accion al pie: [Completar] (solo artista) [Cancelar] (artista y profesional)
- [ ] Botones NO visibles cuando acuerdo no esta activo
- [ ] Alert/sugerencia "Marcar milestone completado" cuando todos sus entregables estan aprobados
- [ ] Loading skeleton completo
- [ ] Error 403 y 404 con mensajes apropiados
- [ ] ARIA labels en progress bar, botones de icono, timeline

### Pantalla 4: Formulario de Milestone (Dialog)
- [ ] Dialog con titulo "Nuevo milestone" o "Editar milestone"
- [ ] Campos: titulo (min 3), descripcion (max 1000), importe parcial (> 0), fecha limite (opcional)
- [ ] Indicador de progreso con calculo en tiempo real al cambiar importe
- [ ] Error visible cuando suma de importes supera el total pactado
- [ ] Submit disabled si suma supera total o campos invalidos
- [ ] Zod schema + React Hook Form
- [ ] Loading: spinner + "Creando milestone..." o "Guardando..."
- [ ] Success: dialog cierra + milestone aparece en lista + barra progreso actualizada

### Pantalla 5: Formulario de Entregable (Dialog)
- [ ] Dialog con titulo "Subir entregable"
- [ ] Campos: titulo (min 3), descripcion (max 1000), URL (opcional, validada), milestone select
- [ ] Icono de URL validada (checkmark verde) cuando URL es valida
- [ ] Select de milestones: opciones "Sin milestone" + lista de milestones del acuerdo
- [ ] Zod schema + React Hook Form
- [ ] Loading: spinner + "Subiendo..."
- [ ] Success: dialog cierra + toast + entregable aparece en lista del milestone

### Pantalla 6: Aprobar Entregable (Dialog)
- [ ] Dialog con titulo "Aprobar entregable"
- [ ] Info del entregable: titulo, descripcion, URL (link externo), fecha
- [ ] Textarea comentario opcional (max 500 chars)
- [ ] Info sobre sugerencia de milestone completado (si aplica)
- [ ] Boton verde "Aprobar entregable" con icono CheckCircle
- [ ] Success: toast + badge [APROBADO] + botones Aprobar/Rechazar desaparecen
- [ ] Si todos aprobados en milestone: alert sugerencia aparece en pagina

### Pantalla 7: Rechazar Entregable (Dialog)
- [ ] Dialog con titulo "Rechazar entregable"
- [ ] Info del entregable igual que pantalla 6
- [ ] Warning alert amber: "El profesional podra ver este comentario"
- [ ] Textarea motivo obligatorio (min 10, max 500)
- [ ] Submit disabled hasta que haya 10+ chars
- [ ] Boton destructivo "Rechazar entregable" con icono XCircle
- [ ] Success: toast + badge [RECHAZADO] + botones desaparecen

### Pantalla 8: Completar Acuerdo (Dialog)
- [ ] Dialog con titulo "Completar acuerdo"
- [ ] Resumen: milestones completados, entregables aprobados, importe total
- [ ] Warning amber si hay entregables sin revisar (con conteo)
- [ ] Lista de efectos de la accion (bullets)
- [ ] Boton gradient "Completar acuerdo" con icono CheckCircle
- [ ] Success: toast + badge [COMPLETADO] + botones de accion desaparecen

### Pantalla 9: Cancelar Acuerdo (Dialog)
- [ ] Dialog con titulo "Cancelar acuerdo"
- [ ] Alert rojo: "Cancelar un acuerdo es una accion irreversible"
- [ ] Textarea motivo obligatorio (min 20, max 1000)
- [ ] Submit disabled hasta que haya 20+ chars
- [ ] Lista de efectos (bullets con icono rojo)
- [ ] Boton "Volver" (outline) + boton destructivo "Cancelar acuerdo" (rojo)
- [ ] Success: toast + badge [CANCELADO] + botones de accion desaparecen

### Componentes Reutilizables
- [ ] MilestoneItem con props tipadas, logica de acciones por rol
- [ ] EntregableRow con estado badges de colores correctos y borde lateral segun estado
- [ ] AcuerdoDetailSkeleton completo
- [ ] EstadoAcuerdoBadge con los 3 estados y colores

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Nuevos tokens CSS para estados de acuerdo y entregable
- [ ] shadcn/ui: Button, Card, Badge, Dialog, Input, Textarea, Select, Progress, Avatar, Alert, Label, Skeleton
- [ ] Tailwind utilities sin valores hardcoded
- [ ] Animaciones smooth (150-300ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring purple en todos los interactivos
- [ ] ARIA labels, roles, live regions en todos los componentes
- [ ] Keyboard navigation: Tab, Enter, Space, Escape
- [ ] Loading states con aria-busy y role="status"
- [ ] Mobile-first responsive (dialogs, layout, botones)
- [ ] Toast notifications con shadcn/ui Toaster
- [ ] Zod schemas + React Hook Form para todos los formularios
- [ ] TanStack Query: useQuery (staleTime 15s), useMutation con invalidateQueries
- [ ] Ruta /crowdsourcing/acuerdos/:id protegida (redirect a login si no autenticado)
- [ ] Acciones condicionales por `miRol`: ausentes del DOM cuando no aplican (no solo disabled)
- [ ] Entregables agrupados por milestone en el frontend (algunos sin milestone en grupo "Sin milestone")
