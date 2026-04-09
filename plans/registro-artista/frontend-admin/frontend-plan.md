# Plan Frontend: registro-artista (Admin)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/admin (Next.js 14 App Router)

---

## 1. Resumen

- **Screens:** 3 pantallas (Register, Crear Perfil, Dashboard con banner)
- **Componentes:** 2 componentes principales ya implementados + 1 nuevo (banner)
- **Hooks:** 2 hooks ya implementados (useRegister via authService, useCreateArtista)
- **Services:** 2 services ya implementados (authService, artistaService)
- **Estado:** Zustand store ya implementado (auth-store)

### Estado Actual de Implementación

| Elemento | Estado | Ubicación | Acción Requerida |
|----------|--------|-----------|------------------|
| **Página `/auth/register`** | ✅ Implementada | `src/app/(auth)/register/page.tsx` | ⚠️ Validar flujo completo |
| **Componente `RegisterForm`** | ✅ Implementado | `src/components/auth/RegisterForm.tsx` | ✅ Completo |
| **Página `/artista/perfil/crear`** | ✅ Implementada | `src/app/(dashboard)/artista/perfil/crear/page.tsx` | ⚠️ Agregar "Saltar por ahora" |
| **Componente `CreateArtistaForm`** | ✅ Implementado | `src/components/artistas/CreateArtistaForm.tsx` | ⚠️ Agregar image preview |
| **Página `/dashboard`** | ✅ Implementada | `src/app/(dashboard)/dashboard/page.tsx` | ❌ Agregar banner "Completa tu perfil" |
| **Hook `useRegister`** | ⚠️ Vía `authService.register()` | `src/services/auth.service.ts` | ✅ Funcional |
| **Hook `useCreateArtista`** | ✅ Implementado | `src/hooks/use-artista.ts` | ✅ Completo |
| **Service `authService`** | ✅ Implementado | `src/services/auth.service.ts` | ⚠️ Validar manejo de errores |
| **Service `artistaService`** | ✅ Implementado | `src/services/artista.service.ts` | ✅ Completo |
| **Store `auth-store`** | ✅ Implementado | `src/store/auth-store.ts` | ✅ Completo |

**Conclusión:** La mayoría del código ya está implementado. Solo faltan ajustes menores y un componente nuevo (banner).

---

## 2. Estructura de Carpetas

### 2.1 Estructura Actual (Next.js 14 App Router)

```
src/admin/src/
├── app/
│   ├── (auth)/
│   │   ├── layout.tsx                      ✅ Layout para páginas públicas
│   │   ├── login/
│   │   │   └── page.tsx                    ✅ Página de login
│   │   └── register/
│   │       └── page.tsx                    ✅ Página de registro (EXISTE)
│   │
│   ├── (dashboard)/
│   │   ├── layout.tsx                      ✅ Layout protegido con auth
│   │   ├── dashboard/
│   │   │   └── page.tsx                    ✅ Dashboard principal (EXISTE, falta banner)
│   │   ├── artista/
│   │   │   └── perfil/
│   │   │       └── crear/
│   │   │           └── page.tsx            ✅ Crear perfil artista (EXISTE)
│   │   ├── campanias/
│   │   │   ├── page.tsx                    ✅ Listado de campañas
│   │   │   └── nueva/
│   │   │       └── page.tsx                ✅ Nueva campaña
│   │   └── perfil/
│   │       └── page.tsx                    ✅ Editar perfil (futuro)
│   │
│   ├── layout.tsx                          ✅ Root layout
│   └── page.tsx                            ✅ Redirect a /dashboard o /login
│
├── components/
│   ├── ui/                                 ✅ shadcn/ui components
│   │   ├── button.tsx
│   │   ├── card.tsx
│   │   ├── input.tsx
│   │   ├── label.tsx
│   │   ├── textarea.tsx
│   │   ├── badge.tsx
│   │   ├── skeleton.tsx
│   │   ├── sonner.tsx                      ✅ Toast notifications
│   │   └── ...
│   │
│   ├── layout/
│   │   ├── sidebar.tsx                     ✅ Sidebar navigation
│   │   └── header.tsx                      ✅ Top header
│   │
│   ├── auth/
│   │   └── RegisterForm.tsx                ✅ Form de registro (EXISTE)
│   │
│   ├── artistas/
│   │   ├── CreateArtistaForm.tsx           ✅ Form crear perfil (EXISTE)
│   │   └── artista-form.tsx                ✅ Form editar perfil (ya existe)
│   │
│   ├── dashboard/
│   │   ├── stats-card.tsx                  ✅ Card de estadísticas
│   │   ├── recent-backings.tsx             ✅ Recent backings widget
│   │   └── complete-profile-banner.tsx     ❌ CREAR (nuevo)
│   │
│   └── campanias/
│       └── campania-form.tsx               ✅ Form campañas
│
├── hooks/
│   ├── use-artista.ts                      ✅ useMyArtistProfile, useCreateArtista (EXISTE)
│   └── use-campanias.ts                    ✅ Hooks campañas
│
├── services/
│   ├── auth.service.ts                     ✅ Login, register, getCurrentUser (EXISTE)
│   ├── artista.service.ts                  ✅ CRUD artista (EXISTE)
│   └── campania.service.ts                 ✅ CRUD campañas
│
├── store/
│   ├── auth-store.ts                       ✅ Zustand auth state (EXISTE)
│   └── sidebar-store.ts                    ✅ Sidebar collapsed state
│
├── lib/
│   ├── api-client.ts                       ✅ Axios client con interceptors (EXISTE)
│   └── utils.ts                            ✅ cn() para Tailwind
│
└── providers/
    └── index.tsx                           ✅ QueryClientProvider wrapper
```

### 2.2 Archivos Nuevos a Crear

Solo **1 archivo nuevo**:

```
src/admin/src/components/dashboard/complete-profile-banner.tsx
```

### 2.3 Archivos a Modificar

| Archivo | Modificación Requerida |
|---------|------------------------|
| `src/app/(dashboard)/dashboard/page.tsx` | Agregar `CompleteProfileBanner` cuando artista es null |
| `src/app/(dashboard)/artista/perfil/crear/page.tsx` | Agregar botón "Saltar por ahora" (opcional) |
| `src/components/artistas/CreateArtistaForm.tsx` | Agregar image preview con debounced validation |
| `src/services/auth.service.ts` | Mejorar manejo de errores (mapear errorCode) |

---

## 3. Componentes

### 3.1 RegisterForm (YA EXISTE) ✅

**Archivo:** `src/components/auth/RegisterForm.tsx`

**Estado:** ✅ Implementado completamente

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| onSubmit | `(data: RegisterFormData) => Promise<void>` | Sí | Callback al enviar formulario |
| isSubmitting | `boolean` | No | Estado de carga |
| serverError | `string` | No | Error del servidor a mostrar |

**Estado Local:**
- React Hook Form state (errors, register, handleSubmit)

**Dependencias:**
- Hooks: `useForm` (react-hook-form)
- Schemas: `registerSchema` de `@shared/schemas`
- Componentes UI: `Input`, `Label`, `Button` (shadcn/ui)

