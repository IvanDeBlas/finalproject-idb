# Contratos: Hacer Backing (Aporte a Campania)

> **Feature:** hacer-backing (US-04)
> **Ultima actualizacion:** 2026-02-13

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### GET /api/campanias

**Descripcion:** Lista campanias activas con paginacion. Endpoint publico usado para explorar campanias disponibles para hacer backing.

**Autorizacion:** ❌ Publica

**Query Parameters:**
- `searchTerm` (string, opcional): Busqueda por titulo o descripcion
- `artistaId` (guid, opcional): Filtra por artista especifico
- `estadoCampaniaId` (int, opcional): Filtra por estado (default 2 = PUBLICADA)
- `pageNumber` (int, default 1): Numero de pagina
- `pageSize` (int, default 10): Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "titulo": "Mi Album Debut",
        "subtitulo": "Rock alternativo desde Madrid",
        "importeObjetivo": 5000.00,
        "importePledgedActual": 2340.00,
        "porcentajeProgreso": 46.8,
        "estadoCampaniaId": 2,
        "fechaInicio": "2026-02-01T00:00:00Z",
        "fechaFin": "2026-03-31T23:59:59Z",
        "artistaNombre": "Juan Perez Music",
        "artistaImagenUrl": "https://...",
        "imagenPrincipalUrl": "https://..."
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
  },
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

### GET /api/campanias/{id}

**Descripcion:** Obtiene el detalle completo de una campania, incluyendo recompensas activas y aportes recientes. Este endpoint es el principal para mostrar la pagina de detalle donde el usuario decide hacer backing.

**Autorizacion:** ❌ Publica

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "artistaId": "guid",
    "titulo": "Mi Album Debut",
    "subtitulo": "Rock alternativo desde Madrid",
    "descripcionCorta": "Un viaje musical de 10 canciones...",
    "videoPrincipalUrl": "https://youtube.com/watch?v=...",
    "imagenPrincipalUrl": "https://...",
    "importeObjetivo": 5000.00,
    "importeMinimo": 1000.00,
    "importePledgedActual": 2340.00,
    "porcentajeProgreso": 46.8,
    "monedaId": 1,
    "monedaSimbolo": "EUR",
    "estadoCampaniaId": 2,
    "estadoCampaniaNombre": "Publicada",
    "permiteAportacionesAnonimas": true,
    "permitePropinas": true,
    "fechaInicio": "2026-02-01T00:00:00Z",
    "fechaFin": "2026-03-31T23:59:59Z",
    "diasRestantes": 46,
    "artistaNombre": "Juan Perez Music",
    "artistaImagenUrl": "https://...",
    "rewards": [
      {
        "id": "guid",
        "nombre": "Descarga Digital",
        "descripcion": "Album completo en MP3 y FLAC",
        "importeMinimo": 10.00,
        "cantidadMaxima": null,
        "cantidadVendida": 45,
        "disponible": true,
        "incluyeEnvioFisico": false,
        "tiempoEntregaEstimado": "Inmediato tras finalizar campania",
        "orden": 1,
        "esActivo": true
      },
      {
        "id": "guid",
        "nombre": "CD Fisico Firmado",
        "descripcion": "CD fisico del album con firma del artista",
        "importeMinimo": 25.00,
        "cantidadMaxima": 100,
        "cantidadVendida": 78,
        "disponible": true,
        "incluyeEnvioFisico": true,
        "tiempoEntregaEstimado": "60 dias tras finalizar campania",
        "orden": 2,
        "esActivo": true
      }
    ],
    "backingsRecientes": [
      {
        "id": "guid",
        "nombreBacker": "Maria Lopez",
        "monto": 25.00,
        "rewardNombre": "CD Fisico Firmado",
        "fechaCreacion": "2026-02-13T10:30:00Z"
      },
      {
        "id": "guid",
        "nombreBacker": "Anonimo",
        "monto": 50.00,
        "rewardNombre": null,
        "fechaCreacion": "2026-02-13T09:15:00Z"
      }
    ],
    "totalBackers": 234,
    "fechaCreacion": "2026-01-15T12:00:00Z"
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

### POST /api/campanias/{id}/backings

