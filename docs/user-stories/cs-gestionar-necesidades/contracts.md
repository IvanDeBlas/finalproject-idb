# Contratos: Gestionar Necesidades de Crowdsourcing

> **Feature:** cs-gestionar-necesidades (US-CS-02)
> **Última actualización:** 2026-02-16

Este documento define los contratos entre proyectos. **Cualquier cambio aquí debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### POST /api/crowdsourcing/necesidades

**Descripción:** Crea una nueva necesidad de crowdsourcing para un proyecto artístico. El artista puede crear necesidades manualmente o desde templates. El ArtistaId se obtiene del token JWT.

**Autorización:** ✅ Bearer JWT (Artista autenticado)

**Request Body:**
```json
{
  "titulo": "Mezcla de pistas para EP de 5 canciones",
  "descripcion": "Buscamos un ingeniero de mezcla experimentado en indie rock para mezclar 5 canciones. El material está grabado en 24bit/96kHz y requiere un enfoque orgánico con énfasis en las guitarras.",
  "tipoNecesidadId": 3,
  "modalidadTrabajoId": 2,
  "presupuestoMin": 150.00,
  "presupuestoMax": 800.00,
  "monedaId": 1,
  "ubicacionCiudad": null,
  "ubicacionPais": null,
  "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
  "fechaInicioPrevista": "2026-04-01T00:00:00Z",
  "proyectoArtisticoId": "9f8e7d6c-5b4a-3c2d-1e0f-9a8b7c6d5e4f"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "estadoNecesidadId": 1,
    "estadoNecesidadNombre": "Abierta",
    "fechaCreacion": "2026-02-16T14:30:00Z"
  },
  "messages": [
    {
      "message": "Necesidad publicada correctamente",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El título es obligatorio | titulo vacío o nulo |
| 400 | 1011 | El título debe tener al menos 5 caracteres | titulo < 5 caracteres |
| 400 | 1002 | El título no puede superar los 200 caracteres | titulo > 200 caracteres |
| 400 | 1002 | La descripción no puede superar los 4000 caracteres | descripcion > 4000 caracteres |
| 400 | 1001 | El tipo de necesidad es obligatorio | tipoNecesidadId vacío |
| 400 | 1001 | La modalidad de trabajo es obligatoria | modalidadTrabajoId vacío |
| 400 | 1009 | El presupuesto máximo debe ser mayor o igual al mínimo | presupuestoMax < presupuestoMin |
| 400 | 1001 | La moneda es obligatoria cuando se especifica presupuesto | monedaId nulo con presupuesto presente |
| 400 | 1001 | La ubicación es obligatoria para modalidad Presencial o Híbrida | ubicacionCiudad nula con modalidad 1 o 3 |
| 400 | 1012 | La fecha límite debe ser posterior a hoy | fechaLimitePropuestas <= hoy |
| 400 | 1012 | La fecha de inicio debe ser igual o posterior a hoy | fechaInicioPrevista < hoy |
| 400 | 1001 | El proyecto artístico es obligatorio | proyectoArtisticoId vacío |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para crear necesidades en este proyecto | UserId del token != dueño del proyecto |
| 404 | 1010 | El tipo de necesidad no existe | tipoNecesidadId no existe |
| 404 | 1010 | La modalidad de trabajo no existe | modalidadTrabajoId no existe |
| 404 | 1010 | La moneda no existe | monedaId no existe |
| 404 | 2007 | Proyecto artístico no encontrado | proyectoArtisticoId no existe |
| 500 | 5000 | Error inesperado al crear necesidad | Excepción no controlada |

---

### GET /api/crowdsourcing/necesidades/mis-necesidades

**Descripción:** Lista todas las necesidades del artista autenticado con paginación y filtros. Incluye contador de propuestas recibidas.

**Autorización:** ✅ Bearer JWT (Artista autenticado)

**Query Parameters:**
- `estado` (int, opcional) - Filtrar por EstadoNecesidadId (1=Abierta, 2=EnProgreso, 3=Cerrada, 4=Cancelada)
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
        "estadoNecesidadId": 1,
        "estadoNecesidadNombre": "Abierta",
        "tipoNecesidadId": 3,
        "tipoNecesidadNombre": "Ingeniería de Audio",
        "presupuestoMin": 150.00,
        "presupuestoMax": 800.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "modalidadTrabajoId": 2,
        "modalidadTrabajoNombre": "Remoto",
        "numeroPropuestas": 5,
        "fechaCreacion": "2026-02-16T14:30:00Z",
        "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
        "fechaActualizacion": null
      },
      {
        "id": "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
        "titulo": "Diseño de portada del álbum",
        "estadoNecesidadId": 2,
        "estadoNecesidadNombre": "En Progreso",
        "tipoNecesidadId": 5,
        "tipoNecesidadNombre": "Diseño Gráfico",
        "presupuestoMin": 200.00,
        "presupuestoMax": 500.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "modalidadTrabajoId": 2,
        "modalidadTrabajoNombre": "Remoto",
        "numeroPropuestas": 12,
        "fechaCreacion": "2026-02-10T10:00:00Z",
        "fechaLimitePropuestas": null,
        "fechaActualizacion": "2026-02-14T16:20:00Z"
      }
    ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 12,
    "totalPages": 1
  },
  "messages": [
    {
      "message": "Necesidades obtenidas exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 500 | 5000 | Error inesperado al obtener necesidades | Excepción no controlada |

---

### GET /api/crowdsourcing/necesidades/{id}

**Descripción:** Obtiene el detalle completo de una necesidad incluyendo sus propuestas recibidas. Solo el artista propietario puede acceder.

**Autorización:** ✅ Bearer JWT (Artista propietario de la necesidad)

**Path Parameters:**
- `id` (Guid) - ID de la necesidad

**Response 200 OK:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "descripcion": "Buscamos un ingeniero de mezcla experimentado en indie rock...",
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
    "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
    "fechaInicioPrevista": "2026-04-01T00:00:00Z",
    "fechaCreacion": "2026-02-16T14:30:00Z",
    "fechaActualizacion": null,
    "proyectoArtisticoId": "9f8e7d6c-5b4a-3c2d-1e0f-9a8b7c6d5e4f",
    "proyectoArtisticoNombre": "Mi Primer EP",
    "propuestas": [
      {
        "id": "p1a2b3c4-d5e6-f7a8-b9c0-d1e2f3a4b5c6",
        "profesionalId": "prof-123-456",
        "profesionalNombre": "Juan Pérez",
        "precioPropuesto": 600.00,
        "monedaId": 1,
        "tiempoEstimadoDias": 14,
        "mensaje": "Tengo 8 años de experiencia mezclando indie rock...",
        "estadoPropuestaId": 1,
        "estadoPropuestaNombre": "Pendiente",
        "fechaCreacion": "2026-02-17T09:15:00Z"
      }
    ]
  },
  "messages": [
    {
      "message": "Necesidad obtenida exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El ID es obligatorio | ID vacío o nulo |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para ver esta necesidad | UserId del token != ArtistaId de la necesidad |
| 404 | 2009 | Necesidad no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al obtener necesidad | Excepción no controlada |

---

### PUT /api/crowdsourcing/necesidades/{id}

**Descripción:** Edita una necesidad existente. Solo se puede editar si el estado es "Abierta". El tipo de necesidad NO es editable para no invalidar propuestas existentes.

**Autorización:** ✅ Bearer JWT (Artista propietario de la necesidad)

**Path Parameters:**
- `id` (Guid) - ID de la necesidad

**Request Body:**
```json
{
  "titulo": "Mezcla profesional para EP indie rock (5 canciones)",
  "descripcion": "Actualizado: Buscamos ingeniero con experiencia en indie rock y folk...",
  "modalidadTrabajoId": 2,
  "presupuestoMin": 200.00,
  "presupuestoMax": 900.00,
  "monedaId": 1,
  "ubicacionCiudad": null,
  "ubicacionPais": null,
  "fechaLimitePropuestas": "2026-03-20T00:00:00Z",
  "fechaInicioPrevista": "2026-04-01T00:00:00Z"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "titulo": "Mezcla profesional para EP indie rock (5 canciones)",
    "estadoNecesidadId": 1,
    "estadoNecesidadNombre": "Abierta",
    "fechaActualizacion": "2026-02-16T15:45:00Z"
  },
  "messages": [
    {
      "message": "Necesidad actualizada correctamente",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El título es obligatorio | titulo vacío o nulo |
| 400 | 1011 | El título debe tener al menos 5 caracteres | titulo < 5 caracteres |
| 400 | 1002 | El título no puede superar los 200 caracteres | titulo > 200 caracteres |
| 400 | 1002 | La descripción no puede superar los 4000 caracteres | descripcion > 4000 caracteres |
| 400 | 1001 | La modalidad de trabajo es obligatoria | modalidadTrabajoId vacío |
| 400 | 1009 | El presupuesto máximo debe ser mayor o igual al mínimo | presupuestoMax < presupuestoMin |
| 400 | 1001 | La moneda es obligatoria cuando se especifica presupuesto | monedaId nulo con presupuesto presente |
| 400 | 1001 | La ubicación es obligatoria para modalidad Presencial o Híbrida | ubicacionCiudad nula con modalidad 1 o 3 |
| 400 | 1012 | La fecha límite debe ser posterior a hoy | fechaLimitePropuestas <= hoy |
| 400 | 1012 | La fecha de inicio debe ser igual o posterior a hoy | fechaInicioPrevista < hoy |
| 400 | 4001 | Solo se pueden editar necesidades en estado Abierta | EstadoNecesidadId != 1 |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para editar esta necesidad | UserId del token != ArtistaId de la necesidad |
| 404 | 2009 | Necesidad no encontrada | ID no existe en DB |
| 404 | 1010 | La modalidad de trabajo no existe | modalidadTrabajoId no existe |
| 404 | 1010 | La moneda no existe | monedaId no existe |
| 500 | 5000 | Error inesperado al actualizar necesidad | Excepción no controlada |

---

### PATCH /api/crowdsourcing/necesidades/{id}/cerrar

**Descripción:** Cierra una necesidad en estado "Abierta" o "En Progreso". Las propuestas pendientes se rechazan automáticamente.

**Autorización:** ✅ Bearer JWT (Artista propietario de la necesidad)

**Path Parameters:**
- `id` (Guid) - ID de la necesidad

**Request Body:**
```json
{
  "motivo": "Ya no necesitamos este servicio porque decidimos cambiar el enfoque del proyecto"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "estadoNecesidadNombre": "Cerrada",
    "propuestasRechazadas": 3
  },
  "messages": [
    {
      "message": "Necesidad cerrada correctamente. Se han rechazado 3 propuestas pendientes.",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1002 | El motivo no puede superar los 500 caracteres | motivo > 500 caracteres |
| 400 | 4002 | Solo se pueden cerrar necesidades en estado Abierta o En Progreso | EstadoNecesidadId != 1 y != 2 |
| 401 | 3001 | Token no válido o expirado | Token JWT inválido/expirado |
| 403 | 3002 | No tienes permiso para cerrar esta necesidad | UserId del token != ArtistaId de la necesidad |
| 404 | 2009 | Necesidad no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al cerrar necesidad | Excepción no controlada |

---

## 🔐 Autorización

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/crowdsourcing/necesidades` | ✅ | Artista | ArtistaId del token, validar ownership del proyecto |
| `GET /api/crowdsourcing/necesidades/mis-necesidades` | ✅ | Artista | Filtra por ArtistaId del token |
| `GET /api/crowdsourcing/necesidades/{id}` | ✅ | Artista | Validar que ArtistaId del token = ArtistaId de la necesidad |
| `PUT /api/crowdsourcing/necesidades/{id}` | ✅ | Artista | Validar ownership + estado = Abierta |
| `PATCH /api/crowdsourcing/necesidades/{id}/cerrar` | ✅ | Artista | Validar ownership + estado in (Abierta, EnProgreso) |

### Claims JWT Requeridos

```json
{
  "sub": "userId (GUID del usuario)",
  "email": "artista@example.com",
  "role": "Artista",
  "artistaId": "guid-del-artista",
  "exp": 1739823600,
  "iat": 1739737200
}
```

**Configuración JWT:**
- **Algoritmo:** HS256
- **Expiración:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Protección de Rutas Frontend

| Ruta (Admin Dashboard) | Auth | Redirect | Notas |
|------------------------|------|----------|-------|
| `/dashboard/crowdsourcing/necesidades` | ✅ | `/auth/login` | Mis necesidades |
| `/dashboard/crowdsourcing/necesidades/nueva` | ✅ | `/auth/login` | Crear necesidad |
| `/dashboard/crowdsourcing/necesidades/:id` | ✅ | `/auth/login` | Detalle + propuestas |
| `/dashboard/crowdsourcing/necesidades/:id/editar` | ✅ | `/auth/login` | Editar necesidad |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingListDto.cs
public class NecesidadCrowdsourcingListDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public int NumeroPropuestas { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingDto.cs
public class NecesidadCrowdsourcingDto
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
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public string ProyectoArtisticoNombre { get; set; } = null!;
    public List<PropuestaCrowdsourcingDto> Propuestas { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PropuestaCrowdsourcingDto.cs
public class PropuestaCrowdsourcingDto
{
    public Guid Id { get; set; }
    public Guid ProfesionalId { get; set; }
    public string ProfesionalNombre { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public int MonedaId { get; set; }
    public int? TiempoEstimadoDias { get; set; }
    public string Mensaje { get; set; } = null!;
    public int EstadoPropuestaId { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingCreateResultDto.cs
public class NecesidadCrowdsourcingCreateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingUpdateResultDto.cs
public class NecesidadCrowdsourcingUpdateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime? FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CerrarNecesidadResultDto.cs
public class CerrarNecesidadResultDto
{
    public Guid Id { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int PropuestasRechazadas { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts

export interface NecesidadCrowdsourcingList {
  id: string;
  titulo: string;
  estadoNecesidadId: number;
  estadoNecesidadNombre: string;
  tipoNecesidadId: number;
  tipoNecesidadNombre: string;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  monedaNombre?: string;
  modalidadTrabajoId: number;
  modalidadTrabajoNombre: string;
  numeroPropuestas: number;
  fechaCreacion: string;
  fechaLimitePropuestas?: string;
  fechaActualizacion?: string;
}

export interface NecesidadCrowdsourcing {
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
  fechaLimitePropuestas?: string;
  fechaInicioPrevista?: string;
  fechaCreacion: string;
  fechaActualizacion?: string;
  proyectoArtisticoId: string;
  proyectoArtisticoNombre: string;
  propuestas: PropuestaCrowdsourcing[];
}

export interface PropuestaCrowdsourcing {
  id: string;
  profesionalId: string;
  profesionalNombre: string;
  precioPropuesto: number;
  monedaId: number;
  tiempoEstimadoDias?: number;
  mensaje: string;
  estadoPropuestaId: number;
  estadoPropuestaNombre: string;
  fechaCreacion: string;
}

export interface CreateNecesidadRequest {
  titulo: string;
  descripcion?: string;
  tipoNecesidadId: number;
  modalidadTrabajoId: number;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  ubicacionCiudad?: string;
  ubicacionPais?: string;
  fechaLimitePropuestas?: string;
  fechaInicioPrevista?: string;
  proyectoArtisticoId: string;
}

export interface UpdateNecesidadRequest {
  titulo: string;
  descripcion?: string;
  modalidadTrabajoId: number;
  presupuestoMin?: number;
  presupuestoMax?: number;
  monedaId?: number;
  ubicacionCiudad?: string;
  ubicacionPais?: string;
  fechaLimitePropuestas?: string;
  fechaInicioPrevista?: string;
}

export interface CerrarNecesidadRequest {
  motivo?: string;
}

export interface NecesidadCreateResult {
  id: string;
  titulo: string;
  estadoNecesidadId: number;
  estadoNecesidadNombre: string;
  fechaCreacion: string;
}

export interface NecesidadUpdateResult {
  id: string;
  titulo: string;
  estadoNecesidadId: number;
  estadoNecesidadNombre: string;
  fechaActualizacion?: string;
}

export interface CerrarNecesidadResult {
  id: string;
  estadoNecesidadNombre: string;
  propuestasRechazadas: number;
}

export type EstadoNecesidad = "Abierta" | "En Progreso" | "Cerrada" | "Cancelada";
export type ModalidadTrabajo = "Presencial" | "Remoto" | "Híbrido";
```

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| titulo | requerido | `.NotEmpty().WithErrorCode(Validation_Required)` | `.min(1, 'El título es obligatorio')` |
| titulo | min 5 | `.MinimumLength(5).WithErrorCode(Validation_MinLength)` | `.min(5, 'El título debe tener al menos 5 caracteres')` |
| titulo | max 200 | `.MaximumLength(200).WithErrorCode(Validation_MaxLength)` | `.max(200, 'El título no puede superar los 200 caracteres')` |
| descripcion | max 4000 | `.MaximumLength(4000).When(x => !string.IsNullOrEmpty(x.Descripcion)).WithErrorCode(Validation_MaxLength)` | `.max(4000, 'La descripción no puede superar los 4000 caracteres').optional()` |
| tipoNecesidadId | requerido | `.NotEmpty().WithErrorCode(Validation_Required)` | `.positive('El tipo de necesidad es obligatorio')` |
| modalidadTrabajoId | requerido | `.NotEmpty().WithErrorCode(Validation_Required)` | `.positive('La modalidad de trabajo es obligatoria')` |
| presupuestoMin | >= 0 | `.GreaterThanOrEqualTo(0).When(x => x.PresupuestoMin.HasValue).WithErrorCode(Validation_InvalidRange)` | `.nonnegative('El presupuesto mínimo no puede ser negativo').optional()` |
| presupuestoMax | >= min | `.GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0).When(x => x.PresupuestoMax.HasValue).WithErrorCode(Validation_InvalidRange)` | `refine(max >= min, 'El máximo debe ser >= al mínimo')` |
| monedaId | requerido si presupuesto | `.NotEmpty().When(x => x.PresupuestoMin.HasValue \|\| x.PresupuestoMax.HasValue).WithErrorCode(Validation_Required)` | `.positive().optional()` con refine |
| ubicacionCiudad | requerido si presencial/híbrido | `.NotEmpty().When(x => x.ModalidadTrabajoId == 1 \|\| x.ModalidadTrabajoId == 3).WithErrorCode(Validation_Required)` | `refine si modalidad 1 o 3` |
| fechaLimitePropuestas | > hoy | `.GreaterThan(DateTime.UtcNow.Date).When(x => x.FechaLimitePropuestas.HasValue).WithErrorCode(Validation_InvalidDate)` | `refine(fecha > hoy)` |
| fechaInicioPrevista | >= hoy | `.GreaterThanOrEqualTo(DateTime.UtcNow.Date).When(x => x.FechaInicioPrevista.HasValue).WithErrorCode(Validation_InvalidDate)` | `refine(fecha >= hoy)` |
| proyectoArtisticoId | requerido (create) | `.NotEmpty().WithErrorCode(Validation_Required)` | `.min(1, 'El proyecto es obligatorio')` |
| motivo | max 500 | `.MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Motivo)).WithErrorCode(Validation_MaxLength)` | `.max(500).optional()` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
import { z } from 'zod';

export const createNecesidadSchema = z.object({
  titulo: z
    .string()
    .min(1, 'El título es obligatorio')
    .min(5, 'El título debe tener al menos 5 caracteres')
    .max(200, 'El título no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(4000, 'La descripción no puede superar los 4000 caracteres')
    .optional(),
  tipoNecesidadId: z
    .number()
    .positive('El tipo de necesidad es obligatorio'),
  modalidadTrabajoId: z
    .number()
    .positive('La modalidad de trabajo es obligatoria'),
  presupuestoMin: z
    .number()
    .nonnegative('El presupuesto mínimo no puede ser negativo')
    .optional(),
  presupuestoMax: z
    .number()
    .nonnegative('El presupuesto máximo no puede ser negativo')
    .optional(),
  monedaId: z
    .number()
    .positive('La moneda es obligatoria')
    .optional(),
  ubicacionCiudad: z.string().max(100).optional(),
  ubicacionPais: z.string().max(100).optional(),
  fechaLimitePropuestas: z.string().optional(),
  fechaInicioPrevista: z.string().optional(),
  proyectoArtisticoId: z
    .string()
    .min(1, 'El proyecto artístico es obligatorio'),
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
).refine(
  (data) => {
    if (data.presupuestoMin !== undefined || data.presupuestoMax !== undefined) {
      return data.monedaId !== undefined;
    }
    return true;
  },
  {
    message: 'La moneda es obligatoria cuando se especifica presupuesto',
    path: ['monedaId'],
  }
).refine(
  (data) => {
    if (data.modalidadTrabajoId === 1 || data.modalidadTrabajoId === 3) {
      return data.ubicacionCiudad && data.ubicacionCiudad.length > 0;
    }
    return true;
  },
  {
    message: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
    path: ['ubicacionCiudad'],
  }
).refine(
  (data) => {
    if (data.fechaLimitePropuestas) {
      const limite = new Date(data.fechaLimitePropuestas);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return limite > hoy;
    }
    return true;
  },
  {
    message: 'La fecha límite debe ser posterior a hoy',
    path: ['fechaLimitePropuestas'],
  }
).refine(
  (data) => {
    if (data.fechaInicioPrevista) {
      const inicio = new Date(data.fechaInicioPrevista);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return inicio >= hoy;
    }
    return true;
  },
  {
    message: 'La fecha de inicio debe ser igual o posterior a hoy',
    path: ['fechaInicioPrevista'],
  }
);

export const updateNecesidadSchema = z.object({
  titulo: z
    .string()
    .min(1, 'El título es obligatorio')
    .min(5, 'El título debe tener al menos 5 caracteres')
    .max(200, 'El título no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(4000, 'La descripción no puede superar los 4000 caracteres')
    .optional(),
  modalidadTrabajoId: z
    .number()
    .positive('La modalidad de trabajo es obligatoria'),
  presupuestoMin: z
    .number()
    .nonnegative('El presupuesto mínimo no puede ser negativo')
    .optional(),
  presupuestoMax: z
    .number()
    .nonnegative('El presupuesto máximo no puede ser negativo')
    .optional(),
  monedaId: z
    .number()
    .positive('La moneda es obligatoria')
    .optional(),
  ubicacionCiudad: z.string().max(100).optional(),
  ubicacionPais: z.string().max(100).optional(),
  fechaLimitePropuestas: z.string().optional(),
  fechaInicioPrevista: z.string().optional(),
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
).refine(
  (data) => {
    if (data.presupuestoMin !== undefined || data.presupuestoMax !== undefined) {
      return data.monedaId !== undefined;
    }
    return true;
  },
  {
    message: 'La moneda es obligatoria cuando se especifica presupuesto',
    path: ['monedaId'],
  }
).refine(
  (data) => {
    if (data.modalidadTrabajoId === 1 || data.modalidadTrabajoId === 3) {
      return data.ubicacionCiudad && data.ubicacionCiudad.length > 0;
    }
    return true;
  },
  {
    message: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
    path: ['ubicacionCiudad'],
  }
).refine(
  (data) => {
    if (data.fechaLimitePropuestas) {
      const limite = new Date(data.fechaLimitePropuestas);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return limite > hoy;
    }
    return true;
  },
  {
    message: 'La fecha límite debe ser posterior a hoy',
    path: ['fechaLimitePropuestas'],
  }
).refine(
  (data) => {
    if (data.fechaInicioPrevista) {
      const inicio = new Date(data.fechaInicioPrevista);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return inicio >= hoy;
    }
    return true;
  },
  {
    message: 'La fecha de inicio debe ser igual o posterior a hoy',
    path: ['fechaInicioPrevista'],
  }
);

export const cerrarNecesidadSchema = z.object({
  motivo: z
    .string()
    .max(500, 'El motivo no puede superar los 500 caracteres')
    .optional(),
});

export type CreateNecesidadFormData = z.infer<typeof createNecesidadSchema>;
export type UpdateNecesidadFormData = z.infer<typeof updateNecesidadSchema>;
export type CerrarNecesidadFormData = z.infer<typeof cerrarNecesidadSchema>;
```

---

## 🔗 Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Query Keys additions
export const QUERY_KEYS = {
  // ... existing keys

  crowdsourcing: {
    templates: {
      all: ['crowdsourcing', 'templates'] as const,
      byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
    },
    maestras: {
      rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
      categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
      tiposNecesidad: ['crowdsourcing', 'maestras', 'tipos-necesidad'] as const,
      modalidadesTrabajo: ['crowdsourcing', 'maestras', 'modalidades'] as const,
      monedas: ['crowdsourcing', 'maestras', 'monedas'] as const,
    },
    necesidades: {
      mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
      byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
    },
  },
};

// API Routes additions
export const API_ROUTES = {
  // ... existing routes

  crowdsourcing: {
    templates: {
      base: '/api/crowdsourcing/templates',
      byId: (id: string) => `/api/crowdsourcing/templates/${id}`,
      generar: (id: string) => `/api/crowdsourcing/templates/${id}/generar`,
    },
    maestras: {
      rolesProfesionales: '/api/crowdsourcing/maestras/roles-profesionales',
      categoriasRol: '/api/crowdsourcing/maestras/categorias-rol',
      tiposNecesidad: '/api/crowdsourcing/maestras/tipos-necesidad',
      modalidadesTrabajo: '/api/crowdsourcing/maestras/modalidades-trabajo',
      monedas: '/api/crowdsourcing/maestras/monedas',
    },
    necesidades: {
      base: '/api/crowdsourcing/necesidades',
      mis: '/api/crowdsourcing/necesidades/mis-necesidades',
      byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
      cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
    },
  },
};

// App Routes additions
export const APP_ROUTES = {
  // ... existing routes

  dashboard: {
    // ... existing dashboard routes
    crowdsourcing: {
      templates: '/dashboard/crowdsourcing/templates',
      templateDetail: (id: string) => `/dashboard/crowdsourcing/templates/${id}`,
      wizard: (projectId: string) => `/dashboard/crowdsourcing/wizard/${projectId}`,
      necesidades: '/dashboard/crowdsourcing/necesidades',
      nuevaNecesidad: '/dashboard/crowdsourcing/necesidades/nueva',
      necesidadDetail: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}`,
      editarNecesidad: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}/editar`,
    },
  },
};

// Estado de Necesidad (int IDs from maestras)
export const ESTADO_NECESIDAD = {
  ABIERTA: 1,
  EN_PROGRESO: 2,
  CERRADA: 3,
  CANCELADA: 4,
} as const;

export const ESTADO_NECESIDAD_LABELS: Record<number, string> = {
  1: 'Abierta',
  2: 'En Progreso',
  3: 'Cerrada',
  4: 'Cancelada',
};

export const ESTADO_NECESIDAD_BADGES: Record<number, string> = {
  1: 'green',    // Abierta - verde
  2: 'blue',     // En Progreso - azul
  3: 'gray',     // Cerrada - gris
  4: 'red',      // Cancelada - rojo
};

// Modalidad de Trabajo (int IDs from maestras)
export const MODALIDAD_TRABAJO = {
  PRESENCIAL: 1,
  REMOTO: 2,
  HIBRIDO: 3,
} as const;

export const MODALIDAD_TRABAJO_LABELS: Record<number, string> = {
  1: 'Presencial',
  2: 'Remoto',
  3: 'Híbrido',
};

export const MODALIDAD_TRABAJO_ICONS: Record<number, string> = {
  1: '📍',  // Presencial
  2: '💻',  // Remoto
  3: '🔄',  // Híbrido
};

// Monedas comunes
export const MONEDA = {
  EUR: 1,
  USD: 2,
  GBP: 3,
} as const;

export const MONEDA_SIMBOLOS: Record<number, string> = {
  1: '€',
  2: '$',
  3: '£',
};

// Helper para formatear presupuesto
export const formatPresupuesto = (min?: number, max?: number, monedaId?: number): string => {
  if (!min && !max) return 'No especificado';
  const simbolo = monedaId ? MONEDA_SIMBOLOS[monedaId] ?? '€' : '€';
  if (min && max) return `${simbolo}${min} - ${simbolo}${max}`;
  if (min) return `Desde ${simbolo}${min}`;
  if (max) return `Hasta ${simbolo}${max}`;
  return 'No especificado';
};
```

---

## 📊 Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... existing error messages

  // Crowdsourcing - Necesidades errors
  '1011': 'El título debe tener al menos 5 caracteres',
  '1012': 'La fecha no es válida',
  '2009': 'La necesidad no fue encontrada',
  '4001': 'Solo se pueden editar necesidades en estado Abierta',
  '4002': 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',

  // Friendly messages
  NECESIDAD_NOT_FOUND: 'La necesidad seleccionada no existe',
  NECESIDAD_NOT_EDITABLE: 'Solo se pueden editar necesidades en estado Abierta',
  NECESIDAD_NOT_CLOSEABLE: 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',
  PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artístico no fue encontrado',
  PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para crear necesidades en este proyecto',
  VALIDATION_TITULO_MIN_LENGTH: 'El título debe tener al menos 5 caracteres',
  VALIDATION_FECHA_INVALIDA: 'La fecha ingresada no es válida',
  VALIDATION_UBICACION_REQUERIDA: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
  VALIDATION_PRESUPUESTO_MONEDA_REQUERIDA: 'Debe especificar la moneda cuando indica presupuesto',
  VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto máximo debe ser mayor o igual al mínimo',
};
```

---

## 🔔 Eventos SignalR

**No aplica para esta feature.** La gestión de necesidades es un flujo síncrono sin necesidad de notificaciones en tiempo real. Las actualizaciones se reflejan mediante refresh de queries en el cliente.

**Nota:** En futuras iteraciones se podría agregar SignalR para notificar al artista cuando recibe nuevas propuestas en sus necesidades abiertas.

---

## 📝 Notas de Implementación

### Backend

1. **ServiceResponseMessageType Constants (nuevo):**
   Agregar al archivo `Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`:
   ```csharp
   // Validation (1000-1999)
   public const string Validation_MinLength = "1011";
   public const string Validation_InvalidDate = "1012";

   // NotFound (2000-2999)
   public const string NotFound_Necesidad = "2009";

   // Business Rules (4000-4999)
   public const string BusinessRule_NecesidadNotEditable = "4001";
   public const string BusinessRule_NecesidadNotCloseable = "4002";
   ```

2. **Servicios:**
   - `INecesidadCrowdsourcingService` - CRUD de necesidades
   - `IProyectoArtisticoService` - Validar ownership del proyecto (reutilizar del módulo UserAccess)
   - `IPropuestaCrowdsourcingService` - Rechazar propuestas al cerrar necesidad

3. **Validaciones críticas en Validators:**
   - **CreateNecesidadValidator:**
     - Validar que el `proyectoArtisticoId` existe y pertenece al artista (usar `IProyectoArtisticoService` con caching)
     - Validar que `tipoNecesidadId`, `modalidadTrabajoId`, `monedaId` existan en maestras
     - Validar fechas (futuras)
     - Validar ubicación requerida para modalidad Presencial/Híbrido
     - Validar presupuesto (max >= min, moneda requerida si presupuesto presente)

   - **UpdateNecesidadValidator:**
     - Validar que la necesidad existe y está en estado `Abierta` (EstadoNecesidadId == 1)
     - Validar ownership (ArtistaId del token == ArtistaId de la necesidad)
     - Mismas validaciones de campos que Create (excepto proyectoArtisticoId y tipoNecesidadId)

   - **CerrarNecesidadValidator:**
     - Validar que la necesidad existe y está en estado `Abierta` (1) o `En Progreso` (2)
     - Validar ownership
     - Validar motivo (max 500 caracteres)

4. **Lógica de negocio:**

   **CreateNecesidadCommand:**
   - Obtener ArtistaId del claim del token JWT
   - Crear entidad NecesidadCrowdsourcing con estado `Abierta` (EstadoNecesidadId = 1)
   - Establecer FechaCreacion = DateTime.UtcNow
   - Retornar ServiceResponse con summary (id, titulo, estado, fechaCreacion)

   **GetMisNecesidadesQuery:**
   - Filtrar por ArtistaId del token
   - Aplicar filtros opcionales (estado, search)
   - Proyectar a NecesidadCrowdsourcingListDto con Include de maestras
   - Agregar COUNT de PropuestaCrowdsourcing.Where(p => p.NecesidadId == n.Id)
   - Retornar ServiceResponse con PaginatedResponse

   **GetNecesidadByIdQuery:**
   - Validar ownership (ArtistaId del token == ArtistaId de la necesidad)
   - Include PropuestaCrowdsourcing con PerfilProfesional y User
   - Proyectar a NecesidadCrowdsourcingDto completo
   - Retornar ServiceResponse

   **UpdateNecesidadCommand:**
   - Validar estado == Abierta en Validator (retornar ServiceResponse con BusinessRule_NecesidadNotEditable si falla)
   - Actualizar campos editables (todos excepto TipoNecesidadId y ProyectoArtisticoId)
   - Establecer FechaActualizacion = DateTime.UtcNow
   - Retornar ServiceResponse con summary

   **CerrarNecesidadCommand:**
   - Validar estado in (Abierta, EnProgreso) en Validator
   - Actualizar EstadoNecesidadId = 3 (Cerrada)
   - Obtener todas las PropuestaCrowdsourcing con estado Pendiente (1)
   - Actualizar todas a estado Rechazada (3) con motivo "Necesidad cerrada por el artista"
   - Contar propuestas rechazadas
   - Retornar ServiceResponse con { id, estadoNecesidadNombre, propuestasRechazadas }

5. **Caching:**
   - `IProyectoArtisticoService.GetByIdAsync` debe usar RequestCache para evitar queries duplicadas en Validator y Handler
   - Cache de maestras (TipoNecesidad, ModalidadTrabajo, Moneda) en validadores

6. **Autorización:**
   - Controller con `[Authorize(Roles = "Artista")]`
   - Validar ownership del proyecto en Create (el proyecto debe pertenecer al artista autenticado)
   - Validar ownership de la necesidad en Get/Update/Cerrar

### Frontend

1. **Nuevos archivos:**
   - `src/shared/types/crowdsourcing.ts` - Agregar tipos de necesidades
   - `src/shared/schemas/crowdsourcing.schema.ts` - Agregar schemas de necesidades
   - `src/shared/constants/index.ts` - Agregar constantes de necesidades
   - `src/admin/src/features/crowdsourcing/necesidades/` - Feature completa

2. **Componentes clave:**
   - `NecesidadesListPage.tsx` - Listado con filtros y paginación
   - `NecesidadCard.tsx` - Card con badge de estado, presupuesto, propuestas counter
   - `NuevaNecesidadPage.tsx` - Formulario de creación
   - `NecesidadDetailPage.tsx` - Detalle con propuestas recibidas
   - `EditarNecesidadPage.tsx` - Formulario de edición
   - `CerrarNecesidadDialog.tsx` - Diálogo de confirmación con motivo opcional
   - `EstadoNecesidadBadge.tsx` - Badge con colores según estado

3. **Hooks personalizados:**
   - `useMisNecesidades(filters)` - Query para listar con filtros
   - `useNecesidadDetail(id)` - Query para detalle
   - `useCreateNecesidad()` - Mutation para crear
   - `useUpdateNecesidad(id)` - Mutation para editar
   - `useCerrarNecesidad(id)` - Mutation para cerrar
   - `useTiposNecesidad()` - Query para maestras
   - `useModalidadesTrabajo()` - Query para maestras
   - `useMonedas()` - Query para maestras

4. **Formularios:**
   - React Hook Form + Zod resolver
   - Campos condicionales:
     - `ubicacionCiudad` y `ubicacionPais` solo si modalidad = Presencial (1) o Híbrido (3)
     - `monedaId` requerido si presupuestoMin o presupuestoMax presentes
   - Validación en tiempo real con feedback visual
   - DatePickers para fechaLimitePropuestas y fechaInicioPrevista

5. **UX considerations:**
   - Badge de estado con colores semánticos (verde/azul/gris/rojo)
   - Contador de propuestas destacado visualmente
   - Filtros de estado con chips/pills activos
   - Empty state amigable con CTA hacia crear o usar template
   - Confirmación antes de cerrar necesidad con advertencia sobre propuestas pendientes
   - Toast de éxito/error en todas las acciones
   - Indicador de urgencia si fechaLimitePropuestas < 3 días
   - Disable de botón "Editar" si estado != Abierta
   - Disable de botón "Cerrar" si estado != Abierta y != EnProgreso
   - Aviso si edita necesidad con propuestas existentes: "Esta necesidad ya tiene N propuestas. Los cambios serán visibles para los profesionales."

6. **Navegación:**
   - Desde listado → click card → detalle con propuestas
   - Desde detalle → botón "Editar" (si Abierta) → formulario edición
   - Desde detalle → botón "Cerrar" → diálogo confirmación
   - Desde listado → botón "Nueva Necesidad" → formulario creación
   - Link a templates si el artista prefiere usar plantillas

7. **Integración con Templates:**
   - Si el artista viene del wizard de templates (US-CS-01), el formulario se pre-rellena
   - Pero todos los campos son editables antes de guardar
   - Indicador visual de que viene de template (opcional)

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores
- [x] Autorización por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas (incluyendo refines)
- [x] Constantes compartidas (query keys, API routes, app routes)
- [x] Nuevas constantes de dominio (ESTADO_NECESIDAD, MODALIDAD_TRABAJO, MONEDA)
- [x] Nuevos ServiceResponseMessageType constants (1011, 1012, 2009, 4001, 4002)
- [x] Mapeo de errores a mensajes UI
- [x] Helper formatPresupuesto para UI
- [x] Notas de implementación detalladas (Backend + Frontend)
