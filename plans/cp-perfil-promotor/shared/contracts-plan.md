# Plan de Contratos Shared: cp-perfil-promotor

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Basado en:** docs/user-stories/cp-perfil-promotor/contracts.md

---

## 1. Resumen

- **Total de types nuevos:** 6 interfaces + 1 union type en archivo nuevo `src/shared/types/crowdpromotion.ts`
- **Total de schemas nuevos:** 2 schemas Zod + helper reutilizable en archivo nuevo `src/shared/schemas/crowdpromotion.schema.ts`
- **Constantes:** 5 bloques de adiciones a `src/shared/constants/index.ts` (QUERY_KEYS, API_ROUTES, APP_ROUTES, TIPO_PROMOTOR, VALIDATION)
- **Utilidades:** 4 adiciones a `src/shared/utils/error-messages.ts` (ERROR_CODE_MESSAGES, PROMOTOR_ERROR_MESSAGES, getPromotorErrorMessage, formatters de comisiones)

### Estado Actual

Esta feature introduce el modulo **Crowdpromotion**, que no tiene ninguna representacion en `src/shared/` actualmente. Todo el codigo compartido de esta feature debe **crearse desde cero** en archivos nuevos, con la excepcion de las constantes y error messages que se agregan a archivos existentes.

| Archivo | Accion |
|---------|--------|
| `src/shared/types/crowdpromotion.ts` | CREAR NUEVO |
| `src/shared/schemas/crowdpromotion.schema.ts` | CREAR NUEVO |
| `src/shared/constants/index.ts` | MODIFICAR (agregar bloque `crowdpromotion`) |
| `src/shared/utils/error-messages.ts` | MODIFICAR (agregar bloque promotor) |
| `src/shared/types/index.ts` | MODIFICAR (agregar export) |
| `src/shared/schemas/index.ts` | MODIFICAR (agregar export) |

---

## 2. Types (`src/shared/types/crowdpromotion.ts`)

Archivo nuevo. Contiene todas las interfaces para el modulo Crowdpromotion.

### 2.1 DTOs de Response

| Tipo | Propiedades Clave | Descripcion |
|------|-------------------|-------------|
| `PromotorCreatedResult` | `id`, `nombrePublico`, `tipoPromotorNombre`, `esActivo`, `fechaCreacion` | Response del `POST /api/crowdpromotion/promotor`. Resultado minimo tras la creacion. |
| `Promotor` | `id`, `nombrePublico`, `tipoPromotorId`, `tipoPromotorNombre`, `emailContacto\|null`, `urlSitioWeb\|null`, `urlInstagram\|null`, `urlTikTok\|null`, `urlYouTube\|null`, `urlTwitter\|null`, `esActivo`, `fechaCreacion`, `totalProgramasActivos`, `totalComisionesGanadas`, `monedaComisiones` | Response del `GET /api/crowdpromotion/promotor/me`. Perfil completo con campos calculados. |
| `PromotorUpdatedResult` | `id`, `nombrePublico`, `fechaActualizacion` | Response del `PUT /api/crowdpromotion/promotor/me`. Confirmacion de actualizacion. |
| `PromotorDesactivadoResult` | `id`, `esActivo`, `programasDadosDeBaja` | Response del `PATCH /api/crowdpromotion/promotor/me/desactivar`. |
| `TipoPromotor` | `id`, `nombre`, `descripcion` | Maestra de tipos. Usada para el select en el formulario de registro. |

**Notas importantes sobre nullable vs optional:**
- Los campos de URL y emailContacto son `string | null` en `Promotor` (response de lectura, el backend puede devolver null explicitamente).
- Los campos de URL y emailContacto son `string | undefined` (`?:`) en los tipos de Request (el cliente puede omitirlos).

### 2.2 DTOs de Request

| Tipo | Propiedades Clave | Descripcion |
|------|-------------------|-------------|
| `CreatePromotorRequest` | `nombrePublico` (requerido), `tipoPromotorId` (requerido), `emailContacto?`, `urlSitioWeb?`, `urlInstagram?`, `urlTikTok?`, `urlYouTube?`, `urlTwitter?` | Body del `POST /api/crowdpromotion/promotor`. `tipoPromotorId` solo en creacion; no editable despues. |
| `UpdatePromotorRequest` | `nombrePublico` (requerido), `emailContacto?`, `urlSitioWeb?`, `urlInstagram?`, `urlTikTok?`, `urlYouTube?`, `urlTwitter?` | Body del `PUT /api/crowdpromotion/promotor/me`. NO incluye `tipoPromotorId`. |

