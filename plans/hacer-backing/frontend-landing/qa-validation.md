# Validacion QA: Hacer Backing (Landing)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/web

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 10 |
| Cubiertos | 2 |
| Parcialmente Cubiertos | 4 |
| No Cubiertos | 4 |
| **Score de Cobertura** | **20%** |

**Estado:** ⛔ **RECHAZADO**

### Analisis de Situacion

Actualmente solo existe el plan de contratos shared (`plans/hacer-backing/shared/contracts-plan.md`). **No existen planes de implementacion para:**

- ❌ Frontend Landing (`frontend-plan.md`)
- ❌ UI Design (`ui-design.md`)
- ❌ Test Strategy (`test-strategy.md`)
- ❌ Backend Implementation

El plan de contratos shared cubre parcialmente algunos criterios de aceptacion a nivel de definicion de tipos y esquemas de validacion, pero **no hay planes concretos de implementacion** que cubran los flujos de usuario, componentes UI, queries/mutations, o logica de negocio en backend.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo |
|----|----------|------|
| AC-04-1 | Solo campanias con estado PUBLICADA o EN_CURSO son visibles en listado publico | Funcional |
| AC-04-2 | El monto ingresado debe ser >= ImporteMinimo del reward seleccionado | Funcional |
| AC-04-3 | Al crear backing, ImportePledgedActual se actualiza sumando el monto | Funcional |
| AC-04-4 | Al crear backing con reward, CantidadVendida del reward se incrementa | Funcional |
| AC-04-5 | No se puede seleccionar reward si CantidadDisponible = 0 | Funcional |
| AC-04-6 | Backers que marcan "Anonimo" no muestran nombre en lista publica | Funcional |
| AC-04-7 | "Apoyar sin recompensa" permite cualquier monto > 0 sin seleccionar reward | Funcional |
| AC-04-8 | Transaccion atomica (PedidoCrowdfunding + lineas + updates) | No Funcional (Integridad) |
| AC-04-9 | Usuarios anonimos pueden hacer backings con UserId = null | Funcional |
| AC-04-10 | Lista de campanias paginada con 12 items por defecto | Funcional |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | contracts-plan | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|----------------|---------------|-----------|---------------|--------|
| AC-04-1 | Solo campanias PUBLICADA/EN_CURSO visibles | ❌ | ❌ | ❌ | ❌ | NO CUBIERTO |
| AC-04-2 | Monto >= ImporteMinimo del reward | ✅ Zod schema + validators | ❌ | ❌ | ❌ | PARCIAL |
| AC-04-3 | ImportePledgedActual se actualiza | ❌ | ❌ | ❌ | ❌ | NO CUBIERTO |
| AC-04-4 | CantidadVendida del reward se incrementa | ❌ | ❌ | ❌ | ❌ | NO CUBIERTO |
| AC-04-5 | Reward con stock 0 no seleccionable | ⚠️ validateRewardAvailability | ❌ | ❌ | ❌ | PARCIAL |
| AC-04-6 | Anonimos no muestran nombre | ✅ getBackerDisplayName helper | ❌ | ❌ | ❌ | PARCIAL |
| AC-04-7 | "Apoyar sin recompensa" con monto > 0 | ⚠️ rewardId optional en schema | ❌ | ❌ | ❌ | PARCIAL |
| AC-04-8 | Transaccion atomica (backend) | ❌ | N/A | N/A | ❌ | NO CUBIERTO |
| AC-04-9 | Usuarios anonimos (UserId null) | ✅ userId optional en types | ❌ | ❌ | ❌ | PARCIAL |
| AC-04-10 | Paginacion 12 items default | ❌ | ❌ | ❌ | ❌ | NO CUBIERTO |

