# Plan de Contratos Shared: Templates y Guia para Artistas Noveles

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Basado en:** docs/user-stories/cs-templates-guia/contracts.md

---

## 1. Resumen

- **Total de types:** 10 interfaces + 1 type
- **Total de schemas:** 2 schemas Zod con refinements
- **Constantes definidas:** 7 grupos de constantes (QUERY_KEYS, API_ROUTES, APP_ROUTES, prioridades, modalidades, fases, error messages)
- **Utilidades planificadas:** 0 (no requiere mappers especificos)

Este plan define los contratos TypeScript compartidos entre Landing y Admin para la feature de templates de crowdsourcing. Es la fuente de verdad para ambos frontends.

---

## 2. Types (`src/shared/types/crowdsourcing.ts`)

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `PlantillaProyectoList` | id, nombre, descripcion?, icono?, orden, precioMinTotal, precioMaxTotal, moneda, cantidadNecesidades, fases[] | Resumen de plantilla para galeria (Paso 1) |
| `PlantillaProyecto` | id, nombre, descripcion?, icono?, orden, necesidades[], resumen | Detalle completo de plantilla (Paso 2) |
| `PlantillaProyectoNecesidad` | id, fase, titulo, descripcion?, rolProfesional, precioMinOrientativo?, precioMaxOrientativo?, moneda, prioridad, orden | Necesidad profesional dentro de una plantilla |
| `PlantillaResumen` | precioMinTotal, precioMaxTotal, moneda, cantidadNecesidadesAlta, cantidadNecesidadesMedia, cantidadNecesidadesBaja | Resumen financiero y de prioridades |
| `RolProfesional` | id, nombre, descripcion?, categoriaRolId, modalidadCobro? | Rol profesional basico (usado en necesidad) |
| `RolProfesionalConCategoria` | id, nombre, descripcion?, categoriaRol, modalidadCobro?, activo | Rol profesional con categoria anidada (maestras) |
| `CategoriaRol` | id, nombre, icono?, orden | Categoria de roles profesionales |
| `GenerarNecesidadesResult` | necesidadesCreadas, necesidadIds[], presupuestoTotalMin, presupuestoTotalMax, moneda | Resultado de generacion masiva |

### 2.2 DTOs de Request

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `GenerarNecesidadesRequest` | proyectoArtisticoId, necesidadesSeleccionadas[] | Payload para generar necesidades desde template |
| `NecesidadSeleccionada` | plantillaNecesidadId, presupuestoMin?, presupuestoMax?, monedaId | Item seleccionado con presupuesto personalizado |

### 2.3 Union Types

| Tipo | Valores | Uso |
|------|---------|-----|
| `PrioridadNecesidad` | "Alta" \| "Media" \| "Baja" | Prioridad de necesidad (aligned con backend string values) |

---

## 3. Schemas Zod (`src/shared/schemas/crowdsourcing.schema.ts`)

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas |
|--------|--------|--------|
| `necesidadSeleccionadaSchema` | plantillaNecesidadId (string), presupuestoMin? (number), presupuestoMax? (number), monedaId (number) | - plantillaNecesidadId: min(1)<br>- presupuestoMin: nonnegative, optional<br>- presupuestoMax: nonnegative, optional<br>- monedaId: positive<br>- **Refinement:** presupuestoMax >= presupuestoMin |
| `generarNecesidadesSchema` | proyectoArtisticoId (string), necesidadesSeleccionadas (array) | - proyectoArtisticoId: min(1)<br>- necesidadesSeleccionadas: array(necesidadSeleccionadaSchema).min(1) |

### 3.2 Types Inferidos

