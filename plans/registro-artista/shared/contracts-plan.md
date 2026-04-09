# Plan de Contratos Shared: registro-artista

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Basado en:** docs/user-stories/registro-artista/contracts.md

---

## 1. Resumen

- **Total de types:** 7 tipos ya existentes (mantener), 0 tipos nuevos
- **Total de schemas:** 2 schemas ya existentes (validar alineamiento con contracts.md)
- **Constantes definidas:** Ya existen en `constants/index.ts` (validar completitud)
- **Utilidades planificadas:** Ya existe `error-messages.ts` (validar códigos de error)

### Estado Actual

El código compartido para `registro-artista` **YA EXISTE** en `src/shared/`. Este plan documenta:
1. ✅ Qué archivos ya están implementados
2. 🔧 Qué ajustes necesitan para alinearse 100% con `contracts.md`
3. 📝 Qué está faltando (si aplica)

---

## 2. Types (`src/shared/types/`)

### 2.1 Archivo: `auth.ts` ✅ COMPLETO

| Tipo | Estado | Propiedades | Alineado con Backend |
|------|--------|-------------|----------------------|
| `RegisterRequest` | ✅ Existe | email, password, confirmPassword | ✅ Sí |
| `RegisterResponse` | ✅ Existe | userId, email, token | ✅ Sí |
| `CreateArtistaRequest` | ✅ Existe | nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl? | ⚠️ Revisar (ver sección 2.3) |
| `User` | ✅ Existe | id, email, nombreCompleto?, roles? | ✅ Sí |
| `LoginCredentials` | ✅ Existe | email, password | N/A (no usado en esta feature) |
| `AuthResponse` | ✅ Existe | token, user | N/A (no usado en esta feature) |
| `JwtPayload` | ✅ Existe | sub, email, name?, roles, exp, iat | ⚠️ Revisar claims (ver sección 2.4) |

### 2.2 Archivo: `artista.ts` ✅ COMPLETO

| Tipo | Estado | Propiedades | Alineado con Backend |
|------|--------|-------------|----------------------|
| `Artista` | ✅ Existe | id, userId, nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl?, generoMusical?, redesSociales?, fechaCreacion, fechaActualizacion? | ⚠️ Incluye campos extras (ver sección 2.5) |
| `ArtistaDto` | ✅ Existe | (igual que Artista) | ⚠️ Duplicado innecesario |
| `ArtistaListItem` | ✅ Existe | id, nombreArtistico, imagenUrl?, ciudad?, pais? | ✅ Sí |
| `CreateArtistaDto` | ✅ Existe | nombreArtistico, descripcion?, pais?, ciudad?, imagenUrl?, generoMusical?, redesSociales? | ⚠️ Incluye campos no definidos en contracts.md |
| `UpdateArtistaDto` | ✅ Existe | (todos opcionales) | N/A (no usado en esta feature) |
| `RedesSociales` | ✅ Existe | instagram?, twitter?, youtube?, spotify?, website? | N/A (no definido en contracts.md) |

### 2.3 ACCIÓN REQUERIDA: Alinear `CreateArtistaRequest`

**Problema:** `CreateArtistaRequest` está definido en `auth.ts` pero debería usar la misma estructura que `CreateArtistaDto`.

**Decisión:**
- **Opción A (Recomendada):** Mover `CreateArtistaRequest` a `artista.ts` y hacerlo alias de `CreateArtistaDto`
- **Opción B:** Mantener separados pero sincronizar propiedades

**Implementación Sugerida (Opción A):**

```typescript
// src/shared/types/artista.ts (modificar)
export interface CreateArtistaRequest {
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  // generoMusical y redesSociales NO están en contracts.md para MVP
  // Se pueden agregar en futuras features
}

// Alias para compatibilidad
export type CreateArtistaDto = CreateArtistaRequest
```

### 2.4 ACCIÓN REQUERIDA: Validar `JwtPayload`

**Según contracts.md, el JWT debe contener:**
```json
{
  "sub": "userId (GUID)",
  "email": "usuario@example.com",
  "exp": 1738000000,
  "iat": 1737913600
}
```

