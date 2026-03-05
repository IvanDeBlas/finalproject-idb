# Plan de Contratos Shared: cp-tracking-metricas

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Basado en:** docs/user-stories/cp-tracking-metricas/contracts.md

---

## 1. Resumen

- **Total de types nuevos:** 11 interfaces + 2 union/const types (todos nuevos en archivo nuevo)
- **Total de schemas Zod nuevos:** 2 schemas con sus types inferidos (archivo nuevo)
- **Constantes nuevas:** 3 endpoints en `API_ROUTES.crowdpromotion`, 2 query key functions en `QUERY_KEYS.crowdpromotion`, 1 objeto de tipo de evento, 5 claves de storage, 4 error codes nuevos
- **Utilidades nuevas:** 1 mapper de tipo de evento a label, 1 mapper de tipo de evento a color badge, 4 error messages nuevos en error-messages.ts

### Estado del codigo existente

Los tipos de crowdpromotion ya existen en:
- `src/shared/types/crowdpromotion.ts` - archivo existente con US-CP-01 a US-CP-04
- `src/shared/schemas/crowdpromotion.schema.ts` - archivo existente

Esta feature US-CP-05 sigue el patron establecido en US-CP-04 (`cp-tareas-promocion`): se agrega una seccion nueva al final de cada archivo existente, separada por comentario de bloque `// ========== Tracking y Metricas - US-CP-05 ==========`. Ademas se crea un archivo de constantes especificas de tracking.

El contracts.md de esta feature define directamente el nombre del archivo de types como `cp-tracking-metricas.ts`, pero siguiendo la convencion del proyecto que consolida todos los tipos de crowdpromotion en un unico archivo (`crowdpromotion.ts`), se opta por agregar al archivo existente. Esto evita crear un archivo de types nuevo que fracture la cohesion del dominio.

---

## 2. Types (`src/shared/types/crowdpromotion.ts`)

### 2.1 Estrategia: Agregar seccion al final del archivo existente

El archivo ya contiene secciones para US-CP-01, US-CP-02, US-CP-03 y US-CP-04 separadas con el patron `// ========== ... ==========`. Se agrega la seccion `// ========== Tracking y Metricas - US-CP-05 ==========` al final.

### 2.2 Union Type: TipoEventoPromo

| Tipo | Valores | Descripcion |
|------|---------|-------------|
| `TipoEventoPromo` | `1 \| 2 \| 3 \| 4 \| 5` | Numeric union alineado con `Maestra_TipoEventoPromo`. 1=Click, 2=PageView, 3=Signup, 4=Backing, 5=Share |

Se usa numeric union en lugar de string union porque el backend serializa `TipoEventoPromoId` como `int` en el payload de `RegistrarEventoDto`. Esto es consistente con el patron de `EstadoTareaPromo` ya definido en US-CP-04.

### 2.3 DTOs de Request (Escritura desde el frontend)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `RegistrarEventoRequest` | `codigoReferido?: string`, `tipoEventoPromoId: TipoEventoPromo`, `campaniaCrowdfundingId?: string`, `urlOrigen?: string`, `urlReferer?: string`, `utmSource?: string`, `utmMedium?: string`, `utmCampaign?: string` | Body para POST /tracking/evento. Todos los campos excepto tipoEventoPromoId son opcionales. Endpoint publico, sin auth. |
| `RegistrarConversionRequest` | `codigoReferido: string`, `campaniaCrowdfundingId: string`, `aportacionCrowdfundingId: string`, `valorMonetario: number`, `monedaId: number`, `userIdAfectado: string` | Body para POST /tracking/conversion. Uso interno entre modulos (Crowdfunding -> Crowdpromotion). No llamado directamente desde UI. |

### 2.4 DTOs de Response (Escritura desde el frontend)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `RegistrarEventoResponse` | `eventoId: string`, `registrado: boolean` | Response del POST /tracking/evento. Siempre 201 si el evento se inserta independientemente de si el codigoReferido es valido. |
| `RegistrarConversionResponse` | `eventoId: string`, `comisionCalculada: number`, `monedaNombre: string \| undefined`, `walletTransaccionId: string \| undefined`, `comisionAcreditada: boolean` | Response del POST /tracking/conversion. comisionCalculada = 0 y comisionAcreditada = false si el programa estaba inactivo o el promotor fue dado de baja. |

