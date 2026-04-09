# Plan de Contratos Shared: cs-mensajeria

**Fecha:** 2026-02-18
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)
**Basado en:** docs/user-stories/cs-mensajeria/contracts.md

---

## 1. Resumen

- Total de types a agregar: 10 interfaces + 2 union types
- Total de schemas Zod a agregar: 2 schemas con 2 types inferidos
- Constantes a agregar: 2 valores escalares + 1 objeto `FILTRO_CONVERSACION` + 3 query keys + 4 rutas API + 2 rutas de app
- Utilidades a agregar: 1 objeto de mensajes de error + 1 funcion getter + codigos numericos en `ERROR_CODE_MESSAGES`
- Archivos a modificar: 3 existentes (NO crear archivos nuevos)

### Decision clave: solo modificar archivos existentes

Los archivos `crowdsourcing.ts`, `crowdsourcing.schema.ts` y `constants/index.ts` ya existen y contienen
las secciones previas de crowdsourcing. El patron del proyecto es extender el mismo archivo con una
nueva seccion marcada por un comentario de US. No se crea ningun archivo nuevo.

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

**Accion:** MODIFICAR (agregar al final del archivo existente)

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreateConversacionResult` | `id: string`, `asunto: string`, `nombreDestinatario: string`, `contextoTipo: ContextoConversacion`, `contextoTitulo: string`, `fechaCreacion: string` | Respuesta de POST /conversaciones. `fechaCreacion` es ISO 8601 string (no Date) para consistencia con el resto del proyecto. |
| `ConversacionListItem` | `id: string`, `asunto: string`, `nombreOtraParte: string`, `imagenOtraParte?: string`, `contextoTipo: ContextoConversacion`, `contextoTitulo: string`, `ultimoMensaje?: string`, `fechaUltimoMensaje?: string`, `mensajesNoLeidos: number` | Item del listado de conversaciones. `imagenOtraParte` nullable (sin foto de perfil). `ultimoMensaje` y `fechaUltimoMensaje` undefined si no hay mensajes aun. |
| `ConversacionListResponse` | `items: ConversacionListItem[]`, `totalCount: number`, `totalNoLeidos: number`, `page: number`, `pageSize: number` | Wrapper de lista con metadata de paginacion. `totalNoLeidos` es la suma global (no paginada) de mensajes no leidos. |
| `Mensaje` | `id: string`, `contenido: string`, `urlAdjunto?: string`, `remitenteNombre: string`, `esPropio: boolean`, `leido: boolean`, `fechaCreacion: string` | Entidad de mensaje individual. `esPropio` calculado en backend comparando `UserIdRemitente` con el token. `urlAdjunto` undefined si no hay adjunto. |
| `MensajeListResponse` | `items: Mensaje[]`, `totalCount: number`, `page: number`, `pageSize: number` | Wrapper de lista de mensajes paginada. Sin `totalNoLeidos` (a diferencia de ConversacionListResponse). |
| `MarcarLeidosResponse` | `mensajesMarcados: number` | Respuesta del PATCH marcar-leidos. `0` si no habia mensajes no leidos (no es error). |
| `NoLeidosCountResponse` | `totalNoLeidos: number` | Respuesta del endpoint ligero para el badge del navbar. |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreateConversacionRequest` | `necesidadId?: string`, `acuerdoId?: string`, `userIdDestinatario: string`, `asunto: string` | Body del POST /conversaciones. `necesidadId` y `acuerdoId` mutuamente excluyentes; exactamente uno debe estar presente. La exclusividad mutua se valida en backend, no en el schema Zod del frontend (es logica de negocio que el componente gestiona). |
| `CreateMensajeRequest` | `contenido: string`, `urlAdjunto?: string` | Body del POST /conversaciones/{id}/mensajes. `urlAdjunto` opcional; si se envia debe ser URL valida. |

### 2.3 Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `ContextoConversacion` | `'necesidad' \| 'acuerdo'` | Tipo de contexto de la conversacion. Usado en `CreateConversacionResult`, `ConversacionListItem` y en UI para labels/iconos. Coincide con valores string del backend (`contextoTipo`). |
| `FiltroConversacion` | `'todas' \| 'necesidades' \| 'acuerdos'` | Valores validos del query param `contexto` del GET /conversaciones. Usado en los hooks y en los tabs/pills del UI. |

