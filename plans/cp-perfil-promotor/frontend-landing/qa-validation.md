# Validacion QA: Perfil de Promotor (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/web (Landing - Vite + React 18)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (scope Landing) | 7 |
| Cubiertos | 0 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos | 7 |
| **Score de Cobertura** | **0%** |

**Estado:** RECHAZADO

**Razon critica:** Los tres planes de implementacion requeridos para la Landing NO EXISTEN. Solo existe el plan de contratos compartidos (`plans/cp-perfil-promotor/shared/contracts-plan.md`). Los archivos `frontend-plan.md`, `ui-design.md` y `test-strategy.md` estan ausentes en el directorio `plans/cp-perfil-promotor/frontend-landing/`.

**Alcance de esta validacion:** Esta QA valida exclusivamente los criterios de aceptacion cuya responsabilidad recae en el proyecto Landing (`src/web`), segun la columna "Proyecto" de la feature-spec.md. Los criterios AC-CP01-1 a AC-CP01-5 y AC-CP01-10 son de responsabilidad Backend exclusiva y se marcan como N/A.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Proyecto | Scope Landing |
|----|----------|------|----------|---------------|
| AC-CP01-1 | Fan autenticado puede crear perfil de promotor con nombre publico y tipo; almacenado en BD | Funcional | Backend | N/A |
| AC-CP01-2 | Sistema vincula UserId del token al perfil; vincula FanProfileId si existe | Funcional | Backend | N/A |
| AC-CP01-3 | Perfil creado con EsActivo = true | Funcional | Backend | N/A |
| AC-CP01-4 | Se crea PromotorWallet en EUR automaticamente al crear el perfil | Funcional | Backend | N/A |
| AC-CP01-5 | No se puede crear segundo perfil para el mismo UserId; retorna error descriptivo | Funcional | Backend | N/A |
| **AC-CP01-6** | **URLs de redes sociales validadas con Zod en frontend y FluentValidation en backend; URL invalida impide guardado** | Funcional | Backend + Landing + Shared | **APLICA** |
| **AC-CP01-7** | **Formulario de registro carga tipos de promotor desde Maestra_TipoPromotor en un selector** | Funcional | Backend + Landing | **APLICA** |
| **AC-CP01-8** | **Promotor puede editar perfil (nombre, email de contacto, URLs); FechaActualizacion se actualiza en BD tras guardar** | Funcional | Backend + Landing | **APLICA** |
| **AC-CP01-9** | **Campo Tipo de Promotor no es editable una vez creado el perfil** | Funcional | Landing + Admin | **APLICA** |
| AC-CP01-10 | Al desactivar el perfil, todos los programas activos quedan con baja automatica | Funcional | Backend | N/A |
| **AC-CP01-11** | **Formulario de registro y edicion usa schemas Zod definidos en shared reutilizados en landing y admin** | Tecnico | Shared + Landing + Admin | **APLICA** |
| **AC-CP01-12** | **Usuario sin sesion activa en /promotor/registro es redirigido al login** | Funcional | Landing | **APLICA** |

**Criterios con scope Landing: 6 criterios directos (AC-CP01-6, 7, 8, 9, 11, 12)**

**Nota sobre AC-CP01-7:** El plan de contratos indica que en el MVP las opciones del selector se construyen desde la constante `TIPO_PROMOTOR_LABELS` (valores fijos de seed), sin necesidad de endpoint de maestras. Esto sigue siendo responsabilidad de Landing implementar el componente selector correctamente.

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos con Scope Landing

| ID | Criterio | frontend-plan.md | ui-design.md | test-strategy.md | Estado |
|----|----------|------------------|--------------|------------------|--------|
| AC-CP01-6 | Validacion URLs con Zod en frontend | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |
| AC-CP01-7 | Formulario carga selector de tipos de promotor | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |
| AC-CP01-8 | Promotor puede editar perfil (nombre, email, URLs) | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |
| AC-CP01-9 | Tipo de Promotor no editable post-creacion | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |
| AC-CP01-11 | Schemas Zod de shared reutilizados en landing | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |
| AC-CP01-12 | Usuario sin sesion redirigido a login desde /promotor/registro | ARCHIVO AUSENTE | ARCHIVO AUSENTE | ARCHIVO AUSENTE | NO CUBIERTO |

### 3.2 Requisitos No Funcionales (implicitamente requeridos)

