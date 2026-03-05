# Estrategia de Testing: Registro de Artista (Admin)

**Fecha:** 2026-02-12 (Actualizado)
**Feature:** registro-artista
**Target:** src/admin (Next.js 14)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen Ejecutivo

| Tipo | Cantidad | Archivos | Cobertura |
|------|----------|----------|-----------|
| Unit Tests | 14 | 7 archivos | 85% |
| Integration Tests | 10 | 5 archivos | 80% |
| **Total** | **24** | **12 archivos** | **82%** |

**Stack de Testing:**
- **Framework:** Vitest (a instalar)
- **Testing Library:** @testing-library/react + @testing-library/user-event
- **Mocking:** MSW (Mock Service Worker) v2 para API
- **Providers:** TanStack Query test utilities
- **Assertion:** vitest expect + @testing-library/jest-dom

---

## 2. Estructura de Tests

```
src/admin/
├── src/
│   ├── __tests__/
│   │   ├── setup.ts                          # Config global Vitest
│   │   ├── test-utils.tsx                    # Render helpers con providers
│   │   └── mocks/
│   │       ├── handlers.ts                   # MSW v2 handlers
│   │       ├── server.ts                     # MSW server setup
│   │       ├── auth.mock.ts                  # Mock data auth
│   │       └── artista.mock.ts               # Mock data artista
│   │
│   ├── app/
│   │   ├── (auth)/
│   │   │   └── register/
│   │   │       ├── page.tsx                  # A CREAR
│   │   │       └── __tests__/
│   │   │           └── register-page.test.tsx
│   │   └── (dashboard)/
│   │       └── artista/
│   │           └── perfil/
│   │               └── crear/
│   │                   ├── page.tsx          # A CREAR
│   │                   └── __tests__/
│   │                       └── crear-perfil-page.test.tsx
│   │
│   ├── components/
│   │   ├── auth/
│   │   │   ├── register-form.tsx             # A CREAR
│   │   │   └── __tests__/
│   │   │       └── register-form.test.tsx
│   │   └── artistas/
│   │       ├── __tests__/
│   │       │   └── artista-form.test.tsx
│   │       └── complete-profile-banner.tsx   # A CREAR (opcional)
│   │
│   ├── hooks/
│   │   ├── use-register.ts                   # A CREAR
│   │   ├── use-artista.ts                    # YA EXISTE
│   │   └── __tests__/
│   │       ├── use-register.test.ts
│   │       └── use-artista.test.ts
│   │
│   ├── services/
│   │   ├── auth.service.ts                   # YA EXISTE
│   │   ├── artista.service.ts                # YA EXISTE
│   │   └── __tests__/
│   │       ├── auth.service.test.ts
│   │       └── artista.service.test.ts
│   │
│   ├── store/
│   │   ├── auth-store.ts                     # YA EXISTE
│   │   └── __tests__/
│   │       └── auth-store.test.ts
│   │
│   └── lib/
│       └── __tests__/
│           └── route-protection.test.tsx
│
├── vitest.config.ts                          # Vitest config
├── vitest.setup.ts                           # Setup global
└── package.json                              # Dependencies actualizadas
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data - Auth

**Archivo:** `src/__tests__/mocks/auth.mock.ts`

```typescript
import type { User, RegisterRequest, RegisterResponse, AuthResponse } from '@shared/types';

export const mockUser: User = {
  id: 'user-123',
  email: 'test@artista.com',
  nombreCompleto: 'Test Artista',
  roles: ['Artista'],
};

export const mockRegisterRequest: RegisterRequest = {
  email: 'nuevo@artista.com',
  password: 'password123',
  confirmPassword: 'password123',
};

export const mockRegisterResponse: RegisterResponse = {
  userId: 'user-456',
  email: 'nuevo@artista.com',
  token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test.token',
};

export const mockAuthResponse: AuthResponse = {
  user: mockUser,
  token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test.token',
};

// Error scenarios
export const mockInvalidCredentials = {
  email: 'wrong@test.com',
  password: 'wrongpass',
};

export const mockExistingEmail = 'existing@test.com';
```

### 3.2 Mock Data - Artista

**Archivo:** `src/__tests__/mocks/artista.mock.ts`

```typescript
import type { ArtistaDto, CreateArtistaDto } from '@shared/types';

