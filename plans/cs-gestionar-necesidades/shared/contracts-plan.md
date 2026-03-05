# Plan de Contratos Shared: Gestionar Necesidades de Crowdsourcing

**Fecha:** 2026-02-16
**Feature:** cs-gestionar-necesidades
**Basado en:** docs/user-stories/cs-gestionar-necesidades/contracts.md

---

## 1. Resumen

- **Total de types:** 9 interfaces + 2 union types
- **Total de schemas:** 3 schemas Zod con 5+ refines
- **Constantes definidas:** 15 grupos (QUERY_KEYS, API_ROUTES, APP_ROUTES, estados, modalidades, monedas, badges)
- **Utilidades planificadas:** 1 formatter + 12 error messages

**Estrategia:**
- **MODIFICAR** `src/shared/types/crowdsourcing.ts` - Agregar types de necesidades
- **MODIFICAR** `src/shared/schemas/crowdsourcing.schema.ts` - Agregar schemas de necesidades
- **MODIFICAR** `src/shared/constants/index.ts` - Agregar constantes de necesidades
- **MODIFICAR** `src/shared/utils/error-messages.ts` - Agregar error messages de necesidades
- **NUEVO** helper `formatPresupuesto` en `src/shared/utils/format.ts`

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

**Acción:** MODIFICAR archivo existente - agregar al final después de tipos de templates

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripción |
|------|-------------|-------------|
| `NecesidadCrowdsourcingList` | `id`, `titulo`, `estadoNecesidadId`, `estadoNecesidadNombre`, `tipoNecesidadId`, `tipoNecesidadNombre`, `presupuestoMin?`, `presupuestoMax?`, `monedaId?`, `monedaNombre?`, `modalidadTrabajoId`, `modalidadTrabajoNombre`, `numeroPropuestas`, `fechaCreacion`, `fechaLimitePropuestas?`, `fechaActualizacion?` | DTO para listado con contador de propuestas |
| `NecesidadCrowdsourcing` | `id`, `titulo`, `descripcion?`, `tipoNecesidadId`, `tipoNecesidadNombre`, `estadoNecesidadId`, `estadoNecesidadNombre`, `modalidadTrabajoId`, `modalidadTrabajoNombre`, `presupuestoMin?`, `presupuestoMax?`, `monedaId?`, `monedaNombre?`, `ubicacionCiudad?`, `ubicacionPais?`, `fechaLimitePropuestas?`, `fechaInicioPrevista?`, `fechaCreacion`, `fechaActualizacion?`, `proyectoArtisticoId`, `proyectoArtisticoNombre`, `propuestas[]` | DTO completo con propuestas anidadas |
| `PropuestaCrowdsourcing` | `id`, `profesionalId`, `profesionalNombre`, `precioPropuesto`, `monedaId`, `tiempoEstimadoDias?`, `mensaje`, `estadoPropuestaId`, `estadoPropuestaNombre`, `fechaCreacion` | DTO de propuesta recibida (nested en NecesidadCrowdsourcing) |
| `NecesidadCreateResult` | `id`, `titulo`, `estadoNecesidadId`, `estadoNecesidadNombre`, `fechaCreacion` | DTO de respuesta al crear |
| `NecesidadUpdateResult` | `id`, `titulo`, `estadoNecesidadId`, `estadoNecesidadNombre`, `fechaActualizacion?` | DTO de respuesta al actualizar |
| `CerrarNecesidadResult` | `id`, `estadoNecesidadNombre`, `propuestasRechazadas` | DTO de respuesta al cerrar |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripción |
|------|-------------|-------------|
| `CreateNecesidadRequest` | `titulo`, `descripcion?`, `tipoNecesidadId`, `modalidadTrabajoId`, `presupuestoMin?`, `presupuestoMax?`, `monedaId?`, `ubicacionCiudad?`, `ubicacionPais?`, `fechaLimitePropuestas?`, `fechaInicioPrevista?`, `proyectoArtisticoId` | Request para crear necesidad |
| `UpdateNecesidadRequest` | `titulo`, `descripcion?`, `modalidadTrabajoId`, `presupuestoMin?`, `presupuestoMax?`, `monedaId?`, `ubicacionCiudad?`, `ubicacionPais?`, `fechaLimitePropuestas?`, `fechaInicioPrevista?` | Request para editar (sin tipoNecesidadId ni proyectoArtisticoId) |
| `CerrarNecesidadRequest` | `motivo?` | Request para cerrar necesidad |

### 2.3 Enums y Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `EstadoNecesidad` (union type) | `"Abierta"` \| `"En Progreso"` \| `"Cerrada"` \| `"Cancelada"` | Estado de necesidad (labels) |
| `ModalidadTrabajo` (union type) | `"Presencial"` \| `"Remoto"` \| `"Híbrido"` | Modalidad de trabajo (labels) |

