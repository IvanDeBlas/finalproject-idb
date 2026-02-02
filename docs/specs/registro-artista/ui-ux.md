# UI/UX: Registro de Artista

> **Feature:** registro-artista
> **Última actualización:** 2026-01-26

Este documento especifica el diseño visual y experiencia de usuario para la feature de registro de artista.

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Login | [WPR_4-Login.png](../../ui-images/WPR_4-Login.png) | Landing |
| Perfil Artista (público) | [WPR_8-Artist-Profile.png](../../ui-images/WPR_8-Artist-Profile.png) | Landing |
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Landing, páginas públicas | `references/templates/krowd/krowd-nextjs-main/` |
| **Dashtail** | Dashboard, panel de artista | `references/templates/dashtail/Dashtail/` |

---

## Design Tokens

### Paleta de Colores

```css
/* Basado en mockups */
:root {
  /* Background */
  --bg-primary: #0f0f1a;      /* Fondo principal oscuro */
  --bg-secondary: #1a1a2e;    /* Cards, sidebar */
  --bg-card: rgba(255, 255, 255, 0.05);  /* Cards transparentes */

  /* Primary (gradients) */
  --primary-pink: #ec4899;     /* pink-500 */
  --primary-purple: #8b5cf6;   /* purple-500 */
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #8b5cf6 100%);

  /* Secondary */
  --secondary-green: #10b981;  /* Para éxito, badges "Activa" */
  --secondary-blue: #3b82f6;   /* Links, acciones secundarias */

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #9ca3af;   /* gray-400 */
  --text-muted: #6b7280;       /* gray-500 */

  /* Status */
  --status-success: #10b981;   /* green-500 */
  --status-warning: #f59e0b;   /* amber-500 */
  --status-error: #ef4444;     /* red-500 */
  --status-info: #3b82f6;      /* blue-500 */

  /* Borders */
  --border-default: rgba(255, 255, 255, 0.1);
  --border-hover: rgba(255, 255, 255, 0.2);
}
```

### Tipografía

```css
/* Font family */
--font-sans: 'Inter', system-ui, sans-serif;

/* Sizes */
--text-xs: 0.75rem;    /* 12px */
--text-sm: 0.875rem;   /* 14px */
--text-base: 1rem;     /* 16px */
--text-lg: 1.125rem;   /* 18px */
--text-xl: 1.25rem;    /* 20px */
--text-2xl: 1.5rem;    /* 24px */
--text-3xl: 1.875rem;  /* 30px */
--text-4xl: 2.25rem;   /* 36px */

/* Weights */
--font-normal: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
```

### Espaciado

```css
/* Spacing scale (Tailwind default) */
--space-1: 0.25rem;   /* 4px */
--space-2: 0.5rem;    /* 8px */
--space-3: 0.75rem;   /* 12px */
--space-4: 1rem;      /* 16px */
--space-6: 1.5rem;    /* 24px */
--space-8: 2rem;      /* 32px */
--space-12: 3rem;     /* 48px */
```

### Border Radius

```css
--radius-sm: 0.25rem;  /* 4px */
--radius-md: 0.5rem;   /* 8px - botones, inputs */
--radius-lg: 0.75rem;  /* 12px - cards */
--radius-xl: 1rem;     /* 16px - modals */
--radius-full: 9999px; /* avatares */
```

---

## Pantalla: Login

**Mockup:** WPR_4-Login.png
**Proyecto:** Landing (`src/web`)
**Ruta:** `/auth/login`
**Template base:** `krowd/pages/auth/login`

### Layout

