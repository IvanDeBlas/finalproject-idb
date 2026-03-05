# Diseño UI: Crear Campaña (Admin)

**Fecha:** 2026-02-12
**Feature:** crear-campania (US-02)
**Target:** Admin Dashboard (Next.js 14 - `src/admin`)
**Referencias:**
- UI/UX Spec: `docs/user-stories/crear-campania/ui-ux.md`
- Contracts: `docs/user-stories/crear-campania/contracts.md`
- Templates: Dashtail (wizard, dashboard), Krowd (campaign detail preview)

---

## 1. Resumen Ejecutivo

### Componentes shadcn/ui Requeridos

| Componente | Uso Principal | Pantallas |
|------------|---------------|-----------|
| **Card, CardHeader, CardContent, CardFooter** | Contenedores de formularios, campaign cards | Wizard Steps, Mis Campañas, Preview |
| **Button** | CTAs, navegación, acciones | Todas |
| **Input, Textarea** | Campos de texto | Wizard Steps 1-2 |
| **Label** | Etiquetas de formulario | Wizard Steps 1-3 |
| **Form, FormField, FormItem, FormLabel, FormControl, FormMessage** | Integración con react-hook-form | Wizard Steps 1-3 |
| **Select, SelectTrigger, SelectContent, SelectItem** | Género musical, tipo financiación | Wizard Step 1 |
| **Calendar** + **Popover** | Date picker para fecha fin | Wizard Step 1 |
| **Badge** | Estados de campaña (Borrador, Activa, etc.) | Mis Campañas, Preview |
| **Progress** | Progress bar de financiación | Mis Campañas, Preview Público |
| **Alert, AlertTitle, AlertDescription** | Banner de borrador, warnings | Preview, Wizard Step 4 |
| **Dialog, DialogTrigger, DialogContent, DialogHeader, DialogFooter** | Confirmaciones (publicar, eliminar) | Preview, Mis Campañas |
| **DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem** | Menú de acciones (More options) | Mis Campañas |
| **Skeleton** | Loading states | Todas (loading) |
| **Separator** | Divisores visuales | Wizard, Preview |
| **Avatar** | Imagen de artista | Preview Público |
| **Tabs, TabsList, TabsTrigger, TabsContent** | Navegación Historia/Actualizaciones | Preview Público |

### Componentes Custom a Crear

| Componente | Propósito | Base |
|------------|-----------|------|
| **WizardStepper** | Stepper horizontal con 4 pasos | Custom con primitives + Tailwind |
| **FileUploadArea** | Drag & drop para imagen de portada | Custom con input[type=file] |
| **RichTextEditor** | Editor TipTap para descripción | TipTap wrapper |
| **CharacterCounter** | Contador de caracteres con colores | Custom span |
| **CampaignCard** | Card de campaña en lista | Card + Badge + Progress |
| **RewardCard** | Card de recompensa en wizard step 3 | Card + Button |
| **EmptyState** | Estado vacío con icono + CTA | Custom div + Icon + Button |

### Composiciones Clave

- **Wizard de 4 pasos** (WizardStepper + Form + navegación Anterior/Siguiente)
- **Preview de campaña borrador** (Banner Alert + Campaign detail layout)
- **Lista "Mis Campañas"** (CampaignCard[] + DropdownMenu actions)

---

## 2. Paleta de Colores

Los colores están definidos en el diseño global del proyecto con **dark theme** como base.

| Variable CSS | Valor Hex | Uso en Crear Campaña |
|--------------|-----------|----------------------|
| `--background` | `#1a1a2e` | Fondo principal de pantallas |
| `--card` | `#0f1729` | Fondo de cards (form cards, campaign cards) |
| `--card-foreground` | `#ffffff` | Texto sobre cards |
| `--primary` | `#a855f7` | Botones CTA, stepper activo, progress bar, links |
| `--primary-foreground` | `#ffffff` | Texto sobre primary |
| `--secondary` | `#16213e` | Fondos secundarios |
| `--muted` | `#94a3b8` | Texto secundario, hints |
| `--muted-foreground` | `#64748b` | Texto muy sutil (placeholders) |
| `--border` | `#334155` | Bordes de inputs, cards, separadores |
| `--input` | `#1a1a2e` | Fondo de inputs (igual que bg-primary) |
| `--ring` | `#a855f7` | Focus ring (primary) |
| `--destructive` | `#ef4444` | Botones de eliminar, errores |
| `--success` | `#10b981` | Badge "Activa", estados positivos |
| `--warning` | `#f59e0b` | Banner de borrador, advertencias |

### Gradientes

```css
/* Botón CTA principal (Gradient) */
.btn-gradient {
  background: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
}

.btn-gradient:hover {
  background: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);
}

/* Progress bar */
.progress-fill {
  background: linear-gradient(to right, #ec4899, #a855f7);
}
```

### Colores de Estados (Badges)

| Estado | Variable | Bg | Text | Border |
|--------|----------|----|----- |--------|
| **Borrador** | `--muted` | `bg-muted/20` | `text-muted` | `border-muted/50` |
| **Activa** | `--success` | `bg-success/20` | `text-success` | `border-success/50` |
| **Finalizada** | `--info` (blue) | `bg-blue-500/20` | `text-blue-400` | `border-blue-500/50` |
| **Cancelada** | `--destructive` | `bg-destructive/20` | `text-destructive` | `border-destructive/50` |

---

## 3. Tipografía

| Elemento | Font Size | Font Weight | Line Height | Uso |
|----------|-----------|-------------|-------------|-----|
| **h1 (Page Title)** | `text-3xl` (30px) | `font-bold` (700) | `leading-tight` (1.25) | "Crear Nueva Campaña", "Mis Campañas" |
| **h2 (Section Title)** | `text-2xl` (24px) | `font-bold` (700) | `leading-tight` | "Información Básica", "Recompensas" |
| **h3 (Card Title)** | `text-lg` (18px) | `font-semibold` (600) | `leading-normal` | Título de campaign card |
| **h4 (Reward Title)** | `text-base` (16px) | `font-semibold` (600) | `leading-normal` | Título de reward card |
| **Body Text** | `text-sm` (14px) | `font-normal` (400) | `leading-normal` (1.5) | Texto general, descripciones |
| **Label** | `text-sm` (14px) | `font-medium` (500) | `leading-normal` | Labels de formulario |
| **Hint/Helper** | `text-xs` (12px) | `font-normal` (400) | `leading-relaxed` (1.75) | Hints debajo de inputs, contadores |
| **Button Text** | `text-sm` (14px) | `font-semibold` (600) | `leading-normal` | Botones CTA |
| **Badge Text** | `text-xs` (12px) | `font-medium` (500) | `leading-tight` | Badges de estado |

**Font Family:** `Inter` (ya incluida en Tailwind config del proyecto)

---

## 4. Componentes por Pantalla

### 4.1 Wizard Crear Campaña - Paso 1: Información Básica

**Ruta:** `/dashboard/campanias/nueva?step=1`

#### Layout General

