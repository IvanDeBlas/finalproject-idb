# Plan de Contratos Shared: Registro de Artista

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Basado en:** docs/user-stories/registro-artista/contracts.md

## 1. Resumen

- Total de types: 5 interfaces (Artista, ArtistaListItem, RegisterRequest, RegisterResponse, CreateArtistaRequest)
- Total de schemas: 2 (registerSchema, createArtistaSchema)
- Constantes definidas: 3 archivos (query-keys, api-routes, app-routes)
- Utilidades planificadas: 1 (error-messages con helper)

## 2. Types (`src/shared/types/`)

### 2.1 Artista Types (`artista.ts`)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `Artista` | id, userId, nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl?, fechaCreacion, fechaActualizacion? | Entidad completa de artista con todos los campos |
| `ArtistaListItem` | id, nombreArtistico, imagenUrl?, ciudad?, pais? | DTO simplificado para listados futuros |

**Contenido completo:**
```typescript
export interface Artista {
  id: string;
  userId: string;
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
  fechaCreacion: string; // ISO 8601 string
  fechaActualizacion?: string; // ISO 8601 string
}

export interface ArtistaListItem {
  id: string;
  nombreArtistico: string;
  imagenUrl?: string;
  ciudad?: string;
  pais?: string;
}
```

### 2.2 Auth Types (`auth.ts`)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `RegisterRequest` | email, password, confirmPassword | Request para POST /api/auth/register |
| `RegisterResponse` | userId, email, token | Response exitoso del registro con JWT token |
| `CreateArtistaRequest` | nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl? | Request para POST /api/artistas |

**Contenido completo:**
```typescript
export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
  token: string;
}

export interface CreateArtistaRequest {
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
}
```

## 3. Schemas Zod (`src/shared/schemas/`)

### 3.1 Auth Schema (`auth.schema.ts`)

| Schema | Campos | Reglas |
|--------|--------|--------|
| `registerSchema` | email, password, confirmPassword | email: required + email format; password: min 8 chars; confirmPassword: match password |

**Contenido completo:**
```typescript
import { z } from 'zod';

export const registerSchema = z.object({
  email: z
    .string()
    .min(1, 'El email es obligatorio')
    .email('Formato de email inválido'),
  password: z
    .string()
    .min(1, 'La contraseña es obligatoria')
    .min(8, 'La contraseña debe tener al menos 8 caracteres'),
  confirmPassword: z
    .string()
    .min(1, 'Confirme su contraseña'),
}).refine((data) => data.password === data.confirmPassword, {
  message: 'Las contraseñas no coinciden',
  path: ['confirmPassword'],
});

export type RegisterFormData = z.infer<typeof registerSchema>;
```

### 3.2 Artista Schema (`artista.schema.ts`)

| Schema | Campos | Reglas |
|--------|--------|--------|
| `createArtistaSchema` | nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl? | nombreArtistico: max 200 chars; descripcion: max 2000 chars; pais/ciudad: max 100 chars; imagenUrl: valid URL |

**Contenido completo:**
```typescript
import { z } from 'zod';

export const createArtistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(1, 'El nombre artístico es obligatorio')
    .max(200, 'El nombre artístico no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(2000, 'La descripción no puede superar los 2000 caracteres')
    .optional()
    .or(z.literal('')),
  pais: z
    .string()
    .max(100, 'El país no puede superar los 100 caracteres')
    .optional()
    .or(z.literal('')),
  ciudad: z
    .string()
    .max(100, 'La ciudad no puede superar los 100 caracteres')
    .optional()
    .or(z.literal('')),
  imagenUrl: z
    .string()
    .url('Debe ser una URL válida')
    .optional()
    .or(z.literal('')),
});

export type CreateArtistaFormData = z.infer<typeof createArtistaSchema>;
```

### 3.3 Types Inferidos

- `RegisterFormData` = `z.infer<typeof registerSchema>`
- `CreateArtistaFormData` = `z.infer<typeof createArtistaSchema>`

## 4. Constantes (`src/shared/constants/`)

### 4.1 Query Keys (`query-keys.ts`)