**Estado actual en `auth.ts`:**
```typescript
export interface JwtPayload {
  sub: string       // ✅ OK
  email: string     // ✅ OK
  name?: string     // ⚠️ No está en contracts.md (pero no afecta)
  roles: string[]   // ⚠️ No está en contracts.md (pero puede ser útil)
  exp: number       // ✅ OK
  iat: number       // ✅ OK
}
```

**Conclusión:** ✅ Compatible. Los campos extra (`name`, `roles`) no causan conflicto.

### 2.5 ACCIÓN REQUERIDA: Simplificar `Artista` vs `ArtistaDto`

**Problema:** `Artista` y `ArtistaDto` son idénticos (duplicación innecesaria).

**Decisión:**
- Eliminar `ArtistaDto` y usar solo `Artista`
- O hacer `ArtistaDto` un alias: `export type ArtistaDto = Artista`

**Campos extra en MVP:**
- `generoMusical` - ⚠️ No está en contracts.md pero puede ser útil
- `redesSociales` - ⚠️ No está en contracts.md, considerar para fase 2

**Implementación Sugerida:**

```typescript
// src/shared/types/artista.ts (modificar)

// Interface principal (alineada con contracts.md)
export interface Artista {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  fechaCreacion: string
  fechaActualizacion?: string
}

// Alias para compatibilidad con código existente
export type ArtistaDto = Artista

// DTO simplificado para listados
export interface ArtistaListItem {
  id: string
  nombreArtistico: string
  imagenUrl?: string
  ciudad?: string
  pais?: string
}

// Request para crear artista (alineado con contracts.md)
export interface CreateArtistaRequest {
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
}

// Request para actualizar artista (futuro)
export interface UpdateArtistaRequest {
  nombreArtistico?: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
}
```

### 2.6 Tabla Resumen de Alineamiento Backend-Frontend

| Tipo TS | DTO C# Equivalente | Alineamiento |
|---------|-------------------|--------------|
| `RegisterRequest` | `RegisterCommand` (Command) | ✅ 100% |
| `RegisterResponse` | `RegisterResponseDto` | ✅ 100% |
| `CreateArtistaRequest` | `CreateArtistaCommand` | ⚠️ Requiere ajuste (eliminar generoMusical/redesSociales) |
| `Artista` | `ArtistaDto` | ⚠️ Requiere ajuste (eliminar generoMusical/redesSociales) |
| `ArtistaListItem` | `ArtistaListDto` | ✅ 100% |

---

## 3. Schemas Zod (`src/shared/schemas/`)

### 3.1 Archivo: `auth.schema.ts` ✅ CASI COMPLETO

#### Schema Existente: `registerSchema`

```typescript
export const registerSchema = z.object({
  email: z
    .string()
    .min(1, "El email es obligatorio")              // ✅ Alineado con VALIDATION_REQUIRED
    .email("Formato de email invalido"),            // ⚠️ Mensaje difiere de contracts.md

  password: z
    .string()
    .min(1, "La contrasena es obligatoria")         // ✅ Alineado
    .min(8, "La contrasena debe tener al menos 8 caracteres"), // ✅ Alineado

  confirmPassword: z
    .string()
    .min(1, "Confirme su contrasena"),              // ✅ Alineado
}).refine((data) => data.password === data.confirmPassword, {
  message: "Las contrasenas no coinciden",          // ✅ Alineado con AUTH_PASSWORD_MISMATCH
  path: ["confirmPassword"],
})
```

#### ACCIÓN: Alinear Mensajes con `contracts.md`

**Según contracts.md (línea 322-326):**

| Campo | Mensaje Zod Actual | Mensaje Esperado | Estado |
|-------|-------------------|------------------|--------|
| email (formato) | "Formato de email invalido" | "Formato de email inválido" | ⚠️ Falta tilde |
| password (min) | "La contrasena debe tener al menos 8 caracteres" | (igual sin tildes) | ✅ OK |
| confirmPassword | "Las contrasenas no coinciden" | (igual sin tildes) | ✅ OK |

**Implementación Sugerida:**

```typescript
// src/shared/schemas/auth.schema.ts (MODIFICAR línea 4)
export const registerSchema = z.object({
  email: z
    .string()
    .min(1, "El email es obligatorio")
    .email("El formato del email no es válido"),  // CAMBIAR: agregar tilde y "El formato"

  password: z
    .string()
    .min(1, "La contraseña es obligatoria")
    .min(8, "La contraseña debe tener al menos 8 caracteres"),

  confirmPassword: z
    .string()
    .min(1, "Confirme su contraseña"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Las contraseñas no coinciden",
  path: ["confirmPassword"],
})
```