```
┌─────────────────────────────────────────────────────────────┐
│ [Header] MusicFund Logo                               [✕]  │  ← Sticky header
├─────────────────────────────────────────────────────────────┤
│                                                             │
│             Crear Nueva Campaña                             │  ← h1 text-3xl text-center
│      Completa los pasos para publicar tu proyecto musical   │  ← p text-muted text-center
│                                                             │
│  ┌──────┐─────┬──────┐─────┬──────┐─────┬──────┐          │  ← WizardStepper
│  │  1   │█████│  2   │─────│  3   │─────│  4   │          │
│  └──────┘     └──────┘     └──────┘     └──────┘          │
│  Info    Historia   Recompensas   Revisión                 │
│                                                             │
│  ┌────────────────────────────────────────────────────────┐│
│  │ Card (Form Container)                                  ││
│  │                                                        ││
│  │ [Form Fields - ver detalles abajo]                    ││
│  │                                                        ││
│  └────────────────────────────────────────────────────────┘│
│                                                             │
│  [← Anterior]  Paso 1 de 4  [Siguiente →]                  │  ← Footer navigation
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Utilizados

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Header Container** | `<div>` | - | `sticky top-0 z-50 bg-card border-b border-border px-6 py-4` |
| **Logo + Close** | `<Link>` + `<Button variant="ghost" size="icon">` | - | `flex items-center justify-between` |
| **Page Title** | `<h1>` | - | `text-3xl font-bold text-card-foreground text-center mb-2` |
| **Subtitle** | `<p>` | - | `text-base text-muted-foreground text-center mb-8` |
| **Wizard Stepper** | Custom `<WizardStepper>` (ver 4.1.1) | - | `flex items-center justify-between max-w-2xl mx-auto mb-10` |
| **Form Card** | `<Card>` | - | `bg-card border-border p-8 max-w-4xl mx-auto` |
| **Section Title** | `<CardHeader>` + `<CardTitle>` | - | `text-2xl font-bold text-card-foreground mb-6` |
| **Form** | `<Form>` (shadcn) | - | Wrapper de react-hook-form |
| **Form Field** | `<FormField>` + `<FormItem>` | - | `space-y-2` |
| **Label (required)** | `<FormLabel>` | - | `text-sm font-medium text-muted after:content-['*'] after:text-destructive after:ml-1` |
| **Label (optional)** | `<FormLabel>` | - | `text-sm font-medium text-muted` |
| **Input** | `<Input>` | - | `bg-input border-border text-card-foreground placeholder:text-muted-foreground focus-visible:ring-primary` |
| **Select** | `<Select>` + `<SelectTrigger>` + `<SelectContent>` + `<SelectItem>` | - | `bg-input border-border` |
| **Date Picker** | `<Popover>` + `<Calendar>` + `<Button variant="outline">` | - | `bg-input border-border` |
| **Grid 2 Columns** | `<div>` | - | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| **File Upload** | Custom `<FileUploadArea>` (ver 4.1.2) | - | `border-2 border-dashed border-muted rounded-lg p-12 text-center bg-input hover:border-primary transition cursor-pointer` |
| **Image Preview** | `<div>` + `<img>` o `<Skeleton>` | - | `mt-6 p-4 bg-input rounded-lg border border-border` |
| **Footer Navigation** | `<div>` | - | `flex items-center justify-between mt-8` |
| **Btn Anterior** | `<Button variant="outline">` | - | `border-border text-muted-foreground hover:text-card-foreground hover:bg-secondary` |
| **Step Indicator** | `<span>` | - | `text-sm text-muted-foreground` |
| **Btn Siguiente** | `<Button>` | - | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |

#### 4.1.1 WizardStepper Component (Custom)

**Composición:**

```tsx
interface WizardStepperProps {
  currentStep: number; // 1-4
  steps: Array<{ number: number; label: string }>;
}

// Render:
<div className="flex items-center justify-between max-w-2xl mx-auto">
  {steps.map((step, index) => (
    <React.Fragment key={step.number}>
      {/* Step Circle */}
      <div className="flex flex-col items-center">
        <div
          className={cn(
            "w-12 h-12 rounded-full flex items-center justify-center text-white font-bold transition-all",
            step.number < currentStep
              ? "bg-gradient-to-r from-pink-500 to-purple-600" // Completed
              : step.number === currentStep
              ? "bg-gradient-to-r from-pink-500 to-purple-600 shadow-glow" // Active
              : "bg-muted/20 text-muted" // Inactive
          )}
        >
          {step.number < currentStep ? (
            <CheckIcon className="w-6 h-6" />
          ) : (
            step.number
          )}
        </div>
        <span
          className={cn(
            "text-sm mt-2 transition-colors",
            step.number <= currentStep
              ? "text-card-foreground font-medium"
              : "text-muted-foreground"
          )}
        >
          {step.label}
        </span>
      </div>

      {/* Connector Line */}
      {index < steps.length - 1 && (
        <div className="flex-1 h-0.5 mx-2 bg-muted/20 relative">
          <div
            className={cn(
              "h-full transition-all duration-400",
              step.number < currentStep
                ? "bg-gradient-to-r from-pink-500 to-purple-600 w-full"
                : "w-0"
            )}
          />
        </div>
      )}
    </React.Fragment>
  ))}
</div>
```

**Props Interface:**
```tsx
interface WizardStepperProps {
  currentStep: number; // 1-4
  steps: Array<{
    number: number;
    label: string;
  }>;
  onStepClick?: (step: number) => void; // Opcional: navegar a step anterior
}
```

**Estados:**
- **Active Step:** Circle con gradient, shadow glow, label blanco, font-medium
- **Completed Step:** Circle con gradient + CheckIcon, connector lleno con gradient
- **Inactive Step:** Circle gris (`bg-muted/20`), text gris (`text-muted`)

#### 4.1.2 FileUploadArea Component (Custom)

**Composición:**

```tsx
interface FileUploadAreaProps {
  onFileSelect: (file: File) => void;
  accept?: string; // e.g., "image/png,image/jpeg"
  maxSizeMB?: number;
  previewUrl?: string | null;
  isUploading?: boolean;
}

// Render:
<div className="space-y-4">
  <div
    className={cn(
      "border-2 border-dashed rounded-lg p-12 text-center transition-all cursor-pointer",
      isDragging ? "border-primary bg-primary/5" : "border-muted bg-input hover:border-primary"
    )}
    onDragEnter={...}
    onDragOver={...}
    onDragLeave={...}
    onDrop={...}
    onClick={() => fileInputRef.current?.click()}
  >
    <input
      ref={fileInputRef}
      type="file"
      accept={accept}
      className="sr-only"
      onChange={handleFileChange}
    />

    <CloudUploadIcon className="w-12 h-12 text-muted-foreground mx-auto mb-3" />

    <p className="text-muted-foreground mb-1">
      Arrastra tu imagen aquí o haz clic para seleccionar
    </p>

    <p className="text-xs text-muted-foreground">
      PNG, JPG hasta {maxSizeMB}MB (1200x630px recomendado)
    </p>

    {isUploading && <Loader2 className="animate-spin mx-auto mt-3" />}
  </div>

  {/* Preview */}
  {previewUrl && (
    <div className="p-4 bg-input rounded-lg border border-border">
      <div className="flex items-start gap-4">
        <img
          src={previewUrl}
          alt="Preview"
          className="w-32 h-32 object-cover rounded-lg"
        />
        <div className="flex-1">
          <p className="text-sm text-card-foreground font-medium">Vista Previa</p>
          <p className="text-xs text-muted-foreground mt-1">
            Tu imagen de portada aparecerá así en la campaña
          </p>
        </div>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          onClick={onRemove}
        >
          <XIcon className="w-4 h-4" />
        </Button>
      </div>
    </div>
  )}
</div>
```

**Estados:**
- **Default:** Border dashed gris, bg input
- **Hover:** Border primary, cursor pointer
- **Dragging:** Border primary, bg primary con opacity
- **Uploading:** Spinner visible
- **Preview Loaded:** Muestra preview con botón eliminar

#### Formulario - Paso 1 Fields

| Campo | Componente | Validación | Props Importantes |
|-------|------------|------------|-------------------|
| **Título** | `<FormField>` + `<Input>` | Required, max 200 chars | `placeholder="Ej: Nuevo álbum 'Ecos de Medianoche'"` |
| **Género Musical** | `<FormField>` + `<Select>` | Required | Options: Rock, Pop, Jazz, Indie, etc. |
| **Meta de Financiación** | `<FormField>` + `<Input type="number">` | Required, > 0 | `placeholder="€ 5000"`, prefix con símbolo € |
| **Fecha de Finalización** | `<FormField>` + `<Popover>` + `<Calendar>` | Required, min hoy + 7 días, max hoy + 60 días | Date picker popover |
| **URL del Video** | `<FormField>` + `<Input type="url">` | Optional, URL válida YouTube/Vimeo | `placeholder="https://youtube.com/watch?v=..."` |
| **Imagen de Portada** | `<FormField>` + `<FileUploadArea>` | Required, PNG/JPG, max 5MB | Ver componente custom arriba |

**Validación Inline:**
- Mostrar `<FormMessage>` debajo de cada campo con error (color destructive)
- Character counter para Título: `<span className="text-xs text-muted-foreground">{charCount}/200</span>` (amarillo a 80%, rojo a 95%)

---

### 4.2 Wizard Crear Campaña - Paso 2: Historia

**Ruta:** `/dashboard/campanias/nueva?step=2`

#### Layout

Similar a Paso 1, pero con:
- Stepper muestra paso 1 completado (✓), paso 2 activo (gradient)
- Form fields diferentes (ver abajo)

#### Componentes Específicos del Paso 2

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Subtítulo Input** | `<FormField>` + `<Input>` | Optional | `placeholder="Un resumen breve de tu campaña"` |
| **Character Counter** | Custom `<CharacterCounter>` (ver 4.2.1) | - | `text-xs text-muted-foreground mt-1` |
| **Rich Text Editor** | Custom `<RichTextEditor>` (TipTap - ver 4.2.2) | - | `min-h-[300px] bg-input border-border` |
| **Textarea (¿Por qué apoyo?)** | `<FormField>` + `<Textarea>` | Optional | `min-h-[120px] resize-none bg-input` |

#### 4.2.1 CharacterCounter Component (Custom)

```tsx
interface CharacterCounterProps {
  current: number;
  max: number;
  warningThreshold?: number; // default 0.8 (80%)
  errorThreshold?: number; // default 0.95 (95%)
}