**Leyenda:**
- ✅ CUBIERTO: Requisito completamente implementado en plan
- ⚠️ PARCIAL: Requisito parcialmente cubierto (definicion de tipos/schemas, pero sin implementacion)
- ❌ NO CUBIERTO: Requisito no mencionado en planes
- N/A: No aplica a este plan

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile | ui-design: breakpoints definidos en spec | NO CUBIERTO (sin plan UI) |
| NFR-02 | Accesibilidad WCAG AA | ui-design: aria labels + keyboard nav en spec | NO CUBIERTO (sin plan UI) |
| NFR-03 | Tiempo de carga < 3s | - | NO CUBIERTO |
| NFR-04 | Validacion client-side antes de submit | ✅ contracts-plan: Zod + validators | PARCIAL |
| NFR-05 | Manejo de errores con mensajes traducidos | ✅ contracts-plan: BACKING_ERROR_MESSAGES | PARCIAL |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-04-1 | Filtrado de campanias por estado | No hay plan de query para GET /api/campanias con filtro estadoCampaniaId | Alto | Crear frontend-plan con hook `useCampanias` que incluya filtro por estadoCampaniaId=2 |
| AC-04-3 | Actualizacion de ImportePledgedActual | No hay plan de backend para transaction atomica | Critico | Crear backend-plan con CreateBackingCommand que ejecute update en CampaniaCrowdfunding |
| AC-04-4 | Incremento de CantidadVendida | No hay plan de backend para actualizar Reward | Critico | Incluir en CreateBackingCommand update de CampaniaCrowdfundingReward.CantidadVendida |
| AC-04-8 | Transaccion atomica | No hay plan de backend con strategy de transaccion | Critico | Implementar Unit of Work o DbContext.Transaction en CreateBackingCommandHandler |
| AC-04-10 | Paginacion default 12 items | No hay plan de componente de listado con paginacion | Alto | Crear frontend-plan con CampaniasListPage que use pageSize=12 default |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-04-2 | Validacion monto >= minimo | Solo schema Zod, falta integracion con formulario | Medio | Crear frontend-plan con BackingForm que use validateBackingAmount en submit |
| AC-04-5 | UI deshabilita reward sin stock | Solo validator, falta UI que consuma campo `disponible` | Medio | Crear ui-design con RewardCard disabled si reward.disponible === false |
| AC-04-7 | Boton "Apoyar sin recompensa" | Schema permite rewardId null, pero falta UI del boton | Medio | Crear ui-design con card especial "Sin recompensa" que setea rewardId a null |
| NFR-01 | Responsive design | Specs de UI existen en ui-ux.md, falta plan de implementacion | Medio | Crear ui-design con breakpoints mobile/tablet/desktop |
| NFR-02 | Accesibilidad | Specs de aria labels en ui-ux.md, falta plan de implementacion | Medio | Crear ui-design con checklist de aria attributes por componente |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-04-6 | Display de nombre anonimo | Helper existe, falta documentar uso en componentes | Bajo | Agregar en frontend-plan: BackingsRecentesList usa getBackerDisplayName |
| AC-04-9 | Backings anonimos | Tipo permite userId null, falta flujo de UI sin autenticacion | Bajo | Documentar en frontend-plan: BackingForm no requiere auth si campania.permiteAportacionesAnonimas |
| NFR-03 | Performance < 3s | No hay plan de optimizacion | Bajo | Agregar en test-strategy: performance test con Lighthouse |
| NFR-04 | Validacion client-side | Schemas definidos, falta integracion con react-hook-form | Bajo | Agregar en frontend-plan: BackingForm usa zodResolver(createBackingSchema) |
| NFR-05 | Error messages | Diccionario existe, falta integracion con toast | Bajo | Agregar en frontend-plan: useCreateBacking onError usa getBackingErrorMessage |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

**ESTADO:** ❌ No existe plan de testing

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-04-1 | - | - | NO CUBIERTO |
| AC-04-2 | - | - | NO CUBIERTO |
| AC-04-3 | - | - | NO CUBIERTO |
| AC-04-4 | - | - | NO CUBIERTO |
| AC-04-5 | - | - | NO CUBIERTO |
| AC-04-6 | - | - | NO CUBIERTO |
| AC-04-7 | - | - | NO CUBIERTO |
| AC-04-8 | - | - | NO CUBIERTO |
| AC-04-9 | - | - | NO CUBIERTO |
| AC-04-10 | - | - | NO CUBIERTO |

### Tests Faltantes

**Todos los criterios requieren tests:**

