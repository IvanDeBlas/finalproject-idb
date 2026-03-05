# Plan de Contratos Shared: cp-programas-promocion

**Fecha:** 2026-02-25
**Feature:** cp-programas-promocion (US-CP-02)
**Basado en:** docs/user-stories/cp-programas-promocion/contracts.md

---

## 1. Resumen

- Total de types nuevos: 13 (interfaces y tipos a agregar a crowdpromotion.ts)
- Total de schemas Zod nuevos: 4 exportados (createPromoProgramaSchema, updatePromoProgramaSchema + 2 tipos inferidos) + 2 schemas internos (createPromoTareaSchema, updatePromoTareaSchema)
- Constantes nuevas: 14 bloques (TIPO_PROMO, TIPO_EVENTO_PROMO, TIPO_REWARD_PROMO con labels y descriptions; QUERY_KEYS.crowdpromotion.programas; API_ROUTES.crowdpromotion.programas; APP_ROUTES.dashboard.crowdpromotion.programas; limites de validacion en VALIDATION)
- Mensajes de error nuevos: 12 codigos numericos + 7 keys semanticos en PROMOTOR_ERROR_MESSAGES + funcion getPromoProgramaErrorMessage

---

## 2. Archivo 1: Modificar `src/shared/types/crowdpromotion.ts`

**Operacion:** AGREGAR al final del archivo existente, despues de la linea `export type PromotorEstado = 'activo' | 'inactivo';`

El archivo ya contiene los tipos de Promotor (CreatePromotorRequest, UpdatePromotorRequest, PromotorCreatedResult, Promotor, PromotorUpdatedResult, PromotorDesactivadoResult, TipoPromotor, PromotorEstado).

### 2.1 DTOs de Request a agregar

| Tipo | Propiedades clave | Descripcion |
|------|-------------------|-------------|
| `CreatePromoTareaItem` | titulo, tipoEventoPromoId, tipoRewardId, esRepetible, [descripcion, importeRecompensa, monedaId, puntosRecompensa, urlInstrucciones, maxRepeticiones, fechaInicio, fechaFin] | Item de tarea en el request de creacion |
| `UpdatePromoTareaItem` | extiende CreatePromoTareaItem + id?, esActivo? | Item de tarea en el request de edicion; id presente = editar existente, ausente = crear nueva |
| `CreatePromoProgramaRequest` | titulo, tipoPromoId, monedaId, [descripcion, campaniaCrowdfundingId, proyectoArtisticoId, urlLanding, codigoTrackingBase, importeComisionPorcentaje, importeComisionFija, fechaInicio, fechaFin, tareas?] | Body del POST /api/crowdpromotion/programas |
| `UpdatePromoProgramaRequest` | mismo que CreatePromoProgramaRequest pero tareas usa UpdatePromoTareaItem[] | Body del PUT /api/crowdpromotion/programas/{id} |

**Detalle de propiedades con tipos exactos:**

```
CreatePromoTareaItem:
    titulo: string                   // min 3, max 200
    descripcion?: string             // max 4000
    tipoEventoPromoId: number        // int, min 1
    tipoRewardId: number             // int, min 1
    importeRecompensa?: number       // >= 0; requerido si tipoRewardId == 1 o 3
    monedaId?: number                // int; requerido si importeRecompensa > 0
    puntosRecompensa?: number        // int, >= 0; requerido si tipoRewardId == 2 o 3
    urlInstrucciones?: string        // URL valida, max 500
    esRepetible: boolean             // default true
    maxRepeticiones?: number         // int, >= 1; requerido si esRepetible == true
    fechaInicio?: string             // ISO date YYYY-MM-DD
    fechaFin?: string                // ISO date YYYY-MM-DD

UpdatePromoTareaItem extends CreatePromoTareaItem:
    id?: string                      // Guid como string; presente si es tarea existente
    esActivo?: boolean               // false para desactivar tarea existente; default true

CreatePromoProgramaRequest:
    titulo: string                   // min 5, max 200
    descripcion?: string             // max 4000
    tipoPromoId: number              // int, min 1 (1-4 seed)
    campaniaCrowdfundingId?: string  // Guid como string
    proyectoArtisticoId?: string     // Guid como string
    urlLanding?: string              // URL valida, max 500
    codigoTrackingBase?: string      // solo [a-zA-Z0-9-], max 50
    monedaId: number                 // int, min 1 (1=EUR, 2=USD)
    importeComisionPorcentaje?: number  // 0-100 con 2 decimales
    importeComisionFija?: number        // >= 0 con 2 decimales
    fechaInicio?: string             // ISO date YYYY-MM-DD
    fechaFin?: string                // ISO date YYYY-MM-DD; > fechaInicio si ambas presentes
    tareas?: CreatePromoTareaItem[]  // array opcional; puede estar vacio

UpdatePromoProgramaRequest extends Omit<CreatePromoProgramaRequest, 'tareas'>:
    tareas?: UpdatePromoTareaItem[]
```

### 2.2 DTOs de Response a agregar

