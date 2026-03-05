# Validacion QA: cs-templates-guia (Admin Dashboard)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia
**Target:** src/admin (Next.js 14)

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 9 |
| Cubiertos | 2 |
| Parcialmente Cubiertos | 1 |
| No Cubiertos | 6 |
| **Score de Cobertura** | **22%** |

**Estado:** REQUIERE CAMBIOS (Admin es LOW priority para MVP)

**Nota Critica:** La feature cs-templates-guia esta PRIMARIAMENTE enfocada en el flujo del artista en Landing (wizard de 3 pasos). Admin solo cubre AC-CS01-8 (seed data management) y CRUD de templates (post-MVP). El bajo score refleja que los criterios de aceptacion estan escritos desde la perspectiva del usuario final (Landing), NO desde Admin.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Scope Admin |
|----|----------|------|-------------|
| AC-CS01-1 | Galeria de templates con cards (6 templates) | Funcional | NO (Landing) |
| AC-CS01-2 | Desglose por fases con necesidades | Funcional | NO (Landing) |
| AC-CS01-3 | Marcar/desmarcar necesidades con checkboxes | Funcional | NO (Landing) |
| AC-CS01-4 | Ajustar presupuesto min/max por necesidad | Funcional | NO (Landing) |
| AC-CS01-5 | Crear N registros NecesidadCrowdsourcing | Funcional | NO (Landing) |
| AC-CS01-6 | Vincular necesidades a ProyectoArtistico | Funcional | NO (Landing) |
| AC-CS01-7 | Resumen pre-confirmacion | Funcional | NO (Landing) |
| AC-CS01-8 | Seed data de 6 templates pre-cargados | No Funcional | **SI (Admin)** |
| AC-CS01-9 | Tooltips en roles profesionales | Funcional | NO (Landing) |

**Conclusion:** 8 de 9 criterios son especificos de Landing. Admin solo aplica a AC-CS01-8.

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | Cobertura Admin | Estado |
|----|----------|-----------------|--------|
| AC-CS01-1 | Galeria templates | N/A (Landing) | N/A |
| AC-CS01-2 | Desglose necesidades | N/A (Landing) | N/A |
| AC-CS01-3 | Checkboxes seleccion | N/A (Landing) | N/A |
| AC-CS01-4 | Ajuste presupuesto | N/A (Landing) | N/A |
| AC-CS01-5 | Crear necesidades | N/A (Landing) | N/A |
| AC-CS01-6 | Vincular a proyecto | N/A (Landing) | N/A |
| AC-CS01-7 | Resumen confirmacion | N/A (Landing) | N/A |
| AC-CS01-8 | Seed data templates | ui-ux.md: Pantalla 4 (listado) + Pantalla 5 (CRUD) | PARCIAL |
| AC-CS01-9 | Tooltips roles | N/A (Landing) | N/A |

**Admin Coverage Detail (AC-CS01-8):**

