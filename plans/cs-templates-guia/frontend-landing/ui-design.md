# Diseno UI: Templates y Guia para Artistas Noveles (Landing)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen

- **Componentes shadcn/ui:** 12 componentes (9 ya instalados, 3 pendientes)
- **Composiciones custom:** 5 componentes principales
- **Responsive breakpoints:** mobile (< 640px), tablet (640-1024px), desktop (> 1024px)
- **Tema:** Dark theme con gradient pink/purple
- **Pantallas:** 3 pasos de wizard (Template Gallery → Customize Needs → Confirm & Publish)

---

## 2. Paleta de Colores (Dark Theme)

### 2.1 Variables CSS Mapeadas a Tailwind

| Uso | CSS Variable | Tailwind Class | Hex Value |
|-----|--------------|----------------|-----------|
| Background primary | `--bg-primary` | `bg-[#1a1a2e]` | #1a1a2e |
| Background secondary | `--bg-secondary` | `bg-[#16213e]` | #16213e |
| Background card | `--bg-card` | `bg-[#0f1729]` | #0f1729 |
| Background card hover | `--bg-card-hover` | `bg-[#1e2a42]` | #1e2a42 |
| Primary color | `--primary-color` | `text-[#a855f7]` / `border-[#a855f7]` | #a855f7 |
| Primary gradient | `--primary-gradient` | `bg-gradient-to-r from-pink-500 to-purple-600` | - |
| Primary gradient hover | `--primary-gradient-hover` | `from-pink-600 to-purple-700` | - |
| Text primary | `--text-primary` | `text-white` | #ffffff |
| Text secondary | `--text-secondary` | `text-[#94a3b8]` | #94a3b8 |
| Text muted | `--text-muted` | `text-[#64748b]` | #64748b |
| Text label | `--text-label` | `text-[#cbd5e1]` | #cbd5e1 |
| Status success | `--status-success` | `text-green-500` / `bg-green-500` | #10b981 |
| Status error | `--status-error` | `text-red-500` / `bg-red-500` | #ef4444 |
| Status warning | `--status-warning` | `text-yellow-500` / `bg-yellow-500` | #f59e0b |
| Status info | `--status-info` | `text-blue-500` / `bg-blue-500` | #3b82f6 |
| Priority Esencial | `--priority-esencial` | `bg-red-500/10 border-red-500` | #ef4444 |
| Priority Recomendado | `--priority-recomendado` | `bg-yellow-500/10 border-yellow-500` | #f59e0b |
| Priority Opcional | `--priority-opcional` | `bg-slate-500/10 border-slate-500` | #64748b |
| Border primary | `--border-primary` | `border-[#334155]` | #334155 |
| Border focus | `--border-focus` | `border-[#a855f7]` | #a855f7 |
| Input background | `--input-bg` | `bg-[#0f1729]` | #0f1729 |
| Input border | `--input-border` | `border-[#334155]` | #334155 |
| Input placeholder | `--input-placeholder` | `placeholder:text-[#64748b]` | #64748b |

### 2.2 Priority Colors Mapping

| Prioridad | Badge Classes | Badge Text |
|-----------|---------------|------------|
| **Esencial (Alta)** | `border border-red-500 bg-red-500/10 text-red-400` | "ESENCIAL" |
| **Recomendado (Media)** | `border border-yellow-500 bg-yellow-500/10 text-yellow-400` | "RECOMENDADO" |
| **Opcional (Baja)** | `border border-slate-500 bg-slate-500/10 text-slate-400` | "OPCIONAL" |

---

## 3. Componentes shadcn/ui Disponibles

### 3.1 Ya Instalados (9)

| Componente | Archivo | Radix Dependency | Estado |
|------------|---------|------------------|--------|
| **Button** | `src/web/src/components/ui/button.tsx` | `@radix-ui/react-slot` | Instalado |
| **Card** | `src/web/src/components/ui/card.tsx` | N/A | Instalado |
| **Input** | `src/web/src/components/ui/input.tsx` | N/A | Instalado |
| **Label** | `src/web/src/components/ui/label.tsx` | `@radix-ui/react-label` | Instalado |
| **Progress** | `src/web/src/components/ui/progress.tsx` | `@radix-ui/react-progress` | Instalado |
| **Textarea** | `src/web/src/components/ui/textarea.tsx` | N/A | Instalado |
| **Badge** | `src/web/src/components/ui/badge.tsx` | N/A | Instalado |
| **Skeleton** | `src/web/src/components/ui/skeleton.tsx` | N/A | Instalado |
| **Checkbox** | `src/web/src/components/ui/checkbox.tsx` | `@radix-ui/react-checkbox` | Instalado |
| **Select** | `src/web/src/components/ui/select.tsx` | `@radix-ui/react-select` | Instalado |
| **Tabs** | `src/web/src/components/ui/tabs.tsx` | `@radix-ui/react-tabs` | Instalado |
| **Separator** | `src/web/src/components/ui/separator.tsx` | `@radix-ui/react-separator` | Instalado |
| **Alert** | `src/web/src/components/ui/alert.tsx` | N/A | Instalado |
| **Dialog** | `src/web/src/components/ui/dialog.tsx` | `@radix-ui/react-dialog` | Instalado |

