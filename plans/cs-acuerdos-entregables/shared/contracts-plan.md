# Plan de Contratos Shared: cs-acuerdos-entregables

**Fecha:** 2026-02-18
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Basado en:** docs/user-stories/cs-acuerdos-entregables/contracts.md

---

## 1. Resumen

- Total de types: 17 interfaces + 3 union types (agregados al archivo existente `crowdsourcing.ts`)
- Total de schemas Zod: 7 schemas nuevos + 7 types inferidos (agregados al archivo existente `crowdsourcing.schema.ts`)
- Constantes definidas: 6 bloques de constantes (ESTADO_ACUERDO, ESTADO_ENTREGABLE con labels y badges, extensiones de QUERY_KEYS, API_ROUTES, APP_ROUTES)
- Utilidades planificadas: 1 bloque de error messages nuevo (`ACUERDO_ERROR_MESSAGES`) + 1 funcion helper (`getAcuerdoErrorMessage`)
- **Estrategia:** Todos los cambios son ADICIONES a archivos existentes. No se crea ninguna carpeta nueva en `src/shared/`.

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

**Accion:** Agregar al final del archivo existente. El archivo ya contiene los types de US-CS-01, CS-02 y CS-03.

### 2.1 DTOs de Response (Detalle del Acuerdo)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `AcuerdoArtista` | `id: string`, `nombreArtistico: string` | Sub-objeto del artista dentro del detalle del acuerdo |
| `AcuerdoProfesional` | `userId: string`, `perfilProfesionalId: string`, `nombre: string` | Sub-objeto del profesional dentro del detalle del acuerdo |
| `AcuerdoNecesidad` | `id: string`, `titulo: string` | Sub-objeto de la necesidad vinculada al acuerdo |
| `AcuerdoTimelineEvento` | `accion: string`, `fecha: string`, `actor: string` | Evento del timeline de actividad del acuerdo |
| `Entregable` | `id`, `titulo`, `descripcion?`, `urlRecurso?`, `estadoEntregableId: number`, `estadoEntregableNombre`, `comentarioAprobacion?`, `comentarioRechazo?`, `fechaAprobacion?`, `fechaCreacion` | Entregable anidado dentro de un Milestone |
| `Milestone` | `id`, `titulo`, `descripcion?`, `orden: number`, `importeParcial: number`, `porcentajeParcial: number`, `fechaLimite?`, `fechaCompletado?`, `entregables: Entregable[]` | Milestone con sus entregables anidados |
| `Acuerdo` | `id`, `tituloInterno`, `estadoAcuerdoId: number`, `estadoAcuerdoNombre`, `importeTotalPactado: number`, `monedaNombre`, `fechaInicio`, `fechaFinPrevista?`, `fechaFinReal?`, `artista: AcuerdoArtista`, `profesional: AcuerdoProfesional`, `necesidad: AcuerdoNecesidad`, `conversacionId?`, `milestones: Milestone[]`, `importeAsignado: number`, `porcentajeAsignado: number`, `miRol: 'Artista' \| 'Profesional'`, `timeline: AcuerdoTimelineEvento[]` | Detalle completo del acuerdo (respuesta de GET /api/crowdsourcing/acuerdos/{id}) |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `AceptarPropuestaRequest` | `tituloInterno: string`, `fechaInicio: string`, `fechaFinPrevista?: string` | Body para POST /propuestas/{id}/aceptar. Fechas como "YYYY-MM-DD" |
| `RechazarPropuestaRequest` | `motivo?: string` | Body para PATCH /propuestas/{id}/rechazar. Motivo opcional, max 500 |
| `CreateMilestoneRequest` | `titulo: string`, `descripcion?: string`, `importeParcial: number`, `fechaLimite?: string` | Body para POST /acuerdos/{id}/milestones |
| `CreateEntregableRequest` | `titulo: string`, `descripcion?: string`, `urlRecurso?: string`, `milestoneId?: string` | Body para POST /acuerdos/{id}/entregables |
| `AprobarEntregableRequest` | `comentario?: string` | Body para PATCH /entregables/{id}/aprobar. Comentario opcional max 500 |
| `RechazarEntregableRequest` | `comentario: string` | Body para PATCH /entregables/{id}/rechazar. Comentario obligatorio min 10 max 500 |
| `CancelarAcuerdoRequest` | `motivo: string` | Body para PATCH /acuerdos/{id}/cancelar. Motivo obligatorio min 20 max 1000 |