### 3.2 Archivo: `artista.schema.ts` ✅ CASI COMPLETO

#### Schema Existente: `createArtistaSchema`

```typescript
export const createArtistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(1, "El nombre artistico es obligatorio")             // ✅ Alineado
    .max(200, "El nombre artistico no puede superar los 200 caracteres"), // ✅ Alineado

  descripcion: z
    .string()
    .max(2000, "La descripcion no puede superar los 2000 caracteres") // ✅ Alineado
    .optional()
    .or(z.literal("")),                                       // ✅ Correcto

  pais: z
    .string()
    .max(100, "El pais no puede superar los 100 caracteres")  // ✅ Alineado
    .optional()
    .or(z.literal("")),

  ciudad: z
    .string()
    .max(100, "La ciudad no puede superar los 100 caracteres") // ✅ Alineado
    .optional()
    .or(z.literal("")),

  imagenUrl: z
    .string()
    .url("Debe ser una URL valida")                           // ✅ Alineado
    .optional()
    .or(z.literal("")),
})
```

#### ACCIÓN: Validar Alineamiento con FluentValidation

**Comparación con Backend (contracts.md líneas 319-332):**

| Campo | Regla Backend | Regla Zod | Alineamiento |
|-------|--------------|-----------|--------------|
| nombreArtistico | `.NotEmpty()` + `.MaximumLength(200)` | `.min(1)` + `.max(200)` | ✅ 100% |
| descripcion | `.MaximumLength(2000)` (opcional) | `.max(2000).optional()` | ✅ 100% |
| pais | `.MaximumLength(100)` (opcional) | `.max(100).optional()` | ✅ 100% |
| ciudad | `.MaximumLength(100)` (opcional) | `.max(100).optional()` | ✅ 100% |
| imagenUrl | `.Must(BeValidUrl).When(...)` | `.url().optional()` | ✅ 100% |

**Conclusión:** ✅ Schema `createArtistaSchema` está 100% alineado con backend.

#### Schema Existente: `artistaSchema`

**Problema:** Incluye campos extra no definidos en contracts.md:
- `generoMusical` (línea 28 en artista.schema.ts)
- `redesSociales` (líneas 29-37)

**Decisión:** Mantener `artistaSchema` para futuras features, pero **NO usarlo** en el flujo de registro-artista. Usar solo `createArtistaSchema`.

### 3.3 Types Inferidos

```typescript
// src/shared/schemas/auth.schema.ts
export type LoginFormData = z.infer<typeof loginSchema>       // ✅ Existe
export type RegisterFormData = z.infer<typeof registerSchema> // ✅ Existe

// src/shared/schemas/artista.schema.ts
export type ArtistaFormData = z.infer<typeof artistaSchema>            // ✅ Existe (no usado en MVP)
export type CreateArtistaFormData = z.infer<typeof createArtistaSchema> // ✅ Existe
```

**Conclusión:** ✅ Types inferidos correctos.

---

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Endpoints API ✅ COMPLETO

**Ya definidos (líneas 60-82 en constants/index.ts):**

```typescript
export const API_ROUTES = {
  auth: {
    register: "/api/auth/register",     // ✅ POST /api/auth/register
    login: "/api/auth/login",           // ✅ POST /api/auth/login
  },
  artistas: {
    base: "/api/artistas",                                  // ✅ POST /api/artistas
    byId: (id: string) => `/api/artistas/${id}`,            // ✅ GET /api/artistas/{id}
    byUserId: (userId: string) => `/api/artistas/by-user/${userId}`, // ✅ GET /api/artistas/by-user/{userId}
  },
} as const
```

**Comparación con contracts.md:**

| Endpoint Backend | Constante Frontend | Estado |
|-----------------|-------------------|--------|
| `POST /api/auth/register` | `API_ROUTES.auth.register` | ✅ OK |
| `POST /api/artistas` | `API_ROUTES.artistas.base` | ✅ OK |
| `GET /api/artistas/{id}` | `API_ROUTES.artistas.byId(id)` | ✅ OK |
| `GET /api/artistas/by-user/{userId}` | `API_ROUTES.artistas.byUserId(userId)` | ✅ OK |

