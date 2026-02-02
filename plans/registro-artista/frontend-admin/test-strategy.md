# Estrategia de Testing: Registro de Artista (Admin)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/admin
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 12 | 70% |
| Integration Tests | 4 | 90% |
| Total | 16 | 80%+ |

## 2. Estructura de Tests

```
src/admin/src/
├── __tests__/
│   ├── components/
│   │   ├── auth/
│   │   │   ├── RegisterForm.test.tsx
│   │   │   └── PasswordInput.test.tsx
│   │   └── artista/
│   │       ├── CreateArtistaForm.test.tsx
│   │       └── ImagePreview.test.tsx
│   ├── hooks/
│   │   ├── useRegister.test.ts
│   │   └── useCreateArtista.test.ts
│   └── services/
│       ├── auth.service.test.ts
│       └── artista.service.test.ts
├── __mocks__/
│   ├── auth.mock.ts
│   ├── artista.mock.ts
│   ├── handlers.ts           # MSW handlers
│   └── router.mock.ts        # Next.js router mock
└── test-utils.tsx            # Query client wrapper
```

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/auth.mock.ts`

```typescript
import { RegisterRequest, RegisterResponse } from '@/shared/types/auth';
import { RegisterFormData } from '@/shared/schemas/auth.schema';

export const mockRegisterRequest: RegisterRequest = {
  email: 'artista@example.com',
  password: 'password123',
  confirmPassword: 'password123',
};

export const mockRegisterFormData: RegisterFormData = {
  email: 'artista@example.com',
  password: 'password123',
  confirmPassword: 'password123',
};

export const mockRegisterResponse: RegisterResponse = {
  userId: '123e4567-e89b-12d3-a456-426614174000',
  email: 'artista@example.com',
  token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.mock.token',
};

export const mockRegisterErrorEmailExists = {
  message: 'Este email ya está registrado. ¿Quieres iniciar sesión?',
  errorCode: 'AUTH_EMAIL_EXISTS',
};

export const mockRegisterErrorInvalidEmail = {
  message: 'El formato del email no es válido',
  errorCode: 'AUTH_EMAIL_INVALID',
};

export const mockRegisterErrorPasswordMismatch = {
  message: 'Las contraseñas no coinciden',
  errorCode: 'AUTH_PASSWORD_MISMATCH',
};
```

**Archivo:** `__mocks__/artista.mock.ts`

```typescript
import { CreateArtistaRequest } from '@/shared/types/auth';
import { Artista } from '@/shared/types/artista';
import { CreateArtistaFormData } from '@/shared/schemas/artista.schema';

export const mockCreateArtistaRequest: CreateArtistaRequest = {
  nombreArtistico: 'El Artista Test',
  descripcion: 'Descripcion de prueba para el artista',
  pais: 'España',
  ciudad: 'Madrid',
  imagenUrl: 'https://example.com/avatar.jpg',
};

export const mockCreateArtistaFormData: CreateArtistaFormData = {
  nombreArtistico: 'El Artista Test',
  descripcion: 'Descripcion de prueba para el artista',
  pais: 'España',
  ciudad: 'Madrid',
  imagenUrl: 'https://example.com/avatar.jpg',
};

export const mockArtista: Artista = {
  id: 'art-123e4567-e89b-12d3-a456-426614174000',
  userId: '123e4567-e89b-12d3-a456-426614174000',
  nombreArtistico: 'El Artista Test',
  descripcion: 'Descripcion de prueba para el artista',
  pais: 'España',
  ciudad: 'Madrid',
  imagenUrl: 'https://example.com/avatar.jpg',
  fechaCreacion: '2026-01-26T10:00:00Z',
  fechaActualizacion: '2026-01-26T10:00:00Z',
};

export const mockCreateArtistaErrorNombreRequerido = {
  message: 'El nombre artístico es obligatorio',
  errorCode: 'ARTISTA_NOMBRE_REQUERIDO',
};

export const mockCreateArtistaErrorAlreadyExists = {
  message: 'Ya tienes un perfil de artista creado',
  errorCode: 'ARTISTA_ALREADY_EXISTS',
};

export const mockCreateArtistaErrorImagenInvalida = {
  message: 'La URL de la imagen no es válida. Debe comenzar con http:// o https://',
  errorCode: 'ARTISTA_IMAGEN_URL_INVALIDA',
};
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from 'msw';
import { API_ROUTES } from '@/shared/constants/api-routes';
import {
  mockRegisterResponse,
  mockRegisterErrorEmailExists,
} from './auth.mock';
import {
  mockArtista,
  mockCreateArtistaErrorAlreadyExists,
} from './artista.mock';