### 2.3 Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `PromotorEstado` | `'activo' \| 'inactivo'` | Representacion del campo `esActivo` como string legible. Util en badges de UI. |

### 2.4 Estructura completa del archivo

```typescript
// src/shared/types/crowdpromotion.ts

// ========== DTOs de Request ==========

export interface CreatePromotorRequest {
    nombrePublico: string;       // min 3, max 200 chars
    tipoPromotorId: number;      // 1-4, debe existir en MaestraTipoPromotor
    emailContacto?: string;      // opcional, formato email valido, max 200 chars
    urlSitioWeb?: string;        // opcional, formato URL valido, max 300 chars
    urlInstagram?: string;       // opcional, formato URL valido, max 300 chars
    urlTikTok?: string;          // opcional, formato URL valido, max 300 chars
    urlYouTube?: string;         // opcional, formato URL valido, max 300 chars
    urlTwitter?: string;         // opcional, formato URL valido, max 300 chars
}

export interface UpdatePromotorRequest {
    nombrePublico: string;       // min 3, max 200 chars
    emailContacto?: string;      // opcional, formato email valido, max 200 chars
    urlSitioWeb?: string;        // opcional, formato URL valido, max 300 chars
    urlInstagram?: string;       // opcional, formato URL valido, max 300 chars
    urlTikTok?: string;          // opcional, formato URL valido, max 300 chars
    urlYouTube?: string;         // opcional, formato URL valido, max 300 chars
    urlTwitter?: string;         // opcional, formato URL valido, max 300 chars
}

// ========== DTOs de Response ==========

export interface PromotorCreatedResult {
    id: string;                   // Guid como string
    nombrePublico: string;
    tipoPromotorNombre: string;
    esActivo: boolean;
    fechaCreacion: string;        // ISO 8601 datetime string
}

export interface Promotor {
    id: string;                   // Guid como string
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
    fechaCreacion: string;        // ISO 8601 datetime string
    totalProgramasActivos: number;
    totalComisionesGanadas: number;
    monedaComisiones: string;     // Siempre "EUR" en MVP
}

export interface PromotorUpdatedResult {
    id: string;
    nombrePublico: string;
    fechaActualizacion: string;   // ISO 8601 datetime string
}

export interface PromotorDesactivadoResult {
    id: string;
    esActivo: boolean;
    programasDadosDeBaja: number;
}

// ========== Maestras ==========

export interface TipoPromotor {
    id: number;
    nombre: string;
    descripcion: string;
}

// ========== Union Types ==========

export type PromotorEstado = 'activo' | 'inactivo';
```

---

## 3. Schemas Zod (`src/shared/schemas/crowdpromotion.schema.ts`)

Archivo nuevo. Contiene los schemas de validacion para los formularios de registro y edicion del perfil de promotor.

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas clave |
|--------|--------|--------------|
| `createPromotorSchema` | `nombrePublico`, `tipoPromotorId`, `emailContacto?`, `urlSitioWeb?`, `urlInstagram?`, `urlTikTok?`, `urlYouTube?`, `urlTwitter?` | nombrePublico: required + min(3) + max(200); tipoPromotorId: required int >= 1; email: .email().optional(); URLs: .url().optional() con helper `urlOpcionalSchema` |
| `updatePromotorSchema` | `nombrePublico`, `emailContacto?`, `urlSitioWeb?`, `urlInstagram?`, `urlTikTok?`, `urlYouTube?`, `urlTwitter?` | Igual que create pero sin `tipoPromotorId` |

### 3.2 Helper interno `urlOpcionalSchema`

Este helper es una funcion interna del archivo (no exportada) que evita la duplicacion de la logica de validacion de URL opcional con string vacio permitido.

```typescript
const urlOpcionalSchema = (nombreCampo: string) =>
    z
        .string()
        .url(`La URL de ${nombreCampo} no tiene formato valido`)
        .max(300, 'Maximo 300 caracteres')
        .optional()
        .or(z.literal(''));
```

**Razon del `.or(z.literal(''))` :** Los campos de URL son strings en el formulario HTML. Un campo vacio devuelve `''`, no `undefined`. Este patron permite que el campo este vacio sin disparar el error de formato URL, manteniendo la compatibilidad con React Hook Form. El consumer del formulario debe convertir `''` a `undefined`/`null` antes de enviar al backend (ver seccion 5.3).

