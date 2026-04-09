# Plan de Contratos Shared: Hacer Backing

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Basado en:** docs/user-stories/hacer-backing/contracts.md

## 1. Resumen

- Total de types: 6 nuevos (CampaniaDetail, RewardPublic, BackingPublicDto, CampaniaStats, modificaciones a Backing/BackingDto/CreateBackingRequest)
- Total de schemas: 1 modificado + 2 funciones de validacion adicionales (backing.schema.ts)
- Constantes definidas: 13 (query keys, API routes, app routes, estados de pedido)
- Utilidades planificadas: 3 nuevas funciones de error messages + 1 helper de display

## 2. Types (`src/shared/types/`)

### 2.1 Modificar `src/shared/types/backing.ts`

**Accion:** Reemplazar archivo completo para alinear con contracts.md

**Contenido:**

```typescript
/**
 * Types para Backings (Aportes a Campanias)
 * Alineado con WePlayRises.Crowdfunding.Application.Dtos.BackingDto
 */

// ========== Response DTOs ==========

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

// ========== Request DTOs ==========

export interface CreateBackingRequest {
  rewardId?: string; // opcional, null para aporte sin recompensa
  monto: number; // requerido, >= 1.00 EUR
  mensaje?: string; // opcional, max 500 caracteres
  esAnonimo?: boolean; // default false
}

// ========== Stats ==========

export interface CampaniaStats {
  campaniaId: string;
  totalBackers: number;
  totalRecaudado: number;
  promedioAporte: number;
  aporteMinimo: number;
  aporteMaximo: number;
  diasRestantes: number;
}

// ========== Legacy (deprecated) ==========

/**
 * @deprecated Use BackingDto instead
 * Kept for backwards compatibility
 */
export interface BackingStats {
  totalBackings: number;
  montoPromedio: number;
  ultimosBacking: BackingDto[];
}
```

**Justificacion:**
- Alinea `Backing` con backend: cambia `anonimo` → `esAnonimo`, `createdAt` → `fechaCreacion` (string ISO)
- Agrega `BackingPublicDto` para lista publica de aportes (GET /api/campanias/{id}/backings)
- Agrega `CampaniaStats` para endpoint GET /api/campanias/{id}/stats
- Modifica `CreateBackingRequest` para eliminar `campaniaId` (se pasa en URL)
- Usa campos opcionales con `?` en vez de `| undefined` por consistencia

### 2.2 Modificar `src/shared/types/campania.ts`

**Accion:** Agregar nuevos tipos sin eliminar existentes

**Contenido a AGREGAR al final del archivo:**

```typescript
// ========== Detalle de Campania con Rewards y Backings ==========

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
  porcentajeProgreso: number; // calculado en backend
  monedaId: number;
  monedaSimbolo: string; // EUR, USD, etc.
  estadoCampaniaId: number;
  estadoCampaniaNombre: string; // Publicada, Finalizada, etc.
  permiteAportacionesAnonimas: boolean;
  permitePropinas: boolean;
  fechaInicio?: string; // ISO 8601
  fechaFin?: string; // ISO 8601
  diasRestantes: number; // calculado en backend
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
  cantidadVendida: number; // calculado desde PedidoCrowdfundingLinea
  disponible: boolean; // cantidadMaxima == null || cantidadVendida < cantidadMaxima
  incluyeEnvioFisico: boolean;
  tiempoEntregaEstimado?: string;
  orden: number;
  esActivo: boolean;
}
```

**Justificacion:**
- `CampaniaDetail` es una extension de `Campania` con datos relacionados (rewards, backings)
- `RewardPublic` es una version reducida de `Reward` con campos calculados (`cantidadVendida`, `disponible`)
- Mantiene separacion de concerns: `Reward` (CRUD completo) vs `RewardPublic` (vista publica)
- Importa `BackingPublicDto` de `backing.ts`

**Dependencia:** Debe importar `BackingPublicDto` de `./backing.ts`

### 2.3 Modificar `src/shared/types/index.ts`

**Accion:** Asegurar que exporta todos los nuevos tipos

**Contenido:**
No requiere cambios si ya exporta `export * from "./backing"` y `export * from "./campania"`.
Los nuevos tipos se exportan automaticamente.

## 3. Schemas Zod (`src/shared/schemas/`)

### 3.1 Modificar `src/shared/schemas/backing.schema.ts`

