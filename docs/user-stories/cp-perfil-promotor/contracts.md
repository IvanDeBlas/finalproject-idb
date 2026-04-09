# Contratos: Perfil de Promotor

> **Feature:** cp-perfil-promotor (US-CP-01)
> **Ultima actualizacion:** 2026-02-25
> **Modulo Backend:** Crowdpromotion
> **Depende de:** Ninguna (punto de entrada al modulo Crowdpromotion)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### POST /api/crowdpromotion/promotor

**Descripcion:** Crea el perfil de promotor del usuario autenticado. Un usuario solo puede tener un perfil de promotor (un registro por UserId). Al crear el perfil, el sistema tambien crea automaticamente una `PromotorWallet` en EUR con saldos en cero. El `UserId` se obtiene exclusivamente del token JWT; no se acepta en el body.

**Autorizacion:** Bearer JWT - Cualquier usuario autenticado (rol Fan o cualquier otro)

**Request Body:**
```json
{
  "nombrePublico": "DJ Marketing Pro",
  "tipoPromotorId": 2,
  "emailContacto": "contacto@djmarketing.com",
  "urlSitioWeb": "https://djmarketing.com",
  "urlInstagram": "https://instagram.com/djmarketing",
  "urlTikTok": "https://tiktok.com/@djmarketing",
  "urlYouTube": null,
  "urlTwitter": null
}
```

**Nota sobre campos opcionales:** `emailContacto`, `urlSitioWeb`, `urlInstagram`, `urlTikTok`, `urlYouTube` y `urlTwitter` son opcionales. Enviar `null` o simplemente omitirlos tiene el mismo efecto.

