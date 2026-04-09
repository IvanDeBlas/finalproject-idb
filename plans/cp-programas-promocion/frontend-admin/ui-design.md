# Diseno UI: Programas de Promocion (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-programas-promocion (US-CP-02)
**Target:** src/admin

---

## 1. Resumen

- Componentes shadcn/ui ya instalados usados: 18
- Componentes shadcn/ui por instalar: 1 (Switch)
- Componentes custom a crear: 12
- Pantallas diseñadas: 5 (Wizard Crear, Listado, Detalle, Editar, Dialog Desactivacion)
- Responsive breakpoints: mobile (<768px), tablet (768px-1024px), desktop (>1024px)

### Inventario de Componentes shadcn/ui Instalados

| Componente | Archivo | Estado |
|------------|---------|--------|
| Button | `components/ui/button.tsx` | Instalado |
| Card, CardHeader, CardContent, CardFooter | `components/ui/card.tsx` | Instalado |
| Input | `components/ui/input.tsx` | Instalado |
| Label | `components/ui/label.tsx` | Instalado |
| Textarea | `components/ui/textarea.tsx` | Instalado |
| Badge | `components/ui/badge.tsx` | Instalado |
| Skeleton | `components/ui/skeleton.tsx` | Instalado |
| Select, SelectTrigger, SelectContent, SelectItem | `components/ui/select.tsx` | Instalado |
| Tabs, TabsList, TabsTrigger, TabsContent | `components/ui/tabs.tsx` | Instalado |
| AlertDialog (y variantes) | `components/ui/alert-dialog.tsx` | Instalado |
| Dialog (y variantes) | `components/ui/dialog.tsx` | Instalado |
| DropdownMenu (y variantes) | `components/ui/dropdown-menu.tsx` | Instalado |
| Table, TableHeader, TableBody, TableRow, TableCell, TableHead | `components/ui/table.tsx` | Instalado |
| Avatar, AvatarFallback | `components/ui/avatar.tsx` | Instalado |
| Separator | `components/ui/separator.tsx` | Instalado |
| Alert, AlertTitle, AlertDescription | `components/ui/alert.tsx` | Instalado |
| Tooltip, TooltipProvider, TooltipContent, TooltipTrigger | `components/ui/tooltip.tsx` | Instalado |
| Sonner (Toast) | `components/ui/sonner.tsx` | Instalado |
| **Switch** | `components/ui/switch.tsx` | **POR INSTALAR** |

### Componentes Custom a Crear

| Componente | Ubicacion | Descripcion |
|------------|-----------|-------------|
| `WizardStepper` | `components/crowdpromotion/WizardStepper.tsx` | Stepper de 4 pasos con estados activo/completado/futuro |
| `WizardLayout` | `components/crowdpromotion/WizardLayout.tsx` | Layout full-page del wizard (header, body scrollable, footer fijo) |
| `WizardFooter` | `components/crowdpromotion/WizardFooter.tsx` | Barra inferior fija con Anterior/Siguiente/Publicar |
| `PromoTareaCard` | `components/crowdpromotion/PromoTareaCard.tsx` | Card de tarea en el paso 3 del wizard (vista colapsada) |
| `PromoTareaForm` | `components/crowdpromotion/PromoTareaForm.tsx` | Formulario inline expandible para crear/editar tarea |
| `ComisionPreviewCard` | `components/crowdpromotion/ComisionPreviewCard.tsx` | Card de vista previa de comision en tiempo real (paso 2) |
| `PromoProgramaCard` | `components/crowdpromotion/PromoProgramaCard.tsx` | Card de programa en el listado con menu kebab |
| `PromoProgramaKpiGrid` | `components/crowdpromotion/PromoProgramaKpiGrid.tsx` | Grid de 4 KPI cards para el detalle del programa |
| `PromoProgramaKpiCard` | `components/crowdpromotion/PromoProgramaKpiCard.tsx` | KPI individual: icono coloreado + valor grande + label |
| `DesactivarProgramaDialog` | `components/crowdpromotion/DesactivarProgramaDialog.tsx` | AlertDialog de confirmacion de desactivacion |
| `AbandonarWizardDialog` | `components/crowdpromotion/AbandonarWizardDialog.tsx` | AlertDialog de confirmacion al abandonar el wizard |
| `PromotorInscritoRow` | `components/crowdpromotion/PromotorInscritoRow.tsx` | Fila de tabla de promotor inscrito (avatar + nombre + badges) |

### Componentes Existentes Reutilizados (Patron de Referencia)

| Componente existente | Patron heredado |
|---------------------|-----------------|
| `components/dashboard/stats-card.tsx` | Base para `PromoProgramaKpiCard` (customizado con icon bg coloreado) |
| `promotor/components/DesactivarPromotorDialog.tsx` | Patron para `DesactivarProgramaDialog` |
| `promotor/components/PromotorPageClient.tsx` | Patron de orquestacion para `ProgramaDetalleClient.tsx` |

---

## 2. Paleta de Colores

| Uso | Clase Tailwind | Valor hex | Ejemplo |
|-----|----------------|-----------|---------|
| Fondo base / sidebar / wizard header | `bg-[#0d0d1a]` | #0d0d1a | Header wizard, sidebar |
| Fondo secundario / paginas | `bg-[#1a1a2e]` | #1a1a2e | Body del wizard |
| Fondo card / contenedor de paso | `bg-[#151525]` | #151525 | Cards, contenedor de paso activo |
| Fondo hover card / pasos futuros | `bg-[#1e1e38]` | #1e1e38 | Hover cards, steppers futuros |
| Fondo input | `bg-[#0f0f1f]` | #0f0f1f | Todos los inputs, selects |
| Gradiente primario | `from-pink-500 to-purple-600` | #ec4899->#a855f7 | Botones principales, paso activo stepper |
| Gradiente primario hover | `from-pink-600 to-purple-700` | hover del gradiente | Hover botones primarios |
| Color accent / links | `text-[#a855f7]` | #a855f7 | Foco de bordes, texto accent |
| Texto primario | `text-white` | #ffffff | Titulos, valores importantes |
| Texto secundario | `text-[#94a3b8]` | #94a3b8 | Labels, descripciones |
| Texto muted | `text-[#64748b]` | #64748b | Hints, placeholders, valores nulos |
| Texto label | `text-[#cbd5e1]` | #cbd5e1 | Labels de formulario |
| Borde default | `border-[#334155]` | #334155 | Cards, inputs, separadores |
| Borde focus | `border-[#a855f7]` | #a855f7 | Input/select con foco |
| Borde error | `border-red-500` | #ef4444 | Input con error |
| Estado activo (verde) | `text-green-400` / `bg-green-950/50` / `border-green-800/50` | #10b981 | Badge ACTIVO, toggle ON |
| Estado inactivo (gris) | `text-slate-400` / `bg-slate-800` | #64748b | Badge INACTIVO |
| Aviso (amarillo) | `text-amber-300` / `bg-amber-950/40` / `border-amber-800/50` | #f59e0b | Banners de aviso |
| Info (azul) | `text-blue-300` / `bg-blue-950/40` / `border-blue-800/50` | #3b82f6 | Hints informativos |
| Error (rojo) | `text-red-400` | #ef4444 | Mensajes de error, acciones destructivas |
| Stepper completado bg | `bg-[#1e3a2f] border-2 border-[#10b981]` | #1e3a2f | Paso completado |
| Stepper conector completado | `bg-[#10b981]` | #10b981 | Linea entre pasos completados |
| Stepper conector pendiente | `bg-[#334155]` | #334155 | Linea entre pasos pendientes |

---

## 3. Componentes por Pantalla

---

### 3.1 Pantalla: Wizard - Crear / Editar Programa

**Ruta crear:** `/dashboard/crowdpromotion/programas/nuevo`
**Ruta editar:** `/dashboard/crowdpromotion/programas/{id}/editar`

#### Layout General

```
┌──────────────────────────────────────────────────────────────────────┐
│ WIZARD HEADER (h-14, bg-[#0d0d1a], border-b border-[#334155])        │
│ [Logo WePlay]  Crear programa de promocion              [X Cancelar] │
├──────────────────────────────────────────────────────────────────────┤
│                         MAIN SCROLLABLE AREA                         │
│  [Banner aviso promotores - solo modo edicion con promotores]        │
│                                                                      │
│    WIZARD STEPPER (mt-8 mb-10, max-w-2xl mx-auto)                   │
│    [1]───────────[2]───────────[3]───────────[4]                     │
│    Datos         Comisiones    Tareas         Revisar                 │
│                                                                      │
│    ┌──────────────────────────────────────────────┐                  │
│    │  CONTENIDO DEL PASO ACTIVO                   │                  │
│    │  (max-w-2xl mx-auto, bg-[#151525],           │                  │
│    │   border border-[#334155], rounded-xl, p-6)  │                  │
│    └──────────────────────────────────────────────┘                  │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│ WIZARD FOOTER (h-16, bg-[#0d0d1a], border-t border-[#334155])        │
│ [<- Anterior]           Paso X de 4           [Siguiente / Publicar] │
└──────────────────────────────────────────────────────────────────────┘
```

#### Componente: WizardLayout