**Accion:** Reemplazar archivo completo con schema mejorado + validaciones adicionales

**Contenido:**

```typescript
import { z } from "zod";
import type { RewardPublic } from "../types/campania";

/**
 * Schema de validacion para crear un backing
 * Alineado con CreateBackingCommand del backend
 */
export const createBackingSchema = z.object({
  rewardId: z
    .string()
    .uuid("ID de recompensa invalido")
    .optional(),
  monto: z
    .number({ invalid_type_error: "El monto debe ser un numero" })
    .min(1, "El monto minimo es 1 EUR")
    .max(100000, "El monto maximo es 100,000 EUR"),
  mensaje: z
    .string()
    .max(500, "El mensaje no puede superar los 500 caracteres")
    .optional()
    .or(z.literal("")), // permite strings vacios
  esAnonimo: z
    .boolean()
    .default(false),
});

export type CreateBackingFormData = z.infer<typeof createBackingSchema>;

// ========== Validaciones Adicionales (Client-Side) ==========

/**
 * Valida que el monto sea >= reward.importeMinimo
 * Retorna mensaje de error o null si es valido
 */
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

/**
 * Valida que la recompensa este disponible (stock)
 * Retorna mensaje de error o null si es valido
 */
export const validateRewardAvailability = (
  reward?: RewardPublic
): string | null => {
  if (!reward) return null;

  if (!reward.esActivo) {
    return "Esta recompensa ya no esta disponible";
  }

  if (!reward.disponible) {
    return "Esta recompensa esta agotada";
  }

  return null;
};

/**
 * Valida que la campania este activa y acepte backings
 * Retorna mensaje de error o null si es valido
 */
export const validateCampaniaActive = (
  estadoCampaniaId: number,
  fechaFin?: string
): string | null => {
  // PUBLICADA = 2
  if (estadoCampaniaId !== 2) {
    return "Esta campania no esta activa en este momento";
  }

  if (fechaFin) {
    const endDate = new Date(fechaFin);
    const now = new Date();
    if (now > endDate) {
      return "Esta campania ya ha finalizado";
    }
  }

  return null;
};

// ========== Legacy (deprecated) ==========

/**
 * @deprecated Use createBackingSchema instead
 */
export const backingSchema = createBackingSchema.extend({
  campaniaId: z.string().uuid("ID de campania invalido"),
});

/**
 * @deprecated Use CreateBackingFormData instead
 */
export type BackingFormData = z.infer<typeof backingSchema>;
```

**Justificacion:**
- Renombra `backingSchema` → `createBackingSchema` para claridad
- Agrega 3 funciones de validacion client-side que no se pueden hacer en Zod solo:
  - `validateBackingAmount`: valida monto vs reward.importeMinimo (requiere datos de reward)
  - `validateRewardAvailability`: valida stock de reward
  - `validateCampaniaActive`: valida estado de campania y fecha fin
- Mantiene compatibilidad con `backingSchema` legacy

### 3.2 Modificar `src/shared/schemas/index.ts`

**Accion:** Asegurar que exporta funciones de validacion

**Contenido:**

```typescript
export * from "./backing.schema";
// ... otros exports existentes
```

No requiere cambios si ya exporta `backing.schema.ts`.

## 4. Constantes (`src/shared/constants/`)

### 4.1 Modificar `src/shared/constants/index.ts`

**Accion:** Agregar/modificar constantes para backing

**Contenido a AGREGAR/MODIFICAR:**

