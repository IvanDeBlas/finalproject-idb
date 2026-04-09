# Validacion QA: cs-explorar-propuestas (Landing)

**Fecha:** 2026-02-17
**Feature:** cs-explorar-propuestas (US-CS-03)
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (AC) | 13 |
| Total Flujos Alternativos (FA) | 9 |
| Total Requisitos No Funcionales (NFR) | 9 |
| Total Items evaluados | 31 |
| Cubiertos (por contratos-plan + specs) | 24 |
| Parcialmente Cubiertos | 5 |
| No Cubiertos (frontend-plan pendiente) | 2 |
| **Score de Cobertura (AC unicamente)** | **84.6%** |
| **Score Global (AC + FA + NFR)** | **83.9%** |

**Estado: REQUIERE CAMBIOS**

> Los planes `frontend-plan.md`, `ui-design.md` y `test-strategy.md` para el target `frontend-landing` **aun no han sido generados**. Esta validacion se realiza contra las fuentes disponibles: `feature-spec.md`, `contracts.md`, `ui-ux.md` y `contracts-plan.md`. Los gaps identificados deben trasladarse como requerimientos obligatorios a los planes frontend cuando sean creados.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo |
|----|----------|------|
| AC-CS03-1 | Solo necesidades en estado `Abierta` en el listado publico | Funcional |
| AC-CS03-2 | Listado paginado (12/pagina) con filtros y debounce 300ms | Funcional |
| AC-CS03-3 | Badge urgencia si `FechaLimitePropuestas` < 3 dias | Funcional/UI |
| AC-CS03-4 | Necesidades expiradas no se muestran (filtro backend) | Funcional |
| AC-CS03-5 | No mas de una propuesta por necesidad por usuario | Funcional/Seguridad |
| AC-CS03-6 | Artista propietario no puede enviarse propuesta a si mismo | Funcional/Seguridad |
| AC-CS03-7 | Propuesta creada con estado Pendiente, UserId y PerfilProfesionalId automaticos + toast exito | Funcional |
| AC-CS03-8 | Sin PerfilProfesional muestra CTA en lugar de boton enviar | Funcional/UX |
| AC-CS03-9 | Listado mis propuestas con badges de estado semanticos (colores) | Funcional/UI |
| AC-CS03-10 | Propuesta aceptada -> "Ver acuerdo"; Pendiente -> "Retirar propuesta" | Funcional |
| AC-CS03-11 | Retirar solo en estado Pendiente con dialogo de confirmacion | Funcional |
| AC-CS03-12 | Empty state con sugerencia de ampliar filtros | UX |
| AC-CS03-13 | Detalle visible sin PerfilProfesional (`tienePerfilProfesional=false`) | Funcional |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | Cobertura en contracts-plan | Cobertura en ui-ux.md | Cobertura en test (pendiente) | Estado |
|----|----------|----------------------------|----------------------|-------------------------------|--------|
| AC-CS03-1 | Solo necesidades Abiertas en listado | `GetNecesidadesPublicasQuery` filtra `EstadoNecesidadId == 1`. Endpoint `GET /api/crowdsourcing/necesidades` documentado en contracts.md. | Pantalla 1 consume el endpoint. No se muestran necesidades en otros estados. | Sin test-strategy.md generado | CUBIERTO (logica backend + contratos definidos; falta plan de tests frontend) |
| AC-CS03-2 | Paginacion 12/pagina + filtros + debounce 300ms | `NecesidadesPublicasFilter` type definido en contracts-plan. `filterNecesidadesSchema` con pageSize. `QUERY_KEYS.crowdsourcing.necesidades.publicas`. | Pantalla 1: sidebar filtros, debounce 300ms en search, 500ms en presupuesto/ciudad. Paginacion con `<Pagination>` shadcn. Spec UI detalla cada filtro y debounce. | Sin test-strategy.md generado | CUBIERTO (contratos, filtros y UX completamente documentados; falta test) |
| AC-CS03-3 | Badge urgencia < 3 dias | `esUrgente: boolean` en `NecesidadPublicaList`. `URGENCIA_DIAS_UMBRAL = 3` en constants. Backend calcula el campo. Frontend puede recalcular con la constante. | Badge `[!URGENTE!]` en NecesidadCard con estilos `bg-red-900/30 text-red-300 border-red-700` y animacion `urgency-pulse`. Visible en listado y detalle. | Sin test-strategy.md generado | CUBIERTO (campo backend + constante frontend + especificacion visual completa) |
| AC-CS03-4 | Expiradas no se muestran | Backend: `FechaLimitePropuestas IS NULL OR FechaLimitePropuestas > DateTime.UtcNow` en `GetNecesidadesPublicasQuery`. Contratos documentados. Si se accede por URL directo, `GET /api/crowdsourcing/necesidades/{id}` retorna 404. | Pantalla 1 no muestra expiradas. Pantalla 2 tiene estado "Necesidad expirada" con banner amber. | Sin test-strategy.md generado | CUBIERTO (filtro backend documentado + manejo 404 en UI) |
| AC-CS03-5 | Una propuesta por necesidad por usuario | `yaPropuso: boolean` en `NecesidadPublica`. ErrorCode `4003` con mensaje en `PROPUESTA_ERROR_MESSAGES`. Backend: `CreatePropuestaValidator` valida unicidad. Constraint BD `(NecesidadId, UserId)`. | CTA "Ya enviaste una propuesta" (bloque verde) con `CheckCircle`. Boton "Enviar propuesta" deshabilitado si `yaPropuso=true`. Toast error si intento via API directa. | Sin test-strategy.md generado | CUBIERTO (validacion backend + frontend documentados) |
| AC-CS03-6 | Artista propietario no puede enviarse propuesta | `esPropietario: boolean` en `NecesidadPublica`. ErrorCode `4004`. Backend: `CreatePropuestaValidator` valida ownership. Retorna 403. | CTA "Esta es tu necesidad" (bloque info). Boton deshabilitado si `esPropietario=true`. | Sin test-strategy.md generado | CUBIERTO (validacion backend + frontend documentados) |
| AC-CS03-7 | Propuesta con estado Pendiente, UserId y PerfilProfesionalId automaticos + toast | `CreatePropuestaCommand` asigna `EstadoPropuestaId=1`, `UserId` desde JWT, `PerfilProfesionalId` por lookup. `PropuestaCreatedResult` incluye `estadoPropuestaNombre: "Pendiente"`. Toast definido en ui-ux.md. | Toast "Propuesta enviada correctamente. El artista sera notificado." (verde). Dialog de envio se cierra tras exito. Pantalla 3 (modal) con estados Submitting -> Success documentados. | Sin test-strategy.md generado | CUBIERTO (flujo backend + UI documentados en contratos y ui-ux) |
| AC-CS03-8 | Sin PerfilProfesional -> CTA en lugar de boton | `tienePerfilProfesional: boolean` en `NecesidadPublica`. ErrorCode `4006`. Backend: si no tiene perfil retorna 403 al intentar crear propuesta. | CTA "Autenticado sin PerfilProfesional" documentado: bloque con texto + boton "Crear perfil profesional". Detalle completo sigue visible. ui-ux.md especifica estilos exactos del bloque CTA. | Sin test-strategy.md generado | CUBIERTO (campo backend + especificacion CTA en ui-ux) |
| AC-CS03-9 | Mis propuestas con badges semanticos (colores) | `ESTADO_PROPUESTA_BADGES: {1:'yellow', 2:'green', 3:'red', 4:'gray'}`. `MiPropuestaList` con `estadoPropuestaId` y `estadoPropuestaNombre`. Endpoint `GET /api/crowdsourcing/propuestas/mis-propuestas`. | PropuestaCard con badges: Pendiente=amber, Aceptada=green, Rechazada=red, Retirada=gray. Estilos CSS exactos definidos. Filter chips por estado con colores correspondientes. | Sin test-strategy.md generado | CUBIERTO (constantes + endpoint + estilos UI documentados) |
| AC-CS03-10 | Propuesta aceptada -> "Ver acuerdo"; Pendiente -> "Retirar" | `acuerdoId?: string` en `MiPropuestaList`. `APP_ROUTES` y navegacion al acuerdo pendiente de US-CS-04. Boton "Retirar" para Pendiente definido. | PropuestaCard: Pendiente -> boton "Retirar propuesta" (rojo outline). Aceptada -> boton "Ver acuerdo" (gradient). Interaccion "Click Ver acuerdo" navega a detalle acuerdo. | Sin test-strategy.md generado | PARCIAL (boton Retirar y Ver acuerdo documentados en UI; ruta `/acuerdos/{id}` no definida en APP_ROUTES de esta feature - depende US-CS-04) |
| AC-CS03-11 | Retirar solo Pendiente con confirmacion | `RetirarPropuestaCommand`: valida estado Pendiente y ownership. `PATCH /api/crowdsourcing/propuestas/{id}/retirar`. ErrorCode `4005`. `RetirarPropuestaResult`. | Pantalla 5: Dialog confirmacion con warning rojo, texto de consecuencias, boton "Confirmar retirada" (destructive). Loading state. Success: badge cambia a Retirada, boton Retirar desaparece. | Sin test-strategy.md generado | CUBIERTO (backend + dialog UI completamente documentados) |
| AC-CS03-12 | Empty state con sugerencia ampliar filtros | `NecesidadesPublicasFilter` sin resultados -> lista vacia (el endpoint no retorna error). | Empty state con filtros: icono `SearchX` + "No hay necesidades que coincidan con tus filtros. Intenta ampliar la busqueda." + boton [Limpiar filtros]. Empty state sin filtros: icono `Inbox` + mensaje diferente. Ambos documentados. | Sin test-strategy.md generado | CUBIERTO (dos variantes de empty state especificadas en ui-ux) |
| AC-CS03-13 | Detalle visible sin PerfilProfesional | `GET /api/crowdsourcing/necesidades/{id}` no requiere PerfilProfesional (accesible a cualquier rol autenticado). `tienePerfilProfesional` devuelto como campo calculado. | Pantalla 2: detalle completo siempre visible. CTA area cambia segun `tienePerfilProfesional`. "Autenticado sin PerfilProfesional": bloque CTA en lugar de boton, pero descripcion y detalles cargados. | Sin test-strategy.md generado | CUBIERTO (endpoint publico + logica CTA condicional documentados) |