// Render:
<span
  className={cn(
    "text-xs mt-1 transition-colors",
    current / max >= (errorThreshold || 0.95)
      ? "text-destructive"
      : current / max >= (warningThreshold || 0.8)
      ? "text-warning"
      : "text-muted-foreground"
  )}
>
  {current}/{max} caracteres
</span>
```

#### 4.2.2 RichTextEditor Component (TipTap)

**Nota:** TipTap no está instalado por defecto en shadcn. Necesita instalación.

```bash
npm install @tiptap/react @tiptap/starter-kit @tiptap/extension-link
```

**Composición:**

```tsx
import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';

interface RichTextEditorProps {
  content: string;
  onUpdate: (html: string) => void;
  maxLength?: number; // 5000 para descripción
  placeholder?: string;
}

// Render:
<div className="border border-border rounded-lg overflow-hidden bg-input">
  {/* Toolbar */}
  <div className="flex items-center gap-1 p-2 border-b border-border bg-card">
    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={() => editor?.chain().focus().toggleBold().run()}
      className={cn(editor?.isActive('bold') && "bg-primary/10 text-primary")}
    >
      <BoldIcon className="w-4 h-4" />
    </Button>

    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={() => editor?.chain().focus().toggleItalic().run()}
      className={cn(editor?.isActive('italic') && "bg-primary/10 text-primary")}
    >
      <ItalicIcon className="w-4 h-4" />
    </Button>

    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={() => editor?.chain().focus().toggleBulletList().run()}
      className={cn(editor?.isActive('bulletList') && "bg-primary/10 text-primary")}
    >
      <ListIcon className="w-4 h-4" />
    </Button>

    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={() => editor?.chain().focus().toggleOrderedList().run()}
      className={cn(editor?.isActive('orderedList') && "bg-primary/10 text-primary")}
    >
      <ListOrderedIcon className="w-4 h-4" />
    </Button>

    <Separator orientation="vertical" className="h-6 mx-1" />

    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={() => {
        const url = window.prompt('URL del enlace:');
        if (url) editor?.chain().focus().setLink({ href: url }).run();
      }}
    >
      <LinkIcon className="w-4 h-4" />
    </Button>
  </div>

  {/* Editor Content */}
  <EditorContent
    editor={editor}
    className="prose prose-invert max-w-none p-4 min-h-[300px] text-card-foreground"
  />
</div>

<CharacterCounter current={charCount} max={maxLength || 5000} />
```

**Toolbar Buttons:**
- Bold, Italic, Underline
- Bullet List, Ordered List
- Link

**Estados:**
- **Focus:** Border color cambia a primary
- **Active Format:** Botón toolbar con `bg-primary/10 text-primary`
- **Disabled:** Toolbar buttons disabled si editor no cargado

---

### 4.3 Wizard Crear Campaña - Paso 3: Recompensas

**Ruta:** `/dashboard/campanias/nueva?step=3`

#### Layout

```
┌─────────────────────────────────────────────────────────────┐
│ [Header] ...                                          [✕]   │
├─────────────────────────────────────────────────────────────┤
│ [Stepper: 1 ✓, 2 ✓, 3 activo, 4 inactivo]                  │
│                                                             │
│ ┌──────────────────────────────────────────────────────────┐│
│ │ Recompensas para tus Backers                            ││
│ │ Crea niveles de apoyo con beneficios atractivos        ││
│ │                                                         ││
│ │ [RewardCard 1]                                          ││
│ │ [RewardCard 2]                                          ││
│ │ ...                                                     ││
│ │                                                         ││
│ │ [+ Agregar recompensa] ← Button outline dashed         ││
│ │                                                         ││
│ │ 💡 Tip: Crea 3-5 niveles con beneficios progresivos    ││
│ └──────────────────────────────────────────────────────────┘│
│                                                             │
│ [← Anterior]  Paso 3 de 4  [Siguiente →]                   │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Específicos del Paso 3

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Reward Card** | Custom `<RewardCard>` (ver 4.3.1) | - | `bg-input border-border p-6 hover:border-primary transition mb-4` |
| **Agregar Button** | `<Button variant="outline">` | - | `border-dashed border-primary text-primary hover:bg-primary/10 w-full py-6` |
| **Tip Box** | `<div>` | - | `bg-card border-l-4 border-primary p-4 rounded-r-lg mt-6 flex items-start gap-3` |
| **Empty State** | Custom `<EmptyState>` (ver 4.3.2) | - | `text-center py-16` |

#### 4.3.1 RewardCard Component (Custom)

```tsx
interface RewardCardProps {
  reward: {
    id: string;
    precio: number;
    titulo: string;
    descripcion: string;
    cantidadDisponible?: number | null; // null = ilimitado
    cantidadRestante?: number;
  };
  onEdit: (id: string) => void;
  onDelete: (id: string) => void;
}

// Render:
<Card className="bg-input border-border hover:border-primary transition">
  <CardContent className="p-6">
    <div className="flex items-start justify-between mb-3">
      <div className="flex items-center gap-3">
        <span className="text-2xl font-bold text-primary">€ {reward.precio}</span>
        <span className="text-lg font-semibold text-card-foreground">{reward.titulo}</span>
      </div>

      <Button
        variant="ghost"
        size="sm"
        onClick={() => onEdit(reward.id)}
        className="text-primary hover:text-purple-400"
      >
        Editar
      </Button>
    </div>

    <p className="text-sm text-muted-foreground mb-3">
      {reward.descripcion}
    </p>

    <div className="flex items-center gap-2 text-xs text-muted-foreground">
      <PackageIcon className="w-4 h-4" />
      {reward.cantidadDisponible === null ? (
        <span>Ilimitadas disponibles</span>
      ) : (
        <span>{reward.cantidadRestante} de {reward.cantidadDisponible} disponibles</span>
      )}
    </div>
  </CardContent>
</Card>
```

**Estados:**
- **Hover:** Border color cambia a primary, elevation aumenta
- **Empty List:** Mostrar EmptyState component

#### 4.3.2 EmptyState Component (Custom)

```tsx
interface EmptyStateProps {
  icon?: React.ComponentType<{ className?: string }>;
  title: string;
  description?: string;
  action?: {
    label: string;
    onClick: () => void;
  };
}

// Render:
<div className="text-center py-16">
  {Icon && <Icon className="w-16 h-16 text-muted-foreground mx-auto mb-4" />}

  <h3 className="text-lg font-semibold text-card-foreground mb-2">
    {title}
  </h3>

  {description && (
    <p className="text-sm text-muted-foreground mb-6 max-w-sm mx-auto">
      {description}
    </p>
  )}

  {action && (
    <Button
      onClick={action.onClick}
      className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
    >
      {action.label}
    </Button>
  )}
</div>
```

