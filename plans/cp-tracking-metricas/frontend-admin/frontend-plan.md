# Plan Frontend: cp-tracking-metricas (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/admin (Next.js 14, App Router)

---

## 1. Resumen

- **Screens:** 1 (tab "Metricas" dentro de `PromoProgramaDetailClient` existente)
- **Componentes nuevos:** 8
- **Hooks nuevos:** 2
- **Services nuevos:** 1
- **Archivos mock nuevos:** 1

### Estrategia de integracion

El detalle del programa (`/crowdpromotion/programas/[id]`) ya existe y usa el patron de `<Tabs>` de shadcn con pestanas como "Info general", "Tareas", "Promotores", etc. Esta feature agrega una pestana nueva llamada **"Metricas"** en ese `<Tabs>`. La pestana es el punto de entrada del dashboard.

No se crea una ruta separada para metricas. El acceso es:
```
/crowdpromotion/programas/{programaId}  →  tab value="metricas"
```

El breadcrumb de la pantalla ya existe en `PromoProgramaDetailClient`. El componente `ProgramaMetricasTab` es auto-contenido: gestiona su propio filtro de fechas y datos.

---

## 2. Estructura de Carpetas

```
src/admin/src/
│
├── app/(dashboard)/crowdpromotion/programas/[id]/
│   └── components/
│       └── PromoProgramaDetailClient.tsx     MODIFICAR: agregar tab "Metricas"
│
├── components/crowdpromotion/metricas/        NUEVO directorio
│   ├── ProgramaMetricasTab.tsx               Orquestador principal del tab
│   ├── FiltroFechas.tsx                      Selector de rango de fechas
│   ├── KpiCardsGrid.tsx                      Grid de 4 KPI primarias + 2 secundarias
│   ├── KpiCard.tsx                           Card KPI individual reutilizable
│   ├── RankingPromotoresTable.tsx            Tabla ordenable de promotores
│   ├── DesgloseEventosPanel.tsx              Panel con barras de progreso por tipo
│   └── GraficoTemporal.tsx                  LineChart de Recharts con 3 series
│
├── hooks/
│   ├── use-programa-metricas.ts              NUEVO: query hook con filtro de fechas
│   └── use-sortable-table.ts                 NUEVO: hook de ordenacion client-side
│
├── services/
│   └── metricas.service.ts                   NUEVO: GET /programas/{id}/metricas
│
└── __mocks__/
    └── cp-tracking-metricas.mock.ts           NUEVO: mocks para tests
```

---

## 3. Componentes

### 3.1 ProgramaMetricasTab

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/ProgramaMetricasTab.tsx`

**Responsabilidad:** Orquestador del tab de metricas. Gestiona el estado del filtro de fechas en los query params de la URL, coordina el refetch al aplicar el filtro, y renderiza los cuatro paneles del dashboard (KPIs, ranking, desglose, grafico) con sus estados de carga/error/vacio.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa cuyas metricas se consultan |

**Estado local:**
- `filtroLocal: { fechaDesde: string; fechaHasta: string }` - estado de los inputs del formulario de fechas antes de aplicar
- Los filtros aplicados viven en los query params de la URL (`searchParams`), no en estado local

**Dependencias:**
- Hooks: `useProgramaMetricas`, `useSortableTable`
- Componentes: `FiltroFechas`, `KpiCardsGrid`, `RankingPromotoresTable`, `DesgloseEventosPanel`, `GraficoTemporal`
- shadcn/ui: `Button`, `Card`, `Skeleton`
- lucide-react: `AlertCircle`, `RotateCcw`
- next/navigation: `useSearchParams`, `useRouter`

**Logica de filtro de fechas:**
1. Al montar: leer `?fechaDesde=&fechaHasta=` de los query params para inicializar `filtroLocal`
2. El hook `useProgramaMetricas` recibe las fechas de los query params (no del estado local)
3. Al presionar "Aplicar": actualizar los query params en la URL (sin navegacion) y TanStack Query refetch automaticamente al cambiar la query key
4. Al presionar "Limpiar": remover `fechaDesde` y `fechaHasta` de los query params

**Renderizado condicional:**

| Estado | Que renderiza |
|--------|---------------|
| `isLoading` | `MetricasSkeletonLoader` (inline, sin componente separado) |
| `isError` | Card de error centrado con boton "Reintentar" |
| `data` con kpis en 0 y ranking vacio | Componentes normales pero con valores 0 y `EmptyRanking` en la tabla |
| `data` con datos | Layout completo con las 4 secciones |

**Directive:** `"use client"` (es un Client Component porque usa hooks, URL params y el formulario de fechas)

---

### 3.2 FiltroFechas

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/FiltroFechas.tsx`

