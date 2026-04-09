# Plan de Contratos Shared: cs-explorar-propuestas

**Fecha:** 2026-02-17
**Feature:** cs-explorar-propuestas (US-CS-03)
**Basado en:** docs/user-stories/cs-explorar-propuestas/contracts.md

---

## 1. Resumen

- Total de types nuevos: 8 (mas 1 union type)
- Total de schemas Zod nuevos: 2 (mas 2 types inferidos)
- Constantes nuevas: 8 bloques de constantes en archivos existentes
- Utilidades nuevas: 1 objeto de mensajes de error + 1 funcion helper
- Todos los archivos a modificar **ya existen**; no se crea ningun archivo nuevo

### Archivos a modificar (no crear)

```
src/shared/
├── types/
│   └── crowdsourcing.ts          MODIFICAR - agregar 8 interfaces + 1 union type
├── schemas/
│   └── crowdsourcing.schema.ts   MODIFICAR - agregar 2 schemas + 2 types inferidos
├── constants/
│   └── index.ts                  MODIFICAR - agregar constantes de estado, ordenamiento,
│                                             urgencia, query keys, api routes, app routes
└── utils/
    └── error-messages.ts         MODIFICAR - agregar PROPUESTA_ERROR_MESSAGES y
                                              getPropuestaErrorMessage
```

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

### Estado actual del archivo

El archivo ya contiene types para:
- Plantillas de proyecto (`PlantillaProyectoList`, `PlantillaProyecto`, etc.)
- Necesidades del artista (`NecesidadCrowdsourcingList`, `NecesidadCrowdsourcing`)
- Propuestas recibidas (vista artista) (`PropuestaCrowdsourcing`)
- Maestras (`MaestraTipoNecesidad`, `MaestraModalidadTrabajo`, `MaestraMoneda`)
- Union types de dominio (`EstadoNecesidad`, `ModalidadTrabajo`, `PrioridadNecesidad`)

### 2.1 Seccion nueva a agregar al final del archivo

Agregar bajo el comentario `// ========== Explorar Propuestas - Vista Profesional ==========`

#### DTOs de Response

| Tipo | Propiedades clave | Descripcion |
|------|-------------------|-------------|
| `NecesidadPublicaList` | id, titulo, descripcion?, tipoNecesidadId, tipoNecesidadNombre, presupuestoMin?, presupuestoMax?, monedaId?, monedaNombre?, modalidadTrabajoId, modalidadTrabajoNombre, ubicacionCiudad?, ubicacionPais?, artistaNombre, fechaCreacion, fechaLimitePropuestas?, esUrgente, numeroPropuestas | DTO para card en listado publico. Descripcion truncada a 150 chars por el backend |
| `NecesidadPublica` | id, titulo, descripcion?, tipoNecesidadId, tipoNecesidadNombre, estadoNecesidadId, estadoNecesidadNombre, modalidadTrabajoId, modalidadTrabajoNombre, presupuestoMin?, presupuestoMax?, monedaId?, monedaNombre?, ubicacionCiudad?, ubicacionPais?, fechaCreacion, fechaLimitePropuestas?, fechaInicioPrevista?, numeroPropuestas, artista: ArtistaPublico, yaPropuso, esPropietario, tienePerfilProfesional | DTO detalle completo para el profesional. Campos calculados segun userId del token |
| `ArtistaPublico` | id, nombreArtistico, imagenUrl? | Subtype embebido en NecesidadPublica. Solo datos publicos del artista |
| `PropuestaCreatedResult` | id, necesidadTitulo, precioPropuesto, estadoPropuestaNombre, fechaCreacion | Respuesta al enviar propuesta (POST 201) |
| `MiPropuestaList` | id, necesidadTitulo, artistaNombre, precioPropuesto, monedaId, monedaNombre, estadoPropuestaId, estadoPropuestaNombre, fechaCreacion, fechaActualizacion?, acuerdoId? | DTO para card en listado "Mis propuestas". acuerdoId nulo si no fue aceptada |
| `RetirarPropuestaResult` | id, estadoPropuestaNombre | Respuesta minima al retirar propuesta (PATCH 200) |

#### DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CreatePropuestaRequest` | precioPropuesto: number, monedaId: number, diasEstimados?: number, mensajePropuesta: string | Body del POST para enviar propuesta. diasEstimados opcional |
| `NecesidadesPublicasFilter` | tipoNecesidadId?: number, modalidad?: number, presupuestoMin?: number, presupuestoMax?: number, pais?: string, orderBy?: OrderByNecesidades, page?: number, pageSize?: number, search?: string | Parametros de query para GET /api/crowdsourcing/necesidades |

#### Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `EstadoPropuesta` | `'Pendiente' \| 'Aceptada' \| 'Rechazada' \| 'Retirada'` | Estado string de propuesta para UI. Complementa ESTADO_PROPUESTA (numerico) |
| `OrderByNecesidades` | `'recientes' \| 'mayor-presupuesto' \| 'fecha-limite'` | Valores de query param orderBy para el listado publico |

### 2.2 Contenido exacto a agregar

```typescript
// ========== Explorar Propuestas - Vista Profesional (US-CS-03) ==========

export interface NecesidadPublicaList {
    id: string;
    titulo: string;
    /** Truncada a 150 caracteres por el backend */
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
    /** Calculado en backend: fechaLimitePropuestas existe y es menor a 3 dias desde ahora */
    esUrgente: boolean;
    numeroPropuestas: number;
}

export interface ArtistaPublico {
    id: string;
    nombreArtistico: string;
    imagenUrl?: string;
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
    /** true si el usuario autenticado ya envio una propuesta no-Retirada */
    yaPropuso: boolean;
    /** true si el usuario autenticado es el artista propietario de la necesidad */
    esPropietario: boolean;
    /** true si existe PerfilProfesional para el usuario autenticado */
    tienePerfilProfesional: boolean;
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
    /** Nulo hasta que la propuesta sea aceptada y se genere un acuerdo */
    acuerdoId?: string;
}

export interface RetirarPropuestaResult {
    id: string;
    estadoPropuestaNombre: string;
}

export type OrderByNecesidades = 'recientes' | 'mayor-presupuesto' | 'fecha-limite';

export interface NecesidadesPublicasFilter {
    tipoNecesidadId?: number;
    modalidad?: number;
    presupuestoMin?: number;
    presupuestoMax?: number;
    pais?: string;
    orderBy?: OrderByNecesidades;
    page?: number;
    pageSize?: number;
    search?: string;
}

export type EstadoPropuesta = 'Pendiente' | 'Aceptada' | 'Rechazada' | 'Retirada';
```

### 2.3 Nota sobre PaginatedResponse

El type `PaginatedResponse<T>` ya existe en `src/shared/types/api.ts` con la estructura exacta que devuelve el backend (items, totalCount, page, pageSize, totalPages). No se requiere modificar.

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

### Estado actual del archivo

El archivo ya contiene:
- `necesidadSeleccionadaSchema` y `generarNecesidadesSchema` (para templates)
- `createNecesidadSchema` y `updateNecesidadSchema` (para admin de necesidades del artista)
- `cerrarNecesidadSchema`
- Types inferidos correspondientes

### 3.1 Schemas nuevos a agregar

| Schema | Campos | Reglas |
|--------|--------|--------|
| `createPropuestaSchema` | precioPropuesto, monedaId, diasEstimados?, mensajePropuesta | precioPropuesto > 0; monedaId > 0; diasEstimados entero > 0 y <= 365 si presente; mensajePropuesta min 20 max 2000 |
| `filterNecesidadesSchema` | tipoNecesidadId?, modalidad?, presupuestoMin?, presupuestoMax?, pais?, orderBy?, search?, page?, pageSize? | presupuestoMax >= presupuestoMin si ambos presentes (refine); pais max 100; search max 200; pageSize max 50 |

### 3.2 Reglas de validacion detalladas

**createPropuestaSchema:**
- `precioPropuesto`: `z.number({ required_error: '...', invalid_type_error: '...' }).positive('El precio propuesto debe ser mayor a 0')`
- `monedaId`: `z.number({ required_error: '...', invalid_type_error: '...' }).positive('La moneda es obligatoria')`
- `diasEstimados`: `z.number().int('Los dias estimados deben ser un numero entero').positive('Los dias estimados deben ser mayor a 0').max(365, 'Maximo 365 dias').optional()`
- `mensajePropuesta`: `z.string().min(1, 'El mensaje de propuesta es obligatorio').min(20, 'El mensaje debe tener al menos 20 caracteres').max(2000, 'El mensaje no puede superar los 2000 caracteres')`