| Key | Patron | Uso |
|-----|--------|-----|
| `artistas.all` | `['artistas']` | Todas las queries de artistas |
| `artistas.byId(id)` | `['artistas', id]` | Query de artista por ID publico |
| `artistas.byUserId(userId)` | `['artistas', 'user', userId]` | Query de artista por UserId autenticado |
| `auth.currentUser` | `['auth', 'current-user']` | Usuario actualmente logueado |

**Contenido completo:**
```typescript
export const QUERY_KEYS = {
  artistas: {
    all: ['artistas'] as const,
    byId: (id: string) => ['artistas', id] as const,
    byUserId: (userId: string) => ['artistas', 'user', userId] as const,
  },
  auth: {
    currentUser: ['auth', 'current-user'] as const,
  },
} as const;
```

### 4.2 API Routes (`api-routes.ts`)

| Constante | Valor | Uso |
|-----------|-------|-----|
| `auth.register` | `/api/auth/register` | Endpoint de registro |
| `auth.login` | `/api/auth/login` | Endpoint de login (futuro) |
| `artistas.base` | `/api/artistas` | POST crear artista |
| `artistas.byId(id)` | `/api/artistas/${id}` | GET artista publico por ID |
| `artistas.byUserId(userId)` | `/api/artistas/by-user/${userId}` | GET artista por UserId autenticado |

**Contenido completo:**
```typescript
export const API_ROUTES = {
  auth: {
    register: '/api/auth/register',
    login: '/api/auth/login',
  },
  artistas: {
    base: '/api/artistas',
    byId: (id: string) => `/api/artistas/${id}`,
    byUserId: (userId: string) => `/api/artistas/by-user/${userId}`,
  },
} as const;
```

### 4.3 App Routes (`app-routes.ts`)

| Constante | Valor | Uso |
|-----------|-------|-----|
| `auth.register` | `/auth/register` | Pagina de registro (Admin) |
| `auth.login` | `/auth/login` | Pagina de login (Admin) |
| `artista.crearPerfil` | `/artista/perfil/crear` | Crear perfil de artista (Admin) |
| `dashboard` | `/dashboard` | Dashboard principal (Admin) |
| `landing.artistaById(id)` | `/artistas/${id}` | Perfil publico artista (Landing) |

**Contenido completo:**
```typescript
export const APP_ROUTES = {
  auth: {
    register: '/auth/register',
    login: '/auth/login',
  },
  artista: {
    crearPerfil: '/artista/perfil/crear',
  },
  dashboard: '/dashboard',
  landing: {
    artistaById: (id: string) => `/artistas/${id}`,
  },
} as const;
```

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Error Messages (`error-messages.ts`)

| Funcion | Input | Output | Descripcion |
|---------|-------|--------|-------------|
| `getErrorMessage(errorCode)` | string (errorCode) | string (mensaje) | Retorna mensaje en español para errorCode dado |

| Codigo | Mensaje | Contexto |
|--------|---------|----------|
| `AUTH_EMAIL_INVALID` | El formato del email no es válido | Validacion de email |
| `AUTH_EMAIL_EXISTS` | Este email ya está registrado. ¿Quieres iniciar sesión? | Email duplicado |
| `AUTH_PASSWORD_MIN_LENGTH` | La contraseña debe tener al menos 8 caracteres | Password < 8 chars |
| `AUTH_PASSWORD_MISMATCH` | Las contraseñas no coinciden | Password != ConfirmPassword |
| `AUTH_UNAUTHORIZED` | Tu sesión ha expirado. Por favor, inicia sesión nuevamente | Token JWT invalido/expirado |
| `ARTISTA_NOMBRE_REQUERIDO` | El nombre artístico es obligatorio | nombreArtistico vacio |
| `ARTISTA_NOMBRE_MAX_LENGTH` | El nombre artístico no puede superar los 200 caracteres | nombreArtistico > 200 chars |
| `ARTISTA_DESC_MAX_LENGTH` | La descripción no puede superar los 2000 caracteres | descripcion > 2000 chars |
| `ARTISTA_PAIS_MAX_LENGTH` | El país no puede superar los 100 caracteres | pais > 100 chars |
| `ARTISTA_CIUDAD_MAX_LENGTH` | La ciudad no puede superar los 100 caracteres | ciudad > 100 chars |
| `ARTISTA_IMAGEN_URL_INVALIDA` | La URL de la imagen no es válida. Debe comenzar con http:// o https:// | imagenUrl no es URL valida |
| `ARTISTA_ALREADY_EXISTS` | Ya tienes un perfil de artista creado | UserId ya tiene Artista |
| `ARTISTA_NOT_FOUND` | Artista no encontrado | ID no existe en DB |
| `VALIDATION_REQUIRED` | Este campo es obligatorio | Campo requerido vacio |
| `ERROR_UNEXPECTED` | Ha ocurrido un error inesperado. Por favor, intenta nuevamente | Error generico |
| `NOT_FOUND` | Recurso no encontrado | 404 generico |

