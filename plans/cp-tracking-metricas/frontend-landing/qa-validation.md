# Validacion QA: cp-tracking-metricas (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/web (Vite + React 18, puerto 3000)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos Landing | 15 |
| Cubiertos | 13 |
| Parcialmente Cubiertos | 2 |
| No Cubiertos | 0 |
| **Score de Cobertura** | **93%** |

**Estado:** APROBADO

Los tres planes generados (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) cubren de forma completa la gran mayoria de los criterios de aceptacion correspondientes a Landing para la feature cp-tracking-metricas. Se identifican 2 gaps mayores: la cobertura parcial del mecanismo de disparo del evento PageView al navegar a una campana, y la cobertura parcial del mecanismo de disparo del evento Signup al completar el registro. Ambos gaps son resolubles sin replantear la arquitectura y no bloquean el inicio de la implementacion. No se identifican gaps criticos.

---

## 2. Criterios de Aceptacion

### Fuente: docs/user-stories/cp-tracking-metricas/feature-spec.md

Los criterios con alcance Landing son los siguientes. Los criterios con alcance exclusivo Backend (AC-CP05-5, AC-CP05-6, AC-CP05-7, AC-CP05-8) y exclusivo Admin (AC-CP05-9, AC-CP05-10, AC-CP05-11) quedan fuera del scope de esta validacion.

