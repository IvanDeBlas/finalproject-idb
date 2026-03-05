# UI/UX: Perfil de Promotor

> **Feature:** cp-perfil-promotor
> **User Story:** US-CP-01
> **Ultima actualizacion:** 2026-02-25

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Login / Auth | [WPR_4-Login.png](../../ui-images/WPR_4-Login.png) | Admin | Patron de formulario centrado con Card oscura |
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron sidebar + KPI cards + layout del dashboard |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing | Header, paleta de colores, estilo de la pagina publica |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Pagina de registro publica (/promotor/registro) | `references/templates/krowd/` |
| **Dashtail** | Dashboard promotor, edicion de perfil (admin) | `references/templates/dashtail/` |

**Componentes de referencia clave:**
- Formulario auth: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/auth/login-form.tsx`
- Dialogo de confirmacion: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/delete-confirmation-dialog.tsx`
- Layout dashboard: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/layout.tsx`

---

## Design Tokens

### Paleta de Colores

Extraida de WPR_4-Login.png (dark bg #1e1e2e), WPR_5-Dashboard-Artist.png (sidebar #0d0d1a, cards #151525), WPR_1-Landing.png (gradient buttons, accent pink/purple).

```css
:root {
  /* Backgrounds */
  --bg-primary: #0d0d1a;        /* Fondo base (sidebar, pantalla auth) */
  --bg-secondary: #1a1a2e;      /* Fondo de paginas y contenedores */
  --bg-card: #151525;           /* Fondo de cards y formularios */
  --bg-card-hover: #1e1e38;     /* Card hover state */
  --bg-input: #0f0f1f;          /* Fondo de inputs */
  --bg-input-disabled: #1a1a2e; /* Input deshabilitado */
  --bg-sidebar: #0d0d1a;        /* Sidebar del dashboard */

  /* Colores primarios - Gradiente pink/purple (extraido de botones WPR_1 y WPR_4) */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);
  --primary-active: #9333ea;

  /* Texto */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;
  --text-link: #a855f7;

  /* Estado */
  --status-active: #10b981;     /* Verde - estado ACTIVO del promotor */
  --status-inactive: #64748b;   /* Gris - estado INACTIVO */
  --status-warning: #f59e0b;    /* Amarillo - advertencia (programas afectados) */
  --status-error: #ef4444;      /* Rojo - errores de validacion */
  --status-info: #3b82f6;       /* Azul - mensajes informativos */

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
  --border-readonly: #1e2a42;

  /* KPI Cards (extraido de WPR_5) */
  --card-kpi-bg: #151525;
  --card-kpi-icon-bg-green: rgba(16, 185, 129, 0.15);
  --card-kpi-icon-bg-blue: rgba(59, 130, 246, 0.15);
  --card-kpi-icon-bg-purple: rgba(168, 85, 247, 0.15);
  --card-kpi-icon-green: #10b981;
  --card-kpi-icon-blue: #3b82f6;
  --card-kpi-icon-purple: #a855f7;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - etiquetas auxiliares, placeholders */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario */
  --text-base: 1rem;     /* 16px - texto de inputs, contenido */
  --text-lg: 1.125rem;   /* 18px - subtitulos de seccion */
  --text-xl: 1.25rem;    /* 20px - titulos de card */
  --text-2xl: 1.5rem;    /* 24px - nombre del promotor en header */
  --text-3xl: 1.875rem;  /* 30px - titulos de pagina */
  --text-4xl: 2.25rem;   /* 36px - valor de KPI card */

  /* Pesos */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Interlineado */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
  --leading-relaxed: 1.75;
}
```

### Espaciado

```css
:root {
  --space-1: 0.25rem;   /* 4px */
  --space-2: 0.5rem;    /* 8px */
  --space-3: 0.75rem;   /* 12px */
  --space-4: 1rem;      /* 16px */
  --space-5: 1.25rem;   /* 20px */
  --space-6: 1.5rem;    /* 24px */
  --space-8: 2rem;      /* 32px */
  --space-10: 2.5rem;   /* 40px */
  --space-12: 3rem;     /* 48px */
  --space-16: 4rem;     /* 64px */
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;  /* 6px */
  --radius-md: 0.5rem;    /* 8px */
  --radius-lg: 0.75rem;   /* 12px */
  --radius-xl: 1rem;      /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.35);
  --shadow-glow-green: 0 0 12px rgba(16, 185, 129, 0.25);
}
```

---

## Pantalla: Registro de Promotor

**Mockup:** N/A (patron visual de WPR_4-Login.png adaptado)
**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/registro`
**Template base:** krowd / patron auth de WPR_4

