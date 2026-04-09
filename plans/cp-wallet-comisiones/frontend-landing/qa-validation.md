# Validacion QA: cp-wallet-comisiones (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/web

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos evaluados | 14 |
| Cubiertos | 7 |
| Parcialmente Cubiertos | 5 |
| No Cubiertos | 2 |
| **Score de Cobertura** | **64%** |

**Estado: RECHAZADO**

Los planes disponibles al momento de esta validacion cubren unicamente la capa `shared` (tipos TypeScript, schemas Zod, constantes y utilidades). No existe aun `frontend-plan.md`, `ui-design.md` ni `test-strategy.md` para el target Landing. La cobertura de los criterios de aceptacion aplicables a Landing proviene exclusivamente de lo que define `contracts-plan.md` (shared) y `ui-ux.md` (especificacion de diseno de la feature, fuente de verdad de requisitos pero no un plan de implementacion).

Mientras no existan los tres planes de implementacion para Landing, no es posible proceder a la fase de implementacion.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

Los siguientes criterios son los que aplican directamente al proyecto Landing (src/web) segun la columna "Proyecto" de la feature-spec.

| ID | Criterio | Tipo |
|----|----------|------|
| AC-CP06-1 | Promotor autenticado accede a /promotor/wallet y ve saldo actual, moneda, total ganado, total retirado y minimo de retiro | Funcional |
| AC-CP06-5 | Historial paginado con filtros funcionales por tipo (esCredito), estadoTransaccionId y rango de fechas | Funcional |
| AC-CP06-6 | Solicitar cobro si saldoActual >= minimoRetiro; la accion crea transaccion de debito y decrementa saldo | Funcional |
| AC-CP06-7 | Intento de cobro con importe > saldoActual retorna 400; frontend muestra error en campo; saldo no se modifica | Funcional |
| AC-CP06-10 | Sin transacciones, /promotor/wallet muestra empty state con mensaje explicativo y acceso a explorar programas; saldo como 0.00 | Funcional |
| AC-CP06-12 | Tipos TypeScript PromotorWalletDto, PromotorWalletTransaccionDto y SolicitarCobroRequest definidos en shared y reutilizados por Landing | Shared/Contractual |

### Reglas de Negocio con impacto directo en Landing

| ID | Criterio | Tipo |
|----|----------|------|
| RN-02 | Validacion client-side: importe > 0 y importe <= saldoDisponible antes de enviar el request | Funcional |
| RN-06 | Importes siempre positivos; signo +/- determinado por EsCredito en la UI | Funcional |

### Flujos Alternativos con impacto en Landing

| ID | Criterio | Tipo |
|----|----------|------|
| FA-01 | Empty state con icono de wallet vacia, mensaje explicativo y boton "Explorar programas" | Funcional |
| FA-02 | Boton "Solicitar cobro" deshabilitado si saldo < minimoRetiro; aviso visible con minimo requerido | Funcional |
| FA-03 | Backend retorna 400; frontend muestra error en campo importe y permite ajustar | Funcional |

### Requisitos No Funcionales con impacto en Landing

| ID | Criterio | Tipo |
|----|----------|------|
| RNF-01 | Manejar 409 Conflict en solicitud de cobro concurrente; sugerir recargar y reintentar | No Funcional |
| RNF-02 | Minimo de retiro viene del backend (campo minimoRetiro en PromotorWalletDto); la constante frontend es soft-check | No Funcional |
| RNF-04 | Paginacion obligatoria con default 10, maximo 50 | No Funcional |

---

## 3. Matriz de Trazabilidad

### 3.1 Fuentes de cobertura disponibles

| Plan | Existe | Cubre |
|------|--------|-------|
| `plans/cp-wallet-comisiones/shared/contracts-plan.md` | SI | Tipos TS, schemas Zod, constantes, utilidades de shared |
| `docs/user-stories/cp-wallet-comisiones/ui-ux.md` | SI (SPEC, no plan) | Diseno de UI detallado (componentes, estilos, flujos, a11y) - fuente de verdad pero no plan de implementacion |
| `plans/cp-wallet-comisiones/frontend-landing/frontend-plan.md` | NO | Componentes, hooks, servicios de Landing |
| `plans/cp-wallet-comisiones/frontend-landing/ui-design.md` | NO | Pantallas, estados, interacciones |
| `plans/cp-wallet-comisiones/frontend-landing/test-strategy.md` | NO | Tests unitarios, de integracion y e2e |