| Tipo | Propiedades clave | Descripcion |
|------|-------------------|-------------|
| `PromoProgramaCreatedResult` | id, titulo, tipoPromoNombre, esActivo, tareasCreadas, fechaCreacion | Response 201 del POST crear programa |
| `PromoProgramaListItem` | id, titulo, tipoPromoId, tipoPromoNombre, campaniaTitulo, esActivo, importeComisionPorcentaje, importeComisionFija, monedaNombre, numeroPromotores, numeroTareas, fechaInicio, fechaFin, fechaCreacion | Fila del listado paginado |
| `PromoProgramaListResult` | items, totalCount, page, pageSize | Envelope de paginacion del GET mis-programas |
| `PromoTareaDetail` | id, titulo, descripcion, tipoEventoPromoId, tipoEventoPromoNombre, tipoRewardId, tipoRewardNombre, importeRecompensa, monedaId, monedaNombre, puntosRecompensa, urlInstrucciones, esRepetible, maxRepeticiones, orden, esActivo, fechaInicio, fechaFin, completadosPorPromotores | Tarea dentro del detalle de programa |
| `PromoProgramaPromotorSummary` | id, promotorNombre, tipoPromotorNombre, esAprobado, esBloqueado, fechaAlta | Promotor inscrito en el detalle |
| `PromoProgramaResumen` | totalPromotoresAprobados, totalPromotoresPendientes, totalEventos, totalConversiones, valorTotalGenerado | Objeto de metricas en el detalle |
| `PromoProgramaDetail` | id, titulo, descripcion, tipoPromoId, tipoPromoNombre, campaniaCrowdfundingId, campaniaTitulo, proyectoArtisticoId, urlLanding, codigoTrackingBase, monedaId, monedaNombre, importeComisionPorcentaje, importeComisionFija, esActivo, fechaInicio, fechaFin, fechaCreacion, fechaActualizacion, tareas, promotores, resumen | Response del GET /programas/{id} |
| `PromoProgramaUpdatedResult` | id, titulo, fechaActualizacion | Response 200 del PUT |
| `PromoProgramaDesactivadoResult` | id, esActivo, tareasDesactivadas | Response 200 del PATCH desactivar |

**Detalle de tipos exactos para los campos nullable:**

```
PromoProgramaCreatedResult:
    id: string
    titulo: string
    tipoPromoNombre: string
    esActivo: boolean
    tareasCreadas: number
    fechaCreacion: string              // ISO datetime

PromoProgramaListItem:
    id: string
    titulo: string
    tipoPromoId: number
    tipoPromoNombre: string
    campaniaTitulo: string | null       // null si sin campana vinculada
    esActivo: boolean
    importeComisionPorcentaje: number | null
    importeComisionFija: number | null
    monedaNombre: string
    numeroPromotores: number
    numeroTareas: number
    fechaInicio: string | null         // ISO date YYYY-MM-DD
    fechaFin: string | null
    fechaCreacion: string              // ISO datetime

PromoProgramaListResult:
    items: PromoProgramaListItem[]
    totalCount: number
    page: number
    pageSize: number

PromoTareaDetail:
    id: string
    titulo: string
    descripcion: string | null
    tipoEventoPromoId: number
    tipoEventoPromoNombre: string
    tipoRewardId: number
    tipoRewardNombre: string
    importeRecompensa: number | null
    monedaId: number | null
    monedaNombre: string | null
    puntosRecompensa: number | null
    urlInstrucciones: string | null
    esRepetible: boolean
    maxRepeticiones: number | null
    orden: number
    esActivo: boolean
    fechaInicio: string | null
    fechaFin: string | null
    completadosPorPromotores: number

PromoProgramaPromotorSummary:
    id: string
    promotorNombre: string
    tipoPromotorNombre: string
    esAprobado: boolean
    esBloqueado: boolean
    fechaAlta: string                  // ISO datetime

PromoProgramaResumen:
    totalPromotoresAprobados: number
    totalPromotoresPendientes: number
    totalEventos: number
    totalConversiones: number
    valorTotalGenerado: number

PromoProgramaDetail:
    id: string
    titulo: string
    descripcion: string | null
    tipoPromoId: number
    tipoPromoNombre: string
    campaniaCrowdfundingId: string | null
    campaniaTitulo: string | null
    proyectoArtisticoId: string | null
    urlLanding: string | null
    codigoTrackingBase: string | null
    monedaId: number
    monedaNombre: string
    importeComisionPorcentaje: number | null
    importeComisionFija: number | null
    esActivo: boolean
    fechaInicio: string | null
    fechaFin: string | null
    fechaCreacion: string
    fechaActualizacion: string | null
    tareas: PromoTareaDetail[]
    promotores: PromoProgramaPromotorSummary[]
    resumen: PromoProgramaResumen

PromoProgramaUpdatedResult:
    id: string
    titulo: string
    fechaActualizacion: string         // ISO datetime

PromoProgramaDesactivadoResult:
    id: string
    esActivo: boolean
    tareasDesactivadas: number
```

### 2.3 Maestras (interfaces nuevas a agregar)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `TipoPromo` | id: number, nombre: string, descripcion: string | Maestra_TipoPromo (Referral, Afiliado, Influencer, Mixto) |
| `TipoEventoPromo` | id: number, nombre: string, descripcion: string | Maestra_TipoEventoPromo (Click, PageView, Share, Post, Signup, Backing) |
| `TipoRewardPromo` | id: number, nombre: string, descripcion: string | Maestra_TipoReward en contexto promo (Dinero, Puntos, Mixto) |

**Nota importante:** `TipoRewardPromo` debe nombrarse asi (no `TipoReward`) para no colisionar con el tipo `TIPO_REWARD` existente del modulo de campanias (Digital, Fisico, Experiencia, Otro).

### 2.4 Union Types a agregar

| Tipo | Valores | Uso |
|------|---------|-----|
| `PromoProgramaEstado` | `'activo' \| 'inactivo'` | Filtro en el listado de programas; patron consistente con PromotorEstado |

### 2.5 Bloque de separacion visual

Usar el mismo patron de comentarios separadores que existe en el archivo:

```
// ========== PromoPrograma - Requests ==========
// ========== PromoPrograma - Results ==========
// ========== Maestras Crowdpromotion ==========
// ========== Union Types PromoPrograma ==========
```

---

## 3. Archivo 2: Modificar `src/shared/schemas/crowdpromotion.schema.ts`