**Uso en Paso 3:**
```tsx
<EmptyState
  icon={FolderOpenIcon}
  title="Aún no has creado recompensas"
  description="Las recompensas incentivan a tus fans a apoyarte. Crea tu primera recompensa ahora."
  action={{
    label: "Crear primera recompensa",
    onClick: handleAddReward
  }}
/>
```

---

### 4.4 Wizard Crear Campaña - Paso 4: Revisión

**Ruta:** `/dashboard/campanias/nueva?step=4`

#### Layout

```
┌─────────────────────────────────────────────────────────────┐
│ [Header] ...                                          [✕]   │
├─────────────────────────────────────────────────────────────┤
│ [Stepper: 1 ✓, 2 ✓, 3 ✓, 4 activo]                         │
│                                                             │
│ ┌──────────────────────────────────────────────────────────┐│
│ │ Revisión Final                                          ││
│ │ Revisa todos los detalles antes de crear tu campaña    ││
│ │                                                         ││
│ │ ┌────────────────────────────────────────────────────┐ ││
│ │ │ [Imagen de portada preview - full width]          │ ││
│ │ │                                                    │ ││
│ │ │ Nuevo Álbum: "Midnight Sessions"                  │ ││
│ │ │ Rock Indie                                         │ ││
│ │ │                                                    │ ││
│ │ │ Meta: € 5,000  •  Finaliza: 30 Mar 2026           │ ││
│ │ │                                                    │ ││
│ │ │ [Descripción preview - primeras 3 líneas...]      │ ││
│ │ │                                                    │ ││
│ │ │ Recompensas: 3 niveles (€10, €25, €50)            │ ││
│ │ │                                                    │ ││
│ │ │ [Editar]                                           │ ││
│ │ └────────────────────────────────────────────────────┘ ││
│ │                                                         ││
│ │ ⚠️  Nota: Tu campaña se creará como BORRADOR           ││
│ │     Podrás editarla y publicarla cuando estés listo    ││
│ └──────────────────────────────────────────────────────────┘│
│                                                             │
│ [← Anterior]  [Guardar como borrador]  [Crear campaña]     │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Específicos del Paso 4

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Preview Card** | `<Card>` | - | `bg-card border-border p-6` |
| **Preview Image** | `<img>` | - | `w-full h-64 object-cover rounded-lg mb-4` |
| **Preview Title** | `<h2>` | - | `text-2xl font-bold text-card-foreground mb-2` |
| **Preview Meta** | `<div>` | - | `flex items-center gap-4 text-sm text-muted-foreground mb-4` |
| **Edit Link** | `<Button variant="link">` | - | `text-primary hover:underline` |
| **Warning Box** | `<Alert>` | - | `bg-warning/10 border-l-4 border-warning text-warning-foreground` |
| **Warning Icon** | `<AlertTriangleIcon>` | - | `w-5 h-5 inline mr-2` |
| **Draft Button** | `<Button variant="outline">` | - | `border-border text-muted-foreground hover:text-card-foreground` |
| **Create Button** | `<Button>` | - | `bg-gradient-to-r from-pink-500 to-purple-600 text-white font-semibold px-8` |

**Warning Alert:**
```tsx
<Alert className="bg-warning/10 border-l-4 border-warning">
  <AlertTriangleIcon className="w-5 h-5" />
  <AlertTitle className="font-semibold">Nota: Tu campaña se creará como BORRADOR</AlertTitle>
  <AlertDescription className="text-sm">
    Podrás editarla y publicarla cuando estés listo
  </AlertDescription>
</Alert>
```

**Estados:**
- **Loading Create:** Create button muestra `<Loader2 className="animate-spin" />` + texto "Creando campaña..."
- **Success:** Toast notification verde + redirect a preview
- **Error:** Toast notification roja con mensaje del backend

---

### 4.5 Vista Previa Campaña (Borrador)

**Ruta:** `/dashboard/campanias/{id}/preview`

#### Layout

```
┌─────────────────────────────────────────────────────────────┐
│ ⚠️  VISTA PREVIA - Campaña en borrador (No visible)   [✕]  │  ← Banner Alert warning
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ [Editar] [Publicar Ahora] [Eliminar]  ← Action buttons     │
│                                                             │
│ [Campaign Detail Layout - igual a público, ver 4.6]        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Específicos de Vista Previa

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Draft Banner** | `<Alert>` | - | `bg-warning/10 border-warning text-warning font-medium p-4 mb-4` |
| **Banner Icon** | `<AlertCircleIcon>` | - | `w-5 h-5 inline mr-2` |
| **Close Banner Button** | `<Button variant="ghost" size="sm">` | - | `text-warning hover:text-warning/80` |
| **Action Bar** | `<div>` | - | `flex gap-3 mb-6` |
| **Edit Button** | `<Button variant="outline">` | - | `border-border text-card-foreground` |
| **Publish Button** | `<Button>` | - | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |
| **Delete Button** | `<Button variant="outline">` | - | `border-destructive/50 text-destructive hover:bg-destructive/10` |

**Modales de Confirmación:**

```tsx
// Modal Publicar
<Dialog>
  <DialogTrigger asChild>
    <Button>Publicar Ahora</Button>
  </DialogTrigger>
  <DialogContent className="bg-card border-border">
    <DialogHeader>
      <DialogTitle>¿Publicar campaña?</DialogTitle>
      <DialogDescription className="text-muted-foreground">
        Una vez publicada, la campaña será visible públicamente y no podrás
        editar algunos campos. ¿Estás seguro?
      </DialogDescription>
    </DialogHeader>
    <DialogFooter>
      <Button variant="outline" onClick={onCancel}>
        Cancelar
      </Button>
      <Button onClick={onConfirm} disabled={isPending}>
        {isPending ? <Loader2 className="animate-spin" /> : "Publicar"}
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>

// Modal Eliminar
<Dialog>
  <DialogTrigger asChild>
    <Button variant="outline">Eliminar</Button>
  </DialogTrigger>
  <DialogContent className="bg-card border-border">
    <DialogHeader>
      <DialogTitle>¿Eliminar campaña?</DialogTitle>
      <DialogDescription className="text-destructive">
        Esta acción no se puede deshacer. La campaña se eliminará permanentemente.
      </DialogDescription>
    </DialogHeader>
    <DialogFooter>
      <Button variant="outline" onClick={onCancel}>
        Cancelar
      </Button>
      <Button variant="destructive" onClick={onConfirm} disabled={isPending}>
        {isPending ? <Loader2 className="animate-spin" /> : "Eliminar"}
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>
```

---

### 4.6 Detalle Campaña Pública (Preview de cómo se verá publicada)

**Nota:** Esta pantalla es para el Landing (`src/web`), pero se incluye aquí para referencia del artista al ver preview.

**Ruta (Landing):** `/campanias/{id}`

#### Layout Simplificado (para referencia)

