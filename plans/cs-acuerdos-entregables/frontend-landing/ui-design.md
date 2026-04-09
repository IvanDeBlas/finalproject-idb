# Diseno UI: cs-acuerdos-entregables (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Target:** src/web (Vite + React 18, puerto 3000)

---

## 1. Resumen

- Componentes shadcn/ui: Dialog, Card, Badge, Button, Input, Textarea, Label, Select, Progress, Avatar, Alert, Skeleton, Tooltip
- Composiciones custom: 6 (AcuerdoDetailPage, MilestoneItem, EntregableRow, EstadoAcuerdoBadge, EstadoEntregableBadge, AcuerdoDetailSkeleton)
- Dialogs: 8 (AceptarPropuesta, RechazarPropuesta, MilestoneForm, EntregableForm, AprobarEntregable, RechazarEntregable, CompletarAcuerdo, CancelarAcuerdo)
- Responsive breakpoints: mobile (< 640px), tablet (640px - 1024px), desktop (> 1024px)
- Dark theme: gradients pink/purple, fondo #0f1729 / #1a1a2e / #16213e

---

## 2. Paleta de Colores

| Uso | Variable CSS / Valor | Ejemplo de Uso |
|-----|---------------------|----------------|
| Background card | `#0f1729` | Cards principales |
| Background card secundario | `#16213e` | Cards anidados, resumen sections |
| Background input | `#1a1a2e` | Inputs, Textareas |
| Background hover | `#1e2a42` | Hover de items, botones outline hover |
| Text primary | `#ffffff` | Titulos, valores importantes |
| Text secondary | `#94a3b8` | Labels, descripciones |
| Text muted | `#64748b` | Contadores, fechas, placeholders |
| Text label | `#cbd5e1` | Labels de formulario |
| Border default | `#334155` | Bordes de cards, inputs |
| Border focus | `#a855f7` | Focus ring en inputs |
| Primary gradient | `from-pink-500 to-purple-600` | Botones primarios, progress fill |
| Primary gradient hover | `from-pink-600 to-purple-700` | Hover estado del boton primario |
| Estado Acuerdo Activo | `bg-blue-900/30 border-blue-700 text-blue-300` | Badge Activo |
| Estado Acuerdo Completado | `bg-green-900/30 border-green-700 text-green-300` | Badge Completado |
| Estado Acuerdo Cancelado | `bg-red-900/30 border-red-700 text-red-300` | Badge Cancelado |
| Estado Entregable Entregado | `bg-amber-900/30 border-amber-600 text-amber-300` | Badge Entregado |
| Estado Entregable Aprobado | `bg-green-900/30 border-green-600 text-green-300` | Badge Aprobado |
| Estado Entregable Rechazado | `bg-red-900/30 border-red-600 text-red-300` | Badge Rechazado |
| Milestone Pendiente | `bg-gray-900/30 border-gray-600 text-gray-300` | Badge Pendiente |
| Milestone Completado | `bg-green-900/30 border-green-600 text-green-300` | Badge Completado |
| Warning (amber) | `bg-amber-900/20 border-amber-700/50 text-amber-300` | Alert de advertencia |
| Danger (red) | `bg-red-900/20 border-red-700/50 text-red-300` | Alert de peligro, acciones destructivas |
| Info (blue) | `bg-blue-900/20 border-blue-700/50 text-blue-300` | Alert informativo |
| Neutral info | `bg-[#16213e] border-[#334155] text-[#94a3b8]` | Alert neutro |
| Destructive button | `bg-red-600 hover:bg-red-700 text-white` | Botones de acciones destructivas |
| Approve button | `bg-green-600 hover:bg-green-700 text-white` | Boton aprobar |

---

## 3. Componentes Reutilizables

### 3.1 EstadoAcuerdoBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EstadoAcuerdoBadge.tsx`

**Props Interface:**
```typescript
interface EstadoAcuerdoBadgeProps {
    estado: "Activo" | "Completado" | "Cancelado";
    className?: string;
}
```

**Componente shadcn:** `Badge`

**Mapa de estilos:**

| Estado | Clases Tailwind |
|--------|----------------|
| Activo | `bg-blue-900/30 border-blue-700 text-blue-300` |
| Completado | `bg-green-900/30 border-green-700 text-green-300` |
| Cancelado | `bg-red-900/30 border-red-700 text-red-300` |

**Composicion:**
```tsx
const ESTADO_ACUERDO_STYLES: Record<string, string> = {
    Activo: "bg-blue-900/30 border-blue-700 text-blue-300",
    Completado: "bg-green-900/30 border-green-700 text-green-300",
    Cancelado: "bg-red-900/30 border-red-700 text-red-300",
};

<Badge
    className={`border text-sm px-3 py-1 font-medium ${ESTADO_ACUERDO_STYLES[estado]} ${className}`}
    aria-label={`Estado del acuerdo: ${estado}`}
>
    {estado}
</Badge>
```

**Patron:** Mismo patron que `EstadoPropuestaBadge` ya existente en el proyecto.

---

### 3.2 EstadoEntregableBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EstadoEntregableBadge.tsx`

**Props Interface:**
```typescript
interface EstadoEntregableBadgeProps {
    estado: "Entregado" | "Aprobado" | "Rechazado";
    className?: string;
}
```

**Componente shadcn:** `Badge`

**Mapa de estilos:**

| Estado | Clases Tailwind |
|--------|----------------|
| Entregado | `bg-amber-900/30 border-amber-600 text-amber-300` |
| Aprobado | `bg-green-900/30 border-green-600 text-green-300` |
| Rechazado | `bg-red-900/30 border-red-600 text-red-300` |

**Composicion:**
```tsx
<Badge
    className={`border text-xs font-medium px-2.5 py-0.5 rounded-full ${ESTADO_ENTREGABLE_STYLES[estado]} ${className}`}
    aria-label={`Estado del entregable: ${estado}`}
>
    {estado}
</Badge>
```

---