export const authHandlers = [
  // Register success
  rest.post(API_ROUTES.auth.register, async (req, res, ctx) => {
    const body = await req.json();

    if (body.email === 'existe@example.com') {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [mockRegisterErrorEmailExists],
          isSuccess: false,
        })
      );
    }

    return res(
      ctx.status(201),
      ctx.json({
        data: mockRegisterResponse,
        messages: [{ message: 'Registro exitoso', errorCode: 'SUCCESS' }],
        isSuccess: true,
      })
    );
  }),
];

export const artistaHandlers = [
  // Create artista success
  rest.post(API_ROUTES.artistas.base, async (req, res, ctx) => {
    const body = await req.json();
    const token = req.headers.get('Authorization');

    if (!token) {
      return res(
        ctx.status(401),
        ctx.json({
          data: null,
          messages: [{ message: 'No autorizado', errorCode: 'AUTH_UNAUTHORIZED' }],
          isSuccess: false,
        })
      );
    }

    if (body.nombreArtistico === 'Artista Duplicado') {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [mockCreateArtistaErrorAlreadyExists],
          isSuccess: false,
        })
      );
    }

    return res(
      ctx.status(201),
      ctx.json({
        data: mockArtista.id,
        messages: [{ message: 'Perfil creado exitosamente', errorCode: 'SUCCESS' }],
        isSuccess: true,
      })
    );
  }),

  // Get artista by ID
  rest.get(API_ROUTES.artistas.byId(':id'), (req, res, ctx) => {
    return res(
      ctx.status(200),
      ctx.json({
        data: mockArtista,
        messages: [],
        isSuccess: true,
      })
    );
  }),
];

export const handlers = [...authHandlers, ...artistaHandlers];
```

### 3.3 Router Mock (Next.js)

**Archivo:** `__mocks__/router.mock.ts`

```typescript
import { vi } from 'vitest';

export const mockPush = vi.fn();
export const mockReplace = vi.fn();
export const mockBack = vi.fn();

export const createMockRouter = () => ({
  push: mockPush,
  replace: mockReplace,
  back: mockBack,
  pathname: '/',
  route: '/',
  query: {},
  asPath: '/',
  events: {
    on: vi.fn(),
    off: vi.fn(),
    emit: vi.fn(),
  },
  beforePopState: vi.fn(() => null),
  prefetch: vi.fn(() => Promise.resolve()),
  isReady: true,
  isFallback: false,
  isLocaleDomain: false,
  isPreview: false,
});

export const mockUseRouter = () => createMockRouter();
```

### 3.4 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import { ReactElement, ReactNode } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { RouterContext } from 'next/dist/shared/lib/router-context';
import { createMockRouter } from './__mocks__/router.mock';

const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        cacheTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
  });

interface AllProvidersProps {
  children: ReactNode;
}

const AllProviders = ({ children }: AllProvidersProps) => {
  const testQueryClient = createTestQueryClient();
  const mockRouter = createMockRouter();

  return (
    <QueryClientProvider client={testQueryClient}>
      <RouterContext.Provider value={mockRouter}>
        {children}
      </RouterContext.Provider>
    </QueryClientProvider>
  );
};

const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllProviders, ...options });

export * from '@testing-library/react';
export { customRender as render };
```

## 4. Tests por Modulo

### 4.1 Components - Auth

#### RegisterForm.test.tsx

**Archivo:** `__tests__/components/auth/RegisterForm.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders all fields correctly | Unit | Renderiza email, password, confirmPassword, submit button |
| displays validation errors on blur | Unit | Muestra error si email invalido al perder foco |
| shows password mismatch error | Unit | Muestra error si passwords no coinciden |
| shows password min length error | Unit | Muestra error si password < 8 chars |
| disables submit when form invalid | Unit | Boton submit deshabilitado si form invalido |
| calls onSubmit with form data | Integration | Llama onSubmit con RegisterFormData al enviar |
| shows loading state during submission | Integration | Muestra spinner y deshabilita boton durante submit |
| displays API error message | Integration | Muestra error de API (email exists) |
| redirects to create profile on success | Integration | Redirige a /artista/perfil/crear tras registro exitoso |

**Casos Detallados:**

```markdown
1. **renders all fields correctly**
   - Render <RegisterForm />
   - Assert: input email visible
   - Assert: input password visible
   - Assert: input confirmPassword visible
   - Assert: boton "Registrarse" visible