**Responsabilidad:** Formulario de seleccion de rango de fechas con validacion. Muestra los inputs de fecha, el boton "Aplicar", el boton "Limpiar" (solo cuando hay filtro activo) y el badge de filtro aplicado. Delega la logica de aplicacion al padre via callbacks.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `fechaDesde` | `string` | No | Valor del input fecha inicio (YYYY-MM-DD), controlado externamente |
| `fechaHasta` | `string` | No | Valor del input fecha fin (YYYY-MM-DD), controlado externamente |
| `onFechaDesdeChange` | `(value: string) => void` | Si | Callback al cambiar input fecha inicio |
| `onFechaHastaChange` | `(value: string) => void` | Si | Callback al cambiar input fecha fin |
| `onAplicar` | `() => void` | Si | Callback al presionar "Aplicar" |
| `onLimpiar` | `() => void` | Si | Callback al presionar "Limpiar" |
| `tieneFiltroPeriodo` | `boolean` | Si | Controla visibilidad del boton "Limpiar" y badge de filtro activo |
| `isLoading` | `boolean` | Si | Muestra spinner en boton "Aplicar" mientras hay fetch en curso |

**Estado local:**
- `errorFechas: string | null` - mensaje de validacion inline cuando fechaDesde > fechaHasta

**Validacion (client-side):**
- Si `fechaDesde` y `fechaHasta` ambas presentes y `fechaDesde > fechaHasta`: deshabilitar boton "Aplicar" y mostrar `text-xs text-destructive` bajo el primer input
- El input de `fechaHasta` tiene `max={hoy}` para impedir fechas futuras (calcula `new Date().toISOString().split('T')[0]`)
- Si el rango supera 365 dias: mostrar aviso informativo (no bloquear)

**shadcn/ui:** `Input`, `Button`, `Badge`
**lucide-react:** `Calendar`, `X`, `Loader2`
**Directiva:** `"use client"`

---

### 3.3 KpiCardsGrid

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/KpiCardsGrid.tsx`

**Responsabilidad:** Renderiza el grupo completo de KPI cards en dos filas: 4 primarias (Clicks, Signups, Backings, Valor generado) y 2 secundarias (Tasa de conversion, Comisiones totales).

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `kpis` | `ProgramaMetricasKpis` | Si | Objeto KPIs del response de metricas |

**Estado local:** Ninguno

**Dependencias:**
- Componente: `KpiCard`
- Importa de shared: `ProgramaMetricasKpis`
- Importa de shared/utils: `formatTasaConversion`
- shadcn/ui: `Card`
- lucide-react: `MousePointerClick`, `UserPlus`, `ShoppingCart`, `Euro`, `TrendingUp`, `Wallet`

**Layout:**
- Primera fila: `grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4`
- Segunda fila: `grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8`

**Directiva:** Componente servidor (no usa hooks ni estado del cliente)

---

### 3.4 KpiCard

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/KpiCard.tsx`

**Responsabilidad:** Card KPI individual y reutilizable. Muestra un icono con fondo de color, valor numerico grande (o monetario con sufijo EUR) y etiqueta descriptiva. Soporta dos variantes: numerica y monetaria.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `label` | `string` | Si | Texto descriptivo del KPI (ej: "Total Clicks") |
| `value` | `string \| number` | Si | Valor a mostrar. Si es number, se formatea con `toLocaleString()` |
| `icon` | `LucideIcon` | Si | Icono de Lucide React |
| `iconBgClass` | `string` | Si | Clase Tailwind para el fondo del icono (ej: `"bg-blue-950/50"`) |
| `iconColorClass` | `string` | Si | Clase Tailwind para el color del icono (ej: `"text-[#3b82f6]"`) |
| `valueColorClass` | `string` | No | Clase Tailwind para el color del valor. Default: `"text-white"` |
| `suffix` | `string` | No | Sufijo a mostrar despues del valor (ej: `"EUR"`, `"%"`) |
| `note` | `string` | No | Texto auxiliar debajo del label (ej: `"conversiones / clicks"`) |
| `layout` | `"default" \| "horizontal"` | No | Default: renderizado vertical. "horizontal": icono + datos en fila (para KPIs secundarias) |
| `className` | `string` | No | Clase adicional para el Card contenedor |

