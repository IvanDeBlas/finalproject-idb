# Plan de Contratos Shared: Crear Campania

**Fecha:** 2026-02-12
**Feature:** crear-campania (US-02)
**Basado en:** docs/user-stories/crear-campania/contracts.md

---

## 1. Resumen

- Total de types: 7 (Campania, CampaniaListItem, CreateCampaniaRequest, UpdateCampaniaRequest, PublishCampaniaResponse, + 2 enums)
- Total de schemas: 6 (4 schemas por step del wizard + createCampaniaSchema + updateCampaniaSchema + publishCampaniaSchema)
- Constantes definidas: 4 grupos (QUERY_KEYS, API_ROUTES, APP_ROUTES, estados y tipos de financiacion con labels)
- Utilidades planificadas: 3 (error message helpers + formatCurrency mejorado)

---

## 2. Types (`src/shared/types/campania.ts`)

**Accion:** ACTUALIZAR archivo existente

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `Campania` | id, artistaId, proyectoArtisticoId?, titulo, subtitulo?, descripcionCorta?, videoPrincipalUrl?, imagenPrincipalUrl?, monedaId, importeObjetivo, importeMinimo?, importePledgedActual, tipoFinanciacionId, estadoCampaniaId, permiteAportacionesAnonimas, permitePropinas, porcentajeComisionPlataforma?, fechaInicio?, fechaFin?, fechaPublicacion?, fechaCierre?, fechaCreacion, fechaActualizacion? | DTO completo de campania, todos los campos en camelCase, fechas como string ISO 8601 |
| `CampaniaListItem` | id, artistaId, titulo, subtitulo?, descripcionCorta?, imagenPrincipalUrl?, importeObjetivo, importePledgedActual, estadoCampaniaId, fechaInicio?, fechaFin?, fechaCreacion | DTO para listar campanias (subset de Campania para performance) |
| `PublishCampaniaResponse` | id, estadoCampaniaId, fechaPublicacion, message | Respuesta del endpoint de publicar campania |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreateCampaniaRequest` | proyectoArtisticoId?, titulo, subtitulo?, descripcionCorta?, videoPrincipalUrl?, imagenPrincipalUrl?, monedaId (default 1), importeObjetivo, importeMinimo?, tipoFinanciacionId, permiteAportacionesAnonimas (default false), permitePropinas (default false), fechaInicio?, fechaFin? | DTO para crear campania (sin ArtistaId, se extrae del token) |
| `UpdateCampaniaRequest` | id, titulo?, subtitulo?, descripcionCorta?, videoPrincipalUrl?, imagenPrincipalUrl?, importeObjetivo?, importeMinimo?, tipoFinanciacionId?, permiteAportacionesAnonimas?, permitePropinas?, fechaInicio?, fechaFin? | DTO para actualizar campania (todos opcionales excepto id) |

### 2.3 Enums y Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `EstadoCampania` | Borrador = 1, Publicada = 2, Finalizada = 3, Cancelada = 4 | Enum numerico para estados de campania (alineado con backend) |
| `TipoFinanciacion` | TodoONada = 1, FlexibleGoal = 2 | Enum numerico para tipos de financiacion |

### 2.4 Estructura del Archivo Actualizado

```typescript
// src/shared/types/campania.ts

// ==========================================
// NOTA: Este archivo reemplaza el contenido existente
// Mantiene compatibilidad con interfaces legacy si se usan en otros lados
// ==========================================

// DTOs alineados con contracts.md
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
  monedaId: number; // default 1 en form
  importeObjetivo: number;
  importeMinimo?: number;
  tipoFinanciacionId: number;
  permiteAportacionesAnonimas: boolean; // default false
  permitePropinas: boolean; // default false
  fechaInicio?: string; // ISO 8601
  fechaFin?: string; // ISO 8601
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

// Enums (alineados con backend)
export enum EstadoCampania {
  Borrador = 1,
  Publicada = 2,
  Finalizada = 3,
  Cancelada = 4,
}

export enum TipoFinanciacion {
  TodoONada = 1,
  FlexibleGoal = 2,
}

// Legacy types (mantener por compatibilidad temporal, marcar como deprecated)
/** @deprecated Use EstadoCampania enum instead */
export type CampaniaEstado = "borrador" | "activa" | "finalizada" | "cancelada";

/** @deprecated Use Campania interface instead */
export interface CampaniaDto {
  id: string;
  artistaId: string;
  titulo: string;
  descripcion: string;
  importeObjetivo: number;
  importePledgedActual: number;
  fechaInicio: string;
  fechaFin: string;
  estado: string;
  imagenUrl?: string;
  videoUrl?: string;
  createdAt: string;
  updatedAt: string;
}

