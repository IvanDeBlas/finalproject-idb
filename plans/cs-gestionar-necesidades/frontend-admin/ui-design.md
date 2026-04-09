# Diseno UI: Gestionar Necesidades de Crowdsourcing (Admin)

**Fecha:** 2026-02-16
**Feature:** cs-gestionar-necesidades
**Target:** src/admin

---

## 1. Resumen

- **Componentes shadcn/ui:** 15 componentes base
- **Composiciones custom:** 6 componentes personalizados
- **Responsive breakpoints:** mobile (<768px), tablet (768-1024px), desktop (>1024px)
- **Tema:** Dark theme con gradientes pink/purple
- **Accesibilidad:** WCAG AA, ARIA labels completos, navegacion por teclado

---

## 2. Paleta de Colores (Design Tokens)

| Uso | Clase Tailwind / CSS Var | Ejemplo de Aplicacion |
|-----|--------------------------|----------------------|
| **Primary Gradient** | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones principales, badges destacados |
| **Background Primary** | `bg-[#1a1a2e]` | Fondo general de la app |
| **Background Card** | `bg-[#0f1729]` | Cards, formularios, modales |
| **Background Card Hover** | `bg-[#1e2a42]` | Hover en cards interactivas |
| **Background Input** | `bg-[#1a1a2e]` | Inputs, textareas, selects |
| **Border Primary** | `border-[#334155]` | Bordes de cards, inputs |
| **Border Focus** | `border-purple-500 ring-2 ring-purple-500/50` | Focus states en inputs |
| **Text Primary** | `text-white` | Titulos, contenido principal |
| **Text Secondary** | `text-[#94a3b8]` | Subtitulos, meta info |
| **Text Muted** | `text-[#64748b]` | Placeholders, hints |
| **Text Label** | `text-[#cbd5e1]` | Labels de formularios |
| **Estado Abierta** | `bg-green-900/20 text-green-400 border-green-700` | Badge estado "Abierta" |
| **Estado En Progreso** | `bg-blue-900/20 text-blue-400 border-blue-700` | Badge estado "En Progreso" |
| **Estado Cerrada** | `bg-gray-900/20 text-gray-400 border-gray-700` | Badge estado "Cerrada" |
| **Estado Cancelada** | `bg-red-900/20 text-red-400 border-red-700` | Badge estado "Cancelada" |
| **Warning** | `bg-amber-900/20 text-amber-300 border-amber-700` | Alertas, avisos de fechas proximas |
| **Destructive** | `bg-red-600 hover:bg-red-700` | Botones de cierre/eliminacion |

---

## 3. Componentes shadcn/ui Utilizados

### 3.1 Layout Components
- **Card** (`Card`, `CardHeader`, `CardTitle`, `CardDescription`, `CardContent`, `CardFooter`) - Contenedores principales
- **Separator** - Divisores visuales
- **Tabs** - (futuro: organizar propuestas por estado)

### 3.2 Form Components
- **Input** - Titulo, presupuestos, ubicacion
- **Textarea** - Descripcion, motivo de cierre
- **Label** - Labels de formularios
- **Select** (`Select`, `SelectTrigger`, `SelectContent`, `SelectItem`) - Tipo, modalidad, moneda, proyecto
- **Button** - Acciones primarias y secundarias
- **Calendar** + **Popover** - DatePicker para fechas limite e inicio

### 3.3 Feedback Components
- **Alert** (`Alert`, `AlertTitle`, `AlertDescription`) - Warnings en edicion, cierre
- **Sonner** (Toast) - Mensajes de exito/error
- **Skeleton** - Loading states

### 3.4 Overlay Components
- **Dialog** (`Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`) - Dialogo de cierre
- **Popover** - Calendarios, filtros avanzados (futuro)
- **DropdownMenu** - Menu de acciones (⋮)
- **Tooltip** - Tooltips informativos

### 3.5 Data Display
- **Badge** - Estados, modalidad, precio, propuestas count
- **Avatar** - Foto del profesional en propuestas
- **Table** - (futuro: vista de tabla alternativa)

---

## 4. Componentes por Pantalla

### 4.1 MisNecesidadesPage

**Ruta:** `/dashboard/crowdsourcing/necesidades`

#### Layout General

```
┌─────────────────────────────────────────────────────────┐
│  [Sidebar - fuera del scope]                            │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Header Section                                    │  │
│  │   <h1> + <p subtitle> + <Button actions>         │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Filters Card                                      │  │
│  │   <Select estado> + <Input search>               │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Grid de NecesidadCard (responsive)               │  │
│  │   [Card 1] [Card 2] [Card 3]                     │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Paginacion (si > pageSize)                       │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

#### Componentes Detallados

**Header Section**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-6` |
| Actions Container | `<div>` | `flex gap-3 mb-6` |
| Nueva Necesidad Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white` |
| Usar Plantilla Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |

**Composicion:**
```tsx
<div className="space-y-6">
  <div>
    <h1 className="text-3xl font-bold text-white mb-2">
      Mis Necesidades
    </h1>
    <p className="text-lg text-[#94a3b8]">
      Gestiona tus solicitudes de servicios profesionales
    </p>
  </div>

  <div className="flex gap-3">
    <Button
      onClick={handleNuevaNecesidad}
      className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
    >
      <Plus className="w-4 h-4 mr-2" />
      Nueva Necesidad
    </Button>
    <Button
      variant="outline"
      onClick={handleUsarPlantilla}
      className="border-[#334155] text-white hover:bg-[#1e2a42]"
    >
      <FileText className="w-4 h-4 mr-2" />
      Usar Plantilla
    </Button>
  </div>
</div>
```

**Filters Card**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mb-6` |
| Inner Flex | `<div>` | `flex flex-col md:flex-row gap-4 items-start md:items-center` |
| Estado Filter Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Estado Select | `<Select>` | `w-full md:w-48` |
| SelectTrigger | `<SelectTrigger>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| SelectContent | `<SelectContent>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Search Input | `<Input>` | `flex-1 bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-4 mb-6">
  <div className="flex flex-col md:flex-row gap-4 items-start md:items-center">
    <div className="w-full md:w-48">
      <Label className="text-sm font-medium text-[#cbd5e1] mb-2">
        Estado
      </Label>
      <Select value={estadoFilter} onValueChange={setEstadoFilter}>
        <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white">
          <SelectValue placeholder="Todos los estados" />
        </SelectTrigger>
        <SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
          <SelectItem value="all">Todos</SelectItem>
          <SelectItem value="1">Abierta</SelectItem>
          <SelectItem value="2">En Progreso</SelectItem>
          <SelectItem value="3">Cerrada</SelectItem>
          <SelectItem value="4">Cancelada</SelectItem>
        </SelectContent>
      </Select>
    </div>

    <div className="flex-1 w-full">
      <Label className="text-sm font-medium text-[#cbd5e1] mb-2">
        Buscar
      </Label>
      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b]" />
        <Input
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Buscar por titulo o descripcion..."
          className="pl-10 bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]"
        />
      </div>
    </div>
  </div>
</Card>
```