**Estado local:** Ninguno

**shadcn/ui:** `Card`

**Directiva:** Componente servidor

---

### 3.5 RankingPromotoresTable

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/RankingPromotoresTable.tsx`

**Responsabilidad:** Tabla del ranking de promotores con 6 columnas y ordenacion client-side. Muestra avatar con fallback de inicial, nombre + badge de tipo de promotor, y metricas de clicks, conversiones, valor generado y comision. Las columnas numericas tienen cabeceras clicables que alternan ASC/DESC.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `items` | `RankingPromotorItem[]` | Si | Array de promotores del ranking |

**Estado local:**
- Gestionado via hook `useSortableTable<RankingPromotorItem>` (ver seccion 4.2)

**Columnas:**

| Columna | Campo | Tipo | Ordenable |
|---------|-------|------|-----------|
| # | posicion (1-N) | numerico | No |
| Promotor | `promotorNombre` + `tipoPromotorNombre` | texto + badge | No |
| Clicks | `clicks` | numero | Si |
| Conv. | `conversiones` | numero | Si |
| Valor | `valorGenerado` | monetario EUR | Si |
| Comision | `comisionAcumulada` | monetario EUR | Si |

**Empty state:** Cuando `items.length === 0`, renderiza una fila unica con mensaje "Sin promotores con actividad en este periodo" alineado al centro, con `colspan={6}`.

**Dependencias:**
- Hooks: `useSortableTable`
- Importa de shared: `RankingPromotorItem`
- shadcn/ui: `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableHead`, `TableCell`, `Badge`, `Avatar`, `AvatarFallback`
- lucide-react: `ArrowUpDown`, `ArrowUp`, `ArrowDown`
- Accesibilidad: `<th scope="col">`, `aria-sort` en columnas ordenables

**Directiva:** `"use client"` (usa el hook de ordenacion)

---

### 3.6 DesgloseEventosPanel

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/DesgloseEventosPanel.tsx`

**Responsabilidad:** Panel lateral que muestra el desglose de eventos por tipo (Clicks, PageViews, Signups, Conversiones, Shares) con un indicador de color, barra de progreso animada y cantidad absoluta. El ancho de cada barra se calcula como porcentaje del total de todos los eventos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `kpis` | `ProgramaMetricasKpis` | Si | KPIs del programa (fuente de los contadores por tipo) |

**Estado local:** Ninguno

**Calculo de barras:**
- Total de eventos = `totalClicks + totalPageViews + totalSignups + totalConversiones`
- Porcentaje de cada tipo = `(valor / total) * 100` o `0` si total es 0
- La barra de progreso usa `style={{ width: `${porcentaje}%` }}` con `transition-[width] duration-500 ease-out`

**Filas del panel:**

| Tipo | Color indicador | Color barra | Campo kpis |
|------|----------------|-------------|------------|
| Clicks | `bg-[#3b82f6]` | `bg-[#3b82f6]` | `totalClicks` |
| Page Views | `bg-[#64748b]` | `bg-[#64748b]` | `totalPageViews` |
| Signups | `bg-[#10b981]` | `bg-[#10b981]` | `totalSignups` |
| Conversiones | `bg-[#a855f7]` | `bg-[#a855f7]` | `totalConversiones` |

**shadcn/ui:** `Card`, `CardHeader`, `CardTitle`

**Directiva:** Componente servidor

---

### 3.7 GraficoTemporal

**Archivo:** `src/admin/src/components/crowdpromotion/metricas/GraficoTemporal.tsx`

**Responsabilidad:** Grafico de lineas temporal que muestra la evolucion diaria de 3 series de eventos (Clicks, Signups, Conversiones) usando Recharts. Incluye tooltip con estilos dark, leyenda y ejes con formato de fecha DD/MM. Renderiza un empty state cuando no hay datos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `datos` | `EventosPorDiaItem[]` | Si | Array de puntos temporales ordenados por fecha ASC |

