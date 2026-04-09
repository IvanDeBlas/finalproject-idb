# Diseno UI: Templates y Guia para Artistas Noveles (Admin)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** src/admin (Next.js 14)

---

## 1. Resumen

- **Componentes shadcn/ui:** 18 componentes (Table, Card, Input, Select, Checkbox, Dialog, Badge, Button, Skeleton, Alert, Tooltip, Separator, DropdownMenu, AlertDialog, Textarea, Label, Tabs, Progress)
- **Composiciones custom:** 6 (TemplateTable, TemplateForm, NeedFormItem, NeedsList, TemplateFilters, DeleteConfirmDialog)
- **Screens:** 2 (Template List, Create/Edit Template)
- **Responsive breakpoints:** Mobile (< 640px), Tablet (640-1024px), Desktop (> 1024px)
- **Dark theme:** Si (design tokens de ui-ux.md)

**Objetivo:** CRUD completo de templates de proyecto con gestion dinamica de necesidades profesionales.

---

## 2. Paleta de Colores (Dark Theme)

| Uso | Variable | Hex | Aplicacion |
|-----|----------|-----|------------|
| Background Primary | `--bg-primary` | #1a1a2e | Fondo principal de pagina |
| Background Secondary | `--bg-secondary` | #16213e | Fondo de sidebar |
| Background Card | `--bg-card` | #0f1729 | Fondo de cards, inputs, table |
| Background Card Hover | `--bg-card-hover` | #1e2a42 | Hover en rows, cards |
| Primary Color | `--primary-color` | #a855f7 | Botones, borders focus, links |
| Primary Gradient | `--primary-gradient` | linear-gradient(135deg, #ec4899 0%, #a855f7 100%) | Botones principales |
| Text Primary | `--text-primary` | #ffffff | Texto principal |
| Text Secondary | `--text-secondary` | #94a3b8 | Texto descriptivo |
| Text Muted | `--text-muted` | #64748b | Placeholders |
| Text Label | `--text-label` | #cbd5e1 | Labels de formularios |
| Status Success | `--status-success` | #10b981 | Badge "Activo", toasts exito |
| Status Error | `--status-error` | #ef4444 | Badge "Inactivo", errores |
| Status Warning | `--status-warning` | #f59e0b | Prioridad "Recomendado" |
| Priority Esencial | `--priority-esencial` | #ef4444 | Badge prioridad "Esencial" |
| Priority Recomendado | `--priority-recomendado` | #f59e0b | Badge prioridad "Recomendado" |
| Priority Opcional | `--priority-opcional` | #64748b | Badge prioridad "Opcional" |
| Border Primary | `--border-primary` | #334155 | Borders de inputs, cards |
| Border Focus | `--border-focus` | #a855f7 | Borders en focus |
| Border Error | `--border-error` | #ef4444 | Borders con error de validacion |
| Input Background | `--input-bg` | #0f1729 | Fondo de inputs |
| Input Disabled | `--input-bg-disabled` | #1e293b | Fondo de inputs disabled |

**Tailwind equivalents:**
```tsx
// Backgrounds
className="bg-[#1a1a2e]"    // bg-primary
className="bg-[#0f1729]"    // bg-card
className="hover:bg-[#1e2a42]"  // bg-card-hover

// Borders
className="border-[#334155]"  // border-primary
className="focus:border-[#a855f7]"  // border-focus

// Text
className="text-white"        // text-primary
className="text-[#94a3b8]"    // text-secondary
className="text-[#64748b]"    // text-muted
```

---

## 3. Componentes por Screen

### 3.1 Screen: Template List (Admin)

**Ruta:** `/dashboard/crowdsourcing/templates`
**Layout:** Dashboard layout con sidebar + contenido principal

#### Layout Visual

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
│             │  │ [Buscar...]          [Activos / Todos ▾]     ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  ┌──────────────────────────────────────────────┐│
│             │  │ Nombre        │Neces.│ Precio   │Estado│... ││
│             │  ├────────────────────────────────────────────────│
│             │  │ Grabar Album/EP  11   1.9k-10k  🟢 Activo   ││
│             │  │ [Editar] [Ver necesidades] [Desactivar]     ││
│             │  ├────────────────────────────────────────────────│
│             │  │ Videoclip        11   2k-20k    🟢 Activo   ││
│             │  │ [Editar] [Ver necesidades] [Desactivar]     ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [< Anterior] Pagina 1 de 2 [Siguiente >]        │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

#### Componentes

**Page Header**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `mb-6` |
| Titulo | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Subtitulo | `<p>` | `text-lg text-[#94a3b8]` |

**Action Bar**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `flex justify-between items-center mb-6` |
| New Button | `Button` | `variant="default" className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"` |
| Button Icon | Lucide `Plus` | `w-4 h-4 mr-2` |

**Filters Card**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `Card` | `bg-[#0f1729] border-[#334155] p-4 mb-4` |
| Inner Flex | `<div>` | `flex gap-4 flex-wrap` |
| Search Input | `Input` | `w-full md:w-64 bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| Status Select | `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | `w-full md:w-48` |

**Templates Table**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Table Container | `Card` | `bg-[#0f1729] border-[#334155]` |
| Table | `Table` | - |
| Table Header | `TableHeader` | `bg-[#16213e]` |
| Header Row | `TableRow` | - |
| Header Cell | `TableHead` | `text-[#cbd5e1] font-semibold` |
| Table Body | `TableBody` | - |
| Body Row | `TableRow` | `hover:bg-[#1e2a42] transition-colors cursor-pointer` |
| Body Cell - Nombre | `TableCell` | `font-medium text-white` |
| Body Cell - Necesidades | `TableCell` | `text-[#94a3b8] text-center` |
| Body Cell - Precio | `TableCell` | `text-[#94a3b8]` |
| Body Cell - Estado | `TableCell` | - |
| Status Badge | `Badge` | `variant="default"` (Activo) o `variant="secondary"` (Inactivo) |
| Actions Cell | `TableCell` | `flex gap-2` |
| Edit Button | `Button` | `variant="ghost" size="sm" className="text-primary hover:text-primary/80"` |
| View Button | `Button` | `variant="ghost" size="sm" className="text-[#94a3b8] hover:text-white"` |
| Toggle Button | `Button` | `variant="ghost" size="sm" className="text-[#64748b] hover:text-white"` |

**Table Columns:**
| Column | Header | Width | Align | Content |
|--------|--------|-------|-------|---------|
| Nombre | "Nombre" | flex-1 | left | `{template.nombre}` |
| Necesidades | "Neces." | 100px | center | `{template.cantidadNecesidades}` |
| Precio | "Precio" | 150px | left | `{formatCurrency(min)}-{formatCurrency(max)}` |
| Estado | "Estado" | 120px | center | Badge con "Activo" o "Inactivo" |
| Acciones | "" | 300px | right | Botones Editar, Ver, Toggle |

**Pagination**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | `<div>` | `flex justify-center items-center gap-4 mt-6` |
| Prev Button | `Button` | `variant="outline" size="sm" disabled={currentPage === 1}` |
| Page Info | `<span>` | `text-sm text-[#94a3b8]` "Pagina X de Y" |
| Next Button | `Button` | `variant="outline" size="sm" disabled={currentPage === totalPages}` |

**Composicion completa:**

```tsx
<div className="p-6">
    {/* Header */}
    <div className="mb-6">
        <h1 className="text-3xl font-bold text-white mb-2">
            Templates de Proyecto
        </h1>
        <p className="text-lg text-[#94a3b8]">
            Gestiona las plantillas para artistas noveles
        </p>
    </div>

    {/* Action Bar */}
    <div className="flex justify-between items-center mb-6">
        <Button
            onClick={handleCreateNew}
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
        >
            <Plus className="w-4 h-4 mr-2" />
            Nuevo Template
        </Button>
    </div>

    {/* Filters */}
    <Card className="bg-[#0f1729] border-[#334155] p-4 mb-4">
        <div className="flex gap-4 flex-wrap">
            <Input
                placeholder="Buscar templates..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full md:w-64 bg-[#1a1a2e] border-[#334155] text-white"
            />
            <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger className="w-full md:w-48">
                    <SelectValue placeholder="Todos" />
                </SelectTrigger>
                <SelectContent>
                    <SelectItem value="all">Todos</SelectItem>
                    <SelectItem value="active">Activos</SelectItem>
                    <SelectItem value="inactive">Inactivos</SelectItem>
                </SelectContent>
            </Select>
        </div>
    </Card>

    {/* Table */}
    <Card className="bg-[#0f1729] border-[#334155]">
        <Table>
            <TableHeader className="bg-[#16213e]">
                <TableRow>
                    <TableHead className="text-[#cbd5e1] font-semibold">Nombre</TableHead>
                    <TableHead className="text-[#cbd5e1] font-semibold text-center">Neces.</TableHead>
                    <TableHead className="text-[#cbd5e1] font-semibold">Precio</TableHead>
                    <TableHead className="text-[#cbd5e1] font-semibold text-center">Estado</TableHead>
                    <TableHead className="text-[#cbd5e1] font-semibold text-right">Acciones</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {templates.map((template) => (
                    <TableRow key={template.id} className="hover:bg-[#1e2a42] transition-colors">
                        <TableCell className="font-medium text-white">
                            {template.nombre}
                        </TableCell>
                        <TableCell className="text-[#94a3b8] text-center">
                            {template.cantidadNecesidades}
                        </TableCell>
                        <TableCell className="text-[#94a3b8]">
                            {formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}
                        </TableCell>
                        <TableCell className="text-center">
                            <Badge variant={template.activo ? "default" : "secondary"}>
                                {template.activo ? "Activo" : "Inactivo"}
                            </Badge>
                        </TableCell>
                        <TableCell className="text-right">
                            <div className="flex gap-2 justify-end">
                                <Button variant="ghost" size="sm" className="text-primary hover:text-primary/80">
                                    <Edit className="w-4 h-4 mr-1" />
                                    Editar
                                </Button>
                                <Button variant="ghost" size="sm" className="text-[#94a3b8] hover:text-white">
                                    <Eye className="w-4 h-4 mr-1" />
                                    Ver necesidades
                                </Button>
                                <Button variant="ghost" size="sm" className="text-[#64748b] hover:text-white">
                                    {template.activo ? "Desactivar" : "Activar"}
                                </Button>
                            </div>
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    </Card>

    {/* Pagination */}
    <div className="flex justify-center items-center gap-4 mt-6">
        <Button variant="outline" size="sm" disabled={currentPage === 1}>
            <ChevronLeft className="w-4 h-4 mr-1" />
            Anterior
        </Button>
        <span className="text-sm text-[#94a3b8]">
            Pagina {currentPage} de {totalPages}
        </span>
        <Button variant="outline" size="sm" disabled={currentPage === totalPages}>
            Siguiente
            <ChevronRight className="w-4 h-4 ml-1" />
        </Button>
    </div>
</div>
```

---

### 3.2 Screen: Create/Edit Template (Admin)

**Ruta:** `/dashboard/crowdsourcing/templates/nuevo` o `/dashboard/crowdsourcing/templates/{id}/editar`
**Layout:** Form con 2 secciones (Datos Generales + Necesidades)

#### Layout Visual

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
│             │  │ [____] (posicion en galeria)                ││
│             │  │                                              ││
│             │  │ [✓] Activo                                   ││
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
│             │  │ │ [X Eliminar]                             │ ││
│             │  │ └──────────────────────────────────────────┘ ││
│             │  │ ...                                          ││
│             │  └──────────────────────────────────────────────┘│
│             │                                                  │
│             │  [Cancelar] [Guardar template]                   │
│             │                                                  │
└────────────────────────────────────────────────────────────────┘
```

#### Componentes

**Page Header**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Back Link | `<Link>` (Next.js) | `flex items-center gap-2 text-primary hover:underline mb-4` |
| Back Icon | Lucide `ChevronLeft` | `w-4 h-4` |
| Page Title | `<h1>` | `text-3xl font-bold text-white mb-6` |

**Section: Datos Generales**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Section Card | `Card` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-xl font-semibold text-white mb-4` |
| Form Container | `<form>` | `space-y-4` |
| Field Label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Text Input | `Input` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| Textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px]` |
| Icon Select | `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | - |
| Order Input | `Input` | `type="number" w-24` |
| Active Checkbox | `Checkbox` + `Label` | - |
| Error Message | `<p>` | `text-sm text-[#ef4444] mt-1` |