**Conclusión:** ✅ 100% alineado con contracts.md.

### 4.2 Query Keys (React Query) ✅ COMPLETO

**Ya definidos (líneas 8-18 en constants/index.ts):**

```typescript
export const QUERY_KEYS = {
  auth: {
    currentUser: ["auth", "current-user"] as const,          // ✅ OK
  },

  artistas: {
    all: ["artistas"] as const,                              // ✅ OK
    byId: (id: string) => ["artistas", id] as const,         // ✅ OK (para GET /api/artistas/{id})
    byUserId: (userId: string) => ["artistas", "user", userId] as const, // ✅ OK (para GET /api/artistas/by-user/{userId})
  },
} as const
```

**Uso esperado en features:**

| Query | Key | Endpoint Asociado |
|-------|-----|------------------|
| Obtener artista por ID | `QUERY_KEYS.artistas.byId(id)` | `GET /api/artistas/{id}` |
| Obtener artista del usuario autenticado | `QUERY_KEYS.artistas.byUserId(userId)` | `GET /api/artistas/by-user/{userId}` |
| Crear artista | Invalidar `QUERY_KEYS.artistas.all` | Tras `POST /api/artistas` |

**Conclusión:** ✅ Estructura correcta y completa.

### 4.3 App Routes (Frontend Navigation) ✅ COMPLETO

**Ya definidos (líneas 85-99 en constants/index.ts):**

```typescript
export const APP_ROUTES = {
  auth: {
    register: "/auth/register",                       // ✅ Admin: Registro público
    login: "/auth/login",                             // ✅ Admin: Login público
  },
  artista: {
    crearPerfil: "/artista/perfil/crear",             // ✅ Admin: Crear perfil artista (autenticado)
  },
  dashboard: "/dashboard",                            // ✅ Admin: Dashboard principal
  landing: {
    home: "/",                                        // ✅ Landing: Home
    artistaById: (id: string) => `/artistas/${id}`,   // ✅ Landing: Perfil público artista
  },
} as const
```

**Comparación con contracts.md (líneas 212-222):**

| Ruta Esperada | Constante | Auth | Estado |
|--------------|-----------|------|--------|
| `/auth/register` (Admin) | `APP_ROUTES.auth.register` | ❌ Público | ✅ OK |
| `/auth/login` (Admin) | `APP_ROUTES.auth.login` | ❌ Público | ✅ OK |
| `/artista/perfil/crear` (Admin) | `APP_ROUTES.artista.crearPerfil` | ✅ Requiere auth | ✅ OK |
| `/dashboard` (Admin) | `APP_ROUTES.dashboard` | ✅ Requiere perfil Artista | ✅ OK |
| `/artistas/{id}` (Landing) | `APP_ROUTES.landing.artistaById(id)` | ❌ Público | ✅ OK |

**Conclusión:** ✅ 100% alineado con contracts.md.

### 4.4 Validations Limits

**Ya definidos (líneas 124-130 en constants/index.ts):**

```typescript
export const VALIDATION = {
  TITULO_MAX: 200,
  DESCRIPCION_MAX: 5000,
  IMPORTE_MIN: 100,
  IMPORTE_MAX: 1000000,
  PASSWORD_MIN: 6,              // ⚠️ CONFLICTO: Backend usa 8 (contracts.md línea 324)
} as const
```

#### ACCIÓN REQUERIDA: Corregir `PASSWORD_MIN`

**Problema:** Backend requiere mínimo 8 caracteres (contracts.md), frontend define 6.

**Implementación Sugerida:**

```typescript
// src/shared/constants/index.ts (MODIFICAR línea 129)
export const VALIDATION = {
  // Artista
  NOMBRE_ARTISTICO_MAX: 200,        // AGREGAR: Para nombreArtistico
  DESCRIPCION_ARTISTA_MAX: 2000,    // AGREGAR: Para descripcion artista
  PAIS_MAX: 100,                    // AGREGAR
  CIUDAD_MAX: 100,                  // AGREGAR

  // Campania (existentes)
  TITULO_MAX: 200,
  DESCRIPCION_MAX: 5000,
  IMPORTE_MIN: 100,
  IMPORTE_MAX: 1000000,

  // Auth
  PASSWORD_MIN: 8,                  // CAMBIAR: de 6 a 8
  EMAIL_MAX: 256,                   // AGREGAR (estándar)
} as const
```

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Archivo: `error-messages.ts` ✅ CASI COMPLETO