### 2.5 DTOs del Dashboard del Artista (GET /programas/{id}/metricas)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `ProgramaMetricasKpis` | `totalClicks: number`, `totalPageViews: number`, `totalSignups: number`, `totalConversiones: number`, `valorTotalGenerado: number`, `monedaNombre: string \| null`, `tasaConversion: number`, `comisionesTotales: number` | KPIs agregados del programa. Todos los contadores son COUNT/SUM de PromoEvento. tasaConversion = totalConversiones / totalClicks * 100 (0 si sin clicks). |
| `RankingPromotorItem` | `promotorId: string`, `promotorNombre: string`, `tipoPromotorNombre: string \| null`, `clicks: number`, `pageViews: number`, `signups: number`, `conversiones: number`, `valorGenerado: number`, `comisionAcumulada: number` | Item del ranking de promotores ordenado por conversiones DESC. Solo promotores con al menos 1 evento en el periodo. |
| `EventosPorDiaItem` | `fecha: string`, `clicks: number`, `pageViews: number`, `signups: number`, `conversiones: number` | Punto de la serie temporal para el grafico de lineas de Recharts. fecha en formato YYYY-MM-DD. Solo dias con al menos 1 evento. |
| `ProgramaMetricasResponse` | `programaId: string`, `programaTitulo: string`, `fechaDesde: string`, `fechaHasta: string`, `kpis: ProgramaMetricasKpis`, `rankingPromotores: RankingPromotorItem[]`, `eventosPorDia: EventosPorDiaItem[]` | Response completo del GET metricas del artista. fechaDesde/fechaHasta en formato YYYY-MM-DD (periodo aplicado por el backend). |

### 2.6 DTOs del Dashboard del Promotor (GET /promotor/metricas)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `PromotorMetricasKpis` | `misClicks: number`, `misPageViews: number`, `misSignups: number`, `misConversiones: number`, `miValorGenerado: number`, `miComisionAcumulada: number`, `monedaNombre: string \| null`, `miTasaConversion: number` | KPIs individuales del promotor. miTasaConversion = misConversiones / misClicks * 100. monedaNombre null si sin conversiones. |
| `EventoReciente` | `id: string`, `tipoEventoId: TipoEventoPromo`, `tipoEventoNombre: string`, `valorMonetario: number`, `comisionGenerada: number \| null`, `monedaNombre: string \| null`, `fechaEvento: string` | Evento reciente tipo Backing en el historial del promotor. Solo tipo 4 (Backing) porque tienen ValorMonetario. comisionGenerada es null si no se genero comision. fechaEvento en ISO 8601 UTC. |
| `PromotorMetricasResponse` | `promotorId: string`, `promotorNombre: string`, `programaId: string \| null`, `programaTitulo: string \| null`, `fechaDesde: string`, `fechaHasta: string`, `kpis: PromotorMetricasKpis`, `eventosRecientes: EventoReciente[]` | Response completo del GET metricas del promotor. programaId/programaTitulo son null si la consulta no filtra por programa especifico. eventosRecientes: maximo 20 items, ordenados por fechaEvento DESC. |

### 2.7 Tipo auxiliar: FiltroFechas (parametros de query)

| Tipo | Propiedades | Descripcion |
|------|-------------|-------------|
| `FiltroFechas` | `fechaDesde?: string`, `fechaHasta?: string` | Parametros de query para los endpoints de metricas. Formato YYYY-MM-DD. Reutilizado en los hooks de ambos dashboards. |
| `FiltroMetricasPromotor` | `programaId?: string`, `fechaDesde?: string`, `fechaHasta?: string` | Extiende FiltroFechas con el selector de programa del promotor. |

### 2.8 Alineamiento Backend-Frontend

| Interface TS | DTO C# Backend | Alineamiento |
|---|---|---|
| `RegistrarEventoRequest` | `RegistrarEventoDto` | Completo. `TipoEventoPromo` en TS equivale a `int` en C#. Todos los campos opcionales en TS excepto `tipoEventoPromoId`. |
| `RegistrarEventoResponse` | `RegistrarEventoResponseDto` | Completo. `eventoId: Guid` en C# serializa como `string` en JSON. |
| `RegistrarConversionRequest` | `RegistrarConversionDto` | Completo. `valorMonetario: decimal` en C# equivale a `number` en TS. Solo para uso interno. |
| `RegistrarConversionResponse` | `RegistrarConversionResponseDto` | Completo. `walletTransaccionId: Guid?` en C# serializa como `string \| null` en JSON, modelado como `string \| undefined` en TS. |
| `ProgramaMetricasKpis` | `ProgramaMetricasKpisDto` | Completo. `decimal` en C# equivale a `number` en TS. |
| `RankingPromotorItem` | `RankingPromotorItemDto` | Completo. |
| `EventosPorDiaItem` | `EventosPorDiaItemDto` | Completo. `fecha: string` formato YYYY-MM-DD. |
| `ProgramaMetricasResponse` | `ProgramaMetricasResponseDto` | Completo. |
| `PromotorMetricasKpis` | `PromotorMetricasKpisDto` | Completo. |
| `EventoReciente` | `EventoRecienteDto` | Completo. `FechaEvento: DateTime` en C# serializa como ISO 8601 string en JSON. |
| `PromotorMetricasResponse` | `PromotorMetricasResponseDto` | Completo. |

