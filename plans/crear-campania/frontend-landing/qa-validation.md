# Validacion QA: Crear Campania (Landing)

**Fecha:** 2026-02-12
**Feature:** crear-campania
**Target:** src/web (Landing publica)

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 9 |
| Cubiertos | 0 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 9 |
| **Score de Cobertura** | **0%** |

**Estado:** RECHAZADO

**Razon:** No existen planes de implementacion para Landing. Se requiere crear frontend-plan.md, ui-design.md y test-strategy.md para la landing publica antes de proceder.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo |
|----|----------|------|
| AC-02-7 | Campania en estado BORRADOR NO es visible en landing page al listar campanias publicas (GET `/api/campanias` debe filtrar por estado PUBLICADA) | Funcional |
| AC-02-8 | Campania en estado PUBLICADA es visible en landing page en `/campanias/{id}` sin requerir autenticacion | Funcional |
| NFR-01 | Pagina de detalle responsive (mobile, tablet, desktop) | No Funcional |
| NFR-02 | Accesibilidad WCAG AA (contraste minimo 4.5:1, keyboard navigation, ARIA labels) | No Funcional |
| NFR-03 | Tiempo de carga inicial < 3s | No Funcional |
| NFR-04 | Imagenes optimizadas (lazy loading, responsive images) | No Funcional |
| NFR-05 | SEO optimizado (meta tags, open graph, schema.org) | No Funcional |
| NFR-06 | Error 404 si campania no existe o esta en borrador | Funcional |
| NFR-07 | Skeleton loaders durante carga de datos | No Funcional |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|---------------|-----------|---------------|--------|
| AC-02-7 | Campania BORRADOR NO visible en landing | NO EXISTE | contracts.md line 239: filtra publicadas por defecto | NO EXISTE | NO CUBIERTO |
| AC-02-8 | Campania PUBLICADA visible sin auth | NO EXISTE | ui-ux.md lines 719-788: detalle campania publica | NO EXISTE | PARCIAL |
| NFR-06 | Error 404 si no existe o borrador | NO EXISTE | ui-ux.md line 851: "404 Not Found" | NO EXISTE | PARCIAL |

**Leyenda:**
- CUBIERTO: Requisito completamente implementado en plan
- PARCIAL: Requisito parcialmente cubierto (solo diseño UI, sin implementacion)
- NO CUBIERTO: Requisito no mencionado en planes
- N/A: No aplica a este plan

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile, tablet, desktop | ui-ux.md lines 869-875, 986-1013 | PARCIAL |
| NFR-02 | Accesibilidad WCAG AA | ui-ux.md lines 1051-1114 | PARCIAL |
| NFR-03 | Tiempo de carga < 3s | - | NO CUBIERTO |
| NFR-04 | Imagenes optimizadas (lazy loading) | - | NO CUBIERTO |
| NFR-05 | SEO (meta tags, schema.org) | - | NO CUBIERTO |
| NFR-07 | Skeleton loaders | ui-ux.md line 844: "Skeleton loaders" | PARCIAL |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-01 | AC-02-7 | No existe implementacion para filtrar campanias por estado PUBLICADA en landing | Alto | Crear servicio `campanias.service.ts` con GET `/api/campanias?estadoCampaniaId=2` |
| GAP-02 | AC-02-8 | No existe implementacion para pagina detalle `/campanias/{id}` en web/ | Alto | Crear componente `CampaniaDetailPage.tsx` en `src/web/src/pages/campanias/[id].tsx` |
| GAP-03 | NFR-06 | No existe manejo de error 404 para campanias no encontradas o en borrador | Alto | Agregar validacion en servicio: si estadoCampaniaId !== 2 → throw NotFoundError |
| GAP-04 | PLAN | No existe frontend-plan.md para Landing | Critico | Crear plan de implementacion con arquitectura, componentes, hooks y services |
| GAP-05 | PLAN | No existe test-strategy.md para Landing | Critico | Crear estrategia de testing con unit, integration y e2e tests |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-06 | NFR-01 | Responsive design especificado en UI pero sin plan de implementacion | Medio | Documentar breakpoints y estrategia mobile-first en frontend-plan |
| GAP-07 | NFR-03 | No hay plan para optimizacion de rendimiento | Medio | Agregar code splitting, lazy loading de componentes en frontend-plan |
| GAP-08 | NFR-04 | No hay plan para optimizacion de imagenes | Medio | Usar next/image o react-lazy-load-image-component |
| GAP-09 | NFR-05 | No hay plan para SEO | Medio | Agregar react-helmet-async para meta tags, Open Graph y schema.org |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-10 | NFR-07 | Skeleton loaders especificados pero sin libreria definida | Bajo | Usar shadcn/ui Skeleton component o react-loading-skeleton |
| GAP-11 | UI-UX | Animaciones definidas pero sin libreria | Bajo | Usar framer-motion o CSS animations nativas |
| GAP-12 | DOCS | Falta documentacion de routing para landing | Bajo | Documentar react-router-dom routes en frontend-plan |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-02-7 | - | - | NO CUBIERTO |
| AC-02-8 | - | - | NO CUBIERTO |
| NFR-06 | - | - | NO CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-02-7 | test-campanias-list-filters-drafts | Validar que GET /api/campanias NO retorna campanias borrador |
| AC-02-8 | test-campania-detail-public-access | Validar que usuario no autenticado puede ver detalle de campania publicada |
| NFR-06 | test-campania-404-not-found | Validar que campania inexistente retorna 404 |
| NFR-06 | test-campania-404-draft | Validar que campania borrador retorna 404 para usuarios publicos |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| Lista Campanias Publicas (`/explorar`) | ui-ux.md NO incluye explorar | - | NO CUBIERTO |
| Detalle Campania Publica (`/campanias/{id}`) | ui-ux.md lines 719-788 | CampaniaDetailPage, components UI | PARCIAL (solo diseño) |
| Error 404 | ui-ux.md line 851: mencionado | NotFoundPage | NO CUBIERTO |