**Estado local:** Ninguno

**Configuracion del grafico:**

| Elemento | Configuracion |
|----------|---------------|
| Tipo | `<ResponsiveContainer width="100%" height={256}>` |
| Chart | `<LineChart data={datos}>` |
| Eje X | `<XAxis dataKey="fecha" tickFormatter={(v) => formatFechaGrafico(v)} tick={{ fill: '#64748b', fontSize: 11 }}` |
| Eje Y | `<YAxis tick={{ fill: '#64748b', fontSize: 11 }} allowDecimals={false}` |
| Grid | `<CartesianGrid stroke="#334155" strokeDasharray="3 3" opacity={0.5}` |
| Linea Clicks | `<Line dataKey="clicks" stroke="#3b82f6" strokeWidth={2} dot={false}` |
| Linea Signups | `<Line dataKey="signups" stroke="#10b981" strokeWidth={2} dot={false}` |
| Linea Conversiones | `<Line dataKey="conversiones" stroke="#a855f7" strokeWidth={2} dot={false}` |
| Tooltip | `contentStyle={{ background: '#1e1e38', border: '1px solid #334155', borderRadius: '8px', color: '#fff' }}` |
| Leyenda | `<Legend wrapperStyle={{ fontSize: '12px', color: '#94a3b8' }}` |

**Funcion auxiliar `formatFechaGrafico`:** Convierte `YYYY-MM-DD` a `DD/MM` para el eje X. Se define localmente en el archivo.

**Empty state:** Cuando `datos.length === 0`, renderiza un div centrado con `text-sm text-[#64748b]` y texto "No hay datos para el periodo seleccionado".

**Nota Recharts:** `LineChart` y sus subcomponentes deben importarse desde `recharts`. Si recharts no esta instalado en `src/admin`, agregar `recharts` como dependencia en `src/admin/package.json`.

**Accesibilidad:** Incluir un elemento `<p className="sr-only">` debajo del grafico con texto descriptivo de los datos para lectores de pantalla.

**Directiva:** `"use client"` (Recharts requiere DOM)

---

### 3.8 Modificacion: PromoProgramaDetailClient

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx`

**Cambio:** Agregar una nueva pestana `<TabsTrigger value="metricas">Metricas</TabsTrigger>` y su `<TabsContent>` correspondiente al `<Tabs>` existente.

**Posicion en el TabsList:** Al final, antes del tab "Resumen" o despues de "Bloqueados" (el orden definitivo lo decide el implementador segun la logica del producto).

**TabsContent a agregar:**
```
<TabsContent value="metricas" className="mt-4">
    <ProgramaMetricasTab programaId={programa.id} />
</TabsContent>
```

**Import a agregar:**
```
import { ProgramaMetricasTab } from "@/components/crowdpromotion/metricas/ProgramaMetricasTab"
```

---

## 4. Hooks

### 4.1 useProgramaMetricas

**Archivo:** `src/admin/src/hooks/use-programa-metricas.ts`

**Tipo:** Query Hook (solo lectura)

**Responsabilidad:** Wrapper de `useQuery` para GET /api/crowdpromotion/programas/{id}/metricas. Incluye los filtros de fechas como parte de la query key para que TanStack Query refetch automaticamente cuando cambian. Devuelve los datos tipados como `ProgramaMetricasResponse`.

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string` | ID del programa a consultar |
| `filtro` | `FiltroFechas` | Objeto con `fechaDesde?` y `fechaHasta?` opcionales |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `ProgramaMetricasResponse \| undefined` | Response completo del endpoint |
| `isLoading` | `boolean` | True durante el fetch inicial |
| `isFetching` | `boolean` | True durante refetch (cambio de filtro) |
| `isError` | `boolean` | True si el fetch fallo |
| `error` | `Error \| null` | Objeto de error |
| `refetch` | `() => void` | Funcion para reintentar manualmente |

**Query Key:** `QUERY_KEYS.crowdpromotion.metricas.programa(programaId, filtro.fechaDesde, filtro.fechaHasta)`

**Configuracion:**
- `enabled: !!programaId` - no ejecutar si no hay programaId
- `staleTime: 0` - datos de metricas sin cache (RNF-07 del feature-spec)
- `retry: false` - no reintentar automaticamente (evitar multiples llamadas en errores 403)