### 3.3 Estructura completa del archivo

```typescript
// src/shared/schemas/crowdpromotion.schema.ts
import { z } from 'zod';

// Helper interno: no exportar
const urlOpcionalSchema = (nombreCampo: string) =>
    z
        .string()
        .url(`La URL de ${nombreCampo} no tiene formato valido`)
        .max(300, 'Maximo 300 caracteres')
        .optional()
        .or(z.literal(''));

// ========== Schema de Creacion ==========

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

    urlSitioWeb: urlOpcionalSchema('sitio web'),
    urlInstagram: urlOpcionalSchema('Instagram'),
    urlTikTok: urlOpcionalSchema('TikTok'),
    urlYouTube: urlOpcionalSchema('YouTube'),
    urlTwitter: urlOpcionalSchema('Twitter/X'),
});

// ========== Schema de Edicion ==========

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

    urlSitioWeb: urlOpcionalSchema('sitio web'),
    urlInstagram: urlOpcionalSchema('Instagram'),
    urlTikTok: urlOpcionalSchema('TikTok'),
    urlYouTube: urlOpcionalSchema('YouTube'),
    urlTwitter: urlOpcionalSchema('Twitter/X'),
});

// ========== Types Inferidos ==========

export type CreatePromotorFormData = z.infer<typeof createPromotorSchema>;
export type UpdatePromotorFormData = z.infer<typeof updatePromotorSchema>;
```

### 3.4 Types Inferidos

| Type Inferido | Schema de origen | Uso |
|---------------|-----------------|-----|
| `CreatePromotorFormData` | `createPromotorSchema` | Tipo del `useForm<CreatePromotorFormData>` en el formulario de registro |
| `UpdatePromotorFormData` | `updatePromotorSchema` | Tipo del `useForm<UpdatePromotorFormData>` en el formulario de edicion |

**Diferencia entre `CreatePromotorFormData` y `CreatePromotorRequest`:**
- `CreatePromotorFormData` es el estado del formulario (puede tener strings vacios `''`).
- `CreatePromotorRequest` es el payload que se envia al backend (sin strings vacios, convertidos a `undefined`).
- El hook `useCreatePromotor` es responsable de la conversion entre ambos (ver seccion 5.3).

---

## 4. Constantes (`src/shared/constants/index.ts`)

Se agregan bloques al archivo existente. No se modifica ningun bloque existente.

### 4.1 QUERY_KEYS - Nuevo bloque `crowdpromotion`

Agregar al objeto `QUERY_KEYS` existente (despues del bloque `crowdsourcing`, antes del bloque `dashboard`):

```typescript
// En QUERY_KEYS, agregar:
crowdpromotion: {
    promotor: {
        me: ['crowdpromotion', 'promotor', 'me'] as const,
    },
    maestras: {
        tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
    },
},
```

| Key | Patron | Endpoint Asociado | Uso |
|-----|--------|-------------------|-----|
| `QUERY_KEYS.crowdpromotion.promotor.me` | `['crowdpromotion', 'promotor', 'me']` | `GET /api/crowdpromotion/promotor/me` | Hook `usePromotor()`. Invalidar tras create, update, desactivar. |
| `QUERY_KEYS.crowdpromotion.maestras.tiposPromotor` | `['crowdpromotion', 'maestras', 'tipos-promotor']` | `GET /api/crowdpromotion/maestras/tipos-promotor` | Hook `useTiposPromotor()`. En MVP puede no usarse (valores fijos via constante). |

### 4.2 API_ROUTES - Nuevo bloque `crowdpromotion`

Agregar al objeto `API_ROUTES` existente (despues del bloque `crowdsourcing`):

```typescript
// En API_ROUTES, agregar:
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
```

| Constante | Valor | Metodo HTTP | Uso |
|-----------|-------|-------------|-----|
| `API_ROUTES.crowdpromotion.promotor.base` | `/api/crowdpromotion/promotor` | POST | Crear perfil de promotor |
| `API_ROUTES.crowdpromotion.promotor.me` | `/api/crowdpromotion/promotor/me` | GET / PUT | Leer y actualizar perfil |
| `API_ROUTES.crowdpromotion.promotor.desactivar` | `/api/crowdpromotion/promotor/me/desactivar` | PATCH | Desactivar perfil |
| `API_ROUTES.crowdpromotion.maestras.tiposPromotor` | `/api/crowdpromotion/maestras/tipos-promotor` | GET | Listar tipos de promotor (futuro) |