#### Códigos Ya Definidos

**Comparación con contracts.md (líneas 44-53, 96-105, 136-141, 174-178):**

| ErrorCode (Backend) | Definido en Frontend | Mensaje | Estado |
|---------------------|---------------------|---------|--------|
| `VALIDATION_REQUIRED` | ✅ Sí | "Este campo es obligatorio" | ✅ OK |
| `AUTH_EMAIL_INVALID` | ✅ Sí | "El formato del email no es valido" | ⚠️ Falta tilde |
| `AUTH_EMAIL_EXISTS` | ✅ Sí | "Este email ya esta registrado..." | ⚠️ Faltan tildes |
| `AUTH_PASSWORD_MIN_LENGTH` | ✅ Sí | "La contrasena debe tener al menos 8 caracteres" | ⚠️ Falta tilde |
| `AUTH_PASSWORD_MISMATCH` | ✅ Sí | "Las contrasenas no coinciden" | ⚠️ Falta tilde |
| `AUTH_UNAUTHORIZED` | ✅ Sí | "Tu sesion ha expirado..." | ⚠️ Falta tilde |
| `ARTISTA_ALREADY_EXISTS` | ✅ Sí | "Ya tienes un perfil de artista creado" | ✅ OK |
| `ARTISTA_NOT_FOUND` | ✅ Sí | "Artista no encontrado" | ✅ OK |
| `ARTISTA_NOMBRE_MAX_LENGTH` | ✅ Sí | "El nombre artistico no puede superar los 200 caracteres" | ⚠️ Falta tilde |
| `ARTISTA_DESC_MAX_LENGTH` | ✅ Sí | "La descripcion no puede superar los 2000 caracteres" | ⚠️ Falta tilde |
| `ARTISTA_PAIS_MAX_LENGTH` | ✅ Sí | "El pais no puede superar los 100 caracteres" | ⚠️ Falta tilde |
| `ARTISTA_CIUDAD_MAX_LENGTH` | ✅ Sí | "La ciudad no puede superar los 100 caracteres" | ✅ OK |
| `ARTISTA_IMAGEN_URL_INVALIDA` | ✅ Sí | "La URL de la imagen no es valida..." | ⚠️ Falta tilde |
| `ERROR_UNEXPECTED` | ✅ Sí | "Ha ocurrido un error inesperado..." | ✅ OK |

#### ACCIÓN OPCIONAL: Corregir Tildes

Los mensajes actuales omiten tildes (probablemente para evitar encoding issues). Decidir si:
- **Opción A:** Mantener sin tildes (consistencia con código existente)
- **Opción B:** Agregar tildes según contracts.md (mejor UX para usuarios)

**Recomendación:** Mantener sin tildes por consistencia, salvo que se actualice todo el proyecto.

### 5.2 Función: `getErrorMessage` ✅ COMPLETO

```typescript
export const getErrorMessage = (errorCode: string | undefined | null): string => {
  if (!errorCode) {
    return ERROR_MESSAGES.ERROR_UNEXPECTED
  }
  return ERROR_MESSAGES[errorCode] || ERROR_MESSAGES.ERROR_UNEXPECTED
}
```

**Uso esperado:**
```typescript
// En componentes React
const { error } = useMutation(...)
const errorMessage = getErrorMessage(error?.response?.data?.messages?.[0]?.errorCode)
toast.error(errorMessage)
```

**Conclusión:** ✅ Implementación correcta.

### 5.3 Archivo: `mappers.ts` (Opcional)

**Estado:** Actualmente existe pero no tiene mapper específico para artistas.

**ACCIÓN SUGERIDA:** Agregar mapper para convertir `Artista` a `ArtistaListItem`:

```typescript
// src/shared/utils/mappers.ts (AGREGAR)

import type { Artista, ArtistaListItem } from '../types'

export const artistaToListItem = (artista: Artista): ArtistaListItem => ({
  id: artista.id,
  nombreArtistico: artista.nombreArtistico,
  imagenUrl: artista.imagenUrl,
  ciudad: artista.ciudad,
  pais: artista.pais,
})

export const artistasToListItems = (artistas: Artista[]): ArtistaListItem[] =>
  artistas.map(artistaToListItem)
```

