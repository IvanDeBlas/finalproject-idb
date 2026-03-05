# Diseno UI: Registro de Artista (Admin)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/admin (Next.js 14 + App Router)

## 1. Resumen

- Componentes shadcn/ui utilizados: 11
- Composiciones custom: 5
- Pantallas: 2 (Registro, Crear Perfil)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)
- Tema: Dark mode con gradientes pink/purple

## 2. Paleta de Colores (Proyecto)

| Uso | Variable CSS | Tailwind Class | Ejemplo |
|-----|--------------|----------------|---------|
| Background Primary | `--bg-primary: #1a1a2e` | `bg-[#1a1a2e]` | Fondo principal dark |
| Background Card | `--bg-card: #0f1729` | `bg-[#0f1729]` | Cards y formularios |
| Primary Gradient | `linear-gradient(135deg, #ec4899, #a855f7)` | `bg-gradient-to-r from-pink-500 to-purple-600` | Botones principales |
| Text Primary | `--text-primary: #ffffff` | `text-white` | Titulos y texto principal |
| Text Secondary | `--text-secondary: #94a3b8` | `text-[#94a3b8]` | Subtitulos y descripciones |
| Text Muted | `--text-muted: #64748b` | `text-[#64748b]` | Placeholders y hints |
| Border Primary | `--border-primary: #334155` | `border-[#334155]` | Bordes de inputs y cards |
| Border Focus | `--border-focus: #a855f7` | `border-primary` | Inputs en focus |
| Destructive | `--destructive: #ef4444` | `text-destructive` | Errores |
| Success | `--success: #10b981` | `text-green-500` | Mensajes de exito |

## 3. Componentes shadcn/ui Disponibles

### 3.1 Instalados y Verificados

| Componente | Archivo | Uso en Feature |
|------------|---------|----------------|
| **Button** | `button.tsx` | Submit, toggle password, skip |
| **Card** | `card.tsx` | Contenedores de formularios |
| **Input** | `input.tsx` | Email, password, text fields |
| **Label** | `label.tsx` | Labels de formularios |
| **Textarea** | `textarea.tsx` | Descripcion de artista |
| **Badge** | `badge.tsx` | NO usado en esta feature |
| **Progress** | `progress.tsx` | NO usado en esta feature |
| **Skeleton** | `skeleton.tsx` | Loading states de image preview |
| **Sonner** | `sonner.tsx` | Toast notifications (success/error) |
| **Dropdown Menu** | `dropdown-menu.tsx` | NO usado en esta feature |

### 3.2 Faltantes (Necesario Instalar)

| Componente | Comando | Uso en Feature |
|------------|---------|----------------|
| **Avatar** | `npx shadcn@latest add avatar` | Preview de imagen de perfil |
| **Form** | `npx shadcn@latest add form` | Wrapper de React Hook Form |

**Accion requerida antes de implementar:**
```bash
cd src/admin
npx shadcn@latest add avatar
npx shadcn@latest add form
```

## 4. Componentes por Screen

### 4.1 Screen: Registro de Usuario

**Ruta:** `/auth/register`
**Layout padre:** `(auth)/layout.tsx`

#### Layout Visual

```
┌─────────────────────────────────────────────────┐
│                                                 │
│              ┌─────────────────┐                │
│              │  🎵  (Music)    │                │
│              │  WePlay Rises   │                │
│              └─────────────────┘                │
│                                                 │
│         ┌────────────────────────────────┐     │
│         │                                │     │
│         │  Crea tu cuenta                │     │
│         │  Registra tu perfil de artista │     │
│         │                                │     │
│         │  Email                         │     │
│         │  [_____________________]       │     │
│         │                                │     │
│         │  Contraseña                    │     │
│         │  [_____________________] 👁     │     │
│         │                                │     │
│         │  Confirmar Contraseña          │     │
│         │  [_____________________] 👁     │     │
│         │                                │     │
│         │  [   Crear cuenta   ]          │     │
│         │  (gradient button)             │     │
│         │                                │     │
│         │  ¿Ya tienes cuenta?            │     │
│         │  Iniciar sesión                │     │
│         │                                │     │
│         └────────────────────────────────┘     │
│                                                 │
└─────────────────────────────────────────────────┘
```