### Estados de UI

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading | Si | ui-ux.md line 844: Skeleton loaders | PARCIAL |
| Error | Si | ui-ux.md line 851: 404 | PARCIAL |
| Empty State (sin campanias) | No | - | N/A |
| Campaign Ended | Si | ui-ux.md line 846: "Campaña finalizada" | CUBIERTO |
| Funded 100% | Si | ui-ux.md line 845: confetti animation | CUBIERTO |

---

## 7. Validacion de Contratos API

### Endpoints Necesarios para Landing

| Endpoint | Contrato Definido | Validacion Frontend | Estado |
|----------|-------------------|---------------------|--------|
| GET `/api/campanias` | contracts.md lines 234-278 | - | PARCIAL (contrato OK, falta implementacion) |
| GET `/api/campanias/{id}` | contracts.md lines 95-133 | - | PARCIAL (contrato OK, falta implementacion) |

### Validacion de Filtros

| Filtro | Backend Contract | Frontend Plan | Estado |
|--------|------------------|---------------|--------|
| `estadoCampaniaId` | contracts.md line 243: opcional | - | NO CUBIERTO |
| Default filter publicadas | contracts.md line 238: "filtra solo campanias publicadas por defecto" | - | NO CUBIERTO |
| `searchTerm` | contracts.md line 241 | - | NO CUBIERTO |
| `artistaId` | contracts.md line 242 | - | NO CUBIERTO |

**NOTA CRITICA:** El contrato especifica que GET `/api/campanias` filtra campanias PUBLICADAS por defecto, pero no especifica si esto es backend-side o frontend-side. **DEBE clarificarse** en contracts.md o backend-plan.

---

## 8. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear frontend-plan.md**
   - Archivo: `plans/crear-campania/frontend-landing/frontend-plan.md`
   - Cambio: Definir arquitectura, componentes, hooks, services, routing para:
     - `/explorar` - Lista de campanias publicas
     - `/campanias/{id}` - Detalle de campania publica
     - Error handling y 404 pages

2. **Crear test-strategy.md**
   - Archivo: `plans/crear-campania/frontend-landing/test-strategy.md`
   - Cambio: Definir tests para AC-02-7, AC-02-8, NFR-06

3. **Clarificar filtrado de campanias en backend**
   - Archivo: `docs/user-stories/crear-campania/contracts.md` line 238
   - Cambio: Especificar explicitamente si GET `/api/campanias` SIN parametros filtra estado PUBLICADA por defecto en BACKEND o si frontend debe enviar `?estadoCampaniaId=2`

4. **Implementar servicio campanias.service.ts**
   - Archivo: `src/web/src/services/campanias.service.ts`
   - Cambio: Crear service con metodos:
     - `getAll(filters?)` → GET `/api/campanias`
     - `getById(id)` → GET `/api/campanias/{id}`