2. **displays validation errors on blur**
   - Render <RegisterForm />
   - Type invalid email "notanemail"
   - Blur email input
   - Assert: mensaje "Formato de email invalido" visible

3. **shows password mismatch error**
   - Render <RegisterForm />
   - Type password "password123"
   - Type confirmPassword "password456"
   - Blur confirmPassword input
   - Assert: mensaje "Las contraseñas no coinciden" visible

4. **shows password min length error**
   - Render <RegisterForm />
   - Type password "pass" (< 8 chars)
   - Blur password input
   - Assert: mensaje "La contraseña debe tener al menos 8 caracteres" visible

5. **disables submit when form invalid**
   - Render <RegisterForm />
   - Assert: boton submit deshabilitado inicialmente
   - Type invalid data
   - Assert: boton submit sigue deshabilitado

6. **calls onSubmit with form data**
   - Render <RegisterForm onSubmit={mockOnSubmit} />
   - Fill email "artista@example.com"
   - Fill password "password123"
   - Fill confirmPassword "password123"
   - Click submit button
   - Assert: mockOnSubmit llamado con { email, password, confirmPassword }

7. **shows loading state during submission**
   - Render <RegisterForm /> con useRegister mock
   - Setup mock mutation con isPending: true
   - Fill form data
   - Click submit
   - Assert: boton muestra spinner
   - Assert: boton deshabilitado

8. **displays API error message**
   - Render <RegisterForm />
   - Setup MSW handler para retornar AUTH_EMAIL_EXISTS
   - Fill email "existe@example.com"
   - Fill passwords
   - Click submit
   - Wait for error message
   - Assert: mensaje "Este email ya está registrado. ¿Quieres iniciar sesión?" visible

9. **redirects to create profile on success**
   - Render <RegisterForm />
   - Setup MSW handler para registro exitoso
   - Fill form data valido
   - Click submit
   - Wait for navigation
   - Assert: router.push llamado con "/artista/perfil/crear"
```

#### PasswordInput.test.tsx

**Archivo:** `__tests__/components/auth/PasswordInput.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with type password | Unit | Renderiza input con type="password" por defecto |
| toggles visibility on icon click | Unit | Cambia type a "text" al hacer click en icono ojo |
| displays password when visible | Unit | Muestra password en texto plano cuando visible=true |
| hides password when not visible | Unit | Muestra password oculto cuando visible=false |

**Casos Detallados:**

```markdown
1. **renders with type password**
   - Render <PasswordInput value="" onChange={vi.fn()} />
   - Get input element
   - Assert: input.type === "password"

2. **toggles visibility on icon click**
   - Render <PasswordInput value="test123" onChange={vi.fn()} />
   - Assert: input.type === "password"
   - Click toggle icon button
   - Assert: input.type === "text"
   - Click toggle icon button again
   - Assert: input.type === "password"

3. **displays password when visible**
   - Render <PasswordInput value="password123" onChange={vi.fn()} />
   - Click toggle icon (mostrar)
   - Assert: value "password123" visible como texto

4. **hides password when not visible**
   - Render <PasswordInput value="password123" onChange={vi.fn()} />
   - Assert: value oculto (bullets/asteriscos)
```

### 4.2 Components - Artista

#### CreateArtistaForm.test.tsx

**Archivo:** `__tests__/components/artista/CreateArtistaForm.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders all fields correctly | Unit | Renderiza nombreArtistico, descripcion, pais, ciudad, imagenUrl |
| marks nombreArtistico as required | Unit | Muestra asterisco o indicador de campo obligatorio |
| displays character counter for descripcion | Unit | Muestra "X/2000 caracteres" |
| updates character counter on input | Unit | Counter actualiza al escribir en descripcion |
| shows validation error for empty nombreArtistico | Unit | Muestra error si nombreArtistico vacio al submit |
| shows validation error for nombreArtistico too long | Unit | Muestra error si nombreArtistico > 200 chars |
| shows validation error for descripcion too long | Unit | Muestra error si descripcion > 2000 chars |
| shows validation error for invalid imagenUrl | Unit | Muestra error si imagenUrl no es URL valida |
| loads image preview when valid URL | Unit | Muestra preview de imagen si imagenUrl valida |
| calls onSubmit with form data | Integration | Llama onSubmit con CreateArtistaFormData |
| shows loading state during submission | Integration | Muestra spinner durante submit |
| displays API error message | Integration | Muestra error de API (ARTISTA_ALREADY_EXISTS) |
| redirects to dashboard on success | Integration | Redirige a /dashboard tras crear perfil |

**Casos Detallados:**

```markdown
1. **renders all fields correctly**
   - Render <CreateArtistaForm />
   - Assert: input nombreArtistico visible
   - Assert: textarea descripcion visible
   - Assert: input pais visible
   - Assert: input ciudad visible
   - Assert: input imagenUrl visible
   - Assert: boton "Crear Perfil" visible