| ID | Criterio | Tipo | Scope |
|----|----------|------|-------|
| AC-CP05-1 | Al visitar URL con `ref` valido, frontend llama al endpoint de tracking y se crea PromoEvento tipo Click con programaId, promotorId, urlOrigen y UTMs correctamente persistidos | Funcional | Backend + Landing |
| AC-CP05-2 | El codigoReferido detectado en URL se persiste en cookie o sessionStorage; visitas posteriores sin `ref` siguen atribuyendo eventos al mismo promotor | Funcional | Landing |
| AC-CP05-3 | Al navegar a pagina de detalle de campana con codigo referido activo en sesion, se registra PromoEvento tipo PageView con CampaniaCrowdfundingId correcto | Funcional | Backend + Landing |
| AC-CP05-4 | Al completar registro con codigo referido activo, se registra PromoEvento tipo Signup con UserIdAfectado correcto | Funcional | Backend + Landing |
| AC-CP05-12 | Promotor autenticado puede ver dashboard individual con KPIs: misClicks, misConversiones, miValorGenerado, miComisionAcumulada y miTasaConversion para el programa seleccionado | Funcional | Backend + Landing |
| AC-CP05-13 | Dashboard del promotor muestra historial de eventos recientes con tipo, valor monetario (si aplica), comision generada (si aplica) y fechaEvento | Funcional | Backend + Landing |
| AC-CP05-14 | Promotor puede ver y copiar su enlace referido completo con parametros UTM desde su dashboard | Funcional | Landing |
| AC-CP05-15 | Tipos TypeScript definidos en shared y reutilizados por Landing | Funcional | Shared + Landing |
| NFR-01 | Hook de tracking no debe bloquear ni degradar la experiencia de navegacion del fan | No Funcional | Landing |
| NFR-02 | Frontend maneja silenciosamente el HTTP 429 del endpoint de tracking | No Funcional | Landing |
| NFR-06 | Endpoint publico: frontend no debe enviar Authorization header si el usuario es anonimo | No Funcional | Landing |
| RN-01 | Tracking de eventos es publico; fan puede ser anonimo en el hook | No Funcional | Landing |
| RN-07 | Promotor solo puede ver sus propias metricas (ruta protegida) | No Funcional | Landing |
| FA-04 | Si endpoint devuelve 429, frontend no reintenta el request | Flujo Alternativo | Landing |
| FA-06 | Si URL no tiene parametros de tracking, frontend NO llama al endpoint | Flujo Alternativo | Landing |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio (Resumen) | frontend-plan.md | ui-design.md | test-strategy.md | Estado |
|----|--------------------|------------------|--------------|------------------|--------|
| AC-CP05-1 | URL con ref -> Click tracking | `useTrackingInterceptor` lee `ref`, UTMs, `urlOrigen`, llama `POST /tracking/evento` con `tipoEventoPromoId: 1`; montado en `App.tsx` | Seccion 7: hook sin UI, sin bloqueo de render | 14 tests: detecta ref, llama servicio con tipo Click, envia UTMs y urlOrigen, urlReferer | CUBIERTO |
| AC-CP05-2 | Persistencia de ref en sesion para atribucion posterior | `useTrackingInterceptor`: persiste en `sessionStorage` (5 claves `TRACKING_STORAGE_KEYS`) + cookie `wp_ref` 30 min; recupera de sesion si no hay param en URL | Seccion 7: tabla de persistencia con todas las claves y TTLs | Tests: persiste ref en sessionStorage, UTMs en sessionStorage, escribe cookie wp_ref, atribucion de sesion sin nuevo param ref | CUBIERTO |
| AC-CP05-3 | Navegar a campana con ref activo -> PageView | `frontend-plan.md` Seccion 1: hook "engancha el evento PageView al navegar a `/campanias/:id`" pero no especifica el mecanismo concreto (hook secundario, useEffect en pagina, router listener) | ui-design.md no detalla el trigger de PageView ni la integracion con la pagina de campana existente | test-strategy.md no incluye test explicito para el flujo de navegacion a `/campanias/:id` con PageView | PARCIAL |
| AC-CP05-4 | Registro exitoso con ref activo -> Signup | `frontend-plan.md` Seccion 1: hook "engancha el evento Signup al completar el registro" pero no detalla el punto de integracion con el flujo de auth/registro existente | ui-design.md no detalla integracion con flujo de registro | test-strategy.md no incluye test explicito para el Signup post-registro | PARCIAL |
| AC-CP05-12 | Dashboard promotor con KPIs individuales | `usePromotorMetricas` hook + `KpiCard` x3 (clicks, conversiones, comision) + `TasaConversionInline`; `ProgramaSelector` para cambiar programa | Especificacion detallada: 3 variantes de KpiCard, tasa inline, layout grid 3 cols, selector de programa con `w-64` | 8 tests `useMetricasPromotor`, 7 tests `KpiCard`, 14 tests `PromotorMetricasPage` (incluye renders de KPIs y tasa) | CUBIERTO |
| AC-CP05-13 | Historial de eventos recientes | `EventosRecientesList` + `EventoRow` con `EventoBadge`, valor monetario y comision para Backing, fecha formateada ISO8601 | Especificacion completa: badges por tipo (5 variantes), detalle monetario solo para Backing, formato "DD mmm YYYY, HH:MM" | 9 tests `EventosRecientesList`: backing con valor+comision, no-backing sin importe, fecha formateada, empty state | CUBIERTO |
| AC-CP05-14 | Copiar enlace referido | `EnlaceReferido` con Input readonly + Button 4 estados (idle, copying, copied, error); `navigator.clipboard.writeText`; feedback 2000ms en boton; toast solo en fallo | Especificacion completa: estados del boton, CopyState type, feedback en boton sin toast (salvo fallo Clipboard) | 9 tests `EnlaceReferido`: copia URL, cambia a Copiado!, vuelve a Copiar en 2s con fakeTimers, toast en fallo, aria-label dinamico | CUBIERTO |
| AC-CP05-15 | Tipos TypeScript en shared reutilizados en Landing | Importa `RegistrarEventoRequest`, `PromotorMetricasResponse`, `EventoReciente`, `TipoEventoPromo` de `@shared/types/crowdpromotion`; usa `TRACKING_PARAMS`, `TRACKING_STORAGE_KEYS`, `QUERY_KEYS` de shared | Props de componentes tipadas con interfaces de shared; `EventoBadge` usa `TipoEventoPromo` como tipo del prop | Tests usan imports de `@shared/types/crowdpromotion`; 16 tests de mappers en `tracking.mappers.test.ts` en shared | CUBIERTO |

### 3.2 Requisitos No Funcionales y Reglas de Negocio

| ID | Criterio | Cobertura en Planes | Estado |
|----|----------|---------------------|--------|
| NFR-01 | Hook no bloquea render | `frontend-plan.md` Seccion 1 y `ui-design.md` Seccion 7: "no bloquea render de la aplicacion"; usa `useEffect` con dependencias vacias (fire-and-forget) | CUBIERTO |
| NFR-02 | 429 manejado silenciosamente | `frontend-plan.md`: no reintentar en 429; test "maneja 429 silenciosamente sin reintentar" en `test-strategy.md` Seccion 4.1 | CUBIERTO |
| NFR-06 | Endpoint publico (sin JWT) | `contracts-plan.md` confirma endpoint publico; `frontend-plan.md` menciona "Endpoint publico, sin Authorization header"; test en service "llama al endpoint correcto con POST" sin header de auth | CUBIERTO |
| RN-01 | Fan puede ser anonimo | `useTrackingInterceptor` no requiere autenticacion; `UserIdAfectado` se omite si el usuario es anonimo (campo no en body segun contracts.md) | CUBIERTO |
| RN-07 | Promotor solo ve sus metricas | Ruta `/promotor/metricas` protegida por autenticacion en router; endpoint GET filtra por token JWT del promotor autenticado en el backend | CUBIERTO |
| FA-04 | 429 -> no reintentar | Test dedicado en `test-strategy.md` Seccion 4.1: "maneja 429 silenciosamente sin reintentar" + flujo de integracion Seccion 8 | CUBIERTO |
| FA-05 | UTMs sin ref -> no llamar API | Edge case documentado en `test-strategy.md` Seccion 7: "URL con UTMs pero sin `ref` -> No llama al servicio" | CUBIERTO |
| FA-06 | Sin params -> no llamar API | Test dedicado en `test-strategy.md` Seccion 4.1: "no llama al servicio si no hay param ref ni cookie" | CUBIERTO |