| Elemento | Componente shadcn | Clase Tailwind / Notas |
|----------|-------------------|------------------------|
| Contenedor raiz | `<div>` | `flex flex-col h-screen bg-[#1a1a2e] overflow-hidden` |
| Header del wizard | `<header>` | `h-14 flex items-center justify-between px-6 bg-[#0d0d1a] border-b border-[#334155] flex-shrink-0` |
| Logo en header | `<Link>` + `<Music>` | `flex items-center gap-2 text-white font-bold` |
| Titulo en header | `<span>` | `text-sm font-medium text-[#94a3b8]` (titulo del wizard) |
| Boton cerrar | `<Button variant="ghost" size="sm">` | `text-[#94a3b8] hover:text-white gap-2` con icono `<X w-4 h-4>` + texto "Cancelar" |
| Area scrollable | `<main>` | `flex-1 overflow-y-auto px-6 py-8` |
| Footer fijo | `<footer>` | `h-16 flex items-center justify-between px-6 bg-[#0d0d1a] border-t border-[#334155] flex-shrink-0` |

**Props de WizardLayout:**
```typescript
interface WizardLayoutProps {
    titulo: string
    children: React.ReactNode
    onCancelar: () => void
}
```

#### Componente: WizardStepper

**Este componente es completamente custom (no existe en shadcn/ui).**

| Elemento | Implementacion | Clase Tailwind |
|----------|---------------|----------------|
| Contenedor nav | `<nav aria-label="Progreso del wizard">` | `max-w-2xl mx-auto mb-8` |
| Lista de pasos | `<ol>` | `flex items-start justify-between` |
| Item de paso | `<li>` | `flex flex-col items-center flex-1 relative` |
| Circulo activo | `<div>` | `w-10 h-10 rounded-full bg-gradient-to-r from-pink-500 to-purple-600 text-white font-bold flex items-center justify-center text-sm transition-all duration-300` |
| Circulo completado | `<div>` | `w-10 h-10 rounded-full bg-[#1e3a2f] border-2 border-[#10b981] text-[#10b981] flex items-center justify-center transition-all duration-300` + icono `<Check w-4 h-4>` |
| Circulo futuro | `<div>` | `w-10 h-10 rounded-full bg-[#1e1e38] text-[#64748b] flex items-center justify-center text-sm transition-all duration-300` |
| Etiqueta activo | `<span>` | `text-xs font-medium text-[#a855f7] mt-2 text-center block` |
| Etiqueta completado | `<span>` | `text-xs text-[#10b981] mt-2 text-center block` |
| Etiqueta futuro | `<span>` | `text-xs text-[#64748b] mt-2 text-center block` |
| Linea conectora pendiente | `<div aria-hidden="true">` | `absolute top-5 left-1/2 w-full h-px bg-[#334155] -translate-y-1/2` |
| Linea conectora completada | `<div aria-hidden="true">` | `absolute top-5 left-1/2 w-full h-px bg-[#10b981] -translate-y-1/2 transition-colors duration-300` |

**Props de WizardStepper:**
```typescript
interface WizardStepperProps {
    pasoActual: 1 | 2 | 3 | 4
    pasos: Array<{ numero: number; etiqueta: string }>
}
```

**ARIA en WizardStepper:**
- `<nav aria-label="Progreso del wizard">`
- `<ol role="list">`
- Paso activo: `aria-current="step"`
- Paso completado: `aria-label="{etiqueta}: completado"`
- Paso futuro: `aria-label="{etiqueta}: pendiente"`

#### Componente: WizardFooter

