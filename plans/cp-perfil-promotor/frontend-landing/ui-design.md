# Diseno UI: Perfil de Promotor (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

| Metrica | Valor |
|---------|-------|
| Componentes shadcn/ui utilizados | 11 |
| Composiciones custom nuevas | 7 |
| Pantallas | 3 (/promotor/registro, /promotor/dashboard, /promotor/perfil) |
| Responsive breakpoints | sm (640px), md (768px), lg (1024px) |
| Tema | Dark (colores hardcodeados alineados al sistema de diseno del proyecto) |

**Componentes shadcn/ui instalados y disponibles (verificados en src/web/src/components/ui/):**
- `Card`, `CardHeader`, `CardContent`, `CardFooter`, `CardTitle`, `CardDescription`
- `Input`
- `Label`
- `Button`
- `Select`, `SelectTrigger`, `SelectContent`, `SelectItem`, `SelectValue`
- `Badge`
- `Avatar`, `AvatarFallback`
- `Skeleton`
- `Separator`
- `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`
- `Alert`, `AlertTitle`, `AlertDescription`
- `Progress`
- `Tooltip`

**Nota sobre `Form`:** El proyecto usa el patron nativo de react-hook-form con `Label` + `Input` directos (sin el wrapper `Form`/`FormField` de shadcn). Seguir el patron existente en `RegisterPage.tsx` y `ArtistaPerfilPage.tsx`.

**Nota sobre `AlertDialog`:** No esta instalado. Usar `Dialog` existente para el dialogo de confirmacion de desactivacion.

---

## 2. Paleta de Colores

Extraida de `docs/user-stories/cp-perfil-promotor/ui-ux.md` (tokens del proyecto).

| Uso | Valor hex | Tailwind equivalente | Ejemplo |
|-----|-----------|---------------------|---------|
| Fondo pagina | `#1a1a2e` | `bg-[#1a1a2e]` | Fondo de paginas |
| Fondo card / form | `#151525` | `bg-[#151525]` | Cards, formularios |
| Fondo sidebar | `#0d0d1a` | `bg-[#0d0d1a]` | Sidebar del dashboard |
| Fondo input | `#0f0f1f` | `bg-[#0f0f1f]` | Inputs de formulario |
| Fondo input readonly | `#1a1a2e` | `bg-[#1a1a2e]` | Input tipo promotor deshabilitado |
| Borde default | `#334155` | `border-[#334155]` | Bordes de cards e inputs |
| Borde focus | `#a855f7` | `focus:border-[#a855f7]` | Input enfocado |
| Borde readonly | `#1e2a42` | `border-[#1e2a42]` | Campo tipo promotor |
| Borde error | `#ef4444` | `border-red-500` | Input con error |
| Texto primario | `#ffffff` | `text-white` | Titulos, valores |
| Texto secundario | `#94a3b8` | `text-[#94a3b8]` | Labels, placeholders |
| Texto muted | `#64748b` | `text-[#64748b]` | Hints, iconos secundarios |
| Texto label | `#cbd5e1` | `text-[#cbd5e1]` | Labels de campos |
| Acento / link | `#a855f7` | `text-[#a855f7]` | Links, borde focus |
| Estado activo | `#10b981` | `text-[#10b981]` | Badge ACTIVO |
| Estado inactivo | `#64748b` | `text-slate-500` | Badge INACTIVO |
| Advertencia | `#f59e0b` | `text-amber-400` | Banners de aviso |
| Error | `#ef4444` | `text-red-400` | Errores |
| Gradiente primario | `from-pink-500 to-purple-600` | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones principales |
| KPI icon verde | `rgba(16,185,129,0.15)` | `bg-emerald-950/40` | Fondo icono KPI |
| KPI icon azul | `rgba(59,130,246,0.15)` | `bg-blue-950/40` | Fondo icono KPI |
| KPI icon purple | `rgba(168,85,247,0.15)` | `bg-purple-950/40` | Fondo icono KPI |

---

## 3. Componentes por Pantalla

---

### 3.1 Pantalla: Registro de Promotor (`/promotor/registro`)

#### Layout