**Imports:**
- `useQuery` de `@tanstack/react-query`
- `metricasService` de `@/services/metricas.service`
- `QUERY_KEYS` de `@shared/constants`
- `FiltroFechas`, `ProgramaMetricasResponse` de `@shared/types`

---

### 4.2 useSortableTable

**Archivo:** `src/admin/src/hooks/use-sortable-table.ts`

**Tipo:** Hook de estado local (sin server state)

**Responsabilidad:** Hook generico reutilizable para gestionar la ordenacion client-side de cualquier array de datos en una tabla. Gestiona la columna activa, la direccion (asc/desc) y aplica el ordenamiento al array de datos.

**Parametros:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `T[]` | Array de datos a ordenar (generico) |
| `defaultSortKey` | `keyof T` | Columna de ordenacion inicial |
| `defaultDirection` | `"asc" \| "desc"` | Direccion inicial (default: `"desc"`) |

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `sortedData` | `T[]` | Array de datos ya ordenado |
| `sortKey` | `keyof T` | Columna actualmente activa |
| `sortDirection` | `"asc" \| "desc"` | Direccion actual |
| `handleSort` | `(key: keyof T) => void` | Funcion a pasar al onClick de cabeceras de tabla |
| `getSortIcon` | `(key: keyof T) => "asc" \| "desc" \| "none"` | Retorna el estado del icono para cada columna |

**Logica interna:**
- Mantiene `sortKey` y `sortDirection` como estado local (`useState`)
- `sortedData` se calcula con `useMemo` para evitar re-renders innecesarios
- Al llamar `handleSort(key)`: si `key === sortKey`, alternar direccion; si no, cambiar a la nueva key con direction `"desc"` por defecto
- El ordenamiento de valores `null` o `undefined`: siempre al final, independientemente de la direccion

**Uso en `RankingPromotoresTable`:**
```typescript
const { sortedData, handleSort, getSortIcon } = useSortableTable<RankingPromotorItem>(
    items,
    "conversiones",
    "desc"
)
```

**Directiva:** Sin directiva (es un hook puro, sin dependencias de DOM)

---

## 5. Services

### 5.1 metricasService

**Archivo:** `src/admin/src/services/metricas.service.ts`

**Responsabilidad:** Clase de servicio para el endpoint GET /api/crowdpromotion/programas/{id}/metricas. Construye la URL con los query params de fecha opcionales, llama a `apiFetch`, verifica errores en `messages` y retorna el dato tipado.

**Patron:** Sigue exactamente el mismo patron que `tareasService` y `inscripcionService` del codebase existente: clase con metodos async, instancia exportada como singleton, uso de `apiFetch` + `API_ROUTES`.

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getProgramaMetricas` | `programaId: string, filtro?: FiltroFechas` | `Promise<ProgramaMetricasResponse>` | `GET /api/crowdpromotion/programas/{id}/metricas` |

**Detalle del metodo `getProgramaMetricas`:**
1. Construir `URLSearchParams` con `fechaDesde` y `fechaHasta` si estan definidos y no son strings vacios
2. Construir la URL base con `API_ROUTES.crowdpromotion.programaMetricas(programaId)`
3. Concatenar query string si hay params: `` `${baseUrl}${queryString ? `?${queryString}` : ""}` ``
4. Llamar `apiFetch<ServiceResponse<ProgramaMetricasResponse>>(url)` - usa el interceptor de axios que agrega el token JWT
5. Si `response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))`: extraer el errorCode y lanzar `new Error(errorMessage)` usando mensajes de `@shared/utils/error-messages`
6. Retornar `response.data`

**Imports:**
- `apiFetch` de `@/lib/api-client`
- `API_ROUTES` de `@shared/constants`
- `FiltroFechas`, `ProgramaMetricasResponse`, `ServiceResponse` de `@shared/types`

**Export:** `export const metricasService = new MetricasService()`

---

## 6. Flujo de Datos

```
Artista accede al tab "Metricas"
    |
    v
PromoProgramaDetailClient
    + TabsTrigger value="metricas" (shadcn Tabs)
    |
    v
ProgramaMetricasTab (Client Component)
    + Lee searchParams de la URL para filtro inicial
    + Instancia useProgramaMetricas(programaId, filtro)
    |
    v