### 2.3 DTOs de Result (respuestas de mutaciones)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `AceptarPropuestaResult` | `acuerdoId: string`, `tituloInterno: string`, `estadoAcuerdoNombre: string`, `importeTotalPactado: number`, `monedaNombre: string`, `conversacionId: string`, `propuestasRechazadas: number` | Respuesta de POST /propuestas/{id}/aceptar (201) |
| `RechazarPropuestaResult` | `id: string`, `estadoPropuestaNombre: string` | Respuesta de PATCH /propuestas/{id}/rechazar (200) |
| `MilestoneCreatedResult` | `id: string`, `titulo: string`, `orden: number`, `importeParcial: number`, `porcentajeParcial: number`, `importeAsignadoTotal: number` | Respuesta de POST y PUT /milestones (201 / 200) |
| `EntregableCreatedResult` | `id: string`, `titulo: string`, `estadoEntregableNombre: string`, `fechaCreacion: string` | Respuesta de POST /entregables (201) |
| `AprobarEntregableResult` | `id: string`, `estadoEntregableNombre: string`, `fechaAprobacion: string`, `todosAprobadosEnMilestone: boolean` | Respuesta de PATCH /entregables/{id}/aprobar (200) |
| `RechazarEntregableResult` | `id: string`, `estadoEntregableNombre: string` | Respuesta de PATCH /entregables/{id}/rechazar (200) |
| `CompletarAcuerdoResult` | `id: string`, `estadoAcuerdoNombre: string`, `fechaFinReal: string` | Respuesta de PATCH /acuerdos/{id}/completar (200) |
| `CancelarAcuerdoResult` | `id: string`, `estadoAcuerdoNombre: string`, `fechaFinReal: string`, `necesidadEstadoNombre: string` | Respuesta de PATCH /acuerdos/{id}/cancelar (200) |

### 2.4 Enums y Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `EstadoAcuerdo` | `'Activo' \| 'Completado' \| 'Cancelado'` | Union type para el campo `estadoAcuerdoNombre` en `Acuerdo` y results. Complementa `ESTADO_ACUERDO` numericos |
| `EstadoEntregable` | `'Entregado' \| 'Aprobado' \| 'Rechazado'` | Union type para el campo `estadoEntregableNombre` en `Entregable` y results |
| `RolAcuerdo` | `'Artista' \| 'Profesional'` | Union type para el campo `miRol` en `Acuerdo`. Determina que acciones muestra el frontend |

**Nota de implementacion:** Los union types `EstadoAcuerdo` y `EstadoEntregable` son los valores de texto que devuelve el backend. Las constantes numericas `ESTADO_ACUERDO` y `ESTADO_ENTREGABLE` (en constants/index.ts) corresponden a los IDs enteros usados para comparaciones logicas en el frontend.

### 2.5 Posicion en el archivo

Agregar una nueva seccion al final de `src/shared/types/crowdsourcing.ts`:

```
// ========== Acuerdos, Milestones y Entregables (US-CS-04) ==========
// [union types]
// [request interfaces]
// [result interfaces]
// [detail DTOs]
```

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

**Accion:** Agregar al final del archivo existente una nueva seccion delimitada por comentario.

### 3.1 Schemas de Validacion