**Operacion:** AGREGAR al final del archivo existente, despues de los exports de CreatePromotorFormData y UpdatePromotorFormData.

El archivo ya tiene: `createPromotorSchema`, `updatePromotorSchema`, helper interno `urlOpcionalSchema`.

### 3.1 Schemas a agregar

| Schema | Exportado | Campos | Reglas especiales |
|--------|-----------|--------|-------------------|
| `createPromoTareaSchema` | No (interno) | titulo, descripcion, tipoEventoPromoId, tipoRewardId, importeRecompensa, monedaId, puntosRecompensa, urlInstrucciones, esRepetible, maxRepeticiones, fechaInicio, fechaFin | 3 refines: (1) esRepetible => maxRepeticiones != null; (2) tipoRewardId 1 o 3 => importeRecompensa != null; (3) tipoRewardId 2 o 3 => puntosRecompensa != null |
| `updatePromoTareaSchema` | No (interno) | extiende createPromoTareaSchema con .extend({id?, esActivo?}) | Hereda todos los refines del schema base |
| `createPromoProgramaSchema` | Si | titulo, descripcion, tipoPromoId, campaniaCrowdfundingId, proyectoArtisticoId, urlLanding, codigoTrackingBase, monedaId, importeComisionPorcentaje, importeComisionFija, fechaInicio, fechaFin, tareas | 2 refines: (1) al menos una comision; (2) fechaFin > fechaInicio |
| `updatePromoProgramaSchema` | Si | createPromoProgramaSchema.omit({tareas}) + .extend({tareas: z.array(updatePromoTareaSchema)}) | Hereda los 2 refines del schema base |

**Reglas campo por campo de createPromoTareaSchema:**

```
titulo:
    z.string({ required_error: 'El titulo de la tarea es obligatorio' })
    .min(1, 'El titulo de la tarea es obligatorio')
    .min(3, 'Al menos 3 caracteres')
    .max(200, 'Maximo 200 caracteres')

descripcion:
    z.string().max(4000, 'Maximo 4000 caracteres').optional().or(z.literal(''))

tipoEventoPromoId:
    z.number({
        required_error: 'El tipo de evento es obligatorio',
        invalid_type_error: 'El tipo de evento es obligatorio'
    }).int().min(1, 'El tipo de evento es obligatorio')

tipoRewardId:
    z.number({
        required_error: 'El tipo de recompensa es obligatorio',
        invalid_type_error: 'El tipo de recompensa es obligatorio'
    }).int().min(1, 'El tipo de recompensa es obligatorio')

importeRecompensa:
    z.number().min(0, 'No puede ser negativo').optional()

monedaId:
    z.number().int().min(1).optional()

puntosRecompensa:
    z.number().int().min(0, 'No puede ser negativo').optional()

urlInstrucciones:
    z.string()
    .url('La URL de instrucciones no tiene formato valido')
    .max(500, 'Maximo 500 caracteres')
    .optional()
    .or(z.literal(''))

esRepetible:
    z.boolean().default(true)

maxRepeticiones:
    z.number().int().min(1, 'Requiere al menos 1 repeticion').optional()

fechaInicio:
    z.string().optional()

fechaFin:
    z.string().optional()

Refine 1 - esRepetible con maxRepeticiones:
    .refine(
        (data) => !data.esRepetible || data.maxRepeticiones != null,
        { message: 'Las tareas repetibles requieren max repeticiones >= 1', path: ['maxRepeticiones'] }
    )

Refine 2 - recompensa monetaria requiere importe:
    .refine(
        (data) => {
            if (data.tipoRewardId === 1 || data.tipoRewardId === 3) {
                return data.importeRecompensa != null;
            }
            return true;
        },
        { message: 'El importe de recompensa es requerido para recompensas monetarias', path: ['importeRecompensa'] }
    )

Refine 3 - recompensa de puntos requiere puntos:
    .refine(
        (data) => {
            if (data.tipoRewardId === 2 || data.tipoRewardId === 3) {
                return data.puntosRecompensa != null;
            }
            return true;
        },
        { message: 'Los puntos de recompensa son requeridos para recompensas de puntos', path: ['puntosRecompensa'] }
    )
```

**Reglas campo por campo de createPromoProgramaSchema:**

```
titulo:
    z.string({ required_error: 'El titulo es obligatorio' })
    .min(1, 'El titulo es obligatorio')
    .min(5, 'El titulo debe tener al menos 5 caracteres')
    .max(200, 'Maximo 200 caracteres')

descripcion:
    z.string().max(4000, 'Maximo 4000 caracteres').optional().or(z.literal(''))

tipoPromoId:
    z.number({
        required_error: 'El tipo de programa es obligatorio',
        invalid_type_error: 'El tipo de programa es obligatorio'
    }).int().min(1, 'El tipo de programa es obligatorio')

campaniaCrowdfundingId:
    z.string().uuid().optional()

proyectoArtisticoId:
    z.string().uuid().optional()

urlLanding:
    z.string()
    .url('La URL de landing no tiene formato valido')
    .max(500, 'Maximo 500 caracteres')
    .optional()
    .or(z.literal(''))

codigoTrackingBase:
    z.string()
    .max(50, 'Maximo 50 caracteres')
    .regex(/^[a-zA-Z0-9-]*$/, 'Solo letras, numeros y guiones')
    .optional()
    .or(z.literal(''))

monedaId:
    z.number({
        required_error: 'La moneda es obligatoria',
        invalid_type_error: 'La moneda es obligatoria'
    }).int().min(1, 'La moneda es obligatoria')

importeComisionPorcentaje:
    z.number()
    .min(0, 'No puede ser negativo')
    .max(100, 'No puede superar el 100%')
    .optional()

importeComisionFija:
    z.number().min(0, 'No puede ser negativa').optional()

fechaInicio:
    z.string().optional()

fechaFin:
    z.string().optional()

tareas:
    z.array(createPromoTareaSchema).optional().default([])

Refine 1 - al menos una comision:
    .refine(
        (data) => data.importeComisionPorcentaje != null || data.importeComisionFija != null,
        {
            message: 'Debe definir al menos una comision (porcentaje o fija)',
            path: ['importeComisionPorcentaje']
        }
    )

Refine 2 - fechas:
    .refine(
        (data) => {
            if (data.fechaFin && data.fechaInicio) {
                return data.fechaFin > data.fechaInicio;
            }
            return true;
        },
        { message: 'La fecha fin debe ser posterior a la fecha inicio', path: ['fechaFin'] }
    )
```