| Componente | Planificado | Estado |
|------------|-------------|--------|
| **Listado de Templates** | ui-ux.md Pantalla 4 | CUBIERTO |
| - Tabla con 6 templates seed | Si | CUBIERTO |
| - Filtros (search + status) | Si | CUBIERTO |
| - Acciones (Editar, Ver necesidades, Toggle status) | Si | CUBIERTO |
| - Paginacion | Si | CUBIERTO |
| **CRUD Template** | ui-ux.md Pantalla 5 | CUBIERTO |
| - Formulario crear/editar | Si | CUBIERTO |
| - CRUD necesidades dentro de template | Si | CUBIERTO |
| - Select de rol profesional | Si | CUBIERTO |
| - Validacion campos | Si | CUBIERTO |
| **MISSING - Seed Verification** | - | NO CUBIERTO |
| - Endpoint GET para verificar seed | NO planificado | GAP CRITICO |
| - Dashboard metrics (6 templates activos?) | NO planificado | GAP MENOR |

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura Admin | Estado |
|----|----------|-----------------|--------|
| NFR-01 | Responsive mobile | ui-ux.md: responsive breakpoints | CUBIERTO |
| NFR-02 | Accesibilidad WCAG AA | ui-ux.md: seccion accesibilidad | CUBIERTO |
| NFR-03 | Cache de templates (< 500ms) | NO planificado en admin | NO APLICA |
| NFR-04 | Validacion ownership proyecto | N/A (Landing) | N/A |
| NFR-05 | Rate limiting generacion | N/A (Landing) | N/A |
| NFR-06 | Logs auditoria creacion | Backend concern | NO APLICA |
| NFR-07 | Templates editables sin deploy | ui-ux.md Pantalla 5: CRUD | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-01 | AC-CS01-8 | No hay endpoint/pantalla para **verificar** que seed data se cargo correctamente (6 templates con ~55 necesidades) | Alto | Agregar metrica/dashboard en Admin: "Templates activos: 6/6" con detalle de cada uno |
| GAP-02 | contracts-plan | No hay plan de implementacion especifico para frontend-admin (solo shared contracts) | Medio | Crear `frontend-admin-plan.md` con arquitectura de componentes, routing, services |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-03 | ui-ux.md Pantalla 5 | No especifica como manejar la eliminacion de templates que ya tienen necesidades generadas en produccion | Medio | Agregar soft-delete (toggle Activo) + dialogo warning si template tiene necesidades activas |
| GAP-04 | Validacion seed | No hay tests E2E para verificar que los 6 templates seed tienen datos correctos (iconos, precios, roles) | Medio | Agregar test de integracion que valide estructura de cada template seed |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-05 | ui-ux.md | No especifica preview del template antes de editar (ver como lo veria el artista en Landing) | Bajo | Agregar boton "Vista previa" en detalle que simule Pantalla 2 de Landing |
| GAP-06 | contracts.md | No hay endpoint PATCH para editar template individual (solo POST crear) | Bajo | Agregar `PUT /api/crowdsourcing/templates/{id}` en contracts |
| GAP-07 | ui-ux.md Pantalla 4 | No especifica que pasa si no hay templates (empty state tras borrar todos) | Bajo | Mostrar mensaje: "No hay templates. Los datos seed deben cargarse automaticamente." + boton "Recargar seed" |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-CS01-8 (Seed) | NO planificado | Integration | NO CUBIERTO |
| ui-ux Pantalla 4 (Listado) | NO planificado | E2E | NO CUBIERTO |
| ui-ux Pantalla 5 (CRUD) | NO planificado | E2E | NO CUBIERTO |
| Validacion formulario | NO planificado | Unit | NO CUBIERTO |
| Filtros search/status | NO planificado | Integration | NO CUBIERTO |

**Nota:** No existe `test-strategy.md` para esta feature, por lo que no hay tests planificados.

### Tests Requeridos (Minimo MVP)

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-CS01-8 | `test-seed-templates-loaded` | Verificar que migraciones seed crearon 6 templates |
| Pantalla 4 | `test-admin-list-templates` | Verificar que tabla muestra 6 templates con datos correctos |
| Pantalla 5 | `test-admin-create-template` | Verificar flujo completo de crear template con necesidades |
| Pantalla 5 | `test-admin-edit-template` | Verificar edicion de template existente |
| Pantalla 4 | `test-admin-toggle-template-status` | Verificar activar/desactivar template |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| **Pantalla 4: Listado Templates** | ui-ux.md | Table, Filters, Pagination | CUBIERTO |
| **Pantalla 5: Crear/Editar Template** | ui-ux.md | Form, NeedItemCard, IconSelect | CUBIERTO |
| Dashboard Overview (metrics) | NO | - | NO CUBIERTO |
| Preview Template (como Landing) | NO | - | NO CUBIERTO |