```typescript
export type NecesidadSeleccionadaFormData = z.infer<typeof necesidadSeleccionadaSchema>;
export type GenerarNecesidadesFormData = z.infer<typeof generarNecesidadesSchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Endpoints API (Additions)

| Constante | Valor | Uso |
|-----------|-------|-----|
| `API_ROUTES.crowdsourcing.templates.base` | `/api/crowdsourcing/templates` | Listar templates |
| `API_ROUTES.crowdsourcing.templates.byId(id)` | `/api/crowdsourcing/templates/${id}` | Detalle de template |
| `API_ROUTES.crowdsourcing.templates.generar(id)` | `/api/crowdsourcing/templates/${id}/generar` | Generar necesidades desde template |
| `API_ROUTES.crowdsourcing.maestras.rolesProfesionales` | `/api/crowdsourcing/maestras/roles-profesionales` | Catalogo de roles |
| `API_ROUTES.crowdsourcing.maestras.categoriasRol` | `/api/crowdsourcing/maestras/categorias-rol` | Categorias de roles |

### 4.2 Query Keys (React Query)

| Key | Patron | Uso |
|-----|--------|-----|
| `QUERY_KEYS.crowdsourcing.templates.all` | `['crowdsourcing', 'templates']` | Lista de templates activos |
| `QUERY_KEYS.crowdsourcing.templates.byId(id)` | `['crowdsourcing', 'templates', id]` | Detalle de template especifico |
| `QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales` | `['crowdsourcing', 'maestras', 'roles']` | Catalogo de roles |
| `QUERY_KEYS.crowdsourcing.maestras.categoriasRol` | `['crowdsourcing', 'maestras', 'categorias']` | Categorias de roles |

### 4.3 App Routes (Frontend Navigation)

| Key | Patron | Uso |
|-----|--------|-----|
| `APP_ROUTES.dashboard.crowdsourcing.templates` | `/dashboard/crowdsourcing/templates` | Lista de templates (admin) |
| `APP_ROUTES.dashboard.crowdsourcing.templateDetail(id)` | `/dashboard/crowdsourcing/templates/${id}` | Detalle template (admin) |
| `APP_ROUTES.dashboard.crowdsourcing.wizard(projectId)` | `/dashboard/crowdsourcing/wizard/${projectId}` | Wizard de generacion |
| `APP_ROUTES.landing.crowdsourcing.templates` | `/crowdsourcing/nuevo-proyecto` | Galeria templates (landing) |
| `APP_ROUTES.landing.crowdsourcing.wizard` | `/crowdsourcing/nuevo-proyecto?step=` | Wizard steps (landing) |

### 4.4 Domain Constants - Prioridad Necesidad

| Constante | Tipo | Valores |
|-----------|------|---------|
| `PRIORIDAD_NECESIDAD` | const object | `{ ALTA: 'Alta', MEDIA: 'Media', BAJA: 'Baja' }` |
| `PRIORIDAD_NECESIDAD_LABELS` | Record<string, string> | `{ Alta: 'Alta prioridad', Media: 'Prioridad media', Baja: 'Prioridad baja' }` |
| `PRIORIDAD_NECESIDAD_COLORS` | Record<string, string> | `{ Alta: 'red', Media: 'yellow', Baja: 'blue' }` |

### 4.5 Domain Constants - Modalidad Cobro

| Constante | Tipo | Valores |
|-----------|------|---------|
| `MODALIDAD_COBRO` | const object | `{ POR_PROYECTO: 'Por proyecto', POR_DIA: 'Por día', POR_HORA: 'Por hora', POR_CANCION: 'Por canción', POR_MES: 'Por mes', POR_SESION: 'Por sesión', POR_VIDEO: 'Por video' }` |

### 4.6 Domain Constants - Fases Proyecto

| Constante | Tipo | Valores |
|-----------|------|---------|
| `FASES_PROYECTO` | const array | `['Preproducción', 'Grabación', 'Mezcla y Master', 'Diseño y Producción', 'Promoción', 'Distribución']` |

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

### 5.1 Error Messages (Additions)

| Codigo | Mensaje | Contexto |
|--------|---------|----------|
| `TEMPLATE_NOT_FOUND` | "La plantilla seleccionada no existe" | GET /templates/{id} 404 |
| `PROYECTO_ARTISTICO_NOT_FOUND` | "El proyecto artistico no fue encontrado" | POST /generar 404 |
| `PLANTILLA_NECESIDAD_NOT_FOUND` | "Una o mas necesidades de la plantilla no fueron encontradas" | POST /generar 404 |
| `PROYECTO_NO_PERTENECE_ARTISTA` | "No tienes permiso para modificar este proyecto" | POST /generar 403 |
| `VALIDATION_PRESUPUESTO_MIN_NEGATIVO` | "El presupuesto minimo no puede ser negativo" | Validacion frontend/backend |
| `VALIDATION_PRESUPUESTO_MAX_MENOR_MIN` | "El presupuesto maximo debe ser mayor o igual al minimo" | Validacion refine Zod |
| `VALIDATION_NECESIDADES_REQUERIDAS` | "Debe seleccionar al menos una necesidad" | Validacion array min(1) |
| `VALIDATION_PROYECTO_REQUERIDO` | "Debe seleccionar un proyecto artistico" | Validacion POST /generar |

---

## 6. Archivos a Crear

```
src/shared/
├── types/
│   └── crowdsourcing.ts                 (NUEVO)
├── schemas/
│   └── crowdsourcing.schema.ts          (NUEVO)
├── constants/
│   └── index.ts                         (MODIFICAR - agregar constantes crowdsourcing)
└── utils/
    └── error-messages.ts                (MODIFICAR - agregar error messages crowdsourcing)
