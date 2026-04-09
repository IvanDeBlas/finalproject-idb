# Validacion QA: Hacer Backing (Admin)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/admin

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 8 |
| Cubiertos | 0 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 8 |
| **Score de Cobertura** | **0%** |

**Estado:** ❌ RECHAZADO

**Razon:** No existen planes de implementacion para Admin. La feature-spec.md indica impacto BAJO para Admin en MVP.

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md - Seccion "Proyectos Involucrados"

| ID | Criterio | Tipo | Prioridad |
|----|----------|------|-----------|
| AC-ADM-01 | Vista de backings recibidos por campania | Funcional | MVP |
| AC-ADM-02 | Estadisticas basicas (Total Raised, Total Backers, Avg Amount) | Funcional | MVP |
| AC-ADM-03 | Tabla con datos de backers (nombre/anonimo, monto, reward, fecha) | Funcional | MVP |
| AC-ADM-04 | Filtrado por reward | Funcional | Post-MVP |
| AC-ADM-05 | Busqueda por nombre de backer | Funcional | Post-MVP |
| AC-ADM-06 | Export CSV de backings | Funcional | Post-MVP |
| AC-ADM-07 | Nombres anonimos mostrados correctamente | Funcional | MVP |
| AC-ADM-08 | Paginacion funcional | Funcional | MVP |

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|---------------|-----------|---------------|--------|
| AC-ADM-01 | Vista de backings recibidos por campania | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-02 | Estadisticas basicas (Total Raised, Total Backers, Avg Amount) | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-03 | Tabla con datos de backers | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-04 | Filtrado por reward | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-05 | Busqueda por nombre | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-06 | Export CSV | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-07 | Nombres anonimos mostrados correctamente | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |
| AC-ADM-08 | Paginacion funcional | ❌ NO EXISTE | ❌ NO EXISTE | ❌ NO EXISTE | NO CUBIERTO |

**Leyenda:**
- ✅ CUBIERTO: Requisito completamente implementado en plan
- ⚠️ PARCIAL: Requisito parcialmente cubierto
- ❌ NO CUBIERTO: Requisito no mencionado en planes
- N/A: No aplica a este plan

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-ADM-01 | Responsive mobile | ❌ NO EXISTE | NO CUBIERTO |
| NFR-ADM-02 | Accesibilidad WCAG AA | ❌ NO EXISTE | NO CUBIERTO |
| NFR-ADM-03 | Tiempo de carga < 3s | ❌ NO EXISTE | NO CUBIERTO |

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-C-01 | AC-ADM-01 | No existe frontend-plan.md | CRITICO | Crear plan de implementacion completo |
| GAP-C-02 | AC-ADM-02 | No existe ui-design.md | CRITICO | Disenar UI de estadisticas y tabla |
| GAP-C-03 | AC-ADM-03 | No existe test-strategy.md | CRITICO | Planificar tests unitarios e integracion |
| GAP-C-04 | ENDPOINTS | No se especifica uso de endpoints GET /api/campanias/{id}/backings y GET /api/campanias/{id}/stats | CRITICO | Documentar integracion con endpoints del contrato |
| GAP-C-05 | TIPOS | No se especifica uso de tipos BackingPublicDto, CampaniaStats de shared | CRITICO | Documentar uso de tipos compartidos |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-M-01 | AC-ADM-07 | No se especifica logica de display de nombres anonimos | MAYOR | Usar getBackerDisplayName de shared/utils |
| GAP-M-02 | COMPONENTES | No se especifica estructura de componentes (StatsCards, BackingsTable, etc.) | MAYOR | Definir arquitectura de componentes |
| GAP-M-03 | HOOKS | No se especifica hooks para queries (useCampaniaBackings, useCampaniaStats) | MAYOR | Planificar custom hooks con TanStack Query |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-L-01 | AC-ADM-04 | Filtrado por reward no planificado (Post-MVP) | BAJO | Considerar en planificacion futura |
| GAP-L-02 | AC-ADM-05 | Busqueda por nombre no planificada (Post-MVP) | BAJO | Considerar en planificacion futura |
| GAP-L-03 | AC-ADM-06 | Export CSV no planificado (Post-MVP) | BAJO | Considerar en planificacion futura |

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-ADM-01 | ❌ NO EXISTE | - | NO CUBIERTO |
| AC-ADM-02 | ❌ NO EXISTE | - | NO CUBIERTO |
| AC-ADM-03 | ❌ NO EXISTE | - | NO CUBIERTO |
| AC-ADM-07 | ❌ NO EXISTE | - | NO CUBIERTO |
| AC-ADM-08 | ❌ NO EXISTE | - | NO CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-ADM-01 | test-backings-list-renders | Validar renderizado de lista de backings |
| AC-ADM-02 | test-stats-calculation | Validar calculo de estadisticas |
| AC-ADM-03 | test-table-data-display | Validar display de datos en tabla |
| AC-ADM-07 | test-anonymous-backer-display | Validar que nombres anonimos muestran "Anonimo" |
| AC-ADM-08 | test-pagination-functionality | Validar navegacion entre paginas |

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| Dashboard Backings Management | ❌ NO | - | NO CUBIERTO |
| Stats Cards (Total Raised, Backers, Avg) | ❌ NO | - | NO CUBIERTO |
| Backings Table | ❌ NO | - | NO CUBIERTO |
| Filters Row (Search, Reward filter, Export) | ❌ NO | - | NO CUBIERTO |