```
┌──────────────────────────────────────────────────────────────┐
│  HEADER: PublicLayout header existente (sticky)              │
├──────────────────────────────────────────────────────────────┤
│  bg-[#1a1a2e] min-h-screen                                  │
│                                                              │
│       [Icono megafono - gradient]   (centrado)               │
│       Conviertete en Promotor        h1                      │
│       Difunde la musica...           p.subtitle              │
│                                                              │
│  ┌─────────────────────────────────────────────────────┐     │
│  │  Card max-w-lg mx-auto bg-[#151525]                 │     │
│  │                                                     │     │
│  │  [Nombre publico *]                                 │     │
│  │  [Input text]                                       │     │
│  │  [Error message?]                                   │     │
│  │                                                     │     │
│  │  [Tipo de promotor *]                               │     │
│  │  [Select dropdown]                                  │     │
│  │  [Error message?]                                   │     │
│  │                                                     │     │
│  │  [Email de contacto]                                │     │
│  │  [Input email]                                      │     │
│  │                                                     │     │
│  │  [Sitio web]                                        │     │
│  │  [Input url]                                        │     │
│  │                                                     │     │
│  │  REDES SOCIALES                                     │     │
│  │  [ig] Instagram  [Input url]                        │     │
│  │  [tt] TikTok     [Input url]                        │     │
│  │  [yt] YouTube    [Input url]                        │     │
│  │  [tw] Twitter/X  [Input url]                        │     │
│  │                                                     │     │
│  │  [Aviso FA-03 - azul - condicional]                 │     │
│  │                                                     │     │
│  │  [Cancelar]         [Crear perfil ->]               │     │
│  └─────────────────────────────────────────────────────┘     │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

**Breakpoints responsive:**
- Mobile (< sm): `max-w-lg` se convierte en `w-full px-4`
- sm+: Card centrada `mx-auto`

#### Componentes

**RegistroPromotorPage (contenedor de pagina)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Wrapper pagina | `<div>` | `min-h-screen bg-[#1a1a2e] py-12 px-4` |
| Hero icon | `<div>` | `w-14 h-14 p-3 rounded-xl bg-gradient-to-br from-pink-500 to-purple-600 text-white mx-auto mb-4 flex items-center justify-center` |
| Titulo h1 | `<h1>` | `text-3xl font-bold text-white text-center mb-2` |
| Subtitulo | `<p>` | `text-base text-[#94a3b8] text-center mb-8 max-w-sm mx-auto` |
| Card formulario | `Card` | `max-w-lg mx-auto bg-[#151525] border border-[#334155] rounded-xl shadow-lg` |
| Cuerpo card | `CardContent` | `p-6 sm:p-8` |

**RegistroPromotorForm (componente de formulario)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Formulario | `<form>` nativo | `space-y-5` |
| Field wrapper | `<div>` | `space-y-1.5` |
| Label requerido | `Label` | `text-sm font-medium text-[#cbd5e1] block` + asterisco `<span className="text-red-500 ml-1">*</span>` |
| Label opcional | `Label` | `text-sm font-medium text-[#cbd5e1] block` |
| Input nombre | `Input` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11 focus-visible:ring-0 focus-visible:ring-offset-0` |
| Select tipo | `Select` + sub-partes | Ver seccion 4.1 |
| Input email | `Input` type="email" | Igual que input nombre |
| Input sitio web | `Input` type="url" | Igual que input nombre, `placeholder="https://tusitio.com"` |
| Error de campo | `<p>` | `text-xs text-red-400 mt-1 flex items-center gap-1` + `<AlertCircle className="w-3 h-3 flex-shrink-0" />` |

**SocialNetworksSection (sub-componente)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Heading de seccion | `<p>` | `text-sm font-medium text-[#cbd5e1] mb-3` |
| Fila red social | `<div>` | `flex items-center gap-3 mb-3` |
| Icono red social | `<span>` | `w-8 h-8 rounded-md flex items-center justify-center bg-[#1e1e38] text-[#94a3b8] flex-shrink-0` |
| Label inline red | `<span>` | `text-sm text-[#94a3b8] w-20 flex-shrink-0` |
| Input URL red | `Input` type="url" | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-10 flex-1 focus-visible:ring-0` |
| Error de URL | `<p>` con `role="alert"` | `text-xs text-red-400 mt-1 ml-11 flex items-center gap-1` |

**SocialNetworksWarning (aviso FA-03 - condicional)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Aviso informativo | `<div>` | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-1 animate-in fade-in duration-300` |
| Icono info | `<Info>` lucide-react | `w-4 h-4 flex-shrink-0 mt-0.5` |
| Texto aviso | `<p>` | `text-sm text-blue-300` |

**FormActions (botones de accion)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Contenedor botones | `<div>` | `flex items-center justify-between pt-4` |
| Boton Cancelar | `Button` variant="ghost" | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Crear perfil | `Button` type="submit" | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11` |
| Spinner loading | `<Loader2>` lucide | `w-4 h-4 animate-spin mr-2` (solo en estado submitting) |

**Composicion completa RegistroPromotorPage:**
```tsx
<div className="min-h-screen bg-[#1a1a2e] py-12 px-4">
    <div className="mx-auto mb-8 text-center">
        <div className="w-14 h-14 p-3 rounded-xl bg-gradient-to-br from-pink-500 to-purple-600 text-white mx-auto mb-4 flex items-center justify-center">
            <Megaphone className="w-7 h-7" />
        </div>
        <h1 className="text-3xl font-bold text-white mb-2">
            Conviertete en Promotor
        </h1>
        <p className="text-base text-[#94a3b8] max-w-sm mx-auto">
            Difunde la musica que amas y gana comisiones
        </p>
    </div>

    <Card className="max-w-lg mx-auto bg-[#151525] border border-[#334155] rounded-xl shadow-lg">
        <CardContent className="p-6 sm:p-8">
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                {/* Campo: Nombre publico */}
                {/* Campo: Tipo de promotor (Select) */}
                {/* Campo: Email de contacto */}
                {/* Campo: Sitio web */}
                {/* Seccion: Redes sociales */}
                {/* Aviso FA-03 (condicional) */}
                {/* Botones: Cancelar / Crear perfil */}
            </form>
        </CardContent>
    </Card>
</div>
```

---

### 3.2 Pantalla: Dashboard de Promotor (`/promotor/dashboard`)

#### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ SIDEBAR w-64 bg-[#0d0d1a]  │  MAIN flex-1 bg-[#1a1a2e]         │
│                             │                                     │
│ [Logo WePlay]               │  TOPBAR h-16 bg-[#0d0d1a]          │
│ ─────────────────────────  │  ──────────────────────────────────  │
│ [Avatar] {nombrePublico}    │  Hola, {nombrePublico}              │
│          Promotor           │  {fechaHoy}    [Editar perfil ->]   │
│ ─────────────────────────  │                                     │
│  > Dashboard                │  KPI Cards (grid 3 columnas)        │
│    Mis Programas            │  ┌──────────┐┌─────────┐┌────────┐ │
│    Comisiones               │  │Programas ││Comision.││Wallet  │ │
│    Mi Perfil                │  │ activos  ││ ganadas ││ saldo  │ │
│ ─────────────────────────  │  │  [  3  ] ││[€150.50]││[€150.5]│ │
│    Configuracion            │  └──────────┘└─────────┘└────────┘ │
│    [Logout]                 │                                     │
│                             │  PromotorProfileCard                │
│                             │  ┌────────────────────────────────┐ │
│                             │  │ Mi Perfil de Promotor          │ │
│                             │  │                                │ │
│                             │  │ [Avatar initials]              │ │
│                             │  │ {nombre}   [ACTIVO badge]      │ │
│                             │  │ {tipoPromotor}                 │ │
│                             │  │ Separator                      │ │
│                             │  │ Email: {val}  Web: {val}       │ │
│                             │  │ Redes: [ig][tt][yt][tw]        │ │
│                             │  │                                │ │
│                             │  │ [Editar perfil] [Ver programas]│ │
│                             │  └────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

**Breakpoints responsive:**
- Mobile (< md): Sidebar colapsa en menu hamburguesa (usar `Sheet` de shadcn si se implementa) o simplemente oculto. Layout de una columna.
- md+: Layout de 2 columnas (sidebar + main).
- Grid de KPI: `grid-cols-1 sm:grid-cols-3`.

#### Componentes

**PromotorDashboardLayout (wrapper del layout)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Wrapper principal | `<div>` | `flex h-screen bg-[#1a1a2e]` |
| Sidebar | `<aside>` | `hidden md:flex w-64 min-h-screen bg-[#0d0d1a] border-r border-[#334155] flex-col` |
| Main content | `<main>` | `flex-1 overflow-auto` |
| Topbar | `<header>` | `h-16 bg-[#0d0d1a] border-b border-[#334155] flex items-center justify-between px-6` |
| Page content | `<div>` | `p-6 space-y-6` |

**PromotorSidebar**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Logo area | `<div>` | `p-6 flex items-center gap-3 border-b border-[#334155]` |
| Logo icono | `<div>` | `w-8 h-8 p-1.5 rounded-lg bg-gradient-to-br from-pink-500 to-purple-600 text-white flex items-center justify-center` |
| Logo texto | `<span>` | `text-lg font-bold text-white` |
| User area | `<div>` | `p-4 flex items-center gap-3 border-b border-[#334155]` |
| Avatar usuario | `Avatar` | `w-10 h-10` |
| Avatar fallback | `AvatarFallback` | `bg-gradient-to-br from-pink-500 to-purple-600 text-white text-sm font-semibold` |
| Nombre usuario | `<p>` | `text-sm font-medium text-white` |
| Rol | `<p>` | `text-xs text-[#94a3b8]` con "Promotor" |
| Nav | `<nav>` | `flex-1 p-3 space-y-1` aria-label="Navegacion de promotor" |
| Nav item activo | `Button` variant="ghost" | `w-full justify-start text-white bg-[#1e1e38] border-l-2 border-[#a855f7]` |
| Nav item inactivo | `Button` variant="ghost" | `w-full justify-start text-[#94a3b8] hover:text-white hover:bg-[#1a1a2e]` |
| Nav icono | lucide-react icon | `w-4 h-4 mr-3 flex-shrink-0` |
| Footer sidebar | `<div>` | `p-3 border-t border-[#334155]` |
| Boton logout | `Button` variant="ghost" | `w-full justify-start text-red-400 hover:text-red-300 hover:bg-red-950/30 gap-3` |

**DashboardTopbar**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Saludo | `<h1>` | `text-2xl font-bold text-white` |
| Fecha | `<p>` | `text-sm text-[#94a3b8] mt-0.5` |
| Boton editar | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-5` |

**PromotorInactiveBanner (condicional: solo si esActivo === false)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Banner | `Alert` | `mb-6 bg-amber-950/40 border border-amber-800/50` |
| Icono | `<AlertCircle>` lucide | `h-4 w-4 text-amber-400` |
| Titulo | `AlertTitle` | `text-amber-300 font-semibold` |
| Descripcion | `AlertDescription` | `text-amber-400/80` |
| Link reactivar | `<a>` o `Button` variant="link" | `text-amber-300 underline hover:text-amber-200 ml-1` |

**KpiCardsGrid**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Grid wrapper | `<div>` | `grid grid-cols-1 sm:grid-cols-3 gap-4` |
| KPI Card | `Card` | `bg-[#151525] border-[#334155] p-5` |
| Icono container programas | `<div>` | `w-10 h-10 rounded-lg bg-emerald-950/40 flex items-center justify-center mb-3` |
| Icono programas | `<Activity>` lucide | `w-5 h-5 text-[#10b981]` |
| Icono container comisiones | `<div>` | `w-10 h-10 rounded-lg bg-blue-950/40 flex items-center justify-center mb-3` |
| Icono comisiones | `<TrendingUp>` lucide | `w-5 h-5 text-[#3b82f6]` |
| Icono container wallet | `<div>` | `w-10 h-10 rounded-lg bg-purple-950/40 flex items-center justify-center mb-3` |
| Icono wallet | `<Wallet>` lucide | `w-5 h-5 text-[#a855f7]` |
| Valor KPI | `<p>` | `text-3xl font-bold text-white` |
| Label KPI | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Skeleton loading | `Skeleton` | `h-8 w-16 rounded bg-[#1e1e38]` |

**Composicion KPI Card (ejemplo: Programas Activos):**
```tsx
<Card className="bg-[#151525] border-[#334155] p-5">
    <CardContent className="p-0">
        <div className="w-10 h-10 rounded-lg bg-emerald-950/40 flex items-center justify-center mb-3">
            <Activity className="w-5 h-5 text-[#10b981]" aria-hidden="true" />
        </div>
        {isLoading ? (
            <Skeleton className="h-8 w-16 rounded bg-[#1e1e38]" />
        ) : (
            <p className="text-3xl font-bold text-white">
                {promotor.totalProgramasActivos}
            </p>
        )}
        <p className="text-sm text-[#94a3b8] mt-1">Programas activos</p>
    </CardContent>
</Card>
```

**PromotorProfileCard**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Card | `Card` | `bg-[#151525] border-[#334155]` |
| Card header | `CardHeader` | `border-b border-[#334155] pb-4` |
| Card titulo | `<h2>` | `text-lg font-semibold text-white` |
| Card body | `CardContent` | `pt-6` |
| Avatar | `Avatar` | `w-16 h-16` |
| Avatar fallback | `AvatarFallback` | `bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold` con iniciales del nombre |
| Nombre | `<p>` | `text-xl font-semibold text-white mt-3` |
| Badge activo | `Badge` | className custom: `bg-green-950/50 text-green-400 border border-green-800/50 ml-2` |
| Badge inactivo | `Badge` | className custom: `bg-slate-800 text-slate-400 border border-slate-700 ml-2` |
| Tipo promotor | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Separador | `Separator` | `bg-[#334155] my-4` |
| Fila de dato | `<div>` | `flex items-start gap-3 mb-3` |
| Label campo | `<span>` | `text-xs text-[#64748b] uppercase tracking-wider w-14 flex-shrink-0 mt-0.5` |
| Valor campo | `<span>` | `text-sm text-[#cbd5e1]` |
| Valor vacio | `<span>` | `text-sm text-[#64748b] italic` con "--" |
| Enlace sitio web | `<a>` | `text-sm text-[#a855f7] hover:underline truncate max-w-[200px] inline-block` target="_blank" rel="noopener noreferrer" |
| Fila redes sociales | `<div>` | `flex items-center gap-2 mt-1` |
| Icono red social | `<a>` | `w-8 h-8 flex items-center justify-center rounded-md bg-[#1e1e38] text-[#94a3b8] hover:text-white hover:bg-[#334155] transition-colors` target="_blank" |
| Texto sin redes | `<p>` | `text-sm text-[#64748b] italic` con "Sin redes sociales configuradas" |
| Card footer | `CardFooter` | `pt-4 border-t border-[#334155] flex gap-3` |
| Boton editar | `Button` variant="outline" | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white` |
| Boton ver programas | `Button` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white` deshabilitado si esActivo === false |

**Composicion PromotorProfileCard:**
```tsx
<Card className="bg-[#151525] border-[#334155]">
    <CardHeader className="border-b border-[#334155] pb-4">
        <h2 className="text-lg font-semibold text-white">Mi Perfil de Promotor</h2>
    </CardHeader>
    <CardContent className="pt-6">
        <div className="flex flex-col items-center text-center sm:flex-row sm:items-start sm:text-left gap-4">
            <Avatar className="w-16 h-16">
                <AvatarFallback className="bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold">
                    {initials}
                </AvatarFallback>
            </Avatar>
            <div>
                <div className="flex items-center gap-2 flex-wrap">
                    <p className="text-xl font-semibold text-white">{promotor.nombrePublico}</p>
                    <Badge className={esActivo ? "bg-green-950/50 text-green-400 border border-green-800/50" : "bg-slate-800 text-slate-400 border border-slate-700"}>
                        {esActivo ? "ACTIVO" : "INACTIVO"}
                    </Badge>
                </div>
                <p className="text-sm text-[#94a3b8] mt-1">{promotor.tipoPromotorNombre}</p>
            </div>
        </div>
        <Separator className="bg-[#334155] my-4" />
        {/* Datos: Email, Web */}
        {/* Iconos de redes */}
    </CardContent>
    <CardFooter className="pt-4 border-t border-[#334155] flex gap-3">
        <Button variant="outline" className="border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white">
            Editar perfil
        </Button>
        <Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white" disabled={!esActivo}>
            Ver programas
        </Button>
    </CardFooter>
</Card>
```

---

### 3.3 Pantalla: Edicion de Perfil (`/promotor/perfil`)

#### Layout

```
┌──────────────────────────────────────────────────────────────────┐
│ SIDEBAR (mismo que dashboard)   │  MAIN flex-1                   │
│                                 │                                 │
│  > Dashboard                    │  TOPBAR                         │
│    Mis Programas                │  ────────────────────────────── │
│    Comisiones                   │  Editar Perfil                  │
│  * Mi Perfil [activo]           │  Actualiza tu informacion       │
│                                 │                                 │
│                                 │  ┌──────────────────────────┐   │
│                                 │  │ Informacion publica      │   │
│                                 │  │ ─────────────────────    │   │
│                                 │  │ Nombre publico *         │   │
│                                 │  │ [Input - editable]       │   │
│                                 │  │                          │   │
│                                 │  │ Tipo de promotor         │   │
│                                 │  │ [Input - disabled+lock]  │   │
│                                 │  │ Nota explicativa         │   │
│                                 │  │                          │   │
│                                 │  │ Email de contacto        │   │
│                                 │  │ [Input - editable]       │   │
│                                 │  │                          │   │
│                                 │  │ Sitio web                │   │
│                                 │  │ [Input - editable]       │   │
│                                 │  └──────────────────────────┘   │
│                                 │                                 │
│                                 │  ┌──────────────────────────┐   │
│                                 │  │ Redes sociales           │   │
│                                 │  │ [ig] Instagram [Input]   │   │
│                                 │  │ [tt] TikTok    [Input]   │   │
│                                 │  │ [yt] YouTube   [Input]   │   │
│                                 │  │ [tw] Twitter/X [Input]   │   │
│                                 │  └──────────────────────────┘   │
│                                 │                                 │
│                                 │  ┌──────────────────────────┐   │
│                                 │  │ [!] ZONA DE PELIGRO      │   │
│                                 │  │ Desactivar cuenta...     │   │
│                                 │  │ [Desactivar cuenta]      │   │
│                                 │  └──────────────────────────┘   │
│                                 │                                 │
│                                 │  [Cancelar] [Guardar cambios->] │
└──────────────────────────────────────────────────────────────────┘
```

#### Componentes

**EditarPerfilPage (contenedor de pagina)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Wrapper pagina | `<div>` | `p-6 max-w-2xl` |
| Titulo de pagina | `<h1>` | `text-2xl font-bold text-white mb-1` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mb-6` |
| Stack de cards | `<div>` | `space-y-6` |

**InformacionPublicaCard**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Card | `Card` | `bg-[#151525] border-[#334155]` |
| Card header | `CardHeader` | `border-b border-[#334155] pb-4` |
| Card titulo | `CardTitle` | `text-lg font-semibold text-white` |
| Card body | `CardContent` | `pt-6 space-y-5` |
| Input nombre | `Input` | `bg-[#0f0f1f] border-[#334155] text-white h-11 focus:border-[#a855f7] focus-visible:ring-0` pre-rellenado |
| Input tipo (readonly) | `Input` disabled | `bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed h-11` |
| Wrapper tipo | `<div>` | `relative` |
| Icono lock | `<Lock>` lucide | `absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b]` |
| Nota tipo | `<p>` | `text-xs text-[#64748b] mt-1` |
| Input email | `Input` type="email" | Igual que input nombre, pre-rellenado |
| Input sitio web | `Input` type="url" | Igual que input nombre, pre-rellenado |

**RedesSocialesCard**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Card | `Card` | `bg-[#151525] border-[#334155]` |
| Card header | `CardHeader` | `border-b border-[#334155] pb-4` |
| Card titulo | `CardTitle` | `text-lg font-semibold text-white` |
| Card body | `CardContent` | `pt-6 space-y-4` |
| Field wrapper por red | `<div>` | `space-y-1.5` |
| Label con icono | `Label` | `flex items-center gap-2 text-sm font-medium text-[#cbd5e1]` |
| Icono red social | Icono SVG / lucide | `w-4 h-4` con color de la plataforma |
| Input URL | `Input` type="url" | `bg-[#0f0f1f] border-[#334155] text-white h-11 focus:border-[#a855f7] focus-visible:ring-0` pre-rellenado |
| Error URL | `<p>` role="alert" | `text-xs text-red-400 mt-1 flex items-center gap-1` |

**DangerZoneCard**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Card | `Card` | `bg-[#151525] border border-red-900/50` |
| Card header | `CardHeader` | `border-b border-red-900/50 pb-4` |
| Card titulo | `CardTitle` | `text-base font-semibold text-red-400 flex items-center gap-2` |
| Icono alerta | `<AlertTriangle>` lucide | `w-4 h-4 text-red-400` aria-hidden="true" |
| Card body | `CardContent` | `pt-4` |
| Descripcion | `<p>` | `text-sm text-[#94a3b8] mb-4` |
| Boton desactivar | `Button` variant="outline" | `border-red-800 text-red-400 hover:bg-red-950/50 hover:text-red-300 hover:border-red-700` |

**EditarPerfilFormActions (botones sticky)**

| Elemento | Componente shadcn | Customizacion Tailwind |
|----------|-------------------|----------------------|
| Contenedor | `<div>` | `flex items-center justify-end gap-3 pt-6 border-t border-[#334155] sticky bottom-0 bg-[#1a1a2e] pb-4` |
| Boton Cancelar | `Button` variant="ghost" | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Guardar | `Button` type="submit" | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11` deshabilitado si `!isDirty` |
| Spinner loading | `<Loader2>` lucide | `w-4 h-4 animate-spin mr-2` solo en estado saving |

---

## 4. Formularios

### 4.1 Selector de Tipo de Promotor (solo en Registro)

**Composicion Select:**
```tsx
<div className="space-y-1.5">
    <Label htmlFor="tipoPromotorId" className="text-sm font-medium text-[#cbd5e1]">
        Tipo de promotor <span className="text-red-500 ml-1">*</span>
    </Label>
    {isLoadingTipos ? (
        <Skeleton className="h-11 w-full rounded-md bg-[#1e1e38]" />
    ) : (
        <Select
            value={String(field.value)}
            onValueChange={(val) => field.onChange(Number(val))}
            disabled={errorLoadingTipos}
        >
            <SelectTrigger
                id="tipoPromotorId"
                className="bg-[#0f0f1f] border-[#334155] text-white h-11 focus:ring-[#a855f7] focus:ring-offset-0"
                aria-describedby={errors.tipoPromotorId ? "tipoPromotorId-error" : undefined}
            >
                <SelectValue placeholder="Selecciona un tipo de promotor" />
            </SelectTrigger>
            <SelectContent className="bg-[#151525] border-[#334155]">
                {tiposPromotor.map((tipo) => (
                    <SelectItem
                        key={tipo.id}
                        value={String(tipo.id)}
                        className="text-white hover:bg-[#1e1e38] focus:bg-[#1e1e38]"
                    >
                        {tipo.nombre}
                    </SelectItem>
                ))}
            </SelectContent>
        </Select>
    )}
    {errorLoadingTipos && (
        <p className="text-xs text-[#94a3b8]">No se pudo cargar los tipos de promotor</p>
    )}
    {errors.tipoPromotorId && (
        <p id="tipoPromotorId-error" role="alert" className="text-xs text-red-400 mt-1 flex items-center gap-1">
            <AlertCircle className="w-3 h-3 flex-shrink-0" aria-hidden="true" />
            {errors.tipoPromotorId.message}
        </p>
    )}
</div>
```

**Opciones del Select:**

| id | nombre visible | Descripcion tooltip |
|----|---------------|---------------------|
| 1 | Fan Embajador | Fan que promueve artistas por pasion y por recompensas |
| 2 | Influencer | Creador de contenido con audiencia en redes sociales |
| 3 | Medio / Blog | Medio de comunicacion, blog o podcast musical |
| 4 | Profesional Marketing | Profesional del marketing digital o musical |

Nota: Los tipos se pueden cargar desde constantes `TIPO_PROMOTOR_LABELS` del shared (sin llamada a API en MVP). Ver `contracts.md`.

### 4.2 Campos de Formulario - Estado de Inputs

**Campos del formulario de registro:**

| Campo | Componente | Tipo | Requerido | Validacion visual |
|-------|------------|------|-----------|-------------------|
| nombrePublico | `Input` | text | Si | `FormMessage` con error bajo el input |
| tipoPromotorId | `Select` | select | Si | `FormMessage` con error bajo el select |
| emailContacto | `Input` | email | No | `FormMessage` si formato invalido |
| urlSitioWeb | `Input` | url | No | `FormMessage` si URL invalida |
| urlInstagram | `Input` | url | No | `FormMessage` si URL invalida |
| urlTikTok | `Input` | url | No | `FormMessage` si URL invalida |
| urlYouTube | `Input` | url | No | `FormMessage` si URL invalida |
| urlTwitter | `Input` | url | No | `FormMessage` si URL invalida |

**Campos del formulario de edicion (updatePromotorSchema - sin tipoPromotorId):**

| Campo | Componente | Editable | Notas |
|-------|------------|----------|-------|
| nombrePublico | `Input` | Si | Pre-rellenado con valor actual |
| tipoPromotorId | `Input` disabled | No | Solo lectura. Icono `<Lock>`. Nota explicativa debajo |
| emailContacto | `Input` type="email" | Si | Pre-rellenado |
| urlSitioWeb | `Input` type="url" | Si | Pre-rellenado |
| urlInstagram | `Input` type="url" | Si | Pre-rellenado |
| urlTikTok | `Input` type="url" | Si | Pre-rellenado |
| urlYouTube | `Input` type="url" | Si | Pre-rellenado |
| urlTwitter | `Input` type="url" | Si | Pre-rellenado |

**Estados visuales de inputs:**

| Estado | Clases aplicadas |
|--------|-----------------|
| Default | `bg-[#0f0f1f] border-[#334155] text-white` |
| Focus | `border-[#a855f7]` (anula el ring por defecto de shadcn con `focus-visible:ring-0`) |
| Error | `border-red-500 focus:border-red-500` |
| Disabled (readonly) | `bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed opacity-100` (no usar `opacity-50` para que sea legible) |
| Submitting | `disabled:opacity-70` en todos los inputs |

---

## 5. Dialogos

### 5.1 Dialogo de Confirmacion de Desactivacion

**Trigger:** `Button` variant="outline" en la DangerZoneCard de `/promotor/perfil`.

**Estado pre-apertura:** Antes de abrir el dialogo, hacer GET del perfil (ya cargado en la pagina) para obtener `totalProgramasActivos`. El numero de programas ya esta disponible en el estado de la pagina.

**Composicion:**
```tsx
<Dialog open={isDesactivarDialogOpen} onOpenChange={setIsDesactivarDialogOpen}>
    <DialogContent className="max-w-md bg-[#151525] border border-[#334155] text-white">
        <DialogHeader>
            <DialogTitle className="text-lg font-semibold text-white flex items-center gap-2">
                <AlertTriangle className="w-5 h-5 text-red-400" aria-hidden="true" />
                Desactivar cuenta de promotor
            </DialogTitle>
        </DialogHeader>

        <DialogDescription className="text-sm text-[#94a3b8] mt-2">
            {/* Caso A: con programas activos */}
            {promotor.totalProgramasActivos > 0 ? (
                <>
                    Estas participando en <strong className="text-white font-semibold">{promotor.totalProgramasActivos} programa{promotor.totalProgramasActivos > 1 ? 's' : ''}</strong> de
                    promocion activos. Al desactivar tu cuenta seras dado de baja de todos ellos automaticamente.
                </>
            ) : (
                /* Caso B: sin programas activos */
                <>
                    Seguro que quieres desactivar tu cuenta de promotor? Esta accion desactivara
                    tu perfil pero no lo eliminara. Podras reactivarlo contactando con soporte.
                </>
            )}
        </DialogDescription>

        {promotor.totalProgramasActivos > 0 && (
            <div className="bg-amber-950/30 border border-amber-800/50 rounded-lg p-3 flex items-start gap-2 mt-2">
                <AlertTriangle className="w-4 h-4 text-amber-400 flex-shrink-0 mt-0.5" aria-hidden="true" />
                <p className="text-sm text-amber-300">
                    Esta accion afectara a {promotor.totalProgramasActivos} programa{promotor.totalProgramasActivos > 1 ? 's' : ''} activo{promotor.totalProgramasActivos > 1 ? 's' : ''}.
                </p>
            </div>
        )}

        <Separator className="bg-[#334155] my-1" />

        <DialogFooter className="pt-2">
            <Button
                variant="outline"
                disabled={isDesactivando}
                className="border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white"
                onClick={() => setIsDesactivarDialogOpen(false)}
            >
                Cancelar
            </Button>
            <Button
                variant="destructive"
                disabled={isDesactivando}
                className="bg-red-600 hover:bg-red-700 text-white font-semibold"
                onClick={handleDesactivar}
            >
                {isDesactivando ? (
                    <>
                        <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />
                        Desactivando...
                    </>
                ) : (
                    "Desactivar cuenta"
                )}
            </Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

### 5.2 Dialogo de Confirmacion de Salida (Cancelar con cambios)

Se activa en `/promotor/perfil` cuando el usuario hace clic en "Cancelar" con cambios pendientes (`isDirty === true`).

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Dialogo | `Dialog` | `max-w-sm` |
| Titulo | `DialogTitle` | `text-base font-semibold text-white` |
| Descripcion | `DialogDescription` | `text-sm text-[#94a3b8]` |
| Footer | `DialogFooter` | `pt-2` |
| Boton continuar | `Button` variant="ghost" | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38]` |
| Boton salir | `Button` variant="outline" | `border-[#334155] text-white hover:bg-[#1e2a42]` |

---

## 6. Feedback y Estados

### 6.1 Loading States

| Elemento | Componente | Notas |
|----------|------------|-------|
| KPI cards cargando | `Skeleton` | `h-8 w-16 rounded bg-[#1e1e38] animate-pulse` para valor, `h-4 w-24 rounded bg-[#1e1e38] animate-pulse mt-2` para label |
| Profile card cargando | `Skeleton` | Avatar: `Skeleton className="w-16 h-16 rounded-full bg-[#1e1e38]"`, Nombre: `Skeleton className="h-6 w-40 rounded bg-[#1e1e38]"` |
| Selector tipo promotor cargando | `Skeleton` | `h-11 w-full rounded-md bg-[#1e1e38]` |
| Editar perfil cargando | `Skeleton` x N | Un skeleton por cada input, `h-11 w-full rounded-md bg-[#1e1e38]` |
| Boton submitting | `Button` disabled | Spinner `<Loader2 className="w-4 h-4 animate-spin mr-2" />` + texto "Creando perfil..." o "Guardando..." |

### 6.2 Error States

| Error | Componente | Visual |
|-------|------------|--------|
| Error de campo (validacion Zod) | `<p>` role="alert" | `text-xs text-red-400 mt-1 flex items-center gap-1` con `<AlertCircle className="w-3 h-3" />` |
| Error de carga de datos (query) | `Alert` variant="destructive" | `bg-red-950/40 border border-red-800/50` con `AlertTitle` + `AlertDescription` + boton "Reintentar" |
| Error de API en mutation | Toast (sonner) | `toast.error("titulo", { description: "mensaje" })` - la config de sonner ya existe en el proyecto |
| Selector tipo promotor no carga | `<p>` + `Input` disabled | Texto "No se pudo cargar los tipos de promotor" debajo del select deshabilitado |
| Error 4018 (promotor ya existe) | Toast + redirect | `toast.error("Ya tienes un perfil de promotor")` + redirect a `/promotor/dashboard` |

### 6.3 Success States

| Accion | Feedback |
|--------|----------|
| Perfil creado (POST OK) | `toast.success("Perfil de promotor creado correctamente")` + redirect a `/promotor/dashboard` tras 1.5s |
| Perfil actualizado (PUT OK) | `toast.success("Perfil actualizado correctamente")` + boton "Guardar" vuelve a disabled |
| Perfil desactivado (PATCH OK) | `toast.success("Perfil de promotor desactivado")` + cerrar dialogo + redirect a `/` |

### 6.4 Empty States

| Contexto | Visual |
|----------|--------|
| Sin redes sociales (Dashboard) | `<p className="text-sm text-[#64748b] italic">Sin redes sociales configuradas</p>` en la fila de iconos de redes |
| Sin programas activos (KPI) | Valor "0" mostrado normalmente, sin estilo especial |
| Wallet en cero | "€0.00" mostrado normalmente |

---

## 7. Componentes Nuevos a Crear

Los siguientes componentes son nuevos dentro de `src/web/src/features/crowdpromotion/`:

| Componente | Tipo | Ruta sugerida |
|------------|------|---------------|
| `RegistroPromotorPage` | Page | `features/crowdpromotion/presentation/pages/RegistroPromotorPage.tsx` |
| `DashboardPromotorPage` | Page | `features/crowdpromotion/presentation/pages/DashboardPromotorPage.tsx` |
| `EditarPerfilPromotorPage` | Page | `features/crowdpromotion/presentation/pages/EditarPerfilPromotorPage.tsx` |
| `RegistroPromotorForm` | Component | `features/crowdpromotion/presentation/components/RegistroPromotorForm.tsx` |
| `SocialNetworksSection` | Component | `features/crowdpromotion/presentation/components/SocialNetworksSection.tsx` |
| `KpiCardsGrid` | Component | `features/crowdpromotion/presentation/components/KpiCardsGrid.tsx` |
| `PromotorProfileCard` | Component | `features/crowdpromotion/presentation/components/PromotorProfileCard.tsx` |
| `DangerZoneCard` | Component | `features/crowdpromotion/presentation/components/DangerZoneCard.tsx` |
| `DesactivarPromotorDialog` | Component | `features/crowdpromotion/presentation/components/DesactivarPromotorDialog.tsx` |
| `PromotorDashboardSkeleton` | Component | `features/crowdpromotion/presentation/components/PromotorDashboardSkeleton.tsx` |
| `PromotorInactiveBanner` | Component | `features/crowdpromotion/presentation/components/PromotorInactiveBanner.tsx` |

**Componentes existentes a reutilizar:**
- `DashboardLayout` (ya existe en `src/web/src/components/layout/DashboardLayout.tsx`) — se reutiliza sin modificar
- `Sidebar` (ya existe) — se extiende con items de navegacion de promotor
- `PublicLayout` (ya existe) — envuelve `/promotor/registro`
- Todos los componentes shadcn de `src/web/src/components/ui/`

---

## 8. Responsive Design

### Breakpoints

| Breakpoint | Resolucion | Cambios de layout |
|------------|------------|-------------------|
| Default (mobile) | < 640px | Stack vertical, card full width, sidebar oculto, form ocupa pantalla completa |
| sm (tablet small) | >= 640px | KPI cards en 3 columnas (`grid-cols-3`), card con `max-w-lg` |
| md (tablet) | >= 768px | Sidebar visible (`hidden md:flex`), layout 2 columnas |
| lg (desktop) | >= 1024px | Contenido maximo `max-w-2xl` para formularios |

### Estrategia Mobile

**Registro (`/promotor/registro`):**
```tsx
// Wrapper mobile-first
className="min-h-screen bg-[#1a1a2e] py-8 px-4 sm:py-12"

// Card del formulario
className="w-full sm:max-w-lg sm:mx-auto bg-[#151525] border border-[#334155] rounded-xl shadow-lg"

// CardContent padding responsive
className="p-5 sm:p-8"

// Botones de accion en mobile: stack vertical
className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 pt-4"
```

**Dashboard (`/promotor/dashboard`):**
```tsx
// Sidebar oculto en mobile
className="hidden md:flex w-64 min-h-screen ..."

// KPI grid responsive
className="grid grid-cols-1 sm:grid-cols-3 gap-4"

// Topbar en mobile: solo titulo + boton
className="h-16 bg-[#0d0d1a] border-b border-[#334155] flex items-center justify-between px-4 sm:px-6"
```

**Editar Perfil (`/promotor/perfil`):**
```tsx
// Page content max-width
className="p-4 sm:p-6 max-w-2xl"

// Botones: stack en mobile, horizontal en desktop
className="flex flex-col-reverse sm:flex-row sm:items-center sm:justify-end gap-3 pt-6 border-t border-[#334155]"
```

---

## 9. Accesibilidad (WCAG 2.1 AA)

### Requisitos por pantalla

| Requisito | Implementacion |
|-----------|----------------|
| Labels vinculados a inputs | `<Label htmlFor="fieldId">` + `<Input id="fieldId">` en todos los campos |
| Errores accesibles | `<p role="alert" id="fieldId-error">` + `aria-describedby="fieldId-error"` en input con error |
| Campo requerido | `required` attr en input + `aria-required="true"` en Select trigger |
| Campo deshabilitado | `disabled` attr + `aria-disabled="true"` en input tipo promotor |
| Dialogo accesible | `Dialog` de Radix UI maneja `role="dialog"`, `aria-modal="true"`, `aria-labelledby`, `aria-describedby` automaticamente |
| Focus trap en dialogo | Radix UI lo gestiona automaticamente |
| Focus visible | Mantener el ring de focus visible; solo desactivar el ring por defecto de shadcn cuando el border de focus es suficiente visualmente (`focus-visible:ring-0 focus-visible:ring-offset-0` solo en inputs donde el border #a855f7 comunica el focus) |
| Contraste de texto | Texto primario #ffffff sobre fondo #151525: ratio 15.1:1 (pasa WCAG AAA). Texto secundario #94a3b8 sobre #151525: ratio 4.8:1 (pasa WCAG AA) |
| Contraste texto error | #ef4444 sobre #151525: ratio 4.6:1 (pasa WCAG AA) |
| Iconos decorativos | `aria-hidden="true"` en todos los iconos de lucide-react que son decorativos |
| Iconos de accion | `aria-label` descriptivo en los `<a>` de iconos de redes sociales (ej: `aria-label="Ver Instagram de {nombre}"`) |
| Skip links | No requerido en MVP; la navegacion es simple |
| Breadcrumb dashboard | `<nav aria-label="breadcrumb">` con `aria-current="page"` en el item activo |
| Sidebar nav | `<nav aria-label="Navegacion de promotor">` |
| Estado badge | Usar `aria-live="polite"` o texto descriptivo complementario para el Badge de estado ACTIVO/INACTIVO |
| KPI cards | `<article>` o `<section>` con titulos accesibles para cada KPI card |
| Loading states | Skeleton items con `aria-busy="true"` en el contenedor o `role="status"` con texto sr-only "Cargando..." |

### Navegacion por teclado

| Elemento | Comportamiento teclado |
|----------|----------------------|
| Formulario registro | Tab navega linealmente por todos los campos; Enter envia el form |
| Select tipo promotor | Space/Enter abre el dropdown; Arrow keys navegan opciones; Enter/Space selecciona; Escape cierra |
| Dialogo desactivacion | Al abrir: foco va al primer elemento interactivo (boton Cancelar). Escape cierra. Tab cicla dentro del dialogo |
| Botones CTA | Space y Enter los activan |
| Links de redes (dashboard) | Accesibles por Tab, Enter abre en nueva pestana |

---

## 10. Estructura de Archivos Sugerida

```
src/web/src/features/crowdpromotion/
├── domain/
│   └── types.ts                    # Importar desde src/shared/types/crowdpromotion.ts
├── application/
│   ├── hooks/
│   │   ├── usePromotor.ts          # Query GET /api/crowdpromotion/promotor/me
│   │   ├── useCreatePromotor.ts    # Mutation POST /api/crowdpromotion/promotor
│   │   ├── useUpdatePromotor.ts    # Mutation PUT /api/crowdpromotion/promotor/me
│   │   └── useDesactivarPromotor.ts # Mutation PATCH /api/crowdpromotion/promotor/me/desactivar
│   └── index.ts                    # Barrel exports
├── presentation/
│   ├── pages/
│   │   ├── RegistroPromotorPage.tsx
│   │   ├── DashboardPromotorPage.tsx
│   │   └── EditarPerfilPromotorPage.tsx
│   └── components/
│       ├── RegistroPromotorForm.tsx
│       ├── SocialNetworksSection.tsx
│       ├── KpiCardsGrid.tsx
│       ├── KpiCard.tsx
│       ├── PromotorProfileCard.tsx
│       ├── PromotorInactiveBanner.tsx
│       ├── DangerZoneCard.tsx
│       ├── DesactivarPromotorDialog.tsx
│       └── PromotorDashboardSkeleton.tsx
└── __tests__/
    └── components/
        ├── RegistroPromotorForm.test.tsx
        ├── KpiCard.test.tsx
        └── PromotorProfileCard.test.tsx
```

---

## 11. Checklist de Diseno UI

### Formularios
- [ ] Todos los inputs tienen `Label` con `htmlFor` vinculado al `id` del input
- [ ] Errores de validacion Zod muestran mensaje con `role="alert"` bajo el campo
- [ ] Input tipo promotor en edicion: visualmente deshabilitado con icono `<Lock>` y nota explicativa
- [ ] Campos de URL opcionales: validacion solo si tienen valor (patron `or(z.literal(''))`)
- [ ] Aviso FA-03 (sin redes): aparece solo si todos los campos de URL de redes estan vacios

### Botones y acciones
- [ ] Boton submit con estado loading: spinner + texto "Creando perfil..." / "Guardando..."
- [ ] Boton "Guardar cambios" en edicion: deshabilitado si `!isDirty` (react-hook-form)
- [ ] Boton "Ver programas" en dashboard: deshabilitado si `esActivo === false`
- [ ] Boton "Cancelar" con cambios pendientes en edicion: abre dialogo de confirmacion de salida
- [ ] Todos los botones destructivos (Desactivar): requieren confirmacion via dialogo

### Estados de carga y error
- [ ] Dashboard: Skeleton para KPI cards y profile card durante carga inicial
- [ ] Editar perfil: Skeleton para inputs durante carga inicial
- [ ] Select tipo promotor: Skeleton durante carga de maestras; disabled si falla la carga
- [ ] Error de query: Alert con mensaje y boton "Reintentar"
- [ ] Error de mutation: Toast destructivo (sonner) con datos del formulario intactos

### Accesibilidad
- [ ] `aria-describedby` en inputs con error apuntando al `id` del mensaje de error
- [ ] `aria-hidden="true"` en todos los iconos decorativos
- [ ] `aria-label` descriptivo en iconos de redes sociales como enlaces
- [ ] `aria-label` en `<nav>` del sidebar
- [ ] Dialogo de desactivacion: foco inicial en boton Cancelar (el mas seguro)
- [ ] `aria-required="true"` en campos requeridos
- [ ] `aria-disabled="true"` en input tipo promotor (edicion)

### Responsive
- [ ] Registro: card full-width en mobile, `max-w-lg` en sm+
- [ ] Dashboard: sidebar oculto en mobile (`hidden md:flex`)
- [ ] KPI grid: 1 columna en mobile, 3 columnas en sm+
- [ ] Botones de accion: stack vertical en mobile, horizontal en sm+
- [ ] Iconos de redes: tap target minimo 44x44px en mobile

### Dark theme
- [ ] Todos los fondos usan los tokens del sistema: `#0d0d1a`, `#1a1a2e`, `#151525`, `#0f0f1f`
- [ ] Todos los bordes usan `#334155` (default) o sus variantes
- [ ] Botones principales usan el gradiente `from-pink-500 to-purple-600`
- [ ] Badge ACTIVO: green tones / Badge INACTIVO: slate tones
- [ ] Links y acentos usan `#a855f7`
- [ ] Banner de perfil inactivo: amber tones (`amber-950/40`, `amber-800/50`, `amber-300`)
- [ ] Zona de peligro: border y texto en red tones (`red-900/50`, `red-400`)