**Nota:** Los tipos de response incluyen tanto el `*Id` (número) como el `*Nombre` (string) para facilitar el renderizado en UI sin lookups adicionales.

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

**Acción:** MODIFICAR archivo existente - agregar después de schemas de templates

### 3.1 Schemas de Validación

| Schema | Campos | Reglas Críticas |
|--------|--------|-----------------|
| `createNecesidadSchema` | `titulo`, `descripcion?`, `tipoNecesidadId`, `modalidadTrabajoId`, `presupuestoMin?`, `presupuestoMax?`, `monedaId?`, `ubicacionCiudad?`, `ubicacionPais?`, `fechaLimitePropuestas?`, `fechaInicioPrevista?`, `proyectoArtisticoId` | **5 refines:** (1) presupuestoMax >= presupuestoMin, (2) monedaId requerido si presupuesto presente, (3) ubicacionCiudad requerida si modalidad Presencial/Híbrido, (4) fechaLimitePropuestas > hoy, (5) fechaInicioPrevista >= hoy |
| `updateNecesidadSchema` | Mismo que `createNecesidadSchema` pero sin `tipoNecesidadId` ni `proyectoArtisticoId` | **Mismos 5 refines** que create (excepto validar proyecto) |
| `cerrarNecesidadSchema` | `motivo?` (string, max 500) | Motivo opcional, validar max length |

**Detalles de validación:**

**Campo `titulo`:**
```typescript
z.string()
  .min(1, 'El título es obligatorio')
  .min(5, 'El título debe tener al menos 5 caracteres')
  .max(200, 'El título no puede superar los 200 caracteres')
```

**Campo `descripcion`:**
```typescript
z.string()
  .max(4000, 'La descripción no puede superar los 4000 caracteres')
  .optional()
```

**Campo `tipoNecesidadId`:**
```typescript
z.number()
  .positive('El tipo de necesidad es obligatorio')
```

**Campo `modalidadTrabajoId`:**
```typescript
z.number()
  .positive('La modalidad de trabajo es obligatoria')
```

**Campo `presupuestoMin/Max`:**
```typescript
z.number()
  .nonnegative('El presupuesto no puede ser negativo')
  .optional()
```

**Campo `monedaId`:**
```typescript
z.number()
  .positive('La moneda es obligatoria')
  .optional()
```

**Campo `ubicacionCiudad/Pais`:**
```typescript
z.string().max(100).optional()
```

**Campo `fechaLimitePropuestas` y `fechaInicioPrevista`:**
```typescript
z.string().optional()  // ISO 8601 date string
```

**Refine 1 - Presupuesto Max >= Min:**
```typescript
.refine(
  (data) => {
    if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
      return data.presupuestoMax >= data.presupuestoMin;
    }
    return true;
  },
  {
    message: 'El presupuesto máximo debe ser mayor o igual al mínimo',
    path: ['presupuestoMax'],
  }
)
```

**Refine 2 - Moneda requerida si presupuesto:**
```typescript
.refine(
  (data) => {
    if (data.presupuestoMin !== undefined || data.presupuestoMax !== undefined) {
      return data.monedaId !== undefined;
    }
    return true;
  },
  {
    message: 'La moneda es obligatoria cuando se especifica presupuesto',
    path: ['monedaId'],
  }
)
```

**Refine 3 - Ubicación requerida para Presencial/Híbrido:**
```typescript
.refine(
  (data) => {
    if (data.modalidadTrabajoId === 1 || data.modalidadTrabajoId === 3) {
      return data.ubicacionCiudad && data.ubicacionCiudad.length > 0;
    }
    return true;
  },
  {
    message: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
    path: ['ubicacionCiudad'],
  }
)
```

**Refine 4 - Fecha límite futura:**
```typescript
.refine(
  (data) => {
    if (data.fechaLimitePropuestas) {
      const limite = new Date(data.fechaLimitePropuestas);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return limite > hoy;
    }
    return true;
  },
  {
    message: 'La fecha límite debe ser posterior a hoy',
    path: ['fechaLimitePropuestas'],
  }
)
```

**Refine 5 - Fecha inicio >= hoy:**
```typescript
.refine(
  (data) => {
    if (data.fechaInicioPrevista) {
      const inicio = new Date(data.fechaInicioPrevista);
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      return inicio >= hoy;
    }
    return true;
  },
  {
    message: 'La fecha de inicio debe ser igual o posterior a hoy',
    path: ['fechaInicioPrevista'],
  }
)
```

### 3.2 Types Inferidos