### 4.3 APP_ROUTES - Agregar `promotor` dentro de `landing`

Agregar dentro de `APP_ROUTES.landing` (despues de `crowdsourcing`):

```typescript
// En APP_ROUTES.landing, agregar:
promotor: {
    registro: '/promotor/registro',
    dashboard: '/promotor/dashboard',
    perfil: '/promotor/perfil',
},
```

| Ruta | Autenticacion | Redirect si no auth | Notas |
|------|---------------|---------------------|-------|
| `/promotor/registro` | Bearer JWT requerido | `/auth/login` | Si ya tiene perfil (activo o inactivo) -> redirigir a `/promotor/dashboard` |
| `/promotor/dashboard` | Bearer JWT requerido | `/auth/login` | Requiere perfil de promotor existente |
| `/promotor/perfil` | Bearer JWT requerido | `/auth/login` | Requiere perfil de promotor existente |

### 4.4 TIPO_PROMOTOR - Nuevas constantes de dominio

Agregar al final del archivo, despues del bloque de mensajeria existente:

```typescript
// ========== Crowdpromotion - Tipo de Promotor ==========
// Valores de seed de MaestraTipoPromotor. IDs fijos en MVP.

export const TIPO_PROMOTOR = {
    FAN_EMBAJADOR:           1,
    INFLUENCER:              2,
    MEDIO_BLOG:              3,
    PROFESIONAL_MARKETING:   4,
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

| Constante | Tipo | Uso |
|-----------|------|-----|
| `TIPO_PROMOTOR` | `{ FAN_EMBAJADOR: 1, INFLUENCER: 2, MEDIO_BLOG: 3, PROFESIONAL_MARKETING: 4 }` | Referencia semantica al ID de tipo en formularios y condiciones |
| `TIPO_PROMOTOR_LABELS` | `Record<number, string>` | Texto visible en el select del formulario de registro y en badges de perfil |
| `TIPO_PROMOTOR_DESCRIPTIONS` | `Record<number, string>` | Descripcion visible en el selector de tipo (tooltip o subtitulo) |

### 4.5 VALIDATION - Agregar limites de promotor

Agregar dentro del objeto `VALIDATION` existente los nuevos limites:

```typescript
// En VALIDATION, agregar (despues de los limites de Artista):
// Promotor
NOMBRE_PUBLICO_MIN: 3,
NOMBRE_PUBLICO_MAX: 200,
EMAIL_CONTACTO_MAX: 200,
URL_PROMOTOR_MAX: 300,
```

| Constante | Valor | Campo asociado |
|-----------|-------|----------------|
| `VALIDATION.NOMBRE_PUBLICO_MIN` | `3` | `nombrePublico` en create y update |
| `VALIDATION.NOMBRE_PUBLICO_MAX` | `200` | `nombrePublico` en create y update |
| `VALIDATION.EMAIL_CONTACTO_MAX` | `200` | `emailContacto` en create y update |
| `VALIDATION.URL_PROMOTOR_MAX` | `300` | Todos los campos `url*` en create y update |

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

Se agregan bloques al archivo existente. No se modifica ninguna funcion ni constante existente.

### 5.1 Nuevos codigos en `ERROR_CODE_MESSAGES`

Agregar las tres claves nuevas al objeto `ERROR_CODE_MESSAGES` existente:

```typescript
// En ERROR_CODE_MESSAGES, agregar en sus secciones correspondientes:

// Not Found errors (2000-2999) - despues de "2014"
"2015": "No tienes un perfil de promotor. Registrate primero.",

// Business Rule errors (4000-4999) - despues de "4016"
"4018": "Ya tienes un perfil de promotor creado.",
"4019": "Tu perfil de promotor ya esta desactivado.",
```

**Nota sobre continuidad de codigos:**
- `2015` continua la secuencia del modulo Crowdsourcing (que llega hasta `2014`).
- `4018` y `4019` continuan desde `4017` (ultimo codigo business rule de Crowdsourcing).
- Esto es coherente con la justificacion documentada en `contracts.md`.

### 5.2 Nuevo bloque `PROMOTOR_ERROR_MESSAGES`

Agregar al final del archivo, siguiendo el patron de los bloques existentes (`BACKING_ERROR_MESSAGES`, `CROWDSOURCING_ERROR_MESSAGES`, etc.):

```typescript
// ========== Crowdpromotion - Perfil de Promotor Error Messages ==========

