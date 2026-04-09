# Plan de Contratos Shared: Definir Recompensas

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Basado en:** docs/user-stories/definir-recompensas/contracts.md

## 1. Resumen

- Total de types: 6 (Reward, RewardListItem, CreateRewardRequest, UpdateRewardRequest, ReorderRewardsRequest, RewardOrder)
- Total de schemas: 3 (createRewardSchema, updateRewardSchema, reorderRewardsSchema)
- Constantes actualizadas: QUERY_KEYS.rewards, API_ROUTES.rewards, APP_ROUTES.dashboard.rewards, TIPO_REWARD, TIPO_REWARD_LABELS, TIPO_REWARD_DESCRIPTIONS
- Utilidades planificadas: REWARD_ERROR_MESSAGES, getRewardErrorMessage, getRewardSpecificErrorMessage

## 2. Types (`src/shared/types/reward.ts`)

### 2.1 Estado Actual

El archivo **YA EXISTE** pero tiene una estructura desactualizada que NO coincide con contracts.md.

**Problemas del archivo actual:**
- Usa `stockLimitado` y `stockDisponible` (nomenclatura incorrecta)
- Falta `tipoRewardId`, `monedaId`, `esAddOn`, `cantidadMaxima`, `cantidadPorBacker`, `incluyeEnvioFisico`, `tiempoEntregaEstimado`, `orden`, `esActivo`
- Usa `fechaEntregaEstimada` en lugar de `tiempoEntregaEstimado` (string libre)

**Accion:** REEMPLAZAR completamente el archivo.

### 2.2 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `Reward` | id, campaniaId, tipoRewardId, nombre, descripcion?, importeMinimo, monedaId, esAddOn, cantidadMaxima?, cantidadPorBacker?, incluyeEnvioFisico, tiempoEntregaEstimado?, orden, esActivo, fechaCreacion, fechaActualizacion? | Entidad completa de recompensa (equivale a RewardDto de backend) |
| `RewardListItem` | id, campaniaId, nombre, descripcion?, importeMinimo, esAddOn, cantidadMaxima?, incluyeEnvioFisico, orden, esActivo | Version simplificada para listas (equivale a RewardListDto de backend) |

### 2.3 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreateRewardRequest` | campaniaId, tipoRewardId, nombre, descripcion?, importeMinimo, monedaId, esAddOn, cantidadMaxima?, cantidadPorBacker?, incluyeEnvioFisico, tiempoEntregaEstimado?, orden | Request para crear recompensa (equivale a CreateRewardCommand) |
| `UpdateRewardRequest` | id, nombre?, descripcion?, importeMinimo?, tipoRewardId?, monedaId?, esAddOn?, cantidadMaxima?, cantidadPorBacker?, incluyeEnvioFisico?, tiempoEntregaEstimado?, orden?, esActivo? | Request para actualizar recompensa (equivale a UpdateRewardCommand) |
| `ReorderRewardsRequest` | campaniaId, rewardOrders | Request para reordenar multiples recompensas (equivale a ReorderRewardsCommand) |
| `RewardOrder` | rewardId, orden | Item individual de reordenamiento (equivale a RewardOrderDto) |

### 2.4 Enums y Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `TipoReward` | Digital = 1, Fisico = 2, Experiencia = 3, Otro = 4 | Enum para tipos de recompensa (catalogo backend) |

### 2.5 Contenido Completo del Archivo

**Ruta:** `src/shared/types/reward.ts`

```typescript
/**
 * Types para Recompensas (Rewards)
 * Alineado con WePlayRises.Crowdfunding.Application.Dtos.RewardDto
 */

// ========== Response DTOs ==========

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
  tiempoEntregaEstimado?: string; // Texto libre, ej: "30 dias", "Inmediato"
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

// ========== Request DTOs ==========

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

// ========== Enums ==========

export enum TipoReward {
  Digital = 1,
  Fisico = 2,
  Experiencia = 3,
  Otro = 4,
}
```

---

## 3. Schemas Zod (`src/shared/schemas/reward.schema.ts`)

### 3.1 Estado Actual

El archivo **YA EXISTE** pero tiene un schema muy simplificado (`rewardSchema`) que NO coincide con los requisitos de contracts.md.

**Problemas del archivo actual:**
- Solo tiene un schema generico `rewardSchema`
- Falta `createRewardSchema`, `updateRewardSchema`, `reorderRewardsSchema`
- Validaciones desactualizadas (nombre max 100, descripcion max 1000, descripcion requerida cuando deberia ser opcional)

**Accion:** REEMPLAZAR completamente el archivo.

