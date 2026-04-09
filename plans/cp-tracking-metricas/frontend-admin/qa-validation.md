# Validacion QA: cp-tracking-metricas (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/admin (Next.js 14, puerto 3001)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (Admin-scope) | 18 |
| Cubiertos completamente | 14 |
| Parcialmente Cubiertos | 3 |
| No Cubiertos | 1 |
| **Score de Cobertura** | **83%** |

**Estado:** REQUIERE CAMBIOS

Los planes `frontend-plan.md` y `ui-design.md` son de alta calidad y cubren correctamente el nucleo funcional del dashboard del artista. Los gaps identificados son menores o mayores, sin brechas criticas que impidan la implementacion. Sin embargo, hay areas de cobertura parcial que deben resolverse antes de considerar la implementacion lista.

---

## 2. Criterios de Aceptacion (Scope Admin)

### Fuente: feature-spec.md - Criterios aplicables al Admin

| ID | Criterio | Tipo | Proyectos |
|----|----------|------|-----------|
| AC-CP05-9 | El artista propietario del programa puede ver el dashboard de metricas con los KPIs: totalClicks, totalPageViews, totalSignups, totalConversiones, valorTotalGenerado, tasaConversion y comisionesTotales | Funcional | Backend + Admin |
| AC-CP05-10 | El dashboard del artista soporta filtrado por rango de fechas (fechaDesde y fechaHasta); al cambiar el rango los KPIs y el ranking se recalculan correctamente | Funcional | Backend + Admin |
| AC-CP05-11 | El dashboard del artista muestra el ranking de promotores con: nombre del promotor, tipo de promotor, clicks generados, conversiones, valor generado y comision acumulada, ordenado por conversiones descendente | Funcional | Backend + Admin |
| AC-CP05-15 | Los tipos TypeScript para PromoEventoDto, MetricasProgramaDto, RankingPromotorDto, MetricasPromotorDto y los schemas Zod estan definidos en shared y son reutilizados por Admin | Shared + Admin |

### Criterios de UI/UX derivados de ui-ux.md Pantalla 2 (Admin)

| ID | Criterio | Tipo |
|----|----------|------|
| UI-01 | Breadcrumb completo: Campanias > Campana > CrowdPromotion > Metricas | UI/UX |
| UI-02 | Filtro de fechas con validacion inline (fechaDesde <= fechaHasta, max=today) | UI/UX |
| UI-03 | 4 KPI cards primarias (Clicks, Signups, Backings, Valor) | UI/UX |
| UI-04 | 2 KPI cards secundarias (Tasa conversion, Comisiones) | UI/UX |
| UI-05 | Tabla ranking con 6 columnas, avatar con fallback, badge tipo promotor | UI/UX |
| UI-06 | Ordenacion de tabla client-side (clicks, conv, valor, comision) con aria-sort | Accesibilidad |
| UI-07 | Panel de desglose por tipo de evento con barras de progreso animadas y role="progressbar" | UI/UX + Accesibilidad |
| UI-08 | Grafico temporal Recharts con 3 series (clicks, signups, conversiones) | UI/UX |
| UI-09 | Estado loading con skeletons para cada seccion (animate-pulse) | UI/UX |
| UI-10 | Estado loading con filtro: datos previos visibles con overlay + spinner en boton Aplicar | UI/UX |
| UI-11 | Estado error con card AlertCircle, mensaje y boton Reintentar (role="alert") | UI/UX + Accesibilidad |
| UI-12 | Empty state: KPIs en 0, tabla con mensaje, grafico con texto "No hay datos" | UI/UX |
| UI-13 | Badge de filtro activo con boton para limpiar filtro | UI/UX |
| UI-14 | Responsive: 2 cols tablet, 4 cols desktop; tabla oculta columnas en mobile | Responsive |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales del feature-spec.md