### Nota sobre tipos de fecha

Todo el proyecto usa `string` para fechas (ISO 8601) en lugar de `Date`. Mantener ese patron.
Ver `ConversacionListItem.fechaUltimoMensaje?: string` (nullable porque puede no haber mensajes).

### Bloque completo a agregar al final de `crowdsourcing.ts`

```typescript
// ========== Mensajeria Crowdsourcing (US-CS-05) ==========

// --- Union Types ---

export type ContextoConversacion = 'necesidad' | 'acuerdo';
export type FiltroConversacion = 'todas' | 'necesidades' | 'acuerdos';

// --- DTOs de Request ---

export interface CreateConversacionRequest {
    /** FK a NecesidadCrowdsourcing. Mutuamente excluyente con acuerdoId. */
    necesidadId?: string;
    /** FK a AcuerdoCrowdsourcing. Mutuamente excluyente con necesidadId. */
    acuerdoId?: string;
    /** UserId del destinatario (Identity User string). Requerido. */
    userIdDestinatario: string;
    /** Asunto de la conversacion. Min 1, max 200 chars. Requerido. */
    asunto: string;
}

export interface CreateMensajeRequest {
    /** Contenido del mensaje. Min 1, max 5000 chars. Requerido. */
    contenido: string;
    /** URL de adjunto externo. Opcional. Debe ser URL valida si se proporciona. */
    urlAdjunto?: string;
}

// --- DTOs de Response (POST results) ---

export interface CreateConversacionResult {
    id: string;
    asunto: string;
    nombreDestinatario: string;
    contextoTipo: ContextoConversacion;
    contextoTitulo: string;
    /** ISO 8601 string */
    fechaCreacion: string;
}

// --- DTOs de Response (listados) ---

export interface ConversacionListItem {
    id: string;
    asunto: string;
    nombreOtraParte: string;
    /** URL de imagen de perfil de la otra parte. Undefined si no tiene foto. */
    imagenOtraParte?: string;
    contextoTipo: ContextoConversacion;
    contextoTitulo: string;
    /** Truncado a 80 chars por el backend. Undefined si la conversacion no tiene mensajes. */
    ultimoMensaje?: string;
    /** ISO 8601 string. Undefined si la conversacion no tiene mensajes. */
    fechaUltimoMensaje?: string;
    mensajesNoLeidos: number;
}

export interface ConversacionListResponse {
    items: ConversacionListItem[];
    totalCount: number;
    /** Suma de mensajes no leidos en TODAS las conversaciones del usuario (no paginado). */
    totalNoLeidos: number;
    page: number;
    pageSize: number;
}

export interface Mensaje {
    id: string;
    contenido: string;
    /** URL de adjunto. Undefined si no tiene adjunto. */
    urlAdjunto?: string;
    remitenteNombre: string;
    /** true si UserIdRemitente == UserId del usuario autenticado. Calculado en backend. */
    esPropio: boolean;
    leido: boolean;
    /** ISO 8601 string */
    fechaCreacion: string;
}

export interface MensajeListResponse {
    items: Mensaje[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface MarcarLeidosResponse {
    /** Numero de mensajes que pasaron de Leido=false a Leido=true. 0 si no habia no leidos. */
    mensajesMarcados: number;
}

export interface NoLeidosCountResponse {
    /** Total de mensajes no leidos del usuario en todas sus conversaciones. */
    totalNoLeidos: number;
}
```

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

**Accion:** MODIFICAR (agregar al final del archivo existente)

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas | Alineado con FluentValidation backend |
|--------|--------|--------|---------------------------------------|
| `createConversacionSchema` | `necesidadId`, `acuerdoId`, `userIdDestinatario`, `asunto` | `necesidadId`: string UUID opcional; `acuerdoId`: string UUID opcional; `userIdDestinatario`: string min 1; `asunto`: string min 1 max 200 | `.NotEmpty()` -> `.min(1)`, `.MaximumLength(200)` -> `.max(200)` |
| `createMensajeSchema` | `contenido`, `urlAdjunto` | `contenido`: string min 1 max 5000; `urlAdjunto`: URL valida o string vacio, opcional | `.NotEmpty()` -> `.min(1)`, `.MaximumLength(5000)` -> `.max(5000)`, `.Must(BeAValidUrl).When(...)` -> `.url().optional().or(z.literal(''))` |

