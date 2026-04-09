# Diseno UI: cp-inscripcion-programa (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/admin
**Ruta de pantallas:** `/dashboard/crowdpromotion/programas/[id]`

---

## 1. Resumen

Esta feature extiende la pantalla de detalle de programa (US-CP-02, Pantalla 3) en el panel de administracion del artista. Se agregan tres tabs nuevos al bloque de Tabs existente: "Solicitudes", "Aprobados" y "Bloqueados", mas tres dialogos de confirmacion para las acciones destructivas.

- Componentes shadcn utilizados: 14
- Composiciones custom: 6
- Componentes nuevos a crear: 6
- Responsive breakpoints: md (768px), lg (1024px)

**Componentes shadcn activos en el proyecto** (confirmados en `src/admin/src/components/ui/`):

| Componente | Archivo confirmado |
|------------|--------------------|
| Tabs, TabsList, TabsTrigger, TabsContent | `tabs.tsx` |
| AlertDialog (+ variantes) | `alert-dialog.tsx` |
| Card, CardHeader, CardContent | `card.tsx` |
| Badge | `badge.tsx` |
| Button | `button.tsx` |
| Avatar, AvatarFallback | `avatar.tsx` |
| Separator | `separator.tsx` |
| Skeleton | `skeleton.tsx` |
| Tooltip, TooltipProvider, TooltipContent | `tooltip.tsx` |
| Table, TableHeader, TableBody, TableRow, TableCell, TableHead | `table.tsx` |

---

## 2. Paleta de Colores del Proyecto

Heredada de US-CP-01/02. Variables CSS relevantes para esta feature:

| Uso | Variable / Valor | Aplicacion |
|-----|------------------|------------|
| Fondo base | `#0d0d1a` | Sidebar, header |
| Fondo pagina | `#1a1a2e` | Contenedor principal |
| Fondo card | `#151525` | Cards de solicitudes |
| Fondo card elevado | `#0f0f1f` | Cards dentro de tabs |
| Fondo hover | `#1e1e38` | Hover en filas y botones |
| Color primario | `#a855f7` | Botones, acentos, codigo referido |
| Gradiente primary | `from-pink-500 to-purple-600` | Boton "Aprobar" |
| Borde default | `#334155` | Cards, separadores |
| Texto primario | `#ffffff` | Nombres, valores clave |
| Texto secundario | `#94a3b8` | Tipos, subtitulos |
| Texto muted | `#64748b` | Fechas, labels |
| Estado Pendiente | bg: `amber-950/50`, text: `amber-400`, border: `amber-800/50` | Badge y contador |
| Estado Aprobado | bg: `green-950/50`, text: `green-400`, border: `green-800/50` | Badge |
| Estado Bloqueado | bg: `red-950/50`, text: `red-400`, border: `red-800/50` | Badge, avatar, bordes |
| Accion destructiva | bg: `red-900/80`, border: `red-800/50`, text: `red-300` | Boton confirmar bloquear/baja |

---

## 3. Contexto: Integracion con Pantalla Existente (US-CP-02)

La pantalla de detalle del programa en `/dashboard/crowdpromotion/programas/[id]` tiene actualmente un bloque de Tabs con los valores `"info"` y `"tareas"`. Esta feature agrega tres tabs nuevos sin modificar los existentes.

### Layout general (sin cambios estructurales)

```
┌───────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64 lg, w-56 md, drawer mobile)                     │
│  ┌────────────────────────────────────────────────────────┐   │
│  │ Breadcrumb: CrowdPromotion / Mis Programas / {titulo}  │   │
│  ├────────────────────────────────────────────────────────┤   │
│  │ Header de programa (titulo, badges, botones accion)     │   │
│  │                                                        │   │
│  │ KPI Cards: [Aprobados: 5] [Pendientes: 2] [Bloq.: 1]  │   │
│  │                                                        │   │
│  │ ┌──────┬──────┬─────────────┬──────────┬───────────┐  │   │
│  │ │Info  │Tareas│Solicitudes 2│Aprobados │Bloqueados │  │   │
│  │ └──────┴──────┴─────────────┴──────────┴───────────┘  │   │
│  │                                                        │   │
│  │ [Contenido del tab activo]                             │   │
│  └────────────────────────────────────────────────────────┘   │
└───────────────────────────────────────────────────────────────┘
```