**Precondicion:** Usuario autenticado con rol Fan. Si ya tiene perfil de promotor (activo o inactivo), redirigir inmediatamente a `/promotor/dashboard`.

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│  HEADER: [Logo WePlay Rises]  Explorar  Artistas  [CTA btn] │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│              [Icono megafono / estrella - gradient]          │
│              Conviertete en Promotor                         │
│              Difunde la musica que amas y gana comisiones    │
│                                                              │
│         ┌────────────────────────────────────────┐           │
│         │                                        │           │
│         │  Nombre publico *                      │           │
│         │  [__________________________________]  │           │
│         │                                        │           │
│         │  Tipo de promotor *                    │           │
│         │  [Fan Embajador                    v]  │           │
│         │                                        │           │
│         │  Email de contacto                     │           │
│         │  [__________________________________]  │           │
│         │                                        │           │
│         │  Sitio web                             │           │
│         │  [__________________________________]  │           │
│         │                                        │           │
│         │  Redes sociales                        │           │
│         │  [ig] Instagram  [_________________]   │           │
│         │  [tt] TikTok     [_________________]   │           │
│         │  [yt] YouTube    [_________________]   │           │
│         │  [tw] Twitter/X  [_________________]   │           │
│         │                                        │           │
│         │  [i] Agrega al menos una red social    │           │
│         │      para mejorar tu visibilidad       │           │
│         │                                        │           │
│         │  [Cancelar]        [Crear perfil  ->]  │           │
│         │                                        │           │
│         └────────────────────────────────────────┘           │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Header de pagina | `<header>` con nav existente | Reutilizar header de landing: `bg-[#0d0d1a] border-b border-[#334155] sticky top-0 z-50` |
| Icono hero | Icono SVG megafono o `<Megaphone>` de lucide-react | `w-14 h-14 p-3 rounded-xl bg-gradient-to-br from-pink-500 to-purple-600 text-white mx-auto mb-4` |
| Titulo de pagina | `<h1>` | `text-3xl font-bold text-white text-center mb-2` |
| Subtitulo | `<p>` | `text-base text-[#94a3b8] text-center mb-8 max-w-sm mx-auto` |
| Contenedor del formulario | `<Card>` | `max-w-lg mx-auto bg-[#151525] border border-[#334155] rounded-xl shadow-lg` |
| Cuerpo del formulario | `<CardContent>` | `p-8` |
| Formulario | `<Form>` (react-hook-form + zodResolver) | `space-y-5` |
| Label requerido | `<Label>` con asterisco | `text-sm font-medium text-[#cbd5e1] mb-1.5 block after:content-['*'] after:text-red-500 after:ml-1` |
| Label opcional | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Input nombre publico | `<Input>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11` |
| Selector tipo promotor | `<Select>` + `<SelectTrigger>` + `<SelectContent>` + `<SelectItem>` x4 | Trigger: `bg-[#0f0f1f] border-[#334155] text-white h-11` / Content: `bg-[#151525] border-[#334155]` / Item hover: `bg-[#1e1e38] text-white` |
| Input email | `<Input type="email">` | Igual que input nombre publico |
| Input sitio web | `<Input type="url">` | Igual que input nombre publico, placeholder `https://tusitio.com` |
| Cabecera de seccion redes | `<p>` | `text-sm font-medium text-[#cbd5e1] mb-3` |
| Fila de red social | `<div>` | `flex items-center gap-3 mb-3` |
| Icono de red social | `<span>` con SVG o icono lucide | `w-8 h-8 rounded-md flex items-center justify-center bg-[#1e1e38] text-[#94a3b8] flex-shrink-0` |
| Label de red social inline | `<span>` | `text-sm text-[#94a3b8] w-20 flex-shrink-0` |
| Input URL red social | `<Input type="url">` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-10 flex-1` |
| Aviso sin redes (FA-03) | `<div>` con icono Info | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-1` |
| Mensaje de error de campo | `<p>` con role="alert" | `text-xs text-red-400 mt-1 flex items-center gap-1` (icono AlertCircle + texto) |
| Contenedor de botones | `<div>` | `flex items-center justify-between pt-2` |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Crear perfil | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11` |
| Spinner en boton | `<Loader2>` lucide-react | `w-4 h-4 animate-spin mr-2` (visible solo en estado loading) |

### Opciones del selector Tipo de Promotor

| Valor (id) | Etiqueta visible |
|------------|-----------------|
| `1` | Fan Embajador |
| `2` | Influencer |
| `3` | Medio / Blog |
| `4` | Profesional Marketing |

Los items se cargan desde el endpoint de maestras. Mientras carga, el `<SelectTrigger>` muestra un estado skeleton. Si la carga falla, el select queda deshabilitado con texto "No se pudo cargar".

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Formulario vacio con placeholders. Boton "Crear perfil" habilitado. Aviso de redes sociales oculto. |
| **Loading maestras** | Selector tipo promotor muestra skeleton animado (pulse). Resto del formulario accesible. |
| **Error maestras** | Selector deshabilitado, `bg-[#1a1a2e]`, texto "No se pudo cargar los tipos". Badge de error inline junto al selector. |
| **Focus input** | Border cambia a `#a855f7`, sombra sutil `0 0 0 3px rgba(168,85,247,0.15)`. Transicion 200ms. |
| **Typing** | Validacion onBlur para URL fields. Validacion onChange para nombre publico (debounce 300ms). |
| **Sin redes (FA-03)** | Al perder foco en el ultimo campo de red social sin haber rellenado ninguna, aparece el aviso informativo azul. No bloquea el envio. |
| **URL invalida (FA-04)** | Border rojo en el input, icono AlertCircle + mensaje de error debajo. Boton queda deshabilitado. |
| **Submitting** | Boton muestra spinner + texto "Creando perfil...", todos los inputs `disabled`, opacidad del form a 80%. |
| **Error API** | Toast destructivo en esquina superior derecha. Formulario se rehabilita con los datos intactos para reintento (FA-05). |
| **Success** | Toast verde "Perfil de promotor creado correctamente", redireccion automatica a `/promotor/dashboard` tras 1.5s. |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de error |
|-------|------------|------------------|
| Nombre publico | No vacio | "El nombre publico es obligatorio" |
| Nombre publico | Min 3 caracteres | "El nombre debe tener al menos 3 caracteres" |
| Nombre publico | Max 200 caracteres | "Maximo 200 caracteres" |
| Tipo de promotor | Seleccion obligatoria | "Selecciona un tipo de promotor" |
| Email de contacto | Formato email valido (si se proporciona) | "Ingresa un email valido" |
| Email de contacto | Max 200 caracteres | "Maximo 200 caracteres" |
| Sitio web | Formato URL valido (si se proporciona) | "Ingresa una URL valida (ej: https://tusitio.com)" |
| Sitio web | Max 300 caracteres | "Maximo 300 caracteres" |
| Instagram URL | Formato URL valido (si se proporciona) | "Ingresa una URL valida de Instagram" |
| TikTok URL | Formato URL valido (si se proporciona) | "Ingresa una URL valida de TikTok" |
| YouTube URL | Formato URL valido (si se proporciona) | "Ingresa una URL valida de YouTube" |
| Twitter/X URL | Formato URL valido (si se proporciona) | "Ingresa una URL valida de Twitter/X" |
| Todas las URLs | Max 300 caracteres | "Maximo 300 caracteres" |

### Zod Schema (Shared)

```typescript
// src/shared/schemas/promotor.schema.ts

const urlOptionalSchema = z
  .string()
  .max(300, "Maximo 300 caracteres")
  .refine(
    (val) => val === "" || z.string().url().safeParse(val).success,
    "Ingresa una URL valida"
  )
  .optional()
  .or(z.literal(""));

export const createPromotorSchema = z.object({
  nombrePublico: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  tipoPromotorId: z
    .number({ required_error: "Selecciona un tipo de promotor" })
    .int()
    .positive(),
  emailContacto: z
    .string()
    .max(200, "Maximo 200 caracteres")
    .email("Ingresa un email valido")
    .optional()
    .or(z.literal("")),
  urlSitioWeb: urlOptionalSchema,
  urlInstagram: urlOptionalSchema,
  urlTikTok: urlOptionalSchema,
  urlYoutube: urlOptionalSchema,
  urlTwitter: urlOptionalSchema,
});

export type CreatePromotorFormData = z.infer<typeof createPromotorSchema>;
```

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Click boton Cancelar | Navegar de vuelta a la pagina anterior (router.back()) o a `/` si no hay historial |
| Submit formulario | Validar con Zod, si OK: POST a `/api/promotores`, mostrar toast success, redirect a `/promotor/dashboard` |
| Perder foco en campos URL | Validacion onBlur inmediata con mensaje de error si formato invalido |
| Perder foco en ultimo campo de red social sin rellenar ninguno | Mostrar aviso informativo FA-03 con animacion fade-in |
| Error de red / 500 | Toast error manteniendo datos del formulario. Sin redirect. |
| Error 409 (promotor ya existe) | Este caso no deberia ocurrir (hay redirect previo), pero si llega: toast "Ya tienes un perfil de promotor" + redirect a `/promotor/dashboard` |
| Carga exitosa de maestras | Selector se activa con las opciones cargadas, primer item preseleccionado |
| Usuario no autenticado | Redirect a `/auth/login?returnUrl=/promotor/registro` antes de renderizar el formulario |

---

## Pantalla: Dashboard de Promotor

**Mockup:** Basado en WPR_5-Dashboard-Artist.png (mismo patron de sidebar + KPI cards)
**Proyecto:** Admin (Next.js 14)
**Ruta:** `/promotor/dashboard`
**Template base:** `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/`

### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a])                                    │
│ ┌──────────────────┐  ┌───────────────────────────────────────┐ │
│ │ [Logo WePlay]    │  │ TOPBAR: breadcrumb + acciones         │ │
│ │ WePlay Rises     │  ├───────────────────────────────────────┤ │
│ ├──────────────────┤  │                                       │ │
│ │ [Avatar]         │  │  Hola, {nombrePublico}                │ │
│ │ {nombrePublico}  │  │  {fechaHoy}         [Editar perfil]   │ │
│ │ Promotor         │  │                                       │ │
│ ├──────────────────┤  │  ┌───────────┐ ┌──────────┐ ┌──────┐  │ │
│ │ > Dashboard      │  │  │Programas  │ │ Tareas   │ │Wallet│  │ │
│ │   Mis Programas  │  │  │activos    │ │pendientes│ │saldo │  │ │
│ │   Comisiones     │  │  │  [  3  ]  │ │ [ 12  ] │ │150.5E│  │ │
│ │   Mi Perfil      │  │  └───────────┘ └──────────┘ └──────┘  │ │
│ │ ──────────────── │  │                                       │ │
│ │   Configuracion  │  │  ┌───────────────────────────────────┐│ │
│ │ [Logout]         │  │  │ Mi Perfil de Promotor             ││ │
│ └──────────────────┘  │  │                                   ││ │
│                       │  │  [Avatar initials]                ││ │
│                       │  │  {nombrePublico}   [ACTIVO badge] ││ │
│                       │  │  {tipoPromotor}                   ││ │
│                       │  │                                   ││ │
│                       │  │  Email: {emailContacto o --}      ││ │
│                       │  │  Web:   {urlSitioWeb o --}        ││ │
│                       │  │  Redes: [ig][tt][yt][tw] icons    ││ │
│                       │  │                                   ││ │
│                       │  │  [Editar perfil] [Ver programas]  ││ │
│                       │  └───────────────────────────────────┘│ │
│                       └───────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Especificaciones

**Sidebar**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Sidebar contenedor | `<aside>` | `w-64 min-h-screen bg-[#0d0d1a] border-r border-[#334155] flex flex-col` |
| Logo area | `<div>` con logo SVG | `p-6 flex items-center gap-3 border-b border-[#334155]` |
| Logo icono | SVG / icono musica | `w-8 h-8 p-1.5 rounded-lg bg-gradient-to-br from-pink-500 to-purple-600 text-white` |
| Logo texto | `<span>` | `text-lg font-bold text-white` |
| Avatar usuario | `<Avatar>` | `w-10 h-10 rounded-full` con `<AvatarFallback>` iniciales |
| Nombre usuario | `<p>` | `text-sm font-medium text-white` |
| Rol | `<p>` | `text-xs text-[#94a3b8]` con texto "Promotor" |
| Nav item activo | `<Link>` + `<Button variant="ghost">` | `w-full justify-start text-white bg-[#1e1e38] border-l-2 border-[#a855f7]` |
| Nav item inactivo | `<Link>` + `<Button variant="ghost">` | `w-full justify-start text-[#94a3b8] hover:text-white hover:bg-[#1a1a2e]` |
| Nav icono | lucide-react icon | `w-4 h-4 mr-3` |
| Logout | `<Button variant="ghost">` | `w-full justify-start text-red-400 hover:text-red-300 hover:bg-red-950/30` |

**Topbar y cabecera de pagina**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Topbar | `<header>` | `h-16 bg-[#0d0d1a] border-b border-[#334155] flex items-center px-6 gap-4` |
| Breadcrumb | `<nav aria-label="breadcrumb">` | `text-sm text-[#94a3b8]` / separador `/` / item activo `text-white` |
| Saludo | `<h1>` | `text-2xl font-bold text-white` |
| Fecha | `<p>` | `text-sm text-[#94a3b8] mt-0.5` |
| Boton Editar perfil | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-5` |

**KPI Cards** (misma estructura que WPR_5-Dashboard-Artist.png)

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid de KPI | `<div>` | `grid grid-cols-3 gap-4 mb-6` |
| KPI Card | `<Card>` | `bg-[#151525] border-[#334155] p-5` |
| Icono KPI | `<div>` con icono lucide | `w-10 h-10 rounded-lg flex items-center justify-center mb-3` |
| KPI Programas activos | `<Activity>` icon | Icono container: `bg-[rgba(16,185,129,0.15)]`, icono: `text-[#10b981] w-5 h-5` |
| KPI Tareas pendientes | `<ClipboardList>` icon | Icono container: `bg-[rgba(59,130,246,0.15)]`, icono: `text-[#3b82f6] w-5 h-5` |
| KPI Wallet saldo | `<Wallet>` icon | Icono container: `bg-[rgba(168,85,247,0.15)]`, icono: `text-[#a855f7] w-5 h-5` |
| Valor KPI | `<p>` | `text-3xl font-bold text-white` |
| Label KPI | `<p>` | `text-sm text-[#94a3b8] mt-1` |

Formato del valor de wallet: `€{saldo}` con 2 decimales. Si saldo es 0: mostrar `€0.00`.

**Card de Perfil del Promotor**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] p-6` |
| Card titulo | `<h2>` | `text-lg font-semibold text-white mb-4` |
| Avatar initials | `<Avatar>` | `w-16 h-16` con `<AvatarFallback className="bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold">` |
| Nombre | `<p>` | `text-xl font-semibold text-white mt-3` |
| Badge de estado | `<Badge>` | Estado ACTIVO: `bg-green-950/50 text-green-400 border border-green-800/50` / Estado INACTIVO: `bg-slate-800 text-slate-400 border border-slate-700` |
| Tipo promotor | `<p>` | `text-sm text-[#94a3b8] mt-1` |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Email label | `<span>` | `text-xs text-[#64748b] uppercase tracking-wider` |
| Email valor | `<p>` o `<span>` | `text-sm text-[#cbd5e1]` / Si no hay valor: `text-[#64748b] italic` con "--" |
| Web label | igual que Email label | |
| Web valor | `<a>` | `text-sm text-[#a855f7] hover:underline truncate max-w-[200px] inline-block` |
| Iconos de redes | `<a>` con icono SVG | `w-8 h-8 flex items-center justify-center rounded-md bg-[#1e1e38] text-[#94a3b8] hover:text-white hover:bg-[#334155] transition-colors` (solo iconos de redes con URL) |
| Boton Editar perfil | `<Button variant="outline">` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white` |
| Boton Ver programas | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | KPI cards muestran skeleton (pulse animation): bloque gris `h-8 rounded bg-[#1e1e38] animate-pulse`. Card de perfil muestra skeleton para nombre, badge y campos. |
| **Default / cargado** | Todos los datos visibles. KPI values con numeros reales. |
| **Perfil ACTIVO** | Badge verde "ACTIVO" en la card de perfil. |
| **Perfil INACTIVO** | Badge gris "INACTIVO". Boton "Ver programas" deshabilitado. Banner informativo amarillo en la parte superior del main: "Tu perfil esta desactivado. [Reactivar cuenta]" |
| **Sin redes sociales** | La fila de iconos de redes no se renderiza. Se muestra texto muted "Sin redes sociales configuradas". |
| **Error de carga** | Sustituir las KPI cards y la card de perfil por un mensaje de error con boton "Reintentar". |
| **Wallet en 0** | Mostrar `€0.00` sin estilo especial. |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Click en "Editar perfil" (topbar o card) | Navegar a `/promotor/perfil` |
| Click en "Ver programas" | Navegar a `/promotor/programas` (fuera de scope de US-CP-01, puede estar deshabilitado si no existe aun) |
| Click en icono de red social | Abrir URL en nueva pestana (`target="_blank" rel="noopener noreferrer"`) |
| Click en "Reactivar cuenta" (banner inactivo) | Navegar a `/promotor/perfil` con un query param `?action=reactivate` |
| Hover en nav items | Transicion de color y fondo 150ms ease |
| Click logout | Borrar JWT, redirect a `/` |

---

## Pantalla: Edicion de Perfil de Promotor

**Mockup:** Basado en patron dashboard WPR_5 (formulario dentro del layout admin)
**Proyecto:** Admin (Next.js 14)
**Ruta:** `/promotor/perfil`
**Template base:** `dashtail/` (layout dashboard + form patterns)

### Layout

```
┌─────────────────────────────────────────────────────────────────┐
│ SIDEBAR (igual que dashboard)                                   │
│ ┌──────────────────┐  ┌───────────────────────────────────────┐ │
│ │ (sidebar)        │  │ TOPBAR                                │ │
│ │                  │  ├───────────────────────────────────────┤ │
│ │ > Dashboard      │  │                                       │ │
│ │   Mis Programas  │  │  Editar Perfil de Promotor            │ │
│ │   Comisiones     │  │  Actualiza tu informacion publica     │ │
│ │ * Mi Perfil [->] │  │                                       │ │
│ │                  │  │  ┌─────────────────────────────────┐  │ │
│ └──────────────────┘  │  │ Informacion publica             │  │ │
│                       │  │                                 │  │ │
│                       │  │ Nombre publico *                │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  │                                 │  │ │
│                       │  │ Tipo de promotor (no editable)  │  │ │
│                       │  │ [Fan Embajador          [lock]] │  │ │
│                       │  │                                 │  │ │
│                       │  │ Email de contacto               │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  │                                 │  │ │
│                       │  │ Sitio web                       │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  ┌─────────────────────────────────┐  │ │
│                       │  │ Redes sociales                  │  │ │
│                       │  │                                 │  │ │
│                       │  │ [ig] Instagram                  │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  │ [tt] TikTok                     │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  │ [yt] YouTube                    │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  │ [tw] Twitter/X                  │  │ │
│                       │  │ [_____________________________] │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  ┌─────────────────────────────────┐  │ │
│                       │  │ ZONA DE PELIGRO                 │  │ │
│                       │  │ [!] Desactivar cuenta           │  │ │
│                       │  │     Al desactivar...texto.      │  │ │
│                       │  │     [Desactivar cuenta]         │  │ │
│                       │  └─────────────────────────────────┘  │ │
│                       │                                       │ │
│                       │  [Cancelar]     [Guardar cambios ->]  │ │
│                       └───────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Especificaciones

**Card: Informacion publica**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card principal | `<Card>` | `bg-[#151525] border-[#334155] mb-6` |
| Card header | `<CardHeader>` | `border-b border-[#334155] pb-4` |
| Card titulo | `<CardTitle>` | `text-lg font-semibold text-white` |
| Card body | `<CardContent>` | `pt-6 space-y-5` |
| Input nombre publico | `<Input>` | `bg-[#0f0f1f] border-[#334155] text-white h-11 focus:border-[#a855f7]` pre-rellenado con valor actual |
| Campo tipo (solo lectura) | `<Input disabled>` con icono Lock | `bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed h-11` + icono `<Lock w-4 h-4 text-[#64748b]>` a la derecha |
| Nota explicativa bajo tipo | `<p>` | `text-xs text-[#64748b] mt-1` texto: "El tipo de promotor no puede modificarse una vez creado el perfil" |
| Input email | `<Input type="email">` | Igual que nombre publico, pre-rellenado |
| Input sitio web | `<Input type="url">` | Igual que nombre publico, pre-rellenado |

**Card: Redes sociales**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card redes | `<Card>` | `bg-[#151525] border-[#334155] mb-6` |
| Layout de cada red | `<FormField>` con label + input | `space-y-1.5` |
| Label de cada red | `<Label>` con icono de red | `flex items-center gap-2 text-sm font-medium text-[#cbd5e1]` |
| Icono de red | Icono SVG de cada plataforma | `w-4 h-4` con color propio de cada plataforma |
| Input URL | `<Input type="url">` | Igual que campos de informacion publica, pre-rellenado |

**Card: Zona de peligro**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card danger zone | `<Card>` | `bg-[#151525] border border-red-900/50 mb-8` |
| Card header | `<CardHeader>` | `border-b border-red-900/50` |
| Titulo zona peligro | `<CardTitle>` | `text-base font-semibold text-red-400 flex items-center gap-2` con icono `<AlertTriangle w-4 h-4>` |
| Descripcion | `<p>` | `text-sm text-[#94a3b8] mb-4` texto: "Al desactivar tu cuenta de promotor, seras dado de baja automaticamente de todos los programas activos en los que participas." |
| Boton desactivar | `<Button variant="outline">` | `border-red-800 text-red-400 hover:bg-red-950/50 hover:text-red-300 hover:border-red-700` |

**Botones de accion principales**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor sticky | `<div>` | `flex items-center justify-end gap-3 pt-4 border-t border-[#334155]` |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Guardar cambios | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11` |

### Zod Schema (Shared, reutilizado)

```typescript
// src/shared/schemas/promotor.schema.ts (extension del schema de creacion)

export const updatePromotorSchema = z.object({
  nombrePublico: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  emailContacto: z
    .string()
    .max(200, "Maximo 200 caracteres")
    .email("Ingresa un email valido")
    .optional()
    .or(z.literal("")),
  urlSitioWeb: urlOptionalSchema,
  urlInstagram: urlOptionalSchema,
  urlTikTok: urlOptionalSchema,
  urlYoutube: urlOptionalSchema,
  urlTwitter: urlOptionalSchema,
  // tipoPromotorId: NO incluido (campo no editable, AC-CP01-9)
});

export type UpdatePromotorFormData = z.infer<typeof updatePromotorSchema>;
```

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | Los inputs muestran skeleton animado mientras se carga el perfil actual. Botones deshabilitados. |
| **Pre-rellenado** | Todos los campos con datos actuales. Input tipo promotor claramente deshabilitado (fondo mas oscuro, icono Lock visible). |
| **Sin cambios** | Boton "Guardar cambios" deshabilitado (deteccion de dirty state con react-hook-form). |
| **Con cambios** | Boton "Guardar cambios" habilitado y con su estilo gradient normal. |
| **Saving** | Boton muestra spinner + "Guardando...", inputs disabled. |
| **Success** | Toast verde "Perfil actualizado correctamente". Formulario se mantiene visible con datos actualizados. Boton vuelve a estado deshabilitado (sin cambios pendientes). |
| **Error API** | Toast rojo con descripcion del error. Formulario se rehabilita. |
| **Error validacion URL** | Border rojo en el input afectado, mensaje debajo. Boton "Guardar" deshabilitado. |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Edicion de cualquier campo | Habilitar boton "Guardar cambios" (dirty state) |
| Click Cancelar (sin cambios) | Navegar a `/promotor/dashboard` |
| Click Cancelar (con cambios) | Mostrar `<AlertDialog>` de confirmacion: "Tienes cambios sin guardar. ¿Seguro que quieres salir?" con opciones "Continuar editando" / "Salir sin guardar" |
| Click Guardar cambios | Validar schema, si OK: PUT a `/api/promotores/{id}`, toast success |
| Click Desactivar cuenta | Abrir dialogo de confirmacion de desactivacion |

---

## Dialogo: Confirmacion de Desactivacion

Este dialogo aparece tanto desde `/promotor/perfil` como potencialmente desde otros puntos de acceso a la desactivacion.

### Layout del Dialogo

```
┌──────────────────────────────────────────┐
│  [!] Desactivar cuenta de promotor       │
│ ──────────────────────────────────────── │
│                                          │
│  Caso A - Con programas activos:         │
│  "Estas participando en 3 programas de   │
│   promocion activos. Al desactivar tu    │
│   cuenta seras dado de baja de todos     │
│   ellos automaticamente."               │
│                                          │
│  Caso B - Sin programas activos:         │
│  "¿Seguro que quieres desactivar tu      │
│   cuenta de promotor? Esta accion        │
│   desactivara tu perfil pero no lo       │
│   eliminara. Podras reactivarlo          │
│   contactando con soporte."             │
│                                          │
│  [Cancelar]    [Desactivar cuenta]       │
└──────────────────────────────────────────┘
```

### Especificaciones del Dialogo

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialogo | `<AlertDialog>` shadcn/ui | `max-w-md` |
| Overlay | `<AlertDialogOverlay>` | `bg-black/60 backdrop-blur-sm` |
| Contenedor | `<AlertDialogContent>` | `bg-[#151525] border border-[#334155] p-6` |
| Icono de advertencia | `<AlertTriangle>` lucide | `w-10 h-10 text-red-400 mb-3` (solo en Caso A con programas activos) |
| Titulo | `<AlertDialogTitle>` | `text-lg font-semibold text-white` |
| Descripcion | `<AlertDialogDescription>` | `text-sm text-[#94a3b8] mt-2` |
| Numero de programas (caso A) | `<strong>` | `text-white font-semibold` dentro del parrafo de descripcion |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Boton Cancelar | `<AlertDialogCancel>` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton Confirmar | `<AlertDialogAction>` | `bg-red-600 hover:bg-red-700 text-white font-semibold` |
| Boton confirmando | `<AlertDialogAction>` | Mismo estilo + spinner `<Loader2 animate-spin>` + texto "Desactivando..." |

### Estados del Dialogo

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Abriendo** | Fade in + scale 0.95 → 1 en 200ms |
| **Cargando numero de programas** | Descripcion del dialogo muestra skeleton de una linea mientras se hace GET de programas activos. |
| **Con programas activos** | Icono de advertencia visible. Texto incluye el numero de programas afectados. |
| **Sin programas activos** | Sin icono de advertencia. Texto de confirmacion simple. |
| **Confirmando** | Boton muestra spinner. Boton Cancelar deshabilitado. |
| **Error** | Cerrar dialogo. Toast rojo con mensaje de error. |
| **Success** | Cerrar dialogo. Toast "Perfil de promotor desactivado". Redirect a `/`. |

---

## Tipos TypeScript (Shared)

```typescript
// src/shared/types/promotor.types.ts

export interface MaestraTipoPromotor {
  id: number;
  nombre: string;
}

export interface PromotorDto {
  id: string;
  userId: string;
  nombrePublico: string;
  tipoPromotorId: number;
  tipoPromotorNombre: string;
  emailContacto: string | null;
  urlSitioWeb: string | null;
  urlInstagram: string | null;
  urlTikTok: string | null;
  urlYoutube: string | null;
  urlTwitter: string | null;
  esActivo: boolean;
  fechaCreacion: string;
  fechaActualizacion: string | null;
  wallet: PromotorWalletDto | null;
}

export interface PromotorWalletDto {
  id: string;
  moneda: string;       // "EUR"
  saldo: number;
}

export interface PromotorDashboardStatsDto {
  programasActivos: number;
  tareasPendientes: number;
  walletSaldo: number;
  walletMoneda: string;
}
```

---

## Responsive Breakpoints

| Breakpoint | Width | Descripcion |
|------------|-------|-------------|
| **Mobile** | < 640px | Stack completo |
| **Tablet** | 640 - 1024px | Sidebar colapsado |
| **Desktop** | > 1024px | Layout completo con sidebar visible |

### Pantalla: Registro (/promotor/registro) - Responsive

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Card ocupa full width con `mx-4`. Padding interno reducido a `p-5`. Titulo `text-2xl`. Las filas de redes sociales: label en una linea, input debajo (stack vertical). Botones full width apilados verticalmente (Crear perfil arriba, Cancelar abajo). |
| **Tablet (640 - 1024px)** | Card con `max-w-lg mx-auto`. Redes sociales en layout horizontal (label + input en la misma fila). Botones en fila. |
| **Desktop (> 1024px)** | Card `max-w-lg mx-auto`. Layout horizontal completo para redes sociales. |

### Pantalla: Dashboard (/promotor/dashboard) - Responsive

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar oculto. Menu hamburguesa en topbar que abre un drawer lateral. Grid KPI: `grid-cols-1` (apilado). Card de perfil: `flex-col` (Avatar centrado arriba). Botones de accion: full width apilados. |
| **Tablet (640 - 1024px)** | Sidebar colapsado a iconos (w-16). Grid KPI: `grid-cols-2` (ultimo card full width o en nueva fila). Card de perfil en columna completa. |
| **Desktop (> 1024px)** | Sidebar expandido (w-64). Grid KPI: `grid-cols-3`. Card de perfil con layout horizontal (Avatar a la izquierda, datos a la derecha). |

### Pantalla: Edicion (/promotor/perfil) - Responsive

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Sidebar oculto (drawer). Cards apiladas verticalmente con padding `p-4`. Zona de peligro visible al final del scroll. Botones de accion sticky al fondo: `fixed bottom-0 w-full bg-[#0d0d1a] border-t border-[#334155] p-4 flex gap-3`. |
| **Tablet (640 - 1024px)** | Sidebar colapsado. Cards con padding normal. Botones en footer del contenido (no sticky). |
| **Desktop (> 1024px)** | Layout completo. Cards en columna central `max-w-2xl`. Botones al final del formulario alineados a la derecha. |

---

## Animaciones

| Elemento | Animacion | Duracion |
|----------|-----------|----------|
| Hover en botones primarios (gradient) | Scale 1.02 + shadow glow `rgba(168,85,247,0.4)` | 150ms ease |
| Hover en botones secundarios / ghost | Background lighten | 150ms ease |
| Focus en inputs | Border color a `#a855f7` + ring `rgba(168,85,247,0.15)` | 200ms ease |
| Transicion de pagina | Fade in `opacity: 0 → 1` | 250ms ease-in-out |
| Toast notification | Slide in desde esquina superior derecha + fade in | 300ms ease-out |
| Toast salida | Fade out + slide out | 200ms ease-in |
| KPI card hover | Background `#1e1e38`, sombra ligera | 200ms ease |
| Dialogo apertura | Scale `0.95 → 1` + fade in | 200ms ease-out |
| Dialogo cierre | Scale `1 → 0.95` + fade out | 150ms ease-in |
| Skeleton loading | Pulse animation `opacity 0.5 ↔ 1` | 1500ms ease-in-out infinite |
| Error de validacion en input | Shake horizontal 4px x2 + fade in del mensaje de error | 400ms ease |
| Badge estado ACTIVO | Ninguna animacion, estatico | - |
| Aviso redes sociales (FA-03) | Fade in desde `opacity: 0` | 300ms ease |
| Icono de red social hover | Color a `#ffffff`, fondo a `#334155` | 150ms ease |

### Keyframe de Shake (Validacion)

```css
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  20%       { transform: translateX(-4px); }
  60%       { transform: translateX(4px); }
  80%       { transform: translateX(-2px); }
}
```

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1. Blanco `#ffffff` sobre `#151525` = 16.1:1. `#94a3b8` sobre `#151525` = 4.8:1. Ambos superan el minimo WCAG AA. |
| Focus visible | Ring de 2px con color `#a855f7` y offset de 2px en todos los elementos interactivos: `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#151525]` |
| Labels en inputs | Todos los inputs tienen `<Label htmlFor="id">` asociado. Los inputs tienen `id` correspondiente. |
| Campos requeridos | `aria-required="true"` en inputs obligatorios. Asterisco `*` junto al label (no solo via CSS). |
| Mensajes de error | `role="alert"` y `aria-live="polite"` en contenedores de errores de campo. `aria-invalid="true"` en el input con error. `aria-describedby` apuntando al id del mensaje de error. |
| Selector tipo promotor | El `<Select>` de shadcn/ui implementa WAI-ARIA combobox. Asegurar que el `<Label>` este asociado con el trigger via `htmlFor`. |
| Campo solo lectura | Input tipo promotor usa `disabled` (no `readOnly`) con nota explicativa. `aria-disabled="true"` y texto visible que indica no es editable. |
| Botones con estado | Boton "Guardar cambios" usa `aria-disabled="true"` cuando no hay cambios. Boton "Crear perfil" / "Guardar" usa `aria-busy="true"` durante loading. |
| Iconos de redes sociales | Links de redes tienen `aria-label="Ver perfil en {red}"` (ej: "Ver perfil en Instagram"). Iconos tienen `aria-hidden="true"`. |
| Dialogo | `<AlertDialog>` de shadcn/ui implementa ARIA dialog role. Focus atrapado dentro del dialogo mientras esta abierto. Esc cierra el dialogo. |
| Sidebar navigation | `<nav aria-label="Menu principal">`. Item activo: `aria-current="page"`. |
| Loading states | Skeletons tienen `aria-busy="true"` en el contenedor. Screen readers anuncian "Cargando..." con `aria-live="polite"`. |
| Keyboard navigation | Tab order logico: header → sidebar → contenido principal. Enter activa botones y links. Esc cierra dialogos y dropdowns. |
| Color no es unico indicador | Los errores siempre usan icono `<AlertCircle>` + color rojo + texto del mensaje. El estado ACTIVO usa icono `<CheckCircle>` + color verde + texto "ACTIVO". |

### Ejemplo de ARIA en campos de formulario

```tsx
<div>
  <Label htmlFor="nombrePublico">
    Nombre publico <span aria-hidden="true" className="text-red-500 ml-1">*</span>
    <span className="sr-only">(requerido)</span>
  </Label>
  <Input
    id="nombrePublico"
    aria-required="true"
    aria-invalid={!!errors.nombrePublico}
    aria-describedby={errors.nombrePublico ? "nombrePublico-error" : undefined}
    {...register("nombrePublico")}
  />
  {errors.nombrePublico && (
    <p id="nombrePublico-error" role="alert" className="text-xs text-red-400 mt-1 flex items-center gap-1">
      <AlertCircle className="w-3 h-3" aria-hidden="true" />
      {errors.nombrePublico.message}
    </p>
  )}
</div>

<Button
  type="submit"
  disabled={isSubmitting}
  aria-busy={isSubmitting}
>
  {isSubmitting && <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />}
  {isSubmitting ? "Creando perfil..." : "Crear perfil"}
</Button>
```

---

## Checklist UI/UX

### Registro de Promotor (/promotor/registro - Landing)
- [ ] Header de landing reutilizado (sticky, con nav)
- [ ] Icono hero gradient visible
- [ ] Titulo y subtitulo centrados
- [ ] Card con fondo `#151525` y border `#334155`
- [ ] Campo nombre publico: Input con validacion min 3, max 200
- [ ] Selector tipo promotor: carga maestras desde API, primer item preseleccionado
- [ ] Estado loading del selector (skeleton)
- [ ] Estado error del selector (deshabilitado con mensaje)
- [ ] Campo email: validacion formato email (opcional)
- [ ] Campo sitio web: validacion URL (opcional)
- [ ] 4 campos de redes sociales con icono de plataforma
- [ ] Validacion URL en redes (onBlur)
- [ ] Aviso informativo FA-03 cuando ninguna red rellenada
- [ ] Mensajes de error con icono AlertCircle + texto
- [ ] Boton Cancelar: ghost, navega atras
- [ ] Boton Crear perfil: gradient, spinner en loading
- [ ] Estado submitting: todos los inputs disabled
- [ ] Toast success + redirect a `/promotor/dashboard`
- [ ] Toast error (FA-05): datos del formulario se mantienen
- [ ] Redirect si ya tiene perfil (FA-01)
- [ ] Redirect si no autenticado → login
- [ ] Responsive mobile: stack vertical en redes, botones full width
- [ ] Focus ring purple visible en todos los inputs
- [ ] ARIA labels, aria-required, aria-invalid, aria-describedby
- [ ] Shake animation en primer error de submit

### Dashboard de Promotor (/promotor/dashboard - Admin)
- [ ] Sidebar con navegacion completa del promotor
- [ ] Item "Dashboard" marcado como activo (aria-current="page")
- [ ] Avatar con iniciales del nombre publico
- [ ] Topbar con saludo personalizado y fecha
- [ ] Boton "Editar perfil" en topbar
- [ ] 3 KPI cards: Programas activos, Tareas pendientes, Wallet saldo
- [ ] Iconos de KPI con colores distintos (verde, azul, purple)
- [ ] Skeleton loading en KPI cards
- [ ] Card de perfil del promotor con datos completos
- [ ] Badge ACTIVO / INACTIVO con colores correctos
- [ ] Campo tipo promotor visible (solo lectura)
- [ ] Iconos de redes sociales linkeados (solo los que tienen URL)
- [ ] Links de redes: target="_blank" rel="noopener noreferrer"
- [ ] Campos sin dato muestran "--" en muted
- [ ] Banner de alerta si perfil inactivo
- [ ] Boton "Ver programas" deshabilitado si perfil inactivo
- [ ] Error state con boton "Reintentar"
- [ ] Responsive mobile: hamburger menu, KPI en columna
- [ ] Responsive tablet: sidebar colapsado a iconos
- [ ] Hover effects en cards y botones
- [ ] ARIA labels en iconos de redes

### Edicion de Perfil (/promotor/perfil - Admin)
- [ ] Sidebar con "Mi Perfil" como item activo
- [ ] Titulo y subtitulo de pagina
- [ ] Card "Informacion publica" con todos los campos pre-rellenados
- [ ] Campo nombre publico editable con validacion
- [ ] Campo tipo promotor DESHABILITADO con icono Lock visible
- [ ] Nota explicativa bajo tipo promotor
- [ ] Campo email editable (opcional)
- [ ] Campo sitio web editable (opcional)
- [ ] Card "Redes sociales" con los 4 campos pre-rellenados
- [ ] Iconos de plataforma junto a cada label
- [ ] Card "Zona de peligro" con border rojo
- [ ] Boton "Desactivar cuenta" en zona de peligro (estilo outline rojo)
- [ ] Boton "Guardar cambios" deshabilitado cuando no hay cambios (dirty state)
- [ ] Boton "Guardar cambios" habilitado al detectar cambios
- [ ] Boton "Cancelar" con confirmacion si hay cambios sin guardar
- [ ] AlertDialog de confirmacion al cancelar con cambios
- [ ] Spinner en boton Guardar durante submit
- [ ] Toast success "Perfil actualizado correctamente"
- [ ] Toast error con datos del formulario mantenidos
- [ ] Skeleton loading inicial
- [ ] Responsive mobile: botones sticky al fondo
- [ ] ARIA completo en todos los campos

### Dialogo de Desactivacion
- [ ] AlertDialog de shadcn/ui
- [ ] Overlay con backdrop-blur
- [ ] Carga numero de programas activos antes de mostrar
- [ ] Skeleton en descripcion mientras carga
- [ ] Caso A (con programas): icono AlertTriangle + numero de programas en bold
- [ ] Caso B (sin programas): texto simple de confirmacion
- [ ] Boton Cancelar: cierra el dialogo sin accion
- [ ] Boton Desactivar: color rojo, spinner al confirmar
- [ ] Boton Cancelar deshabilitado durante la confirmacion
- [ ] Toast "Perfil de promotor desactivado" + redirect a `/`
- [ ] Toast error si falla la desactivacion
- [ ] Focus atrapado en el dialogo (accesibilidad)
- [ ] Esc cierra el dialogo
- [ ] Animacion de apertura/cierre

### Cross-cutting
- [ ] Dark theme aplicado consistentemente en todas las pantallas
- [ ] Design tokens usados (no valores hardcodeados fuera de los especificados)
- [ ] Todos los componentes de shadcn/ui (no HTML nativo)
- [ ] Zod schemas definidos en `src/shared/schemas/promotor.schema.ts`
- [ ] Tipos TypeScript definidos en `src/shared/types/promotor.types.ts`
- [ ] TanStack Query para fetch de datos (perfil, maestras, stats)
- [ ] react-hook-form + zodResolver en todos los formularios
- [ ] Animaciones smooth (150-300ms), no abrupto
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Keyboard navigation logica en todas las pantallas
- [ ] Loading skeletons coherentes con el layout final
- [ ] Toast notifications configuradas (success verde, error rojo)
- [ ] Mobile-first responsive design
