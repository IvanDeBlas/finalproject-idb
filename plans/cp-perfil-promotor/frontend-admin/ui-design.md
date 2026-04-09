# Diseno UI: Perfil de Promotor (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor
**Target:** src/admin
**User Story:** US-CP-01

---

## 1. Resumen

El admin (Next.js 14) es responsable de dos pantallas de esta feature: el **Dashboard de Promotor** (seccion con KPIs y card de perfil) y la **Pagina de Edicion de Perfil de Promotor**. Ambas viven dentro del layout existente `(dashboard)` con sidebar colapsable.

- Componentes shadcn instalados en uso: Card, Badge, Avatar, Separator, Button, Input, Label, Alert, AlertDialog, Skeleton, Tooltip, Dialog
- Composiciones custom: PromotorKpiGrid, PromotorProfileCard, PromotorEditForm, DesactivarPromotorDialog, PromotorInactivoBanner
- Responsive breakpoints: mobile (< 640px), tablet (640-1024px), desktop (> 1024px)
- Tema: dark (patron Dashtail, bg #0d0d1a sidebar / #151525 cards / #1a1a2e body)

---

## 2. Paleta de Colores

| Uso | Variable Tailwind / CSS | Valor Hex | Ejemplo |
|-----|------------------------|-----------|---------|
| Fondo sidebar | `bg-background` / custom | `#0d0d1a` | Sidebar, topbar |
| Fondo pagina | `bg-muted/30` | `#1a1a2e` | Main content area |
| Fondo cards | custom dark card | `#151525` | Cards y formularios |
| Fondo inputs | custom dark input | `#0f0f1f` | Inputs activos |
| Fondo input disabled | custom | `#1a1a2e` | Campo tipo promotor bloqueado |
| Primario / accent | `from-pink-500 to-purple-600` | `#ec4899 → #a855f7` | Botones CTA, badge avatar |
| Texto primario | `text-white` / `--text-primary` | `#ffffff` | Titulos, valores |
| Texto secundario | `text-muted-foreground` | `#94a3b8` | Labels, descripciones |
| Texto muted | custom | `#64748b` | Placeholders, textos terciarios |
| Texto label | custom | `#cbd5e1` | Labels de campo |
| Estado activo | `text-green-400` / `bg-green-950/50` | `#10b981` | Badge ACTIVO, icono KPI programas |
| Estado inactivo | `text-slate-400` / `bg-slate-800` | `#64748b` | Badge INACTIVO |
| Estado error | `text-destructive` / `border-red-900/50` | `#ef4444` | Zona de peligro, errores |
| Estado warning | `text-yellow-400` / `bg-yellow-950/50` | `#f59e0b` | Banner perfil inactivo |
| Borde default | custom | `#334155` | Cards, separadores, inputs |
| Borde focus | custom | `#a855f7` | Ring de focus en inputs |
| Icono KPI verde | `text-emerald-500` | `#10b981` | Programas activos |
| Icono KPI azul | `text-blue-500` | `#3b82f6` | Tareas pendientes |
| Icono KPI purple | `text-purple-500` | `#a855f7` | Wallet saldo |

---

## 3. Contexto de Layout (Dashboard Admin)

El layout base ya existe en `src/admin/src/app/(dashboard)/layout.tsx`. Todas las nuevas pantallas heredan:

```
┌──────────────────────────────────────────────────────────────────┐
│ SIDEBAR (fixed, w-[250px] o w-[70px] colapsado)                  │
│ ┌──────────────────┐  ┌────────────────────────────────────────┐ │
│ │ [Logo WePlay]    │  │ HEADER (h-16, sticky)                  │ │
│ │                  │  ├────────────────────────────────────────┤ │
│ │ [Avatar]         │  │                                        │ │
│ │ {nombrePublico}  │  │  <main className="flex-1 overflow-auto │ │
│ │ Promotor         │  │   bg-muted/30 p-6">                    │ │
│ │                  │  │                                        │ │
│ │ > Dashboard      │  │   {children}                           │ │
│ │   Mis Programas  │  │                                        │ │
│ │   Comisiones     │  │                                        │ │
│ │ * Mi Perfil      │  │                                        │ │
│ │                  │  │                                        │ │
│ │ [Logout]         │  │                                        │ │
│ └──────────────────┘  └────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
```

**Nota sobre Sidebar:** El sidebar actual en `sidebar.tsx` usa los items: Dashboard, Mis Campanias, Nueva Campania, Templates, Mi Perfil. Para la seccion de promotor se anadir una nueva entrada `Promotor` con sub-items o como item independiente. El sidebar colapsable ya implementado se reutiliza sin cambios estructurales.

---

## 4. Pantallas a Disenar

### 4.1 Seccion Promotor en Dashboard

**Ruta:** `/dashboard` (extension de la pagina existente)
**Archivo de pagina:** `src/admin/src/app/(dashboard)/dashboard/page.tsx` (ampliacion) o nueva seccion dentro

#### Layout de la Seccion

```
┌─────────────────────────────────────────────────────────────────┐
│ [main content: p-6 space-y-6]                                   │
│                                                                 │
│  [CompleteProfileBanner (existente para artista)]               │
│  [PromotorInactivoBanner (nuevo, solo si esActivo=false)]       │
│                                                                 │
│  Hola, {nombrePublico}                                          │
│  miercoles, 25 de febrero 2026        [Editar perfil ->]        │
│                                                                 │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ Seccion: Mi Perfil de Promotor (PromotorDashboardSection)  │ │
│  │                                                            │ │
│  │  ┌─────────────┐  ┌─────────────┐  ┌──────────────────┐   │ │
│  │  │ Programas   │  │  Tareas     │  │  Wallet          │   │ │
│  │  │ activos     │  │  pendientes │  │  Saldo           │   │ │
│  │  │ [icono vrd] │  │  [icono az] │  │  [icono purp]    │   │ │
│  │  │   3         │  │   12        │  │   €150.50        │   │ │
│  │  └─────────────┘  └─────────────┘  └──────────────────┘   │ │
│  │                                                            │ │
│  │  ┌─────────────────────────────────────────────────────┐  │ │
│  │  │ Mi Perfil de Promotor                               │  │ │
│  │  │                                                     │  │ │
│  │  │  [Avatar]  DJ Marketing Pro    [ACTIVO]             │  │ │
│  │  │            Influencer                               │  │ │
│  │  │  ─────────────────────────────────────────────      │  │ │
│  │  │  Email  contacto@djmarketing.com                    │  │ │
│  │  │  Web    djmarketing.com                             │  │ │
│  │  │  Redes  [ig][tt][yt][tw]                            │  │ │
│  │  │                                                     │  │ │
│  │  │  [Editar perfil]    [Ver programas]                 │  │ │
│  │  └─────────────────────────────────────────────────────┘  │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

#### Componentes de la Seccion

**PromotorInactivoBanner**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `Alert` | `border-yellow-800/50 bg-yellow-950/30 text-yellow-300` |
| Icono | `AlertCircle` lucide | `h-4 w-4 text-yellow-400` |
| Titulo | `AlertTitle` | `text-yellow-300 font-semibold` |
| Descripcion | `AlertDescription` | `text-yellow-400/80` |
| CTA link | `Button variant="link"` | `text-yellow-300 underline p-0 h-auto` |

Solo visible cuando `promotor.esActivo === false`. Oculto si `esActivo === true` o si no hay perfil.

```
[!] Tu perfil de promotor esta desactivado.
    No puedes participar en programas nuevos.
    [Ir a mi perfil para gestionar la cuenta]
```

**PromotorKpiGrid**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Grid contenedor | `div` | `grid grid-cols-1 sm:grid-cols-3 gap-4` |
| Card KPI | `Card` | `bg-[#151525] border-[#334155] hover:bg-[#1e1e38] transition-colors duration-200` |
| Card body | `CardContent` | `p-5` |
| Icono KPI | `div` con icono lucide | `w-10 h-10 rounded-lg flex items-center justify-center mb-3` |
| Icono Programas | `Activity` lucide | Container: `bg-emerald-500/15`, icono: `text-emerald-500 w-5 h-5` |
| Icono Tareas | `ClipboardList` lucide | Container: `bg-blue-500/15`, icono: `text-blue-500 w-5 h-5` |
| Icono Wallet | `Wallet` lucide | Container: `bg-purple-500/15`, icono: `text-purple-500 w-5 h-5` |
| Valor KPI | `p` | `text-3xl font-bold text-white` |
| Label KPI | `p` | `text-sm text-muted-foreground mt-1` |
| Skeleton (loading) | `Skeleton` | `h-[100px] rounded-lg` (x3) |

Formato de wallet: `€{saldo}` con dos decimales. Si `totalComisionesGanadas === 0` mostrar `€0.00` sin estilo especial.

**PromotorProfileCard**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card contenedor | `Card` | `bg-[#151525] border-[#334155]` |
| Card header | `CardHeader` | `border-b border-[#334155] pb-4 flex flex-row items-center justify-between` |
| Titulo de card | `CardTitle` | `text-lg font-semibold text-white` |
| Avatar | `Avatar` | `w-16 h-16` |
| Avatar fallback | `AvatarFallback` | `bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold` iniciales del nombre |
| Nombre publico | `p` | `text-xl font-semibold text-white mt-3` |
| Badge activo | `Badge` | `bg-green-950/50 text-green-400 border border-green-800/50 ml-2` |
| Badge inactivo | `Badge` | `bg-slate-800 text-slate-400 border border-slate-700 ml-2` |
| Tipo promotor | `p` | `text-sm text-muted-foreground mt-1` |
| Separador | `Separator` | `bg-[#334155] my-4` |
| Label de campo (Email, Web) | `span` | `text-xs text-[#64748b] uppercase tracking-wider block mb-1` |
| Valor de campo | `p` o `span` | `text-sm text-[#cbd5e1]` |
| Valor ausente | `span` | `text-sm text-muted-foreground italic` con texto `--` |
| Enlace URL web | `a` | `text-sm text-purple-400 hover:underline truncate max-w-[200px] inline-block` |
| Seccion redes | `div` | `flex items-center gap-2 flex-wrap mt-2` |
| Icono red social | `a` | `w-8 h-8 flex items-center justify-center rounded-md bg-[#1e1e38] text-muted-foreground hover:text-white hover:bg-[#334155] transition-colors duration-150` con `aria-label="Ver perfil en {red}"` |
| Footer card | `CardFooter` | `pt-0 flex gap-3 flex-wrap` |
| Boton editar | `Button variant="outline"` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white` |
| Boton ver programas | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold` - `disabled` si `esActivo === false` |
| Skeleton header | `Skeleton` | `h-6 w-40` |
| Skeleton nombre | `Skeleton` | `h-7 w-48 mt-3` |
| Skeleton badge | `Skeleton` | `h-5 w-16 ml-2` |
| Skeleton campos | `Skeleton` | `h-4 w-full` (x2) |
| Skeleton botones | `Skeleton` | `h-9 w-28` (x2) |

**Composicion PromotorProfileCard:**
```tsx
<Card className="bg-[#151525] border-[#334155]">
    <CardHeader className="border-b border-[#334155] pb-4 flex flex-row items-center justify-between">
        <CardTitle className="text-lg font-semibold text-white">
            Mi Perfil de Promotor
        </CardTitle>
    </CardHeader>
    <CardContent className="pt-6">
        <div className="flex flex-col sm:flex-row sm:items-start gap-4">
            <Avatar className="w-16 h-16 shrink-0">
                <AvatarFallback className="bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold">
                    {initials}
                </AvatarFallback>
            </Avatar>
            <div className="flex-1">
                <div className="flex items-center gap-2 flex-wrap">
                    <p className="text-xl font-semibold text-white">{nombrePublico}</p>
                    <Badge className={esActivo ? "bg-green-950/50 text-green-400 border border-green-800/50" : "bg-slate-800 text-slate-400 border border-slate-700"}>
                        {esActivo ? "ACTIVO" : "INACTIVO"}
                    </Badge>
                </div>
                <p className="text-sm text-muted-foreground mt-1">{tipoPromotorNombre}</p>
            </div>
        </div>
        <Separator className="bg-[#334155] my-4" />
        {/* Email y Web */}
        {/* Iconos de redes sociales */}
    </CardContent>
    <CardFooter className="pt-0 flex gap-3 flex-wrap">
        <Button variant="outline" asChild>
            <Link href="/promotor/perfil">Editar perfil</Link>
        </Button>
        <Button
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
            disabled={!esActivo}
        >
            Ver programas
        </Button>
    </CardFooter>
</Card>
```

**Empty State (sin perfil de promotor):**

Cuando el usuario no tiene perfil de promotor aun (GET /me retorna 404), mostrar un banner informativo similar al `CompleteProfileBanner` existente pero para el rol promotor. Se puede colocar en la seccion del dashboard o como un banner separado:

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `Card` | `bg-gradient-to-r from-purple-500/10 to-pink-600/10 border-purple-500/50` |
| Icono | `Megaphone` lucide | `h-5 w-5 text-purple-500 mt-0.5 shrink-0` |
| Titulo | `h3` | `font-semibold text-white` |
| Descripcion | `p` | `text-sm text-muted-foreground mt-1` |
| CTA | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Dismiss | `Button variant="ghost" size="icon"` | `text-muted-foreground hover:text-white` |

---

### 4.2 Pagina de Perfil Promotor (Edicion)

**Ruta:** `/promotor/perfil`
**Archivo:** `src/admin/src/app/(dashboard)/promotor/perfil/page.tsx` (nuevo)

#### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ SIDEBAR (existente, Mi Perfil marcado activo)                   │
│ ┌──────────────────┐  ┌───────────────────────────────────────┐ │
│ │ (sidebar)        │  │ HEADER (existente)                    │ │
│ │ * Mi Perfil [->] │  ├───────────────────────────────────────┤ │
│ └──────────────────┘  │ <main bg-muted/30 p-6>               │ │
│                       │                                       │ │
│                       │  Editar Perfil de Promotor            │ │
│                       │  Actualiza tu informacion publica     │ │
│                       │                                       │ │
│                       │  max-w-2xl mx-auto space-y-6          │ │
│                       │                                       │ │
│                       │  ┌─────────────────────────────────┐  │ │
│                       │  │ [Card] Informacion publica       │  │ │
│                       │  │  nombre publico (editable)       │  │ │
│                       │  │  tipo promotor (disabled+lock)   │  │ │
│                       │  │  email contacto (editable)       │  │ │
│                       │  │  sitio web (editable)            │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  ┌─────────────────────────────────┐  │ │
│                       │  │ [Card] Redes sociales            │  │ │
│                       │  │  [ig] Instagram [___________]   │  │ │
│                       │  │  [tt] TikTok    [___________]   │  │ │
│                       │  │  [yt] YouTube   [___________]   │  │ │
│                       │  │  [tw] Twitter/X [___________]   │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  ┌─────────────────────────────────┐  │ │
│                       │  │ [Card] Zona de peligro (rojo)    │  │ │
│                       │  │  [!] Desactivar cuenta           │  │ │
│                       │  │  [Desactivar cuenta de promotor] │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  [Cancelar]    [Guardar cambios ->]   │ │
│                       └───────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

#### Componentes de la Pagina de Edicion

**PageHeader (no es componente shadcn, es estructura de pagina)**

| Elemento | Componente | Customizacion |
|----------|------------|---------------|
| Titulo de pagina | `h1` | `text-3xl font-bold text-white` |
| Subtitulo | `p` | `text-muted-foreground mt-1` |

**Card: Informacion Publica**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card | `Card` | `bg-[#151525] border-[#334155]` |
| Header | `CardHeader` | `border-b border-[#334155] pb-4` |
| Titulo | `CardTitle` | `text-lg font-semibold text-white` |
| Contenido | `CardContent` | `pt-6 space-y-5` |
| Label requerido | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` con asterisco `*` en `text-red-500` |
| Label opcional | `Label` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Input nombre publico | `Input` | `bg-[#0f0f1f] border-[#334155] text-white h-11 focus:border-[#a855f7] placeholder:text-[#64748b]` |
| Wrapper tipo promotor | `div` | `relative` |
| Input tipo (disabled) | `Input` | `bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed h-11 pr-10` con `disabled` y `aria-disabled="true"` |
| Icono lock | `Lock` lucide | `absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b] pointer-events-none` |
| Nota bajo tipo | `p` | `text-xs text-[#64748b] mt-1` |
| Input email | `Input type="email"` | Igual que nombre publico, pre-rellenado |
| Input sitio web | `Input type="url"` | Igual que nombre publico, placeholder `https://tusitio.com` |
| Error de campo | `p role="alert"` | `text-xs text-destructive mt-1 flex items-center gap-1` con `AlertCircle w-3 h-3` |
| Skeleton campos | `Skeleton` | `h-11 w-full rounded-md` (x4) |

**Card: Redes Sociales**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card | `Card` | `bg-[#151525] border-[#334155]` |
| Header | `CardHeader` | `border-b border-[#334155] pb-4` |
| Titulo | `CardTitle` | `text-lg font-semibold text-white` |
| Contenido | `CardContent` | `pt-6 space-y-4` |
| Label con icono | `Label` | `flex items-center gap-2 text-sm font-medium text-[#cbd5e1] mb-1.5` |
| Icono Instagram | SVG / `Instagram` (si disponible) | `w-4 h-4 text-pink-400` |
| Icono TikTok | SVG custom | `w-4 h-4 text-slate-300` |
| Icono YouTube | SVG / lucide | `w-4 h-4 text-red-400` |
| Icono Twitter/X | SVG custom | `w-4 h-4 text-sky-400` |
| Input URL red | `Input type="url"` | `bg-[#0f0f1f] border-[#334155] text-white h-10 focus:border-[#a855f7] placeholder:text-[#64748b]` |
| Error de URL | `p role="alert"` | `text-xs text-destructive mt-1 flex items-center gap-1` |

**Composicion Card Redes Sociales:**
```tsx
<Card className="bg-[#151525] border-[#334155]">
    <CardHeader className="border-b border-[#334155] pb-4">
        <CardTitle className="text-lg font-semibold text-white">
            Redes sociales
        </CardTitle>
    </CardHeader>
    <CardContent className="pt-6 space-y-4">
        {redesSociales.map(({ id, label, icon: Icon, iconColor, placeholder }) => (
            <div key={id} className="space-y-1.5">
                <Label htmlFor={id} className="flex items-center gap-2 text-sm font-medium text-[#cbd5e1]">
                    <Icon className={cn("w-4 h-4", iconColor)} aria-hidden="true" />
                    {label}
                </Label>
                <Input
                    id={id}
                    type="url"
                    placeholder={placeholder}
                    aria-invalid={!!errors[id]}
                    aria-describedby={errors[id] ? `${id}-error` : undefined}
                    {...register(id)}
                />
                {errors[id] && (
                    <p id={`${id}-error`} role="alert" className="text-xs text-destructive mt-1 flex items-center gap-1">
                        <AlertCircle className="w-3 h-3" aria-hidden="true" />
                        {errors[id].message}
                    </p>
                )}
            </div>
        ))}
    </CardContent>
</Card>
```

**Card: Zona de Peligro**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card | `Card` | `bg-[#151525] border border-red-900/50` |
| Header | `CardHeader` | `border-b border-red-900/50 pb-4` |
| Titulo | `CardTitle` | `text-base font-semibold text-red-400 flex items-center gap-2` con `AlertTriangle w-4 h-4` |
| Contenido | `CardContent` | `pt-6` |
| Descripcion | `p` | `text-sm text-muted-foreground mb-4` |
| Boton desactivar | `Button variant="outline"` | `border-red-800 text-red-400 hover:bg-red-950/50 hover:text-red-300 hover:border-red-700` |

Ocultar esta card si `promotor.esActivo === false` (ya esta desactivado).

**Composicion Card Zona de Peligro:**
```tsx
<Card className="bg-[#151525] border border-red-900/50">
    <CardHeader className="border-b border-red-900/50 pb-4">
        <CardTitle className="text-base font-semibold text-red-400 flex items-center gap-2">
            <AlertTriangle className="w-4 h-4" aria-hidden="true" />
            Zona de peligro
        </CardTitle>
    </CardHeader>
    <CardContent className="pt-6">
        <p className="text-sm text-muted-foreground mb-4">
            Al desactivar tu cuenta de promotor, seras dado de baja automaticamente
            de todos los programas activos en los que participas.
        </p>
        <Button
            variant="outline"
            className="border-red-800 text-red-400 hover:bg-red-950/50 hover:text-red-300 hover:border-red-700"
            onClick={onDesactivarClick}
            type="button"
        >
            Desactivar cuenta de promotor
        </Button>
    </CardContent>
</Card>
```

**Footer de Acciones (Botones principales)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Contenedor | `div` | `flex items-center justify-end gap-3 pt-4 border-t border-[#334155]` |
| Boton Cancelar | `Button variant="ghost"` | `text-muted-foreground hover:text-white hover:bg-[#1e1e38]` |
| Boton Guardar | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11` |
| Spinner en Guardar | `Loader2` lucide | `w-4 h-4 animate-spin mr-2` solo en estado `isPending` |

El boton Guardar usa `disabled={!isDirty || isSubmitting}` de react-hook-form para controlar el dirty state.

---

### 4.3 Dialogo de Desactivacion

**Tipo:** AlertDialog (bloquea interaccion con el fondo hasta confirmar o cancelar)
**Trigger:** Boton "Desactivar cuenta de promotor" en la Zona de Peligro

#### Layout del Dialogo

```
┌──────────────────────────────────────────┐
│  [!] Desactivar cuenta de promotor       │
│ ──────────────────────────────────────── │
│                                          │
│  [AlertTriangle icon - CASO A]           │
│  Estas participando en {n} programas     │
│  activos. Al desactivar seras dado de    │
│  baja de todos ellos automaticamente.   │
│                                          │
│  [sin icono - CASO B]                    │
│  ¿Seguro que quieres desactivar tu       │
│  cuenta de promotor? Esta accion         │
│  desactivara tu perfil pero no lo        │
│  eliminara.                              │
│  ─────────────────────────────────       │
│  [Cancelar]    [Desactivar cuenta]       │
└──────────────────────────────────────────┘
```

#### Componentes del Dialogo

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Dialogo | `AlertDialog` | Controlado con `open` + `onOpenChange` |
| Overlay | `AlertDialogOverlay` | `bg-black/60 backdrop-blur-sm` |
| Contenido | `AlertDialogContent` | `bg-[#151525] border border-[#334155] max-w-md` |
| Header | `AlertDialogHeader` | Por defecto |
| Icono advertencia (caso A) | `AlertTriangle` lucide | `w-10 h-10 text-red-400 mb-3 mx-auto` (solo si `programasActivos > 0`) |
| Titulo | `AlertDialogTitle` | `text-lg font-semibold text-white` |
| Descripcion | `AlertDialogDescription` | `text-sm text-muted-foreground mt-2` |
| Numero de programas | `strong` dentro de descripcion | `text-white font-semibold` |
| Separador | `Separator` | `bg-[#334155] my-4` |
| Footer | `AlertDialogFooter` | Por defecto |
| Boton Cancelar | `AlertDialogCancel` | `border-[#334155] text-muted-foreground hover:bg-[#1e1e38] hover:text-white` - `disabled={isPending}` |
| Boton Confirmar | `AlertDialogAction` | `bg-red-600 hover:bg-red-700 text-white font-semibold` |
| Spinner confirmar | `Loader2` lucide | `w-4 h-4 animate-spin mr-2` solo en `isPending` |
| Skeleton descripcion | `Skeleton` | `h-4 w-full` + `h-4 w-3/4 mt-2` mientras carga `totalProgramasActivos` |

**Composicion del Dialogo:**
```tsx
<AlertDialog open={open} onOpenChange={onOpenChange}>
    <AlertDialogContent className="bg-[#151525] border border-[#334155] max-w-md">
        <AlertDialogHeader>
            {programasActivos > 0 && (
                <AlertTriangle
                    className="w-10 h-10 text-red-400 mb-3 mx-auto"
                    aria-hidden="true"
                />
            )}
            <AlertDialogTitle className="text-lg font-semibold text-white">
                Desactivar cuenta de promotor
            </AlertDialogTitle>
            <AlertDialogDescription className="text-sm text-muted-foreground mt-2">
                {isLoadingProgramas ? (
                    <span aria-busy="true">
                        <Skeleton className="h-4 w-full" />
                        <Skeleton className="h-4 w-3/4 mt-2" />
                    </span>
                ) : programasActivos > 0 ? (
                    <>
                        Estas participando en{" "}
                        <strong className="text-white font-semibold">
                            {programasActivos} {programasActivos === 1 ? "programa activo" : "programas activos"}
                        </strong>
                        . Al desactivar tu cuenta seras dado de baja de todos ellos automaticamente.
                    </>
                ) : (
                    <>
                        ¿Seguro que quieres desactivar tu cuenta de promotor? Esta accion
                        desactivara tu perfil pero no lo eliminara. Podras contactar con soporte
                        para reactivarlo.
                    </>
                )}
            </AlertDialogDescription>
        </AlertDialogHeader>
        <Separator className="bg-[#334155] my-4" />
        <AlertDialogFooter>
            <AlertDialogCancel
                disabled={isPending}
                className="border-[#334155] text-muted-foreground hover:bg-[#1e1e38] hover:text-white"
            >
                Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
                onClick={onConfirm}
                disabled={isPending || isLoadingProgramas}
                className="bg-red-600 hover:bg-red-700 text-white font-semibold"
            >
                {isPending && <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />}
                {isPending ? "Desactivando..." : "Desactivar cuenta"}
            </AlertDialogAction>
        </AlertDialogFooter>
    </AlertDialogContent>
</AlertDialog>
```

---

## 5. Formulario de Edicion de Promotor

### 5.1 PromotorEditForm

**Schema:** `updatePromotorSchema` de `src/shared/schemas/crowdpromotion.schema.ts` (reutilizado del shared)

**Campos:**

| Campo | Componente | Tipo | Validacion Visual |
|-------|------------|------|-------------------|
| `nombrePublico` | `Input` | text | `FormMessage` con `AlertCircle` - requerido, min 3, max 200 |
| `tipoPromotorNombre` | `Input disabled` + `Lock` icon | text (readonly) | Sin validacion - campo no editable |
| `emailContacto` | `Input type="email"` | email (opcional) | `FormMessage` - formato email |
| `urlSitioWeb` | `Input type="url"` | url (opcional) | `FormMessage` - formato URL |
| `urlInstagram` | `Input type="url"` | url (opcional) | `FormMessage` - formato URL |
| `urlTikTok` | `Input type="url"` | url (opcional) | `FormMessage` - formato URL |
| `urlYouTube` | `Input type="url"` | url (opcional) | `FormMessage` - formato URL |
| `urlTwitter` | `Input type="url"` | url (opcional) | `FormMessage` - formato URL |

**Layout del formulario:**
- `form` con `space-y-6` (separacion entre cards)
- Dentro de cada card: `space-y-5` entre campos
- Labels sobre el input (`flex-col`)
- Errores debajo del input con animacion fade-in

**Estados de campos:**

| Estado | Visual del Input |
|--------|-----------------|
| Default | `bg-[#0f0f1f] border-[#334155] text-white` |
| Focus | `border-[#a855f7] ring-2 ring-[#a855f7]/15` - transicion 200ms |
| Error | `border-destructive ring-2 ring-destructive/15` + mensaje debajo |
| Disabled (tipo promotor) | `bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed` |
| Loading (skeleton) | `Skeleton h-11 w-full rounded-md animate-pulse` |
| Submitting | `disabled opacity-50` en todos los inputs editables |

**Deteccion de dirty state:**
- Usar `formState.isDirty` de react-hook-form
- Boton "Guardar cambios": `disabled={!isDirty || isSubmitting}`
- Al hacer click en Cancelar con `isDirty === true`: mostrar `AlertDialog` de confirmacion de descarte

**AlertDialog de descarte de cambios:**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Dialogo | `AlertDialog` | `max-w-sm` |
| Titulo | `AlertDialogTitle` | `text-lg font-semibold` |
| Descripcion | `AlertDialogDescription` | `text-sm text-muted-foreground` |
| Accion continuar | `AlertDialogCancel` | Texto "Continuar editando" |
| Accion salir | `AlertDialogAction` | Texto "Salir sin guardar" con `variant="destructive"` |

---

## 6. Estados Globales de las Pantallas

### 6.1 Dashboard - Seccion Promotor

| Estado | Componentes afectados | Visual |
|--------|-----------------------|--------|
| **Loading inicial** | `PromotorKpiGrid`, `PromotorProfileCard` | `Skeleton` en cada card. `aria-busy="true"` en contenedores. Texto SR: "Cargando informacion del promotor..." |
| **Sin perfil (404)** | Toda la seccion promotor | `CompletePromotorBanner` (banner de invitacion a crear perfil) |
| **Perfil activo** | `PromotorProfileCard`, `Badge` | Badge verde "ACTIVO". Boton "Ver programas" habilitado |
| **Perfil inactivo** | `PromotorInactivoBanner`, `PromotorProfileCard`, `Badge` | Banner amarillo superior. Badge gris "INACTIVO". Boton "Ver programas" `disabled` |
| **Error de carga** | Toda la seccion promotor | `Alert variant="destructive"` con boton "Reintentar" que llama a `refetch()` |
| **Wallet en cero** | `PromotorKpiGrid` KPI Wallet | Mostrar `€0.00` sin estilos especiales |
| **Sin redes sociales** | `PromotorProfileCard` seccion redes | `p` con texto "Sin redes configuradas" en `text-muted-foreground italic` |

### 6.2 Edicion de Perfil

| Estado | Componentes afectados | Visual |
|--------|-----------------------|--------|
| **Loading inicial** | Todos los `Input`, titulos de card | `Skeleton` en lugar de los campos. Botones `disabled` |
| **Pre-rellenado** | Todos los `Input` | Campos con valores actuales. `isDirty === false`. Boton Guardar `disabled` |
| **Con cambios** | Boton Guardar | `isDirty === true`. Boton Guardar habilitado con estilos gradient |
| **Submitting** | Todos los inputs, boton Guardar | Inputs `disabled`. Boton muestra `Loader2 animate-spin` + "Guardando..." |
| **Success** | Boton Guardar, inputs | Toast verde via `sonner`. `isDirty` se resetea a `false`. Boton vuelve a `disabled` |
| **Error API** | Toast | Toast rojo via `sonner`. Inputs re-habilitados. Datos intactos |
| **Error validacion** | Input afectado | Border rojo. Mensaje bajo el campo. Boton Guardar `disabled` si `!isValid` |

---

## 7. Feedback y Notificaciones

### 7.1 Toast (via sonner)

Todos los toasts usan `sonner` ya instalado en el admin.

| Accion | Toast | Tipo |
|--------|-------|------|
| Perfil actualizado | "Perfil actualizado correctamente" | Success (`toast.success`) |
| Error al guardar | "Error al guardar el perfil" + detalle | Error (`toast.error`) |
| Perfil desactivado | "Tu cuenta de promotor ha sido desactivada" | Success (`toast.success`) |
| Error al desactivar | "No se pudo desactivar la cuenta" + detalle | Error (`toast.error`) |
| Error de validacion global | "Revisa los campos del formulario" | Error (`toast.error`) |

### 7.2 Alerts inline

| Situacion | Componente | Variante |
|-----------|------------|---------|
| Perfil inactivo (banner dashboard) | `Alert` | Custom amarillo (`border-yellow-800/50 bg-yellow-950/30`) |
| Error de carga de perfil | `Alert` + `AlertTitle` + `AlertDescription` | `variant="destructive"` |
| Info: tipo promotor no editable | `p` bajo el campo | Sin componente Alert, texto simple en `text-xs text-[#64748b]` |

### 7.3 Empty State

| Pantalla | Condicion | Visual |
|----------|-----------|--------|
| Dashboard - seccion promotor | Usuario sin perfil de promotor | `Card` con gradient border, icono `Megaphone`, texto y CTA "Convertirme en Promotor" |
| Dashboard - redes sociales | Sin URLs configuradas | Texto inline "Sin redes configuradas" en muted italic |
| Dashboard - error de carga | Error en query | `Alert destructive` con boton "Reintentar" |

---

## 8. Responsive Design

### 8.1 Dashboard - Seccion Promotor

| Breakpoint | `PromotorKpiGrid` | `PromotorProfileCard` | Sidebar |
|------------|-------------------|-----------------------|---------|
| `< sm (< 640px)` | `grid-cols-1` (apilado) | `flex-col` Avatar centrado arriba, datos abajo. Botones full width | Oculto, hamburguer en header |
| `sm - lg (640-1024px)` | `grid-cols-2` (con tercer KPI en fila nueva o full width) o `sm:grid-cols-3` si caben | Avatar a la izquierda, datos inline | Colapsado a iconos (`w-[70px]`) |
| `> lg (> 1024px)` | `grid-cols-3` | Avatar izquierda, datos derecha en fila horizontal | Expandido (`w-[250px]`) |

```tsx
/* KPI Grid */
className="grid grid-cols-1 sm:grid-cols-3 gap-4"

/* Profile Card content */
className="flex flex-col sm:flex-row sm:items-start gap-4"
```

### 8.2 Edicion de Perfil

| Breakpoint | Layout de Cards | Botones de accion |
|------------|-----------------|-------------------|
| `< sm (< 640px)` | Cards full width, `p-4`. Zona peligro al final del scroll. Campos redes en stack vertical (label arriba, input abajo) | Sticky footer `fixed bottom-0 w-full bg-[#0d0d1a] border-t border-[#334155] p-4 flex gap-3` |
| `sm - lg (640-1024px)` | Cards con padding normal `p-6`. Sin sticky footer | `flex justify-end gap-3 pt-4 border-t border-[#334155]` |
| `> lg (> 1024px)` | `max-w-2xl mx-auto` para centrar. Cards con `p-6` | `flex justify-end gap-3 pt-4 border-t border-[#334155]` |

```tsx
/* Contenedor pagina edicion */
className="max-w-2xl mx-auto space-y-6"

/* Botones (tablet/desktop) */
className="flex items-center justify-end gap-3 pt-4 border-t border-[#334155]"

/* Botones (mobile sticky) */
className="fixed bottom-0 left-0 right-0 z-10 bg-[#0d0d1a] border-t border-[#334155] p-4 flex gap-3 lg:relative lg:bottom-auto lg:bg-transparent lg:border-0 lg:p-0 lg:justify-end"
```

---

## 9. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Labels asociados | `Label htmlFor` + `Input id` en todos los campos |
| Campos requeridos | `aria-required="true"` en `nombrePublico`. Asterisco `*` visible + `span.sr-only` "(requerido)" |
| Errores de campo | `role="alert"` + `aria-live="polite"` en `p` de error. `aria-invalid="true"` en input. `aria-describedby` apuntando al `id` del error |
| Campo tipo (solo lectura) | `disabled` + `aria-disabled="true"` + nota visible "no puede modificarse" |
| Focus visible | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#151525]` en todos los interactivos |
| Contraste | Blanco `#ffffff` sobre `#151525` = ratio 16:1 (supera WCAG AA 4.5:1). `#94a3b8` sobre `#151525` = ratio 4.8:1 |
| Iconos de redes | `aria-label="Ver perfil en {red}"` en links. Iconos SVG con `aria-hidden="true"` |
| Botones con estado loading | `aria-busy="true"` durante `isPending` |
| Botones deshabilitados | `aria-disabled="true"` en boton Guardar cuando `!isDirty` |
| Dialogo de desactivacion | `AlertDialog` de shadcn/ui implementa WAI-ARIA `alertdialog`. Focus atrapado. Esc cierra |
| Dialogo de descarte | `AlertDialog` idem. Foco en "Continuar editando" por defecto |
| Navegacion sidebar | `nav aria-label="Menu principal"`. Item activo: `aria-current="page"` |
| Loading states | `aria-busy="true"` en contenedores con skeleton. `aria-live="polite"` para anuncios SR |
| Color no es unico indicador | Badge ACTIVO: `CheckCircle` icono (opcional) + color verde + texto "ACTIVO". Errores: `AlertCircle` + color rojo + texto |
| Skip link (recomendado) | `<a href="#main-content" className="sr-only focus:not-sr-only">Ir al contenido principal</a>` en layout |

### Ejemplo ARIA en campo de edicion:
```tsx
<div className="space-y-1.5">
    <Label htmlFor="nombrePublico" className="text-sm font-medium text-[#cbd5e1] block">
        Nombre publico{" "}
        <span aria-hidden="true" className="text-red-500 ml-1">*</span>
        <span className="sr-only">(requerido)</span>
    </Label>
    <Input
        id="nombrePublico"
        aria-required="true"
        aria-invalid={!!errors.nombrePublico}
        aria-describedby={errors.nombrePublico ? "nombrePublico-error" : undefined}
        className="bg-[#0f0f1f] border-[#334155] text-white h-11"
        {...register("nombrePublico")}
    />
    {errors.nombrePublico && (
        <p
            id="nombrePublico-error"
            role="alert"
            aria-live="polite"
            className="text-xs text-destructive mt-1 flex items-center gap-1"
        >
            <AlertCircle className="w-3 h-3" aria-hidden="true" />
            {errors.nombrePublico.message}
        </p>
    )}
</div>
```

---

## 10. Estructura de Archivos Propuesta

```
src/admin/src/
└── app/
    └── (dashboard)/
        ├── dashboard/
        │   ├── page.tsx                           (existente - anadir PromotorSection)
        │   └── components/
        │       ├── PromotorDashboardSection.tsx   (nuevo - orquesta KPI + ProfileCard)
        │       ├── PromotorKpiGrid.tsx             (nuevo - 3 KPI cards)
        │       ├── PromotorProfileCard.tsx         (nuevo - card de perfil)
        │       └── PromotorInactivoBanner.tsx      (nuevo - banner amarillo)
        └── promotor/
            └── perfil/
                ├── page.tsx                        (nuevo - pagina de edicion)
                └── components/
                    ├── PromotorEditForm.tsx         (nuevo - formulario de edicion)
                    ├── InformacionPublicaCard.tsx   (nuevo - card info publica)
                    ├── RedesSocialesCard.tsx        (nuevo - card redes)
                    ├── ZonaPeligroCard.tsx          (nuevo - card danger zone)
                    └── DesactivarPromotorDialog.tsx (nuevo - AlertDialog desactivacion)
```

**Nota sobre sidebar:** Anadir entrada de navegacion `"Mi Perfil Promotor"` o `"Promotor"` en `sidebar.tsx` apuntando a `/promotor/perfil` para que aparezca como activo al editar. El item actual "Mi Perfil" en `/perfil` es para el perfil de artista.

---

## 11. Inventario Completo de Componentes shadcn/ui

| Componente shadcn | Pantallas que lo usan | Estado |
|-------------------|-----------------------|--------|
| `Card`, `CardHeader`, `CardContent`, `CardFooter`, `CardTitle` | Dashboard (KPI, Perfil), Edicion (3 cards), Banners | Instalado |
| `Badge` | Dashboard (estado ACTIVO/INACTIVO) | Instalado |
| `Avatar`, `AvatarFallback` | Dashboard (ProfileCard) | Instalado |
| `Separator` | Dashboard (ProfileCard), Dialogo desactivacion | Instalado |
| `Button` | Todas las pantallas | Instalado |
| `Input` | Edicion (8 campos) | Instalado |
| `Label` | Edicion (8 campos) | Instalado |
| `Alert`, `AlertTitle`, `AlertDescription` | Banner perfil inactivo, Error de carga | Instalado |
| `AlertDialog`, `AlertDialogContent`, `AlertDialogHeader`, `AlertDialogTitle`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogCancel`, `AlertDialogAction` | Dialogo desactivacion, Dialogo descarte cambios | Instalado |
| `Skeleton` | Loading states (KPI, ProfileCard, campos) | Instalado |
| `Tooltip` | Icono Lock en campo tipo (tooltip "No editable") | Instalado |

**Total componentes shadcn distintos:** 11 componentes (con sus sub-componentes)
**Composiciones custom:** 7 (PromotorKpiGrid, PromotorProfileCard, PromotorInactivoBanner, InformacionPublicaCard, RedesSocialesCard, ZonaPeligroCard, DesactivarPromotorDialog)

---

## 12. Checklist de Diseno UI

### Dashboard - Seccion Promotor

- [ ] `PromotorInactivoBanner` solo visible cuando `esActivo === false`
- [ ] `PromotorKpiGrid` con 3 cards: programas, tareas, wallet
- [ ] Iconos KPI con colores distintos: verde (programas), azul (tareas), purple (wallet)
- [ ] Skeleton loading en KPI cards y ProfileCard
- [ ] `PromotorProfileCard` con `Avatar` (iniciales), nombre, Badge estado
- [ ] Badge ACTIVO verde / INACTIVO gris
- [ ] Links de redes con `target="_blank" rel="noopener noreferrer"` y `aria-label`
- [ ] Campos sin dato muestran "--" en muted italic
- [ ] Boton "Ver programas" `disabled` si perfil inactivo
- [ ] Error state con boton "Reintentar" usando `Alert variant="destructive"`
- [ ] Empty state si usuario no tiene perfil (banner de invitacion)
- [ ] Responsive: `grid-cols-1` en mobile, `grid-cols-3` en desktop (KPIs)
- [ ] Responsive: Profile card `flex-col` en mobile, `flex-row` en sm+

### Edicion de Perfil

- [ ] Todos los campos pre-rellenados desde `GET /api/crowdpromotion/promotor/me`
- [ ] Campo `tipoPromotorNombre` claramente deshabilitado con icono `Lock`
- [ ] Nota explicativa "no puede modificarse" bajo el campo tipo
- [ ] Validacion Zod con `updatePromotorSchema` de shared
- [ ] `isDirty` controla el estado del boton "Guardar cambios"
- [ ] Skeleton loading en todos los campos mientras carga
- [ ] Card Zona de Peligro con border rojo y boton de desactivacion
- [ ] Card Zona de Peligro oculta si `esActivo === false`
- [ ] Toast success via `sonner` tras guardado exitoso
- [ ] Toast error via `sonner` si falla la API
- [ ] Boton Cancelar abre `AlertDialog` de descarte si `isDirty === true`
- [ ] Botones sticky en mobile (fixed bottom)
- [ ] Botones inline en tablet/desktop (al final del form)
- [ ] `max-w-2xl mx-auto` para centrar el formulario en desktop

### Dialogo de Desactivacion

- [ ] `AlertDialog` (no Dialog) para bloquear la UI
- [ ] Loading de `totalProgramasActivos` con Skeleton en descripcion
- [ ] Icono `AlertTriangle` solo si `programasActivos > 0`
- [ ] Texto dinamico segun numero de programas
- [ ] Numero de programas en `strong text-white`
- [ ] `Separator` entre descripcion y footer
- [ ] Boton Confirmar con `Loader2` + "Desactivando..." durante mutation
- [ ] Boton Cancelar `disabled` durante `isPending`
- [ ] Toast success tras desactivacion + redirect a `/dashboard`
- [ ] Toast error si falla la API

### Accesibilidad General

- [ ] Todos los inputs con `Label` asociado via `htmlFor`/`id`
- [ ] `aria-required="true"` en campo `nombrePublico`
- [ ] `aria-invalid` + `aria-describedby` en campos con error
- [ ] `role="alert"` + `aria-live="polite"` en mensajes de error
- [ ] `aria-disabled="true"` en campo tipo promotor
- [ ] `aria-busy="true"` en botones y contenedores loading
- [ ] Focus ring visible en todos los interactivos
- [ ] Contraste WCAG AA verificado
- [ ] Iconos de redes con `aria-label` y SVG `aria-hidden`
- [ ] `aria-current="page"` en item activo del sidebar