**updatePromoProgramaSchema:**

```
createPromoProgramaSchema.omit({ tareas: true }).extend({
    tareas: z.array(updatePromoTareaSchema).optional().default([])
})
```

Los 2 refines se heredan automaticamente via el patron .omit().extend() sobre el schema base.

**Nota critica sobre el patron .omit().extend() con refines:** En Zod, cuando se usa `.omit()` sobre un schema con `.refine()` adjunto, los refines se pierden. La implementacion correcta requiere una de estas dos estrategias:

- **Estrategia A (recomendada):** Definir el schema base sin tareas y sin refines. Luego crear una funcion factory que aplique los refines al schema resultante, usada tanto para create como update.
- **Estrategia B (alternativa):** Repetir los refines explicitamente en updatePromoProgramaSchema.

El implementador debe elegir la estrategia. La Estrategia A es mas mantenible. El plan documenta el schema resultante esperado; el mecanismo exacto es decision de implementacion.

### 3.2 Types inferidos a exportar

| Type | Derivado de | Uso |
|------|-------------|-----|
| `CreatePromoProgramaFormData` | `z.infer<typeof createPromoProgramaSchema>` | Tipo del formulario en el wizard paso 1+2+3 |
| `UpdatePromoProgramaFormData` | `z.infer<typeof updatePromoProgramaSchema>` | Tipo del formulario de edicion |

Los types `CreatePromoTareaFormData` y `UpdatePromoTareaFormData` no son necesarios exportarlos por separado: el componente de tarea puede usar `CreatePromoProgramaFormData['tareas'][number]` para tipar items individuales. Se puede exportar si el implementador lo considera util.

### 3.3 Patron del helper interno

Reutilizar el `urlOpcionalSchema` ya definido en el archivo para `urlLanding` y `urlInstrucciones`, pasando el nombre del campo. La funcion ya tiene `.max(300)` pero los campos de programa requieren max 500. Opciones:

- Crear un segundo helper: `const urlOpcionaMaxSchema = (nombre: string) => z.string().url(...).max(500, ...).optional().or(z.literal(''))`
- Inline los campos que necesiten max 500

El implementador decide. Documentar la eleccion con un comentario.

---

## 4. Archivo 3: Modificar `src/shared/constants/index.ts`

**Operacion:** AGREGAR bloques en tres secciones distintas del archivo existente.

### 4.1 QUERY_KEYS - Agregar dentro del bloque `crowdpromotion` existente

El bloque actual es:
```
crowdpromotion: {
    promotor: { me: [...] },
    maestras: { tiposPromotor: [...] }
}
```

Despues de agregar US-CP-02 debe quedar:
```
crowdpromotion: {
    promotor: {
        me: ['crowdpromotion', 'promotor', 'me'] as const,
    },
    maestras: {
        tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
        tiposPromo: ['crowdpromotion', 'maestras', 'tipos-promo'] as const,
        tiposEventoPromo: ['crowdpromotion', 'maestras', 'tipos-evento-promo'] as const,
        tiposRewardPromo: ['crowdpromotion', 'maestras', 'tipos-reward-promo'] as const,
    },
    programas: {
        mis: ['crowdpromotion', 'programas', 'mis'] as const,
        byId: (id: string) => ['crowdpromotion', 'programas', id] as const,
        misFiltrados: (filters: Record<string, unknown>) =>
            ['crowdpromotion', 'programas', 'mis', filters] as const,
    },
},
```

Cambios respecto al estado actual:
- En `maestras`: agregar `tiposPromo`, `tiposEventoPromo`, `tiposRewardPromo`
- Agregar subobjeto `programas` completo con `mis`, `byId`, `misFiltrados`

### 4.2 API_ROUTES - Agregar dentro del bloque `crowdpromotion` existente

El bloque actual es:
```
crowdpromotion: {
    promotor: { base, me, desactivar },
    maestras: { tiposPromotor }
}
```

Despues de agregar US-CP-02 debe quedar:
```
crowdpromotion: {
    promotor: {
        base: '/api/crowdpromotion/promotor',
        me: '/api/crowdpromotion/promotor/me',
        desactivar: '/api/crowdpromotion/promotor/me/desactivar',
    },
    maestras: {
        tiposPromotor: '/api/crowdpromotion/maestras/tipos-promotor',
        tiposPromo: '/api/crowdpromotion/maestras/tipos-promo',
        tiposEventoPromo: '/api/crowdpromotion/maestras/tipos-evento-promo',
        tiposRewardPromo: '/api/crowdpromotion/maestras/tipos-reward-promo',
    },
    programas: {
        base: '/api/crowdpromotion/programas',
        mis: '/api/crowdpromotion/programas/mis-programas',
        byId: (id: string) => `/api/crowdpromotion/programas/${id}`,
        desactivar: (id: string) => `/api/crowdpromotion/programas/${id}/desactivar`,
    },
},
```