2. **marks nombreArtistico as required**
   - Render <CreateArtistaForm />
   - Get label for nombreArtistico
   - Assert: label contiene "*" o aria-required="true"

3. **displays character counter for descripcion**
   - Render <CreateArtistaForm />
   - Assert: texto "0/2000 caracteres" visible

4. **updates character counter on input**
   - Render <CreateArtistaForm />
   - Type "Hola mundo" en descripcion (10 chars)
   - Assert: texto "10/2000 caracteres" visible

5. **shows validation error for empty nombreArtistico**
   - Render <CreateArtistaForm />
   - Leave nombreArtistico empty
   - Click submit
   - Assert: mensaje "El nombre artístico es obligatorio" visible

6. **shows validation error for nombreArtistico too long**
   - Render <CreateArtistaForm />
   - Type 201 caracteres en nombreArtistico
   - Blur input
   - Assert: mensaje "El nombre artístico no puede superar los 200 caracteres" visible

7. **shows validation error for descripcion too long**
   - Render <CreateArtistaForm />
   - Type 2001 caracteres en descripcion
   - Blur textarea
   - Assert: mensaje "La descripción no puede superar los 2000 caracteres" visible

8. **shows validation error for invalid imagenUrl**
   - Render <CreateArtistaForm />
   - Type "not-a-url" en imagenUrl
   - Blur input
   - Assert: mensaje "Debe ser una URL válida" visible

9. **loads image preview when valid URL**
   - Render <CreateArtistaForm />
   - Type "https://example.com/avatar.jpg" en imagenUrl
   - Wait for image load
   - Assert: <img> con src="https://example.com/avatar.jpg" visible

10. **calls onSubmit with form data**
    - Render <CreateArtistaForm onSubmit={mockOnSubmit} />
    - Fill nombreArtistico "El Artista"
    - Fill descripcion "Descripcion test"
    - Fill pais "España"
    - Fill ciudad "Madrid"
    - Fill imagenUrl "https://example.com/avatar.jpg"
    - Click submit
    - Assert: mockOnSubmit llamado con CreateArtistaFormData

11. **shows loading state during submission**
    - Render <CreateArtistaForm /> con useCreateArtista mock
    - Setup mock mutation con isPending: true
    - Fill form data
    - Click submit
    - Assert: boton muestra spinner
    - Assert: boton deshabilitado

12. **displays API error message**
    - Render <CreateArtistaForm />
    - Setup MSW handler para retornar ARTISTA_ALREADY_EXISTS
    - Fill nombreArtistico "Artista Duplicado"
    - Fill resto de campos
    - Click submit
    - Wait for error message
    - Assert: mensaje "Ya tienes un perfil de artista creado" visible

13. **redirects to dashboard on success**
    - Render <CreateArtistaForm />
    - Setup MSW handler para creacion exitosa
    - Fill form data valido
    - Click submit
    - Wait for navigation
    - Assert: router.push llamado con "/dashboard"
```

#### ImagePreview.test.tsx

**Archivo:** `__tests__/components/artista/ImagePreview.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders placeholder when no URL | Unit | Muestra placeholder si imagenUrl vacio |
| displays image when valid URL | Unit | Muestra <img> con src si URL valido |
| shows loading state while image loads | Unit | Muestra skeleton/spinner mientras carga |
| shows error state on load failure | Unit | Muestra mensaje error si imagen falla al cargar |
| applies correct alt text | Unit | <img> tiene alt descriptivo |

**Casos Detallados:**

```markdown
1. **renders placeholder when no URL**
   - Render <ImagePreview imagenUrl="" />
   - Assert: placeholder icon/texto visible
   - Assert: no <img> en documento

2. **displays image when valid URL**
   - Render <ImagePreview imagenUrl="https://example.com/avatar.jpg" />
   - Wait for image load
   - Assert: <img src="https://example.com/avatar.jpg"> visible

3. **shows loading state while image loads**
   - Render <ImagePreview imagenUrl="https://example.com/slow-image.jpg" />
   - Assert: skeleton/spinner visible
   - Wait for image load
   - Assert: skeleton/spinner ya no visible

4. **shows error state on load failure**
   - Render <ImagePreview imagenUrl="https://example.com/not-found.jpg" />
   - Trigger img onError event
   - Assert: mensaje "Error al cargar imagen" visible

5. **applies correct alt text**
   - Render <ImagePreview imagenUrl="https://example.com/avatar.jpg" alt="Foto de perfil" />
   - Get img element
   - Assert: img.alt === "Foto de perfil"
```