export const mockArtistaDto: ArtistaDto = {
  id: 'artista-123',
  userId: 'user-123',
  nombreArtistico: 'Los Rockeros',
  descripcion: 'Banda de rock alternativo de Madrid',
  pais: 'España',
  ciudad: 'Madrid',
  imagenUrl: 'https://example.com/artista.jpg',
  generoMusical: 'Rock',
  fechaCreacion: '2026-02-12T10:00:00Z',
  fechaActualizacion: '2026-02-12T10:00:00Z',
};

export const mockCreateArtistaDto: CreateArtistaDto = {
  nombreArtistico: 'Nuevo Artista',
  descripcion: 'Descripción del artista',
  pais: 'Argentina',
  ciudad: 'Buenos Aires',
  imagenUrl: 'https://example.com/nuevo.jpg',
  generoMusical: 'Pop',
};

export const mockArtistaFormData = {
  nombreArtistico: 'Test Artist',
  descripcion: 'Test description',
  generoMusical: 'Jazz',
  imagenUrl: 'https://example.com/test.jpg',
};

// Artista sin perfil completo (para banner)
export const mockIncompleteArtista: Partial<ArtistaDto> = {
  id: 'artista-incomplete',
  userId: 'user-456',
  nombreArtistico: 'Artista Incompleto',
};

// Error scenarios
export const mockDuplicateArtistaName = 'DuplicateArtist';
```

### 3.3 MSW Handlers (MSW v2)

**Archivo:** `src/__tests__/mocks/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import { mockRegisterResponse, mockUser, mockAuthResponse, mockExistingEmail } from './auth.mock';
import { mockArtistaDto, mockDuplicateArtistaName } from './artista.mock';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export const handlers = [
  // Auth - Register
  http.post(`${API_URL}/auth/register`, async ({ request }) => {
    const body = await request.json() as any;

    // Simulate email exists error
    if (body.email === mockExistingEmail) {
      return HttpResponse.json(
        {
          messages: [
            {
              message: 'Este email ya está registrado',
              errorCode: 'AUTH_EMAIL_EXISTS',
            },
          ],
        },
        { status: 409 }
      );
    }

    // Success
    return HttpResponse.json({
      data: mockRegisterResponse,
      messages: [{ message: 'Usuario registrado exitosamente', errorCode: 'SUCCESS' }],
    });
  }),

  // Auth - Get Current User
  http.get(`${API_URL}/auth/me`, () => {
    return HttpResponse.json({
      data: mockUser,
      messages: [{ message: 'Usuario encontrado', errorCode: 'SUCCESS' }],
    });
  }),

  // Artista - Get My Profile
  http.get(`${API_URL}/artistas/me`, () => {
    return HttpResponse.json({
      data: mockArtistaDto,
      messages: [{ message: 'Artista encontrado', errorCode: 'SUCCESS' }],
    });
  }),

  // Artista - Get My Profile (Not Found)
  http.get(`${API_URL}/artistas/me/notfound`, () => {
    return HttpResponse.json(
      {
        messages: [
          {
            message: 'Artista no encontrado',
            errorCode: 'ARTISTA_NOT_FOUND',
          },
        ],
      },
      { status: 404 }
    );
  }),

  // Artista - Create
  http.post(`${API_URL}/artistas`, async ({ request }) => {
    const body = await request.json() as any;

    // Simulate duplicate error
    if (body.nombreArtistico === mockDuplicateArtistaName) {
      return HttpResponse.json(
        {
          messages: [
            {
              message: 'Este usuario ya tiene un perfil de artista',
              errorCode: 'ARTISTA_ALREADY_EXISTS',
            },
          ],
        },
        { status: 409 }
      );
    }

    // Success
    return HttpResponse.json({
      data: { ...mockArtistaDto, ...body, id: 'artista-new-123' },
      messages: [
        {
          message: 'Perfil de artista creado exitosamente',
          errorCode: 'SUCCESS',
        },
      ],
    });
  }),
];
```

### 3.4 MSW Server Setup

**Archivo:** `src/__tests__/mocks/server.ts`

```typescript
import { setupServer } from 'msw/node';
import { handlers } from './handlers';

export const server = setupServer(...handlers);
```

### 3.5 Test Utilities

**Archivo:** `src/__tests__/test-utils.tsx`

```typescript
import { ReactElement, ReactNode } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from '@/components/ui/sonner';

// Create a custom query client for testing
export const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        gcTime: 0,
        staleTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  });

interface AllProvidersProps {
  children: ReactNode;
}

export function AllProviders({ children }: AllProvidersProps) {
  const queryClient = createTestQueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <Toaster />
    </QueryClientProvider>
  );
}