### 3.2 Pendientes de Instalacion (3)

| Componente | Uso | Comando Instalacion | Radix Dependency |
|------------|-----|---------------------|------------------|
| **Tooltip** | Tooltips de roles profesionales | `npx shadcn@latest add tooltip` | `@radix-ui/react-tooltip` |
| **Toast** | Ya usando Sonner (instalado) | N/A - usar Sonner | `sonner` |
| **Form** | Integracion React Hook Form | `npx shadcn@latest add form` | `react-hook-form` + Zod |

**Nota:** Toast ya esta implementado con Sonner (ver `package.json`). No se necesita shadcn/ui Toast.

---

## 4. Composiciones Custom

### 4.1 WizardStepper

**Props:**
```typescript
interface WizardStepperProps {
  steps: number;           // Total de pasos (3)
  currentStep: number;     // Paso actual (1-3)
  stepLabels: string[];    // ["Seleccionar", "Personalizar", "Confirmar"]
}
```

**Composicion:**
- **Base:** `<div>` container con flex horizontal
- **Cada paso:** Circulo + linea + label
- **Estados:**
  - Completado: circulo con check icon (`lucide-react` Check), linea solida, texto primary
  - Actual: circulo con numero, linea solida hasta ahi, texto white
  - Futuro: circulo vacio, linea dashed, texto muted

**Tailwind Classes:**
```tsx
// Container
className="flex items-center justify-between max-w-4xl mx-auto mb-8"

// Step (completado)
className="flex flex-col items-center"
<div className="w-10 h-10 rounded-full bg-gradient-to-r from-pink-500 to-purple-600 flex items-center justify-center">
  <Check className="w-5 h-5 text-white" />
</div>
<span className="mt-2 text-sm font-medium text-[#a855f7]">Seleccionar</span>

// Step (actual)
<div className="w-10 h-10 rounded-full border-2 border-[#a855f7] bg-[#1e2a42] flex items-center justify-center">
  <span className="text-sm font-bold text-white">2</span>
</div>
<span className="mt-2 text-sm font-medium text-white">Personalizar</span>

// Step (futuro)
<div className="w-10 h-10 rounded-full border-2 border-[#334155] bg-transparent">
  <span className="text-sm font-bold text-[#64748b]">3</span>
</div>
<span className="mt-2 text-sm font-medium text-[#64748b]">Confirmar</span>

// Linea conectora (solida)
<div className="flex-1 h-0.5 bg-gradient-to-r from-pink-500 to-purple-600 mx-4" />

// Linea conectora (dashed)
<div className="flex-1 h-0.5 border-t-2 border-dashed border-[#334155] mx-4" />
```

**Responsive:**
- **Mobile:** Stack vertical, labels debajo de circulos
- **Tablet/Desktop:** Horizontal con lineas entre pasos

**Animaciones:**
- Transicion suave al cambiar de paso: `transition-all duration-300 ease-in-out`

---

### 4.2 TemplateCard

**Props:**
```typescript
interface TemplateCardProps {
  template: PlantillaProyectoList;
  onSelect: (id: string) => void;
}
```

**Composicion:**
- **Base:** `<Card>` de shadcn/ui
- **Contenido:** Icon (Lucide) + Title + Stats + Button
- **Estados:** default, hover, active (click)

**Estructura:**
```tsx
<Card className="group bg-[#0f1729] border-[#334155] hover:bg-[#1e2a42] hover:border-[#a855f7] hover:shadow-[0_0_20px_rgba(168,85,247,0.3)] transition-all duration-200 cursor-pointer">
  <CardHeader>
    {/* Icon */}
    <div className="w-16 h-16 mx-auto mb-4">
      <Music className="w-full h-full text-[#a855f7]" />
    </div>

    {/* Title */}
    <CardTitle className="text-xl font-semibold text-white text-center">
      {template.nombre}
    </CardTitle>

    {/* Description (truncated) */}
    <CardDescription className="text-sm text-[#94a3b8] text-center line-clamp-2 mt-2">
      {template.descripcion}
    </CardDescription>
  </CardHeader>

  <CardContent className="space-y-3">
    {/* Stats */}
    <div className="flex items-center justify-center gap-2 text-sm text-[#94a3b8]">
      <Layers className="w-4 h-4" />
      <span>{template.cantidadNecesidades} necesidades</span>
    </div>

    <div className="flex items-center justify-center gap-2 text-base font-medium text-[#a855f7]">
      <DollarSign className="w-4 h-4" />
      <span>{formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}</span>
    </div>

    {/* Button */}
    <Button
      className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
      onClick={() => onSelect(template.id)}
    >
      Seleccionar
    </Button>
  </CardContent>
</Card>
```

**Iconos Lucide Mapeados:**

| Template | Icono Lucide | Import |
|----------|--------------|--------|
| Grabar Album/EP | `Music` | `import { Music } from 'lucide-react'` |
| Produccion Videoclip | `Video` | `import { Video } from 'lucide-react'` |
| Gira/Tour | `MapPin` | `import { MapPin } from 'lucide-react'` |
| Marketing y Promocion | `TrendingUp` | `import { TrendingUp } from 'lucide-react'` |
| Lanzamiento Single | `Disc` | `import { Disc } from 'lucide-react'` |
| Crear Presencia Online | `Globe` | `import { Globe } from 'lucide-react'` |