**Icon Select con preview:**

```tsx
<div className="space-y-2">
    <Label>Icono *</Label>
    <div className="flex gap-4 items-center">
        <Select value={selectedIcon} onValueChange={setSelectedIcon}>
            <SelectTrigger className="w-48">
                <SelectValue placeholder="Selecciona icono" />
            </SelectTrigger>
            <SelectContent>
                <SelectItem value="music">🎵 Music</SelectItem>
                <SelectItem value="video">🎬 Video</SelectItem>
                <SelectItem value="route">🗺️ Route</SelectItem>
                <SelectItem value="speaker">📢 Speaker</SelectItem>
                <SelectItem value="music-note">🎶 Music Note</SelectItem>
                <SelectItem value="palette">🎨 Palette</SelectItem>
            </SelectContent>
        </Select>
        {selectedIcon && (
            <div className="text-4xl">
                {ICON_MAP[selectedIcon]}
            </div>
        )}
    </div>
</div>
```

**Section: Necesidades**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Section Card | `Card` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section Title | `<h2>` | `text-xl font-semibold text-white mb-4` |
| Add Button | `Button` | `variant="outline" className="border-[#334155] text-white hover:bg-[#1e2a42] mb-4"` |
| Needs Container | `<div>` | `space-y-3` |
| Need Item Card | `Card` | `bg-[#16213e] border-[#334155] p-4 relative` |
| Delete Button | `Button` | `variant="ghost" size="sm" absolute top-2 right-2 text-red-500 hover:text-red-400` |
| Need Field Row | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4 mb-3` |
| Price Row | `<div>` | `flex gap-2 items-center` |
| Price Separator | `<span>` | `text-[#64748b]` "-" |
| Currency Label | `<span>` | `text-sm text-[#94a3b8]` "EUR" |

**Need Form Item (dentro del Card):**

```tsx
<Card className="bg-[#16213e] border-[#334155] p-4 relative">
    {/* Delete button */}
    <Button
        variant="ghost"
        size="sm"
        className="absolute top-2 right-2 text-red-500 hover:text-red-400"
        onClick={() => handleDeleteNeed(index)}
    >
        <X className="w-4 h-4" />
    </Button>

    {/* Fields */}
    <div className="space-y-4">
        {/* Row 1: Fase + Titulo */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
                <Label>Fase *</Label>
                <Input
                    placeholder="PRE-PRODUCCION"
                    value={need.fase}
                    onChange={(e) => handleFieldChange(index, 'fase', e.target.value)}
                    className="bg-[#1a1a2e] border-[#334155] text-white"
                />
            </div>
            <div>
                <Label>Titulo *</Label>
                <Input
                    placeholder="Composicion y arreglos"
                    value={need.titulo}
                    onChange={(e) => handleFieldChange(index, 'titulo', e.target.value)}
                    className="bg-[#1a1a2e] border-[#334155] text-white"
                />
            </div>
        </div>

        {/* Row 2: Rol */}
        <div>
            <Label>Rol Profesional *</Label>
            <Select value={need.rolProfesionalId} onValueChange={(v) => handleFieldChange(index, 'rolProfesionalId', v)}>
                <SelectTrigger>
                    <SelectValue placeholder="Selecciona rol" />
                </SelectTrigger>
                <SelectContent>
                    {rolesProfesionales.map((rol) => (
                        <SelectItem key={rol.id} value={rol.id.toString()}>
                            {rol.nombre}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
        </div>

        {/* Row 3: Presupuesto Min - Max */}
        <div>
            <Label>Presupuesto Orientativo</Label>
            <div className="flex gap-2 items-center mt-2">
                <Input
                    type="number"
                    placeholder="Min"
                    value={need.precioMin}
                    onChange={(e) => handleFieldChange(index, 'precioMin', parseFloat(e.target.value))}
                    className="w-28 bg-[#1a1a2e] border-[#334155] text-white"
                />
                <span className="text-[#64748b]">-</span>
                <Input
                    type="number"
                    placeholder="Max"
                    value={need.precioMax}
                    onChange={(e) => handleFieldChange(index, 'precioMax', parseFloat(e.target.value))}
                    className="w-28 bg-[#1a1a2e] border-[#334155] text-white"
                />
                <span className="text-sm text-[#94a3b8]">EUR</span>
            </div>
        </div>

        {/* Row 4: Prioridad */}
        <div>
            <Label>Prioridad *</Label>
            <Select value={need.prioridad} onValueChange={(v) => handleFieldChange(index, 'prioridad', v)}>
                <SelectTrigger>
                    <SelectValue placeholder="Selecciona prioridad" />
                </SelectTrigger>
                <SelectContent>
                    <SelectItem value="Alta">🔴 Esencial</SelectItem>
                    <SelectItem value="Media">🟡 Recomendado</SelectItem>
                    <SelectItem value="Baja">⚪ Opcional</SelectItem>
                </SelectContent>
            </Select>
        </div>
    </div>
</Card>
```

**Form Actions**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Actions Container | `<div>` | `flex gap-4 justify-end mt-8` |
| Cancel Button | `Button` | `variant="outline" className="border-[#334155] text-white hover:bg-[#1e2a42]"` |
| Save Button | `Button` | `className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"` |
| Loading Spinner | Lucide `Loader2` | `className="animate-spin w-4 h-4 mr-2"` (dentro de Save Button) |

---

## 4. Formularios

### 4.1 Template Form (Create/Edit)

**Campos - Datos Generales:**

| Campo | Tipo | Componente | Validacion Visual | Requerido |
|-------|------|------------|-------------------|-----------|
| nombre | text | Input | FormMessage con error si vacio | Si |
| descripcion | textarea | Textarea | - | No |
| icono | select | Select | FormMessage si no selecciona | Si |
| orden | number | Input | - | No (default: 0) |
| activo | boolean | Checkbox | - | No (default: true) |

**Campos - Necesidad (array dinamico):**

| Campo | Tipo | Componente | Validacion Visual | Requerido |
|-------|------|------------|-------------------|-----------|
| fase | text | Input | FormMessage si vacio | Si |
| titulo | text | Input | FormMessage si vacio | Si |
| rolProfesionalId | select | Select | FormMessage si no selecciona | Si |
| precioMin | number | Input | Border rojo si negativo | No |
| precioMax | number | Input | Border rojo si < precioMin | No |
| prioridad | select | Select (Alta/Media/Baja) | FormMessage si no selecciona | Si |

**Layout:**
- Stack vertical con `space-y-4`
- Labels arriba del input
- Errores debajo del input con FormMessage
- Inputs con fondo oscuro y border focus en purple

**Estados:**

| Estado | Visual |
|--------|--------|
| Default | Border gris (#334155) |
| Focus | Border purple (#a855f7) + ring purple |
| Error | Border rojo (#ef4444) + mensaje rojo debajo |
| Disabled | Opacity 50% + fondo gris oscuro |
| Loading Submit | Boton muestra spinner + texto "Guardando...", form disabled |

**Validaciones en Tiempo Real:**

```tsx
// Zod schema
const templateFormSchema = z.object({
    nombre: z.string().min(1, "El nombre es obligatorio").max(200, "Maximo 200 caracteres"),
    descripcion: z.string().max(500, "Maximo 500 caracteres").optional(),
    icono: z.string().min(1, "Selecciona un icono"),
    orden: z.number().int().nonnegative("Debe ser un numero positivo").optional(),
    activo: z.boolean(),
    necesidades: z.array(
        z.object({
            fase: z.string().min(1, "La fase es obligatoria").max(100),
            titulo: z.string().min(1, "El titulo es obligatorio").max(200),
            rolProfesionalId: z.number().int().positive("Selecciona un rol profesional"),
            precioMin: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
            precioMax: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
            prioridad: z.enum(["Alta", "Media", "Baja"], {
                errorMap: () => ({ message: "Selecciona la prioridad" })
            }),
        }).refine(
            (data) => {
                if (data.precioMin !== undefined && data.precioMax !== undefined) {
                    return data.precioMax >= data.precioMin;
                }
                return true;
            },
            {
                message: "El precio maximo debe ser mayor o igual al minimo",
                path: ["precioMax"],
            }
        )
    ).min(1, "Debe agregar al menos una necesidad"),
});
```

---

## 5. Tablas (Admin)

### 5.1 Templates Table

**Columnas:**

| Header | Width | Align | Sortable | Content |
|--------|-------|-------|----------|---------|
| Nombre | flex-1 | left | Si | `{template.nombre}` |
| Neces. | 100px | center | Si | `{template.cantidadNecesidades}` |
| Precio | 150px | left | No | `{formatCurrency(min)}-{formatCurrency(max)}` |
| Estado | 120px | center | Si | Badge "Activo" / "Inactivo" |
| Acciones | 300px | right | No | Botones Editar, Ver, Toggle |

**Componentes:**
- `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableCell` (shadcn/ui)
- `Badge` para estado (variant="default" si activo, variant="secondary" si inactivo)
- `Button` variant="ghost" para acciones inline

**Responsive:**
- Desktop (> 1024px): Tabla completa con todas las columnas
- Tablet (640-1024px): Ocultar columna "Precio"
- Mobile (< 640px): Convertir tabla a cards apiladas verticalmente

**Mobile Card Layout:**

```tsx
// En mobile, cada row se convierte en:
<Card className="bg-[#0f1729] border-[#334155] p-4 mb-3">
    <div className="flex justify-between items-start mb-2">
        <h3 className="font-medium text-white">{template.nombre}</h3>
        <Badge variant={template.activo ? "default" : "secondary"}>
            {template.activo ? "Activo" : "Inactivo"}
        </Badge>
    </div>
    <div className="text-sm text-[#94a3b8] mb-3">
        <div>Necesidades: {template.cantidadNecesidades}</div>
        <div>Precio: {formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}</div>
    </div>
    <div className="flex gap-2">
        <Button variant="outline" size="sm" className="flex-1">Editar</Button>
        <Button variant="outline" size="sm" className="flex-1">Ver</Button>
        <Button variant="ghost" size="sm">{template.activo ? "Desactivar" : "Activar"}</Button>
    </div>
</Card>
```

**Empty State:**

```tsx
<div className="flex flex-col items-center justify-center py-16">
    <div className="text-6xl mb-4">📋</div>
    <p className="text-lg text-[#94a3b8] mb-2">
        No hay templates disponibles
    </p>
    <p className="text-sm text-[#64748b] mb-6">
        Crea el primero para comenzar
    </p>
    <Button
        onClick={handleCreateNew}
        className="bg-gradient-to-r from-pink-500 to-purple-600"
    >
        <Plus className="w-4 h-4 mr-2" />
        Nuevo Template
    </Button>
</div>
```

**Filtered Empty State:**

```tsx
<div className="flex flex-col items-center justify-center py-16">
    <div className="text-6xl mb-4">🔍</div>
    <p className="text-lg text-[#94a3b8] mb-2">
        No se encontraron templates
    </p>
    <p className="text-sm text-[#64748b] mb-6">
        Intenta con otros criterios de busqueda
    </p>
    <Button
        variant="outline"
        onClick={handleClearFilters}
    >
        Limpiar filtros
    </Button>
</div>
```

---

## 6. Dialogos/Modales

### 6.1 Delete Confirmation Dialog

**Trigger:** Click en boton "Desactivar" en tabla

**Componente:** `AlertDialog` + `AlertDialogTrigger` + `AlertDialogContent` + `AlertDialogHeader` + `AlertDialogFooter`

**Contenido:**

```tsx
<AlertDialog>
    <AlertDialogTrigger asChild>
        <Button variant="ghost" size="sm" className="text-[#64748b] hover:text-white">
            Desactivar
        </Button>
    </AlertDialogTrigger>
    <AlertDialogContent className="bg-[#0f1729] border-[#334155]">
        <AlertDialogHeader>
            <AlertDialogTitle className="text-white">
                ¿Desactivar template?
            </AlertDialogTitle>
            <AlertDialogDescription className="text-[#94a3b8]">
                Este template dejara de aparecer en la galeria para artistas.
                Podras reactivarlo en cualquier momento.
            </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
            <AlertDialogCancel className="border-[#334155] text-white hover:bg-[#1e2a42]">
                Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
                onClick={handleConfirmToggle}
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
            >
                Si, desactivar
            </AlertDialogAction>
        </AlertDialogFooter>
    </AlertDialogContent>
</AlertDialog>
```

**Variant para Activar:**

```tsx
<AlertDialogDescription className="text-[#94a3b8]">
    Este template volvera a aparecer en la galeria para artistas.
</AlertDialogDescription>
```

### 6.2 Unsaved Changes Dialog

**Trigger:** Click en "Cancelar" o navegacion cuando hay cambios sin guardar

**Contenido:**

```tsx
<AlertDialog open={showUnsavedDialog} onOpenChange={setShowUnsavedDialog}>
    <AlertDialogContent className="bg-[#0f1729] border-[#334155]">
        <AlertDialogHeader>
            <AlertDialogTitle className="text-white">
                ¿Descartar cambios?
            </AlertDialogTitle>
            <AlertDialogDescription className="text-[#94a3b8]">
                Tienes cambios sin guardar. Si sales ahora, se perderan.
            </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
            <AlertDialogCancel className="border-[#334155] text-white hover:bg-[#1e2a42]">
                Seguir editando
            </AlertDialogCancel>
            <AlertDialogAction
                onClick={handleDiscardChanges}
                className="bg-red-600 hover:bg-red-700"
            >
                Descartar cambios
            </AlertDialogAction>
        </AlertDialogFooter>
    </AlertDialogContent>
</AlertDialog>
```

### 6.3 View Needs Modal (Ver necesidades)

**Trigger:** Click en "Ver necesidades" en tabla

**Componente:** `Dialog` + `DialogTrigger` + `DialogContent` + `DialogHeader` + `DialogFooter`

**Contenido:**

```tsx
<Dialog>
    <DialogTrigger asChild>
        <Button variant="ghost" size="sm" className="text-[#94a3b8] hover:text-white">
            <Eye className="w-4 h-4 mr-1" />
            Ver necesidades
        </Button>
    </DialogTrigger>
    <DialogContent className="bg-[#0f1729] border-[#334155] max-w-2xl max-h-[80vh] overflow-y-auto">
        <DialogHeader>
            <DialogTitle className="text-white">
                {template.nombre}
            </DialogTitle>
            <DialogDescription className="text-[#94a3b8]">
                {template.cantidadNecesidades} necesidades profesionales
            </DialogDescription>
        </DialogHeader>

        {/* Agrupar por fase */}
        <div className="space-y-6 py-4">
            {Object.entries(groupedNeeds).map(([fase, needs]) => (
                <div key={fase}>
                    <h3 className="text-sm font-semibold text-[#94a3b8] uppercase tracking-wide mb-3">
                        {fase}
                    </h3>
                    <div className="space-y-2">
                        {needs.map((need) => (
                            <div key={need.id} className="flex items-start gap-3 p-3 bg-[#16213e] rounded-lg">
                                <div className="flex-1">
                                    <div className="flex items-center gap-2 mb-1">
                                        <span className="font-medium text-white">{need.titulo}</span>
                                        <Badge
                                            variant="outline"
                                            className={cn(
                                                need.prioridad === "Alta" && "border-red-500 text-red-500",
                                                need.prioridad === "Media" && "border-yellow-500 text-yellow-500",
                                                need.prioridad === "Baja" && "border-gray-500 text-gray-500"
                                            )}
                                        >
                                            {need.prioridad}
                                        </Badge>
                                    </div>
                                    <div className="text-sm text-[#94a3b8]">
                                        {need.rolProfesional.nombre}
                                    </div>
                                    <div className="text-sm text-[#64748b] mt-1">
                                        {formatCurrency(need.precioMinOrientativo)} - {formatCurrency(need.precioMaxOrientativo)}
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            ))}
        </div>

        <DialogFooter>
            <Button variant="outline" onClick={() => setShowDialog(false)}>
                Cerrar
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

## 7. Feedback y Estados

### Loading States

**Table Loading (Skeleton):**

```tsx
<TableBody>
    {[...Array(5)].map((_, i) => (
        <TableRow key={i}>
            <TableCell><Skeleton className="h-4 w-48" /></TableCell>
            <TableCell><Skeleton className="h-4 w-12 mx-auto" /></TableCell>
            <TableCell><Skeleton className="h-4 w-32" /></TableCell>
            <TableCell><Skeleton className="h-6 w-16 mx-auto" /></TableCell>
            <TableCell><Skeleton className="h-8 w-full" /></TableCell>
        </TableRow>
    ))}
</TableBody>
```

**Form Loading (Edit mode):**

```tsx
{isLoading ? (
    <Card className="bg-[#0f1729] border-[#334155] p-6">
        <Skeleton className="h-8 w-48 mb-4" />
        <Skeleton className="h-10 w-full mb-4" />
        <Skeleton className="h-24 w-full mb-4" />
        <Skeleton className="h-10 w-48 mb-4" />
    </Card>
) : (
    // Render form
)}
```

**Button Loading State:**

```tsx
<Button disabled={isPending}>
    {isPending && <Loader2 className="animate-spin w-4 h-4 mr-2" />}
    {isPending ? "Guardando..." : "Guardar template"}
</Button>
```

### Success States

**Toast Notifications (usando sonner):**

```tsx
import { toast } from "sonner";

// Success
toast.success("Template creado correctamente", {
    description: "El template ya esta disponible para artistas",
});

// Update
toast.success("Template actualizado", {
    description: "Los cambios se han guardado correctamente",
});

// Toggle status
toast.success(`Template ${activo ? "activado" : "desactivado"}`, {
    description: activo
        ? "El template ya esta visible en la galeria"
        : "El template ya no aparece en la galeria",
});
```

### Error States

**Toast Error:**

```tsx
toast.error("Error al guardar template", {
    description: "Intenta de nuevo o contacta a soporte",
});

// Con detalle del error
toast.error("Error de validacion", {
    description: errorMessage || "Revisa los campos marcados en rojo",
});
```

**Form Field Error:**

```tsx
<div>
    <Label htmlFor="nombre">Nombre *</Label>
    <Input
        id="nombre"
        {...register("nombre")}
        className={cn(
            "bg-[#1a1a2e] border-[#334155] text-white",
            errors.nombre && "border-red-500"
        )}
    />
    {errors.nombre && (
        <p className="text-sm text-red-500 mt-1">
            {errors.nombre.message}
        </p>
    )}
</div>
```

### Empty States

Ver seccion 5.1 "Templates Table" para empty states de tabla.

---

## 8. Responsive Design

### Breakpoints y Cambios

| Breakpoint | Width | Cambios Principales |
|------------|-------|---------------------|
| **Mobile** | < 640px | - Tabla → Cards apiladas<br>- Filtros stack vertical<br>- Inputs full width<br>- Acciones dentro de cards<br>- Form: 1 columna<br>- Dialog: full screen |
| **Tablet** | 640-1024px | - Tabla: ocultar columna "Precio"<br>- Filtros en 2 columnas<br>- Form: 2 columnas para algunos fields<br>- Sidebar colapsable |
| **Desktop** | > 1024px | - Tabla completa con todas las columnas<br>- Filtros horizontales<br>- Form: 2 columnas para fields relacionados<br>- Sidebar fijo |

### Clases Tailwind Responsive

**Tabla → Cards (mobile):**

```tsx
{/* Desktop: mostrar tabla */}
<div className="hidden md:block">
    <Table>...</Table>
</div>

{/* Mobile: mostrar cards */}
<div className="md:hidden space-y-3">
    {templates.map((template) => (
        <Card key={template.id}>...</Card>
    ))}
</div>
```

**Filtros:**

```tsx
<div className="flex flex-col md:flex-row gap-4">
    <Input className="w-full md:w-64" />
    <Select className="w-full md:w-48" />
</div>
```

**Form Fields (2 columnas en desktop):**

```tsx
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
    <div>
        <Label>Fase *</Label>
        <Input />
    </div>
    <div>
        <Label>Titulo *</Label>
        <Input />
    </div>
</div>
```

**Dialog (full screen en mobile):**

```tsx
<DialogContent className="bg-[#0f1729] border-[#334155] w-full max-w-2xl sm:max-w-lg md:max-w-2xl">
    {/* En mobile sera casi full screen, en desktop max-w-2xl */}
</DialogContent>
```

---

## 9. Accesibilidad

### Requisitos WCAG AA

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de color** | - Texto blanco (#fff) sobre #1a1a2e = 15.8:1 ✓<br>- Texto secundario (#94a3b8) sobre #1a1a2e = 7.2:1 ✓<br>- Primary (#a855f7) sobre #0f1729 = 5.1:1 ✓ |
| **Focus visible** | - Ring de 2px en primary (#a855f7) con offset de 2px<br>- `focus:ring-2 focus:ring-primary focus:ring-offset-2` |
| **Labels en inputs** | - Todos los inputs con `<Label htmlFor="id">`<br>- Vinculos explicitos con `id` matching |
| **Form validation** | - Mensajes con `role="alert"` y `aria-live="polite"`<br>- `aria-describedby` vinculando input con mensaje de error |
| **Keyboard navigation** | - Todos los botones accesibles con Tab<br>- Enter/Space para activar<br>- Esc para cerrar dialogos |
| **Screen readers** | - `aria-label` descriptivos en botones de accion<br>- `aria-labelledby` en dialogos<br>- `aria-busy` durante loading |
| **Skip links** | - "Saltar al contenido" (si no existe, agregarlo al layout) |

### ARIA Labels Ejemplos

**Table Row:**

```tsx
<TableRow
    role="row"
    aria-label={`Template ${template.nombre}, ${template.cantidadNecesidades} necesidades, ${template.activo ? 'activo' : 'inactivo'}`}
>
```

**Action Buttons:**

```tsx
<Button
    variant="ghost"
    size="sm"
    aria-label={`Editar template ${template.nombre}`}
    onClick={handleEdit}
>
    <Edit className="w-4 h-4 mr-1" aria-hidden="true" />
    Editar
</Button>
```

**Search Input:**

```tsx
<Input
    type="text"
    placeholder="Buscar templates..."
    aria-label="Buscar templates por nombre"
    value={searchTerm}
    onChange={handleSearch}
/>
```

**Select con icono:**

```tsx
<Select
    value={selectedIcon}
    onValueChange={setSelectedIcon}
    aria-label="Seleccionar icono del template"
>
    <SelectTrigger>
        <SelectValue placeholder="Selecciona icono" />
    </SelectTrigger>
    <SelectContent>
        <SelectItem value="music">
            <span role="img" aria-label="Icono de musica">🎵</span> Music
        </SelectItem>
    </SelectContent>
</Select>
```

**Loading Button:**

```tsx
<Button disabled={isPending} aria-busy={isPending}>
    {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
    {isPending ? "Guardando..." : "Guardar template"}
</Button>
```

**Form Error Message:**

```tsx
<div>
    <Input
        id="nombre"
        aria-describedby={errors.nombre ? "nombre-error" : undefined}
        aria-invalid={!!errors.nombre}
        {...register("nombre")}
    />
    {errors.nombre && (
        <p
            id="nombre-error"
            role="alert"
            aria-live="polite"
            className="text-sm text-red-500 mt-1"
        >
            {errors.nombre.message}
        </p>
    )}
</div>
```

**Alert Dialog:**

```tsx
<AlertDialogContent
    className="bg-[#0f1729] border-[#334155]"
    aria-labelledby="dialog-title"
    aria-describedby="dialog-description"
>
    <AlertDialogHeader>
        <AlertDialogTitle id="dialog-title">
            ¿Desactivar template?
        </AlertDialogTitle>
        <AlertDialogDescription id="dialog-description">
            Este template dejara de aparecer en la galeria...
        </AlertDialogDescription>
    </AlertDialogHeader>
</AlertDialogContent>
```

---

## 10. Animaciones y Transiciones

### Duraciones Estandar

| Elemento | Animacion | Duracion | Ease |
|----------|-----------|----------|------|
| **Table row hover** | Background color lighten | 150ms | ease |
| **Button hover** | Background gradient shift + scale 1.02 | 150ms | ease |
| **Input focus** | Border color + ring shadow | 200ms | ease |
| **Card hover** | Border color + shadow increase | 200ms | ease-out |
| **Dialog open** | Backdrop fade + content scale 0.95 → 1 | 200ms | ease-out |
| **Toast show** | Slide in from top + fade | 250ms | ease-out |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 infinite | 1500ms | ease-in-out |
| **Loading spinner** | Rotate 360deg infinite | 1000ms | linear |
| **Validation error shake** | Shake animation (4px horizontal) | 400ms | ease |
| **Add need item** | Fade in + slide down | 300ms | ease-out |
| **Remove need item** | Fade out + slide up | 200ms | ease-in |

### Clases Tailwind

```tsx
// Hover transitions
className="hover:bg-[#1e2a42] transition-colors duration-150"

// Focus transitions
className="focus:border-primary focus:ring-2 focus:ring-primary transition-all duration-200"

// Button hover
className="hover:scale-102 transition-transform duration-150"

// Loading spinner
<Loader2 className="animate-spin" />

// Skeleton
<Skeleton className="animate-pulse" />
```

### Custom Animations (Tailwind Config)

```js
// tailwind.config.js
module.exports = {
    theme: {
        extend: {
            keyframes: {
                shake: {
                    '0%, 100%': { transform: 'translateX(0)' },
                    '25%': { transform: 'translateX(-4px)' },
                    '75%': { transform: 'translateX(4px)' },
                },
                slideDown: {
                    '0%': { opacity: '0', transform: 'translateY(-10px)' },
                    '100%': { opacity: '1', transform: 'translateY(0)' },
                },
                slideUp: {
                    '0%': { opacity: '1', transform: 'translateY(0)' },
                    '100%': { opacity: '0', transform: 'translateY(-10px)' },
                },
            },
            animation: {
                shake: 'shake 400ms ease',
                slideDown: 'slideDown 300ms ease-out',
                slideUp: 'slideUp 200ms ease-in',
            },
        },
    },
};
```

**Uso:**

```tsx
// Shake en error
<Input className={errors.nombre && "animate-shake"} />

// Slide down al agregar necesidad
<Card className="animate-slideDown">...</Card>

// Slide up al eliminar necesidad (con exit animation)
import { AnimatePresence, motion } from "framer-motion";

<AnimatePresence>
    {needs.map((need, index) => (
        <motion.div
            key={need.id}
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
        >
            <NeedFormItem need={need} index={index} />
        </motion.div>
    ))}
</AnimatePresence>
```

---

## 11. Componentes Custom

### 11.1 TemplateFilters

**Props:**

```tsx
interface TemplateFiltersProps {
    searchTerm: string;
    onSearchChange: (value: string) => void;
    statusFilter: "all" | "active" | "inactive";
    onStatusChange: (value: "all" | "active" | "inactive") => void;
}
```

**Renderizado:**

```tsx
export const TemplateFilters: FC<TemplateFiltersProps> = ({
    searchTerm,
    onSearchChange,
    statusFilter,
    onStatusChange,
}) => {
    return (
        <Card className="bg-[#0f1729] border-[#334155] p-4 mb-4">
            <div className="flex flex-col md:flex-row gap-4">
                <Input
                    placeholder="Buscar templates..."
                    value={searchTerm}
                    onChange={(e) => onSearchChange(e.target.value)}
                    className="w-full md:w-64 bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]"
                    aria-label="Buscar templates por nombre"
                />
                <Select value={statusFilter} onValueChange={onStatusChange}>
                    <SelectTrigger className="w-full md:w-48">
                        <SelectValue placeholder="Estado" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">Todos</SelectItem>
                        <SelectItem value="active">Activos</SelectItem>
                        <SelectItem value="inactive">Inactivos</SelectItem>
                    </SelectContent>
                </Select>
            </div>
        </Card>
    );
};
```

### 11.2 TemplateTable

**Props:**

```tsx
interface TemplateTableProps {
    templates: PlantillaProyectoList[];
    isLoading: boolean;
    onEdit: (id: string) => void;
    onView: (id: string) => void;
    onToggleStatus: (id: string, currentStatus: boolean) => void;
}
```

**Renderizado:**

- Desktop: Tabla completa
- Mobile: Cards apiladas
- Loading: Skeleton rows/cards
- Empty: Empty state

### 11.3 NeedFormItem

**Props:**

```tsx
interface NeedFormItemProps {
    need: PlantillaProyectoNecesidadFormData;
    index: number;
    rolesProfesionales: RolProfesional[];
    onFieldChange: (index: number, field: string, value: any) => void;
    onDelete: (index: number) => void;
    errors?: Record<string, string>;
}
```

**Renderizado:**

- Card con delete button en top-right
- Fields en grid responsive
- Validacion inline con FormMessage
- Tooltip en rol profesional (descripcion)

### 11.4 NeedsList (Read-only para modal "Ver necesidades")

**Props:**

```tsx
interface NeedsListProps {
    necesidades: PlantillaProyectoNecesidad[];
}
```

**Renderizado:**

- Agrupado por fase
- Badge de prioridad con colores
- Precio orientativo formateado
- Rol profesional visible

### 11.5 DeleteConfirmDialog

**Props:**

```tsx
interface DeleteConfirmDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    templateName: string;
    currentStatus: boolean; // true = activo, false = inactivo
    onConfirm: () => void;
    isPending: boolean;
}
```

**Renderizado:**

- AlertDialog con titulo dinamico ("Desactivar" o "Activar")
- Descripcion adaptada al action
- Botones Cancel + Confirm
- Loading state en Confirm button

### 11.6 EmptyStateCard

**Props:**

```tsx
interface EmptyStateCardProps {
    icon: ReactNode;
    title: string;
    description?: string;
    action?: {
        label: string;
        onClick: () => void;
    };
}
```

**Renderizado:**

```tsx
export const EmptyStateCard: FC<EmptyStateCardProps> = ({
    icon,
    title,
    description,
    action,
}) => {
    return (
        <div className="flex flex-col items-center justify-center py-16">
            <div className="text-6xl mb-4">{icon}</div>
            <p className="text-lg text-[#94a3b8] mb-2">{title}</p>
            {description && (
                <p className="text-sm text-[#64748b] mb-6">{description}</p>
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
    );
};
```

**Uso:**

```tsx
// No templates
<EmptyStateCard
    icon="📋"
    title="No hay templates disponibles"
    description="Crea el primero para comenzar"
    action={{
        label: "Nuevo Template",
        onClick: handleCreateNew,
    }}
/>

// No results after filter
<EmptyStateCard
    icon="🔍"
    title="No se encontraron templates"
    description="Intenta con otros criterios de busqueda"
    action={{
        label: "Limpiar filtros",
        onClick: handleClearFilters,
    }}
/>
```

---

## 12. Checklist de Implementacion

### Componentes shadcn/ui a Utilizar

- [x] Table, TableHeader, TableBody, TableRow, TableHead, TableCell
- [x] Card, CardHeader, CardContent, CardFooter (opcional)
- [x] Input
- [x] Textarea
- [x] Label
- [x] Button
- [x] Select, SelectTrigger, SelectContent, SelectItem, SelectValue
- [x] Checkbox
- [x] Badge
- [x] Dialog, DialogTrigger, DialogContent, DialogHeader, DialogFooter, DialogTitle, DialogDescription
- [x] AlertDialog, AlertDialogTrigger, AlertDialogContent, AlertDialogHeader, AlertDialogFooter, AlertDialogTitle, AlertDialogDescription, AlertDialogAction, AlertDialogCancel
- [x] Skeleton
- [x] Tooltip, TooltipTrigger, TooltipContent
- [x] Separator
- [x] DropdownMenu (opcional, para actions en mobile)
- [x] Progress (opcional, para future features)
- [x] Alert (opcional, para mensajes inline)
- [x] Tabs (NO USAR en esta feature)

**Total:** 18 componentes shadcn/ui

### Screen 1: Template List

- [ ] Page header con titulo + subtitulo
- [ ] New Template button con gradient
- [ ] TemplateFilters component (search + status select)
- [ ] TemplateTable component con todas las columnas
- [ ] Table rows con hover effect
- [ ] Status badges con colores correctos
- [ ] Action buttons inline (Editar, Ver, Toggle)
- [ ] Pagination component
- [ ] Skeleton loading states
- [ ] Empty state component
- [ ] Filtered empty state
- [ ] Responsive: tabla → cards en mobile
- [ ] DeleteConfirmDialog para toggle status
- [ ] Toast notifications para success/error
- [ ] ARIA labels en tabla y botones
- [ ] Keyboard navigation en tabla

### Screen 2: Create/Edit Template

- [ ] Back link con icono
- [ ] Page title dinamico (Nuevo / Editar)
- [ ] Section: Datos Generales card
- [ ] Field: Nombre con validacion
- [ ] Field: Descripcion (textarea)
- [ ] Field: Icono select con preview
- [ ] Field: Orden (number input)
- [ ] Field: Activo (checkbox)
- [ ] Section: Necesidades card
- [ ] Add Need button
- [ ] NeedFormItem component (dynamic array)
- [ ] Delete need button con confirmacion
- [ ] Fields de necesidad: Fase, Titulo, Rol, Precio Min/Max, Prioridad
- [ ] Validacion Zod schema completo
- [ ] FormMessage para errores
- [ ] Responsive: 2 cols → 1 col en mobile
- [ ] Cancel button con unsaved changes dialog
- [ ] Save button con loading state
- [ ] Toast notifications
- [ ] ARIA labels en form fields
- [ ] Loading skeleton en edit mode

### Cross-cutting Concerns

- [ ] Dark theme aplicado consistentemente (design tokens)
- [ ] Tailwind utilities (no CSS custom)
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste minimo WCAG AA verificado
- [ ] Focus states con ring purple
- [ ] ARIA labels y roles completos
- [ ] Keyboard navigation en todos los interactivos
- [ ] Loading states con aria-busy
- [ ] Error boundaries implementados
- [ ] Mobile-first responsive design
- [ ] Toast notifications con sonner
- [ ] Form validation con Zod + React Hook Form
- [ ] TypeScript strict mode sin errores

---

## 13. Notas de Implementacion

### 13.1 Form State Management

**Opcion 1: React Hook Form + Zod (Recomendado)**

```tsx
import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

const form = useForm<TemplateFormData>({
    resolver: zodResolver(templateFormSchema),
    defaultValues: {
        nombre: "",
        descripcion: "",
        icono: "",
        orden: 0,
        activo: true,
        necesidades: [],
    },
});

const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "necesidades",
});

const handleAddNeed = () => {
    append({
        fase: "",
        titulo: "",
        rolProfesionalId: 0,
        precioMin: undefined,
        precioMax: undefined,
        prioridad: "Media",
    });
};
```

**Opcion 2: Zustand (para estado global si es necesario)**

### 13.2 Data Fetching

**TanStack Query hooks:**

```tsx
// hooks/useTemplates.ts
export const useTemplates = (filters: TemplateFilters) => {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.all,
        queryFn: () => templateService.getAll(filters),
    });
};

// hooks/useTemplateDetail.ts
export const useTemplateDetail = (id: string) => {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.byId(id),
        queryFn: () => templateService.getById(id),
        enabled: !!id,
    });
};