export function renderWithProviders(
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) {
  return render(ui, { wrapper: AllProviders, ...options });
}

// Mock Next.js router
export const mockRouter = {
  push: vi.fn(),
  replace: vi.fn(),
  pathname: '/',
  query: {},
  asPath: '/',
  back: vi.fn(),
  prefetch: vi.fn().mockResolvedValue(undefined),
  route: '/',
  reload: vi.fn(),
};

export const mockUseRouter = () => mockRouter;

// Re-export everything from @testing-library/react
export * from '@testing-library/react';
```

### 3.6 Setup Global

**Archivo:** `src/__tests__/setup.ts`

```typescript
import '@testing-library/jest-dom';
import { afterAll, afterEach, beforeAll, vi } from 'vitest';
import { server } from './mocks/server';

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
    pathname: '/',
    query: {},
    asPath: '/',
  }),
  usePathname: () => '/',
  useSearchParams: () => new URLSearchParams(),
}));

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
  length: 0,
  key: vi.fn(),
};
global.localStorage = localStorageMock as any;

// Setup MSW
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  vi.clearAllMocks();
  localStorageMock.clear();
});
afterAll(() => server.close());
```

---

## 4. Tests por Modulo

### 4.1 Schemas (Validation) - Shared

#### auth.schema.test.ts

**Archivo:** `src/shared/schemas/__tests__/auth.schema.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| validates correct registration data | Unit | ALTA | Schema acepta datos válidos |
| rejects empty email | Unit | ALTA | Error si email vacío |
| rejects invalid email format | Unit | ALTA | Error si email sin formato válido |
| rejects empty password | Unit | ALTA | Error si password vacío |
| rejects password less than 8 chars | Unit | ALTA | Error si password < 8 caracteres |
| rejects empty confirmPassword | Unit | ALTA | Error si confirmPassword vacío |
| rejects password mismatch | Unit | ALTA | Error si passwords no coinciden |

**Ejemplo de Test:**

```typescript
import { describe, it, expect } from 'vitest';
import { registerSchema } from '@shared/schemas';

describe('registerSchema', () => {
  it('validates correct registration data', () => {
    const validData = {
      email: 'test@example.com',
      password: 'password123',
      confirmPassword: 'password123',
    };

    const result = registerSchema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it('rejects password mismatch', () => {
    const data = {
      email: 'test@example.com',
      password: 'password123',
      confirmPassword: 'different123',
    };

    const result = registerSchema.safeParse(data);
    expect(result.success).toBe(false);
    if (!result.success) {
      const confirmPasswordError = result.error.issues.find(
        i => i.path.includes('confirmPassword')
      );
      expect(confirmPasswordError).toBeDefined();
      expect(confirmPasswordError?.message).toContain('no coinciden');
    }
  });
});
```

#### artista.schema.test.ts

**Archivo:** `src/shared/schemas/__tests__/artista.schema.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| validates minimal required data | Unit | ALTA | Solo nombreArtistico requerido |
| rejects empty nombreArtistico | Unit | ALTA | Error si nombreArtistico vacío |
| rejects nombreArtistico > 200 chars | Unit | ALTA | Error si excede límite |
| rejects descripcion > 2000 chars | Unit | MEDIA | Error si descripción muy larga |
| rejects invalid URL format | Unit | MEDIA | Error si imagenUrl no es URL válida |
| accepts empty optional fields | Unit | MEDIA | Campos opcionales pueden ser vacíos |

---

### 4.2 Services

#### auth.service.test.ts

**Archivo:** `src/services/__tests__/auth.service.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| register success returns token and user | Unit | ALTA | Registro exitoso retorna token y user |
| register with existing email throws error | Unit | ALTA | Error 409 si email ya existe |
| register stores token in localStorage | Unit | ALTA | Token guardado en localStorage |
| register calls getCurrentUser after success | Unit | ALTA | Obtiene user después de registro |
| getCurrentUser success returns user | Unit | MEDIA | Retorna user actual autenticado |
| getCurrentUser returns null on error | Unit | MEDIA | Retorna null si falla |

**Setup:**

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { authService } from '@/services/auth.service';
import { server } from '@/__tests__/mocks/server';
import { http, HttpResponse } from 'msw';

