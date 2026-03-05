# Plan de Contratos Shared: cp-tareas-promocion

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Basado en:** docs/user-stories/cp-tareas-promocion/contracts.md

---

## 1. Resumen

- **Total de types nuevos:** 8 interfaces + 1 union type (todos nuevos, se agregan al archivo existente)
- **Total de schemas Zod nuevos:** 3 schemas con sus types inferidos
- **Constantes nuevas:** 5 endpoint functions en `API_ROUTES.crowdpromotion`, 2 query key functions en `QUERY_KEYS.crowdpromotion`, 1 mapa de estado con labels/badges, 7 error codes nuevos
- **Utilidades nuevas:** 1 funcion de error especifica para tareas de promocion, 2 mappers, 1 constante de validacion

### Estado del codigo existente

Los tipos y schemas de crowdpromotion ya existen en:
- `src/shared/types/crowdpromotion.ts` - AMPLIAR con 8 interfaces + 1 union type
- `src/shared/schemas/crowdpromotion.schema.ts` - AMPLIAR con 3 schemas nuevos
- `src/shared/constants/index.ts` - AMPLIAR con endpoints, query keys, estados y errores
- `src/shared/utils/error-messages.ts` - AMPLIAR con error messages para tareas de promocion
- `src/shared/utils/mappers.ts` - AMPLIAR con 2 mappers

Ningun archivo nuevo que crear. Todo se agrega a los archivos existentes siguiendo el patron de seccion con comentario de bloque ya establecido.

---

## 2. Types (`src/shared/types/crowdpromotion.ts`)

### 2.1 Estrategia: Agregar seccion al final del archivo existente

El archivo `crowdpromotion.ts` ya contiene secciones separadas por comentarios de bloque (pattern `// ========== ... ==========`). Se agrega una seccion nueva al final con el marcador `// ========== Tareas de Promocion - US-CP-04 ==========`.

### 2.2 Union Type: EstadoTareaPromo

| Tipo | Valores | Descripcion |
|------|---------|-------------|
| `EstadoTareaPromo` | `1 \| 2 \| 3 \| 4` | Numeric union alineado con `Maestra_EstadoTareaPromo`. 1=Pendiente, 2=Completada, 3=Validada, 4=Rechazada |

Justificacion de usar numeric union en lugar de string union: el backend devuelve `estadoTareaId` como `int`, y el objeto `miEstado` lo incluye como numero. Esto es coherente con el patron de `CAMPANIA_ESTADOS` que tambien usa numericos y el campo `EstadoTareaId int` del modelo de dominio.

### 2.3 DTOs de Response - Vista Promotor (GET mis-tareas)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `MiEstadoTarea` | `tareaPromotorId: string`, `estadoTareaId: EstadoTareaPromo`, `estadoTareaNombre: string`, `vecesCompletada: number`, `fechaPrimeraCompletada: string \| undefined`, `fechaUltimaCompletada: string \| undefined`, `urlPruebaCompletado: string \| undefined`, `comentarioValidacion: string \| undefined` | Estado del promotor para una tarea especifica. Null si nunca completo. Campos calculados en query. |
| `MisTareasItem` | `tareaId: string`, `nombre: string`, `descripcion: string \| undefined`, `instruccionesUrl: string \| undefined`, `tipoEventoPromoNombre: string`, `tipoRewardNombre: string \| undefined`, `importeRecompensa: number \| undefined`, `monedaNombre: string \| undefined`, `puntosRecompensa: number \| undefined`, `esRepetible: boolean`, `maxRepeticiones: number \| undefined`, `orden: number`, `miEstado: MiEstadoTarea \| undefined` | Un item del listado de tareas del promotor. miEstado es undefined si el promotor nunca ha completado la tarea. |
| `MisTareasResponse` | `programaId: string`, `programaTitulo: string`, `items: MisTareasItem[]` | Wrapper del response GET mis-tareas |

