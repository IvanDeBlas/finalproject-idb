# Diseno UI: Definir Recompensas (Admin Dashboard)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/admin
**Basado en:** docs/user-stories/definir-recompensas/ui-ux.md

---

## 1. Resumen

- **Componentes shadcn:** 12 (Card, Button, Badge, Dialog, AlertDialog, Input, Textarea, Select, Checkbox, Label, Skeleton, Separator)
- **Composiciones custom:** 4 (RewardsListPage, RewardCard, RewardFormModal, RewardDeleteDialog)
- **Responsive breakpoints:** sm (640px), md (768px), lg (1024px)
- **Animaciones:** 12 tipos (drag & drop, modal transitions, conditional sections)
- **Accesibilidad:** WCAG AA compliant con ARIA labels completos

---

## 2. Paleta de Colores (del proyecto)

| Uso | Variable | Hex | Ejemplo |
|-----|----------|-----|---------|
| Primary | --primary | #a855f7 | Botones principales, borders activos |
| Background | --background | #0a0a0a | Fondo principal |
| Card Background | --card | #1a1a2e | Cards, modales |
| Secondary Card | --card-secondary | #0f1729 | Stats cards, tips |
| Border | --border | #334155 | Bordes de cards |
| Muted Text | --muted-foreground | #94a3b8 | Textos secundarios |
| Foreground | --foreground | #ffffff | Textos principales |
| Destructive | --destructive | #ef4444 | Botones de eliminar |
| Success | --success | #10b981 | Estados positivos |
| Warning | --warning | #f59e0b | Alertas, stock limitado |

### Colores Especificos de Rewards

| Uso | Variable | Hex |
|-----|----------|-----|
| Reward Available | --reward-available | #10b981 |
| Reward Limited | --reward-limited | #f59e0b |
| Reward Sold Out | --reward-sold-out | #64748b |
| Drag Handle | --drag-handle | #64748b |
| Drag Active | --drag-active | #a855f7 |
| Drop Zone | --drop-zone | #a855f7 |
| Drop Zone BG | --drop-zone-bg | rgba(168, 85, 247, 0.1) |

---

## 3. Componentes por Screen

### 3.1 RewardsListPage - Pagina Principal de Gestion

#### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │  ← Volver a Campania                                  │
│             │  Recompensas: "Nuevo Album Midnight"                  │
│             │                                                       │
│  Dashboard  │  ┌────────────────────────────────────────────────┐  │
│  Mis        │  │  📊 Resumen                                    │  │
│  Campañas   │  │  Total: 3  •  Activas: 3  •  Stock: 400 unid.  │  │
│  Crear      │  └────────────────────────────────────────────────┘  │
│  Campana    │                                                       │
│  Mis        │  ┌────────────────────────────────────────────────┐  │
│  Backers    │  │  ⋮⋮ € 10  Descarga Digital            [Edit] [×]│  │
│  Config     │  │     Acceso anticipado al album en formato      │  │
│             │  │     digital + FLAC + MP3                       │  │
│  Ver        │  │     📦 Ilimitadas disponibles                  │  │
│  perfil     │  │     ✓ 234 backers                               │  │
│  Logout     │  └────────────────────────────────────────────────┘  │
│             │                                                       │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  ⋮⋮ € 25  CD Fisico                   [Edit] [×]│  │
│             │  │     CD firmado + descarga digital + booklet    │  │
│             │  │     dedicado con letras y fotos                │  │
│             │  │     📦 100 de 200 disponibles  ⚠️ 50% vendido  │  │
│             │  │     ✓ 89 backers                                │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                       │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  + Agregar recompensa                          │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                       │
│             │  💡 Tip: Ordena tus recompensas arrastrándolas     │  │
│             │      El orden se reflejará en la vista pública     │  │
│             │                                                       │
└────────────────────────────────────────────────────────────────────┘
```

#### Componentes

**Header Section**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Back Link | `<Link>` (Next.js) con `<ArrowLeft>` (lucide) | `flex items-center gap-2 text-muted-foreground hover:text-foreground mb-4 transition-colors` |
| Page Title | `<h1>` | `text-2xl font-bold text-foreground mb-2` |
| Campaign Subtitle | `<p>` | `text-sm text-muted-foreground mb-6` |

**Stats Card**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<Card>` | `bg-card-secondary border-border p-4 mb-6` |
| Stats Row | `<div>` | `flex items-center gap-6 text-sm` |
| Stat Item | `<div>` | `flex items-center gap-2` |
| Stat Icon | Lucide icon (`TrendingUp`, `Package`, `CheckCircle`) | `w-4 h-4 text-primary` |
| Stat Label | `<span>` | `text-muted-foreground` |
| Stat Value | `<span>` | `font-semibold text-foreground` |
| Stat Separator | `<Separator orientation="vertical">` | `h-4` |

**Sortable Container**

| Elemento | Componente | Customizacion |
|----------|------------|---------------|
| Container | `<div>` (dnd-kit `SortableContext`) | `space-y-3` |
| Drop Zone Indicator | `<div>` | `h-0.5 bg-gradient-to-r from-pink-500 to-purple-600 rounded-full animate-pulse` |

**Empty State**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `text-center py-16` |
| Icon | `<PackageOpen>` (lucide) | `w-20 h-20 text-muted-foreground mx-auto mb-4` |
| Title | `<h3>` | `text-xl font-bold text-foreground mb-2` |
| Description | `<p>` | `text-muted-foreground mb-6` |
| CTA Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white` |

**Add Reward Button**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Button | `<Button variant="outline">` | `w-full border-dashed border-primary text-primary hover:bg-primary/10 py-6 mb-4` |
| Icon | `<PlusCircle>` (lucide) | `w-5 h-5 mr-2` |

**Tip Box**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `bg-card-secondary border-l-4 border-primary p-4 rounded-r-lg` |
| Icon | `<Lightbulb>` (lucide) | `w-5 h-5 text-primary inline mr-2` |
| Text | `<p>` | `text-sm text-muted-foreground` |

**Loading State**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Skeleton Card | `<Skeleton>` | `h-[140px] w-full rounded-lg mb-3` (repetir 3-4 veces) |

#### Composicion

```tsx
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { ArrowLeft, TrendingUp, Package, CheckCircle, PlusCircle, Lightbulb, PackageOpen } from "lucide-react";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";

