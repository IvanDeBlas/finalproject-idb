# UI/UX: Definir Recompensas

> **Feature:** definir-recompensas
> **Última actualización:** 2026-02-13

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Detalle Campaña (Rewards Sidebar) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |
| Wizard Paso 3 (Recompensas) | [WPR_6-Create-Campaign.png](../../ui-images/WPR_6-Create-Campaign.png) | Admin |
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Reward cards en landing, visualización pública | `references/templates/krowd/` |
| **Dashtail** | Gestión de rewards en dashboard, formularios | `references/templates/dashtail/` |

**Componentes de referencia clave:**
- Krowd: reward cards con pricing, stock indicators
- Dashtail: sortable lists, modal forms, CRUD operations

---

## Design Tokens

**Nota:** Se usan los mismos design tokens definidos en [crear-campania/ui-ux.md](../crear-campania/ui-ux.md#design-tokens).

### Colores Específicos de Rewards

```css
:root {
  /* Reward States */
  --reward-available: #10b981;
  --reward-limited: #f59e0b;
  --reward-sold-out: #64748b;

  /* Drag & Drop */
  --drag-handle: #64748b;
  --drag-active: #a855f7;
  --drop-zone: #a855f7;
  --drop-zone-bg: rgba(168, 85, 247, 0.1);
}
```

### Iconos Específicos

| Icono | Uso | Biblioteca |
|-------|-----|-----------|
| `package` | Stock indicator | Lucide React |
| `grip-vertical` | Drag handle | Lucide React |
| `edit` | Edit button | Lucide React |
| `trash-2` | Delete button | Lucide React |
| `plus-circle` | Add reward | Lucide React |
| `check-circle` | Available stock | Lucide React |
| `alert-circle` | Limited stock | Lucide React |
| `x-circle` | Sold out | Lucide React |

---

## Pantalla: Gestión de Recompensas (Dashboard)

**Mockup:** WPR_6-Create-Campaign.png (step 3) + extensión CRUD
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/{id}/rewards`
**Template base:** `dashtail/` sortable lists + CRUD patterns

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│  [SIDEBAR]  │  ← Volver a Campaña                                  │
│             │  Recompensas: "Nuevo Álbum Midnight"                 │
│             │                                                      │
│  Dashboard  │  ┌────────────────────────────────────────────────┐  │
│  Mis        │  │  📊 Resumen                                    │  │
│  Campañas   │  │  Total: 3  •  Activas: 3  •  Stock: 400 unid.  │  │
│  Crear      │  └────────────────────────────────────────────────┘  │
│  Campana    │                                                      │
│  Mis        │  ┌────────────────────────────────────────────────┐  │
│  Backers    │  │  ⋮⋮ € 10  Descarga Digital            [Edit] [×]│  │
│  Config     │  │     Acceso anticipado al álbum en formato      │  │
│             │  │     digital + FLAC + MP3                       │  │
│  Ver        │  │     📦 Ilimitadas disponibles                  │  │
│  perfil     │  │     ✓ 234 backers                               │  │
│  Logout     │  └────────────────────────────────────────────────┘  │
│             │                                                      │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  ⋮⋮ € 25  CD Físico                   [Edit] [×]│  │
│             │  │     CD firmado + descarga digital + booklet    │  │
│             │  │     dedicado con letras y fotos                │  │
│             │  │     📦 100 de 200 disponibles  ⚠️ 50% vendido  │  │
│             │  │     ✓ 89 backers                                │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                      │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  ⋮⋮ € 50  Vinilo Edición Limitada    [Edit] [×]│  │
│             │  │     Vinilo en color especial + póster          │  │
│             │  │     exclusivo + todo lo anterior               │  │
│             │  │     📦 45 de 100 disponibles                    │  │
│             │  │     ✓ 55 backers                                │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                      │
│             │  ┌────────────────────────────────────────────────┐  │
│             │  │  + Agregar recompensa                          │  │
│             │  └────────────────────────────────────────────────┘  │
│             │                                                      │
│             │  💡 Tip: Ordena tus recompensas arrastrándolas     │  │
│             │      El orden se reflejará en la vista pública     │  │
│             │                                                      │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Back Link | `<Link>` con `<Icon>` (arrow-left) | `flex items-center gap-2 text-[#94a3b8] hover:text-white mb-4 transition` |
| Page Title | `<h1>` | `text-2xl font-bold text-white mb-6` |
| Campaign Subtitle | `<p>` | `text-sm text-[#64748b] -mt-4 mb-6` |
| Stats Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-4 mb-6` |
| Stats Row | `<div>` | `flex items-center gap-6 text-sm` |
| Stat Item | `<div>` | `flex items-center gap-2` |
| Stat Icon | `<Icon>` | `w-4 h-4 text-primary` |
| Stat Label | `<span>` | `text-[#64748b]` |
| Stat Value | `<span>` | `font-semibold text-white` |
| Sortable Container | `<div>` (dnd-kit `SortableContext`) | `space-y-3` |
| Reward Card (Sortable) | `<Card>` (dnd-kit `useSortable`) | `bg-[#1a1a2e] border-[#334155] p-5 hover:border-primary cursor-move transition group` |
| Drag Handle | `<div>` con `<Icon>` (grip-vertical) | `absolute left-2 top-1/2 -translate-y-1/2 text-[#64748b] group-hover:text-primary cursor-grab active:cursor-grabbing` |
| Reward Card Content | `<div>` | `pl-8 pr-20` (espacio para handle y botones) |
| Reward Header | `<div>` | `flex items-start justify-between mb-3` |
| Reward Price | `<span>` | `text-2xl font-bold text-primary` |
| Reward Title | `<h3>` | `text-lg font-semibold text-white ml-3` |
| Reward Description | `<p>` | `text-sm text-[#94a3b8] mb-3 line-clamp-2` |
| Reward Meta Row | `<div>` | `flex items-center gap-4 text-xs text-[#64748b]` |
| Stock Indicator | `<div>` | `flex items-center gap-1` |
| Stock Icon (unlimited) | `<Icon>` (infinity) | `w-4 h-4 text-green-400` |
| Stock Icon (limited) | `<Icon>` (package) | `w-4 h-4 text-warning` |
| Stock Icon (sold out) | `<Icon>` (x-circle) | `w-4 h-4 text-[#64748b]` |
| Stock Text (unlimited) | `<span>` | `text-green-400` |
| Stock Text (available) | `<span>` | `text-[#94a3b8]` |
| Stock Text (sold out) | `<span>` | `text-[#64748b]` |
| Stock Warning | `<Badge variant="outline">` | `border-warning text-warning bg-warning/10` |
| Backers Count | `<div>` | `flex items-center gap-1` |
| Backers Icon | `<Icon>` (check-circle) | `w-4 h-4 text-primary` |
| Backers Text | `<span>` | `text-[#94a3b8]` |
| Actions Container | `<div>` | `absolute right-4 top-4 flex gap-2` |
| Edit Button | `<Button variant="ghost" size="sm">` | `text-primary hover:bg-primary/10` |
| Delete Button | `<Button variant="ghost" size="sm">` | `text-red-400 hover:bg-red-500/10` |
| Add Reward Button | `<Button variant="outline">` | `w-full border-dashed border-primary text-primary hover:bg-primary/10 py-6 mb-4` |
| Add Icon | `<Icon>` (plus-circle) | `w-5 h-5 mr-2` |
| Tip Box | `<div>` | `bg-[#0f1729] border-l-4 border-primary p-4 rounded-r-lg` |
| Tip Icon | `<Icon>` (lightbulb) | `w-5 h-5 text-primary inline mr-2` |
| Tip Text | `<p>` | `text-sm text-[#94a3b8]` |
| Empty State Container | `<div>` | `text-center py-16` |
| Empty Icon | `<Icon>` (package-open) | `w-20 h-20 text-[#64748b] mx-auto mb-4` |
| Empty Title | `<h3>` | `text-xl font-bold text-white mb-2` |
| Empty Description | `<p>` | `text-[#94a3b8] mb-6` |
| Empty CTA | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Lista de rewards ordenables, drag handles visibles al hover, stats calculados |
| **Empty State** | Icono package-open grande, texto "No has creado recompensas aún", botón "Crear primera recompensa" gradient |
| **Hover Card** | Border color cambia a primary, drag handle se vuelve más visible |
| **Dragging Card** | Card tiene opacity 0.5, drop indicator line aparece entre cards, cursor grab |
| **Drop Zone Active** | Línea horizontal gradient aparece donde se puede soltar |
| **Drag Complete** | Animación smooth al reordenar, auto-save del nuevo orden |
| **Loading** | Skeleton loaders para reward cards (3-4 skeletons) |
| **Stock Warning** | Badge "50% vendido" amarillo cuando stock < 50% disponible |
| **Sold Out** | Card con opacity reducida, badge "Agotado" gris, stock icon X |
| **Deleting** | Card fade out animation, spinner en delete button |
| **Error** | Toast notification roja con mensaje de error |

### Validación en Tiempo Real

**No aplica** - Página de gestión sin inputs directos. Validación ocurre en el formulario modal.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Back Link** | Navigate a `/dashboard/campanias/{id}` o `/dashboard/campanias/{id}/preview` |
| **Drag Card** | Activar drag state, mostrar drop zones, actualizar orden en drop |
| **Drop Card** | Auto-save nuevo orden vía API, actualizar lista, toast "Orden actualizado" |
| **Click Edit** | Abrir modal "Edit Reward" con formulario pre-completado |
| **Click Delete (× button)** | Mostrar dialog confirmación, verificar si tiene backings |
| **Confirm Delete (no backings)** | DELETE `/api/rewards/{id}`, remover de lista, toast "Recompensa eliminada" |
| **Confirm Delete (has backings)** | Mostrar error modal "No se puede eliminar, tiene X backings. ¿Desactivar?" con botón "Desactivar" |
| **Click Desactivar** | PUT `/api/rewards/{id}/deactivate`, marcar como inactiva (no mostrar en lista activas) |
| **Click + Agregar recompensa** | Abrir modal "New Reward" con formulario vacío |
| **Hover Card** | Border primary, drag handle más visible, elevation aumenta |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar colapsado, drag handles permanentemente visibles (no hover), botones Edit/Delete como iconos sin texto, stats en grid 2x2 |
| **Tablet (640-1024px)** | Sidebar visible, layout normal, drag handles visible al hover |
| **Desktop (> 1024px)** | Layout completo, max-width container, spacing amplio |

---

## Pantalla: Formulario Create/Edit Reward (Modal)

**Mockup:** Derivado de WPR_6-Create-Campaign.png + patterns de Dashtail forms
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/{id}/rewards` (modal overlay)
**Template base:** `dashtail/` modal forms

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│    ┌──────────────────────────────────────────────────────────┐    │
│    │  [×] Nueva Recompensa                                    │    │
│    ├──────────────────────────────────────────────────────────┤    │
│    │                                                          │    │
│    │  Nombre de la recompensa *                               │    │
│    │  [______________________________________________]         │    │
│    │  Máx 100 caracteres - Ej: "Descarga Digital"            │    │
│    │                                                          │    │
│    │  Descripción *                                           │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  Describe qué incluye esta recompensa...          │  │    │
│    │  │                                                    │  │    │
│    │  │  [Textarea ~4 líneas]                             │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │  0/1000 caracteres                                       │    │
│    │                                                          │    │
│    │  Importe mínimo *          Tipo de recompensa           │    │
│    │  [€ 10.00________]          [Seleccionar tipo ▼]        │    │
│    │                                                          │    │
│    │  Moneda                     ☑️ Es add-on (complemento)   │    │
│    │  [EUR ▼]                                                 │    │
│    │                                                          │    │
│    │  ☑️ Stock limitado                                       │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  Cantidad máxima disponible                        │  │    │
│    │  │  [200_______]                                      │  │    │
│    │  │                                                    │  │    │
│    │  │  Máximo por backer (opcional)                     │  │    │
│    │  │  [1_________]                                      │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │                                                          │    │
│    │  ☑️ Incluye envío físico                                 │    │
│    │  ┌────────────────────────────────────────────────────┐  │    │
│    │  │  Tiempo de entrega estimado                       │  │    │
│    │  │  [Marzo 2025____________________________]          │  │    │
│    │  └────────────────────────────────────────────────────┘  │    │
│    │                                                          │    │
│    │                                                          │    │
│    │  [Cancelar]                             [Guardar]       │    │
│    │  (outline)                             (gradient)       │    │
│    └──────────────────────────────────────────────────────────┘    │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Modal Overlay | `<Dialog>` backdrop | `bg-black/80 backdrop-blur-sm` |
| Modal Container | `<DialogContent>` | `bg-[#0f1729] border-[#334155] max-w-2xl max-h-[90vh] overflow-y-auto` |
| Modal Header | `<DialogHeader>` | `border-b border-[#334155] pb-4 mb-6` |
| Modal Title | `<DialogTitle>` | `text-2xl font-bold text-white` |
| Close Button | `<DialogClose>` (X) | `absolute right-4 top-4 text-[#94a3b8] hover:text-white` |
| Form Container | `<form>` | `space-y-6 p-6` |
| Label (required) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block after:content-['*'] after:text-red-500 after:ml-1` |
| Label (optional) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` |
| Text Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] focus:border-primary` |
| Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none placeholder:text-[#64748b]` |
| Character Counter | `<span>` | `text-xs text-[#64748b] mt-1 block` (color warning at 80%, red at 95%) |
| Hint Text | `<p>` | `text-xs text-[#64748b] mt-1 italic` |
| Grid 2 Columns | `<div>` | `grid grid-cols-2 gap-4` |
| Number Input (price) | `<Input type="number" step="0.01">` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Currency Prefix | `<span>` | `absolute left-3 text-[#94a3b8]` (dentro de Input wrapper) |
| Select Dropdown | `<Select>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Checkbox | `<Checkbox>` | `border-[#334155] data-[state=checked]:bg-primary data-[state=checked]:border-primary` |
| Checkbox Label | `<label>` | `text-sm text-white ml-2 cursor-pointer` |
| Conditional Section | `<div>` | `bg-[#1a1a2e] border border-[#334155] p-4 rounded-lg mt-2` (visible when checkbox checked) |
| Error Message | `<p>` | `text-xs text-red-500 mt-1` |
| Modal Footer | `<DialogFooter>` | `border-t border-[#334155] pt-4 mt-6 flex gap-3 justify-end` |
| Cancel Button | `<Button variant="outline">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Save Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (Create)** | Modal con título "Nueva Recompensa", form vacío |
| **Default (Edit)** | Modal con título "Editar Recompensa", form pre-completado con datos existentes |
| **Focus Input** | Border color cambia a primary, glow shadow |
| **Typing Nombre** | Character counter actualizado (X/100), warning color a 80 caracteres |
| **Typing Descripción** | Character counter actualizado (X/1000), warning color a 800 caracteres |
| **Stock Limitado Unchecked** | Conditional section (cantidad máxima, máximo por backer) oculta |
| **Stock Limitado Checked** | Conditional section visible con fade-in animation |
| **Envío Físico Unchecked** | Conditional section (tiempo de entrega) oculta |
| **Envío Físico Checked** | Conditional section visible con fade-in animation |
| **Validation Error** | Input con border rojo, mensaje de error debajo en rojo |
| **Loading Submit** | Save button muestra spinner + "Guardando...", form inputs disabled |
| **Success** | Toast verde "Recompensa creada/actualizada", modal cierra, lista refresh |
| **Error** | Toast roja con mensaje de error del backend |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Nombre** | No vacío | "El nombre es obligatorio" |
| **Nombre** | Máx 100 caracteres | "Máximo 100 caracteres (X/100)" |
| **Descripción** | No vacío | "La descripción es obligatoria" |
| **Descripción** | Máx 1000 caracteres | "Máximo 1000 caracteres (X/1000)" |
| **Importe mínimo** | Mayor a 0 | "El importe debe ser mayor a cero" |
| **Importe mínimo** | Formato decimal válido | "Ingresa un monto válido (ej: 10.00)" |
| **Tipo recompensa** | Selección requerida | "Selecciona un tipo de recompensa" |
| **Cantidad máxima** | (Si stock limitado) Mayor a 0 | "Debe haber al menos 1 disponible" |
| **Cantidad máxima** | (Si stock limitado) Número entero | "Ingresa un número entero" |
| **Máximo por backer** | (Si proporcionado) Mayor a 0 | "Debe ser al menos 1" |
| **Máximo por backer** | (Si proporcionado) Número entero | "Ingresa un número entero" |
| **Máximo por backer** | (Si proporcionado) <= Cantidad máxima | "No puede ser mayor a la cantidad máxima" |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click X (cerrar)** | Cerrar modal sin guardar, mostrar confirmación si hay cambios no guardados |
| **Click Cancelar** | Cerrar modal sin guardar, mostrar confirmación si hay cambios |
| **Toggle Stock Limitado** | Mostrar/ocultar sección condicional con animation |
| **Toggle Envío Físico** | Mostrar/ocultar sección condicional con animation |
| **Toggle Es Add-on** | Actualizar estado del checkbox (no UI condicional adicional) |
| **Typing en inputs** | Validación debounced (300ms), actualizar character counters |
| **Click Guardar** | Validar form completo, si válido POST/PUT `/api/rewards`, cerrar modal, refresh lista, toast success |
| **Form Submit Error** | Mostrar toast error con mensaje del backend, mantener modal abierto |
| **Press Esc** | Cerrar modal (si no hay cambios sin guardar) |

### Zod Schema

```typescript
const rewardFormSchema = z.object({
  nombre: z.string()
    .min(1, "El nombre es obligatorio")
    .max(100, "Máximo 100 caracteres"),

  descripcion: z.string()
    .min(1, "La descripción es obligatoria")
    .max(1000, "Máximo 1000 caracteres"),

  importeMinimo: z.number()
    .min(0.01, "El importe debe ser mayor a cero")
    .positive("Ingresa un monto válido"),

  tipoRewardId: z.number()
    .int()
    .min(1, "Selecciona un tipo de recompensa"),

  monedaId: z.number()
    .int()
    .min(1, "Selecciona una moneda")
    .default(1), // Default EUR

  esAddOn: z.boolean()
    .default(false),

  stockLimitado: z.boolean()
    .default(false),

  cantidadMaxima: z.number()
    .int()
    .min(1, "Debe haber al menos 1 disponible")
    .optional()
    .nullable(),

  cantidadPorBacker: z.number()
    .int()
    .min(1, "Debe ser al menos 1")
    .optional()
    .nullable(),

  incluyeEnvioFisico: z.boolean()
    .default(false),

  tiempoEntregaEstimado: z.string()
    .max(100, "Máximo 100 caracteres")
    .optional()
    .nullable(),

  orden: z.number()
    .int()
    .min(0)
    .default(0), // Auto-assigned al crear
})
.refine(
  (data) => !data.cantidadPorBacker || !data.cantidadMaxima || data.cantidadPorBacker <= data.cantidadMaxima,
  {
    message: "El máximo por backer no puede ser mayor a la cantidad máxima",
    path: ["cantidadPorBacker"],
  }
)
.refine(
  (data) => !data.stockLimitado || (data.cantidadMaxima && data.cantidadMaxima > 0),
  {
    message: "Debes especificar la cantidad máxima si el stock es limitado",
    path: ["cantidadMaxima"],
  }
)
.refine(
  (data) => !data.incluyeEnvioFisico || (data.tiempoEntregaEstimado && data.tiempoEntregaEstimado.length > 0),
  {
    message: "Especifica el tiempo de entrega si incluye envío físico",
    path: ["tiempoEntregaEstimado"],
  }
);
```

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Modal max-w-full, padding reducido, grid 2 columns → stack vertical, botones footer stack vertical full-width |
| **Tablet (640-1024px)** | Modal max-w-xl, grid normal |
| **Desktop (> 1024px)** | Modal max-w-2xl, layout completo |

---

## Pantalla: Delete Reward Confirmation (Dialog)

**Mockup:** Pattern estándar de confirmación
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/{id}/rewards` (dialog overlay)
**Template base:** `dashtail/` confirmation dialogs

### Layout

```
┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│         ┌──────────────────────────────────────────────┐           │
│         │  ⚠️  Eliminar Recompensa                     │           │
│         ├──────────────────────────────────────────────┤           │
│         │                                              │           │
│         │  ¿Estás seguro que deseas eliminar esta     │           │
│         │  recompensa?                                 │           │
│         │                                              │           │
│         │  "Descarga Digital" (€10)                    │           │
│         │                                              │           │
│         │  Esta acción no se puede deshacer.           │           │
│         │                                              │           │
│         │                                              │           │
│         │  [Cancelar]              [Eliminar]          │           │
│         │  (outline)               (destructive)       │           │
│         └──────────────────────────────────────────────┘           │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│                                                                    │
│         ┌──────────────────────────────────────────────┐           │
│         │  ⚠️  No se puede eliminar                    │           │
│         ├──────────────────────────────────────────────┤           │
│         │                                              │           │
│         │  Esta recompensa no se puede eliminar        │           │
│         │  porque tiene 23 backings confirmados.       │           │
│         │                                              │           │
│         │  Puedes desactivarla para que no aparezca   │           │
│         │  a nuevos backers, pero los existentes       │           │
│         │  mantendrán su selección.                    │           │
│         │                                              │           │
│         │                                              │           │
│         │  [Cancelar]            [Desactivar]          │           │
│         │  (outline)             (warning)             │           │
│         └──────────────────────────────────────────────┘           │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Dialog Overlay | `<AlertDialog>` backdrop | `bg-black/80 backdrop-blur-sm` |
| Dialog Container | `<AlertDialogContent>` | `bg-[#0f1729] border-[#334155] max-w-md` |
| Dialog Header | `<AlertDialogHeader>` | `border-b border-[#334155] pb-4 mb-4` |
| Dialog Title | `<AlertDialogTitle>` | `flex items-center gap-2 text-xl font-bold text-white` |
| Warning Icon | `<Icon>` (alert-triangle) | `w-6 h-6 text-warning` |
| Dialog Description | `<AlertDialogDescription>` | `text-[#94a3b8] space-y-3` |
| Reward Name Highlight | `<p>` | `font-semibold text-white bg-[#1a1a2e] px-3 py-2 rounded border border-[#334155]` |
| Warning Text | `<p>` | `text-sm text-[#64748b] italic` |
| Dialog Footer | `<AlertDialogFooter>` | `border-t border-[#334155] pt-4 mt-6 flex gap-3 justify-end` |
| Cancel Button | `<AlertDialogCancel>` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]` |
| Delete Button (no backings) | `<AlertDialogAction>` | `bg-red-500 hover:bg-red-600 text-white font-semibold` |
| Deactivate Button (has backings) | `<AlertDialogAction>` | `bg-warning hover:bg-warning/90 text-[#0f1729] font-semibold` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (no backings)** | Dialog título "Eliminar Recompensa", texto confirmación estándar, botón "Eliminar" rojo |
| **Has Backings** | Dialog título "No se puede eliminar", texto explicativo con número de backings, botón "Desactivar" amarillo |
| **Loading Delete** | Delete button spinner + "Eliminando...", otros disabled |
| **Loading Deactivate** | Deactivate button spinner + "Desactivando...", otros disabled |
| **Success Delete** | Dialog cierra, reward removida de lista, toast "Recompensa eliminada" |
| **Success Deactivate** | Dialog cierra, reward marcada como inactiva (removida de lista activas), toast "Recompensa desactivada" |
| **Error** | Toast roja con mensaje de error, dialog permanece abierto |

### Validación en Tiempo Real

**No aplica** - Dialog de confirmación sin inputs.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Cancelar** | Cerrar dialog sin acción |
| **Click Eliminar (no backings)** | DELETE `/api/rewards/{id}`, cerrar dialog, remover de lista, toast success |
| **Click Desactivar (has backings)** | PUT `/api/rewards/{id}/deactivate`, cerrar dialog, actualizar lista, toast "Recompensa desactivada. Ya no aparecerá a nuevos backers" |
| **Press Esc** | Cerrar dialog (equivalente a Cancelar) |

---

## Pantalla: Reward Cards en Campaign Detail (Landing - Public)

**Mockup:** WPR_3-Detail-Campaign.png (sidebar recompensas)
**Proyecto:** Landing
**Ruta:** `/campanias/{id}` (sección rewards en sidebar)
**Template base:** `krowd/` campaign detail rewards sidebar

### Layout

```
┌──────────────────────────────────────┐
│  [Apoyar esta campaña]               │
│  (gradient button - full width)      │
│  Desde € 5                           │
│                                      │
│  ──────────────────────────────────  │
│                                      │
│  Recompensas                         │
│                                      │
│  ┌────────────────────────────────┐  │
│  │  € 10  Descarga Digital        │  │
│  │  Más popular                   │  │
│  │                                │  │
│  │  Acceso anticipado al álbum    │  │
│  │  completo en formato digital   │  │
│  │  FLAC + MP3                    │  │
│  │                                │  │
│  │  📦 234 de 500 disponibles     │  │
│  │  [Seleccionar]                 │  │
│  └────────────────────────────────┘  │
│                                      │
│  ┌────────────────────────────────┐  │
│  │  € 25  CD Físico               │  │
│  │                                │  │
│  │  CD firmado + descarga digital │  │
│  │  + booklet con letras          │  │
│  │                                │  │
│  │  📦 89 de 200 disponibles      │  │
│  │  [Seleccionar]                 │  │
│  └────────────────────────────────┘  │
│                                      │
│  ┌────────────────────────────────┐  │
│  │  € 50  Vinilo Limitado         │  │
│  │  ⚠️ Pocas unidades             │  │
│  │                                │  │
│  │  Vinilo especial + póster      │  │
│  │  exclusivo + todo lo anterior  │  │
│  │                                │  │
│  │  📦 45 de 100 disponibles      │  │
│  │  [Seleccionar]                 │  │
│  └────────────────────────────────┘  │
│                                      │
│  ┌────────────────────────────────┐  │
│  │  € 100  Paquete VIP  AGOTADO   │  │
│  │                                │  │
│  │  Meet & greet privado + vinilo │  │
│  │  + mercancía exclusiva         │  │
│  │                                │  │
│  │  ❌ Agotado                     │  │
│  │  [Agotado]                     │  │
│  └────────────────────────────────┘  │
│                                      │
│  🔒 Pago seguro                      │
│  📦 Entrega estimada: Marzo 2025     │
│                                      │
└──────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Sidebar Container | `<div>` | `sticky top-24 space-y-6` |
| Support CTA Button | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2` |
| Starting Price | `<p>` | `text-center text-sm text-[#94a3b8] mb-6` |
| Divider | `<hr>` | `border-[#334155] my-6` |
| Rewards Title | `<h3>` | `text-lg font-bold text-white mb-4` |
| Reward Card | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4 mb-3 hover:border-primary cursor-pointer transition group` |
| Reward Card (sold out) | `<Card>` | `bg-[#1a1a2e] border-[#334155] p-4 mb-3 opacity-60 cursor-not-allowed` |
| Reward Header Row | `<div>` | `flex items-start justify-between mb-3` |
| Reward Price | `<div>` | `text-xl font-bold text-primary` |
| Reward Title | `<h4>` | `text-white font-semibold` |
| Popular Badge | `<Badge>` | `bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs` |
| Limited Badge | `<Badge>` | `bg-warning/20 text-warning border-warning/50 text-xs` |
| Sold Out Badge | `<Badge>` | `bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50 text-xs` |
| Reward Description | `<p>` | `text-sm text-[#94a3b8] mb-3` |
| Stock Info Container | `<div>` | `flex items-center gap-2 text-xs mb-3` |
| Stock Icon (available) | `<Icon>` (package) | `w-4 h-4 text-green-400` |
| Stock Icon (limited) | `<Icon>` (alert-circle) | `w-4 h-4 text-warning` |
| Stock Icon (sold out) | `<Icon>` (x-circle) | `w-4 h-4 text-[#64748b]` |
| Stock Text (available) | `<span>` | `text-[#94a3b8]` |
| Stock Text (limited) | `<span>` | `text-warning` |
| Stock Text (sold out) | `<span>` | `text-[#64748b]` |
| Select Button | `<Button variant="outline" size="sm">` | `w-full border-primary text-primary hover:bg-primary/10 group-hover:bg-primary/10` |
| Select Button (sold out) | `<Button variant="outline" size="sm">` | `w-full border-[#64748b]/50 text-[#64748b] cursor-not-allowed` disabled |
| Sidebar Footer | `<div>` | `border-t border-[#334155] pt-4 mt-6 space-y-2` |
| Security Info | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` |
| Security Icon | `<Icon>` (lock) | `w-4 h-4` |
| Delivery Info | `<div>` | `flex items-center gap-2 text-xs text-[#64748b]` |
| Delivery Icon | `<Icon>` (truck) | `w-4 h-4` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Reward cards ordenadas por precio ascendente, botones "Seleccionar" habilitados |
| **No Rewards** | Texto "Esta campaña no tiene recompensas específicas. Puedes hacer una contribución libre." + solo botón general "Apoyar" |
| **Loading** | Skeleton loaders para reward cards |
| **Hover Card (available)** | Border color cambia a primary, elevation aumenta, select button background hover |
| **Popular Reward** | Badge "Más popular" rosa en la reward con más backings |
| **Limited Stock (< 50%)** | Badge "Pocas unidades" amarillo, stock text warning color |
| **Sold Out** | Card opacity 60%, badge "AGOTADO" gris, botón disabled, stock icon X |
| **Mobile View** | Cards stack verticalmente, padding reducido |

### Validación en Tiempo Real

**No aplica** - Vista de solo lectura.

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click Apoyar esta campaña** | Si autenticado → `/campanias/{id}/backing`. Si no → redirect a `/login?returnUrl=/campanias/{id}/backing` |
| **Click Seleccionar (reward)** | Si autenticado → `/campanias/{id}/backing?reward={rewardId}`. Si no → redirect a `/login?returnUrl=/campanias/{id}/backing?reward={rewardId}` |
| **Hover Card (available)** | Border primary, elevation increase, button background hover |
| **Hover Card (sold out)** | No hover effect |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar NO sticky (scroll normal), reward cards padding reducido (p-3), font sizes reducidos |
| **Tablet (640-1024px)** | Sidebar sticky, layout normal |
| **Desktop (> 1024px)** | Sidebar sticky, spacing amplio |

---

## Pantalla: Reward Cards en Wizard Step 3 (Admin - Campaign Creation)

**Mockup:** WPR_6-Create-Campaign.png (step 3)
**Proyecto:** Admin
**Ruta:** `/dashboard/campanias/nueva?step=3`
**Template base:** Ver [crear-campania/ui-ux.md - Paso 3](../crear-campania/ui-ux.md#pantalla-wizard-crear-campaña---paso-3-recompensas)

### Notas de Integración

Esta pantalla ya está documentada en `crear-campania/ui-ux.md` paso 3. Las diferencias específicas para esta feature:

1. **Creación inline vs. modal**: En el wizard MVP, se puede usar formulario inline expandible o modal (preferir modal por consistencia con gestión).

2. **Sin drag & drop en wizard**: El wizard no necesita reordenamiento. El orden se define en la gestión post-creación.

3. **Validación opcional**: El wizard permite avanzar a paso 4 sin rewards (mostrar advertencia en paso 4).

4. **Estado guardado**: Al volver de paso 4 a paso 3, los rewards creados deben aparecer listados.

### Layout Simplificado (dentro del wizard)

```
┌──────────────────────────────────────────────────────────────────┐
│  Paso 3: Recompensas                                             │
│                                                                  │
│  [Lista de rewards creados - sin drag handles]                   │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  + Agregar recompensa                                      │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  [← Anterior]                Paso 3 de 4        [Siguiente →]   │
└──────────────────────────────────────────────────────────────────┘
```

**Diferencias con gestión:**
- No drag handles (orden automático por importe)
- Botones Edit/Delete en cada card
- Modal form para crear/editar (reutilizar mismo componente)
- Permitir avanzar sin rewards (warning en paso 4)

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios Generales |
|------------|-------|----------|
| **Mobile** | < 640px | Stack todo verticalmente, padding reducido (px-4), drag handles permanentemente visibles, botones icon-only, modal full-screen |
| **Tablet** | 640-1024px | Grid 2 columnas donde aplique, padding medio (px-6), drag handles al hover |
| **Desktop** | > 1024px | Layouts completos, max-width containers, padding amplio (px-8), drag handles al hover |

### Gestión Rewards (Mobile)
- Sidebar colapsado a hamburger
- Stats en grid 2x2
- Reward cards padding reducido (p-4)
- Drag handles siempre visibles (no hover)
- Botones Edit/Delete como iconos sin texto
- Modal form fields stack vertical

### Modal Form (Mobile)
- Modal max-w-full, ocupa toda la pantalla
- Grid 2 columns → stack vertical
- Botones footer stack vertical full-width
- Conditional sections padding reducido

### Public Rewards (Mobile)
- Sidebar NO sticky
- Reward cards stack verticalmente
- Font sizes reducidos (precio text-lg en lugar de text-xl)
- Padding reducido en cards (p-3)

---

## Animaciones

| Elemento | Animación | Duración |
|----------|-----------|----------|
| **Reward card hover** | Border color transition + elevation | 200ms ease |
| **Drag start** | Opacity 0.5 + cursor grab → grabbing | 150ms ease |
| **Drop zone indicator** | Line fade in + gradient pulse | 200ms ease |
| **Card reorder** | Smooth position transition | 300ms ease-out |
| **Modal open** | Scale 0.95 → 1 + fade in + backdrop blur | 250ms ease-out |
| **Modal close** | Scale 1 → 0.95 + fade out | 200ms ease-in |
| **Dialog open** | Fade in + scale 0.95 → 1 | 200ms ease-out |
| **Conditional section expand** | Max-height 0 → auto + fade in | 300ms ease-out |
| **Stock warning badge** | Fade in + scale 0.9 → 1 | 200ms ease |
| **Delete card** | Fade out + scale 1 → 0.9 | 250ms ease-in |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Button loading spinner** | Rotate 360deg loop | 600ms linear |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 loop | 1500ms ease-in-out |
| **Select button hover** | Background fade in + border glow | 150ms ease |

### Drag & Drop Animations

```css
/* Card being dragged */
.reward-card.dragging {
  opacity: 0.5;
  cursor: grabbing;
  transform: scale(1.02);
  transition: opacity 150ms ease, transform 150ms ease;
}

/* Drop zone indicator */
.drop-zone-indicator {
  height: 2px;
  background: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  animation: dropZonePulse 1000ms ease-in-out infinite;
}

@keyframes dropZonePulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

/* Card reorder */
.reward-card {
  transition: transform 300ms ease-out;
}
```

---

## Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de color** | Mínimo 4.5:1 para texto normal. Verificado: primary (#a855f7) sobre bg-card (#1a1a2e) = 5.2:1 ✓ |
| **Focus visible** | Ring de 2px en primary (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Keyboard navigation** | Tab order lógico, Enter para seleccionar/guardar, Esc para cerrar modales, Space para toggle checkboxes |
| **Drag & Drop keyboard** | Implementar alternativa con botones ↑↓ para reordenar sin mouse |
| **Screen reader announce** | Cambios de orden anunciados con `aria-live="polite"` |
| **Form labels** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Buttons** | Texto descriptivo o `aria-label` en icon buttons |
| **Modal focus trap** | Focus queda dentro del modal mientras está abierto |
| **Modal close** | Esc cierra modal, focus vuelve al trigger button |
| **Dialog role** | `role="dialog"` con `aria-modal="true"` y `aria-labelledby` apuntando al título |
| **Stock indicators** | Iconos acompañados de texto (no solo color) |
| **Badges** | `role="status"` con texto descriptivo |
| **Loading states** | `aria-busy="true"` y `aria-live="polite"` para anunciar cambios |

### ARIA Labels Específicos

```tsx
// Gestión de Rewards - Sortable List
<div role="list" aria-label="Lista de recompensas ordenables">
  <div role="listitem" aria-roledescription="Recompensa ordenable">
    <button aria-label="Arrastrar para reordenar Descarga Digital">
      <Icon name="grip-vertical" aria-hidden="true" />
    </button>
    <button aria-label="Editar recompensa Descarga Digital">
      <Icon name="edit" aria-hidden="true" />
      Editar
    </button>
    <button aria-label="Eliminar recompensa Descarga Digital">
      <Icon name="trash-2" aria-hidden="true" />
    </button>
  </div>
</div>

// Keyboard reorder buttons (alternativa accesible)
<div role="group" aria-label="Reordenar recompensa">
  <button aria-label="Mover Descarga Digital hacia arriba">
    <Icon name="chevron-up" aria-hidden="true" />
  </button>
  <button aria-label="Mover Descarga Digital hacia abajo">
    <Icon name="chevron-down" aria-hidden="true" />
  </button>
</div>

// Modal Form
<div role="dialog" aria-modal="true" aria-labelledby="reward-form-title">
  <h2 id="reward-form-title">Nueva Recompensa</h2>
  <form aria-label="Formulario de recompensa">
    <label htmlFor="nombre">Nombre de la recompensa *</label>
    <input id="nombre" aria-required="true" aria-describedby="nombre-hint nombre-error" />
    <span id="nombre-hint">Máx 100 caracteres</span>
    <span id="nombre-error" role="alert" aria-live="polite">
      {error && "El nombre es obligatorio"}
    </span>
  </form>
</div>

// Stock limitado toggle
<div role="group" aria-labelledby="stock-limitado-label">
  <label id="stock-limitado-label">
    <input type="checkbox" aria-describedby="stock-limitado-description" />
    Stock limitado
  </label>
  <span id="stock-limitado-description" className="sr-only">
    Marcar si la recompensa tiene cantidad limitada de unidades
  </span>
</div>

// Delete confirmation dialog
<div role="alertdialog" aria-modal="true" aria-labelledby="delete-dialog-title" aria-describedby="delete-dialog-description">
  <h2 id="delete-dialog-title">Eliminar Recompensa</h2>
  <p id="delete-dialog-description">
    ¿Estás seguro que deseas eliminar esta recompensa? Esta acción no se puede deshacer.
  </p>
  <button aria-label="Cancelar eliminación">Cancelar</button>
  <button aria-label="Confirmar eliminación de recompensa">Eliminar</button>
</div>

// Public reward card
<article role="article" aria-labelledby="reward-1-title" aria-describedby="reward-1-description reward-1-stock">
  <h4 id="reward-1-title">Descarga Digital</h4>
  <p id="reward-1-description">Acceso anticipado al álbum completo...</p>
  <p id="reward-1-stock">234 de 500 disponibles</p>
  <button aria-label="Seleccionar recompensa Descarga Digital por 10 euros">
    Seleccionar
  </button>
</article>

// Stock badge
<span role="status" aria-label="Stock disponible: 234 de 500 unidades">
  <Icon name="package" aria-hidden="true" />
  234 de 500 disponibles
</span>

// Loading button
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isPending ? "Guardando..." : "Guardar"}
</Button>
```

### Keyboard Shortcuts

| Tecla | Acción | Contexto |
|-------|--------|----------|
| **Tab** | Navegar entre elementos interactivos | Global |
| **Shift + Tab** | Navegar hacia atrás | Global |
| **Enter** | Activar botón, submit form, seleccionar reward | Botones, forms, cards |
| **Space** | Toggle checkbox, activar botón | Checkboxes, botones |
| **Esc** | Cerrar modal/dialog | Modales, dialogs |
| **Arrow Up** | Mover reward hacia arriba (alternativa drag) | Lista de rewards |
| **Arrow Down** | Mover reward hacia abajo (alternativa drag) | Lista de rewards |
| **Delete** | (Con foco en reward card) Abrir confirmación eliminar | Lista de rewards |

---

## Checklist UI/UX

### Gestión de Recompensas (Dashboard)
- [ ] Breadcrumb/back link a campaña funcional
- [ ] Stats card con total, activas, stock calculado dinámicamente
- [ ] Lista de reward cards ordenables con dnd-kit
- [ ] Drag handles visibles al hover (desktop) / permanentes (mobile)
- [ ] Cada card muestra: precio, título, descripción (line-clamp-2), stock, backers count
- [ ] Iconos de stock correctos (infinity, package, x-circle) con colores
- [ ] Stock warning badge "X% vendido" cuando < 50% disponible
- [ ] Botones Edit (primary color) y Delete (red) en cada card
- [ ] Botón "+ Agregar recompensa" outline dashed full-width
- [ ] Empty state con icono grande, texto, CTA gradient
- [ ] Tip box con icono lightbulb al final
- [ ] Drag state visual (opacity 0.5, cursor grabbing)
- [ ] Drop zone indicator (línea gradient horizontal)
- [ ] Reordenamiento smooth con animation
- [ ] Auto-save del nuevo orden vía API
- [ ] Toast "Orden actualizado" en success
- [ ] Loading skeletons mientras carga lista
- [ ] Responsive: mobile (sidebar hamburger, drag handles permanentes, botones icon-only)
- [ ] Accesibilidad: botones ↑↓ para reordenar sin mouse, ARIA labels

### Modal Create/Edit Reward
- [ ] Modal overlay con backdrop blur
- [ ] Título dinámico: "Nueva Recompensa" o "Editar Recompensa"
- [ ] Botón X cerrar con confirmación si hay cambios
- [ ] Form con react-hook-form + Zod validation
- [ ] Input nombre con label required asterisk, max 100 caracteres
- [ ] Character counter nombre (X/100) con color warning a 80%
- [ ] Textarea descripción con label required, max 1000 caracteres
- [ ] Character counter descripción (X/1000) con color warning a 800
- [ ] Input importe mínimo con € prefix, type number step 0.01
- [ ] Select tipo recompensa (dropdown con opciones)
- [ ] Select moneda (default EUR)
- [ ] Checkbox "Es add-on" con label descriptivo
- [ ] Checkbox "Stock limitado" con conditional section
- [ ] Conditional section fade-in animation cuando checkbox checked
- [ ] Input cantidad máxima (dentro de conditional, required si checked)
- [ ] Input máximo por backer (opcional, validar <= cantidad máxima)
- [ ] Checkbox "Incluye envío físico" con conditional section
- [ ] Input tiempo de entrega estimado (dentro de conditional)
- [ ] Validation errors en tiempo real (debounced 300ms)
- [ ] Error messages debajo de inputs en rojo
- [ ] Botón Cancelar outline con confirmación si hay cambios
- [ ] Botón Guardar gradient con loading state
- [ ] Loading state: spinner + "Guardando...", inputs disabled
- [ ] Toast success "Recompensa creada/actualizada"
- [ ] Toast error con mensaje del backend
- [ ] Modal cierra en success, lista refresh
- [ ] Responsive: mobile (max-w-full, grid → stack, botones stack vertical)
- [ ] Accesibilidad: focus trap, Esc cierra, labels con htmlFor, ARIA

### Dialog Delete Confirmation
- [ ] Dialog overlay con backdrop blur
- [ ] Variante 1: "Eliminar Recompensa" (no backings)
- [ ] Variante 2: "No se puede eliminar" (has backings)
- [ ] Warning icon alert-triangle
- [ ] Texto confirmación con nombre de reward highlighted
- [ ] Texto "Esta acción no se puede deshacer" en variante 1
- [ ] Texto explicativo con número de backings en variante 2
- [ ] Botón Cancelar outline
- [ ] Botón Eliminar (red) en variante 1
- [ ] Botón Desactivar (warning) en variante 2
- [ ] Loading state en botones de acción
- [ ] Dialog cierra en success
- [ ] Toast "Recompensa eliminada" o "Recompensa desactivada"
- [ ] Toast error si falla operación
- [ ] Responsive: mobile (max-w-full)
- [ ] Accesibilidad: role="alertdialog", Esc cierra, focus en botón primario

### Public Reward Cards (Landing)
- [ ] Sidebar sticky en desktop, scroll normal en mobile
- [ ] Botón "Apoyar esta campaña" gradient full-width arriba
- [ ] Texto "Desde € X" (precio mínimo) debajo del botón
- [ ] Divider horizontal antes de rewards
- [ ] Título "Recompensas" h3 bold
- [ ] Reward cards ordenadas por precio ascendente
- [ ] Cada card: precio (text-xl bold primary), título (font-semibold white)
- [ ] Badge "Más popular" rosa en reward con más backings
- [ ] Badge "Pocas unidades" warning cuando stock < 50%
- [ ] Badge "AGOTADO" gris en cards sold out
- [ ] Descripción reward (text-sm secondary)
- [ ] Stock info con icono + texto (package/alert-circle/x-circle)
- [ ] Botón "Seleccionar" outline primary en cada card
- [ ] Botón "Agotado" disabled gris en cards sold out
- [ ] Hover effect en cards disponibles (border primary, elevation)
- [ ] No hover en cards sold out
- [ ] Cards sold out con opacity 60%
- [ ] Empty state si no hay rewards: texto + solo botón general "Apoyar"
- [ ] Footer sidebar: icono lock + "Pago seguro"
- [ ] Footer sidebar: icono truck + "Entrega estimada: [fecha]"
- [ ] Loading skeletons para rewards
- [ ] Responsive: mobile (sidebar no sticky, padding reducido, font sizes reducidos)
- [ ] Accesibilidad: cards con role="article", botones con aria-label descriptivo

### Wizard Step 3 Integration
- [ ] Stepper muestra pasos 1-2 completados, paso 3 activo
- [ ] Lista de rewards creados (sin drag handles)
- [ ] Orden automático por importe (no manual)
- [ ] Botones Edit/Delete en cada card
- [ ] Modal form reutilizado (mismo componente que gestión)
- [ ] Botón "+ Agregar recompensa" outline dashed
- [ ] Empty state con CTA "Crear primera recompensa"
- [ ] Permitir avanzar sin rewards (warning en paso 4)
- [ ] Al volver de paso 4, rewards guardados aparecen
- [ ] Botón Anterior funcional (volver a paso 2)
- [ ] Botón Siguiente funcional (avanzar a paso 4)
- [ ] Responsive: mobile (stack vertical)
- [ ] Accesibilidad: focus states, keyboard navigation

### Cross-cutting
- [ ] Design tokens consistentes con crear-campania
- [ ] Dark theme aplicado (#1a1a2e, #0f1729)
- [ ] Gradient buttons (pink → purple) en CTAs principales
- [ ] shadcn/ui components: Card, Button, Input, Textarea, Dialog, Badge, Select, Checkbox
- [ ] dnd-kit implementado para drag & drop
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-400ms)
- [ ] Contraste mínimo 4.5:1 verificado
- [ ] Focus states con ring purple en todos interactivos
- [ ] Form validation con Zod + react-hook-form
- [ ] TanStack Query para mutations (create, update, delete, reorder)
- [ ] Manejo de errores del backend (ServiceResponse)
- [ ] Toast notifications configuradas (sonner)
- [ ] Mobile-first responsive design
- [ ] Skeleton loaders para loading states
- [ ] Empty states con iconos y CTAs claros
- [ ] Stock calculado dinámicamente (max - backings)
- [ ] Orden persistido en DB (campo `orden` en Reward)
- [ ] API endpoints: GET, POST, PUT, DELETE `/api/rewards`
- [ ] API endpoint: PUT `/api/rewards/{id}/reorder` (actualizar orden)
- [ ] API endpoint: PUT `/api/rewards/{id}/deactivate` (desactivar)