### 2.4 DTOs de Response - Completar Tarea (POST completar)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CompletarTareaResponse` | `tareaPromotorId: string`, `estadoTareaId: EstadoTareaPromo`, `estadoTareaNombre: string`, `vecesCompletada: number`, `fechaUltimaCompletada: string \| undefined` | Response del POST completar. Siempre estadoTareaId=2 (Completada) en exito. |

### 2.5 DTOs de Response - Vista Artista (GET tareas-pendientes)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `TareaPendienteItem` | `tareaPromotorId: string`, `tareaId: string`, `tareaNombre: string`, `promotorId: string`, `promotorNombre: string`, `promotorTipoNombre: string \| undefined`, `urlPruebaCompletado: string \| undefined`, `comentarioPromotor: string \| undefined`, `vecesCompletada: number`, `fechaUltimaCompletada: string \| undefined` | Un item de la lista de completados pendientes de validacion. Solo EstadoTareaId=2. |
| `TareasPendientesResponse` | `items: TareaPendienteItem[]`, `totalCount: number`, `page: number`, `pageSize: number`, `totalPages: number` | Response paginado GET tareas-pendientes. Coincide con el patron PaginatedResponse<T> del proyecto. |

### 2.6 DTOs de Response - Validar/Rechazar (PATCH validar / PATCH rechazar)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `ValidarTareaResponse` | `tareaPromotorId: string`, `estadoTareaId: EstadoTareaPromo`, `estadoTareaNombre: string`, `recompensaAcreditada: number \| undefined`, `monedaNombre: string \| undefined`, `puntosAcreditados: number \| undefined` | Response del PATCH validar. recompensaAcreditada es undefined si la tarea no tiene recompensa monetaria. |
| `RechazarTareaResponse` | `tareaPromotorId: string`, `estadoTareaId: EstadoTareaPromo`, `estadoTareaNombre: string` | Response del PATCH rechazar. Minimal: solo confirma el nuevo estado. |

### 2.7 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `CompletarTareaRequest` | `urlPruebaCompletado: string`, `comentarioPromotor?: string` | Body del POST completar. Alineado con `CompletarTareaDto` del backend. |
| `ValidarTareaRequest` | `comentarioValidacion?: string` | Body del PATCH validar. comentarioValidacion opcional. |
| `RechazarTareaRequest` | `comentarioValidacion: string` | Body del PATCH rechazar. comentarioValidacion OBLIGATORIO. |

### 2.8 Alineamiento Backend-Frontend

| Interface TS | DTO C# Backend | Alineamiento |
|---|---|---|
| `MiEstadoTarea` | `MiEstadoTareaDto` | Completo. `null` en C# se modela como `undefined` en TS siguiendo la convencion del proyecto. |
| `MisTareasItem` | `MisTareasItemDto` | Completo. |
| `MisTareasResponse` | `MisTareasResponseDto` | Completo. |
| `CompletarTareaRequest` | `CompletarTareaDto` | Completo. |
| `CompletarTareaResponse` | `CompletarTareaResponseDto` | Completo. |
| `TareaPendienteItem` | `TareaPendienteItemDto` | Completo. |
| `TareasPendientesResponse` | `TareasPendientesResponseDto` | Completo. |
| `ValidarTareaRequest` | `ValidarTareaDto` | Completo. |
| `ValidarTareaResponse` | `ValidarTareaResponseDto` | Completo. |
| `RechazarTareaRequest` | `RechazarTareaDto` | Completo. |
| `RechazarTareaResponse` | `RechazarTareaResponseDto` | Completo. |

### 2.9 Nota: Uso de `undefined` vs `null`

El proyecto usa `undefined` para campos opcionales (ver `TareaResumen`, `MiInscripcion`, etc.) en lugar de `string | null`. Esto es consistente con las rules de TypeScript del proyecto. Los campos que el backend retorna como `null` se modelan como `string | undefined` en el TS. Los mappers se encargan de la conversion si fuera necesario.

---

## 3. Schemas Zod (`src/shared/schemas/crowdpromotion.schema.ts`)

