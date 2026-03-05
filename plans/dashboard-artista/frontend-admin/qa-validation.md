# Validacion QA: Dashboard Artista (Admin)

**Fecha:** 2026-02-14
**Feature:** dashboard-artista
**Target:** src/admin

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 12 |
| Cubiertos | 1 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 11 |
| **Score de Cobertura** | **8.33%** |

**Estado:** ❌ **RECHAZADO**

**Razon critica:** No existen planes de implementacion del frontend-admin. Solo se ha generado `plans/dashboard-artista/shared/contracts-plan.md`, que define los contratos de tipos/schemas compartidos, pero **NO cubre la implementacion de componentes, paginas, hooks, ni servicios del dashboard en Admin**.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo |
|----|----------|------|
| AC-05-1 | El artista solo visualiza campanias donde ArtistaId = userId (filtro backend, no frontend) | Backend |
| AC-05-2 | Metricas se calculan en tiempo real (no cacheadas mas de 1 minuto) | Backend |
| AC-05-3 | Lista de backings se ordena por FechaCreacion DESC mostrando primero los mas recientes | Backend |
| AC-05-4 | Backings con PermitirMostrarNombre = false muestran "Anonimo" y NO exponen nombre/email en response | Backend + Admin |
| AC-05-5 | Porcentaje se calcula como (ImporteRecaudado / ImporteObjetivo) * 100 con 2 decimales | Backend |
| AC-05-6 | Dias restantes se calculan como (FechaFin - DateOnly.Today).Days y muestran 0 si es negativo | Backend + Admin |
| AC-05-7 | Dashboard principal muestra maximo 10 campanias con paginacion para ver mas | Admin |
| AC-05-8 | Al hacer backing en otra ventana, dashboard se actualiza automaticamente en 30s (polling) | Admin |
| AC-05-9 | Componente BackingTable es accesible (navegacion con teclado, labels en inputs) | Admin |
| AC-05-10 | Endpoint /api/dashboard/resumen responde en < 500ms con usuario que tiene 10 campanias | Backend |
| AC-05-11 | Usuario sin rol Artista recibe 403 Forbidden al acceder a endpoints de dashboard | Backend |
| AC-05-12 | Barra de progreso muestra color verde si >= 100%, amarillo si >= 50%, rojo si < 50% | Admin |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | contracts-plan | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|----------------|---------------|-----------|---------------|--------|
| AC-05-1 | Artista solo ve sus campanias (filtro backend) | - | - | - | - | NO CUBIERTO |
| AC-05-2 | Metricas en tiempo real (no cacheadas >1 min) | - | - | - | - | NO CUBIERTO |
| AC-05-3 | Backings ordenados por FechaCreacion DESC | - | - | - | - | NO CUBIERTO |
| AC-05-4 | Backings anonimos no exponen nombre/email | PARCIAL | - | - | - | PARCIAL |
| AC-05-5 | Porcentaje calculado con 2 decimales | CUBIERTO | - | - | - | CUBIERTO |
| AC-05-6 | Dias restantes calculados correctamente | CUBIERTO | - | - | - | CUBIERTO |
| AC-05-7 | Dashboard muestra max 10 campanias con paginacion | - | - | - | - | NO CUBIERTO |
| AC-05-8 | Auto-refresh cada 30s (polling) | - | - | - | - | NO CUBIERTO |
| AC-05-9 | BackingTable accesible (keyboard, labels) | - | - | - | - | NO CUBIERTO |
| AC-05-10 | /api/dashboard/resumen responde <500ms | - | - | - | - | NO CUBIERTO |
| AC-05-11 | Usuario sin rol Artista recibe 403 | - | - | - | - | NO CUBIERTO |
| AC-05-12 | Barra de progreso con colores segun porcentaje | - | - | - | - | NO CUBIERTO |

**Leyenda:**
- ✅ **CUBIERTO:** Requisito completamente implementado en plan
- 🟡 **PARCIAL:** Requisito parcialmente cubierto
- ❌ **NO CUBIERTO:** Requisito no mencionado en planes
- N/A: No aplica a este plan

**Detalle de cobertura parcial/cubierta:**