**NecesidadCard (Componente Custom)**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-4 hover:bg-[#1e2a42] hover:border-purple-500 transition-all cursor-pointer` |
| Header Row | `<div>` | `flex items-start justify-between mb-3` |
| Estado Badge | `<EstadoBadge>` (custom) | Segun estado (ver componente 4.7) |
| Title | `<h3>` | `text-xl font-semibold text-white mb-3` |
| Meta Info Row | `<div>` | `flex items-center gap-3 text-sm text-[#94a3b8] mb-2` |
| Tipo Icon | Lucide icon | `w-4 h-4` (Music, Palette, Video, Megaphone segun tipo) |
| Tipo Text | `<span>` | `text-[#94a3b8]` |
| Modalidad Badge | `<Badge variant="secondary" size="sm">` | `flex items-center gap-1` |
| Budget Row | `<div>` | `text-base font-medium text-white mb-3` |
| Stats Row | `<div>` | `flex items-center gap-4 text-sm text-[#94a3b8] mb-4` |
| Propuestas Counter | `<div>` | `flex items-center gap-1` con icono Users + Badge si > 0 |
| Date Info | `<div>` | `flex items-center gap-1` con icono Calendar |
| Fecha Limite | `<span>` | `text-[#94a3b8]` normal, `text-amber-400` si < 7 dias, `text-red-400` si < 3 dias |
| Actions Row | `<div>` | `flex gap-2 pt-3 border-t border-[#334155]` |
| Ver Button | `<Button variant="ghost" size="sm">` | `text-purple-400 hover:text-purple-300` |
| Editar Button | `<Button variant="ghost" size="sm">` | `text-white hover:text-white/80` (solo si estado = Abierta) |
| Cerrar Button | `<Button variant="ghost" size="sm">` | `text-red-400 hover:text-red-300` (si Abierta o En Progreso) |

**Composicion:**
```tsx
interface NecesidadCardProps {
  necesidad: NecesidadCrowdsourcingList;
  onView: (id: string) => void;
  onEdit?: (id: string) => void;
  onClose?: (id: string) => void;
}

const NecesidadCard: FC<NecesidadCardProps> = ({ necesidad, onView, onEdit, onClose }) => {
  const isEditable = necesidad.estadoNecesidadId === 1; // Abierta
  const isCloseable = [1, 2].includes(necesidad.estadoNecesidadId); // Abierta o En Progreso

  const tipoIcons: Record<number, any> = {
    1: Music2, // Produccion Musical
    2: Palette, // Diseno Grafico
    3: Video, // Video
    4: Megaphone, // Marketing
  };

  const TipoIcon = tipoIcons[necesidad.tipoNecesidadId] || FileText;

  const diasRestantes = necesidad.fechaLimitePropuestas
    ? differenceInDays(new Date(necesidad.fechaLimitePropuestas), new Date())
    : null;

  return (
    <Card
      className="bg-[#0f1729] border-[#334155] p-6 hover:bg-[#1e2a42] hover:border-purple-500 transition-all cursor-pointer"
      onClick={() => onView(necesidad.id)}
      tabIndex={0}
      role="article"
      aria-label={`${necesidad.titulo}. Estado: ${necesidad.estadoNecesidadNombre}. ${necesidad.numeroPropuestas} propuestas`}
    >
      <div className="flex items-start justify-between mb-3">
        <EstadoBadge estado={necesidad.estadoNecesidadId} />
      </div>

      <h3 className="text-xl font-semibold text-white mb-3">
        {necesidad.titulo}
      </h3>

      <div className="flex items-center gap-3 text-sm text-[#94a3b8] mb-2">
        <div className="flex items-center gap-1">
          <TipoIcon className="w-4 h-4" />
          <span>{necesidad.tipoNecesidadNombre}</span>
        </div>
        <Badge variant="secondary" size="sm" className="flex items-center gap-1">
          {necesidad.modalidadTrabajoId === 2 ? (
            <Globe className="w-3 h-3" />
          ) : necesidad.modalidadTrabajoId === 1 ? (
            <MapPin className="w-3 h-3" />
          ) : (
            <Repeat className="w-3 h-3" />
          )}
          {necesidad.modalidadTrabajoNombre}
        </Badge>
      </div>

      <div className="text-base font-medium text-white mb-3">
        {formatPresupuesto(
          necesidad.presupuestoMin,
          necesidad.presupuestoMax,
          necesidad.monedaId
        )}
      </div>

      <div className="flex items-center gap-4 text-sm text-[#94a3b8] mb-4">
        <div className="flex items-center gap-1">
          <Users className="w-4 h-4" />
          <span>{necesidad.numeroPropuestas} propuestas</span>
          {necesidad.numeroPropuestas > 0 && (
            <Badge variant="outline" className="ml-1 text-xs">
              {necesidad.numeroPropuestas}
            </Badge>
          )}
        </div>

        <div className="flex items-center gap-1">
          <Calendar className="w-4 h-4" />
          <span>
            {formatDistanceToNow(new Date(necesidad.fechaCreacion), {
              addSuffix: true,
              locale: es
            })}
          </span>
        </div>

        {necesidad.fechaLimitePropuestas && (
          <div className={cn(
            "flex items-center gap-1",
            diasRestantes !== null && diasRestantes < 3 && "text-red-400",
            diasRestantes !== null && diasRestantes >= 3 && diasRestantes < 7 && "text-amber-400"
          )}>
            <Clock className="w-4 h-4" />
            <span>
              Cierra: {format(new Date(necesidad.fechaLimitePropuestas), "dd MMM yyyy", { locale: es })}
              {diasRestantes !== null && diasRestantes < 7 && (
                <Badge
                  variant="outline"
                  className={cn(
                    "ml-2",
                    diasRestantes < 3 ? "border-red-500 text-red-400" : "border-amber-500 text-amber-400"
                  )}
                >
                  {diasRestantes < 3 ? "Urgente" : "Cierra pronto"}
                </Badge>
              )}
            </span>
          </div>
        )}
      </div>

      <div className="flex gap-2 pt-3 border-t border-[#334155]" onClick={(e) => e.stopPropagation()}>
        <Button
          variant="ghost"
          size="sm"
          onClick={(e) => { e.stopPropagation(); onView(necesidad.id); }}
          className="text-purple-400 hover:text-purple-300"
        >
          <Eye className="w-4 h-4 mr-1" />
          Ver Detalle
        </Button>

        {isEditable && onEdit && (
          <Button
            variant="ghost"
            size="sm"
            onClick={(e) => { e.stopPropagation(); onEdit(necesidad.id); }}
            className="text-white hover:text-white/80"
          >
            <Edit className="w-4 h-4 mr-1" />
            Editar
          </Button>
        )}

        {isCloseable && onClose && (
          <Button
            variant="ghost"
            size="sm"
            onClick={(e) => { e.stopPropagation(); onClose(necesidad.id); }}
            className="text-red-400 hover:text-red-300"
          >
            <XCircle className="w-4 h-4 mr-1" />
            Cerrar
          </Button>
        )}
      </div>
    </Card>
  );
};
```

**Empty State**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Container | `<div>` | `flex flex-col items-center justify-center min-h-[400px] text-center p-8` |
| Icon | `<Inbox>` | `w-20 h-20 text-[#64748b] mb-4` |
| Title | `<h3>` | `text-xl font-semibold text-white mb-2` |
| Description | `<p>` | `text-base text-[#94a3b8] mb-6 max-w-md` |
| Actions | `<div>` | `flex gap-3` |

**Composicion:**
```tsx
<div className="flex flex-col items-center justify-center min-h-[400px] text-center p-8">
  <Inbox className="w-20 h-20 text-[#64748b] mb-4" aria-hidden="true" />
  <h3 className="text-xl font-semibold text-white mb-2">
    No tienes necesidades publicadas
  </h3>
  <p className="text-base text-[#94a3b8] mb-6 max-w-md">
    Publica lo que necesitas y recibe propuestas de profesionales cualificados
  </p>
  <div className="flex gap-3">
    <Button
      onClick={handleNuevaNecesidad}
      className="bg-gradient-to-r from-pink-500 to-purple-600"
    >
      <Plus className="w-4 h-4 mr-2" />
      Publicar Necesidad
    </Button>
    <Button
      variant="outline"
      onClick={handleUsarPlantilla}
      className="border-[#334155] text-white"
    >
      <FileText className="w-4 h-4 mr-2" />
      Usar Plantilla
    </Button>
  </div>
</div>
```

**Loading State (Skeleton)**

```tsx
{/* Repetir 3-4 veces */}
<Card className="p-6 mb-4 bg-[#0f1729] border-[#334155]">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-6 w-32 bg-[#1e2a42]" />
  </div>
  <Skeleton className="h-7 w-3/4 mb-3 bg-[#1e2a42]" />
  <div className="flex gap-3 mb-2">
    <Skeleton className="h-5 w-24 bg-[#1e2a42]" />
    <Skeleton className="h-5 w-20 bg-[#1e2a42]" />
  </div>
  <Skeleton className="h-5 w-32 mb-3 bg-[#1e2a42]" />
  <div className="flex gap-4 mb-4">
    <Skeleton className="h-4 w-24 bg-[#1e2a42]" />
    <Skeleton className="h-4 w-28 bg-[#1e2a42]" />
  </div>
  <div className="flex gap-2 pt-3 border-t border-[#334155]">
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
  </div>
</Card>
```

---

### 4.2 NuevaNecesidadPage

**Ruta:** `/dashboard/crowdsourcing/necesidades/nueva`

#### Layout General

```
┌─────────────────────────────────────────────────────────┐
│  [Sidebar]                                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Back Link + Title                                 │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Card Section 1: INFORMACION BASICA               │  │
│  │   - Titulo                                        │  │
│  │   - Descripcion                                   │  │
│  │   - Tipo Necesidad + Modalidad (grid 2 cols)     │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Card Section 2: PRESUPUESTO                       │  │
│  │   - Min + Max + Moneda                            │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Card Section 3: UBICACION Y FECHAS                │  │
│  │   - Ciudad + Pais (condicional)                   │  │
│  │   - Fecha Limite + Fecha Inicio                   │  │
│  │   - Proyecto Artistico                            │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Actions: [Cancelar] [Publicar Necesidad]         │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

#### Componentes Detallados

**Back Link + Title**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Back Link | `<Link>` | `flex items-center gap-2 text-purple-400 hover:text-purple-300 hover:underline mb-4` |
| Back Icon | `<ArrowLeft>` | `w-4 h-4` |
| Title | `<h1>` | `text-3xl font-bold text-white mb-6` |

**Composicion:**
```tsx
<Link
  href="/dashboard/crowdsourcing/necesidades"
  className="flex items-center gap-2 text-purple-400 hover:text-purple-300 hover:underline mb-4"
>
  <ArrowLeft className="w-4 h-4" />
  Volver a Mis Necesidades
</Link>

<h1 className="text-3xl font-bold text-white mb-6">
  Publicar Nueva Necesidad
</h1>
```

**Section Card 1: INFORMACION BASICA**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| FormField (cada campo) | Wrapper | `space-y-2` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1]` con `<span className="text-red-400 ml-1">*</span>` si required |
| Input Titulo | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] focus:border-purple-500 focus:ring-2 focus:ring-purple-500/50` |
| Textarea Descripcion | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[120px]` |
| Character Counter | `<span>` | `text-xs text-[#64748b] mt-1` |
| Grid 2 Cols | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Select Tipo Necesidad | `<Select>` | `w-full` |
| Select Modalidad | `<Select>` | `w-full` |
| Error Message | `<p>` | `text-sm text-red-400 mt-1` con role="alert" |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <h2 className="text-lg font-semibold text-white mb-4">
    INFORMACION BASICA
  </h2>

  <form className="space-y-4">
    {/* Titulo */}
    <div className="space-y-2">
      <Label htmlFor="titulo" className="text-sm font-medium text-[#cbd5e1]">
        Titulo
        <span className="text-red-400 ml-1">*</span>
      </Label>
      <Input
        id="titulo"
        placeholder="Ej: Mezcla de pistas para EP de 5 canciones"
        className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] focus:border-purple-500 focus:ring-2 focus:ring-purple-500/50"
        {...register("titulo")}
        aria-required="true"
        aria-invalid={!!errors.titulo}
        aria-describedby={errors.titulo ? "titulo-error" : undefined}
      />
      {errors.titulo && (
        <p id="titulo-error" role="alert" className="text-sm text-red-400 mt-1">
          {errors.titulo.message}
        </p>
      )}
    </div>

    {/* Descripcion */}
    <div className="space-y-2">
      <Label htmlFor="descripcion" className="text-sm font-medium text-[#cbd5e1]">
        Descripcion
      </Label>
      <Textarea
        id="descripcion"
        placeholder="Describe los detalles de lo que necesitas..."
        className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[120px]"
        {...register("descripcion")}
        maxLength={4000}
      />
      <span className="text-xs text-[#64748b]">
        {watch("descripcion")?.length || 0} / 4000 caracteres
      </span>
    </div>

    {/* Tipo Necesidad + Modalidad (grid 2 cols) */}
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="space-y-2">
        <Label htmlFor="tipoNecesidadId" className="text-sm font-medium text-[#cbd5e1]">
          Tipo de Necesidad
          <span className="text-red-400 ml-1">*</span>
        </Label>
        <Select
          value={watch("tipoNecesidadId")?.toString()}
          onValueChange={(value) => setValue("tipoNecesidadId", parseInt(value))}
        >
          <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white">
            <SelectValue placeholder="Selecciona el tipo" />
          </SelectTrigger>
          <SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
            {tiposNecesidad?.map(tipo => (
              <SelectItem key={tipo.id} value={tipo.id.toString()}>
                {tipo.nombre}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        {errors.tipoNecesidadId && (
          <p role="alert" className="text-sm text-red-400 mt-1">
            {errors.tipoNecesidadId.message}
          </p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="modalidadTrabajoId" className="text-sm font-medium text-[#cbd5e1]">
          Modalidad de Trabajo
          <span className="text-red-400 ml-1">*</span>
        </Label>
        <Select
          value={watch("modalidadTrabajoId")?.toString()}
          onValueChange={(value) => setValue("modalidadTrabajoId", parseInt(value))}
        >
          <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white">
            <SelectValue placeholder="Selecciona la modalidad" />
          </SelectTrigger>
          <SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
            {modalidadesTrabajo?.map(modalidad => (
              <SelectItem key={modalidad.id} value={modalidad.id.toString()}>
                {modalidad.nombre}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        {errors.modalidadTrabajoId && (
          <p role="alert" className="text-sm text-red-400 mt-1">
            {errors.modalidadTrabajoId.message}
          </p>
        )}
      </div>
    </div>
  </form>
</Card>
```

**Section Card 2: PRESUPUESTO**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Budget Container | `<div>` | `flex flex-col md:flex-row gap-3 items-start md:items-end` |
| Input Presupuesto Min | `<Input type="number">` | `w-full md:w-32 bg-[#1a1a2e] border-[#334155] text-white` |
| Separator | `<span>` | `text-[#64748b] font-medium self-center` "-" |
| Input Presupuesto Max | `<Input type="number">` | `w-full md:w-32 bg-[#1a1a2e] border-[#334155] text-white` |
| Select Moneda | `<Select>` | `w-full md:w-32` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <h2 className="text-lg font-semibold text-white mb-4">
    PRESUPUESTO
  </h2>

  <div className="space-y-2">
    <Label className="text-sm font-medium text-[#cbd5e1]">
      Rango presupuestario (orientativo)
    </Label>
    <div className="flex flex-col md:flex-row gap-3 items-start md:items-end">
      <div className="space-y-1 flex-1 md:flex-none">
        <Label htmlFor="presupuestoMin" className="text-xs text-[#64748b]">
          Minimo
        </Label>
        <Input
          id="presupuestoMin"
          type="number"
          placeholder="0"
          className="w-full md:w-32 bg-[#1a1a2e] border-[#334155] text-white"
          {...register("presupuestoMin", { valueAsNumber: true })}
        />
      </div>

      <span className="text-[#64748b] font-medium self-center hidden md:block">-</span>

      <div className="space-y-1 flex-1 md:flex-none">
        <Label htmlFor="presupuestoMax" className="text-xs text-[#64748b]">
          Maximo
        </Label>
        <Input
          id="presupuestoMax"
          type="number"
          placeholder="0"
          className="w-full md:w-32 bg-[#1a1a2e] border-[#334155] text-white"
          {...register("presupuestoMax", { valueAsNumber: true })}
        />
      </div>

      <div className="space-y-1 flex-1 md:flex-none">
        <Label htmlFor="monedaId" className="text-xs text-[#64748b]">
          Moneda
        </Label>
        <Select
          value={watch("monedaId")?.toString()}
          onValueChange={(value) => setValue("monedaId", parseInt(value))}
        >
          <SelectTrigger className="w-full md:w-32 bg-[#1a1a2e] border-[#334155] text-white">
            <SelectValue placeholder="EUR" />
          </SelectTrigger>
          <SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
            <SelectItem value="1">EUR (€)</SelectItem>
            <SelectItem value="2">USD ($)</SelectItem>
            <SelectItem value="3">GBP (£)</SelectItem>
          </SelectContent>
        </Select>
      </div>
    </div>

    {errors.presupuestoMax && (
      <p role="alert" className="text-sm text-red-400 mt-1">
        {errors.presupuestoMax.message}
      </p>
    )}
  </div>
</Card>
```

**Section Card 3: UBICACION Y FECHAS**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Conditional Location Container | `<div>` | `hidden` si modalidadTrabajoId === 2 (Remoto), `block` si 1 o 3 |
| Grid 2 Cols | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Input Ciudad | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Input Pais | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| DatePicker Container | `<Popover>` | shadcn/ui popover |
| DatePicker Trigger | `<Button variant="outline">` | `bg-[#1a1a2e] border-[#334155] text-white justify-start` |
| Calendar | `<Calendar>` | `bg-[#1a1a2e] border-[#334155]` |
| Select Proyecto | `<Select>` | `w-full` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <h2 className="text-lg font-semibold text-white mb-4">
    UBICACION Y FECHAS
  </h2>

  <div className="space-y-4">
    {/* Ubicacion condicional */}
    {watch("modalidadTrabajoId") !== 2 && (
      <div>
        <Label className="text-sm font-medium text-[#cbd5e1] mb-2">
          Ubicacion (para trabajo presencial/hibrido)
          <span className="text-red-400 ml-1">*</span>
        </Label>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <Input
              placeholder="Ciudad"
              className="bg-[#1a1a2e] border-[#334155] text-white"
              {...register("ubicacionCiudad")}
            />
            {errors.ubicacionCiudad && (
              <p role="alert" className="text-sm text-red-400">
                {errors.ubicacionCiudad.message}
              </p>
            )}
          </div>
          <div className="space-y-2">
            <Input
              placeholder="Pais"
              className="bg-[#1a1a2e] border-[#334155] text-white"
              {...register("ubicacionPais")}
            />
          </div>
        </div>
      </div>
    )}

    {/* Fechas */}
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div className="space-y-2">
        <Label className="text-sm font-medium text-[#cbd5e1]">
          Limite para recibir propuestas
        </Label>
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className={cn(
                "w-full bg-[#1a1a2e] border-[#334155] text-white justify-start",
                !watch("fechaLimitePropuestas") && "text-[#64748b]"
              )}
            >
              <Calendar className="mr-2 h-4 w-4" />
              {watch("fechaLimitePropuestas")
                ? format(new Date(watch("fechaLimitePropuestas")), "dd MMM yyyy", { locale: es })
                : "Seleccionar fecha"
              }
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0 bg-[#1a1a2e] border-[#334155]">
            <Calendar
              mode="single"
              selected={watch("fechaLimitePropuestas") ? new Date(watch("fechaLimitePropuestas")) : undefined}
              onSelect={(date) => setValue("fechaLimitePropuestas", date?.toISOString())}
              disabled={(date) => date < new Date()}
              className="bg-[#1a1a2e] text-white"
            />
          </PopoverContent>
        </Popover>
      </div>

      <div className="space-y-2">
        <Label className="text-sm font-medium text-[#cbd5e1]">
          Fecha de inicio prevista
        </Label>
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className={cn(
                "w-full bg-[#1a1a2e] border-[#334155] text-white justify-start",
                !watch("fechaInicioPrevista") && "text-[#64748b]"
              )}
            >
              <Calendar className="mr-2 h-4 w-4" />
              {watch("fechaInicioPrevista")
                ? format(new Date(watch("fechaInicioPrevista")), "dd MMM yyyy", { locale: es })
                : "Seleccionar fecha"
              }
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0 bg-[#1a1a2e] border-[#334155]">
            <Calendar
              mode="single"
              selected={watch("fechaInicioPrevista") ? new Date(watch("fechaInicioPrevista")) : undefined}
              onSelect={(date) => setValue("fechaInicioPrevista", date?.toISOString())}
              disabled={(date) => date < new Date()}
              className="bg-[#1a1a2e] text-white"
            />
          </PopoverContent>
        </Popover>
      </div>
    </div>

    {/* Proyecto Artistico */}
    <div className="space-y-2">
      <Label htmlFor="proyectoArtisticoId" className="text-sm font-medium text-[#cbd5e1]">
        Proyecto Artistico
        <span className="text-red-400 ml-1">*</span>
      </Label>
      <Select
        value={watch("proyectoArtisticoId")}
        onValueChange={(value) => setValue("proyectoArtisticoId", value)}
      >
        <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white">
          <SelectValue placeholder="Selecciona tu proyecto" />
        </SelectTrigger>
        <SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
          {proyectos?.map(proyecto => (
            <SelectItem key={proyecto.id} value={proyecto.id}>
              {proyecto.nombre}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      {errors.proyectoArtisticoId && (
        <p role="alert" className="text-sm text-red-400 mt-1">
          {errors.proyectoArtisticoId.message}
        </p>
      )}
    </div>
  </div>
</Card>
```

**Form Actions**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Container | `<div>` | `flex justify-between items-center mt-8` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Publicar Button | `<Button type="submit">` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50` |
| Loader Icon | `<Loader2>` | `w-4 h-4 mr-2 animate-spin` (solo si isPending) |

**Composicion:**
```tsx
<div className="flex justify-between items-center mt-8">
  <Button
    type="button"
    variant="outline"
    onClick={handleCancel}
    disabled={isPending}
    className="border-[#334155] text-white hover:bg-[#1e2a42]"
  >
    Cancelar
  </Button>

  <Button
    type="submit"
    disabled={isPending}
    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50"
    aria-busy={isPending}
  >
    {isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />}
    {isPending ? "Publicando..." : "Publicar Necesidad"}
  </Button>
</div>
```

---

### 4.3 EditarNecesidadPage

**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}/editar`

**Nota:** Layout identico a NuevaNecesidadPage con las siguientes diferencias:

1. **Title:** "Editar Necesidad" en lugar de "Publicar Nueva Necesidad"
2. **Form pre-populated:** Todos los campos cargados con datos existentes
3. **Tipo Necesidad DISABLED:**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Select Tipo (disabled) | `<Select disabled>` | `opacity-60 cursor-not-allowed` |
| Tooltip wrapper | `<Tooltip>` | shadcn/ui tooltip |
| TooltipContent | `<TooltipContent>` | `bg-[#1a1a2e] border-[#334155] text-white text-xs` |

**Composicion:**
```tsx
<div className="space-y-2">
  <Label className="text-sm font-medium text-[#cbd5e1]">
    Tipo de Necesidad
    <span className="text-red-400 ml-1">*</span>
  </Label>
  <Tooltip>
    <TooltipTrigger asChild>
      <div>
        <Select disabled value={watch("tipoNecesidadId")?.toString()}>
          <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white opacity-60 cursor-not-allowed">
            <SelectValue />
          </SelectTrigger>
        </Select>
      </div>
    </TooltipTrigger>
    <TooltipContent className="bg-[#1a1a2e] border-[#334155] text-white text-xs max-w-xs">
      No se puede cambiar el tipo de necesidad una vez publicada para no invalidar las propuestas recibidas
    </TooltipContent>
  </Tooltip>
</div>
```

4. **Warning Banner (si tiene propuestas):**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Alert Container | `<Alert>` | `bg-amber-900/20 border-amber-700 mb-6` |
| Alert Icon | `<AlertCircle>` | `w-5 h-5 text-amber-500` |
| Alert Title | `<AlertTitle>` | `text-base font-semibold text-amber-200` |
| Alert Description | `<AlertDescription>` | `text-sm text-amber-300` |

**Composicion:**
```tsx
{numeroPropuestas > 0 && (
  <Alert className="bg-amber-900/20 border-amber-700 mb-6">
    <AlertCircle className="w-5 h-5 text-amber-500" />
    <AlertTitle className="text-base font-semibold text-amber-200">
      Esta necesidad ya tiene {numeroPropuestas} propuestas
    </AlertTitle>
    <AlertDescription className="text-sm text-amber-300">
      Los cambios que realices seran visibles para los profesionales que ya enviaron propuestas.
      Considera el impacto antes de modificar campos importantes como presupuesto o descripcion.
    </AlertDescription>
  </Alert>
)}
```

5. **Submit Button:** Texto "Guardar Cambios" en lugar de "Publicar Necesidad"

---

### 4.4 NecesidadDetailPage

**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}`

#### Layout General

```
┌─────────────────────────────────────────────────────────┐
│  [Sidebar]                                              │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Back Link                                         │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Header Card                                       │  │
│  │   Title + Estado Badge + Actions Menu            │  │
│  │   [Editar] [Cerrar Necesidad]                    │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Details Card: DETALLES                            │  │
│  │   Descripcion                                     │  │
│  │   Info Grid (Tipo, Modalidad, Presupuesto...)    │  │
│  └───────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────┐  │
│  │ Propuestas Card: PROPUESTAS RECIBIDAS (N)         │  │
│  │   [PropuestaCard 1]                              │  │
│  │   [PropuestaCard 2]                              │  │
│  │   ...                                             │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

#### Componentes Detallados

**Header Card**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Title Row | `<div>` | `flex items-center justify-between mb-4` |
| Title | `<h1>` | `text-2xl font-bold text-white flex-1` |
| Estado Badge | `<EstadoBadge>` | (ver componente 4.7) |
| Actions Menu | `<DropdownMenu>` | shadcn/ui dropdown |
| Menu Trigger | `<Button variant="ghost" size="icon">` | `text-white hover:bg-[#1e2a42]` |
| MoreVertical Icon | `<MoreVertical>` | `w-5 h-5` |
| Menu Content | `<DropdownMenuContent>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Menu Items | `<DropdownMenuItem>` | `focus:bg-[#1e2a42]` |
| Action Buttons Row | `<div>` | `flex gap-3` |
| Editar Button | `<Button variant="outline">` | `border-[#334155] text-white` (solo si estado = Abierta) |
| Cerrar Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <div className="flex items-center justify-between mb-4">
    <h1 className="text-2xl font-bold text-white flex-1">
      {necesidad.titulo}
    </h1>
    <div className="flex items-center gap-3">
      <EstadoBadge estado={necesidad.estadoNecesidadId} />

      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" size="icon" className="text-white hover:bg-[#1e2a42]">
            <MoreVertical className="w-5 h-5" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent className="bg-[#1a1a2e] border-[#334155] text-white">
          {necesidad.estadoNecesidadId === 1 && (
            <DropdownMenuItem
              onClick={handleEditar}
              className="focus:bg-[#1e2a42]"
            >
              <Edit className="w-4 h-4 mr-2" />
              Editar Necesidad
            </DropdownMenuItem>
          )}
          {[1, 2].includes(necesidad.estadoNecesidadId) && (
            <DropdownMenuItem
              onClick={handleCerrar}
              className="focus:bg-[#1e2a42] text-red-400"
            >
              <XCircle className="w-4 h-4 mr-2" />
              Cerrar Necesidad
            </DropdownMenuItem>
          )}
          <DropdownMenuItem className="focus:bg-[#1e2a42]">
            <Share className="w-4 h-4 mr-2" />
            Compartir
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  </div>

  <div className="flex gap-3">
    {necesidad.estadoNecesidadId === 1 && (
      <Button
        variant="outline"
        onClick={handleEditar}
        className="border-[#334155] text-white hover:bg-[#1e2a42]"
      >
        <Edit className="w-4 h-4 mr-2" />
        Editar
      </Button>
    )}

    {[1, 2].includes(necesidad.estadoNecesidadId) && (
      <Button
        variant="destructive"
        onClick={handleCerrar}
        className="bg-red-600 hover:bg-red-700"
      >
        <XCircle className="w-4 h-4 mr-2" />
        Cerrar Necesidad
      </Button>
    )}
  </div>
</Card>
```

**Details Card**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Description | `<p>` | `text-base text-[#94a3b8] mb-4 whitespace-pre-line` |
| Info Grid | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Info Item | `<div>` | `flex flex-col gap-1` |
| Info Label | `<span>` | `text-sm text-[#64748b]` |
| Info Value | `<span>` | `text-base text-white` |
| Separator | `<Separator>` | `my-4 bg-[#334155]` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
  <h2 className="text-lg font-semibold text-white mb-4">
    DETALLES
  </h2>

  {necesidad.descripcion && (
    <>
      <div className="mb-4">
        <span className="text-sm text-[#64748b] mb-2 block">Descripcion:</span>
        <p className="text-base text-[#94a3b8] whitespace-pre-line">
          {necesidad.descripcion}
        </p>
      </div>
      <Separator className="my-4 bg-[#334155]" />
    </>
  )}

  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
    <div className="flex flex-col gap-1">
      <span className="text-sm text-[#64748b]">Tipo:</span>
      <span className="text-base text-white flex items-center gap-2">
        <Music2 className="w-4 h-4" />
        {necesidad.tipoNecesidadNombre}
      </span>
    </div>

    <div className="flex flex-col gap-1">
      <span className="text-sm text-[#64748b]">Modalidad:</span>
      <span className="text-base text-white flex items-center gap-2">
        {necesidad.modalidadTrabajoId === 2 ? (
          <Globe className="w-4 h-4" />
        ) : (
          <MapPin className="w-4 h-4" />
        )}
        {necesidad.modalidadTrabajoNombre}
      </span>
    </div>

    <div className="flex flex-col gap-1">
      <span className="text-sm text-[#64748b]">Presupuesto:</span>
      <span className="text-base text-white font-medium">
        {formatPresupuesto(
          necesidad.presupuestoMin,
          necesidad.presupuestoMax,
          necesidad.monedaId
        )}
      </span>
    </div>

    <div className="flex flex-col gap-1">
      <span className="text-sm text-[#64748b]">Proyecto:</span>
      <span className="text-base text-white">
        {necesidad.proyectoArtisticoNombre}
      </span>
    </div>

    {necesidad.ubicacionCiudad && (
      <div className="flex flex-col gap-1">
        <span className="text-sm text-[#64748b]">Ubicacion:</span>
        <span className="text-base text-white">
          {necesidad.ubicacionCiudad}, {necesidad.ubicacionPais}
        </span>
      </div>
    )}

    <div className="flex flex-col gap-1">
      <span className="text-sm text-[#64748b]">Publicado:</span>
      <span className="text-base text-white">
        {format(new Date(necesidad.fechaCreacion), "dd MMM yyyy", { locale: es })}
        <span className="text-sm text-[#64748b] ml-2">
          ({formatDistanceToNow(new Date(necesidad.fechaCreacion), { addSuffix: true, locale: es })})
        </span>
      </span>
    </div>

    {necesidad.fechaActualizacion && (
      <div className="flex flex-col gap-1">
        <span className="text-sm text-[#64748b]">Ultima actualizacion:</span>
        <span className="text-base text-white">
          {format(new Date(necesidad.fechaActualizacion), "dd MMM yyyy", { locale: es })}
        </span>
      </div>
    )}

    {necesidad.fechaLimitePropuestas && (
      <div className="flex flex-col gap-1">
        <span className="text-sm text-[#64748b]">Limite propuestas:</span>
        <span className="text-base text-white">
          {format(new Date(necesidad.fechaLimitePropuestas), "dd MMM yyyy", { locale: es })}
          {(() => {
            const dias = differenceInDays(new Date(necesidad.fechaLimitePropuestas), new Date());
            return dias > 0 ? ` (${dias} dias)` : ' (expirado)';
          })()}
        </span>
      </div>
    )}

    {necesidad.fechaInicioPrevista && (
      <div className="flex flex-col gap-1">
        <span className="text-sm text-[#64748b]">Inicio previsto:</span>
        <span className="text-base text-white">
          {format(new Date(necesidad.fechaInicioPrevista), "dd MMM yyyy", { locale: es })}
        </span>
      </div>
    )}
  </div>
</Card>
```

**Propuestas Card**

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Card Container | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Header Row | `<div>` | `flex items-center justify-between mb-4` |
| Section Title | `<h2>` | `text-lg font-semibold text-white` |
| Propuestas Count Badge | `<Badge>` | `text-base` |
| PropuestaCard (ver 4.6) | Custom component | Multiple instances |
| Empty State | `<div>` | `text-center py-8` |

**Composicion:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6">
  <div className="flex items-center justify-between mb-4">
    <h2 className="text-lg font-semibold text-white">
      PROPUESTAS RECIBIDAS
    </h2>
    <Badge variant="secondary" className="text-base">
      {propuestas.length}
    </Badge>
  </div>

  {propuestas.length === 0 ? (
    <div className="text-center py-8">
      <Inbox className="w-12 h-12 text-[#64748b] mx-auto mb-3" aria-hidden="true" />
      <p className="text-base text-[#94a3b8]">
        Aun no hay propuestas para esta necesidad
      </p>
      <p className="text-sm text-[#64748b] mt-2">
        Las propuestas de profesionales apareceran aqui
      </p>
    </div>
  ) : (
    <div className="space-y-3">
      {propuestas.map((propuesta) => (
        <PropuestaCard
          key={propuesta.id}
          propuesta={propuesta}
          onVerPerfil={handleVerPerfil}
          onAceptar={necesidad.estadoNecesidadId === 1 ? handleAceptar : undefined}
          onRechazar={necesidad.estadoNecesidadId === 1 ? handleRechazar : undefined}
          readonly={necesidad.estadoNecesidadId !== 1}
          presupuestoMin={necesidad.presupuestoMin}
          presupuestoMax={necesidad.presupuestoMax}
        />
      ))}
    </div>
  )}
</Card>
```

---

### 4.5 CerrarNecesidadDialog

**Contexto:** Modal dialog overlay

#### Layout

```
┌────────────────────────────────────────────┐
│  Cerrar Necesidad                     [×]  │
├────────────────────────────────────────────┤
│                                            │
│  ⚠ Alert Warning                           │
│  Al cerrar esta necesidad, las propuestas │
│  pendientes seran rechazadas...            │
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

#### Componentes Detallados

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Dialog Root | `<Dialog>` | `open={isOpen} onOpenChange={setIsOpen}` |
| Dialog Content | `<DialogContent>` | `max-w-md bg-[#0f1729] border-[#334155]` |
| Dialog Header | `<DialogHeader>` | Default |
| Dialog Title | `<DialogTitle>` | `text-xl font-semibold text-white` |
| Warning Alert | `<Alert>` | `bg-amber-900/20 border-amber-700 mb-4` |
| Alert Icon | `<AlertCircle>` | `w-5 h-5 text-amber-500` |
| Alert Text | `<p>` | `text-sm text-amber-300` |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` |
| Textarea Motivo | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[100px]` |
| Character Counter | `<span>` | `text-xs text-[#64748b]` (rojo si > 500) |
| Dialog Footer | `<DialogFooter>` | `flex gap-3 justify-end` |
| Cancelar Button | `<Button variant="outline">` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Cerrar Button | `<Button variant="destructive">` | `bg-red-600 hover:bg-red-700 disabled:opacity-50` |
| Loader Icon | `<Loader2>` | `w-4 h-4 mr-2 animate-spin` (si isPending) |

**Composicion:**
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent
    className="max-w-md bg-[#0f1729] border-[#334155]"
    role="dialog"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
  >
    <DialogHeader>
      <DialogTitle id="dialog-title" className="text-xl font-semibold text-white">
        Cerrar Necesidad
      </DialogTitle>
    </DialogHeader>

    <Alert className="bg-amber-900/20 border-amber-700 mb-4">
      <AlertCircle className="w-5 h-5 text-amber-500" />
      <p id="dialog-description" className="text-sm text-amber-300">
        Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente.
        Esta accion no se puede deshacer.
      </p>
    </Alert>

    <div className="space-y-2">
      <Label htmlFor="motivo" className="text-sm font-medium text-[#cbd5e1]">
        Motivo del cierre (opcional)
      </Label>
      <Textarea
        id="motivo"
        placeholder="Describe brevemente por que cierras esta necesidad..."
        className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[100px]"
        value={motivo}
        onChange={(e) => setMotivo(e.target.value)}
        maxLength={500}
        disabled={isPending}
      />
      <span className={cn(
        "text-xs",
        motivo.length > 500 ? "text-red-400" : "text-[#64748b]"
      )}>
        {motivo.length} / 500 caracteres
      </span>
    </div>

    <DialogFooter className="flex gap-3 justify-end mt-6">
      <Button
        type="button"
        variant="outline"
        onClick={() => setIsOpen(false)}
        disabled={isPending}
        className="border-[#334155] text-white hover:bg-[#1e2a42]"
      >
        Cancelar
      </Button>
      <Button
        type="button"
        variant="destructive"
        onClick={handleCerrar}
        disabled={isPending}
        className="bg-red-600 hover:bg-red-700 disabled:opacity-50"
        aria-busy={isPending}
      >
        {isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />}
        {isPending ? "Cerrando..." : "Cerrar Necesidad"}
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>
```

---

### 4.6 PropuestaCard (Componente Custom)

**Props:**
```typescript
interface PropuestaCardProps {
  propuesta: PropuestaCrowdsourcing;
  onVerPerfil: (profesionalId: string) => void;
  onAceptar?: (propuestaId: string) => void;
  onRechazar?: (propuestaId: string) => void;
  readonly?: boolean;
  presupuestoMin?: number;
  presupuestoMax?: number;
}
```

| Elemento | Componente | Props/Classes |
|----------|------------|---------------|
| Container | `<Card>` | `bg-[#16213e] border-[#334155] p-4 mb-3` |
| Profesional Row | `<div>` | `flex items-center gap-3 mb-3` |
| Avatar | `<Avatar>` | `w-10 h-10` |
| Avatar Image | `<AvatarImage>` | src con fallback |
| Avatar Fallback | `<AvatarFallback>` | Iniciales del profesional |
| Info Column | `<div>` | `flex-1` |
| Profesional Name | `<span>` | `text-base font-semibold text-white` |
| Profesional Meta | `<div>` | `text-sm text-[#94a3b8]` |
| Rating Display | `<span>` | `flex items-center gap-1` con icono Star + score + count |
| Mensaje | `<p>` | `text-sm text-[#94a3b8] mb-3` (expandible) |
| Mensaje Truncated | Conditional | Truncado a 150 chars con "Leer mas" link |
| Meta Row | `<div>` | `flex items-center gap-4 text-sm text-[#64748b] mb-3` |
| Precio Badge | `<Badge variant="secondary">` | `text-white font-semibold` |
| Fuera Presupuesto Badge | `<Badge variant="outline">` | `border-amber-500 text-amber-400` (si precio < min o > max) |
| Dias Badge | `<Badge variant="outline">` | Con icono Clock |
| Fecha Envio | `<span>` | Con icono Calendar |
| Actions Row | `<div>` | `flex gap-2 pt-3 border-t border-[#334155]` |
| Ver Perfil Button | `<Button variant="ghost" size="sm">` | `text-purple-400 hover:text-purple-300` |
| Aceptar Button | `<Button variant="default" size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600` (solo si !readonly) |
| Rechazar Button | `<Button variant="ghost" size="sm">` | `text-red-400 hover:text-red-300` (solo si !readonly) |

**Composicion:**
```tsx
const PropuestaCard: FC<PropuestaCardProps> = ({
  propuesta,
  onVerPerfil,
  onAceptar,
  onRechazar,
  readonly,
  presupuestoMin,
  presupuestoMax
}) => {
  const [isExpanded, setIsExpanded] = useState(false);
  const isTruncated = propuesta.mensaje.length > 150;
  const displayMensaje = isExpanded ? propuesta.mensaje : propuesta.mensaje.substring(0, 150);

  const fueraPresupuesto = (presupuestoMin && propuesta.precioPropuesto < presupuestoMin) ||
                           (presupuestoMax && propuesta.precioPropuesto > presupuestoMax);

  return (
    <Card className="bg-[#16213e] border-[#334155] p-4">
      <div className="flex items-center gap-3 mb-3">
        <Avatar className="w-10 h-10">
          <AvatarImage src={`/avatars/${propuesta.profesionalId}.jpg`} />
          <AvatarFallback className="bg-purple-600 text-white">
            {propuesta.profesionalNombre.split(' ').map(n => n[0]).join('')}
          </AvatarFallback>
        </Avatar>

        <div className="flex-1">
          <span className="text-base font-semibold text-white block">
            {propuesta.profesionalNombre}
          </span>
          <div className="text-sm text-[#94a3b8] flex items-center gap-2">
            <span>Profesional</span>
            {/* Rating placeholder - implementar cuando haya datos de rating */}
            <span className="flex items-center gap-1">
              <Star className="w-3 h-3 fill-yellow-400 text-yellow-400" />
              <span>4.8 (12)</span>
            </span>
          </div>
        </div>
      </div>

      <p className="text-sm text-[#94a3b8] mb-3">
        {displayMensaje}
        {isTruncated && !isExpanded && "..."}
        {isTruncated && (
          <button
            onClick={() => setIsExpanded(!isExpanded)}
            className="text-purple-400 hover:text-purple-300 ml-1"
          >
            {isExpanded ? "Ver menos" : "Leer mas"}
          </button>
        )}
      </p>

      <div className="flex flex-wrap items-center gap-3 text-sm text-[#64748b] mb-3">
        <Badge variant="secondary" className="text-white font-semibold">
          {propuesta.precioPropuesto} {MONEDA_SIMBOLOS[propuesta.monedaId]}
        </Badge>

        {fueraPresupuesto && (
          <Badge variant="outline" className="border-amber-500 text-amber-400">
            <AlertTriangle className="w-3 h-3 mr-1" />
            Fuera del rango presupuestario
          </Badge>
        )}

        {propuesta.tiempoEstimadoDias && (
          <Badge variant="outline" className="flex items-center gap-1">
            <Clock className="w-3 h-3" />
            {propuesta.tiempoEstimadoDias} dias
          </Badge>
        )}

        <span className="flex items-center gap-1">
          <Calendar className="w-3 h-3" />
          Enviado: {format(new Date(propuesta.fechaCreacion), "dd MMM yyyy", { locale: es })}
        </span>
      </div>

      {!readonly && (onAceptar || onRechazar) && (
        <div className="flex gap-2 pt-3 border-t border-[#334155]">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => onVerPerfil(propuesta.profesionalId)}
            className="text-purple-400 hover:text-purple-300"
          >
            <User className="w-4 h-4 mr-1" />
            Ver Perfil
          </Button>

          {onAceptar && (
            <Button
              variant="default"
              size="sm"
              onClick={() => onAceptar(propuesta.id)}
              className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
            >
              <Check className="w-4 h-4 mr-1" />
              Aceptar
            </Button>
          )}

          {onRechazar && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => onRechazar(propuesta.id)}
              className="text-red-400 hover:text-red-300"
            >
              <X className="w-4 h-4 mr-1" />
              Rechazar
            </Button>
          )}
        </div>
      )}

      {readonly && (
        <div className="pt-3 border-t border-[#334155]">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => onVerPerfil(propuesta.profesionalId)}
            className="text-purple-400 hover:text-purple-300"
          >
            <User className="w-4 h-4 mr-1" />
            Ver Perfil del Profesional
          </Button>
        </div>
      )}
    </Card>
  );
};
```

---

### 4.7 EstadoBadge (Componente Custom)

**Props:**
```typescript
interface EstadoBadgeProps {
  estado: number; // EstadoNecesidadId
}
```

| Estado ID | Label | Classes |
|-----------|-------|---------|
| 1 | Abierta | `bg-green-900/20 text-green-400 border-green-700` |
| 2 | En Progreso | `bg-blue-900/20 text-blue-400 border-blue-700` |
| 3 | Cerrada | `bg-gray-900/20 text-gray-400 border-gray-700` |
| 4 | Cancelada | `bg-red-900/20 text-red-400 border-red-700` |

**Composicion:**
```tsx
const EstadoBadge: FC<EstadoBadgeProps> = ({ estado }) => {
  const config = {
    1: {
      label: 'Abierta',
      className: 'bg-green-900/20 text-green-400 border-green-700',
      icon: CheckCircle
    },
    2: {
      label: 'En Progreso',
      className: 'bg-blue-900/20 text-blue-400 border-blue-700',
      icon: Clock
    },
    3: {
      label: 'Cerrada',
      className: 'bg-gray-900/20 text-gray-400 border-gray-700',
      icon: XCircle
    },
    4: {
      label: 'Cancelada',
      className: 'bg-red-900/20 text-red-400 border-red-700',
      icon: Ban
    },
  };

  const { label, className, icon: Icon } = config[estado] || config[3];

  return (
    <Badge
      variant="outline"
      className={cn("flex items-center gap-1", className)}
      aria-label={`Estado: ${label}`}
    >
      <Icon className="w-3 h-3" aria-hidden="true" />
      {label}
    </Badge>
  );
};
```

---

## 5. Responsive Design

### Breakpoints

| Breakpoint | Width | Layout Changes |
|------------|-------|----------------|
| **Mobile** | < 768px | - Cards 1 col stack<br>- Filtros vertical<br>- Form inputs full width<br>- Actions stack vertical<br>- Grid becomes flex column |
| **Tablet** | 768px - 1024px | - Cards 2 cols grid<br>- Filtros horizontal<br>- Form grid 2 cols parcial<br>- Sidebar colapsable |
| **Desktop** | > 1024px | - Cards 3 cols grid (si espacio)<br>- Sidebar fijo<br>- Form grid 2 cols completo<br>- Hover effects completos |

### Mobile-Specific Adjustments

**MisNecesidadesPage:**
```tsx
{/* Filters - vertical en mobile */}
<div className="flex flex-col md:flex-row gap-4">
  {/* Estado select */}
  {/* Search input */}
</div>

{/* Cards grid - 1 col mobile, 2 cols tablet, 3 cols desktop */}
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  {/* NecesidadCard */}
</div>
```

**NuevaNecesidadPage / EditarNecesidadPage:**
```tsx
{/* Tipo + Modalidad - stack mobile, 2 cols desktop */}
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
  {/* Tipo */}
  {/* Modalidad */}
</div>

{/* Presupuesto - stack mobile, inline desktop */}
<div className="flex flex-col md:flex-row gap-3">
  {/* Min */}
  {/* Max */}
  {/* Moneda */}
</div>

{/* Actions - stack mobile, justify-between desktop */}
<div className="flex flex-col md:flex-row gap-3 md:justify-between">
  <Button className="w-full md:w-auto">Cancelar</Button>
  <Button className="w-full md:w-auto">Guardar</Button>
</div>
```

**NecesidadDetailPage:**
```tsx
{/* Info grid - 1 col mobile, 2 cols desktop */}
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
  {/* Info items */}
</div>

{/* Actions - stack mobile */}
<div className="flex flex-col md:flex-row gap-3">
  {/* Buttons */}
</div>
```

---

## 6. Animaciones y Transiciones

| Elemento | Animacion | Duracion | Timing |
|----------|-----------|----------|--------|
| **Card hover** | `transition-all` + bg-color + border-color + scale(1.01) + shadow-md | 200ms | ease-out |
| **Badge appear** | Fade in opacity | 150ms | ease |
| **Form validation error** | Shake animation (translateX -4px to 4px) + border-red | 400ms | ease |
| **Button hover** | Background gradient shift + scale(1.02) | 150ms | ease |
| **Input focus** | Border color to purple + ring-2 purple/50 + shadow glow | 200ms | ease |
| **Dialog open** | Backdrop fade (opacity 0 → 1) + content scale (0.95 → 1) | 200ms | ease-out |
| **Toast notification** | Slide in from top + fade | 250ms | ease-out |
| **Loading spinner** | Rotate 360deg infinite (animate-spin) | 1000ms | linear |
| **Skeleton shimmer** | Background position shift (shimmer effect) | 1500ms | ease-in-out infinite |
| **Propuestas list** | Stagger animation (cada item +50ms delay) | 200ms | ease-out |
| **Character counter warning** | Color transition gray → red at limit | 200ms | ease |

### Custom Animations (Tailwind Config)

```typescript
// tailwind.config.ts
module.exports = {
  theme: {
    extend: {
      keyframes: {
        shake: {
          '0%, 100%': { transform: 'translateX(0)' },
          '25%': { transform: 'translateX(-4px)' },
          '75%': { transform: 'translateX(4px)' },
        },
        shimmer: {
          '0%': { backgroundPosition: '-200% 0' },
          '100%': { backgroundPosition: '200% 0' },
        },
      },
      animation: {
        shake: 'shake 400ms ease',
        shimmer: 'shimmer 1500ms ease-in-out infinite',
      },
    },
  },
};
```

**Uso:**
```tsx
{/* Error shake en input */}
<Input className={cn(errors.titulo && "animate-shake border-red-400")} />

{/* Shimmer en skeleton */}
<Skeleton className="animate-shimmer bg-gradient-to-r from-[#1e2a42] via-[#2a3652] to-[#1e2a42] bg-[length:200%_100%]" />
```

---

## 7. Accesibilidad (ARIA)

### Principios WCAG AA

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de color** | Minimo 4.5:1 para texto normal. White (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 |
| **Focus visible** | Ring de 2px purple con offset en todos los interactivos (`focus:ring-2 focus:ring-purple-500/50 focus:ring-offset-2`) |
| **Labels en inputs** | Todos los inputs con `<Label htmlFor>` asociado |
| **Required fields** | Marcados con "*" visual + `aria-required="true"` |
| **Form validation** | Mensajes de error con `role="alert"` + `aria-live="polite"` + `aria-invalid` en inputs con error |
| **Loading states** | `aria-busy="true"` durante submits, spinner con `aria-hidden="true"` |
| **Estado badges** | `aria-label="Estado: {estado}"` para screen readers |
| **Date pickers** | Navegables con teclado (arrows, enter, escape) |
| **Dialogs** | `role="dialog"` + `aria-labelledby` + `aria-describedby` + focus trap |
| **Cards navegables** | `tabindex="0"` + `role="article"` + Enter/Space para activar |
| **Propuestas count** | `aria-label="{N} propuestas recibidas"` |
| **Fecha limite warning** | `aria-label="Fecha limite proxima: {fecha}"` si < 7 dias |

### ARIA Labels Ejemplos

**NecesidadCard:**
```tsx
<Card
  tabIndex={0}
  role="article"
  aria-label={`${necesidad.titulo}. Estado: ${necesidad.estadoNecesidadNombre}. ${necesidad.numeroPropuestas} propuestas. Presupuesto ${formatPresupuesto(necesidad.presupuestoMin, necesidad.presupuestoMax, necesidad.monedaId)}`}
  onClick={handleClick}
  onKeyDown={(e) => (e.key === 'Enter' || e.key === ' ') && handleClick()}
>
```

**Form Input con Error:**
```tsx
<Input
  id="titulo"
  aria-required="true"
  aria-invalid={!!errors.titulo}
  aria-describedby={errors.titulo ? "titulo-error" : undefined}
/>
{errors.titulo && (
  <p id="titulo-error" role="alert" className="text-sm text-red-400 mt-1">
    {errors.titulo.message}
  </p>
)}
```

**Estado Badge:**
```tsx
<Badge aria-label={`Estado: ${estadoNombre}`}>
  <Icon aria-hidden="true" />
  {estadoNombre}
</Badge>
```

**Loading Button:**
```tsx
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin mr-2" aria-hidden="true" />}
  {isPending ? "Publicando..." : "Publicar Necesidad"}
</Button>
```

**Dialog:**
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen}>
  <DialogContent
    role="dialog"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
  >
    <DialogTitle id="dialog-title">Cerrar Necesidad</DialogTitle>
    <DialogDescription id="dialog-description">
      Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente.
    </DialogDescription>
  </DialogContent>
</Dialog>
```

**Propuestas Count:**
```tsx
<div aria-label={`${count} propuestas recibidas`}>
  <Users className="w-4 h-4" aria-hidden="true" />
  <Badge>{count}</Badge>
</div>
```

---

## 8. Toast Messages

| Accion | Tipo | Mensaje | Duracion |
|--------|------|---------|----------|
| **Crear necesidad - success** | Success | "Necesidad publicada correctamente" | 3s |
| **Crear necesidad - error** | Error | "Error al publicar la necesidad. Intentalo de nuevo." | 5s |
| **Editar necesidad - success** | Success | "Necesidad actualizada correctamente" | 3s |
| **Editar necesidad - error 400** | Error | "No se puede editar una necesidad cerrada o en progreso" | 5s |
| **Editar necesidad - error general** | Error | "Error al actualizar. Intentalo de nuevo." | 5s |
| **Cerrar necesidad - success** | Success | "Necesidad cerrada. Las propuestas pendientes han sido rechazadas." | 4s |
| **Cerrar necesidad - error** | Error | "Error al cerrar la necesidad. Intentalo de nuevo." | 5s |
| **Aceptar propuesta - success** | Success | "Propuesta aceptada. Se ha creado el acuerdo de trabajo." | 4s |
| **Aceptar propuesta - error** | Error | "Error al aceptar la propuesta. Intentalo de nuevo." | 5s |
| **Rechazar propuesta - success** | Success | "Propuesta rechazada" | 3s |
| **Rechazar propuesta - error** | Error | "Error al rechazar la propuesta. Intentalo de nuevo." | 5s |
| **Acceso denegado** | Error | "No tienes permiso para acceder a esta necesidad" | 5s |
| **No encontrada** | Error | "Necesidad no encontrada" | 5s |

**Implementacion con Sonner:**
```tsx
import { toast } from 'sonner';

// Success
toast.success("Necesidad publicada correctamente", {
  duration: 3000,
});

// Error
toast.error("Error al publicar la necesidad", {
  description: "Verifica los datos e intentalo de nuevo",
  duration: 5000,
});
```

---

## 9. Loading Skeletons

### NecesidadCard Skeleton

```tsx
<Card className="p-6 mb-4 bg-[#0f1729] border-[#334155]">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-6 w-32 bg-[#1e2a42]" />
  </div>
  <Skeleton className="h-7 w-3/4 mb-3 bg-[#1e2a42]" />
  <div className="flex gap-3 mb-2">
    <Skeleton className="h-5 w-24 bg-[#1e2a42]" />
    <Skeleton className="h-5 w-20 bg-[#1e2a42]" />
  </div>
  <Skeleton className="h-5 w-32 mb-3 bg-[#1e2a42]" />
  <div className="flex gap-4 mb-4">
    <Skeleton className="h-4 w-24 bg-[#1e2a42]" />
    <Skeleton className="h-4 w-28 bg-[#1e2a42]" />
  </div>
  <div className="flex gap-2 pt-3 border-t border-[#334155]">
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
    <Skeleton className="h-8 w-20 bg-[#1e2a42]" />
  </div>
</Card>
```

### Form Skeleton (Create/Edit)

```tsx
<Card className="p-6 mb-6 bg-[#0f1729] border-[#334155]">
  <Skeleton className="h-6 w-48 mb-4 bg-[#1e2a42]" /> {/* Section title */}
  <div className="space-y-4">
    <div>
      <Skeleton className="h-4 w-16 mb-2 bg-[#1e2a42]" /> {/* Label */}
      <Skeleton className="h-10 w-full bg-[#1e2a42]" /> {/* Input */}
    </div>
    <div>
      <Skeleton className="h-4 w-24 mb-2 bg-[#1e2a42]" />
      <Skeleton className="h-24 w-full bg-[#1e2a42]" /> {/* Textarea */}
    </div>
    <div className="grid grid-cols-2 gap-4">
      <div>
        <Skeleton className="h-4 w-32 mb-2 bg-[#1e2a42]" />
        <Skeleton className="h-10 w-full bg-[#1e2a42]" />
      </div>
      <div>
        <Skeleton className="h-4 w-24 mb-2 bg-[#1e2a42]" />
        <Skeleton className="h-10 w-full bg-[#1e2a42]" />
      </div>
    </div>
  </div>
</Card>
```

### Detalle Header Skeleton

```tsx
<Card className="p-6 mb-6 bg-[#0f1729] border-[#334155]">
  <div className="flex items-start justify-between mb-4">
    <Skeleton className="h-8 w-1/2 bg-[#1e2a42]" /> {/* Titulo */}
    <Skeleton className="h-6 w-24 bg-[#1e2a42]" /> {/* Badge */}
  </div>
  <div className="flex gap-3">
    <Skeleton className="h-9 w-24 bg-[#1e2a42]" /> {/* Botones */}
    <Skeleton className="h-9 w-32 bg-[#1e2a42]" />
  </div>
</Card>
```

### Details Card Skeleton

```tsx
<Card className="p-6 mb-6 bg-[#0f1729] border-[#334155]">
  <Skeleton className="h-6 w-32 mb-4 bg-[#1e2a42]" />
  <Skeleton className="h-20 w-full mb-4 bg-[#1e2a42]" /> {/* Descripcion */}
  <div className="grid grid-cols-2 gap-4">
    {[1, 2, 3, 4].map(i => (
      <div key={i}>
        <Skeleton className="h-4 w-20 mb-1 bg-[#1e2a42]" />
        <Skeleton className="h-5 w-32 bg-[#1e2a42]" />
      </div>
    ))}
  </div>
</Card>
```

---

## 10. Customizaciones Especificas

### DatePicker Dark Theme

shadcn/ui Calendar component requiere estilos custom para dark theme:

```tsx
// components/ui/calendar.tsx - agregar clases custom
<Calendar
  className={cn(
    "bg-[#1a1a2e] text-white border-[#334155]",
    "[&_.rdp-day_button]:text-white",
    "[&_.rdp-day_button:hover]:bg-purple-600/20",
    "[&_.rdp-day_selected]:bg-purple-600",
    "[&_.rdp-day_today]:bg-purple-900/30",
    "[&_.rdp-head_cell]:text-[#94a3b8]",
    className
  )}
  {...props}
/>
```

### Select Dark Theme

```tsx
<SelectContent className="bg-[#1a1a2e] border-[#334155] text-white">
  <SelectItem
    value="1"
    className="focus:bg-[#1e2a42] focus:text-white"
  >
    Opcion 1
  </SelectItem>
</SelectContent>
```

### DropdownMenu Dark Theme

```tsx
<DropdownMenuContent className="bg-[#1a1a2e] border-[#334155] text-white">
  <DropdownMenuItem className="focus:bg-[#1e2a42] focus:text-white">
    <Edit className="w-4 h-4 mr-2" />
    Editar
  </DropdownMenuItem>
</DropdownMenuContent>
```

---

## 11. Checklist Final UI/UX

### Pantalla 1: Mis Necesidades (Listado)
- [ ] Layout de dashboard con sidebar
- [ ] Header con titulo + subtitulo + botones [Nueva Necesidad] [Usar Plantilla]
- [ ] Filtros card: estado select + search input con debounce 300ms
- [ ] Grid responsive (1/2/3 cols segun breakpoint)
- [ ] NecesidadCard con: estado badge, titulo, tipo + icono, modalidad + icono, presupuesto, propuestas count + badge, fechas
- [ ] Estado badges colores correctos (green/blue/gray/red)
- [ ] Fecha limite con warning (< 7 dias amber, < 3 dias red)
- [ ] Actions condicionales: Editar (solo Abierta), Cerrar (Abierta/En Progreso)
- [ ] Card hover effect (bg, border, shadow, scale)
- [ ] Empty state con ilustracion + mensaje + CTAs
- [ ] Loading skeletons (3-4 cards con shimmer)
- [ ] Paginacion funcional
- [ ] Click card → detalle, click botones → acciones
- [ ] Responsive mobile: stack 1 col, filtros vertical
- [ ] ARIA labels, tabindex, role="article"
- [ ] Focus states visibles

### Pantalla 2: Crear Necesidad
- [ ] Form completo con 3 secciones (Info Basica, Presupuesto, Ubicacion/Fechas)
- [ ] Todos los inputs con labels + placeholders + validacion
- [ ] Required fields marcados con "*" + aria-required
- [ ] Tipo Necesidad populated desde maestras
- [ ] Modalidad con opciones: Presencial, Remoto, Hibrido
- [ ] Ubicacion condicional: visible solo si Presencial/Hibrido (hidden si Remoto)
- [ ] Presupuesto: min - max + moneda select
- [ ] DatePickers para fecha limite y fecha inicio (solo >= hoy)
- [ ] Proyecto Artistico select populated
- [ ] Validacion tiempo real: blur, min < max, required
- [ ] Character counter descripcion (max 4000)
- [ ] Botones: [Cancelar] [Publicar Necesidad]
- [ ] Loading state submit (spinner + disabled)
- [ ] Toast success: "Necesidad publicada correctamente"
- [ ] Redirect a listado tras success
- [ ] Dialog confirmacion si form dirty al cancelar
- [ ] Responsive: 1 col mobile, 2 cols desktop
- [ ] Form validation Zod + React Hook Form
- [ ] ARIA labels, invalid states, error messages

### Pantalla 3: Editar Necesidad
- [ ] Same layout que Crear, titulo "Editar Necesidad"
- [ ] Form pre-rellenado con datos existentes
- [ ] Tipo Necesidad DISABLED con tooltip explicativo
- [ ] Banner warning si tiene propuestas: "Esta necesidad ya tiene N propuestas..."
- [ ] Validacion igual que Crear (excepto Tipo)
- [ ] Boton "Guardar Cambios" (no "Publicar")
- [ ] Loading fetch data (skeleton)
- [ ] PUT a API
- [ ] Toast success: "Necesidad actualizada"
- [ ] Error 400 si estado != Abierta: toast + redirect
- [ ] Redirect a detalle tras success

### Pantalla 4: Detalle de Necesidad
- [ ] Back link + titulo + estado badge + actions menu
- [ ] Actions: [Editar] (solo Abierta), [Cerrar] (Abierta/En Progreso)
- [ ] Card detalles: descripcion + info grid (tipo, modalidad, presupuesto, proyecto, fechas)
- [ ] Card propuestas: header con count + lista
- [ ] PropuestaCard: avatar, nombre, rol, rating (placeholder), mensaje expandible, precio, dias, fecha
- [ ] Actions propuesta: [Ver Perfil] [Aceptar] [Rechazar] (solo si Abierta)
- [ ] Empty state propuestas: "Aun no hay propuestas..."
- [ ] Badge "Fuera del rango" si precio < min o > max
- [ ] Propuestas readonly si estado != Abierta
- [ ] Click Aceptar → dialog confirmacion (futuro US-CS-04)
- [ ] Click Rechazar → dialog con motivo
- [ ] Responsive: info grid 1 col mobile, 2 cols desktop
- [ ] Loading skeletons
- [ ] ARIA labels

### Pantalla 5: Dialogo de Cierre
- [ ] Dialog shadcn/ui
- [ ] Titulo: "Cerrar Necesidad"
- [ ] Alert warning: "Al cerrar esta necesidad..."
- [ ] Textarea motivo (opcional, max 500 chars)
- [ ] Character counter
- [ ] Botones: [Cancelar] [Cerrar Necesidad] (destructive)
- [ ] Loading state submit
- [ ] Toast success: "Necesidad cerrada..."
- [ ] Dialog cierra tras success
- [ ] Focus trap
- [ ] Escape key cierra dialog

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Design tokens usados (no hardcoded)
- [ ] shadcn/ui components correctos
- [ ] Tailwind utilities (no CSS custom)
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Focus states con ring purple
- [ ] ARIA labels, roles, live regions
- [ ] Keyboard navigation completa
- [ ] Loading states con aria-busy
- [ ] Mobile-first responsive
- [ ] Toast notifications consistentes
- [ ] Form validation Zod + React Hook Form
- [ ] TanStack Query para data fetching
- [ ] Error handling con toasts
- [ ] Success flows con redirects
- [ ] Debounce en search (300ms)

---

## 12. Componentes shadcn/ui Faltantes

Todos los componentes necesarios YA ESTAN instalados:

- [x] Card
- [x] Badge
- [x] Button
- [x] Input
- [x] Textarea
- [x] Select
- [x] Label
- [x] Dialog
- [x] Alert
- [x] Separator
- [x] Popover
- [x] Calendar
- [x] DropdownMenu
- [x] Skeleton
- [x] Sonner (Toast)
- [x] Avatar
- [x] Tooltip

**NO se requiere instalacion de componentes adicionales.**

---

**Archivo creado:** `C:\Repos\WePlay_Rises\plans\cs-gestionar-necesidades\frontend-admin\ui-design.md`

**Componentes shadcn/ui utilizados:** 15 componentes base (Card, Badge, Button, Input, Textarea, Select, Label, Dialog, Alert, Separator, Popover, Calendar, DropdownMenu, Skeleton, Avatar, Tooltip)

**Componentes custom a desarrollar:**
1. `NecesidadCard` - Card de necesidad con estado, meta info, acciones condicionales
2. `PropuestaCard` - Card de propuesta con avatar, profesional info, mensaje expandible, acciones
3. `EstadoBadge` - Badge de estado con colores semanticos (green/blue/gray/red)
4. `CerrarNecesidadDialog` - Dialog de confirmacion con warning alert + textarea motivo
5. `MisNecesidadesPage` - Listado con filtros + grid + paginacion
6. `NuevaNecesidadPage` - Formulario creacion con secciones + campos condicionales
7. `EditarNecesidadPage` - Formulario edicion con banner warning + tipo disabled
8. `NecesidadDetailPage` - Detalle con propuestas + actions condicionales

**Consideraciones de accesibilidad:**
- Todos los componentes con ARIA labels adecuados
- Focus states visibles con ring purple
- Contraste minimo 4.5:1 (white sobre dark bg = 15.8:1)
- Navegacion por teclado completa (tab, enter, space, escape)
- Form validation con role="alert" + aria-invalid
- Loading states con aria-busy
- Dialogs con focus trap + labelledby + describedby
- Cards navegables con tabindex + role="article"

**Design tokens aplicados:**
- Dark theme: bg-[#0f1729], bg-[#1a1a2e], border-[#334155]
- Primary gradient: from-pink-500 to-purple-600
- Text: white, #94a3b8, #64748b, #cbd5e1
- Status colors: green (abierta), blue (progreso), gray (cerrada), red (cancelada)
- Warning: amber-900/20 border-amber-700