**Response 201 Created:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombrePublico": "DJ Marketing Pro",
    "tipoPromotorNombre": "Influencer",
    "esActivo": true,
    "fechaCreacion": "2026-02-25T10:00:00Z"
  },
  "messages": [
    { "message": "Perfil de promotor creado", "errorCode": "0001" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El nombre publico es obligatorio | `nombrePublico` vacio o no enviado |
| 400 | 1011 | El nombre debe tener al menos 3 caracteres | `nombrePublico` con menos de 3 caracteres |
| 400 | 1002 | El nombre publico no puede superar los 200 caracteres | `nombrePublico` > 200 chars |
| 400 | 1001 | El tipo de promotor es obligatorio | `tipoPromotorId` no enviado o 0 |
| 400 | 1010 | El tipo de promotor no existe | `tipoPromotorId` no existe en `MaestraTipoPromotor` |
| 400 | 1003 | El email de contacto no tiene formato valido | `emailContacto` con formato de email invalido |
| 400 | 1002 | El email de contacto no puede superar los 200 caracteres | `emailContacto` > 200 chars |
| 400 | 1013 | La URL del sitio web no tiene formato valido | `urlSitioWeb` con formato URL invalido |
| 400 | 1002 | La URL del sitio web no puede superar los 300 caracteres | `urlSitioWeb` > 300 chars |
| 400 | 1013 | La URL de Instagram no tiene formato valido | `urlInstagram` con formato URL invalido |
| 400 | 1002 | La URL de Instagram no puede superar los 300 caracteres | `urlInstagram` > 300 chars |
| 400 | 1013 | La URL de TikTok no tiene formato valido | `urlTikTok` con formato URL invalido |
| 400 | 1002 | La URL de TikTok no puede superar los 300 caracteres | `urlTikTok` > 300 chars |
| 400 | 1013 | La URL de YouTube no tiene formato valido | `urlYouTube` con formato URL invalido |
| 400 | 1002 | La URL de YouTube no puede superar los 300 caracteres | `urlYouTube` > 300 chars |
| 400 | 1013 | La URL de Twitter/X no tiene formato valido | `urlTwitter` con formato URL invalido |
| 400 | 1002 | La URL de Twitter/X no puede superar los 300 caracteres | `urlTwitter` > 300 chars |
| 400 | 4018 | Ya tienes un perfil de promotor creado | Ya existe un `Promotor` con el mismo `UserId` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 500 | 5000 | Error inesperado al crear el perfil de promotor | Excepcion no controlada |

---

### GET /api/crowdpromotion/promotor/me

**Descripcion:** Devuelve el perfil completo del promotor autenticado, incluyendo estadisticas de programas activos y comisiones ganadas calculadas desde `PromotorWallet`. El `UserId` se obtiene del token JWT.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombrePublico": "DJ Marketing Pro",
    "tipoPromotorId": 2,
    "tipoPromotorNombre": "Influencer",
    "emailContacto": "contacto@djmarketing.com",
    "urlSitioWeb": "https://djmarketing.com",
    "urlInstagram": "https://instagram.com/djmarketing",
    "urlTikTok": "https://tiktok.com/@djmarketing",
    "urlYouTube": null,
    "urlTwitter": null,
    "esActivo": true,
    "fechaCreacion": "2026-02-25T10:00:00Z",
    "totalProgramasActivos": 3,
    "totalComisionesGanadas": 150.50,
    "monedaComisiones": "EUR"
  },
  "messages": []
}
```

**Notas sobre campos calculados:**
- `totalProgramasActivos`: COUNT de `PromoProgramaPromotor` donde `PromotorId == id` y el programa esta activo. Devuelve `0` si no hay programas.
- `totalComisionesGanadas`: `TotalGanado` de la `PromotorWallet` con `MonedaId == 1` (EUR). Devuelve `0` si la wallet existe pero no tiene ganancias.
- `monedaComisiones`: Codigo ISO de la moneda de la wallet principal. Siempre `"EUR"` en el MVP.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | 5000 | Error inesperado al obtener el perfil de promotor | Excepcion no controlada |

---

### PUT /api/crowdpromotion/promotor/me

**Descripcion:** Actualiza el perfil del promotor autenticado. Solo se pueden modificar `nombrePublico`, `emailContacto` y las URLs de redes sociales. El campo `tipoPromotorId` no es editable despues de la creacion. Al actualizar con exito, se registra `FechaActualizacion = DateTime.UtcNow`. El `UserId` se obtiene del token JWT.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente

**Request Body:**
```json
{
  "nombrePublico": "DJ Marketing Pro (Updated)",
  "emailContacto": "nuevo@djmarketing.com",
  "urlSitioWeb": "https://djmarketing.com",
  "urlInstagram": "https://instagram.com/djmarketing",
  "urlTikTok": "https://tiktok.com/@djmarketing",
  "urlYouTube": "https://youtube.com/@djmarketing",
  "urlTwitter": null
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombrePublico": "DJ Marketing Pro (Updated)",
    "fechaActualizacion": "2026-02-25T09:00:00Z"
  },
  "messages": [
    { "message": "Perfil actualizado", "errorCode": "0002" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El nombre publico es obligatorio | `nombrePublico` vacio |
| 400 | 1011 | El nombre debe tener al menos 3 caracteres | `nombrePublico` con menos de 3 caracteres |
| 400 | 1002 | El nombre publico no puede superar los 200 caracteres | `nombrePublico` > 200 chars |
| 400 | 1003 | El email de contacto no tiene formato valido | `emailContacto` con formato invalido |
| 400 | 1002 | El email de contacto no puede superar los 200 caracteres | `emailContacto` > 200 chars |
| 400 | 1013 | La URL del sitio web no tiene formato valido | `urlSitioWeb` invalida |
| 400 | 1002 | La URL del sitio web no puede superar los 300 caracteres | `urlSitioWeb` > 300 chars |
| 400 | 1013 | La URL de Instagram no tiene formato valido | `urlInstagram` invalida |
| 400 | 1002 | La URL de Instagram no puede superar los 300 caracteres | `urlInstagram` > 300 chars |
| 400 | 1013 | La URL de TikTok no tiene formato valido | `urlTikTok` invalida |
| 400 | 1002 | La URL de TikTok no puede superar los 300 caracteres | `urlTikTok` > 300 chars |
| 400 | 1013 | La URL de YouTube no tiene formato valido | `urlYouTube` invalida |
| 400 | 1002 | La URL de YouTube no puede superar los 300 caracteres | `urlYouTube` > 300 chars |
| 400 | 1013 | La URL de Twitter/X no tiene formato valido | `urlTwitter` invalida |
| 400 | 1002 | La URL de Twitter/X no puede superar los 300 caracteres | `urlTwitter` > 300 chars |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | 5000 | Error inesperado al actualizar el perfil de promotor | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/promotor/me/desactivar

**Descripcion:** Desactiva el perfil del promotor autenticado. Establece `EsActivo = false` y da de baja automatica al promotor de todos sus programas activos (`PromoProgramaPromotor`). La operacion es logica: el registro `Promotor` no se elimina. El `UserId` se obtiene del token JWT.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente y activo

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "esActivo": false,
    "programasDadosDeBaja": 2
  },
  "messages": [
    { "message": "Perfil desactivado", "errorCode": "0002" }
  ]
}
```

**Nota sobre `programasDadosDeBaja`:** Numero de entradas en `PromoProgramaPromotor` que fueron desactivadas como resultado de esta operacion. Puede ser `0` si el promotor no estaba inscrito en ningun programa activo.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 400 | 4019 | El perfil de promotor ya esta desactivado | `EsActivo == false` en el momento de la solicitud |
| 500 | 5000 | Error inesperado al desactivar el perfil de promotor | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/crowdpromotion/promotor` | Bearer JWT | Cualquier rol autenticado | UserId extraido del token; un UserId = un Promotor |
| `GET /api/crowdpromotion/promotor/me` | Bearer JWT | Promotor existente | Retorna 404 si no tiene perfil de promotor |
| `PUT /api/crowdpromotion/promotor/me` | Bearer JWT | Promotor existente | `tipoPromotorId` no editable; retorna 404 si no tiene perfil |
| `PATCH /api/crowdpromotion/promotor/me/desactivar` | Bearer JWT | Promotor existente y activo | Retorna 400 si ya esta desactivado |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "usuario@example.com",
  "role": "Fan",
  "exp": 1740873600,
  "iat": 1740787200
}
```

**Nota:** El `PromotorId` no se incluye en el token. El backend resuelve el promotor consultando `Promotor.UserId == sub` en cada request. El `FanProfileId` tampoco se incluye en el token; el backend lo resuelve consultando `FanProfile` por `UserId` al momento de crear el perfil de promotor.

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesUsers

### Proteccion de Rutas Frontend

| Ruta | App | Auth | Redirect | Notas |
|------|-----|------|----------|-------|
| `/promotor/registro` | Landing | Bearer JWT | `/auth/login` | Si ya tiene perfil, redirigir a `/promotor/dashboard` |
| `/promotor/dashboard` | Landing | Bearer JWT | `/auth/login` | Requiere perfil de promotor existente |
| `/promotor/perfil` | Landing | Bearer JWT | `/auth/login` | Requiere perfil de promotor existente |

---

## DTOs / Types

### Backend (C#)

#### DTOs de Request

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromotorRequestDto.cs
public class CreatePromotorRequestDto
{
    /// <summary>Requerido. Min 3, max 200 chars.</summary>
    public string NombrePublico { get; set; } = null!;

    /// <summary>Requerido. Debe existir en MaestraTipoPromotor (1-4).</summary>
    public int TipoPromotorId { get; set; }

    /// <summary>Opcional. Formato email valido. Max 200 chars.</summary>
    public string? EmailContacto { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlSitioWeb { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlInstagram { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlTikTok { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlYouTube { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlTwitter { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromotorRequestDto.cs
public class UpdatePromotorRequestDto
{
    /// <summary>Requerido. Min 3, max 200 chars.</summary>
    public string NombrePublico { get; set; } = null!;

    /// <summary>Opcional. Formato email valido. Max 200 chars.</summary>
    public string? EmailContacto { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlSitioWeb { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlInstagram { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlTikTok { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlYouTube { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 300 chars.</summary>
    public string? UrlTwitter { get; set; }
}
```