### Detalle de reglas campo por campo

**`createConversacionSchema`:**
- `necesidadId`: `z.string().uuid('ID de necesidad invalido').optional()` - UUID v4 si se proporciona; la exclusividad mutua con `acuerdoId` NO se valida en Zod (se gestiona en el componente que construye el request)
- `acuerdoId`: `z.string().uuid('ID de acuerdo invalido').optional()` - mismo patron que `necesidadId`
- `userIdDestinatario`: `z.string().min(1, 'El destinatario es obligatorio')` - es un Identity User string (no UUID)
- `asunto`: `z.string().min(1, 'El asunto es obligatorio').max(200, 'El asunto no puede superar los 200 caracteres')`

**`createMensajeSchema`:**
- `contenido`: `z.string().min(1, 'El mensaje no puede estar vacio').max(5000, 'El mensaje no puede superar los 5000 caracteres')`
- `urlAdjunto`: `z.string().url('Debe ser una URL valida').optional().or(z.literal(''))` - mismo patron que `urlRecurso` en `createEntregableSchema` (linea 373 del archivo actual)

### 3.2 Types Inferidos

| Type | Schema origen | Uso |
|------|---------------|-----|
| `CreateConversacionFormData` | `z.infer<typeof createConversacionSchema>` | Props del `IniciarConversacionDialog` con react-hook-form |
| `CreateMensajeFormData` | `z.infer<typeof createMensajeSchema>` | Props del `ChatInput` con react-hook-form |

### Bloque completo a agregar al final de `crowdsourcing.schema.ts`

