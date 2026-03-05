# Plan de Contratos Shared: cp-inscripcion-programa

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Basado en:** docs/user-stories/cp-inscripcion-programa/contracts.md

---

## 1. Resumen

- **Total de types nuevos:** 10 interfaces y 1 union type a agregar en `src/shared/types/crowdpromotion.ts`
- **Total de schemas nuevos:** 2 schemas Zod a agregar en `src/shared/schemas/crowdpromotion.schema.ts`
- **Constantes nuevas:** 3 bloques en `src/shared/constants/index.ts` (QUERY_KEYS, API_ROUTES, APP_ROUTES + INSCRIPCION_ESTADO + VALIDATION)
- **Utilidades nuevas:** 4 error messages y 1 funcion getter en `src/shared/utils/error-messages.ts`, 2 mappers en `src/shared/utils/mappers.ts`

### Estado Actual

Los archivos de `src/shared/` para el modulo crowdpromotion **YA EXISTEN** de features anteriores (US-CP-01 y US-CP-02). Este plan documenta exclusivamente las **adiciones** necesarias para US-CP-03. No se modifica ni elimina nada existente.

### Archivos Afectados

| Archivo | Operacion | Descripcion |
|---------|-----------|-------------|
| `src/shared/types/crowdpromotion.ts` | MODIFICAR (agregar al final) | 10 interfaces + 1 union type |
| `src/shared/schemas/crowdpromotion.schema.ts` | MODIFICAR (agregar al final) | 2 schemas Zod + 2 types inferidos |
| `src/shared/constants/index.ts` | MODIFICAR (ampliar secciones existentes) | QUERY_KEYS, API_ROUTES, APP_ROUTES, INSCRIPCION_ESTADO |
| `src/shared/utils/error-messages.ts` | MODIFICAR (agregar al final) | INSCRIPCION_ERROR_MESSAGES + getter |
| `src/shared/utils/mappers.ts` | MODIFICAR (agregar al final) | 2 mappers de inscripcion |

---

## 2. Types (`src/shared/types/crowdpromotion.ts`)

**Operacion:** Agregar al final del archivo existente. No modificar nada existente.

### 2.1 Union Type de Estado de Inscripcion

| Tipo | Valores | Descripcion |
|------|---------|-------------|
| `InscripcionEstado` | `'Pendiente' \| 'Aprobado' \| 'Bloqueado' \| 'DadoDeBaja'` | Estado calculado server-side de una inscripcion. Alineado con los valores literales del campo `estado` en los DTOs de respuesta. PascalCase porque los valores vienen directamente del backend como strings. |

**Logica de derivacion del estado (documentar en comentario):**

| EsAprobado | EsBloqueado | FechaBaja | Estado |
|------------|-------------|-----------|--------|
| false | false | null | `'Pendiente'` |
| true | false | null | `'Aprobado'` |
| false | true | null | `'Bloqueado'` |
| false | false | valor | `'DadoDeBaja'` |

### 2.2 DTOs de Response - Vista Promotor (Explorar)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `ProgramaExplorarItem` | `id: string`, `titulo: string`, `artistaNombre: string`, `tipoPromoId: number`, `tipoPromoNombre: string`, `importeComisionPorcentaje: number \| null`, `importeComisionFija: number \| null`, `monedaNombre: string \| null`, `numeroTareas: number`, `campaniaTitulo: string \| null`, `fechaInicio: string \| null`, `fechaFin: string \| null`, `miEstado: InscripcionEstado \| null` | Item del catalogo publico de programas. `miEstado` es `null` cuando el promotor no tiene inscripcion. |
| `ProgramasExplorarResponse` | `items: ProgramaExplorarItem[]`, `totalCount: number`, `page: number`, `pageSize: number`, `totalPages: number` | Respuesta paginada del endpoint GET /explorar. |

### 2.3 DTOs de Response - Solicitar Inscripcion (Promotor)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `InscripcionCreada` | `id: string`, `programaId: string`, `programaTitulo: string`, `esAprobado: boolean`, `esBloqueado: boolean`, `fechaAlta: string` | Respuesta del POST de solicitud. `esAprobado` y `esBloqueado` siempre `false` en creacion. `fechaAlta` es ISO datetime string mapeado desde `FechaInscripcion` en la entidad. |