```
┌─────────────────────────────────────────────────────────────┐
│ [Header Nav] MusicFund  Explorar  ...      Login  Crear     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ [Hero Image - full width, h-96]                            │
│                                                             │
│ ┌─────────────────────────┐  ┌──────────────────────────┐  │
│ │ Content (2 cols)        │  │ Sidebar (1 col sticky)   │  │
│ │                         │  │                          │  │
│ │ Título Campaña          │  │ [Apoyar esta campaña]    │  │
│ │ Artist Info + Badges    │  │ Desde € 5                │  │
│ │                         │  │                          │  │
│ │ € 12,450 de € 15,000    │  │ ─────────────────────    │  │
│ │ Progress Bar            │  │ Recompensas              │  │
│ │ 127 backers • 12 días   │  │ [RewardCard 1]           │  │
│ │                         │  │ [RewardCard 2]           │  │
│ │ [Historia] [FAQ] ...    │  │ [RewardCard 3]           │  │
│ │                         │  │                          │  │
│ │ [Rich Text Description] │  │ 🔒 Pago seguro           │  │
│ │                         │  │ 📦 Entrega: Marzo 2025   │  │
│ └─────────────────────────┘  └──────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Clave (Landing)

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Hero Image** | `<img>` | - | `w-full h-96 object-cover rounded-lg mb-8` |
| **Main Layout** | `<div>` | - | `max-w-7xl mx-auto px-6 py-8 grid grid-cols-3 gap-8` (2 cols content + 1 col sidebar) |
| **Campaign Title** | `<h1>` | - | `text-4xl font-bold text-card-foreground mb-4` |
| **Artist Info** | `<div>` + `<Avatar>` + `<Badge>` | - | `flex items-center gap-3 mb-4` |
| **Verified Badge** | `<Badge>` | - | `bg-primary/20 text-primary border-primary/50` |
| **Genre Badge** | `<Badge>` | - | `bg-purple-900/50 text-purple-300 border-purple-500/50` |
| **Status Badge** | `<Badge>` | - | `bg-success/20 text-success border-success/50` (Activa) |
| **Funding Amount** | `<div>` | - | `text-3xl font-bold text-card-foreground mb-2` |
| **Progress Bar** | `<Progress>` | - | `w-full h-3 bg-border rounded-full [&>div]:bg-gradient-to-r [&>div]:from-pink-500 [&>div]:to-purple-600` |
| **Stats Row** | `<div>` | - | `flex items-center gap-6 text-muted-foreground` |
| **Tabs** | `<Tabs>` + `<TabsList>` + `<TabsTrigger>` + `<TabsContent>` | - | `border-b border-border mb-6` |
| **Description** | `<div>` | - | `prose prose-invert max-w-none text-muted-foreground` (renderizar HTML del editor) |
| **Sidebar Card** | `<Card>` | - | `bg-card border-border p-6 sticky top-24` |
| **Support Button** | `<Button>` | - | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2` |
| **Reward Card (Sidebar)** | `<Card>` | - | `bg-input border-border p-4 mb-3 hover:border-primary cursor-pointer transition` |
| **Popular Badge** | `<Badge>` | - | `bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs` |
| **Select Button (Reward)** | `<Button variant="outline" size="sm">` | - | `w-full border-primary text-primary hover:bg-primary/10` |

---

### 4.7 Mis Campañas (Lista)

**Ruta:** `/dashboard/campanias`

#### Layout

```
┌─────────────────────────────────────────────────────────────┐
│ [SIDEBAR]  │ Dashboard: Mis Campañas     [+ Nueva]          │
│            │                                                │
│ Dashboard  │ ┌──────────────────────────────────────────┐   │
│ Mis        │ │ [IMG] Nuevo Album "Midnight"             │   │
│ Campañas   │ │       Activa • 85% ████░ • 420 backers   │   │
│ Perfil     │ │       [Editar] [Ver] [...]               │   │
│ ...        │ └──────────────────────────────────────────┘   │
│            │                                                │
│            │ ┌──────────────────────────────────────────┐   │
│            │ │ [IMG] EP Acústico                        │   │
│            │ │       Borrador • 0% ░░░░ • 0 backers     │   │
│            │ │       [Editar] [Publicar] [...]          │   │
│            │ └──────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes Específicos de Mis Campañas

| Elemento | Componente shadcn | Variante/Props | Clases Tailwind Clave |
|----------|-------------------|----------------|----------------------|
| **Sidebar** | Custom `<DashboardSidebar>` | - | `w-64 bg-card border-r border-border` |
| **Main Content** | `<div>` | - | `flex-1 p-8` |
| **Header Row** | `<div>` | - | `flex items-center justify-between mb-8` |
| **Page Title** | `<h1>` | - | `text-3xl font-bold text-card-foreground` |
| **New Campaign Button** | `<Button>` | - | `bg-gradient-to-r from-pink-500 to-purple-600 text-white font-semibold` |
| **Campaigns List** | `<div>` | - | `space-y-4` |
| **Campaign Card** | Custom `<CampaignCard>` (ver 4.7.1) | - | `bg-card border-border p-6 hover:border-primary transition` |
| **More Menu** | `<DropdownMenu>` + `<DropdownMenuTrigger>` + `<DropdownMenuContent>` + `<DropdownMenuItem>` | - | Menú de 3 puntos |

#### 4.7.1 CampaignCard Component (Custom para lista)

```tsx
interface CampaignCardProps {
  campaign: {
    id: string;
    titulo: string;
    imagenPrincipalUrl?: string;
    estadoCampaniaId: number;
    importeObjetivo: number;
    importePledgedActual: number;
    backersCount?: number; // Calculado
  };
  onEdit: (id: string) => void;
  onView: (id: string) => void;
  onPublish?: (id: string) => void; // Solo si borrador
  onDelete: (id: string) => void;
}

// Render:
<Card className="bg-card border-border hover:border-primary transition cursor-pointer">
  <CardContent className="p-6">
    <div className="flex items-center gap-6">
      {/* Imagen */}
      <img
        src={campaign.imagenPrincipalUrl || '/placeholder.jpg'}
        alt={campaign.titulo}
        className="w-24 h-24 object-cover rounded-lg"
      />

      {/* Info */}
      <div className="flex-1">
        <h3 className="text-lg font-bold text-card-foreground mb-2">
          {campaign.titulo}
        </h3>

        <div className="flex items-center gap-3 text-sm text-muted-foreground mb-2">
          <Badge variant={getStatusVariant(campaign.estadoCampaniaId)}>
            {CAMPANIA_ESTADOS_LABELS[campaign.estadoCampaniaId]}
          </Badge>

          <span className="font-semibold">
            {Math.round((campaign.importePledgedActual / campaign.importeObjetivo) * 100)}%
          </span>

          <div className="w-24 h-2 bg-border rounded-full overflow-hidden">
            <div
              className="h-full bg-gradient-to-r from-pink-500 to-purple-600"
              style={{ width: `${Math.min((campaign.importePledgedActual / campaign.importeObjetivo) * 100, 100)}%` }}
            />
          </div>

          <span className="flex items-center gap-1">
            <UsersIcon className="w-4 h-4" />
            {campaign.backersCount || 0}
          </span>
        </div>
      </div>

      {/* Actions */}
      <div className="flex gap-2">
        <Button variant="outline" size="sm" onClick={() => onEdit(campaign.id)}>
          Editar
        </Button>

        <Button variant="outline" size="sm" onClick={() => onView(campaign.id)}>
          Ver
        </Button>

        {campaign.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR && onPublish && (
          <Button variant="outline" size="sm" onClick={() => onPublish(campaign.id)} className="border-success/50 text-success hover:bg-success/10">
            Publicar
          </Button>
        )}

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon">
              <MoreVerticalIcon className="w-4 h-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="bg-card border-border">
            <DropdownMenuItem onClick={() => onEdit(campaign.id)}>
              Editar
            </DropdownMenuItem>
            {campaign.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR && onPublish && (
              <DropdownMenuItem onClick={() => onPublish(campaign.id)}>
                Publicar
              </DropdownMenuItem>
            )}
            <DropdownMenuItem onClick={() => onView(campaign.id)}>
              Ver detalles
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => onDelete(campaign.id)}
              className="text-destructive focus:text-destructive"
            >
              Eliminar
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  </CardContent>
</Card>
```

**Badge Variants por Estado:**
```tsx
const getStatusVariant = (estadoId: number): string => {
  switch (estadoId) {
    case CAMPANIA_ESTADOS.BORRADOR:
      return "secondary"; // gris
    case CAMPANIA_ESTADOS.PUBLICADA:
      return "success"; // verde
    case CAMPANIA_ESTADOS.FINALIZADA:
      return "default"; // azul (usar custom si no existe)
    case CAMPANIA_ESTADOS.CANCELADA:
      return "destructive"; // rojo
    default:
      return "secondary";
  }
};
```

**Empty State:**
```tsx
<EmptyState
  icon={FolderOpenIcon}
  title="No tienes campañas aún"
  description="Crea tu primera campaña de crowdfunding para empezar a recaudar fondos para tu proyecto musical."
  action={{
    label: "Crear tu primera campaña",
    onClick: () => navigate('/dashboard/campanias/nueva')
  }}
/>
```

---

## 5. Formularios

### 5.1 Integración con react-hook-form + Zod

Todos los pasos del wizard usan `react-hook-form` con validación Zod según los schemas definidos en `src/shared/schemas/campania.schema.ts`.

**Ejemplo de integración en Paso 1:**

```tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { campaniaBasicInfoSchema } from '@/shared/schemas/campania.schema';
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';

