# Plan de Contratos Shared: cs-valoraciones

**Fecha:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)
**Basado en:** docs/user-stories/cs-valoraciones/contracts.md

---

## 1. Resumen

- Total de types: 6 interfaces nuevas + 1 type union nuevo
- Total de schemas: 2 schemas Zod nuevos
- Types inferidos: 2 nuevos (`CreateValoracionFormData`, `ValoracionesQueryParams`)
- Constantes nuevas: 2 entradas en `QUERY_KEYS`, 2 entradas en `API_ROUTES`
- Mensajes de error nuevos: 2 codigos numericos + 4 keys semanticos + 1 funcion getter
- Constantes de dominio: ninguna adicional (se reutilizan `ESTADO_ACUERDO` ya existentes)

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

**Tipo de cambio:** MODIFICACION de archivo existente. Agregar al final del archivo.

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `ValoracionCreatedResult` | `id: string`, `puntuacion: number`, `comentario?: string`, `fechaCreacion: string` | Resultado del POST crear valoracion. `fechaCreacion` es ISO 8601 string. |
| `ValoracionResumen` | `puntuacionMedia: number \| null`, `totalValoraciones: number`, `distribucion: { 5: number; 4: number; 3: number; 2: number; 1: number }` | Resumen calculado de valoraciones recibidas por un usuario. `puntuacionMedia` es null cuando `totalValoraciones === 0`. |
| `ValoracionListItem` | `id: string`, `puntuacion: number`, `comentario?: string`, `autorNombre: string`, `autorImagenUrl: string \| null`, `acuerdoTituloInterno: string`, `fechaCreacion: string` | Item individual de valoracion recibida. `autorImagenUrl` es `null` explicitamente (no `undefined`) porque el backend puede devolver null. |
| `ValoracionesUsuario` | `resumen: ValoracionResumen`, `valoraciones: PaginatedResult<ValoracionListItem>` | Respuesta completa del GET valoraciones por usuario. Combina resumen + listado paginado. |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreateValoracionRequest` | `puntuacion: number`, `comentario?: string` | Body del POST crear valoracion. `puntuacion` es entero 1-5 inclusive. `comentario` max 1000 chars. |

### 2.3 Tipos Genericos

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `PaginatedResult<T>` | `items: T[]`, `totalCount: number`, `page: number`, `pageSize: number` | Wrapper generico de paginacion. Verificar si ya existe en el archivo antes de agregar; si no existe, agregarlo. A fecha del plan, NO existe en `src/shared/types/crowdsourcing.ts`. |

### 2.4 Estructura completa de las nuevas interfaces

Agregar despues de la seccion `// ========== Mensajeria Crowdsourcing (US-CS-05) ==========` al final del archivo:

```typescript
// ========== Valoraciones Bidireccionales (US-CS-06) ==========

// --- DTOs de Request ---

export interface CreateValoracionRequest {
    puntuacion: number;       // entero, 1-5 inclusive
    comentario?: string;      // max 1000 chars
}

// --- DTOs de Response (POST result) ---

export interface ValoracionCreatedResult {
    id: string;
    puntuacion: number;
    comentario?: string;
    fechaCreacion: string;    // ISO 8601 datetime string
}

// --- DTOs de Response (GET resumen + lista) ---

export interface ValoracionResumen {
    puntuacionMedia: number | null;   // null si totalValoraciones === 0; 1 decimal
    totalValoraciones: number;
    distribucion: {
        5: number;
        4: number;
        3: number;
        2: number;
        1: number;
    };
}

export interface ValoracionListItem {
    id: string;
    puntuacion: number;
    comentario?: string;
    autorNombre: string;
    autorImagenUrl: string | null;    // null explicito, no undefined
    acuerdoTituloInterno: string;
    fechaCreacion: string;            // ISO 8601 datetime string
}

export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface ValoracionesUsuario {
    resumen: ValoracionResumen;
    valoraciones: PaginatedResult<ValoracionListItem>;
}
```