### 2.4 DTOs de Response - Mis Programas (Promotor)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `TareaResumen` | `id: string`, `titulo: string`, `descripcion: string \| null`, `tipoEventoPromoNombre: string`, `importeRecompensa: number \| null`, `monedaNombre: string \| null`, `esRepetible: boolean`, `maxRepeticiones: number \| null`, `orden: number` | Tarea resumida incluida en `MiInscripcion`. Solo tareas activas. |
| `MiInscripcion` | `id: string`, `programaId: string`, `programaTitulo: string`, `artistaNombre: string`, `tipoPromoNombre: string`, `importeComisionPorcentaje: number \| null`, `importeComisionFija: number \| null`, `monedaNombre: string \| null`, `esAprobado: boolean`, `esBloqueado: boolean`, `codigoReferido: string \| null`, `urlTrackingPersonalizada: string \| null`, `fechaAlta: string`, `fechaBaja: string \| null`, `estado: InscripcionEstado`, `tareas: TareaResumen[]` | Inscripcion del promotor. `codigoReferido` y `urlTrackingPersonalizada` son `null` a menos que `esAprobado == true && fechaBaja == null`. `tareas` es `[]` en estados no aprobados. |
| `MisProgramasResponse` | `items: MiInscripcion[]`, `totalCount: number`, `page: number`, `pageSize: number`, `totalPages: number` | Respuesta paginada de GET /promotor/mis-programas. |

### 2.5 DTOs de Response - Gestion de Inscripciones (Artista)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `InscripcionAprobada` | `id: string`, `promotorNombre: string`, `esAprobado: boolean`, `esBloqueado: boolean`, `codigoReferido: string`, `urlTrackingPersonalizada: string \| null` | Respuesta del PATCH /aprobar. `codigoReferido` es siempre no-null tras aprobar. `urlTrackingPersonalizada` puede ser null si `PromoPrograma.UrlLanding` era null. |
| `InscripcionRechazada` | `inscripcionId: string`, `promotorNombre: string` | Respuesta del PATCH /rechazar. Minima porque el registro fue eliminado fisicamente. |
| `InscripcionBloqueada` | `id: string`, `promotorNombre: string`, `esBloqueado: boolean`, `esAprobado: boolean` | Respuesta del PATCH /bloquear. `esBloqueado` siempre `true`, `esAprobado` siempre `false`. |
| `InscripcionDadaDeBaja` | `id: string`, `promotorNombre: string`, `esAprobado: boolean`, `fechaBaja: string` | Respuesta del PATCH /dar-de-baja. `esAprobado` siempre `false`. `fechaBaja` es ISO datetime string. |
| `InscripcionListItem` | `id: string`, `promotorId: string`, `promotorNombre: string`, `tipoPromotorNombre: string`, `promotorEmailContacto: string \| null`, `promotorUrlInstagram: string \| null`, `promotorUrlTikTok: string \| null`, `promotorUrlSitioWeb: string \| null`, `esAprobado: boolean`, `esBloqueado: boolean`, `codigoReferido: string \| null`, `fechaAlta: string`, `fechaBaja: string \| null`, `estado: InscripcionEstado` | Item de la lista de inscripciones del artista. Incluye datos del perfil publico del promotor para mostrar en la UI. |
| `InscripcionesListResponse` | `items: InscripcionListItem[]`, `totalCount: number`, `page: number`, `pageSize: number`, `totalPages: number` | Respuesta paginada de GET /inscripciones. |

### 2.6 DTOs de Filtros (Request params)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `ExplorarProgramasFilters` | `artistaNombre?: string`, `tipoPromoId?: number`, `page?: number`, `pageSize?: number` | Query params para GET /explorar. Todos opcionales. |
| `InscripcionesFilters` | `estado?: InscripcionEstado`, `page?: number`, `pageSize?: number` | Query params para GET /inscripciones del artista. |

### 2.7 Notas sobre Tipos Existentes