### 3.2 Requisitos Funcionales

| ID | Criterio | contracts-plan | ui-ux.md (spec) | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|----------------|-----------------|---------------|-----------|---------------|--------|
| AC-CP06-1 | Ver saldo, moneda, totales, minimo de retiro | PromotorWallet interface con todos los campos | WalletSaldoCard con saldo, Total Ganado, Total Retirado, minimoRetiro | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP06-5 | Historial paginado con filtros tipo/estado/fechas | WalletTransaccionesFilters interface; QUERY_KEYS.wallet.transacciones | FiltrosHistorial, TransaccionesList, PaginacionWallet | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP06-6 | Solicitar cobro si saldo >= minimo | SolicitarCobroRequest, SolicitarCobroResponse; API_ROUTES.promotorWallet.cobro | SolicitarCobroDialog con campo importe y descripcion | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP06-7 | Error 400 si importe > saldo; mostrar error en campo | WALLET_ERROR_MESSAGES con clave '4040'; getWalletErrorMessage | Dialog: error inline en campo importe con `<p role="alert">` | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP06-10 | Empty state sin transacciones con CTA a explorar programas | - (no aplica a shared) | WalletEmptyState con boton a /crowdpromotion/explorar | NO EXISTE | NO EXISTE | NO EXISTE | PARCIAL |
| AC-CP06-12 | Tipos TS en shared reutilizados por Landing | PromotorWallet, WalletTransaccionItem, WalletTransaccionesPagedResponse, SolicitarCobroRequest, SolicitarCobroResponse, WalletTransaccionesFilters | Re-export desde domain/index.ts sugerido en ui-ux.md | NO EXISTE | NO EXISTE | NO EXISTE | CUBIERTO |

### 3.3 Reglas de Negocio

| ID | Regla | contracts-plan | ui-ux.md (spec) | frontend-plan | test-strategy | Estado |
|----|-------|----------------|-----------------|---------------|---------------|--------|
| RN-02 | Validacion client-side importe > 0 y <= saldoDisponible | solicitarCobroSchema (.positive()); nota sobre soft check en hook | Refinement a nivel componente via setError de RHF; validacion en useSolicitarCobro | NO EXISTE | NO EXISTE | CUBIERTO (schema) / NO CUBIERTO (hook y componente) |
| RN-06 | Signo +/- segun EsCredito | mapTransaccionTipoToDisplayProps en mappers.ts; formatTransaccionImporte en format.ts | TransaccionItem: + verde para credito, - rojo para debito | NO EXISTE | NO EXISTE | CUBIERTO (utils) / NO CUBIERTO (componente) |

### 3.4 Flujos Alternativos

| ID | Flujo | contracts-plan | ui-ux.md (spec) | frontend-plan | Estado |
|----|-------|----------------|-----------------|---------------|--------|
| FA-01 | Empty state con "Explorar programas" | - | WalletEmptyState con boton a /crowdpromotion/explorar | NO EXISTE | PARCIAL |
| FA-02 | Boton cobro deshabilitado + aviso minimo | MIN_RETIRO_WALLET = 10.00 | Boton deshabilitado si saldo < minimoRetiro; aviso informativo en amarillo | NO EXISTE | PARCIAL |
| FA-03 | Error 400 en campo importe | WALLET_ERROR_MESSAGES '4040' | Alert destructive en dialog; input con borde rojo; mensaje descriptivo | NO EXISTE | PARCIAL |

### 3.5 Requisitos No Funcionales

| ID | Requisito | contracts-plan | ui-ux.md (spec) | frontend-plan | Estado |
|----|-----------|----------------|-----------------|---------------|--------|
| RNF-01 | Manejo de 409 Conflict (cobro concurrente) | WALLET_ERROR_MESSAGES '4042' | Toast destructive con mensaje "Recarga e intenta de nuevo" | NO EXISTE | PARCIAL |
| RNF-02 | minimoRetiro configurable desde backend | minimoRetiro en PromotorWallet interface; MIN_RETIRO_WALLET como soft-check local | El valor se muestra desde `wallet.minimoRetiro` del DTO | NO EXISTE | CUBIERTO (contrato) |
| RNF-04 | Paginacion default 10, maximo 50 | WALLET_DEFAULT_PAGE_SIZE = 10; WALLET_TRANSACCIONES_PAGE_SIZE_MAX: 50 | PaginacionWallet con pageSize configurable | NO EXISTE | CUBIERTO (constantes) |