**Responsabilidad:**
- Renderizar formulario de registro con email, password, confirmPassword
- Validación en tiempo real con Zod
- Mostrar errores de validación y del servidor
- Deshabilitar submit durante carga

**Validaciones:**
- Email: formato válido, requerido
- Password: mínimo 8 caracteres, requerido
- ConfirmPassword: coincide con password, requerido

**Estados UI:**
- Default: campos vacíos, validación onBlur
- Typing: validación en tiempo real
- Error: border rojo + mensaje debajo del input
- Submitting: botón disabled con texto "Registrando..."
- Server Error: banner rojo arriba del botón submit

**Mejoras Sugeridas (Opcionales):**
- [ ] Agregar toggle para mostrar/ocultar password (eye icon)
- [ ] Agregar indicador de fuerza de contraseña

---

### 3.2 CreateArtistaForm (YA EXISTE) ⚠️

**Archivo:** `src/components/artistas/CreateArtistaForm.tsx`

**Estado:** ⚠️ Implementado, falta agregar image preview

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| onSubmit | `(data: CreateArtistaFormData) => Promise<void>` | Sí | Callback al enviar formulario |
| isSubmitting | `boolean` | No | Estado de carga |

**Estado Local:**
- React Hook Form state
- `descripcionLength` (number) - contador de caracteres para descripción

**Dependencias:**
- Hooks: `useForm`, `useState`
- Schemas: `createArtistaSchema` de `@shared/schemas`
- Componentes UI: `Input`, `Label`, `Textarea`, `Button`

**Responsabilidad:**
- Renderizar formulario con: nombreArtistico*, descripcion, pais, ciudad, imagenUrl
- Validación Zod en tiempo real
- Contador de caracteres para descripción (0/2000)
- Grid 2 columnas (país/ciudad) en desktop, stack en mobile

**Campos:**
| Campo | Tipo | Requerido | Max Length | Placeholder |
|-------|------|-----------|-----------|-------------|
| nombreArtistico | text | Sí | 200 | "Tu nombre de artista" |
| descripcion | textarea | No | 2000 | "Cuéntanos sobre ti y tu música..." |
| pais | text | No | 100 | "Ej: España" |
| ciudad | text | No | 100 | "Ej: Madrid" |
| imagenUrl | url | No | - | "https://ejemplo.com/imagen.jpg" |

**ACCIÓN REQUERIDA:** Agregar Image Preview

**Implementación Sugerida:**

```tsx
// Agregar state para preview
const [imagePreview, setImagePreview] = useState<string | null>(null)
const [imageLoading, setImageLoading] = useState(false)
const [imageError, setImageError] = useState(false)

// Watch imagenUrl field
const imagenUrl = watch("imagenUrl", "")

// Debounced validation + preview load
useEffect(() => {
  if (!imagenUrl || imagenUrl === "") {
    setImagePreview(null)
    setImageError(false)
    return
  }

  // Validar formato URL
  try {
    new URL(imagenUrl)
  } catch {
    setImagePreview(null)
    setImageError(true)
    return
  }

  // Debounce 500ms antes de cargar imagen
  const timer = setTimeout(() => {
    setImageLoading(true)
    setImageError(false)

    const img = new Image()
    img.onload = () => {
      setImagePreview(imagenUrl)
      setImageLoading(false)
    }
    img.onerror = () => {
      setImageError(true)
      setImageLoading(false)
    }
    img.src = imagenUrl
  }, 500)

  return () => clearTimeout(timer)
}, [imagenUrl])

// Agregar después del input imagenUrl:
{imagenUrl && (
  <div className="mt-3">
    {imageLoading && (
      <div className="w-32 h-32 bg-gray-800 rounded-lg animate-pulse" />
    )}
    {imageError && !imageLoading && (
      <div className="flex items-center gap-2 text-yellow-500 text-sm">
        <Music className="h-4 w-4" />
        <span>No se pudo cargar la imagen</span>
      </div>
    )}
    {imagePreview && !imageLoading && (
      <div>
        <p className="text-xs text-gray-400 mb-2">Vista previa:</p>
        <img
          src={imagePreview}
          alt="Preview"
          className="w-32 h-32 rounded-lg object-cover border-2 border-gray-700"
        />
      </div>
    )}
  </div>
)}
```

---

### 3.3 CompleteProfileBanner (NUEVO) ❌

**Archivo:** `src/components/dashboard/complete-profile-banner.tsx`

**Estado:** ❌ No existe, debe crearse

**Props:**
Ninguno (consume `useMyArtistProfile()` internamente)

**Responsabilidad:**
- Mostrar banner persistente en `/dashboard` si el usuario autenticado NO tiene perfil de artista
- Link a `/artista/perfil/crear`
- Botón de cierre temporal (localStorage)

**Implementación Sugerida:**

```tsx
"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Music, X } from "lucide-react"
import { useMyArtistProfile } from "@/hooks/use-artista"

export function CompleteProfileBanner() {
  const { data: artista, isLoading } = useMyArtistProfile()
  const [dismissed, setDismissed] = useState(false)

  useEffect(() => {
    // Check if banner was dismissed in localStorage
    const isDismissed = localStorage.getItem("profile-banner-dismissed") === "true"
    setDismissed(isDismissed)
  }, [])

  const handleDismiss = () => {
    localStorage.setItem("profile-banner-dismissed", "true")
    setDismissed(true)
  }

  // No mostrar si:
  // - Está cargando
  // - Ya tiene perfil de artista
  // - El usuario cerró el banner
  if (isLoading || artista || dismissed) {
    return null
  }

  return (
    <Alert className="mb-6 bg-gradient-to-r from-pink-500/10 to-purple-600/10 border-pink-500/50">
      <Music className="h-5 w-5 text-pink-500" />
      <AlertTitle className="text-white">Completa tu perfil de artista</AlertTitle>
      <AlertDescription className="text-gray-300 mt-2">
        Para crear campañas de crowdfunding, necesitas completar tu perfil artístico.
      </AlertDescription>
      <div className="flex items-center gap-3 mt-4">
        <Link href="/artista/perfil/crear">
          <Button size="sm" className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
            Completar perfil
          </Button>
        </Link>
        <Button
          size="sm"
          variant="ghost"
          onClick={handleDismiss}
          className="text-gray-400 hover:text-white"
        >
          <X className="h-4 w-4" />
          Cerrar
        </Button>
      </div>
    </Alert>
  )
}
```

**Uso:**

```tsx
// En src/app/(dashboard)/dashboard/page.tsx

import { CompleteProfileBanner } from "@/components/dashboard/complete-profile-banner"

export default function DashboardPage() {
  // ...
  return (
    <div className="space-y-6">
      {/* Agregar banner ANTES del header */}
      <CompleteProfileBanner />

      {/* Header */}
      <div className="flex items-center justify-between">
        {/* ... */}
      </div>
    </div>
  )
}
```

**Dependencias:**
- Hooks: `useMyArtistProfile` (ya existe)
- Componentes UI: `Alert`, `AlertDescription`, `AlertTitle` (shadcn/ui - verificar si existe, o usar Card)
- Icons: `Music`, `X` (lucide-react)