describe('AuthService', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();
  });

  it('register success - returns token and user', async () => {
    const registerData = {
      email: 'nuevo@test.com',
      password: 'password123',
      confirmPassword: 'password123',
    };

    const result = await authService.register(registerData);

    expect(result.token).toBeTruthy();
    expect(result.user).toBeDefined();
    expect(result.user.email).toBe('test@artista.com'); // From getCurrentUser mock
    expect(localStorage.setItem).toHaveBeenCalledWith('token', expect.any(String));
  });

  it('register with existing email - throws error', async () => {
    const registerData = {
      email: 'existing@test.com',
      password: 'password123',
      confirmPassword: 'password123',
    };

    await expect(authService.register(registerData)).rejects.toThrow();
  });
});
```

#### artista.service.test.ts

**Archivo:** `src/services/__tests__/artista.service.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| getMyProfile success returns artista | Unit | ALTA | Retorna perfil del artista autenticado |
| getMyProfile returns null on error | Unit | ALTA | Retorna null si no existe perfil |
| create success returns ArtistaDto | Unit | ALTA | Crea perfil y retorna ArtistaDto |
| create with duplicate error throws | Unit | MEDIA | Error 409 si ya existe perfil |
| update success returns updated artista | Unit | BAJA | Actualiza perfil existente |

---

### 4.3 Hooks

#### use-register.test.ts

**Archivo:** `src/hooks/__tests__/use-register.test.ts`
**Nota:** Este hook debe crearse basado en el patrón de `use-artista.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| mutation success registers user | Integration | ALTA | Registra usuario y retorna datos |
| mutation error handling shows error | Integration | ALTA | Maneja error de API correctamente |
| mutation sets isLoading during submit | Integration | MEDIA | isLoading true durante mutation |
| onSuccess callback redirects to create profile | Integration | ALTA | Redirige tras éxito |

**Setup:**

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { AllProviders } from '@/__tests__/test-utils';
import { useRegister } from '@/hooks/use-register';

describe('useRegister', () => {
  it('mutation success - registers user', async () => {
    const { result } = renderHook(() => useRegister(), {
      wrapper: AllProviders,
    });

    const registerData = {
      email: 'nuevo@test.com',
      password: 'password123',
      confirmPassword: 'password123',
    };

    result.current.mutate(registerData);

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toBeDefined();
    expect(result.current.data?.token).toBeTruthy();
  });

  it('mutation error - handles existing email', async () => {
    const { result } = renderHook(() => useRegister(), {
      wrapper: AllProviders,
    });

    const registerData = {
      email: 'existing@test.com',
      password: 'password123',
      confirmPassword: 'password123',
    };

    result.current.mutate(registerData);

    await waitFor(() => expect(result.current.isError).toBe(true));
    expect(result.current.error).toBeDefined();
  });
});
```

#### use-artista.test.ts

**Archivo:** `src/hooks/__tests__/use-artista.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| useMyArtistProfile fetches data successfully | Integration | ALTA | Query retorna perfil del artista |
| useMyArtistProfile handles loading state | Integration | MEDIA | isLoading true inicialmente |
| useCreateArtista mutation success creates profile | Integration | ALTA | Crea perfil y invalida queries |
| useCreateArtista invalidates cache after create | Integration | ALTA | Invalida query de perfil tras crear |
| useUpdateArtista mutation success updates profile | Integration | MEDIA | Actualiza perfil correctamente |

**Casos Detallados:**

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { useMyArtistProfile, useCreateArtista } from '@/hooks/use-artista';
import { AllProviders } from '@/__tests__/test-utils';
import { mockCreateArtistaDto } from '@/__tests__/mocks/artista.mock';

describe('useArtista hooks', () => {
  it('useMyArtistProfile fetches artista data', async () => {
    const { result } = renderHook(() => useMyArtistProfile(), {
      wrapper: AllProviders,
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toBeDefined();
    expect(result.current.data?.nombreArtistico).toBe('Los Rockeros');
  });

  it('useCreateArtista creates profile and invalidates cache', async () => {
    const { result } = renderHook(() => useCreateArtista(), {
      wrapper: AllProviders,
    });

    result.current.mutate(mockCreateArtistaDto);

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data).toBeDefined();
    expect(result.current.data?.id).toBeTruthy();
  });
});
```

---

### 4.4 Components

#### register-form.test.tsx

**Archivo:** `src/components/auth/__tests__/register-form.test.tsx`
**Nota:** Componente a crear basado en login page

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| renders all form fields | Unit | ALTA | Muestra email, password, confirmPassword |
| displays validation errors on submit | Integration | ALTA | Muestra errores de Zod en UI |
| password mismatch shows error | Integration | ALTA | Error si passwords no coinciden |
| submits valid data | Integration | ALTA | Llama onSubmit con datos correctos |
| disables button when submitting | Unit | MEDIA | Botón disabled durante envío |
| shows loading state during submission | Unit | MEDIA | Texto "Registrando..." durante envío |