### 3.3 MilestoneItem

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MilestoneItem.tsx`

**Props Interface:**
```typescript
interface MilestoneItemProps {
    milestone: Milestone;          // type de src/shared/types/crowdsourcing.ts
    acuerdoId: string;
    monedaNombre: string;
    acuerdoActivo: boolean;
    miRol: "Artista" | "Profesional";
    onEditar?: (milestone: Milestone) => void;
    onEliminar?: (milestoneId: string) => void;
    onSubirEntregable?: (milestoneId: string) => void;
    onAprobarEntregable?: (entregableId: string) => void;
    onRechazarEntregable?: (entregableId: string) => void;
}
```

**Componentes shadcn:** `Badge`, `Button`, `Tooltip`, `TooltipTrigger`, `TooltipContent`

**Iconos Lucide:** `Pencil`, `Trash2`, `Upload`, `CheckCircle`

**Layout:**
```
┌──────────────────────────────────────────────────────┐
│  [#numero] Titulo del milestone      270 EUR  [✏][🗑] │
│            Limite: 8 mar 2026 | [PENDIENTE]           │
│                                                       │
│  ┌──────────────────────────────────────────────┐     │
│  │ EntregableRow (estado Entregado)             │     │  <- border-l-amber
│  └──────────────────────────────────────────────┘     │
│  ┌──────────────────────────────────────────────┐     │
│  │ EntregableRow (estado Aprobado)              │     │  <- border-l-green
│  └──────────────────────────────────────────────┘     │
│                                                       │
│  [+ Subir entregable]  (solo profesional + activo)    │
└──────────────────────────────────────────────────────┘
```

**Estilos del container:**
- Container: `bg-[#16213e] border border-[#334155] rounded-lg p-4 hover:bg-[#1e2a42] transition-colors duration-150`
- Header row: `flex items-start justify-between gap-2 mb-2`
- Numero: `text-sm font-bold text-[#a855f7] mr-1 flex-shrink-0`
- Titulo: `text-sm font-semibold text-white flex-1 min-w-0`
- Importe: `text-sm font-medium text-white ml-auto flex-shrink-0`
- Iconos editar/eliminar: `text-[#64748b] hover:text-white w-4 h-4 cursor-pointer transition-colors`

**Logica de visibilidad de acciones:**
- Icono editar (`Pencil`): solo si `miRol === "Artista"` y `acuerdoActivo` y `!milestone.fechaCompletado`
- Icono eliminar (`Trash2`): solo si `miRol === "Artista"` y `acuerdoActivo` y `!milestone.fechaCompletado` y `milestone.entregables.length === 0`
- Boton [+ Subir entregable]: solo si `miRol === "Profesional"` y `acuerdoActivo`

**Estilos de fecha limite:**
- Normal: `text-xs text-[#64748b]`
- Proximo (dentro de 3 dias): `text-xs text-amber-400`
- Vencido (pasada sin completar): `text-xs text-red-400`

**Badge de estado del milestone:**

| Estado | Clases |
|--------|--------|
| Pendiente (sin fechaCompletado) | `bg-gray-900/30 border-gray-600 text-gray-300` |
| Completado (con fechaCompletado) | `bg-green-900/30 border-green-600 text-green-300` |

**Alerta de sugerencia (todos los entregables aprobados):**
```tsx
// Visible solo cuando: todos los entregables del milestone estan Aprobados
// y el milestone no esta completado aun
<Alert className="bg-green-900/20 border-green-700/50 mt-3">
    <CheckCircle className="w-4 h-4 text-green-400" aria-hidden="true" />
    <AlertDescription className="text-sm text-green-300 flex items-center justify-between">
        <span>Todos los entregables han sido aprobados.</span>
        <Button size="sm" className="ml-3 h-7 px-3 text-xs bg-green-600 hover:bg-green-700">
            Marcar completado
        </Button>
    </AlertDescription>
</Alert>
```

---

### 3.4 EntregableRow

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EntregableRow.tsx`

**Props Interface:**
```typescript
interface EntregableRowProps {
    entregable: Entregable;        // type de src/shared/types/crowdsourcing.ts
    acuerdoActivo: boolean;
    miRol: "Artista" | "Profesional";
    onAprobar?: (id: string) => void;
    onRechazar?: (id: string) => void;
}
```

**Componentes shadcn:** `Button`, y `EstadoEntregableBadge` (custom)

**Iconos Lucide:** `ExternalLink`, `CheckCircle`, `XCircle`

**Layout:**
```
┌── [border-l-2 segun estado] ───────────────────────────┐
│  Titulo del entregable                      [ESTADO]   │
│  Descripcion truncada (max 80 chars)...                │
│  [Enlace ->]  |  5 mar 2026                            │
│  "Comentario de aprobacion/rechazo" (cursiva)          │
│                                                        │
│  [Aprobar]  [Rechazar]  (si Entregado y artista)       │
└────────────────────────────────────────────────────────┘
```

**Estilos:**

| Elemento | Clases Tailwind |
|----------|----------------|
| Container (Entregado) | `pl-4 py-3 border-l-2 border-amber-600/50 mb-2 last:mb-0` |
| Container (Aprobado) | `pl-4 py-3 border-l-2 border-green-600/50 mb-2 last:mb-0` |
| Container (Rechazado) | `pl-4 py-3 border-l-2 border-red-600/50 mb-2 last:mb-0` |
| Container (Default) | `pl-4 py-3 border-l-2 border-[#334155] mb-2 last:mb-0` |
| Titulo | `text-sm font-medium text-white` |
| Descripcion | `text-xs text-[#94a3b8] mt-0.5 line-clamp-2` |
| URL Link | `text-xs text-[#a855f7] hover:text-purple-300 flex items-center gap-1 mt-1` |
| Fecha | `text-xs text-[#64748b]` |
| Comentario | `text-xs text-[#64748b] italic mt-1` |
| Acciones row | `flex gap-2 mt-2` |

**Boton Aprobar:**
```
size="sm", h-7 px-3 text-xs
className="bg-green-600 hover:bg-green-700 text-white"
icono: CheckCircle w-3 h-3 mr-1
```

**Boton Rechazar:**
```
size="sm", variant="outline", h-7 px-3 text-xs
className="border-red-700/50 text-red-400 hover:bg-red-900/20 hover:border-red-600"
icono: XCircle w-3 h-3 mr-1
```

**Visibilidad de botones:** Solo si `entregable.estadoEntregableNombre === "Entregado"` y `miRol === "Artista"` y `acuerdoActivo`

**Accesibilidad:**
- Link URL: `target="_blank" rel="noopener noreferrer"` con texto descriptivo o aria-label
- Botones Aprobar/Rechazar con aria-label incluyendo el titulo del entregable: `aria-label="Aprobar entregable: {titulo}"`

---

### 3.5 ImporteAsignadoBar

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ImporteAsignadoBar.tsx`

**Props Interface:**
```typescript
interface ImporteAsignadoBarProps {
    importeAsignado: number;
    importeTotal: number;
    monedaNombre: string;
    importePreview?: number;   // Para preview en tiempo real al editar importe de milestone
    showError?: boolean;       // Cuando la suma supera el total
}
```

**Componente shadcn:** `Progress`

**Composicion:**
```tsx
// El valor de Progress va de 0 a 100
// Si showError: barra en rojo, texto de error visible
// Si importePreview: calcula porcentaje incluyendo el preview

<div className="bg-[#16213e] border border-[#334155] rounded-lg p-3">
    <Progress
        value={porcentaje}
        className={`h-2 mb-2 bg-[#334155] ${showError ? "[&>div]:bg-red-500" : "[&>div]:bg-gradient-to-r [&>div]:from-pink-500 [&>div]:to-purple-600"}`}
        aria-label={`Importe asignado: ${importeAsignado} de ${importeTotal} ${monedaNombre}`}
    />
    <p className={`text-sm ${showError ? "text-red-400" : "text-[#94a3b8]"}`}>
        Asignado: {importeAsignado} de {importeTotal} {monedaNombre} ({porcentaje}%)
    </p>
    {showError && (
        <p className="text-sm text-red-400 mt-1">
            El importe supera el total pactado ({importeTotal} {monedaNombre})
        </p>
    )}
    {!showError && (
        <p className="text-xs text-[#64748b] mt-1">
            Disponible: {importeTotal - importeAsignado} {monedaNombre}
        </p>
    )}
</div>
```

---

### 3.6 AcuerdoDetailSkeleton

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AcuerdoDetailSkeleton.tsx`

**Componente shadcn:** `Skeleton`, `Card`

**Composicion (documentada en ui-ux.md, referencia exacta):**
```tsx
// Header skeleton
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
    <div className="flex items-start justify-between mb-4">
        <Skeleton className="h-8 w-2/3 bg-[#1e2a42]" />
        <Skeleton className="h-6 w-20 rounded-full bg-[#1e2a42]" />
    </div>
    <div className="flex items-center gap-4 mb-3">
        <Skeleton className="h-8 w-8 rounded-full bg-[#1e2a42]" />
        <Skeleton className="h-4 w-32 bg-[#1e2a42]" />
        <Skeleton className="h-4 w-8 bg-[#1e2a42]" />
        <Skeleton className="h-8 w-8 rounded-full bg-[#1e2a42]" />
        <Skeleton className="h-4 w-32 bg-[#1e2a42]" />
    </div>
    <Skeleton className="h-4 w-48 mb-2 bg-[#1e2a42]" />
    <Skeleton className="h-4 w-64 bg-[#1e2a42]" />
</Card>

// Milestones skeleton
<Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
    <div className="flex justify-between mb-4">
        <Skeleton className="h-5 w-28 bg-[#1e2a42]" />
        <Skeleton className="h-8 w-32 rounded-md bg-[#1e2a42]" />
    </div>
    <Skeleton className="h-2 w-full mb-4 rounded-full bg-[#1e2a42]" />
    {[1, 2].map(i => (
        <div key={i} className="bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-3">
            <Skeleton className="h-5 w-3/4 mb-2 bg-[#1e2a42]" />
            <Skeleton className="h-4 w-24 mb-3 bg-[#1e2a42]" />
            <Skeleton className="h-12 w-full rounded bg-[#1e2a42]" />
        </div>
    ))}
</Card>
```

**Sidebar skeleton:**
```tsx
<aside className="w-72 flex-shrink-0 space-y-4">
    <Card className="bg-[#0f1729] border-[#334155] p-4">
        <Skeleton className="h-5 w-full bg-[#1e2a42]" />
    </Card>
    <Card className="bg-[#0f1729] border-[#334155] p-4 space-y-3">
        <Skeleton className="h-3 w-16 bg-[#1e2a42]" />
        <Skeleton className="h-4 w-28 bg-[#1e2a42]" />
        <Skeleton className="h-3 w-16 bg-[#1e2a42]" />
        <Skeleton className="h-4 w-28 bg-[#1e2a42]" />
    </Card>
</aside>
```

---

## 4. Pantalla 1: Aceptar Propuesta (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AceptarPropuestaDialog.tsx`

**Context:** Dialog overlay sobre `NecesidadDetallePage`

### Layout Desktop
```
┌──────────────────────────────────────────────────────┐
│  Aceptar propuesta                             [×]   │
│  Confirma los detalles del acuerdo a crear           │
├──────────────────────────────────────────────────────┤
│  RESUMEN DE LA PROPUESTA                             │
│  ┌──────────────────────────────────────────────┐    │
│  │ Profesional: Studio Mix Pro                  │    │
│  │ Precio pactado: 450 EUR                      │    │
│  │ Mensaje: "Soy ingeniero de mezcla..."        │    │
│  └──────────────────────────────────────────────┘    │
│                                                      │
│  Titulo interno *                                    │
│  [Mezcla de pistas para EP                      ]   │
│  Nombre interno para identificar el acuerdo          │
│                                                      │
│  Fecha de inicio *       Fecha fin prevista           │
│  [2026-03-01         ]   [2026-03-15            ]    │
│                                                      │
│  (i) Al aceptar, las demas propuestas seran          │
│  rechazadas automaticamente.                         │
├──────────────────────────────────────────────────────┤
│  [Cancelar]                        [Crear acuerdo]  │
└──────────────────────────────────────────────────────┘
```

### Props Interface
```typescript
interface AceptarPropuestaDialogProps {
    propuesta: {
        id: string;
        profesionalNombre: string;
        precioPropuesto: number;
        monedaNombre: string;
        mensaje: string;
        diasEstimados?: number;
    };
    necesidadTitulo: string;
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (acuerdoId: string) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog container | `Dialog` | `open={isOpen} onOpenChange` |
| Dialog content | `DialogContent` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog header | `DialogHeader` | - |
| Dialog title | `DialogTitle` | `text-xl font-semibold text-white` |
| Dialog description | `DialogDescription` | `text-sm text-[#94a3b8] mt-1` |
| Resumen section | `div` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-5` |
| Titulo interno label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Titulo interno input | `Input` | `bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7]` |
| Fechas row | `div` | `grid grid-cols-2 gap-3 sm:grid-cols-2 grid-cols-1` |
| Fecha inicio input | `Input type="date"` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Info alert | `Alert` | `bg-blue-900/20 border-blue-700/50 mt-4` |
| Info icon | - | `Info` Lucide, `w-4 h-4 text-blue-400 flex-shrink-0` |
| Info text | `AlertDescription` | `text-sm text-blue-300` |
| Error de campo | `p` | `text-sm text-red-400 mt-1` con `role="alert"` |
| Dialog footer | `DialogFooter` | `pt-4 border-t border-[#334155] flex justify-between gap-3` |
| Cancelar button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit button | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Submit loading | `Button disabled` | + `Loader2 animate-spin w-4 h-4 mr-2` |

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default | Form con valores prellenados (titulo=necesidadTitulo, fechaInicio=hoy, fechaFin=hoy+diasEstimados) |
| Validation Error | Border `border-red-500` en campo invalido, mensaje rojo debajo, submit disabled |
| Error shake | Clase `error-shake` (keyframes shake 400ms) en el Dialog al submit fallido |
| Submitting | Spinner en boton + "Creando acuerdo...", todos los inputs `disabled` |
| Success | Dialog cierra, toast success, redirect a `/crowdsourcing/acuerdos/{acuerdoId}` |
| Error API | Toast error descriptivo, dialog permanece abierto |

### Validaciones Visuales
| Campo | Estado Error | Visual |
|-------|-------------|--------|
| Titulo interno | < 3 chars | `border-red-500` + mensaje "El titulo debe tener al menos 3 caracteres" |
| Fecha inicio | vacia | `border-red-500` + mensaje "La fecha de inicio es obligatoria" |
| Fecha fin prevista | < fechaInicio | `border-red-500` + mensaje "La fecha fin no puede ser anterior a la fecha de inicio" |

### Accesibilidad
- `DialogTitle` vinculado automaticamente al dialogo via Radix
- `aria-required="true"` en campos obligatorios
- Required mark: `<span aria-hidden="true" className="text-red-400 ml-1">*</span>`
- Error messages con `role="alert"` y `aria-live="polite"`
- Focus atrapado dentro del Dialog mientras esta abierto (Radix lo maneja)
- Escape key cierra el dialog (Radix lo maneja)

---

## 5. Pantalla 2: Rechazar Propuesta (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RechazarPropuestaDialog.tsx`

**Nota:** Verificar si ya existe archivo con este nombre. Si existe `RetirarPropuestaDialog.tsx`, este es un componente diferente para rechazar (desde el lado artista).

### Props Interface
```typescript
interface RechazarPropuestaDialogProps {
    propuesta: {
        id: string;
        profesionalNombre: string;
    };
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Warning alert | `Alert` | `bg-amber-900/20 border-amber-700/50 mb-4` |
| Warning icon | - | `AlertTriangle` Lucide, `w-4 h-4 text-amber-400 flex-shrink-0` |
| Warning text | `AlertDescription` | `text-sm text-amber-300` |
| Motivo label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Motivo textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[100px] resize-none` |
| Contador | `span` | `text-xs text-[#64748b]` "{count} / 500 caracteres" |
| Info alert | `Alert` | `bg-[#16213e] border-[#334155] mt-3` |
| Info text | `AlertDescription` | `text-sm text-[#94a3b8]` |
| Cancelar button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Rechazar button | `Button variant="destructive"` | `bg-red-600 hover:bg-red-700 text-white` |

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default | Textarea vacio, contador en "0 / 500" |
| Typing | Contador actualizado en tiempo real |
| At limit (500) | Contador `text-red-400`, textarea con `maxLength={500}` |
| Submitting | Spinner + "Rechazando...", inputs disabled |
| Success | Dialog cierra, toast "Propuesta rechazada", badge propuesta -> [RECHAZADA] |

---

## 6. Pantalla 3: Detalle de Acuerdo (Page)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/AcuerdoDetallePage.tsx`

**Ruta:** `/crowdsourcing/acuerdos/:id`

### Layout Desktop
```
┌──────────────────────────────────────────────────────────────┐
│  [NAVBAR]                                                    │
├──────────────────────────────────────────────────────────────┤
│  max-w-6xl mx-auto px-4 py-8                                 │
│                                                              │
│  [< Mis acuerdos]                                            │
│                                                              │
│  ┌─────────────────────────────────────────────────────┐     │
│  │  HEADER CARD (Card bg-[#0f1729])                    │     │
│  │  Titulo h1                              [ACTIVO]    │     │
│  │  [Avatar] Artista  <->  [Avatar] Profesional        │     │
│  │  450 EUR  |  1 mar 2026 - 15 mar 2026               │     │
│  │  Necesidad: "Mezcla de pistas..."                   │     │
│  └─────────────────────────────────────────────────────┘     │
│                                                              │
│  ┌────────────────────────────────────┐  ┌───────────────┐   │
│  │  MILESTONES CARD                   │  │  SIDEBAR      │   │
│  │  Milestones        [+ Agregar]     │  │               │   │
│  │  Asignado: 270/450 EUR (60%)       │  │  [Ir al chat] │   │
│  │  [Progress bar]                    │  │               │   │
│  │                                    │  │  Detalles     │   │
│  │  [MilestoneItem 1]                 │  │  Inicio:...   │   │
│  │  [MilestoneItem sin milestone]     │  │  Fin:...      │   │
│  │                                    │  │  Fin real:... │   │
│  │  TIMELINE                          │  └───────────────┘   │
│  │  - Evento 1                        │                      │
│  │  - Evento 2                        │                      │
│  │                                    │                      │
│  │  [Completar acuerdo]               │                      │
│  │  [Cancelar acuerdo]                │                      │
│  └────────────────────────────────────┘                      │
└──────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 640px)
```
┌──────────────────────────────────────┐
│  [< Mis acuerdos]                    │
│  Titulo                   [ACTIVO]   │
│  Artista <-> Profesional             │
│  450 EUR | 1 mar - 15 mar 2026       │
│                                      │
│  [Ir al chat con Profesional ->]     │
│                                      │
│  [Ver detalles v] (collapsible)      │
│                                      │
│  MILESTONES           [+ Agregar]    │
│  Asignado: 270/450 EUR               │
│  [Progress bar]                      │
│  [MilestoneItem 1]                   │
│                                      │
│  TIMELINE                            │
│  - Evento 1                          │
│                                      │
│  [Completar acuerdo]  (full width)   │
│  [Cancelar acuerdo]   (full width)   │
└──────────────────────────────────────┘
```

### Componentes shadcn por Seccion

**Header Card:**
| Elemento | Componente | Clases Tailwind |
|----------|------------|----------------|
| Page container | `div` | `max-w-6xl mx-auto px-4 py-8` |
| Layout row | `div` | `flex gap-8 items-start` |
| Main content | `div` | `flex-1 min-w-0` |
| Back link | `Link` | `flex items-center gap-2 text-[#94a3b8] hover:text-white mb-6` + `ArrowLeft w-4 h-4` |
| Header card | `Card` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Header top row | `div` | `flex items-start justify-between mb-4` |
| Titulo | `h1` | `text-2xl font-bold text-white leading-tight` |
| Estado badge | `EstadoAcuerdoBadge` | custom, `text-sm px-3 py-1` |
| Parties row | `div` | `flex items-center gap-4 mb-3 flex-wrap` |
| Party avatar | `Avatar` + `AvatarFallback` | `w-8 h-8`, iniciales del nombre |
| Party name | `span` | `text-sm font-medium text-white` |
| Parties separator | `span` | `text-[#64748b]` |
| Importe | `span` | `text-lg font-bold text-white` |
| Dates | `div` | `text-sm text-[#94a3b8]` |
| Necesidad link | `Link` | `text-sm text-[#a855f7] hover:text-purple-300 mt-2 block` |

**Milestones Section:**
| Elemento | Componente | Clases Tailwind |
|----------|------------|----------------|
| Section card | `Card` | `bg-[#0f1729] border-[#334155] p-6 mb-6` |
| Section header | `div` | `flex items-center justify-between mb-4` |
| Section title | `h2` | `text-base font-semibold text-white` |
| Agregar btn | `Button size="sm"` | `bg-gradient-to-r from-pink-500 to-purple-600 text-sm` + `Plus w-4 h-4 mr-1` |
| Progreso label | `p` | `text-sm text-[#94a3b8] mb-2` |
| Progress bar | `Progress` | `h-2 bg-[#334155] [&>div]:bg-gradient-to-r [&>div]:from-pink-500 [&>div]:to-purple-600` |
| Milestones list | `div` | `space-y-4 mt-4` |

**Sidebar:**
| Elemento | Componente | Clases Tailwind |
|----------|------------|----------------|
| Sidebar | `aside` | `w-72 flex-shrink-0 hidden md:block sticky top-24 space-y-4` |
| Chat card | `Card` | `bg-[#0f1729] border-[#334155] p-4` |
| Chat link | `Link` | `flex items-center gap-2 text-[#a855f7] hover:text-purple-300 font-medium` + `MessageSquare w-4 h-4` |
| Detalles card | `Card` | `bg-[#0f1729] border-[#334155] p-4` |
| Detalle label | `span` | `text-xs text-[#64748b] uppercase tracking-wide` |
| Detalle value | `span` | `text-sm font-medium text-white` |

**Timeline:**
| Elemento | Componente | Clases Tailwind |
|----------|------------|----------------|
| Timeline section | `div` | `mt-6` |
| Timeline title | `h3` | `text-sm font-semibold text-[#64748b] uppercase tracking-wide mb-3` |
| Timeline list | `ul` | `space-y-3` |
| Timeline item | `li` | `flex items-start gap-3` |
| Timeline dot | `div` | `w-2 h-2 rounded-full bg-[#334155] mt-1.5 flex-shrink-0` |
| Timeline text | `span` | `text-sm text-[#94a3b8]` |
| Timeline date | `span` | `text-xs text-[#64748b] ml-1` |

**Actions Row:**
| Elemento | Componente | Clases Tailwind |
|----------|------------|----------------|
| Actions container | `div` | `flex gap-3 mt-8 pt-6 border-t border-[#334155]` |
| Completar btn | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600` + `CheckCircle w-4 h-4 mr-2` |
| Cancelar btn | `Button variant="outline"` | `border-red-700/50 text-red-400 hover:bg-red-900/20 hover:border-red-600` + `XCircle w-4 h-4 mr-2` |

### Estados de UI - Pagina Completa
| Estado | Visual |
|--------|--------|
| Loading | `AcuerdoDetailSkeleton` componente completo |
| Loaded (Activo) | Contenido completo, botones contextuales segun rol |
| Loaded (Completado) | Badge verde, sin botones Agregar/Subir/Aprobar/Rechazar, sin Actions Row |
| Loaded (Cancelado) | Badge rojo, todos los botones ocultos, banner informativo con fecha y quien cancelo |
| Error 403 | Icono `Lock` centrado + "No tienes acceso a este acuerdo" + `Button` "Volver al inicio" |
| Error 404 | Icono `SearchX` centrado + "Acuerdo no encontrado" + `Button` "Volver al inicio" |
| Error red | Icono `WifiOff` + "Error de conexion" + `Button` "Reintentar" |

**Empty states dentro de la pagina:**
| Contexto | Icono | Titulo | Accion |
|----------|-------|--------|--------|
| Sin milestones (artista) | `Milestone` | "Sin milestones definidos" | `Button` gradient "+ Agregar milestone" |
| Sin milestones (profesional) | `Milestone` | "Sin milestones definidos" | Sin accion |
| Sin entregables en milestone (profesional) | `Upload` | "Sin entregables" | `Button` sm "+ Subir entregable" |
| Sin entregables en milestone (artista) | `FileCheck` | "Sin entregables" | Sin accion |

---

## 7. Pantalla 4: Formulario de Milestone (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MilestoneFormDialog.tsx`

**Nota:** Mismo componente para crear (POST) y editar (PUT). El modo se determina por la presencia de `milestone` en las props.

### Props Interface
```typescript
interface MilestoneFormDialogProps {
    acuerdoId: string;
    importeTotalPactado: number;
    importeYaAsignado: number;      // Suma de todos los otros milestones
    monedaNombre: string;
    fechaInicioAcuerdo: string;
    milestone?: Milestone;           // Si undefined: modo crear; si presente: modo editar
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: MilestoneCreatedResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog title | `DialogTitle` | `text-xl font-semibold text-white` "Nuevo milestone" / "Editar milestone" |
| Titulo label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Titulo input | `Input` | `bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7]` |
| Descripcion label | `Label` | mismo |
| Descripcion textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| Descripcion counter | `span` | `text-xs text-[#64748b]` "{count} / 1000 caracteres" |
| Importe + Fecha row | `div` | `grid grid-cols-2 gap-3 sm:grid-cols-2 grid-cols-1` |
| Importe label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Importe parcial * ({moneda})" |
| Importe input | `Input type="number"` | `bg-[#1a1a2e] border-[#334155] text-white h-11` step="0.01" min="0.01" |
| Fecha label | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` "Fecha limite" |
| Fecha input | `Input type="date"` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Progreso indicator | `ImporteAsignadoBar` | componente custom, visible debajo del importe |
| Cancelar button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Submit button | `Button` | gradient, disabled si importe invalido o suma excede total |

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default (crear) | Campos vacios, `ImporteAsignadoBar` muestra estado actual |
| Default (editar) | Campos prellenados, `ImporteAsignadoBar` excluye importe del milestone actual |
| Typing importe | `ImporteAsignadoBar` actualiza en tiempo real (sin debounce) |
| Supera total | Barra roja, texto de error, submit disabled |
| Submitting | Spinner + "Creando milestone..." o "Guardando...", inputs disabled |
| Success | Dialog cierra, lista actualizada, barra de progreso recalculada |

### Validaciones Visuales
| Campo | Estado Error | Visual |
|-------|-------------|--------|
| Titulo | < 3 chars | `border-red-500` + mensaje rojo |
| Importe parcial | <= 0 | `border-red-500` + "El importe parcial debe ser mayor a 0" |
| Importe parcial | suma > total | `ImporteAsignadoBar` en modo error + mensaje |
| Fecha limite | < fechaInicio del acuerdo | `border-red-500` + mensaje |

---

## 8. Pantalla 5: Formulario de Entregable (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EntregableFormDialog.tsx`

### Props Interface
```typescript
interface EntregableFormDialogProps {
    acuerdoId: string;
    milestones: Array<{ id: string; titulo: string }>;
    milestoneIdPreseleccionado?: string;   // Si se abre desde un MilestoneItem especifico
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: EntregableCreatedResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-lg bg-[#0f1729] border-[#334155] text-white` |
| Dialog title | `DialogTitle` | `text-xl font-semibold text-white` "Subir entregable" |
| Titulo input | `Input` | `bg-[#1a1a2e] border-[#334155] text-white h-11` |
| Descripcion textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| Descripcion counter | `span` | `text-xs text-[#64748b]` "{count} / 1000" |
| URL label | `Label` | "URL del recurso" |
| URL input | `Input type="url"` | `bg-[#1a1a2e] border-[#334155] text-white h-11 pl-10` (para icono prefijo) |
| URL icono prefijo | - | `Link2` Lucide, posicionado absolutamente a la izquierda del input |
| URL valid icon | - | `CheckCircle w-4 h-4 text-green-400` visible a la derecha cuando URL valida |
| URL hint | `p` | `text-xs text-[#64748b] mt-1` |
| Milestone select label | `Label` | "Milestone asociado" |
| Milestone select | `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Select hint | `p` | `text-xs text-[#64748b] mt-1` "Opcional - vincula este entregable a un milestone" |
| Submit button | `Button` | gradient + `Upload w-4 h-4 mr-2` |

### Select Milestone - Opciones
```
"Sin milestone"  (valor: "")
"Mezcla de pistas 1-3"  (valor: milestone.id)
"Entrega final"  (valor: milestone.id)
...
```

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default | Campos vacios, Select en "Sin milestone", milestoneId preseleccionado si viene de MilestoneItem |
| URL valida | `CheckCircle` verde visible a la derecha del input |
| URL invalida (blur) | `border-red-500` + mensaje rojo |
| Submitting | Spinner + "Subiendo...", inputs disabled |
| Success | Dialog cierra, toast, entregable aparece en lista del milestone |

---

## 9. Pantalla 6: Aprobar Entregable (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AprobarEntregableDialog.tsx`

### Props Interface
```typescript
interface AprobarEntregableDialogProps {
    entregable: Entregable;
    milestoneId?: string;           // Para saber si el milestone tiene mas entregables
    totalEntregablesMilestone?: number;
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: AprobarEntregableResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Entregable info card | `div` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-4` |
| Entregable titulo | `p` | `text-sm font-semibold text-white mb-1` |
| Entregable desc | `p` | `text-sm text-[#94a3b8] mb-2` |
| Entregable URL | `a` | `text-sm text-[#a855f7] hover:text-purple-300 flex items-center gap-1` + `ExternalLink w-3 h-3` |
| Entregable fecha | `p` | `text-xs text-[#64748b] mt-1` |
| Comentario label | `Label` | "Comentario (opcional)" |
| Comentario textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none` |
| Contador | `span` | `text-xs text-[#64748b]` "{count} / 500" |
| Info alert (si hay mas entregables) | `Alert` | `bg-[#16213e] border-[#334155] mt-3` |
| Submit button | `Button` | `bg-green-600 hover:bg-green-700 text-white` + `CheckCircle w-4 h-4 mr-2` |

---

## 10. Pantalla 7: Rechazar Entregable (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RechazarEntregableDialog.tsx`

### Props Interface
```typescript
interface RechazarEntregableDialogProps {
    entregable: Entregable;
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: RechazarEntregableResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Entregable info card | `div` | Mismo que Pantalla 6 |
| Warning alert | `Alert` | `bg-amber-900/20 border-amber-700/50 mb-4` |
| Warning icon | - | `AlertTriangle w-4 h-4 text-amber-400 flex-shrink-0` |
| Warning text | `AlertDescription` | `text-sm text-amber-300` |
| Motivo label | `Label` | "Motivo del rechazo *" |
| Motivo textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[100px] resize-none` placeholder="Explica que debe corregirse..." |
| Counter row | `div` | `flex justify-between mt-1` |
| Counter min | `span` | `text-xs text-[#64748b]` "Minimo 10 caracteres" |
| Counter current | `span` | `text-xs` - `text-amber-400` si chars < 10 y usuario ha escrito; `text-[#64748b]` si no |
| Submit button | `Button variant="destructive"` | `bg-red-600 hover:bg-red-700 disabled:opacity-50` + `XCircle w-4 h-4 mr-2` |

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default | Textarea vacio, submit disabled |
| Insufficient (< 10 chars) | Contador amber, submit disabled |
| Valid (>= 10 chars) | Submit habilitado |
| Submitting | Spinner + "Rechazando...", inputs disabled |

---

## 11. Pantalla 8: Completar Acuerdo (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/CompletarAcuerdoDialog.tsx`

### Props Interface
```typescript
interface CompletarAcuerdoDialogProps {
    acuerdo: {
        id: string;
        importeTotalPactado: number;
        monedaNombre: string;
        milestonesTotal: number;
        milestonesCompletados: number;
        entregablesTotal: number;
        entregablesAprobados: number;
        entregablesPendientes: number;
    };
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: CompletarAcuerdoResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Resumen card | `div` | `bg-[#16213e] border border-[#334155] rounded-lg p-4 mb-4` |
| Resumen title | `h3` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2` "Resumen del acuerdo" |
| Stat row | `div` | `flex items-center justify-between py-1 border-b border-[#334155] last:border-0` |
| Stat label | `span` | `text-sm text-[#94a3b8]` |
| Stat value | `span` | `text-sm font-semibold text-white` |
| Warning alert (pendientes) | `Alert` | `bg-amber-900/20 border-amber-700/50 mb-4` - solo si `entregablesPendientes > 0` |
| Warning icon | - | `AlertTriangle w-4 h-4 text-amber-400` |
| Warning text | `AlertDescription` | `text-sm text-amber-300` |
| Efectos list | `ul` | `space-y-1 text-sm text-[#94a3b8] mt-3` |
| Efecto item | `li` | `flex items-center gap-2` + `ChevronRight w-3 h-3 text-green-400` |
| Submit button | `Button` | gradient + `CheckCircle w-4 h-4 mr-2` |

---

## 12. Pantalla 9: Cancelar Acuerdo (Dialog)

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/CancelarAcuerdoDialog.tsx`

### Props Interface
```typescript
interface CancelarAcuerdoDialogProps {
    acuerdo: {
        id: string;
        tituloInterno: string;
    };
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (result: CancelarAcuerdoResult) => void;
}
```

### Componentes shadcn
| Elemento | Componente shadcn | Clases Tailwind |
|----------|-------------------|-----------------|
| Dialog content | `DialogContent` | `max-w-md bg-[#0f1729] border-[#334155] text-white` |
| Danger alert | `Alert` | `bg-red-900/20 border-red-700/50 mb-4` |
| Danger icon | - | `AlertTriangle w-5 h-5 text-red-400 flex-shrink-0` |
| Danger text | `AlertDescription` | `text-sm text-red-300` |
| Motivo label | `Label` | "Motivo de la cancelacion *" |
| Motivo textarea | `Textarea` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none` placeholder="Explica el motivo de la cancelacion..." |
| Counter row | `div` | `flex justify-between mt-1` |
| Counter min | `span` | `text-xs text-[#64748b]` "Minimo 20 caracteres" |
| Counter current | `span` | `text-xs` - `text-amber-400` si chars < 20 y usuario ha escrito; `text-[#64748b]` si no |
| Efectos list | `ul` | `space-y-1 text-sm text-[#94a3b8] mt-4 mb-1` |
| Efecto item | `li` | `flex items-center gap-2` + `ChevronRight w-3 h-3 text-red-400` |
| Volver button | `Button variant="outline"` | `border-[#334155] text-white hover:bg-[#1e2a42]` |
| Cancelar submit | `Button variant="destructive"` | `bg-red-600 hover:bg-red-700 disabled:opacity-50` - disabled si < 20 chars |

### Estados de UI
| Estado | Visual |
|--------|--------|
| Default | Textarea vacio, submit disabled |
| Chars < 20 | Contador amber, submit disabled |
| Chars >= 20 | Submit habilitado |
| Submitting | Spinner + "Cancelando...", ambos botones disabled |
| Success | Dialog cierra, toast amber "Acuerdo cancelado", pagina actualiza badge a [CANCELADO], botones desaparecen |

---

## 13. Feedback y Estados Globales

### Loading
| Elemento | Implementacion |
|----------|----------------|
| Pagina de detalle | `AcuerdoDetailSkeleton` (header + milestones + sidebar) |
| Boton submit en dialog | `<Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />` + texto descriptivo |
| Progress bar | Transicion de width `transition-all duration-300 ease-out` |

### Error States
| Tipo | Icono Lucide | Titulo | Accion |
|------|-------------|--------|--------|
| Red error | `WifiOff` | "Error de conexion" | `Button` "Reintentar" |
| Not Found | `SearchX` | "Acuerdo no encontrado" | `Button` "Volver al inicio" |
| Forbidden | `Lock` | "Acceso restringido" | `Button` "Volver al inicio" |

**Template error state:**
```tsx
<div className="flex flex-col items-center justify-center py-20 text-center">
    <{Icon} className="w-12 h-12 text-[#64748b] mb-4" aria-hidden="true" />
    <h2 className="text-xl font-bold text-white mb-2">{titulo}</h2>
    <p className="text-[#94a3b8] mb-6">{descripcion}</p>
    <Button variant="outline" className="border-[#334155] text-white hover:bg-[#1e2a42]">
        {cta}
    </Button>
</div>
```

### Empty States
```tsx
// Template empty state inline (dentro de MilestoneItem o seccion milestones)
<div className="flex flex-col items-center justify-center py-8 text-center">
    <{Icon} className="w-8 h-8 text-[#64748b] mb-3" aria-hidden="true" />
    <p className="text-sm font-medium text-[#94a3b8] mb-1">{titulo}</p>
    <p className="text-xs text-[#64748b] mb-3">{descripcion}</p>
    {cta && <Button size="sm" className="...">{ctaText}</Button>}
</div>
```

### Toast Notifications (via sonner)
| Accion | Tipo Sonner | Duracion |
|--------|------------|---------|
| Aceptar propuesta - success | `toast.success()` | 5000ms |
| Aceptar propuesta - error ya existe acuerdo | `toast.error()` | 5000ms |
| Rechazar propuesta | `toast.success()` | 3000ms |
| Crear milestone | `toast.success()` | 3000ms |
| Editar milestone | `toast.success()` | 3000ms |
| Eliminar milestone | `toast.success()` | 3000ms |
| Eliminar milestone - error | `toast.error()` | 5000ms |
| Subir entregable | `toast.success()` | 5000ms |
| Aprobar entregable | `toast.success()` | 3000ms |
| Rechazar entregable | `toast.warning()` | 4000ms |
| Marcar milestone completado | `toast.success()` | 3000ms |
| Completar acuerdo | `toast.success()` | 5000ms |
| Cancelar acuerdo | `toast.warning()` | 4000ms |
| Error generico | `toast.error()` | 5000ms |

---

## 14. Formularios - Resumen de Validaciones Visuales

### Estados de Input

| Estado | Clases Tailwind |
|--------|----------------|
| Default | `bg-[#1a1a2e] border-[#334155] text-white` |
| Focus | `focus:border-[#a855f7] focus:ring-1 focus:ring-[#a855f7]/50` (via Radix/shadcn focus) |
| Error | `border-red-500 focus:border-red-500 focus:ring-red-500/50` |
| Disabled | `disabled:opacity-50 disabled:cursor-not-allowed` |
| Valid (URL) | `border-green-600` (solo input URL cuando valida) |

### Patron de Campo con Label y Error
```
Label (text-[#cbd5e1])
  [span * aria-hidden rojo si obligatorio]
Input / Textarea
  [span contador si hay limite de chars]
p error-message (text-red-400, role="alert")
```

### Contador de Caracteres - Variantes de Color
| Estado | Color del contador |
|--------|-------------------|
| Normal | `text-[#64748b]` |
| Cerca del limite (>80%) | `text-amber-400` |
| En el limite (100%) | `text-red-400` |
| Por debajo del minimo (campo con min) | `text-amber-400` |

---

## 15. Responsive Design

### Breakpoints

| Breakpoint | Clases Tailwind | Cambios |
|------------|-----------------|---------|
| Mobile < 640px | `default` | Stack vertical, sidebar inline, dialogs full-width, acciones en columna |
| Tablet 640-1024px | `sm:` | 2 columnas, dialogs centrados max-w-lg |
| Desktop > 1024px | `md:` / `lg:` | Sidebar sticky visible, hover effects, tooltips en iconos |

### Cambios Responsive por Seccion

**Layout de la pagina de detalle:**
```tsx
// Desktop: main + sidebar horizontal
<div className="flex flex-col lg:flex-row gap-8 items-start">
    <main className="flex-1 min-w-0">...</main>
    <aside className="w-full lg:w-72 flex-shrink-0 lg:sticky lg:top-24 space-y-4">...</aside>
</div>
```

**En mobile:** sidebar se mueve arriba del contenido de milestones.
- Chat link: banner full-width inmediatamente debajo del header
- Detalles: dentro de `Collapsible` con trigger "Ver detalles"

**Dialogs en mobile:**
```tsx
// La fila de fechas y de importe+fecha cambia a columna en mobile
<div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
    ...
</div>

// Dialog content: full width en mobile
<DialogContent className="w-full max-w-full sm:max-w-lg mx-2 sm:mx-auto bg-[#0f1729] border-[#334155] text-white">
```

**Botones de accion en mobile:** `flex-col w-full` en mobile, `flex-row` en desktop:
```tsx
<div className="flex flex-col sm:flex-row gap-3 mt-8 pt-6 border-t border-[#334155]">
    <Button className="w-full sm:w-auto ...">Completar acuerdo</Button>
    <Button className="w-full sm:w-auto ...">Cancelar acuerdo</Button>
</div>
```

**Botones Aprobar/Rechazar en entregables:** En pantallas muy estrechas pueden quedar en columna:
```tsx
<div className="flex flex-col xs:flex-row gap-2 mt-2">
```

---

## 16. Accesibilidad

| Requisito WCAG | Implementacion |
|----------------|----------------|
| Labels vinculados | `<Label htmlFor="inputId">` vinculado a `<Input id="inputId">` |
| Required fields | `aria-required="true"` + indicador visual `*` con `aria-hidden="true"` |
| Errores de formulario | `role="alert"` + `aria-live="polite"` en mensajes de error |
| Inputs con error | `aria-invalid="true"` cuando hay error, `aria-describedby="fieldId-error"` |
| Focus trap en dialogs | Radix UI maneja automaticamente |
| Focus visible | shadcn/ui incluye `focus-visible:ring-2` por defecto |
| Escape key | Radix Dialog cierra con Escape automaticamente |
| Contraste de colores | Todos los textos sobre fondos oscuros cumplen WCAG AA (ratio > 4.5:1) |
| Botones con iconos | `aria-hidden="true"` en el icono, texto descriptivo visible o `aria-label` en el boton |
| Links externos | `rel="noopener noreferrer"` + indicacion visual icono `ExternalLink` |
| Estado loading en botones | `aria-busy="true"` en el boton durante submit |
| Progreso semantico | `Progress` de shadcn genera `role="progressbar"` con `aria-valuenow`, `aria-valuemin`, `aria-valuemax` |
| Dialogs con descripcion | `DialogDescription` siempre presente o `aria-describedby` en el dialog |
| Avatars | `AvatarFallback` con iniciales del nombre para texto alternativo |
| Badges de estado | `aria-label="Estado: {valor}"` para screen readers |

---

## 17. Animaciones e Interacciones

| Elemento | Animacion | Clase / Implementacion |
|----------|-----------|------------------------|
| Dialog open/close | Scale + fade (Radix built-in) | Manejo automatico via Radix |
| Progress bar update | Width transition | `transition-all duration-300 ease-out` via shadcn Progress |
| Milestone item hover | Background change | `hover:bg-[#1e2a42] transition-colors duration-150` |
| Button primary hover | Gradient shift | `hover:from-pink-600 hover:to-purple-700 transition-all duration-150` |
| Button loading spinner | Rotate infinite | `animate-spin` via Tailwind |
| Skeleton shimmer | Shimmer animation | shadcn `Skeleton` incluye `animate-pulse` |
| Input focus | Border purple + glow | `focus:border-[#a855f7] focus:ring-1 focus:ring-[#a855f7]/50 transition-colors duration-200` |
| Submit disabled | Opacity | `disabled:opacity-50 transition-opacity duration-150` |
| Error shake | Shake keyframes | Clase custom `error-shake` con keyframes en CSS global |
| Badge estado change | Fade-in | `animate-in fade-in duration-250` al montar |
| Entregable nuevo | Fade + slide | `animate-in fade-in slide-in-from-top-1 duration-200` al montar |
| Alert suggestion | Fade + expand | `animate-in fade-in duration-300` al montar |
| Toast | Slide + fade (Sonner) | Manejo automatico via sonner |

**Shake animation CSS global (anadir a `src/web/src/index.css`):**
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

## 18. Inventario de Componentes shadcn/ui Utilizados

| Componente shadcn | Pantallas / Contextos |
|-------------------|-----------------------|
| `Dialog` + `DialogContent` + `DialogHeader` + `DialogTitle` + `DialogDescription` + `DialogFooter` | Pantallas 1, 2, 4, 5, 6, 7, 8, 9 (todos los dialogs) |
| `Card` + `CardHeader` + `CardContent` | Pantalla 3 (header, milestones section, sidebar cards) |
| `Badge` | EstadoAcuerdoBadge, EstadoEntregableBadge, badges de milestones |
| `Button` | Todos los dialogs y pantalla 3 (multiples variantes: default, outline, destructive, ghost, sm) |
| `Input` | Pantallas 1, 4, 5 (type text, date, number, url) |
| `Textarea` | Pantallas 2, 4, 5, 6, 7, 9 |
| `Label` | Pantallas 1, 2, 4, 5, 6, 7, 9 |
| `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` | Pantalla 5 (Milestone asociado) |
| `Progress` | Pantalla 3 (barra de progreso de milestones), Pantalla 4 (ImporteAsignadoBar) |
| `Avatar` + `AvatarFallback` | Pantalla 3 (partes del acuerdo) |
| `Alert` + `AlertDescription` | Pantallas 1, 2, 4, 6, 7, 8, 9 (multiples variantes de color) |
| `Skeleton` | AcuerdoDetailSkeleton |
| `Tooltip` + `TooltipTrigger` + `TooltipContent` | MilestoneItem (iconos editar/eliminar en desktop) |
| `Collapsible` + `CollapsibleTrigger` + `CollapsibleContent` | Mobile: seccion "Ver detalles" del sidebar |
| `Separator` | Divider entre secciones del dialog footer |

**Total componentes shadcn/ui distintos:** 14 familias de componentes

---

## 19. Estructura de Archivos de la Feature

```
src/web/src/features/crowdsourcing/
├── presentation/
│   ├── components/
│   │   ├── AceptarPropuestaDialog.tsx      [NUEVO] Pantalla 1
│   │   ├── RechazarPropuestaDialog.tsx     [NUEVO] Pantalla 2 (artista rechaza propuesta)
│   │   ├── MilestoneFormDialog.tsx         [NUEVO] Pantalla 4 (crear/editar)
│   │   ├── EntregableFormDialog.tsx        [NUEVO] Pantalla 5
│   │   ├── AprobarEntregableDialog.tsx     [NUEVO] Pantalla 6
│   │   ├── RechazarEntregableDialog.tsx    [NUEVO] Pantalla 7
│   │   ├── CompletarAcuerdoDialog.tsx      [NUEVO] Pantalla 8
│   │   ├── CancelarAcuerdoDialog.tsx       [NUEVO] Pantalla 9
│   │   ├── MilestoneItem.tsx               [NUEVO] Componente reutilizable
│   │   ├── EntregableRow.tsx               [NUEVO] Componente reutilizable
│   │   ├── EstadoAcuerdoBadge.tsx          [NUEVO] Componente reutilizable
│   │   ├── EstadoEntregableBadge.tsx       [NUEVO] Componente reutilizable
│   │   ├── ImporteAsignadoBar.tsx          [NUEVO] Componente reutilizable
│   │   ├── AcuerdoDetailSkeleton.tsx       [NUEVO] Skeleton de carga
│   │   │
│   │   │   [EXISTENTES - sin cambios]
│   │   ├── EstadoPropuestaBadge.tsx
│   │   ├── PropuestaCard.tsx
│   │   └── RetirarPropuestaDialog.tsx
│   │
│   └── pages/
│       └── AcuerdoDetallePage.tsx          [NUEVO] Pantalla 3 (ruta /crowdsourcing/acuerdos/:id)
```

---

## 20. Checklist

- [ ] Todos los inputs tienen `Label` con `htmlFor` vinculado
- [ ] Campos obligatorios tienen `aria-required="true"` y asterisco `aria-hidden="true"`
- [ ] Mensajes de error con `role="alert"` y `aria-live="polite"`
- [ ] Inputs en error tienen `aria-invalid="true"` y `aria-describedby`
- [ ] Botones de submit tienen estado loading con `aria-busy="true"`
- [ ] Iconos decorativos con `aria-hidden="true"`
- [ ] Botones solo-icono con `aria-label` descriptivo
- [ ] Links externos con `rel="noopener noreferrer"` e icono `ExternalLink`
- [ ] Badges de estado con `aria-label="Estado: {valor}"`
- [ ] Progress bar con `aria-valuenow`, `aria-valuemin`, `aria-valuemax` (shadcn lo incluye)
- [ ] Contraste WCAG AA en todos los textos sobre fondos oscuros
- [ ] Focus visible en todos los elementos interactivos
- [ ] Dialogs cierran con Escape (Radix automatico)
- [ ] Focus trap activo en dialogs (Radix automatico)
- [ ] Responsive funciona en 320px (mobile estrecho), 768px (tablet), 1280px (desktop)
- [ ] Dialogs en mobile usan full-width con padding lateral
- [ ] Grid de fechas e importes cambia a 1 columna en mobile
- [ ] Sidebar se mueve al inicio del contenido en mobile
- [ ] `AcuerdoDetailSkeleton` cubre todos los elementos del layout
- [ ] Todos los estados de error de pagina tienen CTA de recuperacion
- [ ] `EstadoAcuerdoBadge` cubre los 3 estados: Activo, Completado, Cancelado
- [ ] `EstadoEntregableBadge` cubre los 3 estados: Entregado, Aprobado, Rechazado
- [ ] `MilestoneItem` oculta correctamente acciones segun rol y estado del acuerdo
- [ ] `EntregableRow` muestra borde lateral segun estado del entregable
- [ ] `ImporteAsignadoBar` actualiza en tiempo real al cambiar importe en MilestoneFormDialog
- [ ] Shake animation registrada en CSS global
- [ ] Contadores de caracteres cambian a color amber/rojo segun umbrales
- [ ] Toast notifications configuradas con duraciones correctas via sonner
- [ ] `Collapsible` implementado para detalles del acuerdo en mobile
