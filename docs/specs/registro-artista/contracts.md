# Contratos: Registro de Artista

> **Feature:** registro-artista
> **Última actualización:** 2026-01-26

Este documento define los contratos entre proyectos. **Cualquier cambio aquí debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### POST /api/auth/register

**Descripción:** Registrar nuevo usuario
**Autorización:** ❌ Pública (sin JWT)

**Request Body:**
```json
{
  "email": "banda@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "banda@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "messages": [
    { "message": "Usuario registrado correctamente", "errorCode": "SUCCESS" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | VALIDATION_REQUIRED | El email es obligatorio | Email vacío |
| 400 | VALIDATION_EMAIL_FORMAT | Formato de email inválido | Email mal formado |
| 400 | VALIDATION_PASSWORD_MIN | Mínimo 8 caracteres | Password corto |
| 400 | VALIDATION_PASSWORD_MISMATCH | Las contraseñas no coinciden | Confirm != password |
| 409 | AUTH_EMAIL_EXISTS | El email ya está registrado | Email duplicado |

---

### POST /api/auth/login

**Descripción:** Iniciar sesión
**Autorización:** ❌ Pública (sin JWT)

**Request Body:**
```json
{
  "email": "banda@example.com",
  "password": "SecurePass123!"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "banda@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-01-27T10:00:00Z"
  },
  "messages": []
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 401 | AUTH_INVALID_CREDENTIALS | Email o contraseña incorrectos |
| 423 | AUTH_ACCOUNT_LOCKED | Cuenta bloqueada temporalmente |

---

### POST /api/artistas

**Descripción:** Crear perfil de artista
**Autorización:** ✅ Bearer JWT (usuario autenticado)

**Headers:**
```
Authorization: Bearer {token}
```

**Request Body:**
```json
{
  "nombreArtistico": "Los Rockeros",
  "descripcion": "Banda de rock alternativo de Madrid",
  "pais": "España",
  "ciudad": "Madrid",
  "imagenUrl": "https://example.com/imagen.jpg"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/imagen.jpg",
    "fechaCreacion": "2026-01-26T15:30:00Z"
  },
  "messages": [
    { "message": "Perfil de artista creado", "errorCode": "SUCCESS" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 400 | ARTISTA_NOMBRE_REQUERIDO | El nombre artístico es obligatorio |
| 400 | ARTISTA_NOMBRE_MAX_LENGTH | Máximo 200 caracteres |
| 400 | ARTISTA_DESC_MAX_LENGTH | Máximo 2000 caracteres |
| 400 | ARTISTA_IMAGEN_URL_INVALIDA | Debe ser una URL válida |
| 401 | UNAUTHORIZED | Token inválido o expirado |
| 409 | ARTISTA_YA_EXISTE | El usuario ya tiene un perfil de artista |

---

### GET /api/artistas/me

**Descripción:** Obtener perfil del artista autenticado
**Autorización:** ✅ Bearer JWT

**Response 200 OK:**
```json
{
  "data": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/imagen.jpg",
    "fechaCreacion": "2026-01-26T15:30:00Z"
  },
  "messages": []
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 401 | UNAUTHORIZED | Token inválido o expirado |
| 404 | ARTISTA_NO_ENCONTRADO | El usuario no tiene perfil de artista |

---

### GET /api/artistas/{id}

**Descripción:** Obtener perfil público de artista
**Autorización:** ❌ Pública