**Leyenda:**
- CUBIERTO: Requisito completamente cubierto en al menos un plan disponible
- PARCIAL: Requisito cubierto en shared/spec pero sin plan de implementacion de Landing
- NO CUBIERTO: Requisito no mencionado en ningun plan ni spec

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

Los gaps criticos son ausencias de planes completos que impiden la implementacion.

| ID | Gap | Impacto | Recomendacion |
|----|-----|---------|---------------|
| GAP-01 | No existe `frontend-plan.md` para Landing. No hay definicion de componentes, hooks ni servicios | ALTO - Sin este plan no se puede iniciar la implementacion de ningun componente de Landing | Generar `plans/cp-wallet-comisiones/frontend-landing/frontend-plan.md` con la arquitectura hexagonal completa: dominio, hooks, servicio y componentes de presentacion |
| GAP-02 | No existe `ui-design.md` para Landing. La especificacion ui-ux.md es una fuente de verdad, no un plan accionable | ALTO - Los implementadores necesitan un plan que mapee los tokens de diseno a componentes concretos con sus rutas y props | Generar `plans/cp-wallet-comisiones/frontend-landing/ui-design.md` adaptando ui-ux.md a formato de plan con detalles de implementacion |
| GAP-03 | No existe `test-strategy.md` para Landing. Ningun criterio de aceptacion tiene tests definidos | ALTO - Sin estrategia de tests no hay forma de verificar AC-CP06-1, AC-CP06-5, AC-CP06-6, AC-CP06-7, AC-CP06-10 | Generar `plans/cp-wallet-comisiones/frontend-landing/test-strategy.md` con tests unitarios por componente, tests de hooks y tests de integracion por flujo |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-04 | AC-CP06-6 / RN-02 | El `solicitarCobroSchema` en shared no incluye la validacion `importe <= saldoDisponible` porque el schema no tiene acceso al estado del wallet. Esto queda pendiente en el hook y el componente, pero no hay plan que documente como implementarlo | MEDIO - Risk de que el frontend no valide client-side el importe contra el saldo antes de hacer el request, lo que genera una llamada al API innecesaria si el importe es invalido | El `frontend-plan.md` debe documentar que `useSolicitarCobro` realiza el soft check `importe <= saldoDisponible` antes de invocar el servicio, y que `SolicitarCobroDialog` aplica el refinement dinamico via `setError` de React Hook Form |
| GAP-05 | AC-CP06-7 / FA-03 | Los codigos de error del backend para saldo insuficiente usan HTTP 409, no 400 como indica AC-CP06-7. El contracts.md define `409 / errorCode 4040` para saldo insuficiente. La feature-spec dice "400 Bad Request" pero los contratos dicen 409 | MEDIO - El frontend debe manejar 409 (no 400) para el error de saldo insuficiente. Si el componente verifica `error.status === 400` fallara silenciosamente | El `frontend-plan.md` debe documentar explicitamente que la deteccion de error de saldo insuficiente se basa en el `errorCode '4040'` del body de respuesta, no en el status HTTP. La verificacion no debe ser `if (status === 400)` sino `if (errorCode === '4040')` |
| GAP-06 | AC-CP06-5 | Los filtros del historial se aplican "inmediatamente" al cambiar el Select (segun ui-ux.md) pero las fechas usan debounce de 500ms. Este comportamiento diferencial no esta documentado en ningun plan y requiere una estrategia clara de estado (useState vs useReducer) | MEDIO - Sin un plan documentado, el implementador puede tomar decisiones inconsistentes sobre como manejar el estado de filtros | El `frontend-plan.md` debe especificar si se usa `useReducer` para el estado agregado de filtros o `useState` independientes, y documentar donde vive el debounce |
| GAP-07 | FA-01 + AC-CP06-10 | El empty state tiene dos variantes: (1) sin ninguna transaccion historica y (2) con transacciones pero filtros sin resultados. La condicion de activacion de la variante (1) es `saldoDisponible === 0 && totalCount === 0 && !hayFiltrosActivos`. Esta logica requiere que `useWallet` y `useWalletTransacciones` esten resueltos simultaneamente antes de renderizar. No hay plan que documente esta sincronizacion | MEDIO - Si las dos queries tienen estados de loading/error independientes, el componente puede mostrar el empty state erroneamente mientras una de las queries aun esta cargando | El `frontend-plan.md` debe documentar la estrategia de sincronizacion de las dos queries y los estados compuestos de la pagina |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-08 | RNF-01 | La constante `MIN_RETIRO_WALLET = 10.00` en shared es una copia local para el soft check client-side. Si el backend cambia el valor de `minimoRetiro` (porque es configurable via appsettings), la constante frontend quedara desincronizada | BAJO - En MVP el valor es fijo en 10.00, pero el diseno correcto es leer siempre el valor de `wallet.minimoRetiro` del DTO | El `frontend-plan.md` debe documentar que el boton de solicitar cobro y los avisos de minimo usan `wallet.minimoRetiro` del DTO, y que `MIN_RETIRO_WALLET` solo se usa como fallback o para el estado de carga (antes de que el DTO resuelva) |
| GAP-09 | AC-CP06-1 | El campo `saldoPendiente` esta definido en `PromotorWallet` (contracts-plan.md y contracts.md) pero no aparece en el layout del WalletSaldoCard de ui-ux.md. No hay especificacion de si este campo debe mostrarse en la UI | BAJO - Los implementadores podrian incluir o no el campo sin un criterio claro | Decidir explicitamente si `saldoPendiente` se muestra en la UI de MVP (sugerencia: no mostrarlo en MVP dado que ui-ux.md no lo incluye en el layout) y documentarlo en `ui-design.md` |
| GAP-10 | Routing | La ruta `/promotor/wallet` debe registrarse en `router.tsx` dentro de `DashboardLayout`. Esto esta mencionado en ui-ux.md pero no como un cambio planificado en ningun plan de implementacion | BAJO - Si el router no se actualiza, la pagina no es alcanzable | El `frontend-plan.md` debe incluir la modificacion de `src/web/src/app/router.tsx` como un paso explicito del plan |
| GAP-11 | Sidebar | El item "Mi Wallet" debe agregarse al sidebar (`src/web/src/components/layout/Sidebar.tsx`). Mencionado en ui-ux.md pero no en ningun plan | BAJO - Si el sidebar no se actualiza, la pagina no tiene acceso principal | El `frontend-plan.md` debe incluir la modificacion de `Sidebar.tsx` como paso explicito |