### 3.2 Schemas de Validacion

| Schema | Campos | Reglas Principales |
|--------|--------|-------------------|
| `createRewardSchema` | campaniaId, tipoRewardId, nombre, descripcion, importeMinimo, monedaId, esAddOn, cantidadMaxima, cantidadPorBacker, incluyeEnvioFisico, tiempoEntregaEstimado, orden | campaniaId: uuid; nombre: min 1, max 200; descripcion: optional, max 2000; importeMinimo: positive, min 1; tipoRewardId: int positive; monedaId: int positive (default 1); cantidadMaxima: int positive optional; orden: int min 0 (default 0) |
| `updateRewardSchema` | id, nombre, descripcion, importeMinimo, tipoRewardId, monedaId, esAddOn, cantidadMaxima, cantidadPorBacker, incluyeEnvioFisico, tiempoEntregaEstimado, orden, esActivo | Todos los campos opcionales excepto id (uuid); mismas reglas que create pero opcionales |
| `reorderRewardsSchema` | campaniaId, rewardOrders | campaniaId: uuid; rewardOrders: array min 1 de objetos {rewardId: uuid, orden: int min 0} |

### 3.3 Types Inferidos

- `CreateRewardFormData` = `z.infer<typeof createRewardSchema>`
- `UpdateRewardFormData` = `z.infer<typeof updateRewardSchema>`
- `ReorderRewardsFormData` = `z.infer<typeof reorderRewardsSchema>`

### 3.4 Contenido Completo del Archivo

**Ruta:** `src/shared/schemas/reward.schema.ts`

```typescript
import { z } from 'zod';

/**
 * Schema Zod para crear una recompensa
 * Alineado con CreateRewardCommandValidator del backend
 */
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
    .default(1), // EUR por defecto
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

/**
 * Schema Zod para actualizar una recompensa
 * Alineado con UpdateRewardCommandValidator del backend
 * Todos los campos son opcionales excepto id
 */
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

/**
 * Schema Zod para reordenar recompensas
 * Alineado con ReorderRewardsCommandValidator del backend
 */
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

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Estado Actual

El archivo **YA EXISTE** y contiene las siguientes constantes de rewards:

```typescript
rewards: {
  all: ["rewards"] as const,
  byId: (id: string) => ["rewards", id] as const,
  byCampania: (campaniaId: string) => ["rewards", "campania", campaniaId] as const,
},
```

```typescript
rewards: {
  base: "/api/rewards",
  byId: (id: string) => `/api/rewards/${id}`,
},
```

**Accion:** ACTUALIZAR el archivo existente agregando:
1. Nueva ruta `reorder` en `API_ROUTES.rewards`
2. Nuevas rutas en `APP_ROUTES.dashboard.rewards` (list, nueva, editar)
3. Nueva seccion `TIPO_REWARD`, `TIPO_REWARD_LABELS`, `TIPO_REWARD_DESCRIPTIONS`
4. Nueva entrada en `QUERY_KEYS.rewards` para filtros (`filtered`)

### 4.2 Endpoints API

**Agregar en `API_ROUTES.rewards`:**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `reorder` | `/api/rewards/reorder` | Endpoint para reordenar recompensas |

### 4.3 Query Keys (React Query)

**Agregar en `QUERY_KEYS.rewards`:**

| Key | Patron | Uso |
|-----|--------|-----|
| `filtered` | `['rewards', 'filtered', filters]` | Query con filtros (esActivo, esAddOn, campaniaId) |

### 4.4 App Routes (Frontend)

**Agregar en `APP_ROUTES.dashboard`:**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `rewards.list` | `(campaniaId) => /dashboard/campanias/${campaniaId}/recompensas` | Lista de recompensas de una campania |
| `rewards.nueva` | `(campaniaId) => /dashboard/campanias/${campaniaId}/recompensas/nueva` | Crear nueva recompensa |
| `rewards.editar` | `(campaniaId, rewardId) => /dashboard/campanias/${campaniaId}/recompensas/${rewardId}/editar` | Editar recompensa existente |

### 4.5 Tipos de Recompensa

**Agregar nuevas constantes globales:**

```typescript
// Tipos de recompensa (aligned with backend TipoReward catalog)
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

### 4.6 Cambios Exactos a Realizar

**Modificar:** `src/shared/constants/index.ts`

**Lineas a actualizar:**

1. **Actualizar `QUERY_KEYS.rewards` (linea 32-36):**