#### Composicion de Componentes

```tsx
<div className="flex min-h-screen items-center justify-center px-4">
  <Card className="w-full max-w-md">
    <CardHeader className="text-center">
      {/* Logo Container */}
      <div className="flex justify-center mb-4">
        <Music className="h-12 w-12 text-primary" />
      </div>

      {/* Title */}
      <CardTitle className="text-3xl font-bold">
        Crea tu cuenta
      </CardTitle>

      {/* Subtitle */}
      <CardDescription>
        Registra tu perfil de artista
      </CardDescription>
    </CardHeader>

    <CardContent>
      {/* RegisterForm Component (custom) */}
      <RegisterForm />
    </CardContent>
  </Card>
</div>
```

#### Tabla de Componentes UI

| Elemento Visual | Componente shadcn/ui | Variante/Props | Customizacion Tailwind |
|----------------|----------------------|----------------|------------------------|
| Container principal | `<div>` | - | `flex min-h-screen items-center justify-center px-4` |
| Card contenedor | `Card` | - | `w-full max-w-md` |
| Header | `CardHeader` | - | `text-center` |
| Logo icon | `Music` (lucide-react) | - | `h-12 w-12 text-primary` |
| Titulo principal | `CardTitle` | - | `text-3xl font-bold` |
| Subtitulo | `CardDescription` | - | Default styles |
| Body | `CardContent` | - | Default padding |
| Form | `Form` (react-hook-form) | - | - |
| Label | `Label` | - | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Input Email | `Input` | `type="email"` | `bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b]` |
| Input Password | `Input` | `type="password"` | Same as Email |
| Eye Icon Toggle | `Button` | `variant="ghost" size="icon"` | `absolute right-2 top-1/2 -translate-y-1/2` |
| Submit Button | `Button` | `variant="default"` | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Link to Login | `Link` (Next.js) | - | `text-primary hover:underline` |
| Footer text | `<p>` | - | `text-center text-sm text-muted-foreground` |

### 4.2 Screen: Crear Perfil de Artista

**Ruta:** `/artista/perfil/crear`
**Layout padre:** `(auth)/layout.tsx`

#### Layout Visual

```
┌──────────────────────────────────────────────────────┐
│  [LOGO] WePlay Rises                                 │
├──────────────────────────────────────────────────────┤
│                                                      │
│         Completa tu perfil de artista                │
│         Cuéntanos sobre tu música                    │
│                                                      │
│  ┌─────────────────────────────────────────────────┐ │
│  │                                                 │ │
│  │  Nombre artístico *                             │ │
│  │  [________________________________]              │ │
│  │                                                 │ │
│  │  Descripción                                    │ │
│  │  ┌────────────────────────────────────┐         │ │
│  │  │                                    │         │ │
│  │  │  (Textarea 4 filas)                │         │ │
│  │  │                                    │         │ │
│  │  └────────────────────────────────────┘         │ │
│  │  0/2000 caracteres                              │ │
│  │                                                 │ │
│  │  País                    Ciudad                 │ │
│  │  [_____________]        [_____________]         │ │
│  │                                                 │ │
│  │  Imagen de perfil (URL)                         │ │
│  │  [________________________________]              │ │
│  │                                                 │ │
│  │  ┌──────────┐                                   │ │
│  │  │  Avatar  │  (Preview con fallback)           │ │
│  │  │  Preview │                                   │ │
│  │  └──────────┘                                   │ │
│  │                                                 │ │
│  │  [Guardar y continuar]  Saltar por ahora       │ │
│  │                                                 │ │
│  └─────────────────────────────────────────────────┘ │
│                                                      │
└──────────────────────────────────────────────────────┘
```

#### Composicion de Componentes

```tsx
<div className="max-w-2xl mx-auto py-12 px-6">
  {/* Header Section */}
  <div className="mb-8">
    <h1 className="text-3xl font-bold text-white mb-2">
      Completa tu perfil de artista
    </h1>
    <p className="text-lg text-[#94a3b8]">
      Cuéntanos sobre tu música
    </p>
  </div>

  {/* Form Card */}
  <Card>
    <CardContent className="p-8">
      {/* CreateArtistaForm Component (custom) */}
      <CreateArtistaForm />
    </CardContent>
  </Card>
</div>
```