**Nota critica:** `PaginatedResult<T>` es un tipo generico compartido. Si en el futuro se reutiliza para otros modulos (mensajeria, etc.), se recomienda moverlo a una seccion `// ========== Tipos Genericos Compartidos ==========` al inicio del archivo. Por ahora, agregarlo en la seccion de valoraciones.

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

**Tipo de cambio:** MODIFICACION de archivo existente. Agregar al final del archivo, despues de la seccion de mensajeria.

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas de Validacion |
|--------|--------|----------------------|
| `createValoracionSchema` | `puntuacion` (required), `comentario` (optional) | `puntuacion`: number, required_error en espanol, invalid_type_error en espanol, `.int()`, `.gte(1)`, `.lte(5)`. `comentario`: string, `.max(1000)`, `.optional()` |
| `valoracionesQuerySchema` | `page` (optional), `pageSize` (optional) | `page`: number, int, `.gte(1)`, `.optional()`, `.default(1)`. `pageSize`: number, int, `.gte(1)`, `.lte(50)`, `.optional()`, `.default(10)` |

### 3.2 Estructura completa de los nuevos schemas

Agregar despues de `// ========== Types Inferidos - Mensajeria ==========` al final del archivo:

```typescript
// ========== Schemas para Valoraciones Bidireccionales (US-CS-06) ==========

export const createValoracionSchema = z.object({
    puntuacion: z
        .number({
            required_error: 'La puntuacion es obligatoria',
            invalid_type_error: 'La puntuacion debe ser un numero',
        })
        .int('La puntuacion debe ser un numero entero')
        .gte(1, 'Minimo 1 estrella')
        .lte(5, 'Maximo 5 estrellas'),
    comentario: z
        .string()
        .max(1000, 'El comentario no puede superar los 1000 caracteres')
        .optional(),
});

export const valoracionesQuerySchema = z.object({
    page: z
        .number()
        .int()
        .gte(1, 'La pagina debe ser mayor a 0')
        .optional()
        .default(1),
    pageSize: z
        .number()
        .int()
        .gte(1)
        .lte(50, 'El tamano de pagina no puede superar 50')
        .optional()
        .default(10),
});

// ========== Types Inferidos - Valoraciones ==========

export type CreateValoracionFormData = z.infer<typeof createValoracionSchema>;
export type ValoracionesQueryParams = z.infer<typeof valoracionesQuerySchema>;
```

### 3.3 Notas sobre los schemas

- `createValoracionSchema` se usa en el `ValoracionForm.tsx` con `react-hook-form` + `zodResolver`.
- `valoracionesQuerySchema` se usa en los hooks que llaman al GET de valoraciones para validar/parsear los query params antes de enviarlos al API client.
- El campo `puntuacion` usa `z.number()` (no `z.string()`) porque el formulario de estrellas enviara un numero directamente, no un string. El componente `StarRatingInput` debe pasar el valor como `number` al form.
- No se agregan refinements cross-field porque los dos schemas no tienen dependencias entre campos.

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Tipo de cambio:** MODIFICACION de archivo existente. Dos inserciones puntuales.

### 4.1 Endpoints API

**Insercion:** Agregar la clave `valoraciones` dentro del objeto `crowdsourcing` de `API_ROUTES`, despues del bloque `entregables` y antes del bloque `conversaciones`.

| Constante | Valor | Metodo HTTP | Uso |
|-----------|-------|-------------|-----|
| `API_ROUTES.crowdsourcing.valoraciones.create(acuerdoId)` | `` `/api/crowdsourcing/acuerdos/${acuerdoId}/valoraciones` `` | POST | Crear valoracion desde el detalle de un acuerdo completado |
| `API_ROUTES.crowdsourcing.valoraciones.byUser(userId)` | `` `/api/crowdsourcing/usuarios/${userId}/valoraciones` `` | GET | Obtener resumen + listado paginado de valoraciones de un usuario |

**Bloque exacto a insertar** (entre `entregables` y `conversaciones`):