```

### 6.1 Modificaciones a Archivos Existentes

**`src/shared/types/index.ts`** - Agregar:
```typescript
export * from "./crowdsourcing"
```

**`src/shared/schemas/index.ts`** - Agregar:
```typescript
export * from "./crowdsourcing.schema"
```

---

## 7. Codigo Detallado de Implementacion

### 7.1 `src/shared/types/crowdsourcing.ts`

```typescript
/**
 * Tipos TypeScript para feature cs-templates-guia (Crowdsourcing Templates)
 * Alineados con contracts.md del backend
 */

// ========== DTOs de Response ==========

/**
 * Resumen de plantilla para galeria (Paso 1 del wizard)
 */
export interface PlantillaProyectoList {
    id: string;
    nombre: string;
    descripcion?: string;
    icono?: string;
    orden: number;
    precioMinTotal: number;
    precioMaxTotal: number;
    moneda: number;
    cantidadNecesidades: number;
    fases: string[];
}

/**
 * Detalle completo de plantilla con necesidades (Paso 2 del wizard)
 */
export interface PlantillaProyecto {
    id: string;
    nombre: string;
    descripcion?: string;
    icono?: string;
    orden: number;
    necesidades: PlantillaProyectoNecesidad[];
    resumen: PlantillaResumen;
}

/**
 * Necesidad profesional dentro de una plantilla
 */
export interface PlantillaProyectoNecesidad {
    id: string;
    fase: string;
    titulo: string;
    descripcion?: string;
    rolProfesional: RolProfesional;
    precioMinOrientativo?: number;
    precioMaxOrientativo?: number;
    moneda: number;
    prioridad: PrioridadNecesidad;
    orden: number;
}

/**
 * Resumen financiero y de prioridades de una plantilla
 */
export interface PlantillaResumen {
    precioMinTotal: number;
    precioMaxTotal: number;
    moneda: number;
    cantidadNecesidadesAlta: number;
    cantidadNecesidadesMedia: number;
    cantidadNecesidadesBaja: number;
}

/**
 * Rol profesional basico (usado dentro de necesidad)
 */
export interface RolProfesional {
    id: number;
    nombre: string;
    descripcion?: string;
    categoriaRolId: number;
    modalidadCobro?: string;
}

/**
 * Rol profesional con categoria anidada (maestras endpoint)
 */
export interface RolProfesionalConCategoria {
    id: number;
    nombre: string;
    descripcion?: string;
    categoriaRol: CategoriaRol;
    modalidadCobro?: string;
    activo: boolean;
}

/**
 * Categoria de roles profesionales
 */
export interface CategoriaRol {
    id: number;
    nombre: string;
    icono?: string;
    orden: number;
}

/**
 * Resultado de generacion masiva de necesidades desde template
 */
export interface GenerarNecesidadesResult {
    necesidadesCreadas: number;
    necesidadIds: string[];
    presupuestoTotalMin: number;
    presupuestoTotalMax: number;
    moneda: number;
}

// ========== DTOs de Request ==========

/**
 * Payload para generar necesidades desde template
 */
export interface GenerarNecesidadesRequest {
    proyectoArtisticoId: string;
    necesidadesSeleccionadas: NecesidadSeleccionada[];
}

/**
 * Item seleccionado con presupuesto personalizado
 */
export interface NecesidadSeleccionada {
    plantillaNecesidadId: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId: number;
}

// ========== Union Types ==========

/**
 * Prioridad de necesidad (aligned con backend string values)
 */
export type PrioridadNecesidad = "Alta" | "Media" | "Baja";
```

### 7.2 `src/shared/schemas/crowdsourcing.schema.ts`

```typescript
import { z } from "zod";