### 4.3 Hooks

#### useRegister.test.ts

**Archivo:** `__tests__/hooks/useRegister.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns mutation object | Unit | Retorna { mutate, isPending, isError, isSuccess } |
| calls auth.register with correct data | Unit | Llama auth.service.register con RegisterRequest |
| sets isPending to true during mutation | Unit | isPending=true mientras ejecuta |
| stores token in localStorage on success | Integration | Guarda token en localStorage tras registro exitoso |
| invalidates auth queries on success | Integration | Invalida queryClient.invalidateQueries(['auth']) |
| sets isError to true on failure | Unit | isError=true si API retorna error |
| returns error message on failure | Unit | error.message contiene mensaje de API |

**Setup:**
```typescript
const wrapper = ({ children }) => (
  <QueryClientProvider client={testQueryClient}>
    {children}
  </QueryClientProvider>
);

const { result } = renderHook(() => useRegister(), { wrapper });
```

**Casos Detallados:**

```markdown
1. **returns mutation object**
   - renderHook(() => useRegister())
   - Assert: result.current.mutate es funcion
   - Assert: result.current.isPending es boolean
   - Assert: result.current.isError es boolean
   - Assert: result.current.isSuccess es boolean

2. **calls auth.register with correct data**
   - Mock auth.service.register
   - renderHook(() => useRegister())
   - Act: result.current.mutate(mockRegisterRequest)
   - Assert: auth.service.register llamado con mockRegisterRequest

3. **sets isPending to true during mutation**
   - renderHook(() => useRegister())
   - Assert: result.current.isPending === false
   - Act: result.current.mutate(mockRegisterRequest)
   - Assert: result.current.isPending === true (inmediatamente)
   - Wait for mutation complete
   - Assert: result.current.isPending === false

4. **stores token in localStorage on success**
   - Setup MSW handler para registro exitoso
   - renderHook(() => useRegister())
   - Act: result.current.mutate(mockRegisterRequest)
   - Wait for success
   - Assert: localStorage.getItem('token') === mockRegisterResponse.token

5. **invalidates auth queries on success**
   - Mock queryClient.invalidateQueries
   - renderHook(() => useRegister())
   - Act: result.current.mutate(mockRegisterRequest)
   - Wait for success
   - Assert: queryClient.invalidateQueries llamado con { queryKey: ['auth'] }

6. **sets isError to true on failure**
   - Setup MSW handler para retornar error 400
   - renderHook(() => useRegister())
   - Act: result.current.mutate({ email: 'existe@example.com', ... })
   - Wait for error
   - Assert: result.current.isError === true

7. **returns error message on failure**
   - Setup MSW handler para retornar AUTH_EMAIL_EXISTS
   - renderHook(() => useRegister())
   - Act: result.current.mutate({ email: 'existe@example.com', ... })
   - Wait for error
   - Assert: result.current.error.message === 'Este email ya está registrado. ¿Quieres iniciar sesión?'
```

#### useCreateArtista.test.ts

**Archivo:** `__tests__/hooks/useCreateArtista.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns mutation object | Unit | Retorna { mutate, isPending, isError, isSuccess } |
| calls artista.create with correct data | Unit | Llama artista.service.create con CreateArtistaRequest |
| sends JWT token in Authorization header | Unit | Incluye Bearer token en headers |
| sets isPending to true during mutation | Unit | isPending=true mientras ejecuta |
| invalidates artista queries on success | Integration | Invalida queries de artistas tras crear |
| returns artista ID on success | Unit | data contiene ID del artista creado |
| sets isError to true on failure | Unit | isError=true si API retorna error |
| returns error message on failure | Unit | error.message contiene mensaje de API |

**Setup:**
```typescript
const wrapper = ({ children }) => (
  <QueryClientProvider client={testQueryClient}>
    {children}
  </QueryClientProvider>
);

// Mock localStorage con token
beforeEach(() => {
  localStorage.setItem('token', mockRegisterResponse.token);
});

const { result } = renderHook(() => useCreateArtista(), { wrapper });
```