### 3.2 Flujos Alternativos

| ID | Condicion | Cobertura en planes | Estado |
|----|-----------|---------------------|--------|
| FA-01 | Sin PerfilProfesional intenta enviar propuesta | Documentado en contracts.md (403/4006) y ui-ux.md (CTA). Ver AC-CS03-8. | CUBIERTO |
| FA-02 | Segunda propuesta a la misma necesidad | Documentado en contracts.md (400/4003) y ui-ux.md (boton deshabilitado + toast error). Ver AC-CS03-5. | CUBIERTO |
| FA-03 | Artista propietario intenta enviarse propuesta | Documentado en contracts.md (403/4004) y ui-ux.md. Ver AC-CS03-6. | CUBIERTO |
| FA-04 | Necesidad con fecha limite expirada | Backend: expiradas no aparecen en listado. Acceso directo por URL: 404. ui-ux.md: banner amber "ya no admite propuestas". | CUBIERTO |
| FA-05 | Precio fuera de rango del artista | ui-ux.md: nota informativa en tiempo real (verde/amber/azul segun rango). Pantalla 3 documenta logica y estilos. Permite continuar (no bloqueante). | CUBIERTO |
| FA-06 | No hay resultados en busqueda | ui-ux.md: dos variantes de empty state. Ver AC-CS03-12. | CUBIERTO |
| FA-07 | Retirar propuesta que no esta en Pendiente | Boton "Retirar" solo visible para Pendiente (ui-ux.md PropuestaCard). Backend: 400/4005 si llamada directa. | CUBIERTO |
| FA-08 | Necesidad pasa a Cerrada mientras el profesional ve el detalle | Recarga -> no en listado. Acceso por URL -> 404 (documentado en contracts.md). Manejo 404 en ui-ux.md ("Necesidad no encontrada"). | PARCIAL (el escenario de recarga sin navegar no tiene spec de invalidacion automatica de query; TanStack Query `staleTime: 30s` hace que el detalle desactualice en 30s, pero no hay spec de banner en tiempo real) |
| FA-09 | Sin cuenta intenta explorar necesidades | Rutas protegidas documentadas en contracts.md: redireccion a `/auth/login`. | CUBIERTO |