```typescript
// ========== Schemas para Mensajeria Crowdsourcing (US-CS-05) ==========

export const createConversacionSchema = z.object({
    necesidadId: z
        .string()
        .uuid('ID de necesidad invalido')
        .optional(),
    acuerdoId: z
        .string()
        .uuid('ID de acuerdo invalido')
        .optional(),
    userIdDestinatario: z
        .string()
        .min(1, 'El destinatario es obligatorio'),
    asunto: z
        .string()
        .min(1, 'El asunto es obligatorio')
        .max(200, 'El asunto no puede superar los 200 caracteres'),
});

export const createMensajeSchema = z.object({
    contenido: z
        .string()
        .min(1, 'El mensaje no puede estar vacio')
        .max(5000, 'El mensaje no puede superar los 5000 caracteres'),
    urlAdjunto: z
        .string()
        .url('Debe ser una URL valida')
        .optional()
        .or(z.literal('')),
});

// ========== Types Inferidos - Mensajeria ==========

export type CreateConversacionFormData = z.infer<typeof createConversacionSchema>;
export type CreateMensajeFormData = z.infer<typeof createMensajeSchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Accion:** MODIFICAR (tres secciones dentro del archivo existente)

### 4.1 Endpoints API

Agregar dentro de `API_ROUTES.crowdsourcing` (despues de la key `entregables`, antes del cierre del objeto `crowdsourcing`):

| Constante | Valor | Metodo | Uso |
|-----------|-------|--------|-----|
| `conversaciones.base` | `'/api/crowdsourcing/conversaciones'` | POST / GET | Crear conversacion y listar conversaciones del usuario |
| `conversaciones.noLeidos` | `'/api/crowdsourcing/conversaciones/no-leidos'` | GET | Badge del navbar; polling cada 10 segundos |
| `conversaciones.mensajes` | `(id: string) => /api/crowdsourcing/conversaciones/${id}/mensajes` | GET / POST | Leer mensajes de una conversacion y enviar nuevo mensaje |
| `conversaciones.marcarLeidos` | `(id: string) => /api/crowdsourcing/conversaciones/${id}/marcar-leidos` | PATCH | Invocado automaticamente al abrir el chat |

**Atencion:** `noLeidos` debe declararse ANTES que `mensajes` y `marcarLeidos` en el objeto para que el path literal no colisione con las funciones. El orden importa para legibilidad pero no para TS.

### 4.2 App Routes (Landing)

Agregar dentro de `APP_ROUTES.landing.crowdsourcing` (despues de `acuerdoDetail`, antes del cierre del objeto):

| Key | Valor | Uso |
|-----|-------|-----|
| `mensajes` | `'/crowdsourcing/mensajes'` | Ruta del listado de conversaciones (MensajesPage) |
| `mensajeDetail` | `(id: string) => /crowdsourcing/mensajes/${id}` | Ruta del chat de una conversacion (ConversacionChatPage) |

**Nota:** Estas rutas estan protegidas con Bearer JWT y redirigen a `/auth/login` si no autenticado.

### 4.3 Query Keys (React Query)

Agregar dentro de `QUERY_KEYS.crowdsourcing` (despues de la key `acuerdos`, antes del cierre del objeto):

| Key | Patron | Uso |
|-----|--------|-----|
| `conversaciones.lista` | `(filtro?: string) => ['crowdsourcing', 'conversaciones', filtro ?? 'todas'] as const` | `useConversaciones(filtro?)` con polling |
| `conversaciones.mensajes` | `(id: string, page?: number) => ['crowdsourcing', 'conversaciones', id, 'mensajes', page ?? 1] as const` | `useMensajes(conversacionId, page?)` con polling |
| `conversaciones.noLeidos` | `['crowdsourcing', 'conversaciones', 'no-leidos'] as const` | `useNoLeidosCount()` con polling cada 10 segundos |

**Nota de invalidacion:** `useEnviarMensaje` debe invalidar tanto `conversaciones.mensajes(id)` como `conversaciones.lista()` (sin filtro, para refrescar todos los filtros). `useMarcarLeidos` invalida `conversaciones.noLeidos` y `conversaciones.lista()`.

### 4.4 Constantes escalares de mensajeria

Agregar en una nueva seccion al final del archivo, despues de `CAMPANIA_ESTADOS_LEGACY`:

| Constante | Tipo | Valor | Uso |
|-----------|------|-------|-----|
| `MENSAJERIA_POLLING_INTERVAL_MS` | `number` | `10000` | Intervalo de polling para conversaciones, mensajes y badge. Centralizado para facilitar cambio futuro o desactivacion. |
| `MENSAJERIA_PREVIEW_MAX_LENGTH` | `number` | `80` | Longitud maxima del preview de ultimo mensaje en el listado. El backend trunca a este valor; el frontend puede usarlo para truncar en UI si necesita. |

### 4.5 Objeto FILTRO_CONVERSACION

Agregar despues de `MENSAJERIA_PREVIEW_MAX_LENGTH` en la nueva seccion:

```typescript
export const FILTRO_CONVERSACION = {
    TODAS: 'todas',
    NECESIDADES: 'necesidades',
    ACUERDOS: 'acuerdos',
} as const;
```

Este objeto permite usar `FILTRO_CONVERSACION.TODAS` en lugar de strings literales en los tabs del UI y en los parametros de los hooks.

### Bloques exactos a insertar en `constants/index.ts`

**En `API_ROUTES.crowdsourcing`, despues del bloque `entregables`:**

```typescript
        conversaciones: {
            base: '/api/crowdsourcing/conversaciones',
            noLeidos: '/api/crowdsourcing/conversaciones/no-leidos',
            mensajes: (id: string) =>
                `/api/crowdsourcing/conversaciones/${id}/mensajes`,
            marcarLeidos: (id: string) =>
                `/api/crowdsourcing/conversaciones/${id}/marcar-leidos`,
        },
```

**En `APP_ROUTES.landing.crowdsourcing`, despues de `acuerdoDetail`:**

```typescript
            mensajes: '/crowdsourcing/mensajes',
            mensajeDetail: (id: string) => `/crowdsourcing/mensajes/${id}`,
```

**En `QUERY_KEYS.crowdsourcing`, despues del bloque `acuerdos`:**

```typescript
        conversaciones: {
            lista: (filtro?: string) =>
                ['crowdsourcing', 'conversaciones', filtro ?? 'todas'] as const,
            mensajes: (id: string, page?: number) =>
                ['crowdsourcing', 'conversaciones', id, 'mensajes', page ?? 1] as const,
            noLeidos: ['crowdsourcing', 'conversaciones', 'no-leidos'] as const,
        },