### 2.9 Nota sobre `null` vs `undefined` en esta feature

Esta feature mezcla los dos patrones presentes en el proyecto:
- Los campos opcionales de los tipos de sesion de tracking (`FiltroFechas`, `FiltroMetricasPromotor`) siguen el patron TS de `undefined` para opcionales.
- Los campos del response que el backend puede devolver como `null` en JSON (`monedaNombre`, `programaId`, `programaTitulo`, `comisionGenerada`) se modelan como `string | null` y `number | null` para reflejar exactamente el JSON serializado por .NET. Esto difiere del patron de US-CP-04 que usaba `undefined` para estos casos, pero es mas preciso para tipos de response (no de formularios).

---

## 3. Schemas Zod (`src/shared/schemas/crowdpromotion.schema.ts`)

### 3.1 Estrategia: Agregar al final del archivo existente

El archivo ya tiene schemas para US-CP-01 a US-CP-04. Se agrega nueva seccion al final con el comentario `// ========== Tracking y Metricas - US-CP-05 ==========`.

### 3.2 Schemas de Validacion

| Schema | Campos | Reglas de validacion | Tipo inferido |
|--------|--------|----------------------|---------------|
| `filtroFechasSchema` | `fechaDesde?`, `fechaHasta?` | `fechaDesde`: `.string().optional()`. `fechaHasta`: `.string().optional()`. Refinamiento cross-field: si ambas fechas estan presentes, `fechaDesde <= fechaHasta`, mensaje: `'La fecha de inicio no puede ser posterior a la fecha fin'`, path: `['fechaDesde']`. | `FiltroFechasFormData` |
| `filtroMetricasPromotorSchema` | Extiende `filtroFechasSchema` con `programaId?` | `programaId`: `.string().uuid('ID de programa invalido').optional()`. Hereda el refinamiento de fechas de `filtroFechasSchema`. | `FiltroMetricasPromotorFormData` |

### 3.3 Justificacion: Schema solo para filtros de fecha

El endpoint `POST /tracking/evento` NO necesita schema Zod porque la request se construye programaticamente en el hook `useTrackingInterceptor` (leyendo `window.location.search`), no mediante un formulario de usuario. No hay inputs del usuario que validar en tiempo real.

El endpoint `POST /tracking/conversion` tampoco necesita schema Zod porque es una llamada interna entre modulos del backend, nunca expuesta a un formulario del frontend.

Los dashboards tienen filtros de fecha que si son inputs del usuario: el artista selecciona un rango en la Pantalla 2 (Admin) y el promotor puede tener un selector de programa en la Pantalla 1 (Landing). Para estos se definen `filtroFechasSchema` y `filtroMetricasPromotorSchema`.

### 3.4 Alineamiento con FluentValidation del Backend

| Campo | Regla Backend | Regla Zod | Alineado |
|-------|---------------|-----------|---------|
| `fechaDesde` / `fechaHasta` cruce | `.Must((q, f) => f == null || q.FechaHasta == null || f <= q.FechaHasta).WithErrorCode("1036")` | `.refine(d => !d.fechaDesde || !d.fechaHasta || d.fechaDesde <= d.fechaHasta, 'La fecha de inicio no puede ser posterior a la fecha fin')` | Si |
| `programaId` formato | Validado como `Guid` en C# | `.string().uuid('ID de programa invalido').optional()` | Si |

### 3.5 Nota: Compatibilidad de `filtroFechasSchema.extend()`

`filtroMetricasPromotorSchema` se construye como `filtroFechasSchema.extend({ programaId: ... })`. En Zod v3, `extend` sobre un schema con `.refine` no es compatible directamente; en ese caso se puede usar `z.object({ ...filtroFechasSchema.shape, programaId: ... }).refine(...)` replicando el refinamiento. El implementador debe verificar la version de Zod instalada en `src/shared/node_modules/zod/package.json` antes de elegir la forma de extension.

---

## 4. Constantes (`src/shared/constants/index.ts`)

### 4.1 Endpoints API: Nuevas entradas en `API_ROUTES.crowdpromotion`

La seccion `crowdpromotion` del objeto `API_ROUTES` ya existe con subseccion `tracking` y funciones de programas. Se agregan las siguientes entradas:

| Propiedad | Valor | Endpoint |
|-----------|-------|----------|
| `tracking.evento: string` | `'/api/crowdpromotion/tracking/evento'` | `POST /api/crowdpromotion/tracking/evento` |
| `programaMetricas: (programaId: string) => string` | `` `/api/crowdpromotion/programas/${programaId}/metricas` `` | `GET /api/crowdpromotion/programas/{programaId}/metricas` |
| `promotorMetricas: string` | `'/api/crowdpromotion/promotor/metricas'` | `GET /api/crowdpromotion/promotor/metricas` |