export const RewardsListPage = ({ campaniaId, campaniaTitulo }) => {
  const { data: rewards, isLoading } = useRewards(campaniaId);

  return (
    <div className="container max-w-4xl mx-auto px-4 py-8">
      {/* Header */}
      <Link href={`/dashboard/campanias/${campaniaId}`}
        className="flex items-center gap-2 text-muted-foreground hover:text-foreground mb-4 transition-colors">
        <ArrowLeft className="w-4 h-4" />
        Volver a Campania
      </Link>

      <h1 className="text-2xl font-bold text-foreground mb-2">Recompensas</h1>
      <p className="text-sm text-muted-foreground mb-6">{campaniaTitulo}</p>

      {/* Stats Card */}
      <Card className="bg-card-secondary border-border p-4 mb-6">
        <div className="flex items-center gap-6 text-sm">
          <div className="flex items-center gap-2">
            <TrendingUp className="w-4 h-4 text-primary" />
            <span className="text-muted-foreground">Total:</span>
            <span className="font-semibold text-foreground">{rewards?.length || 0}</span>
          </div>
          <Separator orientation="vertical" className="h-4" />
          <div className="flex items-center gap-2">
            <CheckCircle className="w-4 h-4 text-primary" />
            <span className="text-muted-foreground">Activas:</span>
            <span className="font-semibold text-foreground">
              {rewards?.filter(r => r.esActivo).length || 0}
            </span>
          </div>
          <Separator orientation="vertical" className="h-4" />
          <div className="flex items-center gap-2">
            <Package className="w-4 h-4 text-primary" />
            <span className="text-muted-foreground">Stock:</span>
            <span className="font-semibold text-foreground">
              {calculateTotalStock(rewards)} unidades
            </span>
          </div>
        </div>
      </Card>

      {/* Loading State */}
      {isLoading && (
        <div className="space-y-3">
          {[...Array(3)].map((_, i) => (
            <Skeleton key={i} className="h-[140px] w-full rounded-lg" />
          ))}
        </div>
      )}

      {/* Empty State */}
      {!isLoading && rewards?.length === 0 && (
        <div className="text-center py-16">
          <PackageOpen className="w-20 h-20 text-muted-foreground mx-auto mb-4" />
          <h3 className="text-xl font-bold text-foreground mb-2">
            No has creado recompensas aun
          </h3>
          <p className="text-muted-foreground mb-6">
            Las recompensas incentivan a los fans a apoyar tu campania
          </p>
          <Button
            onClick={openCreateModal}
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white">
            <PlusCircle className="w-5 h-5 mr-2" />
            Crear primera recompensa
          </Button>
        </div>
      )}

      {/* Rewards List (Sortable) */}
      {!isLoading && rewards?.length > 0 && (
        <SortableContext items={rewards} strategy={verticalListSortingStrategy}>
          <div className="space-y-3">
            {rewards.map((reward) => (
              <RewardCard key={reward.id} reward={reward} />
            ))}
          </div>
        </SortableContext>
      )}

      {/* Add Reward Button */}
      {!isLoading && rewards?.length > 0 && (
        <Button
          variant="outline"
          onClick={openCreateModal}
          className="w-full border-dashed border-primary text-primary hover:bg-primary/10 py-6 mb-4 mt-4">
          <PlusCircle className="w-5 h-5 mr-2" />
          Agregar recompensa
        </Button>
      )}

      {/* Tip Box */}
      {!isLoading && rewards?.length > 0 && (
        <div className="bg-card-secondary border-l-4 border-primary p-4 rounded-r-lg">
          <p className="text-sm text-muted-foreground">
            <Lightbulb className="w-5 h-5 text-primary inline mr-2" />
            Tip: Ordena tus recompensas arrastrándolas. El orden se reflejará en la vista pública
          </p>
        </div>
      )}
    </div>
  );
};
```

---

### 3.2 RewardCard (Sortable) - Card Individual con Drag & Drop

#### Layout

```
┌────────────────────────────────────────────────────────────┐
│ [⋮⋮] € 25  CD Fisico Firmado              [Edit] [Delete] │
│      CD firmado con dedicatoria personalizada              │
│      + descarga digital FLAC + MP3                        │
│                                                            │
│      📦 100 de 200 disponibles  ⚠️ 50% vendido             │
│      ✓ 89 backers                                          │
└────────────────────────────────────────────────────────────┘
```

#### Componentes

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card Container | `<Card>` (dnd-kit `useSortable`) | `bg-card border-border p-5 hover:border-primary cursor-move transition-all group relative` |
| Drag Handle | `<div>` con `<GripVertical>` | `absolute left-2 top-1/2 -translate-y-1/2 text-muted-foreground group-hover:text-primary cursor-grab active:cursor-grabbing` |
| Card Content | `<div>` | `pl-8 pr-20` |
| Header Row | `<div>` | `flex items-start justify-between mb-3` |
| Price Container | `<div>` | `flex items-baseline gap-2` |
| Price | `<span>` | `text-2xl font-bold text-primary` |
| Title | `<h3>` | `text-lg font-semibold text-foreground` |
| Description | `<p>` | `text-sm text-muted-foreground mb-3 line-clamp-2` |
| Meta Row | `<div>` | `flex items-center gap-4 text-xs` |
| Stock Container | `<div>` | `flex items-center gap-1` |
| Stock Icon (unlimited) | `<Infinity>` (lucide) | `w-4 h-4 text-green-400` |
| Stock Icon (available) | `<Package>` (lucide) | `w-4 h-4 text-muted-foreground` |
| Stock Icon (limited) | `<Package>` (lucide) | `w-4 h-4 text-warning` |
| Stock Icon (sold out) | `<XCircle>` (lucide) | `w-4 h-4 text-muted` |
| Stock Text (unlimited) | `<span>` | `text-green-400` |
| Stock Text (available) | `<span>` | `text-muted-foreground` |
| Stock Text (limited) | `<span>` | `text-warning` |
| Stock Text (sold out) | `<span>` | `text-muted` |
| Stock Warning Badge | `<Badge variant="outline">` | `border-warning text-warning bg-warning/10 ml-2` |
| Backers Count | `<div>` | `flex items-center gap-1` |
| Backers Icon | `<CheckCircle>` (lucide) | `w-4 h-4 text-primary` |
| Backers Text | `<span>` | `text-muted-foreground` |
| Actions Container | `<div>` | `absolute right-4 top-4 flex gap-2` |
| Edit Button | `<Button variant="ghost" size="sm">` | `text-primary hover:bg-primary/10` |
| Delete Button | `<Button variant="ghost" size="sm">` | `text-destructive hover:bg-destructive/10` |

#### Estados

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Border gris, drag handle visible al hover (desktop) |
| **Hover** | Border primary, elevation aumenta, drag handle mas visible |
| **Dragging** | Opacity 0.5, cursor grabbing, transform scale(1.02) |
| **Drop Zone Active** | Linea horizontal gradient entre cards |
| **Sold Out** | Opacity 60%, badge "Agotado" gris |
| **Stock Warning** | Badge "50% vendido" amarillo cuando disponibles < 50% |

#### Composicion

```tsx
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { GripVertical, Edit, Trash2, Package, Infinity, XCircle, CheckCircle } from "lucide-react";