### 3.3 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Performance: listado < 500ms con 500 necesidades | Backend: paginacion obligatoria, proyecciones, indices BD documentados en feature-spec y contracts. | CUBIERTO (backend documentado; falta test de performance en test-strategy) |
| NFR-02 | Performance: debounce 300ms frontend | ui-ux.md: debounce 300ms search, 500ms presupuesto/ciudad. `useDebounce` hook documentado en notas de implementacion. | CUBIERTO |
| NFR-03 | Seguridad: ownership retirar propuesta | contracts.md: PATCH valida `UserId token == propuesta.UserId`. 403 si no. | CUBIERTO |
| NFR-04 | Seguridad: backend valida PerfilProfesional (no confiar en frontend) | feature-spec y contracts.md: backend verifica PerfilProfesional en CreatePropuestaCommand. | CUBIERTO |
| NFR-05 | Seguridad: `yaPropuso` y `esPropietario` calculados en backend | Documentado en contracts.md notas de implementacion. Campos readonly en types. | CUBIERTO |
| NFR-06 | Seguridad: rate limiting 20 propuestas/hora/usuario | Mencionado en feature-spec NFR. **No documentado en contracts-plan ni en ningun plan de implementacion frontend.** | NO CUBIERTO |
| NFR-07 | UX: Responsive mobile-first (3 breakpoints) | ui-ux.md: breakpoints mobile (<640px), tablet (640-1024px), desktop (>1024px) completamente documentados con layouts especificos. | CUBIERTO |
| NFR-08 | Accesibilidad: WCAG AA, contraste 4.5:1, ARIA labels | ui-ux.md: seccion completa de accesibilidad con ARIA labels, roles, live regions, focus visible, keyboard navigation. Ejemplos de codigo incluidos. | CUBIERTO |
| NFR-09 | Mantenibilidad: maestras en BD (no hardcoded) | `ESTADO_PROPUESTA` con valores numericos (1-4) alineados a maestras BD. Constants en shared. | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-01 | NFR-06 (Seguridad) | **Rate limiting** (max 20 propuestas/hora/usuario) no tiene cobertura en ningun plan. No esta en contracts-plan, ni hay mencion de como manejarlo en el frontend (deshabilitar boton temporalmente, mostrar mensaje). | Alto - Puede permitir spam de propuestas o abuso de la plataforma. | En `frontend-plan.md`: documentar que si el backend retorna error 429 (Too Many Requests), el frontend debe mostrar un toast con mensaje "Has alcanzado el limite de propuestas. Espera antes de enviar otra." y bloquear el boton durante ese tiempo. En `contracts.md`: agregar el codigo de error y comportamiento esperado. |
| GAP-02 | AC-CS03-10 (parcial) | **Ruta de acuerdos** (`/acuerdos/{id}` o similar) no definida en `APP_ROUTES` dentro de esta feature. El boton "Ver acuerdo" en `PropuestaCard` no tiene a donde navegar hasta que US-CS-04 defina la ruta. | Alto - El flujo post-aceptacion queda incompleto en el MVP. | En `frontend-plan.md`: documentar que el enlace "Ver acuerdo" queda como placeholder hasta que US-CS-04 defina la ruta. Agregar nota en `APP_ROUTES` sobre dependencia. En `PropuestaCard`, si `acuerdoId` existe pero la ruta no esta definida, mostrar el boton como disabled con tooltip "Proximamente". |