El tipo `PromoProgramaPromotorSummary` ya existe en `crowdpromotion.ts` (de US-CP-02) con propiedades `id, promotorNombre, tipoPromotorNombre, esAprobado, esBloqueado, fechaAlta`. Este tipo es diferente a `InscripcionListItem`: el summary es solo para el bloque compacto dentro de `PromoProgramaDetail`, mientras que `InscripcionListItem` es el tipo completo con contacto del promotor para la vista de gestion del artista. **No reemplazar ni extender `PromoProgramaPromotorSummary`.**

---

## 3. Schemas Zod (`src/shared/schemas/crowdpromotion.schema.ts`)

**Operacion:** Agregar al final del archivo existente. No modificar nada existente.

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas |
|--------|--------|--------|
| `explorarProgramasFiltersSchema` | `artistaNombre?`, `tipoPromoId?`, `page?`, `pageSize?` | `artistaNombre`: string max 200 opcional; `tipoPromoId`: number int positivo opcional; `page`: number int min 1 opcional; `pageSize`: number int min 1 max 50 opcional |
| `inscripcionesFiltersSchema` | `estado?`, `page?`, `pageSize?` | `estado`: enum `['Pendiente', 'Aprobado', 'Bloqueado', 'DadoDeBaja']` opcional; `page`: number int min 1 opcional; `pageSize`: number int min 1 max 50 opcional |

**Detalle de reglas por campo:**

```
explorarProgramasFiltersSchema:
  artistaNombre:
    - z.string()
    - .max(200, 'Maximo 200 caracteres')
    - .optional()

  tipoPromoId:
    - z.number()
    - .int()
    - .positive('El tipo de programa debe ser un numero positivo')
    - .optional()

  page:
    - z.number()
    - .int()
    - .min(1, 'La pagina debe ser mayor a 0')
    - .optional()

  pageSize:
    - z.number()
    - .int()
    - .min(1, 'El tamano de pagina debe ser mayor a 0')
    - .max(50, 'El tamano de pagina no puede superar 50')
    - .optional()

inscripcionesFiltersSchema:
  estado:
    - z.enum(['Pendiente', 'Aprobado', 'Bloqueado', 'DadoDeBaja'])
    - .optional()

  page:
    - z.number()
    - .int()
    - .min(1, 'La pagina debe ser mayor a 0')
    - .optional()

  pageSize:
    - z.number()
    - .int()
    - .min(1, 'El tamano de pagina debe ser mayor a 0')
    - .max(50, 'El tamano de pagina no puede superar 50')
    - .optional()
```

**Nota importante sobre requests sin body:** Los endpoints POST /inscripcion, PATCH /aprobar, PATCH /rechazar, PATCH /bloquear y PATCH /dar-de-baja NO tienen body. La validacion del `programaId` e `inscripcionId` (path params en UUID) es responsabilidad del router de React/Next.js y del model binding de ASP.NET Core. **No se necesitan schemas Zod adicionales para estos endpoints.**

### 3.2 Types Inferidos

| Type | Schema Origen | Uso |
|------|---------------|-----|
| `ExplorarProgramasFiltersData` | `z.infer<typeof explorarProgramasFiltersSchema>` | Tipo para los valores del formulario de filtros en la pagina de explorar |
| `InscripcionesFiltersData` | `z.infer<typeof inscripcionesFiltersSchema>` | Tipo para los valores del formulario de filtros en la lista de inscripciones del artista |

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Operacion:** Modificar el archivo existente en tres secciones. Todas las adiciones son aditivas; no se elimina ni reordena nada.

### 4.1 Adiciones a QUERY_KEYS.crowdpromotion

Dentro del objeto `crowdpromotion` existente se amplian dos sub-objetos:

**Ampliar `crowdpromotion.programas` (ya existe con `mis`, `byId`, `misFiltrados`):**

| Key | Patron | Uso |
|-----|--------|-----|
| `crowdpromotion.programas.explorar` | `(filters?: ExplorarProgramasFilters) => ['crowdpromotion', 'programas', 'explorar', filters] as const` | Cache para GET /programas/explorar con filtros. Los filtros se incluyen en la clave para que cambios de filtro invaliden solo su cache. |
| `crowdpromotion.programas.inscripciones` | `(programaId: string, filters?: InscripcionesFilters) => ['crowdpromotion', 'programas', programaId, 'inscripciones', filters] as const` | Cache para GET /programas/{id}/inscripciones del artista. Incluye `programaId` para granularidad por programa. |