Cambios respecto al estado actual:
- En `maestras`: agregar `tiposPromo`, `tiposEventoPromo`, `tiposRewardPromo`
- Agregar subobjeto `programas` completo con `base`, `mis`, `byId`, `desactivar`

**Mapeo endpoint -> constante:**

| Endpoint | Metodo | Constante API_ROUTES |
|----------|--------|---------------------|
| POST /api/crowdpromotion/programas | POST | `API_ROUTES.crowdpromotion.programas.base` |
| GET /api/crowdpromotion/programas/mis-programas | GET | `API_ROUTES.crowdpromotion.programas.mis` |
| GET /api/crowdpromotion/programas/{id} | GET | `API_ROUTES.crowdpromotion.programas.byId(id)` |
| PUT /api/crowdpromotion/programas/{id} | PUT | `API_ROUTES.crowdpromotion.programas.byId(id)` |
| PATCH /api/crowdpromotion/programas/{id}/desactivar | PATCH | `API_ROUTES.crowdpromotion.programas.desactivar(id)` |

### 4.3 APP_ROUTES - Agregar dentro del bloque `dashboard` existente

El bloque actual de `dashboard` tiene `campanias`, `crowdsourcing`, `rewards`. Agregar subobjeto `crowdpromotion`:

```
dashboard: {
    root: "/dashboard",
    campanias: { ... },      // existente
    crowdsourcing: { ... },  // existente
    rewards: { ... },        // existente
    crowdpromotion: {        // NUEVO
        programas: {
            list: '/dashboard/crowdpromotion/programas',
            nuevo: '/dashboard/crowdpromotion/programas/nuevo',
            detalle: (id: string) => `/dashboard/crowdpromotion/programas/${id}`,
            editar: (id: string) => `/dashboard/crowdpromotion/programas/${id}/editar`,
        },
    },
},
```

### 4.4 VALIDATION - Agregar limites nuevos

Dentro del objeto `VALIDATION` existente, agregar al final del bloque:

```typescript
// Programa de Promocion
PROGRAMA_TITULO_MIN: 5,
PROGRAMA_TITULO_MAX: 200,
PROGRAMA_DESCRIPCION_MAX: 4000,
PROGRAMA_URL_LANDING_MAX: 500,
PROGRAMA_CODIGO_TRACKING_MAX: 50,
TAREA_TITULO_MIN: 3,
TAREA_TITULO_MAX: 200,
TAREA_DESCRIPCION_MAX: 4000,
TAREA_URL_INSTRUCCIONES_MAX: 500,
COMISION_PORCENTAJE_MAX: 100,
PROGRAMAS_PAGE_SIZE_MAX: 50,
```

### 4.5 Constantes de dominio - Agregar al final del archivo

Agregar en el area "Crowdpromotion Domain Constants", a continuacion del bloque TIPO_PROMOTOR existente:

**TIPO_PROMO:**
```typescript
export const TIPO_PROMO = {
    REFERRAL:   1,
    AFILIADO:   2,
    INFLUENCER: 3,
    MIXTO:      4,
} as const;

export const TIPO_PROMO_LABELS: Record<number, string> = {
    1: 'Referral',
    2: 'Afiliado',
    3: 'Influencer',
    4: 'Mixto',
};

export const TIPO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Programa de referidos: comision por cada nuevo backer referido',
    2: 'Programa de afiliados: comision por ventas generadas',
    3: 'Programa para influencers: tareas de contenido con recompensa',
    4: 'Combinacion de referral + tareas de contenido',
};
```

**TIPO_EVENTO_PROMO:**
```typescript
export const TIPO_EVENTO_PROMO = {
    CLICK:     1,
    PAGE_VIEW: 2,
    SHARE:     3,
    POST:      4,
    SIGNUP:    5,
    BACKING:   6,
} as const;

export const TIPO_EVENTO_PROMO_LABELS: Record<number, string> = {
    1: 'Click',
    2: 'PageView',
    3: 'Share',
    4: 'Post',
    5: 'Signup',
    6: 'Backing',
};

export const TIPO_EVENTO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Click en enlace de referido',
    2: 'Visita a la pagina de la campana',
    3: 'Compartir en redes sociales',
    4: 'Publicacion original sobre la campana',
    5: 'Registro de nuevo usuario referido',
    6: 'Aportacion/backing a la campana (conversion)',
};
```

**TIPO_REWARD_PROMO:**
```typescript
export const TIPO_REWARD_PROMO = {
    DINERO: 1,
    PUNTOS: 2,
    MIXTO:  3,
} as const;

export const TIPO_REWARD_PROMO_LABELS: Record<number, string> = {
    1: 'Dinero',
    2: 'Puntos',
    3: 'Mixto',
};

export const TIPO_REWARD_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Recompensa monetaria',
    2: 'Recompensa en puntos canjeables',
    3: 'Dinero + puntos',
};
```

**Nota de naming:** El sufijo `_PROMO` es obligatorio para diferenciar de `TIPO_REWARD` (campanias) que tiene IDs 1-4 con semantica distinta (Digital, Fisico, Experiencia, Otro).

---

## 5. Archivo 4: Modificar `src/shared/utils/error-messages.ts`

**Operacion:** AGREGAR al final del archivo existente, despues del bloque `getPromotorErrorMessage`.

### 5.1 Codigos numericos a agregar en `ERROR_CODE_MESSAGES`

Los siguientes codigos no existen aun en `ERROR_CODE_MESSAGES` y deben agregarse:

| Codigo | Mensaje global | Nota |
|--------|----------------|------|
| `'1020'` | `'Debe definir al menos una comision (porcentaje o fija)'` | Nuevo para US-CP-02 |
| `'1021'` | `'El valor esta fuera del rango permitido'` | Nuevo para US-CP-02 |
| `'1022'` | `'La fecha fin debe ser posterior a la fecha inicio'` | Nuevo - conflicto potencial con 1012 pero semantica diferente |
| `'1023'` | `'El formato del campo no es valido'` | Nuevo - nota: 1013 ya existe con URL; 1023 es formato general |
| `'1024'` | `'Este codigo de tracking ya esta en uso. Elige otro'` | Nuevo para US-CP-02 |
| `'1025'` | `'Las tareas repetibles deben tener un numero maximo de repeticiones'` | Nuevo para US-CP-02 |
| `'1026'` | `'El importe de recompensa es requerido para recompensas monetarias'` | Nuevo para US-CP-02 |
| `'1027'` | `'Los puntos de recompensa son requeridos para recompensas de puntos'` | Nuevo para US-CP-02 |
| `'2016'` | `'No tienes un perfil de artista. Crea tu perfil primero'` | Nuevo para US-CP-02 |
| `'2019'` | `'El programa de promocion no existe o fue eliminado'` | Nuevo para US-CP-02 |
| `'4020'` | `'Este programa ya esta desactivado'` | Nuevo para US-CP-02 |
| `'4021'` | `'No se puede desactivar una tarea que ya tiene completados registrados'` | Nuevo para US-CP-02 |

**Importante:** Los codigos 1020-1027 van en el bloque `// Validation errors (1000-1999)` del objeto `ERROR_CODE_MESSAGES`. Los codigos 2016 y 2019 van en el bloque `// Not Found errors (2000-2999)`. Los codigos 4020 y 4021 van en el bloque `// Business Rule errors (4000-4999)`.

### 5.2 Nuevo objeto y funcion de error messages para programas de promocion

Agregar al final del archivo, despues de `getPromotorErrorMessage`:

```
export const PROMO_PROGRAMA_ERROR_MESSAGES: Record<string, string> = {
    // Numeric codes (override global codes for this context)
    '1001': 'Completa los campos obligatorios: titulo, tipo de programa y moneda',
    '1011': 'El titulo debe tener al menos 5 caracteres',
    '1020': 'Debes definir al menos una comision (porcentaje o fija)',
    '1021': 'El valor esta fuera del rango permitido',
    '1022': 'La fecha fin debe ser posterior a la fecha inicio',
    '1023': 'El formato del campo no es valido (solo letras, numeros y guiones)',
    '1024': 'Este codigo de tracking ya esta en uso para otro programa. Elige otro',
    '1025': 'Las tareas repetibles deben tener un numero maximo de repeticiones definido',
    '1026': 'El importe de recompensa es requerido para recompensas monetarias',
    '1027': 'Los puntos de recompensa son requeridos para recompensas de puntos',
    '2016': 'No tienes un perfil de artista. Crea tu perfil de artista primero',
    '2019': 'El programa de promocion no existe o fue eliminado',
    '3002': 'No tienes permiso para realizar esta accion sobre este programa',
    '4020': 'Este programa ya esta desactivado',
    '4021': 'No se puede desactivar esta tarea porque ya tiene completados registrados',
    '5000': 'Ha ocurrido un error inesperado al procesar el programa. Por favor, intenta nuevamente',

    // Semantic keys para uso interno en hooks y componentes
    PROGRAMA_NOT_FOUND:              'El programa de promocion no existe o fue eliminado',
    PROGRAMA_ALREADY_INACTIVE:       'Este programa ya esta desactivado',
    PROGRAMA_TAREA_CON_COMPLETADOS:  'No se puede desactivar una tarea que ya tiene completados registrados',
    PROGRAMA_COMISION_REQUERIDA:     'Debes definir al menos una comision (porcentaje o fija)',
    PROGRAMA_CODIGO_TRACKING_DUPLICADO: 'Este codigo de tracking ya esta en uso. Elige otro',
    ARTISTA_NOT_FOUND:               'No tienes un perfil de artista. Crea tu perfil primero',
    CAMPANA_NOT_BELONGS_TO_ARTISTA:  'No tienes permiso para vincular esta campana a tu programa',
} as const;

export const getPromoProgramaErrorMessage = (errorCode: string): string => {
    return PROMO_PROGRAMA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### 5.3 Tabla completa de nuevos mensajes de error

| Codigo / Key | Mensaje UI | Contexto |
|--------------|-----------|---------|
| `'1020'` | Debes definir al menos una comision (porcentaje o fija) | Paso 2 del wizard, ambas comisiones vacias |
| `'1021'` | El valor esta fuera del rango permitido | Comision % fuera de 0-100 |
| `'1022'` | La fecha fin debe ser posterior a la fecha inicio | Campos de fecha en paso 1 |
| `'1023'` | El formato del campo no es valido | CodigoTrackingBase con caracteres invalidos |
| `'1024'` | Este codigo de tracking ya esta en uso. Elige otro | CodigoTrackingBase duplicado por artista |
| `'1025'` | Las tareas repetibles deben tener max repeticiones definido | EsRepetible true sin MaxRepeticiones |
| `'1026'` | El importe de recompensa es requerido para recompensas monetarias | TipoReward = Dinero/Mixto sin importe |
| `'1027'` | Los puntos de recompensa son requeridos para recompensas de puntos | TipoReward = Puntos/Mixto sin puntos |
| `'2016'` | No tienes un perfil de artista | Artista no encontrado en JWT |
| `'2019'` | El programa de promocion no existe o fue eliminado | PromoPrograma no encontrado por ID |
| `'4020'` | Este programa ya esta desactivado | Intentar desactivar programa ya inactivo |
| `'4021'` | No se puede desactivar esta tarea con completados | Tarea con registros en PromoTareaPromotor |
| `PROGRAMA_NOT_FOUND` | El programa de promocion no existe o fue eliminado | Hook usePromoPrograma, 404 |
| `PROGRAMA_ALREADY_INACTIVE` | Este programa ya esta desactivado | Hook useDesactivarPromoPrograma, 4020 |
| `PROGRAMA_TAREA_CON_COMPLETADOS` | No se puede desactivar tarea con completados | Hook useUpdatePromoPrograma, 4021 |
| `PROGRAMA_COMISION_REQUERIDA` | Debes definir al menos una comision | Validacion wizard paso 2 |
| `PROGRAMA_CODIGO_TRACKING_DUPLICADO` | Este codigo ya esta en uso | Backend retorna 1024 |
| `ARTISTA_NOT_FOUND` | No tienes un perfil de artista | Redirect a crear perfil de artista |
| `CAMPANA_NOT_BELONGS_TO_ARTISTA` | Sin permiso para vincular esta campana | Backend retorna 3002 en create/update |

---

## 6. Archivos a Crear/Modificar

```
src/shared/
├── types/
│   └── crowdpromotion.ts          <- MODIFICAR (agregar)
├── schemas/
│   └── crowdpromotion.schema.ts   <- MODIFICAR (agregar)
├── constants/
│   └── index.ts                   <- MODIFICAR (agregar en 3 secciones)
└── utils/
    └── error-messages.ts          <- MODIFICAR (agregar al final)