/** @deprecated Use CreateCampaniaRequest instead */
export interface CreateCampaniaDto {
  titulo: string;
  descripcion: string;
  importeObjetivo: number;
  fechaFin: string;
  imagenUrl?: string;
}

/** @deprecated Use UpdateCampaniaRequest instead */
export interface UpdateCampaniaDto {
  titulo?: string;
  descripcion?: string;
  importeObjetivo?: number;
  fechaFin?: string;
  imagenUrl?: string;
}

// Stats (mantener, no cambiar)
export interface CampaniaStats {
  totalBackers: number;
  totalRecaudado: number;
  porcentajeCompletado: number;
  diasRestantes: number;
}
```

---

## 3. Schemas Zod (`src/shared/schemas/campania.schema.ts`)

**Accion:** ACTUALIZAR archivo existente

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas |
|--------|--------|--------|
| `campaniaBasicInfoSchema` | titulo, subtitulo?, descripcionCorta? | titulo: min(1), max(200); subtitulo: max(300) optional; descripcionCorta: max(500) optional |
| `campaniaFundingSchema` | importeObjetivo, importeMinimo?, monedaId, tipoFinanciacionId, permiteAportacionesAnonimas, permitePropinas | importeObjetivo: positive, min(100); importeMinimo: positive, lte(importeObjetivo) optional; tipoFinanciacionId: positive; refine: importeMinimo <= importeObjetivo |
| `campaniaDurationSchema` | fechaInicio?, fechaFin? | fechaFin: datetime, refine(>= now + 7 days); refine: fechaFin > fechaInicio si ambos estan presentes |
| `campaniaMediaSchema` | imagenPrincipalUrl?, videoPrincipalUrl?, proyectoArtisticoId? | URLs: url() optional; proyectoArtisticoId: uuid optional |
| `createCampaniaSchema` | merge de los 4 anteriores | Schema completo para wizard, valida todos los steps combinados |
| `updateCampaniaSchema` | id + campos de create opcionales | id: uuid required; resto opcional pero con mismas reglas de validacion |
| `publishCampaniaSchema` | titulo, importeObjetivo, monedaId, tipoFinanciacionId, fechaFin | Validacion de campos requeridos para publicar, fechaFin: refine(>= now + 7 days) |

### 3.2 Types Inferidos

- `CreateCampaniaFormData` = `z.infer<typeof createCampaniaSchema>`
- `UpdateCampaniaFormData` = `z.infer<typeof updateCampaniaSchema>`
- `PublishCampaniaValidation` = `z.infer<typeof publishCampaniaSchema>`

### 3.3 Estructura del Archivo Actualizado

```typescript
// src/shared/schemas/campania.schema.ts

// ==========================================
// NOTA: Este archivo reemplaza el contenido existente
// Schemas alineados con contracts.md y validaciones backend
// ==========================================

import { z } from "zod";
import { addDays } from "date-fns";

// ========== STEP 1: Informacion Basica ==========
export const campaniaBasicInfoSchema = z.object({
  titulo: z
    .string()
    .min(1, "El titulo es obligatorio")
    .max(200, "El titulo no puede superar los 200 caracteres"),
  subtitulo: z
    .string()
    .max(300, "El subtitulo no puede superar los 300 caracteres")
    .optional()
    .or(z.literal("")),
  descripcionCorta: z
    .string()
    .max(500, "La descripcion corta no puede superar los 500 caracteres")
    .optional()
    .or(z.literal("")),
});

// ========== STEP 2: Meta Financiera ==========
export const campaniaFundingSchema = z
  .object({
    importeObjetivo: z
      .number({ invalid_type_error: "Debe ser un numero" })
      .positive("El importe objetivo debe ser mayor a 0")
      .min(100, "El importe minimo es 100 EUR"),
    importeMinimo: z
      .number()
      .positive("El importe minimo debe ser mayor a 0")
      .optional(),
    monedaId: z.number().default(1), // EUR
    tipoFinanciacionId: z
      .number()
      .positive("Debe seleccionar un tipo de financiacion"),
    permiteAportacionesAnonimas: z.boolean().default(false),
    permitePropinas: z.boolean().default(false),
  })
  .refine(
    (data) => {
      if (data.importeMinimo) {
        return data.importeMinimo <= data.importeObjetivo;
      }
      return true;
    },
    {
      message: "El importe minimo no puede ser mayor al objetivo",
      path: ["importeMinimo"],
    }
  );