#### Tests Backend (xUnit)

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-04-1 | `GetAllCampaniasQueryHandler_DefaultFilter_ReturnsOnlyPublicadas` | Validar filtro por estadoCampaniaId=2 |
| AC-04-2 | `CreateBackingValidator_MontoMenorAlMinimo_ReturnsValidationError` | Validar monto >= reward.ImporteMinimo |
| AC-04-3 | `CreateBackingHandler_ValidBacking_UpdatesCampaniaImportePledged` | Validar actualizacion atomica de campania |
| AC-04-4 | `CreateBackingHandler_BackingWithReward_IncrementsRewardCantidadVendida` | Validar incremento de stock vendido |
| AC-04-5 | `CreateBackingValidator_RewardSinStock_ReturnsBusinessRuleError` | Validar error 4011 cuando stock = 0 |
| AC-04-6 | `GetBackingsRecentesQuery_BackerAnonimo_ReturnsMaskedName` | Validar que nombre no se expone si esAnonimo |
| AC-04-7 | `CreateBackingValidator_SinReward_AllowsAnyAmount` | Validar que rewardId null permite monto > 0 sin minimo |
| AC-04-8 | `CreateBackingHandler_DatabaseFailure_RollbacksTransaction` | Validar rollback completo en error |
| AC-04-9 | `CreateBackingValidator_UserIdNull_AllowsAnonymousBacking` | Validar que userId null es valido si permitido |
| AC-04-10 | `GetAllCampaniasQueryHandler_DefaultPageSize_Returns12Items` | Validar paginacion default |

#### Tests Frontend (Vitest + Testing Library)

| Criterio | Test Requerido | Razon |
|----------|----------------|-------|
| AC-04-1 | `CampaniasListPage_Mount_FetchesOnlyActiveCampaigns` | Validar query con estadoCampaniaId=2 |
| AC-04-2 | `BackingForm_MontoMenorAlMinimo_ShowsValidationError` | Validar mensaje inline de error |
| AC-04-5 | `RewardCard_StockZero_DisablesSelectButton` | Validar UI deshabilita reward agotado |
| AC-04-6 | `BackingsRecentesList_AnonymousBacker_DisplaysAnonimo` | Validar uso de getBackerDisplayName |
| AC-04-7 | `BackingForm_SinRecompensaSelected_AllowsAnyAmount` | Validar monto sin minimo cuando reward null |
| AC-04-9 | `BackingForm_UserNotAuthenticated_AllowsSubmit` | Validar submit sin auth si permitido |
| AC-04-10 | `CampaniasListPage_Pagination_Loads12ItemsPerPage` | Validar grid muestra 12 cards por pagina |

#### Tests E2E (Playwright)

| Flujo | Test Requerido | Criterios Cubiertos |
|-------|----------------|---------------------|
| Flujo Principal | `test_backing_flow_authenticated_user` | AC-04-1, AC-04-2, AC-04-3, AC-04-4, AC-04-5 |
| Flujo Anonimo | `test_backing_flow_anonymous_user` | AC-04-6, AC-04-9 |
| Flujo Sin Recompensa | `test_backing_without_reward` | AC-04-7 |
| Paginacion | `test_campaigns_list_pagination` | AC-04-10 |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

**ESTADO:** ❌ No existe plan de UI

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| Campaign Listing (`/campanias`) | ❌ | - | NO CUBIERTO |
| Campaign Detail (`/campanias/{id}`) | ❌ | - | NO CUBIERTO |
| Backing Form (`/campanias/{id}/backing`) | ❌ | - | NO CUBIERTO |
| Backing Confirmation | ❌ | - | NO CUBIERTO |
| My Backings Dashboard | ❌ | - | NO CUBIERTO |

### Estados de UI