**Response 200 OK:**
```json
{
  "data": {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/imagen.jpg",
    "campanias": [
      { "id": "...", "titulo": "Nuevo álbum", "estado": "publicada" }
    ]
  },
  "messages": []
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje |
|------|-----------|---------|
| 404 | ARTISTA_NO_ENCONTRADO | Artista no encontrado |

---

## 🔐 Autorización

### Resumen por Endpoint

| Endpoint | Auth | Notas |
|----------|------|-------|
| `POST /api/auth/register` | ❌ Pública | - |
| `POST /api/auth/login` | ❌ Pública | - |
| `POST /api/artistas` | ✅ Bearer | UserId extraído del token |
| `GET /api/artistas/me` | ✅ Bearer | Retorna artista del usuario autenticado |
| `GET /api/artistas/{id}` | ❌ Pública | Perfil público |

### Claims JWT Requeridos

```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",  // userId
  "email": "banda@example.com",
  "exp": 1706356800  // expiración
}
```

### Protección de Rutas Frontend

| Ruta | Requiere Auth | Redirect si no auth |
|------|---------------|---------------------|
| `/auth/register` | ❌ | - |
| `/auth/login` | ❌ | - |
| `/artista/perfil/crear` | ✅ | `/auth/login` |
| `/dashboard` | ✅ | `/auth/login` |
| `/artistas/{id}` | ❌ | - |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// ====== AUTH ======

// Modules/UserAccess/Application/Features/Auth/Commands/RegisterCommand.cs
public class RegisterCommand : IRequest<ServiceResponse<AuthResultDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

// Modules/UserAccess/Application/Dtos/AuthResultDto.cs
public class AuthResultDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
}

// ====== ARTISTA ======

// Modules/UserAccess/Application/Features/Artistas/Commands/CreateArtistaCommand.cs
public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
}

// Modules/UserAccess/Application/Dtos/ArtistaDto.cs
public class ArtistaDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
}

// Para perfil público con campañas
public class ArtistaPublicoDto : ArtistaDto
{
    public List<CampaniaResumenDto> Campanias { get; set; } = new();
}
```

### Frontend (TypeScript)

```typescript
// src/shared/types/auth.ts

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResult {
  userId: string;
  email: string;
  token: string;
  expiresAt?: string;
}

// src/shared/types/artista.ts

export interface CreateArtistaRequest {
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
}

export interface Artista {
  id: string;
  userId: string;
  nombreArtistico: string;
  descripcion?: string;
  pais?: string;
  ciudad?: string;
  imagenUrl?: string;
  fechaCreacion: string;
}

export interface ArtistaPublico extends Artista {
  campanias: CampaniaResumen[];
}
```

---

## ✅ Validaciones Compartidas

Las validaciones deben ser idénticas en Backend (FluentValidation) y Frontend (Zod).

### Register

| Campo | Regla | Backend | Frontend |
|-------|-------|---------|----------|
| email | required | `.NotEmpty()` | `.min(1)` |
| email | email format | `.EmailAddress()` | `.email()` |
| password | required | `.NotEmpty()` | `.min(1)` |
| password | min 8 chars | `.MinimumLength(8)` | `.min(8)` |
| confirmPassword | equals password | `.Equal(x => x.Password)` | `.refine()` |

### Artista

| Campo | Regla | Backend | Frontend |
|-------|-------|---------|----------|
| nombreArtistico | required | `.NotEmpty()` | `.min(1)` |
| nombreArtistico | max 200 | `.MaximumLength(200)` | `.max(200)` |
| descripcion | max 2000 | `.MaximumLength(2000)` | `.max(2000)` |
| imagenUrl | valid URL | `.Must(BeAValidUrl)` | `.url()` |

### Zod Schemas

```typescript
// src/shared/schemas/auth.schema.ts
import { z } from 'zod';

export const registerSchema = z.object({
  email: z.string().min(1, 'El email es obligatorio').email('Formato de email inválido'),
  password: z.string().min(8, 'Mínimo 8 caracteres'),
  confirmPassword: z.string(),
}).refine(data => data.password === data.confirmPassword, {
  message: 'Las contraseñas no coinciden',
  path: ['confirmPassword'],
});

export const loginSchema = z.object({
  email: z.string().min(1, 'El email es obligatorio').email('Formato de email inválido'),
  password: z.string().min(1, 'La contraseña es obligatoria'),
});

// src/shared/schemas/artista.schema.ts
import { z } from 'zod';

export const createArtistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(1, 'El nombre artístico es obligatorio')
    .max(200, 'Máximo 200 caracteres'),
  descripcion: z.string().max(2000, 'Máximo 2000 caracteres').optional(),
  pais: z.string().max(100).optional(),
  ciudad: z.string().max(100).optional(),
  imagenUrl: z.string().url('Debe ser una URL válida').optional().or(z.literal('')),
});
```