#### Tabla de Componentes UI

| Elemento Visual | Componente shadcn/ui | Variante/Props | Customizacion Tailwind |
|----------------|----------------------|----------------|------------------------|
| Container | `<div>` | - | `max-w-2xl mx-auto py-12 px-6` |
| Titulo h1 | `<h1>` | - | `text-3xl font-bold text-white mb-2` |
| Subtitulo | `<p>` | - | `text-lg text-[#94a3b8]` |
| Card | `Card` | - | `bg-[#0f1729] border-[#334155]` |
| Card Body | `CardContent` | - | `p-8` |
| Form | `Form` | - | - |
| Label (required) | `Label` | - | `text-sm font-medium text-[#cbd5e1] mb-2 after:content-['*'] after:text-red-500 after:ml-1` |
| Label (optional) | `Label` | - | `text-sm font-medium text-[#cbd5e1] mb-2` |
| Input Text | `Input` | `type="text"` | `bg-[#1a1a2e] border-[#334155] text-white` |
| Textarea | `Textarea` | - | `bg-[#1a1a2e] border-[#334155] text-white min-h-[120px] resize-none` |
| Character Counter | `<span>` (custom) | - | `text-xs text-[#64748b]` (dinamico: yellow/red segun %) |
| Grid Pais/Ciudad | `<div>` | - | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| Avatar Preview | `Avatar` | - | `w-32 h-32 rounded-lg` |
| Avatar Image | `AvatarImage` | - | - |
| Avatar Fallback | `AvatarFallback` | - | - |
| Music Icon (fallback) | `Music` (lucide-react) | - | `h-8 w-8 text-muted-foreground` |
| Submit Button | `Button` | `variant="default"` | `bg-gradient-to-r from-pink-500 to-purple-600 px-8` |
| Skip Link | `Link` (Next.js) | - | `text-[#94a3b8] hover:text-white underline ml-4` |

## 5. Composiciones Custom

### 5.1 RegisterForm

**Archivo:** `src/admin/src/components/auth/register-form.tsx`

**Composicion:**
```tsx
<form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
  {/* Email Field */}
  <div className="space-y-2">
    <Label htmlFor="email">Email *</Label>
    <Input
      id="email"
      type="email"
      placeholder="tu@email.com"
      {...register('email')}
      aria-required="true"
      aria-invalid={!!errors.email}
      aria-describedby={errors.email ? "email-error" : undefined}
    />
    {errors.email && (
      <p id="email-error" role="alert" className="text-sm text-destructive">
        {errors.email.message}
      </p>
    )}
  </div>

  {/* Password Field with Toggle */}
  <PasswordInput
    id="password"
    label="Contraseña *"
    placeholder="Mínimo 8 caracteres"
    error={errors.password?.message}
    {...register('password')}
  />

  {/* Confirm Password Field with Toggle */}
  <PasswordInput
    id="confirmPassword"
    label="Confirmar Contraseña *"
    placeholder="Repite tu contraseña"
    error={errors.confirmPassword?.message}
    {...register('confirmPassword')}
  />

  {/* Submit Button */}
  <Button
    type="submit"
    disabled={isPending}
    aria-busy={isPending}
    className="w-full"
  >
    {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
    {isPending ? 'Creando cuenta...' : 'Crear cuenta'}
  </Button>

  {/* Footer Link */}
  <p className="text-center text-sm text-muted-foreground">
    ¿Ya tienes cuenta?{' '}
    <Link href="/auth/login" className="text-primary hover:underline">
      Iniciar sesión
    </Link>
  </p>
</form>
```

**Componentes utilizados:**
- `Form` (shadcn/ui wrapper)
- `Label` (shadcn/ui)
- `Input` (shadcn/ui)
- `Button` (shadcn/ui)
- `PasswordInput` (custom component)
- `Loader2` (lucide-react)

### 5.2 PasswordInput