| ID | Criterio | frontend-plan.md | ui-design.md | contracts-plan.md | Estado |
|----|----------|-----------------|--------------|-------------------|--------|
| AC-CP05-9 | Dashboard artista con KPIs: clicks, pageviews, signups, conversiones, valorTotal, tasaConversion, comisionesTotales | `KpiCardsGrid` + `KpiCard` + `useProgramaMetricas` + `metricasService.getProgramaMetricas` | KpiCardsGrid con 4 primarias + 2 secundarias; todos los 7 KPIs mapeados | `ProgramaMetricasKpis` con 8 campos (7 KPIs + monedaNombre); `API_ROUTES.crowdpromotion.programaMetricas` | CUBIERTO |
| AC-CP05-10 | Filtro por rango de fechas; KPIs y ranking se recalculan | `FiltroFechas` + `useProgramaMetricas(programaId, filtro)` con `staleTime:0`; query key incluye fechas para refetch automatico | `FiltroFechas` con validacion Zod `filtroFechasSchema`; badge de filtro activo; boton Limpiar | `filtroFechasSchema` con refinamiento `fechaDesde <= fechaHasta`; `QUERY_KEYS.crowdpromotion.metricas.programa(id, desde, hasta)` | CUBIERTO |
| AC-CP05-11 | Ranking de promotores: nombre, tipo, clicks, conversiones, valor, comision; ordenado por conversiones desc | `RankingPromotoresTable` + `useSortableTable` con `defaultSortKey="conversiones"` y `defaultDirection="desc"` | Tabla con 6 columnas; avatar fallback inicial; badge tipo promotor; ordenacion con aria-sort | `RankingPromotorItem` con 9 campos (promotorId, promotorNombre, tipoPromotorNombre, clicks, pageViews, signups, conversiones, valorGenerado, comisionAcumulada) | CUBIERTO |
| AC-CP05-15 | Tipos TypeScript en shared reutilizados por Admin | Plan importa `FiltroFechas`, `ProgramaMetricasKpis`, `RankingPromotorItem`, `EventosPorDiaItem`, `ProgramaMetricasResponse` de `@shared/types`; constantes de `@shared/constants`; `formatTasaConversion` de `@shared/utils` | Referencia a `ProgramaMetricasKpis` y `RankingPromotorItem` de shared en cada componente | Todos los tipos definidos en `src/shared/types/crowdpromotion.ts`; schemas en `crowdpromotion.schema.ts` | CUBIERTO |

### 3.2 Requisitos de UI/UX (Pantalla 2)