// ========== STEP 3: Duracion ==========
export const campaniaDurationSchema = z
  .object({
    fechaInicio: z
      .string()
      .datetime("Formato de fecha invalido")
      .optional(),
    fechaFin: z
      .string()
      .datetime("Formato de fecha invalido")
      .refine(
        (fecha) => {
          if (!fecha) return true;
          const minDate = addDays(new Date(), 7);
          return new Date(fecha) >= minDate;
        },
        {
          message: "La campania debe durar minimo 7 dias",
        }
      )
      .optional(),
  })
  .refine(
    (data) => {
      if (data.fechaInicio && data.fechaFin) {
        return new Date(data.fechaFin) > new Date(data.fechaInicio);
      }
      return true;
    },
    {
      message: "La fecha de fin debe ser posterior a la fecha de inicio",
      path: ["fechaFin"],
    }
  );

// ========== STEP 4: Multimedia ==========
export const campaniaMediaSchema = z.object({
  imagenPrincipalUrl: z
    .string()
    .url("Debe ser una URL valida")
    .optional()
    .or(z.literal("")),
  videoPrincipalUrl: z
    .string()
    .url("Debe ser una URL valida")
    .optional()
    .or(z.literal("")),
  proyectoArtisticoId: z.string().uuid().optional(),
});

// ========== Schema Completo para Crear Campania ==========
export const createCampaniaSchema = campaniaBasicInfoSchema
  .merge(campaniaFundingSchema)
  .merge(campaniaDurationSchema)
  .merge(campaniaMediaSchema);

export type CreateCampaniaFormData = z.infer<typeof createCampaniaSchema>;

// ========== Schema para Actualizar Campania ==========
export const updateCampaniaSchema = z
  .object({
    id: z.string().uuid("ID invalido"),
    titulo: z
      .string()
      .max(200, "El titulo no puede superar los 200 caracteres")
      .optional(),
    subtitulo: z
      .string()
      .max(300, "El subtitulo no puede superar los 300 caracteres")
      .optional(),
    descripcionCorta: z
      .string()
      .max(500, "La descripcion corta no puede superar los 500 caracteres")
      .optional(),
    videoPrincipalUrl: z.string().url("URL invalida").optional(),
    imagenPrincipalUrl: z.string().url("URL invalida").optional(),
    importeObjetivo: z.number().positive().optional(),
    importeMinimo: z.number().positive().optional(),
    tipoFinanciacionId: z.number().positive().optional(),
    permiteAportacionesAnonimas: z.boolean().optional(),
    permitePropinas: z.boolean().optional(),
    fechaInicio: z.string().datetime().optional(),
    fechaFin: z.string().datetime().optional(),
  })
  .refine(
    (data) => {
      if (data.importeMinimo && data.importeObjetivo) {
        return data.importeMinimo <= data.importeObjetivo;
      }
      return true;
    },
    {
      message: "El importe minimo no puede ser mayor al objetivo",
      path: ["importeMinimo"],
    }
  )
  .refine(
    (data) => {
      if (data.fechaInicio && data.fechaFin) {
        return new Date(data.fechaFin) > new Date(data.fechaInicio);
      }
      return true;
    },
    {
      message: "La fecha de fin debe ser posterior a la fecha de inicio",
      path: ["fechaFin"],
    }
  );

export type UpdateCampaniaFormData = z.infer<typeof updateCampaniaSchema>;

// ========== Schema para Validar Publicacion ==========
export const publishCampaniaSchema = z.object({
  titulo: z.string().min(1, "El titulo es obligatorio para publicar"),
  importeObjetivo: z.number().positive("El importe objetivo es obligatorio"),
  monedaId: z.number().positive(),
  tipoFinanciacionId: z
    .number()
    .positive("El tipo de financiacion es obligatorio"),
  fechaFin: z
    .string()
    .datetime()
    .refine(
      (fecha) => {
        const minDate = addDays(new Date(), 7);
        return new Date(fecha) >= minDate;
      },
      {
        message: "La fecha de fin debe ser minimo 7 dias desde hoy",
      }
    ),
});

export type PublishCampaniaValidation = z.infer<typeof publishCampaniaSchema>;

// ========== Legacy Schemas (mantener por compatibilidad) ==========
/** @deprecated Use createCampaniaSchema instead */
export const createCampaniaSchemaLegacy = z.object({
  titulo: z.string().min(1, "El titulo es obligatorio").max(200),
  descripcion: z.string().min(1, "La descripcion es obligatoria").max(5000),
  importeObjetivo: z.number().min(100).max(1000000),
  fechaFin: z.date().refine((date) => date > new Date(), {
    message: "La fecha de fin debe ser futura",
  }),
  imagenUrl: z.string().url("URL invalida").optional().or(z.literal("")),
  videoUrl: z.string().url("URL invalida").optional().or(z.literal("")),
});