### 3.1 Estrategia: Agregar al final del archivo existente

El archivo ya tiene schemas para promotor, programa e inscripcion. Se agrega nueva seccion al final con comentario `// ========== Tareas de Promocion - US-CP-04 ==========`.

### 3.2 Schemas de Validacion

| Schema | Campos | Reglas de validacion | Tipo inferido |
|--------|--------|----------------------|---------------|
| `completarTareaSchema` | `urlPruebaCompletado`, `comentarioPromotor?` | `urlPruebaCompletado`: `.string().min(1, 'La URL de prueba es obligatoria').max(2048, 'La URL no puede superar los 2048 caracteres').url('La URL de prueba no tiene formato valido')`. `comentarioPromotor`: `.string().max(500, 'El comentario no puede superar los 500 caracteres').optional()` | `CompletarTareaFormData` |
| `validarTareaSchema` | `comentarioValidacion?` | `comentarioValidacion`: `.string().max(500, 'El comentario no puede superar los 500 caracteres').optional()` | `ValidarTareaFormData` |
| `rechazarTareaSchema` | `comentarioValidacion` | `comentarioValidacion`: `.string().min(1, 'El motivo de rechazo es obligatorio').max(500, 'El motivo de rechazo no puede superar los 500 caracteres')` | `RechazarTareaFormData` |

### 3.3 Alineamiento con FluentValidation del Backend

| Campo | Regla Backend | Regla Zod | Alineado |
|-------|---------------|-----------|---------|
| `urlPruebaCompletado` requerido | `.NotEmpty()` / ErrorCode `1001` | `.min(1, 'La URL de prueba es obligatoria')` | Si |
| `urlPruebaCompletado` formato URL | `.Must(BeAValidUrl)` / ErrorCode `1013` | `.url('La URL de prueba no tiene formato valido')` | Si |
| `urlPruebaCompletado` max length | `.MaximumLength(2048)` / ErrorCode `1002` | `.max(2048, 'La URL no puede superar los 2048 caracteres')` | Si |
| `comentarioPromotor` max length | `.MaximumLength(500)` / ErrorCode `1002` | `.max(500, 'El comentario no puede superar los 500 caracteres').optional()` | Si |
| `comentarioValidacion` (validar) max | `.MaximumLength(500)` / ErrorCode `1002` | `.max(500, '...').optional()` | Si |
| `comentarioValidacion` (rechazar) requerido | `.NotEmpty()` / ErrorCode `1001` | `.min(1, 'El motivo de rechazo es obligatorio')` | Si |
| `comentarioValidacion` (rechazar) max | `.MaximumLength(500)` / ErrorCode `1002` | `.max(500, 'El motivo de rechazo no puede superar los 500 caracteres')` | Si |

### 3.4 Nota sobre `completarTareaSchema` y URL vacía

No se agrega `.or(z.literal(''))` al campo `urlPruebaCompletado` porque es obligatorio (el campo de URL de prueba no puede quedar vacio). Esto difiere del patron de URLs opcionales usadas en otros schemas (como `urlInstrucciones` en `createPromoTareaSchema`). Si el formulario usa un input controlado que puede emitir cadena vacia, el componente debe asegurarse de no presubmitir; la validacion Zod rechazara la cadena vacia gracias al `.min(1)`.

---

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Endpoints API: Nuevas entradas en `API_ROUTES.crowdpromotion`

La seccion `crowdpromotion` del objeto `API_ROUTES` ya existe. Se agregan 5 nuevas propiedades:

| Propiedad | Valor | Endpoint |
|-----------|-------|----------|
| `misTareas: (programaId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/mis-tareas` `` | `GET /api/crowdpromotion/programas/{programaId}/mis-tareas` |
| `completarTarea: (programaId: string, tareaId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/tareas/${tareaId}/completar` `` | `POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar` |
| `tareasPendientes: (programaId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/tareas-pendientes` `` | `GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes` |
| `validarTarea: (programaId: string, tareaPromotorId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/validar` `` | `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar` |
| `rechazarTarea: (programaId: string, tareaPromotorId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/rechazar` `` | `PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar` |