```typescript
// ========== Query Keys - MODIFICAR seccion backings ==========

export const QUERY_KEYS = {
  // ... existentes (auth, artistas, campanias, rewards)

  // Backings - REEMPLAZAR seccion completa
  backings: {
    all: ["backings"] as const,
    byId: (id: string) => ["backings", id] as const,
    byCampania: (campaniaId: string) => ["backings", "campania", campaniaId] as const,
    me: ["backings", "me"] as const,
  },

  // ... legacy keys existentes
} as const;

// ========== API Routes - MODIFICAR seccion campanias y backings ==========

export const API_ROUTES = {
  // ... existentes (auth, artistas)

  // Campanias - AGREGAR nuevos endpoints
  campanias: {
    base: "/api/campanias",
    byId: (id: string) => `/api/campanias/${id}`,
    publicar: (id: string) => `/api/campanias/${id}/publicar`,
    misCampanias: "/api/campanias/mis-campanias",
    // NUEVOS:
    stats: (id: string) => `/api/campanias/${id}/stats`,
    backings: (id: string) => `/api/campanias/${id}/backings`,
  },

  // Rewards - mantener igual
  // ...

  // Backings - REEMPLAZAR seccion completa
  backings: {
    base: "/api/backings", // legacy endpoint (si existe)
    byId: (id: string) => `/api/backings/${id}`,
    create: (campaniaId: string) => `/api/campanias/${campaniaId}/backings`, // endpoint real
    byCampania: (campaniaId: string) => `/api/campanias/${campaniaId}/backings`,
  },
} as const;

// ========== App Routes - MODIFICAR seccion landing ==========

export const APP_ROUTES = {
  // ... existentes (auth, artista, dashboard)

  // Landing - AGREGAR nuevas rutas
  landing: {
    home: "/",
    explorar: "/explorar",
    artistaById: (id: string) => `/artistas/${id}`,
    campaniaById: (id: string) => `/campanias/${id}`,
    // NUEVOS:
    campanias: {
      list: "/campanias",
      detail: (id: string) => `/campanias/${id}`,
      apoyar: (id: string) => `/campanias/${id}/apoyar`,
    },
  },
} as const;

// ========== Estados de Pedido - AGREGAR nuevo ==========

export const ESTADO_PEDIDO = {
  PENDIENTE: 1,
  PROCESANDO: 2,
  COMPLETADO: 3,
  FALLIDO: 4,
  CANCELADO: 5,
} as const;

export const ESTADO_PEDIDO_LABELS: Record<number, string> = {
  1: "Pendiente",
  2: "Procesando",
  3: "Completado",
  4: "Fallido",
  5: "Cancelado",
};

// ========== Estados de Campania - MODIFICAR agregar PAUSADA ==========

export const CAMPANIA_ESTADOS = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
  PAUSADA: 5, // NUEVO
} as const;

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
  1: "Borrador",
  2: "Publicada",
  3: "Finalizada",
  4: "Cancelada",
  5: "Pausada", // NUEVO
};

export const CAMPANIA_ESTADO_COLORS: Record<number, string> = {
  1: "yellow",
  2: "green",
  3: "blue",
  4: "red",
  5: "gray", // NUEVO
};
```

**Justificacion:**
- `QUERY_KEYS.backings.byCampania`: nuevo key para listar backings de una campania
- `API_ROUTES.campanias.stats/backings`: nuevos endpoints de contratos
- `API_ROUTES.backings.create`: endpoint POST con campaniaId en URL
- `APP_ROUTES.landing.campanias`: estructura jerarquica para rutas de campanias
- `ESTADO_PEDIDO`: nuevas constantes para estados de pedido
- `CAMPANIA_ESTADOS.PAUSADA`: nuevo estado mencionado en contracts.md

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Modificar `src/shared/utils/error-messages.ts`

**Accion:** Agregar mensajes especificos de backing

**Contenido a AGREGAR al final del archivo:**

```typescript
// ========== Backing Error Messages - AGREGAR ==========

/**
 * Mensajes de error especificos para Backings
 * Alineado con ErrorCodes del backend (contracts.md)
 */
export const BACKING_ERROR_MESSAGES: Record<string, string> = {
  // Success codes
  "0000": "Operacion exitosa",
  "0001": "Aporte realizado con exito. Gracias por tu apoyo!",

  // Validation errors (1000-1999)
  "1001": "Este campo es obligatorio",
  "1002": "El mensaje no puede superar los 500 caracteres",
  "1011": "El monto debe ser al menos 1 EUR",
  "1012": "El ID de recompensa no es valido",
  "4012": "El monto esta por debajo del minimo requerido para esta recompensa",

  // Not Found errors (2000-2999)
  "2003": "No encontramos esta campania",
  "2004": "No encontramos esta recompensa",

  // Auth errors (3000-3999)
  "3001": "Tu sesion ha expirado. Por favor, inicia sesion nuevamente",
  "3005": "Debes iniciar sesion para hacer un aporte",

  // Business Rule errors (4000-4999)
  "4006": "Esta campania no esta activa en este momento",
  "4007": "Esta campania ya ha finalizado",
  "4011": "Esta recompensa esta agotada. Por favor, selecciona otra",
  "4013": "Esta campania no permite aportes anonimos. Por favor, inicia sesion",

  // Internal errors (5000-5999)
  "5000": "Ha ocurrido un error al procesar tu aporte. Por favor, intenta nuevamente",
} as const;

/**
 * Obtiene mensaje de error para backings
 */
export const getBackingErrorMessage = (errorCode: string): string => {
  return BACKING_ERROR_MESSAGES[errorCode] || BACKING_ERROR_MESSAGES["5000"];
};

/**
 * Obtiene mensaje de error contextual con datos de reward
 * Util para errores que requieren informacion dinamica
 */
export const getBackingContextualError = (
  errorCode: string,
  context?: { rewardNombre?: string; importeMinimo?: number }
): string => {
  if (errorCode === "4012" && context?.importeMinimo) {
    return `El monto debe ser al menos ${context.importeMinimo} EUR para "${context.rewardNombre}"`;
  }

  if (errorCode === "4011" && context?.rewardNombre) {
    return `La recompensa "${context.rewardNombre}" esta agotada. Selecciona otra opcion`;
  }

  return getBackingErrorMessage(errorCode);
};
```