### 3.3 Estados de UI (desde ui-ux.md - Pantalla 1 Dashboard Promotor)

| Estado Requerido | Plan en ui-design.md | Plan en frontend-plan.md | Test Cubierto | Estado |
|-----------------|----------------------|--------------------------|---------------|--------|
| Loading inicial (skeletons) | Si - KpiCardsSkeleton, EnlaceReferidoSkeleton, EventosRecientesSkeleton con codigo de composicion | Si - Seccion 5.1 con clases `animate-pulse bg-[#1e1e38]` | Si - test "muestra skeletons durante la carga inicial" | CUBIERTO |
| Default con datos | Si - KPI cards con valores reales, EnlaceReferido, EventosRecientesList | Si - componente `PromotorMetricasPage` con datos | Si - tests de renderiza KPIs/eventos tras carga | CUBIERTO |
| Empty state (sin programas) | Si - `EmptyStateSinProgramas` con icono gradiente y boton a `/promotor/mis-programas` | Si - Seccion 5.2 con composicion completa | Si - test "muestra empty state cuando no tiene programas" | CUBIERTO |
| Empty state (sin eventos) | Si - `EmptyStateEventos` dentro de `EventosRecientesList` cuando `eventos.length === 0` | Si - Seccion 5.3 | Si - test "muestra empty state cuando array de eventos vacio" | CUBIERTO |
| Cambio de programa (skeleton intermedio) | Si - skeleton parcial al cambiar `Select` | Si - condicion `isFetching && !isLoading` genera skeletons | Si - test "cambiar programa en el selector provoca nuevo fetch" | CUBIERTO |
| Error de carga | Si - `ErrorState` con `role="alert"` y boton Reintentar | Si - Seccion 5.4 con composicion completa | Si - tests de error state con boton Reintentar | CUBIERTO |
| Copiando enlace (idle/copying/copied) | Si - `CopyState` con 4 estados, feedback en boton 2000ms | Si - detalle de transiciones y duraciones | Si - tests de `EnlaceReferido` con `vi.useFakeTimers()` | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

No se identifican gaps criticos. Todos los requisitos de alto impacto tienen cobertura en al menos uno de los tres planes generados.

### 4.2 Gaps Mayores

| ID | Criterio | Gap Identificado | Impacto | Recomendacion |
|----|----------|-----------------|---------|---------------|
| GAP-M01 | AC-CP05-3: PageView al navegar a campana con ref activo en sesion | `frontend-plan.md` menciona que `useTrackingInterceptor` "engancha el evento PageView al navegar a `/campanias/:id`" pero no especifica el mecanismo concreto de integracion con el router. No hay componente ni hook secundario que gestione la deteccion de la navegacion a `/campanias/:id`. `test-strategy.md` no incluye tests para este sub-flujo especifico. | Medio - Sin este mecanismo el funnel Click -> PageView no funciona aunque el hook de Click si este bien implementado | 1) Agregar en `frontend-plan.md` el mecanismo concreto: opcion recomendada es un hook `usePageViewTracking` montado en `CampaniaDetailPage.tsx` que use `useEffect` con `useLocation()` de react-router-dom, lea el `codigoReferido` de sessionStorage y llame a `trackingService.registrarEvento({ tipoEventoPromoId: 2, codigoReferido, campaniaCrowdfundingId: params.id })`. 2) Agregar en `test-strategy.md` al menos 2 tests para este hook. |
| GAP-M02 | AC-CP05-4: Signup al completar registro con ref activo en sesion | Similar al gap anterior: `frontend-plan.md` menciona que el hook "engancha el evento Signup al completar el registro" pero no detalla el punto de integracion con el flujo de auth existente en `src/web/src/features/auth/`. No hay especificacion de en que punto del flujo de registro se llama al endpoint de tracking. `test-strategy.md` no incluye tests para este sub-flujo. | Medio - Sin este mecanismo los nuevos registros generados por promotores no se trackean | 1) Agregar en `frontend-plan.md` el mecanismo de integracion: en el `onSuccess` callback de la mutacion de registro (hook `useRegister` o equivalente), leer `codigoReferido` de sessionStorage y llamar a `trackingService.registrarEvento({ tipoEventoPromoId: 3, codigoReferido, userIdAfectado: newUser.id })`. 2) Agregar en `test-strategy.md` al menos 2 tests para este flujo. |