export const PROMOTOR_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for promotor context)
    '1001': 'Completa los campos obligatorios: nombre publico y tipo de promotor',
    '1002': 'El valor supera el maximo de caracteres permitido',
    '1003': 'El email de contacto no tiene formato valido',
    '1010': 'El tipo de promotor seleccionado no existe',
    '1011': 'El nombre debe tener al menos 3 caracteres',
    '1013': 'La URL indicada no tiene formato valido. Usa una URL completa (ej: https://...)',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
    '4018': 'Ya tienes un perfil de promotor creado.',
    '4019': 'Tu perfil de promotor ya esta desactivado.',
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',

    // Semantic keys para uso interno en hooks y componentes
    PROMOTOR_NOT_FOUND:          'No tienes un perfil de promotor. Registrate primero.',
    PROMOTOR_ALREADY_EXISTS:     'Ya tienes un perfil de promotor creado.',
    PROMOTOR_ALREADY_INACTIVE:   'Tu perfil de promotor ya esta desactivado.',
    PROMOTOR_TIPO_NOT_FOUND:     'El tipo de promotor seleccionado no existe.',
    PROMOTOR_NOMBRE_REQUIRED:    'El nombre publico es obligatorio.',
    PROMOTOR_NOMBRE_MIN:         'El nombre debe tener al menos 3 caracteres.',
    PROMOTOR_NOMBRE_MAX:         'El nombre publico no puede superar los 200 caracteres.',
    PROMOTOR_EMAIL_INVALID:      'El email de contacto no tiene un formato valido.',
    PROMOTOR_URL_INVALID:        'La URL indicada no tiene un formato valido.',
} as const;

