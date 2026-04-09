# Plan de Contratos Shared: Dashboard Artista

**Fecha:** 2026-02-14
**Feature:** dashboard-artista (US-05)
**Basado en:** docs/user-stories/dashboard-artista/contracts.md

---

## 1. Resumen

- **Total de types nuevos:** 9 interfaces
- **Total de schemas nuevos:** 2 schemas Zod
- **Constantes a actualizar:** 3 archivos (query-keys, api-routes, error-messages)
- **Utilidades planificadas:** 2 funciones (calculos de metricas y mappers)

### Archivos a Crear

```
src/shared/
├── types/
│   └── dashboard.ts (NUEVO - 9 interfaces)
├── schemas/
│   └── dashboard.schema.ts (NUEVO - 2 schemas)
└── constants/
    └── (actualizaciones a archivos existentes)
```

### Archivos a Actualizar

```
src/shared/
├── types/index.ts (agregar export dashboard)
├── schemas/index.ts (agregar export dashboard)
├── constants/index.ts (actualizar QUERY_KEYS, API_ROUTES)
├── utils/error-messages.ts (agregar DASHBOARD_ERROR_MESSAGES)
└── utils/format.ts (agregar calculos de metricas)
```

---

## 2. Types (`src/shared/types/dashboard.ts`)

### 2.1 DTOs de Response

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `DashboardResumen` | artistaId, nombreArtistico, totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas, totalCampanias, monedaSimbolo, fechaUltimoAporte? | Metricas resumidas del dashboard del artista |
| `MiCampaniaListItem` | id, titulo, imagenPrincipalUrl?, estadoCampaniaId, estadoCampaniaNombre, importeObjetivo, importeRecaudado, porcentajeProgreso, numBackers, diasRestantes?, fechaFin?, fechaCreacion | Item de campania en lista de "Mis Campanias" con metricas calculadas |
| `CampaniaBackingList` | campaniaId, campaniaTitulo, stats, backings | Respuesta completa de backings con estadisticas agregadas y lista paginada |
| `CampaniaBackingStats` | totalRecaudado, backingPromedio, totalBackers, rewardMasPopular?, ultimoBacking? | Estadisticas agregadas de backings de una campania |
| `UltimoBacking` | nombreBacker, monto, fechaCreacion | Datos del ultimo backing recibido |
| `CampaniaBackingItem` | id, nombreBacker, email?, monto, rewardNombre?, mensaje?, esAnonimo, estadoPedido, fechaCreacion | Item individual de backing en lista paginada |
| `CampaniaStatsDetail` | campaniaId, campaniaTitulo, importeObjetivo, importeRecaudado, porcentajeProgreso, numBackers, backingPromedio, diasRestantes?, diasTranscurridos, totalDiasCampania, proyeccionFinal?, velocidadDiaria, rewardStats[], progressoPorDia[] | Estadisticas detalladas de campania con proyecciones y series temporales |
| `RewardStat` | rewardId?, rewardNombre, cantidadVendida, totalRecaudado, porcentajeDelTotal | Estadisticas de distribucion de recompensas |
| `ProgressoDia` | fecha (YYYY-MM-DD), numBackings, totalRecaudado, acumulado | Serie temporal de progreso diario de recaudacion |

### 2.2 Estructura del Archivo