| Schema | Campos con Reglas |
|--------|-------------------|
| `aceptarPropuestaSchema` | `tituloInterno`: string, min(1, 'El titulo interno es obligatorio'), max(200, 'El titulo no puede superar los 200 caracteres'). `fechaInicio`: string, min(1, 'La fecha de inicio es obligatoria'). `fechaFinPrevista`: string optional. **Refinement:** si `fechaFinPrevista` presente, debe ser `> fechaInicio` con message 'La fecha de fin prevista debe ser posterior a la fecha de inicio', path: `['fechaFinPrevista']` |
| `rechazarPropuestaSchema` | `motivo`: string, max(500, 'El motivo no puede superar los 500 caracteres'), optional |
| `createMilestoneSchema` | `titulo`: string, min(1, 'El titulo es obligatorio'), min(3, 'El titulo debe tener al menos 3 caracteres'), max(200, 'El titulo no puede superar los 200 caracteres'). `descripcion`: string, max(1000, 'La descripcion no puede superar los 1000 caracteres'), optional. `importeParcial`: number({ required_error, invalid_type_error }), positive('El importe parcial debe ser mayor a 0'). `fechaLimite`: string optional **(nota: validacion de >= fechaInicio del acuerdo se hace en componente con contexto, no en schema)** |
| `createEntregableSchema` | `titulo`: string, min(1, 'El titulo es obligatorio'), min(3, 'El titulo debe tener al menos 3 caracteres'), max(200, 'El titulo no puede superar los 200 caracteres'). `descripcion`: string, max(1000, 'La descripcion no puede superar los 1000 caracteres'), optional. `urlRecurso`: string, url('Debe ser una URL valida'), optional, or(z.literal('')) para aceptar string vacio. `milestoneId`: string, uuid('ID de milestone invalido'), optional |
| `aprobarEntregableSchema` | `comentario`: string, max(500, 'El comentario no puede superar los 500 caracteres'), optional |
| `rechazarEntregableSchema` | `comentario`: string, min(1, 'El comentario es obligatorio al rechazar'), min(10, 'Minimo 10 caracteres explicando que debe corregirse'), max(500, 'El comentario no puede superar los 500 caracteres') |
| `cancelarAcuerdoSchema` | `motivo`: string, min(1, 'El motivo es obligatorio'), min(20, 'El motivo debe tener al menos 20 caracteres'), max(1000, 'El motivo no puede superar los 1000 caracteres') |

**Nota sobre `createMilestoneSchema`:** El schema de edicion de milestone (`updateMilestoneSchema`) usa exactamente los mismos campos y reglas que `createMilestoneSchema`. Se puede reutilizar el mismo schema en ambos formularios (crear y editar), ya que las reglas de validacion client-side son identicas segun contracts.md.

### 3.2 Types Inferidos

| Type Inferido | Schema origen | Uso |
|---------------|--------------|-----|
| `AceptarPropuestaFormData` | `z.infer<typeof aceptarPropuestaSchema>` | React Hook Form en dialogo de aceptar propuesta |
| `RechazarPropuestaFormData` | `z.infer<typeof rechazarPropuestaSchema>` | React Hook Form en dialogo de rechazar propuesta |
| `CreateMilestoneFormData` | `z.infer<typeof createMilestoneSchema>` | React Hook Form en `MilestoneFormDialog` (crear y editar) |
| `CreateEntregableFormData` | `z.infer<typeof createEntregableSchema>` | React Hook Form en `SubirEntregableDialog` |
| `AprobarEntregableFormData` | `z.infer<typeof aprobarEntregableSchema>` | React Hook Form en `AprobarEntregableDialog` |
| `RechazarEntregableFormData` | `z.infer<typeof rechazarEntregableSchema>` | React Hook Form en `RechazarEntregableDialog` |
| `CancelarAcuerdoFormData` | `z.infer<typeof cancelarAcuerdoSchema>` | React Hook Form en `CancelarAcuerdoDialog` |

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Accion:** Agregar al final del archivo existente, despues de la seccion `PROPUESTA` (US-CS-03), una nueva seccion delimitada.

### 4.1 Nuevas Constantes de Estado

#### ESTADO_ACUERDO

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `ESTADO_ACUERDO.ACTIVO` | `1` | El acuerdo esta en progreso, acepta milestones y entregables |
| `ESTADO_ACUERDO.COMPLETADO` | `2` | El artista marco el acuerdo como completado |
| `ESTADO_ACUERDO.CANCELADO` | `3` | Cancelado por artista o profesional (irreversible) |

Acompanado de:
- `ESTADO_ACUERDO_LABELS: Record<number, string>` - `{ 1: 'Activo', 2: 'Completado', 3: 'Cancelado' }`
- `ESTADO_ACUERDO_BADGES: Record<number, string>` - `{ 1: 'blue', 2: 'green', 3: 'gray' }` (badge colors para shadcn/ui)

#### ESTADO_ENTREGABLE

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `ESTADO_ENTREGABLE.ENTREGADO` | `1` | Subido por el profesional, pendiente de revision del artista |
| `ESTADO_ENTREGABLE.APROBADO` | `2` | Aprobado por el artista |
| `ESTADO_ENTREGABLE.RECHAZADO` | `3` | Rechazado por el artista, el profesional puede subir nueva version |