| ID | Criterio | frontend-plan.md | ui-design.md | Estado |
|----|----------|-----------------|--------------|--------|
| UI-01 | Breadcrumb completo | Mencionado como opcional en seccion 8; breadcrumb con `programaTitulo` de `data?.programaTitulo` | `BreadcrumbMetricasProps` con campaniaId, campaniaTitulo, programaId, programaTitulo; markup completo con `aria-label` y `aria-current="page"` | PARCIAL |
| UI-02 | Filtro de fechas con validacion | `FiltroFechas` con validacion `fechaDesde > fechaHasta` inline; `max={hoy}` en fechaHasta | Markup completo con `filtroFechasSchema`; error `role="alert"`; aviso informativo 365 dias | CUBIERTO |
| UI-03 | 4 KPI cards primarias | `KpiCardsGrid` renderiza 4 primarias en `grid-cols-2 lg:grid-cols-4` | Markup con 4 cards: Clicks (azul), Signups (verde), Backings (purpura), Valor (ambar) | CUBIERTO |
| UI-04 | 2 KPI cards secundarias | `KpiCardsGrid` renderiza 2 secundarias en `grid-cols-1 sm:grid-cols-2` | Markup con Tasa conversion (horizontal layout) + Comisiones (horizontal layout) | CUBIERTO |
| UI-05 | Tabla ranking con avatar, badge tipo | `RankingPromotoresTable` con Avatar+fallback+badge | Markup completo con `AvatarFallback` usando iniciales; `Badge` para tipo; 6 columnas | CUBIERTO |
| UI-06 | Ordenacion tabla con aria-sort | `useSortableTable` generico + `RankingPromotoresTable` con `aria-sort` | `aria-sort="ascending"/"descending"/"none"` en cada cabecera ordenable; `onSort` callback | CUBIERTO |
| UI-07 | Panel desglose con barras progreso animadas y role="progressbar" | `DesgloseEventosPanel` con calculo de porcentajes; `transition-[width] duration-500 ease-out` | `role="progressbar"` + `aria-valuenow/min/max/label` en cada barra; 4 tipos (sin Shares en MVP) | CUBIERTO |
| UI-08 | Grafico Recharts 3 series | `GraficoTemporal` con `LineChart`, 3 `<Line>` (clicks, signups, conversiones); nota sobre PageViews excluidos | `ResponsiveContainer + LineChart` completo; tooltip dark; leyenda; empty state con texto; `<p className="sr-only">` alternativa textual | CUBIERTO |
| UI-09 | Loading skeletons por seccion | Plan seccion 10 detalla skeletons: 4+2 KPI, tabla 5 filas, grafico `h-64`; `animate-pulse` | Markup de skeleton para cada seccion con `aria-busy="true"`; `MetricasLoadingSkeleton` como componente separado en ui-design | CUBIERTO |
| UI-10 | Loading con filtro: datos previos + overlay + spinner | Plan seccion 10 menciona `isFetching === true` con `opacity-60 pointer-events-none` + spinner; boton Aplicar con Loader2 | `FiltroFechas` props: `isLoading` controla spinner en boton Aplicar; diferencia entre `isLoading` e `isFetching` documentada | PARCIAL |
| UI-11 | Estado error con role="alert" y boton Reintentar | `ProgramaMetricasTab` renderiza card de error con `AlertCircle` + boton que llama `refetch()`; plan checklist indica `role="alert"` | No hay markup explicito del card de error en ui-design (solo mencionado en resumen de componentes) | PARCIAL |
| UI-12 | Empty state: 0 en KPIs, tabla con mensaje, grafico con texto | `ProgramaMetricasTab` renderiza componentes normales con valores 0; `RankingPromotoresTable` con empty row colspan=6; `GraficoTemporal` con texto centrado | `MetricasEmptyState` listado como componente; tabla empty `colspan={6}`; grafico empty con texto | CUBIERTO |
| UI-13 | Badge filtro activo con boton limpiar | `FiltroFechas` con prop `tieneFiltroPeriodo`; boton "Limpiar" visible condicionalmente | Badge `bg-purple-950/50 text-[#a855f7]` con `<X>` para limpiar filtro; rango formateado visible | CUBIERTO |
| UI-14 | Responsive: 2 cols tablet, 4 cols desktop; tabla oculta cols mobile | Plan checklist: `grid-cols-2 lg:grid-cols-4`; tabla oculta Valor y Comision en mobile; grafico `h-48` en mobile | `hidden md:table-cell` en columnas Valor y Comision; `grid-cols-1 lg:grid-cols-5` para ranking+desglose | CUBIERTO |

### 3.3 Requisitos No Funcionales (Scope Admin)

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| RNF-07 | Dashboards muestran datos actualizados; no cache de consulta | `useProgramaMetricas` tiene `staleTime: 0` (lectura directa); `retry: false` para evitar loops en 403 | CUBIERTO |
| RN-08 | El artista solo puede ver metricas de sus propios programas (403 si no es propietario) | `metricasService` verifica errores en `messages` antes de retornar; `retry: false` evita loops en 403; error state en `ProgramaMetricasTab` | CUBIERTO |
| RNF-04 | Endpoint GET /metricas responde < 800ms para 10.000 eventos | Responsabilidad del backend; Admin consume el dato ya calculado. No hay logica de performance en el plan Admin. | N/A (Backend) |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