**filterNecesidadesSchema:**
- `tipoNecesidadId`: `z.number().positive().optional()`
- `modalidad`: `z.number().positive().optional()`
- `presupuestoMin`: `z.number().nonnegative('El presupuesto minimo no puede ser negativo').optional()`
- `presupuestoMax`: `z.number().nonnegative('El presupuesto maximo no puede ser negativo').optional()`
- `pais`: `z.string().max(100).optional()`
- `orderBy`: `z.enum(['recientes', 'mayor-presupuesto', 'fecha-limite']).optional()`
- `search`: `z.string().max(200).optional()`
- `page`: `z.number().positive().optional()`
- `pageSize`: `z.number().positive().max(50).optional()`
- Refine: si presupuestoMin y presupuestoMax ambos presentes, presupuestoMax >= presupuestoMin

### 3.3 Types inferidos

| Type inferido | Schema fuente |
|---------------|---------------|
| `CreatePropuestaFormData` | `z.infer<typeof createPropuestaSchema>` |
| `FilterNecesidadesFormData` | `z.infer<typeof filterNecesidadesSchema>` |

### 3.4 Contenido exacto a agregar al final del archivo

```typescript
// ========== Schemas para Explorar Propuestas - Vista Profesional (US-CS-03) ==========

export const createPropuestaSchema = z.object({
    precioPropuesto: z
        .number({
            required_error: 'El precio propuesto es obligatorio',
            invalid_type_error: 'El precio debe ser un numero',
        })
        .positive('El precio propuesto debe ser mayor a 0'),
    monedaId: z
        .number({
            required_error: 'La moneda es obligatoria',
            invalid_type_error: 'Selecciona una moneda valida',
        })
        .positive('La moneda es obligatoria'),
    diasEstimados: z
        .number()
        .int('Los dias estimados deben ser un numero entero')
        .positive('Los dias estimados deben ser mayor a 0')
        .max(365, 'Maximo 365 dias')
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
    presupuestoMin: z
        .number()
        .nonnegative('El presupuesto minimo no puede ser negativo')
        .optional(),
    presupuestoMax: z
        .number()
        .nonnegative('El presupuesto maximo no puede ser negativo')
        .optional(),
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
        message: 'El presupuesto maximo debe ser mayor o igual al minimo',
        path: ['presupuestoMax'],
    }
);

// ========== Types Inferidos - Explorar Propuestas ==========

export type CreatePropuestaFormData = z.infer<typeof createPropuestaSchema>;
export type FilterNecesidadesFormData = z.infer<typeof filterNecesidadesSchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

### Estado actual del archivo

Ya contiene: `QUERY_KEYS`, `API_ROUTES`, `APP_ROUTES`, `ESTADO_NECESIDAD` y variantes, `MODALIDAD_TRABAJO`, `MONEDA`, y otras constantes de dominio.

### 4.1 Constantes de estado de propuesta (bloque nuevo)

Agregar despues de la seccion `ESTADO_NECESIDAD_BADGES` (linea ~398).

| Constante | Tipo | Valores |
|-----------|------|---------|
| `ESTADO_PROPUESTA` | `Record<string, number> as const` | PENDIENTE=1, ACEPTADA=2, RECHAZADA=3, RETIRADA=4 |
| `ESTADO_PROPUESTA_LABELS` | `Record<number, string>` | 1='Pendiente', 2='Aceptada', 3='Rechazada', 4='Retirada' |
| `ESTADO_PROPUESTA_BADGES` | `Record<number, string>` | 1='yellow', 2='green', 3='red', 4='gray' |

```typescript
// ========== Estado de Propuesta (int IDs from maestras) ==========

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
    1: 'yellow',   // Pendiente
    2: 'green',    // Aceptada
    3: 'red',      // Rechazada
    4: 'gray',     // Retirada
};
```

### 4.2 Constantes de ordenamiento y urgencia (bloque nuevo)

Agregar inmediatamente despues de ESTADO_PROPUESTA_BADGES.

| Constante | Tipo | Descripcion |
|-----------|------|-------------|
| `ORDER_BY_NECESIDADES` | `as const object` | Valores validos del query param orderBy |
| `ORDER_BY_NECESIDADES_LABELS` | `Record<string, string>` | Etiquetas UI para cada opcion de orden |
| `URGENCIA_DIAS_UMBRAL` | `number` | Umbral en dias para mostrar badge de urgencia (valor: 3) |

```typescript
// ========== Ordenamiento de Necesidades Publicas ==========