**Contenido completo:**
```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // Auth errors
  AUTH_EMAIL_INVALID: 'El formato del email no es válido',
  AUTH_EMAIL_EXISTS: 'Este email ya está registrado. ¿Quieres iniciar sesión?',
  AUTH_PASSWORD_MIN_LENGTH: 'La contraseña debe tener al menos 8 caracteres',
  AUTH_PASSWORD_MISMATCH: 'Las contraseñas no coinciden',
  AUTH_UNAUTHORIZED: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente',

  // Artista errors
  ARTISTA_NOMBRE_REQUERIDO: 'El nombre artístico es obligatorio',
  ARTISTA_NOMBRE_MAX_LENGTH: 'El nombre artístico no puede superar los 200 caracteres',
  ARTISTA_DESC_MAX_LENGTH: 'La descripción no puede superar los 2000 caracteres',
  ARTISTA_PAIS_MAX_LENGTH: 'El país no puede superar los 100 caracteres',
  ARTISTA_CIUDAD_MAX_LENGTH: 'La ciudad no puede superar los 100 caracteres',
  ARTISTA_IMAGEN_URL_INVALIDA: 'La URL de la imagen no es válida. Debe comenzar con http:// o https://',
  ARTISTA_ALREADY_EXISTS: 'Ya tienes un perfil de artista creado',
  ARTISTA_NOT_FOUND: 'Artista no encontrado',

  // Validation errors
  VALIDATION_REQUIRED: 'Este campo es obligatorio',

  // Generic errors
  ERROR_UNEXPECTED: 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
  NOT_FOUND: 'Recurso no encontrado',
} as const;

export const getErrorMessage = (errorCode: string): string => {
  return ERROR_MESSAGES[errorCode] || ERROR_MESSAGES.ERROR_UNEXPECTED;
};
```

## 6. Archivos a Crear

```
src/shared/
├── types/
│   ├── artista.ts          # Artista, ArtistaListItem
│   └── auth.ts             # RegisterRequest, RegisterResponse, CreateArtistaRequest
├── schemas/
│   ├── artista.schema.ts   # createArtistaSchema, CreateArtistaFormData
│   └── auth.schema.ts      # registerSchema, RegisterFormData
├── constants/
│   ├── query-keys.ts       # QUERY_KEYS (artistas, auth)
│   ├── api-routes.ts       # API_ROUTES (backend endpoints)
│   └── app-routes.ts       # APP_ROUTES (frontend routes)
└── utils/
    └── error-messages.ts   # ERROR_MESSAGES, getErrorMessage()
```

## 7. Dependencias

- `zod` (ya instalado en ambos frontends)
- Ningun package adicional requerido

## 8. Notas de Implementacion

### 8.1 Validacion de URL en Zod
Para `imagenUrl`, Zod valida formato de URL pero permite string vacio con `.or(z.literal(''))`. Esto permite campos opcionales que, si se proveen, deben ser URLs validas.

### 8.2 Fechas como ISO 8601 Strings
`fechaCreacion` y `fechaActualizacion` son strings en formato ISO 8601 (ej: `2026-01-26T10:30:00Z`). Backend serializa `DateTime` a este formato automaticamente.