El endpoint `POST /tracking/conversion` no se agrega a `API_ROUTES` porque es una llamada interna entre modulos del backend; el frontend nunca lo invoca directamente.

Estructura esperada dentro del objeto existente:
```
API_ROUTES.crowdpromotion.tracking.evento
API_ROUTES.crowdpromotion.programaMetricas(programaId)
API_ROUTES.crowdpromotion.promotorMetricas
```

### 4.2 Query Keys: Nuevas entradas en `QUERY_KEYS.crowdpromotion`

La seccion `crowdpromotion` del objeto `QUERY_KEYS` ya tiene `promotor`, `maestras`, `programas`, `inscripciones` y `tareas`. Se agrega una nueva subseccion `metricas`:

| Key | Patron | Uso |
|-----|--------|-----|
| `metricas.programa: (programaId: string, fechaDesde?: string, fechaHasta?: string) => readonly [...]` | `['crowdpromotion', 'metricas', 'programa', programaId, fechaDesde, fechaHasta] as const` | Clave para GET /programas/{id}/metricas. Los parametros de fecha forman parte de la clave para que TanStack Query refresque automaticamente al cambiar el filtro. |
| `metricas.promotor: (programaId?: string, fechaDesde?: string, fechaHasta?: string) => readonly [...]` | `['crowdpromotion', 'metricas', 'promotor', programaId, fechaDesde, fechaHasta] as const` | Clave para GET /promotor/metricas. programaId puede ser undefined si el promotor no ha seleccionado un programa. |

Ejemplo de uso en hooks:
```typescript
// Admin: query de metricas del programa con filtro de fechas
queryKey: QUERY_KEYS.crowdpromotion.metricas.programa(programaId, fechaDesde, fechaHasta)

// Landing: query de metricas del promotor por programa
queryKey: QUERY_KEYS.crowdpromotion.metricas.promotor(selectedProgramaId, undefined, undefined)

// Invalidar tras cambio de datos (no aplica en MVP - read only)
queryClient.invalidateQueries({ queryKey: ['crowdpromotion', 'metricas', 'programa', programaId] })
```

### 4.3 Constantes de Tipo de Evento: Nueva seccion en index.ts

Se agrega al final del archivo la seccion `// ========== Crowdpromotion - Tipos de Evento Promo (US-CP-05) ==========`:

| Constante | Tipo | Valores |
|-----------|------|---------|
| `TIPO_EVENTO_PROMO` | `const object` | `Click: 1, PageView: 2, Signup: 3, Backing: 4, Share: 5` |
| `TIPO_EVENTO_PROMO_LABELS` | `Record<number, string>` | `1: 'Click', 2: 'Vista', 3: 'Registro', 4: 'Backing', 5: 'Share'` |

Nota: `TIPO_EVENTO_PROMO_LABELS` usa `'Vista'` para PageView y `'Registro'` para Signup porque son las etiquetas de UI definidas en los badges de la Pantalla 1 (ui-ux.md, seccion "Badges por tipo de evento"). El valor `'Share'` coincide en ambos porque no hay label alternativo en el mockup.

### 4.4 Constantes de Tracking de Sesion: Nuevo archivo `src/shared/constants/tracking.ts`

Las constantes de tracking de URL y sessionStorage son especificas de esta feature y no encajan en el `index.ts` general. Se crean en un archivo separado que se re-exporta desde el barrel `src/shared/constants/index.ts` con `export * from './tracking'`.

| Constante | Tipo | Valor | Descripcion |
|-----------|------|-------|-------------|
| `TRACKING_PARAMS` | `const object` | `{ ref: 'ref', utmSource: 'utm_source', utmMedium: 'utm_medium', utmCampaign: 'utm_campaign' }` | Nombres de los parametros de URL que el hook `useTrackingInterceptor` debe leer de `window.location.search`. |
| `TRACKING_STORAGE_KEYS` | `const object` | `{ ref: 'wp_ref', utmSource: 'wp_utm_source', utmMedium: 'wp_utm_medium', utmCampaign: 'wp_utm_campaign', campaniaId: 'wp_campania_id' }` | Claves de sessionStorage para persistir el contexto de tracking durante la sesion del usuario. Alineadas con la tabla de persistencia de ui-ux.md. |
| `TRACKING_COOKIE_KEY` | `string` | `'wp_ref'` | Nombre de la cookie de 30 minutos para persistir el codigoReferido entre tabs. Separado de sessionStorage para cubrir el caso de multiples tabs. |
| `TRACKING_COOKIE_TTL_MINUTES` | `number` | `30` | TTL de la cookie en minutos (alineado con ui-ux.md: "Cookie: 30 min"). |
| `UTM_SOURCE_WEPLAY` | `string` | `'weplay'` | Valor fijo de utm_source para todos los eventos de la plataforma (RN del contracts.md). |
| `UTM_MEDIUM_REFERRAL` | `string` | `'referral'` | Valor fijo de utm_medium. |