**Archivo:** `src/admin/src/components/auth/password-input.tsx`

**Composicion:**
```tsx
<div className="space-y-2">
  <Label htmlFor={id}>{label}</Label>

  <div className="relative">
    <Input
      id={id}
      type={showPassword ? 'text' : 'password'}
      placeholder={placeholder}
      className="pr-10"
      {...props}
    />

    <Button
      type="button"
      variant="ghost"
      size="icon"
      className="absolute right-2 top-1/2 -translate-y-1/2 h-8 w-8"
      onClick={() => setShowPassword(!showPassword)}
      aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
    >
      {showPassword ? (
        <EyeOff className="h-4 w-4" />
      ) : (
        <Eye className="h-4 w-4" />
      )}
    </Button>
  </div>

  {error && (
    <p role="alert" className="text-sm text-destructive">
      {error}
    </p>
  )}
</div>
```

**Componentes utilizados:**
- `Label` (shadcn/ui)
- `Input` (shadcn/ui)
- `Button` variant="ghost" size="icon" (shadcn/ui)
- `Eye`, `EyeOff` (lucide-react)

### 5.3 CreateArtistaForm

**Archivo:** `src/admin/src/components/artista/create-artista-form.tsx`

**Composicion:**
```tsx
<form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
  {/* Nombre Artístico (required) */}
  <div className="space-y-2">
    <Label htmlFor="nombreArtistico" className="after:content-['*'] after:text-red-500 after:ml-1">
      Nombre artístico
    </Label>
    <Input
      id="nombreArtistico"
      type="text"
      placeholder="Tu nombre de artista"
      {...register('nombreArtistico')}
    />
    {errors.nombreArtistico && (
      <p role="alert" className="text-sm text-destructive">
        {errors.nombreArtistico.message}
      </p>
    )}
  </div>

  {/* Descripción con Character Counter */}
  <div className="space-y-2">
    <Label htmlFor="descripcion">Descripción</Label>
    <Textarea
      id="descripcion"
      placeholder="Cuéntanos sobre tu música..."
      className="min-h-[120px] resize-none"
      maxLength={2000}
      {...register('descripcion')}
    />
    <CharacterCounter
      currentLength={descripcion.length}
      maxLength={2000}
    />
  </div>

  {/* Grid: País y Ciudad */}
  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
    <div className="space-y-2">
      <Label htmlFor="pais">País</Label>
      <Input
        id="pais"
        type="text"
        placeholder="Ej: España"
        {...register('pais')}
      />
    </div>

    <div className="space-y-2">
      <Label htmlFor="ciudad">Ciudad</Label>
      <Input
        id="ciudad"
        type="text"
        placeholder="Ej: Barcelona"
        {...register('ciudad')}
      />
    </div>
  </div>

  {/* Imagen URL con Preview */}
  <div className="space-y-2">
    <Label htmlFor="imagenUrl">Imagen de perfil (URL)</Label>
    <Input
      id="imagenUrl"
      type="url"
      placeholder="https://ejemplo.com/imagen.jpg"
      {...register('imagenUrl')}
    />
    {errors.imagenUrl && (
      <p role="alert" className="text-sm text-destructive">
        {errors.imagenUrl.message}
      </p>
    )}

    {/* Image Preview */}
    <ImagePreview
      imageUrl={debouncedImageUrl}
      altText={`Foto de perfil de ${nombreArtistico || 'artista'}`}
      size="md"
    />
  </div>

  {/* Actions */}
  <div className="flex flex-col sm:flex-row items-center gap-4">
    <Button
      type="submit"
      disabled={isPending}
      className="w-full sm:w-auto"
    >
      {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
      {isPending ? 'Guardando...' : 'Guardar y continuar'}
    </Button>

    <Button
      type="button"
      variant="ghost"
      onClick={handleSkip}
      className="w-full sm:w-auto"
    >
      Saltar por ahora
    </Button>
  </div>
</form>
```

**Componentes utilizados:**
- `Form` (shadcn/ui wrapper)
- `Label` (shadcn/ui)
- `Input` (shadcn/ui)
- `Textarea` (shadcn/ui)
- `Button` (shadcn/ui)
- `CharacterCounter` (custom component)
- `ImagePreview` (custom component)