**Ejemplo de Test:**

```typescript
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { renderWithProviders } from '@/__tests__/test-utils';
import { RegisterForm } from '@/components/auth/register-form';

describe('RegisterForm', () => {
  it('renders all form fields', () => {
    renderWithProviders(<RegisterForm onSubmit={vi.fn()} />);

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/contraseña/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/confirmar/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /registrarse/i })).toBeInTheDocument();
  });

  it('displays validation errors on submit with invalid data', async () => {
    const user = userEvent.setup();
    renderWithProviders(<RegisterForm onSubmit={vi.fn()} />);

    const submitButton = screen.getByRole('button', { name: /registrarse/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/email es obligatorio/i)).toBeInTheDocument();
      expect(screen.getByText(/contraseña es obligatoria/i)).toBeInTheDocument();
    });
  });

  it('shows error when passwords do not match', async () => {
    const user = userEvent.setup();
    renderWithProviders(<RegisterForm onSubmit={vi.fn()} />);

    await user.type(screen.getByLabelText(/email/i), 'test@example.com');
    await user.type(screen.getByLabelText(/^contraseña$/i), 'password123');
    await user.type(screen.getByLabelText(/confirmar/i), 'differentpass');

    const submitButton = screen.getByRole('button', { name: /registrarse/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/contraseñas no coinciden/i)).toBeInTheDocument();
    });
  });

  it('calls onSubmit with valid data', async () => {
    const user = userEvent.setup();
    const handleSubmit = vi.fn();
    renderWithProviders(<RegisterForm onSubmit={handleSubmit} />);

    await user.type(screen.getByLabelText(/email/i), 'test@example.com');
    await user.type(screen.getByLabelText(/^contraseña$/i), 'password123');
    await user.type(screen.getByLabelText(/confirmar/i), 'password123');

    const submitButton = screen.getByRole('button', { name: /registrarse/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(handleSubmit).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123',
        confirmPassword: 'password123',
      });
    });
  });
});
```

#### artista-form.test.tsx

**Archivo:** `src/components/artistas/__tests__/artista-form.test.tsx`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| renders all form fields | Unit | ALTA | Muestra todos los inputs del formulario |
| displays validation errors | Integration | ALTA | Muestra errores de validación Zod |
| nombreArtistico required error | Integration | ALTA | Error si nombreArtistico vacío |
| imagenUrl invalid format error | Integration | MEDIA | Error si URL inválida |
| submits valid data | Integration | ALTA | Llama onSubmit con datos correctos |
| pre-fills form with defaultValues | Unit | MEDIA | Carga valores iniciales correctamente |
| shows loading state when submitting | Unit | MEDIA | Botón disabled y texto cambia |

**Ejemplo de Test:**

```typescript
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { renderWithProviders } from '@/__tests__/test-utils';
import { ArtistaForm } from '@/components/artistas/artista-form';

describe('ArtistaForm', () => {
  it('displays error when nombreArtistico is empty', async () => {
    const user = userEvent.setup();
    renderWithProviders(<ArtistaForm onSubmit={vi.fn()} />);

    const submitButton = screen.getByRole('button', { name: /crear perfil/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/nombre.*obligatorio/i)).toBeInTheDocument();
    });
  });

  it('submits valid artista data', async () => {
    const user = userEvent.setup();
    const handleSubmit = vi.fn();
    renderWithProviders(<ArtistaForm onSubmit={handleSubmit} />);

    await user.type(screen.getByLabelText(/nombre artistico/i), 'Los Rockeros');
    await user.type(screen.getByLabelText(/biografia/i), 'Una banda genial');
    await user.type(screen.getByLabelText(/imagen/i), 'https://example.com/img.jpg');

    const submitButton = screen.getByRole('button', { name: /crear perfil/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(handleSubmit).toHaveBeenCalledWith(
        expect.objectContaining({
          nombreArtistico: 'Los Rockeros',
          descripcion: 'Una banda genial',
          imagenUrl: 'https://example.com/img.jpg',
        })
      );
    });
  });
});
```

---

### 4.5 Pages (Integration Tests)

#### register-page.test.tsx

