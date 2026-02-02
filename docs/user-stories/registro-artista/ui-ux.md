# UI/UX: Registro de Artista

> **Feature:** registro-artista
> **Última actualización:** 2026-01-26

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Login/Auth | [WPR_4-Login.png](../../ui-images/WPR_4-Login.png) | Admin |
| Perfil Público Artista | [WPR_8-Artist-Profile.png](../../ui-images/WPR_8-Artist-Profile.png) | Landing |
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin |

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Dashtail** | Auth forms (login, register), Dashboard, Profile forms | `references/templates/dashtail/` |
| **Krowd** | Perfil público del artista (landing) | `references/templates/krowd/` |

**Componente de referencia clave:**
- Dashtail login form: `dash-tail-starter-kit(TypeScript)/components/auth/login-form.tsx`

---

## Design Tokens

### Paleta de Colores

```css
:root {
  /* Background - Dark theme */
  --bg-primary: #1a1a2e;
  --bg-secondary: #16213e;
  --bg-card: #0f1729;
  --bg-card-hover: #1e2a42;

  /* Primary - Gradient pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;

  /* Status */
  --status-success: #10b981;
  --status-error: #ef4444;
  --status-warning: #f59e0b;
  --status-info: #3b82f6;

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Input */
  --input-bg: #0f1729;
  --input-bg-disabled: #1e293b;
  --input-border: #334155;
  --input-placeholder: #64748b;
}
```