### 4.3 Gaps Menores

| ID | Criterio | Gap Identificado | Impacto | Recomendacion |
|----|----------|-----------------|---------|---------------|
| GAP-m01 | AC-CP05-2 | El test de la cookie `wp_ref` verifica solo la escritura del string pero no puede verificar el atributo `SameSite=Lax` ni el TTL real debido a limitaciones de jsdom. Reconocido como deuda tecnica en `test-strategy.md` Seccion 13. | Bajo | Agregar assertion del string de cookie en el test para verificar que el string escrito contiene `SameSite=Lax` (verificacion textual sin necesidad de TTL real). Ej: `expect(document.cookie).toContain('SameSite=Lax')`. |
| GAP-m02 | AC-CP05-12 | El `PromotorMetricasKpis` incluye `misPageViews` y `misSignups` en el response del API, pero la Landing solo renderiza 3 KPI cards (Clicks, Conversiones, Comision). La decision de UI de no mostrar estos dos KPIs no esta justificada explicitamente en los planes. | Bajo | Agregar nota en `frontend-plan.md` en la descripcion del componente `KpiCardsGrid` indicando que `misPageViews` y `misSignups` se reciben en el response pero no se renderizan en la UI del promotor (decision consciente de diseno, consistente con el mockup ASCII de ui-ux.md que solo muestra 3 cards). |
| GAP-m03 | Accesibilidad | Los planes definen correctamente todos los atributos ARIA (aria-busy, role="alert", aria-label, focus-rings) pero `test-strategy.md` anota en "Deuda Tecnica" que axe-core no esta instalado. No hay tests de accesibilidad automatizados. | Bajo | Evaluar instalacion de `vitest-axe` post-MVP. Por ahora, los atributos ARIA estan bien especificados y deben verificarse manualmente durante el smoke test del dashboard. |
| GAP-m04 | Error handling | Los planes muestran un `ErrorState` generico para todos los errores de carga. No se diferencia el mensaje de error segun el tipo de HTTP error (401 redireccion vs 403 acceso denegado). | Bajo | Agregar en `frontend-plan.md` una nota indicando que el error 401 se maneja por el interceptor de auth global existente (redireccion a `/auth/login`), mientras que el error 403 muestra el `ErrorState` con mensaje "No tienes acceso a esta seccion" en lugar del mensaje generico. |

---

## 5. Validacion de Tests

### 5.1 Cobertura de Criterios en Tests