**Archivo:** `src/app/(auth)/register/__tests__/register-page.test.tsx`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| renders register form | Integration | ALTA | Página muestra formulario de registro |
| successful registration redirects to create profile | Integration | ALTA | Redirige a /artista/perfil/crear tras éxito |
| shows error toast on failure | Integration | ALTA | Toast error si registro falla |
| stores token after registration | Integration | ALTA | Token guardado en localStorage |
| updates auth store on success | Integration | ALTA | Zustand store actualizado con user/token |
| displays link to login page | Unit | MEDIA | Link "¿Ya tienes cuenta?" visible |

**Flujo Completo:**

```typescript
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { renderWithProviders, mockRouter } from '@/__tests__/test-utils';
import RegisterPage from '@/app/(auth)/register/page';
import { useAuthStore } from '@/store/auth-store';

describe('RegisterPage - Full Flow', () => {
  it('registers user and redirects to create profile', async () => {
    const user = userEvent.setup();
    renderWithProviders(<RegisterPage />);

    // Fill form
    await user.type(screen.getByLabelText(/email/i), 'nuevo@artista.com');
    await user.type(screen.getByLabelText(/^contraseña$/i), 'password123');
    await user.type(screen.getByLabelText(/confirmar/i), 'password123');

    // Submit
    const submitButton = screen.getByRole('button', { name: /registrarse/i });
    await user.click(submitButton);

    // Assertions
    await waitFor(() => {
      expect(localStorage.setItem).toHaveBeenCalledWith('token', expect.any(String));
      expect(mockRouter.push).toHaveBeenCalledWith('/artista/perfil/crear');
    });

    // Verify auth store updated
    const { user: storeUser, isAuthenticated } = useAuthStore.getState();
    expect(isAuthenticated).toBe(true);
    expect(storeUser).toBeDefined();
  });

  it('shows error toast when email already exists', async () => {
    const user = userEvent.setup();
    renderWithProviders(<RegisterPage />);

    await user.type(screen.getByLabelText(/email/i), 'existing@test.com');
    await user.type(screen.getByLabelText(/^contraseña$/i), 'password123');
    await user.type(screen.getByLabelText(/confirmar/i), 'password123');

    const submitButton = screen.getByRole('button', { name: /registrarse/i });
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/email ya está registrado/i)).toBeInTheDocument();
    });
  });
});
```

#### crear-perfil-page.test.tsx

**Archivo:** `src/app/(dashboard)/artista/perfil/crear/__tests__/crear-perfil-page.test.tsx`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| renders create profile form | Integration | ALTA | Página muestra formulario artista |
| creates profile and redirects to dashboard | Integration | ALTA | Redirige a /dashboard tras crear perfil |
| shows success toast on creation | Integration | ALTA | Toast éxito después de crear |
| invalidates artista query on success | Integration | ALTA | Query cache actualizada |
| redirects to login if not authenticated | Integration | ALTA | Redirect si no hay token |
| shows error if profile already exists | Integration | MEDIA | Error 409 si artista ya existe |

**Flujo Completo:**

```typescript
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { renderWithProviders, mockRouter } from '@/__tests__/test-utils';
import CrearPerfilPage from '@/app/(dashboard)/artista/perfil/crear/page';

describe('CrearPerfilPage - Full Flow', () => {
  it('creates artista profile and redirects to dashboard', async () => {
    const user = userEvent.setup();
    renderWithProviders(<CrearPerfilPage />);

    // Fill form
    await user.type(screen.getByLabelText(/nombre artistico/i), 'Los Rockeros');
    await user.type(screen.getByLabelText(/biografia/i), 'Banda de rock');
    await user.type(screen.getByLabelText(/imagen/i), 'https://example.com/band.jpg');

    // Submit
    const submitButton = screen.getByRole('button', { name: /crear perfil/i });
    await user.click(submitButton);

    // Assertions
    await waitFor(() => {
      expect(screen.getByText(/perfil creado exitosamente/i)).toBeInTheDocument();
      expect(mockRouter.push).toHaveBeenCalledWith('/dashboard');
    });
  });
});
```

---

### 4.6 Store

#### auth-store.test.ts

**Archivo:** `src/store/__tests__/auth-store.test.ts`

| Test Case | Tipo | Prioridad | Descripción |
|-----------|------|-----------|-------------|
| initial state is unauthenticated | Unit | ALTA | Estado inicial: user null, isAuthenticated false |
| login updates state and localStorage | Unit | ALTA | login() actualiza user, token, isAuthenticated |
| logout clears state and localStorage | Unit | ALTA | logout() limpia user, token, isAuthenticated |
| persists state across sessions | Unit | MEDIA | Zustand persist recupera estado |