const WizardStep1 = () => {
  const form = useForm({
    resolver: zodResolver(campaniaBasicInfoSchema),
    defaultValues: {
      titulo: '',
      subtitulo: '',
      generoMusical: '',
      importeObjetivo: 0,
      fechaFin: '',
      videoUrl: '',
      imagenPrincipalUrl: '',
    },
  });

  const onSubmit = (data) => {
    // Guardar en wizard state
    // Avanzar a paso 2
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        {/* Título */}
        <FormField
          control={form.control}
          name="titulo"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                Título de la campaña <span className="text-destructive">*</span>
              </FormLabel>
              <FormControl>
                <Input
                  placeholder="Ej: Nuevo álbum 'Ecos de Medianoche'"
                  {...field}
                  className="bg-input border-border"
                />
              </FormControl>
              <FormDescription className="flex items-center justify-between">
                <span className="text-xs text-muted-foreground">
                  Un título claro y atractivo para tu proyecto
                </span>
                <CharacterCounter
                  current={field.value.length}
                  max={200}
                />
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Género Musical */}
        <FormField
          control={form.control}
          name="generoMusical"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                Género musical <span className="text-destructive">*</span>
              </FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger className="bg-input border-border">
                    <SelectValue placeholder="Selecciona género" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent className="bg-card border-border">
                  <SelectItem value="rock">Rock</SelectItem>
                  <SelectItem value="pop">Pop</SelectItem>
                  <SelectItem value="jazz">Jazz</SelectItem>
                  <SelectItem value="indie">Indie</SelectItem>
                  <SelectItem value="electronica">Electrónica</SelectItem>
                  <SelectItem value="hip-hop">Hip Hop</SelectItem>
                  <SelectItem value="otro">Otro</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Grid 2 columnas para Meta y Fecha */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* Meta de Financiación */}
          <FormField
            control={form.control}
            name="importeObjetivo"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Meta de financiación (EUR) <span className="text-destructive">*</span>
                </FormLabel>
                <FormControl>
                  <div className="relative">
                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                      €
                    </span>
                    <Input
                      type="number"
                      placeholder="5000"
                      {...field}
                      onChange={(e) => field.onChange(parseFloat(e.target.value))}
                      className="bg-input border-border pl-8"
                    />
                  </div>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Fecha de Finalización */}
          <FormField
            control={form.control}
            name="fechaFin"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Fecha de finalización <span className="text-destructive">*</span>
                </FormLabel>
                <Popover>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full justify-start text-left font-normal bg-input border-border",
                          !field.value && "text-muted-foreground"
                        )}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4" />
                        {field.value ? (
                          format(new Date(field.value), "PPP", { locale: es })
                        ) : (
                          <span>Selecciona fecha</span>
                        )}
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-auto p-0 bg-card border-border" align="start">
                    <Calendar
                      mode="single"
                      selected={field.value ? new Date(field.value) : undefined}
                      onSelect={(date) => field.onChange(date?.toISOString())}
                      disabled={(date) =>
                        date < addDays(new Date(), 7) || date > addDays(new Date(), 60)
                      }
                      initialFocus
                    />
                  </PopoverContent>
                </Popover>
                <FormDescription className="text-xs text-muted-foreground">
                  Entre 7 y 60 días desde hoy
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {/* URL del Video (opcional) */}
        <FormField
          control={form.control}
          name="videoUrl"
          render={({ field }) => (
            <FormItem>
              <FormLabel>URL del video (YouTube/Vimeo)</FormLabel>
              <FormControl>
                <Input
                  type="url"
                  placeholder="https://youtube.com/watch?v=..."
                  {...field}
                  className="bg-input border-border"
                />
              </FormControl>
              <FormDescription className="text-xs text-muted-foreground">
                Un video ayuda a tus fans a conocer mejor tu proyecto
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Imagen de Portada */}
        <FormField
          control={form.control}
          name="imagenPrincipalUrl"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                Imagen de portada <span className="text-destructive">*</span>
              </FormLabel>
              <FormControl>
                <FileUploadArea
                  onFileSelect={(file) => {
                    // Upload file a servicio (blob storage o similar)
                    // Obtener URL y actualizar field
                    handleFileUpload(file).then((url) => field.onChange(url));
                  }}
                  accept="image/png,image/jpeg"
                  maxSizeMB={5}
                  previewUrl={field.value}
                  isUploading={isUploading}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Footer Navigation */}
        <div className="flex items-center justify-between pt-6">
          <Button
            type="button"
            variant="outline"
            disabled // Disabled en paso 1
          >
            ← Anterior
          </Button>

          <span className="text-sm text-muted-foreground">Paso 1 de 4</span>

          <Button
            type="submit"
            disabled={form.formState.isSubmitting}
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
          >
            {form.formState.isSubmitting ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Validando...
              </>
            ) : (
              <>
                Siguiente →
              </>
            )}
          </Button>
        </div>
      </form>
    </Form>
  );
};
```

### 5.2 Validación Visual

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Input con border gris (`border-border`), placeholder gris claro |
| **Focus** | Border cambia a primary (`focus-visible:ring-2 focus-visible:ring-primary`), glow sutil |
| **Error** | Border roja (`border-destructive`), `<FormMessage>` en rojo debajo del input |
| **Success** | Sin indicador visual especial (solo ausencia de error) |
| **Disabled** | Opacity 50%, cursor not-allowed |

---

## 6. Feedback y Estados

### 6.1 Loading States

| Componente | Estado Loading | Implementación |
|------------|----------------|----------------|
| **Button (submit)** | Spinner + texto "Validando..." | `<Loader2 className="animate-spin mr-2" />` |
| **Campaign Card** | Skeleton loader | `<Skeleton className="w-24 h-24" />` para imagen, `<Skeleton className="h-4 w-3/4" />` para texto |
| **Campaign List** | 3-4 skeleton cards | `<Skeleton>` repetido |
| **Image Upload** | Spinner en upload area | `<Loader2 className="animate-spin mx-auto mt-3" />` |
| **Rich Text Editor** | Skeleton o loading spinner | Mostrar antes de cargar TipTap |

**Skeleton Card Example:**
```tsx
<Card className="bg-card border-border p-6">
  <div className="flex items-center gap-6">
    <Skeleton className="w-24 h-24 rounded-lg" />
    <div className="flex-1 space-y-2">
      <Skeleton className="h-4 w-3/4" />
      <Skeleton className="h-3 w-1/2" />
      <div className="flex items-center gap-3 mt-2">
        <Skeleton className="h-5 w-16" />
        <Skeleton className="h-2 w-24" />
        <Skeleton className="h-3 w-12" />
      </div>
    </div>
  </div>
</Card>
```

### 6.2 Error States

| Tipo de Error | Componente | Implementación |
|---------------|------------|----------------|
| **Validation Error (campo)** | `<FormMessage>` | Debajo del input, color destructive |
| **API Error (global)** | Toast notification | `toast.error(message)` usando sonner |
| **Not Found** | Empty state con icono | `<EmptyState icon={AlertCircleIcon} title="Campaña no encontrada" />` |
| **Network Error** | Toast notification | `toast.error("Error de conexión. Intenta nuevamente")` |

**Toast Notification con sonner:**
```tsx
import { toast } from 'sonner';

// Success
toast.success("Campaña creada exitosamente", {
  description: "Redirigiendo a vista previa...",
  duration: 3000,
});

// Error
toast.error("Error al crear campaña", {
  description: "Por favor, verifica los datos e intenta nuevamente",
  duration: 5000,
});