**Casos Detallados:**

```markdown
1. **returns mutation object**
   - renderHook(() => useCreateArtista())
   - Assert: result.current.mutate es funcion
   - Assert: result.current.isPending es boolean
   - Assert: result.current.isError es boolean
   - Assert: result.current.isSuccess es boolean

2. **calls artista.create with correct data**
   - Mock artista.service.create
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate(mockCreateArtistaRequest)
   - Assert: artista.service.create llamado con mockCreateArtistaRequest

3. **sends JWT token in Authorization header**
   - Mock axios/fetch
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate(mockCreateArtistaRequest)
   - Assert: request headers contiene "Authorization: Bearer <token>"

4. **sets isPending to true during mutation**
   - renderHook(() => useCreateArtista())
   - Assert: result.current.isPending === false
   - Act: result.current.mutate(mockCreateArtistaRequest)
   - Assert: result.current.isPending === true (inmediatamente)
   - Wait for mutation complete
   - Assert: result.current.isPending === false

5. **invalidates artista queries on success**
   - Mock queryClient.invalidateQueries
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate(mockCreateArtistaRequest)
   - Wait for success
   - Assert: queryClient.invalidateQueries llamado con { queryKey: QUERY_KEYS.artistas.all }

6. **returns artista ID on success**
   - Setup MSW handler para creacion exitosa
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate(mockCreateArtistaRequest)
   - Wait for success
   - Assert: result.current.data === mockArtista.id

7. **sets isError to true on failure**
   - Setup MSW handler para retornar error 400
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate({ nombreArtistico: 'Artista Duplicado', ... })
   - Wait for error
   - Assert: result.current.isError === true

8. **returns error message on failure**
   - Setup MSW handler para retornar ARTISTA_ALREADY_EXISTS
   - renderHook(() => useCreateArtista())
   - Act: result.current.mutate({ nombreArtistico: 'Artista Duplicado', ... })
   - Wait for error
   - Assert: result.current.error.message === 'Ya tienes un perfil de artista creado'
```

### 4.4 Services

#### auth.service.test.ts

**Archivo:** `__tests__/services/auth.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| register returns RegisterResponse | Unit | Retorna { userId, email, token } |
| register sends POST to correct endpoint | Unit | POST a /api/auth/register |
| register sends data in request body | Unit | Body contiene { email, password, confirmPassword } |
| register throws on API error | Unit | Lanza error si status 400/500 |

**Setup:**
```typescript
import { server } from '../__mocks__/server';
import { authHandlers } from '../__mocks__/handlers';

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

**Casos Detallados:**

```markdown
1. **register returns RegisterResponse**
   - Call authService.register(mockRegisterRequest)
   - Wait for response
   - Assert: response.userId === mockRegisterResponse.userId
   - Assert: response.email === mockRegisterResponse.email
   - Assert: response.token === mockRegisterResponse.token

2. **register sends POST to correct endpoint**
   - Mock fetch/axios
   - Call authService.register(mockRegisterRequest)
   - Assert: POST enviado a "/api/auth/register"

3. **register sends data in request body**
   - Mock fetch/axios
   - Call authService.register(mockRegisterRequest)
   - Assert: body contiene { email, password, confirmPassword }

4. **register throws on API error**
   - Setup MSW handler para retornar 400
   - Call authService.register({ email: 'existe@example.com', ... })
   - Assert: throws error con message 'Este email ya está registrado'
```

#### artista.service.test.ts

**Archivo:** `__tests__/services/artista.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| create returns artista ID | Unit | Retorna string ID del artista creado |
| create sends POST to correct endpoint | Unit | POST a /api/artistas |
| create includes Authorization header | Unit | Header "Authorization: Bearer <token>" |
| create sends data in request body | Unit | Body contiene CreateArtistaRequest |
| create throws on API error | Unit | Lanza error si status 400/500 |
| create throws on missing token | Unit | Lanza error si no hay token en localStorage |

**Setup:**
```typescript
import { server } from '../__mocks__/server';
import { artistaHandlers } from '../__mocks__/handlers';

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

beforeEach(() => {
  localStorage.setItem('token', mockRegisterResponse.token);
});

afterEach(() => {
  localStorage.clear();
});
```

**Casos Detallados:**

```markdown
1. **create returns artista ID**
   - Call artistaService.create(mockCreateArtistaRequest)
   - Wait for response
   - Assert: response === mockArtista.id