**NOTA:** Si `Alert` component no existe en shadcn/ui, usar `Card` en su lugar:

```tsx
import { Card, CardContent } from "@/components/ui/card"

// Reemplazar <Alert> con <Card className="...">
```

---

## 4. Hooks

### 4.1 useRegister (Vía authService) ✅

**Archivo:** `src/services/auth.service.ts` (método `register()`)

**Estado:** ✅ Implementado

**Tipo:** Mutation (no usa useMutation de React Query, llamada directa)

**Uso en Componente:**

```tsx
// En src/app/(auth)/register/page.tsx
const handleSubmit = async (data: RegisterFormData) => {
  try {
    const response = await authService.register(data)
    login(response.user, response.token) // Zustand store
    toast.success("Cuenta creada exitosamente!")
    router.push("/artista/perfil/crear")
  } catch (error) {
    toast.error(error.message)
  }
}
```

**Flujo:**
1. POST `/api/auth/register` con `{ email, password, confirmPassword }`
2. Backend retorna `{ data: { token }, messages: [] }` (ServiceResponse)
3. Service guarda token en localStorage
4. Service llama a `getCurrentUser()` para obtener User
5. Retorna `{ user, token }`
6. Page llama a `login(user, token)` del auth-store
7. Redirect a `/artista/perfil/crear`

**Manejo de Errores:**

**ACCIÓN REQUERIDA:** Mejorar mapeo de errores del backend

```typescript
// src/services/auth.service.ts (MODIFICAR método register)

async register(data: RegisterRequest): Promise<AuthResponse> {
  try {
    const response = await apiFetch<ServiceResponse<{ token: string }>>("/auth/register", {
      method: "POST",
      data,
    })

    if (!response.data.token) {
      throw new Error("Token no recibido del servidor")
    }

    // Store token
    if (typeof window !== "undefined") {
      localStorage.setItem("token", response.data.token)
    }

    // Get user info
    const user = await this.getCurrentUser()
    if (!user) throw new Error("Error al obtener información del usuario")

    return { user, token: response.data.token }

  } catch (error: any) {
    // Mapear errorCode del backend a mensaje user-friendly
    if (error.response?.data?.messages?.[0]?.errorCode) {
      const errorCode = error.response.data.messages[0].errorCode
      const errorMessage = getErrorMessage(errorCode) // De @shared/utils
      throw new Error(errorMessage)
    }
    throw new Error("Error al crear cuenta. Intenta nuevamente.")
  }
}
```

**NOTA:** Importar `getErrorMessage` de `@shared/utils`

---

### 4.2 useCreateArtista ✅

**Archivo:** `src/hooks/use-artista.ts`

**Estado:** ✅ Implementado completamente

**Tipo:** Mutation Hook (useMutation de React Query)

**Parámetros:**
Ninguno (usa mutation sin parámetros)

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| mutate | `(data: CreateArtistaDto) => void` | Ejecutar mutación (fire and forget) |
| mutateAsync | `(data: CreateArtistaDto) => Promise<ArtistaDto>` | Ejecutar mutación con await |
| isPending | `boolean` | Estado de carga |
| isError | `boolean` | Hay error |
| isSuccess | `boolean` | Mutación exitosa |
| error | `Error` | Error si hay |
| data | `ArtistaDto` | Datos retornados tras éxito |

**Query Key Invalidation:**
- Invalida: `QUERY_KEYS.ARTISTA_ME` (para refetch del perfil)

**Uso en Componente:**

```tsx
// src/app/(dashboard)/artista/perfil/crear/page.tsx (YA EXISTE)
const createArtistaMutation = useCreateArtista()

const handleSubmit = async (data: CreateArtistaFormData) => {
  try {
    await createArtistaMutation.mutateAsync({
      nombreArtistico: data.nombreArtistico,
      descripcion: data.descripcion || undefined,
      pais: data.pais || undefined,
      ciudad: data.ciudad || undefined,
      imagenUrl: data.imagenUrl || undefined,
    })
    toast.success("Perfil de artista creado exitosamente!")
    router.push("/dashboard")
  } catch (error) {
    toast.error("Error al crear el perfil. Intenta nuevamente.")
  }
}
```

**Mejora Sugerida:** Mapear errorCode del backend

```tsx
// En el catch block
catch (error: any) {
  const errorCode = error.response?.data?.messages?.[0]?.errorCode
  const errorMessage = getErrorMessage(errorCode) // De @shared/utils
  toast.error(errorMessage)
}
```

---

### 4.3 useMyArtistProfile ✅

**Archivo:** `src/hooks/use-artista.ts`

**Estado:** ✅ Implementado completamente

**Tipo:** Query Hook (useQuery de React Query)

**Parámetros:**
Ninguno

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | `ArtistaDto \| null` | Perfil del artista o null si no existe |
| isLoading | `boolean` | Estado de carga |
| isError | `boolean` | Hay error |
| error | `Error` | Error si hay |

**Query Key:**
`[QUERY_KEYS.ARTISTA_ME]` (definido en `@shared/constants`)

**Uso:**

```tsx
// En src/app/(dashboard)/dashboard/page.tsx
const { data: artista, isLoading } = useMyArtistProfile()

// Mostrar banner si artista es null
{!artista && !isLoading && <CompleteProfileBanner />}
```

**Nota:** El service `artistaService.getMyProfile()` retorna `null` si el usuario no tiene perfil (status 404 o error).

---

## 5. Services

### 5.1 authService ✅

**Archivo:** `src/services/auth.service.ts`

**Estado:** ✅ Implementado, requiere mejoras en manejo de errores

**Métodos:**

| Método | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `login(credentials)` | `LoginCredentials` | `AuthResponse` | POST `/api/auth/login` |
| `register(data)` | `RegisterRequest` | `AuthResponse` | POST `/api/auth/register` |
| `getCurrentUser()` | - | `User \| null` | GET `/api/auth/me` |

**AuthResponse:**
```typescript
{
  user: User
  token: string
}
```

**Flujo de `register()`:**
1. POST `/api/auth/register`
2. Recibe `{ data: { token }, messages: [] }` (ServiceResponse)
3. Guarda token en localStorage
4. Llama a `getCurrentUser()` para obtener User
5. Retorna `{ user, token }`

**ACCIÓN REQUERIDA:** Ver sección 4.1 para mejoras en manejo de errores.

---

### 5.2 artistaService ✅

**Archivo:** `src/services/artista.service.ts`

**Estado:** ✅ Implementado completamente

**Métodos:**

| Método | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getMyProfile()` | - | `ArtistaDto \| null` | GET `/api/artistas/me` |
| `create(data)` | `CreateArtistaDto` | `ArtistaDto` | POST `/api/artistas` |
| `update(id, data)` | `id: string, data: UpdateArtistaDto` | `ArtistaDto` | PUT `/api/artistas/{id}` |

**Nota:** Endpoint `/api/artistas/me` no está definido en `contracts.md`. Según contracts.md, debería ser:
- `GET /api/artistas/by-user/{userId}` (requiere UserId del token)

**ACCIÓN REQUERIDA (Opción A - Modificar Backend):**
Agregar endpoint `/api/artistas/me` en backend que extrae UserId del token JWT automáticamente.

**ACCIÓN REQUERIDA (Opción B - Modificar Service):**

```typescript
// src/services/artista.service.ts (MODIFICAR getMyProfile)