```

**Nueva seccion al final del archivo (despues de `CAMPANIA_ESTADOS_LEGACY`):**

```typescript
// ========== Mensajeria Crowdsourcing (US-CS-05) ==========

/** Intervalo de polling para mensajeria (MVP sin WebSocket). En milisegundos. */
export const MENSAJERIA_POLLING_INTERVAL_MS = 10000;

/** Longitud maxima del preview del ultimo mensaje en el listado de conversaciones. */
export const MENSAJERIA_PREVIEW_MAX_LENGTH = 80;

export const FILTRO_CONVERSACION = {
    TODAS: 'todas',
    NECESIDADES: 'necesidades',
    ACUERDOS: 'acuerdos',
} as const;
```

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

**Accion:** MODIFICAR (dos secciones dentro del archivo existente)

### 5.1 Codigos numericos en `ERROR_CODE_MESSAGES`

Agregar en el bloque correspondiente del objeto `ERROR_CODE_MESSAGES` (que ya existe en el archivo):

| Codigo | Mensaje | Seccion |
|--------|---------|---------|
| `'2014'` | `'La conversacion no fue encontrada'` | NotFound errors (2000-2999) - insertar despues de `'2013'` |
| `'4015'` | `'Ya existe una conversacion para este contexto'` | Business Rule errors (4000-4999) - insertar despues de `'4013'` |
| `'4016'` | `'No tienes relacion con este destinatario para iniciar una conversacion'` | Business Rule errors (4000-4999) - insertar despues de `'4015'` |

**Atencion:** El codigo `'4014'` ya existe como `BusinessRule_InvalidState` segun el estado actual de la tabla de constantes del backend. Verificar que no exista en `ERROR_CODE_MESSAGES` antes de agregar `'4015'` y `'4016'`.

### 5.2 Nuevo objeto de error messages especifico

Agregar al final del archivo, despues del bloque `getAcuerdoErrorMessage`:

| Clave | Mensaje | Tipo |
|-------|---------|------|
| Codigo `'2014'` | `'La conversacion no fue encontrada'` | Numerico (override con contexto) |
| Codigo `'4015'` | `'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.'` | Numerico con mensaje enriquecido para UI |
| Codigo `'4016'` | `'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.'` | Numerico con mensaje enriquecido para UI |
| `CONVERSACION_NOT_FOUND` | `'La conversacion no fue encontrada'` | Semantico |
| `CONVERSACION_DUPLICADA` | `'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.'` | Semantico - clave para el handler de `useCreateConversacion` |
| `CONVERSACION_NO_RELACION` | `'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.'` | Semantico |
| `MENSAJE_CONTENIDO_VACIO` | `'El mensaje no puede estar vacio'` | Semantico |
| `MENSAJE_CONTENIDO_MAX` | `'El mensaje es demasiado largo (maximo 5000 caracteres)'` | Semantico |
| `MENSAJE_URL_INVALIDA` | `'La URL adjunta no es valida. Verifica que sea una URL completa (ej: https://...)'` | Semantico |

### Bloque completo a agregar al final de `error-messages.ts`

```typescript
// ========== Mensajeria Crowdsourcing Error Messages (US-CS-05) ==========

export const MENSAJERIA_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for mensajeria context)
    '2014': 'La conversacion no fue encontrada',
    '4015': 'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.',
    '4016': 'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.',

    // Semantic keys para uso interno en componentes
    CONVERSACION_NOT_FOUND: 'La conversacion no fue encontrada',
    CONVERSACION_DUPLICADA: 'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.',
    CONVERSACION_NO_RELACION: 'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.',
    MENSAJE_CONTENIDO_VACIO: 'El mensaje no puede estar vacio',
    MENSAJE_CONTENIDO_MAX: 'El mensaje es demasiado largo (maximo 5000 caracteres)',
    MENSAJE_URL_INVALIDA: 'La URL adjunta no es valida. Verifica que sea una URL completa (ej: https://...)',
} as const;