Estas 5 funciones se agregan al objeto `programas` de `API_ROUTES.crowdpromotion` existente, ya que todos los endpoints estan bajo el prefijo `/programas/{id}`.

### 4.2 Query Keys: Nuevas entradas en `QUERY_KEYS.crowdpromotion`

La seccion `crowdpromotion` del objeto `QUERY_KEYS` ya tiene `promotor`, `maestras`, `programas` e `inscripciones`. Se agrega una nueva subseccion `tareas`:

| Key | Patron | Uso |
|-----|--------|-----|
| `tareas.mis: (programaId: string) => readonly [...]` | `['crowdpromotion', 'tareas', programaId, 'mis'] as const` | Invalidar/fetch GET mis-tareas del promotor para un programa dado. Parametrizado por programaId para granularidad. |
| `tareas.pendientes: (programaId: string, params?: Record<string, unknown>) => readonly [...]` | `['crowdpromotion', 'tareas', programaId, 'pendientes', params] as const` | Invalidar/fetch GET tareas-pendientes del artista. params incluye `page` y `pageSize` para paginacion. |

Ejemplo de uso en hooks:
```typescript
// Landing: query de tareas del promotor
queryKey: QUERY_KEYS.crowdpromotion.tareas.mis(inscripcion.programaId)

// Admin: query de tareas pendientes con paginacion
queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId, { page: 1, pageSize: 10 })

// Invalidar tras completar una tarea
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.tareas.mis(programaId) })

// Invalidar tras validar o rechazar
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId) })
```

### 4.3 Estado de Tarea de Promocion

Se agrega al final del archivo, en la seccion `// ========== Crowdpromotion - Estado de Tarea de Promocion (US-CP-04) ==========`:

| Constante | Tipo | Valores |
|-----------|------|---------|
| `ESTADO_TAREA_PROMO` | `const object` | `PENDIENTE: 1, COMPLETADA: 2, VALIDADA: 3, RECHAZADA: 4` |
| `ESTADO_TAREA_PROMO_LABELS` | `Record<number, string>` | `1: 'Pendiente', 2: 'Completada', 3: 'Validada', 4: 'Rechazada'` |
| `ESTADO_TAREA_PROMO_BADGES` | `Record<number, string>` | `1: 'secondary', 2: 'warning', 3: 'success', 4: 'destructive'` |

Nota: Los valores de badge siguen el patron `ESTADO_PROPUESTA_BADGES` y `ESTADO_ENTREGABLE_BADGES` ya existentes en el archivo. Se usan valores de variant de shadcn/ui Badge: `secondary` (gris), `warning` (amarillo - via Tailwind custom), `success` (verde - via Tailwind custom), `destructive` (rojo).

| Constante | Tipo | Descripcion |
|-----------|------|-------------|
| `TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE` | `number = 10` | Tamano de pagina por defecto para GET tareas-pendientes (alineado con RNF-03 del contracts.md) |
| `TAREAS_PENDIENTES_MAX_PAGE_SIZE` | `number = 50` | Tamano maximo de pagina |

### 4.4 Constantes de Validacion nuevas en `VALIDATION`

Se agregan al objeto `VALIDATION` existente:

| Clave | Valor | Descripcion |
|-------|-------|-------------|
| `TAREA_URL_PRUEBA_MAX` | `2048` | Max length de `urlPruebaCompletado` (alineado con contracts.md campo `urlPruebaCompletado max 2048`) |
| `TAREA_COMENTARIO_PROMOTOR_MAX` | `500` | Max length de `comentarioPromotor` |
| `TAREA_COMENTARIO_VALIDACION_MAX` | `500` | Max length de `comentarioValidacion` |

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Nuevas entradas en `error-messages.ts`

Se agregan al objeto `ERROR_CODE_MESSAGES` los 7 codigos nuevos definidos en contracts.md:

| Codigo | Mensaje | Contexto de uso |
|--------|---------|-----------------|
| `'2021'` | `'La tarea no existe o no pertenece a este programa.'` | GET/POST cuando `tareaId` no existe en el programa |
| `'2022'` | `'El completado no existe o no pertenece a este programa.'` | PATCH validar/rechazar cuando `tareaPromotorId` no existe |
| `'4027'` | `'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.'` | POST completar en tarea no repetible ya completada/validada |
| `'4028'` | `'Has alcanzado el numero maximo de veces que puedes completar esta tarea.'` | POST completar en tarea repetible con MaxRepeticiones alcanzado |
| `'4029'` | `'Esta tarea ya no esta disponible.'` | POST completar cuando `PromoTarea.EsActivo = false` |
| `'4030'` | `'El plazo para completar esta tarea ha finalizado.'` | POST completar cuando `PromoTarea.FechaFin < now()` |
| `'4031'` | `'Este completado no puede ser procesado porque ya fue validado o rechazado.'` | PATCH validar/rechazar cuando `EstadoTareaId != 2` |

Se agrega tambien una funcion especializada `getTareaPromocionErrorMessage` en la seccion `// ========== Crowdpromotion - Tareas de Promocion Error Messages (US-CP-04) ==========` al final del archivo:

```typescript
export const TAREA_PROMOCION_ERROR_MESSAGES: Record<string, string> = {
    // Numeric codes (override global codes for tareas context)
    '1001': 'La URL de prueba es obligatoria.',
    '1002': 'El campo supera el maximo de caracteres permitido.',
    '1013': 'La URL de prueba no tiene formato valido. Usa una URL completa (ej: https://...)',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '2016': 'No tienes un perfil de artista.',
    '2019': 'El programa de promocion no existe o fue eliminado.',
    '2021': 'La tarea no existe o no pertenece a este programa.',
    '2022': 'El completado no existe o no pertenece a este programa.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente.',
    '4024': 'Este programa no esta activo en este momento.',
    '4026': 'No tienes permiso para acceder a este programa.',
    '4027': 'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.',
    '4028': 'Has alcanzado el numero maximo de veces que puedes completar esta tarea.',
    '4029': 'Esta tarea ya no esta disponible.',
    '4030': 'El plazo para completar esta tarea ha finalizado.',
    '4031': 'Este completado no puede ser procesado porque ya fue validado o rechazado.',
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente.',

    // Semantic keys para uso interno en hooks y componentes
    TAREA_NO_REPETIBLE: 'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.',
    MAX_REPETICIONES_ALCANZADO: 'Has alcanzado el numero maximo de veces que puedes completar esta tarea.',
    TAREA_INACTIVA: 'Esta tarea ya no esta disponible.',
    TAREA_FUERA_FECHA: 'El plazo para completar esta tarea ha finalizado.',
    COMPLETADO_ESTADO_INVALIDO: 'Este completado no puede ser procesado porque ya fue validado o rechazado.',
    TAREA_NOT_FOUND: 'La tarea no existe o no pertenece a este programa.',
    COMPLETADO_NOT_FOUND: 'El completado no existe o no pertenece a este programa.',
} as const;

export const getTareaPromocionErrorMessage = (errorCode: string): string => {
    return TAREA_PROMOCION_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### 5.2 Nuevos mappers en `mappers.ts`

Se agrega al final del archivo la seccion `// ========== Crowdpromotion - Tareas de Promocion Mappers (US-CP-04) ==========`:

| Funcion | Input | Output | Descripcion |
|---------|-------|--------|-------------|
| `mapEstadoTareaPromoToBadge` | `estadoTareaId: number` | `string` | Devuelve el variant del Badge de shadcn para el estado dado. Usa `ESTADO_TAREA_PROMO_BADGES`. |
| `puedeCompletarTarea` | `item: MisTareasItem` | `boolean` | Logica de negocio para determinar si el boton "Completar" debe mostrarse. Retorna `false` si: tarea no repetible con miEstado.estadoTareaId == 2 o 3; o tarea repetible con vecesCompletada >= maxRepeticiones. Util para Landing y Admin. |