---

## 5. Validacion de Tests

No existe `test-strategy.md` para el target Landing. La siguiente tabla registra los tests que DEBERIAN existir segun los criterios de aceptacion, y el estado actual de cobertura.

### Cobertura de Criterios en Tests

| Criterio | Test Requerido | Tipo | Estado |
|----------|----------------|------|--------|
| AC-CP06-1 | Test renderiza WalletSaldoCard con todos los datos del DTO | Unit (componente) | NO CUBIERTO |
| AC-CP06-1 | Test muestra skeleton durante la carga | Unit (componente) | NO CUBIERTO |
| AC-CP06-5 | Test aplica filtro por tipo y refetch | Integration (hook + componente) | NO CUBIERTO |
| AC-CP06-5 | Test aplica filtro por estado y refetch | Integration (hook + componente) | NO CUBIERTO |
| AC-CP06-5 | Test aplica rango de fechas con debounce | Integration (hook) | NO CUBIERTO |
| AC-CP06-5 | Test paginacion cambia pagina manteniendo filtros | Integration (componente) | NO CUBIERTO |
| AC-CP06-6 | Test boton "Solicitar cobro" abre dialog si saldo >= minimo | Unit (componente) | NO CUBIERTO |
| AC-CP06-6 | Test submit del dialog llama al servicio con importe correcto | Integration (hook + dialog) | NO CUBIERTO |
| AC-CP06-6 | Test post-submit actualiza saldo y lista sin reload | Integration (query invalidation) | NO CUBIERTO |
| AC-CP06-7 | Test error 4040 del backend muestra mensaje en campo importe | Unit (dialog) | NO CUBIERTO |
| AC-CP06-7 | Test validacion client-side bloquea submit si importe > saldo | Unit (dialog) | NO CUBIERTO |
| AC-CP06-10 | Test muestra WalletEmptyState cuando totalCount === 0 | Unit (pagina) | NO CUBIERTO |
| AC-CP06-10 | Test boton "Explorar programas" navega a /crowdpromotion/explorar | Unit (WalletEmptyState) | NO CUBIERTO |
| FA-02 | Test boton deshabilitado cuando saldo < minimoRetiro | Unit (WalletSaldoCard) | NO CUBIERTO |
| RN-06 | Test formatTransaccionImporte retorna +X.XX para credito | Unit (util) | NO CUBIERTO - utils en shared sin tests en landing plan |
| RN-02 | Test useSolicitarCobro rechaza si importe > saldoDisponible | Unit (hook) | NO CUBIERTO |