### 4.2 Gaps Mayores

| ID | Criterio afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-03 | AC-CS03-2 (parcial) | El filtro `ciudad` NO esta en `NecesidadesPublicasFilter` type definido en `contracts-plan.md` (solo `pais`), pero la `feature-spec.md` y el `ui-ux.md` incluyen filtro por ciudad. Inconsistencia entre el type TypeScript y la spec. | Medio - El componente `NecesidadFilters` planificado en ui-ux.md incluye input de ciudad pero el type/schema no lo soporta. | En `contracts-plan.md`: agregar `ciudad?: string` a `NecesidadesPublicasFilter` y a `filterNecesidadesSchema`. Verificar que el backend tambien soporte este query param (contracts.md no lo lista, solo `pais`). Alinear spec -> contracts -> type -> schema. |
| GAP-04 | Todos los AC | **Tests frontend ausentes**: `test-strategy.md` no ha sido generado. Sin cobertura de tests para ningun criterio de aceptacion en el target Landing. | Medio - Sin tests definidos, la implementacion no tiene criterio verificable de "done". | Generar `test-strategy.md` cubriendo: unit tests de componentes (`NecesidadCard`, `PropuestaCard`, `EstadoPropuestaBadge`), tests de hooks (`useNecesidadesPublicas`, `useCreatePropuesta`, `useRetirarPropuesta`), tests de integracion (flujo completo enviar/retirar propuesta), E2E tests listados en feature-spec. |
| GAP-05 | FA-08 (parcial) | Escenario de **necesidad cerrada mientras el usuario esta en la pagina de detalle** no tiene especificacion de comportamiento proactivo (solo reactive al recargar). TanStack Query con `staleTime: 30s` podria tardar en detectar el cambio. | Medio - UX degradada si el profesional intenta enviar propuesta a una necesidad que ya cerro. | En `frontend-plan.md`: especificar que `useNecesidadPublicaById` debe tener `staleTime` reducido (ej: 10s) o `refetchInterval` para el campo de estado. Alternativamente, manejar el error 404 que devolveria el backend al intentar enviar la propuesta, con un mensaje explicativo. |

### 4.3 Gaps Menores