// hooks/useCreateTemplate.ts
export const useCreateTemplate = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: TemplateFormData) => templateService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.all,
            });
            toast.success("Template creado correctamente");
        },
        onError: (error) => {
            toast.error("Error al crear template", {
                description: error.message,
            });
        },
    });
};
```

### 13.3 Formateo de Moneda

**Reutilizar utilidad existente:**

```tsx
import { formatCurrency } from "@/shared/utils";

// Uso
<span>{formatCurrency(template.precioMinTotal)}</span>
```

### 13.4 Debounce en Search

```tsx
import { useDebouncedValue } from "@/hooks/useDebouncedValue";

const [searchTerm, setSearchTerm] = useState("");
const debouncedSearch = useDebouncedValue(searchTerm, 300);

// Usar debouncedSearch en el query
const { data } = useTemplates({ search: debouncedSearch, status: statusFilter });
```

### 13.5 Unsaved Changes Detection

```tsx
import { useBeforeUnload } from "@/hooks/useBeforeUnload";

const isDirty = form.formState.isDirty;

useBeforeUnload(isDirty, "Tienes cambios sin guardar");

// En navegacion interna (Next.js)
useEffect(() => {
    const handleRouteChange = () => {
        if (isDirty && !window.confirm("¿Descartar cambios?")) {
            router.events.emit("routeChangeError");
            throw "Navegacion cancelada";
        }
    };

    router.events.on("routeChangeStart", handleRouteChange);
    return () => {
        router.events.off("routeChangeStart", handleRouteChange);
    };
}, [isDirty]);
```

---

## 14. Referencias

### Design Tokens Source

- `docs/user-stories/cs-templates-guia/ui-ux.md` - Seccion "Design Tokens"
- `docs/user-stories/cs-templates-guia/ui-ux.md` - Seccion "Pantalla 4: Gestion de Templates (Admin)"
- `docs/user-stories/cs-templates-guia/ui-ux.md` - Seccion "Pantalla 5: Crear/Editar Template (Admin)"

### Contracts

- `docs/user-stories/cs-templates-guia/contracts.md` - DTOs y endpoints
- `plans/cs-templates-guia/shared/contracts-plan.md` - Types TypeScript y Zod schemas

### shadcn/ui Components

- `src/admin/src/components/ui/` - Componentes existentes

---

**Plan creado:** 2026-02-15
**Arquitecto UI:** Claude Code (Especialista shadcn/ui)
**Estado:** READY FOR IMPLEMENTATION