No se identifican gaps criticos. Todos los requisitos funcionales principales (AC-CP05-9, AC-CP05-10, AC-CP05-11, AC-CP05-15) tienen cobertura completa en los planes.

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-01 | UI-01: Breadcrumb completo | El `frontend-plan.md` trata el breadcrumb como **opcional** ("Si se desea agregar...") en la seccion 8. Sin embargo, `ui-design.md` lo define con markup completo y props interface `BreadcrumbMetricasProps` (con campaniaId, campaniaTitulo). Hay inconsistencia entre planes. Ademas, `ProgramaMetricasTab` recibe solo `programaId` como prop, no `campaniaId` ni `campaniaTitulo`, por lo que el breadcrumb completo no podria renderizarse con la API de props definida. | Medio | Aclarar en `frontend-plan.md` que el breadcrumb es **obligatorio** segun ui-design. Agregar `campaniaId?: string` y `campaniaTitulo?: string` como props opcionales a `ProgramaMetricasTab`, o alternativamente extraer `campaniaId` de la ruta con `useParams()`. Verificar que `PromoProgramaDetailClient` pase `campaniaId` al tab. |
| GAP-02 | UI-10: Estado de loading con filtro aplicado | El `frontend-plan.md` describe el comportamiento (`opacity-60 pointer-events-none` + spinner), pero el detalle de implementacion es ambiguo: no queda claro si el overlay afecta a **cada seccion por separado** (4 secciones independientes) o a **todo el tab** como un bloque unico. `ui-design.md` no tiene markup para este estado especifico mas alla del boton Aplicar. | Medio | Agregar en `ui-design.md` el markup del estado de carga con filtro (isFetching). Definir si el overlay es por seccion o global. Sugerencia: overlay global sobre el area de contenido del tab (bajo el FiltroFechas) con `relative` + `absolute inset-0 bg-[#1a1a2e]/60 backdrop-blur-[1px] z-10` + spinner centrado, es mas simple y menos propenso a errores. |
| GAP-03 | UI-11: Markup del card de error | `frontend-plan.md` describe el comportamiento del error state pero `ui-design.md` solo lo menciona en el resumen de componentes (`MetricasEmptyState`) sin proveer el markup HTML/JSX del card de error con `role="alert"`. El checklist del plan tiene el item `role="alert"` pero no hay guia de implementacion concreta. | Medio | Agregar en `ui-design.md` el markup del card de error, siguiendo el patron: `<div role="alert" className="flex flex-col items-center justify-center py-16 gap-4"><AlertCircle ... /><p ...>No se pudieron cargar las metricas</p><Button variant="outline" size="sm" onClick={refetch}>Reintentar</Button></div>`. |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-04 | Recharts: instalacion y SSR | El `frontend-plan.md` menciona verificar si Recharts esta instalado pero no documenta la solucion para **Server-Side Rendering** en Next.js 14. Recharts usa el DOM y no puede renderizarse en el servidor. `GraficoTemporal` tiene `"use client"` correcto, pero no hay mencion de `dynamic import` como fallback para evitar hydration errors si el componente se usa en un contexto SSR. | Bajo | Agregar nota en `frontend-plan.md`: "Si se producen hydration errors con Recharts, usar `const GraficoTemporal = dynamic(() => import('./GraficoTemporal'), { ssr: false, loading: () => <Skeleton className='h-64' /> })` en `ProgramaMetricasTab`." |
| GAP-05 | KpiCardsGrid: prop isLoading ausente en frontend-plan | `ui-design.md` define `KpiCardsGridProps` con `kpis: ProgramaMetricasKpis` **y** `isLoading: boolean`. El `frontend-plan.md` define `KpiCardsGrid` con solo `kpis: ProgramaMetricasKpis` como prop (sin `isLoading`). La logica de skeleton de KPI cards esta en `ProgramaMetricasTab` segun el plan, no en `KpiCardsGrid`. Hay una discrepancia en la API de props. | Bajo | Alinear las props. Si el skeleton se gestiona en `ProgramaMetricasTab` (mas simple), actualizar `ui-design.md` para eliminar `isLoading` de `KpiCardsGridProps`. Si el skeleton se delega a `KpiCardsGrid`, actualizar `frontend-plan.md`. Recomendacion: skeleton en el padre (`ProgramaMetricasTab`) es mas consistente con el patron de los otros componentes. |
| GAP-06 | DesgloseEventosPanel: Shares excluidos sin documentar alternativa | El plan excluye los Shares del panel de desglose en MVP ("Shares no se muestra por separado en MVP"). Sin embargo, el tipo `ProgramaMetricasKpis` en shared **no incluye** un campo `totalShares`. Si el backend en el futuro devuelve ese dato, el panel no lo mostraria. La nota de exclusion solo aparece en el comentario del `DesgloseEventosPanel` en `ui-design.md`. | Bajo | Documentar en el plan que `ProgramaMetricasKpis` en shared podria necesitar `totalShares?: number` en el futuro. Por ahora la exclusion es correcta segun el scope MVP pero conviene dejarlo anotado como deuda tecnica. |
| GAP-07 | Test strategy ausente en planes Admin | No existe un archivo `test-strategy.md` para el plan Admin de esta feature. El `frontend-plan.md` incluye un archivo `cp-tracking-metricas.mock.ts` en `__mocks__/` pero no define que tests se planifican. | Bajo | Crear `plans/cp-tracking-metricas/frontend-admin/test-strategy.md` con al menos: tests unitarios para `useSortableTable`, tests de integracion para `useProgramaMetricas` con mock del service, y tests de componente para `FiltroFechas` (validacion). |
| GAP-08 | Accesibilidad: `aria-busy` en contenedor de skeletons | El checklist del `frontend-plan.md` indica `aria-busy="true"` en el contenedor padre durante loading. `ui-design.md` lo implementa en `KpiCardsGrid` con `aria-busy={isLoading}`. No queda claro quien es el "contenedor padre" cuando `isLoading` esta en `ProgramaMetricasTab`: si `KpiCardsGrid` no recibe `isLoading` (ver GAP-05), el `aria-busy` no podria propagarse. | Bajo | Resolver junto con GAP-05. Si el skeleton esta en el padre, el contenedor con `aria-busy` debe ser el wrapper del tab completo o de cada seccion. |
| GAP-09 | Alternativa textual del grafico Recharts | `frontend-plan.md` indica `<p className="sr-only">` con texto descriptivo. `ui-design.md` no muestra el contenido exacto de ese texto. Para ser util a lectores de pantalla, el texto debe incluir el resumen de los datos (totales del periodo visible). | Bajo | Definir el texto de la alternativa: `"Grafico de lineas mostrando {totalClicks} clicks, {totalSignups} registros y {totalConversiones} conversiones para el periodo seleccionado."` Usar los datos de `kpis` para generar este texto dinamicamente. |

