# Plan Frontend: Registro de Artista (Admin)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/admin (Next.js 14 + App Router)

## 1. Resumen

- Screens: 2 (Registro de Usuario, Crear Perfil Artista)
- Componentes: 5 principales (RegisterForm, CreateArtistaForm, PasswordInput, ImagePreview, CharacterCounter)
- Hooks: 4 (useRegister, useCreateArtista, useArtistaByUserId, useImagePreview)
- Services: 2 métodos nuevos (auth.register, artista.createByUserId)
- Pages: 2 (page.tsx para cada screen)

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   ├── (auth)/
│   │   ├── layout.tsx                    # Ya existe - AuthLayout centrado
│   │   ├── register/
│   │   │   └── page.tsx                  # Actualizar con nuevo flujo
│   │   └── artista/
│   │       └── perfil/
│   │           └── crear/
│   │               └── page.tsx          # Nueva página
│   └── (dashboard)/
│       └── dashboard/
│           └── page.tsx                  # Actualizar para detectar perfil incompleto
│
├── components/
│   ├── auth/
│   │   ├── register-form.tsx             # Nuevo - Formulario de registro
│   │   ├── password-input.tsx            # Nuevo - Input con toggle visibility
│   │   └── index.ts                      # Barrel exports
│   ├── artista/
│   │   ├── create-artista-form.tsx       # Nuevo - Formulario crear perfil
│   │   ├── image-preview.tsx             # Nuevo - Preview de imagen con fallback
│   │   ├── character-counter.tsx         # Nuevo - Contador de caracteres
│   │   └── index.ts                      # Actualizar exports
│   └── ui/                               # Ya existe - shadcn/ui components
│
├── hooks/
│   ├── use-register.ts                   # Nuevo - Mutation para registro
│   ├── use-artista.ts                    # Actualizar - Agregar createArtista mutation
│   ├── use-image-preview.ts              # Nuevo - Debounced preview logic
│   └── index.ts                          # Actualizar exports
│
├── services/
│   ├── auth.service.ts                   # Actualizar - Método register()
│   ├── artista.service.ts                # Actualizar - Método createByUserId()
│   └── index.ts                          # Ya existe
│
├── store/
│   └── auth-store.ts                     # Ya existe - Zustand store con persist
│
└── lib/
    └── api-client.ts                     # Ya existe - Axios config con interceptors
```

## 3. Pages (Next.js App Router)

### 3.1 Register Page (`/auth/register/page.tsx`)

**Archivo:** `src/admin/src/app/(auth)/register/page.tsx`

**Descripción:** Página de registro de usuario con formulario completo usando RegisterForm component.

**Layout padre:** `(auth)/layout.tsx` (ya existe - centrado con Card)

**Responsabilidad:**
- Renderizar RegisterForm component
- Manejar redirect exitoso a `/artista/perfil/crear`
- Mostrar logo y branding de WePlay Rises

**Estado local:** Ninguno (delegado a RegisterForm)

**Flujo:**
1. Usuario completa formulario (email, password, confirmPassword)
2. Submit invoca useRegister mutation
3. Backend retorna JWT token
4. AuthStore guarda token en localStorage
5. Redirect a `/artista/perfil/crear` para completar perfil

**Props:** Ninguna (Server Component con Client Component hijo)

**Estructura:**
```tsx
export default function RegisterPage() {
  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center">
          <div className="flex justify-center mb-4">
            <Music className="h-12 w-12 text-primary" />
          </div>
          <CardTitle className="text-3xl font-bold">Crea tu cuenta</CardTitle>
          <CardDescription>Registra tu perfil de artista</CardDescription>
        </CardHeader>
        <CardContent>
          <RegisterForm />
        </CardContent>
      </Card>
    </div>
  );
}
```

### 3.2 Create Artista Profile Page (`/artista/perfil/crear/page.tsx`)

**Archivo:** `src/admin/src/app/(auth)/artista/perfil/crear/page.tsx`

**Descripción:** Página para crear perfil de artista tras registro exitoso.

**Layout padre:** `(auth)/layout.tsx` (sin sidebar ni header)

**Responsabilidad:**
- Renderizar CreateArtistaForm component
- Verificar que usuario está autenticado (redirect a login si no)
- Verificar que usuario NO tiene perfil (redirect a dashboard si ya tiene)
- Manejar redirect exitoso a `/dashboard`

**Estado local:** Ninguno (delegado a CreateArtistaForm)

**Flujo:**
1. useEffect verifica autenticación y existencia de perfil
2. Usuario completa formulario (nombreArtistico obligatorio, resto opcional)
3. Submit invoca useCreateArtista mutation
4. Backend crea entidad Artista vinculada a UserId del token
5. Toast success + redirect a `/dashboard`

**Props:** Ninguna

**Estructura:**
```tsx
'use client';