useProgramaMetricas (Hook)
    + useQuery con QUERY_KEYS.crowdpromotion.metricas.programa(...)
    |
    v
metricasService.getProgramaMetricas(programaId, filtro) (Service)
    + Construye URL con query params
    + apiFetch con token JWT (interceptor de axios)
    |
    v
GET /api/crowdpromotion/programas/{id}/metricas?fechaDesde=&fechaHasta=
    |
    v (ServiceResponse<ProgramaMetricasResponse>)
    |
    v
ProgramaMetricasTab recibe data
    |
    |-- KpiCardsGrid (data.kpis)
    |
    |-- RankingPromotoresTable (data.rankingPromotores)
    |       + useSortableTable (ordenacion client-side)
    |
    |-- DesgloseEventosPanel (data.kpis - contadores)
    |
    |-- GraficoTemporal (data.eventosPorDia)

Artista cambia fechas y presiona "Aplicar":
    |
    v
FiltroFechas.onAplicar()
    |
    v
ProgramaMetricasTab actualiza query params en URL (router.push / useRouter)
    |
    v
useProgramaMetricas refetch automatico (query key cambio)
    |
    v
UI actualiza con nuevos datos
```

---

## 7. Dependencias de Shared

**Importar de `@shared/types`:**

| Tipo | Descripcion |
|------|-------------|
| `FiltroFechas` | Parametros de filtro `{ fechaDesde?: string; fechaHasta?: string }` |
| `ProgramaMetricasKpis` | Objeto con los 8 KPIs del programa |
| `RankingPromotorItem` | Item del ranking con clicks, conversiones, valor y comision |
| `EventosPorDiaItem` | Punto de la serie temporal `{ fecha, clicks, pageViews, signups, conversiones }` |
| `ProgramaMetricasResponse` | Response completo del endpoint de metricas |

**Importar de `@shared/constants`:**

| Constante | Uso |
|-----------|-----|
| `API_ROUTES.crowdpromotion.programaMetricas` | Funcion `(programaId: string) => string` para construir la URL |
| `QUERY_KEYS.crowdpromotion.metricas.programa` | Funcion query key con programaId + fechas |

**Importar de `@shared/utils`:**

| Funcion | Uso |
|---------|-----|
| `formatTasaConversion` | Formatea `number` como `"1.11%"` para KPI de tasa |

**Nota:** Todos estos tipos, constantes y utilidades son nuevos en shared (definidos en el `contracts-plan.md` de shared). El implementador de admin debe verificar que el plan de shared este implementado antes de implementar este plan.

---

## 8. Integracion con la Estructura Admin Existente

### Routing

No se crea una nueva ruta. La pagina existente en:
```
src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/page.tsx
```
renderiza `<PromoProgramaDetailClient programaId={id} />`. Solo se modifica `PromoProgramaDetailClient` para agregar el tab "Metricas".

### Autenticacion

El endpoint GET /metricas requiere autenticacion como artista. El interceptor de axios en `api-client.ts` ya agrega el header `Authorization: Bearer {token}` a todas las requests. No se requiere logica adicional.

### Sidebar y navegacion

El sidebar existente ya tiene el item "CrowdPromotion" que navega a `/crowdpromotion/programas`. La pagina de detalle del programa ya esta accesible desde ahi. No se requieren cambios en el sidebar.

### Breadcrumb

La pantalla no tiene un breadcrumb explicito todavia (el `PromoProgramaDetailClient` muestra el nombre del programa como titulo). Si se desea agregar breadcrumb completo (como muestra el ui-ux.md), se puede agregar como elemento opcional dentro de `ProgramaMetricasTab`. En ese caso:

```
<nav aria-label="breadcrumb" className="flex items-center gap-1.5 text-xs text-[#64748b] mb-4">
    <Link href="/crowdpromotion/programas">Programas</Link>
    <ChevronRight className="w-3 h-3 text-[#334155]" />
    <Link href={`/crowdpromotion/programas/${programaId}`}>{programaTitulo}</Link>
    <ChevronRight className="w-3 h-3 text-[#334155]" />
    <span className="text-[#94a3b8]">Metricas</span>