/** @deprecated Use updateCampaniaSchema instead */
export const updateCampaniaSchemaLegacy = createCampaniaSchemaLegacy.partial();
```

**Dependencias adicionales:** Instalar `date-fns` si no existe (`npm install date-fns`)

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Accion:** ACTUALIZAR archivo existente (merge con contenido actual)

### 4.1 Endpoints API (Actualizaciones)

| Constante | Valor | Uso |
|-----------|-------|-----|
| `API_ROUTES.campanias.publicar` | `(id: string) => '/api/campanias/${id}/publicar'` | Endpoint para publicar campania |
| `API_ROUTES.campanias.misCampanias` | `'/api/campanias/mis-campanias'` | Endpoint para listar mis campanias |

### 4.2 Query Keys (React Query) - Actualizaciones

| Key | Patron | Uso |
|-----|--------|-----|
| `QUERY_KEYS.campanias.misCampanias` | `['campanias', 'mis-campanias']` | Query key para listar mis campanias |
| `QUERY_KEYS.campanias.filtered` | `(filters: Record<string, any>) => ['campanias', 'filtered', filters]` | Query key para listar campanias con filtros |

### 4.3 Estados y Valores (Actualizaciones)

**NOTA:** Cambiar de string a number para alinearse con backend

| Constante | Tipo | Valores |
|-----------|------|---------|
| `CAMPANIA_ESTADOS` | `Record<string, number>` | `{ BORRADOR: 1, PUBLICADA: 2, FINALIZADA: 3, CANCELADA: 4 }` |
| `CAMPANIA_ESTADOS_LABELS` | `Record<number, string>` | `{ 1: 'Borrador', 2: 'Publicada', 3: 'Finalizada', 4: 'Cancelada' }` |
| `TIPO_FINANCIACION` | `Record<string, number>` | `{ TODO_O_NADA: 1, FLEXIBLE: 2 }` |
| `TIPO_FINANCIACION_LABELS` | `Record<number, string>` | `{ 1: 'Todo o Nada', 2: 'Meta Flexible' }` |
| `MONEDAS` | `Record<string, number>` | `{ EUR: 1, USD: 2 }` |
| `MONEDA_SYMBOLS` | `Record<number, string>` | `{ 1: '€', 2: '$' }` |
| `MONEDA_CODES` | `Record<number, string>` | `{ 1: 'EUR', 2: 'USD' }` |

### 4.4 App Routes (Actualizaciones)

| Constante | Valor | Uso |
|-----------|-------|-----|
| `APP_ROUTES.dashboard.campanias.list` | `'/dashboard/campanias'` | Ruta lista campanias dashboard |
| `APP_ROUTES.dashboard.campanias.nueva` | `'/dashboard/campanias/nueva'` | Ruta wizard crear campania |
| `APP_ROUTES.dashboard.campanias.editar` | `(id: string) => '/dashboard/campanias/${id}/editar'` | Ruta editar campania |
| `APP_ROUTES.dashboard.campanias.detalle` | `(id: string) => '/dashboard/campanias/${id}'` | Ruta detalle campania dashboard |
| `APP_ROUTES.landing.explorar` | `'/explorar'` | Ruta explorar campanias publicas |
| `APP_ROUTES.landing.campaniaById` | `(id: string) => '/campanias/${id}'` | Ruta detalle campania publica |

### 4.5 Actualizaciones a Realizar

```typescript
// src/shared/constants/index.ts

// ========== ACTUALIZACIONES ==========

// 1. Actualizar API_ROUTES.campanias
export const API_ROUTES = {
  // ... existentes
  campanias: {
    base: "/api/campanias",
    byId: (id: string) => `/api/campanias/${id}`,
    publicar: (id: string) => `/api/campanias/${id}/publicar`, // NUEVO
    misCampanias: "/api/campanias/mis-campanias", // NUEVO
  },
  // ... resto
} as const;

// 2. Actualizar QUERY_KEYS.campanias
export const QUERY_KEYS = {
  // ... existentes
  campanias: {
    all: ["campanias"] as const,
    byId: (id: string) => ["campanias", id] as const,
    byArtista: (artistaId: string) => ["campanias", "artista", artistaId] as const,
    stats: (id: string) => ["campanias", id, "stats"] as const,
    misCampanias: ["campanias", "mis-campanias"] as const, // NUEVO
    filtered: (filters: Record<string, any>) => ["campanias", "filtered", filters] as const, // NUEVO
  },
  // ... resto
} as const;

// 3. REEMPLAZAR CAMPANIA_ESTADOS (cambiar de string a number)
export const CAMPANIA_ESTADOS = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
} as const;

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
  1: "Borrador",
  2: "Publicada",
  3: "Finalizada",
  4: "Cancelada",
};

export const CAMPANIA_ESTADO_COLORS: Record<number, string> = {
  1: "yellow",
  2: "green",
  3: "blue",
  4: "red",
};