export default function CreateArtistaProfilePage() {
  const { isAuthenticated, token } = useAuthStore();
  const { data: artista, isLoading } = useArtistaByUserId(token?.userId);
  const router = useRouter();

  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/auth/login');
    }
    if (artista) {
      router.push('/dashboard'); // Ya tiene perfil
    }
  }, [isAuthenticated, artista]);

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="max-w-2xl mx-auto py-12 px-6">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-white mb-2">
          Completa tu perfil de artista
        </h1>
        <p className="text-lg text-muted-foreground">
          Cuéntanos sobre tu música
        </p>
      </div>
      <Card>
        <CardContent className="p-8">
          <CreateArtistaForm />
        </CardContent>
      </Card>
    </div>
  );
}
```

## 4. Componentes

### 4.1 RegisterForm

**Archivo:** `src/admin/src/components/auth/register-form.tsx`

**Tipo:** Client Component ('use client')

**Props:** Ninguna

**Responsabilidad:**
- Formulario de registro con React Hook Form + Zod
- Validación en tiempo real (onBlur + onChange)
- Mostrar errores de validación frontend y backend
- Invocar useRegister mutation al submit

**Estado Local:**
| Estado | Tipo | Descripción |
|--------|------|-------------|
| showPassword | boolean | Controla visibilidad de password field |
| showConfirmPassword | boolean | Controla visibilidad de confirmPassword field |

**Hooks usados:**
- `useForm<RegisterFormData>` - React Hook Form con zodResolver
- `useRegister()` - Mutation hook para registro
- `useRouter()` - Next.js navigation
- `useAuthStore()` - Zustand store para guardar token

**Componentes UI:**
- `Input` (shadcn/ui)
- `Label` (shadcn/ui)
- `Button` (shadcn/ui)
- `PasswordInput` (custom)

**Validación:**
```typescript
// Usa registerSchema de @shared/schemas
const form = useForm<RegisterFormData>({
  resolver: zodResolver(registerSchema),
  mode: 'onBlur', // Valida al perder foco
});
```

**Manejo de errores:**
```typescript
// Errores de validación frontend (Zod)
{errors.email && (
  <p className="text-sm text-destructive">{errors.email.message}</p>
)}