2. **create sends POST to correct endpoint**
   - Mock fetch/axios
   - Call artistaService.create(mockCreateArtistaRequest)
   - Assert: POST enviado a "/api/artistas"

3. **create includes Authorization header**
   - Mock fetch/axios
   - Call artistaService.create(mockCreateArtistaRequest)
   - Assert: headers contiene "Authorization: Bearer <token>"

4. **create sends data in request body**
   - Mock fetch/axios
   - Call artistaService.create(mockCreateArtistaRequest)
   - Assert: body contiene { nombreArtistico, descripcion, pais, ciudad, imagenUrl }

5. **create throws on API error**
   - Setup MSW handler para retornar 400
   - Call artistaService.create({ nombreArtistico: 'Artista Duplicado', ... })
   - Assert: throws error con message 'Ya tienes un perfil de artista creado'

6. **create throws on missing token**
   - Clear localStorage
   - Call artistaService.create(mockCreateArtistaRequest)
   - Assert: throws error con message 'No autorizado'
```

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| RegisterForm.tsx | 85% | 90% | 80% |
| PasswordInput.tsx | 95% | 100% | 90% |
| CreateArtistaForm.tsx | 85% | 90% | 80% |
| ImagePreview.tsx | 90% | 95% | 85% |
| useRegister.ts | 90% | 100% | 85% |
| useCreateArtista.ts | 90% | 100% | 85% |
| auth.service.ts | 95% | 100% | 90% |
| artista.service.ts | 95% | 100% | 90% |

**Meta Global:** 80% en todas las metricas

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- --filter=registro-artista

# Ejecutar tests de auth
npm run test -- __tests__/components/auth

# Ejecutar tests de artista
npm run test -- __tests__/components/artista

# Ejecutar tests de hooks
npm run test -- __tests__/hooks

# Ejecutar tests de services
npm run test -- __tests__/services

# Watch mode
npm run test:watch
```

## 7. MSW Server Setup

**Archivo:** `__mocks__/server.ts`

```typescript
import { setupServer } from 'msw/node';
import { handlers } from './handlers';

export const server = setupServer(...handlers);
```

**Archivo:** `vitest.setup.ts`

```typescript
import { beforeAll, afterEach, afterAll } from 'vitest';
import { server } from './__mocks__/server';

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

## 8. CI/CD Integration

**Archivo:** `.github/workflows/test-admin.yml`

```yaml
name: Test Admin Frontend

on:
  push:
    branches: [master, develop]
    paths:
      - 'src/admin/**'
      - 'src/shared/**'
  pull_request:
    branches: [master, develop]
    paths:
      - 'src/admin/**'
      - 'src/shared/**'

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
          cache-dependency-path: src/admin/package-lock.json

      - name: Install dependencies
        run: npm ci
        working-directory: src/admin

      - name: Run tests with coverage
        run: npm run test:coverage
        working-directory: src/admin

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/admin/coverage/coverage-final.json
          flags: admin-frontend
          fail_ci_if_error: true

      - name: Check coverage thresholds
        run: |
          COVERAGE=$(jq '.total.lines.pct' coverage/coverage-summary.json)
          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "Coverage $COVERAGE% is below 80% threshold"
            exit 1
          fi
        working-directory: src/admin
```

## 9. Vitest Configuration

**Archivo:** `vitest.config.ts` (en src/admin)

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./vitest.setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      include: ['src/**/*.{ts,tsx}'],
      exclude: [
        'src/**/*.test.{ts,tsx}',
        'src/**/__tests__/**',
        'src/**/__mocks__/**',
        'src/**/*.d.ts',
        'src/**/index.ts',
      ],
      all: true,
      lines: 80,
      functions: 80,
      branches: 80,
      statements: 80,
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@/shared': path.resolve(__dirname, '../shared'),
    },
  },
});
```

## 10. Checklist

- [ ] Mock data definido para auth y artista
- [ ] MSW handlers configurados para endpoints POST /api/auth/register y POST /api/artistas
- [ ] MSW server setup en vitest.setup.ts
- [ ] Test utilities con QueryClientProvider y RouterContext
- [ ] Router mock para Next.js
- [ ] Tests de RegisterForm con validaciones y flujo completo
- [ ] Tests de PasswordInput con toggle de visibilidad
- [ ] Tests de CreateArtistaForm con character counter y validaciones
- [ ] Tests de ImagePreview con loading y error states
- [ ] Tests de useRegister hook con localStorage y invalidaciones
- [ ] Tests de useCreateArtista hook con Authorization header
- [ ] Tests de auth.service con MSW handlers
- [ ] Tests de artista.service con MSW handlers
- [ ] Vitest config con coverage thresholds 80%
- [ ] CI/CD workflow para ejecutar tests en push/PR
- [ ] Codecov integration para tracking de coverage
- [ ] Cobertura 80%+ en todos los archivos criticos
- [ ] Tests pasan en CI con tiempo < 60 segundos