Los siguientes NFRs se derivan de los flujos definidos en feature-spec.md y son responsabilidad de la Landing pero no tienen planes que los cubran:

| ID | Criterio Implicito | Origen | Cobertura | Estado |
|----|-------------------|--------|-----------|--------|
| NFR-01 | Redireccion a /promotor/dashboard si ya tiene perfil activo (FA-01) | feature-spec.md Flujo Principal paso 3 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-02 | Aviso informativo si no hay redes sociales indicadas (FA-03) | feature-spec.md FA-03 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-03 | Toast de error y datos del formulario mantenidos en caso de error de red (FA-05) | feature-spec.md FA-05 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-04 | Toast "Perfil de promotor creado correctamente" tras creacion exitosa | feature-spec.md Flujo Principal paso 12 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-05 | Toast "Perfil actualizado correctamente" tras edicion exitosa | feature-spec.md Flujo Secundario Editar paso 8 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-06 | Dialogo de confirmacion en desactivacion con numero de programas afectados | feature-spec.md Flujo Desactivar paso 3 | ARCHIVO AUSENTE | NO CUBIERTO |
| NFR-07 | Redireccion a pagina principal tras desactivacion | feature-spec.md Flujo Desactivar paso 8 | ARCHIVO AUSENTE | NO CUBIERTO |