// Errores del backend (ServiceResponse)
onError: (error) => {
  const errorCode = error.response?.data?.messages?.[0]?.errorCode;
  const message = getErrorMessage(errorCode);
  toast.error(message);
}
```

**Estructura:**
```tsx
export function RegisterForm() {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const { login } = useAuthStore();
  const router = useRouter();
  const { mutate: register, isPending } = useRegister();

  const form = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      email: '',
      password: '',
      confirmPassword: '',
    },
  });

  const onSubmit = (data: RegisterFormData) => {
    register(data, {
      onSuccess: (response) => {
        login(response.userId, response.email, response.token);
        toast.success('Cuenta creada exitosamente');
        router.push('/artista/perfil/crear');
      },
      onError: (error) => {
        const errorCode = error.response?.data?.messages?.[0]?.errorCode;
        toast.error(getErrorMessage(errorCode));
      },
    });
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
      {/* Email field */}
      {/* Password field with toggle */}
      {/* Confirm Password field with toggle */}
      <Button type="submit" disabled={isPending} className="w-full">
        {isPending ? 'Creando cuenta...' : 'Crear cuenta'}
      </Button>
      <p className="text-center text-sm text-muted-foreground">
        ¿Ya tienes cuenta?{' '}
        <Link href="/auth/login" className="text-primary hover:underline">
          Iniciar sesión
        </Link>
      </p>
    </form>
  );
}
```

### 4.2 PasswordInput

**Archivo:** `src/admin/src/components/auth/password-input.tsx`

**Tipo:** Client Component

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| id | string | Sí | ID del input (para label) |
| label | string | Sí | Texto del label |
| placeholder | string | No | Placeholder text |
| error | string | No | Mensaje de error |
| ...inputProps | InputHTMLAttributes | No | Props del input nativo |

**Responsabilidad:**
- Input type="password" con toggle de visibilidad
- Icono de ojo que cambia entre password/text
- Mostrar mensaje de error debajo si existe

**Estado Local:**
| Estado | Tipo | Descripción |
|--------|------|-------------|
| showPassword | boolean | Controla type del input |

**Componentes UI:**
- `Input` (shadcn/ui)
- `Label` (shadcn/ui)
- `Button` (shadcn/ui, variant="ghost", size="icon")
- `Eye` / `EyeOff` icons (lucide-react)

**Estructura:**
```tsx
export function PasswordInput({ id, label, placeholder, error, ...props }: Props) {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <div className="relative">
        <Input
          id={id}
          type={showPassword ? 'text' : 'password'}
          placeholder={placeholder}
          {...props}
        />
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="absolute right-2 top-1/2 -translate-y-1/2"
          onClick={() => setShowPassword(!showPassword)}
        >
          {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
        </Button>
      </div>
      {error && <p className="text-sm text-destructive">{error}</p>}
    </div>
  );
}
```

### 4.3 CreateArtistaForm

**Archivo:** `src/admin/src/components/artista/create-artista-form.tsx`

**Tipo:** Client Component

**Props:** Ninguna

**Responsabilidad:**
- Formulario para crear perfil de artista
- Validación con createArtistaSchema de shared
- Preview de imagen con debounce
- Character counter en descripción
- Submit invoca useCreateArtista mutation

**Estado Local:** Ninguno (manejado por React Hook Form)

**Hooks usados:**
- `useForm<CreateArtistaFormData>` - RHF con zodResolver
- `useCreateArtista()` - Mutation hook
- `useImagePreview(imagenUrl)` - Preview con debounce
- `useRouter()` - Next.js navigation

**Componentes UI:**
- `Input`, `Label`, `Textarea`, `Button` (shadcn/ui)
- `ImagePreview` (custom)
- `CharacterCounter` (custom)

**Validación:**
```typescript
const form = useForm<CreateArtistaFormData>({
  resolver: zodResolver(createArtistaSchema),
  defaultValues: {
    nombreArtistico: '',
    descripcion: '',
    pais: '',
    ciudad: '',
    imagenUrl: '',
  },
});
```

**Estructura:**
```tsx
export function CreateArtistaForm() {
  const router = useRouter();
  const { mutate: createArtista, isPending } = useCreateArtista();

  const form = useForm<CreateArtistaFormData>({
    resolver: zodResolver(createArtistaSchema),
  });

  const imagenUrl = form.watch('imagenUrl');
  const descripcion = form.watch('descripcion') || '';

  const onSubmit = (data: CreateArtistaFormData) => {
    createArtista(data, {
      onSuccess: () => {
        toast.success('Perfil creado exitosamente');
        router.push('/dashboard');
      },
      onError: (error) => {
        toast.error(getErrorMessage(error.response?.data?.messages?.[0]?.errorCode));
      },
    });
  };

  const handleSkip = () => {
    // Crear perfil con solo nombreArtistico (mínimo)
    if (!form.getValues('nombreArtistico')) {
      toast.error('El nombre artístico es obligatorio');
      return;
    }
    createArtista(
      { nombreArtistico: form.getValues('nombreArtistico') },
      {
        onSuccess: () => {
          toast.success('Perfil creado. Complétalo más tarde desde tu dashboard');
          router.push('/dashboard');
        },
      }
    );
  };

  return (
    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
      {/* Nombre Artístico (required) */}
      {/* Descripción con CharacterCounter */}
      {/* Grid: País y Ciudad */}
      {/* Imagen URL con ImagePreview */}
      <div className="flex items-center gap-4">
        <Button type="submit" disabled={isPending}>
          {isPending ? 'Guardando...' : 'Guardar y continuar'}
        </Button>
        <Button type="button" variant="ghost" onClick={handleSkip}>
          Saltar por ahora
        </Button>
      </div>
    </form>
  );
}
```

### 4.4 ImagePreview

**Archivo:** `src/admin/src/components/artista/image-preview.tsx`

**Tipo:** Client Component

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| imageUrl | string | Sí | URL de la imagen a previsualizar |
| altText | string | No | Alt text (default: "Preview") |
| size | 'sm' \| 'md' \| 'lg' | No | Tamaño del preview (default: 'md') |

**Responsabilidad:**
- Previsualizar imagen desde URL
- Mostrar loading skeleton mientras carga
- Mostrar icono placeholder si URL inválida o no carga
- Usar Avatar component de shadcn/ui

**Estado Local:**
| Estado | Tipo | Descripción |
|--------|------|-------------|
| isLoading | boolean | Indica si imagen está cargando |
| hasError | boolean | Indica si hubo error al cargar |

**Componentes UI:**
- `Avatar`, `AvatarImage`, `AvatarFallback` (shadcn/ui)
- `Music` icon (lucide-react) para fallback
- `Skeleton` (shadcn/ui) para loading

**Estructura:**
```tsx
export function ImagePreview({ imageUrl, altText = 'Preview', size = 'md' }: Props) {
  const [isLoading, setIsLoading] = useState(true);
  const [hasError, setHasError] = useState(false);

  const sizeClasses = {
    sm: 'w-16 h-16',
    md: 'w-32 h-32',
    lg: 'w-48 h-48',
  };

  if (!imageUrl) {
    return (
      <Avatar className={cn(sizeClasses[size], 'rounded-lg')}>
        <AvatarFallback>
          <Music className="h-8 w-8 text-muted-foreground" />
        </AvatarFallback>
      </Avatar>
    );
  }

  return (
    <Avatar className={cn(sizeClasses[size], 'rounded-lg')}>
      {isLoading && <Skeleton className="h-full w-full" />}
      <AvatarImage
        src={imageUrl}
        alt={altText}
        onLoad={() => setIsLoading(false)}
        onError={() => {
          setIsLoading(false);
          setHasError(true);
        }}
      />
      {hasError && (
        <AvatarFallback>
          <Music className="h-8 w-8 text-muted-foreground" />
        </AvatarFallback>
      )}
    </Avatar>
  );
}
```

### 4.5 CharacterCounter

**Archivo:** `src/admin/src/components/artista/character-counter.tsx`

**Tipo:** Client Component

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| currentLength | number | Sí | Caracteres actuales |
| maxLength | number | Sí | Límite de caracteres |
| className | string | No | Classes adicionales |

**Responsabilidad:**
- Mostrar contador "X/Y caracteres"
- Cambiar color según proximidad al límite:
  - Gris: < 80% del límite
  - Amarillo: 80-95% del límite
  - Rojo: > 95% del límite

**Componentes UI:**
- `<span>` con classes condicionales

**Estructura:**
```tsx
export function CharacterCounter({ currentLength, maxLength, className }: Props) {
  const percentage = (currentLength / maxLength) * 100;

  const colorClass =
    percentage >= 95
      ? 'text-destructive'
      : percentage >= 80
      ? 'text-yellow-500'
      : 'text-muted-foreground';

  return (
    <span className={cn('text-xs', colorClass, className)}>
      {currentLength}/{maxLength} caracteres
    </span>
  );
}
```

## 5. Hooks

### 5.1 useRegister

**Archivo:** `src/admin/src/hooks/use-register.ts`

**Tipo:** Mutation Hook (TanStack Query)

**Descripción:** Hook para registrar nuevo usuario y obtener JWT token.

**Parámetros:** Ninguno (recibe data en mutationFn)

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| mutate | function | Ejecuta mutation con RegisterRequest |
| isPending | boolean | Indica si está en proceso |
| isError | boolean | Indica si hubo error |
| error | AxiosError | Error si lo hay |
| data | RegisterResponse | Respuesta exitosa (userId, email, token) |

**Invalidaciones:** Ninguna (no hay cache de usuarios)

**Estructura:**
```typescript
import { useMutation } from '@tanstack/react-query';
import { authService } from '@/services/auth.service';
import type { RegisterRequest, RegisterResponse } from '@shared/types/auth';