5. **Implementar pagina detalle campania**
   - Archivo: `src/web/src/pages/campanias/[id].tsx`
   - Cambio: Implementar layout segun ui-ux.md lines 719-788

### Acciones Sugeridas (Mayor)

1. **Implementar lista de campanias publicas**
   - Archivo: `src/web/src/pages/explorar.tsx`
   - Cambio: Crear pagina de listado con filtros (search, artista, etc.)

2. **Agregar SEO optimization**
   - Archivo: `src/web/src/components/SEO.tsx`
   - Cambio: Usar react-helmet-async para meta tags dinamicos por campania

3. **Optimizacion de imagenes**
   - Archivo: `src/web/vite.config.ts`
   - Cambio: Configurar vite-plugin-imagemin para optimizacion automatica

4. **Code splitting por ruta**
   - Archivo: `src/web/src/app/router.tsx`
   - Cambio: Usar React.lazy() para pages

### Nice to Have (Menor)

1. **Implementar confetti animation**
   - Libreria: `react-confetti` o `canvas-confetti`

2. **Agregar analytics tracking**
   - Libreria: `react-ga4` para Google Analytics

3. **Implementar share buttons**
   - Componente: ShareButtons para compartir en redes sociales

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [ ] AC-02-7: Campanias borrador NO visibles en landing
- [ ] AC-02-8: Campanias publicadas visibles sin auth
- [ ] NFR-06: Error 404 para campanias inexistentes o borrador

### UI/UX
- [ ] Pantalla `/explorar` planificada
- [ ] Pantalla `/campanias/{id}` planificada
- [ ] Pagina 404 planificada
- [ ] Estados de UI definidos (loading, error, success)
- [ ] Responsive design implementable
- [ ] Accesibilidad validada

### Contratos API
- [ ] Contrato GET `/api/campanias` cumple requisitos
- [ ] Contrato GET `/api/campanias/{id}` cumple requisitos
- [ ] Filtrado por estado clarificado (backend vs frontend)
- [ ] Error handling especificado

### Testing
- [ ] Tests para AC-02-7 planificados
- [ ] Tests para AC-02-8 planificados
- [ ] Tests para NFR-06 planificados
- [ ] Cobertura objetivo definida (>80%)

### Performance
- [ ] Estrategia de code splitting
- [ ] Lazy loading de imagenes
- [ ] Skeleton loaders durante carga
- [ ] Tiempo de carga < 3s objetivo

### SEO
- [ ] Meta tags dinamicos
- [ ] Open Graph tags
- [ ] Schema.org markup
- [ ] Sitemap inclusion

---

## 10. Analisis de Dependencias

### Dependencias de Planes

| Plan Faltante | Bloqueante Para | Razon |
|---------------|-----------------|-------|
| frontend-plan.md (Landing) | Implementacion de UI | Define arquitectura y componentes |
| test-strategy.md (Landing) | Validacion de funcionalidad | Define cobertura de tests |
| backend-plan.md | Clarificar filtrado default | Necesario para saber si frontend debe enviar filtro explicitamente |

### Dependencias de Shared

| Shared Asset | Estado | Accion |
|--------------|--------|--------|
| `Campania` type | contracts-plan.md COMPLETO | OK - Usar type |
| `CampaniaListItem` type | contracts-plan.md COMPLETO | OK - Usar type |
| `CAMPANIA_ESTADOS` | contracts-plan.md COMPLETO | OK - Usar constante |
| `API_ROUTES.campanias` | contracts-plan.md COMPLETO | OK - Usar constante |
| `QUERY_KEYS.campanias` | contracts-plan.md COMPLETO | OK - Usar constante |

### Dependencias de Backend

| Backend Feature | Estado | Impacto en Landing |
|-----------------|--------|-------------------|
| GET `/api/campanias` endpoint | Asumido PENDIENTE | Landing NO puede listar campanias sin este endpoint |
| GET `/api/campanias/{id}` endpoint | Asumido PENDIENTE | Landing NO puede mostrar detalle sin este endpoint |
| Filtrado por `estadoCampaniaId` | Asumido PENDIENTE | Landing depende de este filtro para AC-02-7 |

---

## 11. Escenarios de Test Criticos

### Escenario 1: Usuario no autenticado lista campanias

**Given:** Usuario no autenticado accede a `/explorar`
**When:** GET `/api/campanias` se ejecuta
**Then:**
- Solo retorna campanias con `estadoCampaniaId = 2` (PUBLICADA)
- NO retorna campanias con `estadoCampaniaId = 1` (BORRADOR)
- Status 200 OK
- Response incluye array de `CampaniaListItem`