```typescript
        valoraciones: {
            create: (acuerdoId: string) =>
                `/api/crowdsourcing/acuerdos/${acuerdoId}/valoraciones`,
            byUser: (userId: string) =>
                `/api/crowdsourcing/usuarios/${userId}/valoraciones`,
        },
```

**Posicion en el archivo:** linea 175 aproximadamente, despues del bloque `entregables: { ... },` y antes de `conversaciones: { ... }`.

### 4.2 Query Keys (React Query / TanStack Query)

**Insercion:** Agregar la clave `valoraciones` dentro del objeto `crowdsourcing` de `QUERY_KEYS`, despues del bloque `acuerdos` y antes del bloque `conversaciones`.

| Key | Patron | Uso |
|-----|--------|-----|
| `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId)` | `['crowdsourcing', 'valoraciones', userId] as const` | Cache de valoraciones por usuario; invalidar tras crear una valoracion exitosa |

**Bloque exacto a insertar** (entre `acuerdos` y `conversaciones`):

```typescript
        valoraciones: {
            byUserId: (userId: string) => ['crowdsourcing', 'valoraciones', userId] as const,
        },
```

**Posicion en el archivo:** linea 73 aproximadamente, despues de `acuerdos: { byId: ... },` y antes de `conversaciones: { ... }`.

### 4.3 Constantes de dominio adicionales

No se requieren constantes de dominio adicionales para esta feature. Los estados de acuerdo ya estan definidos en `ESTADO_ACUERDO` (con `COMPLETADO: 2`), que es la unica condicion de negocio que el frontend necesita evaluar para mostrar/ocultar el formulario de valoracion.

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

**Tipo de cambio:** MODIFICACION de archivo existente. Tres inserciones.

### 5.1 Insercion 1: codigos numericos en `ERROR_CODE_MESSAGES`

Agregar dentro del objeto `ERROR_CODE_MESSAGES`, en la seccion de comentario `// Validation errors (1000-1999)`, despues de la entrada `"1013"`:

```typescript
    "1014": "La puntuacion debe ser entre 1 y 5 estrellas",
```

Agregar dentro de la seccion `// Business Rule errors (4000-4999)`, despues de la entrada `"4016"`:

```typescript
    "4014": "Ya has enviado una valoracion para este acuerdo",
```

**Nota sobre el codigo `4007`:** Este codigo ya existe en `ERROR_CODE_MESSAGES` con el mensaje `"La campania ha finalizado"` (contexto incorrecto para valoraciones) y en `ACUERDO_ERROR_MESSAGES` con `"La propuesta no esta disponible para esta accion"`. El hook `useCreateValoracion` debe manejar el codigo `4007` con un mensaje especifico de contexto antes de delegar al lookup generico. Ver Seccion 5.3.

### 5.2 Insercion 2: nuevo bloque `VALORACION_ERROR_MESSAGES`

Agregar al final del archivo, despues del bloque `getMensajeriaErrorMessage`:

```typescript
// ========== Valoraciones Crowdsourcing Error Messages (US-CS-06) ==========

export const VALORACION_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for valoraciones context)
    '1014': 'La puntuacion debe ser entre 1 y 5 estrellas',
    '2011': 'El acuerdo no fue encontrado',
    '3002': 'No eres participante de este acuerdo',
    '4007': 'Solo puedes valorar acuerdos que han sido completados',
    '4014': 'Ya has enviado una valoracion para este acuerdo',

    // Semantic keys para uso interno en componentes y hooks
    VALORACION_DUPLICATE: 'Ya has enviado una valoracion para este acuerdo',
    VALORACION_ACUERDO_NOT_COMPLETED: 'Solo puedes valorar acuerdos que han sido completados',
    VALORACION_PUNTUACION_INVALID: 'La puntuacion debe ser entre 1 y 5 estrellas',
    VALORACION_COMENTARIO_MAX: 'El comentario no puede superar los 1000 caracteres',
} as const;

/**
 * Obtiene mensaje de error para el flujo de valoraciones.
 * Caso especial: el codigo 4007 tiene significado distinto en valoraciones
 * respecto a acuerdos (US-CS-04). Esta funcion devuelve el mensaje correcto
 * para el contexto de valoraciones.
 * Usa mensajes especificos de valoraciones si existen, sino fallback a getErrorMessage.
 */
export const getValoracionErrorMessage = (errorCode: string): string => {
    return VALORACION_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### 5.3 Tabla de Error Messages

| Codigo / Key | Mensaje | Contexto de uso |
|--------------|---------|-----------------|
| `'1014'` | La puntuacion debe ser entre 1 y 5 estrellas | POST crear valoracion: puntuacion fuera del rango 1-5 |
| `'4014'` | Ya has enviado una valoracion para este acuerdo | POST crear valoracion: intento de valoracion duplicada |
| `'4007'` (override) | Solo puedes valorar acuerdos que han sido completados | POST crear valoracion: acuerdo no esta en estado Completado. IMPORTANTE: en el contexto de propuestas/acuerdos (US-CS-04) este codigo tiene otro mensaje; `getValoracionErrorMessage` resuelve el override correcto |
| `'2011'` (override) | El acuerdo no fue encontrado | POST crear valoracion: acuerdoId no existe |
| `'3002'` (override) | No eres participante de este acuerdo | POST crear valoracion: usuario no es artista ni profesional del acuerdo |
| `VALORACION_DUPLICATE` | Ya has enviado una valoracion para este acuerdo | Key semantico para uso directo en componentes sin pasar por codigo numerico |
| `VALORACION_ACUERDO_NOT_COMPLETED` | Solo puedes valorar acuerdos que han sido completados | Key semantico para estados de UI (tooltip, mensaje inline) |
| `VALORACION_PUNTUACION_INVALID` | La puntuacion debe ser entre 1 y 5 estrellas | Key semantico para validacion de formulario |
| `VALORACION_COMENTARIO_MAX` | El comentario no puede superar los 1000 caracteres | Key semantico para validacion de formulario |

---

## 6. Archivos a Crear/Modificar

```
src/shared/
├── types/
│   └── crowdsourcing.ts          [MODIFICAR] - Agregar 6 interfaces al final
├── schemas/
│   └── crowdsourcing.schema.ts   [MODIFICAR] - Agregar 2 schemas + 2 types inferidos al final
├── constants/
│   └── index.ts                  [MODIFICAR] - 2 inserciones puntuales en QUERY_KEYS y API_ROUTES
└── utils/
    └── error-messages.ts         [MODIFICAR] - 2 codigos en ERROR_CODE_MESSAGES + nuevo bloque VALORACION_ERROR_MESSAGES