### Estados de UI

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading | Si | ❌ NO | NO CUBIERTO |
| Empty State (sin backings) | Si | ❌ NO | NO CUBIERTO |
| Error | Si | ❌ NO | NO CUBIERTO |
| Paginated Data | Si | ❌ NO | NO CUBIERTO |

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear frontend-plan.md**
   - Archivo: `plans/hacer-backing/frontend-admin/frontend-plan.md`
   - Cambio: Documentar componentes, hooks, services, routing
   - Incluir: Uso de GET /api/campanias/{id}/backings y GET /api/campanias/{id}/stats
   - Incluir: Tipos BackingPublicDto, CampaniaStats de shared

2. **Crear ui-design.md**
   - Archivo: `plans/hacer-backing/frontend-admin/ui-design.md`
   - Cambio: Disenar layout de pantalla de backings management
   - Incluir: Wireframes de stats cards, tabla, filtros
   - Incluir: Estados de UI (loading, empty, error)
   - Incluir: Responsive design y accesibilidad

3. **Crear test-strategy.md**
   - Archivo: `plans/hacer-backing/frontend-admin/test-strategy.md`
   - Cambio: Planificar tests unitarios, integracion, E2E
   - Incluir: Tests para display de nombres anonimos
   - Incluir: Tests para paginacion
   - Incluir: Tests para estadisticas

4. **Especificar Arquitectura de Componentes**
   - Componente: BackingsManagementPage
   - Componentes hijos: StatsGrid, BackingsTable, FiltersRow, PaginationControls
   - Hooks: useCampaniaBackings, useCampaniaStats
   - Service: backings.service.ts (wrapper de API calls)

5. **Documentar Uso de Endpoints**
   - Endpoint GET /api/campanias/{id}/backings:
     - Query params: pageNumber, pageSize
     - Response: PaginatedResponse<BackingPublicDto>
   - Endpoint GET /api/campanias/{id}/stats:
     - Response: CampaniaStats

6. **Documentar Uso de Tipos Shared**
   - BackingPublicDto: para items de la tabla
   - CampaniaStats: para stats cards
   - getBackerDisplayName: helper para display de nombres anonimos
   - QUERY_KEYS.backings.byCampania: para TanStack Query

### Acciones Sugeridas (Mayor)

1. **Definir Layout Responsivo**
   - Desktop: Stats cards grid 3 cols, tabla completa
   - Tablet: Stats cards grid 2 cols, tabla con scroll horizontal
   - Mobile: Stats cards stack vertical, tabla simplificada

2. **Planificar Manejo de Errores**
   - Error loading stats: mostrar toast error, retry button
   - Error loading backings: mostrar empty state con retry
   - Network errors: usar getBackingErrorMessage de shared

3. **Considerar Accesibilidad**
   - Tabla con headers semanticos
   - Stats cards con role="region" y aria-label
   - Focus states en filtros y paginacion
   - Keyboard navigation en tabla

### Nice to Have (Menor)

1. **Implementar Features Post-MVP**
   - Filtrado por reward (dropdown con lista de rewards)
   - Busqueda por nombre de backer (input con debounce)
   - Export CSV (boton que genera CSV de todos los backings)

2. **Agregar Visualizaciones Adicionales**
   - Grafico de line chart de backings por fecha
   - Grafico de bar chart de backings por reward

## 8. Checklist de Validacion

### Requisitos Funcionales
- ❌ Todos los criterios de aceptacion tienen cobertura
- ❌ Vista de backings planificada
- ❌ Estadisticas planificadas
- ❌ Tabla planificada
- ❌ Paginacion planificada
- ❌ Display de nombres anonimos documentado

### UI/UX
- ❌ Screen de backings management planificada
- ❌ Stats cards diseñadas
- ❌ Tabla diseñada
- ❌ Estados de UI definidos
- ❌ Responsive design considerado
- ❌ Accesibilidad validada

### Testing
- ❌ Tests para requisitos criticos planificados
- ❌ Tests de integracion para carga de datos
- ❌ Cobertura objetivo definida