#### DTOs de Response

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorCreatedResultDto.cs
public class PromotorCreatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    /// <summary>Nombre resuelto desde MaestraTipoPromotor.</summary>
    public string TipoPromotorNombre { get; set; } = null!;
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDto.cs
public class PromotorDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    /// <summary>Nombre resuelto desde MaestraTipoPromotor.</summary>
    public string TipoPromotorNombre { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    /// <summary>COUNT de PromoProgramaPromotor activos.</summary>
    public int TotalProgramasActivos { get; set; }
    /// <summary>TotalGanado de la PromotorWallet en EUR.</summary>
    public decimal TotalComisionesGanadas { get; set; }
    /// <summary>Codigo ISO de la moneda principal. Siempre "EUR" en MVP.</summary>
    public string MonedaComisiones { get; set; } = "EUR";
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorUpdatedResultDto.cs
public class PromotorUpdatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public DateTime FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDesactivadoResultDto.cs
public class PromotorDesactivadoResultDto
{
    public Guid Id { get; set; }
    public bool EsActivo { get; set; }
    /// <summary>Numero de PromoProgramaPromotor desactivados.</summary>
    public int ProgramasDadosDeBaja { get; set; }
}
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdpromotion.ts

// --- Requests ---

export interface CreatePromotorRequest {
  nombrePublico: string;       // min 3, max 200 chars
  tipoPromotorId: number;      // 1-4, must exist in MaestraTipoPromotor
  emailContacto?: string;      // optional, valid email, max 200 chars
  urlSitioWeb?: string;        // optional, valid URL, max 300 chars
  urlInstagram?: string;       // optional, valid URL, max 300 chars
  urlTikTok?: string;          // optional, valid URL, max 300 chars
  urlYouTube?: string;         // optional, valid URL, max 300 chars
  urlTwitter?: string;         // optional, valid URL, max 300 chars
}

export interface UpdatePromotorRequest {
  nombrePublico: string;       // min 3, max 200 chars
  emailContacto?: string;      // optional, valid email, max 200 chars
  urlSitioWeb?: string;        // optional, valid URL, max 300 chars
  urlInstagram?: string;       // optional, valid URL, max 300 chars
  urlTikTok?: string;          // optional, valid URL, max 300 chars
  urlYouTube?: string;         // optional, valid URL, max 300 chars
  urlTwitter?: string;         // optional, valid URL, max 300 chars
}

// --- Results ---

export interface PromotorCreatedResult {
  id: string;                   // Guid as string
  nombrePublico: string;
  tipoPromotorNombre: string;
  esActivo: boolean;
  fechaCreacion: string;        // ISO datetime string
}

export interface Promotor {
  id: string;                   // Guid as string
  nombrePublico: string;
  tipoPromotorId: number;
  tipoPromotorNombre: string;
  emailContacto: string | null;
  urlSitioWeb: string | null;
  urlInstagram: string | null;
  urlTikTok: string | null;
  urlYouTube: string | null;
  urlTwitter: string | null;
  esActivo: boolean;
  fechaCreacion: string;        // ISO datetime string
  totalProgramasActivos: number;
  totalComisionesGanadas: number;
  monedaComisiones: string;     // e.g. "EUR"
}

export interface PromotorUpdatedResult {
  id: string;
  nombrePublico: string;
  fechaActualizacion: string;   // ISO datetime string
}

export interface PromotorDesactivadoResult {
  id: string;
  esActivo: boolean;
  programasDadosDeBaja: number;
}

// --- Maestras ---

export interface TipoPromotor {
  id: number;
  nombre: string;
  descripcion: string;
}
```

---

## Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| nombrePublico | requerido | `.NotEmpty().WithMessage("El nombre publico es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El nombre publico es obligatorio')` |
| nombrePublico | min 3 chars | `.MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres").WithErrorCode(Validation_MinLength)` | `.min(3, 'El nombre debe tener al menos 3 caracteres')` |
| nombrePublico | max 200 chars | `.MaximumLength(200).WithMessage("Maximo 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'Maximo 200 caracteres')` |
| tipoPromotorId | requerido (solo Create) | `.NotEmpty().WithMessage("El tipo de promotor es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El tipo de promotor es obligatorio')` |
| tipoPromotorId | debe existir en maestra (solo Create) | `.MustAsync(ExistInMaestra).WithMessage("El tipo de promotor no existe").WithErrorCode(Validation_ForeignKeyNotFound)` | Validado por el select (opciones fijas del seed) |
| emailContacto | formato email cuando presente | `.EmailAddress().When(x => !string.IsNullOrEmpty(x.EmailContacto)).WithMessage("El email de contacto no tiene formato valido").WithErrorCode(Validation_InvalidEmail)` | `.email('El email de contacto no tiene formato valido').optional()` |
| emailContacto | max 200 chars | `.MaximumLength(200).When(x => !string.IsNullOrEmpty(x.EmailContacto)).WithMessage("Maximo 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'Maximo 200 caracteres').optional()` |
| urlSitioWeb | formato URL cuando presente | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlSitioWeb)).WithMessage("La URL del sitio web no tiene formato valido").WithErrorCode(Validation_InvalidUrl)` | `.url('La URL del sitio web no tiene formato valido').optional()` |
| urlSitioWeb | max 300 chars | `.MaximumLength(300).When(x => !string.IsNullOrEmpty(x.UrlSitioWeb)).WithMessage("Maximo 300 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(300, 'Maximo 300 caracteres').optional()` |
| urlInstagram | formato URL cuando presente | `.Must(BeAValidUrl).When(...).WithMessage("La URL de Instagram no tiene formato valido").WithErrorCode(Validation_InvalidUrl)` | `.url('La URL de Instagram no tiene formato valido').optional()` |
| urlInstagram | max 300 chars | `.MaximumLength(300).When(...).WithErrorCode(Validation_MaxLength)` | `.max(300, 'Maximo 300 caracteres').optional()` |
| urlTikTok | formato URL cuando presente | `.Must(BeAValidUrl).When(...).WithErrorCode(Validation_InvalidUrl)` | `.url('La URL de TikTok no tiene formato valido').optional()` |
| urlTikTok | max 300 chars | `.MaximumLength(300).When(...).WithErrorCode(Validation_MaxLength)` | `.max(300, 'Maximo 300 caracteres').optional()` |
| urlYouTube | formato URL cuando presente | `.Must(BeAValidUrl).When(...).WithErrorCode(Validation_InvalidUrl)` | `.url('La URL de YouTube no tiene formato valido').optional()` |
| urlYouTube | max 300 chars | `.MaximumLength(300).When(...).WithErrorCode(Validation_MaxLength)` | `.max(300, 'Maximo 300 caracteres').optional()` |
| urlTwitter | formato URL cuando presente | `.Must(BeAValidUrl).When(...).WithErrorCode(Validation_InvalidUrl)` | `.url('La URL de Twitter/X no tiene formato valido').optional()` |
| urlTwitter | max 300 chars | `.MaximumLength(300).When(...).WithErrorCode(Validation_MaxLength)` | `.max(300, 'Maximo 300 caracteres').optional()` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdpromotion.schema.ts
import { z } from 'zod';

const urlOptionalSchema = (fieldName: string) =>
  z
    .string()
    .url(`La URL de ${fieldName} no tiene formato valido`)
    .max(300, 'Maximo 300 caracteres')
    .optional()
    .or(z.literal(''));

export const createPromotorSchema = z.object({
  nombrePublico: z
    .string({
      required_error: 'El nombre publico es obligatorio',
    })
    .min(1, 'El nombre publico es obligatorio')
    .min(3, 'El nombre debe tener al menos 3 caracteres')
    .max(200, 'Maximo 200 caracteres'),

  tipoPromotorId: z
    .number({
      required_error: 'El tipo de promotor es obligatorio',
      invalid_type_error: 'El tipo de promotor es obligatorio',
    })
    .int()
    .min(1, 'El tipo de promotor es obligatorio'),

  emailContacto: z
    .string()
    .email('El email de contacto no tiene formato valido')
    .max(200, 'Maximo 200 caracteres')
    .optional()
    .or(z.literal('')),

  urlSitioWeb: urlOptionalSchema('sitio web'),
  urlInstagram: urlOptionalSchema('Instagram'),
  urlTikTok: urlOptionalSchema('TikTok'),
  urlYouTube: urlOptionalSchema('YouTube'),
  urlTwitter: urlOptionalSchema('Twitter/X'),
});

export const updatePromotorSchema = z.object({
  nombrePublico: z
    .string({
      required_error: 'El nombre publico es obligatorio',
    })
    .min(1, 'El nombre publico es obligatorio')
    .min(3, 'El nombre debe tener al menos 3 caracteres')
    .max(200, 'Maximo 200 caracteres'),

  emailContacto: z
    .string()
    .email('El email de contacto no tiene formato valido')
    .max(200, 'Maximo 200 caracteres')
    .optional()
    .or(z.literal('')),

  urlSitioWeb: urlOptionalSchema('sitio web'),
  urlInstagram: urlOptionalSchema('Instagram'),
  urlTikTok: urlOptionalSchema('TikTok'),
  urlYouTube: urlOptionalSchema('YouTube'),
  urlTwitter: urlOptionalSchema('Twitter/X'),
});

export type CreatePromotorFormData = z.infer<typeof createPromotorSchema>;
export type UpdatePromotorFormData = z.infer<typeof updatePromotorSchema>;
```

---

## Nuevas Constantes ServiceResponseMessageType

Crear el archivo `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
namespace WePlayRises.Crowdpromotion.Domain.Constants;

public static class ServiceResponseMessageType
{
    // Success (0000-0999)
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // Validation (1000-1999)
    // Reutilizadas del modulo Crowdsourcing (mismos codigos, modulo independiente)
    public const string Validation_Required           = "1001";
    public const string Validation_MaxLength          = "1002";
    public const string Validation_InvalidEmail       = "1003";
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_MinLength          = "1011";
    public const string Validation_InvalidUrl         = "1013";

    // NotFound (2000-2999)
    public const string NotFound_Entity   = "2000";
    public const string NotFound_Promotor = "2015";  // NUEVO - No existe Promotor para el UserId

    // Auth (3000-3999)
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden    = "3002";

    // Business Rules (4000-4999)
    public const string BusinessRule_PromotorAlreadyExists   = "4018";  // NUEVO - Ya existe Promotor para el UserId
    public const string BusinessRule_PromotorAlreadyInactive = "4019";  // NUEVO - Promotor ya esta desactivado

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
}
```

**Codigos nuevos para esta feature:**

| Constante | Codigo | Descripcion |
|-----------|--------|-------------|
| `NotFound_Promotor` | `2015` | No existe `Promotor` con el `UserId` del token |
| `BusinessRule_PromotorAlreadyExists` | `4018` | Ya existe un `Promotor` con el mismo `UserId` |
| `BusinessRule_PromotorAlreadyInactive` | `4019` | El promotor ya tiene `EsActivo = false` |

**Justificacion de numeracion:** Los codigos `2015`, `4018` y `4019` continuan la secuencia del modulo `Crowdsourcing` (que llega hasta `2014` y `4017` respectivamente) para mantener consistencia numerica entre modulos, aunque cada modulo define su propia clase `ServiceResponseMessageType`.

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Query Keys - agregar como nueva clave raiz en QUERY_KEYS:
//
// crowdpromotion: {
//   promotor: {
//     me: ['crowdpromotion', 'promotor', 'me'] as const,
//   },
//   maestras: {
//     tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
//   },
// },

// Estructura completa de la nueva entrada en QUERY_KEYS:
export const QUERY_KEYS = {
  // ... (entradas existentes) ...

  crowdpromotion: {
    promotor: {
      me: ['crowdpromotion', 'promotor', 'me'] as const,
    },
    maestras: {
      tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
    },
  },
} as const;

// API Routes - agregar como nueva clave raiz en API_ROUTES:
export const API_ROUTES = {
  // ... (entradas existentes) ...

  crowdpromotion: {
    promotor: {
      base: '/api/crowdpromotion/promotor',
      me: '/api/crowdpromotion/promotor/me',
      desactivar: '/api/crowdpromotion/promotor/me/desactivar',
    },
    maestras: {
      tiposPromotor: '/api/crowdpromotion/maestras/tipos-promotor',
    },
  },
} as const;

// APP Routes - agregar en APP_ROUTES.landing:
export const APP_ROUTES = {
  // ... (entradas existentes) ...
  landing: {
    // ... (entradas existentes) ...
    promotor: {
      registro: '/promotor/registro',
      dashboard: '/promotor/dashboard',
      perfil: '/promotor/perfil',
    },
  },
} as const;
```

### Constantes de dominio Crowdpromotion

```typescript
// Agregar en src/shared/constants/index.ts

// ========== Tipo de Promotor (seed data MaestraTipoPromotor) ==========

export const TIPO_PROMOTOR = {
  FAN_EMBAJADOR:        1,
  INFLUENCER:           2,
  MEDIO_BLOG:           3,
  PROFESIONAL_MARKETING: 4,
} as const;

export const TIPO_PROMOTOR_LABELS: Record<number, string> = {
  1: 'Fan Embajador',
  2: 'Influencer',
  3: 'Medio / Blog',
  4: 'Profesional Marketing',
};

export const TIPO_PROMOTOR_DESCRIPTIONS: Record<number, string> = {
  1: 'Fan que promueve artistas por pasion y por recompensas',
  2: 'Creador de contenido con audiencia en redes sociales',
  3: 'Medio de comunicacion, blog o podcast musical',
  4: 'Profesional del marketing digital o musical',
};
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... (mensajes existentes) ...

  // Crowdpromotion - Perfil de Promotor
  '2015': 'No tienes un perfil de promotor. Registrate primero.',
  '4018': 'Ya tienes un perfil de promotor creado.',
  '4019': 'Tu perfil de promotor ya esta desactivado.',

  // Keys semanticos para uso interno en hooks
  PROMOTOR_NOT_FOUND:          'No tienes un perfil de promotor. Registrate primero.',
  PROMOTOR_ALREADY_EXISTS:     'Ya tienes un perfil de promotor creado.',
  PROMOTOR_ALREADY_INACTIVE:   'Tu perfil de promotor ya esta desactivado.',
  PROMOTOR_TIPO_NOT_FOUND:     'El tipo de promotor seleccionado no existe.',
  PROMOTOR_NOMBRE_REQUIRED:    'El nombre publico es obligatorio.',
  PROMOTOR_NOMBRE_MIN:         'El nombre debe tener al menos 3 caracteres.',
  PROMOTOR_NOMBRE_MAX:         'El nombre publico no puede superar los 200 caracteres.',
  PROMOTOR_EMAIL_INVALID:      'El email de contacto no tiene un formato valido.',
  PROMOTOR_URL_INVALID:        'La URL indicada no tiene un formato valido.',
};
```

---

## Notas de Implementacion

### Backend

1. **Entidad `Promotor` - campos faltantes:** La entidad actual no tiene `EmailContacto` ni `UrlSitioWeb`. Estos campos deben agregarse a la entidad y a su configuracion EF Core antes de implementar los comandos. Ver seccion de DTOs para las definiciones C#.

2. **CreatePromotorCommand:**
   - Obtener `UserId` del token JWT (claim `sub`).
   - Verificar unicidad: si existe `Promotor` con `UserId == userId`, retornar `BusinessRule_PromotorAlreadyExists` (4018).
   - Resolver `FanProfileId`: consultar `FanProfile` por `UserId`; si no existe, asignar `null` (flujo alternativo FA-02).
   - Validar `TipoPromotorId` contra `MaestraTipoPromotor`; si no existe, retornar `Validation_ForeignKeyNotFound` (1010).
   - Crear `Promotor` con `EsActivo = true`, `FechaCreacion = DateTime.UtcNow`.
   - Crear `PromotorWallet` con `MonedaId = 1` (EUR), todos los saldos en `0`, vinculada al `PromotorId` recien creado.
   - Ambas creaciones deben realizarse en la misma transaccion.
   - La unicidad por `UserId` se garantiza tambien con un constraint unico en BD sobre la columna `UserId` de la tabla `Promotores`.

3. **GetPromotorMeQuery:**
   - Obtener `UserId` del token.
   - Buscar `Promotor` con `UserId == userId`; si no existe, retornar `NotFound_Promotor` (2015).
   - Calcular `TotalProgramasActivos` con COUNT de `PromoProgramaPromotor` activos.
   - Calcular `TotalComisionesGanadas` desde `PromotorWallet` con `MonedaId == 1`.
   - Resolver `TipoPromotorNombre` desde `MaestraTipoPromotor`.
   - Usar `AsNoTracking()` para la query de lectura.

4. **UpdatePromotorCommand:**
   - Obtener `UserId` del token.
   - Buscar `Promotor`; si no existe, retornar `NotFound_Promotor` (2015).
   - Actualizar los campos editables (`NombrePublico`, `EmailContacto`, URLs).
   - `TipoPromotorId` NO se incluye en el request ni se modifica.
   - Establecer `FechaActualizacion = DateTime.UtcNow`.

5. **DesactivarPromotorCommand:**
   - Obtener `UserId` del token.
   - Buscar `Promotor`; si no existe, retornar `NotFound_Promotor` (2015).
   - Si `EsActivo == false`, retornar `BusinessRule_PromotorAlreadyInactive` (4019).
   - Establecer `EsActivo = false`.
   - Desactivar todos los `PromoProgramaPromotor` activos del promotor; contar los afectados para `programasDadosDeBaja`.
   - Toda la operacion en una misma transaccion.

6. **Caching:** Usar `IRequestCacheService` para la resolucion de `Promotor` por `UserId` (puede ser consultado tanto por el Validator como por el Handler en el mismo request).

### Frontend

1. **Nuevo archivo de tipos:** `src/shared/types/crowdpromotion.ts` con todas las interfaces definidas en la seccion TypeScript de este documento.

2. **Nuevo schema de validacion:** `src/shared/schemas/crowdpromotion.schema.ts` con `createPromotorSchema` y `updatePromotorSchema`.

3. **Hooks principales:**
   - `usePromotor()`: Query `GET /api/crowdpromotion/promotor/me` con `QUERY_KEYS.crowdpromotion.promotor.me`. Opcion `retry: false` para no reintentar en 404 (aun no tiene perfil).
   - `useCreatePromotor()`: Mutation `POST /api/crowdpromotion/promotor`; en `onSuccess` invalida `QUERY_KEYS.crowdpromotion.promotor.me` y redirige a `/promotor/dashboard`.
   - `useUpdatePromotor()`: Mutation `PUT /api/crowdpromotion/promotor/me`; en `onSuccess` invalida `QUERY_KEYS.crowdpromotion.promotor.me`.
   - `useDesactivarPromotor()`: Mutation `PATCH /api/crowdpromotion/promotor/me/desactivar`; en `onSuccess` invalida `QUERY_KEYS.crowdpromotion.promotor.me`.

4. **Guard de ruta:** En `/promotor/registro`, verificar si el usuario ya tiene perfil de promotor (query `GET /me`). Si la query retorna datos, redirigir a `/promotor/dashboard`. Si retorna 404, mostrar el formulario de registro.

5. **Tipos de promotor en formulario:** Las opciones del select `tipoPromotorId` se pueden cargar desde las constantes `TIPO_PROMOTOR_LABELS` (valores fijos del seed) sin necesidad de un endpoint de maestras en el MVP. Si en el futuro los tipos son dinamicos, usar `GET /api/crowdpromotion/maestras/tipos-promotor` con `QUERY_KEYS.crowdpromotion.maestras.tiposPromotor`.

6. **Manejo del campo URL vacio:** Los campos de URL son strings en el formulario. Un campo vacio (`""`) se debe convertir a `undefined` o `null` antes de enviar al backend para evitar errores de validacion de formato URL.

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (4 endpoints documentados)
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (CreatePromotorRequestDto, UpdatePromotorRequestDto, PromotorCreatedResultDto, PromotorDto, PromotorUpdatedResultDto, PromotorDesactivadoResultDto)
- [x] Types TypeScript equivalentes (CreatePromotorRequest, UpdatePromotorRequest, PromotorCreatedResult, Promotor, PromotorUpdatedResult, PromotorDesactivadoResult, TipoPromotor)
- [x] Schemas Zod con mismas reglas (createPromotorSchema, updatePromotorSchema)
- [x] Constantes compartidas (QUERY_KEYS.crowdpromotion, API_ROUTES.crowdpromotion, APP_ROUTES.landing.promotor, TIPO_PROMOTOR)
- [x] Nuevas constantes ServiceResponseMessageType (NotFound_Promotor = "2015", BusinessRule_PromotorAlreadyExists = "4018", BusinessRule_PromotorAlreadyInactive = "4019")
- [x] Mapeo de errores a mensajes UI
- [x] Nota sobre campos faltantes en entidad Promotor (EmailContacto, UrlSitioWeb)
- [x] Regla de negocio: un Promotor por UserId con constraint unico en BD
- [x] Regla de negocio: wallet auto-creada en EUR al crear el perfil
- [x] Regla de negocio: tipoPromotorId no editable tras la creacion
- [x] Regla de negocio: desactivacion logica (EsActivo = false, no DELETE)
- [x] Notas de implementacion Backend + Frontend