## 11. Prioridades de Testing

### Alta Prioridad (Critico para MVP)
1. Validaciones de formularios (RegisterForm, CreateArtistaForm)
2. Flujo de autenticacion (useRegister, localStorage token)
3. Creacion de perfil (useCreateArtista con Authorization header)
4. Error handling (API errors, validacion Zod)
5. Redirecciones (register -> crear perfil -> dashboard)

### Media Prioridad
1. Loading states (spinners, botones deshabilitados)
2. Character counter (descripcion)
3. Image preview (carga, error)
4. Password visibility toggle

### Baja Prioridad (Nice to have)
1. Edge cases de validacion (URLs especiales)
2. Animaciones de transicion
3. Toasts/notifications
4. Optimizaciones de performance

## 12. Estrategia de Mocking

### Mockear
- API calls (MSW handlers)
- Next.js router (router.mock.ts)
- localStorage/sessionStorage (vi.stubGlobal)
- QueryClient (test-utils.tsx)

### NO Mockear
- React Hook Form
- Zod validation
- Tailwind CSS classes
- shadcn/ui components (testeamos integracion real)

## 13. Testing Best Practices

1. **AAA Pattern:** Arrange - Act - Assert en todos los tests
2. **User-centric queries:** Usar `getByRole`, `getByLabelText`, evitar `getByTestId`
3. **Wait for async:** Siempre usar `waitFor`, `findBy*` para operaciones async
4. **Clear localStorage:** Limpiar localStorage en `afterEach` para evitar side effects
5. **Reset MSW handlers:** `server.resetHandlers()` en `afterEach`
6. **Mock timers:** Usar `vi.useFakeTimers()` si hay debounce/throttle
7. **Accessibility:** Validar `aria-*` attributes en componentes criticos
8. **Error boundaries:** Testear error states con MSW handlers de fallo

## 14. Flujos de Integracion E2E (Para referencia)

Estos flujos se testean parcialmente con integration tests, pero idealmente tendrian E2E completo:

### Flujo 1: Registro Completo Exitoso
```
1. Usuario navega a /auth/register
2. Completa email, password, confirmPassword
3. Click en "Registrarse"
4. Backend retorna token JWT
5. Frontend guarda token en localStorage
6. Redirige a /artista/perfil/crear
7. Usuario completa nombreArtistico, descripcion, pais, ciudad, imagenUrl
8. Click en "Crear Perfil"
9. Backend crea Artista vinculado a UserId
10. Frontend invalida queries de artistas
11. Redirige a /dashboard
12. Muestra toast "Bienvenido, [nombreArtistico]"
```

### Flujo 2: Error - Email Duplicado
```
1. Usuario navega a /auth/register
2. Completa email existente, password, confirmPassword
3. Click en "Registrarse"
4. Backend retorna error 400 AUTH_EMAIL_EXISTS
5. Frontend muestra mensaje "Este email ya está registrado. ¿Quieres iniciar sesión?"
6. Usuario hace click en link a /auth/login
7. Navega a login
```

### Flujo 3: Error - Token Expirado Durante Creacion de Perfil
```
1. Usuario completa registro exitoso
2. Espera tiempo suficiente para que token expire (simulado con MSW)
3. Usuario navega a /artista/perfil/crear
4. Completa formulario
5. Click en "Crear Perfil"
6. Backend retorna 401 AUTH_UNAUTHORIZED
7. Frontend muestra mensaje "Tu sesión ha expirado"
8. Redirige a /auth/login
9. Usuario debe re-autenticarse
```

---

**Archivo creado:** `C:\Repos\WePlay_Rises\plans\registro-artista\frontend-admin\test-strategy.md`

**Total tests planificados:** 16
- 12 Unit Tests
- 4 Integration Tests

**Cobertura objetivo:** 80%+

**Tiempo estimado de ejecucion:** < 30 segundos

**Stack:** Vitest + Testing Library + MSW + TanStack Query

**Proximo paso:** Implementar tests siguiendo este plan, priorizando tests de alta prioridad primero (formularios, autenticacion, redirecciones).