```
┌──────────────────────────────────────────────┐
│                                              │
│              🎵 MusicFund                    │  Logo + nombre
│                                              │
│         ┌─────────────────────┐              │
│         │  Bienvenido de      │              │  Card centrado
│         │      vuelta         │              │
│         │                     │              │
│         │  ┌───────────────┐  │              │
│         │  │ 📧 Email      │  │              │  Input con icono
│         │  └───────────────┘  │              │
│         │                     │              │
│         │  ┌───────────────┐  │              │
│         │  │ 🔒 ●●●●●●●● 👁│  │              │  Password con toggle
│         │  └───────────────┘  │              │
│         │                     │              │
│         │  ☐ Recordarme  Link │              │  Checkbox + link
│         │                     │              │
│         │  ┌───────────────┐  │              │
│         │  │ Iniciar sesión│  │              │  Botón gradient
│         │  └───────────────┘  │              │
│         │                     │              │
│         │  ─── o continúa ─── │              │  Separador
│         │                     │              │
│         │  [G] Google         │              │  OAuth buttons
│         │  [S] Spotify        │              │
│         │                     │              │
│         │  ¿No tienes cuenta? │              │  Link a registro
│         │      Regístrate     │              │
│         └─────────────────────┘              │
│                                              │
└──────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos |
|----------|------------|---------|
| **Container** | `<div>` | `min-h-screen bg-[#0f0f1a] flex items-center justify-center` |
| **Logo** | Custom | Icono nota musical + texto "MusicFund", color gradient |
| **Card** | `<Card>` | `w-full max-w-md bg-card backdrop-blur border-border p-8 rounded-xl` |
| **Título** | `<h1>` | `text-2xl font-bold text-white text-center` |
| **Subtítulo** | `<p>` | `text-text-secondary text-center mb-6` |
| **Input Email** | `<Input>` | `icon={Mail}`, placeholder="tu@email.com" |
| **Input Password** | `<Input>` | `type="password"`, toggle visibility, `icon={Lock}` |
| **Checkbox** | `<Checkbox>` | label="Recordarme" |
| **Link olvidé** | `<Link>` | `text-sm text-primary-pink hover:underline` |
| **Botón Submit** | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 text-white` |
| **Separador** | Custom | `<div>` con líneas y texto "o continúa con" |
| **Botón Google** | `<Button>` | `variant="outline"`, icono Google |
| **Botón Spotify** | `<Button>` | `variant="outline"`, icono Spotify, verde |
| **Link registro** | `<Link>` | `text-sm`, "Regístrate" en color primary |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Inputs vacíos, botón habilitado |
| **Focused** | Input con borde `primary-pink`, glow sutil |
| **Error validación** | Input con borde `status-error`, mensaje rojo debajo |
| **Loading** | Botón disabled, spinner, texto "Iniciando sesión..." |
| **Error API** | Toast notification en esquina superior derecha |
| **Success** | Redirect inmediato (sin feedback visual extra) |

### Validación en tiempo real

| Campo | Validación | Mensaje de error |
|-------|-----------|------------------|
| Email | `required` | "El email es obligatorio" |
| Email | `email format` | "Formato de email inválido" |
| Password | `required` | "La contraseña es obligatoria" |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| Click en ojo (password) | Toggle `type="password"` ↔ `type="text"` |
| Click "Olvidaste contraseña" | Navega a `/auth/forgot-password` |
| Click "Regístrate" | Navega a `/auth/register` |
| Submit exitoso | Guarda token, redirect a `/dashboard` (Admin) |
| Click Google/Spotify | OAuth flow (fuera de scope MVP) |

---

## Pantalla: Register

**Mockup:** Similar a Login (no hay mockup específico, derivar de WPR_4)
**Proyecto:** Landing (`src/web`)
**Ruta:** `/auth/register`

### Layout

Mismo layout que Login con diferencias:

```
┌─────────────────────┐
│  Crear tu cuenta    │  Título diferente
│                     │
│  ┌───────────────┐  │
│  │ 📧 Email      │  │
│  └───────────────┘  │
│                     │
│  ┌───────────────┐  │
│  │ 🔒 Contraseña │  │
│  └───────────────┘  │
│                     │
│  ┌───────────────┐  │
│  │ 🔒 Confirmar  │  │  Campo adicional
│  └───────────────┘  │
│                     │
│  ┌───────────────┐  │
│  │ Crear cuenta  │  │  Texto botón diferente
│  └───────────────┘  │
│                     │
│  ¿Ya tienes cuenta? │  Link invertido
│     Inicia sesión   │
└─────────────────────┘
```

### Validación en tiempo real

| Campo | Validación | Mensaje de error |
|-------|-----------|------------------|
| Email | `required` | "El email es obligatorio" |
| Email | `email format` | "Formato de email inválido" |
| Password | `required` | "La contraseña es obligatoria" |
| Password | `min 8 chars` | "Mínimo 8 caracteres" |
| Confirm | `match password` | "Las contraseñas no coinciden" |

### Flujo post-registro

```
Register success → Guarda token → Redirect a Admin /artista/perfil/crear
```

---

## Pantalla: Crear Perfil Artista

**Mockup:** Derivado de Dashboard (WPR_5)
**Proyecto:** Admin (`src/admin`)
**Ruta:** `/artista/perfil/crear`
**Template base:** `dashtail/forms`

### Layout

```
┌──────────────────────────────────────────────────────────┐
│ [Sidebar]  │                                             │
│            │  Crear tu perfil de artista                 │
│ WePlay     │  ─────────────────────────────              │
│ Rises      │                                             │
│            │  ┌─────────────────────────────────────┐    │
│ ──────     │  │                                     │    │
│            │  │  [Avatar placeholder]               │    │
│ Dashboard  │  │  + Subir imagen                     │    │
│ (disabled) │  │                                     │    │
│            │  └─────────────────────────────────────┘    │
│ ──────     │                                             │
│            │  Nombre artístico *                         │
│ Logout     │  ┌─────────────────────────────────────┐    │
│            │  │                                     │    │
│            │  └─────────────────────────────────────┘    │
│            │                                             │
│            │  Biografía                                  │
│            │  ┌─────────────────────────────────────┐    │
│            │  │                                     │    │
│            │  │                                     │    │
│            │  │                                     │    │
│            │  └─────────────────────────────────────┘    │
│            │                                             │
│            │  ┌─────────────┐  ┌─────────────────┐       │
│            │  │ País        │  │ Ciudad          │       │
│            │  └─────────────┘  └─────────────────┘       │
│            │                                             │
│            │  URL de imagen (opcional)                   │
│            │  ┌─────────────────────────────────────┐    │
│            │  │ https://...                         │    │
│            │  └─────────────────────────────────────┘    │
│            │                                             │
│            │  ┌─────────────┐  ┌─────────────────┐       │
│            │  │   Omitir    │  │ Guardar perfil  │       │
│            │  └─────────────┘  └─────────────────┘       │
│            │                                             │
└──────────────────────────────────────────────────────────┘
```

### Sidebar (estado durante onboarding)

| Item | Estado | Comportamiento |
|------|--------|----------------|
| Dashboard | Disabled | No clickeable hasta completar perfil |
| Mis Campañas | Disabled | No clickeable |
| Crear Campaña | Disabled | No clickeable |
| Logout | Enabled | Permite salir |

### Especificaciones de Form

| Campo | Componente | Props |
|-------|------------|-------|
| Avatar | Custom `<AvatarUpload>` | placeholder, onClick → file picker |
| Nombre artístico | `<Input>` | `required`, maxLength=200 |
| Biografía | `<Textarea>` | rows=4, maxLength=2000, contador |
| País | `<Select>` o `<Input>` | lista de países o texto libre |
| Ciudad | `<Input>` | maxLength=100 |
| URL imagen | `<Input>` | type="url", validación URL |
| Botón Omitir | `<Button>` | `variant="ghost"` |
| Botón Guardar | `<Button>` | `variant="default"`, gradient |

### Estados de UI

| Estado | Comportamiento |
|--------|----------------|
| **Default** | Form vacío, "Guardar" habilitado |
| **Validating** | Indicadores inline por campo |
| **Submitting** | Botón disabled, spinner, "Guardando..." |
| **Error** | Toast + highlight campo con error |
| **Success** | Redirect a `/dashboard` |

### Preview de imagen

```
Si URL válida → Mostrar preview en el avatar placeholder
Si URL inválida → Mostrar error, mantener placeholder
```

---

## Pantalla: Dashboard Artista

**Mockup:** WPR_5-Dashboard-Artist.png
**Proyecto:** Admin (`src/admin`)
**Ruta:** `/dashboard`
**Template base:** `dashtail/dashboard`

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│ SIDEBAR (200px)      │  MAIN CONTENT                           │
├──────────────────────┼─────────────────────────────────────────┤
│                      │                                         │
│ 🎵 WePlay Rises      │  Hola, {nombre}           [+ Nueva]     │
│                      │  {fecha}                                │
│ ┌──────────────┐     │                                         │
│ │ Avatar       │     │  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐
│ │ Nombre       │     │  │ $24,580│ │ 1,247  │ │   3    │ │  78%   │
│ │ "Artista"    │     │  │+12.5%  │ │ +8.2%  │ │        │ │ +5.1%  │
│ └──────────────┘     │  │Recaud. │ │Backers │ │Campañas│ │ Éxito  │
│                      │  └────────┘ └────────┘ └────────┘ └────────┘
│ ☰ Dashboard     ←    │                                         │
│ 🎵 Mis Campañas      │  Recaudación últimos 30 días            │
│ + Crear Campaña      │  ┌─────────────────────────────────────┐│
│ 👥 Mis Backers       │  │         📈 Gráfico de línea         ││
│ ⚙ Configuración      │  │                                     ││
│                      │  └─────────────────────────────────────┘│
│ ─────────────────    │                                         │
│ 🔗 Ver perfil público│  Mis Campañas          Últimos Backings │
│                      │  ┌─────────────────┐   ┌───────────────┐│
│ 🚪 Logout            │  │ Card campaña 1  │   │ Backing 1     ││
│                      │  │ Card campaña 2  │   │ Backing 2     ││
│                      │  │ Card campaña 3  │   │ Backing 3     ││
│                      │  └─────────────────┘   └───────────────┘│
└──────────────────────┴─────────────────────────────────────────┘
```

### Componentes del Dashboard

#### KPI Cards (4)

| KPI | Icono | Color | Datos |
|-----|-------|-------|-------|
| Total recaudado | 💰 | Pink/purple gradient | `${amount}` + `+X%` |
| Backers totales | 👥 | Blue | `{count}` + `+X%` |
| Campañas activas | 🎵 | Purple | `{count}` |
| Tasa de éxito | 📈 | Green | `{percent}%` + `+X%` |

#### Gráfico de línea

```
Componente: recharts <LineChart> o similar
Datos: recaudación diaria últimos 30 días
Color línea: gradient pink-purple
Fondo: transparent con grid sutil
```

#### Card de Campaña

```
┌──────────────────────────────────────┐
│ [Imagen]  Título de la campaña       │
│           [Badge estado] X% • N back │
└──────────────────────────────────────┘

Badge estados:
- Activa: verde (#10b981)
- Borrador: gris
- Finalizada: azul
```

#### Lista de Backings

```
┌──────────────────────────────────────┐
│ [Avatar] Nombre         $XX          │
│          Reward name    Hace Xh      │
└──────────────────────────────────────┘
```

---

## Pantalla: Perfil Público de Artista

**Mockup:** WPR_8-Artist-Profile.png
**Proyecto:** Landing (`src/web`)
**Ruta:** `/artistas/{id}`
**Template base:** `krowd/artist-profile`

### Layout

```
┌────────────────────────────────────────────────────────────────┐
│ HEADER: Logo | Explorar | Para Artistas | ... | Login | Registro
├────────────────────────────────────────────────────────────────┤
│                                                                │
│ ░░░░░░░░░░░░░░░ BANNER GRADIENT ░░░░░░░░░░░░░░░               │
│                                                                │
│  ┌─────┐   Nombre Artista           [♥ Seguir] [🎵][📺][📷]   │
│  │Avatar│  [Tag1] [Tag2] [Tag3]                                │
│  └─────┘                                                       │
│                                                                │
│  🎵 3 campañas  👥 247 backers  💰 €12,450 recaudados          │
│                                                                │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│ ┌─────────────────────┐  ┌────────────────────────────────┐   │
│ │ Biografía           │  │ [Campañas Activas] [Pasadas]   │   │
│ │                     │  │                                │   │
│ │ Texto largo...      │  │ ┌────────────────────────────┐ │   │
│ │                     │  │ │ Card Campaña               │ │   │
│ │ 🎵 Escuchar Spotify │  │ │ - Imagen                   │ │   │
│ │ 📺 Videos YouTube   │  │ │ - Título + badge           │ │   │
│ │ 🌐 Sitio oficial    │  │ │ - Progress bar             │ │   │
│ │                     │  │ │ - Días + backers           │ │   │
│ ├─────────────────────┤  │ └────────────────────────────┘ │   │
│ │ Reseñas de Fans     │  │                                │   │
│ │                     │  └────────────────────────────────┘   │
│ │ ┌─────────────────┐ │                                       │
│ │ │ Avatar ★★★★★    │ │                                       │
│ │ │ "Comentario..." │ │                                       │
│ │ └─────────────────┘ │                                       │
│ └─────────────────────┘                                       │
│                                                                │
├────────────────────────────────────────────────────────────────┤
│ FOOTER: Links organizados en columnas                          │
└────────────────────────────────────────────────────────────────┘
```

### Componentes específicos

| Componente | Descripción |
|------------|-------------|
| Banner | Gradient purple/pink, puede tener imagen de fondo |
| Avatar | Circular, 120px, borde blanco sutil |
| Tags de género | Pills con fondo semi-transparente |
| Botón Seguir | Icono corazón + texto, outline o filled si siguiendo |
| Iconos redes | Spotify (verde), YouTube (rojo), Instagram (gradient) |
| Stats | Iconos + números en fila |
| Progress bar | Gradient, porcentaje a la derecha |
| Rating stars | 5 estrellas, filled = doradas |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 640px | Stack vertical, sidebar colapsada, cards full width |
| **Tablet** | 640-1024px | Sidebar colapsable, grid 2 cols |
| **Desktop** | > 1024px | Sidebar fija, grid 3-4 cols |

### Mobile específico

- Sidebar se convierte en bottom nav o hamburger menu
- KPI cards en scroll horizontal o 2x2 grid
- Forms ocupan full width con padding reducido

---

## Animaciones y Transiciones

| Elemento | Animación | Duración |
|----------|-----------|----------|
| Page transition | Fade in | 150ms |
| Button hover | Scale 1.02 + brightness | 150ms |
| Card hover | Elevate shadow + border glow | 200ms |
| Input focus | Border color + glow | 150ms |
| Toast | Slide in from top-right | 300ms |
| Modal | Fade + scale from 0.95 | 200ms |
| Progress bar | Width transition | 500ms ease-out |

---

## Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| Contraste | Texto blanco sobre fondo oscuro (ratio > 4.5:1) |
| Focus visible | Ring púrpura en elementos focuseados |
| Labels | Todos los inputs tienen label (visible o sr-only) |
| Keyboard nav | Tab order lógico, Enter para submit |
| Screen reader | aria-labels en iconos, roles en regiones |
| Error messages | Asociados a inputs con aria-describedby |

---

## Checklist UI/UX

### Login/Register (Landing)
- [ ] Layout centrado con card
- [ ] Logo MusicFund/WePlay Rises
- [ ] Inputs con iconos
- [ ] Password toggle
- [ ] Botón gradient
- [ ] OAuth buttons (placeholder para MVP)
- [ ] Links de navegación
- [ ] Validación inline
- [ ] Estados loading/error

### Crear Perfil (Admin)
- [ ] Sidebar con items disabled
- [ ] Form con todos los campos
- [ ] Preview de avatar
- [ ] Contador de caracteres en textarea
- [ ] Botones Omitir y Guardar
- [ ] Validación inline

### Dashboard (Admin)
- [ ] Sidebar completa
- [ ] Header con saludo y botón acción
- [ ] 4 KPI cards
- [ ] Gráfico de línea
- [ ] Lista de campañas
- [ ] Lista de backings recientes

### Perfil Público (Landing)
- [ ] Header de navegación
- [ ] Banner con avatar
- [ ] Stats del artista
- [ ] Biografía con links
- [ ] Tabs de campañas
- [ ] Cards de campaña
- [ ] Sección de reseñas
- [ ] Footer

---

*Este documento debe usarse junto con los mockups originales en `docs/ui-images/`*