export const ORDER_BY_NECESIDADES = {
    RECIENTES: 'recientes',
    MAYOR_PRESUPUESTO: 'mayor-presupuesto',
    FECHA_LIMITE: 'fecha-limite',
} as const;

export const ORDER_BY_NECESIDADES_LABELS: Record<string, string> = {
    recientes: 'Mas recientes',
    'mayor-presupuesto': 'Mayor presupuesto',
    'fecha-limite': 'Fecha limite proxima',
};

/** Dias restantes desde hoy para considerar una necesidad como urgente */
export const URGENCIA_DIAS_UMBRAL = 3;
```

### 4.3 Query Keys (modificacion de QUERY_KEYS.crowdsourcing)

La estructura actual de `QUERY_KEYS.crowdsourcing.necesidades` tiene `mis` y `byId`. Se debe ampliar con las claves para necesidades publicas y propuestas del profesional.

**Modificacion sobre la clave existente** `crowdsourcing.necesidades`:

```typescript
// ANTES (existente):
necesidades: {
    mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
    byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
},

// DESPUES (con adiciones):
necesidades: {
    mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
    byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
    // Nuevas claves para vista profesional (US-CS-03)
    publicas: ['crowdsourcing', 'necesidades', 'publicas'] as const,
    publicaById: (id: string) => ['crowdsourcing', 'necesidades', 'publica', id] as const,
},
```

**Adicion de clave** `crowdsourcing.propuestas` (nueva sub-rama):

```typescript
// Agregar dentro de crowdsourcing: { ... }
propuestas: {
    mis: ['crowdsourcing', 'propuestas', 'mis'] as const,
},
```

### 4.4 API Routes (modificacion de API_ROUTES.crowdsourcing)

La estructura actual de `API_ROUTES.crowdsourcing.necesidades` tiene `base`, `mis`, `byId`, `cerrar`. Se amplia con la ruta para enviar propuestas. Se agrega la sub-rama `propuestas`.

**Modificacion sobre** `crowdsourcing.necesidades`:

```typescript
// ANTES (existente):
necesidades: {
    base: '/api/crowdsourcing/necesidades',
    mis: '/api/crowdsourcing/necesidades/mis-necesidades',
    byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
    cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
},

// DESPUES (con adiciones):
necesidades: {
    base: '/api/crowdsourcing/necesidades',
    mis: '/api/crowdsourcing/necesidades/mis-necesidades',
    byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
    cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
    // Nueva ruta para enviar propuesta (US-CS-03)
    propuestas: (necesidadId: string) =>
        `/api/crowdsourcing/necesidades/${necesidadId}/propuestas`,
},
```

**Adicion de sub-rama** `crowdsourcing.propuestas`:

```typescript
// Agregar dentro de crowdsourcing: { ... }
propuestas: {
    mis: '/api/crowdsourcing/propuestas/mis-propuestas',
    retirar: (id: string) => `/api/crowdsourcing/propuestas/${id}/retirar`,
},
```

### 4.5 App Routes (modificacion de APP_ROUTES.landing)

La estructura actual de `APP_ROUTES.landing.crowdsourcing` tiene `templates` y `wizard`. Se amplia con las rutas para explorar necesidades y mis propuestas.

**Modificacion sobre** `landing.crowdsourcing`:

```typescript
// ANTES (existente):
crowdsourcing: {
    templates: '/crowdsourcing/nuevo-proyecto',
    wizard: '/crowdsourcing/nuevo-proyecto',
},