| Elemento | Componente shadcn | Clase Tailwind / Notas |
|----------|-------------------|------------------------|
| Contenedor | `<div>` | `flex items-center justify-between w-full` |
| Boton Anterior | `<Button variant="outline">` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white gap-2` + `<ChevronLeft w-4 h-4>` / `disabled` en paso 1 |
| Texto indicador | `<span>` | `text-sm text-[#64748b] absolute left-1/2 -translate-x-1/2` |
| Boton Siguiente (pasos 1-3) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold gap-2` + `<ChevronRight w-4 h-4>` |
| Boton Publicar (paso 4) | `<Button>` | Mismo gradiente + icono `<Rocket w-4 h-4>` + texto "Publicar programa" |
| Boton Guardar cambios (editar, paso 4) | `<Button>` | Mismo gradiente + icono `<Save w-4 h-4>` + texto "Guardar cambios" |
| Spinner loading | `<Loader2 className="w-4 h-4 animate-spin">` | Reemplaza icono cuando `isSubmitting` |

**Props de WizardFooter:**
```typescript
interface WizardFooterProps {
    pasoActual: number
    totalPasos: number
    onAnterior: () => void
    onSiguiente: () => void
    isSubmitting?: boolean
    modoEdicion?: boolean
}
```

**Estados del footer:**
| Estado | Comportamiento |
|--------|----------------|
| Paso 1 | Boton Anterior `disabled` con `opacity-50 cursor-not-allowed` |
| Pasos 2-3 | Ambos botones habilitados |
| Paso 4 | "Siguiente" reemplazado por "Publicar programa" o "Guardar cambios" |
| Submitting | Boton accion muestra spinner + texto "Publicando..." o "Guardando...", todos los botones `disabled` |

---

#### 3.1.1 Paso 1: Datos Basicos

**Composicion:**
```
<Card className="max-w-2xl mx-auto bg-[#151525] border-[#334155] rounded-xl">
    <CardHeader>
        <h2>Datos basicos del programa</h2>
        <p>Configura la informacion principal de tu programa</p>
        <Separator />
    </CardHeader>
    <CardContent>
        <Form>
            <!-- Titulo -->
            <FormField name="titulo">
                <FormItem>
                    <FormLabel required>Titulo</FormLabel>
                    <Input placeholder="Ej: Promociona mi nuevo album..." />
                    <FormMessage />
                </FormItem>
            </FormField>

            <!-- Descripcion -->
            <FormField name="descripcion">
                <FormItem>
                    <FormLabel>Descripcion</FormLabel>
                    <Textarea placeholder="Describe que deben hacer..." />
                    <span contador caracteres />
                    <FormMessage />
                </FormItem>
            </FormField>

            <!-- Tipo de programa -->
            <FormField name="tipoPromoId">
                <FormItem>
                    <FormLabel required>Tipo de programa</FormLabel>
                    <Select>
                        <SelectTrigger />
                        <SelectContent>
                            <SelectItem value="1">Referral</SelectItem>
                            ...
                        </SelectContent>
                    </Select>
                    <span hint contextual segun tipo />
                    <FormMessage />
                </FormItem>
            </FormField>

            <!-- Grid: Campana + Proyecto -->
            <div className="grid grid-cols-2 gap-4">
                <Select campana />
                <Select proyecto />
            </div>
            <InfoBanner texto="Vincula al menos uno (opcional)" />

            <!-- Grid: URL Landing + Codigo Tracking -->
            <div className="grid grid-cols-2 gap-4">
                <Input url landing />
                <Input codigo tracking />
            </div>

            <!-- Grid: Fechas -->
            <div className="grid grid-cols-2 gap-4">
                <Input type="date" fechaInicio />
                <Input type="date" fechaFin />
            </div>
        </Form>
    </CardContent>
</Card>
```

**Tabla de elementos Paso 1:**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|------------------------|
| Contenedor paso | `Card` | `max-w-2xl mx-auto bg-[#151525] border-[#334155] rounded-xl` |
| Titulo seccion | `<h2>` nativo | `text-xl font-semibold text-white mb-1` |
| Subtitulo seccion | `<p>` nativo | `text-sm text-[#94a3b8] mb-6` |
| Separador | `Separator` | `bg-[#334155] mb-6` |
| Label requerido | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` + asterisco `after:content-['*'] after:text-red-500 after:ml-1` |
| Label opcional | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Input texto | `Input` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11` |
| Textarea descripcion | `Textarea` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] min-h-[100px] resize-none` |
| Contador caracteres | `<span>` nativo | `text-xs text-[#64748b] text-right block mt-1` |
| Select disparador | `SelectTrigger` | `bg-[#0f0f1f] border-[#334155] text-white h-11 focus:border-[#a855f7]` |
| Select contenido | `SelectContent` | `bg-[#151525] border-[#334155]` |
| Select item | `SelectItem` | `text-white hover:bg-[#1e1e38] focus:bg-[#1e1e38] cursor-pointer` |
| Hint contextual tipo | `<p>` nativo | `text-xs text-[#64748b] mt-1 italic` |
| Grid 2 columnas | `<div>` nativo | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Banner info azul | `<div>` custom | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-1 mb-4` + `<Info w-4 h-4 flex-shrink-0 mt-0.5>` |
| Input URL | `Input` type="url" | Igual que Input texto |
| Input fecha | `Input` type="date" | `bg-[#0f0f1f] border-[#334155] text-white h-11 [color-scheme:dark]` |
| Mensaje error | `FormMessage` | `text-xs text-red-400 mt-1 flex items-center gap-1` + `<AlertCircle w-3 h-3>` |

**Estados Paso 1:**
| Estado | Visual |
|--------|--------|
| Default | Todos los campos vacios con placeholders en `#64748b` |
| Cargando selects (campana/proyecto) | `<Skeleton className="h-11 w-full bg-[#1e1e38] animate-pulse rounded-md">` |
| Error carga selects | Select deshabilitado con texto "No se pudo cargar" en rojo |
| Campo con error | Input con `border-red-500` + `aria-invalid="true"` + `FormMessage` visible |
| Campo valido | Input con `border-[#334155]` o `border-[#a855f7]` si en foco |
| Intentar avanzar con errores | `FormMessage` aparece debajo de cada campo invalido, wizard no avanza |

---

#### 3.1.2 Paso 2: Configurar Comisiones

**Composicion:**
```
<Card className="max-w-2xl mx-auto bg-[#151525] ...">
    <CardContent>
        <!-- Select Moneda (ancho completo) -->
        <FormField name="monedaId">
            <Select moneda />
        </FormField>

        <!-- Grid: Comision % + Comision fija -->
        <div className="grid grid-cols-2 gap-4">
            <div>
                <Label>Comision por porcentaje</Label>
                <div className="flex items-center gap-2">
                    <Input type="number" name="importeComisionPorcentaje" />
                    <span className="text-[#94a3b8]">%</span>
                </div>
                <p hint>De cada backing referido</p>
            </div>
            <div>
                <Label>Comision fija por conversion</Label>
                <Input type="number" name="importeComisionFija" />
                <p hint>EUR por cada conversion</p>
            </div>
        </div>

        <!-- Aviso al menos una comision -->
        <AvisoBanner color="amber" texto="Al menos una de las dos comisiones es obligatoria" />

        <!-- ComisionPreviewCard -->
        <ComisionPreviewCard ... />
    </CardContent>
</Card>
```

**Componente: ComisionPreviewCard**

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Contenedor preview | `Card` | `bg-[#0f0f1f] border-[#334155] p-4 mt-4` |
| Titulo preview | `<p>` | `text-sm font-medium text-[#94a3b8] mb-3` |
| Fila de calculo | `<div>` | `flex justify-between text-sm mb-1` |
| Label fila | `<span>` | `text-[#94a3b8]` |
| Valor fila | `<span>` | `text-white font-medium` |
| Fila total | `<div>` | `flex justify-between text-sm font-semibold border-t border-[#334155] mt-2 pt-2` |
| Label total | `<span>` | `text-[#cbd5e1]` |
| Valor total | `<span>` | `text-[#10b981]` |
| Texto sin comision | `<p>` | `text-sm text-[#64748b] italic text-center py-2` |

**Props de ComisionPreviewCard:**
```typescript
interface ComisionPreviewCardProps {
    importePorcentaje: number | undefined
    importeFija: number | undefined
    monedaNombre: string
    backingEjemplo?: number // default: 50
}
```

La card recalcula en tiempo real con `useWatch` de React Hook Form. Si ninguna comision definida, muestra texto "Ingresa al menos una comision para ver la vista previa".

**Estados Paso 2:**
| Estado | Visual |
|--------|--------|
| Sin comisiones | `ComisionPreviewCard` muestra texto placeholder italic muted. Banner amarillo visible |
| Con porcentaje | Preview muestra calculo "Comision porcentaje: X EUR (Y%)" |
| Con fija | Preview muestra "Comision fija: X EUR" |
| Con ambas | Preview muestra ambas filas + fila "Total promotor" en verde |
| Porcentaje fuera de rango | `FormMessage` "El porcentaje debe estar entre 0 y 100" |

---

#### 3.1.3 Paso 3: Definir Tareas

**Layout del paso:**
```
<Card className="max-w-2xl mx-auto bg-[#151525] ...">
    <CardContent>
        <!-- Lista de tareas existentes -->
        <div className="space-y-3">
            <PromoTareaCard tarea={...} onEditar={...} onEliminar={...} />
            <PromoTareaCard tarea={...} ... />
        </div>

        <!-- Formulario inline (expandido/colapsado) -->
        {formularioAbierto && (
            <PromoTareaForm
                tarea={tareaEnEdicion}
                onGuardar={handleGuardar}
                onCancelar={() => setFormularioAbierto(false)}
            />
        )}

        <!-- Boton agregar nueva tarea -->
        <Button variant="outline" className="w-full border-dashed ...">
            <Plus /> Agregar nueva tarea
        </Button>

        <!-- Banner info opcional -->
        <InfoBanner texto="Puedes crear el programa sin tareas y agregarlas despues" />
    </CardContent>
</Card>
```

**Componente: PromoTareaCard (vista colapsada)**

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Card contenedor | `Card` | `bg-[#0f0f1f] border-[#334155] p-4 hover:border-[#a855f7] transition-colors duration-200` |
| Fila superior | `<div>` | `flex items-center justify-between` |
| Badge tipo evento | `Badge` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium` |
| Nombre tarea | `<p>` | `text-sm font-semibold text-white ml-2 flex-1` |
| Toggle activa | `Switch` | `data-[state=checked]:bg-[#10b981]` con `aria-label="Tarea activa"` |
| Label toggle | `<span>` | `text-xs text-[#94a3b8] ml-2` |
| Detalles resumen | `<p>` | `text-xs text-[#64748b] mt-1.5` (ej: "Dinero: 5.00 EUR | Repetible x10 | Mar-Jun 2026") |
| Contenedor acciones | `<div>` | `flex justify-end gap-2 mt-3` |
| Boton Editar | `Button variant="outline" size="sm"` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 px-3 text-xs` + `<Pencil w-3.5 h-3.5>` |
| Boton Eliminar | `Button variant="ghost" size="sm"` | `text-red-400 hover:text-red-300 hover:bg-red-950/30 h-8 w-8 p-0` + `<Trash2 w-3.5 h-3.5>` |
| Boton Eliminar deshabilitado (edicion) | `Button` `disabled` | `text-[#64748b] cursor-not-allowed opacity-50` envuelto en `TooltipProvider` |
| Tooltip deshabilitado | `TooltipContent` | `"Esta tarea tiene completados registrados y no puede eliminarse"` |

**Props de PromoTareaCard:**
```typescript
interface PromoTareaCardProps {
    tarea: PromoTareaFormData | PromoTareaDetail
    indice: number
    onEditar: () => void
    onEliminar: () => void
    eliminarDeshabilitado?: boolean
    razonDeshabilitado?: string
    modoEdicion?: boolean
}
```

**Componente: PromoTareaForm (formulario inline expandible)**

Este formulario usa animacion de expand/collapse. Se implementa con CSS transition o Framer Motion `AnimatePresence`.

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Contenedor form | `Card` | `bg-[#151525] border border-[#a855f7] rounded-xl p-5 mt-3 transition-all duration-250` |
| Header form | `<div>` | `flex justify-between items-center mb-4` |
| Titulo form | `<h3>` | `text-base font-semibold text-white` |
| Boton cerrar | `Button variant="ghost" size="sm"` | `h-8 w-8 p-0 text-[#94a3b8] hover:text-white` + `<X w-4 h-4>` |
| Input nombre | `Input` | Igual que paso 1 |
| Textarea descripcion | `Textarea` | `min-h-[80px]` igual que paso 1 |
| Select tipo evento | `Select` | Igual que paso 1 |
| Select tipo recompensa | `Select` | Igual que paso 1 |
| Seccion recompensa | `<div>` | `bg-[#0f0f1f] rounded-lg p-4 mt-3 space-y-3` |
| Input importe | `Input` type="number" | `h-11` igual que paso 1 |
| Select moneda tarea | `Select` | Condicional: visible si tipoReward=Dinero/Mixto |
| Input puntos | `Input` type="number" | Condicional: visible si tipoReward=Puntos/Mixto |
| Input URL instrucciones | `Input` type="url" | Igual que paso 1 |
| Contenedor toggle repetible | `<div>` | `flex items-center gap-3 py-2` |
| Toggle es repetible | `Switch` | `data-[state=checked]:bg-[#a855f7]` con `aria-label="La tarea es repetible"` |
| Label toggle | `Label` | `text-sm text-[#cbd5e1] cursor-pointer` |
| Input max repeticiones | `Input` type="number" min="1" | Condicional: `esRepetible = true`. Igual que paso 1 |
| Grid fechas | `<div>` | `grid grid-cols-2 gap-4` |
| Boton Cancelar | `Button variant="ghost"` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Guardar tarea | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold` + `<Save w-4 h-4 mr-2>` |

**Visibilidad condicional de campos de recompensa:**
| Tipo de recompensa seleccionado | Campos visibles |
|---------------------------------|----------------|
| Dinero (id=1) | Importe recompensa (requerido) + Select Moneda (requerido) |
| Puntos (id=2) | Input Puntos (requerido) |
| Mixto (id=3) | Importe (requerido) + Moneda (requerido) + Puntos (requerido) |

**Props de PromoTareaForm:**
```typescript
interface PromoTareaFormProps {
    tareaInicial?: Partial<PromoTareaFormData>
    modoEdicion: boolean
    onGuardar: (tarea: PromoTareaFormData) => void
    onCancelar: () => void
}
```

**Estados del formulario de tarea:**
| Estado | Visual |
|--------|--------|
| Nueva tarea | Titulo "Nueva tarea", campos vacios |
| Editando tarea | Titulo "Editar tarea", campos pre-rellenados con datos de la tarea |
| Toggle esRepetible OFF | Campo maxRepeticiones oculto con `display: none` o `hidden` |
| Toggle esRepetible ON | Campo maxRepeticiones visible y requerido |
| Tipo recompensa cambia | Campos de importe/puntos aparecen/desaparecen con `hidden` condicional |
| Error de validacion | `FormMessage` debajo de cada campo invalido |

---

#### 3.1.4 Paso 4: Revisar y Publicar

**Composicion:**
```
<Card className="max-w-2xl mx-auto bg-[#151525] ...">
    <CardContent>
        <div className="space-y-4">

            <!-- Seccion Datos del programa -->
            <div>
                <div className="flex justify-between items-center mb-2">
                    <h3>Datos del programa</h3>
                    <Button ghost size="sm" onClick={() => irAPaso(1)}>
                        <Pencil /> Editar
                    </Button>
                </div>
                <Card bg-[#0f0f1f]>
                    <FilaDato label="Titulo" valor={...} />
                    <FilaDato label="Tipo" valor={...} />
                    <FilaDato label="Campana" valor={...} nullable />
                    <FilaDato label="URL landing" valor={...} nullable />
                    <FilaDato label="Tracking" valor={...} nullable />
                    <FilaDato label="Periodo" valor={...} nullable />
                </Card>
            </div>

            <!-- Seccion Comisiones -->
            <div>
                <div className="flex justify-between items-center mb-2">
                    <h3>Comisiones</h3>
                    <Button ghost onClick={() => irAPaso(2)}>Editar</Button>
                </div>
                <Card bg-[#0f0f1f]>
                    <FilaDato label="Moneda" valor={...} />
                    <FilaDato label="Por porcentaje" valor={...} />
                    <FilaDato label="Fija por conv." valor={...} />
                </Card>
            </div>

            <!-- Seccion Tareas -->
            <div>
                <div className="flex justify-between items-center mb-2">
                    <h3>Tareas ({n})</h3>
                    <Button ghost onClick={() => irAPaso(3)}>Editar</Button>
                </div>
                <Card bg-[#0f0f1f]>
                    {tareas.map((t, i) => <ItemResumenTarea key={i} tarea={t} />)}
                    {tareas.length === 0 && <p muted>Sin tareas definidas</p>}
                </Card>
            </div>

            <!-- Aviso publicacion -->
            <InfoBanner texto="Al confirmar, el programa se publicara con estado Activo." />

        </div>
    </CardContent>
</Card>
```

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Titulo de seccion | `<h3>` | `text-sm font-semibold text-[#cbd5e1] uppercase tracking-wider mb-2` |
| Card de seccion | `Card` | `bg-[#0f0f1f] border-[#334155] p-4` |
| Fila de dato | `<div>` | `flex justify-between items-start py-1.5 border-b border-[#1e1e38] last:border-0` |
| Label dato | `<span>` | `text-sm text-[#64748b]` |
| Valor dato | `<span>` | `text-sm text-white font-medium text-right max-w-[60%]` |
| Valor nulo | `<span>` | `text-sm text-[#64748b] italic` con texto "--" |
| Boton Editar seccion | `Button variant="ghost" size="sm"` | `text-[#a855f7] hover:text-purple-400 hover:bg-[#a855f7]/10 h-6 px-2 text-xs` + `<Pencil w-3 h-3 mr-1>` |
| Item tarea resumen | `<div>` | `py-2 border-b border-[#1e1e38] last:border-0` |
| Nombre tarea resumen | `<p>` | `text-sm text-white font-medium` con indice numerico |
| Detalles tarea resumen | `<p>` | `text-xs text-[#64748b] mt-0.5` (tipo evento + recompensa) |
| Banner aviso publicacion | `<div>` | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-4` |

---

### 3.2 Pantalla: Listado Mis Programas

**Ruta:** `/dashboard/crowdpromotion/programas`

#### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-[250px], bg-background, border-r)                         │
│ [Logo WePlay]                   MAIN CONTENT AREA                    │
│ Dashboard                       ┌──────────────────────────────────┐ │
│ Mis Campanias                   │ TOPBAR / HEADER                  │ │
│ CrowdPromotion >                │ breadcrumb: CP / Mis Programas   │ │
│  > Mis Programas   (activo)     ├──────────────────────────────────┤ │
│  > Estadisticas                 │                                  │ │
│ Configuracion                   │ Mis programas de promocion       │ │
│ [Logout]                        │                   [+ Nuevo prog] │ │
│                                 │                                  │ │
│                                 │ [Filtro estado v] [Buscar      ] │ │
│                                 │                                  │ │
│                                 │ <PromoProgramaCard />            │ │
│                                 │ <PromoProgramaCard />            │ │
│                                 │ <PromoProgramaCard />            │ │
│                                 │                                  │ │
│                                 │ [< 1 2 3 >] paginacion          │ │
│                                 └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

#### Componentes de la Cabecera de Pagina

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Contenedor cabecera | `<div>` | `flex items-start justify-between mb-6` |
| Titulo pagina | `<h1>` | `text-3xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Boton nuevo programa | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-5` + `<Plus w-4 h-4 mr-2>` |

#### Componentes de la Barra de Filtros

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Contenedor filtros | `<div>` | `flex items-center gap-3 mb-6` |
| Select filtro estado | `Select` | `w-[180px]` |
| SelectTrigger estado | `SelectTrigger` | `bg-[#0f0f1f] border-[#334155] text-white h-10` |
| SelectContent estado | `SelectContent` | `bg-[#151525] border-[#334155]` |
| Opciones filtro | `SelectItem` x3 | "Todos los estados" / "Solo activos" / "Solo inactivos" |
| Contenedor busqueda | `<div>` | `relative flex-1 max-w-xs` |
| Icono busqueda | `<Search w-4 h-4>` | `absolute left-3 top-3 text-[#64748b] pointer-events-none` |
| Input busqueda | `Input` | `pl-9 bg-[#0f0f1f] border-[#334155] text-white h-10 placeholder:text-[#64748b]` |

#### Componente: PromoProgramaCard

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Card raiz | `Card` | `bg-[#151525] border-[#334155] hover:border-[#a855f7]/50 transition-colors duration-200 cursor-pointer p-5` |
| Fila superior | `<div>` | `flex justify-between items-start mb-3` |
| Bloque titulo-badges | `<div>` | `flex-1 min-w-0` |
| Titulo programa | `<h3>` | `text-base font-semibold text-white truncate` |
| Fila badges | `<div>` | `flex items-center gap-2 mt-1` |
| Badge tipo programa | `Badge` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs` |
| Badge ACTIVO | `Badge` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs` |
| Badge INACTIVO | `Badge` | `bg-slate-800 text-slate-400 border border-slate-700 text-xs` |
| Menu kebab trigger | `Button variant="ghost" size="sm"` | `h-8 w-8 p-0 text-[#64748b] hover:text-white hover:bg-[#1e1e38] flex-shrink-0` + `<MoreVertical w-4 h-4>` |
| DropdownMenu | `DropdownMenu` + `DropdownMenuTrigger` + `DropdownMenuContent` | Content: `bg-[#151525] border-[#334155]` |
| Item "Ver detalle" | `DropdownMenuItem` | `text-[#cbd5e1] hover:bg-[#1e1e38] cursor-pointer` + `<Eye w-4 h-4 mr-2>` |
| Item "Editar" | `DropdownMenuItem` | `text-[#cbd5e1] hover:bg-[#1e1e38] cursor-pointer` + `<Pencil w-4 h-4 mr-2>` |
| Item "Desactivar" | `DropdownMenuItem` | `text-red-400 hover:bg-red-950/30 hover:text-red-300 cursor-pointer` + `<PowerOff w-4 h-4 mr-2>` |
| Campana vinculada | `<p>` | `text-sm text-[#94a3b8] mb-3` con `<Music w-3 h-3 mr-1.5 inline-block>` |
| Grid metricas | `<div>` | `grid grid-cols-2 gap-3` |
| Item metrica | `<div>` | `flex items-center gap-1.5` |
| Icono metrica | Lucide icon | `w-3.5 h-3.5 text-[#64748b]` |
| Valor metrica | `<span>` | `text-sm text-white font-medium` |
| Label metrica | `<span>` | `text-xs text-[#64748b]` |
| Comision display | `<span>` | `text-sm font-semibold text-[#a855f7]` |
| Fechas | `<p>` | `text-xs text-[#64748b] mt-3` con `<Calendar w-3 h-3 mr-1 inline-block>` |

**Props de PromoProgramaCard:**
```typescript
interface PromoProgramaCardProps {
    programa: PromoProgramaListItem
    onVerDetalle: (id: string) => void
    onEditar: (id: string) => void
    onDesactivar: (id: string) => void
}
```

#### Componente de Paginacion (inline, sin shadcn)

La paginacion se implementa con botones `Button variant="outline"` para numeros de pagina y `Button variant="ghost"` para anterior/siguiente. No existe un componente de paginacion en shadcn instalado.

| Elemento | Componente | Clase Tailwind |
|----------|-----------|----------------|
| Contenedor | `<div>` | `flex items-center justify-center gap-2 mt-6` |
| Boton anterior | `Button variant="ghost" size="sm"` | `text-[#94a3b8] hover:text-white` + `<ChevronLeft w-4 h-4>` |
| Numero de pagina activo | `Button variant="outline" size="sm"` | `border-[#a855f7] bg-[#a855f7]/10 text-white` |
| Numero de pagina inactivo | `Button variant="ghost" size="sm"` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton siguiente | `Button variant="ghost" size="sm"` | `text-[#94a3b8] hover:text-white` + `<ChevronRight w-4 h-4>` |

#### Estados de Pantalla 2

| Estado | Visual |
|--------|--------|
| Loading inicial | Grid de 3 `<Skeleton className="h-[180px] w-full bg-[#1e1e38] rounded-xl animate-pulse">` |
| Con programas | Grid de `PromoProgramaCard` reales + paginacion si totalCount > pageSize |
| Empty state (sin programas) | Centrado: `<Megaphone w-12 h-12>` con `bg-gradient-to-br from-pink-500 to-purple-600 p-3 rounded-xl`, titulo "No tienes programas de promocion aun", subtitulo, `Button` gradiente "Crear primer programa" |
| Sin resultados con filtro | Icono `<SearchX w-10 h-10 text-[#64748b]>` centrado + texto + `Button variant="outline"` "Limpiar filtros" con `border-[#334155] text-[#94a3b8]` |
| Error de carga | `Alert variant="destructive"` + `<AlertCircle>` + texto + `Button` "Reintentar" |
| Desactivando | Item del menu "Desactivar" muestra `<Loader2 w-4 h-4 animate-spin>` mientras la mutacion esta pendiente |

---

### 3.3 Pantalla: Detalle del Programa

**Ruta:** `/dashboard/crowdpromotion/programas/{id}`

#### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR              MAIN CONTENT AREA                               │
│                      ┌──────────────────────────────────────────┐   │
│                      │ TOPBAR / breadcrumb                      │   │
│                      │ CrowdPromotion > Mis Programas > {titulo}│   │
│                      ├──────────────────────────────────────────┤   │
│                      │                                          │   │
│                      │ [Banner programa inactivo - condicional] │   │
│                      │                                          │   │
│                      │ {Titulo}          [Editar] [Desactivar]  │   │
│                      │ [TipoPromo] [ACTIVO/INACTIVO]            │   │
│                      │ Icono Music  Campana vinculada           │   │
│                      │                                          │   │
│                      │ ┌────────┐ ┌────────┐ ┌──────┐ ┌──────┐ │   │
│                      │ │KPI 1   │ │KPI 2   │ │KPI 3 │ │KPI 4 │ │   │
│                      │ └────────┘ └────────┘ └──────┘ └──────┘ │   │
│                      │                                          │   │
│                      │ [Info] [Tareas] [Promotores] [Resumen]   │   │
│                      │ ────────────────────────────────────     │   │
│                      │ CONTENIDO DE TAB ACTIVO                  │   │
│                      └──────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

#### Componente: Cabecera del Programa

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Banner inactivo | `Alert` | `bg-amber-950/40 border-amber-800/50 text-amber-300 mb-6` + `<AlertTriangle w-4 h-4>` |
| Contenedor cabecera | `<div>` | `flex items-start justify-between mb-6` |
| Bloque titulo | `<div>` | `flex-1 min-w-0` |
| Titulo | `<h1>` | `text-2xl font-bold text-white` |
| Fila badges | `<div>` | `flex items-center gap-2 mt-2` |
| Badge tipo | `Badge` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155]` |
| Badge ACTIVO | `Badge` | `bg-green-950/50 text-green-400 border-green-800/50` |
| Badge INACTIVO | `Badge` | `bg-slate-800 text-slate-400 border-slate-700` |
| Campana vinculada | `<p>` | `text-sm text-[#94a3b8] mt-1` + `<Music w-3.5 h-3.5 inline mr-1.5>` |
| Boton Editar | `Button variant="outline"` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white` + `<Pencil w-4 h-4 mr-2>` |
| Boton Desactivar | `Button variant="outline"` | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300` + `<PowerOff w-4 h-4 mr-2>` (solo si `esActivo = true`) |
| Boton Reactivar | `Button variant="outline"` | `border-green-800/50 text-green-400 hover:bg-green-950/30 hover:text-green-300` + `<Power w-4 h-4 mr-2>` (solo si `esActivo = false`) |

#### Componente: PromoProgramaKpiCard

Extiende el patron de `StatsCard` existente con customizacion de iconos coloreados.

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Card | `Card` | `bg-[#151525] border-[#334155] p-5` |
| Fila superior | `<div>` | `flex items-center justify-between mb-3` |
| Label KPI | `<p>` | `text-sm text-[#94a3b8] font-medium` |
| Icono contenedor | `<div>` | `w-10 h-10 rounded-lg flex items-center justify-center` + color especifico (ver abajo) |
| Icono | Lucide icon | `w-5 h-5` + color especifico |
| Valor KPI | `<p>` | `text-3xl font-bold text-white` |
| Subtexto | `<p>` | `text-xs text-[#64748b] mt-1` |

**KPI Cards del detalle:**
| KPI | Icono Lucide | Clase icono | Clase contenedor icono | Valor |
|-----|-------------|-------------|------------------------|-------|
| Promotores aprobados | `<UserCheck>` | `text-[#10b981]` | `bg-[rgba(16,185,129,0.15)]` | Entero |
| Promotores pendientes | `<Clock>` | `text-[#f59e0b]` | `bg-[rgba(245,158,11,0.15)]` | Entero |
| Total eventos | `<Activity>` | `text-[#3b82f6]` | `bg-[rgba(59,130,246,0.15)]` | Entero |
| Valor generado | `<TrendingUp>` | `text-[#a855f7]` | `bg-[rgba(168,85,247,0.15)]` | "€X.XX" |

#### Componente: PromoProgramaKpiGrid

```typescript
interface PromoProgramaKpiGridProps {
    resumen: PromoProgramaResumen
    monedaNombre: string
}
```

Renderiza 4 `PromoProgramaKpiCard` en: `className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6"`

#### Tabs del Detalle

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Contenedor tabs | `Tabs defaultValue="info"` | `mt-6` |
| TabsList | `TabsList` | `bg-[#0f0f1f] border border-[#334155] p-1 h-10` |
| TabsTrigger | `TabsTrigger` | `text-[#94a3b8] data-[state=active]:bg-[#1e1e38] data-[state=active]:text-white rounded-md text-sm px-4` |
| TabsContent | `TabsContent` | `mt-4` |

Tabs disponibles: "Info General" (`value="info"`), "Tareas" (`value="tareas"`), "Promotores" (`value="promotores"`), "Resumen" (`value="resumen"`).

#### Tab: Info General

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Grid 2 columnas | `<div>` | `grid grid-cols-1 lg:grid-cols-2 gap-4` |
| Card datos | `Card` | `bg-[#151525] border-[#334155] p-5` |
| Titulo card | `<h3>` | `text-sm font-semibold text-[#cbd5e1] uppercase tracking-wider mb-4` |
| Fila dato | `<div>` | `flex justify-between items-start py-2 border-b border-[#1e1e38] last:border-0` |
| Label | `<span>` | `text-sm text-[#64748b]` |
| Valor | `<span>` | `text-sm text-white` |
| Valor URL | `<a target="_blank">` | `text-sm text-[#a855f7] hover:underline truncate max-w-[200px] inline-block` |
| Valor nulo | `<span>` | `text-sm text-[#64748b] italic` con "--" |

#### Tab: Tareas

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Cabecera tab | `<div>` | `flex justify-between items-center mb-4` |
| Titulo | `<h3>` | `text-base font-semibold text-white` |
| Boton agregar (si activo) | `Button size="sm"` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white text-xs h-8 px-3` + `<Plus w-3.5 h-3.5 mr-1.5>` |
| Lista tareas | `<div>` | `space-y-3` |
| Card tarea | `Card` | `bg-[#0f0f1f] border-[#334155] p-4` |
| Cabecera tarea | `<div>` | `flex justify-between items-start` |
| Nombre tarea | `<h4>` | `text-sm font-semibold text-white` |
| Acciones tarea | `<div>` | `flex gap-2 items-center` |
| Boton editar tarea | `Button variant="ghost" size="sm"` | `text-[#94a3b8] hover:text-white h-7 px-2 text-xs` + `<Pencil w-3.5 h-3.5>` |
| Toggle activa | `Switch` | `scale-75 data-[state=checked]:bg-[#10b981]` |
| Badges tarea | `<div>` | `flex gap-2 mt-2 flex-wrap` |
| Badge tipo evento | `Badge` | `bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs` |
| Badge tipo recompensa | `Badge` | `bg-purple-950/50 text-purple-400 border-purple-800/50 text-xs` |
| Badge repetible | `Badge` | `bg-blue-950/50 text-blue-400 border-blue-800/50 text-xs` + `<Repeat w-2.5 h-2.5 mr-1>` |
| Completados | `<p>` | `text-xs text-[#64748b] mt-2` + `<CheckCircle w-3 h-3 inline mr-1 text-green-500>` |
| Empty state tareas | `<div>` centrado | `py-12 text-center` con `<ClipboardList w-10 h-10>` muted + texto + boton |

#### Tab: Promotores Inscritos

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Tabla | `Table` | `rounded-lg overflow-hidden border border-[#334155]` |
| TableHeader | `TableHeader` | `bg-[#0f0f1f]` |
| TableHead | `TableHead` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider` |
| TableRow | `TableRow` | `border-[#1e1e38] hover:bg-[#1e1e38] transition-colors` |
| TableCell avatar+nombre | `TableCell` | `flex items-center gap-2` |
| Avatar | `Avatar` | `w-7 h-7` |
| AvatarFallback | `AvatarFallback` | `text-xs bg-[#1e1e38] text-[#94a3b8]` |
| Nombre promotor | `<span>` | `text-sm text-white font-medium` |
| Badge aprobado | `Badge` | `bg-green-950/50 text-green-400 border-green-800/50 text-xs` |
| Badge pendiente | `Badge` | `bg-amber-950/50 text-amber-400 border-amber-800/50 text-xs` |
| Badge bloqueado | `Badge` | `bg-red-950/50 text-red-400 border-red-800/50 text-xs` |
| Fecha alta | `<span>` | `text-xs text-[#64748b]` |
| Caption tabla | `<caption>` | `sr-only` (accesibilidad) |
| Empty state | `<div>` centrado dentro de `TableBody` | `<Users w-10 h-10>` muted + "Aun no hay promotores inscritos" |

#### Tab: Resumen Metricas

Reutiliza `PromoProgramaKpiGrid` para mostrar las mismas metricas con contexto adicional. Para MVP, no se implementa chart. Layout de 2 columnas con cards de texto descriptivo.

**Estados de Pantalla 3:**
| Estado | Visual |
|--------|--------|
| Loading | Skeleton para cabecera (`h-8 w-64`, `h-5 w-48`), skeleton para KPI grid (4 cards `h-28`), skeleton para tabs (`h-10 w-full`) |
| Default | Datos cargados, tab "Info General" activa |
| Programa inactivo | `Alert` amarillo en la parte superior: "Este programa esta desactivado. Los promotores no pueden inscribirse." |
| Sin promotores | Empty state en tab Promotores |
| Sin tareas | Empty state en tab Tareas con boton "Agregar primera tarea" |
| Error de carga | `Alert variant="destructive"` centrada reemplaza el contenido principal |

---

### 3.4 Pantalla: Editar Programa

**Ruta:** `/dashboard/crowdpromotion/programas/{id}/editar`

Reutiliza el `WizardLayout` y todos los pasos del wizard de creacion. Las diferencias se gestionan via props.

**Diferencias visuales respecto al wizard de creacion:**

| Diferencia | Implementacion |
|------------|---------------|
| Titulo del header | "Editar programa de promocion" en lugar de "Crear programa de promocion" |
| Banner aviso promotores | `<Alert>` amber visible en todos los pasos si `numeroPromotores > 0`. Texto: "Este programa tiene {n} promotores inscritos. Los cambios en las comisiones se aplicaran a nuevas inscripciones unicamente." |
| Boton del footer paso 4 | "Guardar cambios" con `<Save w-4 h-4>` en lugar de "Publicar programa" |
| Spinner boton editar | "Guardando..." en lugar de "Publicando..." |
| Boton Eliminar tarea con completados | `Button disabled` + `TooltipProvider` con `TooltipContent` "Esta tarea tiene completados registrados y no puede eliminarse" |
| Estado inicial | Loading skeleton mientras se carga el GET /programas/{id} para pre-rellenar el wizard |

**Banner aviso promotores:**
```
<Alert className="bg-amber-950/40 border-amber-800/50 mx-auto max-w-2xl mb-6">
    <AlertTriangle className="h-4 w-4 text-amber-400" />
    <AlertTitle className="text-amber-300 text-sm font-medium">Programa con promotores activos</AlertTitle>
    <AlertDescription className="text-amber-300/80 text-sm">
        Este programa tiene {n} promotores inscritos...
    </AlertDescription>
</Alert>
```

**Estado loading inicial (edicion):**
```
<div className="max-w-2xl mx-auto space-y-4">
    <Skeleton className="h-8 w-3/4 bg-[#1e1e38]" />
    <Skeleton className="h-11 w-full bg-[#1e1e38]" />
    <Skeleton className="h-[120px] w-full bg-[#1e1e38]" />
    <Skeleton className="h-11 w-full bg-[#1e1e38]" />
    <div className="grid grid-cols-2 gap-4">
        <Skeleton className="h-11 bg-[#1e1e38]" />
        <Skeleton className="h-11 bg-[#1e1e38]" />
    </div>
</div>
```

---

### 3.5 Pantalla: Dialog de Desactivacion

**Componente:** `DesactivarProgramaDialog`

Sigue exactamente el patron de `DesactivarPromotorDialog.tsx` existente, customizado para programas.

#### Composicion

```
<AlertDialog open={isOpen} onOpenChange={setIsOpen}>
    <AlertDialogContent className="bg-[#151525] border border-[#334155] max-w-md">
        <AlertDialogHeader>
            <AlertDialogTitle>
                <AlertTriangle /> Desactivar programa de promocion
            </AlertDialogTitle>
            <AlertDialogDescription>
                Esta accion desactivara el programa y todas sus tareas activas.
            </AlertDialogDescription>
        </AlertDialogHeader>

        <!-- Lista de impacto -->
        <ul className="mt-3 space-y-1.5">
            <li><AlertCircle /> {n} tareas activas se desactivaran</li>
            <li><AlertCircle /> El programa dejara de aparecer a nuevos promotores</li>
            <li><AlertCircle /> Los promotores actuales mantienen sus registros</li>
        </ul>
        <p className="text-xs text-[#64748b] mt-3 italic">
            Los eventos historicos seguiran siendo rastreables.
        </p>

        <AlertDialogFooter className="flex gap-3 mt-6">
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction className="bg-red-600 hover:bg-red-700">
                {isPending ? <Loader2 /> : <PowerOff />}
                {isPending ? "Desactivando..." : "Desactivar programa"}
            </AlertDialogAction>
        </AlertDialogFooter>
    </AlertDialogContent>
</AlertDialog>
```

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Overlay | `AlertDialogOverlay` | `bg-black/60 backdrop-blur-sm` |
| Content | `AlertDialogContent` | `bg-[#151525] border border-[#334155] max-w-md` |
| Titulo | `AlertDialogTitle` | `text-lg font-semibold text-white flex items-center gap-2` |
| Icono titulo | `<AlertTriangle w-5 h-5>` | `text-amber-400` |
| Descripcion | `AlertDialogDescription` | `text-sm text-[#94a3b8] mt-2` |
| Lista impacto | `<ul>` | `mt-3 space-y-1.5` |
| Item impacto | `<li>` | `flex items-start gap-2 text-sm text-[#94a3b8]` |
| Icono item | `<AlertCircle w-3.5 h-3.5>` | `text-amber-400 mt-0.5 flex-shrink-0` |
| Texto historico | `<p>` | `text-xs text-[#64748b] mt-3 italic` |
| Footer | `AlertDialogFooter` | `flex gap-3 mt-6` override |
| Cancelar | `AlertDialogCancel` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white flex-1` |
| Confirmar | `AlertDialogAction` | `bg-red-600 hover:bg-red-700 text-white font-semibold flex-1` + icono `<PowerOff w-4 h-4 mr-2>` |
| Spinner | `<Loader2 w-4 h-4 animate-spin>` | Reemplaza icono cuando `isPending` |

**Props de DesactivarProgramaDialog:**
```typescript
interface DesactivarProgramaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    programaTitulo: string
    tareasActivas: number
    programaId: string
}
```

**Estados del dialog:**
| Estado | Visual |
|--------|--------|
| Default | Ambos botones habilitados |
| Confirmando | Boton "Desactivar" muestra `<Loader2>` + "Desactivando...", boton "Cancelar" `disabled` |
| Success | Dialog se cierra, toast verde via sonner |
| Error | Toast destructivo, dialog se cierra |

---

### 3.6 Componente: AbandonarWizardDialog

Dialog de confirmacion al hacer click en "X Cancelar" en el header del wizard.

| Elemento | Componente shadcn | Clase Tailwind |
|----------|-------------------|----------------|
| Content | `AlertDialogContent` | `bg-[#151525] border border-[#334155] max-w-sm` |
| Titulo | `AlertDialogTitle` | `text-base font-semibold text-white` |
| Descripcion | `AlertDialogDescription` | `text-sm text-[#94a3b8]` |
| Boton "Seguir editando" | `AlertDialogCancel` | `flex-1` |
| Boton "Salir sin guardar" | `AlertDialogAction` | `bg-red-600 hover:bg-red-700 flex-1` |

**Props:**
```typescript
interface AbandonarWizardDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirmarSalida: () => void
}
```

---

## 4. Formularios

### 4.1 Formulario Wizard - Campos y Validacion Visual

**Paso 1 - Campos:**
| Campo | Componente | Tipo input | Validacion visual |
|-------|-----------|------------|-------------------|
| titulo | `Input` | text | `FormMessage` "El titulo es obligatorio" / "Min 5 caracteres" |
| descripcion | `Textarea` | textarea | Contador `{n}/4000` debajo |
| tipoPromoId | `Select` | select | `FormMessage` "Selecciona un tipo de programa" |
| campaniaCrowdfundingId | `Select` | select | Opcional, primer item "Sin campana vinculada" |
| proyectoArtisticoId | `Select` | select | Opcional, primer item "Sin proyecto vinculado" |
| urlLanding | `Input` | url | `FormMessage` "Ingresa una URL valida" |
| codigoTrackingBase | `Input` | text | `FormMessage` "Solo letras, numeros y guiones" |
| fechaInicio | `Input` | date | Opcional |
| fechaFin | `Input` | date | `FormMessage` "La fecha fin debe ser posterior a la fecha inicio" |

**Paso 2 - Campos:**
| Campo | Componente | Tipo input | Validacion visual |
|-------|-----------|------------|-------------------|
| monedaId | `Select` | select | `FormMessage` "La moneda es obligatoria" |
| importeComisionPorcentaje | `Input` | number | `FormMessage` "Entre 0 y 100" / error nivel form "Define al menos una comision" |
| importeComisionFija | `Input` | number | `FormMessage` "Debe ser positivo" |

**Paso 3 (formulario de tarea) - Campos:**
| Campo | Componente | Tipo input | Validacion visual |
|-------|-----------|------------|-------------------|
| titulo (nombre) | `Input` | text | `FormMessage` "El nombre es obligatorio" / "Min 3 caracteres" |
| descripcion | `Textarea` | textarea | Opcional, contador caracteres |
| tipoEventoPromoId | `Select` | select | `FormMessage` "Selecciona un tipo de evento" |
| tipoRewardId | `Select` | select | `FormMessage` "Selecciona un tipo de recompensa" |
| importeRecompensa | `Input` | number | Condicional. `FormMessage` "El importe es obligatorio" |
| monedaId | `Select` | select | Condicional. `FormMessage` "Selecciona la moneda" |
| puntosRecompensa | `Input` | number | Condicional. `FormMessage` "Los puntos son obligatorios" |
| urlInstrucciones | `Input` | url | Opcional. `FormMessage` "URL invalida" |
| esRepetible | `Switch` | boolean | Sin error visible; controla visibilidad de maxRepeticiones |
| maxRepeticiones | `Input` | number (min=1) | Condicional. `FormMessage` "Minimo 1 repeticion" |
| fechaInicio | `Input` | date | Opcional |
| fechaFin | `Input` | date | Opcional |

**Estados de inputs:**
| Estado | Visual |
|--------|--------|
| Default | `border-[#334155]` |
| Focus | `border-[#a855f7]` + ring via `focus-visible:ring-[#a855f7]` |
| Error | `border-red-500` + `aria-invalid="true"` + `aria-describedby` |
| Disabled | `opacity-50 cursor-not-allowed bg-[#1a1a2e]` |
| Read-only | `bg-[#0f0f1f] cursor-default` sin focus visible |

---

## 5. Tablas (Tab Promotores en Detalle)

### 5.1 Tabla Promotores Inscritos

**Columnas:**
| Header | Ancho | Alineacion | Contenido |
|--------|-------|------------|-----------|
| Promotor | flex-1 | left | Avatar + nombre |
| Tipo | 120px | left | Texto simple |
| Estado | 100px | center | Badge |
| Alta | 140px | right | Fecha formateada |

**Componentes:**
```
<Table aria-label="Promotores inscritos en este programa">
    <caption className="sr-only">Lista de promotores inscritos</caption>
    <TableHeader>
        <TableRow className="border-[#1e1e38] hover:bg-transparent">
            <TableHead scope="col">Promotor</TableHead>
            <TableHead scope="col">Tipo</TableHead>
            <TableHead scope="col" className="text-center">Estado</TableHead>
            <TableHead scope="col" className="text-right">Alta</TableHead>
        </TableRow>
    </TableHeader>
    <TableBody>
        {promotores.map(p => (
            <TableRow key={p.id}>
                <TableCell>
                    <div className="flex items-center gap-2">
                        <Avatar w-7 h-7><AvatarFallback>iniciales</AvatarFallback></Avatar>
                        <span>{p.promotorNombre}</span>
                    </div>
                </TableCell>
                <TableCell>{p.tipoPromotorNombre}</TableCell>
                <TableCell className="text-center">
                    <Badge estado />
                </TableCell>
                <TableCell className="text-right">{formatFecha(p.fechaAlta)}</TableCell>
            </TableRow>
        ))}
    </TableBody>
</Table>
```

---

## 6. Dialogos

### 6.1 Dialog Desactivar Programa

Ya documentado en seccion 3.5.

### 6.2 Dialog Abandonar Wizard

Ya documentado en seccion 3.6.

### 6.3 AlertDialog Eliminar Tarea (inline en Paso 3)

Dialog pequeno que aparece al hacer click en "Eliminar" en la card de tarea del paso 3.

| Elemento | Componente | Clase Tailwind |
|----------|-----------|----------------|
| Content | `AlertDialogContent` | `bg-[#151525] border border-[#334155] max-w-xs` |
| Titulo | `AlertDialogTitle` | `text-sm font-semibold text-white` |
| Descripcion | `AlertDialogDescription` | `text-sm text-[#94a3b8]` ("Esta accion no se puede deshacer") |
| Cancelar | `AlertDialogCancel` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38]` |
| Eliminar | `AlertDialogAction` | `bg-red-600 hover:bg-red-700 text-white` + `<Trash2 w-4 h-4 mr-2>` |

---

## 7. Feedback y Estados

### 7.1 Loading States

| Contexto | Implementacion |
|----------|---------------|
| Listado inicial | 3 `Skeleton` con `className="h-[180px] w-full bg-[#1e1e38] rounded-xl animate-pulse"` |
| Detalle completo | Skeleton para cabecera + KPI grid + tabs en `space-y-4` |
| Selects de maestras (paso 1) | `Skeleton className="h-11 w-full bg-[#1e1e38] animate-pulse rounded-md"` reemplaza el select |
| Editar - carga inicial | Skeleton para cada campo del wizard en `space-y-4 max-w-2xl mx-auto` |
| Boton submit | `<Loader2 className="w-4 h-4 animate-spin mr-2">` + texto "Publicando..." / "Guardando..." / "Desactivando..." |

**ARIA en loading:**
- Contenedor con `aria-busy="true"` mientras carga
- `aria-label="Cargando..."` en el contenedor principal

### 7.2 Error States

| Contexto | Implementacion |
|----------|---------------|
| Error carga listado | `Alert variant="destructive"` + `<AlertCircle>` + texto + `Button` "Reintentar" |
| Error carga detalle | Igual que listado, reemplaza todo el contenido |
| Error API al crear/editar | Toast destructivo (sonner) + datos del wizard intactos |
| Error de campo (validacion Zod) | `FormMessage` con `<AlertCircle w-3 h-3>` inline debajo del campo. Input con `border-red-500` |
| Error de carga selects maestras | Select deshabilitado con `Alert` inline de error |
| Error 403 (campana no pertenece) | Toast destructivo con mensaje del `PROMO_PROGRAMA_ERROR_MESSAGES` |

### 7.3 Success States

| Contexto | Implementacion |
|----------|---------------|
| Crear programa | Toast sonner verde: "Programa de promocion creado correctamente" + redirect a detalle tras 1.5s |
| Editar programa | Toast sonner verde: "Programa actualizado correctamente" + redirect a detalle |
| Desactivar programa | Toast sonner verde: "Programa desactivado correctamente" + actualizacion optimista del badge |

### 7.4 Empty States

| Contexto | Icono | Titulo | Accion |
|----------|-------|--------|--------|
| Sin programas (listado) | `<Megaphone w-12 h-12>` con bg gradient | "No tienes programas de promocion aun" | `Button` gradiente "Crear primer programa" |
| Sin resultados filtro | `<SearchX w-10 h-10 text-[#64748b]>` | "No se encontraron programas" | `Button variant="outline"` "Limpiar filtros" |
| Sin tareas (tab detalle) | `<ClipboardList w-10 h-10 text-[#64748b]>` | "Sin tareas definidas" | `Button size="sm"` "Agregar primera tarea" (si activo) |
| Sin promotores (tab detalle) | `<Users w-10 h-10 text-[#64748b]>` | "Aun no hay promotores inscritos" | Sin CTA |

---

## 8. Responsive Design

### 8.1 Breakpoints

| Breakpoint | Ancho | Cambios de layout |
|------------|-------|-------------------|
| Mobile | `< md (768px)` | Sidebar: `Sheet` lateral (hamburger). Wizard: 1 columna, footer en columna vertical. KPI grid: 2 cols. Grids de 2 cols en formulario: 1 col |
| Tablet | `md - lg (768-1024px)` | Sidebar colapsada (iconos). Wizard `max-w-xl`. KPI grid: 2+2 cols. Listado: 1 col |
| Desktop | `> lg (1024px)` | Sidebar completa `w-[250px]`. Wizard `max-w-2xl`. KPI grid: 4 cols. Info General detalle: 2 cols |

### 8.2 Clases Responsive por Componente

| Componente | Mobile | Tablet | Desktop |
|------------|--------|--------|---------|
| `WizardLayout` contenedor paso | `px-4` | `max-w-xl mx-auto px-6` | `max-w-2xl mx-auto px-6` |
| Grids de 2 columnas (formulario) | `grid-cols-1` | `md:grid-cols-2` | `md:grid-cols-2` |
| KPI grid (detalle) | `grid-cols-2` | `grid-cols-2` | `lg:grid-cols-4` |
| Info General grid | `grid-cols-1` | `grid-cols-1` | `lg:grid-cols-2` |
| `WizardFooter` botones | `flex-col gap-2 w-full` | `flex-row justify-between` | `flex-row justify-between` |
| `WizardStepper` | Comprimido a "Paso X de 4" + indicadores | Visible completo reducido | Visible completo |
| Tabs detalle | `overflow-x-auto` (scroll horizontal) | Normal | Normal |
| Sidebar | `Sheet` (offcanvas) | Colapsada `w-[70px]` | Expandida `w-[250px]` |

**Sidebar en mobile:**
```
<Sheet>
    <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
            <Menu className="h-5 w-5" />
        </Button>
    </SheetTrigger>
    <SheetContent side="left" className="bg-[#0d0d1a] border-r border-[#334155] w-[250px] p-0">
        <!-- Contenido identico al sidebar desktop -->
    </SheetContent>
</Sheet>
```

**Stepper en mobile:** Se reduce a mostrar el numero de paso actual con los circulos miniaturizados o unicamente el texto "Paso X de 4" centrado en el header del paso.

---

## 9. Animaciones y Transiciones

| Elemento | Animacion | Duracion | Implementacion |
|----------|-----------|----------|----------------|
| Avance de paso wizard | Fade in desde derecha | 200ms | `transition-all duration-200 ease-in-out` + `data-[entering=forward]` |
| Retroceso de paso wizard | Fade in desde izquierda | 200ms | `transition-all duration-200 ease-in-out` + `data-[entering=back]` |
| Formulario tarea inline - abrir | Expand height + fade in | 250ms | `animate-accordion-down` (ya en shadcn globals) o `AnimatePresence` de Framer Motion |
| Formulario tarea inline - cerrar | Collapse + fade out | 200ms | `animate-accordion-up` o `AnimatePresence` |
| Stepper circulo - cambio estado | Color/fondo | 300ms | `transition-all duration-300` en las clases del circulo |
| Stepper conector - completado | Color de linea | 300ms | `transition-colors duration-300` |
| Card hover (listado y paso 3) | Border color | 200ms | `transition-colors duration-200` |
| Dialogo apertura/cierre | Fade + scale | 200ms | Nativo shadcn AlertDialog (Radix) |
| Toast entrada | Slide desde abajo-derecha | 300ms | Nativo sonner |
| Tabs cambio | Fade in contenido | 150ms | `data-[state=active]:animate-in data-[state=active]:fade-in-50` |
| Skeleton | Pulse | Loop | `animate-pulse` de Tailwind |
| Boton hover gradient | Brillo | 150ms | `transition-opacity duration-150` |

---

## 10. Accesibilidad (ARIA)

| Requisito | Implementacion |
|-----------|---------------|
| Contraste texto primario | `text-white` sobre `bg-[#151525]` = 14:1 (WCAG AAA) |
| Contraste texto secundario | `text-[#94a3b8]` sobre `bg-[#151525]` = 4.6:1 (WCAG AA) |
| Contraste texto muted | `text-[#64748b]` sobre `bg-[#151525]` = 3.8:1 (solo uso decorativo, no critico) |
| Contraste badge activo | `text-green-400` sobre `bg-green-950/50` = >4.5:1 |
| Focus ring visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#151525]` en todos los interactivos |
| Labels de formulario | `Label htmlFor={id}` vinculado a `Input id={id}` en cada campo. `aria-required="true"` en campos obligatorios |
| Errores de validacion | `FormMessage` con `role="alert"` o `aria-live="polite"`. Input con `aria-invalid="true"` y `aria-describedby={errorId}` |
| Stepper nav | `<nav aria-label="Progreso del wizard">` + `<ol>` + paso activo con `aria-current="step"` |
| Dialog focus trap | `AlertDialog` de Radix implementa focus trap nativo. Escape cierra |
| Menu kebab | `DropdownMenu` de Radix implementa `aria-haspopup` y `aria-expanded` correctamente |
| Iconos decorativos | `aria-hidden="true"` en todos los iconos Lucide decorativos |
| Switch toggle | `aria-label="La tarea es repetible"` / `aria-label="Tarea activa"` |
| Tabla promotores | `<caption className="sr-only">` + `<TableHead scope="col">` |
| Skeleton loading | `aria-busy="true"` en contenedor. `aria-label="Cargando programas..."` |
| Toast success | `role="status"` (informativo) |
| Toast error | `role="alert"` (critico) |
| Boton deshabilitado | `disabled` + `aria-disabled="true"` + `TooltipContent` con explicacion |

---

## 11. Estructura de Archivos Propuesta

```
src/admin/src/
├── components/
│   └── crowdpromotion/
│       ├── WizardLayout.tsx
│       ├── WizardStepper.tsx
│       ├── WizardFooter.tsx
│       ├── ComisionPreviewCard.tsx
│       ├── PromoTareaCard.tsx
│       ├── PromoTareaForm.tsx
│       ├── PromoProgramaCard.tsx
│       ├── PromoProgramaKpiCard.tsx
│       ├── PromoProgramaKpiGrid.tsx
│       ├── DesactivarProgramaDialog.tsx
│       ├── AbandonarWizardDialog.tsx
│       └── PromotorInscritoRow.tsx
│
└── app/(dashboard)/
    └── crowdpromotion/
        └── programas/
            ├── page.tsx                          <- Listado (Server Component)
            ├── nuevo/
            │   └── page.tsx                      <- Wizard crear (Server Component)
            ├── [id]/
            │   ├── page.tsx                      <- Detalle (Server Component)
            │   └── editar/
            │       └── page.tsx                  <- Wizard editar (Server Component)
            └── components/
                ├── MisProgramasClient.tsx        <- Orquestador listado (Client)
                ├── CrearProgramaClient.tsx        <- Orquestador wizard crear (Client)
                ├── EditarProgramaClient.tsx       <- Orquestador wizard editar (Client)
                ├── ProgramaDetalleClient.tsx      <- Orquestador detalle (Client)
                ├── ProgramaWizardSteps/
                │   ├── Step1DatosBasicos.tsx
                │   ├── Step2Comisiones.tsx
                │   ├── Step3Tareas.tsx
                │   └── Step4Revisar.tsx
                └── __tests__/
                    ├── PromoProgramaCard.test.tsx
                    ├── PromoTareaCard.test.tsx
                    ├── DesactivarProgramaDialog.test.tsx
                    └── WizardStepper.test.tsx
```

---

## 12. Sidebar - Actualizacion Requerida

El `Sidebar` existente (`components/layout/sidebar.tsx`) debe extenderse para incluir el item de CrowdPromotion / Programas:

| Item a agregar | Icono | Href |
|---------------|-------|------|
| "Mis Programas" (bajo CrowdPromotion) | `<Megaphone>` | `/dashboard/crowdpromotion/programas` |

El item de "Promotor" existente puede agruparse visualmente bajo una seccion "CrowdPromotion" con una etiqueta de grupo `text-xs text-[#64748b] uppercase tracking-wider px-3 mb-1`.

---

## 13. Checklist

### Componentes shadcn/ui
- [ ] Switch instalado via `npx shadcn@latest add switch`
- [ ] Sheet ya instalado (verificar - usado para sidebar mobile)
- [ ] Todos los demas componentes ya instalados confirmados

### Wizard (Crear y Editar)
- [ ] `WizardLayout` con header, scroll area y footer fijos
- [ ] `WizardStepper` con estados activo/completado/futuro + ARIA nav
- [ ] `WizardFooter` con Anterior/Siguiente/Publicar/Guardar + loading state
- [ ] `Step1DatosBasicos` con todos los campos, selects y validacion Zod por paso
- [ ] `Step2Comisiones` con `ComisionPreviewCard` reactiva
- [ ] `Step3Tareas` con lista de `PromoTareaCard` + `PromoTareaForm` inline
- [ ] `Step4Revisar` con resumen completo y botones "Editar" por seccion
- [ ] `AbandonarWizardDialog` al click en X del header
- [ ] Banner aviso promotores en modo edicion
- [ ] Loading skeleton en modo edicion (carga del GET)
- [ ] Toast success + redirect a detalle

### Listado
- [ ] `PromoProgramaCard` con todos los datos, badges y menu kebab
- [ ] Filtro estado con `Select`
- [ ] Busqueda local con debounce 300ms
- [ ] Empty state sin programas con CTA
- [ ] Empty state sin resultados con filtro
- [ ] Skeleton loading con `animate-pulse`
- [ ] Paginacion con botones `Button`
- [ ] Error state con "Reintentar"

### Detalle
- [ ] Cabecera con titulo, badges y botones Editar/Desactivar
- [ ] `PromoProgramaKpiGrid` con 4 cards coloreadas
- [ ] Tabs: Info, Tareas, Promotores, Resumen
- [ ] Tab Tareas con `PromoTareaCard` (variante detalle)
- [ ] Tab Promotores con `Table` + `Avatar` + badges de estado
- [ ] Banner programa inactivo (amber) cuando `esActivo = false`
- [ ] Empty states en tabs
- [ ] Skeleton loading completo

### Dialogos
- [ ] `DesactivarProgramaDialog` con lista de impacto y loading state
- [ ] AlertDialog de eliminar tarea inline en Paso 3

### General
- [ ] Switch instalado y usado en `PromoTareaCard` y `PromoTareaForm`
- [ ] Todos los inputs tienen `Label` con `htmlFor`
- [ ] `aria-invalid` y `aria-describedby` en campos con error
- [ ] Focus ring visible en todos los interactivos
- [ ] `aria-hidden="true"` en todos los iconos decorativos
- [ ] Responsive: sidebar como `Sheet` en mobile
- [ ] Responsive: grids 2col a 1col en mobile (`md:grid-cols-2`)
- [ ] Responsive: KPI grid `grid-cols-2 lg:grid-cols-4`
- [ ] Sidebar actualizada con item "Mis Programas"
- [ ] Transiciones de paso en wizard
- [ ] Toast configurado con sonner para success y error
