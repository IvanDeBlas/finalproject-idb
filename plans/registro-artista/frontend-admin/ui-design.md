# Diseno UI: Registro de Artista (Admin)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/admin
**Mockup de referencia:** WPR_4-Login.png
**Template de referencia:** Dashtail login-form.tsx

## 1. Resumen

- Componentes shadcn/ui: 8 (Card, Input, Button, Label, Textarea, Avatar, Skeleton, Badge)
- Composiciones custom: 3 (RegisterForm, CreateArtistaForm, AuthLayout)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)
- Formularios: 2 (Registro usuario, Crear perfil artista)
- Estados UI: 7 (Default, Focus, Typing, Loading, Error, Success, Disabled)

## 2. Paleta de Colores (Dark Theme)

| Uso | Variable CSS | Tailwind Class | Hex | Contexto |
|-----|--------------|----------------|-----|----------|
| Background Primary | --bg-primary | bg-[#1a1a2e] | #1a1a2e | Fondo general |
| Background Card | --bg-card | bg-[#0f1729] | #0f1729 | Cards y formularios |
| Background Hover | --bg-card-hover | bg-[#1e2a42] | #1e2a42 | Hover en cards |
| Primary Color | --primary-color | text-primary | #a855f7 | Purple accent |
| Primary Gradient | --primary-gradient | bg-gradient-to-r from-pink-500 to-purple-600 | #ec4899 → #a855f7 | Botones principales |
| Text Primary | --text-primary | text-white | #ffffff | Titulos y textos principales |
| Text Secondary | --text-secondary | text-[#94a3b8] | #94a3b8 | Subtitulos y placeholders |
| Text Muted | --text-muted | text-[#64748b] | #64748b | Textos auxiliares |
| Text Label | --text-label | text-[#cbd5e1] | #cbd5e1 | Labels de formularios |
| Border Primary | --border-primary | border-[#334155] | #334155 | Borders generales |
| Border Focus | --border-focus | border-primary | #a855f7 | Inputs en foco |
| Border Error | --border-error | border-destructive | #ef4444 | Inputs con error |
| Status Success | --status-success | text-green-500 | #10b981 | Mensajes de exito |
| Status Error | --status-error | text-red-500 | #ef4444 | Mensajes de error |
| Input Background | --input-bg | bg-[#0f1729] | #0f1729 | Fondo de inputs |

**Verificacion de contraste:**
- White (#ffffff) sobre bg-primary (#1a1a2e) = 15.8:1 (WCAG AAA ✓)
- Text secondary (#94a3b8) sobre bg-card (#0f1729) = 7.2:1 (WCAG AAA ✓)
- Primary (#a855f7) sobre bg-card (#0f1729) = 5.8:1 (WCAG AA ✓)

## 3. Componentes por Screen

### 3.1 Registro de Usuario (`/auth/register`)

**Mockup:** WPR_4-Login.png (usar mismo estilo adaptado para registro)
**Tipo:** Auth form
**Composicion principal:** RegisterForm

#### Layout Desktop (> 1024px)

```
┌────────────────────────────────────────────────────────────┐
│                                                            │
│                      ┌──────────┐                          │
│                      │   LOGO   │                          │
│                      └──────────┘                          │
│                    WePlay Rises                            │
│                                                            │
│              ┌─────────────────────────────┐               │
│              │                             │               │
│              │  Crea tu cuenta             │               │
│              │  Completa tu registro...    │               │
│              │                             │               │
│              │  Email                      │               │
│              │  [_______________________]  │               │
│              │                             │               │
│              │  Contrasena                 │               │
│              │  [_______________________] 👁 │               │
│              │                             │               │
│              │  Confirmar Contrasena       │               │
│              │  [_______________________] 👁 │               │
│              │                             │               │
│              │  [Crear cuenta]             │               │
│              │  (gradient button)          │               │
│              │                             │               │
│              │  Ya tienes cuenta?          │               │
│              │  Iniciar sesion             │               │
│              │                             │               │
│              └─────────────────────────────┘               │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

#### Layout Mobile (< 640px)

```
┌─────────────────────────┐
│       [LOGO]            │
│     WePlay Rises        │
│                         │
│  Crea tu cuenta         │
│  Completa tu...         │
│                         │
│  Email                  │
│  [_________________]    │
│                         │
│  Contrasena             │
│  [_________________] 👁  │
│                         │
│  Confirmar Contrasena   │
│  [_________________] 👁  │
│                         │
│  [Crear cuenta]         │
│  (full width)           │
│                         │
│  Ya tienes cuenta?      │
│  Iniciar sesion         │
│                         │
└─────────────────────────┘
```

#### Componentes - RegisterForm

| Elemento | Componente shadcn | Variante/Props | Customizacion Tailwind |
|----------|-------------------|----------------|------------------------|
| **Container** | `<div>` | - | `min-h-screen flex items-center justify-center bg-[#1a1a2e] p-4` |
| **Logo Wrapper** | `<div>` | - | `flex flex-col items-center mb-8` |
| **Logo Icon** | Custom SVG | - | `h-12 w-12 md:h-16 md:w-16 text-primary` |
| **Brand Text** | `<h1>` | - | `text-2xl md:text-3xl font-bold text-white mt-3` |
| **Form Card** | `<Card>` | - | `w-full max-w-md bg-[#0f1729] border-[#334155]` |
| **Card Content** | `<CardContent>` | - | `p-6 md:p-8` |
| **Title** | `<CardTitle>` | - | `text-2xl md:text-3xl font-bold text-white mb-2` |
| **Subtitle** | `<CardDescription>` | - | `text-base text-[#94a3b8] mb-6` |
| **Form** | `<form>` | react-hook-form | `space-y-4` |
| **Field Wrapper** | `<div>` | - | `space-y-2` |
| **Label** | `<Label>` | htmlFor | `text-sm font-medium text-[#cbd5e1]` |
| **Email Input** | `<Input>` | type="email" | `bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b]` |
| **Password Input** | `<Input>` | type="password" | Same as Email |
| **Input (Focus)** | - | - | `focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| **Input (Error)** | - | - | `border-destructive focus-visible:ring-destructive` |
| **Eye Icon Toggle** | Lucide `Eye` / `EyeOff` | - | `absolute right-3 top-1/2 -translate-y-1/2 h-5 w-5 text-[#94a3b8] hover:text-white cursor-pointer transition-colors` |
| **Error Message** | `<p>` | role="alert" | `text-sm text-destructive mt-1` |
| **Submit Button** | `<Button>` | type="submit" | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold shadow-lg hover:shadow-xl transition-all duration-200` |
| **Loading Spinner** | Lucide `Loader2` | - | `mr-2 h-4 w-4 animate-spin` |
| **Footer Text** | `<p>` | - | `text-center text-sm text-[#94a3b8] mt-6` |
| **Link to Login** | Next.js `<Link>` | - | `text-primary hover:text-purple-400 hover:underline transition-colors` |

#### Composicion JSX - RegisterForm

```tsx
<div className="min-h-screen flex items-center justify-center bg-[#1a1a2e] p-4">
  {/* Logo */}
  <div className="flex flex-col items-center mb-8">
    <SiteLogo className="h-12 w-12 md:h-16 md:w-16 text-primary" />
    <h1 className="text-2xl md:text-3xl font-bold text-white mt-3">
      WePlay Rises
    </h1>
  </div>

  {/* Form Card */}
  <Card className="w-full max-w-md bg-[#0f1729] border-[#334155]">
    <CardContent className="p-6 md:p-8">
      <CardTitle className="text-2xl md:text-3xl font-bold text-white mb-2">
        Crea tu cuenta
      </CardTitle>
      <CardDescription className="text-base text-[#94a3b8] mb-6">
        Completa tu registro para comenzar tu carrera musical
      </CardDescription>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        {/* Email Field */}
        <div className="space-y-2">
          <Label htmlFor="email" className="text-sm font-medium text-[#cbd5e1]">
            Email
          </Label>
          <Input
            id="email"
            type="email"
            {...register('email')}
            disabled={isPending}
            className={cn(
              "bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b]",
              errors.email && "border-destructive"
            )}
            placeholder="tu@email.com"
            aria-invalid={!!errors.email}
            aria-describedby={errors.email ? "email-error" : undefined}
          />
          {errors.email && (
            <p id="email-error" role="alert" className="text-sm text-destructive mt-1">
              {errors.email.message}
            </p>
          )}
        </div>

        {/* Password Field */}
        <div className="space-y-2">
          <Label htmlFor="password" className="text-sm font-medium text-[#cbd5e1]">
            Contraseña
          </Label>
          <div className="relative">
            <Input
              id="password"
              type={showPassword ? "text" : "password"}
              {...register('password')}
              disabled={isPending}
              className={cn(
                "bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] pr-10",
                errors.password && "border-destructive"
              )}
              placeholder="Mínimo 8 caracteres"
              aria-invalid={!!errors.password}
              aria-describedby={errors.password ? "password-error" : undefined}
            />
            <button
              type="button"
              onClick={() => setShowPassword(!showPassword)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-[#94a3b8] hover:text-white transition-colors"
              aria-label={showPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
            >
              {showPassword ? (
                <EyeOff className="h-5 w-5" />
              ) : (
                <Eye className="h-5 w-5" />
              )}
            </button>
          </div>
          {errors.password && (
            <p id="password-error" role="alert" className="text-sm text-destructive mt-1">
              {errors.password.message}
            </p>
          )}
        </div>

        {/* Confirm Password Field */}
        <div className="space-y-2">
          <Label htmlFor="confirmPassword" className="text-sm font-medium text-[#cbd5e1]">
            Confirmar Contraseña
          </Label>
          <div className="relative">
            <Input
              id="confirmPassword"
              type={showConfirmPassword ? "text" : "password"}
              {...register('confirmPassword')}
              disabled={isPending}
              className={cn(
                "bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] pr-10",
                errors.confirmPassword && "border-destructive"
              )}
              placeholder="Repite tu contraseña"
              aria-invalid={!!errors.confirmPassword}
              aria-describedby={errors.confirmPassword ? "confirm-password-error" : undefined}
            />
            <button
              type="button"
              onClick={() => setShowConfirmPassword(!showConfirmPassword)}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-[#94a3b8] hover:text-white transition-colors"
              aria-label={showConfirmPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
            >
              {showConfirmPassword ? (
                <EyeOff className="h-5 w-5" />
              ) : (
                <Eye className="h-5 w-5" />
              )}
            </button>
          </div>
          {errors.confirmPassword && (
            <p id="confirm-password-error" role="alert" className="text-sm text-destructive mt-1">
              {errors.confirmPassword.message}
            </p>
          )}
        </div>

        {/* Submit Button */}
        <Button
          type="submit"
          disabled={isPending}
          className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
          aria-busy={isPending}
        >
          {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />}
          {isPending ? "Creando cuenta..." : "Crear cuenta"}
        </Button>

        {/* Footer Link */}
        <p className="text-center text-sm text-[#94a3b8] mt-6">
          ¿Ya tienes cuenta?{" "}
          <Link href="/auth/login" className="text-primary hover:text-purple-400 hover:underline transition-colors">
            Iniciar sesión
          </Link>
        </p>
      </form>
    </CardContent>
  </Card>
</div>
```

#### Props Interface - RegisterForm

```typescript
interface RegisterFormProps {
  // No props externas, form es self-contained
}

interface RegisterFormState {
  showPassword: boolean;
  showConfirmPassword: boolean;
  isPending: boolean; // De useMutation
}
```

---

### 3.2 Crear Perfil de Artista (`/artista/perfil/crear`)

**Tipo:** Onboarding form
**Composicion principal:** CreateArtistaForm

#### Layout Desktop (> 1024px)

```
┌────────────────────────────────────────────────────────────────┐
│  [LOGO] WePlay Rises                                           │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│           Completa tu perfil de artista                        │
│           Cuéntanos sobre tu música y proyectos                │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                                                          │  │
│  │  Nombre artístico *                                      │  │
│  │  [______________________________________________]         │  │
│  │                                                          │  │
│  │  Descripción                                             │  │
│  │  [______________________________________________]         │  │
│  │  [                                              ]         │  │
│  │  [            Textarea 4 rows                  ]         │  │
│  │  [______________________________________________]         │  │
│  │  0/2000 caracteres                                       │  │
│  │                                                          │  │
│  │  País                          Ciudad                    │  │
│  │  [____________________]        [____________________]    │  │
│  │                                                          │  │
│  │  Imagen de perfil (URL)                                  │  │
│  │  [______________________________________________]         │  │
│  │                                                          │  │
│  │  ┌──────────┐                                            │  │
│  │  │          │                                            │  │
│  │  │ Preview  │  (Si URL válida, mostrar imagen)           │  │
│  │  │          │                                            │  │
│  │  └──────────┘                                            │  │
│  │                                                          │  │
│  │  [Guardar y continuar]     Saltar por ahora             │  │
│  │  (gradient button)         (text link)                   │  │
│  │                                                          │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

#### Layout Mobile (< 640px)

```
┌─────────────────────────┐
│  [LOGO] WePlay Rises    │
├─────────────────────────┤
│                         │
│  Completa tu perfil     │
│  de artista             │
│                         │
│  Nombre artístico *     │
│  [_________________]    │
│                         │
│  Descripción            │
│  [_________________]    │
│  [_________________]    │
│  [_________________]    │
│  [_________________]    │
│  0/2000 caracteres      │
│                         │
│  País                   │
│  [_________________]    │
│                         │
│  Ciudad                 │
│  [_________________]    │
│                         │
│  Imagen URL             │
│  [_________________]    │
│                         │
│  ┌─────────┐            │
│  │ Preview │            │
│  └─────────┘            │
│                         │
│  [Guardar y continuar]  │
│  (full width)           │
│                         │
│  Saltar por ahora       │
│  (center link)          │
│                         │
└─────────────────────────┘
```

#### Componentes - CreateArtistaForm

| Elemento | Componente shadcn | Variante/Props | Customizacion Tailwind |
|----------|-------------------|----------------|------------------------|
| **Header** | `<header>` | - | `bg-[#0f1729] border-b border-[#334155] px-6 py-4 sticky top-0 z-50` |
| **Logo Link** | Next.js `<Link>` | - | `flex items-center gap-2 text-white font-bold hover:opacity-80 transition-opacity` |
| **Main Container** | `<main>` | - | `min-h-screen bg-[#1a1a2e] py-8 md:py-12 px-4` |
| **Content Wrapper** | `<div>` | - | `max-w-2xl mx-auto` |
| **Title** | `<h1>` | - | `text-2xl md:text-3xl font-bold text-white mb-2` |
| **Subtitle** | `<p>` | - | `text-base md:text-lg text-[#94a3b8] mb-6 md:mb-8` |
| **Form Card** | `<Card>` | - | `bg-[#0f1729] border-[#334155]` |
| **Card Content** | `<CardContent>` | - | `p-6 md:p-8` |
| **Form** | `<form>` | react-hook-form | `space-y-6` |
| **Field Wrapper** | `<div>` | - | `space-y-2` |
| **Label (required)** | `<Label>` | - | `text-sm font-medium text-[#cbd5e1] after:content-['*'] after:text-red-500 after:ml-1` |
| **Label (optional)** | `<Label>` | - | `text-sm font-medium text-[#cbd5e1]` |
| **Text Input** | `<Input>` | type="text" | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]` |
| **Textarea** | `<Textarea>` | rows={4} | `bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[120px] resize-none` |
| **Character Counter** | `<span>` | - | `text-xs text-[#64748b] mt-1 block` |
| **Counter (Warning)** | - | > 1800 chars | `text-yellow-500` |
| **Counter (Danger)** | - | > 1950 chars | `text-red-500` |
| **Grid (Country/City)** | `<div>` | - | `grid grid-cols-1 md:grid-cols-2 gap-4` |
| **Image Preview Container** | `<div>` | - | `mt-3` |
| **Avatar** | `<Avatar>` | size="2xl" | `w-32 h-32 rounded-lg` |
| **Avatar Image** | `<AvatarImage>` | - | - |
| **Avatar Fallback** | `<AvatarFallback>` | - | `bg-[#1a1a2e]` |
| **Fallback Icon** | Lucide `Music` | - | `h-12 w-12 text-[#64748b]` |
| **Footer Actions** | `<div>` | - | `flex flex-col md:flex-row items-center justify-between gap-4 mt-8` |
| **Submit Button** | `<Button>` | type="submit" | `w-full md:w-auto bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8 shadow-lg hover:shadow-xl transition-all duration-200` |
| **Skip Link** | Next.js `<Link>` | - | `text-[#94a3b8] hover:text-white underline transition-colors text-sm` |
| **Error Message** | `<p>` | role="alert" | `text-sm text-destructive mt-1` |
| **Loading Skeleton** | `<Skeleton>` | - | `w-32 h-32 rounded-lg bg-[#1a1a2e]` |

#### Composicion JSX - CreateArtistaForm

```tsx
<>
  {/* Header */}
  <header className="bg-[#0f1729] border-b border-[#334155] px-6 py-4 sticky top-0 z-50">
    <Link href="/dashboard" className="flex items-center gap-2 text-white font-bold hover:opacity-80 transition-opacity">
      <SiteLogo className="h-8 w-8 text-primary" />
      <span className="text-lg">WePlay Rises</span>
    </Link>
  </header>

  {/* Main Content */}
  <main className="min-h-screen bg-[#1a1a2e] py-8 md:py-12 px-4">
    <div className="max-w-2xl mx-auto">
      {/* Page Header */}
      <h1 className="text-2xl md:text-3xl font-bold text-white mb-2">
        Completa tu perfil de artista
      </h1>
      <p className="text-base md:text-lg text-[#94a3b8] mb-6 md:mb-8">
        Cuéntanos sobre tu música y proyectos
      </p>

      {/* Form Card */}
      <Card className="bg-[#0f1729] border-[#334155]">
        <CardContent className="p-6 md:p-8">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {/* Nombre Artistico */}
            <div className="space-y-2">
              <Label
                htmlFor="nombreArtistico"
                className="text-sm font-medium text-[#cbd5e1] after:content-['*'] after:text-red-500 after:ml-1"
              >
                Nombre artístico
              </Label>
              <Input
                id="nombreArtistico"
                type="text"
                {...register('nombreArtistico')}
                disabled={isPending}
                className={cn(
                  "bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]",
                  errors.nombreArtistico && "border-destructive"
                )}
                placeholder="Tu nombre de artista"
                aria-invalid={!!errors.nombreArtistico}
                aria-describedby={errors.nombreArtistico ? "nombre-error" : undefined}
              />
              {errors.nombreArtistico && (
                <p id="nombre-error" role="alert" className="text-sm text-destructive mt-1">
                  {errors.nombreArtistico.message}
                </p>
              )}
            </div>

            {/* Descripcion */}
            <div className="space-y-2">
              <Label htmlFor="descripcion" className="text-sm font-medium text-[#cbd5e1]">
                Descripción
              </Label>
              <Textarea
                id="descripcion"
                {...register('descripcion')}
                disabled={isPending}
                rows={4}
                className={cn(
                  "bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b] min-h-[120px] resize-none",
                  errors.descripcion && "border-destructive"
                )}
                placeholder="Cuéntanos sobre tu música, estilo, inspiraciones..."
                aria-invalid={!!errors.descripcion}
                aria-describedby="descripcion-counter"
              />
              <span
                id="descripcion-counter"
                className={cn(
                  "text-xs mt-1 block",
                  charCount > 1950 && "text-red-500",
                  charCount > 1800 && charCount <= 1950 && "text-yellow-500",
                  charCount <= 1800 && "text-[#64748b]"
                )}
              >
                {charCount}/2000 caracteres
              </span>
              {errors.descripcion && (
                <p role="alert" className="text-sm text-destructive mt-1">
                  {errors.descripcion.message}
                </p>
              )}
            </div>

            {/* Pais y Ciudad */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="pais" className="text-sm font-medium text-[#cbd5e1]">
                  País
                </Label>
                <Input
                  id="pais"
                  type="text"
                  {...register('pais')}
                  disabled={isPending}
                  className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]"
                  placeholder="Ej: España"
                />
                {errors.pais && (
                  <p role="alert" className="text-sm text-destructive mt-1">
                    {errors.pais.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="ciudad" className="text-sm font-medium text-[#cbd5e1]">
                  Ciudad
                </Label>
                <Input
                  id="ciudad"
                  type="text"
                  {...register('ciudad')}
                  disabled={isPending}
                  className="bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]"
                  placeholder="Ej: Barcelona"
                />
                {errors.ciudad && (
                  <p role="alert" className="text-sm text-destructive mt-1">
                    {errors.ciudad.message}
                  </p>
                )}
              </div>
            </div>

            {/* Imagen URL */}
            <div className="space-y-2">
              <Label htmlFor="imagenUrl" className="text-sm font-medium text-[#cbd5e1]">
                Imagen de perfil (URL)
              </Label>
              <Input
                id="imagenUrl"
                type="url"
                {...register('imagenUrl')}
                disabled={isPending}
                className={cn(
                  "bg-[#1a1a2e] border-[#334155] text-white placeholder:text-[#64748b]",
                  errors.imagenUrl && "border-destructive"
                )}
                placeholder="https://ejemplo.com/mi-foto.jpg"
                aria-invalid={!!errors.imagenUrl}
                aria-describedby={errors.imagenUrl ? "imagen-error" : undefined}
              />
              {errors.imagenUrl && (
                <p id="imagen-error" role="alert" className="text-sm text-destructive mt-1">
                  {errors.imagenUrl.message}
                </p>
              )}

              {/* Image Preview */}
              {watchImagenUrl && (
                <div className="mt-3">
                  {isImageLoading ? (
                    <Skeleton className="w-32 h-32 rounded-lg bg-[#1a1a2e]" />
                  ) : (
                    <Avatar className="w-32 h-32 rounded-lg">
                      <AvatarImage
                        src={watchImagenUrl}
                        alt="Vista previa de imagen de perfil"
                        onError={() => setImageError(true)}
                        onLoad={() => setImageError(false)}
                      />
                      <AvatarFallback className="bg-[#1a1a2e] rounded-lg">
                        <Music className="h-12 w-12 text-[#64748b]" aria-hidden="true" />
                      </AvatarFallback>
                    </Avatar>
                  )}
                </div>
              )}
            </div>

            {/* Footer Actions */}
            <div className="flex flex-col md:flex-row items-center justify-between gap-4 mt-8">
              <Button
                type="submit"
                disabled={isPending}
                className="w-full md:w-auto bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8 shadow-lg hover:shadow-xl transition-all duration-200"
                aria-busy={isPending}
              >
                {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />}
                {isPending ? "Guardando perfil..." : "Guardar y continuar"}
              </Button>

              <Link
                href="/dashboard"
                className="text-[#94a3b8] hover:text-white underline transition-colors text-sm"
              >
                Saltar por ahora
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  </main>
</>
```

#### Props Interface - CreateArtistaForm

```typescript
interface CreateArtistaFormProps {
  // No props externas, form es self-contained
}

interface CreateArtistaFormState {
  charCount: number; // Contador de caracteres de descripcion
  isPending: boolean; // De useMutation
  isImageLoading: boolean; // Estado de carga de preview
  imageError: boolean; // Error al cargar imagen
  watchImagenUrl: string; // Valor del input imagenUrl (watch)
}
```

---

## 4. Formularios - React Hook Form Integration

### 4.1 RegisterForm

**Schema Zod:** `registerSchema` (desde `@/shared/schemas/auth.schema`)

| Campo | Tipo | Validacion | Mensaje |
|-------|------|-----------|---------|
| `email` | email | required + email format | "El email es obligatorio" / "Formato de email inválido" |
| `password` | password | required + min 8 chars | "La contraseña es obligatoria" / "La contraseña debe tener al menos 8 caracteres" |
| `confirmPassword` | password | required + match password | "Confirme su contraseña" / "Las contraseñas no coinciden" |

**Form Setup:**
```typescript
const form = useForm<RegisterFormData>({
  resolver: zodResolver(registerSchema),
  mode: 'onBlur', // Validar al perder foco
  defaultValues: {
    email: '',
    password: '',
    confirmPassword: '',
  },
});

const {
  register,
  handleSubmit,
  formState: { errors },
} = form;
```

**Mutation Setup:**
```typescript
const registerMutation = useMutation({
  mutationFn: (data: RegisterRequest) =>
    api.post<RegisterResponse>(API_ROUTES.auth.register, data),
  onSuccess: (response) => {
    // Guardar JWT en localStorage
    localStorage.setItem('token', response.token);
    localStorage.setItem('userId', response.userId);

    // Redirect a crear perfil
    toast.success('Cuenta creada exitosamente');
    router.push(APP_ROUTES.artista.crearPerfil);
  },
  onError: (error: ApiError) => {
    // Mapear error code a mensaje
    const message = getErrorMessage(error.errorCode);
    toast.error(message);
  },
});

const { isPending } = registerMutation;
```

### 4.2 CreateArtistaForm

**Schema Zod:** `createArtistaSchema` (desde `@/shared/schemas/artista.schema`)

| Campo | Tipo | Validacion | Mensaje |
|-------|------|-----------|---------|
| `nombreArtistico` | text | required + max 200 chars | "El nombre artístico es obligatorio" / "Máximo 200 caracteres" |
| `descripcion` | textarea | optional + max 2000 chars | "Máximo 2000 caracteres" |
| `pais` | text | optional + max 100 chars | "Máximo 100 caracteres" |
| `ciudad` | text | optional + max 100 chars | "Máximo 100 caracteres" |
| `imagenUrl` | url | optional + valid URL | "Debe ser una URL válida" |

**Form Setup:**
```typescript
const form = useForm<CreateArtistaFormData>({
  resolver: zodResolver(createArtistaSchema),
  mode: 'onBlur',
  defaultValues: {
    nombreArtistico: '',
    descripcion: '',
    pais: '',
    ciudad: '',
    imagenUrl: '',
  },
});

const {
  register,
  handleSubmit,
  watch,
  formState: { errors },
} = form;

// Watch para character counter y image preview
const watchDescripcion = watch('descripcion');
const watchImagenUrl = watch('imagenUrl');

const charCount = watchDescripcion?.length || 0;
```

**Mutation Setup:**
```typescript
const createArtistaMutation = useMutation({
  mutationFn: (data: CreateArtistaRequest) =>
    api.post<Artista>(API_ROUTES.artistas.base, data),
  onSuccess: (response) => {
    toast.success('Perfil de artista creado exitosamente');

    // Invalidar cache de artistas
    queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.all });

    // Redirect a dashboard
    router.push(APP_ROUTES.dashboard);
  },
  onError: (error: ApiError) => {
    const message = getErrorMessage(error.errorCode);
    toast.error(message);
  },
});

const { isPending } = createArtistaMutation;
```

**Debounced Image Validation:**
```typescript
// Hook personalizado para debounce de imagen URL
const [isImageLoading, setIsImageLoading] = useState(false);
const [imageError, setImageError] = useState(false);

useEffect(() => {
  if (!watchImagenUrl) {
    setImageError(false);
    setIsImageLoading(false);
    return;
  }

  setIsImageLoading(true);

  const timer = setTimeout(() => {
    // La validacion real ocurre en el evento onLoad/onError del <AvatarImage>
    setIsImageLoading(false);
  }, 500); // Debounce de 500ms

  return () => clearTimeout(timer);
}, [watchImagenUrl]);
```

---

## 5. Estados de UI

### 5.1 Estados Globales (Aplican a ambos forms)

| Estado | Descripcion | Visual | Comportamiento |
|--------|-------------|--------|----------------|
| **Default** | Form vacio, sin interaccion | Inputs con placeholder, border gris, boton enabled | Usuario puede interactuar |
| **Focus** | Input seleccionado | Border purple (`#a855f7`), ring glow, placeholder se oculta | Keyboard listo para input |
| **Typing** | Usuario escribiendo | Texto visible, validacion diferida | Validar solo en `onBlur` o `onSubmit` |
| **Loading** | Procesando request | Boton con spinner, inputs disabled, cursor wait | Usuario no puede interactuar |
| **Error** | Validacion fallida | Border rojo, mensaje rojo debajo, icono de error | Usuario puede corregir |
| **Success** | Accion completada | Toast verde, redirect automatico | No mostrar en form, solo toast |
| **Disabled** | Input no editable | Background mas oscuro, opacity 50%, cursor not-allowed | Usuario no puede editar |

### 5.2 Estados Especificos - RegisterForm

| Estado | Trigger | Visual |
|--------|---------|--------|
| **Password Visible** | Click en eye icon | Input type="text", icono EyeOff |
| **Password Hidden** | Click en eye-off icon | Input type="password", icono Eye |
| **Email Exists (409)** | Backend error 409 | Error message "Este email ya está registrado. [Iniciar sesión]" con link |
| **Validation Error** | onBlur o onSubmit | Border rojo, mensaje especifico debajo del input |

### 5.3 Estados Especificos - CreateArtistaForm

| Estado | Trigger | Visual |
|--------|---------|--------|
| **Character Count Normal** | < 1800 chars | Contador gris (#64748b) |
| **Character Count Warning** | 1801-1950 chars | Contador amarillo (#f59e0b) |
| **Character Count Danger** | 1951-2000 chars | Contador rojo (#ef4444) |
| **Image Loading** | Cambio en imagenUrl (debounced) | Skeleton de 32x32 con animacion pulse |
| **Image Loaded** | onLoad evento exitoso | Avatar muestra imagen con border radius |
| **Image Error** | onError evento o URL invalida | Avatar muestra icono Music (placeholder) |
| **No Image** | imagenUrl vacio | No mostrar preview |

---

## 6. Animaciones

| Elemento | Animacion | Duracion | Easing | Trigger |
|----------|-----------|----------|--------|---------|
| **Button hover** | `scale(1.02) + shadow-xl` | 200ms | ease-in-out | Mouse hover |
| **Input focus** | `ring-2 ring-primary` | 150ms | ease | Focus |
| **Border color change** | `transition-colors` | 200ms | ease | Focus/Blur/Error |
| **Error message fade in** | `opacity 0 → 1` | 200ms | ease-out | Validation error |
| **Toast notification** | `slide-in from top` | 250ms | ease-out | Success/Error |
| **Loading spinner** | `rotate(360deg)` | 1000ms | linear | isPending = true |
| **Character counter color** | `text-color transition` | 200ms | ease | Char count change |
| **Image preview fade** | `opacity 0 → 1 + scale(0.95 → 1)` | 300ms | ease-in-out | Image loaded |
| **Skeleton pulse** | `opacity 0.5 ↔ 1` | 1500ms | ease-in-out | Image loading |

**CSS Classes:**
```css
/* Global transitions */
.transition-colors { transition: color 200ms ease, border-color 200ms ease, background-color 200ms ease; }
.transition-all { transition: all 200ms ease-in-out; }

/* Button gradient hover */
.hover\:shadow-xl:hover { box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3); }

/* Ring glow effect */
.focus-visible\:ring-2:focus-visible {
  outline: none;
  box-shadow: 0 0 0 2px var(--bg-card), 0 0 0 4px var(--primary-color), 0 0 20px rgba(168, 85, 247, 0.3);
}
```

---

## 7. Responsive Design

### 7.1 Breakpoints Tailwind

| Breakpoint | Width | Uso |
|------------|-------|-----|
| `sm` | 640px | Mobile large / Tablet small |
| `md` | 768px | Tablet |
| `lg` | 1024px | Desktop |
| `xl` | 1280px | Desktop large |
| `2xl` | 1536px | Desktop XL |

### 7.2 Cambios por Breakpoint

#### RegisterForm

| Elemento | Mobile (< 640px) | Desktop (> 1024px) |
|----------|------------------|-------------------|
| Container padding | `p-4` | `p-6 md:p-8` |
| Card max-width | `w-full` (con padding lateral) | `max-w-md` |
| Logo size | `h-10 w-10` | `h-12 w-12 md:h-16 md:w-16` |
| Title font size | `text-xl` | `text-2xl md:text-3xl` |
| Button width | `w-full` | `w-full` (se mantiene full) |
| Footer text | `text-xs` | `text-sm` |

#### CreateArtistaForm

| Elemento | Mobile (< 640px) | Desktop (> 1024px) |
|----------|------------------|-------------------|
| Main padding | `py-6 px-4` | `py-8 md:py-12 px-4` |
| Title font size | `text-xl` | `text-2xl md:text-3xl` |
| Subtitle font size | `text-sm` | `text-base md:text-lg` |
| Card padding | `p-4` | `p-6 md:p-8` |
| Country/City grid | `grid-cols-1` (stack vertical) | `md:grid-cols-2` |
| Image preview size | `w-24 h-24` | `w-32 h-32` |
| Button layout | `flex-col gap-2` (stack vertical) | `md:flex-row gap-4` |
| Button width | `w-full` | `w-full md:w-auto` |

**Classes responsive clave:**
```tsx
{/* Padding responsive */}
<div className="p-4 md:p-6 lg:p-8">

{/* Font size responsive */}
<h1 className="text-xl md:text-2xl lg:text-3xl">

{/* Grid responsive */}
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">

{/* Flex direction responsive */}
<div className="flex flex-col md:flex-row items-center gap-4">

{/* Width responsive */}
<Button className="w-full md:w-auto">
```

---

## 8. Accesibilidad (WCAG 2.1 AA)

### 8.1 Contraste de Color

| Combinacion | Ratio | Nivel | Cumple |
|-------------|-------|-------|--------|
| White (#fff) sobre bg-primary (#1a1a2e) | 15.8:1 | AAA | Si |
| Text secondary (#94a3b8) sobre bg-card (#0f1729) | 7.2:1 | AAA | Si |
| Primary (#a855f7) sobre bg-card (#0f1729) | 5.8:1 | AA | Si |
| Red error (#ef4444) sobre bg-card (#0f1729) | 6.1:1 | AA | Si |
| Text label (#cbd5e1) sobre bg-card (#0f1729) | 11.3:1 | AAA | Si |

### 8.2 ARIA Attributes

| Atributo | Uso | Ejemplo |
|----------|-----|---------|
| `aria-label` | Describir iconos sin texto visible | `<button aria-label="Mostrar contraseña">` |
| `aria-describedby` | Vincular input con error message | `<Input aria-describedby="email-error">` |
| `aria-invalid` | Indicar input con error | `<Input aria-invalid={!!errors.email}>` |
| `aria-required` | Indicar campo requerido | `<Input aria-required="true">` (campos con *) |
| `aria-busy` | Indicar estado de carga | `<Button aria-busy={isPending}>` |
| `role="alert"` | Mensajes de error (live region) | `<p role="alert">{error.message}</p>` |
| `aria-hidden` | Ocultar decorativos a screen readers | `<Loader2 aria-hidden="true" />` |

### 8.3 Keyboard Navigation

| Accion | Tecla | Comportamiento |
|--------|-------|----------------|
| Navegar entre campos | `Tab` | Focus en siguiente input (orden logico) |
| Navegar atras | `Shift + Tab` | Focus en input anterior |
| Submit form | `Enter` | Submit cuando focus en input (excepto textarea) |
| Toggle password | `Space` o `Enter` | Alternar visibilidad (cuando focus en boton) |
| Saltar links | `Tab` | Focus en links (Iniciar sesion, Saltar por ahora) |

**Tab Order:**
1. Email input
2. Password input
3. Password toggle button
4. Confirm password input
5. Confirm password toggle button
6. Submit button
7. Footer link

### 8.4 Focus States

Todos los elementos interactivos DEBEN tener focus visible:

```css
/* Global focus style */
.focus-visible:focus-visible {
  outline: none;
  box-shadow: 0 0 0 2px var(--bg-card), 0 0 0 4px var(--primary-color);
}

/* Input focus con ring glow */
input:focus-visible, textarea:focus-visible {
  outline: none;
  border-color: var(--primary-color);
  box-shadow:
    0 0 0 2px var(--bg-card),
    0 0 0 4px var(--primary-color),
    0 0 20px rgba(168, 85, 247, 0.3);
}

/* Button focus */
button:focus-visible {
  outline: none;
  box-shadow: 0 0 0 3px rgba(168, 85, 247, 0.5);
}

/* Link focus */
a:focus-visible {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
  border-radius: 2px;
}
```

### 8.5 Screen Reader Support

| Elemento | Texto anunciado |
|----------|-----------------|
| Email input con error | "Email. Inválido. Formato de email inválido." |
| Password toggle | "Mostrar contraseña, botón" / "Ocultar contraseña, botón" |
| Submit button loading | "Creando cuenta..., ocupado" |
| Character counter | "0 de 2000 caracteres" (actualizado live) |
| Image preview error | "Foto de perfil de [nombre]. Imagen no disponible." |

**Live Regions:**
```tsx
{/* Error messages con aria-live implícito (role="alert") */}
<p role="alert" className="text-destructive">
  {errors.email?.message}
</p>

{/* Character counter como live region polite */}
<span aria-live="polite" aria-atomic="true">
  {charCount}/2000 caracteres
</span>
```

### 8.6 Labels y Asociaciones

**Todos los inputs DEBEN tener label asociado:**

```tsx
{/* CORRECTO - Label con htmlFor */}
<Label htmlFor="email">Email</Label>
<Input id="email" type="email" />

{/* INCORRECTO - Sin label */}
<Input type="email" placeholder="Email" /> {/* NO HACER */}

{/* CORRECTO - Label con required indicator */}
<Label
  htmlFor="nombreArtistico"
  className="after:content-['*'] after:text-red-500 after:ml-1"
>
  Nombre artístico
</Label>
<Input id="nombreArtistico" aria-required="true" />
```

---

## 9. Componentes Auxiliares

### 9.1 PasswordInput Component (Custom)

**Composicion:**
- Input + Eye Icon Toggle
- Reutilizable en ambos forms

```tsx
interface PasswordInputProps {
  id: string;
  label: string;
  error?: string;
  disabled?: boolean;
  register: UseFormRegisterReturn;
  placeholder?: string;
  'aria-describedby'?: string;
}

export const PasswordInput: React.FC<PasswordInputProps> = ({
  id,
  label,
  error,
  disabled,
  register,
  placeholder,
  'aria-describedby': ariaDescribedby,
}) => {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="space-y-2">
      <Label htmlFor={id} className="text-sm font-medium text-[#cbd5e1]">
        {label}
      </Label>
      <div className="relative">
        <Input
          id={id}
          type={showPassword ? "text" : "password"}
          {...register}
          disabled={disabled}
          className={cn(
            "bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] pr-10",
            error && "border-destructive"
          )}
          placeholder={placeholder}
          aria-invalid={!!error}
          aria-describedby={ariaDescribedby}
        />
        <button
          type="button"
          onClick={() => setShowPassword(!showPassword)}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-[#94a3b8] hover:text-white transition-colors"
          aria-label={showPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
          tabIndex={0}
        >
          {showPassword ? (
            <EyeOff className="h-5 w-5" />
          ) : (
            <Eye className="h-5 w-5" />
          )}
        </button>
      </div>
      {error && (
        <p role="alert" className="text-sm text-destructive mt-1">
          {error}
        </p>
      )}
    </div>
  );
};
```

### 9.2 CharacterCounter Component (Custom)

**Composicion:**
- Span con color dinamico segun count

```tsx
interface CharacterCounterProps {
  current: number;
  max: number;
  id?: string;
}

export const CharacterCounter: React.FC<CharacterCounterProps> = ({
  current,
  max,
  id,
}) => {
  const percentage = (current / max) * 100;

  const colorClass = cn(
    "text-xs mt-1 block transition-colors duration-200",
    percentage > 97.5 && "text-red-500", // > 1950 chars
    percentage > 90 && percentage <= 97.5 && "text-yellow-500", // 1801-1950 chars
    percentage <= 90 && "text-[#64748b]" // 0-1800 chars
  );

  return (
    <span
      id={id}
      className={colorClass}
      aria-live="polite"
      aria-atomic="true"
    >
      {current}/{max} caracteres
    </span>
  );
};
```

### 9.3 ImagePreview Component (Custom)

**Composicion:**
- Avatar + Skeleton loader + Error fallback

```tsx
interface ImagePreviewProps {
  url: string | undefined;
  alt: string;
  size?: 'sm' | 'md' | 'lg';
}

export const ImagePreview: React.FC<ImagePreviewProps> = ({
  url,
  alt,
  size = 'md',
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [hasError, setHasError] = useState(false);

  useEffect(() => {
    if (!url) {
      setIsLoading(false);
      setHasError(false);
      return;
    }

    setIsLoading(true);
    setHasError(false);

    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 500);

    return () => clearTimeout(timer);
  }, [url]);

  const sizeClasses = {
    sm: 'w-24 h-24',
    md: 'w-32 h-32',
    lg: 'w-40 h-40',
  };

  if (!url) return null;

  if (isLoading) {
    return (
      <Skeleton className={cn(sizeClasses[size], "rounded-lg bg-[#1a1a2e]")} />
    );
  }

  return (
    <Avatar className={cn(sizeClasses[size], "rounded-lg")}>
      <AvatarImage
        src={url}
        alt={alt}
        onError={() => setHasError(true)}
        onLoad={() => setHasError(false)}
      />
      <AvatarFallback className="bg-[#1a1a2e] rounded-lg">
        <Music className="h-12 w-12 text-[#64748b]" aria-hidden="true" />
      </AvatarFallback>
    </Avatar>
  );
};
```

---

## 10. Toast Notifications (Sonner)

### 10.1 Configuracion Global

**Provider en layout:**
```tsx
import { Toaster } from 'sonner';

export default function RootLayout({ children }) {
  return (
    <html lang="es">
      <body>
        {children}
        <Toaster
          position="top-center"
          richColors
          closeButton
          duration={4000}
          toastOptions={{
            style: {
              background: '#0f1729',
              border: '1px solid #334155',
              color: '#ffffff',
            },
            className: 'font-sans',
          }}
        />
      </body>
    </html>
  );
}
```

### 10.2 Uso en Forms

```typescript
import { toast } from 'sonner';

// Success
toast.success('Cuenta creada exitosamente', {
  description: 'Redirigiendo al perfil de artista...',
});

// Error
toast.error('Error al crear cuenta', {
  description: getErrorMessage(error.errorCode),
});

// Loading (con promise)
toast.promise(
  registerMutation.mutateAsync(data),
  {
    loading: 'Creando cuenta...',
    success: 'Cuenta creada exitosamente',
    error: (err) => getErrorMessage(err.errorCode),
  }
);
```

---

## 11. Error Handling

### 11.1 Tipos de Errores

| Tipo | Origen | Manejo |
|------|--------|--------|
| **Validation (Client)** | Zod schema | Mostrar mensaje debajo del input con `role="alert"` |
| **API Error (409 Conflict)** | Backend - Email duplicado | Toast error + sugerencia "Iniciar sesión" |
| **API Error (400 Bad Request)** | Backend - Validacion | Mapear errorCode a mensaje en español |
| **API Error (401 Unauthorized)** | Backend - Token invalido | Redirect a login + toast "Sesión expirada" |
| **API Error (500 Server Error)** | Backend - Error inesperado | Toast generico "Ocurrió un error. Intenta nuevamente." |
| **Network Error** | Sin conexion | Toast "Sin conexión. Verifica tu internet." |

### 11.2 Mapeo de Error Codes

**Helper:** `getErrorMessage(errorCode)` (desde `@/shared/utils/error-messages`)

```typescript
// RegisterForm
onError: (error: ApiError) => {
  if (error.status === 409) {
    toast.error('Email ya registrado', {
      description: '¿Ya tienes cuenta?',
      action: {
        label: 'Iniciar sesión',
        onClick: () => router.push('/auth/login'),
      },
    });
  } else {
    const message = getErrorMessage(error.errorCode);
    toast.error(message);
  }
}

// CreateArtistaForm
onError: (error: ApiError) => {
  if (error.status === 401) {
    toast.error('Sesión expirada', {
      description: 'Por favor, inicia sesión nuevamente',
    });
    router.push('/auth/login');
  } else {
    const message = getErrorMessage(error.errorCode);
    toast.error(message);
  }
}
```

### 11.3 Field-Level Errors

**Mostrar errores de Zod:**

```tsx
{errors.email && (
  <p
    id="email-error"
    role="alert"
    className="text-sm text-destructive mt-1 flex items-center gap-1"
  >
    <AlertCircle className="h-4 w-4" aria-hidden="true" />
    {errors.email.message}
  </p>
)}
```

---

## 12. Loading States

### 12.1 Button Loading

```tsx
<Button disabled={isPending} aria-busy={isPending}>
  {isPending && (
    <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />
  )}
  {isPending ? "Creando cuenta..." : "Crear cuenta"}
</Button>
```

### 12.2 Form Disabled During Submit

```tsx
<Input
  {...register('email')}
  disabled={isPending}
  className={cn(
    "...",
    isPending && "cursor-not-allowed opacity-50"
  )}
/>
```

### 12.3 Image Preview Loading

```tsx
{isImageLoading ? (
  <Skeleton className="w-32 h-32 rounded-lg bg-[#1a1a2e] animate-pulse" />
) : (
  <Avatar>...</Avatar>
)}
```

---

## 13. Checklist de Implementacion

### 13.1 RegisterForm (`/auth/register`)

- [ ] Layout centrado con Card y logo en header
- [ ] Componentes shadcn/ui: Card, Input, Button, Label
- [ ] Gradient button implementado (from-pink-500 to-purple-600)
- [ ] PasswordInput con toggle (Eye icon)
- [ ] Validacion Zod integrada (registerSchema)
- [ ] React Hook Form con zodResolver
- [ ] Estados: default, focus, typing, loading, error, success
- [ ] Mensajes de error en español debajo de cada input
- [ ] Link a login funcional
- [ ] Responsive: mobile (full width, padding 4), desktop (max-w-md, padding 8)
- [ ] Focus states con ring purple (#a855f7)
- [ ] ARIA labels: htmlFor, aria-invalid, aria-describedby, role="alert"
- [ ] Loading spinner en boton (Loader2 de lucide-react)
- [ ] useMutation para POST /api/auth/register
- [ ] Toast notifications (sonner) para success/error
- [ ] Redirect a /artista/perfil/crear on success
- [ ] Error 409 (email duplicado) con link a login

### 13.2 CreateArtistaForm (`/artista/perfil/crear`)

- [ ] Header con logo y link a dashboard
- [ ] Layout con Card centrado (max-w-2xl)
- [ ] Todos los campos con Label correcto (required con asterisco)
- [ ] Textarea para descripcion (4 rows, resize-none)
- [ ] CharacterCounter component (0/2000, colores: gris/amarillo/rojo)
- [ ] Grid 2 columnas para Pais/Ciudad (desktop), stack en mobile
- [ ] ImagePreview component con Avatar
- [ ] Preview muestra icono Music si URL invalida
- [ ] Debounced validation en imagenUrl (500ms)
- [ ] Skeleton loader mientras carga preview
- [ ] Boton gradient "Guardar y continuar"
- [ ] Link "Saltar por ahora" funcional (redirect a dashboard)
- [ ] Validacion Zod integrada (createArtistaSchema)
- [ ] React Hook Form con watch() para descripcion e imagenUrl
- [ ] Estados: typing, loading, error, success
- [ ] Responsive: mobile (grid-cols-1, w-24 h-24 preview, flex-col buttons), desktop (grid-cols-2, w-32 h-32 preview, flex-row buttons)
- [ ] Toast notifications para success/error
- [ ] useMutation para POST /api/artistas
- [ ] Invalidar query cache on success
- [ ] Redirect a /dashboard on success
- [ ] ARIA labels y focus states

### 13.3 Cross-Cutting Concerns

- [ ] Dark theme consistente (bg-[#1a1a2e], cards bg-[#0f1729])
- [ ] Design tokens implementados (colores, spacing)
- [ ] Tailwind utilities en lugar de CSS custom
- [ ] Animaciones smooth (150-300ms)
- [ ] Contraste minimo 4.5:1 verificado
- [ ] Keyboard navigation funcional (Tab order logico)
- [ ] Form validation con Zod + react-hook-form
- [ ] Integracion con TanStack Query (useMutation, invalidateQueries)
- [ ] Manejo de errores del backend (mapeo de error codes)
- [ ] Toast notifications configuradas (sonner)
- [ ] Mobile-first responsive design
- [ ] TypeScript strict mode (sin any)
- [ ] Compartir schemas Zod desde @/shared/schemas
- [ ] Compartir types desde @/shared/types
- [ ] Compartir constantes desde @/shared/constants

---

## 14. Archivos a Crear

```
src/admin/src/
├── app/
│   ├── (auth)/
│   │   ├── register/
│   │   │   └── page.tsx                    # RegisterForm page
│   │   └── layout.tsx                       # Auth layout wrapper
│   └── artista/
│       └── perfil/
│           └── crear/
│               └── page.tsx                 # CreateArtistaForm page
│
├── components/
│   ├── auth/
│   │   ├── RegisterForm.tsx                 # Componente principal de registro
│   │   └── PasswordInput.tsx                # Custom input con toggle
│   ├── artista/
│   │   ├── CreateArtistaForm.tsx            # Componente principal de crear perfil
│   │   ├── CharacterCounter.tsx             # Contador de caracteres
│   │   └── ImagePreview.tsx                 # Preview de imagen con Avatar
│   └── ui/                                  # shadcn/ui components (ya existen)
│       ├── button.tsx
│       ├── input.tsx
│       ├── label.tsx
│       ├── card.tsx
│       ├── textarea.tsx
│       ├── skeleton.tsx
│       └── sonner.tsx
│
├── hooks/
│   ├── useRegister.ts                       # Hook para mutation de registro
│   └── useCreateArtista.ts                  # Hook para mutation de crear artista
│
└── lib/
    └── api/
        └── auth.ts                          # API service para auth endpoints
```

---

## 15. Dependencies

| Package | Version | Uso |
|---------|---------|-----|
| `react` | 18+ | Framework |
| `next` | 14+ | Next.js App Router |
| `react-hook-form` | 7+ | Form handling |
| `@hookform/resolvers` | 3+ | Zod resolver |
| `zod` | 3+ | Schema validation |
| `@tanstack/react-query` | 5+ | Data fetching |
| `sonner` | 1+ | Toast notifications |
| `lucide-react` | Latest | Icons (Eye, EyeOff, Loader2, Music) |
| `@radix-ui/react-avatar` | Latest | Avatar component |
| `@radix-ui/react-label` | Latest | Label component |
| `class-variance-authority` | Latest | CVA for variants |
| `tailwind-merge` | Latest | Merge Tailwind classes |

**Comando de instalacion:**
```bash
cd src/admin
npm install react-hook-form @hookform/resolvers zod @tanstack/react-query sonner lucide-react
```

---

## 16. Notas Finales

### 16.1 Prioridades de Implementacion

1. **Alta:** RegisterForm (bloquea flujo completo)
2. **Alta:** CreateArtistaForm (bloquea onboarding)
3. **Media:** PasswordInput component (mejora UX)
4. **Media:** CharacterCounter component (mejora UX)
5. **Baja:** ImagePreview component (nice-to-have, puede ser mas simple)

### 16.2 Simplificaciones Permitidas (MVP)

- **ImagePreview:** Puede empezar sin debounce, solo validar en onBlur
- **CharacterCounter:** Puede empezar sin colores dinamicos, solo mostrar numero
- **Animaciones:** Pueden simplificarse a `transition-all` basico
- **Toast custom styling:** Puede usar estilos default de sonner

### 16.3 NO Simplificar

- **Validacion Zod:** DEBE coincidir 100% con backend
- **ARIA attributes:** CRITICO para accesibilidad
- **Error handling:** DEBE mapear error codes correctamente
- **Responsive:** DEBE funcionar en mobile y desktop

### 16.4 Referencias de Codigo

- **Template Dashtail:** `references/templates/dashtail/.../login-form.tsx`
- **Componentes shadcn/ui:** `src/admin/src/components/ui/`
- **Schemas Zod:** `src/shared/schemas/`
- **Types:** `src/shared/types/`
- **Constantes:** `src/shared/constants/`

---

**Plan creado:** 2026-01-26
**Estimacion de implementacion:** 6-8 horas (ambos forms + componentes)
**Prioridad:** ALTA (bloquea flujo de registro completo)