```

**No se crean archivos nuevos.** Todos son modificaciones de archivos existentes.

---

## 7. Dependencias

- `zod` (ya instalado en el proyecto)
- Ningun package adicional requerido
- Los tipos no dependen de ninguna libreria de terceros

---

## 8. Notas de Implementacion

### 8.1 Tipo `PaginatedResult<T>`

El contrato define `PaginatedResult<T>` como tipo generico. Al momento de redactar este plan, NO existe en `src/shared/types/crowdsourcing.ts`. Es el primer uso de paginacion real en el modulo crowdsourcing (las features anteriores no tenian listados paginados en shared). Agregarlo en la seccion de valoraciones con un comentario que indique que es reutilizable.

### 8.2 Tipo de `distribucion` en `ValoracionResumen`

El campo `distribucion` usa numeric literal types como claves (`5: number; 4: number; ...`) en lugar de `Record<number, number>`. Esto refleja exactamente el contrato del backend (el API siempre devuelve las 5 claves, nunca menos) y permite al TypeScript compiler garantizar que el consumer accede con claves validas (1-5). Este es el patron correcto alineado con la documentacion en contracts.md.

### 8.3 `autorImagenUrl: string | null` vs `string | undefined`

El campo `autorImagenUrl` en `ValoracionListItem` se tipea como `string | null` (no como `string | undefined` ni como `string?`). El backend puede devolver explicitamente `null` en el JSON cuando el autor no tiene imagen. En el frontend, `null` y `undefined` tienen semanticas distintas: `null` = "el backend confirmo que no hay imagen"; `undefined` = "el campo no vino en la respuesta". Usar `null` es correcto aqui y es consistente con como el backend lo documenta.

### 8.4 Conflicto del codigo `4007`

El codigo `4007` esta definido dos veces con mensajes distintos:
- En `ERROR_CODE_MESSAGES` (global): `"La campania ha finalizado"` (contexto: campanias de crowdfunding)
- En `ACUERDO_ERROR_MESSAGES`: `"La propuesta no esta disponible para esta accion"` (contexto: US-CS-04)
- En `VALORACION_ERROR_MESSAGES` (nuevo): `"Solo puedes valorar acuerdos que han sido completados"` (contexto: US-CS-06)

La solucion es que cada hook de valoracion use `getValoracionErrorMessage` en lugar de `getErrorMessage` o `getAcuerdoErrorMessage`. El hook `useCreateValoracion` debe importar y usar `getValoracionErrorMessage` para resolver mensajes de error.

### 8.5 Schema `valoracionesQuerySchema` con `.default()`

El schema usa `.default(1)` y `.default(10)` para que al parsear params vacios se obtengan los valores por defecto del backend. El hook `useValoracionesUsuario` debe parsear los params con `valoracionesQuerySchema.parse(params)` antes de construir la query string.

### 8.6 Invalidacion de cache tras crear valoracion

Cuando `useCreateValoracion` recibe una respuesta 201 exitosa, debe invalidar:
1. `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userIdValorado)` - para refrescar las valoraciones del usuario valorado
2. `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` - por si el acuerdo expone alguna propiedad de "ya valorado"

El `userIdValorado` no se devuelve en la respuesta 201 (solo se devuelve el id de la valoracion, puntuacion, comentario y fecha). El hook debera recibirlo como parametro adicional o el componente padre deberá pasarlo al hook.

---

## 9. Checklist

- [ ] Interfaces de valoraciones agregadas en `src/shared/types/crowdsourcing.ts` (6 interfaces: `CreateValoracionRequest`, `ValoracionCreatedResult`, `ValoracionResumen`, `ValoracionListItem`, `PaginatedResult<T>`, `ValoracionesUsuario`)
- [ ] Schemas Zod agregados en `src/shared/schemas/crowdsourcing.schema.ts` (`createValoracionSchema`, `valoracionesQuerySchema`)
- [ ] Types inferidos exportados (`CreateValoracionFormData`, `ValoracionesQueryParams`)
- [ ] `QUERY_KEYS.crowdsourcing.valoraciones.byUserId` agregado en `src/shared/constants/index.ts`
- [ ] `API_ROUTES.crowdsourcing.valoraciones.create` y `API_ROUTES.crowdsourcing.valoraciones.byUser` agregados en `src/shared/constants/index.ts`
- [ ] Codigo `"1014"` agregado en `ERROR_CODE_MESSAGES` de `src/shared/utils/error-messages.ts`
- [ ] Codigo `"4014"` agregado en `ERROR_CODE_MESSAGES` de `src/shared/utils/error-messages.ts`
- [ ] Bloque `VALORACION_ERROR_MESSAGES` agregado con 4 keys semanticos y 5 overrides de codigos numericos
- [ ] Funcion `getValoracionErrorMessage` exportada
- [ ] Todos los mensajes de error escritos en espanol, sin tildes en vocales (consistente con el resto del archivo)
- [ ] `PaginatedResult<T>` verificado como no existente antes de agregarlo (evitar duplicado)
- [ ] Ninguna propiedad tipada como `any`
- [ ] Todos los campos `fechaCreacion` tipados como `string` (ISO 8601), no como `Date`