import { useAuthStore } from "@/store/auth-store"

async getMyProfile(): Promise<ArtistaDto | null> {
  try {
    // Obtener userId del store
    const userId = useAuthStore.getState().user?.id
    if (!userId) return null

    const response = await apiFetch<ServiceResponse<ArtistaDto>>(
      `/artistas/by-user/${userId}`
    )
    return response.data
  } catch {
    return null
  }
}
```

**Recomendación:** Opción A (modificar backend) es más limpia y segura.

---

## 6. Flujo de Datos

### 6.1 Flujo de Registro

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. User accede a /auth/register                                │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. RegisterForm renderiza con validación Zod                   │
│    - Email: required, email format                             │
│    - Password: required, min 8 chars                           │
│    - ConfirmPassword: required, equals password                │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. User llena formulario y hace submit                         │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. Page llama authService.register(data)                       │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 5. Service POST /api/auth/register                             │
│    Backend valida y crea usuario en Identity                   │
│    Backend retorna { data: { token }, messages: [...] }        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 6. Service guarda token en localStorage                        │
│    Service llama getCurrentUser() → GET /api/auth/me           │
│    Service retorna { user, token }                             │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 7. Page llama login(user, token) del auth-store                │
│    Store persiste en localStorage + state                      │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 8. Page muestra toast success                                  │
│    router.push("/artista/perfil/crear")                        │
└─────────────────────────────────────────────────────────────────┘
```

**Errores Posibles:**
- 400: Email inválido, password corto, passwords no coinciden → Mostrar error en formulario
- 409: Email ya existe → Mostrar mensaje "Email ya registrado. ¿Iniciar sesión?"
- 500: Error inesperado → Mostrar mensaje genérico

---

### 6.2 Flujo de Crear Perfil de Artista

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. User autenticado accede a /artista/perfil/crear             │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. Page verifica auth (layout (dashboard) hace redirect si no  │
│    autenticado)                                                 │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. CreateArtistaForm renderiza con validación Zod              │
│    - nombreArtistico: required, max 200                        │
│    - descripcion: optional, max 2000                           │
│    - pais, ciudad: optional, max 100                           │
│    - imagenUrl: optional, URL valid                            │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. User llena formulario                                       │
│    - Contador caracteres en descripción                        │
│    - Image preview con debounce 500ms (si URL válida)          │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 5. User hace submit                                            │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 6. Page llama createArtistaMutation.mutateAsync(data)          │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 7. Hook llama artistaService.create(data)                      │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 8. Service POST /api/artistas                                  │
│    Headers: Authorization: Bearer {token}                      │
│    Backend extrae UserId del token JWT                         │
│    Backend crea Artista vinculada a UserId                     │
│    Backend retorna { data: ArtistaDto, messages: [...] }       │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 9. Hook invalida queryKey ARTISTA_ME                           │
│    Page muestra toast success                                  │
│    router.push("/dashboard")                                   │
└─────────────────────────────────────────────────────────────────┘
```

**Errores Posibles:**
- 400: Validación falló (nombre vacío, descripción larga, URL inválida) → Mostrar error del backend
- 401: Token expirado → Redirect a /login (axios interceptor)
- 409: Usuario ya tiene perfil de artista → Mostrar mensaje
- 500: Error inesperado → Mostrar mensaje genérico

---

### 6.3 Flujo de Dashboard con Banner

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. User autenticado accede a /dashboard                        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. Page llama useMyArtistProfile()                             │
│    Hook hace GET /api/artistas/me (o /by-user/{userId})        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. Si artista === null && !isLoading:                          │
│    - Renderizar <CompleteProfileBanner />                      │
│    - Banner muestra mensaje "Completa tu perfil"               │
│    - Link a /artista/perfil/crear                              │
│    - Botón "Cerrar" guarda en localStorage (dismiss temporal)  │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. Si artista !== null:                                        │
│    - No mostrar banner                                         │
│    - Mostrar dashboard normal con stats, campañas, etc.        │
└─────────────────────────────────────────────────────────────────┘
```

**Nota:** Banner se cierra temporalmente con localStorage pero NO es permanente. Se vuelve a mostrar si el usuario refresca la página SIN completar el perfil.

---

## 7. Rutas y Protección

### 7.1 Rutas Públicas (Auth Layout)

| Ruta | Componente | Descripción |
|------|-----------|-------------|
| `/auth/login` | `app/(auth)/login/page.tsx` | Login existente |
| `/auth/register` | `app/(auth)/register/page.tsx` | Registro de usuario ✅ |

**Protección:** Ninguna (rutas públicas)

**Layout:** `app/(auth)/layout.tsx`
- Centrado en pantalla
- Fondo oscuro
- No sidebar, no header

---

### 7.2 Rutas Protegidas (Dashboard Layout)

| Ruta | Componente | Auth | Perfil Artista | Descripción |
|------|-----------|------|----------------|-------------|
| `/dashboard` | `app/(dashboard)/dashboard/page.tsx` | ✅ Requerido | ⚠️ Opcional (banner si falta) | Dashboard principal |
| `/artista/perfil/crear` | `app/(dashboard)/artista/perfil/crear/page.tsx` | ✅ Requerido | ❌ No debe tener | Crear perfil |
| `/campanias` | `app/(dashboard)/campanias/page.tsx` | ✅ Requerido | ✅ Requerido | Listado campañas |
| `/campanias/nueva` | `app/(dashboard)/campanias/nueva/page.tsx` | ✅ Requerido | ✅ Requerido | Crear campaña |
| `/perfil` | `app/(dashboard)/perfil/page.tsx` | ✅ Requerido | ✅ Requerido | Editar perfil |

**Protección de Auth:** Implementada en `app/(dashboard)/layout.tsx`

```tsx
// src/app/(dashboard)/layout.tsx (YA EXISTE)

useEffect(() => {
  if (!isAuthenticated) {
    router.push("/login")
  }
}, [isAuthenticated, router])

if (!isAuthenticated) {
  return null
}
```

**ACCIÓN SUGERIDA:** Agregar protección adicional para rutas que requieren perfil artista

```tsx
// src/app/(dashboard)/campanias/nueva/page.tsx (MODIFICAR)

export default function NuevaCampaniaPage() {
  const router = useRouter()
  const { data: artista, isLoading } = useMyArtistProfile()

  useEffect(() => {
    if (!isLoading && !artista) {
      toast.error("Necesitas completar tu perfil de artista primero")
      router.push("/artista/perfil/crear")
    }
  }, [isLoading, artista, router])

  if (isLoading) return <Spinner />
  if (!artista) return null

  // ... resto del componente
}
```

**Aplicar mismo patrón en:**
- `/campanias/nueva/page.tsx`
- `/perfil/page.tsx`
- `/campanias/page.tsx` (opcional)

---

