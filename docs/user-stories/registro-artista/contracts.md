# Contratos: Registro de Artista

> **Feature:** registro-artista
> **Última actualización:** 2026-01-26

Este documento define los contratos entre proyectos. **Cualquier cambio aquí debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### POST /api/auth/register

**Descripción:** Crea una nueva cuenta de usuario en ASP.NET Core Identity y retorna JWT token.

**Autorización:** ❌ Pública (usuario no autenticado)

**Request Body:**
```json
{
  "email": "string (formato email válido, requerido)",
  "password": "string (mínimo 8 caracteres, requerido)",
  "confirmPassword": "string (debe coincidir con password, requerido)"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "userId": "guid",
    "email": "banda@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "messages": [
    {
      "message": "Usuario registrado exitosamente",
      "errorCode": "SUCCESS"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | VALIDATION_REQUIRED | El email es obligatorio | Campo email vacío |
| 400 | AUTH_EMAIL_INVALID | El formato del email no es válido | Email malformado |
| 400 | VALIDATION_REQUIRED | La contraseña es obligatoria | Campo password vacío |
| 400 | AUTH_PASSWORD_MIN_LENGTH | La contraseña debe tener al menos 8 caracteres | Password < 8 caracteres |
| 400 | AUTH_PASSWORD_MISMATCH | Las contraseñas no coinciden | Password != ConfirmPassword |
| 409 | AUTH_EMAIL_EXISTS | Este email ya está registrado | Email duplicado en DB |
| 500 | ERROR_UNEXPECTED | Error inesperado al crear cuenta | Excepción no controlada |

---

### POST /api/artistas

**Descripción:** Crea el perfil de Artista vinculado al UserId del token JWT.

**Autorización:** ✅ Bearer JWT (UserId extraído del token)

**Request Body:**
```json
{
  "nombreArtistico": "string (máx 200 caracteres, requerido)",
  "descripcion": "string (máx 2000 caracteres, opcional)",
  "pais": "string (máx 100 caracteres, opcional)",
  "ciudad": "string (máx 100 caracteres, opcional)",
  "imagenUrl": "string (URL válida, opcional)"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "userId": "guid",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z"
  },
  "messages": [
    {
      "message": "Perfil de artista creado exitosamente",
      "errorCode": "SUCCESS"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | VALIDATION_REQUIRED | El nombre artístico es obligatorio | Campo nombreArtistico vacío |
| 400 | ARTISTA_NOMBRE_MAX_LENGTH | El nombre artístico no puede superar los 200 caracteres | nombreArtistico > 200 chars |
| 400 | ARTISTA_DESC_MAX_LENGTH | La descripción no puede superar los 2000 caracteres | descripcion > 2000 chars |
| 400 | ARTISTA_IMAGEN_URL_INVALIDA | La URL de la imagen no es válida | imagenUrl no es URL válida |
| 401 | AUTH_UNAUTHORIZED | Token no válido o expirado | Token JWT inválido/expirado |
| 409 | ARTISTA_ALREADY_EXISTS | Este usuario ya tiene un perfil de artista | UserId ya tiene Artista |
| 500 | ERROR_UNEXPECTED | Error inesperado al crear perfil | Excepción no controlada |

---

### GET /api/artistas/{id}

**Descripción:** Obtiene el perfil público de un artista por su ID.

**Autorización:** ❌ Pública (perfil visible para todos)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z"
  },
  "messages": [
    {
      "message": "Artista encontrado",
      "errorCode": "SUCCESS"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | VALIDATION_REQUIRED | El ID es obligatorio | ID vacío o nulo |
| 404 | ARTISTA_NOT_FOUND | Artista no encontrado | ID no existe en DB |
| 500 | ERROR_UNEXPECTED | Error inesperado | Excepción no controlada |

---

### GET /api/artistas/by-user/{userId}

**Descripción:** Obtiene el perfil de artista por UserId (para uso interno tras login).

**Autorización:** ✅ Bearer JWT (UserId del token debe coincidir)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "userId": "guid",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z"
  },
  "messages": [
    {
      "message": "Artista encontrado",
      "errorCode": "SUCCESS"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | AUTH_UNAUTHORIZED | Token no válido o expirado | Token JWT inválido/expirado |
| 404 | ARTISTA_NOT_FOUND | Artista no encontrado para este usuario | UserId sin perfil Artista |
| 500 | ERROR_UNEXPECTED | Error inesperado | Excepción no controlada |

---

## 🔐 Autorización

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/auth/register` | ❌ | - | Público, crea usuario |
| `POST /api/artistas` | ✅ | - | UserId extraído del token |
| `GET /api/artistas/{id}` | ❌ | - | Perfil público |
| `GET /api/artistas/by-user/{userId}` | ✅ | - | UserId debe coincidir con token |

### Claims JWT Requeridos

```json
{
  "sub": "userId (GUID)",
  "email": "usuario@example.com",
  "exp": 1738000000,
  "iat": 1737913600
}
```

**Configuración JWT:**
- **Algoritmo:** HS256
- **Expiración:** 24 horas por defecto
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Protección de Rutas Frontend

| Ruta (Admin) | Auth | Redirect | Notas |
|--------------|------|----------|-------|
| `/auth/register` | ❌ | - | Pública |
| `/auth/login` | ❌ | - | Pública |
| `/artista/perfil/crear` | ✅ | `/auth/login` | Solo para usuarios sin perfil |
| `/dashboard` | ✅ | `/auth/login` | Requiere perfil Artista |

| Ruta (Landing) | Auth | Redirect | Notas |
|----------------|------|----------|-------|
| `/artistas/{id}` | ❌ | - | Pública |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/UserAccess/UserAccess.Application/Dtos/ArtistaDto.cs
public class ArtistaDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/UserAccess/UserAccess.Application/Dtos/RegisterResponseDto.cs
public class RegisterResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/UserAccess/UserAccess.Application/Dtos/ArtistaListDto.cs
// DTO simplificado para listados (futuro)
public class ArtistaListDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/artista.ts
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

```typescript
// Ruta: src/shared/types/auth.ts
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

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| email | requerido | `.NotEmpty().WithErrorCode("VALIDATION_REQUIRED")` | `.min(1, 'El email es obligatorio')` |
| email | formato email | `.EmailAddress().WithErrorCode("AUTH_EMAIL_INVALID")` | `.email('Formato de email inválido')` |
| password | requerido | `.NotEmpty().WithErrorCode("VALIDATION_REQUIRED")` | `.min(1, 'La contraseña es obligatoria')` |
| password | mínimo 8 caracteres | `.MinimumLength(8).WithErrorCode("AUTH_PASSWORD_MIN_LENGTH")` | `.min(8, 'Mínimo 8 caracteres')` |
| confirmPassword | coincide con password | `.Equal(x => x.Password).WithErrorCode("AUTH_PASSWORD_MISMATCH")` | `.refine((data) => data.password === data.confirmPassword)` |
| nombreArtistico | requerido | `.NotEmpty().WithErrorCode("VALIDATION_REQUIRED")` | `.min(1, 'El nombre artístico es obligatorio')` |
| nombreArtistico | máximo 200 caracteres | `.MaximumLength(200).WithErrorCode("ARTISTA_NOMBRE_MAX_LENGTH")` | `.max(200, 'Máximo 200 caracteres')` |
| descripcion | máximo 2000 caracteres | `.MaximumLength(2000).WithErrorCode("ARTISTA_DESC_MAX_LENGTH")` | `.max(2000, 'Máximo 2000 caracteres').optional()` |
| pais | máximo 100 caracteres | `.MaximumLength(100).WithErrorCode("ARTISTA_PAIS_MAX_LENGTH")` | `.max(100, 'Máximo 100 caracteres').optional()` |
| ciudad | máximo 100 caracteres | `.MaximumLength(100).WithErrorCode("ARTISTA_CIUDAD_MAX_LENGTH")` | `.max(100, 'Máximo 100 caracteres').optional()` |
| imagenUrl | URL válida (si no vacía) | `.Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.ImagenUrl)).WithErrorCode("ARTISTA_IMAGEN_URL_INVALIDA")` | `.url('Debe ser una URL válida').optional().or(z.literal(''))` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/auth.schema.ts
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

```typescript
// Ruta: src/shared/schemas/artista.schema.ts
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

---

## 🔗 Constantes Compartidas

```typescript
// Ruta: src/shared/constants/query-keys.ts
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

```typescript
// Ruta: src/shared/constants/api-routes.ts
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

```typescript
// Ruta: src/shared/constants/app-routes.ts
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

---

## 📊 Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts
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

---

## 🔔 Eventos SignalR

**No aplica para esta feature.** El registro de artista es un flujo síncrono sin necesidad de notificaciones en tiempo real.

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores
- [x] Autorización por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas
- [x] Constantes compartidas (query keys, API routes, app routes)
- [x] Mapeo de errores a mensajes UI