// 4. AGREGAR nuevas constantes para tipos de financiacion
export const TIPO_FINANCIACION = {
  TODO_O_NADA: 1,
  FLEXIBLE: 2,
} as const;

export const TIPO_FINANCIACION_LABELS: Record<number, string> = {
  1: "Todo o Nada",
  2: "Meta Flexible",
};

export const TIPO_FINANCIACION_DESCRIPTIONS: Record<number, string> = {
  1: "Solo recibiras los fondos si alcanzas tu meta",
  2: "Recibiras los fondos recaudados aunque no alcances tu meta",
};

// 5. AGREGAR constantes de monedas
export const MONEDAS = {
  EUR: 1,
  USD: 2,
} as const;

export const MONEDA_SYMBOLS: Record<number, string> = {
  1: "€",
  2: "$",
};

export const MONEDA_CODES: Record<number, string> = {
  1: "EUR",
  2: "USD",
};

// 6. Actualizar APP_ROUTES
export const APP_ROUTES = {
  // ... existentes
  dashboard: {
    campanias: {
      list: "/dashboard/campanias", // NUEVO
      nueva: "/dashboard/campanias/nueva", // NUEVO
      editar: (id: string) => `/dashboard/campanias/${id}/editar`, // NUEVO
      detalle: (id: string) => `/dashboard/campanias/${id}`, // NUEVO
    },
  },
  landing: {
    home: "/",
    explorar: "/explorar", // NUEVO
    artistaById: (id: string) => `/artistas/${id}`,
    campaniaById: (id: string) => `/campanias/${id}`,
  },
  // ... resto
} as const;

// 7. MANTENER legacy exports con deprecation warning
/** @deprecated Use CAMPANIA_ESTADOS with numeric values instead */
export const CAMPANIA_ESTADOS_LEGACY = {
  BORRADOR: "borrador",
  ACTIVA: "activa",
  FINALIZADA: "finalizada",
  CANCELADA: "cancelada",
} as const;
```

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

**Accion:** CREAR nuevo archivo

### 5.1 Error Messages Mapping

| Codigo | Mensaje | Contexto |
|--------|---------|----------|
| 0000 | Operacion exitosa | Success |
| 0001 | Creado exitosamente | Success create |
| 0002 | Actualizado exitosamente | Success update |
| 1001 | Este campo es obligatorio | Validation required |
| 1002 | El valor supera el maximo de caracteres permitido | Validation max length |
| 1006 | La URL proporcionada no es valida | Validation URL |
| 1007 | El valor esta fuera del rango permitido | Validation range |
| 1011 | El monto no es valido | Validation amount |
| 1012 | La fecha no es valida | Validation date |
| 2003 | Campania no encontrada | Not found |
| 3001 | No autorizado. Por favor, inicia sesion nuevamente | Unauthorized |
| 3002 | No tienes permiso para realizar esta accion | Forbidden |
| 4009 | Solo se pueden editar campanias en estado borrador | Business rule |
| 5000 | Ha ocurrido un error inesperado | Internal error |

### 5.2 Estructura del Archivo

```typescript
// src/shared/utils/error-messages.ts

/**
 * Mapeo de codigos de error backend a mensajes amigables para el usuario
 * Alineado con ServiceResponseMessageType del backend
 */

export const ERROR_MESSAGES: Record<string, string> = {
  // ========== Success codes (0000-0999) ==========
  "0000": "Operacion exitosa",
  "0001": "Creado exitosamente",
  "0002": "Actualizado exitosamente",
  "0003": "Eliminado exitosamente",

  // ========== Validation errors (1000-1999) ==========
  "1001": "Este campo es obligatorio",
  "1002": "El valor supera el maximo de caracteres permitido",
  "1003": "El valor no cumple con el minimo requerido",
  "1004": "El formato del valor no es valido",
  "1005": "El formato del email no es valido",
  "1006": "La URL proporcionada no es valida",
  "1007": "El valor esta fuera del rango permitido",
  "1008": "Este nombre ya esta en uso",
  "1009": "Este email ya esta registrado",
  "1010": "La referencia proporcionada no existe",
  "1011": "El monto no es valido",
  "1012": "La fecha no es valida",

  // ========== Not Found errors (2000-2999) ==========
  "2000": "Recurso no encontrado",
  "2002": "Artista no encontrado",
  "2003": "Campania no encontrada",
  "2004": "Recompensa no encontrada",
  "2005": "Aporte no encontrado",

  // ========== Auth errors (3000-3999) ==========
  "3001": "No autorizado. Por favor, inicia sesion nuevamente",
  "3002": "No tienes permiso para realizar esta accion",
  "3003": "Tu sesion ha expirado. Por favor, inicia sesion nuevamente",
  "3004": "Token invalido",
  "3005": "Debes iniciar sesion para continuar",

  // ========== Business Rule errors (4000-4999) ==========
  "4001": "Este registro ya existe",
  "4002": "El estado actual no permite esta operacion",
  "4003": "Esta operacion no esta permitida",
  "4004": "Se ha excedido el limite permitido",
  "4005": "Fondos insuficientes",
  "4006": "La campania no esta activa",
  "4007": "La campania ha finalizado",
  "4009": "Solo se pueden editar campanias en estado borrador",

  // ========== Internal errors (5000-5999) ==========
  "5000": "Ha ocurrido un error inesperado. Por favor, intenta nuevamente",
  "5001": "Error de base de datos",
  "5002": "Error de servicio externo",
  "5003": "Error de configuracion",
} as const;