```typescript
// src/shared/types/dashboard.ts

import { PaginatedResponse } from './api';

/**
 * Metricas resumidas del dashboard del artista
 * Endpoint: GET /api/dashboard/resumen
 */
export interface DashboardResumen {
  artistaId: string;
  nombreArtistico: string;
  totalRecaudado: number;
  totalBackers: number;
  campaniasActivas: number;       // EstadoCampaniaId = 2 (Publicada)
  campaniasCompletadas: number;   // EstadoCampaniaId = 3 (Finalizada)
  totalCampanias: number;
  monedaSimbolo: string;          // Default: "EUR"
  fechaUltimoAporte?: string;     // ISO 8601, undefined si no hay aportes
}

/**
 * Item de campania en lista de "Mis Campanias" con metricas
 * Endpoint: GET /api/campanias/mis-campanias
 */
export interface MiCampaniaListItem {
  id: string;
  titulo: string;
  imagenPrincipalUrl?: string;
  estadoCampaniaId: number;       // 1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada, 5=Pausada
  estadoCampaniaNombre: string;
  importeObjetivo: number;
  importeRecaudado: number;
  porcentajeProgreso: number;     // (importeRecaudado / importeObjetivo) * 100
  numBackers: number;             // COUNT de PedidoCrowdfunding con EstadoPedidoId = 3
  diasRestantes?: number;         // undefined para borradores o campanias sin fechaFin
  fechaFin?: string;              // ISO 8601
  fechaCreacion: string;          // ISO 8601
}

/**
 * Respuesta completa de backings con stats y lista paginada
 * Endpoint: GET /api/campanias/{id}/backings
 */
export interface CampaniaBackingList {
  campaniaId: string;
  campaniaTitulo: string;
  stats: CampaniaBackingStats;
  backings: PaginatedResponse<CampaniaBackingItem>;
}

/**
 * Estadisticas agregadas de backings
 */
export interface CampaniaBackingStats {
  totalRecaudado: number;
  backingPromedio: number;
  totalBackers: number;
  rewardMasPopular?: string;      // Nombre del reward mas vendido
  ultimoBacking?: UltimoBacking;
}

/**
 * Datos del ultimo backing recibido
 */
export interface UltimoBacking {
  nombreBacker: string;           // "Anonimo" o nombre real
  monto: number;
  fechaCreacion: string;          // ISO 8601
}

/**
 * Item individual de backing
 */
export interface CampaniaBackingItem {
  id: string;
  nombreBacker: string;           // "Anonimo" o nombre del usuario
  email?: string;                 // undefined si anonimo o sin permiso mostrar
  monto: number;
  rewardNombre?: string;          // undefined si backing sin recompensa
  mensaje?: string;
  esAnonimo: boolean;
  estadoPedido: string;           // "Completado", "Pendiente", etc.
  fechaCreacion: string;          // ISO 8601
}

/**
 * Estadisticas detalladas de campania con proyecciones
 * Endpoint: GET /api/campanias/{id}/stats
 */
export interface CampaniaStatsDetail {
  campaniaId: string;
  campaniaTitulo: string;
  importeObjetivo: number;
  importeRecaudado: number;
  porcentajeProgreso: number;
  numBackers: number;
  backingPromedio: number;
  diasRestantes?: number;
  diasTranscurridos: number;      // Desde FechaInicio hasta hoy
  totalDiasCampania: number;      // FechaFin - FechaInicio
  proyeccionFinal?: number;       // Estimacion basada en velocidad diaria
  velocidadDiaria: number;        // Promedio de recaudacion diaria
  rewardStats: RewardStat[];
  progressoPorDia: ProgressoDia[];
}

/**
 * Estadisticas de distribucion de recompensas
 */
export interface RewardStat {
  rewardId?: string;              // undefined = "Sin recompensa"
  rewardNombre: string;
  cantidadVendida: number;
  totalRecaudado: number;
  porcentajeDelTotal: number;     // (totalRecaudado / importeRecaudadoCampania) * 100
}

/**
 * Serie temporal de progreso diario
 */
export interface ProgressoDia {
  fecha: string;                  // "YYYY-MM-DD" format
  numBackings: number;
  totalRecaudado: number;
  acumulado: number;
}
```

---

## 3. Schemas Zod (`src/shared/schemas/dashboard.schema.ts`)

### 3.1 Schemas de Validacion

| Schema | Campos | Reglas |
|--------|--------|--------|
| `misCampaniasQuerySchema` | estadoCampaniaId?, page, pageSize | estadoCampaniaId: optional int 1-5; page: int min 1 default 1; pageSize: int min 1 max 100 default 10 |
| `backingsQuerySchema` | page, pageSize | page: int min 1 default 1; pageSize: int min 1 max 100 default 20 |

### 3.2 Types Inferidos

- `MisCampaniasQueryParams` = `z.infer<typeof misCampaniasQuerySchema>`
- `BackingsQueryParams` = `z.infer<typeof backingsQuerySchema>`

### 3.3 Estructura del Archivo