Acompanado de:
- `ESTADO_ENTREGABLE_LABELS: Record<number, string>` - `{ 1: 'Entregado', 2: 'Aprobado', 3: 'Rechazado' }`
- `ESTADO_ENTREGABLE_BADGES: Record<number, string>` - `{ 1: 'yellow', 2: 'green', 3: 'red' }`

### 4.2 Extension de QUERY_KEYS

**Accion:** Agregar `acuerdos` dentro del objeto existente `QUERY_KEYS.crowdsourcing`. El objeto actual ya tiene `necesidades` y `propuestas`. Se agrega al mismo nivel:

```
crowdsourcing: {
    ...existente (templates, maestras, necesidades, propuestas)...
    acuerdos: {
        byId: (id: string) => ['crowdsourcing', 'acuerdos', id] as const,
    },
}
```

**Nota sobre milestones y entregables:** Segun contracts.md, milestones y entregables se cargan anidados en la respuesta del GET del acuerdo. No tienen query key propio. Todas las mutaciones de milestones y entregables invalidan `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`.

### 4.3 Extension de API_ROUTES

**Accion:** Modificar el objeto existente `API_ROUTES.crowdsourcing` para agregar las nuevas rutas dentro de `propuestas` y agregar el nuevo grupo `acuerdos` y `entregables`.

Adiciones a `API_ROUTES.crowdsourcing.propuestas`:

| Funcion | Valor | Uso |
|---------|-------|-----|
| `aceptar: (id: string)` | `` `/api/crowdsourcing/propuestas/${id}/aceptar` `` | POST - Acepta propuesta y crea acuerdo |
| `rechazar: (id: string)` | `` `/api/crowdsourcing/propuestas/${id}/rechazar` `` | PATCH - Rechaza propuesta individualmente |

Nuevo grupo `API_ROUTES.crowdsourcing.acuerdos`:

| Clave | Valor | Metodo | Uso |
|-------|-------|--------|-----|
| `byId: (id)` | `` `/api/crowdsourcing/acuerdos/${id}` `` | GET | Detalle completo del acuerdo |
| `completar: (id)` | `` `/api/crowdsourcing/acuerdos/${id}/completar` `` | PATCH | Completa el acuerdo |
| `cancelar: (id)` | `` `/api/crowdsourcing/acuerdos/${id}/cancelar` `` | PATCH | Cancela el acuerdo |
| `milestones: (acuerdoId)` | `` `/api/crowdsourcing/acuerdos/${acuerdoId}/milestones` `` | POST | Crea un milestone |
| `milestoneById: (acuerdoId, id)` | `` `/api/crowdsourcing/acuerdos/${acuerdoId}/milestones/${id}` `` | PUT / DELETE | Edita o elimina un milestone |
| `entregables: (acuerdoId)` | `` `/api/crowdsourcing/acuerdos/${acuerdoId}/entregables` `` | POST | Sube un entregable |

Nuevo grupo `API_ROUTES.crowdsourcing.entregables`:

| Clave | Valor | Metodo | Uso |
|-------|-------|--------|-----|
| `aprobar: (id)` | `` `/api/crowdsourcing/entregables/${id}/aprobar` `` | PATCH | El artista aprueba un entregable |
| `rechazar: (id)` | `` `/api/crowdsourcing/entregables/${id}/rechazar` `` | PATCH | El artista rechaza un entregable |

### 4.4 Extension de APP_ROUTES

**Accion:** Agregar `acuerdoDetail` dentro del objeto existente `APP_ROUTES.landing.crowdsourcing`.

El objeto actual ya tiene `necesidades`, `necesidadDetail`, `misPropuestas`. Se agrega:

| Clave | Valor | Uso |
|-------|-------|-----|
| `acuerdoDetail: (id: string)` | `` `/crowdsourcing/acuerdos/${id}` `` | Ruta de detalle del acuerdo en la Landing. Protegida por JWT segun contracts.md |

**Nota:** La ruta de detalle de acuerdo en el Admin (`/dashboard/crowdsourcing/acuerdos/:id`) se definira en el proyecto admin. No hay una ruta de admin en este contracts.md.

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

**Accion:** Agregar al final del archivo existente, despues de la seccion `PROPUESTA_ERROR_MESSAGES` (US-CS-03), una nueva seccion.