| Criterio | Test Planificado | Archivo | Tipo | Estado |
|----------|-----------------|---------|------|--------|
| AC-CP05-1: Click al detectar ref | "detecta param ref y llama al servicio con tipo Click" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-1: UTMs y urlOrigen en payload | "envia urlOrigen en el payload", "persiste utm_source", "persiste utm_medium", "persiste utm_campaign" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-1: urlReferer en payload | "envia urlReferer en el payload de tracking" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-2: Persistencia en sessionStorage | "persiste ref en sessionStorage al detectarlo en URL" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-2: Escritura de cookie | "escribe cookie wp_ref al detectar ref" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-2: Atribucion de sesion | "no llama al servicio si ref ya fue procesado en esta sesion y no hay nuevo param" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| AC-CP05-3: PageView al navegar a campana | Sin test explicito para navegacion -> PageView | - | - | NO CUBIERTO |
| AC-CP05-4: Signup post-registro | Sin test explicito para registro -> Signup | - | - | NO CUBIERTO |
| AC-CP05-12: KPI cards en dashboard | "renderiza KPI cards con datos del promotor tras carga" | PromotorMetricasPage.test.tsx | Integration | CUBIERTO |
| AC-CP05-12: Tasa de conversion | "muestra tasa de conversion correctamente" (1.11%) | PromotorMetricasPage.test.tsx | Integration | CUBIERTO |
| AC-CP05-12: Selector de programa | "muestra el selector de programa con opciones cargadas", "cambiar programa provoca nuevo fetch" | PromotorMetricasPage.test.tsx | Integration | CUBIERTO |
| AC-CP05-13: Historial eventos recientes | "renderiza la lista EventosRecientesList con eventos" | PromotorMetricasPage.test.tsx | Integration | CUBIERTO |
| AC-CP05-13: Detalle Backing (valor + comision) | "muestra valor monetario y comision para eventos Backing" | EventosRecientesList.test.tsx | Unit | CUBIERTO |
| AC-CP05-13: No detalle para non-Backing | "no muestra valor monetario para eventos no-Backing" | EventosRecientesList.test.tsx | Unit | CUBIERTO |
| AC-CP05-13: Fecha formateada | "muestra fecha formateada para cada evento" | EventosRecientesList.test.tsx | Unit | CUBIERTO |
| AC-CP05-14: Copia al portapapeles | "click en Copiar llama a navigator.clipboard.writeText con la URL" | EnlaceReferido.test.tsx | Integration | CUBIERTO |
| AC-CP05-14: Feedback Copiado! 2000ms | "boton cambia a Copiado! despues del click" + "boton vuelve a Copiar despues de 2000ms" con fakeTimers | EnlaceReferido.test.tsx | Integration | CUBIERTO |
| AC-CP05-14: aria-label dinamico | "boton tiene aria-label dinamico Enlace copiado cuando copiado" | EnlaceReferido.test.tsx | Unit | CUBIERTO |
| AC-CP05-14: Fallback clipboard | "muestra toast de error si Clipboard API falla" | EnlaceReferido.test.tsx | Integration | CUBIERTO |
| AC-CP05-15: Tipos shared en componentes | 16 tests de mappers `mapTipoEventoPromoToLabel`, `mapTipoEventoPromoToBadgeClass`, `formatTasaConversion` | tracking.mappers.test.ts | Unit | CUBIERTO |
| NFR-02 / FA-04: 429 silencioso | "maneja 429 silenciosamente sin reintentar" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| FA-05: UTMs sin ref -> no llamar API | Edge case "URL con UTMs pero sin ref -> No llama al servicio" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |
| FA-06: Sin params -> no llamar API | "no llama al servicio si no hay param ref ni cookie" | useTrackingInterceptor.test.ts | Unit | CUBIERTO |

### 5.2 Tests Faltantes

| Criterio | Test Requerido | Archivo Sugerido | Razon |
|----------|----------------|-----------------|-------|
| AC-CP05-3 | "detecta navegacion a /campanias/:id con ref en sesion y llama al API con tipoEventoPromoId=2 y campaniaCrowdfundingId correcto" | usePageViewTracking.test.ts (nuevo) | Validar que el PageView se registra al navegar a la pagina de campana con ref activo en sesion |
| AC-CP05-3 | "no llama al API tipo PageView si no hay ref en sesion al navegar a campana" | usePageViewTracking.test.ts (nuevo) | Validar que FA-06 aplica tambien para el evento PageView |
| AC-CP05-4 | "al completar registro exitoso con ref en sesion, llama al API con tipoEventoPromoId=3 y userIdAfectado del nuevo usuario" | useTrackingInterceptor.test.ts o useRegister.test.ts (existente) | Validar que el Signup se registra con el userId del nuevo usuario |
| AC-CP05-4 | "no llama al API tipo Signup si no hay ref en sesion al registrarse" | useTrackingInterceptor.test.ts o useRegister.test.ts | Validar que FA-06 aplica tambien para el evento Signup |

---

## 6. Validacion de UI/UX

### 6.1 Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Estado |
|------------------|-------------|-------------|--------|
| `/promotor/metricas` - Dashboard del Promotor | Si - frontend-plan.md + ui-design.md | PromotorMetricasPage + ProgramaSelector + KpiCard + TasaConversionInline + EnlaceReferido + EventosRecientesList + EventoBadge + skeletons + empty states + ErrorState | CUBIERTO |
| Tracking interceptor (invisible, App.tsx) | Si - ui-design.md Seccion 7 + frontend-plan.md Seccion 2 | `useTrackingInterceptor` hook montado en App.tsx (sin UI) | CUBIERTO |
| `/promotor/eventos` - Historial completo | No - diferido fuera de MVP segun frontend-plan.md Seccion 1 | - | N/A (fuera de MVP) |

### 6.2 Estados de UI