---

## 5. Validacion de Tests

No existe un `test-strategy.md` para el plan Admin de esta feature. La validacion de tests se basa en lo que describe `frontend-plan.md` (mock file) y las expectativas del feature-spec.

### Cobertura de Criterios en Tests (Planificada implicitamente)

| Criterio | Test Planificado | Tipo | Estado |
|----------|-----------------|------|--------|
| AC-CP05-9 (KPIs) | Mock en `cp-tracking-metricas.mock.ts`; render de `KpiCardsGrid` con mock data | Unit/Component | PARCIAL (mock definido, test no especificado) |
| AC-CP05-10 (Filtro fechas) | Validacion de `filtroFechasSchema` desde shared; comportamiento de `useProgramaMetricas` con filtro | Unit + Integration | PARCIAL (schema existe, test de hook no especificado) |
| AC-CP05-11 (Ranking) | `useSortableTable` con datos mock; render de `RankingPromotoresTable` | Unit + Component | PARCIAL (mock definido, test no especificado) |
| UI-06 (aria-sort) | Test de accesibilidad con `@testing-library/react` comprobando `aria-sort` | Component | NO CUBIERTO |
| UI-07 (progressbar) | Test de accesibilidad verificando `role="progressbar"` | Component | NO CUBIERTO |
| UI-08 (Recharts) | Snapshot o render test del grafico con datos mock | Component | NO CUBIERTO |
| UI-11 (Error state) | Test de manejo de error en `useProgramaMetricas` con mock de error | Unit | NO CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Razon |
|----------|---------------|-------|
| useSortableTable | `use-sortable-table.test.ts` - ordenacion ASC/DESC, alternancia, valores null al final | Hook generico critico, alta probabilidad de bugs en edge cases |
| FiltroFechas validacion | `FiltroFechas.test.tsx` - fechaDesde > fechaHasta deshabilita boton; max=hoy en fechaHasta | AC-CP05-10 requiere validacion de fechas |
| RankingPromotoresTable | `RankingPromotoresTable.test.tsx` - aria-sort presente; empty state; ordenacion visual | AC-CP05-11 + accesibilidad WCAG |
| useProgramaMetricas error | Test de error 403 con `retry: false` verificando que no hay loop | RN-08 (acceso denegado a programas ajenos) |

---

## 6. Validacion de UI/UX

### Screens Planificadas vs Requeridas