---

## 4. Componente: TabsList Extendido

### Descripcion

El `TabsList` existente se extiende con tres `TabsTrigger` nuevos. El tab "Solicitudes" lleva un `Badge` contador que muestra el numero de solicitudes pendientes cuando es mayor que 0.

### Composicion

```tsx
<ScrollArea className="w-full" orientation="horizontal">
  <Tabs defaultValue="info">
    <TabsList className="bg-[#151525] border border-[#334155] h-auto p-1 gap-1 w-max min-w-full">
      <TabsTrigger value="info" className="...">Info General</TabsTrigger>
      <TabsTrigger value="tareas" className="...">Tareas</TabsTrigger>

      {/* NUEVO - Tab Solicitudes con badge contador */}
      <TabsTrigger value="solicitudes" className="... flex items-center gap-1.5">
        Solicitudes
        {pendientesCount > 0 && (
          <Badge
            className="bg-amber-500 text-black text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0"
            aria-label={`${pendientesCount} solicitudes pendientes`}
          >
            {pendientesCount}
          </Badge>
        )}
      </TabsTrigger>

      {/* NUEVO - Tab Aprobados */}
      <TabsTrigger value="aprobados" className="...">Aprobados</TabsTrigger>

      {/* NUEVO - Tab Bloqueados */}
      <TabsTrigger value="bloqueados" className="...">Bloqueados</TabsTrigger>
    </TabsList>

    {/* Contenidos de tabs existentes */}
    <TabsContent value="info">...</TabsContent>
    <TabsContent value="tareas">...</TabsContent>

    {/* NUEVOS contenidos */}
    <TabsContent value="solicitudes">
      <SolicitudesPendientesTab programaId={programaId} />
    </TabsContent>
    <TabsContent value="aprobados">
      <PromotoresAprobadosTab programaId={programaId} />
    </TabsContent>
    <TabsContent value="bloqueados">
      <PromotoresBloqueadosTab programaId={programaId} />
    </TabsContent>
  </Tabs>
</ScrollArea>
```

### Customizaciones del TabsTrigger

| Propiedad | Valor |
|-----------|-------|
| Padding | `px-4 py-2` |
| Texto | `text-sm font-medium` |
| Estado inactivo | `text-[#64748b] hover:text-white hover:bg-[#1e1e38]` |
| Estado activo | `data-[state=active]:bg-[#1e1e38] data-[state=active]:text-white data-[state=active]:shadow-none` |
| Border radius | `rounded-md` |

### Mobile (ScrollArea horizontal)

En pantallas `< 768px`, el TabsList se envuelve en `<ScrollArea orientation="horizontal">` para permitir scroll horizontal sin ocultar tabs. Los tabs mantienen `whitespace-nowrap`.

---

## 5. Tab: Solicitudes Pendientes

### Archivo sugerido

`src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/SolicitudesPendientesTab.tsx`

### Layout del tab

```
┌─────────────────────────────────────────────────────────────┐
│ Cabecera del tab                                             │
│ Solicitudes pendientes              "Revisa y gestiona..."  │
│                                                             │
│ ┌───────────────────────────────────────────────────────┐   │
│ │ [AV] DJ Marketing Pro                    hace 2 dias   │   │
│ │      Influencer                                        │   │
│ │ ─────────────────────────────────────────────────────  │   │
│ │ Instagram: @djmarkpro (15k)                            │   │
│ │ TikTok:    @djmarkpro.official                         │   │
│ │ Web:       djmarkpro.com                               │   │
│ │ ─────────────────────────────────────────────────────  │   │
│ │                        [Aprobar]  [Rechazar] [Bloq.]   │   │
│ └───────────────────────────────────────────────────────┘   │
│                                                             │
│ [Estado vacio: InboxIcon + "No hay solicitudes pendientes"] │
└─────────────────────────────────────────────────────────────┘
```