### 5.1 Nuevos Error Messages

**Nombre del objeto:** `ACUERDO_ERROR_MESSAGES: Record<string, string>`

Agregar ademas los siguientes codigos numericos al objeto global `ERROR_CODE_MESSAGES` existente (ya que esos codigos no estan presentes):

| Codigo | Mensaje | Contexto |
|--------|---------|----------|
| `'1011'` | Ya existe con valor: 'El monto no es valido' | **Verificar:** el codigo 1011 ya existe en `ERROR_CODE_MESSAGES` con ese mensaje. En el contexto de acuerdos, 1011 significa "min length". Se usa `ACUERDO_ERROR_MESSAGES` local para sobreescribir con mensaje contextual |
| `'1013'` | `'La URL del recurso no es valida'` | Nuevo codigo. Agregar a `ERROR_CODE_MESSAGES` |
| `'2011'` | `'El acuerdo no fue encontrado'` | Nuevo codigo. Agregar a `ERROR_CODE_MESSAGES` |
| `'2012'` | `'El milestone no fue encontrado'` | Nuevo codigo. Agregar a `ERROR_CODE_MESSAGES` |
| `'2013'` | `'El entregable no fue encontrado'` | Nuevo codigo. Agregar a `ERROR_CODE_MESSAGES` |
| `'4007'` | Ya existe con valor distinto (campania) | Ver nota abajo |
| `'4008'` | Ya existe con valor distinto | Ver nota abajo |
| `'4009'` | Ya existe con valor distinto | Ver nota abajo |
| `'4010'` | Ya existe con valor distinto | Ver nota abajo |
| `'4011'` | Ya existe con valor distinto | Ver nota abajo |
| `'4012'` | Ya existe con valor distinto | Ver nota abajo |
| `'4013'` | Ya existe con valor distinto | Ver nota abajo |

**Conflicto de codigos 4007-4013:** El archivo existente ya tiene codigos 4007-4013 mapeados a mensajes del dominio de campanias y backings. Para evitar sobreescribir, se crea el objeto especifico `ACUERDO_ERROR_MESSAGES` con sus propios mappings, y la funcion `getAcuerdoErrorMessage` usa ese objeto primero antes de hacer fallback a `getErrorMessage`.

**Objeto `ACUERDO_ERROR_MESSAGES`:**

| Clave | Mensaje | Contexto |
|-------|---------|----------|
| `'2011'` | `'El acuerdo no fue encontrado'` | GET acuerdo, completar, cancelar |
| `'2012'` | `'El milestone no fue encontrado'` | PUT/DELETE milestone |
| `'2013'` | `'El entregable no fue encontrado'` | aprobar/rechazar entregable |
| `'4007'` | `'La propuesta no esta disponible para esta accion'` | Propuesta no en estado Pendiente al aceptar/rechazar |
| `'4008'` | `'Ya existe un acuerdo activo para esta necesidad'` | Intento de aceptar propuesta cuando ya hay acuerdo activo |
| `'4009'` | `'La suma de importes de los milestones supera el total pactado'` | Crear/editar milestone |
| `'4010'` | `'Esta accion solo esta disponible cuando el acuerdo esta activo'` | Cualquier mutacion sobre acuerdo no-activo |
| `'4011'` | `'No se puede modificar un milestone que ya fue completado'` | PUT/DELETE milestone completado |
| `'4012'` | `'No se puede eliminar un milestone que tiene entregables asociados'` | DELETE milestone con entregables |
| `'4013'` | `'Solo se pueden revisar entregables en estado Entregado'` | aprobar/rechazar entregable no-Entregado |
| `'1013'` | `'La URL del recurso no es valida. Usa un enlace de Dropbox, Drive o similar'` | urlRecurso invalida en entregable |
| `'1011'` | `'El campo no cumple la longitud minima requerida'` | Contexto acuerdos: titulo min 3, comentario min 10, motivo min 20 |
| `ACUERDO_NOT_FOUND` | `'El acuerdo no fue encontrado'` | Key semantica para uso interno |
| `MILESTONE_NOT_FOUND` | `'El milestone no fue encontrado'` | Key semantica para uso interno |
| `ENTREGABLE_NOT_FOUND` | `'El entregable no fue encontrado'` | Key semantica para uso interno |
| `PROPUESTA_NOT_ACCEPTABLE` | `'La propuesta no esta en estado Pendiente'` | Key semantica para uso interno |
| `ACUERDO_ALREADY_EXISTS` | `'Ya existe un acuerdo activo para esta necesidad'` | Key semantica para uso interno |
| `MILESTONE_IMPORTE_EXCEEDED` | `'La suma de importes supera el total pactado del acuerdo'` | Key semantica para uso interno |
| `ACUERDO_NOT_ACTIVE` | `'Esta accion solo esta disponible para acuerdos activos'` | Key semantica para uso interno |
| `MILESTONE_COMPLETED` | `'El milestone ya fue completado y no puede modificarse'` | Key semantica para uso interno |
| `MILESTONE_HAS_ENTREGABLES` | `'El milestone tiene entregables asociados y no puede eliminarse'` | Key semantica para uso interno |
| `ENTREGABLE_NOT_REVIEWABLE` | `'El entregable no esta en estado Entregado'` | Key semantica para uso interno |
| `VALIDATION_COMENTARIO_RECHAZO_MIN` | `'El comentario de rechazo debe tener al menos 10 caracteres'` | Key semantica para rechazar entregable |
| `VALIDATION_MOTIVO_CANCELACION_MIN` | `'El motivo de cancelacion debe tener al menos 20 caracteres'` | Key semantica para cancelar acuerdo |