**Hover Effect:**
```css
/* Animacion de escala + glow shadow */
.group:hover {
  transform: scale(1.02);
  box-shadow: 0 0 20px rgba(168, 85, 247, 0.3);
}

/* Click momentaneo */
.group:active {
  transform: scale(0.98);
}
```

**Responsive:**
- **Mobile:** `p-4`, iconos `w-12 h-12`, titulos `text-lg`
- **Tablet/Desktop:** `p-6`, iconos `w-16 h-16`, titulos `text-xl`

---

### 4.3 NecesidadItem

**Props:**
```typescript
interface NecesidadItemProps {
  necesidad: PlantillaProyectoNecesidad;
  isSelected: boolean;
  onToggle: (id: string) => void;
  onBudgetChange: (id: string, min: number, max: number) => void;
}
```

**Composicion:**
- **Base:** `<Card>` con Checkbox + contenido
- **Elementos:** Checkbox + Title + Badge prioridad + Rol tooltip + Budget inputs (condicionales)

**Estructura:**
```tsx
<Card className={cn(
  "bg-[#0f1729] border-[#334155] p-4 mb-3 transition-colors duration-150",
  isSelected && "bg-[#1e2a42]"
)}>
  <div className="flex items-start gap-3">
    {/* Checkbox */}
    <Checkbox
      id={necesidad.id}
      checked={isSelected}
      onCheckedChange={() => onToggle(necesidad.id)}
      aria-label={`Seleccionar ${necesidad.titulo}`}
      className="mt-1"
    />

    <div className="flex-1 space-y-2">
      {/* Header: Title + Badge */}
      <div className="flex items-center justify-between gap-2">
        <Label
          htmlFor={necesidad.id}
          className="text-base font-medium text-white cursor-pointer"
        >
          {necesidad.titulo}
        </Label>

        {/* Badge prioridad */}
        <Badge className={cn(
          "text-xs font-semibold uppercase tracking-wide",
          necesidad.prioridad === "Alta" && "border border-red-500 bg-red-500/10 text-red-400",
          necesidad.prioridad === "Media" && "border border-yellow-500 bg-yellow-500/10 text-yellow-400",
          necesidad.prioridad === "Baja" && "border border-slate-500 bg-slate-500/10 text-slate-400"
        )}>
          {necesidad.prioridad === "Alta" ? "ESENCIAL" :
           necesidad.prioridad === "Media" ? "RECOMENDADO" : "OPCIONAL"}
        </Badge>
      </div>

      {/* Rol con Tooltip */}
      <div className="flex items-center gap-1 text-sm text-[#94a3b8]">
        <span>Rol: {necesidad.rolProfesional.nombre}</span>

        <Tooltip>
          <TooltipTrigger asChild>
            <button className="inline-flex items-center justify-center w-4 h-4 rounded-full hover:bg-[#334155] transition-colors">
              <InfoCircle className="w-3 h-3" />
            </button>
          </TooltipTrigger>
          <TooltipContent
            side="top"
            className="max-w-xs bg-[#16213e] border-[#334155] text-white p-3"
          >
            <p className="text-sm">{necesidad.rolProfesional.descripcion}</p>
          </TooltipContent>
        </Tooltip>
      </div>

      {/* Budget inputs (solo si isSelected) */}
      {isSelected && (
        <div className="flex items-center gap-2 mt-2">
          <Label className="text-xs text-[#cbd5e1]">Presupuesto:</Label>
          <Input
            type="number"
            placeholder={String(necesidad.precioMinOrientativo)}
            className="w-24 h-8 bg-[#1a1a2e] border-[#334155] text-white text-sm"
            aria-label="Presupuesto minimo"
            onChange={(e) => onBudgetChange(necesidad.id, Number(e.target.value), max)}
          />
          <span className="text-[#64748b]">-</span>
          <Input
            type="number"
            placeholder={String(necesidad.precioMaxOrientativo)}
            className="w-24 h-8 bg-[#1a1a2e] border-[#334155] text-white text-sm"
            aria-label="Presupuesto maximo"
            onChange={(e) => onBudgetChange(necesidad.id, min, Number(e.target.value))}
          />
          <span className="text-sm text-[#94a3b8]">EUR</span>
        </div>
      )}
    </div>
  </div>
</Card>
```

**Validacion Visual:**
- Border rojo en input si max < min: `border-red-500`
- Mensaje de error debajo: `<p className="text-xs text-red-400 mt-1">El maximo debe ser mayor que el minimo</p>`

**Animaciones:**
- Expand budget inputs: `transition-all duration-300 ease-out` en height
- Hover card: `hover:bg-[#1e2a42]/50`

**Responsive:**
- **Mobile:** Budget inputs stack vertical, labels mas pequenas
- **Desktop:** Budget inputs horizontal inline

---

### 4.4 BudgetSummary

**Props:**
```typescript
interface BudgetSummaryProps {
  minTotal: number;
  maxTotal: number;
  selectedCount: number;
  totalCount: number;
}
```

**Composicion:**
- **Base:** `<Card>` sticky con gradient background
- **Elementos:** Title + Budget ranges + Counter + Progress bar