| Estado | Requerido por ui-ux.md | Planificado en planes | Estado |
|--------|------------------------|-----------------------|--------|
| Loading inicial (skeletons) | Si | Si - KpiCardsSkeleton, EnlaceReferidoSkeleton, EventosRecientesSkeleton con `animate-pulse` | CUBIERTO |
| Default con datos | Si | Si - KpiCards con valores reales, badges de eventos | CUBIERTO |
| Empty state (sin programas) | Si | Si - EmptyStateSinProgramas con boton a `/promotor/mis-programas` | CUBIERTO |
| Empty state (sin eventos en programa) | Si | Si - EmptyStateEventos dentro de EventosRecientesList | CUBIERTO |
| Cambio de programa (loading intermedio) | Si | Si - condicion `isFetching && !isLoading` con skeletons | CUBIERTO |
| Error de carga | Si | Si - ErrorState con role="alert" y boton Reintentar | CUBIERTO |
| Boton Copiar idle/copying/copied | Si | Si - CopyState type con 4 estados (idle, copying, copied, error) | CUBIERTO |
| Toast error Clipboard API | Si | Si - toast.error de sonner solo en fallo de Clipboard API | CUBIERTO |

### 6.3 Responsive Breakpoints

| Breakpoint | Requisito ui-ux.md | Planificado en ui-design.md | Estado |
|------------|--------------------|-----------------------------|--------|
| Mobile < 640px | KPI en 1 columna, selector ancho completo, titulo text-xl, input con truncate | Si - `grid-cols-1`, `w-full sm:w-64`, `text-xl sm:text-2xl`, `truncate sm:truncate-none` | CUBIERTO |
| Tablet >= 640px | KPI en 3 columnas, cabecera en fila | Si - `sm:grid-cols-3`, `sm:flex-row sm:items-center` | CUBIERTO |
| Desktop > 1024px | Layout completo, max-w-4xl centrado | Si - `max-w-4xl mx-auto` aplica en todos los breakpoints | CUBIERTO |

### 6.4 Accesibilidad (WCAG AA)

| Requisito (ui-ux.md) | Implementacion en Planes | Estado |
|----------------------|--------------------------|--------|
| Contraste texto minimo 4.5:1 | `ui-design.md` Seccion 10: blanco `#ffffff` sobre `#151525` ratio > 7:1 (AAA); `#94a3b8` sobre `#151525` ratio ~4.6:1 (AA) | CUBIERTO |
| Focus ring en elementos interactivos | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en todos los elementos | CUBIERTO |
| Labels vinculados a Select | `<label htmlFor="programa-select">` vinculado a `<SelectTrigger id="programa-select">` + `aria-label` | CUBIERTO |
| aria-label dinamico en boton copiar | "Copiar enlace de promocion" -> "Enlace copiado" segun estado | CUBIERTO |
| aria-busy en contenedores de carga | `aria-busy="true"` en grid de KPI y contenedor de metricas | CUBIERTO |
| role="alert" en errores | ErrorState con `role="alert"` para lectores de pantalla | CUBIERTO |
| Input readonly accesible | `aria-label="Tu enlace de promocion"`, `aria-readonly="true"` | CUBIERTO |
| Badges de contraste | Colores de evento cumplen minimo 3:1 sobre fondos oscuros semi-transparentes | CUBIERTO |
| Pruebas automatizadas de a11y | axe-core no instalado - deuda tecnica reconocida en test-strategy.md | PARCIAL |

---

## 7. Validacion de Contratos API

### 7.1 Endpoints Cubiertos en los Planes

| Endpoint | Metodo | Cubierto en Plan | Estado |
|----------|--------|------------------|--------|
| `/api/crowdpromotion/tracking/evento` | POST | `trackingService.registrarEvento`, `contracts-plan.md` `API_ROUTES.crowdpromotion.tracking.evento` | CUBIERTO |
| `/api/crowdpromotion/promotor/metricas` | GET | `trackingService.getMetricasPromotor`, `useMetricasPromotor`, `QUERY_KEYS.crowdpromotion.metricas.promotor` | CUBIERTO |
| `/api/crowdpromotion/promotor/mis-programas` | GET | `useMisProgramasPromotor` para poblar el selector de programa | CUBIERTO |

### 7.2 Codigos de Error Cubiertos en Manejo Frontend

| HTTP Code | ErrorCode | Manejo en Plans | Estado |
|-----------|-----------|-----------------|--------|
| 429 | 4032 | Hook ignora silenciosamente, no reintenta - test cubierto en `useTrackingInterceptor.test.ts` | CUBIERTO |
| 400 | 1033 / 1034 | Hook limpia sessionStorage - test "maneja error 400 y limpia sessionStorage" | CUBIERTO |
| 400 | 1001 / 1002 | Manejo silencioso en hook de tracking (publico, sin UI de error) | CUBIERTO |
| 401 | 3001 | Service test "lanza error 401 cuando no autenticado"; UI muestra ErrorState | CUBIERTO |
| 403 | 4026 | Service test "lanza error 403 cuando no tiene acceso"; UI muestra ErrorState | CUBIERTO |
| 500 | 5000 | Service test "lanza error generico en 500"; UI muestra ErrorState | CUBIERTO |