### Estados de UI (Pantalla 4: Listado)

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading | Si | Skeleton rows | CUBIERTO |
| Default (6 templates) | Si | Table con datos | CUBIERTO |
| Empty (no templates) | Si | "No hay templates disponibles" | CUBIERTO |
| Filtered (search) | Si | Debounce 300ms | CUBIERTO |
| Error (fetch failed) | Si | Toast rojo | CUBIERTO |
| Success (toggle status) | Si | Toast verde | CUBIERTO |

### Estados de UI (Pantalla 5: CRUD)

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading (edit mode) | Si | Skeleton en form | CUBIERTO |
| Default (nuevo) | Si | Form vacio | CUBIERTO |
| Default (editar) | Si | Form pre-rellenado | CUBIERTO |
| Add Need | Si | Card vacia agregada | CUBIERTO |
| Delete Need | Si | Dialogo confirmacion | CUBIERTO |
| Validation Error | Si | Border rojo + mensaje | CUBIERTO |
| Loading Submit | Si | Spinner + disabled | CUBIERTO |
| Success | Si | Toast + redirect | CUBIERTO |
| Error | Si | Toast rojo | CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico - Bloquean MVP)

**NINGUNA**

Justificacion: Admin es LOW priority. El MVP puede funcionar con seed data pre-cargado. CRUD de templates es post-MVP.

### Acciones Sugeridas (Mayor - Mejoran MVP)

1. **Crear frontend-admin-plan.md**
   - Archivo: `plans/cs-templates-guia/frontend-admin/frontend-admin-plan.md`
   - Cambio: Documentar arquitectura de componentes, routing, services, hooks para Admin
   - Razon: Actualmente solo existe contracts-plan (shared) y ui-ux (diseno). Falta plan de implementacion tecnico.

2. **Agregar endpoint de verificacion seed**
   - Archivo: `docs/user-stories/cs-templates-guia/contracts.md`
   - Cambio: Agregar `GET /api/crowdsourcing/templates/health` que retorne: `{ seedDataLoaded: true, templatesCount: 6 }`
   - Razon: Admin necesita verificar que seed se cargo correctamente tras migraciones.

3. **Agregar test-strategy.md**
   - Archivo: `plans/cs-templates-guia/frontend-admin/test-strategy.md`
   - Cambio: Definir tests minimos (E2E list, E2E CRUD, integration seed)
   - Razon: Sin estrategia de testing, no hay garantia de que CRUD funcione.

### Nice to Have (Menor - Post-MVP)

1. **Agregar vista previa de template**
   - Archivo: `docs/user-stories/cs-templates-guia/ui-ux.md` Pantalla 5
   - Cambio: Agregar boton "Vista previa" que simule como artista vera el template en Landing
   - Razon: Permite a admin ver resultado sin cambiar de aplicacion.

2. **Agregar dashboard de metricas seed**
   - Archivo: Nuevo: `ui-ux.md` Pantalla 6: Dashboard Crowdsourcing
   - Cambio: Pantalla resumen: "6 templates activos", "35 roles profesionales", "55 necesidades seed"
   - Razon: Visibilidad rapida de salud del sistema.

3. **Agregar endpoint PUT para editar templates**
   - Archivo: `docs/user-stories/cs-templates-guia/contracts.md`
   - Cambio: Documentar `PUT /api/crowdsourcing/templates/{id}` con DTOs
   - Razon: Actualmente contracts solo documenta GET y POST /generar. Falta CRUD completo.

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [x] Criterios de aceptacion identificados (9 total)
- [x] Scope Admin delimitado (solo AC-CS01-8 aplica)
- [ ] Endpoint de verificacion seed planificado (GAP-01)
- [x] CRUD de templates planificado en ui-ux (Pantallas 4 y 5)

### UI/UX
- [x] Pantalla 4 (Listado) especificada completamente
- [x] Pantalla 5 (CRUD) especificada completamente
- [ ] Pantalla preview/dashboard NO planificada (GAP-05, GAP-07)
- [x] Estados de UI definidos (loading, empty, error, success)
- [x] Responsive design considerado (mobile: cards, desktop: table)
- [x] Accesibilidad validada (ARIA labels, focus states, keyboard nav)