/**
 * Schemas Zod para validacion de feature cs-templates-guia
 * Reglas alineadas con FluentValidation del backend
 */

// ========== Schema para Item de Necesidad Seleccionada ==========

/**
 * Valida un item de necesidad seleccionada con presupuesto personalizado
 *
 * Reglas:
 * - plantillaNecesidadId: obligatorio (GUID)
 * - presupuestoMin: opcional, >= 0
 * - presupuestoMax: opcional, >= presupuestoMin
 * - monedaId: obligatorio, > 0
 */
export const necesidadSeleccionadaSchema = z
    .object({
        plantillaNecesidadId: z
            .string()
            .min(1, "La necesidad es obligatoria"),
        presupuestoMin: z
            .number()
            .nonnegative("El presupuesto minimo no puede ser negativo")
            .optional(),
        presupuestoMax: z
            .number()
            .nonnegative("El presupuesto maximo no puede ser negativo")
            .optional(),
        monedaId: z
            .number()
            .positive("La moneda es obligatoria"),
    })
    .refine(
        (data) => {
            // Solo validar si ambos estan presentes
            if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
                return data.presupuestoMax >= data.presupuestoMin;
            }
            return true;
        },
        {
            message: "El presupuesto maximo debe ser mayor o igual al minimo",
            path: ["presupuestoMax"],
        }
    );

// ========== Schema para Request de Generar Necesidades ==========

/**
 * Valida el request completo de generacion de necesidades
 *
 * Reglas:
 * - proyectoArtisticoId: obligatorio (GUID)
 * - necesidadesSeleccionadas: array minimo 1 item, cada uno validado con necesidadSeleccionadaSchema
 */
export const generarNecesidadesSchema = z.object({
    proyectoArtisticoId: z
        .string()
        .min(1, "El proyecto artistico es obligatorio"),
    necesidadesSeleccionadas: z
        .array(necesidadSeleccionadaSchema)
        .min(1, "Debe seleccionar al menos una necesidad"),
});

// ========== Types Inferidos ==========

export type NecesidadSeleccionadaFormData = z.infer<typeof necesidadSeleccionadaSchema>;
export type GenerarNecesidadesFormData = z.infer<typeof generarNecesidadesSchema>;
```

### 7.3 Additions to `src/shared/constants/index.ts`

```typescript
// ========== ADDITIONS FOR CROWDSOURCING TEMPLATES ==========

// Query Keys additions
export const QUERY_KEYS = {
    // ... existing keys

    // Crowdsourcing Templates
    crowdsourcing: {
        templates: {
            all: ['crowdsourcing', 'templates'] as const,
            byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
        },
        maestras: {
            rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
            categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
        },
    },
};

// API Routes additions
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
        },
    },
};

// App Routes additions
export const APP_ROUTES = {
    // ... existing routes

    dashboard: {
        // ... existing dashboard routes
        crowdsourcing: {
            templates: '/dashboard/crowdsourcing/templates',
            templateDetail: (id: string) => `/dashboard/crowdsourcing/templates/${id}`,
            wizard: (projectId: string) => `/dashboard/crowdsourcing/wizard/${projectId}`,
        },
    },

    landing: {
        // ... existing landing routes
        crowdsourcing: {
            templates: '/crowdsourcing/nuevo-proyecto',
            wizard: '/crowdsourcing/nuevo-proyecto', // with ?step=1|2|3
        },
    },
};

// ========== NEW CONSTANTS FOR CROWDSOURCING ==========

/**
 * Prioridad de Necesidad (aligned with backend string values)
 */
export const PRIORIDAD_NECESIDAD = {
    ALTA: 'Alta',
    MEDIA: 'Media',
    BAJA: 'Baja',
} as const;

/**
 * Labels descriptivos para prioridades
 */
export const PRIORIDAD_NECESIDAD_LABELS: Record<string, string> = {
    Alta: 'Alta prioridad',
    Media: 'Prioridad media',
    Baja: 'Prioridad baja',
};

/**
 * Colores para badges de prioridad
 */
export const PRIORIDAD_NECESIDAD_COLORS: Record<string, string> = {
    Alta: 'red',
    Media: 'yellow',
    Baja: 'blue',
};

/**
 * Modalidades de Cobro (catalogo de valores posibles)
 */