**Funcion helper:**

```
getAcuerdoErrorMessage(errorCode: string): string
```
- Busca primero en `ACUERDO_ERROR_MESSAGES`
- Fallback a `getErrorMessage(errorCode)` (funcion existente)
- Mismo patron que `getPropuestaErrorMessage` y `getNecesidadErrorMessage` existentes

### 5.2 Adicion a ERROR_CODE_MESSAGES

Agregar los codigos que son genuinamente nuevos y no conflictivos al objeto `ERROR_CODE_MESSAGES` existente:

| Codigo | Mensaje |
|--------|---------|
| `'1013'` | `'La URL proporcionada no es valida'` |
| `'2011'` | `'Acuerdo no encontrado'` |
| `'2012'` | `'Milestone no encontrado'` |
| `'2013'` | `'Entregable no encontrado'` |

---

## 6. Archivos a Crear/Modificar

```
src/shared/
├── types/
│   └── crowdsourcing.ts          <-- MODIFICAR: agregar seccion US-CS-04 al final
├── schemas/
│   └── crowdsourcing.schema.ts   <-- MODIFICAR: agregar 7 schemas + 7 types inferidos al final
├── constants/
│   └── index.ts                  <-- MODIFICAR: agregar ESTADO_ACUERDO, ESTADO_ENTREGABLE,
│                                      extender QUERY_KEYS, API_ROUTES y APP_ROUTES
└── utils/
    └── error-messages.ts         <-- MODIFICAR: agregar ACUERDO_ERROR_MESSAGES +
                                       getAcuerdoErrorMessage + 4 entradas a ERROR_CODE_MESSAGES
```

**No se crean nuevos archivos en `src/shared/`.** Todo son adiciones a archivos existentes.

---

## 7. Dependencias

- `zod` - ya instalado (version disponible en node_modules)
- Ningun package adicional requerido

---

## 8. Notas de Implementacion

### 8.1 Sobre los types de fechas

Todas las fechas en los types TypeScript se representan como `string`:
- En requests: formato ISO date `"YYYY-MM-DD"` (sin hora)
- En responses del servidor: formato ISO datetime `"YYYY-MM-DDTHH:mm:ssZ"` (con hora UTC)
- El campo `fechaCreacion` en `Entregable` y `EntregableCreatedResult` es `string` (ISO datetime)

### 8.2 Sobre la reutilizacion del schema de milestone

`createMilestoneSchema` se usa para crear (POST) y editar (PUT). Los campos y reglas son identicos. La validacion de `fechaLimite >= fechaInicio del acuerdo` no puede hacerse en el schema Zod porque requiere el contexto del acuerdo (que se carga por separado). Esta validacion se implementa en el componente `MilestoneFormDialog` usando `refine` con closure o directamente en el submit handler.

### 8.3 Sobre los codigos de error conflictivos (4007-4013)