/**
 * Obtiene mensaje de error para el flujo de mensajeria entre partes.
 * Caso especial: error 4015 indica conversacion duplicada; el componente
 * debe detectar este codigo y redirigir al chat existente en lugar de mostrar error.
 * Usa mensajes especificos de mensajeria si existen, sino fallback a getErrorMessage.
 */
export const getMensajeriaErrorMessage = (errorCode: string): string => {
    return MENSAJERIA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### Modificacion en `ERROR_CODE_MESSAGES` (objeto ya existente)

Dentro del bloque `// Not Found errors (2000-2999)`, agregar despues de `'2013'`:
```typescript
    '2014': 'La conversacion no fue encontrada',
```

Dentro del bloque `// Business Rule errors (4000-4999)`, agregar despues de `'4013'`:
```typescript
    '4015': 'Ya existe una conversacion para este contexto',
    '4016': 'No tienes relacion con este destinatario',
```

---

## 6. Archivos a Modificar/Crear

```
src/shared/
├── types/
│   └── crowdsourcing.ts          [MODIFICAR] Agregar 10 interfaces + 2 union types al final
├── schemas/
│   └── crowdsourcing.schema.ts   [MODIFICAR] Agregar 2 schemas + 2 types inferidos al final
├── constants/
│   └── index.ts                  [MODIFICAR] Tres puntos de insercion:
│                                   1. QUERY_KEYS.crowdsourcing.conversaciones (nuevo sub-objeto)
│                                   2. API_ROUTES.crowdsourcing.conversaciones (nuevo sub-objeto)
│                                   3. APP_ROUTES.landing.crowdsourcing.mensajes y mensajeDetail
│                                   4. Nueva seccion al final (MENSAJERIA_POLLING_INTERVAL_MS, etc.)
└── utils/
    └── error-messages.ts         [MODIFICAR] Dos puntos de modificacion:
                                    1. ERROR_CODE_MESSAGES: agregar '2014', '4015', '4016'
                                    2. Nueva seccion al final: MENSAJERIA_ERROR_MESSAGES + getter
```

**No se crea ningun archivo nuevo.** Los archivos `types/index.ts`, `schemas/index.ts` y `utils/index.ts`
ya re-exportan sus respectivos archivos con `export *`, por lo que las nuevas exportaciones seran
visibles automaticamente sin modificar los index.

---

## 7. Dependencias

- `zod` (ya instalado en `src/shared/node_modules/zod`)
- Ningun package adicional requerido
- No se requieren imports nuevos en `crowdsourcing.schema.ts` (ya importa `z` de `'zod'` en linea 1)
- No se requieren imports nuevos en `error-messages.ts` (referencia a `getErrorMessage` ya esta definida en el mismo archivo)

---

## 8. Notas de Implementacion

### Sobre la exclusividad mutua de `necesidadId` / `acuerdoId`

El schema Zod `createConversacionSchema` NO implementa un `.refine()` para validar la exclusividad mutua entre `necesidadId` y `acuerdoId`. Esta decision es intencionada porque:
1. En el flujo principal (Landing), el componente que abre `IniciarConversacionDialog` siempre tiene un contexto definido (desde una necesidad o desde un acuerdo) y pasa exactamente uno de los dos.
2. Los acuerdos crean la conversacion automaticamente al aceptar la propuesta (US-CS-04), por lo que el formulario se usa principalmente para necesidades.
3. El backend valida y retorna 400 si ambos o ninguno estan presentes.

Si en el futuro se quiere agregar la validacion al schema, usar `.refine()` al nivel del objeto.

### Sobre el campo `userIdDestinatario`

El backend usa `string` para `UserIdDestinatario` (Identity User string, no GUID de entidad de dominio). Por eso el schema usa `.min(1)` en lugar de `.uuid()`. El valor lo obtiene el frontend del perfil del otro participante en el contexto de la necesidad o acuerdo.

### Sobre el patron `urlAdjunto`

El patron `.url().optional().or(z.literal(''))` es identico al de `createEntregableSchema.urlRecurso` (linea 373-376 del schema existente). Esto es coherente: permite string vacio (cuando el usuario borra la URL), undefined (campo no tocado) o URL valida.

### Sobre `MENSAJERIA_POLLING_INTERVAL_MS`

Es un numero simple (no `as const` en su valor). El `as const` en el objeto `FILTRO_CONVERSACION` si aplica para que TypeScript infiera los tipos literales de los valores. Los hooks de React Query lo usaran como `refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS`.

### Sobre la query key `conversaciones.noLeidos`

Es un array literal (no una funcion) porque no tiene parametros. Usar `as const` directamente en el array: `['crowdsourcing', 'conversaciones', 'no-leidos'] as const`. Mismo patron que `QUERY_KEYS.crowdsourcing.propuestas.mis`.

### Sobre el error 4015 en el frontend

El hook `useCreateConversacion` debe tener logica especial: cuando recibe errorCode `'4015'`, en lugar de mostrar un toast de error, debe extraer el ID de la conversacion existente (el backend no lo devuelve en el contrato actual) y redirigir al usuario. Como el contrato actual no incluye el ID en el error 4015, la navegacion seria a la lista de conversaciones. Este comportamiento lo documenta el mensaje enriquecido de `CONVERSACION_DUPLICADA`.

### Sobre `APP_ROUTES.landing.crowdsourcing`

El objeto `APP_ROUTES.landing.crowdsourcing` ya existe (tiene `templates`, `wizard`, `necesidades`, `necesidadDetail`, `misPropuestas`, `acuerdoDetail`). Solo se agregan dos keys al final: `mensajes` y `mensajeDetail`. El objeto `APP_ROUTES` tiene `as const` al final, lo que es correcto.

### Sobre `APP_ROUTES.dashboard.crowdsourcing`

No se agregan rutas en dashboard para mensajeria en esta US. La mensajeria es feature exclusiva de Landing (la vista de artista y profesional para chatear entre partes vive en Landing, no en el dashboard de gestion del artista).

---

## 9. Orden de Implementacion

1. **`src/shared/types/crowdsourcing.ts`** - Sin dependencias, puede hacerse primero
2. **`src/shared/schemas/crowdsourcing.schema.ts`** - Depende solo de `zod` (ya importado)
3. **`src/shared/constants/index.ts`** - Sin dependencias de los otros archivos shared
4. **`src/shared/utils/error-messages.ts`** - Depende de `getErrorMessage` (ya definida en el mismo archivo)

Todos pueden implementarse en paralelo ya que no hay dependencias entre ellos.

---

## 10. Checklist de Verificacion

- [ ] Union types `ContextoConversacion` y `FiltroConversacion` exportados desde `crowdsourcing.ts`
- [ ] Los 8 interfaces de response exportados desde `crowdsourcing.ts`
- [ ] Los 2 interfaces de request exportados desde `crowdsourcing.ts`
- [ ] `createConversacionSchema` exportado con mensajes de validacion en espanol
- [ ] `createMensajeSchema` exportado con mensajes de validacion en espanol
- [ ] `CreateConversacionFormData` y `CreateMensajeFormData` exportados
- [ ] `QUERY_KEYS.crowdsourcing.conversaciones` con las 3 sub-keys (lista, mensajes, noLeidos)
- [ ] `API_ROUTES.crowdsourcing.conversaciones` con las 4 sub-keys (base, noLeidos, mensajes, marcarLeidos)
- [ ] `APP_ROUTES.landing.crowdsourcing.mensajes` y `mensajeDetail` agregados
- [ ] `MENSAJERIA_POLLING_INTERVAL_MS = 10000` exportado
- [ ] `MENSAJERIA_PREVIEW_MAX_LENGTH = 80` exportado
- [ ] `FILTRO_CONVERSACION` con las 3 keys exportado
- [ ] Codigos `'2014'`, `'4015'`, `'4016'` en `ERROR_CODE_MESSAGES`
- [ ] `MENSAJERIA_ERROR_MESSAGES` con codigos numericos y claves semanticas exportado
- [ ] `getMensajeriaErrorMessage` exportada
- [ ] Ningun uso de `any` en ninguno de los tipos
- [ ] Ninguna fecha tipada como `Date` (todas como `string`)
- [ ] Los archivos `types/index.ts`, `schemas/index.ts` y `utils/index.ts` NO modificados (re-exportan con `export *`)