/**
 * Obtiene el mensaje de error amigable para un codigo dado
 * @param errorCode - Codigo de error del backend
 * @returns Mensaje de error localizado
 */
export const getErrorMessage = (errorCode: string): string => {
  return ERROR_MESSAGES[errorCode] || ERROR_MESSAGES["5000"];
};

/**
 * Helper para obtener mensajes especificos de errores de campania
 * Sobrescribe mensajes genericos con versiones contextuales
 * @param errorCode - Codigo de error del backend
 * @returns Mensaje de error contextual para campanias
 */
export const getCampaniaErrorMessage = (errorCode: string): string => {
  const customMessages: Record<string, string> = {
    "1001": "Completa todos los campos obligatorios para continuar",
    "1011": "El importe debe ser mayor a 0. Ingresa un monto valido",
    "1012": "La fecha de fin debe ser posterior a la fecha de inicio",
    "2003": "No encontramos esta campania. Puede haber sido eliminada",
    "3002": "No tienes permiso para modificar esta campania",
    "4009": "No puedes editar una campania que ya ha sido publicada",
  };

  return customMessages[errorCode] || getErrorMessage(errorCode);
};

/**
 * Helper para extraer y formatear multiples mensajes de error
 * @param messages - Array de ServiceResponseMessage del backend
 * @returns Array de mensajes formateados
 */
export const formatErrorMessages = (
  messages: Array<{ message: string; errorCode: string }>
): string[] => {
  return messages
    .filter((m) => m.errorCode && !m.errorCode.startsWith("0")) // Filtrar solo errores (no success)
    .map((m) => getErrorMessage(m.errorCode));
};

/**
 * Helper para determinar si un errorCode es de exito
 * @param errorCode - Codigo de error del backend
 * @returns true si es codigo de exito (0xxx)
 */
export const isSuccessCode = (errorCode: string): boolean => {
  return errorCode.startsWith("0");
};

/**
 * Helper para determinar si un errorCode requiere re-autenticacion
 * @param errorCode - Codigo de error del backend
 * @returns true si el usuario debe volver a autenticarse
 */
export const requiresReAuth = (errorCode: string): boolean => {
  return ["3001", "3003", "3004"].includes(errorCode);
};
```

---

## 6. Utilidades (`src/shared/utils/format.ts`)

**Accion:** ACTUALIZAR archivo existente (agregar helper para monedas)

### 6.1 Nueva Funcionalidad

Agregar helper `formatCurrencyWithSymbol` que usa MONEDA_SYMBOLS:

```typescript
// src/shared/utils/format.ts

// ========== AGREGAR AL FINAL DEL ARCHIVO EXISTENTE ==========

import { MONEDA_SYMBOLS, MONEDA_CODES } from "../constants";

/**
 * Formatea un monto con el simbolo de moneda segun monedaId
 * @param amount - Monto a formatear
 * @param monedaId - ID de moneda (1=EUR, 2=USD)
 * @returns Monto formateado con simbolo
 */
export function formatCurrencyWithSymbol(
  amount: number,
  monedaId: number = 1
): string {
  const currencyCode = MONEDA_CODES[monedaId] || "EUR";
  const locale = monedaId === 1 ? "es-ES" : "en-US";

  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency: currencyCode,
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(amount);
}

/**
 * Obtiene solo el simbolo de moneda
 * @param monedaId - ID de moneda
 * @returns Simbolo de moneda (€, $)
 */
export function getCurrencySymbol(monedaId: number = 1): string {
  return MONEDA_SYMBOLS[monedaId] || "€";
}
```

---

## 7. Archivos a Crear/Modificar

### 7.1 Resumen de Acciones

```
src/shared/
├── types/
│   └── campania.ts                     [ACTUALIZAR - reemplazar con nuevos DTOs]
├── schemas/
│   └── campania.schema.ts              [ACTUALIZAR - agregar schemas por step]
├── constants/
│   └── index.ts                        [ACTUALIZAR - agregar constantes de monedas, tipos, rutas]
└── utils/
    ├── error-messages.ts               [CREAR - nuevo archivo]
    └── format.ts                       [ACTUALIZAR - agregar formatCurrencyWithSymbol]