```typescript
// src/shared/schemas/dashboard.schema.ts

import { z } from 'zod';

/**
 * Schema de validacion para query params de GET /api/campanias/mis-campanias
 */
export const misCampaniasQuerySchema = z.object({
  estadoCampaniaId: z
    .number()
    .int()
    .min(1, 'Estado invalido')
    .max(5, 'Estado invalido')
    .optional(),
  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor o igual a 1')
    .default(1),
  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
    .max(100, 'El tamano de pagina no puede superar 100')
    .default(10),
});

export type MisCampaniasQueryParams = z.infer<typeof misCampaniasQuerySchema>;

/**
 * Schema de validacion para query params de GET /api/campanias/{id}/backings
 */
export const backingsQuerySchema = z.object({
  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor o igual a 1')
    .default(1),
  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
    .max(100, 'El tamano de pagina no puede superar 100')
    .default(20),
});

export type BackingsQueryParams = z.infer<typeof backingsQuerySchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Actualizaciones en QUERY_KEYS

**Archivo:** `src/shared/constants/index.ts`

**Cambios:** Agregar nueva seccion `dashboard` dentro del objeto `QUERY_KEYS` existente.

```typescript
// AGREGAR a QUERY_KEYS existente (linea ~8-61)
export const QUERY_KEYS = {
    // ... existentes (auth, artistas, campanias, rewards, backings)

    // Dashboard (NUEVO)
    dashboard: {
        resumen: ['dashboard', 'resumen'] as const,
        misCampanias: (params?: MisCampaniasQueryParams) =>
            ['dashboard', 'mis-campanias', params] as const,
        campaniaBackings: (campaniaId: string, params?: BackingsQueryParams) =>
            ['dashboard', 'campanias', campaniaId, 'backings', params] as const,
        campaniaStats: (campaniaId: string) =>
            ['dashboard', 'campanias', campaniaId, 'stats'] as const,
    },

    // ... legacy keys existentes
} as const;
```

**Nota:** Importar `MisCampaniasQueryParams` y `BackingsQueryParams` desde `../schemas/dashboard.schema` al inicio del archivo.

### 4.2 Actualizaciones en API_ROUTES

**Archivo:** `src/shared/constants/index.ts`

**Cambios:** Agregar nueva seccion `dashboard` dentro del objeto `API_ROUTES` existente.

```typescript
// AGREGAR a API_ROUTES existente (linea ~64-93)
export const API_ROUTES = {
    // ... existentes (auth, artistas, campanias, rewards, backings)

    // Dashboard (NUEVO)
    dashboard: {
        resumen: '/api/dashboard/resumen',
        misCampanias: '/api/campanias/mis-campanias',
        campaniaBackings: (campaniaId: string) => `/api/campanias/${campaniaId}/backings`,
        campaniaStats: (campaniaId: string) => `/api/campanias/${campaniaId}/stats`,
    },

    // ... resto
} as const;
```

**Nota:** Los endpoints de campanias ya existen parcialmente (`misCampanias`, `stats`, `backings`), pero se agrupan bajo `dashboard` para mayor claridad contextual. Evaluar si mantener ambos o consolidar.

### 4.3 Actualizaciones en APP_ROUTES

**Archivo:** `src/shared/constants/index.ts`

**Cambios:** Actualizar rutas de dashboard para incluir ruta de backings de campania.

```typescript
// ACTUALIZAR APP_ROUTES.dashboard existente (linea ~104-118)
export const APP_ROUTES = {
    // ... auth, artista, landing existentes

    dashboard: {
        root: '/dashboard',
        campanias: {
            list: '/dashboard/campanias',
            nueva: '/dashboard/campanias/nueva',
            editar: (id: string) => `/dashboard/campanias/${id}/editar`,
            detalle: (id: string) => `/dashboard/campanias/${id}`,
            backings: (id: string) => `/dashboard/campanias/${id}/backings`, // AGREGAR
        },
        rewards: {
            // ... existente
        },
    },
} as const;
```

### 4.4 Estados de Campania y Pedido

**Status:** Ya existen en `constants/index.ts` (lineas 133-173).

**Constantes existentes:**
- `CAMPANIA_ESTADOS` (BORRADOR=1, PUBLICADA=2, FINALIZADA=3, CANCELADA=4, PAUSADA=5)
- `CAMPANIA_ESTADOS_LABELS`
- `CAMPANIA_ESTADO_COLORS`
- `ESTADO_PEDIDO` (PENDIENTE=1, PROCESANDO=2, COMPLETADO=3, FALLIDO=4, CANCELADO=5)
- `ESTADO_PEDIDO_LABELS`

**Accion:** No requiere cambios. Usar constantes existentes.

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Error Messages (`src/shared/utils/error-messages.ts`)

**Accion:** Agregar nueva funcion `getDashboardErrorMessage` y constante `DASHBOARD_ERROR_MESSAGES`.

**Ubicacion:** Al final del archivo, despues de `getBackingContextualError`.

```typescript
// ========== Dashboard Error Messages ==========