| ID | Plan | Seccion | Detalle |
|----|------|---------|---------|
| AC-05-4 | contracts-plan | `types/dashboard.ts` | Define `CampaniaBackingItem.esAnonimo: boolean` y `nombreBacker: string`, pero NO documenta logica de renderizado "Anonimo" en UI |
| AC-05-5 | contracts-plan | `utils/format.ts` | Define `calculateCampaniaProgress()` con formula `(importeRecaudado / importeObjetivo) * 100` y redondeo a 2 decimales |
| AC-05-6 | contracts-plan | `utils/format.ts` | Define `calculateDiasRestantes(fechaFin?)` con formula `Max(0, (fechaFin - hoy).days)` |

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile | - | NO CUBIERTO |
| NFR-02 | Accesibilidad WCAG AA | - | NO CUBIERTO |
| NFR-03 | Tiempo de carga < 3s | - | NO CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos (Bloquean MVP)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-CRIT-01 | AC-05-7, AC-05-8, AC-05-9, AC-05-12 | **NO existe plan de implementacion de frontend-admin** | CRITICO | Generar `frontend-plan.md` con: componentes (DashboardPage, StatsCard, BackingTable, ProgressBar), hooks (useDashboardResumen, useMisCampanias, useCampaniaBackings), servicios (dashboard.service.ts), y logica de polling cada 30s |
| GAP-CRIT-02 | AC-05-9 | **NO existe plan de accesibilidad** | CRITICO | Generar `test-strategy.md` incluyendo tests de accesibilidad (navegacion con teclado, aria-labels, contraste WCAG AA) |
| GAP-CRIT-03 | AC-05-12 | **NO existe plan de UI/UX para componentes** | CRITICO | Generar `ui-design.md` con especificaciones de: ProgressBar (colores segun porcentaje), BackingTable (columnas, estados), EmptyStates, LoadingSkeletons |
| GAP-CRIT-04 | AC-05-1, AC-05-2, AC-05-3, AC-05-10, AC-05-11 | **NO existe plan de backend** | BLOQUEA TODO | Generar `backend-plan.md` con: Queries CQRS (GetDashboardResumenQuery, GetMisCampaniasQuery, GetCampaniaBackingsQuery), Validators, autorizacion (rol Artista, ownership), indices DB |

### 4.2 Gaps Mayores (Impactan calidad)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-MAY-01 | AC-05-4 | `contracts-plan` define tipo `esAnonimo` pero NO especifica logica UI de renderizado "Anonimo" | MEDIO | En `frontend-plan.md` agregar seccion de renderizado condicional: `nombreBacker === "Anonimo" ? <Badge>Anonimo</Badge> : <Avatar>{nombreBacker}</Avatar>` |
| GAP-MAY-02 | AC-05-8 | No hay plan de estrategia de polling (useQuery con refetchInterval) | MEDIO | En `frontend-plan.md` documentar config de TanStack Query: `useQuery({ queryKey: QUERY_KEYS.dashboard.resumen, refetchInterval: 30000 })` |
| GAP-MAY-03 | AC-05-2 | No hay validacion de caching (staleTime max 1 min) | MEDIO | En `frontend-plan.md` agregar config: `useQuery({ staleTime: 60000 })` para evitar cache > 1 min |

### 4.3 Gaps Menores (Mejoras de documentacion)

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-MIN-01 | AC-05-5, AC-05-6 | `contracts-plan` define funciones de calculo pero NO documenta donde se usan | BAJO | En `frontend-plan.md` referenciar uso de `calculateCampaniaProgress()` en componente `<ProgressBar>` y `calculateDiasRestantes()` en `<StatCard>` |
| GAP-MIN-02 | Todos | No existe documento de estimacion de esfuerzo (horas) | BAJO | Agregar seccion "Estimacion" en `frontend-plan.md` (referencia: feature-spec.md estima 30h total) |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-05-7 | - | - | NO CUBIERTO |
| AC-05-8 | - | - | NO CUBIERTO |
| AC-05-9 | - | - | NO CUBIERTO |
| AC-05-12 | - | - | NO CUBIERTO |
| AC-05-4 | - | - | NO CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-05-7 | `test-dashboard-pagination` | Validar que dashboard muestra max 10 campanias y boton "Ver mas" |
| AC-05-8 | `test-auto-refresh-polling` | Validar que dashboard refetch datos cada 30s automaticamente |
| AC-05-9 | `test-backing-table-a11y` | Validar navegacion con Tab/Enter, aria-labels en headers, role="table" |
| AC-05-12 | `test-progress-bar-colors` | Validar colores: verde >= 100%, amarillo >= 50%, rojo < 50% |
| AC-05-4 | `test-anonymous-backing` | Validar que backing con esAnonimo=true muestra "Anonimo" sin email |

**Score de Cobertura de Tests:** 0/5 = **0%**

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| Dashboard Principal (`/dashboard`) | NO | - | NO CUBIERTO |
| Campana Detail (`/dashboard/campanias/[id]`) | NO | - | NO CUBIERTO |
| Backings List (`/dashboard/campanias/[id]/backings`) | NO | - | NO CUBIERTO |