### Integracion con Backend
- ❌ Uso de endpoints documentado
- ❌ Tipos compartidos especificados
- ❌ Manejo de errores planificado

## 9. Conclusion

**Score Final:** 0%

**Veredicto:** ❌ RECHAZADO

**Razon:** No existen planes de implementacion para Admin. La feature-spec.md indica impacto BAJO para MVP, lo cual es consistente con la ausencia de planes.

**Proximo Paso:**
- Si Admin es REQUERIDO para MVP: Crear planes faltantes (frontend-plan.md, ui-design.md, test-strategy.md) siguiendo las recomendaciones criticas
- Si Admin es OPCIONAL para MVP: Marcar como "Pospuesto post-MVP" y priorizar Landing (impacto ALTO)

**Recomendacion:** Dado que:
1. La feature-spec.md marca Admin con impacto BAJO
2. La seccion "Backings Management (Artist View)" en ui-ux.md tiene nota "MVP Simplificado" con layout basico
3. El foco principal es el flujo del fan (Landing)

**SUGERENCIA: Implementar Admin post-MVP**, enfocando el MVP en:
- Landing: Flujo completo de explorar campanias → ver detalle → hacer backing → confirmacion
- Admin: Posponer dashboard de backings para artista

**Alternativa rapida para MVP (si se requiere algo de Admin):**
- Crear vista READ-ONLY simple: Stats cards + tabla basica sin filtros/busqueda/export
- Reutilizar componentes de shadcn/ui (Table, Card)
- Tiempo estimado: 4-6 horas vs 12-16 horas para version completa

---

## 10. Especificacion Minima para MVP (Si se Requiere Admin)

### 10.1 Componentes Minimos

```
src/admin/src/app/(dashboard)/campanias/[id]/backings/
├── page.tsx                    # BackingsManagementPage
├── components/
│   ├── StatsGrid.tsx          # 3 stats cards
│   ├── BackingsTable.tsx      # Tabla simple de shadcn/ui
│   └── PaginationControls.tsx # Prev/Next buttons
```

### 10.2 Hooks Minimos

```typescript
// hooks/useCampaniaBackings.ts
export const useCampaniaBackings = (campaniaId: string, page: number) => {
  return useQuery({
    queryKey: QUERY_KEYS.backings.byCampania(campaniaId),
    queryFn: () => backingsService.getByCampania(campaniaId, page),
  });
};

// hooks/useCampaniaStats.ts
export const useCampaniaStats = (campaniaId: string) => {
  return useQuery({
    queryKey: QUERY_KEYS.campanias.stats(campaniaId),
    queryFn: () => campaniaService.getStats(campaniaId),
  });
};
```

### 10.3 UI Minima

```
┌────────────────────────────────────────────────────────────────┐
│  Dashboard > Campanias > "Ecos del Silencio" > Backings       │
│                                                                │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │ Total Raised │  │ Total Backers│  │ Avg. Amount  │         │
│  │ €8,450       │  │ 127          │  │ €66.53       │         │
│  └──────────────┘  └──────────────┘  └──────────────┘         │
│                                                                │
│  ┌────────────────────────────────────────────────────────┐   │
│  │ Backer       │ Amount │ Reward        │ Date          │   │
│  ├────────────────────────────────────────────────────────┤   │
│  │ Carlos M.    │ €75    │ Disco firmado │ 15 Ene 2025   │   │
│  │ Ana G.       │ €50    │ Vinilo limited│ 14 Ene 2025   │   │
│  │ [Anonimo]    │ €25    │ CD Fisico     │ 13 Ene 2025   │   │
│  └────────────────────────────────────────────────────────┘   │
│                                                                │
│  [← Anterior]   1  2  3  [Siguiente →]                         │
└────────────────────────────────────────────────────────────────┘
```

### 10.4 Criterios de Aceptacion Minimos

- ✅ Vista de backings accesible desde `/dashboard/campanias/{id}/backings`
- ✅ Stats cards: Total Raised, Total Backers, Avg Amount
- ✅ Tabla: Backer, Amount, Reward, Date (sin filtros, sin busqueda)
- ✅ Nombres anonimos muestran "[Anonimo]"
- ✅ Paginacion: Prev/Next buttons
- ❌ Filtrado por reward (pospuesto)
- ❌ Busqueda por nombre (pospuesto)
- ❌ Export CSV (pospuesto)

### 10.5 Tiempo Estimado

| Tarea | Tiempo |
|-------|--------|
| Crear componentes (StatsGrid, BackingsTable, Pagination) | 2h |
| Crear hooks (useCampaniaBackings, useCampaniaStats) | 1h |
| Crear page.tsx y routing | 1h |
| Styling con Tailwind + shadcn/ui | 1h |
| Tests unitarios basicos | 1h |
| **TOTAL** | **6h** |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-13