| ID | Criterio afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-06 | AC-CS03-2 | El `contracts-plan.md` no documenta el parametro `ciudad` en `NecesidadesPublicasFilter` (ver GAP-03), y ademas **el filtro multi-value de `tipoNecesidadId`** (permite pasar varios tipos simultaneos) tampoco esta reflejado en el type (el type tiene `tipoNecesidadId?: number` singular, pero la feature-spec indica multi-select). | Bajo - El UI soportara multi-select pero el type solo acepta un valor. | Cambiar `tipoNecesidadId?: number` a `tipoNecesidadId?: number[]` en `NecesidadesPublicasFilter` y ajustar el schema Zod. Verificar que el backend soporte multi-value query params (contracts.md lo menciona). |
| GAP-07 | AC-CS03-7 | El mensaje del toast de exito en `contracts.md` es "Propuesta enviada correctamente. El artista sera notificado." pero en `ui-ux.md` el mismo toast dice "Propuesta enviada correctamente. El artista sera notificado." (coinciden). Sin embargo, el response del backend tiene `"message": "Propuesta enviada correctamente. El artista será notificado."` con tilde en "sera". **No hay constante centralizada para este mensaje en el shared.** | Bajo - Duplicacion del string; riesgo de inconsistencia futura. | Agregar `TOAST_MESSAGES.PROPUESTA_ENVIADA` en `constants/index.ts` o en `PROPUESTA_ERROR_MESSAGES` con clave semantica. |
| GAP-08 | NFR-07 (Responsive) | El `contracts-plan.md` define types y constants pero no documenta que el componente `NecesidadCard` tiene `fechaRelativa` como prop (`"hace 2 dias"`). El campo **no existe en `NecesidadPublicaList`** del contracts-plan (el type tiene `fechaCreacion: string` pero no `fechaRelativa`). Sin embargo `ui-ux.md` usa `fechaRelativa` en las props del componente. | Bajo - El frontend necesitara calcular `fechaRelativa` localmente a partir de `fechaCreacion`, o el backend necesita agregarlo. | Agregar funcion `formatFechaRelativa(fechaCreacion: string): string` en `src/shared/utils/` o especificar en `frontend-plan.md` que se calcula en el componente con `date-fns`. Actualizar `NecesidadCardProps` para reflejar que viene de `fechaCreacion`, no de un campo dedicado. |
| GAP-09 | AC-CS03-9 | `MiPropuestaList` tiene `estadoPropuestaId: number` y `estadoPropuestaNombre: string`. El tipo `PropuestaCardProps` en ui-ux.md usa `estadoPropuestaNombre: "Pendiente" | "Aceptada" | "Rechazada" | "Retirada"` (union type), pero el type de shared usa `estadoPropuestaNombre: string` (generico). **Discrepancia de tipos.** | Bajo - El componente `PropuestaCard` necesitara castear o el type de shared deberia ser mas especifico. | Cambiar `estadoPropuestaNombre: string` en `MiPropuestaList` a `estadoPropuestaNombre: EstadoPropuesta` para aprovechar el union type ya definido. |
| GAP-10 | Todos | Los plans `frontend-plan.md` y `ui-design.md` **no han sido generados** para el target `frontend-landing`. Sin estos planes, la implementacion carece de estructura de archivos, orden de tareas y dependencias tecnicas documentadas. | Bajo (en esta fase de planificacion, alto para la implementacion) | Generar `frontend-plan.md` con: estructura de features/crowdsourcing, componentes, hooks, servicios, rutas, y orden de implementacion. Generar `ui-design.md` que consolide el `ui-ux.md` existente con las decisiones de implementacion tecnica (props exactas, imports, etc.). |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

> **NOTA CRITICA:** El archivo `test-strategy.md` para `frontend-landing` no ha sido generado. La siguiente tabla refleja los tests que la `feature-spec.md` requiere y su estado actual.

| Criterio | Test Requerido (segun feature-spec) | Tipo | Estado |
|----------|-------------------------------------|------|--------|
| AC-CS03-1 | Verificar que GET `/api/crowdsourcing/necesidades` solo retorna Abiertas | Integration | NO CUBIERTO (falta test-strategy) |
| AC-CS03-2 | Verificar paginacion 12/pagina + debounce con network throttling | E2E | NO CUBIERTO |
| AC-CS03-3 | Verificar badge URGENTE con necesidad a 2 dias | Unit/E2E | NO CUBIERTO |
| AC-CS03-4 | Verificar expiradas no aparecen en listado | Integration | NO CUBIERTO |
| AC-CS03-5 | Verificar `yaPropuso=true` desactiva boton + 400 en segundo POST | Integration/E2E | NO CUBIERTO |
| AC-CS03-6 | Verificar `esPropietario=true` desactiva boton + 403 en POST | Integration/E2E | NO CUBIERTO |
| AC-CS03-7 | Verificar propuesta en BD con estado=Pendiente + toast en UI | Integration/E2E | NO CUBIERTO |
| AC-CS03-8 | Verificar CTA visible con usuario sin PerfilProfesional + detalle visible | E2E | NO CUBIERTO |
| AC-CS03-9 | Verificar badges con clases CSS correctas segun estado | Unit | NO CUBIERTO |
| AC-CS03-10 | Verificar enlace "Ver acuerdo" y boton "Retirar" | E2E | NO CUBIERTO |
| AC-CS03-11 | Verificar dialogo confirmacion + estado=Retirada en BD | Integration/E2E | NO CUBIERTO |
| AC-CS03-12 | Verificar empty state visible con mensaje correcto al aplicar filtros sin resultado | E2E | NO CUBIERTO |
| AC-CS03-13 | Verificar GET detalle retorna 200 con `tienePerfilProfesional=false` | Integration | NO CUBIERTO |