Estructura del archivo `tracking.ts`:
```
src/shared/constants/
└── tracking.ts        NUEVO
```

### 4.5 Constantes de Validacion: Nuevas entradas en `VALIDATION`

Se agregan al objeto `VALIDATION` existente:

| Clave | Valor | Descripcion |
|-------|-------|-------------|
| `TRACKING_CODIGO_REFERIDO_MAX` | `50` | Max length de `codigoReferido` (alineado con contracts.md campo `codigoReferido max 50`). |
| `TRACKING_URL_MAX` | `2048` | Max length de `urlOrigen` y `urlReferer`. Reutilizado tambien en `TAREA_URL_PRUEBA_MAX` de US-CP-04. |
| `TRACKING_UTM_MAX` | `100` | Max length de campos UTM (source, medium, campaign). |
| `METRICAS_EVENTOS_RECIENTES_MAX` | `20` | Limite de `eventosRecientes` en la respuesta del promotor (RN-08 del contracts.md). |

---

## 5. Utilidades (`src/shared/utils/`)

### 5.1 Nuevas entradas en `error-messages.ts`

Se agregan al objeto `ERROR_CODE_MESSAGES` (o al pattern equivalente del archivo) los 4 codigos nuevos definidos en contracts.md. Se usa la seccion `// ========== Crowdpromotion - Tracking y Metricas (US-CP-05) ==========`:

| Codigo | Mensaje | Contexto de uso |
|--------|---------|-----------------|
| `'1033'` | `'Tipo de evento no reconocido.'` | POST /tracking/evento con tipoEventoPromoId fuera del rango 1-5 |
| `'1034'` | `'Este tipo de evento no puede registrarse por esta via.'` | POST /tracking/evento con tipoEventoPromoId = 4 (Backing, reservado para uso interno) |
| `'1035'` | `'El valor de la aportacion debe ser mayor que cero.'` | POST /tracking/conversion con valorMonetario <= 0 |
| `'4032'` | `'Demasiadas peticiones en poco tiempo. Espera unos minutos e intenta de nuevo.'` | Rate limit excedido (429) para eventos tipo Click |

No se agrega un objeto dedicado `TRACKING_ERROR_MESSAGES` (a diferencia del patron de US-CP-04 que creo `TAREA_PROMOCION_ERROR_MESSAGES`) porque el tracking de eventos es silencioso: los errores 429 y 400 del endpoint publico nunca se muestran al usuario (la UI ignora silenciosamente los errores del tracking segun ui-ux.md). Solo los errores de los dashboards de metricas (401, 403, 404, 500) pueden mostrarse, y estos ya estan cubiertos por los codigos existentes `3001`, `4026`, `2015`, `2016`, `2019`, `5000`.

### 5.2 Nuevas funciones en `mappers.ts`

Se agrega al final del archivo la seccion `// ========== Crowdpromotion - Tracking y Metricas Mappers (US-CP-05) ==========`:

| Funcion | Input | Output | Descripcion |
|---------|-------|--------|-------------|
| `mapTipoEventoPromoToLabel` | `tipoEventoId: number` | `string` | Devuelve el label de UI para el tipo de evento. Usa `TIPO_EVENTO_PROMO_LABELS`. Para tipos desconocidos devuelve `'Evento'` como fallback. Util para el badge de tipo en la lista de eventos recientes del promotor. |
| `mapTipoEventoPromoToBadgeClass` | `tipoEventoId: number` | `string` | Devuelve la clase CSS Tailwind para el badge del tipo de evento segun el design system de ui-ux.md. Ejemplo: `1 -> 'bg-blue-950/50 text-[#3b82f6] border border-blue-800/50'`. Cubre los 5 tipos mas un fallback para desconocidos. |
| `formatTasaConversion` | `tasa: number` | `string` | Formatea la tasa de conversion como porcentaje con 2 decimales. Ejemplo: `1.11 -> '1.11%'`. Util para los KPI de ambos dashboards. Devuelve `'0.00%'` si tasa es 0 o falsy. |

Ejemplo de implementacion planificada para `mapTipoEventoPromoToBadgeClass`:
```typescript
// Usa el design system de ui-ux.md - Badges por tipo de evento
const BADGE_CLASSES: Record<number, string> = {
    1: 'bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs',    // Click
    2: 'bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs',   // PageView/Vista
    3: 'bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs',   // Signup/Registro
    4: 'bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs', // Backing
    5: 'bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs',   // Share
};

export function mapTipoEventoPromoToBadgeClass(tipoEventoId: number): string {
    return BADGE_CLASSES[tipoEventoId] ?? 'bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs';
}
```