</nav>
```

El `programaTitulo` se obtiene de `data?.programaTitulo` (incluido en `ProgramaMetricasResponse`).

### Libreria Recharts

Verificar si `recharts` esta instalado en `src/admin/package.json`. Si no lo esta, agregarlo:
```
npm install recharts --prefix src/admin
```

---

## 9. Archivos a Crear o Modificar

| Archivo | Accion | Descripcion |
|---------|--------|-------------|
| `src/admin/src/components/crowdpromotion/metricas/ProgramaMetricasTab.tsx` | CREAR | Orquestador del tab, filtro de fechas y estados de UI |
| `src/admin/src/components/crowdpromotion/metricas/FiltroFechas.tsx` | CREAR | Formulario de rango de fechas con validacion inline |
| `src/admin/src/components/crowdpromotion/metricas/KpiCardsGrid.tsx` | CREAR | Grid de 4 KPI primarias + 2 secundarias |
| `src/admin/src/components/crowdpromotion/metricas/KpiCard.tsx` | CREAR | Card KPI individual reutilizable |
| `src/admin/src/components/crowdpromotion/metricas/RankingPromotoresTable.tsx` | CREAR | Tabla de ranking con ordenacion |
| `src/admin/src/components/crowdpromotion/metricas/DesgloseEventosPanel.tsx` | CREAR | Panel de desglose con barras de progreso |
| `src/admin/src/components/crowdpromotion/metricas/GraficoTemporal.tsx` | CREAR | LineChart Recharts con 3 series |
| `src/admin/src/hooks/use-programa-metricas.ts` | CREAR | Query hook con filtro de fechas |
| `src/admin/src/hooks/use-sortable-table.ts` | CREAR | Hook generico de ordenacion client-side |
| `src/admin/src/services/metricas.service.ts` | CREAR | Service GET /programas/{id}/metricas |
| `src/admin/src/__mocks__/cp-tracking-metricas.mock.ts` | CREAR | Mocks para tests de hooks y servicios |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx` | MODIFICAR | Agregar TabsTrigger + TabsContent "metricas" |
| `src/admin/src/hooks/index.ts` | MODIFICAR | Exportar `useProgramaMetricas` y `useSortableTable` |
| `src/admin/src/services/index.ts` | MODIFICAR | Exportar `metricasService` |

---

## 10. Consideraciones de Implementacion

### Skeleton de carga

El estado de loading de `ProgramaMetricasTab` debe mostrar skeletons para cada seccion:
- 4 `<Skeleton className="h-24 bg-[#1e1e38] rounded-xl animate-pulse" />` para KPI primarias
- 2 `<Skeleton className="h-16 bg-[#1e1e38] rounded-xl animate-pulse" />` para KPI secundarias
- `<Skeleton className="h-64 bg-[#1e1e38] rounded-lg animate-pulse" />` para la tabla de ranking y el panel de desglose
- `<Skeleton className="h-64 bg-[#1e1e38] rounded-lg animate-pulse" />` para el grafico

### Estado de carga con filtro aplicado

Cuando hay un refetch por cambio de filtro (`isFetching === true` pero `data !== undefined`), mostrar los datos anteriores con una capa de `opacity-60 pointer-events-none` encima y un spinner `<Loader2>` centrado sobre cada seccion. El boton "Aplicar" muestra `<Loader2 w-4 h-4 animate-spin>` mientras `isFetching`.

### Colores del design system

Todos los colores CSS custom del design system (`#0d0d1a`, `#1a1a2e`, `#151525`, etc.) se aplican directamente como clases Tailwind con notacion bracket `bg-[#151525]`. No se crean variables CSS adicionales. Los tokens del design system de ui-ux.md son la fuente de verdad.

### Valores monetarios

Para mostrar valores en EUR en la tabla y KPIs:
- Usar `(valor).toLocaleString('es-ES', { minimumFractionDigits: 0, maximumFractionDigits: 2 })` seguido del sufijo `monedaNombre` del response (normalmente "EUR")
- Si `monedaNombre` es `null`, mostrar solo el numero sin sufijo

### PageViews en el grafico

Segun ui-ux.md: "PageViews no se muestra en el grafico por defecto (demasiado volumen vs los otros tipos)". El campo `pageViews` existe en `EventosPorDiaItem` pero `GraficoTemporal` solo renderiza 3 `<Line>`: clicks, signups, conversiones. No se agrega toggle de visibilidad en MVP.

---

## 11. Checklist