### Tipografía

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Font Sizes */
  --text-xs: 0.75rem;      /* 12px */
  --text-sm: 0.875rem;     /* 14px */
  --text-base: 1rem;       /* 16px */
  --text-lg: 1.125rem;     /* 18px */
  --text-xl: 1.25rem;      /* 20px */
  --text-2xl: 1.5rem;      /* 24px */
  --text-3xl: 1.875rem;    /* 30px */

  /* Font Weights */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Line Heights */
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
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;   /* 6px */
  --radius-md: 0.5rem;     /* 8px */
  --radius-lg: 0.75rem;    /* 12px */
  --radius-xl: 1rem;       /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.3);
}
```

---

## Pantalla: Registro de Usuario

**Mockup:** WPR_4-Login.png (usar mismo estilo para registro)
**Proyecto:** Admin
**Ruta:** `/auth/register`
**Template base:** `dashtail/components/auth/login-form.tsx`

### Layout

```
┌────────────────────────────────────────────────────────┐
│                                                        │
│                    [LOGO + BRAND]                      │
│                     WePlay Rises                       │
│                                                        │
│              ┌──────────────────────────┐              │
│              │                          │              │
│              │  Crea tu cuenta          │              │
│              │  Registra tu perfil...   │              │
│              │                          │              │
│              │  Email                   │              │
│              │  [________________]      │              │
│              │                          │              │
│              │  Contraseña              │              │
│              │  [________________] 👁    │              │
│              │                          │              │
│              │  Confirmar Contraseña    │              │
│              │  [________________] 👁    │              │
│              │                          │              │
│              │  [Crear cuenta]          │              │
│              │  (gradient button)       │              │
│              │                          │              │
│              │  ¿Ya tienes cuenta?      │              │
│              │  Iniciar sesión          │              │
│              │                          │              │
│              └──────────────────────────┘              │
│                                                        │
└────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Logo Container | `<div>` con `<SiteLogo>` | `mb-8 text-center` |
| Logo Icon | Custom SVG / Icon | `h-12 w-12 text-primary` (gradient mask) |
| Brand Text | `<h1>` | `text-2xl font-bold text-white mt-2` |
| Card Container | `<Card>` | `max-w-md mx-auto bg-[#0f1729] border-[#334155]` |
| Card Content | `<CardContent>` | `p-8` |
| Title | `<h2>` | `text-3xl font-bold text-white mb-2` |
| Subtitle | `<p>` | `text-base text-[#94a3b8] mb-6` |
| Form | `<Form>` (react-hook-form) | - |
| Label | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Email Input | `<Input type="email">` | `bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] focus:border-primary` |
| Password Input | `<Input type="password">` | Same as Email, with eye icon toggle |
| Eye Icon | `<Icon>` (heroicons) | `absolute right-4 top-1/2 -translate-y-1/2 cursor-pointer text-[#94a3b8] hover:text-white` |
| Submit Button | `<Button>` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold` |
| Link to Login | `<Link>` | `text-primary hover:underline` |
| Footer Text | `<p>` | `text-center text-sm text-[#94a3b8] mt-6` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Form vacío, inputs con placeholder, botón enabled |
| **Focus** | Input seleccionado: border color `#a855f7`, glow shadow |
| **Typing** | Validación en tiempo real (onChange), mostrar errores debajo del input |
| **Loading** | Botón muestra spinner + texto "Creando cuenta...", inputs disabled |
| **Error** | Border rojo en input con error (`#ef4444`), mensaje de error en rojo debajo |
| **Success** | Redirect automático a `/artista/perfil/crear` (no mostrar mensaje) |
| **Disabled** | Inputs con `bg-[#1e293b]`, cursor not-allowed |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Email** | Formato email válido | "Ingresa un email válido" |
| **Email** | No vacío | "El email es obligatorio" |
| **Password** | Mínimo 8 caracteres | "La contraseña debe tener mínimo 8 caracteres" |
| **Password** | No vacío | "La contraseña es obligatoria" |
| **Confirmar Password** | Coincide con Password | "Las contraseñas no coinciden" |
| **Confirmar Password** | No vacío | "Confirma tu contraseña" |

**Códigos de error del backend:**
- `409 Conflict` → "Este email ya está registrado. [Iniciar sesión](#)"
- `400 Bad Request` → Mostrar mensaje de validación específico del backend

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click en eye icon** | Toggle tipo de input (password ↔ text) |
| **Submit form** | Validar frontend, si OK enviar POST a `/api/auth/register`, guardar JWT en localStorage, redirect a `/artista/perfil/crear` |
| **Click "Iniciar sesión"** | Navigate a `/auth/login` |
| **Validación onChange** | Validar campo al perder foco (onBlur) y al escribir (debounced) |
| **Enter en input** | Submit form si validación OK |

### Zod Schema

```typescript
const registerSchema = z.object({
  email: z.string().email("Ingresa un email válido"),
  password: z.string().min(8, "La contraseña debe tener mínimo 8 caracteres"),
  confirmPassword: z.string()
}).refine(data => data.password === data.confirmPassword, {
  message: "Las contraseñas no coinciden",
  path: ["confirmPassword"]
});
```

---

## Pantalla: Crear Perfil de Artista

**Mockup:** N/A (usar estilo dashboard de WPR_5)
**Proyecto:** Admin
**Ruta:** `/artista/perfil/crear`
**Template base:** `dashtail/components/` (form patterns)

### Layout

```
┌────────────────────────────────────────────────────────────┐
│  [LOGO] WePlay Rises                                       │
├────────────────────────────────────────────────────────────┤
│                                                            │
│           Completa tu perfil de artista                    │
│           Cuéntanos sobre tu música                        │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                                                      │  │
│  │  Nombre artístico *                                  │  │
│  │  [_____________________________]                     │  │
│  │                                                      │  │
│  │  Descripción                                         │  │
│  │  [                             ]                     │  │
│  │  [      Textarea 4 rows        ]                     │  │
│  │  [                             ]                     │  │
│  │  [_____________________________]                     │  │
│  │  0/2000 caracteres                                   │  │
│  │                                                      │  │
│  │  País                    Ciudad                      │  │
│  │  [______________]        [______________]            │  │
│  │                                                      │  │
│  │  Imagen de perfil (URL)                              │  │
│  │  [_____________________________]                     │  │
│  │                                                      │  │
│  │  ┌────────────┐                                      │  │
│  │  │  Preview   │  (Si URL válida, mostrar imagen)     │  │
│  │  │            │                                      │  │
│  │  └────────────┘                                      │  │
│  │                                                      │  │
│  │  [Guardar y continuar]  Saltar por ahora             │  │
│  │  (gradient button)      (text link)                  │  │
│  │                                                      │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Header | `<div>` | `bg-[#0f1729] border-b border-[#334155] px-6 py-4` |
| Logo | `<Link>` + Icon | `flex items-center gap-2 text-white font-bold` |
| Main Container | `<div>` | `max-w-2xl mx-auto py-12 px-6` |
| Title | `<h1>` | `text-3xl font-bold text-white mb-2` |
| Subtitle | `<p>` | `text-lg text-[#94a3b8] mb-8` |
| Form Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-8` |
| Label (required) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 after:content-['*'] after:text-red-500 after:ml-1` |
| Label (optional) | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Text Input | `<Input>` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Textarea | `<Textarea>` | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none` |
| Character Count | `<span>` | `text-xs text-[#64748b] mt-1` |
| Country/City Grid | `<div>` | `grid grid-cols-2 gap-4` |
| Image Preview | `<Avatar size="xl">` | `w-32 h-32 rounded-lg mt-3` |
| Submit Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8` |
| Skip Link | `<Link>` | `text-[#94a3b8] hover:text-white underline ml-4` |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Form vacío, solo nombre artístico required |
| **Typing Descripción** | Character counter actualizado en tiempo real (X/2000) |
| **Typing Imagen URL** | Debounced validation (500ms), si válida mostrar preview |
| **Preview Cargando** | Skeleton o spinner en el área de preview |
| **Preview Error** | Icono placeholder si URL no carga imagen |
| **Loading Submit** | Botón muestra spinner + "Guardando perfil...", inputs disabled |
| **Error** | Toast notification rojo con mensaje de error |
| **Success** | Toast notification verde "Perfil creado", redirect a `/dashboard` |

### Validación en Tiempo Real

| Campo | Validación | Mensaje |
|-------|-----------|---------|
| **Nombre artístico** | No vacío | "El nombre artístico es obligatorio" |
| **Nombre artístico** | Máx 200 caracteres | "Máximo 200 caracteres" |
| **Descripción** | Máx 2000 caracteres | "Máximo 2000 caracteres" (mostrar contador) |
| **País** | Opcional | - |
| **Ciudad** | Opcional | - |
| **Imagen URL** | URL válida (si se proporciona) | "Ingresa una URL válida" |

**Códigos de error del backend:**
- `ARTISTA_NOMBRE_REQUERIDO` → "El nombre artístico es obligatorio"
- `ARTISTA_DESC_MAX_LENGTH` → "La descripción es muy larga (máx 2000 caracteres)"
- `ARTISTA_IMAGEN_URL_INVALIDA` → "La URL de la imagen no es válida"

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Typing en descripción** | Actualizar contador de caracteres en tiempo real |
| **Typing en Imagen URL** | Debounce 500ms, luego intentar cargar imagen en preview |
| **Click "Guardar y continuar"** | Validar, POST a `/api/artistas`, guardar, redirect a `/dashboard` |
| **Click "Saltar por ahora"** | Crear perfil con solo nombre artístico (mínimo), redirect a `/dashboard` |
| **Imagen carga con éxito** | Mostrar preview en Avatar component |
| **Imagen falla al cargar** | Mostrar icono placeholder (music note icon) |

### Zod Schema

```typescript
const crearPerfilSchema = z.object({
  nombreArtistico: z.string().min(1, "El nombre artístico es obligatorio").max(200, "Máximo 200 caracteres"),
  descripcion: z.string().max(2000, "Máximo 2000 caracteres").optional(),
  pais: z.string().optional(),
  ciudad: z.string().optional(),
  imagenUrl: z.string().url("Ingresa una URL válida").optional().or(z.literal(""))
});
```

---

## Pantalla: Perfil Público de Artista

**Mockup:** WPR_8-Artist-Profile.png
**Proyecto:** Landing
**Ruta:** `/artistas/{id}`
**Template base:** `krowd/` (public profile layouts)

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│  [LOGO] MusicFund  Explorar  Para Artistas  Cómo funciona   │
│                                    Iniciar Sesión  Registro  │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │          [HERO IMAGE - Artist Banner]                  │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌────┐                                                      │
│  │IMG │  Luna Vibe                                           │
│  └────┘  Electronic  Synthwave  Indie                        │
│                                                              │
│  🎵 3 campañas  👥 247 backers  💰 €12,450 recaudados         │
│                                                              │
│  [💜 Seguir]  🌐 🎵 📺                                         │
│                                                              │
│  ┌─────────────────────────┐  ┌─────────────────────────┐    │
│  │  Biografía              │  │  Campañas Activas       │    │
│  │                         │  │                         │    │
│  │  Luna Vibe es una      │  │  ┌─────────────────────┐ │    │
│  │  artista emergente...   │  │  │ [IMG] Campaign Card │ │    │
│  │                         │  │  │ Neon Dreams         │ │    │
│  │  Desde Barcelona...     │  │  │ €8,750 / €15,000   │ │    │
│  │                         │  │  │ 58% ████░░░        │ │    │
│  │  🎵 Spotify             │  │  └─────────────────────┘ │    │
│  │  📺 YouTube             │  │                         │    │
│  │  🌐 Sitio oficial       │  │  [Ver todas]            │    │
│  └─────────────────────────┘  └─────────────────────────┘    │
│                                                              │
│  ┌─────────────────────────┐                                 │
│  │  Reseñas de Fans        │                                 │
│  │                         │                                 │
│  │  👤 Carlos M. ⭐⭐⭐⭐⭐     │                                 │
│  │  Increíble artista...   │                                 │
│  └─────────────────────────┘                                 │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### Especificaciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Header | `<header>` | `bg-[#1a1a2e] border-b border-[#334155] sticky top-0 z-50` |
| Hero Banner | `<div>` con imagen o gradient | `h-64 bg-gradient-to-r from-purple-900 to-pink-900` |
| Artist Avatar | `<Avatar size="2xl">` | `w-32 h-32 -mt-16 border-4 border-[#1a1a2e] rounded-full` |
| Artist Name | `<h1>` | `text-4xl font-bold text-white mt-4` |
| Genre Tags | `<Badge>` x3 | `bg-[#2d1b4e] text-purple-300 border-purple-500/50` |
| Stats Row | `<div>` | `flex gap-6 text-[#94a3b8] mt-4` |
| Stat Item | `<span>` | `flex items-center gap-2` (icono + texto) |
| Follow Button | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white px-6` |
| Social Icons | `<Button variant="ghost" size="icon">` | `text-[#94a3b8] hover:text-white` |
| Bio Card | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Bio Title | `<h2>` | `text-xl font-bold text-white mb-4` |
| Bio Text | `<p>` | `text-[#94a3b8] leading-relaxed whitespace-pre-wrap` |
| Social Links | `<Link>` | `flex items-center gap-2 text-primary hover:underline` |
| Campaigns Section | `<div>` | `grid gap-4` |
| Campaign Card | Custom component | Ver diseño de campañas (fuera de scope, placeholder) |
| Reviews Section | `<Card>` | `bg-[#0f1729] border-[#334155] p-6` |
| Review Item | `<div>` | `flex gap-3 mb-4 pb-4 border-b border-[#334155] last:border-0` |
| Reviewer Avatar | `<Avatar>` | `w-10 h-10` |
| Stars | `<div>` con iconos | `text-yellow-400` (5 estrellas) |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton loaders para avatar, nombre, bio, cards |
| **No Image** | Avatar muestra icono de música placeholder |
| **No Description** | Mostrar "Este artista aún no ha agregado una biografía" (texto muted) |
| **No Campaigns** | Mostrar mensaje "Próximamente campañas..." |
| **Error 404** | Mostrar página "Artista no encontrado" |
| **Social Link Hover** | Cambio de color a white, underline |

### Interacciones

| Acción | Comportamiento |
|--------|----------------|
| **Click en Follow** | Si no autenticado → redirect a login. Si autenticado → POST follow (fuera de MVP) |
| **Click en social link** | Abrir en nueva pestaña (target="_blank") |
| **Click en campaign card** | Navigate a `/campanias/{id}` |
| **Hover en social icons** | Color change a white |

### Responsive Behavior

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | Hero banner h-48, Avatar w-24 h-24, Stats apilados verticalmente, Bio y Campaigns en columna única |
| **Tablet (640-1024px)** | Hero banner h-56, Grid de 2 columnas (Bio | Campaigns) |
| **Desktop (> 1024px)** | Hero banner h-64, Grid de 3 columnas (Bio | Campaigns | Reviews) |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios Generales |
|------------|-------|----------|
| **Mobile** | < 640px | Stack todo verticalmente, padding reducido (px-4), font sizes reducidos (-2px), botones full width |
| **Tablet** | 640-1024px | Grid 2 columnas donde aplique, padding medio (px-6), font sizes normales |
| **Desktop** | > 1024px | Layouts completos, max-width containers (max-w-7xl), padding amplio (px-8) |

### Registro Page (Mobile)
- Card toma full width con padding 4
- Logo más pequeño (h-10 w-10)
- Font title 2xl → xl
- Inputs size="md" en lugar de size="lg"

### Crear Perfil Page (Mobile)
- Grid de País/Ciudad → stack vertical
- Preview imagen más pequeño (w-24 h-24)
- Botones apilados verticalmente (no inline)

### Perfil Público (Mobile)
- Hero banner height 200px
- Avatar w-20 h-20
- Stats en grid 2x2
- Bio, Campaigns, Reviews en columna única

---

## Animaciones

| Elemento | Animación | Duración |
|----------|-----------|----------|
| **Button hover** | Scale 1.02 + shadow glow | 150ms ease |
| **Input focus** | Border color transition + glow shadow | 200ms ease |
| **Card hover** | Background lighten + shadow increase | 200ms ease |
| **Page transition** | Fade in opacity 0 → 1 | 300ms ease-in-out |
| **Toast notification** | Slide in from top + fade | 250ms ease-out |
| **Image preview load** | Fade in + scale 0.95 → 1 | 300ms ease |
| **Validation error** | Shake animation + fade in | 400ms ease |
| **Loading spinner** | Rotate 360deg infinite | 1000ms linear |
| **Character counter** | Color transition (gray → yellow → red) | 200ms ease |

### Shake Animation (Validation Error)

```css
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}
```

---

## Accesibilidad

| Requisito | Implementación |
|-----------|----------------|
| **Contraste de color** | Mínimo 4.5:1 para texto normal, 3:1 para texto grande. Verificado: white (#fff) sobre bg-primary (#1a1a2e) = 15.8:1 ✓ |
| **Focus visible** | Ring de 2px en `border-primary` (#a855f7) con offset de 2px en todos los elementos interactivos |
| **Labels en inputs** | Todos los inputs tienen `<Label>` asociado con `htmlFor` |
| **Form validation** | Mensajes de error con `role="alert"` y `aria-live="polite"` |
| **Buttons** | Texto descriptivo o `aria-label` en icon buttons |
| **Imágenes** | `alt` text descriptivo en Avatar, "Foto de perfil de {nombre}" |
| **Skip links** | "Saltar al contenido" invisible hasta focus (solo en landing) |
| **Keyboard navigation** | Tab order lógico, Enter para submit, Esc para cerrar modales |
| **Screen reader** | Loading states anunciados con `aria-busy="true"` y `aria-live="polite"` |
| **Color no es único indicador** | Errores usan icono + color + texto |

### ARIA Labels

```tsx
// Email input
<Label htmlFor="email">Email *</Label>
<Input
  id="email"
  type="email"
  aria-required="true"
  aria-invalid={!!errors.email}
  aria-describedby={errors.email ? "email-error" : undefined}
/>
{errors.email && (
  <span id="email-error" role="alert" className="text-destructive">
    {errors.email.message}
  </span>
)}

// Button loading
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 className="animate-spin" aria-hidden="true" />}
  {isPending ? "Creando cuenta..." : "Crear cuenta"}
</Button>

// Image preview
<Avatar>
  <AvatarImage src={imagenUrl} alt={`Foto de perfil de ${nombreArtistico}`} />
  <AvatarFallback>
    <Icon icon="heroicons:musical-note" aria-hidden="true" />
  </AvatarFallback>
</Avatar>
```

---

## Checklist UI/UX

### Registro de Usuario (/auth/register)
- [ ] Layout centrado con Card y logo
- [ ] Componentes shadcn/ui correctos (Input, Button, Label, Card)
- [ ] Gradient button implementado
- [ ] Validación en tiempo real (onChange/onBlur)
- [ ] Estados: default, focus, loading, error, success
- [ ] Password toggle (eye icon)
- [ ] Mensajes de error claros y en español
- [ ] Link a login funcional
- [ ] Responsive en mobile (card full width, padding reducido)
- [ ] Animación de shake en errores
- [ ] Focus states con ring purple
- [ ] ARIA labels y roles
- [ ] Loading spinner en botón

### Crear Perfil de Artista (/artista/perfil/crear)
- [ ] Header con logo
- [ ] Layout con Card centrado
- [ ] Todos los campos con Label correcto
- [ ] Textarea para descripción con contador de caracteres
- [ ] Grid 2 columnas para País/Ciudad (desktop)
- [ ] Image preview con Avatar component
- [ ] Preview muestra placeholder si URL inválida
- [ ] Debounced validation en imagen URL (500ms)
- [ ] Botón gradient "Guardar y continuar"
- [ ] Link "Saltar por ahora" funcional
- [ ] Validación Zod integrada
- [ ] Estados: typing, loading, error, success
- [ ] Responsive en mobile (grid → stack, preview reducido)
- [ ] Toast notifications para success/error
- [ ] Character counter actualizado en tiempo real
- [ ] Focus states y accesibilidad

### Perfil Público de Artista (/artistas/{id})
- [ ] Header sticky con navegación
- [ ] Hero banner con gradient o imagen
- [ ] Avatar con border y posición -mt-16
- [ ] Nombre artístico en h1 grande
- [ ] Genre tags como Badge components
- [ ] Stats row (campañas, backers, recaudado)
- [ ] Follow button con gradient
- [ ] Social icons (Spotify, YouTube, Web)
- [ ] Bio Card con texto formateado
- [ ] Campañas section (placeholder para MVP)
- [ ] Loading skeletons
- [ ] Estado: No Image (placeholder icon)
- [ ] Estado: No Description (mensaje muted)
- [ ] Error 404 page
- [ ] Responsive: mobile (stack), tablet (2 cols), desktop (3 cols)
- [ ] Social links abren en nueva pestaña
- [ ] Hover effects en cards y buttons
- [ ] ARIA labels en social icons

### Cross-cutting
- [ ] Dark theme aplicado consistentemente
- [ ] Design tokens definidos y usados
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-300ms)
- [ ] Contraste mínimo 4.5:1 verificado
- [ ] Keyboard navigation funcional
- [ ] Form validation con Zod + react-hook-form
- [ ] Integración con TanStack Query
- [ ] Manejo de errores del backend (ServiceResponse)
- [ ] Toast notifications configuradas
- [ ] Mobile-first responsive design