**Agregar nuevo sub-objeto `crowdpromotion.inscripciones` (nuevo):**

| Key | Patron | Uso |
|-----|--------|-----|
| `crowdpromotion.inscripciones.misProgramas` | `(page?: number) => ['crowdpromotion', 'inscripciones', 'mis-programas', page] as const` | Cache para GET /promotor/mis-programas del promotor. |

### 4.2 Adiciones a API_ROUTES.crowdpromotion

Dentro del objeto `crowdpromotion.programas` existente (ya tiene `base, mis, byId, desactivar`) se agregan:

| Constante | Valor | Uso |
|-----------|-------|-----|
| `programas.explorar` | `'/api/crowdpromotion/programas/explorar'` | GET catalogo publico de programas para promotores |
| `programas.inscripcion` | `(programaId: string) => '/api/crowdpromotion/programas/${programaId}/inscripcion'` | POST solicitar inscripcion |
| `programas.inscripciones` | `(programaId: string) => '/api/crowdpromotion/programas/${programaId}/inscripciones'` | GET lista de inscripciones (artista) |
| `programas.aprobar` | `(programaId: string, inscripcionId: string) => '/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/aprobar'` | PATCH aprobar inscripcion |
| `programas.rechazar` | `(programaId: string, inscripcionId: string) => '/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/rechazar'` | PATCH rechazar inscripcion |
| `programas.bloquear` | `(programaId: string, inscripcionId: string) => '/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/bloquear'` | PATCH bloquear promotor |
| `programas.darDeBaja` | `(programaId: string, inscripcionId: string) => '/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/dar-de-baja'` | PATCH dar de baja a promotor aprobado |

Agregar nuevo sub-objeto `crowdpromotion.promotorInscripciones` (nuevo, independiente de `promotor`):

| Constante | Valor | Uso |
|-----------|-------|-----|
| `promotorInscripciones.misProgramas` | `'/api/crowdpromotion/promotor/mis-programas'` | GET inscripciones del promotor autenticado |

### 4.3 Adiciones a APP_ROUTES

**Ampliar `APP_ROUTES.landing.crowdpromotion` (ya existe con `templates, wizard, necesidades, etc.`):**

Nota: `APP_ROUTES.landing` ya tiene un sub-objeto `promotor` con `registro, dashboard, perfil`. Se agregan rutas de crowdpromotion en `APP_ROUTES.landing.crowdpromotion` (sub-objeto nuevo dentro de landing):

| Constante | Valor | Uso |
|-----------|-------|-----|
| `landing.crowdpromotion.explorar` | `'/crowdpromotion/explorar'` | Pagina de catalogo de programas para promotores (Landing) |
| `landing.crowdpromotion.misProgramas` | `'/promotor/mis-programas'` | Pagina "Mis Programas" del promotor (Landing) |
| `landing.crowdpromotion.inscripcionDetalle` | `(inscripcionId: string) => '/promotor/mis-programas/${inscripcionId}'` | Detalle de inscripcion aprobada con codigo referido (Landing) |

**Ampliar `APP_ROUTES.dashboard.crowdpromotion.programas` (ya existe con `list, nuevo, detalle, editar`):**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `dashboard.crowdpromotion.programas.inscripciones` | `(programaId: string) => '/dashboard/crowdpromotion/programas/${programaId}/inscripciones'` | Tab de inscripciones en el detalle de programa del artista (Admin) |

### 4.4 Nuevas Constantes de Dominio - Estado de Inscripcion

Agregar como nuevo bloque al final de la seccion de constantes Crowdpromotion (despues de `TIPO_REWARD_PROMO`):

| Constante | Tipo | Valores |
|-----------|------|---------|
| `INSCRIPCION_ESTADO` | `const` object | `PENDIENTE: 'Pendiente'`, `APROBADO: 'Aprobado'`, `BLOQUEADO: 'Bloqueado'`, `DADO_DE_BAJA: 'DadoDeBaja'` |
| `INSCRIPCION_ESTADO_LABELS` | `Record<string, string>` | `Pendiente: 'Pendiente de aprobacion'`, `Aprobado: 'Aprobado'`, `Bloqueado: 'Bloqueado'`, `DadoDeBaja: 'Dado de baja'` |
| `INSCRIPCION_ESTADO_BADGE_VARIANT` | `Record<string, string>` | `Pendiente: 'secondary'`, `Aprobado: 'default'`, `Bloqueado: 'destructive'`, `DadoDeBaja: 'outline'` |