**Justificacion:**
- Cubre todos los errorCodes especificados en contracts.md (tabla de errores)
- `getBackingContextualError`: permite mensajes dinamicos con datos de reward
- Mantiene consistencia con `getRewardErrorMessage`, `getCampaniaErrorMessage`

### 5.2 Modificar `src/shared/utils/error-messages.ts` (registro en ERROR_CODE_MESSAGES)

**Accion:** Agregar nuevos codes al diccionario global

**Contenido a AGREGAR en `ERROR_CODE_MESSAGES`:**

```typescript
export const ERROR_CODE_MESSAGES: Record<string, string> = {
  // ... existentes (0000-0003, 1001-1012, 2000-2005, 3001-3005, 4001-4010, 5000-5003)

  // Business Rule errors - AGREGAR nuevos codes
  "4011": "Esta recompensa esta agotada",
  "4012": "El monto esta por debajo del minimo requerido para esta recompensa",
  "4013": "Esta campania no permite aportes anonimos",

  // ... resto existente
} as const;
```

**Justificacion:**
- Agrega codes faltantes de backing para que `getErrorMessage()` los encuentre
- Mantiene diccionario global sincronizado con todos los features

### 5.3 Modificar `src/shared/utils/format.ts`

**Accion:** Agregar helper para display de nombre de backer

**Contenido a AGREGAR al final del archivo:**

```typescript
// ========== Backing Helpers - AGREGAR ==========

/**
 * Obtiene el nombre a mostrar de un backer
 * Logica: si esAnonimo o userName vacio/null → "Anonimo", sino userName
 */
export function getBackerDisplayName(
  esAnonimo: boolean,
  userName?: string
): string {
  if (esAnonimo || !userName) {
    return "Anonimo";
  }
  return userName;
}
```

**Justificacion:**
- Centraliza logica de display de nombre de backer (mencionada en contracts.md)
- Evita duplicacion en componentes de landing

### 5.4 Modificar `src/shared/utils/index.ts`

**Accion:** Asegurar que exporta nuevas funciones

**Contenido:**

```typescript
export * from "./cn";
export * from "./error-messages";
export * from "./format";
export * from "./mappers";
```

No requiere cambios si ya exporta todos los archivos.

## 6. Archivos a Crear/Modificar

### Resumen de Cambios

```
src/shared/
├── types/
│   ├── backing.ts                    # REEMPLAZAR completo
│   ├── campania.ts                   # AGREGAR CampaniaDetail y RewardPublic
│   └── index.ts                      # Sin cambios (ya exporta todo)
├── schemas/
│   ├── backing.schema.ts             # REEMPLAZAR completo (agregar validaciones)
│   └── index.ts                      # Sin cambios
├── constants/
│   └── index.ts                      # MODIFICAR (agregar query keys, API routes, app routes, estados)
└── utils/
    ├── error-messages.ts             # AGREGAR backing messages + codes faltantes
    ├── format.ts                     # AGREGAR getBackerDisplayName
    └── index.ts                      # Sin cambios
```

### Archivos sin cambios

- `src/shared/types/index.ts` (ya exporta `backing` y `campania`)
- `src/shared/schemas/index.ts` (ya exporta `backing.schema`)
- `src/shared/utils/index.ts` (ya exporta todos los utils)
- `src/shared/utils/cn.ts` (no requiere cambios)
- `src/shared/utils/mappers.ts` (opcional: agregar mappers de backing, pero no critico)