```

### 7.2 Orden de Implementacion Recomendado

1. **Types (`campania.ts`)** - Base de todo, define contratos
2. **Constants (`index.ts`)** - Necesarios para schemas y utils
3. **Utils (`error-messages.ts`)** - Standalone, no depende de schemas
4. **Utils (`format.ts`)** - Agregar helpers de moneda
5. **Schemas (`campania.schema.ts`)** - Depende de types y constants

### 7.3 Validacion Post-Implementacion

Verificar que:
- No hay errores de TypeScript en `src/shared/`
- Todos los exports estan en `src/shared/index.ts`
- Tests de schemas pasan (crear tests basicos)
- Web y Admin pueden importar desde `@/shared`

---

## 8. Dependencias

### 8.1 Dependencias Existentes
- `zod` - Ya instalado
- `tailwind-merge` - Ya instalado
- `clsx` - Ya instalado

### 8.2 Dependencias Nuevas Requeridas
- `date-fns` - Para helpers de fechas en schemas (validacion de 7 dias)

```bash
# Ejecutar en src/shared/
npm install date-fns
```

### 8.3 Dependencias de Dev (Opcional para Testing)
- `vitest` - Para tests de schemas
- `@testing-library/react` - Para tests de components que usan types

---

## 9. Notas de Implementacion

### 9.1 Breaking Changes

**IMPORTANTE:** Esta actualizacion introduce cambios que rompen compatibilidad:

1. **CAMPANIA_ESTADOS cambio de string a number**
   - Antes: `CAMPANIA_ESTADOS.BORRADOR = "borrador"`
   - Ahora: `CAMPANIA_ESTADOS.BORRADOR = 1`
   - **Accion:** Buscar y reemplazar en web/ y admin/ cualquier uso de estados como strings

2. **Interfaces renombradas**
   - `CampaniaDto` → `Campania`
   - `CreateCampaniaDto` → `CreateCampaniaRequest`
   - `UpdateCampaniaDto` → `UpdateCampaniaRequest`
   - **Accion:** Las versiones antiguas estan marcadas como `@deprecated` pero funcionales

3. **Campos nuevos en Campania**
   - Agregados: `proyectoArtisticoId`, `subtitulo`, `descripcionCorta`, `videoPrincipalUrl`, `imagenPrincipalUrl`, `monedaId`, `importeMinimo`, `tipoFinanciacionId`, `estadoCampaniaId`, `permiteAportacionesAnonimas`, `permitePropinas`, `porcentajeComisionPlataforma`, `fechaPublicacion`, `fechaCierre`
   - Removidos: `descripcion` (reemplazado por `descripcionCorta`), `imagenUrl` (reemplazado por `imagenPrincipalUrl`), `videoUrl` (reemplazado por `videoPrincipalUrl`)
   - **Accion:** Actualizar componentes que usan estos campos

### 9.2 Migracion de Codigo Existente

**Web y Admin deben actualizar:**

1. Importaciones:
```typescript
// Antes
import { CampaniaDto } from "@/shared/types/campania";

// Despues
import { Campania } from "@/shared/types/campania";
```

2. Estados:
```typescript
// Antes
if (campania.estado === "borrador") { }

// Despues
import { CAMPANIA_ESTADOS } from "@/shared/constants";
if (campania.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR) { }
```

3. Schemas:
```typescript
// Antes
import { createCampaniaSchema } from "@/shared/schemas/campania.schema";

// Despues - Usar schemas por step en wizard
import {
  campaniaBasicInfoSchema,
  campaniaFundingSchema,
  campaniaDurationSchema,
  campaniaMediaSchema,
  createCampaniaSchema // Para validacion final
} from "@/shared/schemas/campania.schema";
```

### 9.3 Validacion de Fechas

Los schemas usan `date-fns` para validar que fechaFin sea >= hoy + 7 dias:

```typescript
import { addDays } from "date-fns";

// En schema
.refine(
  (fecha) => {
    const minDate = addDays(new Date(), 7);
    return new Date(fecha) >= minDate;
  },
  { message: "La campania debe durar minimo 7 dias" }
)
```

### 9.4 ErrorCode Mapping

El backend retorna errores como:
```json
{
  "messages": [
    { "message": "El titulo es obligatorio", "errorCode": "1001" }
  ]
}
```

Usar helpers de `error-messages.ts`:
```typescript
import { getErrorMessage, getCampaniaErrorMessage } from "@/shared/utils/error-messages";

// Generico
const msg = getErrorMessage("1001"); // "Este campo es obligatorio"