**Referencias en ui-ux.md:**
- Mockup: `WPR_5-Dashboard-Artist.png`
- Template base: `dashtail/pages/dashboard.html`
- 3 pantallas documentadas con layout, componentes, estados de UI, interacciones

**Gap:** ui-ux.md existe como especificacion de diseno, pero **NO hay plan de implementacion** (frontend-plan.md) que mapee estos disenos a codigo React.

### Estados de UI

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading | Si (ui-ux.md documenta Skeletons) | NO | NO CUBIERTO |
| Error | Si (ui-ux.md documenta Alert component) | NO | NO CUBIERTO |
| Empty (sin campanias) | Si (ui-ux.md documenta EmptyState con CTA) | NO | NO CUBIERTO |
| Empty (sin backings) | Si (ui-ux.md documenta EmptyState) | NO | NO CUBIERTO |

### Componentes Requeridos vs Planificados

| Componente | Requerido en ui-ux.md | Planificado | Estado |
|------------|----------------------|-------------|--------|
| `<StatsCard>` | Si (4 cards en dashboard) | NO | NO CUBIERTO |
| `<ProgressBar>` | Si (barra de progreso con colores) | NO | NO CUBIERTO |
| `<CampaignCard>` | Si (mis campanias list) | NO | NO CUBIERTO |
| `<BackingTable>` | Si (tabla de backings con a11y) | NO | NO CUBIERTO |
| `<EmptyState>` | Si (sin campanias/backings) | NO | NO CUBIERTO |
| `<RecentBackings>` | Si (dashboard card) | NO | NO CUBIERTO |

**Nota:** ui-ux.md documenta que `<StatsCard>` y `<CompleteProfileBanner>` ya existen, pero NO hay plan de como integrarlos con datos reales de API.

---

## 7. Recomendaciones

### Acciones Requeridas (Critico) - BLOQUEAN MVP

1. **Generar `plans/dashboard-artista/frontend-admin/frontend-plan.md`**
   - Seccion 1: Componentes
     - `DashboardPage.tsx` (pagina principal)
     - `CampaniaDetailPage.tsx` (detalle de campania)
     - `BackingsListPage.tsx` (lista completa de backings)
     - `StatsCard.tsx` (reutilizar existente, documentar props y uso)
     - `ProgressBar.tsx` (nuevo, con logica de colores AC-05-12)
     - `CampaignCard.tsx` (nuevo, cards de mis campanias)
     - `BackingTable.tsx` (nuevo, tabla accesible AC-05-9)
     - `EmptyState.tsx` (reutilizar o crear)
   - Seccion 2: Hooks
     - `useDashboardResumen()` (GET /api/dashboard/resumen con polling 30s)
     - `useMisCampanias(params)` (GET /api/campanias/mis-campanias con paginacion)
     - `useCampaniaBackings(id, params)` (GET /api/campanias/{id}/backings)
     - `useCampaniaStats(id)` (GET /api/campanias/{id}/stats)
   - Seccion 3: Servicios
     - `dashboard.service.ts` (fetch functions con error handling)
   - Seccion 4: TanStack Query Config
     - Polling: `refetchInterval: 30000` (AC-05-8)
     - Caching: `staleTime: 60000` (AC-05-2)
     - Query keys: usar `QUERY_KEYS.dashboard.*` de contracts-plan
   - Seccion 5: Logica de Renderizado
     - Backings anonimos: `nombreBacker === "Anonimo" ? <Badge>Anonimo</Badge> : nombre` (AC-05-4)
     - Progress bar colores: `porcentaje >= 100 ? "green" : porcentaje >= 50 ? "yellow" : "red"` (AC-05-12)
     - Dias restantes: `calculateDiasRestantes(fechaFin)` con fallback "N/A" si undefined

2. **Generar `plans/dashboard-artista/frontend-admin/ui-design.md`**
   - Archivo: Ya existe `docs/user-stories/dashboard-artista/ui-ux.md` con especificaciones completas
   - Accion: Crear symlink o mover a `plans/dashboard-artista/frontend-admin/ui-design.md`
   - Validar que cubre: colores, tipografia, espaciado, responsive, animaciones, accesibilidad