---

## 🔔 Eventos SignalR

Para esta feature no hay eventos SignalR necesarios. El registro es una operación puntual que no requiere notificaciones en tiempo real a otros usuarios.

**Nota:** En features futuras (US-04: Hacer Backing), sí habrá eventos como `BackingRecibido`.

---

## 🔗 Constantes Compartidas

```typescript
// src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  // Auth
  currentUser: ['auth', 'me'] as const,

  // Artistas
  artista: (id: string) => ['artistas', id] as const,
  artistaMe: ['artistas', 'me'] as const,
  artistas: ['artistas'] as const,
};

// src/shared/constants/api-routes.ts
export const API_ROUTES = {
  auth: {
    register: '/api/auth/register',
    login: '/api/auth/login',
  },
  artistas: {
    create: '/api/artistas',
    me: '/api/artistas/me',
    byId: (id: string) => `/api/artistas/${id}`,
  },
};

// src/shared/constants/routes.ts
export const ROUTES = {
  auth: {
    register: '/auth/register',
    login: '/auth/login',
  },
  artista: {
    crearPerfil: '/artista/perfil/crear',
    perfil: (id: string) => `/artistas/${id}`,
  },
  dashboard: '/dashboard',
};
```

---

## 📊 Mapeo de Errores a UI

```typescript
// src/shared/utils/error-messages.ts
export const ERROR_MESSAGES: Record<string, string> = {
  // Auth
  VALIDATION_REQUIRED: 'Este campo es obligatorio',
  VALIDATION_EMAIL_FORMAT: 'El formato del email no es válido',
  VALIDATION_PASSWORD_MIN: 'La contraseña debe tener al menos 8 caracteres',
  VALIDATION_PASSWORD_MISMATCH: 'Las contraseñas no coinciden',
  AUTH_EMAIL_EXISTS: 'Este email ya está registrado. ¿Quieres iniciar sesión?',
  AUTH_INVALID_CREDENTIALS: 'Email o contraseña incorrectos',
  AUTH_ACCOUNT_LOCKED: 'Tu cuenta ha sido bloqueada temporalmente. Intenta más tarde.',

  // Artista
  ARTISTA_NOMBRE_REQUERIDO: 'El nombre artístico es obligatorio',
  ARTISTA_NOMBRE_MAX_LENGTH: 'El nombre no puede superar 200 caracteres',
  ARTISTA_DESC_MAX_LENGTH: 'La descripción no puede superar 2000 caracteres',
  ARTISTA_IMAGEN_URL_INVALIDA: 'La URL de la imagen no es válida',
  ARTISTA_YA_EXISTE: 'Ya tienes un perfil de artista',
  ARTISTA_NO_ENCONTRADO: 'Perfil de artista no encontrado',

  // Generic
  UNAUTHORIZED: 'Tu sesión ha expirado. Por favor, inicia sesión de nuevo.',
  ERROR_UNEXPECTED: 'Ha ocurrido un error. Por favor, intenta de nuevo.',
};
```

---

## Checklist de Contratos

- [x] Endpoints definidos con request/response
- [x] Códigos de error documentados
- [x] Autorización por endpoint
- [x] Claims JWT especificados
- [x] DTOs C# definidos
- [x] Types TypeScript definidos
- [x] Schemas Zod definidos
- [x] Rutas protegidas identificadas
- [x] Constantes compartidas
- [x] Mapeo de errores a mensajes UI
- [ ] Eventos SignalR (no aplica para esta feature)

---

*Este documento es el contrato entre proyectos. Cualquier cambio debe comunicarse a todos los equipos.*