**Uso:** Para transformar respuestas completas de artistas en listas simplificadas.

---

## 6. Archivos Existentes (No Requieren Cambios)

### 6.1 Estructura Actual

```
src/shared/
├── types/
│   ├── api.ts                  ✅ OK (ServiceResponse genérico)
│   ├── auth.ts                 ⚠️ MODIFICAR (mover CreateArtistaRequest)
│   ├── artista.ts              ⚠️ MODIFICAR (simplificar Artista, eliminar campos extra)
│   ├── campania.ts             ✅ OK (no afectado)
│   ├── reward.ts               ✅ OK (no afectado)
│   ├── backing.ts              ✅ OK (no afectado)
│   └── index.ts                ✅ OK
├── schemas/
│   ├── auth.schema.ts          ⚠️ MODIFICAR (mensaje email con tilde)
│   ├── artista.schema.ts       ✅ OK (createArtistaSchema perfecto)
│   ├── campania.schema.ts      ✅ OK (no afectado)
│   ├── reward.schema.ts        ✅ OK (no afectado)
│   ├── backing.schema.ts       ✅ OK (no afectado)
│   └── index.ts                ✅ OK
├── constants/
│   └── index.ts                ⚠️ MODIFICAR (PASSWORD_MIN, agregar VALIDATION artista)
├── utils/
│   ├── cn.ts                   ✅ OK
│   ├── format.ts               ✅ OK
│   ├── error-messages.ts       ⚠️ MODIFICAR (opcional: tildes)
│   ├── mappers.ts              ⚠️ MODIFICAR (agregar artistaToListItem)
│   └── index.ts                ✅ OK
└── index.ts                    ✅ OK
```

### 6.2 Archivos Críticos para `registro-artista`

| Archivo | Usado en Landing | Usado en Admin | Estado |
|---------|------------------|----------------|--------|
| `types/auth.ts` | ❌ No | ✅ Sí (Register) | ⚠️ Ajustar |
| `types/artista.ts` | ✅ Sí (Perfil público) | ✅ Sí (Crear perfil) | ⚠️ Ajustar |
| `schemas/auth.schema.ts` | ❌ No | ✅ Sí (Register form) | ⚠️ Ajustar |
| `schemas/artista.schema.ts` | ❌ No | ✅ Sí (Crear perfil form) | ✅ OK |
| `constants/index.ts` | ✅ Sí (rutas) | ✅ Sí (API + rutas) | ⚠️ Ajustar |
| `utils/error-messages.ts` | ✅ Sí | ✅ Sí | ✅ OK |

---

## 7. Integración Landing y Admin

### 7.1 Landing (Vite + React)

**Features que consumen `src/shared/`:**

#### Feature: `artistas` (Perfil Público)

```typescript
// src/web/src/features/artistas/services/artista.service.ts
import { API_ROUTES, type Artista } from '@/shared'

export const getArtistaById = async (id: string): Promise<Artista> => {
  const response = await fetch(API_ROUTES.artistas.byId(id))
  const data: ServiceResponse<Artista> = await response.json()
  return data.data
}
```

#### Componente: `ArtistaProfile.tsx`

```typescript
// src/web/src/features/artistas/components/ArtistaProfile.tsx
import { useQuery } from '@tanstack/react-query'
import { QUERY_KEYS, type Artista } from '@/shared'

export const ArtistaProfile = ({ artistaId }: { artistaId: string }) => {
  const { data: artista, isLoading } = useQuery({
    queryKey: QUERY_KEYS.artistas.byId(artistaId),
    queryFn: () => getArtistaById(artistaId),
  })

  if (isLoading) return <Spinner />
  if (!artista) return <NotFound />

  return (
    <div>
      <h1>{artista.nombreArtistico}</h1>
      <p>{artista.descripcion}</p>
      {/* ... */}
    </div>
  )
}
```

**Tipos usados:**
- ✅ `Artista` (perfil completo)
- ✅ `ArtistaListItem` (para listados de artistas)
- ✅ `API_ROUTES.artistas.byId(id)`
- ✅ `QUERY_KEYS.artistas.byId(id)`
- ✅ `APP_ROUTES.landing.artistaById(id)`

### 7.2 Admin (Next.js 14)