### 7.3 Middleware (Next.js 14)

**Estado:** No implementado actualmente

**ACCIÓN OPCIONAL:** Crear middleware para proteger rutas a nivel de Next.js

```typescript
// src/middleware.ts (CREAR)

import { NextResponse } from "next/server"
import type { NextRequest } from "next/server"

export function middleware(request: NextRequest) {
  const token = request.cookies.get("auth-token")?.value

  // Rutas protegidas
  if (request.nextUrl.pathname.startsWith("/dashboard") ||
      request.nextUrl.pathname.startsWith("/campanias") ||
      request.nextUrl.pathname.startsWith("/artista")) {

    if (!token) {
      return NextResponse.redirect(new URL("/login", request.url))
    }
  }

  // Rutas públicas (login, register) redirigir si ya autenticado
  if (request.nextUrl.pathname.startsWith("/login") ||
      request.nextUrl.pathname.startsWith("/register")) {

    if (token) {
      return NextResponse.redirect(new URL("/dashboard", request.url))
    }
  }

  return NextResponse.next()
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"],
}
```

**NOTA:** Actualmente el token se guarda en localStorage, no en cookies. Para usar middleware, necesitarías cambiar a cookies.

---

## 8. Gestión de JWT

### 8.1 Almacenamiento

**Método Actual:** localStorage

**Ubicaciones:**
1. `src/lib/api-client.ts` - Interceptor lee token de localStorage
2. `src/store/auth-store.ts` - Store guarda/elimina token de localStorage
3. `src/services/auth.service.ts` - Service guarda token tras registro

**Código:**

```typescript
// Guardar token
localStorage.setItem("token", token)

// Leer token
const token = localStorage.getItem("token")

// Eliminar token
localStorage.removeItem("token")
```

**Pros:**
- Simple de implementar
- No requiere configuración adicional
- Funciona en client-side

**Contras:**
- Vulnerable a XSS attacks
- No funciona con Next.js middleware (server-side)
- No se envía automáticamente en requests

**ALTERNATIVA (Opcional):** Cambiar a httpOnly cookies

**Requeriría:**
1. Backend configura cookie httpOnly al retornar token
2. Frontend NO accede directamente al token (solo backend lo lee)
3. Axios envía cookies automáticamente con requests
4. Middleware de Next.js puede leer cookies

**Decisión:** Mantener localStorage para MVP por simplicidad.

---

### 8.2 Interceptor de Axios

**Archivo:** `src/lib/api-client.ts`

**Estado:** ✅ Implementado completamente

**Request Interceptor:**

```typescript
apiClient.interceptors.request.use(
  (config) => {
    if (typeof window !== "undefined") {
      const token = localStorage.getItem("token")
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
    }
    return config
  },
  (error) => Promise.reject(error)
)
```

**Funcionalidad:**
- Lee token de localStorage
- Agrega header `Authorization: Bearer {token}` a todas las requests
- Solo en client-side (typeof window !== "undefined")

**Response Interceptor:**

```typescript
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      if (typeof window !== "undefined") {
        localStorage.removeItem("token")
        window.location.href = "/login"
      }
    }
    return Promise.reject(error)
  }
)
```

**Funcionalidad:**
- Si respuesta es 401 Unauthorized:
  - Eliminar token de localStorage
  - Redirect a /login
  - Limpiar state de auth-store (opcional)

**ACCIÓN SUGERIDA:** Agregar limpieza de Zustand store

```typescript
// src/lib/api-client.ts (MODIFICAR response interceptor)

import { useAuthStore } from "@/store/auth-store"

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      if (typeof window !== "undefined") {
        // Limpiar token
        localStorage.removeItem("token")

        // Limpiar Zustand store
        useAuthStore.getState().logout()

        // Redirect
        window.location.href = "/login"
      }
    }
    return Promise.reject(error)
  }
)
```

---

### 8.3 JWT Claims Esperados

**Según contracts.md:**

```json
{
  "sub": "userId (GUID)",
  "email": "usuario@example.com",
  "exp": 1738000000,
  "iat": 1737913600
}
```

**Uso en Frontend:**
- Frontend NO decodifica el JWT (no es necesario)
- Frontend solo envía el token en headers
- Backend valida y extrae claims del token
- Frontend obtiene User data via `/api/auth/me`

**Expiración:**
- Default: 24 horas (según contracts.md)
- Cuando expira: Backend retorna 401
- Frontend: Interceptor hace logout + redirect a /login

---

## 9. Integración con Shared Types/Schemas

### 9.1 Types Importados

**Desde:** `@shared/types`

| Tipo | Uso en Admin | Archivo |
|------|--------------|---------|
| `RegisterRequest` | authService.register() | auth.service.ts |
| `RegisterResponse` | authService.register() return | auth.service.ts |
| `User` | auth-store, getCurrentUser() | auth-store.ts, auth.service.ts |
| `LoginCredentials` | authService.login() | auth.service.ts |
| `AuthResponse` | authService return type | auth.service.ts |
| `CreateArtistaDto` | artistaService.create(), useCreateArtista | artista.service.ts, use-artista.ts |
| `ArtistaDto` | artistaService return, useMyArtistProfile | artista.service.ts, use-artista.ts |
| `UpdateArtistaDto` | artistaService.update() | artista.service.ts |
| `ServiceResponse<T>` | apiFetch generic return | api-client.ts |

**Imports:**

```typescript
import type {
  RegisterRequest,
  RegisterResponse,
  User,
  AuthResponse,
  CreateArtistaDto,
  ArtistaDto,
  ServiceResponse,
} from "@shared/types"
```

**NOTA:** Path alias `@shared` debe estar configurado en `tsconfig.json`:

```json
{
  "compilerOptions": {
    "paths": {
      "@/*": ["./src/*"],
      "@shared/*": ["../shared/*"]
    }
  }
}
```

---

### 9.2 Schemas Zod Importados

**Desde:** `@shared/schemas`

| Schema | Tipo Inferido | Uso en Admin | Archivo |
|--------|--------------|--------------|---------|
| `registerSchema` | `RegisterFormData` | RegisterForm validation | RegisterForm.tsx |
| `createArtistaSchema` | `CreateArtistaFormData` | CreateArtistaForm validation | CreateArtistaForm.tsx |
| `loginSchema` | `LoginFormData` | LoginForm validation | login/page.tsx |

**Imports:**

```typescript
import { registerSchema, type RegisterFormData } from "@shared/schemas"
import { createArtistaSchema, type CreateArtistaFormData } from "@shared/schemas"
import { loginSchema, type LoginFormData } from "@shared/schemas"
```

**Uso con React Hook Form:**

```tsx
const form = useForm<RegisterFormData>({
  resolver: zodResolver(registerSchema),
})
```

**Validación Automática:**
- onBlur: Validar campo al salir
- onChange: Validar en tiempo real (opcional)
- onSubmit: Validar todo el formulario antes de enviar

---

### 9.3 Constantes Importadas

**Desde:** `@shared/constants`