3. **Generar `plans/dashboard-artista/frontend-admin/test-strategy.md`**
   - Unit Tests:
     - `StatsCard.test.tsx`
     - `ProgressBar.test.tsx` (validar colores AC-05-12)
     - `BackingTable.test.tsx` (validar renderizado anonimos AC-05-4)
     - `hooks/useDashboardResumen.test.ts`
   - Integration Tests:
     - `DashboardPage.integration.test.tsx` (validar polling AC-05-8)
   - A11y Tests:
     - `BackingTable.a11y.test.tsx` (validar keyboard navigation AC-05-9)
     - `dashboard-a11y.test.tsx` (validar contraste, aria-labels)
   - E2E Tests:
     - `dashboard-artista.e2e.ts` (flujo completo: login artista -> dashboard -> ver campania -> ver backings)

4. **Generar `plans/dashboard-artista/backend/backend-plan.md`**
   - CQRS Queries:
     - `GetDashboardResumenQuery.cs` (AC-05-10: responde <500ms)
     - `GetMisCampaniasQuery.cs` (AC-05-1: filtro ArtistaId backend, AC-05-7: paginacion)
     - `GetCampaniaBackingsQuery.cs` (AC-05-3: orden DESC, AC-05-4: no exponer datos anonimos)
     - `GetCampaniaStatsQuery.cs`
   - Validators:
     - `GetMisCampaniasValidator.cs` (validar pageSize <= 100)
     - `GetCampaniaBackingsValidator.cs` (validar ownership)
   - Autorizacion:
     - Middleware: validar rol "Artista" (AC-05-11: 403 si no tiene rol)
     - Validators: validar `campania.ArtistaId == artista.Id` (AC-05-1)
   - Indices DB (AC-05-10: performance <500ms):
     - `IX_CampaniaCrowdfunding_ArtistaId`
     - `IX_PedidoCrowdfunding_CampaniaId_FechaCreacion_DESC`
   - DTOs C# (ya definidos en contracts.md, implementar mappings)

### Acciones Sugeridas (Mayor) - IMPACTAN CALIDAD

1. **En `frontend-plan.md` agregar seccion de Error Handling**
   - Mapeo de error codes a mensajes user-friendly (usar `getDashboardErrorMessage()` de contracts-plan)
   - Manejo de 403 Forbidden: redirect a `/artista/perfil/crear` con toast "Necesitas crear perfil de artista"
   - Manejo de 401 Unauthorized: redirect a `/auth/login?returnUrl=/dashboard`

2. **En `frontend-plan.md` agregar seccion de Loading States**
   - Usar `<Skeleton>` de shadcn/ui (no spinners)
   - Loading granular: stats cards, campanias list, backings table
   - Skeleton layout match con contenido real

3. **En `test-strategy.md` agregar seccion de Performance Tests**
   - Validar tiempo de renderizado inicial < 2s
   - Validar polling no bloquea UI
   - Validar paginacion no causa re-render de toda la lista

### Nice to Have (Menor) - MEJORAS POST-MVP

1. **Agregar grafico de recaudacion (temporal)**
   - ui-ux.md documenta chart de recaudacion ultimos 30 dias
   - Puede ser placeholder "Proximamente" en MVP
   - Usar Recharts o Chart.js en iteracion posterior

2. **Exportar backings a CSV**
   - ui-ux.md documenta boton "Exportar CSV" en BackingsListPage
   - Implementar con `papaparse` o download directo

3. **Filtros avanzados en BackingsTable**
   - ui-ux.md documenta filtros: Todos, Con Reward, Anonimos
   - Agregar busqueda por nombre/email con debounce 300ms

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [ ] AC-05-1: Filtro backend (necesita backend-plan)
- [ ] AC-05-2: Metricas tiempo real (necesita frontend-plan con staleTime config)
- [ ] AC-05-3: Orden backings DESC (necesita backend-plan)
- [ ] AC-05-4: Backings anonimos (PARCIAL en contracts, necesita frontend-plan con logica renderizado)
- [x] AC-05-5: Porcentaje 2 decimales (CUBIERTO en contracts-plan)
- [x] AC-05-6: Dias restantes calculados (CUBIERTO en contracts-plan)
- [ ] AC-05-7: Paginacion max 10 (necesita frontend-plan)
- [ ] AC-05-8: Auto-refresh 30s (necesita frontend-plan con polling)
- [ ] AC-05-9: BackingTable a11y (necesita frontend-plan + test-strategy)
- [ ] AC-05-10: Performance <500ms (necesita backend-plan con indices)
- [ ] AC-05-11: 403 sin rol Artista (necesita backend-plan)
- [ ] AC-05-12: ProgressBar colores (necesita frontend-plan)