**Features que consumen `src/shared/`:**

#### Feature: `auth` (Registro)

```typescript
// src/admin/src/features/auth/hooks/useRegister.ts
import { useMutation } from '@tanstack/react-query'
import { API_ROUTES, type RegisterRequest, type RegisterResponse, getErrorMessage } from '@/shared'

export const useRegister = () => {
  return useMutation({
    mutationFn: async (data: RegisterRequest): Promise<RegisterResponse> => {
      const response = await fetch(API_ROUTES.auth.register, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      })

      if (!response.ok) {
        const errorData = await response.json()
        throw new Error(errorData.messages[0].errorCode)
      }

      const result: ServiceResponse<RegisterResponse> = await response.json()
      return result.data
    },
    onSuccess: (data) => {
      // Guardar token en localStorage
      localStorage.setItem('token', data.token)
    },
    onError: (error: Error) => {
      const message = getErrorMessage(error.message)
      toast.error(message)
    },
  })
}
```

#### Componente: `RegisterForm.tsx`

```typescript
// src/admin/src/features/auth/components/RegisterForm.tsx
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { registerSchema, type RegisterFormData, APP_ROUTES } from '@/shared'

export const RegisterForm = () => {
  const { register: registerMutation } = useRegister()
  const router = useRouter()

  const form = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  })

  const onSubmit = async (data: RegisterFormData) => {
    await registerMutation.mutateAsync(data)
    // Redirigir a crear perfil artista
    router.push(APP_ROUTES.artista.crearPerfil)
  }

  return (
    <form onSubmit={form.handleSubmit(onSubmit)}>
      <Input {...form.register('email')} />
      <Input type="password" {...form.register('password')} />
      <Input type="password" {...form.register('confirmPassword')} />
      <Button type="submit">Registrarse</Button>
    </form>
  )
}
```

**Tipos usados:**
- ✅ `RegisterRequest`
- ✅ `RegisterResponse`
- ✅ `RegisterFormData` (inferido de `registerSchema`)
- ✅ `registerSchema` (validación)
- ✅ `API_ROUTES.auth.register`
- ✅ `APP_ROUTES.artista.crearPerfil`
- ✅ `getErrorMessage()`

#### Feature: `artista` (Crear Perfil)

```typescript
// src/admin/src/features/artista/hooks/useCreateArtista.ts
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { API_ROUTES, QUERY_KEYS, type CreateArtistaRequest, type Artista, getErrorMessage } from '@/shared'

export const useCreateArtista = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateArtistaRequest): Promise<Artista> => {
      const token = localStorage.getItem('token')
      const response = await fetch(API_ROUTES.artistas.base, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(data),
      })

      if (!response.ok) {
        const errorData = await response.json()
        throw new Error(errorData.messages[0].errorCode)
      }

      const result: ServiceResponse<Artista> = await response.json()
      return result.data
    },
    onSuccess: (data) => {
      // Invalidar cache de artistas
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.all })
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.artistas.byUserId(data.userId) })
    },
    onError: (error: Error) => {
      const message = getErrorMessage(error.message)
      toast.error(message)
    },
  })
}
```

**Tipos usados:**
- ✅ `CreateArtistaRequest`
- ✅ `Artista` (respuesta)
- ✅ `CreateArtistaFormData` (inferido de `createArtistaSchema`)
- ✅ `createArtistaSchema` (validación)
- ✅ `API_ROUTES.artistas.base`
- ✅ `QUERY_KEYS.artistas.all`
- ✅ `QUERY_KEYS.artistas.byUserId(userId)`
- ✅ `APP_ROUTES.dashboard`
- ✅ `getErrorMessage()`

---

## 8. Dependencias

### 8.1 Ya Instaladas

- ✅ `zod` - Validación schemas
- ✅ `@tanstack/react-query` - Data fetching (Landing y Admin)
- ✅ `react-hook-form` - Formularios (Landing y Admin)
- ✅ `@hookform/resolvers` - Integración Zod + React Hook Form

### 8.2 Ninguna Dependencia Adicional Requerida

**Conclusión:** Todo el código compartido usa solo las dependencias ya instaladas en el proyecto.

---

## 9. Notas de Implementación

### 9.1 Convenciones de Naming