| Constante | Uso en Admin | Archivo |
|-----------|--------------|---------|
| `API_ROUTES.auth.register` | authService.register() | auth.service.ts |
| `API_ROUTES.auth.login` | authService.login() | auth.service.ts |
| `API_ROUTES.artistas.base` | artistaService.create() | artista.service.ts |
| `QUERY_KEYS.ARTISTA_ME` | useMyArtistProfile queryKey | use-artista.ts |
| `APP_ROUTES.auth.register` | Link en login page | login/page.tsx |
| `APP_ROUTES.artista.crearPerfil` | Redirect tras registro | register/page.tsx |
| `APP_ROUTES.dashboard` | Redirect tras crear perfil | artista/perfil/crear/page.tsx |

**Imports:**

```typescript
import { API_ROUTES, QUERY_KEYS, APP_ROUTES } from "@shared/constants"
```

**NOTA:** Verificar que QUERY_KEYS.ARTISTA_ME esté definido en `@shared/constants/index.ts`

**Si NO existe:**

```typescript
// src/shared/constants/index.ts (AGREGAR)

export const QUERY_KEYS = {
  // ... existing keys
  ARTISTA_ME: "artista-me", // O ["artista", "me"] as const
} as const
```

---

### 9.4 Utils Importados

**Desde:** `@shared/utils`

| Función | Uso en Admin | Archivo |
|---------|--------------|---------|
| `getErrorMessage(errorCode)` | Mapear errorCode a mensaje UI | auth.service.ts, artista service |
| `formatCurrency(amount)` | Formatear montos en dashboard | dashboard/page.tsx |
| `calculatePercentage(current, goal)` | Calcular % de campaña | dashboard/page.tsx |

**Imports:**

```typescript
import { getErrorMessage, formatCurrency, calculatePercentage } from "@shared/utils"
```

**Uso de `getErrorMessage`:**

```typescript
// En catch blocks de services/pages
catch (error: any) {
  const errorCode = error.response?.data?.messages?.[0]?.errorCode
  const errorMessage = getErrorMessage(errorCode)
  toast.error(errorMessage)
}
```

---

## 10. Estados de Loading/Error/Success

### 10.1 Estados de RegisterForm

| Estado | Trigger | UI Behavior |
|--------|---------|-------------|
| **Default** | Carga inicial | Formulario vacío, campos enabled, botón enabled |
| **Typing** | onChange | Validación en tiempo real si hay errores previos |
| **Validation Error** | onBlur / onSubmit | Border rojo en input, mensaje error debajo (text-destructive) |
| **Submitting** | onClick submit | Botón disabled, texto "Registrando...", inputs disabled |
| **Server Error** | catch block | Banner rojo arriba del botón con mensaje del servidor |
| **Success** | register success | Toast success "Cuenta creada", redirect a /artista/perfil/crear |

**Código:**

```tsx
// RegisterForm states
const { formState: { errors, isSubmitting } } = useForm()

// Page states
const [isSubmitting, setIsSubmitting] = useState(false)
const [serverError, setServerError] = useState<string>()

// Loading state
{isSubmitting ? "Registrando..." : "Crear cuenta"}

// Error display
{errors.email && <p className="text-sm text-destructive">{errors.email.message}</p>}
{serverError && <div className="bg-destructive/10 border-destructive">{serverError}</div>}

// Success
toast.success("Cuenta creada exitosamente!")
router.push("/artista/perfil/crear")
```

---

### 10.2 Estados de CreateArtistaForm

| Estado | Trigger | UI Behavior |
|--------|---------|-------------|
| **Default** | Carga inicial | Formulario vacío, solo nombreArtistico required |
| **Typing Descripción** | onChange | Contador caracteres actualizado (X/2000), color cambia según proximidad a límite |
| **Typing Imagen URL** | onChange | Debounce 500ms → validar URL → cargar preview |
| **Image Loading** | URL válida | Skeleton/spinner en área de preview |
| **Image Error** | URL inválida o no carga | Icono placeholder con mensaje "No se pudo cargar imagen" |
| **Image Success** | Imagen cargada | Preview de imagen (w-32 h-32 rounded-lg) |
| **Submitting** | onClick submit | Botón disabled, texto "Creando perfil...", inputs disabled |
| **Validation Error** | onBlur / onSubmit | Border rojo, mensaje error debajo |
| **Success** | create success | Toast success "Perfil creado", redirect a /dashboard |

**Código:**

```tsx
// Descripción character counter
const descripcion = watch("descripcion", "")
const descripcionLength = descripcion?.length || 0

<span className={cn(
  "text-xs",
  descripcionLength > 1800 ? "text-yellow-500" : "text-gray-400",
  descripcionLength >= 2000 && "text-red-500"
)}>
  {descripcionLength} / 2000
</span>

// Image preview states
const [imagePreview, setImagePreview] = useState<string | null>(null)
const [imageLoading, setImageLoading] = useState(false)
const [imageError, setImageError] = useState(false)

// Success
toast.success("Perfil de artista creado exitosamente!")
router.push("/dashboard")
```

---

### 10.3 Estados de CompleteProfileBanner

| Estado | Trigger | UI Behavior |
|--------|---------|-------------|
| **Loading** | useMyArtistProfile isLoading | No mostrar banner (esperar) |
| **Has Profile** | artista !== null | No mostrar banner |
| **No Profile** | artista === null && !isLoading | Mostrar banner con gradient background |
| **Dismissed** | Click "Cerrar" | localStorage = true, banner se oculta |

**Código:**

```tsx
const { data: artista, isLoading } = useMyArtistProfile()
const [dismissed, setDismissed] = useState(false)

useEffect(() => {
  const isDismissed = localStorage.getItem("profile-banner-dismissed") === "true"
  setDismissed(isDismissed)
}, [])

if (isLoading || artista || dismissed) {
  return null
}

return <Alert>...</Alert>
```

---

### 10.4 Toasts (Sonner)

**Librería:** `sonner` (ya instalada)

**Componente:** `src/components/ui/sonner.tsx`

**Uso:**

```typescript
import { toast } from "sonner"

// Success
toast.success("Cuenta creada exitosamente!")

// Error
toast.error("Error al crear cuenta. Intenta nuevamente.")

// Info
toast.info("Completa tu perfil para continuar")

// Loading
const toastId = toast.loading("Creando perfil...")
// Later
toast.dismiss(toastId)
toast.success("Perfil creado!")
```

**Posición:** Top-center (configurado en providers)

**Duración:** 5 segundos por defecto

**Estilo:** Dark theme compatible

---

## 11. Redirecciones Post-Registro

### 11.1 Flujo Completo de Navegación

```
┌─────────────────────────────────────────────────────────────────┐
│ Landing Page (/) → Click "Registrarse"                          │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ /auth/register → RegisterForm → Submit                          │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ authService.register() → Success                                │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ auth-store.login(user, token) → Persist                         │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ router.push("/artista/perfil/crear")                            │
│ → REDIRECT 1: A crear perfil de artista                         │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ /artista/perfil/crear → CreateArtistaForm → Submit              │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ artistaService.create() → Success                               │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ router.push("/dashboard")                                       │
│ → REDIRECT 2: A dashboard principal                             │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ /dashboard → useMyArtistProfile() → artista !== null            │
│ → Mostrar dashboard normal (sin banner)                         │
└─────────────────────────────────────────────────────────────────┘
```