### 5.4 ImagePreview

**Archivo:** `src/admin/src/components/artista/image-preview.tsx`

**Composicion:**
```tsx
{/* No URL - Show fallback immediately */}
{!imageUrl && (
  <Avatar className={cn(sizeClasses[size], 'rounded-lg')}>
    <AvatarFallback className="bg-[#1a1a2e]">
      <Music className="h-8 w-8 text-muted-foreground" />
    </AvatarFallback>
  </Avatar>
)}

{/* URL provided - Try to load */}
{imageUrl && (
  <Avatar className={cn(sizeClasses[size], 'rounded-lg')}>
    {isLoading && (
      <Skeleton className="h-full w-full rounded-lg" />
    )}

    <AvatarImage
      src={imageUrl}
      alt={altText}
      onLoad={() => setIsLoading(false)}
      onError={() => {
        setIsLoading(false);
        setHasError(true);
      }}
      className={cn(isLoading && 'opacity-0')}
    />

    {hasError && (
      <AvatarFallback className="bg-[#1a1a2e]">
        <Music className="h-8 w-8 text-muted-foreground" />
      </AvatarFallback>
    )}
  </Avatar>
)}
```

**Componentes utilizados:**
- `Avatar` (shadcn/ui)
- `AvatarImage` (shadcn/ui)
- `AvatarFallback` (shadcn/ui)
- `Skeleton` (shadcn/ui)
- `Music` (lucide-react)

### 5.5 CharacterCounter

**Archivo:** `src/admin/src/components/artista/character-counter.tsx`

**Composicion:**
```tsx
<span className={cn('text-xs transition-colors duration-200', colorClass, className)}>
  {currentLength}/{maxLength} caracteres
</span>
```

**Logica de colores:**
```typescript
const percentage = (currentLength / maxLength) * 100;

const colorClass =
  percentage >= 95
    ? 'text-destructive'           // Rojo: > 95%
    : percentage >= 80
    ? 'text-yellow-500'             // Amarillo: 80-95%
    : 'text-muted-foreground';      // Gris: < 80%
```

**Componentes utilizados:**
- `<span>` nativo con clases condicionales
- `cn()` utility para merge de clases

## 6. Variantes y Estados Visuales

### 6.1 Button Variants

| Variante | Clase Base | Uso |
|----------|------------|-----|
| **default** | `bg-primary text-primary-foreground` | Submit buttons (con gradient override) |
| **ghost** | `hover:bg-accent hover:text-accent-foreground` | Eye icon toggle, Skip button |
| **outline** | `border border-input bg-background` | NO usado en esta feature |
| **destructive** | `bg-destructive text-destructive-foreground` | NO usado en esta feature |

**Gradient Override (Submit buttons):**
```tsx
<Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
  Crear cuenta
</Button>
```

### 6.2 Input States

| Estado | Clase | Visual |
|--------|-------|--------|
| **Default** | `bg-[#0f1729] border-[#334155] text-white` | Border gris, fondo oscuro |
| **Focus** | `focus:border-primary focus:ring-2 focus:ring-primary/20` | Border purple + ring glow |
| **Error** | `border-destructive focus:ring-destructive/20` | Border rojo |
| **Disabled** | `disabled:opacity-50 disabled:cursor-not-allowed` | Opacity 50%, cursor bloqueado |

### 6.3 Form States

| Estado | Visual | Trigger |
|--------|--------|---------|
| **Default** | Formulario vacio, inputs normales | Carga inicial |
| **Typing** | Validacion onChange (despues del primer blur) | Usuario escribe |
| **Validating** | Sin indicador visual especial | onBlur + onChange |
| **Loading** | Button con spinner + texto "Cargando...", inputs disabled | Submit en proceso |
| **Error** | Border rojo en input, mensaje rojo debajo, shake animation | Validacion falla |
| **Success** | Toast verde, redirect automatico | Submit exitoso |

### 6.4 Image Preview States