**Leyenda:**
- CUBIERTO: Requisito completamente implementado en plan
- PARCIAL: Requisito parcialmente cubierto
- NO CUBIERTO: Requisito no mencionado en planes (en este caso por ausencia de los archivos de plan)
- N/A: No aplica a Landing

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| - | frontend-plan.md | El archivo `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md` no existe | BLOQUEANTE | Crear el plan de componentes, hooks y servicios para Landing antes de iniciar implementacion |
| - | ui-design.md | El archivo `plans/cp-perfil-promotor/frontend-landing/ui-design.md` no existe | BLOQUEANTE | Crear el diseno de UI con screens, estados y especificaciones visuales antes de implementar |
| - | test-strategy.md | El archivo `plans/cp-perfil-promotor/frontend-landing/test-strategy.md` no existe | BLOQUEANTE | Crear la estrategia de tests antes de iniciar implementacion para guiar la cobertura |
| AC-CP01-6 | Validacion URLs con Zod | Sin plan de implementacion para el formulario de registro/edicion con validacion Zod en Landing | Alto | Crear frontend-plan.md con detalle de integracion de `createPromotorSchema` y `updatePromotorSchema` desde shared |
| AC-CP01-7 | Selector tipos de promotor | Sin plan para componente selector que cargue `TIPO_PROMOTOR_LABELS` desde shared | Alto | Crear frontend-plan.md con el componente `<Select>` que usa las constantes de seed de MaestraTipoPromotor |
| AC-CP01-8 | Edicion del perfil | Sin plan para la pagina /promotor/perfil ni el formulario de edicion | Alto | Crear frontend-plan.md con `PromotorPerfilPage`, hook `useUpdatePromotor` y formulario con React Hook Form |
| AC-CP01-9 | Tipo de Promotor no editable | Sin plan para mostrar tipoPromotorNombre como campo de solo lectura en edicion | Alto | Crear ui-design.md con especificacion del estado read-only del campo tipo en el formulario de edicion |
| AC-CP01-11 | Schemas Zod desde shared | Sin plan que detalle como Landing importa y usa `createPromotorSchema` / `updatePromotorSchema` de shared | Alto | Crear frontend-plan.md con imports explicitos desde `src/shared/schemas/crowdpromotion.schema.ts` |
| AC-CP01-12 | Redireccion a login | Sin plan para el guard de rutas en /promotor/registro, /promotor/perfil y /promotor/dashboard | Alto | Crear frontend-plan.md con implementacion de ProtectedRoute o guard basado en estado JWT |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| NFR-01 | Redireccion si ya tiene perfil | Sin plan para el guard de ruta que detecta perfil existente via GET /me y redirige a /promotor/dashboard | Medio | Incluir en frontend-plan.md la logica de guard: query `usePromotor()` con `retry: false`, rama 200 -> redirect, rama 404 -> mostrar formulario |
| NFR-03 | Manejo de error de red | Sin plan para el manejo de FA-05 (mantener datos del form en error de red) | Medio | Incluir en frontend-plan.md la configuracion del hook `useCreatePromotor` con `onError` que muestra toast y NO limpia el formulario |
| NFR-04 | Toast creacion exitosa | Sin plan para el toast de exito tras POST exitoso | Medio | Incluir en frontend-plan.md el `onSuccess` del hook de mutacion con mensaje "Perfil de promotor creado correctamente" |
| NFR-05 | Toast edicion exitosa | Sin plan para el toast de exito tras PUT exitoso | Medio | Incluir en frontend-plan.md el `onSuccess` del hook `useUpdatePromotor` con mensaje "Perfil actualizado correctamente" |
| NFR-06 | Dialogo de confirmacion desactivacion | Sin plan para el dialogo que informa sobre programas afectados | Medio | Incluir en ui-design.md el componente dialog con estado condicional segun `totalProgramasActivos` del perfil |
| - | Estructura de rutas Landing | No se ha planificado como se integran las rutas /promotor/* en el router existente de src/web | Medio | Incluir en frontend-plan.md la seccion de routing con `APP_ROUTES.landing.promotor` |
| - | Servicio API promotor | Sin plan para `promotor.service.ts` en src/web que encapsule las llamadas a los 4 endpoints | Medio | Crear en frontend-plan.md el servicio con metodos createPromotor, getPromotor, updatePromotor, desactivarPromotor |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| - | Flujo FA-02 | Sin documentacion de como Landing maneja el caso de FanProfile no vinculado | Bajo | Documentar en frontend-plan.md que este flujo es transparente para el usuario (no es error bloqueante) |
| - | Flujo FA-03 | Sin plan para el aviso informativo cuando no hay redes sociales | Bajo | Incluir en ui-design.md un componente de aviso inline no bloqueante |
| NFR-07 | Redireccion post-desactivacion | Sin plan para redireccion a landing home tras desactivar perfil | Bajo | Incluir en frontend-plan.md el `onSuccess` de `useDesactivarPromotor` con redirect a `/` |
| - | Estados vacios | Sin especificacion de empty states para el perfil cuando campos opcionales estan vacios | Bajo | Incluir en ui-design.md los estados vacios para email, sitio web y redes sociales sin valor |
| - | Accesibilidad | Sin especificacion de requisitos WCAG AA para los formularios | Bajo | Incluir en ui-design.md seccion de accesibilidad: labels, ARIA, focus states en formularios |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-CP01-6: Validacion URL invalida bloquea envio | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-6: Validacion URL valida permite envio | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-7: Selector muestra 4 tipos de promotor | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-7: Tipo requerido en submit | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-8: Formulario edicion pre-rellena datos actuales | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-8: Submit actualiza y muestra toast exito | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-9: Campo tipo muestra como read-only en edicion | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-11: Schema Zod bloquea formato invalido | Sin test-strategy.md | - | NO CUBIERTO |
| AC-CP01-12: Ruta /promotor/registro sin JWT redirige a login | Sin test-strategy.md | - | NO CUBIERTO |

### Tests Minimos Requeridos (pendientes de crear en test-strategy.md)

| Criterio | Test Requerido | Tipo Sugerido | Razon |
|----------|----------------|---------------|-------|
| AC-CP01-6 | `test-url-validation-zod` | Unit | Verificar que schema Zod rechaza URLs invalidas y acepta validas |
| AC-CP01-7 | `test-tipo-promotor-selector` | Unit | Verificar que el select renderiza las 4 opciones de TIPO_PROMOTOR_LABELS |
| AC-CP01-8 | `test-edicion-perfil-form` | Integration | Verificar precarga, edicion y submit del formulario de edicion |
| AC-CP01-9 | `test-tipo-readonly-en-edicion` | Unit | Verificar que tipoPromotorNombre se muestra como texto no editable |
| AC-CP01-11 | `test-schema-shared-import` | Unit | Verificar que el formulario usa createPromotorSchema y updatePromotorSchema de shared |
| AC-CP01-12 | `test-route-guard-no-auth` | Integration | Verificar redireccion a /auth/login cuando no hay token JWT |
| NFR-01 | `test-route-guard-ya-tiene-perfil` | Integration | Verificar redireccion a /promotor/dashboard cuando GET /me retorna 200 |
| NFR-03 | `test-error-red-mantiene-datos` | Integration | Verificar que en error de red el formulario no se limpia |
| NFR-04 | `test-toast-creacion-exitosa` | Integration | Verificar toast "Perfil de promotor creado correctamente" |
| NFR-05 | `test-toast-edicion-exitosa` | Integration | Verificar toast "Perfil actualizado correctamente" |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Ruta | Planificada | Componentes | Estado |
|-----------------|------|-------------|-------------|--------|
| Registro de promotor | /promotor/registro | No (ui-design.md ausente) | - | NO CUBIERTO |
| Perfil / Edicion promotor | /promotor/perfil | No (ui-design.md ausente) | - | NO CUBIERTO |
| Dashboard promotor | /promotor/dashboard | No (ui-design.md ausente) | - | NO CUBIERTO |
| Dialogo confirmacion desactivacion | (modal) | No (ui-design.md ausente) | - | NO CUBIERTO |

### Estados de UI Requeridos

| Estado | Requerido por | Planificado | Estado |
|--------|--------------|-------------|--------|
| Loading (carga inicial del perfil) | Flujo Secundario Editar paso 2 | No | NO CUBIERTO |
| Error validacion URL | AC-CP01-6 / FA-04 | No | NO CUBIERTO |
| Error de red en submit | FA-05 | No | NO CUBIERTO |
| Toast exito creacion | Flujo Principal paso 12 | No | NO CUBIERTO |
| Toast exito edicion | Flujo Secundario Editar paso 8 | No | NO CUBIERTO |
| Toast exito desactivacion | Flujo Desactivar paso 7 | No | NO CUBIERTO |
| Aviso sin redes sociales | FA-03 | No | NO CUBIERTO |
| Tipo de promotor read-only en edicion | AC-CP01-9 | No | NO CUBIERTO |
| Dialogo confirmacion con N programas afectados | Flujo Desactivar paso 3 | No | NO CUBIERTO |
| Dialogo confirmacion simple (sin programas) | Flujo Desactivar paso 4 | No | NO CUBIERTO |

---

## 7. Validacion de Contratos

### Endpoints API (contracts.md) - Cobertura en Landing

| Endpoint | Metodo | Uso en Landing | Cobertura Plan | Estado |
|----------|--------|----------------|----------------|--------|
| `/api/crowdpromotion/promotor` | POST | Crear perfil (formulario registro) | frontend-plan.md ausente | NO CUBIERTO |
| `/api/crowdpromotion/promotor/me` | GET | Verificar perfil existente (guard ruta) + pre-rellenar edicion | frontend-plan.md ausente | NO CUBIERTO |
| `/api/crowdpromotion/promotor/me` | PUT | Actualizar perfil (formulario edicion) | frontend-plan.md ausente | NO CUBIERTO |
| `/api/crowdpromotion/promotor/me/desactivar` | PATCH | Desactivar perfil (dialogo confirmacion) | frontend-plan.md ausente | NO CUBIERTO |

### Constantes Shared (contracts-plan.md) - Uso en Landing

| Constante | Definida en Shared | Uso en Landing | Plan de uso | Estado |
|-----------|-------------------|----------------|-------------|--------|
| `QUERY_KEYS.crowdpromotion.promotor.me` | Si (contracts-plan.md) | usePromotor hook | frontend-plan.md ausente | NO CUBIERTO |
| `API_ROUTES.crowdpromotion.promotor.base` | Si (contracts-plan.md) | useCreatePromotor | frontend-plan.md ausente | NO CUBIERTO |
| `API_ROUTES.crowdpromotion.promotor.me` | Si (contracts-plan.md) | useUpdatePromotor, useDesactivarPromotor | frontend-plan.md ausente | NO CUBIERTO |
| `APP_ROUTES.landing.promotor.registro` | Si (contracts-plan.md) | Router + guards | frontend-plan.md ausente | NO CUBIERTO |
| `APP_ROUTES.landing.promotor.dashboard` | Si (contracts-plan.md) | Redireccion post-creacion | frontend-plan.md ausente | NO CUBIERTO |
| `APP_ROUTES.landing.promotor.perfil` | Si (contracts-plan.md) | Router | frontend-plan.md ausente | NO CUBIERTO |
| `TIPO_PROMOTOR_LABELS` | Si (contracts-plan.md) | Selector del formulario | frontend-plan.md ausente | NO CUBIERTO |
| `createPromotorSchema` | Si (contracts-plan.md) | Formulario registro | frontend-plan.md ausente | NO CUBIERTO |
| `updatePromotorSchema` | Si (contracts-plan.md) | Formulario edicion | frontend-plan.md ausente | NO CUBIERTO |
| `mapCreateFormToRequest` | Si (contracts-plan.md) | Hook useCreatePromotor | frontend-plan.md ausente | NO CUBIERTO |
| `mapUpdateFormToRequest` | Si (contracts-plan.md) | Hook useUpdatePromotor | frontend-plan.md ausente | NO CUBIERTO |
| `mapPromotorToUpdateForm` | Si (contracts-plan.md) | Pre-rellenado formulario edicion | frontend-plan.md ausente | NO CUBIERTO |

**Nota positiva:** El plan de contratos compartidos (`contracts-plan.md`) esta completamente elaborado y define con precision todos los tipos, schemas, constantes y mappers que Landing necesitara. Este trabajo es una base solida para generar los planes de Landing sin ambiguedad.

---

## 8. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md`**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md`
   - Contenido minimo requerido:
     - Estructura de features: `src/web/src/features/crowdpromotion/`
     - Paginas: `PromotorRegistroPage` (/promotor/registro), `PromotorPerfilPage` (/promotor/perfil), `PromotorDashboardPage` (/promotor/dashboard)
     - Componentes: `PromotorRegistroForm`, `PromotorPerfilForm`, `TipoPromotorSelect`, `DesactivarPromotorDialog`
     - Hooks: `usePromotor`, `useCreatePromotor`, `useUpdatePromotor`, `useDesactivarPromotor`
     - Servicio: `promotor.service.ts` con los 4 metodos de API
     - Guards de ruta: autenticacion JWT y verificacion de perfil existente
     - Integracion con schemas y mappers de shared

2. **Crear `plans/cp-perfil-promotor/frontend-landing/ui-design.md`**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/ui-design.md`
   - Contenido minimo requerido:
     - Diseno del formulario de registro (campos, layout, validacion inline)
     - Diseno del formulario de edicion con campo tipo como read-only
     - Diseno del dialogo de confirmacion de desactivacion (dos variantes: con/sin programas activos)
     - Estados de UI: loading, error de validacion por campo, error de red, success
     - Responsive design para los 3 breakpoints del proyecto
     - Especificaciones de accesibilidad para formularios (labels, ARIA, focus)

3. **Crear `plans/cp-perfil-promotor/frontend-landing/test-strategy.md`**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/test-strategy.md`
   - Contenido minimo requerido:
     - Tests unitarios para schemas Zod (validacion URL, email, nombre)
     - Tests unitarios para el selector TipoPromotorSelect
     - Tests de integracion para PromotorRegistroPage (flujo completo, error red, redireccion)
     - Tests de integracion para PromotorPerfilPage (precarga datos, edicion, campo readonly)
     - Tests de guard de rutas (sin JWT redirige a login, con perfil existente redirige a dashboard)
     - Objetivo de cobertura: 80%+

### Acciones Sugeridas (Mayor)

1. **Detallar el guard de ruta para /promotor/registro**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md`
   - Cambio: Documentar la logica de doble guard: (1) verificar JWT, (2) verificar perfil existente via `usePromotor()` con `retry: false`. El comportamiento esperado esta documentado en `contracts-plan.md` seccion 10.3 y debe reflejarse en el plan de Landing.

2. **Especificar la conversion de string vacio a undefined antes de llamar al API**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md`
   - Cambio: Documentar explicitamente que los hooks `useCreatePromotor` y `useUpdatePromotor` deben usar `mapCreateFormToRequest` / `mapUpdateFormToRequest` de shared para la conversion `'' -> undefined` en campos de URL opcionales. Esto esta definido en `contracts-plan.md` seccion 5.3 pero debe aparecer en el plan de Landing.

3. **Definir coercion de tipoPromotorId en React Hook Form**
   - Archivo: `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md`
   - Cambio: Documentar el uso de `z.coerce.number()` en el schema (alternativa recomendada en `contracts-plan.md` seccion 10.2) o el uso de `valueAsNumber` en el registro del select de RHF.

### Nice to Have (Menor)

1. **Agregar flujo de onboarding**
   - Si el usuario llega a /promotor/registro desde otro lugar de la Landing, considerar mostrar un breve texto motivacional sobre las ventajas de ser promotor antes del formulario.

2. **Validacion de accesibilidad de formularios**
   - Incluir en ui-design.md una checklist WCAG AA especifica para formularios: asociacion label-input, mensajes de error con aria-describedby, focus trap en dialog de confirmacion.

3. **Feedback visual en campo URL**
   - Considerar un icono de verificacion en tiempo real mientras el usuario escribe la URL (debounce + validacion Zod) para mejorar la UX antes del submit.

---

## 9. Checklist de Validacion

### Planes de Implementacion
- [ ] `frontend-plan.md` existe y esta completo
- [ ] `ui-design.md` existe y esta completo
- [ ] `test-strategy.md` existe y esta completo

### Requisitos Funcionales
- [ ] AC-CP01-6: Validacion URLs con Zod en formulario registro y edicion
- [ ] AC-CP01-7: Selector de tipos de promotor con datos de seed
- [ ] AC-CP01-8: Formulario de edicion de perfil (nombre, email, URLs)
- [ ] AC-CP01-9: Tipo de promotor como campo read-only en formulario de edicion
- [ ] AC-CP01-11: Schemas Zod importados desde src/shared
- [ ] AC-CP01-12: Guard de ruta redirige a login si no hay sesion

### Flujos Alternativos
- [ ] FA-01: Redireccion a /promotor/dashboard si ya tiene perfil
- [ ] FA-03: Aviso informativo si no hay redes sociales
- [ ] FA-04: Error de validacion por campo en URL invalida
- [ ] FA-05: Mantenimiento de datos del formulario en error de red

### UI/UX
- [ ] Screen /promotor/registro planificada
- [ ] Screen /promotor/perfil planificada
- [ ] Dialogo de confirmacion de desactivacion planificado
- [ ] Estados de interaccion definidos (loading, error, success)
- [ ] Campo tipo promotor read-only en edicion documentado
- [ ] Responsive design considerado
- [ ] Accesibilidad de formularios validada

### Testing
- [ ] Tests unitarios para validacion Zod
- [ ] Tests para selector de tipos
- [ ] Tests de integracion para flujos de registro y edicion
- [ ] Tests de guard de rutas
- [ ] Cobertura objetivo definida (80%+)

### Contratos
- [ ] Todos los endpoints de contracts.md cubiertos en el plan
- [ ] Constantes de shared (QUERY_KEYS, API_ROUTES, APP_ROUTES) usadas en plan
- [ ] Schemas y mappers de shared referenciados en plan

---

## 10. Analisis por Proyecto

### 10.1 Backend (No validado aqui)

| Criterio | Responsabilidad Backend | Estado Validacion |
|----------|------------------------|-------------------|
| AC-CP01-1 | POST /api/crowdpromotion/promotor - crear registro | Fuera de scope Landing |
| AC-CP01-2 | Vincular UserId y FanProfileId | Fuera de scope Landing |
| AC-CP01-3 | EsActivo = true en creacion | Fuera de scope Landing |
| AC-CP01-4 | Crear PromotorWallet en EUR | Fuera de scope Landing |
| AC-CP01-5 | Unicidad UserId (error 4018) | Fuera de scope Landing |
| AC-CP01-6 | FluentValidation de URLs | Validar en QA Backend |
| AC-CP01-8 | FechaActualizacion en PUT | Validar en QA Backend |
| AC-CP01-10 | Baja automatica de programas en desactivacion | Fuera de scope Landing |

### 10.2 Shared (Parcialmente validado aqui)

El `contracts-plan.md` define completamente los artifacts de shared. Su implementacion es prerequisito para los planes de Landing:

| Artifact | Definido en contracts-plan.md | Necesario para Landing | Estado |
|----------|-------------------------------|------------------------|--------|
| `src/shared/types/crowdpromotion.ts` | Si (seccion 2) | Si (tipos en hooks y componentes) | Pendiente implementacion |
| `src/shared/schemas/crowdpromotion.schema.ts` | Si (seccion 3) | Si (AC-CP01-6 y AC-CP01-11) | Pendiente implementacion |
| `src/shared/constants/index.ts` (bloque crowdpromotion) | Si (seccion 4) | Si (QUERY_KEYS, API_ROUTES, APP_ROUTES, TIPO_PROMOTOR) | Pendiente implementacion |
| `src/shared/utils/mappers.ts` (funciones promotor) | Si (seccion 5.3) | Si (conversion form -> request) | Pendiente implementacion |
| `src/shared/utils/error-messages.ts` (bloque promotor) | Si (seccion 5.1/5.2) | Si (manejo de errores en hooks) | Pendiente implementacion |

**Nota:** La implementacion de shared es prerequisito bloqueante para el plan de Landing. Los planes de Landing deben generarse DESPUES de confirmar que contracts-plan.md esta aprobado.

### 10.3 Admin (No validado aqui)

| Criterio | Responsabilidad Admin | Estado Validacion |
|----------|-----------------------|-------------------|
| AC-CP01-9 | Tipo de Promotor read-only en admin | Validar en QA Admin |
| AC-CP01-11 | Schemas Zod de shared en admin | Validar en QA Admin |

---

## 11. Conclusion

### Score Final: 0%

**Veredicto:** RECHAZADO

**Justificacion:**

Los 6 criterios de aceptacion con scope Landing (AC-CP01-6, AC-CP01-7, AC-CP01-8, AC-CP01-9, AC-CP01-11, AC-CP01-12) no tienen cobertura en ningun plan porque los tres archivos de plan requeridos no han sido creados:

- `plans/cp-perfil-promotor/frontend-landing/frontend-plan.md` - **AUSENTE**
- `plans/cp-perfil-promotor/frontend-landing/ui-design.md` - **AUSENTE**
- `plans/cp-perfil-promotor/frontend-landing/test-strategy.md` - **AUSENTE**

**Punto positivo:** El plan de contratos compartidos (`plans/cp-perfil-promotor/shared/contracts-plan.md`) esta completo y bien elaborado. Define con precision todos los tipos TypeScript, schemas Zod, constantes, mappers y error messages que Landing necesitara. Esto minimiza la ambiguedad para generar los planes pendientes.

**Gap Summary:**
- Gaps Criticos: 9 (3 archivos ausentes + 6 criterios AC sin cobertura)
- Gaps Mayores: 7 (flujos alternativos y servicios sin planificar)
- Gaps Menores: 5 (detalles de UX y accesibilidad)

**Proximo Paso:**

RECHAZADO - Crear los tres planes de implementacion antes de proceder a desarrollo:

1. Generar `frontend-plan.md` tomando como base `contracts-plan.md` (arquitectura hexagonal, hooks, servicio, guard de rutas)
2. Generar `ui-design.md` (formularios, estados de UI, responsive, dialogo de desactivacion)
3. Generar `test-strategy.md` (tests unitarios de schemas, tests de integracion de paginas, tests de guards)
4. Re-ejecutar esta validacion QA con los tres planes disponibles

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-25

**Archivos Validados:**
- `C:\Repos\WePlay_Rises\docs\user-stories\cp-perfil-promotor\feature-spec.md`
- `C:\Repos\WePlay_Rises\docs\user-stories\cp-perfil-promotor\contracts.md`
- `C:\Repos\WePlay_Rises\plans\cp-perfil-promotor\shared\contracts-plan.md`

**Archivos Buscados (NO encontrados):**
- `C:\Repos\WePlay_Rises\plans\cp-perfil-promotor\frontend-landing\frontend-plan.md` - AUSENTE
- `C:\Repos\WePlay_Rises\plans\cp-perfil-promotor\frontend-landing\ui-design.md` - AUSENTE
- `C:\Repos\WePlay_Rises\plans\cp-perfil-promotor\frontend-landing\test-strategy.md` - AUSENTE

**Resumen de Cobertura por Criterio:**
- AC-CP01-1 a AC-CP01-5: N/A (Backend exclusivo)
- AC-CP01-6: 0% - NO CUBIERTO (frontend-plan.md ausente)
- AC-CP01-7: 0% - NO CUBIERTO (frontend-plan.md ausente)
- AC-CP01-8: 0% - NO CUBIERTO (frontend-plan.md ausente)
- AC-CP01-9: 0% - NO CUBIERTO (ui-design.md ausente)
- AC-CP01-10: N/A (Backend exclusivo)
- AC-CP01-11: 0% - NO CUBIERTO (frontend-plan.md ausente)
- AC-CP01-12: 0% - NO CUBIERTO (frontend-plan.md ausente)