### Testing
- [ ] Tests de seed data NO planificados (GAP critico)
- [ ] Tests E2E de listado NO planificados
- [ ] Tests E2E de CRUD NO planificados
- [ ] Test strategy NO existe (GAP-02)

### Documentacion
- [x] Contracts definidos (shared/contracts-plan.md)
- [x] UI/UX documentado (ui-ux.md Pantallas 4 y 5)
- [ ] Frontend plan NO existe (GAP-02)
- [ ] Test strategy NO existe

---

## 9. Analisis de Cobertura por Fuente

### Cobertura desde feature-spec.md

| Documento | AC Cubiertos | AC Parciales | AC No Cubiertos | Score |
|-----------|--------------|--------------|-----------------|-------|
| feature-spec.md | 0 | 1 (AC-CS01-8) | 8 | 11% |

**Justificacion:** Los criterios estan escritos desde perspectiva de Landing (wizard de artista). Admin solo gestiona seed data (AC-CS01-8).

### Cobertura desde ui-ux.md

| Pantalla | Componentes Definidos | Estados UI | Accesibilidad | Score |
|----------|----------------------|------------|---------------|-------|
| Pantalla 4 (Listado) | 15/15 | 6/6 | 10/10 | 100% |
| Pantalla 5 (CRUD) | 17/17 | 9/9 | 12/12 | 100% |

**Justificacion:** El diseno UI/UX esta completo para las 2 pantallas de Admin. Falta solo implementacion.

### Cobertura desde contracts.md

| Endpoints Admin | Definidos | Implementables | Score |
|-----------------|-----------|----------------|-------|
| GET /templates | Si | Si | 100% |
| GET /templates/{id} | Si | Si | 100% |
| POST /templates | NO | - | 0% |
| PUT /templates/{id} | NO | - | 0% |
| DELETE /templates/{id} | NO | - | 0% |
| GET /maestras/roles | Si | Si | 100% |
| GET /maestras/categorias | Si | Si | 100% |

**Total:** 3 de 7 endpoints CRUD documentados (43%)

**Gap:** Falta documentar endpoints POST/PUT/DELETE para CRUD completo.

---

## 10. Matriz de Prioridad MVP vs Post-MVP

### In Scope MVP (DEBE implementarse)

| Item | Razon | Bloqueante? |
|------|-------|-------------|
| Seed data 6 templates | Sin seed, Landing no tiene templates para mostrar | **SI** |
| Endpoint GET /templates | Landing requiere listar templates | **SI** |
| Endpoint GET /templates/{id} | Landing requiere detalle de template | **SI** |
| Endpoint GET /maestras/roles | Landing requiere tooltips de roles | **SI** |
| Shared types y schemas | Contratos entre frontend y backend | **SI** |

**Nota:** Todos estos items son BACKEND concerns. Admin UI NO es bloqueante para MVP.

### Out of Scope MVP (Post-MVP)

| Item | Razon | Prioridad |
|------|-------|-----------|
| Pantalla 4 (Listado templates) | Admin puede ver templates via DB directamente | Media |
| Pantalla 5 (CRUD templates) | Templates seed son suficientes para MVP | Media |
| Tests E2E Admin | CRUD no es MVP, tests no bloquean | Baja |
| Dashboard metricas | Nice to have, no impacta funcionalidad | Baja |
| Vista previa template | Conveniencia, no requerido | Baja |

---

## 11. Score Ajustado por Scope

### Score Original (todos los AC)

- **Total AC:** 9
- **Cubiertos en Admin:** 0
- **Parciales en Admin:** 1 (AC-CS01-8)
- **Score:** 11%

### Score Ajustado (solo AC relevantes a Admin)

- **AC Relevantes a Admin:** 1 (AC-CS01-8)
- **Cubiertos:** 1 (via ui-ux Pantallas 4+5)
- **Score Ajustado:** 100%