---

### 11.2 Flujo Alternativo (Usuario Omite Creación de Perfil)

**Escenario:** Usuario registra cuenta pero cierra navegador SIN crear perfil.

```
┌─────────────────────────────────────────────────────────────────┐
│ /auth/register → Success → router.push("/artista/perfil/crear") │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ Usuario CIERRA navegador sin completar perfil                  │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ Usuario vuelve → Sesión persiste (localStorage + Zustand)      │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ Usuario navega directamente a /dashboard                        │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ useMyArtistProfile() → artista === null                         │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ <CompleteProfileBanner /> se muestra                            │
│ → Mensaje: "Completa tu perfil de artista"                     │
│ → Link: /artista/perfil/crear                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Protección Adicional (Opcional):**

Si usuario intenta crear campaña SIN perfil:

```tsx
// src/app/(dashboard)/campanias/nueva/page.tsx

useEffect(() => {
  if (!isLoading && !artista) {
    toast.error("Necesitas completar tu perfil de artista primero")
    router.push("/artista/perfil/crear")
  }
}, [isLoading, artista, router])
```

---

### 11.3 Casos Edge

**Caso 1:** Token expira entre registro y creación de perfil

```
/auth/register → Success → /artista/perfil/crear
→ User espera 24+ horas → Token expira
→ User intenta submit → Backend retorna 401
→ Axios interceptor:
  - Elimina token de localStorage
  - Llama auth-store.logout()
  - window.location.href = "/login"
```

**Caso 2:** Usuario registra dos veces con mismo email

```
/auth/register → Submit con email ya registrado
→ Backend retorna 409 Conflict con errorCode AUTH_EMAIL_EXISTS
→ Frontend muestra mensaje: "Este email ya está registrado. ¿Quieres iniciar sesión?"
→ Link a /auth/login
```

**Caso 3:** Usuario ya tiene perfil de artista pero intenta crear otro

```
/artista/perfil/crear → Submit
→ Backend retorna 409 Conflict con errorCode ARTISTA_ALREADY_EXISTS
→ Frontend muestra toast: "Ya tienes un perfil de artista creado"
→ Redirect a /dashboard
```

**Implementación:**

```tsx
// src/app/(dashboard)/artista/perfil/crear/page.tsx (AGREGAR)