### Tests Faltantes (minimo requerido para MVP)

| Criterio | Test Requerido | Tipo | Prioridad |
|----------|----------------|------|-----------|
| AC-CS03-1, AC-CS03-4 | `NecesidadCard.test.tsx`: renderiza solo necesidades Abiertas y no expiradas | Unit | Alta |
| AC-CS03-3 | `NecesidadCard.test.tsx`: muestra badge URGENTE si `esUrgente=true` | Unit | Alta |
| AC-CS03-9 | `EstadoPropuestaBadge.test.tsx`: colores correctos por estado | Unit | Alta |
| AC-CS03-5, AC-CS03-6 | `NecesidadDetallePage.test.tsx`: CTA correcto segun `yaPropuso`/`esPropietario` | Unit/Integration | Alta |
| AC-CS03-8, AC-CS03-13 | `NecesidadDetallePage.test.tsx`: CTA perfil profesional; detalle visible sin perfil | Unit/Integration | Alta |
| AC-CS03-7 | `useCreatePropuesta.test.ts`: mutation exitosa + invalidacion queries | Unit (hook) | Alta |
| AC-CS03-11 | `RetirarPropuestaDialog.test.tsx`: dialogo confirmacion + mutation | Unit/Integration | Alta |
| AC-CS03-12 | `ExplorarNecesidadesPage.test.tsx`: empty state con y sin filtros activos | Integration | Media |
| AC-CS03-2 | `NecesidadFilters.test.tsx`: debounce 300ms + paginacion | Unit | Media |
| AC-CS03-10 | `PropuestaCard.test.tsx`: acciones correctas por estado | Unit | Media |
| General | E2E: flujo completo explorar -> ver detalle -> enviar propuesta | E2E (Playwright) | Alta |
| General | E2E: ver mis propuestas -> retirar propuesta | E2E (Playwright) | Alta |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida (feature-spec) | Documentada en ui-ux.md | Componentes Clave Definidos | Estado |
|---------------------------------|-------------------------|-----------------------------|--------|
| `/crowdsourcing/necesidades` (Explorar - Pantalla 1) | Si | `NecesidadCard`, `NecesidadFilters`, `NecesidadCardSkeleton`, empty states, paginacion | CUBIERTO |
| `/crowdsourcing/necesidades/:id` (Detalle - Pantalla 2) | Si | Header, Description, Details cards, sidebar accion, CTA dinamico, skeleton | CUBIERTO |
| Modal/drawer Enviar Propuesta (Pantalla 3) | Si | `EnviarPropuestaForm` (Dialog shadcn), campos, validacion, nota informativa precio, loading | CUBIERTO |
| `/crowdsourcing/mis-propuestas` (Mis Propuestas - Pantalla 4) | Si | `PropuestaCard`, `PropuestaCardSkeleton`, filter chips, empty states, paginacion | CUBIERTO |
| Dialog Confirmar Retirar (Pantalla 5) | Si | `RetirarPropuestaDialog`, warning alert, loading state, botones | CUBIERTO |

### Estados de UI

| Estado | Requerido | Documentado en ui-ux.md | Status |
|--------|-----------|------------------------|--------|
| Loading (skeletons) - todas las pantallas | Si | Si (6 skeletons listado, 3 detalle, 4 mis propuestas) | CUBIERTO |
| Error de red / reintentar | Si | Si (icono WifiOff + boton Reintentar) | CUBIERTO |
| Empty state sin filtros | Si | Si (icono Inbox + mensaje diferenciado) | CUBIERTO |
| Empty state con filtros activos | Si (AC-CS03-12) | Si (icono SearchX + boton Limpiar filtros) | CUBIERTO |
| CTA segun usuario (5 variantes) | Si | Si (todas documentadas en Pantalla 2) | CUBIERTO |
| Badge urgencia (pulsante) | Si (AC-CS03-3) | Si (animacion CSS urgency-pulse) | CUBIERTO |
| Nota informativa precio fuera de rango | Si (FA-05) | Si (3 variantes: dentro/por encima/por debajo) | CUBIERTO |
| Submitting (spinner + disabled) | Si | Si (Pantalla 3 y Pantalla 5) | CUBIERTO |
| 404 necesidad no encontrada | Si (FA-04, FA-08) | Si (icono AlertCircle + link volver) | CUBIERTO |
| Banner necesidad expirada | Si (FA-04) | Si (banner amber) | CUBIERTO |
| Sin autenticar -> redirect login | Si (FA-09) | Si (en contratos y ui-ux) | CUBIERTO |