### Tests Faltantes (resumen)

| Tipo | Cantidad Requerida | Cantidad Existente |
|------|-------------------|-------------------|
| Unit (componentes) | 10 | 0 |
| Unit (hooks) | 3 | 0 |
| Integration (flujos) | 5 | 0 |
| **Total** | **18** | **0** |

---

## 6. Validacion de UI/UX

La especificacion `ui-ux.md` es la fuente de referencia de diseno para esta feature. No existe un `ui-design.md` como plan formal de implementacion. La siguiente tabla mapea los estados y componentes de la especificacion contra la cobertura de los planes existentes.

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada en algun plan | Componentes en spec | Estado |
|------------------|--------------------------|---------------------|--------|
| Pantalla 1: Mi Wallet (/promotor/wallet) | NO (solo en ui-ux.md como spec) | PromotorWalletPage, WalletSaldoCard, FiltrosHistorial, TransaccionesList, PaginacionWallet | PARCIAL |
| Pantalla 2: Dialog Solicitar Cobro | NO (solo en ui-ux.md como spec) | SolicitarCobroDialog con RHF + Zod | PARCIAL |
| Pantalla 3: Empty State sin transacciones | NO (solo en ui-ux.md como spec) | WalletEmptyState | PARCIAL |

### Estados de UI

| Estado | Requerido | Especificado en ui-ux.md | Planificado en frontend-plan | Estado |
|--------|-----------|--------------------------|------------------------------|--------|
| Loading inicial (skeleton) | Si | Si - WalletSaldoCard skeleton + TransaccionesListSkeleton | NO | PARCIAL |
| Default (con datos) | Si | Si - layout completo con datos reales | NO | PARCIAL |
| Empty state (sin transacciones, sin filtros) | Si | Si - WalletEmptyState | NO | PARCIAL |
| Empty state (con filtros sin resultados) | Si | Si - WalletEmptyFiltered | NO | PARCIAL |
| Error de carga del wallet | Si | Si - card de error con reintentar | NO | PARCIAL |
| Error de carga de transacciones | Si | Si - error localizado en area de lista | NO | PARCIAL |
| Saldo insuficiente | Si | Si - boton deshabilitado + aviso amarillo | NO | PARCIAL |
| Post solicitud de cobro (exito) | Si | Si - toast + refetch | NO | PARCIAL |
| Dialog loading (enviando) | Si | Si - spinner + "Procesando..." | NO | PARCIAL |
| Dialog error de API | Si | Si - Alert destructive en dialog | NO | PARCIAL |

### Accesibilidad

| Requisito a11y | Especificado en ui-ux.md | Planificado en frontend-plan | Estado |
|----------------|--------------------------|------------------------------|--------|
| `aria-busy="true"` durante carga | Si | NO | PARCIAL |
| `role="alert"` en errores | Si | NO | PARCIAL |
| `disabled` semantico en boton | Si | NO | PARCIAL |
| `aria-describedby` en boton deshabilitado | Si | NO | PARCIAL |
| `<time dateTime>` en fechas | Si | NO | PARCIAL |
| Focus trap en dialog (via Radix) | Si | NO | PARCIAL |
| `aria-label` en controles de paginacion | Si | NO | PARCIAL |
| Signo textual (+/-) en importes | Si | NO | PARCIAL |