export const getPromotorErrorMessage = (errorCode: string): string => {
    return PROMOTOR_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### 5.3 Mappers en `src/shared/utils/mappers.ts`

Agregar al final del archivo `mappers.ts` existente. Estos mappers resuelven la conversion entre datos del formulario y el payload del API.

```typescript
// ========== Crowdpromotion - Promotor Mappers ==========

import type {
    CreatePromotorRequest,
    UpdatePromotorRequest,
    Promotor,
    PromotorEstado,
} from '../types/crowdpromotion';
import type {
    CreatePromotorFormData,
    UpdatePromotorFormData,
} from '../schemas/crowdpromotion.schema';

/**
 * Convierte datos del formulario de creacion al payload del API.
 * Los strings vacios ('') se convierten a undefined para no enviar
 * campos opcionales al backend y evitar errores de validacion de URL/email.
 */
export function mapCreateFormToRequest(
    formData: CreatePromotorFormData
): CreatePromotorRequest {
    return {
        nombrePublico: formData.nombrePublico,
        tipoPromotorId: formData.tipoPromotorId,
        emailContacto: formData.emailContacto || undefined,
        urlSitioWeb: formData.urlSitioWeb || undefined,
        urlInstagram: formData.urlInstagram || undefined,
        urlTikTok: formData.urlTikTok || undefined,
        urlYouTube: formData.urlYouTube || undefined,
        urlTwitter: formData.urlTwitter || undefined,
    };
}

/**
 * Convierte datos del formulario de edicion al payload del API.
 * Misma logica de conversion de strings vacios que en creacion.
 */
export function mapUpdateFormToRequest(
    formData: UpdatePromotorFormData
): UpdatePromotorRequest {
    return {
        nombrePublico: formData.nombrePublico,
        emailContacto: formData.emailContacto || undefined,
        urlSitioWeb: formData.urlSitioWeb || undefined,
        urlInstagram: formData.urlInstagram || undefined,
        urlTikTok: formData.urlTikTok || undefined,
        urlYouTube: formData.urlYouTube || undefined,
        urlTwitter: formData.urlTwitter || undefined,
    };
}

/**
 * Convierte el perfil completo del promotor a los valores iniciales
 * del formulario de edicion. Los null del backend se convierten a ''
 * para que React Hook Form pueda controlar los inputs como strings.
 */
export function mapPromotorToUpdateForm(
    promotor: Promotor
): UpdatePromotorFormData {
    return {
        nombrePublico: promotor.nombrePublico,
        emailContacto: promotor.emailContacto ?? '',
        urlSitioWeb: promotor.urlSitioWeb ?? '',
        urlInstagram: promotor.urlInstagram ?? '',
        urlTikTok: promotor.urlTikTok ?? '',
        urlYouTube: promotor.urlYouTube ?? '',
        urlTwitter: promotor.urlTwitter ?? '',
    };
}

/**
 * Devuelve el estado del promotor como string legible.
 */
export function getPromotorEstado(esActivo: boolean): PromotorEstado {
    return esActivo ? 'activo' : 'inactivo';
}
```

### 5.4 Formatter en `src/shared/utils/format.ts`

Agregar al final del archivo `format.ts` existente. Formatters especificos para datos de promotor:

```typescript
// ========== Crowdpromotion - Promotor Formatters ==========

/**
 * Formatea el total de comisiones ganadas con la moneda indicada.
 * @param total - Importe de comisiones
 * @param monedaCodigo - Codigo ISO de la moneda (ej: "EUR")
 * @returns String formateado: "€ 150,50" o "0,00 €"
 */
export function formatComisionesGanadas(
    total: number,
    monedaCodigo: string = 'EUR'
): string {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: monedaCodigo,
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(total);
}
```

---

## 6. Archivos a Crear y Modificar

```
src/shared/
├── types/
│   ├── crowdpromotion.ts       <-- CREAR NUEVO (interfaces Promotor, TipoPromotor, etc.)
│   └── index.ts                <-- MODIFICAR (agregar export)
├── schemas/
│   ├── crowdpromotion.schema.ts <-- CREAR NUEVO (createPromotorSchema, updatePromotorSchema)
│   └── index.ts                 <-- MODIFICAR (agregar export)
├── constants/
│   └── index.ts                <-- MODIFICAR (5 adiciones: QUERY_KEYS, API_ROUTES, APP_ROUTES,
│                                              TIPO_PROMOTOR, VALIDATION)
└── utils/
    ├── error-messages.ts       <-- MODIFICAR (3 adiciones en ERROR_CODE_MESSAGES,
    │                                          nuevo bloque PROMOTOR_ERROR_MESSAGES,
    │                                          nueva funcion getPromotorErrorMessage)
    ├── mappers.ts              <-- MODIFICAR (4 nuevas funciones: mapCreateFormToRequest,
    │                                          mapUpdateFormToRequest, mapPromotorToUpdateForm,
    │                                          getPromotorEstado)
    └── format.ts               <-- MODIFICAR (1 nueva funcion: formatComisionesGanadas)
```

### 6.1 Modificaciones a `src/shared/types/index.ts`

Agregar al final del archivo:

```typescript
export * from "./crowdpromotion"
```

### 6.2 Modificaciones a `src/shared/schemas/index.ts`

Agregar al final del archivo:

```typescript
export * from "./crowdpromotion.schema"
```

---

## 7. Tabla de Alineamiento Backend-Frontend

### 7.1 DTOs

| Tipo TypeScript | DTO C# Equivalente | Alineamiento |
|----------------|--------------------|--------------|
| `CreatePromotorRequest` | `CreatePromotorRequestDto` | 100% alineado |
| `UpdatePromotorRequest` | `UpdatePromotorRequestDto` | 100% alineado (sin tipoPromotorId) |
| `PromotorCreatedResult` | `PromotorCreatedResultDto` | 100% alineado |
| `Promotor` | `PromotorDto` | 100% alineado |
| `PromotorUpdatedResult` | `PromotorUpdatedResultDto` | 100% alineado |
| `PromotorDesactivadoResult` | `PromotorDesactivadoResultDto` | 100% alineado |
| `TipoPromotor` | Proyeccion de `MaestraTipoPromotor` | 100% alineado |

### 7.2 Validaciones

| Campo | Regla Backend | Regla Zod | Alineamiento |
|-------|--------------|-----------|--------------|
| `nombrePublico` | `NotEmpty` + `MinLength(3)` + `MaxLength(200)` | `.min(1).min(3).max(200)` | 100% |
| `tipoPromotorId` | `NotEmpty` + `MustAsync(ExistInMaestra)` | `.int().min(1)` (DB validado por select) | 100% |
| `emailContacto` | `EmailAddress().When(present)` + `MaxLength(200)` | `.email().max(200).optional().or(literal(''))` | 100% |
| `urlSitioWeb` | `Must(BeAValidUrl).When(present)` + `MaxLength(300)` | `.url().max(300).optional().or(literal(''))` | 100% |
| `urlInstagram` | `Must(BeAValidUrl).When(present)` + `MaxLength(300)` | `.url().max(300).optional().or(literal(''))` | 100% |
| `urlTikTok` | `Must(BeAValidUrl).When(present)` + `MaxLength(300)` | `.url().max(300).optional().or(literal(''))` | 100% |
| `urlYouTube` | `Must(BeAValidUrl).When(present)` + `MaxLength(300)` | `.url().max(300).optional().or(literal(''))` | 100% |
| `urlTwitter` | `Must(BeAValidUrl).When(present)` + `MaxLength(300)` | `.url().max(300).optional().or(literal(''))` | 100% |

### 7.3 Codigos de Error

| ErrorCode | Constante Backend | Mensaje Frontend | En ERROR_CODE_MESSAGES | En PROMOTOR_ERROR_MESSAGES |
|-----------|------------------|------------------|------------------------|---------------------------|
| `1001` | `Validation_Required` | "Este campo es obligatorio" | Ya existe (global) | Si (contextualizado) |
| `1002` | `Validation_MaxLength` | "El valor supera el maximo..." | Ya existe (global) | Si |
| `1003` | `Validation_InvalidEmail` | "El email de contacto no tiene formato valido" | Ya existe (global como `1004`) | Si |
| `1010` | `Validation_ForeignKeyNotFound` | "La referencia proporcionada no existe" | Ya existe (global) | Si |
| `1011` | `Validation_MinLength` | "El campo no cumple la longitud minima requerida" | Ya existe (global) | Si |
| `1013` | `Validation_InvalidUrl` | "La URL proporcionada no es valida" | Ya existe (global) | Si |
| `2015` | `NotFound_Promotor` | "No tienes un perfil de promotor. Registrate primero." | NUEVO | Si |
| `3001` | `Auth_Unauthorized` | "No autorizado. Por favor, inicia sesion nuevamente" | Ya existe (global) | Si |
| `4018` | `BusinessRule_PromotorAlreadyExists` | "Ya tienes un perfil de promotor creado." | NUEVO | Si |
| `4019` | `BusinessRule_PromotorAlreadyInactive` | "Tu perfil de promotor ya esta desactivado." | NUEVO | Si |
| `5000` | `Internal_UnexpectedError` | "Ha ocurrido un error inesperado..." | Ya existe (global) | Si |

---

## 8. Orden de Implementacion

El orden importa porque hay dependencias entre archivos:

1. **Paso 1 - Tipos** (sin dependencias): Crear `src/shared/types/crowdpromotion.ts`
2. **Paso 2 - Schemas** (depende de tipos para inferencia): Crear `src/shared/schemas/crowdpromotion.schema.ts`
3. **Paso 3 - Constantes** (sin dependencias): Modificar `src/shared/constants/index.ts`
4. **Paso 4 - Error messages** (depende de funcion `getErrorMessage` ya existente): Modificar `src/shared/utils/error-messages.ts`
5. **Paso 5 - Mappers** (depende de tipos y schemas): Modificar `src/shared/utils/mappers.ts`
6. **Paso 6 - Formatters** (sin dependencias): Modificar `src/shared/utils/format.ts`
7. **Paso 7 - Barrel exports**: Modificar `src/shared/types/index.ts` y `src/shared/schemas/index.ts`

---

## 9. Dependencias

- `zod` - Ya instalado en `src/shared/node_modules/`
- Ningun package adicional requerido
- No hay dependencias circulares: `types` <- `schemas` <- `mappers` (unidireccional)

---

## 10. Notas de Implementacion

### 10.1 Patron de nombres URL en formulario

Los campos de URL en el formulario son controlled inputs de tipo `string`. El patron `.optional().or(z.literal(''))` en Zod es necesario porque:
- React Hook Form registra strings vacios `''`, no `undefined`, cuando el usuario no escribe nada.
- Sin `.or(z.literal(''))`, Zod intenta validar `''` como URL y falla.
- El mapper `mapCreateFormToRequest` convierte `'' -> undefined` antes de enviar al API.

### 10.2 `tipoPromotorId` en el formulario

React Hook Form con un `<select>` devuelve strings, no numbers. El campo `tipoPromotorId` debe ser coercionado a number. Usar `z.coerce.number()` o manejar la conversion en el `setValue` del formulario. El schema actual usa `.number()` directo, lo que requiere que el componente `<select>` use `valueAsNumber` en el registro de RHF.

Alternativa recomendada para el schema:
```typescript
tipoPromotorId: z.coerce
    .number({ invalid_type_error: 'El tipo de promotor es obligatorio' })
    .int()
    .min(1, 'El tipo de promotor es obligatorio'),
```

Esto simplifica el componente al no requerir `valueAsNumber`.

### 10.3 Guard de ruta en `/promotor/registro`

El guard de la ruta `/promotor/registro` usa `QUERY_KEYS.crowdpromotion.promotor.me` con `retry: false` para detectar si el usuario ya tiene perfil. El comportamiento es:
- Response 200 con datos -> ya tiene perfil -> redirigir a `APP_ROUTES.landing.promotor.dashboard`
- Response 404 (errorCode `2015`) -> no tiene perfil -> mostrar formulario de registro
- Response 401 -> no autenticado -> redirigir a `APP_ROUTES.auth.login`

### 10.4 Invalidacion de cache tras mutaciones

Todos los hooks de mutacion deben invalidar `QUERY_KEYS.crowdpromotion.promotor.me` en `onSuccess`:
- `useCreatePromotor`: invalidar + redirigir a `/promotor/dashboard`
- `useUpdatePromotor`: invalidar (no redirige)
- `useDesactivarPromotor`: invalidar + redirigir a landing home

### 10.5 Valores fijos de tipos de promotor en MVP

En el MVP, las opciones del select de `tipoPromotorId` se construyen desde `TIPO_PROMOTOR_LABELS` (constante local, sin llamada a API). Esto evita una dependencia en el endpoint de maestras durante el registro. Si en el futuro los tipos son dinamicos, usar `QUERY_KEYS.crowdpromotion.maestras.tiposPromotor` con `API_ROUTES.crowdpromotion.maestras.tiposPromotor`.

### 10.6 `monedaComisiones` en el perfil

El campo `monedaComisiones` en `Promotor` siempre es `"EUR"` en el MVP. No se necesita ninguna constante especial para este campo; usar `MONEDA_CODES[MONEDAS.EUR]` del modulo existente si se necesita comparar.

---

## 11. Checklist de Implementacion

### Archivos nuevos
- [ ] `src/shared/types/crowdpromotion.ts` creado con las 6 interfaces y 1 union type
- [ ] `src/shared/schemas/crowdpromotion.schema.ts` creado con los 2 schemas y helper interno
- [ ] Types exportados desde `src/shared/types/index.ts`
- [ ] Schemas exportados desde `src/shared/schemas/index.ts`

### Constantes (constants/index.ts)
- [ ] `QUERY_KEYS.crowdpromotion` agregado con `promotor.me` y `maestras.tiposPromotor`
- [ ] `API_ROUTES.crowdpromotion` agregado con 4 rutas
- [ ] `APP_ROUTES.landing.promotor` agregado con 3 rutas
- [ ] `TIPO_PROMOTOR` con 4 valores seed agregado
- [ ] `TIPO_PROMOTOR_LABELS` con 4 etiquetas agregado
- [ ] `TIPO_PROMOTOR_DESCRIPTIONS` con 4 descripciones agregado
- [ ] `VALIDATION.NOMBRE_PUBLICO_MIN`, `VALIDATION.NOMBRE_PUBLICO_MAX`, `VALIDATION.EMAIL_CONTACTO_MAX`, `VALIDATION.URL_PROMOTOR_MAX` agregados

### Utilidades
- [ ] Codigos `2015`, `4018`, `4019` agregados a `ERROR_CODE_MESSAGES`
- [ ] Bloque `PROMOTOR_ERROR_MESSAGES` agregado a `error-messages.ts`
- [ ] Funcion `getPromotorErrorMessage` agregada a `error-messages.ts`
- [ ] Funcion `mapCreateFormToRequest` agregada a `mappers.ts`
- [ ] Funcion `mapUpdateFormToRequest` agregada a `mappers.ts`
- [ ] Funcion `mapPromotorToUpdateForm` agregada a `mappers.ts`
- [ ] Funcion `getPromotorEstado` agregada a `mappers.ts`
- [ ] Funcion `formatComisionesGanadas` agregada a `format.ts`

### Validaciones finales
- [ ] Schemas Zod con mensajes en espanol sin tildes (consistente con el resto del proyecto)
- [ ] Endpoints en `API_ROUTES` 100% alineados con `contracts.md`
- [ ] Query keys consistentes con patron jerarquico del proyecto (`['crowdpromotion', ...]`)
- [ ] Ningun `any` en los tipos definidos
- [ ] Mappers cubren los edge cases de strings vacios y null del backend