**Ejemplo:**

```typescript
import { renderHook, act } from '@testing-library/react';
import { useAuthStore } from '@/store/auth-store';
import { mockUser } from '@/__tests__/mocks/auth.mock';

describe('AuthStore', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('initial state is unauthenticated', () => {
    const { result } = renderHook(() => useAuthStore());

    expect(result.current.user).toBeNull();
    expect(result.current.token).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('login updates state and localStorage', () => {
    const { result } = renderHook(() => useAuthStore());

    act(() => {
      result.current.login(mockUser, 'test-token-123');
    });

    expect(result.current.user).toEqual(mockUser);
    expect(result.current.token).toBe('test-token-123');
    expect(result.current.isAuthenticated).toBe(true);
    expect(localStorage.setItem).toHaveBeenCalledWith('token', 'test-token-123');
  });

  it('logout clears state and localStorage', () => {
    const { result } = renderHook(() => useAuthStore());

    // Setup: login first
    act(() => {
      result.current.login(mockUser, 'test-token-123');
    });

    // Logout
    act(() => {
      result.current.logout();
    });

    expect(result.current.user).toBeNull();
    expect(result.current.token).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
    expect(localStorage.removeItem).toHaveBeenCalledWith('token');
  });
});
```

---

## 5. Cobertura por Archivo

| Archivo | Líneas | Funciones | Branches | Prioridad |
|---------|--------|-----------|----------|-----------|
| `auth.schema.ts` | 90% | 100% | 85% | ALTA |
| `artista.schema.ts` | 90% | 100% | 85% | ALTA |
| `auth.service.ts` | 85% | 95% | 80% | ALTA |
| `artista.service.ts` | 85% | 95% | 80% | ALTA |
| `use-register.ts` | 80% | 90% | 75% | ALTA |
| `use-artista.ts` | 85% | 90% | 80% | ALTA |
| `register-form.tsx` | 80% | 85% | 75% | ALTA |
| `artista-form.tsx` | 85% | 85% | 75% | MEDIA |
| `register/page.tsx` | 75% | 80% | 70% | ALTA |
| `crear/page.tsx` | 75% | 80% | 70% | ALTA |
| `auth-store.ts` | 90% | 100% | 85% | ALTA |
| **PROMEDIO** | **82%** | **89%** | **78%** | - |

**Meta Global:** 80%+ en todas las métricas

---

## 6. Configuración de Testing

### 6.1 Vitest Config

**Archivo:** `vitest.config.ts`

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/__tests__/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: [
        'node_modules/',
        'src/__tests__/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
        'src/components/ui/**', // shadcn components (already tested)
      ],
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 75,
        statements: 80,
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@shared': path.resolve(__dirname, '../shared'),
    },
  },
});
```

### 6.2 Package.json Updates

**Agregar al `package.json`:**

```json
{
  "scripts": {
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest --coverage",
    "test:watch": "vitest --watch",
    "test:run": "vitest run"
  },
  "devDependencies": {
    "@testing-library/jest-dom": "^6.1.5",
    "@testing-library/react": "^14.1.2",
    "@testing-library/user-event": "^14.5.1",
    "@vitejs/plugin-react": "^4.2.1",
    "@vitest/ui": "^1.0.4",
    "jsdom": "^23.0.1",
    "msw": "^2.0.11",
    "vitest": "^1.0.4",
    "@vitest/coverage-v8": "^1.0.4"
  }
}
```

---

## 7. Estrategia de Ejecución

### 7.1 Comandos

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con UI interactiva
npm run test:ui

# Ejecutar con coverage
npm run test:coverage

# Watch mode (desarrollo)
npm run test:watch

# Run once (CI)
npm run test:run

# Filtrar por feature
npm run test -- --filter=auth
npm run test -- --filter=artista
```

### 7.2 CI/CD Integration

**GitHub Actions Workflow (ejemplo):**

```yaml
name: Frontend Tests - Admin

on:
  push:
    paths:
      - 'src/admin/**'
      - 'src/shared/**'
  pull_request:
    paths:
      - 'src/admin/**'
      - 'src/shared/**'

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install dependencies
        working-directory: ./src/admin
        run: npm ci

      - name: Run tests with coverage
        working-directory: ./src/admin
        run: npm run test:coverage

      - name: Upload coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./src/admin/coverage/lcov.info
          flags: admin-frontend
```

---

## 8. Priorización de Tests

### Fase 1 - Tests Críticos (ALTA prioridad)