Ejemplo de implementacion planificada para `puedeCompletarTarea`:
```typescript
// Retorna false si la tarea no puede ser completada por el promotor
// - Tarea no repetible con un completado activo (estado 2=Completada o 3=Validada)
// - Tarea repetible con MaxRepeticiones alcanzado
// - miEstado no definido (nunca completada) -> siempre puede completar
export function puedeCompletarTarea(item: MisTareasItem): boolean {
    if (!item.miEstado) return true;
    if (!item.esRepetible) {
        return item.miEstado.estadoTareaId === 4; // Solo puede si fue rechazada (re-envio)
    }
    if (item.maxRepeticiones != null) {
        return item.miEstado.vecesCompletada < item.maxRepeticiones;
    }
    return true; // Repetible sin limite
}
```

Nota: `puedeCompletarTarea` encapsula las reglas de negocio RN-04 y RN-05 del contracts.md. Evitar duplicacion de esta logica en Landing y Admin.

---

## 6. Archivos a Modificar

```
src/shared/
├── types/
│   └── crowdpromotion.ts         AMPLIAR: nueva seccion US-CP-04 al final
├── schemas/
│   └── crowdpromotion.schema.ts  AMPLIAR: 3 schemas + types inferidos al final
├── constants/
│   └── index.ts                  AMPLIAR: endpoints, query keys, estados, validation
└── utils/
    ├── error-messages.ts         AMPLIAR: 7 nuevos error codes + funcion getTareaPromocionErrorMessage
    └── mappers.ts                AMPLIAR: 2 nuevas funciones
```

Ningun archivo nuevo. Ningun cambio en `index.ts` de tipos/schemas/utils porque esos re-exportan con `export * from './crowdpromotion'` (ya existente).

---

## 7. Dependencias

- `zod` ya instalado en `src/shared/node_modules/zod`
- Ningun package adicional requerido
- No se requiere cambio en `tsconfig.json` ni `package.json`

---

## 8. Notas de Implementacion

### 8.1 Naming convention para types nuevos

Se sigue el patron del archivo existente `crowdpromotion.ts`:
- Interfaces con `interface` (no `type`) para todos los objetos DTO
- Union types con `type` para `EstadoTareaPromo`
- PascalCase para todos los nombres de tipo

### 8.2 `undefined` vs `null` para campos opcionales

El proyecto modela campos opcionales como `string | undefined` (no `string | null`) en los types TypeScript, aunque el backend devuelva `null`. Esto es consistente con el resto del archivo `crowdpromotion.ts`. Los servicios de API deben mapear `null -> undefined` si es necesario, o usar el operador `?? undefined` al leer la respuesta.

### 8.3 `EstadoTareaPromo` como numeric union, no const object

El contracts.md define `ESTADO_TAREA_PROMO` como objeto de constantes Y `EstadoTareaPromo` como union type. La implementacion sigue el patron del proyecto:
- `ESTADO_TAREA_PROMO` en `constants/index.ts` (el lugar canonico de las constantes del dominio)
- `EstadoTareaPromo = 1 | 2 | 3 | 4` en `types/crowdpromotion.ts` (para tipado de las interfaces)
- Esto sigue el patron de `CAMPANIA_ESTADOS` (constante en constants.ts) vs uso directo del numero en types

### 8.4 TareasPendientesResponse y el patron PaginatedResponse

`TareasPendientesResponse` tiene exactamente las mismas propiedades que `PaginatedResponse<TareaPendienteItem>` definido en `types/api.ts`. Sin embargo, se define como interface propia (no como alias) para mantener la consistencia con `MisProgramasResponse`, `InscripcionesListResponse` etc. que siguen el mismo patron en el archivo existente.

### 8.5 Posicion de las 5 rutas en API_ROUTES.crowdpromotion.programas