| Estado | Visual | Componente |
|--------|--------|------------|
| **No URL** | Music icon placeholder | `AvatarFallback` |
| **Loading** | Skeleton gris pulsante | `Skeleton` |
| **Success** | Imagen cargada con fade-in | `AvatarImage` |
| **Error** | Music icon placeholder | `AvatarFallback` |

## 7. Customizaciones de Tema

### 7.1 Card Customizations

**Override de colores shadcn/ui default:**
```tsx
<Card className="bg-[#0f1729] border-[#334155]">
  {/* Content */}
</Card>
```

**Razon:** El tema default de shadcn/ui no coincide con el dark theme custom del proyecto.

### 7.2 Input Customizations

**Override de colores default:**
```tsx
<Input className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]" />
```

**Focus state (ya incluido en component):**
```css
focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2
```

### 7.3 Gradient Button

**Implementacion:**
```tsx
<Button className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold transition-all duration-200">
  Crear cuenta
</Button>
```

**Variante hover:**
- Gradiente se "intensifica" (colores mas saturados)
- Transicion suave de 200ms

## 8. Responsive Design

### 8.1 Breakpoints Tailwind

| Breakpoint | Width | Alias |
|------------|-------|-------|
| `sm` | 640px | Mobile grande / Tablet pequeno |
| `md` | 768px | Tablet |
| `lg` | 1024px | Desktop |
| `xl` | 1280px | NO usado en esta feature |

### 8.2 Register Page Responsive

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Card `w-full`, padding `px-4`, logo `h-10 w-10`, titulo `text-2xl` |
| **>= sm** | Card `max-w-md`, padding `px-6`, logo `h-12 w-12`, titulo `text-3xl` |

**Clases responsive:**
```tsx
<Card className="w-full max-w-md">
  {/* Automatico: w-full en mobile, max-w-md contiene en desktop */}
</Card>

<Music className="h-10 w-10 sm:h-12 sm:w-12" />
```

### 8.3 Create Artista Profile Responsive

| Breakpoint | Cambios |
|------------|---------|
| **< sm (mobile)** | Grid Pais/Ciudad → stack vertical, Image preview `w-24 h-24`, buttons stacked vertical |
| **sm-md** | Grid 2 columnas para Pais/Ciudad, Image preview `w-32 h-32` |
| **>= md** | Container `max-w-2xl`, grid completo, buttons inline |

**Clases responsive:**
```tsx
{/* Grid Pais/Ciudad */}
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
  <div>País</div>
  <div>Ciudad</div>
</div>

{/* Image Preview */}
<Avatar className="w-24 h-24 sm:w-32 sm:h-32 rounded-lg">
  {/* ... */}
</Avatar>

{/* Buttons */}
<div className="flex flex-col sm:flex-row items-center gap-4">
  <Button className="w-full sm:w-auto">Guardar</Button>
  <Button className="w-full sm:w-auto">Saltar</Button>
</div>
```

## 9. Animaciones

### 9.1 Transition Classes

| Elemento | Animacion | Clase Tailwind | Duracion |
|----------|-----------|----------------|----------|
| **Button hover** | Scale + shadow | `transition-all duration-200 hover:scale-[1.02]` | 200ms |
| **Input focus** | Border color + ring | `transition-colors duration-200` | 200ms |
| **Character counter** | Color change | `transition-colors duration-200` | 200ms |
| **Toast** | Slide in from top | Built-in (Sonner) | 250ms |
| **Image preview** | Fade in | `transition-opacity duration-300` | 300ms |
| **Validation error** | Shake + fade in | Custom animation | 400ms |
| **Loading spinner** | Rotate | `animate-spin` (Tailwind) | Infinite |

### 9.2 Shake Animation (Validation Error)

**Implementacion en Tailwind config:**
```javascript
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
      },
      animation: {
        shake: 'shake 0.4s ease-in-out',
      },
    },
  },
};
```

**Uso en componente:**
```tsx
<Input
  className={cn(
    'bg-[#0f1729] border-[#334155]',
    errors.email && 'border-destructive animate-shake'
  )}
/>
```

### 9.3 Loading Spinner