- **Types:** PascalCase (`Artista`, `RegisterRequest`)
- **Interfaces vs Types:** Usar `interface` para objetos, `type` para unions/aliases
- **Schemas Zod:** camelCase con sufijo `Schema` (`registerSchema`, `createArtistaSchema`)
- **Types inferidos:** PascalCase con sufijo `FormData` (`RegisterFormData`)
- **Constantes:** UPPER_SNAKE_CASE para valores (`API_ROUTES`, `QUERY_KEYS`)

### 9.2 Manejo de Errores

**Patrón recomendado en hooks:**

```typescript
import { getErrorMessage } from '@/shared'

const mutation = useMutation({
  mutationFn: async (data) => {
    const response = await fetch(endpoint, { ... })

    if (!response.ok) {
      const errorData: ServiceResponse<null> = await response.json()
      // Throw el errorCode (no el mensaje)
      throw new Error(errorData.messages[0].errorCode)
    }

    const result: ServiceResponse<T> = await response.json()
    return result.data
  },
  onError: (error: Error) => {
    // Traducir errorCode a mensaje user-friendly
    const message = getErrorMessage(error.message)
    toast.error(message)
  },
})
```

### 9.3 Validación Client-Side vs Server-Side

| Campo | Zod (Client) | FluentValidation (Server) | ¿Sincronizado? |
|-------|-------------|--------------------------|----------------|
| email formato | `.email()` | `.EmailAddress()` | ✅ Sí |
| password min | `.min(8)` | `.MinimumLength(8)` | ✅ Sí |
| nombreArtistico max | `.max(200)` | `.MaximumLength(200)` | ✅ Sí |
| imagenUrl formato | `.url()` | `.Must(BeValidUrl)` | ✅ Sí |

---

## 10. Checklist de Implementación

### 10.1 Ajustes Requeridos (Prioridad Alta)

- [ ] **`types/auth.ts`:** Mover `CreateArtistaRequest` a `artista.ts` (opcional, compatibilidad con código existente)
- [ ] **`types/artista.ts`:** Simplificar `Artista` (eliminar `generoMusical`, `redesSociales` o mantener para futuros features)
- [ ] **`types/artista.ts`:** Convertir `ArtistaDto` en alias de `Artista`
- [ ] **`schemas/auth.schema.ts`:** Cambiar mensaje email a "El formato del email no es válido" (opcional, consistencia)
- [ ] **`constants/index.ts`:** Cambiar `PASSWORD_MIN` de 6 a 8
- [ ] **`constants/index.ts`:** Agregar constantes `NOMBRE_ARTISTICO_MAX`, `DESCRIPCION_ARTISTA_MAX`, etc.
- [ ] **`utils/mappers.ts`:** Agregar función `artistaToListItem()`

### 10.2 Validaciones (Prioridad Media)

- [ ] Verificar que schemas Zod replican 100% reglas FluentValidation
- [ ] Revisar si agregar tests unitarios para schemas

### 10.3 Opcional (Prioridad Baja)

- [ ] Corregir tildes en `error-messages.ts` (si se decide)
- [ ] Agregar JSDoc comments a types complejos

---

## 11. Resumen Ejecutivo

### ✅ Qué Ya Existe y Funciona

- ✅ Estructura de carpetas `src/shared/` completa
- ✅ Types TypeScript alineados al 90% con backend
- ✅ Schemas Zod con validaciones correctas
- ✅ Constantes API_ROUTES, QUERY_KEYS, APP_ROUTES
- ✅ Error messages mapeados a códigos backend
- ✅ ServiceResponse<T> genérico para wrapping

### ⚠️ Qué Requiere Ajustes Menores

- ⚠️ `PASSWORD_MIN` debe cambiar de 6 a 8
- ⚠️ Agregar constantes VALIDATION para artista
- ⚠️ Simplificar `Artista` (opcional, eliminar campos no MVP)
- ⚠️ Agregar mapper `artistaToListItem` en utils

### 🚀 Siguiente Acción

1. Implementar ajustes listados en **Checklist 10.1**
2. Validar con backend que contracts.md está actualizado
3. Proceder con implementación de features Landing y Admin

---

**Nota Final:** Este plan documenta el estado actual de `src/shared/` y proporciona acciones concretas para alinear 100% con `contracts.md`. La mayoría del código ya existe y es correcto, solo requiere ajustes menores para MVP.