```typescript
export type CreateNecesidadFormData = z.infer<typeof createNecesidadSchema>;
export type UpdateNecesidadFormData = z.infer<typeof updateNecesidadSchema>;
export type CerrarNecesidadFormData = z.infer<typeof cerrarNecesidadSchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Acción:** MODIFICAR archivo existente - agregar en secciones correspondientes

### 4.1 QUERY_KEYS (agregar en `QUERY_KEYS.crowdsourcing`)

**Ubicación:** Dentro del objeto `QUERY_KEYS.crowdsourcing` existente

```typescript
export const QUERY_KEYS = {
  // ... existing keys

  crowdsourcing: {
    templates: {
      all: ['crowdsourcing', 'templates'] as const,
      byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
    },
    maestras: {
      rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
      categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
      // NUEVAS maestras
      tiposNecesidad: ['crowdsourcing', 'maestras', 'tipos-necesidad'] as const,
      modalidadesTrabajo: ['crowdsourcing', 'maestras', 'modalidades'] as const,
      monedas: ['crowdsourcing', 'maestras', 'monedas'] as const,
    },
    // NUEVO objeto necesidades
    necesidades: {
      mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
      byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
    },
  },
};
```

| Key | Patrón | Uso |
|-----|--------|-----|
| `crowdsourcing.maestras.tiposNecesidad` | `['crowdsourcing', 'maestras', 'tipos-necesidad']` | Lista de tipos de necesidad |
| `crowdsourcing.maestras.modalidadesTrabajo` | `['crowdsourcing', 'maestras', 'modalidades']` | Lista de modalidades |
| `crowdsourcing.maestras.monedas` | `['crowdsourcing', 'maestras', 'monedas']` | Lista de monedas |
| `crowdsourcing.necesidades.mis` | `['crowdsourcing', 'necesidades', 'mis']` | Mis necesidades paginadas |
| `crowdsourcing.necesidades.byId(id)` | `['crowdsourcing', 'necesidades', id]` | Detalle de necesidad |

### 4.2 API_ROUTES (agregar en `API_ROUTES.crowdsourcing`)

**Ubicación:** Dentro del objeto `API_ROUTES.crowdsourcing` existente

```typescript
export const API_ROUTES = {
  // ... existing routes

  crowdsourcing: {
    templates: {
      base: '/api/crowdsourcing/templates',
      byId: (id: string) => `/api/crowdsourcing/templates/${id}`,
      generar: (id: string) => `/api/crowdsourcing/templates/${id}/generar`,
    },
    maestras: {
      rolesProfesionales: '/api/crowdsourcing/maestras/roles-profesionales',
      categoriasRol: '/api/crowdsourcing/maestras/categorias-rol',
      // NUEVAS maestras
      tiposNecesidad: '/api/crowdsourcing/maestras/tipos-necesidad',
      modalidadesTrabajo: '/api/crowdsourcing/maestras/modalidades-trabajo',
      monedas: '/api/crowdsourcing/maestras/monedas',
    },
    // NUEVO objeto necesidades
    necesidades: {
      base: '/api/crowdsourcing/necesidades',
      mis: '/api/crowdsourcing/necesidades/mis-necesidades',
      byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
      cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
    },
  },
};
```

| Constante | Valor | Uso |
|-----------|-------|-----|
| `necesidades.base` | `/api/crowdsourcing/necesidades` | POST crear necesidad |
| `necesidades.mis` | `/api/crowdsourcing/necesidades/mis-necesidades` | GET listado con filtros |
| `necesidades.byId(id)` | `/api/crowdsourcing/necesidades/{id}` | GET detalle, PUT editar |
| `necesidades.cerrar(id)` | `/api/crowdsourcing/necesidades/{id}/cerrar` | PATCH cerrar necesidad |

### 4.3 APP_ROUTES (agregar en `APP_ROUTES.dashboard.crowdsourcing`)

**Ubicación:** Dentro del objeto `APP_ROUTES.dashboard.crowdsourcing` existente

```typescript
export const APP_ROUTES = {
  // ... existing routes

  dashboard: {
    // ... existing dashboard routes
    crowdsourcing: {
      templates: '/dashboard/crowdsourcing/templates',
      templateDetail: (id: string) => `/dashboard/crowdsourcing/templates/${id}`,
      wizard: (projectId: string) => `/dashboard/crowdsourcing/wizard/${projectId}`,
      // NUEVAS rutas de necesidades
      necesidades: '/dashboard/crowdsourcing/necesidades',
      nuevaNecesidad: '/dashboard/crowdsourcing/necesidades/nueva',
      necesidadDetail: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}`,
      editarNecesidad: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}/editar`,
    },
  },
};
```

| Ruta | Función | Uso |
|------|---------|-----|
| `necesidades` | `/dashboard/crowdsourcing/necesidades` | Listado mis necesidades |
| `nuevaNecesidad` | `/dashboard/crowdsourcing/necesidades/nueva` | Formulario crear |
| `necesidadDetail(id)` | `/dashboard/crowdsourcing/necesidades/{id}` | Detalle + propuestas |
| `editarNecesidad(id)` | `/dashboard/crowdsourcing/necesidades/{id}/editar` | Formulario editar |

### 4.4 Estados de Necesidad (NUEVO)

**Ubicación:** Al final del archivo, después de constantes de Crowdsourcing existentes

```typescript
// ========== Estado de Necesidad (int IDs from maestras) ==========