**Descripcion:** Crea un nuevo aporte (backing) a una campania. Crea un PedidoCrowdfunding con PedidoCrowdfundingLinea asociado. Actualiza ImportePledgedActual de la campania atomicamente. Si el usuario esta autenticado, usa su UserId. Si no, permite backing anonimo (si la campania lo permite). Para MVP, no procesa pago real - marca el pedido como completado inmediatamente.

**Autorizacion:** 🔓 Opcional (Bearer JWT si autenticado, anonimo si campaign.PermiteAportacionesAnonimas = true)

**Request Body:**
```json
{
  "rewardId": "guid (opcional, null para aporte sin recompensa)",
  "monto": "decimal (requerido, >= 1.00 EUR, >= reward.ImporteMinimo si reward seleccionado)",
  "mensaje": "string (opcional, max 500 caracteres, comentario del backer)",
  "esAnonimo": "boolean (default false, si true no muestra nombre en lista publica)"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "campaniaId": "guid",
    "campaniaTitulo": "Mi Album Debut",
    "rewardId": "guid",
    "rewardNombre": "CD Fisico Firmado",
    "monto": 25.00,
    "monedaSimbolo": "EUR",
    "mensaje": "Mucha suerte con el proyecto!",
    "esAnonimo": false,
    "userName": "Maria Lopez",
    "fechaCreacion": "2026-02-13T10:30:00Z",
    "estadoPedido": "Completado"
  },
  "messages": [
    {
      "message": "Aporte realizado con exito. Gracias por tu apoyo!",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El monto es obligatorio | monto no proporcionado |
| 400 | 1011 | El monto debe ser al menos 1 EUR | monto < 1 |
| 400 | 4012 | El monto debe ser al menos {reward.ImporteMinimo} EUR para esta recompensa | monto < reward.ImporteMinimo cuando reward seleccionado |
| 400 | 1002 | El mensaje no puede superar los 500 caracteres | mensaje > 500 chars |
| 400 | 1012 | El rewardId no es valido | rewardId no es un GUID valido |
| 401 | 3005 | Debes iniciar sesion para hacer un aporte | Usuario no autenticado y campania no permite anonimos |
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 404 | 2004 | Recompensa no encontrada | rewardId no existe en DB |
| 409 | 4006 | Esta campania no esta activa | campania.EstadoCampaniaId != 2 (PUBLICADA) |
| 409 | 4007 | Esta campania ya finalizo | DateTime.UtcNow > campania.FechaFin |
| 409 | 4011 | Esta recompensa esta agotada | reward.CantidadMaxima != null && cantidadVendida >= CantidadMaxima |
| 422 | 4013 | Esta campania no permite aportes anonimos | esAnonimo = true (o user no auth) y campania.PermiteAportacionesAnonimas = false |
| 500 | 5000 | Error inesperado al procesar el aporte | Excepcion no controlada |

---

### GET /api/campanias/{id}/backings

**Descripcion:** Lista aportes recientes de una campania para visualizacion publica. Muestra nombre del backer (o "Anonimo"), monto, recompensa seleccionada, y fecha. Soporta paginacion.

**Autorizacion:** ❌ Publica

**Query Parameters:**
- `pageNumber` (int, default 1): Numero de pagina
- `pageSize` (int, default 20): Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "nombreBacker": "Maria Lopez",
        "monto": 25.00,
        "rewardNombre": "CD Fisico Firmado",
        "mensaje": "Mucha suerte con el proyecto!",
        "fechaCreacion": "2026-02-13T10:30:00Z"
      },
      {
        "id": "guid",
        "nombreBacker": "Anonimo",
        "monto": 50.00,
        "rewardNombre": null,
        "mensaje": null,
        "fechaCreacion": "2026-02-13T09:15:00Z"
      },
      {
        "id": "guid",
        "nombreBacker": "Carlos Sanchez",
        "monto": 10.00,
        "rewardNombre": "Descarga Digital",
        "mensaje": null,
        "fechaCreacion": "2026-02-13T08:45:00Z"
      }
    ],
    "totalCount": 234,
    "page": 1,
    "pageSize": 20,
    "totalPages": 12
  },
  "messages": [
    {
      "message": "Aportes encontrados",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### GET /api/campanias/{id}/stats

**Descripcion:** Obtiene estadisticas agregadas de aportes de una campania. Usado para mostrar metricas en la pagina de detalle.

**Autorizacion:** ❌ Publica

**Response 200 OK:**
```json
{
  "data": {
    "campaniaId": "guid",
    "totalBackers": 234,
    "totalRecaudado": 2340.00,
    "promedioAporte": 10.00,
    "aporteMinimo": 1.00,
    "aporteMaximo": 100.00,
    "diasRestantes": 46
  },
  "messages": [
    {
      "message": "Estadisticas obtenidas",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

## 🔐 Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `GET /api/campanias` | ❌ | - | Publico, explorar campanias |
| `GET /api/campanias/{id}` | ❌ | - | Publico, detalle de campania |
| `POST /api/campanias/{id}/backings` | 🔓 | - | Opcional. Requerido si campania no permite anonimos |
| `GET /api/campanias/{id}/backings` | ❌ | - | Publico, ver aportes de campania |
| `GET /api/campanias/{id}/stats` | ❌ | - | Publico, estadisticas de campania |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string, Identity User ID)",
  "email": "fan@example.com",
  "name": "Maria Lopez",
  "exp": 1739000000,
  "iat": 1738913600
}
```

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas por defecto
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

**Nota sobre autenticacion opcional:**
- Si el usuario esta autenticado, usar claim `sub` para `UserId` y `name` para mostrar nombre
- Si el usuario NO esta autenticado:
  - Validar que `campania.PermiteAportacionesAnonimas = true`
  - Crear `PedidoCrowdfunding` con `UserId = null`
  - Nombre mostrado sera "Anonimo"

### Proteccion de Rutas Frontend

| Ruta (Landing) | Auth | Redirect | Notas |
|----------------|------|----------|-------|
| `/campanias` | ❌ | - | Publico, lista de campanias |
| `/campanias/{id}` | ❌ | - | Publico, detalle de campania con recompensas |
| `/campanias/{id}/apoyar` | 🔓 | `/auth/login` (si campania no permite anonimos) | Formulario de backing |

**Flujo de autenticacion condicional:**
1. Usuario hace clic en "Apoyar" en detalle de campania
2. Si `campania.PermiteAportacionesAnonimas = true` → Mostrar formulario inmediatamente (opcion de login opcional)
3. Si `campania.PermiteAportacionesAnonimas = false` → Verificar autenticacion:
   - Autenticado → Mostrar formulario
   - No autenticado → Redirect a `/auth/login?returnUrl=/campanias/{id}/apoyar`

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/BackingDto.cs
public class BackingDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string? UserId { get; set; } // null para anonimos
    public Guid? RewardId { get; set; } // null para aporte sin recompensa
    public decimal Monto { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Campos populados
    public string CampaniaTitulo { get; set; } = null!;
    public string? UserName { get; set; } // "Anonimo" si EsAnonimo o UserId null
    public string? RewardNombre { get; set; }
    public string MonedaSimbolo { get; set; } = null!;
    public string EstadoPedido { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/BackingPublicDto.cs
// DTO reducido para lista publica de aportes
public class BackingPublicDto
{
    public Guid Id { get; set; }
    public string NombreBacker { get; set; } = null!; // "Anonimo" o nombre real
    public decimal Monto { get; set; }
    public string? RewardNombre { get; set; }
    public string? Mensaje { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaStatsDto.cs
public class CampaniaStatsDto
{
    public Guid CampaniaId { get; set; }
    public int TotalBackers { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PromedioAporte { get; set; }
    public decimal AporteMinimo { get; set; }
    public decimal AporteMaximo { get; set; }
    public int DiasRestantes { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaDetailDto.cs
// NUEVO - Extension de CampaniaDto con rewards y backings
public class CampaniaDetailDto
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public decimal ImportePledgedActual { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int MonedaId { get; set; }
    public string MonedaSimbolo { get; set; } = null!;
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int DiasRestantes { get; set; }
    public string ArtistaNombre { get; set; } = null!;
    public string? ArtistaImagenUrl { get; set; }
    public List<RewardPublicDto> Rewards { get; set; } = new();
    public List<BackingPublicDto> BackingsRecientes { get; set; } = new();
    public int TotalBackers { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardPublicDto.cs
// DTO reducido de reward para mostrar en detalle de campania
public class RewardPublicDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int? CantidadMaxima { get; set; }
    public int CantidadVendida { get; set; } // Calculado desde PedidoCrowdfundingLinea
    public bool Disponible { get; set; } // CantidadMaxima == null || CantidadVendida < CantidadMaxima
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Backings/Commands/CreateBackingCommand.cs
public class CreateBackingCommand : IRequest<ServiceResponse<BackingDto>>
{
    public Guid CampaniaId { get; set; }
    public Guid? RewardId { get; set; }
    public decimal Monto { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }

    // Inyectado desde controller via HttpContext.User
    public string? UserId { get; set; } // Asignado desde JWT claim "sub" si autenticado
    public string? UserName { get; set; } // Asignado desde JWT claim "name" si autenticado
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/backing.ts
export interface Backing {
  id: string;
  campaniaId: string;
  userId?: string; // undefined para anonimos
  rewardId?: string; // undefined para aporte sin recompensa
  monto: number;
  mensaje?: string;
  esAnonimo: boolean;
  fechaCreacion: string; // ISO 8601
}

export interface BackingDto extends Backing {
  campaniaTitulo: string;
  userName?: string; // "Anonimo" si esAnonimo o userId undefined
  rewardNombre?: string;
  monedaSimbolo: string;
  estadoPedido: string;
}

export interface BackingPublicDto {
  id: string;
  nombreBacker: string; // "Anonimo" o nombre real
  monto: number;
  rewardNombre?: string;
  mensaje?: string;
  fechaCreacion: string; // ISO 8601
}

export interface CreateBackingRequest {
  rewardId?: string;
  monto: number;
  mensaje?: string;
  esAnonimo?: boolean;
}

export interface CampaniaStats {
  campaniaId: string;
  totalBackers: number;
  totalRecaudado: number;
  promedioAporte: number;
  aporteMinimo: number;
  aporteMaximo: number;
  diasRestantes: number;
}
```

```typescript
// Ruta: src/shared/types/campania.ts
export interface CampaniaDetail {
  id: string;
  artistaId: string;
  titulo: string;
  subtitulo?: string;
  descripcionCorta?: string;
  videoPrincipalUrl?: string;
  imagenPrincipalUrl?: string;
  importeObjetivo: number;
  importeMinimo?: number;
  importePledgedActual: number;
  porcentajeProgreso: number;
  monedaId: number;
  monedaSimbolo: string;
  estadoCampaniaId: number;
  estadoCampaniaNombre: string;
  permiteAportacionesAnonimas: boolean;
  permitePropinas: boolean;
  fechaInicio?: string; // ISO 8601
  fechaFin?: string; // ISO 8601
  diasRestantes: number;
  artistaNombre: string;
  artistaImagenUrl?: string;
  rewards: RewardPublic[];
  backingsRecientes: BackingPublicDto[];
  totalBackers: number;
  fechaCreacion: string; // ISO 8601
}

export interface RewardPublic {
  id: string;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  cantidadMaxima?: number; // null = ilimitado
  cantidadVendida: number;
  disponible: boolean;
  incluyeEnvioFisico: boolean;
  tiempoEntregaEstimado?: string;
  orden: number;
  esActivo: boolean;
}
```

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| campaniaId | requerido | `.NotEmpty().WithErrorCode("1001")` | `.string().uuid('ID invalido')` |
| rewardId | opcional, guid valido | `.Must(BeValidGuid).When(x => x.RewardId.HasValue).WithErrorCode("1012")` | `.string().uuid('ID invalido').optional()` |
| monto | requerido, >= 1 | `.GreaterThanOrEqualTo(1).WithErrorCode("1011")` | `.number().min(1, 'Minimo 1 EUR')` |
| monto | >= reward.ImporteMinimo | Custom validator (async check reward).WithErrorCode("4012") | Validacion manual en submit (client-side) |
| mensaje | max 500 | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres').optional()` |
| esAnonimo | boolean | `.NotNull()` | `.boolean().default(false)` |

### Validaciones de Negocio Adicionales

| Regla | Backend | Frontend |
|-------|---------|----------|
| Campania debe estar PUBLICADA | `EstadoCampaniaId == 2` (error 4006) | Deshabilitar boton "Apoyar" si estado != 2 |
| Campania no debe haber finalizado | `DateTime.UtcNow <= FechaFin` (error 4007) | Mostrar "Campania finalizada" en lugar de boton |
| Reward debe tener stock | `CantidadMaxima == null || CantidadVendida < CantidadMaxima` (error 4011) | Deshabilitar reward si `!disponible` |
| Anonimos solo si campania permite | `PermiteAportacionesAnonimas || UserId != null` (error 4013) | Mostrar modal login si no permite anonimos |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/backing.schema.ts
import { z } from 'zod';

export const createBackingSchema = z.object({
  rewardId: z
    .string()
    .uuid('ID de recompensa invalido')
    .optional(),
  monto: z
    .number({ invalid_type_error: 'El monto debe ser un numero' })
    .min(1, 'El monto minimo es 1 EUR')
    .max(100000, 'El monto maximo es 100,000 EUR'),
  mensaje: z
    .string()
    .max(500, 'El mensaje no puede superar los 500 caracteres')
    .optional()
    .or(z.literal('')),
  esAnonimo: z
    .boolean()
    .default(false),
});

export type CreateBackingFormData = z.infer<typeof createBackingSchema>;

// Validacion adicional en runtime (client-side)
export const validateBackingAmount = (
  monto: number,
  reward?: RewardPublic
): string | null => {
  if (!reward) return null;

  if (monto < reward.importeMinimo) {
    return `El monto debe ser al menos ${reward.importeMinimo} EUR para esta recompensa`;
  }

  return null;
};

export const validateRewardAvailability = (
  reward?: RewardPublic
): string | null => {
  if (!reward) return null;

  if (!reward.esActivo) {
    return 'Esta recompensa ya no esta disponible';
  }

  if (!reward.disponible) {
    return 'Esta recompensa esta agotada';
  }

  return null;
};
```

---

## 🔗 Constantes Compartidas

```typescript
// Ruta: src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  campanias: {
    all: ['campanias'] as const,
    byId: (id: string) => ['campanias', id] as const,
    detail: (id: string) => ['campanias', id, 'detail'] as const,
    stats: (id: string) => ['campanias', id, 'stats'] as const,
  },
  backings: {
    all: ['backings'] as const,
    byId: (id: string) => ['backings', id] as const,
    byCampania: (campaniaId: string) => ['backings', 'campania', campaniaId] as const,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/api-routes.ts
export const API_ROUTES = {
  campanias: {
    base: '/api/campanias',
    byId: (id: string) => `/api/campanias/${id}`,
    stats: (id: string) => `/api/campanias/${id}/stats`,
    backings: (id: string) => `/api/campanias/${id}/backings`,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/app-routes.ts
export const APP_ROUTES = {
  landing: {
    campanias: {
      list: '/campanias',
      detail: (id: string) => `/campanias/${id}`,
      apoyar: (id: string) => `/campanias/${id}/apoyar`,
    },
  },
  auth: {
    login: '/auth/login',
    register: '/auth/register',
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/campania-states.ts
export const ESTADO_CAMPANIA = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
  PAUSADA: 5,
} as const;

export const ESTADO_CAMPANIA_LABELS: Record<number, string> = {
  1: 'Borrador',
  2: 'Publicada',
  3: 'Finalizada',
  4: 'Cancelada',
  5: 'Pausada',
};
```

```typescript
// Ruta: src/shared/constants/pedido-states.ts
export const ESTADO_PEDIDO = {
  PENDIENTE: 1,
  PROCESANDO: 2,
  COMPLETADO: 3,
  FALLIDO: 4,
  CANCELADO: 5,
} as const;

export const ESTADO_PEDIDO_LABELS: Record<number, string> = {
  1: 'Pendiente',
  2: 'Procesando',
  3: 'Completado',
  4: 'Fallido',
  5: 'Cancelado',
};
```

---

## 📊 Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts
export const BACKING_ERROR_MESSAGES: Record<string, string> = {
  // Success codes
  '0000': 'Operacion exitosa',
  '0001': 'Aporte realizado con exito. Gracias por tu apoyo!',

  // Validation errors (1000-1999)
  '1001': 'Este campo es obligatorio',
  '1002': 'El mensaje no puede superar los 500 caracteres',
  '1011': 'El monto debe ser al menos 1 EUR',
  '1012': 'El ID de recompensa no es valido',
  '4012': 'El monto esta por debajo del minimo requerido para esta recompensa',

  // Not Found errors (2000-2999)
  '2003': 'No encontramos esta campania',
  '2004': 'No encontramos esta recompensa',

  // Auth errors (3000-3999)
  '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
  '3005': 'Debes iniciar sesion para hacer un aporte',

  // Business Rule errors (4000-4999)
  '4006': 'Esta campania no esta activa en este momento',
  '4007': 'Esta campania ya ha finalizado',
  '4011': 'Esta recompensa esta agotada. Por favor, selecciona otra',
  '4013': 'Esta campania no permite aportes anonimos. Por favor, inicia sesion',

  // Internal errors (5000-5999)
  '5000': 'Ha ocurrido un error al procesar tu aporte. Por favor, intenta nuevamente',
} as const;

export const getBackingErrorMessage = (errorCode: string): string => {
  return BACKING_ERROR_MESSAGES[errorCode] || BACKING_ERROR_MESSAGES['5000'];
};

// Helper para mensajes contextuales
export const getBackingContextualError = (
  errorCode: string,
  context?: { rewardNombre?: string; importeMinimo?: number }
): string => {
  if (errorCode === '4012' && context?.importeMinimo) {
    return `El monto debe ser al menos ${context.importeMinimo} EUR para "${context.rewardNombre}"`;
  }

  if (errorCode === '4011' && context?.rewardNombre) {
    return `La recompensa "${context.rewardNombre}" esta agotada. Selecciona otra opcion`;
  }

  return getBackingErrorMessage(errorCode);
};
```

---

## 🔔 Eventos SignalR

**No aplica para MVP.** Los aportes son operaciones sincronas sin necesidad de notificaciones en tiempo real.

**Futuro (post-MVP):** Considerar eventos SignalR para:
1. **BackingCreated** - Notificar al artista cuando recibe un nuevo aporte
2. **CampaignProgressUpdated** - Actualizar en tiempo real el progreso de la campania para usuarios viendo la pagina de detalle
3. **RewardStockLow** - Alertar cuando una recompensa esta por agotarse (< 10 unidades restantes)

Ejemplo de evento futuro:
```typescript
// SignalR Hub: CampaignHub
// Evento: "BackingCreated"
// Payload: { campaniaId, monto, rewardNombre, nombreBacker }
// Suscriptores: Artista propietario, usuarios en pagina /campanias/{id}
```

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (5 endpoints: GET campanias, GET campania/{id}, POST backings, GET backings, GET stats)
- [x] Autorizacion por endpoint (tabla resumen completa con autenticacion opcional)
- [x] Claims JWT documentados (sub, email, name, exp, iat)
- [x] DTOs C# completos (BackingDto, BackingPublicDto, CampaniaStatsDto, CampaniaDetailDto, RewardPublicDto, CreateBackingCommand)
- [x] Types TypeScript equivalentes (Backing, BackingDto, BackingPublicDto, CreateBackingRequest, CampaniaStats, CampaniaDetail, RewardPublic)
- [x] Schemas Zod con mismas reglas (createBackingSchema + validaciones de negocio)
- [x] Constantes compartidas (query keys, API routes, app routes, estados de campania/pedido)
- [x] Mapeo de errores a mensajes UI (todos los codigos + helpers contextuales)
- [x] SignalR evaluado (no aplica para MVP, considerado para post-MVP)

---

## Notas Tecnicas Adicionales

### Nuevas Constantes de Error a Agregar

**IMPORTANTE**: Agregar estas constantes en `ServiceResponseMessageType.cs`:

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs

// Business Rule errors (4000-4999) - NUEVAS
public const string BusinessRule_RewardOutOfStock = "4011";
public const string BusinessRule_AmountBelowMinimum = "4012";
public const string BusinessRule_AnonymousNotAllowed = "4013";
```

### Flujo de Creacion de Backing (MVP Simplificado)

1. Usuario navega a `/campanias/{id}`
2. Frontend carga `GET /api/campanias/{id}` → Obtiene detalle con rewards y backings recientes
3. Usuario selecciona reward (opcional) y monto
4. Frontend valida:
   - Monto >= 1 EUR
   - Si reward seleccionado, monto >= reward.ImporteMinimo
   - Reward disponible (stock)
   - Campania activa (estadoCampaniaId == 2)
5. Usuario hace clic en "Apoyar":
   - Si no esta autenticado y campania no permite anonimos → Redirect a login
   - Si autenticado o campania permite anonimos → Mostrar modal de confirmacion
6. Usuario confirma:
   - Frontend POST `/api/campanias/{id}/backings`
   - Backend crea `PedidoCrowdfunding` con:
     - `UserId` del JWT (o null si anonimo)
     - `EstadoPedidoId = 3` (COMPLETADO - MVP sin pago real)
     - `ImporteSubtotal = monto`, `ImporteTotal = monto` (sin taxes/shipping en MVP)
     - `PermitirMostrarNombre = !esAnonimo`
     - `ComentarioBacker = mensaje`
   - Backend crea `PedidoCrowdfundingLinea` con:
     - `RewardId` seleccionado (o null)
     - `Cantidad = 1`
     - `PrecioUnitario = monto`
     - `ImporteLinea = monto`
     - `EsRewardPrincipal = true`
   - Backend actualiza `CampaniaCrowdfunding.ImportePledgedActual += monto` (atomicamente)
7. Frontend muestra toast de exito y redirige a `/campanias/{id}?success=true`

### Validacion de Stock de Rewards

El backend debe calcular `CantidadVendida` dinamicamente:

```csharp
// Pseudocodigo
var cantidadVendida = await _context.PedidoCrowdfundingLinea
    .Where(l => l.RewardId == rewardId &&
                l.PedidoCrowdfunding.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
    .SumAsync(l => l.Cantidad);

if (reward.CantidadMaxima.HasValue && cantidadVendida >= reward.CantidadMaxima.Value)
{
    return ServiceResponse con error 4011 (RewardOutOfStock)
}
```

### Atomicidad del Aporte

El handler debe usar una transaccion para asegurar atomicidad:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    // 1. Crear PedidoCrowdfunding
    // 2. Crear PedidoCrowdfundingLinea
    // 3. Actualizar CampaniaCrowdfunding.ImportePledgedActual
    // 4. SaveChanges
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

### Display de Nombre de Backer

Logica para decidir que nombre mostrar:

```csharp
// Backend (en mapper o query)
var nombreBacker = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
    ? await _userService.GetUserNameAsync(pedido.UserId)
    : "Anonimo";
```

```typescript
// Frontend (en componente)
const displayName = backing.esAnonimo || !backing.userName
  ? "Anonimo"
  : backing.userName;
```

### Calculo de Dias Restantes

```csharp
// Backend
var diasRestantes = campania.FechaFin.HasValue
    ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
    : 0;
```

### Paginacion de Backings

- Default: ultimos 20 aportes (ORDER BY FechaCreacion DESC)
- Maximo pageSize: 100 (validar en backend)
- Frontend puede cargar mas con "Load More" button o infinite scroll

### Estado PUBLICADA (EstadoCampaniaId = 2)

Solo las campanias con `EstadoCampaniaId = 2` pueden recibir backings.

Estados posibles:
- 1 = BORRADOR (artista editando)
- 2 = PUBLICADA (aceptando aportes)
- 3 = FINALIZADA (fecha fin pasada)
- 4 = CANCELADA (artista cancelo)
- 5 = PAUSADA (temporalmente deshabilitada)

### Monedas (MVP: Solo EUR)

Para MVP, solo soportar MonedaId = 1 (EUR).
El backend debe validar que `campania.MonedaId == 1` y crear pedidos con `MonedaId = 1`.

Futuro: Expandir a USD, GBP, etc. con conversion de tipos de cambio.

### Estadisticas de Campania

Calculos para `CampaniaStatsDto`:

```csharp
var stats = new CampaniaStatsDto
{
    CampaniaId = campaniaId,
    TotalBackers = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .CountAsync(ct),
    TotalRecaudado = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .SumAsync(p => p.ImporteTotal, ct),
    PromedioAporte = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .AverageAsync(p => p.ImporteTotal, ct),
    AporteMinimo = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .MinAsync(p => p.ImporteTotal, ct),
    AporteMaximo = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaId == campaniaId && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .MaxAsync(p => p.ImporteTotal, ct),
    DiasRestantes = calcular dias restantes...
};
```

### Integracion con Payment Gateway (Post-MVP)

Para MVP, NO procesar pagos reales. El flujo futuro seria:

1. POST `/api/campanias/{id}/backings` → Crea pedido con `EstadoPedidoId = 1` (PENDIENTE)
2. Retorna `paymentUrl` del gateway (Stripe, PayPal, etc.)
3. Frontend redirige a payment gateway
4. Gateway webhook → Backend actualiza pedido a `EstadoPedidoId = 3` (COMPLETADO)
5. Backend actualiza `ImportePledgedActual` solo tras confirmacion de pago

---

**Fin del documento de contratos.**