### 7.3 Alineamiento de Tipos con contracts.md

| Interface TS (shared) | DTO Backend | Plan contracts-plan.md | Estado |
|-----------------------|-------------|------------------------|--------|
| `RegistrarEventoRequest` | `RegistrarEventoDto` | Si - todos los campos, tipoEventoPromoId obligatorio, resto opcionales | CUBIERTO |
| `RegistrarEventoResponse` | `RegistrarEventoResponseDto` | Si - `eventoId: string`, `registrado: boolean` | CUBIERTO |
| `PromotorMetricasKpis` | `PromotorMetricasKpisDto` | Si - 8 campos incluyendo misPageViews, misSignups, monedaNombre | CUBIERTO |
| `EventoReciente` | `EventoRecienteDto` | Si - todos los campos incluyendo `comisionGenerada: number | null` | CUBIERTO |
| `PromotorMetricasResponse` | `PromotorMetricasResponseDto` | Si - eventosRecientes max 20 items, ordenados DESC por fechaEvento | CUBIERTO |
| `TipoEventoPromo` | int en C# | Si - numeric union `1 | 2 | 3 | 4 | 5`, no string union | CUBIERTO |
| `FiltroMetricasPromotor` | query params | Si - programaId?, fechaDesde?, fechaHasta? todos opcionales | CUBIERTO |

---

## 8. Recomendaciones

### Acciones Requeridas (Mayor - resolver antes o durante implementacion de useTrackingInterceptor)

1. **Especificar mecanismo de PageView tracking en frontend-plan.md**
   - Archivo: `plans/cp-tracking-metricas/frontend-landing/frontend-plan.md`
   - Cambio: Agregar en la Seccion 3 (Hooks) la descripcion del mecanismo para lanzar el evento PageView. Opcion recomendada: crear un hook `usePageViewTracking` con la firma `usePageViewTracking(campaniaCrowdfundingId: string | undefined): void`. El hook usa `useEffect` con `useLocation()` de react-router-dom, verifica que existe `codigoReferido` en `sessionStorage.getItem(TRACKING_STORAGE_KEYS.ref)` y llama a `trackingService.registrarEvento({ tipoEventoPromoId: TIPO_EVENTO_PROMO.PageView, codigoReferido, campaniaCrowdfundingId })`. Se monta en `CampaniaDetailPage.tsx` pasando el `id` de la ruta como `campaniaCrowdfundingId`.

2. **Especificar integracion de Signup tracking con flujo de registro en frontend-plan.md**
   - Archivo: `plans/cp-tracking-metricas/frontend-landing/frontend-plan.md`
   - Cambio: Agregar en la Seccion 3 (Hooks) o en la descripcion de `useTrackingInterceptor` el punto de integracion con el flujo de registro. Descripcion: en el `onSuccess` callback de la mutacion de registro (hook `useRegister` o equivalente en `src/web/src/features/auth/`), verificar si `sessionStorage.getItem(TRACKING_STORAGE_KEYS.ref)` tiene valor. Si existe, llamar a `trackingService.registrarEvento({ tipoEventoPromoId: TIPO_EVENTO_PROMO.Signup, codigoReferido: storedRef, userIdAfectado: newUserId })`. Este callback se agrega en la feature `auth`, no en el hook de tracking.

3. **Agregar tests de PageView y Signup en test-strategy.md**
   - Archivo: `plans/cp-tracking-metricas/frontend-landing/test-strategy.md`
   - Cambio: Agregar seccion 4.1b "Hook: usePageViewTracking" con al menos 4 tests (detecta navegacion a campana con ref en sesion, no llama API sin ref, envia campaniaCrowdfundingId correcto, maneja error silenciosamente). Agregar en seccion de `PromotorMetricasPage` o en nueva seccion los 2 tests de Signup tracking.

### Acciones Sugeridas (Menor - pueden hacerse en paralelo a implementacion)

4. **Documentar decision de no mostrar misPageViews y misSignups en KPI del promotor**
   - Archivo: `plans/cp-tracking-metricas/frontend-landing/frontend-plan.md`
   - Cambio: Agregar nota en la descripcion del componente `KpiCardsGrid` o en el comentario del hook `usePromotorMetricas` indicando que el response incluye `misPageViews` y `misSignups` pero la UI del promotor en Landing solo renderiza 3 KPI cards por decision de diseno (consistente con el mockup de ui-ux.md).