### Componentes Reutilizables

| Componente | Documentado | Props tipadas | Estilos | Estado |
|------------|-------------|---------------|---------|--------|
| `NecesidadCard` | Si (ui-ux.md) | Si (interface completa) | Si (Tailwind especifico) | CUBIERTO |
| `NecesidadCardSkeleton` | Si | N/A | Si (codigo JSX) | CUBIERTO |
| `PropuestaCard` | Si | Si (interface completa) | Si (por estado) | CUBIERTO |
| `PropuestaCardSkeleton` | Si | N/A | Si (codigo JSX) | CUBIERTO |
| `EstadoPropuestaBadge` | Mencionado | No (falta interface Props) | Si (en PropuestaCard) | PARCIAL |
| `RetirarPropuestaDialog` | Si (Pantalla 5) | No (falta interface Props) | Si | PARCIAL |
| `EnviarPropuestaForm` | Si (Pantalla 3) | No (falta interface Props) | Si | PARCIAL |
| `PerfilProfesionalCTA` | Mencionado en feature-spec | No (falta spec separada) | Si (en Pantalla 2) | PARCIAL |
| `EmptyStateNecesidades` | Si (ui-ux.md) | No (falta interface Props) | Si | PARCIAL |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico - antes de implementar)

1. **Generar `frontend-plan.md` para target `frontend-landing`**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
   - Debe incluir: estructura de directorios, lista de componentes con responsabilidades, hooks con firmas, servicios, rutas React Router, orden de implementacion, dependencias entre archivos.
   - Cubrir GAP-10.