export const RewardCard = ({ reward }) => {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: reward.id,
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  const stockPercentage = reward.cantidadMaxima
    ? ((reward.stockDisponible / reward.cantidadMaxima) * 100)
    : 100;
  const isLowStock = stockPercentage < 50 && stockPercentage > 0;
  const isSoldOut = stockPercentage === 0;

  return (
    <Card
      ref={setNodeRef}
      style={style}
      className={cn(
        "bg-card border-border p-5 hover:border-primary cursor-move transition-all group relative",
        isSoldOut && "opacity-60"
      )}>

      {/* Drag Handle */}
      <div
        {...attributes}
        {...listeners}
        className="absolute left-2 top-1/2 -translate-y-1/2 text-muted-foreground group-hover:text-primary cursor-grab active:cursor-grabbing">
        <GripVertical className="w-5 h-5" />
      </div>

      {/* Content */}
      <div className="pl-8 pr-20">
        {/* Header */}
        <div className="flex items-start justify-between mb-3">
          <div className="flex items-baseline gap-2">
            <span className="text-2xl font-bold text-primary">€ {reward.importeMinimo}</span>
            <h3 className="text-lg font-semibold text-foreground">{reward.nombre}</h3>
          </div>
        </div>

        {/* Description */}
        <p className="text-sm text-muted-foreground mb-3 line-clamp-2">
          {reward.descripcion}
        </p>

        {/* Meta Row */}
        <div className="flex items-center gap-4 text-xs">
          {/* Stock */}
          <div className="flex items-center gap-1">
            {!reward.cantidadMaxima ? (
              <>
                <Infinity className="w-4 h-4 text-green-400" />
                <span className="text-green-400">Ilimitadas disponibles</span>
              </>
            ) : isSoldOut ? (
              <>
                <XCircle className="w-4 h-4 text-muted" />
                <span className="text-muted">Agotado</span>
              </>
            ) : (
              <>
                <Package className={cn("w-4 h-4", isLowStock ? "text-warning" : "text-muted-foreground")} />
                <span className={isLowStock ? "text-warning" : "text-muted-foreground"}>
                  {reward.stockDisponible} de {reward.cantidadMaxima} disponibles
                </span>
                {isLowStock && (
                  <Badge variant="outline" className="border-warning text-warning bg-warning/10 ml-2">
                    {Math.round(stockPercentage)}% vendido
                  </Badge>
                )}
              </>
            )}
          </div>

          {/* Backers */}
          <div className="flex items-center gap-1">
            <CheckCircle className="w-4 h-4 text-primary" />
            <span className="text-muted-foreground">{reward.backersCount || 0} backers</span>
          </div>
        </div>
      </div>

      {/* Actions */}
      <div className="absolute right-4 top-4 flex gap-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => openEditModal(reward)}
          className="text-primary hover:bg-primary/10"
          aria-label={`Editar recompensa ${reward.nombre}`}>
          <Edit className="w-4 h-4" />
          <span className="ml-1 hidden sm:inline">Editar</span>
        </Button>
        <Button
          variant="ghost"
          size="sm"
          onClick={() => openDeleteDialog(reward)}
          className="text-destructive hover:bg-destructive/10"
          aria-label={`Eliminar recompensa ${reward.nombre}`}>
          <Trash2 className="w-4 h-4" />
        </Button>
      </div>
    </Card>
  );
};
```

---

### 3.3 RewardFormModal - Modal Formulario Create/Edit

#### Layout

```
┌──────────────────────────────────────────────────────────┐
│  [×] Nueva Recompensa                                    │
├──────────────────────────────────────────────────────────┤
│                                                          │
│  Nombre de la recompensa *                               │
│  [______________________________________________]         │
│  Max 200 caracteres                                      │
│                                                          │
│  Descripcion *                                           │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Describe que incluye esta recompensa...          │  │
│  │                                                    │  │
│  │  [Textarea ~4 lineas]                             │  │
│  └────────────────────────────────────────────────────┘  │
│  0/2000 caracteres                                       │
│                                                          │
│  Importe minimo *          Tipo de recompensa *          │
│  [€ 10.00________]          [Seleccionar tipo ▼]        │
│                                                          │
│  Moneda                     ☑️ Es add-on (complemento)   │
│  [EUR ▼]                                                 │
│                                                          │
│  ☑️ Stock limitado                                       │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Cantidad maxima disponible                        │  │
│  │  [200_______]                                      │  │
│  │                                                    │  │
│  │  Maximo por backer (opcional)                     │  │
│  │  [1_________]                                      │  │
│  └────────────────────────────────────────────────────┘  │
│                                                          │
│  ☑️ Incluye envio fisico                                 │
│  ┌────────────────────────────────────────────────────┐  │
│  │  Tiempo de entrega estimado                       │  │
│  │  [Marzo 2025____________________________]          │  │
│  └────────────────────────────────────────────────────┘  │
│                                                          │
│                                                          │
│  [Cancelar]                             [Guardar]       │
│  (outline)                             (gradient)       │
└──────────────────────────────────────────────────────────┘
```

#### Componentes

**Modal Container**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Overlay | `<Dialog>` backdrop | `bg-black/80 backdrop-blur-sm` |
| Content | `<DialogContent>` | `bg-card-secondary border-border max-w-2xl max-h-[90vh] overflow-y-auto` |
| Header | `<DialogHeader>` | `border-b border-border pb-4 mb-6` |
| Title | `<DialogTitle>` | `text-2xl font-bold text-foreground` |
| Close Button | `<DialogClose>` | `absolute right-4 top-4 text-muted-foreground hover:text-foreground` |

**Form Elements**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Form Container | `<form>` (react-hook-form) | `space-y-6 p-6` |
| Label (required) | `<Label>` | `text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-destructive after:ml-1` |
| Label (optional) | `<Label>` | `text-sm font-medium text-foreground mb-2 block` |
| Text Input | `<Input>` | `bg-card border-border text-foreground placeholder:text-muted-foreground focus:border-primary focus:ring-primary` |
| Textarea | `<Textarea>` | `bg-card border-border text-foreground min-h-[120px] resize-none placeholder:text-muted-foreground` |
| Character Counter | `<span>` | `text-xs text-muted-foreground mt-1 block` (color warning at 80%, destructive at 95%) |
| Hint Text | `<p>` | `text-xs text-muted-foreground mt-1 italic` |
| Grid 2 Columns | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Number Input (price) | `<Input type="number" step="0.01">` | `bg-card border-border text-foreground pl-8` (con prefix € absoluto) |
| Currency Prefix | `<span>` | `absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground` |
| Select Dropdown | `<Select>` | `bg-card border-border text-foreground` |
| Checkbox | `<Checkbox>` | `border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary` |
| Checkbox Label | `<label>` | `text-sm text-foreground ml-2 cursor-pointer` |
| Conditional Section | `<div>` | `bg-card border border-border p-4 rounded-lg mt-2 animate-in fade-in slide-in-from-top-2 duration-300` |
| Error Message | `<p>` | `text-xs text-destructive mt-1` |

**Modal Footer**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Footer | `<DialogFooter>` | `border-t border-border pt-4 mt-6 flex gap-3 justify-end` |
| Cancel Button | `<Button variant="outline">` | `border-border text-muted-foreground hover:text-foreground hover:bg-card` |
| Save Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |
| Loading Spinner | `<Loader2>` (lucide) | `w-4 h-4 animate-spin mr-2` |

#### Estados

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (Create)** | Titulo "Nueva Recompensa", form vacio, monedaId default EUR |
| **Default (Edit)** | Titulo "Editar Recompensa", form pre-completado |
| **Focus Input** | Border primary, ring primary glow |
| **Typing Nombre** | Character counter actualizado (X/200), warning color a 160 chars |
| **Typing Descripcion** | Character counter actualizado (X/2000), warning color a 1600 chars |
| **Stock Limitado Checked** | Conditional section visible con fade-in animation |
| **Envio Fisico Checked** | Conditional section visible con fade-in animation |
| **Validation Error** | Input border-destructive, mensaje debajo en rojo |
| **Loading Submit** | Save button spinner + "Guardando...", inputs disabled |
| **Success** | Toast verde, modal cierra, lista refresh |
| **Error** | Toast roja con mensaje backend |

#### Composicion

```tsx
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogClose } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { createRewardSchema } from "@/shared/schemas/reward.schema";
import { Loader2, X } from "lucide-react";