5. **Agregar diferenciacion de errores 401 vs 403 en ErrorState**
   - Archivo: `plans/cp-tracking-metricas/frontend-landing/frontend-plan.md`
   - Cambio: Agregar nota en la descripcion de `PromotorMetricasPage` indicando que error 401 es manejado por el interceptor de auth global (redireccion a `/auth/login`) y no llega al `ErrorState`. El error 403 si llega al `ErrorState` con mensaje de acceso denegado.

### Nice to Have (Post-MVP)

6. Instalar `vitest-axe` o `@axe-core/react` para tests automatizados de accesibilidad en los componentes del dashboard del promotor.

7. Agregar test de integracion E2E (Playwright) para el flujo completo: navegar a URL con `?ref=CODE` -> verificar evento Click en DB -> navegar a campana -> verificar PageView -> registrarse -> verificar Signup.

8. Agregar log de nivel `debug` en `useTrackingInterceptor` para facilitar debugging en desarrollo (con guard `import.meta.env.DEV` para que no aparezca en produccion).

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [x] AC-CP05-1: Click tracking cubierto en hook, service y 14 tests
- [x] AC-CP05-2: Persistencia sessionStorage y cookie cubierta en hook y tests
- [ ] AC-CP05-3: PageView tracking - mecanismo de navegacion no especificado en detalle (Gap Mayor)
- [ ] AC-CP05-4: Signup tracking - integracion con flujo de registro no especificada en detalle (Gap Mayor)
- [x] AC-CP05-12: Dashboard promotor con KPIs cubierto en componentes, hooks y tests de integracion
- [x] AC-CP05-13: Historial de eventos recientes cubierto con badges, valores monetarios y fechas
- [x] AC-CP05-14: Enlace referido con copia al portapapeles cubierto con 4 estados y 9 tests
- [x] AC-CP05-15: Tipos TypeScript en shared definidos y reutilizados en Landing

### UI/UX
- [x] Pantalla `/promotor/metricas` planificada con layout completo (9 componentes)
- [x] 7 estados de interaccion definidos en ui-design.md (loading, empty x2, error, cambio, default, copiado)
- [x] Responsive design cubierto (mobile 1 col, tablet/desktop 3 cols KPI)
- [x] Accesibilidad definida (aria-busy, role="alert", aria-label, focus-rings, labels vinculados)
- [x] Design tokens alineados entre ui-ux.md y ui-design.md
- [x] 5 variantes de badges de evento con colores del design system
- [x] Animaciones y transiciones especificadas (fade-in, slide-up, transicion boton copiar)

### Testing
- [x] Tests para useTrackingInterceptor (14 tests, critico)
- [x] Tests de service con todos los endpoints y error codes (13 tests)
- [x] Tests de integracion para flujo completo de la pagina (14 tests en PromotorMetricasPage)
- [x] Cobertura objetivo 80%+ definida por archivo
- [x] Tests de edge cases: 429, 400, sin ref, sin programas, 0 clicks, Clipboard API falla
- [x] Mocks de sessionStorage, window.location, navigator.clipboard definidos
- [ ] Tests de PageView tracking (2 tests faltantes)
- [ ] Tests de Signup tracking (2 tests faltantes)

---

## 10. Conclusion

**Score Final:** 93%

**Veredicto:** APROBADO

Los tres planes (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) para el target Landing cubren de manera excelente los requisitos funcionales y no funcionales de la feature cp-tracking-metricas. La arquitectura del hook `useTrackingInterceptor`, los 9 componentes de la pagina del promotor, el servicio de tracking y la estrategia de 54 tests (38 unit + 16 integration) demuestran una planificacion solida y bien alineada con los contratos del backend definidos en `contracts.md` y `contracts-plan.md`.

Los 2 gaps mayores identificados (mecanismo concreto del trigger PageView en navegacion a campana y del trigger Signup post-registro) no bloquean el inicio de la implementacion porque el patron a seguir es el mismo que el de `useTrackingInterceptor`: leer `codigoReferido` de sessionStorage y llamar al servicio con el `tipoEventoPromoId` correspondiente. Se recomienda documentar estos mecanismos al implementar el hook de tracking.

**Proximo Paso:** Proceder a implementacion. Al implementar `useTrackingInterceptor`, resolver los gaps mayores (AC-CP05-3 y AC-CP05-4), documentar el mecanismo elegido en `frontend-plan.md` y agregar los 4 tests faltantes en `test-strategy.md`.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-02