```

No se crea ningun archivo nuevo. Todos los cambios son adiciones a archivos existentes.

---

## 7. Orden de Implementacion

El orden recomendado minimiza dependencias durante la implementacion:

1. **`src/shared/types/crowdpromotion.ts`** - Sin dependencias externas. Agregar todas las interfaces y union types nuevos.

2. **`src/shared/constants/index.ts`** - Sin dependencias externas. Agregar TIPO_PROMO, TIPO_EVENTO_PROMO, TIPO_REWARD_PROMO, limites de validacion, y las secciones de QUERY_KEYS, API_ROUTES y APP_ROUTES.

3. **`src/shared/schemas/crowdpromotion.schema.ts`** - Depende de que los tipos existan (para z.infer). Agregar los 4 schemas y los 2 types inferidos.

4. **`src/shared/utils/error-messages.ts`** - Sin dependencias del shared. Agregar los codigos numericos en ERROR_CODE_MESSAGES y el bloque PROMO_PROGRAMA_ERROR_MESSAGES con su funcion getter.

---

## 8. Dependencias

- `zod` - Ya instalado en `src/shared/node_modules/zod`
- Ningun package adicional requerido

---

## 9. Verificacion de Consistencia con el Modulo Existente

### Patrones heredados de Promotor

| Patron | Promotor (US-CP-01) | PromoPrograma (US-CP-02) |
|--------|---------------------|--------------------------|
| Sufijo Result en creates | `PromotorCreatedResult` | `PromoProgramaCreatedResult` |
| Sufijo Result en updates | `PromotorUpdatedResult` | `PromoProgramaUpdatedResult` |
| Sufijo Result en desactivar | `PromotorDesactivadoResult` | `PromoProgramaDesactivadoResult` |
| Union type de estado | `PromotorEstado` | `PromoProgramaEstado` |
| Schema create | `createPromotorSchema` | `createPromoProgramaSchema` |
| Schema update | `updatePromotorSchema` | `updatePromoProgramaSchema` |
| Type form create | `CreatePromotorFormData` | `CreatePromoProgramaFormData` |
| Type form update | `UpdatePromotorFormData` | `UpdatePromoProgramaFormData` |
| Error messages | `PROMOTOR_ERROR_MESSAGES` | `PROMO_PROGRAMA_ERROR_MESSAGES` |
| Error getter | `getPromotorErrorMessage` | `getPromoProgramaErrorMessage` |
| QUERY_KEY me | `crowdpromotion.promotor.me` | `crowdpromotion.programas.mis` |
| QUERY_KEY byId | No aplica (perfil unico) | `crowdpromotion.programas.byId(id)` |

### Colisiones de nombres a evitar

| Nombre conflictivo | Modulo existente | Nombre correcto para US-CP-02 |
|-------------------|------------------|-------------------------------|
| `TIPO_REWARD` | Campanias (Digital=1, Fisico=2, Experiencia=3, Otro=4) | `TIPO_REWARD_PROMO` |
| `TipoReward` | Si existe en types de campaña | `TipoRewardPromo` |
| `MONEDAS` | Ya existe con EUR=1, USD=2 | Reusar `MONEDAS` existente; no crear `MONEDA_PROMO` |

### Campos de fecha: convencion ISO date string

Tanto en el tipo de request como en los DTOs de response, las fechas se representan como `string` en TypeScript. El backend usa `DateOnly` en C# que serializa como `"YYYY-MM-DD"`. Los campos de datetime (fechaCreacion, fechaActualizacion, etc.) también son `string` con formato ISO 8601 completo (`"2026-02-25T10:00:00Z"`). Esta convencion es consistente con los tipos de Promotor ya definidos.

---

## 10. Notas de Implementacion

1. **No crear archivos nuevos.** Todos los tipos, schemas, constantes y mensajes de error para PromoPrograma se agregan a los archivos existentes de Crowdpromotion. El modulo comparte un unico archivo de tipos y un unico archivo de schemas.

2. **Separadores de secciones.** Usar el patron `// ========== Titulo ==========` ya establecido en todos los archivos para demarcar las nuevas secciones de PromoPrograma.

3. **Refines de Zod con chain complejo.** El schema de tarea encadena 3 `.refine()` consecutivos. Cada uno tiene su `path` correcto para que React Hook Form posicione el error en el campo correcto del formulario.