export const ESTADO_NECESIDAD = {
  ABIERTA: 1,
  EN_PROGRESO: 2,
  CERRADA: 3,
  CANCELADA: 4,
} as const;

export const ESTADO_NECESIDAD_LABELS: Record<number, string> = {
  1: 'Abierta',
  2: 'En Progreso',
  3: 'Cerrada',
  4: 'Cancelada',
};

export const ESTADO_NECESIDAD_BADGES: Record<number, string> = {
  1: 'green',    // Abierta - verde
  2: 'blue',     // En Progreso - azul
  3: 'gray',     // Cerrada - gris
  4: 'red',      // Cancelada - rojo
};
```

| Constante | Tipo | Valores |
|-----------|------|---------|
| `ESTADO_NECESIDAD` | Object (readonly) | ABIERTA: 1, EN_PROGRESO: 2, CERRADA: 3, CANCELADA: 4 |
| `ESTADO_NECESIDAD_LABELS` | Record<number, string> | Mapeo ID → Label en español |
| `ESTADO_NECESIDAD_BADGES` | Record<number, string> | Mapeo ID → Color del badge |

### 4.5 Modalidad de Trabajo (NUEVO)

```typescript
// ========== Modalidad de Trabajo (int IDs from maestras) ==========

export const MODALIDAD_TRABAJO = {
  PRESENCIAL: 1,
  REMOTO: 2,
  HIBRIDO: 3,
} as const;

export const MODALIDAD_TRABAJO_LABELS: Record<number, string> = {
  1: 'Presencial',
  2: 'Remoto',
  3: 'Híbrido',
};

export const MODALIDAD_TRABAJO_ICONS: Record<number, string> = {
  1: '📍',  // Presencial
  2: '💻',  // Remoto
  3: '🔄',  // Híbrido
};
```

| Constante | Tipo | Valores |
|-----------|------|---------|
| `MODALIDAD_TRABAJO` | Object (readonly) | PRESENCIAL: 1, REMOTO: 2, HIBRIDO: 3 |
| `MODALIDAD_TRABAJO_LABELS` | Record<number, string> | Mapeo ID → Label en español |
| `MODALIDAD_TRABAJO_ICONS` | Record<number, string> | Mapeo ID → Emoji/icon |

### 4.6 Monedas Crowdsourcing (NUEVO)

**Nota:** Ya existe `MONEDAS` en constants, pero agregamos alias específico y símbolos para necesidades

```typescript
// ========== Monedas para Necesidades ==========

export const MONEDA = {
  EUR: 1,
  USD: 2,
  GBP: 3,
} as const;

export const MONEDA_SIMBOLOS: Record<number, string> = {
  1: '€',
  2: '$',
  3: '£',
};
```

| Constante | Tipo | Valores |
|-----------|------|---------|
| `MONEDA` | Object (readonly) | EUR: 1, USD: 2, GBP: 3 |
| `MONEDA_SIMBOLOS` | Record<number, string> | Mapeo ID → Símbolo de moneda |

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Formatters (agregar a `src/shared/utils/format.ts`)

**Acción:** MODIFICAR archivo existente - agregar al final

**Función: `formatPresupuesto`**

```typescript
/**
 * Formatea el rango de presupuesto con moneda
 * @param min - Presupuesto mínimo (opcional)
 * @param max - Presupuesto máximo (opcional)
 * @param monedaId - ID de moneda (1=EUR, 2=USD, 3=GBP)
 * @returns String formateado: "€150 - €800", "Desde €150", "Hasta €800", "No especificado"
 */