### Componentes por elemento

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor del tab | div | `space-y-4 mt-4` |
| Cabecera | div | `flex items-start justify-between mb-4` |
| Titulo | h3 | `text-base font-semibold text-white` |
| Subtitulo | p | `text-sm text-[#94a3b8] mt-0.5` |
| Card solicitud | `Card` | `bg-[#0f0f1f] border border-[#334155] p-4 mb-3 transition-all duration-200` |
| Header card | div | `flex items-start justify-between` |
| Bloque promotor | div | `flex items-center gap-3` |
| Avatar | `Avatar` (w-10 h-10) | `AvatarFallback`: `bg-[#1e1e38] text-[#94a3b8] text-sm font-semibold` |
| Nombre promotor | p | `text-sm font-semibold text-white` |
| Tipo promotor | p | `text-xs text-[#94a3b8]` |
| Fecha relativa | p | `text-xs text-[#64748b] shrink-0` |
| Separador | `Separator` | `bg-[#334155] my-3` |
| Bloque redes | div | `space-y-1.5` |
| Fila de red | div | `flex items-center gap-2` |
| Label red | span | `text-xs text-[#64748b] w-20 shrink-0` |
| Valor URL red | a | `text-xs text-[#a855f7] hover:underline` |
| Valor handle red | span | `text-xs text-[#94a3b8]` |
| Sin redes | p | `text-xs text-[#64748b] italic` |
| Segundo separador | `Separator` | `bg-[#334155] mt-3 mb-2` |
| Contenedor acciones | div | `flex justify-end gap-2 mt-2 flex-col sm:flex-row` |
| Boton Aprobar | `Button` size="sm" | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-8 px-4 text-xs` |
| Boton Rechazar | `Button` variant="outline" size="sm" | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 px-3 text-xs` |
| Boton Bloquear | `Button` variant="outline" size="sm" | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-8 px-3 text-xs` |

### Iconos Lucide

| Boton | Icono | Clases |
|-------|-------|--------|
| Aprobar | `UserCheck` | `w-3.5 h-3.5 mr-1.5` |
| Rechazar | `UserX` | `w-3.5 h-3.5 mr-1.5` |
| Bloquear | `Ban` | `w-3.5 h-3.5 mr-1.5` |
| Red Instagram | `Instagram` (brand) o `Link` | `w-3.5 h-3.5 text-[#64748b] shrink-0` |
| Red TikTok | `Link` o SVG custom | `w-3.5 h-3.5 text-[#64748b] shrink-0` |
| Sitio web | `Globe` | `w-3.5 h-3.5 text-[#64748b] shrink-0` |
| Email | `Mail` | `w-3.5 h-3.5 text-[#64748b] shrink-0` |

### Estado loading

```tsx
{/* Skeleton de cards para estado loading */}
<div className="space-y-3" aria-busy="true" aria-label="Cargando solicitudes">
  {[1, 2, 3].map((i) => (
    <Skeleton key={i} className="h-[140px] rounded-xl bg-[#1e1e38]" />
  ))}
</div>
```

### Estado empty

```tsx
<div className="flex flex-col items-center justify-center py-12 text-center">
  <InboxIcon className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
  <p className="text-sm text-[#64748b]">No hay solicitudes pendientes</p>
</div>
```

### Estado loading por accion (boton pulsado)

Cuando el artista pulsa "Aprobar" en un item:
- El boton "Aprobar" muestra `<Loader2 className="w-3.5 h-3.5 animate-spin mr-1.5" />` y el texto cambia a "Aprobando..."
- Los tres botones del mismo item quedan `disabled`
- Los botones de otros items permanecen activos
- Tras respuesta exitosa: la card desaparece con `transition-all opacity-0 h-0 overflow-hidden duration-200`

### Variantes de estado de accion

| Accion | Boton loading | Texto loading | Items deshabilitados |
|--------|---------------|---------------|----------------------|
| Aprobar | Aprobar | "Aprobando..." | Los 3 del mismo item |
| Rechazar | Rechazar (post-dialog) | "Rechazando..." | Los 3 del mismo item |
| Bloquear | Bloquear (post-dialog) | "Bloqueando..." | Los 3 del mismo item |

---

## 6. Tab: Promotores Aprobados

### Archivo sugerido

`src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromotoresAprobadosTab.tsx`

### Layout del tab

```
┌─────────────────────────────────────────────────────────────┐
│ Promotores aprobados (5)                                    │
│                                                             │
│ ┌───────────────────────────────────────────────────────┐   │
│ │ [AV] DJ Marketing Pro        album-2026-x7k9m         │   │
│ │      Influencer              Clics: 142 | Conv.: 8     │   │
│ │      Aprobado: 10 Mar 2026              [Dar de baja]  │   │
│ └───────────────────────────────────────────────────────┘   │
│                                                             │
│ [Estado vacio: Users icon + "No hay promotores aprobados"]  │
└─────────────────────────────────────────────────────────────┘
```

### Componentes por elemento

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor del tab | div | `space-y-3 mt-4` |
| Cabecera | div | `flex items-center justify-between mb-4` |
| Titulo con contador | h3 | `text-base font-semibold text-white` |
| Card promotor | `Card` | `bg-[#0f0f1f] border border-[#334155] p-4 transition-all duration-200` |
| Cabecera card | div | `flex items-start gap-3` |
| Avatar | `Avatar` (w-9 h-9) | `AvatarFallback`: `bg-[#1e1e38] text-[#94a3b8] text-xs font-semibold` |
| Bloque info izquierda | div | `flex-1 min-w-0` |
| Nombre promotor | p | `text-sm font-semibold text-white truncate` |
| Tipo promotor | p | `text-xs text-[#94a3b8]` |
| Fecha aprobacion | p | `text-xs text-[#64748b] mt-0.5 flex items-center gap-1` |
| Icono calendario | `Calendar` | `w-3 h-3` |
| Bloque info derecha | div | `text-right shrink-0` |
| Codigo referido | p | `text-xs font-mono text-[#a855f7] font-medium` |
| Stats MVP (placeholder) | p | `text-xs text-[#64748b] mt-0.5` |
| Boton Dar de baja | `Button` variant="outline" size="sm" | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-7 px-3 text-xs mt-2 w-full sm:w-auto` |

### Icono del boton

| Boton | Icono | Clases |
|-------|-------|--------|
| Dar de baja | `UserMinus` | `w-3.5 h-3.5 mr-1` |

### Estado loading

```tsx
<div className="space-y-3" aria-busy="true" aria-label="Cargando promotores aprobados">
  {[1, 2, 3].map((i) => (
    <Skeleton key={i} className="h-[90px] rounded-xl bg-[#1e1e38]" />
  ))}
</div>
```

### Estado empty

```tsx
<div className="flex flex-col items-center justify-center py-12 text-center">
  <Users className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
  <p className="text-sm text-[#64748b]">No hay promotores aprobados en este programa</p>
</div>
```

---

## 7. Tab: Promotores Bloqueados

### Archivo sugerido

`src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromotoresBloqueadosTab.tsx`

### Layout del tab

```
┌─────────────────────────────────────────────────────────────┐
│ ┌──────────────────────────────────────────────────────┐    │
│ │ [i] Los promotores bloqueados no pueden re-solicitar │    │
│ │     inscripcion en este programa.                    │    │
│ └──────────────────────────────────────────────────────┘    │
│                                                             │
│ ┌───────────────────────────────────────────────────────┐   │
│ │ [AV] Spammer123                           [BLOQUEADO] │   │
│ │      Fan Embajador                                    │   │
│ │      Bloqueado: 01 Feb 2026                           │   │
│ └───────────────────────────────────────────────────────┘   │
│                                                             │
│ [Estado vacio: ShieldCheck + "No hay promotores bloqueados"]│
└─────────────────────────────────────────────────────────────┘
```

### Componentes por elemento

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor del tab | div | `space-y-3 mt-4` |
| Aviso informativo | div | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 mb-4` |
| Icono info | `Info` (Lucide) | `w-4 h-4 text-blue-400 shrink-0 mt-0.5` |
| Texto aviso | p | `text-sm text-blue-300` |
| Card bloqueado | `Card` | `bg-[#0f0f1f] border border-[#334155] p-4 opacity-80` |
| Cabecera card | div | `flex items-center gap-3` |
| Avatar | `Avatar` (w-9 h-9) | `AvatarFallback`: `bg-red-950/50 text-red-400 text-xs font-semibold` |
| Bloque info | div | `flex-1 min-w-0` |
| Nombre promotor | p | `text-sm font-medium text-white truncate` |
| Tipo promotor | p | `text-xs text-[#94a3b8]` |
| Fecha bloqueo | p | `text-xs text-[#64748b] mt-1 flex items-center gap-1` |
| Badge BLOQUEADO | `Badge` | `bg-red-950/50 text-red-400 border border-red-800/50 text-xs ml-auto shrink-0 flex items-center gap-1` |
| Icono Ban en badge | `Ban` | `w-3 h-3 mr-1` |

### Estado empty

```tsx
<div className="flex flex-col items-center justify-center py-12 text-center">
  <ShieldCheck className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
  <p className="text-sm text-[#64748b]">No hay promotores bloqueados</p>
</div>
```

**Nota:** Este tab es de solo lectura. No tiene botones de accion. Sin estado loading por ser una lista simple derivada del mismo endpoint que los otros tabs.

---

## 8. Dialogos de Confirmacion

Los tres dialogos utilizan el componente `AlertDialog` de shadcn, que ya esta en el proyecto (`src/admin/src/components/ui/alert-dialog.tsx`) y cuenta con el patron de uso establecido en `DeleteConfirmDialog.tsx`.

Se recomienda crear un componente reutilizable `InscripcionConfirmDialog.tsx` que extienda el patron existente de `DeleteConfirmDialog.tsx` con soporte para variantes visuales (neutral vs destructivo).

### Archivo sugerido

`src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/InscripcionConfirmDialog.tsx`

### Props del componente

```tsx
interface InscripcionConfirmDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
  isPending?: boolean
  variante: "rechazar" | "bloquear" | "dar-de-baja"
  promotorNombre: string
}
```

### 8.1 Dialogo: Rechazar Solicitud

| Elemento | Componente shadcn | Contenido / Customizacion |
|----------|-------------------|---------------------------|
| Contenedor | `AlertDialog` | Controlado via `open` / `onOpenChange` |
| Content | `AlertDialogContent` | `bg-[#151525] border-[#334155] sm:max-w-md` |
| Titulo | `AlertDialogTitle` | "Rechazar solicitud" / `text-white font-semibold` |
| Descripcion | `AlertDialogDescription` | "¿Seguro que deseas rechazar la solicitud de **{nombre}**? Podra volver a solicitarse en el futuro." / `text-[#94a3b8]` |
| Boton cancelar | `AlertDialogCancel` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` / Texto: "Cancelar" |
| Boton confirmar | `AlertDialogAction` | `border-[#334155] text-white hover:bg-[#1e1e38]` (estilo neutro, no destructivo) / Texto: "Rechazar" |
| Estado loading | `Loader2` en confirmar | `w-4 h-4 animate-spin mr-2` cuando `isPending=true` |

**Racional del estilo neutro:** Rechazar no tiene consecuencias permanentes (el registro se elimina y el promotor puede re-solicitar). El boton de confirmacion no debe ser visualmente destructivo.

### 8.2 Dialogo: Bloquear Promotor

| Elemento | Componente shadcn | Contenido / Customizacion |
|----------|-------------------|---------------------------|
| Contenedor | `AlertDialog` | Controlado via `open` / `onOpenChange` |
| Content | `AlertDialogContent` | `bg-[#151525] border-[#334155] sm:max-w-md` |
| Titulo | `AlertDialogTitle` | "Bloquear promotor" / `text-white font-semibold` |
| Descripcion | `AlertDialogDescription` | "¿Seguro que deseas bloquear a **{nombre}**? No podra volver a solicitar inscripcion en este programa." / `text-[#94a3b8]` |
| Boton cancelar | `AlertDialogCancel` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton confirmar | `AlertDialogAction` | `bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200` (estilo destructivo) / Texto: "Bloquear" |
| Estado loading | `Loader2` en confirmar | `w-4 h-4 animate-spin mr-2` cuando `isPending=true` |

**Racional del estilo destructivo:** Bloquear es una accion permanente con consecuencias significativas para el promotor.

### 8.3 Dialogo: Dar de Baja

| Elemento | Componente shadcn | Contenido / Customizacion |
|----------|-------------------|---------------------------|
| Contenedor | `AlertDialog` | Controlado via `open` / `onOpenChange` |
| Content | `AlertDialogContent` | `bg-[#151525] border-[#334155] sm:max-w-md` |
| Titulo | `AlertDialogTitle` | "Dar de baja al promotor" / `text-white font-semibold` |
| Descripcion | `AlertDialogDescription` | "¿Seguro que deseas dar de baja a **{nombre}**? Su codigo referido quedara desactivado." / `text-[#94a3b8]` |
| Boton cancelar | `AlertDialogCancel` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton confirmar | `AlertDialogAction` | `bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200` (estilo destructivo) / Texto: "Dar de baja" |
| Estado loading | `Loader2` en confirmar | `w-4 h-4 animate-spin mr-2` cuando `isPending=true` |

### Composicion consolidada del dialogo

```tsx
<AlertDialog open={open} onOpenChange={onOpenChange}>
  <AlertDialogContent className="bg-[#151525] border-[#334155] sm:max-w-md">
    <AlertDialogHeader>
      <AlertDialogTitle className="text-white font-semibold">
        {titulo}
      </AlertDialogTitle>
      <AlertDialogDescription className="text-[#94a3b8]">
        {descripcion}
      </AlertDialogDescription>
    </AlertDialogHeader>
    <AlertDialogFooter>
      <AlertDialogCancel
        disabled={isPending}
        className="border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white"
      >
        Cancelar
      </AlertDialogCancel>
      <AlertDialogAction
        onClick={onConfirm}
        disabled={isPending}
        className={esDestructivo
          ? "bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200"
          : "border-[#334155] text-white hover:bg-[#1e1e38]"
        }
      >
        {isPending && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
        {textoConfirmar}
      </AlertDialogAction>
    </AlertDialogFooter>
  </AlertDialogContent>
</AlertDialog>
```

---

## 9. Feedback y Estados

### 9.1 Toast Messages (via sonner)

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Promotor aprobado | `toast.success` | "{nombre} ha sido aprobado. Se ha generado su codigo referido." |
| Promotor rechazado | `toast` (default) | "Solicitud de {nombre} rechazada." |
| Promotor bloqueado | `toast` (default) | "{nombre} ha sido bloqueado en este programa." |
| Promotor dado de baja | `toast` (default) | "{nombre} ha sido dado de baja. Su codigo referido ha sido desactivado." |
| Error al aprobar | `toast.error` | "No se pudo aprobar al promotor. Intentalo de nuevo." |
| Error al rechazar | `toast.error` | "No se pudo rechazar la solicitud. Intentalo de nuevo." |
| Error al bloquear | `toast.error` | "No se pudo bloquear al promotor. Intentalo de nuevo." |
| Error al dar de baja | `toast.error` | "No se pudo dar de baja al promotor. Intentalo de nuevo." |

### 9.2 Loading States

| Contexto | Componente | Detalles |
|----------|------------|---------|
| Tab Solicitudes cargando | `Skeleton` x3 | `h-[140px] rounded-xl bg-[#1e1e38] animate-pulse` |
| Tab Aprobados cargando | `Skeleton` x3 | `h-[90px] rounded-xl bg-[#1e1e38] animate-pulse` |
| Boton accion en proceso | `Loader2` dentro de `Button` | `w-3.5 h-3.5 animate-spin mr-1.5` + texto cambiado + `disabled` |
| Dialogo confirmando | `Loader2` en `AlertDialogAction` | `w-4 h-4 animate-spin mr-2` + ambos botones `disabled` |

### 9.3 Animacion fade-out al eliminar item

Al ejecutarse una accion exitosa que remueve un item (aprobar, rechazar, bloquear, dar de baja), la card desaparece con la siguiente transicion CSS:

```
transition: opacity 200ms ease-out, max-height 200ms ease-out
Estado inicial: opacity-100, max-h-[auto]
Estado final: opacity-0, max-h-0, overflow-hidden
```

En React, esto se puede implementar via estado local `isRemoving` que aplica las clases de transicion antes de remover el item del array.

### 9.4 Actualizacion del badge contador

Tras aprobar/rechazar/bloquear una solicitud pendiente, el badge contador del tab "Solicitudes" se actualiza de forma inmediata (sincronamente) decrementando el valor en 1. No requiere re-fetch.

---

## 10. Estructura de Archivos

```
src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/
  components/
    SolicitudesPendientesTab.tsx        <- Tab con cards de solicitudes + acciones
    PromotoresAprobadosTab.tsx          <- Tab con cards de aprobados + dar de baja
    PromotoresBloqueadosTab.tsx         <- Tab de solo lectura
    InscripcionConfirmDialog.tsx        <- Dialogo reutilizable (3 variantes)
    SolicitudCard.tsx                   <- Card individual de solicitud pendiente
    PromoторAprobadoCard.tsx            <- Card individual de promotor aprobado
```

---

## 11. Responsive Design

| Breakpoint | Comportamiento |
|------------|----------------|
| `< 768px` (mobile) | Tabs en ScrollArea horizontal. Botones de accion en columna (`flex-col gap-2 w-full`). Cards a ancho completo. Sidebar como drawer |
| `768px-1024px` (tablet) | Tabs visibles sin scroll. Botones de accion en fila (`flex-row justify-end`). Sidebar visible (w-56) |
| `> 1024px` (desktop) | Layout completo. Sidebar completo (w-64). Cards con layout dos columnas cuando hay espacio |

### Clases Tailwind clave para responsive

```
- Botones de accion: flex-col sm:flex-row gap-2 sm:justify-end
- Avatar: w-10 h-10 (solicitudes) / w-9 h-9 (aprobados/bloqueados)
- Cards: w-full (por defecto en todos los breakpoints)
- TabsList: overflow-x-auto en mobile via ScrollArea
```

---

## 12. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Badge contador en tab | `aria-label="N solicitudes pendientes"` en el Badge numerico |
| Botones icon+texto | Los iconos tienen `aria-hidden="true"`. El texto visible es suficiente para el nombre accesible |
| Dialogo de confirmacion | `AlertDialog` de Radix/shadcn gestiona: focus trap, cierre con Escape, `aria-labelledby` apuntando a `AlertDialogTitle`, `aria-describedby` apuntando a `AlertDialogDescription` |
| Estados de carga | Contenedor de skeletons con `aria-busy="true"` y `aria-label="Cargando..."` |
| Spinners en botones | `<Loader2 aria-hidden="true" />` junto con texto visible que cambia (ej: "Aprobando...") |
| Cards sin action accesible | Cada card de solicitud tiene sus botones con nombres de accion claros que incluyen contexto (no solo "Aprobar" sino el contexto del item activo) |
| Avatar fallback | El texto de initiales en `AvatarFallback` es `aria-hidden="true"` porque el nombre completo esta visible en el elemento adyacente |
| Iconos decorativos | Todos los iconos sin texto adyacente tienen `aria-hidden="true"` |
| Empty states | El icono del empty state tiene `aria-hidden="true"`. El mensaje de texto es suficiente |
| Focus visible | Heredado del sistema de shadcn: `focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2` en todos los elementos interactivos |
| Contraste | Texto blanco (`#ffffff`) sobre fondo oscuro (`#0f0f1f`): ratio ~21:1. Texto muted (`#94a3b8`) sobre `#0f0f1f`: ratio ~8:1. Ambos superan WCAG AA |
| Rol de tabs | `TabsList` tiene `role="tablist"` automatico via Radix. `TabsTrigger` tiene `role="tab"`. `TabsContent` tiene `role="tabpanel"` con `aria-labelledby` apuntando al trigger correspondiente |

---

## 13. Checklist

- [ ] Tab "Solicitudes" muestra badge contador cuando hay pendientes > 0
- [ ] Badge contador tiene aria-label accesible
- [ ] Tabs scrollables horizontalmente en mobile via ScrollArea
- [ ] Card de solicitud muestra: avatar con iniciales, nombre, tipo, fecha relativa, redes sociales, tres botones
- [ ] Redes sociales muestran mensaje "Sin redes" cuando todos los campos son null
- [ ] Boton "Aprobar" tiene estilo gradient (consistente con acciones primarias del proyecto)
- [ ] Boton "Rechazar" tiene estilo outline neutro (no destructivo)
- [ ] Boton "Bloquear" tiene estilo outline rojo (destructivo suave)
- [ ] Click en "Rechazar" abre AlertDialog de confirmacion (NO ejecuta directamente)
- [ ] Click en "Bloquear" abre AlertDialog de confirmacion (NO ejecuta directamente)
- [ ] Click en "Aprobar" ejecuta directamente (sin dialogo, es accion positiva)
- [ ] Dialogo "Rechazar": boton confirmar estilo neutro (sin color destructivo)
- [ ] Dialogo "Bloquear": boton confirmar estilo destructivo rojo
- [ ] Dialogo "Dar de baja": boton confirmar estilo destructivo rojo
- [ ] AlertDialogContent con fondo `bg-[#151525]` (consistente con el tema oscuro)
- [ ] Botones deshabilitados durante peticion en curso (`isPending=true`)
- [ ] Card de solicitud desaparece con fade-out tras accion exitosa
- [ ] Toast de exito/error tras cada accion
- [ ] Tab Aprobados: card muestra avatar, nombre, tipo, fecha, codigo referido (monospace, color primario), stats placeholder
- [ ] Boton "Dar de baja" en aprobados abre AlertDialog
- [ ] Tab Bloqueados: solo lectura, sin botones de accion
- [ ] Tab Bloqueados: aviso informativo en azul en la parte superior
- [ ] Tab Bloqueados: cards con opacidad 80% y avatar en tono rojo
- [ ] Todos los skeletons tienen aria-busy="true" en el contenedor
- [ ] Estados empty con icono decorativo (aria-hidden) y texto descriptivo
- [ ] Responsive: botones apilados en mobile, en fila en sm+
