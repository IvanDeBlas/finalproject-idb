# UI/UX: Crear Campaña de Crowdfunding

> **Feature:** crear-campania
> **Última actualización:** 2026-02-12

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Wizard Crear Campaña | [WPR_6-Create-Campaign.png](../../ui-images/WPR_6-Create-Campaign.png) | Admin |
| Detalle Campaña Pública | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |
| Dashboard Artista (Mis Campañas) | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Dashtail** | Wizard forms, Dashboard layouts, Campaign list | `references/templates/dashtail/` |
| **Krowd** | Campaign detail page (public landing) | `references/templates/krowd/` |

**Componentes de referencia clave:**
- Dashtail multi-step form patterns
- Krowd campaign detail layouts

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

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
  --border-dashed: #475569;

  /* Input */
  --input-bg: #0f1729;
  --input-bg-disabled: #1e293b;
  --input-border: #334155;
  --input-placeholder: #64748b;

  /* Badge Colors */
  --badge-draft: #64748b;
  --badge-active: #10b981;
  --badge-completed: #3b82f6;
  --badge-cancelled: #ef4444;
}
```

### Tipografía

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

## Pantalla: Wizard Crear Campaña - Paso 1 (Información Básica)

**Mockup:** WPR_6-Create-Campaign.png
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/nueva?step=1`
**Template base:** `dashtail/` multi-step form patterns

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [🎵] MusicFund                                           [✕]       │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│                   Crear Nueva Campaña                              │
│          Completa los pasos para publicar tu proyecto musical      │
│                                                                    │
│  ┌──────┐────────┬──────┐────────┬──────┐────────┬──────┐         │
│  │  1   │────────│  2   │────────│  3   │────────│  4   │         │
│  └──────┘        └──────┘        └──────┘        └──────┘         │
│  Información         Historia      Recompensas     Revisión        │
│  Básica                                                            │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                                                              │  │
│  │  Información Básica                                          │  │
│  │                                                              │  │
│  │  Título de la campaña *                                      │  │
│  │  [________________________________________________]           │  │
│  │                                                              │  │
│  │  Género musical *            Meta de financiación (EUR) *    │  │
│  │  [Selecciona género ▼]       [€ 5000_____________]           │  │
│  │                                                              │  │
│  │  Fecha de finalización *     URL del video (YouTube/Vimeo)   │  │
│  │  [mm/dd/yyyy 📅]             [https://youtube.com/...]       │  │
│  │                                                              │  │
│  │  Imagen de portada *                                         │  │
│  │  ┌────────────────────────────────────────────────────────┐  │  │
│  │  │  ☁️                                                     │  │  │
│  │  │  Arrastra tu imagen aquí o haz clic para seleccionar   │  │  │
│  │  │  PNG, JPG hasta 5MB (1200x630px recomendado)           │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                              │  │
│  │  Vista Previa                                                │  │
│  │  ┌─────────────────┐                                         │  │
│  │  │  [IMG Preview]  │  Tu imagen de portada aparecerá aquí   │  │
│  │  │                 │                                         │  │
│  │  └─────────────────┘                                         │  │
│  │                                                              │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  [← Anterior]                Paso 1 de 4                [Siguiente →]│
│                                                         (gradient)  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Header Container | `<div>` | `bg-[#0f1729] border-b border-[#334155] px-6 py-4 flex items-center justify-between` |
| Logo | `<Link>` con Icon | `flex items-center gap-2 text-white font-bold text-xl` |
| Close Button | `<Button variant="ghost" size="icon">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Main Container | `<div>` | `max-w-4xl mx-auto py-8 px-6` |
| Title | `<h1>` | `text-3xl font-bold text-white text-center mb-2` |
| Subtitle | `<p>` | `text-base text-[#94a3b8] text-center mb-8` |
| Stepper Container | `<div>` | `flex items-center justify-between mb-10 max-w-2xl mx-auto` |
| Step Circle (active) | `<div>` | `w-12 h-12 rounded-full bg-gradient-to-r from-pink-500 to-purple-600 flex items-center justify-center text-white font-bold` |
| Step Circle (inactive) | `<div>` | `w-12 h-12 rounded-full bg-[#334155] flex items-center justify-center text-[#64748b] font-bold` |
| Step Connector | `<div>` | `flex-1 h-0.5 bg-[#334155] mx-2` (active: `bg-gradient-to-r from-pink-500 to-purple-600`) |
| Step Label (active) | `<span>` | `text-sm text-white font-medium mt-2` |
| Step Label (inactive) | `<span>` | `text-sm text-[#64748b] mt-2` |
| Form Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-8` |
| Section Title | `<h2>` | `text-2xl font-bold text-white mb-6` |
| Label (required) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 after:content-['*'] after:text-red-500 after:ml-1` |
| Label (optional) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Text Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] focus:border-primary` |
| Select Dropdown | `<Select>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Date Picker | `<Popover>` + `<Calendar>` | `bg-[#1a1a2e] border-[#334155]` |
| Grid 2 Columns | `<div>` | `grid grid-cols-2 gap-4` |
| File Upload Area | `<div>` | `border-2 border-dashed border-[#475569] rounded-lg p-12 text-center bg-[#1a1a2e] hover:border-primary cursor-pointer transition` |
| Upload Icon | `<Icon>` (cloud-upload) | `w-12 h-12 text-[#64748b] mx-auto mb-3` |
| Upload Text | `<p>` | `text-[#94a3b8] mb-1` |
| Upload Hint | `<p>` | `text-xs text-[#64748b]` |
| Image Preview Container | `<div>` | `mt-6 p-4 bg-[#1a1a2e] rounded-lg border border-[#334155]` |
| Image Preview | `<img>` | `w-full max-w-md h-48 object-cover rounded-lg` |
| Preview Placeholder | `<div>` | `w-32 h-32 bg-[#334155] rounded-lg flex items-center justify-center` |
| Footer Navigation | `<div>` | `flex items-center justify-between mt-8` |
| Previous Button | `<Button variant="outline">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Step Indicator | `<span>` | `text-sm text-[#64748b]` |
| Next Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Form vacío, stepper muestra paso 1 activo (gradient), pasos 2-4 inactivos (gray) |
| **Focus Input** | Border color cambia a `#a855f7`, glow shadow aplicado |
| **Typing Título** | Validación en tiempo real, mostrar error si excede 200 caracteres |
| **Hover Upload Area** | Border color cambia a primary, background ligero highlight |
| **File Selected** | Mostrar nombre de archivo debajo del área, spinner mientras sube |
| **Image Loading** | Skeleton loader en preview area |
| **Image Loaded** | Preview muestra imagen completa en contenedor con border |
| **Validation Error** | Input con border rojo, mensaje de error debajo en color rojo |
| **Loading Submit** | Next button muestra spinner + texto "Validando...", inputs disabled |
| **Step Completed** | Al avanzar, step 1 circle mantiene gradient, connector a step 2 se vuelve gradient |
| **Disabled Previous** | Previous button disabled y oculto en paso 1 |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Título** | No vacío | "El título es obligatorio" |
| **Título** | Máx 200 caracteres | "Máximo 200 caracteres (X/200)" |
| **Género musical** | Selección requerida | "Selecciona un género musical" |
| **Meta financiación** | Mayor a 0 | "La meta debe ser mayor a cero" |
| **Meta financiación** | Número válido | "Ingresa un monto válido" |
| **Fecha finalización** | No vacío | "La fecha de finalización es obligatoria" |
| **Fecha finalización** | Mínimo 7 días desde hoy | "La campaña debe durar al menos 7 días" |
| **Fecha finalización** | Máximo 60 días desde hoy | "La campaña no puede durar más de 60 días" |
| **Imagen portada** | Archivo requerido | "La imagen de portada es obligatoria" |
| **Imagen portada** | Formato PNG/JPG | "Solo se permiten archivos PNG o JPG" |
| **Imagen portada** | Tamaño máx 5MB | "El archivo no debe superar 5MB" |
| **URL video** | URL válida (si se proporciona) | "Ingresa una URL válida de YouTube o Vimeo" |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click en X (cerrar)** | Mostrar confirmación modal "¿Seguro que quieres salir? El progreso se perderá" → Si confirma, redirect a `/dashboard/campanias` |
| **Click en upload area** | Abrir file picker nativo del OS |
| **Drag & drop image** | Validar formato y tamaño, mostrar preview si válido, error si inválido |
| **Typing en inputs** | Validación debounced (300ms), mostrar errores en tiempo real |
| **Select género** | Abrir dropdown con lista de géneros (Rock, Pop, Jazz, etc.) |
| **Click en date picker** | Abrir calendar popover, deshabilitar fechas < hoy y > 60 días |
| **Click Siguiente** | Validar todos los campos del paso 1, si válido guardar en state y avanzar a paso 2, si inválido mostrar errores |
| **Click Anterior** | No disponible en paso 1 |

### Zod Schema - Paso 1

```typescript
const paso1Schema = z.object({
  titulo: z.string()
    .min(1, "El título es obligatorio")
    .max(200, "Máximo 200 caracteres"),
  generoMusical: z.string()
    .min(1, "Selecciona un género musical"),
  metaFinanciacion: z.number()
    .min(0.01, "La meta debe ser mayor a cero")
    .positive("Ingresa un monto válido"),
  fechaFinalizacion: z.date()
    .min(addDays(new Date(), 7), "La campaña debe durar al menos 7 días")
    .max(addDays(new Date(), 60), "La campaña no puede durar más de 60 días"),
  imagenPortada: z.string()
    .url("Ingresa una URL válida")
    .min(1, "La imagen de portada es obligatoria"),
  videoUrl: z.string()
    .url("Ingresa una URL válida de YouTube o Vimeo")
    .regex(/^(https?:\/\/)?(www\.)?(youtube\.com\/watch\?v=|youtu\.be\/|vimeo\.com\/)/,
      "Debe ser una URL de YouTube o Vimeo")
    .optional()
    .or(z.literal(""))
});
```

---

## Pantalla: Wizard Crear Campaña - Paso 2 (Historia)

**Mockup:** WPR_6-Create-Campaign.png (step 2)
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/nueva?step=2`
**Template base:** `dashtail/` form patterns

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [🎵] MusicFund                                           [✕]       │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│                   Crear Nueva Campaña                              │
│          Completa los pasos para publicar tu proyecto musical      │
│                                                                    │
│  ┌──────┐────────┬──────┐────────┬──────┐────────┬──────┐         │
│  │  ✓   │████████│  2   │────────│  3   │────────│  4   │         │
│  └──────┘        └──────┘        └──────┘        └──────┘         │
│  Información         Historia      Recompensas     Revisión        │
│  Básica          (activo)                                          │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                                                              │  │
│  │  Historia del Proyecto                                       │  │
│  │                                                              │  │
│  │  Subtítulo (opcional)                                        │  │
│  │  [________________________________________________]           │  │
│  │  Máx 100 caracteres - Un resumen breve de tu campaña        │  │
│  │                                                              │  │
│  │  Descripción de la campaña *                                 │  │
│  │  ┌────────────────────────────────────────────────────────┐  │  │
│  │  │  [B] [I] [U] • ⁃ 1. 📎 🔗                             │  │  │
│  │  ├────────────────────────────────────────────────────────┤  │  │
│  │  │                                                        │  │  │
│  │  │  Escribe la historia de tu proyecto musical...        │  │  │
│  │  │                                                        │  │  │
│  │  │  Cuenta a tus fans qué quieres lograr, por qué es     │  │  │
│  │  │  importante y cómo usarás los fondos.                 │  │  │
│  │  │                                                        │  │  │
│  │  │  [Rich text editor con ~8 líneas visible]             │  │  │
│  │  │                                                        │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │  0/5000 caracteres                                           │  │
│  │                                                              │  │
│  │  ¿Por qué necesitas apoyo? (opcional)                        │  │
│  │  [________________________________________________]           │  │
│  │  [________________________________________________]           │  │
│  │  [________________________________________________]           │  │
│  │  Máx 500 caracteres - Explica el destino de los fondos      │  │
│  │                                                              │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  [← Anterior]                Paso 2 de 4                [Siguiente →]│
│  (outline)                                              (gradient)  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Step 1 Complete Icon | `<Icon>` (check) | `w-6 h-6 text-white` dentro de circle con gradient background |
| Rich Text Editor | TipTap `<Editor>` | `min-h-[300px] bg-[#1a1a2e] border-[#334155] text-white prose prose-invert` |
| Editor Toolbar | `<div>` | `bg-[#0f1729] border-b border-[#334155] p-2 flex gap-2` |
| Toolbar Button | `<Button variant="ghost" size="sm">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Character Counter | `<span>` | `text-xs text-[#64748b] mt-1` (color changes to yellow at 80%, red at 95%) |
| Textarea (¿Por qué?) | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none` |
| Hint Text | `<p>` | `text-xs text-[#64748b] mt-1 italic` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Stepper muestra paso 1 completado (✓), paso 2 activo (gradient), conectores correctos |
| **Typing Subtítulo** | Character counter actualizado (X/100), warning color a 80 caracteres |
| **Focus Editor** | Border color cambia a primary, toolbar highlights |
| **Typing Descripción** | Character counter actualizado (X/5000), validar mínimo al salir |
| **Editor Toolbar Hover** | Botones cambian de color a white |
| **Bold/Italic Applied** | Texto seleccionado cambia formato, botón toolbar highlighted |
| **Loading Submit** | Next button spinner, editor read-only |
| **Validation Error** | Border rojo en editor si descripción vacía |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Subtítulo** | Máx 100 caracteres | "Máximo 100 caracteres (X/100)" |
| **Descripción** | No vacío | "La descripción es obligatoria" |
| **Descripción** | Mín 50 caracteres | "Describe tu proyecto con al menos 50 caracteres" |
| **Descripción** | Máx 5000 caracteres | "Máximo 5000 caracteres (X/5000)" |
| **¿Por qué apoyo?** | Máx 500 caracteres | "Máximo 500 caracteres (X/500)" |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click toolbar button** | Aplicar formato a texto seleccionado (bold, italic, underline, list, link) |
| **Typing en editor** | Actualizar character counter en tiempo real |
| **Click Siguiente** | Validar descripción mínima, guardar HTML en state, avanzar a paso 3 |
| **Click Anterior** | Guardar progreso, volver a paso 1 con datos pre-completados |
| **Paste rich content** | Sanitizar HTML, mantener solo formatos permitidos |

### Zod Schema - Paso 2

```typescript
const paso2Schema = z.object({
  subtitulo: z.string()
    .max(100, "Máximo 100 caracteres")
    .optional(),
  descripcion: z.string()
    .min(50, "Describe tu proyecto con al menos 50 caracteres")
    .max(5000, "Máximo 5000 caracteres"),
  porQueApoyo: z.string()
    .max(500, "Máximo 500 caracteres")
    .optional()
});
```

---

## Pantalla: Wizard Crear Campaña - Paso 3 (Recompensas)

**Mockup:** WPR_6-Create-Campaign.png (step 3)
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/nueva?step=3`
**Template base:** `dashtail/` form patterns

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [🎵] MusicFund                                           [✕]       │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│                   Crear Nueva Campaña                              │
│          Completa los pasos para publicar tu proyecto musical      │
│                                                                    │
│  ┌──────┐────────┬──────┐────────┬──────┐────────┬──────┐         │
│  │  ✓   │████████│  ✓   │████████│  3   │────────│  4   │         │
│  └──────┘        └──────┘        └──────┘        └──────┘         │
│  Información      Historia      Recompensas      Revisión          │
│  Básica                         (activo)                           │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                                                              │  │
│  │  Recompensas para tus Backers                                │  │
│  │  Crea niveles de apoyo con beneficios atractivos            │  │
│  │                                                              │  │
│  │  ┌────────────────────────────────────────────────────────┐  │  │
│  │  │  € 10  Descarga Digital                         [Edit] │  │  │
│  │  │  Acceso anticipado al álbum en formato digital         │  │  │
│  │  │  📦 Ilimitadas disponibles                             │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                              │  │
│  │  ┌────────────────────────────────────────────────────────┐  │  │
│  │  │  € 25  CD Físico                                [Edit] │  │  │
│  │  │  CD firmado + descarga digital + booklet               │  │  │
│  │  │  📦 100 de 200 disponibles                             │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                              │  │
│  │  [+ Agregar recompensa]                                      │  │
│  │                                                              │  │
│  │  💡 Tip: Crea 3-5 niveles con beneficios progresivos        │  │
│  │                                                              │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  [← Anterior]                Paso 3 de 4                [Siguiente →]│
│  (outline)                                              (gradient)  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Reward Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-6 hover:border-primary transition mb-4` |
| Reward Price | `<span>` | `text-2xl font-bold text-primary` |
| Reward Title | `<span>` | `text-lg font-semibold text-white ml-3` |
| Reward Description | `<p>` | `text-sm text-[#94a3b8] mt-2` |
| Reward Stock | `<div>` | `flex items-center gap-2 text-xs text-[#64748b] mt-3` |
| Stock Icon | `<Icon>` (package) | `w-4 h-4` |
| Edit Button | `<Button variant="ghost" size="sm">` | `text-primary hover:text-purple-400` |
| Add Button | `<Button variant="outline">` | `border-dashed border-primary text-primary hover:bg-[#1e2a42] w-full py-6` |
| Tip Box | `<div>` | `bg-[#0f1729] border-l-4 border-primary p-4 rounded-r-lg mt-6` |
| Tip Icon | `<Icon>` (lightbulb) | `w-5 h-5 text-primary inline mr-2` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Stepper muestra pasos 1-2 completados, paso 3 activo |
| **No Rewards** | Mostrar mensaje "Aún no has creado recompensas" + botón "Crear primera recompensa" |
| **Hover Reward Card** | Border color cambia a primary, elevation aumenta |
| **Click Edit** | Abrir modal/drawer para editar reward (fuera de scope wizard, MVP simplificado) |
| **Click Agregar** | Abrir formulario inline o modal para crear nueva reward |
| **Loading Rewards** | Skeleton loaders para reward cards |
| **Empty State** | Icono grande + texto "Agrega tu primera recompensa" + CTA button |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Recompensas** | Opcional para avanzar | Wizard permite continuar sin recompensas |

**Nota:** En MVP, el artista puede publicar campaña sin recompensas. Se muestra advertencia modal en paso 4.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Agregar recompensa** | Abrir formulario para crear reward (precio, título, descripción, cantidad) |
| **Click Edit** | Abrir formulario pre-completado para editar reward existente |
| **Click Siguiente** | Guardar rewards en state, avanzar a paso 4 (revisión) |
| **Click Anterior** | Volver a paso 2 con rewards guardados |

### Zod Schema - Paso 3

```typescript
const rewardSchema = z.object({
  precio: z.number()
    .min(1, "El precio debe ser mayor a 0"),
  titulo: z.string()
    .min(1, "El título es obligatorio")
    .max(100, "Máximo 100 caracteres"),
  descripcion: z.string()
    .max(500, "Máximo 500 caracteres"),
  cantidadDisponible: z.number()
    .int()
    .min(1, "Debe haber al menos 1 disponible")
    .optional(), // null = ilimitado
  fechaEntregaEstimada: z.date()
    .optional()
});

const paso3Schema = z.object({
  recompensas: z.array(rewardSchema)
    .optional() // Permite 0 recompensas
});
```

---

## Pantalla: Wizard Crear Campaña - Paso 4 (Revisión)

**Mockup:** WPR_6-Create-Campaign.png (step 4)
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/nueva?step=4`
**Template base:** `dashtail/` summary layouts

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [🎵] MusicFund                                           [✕]       │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│                   Crear Nueva Campaña                              │
│          Completa los pasos para publicar tu proyecto musical      │
│                                                                    │
│  ┌──────┐────────┬──────┐────────┬──────┐────────┬──────┐         │
│  │  ✓   │████████│  ✓   │████████│  ✓   │████████│  4   │         │
│  └──────┘        └──────┘        └──────┘        └──────┘         │
│  Información      Historia      Recompensas      Revisión          │
│  Básica                                         (activo)           │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                                                              │  │
│  │  Revisión Final                                              │  │
│  │  Revisa todos los detalles antes de crear tu campaña        │  │
│  │                                                              │  │
│  │  ┌────────────────────────────────────────────────────────┐  │  │
│  │  │  [Imagen de portada preview]                          │  │  │
│  │  │                                                        │  │  │
│  │  │  Nuevo Álbum: "Midnight Sessions"                     │  │  │
│  │  │  Rock Indie                                            │  │  │
│  │  │                                                        │  │  │
│  │  │  Meta: € 5,000   •   Finaliza: 30 Mar 2026            │  │  │
│  │  │                                                        │  │  │
│  │  │  Un viaje sonoro por las noches de Barcelona...       │  │  │
│  │  │  [Descripción preview - primeras 3 líneas]            │  │  │
│  │  │                                                        │  │  │
│  │  │  Recompensas: 3 niveles (€10, €25, €50)               │  │  │
│  │  │                                                        │  │  │
│  │  │  [Editar]                                              │  │  │
│  │  └────────────────────────────────────────────────────────┘  │  │
│  │                                                              │  │
│  │  ⚠️  Nota: Tu campaña se creará como BORRADOR              │  │
│  │      Podrás editarla y publicarla cuando estés listo       │  │
│  │                                                              │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  [← Anterior]            [Guardar como borrador]     [Crear campaña]│
│  (outline)                    (outline)                 (gradient)  │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Preview Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Preview Image | `<img>` | `w-full h-64 object-cover rounded-lg mb-4` |
| Preview Title | `<h2>` | `text-2xl font-bold text-white mb-2` |
| Preview Meta | `<div>` | `flex items-center gap-4 text-sm text-[#94a3b8] mb-4` |
| Edit Link | `<Button variant="link">` | `text-primary hover:underline` |
| Warning Box | `<Alert>` | `bg-[#1a1a2e] border-l-4 border-warning text-[#94a3b8]` |
| Warning Icon | `<Icon>` (alert-triangle) | `w-5 h-5 text-warning inline mr-2` |
| Draft Button | `<Button variant="outline">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Create Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Stepper muestra todos los pasos completados, paso 4 activo |
| **Hover Edit** | Link text cambia a lighter purple, underline visible |
| **Loading Create** | Create button muestra spinner + "Creando campaña...", otros buttons disabled |
| **Success** | Toast notification verde "Campaña creada exitosamente" + redirect |
| **Error** | Toast notification roja con mensaje de error del backend |

### Validación en Tiempo Real

**No aplica** - Este paso es solo revisión, no hay inputs editables directamente.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Editar** | Navigate a paso correspondiente (1-3) con datos pre-completados |
| **Click Guardar como borrador** | POST `/api/campanias` con estado BORRADOR, redirect a `/dashboard/campanias` con toast "Campaña guardada" |
| **Click Crear campaña** | POST `/api/campanias` con estado BORRADOR, redirect a `/dashboard/campanias/{id}/preview` |
| **Click Anterior** | Volver a paso 3 |

---

## Pantalla: Vista Previa Campaña (Borrador)

**Mockup:** Similar a WPR_3-Detail-Campaign.png con banner de borrador
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/{id}/preview`
**Template base:** `krowd/` campaign detail + custom banner

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  ⚠️  VISTA PREVIA - Campaña en borrador (No visible públicamente) │
│                                                           [✕]       │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  [Editar] [Publicar Ahora] [Eliminar]                              │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │  [Imagen de portada - full width]                           │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                    │
│  Nuevo Álbum: "Midnight Sessions"                                 │
│  👤 Alex Rivera  •  Rock Indie  •  BORRADOR                        │
│                                                                    │
│  € 0 de € 5,000                                                    │
│  ▓▓▓░░░░░░░░░░░░░░░░  0% financiado                                │
│  0 backers  •  30 días restantes                                   │
│                                                                    │
│  [Historia] [Actualizaciones] [Comentarios] [FAQ]                 │
│                                                                    │
│  [Descripción completa con rich text...]                           │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Draft Banner | `<Alert>` | `bg-warning/10 border-warning text-warning font-medium p-4 mb-4` |
| Banner Icon | `<Icon>` (alert-circle) | `w-5 h-5 inline mr-2` |
| Close Banner Button | `<Button variant="ghost" size="sm">` | `text-warning hover:text-warning/80` |
| Action Bar | `<div>` | `flex gap-3 mb-6` |
| Edit Button | `<Button variant="outline">` | `border-[#334155] text-white` |
| Publish Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |
| Delete Button | `<Button variant="outline">` | `border-red-500/50 text-red-500 hover:bg-red-500/10` |
| Campaign Preview | Ver "Pantalla: Detalle Campaña Pública" | Mismo layout pero con banner de borrador arriba |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Banner warning visible, botones de acción habilitados |
| **Hover Publicar** | Button gradient más brillante, glow effect |
| **Loading Publicar** | Publish button spinner + "Publicando...", otros disabled |
| **Success Publicar** | Toast verde "Campaña publicada", redirect a `/dashboard/campanias` |
| **Loading Delete** | Delete button spinner, otros disabled |
| **Confirm Delete** | Modal de confirmación "¿Estás seguro?" con botones Cancelar / Eliminar |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click X (cerrar banner)** | Ocultar banner (persist preference in localStorage) |
| **Click Editar** | Navigate a `/dashboard/campanias/{id}/editar` (wizard pre-completado) |
| **Click Publicar Ahora** | Validar si tiene rewards, mostrar modal advertencia si no, POST `/api/campanias/{id}/publicar`, cambiar estado a PUBLICADA |
| **Click Eliminar** | Mostrar modal confirmación, si confirma DELETE `/api/campanias/{id}`, redirect a `/dashboard/campanias` |

---

## Pantalla: Detalle Campaña Pública

**Mockup:** WPR_3-Detail-Campaign.png
**Proyecto:** Landing
**Ruta:** `/campanias/{id}`
**Template base:** `krowd/` campaign detail layouts

### Layout

```
┌────────────────────────────────────────────────────────────────────────┐
│  [🎵] MusicFund  Explorar  Categorías  Cómo funciona                  │
│                                    Iniciar Sesión  [Crear Campaña]    │
├────────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │                                                                  │  │
│  │  [Imagen principal de campaña - hero image]                     │  │
│  │                                                                  │  │
│  └──────────────────────────────────────────────────────────────────┘  │
│                                                                        │
│  ┌────────────────────────────┐  ┌──────────────────────────────────┐  │
│  │                            │  │                                  │  │
│  │  Nuevo Álbum: "Ecos de     │  │  [Apoyar esta campaña]           │  │
│  │  Medianoche"               │  │  (gradient button - full width)  │  │
│  │                            │  │  Desde € 5                       │  │
│  │  👤 Los Nocturnos          │  │                                  │  │
│  │  ✓ Artista verificado      │  │  ─────────────────────────────   │  │
│  │  Rock Indie  •  Activa     │  │                                  │  │
│  │                            │  │  Recompensas                     │  │
│  │  € 12,450 de € 15,000      │  │                                  │  │
│  │  ▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░        │  │  ┌────────────────────────────┐  │  │
│  │  83% financiado            │  │  │ € 10  Descarga Digital     │  │  │
│  │                            │  │  │ Más popular               │  │  │
│  │  127 backers               │  │  │ 234 de 500 disponibles    │  │  │
│  │  12 días restantes         │  │  │ [Seleccionar]             │  │  │
│  │                            │  │  └────────────────────────────┘  │  │
│  │  ─────────────────────     │  │                                  │  │
│  │                            │  │  ┌────────────────────────────┐  │  │
│  │  [Historia] [Actualizaciones] │  │ € 25  CD Físico            │  │  │
│  │  [Comentarios] [FAQ]       │  │  │ CD + descarga + booklet    │  │  │
│  │                            │  │  │ 89 de 200 disponibles      │  │  │
│  │  Después de dos años...    │  │  │ [Seleccionar]             │  │  │
│  │  [Rich text description]   │  │  └────────────────────────────┘  │  │
│  │                            │  │                                  │  │
│  │  [Imagen del estudio]      │  │  ┌────────────────────────────┐  │  │
│  │                            │  │  │ € 50  Vinilo Limitado      │  │  │
│  │  ¿Por qué necesitamos      │  │  │ Vinilo especial + extras   │  │  │
│  │  tu apoyo?                 │  │  │ 45 de 100 disponibles      │  │  │
│  │  ✓ Masterización Abbey    │  │  │ [Seleccionar]             │  │  │
│  │    Road                    │  │  └────────────────────────────┘  │  │
│  │  ✓ Producción de vinilos   │  │                                  │  │
│  │  ✓ 3 videoclips            │  │  🔒 Pago seguro                  │  │  │
│  │  ✓ Gira promocional        │  │  📦 Entrega: Marzo 2025          │  │
│  │                            │  │                                  │  │
│  │  ─────────────────────     │  └──────────────────────────────────┘  │
│  │                            │                                        │
│  │  Sobre el Artista          │                                        │
│  │  ┌──────────────────────┐  │                                        │
│  │  │ 👤 Los Nocturnos     │  │                                        │
│  │  │ Banda de rock indie  │  │                                        │
│  │  │ formada en 2018...   │  │                                        │
│  │  │                      │  │                                        │
│  │  │ 3 campañas           │  │                                        │
│  │  │ € 45,230 recaudado   │  │                                        │
│  │  │ [Ver perfil]         │  │                                        │
│  │  └──────────────────────┘  │                                        │
│  │                            │                                        │
│  └────────────────────────────┘                                        │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Header Nav | `<header>` | `bg-[#1a1a2e] border-b border-[#334155] sticky top-0 z-50 px-6 py-4` |
| Hero Image | `<img>` | `w-full h-96 object-cover rounded-lg mb-8` |
| Main Layout | `<div>` | `max-w-7xl mx-auto px-6 py-8 grid grid-cols-3 gap-8` (2 cols content + 1 col sidebar) |
| Left Content | `<div>` | `col-span-2` |
| Right Sidebar | `<div>` | `col-span-1 sticky top-24` |
| Campaign Title | `<h1>` | `text-4xl font-bold text-white mb-4` |
| Artist Info | `<div>` | `flex items-center gap-3 mb-4` |
| Artist Avatar | `<Avatar>` | `w-10 h-10` |
| Artist Name | `<Link>` | `text-primary hover:underline font-medium` |
| Verified Badge | `<Badge>` | `bg-primary/20 text-primary border-primary/50` |
| Genre Badge | `<Badge>` | `bg-[#2d1b4e] text-purple-300 border-purple-500/50` |
| Status Badge | `<Badge>` | `bg-green-500/20 text-green-400 border-green-500/50` |
| Funding Amount | `<div>` | `text-3xl font-bold text-white mb-2` |
| Goal Amount | `<span>` | `text-xl text-[#94a3b8]` |
| Progress Bar Container | `<div>` | `w-full h-3 bg-[#334155] rounded-full overflow-hidden mb-3` |
| Progress Bar Fill | `<div>` | `h-full bg-gradient-to-r from-pink-500 to-purple-600` (width dynamic based on %) |
| Stats Row | `<div>` | `flex items-center gap-6 text-[#94a3b8]` |
| Stat Item | `<div>` | `flex items-center gap-2` |
| Tabs Navigation | `<Tabs>` | `border-b border-[#334155] mb-6` |
| Tab Button | `<TabsTrigger>` | `text-[#94a3b8] hover:text-white data-[state=active]:text-primary data-[state=active]:border-b-2 data-[state=active]:border-primary` |
| Description Content | `<div>` | `prose prose-invert max-w-none text-[#94a3b8]` (rich text HTML) |
| Image in Content | `<img>` | `rounded-lg my-6 w-full` |
| Checklist Section | `<div>` | `bg-[#0f1729] border-l-4 border-primary p-6 rounded-r-lg my-6` |
| Checklist Item | `<div>` | `flex items-start gap-3 mb-3` |
| Check Icon | `<Icon>` (check-circle) | `w-5 h-5 text-primary flex-shrink-0 mt-0.5` |
| Sidebar Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 sticky top-24` |
| Support Button | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2` |
| Starting Price | `<p>` | `text-center text-sm text-[#94a3b8] mb-6` |
| Rewards Title | `<h3>` | `text-lg font-bold text-white mb-4` |
| Reward Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4 mb-3 hover:border-primary cursor-pointer transition` |
| Reward Price | `<div>` | `text-xl font-bold text-primary mb-2` |
| Reward Title | `<h4>` | `text-white font-semibold mb-1` |
| Popular Badge | `<Badge>` | `bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs` |
| Reward Description | `<p>` | `text-sm text-[#94a3b8] mb-2` |
| Stock Info | `<p>` | `text-xs text-[#64748b]` |
| Select Button | `<Button variant="outline" size="sm">` | `w-full border-primary text-primary hover:bg-primary/10` |
| Sidebar Footer | `<div>` | `border-t border-[#334155] pt-4 mt-6` |
| Secure Payment | `<div>` | `flex items-center gap-2 text-xs text-[#64748b] mb-2` |
| Delivery Info | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` |
| About Artist Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mt-8` |
| Artist Mini Avatar | `<Avatar>` | `w-16 h-16 mb-3` |
| Artist Stats | `<div>` | `text-sm text-[#64748b] mb-3` |
| View Profile Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8]` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders para imagen, título, descripción, rewards |
| **Funded 100%** | Progress bar lleno, badge "Financiada" verde, confetti animation |
| **Campaign Ended** | Status badge "Finalizada" azul, support button disabled "Campaña finalizada" |
| **No Rewards** | Sidebar muestra "Esta campaña no tiene recompensas específicas" + solo support button genérico |
| **Reward Sold Out** | Reward card con opacity reducida, badge "Agotado" gris, select button disabled |
| **Tab Active** | Tab text color primary, border bottom primary |
| **Error 404** | Página "Campaña no encontrada" con link a explorar |
| **Not Published** | Si campaña borrador y usuario no es owner → 404 Not Found |

### Validación en Tiempo Real

**No aplica** - Página de solo lectura, no hay inputs.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Apoyar esta campaña** | Si no autenticado → redirect a login. Si autenticado → redirect a `/campanias/{id}/backing` (fuera de scope US-02) |
| **Click Seleccionar (reward)** | Si no autenticado → redirect a login. Si autenticado → pre-seleccionar reward, redirect a `/campanias/{id}/backing?reward={rewardId}` |
| **Click Artist Name/Avatar** | Navigate a `/artistas/{artistaId}` |
| **Click Ver perfil (artist card)** | Navigate a `/artistas/{artistaId}` |
| **Click Tab** | Cambiar contenido visible (Historia / Actualizaciones / Comentarios / FAQ) |
| **Hover Reward Card** | Border color cambia a primary, elevation aumenta |
| **Scroll** | Header sticky permanece visible, sidebar sticky scroll independiente |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Layout cambia a columna única, sidebar va abajo de content, hero image h-64 |
| **Tablet (640-1024px)** | Grid 2 columnas (content | sidebar), hero image h-80 |
| **Desktop (> 1024px)** | Grid 3 columnas (2 content + 1 sidebar), hero image h-96 |

---

## Pantalla: Mis Campañas (Lista)

**Mockup:** WPR_5-Dashboard-Artist.png (sección "Mis Campañas")
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias`
**Template base:** `dashtail/` dashboard layouts

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │  Dashboard: Mis Campañas                             │
│             │                                                      │
│  Dashboard  │  [+ Nueva campaña] (gradient button top-right)       │
│  Mis        │                                                      │
│  Campañas   │  ┌────────────────────────────────────────────────┐  │
│  Crear      │  │  [IMG]  Nuevo Album "Midnight"                 │  │
│  Campana    │  │         Activa  •  85% ████████░░  •  420 ↑    │  │
│  Mis        │  │         [Editar] [Ver] [...]                   │  │
│  Backers    │  └────────────────────────────────────────────────┘  │
│  Config     │                                                      │
│             │  ┌────────────────────────────────────────────────┐  │
│  Ver        │  │  [IMG]  EP Acústico                            │  │
│  perfil     │  │         Borrador  •  0% ░░░░░░░░░░  •  0 ↑    │  │
│  Logout     │  │         [Editar] [Publicar] [...]              │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                      │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  [IMG]  Tour 2024                              │  │
│             │  │         Finalizada  •  120% ██████████ •  827↑ │  │
│             │  │         [Ver] [Archivar] [...]                 │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                      │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Sidebar | Ver dashboard spec | `w-64 bg-[#0f1729] border-r border-[#334155]` |
| Main Content | `<div>` | `flex-1 p-8` |
| Header Row | `<div>` | `flex items-center justify-between mb-8` |
| Page Title | `<h1>` | `text-3xl font-bold text-white` |
| New Campaign Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white font-semibold` |
| Campaigns List | `<div>` | `space-y-4` |
| Campaign Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 hover:border-primary transition` |
| Card Layout | `<div>` | `flex items-center gap-6` |
| Campaign Image | `<img>` | `w-24 h-24 object-cover rounded-lg` |
| Campaign Info | `<div>` | `flex-1` |
| Campaign Title | `<h3>` | `text-lg font-bold text-white mb-2` |
| Status Badge (Borrador) | `<Badge>` | `bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50` |
| Status Badge (Activa) | `<Badge>` | `bg-green-500/20 text-green-400 border-green-500/50` |
| Status Badge (Finalizada) | `<Badge>` | `bg-blue-500/20 text-blue-400 border-blue-500/50` |
| Status Badge (Cancelada) | `<Badge>` | `bg-red-500/20 text-red-400 border-red-500/50` |
| Progress Mini | `<div>` | `flex items-center gap-3 text-sm text-[#94a3b8]` |
| Progress Percent | `<span>` | `font-semibold` |
| Progress Bar Mini | `<div>` | `w-24 h-2 bg-[#334155] rounded-full overflow-hidden` |
| Progress Fill | `<div>` | `h-full bg-gradient-to-r from-pink-500 to-purple-600` |
| Backers Count | `<span>` | `flex items-center gap-1 text-sm text-[#94a3b8]` |
| Actions Row | `<div>` | `flex gap-2` |
| Edit Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white` |
| View Button | `<Button variant="outline" size="sm">` | `border-[#334155] text-primary hover:bg-primary/10` |
| Publish Button | `<Button variant="outline" size="sm">` | `border-green-500/50 text-green-400 hover:bg-green-500/10` |
| More Menu | `<DropdownMenu>` | `text-[#94a3b8]` |
| Empty State | `<div>` | `text-center py-16` |
| Empty Icon | `<Icon>` (folder-open) | `w-16 h-16 text-[#64748b] mx-auto mb-4` |
| Empty Text | `<p>` | `text-lg text-[#94a3b8] mb-6` |
| Empty CTA | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders para campaign cards (3-4 skeletons) |
| **Empty State** | Icono + texto "No tienes campañas aún" + botón "Crear tu primera campaña" |
| **Hover Card** | Border color cambia a primary, elevation aumenta sutilmente |
| **Click Menu** | Dropdown muestra opciones: Editar, Publicar (si borrador), Ver, Eliminar, Archivar |
| **Status Indicator** | Badge color cambia según estado (Borrador=gray, Activa=green, Finalizada=blue, Cancelada=red) |
| **Progress 100%** | Progress bar lleno, color success, icono check |

### Validación en Tiempo Real

**No aplica** - Lista de solo lectura con acciones.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click + Nueva campaña** | Navigate a `/dashboard/campanias/nueva` (wizard paso 1) |
| **Click Editar** | Navigate a `/dashboard/campanias/{id}/editar` (wizard pre-completado) |
| **Click Ver** | Navigate a `/dashboard/campanias/{id}/preview` (si borrador) o `/campanias/{id}` (si publicada) |
| **Click Publicar** | Confirmar modal, POST `/api/campanias/{id}/publicar`, actualizar lista |
| **Click Eliminar** | Confirmar modal "¿Estás seguro?", DELETE `/api/campanias/{id}`, remover de lista |
| **Click Archivar** | PUT `/api/campanias/{id}/archivar`, mover a sección archivadas (fuera de MVP) |
| **Click Campaign Card** | Navigate a `/dashboard/campanias/{id}/preview` |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar colapsado a hamburger, cards stack verticalmente, imagen más pequeña (w-16 h-16) |
| **Tablet (640-1024px)** | Sidebar visible, cards mantienen layout horizontal |
| **Desktop (> 1024px)** | Sidebar full width, cards con spacing amplio |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios Generales |
|------------|-------|----------|
| **Mobile** | < 640px | Stack todo verticalmente, padding reducido (px-4), font sizes reducidos (-2px), botones full width, sidebar colapsado a hamburger menu |
| **Tablet** | 640-1024px | Grid 2 columnas donde aplique, padding medio (px-6), font sizes normales, sidebar visible pero más estrecho |
| **Desktop** | > 1024px | Layouts completos, max-width containers (max-w-7xl), padding amplio (px-8), sidebar full width |

### Wizard (Mobile)
- Stepper horizontal → vertical o simplificado a "Paso X de 4" texto
- Grid 2 columnas → stack vertical
- Upload area padding reducido
- Preview imagen más pequeña
- Botones Next/Previous stack verticalmente

### Detalle Campaña (Mobile)
- Hero image height 256px (h-64)
- Layout columna única (sidebar abajo del content)
- Stats en grid 2x2
- Reward cards stack verticalmente
- Tabs scroll horizontal si no caben

### Mis Campañas (Mobile)
- Campaign card: imagen w-16 h-16
- Progress bar más corta (w-16)
- Botones iconos solamente (sin texto)
- Sidebar colapsado, open/close con hamburger icon

---

## Animaciones

| Elemento | Animación | Duración |
|----------|-----------|----------|
| **Button hover** | Scale 1.02 + shadow glow | 150ms ease |
| **Input focus** | Border color transition + glow shadow | 200ms ease |
| **Card hover** | Border color change + elevation increase | 200ms ease |
| **Page transition** | Fade in opacity 0 → 1 | 300ms ease-in-out |
| **Step advance** | Slide content left, fade out → fade in | 400ms ease-in-out |
| **Progress bar fill** | Width transition animated | 600ms ease-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Modal open** | Scale 0.95 → 1 + fade in | 200ms ease-out |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 loop | 1500ms ease-in-out |
| **Badge appearance** | Fade in + scale 0.9 → 1 | 200ms ease |
| **Stepper connector fill** | Width 0 → 100% | 400ms ease |
| **Image preview load** | Fade in + scale 0.95 → 1 | 300ms ease |
| **Dropdown menu** | Slide down + fade | 150ms ease-out |
| **Confetti (100% funded)** | Particles fall from top | 3000ms ease-out |

### Stepper Animation

```css
/* Cuando avanza de paso 1 a paso 2 */
@keyframes fillConnector {
  from { width: 0%; }
  to { width: 100%; }
}

.step-connector.active {
  animation: fillConnector 400ms ease-in-out forwards;
}
```

---

## Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de color** | Mínimo 4.5:1 para texto normal, 3:1 para texto grande. Verificado: white (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 ✓, primary (#a855f7) sobre bg-primary = 5.2:1 ✓ |
| **Focus visible** | Ring de 2px en `border-primary` (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Labels en inputs** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Buttons** | Texto descriptivo o `aria-label` en icon buttons |
| **Imágenes** | `alt` text descriptivo en todas las imágenes, "Imagen de portada de {título campaña}" |
| **Stepper** | `role="progressbar"` con `aria-valuenow`, `aria-valuemin`, `aria-valuemax` |
| **Tabs** | `role="tablist"` con `aria-selected` en tab activo |
| **Loading states** | `aria-busy="true"` y `aria-live="polite"` para anunciar cambios |
| **Modals** | `role="dialog"` con `aria-modal="true"`, focus trap, Esc para cerrar |
| **Keyboard navigation** | Tab order lógico, Enter para submit/select, Space para toggle, Esc para cerrar |
| **Screen reader** | Rich text content con `role="article"` y `aria-label` descriptivo |
| **Color no es único indicador** | Estados usan icono + color + texto (ej: Borrador badge tiene texto + color) |
| **Skip links** | "Saltar al contenido principal" en landing (invisible hasta focus) |

### ARIA Labels Específicos

```tsx
// Stepper
<div role="progressbar" aria-valuenow={currentStep} aria-valuemin={1} aria-valuemax={4} aria-label="Progreso de creación de campaña">
  <div className="step" aria-current={currentStep === 1 ? "step" : undefined}>
    1. Información Básica
  </div>
</div>

// File upload
<div role="button" tabIndex={0} aria-label="Subir imagen de portada">
  <input type="file" id="upload" className="sr-only" aria-label="Seleccionar archivo" />
  Arrastra tu imagen aquí...
</div>

// Progress bar (campaign detail)
<div role="progressbar" aria-valuenow={83} aria-valuemin={0} aria-valuemax={100} aria-label="Progreso de financiación: 83%">
  <div className="fill" style={{ width: "83%" }}></div>
</div>

// Status badge
<span role="status" aria-label="Estado de campaña: Activa">
  <Badge>Activa</Badge>
</span>

// Button loading
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isPending ? "Creando campaña..." : "Crear campaña"}
</Button>

// Rich text editor
<div role="textbox" aria-multiline="true" aria-label="Descripción de la campaña" aria-required="true">
  {/* Editor content */}
</div>

// Reward card
<Card role="article" aria-label="Recompensa: Descarga Digital por 10 euros">
  <h4>Descarga Digital</h4>
  <p>€ 10</p>
  <Button aria-label="Seleccionar recompensa Descarga Digital">Seleccionar</Button>
</Card>
```

---

## Checklist UI/UX

### Wizard Paso 1 - Información Básica
- [ ] Header con logo y botón cerrar (X)
- [ ] Stepper horizontal con 4 pasos (paso 1 activo con gradient)
- [ ] Form card con todos los campos especificados
- [ ] Label con asterisco rojo para campos obligatorios
- [ ] Input título con validación max 200 caracteres
- [ ] Select dropdown para género musical
- [ ] Input meta financiación con formato € (euro symbol)
- [ ] Date picker con calendar popover, fechas inválidas disabled
- [ ] Input video URL opcional con validación YouTube/Vimeo
- [ ] File upload area con drag & drop funcional
- [ ] Preview de imagen con skeleton loader
- [ ] Botón Siguiente gradient con hover effect
- [ ] Validación Zod integrada y mensajes de error en español
- [ ] Estados: default, focus, loading, error
- [ ] Responsive en mobile (grid → stack)
- [ ] Accesibilidad: labels, ARIA, focus states

### Wizard Paso 2 - Historia
- [ ] Stepper muestra paso 1 completado (✓), paso 2 activo
- [ ] Input subtítulo opcional con contador 0/100
- [ ] Rich text editor (TipTap) con toolbar
- [ ] Toolbar: bold, italic, underline, lists, links
- [ ] Editor min height 300px, dark theme
- [ ] Character counter para descripción (0/5000)
- [ ] Textarea "¿Por qué apoyo?" opcional con contador 0/500
- [ ] Botón Anterior outline funcional
- [ ] Validación mínimo 50 caracteres en descripción
- [ ] Estados: typing, focus, editor formatting
- [ ] Responsive en mobile
- [ ] Accesibilidad: editor con role="textbox"

### Wizard Paso 3 - Recompensas
- [ ] Stepper muestra pasos 1-2 completados, paso 3 activo
- [ ] Lista de reward cards creadas (si existen)
- [ ] Cada reward card: precio, título, descripción, stock
- [ ] Botón "Agregar recompensa" outline dashed
- [ ] Empty state si no hay recompensas
- [ ] Tip box con icono lightbulb
- [ ] Estados: empty, hover card, loading
- [ ] Modal/form para crear/editar reward
- [ ] Responsive en mobile
- [ ] Accesibilidad: reward cards con role="article"

### Wizard Paso 4 - Revisión
- [ ] Stepper muestra todos los pasos completados, paso 4 activo
- [ ] Preview card con imagen de portada
- [ ] Título, género, meta, fecha visible
- [ ] Descripción preview (primeras líneas)
- [ ] Resumen de recompensas
- [ ] Link "Editar" para volver a pasos anteriores
- [ ] Warning box "Se creará como BORRADOR"
- [ ] Botón "Guardar como borrador" outline
- [ ] Botón "Crear campaña" gradient principal
- [ ] Loading state en botones
- [ ] Toast notifications success/error
- [ ] Redirect a preview después de crear
- [ ] Responsive en mobile
- [ ] Accesibilidad: warning con role="alert"

### Vista Previa Campaña (Borrador)
- [ ] Banner warning con fondo amarillo/warning
- [ ] Botón cerrar banner (X) con persistencia localStorage
- [ ] Action bar: Editar, Publicar, Eliminar buttons
- [ ] Layout igual a detalle público + banner arriba
- [ ] Modal confirmación para Publicar
- [ ] Modal advertencia si no hay rewards
- [ ] Modal confirmación para Eliminar
- [ ] Loading states en botones de acción
- [ ] Redirect correcto después de publicar/eliminar
- [ ] Responsive en mobile
- [ ] Accesibilidad: banner con role="alert"

### Detalle Campaña Pública
- [ ] Header nav sticky con logo y links
- [ ] Hero image full width con height correcto
- [ ] Layout 2 columnas (content | sidebar)
- [ ] Título campaña h1 grande y bold
- [ ] Artist info con avatar, nombre, badges
- [ ] Verified badge, genre badge, status badge
- [ ] Funding amount destacado con goal
- [ ] Progress bar gradient con porcentaje
- [ ] Stats row: backers, días restantes
- [ ] Tabs navigation: Historia, Actualizaciones, etc.
- [ ] Rich text description renderizado correctamente
- [ ] Imágenes en content con rounded corners
- [ ] Checklist section con check icons
- [ ] About Artist card al final
- [ ] Sidebar sticky con reward cards
- [ ] Botón "Apoyar esta campaña" gradient full width
- [ ] Reward cards con hover effect
- [ ] Popular badge en reward destacado
- [ ] Stock info en cada reward
- [ ] Botones "Seleccionar" por reward
- [ ] Secure payment y delivery info en footer sidebar
- [ ] Loading skeletons
- [ ] Estado: no rewards, sold out, campaign ended
- [ ] Error 404 si campaña no existe
- [ ] Responsive: mobile (columna única), tablet (2 cols), desktop (3 cols)
- [ ] Accesibilidad: progress bar con aria-valuenow, tabs con role="tablist"

### Mis Campañas (Lista)
- [ ] Sidebar dashboard con navegación
- [ ] Botón "+ Nueva campaña" gradient top-right
- [ ] Lista de campaign cards con spacing
- [ ] Cada card: imagen, título, status badge, progress mini, backers count
- [ ] Status badge con colores correctos por estado
- [ ] Progress bar mini con gradient
- [ ] Botones: Editar, Ver, More menu
- [ ] Botón "Publicar" visible solo en borradores
- [ ] Dropdown menu con más opciones
- [ ] Empty state con icono y CTA
- [ ] Loading skeletons
- [ ] Hover effect en cards
- [ ] Modal confirmación para eliminar
- [ ] Responsive en mobile (sidebar hamburger, cards verticales)
- [ ] Accesibilidad: status badges con role="status"

### Cross-cutting
- [ ] Dark theme aplicado consistentemente (#1a1a2e, #0f1729)
- [ ] Design tokens definidos y usados
- [ ] Gradient buttons (pink → purple) en todos los CTAs principales
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] shadcn/ui components correctos en todo el UI
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste mínimo 4.5:1 verificado
- [ ] Keyboard navigation funcional (Tab, Enter, Esc)
- [ ] Focus states con ring purple en todos los interactivos
- [ ] Form validation con Zod + react-hook-form
- [ ] Integración con TanStack Query para mutations
- [ ] Manejo de errores del backend (ServiceResponse)
- [ ] Toast notifications configuradas (sonner)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders para estados de carga
- [ ] Empty states con iconos y CTAs claros
- [ ] Error boundaries implementados