```typescript
// Rewards
rewards: {
    all: ["rewards"] as const,
    byId: (id: string) => ["rewards", id] as const,
    byCampania: (campaniaId: string) => ["rewards", "campania", campaniaId] as const,
    filtered: (filters: Record<string, unknown>) => ["rewards", "filtered", filters] as const, // NUEVO
},
```

2. **Actualizar `API_ROUTES.rewards` (linea 78-81):**

```typescript
rewards: {
    base: "/api/rewards",
    byId: (id: string) => `/api/rewards/${id}`,
    reorder: "/api/rewards/reorder", // NUEVO
},
```

3. **Actualizar `APP_ROUTES.dashboard` (linea 98-105):**

```typescript
dashboard: {
    root: "/dashboard",
    campanias: {
        list: "/dashboard/campanias",
        nueva: "/dashboard/campanias/nueva",
        editar: (id: string) => `/dashboard/campanias/${id}/editar`,
        detalle: (id: string) => `/dashboard/campanias/${id}`,
    },
    // NUEVO
    rewards: {
        list: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas`,
        nueva: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas/nueva`,
        editar: (campaniaId: string, rewardId: string) =>
            `/dashboard/campanias/${campaniaId}/recompensas/${rewardId}/editar`,
    },
},
```

4. **Agregar despues de `TIPO_FINANCIACION_DESCRIPTIONS` (linea ~150):**

```typescript
// Tipos de recompensa (aligned with backend TipoReward catalog)
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

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

### 5.1 Estado Actual

El archivo **YA EXISTE** con:
- `ERROR_CODE_MESSAGES` (mapeo de codigos numericos)
- `getErrorMessage()` (helper generico)
- `getCampaniaErrorMessage()` (helper especifico para campanias)
- `formatErrorMessages()`, `isSuccessCode()`, `requiresReAuth()`

**Accion:** ACTUALIZAR el archivo agregando:
1. Nuevos codigos de error especificos de rewards en `ERROR_CODE_MESSAGES` (si no existen)
2. Nuevo helper `getRewardErrorMessage()` para mensajes contextuales de rewards
3. Nuevo helper `getRewardSpecificErrorMessage()` para casos edge de rewards

### 5.2 Error Messages

**Codigos nuevos a agregar en `ERROR_CODE_MESSAGES` (si no existen):**

| Codigo | Mensaje | Contexto |
|--------|---------|----------|
| `4010` | 'No se puede eliminar una recompensa con aportes existentes' | BusinessRule_RewardHasBackings |

**Nota:** Los demas codigos (1001, 1002, 1007, 1011, 2003, 2004, 3001, 3002, 5000) YA EXISTEN en el archivo actual.

### 5.3 Helpers Especificos

**Agregar 2 nuevos helpers:**

1. **`getRewardErrorMessage(errorCode: string): string`**
   - Similar a `getCampaniaErrorMessage` pero para rewards
   - Retorna mensaje generico de `getErrorMessage()` por defecto

2. **`getRewardSpecificErrorMessage(errorCode: string): string`**
   - Mensajes altamente especificos para UX de rewards
   - Incluye contexto adicional para casos edge (eliminar con backings, permisos, etc.)

### 5.4 Cambios Exactos a Realizar

**Modificar:** `src/shared/utils/error-messages.ts`

**Agregar despues de `getCampaniaErrorMessage()` (linea ~114):**

```typescript
/**
 * Mensajes de error especificos para Recompensas
 */
export const getRewardErrorMessage = (errorCode: string): string => {
    const customMessages: Record<string, string> = {
        "1001": "Completa todos los campos obligatorios para crear la recompensa",
        "1011": "El importe debe ser al menos 1 EUR",
        "2004": "Esta recompensa no existe o ha sido eliminada",
        "3002": "Solo el creador de la campania puede modificar sus recompensas",
        "4010": "Esta recompensa ya tiene aportes y no puede ser eliminada. Puedes desactivarla en su lugar.",
    };

    return customMessages[errorCode] || getErrorMessage(errorCode);
};

/**
 * Mensajes de error especificos para recompensas con contexto adicional
 * Util para tooltips, modals de confirmacion, etc.
 */