export function formatPresupuesto(
  min?: number,
  max?: number,
  monedaId: number = 1
): string {
  if (!min && !max) return 'No especificado';

  const simbolo = MONEDA_SIMBOLOS[monedaId] ?? '€';

  if (min && max) {
    return `${simbolo}${min} - ${simbolo}${max}`;
  }
  if (min) {
    return `Desde ${simbolo}${min}`;
  }
  if (max) {
    return `Hasta ${simbolo}${max}`;
  }

  return 'No especificado';
}
```

**Nota:** Requiere importar `MONEDA_SIMBOLOS` desde constants:
```typescript
import { MONEDA_SIMBOLOS } from '../constants';
```

| Función | Input | Output | Descripción |
|---------|-------|--------|-------------|
| `formatPresupuesto(min?, max?, monedaId?)` | `min?: number`, `max?: number`, `monedaId: number = 1` | `string` | Formatea rango presupuesto con símbolo moneda |

**Casos de uso:**
- `formatPresupuesto(150, 800, 1)` → `"€150 - €800"`
- `formatPresupuesto(150, undefined, 1)` → `"Desde €150"`
- `formatPresupuesto(undefined, 800, 2)` → `"Hasta $800"`
- `formatPresupuesto(undefined, undefined)` → `"No especificado"`

### 5.2 Error Messages (agregar a `src/shared/utils/error-messages.ts`)

**Acción:** MODIFICAR archivo existente - agregar en sección `CROWDSOURCING_ERROR_MESSAGES`

**Ubicación:** Después de los error messages de templates, agregar:

```typescript
// ========== Crowdsourcing - Necesidades Error Messages ==========

export const NECESIDAD_ERROR_MESSAGES: Record<string, string> = {
  // Validation errors específicos de necesidades (1000-1999)
  '1011': 'El título debe tener al menos 5 caracteres',
  '1012': 'La fecha no es válida',

  // NotFound errors (2000-2999)
  '2009': 'La necesidad no fue encontrada',

  // Business Rule errors (4000-4999)
  '4001': 'Solo se pueden editar necesidades en estado Abierta',
  '4002': 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',

  // Friendly context messages (semantic keys)
  NECESIDAD_NOT_FOUND: 'La necesidad seleccionada no existe',
  NECESIDAD_NOT_EDITABLE: 'Solo se pueden editar necesidades en estado Abierta',
  NECESIDAD_NOT_CLOSEABLE: 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',
  PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artístico no fue encontrado',
  PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para crear necesidades en este proyecto',
  VALIDATION_TITULO_MIN_LENGTH: 'El título debe tener al menos 5 caracteres',
  VALIDATION_FECHA_INVALIDA: 'La fecha ingresada no es válida',
  VALIDATION_UBICACION_REQUERIDA: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
  VALIDATION_PRESUPUESTO_MONEDA_REQUERIDA: 'Debe especificar la moneda cuando indica presupuesto',
  VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto máximo debe ser mayor o igual al mínimo',
} as const;

/**
 * Obtiene mensaje de error para necesidades crowdsourcing
 */
export const getNecesidadErrorMessage = (errorCode: string): string => {
  return NECESIDAD_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

**Agregar también al objeto general `ERROR_MESSAGES`:**

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... existing messages

  // Crowdsourcing - Necesidades errors (agregar a partir de línea ~250)
  '1011': 'El título debe tener al menos 5 caracteres',
  '1012': 'La fecha no es válida',
  '2009': 'La necesidad no fue encontrada',
  '4001': 'Solo se pueden editar necesidades en estado Abierta',
  '4002': 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',

  // Friendly messages
  NECESIDAD_NOT_FOUND: 'La necesidad seleccionada no existe',
  NECESIDAD_NOT_EDITABLE: 'Solo se pueden editar necesidades en estado Abierta',
  NECESIDAD_NOT_CLOSEABLE: 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',
  PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artístico no fue encontrado',
  PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para crear necesidades en este proyecto',
  VALIDATION_TITULO_MIN_LENGTH: 'El título debe tener al menos 5 caracteres',
  VALIDATION_FECHA_INVALIDA: 'La fecha ingresada no es válida',
  VALIDATION_UBICACION_REQUERIDA: 'La ubicación es obligatoria para modalidad Presencial o Híbrida',
  VALIDATION_PRESUPUESTO_MONEDA_REQUERIDA: 'Debe especificar la moneda cuando indica presupuesto',
  VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto máximo debe ser mayor o igual al mínimo',
} as const;
```

| Código | Mensaje | Contexto |
|--------|---------|----------|
| `1011` | El título debe tener al menos 5 caracteres | Validación de título en create/update |
| `1012` | La fecha no es válida | Validación de fechas (límite propuestas, inicio) |
| `2009` | La necesidad no fue encontrada | GET/PUT/PATCH cuando ID no existe |
| `4001` | Solo se pueden editar necesidades en estado Abierta | Intentar editar necesidad cerrada/cancelada |
| `4002` | Solo se pueden cerrar necesidades en estado Abierta o En Progreso | Intentar cerrar necesidad ya cerrada |
| `PROYECTO_ARTISTICO_NOT_FOUND` | El proyecto artístico no fue encontrado | ProyectoArtisticoId inválido en create |
| `PROYECTO_NO_PERTENECE_ARTISTA` | No tienes permiso para crear necesidades en este proyecto | Intentar crear necesidad en proyecto ajeno |
| `VALIDATION_UBICACION_REQUERIDA` | La ubicación es obligatoria para modalidad Presencial o Híbrida | Modalidad 1 o 3 sin ubicación |
| `VALIDATION_PRESUPUESTO_MONEDA_REQUERIDA` | Debe especificar la moneda cuando indica presupuesto | Presupuesto sin monedaId |
| `VALIDATION_PRESUPUESTO_MAX_MENOR_MIN` | El presupuesto máximo debe ser mayor o igual al mínimo | PresupuestoMax < PresupuestoMin |

---

## 6. Archivos a Crear/Modificar

### 6.1 Archivos a MODIFICAR (4 archivos)

```
src/shared/
├── types/
│   └── crowdsourcing.ts                    # MODIFICAR - Agregar 9 interfaces + 2 unions
├── schemas/
│   └── crowdsourcing.schema.ts             # MODIFICAR - Agregar 3 schemas con refines
├── constants/
│   └── index.ts                            # MODIFICAR - Agregar QUERY_KEYS, API_ROUTES, APP_ROUTES, estados, modalidades, monedas
└── utils/
    ├── format.ts                           # MODIFICAR - Agregar formatPresupuesto()
    └── error-messages.ts                   # MODIFICAR - Agregar 12 error messages
