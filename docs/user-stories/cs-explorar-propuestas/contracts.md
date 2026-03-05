# Contratos: Explorar Propuestas de Crowdsourcing

> **Feature:** cs-explorar-propuestas (US-CS-03)
> **Última actualización:** 2026-02-17

Este documento define los contratos entre proyectos. **Cualquier cambio aquí debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### GET /api/crowdsourcing/necesidades

**Descripción:** Lista pública de necesidades abiertas para profesionales. Solo devuelve necesidades en estado `Abierta` cuya fecha límite no haya expirado. Excluye las necesidades del propio artista autenticado.

**Autorización:** ✅ Bearer JWT (cualquier rol autenticado)

**Query Parameters:**
- `tipoNecesidadId` (int, opcional) - Filtrar por tipo de necesidad
- `modalidad` (int, opcional) - Filtrar por ModalidadTrabajoId (1=Presencial, 2=Remoto, 3=Híbrido)
- `presupuestoMin` (decimal, opcional) - Precio mínimo del rango de presupuesto
- `presupuestoMax` (decimal, opcional) - Precio máximo del rango de presupuesto
- `pais` (string, opcional) - Filtrar por ubicación del país
- `orderBy` (string, opcional, default: `recientes`) - Orden: `recientes`, `mayor-presupuesto`, `fecha-limite`
- `page` (int, default: 1) - Número de página
- `pageSize` (int, default: 12, max: 50) - Elementos por página
- `search` (string, opcional) - Buscar en título y descripción

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
        "titulo": "Mezcla de pistas para EP de 5 canciones",
        "descripcion": "Buscamos un ingeniero de mezcla experimentado en indie rock...",
        "tipoNecesidadId": 3,
        "tipoNecesidadNombre": "Ingeniería de Audio",
        "presupuestoMin": 150.00,
        "presupuestoMax": 800.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "modalidadTrabajoId": 2,
        "modalidadTrabajoNombre": "Remoto",
        "ubicacionCiudad": null,
        "ubicacionPais": null,
        "artistaNombre": "Los Rockeros",
        "fechaCreacion": "2026-02-15T10:30:00Z",
        "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
        "esUrgente": false,
        "numeroPropuestas": 3
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 12,
    "totalPages": 3
  },
  "messages": [
    {
      "message": "Necesidades obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Notas sobre `esUrgente`:** calculado en el backend: `FechaLimitePropuestas` existe y es menor a 3 días desde la fecha actual.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener necesidades | Excepción no controlada |

---

### GET /api/crowdsourcing/necesidades/{id}

**Descripción:** Detalle completo de una necesidad para el profesional. Incluye campos calculados según el usuario autenticado: `yaPropuso`, `esPropietario`, `tienePerfilProfesional`. Accessible para cualquier usuario autenticado, a diferencia del endpoint del artista que valida ownership.

**Autorización:** ✅ Bearer JWT (cualquier rol autenticado)

**Path Parameters:**
- `id` (Guid) - ID de la necesidad

**Response 200 OK:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "descripcion": "Buscamos un ingeniero de mezcla experimentado en indie rock para mezclar 5 canciones...",
    "tipoNecesidadId": 3,
    "tipoNecesidadNombre": "Ingeniería de Audio",
    "estadoNecesidadId": 1,
    "estadoNecesidadNombre": "Abierta",
    "modalidadTrabajoId": 2,
    "modalidadTrabajoNombre": "Remoto",
    "presupuestoMin": 150.00,
    "presupuestoMax": 800.00,
    "monedaId": 1,
    "monedaNombre": "EUR",
    "ubicacionCiudad": null,
    "ubicacionPais": null,
    "fechaCreacion": "2026-02-15T10:30:00Z",
    "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
    "fechaInicioPrevista": "2026-04-01T00:00:00Z",
    "numeroPropuestas": 3,
    "artista": {
      "id": "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
      "nombreArtistico": "Los Rockeros",
      "imagenUrl": "https://cdn.weplayrises.com/artistas/los-rockeros.jpg"
    },
    "yaPropuso": false,
    "esPropietario": false,
    "tienePerfilProfesional": true
  },
  "messages": [
    {
      "message": "Necesidad obtenida exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Notas sobre campos calculados:**
- `yaPropuso`: true si el UserId autenticado tiene una PropuestaCrowdsourcing activa (no Retirada) para esta necesidad
- `esPropietario`: true si el UserId autenticado corresponde al artista propietario de la necesidad
- `tienePerfilProfesional`: true si existe un PerfilProfesional vinculado al UserId autenticado

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 404 | 2009 | Necesidad no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al obtener necesidad | Excepción no controlada |

---

### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas

**Descripción:** Envía una propuesta a una necesidad abierta. El UserId y PerfilProfesionalId se obtienen del token JWT y del perfil del usuario. Se permite un máximo de una propuesta por necesidad por usuario.

**Autorización:** ✅ Bearer JWT (usuario autenticado con PerfilProfesional)

**Path Parameters:**
- `necesidadId` (Guid) - ID de la necesidad a la que se propone

**Request Body:**
```json
{
  "precioPropuesto": 450.00,
  "monedaId": 1,
  "diasEstimados": 14,
  "mensajePropuesta": "Soy ingeniero de mezcla con 10 años de experiencia en rock alternativo. He trabajado con bandas como X e Y, y el resultado que ofrezco es..."
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    "necesidadTitulo": "Mezcla de pistas para EP de 5 canciones",
    "precioPropuesto": 450.00,
    "estadoPropuestaNombre": "Pendiente",
    "fechaCreacion": "2026-02-20T14:00:00Z"
  },
  "messages": [
    {
      "message": "Propuesta enviada correctamente. El artista será notificado.",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El precio propuesto debe ser mayor a 0 | precioPropuesto <= 0 |
| 400 | 1001 | La moneda es obligatoria | monedaId vacío |
| 400 | 1009 | Los días estimados deben ser mayor a 0 | diasEstimados <= 0 (cuando se envía) |
| 400 | 1002 | Los días estimados no pueden superar 365 | diasEstimados > 365 |
| 400 | 1001 | El mensaje de propuesta es obligatorio | mensajePropuesta vacío |
| 400 | 1011 | El mensaje debe tener al menos 20 caracteres | mensajePropuesta < 20 chars |
| 400 | 1002 | El mensaje no puede superar los 2000 caracteres | mensajePropuesta > 2000 chars |
| 400 | 4003 | Ya tienes una propuesta enviada para esta necesidad | Propuesta duplicada para el mismo usuario y necesidad |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 4006 | Debes crear un perfil profesional para enviar propuestas | Usuario sin PerfilProfesional |
| 403 | 4004 | No puedes enviar propuesta a tu propia necesidad | UserId es el artista propietario |
| 404 | 2009 | Necesidad no encontrada | necesidadId no existe |
| 404 | 2009 | La necesidad no está disponible para recibir propuestas | Necesidad no está en estado Abierta o está expirada |
| 500 | 5000 | Error inesperado al enviar propuesta | Excepción no controlada |

---

### GET /api/crowdsourcing/propuestas/mis-propuestas

**Descripción:** Lista paginada de las propuestas enviadas por el profesional autenticado. Se filtra automáticamente por el UserId del token.

**Autorización:** ✅ Bearer JWT (cualquier rol autenticado)

**Query Parameters:**
- `estado` (int, opcional) - Filtrar por EstadoPropuestaId (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada)
- `page` (int, default: 1) - Número de página
- `pageSize` (int, default: 10, max: 50) - Elementos por página

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
        "necesidadTitulo": "Mezcla de pistas para EP de 5 canciones",
        "artistaNombre": "Los Rockeros",
        "precioPropuesto": 450.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "estadoPropuestaId": 1,
        "estadoPropuestaNombre": "Pendiente",
        "fechaCreacion": "2026-02-20T14:00:00Z",
        "fechaActualizacion": null,
        "acuerdoId": null
      },
      {
        "id": "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
        "necesidadTitulo": "Diseño de portada del álbum",
        "artistaNombre": "Indie Band",
        "precioPropuesto": 300.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "estadoPropuestaId": 2,
        "estadoPropuestaNombre": "Aceptada",
        "fechaCreacion": "2026-02-15T09:00:00Z",
        "fechaActualizacion": "2026-02-18T11:30:00Z",
        "acuerdoId": "e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b"
      }
    ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "messages": [
    {
      "message": "Propuestas obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener propuestas | Excepción no controlada |

---

### PATCH /api/crowdsourcing/propuestas/{id}/retirar

**Descripción:** Retira una propuesta propia que está en estado `Pendiente`. La propuesta pasa a estado `Retirada`. Esta acción es irreversible y el artista deja de verla como candidato activo.

**Autorización:** ✅ Bearer JWT (propietario de la propuesta)

**Path Parameters:**
- `id` (Guid) - ID de la propuesta a retirar

**Request Body:** Ninguno.

**Response 200 OK:**
```json
{
  "data": {
    "id": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    "estadoPropuestaNombre": "Retirada"
  },
  "messages": [
    {
      "message": "Propuesta retirada correctamente",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4005 | Solo se pueden retirar propuestas en estado Pendiente | EstadoPropuestaId != 1 |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para retirar esta propuesta | UserId del token != UserId de la propuesta |
| 404 | 2010 | Propuesta no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al retirar propuesta | Excepción no controlada |

---

## Autorización

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `GET /api/crowdsourcing/necesidades` | ✅ | Cualquiera | Excluye necesidades del propio artista autenticado |
| `GET /api/crowdsourcing/necesidades/{id}` | ✅ | Cualquiera | Calcula yaPropuso, esPropietario, tienePerfilProfesional según UserId |
| `POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas` | ✅ | Cualquiera + PerfilProfesional | PerfilProfesionalId se obtiene del perfil del usuario |
| `GET /api/crowdsourcing/propuestas/mis-propuestas` | ✅ | Cualquiera | Filtra por UserId del token |
| `PATCH /api/crowdsourcing/propuestas/{id}/retirar` | ✅ | Cualquiera | Valida que UserId del token == UserId de la propuesta |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "profesional@example.com",
  "role": "Fan",
  "exp": 1739823600,
  "iat": 1739737200
}
```

**Nota:** Los claims `artistaId` y `perfilProfesionalId` no se incluyen en el token. El backend resuelve el PerfilProfesionalId consultando la tabla PerfilProfesional por UserId.

**Configuración JWT:**
- **Algoritmo:** HS256
- **Expiración:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Protección de Rutas Frontend

| Ruta (Landing) | Auth | Redirect | Notas |
|----------------|------|----------|-------|
| `/crowdsourcing/necesidades` | ✅ | `/auth/login` | Listado público de necesidades abiertas |
| `/crowdsourcing/necesidades/:id` | ✅ | `/auth/login` | Detalle de necesidad; enviar propuesta requiere perfil |
| `/crowdsourcing/mis-propuestas` | ✅ | `/auth/login` | Mis propuestas enviadas |

---

## DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadPublicaListDto.cs
public class NecesidadPublicaListDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    /// <summary>Descripción truncada a 150 caracteres</summary>
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public string ArtistaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    /// <summary>true si FechaLimitePropuestas existe y es menor a 3 días desde ahora</summary>
    public bool EsUrgente { get; set; }
    public int NumeroPropuestas { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadPublicaDto.cs
public class NecesidadPublicaDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public int NumeroPropuestas { get; set; }
    public ArtistaPublicoDto Artista { get; set; } = null!;
    /// <summary>true si el UserId autenticado ya envió una propuesta no-Retirada</summary>
    public bool YaPropuso { get; set; }
    /// <summary>true si el UserId autenticado es el artista propietario de la necesidad</summary>
    public bool EsPropietario { get; set; }
    /// <summary>true si existe PerfilProfesional para el UserId autenticado</summary>
    public bool TienePerfilProfesional { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ArtistaPublicoDto.cs
public class ArtistaPublicoDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? ImagenUrl { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreatePropuestaRequestDto.cs
public class CreatePropuestaRequestDto
{
    /// <summary>Debe ser > 0</summary>
    public decimal PrecioPropuesto { get; set; }
    /// <summary>ID de maestra moneda; requerido</summary>
    public int MonedaId { get; set; }
    /// <summary>Opcional; si se envía debe ser > 0 y <= 365</summary>
    public int? DiasEstimados { get; set; }
    /// <summary>Requerido; min 20 chars, max 2000 chars</summary>
    public string MensajePropuesta { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PropuestaCreatedResultDto.cs
public class PropuestaCreatedResultDto
{
    public Guid Id { get; set; }
    public string NecesidadTitulo { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MiPropuestaListDto.cs
public class MiPropuestaListDto
{
    public Guid Id { get; set; }
    public string NecesidadTitulo { get; set; } = null!;
    public string ArtistaNombre { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public int MonedaId { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public int EstadoPropuestaId { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    /// <summary>Nulo si la propuesta aún no fue aceptada (sin acuerdo generado)</summary>
    public Guid? AcuerdoId { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RetirarPropuestaResultDto.cs
public class RetirarPropuestaResultDto
{
    public Guid Id { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts
// Agregar las siguientes interfaces al archivo existente

export interface NecesidadPublicaList {
  id: string;
  titulo: string;
  descripcion?: string;
  tipoNecesidadId: number;
  tipoNecesidadNombre: string;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  monedaNombre?: string;
  modalidadTrabajoId: number;
  modalidadTrabajoNombre: string;
  ubicacionCiudad?: string;
  ubicacionPais?: string;
  artistaNombre: string;
  fechaCreacion: string;
  fechaLimitePropuestas?: string;
  esUrgente: boolean;
  numeroPropuestas: number;
}

export interface NecesidadPublica {
  id: string;
  titulo: string;
  descripcion?: string;
  tipoNecesidadId: number;
  tipoNecesidadNombre: string;
  estadoNecesidadId: number;
  estadoNecesidadNombre: string;
  modalidadTrabajoId: number;
  modalidadTrabajoNombre: string;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  monedaNombre?: string;
  ubicacionCiudad?: string;
  ubicacionPais?: string;
  fechaCreacion: string;
  fechaLimitePropuestas?: string;
  fechaInicioPrevista?: string;
  numeroPropuestas: number;
  artista: ArtistaPublico;
  yaPropuso: boolean;
  esPropietario: boolean;
  tienePerfilProfesional: boolean;
}

export interface ArtistaPublico {
  id: string;
  nombreArtistico: string;
  imagenUrl?: string;
}

export interface CreatePropuestaRequest {
  precioPropuesto: number;
  monedaId: number;
  diasEstimados?: number;
  mensajePropuesta: string;
}

export interface PropuestaCreatedResult {
  id: string;
  necesidadTitulo: string;
  precioPropuesto: number;
  estadoPropuestaNombre: string;
  fechaCreacion: string;
}

export interface MiPropuestaList {
  id: string;
  necesidadTitulo: string;
  artistaNombre: string;
  precioPropuesto: number;
  monedaId: number;
  monedaNombre: string;
  estadoPropuestaId: number;
  estadoPropuestaNombre: string;
  fechaCreacion: string;
  fechaActualizacion?: string;
  acuerdoId?: string;
}

export interface RetirarPropuestaResult {
  id: string;
  estadoPropuestaNombre: string;
}

export interface NecesidadesPublicasFilter {
  tipoNecesidadId?: number;
  modalidad?: number;
  presupuestoMin?: number;
  presupuestoMax?: number;
  pais?: string;
  orderBy?: 'recientes' | 'mayor-presupuesto' | 'fecha-limite';
  page?: number;
  pageSize?: number;
  search?: string;
}

export type EstadoPropuesta = 'Pendiente' | 'Aceptada' | 'Rechazada' | 'Retirada';
```

---

## Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| precioPropuesto | requerido y > 0 | `.GreaterThan(0).WithMessage("El precio propuesto debe ser mayor a 0").WithErrorCode(Validation_Required)` | `.positive('El precio propuesto debe ser mayor a 0')` |
| monedaId | requerido | `.NotEmpty().WithMessage("La moneda es obligatoria").WithErrorCode(Validation_Required)` | `.positive('La moneda es obligatoria')` |
| diasEstimados | > 0 si presente | `.GreaterThan(0).When(x => x.DiasEstimados.HasValue).WithMessage("Los días estimados deben ser mayor a 0").WithErrorCode(Validation_InvalidRange)` | `.positive('Los días estimados deben ser mayor a 0').optional()` |
| diasEstimados | <= 365 si presente | `.LessThanOrEqualTo(365).When(x => x.DiasEstimados.HasValue).WithMessage("Máximo 365 días").WithErrorCode(Validation_MaxLength)` | `.max(365, 'Máximo 365 días').optional()` |
| mensajePropuesta | requerido | `.NotEmpty().WithMessage("El mensaje de propuesta es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El mensaje de propuesta es obligatorio')` |
| mensajePropuesta | min 20 chars | `.MinimumLength(20).WithMessage("El mensaje debe tener al menos 20 caracteres").WithErrorCode(Validation_MinLength)` | `.min(20, 'El mensaje debe tener al menos 20 caracteres')` |
| mensajePropuesta | max 2000 chars | `.MaximumLength(2000).WithMessage("El mensaje no puede superar los 2000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(2000, 'El mensaje no puede superar los 2000 caracteres')` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
// Agregar al archivo existente

import { z } from 'zod';

export const createPropuestaSchema = z.object({
  precioPropuesto: z
    .number({
      required_error: 'El precio propuesto es obligatorio',
      invalid_type_error: 'El precio debe ser un número',
    })
    .positive('El precio propuesto debe ser mayor a 0'),
  monedaId: z
    .number({
      required_error: 'La moneda es obligatoria',
      invalid_type_error: 'Selecciona una moneda válida',
    })
    .positive('La moneda es obligatoria'),
  diasEstimados: z
    .number()
    .int('Los días estimados deben ser un número entero')
    .positive('Los días estimados deben ser mayor a 0')
    .max(365, 'Máximo 365 días')
    .optional(),
  mensajePropuesta: z
    .string()
    .min(1, 'El mensaje de propuesta es obligatorio')
    .min(20, 'El mensaje debe tener al menos 20 caracteres')
    .max(2000, 'El mensaje no puede superar los 2000 caracteres'),
});

export const filterNecesidadesSchema = z.object({
  tipoNecesidadId: z.number().positive().optional(),
  modalidad: z.number().positive().optional(),
  presupuestoMin: z.number().nonnegative('El presupuesto mínimo no puede ser negativo').optional(),
  presupuestoMax: z.number().nonnegative('El presupuesto máximo no puede ser negativo').optional(),
  pais: z.string().max(100).optional(),
  orderBy: z.enum(['recientes', 'mayor-presupuesto', 'fecha-limite']).optional(),
  search: z.string().max(200).optional(),
  page: z.number().positive().optional(),
  pageSize: z.number().positive().max(50).optional(),
}).refine(
  (data) => {
    if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
      return data.presupuestoMax >= data.presupuestoMin;
    }
    return true;
  },
  {
    message: 'El presupuesto máximo debe ser mayor o igual al mínimo',
    path: ['presupuestoMax'],
  }
);

export type CreatePropuestaFormData = z.infer<typeof createPropuestaSchema>;
export type FilterNecesidadesFormData = z.infer<typeof filterNecesidadesSchema>;
```

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Estado de Propuesta (int IDs desde maestras)
export const ESTADO_PROPUESTA = {
  PENDIENTE: 1,
  ACEPTADA: 2,
  RECHAZADA: 3,
  RETIRADA: 4,
} as const;

export const ESTADO_PROPUESTA_LABELS: Record<number, string> = {
  1: 'Pendiente',
  2: 'Aceptada',
  3: 'Rechazada',
  4: 'Retirada',
};

export const ESTADO_PROPUESTA_BADGES: Record<number, string> = {
  1: 'yellow',   // Pendiente - amarillo
  2: 'green',    // Aceptada - verde
  3: 'red',      // Rechazada - rojo
  4: 'gray',     // Retirada - gris
};

// Opciones de ordenamiento para el listado de necesidades
export const ORDER_BY_NECESIDADES = {
  RECIENTES: 'recientes',
  MAYOR_PRESUPUESTO: 'mayor-presupuesto',
  FECHA_LIMITE: 'fecha-limite',
} as const;

export const ORDER_BY_NECESIDADES_LABELS: Record<string, string> = {
  recientes: 'Más recientes',
  'mayor-presupuesto': 'Mayor presupuesto',
  'fecha-limite': 'Fecha límite próxima',
};

// Umbral de urgencia en días para badge de urgencia
export const URGENCIA_DIAS_UMBRAL = 3;

// Query Keys additions
// Agregar dentro de QUERY_KEYS.crowdsourcing:
//
// necesidades: {
//   ...existentes (mis, byId),
//   publicas: ['crowdsourcing', 'necesidades', 'publicas'] as const,
//   publicaById: (id: string) => ['crowdsourcing', 'necesidades', 'publica', id] as const,
// },
// propuestas: {
//   mis: ['crowdsourcing', 'propuestas', 'mis'] as const,
// },
export const QUERY_KEYS = {
  crowdsourcing: {
    necesidades: {
      mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
      byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
      publicas: ['crowdsourcing', 'necesidades', 'publicas'] as const,
      publicaById: (id: string) => ['crowdsourcing', 'necesidades', 'publica', id] as const,
    },
    propuestas: {
      mis: ['crowdsourcing', 'propuestas', 'mis'] as const,
    },
  },
};

// API Routes additions
// Agregar dentro de API_ROUTES.crowdsourcing:
export const API_ROUTES = {
  crowdsourcing: {
    necesidades: {
      base: '/api/crowdsourcing/necesidades',
      mis: '/api/crowdsourcing/necesidades/mis-necesidades',
      byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
      cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
      propuestas: (necesidadId: string) =>
        `/api/crowdsourcing/necesidades/${necesidadId}/propuestas`,
    },
    propuestas: {
      mis: '/api/crowdsourcing/propuestas/mis-propuestas',
      retirar: (id: string) => `/api/crowdsourcing/propuestas/${id}/retirar`,
    },
  },
};

// App Routes additions (Landing)
// Agregar dentro de APP_ROUTES:
export const APP_ROUTES = {
  landing: {
    crowdsourcing: {
      necesidades: '/crowdsourcing/necesidades',
      necesidadDetail: (id: string) => `/crowdsourcing/necesidades/${id}`,
      misPropuestas: '/crowdsourcing/mis-propuestas',
    },
  },
};
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... mensajes existentes

  // Crowdsourcing - Propuestas errors
  '2010': 'La propuesta no fue encontrada',
  '4003': 'Ya tienes una propuesta enviada para esta necesidad',
  '4004': 'No puedes enviar propuesta a tu propia necesidad',
  '4005': 'Solo se pueden retirar propuestas en estado Pendiente',
  '4006': 'Debes crear un perfil profesional antes de enviar propuestas',

  // Keys semánticos para uso interno
  PROPUESTA_NOT_FOUND: 'La propuesta no fue encontrada',
  ALREADY_PROPOSED: 'Ya tienes una propuesta enviada para esta necesidad',
  CANNOT_PROPOSE_SELF: 'No puedes enviar propuesta a tu propia necesidad',
  PROPUESTA_NOT_RETIRABLE: 'Solo se pueden retirar propuestas en estado Pendiente',
  NO_PROFESSIONAL_PROFILE: 'Debes crear un perfil profesional para poder enviar propuestas',
  VALIDATION_PRECIO_REQUERIDO: 'El precio propuesto debe ser mayor a 0',
  VALIDATION_MENSAJE_MIN_LENGTH: 'El mensaje debe tener al menos 20 caracteres',
  VALIDATION_DIAS_ESTIMADOS_RANGO: 'Los días estimados deben ser entre 1 y 365',
};
```

---

## Eventos SignalR

**No aplica para esta feature.** El flujo de exploración y envío de propuestas es síncrono. Las actualizaciones de estado se reflejan mediante invalidación de queries en el cliente.

**Nota:** En futuras iteraciones se podría agregar SignalR para notificar al artista en tiempo real cuando recibe una nueva propuesta.

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo `Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// NotFound (2000-2999)
public const string NotFound_Propuesta = "2010";

// Business Rules (4000-4999)
public const string BusinessRule_AlreadyProposed = "4003";
public const string BusinessRule_CannotProposeSelf = "4004";
public const string BusinessRule_PropuestaNotRetirable = "4005";
public const string BusinessRule_NoProfessionalProfile = "4006";
```

---

## Notas de Implementación

### Backend

1. **Diferencia entre GET /api/crowdsourcing/necesidades/{id} (US-CS-02 vs US-CS-03):**

   El endpoint del artista (US-CS-02) valida ownership y retorna propuestas recibidas (`PropuestaCrowdsourcingDto[]`). El endpoint público (US-CS-03) es accesible para cualquier usuario autenticado y retorna campos calculados (`yaPropuso`, `esPropietario`, `tienePerfilProfesional`). Se recomienda implementar como dos queries CQRS distintas:
   - `GetNecesidadByIdQuery` (US-CS-02): para artistas, valida ownership, incluye propuestas completas
   - `GetNecesidadPublicaByIdQuery` (US-CS-03): sin restricción de ownership, incluye campos calculados

   Ambas pueden compartir el mismo endpoint `GET /api/crowdsourcing/necesidades/{id}`, con el controller determinando cuál query invocar según si el UserId corresponde al artista propietario.

2. **GetNecesidadesPublicasQuery:**
   - Filtrar por `EstadoNecesidadId == 1` (Abierta)
   - Filtrar expiradas: `FechaLimitePropuestas IS NULL OR FechaLimitePropuestas > DateTime.UtcNow`
   - Excluir las necesidades donde `ArtistaId` corresponde al artista del usuario autenticado (si tiene artista)
   - Calcular `EsUrgente` en la proyección: `FechaLimitePropuestas.HasValue && FechaLimitePropuestas.Value <= DateTime.UtcNow.AddDays(3)`
   - Truncar `Descripcion` a 150 caracteres en la proyección
   - Usar `AsNoTracking()` para la consulta
   - Soportar ordenamiento: recientes (FechaCreacion DESC), mayor-presupuesto (PresupuestoMax DESC), fecha-limite (FechaLimitePropuestas ASC)

3. **CreatePropuestaCommand:**
   - Obtener `UserId` del claim del token JWT
   - Buscar `PerfilProfesionalId` por `UserId`; si no existe, retornar `ServiceResponse` con `BusinessRule_NoProfessionalProfile`
   - Validar que la necesidad existe y está en estado Abierta y no expirada
   - Validar que el `UserId` no es el artista propietario de la necesidad (`BusinessRule_CannotProposeSelf`)
   - Validar unicidad: no existe otra PropuestaCrowdsourcing con el mismo `NecesidadId` y `UserId` que no esté en estado Retirada (`BusinessRule_AlreadyProposed`)
   - Crear entidad con `EstadoPropuestaId = 1` (Pendiente) y `FechaCreacion = DateTime.UtcNow`
   - Retornar `PropuestaCreatedResultDto`

4. **RetirarPropuestaCommand:**
   - Obtener `UserId` del claim del token JWT
   - Validar que la propuesta existe; si no, `NotFound_Propuesta`
   - Validar que `UserId` del token == `PropuestaCrowdsourcing.UserId`; si no, `Auth_Forbidden`
   - Validar que `EstadoPropuestaId == 1` (Pendiente); si no, `BusinessRule_PropuestaNotRetirable`
   - Actualizar `EstadoPropuestaId = 4` (Retirada) y `FechaActualizacion = DateTime.UtcNow`

5. **GetMisPropuestasQuery:**
   - Filtrar por `UserId` del token JWT
   - Aplicar filtro opcional por `EstadoPropuestaId`
   - Include Necesidad (para obtener Titulo) e Include Artista (para obtener NombreArtistico)
   - Include AcuerdoCrowdsourcing para obtener `AcuerdoId` si existe
   - Usar `AsNoTracking()` y paginación

6. **Caching:**
   - La resolución de `PerfilProfesionalId` por `UserId` debe usar `IRequestCacheService` para compartir resultado entre Validator y Handler
   - La carga de la `NecesidadCrowdsourcing` en Validator y Handler también debe usar cache

### Frontend

1. **Nuevos archivos recomendados:**
   - `src/web/src/features/crowdsourcing/necesidades/` - Feature de exploración (Landing)
   - `src/web/src/features/crowdsourcing/propuestas/` - Feature de mis propuestas (Landing)
   - `src/shared/schemas/crowdsourcing.schema.ts` - Agregar `createPropuestaSchema` y `filterNecesidadesSchema`
   - `src/shared/types/crowdsourcing.ts` - Agregar los nuevos tipos

2. **Componentes clave (Landing):**
   - `NecesidadesPublicasPage.tsx` - Listado con filtros y paginación
   - `NecesidadPublicaCard.tsx` - Card con badge de tipo, presupuesto, urgencia, contador de propuestas
   - `NecesidadPublicaDetailPage.tsx` - Detalle con formulario de propuesta integrado o CTA
   - `EnviarPropuestaForm.tsx` - Formulario de propuesta con React Hook Form + Zod
   - `MisPropuestasPage.tsx` - Listado de propuestas propias con filtro de estado
   - `MiPropuestaCard.tsx` - Card con badge de estado coloreado
   - `RetirarPropuestaDialog.tsx` - Diálogo de confirmación antes de retirar
   - `UrgenciaBadge.tsx` - Badge de urgencia cuando quedan menos de 3 días

3. **Hooks personalizados:**
   - `useNecesidadesPublicas(filters)` - Query con filtros y paginación
   - `useNecesidadPublica(id)` - Query para detalle público
   - `useCreatePropuesta(necesidadId)` - Mutation para enviar propuesta
   - `useMisPropuestas(filters)` - Query con filtros de estado
   - `useRetirarPropuesta(id)` - Mutation para retirar

4. **UX considerations:**
   - Badge de urgencia (rojo/naranja) cuando `esUrgente == true` o calcular en frontend si `fechaLimitePropuestas` es menor a 3 días
   - Nota informativa si el precio del profesional está fuera del rango del artista (sin bloquear el envío)
   - Botón "Enviar propuesta" deshabilitado si `yaPropuso == true`, `esPropietario == true`, o la necesidad no está Abierta
   - CTA "Crear perfil profesional" si `tienePerfilProfesional == false`
   - Debounce de 300ms en el campo de búsqueda del listado
   - Empty state descriptivo en el listado: "No hay necesidades que coincidan con tus filtros. Intenta ampliar la búsqueda."
   - Toast de confirmación al enviar propuesta y al retirar
   - Click en propuesta con `acuerdoId != null` navega al detalle del acuerdo

5. **Filtros del listado:**
   - Aplicar filtros con debounce de 300ms excepto selects (aplicar inmediato)
   - Sincronizar filtros con query params de la URL para compartir y recargar búsquedas
   - Mostrar contador de filtros activos

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (5 endpoints documentados)
- [x] Autorización por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (NecesidadPublicaListDto, NecesidadPublicaDto, ArtistaPublicoDto, CreatePropuestaRequestDto, PropuestaCreatedResultDto, MiPropuestaListDto, RetirarPropuestaResultDto)
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas (createPropuestaSchema, filterNecesidadesSchema)
- [x] Constantes compartidas (ESTADO_PROPUESTA, QUERY_KEYS additions, API_ROUTES additions, APP_ROUTES additions)
- [x] Nuevas constantes ServiceResponseMessageType (2010, 4003, 4004, 4005, 4006)
- [x] Mapeo de errores a mensajes UI
- [x] Notas de implementación detalladas (Backend + Frontend)
- [x] Diferencia entre endpoint de artista (US-CS-02) y endpoint público (US-CS-03) documentada