### Componentes
- [ ] `ProgramaMetricasTab` usa `"use client"` y gestiona filtro via URL query params
- [ ] `FiltroFechas` valida que fechaDesde <= fechaHasta antes de habilitar boton "Aplicar"
- [ ] `FiltroFechas` limita `fechaHasta` a hoy con `max={new Date().toISOString().split('T')[0]}`
- [ ] `KpiCardsGrid` renderiza 4 KPI primarias en grid-cols-4 y 2 secundarias en grid-cols-2
- [ ] `KpiCard` soporta variantes numerica y monetaria (con sufijo EUR y color amber)
- [ ] `RankingPromotoresTable` tiene `<thead>` con `<th scope="col">` y `aria-sort` en columnas ordenables
- [ ] `RankingPromotoresTable` muestra `Avatar` con fallback de inicial del nombre del promotor
- [ ] `RankingPromotoresTable` muestra empty state cuando `items.length === 0`
- [ ] `DesgloseEventosPanel` calcula porcentajes relativos al total de todos los eventos
- [ ] `DesgloseEventosPanel` anima las barras de progreso con `transition-[width] duration-500 ease-out`
- [ ] `GraficoTemporal` muestra empty state cuando `datos.length === 0`
- [ ] `GraficoTemporal` tiene `<p className="sr-only">` con alternativa textual

### Hooks
- [ ] `useProgramaMetricas` tiene `staleTime: 0` (sin cache, RNF-07)
- [ ] `useProgramaMetricas` tiene `retry: false` para evitar multiples llamadas en 403
- [ ] `useProgramaMetricas` tiene `enabled: !!programaId`
- [ ] `useSortableTable` ordena valores `null` al final independientemente de la direccion
- [ ] `useSortableTable` usa `useMemo` para la lista ordenada

### Services
- [ ] `metricasService.getProgramaMetricas` solo agrega params de fecha si no son strings vacios
- [ ] `metricasService` verifica errores en `messages` antes de retornar `data`
- [ ] `metricasService` usa `apiFetch` (no fetch directo) para que el interceptor agregue el token JWT

### Integracion
- [ ] `PromoProgramaDetailClient` importa y renderiza `ProgramaMetricasTab` con `programaId`
- [ ] El nuevo `<TabsTrigger value="metricas">` se agrega en el `<TabsList>` existente
- [ ] `recharts` verificado o instalado en `src/admin/package.json`
- [ ] Tipos importados de `@shared/types` (no duplicados en admin)
- [ ] Constants importados de `@shared/constants`
- [ ] `use-programa-metricas` y `use-sortable-table` exportados desde `src/admin/src/hooks/index.ts`
- [ ] `metricasService` exportado desde `src/admin/src/services/index.ts`

### Estados de UI
- [ ] Loading inicial: todos los paneles muestran skeletons con `animate-pulse`
- [ ] Refetch (filtro aplicado): datos anteriores con overlay + spinner en boton "Aplicar"
- [ ] Error: card centrado con `AlertCircle` y boton "Reintentar" que llama `refetch()`
- [ ] Empty state (0 datos): KPIs en 0, tabla con mensaje "Sin promotores con actividad", grafico con texto "No hay datos para el periodo seleccionado"
- [ ] Badge de filtro activo visible cuando hay fechaDesde o fechaHasta en URL params
- [ ] Boton "Limpiar" visible solo cuando `tieneFiltroPeriodo === true`

### Accesibilidad
- [ ] Focus rings en todos los elementos interactivos: `focus-visible:ring-2 focus-visible:ring-[#a855f7]`
- [ ] Columnas ordenables de la tabla tienen `aria-sort="ascending"/"descending"/"none"`
- [ ] Grafico temporal tiene alternativa textual con `<p className="sr-only">`
- [ ] Card de error tiene `role="alert"` para lectores de pantalla
- [ ] Skeletons tienen `aria-busy="true"` en el contenedor padre

### Responsive
- [ ] KPI primarias: `grid-cols-2 lg:grid-cols-4`
- [ ] KPI secundarias: `grid-cols-1 sm:grid-cols-2`
- [ ] Ranking + desglose: `grid-cols-1 lg:grid-cols-5` (ranking col-span-3, desglose col-span-2)
- [ ] En mobile, la tabla de ranking oculta columnas Valor y Comision (visibles solo en md+)