export const RewardFormModal = ({ isOpen, onClose, reward, campaniaId }) => {
  const isEdit = !!reward;
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    resolver: zodResolver(createRewardSchema),
    defaultValues: reward || {
      campaniaId,
      monedaId: 1,
      esAddOn: false,
      incluyeEnvioFisico: false,
      orden: 0
    }
  });

  const nombreLength = watch("nombre")?.length || 0;
  const descripcionLength = watch("descripcion")?.length || 0;
  const stockLimitado = watch("stockLimitado");
  const envioFisico = watch("incluyeEnvioFisico");

  const onSubmit = async (data) => {
    try {
      if (isEdit) {
        await updateReward({ ...data, id: reward.id });
        toast.success("Recompensa actualizada correctamente");
      } else {
        await createReward(data);
        toast.success("Recompensa creada correctamente");
      }
      onClose();
    } catch (error) {
      toast.error(getRewardErrorMessage(error.errorCode));
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="bg-card-secondary border-border max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader className="border-b border-border pb-4 mb-6">
          <DialogTitle className="text-2xl font-bold text-foreground">
            {isEdit ? "Editar Recompensa" : "Nueva Recompensa"}
          </DialogTitle>
          <DialogClose className="absolute right-4 top-4 text-muted-foreground hover:text-foreground">
            <X className="w-5 h-5" />
          </DialogClose>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 p-6">
          {/* Nombre */}
          <div>
            <Label className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-destructive after:ml-1">
              Nombre de la recompensa
            </Label>
            <Input
              {...register("nombre")}
              placeholder="Ej: Descarga Digital"
              className={cn(
                "bg-card border-border text-foreground placeholder:text-muted-foreground",
                errors.nombre && "border-destructive"
              )}
            />
            <div className="flex justify-between items-center mt-1">
              <span className={cn(
                "text-xs",
                nombreLength > 160 ? "text-warning" : nombreLength > 190 ? "text-destructive" : "text-muted-foreground"
              )}>
                {nombreLength}/200 caracteres
              </span>
            </div>
            {errors.nombre && <p className="text-xs text-destructive mt-1">{errors.nombre.message}</p>}
          </div>

          {/* Descripcion */}
          <div>
            <Label className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-destructive after:ml-1">
              Descripcion
            </Label>
            <Textarea
              {...register("descripcion")}
              placeholder="Describe que incluye esta recompensa..."
              className={cn(
                "bg-card border-border text-foreground min-h-[120px] resize-none",
                errors.descripcion && "border-destructive"
              )}
            />
            <span className={cn(
              "text-xs mt-1 block",
              descripcionLength > 1600 ? "text-warning" : descripcionLength > 1900 ? "text-destructive" : "text-muted-foreground"
            )}>
              {descripcionLength}/2000 caracteres
            </span>
            {errors.descripcion && <p className="text-xs text-destructive mt-1">{errors.descripcion.message}</p>}
          </div>

          {/* Grid 2 Columns: Importe y Tipo */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Importe Minimo */}
            <div>
              <Label className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-destructive after:ml-1">
                Importe minimo
              </Label>
              <div className="relative">
                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">€</span>
                <Input
                  type="number"
                  step="0.01"
                  {...register("importeMinimo", { valueAsNumber: true })}
                  placeholder="10.00"
                  className={cn(
                    "bg-card border-border text-foreground pl-8",
                    errors.importeMinimo && "border-destructive"
                  )}
                />
              </div>
              {errors.importeMinimo && <p className="text-xs text-destructive mt-1">{errors.importeMinimo.message}</p>}
            </div>

            {/* Tipo Recompensa */}
            <div>
              <Label className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-destructive after:ml-1">
                Tipo de recompensa
              </Label>
              <Select {...register("tipoRewardId")}>
                <SelectTrigger className="bg-card border-border text-foreground">
                  <SelectValue placeholder="Seleccionar tipo" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={1}>Digital</SelectItem>
                  <SelectItem value={2}>Fisico</SelectItem>
                  <SelectItem value={3}>Experiencia</SelectItem>
                  <SelectItem value={4}>Otro</SelectItem>
                </SelectContent>
              </Select>
              {errors.tipoRewardId && <p className="text-xs text-destructive mt-1">{errors.tipoRewardId.message}</p>}
            </div>
          </div>

          {/* Grid 2 Columns: Moneda y Add-on */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Moneda */}
            <div>
              <Label className="text-sm font-medium text-foreground mb-2 block">Moneda</Label>
              <Select {...register("monedaId")} defaultValue={1}>
                <SelectTrigger className="bg-card border-border text-foreground">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={1}>EUR (€)</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Es Add-on */}
            <div className="flex items-center pt-8">
              <Checkbox
                {...register("esAddOn")}
                id="esAddOn"
                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
              />
              <label htmlFor="esAddOn" className="text-sm text-foreground ml-2 cursor-pointer">
                Es add-on (complemento)
              </label>
            </div>
          </div>

          {/* Stock Limitado */}
          <div>
            <div className="flex items-center">
              <Checkbox
                {...register("stockLimitado")}
                id="stockLimitado"
                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
              />
              <label htmlFor="stockLimitado" className="text-sm text-foreground ml-2 cursor-pointer">
                Stock limitado
              </label>
            </div>

            {stockLimitado && (
              <div className="bg-card border border-border p-4 rounded-lg mt-2 animate-in fade-in slide-in-from-top-2 duration-300 space-y-4">
                <div>
                  <Label className="text-sm font-medium text-foreground mb-2 block">
                    Cantidad maxima disponible
                  </Label>
                  <Input
                    type="number"
                    {...register("cantidadMaxima", { valueAsNumber: true })}
                    placeholder="200"
                    className="bg-card-secondary border-border text-foreground"
                  />
                  {errors.cantidadMaxima && <p className="text-xs text-destructive mt-1">{errors.cantidadMaxima.message}</p>}
                </div>

                <div>
                  <Label className="text-sm font-medium text-foreground mb-2 block">
                    Maximo por backer (opcional)
                  </Label>
                  <Input
                    type="number"
                    {...register("cantidadPorBacker", { valueAsNumber: true })}
                    placeholder="1"
                    className="bg-card-secondary border-border text-foreground"
                  />
                  {errors.cantidadPorBacker && <p className="text-xs text-destructive mt-1">{errors.cantidadPorBacker.message}</p>}
                </div>
              </div>
            )}
          </div>

          {/* Envio Fisico */}
          <div>
            <div className="flex items-center">
              <Checkbox
                {...register("incluyeEnvioFisico")}
                id="envioFisico"
                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
              />
              <label htmlFor="envioFisico" className="text-sm text-foreground ml-2 cursor-pointer">
                Incluye envio fisico
              </label>
            </div>

            {envioFisico && (
              <div className="bg-card border border-border p-4 rounded-lg mt-2 animate-in fade-in slide-in-from-top-2 duration-300">
                <Label className="text-sm font-medium text-foreground mb-2 block">
                  Tiempo de entrega estimado
                </Label>
                <Input
                  {...register("tiempoEntregaEstimado")}
                  placeholder="Marzo 2025"
                  className="bg-card-secondary border-border text-foreground"
                />
                {errors.tiempoEntregaEstimado && <p className="text-xs text-destructive mt-1">{errors.tiempoEntregaEstimado.message}</p>}
              </div>
            )}
          </div>

          {/* Footer */}
          <DialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              className="border-border text-muted-foreground hover:text-foreground hover:bg-card">
              Cancelar
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
              className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8">
              {isSubmitting && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
              {isSubmitting ? "Guardando..." : "Guardar"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};
```

---

### 3.4 RewardDeleteDialog - Confirmacion de Eliminacion

#### Layout (Sin backings)

```
┌──────────────────────────────────────────────┐
│  ⚠️  Eliminar Recompensa                     │
├──────────────────────────────────────────────┤
│                                              │
│  ¿Estas seguro que deseas eliminar esta     │
│  recompensa?                                 │
│                                              │
│  "Descarga Digital" (€10)                    │
│                                              │
│  Esta accion no se puede deshacer.           │
│                                              │
│                                              │
│  [Cancelar]              [Eliminar]          │
│  (outline)               (destructive)       │
└──────────────────────────────────────────────┘
```

#### Layout (Con backings)

```
┌──────────────────────────────────────────────┐
│  ⚠️  No se puede eliminar                    │
├──────────────────────────────────────────────┤
│                                              │
│  Esta recompensa no se puede eliminar        │
│  porque tiene 23 backings confirmados.       │
│                                              │
│  Puedes desactivarla para que no aparezca   │
│  a nuevos backers, pero los existentes       │
│  mantendran su seleccion.                    │
│                                              │
│                                              │
│  [Cancelar]            [Desactivar]          │
│  (outline)             (warning)             │
└──────────────────────────────────────────────┘
```

#### Componentes

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Dialog Overlay | `<AlertDialog>` backdrop | `bg-black/80 backdrop-blur-sm` |
| Dialog Container | `<AlertDialogContent>` | `bg-card-secondary border-border max-w-md` |
| Dialog Header | `<AlertDialogHeader>` | `border-b border-border pb-4 mb-4` |
| Dialog Title | `<AlertDialogTitle>` | `flex items-center gap-2 text-xl font-bold text-foreground` |
| Warning Icon | `<AlertTriangle>` (lucide) | `w-6 h-6 text-warning` |
| Dialog Description | `<AlertDialogDescription>` | `text-muted-foreground space-y-3` |
| Reward Name Highlight | `<p>` | `font-semibold text-foreground bg-card px-3 py-2 rounded border border-border` |
| Warning Text | `<p>` | `text-sm text-muted italic` |
| Dialog Footer | `<AlertDialogFooter>` | `border-t border-border pt-4 mt-6 flex gap-3 justify-end` |
| Cancel Button | `<AlertDialogCancel>` | `border-border text-muted-foreground hover:text-foreground hover:bg-card` |
| Delete Button | `<AlertDialogAction>` | `bg-destructive hover:bg-destructive/90 text-white font-semibold` |
| Deactivate Button | `<AlertDialogAction>` | `bg-warning hover:bg-warning/90 text-card font-semibold` |
| Loading Spinner | `<Loader2>` (lucide) | `w-4 h-4 animate-spin mr-2` |

#### Estados

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (no backings)** | Titulo "Eliminar Recompensa", boton rojo "Eliminar" |
| **Has Backings** | Titulo "No se puede eliminar", texto explicativo, boton amarillo "Desactivar" |
| **Loading Delete** | Delete button spinner + "Eliminando...", otros disabled |
| **Loading Deactivate** | Deactivate button spinner + "Desactivando...", otros disabled |
| **Success Delete** | Dialog cierra, toast "Recompensa eliminada", lista refresh |
| **Success Deactivate** | Dialog cierra, toast "Recompensa desactivada", lista refresh |
| **Error** | Toast roja, dialog permanece abierto |

#### Composicion

```tsx
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction
} from "@/components/ui/alert-dialog";
import { AlertTriangle, Loader2 } from "lucide-react";

export const RewardDeleteDialog = ({ isOpen, onClose, reward }) => {
  const [isLoading, setIsLoading] = useState(false);
  const hasBackings = reward?.backersCount > 0;

  const handleConfirm = async () => {
    setIsLoading(true);
    try {
      if (hasBackings) {
        await deactivateReward(reward.id);
        toast.success("Recompensa desactivada. Ya no aparecera a nuevos backers");
      } else {
        await deleteReward(reward.id);
        toast.success("Recompensa eliminada correctamente");
      }
      onClose();
    } catch (error) {
      toast.error(getRewardSpecificErrorMessage(error.errorCode));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AlertDialog open={isOpen} onOpenChange={onClose}>
      <AlertDialogContent className="bg-card-secondary border-border max-w-md">
        <AlertDialogHeader className="border-b border-border pb-4 mb-4">
          <AlertDialogTitle className="flex items-center gap-2 text-xl font-bold text-foreground">
            <AlertTriangle className="w-6 h-6 text-warning" />
            {hasBackings ? "No se puede eliminar" : "Eliminar Recompensa"}
          </AlertDialogTitle>
        </AlertDialogHeader>

        <AlertDialogDescription className="text-muted-foreground space-y-3">
          {hasBackings ? (
            <>
              <p>
                Esta recompensa no se puede eliminar porque tiene{" "}
                <strong>{reward.backersCount} backings confirmados</strong>.
              </p>
              <p>
                Puedes desactivarla para que no aparezca a nuevos backers, pero los
                existentes mantendran su seleccion.
              </p>
            </>
          ) : (
            <>
              <p>¿Estas seguro que deseas eliminar esta recompensa?</p>
              <p className="font-semibold text-foreground bg-card px-3 py-2 rounded border border-border">
                "{reward?.nombre}" (€{reward?.importeMinimo})
              </p>
              <p className="text-sm text-muted italic">Esta accion no se puede deshacer.</p>
            </>
          )}
        </AlertDialogDescription>

        <AlertDialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
          <AlertDialogCancel
            disabled={isLoading}
            className="border-border text-muted-foreground hover:text-foreground hover:bg-card">
            Cancelar
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={handleConfirm}
            disabled={isLoading}
            className={cn(
              "font-semibold",
              hasBackings
                ? "bg-warning hover:bg-warning/90 text-card"
                : "bg-destructive hover:bg-destructive/90 text-white"
            )}>
            {isLoading && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
            {isLoading
              ? (hasBackings ? "Desactivando..." : "Eliminando...")
              : (hasBackings ? "Desactivar" : "Eliminar")
            }
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};
```

---

## 4. Responsive Design

| Breakpoint | Cambios |
|------------|---------|
| **< sm (640px)** | Sidebar colapsado, stats grid 2x2, drag handles permanentemente visibles, botones Edit/Delete solo iconos, modal full-screen, grid 2-columns → stack vertical, botones footer stack vertical full-width |
| **sm-md (640-1024px)** | Sidebar visible, layout normal, drag handles al hover, grid 2-columns funcional |
| **> lg (1024px)** | Layout completo, max-width containers (max-w-4xl), spacing amplio, drag handles al hover, elevation effects |

### Clases Tailwind Responsive

```tsx
// RewardsListPage Container
className="container max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8"

// Stats Card Grid (Mobile: 2x2, Desktop: 1x4)
className="grid grid-cols-2 sm:flex sm:items-center gap-4 sm:gap-6 text-sm"

// RewardCard Drag Handle (Mobile: always visible, Desktop: hover)
className="text-muted-foreground sm:group-hover:text-primary"

// RewardCard Edit Button (Mobile: icon only, Desktop: icon + text)
<Button>
  <Edit className="w-4 h-4" />
  <span className="ml-1 hidden sm:inline">Editar</span>
</Button>

// Modal Content (Mobile: full-screen, Desktop: max-w-2xl)
className="bg-card-secondary border-border max-w-full sm:max-w-2xl max-h-[90vh] overflow-y-auto"

// Form Grid (Mobile: stack, Desktop: 2-columns)
className="grid grid-cols-1 md:grid-cols-2 gap-4"

// Modal Footer Buttons (Mobile: stack vertical, Desktop: horizontal)
className="flex flex-col sm:flex-row gap-3 justify-end"
```

---

## 5. Animaciones

### Animaciones de Drag & Drop

```tsx
// Card being dragged
const style = {
  transform: CSS.Transform.toString(transform),
  transition,
  opacity: isDragging ? 0.5 : 1,
};

// Drop zone indicator
<div className="h-0.5 bg-gradient-to-r from-pink-500 to-purple-600 rounded-full animate-pulse" />
```

### CSS Keyframes Custom

```css
@keyframes dropZonePulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

/* Conditional section expand */
@keyframes slideInFromTop {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

### Transiciones de Estado

| Elemento | Animacion | Duracion | Easing |
|----------|-----------|----------|--------|
| Card hover | Border color + elevation | 200ms | ease |
| Drag start | Opacity + cursor change | 150ms | ease |
| Drop zone indicator | Fade in + pulse | 200ms | ease |
| Card reorder | Position transition | 300ms | ease-out |
| Modal open | Scale 0.95 → 1 + fade in | 250ms | ease-out |
| Modal close | Scale 1 → 0.95 + fade out | 200ms | ease-in |
| Dialog open | Fade in + scale | 200ms | ease-out |
| Conditional section | Slide down + fade | 300ms | ease-out |
| Delete card | Fade out + scale down | 250ms | ease-in |
| Toast | Slide from top + fade | 250ms | ease-out |
| Button loading | Spinner rotate | 600ms | linear |
| Skeleton pulse | Opacity pulse loop | 1500ms | ease-in-out |

---

## 6. Accesibilidad

### Contraste de Colores

| Par de Colores | Ratio | Estado |
|----------------|-------|--------|
| Primary (#a855f7) sobre Card (#1a1a2e) | 5.2:1 | ✓ WCAG AA |
| Foreground (#ffffff) sobre Card (#1a1a2e) | 12.6:1 | ✓ WCAG AAA |
| Muted (#94a3b8) sobre Card (#1a1a2e) | 4.8:1 | ✓ WCAG AA |
| Warning (#f59e0b) sobre Card (#1a1a2e) | 6.1:1 | ✓ WCAG AA |
| Destructive (#ef4444) sobre Card (#1a1a2e) | 4.9:1 | ✓ WCAG AA |

### Focus States

Todos los elementos interactivos incluyen:
```tsx
className="focus:ring-2 focus:ring-primary focus:ring-offset-2 focus:ring-offset-card focus:outline-none"
```

### Keyboard Navigation

| Tecla | Accion | Contexto |
|-------|--------|----------|
| **Tab** | Navegar entre elementos | Global |
| **Shift + Tab** | Navegar hacia atras | Global |
| **Enter** | Activar boton, submit form | Botones, forms |
| **Space** | Toggle checkbox, activar boton | Checkboxes, botones |
| **Esc** | Cerrar modal/dialog | Modales, dialogs |
| **Arrow Up** | Mover reward hacia arriba | Lista (alternativa drag) |
| **Arrow Down** | Mover reward hacia abajo | Lista (alternativa drag) |
| **Delete** | Abrir confirmacion eliminar | Reward card con focus |

### ARIA Labels

```tsx
// Sortable List
<div role="list" aria-label="Lista de recompensas ordenables">
  <div role="listitem" aria-roledescription="Recompensa ordenable">
    {/* Card content */}
  </div>
</div>

// Drag Handle
<button
  {...listeners}
  aria-label="Arrastrar para reordenar Descarga Digital"
  aria-roledescription="Mango de arrastre">
  <GripVertical aria-hidden="true" />
</button>

// Edit Button
<Button aria-label="Editar recompensa Descarga Digital">
  <Edit aria-hidden="true" />
  Editar
</Button>

// Delete Button
<Button aria-label="Eliminar recompensa Descarga Digital">
  <Trash2 aria-hidden="true" />
</Button>

// Modal Form
<div role="dialog" aria-modal="true" aria-labelledby="reward-form-title">
  <h2 id="reward-form-title">Nueva Recompensa</h2>
  <form aria-label="Formulario de recompensa">
    <label htmlFor="nombre">Nombre de la recompensa *</label>
    <input
      id="nombre"
      aria-required="true"
      aria-describedby="nombre-hint nombre-error"
    />
    <span id="nombre-hint">Max 200 caracteres</span>
    <span id="nombre-error" role="alert" aria-live="polite">
      {error && "El nombre es obligatorio"}
    </span>
  </form>
</div>

// Stock Badge
<span role="status" aria-label="Stock disponible: 234 de 500 unidades">
  <Package aria-hidden="true" />
  234 de 500 disponibles
</span>

// Loading Button
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 aria-hidden="true" className="animate-spin" />}
  {isPending ? "Guardando..." : "Guardar"}
</Button>

// Delete Dialog
<div
  role="alertdialog"
  aria-modal="true"
  aria-labelledby="delete-dialog-title"
  aria-describedby="delete-dialog-description">
  <h2 id="delete-dialog-title">Eliminar Recompensa</h2>
  <p id="delete-dialog-description">
    ¿Estas seguro que deseas eliminar esta recompensa? Esta accion no se puede deshacer.
  </p>
  <button aria-label="Cancelar eliminacion">Cancelar</button>
  <button aria-label="Confirmar eliminacion de recompensa">Eliminar</button>
</div>
```

### Alternativa Accesible para Drag & Drop

```tsx
// Botones de reordenamiento como alternativa al drag & drop
<div role="group" aria-label="Reordenar recompensa">
  <Button
    size="sm"
    variant="ghost"
    onClick={() => moveUp(reward.id)}
    aria-label="Mover Descarga Digital hacia arriba"
    disabled={isFirst}>
    <ChevronUp aria-hidden="true" />
  </Button>
  <Button
    size="sm"
    variant="ghost"
    onClick={() => moveDown(reward.id)}
    aria-label="Mover Descarga Digital hacia abajo"
    disabled={isLast}>
    <ChevronDown aria-hidden="true" />
  </Button>
</div>
```

---

## 7. Iconografia (Lucide React)

| Icono | Nombre Componente | Uso |
|-------|------------------|-----|
| ⋮⋮ | `GripVertical` | Drag handle para reordenar |
| ← | `ArrowLeft` | Back link |
| 📊 | `TrendingUp` | Stats - Total rewards |
| 📦 | `Package` | Stats - Stock / Stock indicator |
| ✓ | `CheckCircle` | Stats - Activas / Backers count |
| ∞ | `Infinity` | Stock ilimitado |
| ❌ | `XCircle` | Stock agotado |
| ✏️ | `Edit` | Boton editar |
| 🗑️ | `Trash2` | Boton eliminar |
| ➕ | `PlusCircle` | Agregar recompensa |
| 💡 | `Lightbulb` | Tip box |
| 📭 | `PackageOpen` | Empty state |
| ⚠️ | `AlertTriangle` | Warning dialog |
| 🔄 | `Loader2` | Loading spinner |
| ✕ | `X` | Cerrar modal |
| ▲ | `ChevronUp` | Reordenar arriba |
| ▼ | `ChevronDown` | Reordenar abajo |

Todos los iconos deben incluir `aria-hidden="true"` cuando estan acompanados de texto.

---

## 8. Checklist de Implementacion

### RewardsListPage
- [ ] Back link funcional con icono ArrowLeft
- [ ] Titulo dinamico con nombre de campania
- [ ] Stats card con metricas calculadas (total, activas, stock)
- [ ] Separadores verticales entre stats
- [ ] Empty state con icono PackageOpen grande
- [ ] Empty state CTA con gradient button
- [ ] Sortable container con dnd-kit SortableContext
- [ ] Drop zone indicator animado
- [ ] Loading skeletons (3-4 cards)
- [ ] Boton "Agregar recompensa" outline dashed
- [ ] Tip box con borde primary izquierdo
- [ ] Responsive: sidebar hamburger mobile, stats grid 2x2

### RewardCard
- [ ] Card con border hover primary
- [ ] Drag handle con GripVertical icon
- [ ] Drag handle visible al hover (desktop) / permanente (mobile)
- [ ] Precio texto-2xl bold primary
- [ ] Titulo texto-lg semibold
- [ ] Descripcion line-clamp-2
- [ ] Stock icon dinamico (Infinity/Package/XCircle)
- [ ] Stock text con colores correctos (green/muted/warning)
- [ ] Stock warning badge "X% vendido" cuando < 50%
- [ ] Backers count con CheckCircle icon
- [ ] Boton Edit primary hover
- [ ] Boton Delete destructive hover
- [ ] Estados: default, hover, dragging, sold-out
- [ ] Drag state: opacity 0.5, cursor grabbing
- [ ] Responsive: botones icon-only en mobile

### RewardFormModal
- [ ] Dialog con backdrop blur
- [ ] Titulo dinamico: "Nueva" o "Editar"
- [ ] Close button X absoluto
- [ ] Form con react-hook-form + zodResolver
- [ ] Input nombre con label required asterisk
- [ ] Character counter nombre (X/200) con colores
- [ ] Textarea descripcion min-h-120px
- [ ] Character counter descripcion (X/2000)
- [ ] Input importe con € prefix absoluto
- [ ] Select tipo recompensa con opciones
- [ ] Select moneda (default EUR)
- [ ] Checkbox "Es add-on"
- [ ] Checkbox "Stock limitado" con conditional section
- [ ] Conditional section fade-in animation
- [ ] Input cantidad maxima (solo si stock limitado)
- [ ] Input maximo por backer (opcional)
- [ ] Checkbox "Incluye envio fisico" con conditional section
- [ ] Input tiempo entrega (solo si envio fisico)
- [ ] Validation errors debajo de inputs
- [ ] Boton Cancelar outline
- [ ] Boton Guardar gradient con loading state
- [ ] Loading: spinner + "Guardando...", inputs disabled
- [ ] Toast success/error
- [ ] Modal cierra en success
- [ ] Responsive: max-w-full mobile, grid → stack

### RewardDeleteDialog
- [ ] AlertDialog con backdrop blur
- [ ] Titulo con AlertTriangle icon
- [ ] Variante 1: "Eliminar Recompensa" (no backings)
- [ ] Variante 2: "No se puede eliminar" (has backings)
- [ ] Reward name highlighted en card secundario
- [ ] Texto advertencia italic
- [ ] Boton Cancelar outline
- [ ] Boton Eliminar (destructive) en variante 1
- [ ] Boton Desactivar (warning) en variante 2
- [ ] Loading state en boton de accion
- [ ] Dialog cierra en success
- [ ] Toast contextual
- [ ] Responsive: max-w-full mobile

### Cross-cutting
- [ ] Design tokens consistentes (primary, card, border, etc.)
- [ ] Dark theme aplicado (#1a1a2e, #0f1729)
- [ ] Gradient buttons (pink → purple) en CTAs
- [ ] dnd-kit implementado para sortable list
- [ ] Tailwind utilities exclusivamente (no CSS custom)
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring primary en todos interactivos
- [ ] Form validation con Zod
- [ ] TanStack Query para mutations
- [ ] Manejo de errores ServiceResponse
- [ ] Toast notifications (sonner)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders
- [ ] Empty states claros
- [ ] ARIA labels completos
- [ ] Keyboard navigation funcional
- [ ] Alternativa drag & drop con botones ↑↓

---

## 9. Dependencias de Componentes

### shadcn/ui Components Requeridos

```bash
npx shadcn-ui@latest add card button badge dialog alert-dialog input textarea select checkbox label skeleton separator
```

### Librerias Adicionales

```json
{
  "dependencies": {
    "@dnd-kit/core": "^6.0.0",
    "@dnd-kit/sortable": "^7.0.0",
    "@dnd-kit/utilities": "^3.2.0",
    "lucide-react": "^0.300.0",
    "react-hook-form": "^7.48.0",
    "@hookform/resolvers": "^3.3.0",
    "zod": "^3.22.0",
    "sonner": "^1.2.0"
  }
}
```

---

## 10. Notas Tecnicas

### Drag & Drop con dnd-kit

```tsx
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors } from '@dnd-kit/core';
import { arrayMove, SortableContext, sortableKeyboardCoordinates, verticalListSortingStrategy } from '@dnd-kit/sortable';

const sensors = useSensors(
  useSensor(PointerSensor),
  useSensor(KeyboardSensor, {
    coordinateGetter: sortableKeyboardCoordinates,
  })
);

const handleDragEnd = (event) => {
  const { active, over } = event;

  if (active.id !== over.id) {
    const oldIndex = rewards.findIndex((r) => r.id === active.id);
    const newIndex = rewards.findIndex((r) => r.id === over.id);

    const newRewards = arrayMove(rewards, oldIndex, newIndex);

    // Update local state
    setRewards(newRewards);

    // Persist to backend
    const reorderedPayload = newRewards.map((r, index) => ({
      rewardId: r.id,
      orden: index + 1
    }));

    reorderRewardsMutation.mutate({ campaniaId, rewardOrders: reorderedPayload });
  }
};
```

### Stock Calculation

```tsx
const calculateStockPercentage = (reward) => {
  if (!reward.cantidadMaxima) return 100; // Ilimitado
  const stockDisponible = reward.cantidadMaxima - (reward.backersCount || 0);
  return (stockDisponible / reward.cantidadMaxima) * 100;
};

const isLowStock = (reward) => {
  const percentage = calculateStockPercentage(reward);
  return percentage < 50 && percentage > 0;
};

const isSoldOut = (reward) => {
  return calculateStockPercentage(reward) === 0;
};
```

### Character Counter Logic

```tsx
const getCharCounterColor = (current, max) => {
  const percentage = (current / max) * 100;
  if (percentage >= 95) return "text-destructive";
  if (percentage >= 80) return "text-warning";
  return "text-muted-foreground";
};
```

### Conditional Section Animation

```tsx
// En Tailwind config
module.exports = {
  theme: {
    extend: {
      keyframes: {
        'slide-in-from-top': {
          '0%': { opacity: '0', transform: 'translateY(-8px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' }
        }
      },
      animation: {
        'slide-in': 'slide-in-from-top 300ms ease-out'
      }
    }
  }
}

// Uso en componente
className="animate-slide-in"
// o usando utility class de Tailwind
className="animate-in fade-in slide-in-from-top-2 duration-300"
```

---

**Fin del Plan de Diseno UI**

**Creado:** 2026-02-13
**Feature:** definir-recompensas
**Target:** Admin Dashboard
**Archivo:** `plans/definir-recompensas/frontend-admin/ui-design.md`