### 5.3 Nota: No se crea `formatters.ts` para esta feature

Las funciones `formatTasaConversion` y el formateo de valores monetarios pueden integrarse en el archivo `format.ts` ya existente en `src/shared/utils/`. No se crea un nuevo archivo de formatters especifico para tracking.

---

## 6. Archivos a Crear o Modificar

```
src/shared/
├── types/
│   └── crowdpromotion.ts         AMPLIAR: nueva seccion US-CP-05 al final
├── schemas/
│   └── crowdpromotion.schema.ts  AMPLIAR: 2 schemas + types inferidos al final
├── constants/
│   ├── index.ts                  AMPLIAR: endpoints, query keys, tipo evento, VALIDATION
│   └── tracking.ts               NUEVO: constantes TRACKING_PARAMS, TRACKING_STORAGE_KEYS, etc.
└── utils/
    ├── error-messages.ts         AMPLIAR: 4 nuevos error codes
    ├── mappers.ts                 AMPLIAR: 3 nuevas funciones
    └── format.ts                  AMPLIAR: funcion formatTasaConversion
```

El archivo `src/shared/index.ts` no requiere cambios porque ya re-exporta con `export * from './constants'` y el nuevo `tracking.ts` se agrega al barrel de constants existente.

Los archivos de types e index de types/schemas/utils ya tienen `export *` genericos que incluiran automaticamente los nuevos exports de los archivos modificados.

---

## 7. Integracion por App

### 7.1 Landing (Vite + React 18) - Dashboard del Promotor

Tipos y constantes que consume la feature `tracking`:

| Archivo | Exportaciones usadas |
|---------|----------------------|
| `types/crowdpromotion.ts` | `TipoEventoPromo`, `RegistrarEventoRequest`, `RegistrarEventoResponse`, `FiltroMetricasPromotor`, `PromotorMetricasKpis`, `EventoReciente`, `PromotorMetricasResponse` |
| `schemas/crowdpromotion.schema.ts` | `filtroMetricasPromotorSchema`, `FiltroMetricasPromotorFormData` |
| `constants/index.ts` | `API_ROUTES.crowdpromotion.tracking.evento`, `API_ROUTES.crowdpromotion.promotorMetricas`, `QUERY_KEYS.crowdpromotion.metricas.promotor`, `TIPO_EVENTO_PROMO`, `TIPO_EVENTO_PROMO_LABELS` |
| `constants/tracking.ts` | `TRACKING_PARAMS`, `TRACKING_STORAGE_KEYS`, `TRACKING_COOKIE_KEY`, `TRACKING_COOKIE_TTL_MINUTES`, `UTM_SOURCE_WEPLAY`, `UTM_MEDIUM_REFERRAL` |
| `utils/mappers.ts` | `mapTipoEventoPromoToLabel`, `mapTipoEventoPromoToBadgeClass` |
| `utils/format.ts` | `formatTasaConversion` |

Flujo de uso en Landing:
1. Hook `useTrackingInterceptor` (montado en App.tsx) lee `TRACKING_PARAMS` de la URL, persiste en `TRACKING_STORAGE_KEYS` y llama a `API_ROUTES.crowdpromotion.tracking.evento` con `RegistrarEventoRequest`.
2. Al navegar a pagina de campana: el hook usa `TIPO_EVENTO_PROMO.PageView` para construir el request.
3. Al completar registro: el hook usa `TIPO_EVENTO_PROMO.Signup`.
4. Pagina `/promotor/metricas`: usa `useQuery` con `QUERY_KEYS.crowdpromotion.metricas.promotor` para obtener `PromotorMetricasResponse`. El selector de programa actualiza el query param `programaId`.
5. Badges de eventos: `mapTipoEventoPromoToBadgeClass(evento.tipoEventoId)` para el color del Badge de shadcn.
6. Tasa de conversion: `formatTasaConversion(kpis.miTasaConversion)` para el texto inline.

### 7.2 Admin (Next.js 14) - Dashboard del Artista

Tipos y constantes que consume la feature `metricas`:

| Archivo | Exportaciones usadas |
|---------|----------------------|
| `types/crowdpromotion.ts` | `FiltroFechas`, `ProgramaMetricasKpis`, `RankingPromotorItem`, `EventosPorDiaItem`, `ProgramaMetricasResponse` |
| `schemas/crowdpromotion.schema.ts` | `filtroFechasSchema`, `FiltroFechasFormData` |
| `constants/index.ts` | `API_ROUTES.crowdpromotion.programaMetricas`, `QUERY_KEYS.crowdpromotion.metricas.programa`, `TIPO_EVENTO_PROMO_LABELS`, `METRICAS_EVENTOS_RECIENTES_MAX` |
| `utils/format.ts` | `formatTasaConversion` |