**Validacion:**
- [ ] Frontend NO debe enviar filtro adicional (backend filtra por defecto)
- [ ] O frontend DEBE enviar `?estadoCampaniaId=2` explicitamente

**GAP:** Contrato NO especifica cual de las dos opciones es la correcta

### Escenario 2: Usuario no autenticado accede a campania publicada

**Given:** Usuario no autenticado
**When:** Accede a `/campanias/{id}` donde campania tiene `estadoCampaniaId = 2`
**Then:**
- Backend retorna status 200 OK
- Response incluye `Campania` completa
- Frontend renderiza detalle sin auth

**Validacion:**
- [x] Contrato especifica endpoint publico (NO auth)
- [ ] Plan de implementacion de pagina detalle

### Escenario 3: Usuario no autenticado accede a campania borrador

**Given:** Usuario no autenticado
**When:** Accede a `/campanias/{id}` donde campania tiene `estadoCampaniaId = 1`
**Then:**
- Backend retorna status 404 Not Found (o 403 Forbidden?)
- Frontend muestra pagina 404

**Validacion:**
- [ ] Contrato NO especifica comportamiento para este caso
- [ ] Backend plan debe clarificar: 404 o 403?

**RECOMENDACION:** Retornar 404 para ocultar existencia de campania borrador

### Escenario 4: Usuario accede a campania inexistente

**Given:** Usuario accede a `/campanias/{id}` con ID que no existe
**When:** Backend busca campania
**Then:**
- Backend retorna status 404 Not Found
- Frontend muestra pagina 404

**Validacion:**
- [x] Contrato especifica error 404 (contracts.md line 132)
- [ ] Frontend plan debe especificar componente NotFoundPage

---

## 12. Conclusion

**Score Final:** 0%

**Veredicto:** RECHAZADO

**Razones:**

1. **No existen planes de implementacion para Landing:**
   - Falta `frontend-plan.md` con arquitectura, componentes y services
   - Falta `test-strategy.md` con estrategia de testing

2. **Contratos API incompletos:**
   - GET `/api/campanias` NO especifica si filtrado por PUBLICADA es default backend-side
   - Falta especificar comportamiento para acceso a campanias BORRADOR (404 vs 403)

3. **UI/UX diseñado pero sin plan de implementacion:**
   - Diseño de detalle campania existe en ui-ux.md
   - NO existe diseño de lista campanias (`/explorar`)
   - Falta plan de componentes y arquitectura

4. **Criterios de aceptacion NO validables:**
   - AC-02-7: NO se puede validar sin plan de implementacion de servicio
   - AC-02-8: Solo diseño UI, falta plan de componente y routing

**Proximo Paso:**

**BLOQUEANTE:** NO se puede proceder a implementacion hasta:

1. Crear `plans/crear-campania/frontend-landing/frontend-plan.md`
2. Crear `plans/crear-campania/frontend-landing/test-strategy.md`
3. Clarificar en `docs/user-stories/crear-campania/contracts.md`:
   - Filtrado default de GET `/api/campanias`
   - Comportamiento de GET `/api/campanias/{id}` para campanias borrador

**RECOMENDACION:** Ejecutar agente `frontend-planner` para Landing antes de validacion QA.

---

## 13. Comparacion con Admin Dashboard

### Admin tiene planes completos (asumido)

Si Admin Dashboard tiene planes para:
- Wizard crear campania (4 steps)
- Lista mis campanias
- Preview campania borrador

Entonces Landing debe tener equivalente para:
- Lista campanias publicas (`/explorar`)
- Detalle campania publica (`/campanias/{id}`)
- Error 404

**IMPORTANTE:** Landing es mas simple que Admin pero CRITICO para AC-02-7 y AC-02-8.

### Aprendizajes de Admin (si existe plan)

Reutilizar de Admin:
- Componentes UI de shadcn/ui (Card, Button, Progress, etc.)
- Servicios compartidos (campanias.service.ts puede ser shared)
- Tipos y schemas de `src/shared/`
- Mismos query keys de React Query

**NO reutilizar:**
- Componentes de wizard (Landing no crea campanias)
- Componentes de edicion (Landing es read-only)
- Auth guards (Landing es publico)

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-12
**Status:** RECHAZADO - Requiere planes de implementacion antes de validacion
