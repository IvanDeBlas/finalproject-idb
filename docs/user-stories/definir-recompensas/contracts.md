# Contratos: Definir Recompensas

> **Feature:** definir-recompensas (US-03)
> **Ultima actualizacion:** 2026-02-13

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### POST /api/rewards

**Descripcion:** Crea una nueva recompensa para una campania. Solo el propietario de la campania puede crear recompensas. La recompensa se crea con EsActivo = true.

**Autorizacion:** ✅ Bearer JWT (el ArtistaId del token debe coincidir con el ArtistaId de la campania)

**Request Body:**
```json
{
  "campaniaId": "guid (requerido)",
  "tipoRewardId": "int (requerido, > 0)",
  "nombre": "string (requerido, max 200 caracteres)",
  "descripcion": "string (opcional, max 2000 caracteres)",
  "importeMinimo": "decimal (requerido, > 0)",
  "monedaId": "int (requerido, > 0, default 1 para EUR)",
  "esAddOn": "boolean (default false)",
  "cantidadMaxima": "int (opcional, > 0, null = ilimitado)",
  "cantidadPorBacker": "int (opcional, > 0)",
  "incluyeEnvioFisico": "boolean (default false)",
  "tiempoEntregaEstimado": "string (opcional, max 200 caracteres, ejemplo: '30 dias')",
  "orden": "int (requerido, >= 0)"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "campaniaId": "guid",
    "tipoRewardId": 1,
    "nombre": "Descarga digital del album",
    "descripcion": "Recibe el album completo en formato MP3 y FLAC",
    "importeMinimo": 10.00,
    "monedaId": 1,
    "esAddOn": false,
    "cantidadMaxima": null,
    "cantidadPorBacker": 1,
    "incluyeEnvioFisico": false,
    "tiempoEntregaEstimado": "Inmediato tras finalizar campania",
    "orden": 1,
    "esActivo": true,
    "fechaCreacion": "2026-02-13T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Reward creado correctamente",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El CampaniaId es obligatorio | campaniaId vacio |
| 400 | 1001 | El nombre es obligatorio | nombre vacio |
| 400 | 1002 | El nombre no puede superar los 200 caracteres | nombre > 200 chars |
| 400 | 1002 | La descripcion no puede superar los 2000 caracteres | descripcion > 2000 chars |
| 400 | 1011 | El importe minimo debe ser mayor a 0 | importeMinimo <= 0 |
| 400 | 1001 | La moneda es obligatoria | monedaId <= 0 |
| 400 | 1001 | El tipo de reward es obligatorio | tipoRewardId <= 0 |
| 400 | 1007 | La cantidad maxima debe ser mayor a 0 | cantidadMaxima <= 0 (si se provee) |
| 400 | 1007 | La cantidad por backer debe ser mayor a 0 | cantidadPorBacker <= 0 (si se provee) |
| 400 | 1002 | El tiempo de entrega estimado no puede superar los 200 caracteres | tiempoEntregaEstimado > 200 chars |
| 400 | 1007 | El orden debe ser mayor o igual a 0 | orden < 0 |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para crear recompensas en esta campania | ArtistaId token != ArtistaId campania |
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 500 | 5000 | Error inesperado al crear reward | Excepcion no controlada |

---

### GET /api/rewards

**Descripcion:** Lista recompensas con filtros opcionales. Retorna todas las recompensas activas por defecto.

**Autorizacion:** ❌ Publica (cualquier usuario puede ver recompensas de campanias publicadas)

**Query Parameters:**
- `campaniaId` (guid, opcional): Filtra por campania especifica
- `esActivo` (boolean, opcional): Filtra por estado activo/inactivo
- `esAddOn` (boolean, opcional): Filtra por tipo add-on

**Response 200 OK:**
```json
{
  "data": [
    {
      "id": "guid",
      "campaniaId": "guid",
      "nombre": "Descarga digital del album",
      "descripcion": "Recibe el album completo en formato MP3 y FLAC",
      "importeMinimo": 10.00,
      "esAddOn": false,
      "cantidadMaxima": null,
      "incluyeEnvioFisico": false,
      "orden": 1,
      "esActivo": true
    },
    {
      "id": "guid",
      "campaniaId": "guid",
      "nombre": "CD fisico firmado",
      "descripcion": "CD fisico del album con firma del artista",
      "importeMinimo": 25.00,
      "esAddOn": false,
      "cantidadMaxima": 100,
      "incluyeEnvioFisico": true,
      "orden": 2,
      "esActivo": true
    }
  ],
  "messages": [
    {
      "message": "Rewards encontrados",
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

### GET /api/rewards/{id}

**Descripcion:** Obtiene los detalles completos de una recompensa por su ID.

**Autorizacion:** ❌ Publica (cualquier usuario puede ver recompensas)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "campaniaId": "guid",
    "tipoRewardId": 1,
    "nombre": "Descarga digital del album",
    "descripcion": "Recibe el album completo en formato MP3 y FLAC",
    "importeMinimo": 10.00,
    "monedaId": 1,
    "esAddOn": false,
    "cantidadMaxima": null,
    "cantidadPorBacker": 1,
    "incluyeEnvioFisico": false,
    "tiempoEntregaEstimado": "Inmediato tras finalizar campania",
    "orden": 1,
    "esActivo": true,
    "fechaCreacion": "2026-02-13T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Reward encontrado",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 404 | 2004 | Reward no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### PUT /api/rewards/{id}

**Descripcion:** Actualiza una recompensa existente. Solo el propietario de la campania puede editar. Todos los campos son opcionales excepto id. Solo se actualizan los campos proporcionados.

**Autorizacion:** ✅ Bearer JWT, solo propietario de la campania

**Request Body:**
```json
{
  "id": "guid (requerido, debe coincidir con URL)",
  "nombre": "string (opcional, max 200)",
  "descripcion": "string (opcional, max 2000)",
  "importeMinimo": "decimal (opcional, > 0)",
  "tipoRewardId": "int (opcional, > 0)",
  "monedaId": "int (opcional, > 0)",
  "esAddOn": "boolean (opcional)",
  "cantidadMaxima": "int (opcional, > 0, null para ilimitado)",
  "cantidadPorBacker": "int (opcional, > 0)",
  "incluyeEnvioFisico": "boolean (opcional)",
  "tiempoEntregaEstimado": "string (opcional, max 200)",
  "orden": "int (opcional, >= 0)",
  "esActivo": "boolean (opcional)"
}
```

**Response 200 OK:**
```json
{
  "data": true,
  "messages": [
    {
      "message": "Reward actualizado correctamente",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El Id es obligatorio | id vacio |
| 400 | 1002 | El nombre no puede superar los 200 caracteres | nombre > 200 chars |
| 400 | 1002 | La descripcion no puede superar los 2000 caracteres | descripcion > 2000 chars |
| 400 | 1011 | El importe minimo debe ser mayor a 0 | importeMinimo <= 0 |
| 400 | 1007 | La cantidad maxima debe ser mayor a 0 | cantidadMaxima <= 0 (si se provee) |
| 400 | 1007 | El orden debe ser mayor o igual a 0 | orden < 0 |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para editar esta recompensa | ArtistaId token != ArtistaId campania |
| 404 | 2004 | Reward no encontrado | ID no existe en DB |
| 409 | 4010 | No se puede modificar una recompensa con aportes existentes | Reward tiene backings asociados |
| 500 | 5000 | Error inesperado al actualizar reward | Excepcion no controlada |

---

### DELETE /api/rewards/{id}

**Descripcion:** Elimina (desactiva) una recompensa. Usa soft delete (EsActivo = false) para mantener historial. Si la recompensa tiene backings, retorna error 4010.

**Autorizacion:** ✅ Bearer JWT, solo propietario de la campania

**Response 200 OK:**
```json
{
  "data": true,
  "messages": [
    {
      "message": "Reward eliminado correctamente",
      "errorCode": "0003"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para eliminar esta recompensa | ArtistaId token != ArtistaId campania |
| 404 | 2004 | Reward no encontrado | ID no existe en DB |
| 409 | 4010 | No se puede eliminar una recompensa con aportes existentes | Reward tiene backings asociados |
| 500 | 5000 | Error inesperado al eliminar reward | Excepcion no controlada |

---

### PUT /api/rewards/reorder

**Descripcion:** Reordena multiples recompensas de una campania en una sola operacion. Actualiza el campo `Orden` de cada reward. Solo el propietario de la campania puede reordenar.

**Autorizacion:** ✅ Bearer JWT, solo propietario de la campania

**Request Body:**
```json
{
  "campaniaId": "guid (requerido)",
  "rewardOrders": [
    {
      "rewardId": "guid",
      "orden": 1
    },
    {
      "rewardId": "guid",
      "orden": 2
    },
    {
      "rewardId": "guid",
      "orden": 3
    }
  ]
}
```

**Response 200 OK:**
```json
{
  "data": true,
  "messages": [
    {
      "message": "Recompensas reordenadas correctamente",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El CampaniaId es obligatorio | campaniaId vacio |
| 400 | 1001 | La lista de rewards es obligatoria | rewardOrders vacio |
| 400 | 1007 | Todos los ordenes deben ser mayor o igual a 0 | Algun orden < 0 |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para reordenar recompensas en esta campania | ArtistaId token != ArtistaId campania |
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 404 | 2004 | Uno o mas rewards no encontrados | Algun rewardId no existe |
| 500 | 5000 | Error inesperado al reordenar rewards | Excepcion no controlada |

---

## 🔐 Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/rewards` | ✅ | - | Solo propietario de la campania (ArtistaId match) |
| `GET /api/rewards` | ❌ | - | Publico |
| `GET /api/rewards/{id}` | ❌ | - | Publico |
| `PUT /api/rewards/{id}` | ✅ | - | Solo propietario de la campania |
| `DELETE /api/rewards/{id}` | ✅ | - | Solo propietario, soft delete si tiene backings |
| `PUT /api/rewards/reorder` | ✅ | - | Solo propietario de la campania |

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
| `/dashboard/campanias/{id}/recompensas` | ✅ | `/auth/login` | Lista mis recompensas de la campania |
| `/dashboard/campanias/{id}/recompensas/nueva` | ✅ | `/auth/login` | Crear nueva recompensa |
| `/dashboard/campanias/{id}/recompensas/{rewardId}/editar` | ✅ | `/auth/login` | Editar recompensa existente |

| Ruta (Landing) | Auth | Redirect | Notas |
|----------------|------|----------|-------|
| `/campanias/{id}` | ❌ | - | Publico, muestra recompensas en la vista de campania |

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardDto.cs
public class RewardDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public int TipoRewardId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int MonedaId { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; } // null = ilimitado
    public int? CantidadPorBacker { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardListDto.cs
public class RewardListDto
{
    public Guid Id { get; set; }
    public Guid CampaniaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/CreateRewardCommand.cs
public class CreateRewardCommand : IRequest<ServiceResponse<RewardDto>>
{
    public Guid CampaniaId { get; set; }
    public int TipoRewardId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal ImporteMinimo { get; set; }
    public int MonedaId { get; set; }
    public bool EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public int? CantidadPorBacker { get; set; }
    public bool IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int Orden { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/UpdateRewardCommand.cs
public class UpdateRewardCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoRewardId { get; set; }
    public int? MonedaId { get; set; }
    public bool? EsAddOn { get; set; }
    public int? CantidadMaxima { get; set; }
    public int? CantidadPorBacker { get; set; }
    public bool? IncluyeEnvioFisico { get; set; }
    public string? TiempoEntregaEstimado { get; set; }
    public int? Orden { get; set; }
    public bool? EsActivo { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Rewards/Commands/ReorderRewardsCommand.cs
// NUEVO - A implementar
public class ReorderRewardsCommand : IRequest<ServiceResponse<bool>>
{
    public Guid CampaniaId { get; set; }
    public List<RewardOrderDto> RewardOrders { get; set; } = new();
}

public class RewardOrderDto
{
    public Guid RewardId { get; set; }
    public int Orden { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/reward.ts
export interface Reward {
  id: string;
  campaniaId: string;
  tipoRewardId: number;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  monedaId: number;
  esAddOn: boolean;
  cantidadMaxima?: number; // null = ilimitado
  cantidadPorBacker?: number;
  incluyeEnvioFisico: boolean;
  tiempoEntregaEstimado?: string;
  orden: number;
  esActivo: boolean;
  fechaCreacion: string; // ISO 8601
  fechaActualizacion?: string; // ISO 8601
}

export interface RewardListItem {
  id: string;
  campaniaId: string;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  esAddOn: boolean;
  cantidadMaxima?: number;
  incluyeEnvioFisico: boolean;
  orden: number;
  esActivo: boolean;
}

export interface CreateRewardRequest {
  campaniaId: string;
  tipoRewardId: number;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  monedaId: number;
  esAddOn: boolean;
  cantidadMaxima?: number;
  cantidadPorBacker?: number;
  incluyeEnvioFisico: boolean;
  tiempoEntregaEstimado?: string;
  orden: number;
}

export interface UpdateRewardRequest {
  id: string;
  nombre?: string;
  descripcion?: string;
  importeMinimo?: number;
  tipoRewardId?: number;
  monedaId?: number;
  esAddOn?: boolean;
  cantidadMaxima?: number;
  cantidadPorBacker?: number;
  incluyeEnvioFisico?: boolean;
  tiempoEntregaEstimado?: string;
  orden?: number;
  esActivo?: boolean;
}

export interface ReorderRewardsRequest {
  campaniaId: string;
  rewardOrders: RewardOrder[];
}

export interface RewardOrder {
  rewardId: string;
  orden: number;
}

// Enums para tipos
export enum TipoReward {
  Digital = 1,
  Fisico = 2,
  Experiencia = 3,
  Otro = 4,
}

export const TIPO_REWARD_LABELS: Record<number, string> = {
  1: 'Digital',
  2: 'Fisico',
  3: 'Experiencia',
  4: 'Otro',
};
```

---

## ✅ Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| campaniaId | requerido | `.NotEmpty().WithErrorCode("1001")` | `.string().uuid('ID invalido')` |
| nombre | requerido | `.NotEmpty().WithErrorCode("1001")` | `.string().min(1, 'El nombre es obligatorio')` |
| nombre | max 200 | `.MaximumLength(200).WithErrorCode("1002")` | `.max(200, 'Maximo 200 caracteres')` |
| descripcion | max 2000 | `.MaximumLength(2000).WithErrorCode("1002")` | `.max(2000, 'Maximo 2000 caracteres').optional()` |
| importeMinimo | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1011")` | `.number().positive('Debe ser mayor a 0')` |
| monedaId | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1001")` | `.number().int().positive('Moneda requerida')` |
| tipoRewardId | requerido, > 0 | `.GreaterThan(0).WithErrorCode("1001")` | `.number().int().positive('Tipo requerido')` |
| cantidadMaxima | > 0 (si provee) | `.GreaterThan(0).WithErrorCode("1007")` | `.number().int().positive().optional()` |
| cantidadPorBacker | > 0 (si provee) | `.GreaterThan(0).WithErrorCode("1007")` | `.number().int().positive().optional()` |
| tiempoEntregaEstimado | max 200 | `.MaximumLength(200).WithErrorCode("1002")` | `.max(200, 'Maximo 200 caracteres').optional()` |
| orden | >= 0 | `.GreaterThanOrEqualTo(0).WithErrorCode("1007")` | `.number().int().min(0, 'Orden debe ser >= 0')` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/reward.schema.ts
import { z } from 'zod';

export const createRewardSchema = z.object({
  campaniaId: z
    .string()
    .uuid('ID de campania invalido'),
  tipoRewardId: z
    .number()
    .int()
    .positive('Debe seleccionar un tipo de recompensa'),
  nombre: z
    .string()
    .min(1, 'El nombre es obligatorio')
    .max(200, 'El nombre no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(2000, 'La descripcion no puede superar los 2000 caracteres')
    .optional()
    .or(z.literal('')),
  importeMinimo: z
    .number({ invalid_type_error: 'Debe ser un numero' })
    .positive('El importe debe ser mayor a 0')
    .min(1, 'El importe minimo es 1 EUR'),
  monedaId: z
    .number()
    .int()
    .positive('La moneda es obligatoria')
    .default(1), // EUR
  esAddOn: z.boolean().default(false),
  cantidadMaxima: z
    .number()
    .int()
    .positive('La cantidad maxima debe ser mayor a 0')
    .optional()
    .nullable(),
  cantidadPorBacker: z
    .number()
    .int()
    .positive('La cantidad por backer debe ser mayor a 0')
    .optional()
    .nullable(),
  incluyeEnvioFisico: z.boolean().default(false),
  tiempoEntregaEstimado: z
    .string()
    .max(200, 'El tiempo de entrega no puede superar los 200 caracteres')
    .optional()
    .or(z.literal('')),
  orden: z
    .number()
    .int()
    .min(0, 'El orden debe ser mayor o igual a 0')
    .default(0),
});

export type CreateRewardFormData = z.infer<typeof createRewardSchema>;

export const updateRewardSchema = z.object({
  id: z.string().uuid('ID invalido'),
  nombre: z
    .string()
    .min(1, 'El nombre no puede estar vacio')
    .max(200, 'El nombre no puede superar los 200 caracteres')
    .optional(),
  descripcion: z
    .string()
    .max(2000, 'La descripcion no puede superar los 2000 caracteres')
    .optional(),
  importeMinimo: z
    .number()
    .positive('El importe debe ser mayor a 0')
    .optional(),
  tipoRewardId: z
    .number()
    .int()
    .positive()
    .optional(),
  monedaId: z
    .number()
    .int()
    .positive()
    .optional(),
  esAddOn: z.boolean().optional(),
  cantidadMaxima: z
    .number()
    .int()
    .positive()
    .optional()
    .nullable(),
  cantidadPorBacker: z
    .number()
    .int()
    .positive()
    .optional()
    .nullable(),
  incluyeEnvioFisico: z.boolean().optional(),
  tiempoEntregaEstimado: z
    .string()
    .max(200)
    .optional(),
  orden: z
    .number()
    .int()
    .min(0)
    .optional(),
  esActivo: z.boolean().optional(),
});

export type UpdateRewardFormData = z.infer<typeof updateRewardSchema>;

export const reorderRewardsSchema = z.object({
  campaniaId: z.string().uuid('ID de campania invalido'),
  rewardOrders: z
    .array(
      z.object({
        rewardId: z.string().uuid('ID de reward invalido'),
        orden: z.number().int().min(0, 'El orden debe ser mayor o igual a 0'),
      })
    )
    .min(1, 'Debe proporcionar al menos una recompensa'),
});

export type ReorderRewardsFormData = z.infer<typeof reorderRewardsSchema>;
```

---

## 🔗 Constantes Compartidas

```typescript
// Ruta: src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  rewards: {
    all: ['rewards'] as const,
    byId: (id: string) => ['rewards', id] as const,
    byCampania: (campaniaId: string) => ['rewards', 'campania', campaniaId] as const,
    filtered: (filters: Record<string, any>) => ['rewards', 'filtered', filters] as const,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/api-routes.ts
export const API_ROUTES = {
  rewards: {
    base: '/api/rewards',
    byId: (id: string) => `/api/rewards/${id}`,
    reorder: '/api/rewards/reorder',
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/app-routes.ts
export const APP_ROUTES = {
  dashboard: {
    rewards: {
      list: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas`,
      nueva: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas/nueva`,
      editar: (campaniaId: string, rewardId: string) =>
        `/dashboard/campanias/${campaniaId}/recompensas/${rewardId}/editar`,
    },
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/reward-types.ts
export const TIPO_REWARD = {
  DIGITAL: 1,
  FISICO: 2,
  EXPERIENCIA: 3,
  OTRO: 4,
} as const;

export const TIPO_REWARD_LABELS: Record<number, string> = {
  1: 'Digital',
  2: 'Fisico',
  3: 'Experiencia',
  4: 'Otro',
};

export const TIPO_REWARD_DESCRIPTIONS: Record<number, string> = {
  1: 'Descarga digital, streaming, acceso online',
  2: 'CD, vinilo, merchandising, productos fisicos',
  3: 'Conciertos privados, meet & greet, workshops',
  4: 'Otras recompensas personalizadas',
};
```

---

## 📊 Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts
export const REWARD_ERROR_MESSAGES: Record<string, string> = {
  // Success codes
  '0000': 'Operacion exitosa',
  '0001': 'Recompensa creada exitosamente',
  '0002': 'Recompensa actualizada exitosamente',
  '0003': 'Recompensa eliminada exitosamente',

  // Validation errors (1000-1999)
  '1001': 'Este campo es obligatorio',
  '1002': 'El valor supera el maximo de caracteres permitido',
  '1007': 'El valor esta fuera del rango permitido',
  '1011': 'El monto debe ser mayor a 0',

  // Not Found errors (2000-2999)
  '2003': 'No encontramos la campania asociada',
  '2004': 'No encontramos esta recompensa',

  // Auth errors (3000-3999)
  '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
  '3002': 'No tienes permiso para modificar esta recompensa',

  // Business Rule errors (4000-4999)
  '4010': 'No puedes eliminar una recompensa que ya tiene aportes',

  // Internal errors (5000-5999)
  '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
} as const;

export const getRewardErrorMessage = (errorCode: string): string => {
  return REWARD_ERROR_MESSAGES[errorCode] || REWARD_ERROR_MESSAGES['5000'];
};

// Helper para mensajes especificos de recompensas
export const getRewardSpecificErrorMessage = (errorCode: string): string => {
  const customMessages: Record<string, string> = {
    '1001': 'Completa todos los campos obligatorios para crear la recompensa',
    '1011': 'El importe debe ser al menos 1 EUR',
    '2004': 'Esta recompensa no existe o ha sido eliminada',
    '3002': 'Solo el creador de la campania puede modificar sus recompensas',
    '4010': 'Esta recompensa ya tiene aportes y no puede ser eliminada. Puedes desactivarla en su lugar.',
  };

  return customMessages[errorCode] || getRewardErrorMessage(errorCode);
};
```

---

## 🔔 Eventos SignalR

**No aplica para MVP.** La gestion de recompensas es un flujo sincrono sin necesidad de notificaciones en tiempo real.

**Futuro (post-MVP):** Si se implementa notificacion de stock limitado en tiempo real (cuando una recompensa esta por agotarse), considerar eventos SignalR para sincronizar disponibilidad entre multiples usuarios.

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (6 endpoints documentados: POST, GET, GET/:id, PUT, DELETE, PUT/reorder)
- [x] Autorizacion por endpoint (tabla resumen completa)
- [x] Claims JWT documentados (sub, email, exp, iat)
- [x] DTOs C# completos (RewardDto, RewardListDto, CreateRewardCommand, UpdateRewardCommand, ReorderRewardsCommand)
- [x] Types TypeScript equivalentes (Reward, RewardListItem, CreateRewardRequest, UpdateRewardRequest, ReorderRewardsRequest, enums)
- [x] Schemas Zod con mismas reglas (createReward, updateReward, reorderRewards)
- [x] Constantes compartidas (query keys, API routes, app routes, tipos de reward)
- [x] Mapeo de errores a mensajes UI (todos los codigos relevantes + helpers especificos)
- [x] SignalR evaluado (no aplica para MVP)

---

## Notas Tecnicas Adicionales

### Flujo de Creacion de Recompensas

1. Artista navega a `/dashboard/campanias/{id}/recompensas`
2. Click en "Agregar Recompensa" → Formulario modal o pagina nueva
3. Completa formulario:
   - **Basico**: Nombre, Descripcion, Tipo
   - **Financiero**: Importe minimo, Moneda
   - **Limitaciones**: Cantidad maxima (opcional), Cantidad por backer (opcional)
   - **Logistica**: Envio fisico (checkbox), Tiempo entrega estimado
4. Frontend valida con Zod schema
5. POST /api/rewards → Backend crea con Orden = max(orden_existente) + 1 automaticamente
6. Success → Redirect a lista de recompensas con toast "Recompensa creada"

### Reordenamiento Drag & Drop

1. Artista arrastra y suelta recompensas en la lista
2. Frontend recolecta nuevo orden: `[{rewardId: 'xxx', orden: 1}, {rewardId: 'yyy', orden: 2}]`
3. PUT /api/rewards/reorder con array completo
4. Backend actualiza todos en una transaccion
5. Frontend invalida query cache para re-fetch con nuevo orden

### Validacion de Propiedad

Para todos los endpoints autenticados (POST, PUT, DELETE, PUT/reorder):

```csharp
// Pseudocodigo del flujo
1. Extraer ArtistaId del token JWT (claim "sub")
2. Obtener Campania del CampaniaId
3. Validar: campania.ArtistaId == artistaIdFromToken
4. Si no match → Return 403 Forbidden con error code 3002
```

### Constante de Error BusinessRule_RewardHasBackings

**IMPORTANTE**: Agregar nueva constante en `ServiceResponseMessageType.cs`:

```csharp
// En rango 4000-4999 (Business Rule Errors)
public const string BusinessRule_RewardHasBackings = "4010";
```

Esta constante debe ser usada en:
- `UpdateRewardCommand` (si se intenta desactivar reward con backings)
- `DeleteRewardCommand` (siempre validar backings antes de soft delete)

### Soft Delete con Backings

Si una recompensa tiene backings asociados:
- NO permitir hard delete
- NO permitir cambiar `ImporteMinimo`, `Nombre`, o `Descripcion` (solo permitir cambiar `EsActivo`)
- Permitir soft delete (EsActivo = false) para ocultarla de nuevos backers
- Los backings existentes mantienen referencia al reward inactivo

### Orden de Recompensas

- Orden por defecto: ascendente (1, 2, 3...)
- Frontend muestra recompensas ordenadas de menor a mayor importe (convencion crowdfunding)
- Artista puede reordenar manualmente con drag & drop
- Backend NO valida unicidad de orden (puede haber duplicados, frontend resuelve conflictos)

### Tipos de Recompensa

Usar catalogo `TipoReward`:
1. **Digital** - Descarga, streaming, acceso online
2. **Fisico** - CD, vinilo, merch, productos fisicos
3. **Experiencia** - Conciertos privados, meet & greet, workshops
4. **Otro** - Recompensas personalizadas

El campo `IncluyeEnvioFisico` determina si se requiere direccion de envio del backer.

### Monedas Soportadas

Por MVP, solo EUR (MonedaId = 1). El campo `MonedaId` debe coincidir con la moneda de la campania.

Validacion futura: Backend debe validar que `reward.MonedaId == campania.MonedaId`.

---

**Fin del documento de contratos.**