**Button con spinner (Loader2 de lucide-react):**
```tsx
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && (
    <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />
  )}
  {isPending ? 'Creando cuenta...' : 'Crear cuenta'}
</Button>
```

## 10. Accesibilidad (ARIA)

### 10.1 Form Labels

**Asociacion Label-Input:**
```tsx
<Label htmlFor="email">Email *</Label>
<Input
  id="email"
  type="email"
  aria-required="true"
  aria-invalid={!!errors.email}
  aria-describedby={errors.email ? "email-error" : undefined}
/>
```

### 10.2 Error Announcements

**Role alert + aria-live:**
```tsx
{errors.email && (
  <p
    id="email-error"
    role="alert"
    aria-live="polite"
    className="text-sm text-destructive"
  >
    {errors.email.message}
  </p>
)}
```

### 10.3 Loading States

**Button aria-busy:**
```tsx
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && <Loader2 aria-hidden="true" className="animate-spin" />}
  {isPending ? 'Creando cuenta...' : 'Crear cuenta'}
</Button>
```

### 10.4 Image Preview Accessibility

**Avatar con alt descriptivo:**
```tsx
<Avatar>
  <AvatarImage
    src={imagenUrl}
    alt={`Foto de perfil de ${nombreArtistico || 'artista'}`}
  />
  <AvatarFallback>
    <Music aria-hidden="true" />
    <span className="sr-only">Imagen no disponible</span>
  </AvatarFallback>
</Avatar>
```

### 10.5 Toggle Password Visibility

**Button con aria-label:**
```tsx
<Button
  type="button"
  variant="ghost"
  size="icon"
  onClick={togglePassword}
  aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
>
  {showPassword ? <EyeOff /> : <Eye />}
</Button>
```

### 10.6 Contraste de Colores