## 7. Dependencias

### Dependencias Existentes (ya instaladas)

- `zod` (validacion de schemas) ✓
- `clsx` + `tailwind-merge` (cn utility) ✓

### Dependencias Internas

- `types/backing.ts` → imports: ninguno
- `types/campania.ts` → imports: `BackingPublicDto` de `./backing`
- `schemas/backing.schema.ts` → imports: `RewardPublic` de `../types/campania`
- `constants/index.ts` → imports: ninguno
- `utils/error-messages.ts` → imports: ninguno
- `utils/format.ts` → imports: ninguno

### Orden de Implementacion Recomendado

1. `types/backing.ts` (sin dependencias)
2. `types/campania.ts` (depende de backing)
3. `schemas/backing.schema.ts` (depende de types/campania)
4. `constants/index.ts` (sin dependencias)
5. `utils/error-messages.ts` (sin dependencias)
6. `utils/format.ts` (sin dependencias)

## 8. Notas de Implementacion

### 8.1 Cambios de Naming

| Antes | Despues | Razon |
|-------|---------|-------|
| `anonimo` | `esAnonimo` | Alineacion con backend C# (prefijo `es` para booleans) |
| `createdAt` (Date) | `fechaCreacion` (string) | Consistencia con otros DTOs (campania, reward) |
| `BackingFormData` | `CreateBackingFormData` | Claridad de proposito |
| `campaniaId` en request | campaniaId en URL | Endpoint es POST /campanias/{id}/backings |

### 8.2 Validaciones Client vs Server

| Validacion | Client (Zod) | Server (FluentValidation) | Notas |
|------------|--------------|---------------------------|-------|
| monto >= 1 | ✓ | ✓ | Validacion basica |
| monto >= reward.importeMinimo | ✓ (runtime) | ✓ | Client usa `validateBackingAmount` |
| reward disponible | ✓ (runtime) | ✓ | Client usa `validateRewardAvailability` |
| campania activa | ✓ (runtime) | ✓ | Client usa `validateCampaniaActive` |
| anonimos permitidos | UI (show/hide login) | ✓ | Client deshabilita UI si no permite |

**Client-side**: Validaciones en 3 capas:
1. Zod schema (tipos, rangos basicos)
2. Runtime validators (reglas de negocio con datos relacionados)
3. UI guards (deshabilitar botones, mostrar modals)

### 8.3 Tipos Opcionales vs Undefined

```typescript
// PATRON ADOPTADO - Usar opcional con ?
interface Backing {
  userId?: string;  // ✓ CORRECTO
  rewardId?: string;
}

// EVITAR - Union con undefined
interface Backing {
  userId: string | undefined;  // ✗ EVITAR
}
```

**Razon**: `?` es mas conciso y semanticamente equivalente para DTOs simples.

### 8.4 Constantes de Query Keys - Patron Jerarquico

```typescript
// CORRECTO - Jerarquia para invalidacion granular
QUERY_KEYS.backings.byCampania(campaniaId)  // ['backings', 'campania', campaniaId]

// EVITAR - Keys planas
QUERY_KEYS.BACKINGS_BY_CAMPANIA  // 'backings-by-campania'
```

**Beneficio**: Permite invalidar todas las queries de `backings` con `queryClient.invalidateQueries(['backings'])`.

### 8.5 API Routes vs App Routes

| Constante | Uso | Ejemplo |
|-----------|-----|---------|
| `API_ROUTES` | Llamadas a backend | `fetch(API_ROUTES.campanias.stats(id))` |
| `APP_ROUTES` | Navegacion frontend | `navigate(APP_ROUTES.landing.campanias.detail(id))` |

**Importante**: No confundir. `API_ROUTES` empieza con `/api`, `APP_ROUTES` no.

### 8.6 Estados de Campania - Enum vs Constantes

```typescript
// MANTENER - Constantes numericas (compatibilidad con backend)
CAMPANIA_ESTADOS.PUBLICADA  // 2

// DEPRECADO - Enum de TypeScript
EstadoCampania.Publicada  // 2
```

**Decision**: Usar constantes `as const` en lugar de enums para:
- Menor bundle size
- Interoperabilidad con JavaScript
- Consistencia con el resto del codebase

### 8.7 Mensajes de Error - Estrategia de Fallback