### 8.3 Campos Opcionales
Los campos opcionales (`descripcion`, `pais`, `ciudad`, `imagenUrl`) usan `optional()` en types y `.or(z.literal(''))` en schemas para soportar tanto `undefined` como string vacio.

### 8.4 Constantes con `as const`
Todos los objetos de constantes usan `as const` para type narrowing y autocomplete preciso en TypeScript.

### 8.5 Error Codes Alineados con Backend
Los error codes en `ERROR_MESSAGES` coinciden exactamente con los `WithErrorCode()` de FluentValidation en backend. Esto garantiza que frontend muestre mensajes correctos.

### 8.6 Query Keys Jerarquicos
`QUERY_KEYS.artistas.all` es prefijo de `QUERY_KEYS.artistas.byId(id)`, permitiendo invalidaciones granulares con TanStack Query:
```typescript
// Invalida todas las queries de artistas
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.all });

// Invalida solo un artista especifico
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.byId('123') });
```

### 8.7 Sin Mappers en esta Feature
No se requieren mappers complejos para esta feature. Los DTOs del backend son identicos a los types del frontend (solo cambio de `Guid` a `string`).

## 9. Checklist

- [ ] Types creados y exportados (`artista.ts`, `auth.ts`)
- [ ] Schemas Zod con mensajes en español (`artista.schema.ts`, `auth.schema.ts`)
- [ ] Constantes de endpoints alineadas con backend (`api-routes.ts`)
- [ ] Query keys consistentes con patron del proyecto (`query-keys.ts`)
- [ ] App routes para navegacion frontend (`app-routes.ts`)
- [ ] Error messages mapeados 1:1 con backend error codes (`error-messages.ts`)
- [ ] Helper `getErrorMessage()` con fallback a ERROR_UNEXPECTED
- [ ] Todos los archivos usan `as const` para type safety
- [ ] Validaciones Zod replican exactamente las reglas de FluentValidation

## 10. Alineacion con Backend

### 10.1 Endpoints API
| Backend | Constante Frontend |
|---------|-------------------|
| `POST /api/auth/register` | `API_ROUTES.auth.register` |
| `POST /api/artistas` | `API_ROUTES.artistas.base` |
| `GET /api/artistas/{id}` | `API_ROUTES.artistas.byId(id)` |
| `GET /api/artistas/by-user/{userId}` | `API_ROUTES.artistas.byUserId(userId)` |

### 10.2 Error Codes
Todos los error codes en `ERROR_MESSAGES` estan documentados en `contracts.md` secciones de errores de cada endpoint.

### 10.3 Validaciones
Las reglas de Zod schemas replican exactamente las de FluentValidation:
- Mismos min/max lengths
- Mismas validaciones de formato (email, URL)
- Mismos mensajes de error en español
- Mismos error codes

## 11. Uso Esperado en Frontend

### 11.1 Landing (Vite + React)
```typescript
// Ver perfil publico de artista
import { API_ROUTES } from '@/shared/constants/api-routes';
import { QUERY_KEYS } from '@/shared/constants/query-keys';
import { Artista } from '@/shared/types/artista';

const { data: artista } = useQuery({
  queryKey: QUERY_KEYS.artistas.byId(id),
  queryFn: () => api.get<Artista>(API_ROUTES.artistas.byId(id)),
});
```

### 11.2 Admin (Next.js)
```typescript
// Formulario de registro
import { registerSchema, RegisterFormData } from '@/shared/schemas/auth.schema';
import { API_ROUTES } from '@/shared/constants/api-routes';
import { RegisterRequest, RegisterResponse } from '@/shared/types/auth';

const form = useForm<RegisterFormData>({
  resolver: zodResolver(registerSchema),
});

const mutation = useMutation({
  mutationFn: (data: RegisterRequest) =>
    api.post<RegisterResponse>(API_ROUTES.auth.register, data),
});
```

---

**Siguiente paso sugerido:** Ejecutar agentes de backend y frontend en paralelo para implementacion basada en este plan.