// DESPUES (con adiciones):
crowdsourcing: {
    templates: '/crowdsourcing/nuevo-proyecto',
    wizard: '/crowdsourcing/nuevo-proyecto',
    // Nuevas rutas para profesionales (US-CS-03)
    necesidades: '/crowdsourcing/necesidades',
    necesidadDetail: (id: string) => `/crowdsourcing/necesidades/${id}`,
    misPropuestas: '/crowdsourcing/mis-propuestas',
},
```

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

### Estado actual del archivo

El archivo ya contiene:
- `ERROR_CODE_MESSAGES` (codigos numericos globales)
- `ERROR_MESSAGES` (semanticos + spread de ERROR_CODE_MESSAGES)
- `BACKING_ERROR_MESSAGES` + `getBackingErrorMessage`
- `CROWDSOURCING_ERROR_MESSAGES` + `getCrowdsourcingErrorMessage` (templates)
- `DASHBOARD_ERROR_MESSAGES` + `getDashboardErrorMessage`
- `NECESIDAD_ERROR_MESSAGES` + `getNecesidadErrorMessage`

### 5.1 Bloque nuevo: PROPUESTA_ERROR_MESSAGES

Agregar al final del archivo, despues de `getNecesidadErrorMessage`.

| Clave / Codigo | Mensaje | Contexto |
|----------------|---------|----------|
| `'2009'` | 'La necesidad no fue encontrada' | Necesidad no existe al intentar ver detalle o enviar propuesta |
| `'2010'` | 'La propuesta no fue encontrada' | ErrorCode: NotFound_Propuesta (backend) |
| `'4003'` | 'Ya tienes una propuesta enviada para esta necesidad' | ErrorCode: BusinessRule_AlreadyProposed |
| `'4004'` | 'No puedes enviar propuesta a tu propia necesidad' | ErrorCode: BusinessRule_CannotProposeSelf |
| `'4005'` | 'Solo se pueden retirar propuestas en estado Pendiente' | ErrorCode: BusinessRule_PropuestaNotRetirable |
| `'4006'` | 'Debes crear un perfil profesional para poder enviar propuestas' | ErrorCode: BusinessRule_NoProfessionalProfile |
| `PROPUESTA_NOT_FOUND` | 'La propuesta no fue encontrada' | Clave semantica |
| `ALREADY_PROPOSED` | 'Ya tienes una propuesta enviada para esta necesidad' | Clave semantica |
| `CANNOT_PROPOSE_SELF` | 'No puedes enviar propuesta a tu propia necesidad' | Clave semantica |
| `PROPUESTA_NOT_RETIRABLE` | 'Solo se pueden retirar propuestas en estado Pendiente' | Clave semantica |
| `NO_PROFESSIONAL_PROFILE` | 'Debes crear un perfil profesional para poder enviar propuestas' | Clave semantica; usado para mostrar CTA |
| `VALIDATION_PRECIO_REQUERIDO` | 'El precio propuesto debe ser mayor a 0' | Clave semantica de validacion |
| `VALIDATION_MENSAJE_MIN_LENGTH` | 'El mensaje debe tener al menos 20 caracteres' | Clave semantica de validacion |
| `VALIDATION_DIAS_ESTIMADOS_RANGO` | 'Los dias estimados deben ser entre 1 y 365' | Clave semantica de validacion |

```typescript
// ========== Propuestas Crowdsourcing Error Messages (US-CS-03) ==========

export const PROPUESTA_ERROR_MESSAGES: Record<string, string> = {
    // NotFound errors (2000-2999)
    '2009': 'La necesidad no fue encontrada',
    '2010': 'La propuesta no fue encontrada',

    // Business Rule errors (4000-4999)
    '4003': 'Ya tienes una propuesta enviada para esta necesidad',
    '4004': 'No puedes enviar propuesta a tu propia necesidad',
    '4005': 'Solo se pueden retirar propuestas en estado Pendiente',
    '4006': 'Debes crear un perfil profesional para poder enviar propuestas',

    // Semantic keys para uso interno en componentes
    PROPUESTA_NOT_FOUND: 'La propuesta no fue encontrada',
    ALREADY_PROPOSED: 'Ya tienes una propuesta enviada para esta necesidad',
    CANNOT_PROPOSE_SELF: 'No puedes enviar propuesta a tu propia necesidad',
    PROPUESTA_NOT_RETIRABLE: 'Solo se pueden retirar propuestas en estado Pendiente',
    NO_PROFESSIONAL_PROFILE: 'Debes crear un perfil profesional para poder enviar propuestas',
    VALIDATION_PRECIO_REQUERIDO: 'El precio propuesto debe ser mayor a 0',
    VALIDATION_MENSAJE_MIN_LENGTH: 'El mensaje debe tener al menos 20 caracteres',
    VALIDATION_DIAS_ESTIMADOS_RANGO: 'Los dias estimados deben ser entre 1 y 365',
} as const;

/**
 * Obtiene mensaje de error para el flujo de propuestas (enviar / retirar)
 * Usa mensajes especificos de propuestas si existen, sino fallback a getErrorMessage
 */