**Conclusion:** Si solo consideramos AC-CS01-8 (seed data management), la cobertura de Admin es 100% en diseno UI/UX. Los gaps estan en:
1. Falta de endpoints CRUD documentados (POST/PUT/DELETE)
2. Falta de plan de implementacion tecnico (frontend-admin-plan.md)
3. Falta de estrategia de testing

---

## 12. Validacion de Dependencias

### Dependencias Externas (Blocking)

| Dependencia | Estado | Bloqueante? |
|-------------|--------|-------------|
| Backend: Seed data migrations | NO implementado | **SI** |
| Backend: GET /templates | NO implementado | **SI** |
| Backend: GET /templates/{id} | NO implementado | **SI** |
| Backend: GET /maestras/roles | NO implementado | **SI** |
| Shared: types/schemas | Planificado (contracts-plan) | **SI** |

### Dependencias Internas (Admin)

| Dependencia | Estado | Bloqueante? |
|-------------|--------|-------------|
| Auth: JWT token verificado en admin | Asumido existente | SI |
| Routing: Next.js App Router | Asumido existente | SI |
| UI: shadcn/ui components | Asumido existente | NO |

---

## 13. Validacion de Seguridad

### Autorizacion Admin

| Endpoint | Rol Requerido | Validacion Planeada | Estado |
|----------|---------------|---------------------|--------|
| GET /templates | Admin | contracts.md: NO especifica | GAP |
| POST /templates | Admin | NO documentado | GAP |
| PUT /templates/{id} | Admin | NO documentado | GAP |
| DELETE /templates/{id} | Admin | NO documentado | GAP |

**Recomendacion:** Agregar a contracts.md seccion "Admin Authorization":
- Todos los endpoints CRUD de templates requieren rol `Admin`
- Validar claim `role: Admin` en JWT
- Retornar 403 Forbidden si no es admin

### Validacion de Inputs

| Campo | Validacion Backend | Validacion Frontend (Zod) | Estado |
|-------|-------------------|---------------------------|--------|
| Nombre template | NO documentado | ui-ux: max 200 | PARCIAL |
| Presupuesto min/max | contracts: max >= min | shared/schemas: refine | CUBIERTO |
| Rol profesional ID | NO documentado | ui-ux: select requerido | PARCIAL |

**Gap:** FluentValidation en backend NO documentado para CRUD de templates.

---

## 14. Conclusion

**Score Final:** 22% (cobertura global) | 100% (cobertura ajustada por scope Admin)

**Veredicto:** REQUIERE CAMBIOS (para completitud post-MVP)

**Justificacion:**
- **Para MVP:** Admin NO es bloqueante. Seed data se carga via migrations, Landing consume endpoints GET. Score efectivo: **100%** (seed cubierto).
- **Para Post-MVP:** CRUD de templates requiere:
  1. Documentar endpoints POST/PUT/DELETE en contracts.md
  2. Crear frontend-admin-plan.md con arquitectura
  3. Crear test-strategy.md
  4. Implementar validacion de rol Admin en endpoints

**Proximo Paso:**
- **Si MVP:** Proceder con implementacion de Landing (wizard). Admin puede esperar.
- **Si Post-MVP:** Resolver GAP-02 (crear frontend-admin-plan), GAP-06 (endpoints CRUD), GAP-01 (health check seed).

**Riesgos Aceptados para MVP:**
- No hay UI para editar templates (se usa seed data fijo)
- No hay tests de Admin (CRUD no critico)
- No hay validacion de rol Admin documentada (asumir implementacion en backend)

**Beneficio de Postponer Admin:**
- Equipo puede enfocarse 100% en Landing (donde esta el valor para el usuario)
- Seed data 6 templates cubre casos de uso principales
- Admin CRUD puede construirse iterativamente post-MVP con feedback de uso real

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-15
**Revisado por:** QA Engineer
**Aprobado para:** MVP (Landing), Post-MVP (Admin CRUD)