| Screen Requerida (ui-ux.md) | Planificada | Componentes | Estado |
|----------------------------|-------------|-------------|--------|
| Pestana Metricas en detalle de programa | Si (tab en PromoProgramaDetailClient) | `ProgramaMetricasTab` | CUBIERTO |
| Layout con sidebar de dashboard | Si (hereda de PromoProgramaDetailClient existente) | Sidebar existente | CUBIERTO |
| Breadcrumb completo | Si (con gap de props) | Inline en ProgramaMetricasTab | PARCIAL |
| Filtro de fechas con badge activo | Si | `FiltroFechas` | CUBIERTO |
| KPI cards (4 primarias + 2 secundarias) | Si | `KpiCardsGrid` + `KpiCard` | CUBIERTO |
| Tabla ranking con ordenacion | Si | `RankingPromotoresTable` | CUBIERTO |
| Panel desglose eventos | Si | `DesgloseEventosPanel` | CUBIERTO |
| Grafico temporal Recharts | Si | `GraficoTemporal` | CUBIERTO |

### Estados de UI

| Estado | Requerido (ui-ux.md) | Planificado | Estado |
|--------|---------------------|-------------|--------|
| Loading inicial (skeletons) | Si | Si (4+2 KPI, tabla, grafico) | CUBIERTO |
| Loading con filtro (overlay + spinner) | Si | Si (opacity-60, Loader2 en boton) | PARCIAL (markup no definido en ui-design) |
| Default con datos | Si | Si | CUBIERTO |
| Empty state (0 datos en periodo) | Si | Si (MetricasEmptyState, empty tabla, empty grafico) | CUBIERTO |
| Error de carga | Si | Si (AlertCircle + Reintentar) | PARCIAL (markup no en ui-design) |
| Filtro aplicado (badge activo) | Si | Si (badge con rango formateado + X) | CUBIERTO |
| Ordenacion de tabla activa | Si | Si (cabecera en purple, icono visible) | CUBIERTO |

### Validacion del Filtro de Fechas

| Campo | Validacion Requerida | Planificada | Estado |
|-------|---------------------|-------------|--------|
| fechaDesde > fechaHasta | Deshabilitar boton Aplicar + mensaje error inline | Si (`FiltroFechas` con `errorFechas` + `filtroFechasSchema`) | CUBIERTO |
| fechaHasta en el futuro | max=hoy en el input | Si (`max={new Date().toISOString().split('T')[0]}`) | CUBIERTO |
| Rango > 365 dias | Aviso informativo (no bloquea) | Si (aviso informativo mencionado en plan y ui-design) | CUBIERTO |

### Accesibilidad

| Requisito WCAG | Implementacion Planificada | Estado |
|----------------|---------------------------|--------|
| Contraste de texto (4.5:1 minimo) | Design tokens con colores definidos en ui-ux.md; valores en blanco sobre #151525 | CUBIERTO |
| Focus ring en interactivos | `focus-visible:ring-2 focus-visible:ring-[#a855f7]` en checklist del plan | CUBIERTO |
| Labels para inputs | `htmlFor="fechaDesde"`, `aria-label="Fecha de inicio"` en markup | CUBIERTO |
| aria-sort en tabla | `aria-sort="ascending"/"descending"/"none"` en columnas ordenables | CUBIERTO |
| scope="col" en th | `<TableHead scope="col">` en todas las cabeceras | CUBIERTO |
| progressbar role | `role="progressbar"` + `aria-valuenow/min/max/label` en barras | CUBIERTO |
| Alternativa textual grafico | `<p className="sr-only">` en GraficoTemporal | PARCIAL (contenido del texto no definido) |
| aria-busy en skeletons | Mencionado en checklist; implementado en KpiCardsGrid | PARCIAL (depende de resolucion de GAP-05/GAP-08) |
| role="alert" en errores | Mencionado en checklist del plan | PARCIAL (markup no en ui-design, ver GAP-03) |
| Breadcrumb: aria-label, aria-current | `aria-label="Breadcrumb"`, `aria-current="page"` en ui-design | CUBIERTO (si breadcrumb se implementa obligatorio) |

### Responsive

| Breakpoint | Comportamiento Requerido | Planificado | Estado |
|------------|--------------------------|-------------|--------|
| Mobile < 768px | Sidebar colapsado; KPI en 2 cols; ranking y desglose apilados; grafico h-48; tabla sin Valor/Comision | `grid-cols-2 lg:grid-cols-4`; `hidden md:table-cell`; sidebar hereda comportamiento existente | CUBIERTO |
| Tablet 768-1024px | Sidebar visible; KPI 2+2; ranking+desglose en grid-cols-2 | `lg:grid-cols-5` vs `grid-cols-2` en tablet | CUBIERTO |
| Desktop > 1024px | Layout completo 4 KPI + 3+2 ranking+desglose | `lg:grid-cols-4` y `lg:grid-cols-5` | CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Mayor)