```

### 6.2 Detalles de Modificaciones

**1. `src/shared/types/crowdsourcing.ts`**
- Línea de inserción: Después de tipo `PrioridadNecesidad` (última línea existente)
- Agregar:
  - 3 DTOs de Response para listado y detalle
  - 3 DTOs de Request para create/update/cerrar
  - 3 DTOs de Result para respuestas de mutations
  - 2 Union Types para estados y modalidad

**2. `src/shared/schemas/crowdsourcing.schema.ts`**
- Línea de inserción: Después de `GenerarNecesidadesFormData` (última línea existente)
- Agregar:
  - `createNecesidadSchema` con 5 refines
  - `updateNecesidadSchema` con 5 refines
  - `cerrarNecesidadSchema` simple
  - 3 types inferidos

**3. `src/shared/constants/index.ts`**
- **QUERY_KEYS:** Modificar objeto `QUERY_KEYS.crowdsourcing` (línea ~50)
  - Agregar `maestras.tiposNecesidad`, `maestras.modalidadesTrabajo`, `maestras.monedas`
  - Agregar objeto `necesidades` con keys `mis` y `byId`
- **API_ROUTES:** Modificar objeto `API_ROUTES.crowdsourcing` (línea ~118)
  - Agregar `maestras.tiposNecesidad`, `maestras.modalidadesTrabajo`, `maestras.monedas`
  - Agregar objeto `necesidades` con `base`, `mis`, `byId`, `cerrar`
- **APP_ROUTES:** Modificar objeto `APP_ROUTES.dashboard.crowdsourcing` (línea ~155)
  - Agregar `necesidades`, `nuevaNecesidad`, `necesidadDetail`, `editarNecesidad`
- **Constantes nuevas:** Al final del archivo (después de línea ~356)
  - Agregar `ESTADO_NECESIDAD` + labels + badges (12 líneas)
  - Agregar `MODALIDAD_TRABAJO` + labels + icons (10 líneas)
  - Agregar `MONEDA` + símbolos (7 líneas)

**4. `src/shared/utils/format.ts`**
- Línea de inserción: Después de función `formatDecimal` (última línea existente, ~196)
- Agregar:
  - Import de `MONEDA_SIMBOLOS` en línea 1
  - Función `formatPresupuesto` con JSDoc (15 líneas)

**5. `src/shared/utils/error-messages.ts`**
- Línea de inserción: Después de sección de Dashboard errors (línea ~287)
- Agregar:
  - Objeto `NECESIDAD_ERROR_MESSAGES` (30 líneas)
  - Función `getNecesidadErrorMessage` (5 líneas)
  - Extender objeto `ERROR_MESSAGES` con keys de necesidades (integrar en línea ~64)

---

## 7. Dependencias

### 7.1 Packages Externos (Ya instalados)

- `zod` - Para schemas de validación (ya existe en shared)
- Ningún package adicional requerido

### 7.2 Tipos Reutilizados de Shared

| Tipo Existente | Origen | Uso en Necesidades |
|----------------|--------|-------------------|
| `ServiceResponse<T>` | `src/shared/types/api.ts` | Wrapper de respuestas API |
| `PaginatedResponse<T>` | `src/shared/types/api.ts` | Respuesta de listado paginado mis-necesidades |
| `PaginatedRequest` | `src/shared/types/api.ts` | Query params paginación |

**Nota:** Los DTOs de necesidades NO reutilizan tipos de templates (son entidades separadas).

### 7.3 Schemas Reutilizados

| Schema Existente | Origen | Reutilizado en |
|------------------|--------|----------------|
| `necesidadSeleccionadaSchema` | `src/shared/schemas/crowdsourcing.schema.ts` | NO (templates vs necesidades son distintos) |

**Importante:** Los schemas de necesidades (`createNecesidadSchema`, `updateNecesidadSchema`) son NUEVOS y no reutilizan schemas de templates. Aunque las plantillas generan necesidades, el schema de validación del formulario de creación es independiente.

---

## 8. Notas de Implementación

### 8.1 Alineación con Backend

**Importante:** Los tipos TypeScript deben mapear exactamente con los DTOs del backend:

| Backend DTO (C#) | Frontend Type (TS) |
|------------------|-------------------|
| `NecesidadCrowdsourcingListDto` | `NecesidadCrowdsourcingList` |
| `NecesidadCrowdsourcingDto` | `NecesidadCrowdsourcing` |
| `PropuestaCrowdsourcingDto` | `PropuestaCrowdsourcing` |
| `NecesidadCrowdsourcingCreateResultDto` | `NecesidadCreateResult` |
| `NecesidadCrowdsourcingUpdateResultDto` | `NecesidadUpdateResult` |
| `CerrarNecesidadResultDto` | `CerrarNecesidadResult` |

**Naming conventions:**
- Backend C#: `PascalCase` con sufijo `Dto`
- Frontend TS: `PascalCase` sin sufijo (más conciso)
- Propiedades: Backend usa `PascalCase`, frontend usa `camelCase` (mapeo automático)

### 8.2 Validaciones Compartidas Backend/Frontend

**Critical:** Los mensajes de error deben coincidir entre Zod (frontend) y FluentValidation (backend):

| Regla | Backend Message | Frontend Message | ErrorCode Backend |
|-------|----------------|------------------|-------------------|
| Título requerido | "El título es obligatorio" | "El título es obligatorio" | `Validation_Required` |
| Título min 5 | "El título debe tener al menos 5 caracteres" | "El título debe tener al menos 5 caracteres" | `Validation_MinLength` |
| Título max 200 | "El título no puede superar los 200 caracteres" | "El título no puede superar los 200 caracteres" | `Validation_MaxLength` |
| Presupuesto max >= min | "El presupuesto máximo debe ser mayor o igual al mínimo" | "El presupuesto máximo debe ser mayor o igual al mínimo" | `Validation_InvalidRange` |
| Moneda requerida si presupuesto | "La moneda es obligatoria cuando se especifica presupuesto" | "La moneda es obligatoria cuando se especifica presupuesto" | `Validation_Required` |
| Ubicación requerida presencial | "La ubicación es obligatoria para modalidad Presencial o Híbrida" | "La ubicación es obligatoria para modalidad Presencial o Híbrida" | `Validation_Required` |
| Fecha límite futura | "La fecha límite debe ser posterior a hoy" | "La fecha límite debe ser posterior a hoy" | `Validation_InvalidDate` |
| Fecha inicio >= hoy | "La fecha de inicio debe ser igual o posterior a hoy" | "La fecha de inicio debe ser igual o posterior a hoy" | `Validation_InvalidDate` |

**Importante:** Los refines de Zod replicanexactamente las validaciones condicionales del backend.

### 8.3 Estados y Modalidades - IDs Numéricos

**Critical:** Usar IDs numéricos (no strings) para alinearse con maestras del backend:

```typescript
// CORRECTO - Numeric IDs
const ESTADO_NECESIDAD = {
  ABIERTA: 1,
  EN_PROGRESO: 2,
  CERRADA: 3,
  CANCELADA: 4,
};