export const MODALIDAD_COBRO = {
    POR_PROYECTO: 'Por proyecto',
    POR_DIA: 'Por día',
    POR_HORA: 'Por hora',
    POR_CANCION: 'Por canción',
    POR_MES: 'Por mes',
    POR_SESION: 'Por sesión',
    POR_VIDEO: 'Por video',
} as const;

/**
 * Fases comunes de proyectos musicales
 */
export const FASES_PROYECTO = [
    'Preproducción',
    'Grabación',
    'Mezcla y Master',
    'Diseño y Producción',
    'Promoción',
    'Distribución',
] as const;
```

### 7.4 Additions to `src/shared/utils/error-messages.ts`

```typescript
/**
 * Error messages para feature cs-templates-guia
 */
export const ERROR_MESSAGES: Record<string, string> = {
    // ... existing error messages

    // ========== CROWDSOURCING TEMPLATE ERRORS ==========

    // NotFound errors (2000-2999)
    TEMPLATE_NOT_FOUND: 'La plantilla seleccionada no existe',
    PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artistico no fue encontrado',
    PLANTILLA_NECESIDAD_NOT_FOUND: 'Una o mas necesidades de la plantilla no fueron encontradas',

    // Authorization errors (3000-3999)
    PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para modificar este proyecto',

    // Validation errors for crowdsourcing (1000-1999)
    VALIDATION_PRESUPUESTO_MIN_NEGATIVO: 'El presupuesto minimo no puede ser negativo',
    VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto maximo debe ser mayor o igual al minimo',
    VALIDATION_NECESIDADES_REQUERIDAS: 'Debe seleccionar al menos una necesidad',
    VALIDATION_PROYECTO_REQUERIDO: 'Debe seleccionar un proyecto artistico',
};
```

---

## 8. Dependencias

### 8.1 Packages Necesarios

- **zod** - Ya instalado (validacion de schemas)
- **@tanstack/react-query** - Ya instalado (query keys)

No se requieren packages adicionales.

### 8.2 Dependencias Internas

- `src/shared/types/api.ts` - Usa `ServiceResponse<T>` wrapper
- `src/shared/constants/index.ts` - Extiende constantes existentes

---

## 9. Notas de Implementacion

### 9.1 Alineacion con Backend

- **Nombres de campos:** Seguir camelCase en TypeScript (aligned con JSON response del backend)
- **Prioridad:** Backend usa strings ("Alta", "Media", "Baja"), NO enums numericos
- **Moneda:** Backend usa int (1 = EUR), usar constante `MONEDAS.EUR` existente
- **GUIDs:** Backend usa `Guid`, TypeScript usa `string`

### 9.2 Validacion Zod

- **Refinement de presupuesto:** Solo valida si ambos valores estan presentes (evitar error en opcionales)
- **Mensajes en espanol:** Todos los mensajes de error en espanol para UX consistente
- **Tipos inferidos:** Exportar `FormData` types para React Hook Form

### 9.3 Query Keys

- **Patron jerarquico:** `['crowdsourcing', 'templates', id]` permite invalidacion granular
- **Maestras cacheables:** `rolesProfesionales` y `categoriasRol` pueden tener `staleTime` largo (datos maestros cambian poco)

### 9.4 Error Messages

- **Codigo vs Mensaje:** El `errorCode` del backend (ej: "2006") mapea a mensaje descriptivo en frontend
- **Centralizados:** Un solo lugar para todos los mensajes, facilita i18n futuro

---

## 10. Uso en Frontend

### 10.1 Landing (web) - Wizard de Templates

```typescript
// hooks/useTemplates.ts
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS, API_ROUTES } from '@/shared/constants';
import type { PlantillaProyectoList } from '@/shared/types';

export const useTemplates = () => {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.all,
        queryFn: async () => {
            const res = await fetch(API_ROUTES.crowdsourcing.templates.base);
            const data = await res.json();
            return data.data as PlantillaProyectoList[];
        },
        staleTime: 5 * 60 * 1000, // 5 min - datos maestros cambian poco
    });
};