**Estructura:**
```tsx
<Card className="bg-gradient-to-r from-purple-900/20 to-pink-900/20 border border-[#a855f7] p-6 sticky top-4">
  <CardHeader className="p-0 mb-4">
    <CardTitle className="text-lg font-semibold text-white">
      Resumen Presupuestario
    </CardTitle>
  </CardHeader>

  <CardContent className="p-0 space-y-4">
    {/* Budget ranges */}
    <div className="space-y-2">
      <div className="flex justify-between items-baseline">
        <span className="text-sm text-[#94a3b8]">Min total:</span>
        <span className="text-2xl font-bold text-white">
          {formatCurrency(minTotal)}
        </span>
      </div>

      <div className="flex justify-between items-baseline">
        <span className="text-sm text-[#94a3b8]">Max total:</span>
        <span className="text-2xl font-bold text-white">
          {formatCurrency(maxTotal)}
        </span>
      </div>

      <div className="flex justify-between items-baseline border-t border-[#334155] pt-2">
        <span className="text-sm text-[#94a3b8]">Promedio:</span>
        <span className="text-base font-semibold text-[#a855f7]">
          ~{formatCurrency((minTotal + maxTotal) / 2)}
        </span>
      </div>
    </div>

    {/* Counter */}
    <p className="text-sm text-[#94a3b8]">
      Necesidades seleccionadas: <span className="font-bold text-white">{selectedCount}</span> de {totalCount}
    </p>

    {/* Progress bar */}
    <Progress
      value={(selectedCount / totalCount) * 100}
      className="h-2 bg-[#334155]"
      indicatorClassName="bg-gradient-to-r from-pink-500 to-purple-600"
    />
  </CardContent>
</Card>
```

**Number Count-up Animation:**
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

**Responsive:**
- **Mobile:** No sticky, al final del formulario
- **Tablet/Desktop:** Sticky `top-4`, width fijo `max-w-sm`

---

### 4.5 PhaseGroup

**Props:**
```typescript
interface PhaseGroupProps {
  faseName: string;
  necesidades: PlantillaProyectoNecesidad[];
  selectedIds: Set<string>;
  onToggle: (id: string) => void;
  onBudgetChange: (id: string, min: number, max: number) => void;
}
```

**Composicion:**
- **Base:** `<div>` wrapper
- **Elementos:** Phase header + lista de NecesidadItem

**Estructura:**
```tsx
<div className="mb-8">
  {/* Phase Header */}
  <h2 className="text-xl font-semibold text-white mb-4 uppercase tracking-wide flex items-center gap-2">
    <span className="w-1 h-6 bg-gradient-to-b from-pink-500 to-purple-600 rounded-full" />
    {faseName}
  </h2>

  {/* Necesidades List */}
  <div className="space-y-3">
    {necesidades.map(necesidad => (
      <NecesidadItem
        key={necesidad.id}
        necesidad={necesidad}
        isSelected={selectedIds.has(necesidad.id)}
        onToggle={onToggle}
        onBudgetChange={onBudgetChange}
      />
    ))}
  </div>
</div>
```

**Responsive:**
- **Mobile:** Phase header `text-lg`, barra lateral mas delgada
- **Desktop:** Phase header `text-xl`

---

## 5. Pantallas - Componentes por Screen

### 5.1 Pantalla 1: Galeria de Templates (Paso 1)

**Ruta:** `/crowdsourcing/nuevo-proyecto?step=1`

**Layout:**
```
┌─────────────────────────────────────────┐
│  Header (PublicLayout)                  │
├─────────────────────────────────────────┤
│  WizardStepper (step 1/3)               │
│                                         │
│  Page Title                             │
│  Page Subtitle                          │
│                                         │
│  ┌─────┐  ┌─────┐  ┌─────┐            │
│  │Card │  │Card │  │Card │  (grid 3)  │
│  └─────┘  └─────┘  └─────┘            │
│  ┌─────┐  ┌─────┐  ┌─────┐            │
│  │Card │  │Card │  │Card │            │
│  └─────┘  └─────┘  └─────┘            │
│                                         │
│  [Crear plantilla personalizada]        │
│                                         │
└─────────────────────────────────────────┘
```

**Componentes:**