Los codigos 4007-4013 ya existen en `ERROR_CODE_MESSAGES` del archivo `error-messages.ts` con mensajes del dominio de campanias/backings. La solucion es el patron ya establecido en el proyecto: crear un objeto especifico por dominio (`ACUERDO_ERROR_MESSAGES`) y una funcion helper que lo usa como primer lookup. Esto no rompe ningun consumer existente.

### 8.4 Sobre el campo `miRol` en Acuerdo

El campo `miRol: 'Artista' | 'Profesional'` que devuelve el backend determina que componentes y botones se muestran en el frontend. Es el mecanismo de autorizacion en UI: el frontend no necesita comparar IDs, solo lee `miRol`.

### 8.5 Sobre el schema de urlRecurso

El schema usa `.url().optional().or(z.literal(''))` para manejar el caso de campo opcional en formulario React Hook Form donde un campo vacio es string vacio `""` (no `undefined`). Esto es coherente con el patron usado en otros schemas del proyecto.

### 8.6 Sobre QUERY_KEYS y invalidacion

Milestones y entregables NO tienen query keys propios porque siempre se cargan anidados dentro de `GET /acuerdos/{id}`. Todas las mutaciones (create/update/delete milestone, create/aprobar/rechazar entregable) invalidan `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` para refrescar el detalle completo.

### 8.7 Sobre la extension de API_ROUTES y APP_ROUTES

Los objetos `API_ROUTES` y `APP_ROUTES` en el archivo existente estan declarados con `as const`. Para extenderlos con nuevas claves se modifican directamente los objetos literales en el archivo. Las rutas de propuestas `aceptar` y `rechazar` se agregan dentro del objeto `crowdsourcing.propuestas` existente (que actualmente solo tiene `mis` y `retirar`).

---

## 9. Orden de Implementacion

1. **Primero:** `src/shared/types/crowdsourcing.ts`
   - Agregar union types, request interfaces, result interfaces, detail DTOs
   - Sin dependencias de otros archivos

2. **Segundo:** `src/shared/schemas/crowdsourcing.schema.ts`
   - Agregar los 7 schemas Zod y sus types inferidos
   - Depende de `zod` (ya instalado)

3. **Tercero:** `src/shared/constants/index.ts`
   - Agregar ESTADO_ACUERDO y ESTADO_ENTREGABLE con labels y badges
   - Extender QUERY_KEYS, API_ROUTES y APP_ROUTES

4. **Cuarto:** `src/shared/utils/error-messages.ts`
   - Agregar 4 entradas nuevas a ERROR_CODE_MESSAGES
   - Agregar ACUERDO_ERROR_MESSAGES
   - Agregar getAcuerdoErrorMessage

---

## 10. Checklist

- [ ] Types creados y exportados en `crowdsourcing.ts` (sigue exportando via `types/index.ts`)
- [ ] Schemas Zod con mensajes de error en espanol alineados con contracts.md
- [ ] `aceptarPropuestaSchema` incluye el `refine` de fechaFinPrevista > fechaInicio
- [ ] `createEntregableSchema` maneja `urlRecurso` con `.optional().or(z.literal(''))`
- [ ] Constantes ESTADO_ACUERDO y ESTADO_ENTREGABLE con IDs numericos (1, 2, 3)
- [ ] ESTADO_ACUERDO_LABELS, ESTADO_ACUERDO_BADGES definidos
- [ ] ESTADO_ENTREGABLE_LABELS, ESTADO_ENTREGABLE_BADGES definidos
- [ ] QUERY_KEYS.crowdsourcing.acuerdos.byId agregado
- [ ] API_ROUTES.crowdsourcing.propuestas extendido con aceptar y rechazar
- [ ] API_ROUTES.crowdsourcing.acuerdos agregado (byId, completar, cancelar, milestones, milestoneById, entregables)
- [ ] API_ROUTES.crowdsourcing.entregables agregado (aprobar, rechazar)
- [ ] APP_ROUTES.landing.crowdsourcing.acuerdoDetail agregado
- [ ] ACUERDO_ERROR_MESSAGES con codigos numericos Y keys semanticas
- [ ] getAcuerdoErrorMessage funcion helper exportada
- [ ] Codigos 1013, 2011, 2012, 2013 agregados a ERROR_CODE_MESSAGES global
- [ ] Ningun export existente modificado o eliminado (solo adiciones)