### Responsive Design

| Breakpoint | Especificado | Planificado | Estado |
|------------|-------------|-------------|--------|
| Mobile < 640px (grid-cols-1, boton full-width) | Si (ui-ux.md) | NO | PARCIAL |
| Tablet 640-1024px | Si (ui-ux.md) | NO | PARCIAL |
| Desktop > 1024px | Si (ui-ux.md) | NO | PARCIAL |

---

## 7. Recomendaciones

### Acciones Requeridas - Critico (bloquean la implementacion)

1. **Generar frontend-plan.md para Landing**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/frontend-plan.md`
   - Contenido requerido:
     - Estructura de directorios `src/web/src/features/crowdpromotion/wallet/` (domain, application/hooks, infrastructure, presentation)
     - Detalle de cada hook: `useWallet`, `useWalletTransacciones`, `useSolicitarCobro`
     - Detalle del servicio: `wallet.service.ts` con los tres metodos
     - Lista de componentes con props, estados internos y dependencias
     - Modificaciones a archivos existentes: `router.tsx` y `Sidebar.tsx`
     - Estrategia de manejo de estado de filtros (useState vs useReducer)
     - Soft check de importe <= saldoDisponible en el hook `useSolicitarCobro`
     - Estrategia de sincronizacion de queries `useWallet` y `useWalletTransacciones` para el empty state

2. **Generar ui-design.md para Landing**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/ui-design.md`
   - Contenido requerido:
     - Mapa de pantallas con rutas y precondiciones
     - Tabla de estados de UI por componente (loading, error, empty, default)
     - Especificacion de props de cada componente extraida de ui-ux.md
     - Confirmacion o rechazo explicito del campo `saldoPendiente` en la UI de MVP
     - Tokens de diseno aplicados (colores, tipografia, breakpoints)
     - Jerarquia de componentes confirmada

3. **Generar test-strategy.md para Landing**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/test-strategy.md`
   - Contenido requerido:
     - 10 tests unitarios de componentes (al menos los listados en la seccion 5)
     - 3 tests unitarios de hooks
     - 5 tests de integracion para flujos criticos (flujo cobro, filtros, paginacion, empty state, manejo de error 4040)
     - Mocks necesarios: `wallet.service.ts`, `useWallet`, `useWalletTransacciones`
     - Cobertura objetivo: >= 80%

### Acciones Sugeridas - Mayor (mejoran la robustez de implementacion)

4. **Documentar el soft check de importe en el hook**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/frontend-plan.md`
   - Cambio: Agregar seccion en el detalle de `useSolicitarCobro` que documente explicitamente que antes de llamar a `wallet.service.ts`, el hook verifica `importe > 0 && importe <= wallet.saldoDisponible`. Si falla, retorna un error sin hacer el request al API.