Flujo de uso en Admin:
1. Pagina `/campanias/{id}/crowdpromotion/programas/{programaId}/metricas` (pestana "Metricas"): usa `useQuery` con `QUERY_KEYS.crowdpromotion.metricas.programa(programaId, fechaDesde, fechaHasta)`.
2. Filtro de fechas: formulario con `filtroFechasSchema` via `react-hook-form + zodResolver`. Al hacer submit, actualiza los query params de la URL y TanStack Query refetch automaticamente.
3. Grafico de lineas (Recharts): `EventosPorDiaItem[]` se pasa directamente como `data` al `<LineChart>`.
4. Ranking de promotores: `RankingPromotorItem[]` se renderiza en la tabla con ordenacion local por columna.

---

## 8. Dependencias

- `zod` ya instalado en `src/shared/node_modules/zod`
- Ningun package adicional requerido
- No se requiere cambio en `tsconfig.json`, `package.json` ni `src/shared/index.ts`
- El archivo `tracking.ts` nuevo debe agregarse al barrel `constants/index.ts` con `export * from './tracking'`

---

## 9. Notas de Implementacion

### 9.1 Naming convention para types nuevos

Se sigue el patron del archivo existente `crowdpromotion.ts`:
- `interface` para todos los tipos DTO de request y response
- `type` para union types (`TipoEventoPromo`)
- Interfaces de KPIs sin sufijo `Dto` en TS (`ProgramaMetricasKpis`, `PromotorMetricasKpis`)
- PascalCase para todos los nombres

### 9.2 `null` vs `undefined` en tipos de response

A diferencia del patron de US-CP-04 que usa `undefined` para campos opcionales en responses, esta feature usa `string | null` y `number | null` para campos que el backend serializa explicitamente como `null` (por ejemplo `programaId`, `comisionGenerada`, `monedaNombre`). Esto es mas preciso para tipos de response donde `null` tiene significado semantico distinto a "campo no presente".

Los tipos de filtro/request (`FiltroFechas`, `FiltroMetricasPromotor`, `RegistrarEventoRequest`) si usan `undefined` porque son opcionales del lado del cliente.

### 9.3 TipoEventoPromo como numeric union, no string union

Se usa `type TipoEventoPromo = 1 | 2 | 3 | 4 | 5` en lugar de `type TipoEventoPromo = 'Click' | 'PageView' | ...` porque el backend serializa `TipoEventoPromoId` como `int` en el JSON. La conversion a label de UI se hace con `TIPO_EVENTO_PROMO_LABELS[tipoEventoId]` en el mapper, no en el tipo.

### 9.4 El hook useTrackingInterceptor no usa schemas Zod

El `useTrackingInterceptor` construye el `RegistrarEventoRequest` programaticamente desde los parametros de URL: no hay inputs del usuario que validar. Las constantes `TRACKING_PARAMS` guian la extraccion de parametros de `window.location.search`. Los errores del endpoint se ignoran silenciosamente (429 no reintenta, 400 limpia sessionStorage).

### 9.5 Invalidacion de queries

Los endpoints de metricas son solo de lectura (no hay mutaciones del usuario que cambien los datos de PromoEvento). La invalidacion manual de query keys no es necesaria en MVP. TanStack Query refetcha automaticamente cuando cambian los params de la query key (fechaDesde, fechaHasta, programaId).

### 9.6 Posicion de las constantes de tipo de evento

`TIPO_EVENTO_PROMO` se define en `constants/index.ts` (no en el archivo de types `crowdpromotion.ts`) siguiendo el patron del proyecto donde `ESTADO_TAREA_PROMO` y `CAMPANIA_ESTADOS` estan en constants. Sin embargo, el `type TipoEventoPromo = 1 | 2 | 3 | 4 | 5` si va en `types/crowdpromotion.ts` para el tipado de las interfaces.

---

## 10. Checklist de Implementacion

### Types (`types/crowdpromotion.ts`)

- [ ] Agregar `type TipoEventoPromo = 1 | 2 | 3 | 4 | 5` con comentario de cada valor
- [ ] Agregar `interface RegistrarEventoRequest` con todos los campos opcionales excepto `tipoEventoPromoId`
- [ ] Agregar `interface RegistrarEventoResponse` con `eventoId` y `registrado`
- [ ] Agregar `interface RegistrarConversionRequest` (uso interno, documentar con JSDoc)
- [ ] Agregar `interface RegistrarConversionResponse` con `comisionCalculada`, `walletTransaccionId?: string`, `comisionAcreditada`
- [ ] Agregar `interface ProgramaMetricasKpis` con los 8 campos del DTO C#
- [ ] Agregar `interface RankingPromotorItem` con los 9 campos del DTO C#
- [ ] Agregar `interface EventosPorDiaItem` con `fecha: string` y los 4 contadores
- [ ] Agregar `interface ProgramaMetricasResponse` con `rankingPromotores` y `eventosPorDia` como arrays
- [ ] Agregar `interface PromotorMetricasKpis` con los 8 campos del DTO C#
- [ ] Agregar `interface EventoReciente` con `comisionGenerada: number | null`
- [ ] Agregar `interface PromotorMetricasResponse` con `programaId: string | null`
- [ ] Agregar `interface FiltroFechas` con `fechaDesde?` y `fechaHasta?`
- [ ] Agregar `interface FiltroMetricasPromotor` extendiendo `FiltroFechas` con `programaId?`