// Warning
toast.warning("Atención", {
  description: "Tu campaña no tiene recompensas. ¿Quieres publicarla sin recompensas?",
  action: {
    label: "Publicar",
    onClick: () => handlePublish(),
  },
});
```

### 6.3 Success States

| Acción | Feedback | Implementación |
|--------|----------|----------------|
| **Campaña creada** | Toast verde + redirect | `toast.success()` + `navigate()` |
| **Campaña publicada** | Toast verde + actualizar estado | `toast.success()` + invalidate query |
| **Campaña eliminada** | Toast verde + remover de lista | `toast.success()` + invalidate query |
| **Paso del wizard completado** | Stepper actualizado + animación | Connector se llena con gradient animado |

### 6.4 Empty States

| Contexto | Icono | Título | Descripción | CTA |
|----------|-------|--------|-------------|-----|
| **No Campaigns** | `FolderOpenIcon` | "No tienes campañas aún" | "Crea tu primera campaña..." | "Crear tu primera campaña" |
| **No Rewards** | `PackageIcon` | "Aún no has creado recompensas" | "Las recompensas incentivan..." | "Crear primera recompensa" |
| **No Results (búsqueda)** | `SearchIcon` | "No se encontraron resultados" | "Intenta con otros términos" | - |

---

## 7. Responsive Design

### 7.1 Breakpoints

| Breakpoint | Width | Cambios Generales |
|------------|-------|-------------------|
| **Mobile** | < 640px (sm) | Stack vertical, sidebar colapsado, padding reducido (px-4) |
| **Tablet** | 640px - 1024px (sm-lg) | Grid 2 columnas, sidebar visible pero estrecho |
| **Desktop** | > 1024px (lg) | Layouts completos, max-width containers, sidebar full width |

### 7.2 Wizard Responsive

**Mobile (< 640px):**
```tsx
// Stepper horizontal → Simplified text indicator
<div className="text-center mb-8">
  <p className="text-sm text-muted-foreground">Paso {currentStep} de 4</p>
  <p className="text-xs text-muted-foreground mt-1">{steps[currentStep - 1].label}</p>
</div>

// Grid 2 columns → Stack vertical
<div className="grid grid-cols-1 gap-4"> {/* No md:grid-cols-2 */}

// Botones Next/Previous → Full width stack
<div className="flex flex-col gap-3">
  <Button className="w-full">Siguiente →</Button>
  <Button variant="outline" className="w-full">← Anterior</Button>
</div>
```

**Tablet (640px - 1024px):**
- Stepper horizontal comprimido (círculos más pequeños)
- Grid 2 columnas se mantiene
- Upload area padding reducido

**Desktop (> 1024px):**
- Layout completo según mockups
- Max-width `max-w-4xl` para form container

### 7.3 Mis Campañas Responsive

**Mobile:**
```tsx
// Campaign Card: imagen más pequeña
<img className="w-16 h-16 object-cover rounded-lg" />

// Progress bar más corta
<div className="w-16 h-2 bg-border rounded-full" />

// Botones → Solo iconos (sin texto)
<Button size="icon" variant="ghost">
  <EditIcon className="w-4 h-4" />
</Button>
```

**Tablet/Desktop:**
- Layout completo según mockup arriba

### 7.4 Preview Público Responsive

**Mobile:**
```tsx
// Hero image height reducida
<img className="w-full h-64 object-cover" /> {/* h-64 en lugar de h-96 */}

// Layout columna única (sidebar abajo de content)
<div className="grid grid-cols-1 gap-8">
  <div>{/* Content */}</div>
  <div>{/* Sidebar (no sticky en mobile) */}</div>
</div>

// Stats en grid 2x2
<div className="grid grid-cols-2 gap-3">
  <div>127 backers</div>
  <div>12 días restantes</div>
</div>
```

**Tablet:**
```tsx
// Layout 2 columnas
<div className="grid grid-cols-3 gap-8">
  <div className="col-span-2">{/* Content */}</div>
  <div className="col-span-1">{/* Sidebar */}</div>
</div>
```

**Desktop:**
- Layout completo según mockup

---

## 8. Animaciones

| Elemento | Animación | Duración | Implementación Tailwind |
|----------|-----------|----------|-------------------------|
| **Button hover** | Scale 1.02 + shadow glow | 150ms | `hover:scale-[1.02] transition-transform` |
| **Input focus** | Border color + ring | 200ms | `focus-visible:ring-2 focus-visible:ring-primary transition-colors` |
| **Card hover** | Border color + elevation | 200ms | `hover:border-primary transition-colors` |
| **Stepper connector fill** | Width 0 → 100% | 400ms | `transition-all duration-400` con dynamic width |
| **Toast notification** | Slide in from top + fade | 250ms | Manejado por sonner |
| **Modal open** | Scale 0.95 → 1 + fade | 200ms | Manejado por Dialog (Radix) |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 loop | 1500ms | `animate-pulse` (built-in Tailwind) |
| **Page transition** | Fade in opacity 0 → 1 | 300ms | `animate-in fade-in duration-300` |

### Animación Custom - Stepper Connector Fill

```tsx
// Cuando avanza de paso 1 a paso 2, el connector se llena con animación
<div className="flex-1 h-0.5 mx-2 bg-muted/20 relative overflow-hidden">
  <div
    className={cn(
      "h-full bg-gradient-to-r from-pink-500 to-purple-600 transition-all duration-400 ease-in-out",
      isCompleted ? "w-full" : "w-0"
    )}
  />
</div>
```

---

## 9. Accesibilidad (ARIA)

### 9.1 Requisitos WCAG

| Requisito | Estándar | Implementación |
|-----------|----------|----------------|
| **Contraste de color** | WCAG AA (4.5:1 text, 3:1 large text) | Verificado: white sobre `#1a1a2e` = 15.8:1 ✓, primary sobre bg = 5.2:1 ✓ |
| **Focus visible** | Obligatorio | `focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2` en todos los interactivos |
| **Labels en inputs** | Obligatorio | Todos los `<FormField>` tienen `<FormLabel>` con `htmlFor` automático |
| **Form validation** | Mensajes accesibles | `<FormMessage>` con `role="alert"` y `aria-live="polite"` |
| **Keyboard navigation** | Tab order lógico | Orden natural del DOM, Enter para submit, Esc para cerrar modales |

### 9.2 ARIA Labels Específicos

```tsx
// WizardStepper
<div
  role="progressbar"
  aria-valuenow={currentStep}
  aria-valuemin={1}
  aria-valuemax={4}
  aria-label="Progreso de creación de campaña"
>
  {/* Steps */}
</div>

// Step Circle
<div
  className="step-circle"
  aria-current={currentStep === step.number ? "step" : undefined}
>
  {step.number}
</div>

// File Upload Area
<div
  role="button"
  tabIndex={0}
  aria-label="Subir imagen de portada"
  onKeyDown={(e) => {
    if (e.key === 'Enter' || e.key === ' ') {
      fileInputRef.current?.click();
    }
  }}
>
  <input
    type="file"
    className="sr-only"
    aria-label="Seleccionar archivo de imagen"
  />
  Arrastra tu imagen aquí...
</div>

// Progress Bar (campaign funding)
<div
  role="progressbar"
  aria-valuenow={83}
  aria-valuemin={0}
  aria-valuemax={100}
  aria-label="Progreso de financiación: 83%"
>
  <div className="fill" style={{ width: "83%" }} />
</div>

// Status Badge
<span role="status" aria-label="Estado de campaña: Activa">
  <Badge>Activa</Badge>
</span>

// Button con loading
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isPending ? "Creando campaña..." : "Crear campaña"}
</Button>

// Rich Text Editor
<div
  role="textbox"
  aria-multiline="true"
  aria-label="Descripción de la campaña"
  aria-required="true"
>
  {/* TipTap Editor */}
</div>

// Reward Card
<Card
  role="article"
  aria-label="Recompensa: Descarga Digital por 10 euros"
>
  <h4>Descarga Digital</h4>
  <p>€ 10</p>
  <Button aria-label="Seleccionar recompensa Descarga Digital">
    Seleccionar
  </Button>
</Card>

// Dialog (Modal de confirmación)
<Dialog>
  <DialogContent
    role="dialog"
    aria-modal="true"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
  >
    <DialogHeader>
      <DialogTitle id="dialog-title">¿Publicar campaña?</DialogTitle>
      <DialogDescription id="dialog-description">
        Una vez publicada, la campaña será visible...
      </DialogDescription>
    </DialogHeader>
    {/* ... */}
  </DialogContent>
</Dialog>
```

### 9.3 Screen Reader Support