2. **Generar `test-strategy.md` para target `frontend-landing`**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/test-strategy.md`
   - Debe incluir como minimo todos los tests listados en la seccion 5 de este informe.
   - Herramientas: Vitest + Testing Library + Playwright para E2E.
   - Cubrir GAP-04 (CRITICO: sin tests no se puede verificar ninguno de los 13 AC).

3. **Documentar comportamiento frontend ante rate limiting (429)**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
   - Tambien actualizar `plans/cs-explorar-propuestas/shared/contracts-plan.md` con el error 429.
   - Cubrir GAP-01 (NFR-06).

### Acciones Sugeridas (Mayor - resolver antes o durante implementacion)

4. **Corregir inconsistencia: agregar `ciudad` a `NecesidadesPublicasFilter`**
   - Archivo: `plans/cs-explorar-propuestas/shared/contracts-plan.md` (seccion 2.1 y 3.2)
   - Cambio: agregar `ciudad?: string` al type `NecesidadesPublicasFilter` y al `filterNecesidadesSchema`.
   - Verificar tambien en `docs/user-stories/cs-explorar-propuestas/contracts.md` (no aparece como query param).
   - Cubrir GAP-03.

5. **Corregir: `tipoNecesidadId` debe soportar multi-value**
   - Archivo: `plans/cs-explorar-propuestas/shared/contracts-plan.md`
   - Cambio: `tipoNecesidadId?: number` -> `tipoNecesidadId?: number[]` en el type y schema.
   - Cubrir GAP-06.

6. **Documentar ruta de acuerdo como placeholder (dependencia US-CS-04)**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
   - Cambio: `PropuestaCard` muestra "Ver acuerdo" como disabled con tooltip "Proximamente" hasta que US-CS-04 defina la ruta.
   - Cubrir GAP-02 (parcialmente; la ruta completa se resolvera en US-CS-04).

7. **Especificar calculo de `fechaRelativa` en el frontend**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
   - Cambio: documentar uso de `date-fns/formatDistanceToNow` o una funcion utilitaria en `src/shared/utils/dates.ts`.
   - Cubrir GAP-08.

8. **Especificar `staleTime` reducido para detalle de necesidad (FA-08)**
   - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
   - Cambio: `useNecesidadPublicaById` con `staleTime: 10_000` (10s) en lugar del default, para reducir ventana de stale data.
   - Cubrir GAP-05.

### Nice to Have (Menor)

9. **Cambiar `estadoPropuestaNombre` a union type en `MiPropuestaList`**
   - Archivo: `plans/cs-explorar-propuestas/shared/contracts-plan.md`
   - Cambio: `estadoPropuestaNombre: string` -> `estadoPropuestaNombre: EstadoPropuesta` en la interface `MiPropuestaList`.
   - Cubrir GAP-09.

10. **Agregar constante centralizada para mensajes de toast**
    - Archivo: `plans/cs-explorar-propuestas/shared/contracts-plan.md` (seccion de constantes)
    - Cambio: agregar `TOAST_MESSAGES` con claves `PROPUESTA_ENVIADA`, `PROPUESTA_RETIRADA`.
    - Cubrir GAP-07.

11. **Definir interface Props para `EstadoPropuestaBadge`, `RetirarPropuestaDialog`, `EnviarPropuestaForm`, `PerfilProfesionalCTA`**
    - Archivo: `plans/cs-explorar-propuestas/frontend-landing/frontend-plan.md`
    - Cambio: agregar seccion de "Componentes con props documentadas" para los 4 componentes marcados como PARCIAL.
    - Cubrir parcialmente GAP-10.

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [x] Todos los 13 criterios de aceptacion tienen cobertura (al menos en contratos/spec)
- [x] Flujos de usuario completos documentados (5 pantallas + 9 flujos alternativos)
- [x] Casos de error cubiertos (codigos 400, 403, 404, toasts definidos)
- [ ] Tests definidos para cada criterio critico (PENDIENTE - falta test-strategy.md)
- [ ] Rate limiting documentado en planes (GAP-01 pendiente)

### UI/UX
- [x] Todas las screens planificadas (5 pantallas/modales documentadas)
- [x] Estados de interaccion definidos (loading, empty, error, success)
- [x] Responsive design considerado (3 breakpoints con layouts especificos)
- [x] Accesibilidad WCAG AA documentada (contraste, ARIA, keyboard nav)
- [ ] Props de todos los componentes reutilizables formalizadas (EstadoPropuestaBadge, RetirarPropuestaDialog, EnviarPropuestaForm, PerfilProfesionalCTA - PARCIAL)

### Testing
- [ ] Tests para criterios criticos definidos (PENDIENTE)
- [ ] Tests de integracion para flujos principales definidos (PENDIENTE)
- [ ] Cobertura objetivo definida (PENDIENTE - feature-spec dice 80%+)
- [ ] Framework y herramientas especificados en test-strategy (PENDIENTE)

### Shared (contracts-plan)
- [x] Types TypeScript completos para todos los DTOs
- [x] Schemas Zod alineados con validaciones backend
- [x] Constantes de estado, ordenamiento y urgencia
- [x] Query keys y API routes definidos
- [x] Error messages y helper definidos
- [ ] `ciudad` en `NecesidadesPublicasFilter` (GAP-03)
- [ ] `tipoNecesidadId` multi-value (GAP-06)
- [ ] `estadoPropuestaNombre` con union type en `MiPropuestaList` (GAP-09)

---

## 9. Conclusion

**Score Final AC:** 11/13 = **84.6%**
- 11 AC completamente cubiertos por contratos, ui-ux y feature-spec
- 1 AC parcialmente cubierto (AC-CS03-10: ruta acuerdo pendiente US-CS-04)
- 1 AC en riesgo por inconsistencia de datos (AC-CS03-2: campo `ciudad` ausente en type)

**Score Global (AC + FA + NFR):** ~26/31 = **83.9%**

**Veredicto: REQUIERE CAMBIOS**

Los planes de implementacion frontend (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) para el target `frontend-landing` **aun no existen**. La documentacion base (feature-spec, contracts, ui-ux, contracts-plan) cubre correctamente los requisitos funcionales y de UI, pero sin los planes de implementacion y la estrategia de tests, no se puede proceder a la implementacion con garantias de calidad.

**Proximo Paso:**
1. Resolver GAP-01 (rate limiting) y GAP-03/GAP-06 (inconsistencias de types) en `contracts-plan.md`
2. Generar `frontend-plan.md` con estructura de componentes, hooks, servicios y orden de implementacion
3. Generar `test-strategy.md` con cobertura de los 13 AC + flujos E2E criticos
4. Una vez generados y validados los tres planes, re-ejecutar este QA para alcanzar score >= 90% (APROBADO)

---

## 10. Inventario de Gaps por Severidad

| Severidad | Total | IDs |
|-----------|-------|-----|
| Critico | 2 | GAP-01, GAP-02 |
| Mayor | 3 | GAP-03, GAP-04, GAP-05 |
| Menor | 5 | GAP-06, GAP-07, GAP-08, GAP-09, GAP-10 |
| **Total** | **10** | |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-17