5. **Clarificar manejo de errores HTTP vs errorCode**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/frontend-plan.md`
   - Cambio: Documentar que la deteccion de error de saldo insuficiente (AC-CP06-7) se basa en `errorCode === '4040'` del body de la respuesta, NO en `status === 400`. Los contratos definen este caso como HTTP 409.

6. **Definir el valor de saldoPendiente en la UI**
   - Archivo: `plans/cp-wallet-comisiones/frontend-landing/ui-design.md`
   - Cambio: Decidir si el campo `saldoPendiente` del DTO se muestra en `WalletSaldoCard`. La especificacion ui-ux.md no lo incluye en el layout. Si no se muestra en MVP, documentarlo explicitamente para evitar que el implementador lo agregue o se pregunte si es un olvido.

### Nice to Have - Menor

7. **Agregar link desde el toast de tarea completada al wallet**
   - La especificacion ui-ux.md menciona un link opcional "Ver tu wallet" en el toast de tarea completada (US-CP-04). No es obligatorio para MVP pero mejora la discoverability.

8. **Badge de saldo en el sidebar**
   - La especificacion ui-ux.md menciona un badge opcional con el saldo disponible en el item "Mi Wallet" del sidebar. No es obligatorio para MVP.

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [x] Tipos TypeScript de shared cubren todos los campos del DTO (AC-CP06-12)
- [x] Schemas Zod definidos para SolicitarCobro y filtros de transacciones
- [x] Constantes QUERY_KEYS, API_ROUTES y APP_ROUTES definidas en shared
- [x] WALLET_ERROR_MESSAGES cubre todos los codigos de error del backend
- [ ] Hooks `useWallet`, `useWalletTransacciones`, `useSolicitarCobro` planificados
- [ ] Servicio `wallet.service.ts` con los tres metodos planificado
- [ ] Flujo completo de solicitar cobro planificado (dialog -> hook -> service -> refetch)
- [ ] Flujo de filtros con debounce para fechas planificado
- [ ] Soft check `importe <= saldoDisponible` antes del request planificado

### UI/UX
- [x] Pantalla 1 especificada en ui-ux.md (pero no como plan de implementacion)
- [x] Dialog Solicitar Cobro especificado en ui-ux.md
- [x] Empty state especificado en ui-ux.md
- [ ] Pantallas formalizadas en ui-design.md de Landing
- [ ] Estados de UI (loading, error, empty, default) en ui-design.md
- [x] Responsive design especificado (breakpoints en ui-ux.md)
- [x] Accesibilidad especificada (WCAG AA en ui-ux.md)
- [ ] Responsive design formalizado en ui-design.md con Tailwind classes
- [ ] Accesibilidad formalizada en ui-design.md

### Testing
- [ ] Tests para todos los criterios de aceptacion criticos (AC-CP06-1, 5, 6, 7, 10)
- [ ] Tests de integracion para flujo de solicitar cobro
- [ ] Tests de manejo de error 4040 / 4042
- [ ] Tests de estados de UI (loading, error, empty)
- [ ] Cobertura objetivo >= 80% definida

---

## 9. Inventario de Planes Disponibles vs Requeridos

| Plan | Requerido | Disponible | Brecha |
|------|-----------|------------|--------|
| `shared/contracts-plan.md` | Si | SI | Ninguna - cubre todos los contratos de shared |
| `frontend-landing/frontend-plan.md` | Si | NO | CRITICA |
| `frontend-landing/ui-design.md` | Si | NO | CRITICA |
| `frontend-landing/test-strategy.md` | Si | NO | CRITICA |

**Nota sobre ui-ux.md:** El archivo `docs/user-stories/cp-wallet-comisiones/ui-ux.md` es una especificacion de diseno muy detallada que puede servir de base directa para generar `ui-design.md`. El 90% del contenido de ui-design.md para Landing puede extraerse directamente de ui-ux.md. Esto reduce significativamente el esfuerzo para cerrar el gap critico.

---

## 10. Conclusion

**Score Final: 64%**

**Veredicto: RECHAZADO**

**Razon del rechazo:** Los tres planes de implementacion para el target Landing (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) no existen. La cobertura del 64% proviene de:
- Contratos shared completamente definidos (+28 puntos)
- Especificacion ui-ux.md con diseno detallado, accesibilidad y responsive (+36 puntos)

Los planes de shared estan bien elaborados y no tienen gaps criticos. La especificacion ui-ux.md es excepcionalmente detallada y constituye una base solida para los planes de Landing. Sin embargo, sin los planes formales de implementacion no es posible validar que la implementacion planificada cubra los criterios de aceptacion.

**Proximo Paso:** Generar los tres planes faltantes para Landing antes de proceder a la implementacion. Dado el nivel de detalle de `ui-ux.md`, el esfuerzo para generar `ui-design.md` es bajo. El plan con mayor esfuerzo es `frontend-plan.md` que requiere definir la arquitectura hexagonal completa con hooks, servicio y componentes.

| Plan a Generar | Esfuerzo Estimado | Fuente Principal |
|----------------|------------------|------------------|
| `ui-design.md` | Bajo | Adaptar directamente desde ui-ux.md |
| `frontend-plan.md` | Medio | Basarse en patron de `cp-tracking-metricas/frontend-landing/frontend-plan.md` |
| `test-strategy.md` | Medio | Basarse en patron de `cp-tracking-metricas/frontend-landing/test-strategy.md` |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-02