export function useRegister() {
  return useMutation<RegisterResponse, AxiosError, RegisterRequest>({
    mutationFn: (data: RegisterRequest) => authService.register(data),
  });
}
```

### 5.2 useCreateArtista

**Archivo:** `src/admin/src/hooks/use-artista.ts` (actualizar existente)

**Tipo:** Mutation Hook

**Descripción:** Hook para crear perfil de artista tras registro.

**Parámetros:** Ninguno

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| mutate | function | Ejecuta mutation con CreateArtistaRequest |
| isPending | boolean | Indica si está en proceso |
| error | AxiosError | Error si lo hay |
| data | Artista | Perfil creado |

**Invalidaciones:**
- `QUERY_KEYS.artistas.all` - Invalida todas las queries de artistas
- `QUERY_KEYS.artistas.byUserId(userId)` - Invalida query del artista actual

**Estructura:**
```typescript
export function useCreateArtista() {
  const queryClient = useQueryClient();

  return useMutation<Artista, AxiosError, CreateArtistaRequest>({
    mutationFn: (data: CreateArtistaRequest) => artistaService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.all });
      // Nota: userId viene del token, no necesitamos invalidar específicamente
    },
  });
}
```

### 5.3 useArtistaByUserId

**Archivo:** `src/admin/src/hooks/use-artista.ts` (agregar a existente)

**Tipo:** Query Hook

**Descripción:** Hook para obtener perfil de artista por UserId (verificar si ya tiene perfil).

**Parámetros:**
| Param | Tipo | Descripción |
|-------|------|-------------|
| userId | string \| undefined | UserId extraído del token JWT |

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | Artista \| null | Perfil del artista si existe |
| isLoading | boolean | Indica si está cargando |
| error | AxiosError | Error si lo hay |

**Query Key:** `QUERY_KEYS.artistas.byUserId(userId)`

**Enabled:** Solo si userId existe

**Estructura:**
```typescript
export function useArtistaByUserId(userId?: string) {
  return useQuery({
    queryKey: QUERY_KEYS.artistas.byUserId(userId!),
    queryFn: () => artistaService.getByUserId(userId!),
    enabled: !!userId, // Solo ejecuta si userId existe
    retry: false, // No reintentar si 404 (no tiene perfil)
  });
}
```

### 5.4 useImagePreview

**Archivo:** `src/admin/src/hooks/use-image-preview.ts`

**Tipo:** Custom Hook

**Descripción:** Hook para manejar preview de imagen con debounce (evita requests excesivos).

**Parámetros:**
| Param | Tipo | Descripción |
|-------|------|-------------|
| imageUrl | string \| undefined | URL de la imagen |
| delay | number | Delay en ms (default: 500) |

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| debouncedUrl | string | URL debounced para preview |
| isValidating | boolean | Indica si está en periodo de debounce |

**Estructura:**
```typescript
import { useState, useEffect } from 'react';