**Tiempo estimado: 4-5 horas**

1. **Schemas** (30 min)
   - `registerSchema` validaciones
   - `createArtistaSchema` validaciones

2. **Services** (1h)
   - `auth.service.test.ts` - register, getCurrentUser
   - `artista.service.test.ts` - create, getMyProfile

3. **Hooks** (1h)
   - `use-register.test.ts` - mutation success/error
   - `use-artista.test.ts` - create mutation

4. **Pages Integration** (2h)
   - `register-page.test.tsx` - flujo completo registro
   - `crear-perfil-page.test.tsx` - flujo crear perfil

### Fase 2 - Tests de Componentes (MEDIA prioridad)

**Tiempo estimado: 3-4 horas**

5. **Forms** (2h)
   - `register-form.test.tsx`
   - `artista-form.test.tsx`

6. **Store** (1h)
   - `auth-store.test.ts`

### Fase 3 - Tests Complementarios (BAJA prioridad)

**Tiempo estimado: 2 horas**

7. **Edge Cases** (2h)
   - Error scenarios
   - Token expiration
   - Network failures

---

## 9. Checklist de Implementación

### Pre-requisitos

- [ ] Instalar dependencias de testing (vitest, testing-library, msw)
- [ ] Configurar `vitest.config.ts`
- [ ] Crear estructura de carpetas `__tests__/`
- [ ] Setup global en `setup.ts`
- [ ] Crear test-utils.tsx con providers

### Mocks

- [ ] Crear `auth.mock.ts` con datos de registro/login
- [ ] Crear `artista.mock.ts` con datos de perfil
- [ ] Implementar MSW handlers en `handlers.ts`
- [ ] Configurar MSW server en `server.ts`

### Tests Críticos (Fase 1)

- [ ] Schemas: `auth.schema.test.ts`
- [ ] Schemas: `artista.schema.test.ts`
- [ ] Services: `auth.service.test.ts`
- [ ] Services: `artista.service.test.ts`
- [ ] Hooks: `use-register.test.ts` (crear hook primero)
- [ ] Hooks: `use-artista.test.ts`
- [ ] Pages: `register-page.test.tsx`
- [ ] Pages: `crear-perfil-page.test.tsx`

### Tests Secundarios (Fase 2)

- [ ] Components: `register-form.test.tsx` (crear componente)
- [ ] Components: `artista-form.test.tsx`
- [ ] Store: `auth-store.test.ts`

### CI/CD

- [ ] Configurar GitHub Actions workflow
- [ ] Integrar coverage reports
- [ ] Establecer umbrales de cobertura mínimos

---

## 10. Componentes a Crear

Los siguientes componentes NO existen actualmente y deben crearse antes de los tests:

1. **RegisterForm.tsx** - Formulario de registro basado en LoginPage
2. **RegisterPage** (`/auth/register/page.tsx`) - Página completa de registro
3. **CrearPerfilPage** (`/artista/perfil/crear/page.tsx`) - Página crear perfil
4. **useRegister.ts** - Hook custom para mutation de registro

### Componentes Existentes

- `ArtistaForm` - Ya existe en `components/artistas/artista-form.tsx`
- `useMyArtistProfile`, `useCreateArtista` - Ya existen en `hooks/use-artista.ts`
- `authService` - Ya implementado con `register()`, `login()`, `getCurrentUser()`
- `artistaService` - Ya implementado con `create()`, `getMyProfile()`
- `useAuthStore` - Ya existe en `store/auth-store.ts`

---

## 11. Referencias

### Documentación

- [Vitest Documentation](https://vitest.dev/)
- [Testing Library React](https://testing-library.com/react)
- [MSW v2 Documentation](https://mswjs.io/)
- [TanStack Query Testing](https://tanstack.com/query/latest/docs/react/guides/testing)

### Contratos API

- Ver: `docs/user-stories/registro-artista/contracts.md`

### Reglas de Testing

- Ver: `.claude/rules/testing/unit-tests.rule.md`

---

**Resumen Final:**

- **Total de Tests:** 24 tests distribuidos en 12 archivos
- **Tiempo Estimado:** 9-11 horas de desarrollo
- **Cobertura Esperada:** 82% líneas, 89% funciones, 78% branches
- **Framework:** Vitest + Testing Library + MSW v2
- **Prioridad:** Tests críticos de flujo completo (registro → perfil → dashboard)

**Siguiente Paso:** Instalar dependencias y comenzar con Fase 1 (tests críticos).