export const getRewardSpecificErrorMessage = (errorCode: string): string => {
    const specificMessages: Record<string, string> = {
        "1001": "Los campos Nombre, Importe Minimo y Tipo de Recompensa son obligatorios. Completa todos los datos antes de continuar.",
        "1002": "Has superado el limite de caracteres permitido. Revisa los campos marcados.",
        "1007": "Los valores numericos deben ser positivos. Corrige los campos marcados.",
        "1011": "El importe minimo debe ser mayor a 0 EUR. Ingresa un monto valido para la recompensa.",
        "2003": "La campania asociada no existe. Verifica que la campania este activa.",
        "2004": "No pudimos encontrar esta recompensa. Puede haber sido eliminada por el creador de la campania.",
        "3001": "Tu sesion ha expirado. Inicia sesion nuevamente para continuar gestionando recompensas.",
        "3002": "Solo el artista creador de la campania puede crear, editar o eliminar sus recompensas.",
        "4010": "Esta recompensa ya tiene aportes de fans y no puede ser eliminada para preservar los compromisos. Puedes desactivarla para ocultarla de nuevos backers, pero los aportes existentes se mantendran.",
    };

    return specificMessages[errorCode] || getRewardErrorMessage(errorCode);
};
```

**Agregar en `ERROR_CODE_MESSAGES` (si el codigo 4010 NO existe, linea ~51):**

```typescript
// Business Rule errors (4000-4999)
"4001": "Este registro ya existe",
"4002": "El estado actual no permite esta operacion",
"4003": "Esta operacion no esta permitida",
"4004": "Se ha excedido el limite permitido",
"4005": "Fondos insuficientes",
"4006": "La campania no esta activa",
"4007": "La campania ha finalizado",
"4009": "Solo se pueden editar campanias en estado borrador",
"4010": "No se puede eliminar una recompensa con aportes existentes", // NUEVO
```

---

## 6. Archivos a Crear/Modificar

```
src/shared/
├── types/
│   └── reward.ts                    [MODIFICAR] - Reemplazar completamente
├── schemas/
│   └── reward.schema.ts              [MODIFICAR] - Reemplazar completamente
├── constants/
│   └── index.ts                      [MODIFICAR] - Agregar rewards routes, TIPO_REWARD
└── utils/
    └── error-messages.ts             [MODIFICAR] - Agregar helpers de rewards