export function useImagePreview(imageUrl?: string, delay = 500) {
  const [debouncedUrl, setDebouncedUrl] = useState(imageUrl);
  const [isValidating, setIsValidating] = useState(false);

  useEffect(() => {
    if (!imageUrl) {
      setDebouncedUrl('');
      setIsValidating(false);
      return;
    }

    setIsValidating(true);
    const handler = setTimeout(() => {
      setDebouncedUrl(imageUrl);
      setIsValidating(false);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [imageUrl, delay]);

  return { debouncedUrl, isValidating };
}
```

## 6. Services

### 6.1 authService.register()

**Archivo:** `src/admin/src/services/auth.service.ts` (actualizar existente)

**Método:** `register(data: RegisterRequest): Promise<RegisterResponse>`

**Descripción:** Registra nuevo usuario y retorna JWT token.

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```typescript
{
  email: string;
  password: string;
  confirmPassword: string;
}
```

**Response Success (200):**
```typescript
{
  userId: string;
  email: string;
  token: string;
}
```

**Errores posibles:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 400 | AUTH_EMAIL_INVALID | Email inválido |
| 400 | AUTH_PASSWORD_MIN_LENGTH | Password < 8 caracteres |
| 400 | AUTH_PASSWORD_MISMATCH | Passwords no coinciden |
| 409 | AUTH_EMAIL_EXISTS | Email ya registrado |

**Implementación:**
```typescript
async register(data: RegisterRequest): Promise<RegisterResponse> {
  const response = await apiFetch<ServiceResponse<RegisterResponse>>(
    API_ROUTES.auth.register,
    {
      method: 'POST',
      data,
    }
  );

  if (!response.data.isSuccess) {
    throw new Error(response.data.messages[0]?.message);
  }

  return response.data.data!;
}
```

### 6.2 artistaService.create()

**Archivo:** `src/admin/src/services/artista.service.ts` (actualizar existente)

**Método:** `create(data: CreateArtistaRequest): Promise<Artista>`

**Descripción:** Crea perfil de artista vinculado al UserId del token JWT.

**Endpoint:** `POST /api/artistas`

**Headers:** `Authorization: Bearer {token}` (automático via interceptor)

**Request Body:**
```typescript
{
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
}
```

**Response Success (200):**
```typescript
{
  id: string;
  userId: string;
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
  fechaCreacion: string;
  fechaActualizacion?: string;
}
```

**Errores posibles:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 400 | VALIDATION_REQUIRED | nombreArtistico vacío |
| 400 | ARTISTA_NOMBRE_MAX_LENGTH | nombreArtistico > 200 chars |
| 401 | AUTH_UNAUTHORIZED | Token inválido/expirado |
| 409 | ARTISTA_ALREADY_EXISTS | UserId ya tiene perfil |

**Implementación:**
```typescript
async create(data: CreateArtistaRequest): Promise<Artista> {
  const response = await apiFetch<ServiceResponse<Artista>>(
    API_ROUTES.artistas.base,
    {
      method: 'POST',
      data,
    }
  );

  if (!response.data.isSuccess) {
    throw new Error(response.data.messages[0]?.message);
  }

  return response.data.data!;
}
```

### 6.3 artistaService.getByUserId()

**Archivo:** `src/admin/src/services/artista.service.ts` (agregar método nuevo)

**Método:** `getByUserId(userId: string): Promise<Artista | null>`

**Descripción:** Obtiene perfil de artista por UserId (para verificar si existe).

**Endpoint:** `GET /api/artistas/by-user/{userId}`

**Headers:** `Authorization: Bearer {token}`

**Response Success (200):**
```typescript
{
  id: string;
  userId: string;
  nombreArtistico: string;
  // ... resto de campos
}
```

**Errores posibles:**
| HTTP | ErrorCode | Acción |
|------|-----------|--------|
| 404 | ARTISTA_NOT_FOUND | Retornar null (no tiene perfil) |
| 401 | AUTH_UNAUTHORIZED | Throw error (token inválido) |

**Implementación:**
```typescript
async getByUserId(userId: string): Promise<Artista | null> {
  try {
    const response = await apiFetch<ServiceResponse<Artista>>(
      API_ROUTES.artistas.byUserId(userId)
    );

    if (!response.data.isSuccess) {
      return null; // 404 - no tiene perfil
    }

    return response.data.data!;
  } catch (error) {
    if (error.response?.status === 404) {
      return null; // Usuario no tiene perfil aún
    }
    throw error; // Otros errores (401, 500) sí los propagamos
  }
}
```

## 7. State Management

### 7.1 AuthStore (Zustand)

**Archivo:** `src/admin/src/store/auth-store.ts` (ya existe, actualizar si necesario)

**Estado:**
```typescript
interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (userId: string, email: string, token: string) => void;
  logout: () => void;
}
```

**Persistencia:** localStorage con `zustand/persist`

**Acciones:**
- `login(userId, email, token)` - Guarda token en localStorage, actualiza estado
- `logout()` - Limpia token de localStorage, resetea estado

**Uso en componentes:**
```typescript
const { login, isAuthenticated, token } = useAuthStore();

// Después de registro exitoso
login(response.userId, response.email, response.token);
```

**Nota:** El token se guarda también en localStorage para que axios interceptor lo agregue a headers automáticamente.

## 8. Flujo de Datos

```
1. Registro de Usuario
   User completa RegisterForm
        ↓
   useRegister mutation
        ↓
   authService.register(data)
        ↓
   POST /api/auth/register
        ↓
   Backend valida y crea usuario Identity
        ↓
   Retorna JWT token
        ↓
   AuthStore.login(userId, email, token)
        ↓
   localStorage.setItem('token', token)
        ↓
   router.push('/artista/perfil/crear')

2. Crear Perfil Artista
   User llega a /artista/perfil/crear
        ↓
   useArtistaByUserId(userId) verifica si ya tiene perfil
        ↓
   Si tiene perfil → redirect a /dashboard
   Si no tiene perfil → muestra CreateArtistaForm
        ↓
   User completa formulario
        ↓
   useCreateArtista mutation
        ↓
   artistaService.create(data)
        ↓
   POST /api/artistas (con token en headers)
        ↓
   Backend crea Artista vinculado a UserId
        ↓
   Retorna Artista creado
        ↓
   Invalida queries de artistas
        ↓
   Toast success
        ↓
   router.push('/dashboard')
```

## 9. Dependencias de Shared

**Importar desde `@shared/`:**

### Types:
```typescript
import type {
  RegisterRequest,
  RegisterResponse,
  CreateArtistaRequest,
  Artista,
} from '@shared/types';
```

### Schemas:
```typescript
import {
  registerSchema,
  createArtistaSchema,
  type RegisterFormData,
  type CreateArtistaFormData,
} from '@shared/schemas';
```

### Constantes:
```typescript
import { API_ROUTES, QUERY_KEYS, APP_ROUTES } from '@shared/constants';
```

### Utilidades:
```typescript
import { getErrorMessage } from '@shared/utils/error-messages';
```

## 10. Validación de Formularios

### 10.1 RegisterForm Validation

**Schema:** `registerSchema` de `@shared/schemas/auth.schema.ts`

**Reglas:**
| Campo | Validación | Mensaje |
|-------|-----------|---------|
| email | required + email format | "El email es obligatorio" / "Formato de email inválido" |
| password | required + min 8 chars | "La contraseña es obligatoria" / "Mínimo 8 caracteres" |
| confirmPassword | required + match password | "Confirme su contraseña" / "Las contraseñas no coinciden" |

**Validación en tiempo real:**
- `mode: 'onBlur'` - Valida al perder foco
- `reValidateMode: 'onChange'` - Revalida al escribir después del primer error

### 10.2 CreateArtistaForm Validation

**Schema:** `createArtistaSchema` de `@shared/schemas/artista.schema.ts`

**Reglas:**
| Campo | Validación | Mensaje |
|-------|-----------|---------|
| nombreArtistico | required + max 200 chars | "El nombre artístico es obligatorio" / "Máximo 200 caracteres" |
| descripcion | optional + max 2000 chars | "Máximo 2000 caracteres" |
| pais | optional + max 100 chars | "Máximo 100 caracteres" |
| ciudad | optional + max 100 chars | "Máximo 100 caracteres" |
| imagenUrl | optional + valid URL | "Debe ser una URL válida" |

**Validación en tiempo real:**
- `mode: 'onBlur'`
- Character counter actualizado con `form.watch('descripcion')`
- Image preview con debounce de 500ms

## 11. Manejo de Errores

### 11.1 Errores de Validación Frontend (Zod)

**Mostrados inmediatamente debajo del input:**
```tsx
{errors.email && (
  <p className="text-sm text-destructive">{errors.email.message}</p>
)}
```

### 11.2 Errores del Backend (ServiceResponse)

**Mostrados con Toast:**
```typescript
onError: (error) => {
  const errorCode = error.response?.data?.messages?.[0]?.errorCode;
  const message = getErrorMessage(errorCode);
  toast.error(message);
}
```

**Mapeo de errores:**
| ErrorCode Backend | Mensaje Usuario |
|-------------------|-----------------|
| `AUTH_EMAIL_EXISTS` | "Este email ya está registrado. ¿Quieres iniciar sesión?" |
| `AUTH_PASSWORD_MIN_LENGTH` | "La contraseña debe tener al menos 8 caracteres" |
| `ARTISTA_ALREADY_EXISTS` | "Ya tienes un perfil de artista creado" |
| `ERROR_UNEXPECTED` | "Ha ocurrido un error inesperado. Por favor, intenta nuevamente" |

### 11.3 Errores de Red (Axios Interceptor)

**Manejados globalmente en `api-client.ts`:**
- 401 Unauthorized → Redirect automático a `/auth/login`
- Otros errores → Propagados al componente

## 12. Routing y Navegación

### 12.1 Rutas Públicas (Auth)

| Ruta | Layout | Descripción |
|------|--------|-------------|
| `/auth/register` | `(auth)/layout.tsx` | Registro de usuario |
| `/auth/login` | `(auth)/layout.tsx` | Login (ya existe) |

### 12.2 Rutas Semi-Privadas (Auth requerida)

| Ruta | Layout | Descripción | Verificación |
|------|--------|-------------|--------------|
| `/artista/perfil/crear` | `(auth)/layout.tsx` | Crear perfil artista | Autenticado + sin perfil |

**Lógica de verificación:**
```typescript
// En /artista/perfil/crear/page.tsx
useEffect(() => {
  if (!isAuthenticated) {
    router.push('/auth/login'); // No autenticado → login
  }
  if (artista) {
    router.push('/dashboard'); // Ya tiene perfil → dashboard
  }
}, [isAuthenticated, artista]);
```

### 12.3 Rutas Privadas (Dashboard)

| Ruta | Layout | Descripción | Verificación |
|------|--------|-------------|--------------|
| `/dashboard` | `(dashboard)/layout.tsx` | Dashboard principal | Autenticado + con perfil |

**Actualización necesaria en `/dashboard/page.tsx`:**
```typescript
// Agregar verificación de perfil incompleto
const { data: artista } = useArtistaByUserId(userId);

if (!artista) {
  return (
    <Banner variant="warning">
      Completa tu perfil de artista para empezar a crear campañas.
      <Link href="/artista/perfil/crear">Completar ahora</Link>
    </Banner>
  );
}
```

## 13. Responsividad

### 13.1 Register Page

| Breakpoint | Cambios |
|------------|---------|
| Mobile (< 640px) | Card width 100%, padding px-4, font-size reducido |
| Tablet/Desktop | Card max-w-md centrado, padding px-6 |

### 13.2 Create Artista Profile Page

| Breakpoint | Cambios |
|------------|---------|
| Mobile (< 640px) | Stack vertical País/Ciudad, Image preview w-24 h-24, buttons stacked |
| Tablet (640-1024px) | Grid 2 cols para País/Ciudad, Image preview w-32 h-32 |
| Desktop (> 1024px) | Max-w-2xl container, Image preview w-32 h-32 |

**Grid responsive:**
```tsx
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
  <div>País</div>
  <div>Ciudad</div>
</div>
```

## 14. Accesibilidad

### 14.1 Form Accessibility

**Labels asociados:**
```tsx
<Label htmlFor="email">Email</Label>
<Input id="email" aria-required="true" />
```

**Error announcements:**
```tsx
{errors.email && (
  <span role="alert" aria-live="polite" className="text-sm text-destructive">
    {errors.email.message}
  </span>
)}
```

**Loading states:**
```tsx
<Button disabled={isPending} aria-busy={isPending}>
  {isPending ? 'Creando cuenta...' : 'Crear cuenta'}
</Button>
```

### 14.2 Image Preview Accessibility

```tsx
<Avatar>
  <AvatarImage src={imageUrl} alt={`Foto de perfil de ${nombreArtistico}`} />
  <AvatarFallback>
    <Music aria-hidden="true" />
    <span className="sr-only">Imagen no disponible</span>
  </AvatarFallback>
</Avatar>
```

### 14.3 Keyboard Navigation

- Tab order lógico en formularios
- Enter para submit
- Botón "Saltar por ahora" accesible con Tab
- Toggle password visible con Space/Enter

## 15. Testing Strategy

### 15.1 Unit Tests (Componentes)

**RegisterForm.test.tsx:**
- Valida email format
- Valida password min length
- Valida passwords match
- Muestra errores correctamente
- Llama mutation al submit

**CreateArtistaForm.test.tsx:**
- Valida nombreArtistico required
- Character counter funciona
- Image preview se muestra
- Botón "Saltar" funciona

### 15.2 Integration Tests (Hooks)

**useRegister.test.ts:**
- Mutation exitosa guarda token
- Error 409 muestra mensaje correcto

**useCreateArtista.test.ts:**
- Mutation exitosa invalida queries
- Error 401 redirige a login

### 15.3 E2E Tests (Flujo completo)

**registro-artista.spec.ts:**
1. Navegar a /auth/register
2. Completar formulario
3. Submit exitoso
4. Verificar redirect a /artista/perfil/crear
5. Completar perfil
6. Verificar redirect a /dashboard

## 16. Archivos a Crear/Actualizar

### Crear Nuevos:

| Archivo | Tipo | Descripción |
|---------|------|-------------|
| `app/(auth)/artista/perfil/crear/page.tsx` | Page | Página crear perfil artista |
| `components/auth/register-form.tsx` | Component | Formulario de registro |
| `components/auth/password-input.tsx` | Component | Input password con toggle |
| `components/auth/index.ts` | Barrel | Exports de auth components |
| `components/artista/create-artista-form.tsx` | Component | Formulario crear perfil |
| `components/artista/image-preview.tsx` | Component | Preview de imagen |
| `components/artista/character-counter.tsx` | Component | Contador de caracteres |
| `hooks/use-register.ts` | Hook | Mutation hook registro |
| `hooks/use-image-preview.ts` | Hook | Debounced preview logic |

### Actualizar Existentes:

| Archivo | Cambios |
|---------|---------|
| `app/(auth)/register/page.tsx` | Simplificar para usar RegisterForm component |
| `services/auth.service.ts` | Agregar método `register()` |
| `services/artista.service.ts` | Agregar métodos `create()` y `getByUserId()` |
| `hooks/use-artista.ts` | Agregar hooks `useCreateArtista()` y `useArtistaByUserId()` |
| `hooks/index.ts` | Exportar nuevos hooks |
| `components/artista/index.ts` | Exportar nuevos components |
| `app/(dashboard)/dashboard/page.tsx` | Agregar verificación de perfil incompleto |

## 17. Configuración Necesaria

### 17.1 Next.js Config

**Ya configurado** - No requiere cambios.

### 17.2 Environment Variables

**Ya configurado:**
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

### 17.3 Path Aliases

**Ya configurado en `tsconfig.json`:**
```json
{
  "compilerOptions": {
    "paths": {
      "@/*": ["./src/*"],
      "@shared/*": ["../../shared/*"]
    }
  }
}
```

## 18. Performance Considerations

### 18.1 Debounced Image Preview

- 500ms delay evita requests excesivos al tipear URL
- Solo carga imagen cuando URL es estable

### 18.2 Query Cache

- `useArtistaByUserId` cachea resultado
- No invalida cache en register (solo al crear perfil)

### 18.3 Form Validation

- `mode: 'onBlur'` evita validaciones excesivas
- `reValidateMode: 'onChange'` solo tras primer error

### 18.4 Code Splitting

- Componentes de formulario lazy loaded con Next.js automático
- Hooks importados solo cuando se usan

## 19. Security Considerations

### 19.1 Token Storage

**Implementación actual:**
- Token guardado en localStorage (persistente)
- Enviado en headers via axios interceptor
- Limpiado al logout

**Trade-off:**
- localStorage vulnerable a XSS
- Alternativa: httpOnly cookies (requiere backend change)
- Para MVP, localStorage es aceptable con sanitización estricta

### 19.2 Password Handling

- Password nunca almacenado localmente
- Enviado solo via HTTPS (producción)
- Validación min 8 caracteres

### 19.3 Input Sanitization

- React escapa automáticamente JSX
- Zod valida formatos (email, URL)
- Backend hace validación adicional

## 20. Checklist de Implementación

### Setup:
- [ ] Verificar shared types, schemas y constants creados
- [ ] Verificar api-client configurado con interceptors

### Components:
- [ ] RegisterForm component creado
- [ ] PasswordInput component creado
- [ ] CreateArtistaForm component creado
- [ ] ImagePreview component creado
- [ ] CharacterCounter component creado
- [ ] Barrel exports actualizados

### Pages:
- [ ] /auth/register/page.tsx actualizado
- [ ] /artista/perfil/crear/page.tsx creado
- [ ] /dashboard/page.tsx actualizado con verificación de perfil

### Hooks:
- [ ] useRegister hook creado
- [ ] useCreateArtista hook agregado a use-artista.ts
- [ ] useArtistaByUserId hook agregado a use-artista.ts
- [ ] useImagePreview hook creado
- [ ] Hooks exportados en index.ts

### Services:
- [ ] authService.register() implementado
- [ ] artistaService.create() implementado
- [ ] artistaService.getByUserId() implementado

### State:
- [ ] AuthStore verificado y funcional

### Validation:
- [ ] registerSchema integrado con React Hook Form
- [ ] createArtistaSchema integrado con React Hook Form
- [ ] Error messages mapeados correctamente

### Routing:
- [ ] Navegación register → crear perfil funciona
- [ ] Navegación crear perfil → dashboard funciona
- [ ] Verificación de autenticación implementada
- [ ] Verificación de perfil existente implementada

### Testing:
- [ ] Unit tests para componentes críticos
- [ ] Integration tests para hooks
- [ ] E2E test del flujo completo

### Accessibility:
- [ ] Labels asociados a inputs
- [ ] ARIA attributes correctos
- [ ] Keyboard navigation funcional
- [ ] Error announcements con role="alert"

### Responsiveness:
- [ ] Mobile layout probado (< 640px)
- [ ] Tablet layout probado (640-1024px)
- [ ] Desktop layout probado (> 1024px)

## 21. Siguiente Paso Sugerido

**Implementación incremental recomendada:**

1. **Fase 1 - Setup Shared** (1h)
   - Crear types en `@shared/types`
   - Crear schemas en `@shared/schemas`
   - Crear constantes en `@shared/constants`

2. **Fase 2 - Services** (1h)
   - Implementar `authService.register()`
   - Implementar `artistaService.create()`
   - Implementar `artistaService.getByUserId()`

3. **Fase 3 - Hooks** (1h)
   - Crear `useRegister` hook
   - Actualizar `use-artista.ts` con nuevos hooks
   - Crear `useImagePreview` hook

4. **Fase 4 - Components** (2h)
   - Crear `PasswordInput` component
   - Crear `RegisterForm` component
   - Crear `CharacterCounter` component
   - Crear `ImagePreview` component
   - Crear `CreateArtistaForm` component

5. **Fase 5 - Pages** (1h)
   - Actualizar `/auth/register/page.tsx`
   - Crear `/artista/perfil/crear/page.tsx`
   - Actualizar `/dashboard/page.tsx`

6. **Fase 6 - Testing** (1h)
   - Probar flujo completo E2E
   - Ajustar estilos y UX
   - Verificar responsive en mobile/tablet/desktop

**Total estimado:** 7 horas de implementación

**Archivos clave para comenzar:**
1. `C:\Repos\WePlay_Rises\src\shared\types\auth.ts`
2. `C:\Repos\WePlay_Rises\src\shared\schemas\auth.schema.ts`
3. `C:\Repos\WePlay_Rises\src\admin\src\services\auth.service.ts`
4. `C:\Repos\WePlay_Rises\src\admin\src\components\auth\register-form.tsx`