**ESTADO:** ❌ No existe plan de UI

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading (campanias list) | Si | ❌ | NO CUBIERTO |
| Empty State (no campanias) | Si | ❌ | NO CUBIERTO |
| Error (fetch failed) | Si | ❌ | NO CUBIERTO |
| Success (backing created) | Si | ❌ | NO CUBIERTO |
| Validation Error (form) | Si | ❌ | NO CUBIERTO |
| Reward Stock Depleted | Si | ❌ | NO CUBIERTO |
| Campaign Expired | Si | ❌ | NO CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear `frontend-plan.md`**
   - Archivo: `plans/hacer-backing/frontend-landing/frontend-plan.md`
   - Cambio: Planificar componentes, hooks, services para:
     - `CampaniasListPage`: Grid con filtro estadoCampaniaId=2, paginacion 12 items
     - `CampaniaDetailPage`: Detalle con rewards sidebar, backings recientes
     - `BackingForm`: Formulario con validacion Zod, integration con react-hook-form
     - `BackingConfirmationPage`: Pantalla de exito post-backing
     - Hooks: `useCampanias`, `useCampaniaDetail`, `useCreateBacking`
     - Services: `campania.service.ts`, `backing.service.ts`

2. **Crear `backend-plan.md`**
   - Archivo: `plans/hacer-backing/backend/backend-plan.md`
   - Cambio: Planificar CQRS implementation:
     - `CreateBackingCommand`: Transaction atomica con updates de Campania + Reward
     - `CreateBackingCommandValidator`: Validaciones de monto, stock, estado campania
     - `GetAllCampaniasQuery`: Filtro por estadoCampaniaId con default=2
     - `GetCampaniaDetailQuery`: Join con rewards + backings recientes
     - `CampaniaStatsQuery`: Agregaciones de backings
     - Controllers: POST `/api/campanias/{id}/backings`, GET endpoints

3. **Crear `ui-design.md`**
   - Archivo: `plans/hacer-backing/frontend-landing/ui-design.md`
   - Cambio: Documentar componentes UI basados en `ui-ux.md`:
     - Campaign cards con progress bars, badges
     - Reward cards con stock indicator, disabled state
     - Backing form fields con validacion inline
     - Empty states, loading skeletons
     - Responsive breakpoints (mobile/tablet/desktop)
     - Accesibilidad (aria labels, keyboard nav)

4. **Crear `test-strategy.md`**
   - Archivo: `plans/hacer-backing/frontend-landing/test-strategy.md`
   - Cambio: Planificar tests para todos los AC:
     - Unit tests (Vitest): schemas, validators, utils
     - Component tests (Testing Library): forms, cards, lists
     - Integration tests: hooks con API mocks
     - E2E tests (Playwright): flujo completo de backing
     - Cobertura objetivo: 80%+

### Acciones Sugeridas (Mayor)

1. **Extender `contracts-plan.md`**
   - Archivo: `plans/hacer-backing/shared/contracts-plan.md`
   - Cambio: Agregar ejemplos de uso de validators:
     - Ejemplo: `validateBackingAmount` en BackingForm.onSubmit
     - Ejemplo: `getBackerDisplayName` en BackingsRecentesList

2. **Crear plan de integracion con Stripe**
   - Archivo: `plans/hacer-backing/payment-integration.md`
   - Cambio: Documentar flujo de Stripe Checkout (mencionado en ui-ux.md):
     - Endpoint `POST /api/backings/create-checkout-session`
     - Webhook handler `/api/webhooks/stripe`
     - Estados de pedido (PENDIENTE → COMPLETADO)

3. **Documentar flujos alternativos**
   - Archivo: `plans/hacer-backing/frontend-landing/error-handling.md`
   - Cambio: Planificar manejo de cada flujo alternativo (FA-01 a FA-07):
     - FA-01: Monto < minimo → error inline
     - FA-02: Stock agotado → disable reward UI
     - FA-03: Campania finalizada → redirect + toast
     - FA-04: Usuario no autenticado → modal login
     - FA-05: Error transaccion → rollback + retry
     - FA-06: Estado cambia durante form → validacion al submit
     - FA-07: Race condition stock → optimistic locking

### Nice to Have (Menor)

1. **Agregar diagramas de flujo**
   - Archivo: `plans/hacer-backing/diagrams/backing-flow.mermaid`
   - Cambio: Crear diagrama de flujo principal (paso a paso)