```typescript
// Orden de fallback en getBackingErrorMessage:
1. BACKING_ERROR_MESSAGES[code]      // Especifico de backing
2. ERROR_CODE_MESSAGES[code]         // Global
3. ERROR_MESSAGES[code]              // Legacy
4. BACKING_ERROR_MESSAGES["5000"]    // Default error
```

**Beneficio**: Permite mensajes contextuales sin duplicar codes globales.

## 9. Checklist

- [ ] `types/backing.ts` reemplazado con nuevos tipos
- [ ] `types/campania.ts` ampliado con CampaniaDetail y RewardPublic
- [ ] `schemas/backing.schema.ts` reemplazado con validaciones adicionales
- [ ] `constants/index.ts` actualizado con query keys, API routes, app routes
- [ ] `constants/index.ts` agregar ESTADO_PEDIDO y CAMPANIA_ESTADOS.PAUSADA
- [ ] `utils/error-messages.ts` agregar BACKING_ERROR_MESSAGES y helpers
- [ ] `utils/error-messages.ts` agregar codes 4011, 4012, 4013 a ERROR_CODE_MESSAGES
- [ ] `utils/format.ts` agregar getBackerDisplayName
- [ ] Verificar imports entre archivos (backing → campania → schemas)
- [ ] Verificar que index.ts exportan nuevos tipos/funciones

## 10. Testing Checklist (Post-Implementacion)

- [ ] Todos los tipos compilan sin errores de TypeScript
- [ ] Zod schemas validan correctamente casos edge:
  - Monto negativo → error
  - Mensaje > 500 chars → error
  - rewardId invalido (no UUID) → error
  - esAnonimo default a false
- [ ] Validaciones runtime retornan mensajes correctos:
  - `validateBackingAmount(5, { importeMinimo: 10 })` → error
  - `validateRewardAvailability({ disponible: false })` → error
  - `validateCampaniaActive(1)` → error (no PUBLICADA)
- [ ] Error messages devuelven strings en español
- [ ] `getBackerDisplayName(true, "Juan")` → "Anonimo"
- [ ] `getBackerDisplayName(false, "Juan")` → "Juan"
- [ ] Query keys son arrays readonly (`as const`)
- [ ] API routes generan URLs correctas:
  - `API_ROUTES.backings.create("abc-123")` → `/api/campanias/abc-123/backings`

## 11. Migracion de Codigo Existente

### Componentes que Usan Tipos Legacy

Si existen componentes usando los tipos antiguos, deben migrar:

```typescript
// ANTES
import { BackingDto, CreateBackingDto } from '@/shared/types/backing';
const [formData, setFormData] = useState<CreateBackingDto>({ ...});

// DESPUES
import { BackingDto, CreateBackingRequest } from '@/shared/types/backing';
const [formData, setFormData] = useState<CreateBackingRequest>({ ...});
```

**Cambios necesarios:**
- `CreateBackingDto` → `CreateBackingRequest`
- `anonimo` → `esAnonimo`
- `createdAt: Date` → `fechaCreacion: string`

### Scripts de Busqueda (Opcional)

```bash
# Buscar usos de tipos deprecated
grep -r "CreateBackingDto" src/web src/admin
grep -r "BackingFormData" src/web src/admin
grep -r "\.anonimo" src/web src/admin
```

## 12. Proximos Pasos

Despues de implementar estos contratos shared:

1. **Backend**: Ejecutar agente de backend para crear:
   - `CampaniaDetailDto.cs`
   - `BackingPublicDto.cs`
   - `CampaniaStatsDto.cs`
   - `RewardPublicDto.cs`
   - `CreateBackingCommand.cs`
   - Handlers, validators, services
   - Endpoints nuevos en CampaniasController

2. **Landing**: Ejecutar agente de frontend para crear:
   - `/campanias` lista de campanias
   - `/campanias/{id}` detalle con rewards y backings
   - `/campanias/{id}/apoyar` formulario de backing
   - Componentes: CampaniaCard, RewardCard, BackingForm
   - Hooks: useCampaniaDetail, useCampaniaStats, useCreateBacking

3. **Testing E2E**: Crear tests de flujo completo:
   - Usuario explora campanias → selecciona campania → ve rewards → hace backing
   - Usuario anonimo intenta backing en campania que no permite anonimos → redirect login

---

**Fin del plan de contratos shared para hacer-backing.**