### Schemas (`schemas/crowdpromotion.schema.ts`)

- [ ] Agregar `filtroFechasSchema` con refinamiento de fechas y mensaje en espanol
- [ ] Agregar `export type FiltroFechasFormData = z.infer<typeof filtroFechasSchema>`
- [ ] Agregar `filtroMetricasPromotorSchema` extendiendo `filtroFechasSchema` con `programaId` uuid opcional
- [ ] Agregar `export type FiltroMetricasPromotorFormData = z.infer<typeof filtroMetricasPromotorSchema>`
- [ ] Verificar compatibilidad de `.extend()` con Zod version instalada (puede requerir `.and()` o reconstruccion del schema)

### Constantes (`constants/index.ts`)

- [ ] Agregar `API_ROUTES.crowdpromotion.tracking.evento` con valor string literal
- [ ] Agregar `API_ROUTES.crowdpromotion.programaMetricas` como funcion `(programaId: string) => string`
- [ ] Agregar `API_ROUTES.crowdpromotion.promotorMetricas` con valor string literal
- [ ] Agregar subseccion `metricas` en `QUERY_KEYS.crowdpromotion` con `.programa` y `.promotor`
- [ ] Agregar `TIPO_EVENTO_PROMO` const object con los 5 tipos
- [ ] Agregar `TIPO_EVENTO_PROMO_LABELS` con labels de UI (Click, Vista, Registro, Backing, Share)
- [ ] Agregar `TRACKING_CODIGO_REFERIDO_MAX: 50` en `VALIDATION`
- [ ] Agregar `TRACKING_UTM_MAX: 100` en `VALIDATION`
- [ ] Agregar `METRICAS_EVENTOS_RECIENTES_MAX: 20` en `VALIDATION`
- [ ] Agregar `export * from './tracking'` al final del barrel de constants

### Constantes de Tracking (`constants/tracking.ts` - NUEVO)

- [ ] Crear archivo `src/shared/constants/tracking.ts`
- [ ] Definir `TRACKING_PARAMS` con `ref`, `utmSource`, `utmMedium`, `utmCampaign`
- [ ] Definir `TRACKING_STORAGE_KEYS` con los 5 valores de sessionStorage (ref, utmSource, utmMedium, utmCampaign, campaniaId)
- [ ] Definir `TRACKING_COOKIE_KEY = 'wp_ref'`
- [ ] Definir `TRACKING_COOKIE_TTL_MINUTES = 30`
- [ ] Definir `UTM_SOURCE_WEPLAY = 'weplay'`
- [ ] Definir `UTM_MEDIUM_REFERRAL = 'referral'`

### Utilidades (`utils/error-messages.ts`)

- [ ] Agregar codigo `'1033'` con mensaje de tipo de evento no reconocido
- [ ] Agregar codigo `'1034'` con mensaje de tipo backing no permitido en endpoint publico
- [ ] Agregar codigo `'1035'` con mensaje de valor monetario invalido
- [ ] Agregar codigo `'4032'` con mensaje de rate limit excedido

### Utilidades (`utils/mappers.ts`)

- [ ] Agregar `mapTipoEventoPromoToLabel(tipoEventoId: number): string` con fallback `'Evento'`
- [ ] Agregar `mapTipoEventoPromoToBadgeClass(tipoEventoId: number): string` con las 5 clases Tailwind del design system y fallback
- [ ] Verificar que el fallback para tipo desconocido usa la clase del badge slate (gris)

### Utilidades (`utils/format.ts`)

- [ ] Agregar `formatTasaConversion(tasa: number): string` con `.toFixed(2)` y sufijo `%`
- [ ] Agregar caso especial para `tasa = 0` -> `'0.00%'`

### Verificacion final

- [ ] Verificar que todos los exports son accesibles via `src/shared/index.ts` (no requiere cambios en index.ts raiz)
- [ ] Verificar que el barrel de constants incluye `export * from './tracking'`
- [ ] Verificar alineamiento de tipos con los JSON de ejemplo del contracts.md
- [ ] Verificar que `TipoEventoPromo` se usa como tipo en `RegistrarEventoRequest.tipoEventoPromoId` y `EventoReciente.tipoEventoId`