```

### Detalle de Archivos:

| Archivo | Accion | Razon |
|---------|--------|-------|
| `src/shared/types/reward.ts` | **REEMPLAZAR** | Estructura actual desactualizada, cambios extensos requieren reescritura |
| `src/shared/schemas/reward.schema.ts` | **REEMPLAZAR** | Solo tiene un schema generico, falta create/update/reorder |
| `src/shared/constants/index.ts` | **ACTUALIZAR** | Agregar rutas y constantes nuevas a archivo existente |
| `src/shared/utils/error-messages.ts` | **ACTUALIZAR** | Agregar helpers especificos para rewards |

---

## 7. Dependencias

- **zod** - Ya instalado en `src/shared/package.json`
- **No se requieren packages adicionales**

---

## 8. Notas de Implementacion

### 8.1 Alineacion con Backend

Los types y schemas estan 100% alineados con:
- `WePlayRises.Crowdfunding.Application.Dtos.RewardDto`
- `WePlayRises.Crowdfunding.Application.Features.Rewards.Commands.CreateRewardCommand`
- `WePlayRises.Crowdfunding.Application.Features.Rewards.Commands.UpdateRewardCommand`
- `WePlayRises.Crowdfunding.Application.Features.Rewards.Commands.ReorderRewardsCommand`

### 8.2 Validaciones Zod

Las validaciones Zod replican exactamente las reglas de FluentValidation del backend:
- `campaniaId`: uuid, requerido → ErrorCode 1001
- `nombre`: min 1, max 200 → ErrorCode 1001, 1002
- `descripcion`: max 2000, opcional → ErrorCode 1002
- `importeMinimo`: positive, min 1 → ErrorCode 1011
- `monedaId`: int positive, default 1 → ErrorCode 1001
- `tipoRewardId`: int positive → ErrorCode 1001
- `cantidadMaxima`: int positive, nullable → ErrorCode 1007
- `cantidadPorBacker`: int positive, nullable → ErrorCode 1007
- `tiempoEntregaEstimado`: max 200, opcional → ErrorCode 1002
- `orden`: int min 0, default 0 → ErrorCode 1007

### 8.3 Tipos Opcionales vs Nullable

**Diferencia critica:**
- `descripcion?: string` - Campo opcional (puede no enviarse)
- `cantidadMaxima?: number | null` - Campo opcional que puede ser null explicitamente

**Zod handling:**
- `.optional()` - No required en formulario
- `.nullable()` - Acepta null como valor valido
- `.optional().nullable()` - Ambos

### 8.4 Orden de Recompensas

El campo `orden` se usa para ordenar visualmente las recompensas en la UI.

**Backend:**
- Auto-incrementa `orden = max(orden) + 1` al crear
- Permite reordenar via `PUT /api/rewards/reorder`

**Frontend:**
- Drag & drop para reordenar
- Actualizar array completo de ordenes en una sola request

### 8.5 Soft Delete

El campo `esActivo: boolean` se usa para soft delete:
- `DELETE /api/rewards/{id}` → Backend pone `esActivo = false`
- Frontend filtra con `?esActivo=true` para mostrar solo activos
- Si reward tiene backings, backend retorna error 4010 y NO permite eliminar

### 8.6 Tipos de Recompensa

El catalogo `TipoReward` define 4 tipos:
1. **Digital** (1) - Descargas, streaming, acceso online
2. **Fisico** (2) - CD, vinilo, merchandising
3. **Experiencia** (3) - Conciertos privados, meet & greet
4. **Otro** (4) - Recompensas personalizadas

El campo `incluyeEnvioFisico` determina si se requiere direccion de envio del backer (independiente de TipoReward).

### 8.7 Monedas

Por MVP, solo EUR (MonedaId = 1).

**Futuro:** Validar que `reward.MonedaId == campania.MonedaId` en backend.

### 8.8 Stock Limitado

**Nomenclatura:**
- Backend: `CantidadMaxima` (int nullable, null = ilimitado)
- Frontend: `cantidadMaxima` (number optional nullable)

**Logica:**
- Si `cantidadMaxima` es null → stock ilimitado
- Si `cantidadMaxima` tiene valor → stock limitado

**Decrementar stock:** Se implementara en feature `realizar-backing` (US-04).

### 8.9 Error Handling

Los helpers `getRewardErrorMessage()` y `getRewardSpecificErrorMessage()` retornan mensajes en espanol adaptados al contexto de recompensas.

**Uso recomendado:**
- `getRewardErrorMessage()` - Toasts y mensajes inline breves
- `getRewardSpecificErrorMessage()` - Modals de confirmacion y tooltips con contexto extendido

### 8.10 Endpoints - Estructura de Rutas

**Backend endpoints:**
- `POST /api/rewards`
- `GET /api/rewards?campaniaId=X`
- `GET /api/rewards/{id}`
- `PUT /api/rewards/{id}`
- `DELETE /api/rewards/{id}`
- `PUT /api/rewards/reorder`

**Frontend routes (Admin Dashboard):**
- `/dashboard/campanias/{campaniaId}/recompensas` - Lista
- `/dashboard/campanias/{campaniaId}/recompensas/nueva` - Crear
- `/dashboard/campanias/{campaniaId}/recompensas/{rewardId}/editar` - Editar

**Decision:** Backend usa `/api/rewards` como recurso independiente, Frontend usa ruta jerarquica para UX (rewards siempre bajo campania en dashboard).

---

## 9. Checklist

- [ ] Types creados y exportados (`Reward`, `RewardListItem`, `CreateRewardRequest`, `UpdateRewardRequest`, `ReorderRewardsRequest`, `RewardOrder`, `TipoReward`)
- [ ] Schemas Zod con mensajes en espanol (`createRewardSchema`, `updateRewardSchema`, `reorderRewardsSchema`)
- [ ] Constantes de endpoints alineadas con backend (`API_ROUTES.rewards.reorder`)
- [ ] Query keys consistentes con patron del proyecto (`QUERY_KEYS.rewards.filtered`)
- [ ] App routes para dashboard (`APP_ROUTES.dashboard.rewards`)
- [ ] Constantes de tipos de recompensa (`TIPO_REWARD`, `TIPO_REWARD_LABELS`, `TIPO_REWARD_DESCRIPTIONS`)
- [ ] Utilidades de error messages (`getRewardErrorMessage`, `getRewardSpecificErrorMessage`)
- [ ] Codigo 4010 agregado en `ERROR_CODE_MESSAGES`
- [ ] Validaciones Zod alineadas con FluentValidation del backend
- [ ] Types alineados con DTOs de backend (C#)

---

## 10. Siguiente Paso Sugerido

Una vez implementado este plan de contratos compartidos, ejecutar en paralelo:

1. **Backend Agent** - Implementar `ReorderRewardsCommand`, validaciones de ownership, y soft delete con backings
2. **Admin Agent** - Crear dashboard de gestion de recompensas (lista, form modal, drag & drop)
3. **Landing Agent** - Crear vista publica de recompensas en detalle de campania

**Prioridad:** Backend primero para tener endpoints listos, luego Admin y Landing en paralelo.

---

**Fin del plan de contratos compartidos.**