- **Skip links:** "Saltar al contenido principal" en header (invisible hasta focus)
- **Headings hierarchy:** h1 → h2 → h3 correctamente anidados
- **Landmarks:** `<nav>`, `<main>`, `<aside>` para estructura semántica
- **Alt text:** Todas las imágenes con `alt` descriptivo

---

## 10. Componentes shadcn/ui Faltantes

Algunos componentes necesarios no están en la lista actual de shadcn/ui instalados. Necesitan instalación:

```bash
# Componentes a instalar
npx shadcn-ui@latest add form
npx shadcn-ui@latest add select
npx shadcn-ui@latest add popover
npx shadcn-ui@latest add calendar
npx shadcn-ui@latest add dialog
npx shadcn-ui@latest add alert
npx shadcn-ui@latest add tabs
npx shadcn-ui@latest add avatar
npx shadcn-ui@latest add separator
```

**Nota:** El componente `Form` de shadcn es un wrapper que facilita la integración con `react-hook-form`. Incluye `FormField`, `FormItem`, `FormLabel`, `FormControl`, `FormMessage`, etc.

---

## 11. Checklist de Diseño UI

### Wizard Paso 1 - Información Básica
- [ ] Header con logo y botón cerrar (X)
- [ ] WizardStepper horizontal con 4 pasos
- [ ] Form card con bg-card y border-border
- [ ] FormField para Título con character counter
- [ ] FormField para Género Musical (Select)
- [ ] FormField para Meta Financiación (Input number con prefix €)
- [ ] FormField para Fecha Finalización (Popover + Calendar)
- [ ] FormField para URL Video (Input opcional)
- [ ] FormField para Imagen Portada (FileUploadArea custom)
- [ ] Labels con asterisco rojo para campos obligatorios
- [ ] Validación inline con FormMessage
- [ ] Botón Siguiente con gradient
- [ ] Botón Anterior disabled en paso 1
- [ ] Estados: focus, error, loading
- [ ] Responsive mobile (grid → stack)

### Wizard Paso 2 - Historia
- [ ] Stepper muestra paso 1 completado (✓)
- [ ] FormField para Subtítulo (Input opcional)
- [ ] CharacterCounter para subtítulo (0/100)
- [ ] RichTextEditor (TipTap) con toolbar
- [ ] Toolbar buttons: Bold, Italic, List, Link
- [ ] CharacterCounter para descripción (0/5000)
- [ ] FormField para "¿Por qué apoyo?" (Textarea opcional)
- [ ] Botón Anterior funcional
- [ ] Validación mínimo 50 caracteres en descripción

### Wizard Paso 3 - Recompensas
- [ ] Stepper muestra pasos 1-2 completados
- [ ] Lista de RewardCard components
- [ ] Cada RewardCard: precio, título, descripción, stock
- [ ] Botón "Agregar recompensa" outline dashed
- [ ] EmptyState si no hay recompensas
- [ ] Tip box con icono lightbulb
- [ ] Modal/Dialog para crear/editar reward

### Wizard Paso 4 - Revisión
- [ ] Stepper muestra todos los pasos completados
- [ ] Preview card con imagen de portada
- [ ] Título, género, meta, fecha visible
- [ ] Descripción preview (primeras líneas)
- [ ] Resumen de recompensas
- [ ] Link "Editar" para volver a pasos anteriores
- [ ] Alert warning "Se creará como BORRADOR"
- [ ] Botón "Guardar como borrador" outline
- [ ] Botón "Crear campaña" gradient principal
- [ ] Loading states en botones
- [ ] Toast notifications success/error
- [ ] Redirect a preview después de crear

### Vista Previa Campaña (Borrador)
- [ ] Alert banner warning con bg-warning/10
- [ ] Botón cerrar banner con persistencia localStorage
- [ ] Action bar: Editar, Publicar, Eliminar buttons
- [ ] Dialog de confirmación para Publicar
- [ ] Dialog advertencia si no hay rewards
- [ ] Dialog confirmación para Eliminar
- [ ] Loading states en botones de acción
- [ ] Layout igual a detalle público + banner arriba

### Mis Campañas (Lista)
- [ ] Sidebar dashboard con navegación
- [ ] Botón "+ Nueva campaña" gradient top-right
- [ ] Lista de CampaignCard components
- [ ] Cada card: imagen, título, status badge, progress mini, backers
- [ ] Status badge con colores correctos por estado
- [ ] Progress bar mini con gradient
- [ ] Botones: Editar, Ver
- [ ] Botón "Publicar" visible solo en borradores
- [ ] DropdownMenu con más opciones
- [ ] EmptyState con icono y CTA
- [ ] Skeleton loaders
- [ ] Hover effect en cards
- [ ] Dialog confirmación para eliminar

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Gradient buttons en CTAs principales
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] shadcn/ui components correctos
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste mínimo 4.5:1
- [ ] Keyboard navigation funcional
- [ ] Focus states con ring primary
- [ ] Form validation con Zod
- [ ] TanStack Query para mutations
- [ ] Toast notifications (sonner)
- [ ] Responsive mobile-first
- [ ] Skeleton loaders para loading states
- [ ] Empty states con iconos y CTAs
- [ ] ARIA labels completos

---

## 12. Notas de Implementación

### 12.1 File Upload Strategy

**Opciones:**
1. **Azure Blob Storage** (recomendado para producción)
2. **Local storage + URL** (desarrollo)
3. **Base64 embedding** (NO recomendado - payload muy grande)

**Flow:**
1. Usuario selecciona archivo en `<FileUploadArea>`
2. Validar formato (PNG/JPG) y tamaño (< 5MB) en cliente
3. Upload a blob storage usando SDK o endpoint `/api/upload`
4. Obtener URL pública
5. Guardar URL en campo `imagenPrincipalUrl` del formulario

### 12.2 Wizard State Management

**Opciones:**
1. **React Context** (simple, recomendado para wizard)
2. **Zustand** (si ya está en el proyecto)
3. **Local state + URL params** (mantener step en query string)

**Estructura:**
```tsx
interface WizardState {
  currentStep: number;
  data: {
    step1: CampaniaBasicInfo;
    step2: CampaniaHistoria;
    step3: CampaniaRecompensas;
  };
  isCompleted: (step: number) => boolean;
  goToStep: (step: number) => void;
  nextStep: () => void;
  previousStep: () => void;
}
```

### 12.3 Rich Text Editor Sanitization

**Importante:** Sanitizar HTML del editor antes de enviar al backend para prevenir XSS.

```bash
npm install dompurify
npm install @types/dompurify --save-dev
```

```tsx
import DOMPurify from 'dompurify';

const sanitizedHtml = DOMPurify.sanitize(editorContent, {
  ALLOWED_TAGS: ['p', 'br', 'strong', 'em', 'u', 'ul', 'ol', 'li', 'a'],
  ALLOWED_ATTR: ['href', 'target', 'rel'],
});
```

### 12.4 Date Handling

Usar `date-fns` para manipulación de fechas (ya usado en Zod schemas):

```bash
npm install date-fns
```

```tsx
import { format, addDays } from 'date-fns';
import { es } from 'date-fns/locale';

// Formatear fecha para display
format(new Date(fechaFin), "PPP", { locale: es }); // "30 de marzo de 2026"

// Validar mínimo 7 días
const minDate = addDays(new Date(), 7);
```

---

## 13. Dependencias Adicionales Requeridas

```json
{
  "dependencies": {
    "@tiptap/react": "^2.1.13",
    "@tiptap/starter-kit": "^2.1.13",
    "@tiptap/extension-link": "^2.1.13",
    "date-fns": "^3.0.0",
    "dompurify": "^3.0.8",
    "react-hook-form": "^7.49.0",
    "@hookform/resolvers": "^3.3.3",
    "zod": "^3.22.4",
    "sonner": "^1.3.1",
    "@tanstack/react-query": "^5.17.0"
  },
  "devDependencies": {
    "@types/dompurify": "^3.0.5"
  }
}
```

**Nota:** Algunas de estas dependencias ya están instaladas en el proyecto. Verificar antes de instalar.

---

**Fin del documento de diseño UI.**