export const getPropuestaErrorMessage = (errorCode: string): string => {
    return PROPUESTA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

---

## 6. Archivos a modificar (resumen)

```
src/shared/
├── types/
│   └── crowdsourcing.ts          AGREGAR al final: 6 interfaces + 2 union types
│                                 bajo seccion "Explorar Propuestas - Vista Profesional"
├── schemas/
│   └── crowdsourcing.schema.ts   AGREGAR al final: createPropuestaSchema,
│                                 filterNecesidadesSchema, 2 types inferidos
├── constants/
│   └── index.ts                  AGREGAR en posicion indicada:
│                                   - ESTADO_PROPUESTA + labels + badges
│                                   - ORDER_BY_NECESIDADES + labels
│                                   - URGENCIA_DIAS_UMBRAL
│                                 MODIFICAR objetos existentes:
│                                   - QUERY_KEYS.crowdsourcing.necesidades (ampliar)
│                                   - QUERY_KEYS.crowdsourcing.propuestas (nueva sub-rama)
│                                   - API_ROUTES.crowdsourcing.necesidades (ampliar)
│                                   - API_ROUTES.crowdsourcing.propuestas (nueva sub-rama)
│                                   - APP_ROUTES.landing.crowdsourcing (ampliar)
└── utils/
    └── error-messages.ts         AGREGAR al final:
                                    - PROPUESTA_ERROR_MESSAGES
                                    - getPropuestaErrorMessage
```

---

## 7. Dependencias

- `zod` (ya instalado en `src/shared/node_modules/zod`)
- Ningun package adicional requerido

### Dependencias entre archivos

```
error-messages.ts
  depende de: getErrorMessage (ya definida en el mismo archivo)
  no requiere importar nada nuevo

crowdsourcing.schema.ts
  depende de: zod (ya importado en la linea 1)
  no requiere importar nada nuevo

crowdsourcing.ts
  depende de: ninguna importacion (solo tipos primitivos TS)
  no requiere importar nada nuevo

constants/index.ts
  depende de: tipos de dashboard.schema (ya importados en linea 1)
  no requiere importar nada nuevo
```

---

## 8. Orden de implementacion

1. **`src/shared/types/crowdsourcing.ts`**
   - Agregar al final del archivo la seccion de tipos nuevos
   - Sin dependencias, se puede hacer primero

2. **`src/shared/schemas/crowdsourcing.schema.ts`**
   - Agregar al final del archivo los dos schemas nuevos
   - Sin dependencias entre archivos; solo depende de zod que ya esta importado

3. **`src/shared/constants/index.ts`**
   - Agregar los tres bloques de constantes nuevas (ESTADO_PROPUESTA, ORDER_BY_NECESIDADES, URGENCIA_DIAS_UMBRAL)
   - Modificar los objetos QUERY_KEYS, API_ROUTES y APP_ROUTES en sus secciones `crowdsourcing` y `landing.crowdsourcing`
   - Requiere que los types de `crowdsourcing.ts` ya existan para referencia conceptual, aunque no hay importacion directa

4. **`src/shared/utils/error-messages.ts`**
   - Agregar al final del archivo PROPUESTA_ERROR_MESSAGES y getPropuestaErrorMessage
   - Depende de que `getErrorMessage` ya este definida (lo esta, linea 100)

No se requiere modificar ningun archivo `index.ts` de carpeta porque todos los archivos modificados ya estan exportados:
- `types/crowdsourcing.ts` ya exportado en `types/index.ts`
- `schemas/crowdsourcing.schema.ts` ya exportado en `schemas/index.ts`
- `constants/index.ts` ya exportado en `shared/index.ts`
- `utils/error-messages.ts` ya exportado en `utils/index.ts`

---

## 9. Notas de implementacion

1. **Conflicto potencial en error codes**: Los codigos `'4003'`, `'4004'`, `'4005'`, `'4006'` que se agregan en `PROPUESTA_ERROR_MESSAGES` **sobreescriben los mensajes genericos** definidos en `ERROR_CODE_MESSAGES` (lineas 43-48 de error-messages.ts). Esto es intencional: `PROPUESTA_ERROR_MESSAGES` es mas especifico para el contexto de propuestas. Al usar `getPropuestaErrorMessage` en los componentes de propuestas, los mensajes especificos tienen prioridad. Los mensajes genericos en `ERROR_MESSAGES` se mantienen inalterados para otros contextos.

2. **`yaPropuso`, `esPropietario`, `tienePerfilProfesional` son readonly**: Estos campos de `NecesidadPublica` son calculados por el backend y no deben usarse como parametros de request. Los frontends deben tratarlos como de solo lectura.

3. **`esUrgente` en `NecesidadPublicaList`**: El backend calcula `esUrgente` (fechaLimitePropuestas < 3 dias desde ahora). Sin embargo, el frontend puede recalcularlo localmente con `URGENCIA_DIAS_UMBRAL` si necesita mostrar un countdown en tiempo real. La fuente de verdad es el campo que viene del backend.

4. **`acuerdoId` en `MiPropuestaList`**: Cuando `acuerdoId` no es nulo, el componente `MiPropuestaCard` debe navegar al detalle del acuerdo. La ruta de acuerdos no esta definida en esta feature; se planificara en la feature correspondiente.

5. **`OrderByNecesidades` union type**: Este type esta definido en `crowdsourcing.ts` y referenciado en `NecesidadesPublicasFilter`. Tambien se usa en `filterNecesidadesSchema` con `z.enum([...])`. Las dos definiciones deben mantenerse sincronizadas. Si se agregan nuevas opciones de ordenamiento en el futuro, hay que actualizar ambos lugares.

6. **Modificaciones a `QUERY_KEYS` y `API_ROUTES` son objetos anidados**: Como ambos objetos terminan en `as const`, al modificarlos hay que asegurarse de que las nuevas claves tambien sean tipadas correctamente. Las funciones tipo `(id: string) => [...] as const` son el patron existente en el archivo.

7. **`as const` en `PROPUESTA_ERROR_MESSAGES`**: A diferencia de `NECESIDAD_ERROR_MESSAGES` (que no tiene `as const`), se propone agregar `as const` a `PROPUESTA_ERROR_MESSAGES` para consistencia con `BACKING_ERROR_MESSAGES` y permitir inferencia estricta de claves.

---

## 10. Checklist de implementacion

- [ ] `crowdsourcing.ts`: agregar seccion "Explorar Propuestas - Vista Profesional"
- [ ] `crowdsourcing.ts`: 6 interfaces exportadas (`NecesidadPublicaList`, `ArtistaPublico`, `NecesidadPublica`, `CreatePropuestaRequest`, `PropuestaCreatedResult`, `MiPropuestaList`, `RetirarPropuestaResult`)
- [ ] `crowdsourcing.ts`: 2 union types exportados (`OrderByNecesidades`, `EstadoPropuesta`)
- [ ] `crowdsourcing.schema.ts`: `createPropuestaSchema` con 4 campos y reglas correctas
- [ ] `crowdsourcing.schema.ts`: `filterNecesidadesSchema` con refine de presupuesto
- [ ] `crowdsourcing.schema.ts`: types inferidos `CreatePropuestaFormData` y `FilterNecesidadesFormData`
- [ ] `constants/index.ts`: `ESTADO_PROPUESTA` con 4 valores numericos y `as const`
- [ ] `constants/index.ts`: `ESTADO_PROPUESTA_LABELS` con etiquetas en espanol
- [ ] `constants/index.ts`: `ESTADO_PROPUESTA_BADGES` con colores alineados a shadcn Badge variants
- [ ] `constants/index.ts`: `ORDER_BY_NECESIDADES` con 3 valores y `as const`
- [ ] `constants/index.ts`: `ORDER_BY_NECESIDADES_LABELS` con etiquetas en espanol
- [ ] `constants/index.ts`: `URGENCIA_DIAS_UMBRAL = 3`
- [ ] `constants/index.ts`: `QUERY_KEYS.crowdsourcing.necesidades.publicas` y `.publicaById`
- [ ] `constants/index.ts`: `QUERY_KEYS.crowdsourcing.propuestas.mis`
- [ ] `constants/index.ts`: `API_ROUTES.crowdsourcing.necesidades.propuestas`
- [ ] `constants/index.ts`: `API_ROUTES.crowdsourcing.propuestas.mis` y `.retirar`
- [ ] `constants/index.ts`: `APP_ROUTES.landing.crowdsourcing.necesidades`, `.necesidadDetail` y `.misPropuestas`
- [ ] `error-messages.ts`: `PROPUESTA_ERROR_MESSAGES` con 6 codigos numericos y 8 claves semanticas
- [ ] `error-messages.ts`: `getPropuestaErrorMessage` exportada
- [ ] Verificar que ningun archivo `index.ts` de carpeta requiere actualizacion (no requiere)
- [ ] Verificar que no se usa `any` en ningun type nuevo
- [ ] Verificar que mensajes de validacion estan en espanol sin tildes (convencion del proyecto)