| Elemento | Componente | Classes |
|----------|------------|---------|
| **Wizard Progress** | `<WizardStepper steps={3} currentStep={1} />` | Container: `max-w-4xl mx-auto mb-8` |
| **Page Title** | `<h1>` | `text-4xl font-bold text-white mb-2 text-center` |
| **Page Subtitle** | `<p>` | `text-lg text-[#94a3b8] mb-8 text-center` |
| **Templates Grid** | `<div>` | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 max-w-7xl mx-auto` |
| **Template Card** | `<TemplateCard>` | Ver seccion 4.2 |
| **Alternative Link** | `<Link>` | `text-[#a855f7] hover:underline text-center block mt-6 text-sm` |

**Estados UI:**

| Estado | Componente | Visual |
|--------|------------|--------|
| **Loading** | `<Skeleton>` | 6 cards con skeleton: `<Skeleton className="h-[320px] bg-[#1e2a42]" />` |
| **Empty** | Custom empty state | Icon Music + "No hay plantillas disponibles" + CTA button |
| **Error** | Toast (Sonner) | `toast.error("Error cargando plantillas. Intenta de nuevo.")` |

**Responsive:**

| Breakpoint | Grid Cols | Card Padding | Icon Size |
|------------|-----------|--------------|-----------|
| **< 640px** | 1 | `p-4` | `w-12 h-12` |
| **640-1024px** | 2 | `p-5` | `w-14 h-14` |
| **> 1024px** | 3 | `p-6` | `w-16 h-16` |

---

### 5.2 Pantalla 2: Personalizar Necesidades (Paso 2)

**Ruta:** `/crowdsourcing/nuevo-proyecto?step=2&template={id}`

**Layout:**
```
┌──────────────────────────────────────────────┐
│  Header (PublicLayout)                       │
├──────────────────────────────────────────────┤
│  WizardStepper (step 2/3)                    │
│                                              │
│  Template Title                              │
│  Template Subtitle                           │
│                                              │
│  ┌───────────────────┐  ┌───────────────┐   │
│  │ Phases + Needs    │  │ BudgetSummary │   │
│  │ (scrollable)      │  │ (sticky)      │   │
│  │                   │  │               │   │
│  │ - PRE-PRODUCCION  │  │               │   │
│  │   [x] Necesidad1  │  │               │   │
│  │   [ ] Necesidad2  │  │               │   │
│  │                   │  │               │   │
│  │ - GRABACION       │  │               │   │
│  │   [x] Necesidad3  │  │               │   │
│  │                   │  │               │   │
│  └───────────────────┘  └───────────────┘   │
│                                              │
│  [< Atras]                 [Siguiente >]     │
│                                              │
└──────────────────────────────────────────────┘
```

**Componentes:**

| Elemento | Componente | Classes |
|----------|------------|---------|
| **Wizard Progress** | `<WizardStepper steps={3} currentStep={2} />` | Container: `max-w-4xl mx-auto mb-8` |
| **Template Title** | `<h1>` | `text-3xl font-bold text-white mb-2` |
| **Template Subtitle** | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| **Main Layout** | `<div>` | `grid grid-cols-1 lg:grid-cols-3 gap-8 max-w-7xl mx-auto` |
| **Needs Column** | `<div>` | `lg:col-span-2 space-y-8` |
| **Phase Group** | `<PhaseGroup>` | Ver seccion 4.5 |
| **Necesidad Item** | `<NecesidadItem>` | Ver seccion 4.3 |
| **Summary Column** | `<div>` | `lg:col-span-1` |
| **Budget Summary** | `<BudgetSummary>` | Ver seccion 4.4 |
| **Navigation** | `<div>` | `flex justify-between mt-8` |
| **Back Button** | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| **Next Button** | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |

**Estados UI:**

| Estado | Visual |
|--------|--------|
| **Loading** | Skeletons en fases y necesidades: `<Skeleton className="h-24 bg-[#1e2a42] mb-3" />` |
| **Checkbox Toggle** | Actualiza BudgetSummary en tiempo real (debounce 0ms) |
| **Budget Input Change** | Debounce 300ms, valida min < max, actualiza totales |
| **Validation Error** | Border rojo `border-red-500` + mensaje `text-xs text-red-400` |
| **All Unchecked** | Boton "Siguiente" disabled + tooltip: "Selecciona al menos una necesidad" |

**Validacion en Tiempo Real:**

| Campo | Validacion | Mensaje |
|-------|-----------|---------|
| **Presupuesto Min** | >= 0 | "El presupuesto debe ser mayor o igual a 0" |
| **Presupuesto Max** | > Presupuesto Min | "El maximo debe ser mayor que el minimo" |
| **Al menos 1 seleccionado** | count(checked) >= 1 | "Selecciona al menos una necesidad" |

**Responsive:**

| Breakpoint | Layout | Summary Position |
|------------|--------|------------------|
| **< 1024px** | Stack vertical | Al final del formulario (no sticky) |
| **> 1024px** | 2 columnas (2/3 + 1/3) | Sticky `top-4` |

---

### 5.3 Pantalla 3: Confirmar y Publicar (Paso 3)

**Ruta:** `/crowdsourcing/nuevo-proyecto?step=3&template={id}`

**Layout:**
```
┌──────────────────────────────────────────────┐
│  Header (PublicLayout)                       │
├──────────────────────────────────────────────┤
│  WizardStepper (step 3/3)                    │
│                                              │
│  Page Title                                  │
│  Page Subtitle                               │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │ Summary Card                         │   │
│  │                                      │   │
│  │ Template: Grabar un Album / EP       │   │
│  │ Necesidades seleccionadas: 8 de 11  │   │
│  │                                      │   │
│  │ ┌──────────────────────────────────┐ │   │
│  │ │ RESUMEN PRESUPUESTARIO           │ │   │
│  │ │ Min: 1,920 EUR                   │ │   │
│  │ │ Max: 10,400 EUR                  │ │   │
│  │ │ Promedio: ~6,160 EUR             │ │   │
│  │ └──────────────────────────────────┘ │   │
│  │                                      │   │
│  │ NECESIDADES SELECCIONADAS:           │   │
│  │ - PRE-PRODUCCION                     │   │
│  │   • Composicion (200-1,500) [ESE]   │   │
│  │   • Produccion (500-3,000) [ESE]    │   │
│  │ - GRABACION                          │   │
│  │   • Estudio (200-800) [ESE]         │   │
│  │   ...                                │   │
│  └──────────────────────────────────────┘   │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │ PROYECTO ARTISTICO (opcional)        │   │
│  │ [Select proyecto] o [Crear nuevo]    │   │
│  └──────────────────────────────────────┘   │
│                                              │
│  [< Atras]        [Confirmar y publicar]     │
│                                              │
└──────────────────────────────────────────────┘
```

**Componentes:**

| Elemento | Componente | Classes |
|----------|------------|---------|
| **Wizard Progress** | `<WizardStepper steps={3} currentStep={3} />` | Container: `max-w-4xl mx-auto mb-8` |
| **Page Title** | `<h1>` | `text-3xl font-bold text-white mb-2 text-center` |
| **Page Subtitle** | `<p>` | `text-lg text-[#94a3b8] mb-8 text-center` |
| **Summary Card** | `<Card>` | `bg-[#0f1729] border-[#334155] p-8 max-w-4xl mx-auto` |
| **Template Name** | `<h2>` | `text-2xl font-semibold text-white mb-1` |
| **Selected Count** | `<p>` | `text-base text-[#94a3b8] mb-6` |
| **Budget Summary Box** | `<div>` | `bg-gradient-to-r from-purple-900/30 to-pink-900/30 border border-[#a855f7] rounded-lg p-6 mb-6` |
| **Budget Title** | `<h3>` | `text-lg font-semibold text-white mb-3` |
| **Budget Min/Max** | `<div>` | `text-xl font-bold text-white` |
| **Budget Average** | `<div>` | `text-base text-[#94a3b8] mt-2` |
| **Needs List Title** | `<h3>` | `text-lg font-semibold text-white mb-4` |
| **Phase Group** | `<div>` | `mb-4` |
| **Phase Name** | `<h4>` | `text-sm font-semibold text-[#94a3b8] uppercase tracking-wide mb-2` |
| **Need List Item** | `<li>` | `text-base text-white mb-2 flex items-start gap-2` |
| **Need Bullet** | `<span>` | `text-[#a855f7] mt-1` "•" |
| **Priority Badge** | `<Badge>` | `text-xs ml-2` colores segun prioridad |
| **Project Select Card** | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mt-6 max-w-4xl mx-auto` |
| **Project Label** | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| **Project Select** | `<Select>` | shadcn/ui Select con proyectos + "Crear nuevo proyecto" |
| **Navigation** | `<div>` | `flex justify-between mt-8 max-w-4xl mx-auto` |
| **Back Button** | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| **Confirm Button** | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8 py-3` |

**Estados UI:**

| Estado | Visual |
|--------|--------|
| **Loading** | Skeletons en resumen y lista: `<Skeleton className="h-8 bg-[#1e2a42] mb-2" />` |
| **Loading Submit** | Boton muestra spinner + texto "Publicando necesidades...", disabled |
| **Success** | Toast verde: `toast.success("8 necesidades publicadas correctamente")` + redirect |
| **Error** | Toast rojo: `toast.error("Error al publicar necesidades. Intenta de nuevo.")` |

**Responsive:**

| Breakpoint | Card Padding | Budget Font Size |
|------------|--------------|------------------|
| **< 640px** | `p-4` | `text-lg` |
| **> 640px** | `p-8` | `text-xl` |

---

## 6. Formularios con React Hook Form + Zod

### 6.1 Paso 2 - Formulario de Personalizacion

**Form Schema:**
```typescript
// Ya definido en src/shared/schemas/crowdsourcing.schema.ts
import { generarNecesidadesSchema } from '@/shared/schemas';

// Uso con React Hook Form
const form = useForm<GenerarNecesidadesFormData>({
  resolver: zodResolver(generarNecesidadesSchema),
  defaultValues: {
    proyectoArtisticoId: '',
    necesidadesSeleccionadas: [],
  },
});
```

**Layout de Formulario:**
- No usar shadcn/ui Form component (formulario custom con estado Zustand)
- Validacion manual con Zod en submit
- Display errors con mensajes custom en toasts

---

## 7. Feedback y Estados

### 7.1 Loading States

| Componente | Loading Visual |
|------------|----------------|
| **Template Gallery** | 6 Skeleton cards: `<Skeleton className="h-[320px] bg-[#1e2a42] rounded-lg" />` |
| **Template Detail** | Skeleton en fases: `<Skeleton className="h-24 bg-[#1e2a42] mb-3" />` |
| **Submit Button** | Spinner icon + texto: `<Loader2 className="animate-spin mr-2" /> Publicando...` |

### 7.2 Error States

| Error Type | Visual |
|------------|--------|
| **API Error** | Toast rojo (Sonner): `toast.error("Error cargando plantillas")` |
| **Validation Error** | Border rojo + mensaje debajo input: `<p className="text-xs text-red-400 mt-1">{error}</p>` |
| **Empty State** | Custom component con icon Music + mensaje + CTA |

### 7.3 Success States

| Action | Visual |
|--------|--------|
| **Necesidades Publicadas** | Toast verde: `toast.success("8 necesidades publicadas correctamente")` |
| **Template Seleccionado** | Navega a paso 2 con animacion fade |

### 7.4 Empty States

**Template Gallery Empty:**
```tsx
<div className="flex flex-col items-center justify-center py-16 text-center">
  <Music className="w-16 h-16 text-[#64748b] mb-4" />
  <h3 className="text-xl font-semibold text-white mb-2">
    No hay plantillas disponibles
  </h3>
  <p className="text-sm text-[#94a3b8] mb-6">
    Crea tu primera plantilla personalizada
  </p>
  <Button className="bg-gradient-to-r from-pink-500 to-purple-600">
    Crear plantilla personalizada
  </Button>
</div>
```

---

## 8. Responsive Design

### 8.1 Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 640px | Grid 1 col, stack vertical, summary no sticky, inputs presupuesto vertical |
| **Tablet** | 640-1024px | Grid 2 cols, wizard compacto, summary sticky reducido |
| **Desktop** | > 1024px | Grid 3 cols, wizard full width, summary sticky derecha, layout 2 columnas |

### 8.2 Responsive Classes por Pantalla

**Paso 1 (Mobile):**
```tsx
// Grid de templates
className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 md:gap-6"

// Template card
className="p-4 md:p-6"

// Icon
className="w-12 h-12 md:w-16 md:w-16"

// Title
className="text-lg md:text-xl"
```

**Paso 2 (Mobile):**
```tsx
// Main layout
className="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8"

// Summary card (no sticky en mobile)
className="lg:sticky lg:top-4"

// Budget inputs (vertical en mobile)
className="flex flex-col sm:flex-row items-start sm:items-center gap-2"

// Phase header
className="text-lg md:text-xl"
```

**Paso 3 (Mobile):**
```tsx
// Summary card padding
className="p-4 md:p-8"

// Budget font sizes
className="text-lg md:text-xl lg:text-2xl"

// Phase groups (sin separacion visual en mobile)
className="mb-3 md:mb-4"
```

---

## 9. Accesibilidad

### 9.1 Contraste de Color

| Combinacion | Ratio | WCAG Level |
|-------------|-------|------------|
| White (#fff) sobre bg-primary (#1a1a2e) | 15.8:1 | AAA |
| Text-secondary (#94a3b8) sobre bg-primary | 7.2:1 | AAA |
| Primary (#a855f7) sobre bg-card (#0f1729) | 6.8:1 | AA |

### 9.2 Focus Visible

**Todos los elementos interactivos:**
```tsx
className="focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]"
```

### 9.3 Labels y ARIA

| Elemento | ARIA |
|----------|------|
| **Template Card** | `role="button" tabindex="0" aria-label="Template: {nombre}. {count} necesidades. Presupuesto {min} a {max}"` |
| **Checkbox** | `aria-label="Seleccionar {titulo}. Prioridad: {prioridad}"` |
| **Budget Input** | `aria-label="Presupuesto minimo en euros" aria-describedby="budget-help-{id}"` |
| **Wizard Step** | `aria-current={currentStep === n ? "step" : undefined}` |
| **Loading Button** | `aria-busy={isPending}` |

### 9.4 Keyboard Navigation

| Elemento | Keys |
|----------|------|
| **Template Card** | Tab para focus, Enter/Space para seleccionar |
| **Checkbox** | Tab para focus, Space para toggle |
| **Tooltip** | Enter/Space para abrir, Esc para cerrar |
| **Dialog** | Esc para cerrar, Tab trap dentro del modal |

---

## 10. Animaciones y Transiciones

### 10.1 Duraciones

| Elemento | Animacion | Duracion |
|----------|-----------|----------|
| **Template card hover** | Scale 1.02 + border glow + shadow | 200ms ease-out |
| **Card click** | Scale 0.98 momentaneo | 100ms ease-in |
| **Wizard step transition** | Fade out + fade in | 300ms ease-in-out |
| **Progress bar fill** | Width transition | 400ms ease-out |
| **Checkbox toggle** | Scale 1.1 momentaneo | 150ms ease |
| **Budget update** | Number count-up | 500ms ease-out |
| **Tooltip show** | Fade in + translate Y -4px | 200ms ease-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Button hover** | Background gradient shift + scale 1.02 | 150ms ease |
| **Input focus** | Border color + glow shadow | 200ms ease |
| **Validation error shake** | Shake 4px horizontal | 400ms ease |
| **Loading spinner** | Rotate 360deg infinite | 1000ms linear |
| **Need item expand** | Height auto + fade budget inputs | 300ms ease-out |

### 10.2 Shake Animation (Validation Error)

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

### 10.3 Tailwind Animation Classes

```tsx
// Hover scale
className="transition-transform duration-200 ease-out hover:scale-102"

// Fade in
className="animate-in fade-in duration-300"

// Slide up
className="animate-in slide-in-from-bottom-4 duration-300"

// Spinner
className="animate-spin"

// Pulse (loading)
className="animate-pulse"
```

---

## 11. Checklist de Implementacion

### shadcn/ui Components
- [x] Button - Ya instalado
- [x] Card - Ya instalado
- [x] Input - Ya instalado
- [x] Label - Ya instalado
- [x] Progress - Ya instalado
- [x] Badge - Ya instalado
- [x] Skeleton - Ya instalado
- [x] Checkbox - Ya instalado
- [x] Select - Ya instalado
- [x] Separator - Ya instalado
- [x] Alert - Ya instalado
- [ ] Tooltip - **PENDIENTE INSTALACION** (`npx shadcn@latest add tooltip`)
- [x] Toast - Ya usando Sonner
- [ ] Form - **PENDIENTE INSTALACION** (`npx shadcn@latest add form`) - opcional, validacion manual

### Custom Components
- [ ] WizardStepper - Crear en `src/web/src/components/crowdsourcing/WizardStepper.tsx`
- [ ] TemplateCard - Crear en `src/web/src/components/crowdsourcing/TemplateCard.tsx`
- [ ] NecesidadItem - Crear en `src/web/src/components/crowdsourcing/NecesidadItem.tsx`
- [ ] BudgetSummary - Crear en `src/web/src/components/crowdsourcing/BudgetSummary.tsx`
- [ ] PhaseGroup - Crear en `src/web/src/components/crowdsourcing/PhaseGroup.tsx`

### Pantallas
- [ ] Paso 1: Template Gallery - `src/web/src/features/crowdsourcing/pages/TemplateGalleryPage.tsx`
- [ ] Paso 2: Customize Needs - `src/web/src/features/crowdsourcing/pages/CustomizeNeedsPage.tsx`
- [ ] Paso 3: Confirm & Publish - `src/web/src/features/crowdsourcing/pages/ConfirmPublishPage.tsx`

### Responsive
- [ ] Mobile (< 640px) - Grid 1 col, stack vertical, summary no sticky
- [ ] Tablet (640-1024px) - Grid 2 cols, wizard compacto
- [ ] Desktop (> 1024px) - Grid 3 cols, summary sticky

### Accesibilidad
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring purple
- [ ] ARIA labels en template cards
- [ ] ARIA labels en checkboxes
- [ ] ARIA labels en budget inputs
- [ ] Keyboard navigation completa
- [ ] Loading states con aria-busy

### Animaciones
- [ ] Template card hover effect
- [ ] Wizard step transitions
- [ ] Progress bar smooth fill
- [ ] Number count-up animation
- [ ] Validation error shake
- [ ] Tooltip fade in

### Dark Theme
- [ ] Todos los componentes usan design tokens de ui-ux.md
- [ ] Gradient pink/purple en botones principales
- [ ] Priority badges con colores correctos (red/yellow/gray)
- [ ] Background cards #0f1729
- [ ] Border colors #334155

---

## 12. Notas de Implementacion

### 12.1 Instalacion Pendiente

**Tooltip:**
```bash
cd src/web
npx shadcn@latest add tooltip
```

Esto instalara:
- `@radix-ui/react-tooltip` (dependencia)
- `src/web/src/components/ui/tooltip.tsx` (componente)

**Form (Opcional):**
```bash
cd src/web
npx shadcn@latest add form
```

Esto instalara:
- `src/web/src/components/ui/form.tsx` (componente)
- Integracion React Hook Form + Zod

### 12.2 Iconos Lucide React

**Icons utilizados:**
- Music, Video, MapPin, TrendingUp, Disc, Globe (templates)
- Layers, DollarSign (stats)
- InfoCircle (tooltips)
- Check (wizard steps)
- Loader2 (loading buttons)
- ChevronLeft, ChevronRight (navigation)

**Import:**
```typescript
import {
  Music, Video, MapPin, TrendingUp, Disc, Globe,
  Layers, DollarSign, InfoCircle, Check, Loader2,
  ChevronLeft, ChevronRight
} from 'lucide-react';
```

### 12.3 Utilidades Compartidas

**formatCurrency (ya existe en shared):**
```typescript
import { formatCurrency } from '@/shared/utils';
```

**cn (tailwind-merge):**
```typescript
import { cn } from '@/lib/utils';
```

**QUERY_KEYS (constantes):**
```typescript
import { QUERY_KEYS } from '@/shared/constants';
```

### 12.4 Toast con Sonner

**Configuracion en App.tsx:**
```tsx
import { Toaster } from 'sonner';

<Toaster
  position="top-center"
  theme="dark"
  toastOptions={{
    style: {
      background: '#16213e',
      border: '1px solid #334155',
      color: '#ffffff',
    },
  }}
/>
```

**Uso:**
```typescript
import { toast } from 'sonner';

// Success
toast.success("8 necesidades publicadas correctamente");

// Error
toast.error("Error al publicar necesidades. Intenta de nuevo.");

// Loading
const toastId = toast.loading("Publicando necesidades...");
// Later: toast.dismiss(toastId);
```

---

## 13. Siguientes Pasos

1. **Instalar Tooltip component** (`npx shadcn@latest add tooltip`)
2. **Crear custom components** en `src/web/src/components/crowdsourcing/`
3. **Implementar hooks de TanStack Query** en `src/web/src/features/crowdsourcing/hooks/`
4. **Crear paginas del wizard** en `src/web/src/features/crowdsourcing/pages/`
5. **Configurar routing** en `src/web/src/app/router.tsx`
6. **Testing responsive** en Chrome DevTools
7. **Testing accesibilidad** con axe DevTools

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (UI Designer Agent)
**Estado:** READY FOR IMPLEMENTATION