1. **Resolver inconsistencia de Breadcrumb entre frontend-plan y ui-design (GAP-01)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/frontend-plan.md`
   - Cambio: Mover el breadcrumb de "opcional" a "obligatorio". Agregar `campaniaId?: string` y `campaniaTitulo?: string` a los props de `ProgramaMetricasTab`. Actualizar la seccion 3.1 (Props de ProgramaMetricasTab) y la seccion 9 (Archivos a crear/modificar) para reflejar que `PromoProgramaDetailClient` debe pasar el `campaniaId` de la campana al tab.

2. **Definir markup del estado de loading con filtro (GAP-02)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/ui-design.md`
   - Cambio: Agregar en la seccion del componente `ProgramaMetricasTab` (o en una nueva seccion de estados globales) el markup del overlay de loading durante refetch: contenedor relativo sobre el area de KPIs+Ranking+Grafico con capa semi-transparente y spinner centrado. Definir si es overlay global o por seccion.

3. **Agregar markup del card de error en ui-design (GAP-03)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/ui-design.md`
   - Cambio: Agregar la composicion JSX del estado de error con `role="alert"`, icono `AlertCircle`, texto "No se pudieron cargar las metricas", y boton "Reintentar". Asegurar que el patron es consistente con otros estados de error del codebase admin.

### Acciones Sugeridas (Menor)

4. **Alinear prop isLoading en KpiCardsGrid entre los dos planes (GAP-05)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/frontend-plan.md` o `ui-design.md`
   - Cambio: Decidir si el skeleton de KPI se gestiona en `ProgramaMetricasTab` (recomendado) o en `KpiCardsGrid`. Actualizar la API de props de `KpiCardsGrid` en el plan que quede desalineado. Si el skeleton va en el padre, eliminar `isLoading` de `KpiCardsGridProps` en ui-design.

5. **Agregar nota de SSR para Recharts en Next.js 14 (GAP-04)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/frontend-plan.md`
   - Cambio: En la seccion 3.7 (GraficoTemporal), agregar nota: "En Next.js 14 App Router, si se producen hydration errors, usar `dynamic()` con `ssr: false` al importar `GraficoTemporal` desde `ProgramaMetricasTab`."

6. **Definir contenido dinamico del texto sr-only del grafico (GAP-09)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/frontend-plan.md`
   - Cambio: En la seccion 3.7, especificar que el texto del `<p className="sr-only">` debe incluir los totales del periodo: `Grafico de actividad: {totalClicks} clicks, {totalSignups} registros y {totalConversiones} conversiones en el periodo seleccionado.`

### Nice to Have (Menor)

7. **Crear test-strategy.md para el plan Admin (GAP-07)**
   - Archivo nuevo: `plans/cp-tracking-metricas/frontend-admin/test-strategy.md`
   - Contenido minimo: tests para `useSortableTable`, `FiltroFechas` (validacion de fechas), `RankingPromotoresTable` (aria-sort, empty state), y `useProgramaMetricas` (error handling con retry:false).

8. **Documentar deuda tecnica de Shares en DesgloseEventosPanel (GAP-06)**
   - Archivo: `plans/cp-tracking-metricas/frontend-admin/frontend-plan.md`
   - Cambio: Agregar en la seccion 3.6 una nota: "Si en futuras iteraciones el backend incluye `totalShares` en `ProgramaMetricasKpis`, agregar la fila de Shares al panel. El tipo en shared debera actualizarse con `totalShares?: number`."

---

## 8. Checklist de Validacion

### Requisitos Funcionales

- [x] AC-CP05-9: KPIs del artista completamente cubiertos en KpiCardsGrid (7 KPIs: clicks, pageviews, signups, conversiones, valorTotal, tasaConversion, comisionesTotales)
- [x] AC-CP05-10: Filtro de fechas con refetch automatico al cambiar query key
- [x] AC-CP05-11: Ranking de promotores con 6 columnas y ordenacion por conversiones desc por defecto
- [x] AC-CP05-15: Tipos shared importados desde `@shared/types`; constantes de `@shared/constants`; schemas Zod de shared
- [ ] Breadcrumb completo disponible para todos los artistas (requiere resolver GAP-01)