4. **El campo `esRepetible` tiene `default(true)`.** Esto significa que si el formulario no envia el campo, Zod asignara `true`. El implementador debe asegurarse de que el form inicializa este campo correctamente.

5. **Strings vacios vs undefined en URLs.** Usar `.or(z.literal(''))` en todos los campos URL opcionales para permitir que el usuario borre el campo en el formulario. Antes del submit, el hook debe transformar strings vacios a `undefined` para que el backend no reciba strings vacios que fallarian la validacion de formato URL.

6. **CodigoTrackingBase: el regex `/^[a-zA-Z0-9-]*$/` permite string vacio.** Si el campo es opcional y el usuario no lo rellena, el regex pasa con string vacio. La combinacion `.optional().or(z.literal(''))` con el regex es correcta; si el campo tiene valor, el regex valida; si esta vacio o undefined, pasa como opcional.

7. **`campaniaCrowdfundingId` y `proyectoArtisticoId` como `z.string().uuid().optional()`.** Si el form envia string vacio, `.uuid()` fallaria. Agregar `.or(z.literal(''))` igual que en las URLs, y transformar a undefined antes del POST.

8. **`misFiltrados` en QUERY_KEYS.** Este key acepta `Record<string, unknown>` como filtros para invalidacion granular. El hook `useMisProgramas` puede pasar `{ esActivo: true }` o `{ esActivo: false }` o `{}` (todos). Si los filtros cambian, TanStack Query refetchea automaticamente.

9. **Constantes de maestras sin endpoints en MVP.** Segun las notas del contracts.md, los selects de `tipoPromoId`, `tipoEventoPromoId` y `tipoRewardId` pueden poblarse con las constantes `TIPO_PROMO_LABELS`, `TIPO_EVENTO_PROMO_LABELS` y `TIPO_REWARD_PROMO_LABELS` sin hacer llamadas a endpoints de maestras. Las rutas `API_ROUTES.crowdpromotion.maestras.tiposPromo`, etc. se definen por completitud del contrato pero pueden no usarse en el MVP.

10. **`PromoProgramaResumen.valorTotalGenerado`.** Este campo puede ser `0` en MVP si las tablas de eventos no existen. El tipo TypeScript es `number` (no nullable) porque el backend siempre devuelve un valor (aunque sea 0).

---

## 11. Checklist

- [ ] Types creados y exportados en `src/shared/types/crowdpromotion.ts`
    - [ ] CreatePromoTareaItem
    - [ ] UpdatePromoTareaItem (extiende CreatePromoTareaItem)
    - [ ] CreatePromoProgramaRequest
    - [ ] UpdatePromoProgramaRequest (extiende via Omit)
    - [ ] PromoProgramaCreatedResult
    - [ ] PromoProgramaListItem
    - [ ] PromoProgramaListResult
    - [ ] PromoTareaDetail
    - [ ] PromoProgramaPromotorSummary
    - [ ] PromoProgramaResumen
    - [ ] PromoProgramaDetail
    - [ ] PromoProgramaUpdatedResult
    - [ ] PromoProgramaDesactivadoResult
    - [ ] TipoPromo (maestra)
    - [ ] TipoEventoPromo (maestra)
    - [ ] TipoRewardPromo (maestra)
    - [ ] PromoProgramaEstado (union type)
- [ ] Schemas Zod con mensajes en espanol en `src/shared/schemas/crowdpromotion.schema.ts`
    - [ ] createPromoTareaSchema (interno, no exportado)
    - [ ] updatePromoTareaSchema (interno, no exportado)
    - [ ] createPromoProgramaSchema (exportado)
    - [ ] updatePromoProgramaSchema (exportado)
    - [ ] CreatePromoProgramaFormData (type inferido exportado)
    - [ ] UpdatePromoProgramaFormData (type inferido exportado)
    - [ ] Refine al menos una comision validado
    - [ ] Refine fechaFin > fechaInicio validado
    - [ ] Refine esRepetible con maxRepeticiones validado
    - [ ] Refines de tipoRewardId con importeRecompensa y puntosRecompensa validados
- [ ] Constantes en `src/shared/constants/index.ts`
    - [ ] QUERY_KEYS.crowdpromotion.maestras extendido (tiposPromo, tiposEventoPromo, tiposRewardPromo)
    - [ ] QUERY_KEYS.crowdpromotion.programas agregado (mis, byId, misFiltrados)
    - [ ] API_ROUTES.crowdpromotion.maestras extendido
    - [ ] API_ROUTES.crowdpromotion.programas agregado (5 endpoints)
    - [ ] APP_ROUTES.dashboard.crowdpromotion.programas agregado (4 rutas)
    - [ ] VALIDATION extendido con 10 limites nuevos
    - [ ] TIPO_PROMO con LABELS y DESCRIPTIONS
    - [ ] TIPO_EVENTO_PROMO con LABELS y DESCRIPTIONS
    - [ ] TIPO_REWARD_PROMO con LABELS y DESCRIPTIONS
- [ ] Mensajes de error en `src/shared/utils/error-messages.ts`
    - [ ] Codigos 1020-1027 en ERROR_CODE_MESSAGES
    - [ ] Codigos 2016, 2019 en ERROR_CODE_MESSAGES
    - [ ] Codigos 4020, 4021 en ERROR_CODE_MESSAGES
    - [ ] PROMO_PROGRAMA_ERROR_MESSAGES con codigos numericos y keys semanticos
    - [ ] getPromoProgramaErrorMessage exportado
- [ ] Sin colisiones de nombres con modulos existentes (TIPO_REWARD vs TIPO_REWARD_PROMO)
- [ ] Sin archivos nuevos creados (todo es adicion a archivos existentes)