useEffect(() => {
  const checkExistingProfile = async () => {
    const profile = await artistaService.getMyProfile()
    if (profile) {
      toast.info("Ya tienes un perfil de artista")
      router.push("/dashboard")
    }
  }
  checkExistingProfile()
}, [router])
```

---

## 12. Análisis: Qué ya existe vs qué falta

### 12.1 Componentes

| Componente | Estado | Ubicación | Acción |
|-----------|--------|-----------|--------|
| `RegisterForm` | ✅ Completo | `components/auth/RegisterForm.tsx` | ✅ Ninguna |
| `CreateArtistaForm` | ⚠️ Falta image preview | `components/artistas/CreateArtistaForm.tsx` | ⚠️ Agregar preview con debounce |
| `CompleteProfileBanner` | ❌ No existe | - | ❌ Crear desde cero |
| `LoginForm` | ✅ Existe (inline en page) | `app/(auth)/login/page.tsx` | ✅ Ninguna |

---

### 12.2 Páginas

| Página | Estado | Ubicación | Acción |
|--------|--------|-----------|--------|
| `/auth/register` | ✅ Completa | `app/(auth)/register/page.tsx` | ✅ Ninguna |
| `/artista/perfil/crear` | ⚠️ Falta "Saltar por ahora" | `app/(dashboard)/artista/perfil/crear/page.tsx` | ⚠️ Agregar botón (opcional) |
| `/dashboard` | ⚠️ Falta banner | `app/(dashboard)/dashboard/page.tsx` | ⚠️ Agregar `<CompleteProfileBanner />` |
| `/auth/login` | ✅ Completa | `app/(auth)/login/page.tsx` | ✅ Ninguna |

---

### 12.3 Hooks

| Hook | Estado | Ubicación | Acción |
|------|--------|-----------|--------|
| `useRegister` (vía authService) | ✅ Funcional | `services/auth.service.ts` | ⚠️ Mejorar manejo errores |
| `useCreateArtista` | ✅ Completo | `hooks/use-artista.ts` | ✅ Ninguna |
| `useMyArtistProfile` | ✅ Completo | `hooks/use-artista.ts` | ⚠️ Validar endpoint (ver sección 5.2) |

---

### 12.4 Services

| Service | Estado | Ubicación | Acción |
|---------|--------|-----------|--------|
| `authService.register()` | ✅ Funcional | `services/auth.service.ts` | ⚠️ Mapear errorCode a mensaje |
| `authService.login()` | ✅ Completo | `services/auth.service.ts` | ✅ Ninguna |
| `authService.getCurrentUser()` | ✅ Completo | `services/auth.service.ts` | ✅ Ninguna |
| `artistaService.create()` | ✅ Completo | `services/artista.service.ts` | ✅ Ninguna |
| `artistaService.getMyProfile()` | ⚠️ Endpoint /me no existe | `services/artista.service.ts` | ⚠️ Usar /by-user/{userId} o agregar /me en backend |

---

### 12.5 Store

| Store | Estado | Ubicación | Acción |
|-------|--------|-----------|--------|
| `auth-store` | ✅ Completo | `store/auth-store.ts` | ✅ Ninguna |

---

### 12.6 Infraestructura

| Elemento | Estado | Ubicación | Acción |
|----------|--------|-----------|--------|
| Axios client con interceptors | ✅ Completo | `lib/api-client.ts` | ⚠️ Agregar logout() en interceptor 401 |
| shadcn/ui components | ✅ Instalados | `components/ui/` | ✅ Ninguna |
| TanStack Query setup | ✅ Configurado | `providers/index.tsx` | ✅ Ninguna |
| Toast notifications | ✅ Configurado (Sonner) | `components/ui/sonner.tsx` | ✅ Ninguna |
| Auth layout | ✅ Completo | `app/(auth)/layout.tsx` | ✅ Ninguna |
| Dashboard layout con auth guard | ✅ Completo | `app/(dashboard)/layout.tsx` | ✅ Ninguna |

---

## 13. Checklist de Implementación

### 13.1 Prioridad Alta (Crítico para MVP)

- [ ] **Crear `CompleteProfileBanner` component**
  - Archivo: `src/components/dashboard/complete-profile-banner.tsx`
  - Lógica: Mostrar si `artista === null && !isLoading`
  - Dismiss: localStorage temporal

- [ ] **Agregar banner en `/dashboard`**
  - Archivo: `src/app/(dashboard)/dashboard/page.tsx`
  - Importar y renderizar `<CompleteProfileBanner />`

- [ ] **Mejorar manejo de errores en `authService.register()`**
  - Archivo: `src/services/auth.service.ts`
  - Mapear `errorCode` del backend a mensajes user-friendly
  - Usar `getErrorMessage()` de `@shared/utils`

- [ ] **Validar endpoint `/api/artistas/me` en backend**
  - Si NO existe: Modificar `artistaService.getMyProfile()` para usar `/by-user/{userId}`
  - Si SÍ existe: Ninguna acción

---

### 13.2 Prioridad Media (Importante para UX)

- [ ] **Agregar image preview en `CreateArtistaForm`**
  - Archivo: `src/components/artistas/CreateArtistaForm.tsx`
  - Debounced validation 500ms
  - Preview con placeholder si error
  - Skeleton durante carga

- [ ] **Agregar protección de rutas que requieren perfil artista**
  - Archivos: `app/(dashboard)/campanias/nueva/page.tsx`, `perfil/page.tsx`
  - Redirect a `/artista/perfil/crear` si `artista === null`

- [ ] **Mejorar axios interceptor 401**
  - Archivo: `src/lib/api-client.ts`
  - Llamar `auth-store.logout()` además de limpiar localStorage

---

### 13.3 Prioridad Baja (Mejoras Opcionales)

- [ ] **Agregar botón "Saltar por ahora" en crear perfil**
  - Archivo: `src/app/(dashboard)/artista/perfil/crear/page.tsx`
  - Crear perfil con solo nombreArtistico (mínimo required)

- [ ] **Agregar toggle password visibility en RegisterForm**
  - Archivo: `src/components/auth/RegisterForm.tsx`
  - Eye icon para mostrar/ocultar password

- [ ] **Agregar verificación de perfil existente antes de crear**
  - Archivo: `src/app/(dashboard)/artista/perfil/crear/page.tsx`
  - useEffect que verifica si ya tiene perfil → redirect a /dashboard

- [ ] **Implementar Next.js middleware para protección de rutas**
  - Archivo: `src/middleware.ts` (crear)
  - Requiere cambiar de localStorage a cookies

---

## 14. Testing Sugerido

### 14.1 Flujos E2E a Validar

| Flujo | Pasos | Resultado Esperado |
|-------|-------|-------------------|
| **Happy Path - Registro completo** | 1. /register → llenar form → submit<br>2. /artista/perfil/crear → llenar form → submit<br>3. /dashboard | Dashboard sin banner, artista visible |
| **Registro parcial** | 1. /register → submit → cerrar navegador<br>2. Volver → /dashboard | Banner "Completa tu perfil" visible |
| **Email duplicado** | 1. /register con email existente → submit | Mensaje "Email ya registrado" + link a /login |
| **Token expirado** | 1. Registrar → esperar 24h → crear perfil | Redirect a /login tras 401 |
| **Protección de ruta /campanias/nueva** | 1. Registrar sin perfil → navegar a /campanias/nueva | Redirect a /artista/perfil/crear |

---

### 14.2 Unit Tests Sugeridos

| Componente/Hook | Test |
|----------------|------|
| `RegisterForm` | Validación email inválido muestra error |
| `RegisterForm` | Passwords no coinciden bloquea submit |
| `RegisterForm` | Submit exitoso llama onSubmit con datos correctos |
| `CreateArtistaForm` | nombreArtistico vacío muestra error |
| `CreateArtistaForm` | Descripción > 2000 chars muestra error |
| `CreateArtistaForm` | Image preview carga con URL válida |
| `CompleteProfileBanner` | No se muestra si artista existe |
| `CompleteProfileBanner` | Se muestra si artista === null |
| `CompleteProfileBanner` | Dismiss guarda en localStorage |
| `useCreateArtista` | Invalida QUERY_KEYS.ARTISTA_ME tras success |
| `authService.register` | Mapea errorCode AUTH_EMAIL_EXISTS correctamente |

---

## 15. Próximos Pasos

### 15.1 Orden de Implementación Sugerido

1. **Crear `CompleteProfileBanner` component** (30 min)
2. **Agregar banner en `/dashboard`** (5 min)
3. **Mejorar manejo errores en `authService`** (15 min)
4. **Validar endpoint `/api/artistas/me`** (10 min backend, 5 min frontend si hay que modificar)
5. **Agregar image preview en `CreateArtistaForm`** (45 min)
6. **Agregar protección de rutas** (20 min)
7. **Testing manual de flujos E2E** (30 min)
8. **Mejoras opcionales** (según tiempo disponible)

**Tiempo Total Estimado:** ~2.5 horas

---

### 15.2 Dependencias con Otros Planes

| Plan | Dependencia | Estado |
|------|------------|--------|
| `backend-plan.md` | Endpoints `/api/auth/register`, `/api/artistas` | ✅ Asumido implementado |
| `shared/contracts-plan.md` | Types, Schemas, Constants | ✅ Ya existe (requiere ajustes menores) |
| `frontend-landing-plan.md` | Perfil público `/artistas/{id}` | ⚠️ Independiente (no bloquea admin) |

---

### 15.3 Validación Final

Antes de marcar la feature como completa, validar:

- [ ] Usuario puede registrarse con email/password
- [ ] Tras registro, redirect a crear perfil
- [ ] Usuario puede crear perfil de artista con nombre, descripción, país, ciudad, imagen
- [ ] Tras crear perfil, redirect a dashboard
- [ ] Dashboard NO muestra banner si artista existe
- [ ] Dashboard SÍ muestra banner si artista NO existe
- [ ] Banner tiene link a /artista/perfil/crear
- [ ] Banner se puede cerrar temporalmente
- [ ] Errores del backend se mapean a mensajes user-friendly
- [ ] Token expira tras 24h y redirige a login
- [ ] Email duplicado muestra mensaje apropiado
- [ ] Image preview funciona con debounce en crear perfil
- [ ] Rutas protegidas redirigen correctamente

---

## 16. Resumen Ejecutivo

### 16.1 Estado Actual

**✅ Funcional al 85%**

La mayoría del código para la feature "registro-artista" **ya está implementado** en el proyecto:
- Página de registro: ✅ Completa
- Página de crear perfil: ✅ Completa (falta image preview)
- Hooks de React Query: ✅ Completos
- Services API: ✅ Completos (requiere mejoras menores)
- Store de autenticación: ✅ Completo
- Validación con Zod: ✅ Completa

### 16.2 Tareas Pendientes

**Solo 1 componente nuevo:**
- `CompleteProfileBanner` para mostrar en dashboard

**Mejoras menores:**
- Image preview en CreateArtistaForm
- Mapeo de errores del backend
- Protección de rutas que requieren perfil

**Tiempo estimado:** 2-3 horas

### 16.3 Archivos a Crear/Modificar

**Nuevos (1):**
1. `src/components/dashboard/complete-profile-banner.tsx`

**Modificar (4):**
1. `src/app/(dashboard)/dashboard/page.tsx` - Agregar banner
2. `src/components/artistas/CreateArtistaForm.tsx` - Image preview
3. `src/services/auth.service.ts` - Mapeo errores
4. `src/lib/api-client.ts` - Logout en interceptor 401

### 16.4 Siguiente Acción Inmediata

**Implementar `CompleteProfileBanner`** y validar flujo E2E de registro completo.

---

**Fin del Plan Frontend: registro-artista (Admin)**