// Contextual para campanias
const msg = getCampaniaErrorMessage("1001"); // "Completa todos los campos obligatorios para continuar"
```

### 9.5 Wizard de 4 Steps

El wizard debe validar cada step por separado:

```typescript
// Step 1
const step1Form = useForm({
  resolver: zodResolver(campaniaBasicInfoSchema),
});

// Step 2
const step2Form = useForm({
  resolver: zodResolver(campaniaFundingSchema),
});

// Step 3
const step3Form = useForm({
  resolver: zodResolver(campaniaDurationSchema),
});

// Step 4
const step4Form = useForm({
  resolver: zodResolver(campaniaMediaSchema),
});

// Submit final (combina todos los datos)
const finalData = {
  ...step1Form.getValues(),
  ...step2Form.getValues(),
  ...step3Form.getValues(),
  ...step4Form.getValues(),
};

// Validar schema completo antes de enviar
const validation = createCampaniaSchema.safeParse(finalData);
if (!validation.success) {
  // Manejar errores
}
```

---

## 10. Checklist de Implementacion

### 10.1 Types
- [ ] Actualizar `src/shared/types/campania.ts`
- [ ] Agregar interfaces: Campania, CampaniaListItem, CreateCampaniaRequest, UpdateCampaniaRequest, PublishCampaniaResponse
- [ ] Agregar enums: EstadoCampania, TipoFinanciacion
- [ ] Marcar interfaces legacy como `@deprecated`
- [ ] Exportar todo desde `src/shared/types/index.ts`

### 10.2 Schemas
- [ ] Actualizar `src/shared/schemas/campania.schema.ts`
- [ ] Implementar `campaniaBasicInfoSchema`
- [ ] Implementar `campaniaFundingSchema` con refine para importeMinimo
- [ ] Implementar `campaniaDurationSchema` con refine para fechas
- [ ] Implementar `campaniaMediaSchema`
- [ ] Implementar `createCampaniaSchema` (merge de 4 schemas)
- [ ] Implementar `updateCampaniaSchema`
- [ ] Implementar `publishCampaniaSchema`
- [ ] Exportar types inferidos
- [ ] Instalar `date-fns` como dependencia

### 10.3 Constants
- [ ] Actualizar `src/shared/constants/index.ts`
- [ ] Agregar `API_ROUTES.campanias.publicar`
- [ ] Agregar `API_ROUTES.campanias.misCampanias`
- [ ] Agregar `QUERY_KEYS.campanias.misCampanias`
- [ ] Agregar `QUERY_KEYS.campanias.filtered`
- [ ] Cambiar `CAMPANIA_ESTADOS` de string a number
- [ ] Actualizar `CAMPANIA_ESTADOS_LABELS` con keys numericas
- [ ] Agregar `TIPO_FINANCIACION` y `TIPO_FINANCIACION_LABELS`
- [ ] Agregar `MONEDAS`, `MONEDA_SYMBOLS`, `MONEDA_CODES`
- [ ] Agregar `APP_ROUTES.dashboard.campanias.*`
- [ ] Agregar `APP_ROUTES.landing.explorar`
- [ ] Mantener legacy exports con `@deprecated`

### 10.4 Utilities
- [ ] Crear `src/shared/utils/error-messages.ts`
- [ ] Implementar `ERROR_MESSAGES` con todos los codigos
- [ ] Implementar `getErrorMessage()`
- [ ] Implementar `getCampaniaErrorMessage()`
- [ ] Implementar `formatErrorMessages()`
- [ ] Implementar `isSuccessCode()`
- [ ] Implementar `requiresReAuth()`
- [ ] Actualizar `src/shared/utils/format.ts`
- [ ] Implementar `formatCurrencyWithSymbol()`
- [ ] Implementar `getCurrencySymbol()`

### 10.5 Validacion
- [ ] Verificar que no hay errores TypeScript en `src/shared/`
- [ ] Verificar exports en `src/shared/index.ts`
- [ ] Crear tests basicos para schemas (opcional pero recomendado)
- [ ] Documentar breaking changes en CHANGELOG o docs

---

## 11. Siguiente Paso

Una vez completado este plan, los agentes de frontend (Web y Admin) pueden:

1. **Implementar componentes del wizard** usando los schemas por step
2. **Crear API services** usando las constantes de API_ROUTES
3. **Implementar queries y mutations** usando QUERY_KEYS
4. **Manejar errores** usando helpers de error-messages.ts
5. **Formatear montos** usando formatCurrencyWithSymbol()

**Paralelizacion:**
- Backend puede implementar endpoints mientras esto se completa
- Una vez shared/ este listo, Web y Admin pueden trabajar en paralelo

---

**Fin del Plan de Contratos Shared**