/**
 * Mensajes de error especificos para Dashboard de Artista
 * Alineado con contracts.md de dashboard-artista
 */
export const DASHBOARD_ERROR_MESSAGES: Record<string, string> = {
    // Success codes
    '0000': 'Operacion exitosa',

    // Validation errors (1000-1999)
    '1007': 'Valor fuera de rango',

    // Not Found errors (2000-2999)
    '2002': 'No tienes un perfil de artista. Por favor, crea tu perfil primero',
    '2003': 'Campania no encontrada',

    // Auth errors (3000-3999)
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
    '3002': 'No tienes permiso para acceder a este recurso',

    // Internal errors (5000-5999)
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
} as const;

/**
 * Obtiene mensaje de error para dashboard
 * Usa mensajes especificos de dashboard si existen, sino fallback a getErrorMessage
 */
export const getDashboardErrorMessage = (errorCode: string): string => {
    return DASHBOARD_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

### 5.2 Format Utilities (`src/shared/utils/format.ts`)

**Accion:** Agregar funciones de calculo de metricas para dashboard.

**Ubicacion:** Al final del archivo, despues de `getBackerDisplayName`.

```typescript
// ========== Dashboard Metrics Helpers ==========

/**
 * Calcula el porcentaje de progreso de una campania
 * Formula: (importeRecaudado / importeObjetivo) * 100
 * Retorna valor redondeado a 2 decimales, max 100%
 */
export function calculateCampaniaProgress(
  importeRecaudado: number,
  importeObjetivo: number
): number {
  if (importeObjetivo === 0) return 0;
  const percentage = (importeRecaudado / importeObjetivo) * 100;
  return Math.min(Math.round(percentage * 100) / 100, 100); // Round to 2 decimals
}

/**
 * Calcula los dias restantes de una campania
 * Formula: Max(0, (fechaFin - hoy).days)
 * Retorna undefined si fechaFin es null/undefined
 */
export function calculateDiasRestantes(fechaFin?: string): number | undefined {
  if (!fechaFin) return undefined;

  const end = new Date(fechaFin);
  const now = new Date();
  const diffMs = end.getTime() - now.getTime();
  const diffDays = Math.ceil(diffMs / (1000 * 60 * 60 * 24));

  return Math.max(0, diffDays);
}

/**
 * Calcula la proyeccion final de recaudacion basada en velocidad diaria
 * Formula: importeRecaudado + (velocidadDiaria * diasRestantes)
 */
export function calculateProyeccionFinal(
  importeRecaudado: number,
  velocidadDiaria: number,
  diasRestantes: number
): number {
  return importeRecaudado + (velocidadDiaria * diasRestantes);
}

/**
 * Calcula la velocidad diaria de recaudacion
 * Formula: importeRecaudado / diasTranscurridos
 * Retorna 0 si diasTranscurridos es 0 o negativo
 */
export function calculateVelocidadDiaria(
  importeRecaudado: number,
  diasTranscurridos: number
): number {
  if (diasTranscurridos <= 0) return 0;
  return importeRecaudado / diasTranscurridos;
}

/**
 * Calcula el promedio de backing
 * Formula: totalRecaudado / numBackers
 * Retorna 0 si numBackers es 0
 */
export function calculateBackingPromedio(
  totalRecaudado: number,
  numBackers: number
): number {
  if (numBackers === 0) return 0;
  return totalRecaudado / numBackers;
}

/**
 * Formatea un numero con 2 decimales
 * Util para mostrar promedios y porcentajes
 */
export function formatDecimal(value: number, decimals: number = 2): string {
  return value.toFixed(decimals);
}
```

### 5.3 Mappers (`src/shared/utils/mappers.ts`)

**Status:** Archivo ya existe. Revisar si requiere mappers especificos para dashboard.

**Accion sugerida:** No se requieren mappers especificos. Los tipos de dashboard son DTOs directos del backend sin transformaciones complejas. Si se necesitan, agregar en una iteracion futura.

---

## 6. Archivos Index (Barrel Exports)

### 6.1 `src/shared/types/index.ts`

**Accion:** Agregar export de dashboard.

```typescript
// Agregar despues de linea 9 (despues de backing)
export * from "./dashboard"
```

### 6.2 `src/shared/schemas/index.ts`

**Accion:** Agregar export de dashboard.schema.

```typescript
// Agregar despues de linea 5 (despues de backing.schema)
export * from "./dashboard.schema"
```

### 6.3 `src/shared/utils/index.ts`

**Status:** Verificar si existe. Si no existe, crearlo.

**Contenido esperado:**
```typescript
// src/shared/utils/index.ts
export * from "./cn"
export * from "./error-messages"
export * from "./format"
export * from "./mappers"
```

**Nota:** Si ya existe, no requiere cambios (ya exporta todos los archivos).

---

## 7. Dependencias entre Archivos

### 7.1 Imports Cruzados

| Archivo | Importa de |
|---------|------------|
| `types/dashboard.ts` | `types/api.ts` (PaginatedResponse) |
| `schemas/dashboard.schema.ts` | `zod` (peer dependency, ya instalado) |
| `constants/index.ts` | `schemas/dashboard.schema.ts` (MisCampaniasQueryParams, BackingsQueryParams) |
| `utils/format.ts` | Ninguno (standalone) |
| `utils/error-messages.ts` | Ninguno (usa getErrorMessage que ya existe en el mismo archivo) |

### 7.2 Orden de Creacion Recomendado

1. **Crear:** `types/dashboard.ts` (no tiene dependencias de nuevos archivos)
2. **Crear:** `schemas/dashboard.schema.ts` (depende de zod, ya instalado)
3. **Actualizar:** `types/index.ts` (agregar export dashboard)
4. **Actualizar:** `schemas/index.ts` (agregar export dashboard.schema)
5. **Actualizar:** `constants/index.ts` (agregar imports y actualizar QUERY_KEYS, API_ROUTES, APP_ROUTES)
6. **Actualizar:** `utils/error-messages.ts` (agregar DASHBOARD_ERROR_MESSAGES y getDashboardErrorMessage)
7. **Actualizar:** `utils/format.ts` (agregar funciones de calculo de metricas)
8. **Verificar:** `utils/index.ts` (asegurar que exporta todos los utils)

---

## 8. Dependencias de Paquetes

### 8.1 Ya Instaladas

- `zod` (para schemas de validacion)
- `typescript` (tipos)

### 8.2 Requeridas Nuevas

**Ninguna.** Todos los paquetes necesarios ya estan instalados en `src/shared/package.json`.

---

## 9. Notas de Implementacion

### 9.1 Alineacion con Backend

- **Tipos numericos:** Todos los IDs de estado (campania, pedido) son `number`, no strings.
- **Fechas:** Todas las fechas vienen como strings ISO 8601 del backend, convertir en cliente si es necesario.
- **Nullable vs Undefined:** Backend usa `null` para valores opcionales, frontend usa `undefined` (TypeScript convention). Mapear al deserializar si es necesario.
- **Porcentaje de progreso:** Backend calcula y envia el valor, frontend puede recalcular client-side con `calculateCampaniaProgress` para validacion.

### 9.2 Paginacion Defaults

- **Mis Campanias:** pageSize default = 10
- **Backings:** pageSize default = 20
- **Maximo:** pageSize max = 100 (validado en schema)

### 9.3 Calculo de Metricas en Cliente

**Formulas clave (alineadas con backend):**

```typescript
// Porcentaje de progreso
porcentajeProgreso = (importeRecaudado / importeObjetivo) * 100

// Dias restantes
diasRestantes = Max(0, (fechaFin - DateTime.UtcNow).Days)

// Dias transcurridos
diasTranscurridos = (DateTime.UtcNow - fechaInicio).Days

// Velocidad diaria
velocidadDiaria = importeRecaudado / diasTranscurridos

// Proyeccion final
proyeccionFinal = importeRecaudado + (velocidadDiaria * diasRestantes)

// Backing promedio
backingPromedio = totalRecaudado / numBackers

// Porcentaje del total (reward)
porcentajeDelTotal = (totalRecaudadoReward / importeRecaudadoCampania) * 100
```

### 9.4 Validacion de Ownership

**IMPORTANTE:** Los endpoints de dashboard requieren validacion de ownership en backend:
- `GET /api/campanias/{id}/backings` - Validar que `campania.ArtistaId == artista.Id` del usuario autenticado
- `GET /api/campanias/{id}/stats` - Validar que `campania.ArtistaId == artista.Id` del usuario autenticado

Frontend asume que backend valida ownership. Si backend retorna 403 (errorCode 3002), frontend debe mostrar mensaje apropiado y redirigir.

### 9.5 Manejo de Datos Anonimos

- **nombreBacker:** Backend retorna "Anonimo" si `esAnonimo = true`, frontend solo muestra el valor recibido.
- **email:** Backend retorna `null` si anonimo o sin permiso, frontend mapea a `undefined`.

### 9.6 Series Temporales (ProgressoPorDia)

- **Formato de fecha:** "YYYY-MM-DD" (string, no ISO 8601 completo)
- **Acumulado:** Backend calcula post-procesamiento, frontend solo renderiza
- **Uso tipico:** Graficos de linea con Chart.js o Recharts

---

## 10. Checklist de Implementacion

### 10.1 Archivos a Crear

- [ ] `src/shared/types/dashboard.ts` (9 interfaces)
- [ ] `src/shared/schemas/dashboard.schema.ts` (2 schemas + 2 types inferidos)

### 10.2 Archivos a Actualizar

- [ ] `src/shared/types/index.ts` (agregar `export * from "./dashboard"`)
- [ ] `src/shared/schemas/index.ts` (agregar `export * from "./dashboard.schema"`)
- [ ] `src/shared/constants/index.ts` (actualizar QUERY_KEYS, API_ROUTES, APP_ROUTES)
- [ ] `src/shared/utils/error-messages.ts` (agregar DASHBOARD_ERROR_MESSAGES y getDashboardErrorMessage)
- [ ] `src/shared/utils/format.ts` (agregar 6 funciones de calculo de metricas)
- [ ] `src/shared/utils/index.ts` (verificar exports, crear si no existe)

### 10.3 Validaciones Post-Implementacion

- [ ] Todos los types alineados con contracts.md (nombres y tipos coinciden)
- [ ] Schemas Zod con mensajes en espanol
- [ ] Constantes de query keys con typing correcto (`as const`)
- [ ] Funciones de calculo de metricas probadas con edge cases (divisiones por 0)
- [ ] Error messages cubriendo todos los error codes de contracts.md
- [ ] Imports circulares verificados (no deben existir)
- [ ] Build de `src/shared` exitoso (`tsc --noEmit` sin errores)

---

## 11. Siguiente Paso Sugerido

Una vez implementado este plan, ejecutar agentes de backend y frontend en paralelo:

1. **Backend:** Implementar Queries (GetDashboardResumenQuery, GetMisCampaniasQuery, GetCampaniaBackingsQuery, GetCampaniaStatsQuery)
2. **Admin Frontend:** Implementar componentes de dashboard (DashboardPage, MisCampaniasPage, CampaniaStatsPage, BackingsTable)
3. **Landing Frontend:** No requiere cambios (dashboard es admin-only)

**Prioridad:** Backend primero (para poder probar endpoints desde Swagger), luego Admin frontend.

---

**Fin del Plan de Contratos Shared: Dashboard Artista**