2. **Documentar reglas de negocio**
   - Archivo: `plans/hacer-backing/business-rules.md`
   - Cambio: Centralizar logica como:
     - Calculo de `diasRestantes`
     - Calculo de `porcentajeProgreso`
     - Calculo de `disponible` (reward)

3. **Crear checklist de revision de codigo**
   - Archivo: `plans/hacer-backing/code-review-checklist.md`
   - Cambio: Checklist especifico de backing para PRs

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [ ] AC-04-1: Campanias con filtro de estado (NO CUBIERTO)
- [ ] AC-04-2: Validacion de monto minimo (PARCIAL - schema definido, falta UI)
- [ ] AC-04-3: Actualizacion de ImportePledgedActual (NO CUBIERTO)
- [ ] AC-04-4: Incremento de CantidadVendida (NO CUBIERTO)
- [ ] AC-04-5: Reward sin stock no seleccionable (PARCIAL - validator definido, falta UI)
- [ ] AC-04-6: Display de nombre anonimo (PARCIAL - helper definido, falta uso)
- [ ] AC-04-7: Apoyo sin recompensa (PARCIAL - schema permite, falta UI)
- [ ] AC-04-8: Transaccion atomica (NO CUBIERTO)
- [ ] AC-04-9: Backings anonimos (PARCIAL - tipo permite, falta flujo)
- [ ] AC-04-10: Paginacion 12 items (NO CUBIERTO)

### UI/UX
- [ ] Todas las screens planificadas (0/5 screens)
- [ ] Estados de interaccion definidos (0/7 estados)
- [ ] Responsive design considerado (NO CUBIERTO)
- [ ] Accesibilidad validada (NO CUBIERTO)

### Testing
- [ ] Tests para criterios criticos (0/10 tests backend)
- [ ] Tests de integracion para flujos (0/7 tests frontend)
- [ ] Cobertura objetivo definida (NO CUBIERTO)

### Documentacion
- [ ] Frontend plan existe (NO)
- [ ] Backend plan existe (NO)
- [ ] UI design plan existe (NO)
- [ ] Test strategy existe (NO)

---

## 9. Conclusion

**Score Final:** 20%

**Veredicto:** ⛔ **RECHAZADO**

### Razon del Rechazo

El proyecto **NO puede proceder a implementacion** porque:

1. **Solo 1 de 4 planes requeridos existe** (contracts-plan.md)
2. **80% de criterios NO estan cubiertos** en ningun plan
3. **No hay estrategia de testing** definida
4. **No hay plan de UI/UX** implementation
5. **Logica de negocio critica (backend)** sin planificar

### Impacto

- **Backend**: CreateBackingCommand (transaccion atomica), validators, queries completamente sin planificar
- **Frontend**: TODOS los componentes UI (5 screens, 15+ componentes) sin planificar
- **Testing**: 0% de tests planificados
- **Integracion**: No hay plan de integracion con Stripe Checkout

### Proximo Paso

**ACCION REQUERIDA URGENTE:**

1. **Generar planes faltantes antes de continuar:**
   - `plans/hacer-backing/backend/backend-plan.md`
   - `plans/hacer-backing/frontend-landing/frontend-plan.md`
   - `plans/hacer-backing/frontend-landing/ui-design.md`
   - `plans/hacer-backing/frontend-landing/test-strategy.md`

2. **Una vez generados los planes, re-ejecutar qa-validation** para validar cobertura completa

3. **Solo proceder a implementacion cuando score >= 90%**

### Riesgos de Implementar Sin Planes

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Logica de transaccion atomica incorrecta (data corruption) | Alta | Critico | Backend plan con strategy de transaccion + tests |
| UI no cubre todos los estados de error | Alta | Alto | UI design plan con estados completos |
| Race conditions en stock de rewards | Media | Alto | Backend plan con optimistic locking |
| Experiencia de usuario pobre (sin responsive) | Alta | Medio | UI design plan con breakpoints |
| Bugs en produccion (sin tests) | Muy Alta | Alto | Test strategy con cobertura 80%+ |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-13

**Estado de Aprobacion:** ⛔ **RECHAZADO - REQUIERE PLANES COMPLETOS**