// hooks/useGenerarNecesidades.ts
import { useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { generarNecesidadesSchema, type GenerarNecesidadesFormData } from '@/shared/schemas';

export const useGenerarNecesidades = (templateId: string) => {
    const form = useForm<GenerarNecesidadesFormData>({
        resolver: zodResolver(generarNecesidadesSchema),
    });

    const mutation = useMutation({
        mutationFn: async (data: GenerarNecesidadesFormData) => {
            const res = await fetch(API_ROUTES.crowdsourcing.templates.generar(templateId), {
                method: 'POST',
                body: JSON.stringify(data),
            });
            return res.json();
        },
    });

    return { form, mutation };
};
```

### 10.2 Admin - Gestion de Templates (Futuro)

```typescript
// Para CRUD de templates en admin (no MVP)
import { PRIORIDAD_NECESIDAD, PRIORIDAD_NECESIDAD_COLORS } from '@/shared/constants';
import type { PlantillaProyecto } from '@/shared/types';

// Renderizar badge de prioridad
const PriorityBadge = ({ prioridad }: { prioridad: PrioridadNecesidad }) => (
    <Badge variant={PRIORIDAD_NECESIDAD_COLORS[prioridad]}>
        {prioridad}
    </Badge>
);
```

---

## 11. Testing

### 11.1 Validacion de Schemas

```typescript
// tests/schemas/crowdsourcing.schema.test.ts
import { describe, it, expect } from 'vitest';
import { necesidadSeleccionadaSchema, generarNecesidadesSchema } from '@/shared/schemas';

describe('necesidadSeleccionadaSchema', () => {
    it('acepta valores validos', () => {
        const valid = {
            plantillaNecesidadId: '123e4567-e89b-12d3-a456-426614174000',
            presupuestoMin: 100,
            presupuestoMax: 500,
            monedaId: 1,
        };
        expect(() => necesidadSeleccionadaSchema.parse(valid)).not.toThrow();
    });

    it('rechaza presupuesto max < min', () => {
        const invalid = {
            plantillaNecesidadId: '123e4567-e89b-12d3-a456-426614174000',
            presupuestoMin: 500,
            presupuestoMax: 100,
            monedaId: 1,
        };
        expect(() => necesidadSeleccionadaSchema.parse(invalid)).toThrow();
    });

    it('rechaza presupuesto negativo', () => {
        const invalid = {
            plantillaNecesidadId: '123e4567-e89b-12d3-a456-426614174000',
            presupuestoMin: -100,
            monedaId: 1,
        };
        expect(() => necesidadSeleccionadaSchema.parse(invalid)).toThrow();
    });
});
```

---

## 12. Checklist de Implementacion

### Archivos Nuevos
- [ ] `src/shared/types/crowdsourcing.ts` creado con 11 interfaces/types
- [ ] `src/shared/schemas/crowdsourcing.schema.ts` creado con 2 schemas Zod

### Archivos Modificados
- [ ] `src/shared/types/index.ts` - export agregado
- [ ] `src/shared/schemas/index.ts` - export agregado
- [ ] `src/shared/constants/index.ts` - 7 grupos de constantes agregados
- [ ] `src/shared/utils/error-messages.ts` - 8 mensajes agregados (crear archivo si no existe)

### Validacion
- [ ] Types alineados con contracts.md (DTOs C#)
- [ ] Schemas Zod con mensajes en espanol
- [ ] Constantes de endpoints alineadas con backend
- [ ] Query keys siguen patron jerarquico existente
- [ ] Prioridad usa strings ("Alta", "Media", "Baja") no numericos
- [ ] Moneda usa int (1 = EUR) como en rest del proyecto
- [ ] Error messages centralizados en utils
- [ ] Todos los exports agregados a index files

### Testing
- [ ] Unit tests para schemas Zod (casos validos/invalidos)
- [ ] Verificar tipos con TypeScript compiler (no type errors)

---

## 13. Siguiente Paso

**Este plan es BLOCKING para backend y frontend.**

1. **Inmediato:** Implementar este plan (crear/modificar archivos shared)
2. **Backend:** Puede implementar Commands/Queries usando DTOs C# de contracts.md en paralelo
3. **Frontend Landing:** Implementar wizard de 3 pasos usando estos types y schemas
4. **Frontend Admin:** Implementar CRUD de templates (post-MVP)

Una vez completado este plan, los equipos de backend (API) y frontend (Landing + Admin) tendran la fuente de verdad compartida para desarrollo paralelo sin merge conflicts.

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (Arquitecto TypeScript)
**Estado:** READY FOR IMPLEMENTATION
