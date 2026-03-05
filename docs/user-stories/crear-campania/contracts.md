# Contratos: Crear Campania

> **Feature:** crear-campania (US-02)
> **Ultima actualizacion:** 2026-02-12

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### POST /api/campanias

**Descripcion:** Crea una nueva campania en estado BORRADOR (EstadoCampaniaId = 1). La campania inicia con ImportePledgedActual = 0.

**Autorizacion:** ✅ Bearer JWT (ArtistaId debe extraerse del token sub claim, NO del body)

**Request Body:**
```json
{
  "proyectoArtisticoId": "guid (opcional)",
  "titulo": "string (requerido, max 200 caracteres)",
  "subtitulo": "string (opcional, max 300 caracteres)",
  "descripcionCorta": "string (opcional, max 500 caracteres)",
  "videoPrincipalUrl": "string (URL valida, opcional, max 500 caracteres)",
  "imagenPrincipalUrl": "string (URL valida, opcional, max 500 caracteres)",
  "monedaId": "int (requerido, > 0, default 1 para EUR)",
  "importeObjetivo": "decimal (requerido, > 0)",
  "importeMinimo": "decimal (opcional, > 0, <= importeObjetivo)",
  "tipoFinanciacionId": "int (requerido, > 0)",
  "permiteAportacionesAnonimas": "boolean (default false)",
  "permitePropinas": "boolean (default false)",
  "fechaInicio": "datetime (opcional, ISO 8601)",
  "fechaFin": "datetime (opcional, ISO 8601, > fechaInicio)"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "artistaId": "guid",
    "proyectoArtisticoId": "guid | null",
    "titulo": "Mi primer album",
    "subtitulo": "Rock alternativo con influencias indie",
    "descripcionCorta": "Necesitamos tu apoyo para grabar...",
    "videoPrincipalUrl": "https://youtube.com/watch?v=xyz",
    "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "importeMinimo": 100.00,
    "importePledgedActual": 0.00,
    "tipoFinanciacionId": 1,
    "estadoCampaniaId": 1,
    "permiteAportacionesAnonimas": false,
    "permitePropinas": true,
    "porcentajeComisionPlataforma": null,
    "fechaInicio": "2026-03-01T00:00:00Z",
    "fechaFin": "2026-04-30T23:59:59Z",
    "fechaPublicacion": null,
    "fechaCierre": null,
    "fechaCreacion": "2026-02-12T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Campania creada correctamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | Campo titulo vacio |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | titulo > 200 chars |
| 400 | 1002 | El subtitulo no puede superar los 300 caracteres | subtitulo > 300 chars |
| 400 | 1002 | La descripcion corta no puede superar los 500 caracteres | descripcionCorta > 500 chars |
| 400 | 1006 | La URL del video no es valida | videoPrincipalUrl formato invalido |
| 400 | 1006 | La URL de la imagen no es valida | imagenPrincipalUrl formato invalido |
| 400 | 1001 | La moneda es obligatoria | monedaId <= 0 |
| 400 | 1011 | El importe objetivo debe ser mayor a 0 | importeObjetivo <= 0 |
| 400 | 1011 | El importe minimo debe ser mayor a 0 | importeMinimo <= 0 |
| 400 | 1007 | El importe minimo no puede ser mayor al importe objetivo | importeMinimo > importeObjetivo |
| 400 | 1001 | El tipo de financiacion es obligatorio | tipoFinanciacionId <= 0 |
| 400 | 1012 | La fecha de fin debe ser posterior a la fecha de inicio | fechaFin <= fechaInicio |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 500 | 5000 | Error inesperado al crear campania | Excepcion no controlada |

---

### GET /api/campanias/{id}

**Descripcion:** Obtiene los detalles completos de una campania por su ID.

**Autorizacion:** ❌ Publica (cualquier usuario puede ver campanias publicadas)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "artistaId": "guid",
    "titulo": "Mi primer album",
    "subtitulo": "Rock alternativo",
    "descripcionCorta": "Descripcion...",
    "videoPrincipalUrl": "https://youtube.com/watch?v=xyz",
    "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "importePledgedActual": 1250.50,
    "estadoCampaniaId": 2,
    "fechaInicio": "2026-03-01T00:00:00Z",
    "fechaFin": "2026-04-30T23:59:59Z",
    "fechaCreacion": "2026-02-12T10:30:00Z"
  },
  "messages": [
    {
      "message": "Campania encontrada",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 404 | 2003 | Campania no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### PUT /api/campanias/{id}

**Descripcion:** Actualiza una campania existente. Solo se pueden editar campanias en estado BORRADOR (EstadoCampaniaId = 1). Solo el propietario (ArtistaId del token == ArtistaId de la campania) puede editar.

**Autorizacion:** ✅ Bearer JWT, solo propietario de la campania

**Request Body:**
```json
{
  "id": "guid (requerido, debe coincidir con URL)",
  "titulo": "string (opcional, max 200)",
  "subtitulo": "string (opcional, max 300)",
  "descripcionCorta": "string (opcional, max 500)",
  "videoPrincipalUrl": "string (URL valida, opcional)",
  "imagenPrincipalUrl": "string (URL valida, opcional)",
  "importeObjetivo": "decimal (opcional, > 0)",
  "importeMinimo": "decimal (opcional, > 0)",
  "tipoFinanciacionId": "int (opcional, > 0)",
  "permiteAportacionesAnonimas": "boolean (opcional)",
  "permitePropinas": "boolean (opcional)",
  "fechaInicio": "datetime (opcional)",
  "fechaFin": "datetime (opcional, > fechaInicio)"
}
```

**Response 200 OK:**
```json
{
  "data": true,
  "messages": [
    {
      "message": "Campania actualizada correctamente",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El Id es obligatorio | id vacio |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | titulo > 200 chars |
| 400 | 1011 | El importe objetivo debe ser mayor a 0 | importeObjetivo <= 0 |
| 400 | 1012 | La fecha de fin debe ser posterior a la fecha de inicio | fechaFin <= fechaInicio |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para editar esta campania | ArtistaId token != ArtistaId campania |
| 404 | 2003 | Campania no encontrada | ID no existe en DB |
| 409 | 4009 | Solo se pueden editar campanias en estado borrador | EstadoCampaniaId != 1 |
| 500 | 5000 | Error inesperado al actualizar campania | Excepcion no controlada |

---

### POST /api/campanias/{id}/publicar

**Descripcion:** Publica una campania (cambia EstadoCampaniaId de 1 a 2). Valida que la campania tenga todos los campos requeridos completos. Solo el propietario puede publicar. Establece FechaPublicacion = DateTime.UtcNow.

**Autorizacion:** ✅ Bearer JWT, solo propietario de la campania

**Campos requeridos para publicacion:**
- Titulo (NotEmpty)
- ImporteObjetivo (> 0)
- MonedaId (> 0)
- TipoFinanciacionId (> 0)
- FechaFin (NotNull, > DateTime.UtcNow + 7 days)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoCampaniaId": 2,
    "fechaPublicacion": "2026-02-12T14:00:00Z",
    "message": "Campania publicada exitosamente"
  },
  "messages": [
    {
      "message": "Campania publicada exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | Faltan campos requeridos para publicar | Titulo, ImporteObjetivo, FechaFin vacios |
| 400 | 1012 | La campania debe durar minimo 7 dias | FechaFin < DateTime.UtcNow + 7 days |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para publicar esta campania | ArtistaId token != ArtistaId campania |
| 404 | 2003 | Campania no encontrada | ID no existe en DB |
| 409 | 4009 | Solo se pueden publicar campanias en estado borrador | EstadoCampaniaId != 1 |
| 500 | 5000 | Error inesperado al publicar campania | Excepcion no controlada |

---

### GET /api/campanias

**Descripcion:** Lista campanias con filtros opcionales y paginacion.

**Autorizacion:** ❌ Publica (filtra solo campanias publicadas por defecto)

**Query Parameters:**
- `searchTerm` (string, opcional): Busca en titulo, subtitulo, descripcionCorta
- `artistaId` (guid, opcional): Filtra por artista
- `estadoCampaniaId` (int, opcional): Filtra por estado (1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada)
- `pageNumber` (int, opcional, default 1)
- `pageSize` (int, opcional, default 10, max 50)

**Response 200 OK:**
```json
{
  "data": [
    {
      "id": "guid",
      "artistaId": "guid",
      "titulo": "Mi primer album",
      "subtitulo": "Rock alternativo",
      "descripcionCorta": "Descripcion corta...",
      "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
      "importeObjetivo": 5000.00,
      "importePledgedActual": 1250.50,
      "estadoCampaniaId": 2,
      "fechaInicio": "2026-03-01T00:00:00Z",
      "fechaFin": "2026-04-30T23:59:59Z",
      "fechaCreacion": "2026-02-12T10:30:00Z"
    }
  ],
  "messages": [
    {
      "message": "Campanias encontradas",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### GET /api/campanias/mis-campanias

**Descripcion:** Obtiene todas las campanias del artista autenticado (usa ArtistaId del token JWT).

**Autorizacion:** ✅ Bearer JWT

**Query Parameters:**
- `estadoCampaniaId` (int, opcional): Filtra por estado
- `pageNumber` (int, opcional, default 1)
- `pageSize` (int, opcional, default 10)

**Response 200 OK:**
```json
{
  "data": [
    {
      "id": "guid",
      "artistaId": "guid",
      "titulo": "Mi primer album",
      "subtitulo": "Rock alternativo",
      "descripcionCorta": "Descripcion...",
      "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
      "importeObjetivo": 5000.00,
      "importePledgedActual": 1250.50,
      "estadoCampaniaId": 1,
      "fechaInicio": "2026-03-01T00:00:00Z",
      "fechaFin": "2026-04-30T23:59:59Z",
      "fechaCreacion": "2026-02-12T10:30:00Z"
    }
  ],
  "messages": [
    {
      "message": "Campanias encontradas",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

## 🔐 Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/campanias` | ✅ | - | ArtistaId extraido del token (sub claim) |
| `GET /api/campanias/{id}` | ❌ | - | Publico |
| `PUT /api/campanias/{id}` | ✅ | - | Solo propietario, solo estado BORRADOR |
| `POST /api/campanias/{id}/publicar` | ✅ | - | Solo propietario, valida campos requeridos |
| `GET /api/campanias` | ❌ | - | Publico, filtra publicadas por defecto |
| `GET /api/campanias/mis-campanias` | ✅ | - | ArtistaId del token |

### Claims JWT Requeridos

```json
{
  "sub": "artistaId (GUID)",
  "email": "artista@example.com",
  "exp": 1739000000,
  "iat": 1738913600
}
```

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas por defecto
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Proteccion de Rutas Frontend

| Ruta (Admin Dashboard) | Auth | Redirect | Notas |
|------------------------|------|----------|-------|
| `/dashboard/campanias` | ✅ | `/auth/login` | Lista mis campanias |
| `/dashboard/campanias/nueva` | ✅ | `/auth/login` | Wizard crear campania |
| `/dashboard/campanias/{id}/editar` | ✅ | `/auth/login` | Solo si propietario + BORRADOR |
| `/dashboard/campanias/{id}` | ✅ | `/auth/login` | Detalle mi campania |

| Ruta (Landing) | Auth | Redirect | Notas |
|----------------|------|----------|-------|
| `/explorar` | ❌ | - | Publico, lista campanias publicadas |
| `/campanias/{id}` | ❌ | - | Publico, detalle campania publicada |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaDto.cs
public class CampaniaDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public int TipoFinanciacionId { get; set; }
    public int EstadoCampaniaId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public decimal? PorcentajeComisionPlataforma { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaListDto.cs
public class CampaniaListDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public int EstadoCampaniaId { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CreateCampaniaRequest.cs
// DTO que recibe el controller (sin ArtistaId, se extrae del token)
public class CreateCampaniaRequest
{
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; } = 1; // Default EUR
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int TipoFinanciacionId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PublishCampaniaResponse.cs
public class PublishCampaniaResponse
{
    public Guid Id { get; set; }
    public int EstadoCampaniaId { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public string Message { get; set; } = null!;
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/campania.ts
export interface Campania {
  id: string;
  artistaId: string;
  proyectoArtisticoId?: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  videoPrincipalUrl?: string;
  imagenPrincipalUrl?: string;
  monedaId: number;
  importeObjetivo: number;
  importeMinimo?: number;
  importePledgedActual: number;
  tipoFinanciacionId: number;
  estadoCampaniaId: number;
  permiteAportacionesAnonimas: boolean;
  permitePropinas: boolean;
  porcentajeComisionPlataforma?: number;
  fechaInicio?: string; // ISO 8601
  fechaFin?: string; // ISO 8601
  fechaPublicacion?: string; // ISO 8601
  fechaCierre?: string; // ISO 8601
  fechaCreacion: string; // ISO 8601
  fechaActualizacion?: string; // ISO 8601
}

export interface CampaniaListItem {
  id: string;
  artistaId: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  imagenPrincipalUrl?: string;
  importeObjetivo: number;
  importePledgedActual: number;
  estadoCampaniaId: number;
  fechaInicio?: string;
  fechaFin?: string;
  fechaCreacion: string;
}

export interface CreateCampaniaRequest {
  proyectoArtisticoId?: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  videoPrincipalUrl?: string;
  imagenPrincipalUrl?: string;
  monedaId: number;
  importeObjetivo: number;
  importeMinimo?: number;
  tipoFinanciacionId: number;
  permiteAportacionesAnonimas: boolean;
  permitePropinas: boolean;
  fechaInicio?: string;
  fechaFin?: string;
}

export interface UpdateCampaniaRequest {
  id: string;
  titulo?: string;
  subtitulo?: string;
  descripcionCorta?: string;
  videoPrincipalUrl?: string;
  imagenPrincipalUrl?: string;
  importeObjetivo?: number;
  importeMinimo?: number;
  tipoFinanciacionId?: number;
  permiteAportacionesAnonimas?: boolean;
  permitePropinas?: boolean;
  fechaInicio?: string;
  fechaFin?: string;
}

export interface PublishCampaniaResponse {
  id: string;
  estadoCampaniaId: number;
  fechaPublicacion: string;
  message: string;
}

// Enums para estados
export enum EstadoCampania {
  Borrador = 1,
  Publicada = 2,
  Finalizada = 3,
  Cancelada = 4,
}

export enum TipoFinanciacion {
  TodoONada = 1, // All-or-nothing
  FlexibleGoal = 2, // Keep what you raise
}
```

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| titulo | requerido | `.NotEmpty().WithErrorCode("1001")` | `.min(1, 'El titulo es obligatorio')` |
| titulo | max 200 | `.MaximumLength(200).WithErrorCode("1002")` | `.max(200, 'Maximo 200 caracteres')` |
| subtitulo | max 300 | `.MaximumLength(300).WithErrorCode("1002")` | `.max(300, 'Maximo 300 caracteres').optional()` |
| descripcionCorta | max 500 | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres').optional()` |
| videoPrincipalUrl | URL valida | `.Must(BeValidUrl).WithErrorCode("1006")` | `.url('URL no valida').optional()` |
| imagenPrincipalUrl | URL valida | `.Must(BeValidUrl).WithErrorCode("1006")` | `.url('URL no valida').optional()` |
| monedaId | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1001")` | `.int().positive('Moneda requerida')` |
| importeObjetivo | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1011")` | `.number().positive('Debe ser mayor a 0')` |
| importeMinimo | > 0, <= objetivo | `.GreaterThan(0).LessThanOrEqualTo(x => x.ImporteObjetivo).WithErrorCode("1011")` | `.number().positive().lte(importeObjetivo).optional()` |
| tipoFinanciacionId | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1001")` | `.int().positive('Tipo requerido')` |
| fechaFin | > fechaInicio | `.GreaterThan(x => x.FechaInicio).WithErrorCode("1012")` | `.refine((data) => !data.fechaInicio || fechaFin > fechaInicio)` |
| fechaFin (publicar) | > now + 7 days | `.Must(x => x > DateTime.UtcNow.AddDays(7)).WithErrorCode("1012")` | `.refine((data) => fechaFin > addDays(new Date(), 7))` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/campania.schema.ts
import { z } from 'zod';
import { addDays } from 'date-fns';

// Schema para Step 1: Informacion Basica
export const campaniaBasicInfoSchema = z.object({
  titulo: z
    .string()
    .min(1, 'El titulo es obligatorio')
    .max(200, 'El titulo no puede superar los 200 caracteres'),
  subtitulo: z
    .string()
    .max(300, 'El subtitulo no puede superar los 300 caracteres')
    .optional()
    .or(z.literal('')),
  descripcionCorta: z
    .string()
    .max(500, 'La descripcion corta no puede superar los 500 caracteres')
    .optional()
    .or(z.literal('')),
});

// Schema para Step 2: Meta Financiera
export const campaniaFundingSchema = z.object({
  importeObjetivo: z
    .number({ invalid_type_error: 'Debe ser un numero' })
    .positive('El importe objetivo debe ser mayor a 0')
    .min(100, 'El importe minimo es 100 EUR'),
  importeMinimo: z
    .number()
    .positive('El importe minimo debe ser mayor a 0')
    .optional(),
  monedaId: z.number().default(1), // EUR
  tipoFinanciacionId: z
    .number()
    .positive('Debe seleccionar un tipo de financiacion'),
  permiteAportacionesAnonimas: z.boolean().default(false),
  permitePropinas: z.boolean().default(false),
}).refine(
  (data) => {
    if (data.importeMinimo) {
      return data.importeMinimo <= data.importeObjetivo;
    }
    return true;
  },
  {
    message: 'El importe minimo no puede ser mayor al objetivo',
    path: ['importeMinimo'],
  }
);

// Schema para Step 3: Duracion
export const campaniaDurationSchema = z.object({
  fechaInicio: z
    .string()
    .datetime('Formato de fecha invalido')
    .optional(),
  fechaFin: z
    .string()
    .datetime('Formato de fecha invalido')
    .refine(
      (fecha) => {
        if (!fecha) return true;
        const minDate = addDays(new Date(), 7);
        return new Date(fecha) >= minDate;
      },
      {
        message: 'La campania debe durar minimo 7 dias',
      }
    )
    .optional(),
}).refine(
  (data) => {
    if (data.fechaInicio && data.fechaFin) {
      return new Date(data.fechaFin) > new Date(data.fechaInicio);
    }
    return true;
  },
  {
    message: 'La fecha de fin debe ser posterior a la fecha de inicio',
    path: ['fechaFin'],
  }
);

// Schema para Step 4: Multimedia
export const campaniaMediaSchema = z.object({
  imagenPrincipalUrl: z
    .string()
    .url('Debe ser una URL valida')
    .optional()
    .or(z.literal('')),
  videoPrincipalUrl: z
    .string()
    .url('Debe ser una URL valida')
    .optional()
    .or(z.literal('')),
  proyectoArtisticoId: z.string().uuid().optional(),
});

// Schema completo para crear campania
export const createCampaniaSchema = campaniaBasicInfoSchema
  .merge(campaniaFundingSchema)
  .merge(campaniaDurationSchema)
  .merge(campaniaMediaSchema);

export type CreateCampaniaFormData = z.infer<typeof createCampaniaSchema>;

// Schema para actualizar campania (todos los campos opcionales excepto id)
export const updateCampaniaSchema = z.object({
  id: z.string().uuid('ID invalido'),
  titulo: z
    .string()
    .max(200, 'El titulo no puede superar los 200 caracteres')
    .optional(),
  subtitulo: z
    .string()
    .max(300, 'El subtitulo no puede superar los 300 caracteres')
    .optional(),
  descripcionCorta: z
    .string()
    .max(500, 'La descripcion corta no puede superar los 500 caracteres')
    .optional(),
  videoPrincipalUrl: z.string().url('URL invalida').optional(),
  imagenPrincipalUrl: z.string().url('URL invalida').optional(),
  importeObjetivo: z.number().positive().optional(),
  importeMinimo: z.number().positive().optional(),
  tipoFinanciacionId: z.number().positive().optional(),
  permiteAportacionesAnonimas: z.boolean().optional(),
  permitePropinas: z.boolean().optional(),
  fechaInicio: z.string().datetime().optional(),
  fechaFin: z.string().datetime().optional(),
});

export type UpdateCampaniaFormData = z.infer<typeof updateCampaniaSchema>;

// Schema para validar publicacion (campos requeridos)
export const publishCampaniaSchema = z.object({
  titulo: z.string().min(1, 'El titulo es obligatorio para publicar'),
  importeObjetivo: z.number().positive('El importe objetivo es obligatorio'),
  monedaId: z.number().positive(),
  tipoFinanciacionId: z.number().positive('El tipo de financiacion es obligatorio'),
  fechaFin: z
    .string()
    .datetime()
    .refine(
      (fecha) => {
        const minDate = addDays(new Date(), 7);
        return new Date(fecha) >= minDate;
      },
      {
        message: 'La fecha de fin debe ser minimo 7 dias desde hoy',
      }
    ),
});

export type PublishCampaniaValidation = z.infer<typeof publishCampaniaSchema>;
```

---

## 🔗 Constantes Compartidas

```typescript
// Ruta: src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  campanias: {
    all: ['campanias'] as const,
    byId: (id: string) => ['campanias', id] as const,
    misCampanias: ['campanias', 'mis-campanias'] as const,
    filtered: (filters: Record<string, any>) => ['campanias', 'filtered', filters] as const,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/api-routes.ts
export const API_ROUTES = {
  campanias: {
    base: '/api/campanias',
    byId: (id: string) => `/api/campanias/${id}`,
    publicar: (id: string) => `/api/campanias/${id}/publicar`,
    misCampanias: '/api/campanias/mis-campanias',
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/app-routes.ts
export const APP_ROUTES = {
  dashboard: {
    campanias: {
      list: '/dashboard/campanias',
      nueva: '/dashboard/campanias/nueva',
      editar: (id: string) => `/dashboard/campanias/${id}/editar`,
      detalle: (id: string) => `/dashboard/campanias/${id}`,
    },
  },
  landing: {
    explorar: '/explorar',
    campaniaById: (id: string) => `/campanias/${id}`,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/campania-states.ts
export const CAMPANIA_ESTADOS = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
} as const;

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
  1: 'Borrador',
  2: 'Publicada',
  3: 'Finalizada',
  4: 'Cancelada',
};

export const TIPO_FINANCIACION = {
  TODO_O_NADA: 1,
  FLEXIBLE: 2,
} as const;

export const TIPO_FINANCIACION_LABELS: Record<number, string> = {
  1: 'Todo o Nada',
  2: 'Meta Flexible',
};

export const MONEDAS = {
  EUR: 1,
  USD: 2,
} as const;

export const MONEDA_SYMBOLS: Record<number, string> = {
  1: '€',
  2: '$',
};
```

---

## 📊 Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts
export const ERROR_MESSAGES: Record<string, string> = {
  // Success codes
  '0000': 'Operacion exitosa',
  '0001': 'Creado exitosamente',
  '0002': 'Actualizado exitosamente',
  '0003': 'Eliminado exitosamente',

  // Validation errors (1000-1999)
  '1001': 'Este campo es obligatorio',
  '1002': 'El valor supera el maximo de caracteres permitido',
  '1003': 'El valor no cumple con el minimo requerido',
  '1004': 'El formato del valor no es valido',
  '1005': 'El formato del email no es valido',
  '1006': 'La URL proporcionada no es valida',
  '1007': 'El valor esta fuera del rango permitido',
  '1008': 'Este nombre ya esta en uso',
  '1009': 'Este email ya esta registrado',
  '1010': 'La referencia proporcionada no existe',
  '1011': 'El monto no es valido',
  '1012': 'La fecha no es valida',

  // Not Found errors (2000-2999)
  '2000': 'Recurso no encontrado',
  '2002': 'Artista no encontrado',
  '2003': 'Campania no encontrada',
  '2004': 'Recompensa no encontrada',
  '2005': 'Aporte no encontrado',

  // Auth errors (3000-3999)
  '3001': 'No autorizado. Por favor, inicia sesion nuevamente',
  '3002': 'No tienes permiso para realizar esta accion',
  '3003': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
  '3004': 'Token invalido',
  '3005': 'Debes iniciar sesion para continuar',

  // Business Rule errors (4000-4999)
  '4001': 'Este registro ya existe',
  '4002': 'El estado actual no permite esta operacion',
  '4003': 'Esta operacion no esta permitida',
  '4004': 'Se ha excedido el limite permitido',
  '4005': 'Fondos insuficientes',
  '4006': 'La campania no esta activa',
  '4007': 'La campania ha finalizado',
  '4009': 'Solo se pueden editar campanias en estado borrador',

  // Internal errors (5000-5999)
  '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
  '5001': 'Error de base de datos',
  '5002': 'Error de servicio externo',
  '5003': 'Error de configuracion',
} as const;

export const getErrorMessage = (errorCode: string): string => {
  return ERROR_MESSAGES[errorCode] || ERROR_MESSAGES['5000'];
};

// Helper para mostrar mensajes especificos de campania
export const getCampaniaErrorMessage = (errorCode: string): string => {
  const customMessages: Record<string, string> = {
    '1001': 'Completa todos los campos obligatorios para continuar',
    '1011': 'El importe debe ser mayor a 0. Ingresa un monto valido',
    '1012': 'La fecha de fin debe ser posterior a la fecha de inicio',
    '2003': 'No encontramos esta campania. Puede haber sido eliminada',
    '3002': 'No tienes permiso para modificar esta campania',
    '4009': 'No puedes editar una campania que ya ha sido publicada',
  };

  return customMessages[errorCode] || getErrorMessage(errorCode);
};
```

---

## 🔔 Eventos SignalR

**No aplica para esta feature.** La creacion y edicion de campanias es un flujo sincrono sin necesidad de notificaciones en tiempo real.

**Futuro (post-MVP):** Si se implementa colaboracion en tiempo real (multiples artistas editando una campania), considerar eventos SignalR para sincronizar cambios.

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (6 endpoints documentados)
- [x] Autorizacion por endpoint (tabla resumen completa)
- [x] Claims JWT documentados (sub, email, exp, iat)
- [x] DTOs C# completos (CampaniaDto, CampaniaListDto, CreateCampaniaRequest, PublishCampaniaResponse)
- [x] Types TypeScript equivalentes (Campania, CampaniaListItem, CreateCampaniaRequest, UpdateCampaniaRequest, enums)
- [x] Schemas Zod con mismas reglas (por steps del wizard + validacion publicacion)
- [x] Constantes compartidas (query keys, API routes, app routes, estados, tipos, monedas)
- [x] Mapeo de errores a mensajes UI (todos los codigos 0000-5999 + helpers especificos)
- [x] SignalR evaluado (no aplica para MVP)

---

## Notas Tecnicas Adicionales

### Wizard de Creacion (4 Steps)

El frontend implementara un wizard con los siguientes pasos:

1. **Informacion Basica** - titulo, subtitulo, descripcionCorta
2. **Meta Financiera** - importeObjetivo, importeMinimo, monedaId, tipoFinanciacionId
3. **Duracion** - fechaInicio, fechaFin
4. **Multimedia** - imagenPrincipalUrl, videoPrincipalUrl

Cada step valida con su propio schema Zod antes de avanzar. El backend valida todo el objeto completo en POST.

### Flujo de Publicacion

1. Usuario completa wizard → Campania creada en estado BORRADOR (EstadoCampaniaId = 1)
2. Usuario puede editar campania mientras este en BORRADOR
3. Usuario hace clic en "Publicar" → Validacion de campos requeridos
4. Si valida OK → POST /api/campanias/{id}/publicar → Estado cambia a PUBLICADA (EstadoCampaniaId = 2)
5. Una vez publicada, NO se puede editar (retorna error 4009)

### Extraccion de ArtistaId del Token

El backend debe extraer ArtistaId del claim "sub" del token JWT. El frontend NO debe enviar ArtistaId en el body de CreateCampaniaRequest.

```csharp
// En el Controller o Handler
var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // "sub" claim
if (!Guid.TryParse(userIdClaim, out var artistaId))
{
    return Unauthorized("Token invalido");
}

command.ArtistaId = artistaId; // Inyectar desde token
```

### Validacion de Propiedad

Para PUT y POST /publicar, validar que el ArtistaId del token coincida con el ArtistaId de la campania en DB.

```csharp
var campania = await _service.GetByIdAsync(id, ct);
if (campania.ArtistaId != artistaIdFromToken)
{
    return Forbidden("No tienes permiso para modificar esta campania");
}
```

---

**Fin del documento de contratos.**