### UI/UX
- [ ] Todas las screens planificadas (necesita frontend-plan)
- [ ] Estados de interaccion definidos (necesita frontend-plan)
- [ ] Responsive design considerado (ui-ux.md lo documenta, necesita frontend-plan para implementacion)
- [ ] Accesibilidad validada (necesita test-strategy)

### Testing
- [ ] Tests para criterios criticos (necesita test-strategy)
- [ ] Tests de integracion para flujos (necesita test-strategy)
- [ ] Cobertura objetivo definida (necesita test-strategy)

---

## 9. Conclusion

**Score Final:** 8.33%

**Veredicto:** ❌ **RECHAZADO**

**Razon:** Solo se ha generado el plan de contratos compartidos (`contracts-plan.md`), que cubre **unicamente la definicion de tipos, schemas y constantes**, pero **NO incluye ningun plan de implementacion** de componentes, paginas, hooks, servicios, tests, ni backend.

**Proximo Paso:** **CRITICO - GENERAR PLANES FALTANTES**

Antes de proceder a implementacion, se deben generar en este orden:

1. **Backend Plan** (BLOQUEA TODO) → Define endpoints, queries, validators, autorizacion
2. **Frontend Plan** (ALTA PRIORIDAD) → Define componentes, hooks, servicios, logica de polling
3. **Test Strategy** (ALTA PRIORIDAD) → Define unit tests, integration tests, a11y tests, E2E tests
4. **UI Design** (MEDIA PRIORIDAD) → Ya existe ui-ux.md, mover/symlink a plans/

**Estimacion de esfuerzo para generar planes:**
- Backend Plan: 3-4 horas
- Frontend Plan: 4-5 horas
- Test Strategy: 2-3 horas
- **Total:** 9-12 horas de planificacion antes de implementar

**Riesgos si se implementa sin planes:**
- Desalineacion entre frontend y backend (tipos no coinciden)
- Falta de cobertura de tests (AC-05-9 accesibilidad, AC-05-8 polling)
- Performance issues (AC-05-10 <500ms sin indices DB)
- Seguridad comprometida (AC-05-11 sin validacion rol Artista)

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-14

---

## Anexo A: Cobertura por Plan Existente

### `contracts-plan.md` - Cobertura: 8.33%

**Contenido:**
- ✅ 9 interfaces TypeScript (DashboardResumen, MiCampaniaListItem, etc.)
- ✅ 2 schemas Zod (misCampaniasQuerySchema, backingsQuerySchema)
- ✅ Actualizaciones a constantes (QUERY_KEYS, API_ROUTES, APP_ROUTES)
- ✅ 6 funciones de utilidad (calculateCampaniaProgress, calculateDiasRestantes, etc.)
- ✅ Error messages (DASHBOARD_ERROR_MESSAGES)

**Criterios cubiertos:**
- AC-05-5: Porcentaje 2 decimales (via `calculateCampaniaProgress()`)
- AC-05-6: Dias restantes (via `calculateDiasRestantes()`)

**Criterios parcialmente cubiertos:**
- AC-05-4: Define tipo `esAnonimo` pero NO logica de renderizado "Anonimo" en UI

**Criterios NO cubiertos:**
- AC-05-1, AC-05-2, AC-05-3, AC-05-7, AC-05-8, AC-05-9, AC-05-10, AC-05-11, AC-05-12

**Limitaciones:**
- Solo define contratos, NO implementacion
- NO incluye logica de componentes React
- NO incluye logica de backend (queries, validators)
- NO incluye tests

---

## Anexo B: Referencias a Documentacion Existente

| Documento | Ruta | Contenido | Utilidad para validacion |
|-----------|------|-----------|--------------------------|
| Feature Spec | `docs/user-stories/dashboard-artista/feature-spec.md` | 12 criterios de aceptacion, flujos, componentes UI, estimacion 30h | Fuente de verdad de requisitos |
| Contracts | `docs/user-stories/dashboard-artista/contracts.md` | 4 endpoints API, DTOs C#/TypeScript, validaciones, errores | Define contrato backend-frontend |
| UI/UX | `docs/user-stories/dashboard-artista/ui-ux.md` | 3 pantallas, mockup WPR_5, design tokens, componentes shadcn/ui, accesibilidad | Define diseno visual y UX |
| Contracts Plan | `plans/dashboard-artista/shared/contracts-plan.md` | Plan de implementacion de tipos/schemas shared | Unico plan existente (8.33% cobertura) |

**Gap critico:** Falta documentar COMO se implementan los disenos de ui-ux.md en codigo React (componentes, hooks, estado).