// INCORRECTO - String values
const ESTADO_NECESIDAD = {
  ABIERTA: 'abierta',  // NO HACER
  EN_PROGRESO: 'en-progreso',  // NO HACER
};
```

**Beneficio:** Los DTOs del backend incluyen tanto el `estadoNecesidadId` (number) como el `estadoNecesidadNombre` (string), lo que permite renderizar en UI sin conversiones adicionales.

### 8.4 Formateo de Presupuesto

La función `formatPresupuesto` debe manejar todos los casos edge:

```typescript
// Casos válidos
formatPresupuesto(150, 800, 1)  // "€150 - €800"
formatPresupuesto(150, undefined, 1)  // "Desde €150"
formatPresupuesto(undefined, 800, 1)  // "Hasta €800"
formatPresupuesto(undefined, undefined, 1)  // "No especificado"

// Edge cases
formatPresupuesto(0, 0, 1)  // "€0 - €0" (presupuesto 0 es válido)
formatPresupuesto(100, 100, 1)  // "€100 - €100" (min = max es válido)
formatPresupuesto(150, 800, 99)  // "€150 - €800" (monedaId inválido fallback a EUR)
```

### 8.5 Refines de Zod - Orden Importa

Los `.refine()` se ejecutan en orden. Ordenar de más específico a más general:

1. **Refine presupuesto max >= min** (condicional, solo si ambos presentes)
2. **Refine moneda requerida si presupuesto** (condicional, si alguno presente)
3. **Refine ubicación requerida** (condicional, modalidad 1 o 3)
4. **Refine fecha límite futura** (condicional, si presente)
5. **Refine fecha inicio >= hoy** (condicional, si presente)

**Importante:** Usar `path: ['campo']` en cada refine para marcar el campo específico con error (UX mejorada).

### 8.6 Integration con Templates (US-CS-01)

Los types y schemas de necesidades deben ser compatibles con el flujo de templates:

**Flujo desde template:**
1. Artista selecciona plantilla y necesidades
2. Endpoint `POST /templates/{id}/generar` crea necesidades
3. Frontend navega a `/necesidades/{id}` para ver detalle

**Flujo manual:**
1. Artista navega a `/necesidades/nueva`
2. Formulario vacío con `createNecesidadSchema`
3. Submit a `POST /necesidades` (mismo endpoint que templates usan internamente)

**No hay diferencia en el contrato** - ambos flujos usan el mismo `CreateNecesidadRequest` y backend handle.

---

## 9. Checklist

### Pre-implementación
- [ ] Revisar `contracts.md` para confirmar DTOs y endpoints finales
- [ ] Verificar que backend ha definido `ServiceResponseMessageType` constants (1011, 1012, 2009, 4001, 4002)
- [ ] Confirmar que maestras de TipoNecesidad, ModalidadTrabajo, Moneda existen en BD
- [ ] Validar que `ProyectoArtistico` entity existe (dependencia de UserAccess o Crowdfunding)

### Durante implementación
- [ ] Types creados en `crowdsourcing.ts` con naming consistente (camelCase propiedades)
- [ ] Schemas Zod con mensajes en español y errorCodes alineados
- [ ] Refines de Zod con `path` específico para cada campo
- [ ] Constantes de QUERY_KEYS con `as const` para type safety
- [ ] API_ROUTES usando funciones para IDs dinámicos
- [ ] APP_ROUTES usando funciones para IDs dinámicos
- [ ] `formatPresupuesto` con manejo de edge cases (undefined, 0, fallback moneda)
- [ ] Error messages con códigos numéricos y semantic keys
- [ ] Exportar todos los types/schemas/constants desde archivos index

### Post-implementación
- [ ] Verificar imports funcionan: `import { NecesidadCrowdsourcing } from '@/shared'`
- [ ] Validar schemas Zod con datos de ejemplo (positivos y negativos)
- [ ] Probar `formatPresupuesto` con casos edge
- [ ] Verificar QUERY_KEYS en React Query DevTools (formato correcto)
- [ ] Confirmar API_ROUTES match con endpoints backend
- [ ] Testing: Types compile sin errores de TypeScript
- [ ] Testing: Schemas validan correctamente con `schema.parse()`
- [ ] Testing: Error messages resuelven correctamente con `getNecesidadErrorMessage()`

---

## 10. Siguiente Paso Sugerido

**Orden de implementación recomendado:**

1. **Shared Contracts (este plan)** - Implementar types, schemas, constants, utils
2. **Backend (paralelo)** - Commands/Queries, Validators, Services, Controller
3. **Admin Frontend (depende de 1 y 2)** - Páginas, componentes, hooks, services

**Comandos para ejecutar:**

```bash
# 1. Implementar shared contracts
cd src/shared
# Editar archivos según este plan
npm run type-check  # Validar TypeScript

# 2. Verificar no hay errores de import
cd ../admin
npm run type-check

# 3. Proceder con backend
cd ../../api
dotnet build

# 4. Proceder con frontend Admin
cd ../src/admin
npm run dev
```

**Dependencias bloqueantes:**
- Frontend NO puede avanzar sin este shared (types requeridos)
- Backend puede avanzar en paralelo (usa contracts.md como referencia)
- Testing E2E requiere backend + frontend completos

---

**Fin del Plan de Contratos Shared**