**Nota sobre `INSCRIPCION_ESTADO_BADGE_VARIANT`:** Los valores (`secondary`, `default`, `destructive`, `outline`) son los `variant` del componente `<Badge>` de shadcn/ui. Permiten que Landing y Admin rendericen el badge con el color correcto sin duplicar logica de presentacion.

### 4.5 Adicion a VALIDATION

Agregar al objeto `VALIDATION` existente (dentro de la seccion Crowdpromotion):

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `INSCRIPCION_NOMBRE_ARTISTA_FILTRO_MAX` | `200` | Max chars para filtro de nombre de artista en explorar programas |
| `INSCRIPCION_PAGE_SIZE_MAX` | `50` | Tamano maximo de pagina para explorar y mis-programas |

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Adiciones a `error-messages.ts`

**Operacion:** Agregar nuevo bloque `INSCRIPCION_ERROR_MESSAGES` al final del archivo, siguiendo el patron de `PROMOTOR_ERROR_MESSAGES` y `PROMO_PROGRAMA_ERROR_MESSAGES`.

**Agregar a `ERROR_CODE_MESSAGES` (objeto global):**

| Codigo | Mensaje |
|--------|---------|
| `'2020'` | `'La inscripcion no existe.'` |
| `'4021'` | `'Ya estas inscrito en este programa.'` |
| `'4022'` | `'No puedes inscribirte en este programa.'` |
| `'4023'` | `'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.'` |
| `'4024'` | `'Este programa no esta activo en este momento.'` |
| `'4025'` | `'La inscripcion no se puede modificar en su estado actual.'` |
| `'4026'` | `'No tienes permiso para gestionar las inscripciones de este programa.'` |

**Agregar nuevo objeto `INSCRIPCION_ERROR_MESSAGES`:**

| Codigo / Key | Mensaje | Contexto de uso |
|--------------|---------|-----------------|
| `'2020'` | `'La inscripcion no existe.'` | Inscripcion no encontrada (artista) |
| `'4021'` | `'Ya estas inscrito en este programa.'` | POST inscripcion duplicada |
| `'4022'` | `'No puedes inscribirte en este programa.'` | POST cuando EsBloqueado == true |
| `'4023'` | `'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.'` | POST con promotor inactivo |
| `'4024'` | `'Este programa no esta activo en este momento.'` | POST con programa inactivo |
| `'4025'` | `'La inscripcion no se puede modificar en su estado actual.'` | PATCH aprobar/rechazar/bloquear/dar-de-baja con estado incorrecto |
| `'4026'` | `'No tienes permiso para gestionar las inscripciones de este programa.'` | PATCH/GET artista no propietario |
| `INSCRIPCION_NOT_FOUND` | `'La inscripcion no existe.'` | Semantic key para uso en hooks |
| `INSCRIPCION_ALREADY_EXISTS` | `'Ya estas inscrito en este programa.'` | Semantic key |
| `INSCRIPCION_PROMOTOR_BLOCKED` | `'No puedes inscribirte en este programa.'` | Semantic key |
| `INSCRIPCION_PROMOTOR_INACTIVE` | `'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.'` | Semantic key |
| `INSCRIPCION_PROGRAMA_INACTIVE` | `'Este programa no esta activo en este momento.'` | Semantic key |
| `INSCRIPCION_ESTADO_INVALIDO` | `'La inscripcion no se puede modificar en su estado actual.'` | Semantic key |
| `INSCRIPCION_NOT_OWNER` | `'No tienes permiso para gestionar las inscripciones de este programa.'` | Semantic key |

**Agregar funcion getter:**

```
getInscripcionErrorMessage(errorCode: string): string
  - Lookup en INSCRIPCION_ERROR_MESSAGES primero
  - Fallback a getErrorMessage(errorCode)
  - Patron identico a getPromotorErrorMessage y getPromoProgramaErrorMessage
```

### 5.2 Adiciones a `mappers.ts`