Los 5 endpoints nuevos van dentro del objeto `programas` de `API_ROUTES.crowdpromotion` (no en un subobjeto `tareas` separado) porque todos siguen el path `/programas/{id}/...`. Esto mantiene coherencia con el patron existente donde `aprobar`, `rechazar`, `bloquear`, `darDeBaja` estan dentro de `programas`.

### 8.6 Invalidacion de queries tras mutaciones

El plan asume el siguiente patron de invalidacion (documentado para los agentes de Landing y Admin):

| Mutacion | Queries a invalidar |
|----------|---------------------|
| POST completarTarea | `QUERY_KEYS.crowdpromotion.tareas.mis(programaId)` |
| PATCH validarTarea | `QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId)` + opcionalmente mis-tareas si la Landing tiene la tarea en cache |
| PATCH rechazarTarea | `QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId)` |

---

## 9. Checklist de Implementacion

- [ ] `types/crowdpromotion.ts`: Agregar `EstadoTareaPromo` union type
- [ ] `types/crowdpromotion.ts`: Agregar `MiEstadoTarea` interface
- [ ] `types/crowdpromotion.ts`: Agregar `MisTareasItem` interface
- [ ] `types/crowdpromotion.ts`: Agregar `MisTareasResponse` interface
- [ ] `types/crowdpromotion.ts`: Agregar `CompletarTareaRequest` interface
- [ ] `types/crowdpromotion.ts`: Agregar `CompletarTareaResponse` interface
- [ ] `types/crowdpromotion.ts`: Agregar `TareaPendienteItem` interface
- [ ] `types/crowdpromotion.ts`: Agregar `TareasPendientesResponse` interface
- [ ] `types/crowdpromotion.ts`: Agregar `ValidarTareaRequest` interface
- [ ] `types/crowdpromotion.ts`: Agregar `ValidarTareaResponse` interface
- [ ] `types/crowdpromotion.ts`: Agregar `RechazarTareaRequest` interface
- [ ] `types/crowdpromotion.ts`: Agregar `RechazarTareaResponse` interface
- [ ] `schemas/crowdpromotion.schema.ts`: Agregar `completarTareaSchema` + `CompletarTareaFormData`
- [ ] `schemas/crowdpromotion.schema.ts`: Agregar `validarTareaSchema` + `ValidarTareaFormData`
- [ ] `schemas/crowdpromotion.schema.ts`: Agregar `rechazarTareaSchema` + `RechazarTareaFormData`
- [ ] `constants/index.ts`: Agregar 5 endpoints en `API_ROUTES.crowdpromotion.programas`
- [ ] `constants/index.ts`: Agregar subseccion `tareas` en `QUERY_KEYS.crowdpromotion`
- [ ] `constants/index.ts`: Agregar `ESTADO_TAREA_PROMO`, `ESTADO_TAREA_PROMO_LABELS`, `ESTADO_TAREA_PROMO_BADGES`
- [ ] `constants/index.ts`: Agregar `TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE`, `TAREAS_PENDIENTES_MAX_PAGE_SIZE`
- [ ] `constants/index.ts`: Agregar `TAREA_URL_PRUEBA_MAX`, `TAREA_COMENTARIO_PROMOTOR_MAX`, `TAREA_COMENTARIO_VALIDACION_MAX` en `VALIDATION`
- [ ] `utils/error-messages.ts`: Agregar 7 nuevos codigos en `ERROR_CODE_MESSAGES`
- [ ] `utils/error-messages.ts`: Agregar `TAREA_PROMOCION_ERROR_MESSAGES` y `getTareaPromocionErrorMessage`
- [ ] `utils/mappers.ts`: Agregar `mapEstadoTareaPromoToBadge`
- [ ] `utils/mappers.ts`: Agregar `puedeCompletarTarea`
- [ ] Verificar que todos los exports son accesibles via `src/shared/index.ts` (no requiere cambios)
- [ ] Verificar alineamiento de tipos con ejemplo JSON del contracts.md