### UI/UX

- [x] Pestana "Metricas" integrada en PromoProgramaDetailClient via TabsTrigger
- [x] Filtro de fechas con inputs date y validacion fechaDesde <= fechaHasta
- [x] fechaHasta limitada a hoy con `max=`
- [x] Badge de filtro activo con rango formateado y boton limpiar
- [x] 4 KPI cards primarias con iconos y colores del design system
- [x] 2 KPI cards secundarias (tasa conversion + comisiones) con layout horizontal
- [x] Tabla ranking con Avatar (fallback inicial), nombre, badge tipo promotor
- [x] Panel desglose con barras de progreso animadas (duration-500 ease-out)
- [x] Grafico Recharts con 3 series (clicks, signups, conversiones) sin pageviews
- [x] Tooltip del grafico con estilos dark
- [x] Empty state: KPIs en 0, tabla con mensaje, grafico con texto
- [ ] Loading con filtro (overlay + spinner) definido en ui-design (pendiente GAP-02)
- [ ] Card de error con markup completo en ui-design (pendiente GAP-03)

### Responsive Design

- [x] KPI primarias: grid-cols-2 lg:grid-cols-4
- [x] KPI secundarias: grid-cols-1 sm:grid-cols-2
- [x] Ranking + desglose: grid-cols-1 lg:grid-cols-5 (col-span-3 + col-span-2)
- [x] Tabla: columnas Valor y Comision ocultas en mobile (hidden md:table-cell)
- [x] Sidebar: hereda comportamiento colapsado del layout existente

### Accesibilidad

- [x] focus-visible:ring-2 ring-[#a855f7] en elementos interactivos
- [x] Tabla con scope="col" en cabeceras
- [x] aria-sort en columnas ordenables
- [x] Breadcrumb con aria-label y aria-current="page"
- [x] role="progressbar" con aria-valuenow/min/max en barras de desglose
- [x] sr-only alternativa textual en GraficoTemporal (contenido pendiente GAP-09)
- [ ] role="alert" en card de error (pendiente GAP-03)
- [ ] aria-busy en contenedores de skeleton (pendiente GAP-05/GAP-08)

### Testing

- [ ] test-strategy.md creado (pendiente GAP-07)
- [ ] useSortableTable: tests de ordenacion, alternancia, valores null
- [ ] FiltroFechas: tests de validacion de fechas
- [ ] RankingPromotoresTable: tests de aria-sort y empty state
- [ ] useProgramaMetricas: test de error handling con retry:false

---

## 9. Conclusion

**Score Final:** 83%

**Veredicto:** REQUIERE CAMBIOS

### Resumen de Hallazgos

Los planes `frontend-plan.md` y `ui-design.md` para el dashboard Admin de `cp-tracking-metricas` son documentos de alta calidad que cubren correctamente el nucleo funcional del dashboard del artista. Los tres criterios de aceptacion principales del Admin (AC-CP05-9, AC-CP05-10, AC-CP05-11) estan completamente cubiertos con componentes bien definidos, tipado desde shared correcto y logica de estado clara.

Los 3 gaps mayores identificados son inconsistencias entre los dos planes (breadcrumb opcional vs obligatorio, markup de estados de error/loading con filtro ausente en ui-design). Ninguno impide la implementacion, pero si se ignoran podran generar confusion durante el desarrollo o resultados inconsistentes con el design system.

Los 5 gaps menores son oportunidades de mejora: la ausencia de test-strategy es la mas importante desde el punto de vista de calidad a largo plazo.

### Proximo Paso

Resolver los 3 gaps mayores (GAP-01, GAP-02, GAP-03) antes de iniciar la implementacion. Especificamente:
1. Actualizar `frontend-plan.md` para hacer el breadcrumb obligatorio y agregar `campaniaId` a las props de `ProgramaMetricasTab`
2. Agregar markup de estado de loading-con-filtro en `ui-design.md`
3. Agregar markup del card de error en `ui-design.md`

Una vez resueltos esos tres puntos, el score sube aproximadamente a 94% (APROBADO) y se puede proceder a la implementacion.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-02