**Operacion:** Agregar al final del archivo siguiendo el patron de `mapPromotorToUpdateForm`.

| Funcion | Input | Output | Descripcion |
|---------|-------|--------|-------------|
| `mapInscripcionEstado` | `{ esAprobado: boolean, esBloqueado: boolean, fechaBaja: string \| null }` | `InscripcionEstado` | Deriva el estado de la inscripcion a partir de las flags booleanas. Util en componentes que tienen los datos crudos del DTO y necesitan computar el estado sin repetir la logica. El backend ya incluye `estado` calculado en las respuestas, pero este mapper es util para operaciones locales optimistas (cuando el frontend actualiza la cache antes de confirmar con el servidor). |
| `getInscripcionBadgeVariant` | `estado: InscripcionEstado` | `string` | Devuelve el `variant` del Badge de shadcn (`'secondary' \| 'default' \| 'destructive' \| 'outline'`) para el estado dado. Wrappea la constante `INSCRIPCION_ESTADO_BADGE_VARIANT`. Evita que cada componente acceda al objeto de constante directamente. |

**Implementacion de `mapInscripcionEstado` (logica a codificar):**

```
si esBloqueado == true  -> retorna 'Bloqueado'
si fechaBaja != null    -> retorna 'DadoDeBaja'
si esAprobado == true   -> retorna 'Aprobado'
default                 -> retorna 'Pendiente'
```

Esta precedencia coincide exactamente con la logica server-side documentada en contracts.md.

---

## 6. Archivos a Modificar

```
src/shared/
├── types/
│   └── crowdpromotion.ts         <- MODIFICAR: agregar 10 interfaces + 1 union type al final
├── schemas/
│   └── crowdpromotion.schema.ts  <- MODIFICAR: agregar 2 schemas + 2 type inferidos al final
├── constants/
│   └── index.ts                  <- MODIFICAR: ampliar QUERY_KEYS, API_ROUTES, APP_ROUTES + 3 nuevas constantes de dominio
└── utils/
    ├── error-messages.ts         <- MODIFICAR: agregar 7 codigos a ERROR_CODE_MESSAGES + nuevo INSCRIPCION_ERROR_MESSAGES + getter
    └── mappers.ts                <- MODIFICAR: agregar 2 funciones al final
```

**Ningun archivo nuevo se crea.** Todo el codigo se integra en los archivos ya existentes de la feature crowdpromotion.

---

## 7. Dependencias

- `zod` (ya instalado en `src/shared/node_modules`)
- `src/shared/types/crowdpromotion.ts` debe ser importado en `mappers.ts` para los tipos `InscripcionEstado`
- Los types `ExplorarProgramasFilters` e `InscripcionesFilters` deben importarse en `constants/index.ts` para tipar las funciones de QUERY_KEYS; actualmente el archivo ya importa `MisCampaniasQueryParams` y `BackingsQueryParams` de `schemas/dashboard.schema`. Agregar import de `ExplorarProgramasFilters` e `InscripcionesFilters` de `types/crowdpromotion`
- Ningun package adicional requerido

---

## 8. Notas de Implementacion

### Orden de edicion recomendado

1. **Primero `types/crowdpromotion.ts`**: agregar `InscripcionEstado` (union type) antes de las interfaces, ya que es referenciado por varias de ellas.
2. **Luego `schemas/crowdpromotion.schema.ts`**: los schemas referencian los tipos de filtros que se acaban de agregar.
3. **Luego `constants/index.ts`**: requiere que los tipos de filtros existan para tipar las funciones de QUERY_KEYS.
4. **Luego `utils/error-messages.ts`**: independiente, puede hacerse en cualquier momento.
5. **Finalmente `utils/mappers.ts`**: requiere que `InscripcionEstado` y `INSCRIPCION_ESTADO_BADGE_VARIANT` existan.

### Gestion de valores nulos en `codigoReferido` y `urlTrackingPersonalizada`

Los componentes de Landing que muestran el codigo referido y la URL deben verificar siempre `miInscripcion.estado === 'Aprobado'` antes de intentar renderizar estos campos. Aunque el backend garantiza que solo los devuelve con valor en inscripciones aprobadas sin baja, el tipo TypeScript los declara como `string | null` para reflejar la realidad del transporte HTTP.