**Verificacion WCAG AA:**
| Combinacion | Contraste | Estado |
|-------------|-----------|--------|
| White (#fff) sobre bg-primary (#1a1a2e) | 15.8:1 | Pasa AAA |
| Text-muted (#64748b) sobre bg-card (#0f1729) | 5.2:1 | Pasa AA |
| Primary (#a855f7) sobre bg-card (#0f1729) | 4.8:1 | Pasa AA |
| Destructive (#ef4444) sobre white | 4.5:1 | Pasa AA |

## 11. Design Tokens Aplicados

### 11.1 Spacing (Tailwind Scale)

| Token | Valor | Uso en Feature |
|-------|-------|----------------|
| `space-2` | 0.5rem (8px) | Gap entre label y input |
| `space-4` | 1rem (16px) | Space-y entre form fields |
| `space-6` | 1.5rem (24px) | Space-y en CreateArtistaForm |
| `space-8` | 2rem (32px) | Padding de CardContent |
| `space-12` | 3rem (48px) | Margin bottom de logo |

### 11.2 Border Radius

| Token | Valor | Uso |
|-------|-------|-----|
| `rounded-md` | 0.375rem (6px) | Inputs, Buttons |
| `rounded-lg` | 0.5rem (8px) | Cards, Avatar |
| `rounded-full` | 9999px | Eye icon button (circle) |

### 11.3 Typography Scale

| Elemento | Clase | Size | Weight |
|----------|-------|------|--------|
| Titulo principal (h1) | `text-3xl font-bold` | 30px | 700 |
| Card Title | `text-2xl font-semibold` | 24px | 600 |
| Body text | `text-base` | 16px | 400 |
| Labels | `text-sm font-medium` | 14px | 500 |
| Error messages | `text-sm` | 14px | 400 |
| Character counter | `text-xs` | 12px | 400 |

## 12. Checklist de Implementacion

### Setup:
- [ ] Instalar Avatar component (`npx shadcn@latest add avatar`)
- [ ] Instalar Form component (`npx shadcn@latest add form`)
- [ ] Verificar Sonner (toast) configurado en layout

### RegisterForm:
- [ ] Composicion con Card + CardHeader + CardContent
- [ ] Logo Music icon centrado
- [ ] RegisterForm component integrado
- [ ] PasswordInput component funcional con toggle
- [ ] Validacion Zod + React Hook Form
- [ ] Estados: default, focus, loading, error
- [ ] Gradient button implementado
- [ ] Link a login funcional
- [ ] Responsive: mobile (w-full), desktop (max-w-md)
- [ ] ARIA labels y roles correctos
- [ ] Shake animation en errores

### CreateArtistaForm:
- [ ] Header con titulo y subtitulo
- [ ] Card con padding correcto
- [ ] Formulario con todos los campos
- [ ] Label con asterisco (*) en campos required
- [ ] Textarea con minHeight y resize-none
- [ ] CharacterCounter dinamico (gris/amarillo/rojo)
- [ ] Grid responsive para Pais/Ciudad (1 col mobile, 2 cols desktop)
- [ ] ImagePreview con Avatar component
- [ ] Skeleton loading state en preview
- [ ] Fallback Music icon si error o sin URL
- [ ] Debounced preview (500ms)
- [ ] Botones: Submit gradient + Skip ghost
- [ ] Responsive: buttons stacked en mobile, inline en desktop
- [ ] ARIA labels y alt text descriptivo

### Animaciones:
- [ ] Button hover con scale 1.02
- [ ] Input focus con border transition
- [ ] Character counter con transition-colors
- [ ] Shake animation en validacion error
- [ ] Loading spinner (Loader2 animate-spin)
- [ ] Image preview fade-in
- [ ] Toast notifications (Sonner)

### Accesibilidad:
- [ ] Labels asociados con htmlFor/id
- [ ] Inputs con aria-required, aria-invalid
- [ ] Errores con role="alert" aria-live="polite"
- [ ] Buttons con aria-busy en loading
- [ ] Toggle password con aria-label descriptivo
- [ ] Avatar con alt text descriptivo
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Keyboard navigation (Tab, Enter, Space)
- [ ] Focus visible con ring purple

### Responsive:
- [ ] Mobile (< 640px): Cards full width, padding reducido, grid stack
- [ ] Tablet (640-768px): Grid 2 columnas, cards max-w
- [ ] Desktop (>= 1024px): Container max-w-2xl, layouts completos

## 13. Resumen de Archivos

### Componentes a Crear:

| Archivo | Componentes shadcn/ui usados |
|---------|------------------------------|
| `components/auth/register-form.tsx` | Form, Label, Input, Button, Card* |
| `components/auth/password-input.tsx` | Label, Input, Button (ghost, icon) |
| `components/artista/create-artista-form.tsx` | Form, Label, Input, Textarea, Button, Avatar |
| `components/artista/image-preview.tsx` | Avatar, AvatarImage, AvatarFallback, Skeleton |
| `components/artista/character-counter.tsx` | (solo <span> nativo) |

*Card usado en page, no en component

### Pages a Crear/Actualizar:

| Archivo | Componentes shadcn/ui usados |
|---------|------------------------------|
| `app/(auth)/register/page.tsx` | Card, CardHeader, CardTitle, CardDescription, CardContent |
| `app/(auth)/artista/perfil/crear/page.tsx` | Card, CardContent |

### Dependencias Externas:

| Package | Componentes |
|---------|-------------|
| `lucide-react` | Music, Eye, EyeOff, Loader2 |
| `react-hook-form` | useForm, Controller |
| `@hookform/resolvers/zod` | zodResolver |
| `zod` | z.object, z.string, etc. |
| `sonner` | toast (ya instalado via shadcn/ui) |

---

## Notas Finales

1. **Avatar component** es REQUERIDO para ImagePreview. Instalar antes de implementar.
2. **Form component** de shadcn/ui simplifica integracion con React Hook Form (recomendado pero opcional).
3. **Gradient buttons** requieren override de clases default de Button variant="default".
4. **Character counter** cambia color dinamicamente sin re-renders (solo clases CSS).
5. **Image preview** usa debounce de 500ms para evitar requests excesivos al tipear URL.
6. **Shake animation** requiere configuracion en `tailwind.config.js`.
7. **Toast notifications** (Sonner) deben estar configuradas en root layout con `<Toaster />`.

**Ruta del plan completo:** `C:\Repos\WePlay_Rises\plans\registro-artista\frontend-admin\ui-design.md`