### Invalidacion de cache tras mutaciones

La tabla siguiente define que query keys deben invalidarse tras cada mutacion:

| Mutacion | Query keys a invalidar |
|----------|------------------------|
| POST /inscripcion (exito) | `crowdpromotion.programas.explorar` (el `miEstado` cambia), `crowdpromotion.inscripciones.misProgramas` |
| PATCH /aprobar | `crowdpromotion.programas.inscripciones(programaId)`, `crowdpromotion.programas.byId(programaId)` (el resumen de promotores cambia) |
| PATCH /rechazar | `crowdpromotion.programas.inscripciones(programaId)`, `crowdpromotion.programas.byId(programaId)` |
| PATCH /bloquear | `crowdpromotion.programas.inscripciones(programaId)`, `crowdpromotion.programas.byId(programaId)` |
| PATCH /dar-de-baja | `crowdpromotion.programas.inscripciones(programaId)`, `crowdpromotion.programas.byId(programaId)` |

### Nombre del campo `urlTrackingPersonalizada` vs `urlReferido`

La entidad backend tiene `UrlReferido` como nombre de columna. Los DTOs exponen `UrlTrackingPersonalizada` como nombre de campo serializado. Los types TypeScript deben usar `urlTrackingPersonalizada` (camelCase) alineado con los DTOs, no con la entidad.

### Codigos de error nuevos en `ERROR_CODE_MESSAGES` global

Los codigos `2020` y `4021`-`4026` son nuevos en el sistema. Deben agregarse tanto al objeto `ERROR_CODE_MESSAGES` (para lookup general) como al nuevo objeto `INSCRIPCION_ERROR_MESSAGES` (para lookup contextual especifico de inscripciones).

### Conflicto potencial con `TIPO_REWARD_PROMO`

El archivo `constants/index.ts` ya tiene un comentario que advierte sobre la confusion entre `TIPO_REWARD_PROMO` (Crowdpromotion) y `TIPO_REWARD` (Campanias). Las constantes `INSCRIPCION_ESTADO` que se agregan no tienen conflicto con ninguna constante existente.

---

## 9. Checklist de Implementacion

- [ ] `InscripcionEstado` union type creado antes de las interfaces que lo usan
- [ ] `ProgramaExplorarItem.miEstado` tipado como `InscripcionEstado | null` (no como `string | null`)
- [ ] `MiInscripcion.estado` y `InscripcionListItem.estado` tipados como `InscripcionEstado` (no como `string`)
- [ ] `explorarProgramasFiltersSchema` con mensajes de error en espanol
- [ ] `inscripcionesFiltersSchema` usa `z.enum([...])` con los 4 valores exactos del union type
- [ ] QUERY_KEYS ampliados en `crowdpromotion.programas` (no reemplazado)
- [ ] `crowdpromotion.inscripciones.misProgramas` agregado como nuevo sub-objeto
- [ ] API_ROUTES incluye los 7 nuevos endpoints alineados exactamente con contracts.md
- [ ] APP_ROUTES incluye las 3 rutas de Landing y 1 de Admin
- [ ] `INSCRIPCION_ESTADO` con valores en PascalCase (coinciden con el backend: `'Pendiente'`, `'Aprobado'`, `'Bloqueado'`, `'DadoDeBaja'`)
- [ ] `INSCRIPCION_ESTADO_BADGE_VARIANT` usa variants validos de shadcn Badge
- [ ] Codigos `2020` y `4021`-`4026` agregados a `ERROR_CODE_MESSAGES` global
- [ ] `INSCRIPCION_ERROR_MESSAGES` con claves semanticas para uso interno en hooks
- [ ] `getInscripcionErrorMessage` con fallback a `getErrorMessage`
- [ ] `mapInscripcionEstado` con precedencia correcta: Bloqueado > DadoDeBaja > Aprobado > Pendiente
- [ ] `getInscripcionBadgeVariant` wrappea constante (no hardcodea strings)
- [ ] Import de `ExplorarProgramasFilters` e `InscripcionesFilters` agregado en `constants/index.ts`
- [ ] Ningun tipo existente de US-CP-01 o US-CP-02 fue modificado
