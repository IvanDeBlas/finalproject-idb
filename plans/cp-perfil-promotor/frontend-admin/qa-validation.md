# Validacion QA: cp-perfil-promotor (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/admin

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos Admin | 12 |
| Cubiertos | 3 |
| Parcialmente Cubiertos | 1 |
| No Cubiertos | 8 |
| **Score de Cobertura** | **29%** |

**Estado:** RECHAZADO

> **Nota critica:** El directorio `plans/cp-perfil-promotor/frontend-admin/` no existia al momento de esta validacion. No hay ningun `frontend-plan.md`, `ui-design.md` ni `test-strategy.md` para el target Admin. La unica cobertura parcial proviene del plan de shared (`contracts-plan.md`), que define los contratos de tipos y schemas que Admin consumiria, pero no planifica la implementacion de pantallas, componentes ni tests del dashboard administrativo.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Proyecto asignado |
|----|----------|------|-------------------|
| AC-CP01-1 | Fan autenticado puede crear perfil de promotor (nombre publico + tipo promotor); registro en BD correcto | Funcional | Backend |
| AC-CP01-2 | Sistema vincula UserId del token al perfil; FanProfileId vinculado si existe | Funcional | Backend |
| AC-CP01-3 | Perfil se crea con EsActivo = true | Funcional | Backend |
| AC-CP01-4 | PromotorWallet en EUR creada automaticamente al crear perfil | Funcional | Backend |
| AC-CP01-5 | No se puede crear segundo perfil para el mismo UserId; error descriptivo | Funcional | Backend |
| AC-CP01-6 | URLs de redes sociales validadas con formato URL correcto en frontend (Zod) y backend | Funcional | Backend + Landing + Shared |
| AC-CP01-7 | Formulario de registro carga tipos de promotor desde Maestra_TipoPromotor en selector | Funcional | Backend + Landing |
| AC-CP01-8 | Promotor puede editar perfil (nombre publico, email, URLs); FechaActualizacion actualizada en BD | Funcional | Backend + Landing |
| AC-CP01-9 | Campo Tipo de Promotor no editable una vez creado | Funcional | **Landing + Admin** |
| AC-CP01-10 | Al desactivar perfil, todos los programas activos quedan con baja automatica | Funcional | Backend |
| AC-CP01-11 | Formulario de registro y edicion usa schemas Zod de shared reutilizados en landing y admin | Funcional | **Shared + Landing + Admin** |
| AC-CP01-12 | Usuario sin sesion redirigido al login desde /promotor/registro | Funcional | Landing |

**Criterios con responsabilidad directa de Admin:** AC-CP01-9, AC-CP01-11
**Criterios con impacto indirecto en Admin:** AC-CP01-8 (flujo secundario menciona "landing (o admin)"), AC-CP01-6 (validacion Zod compartida)

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | frontend-plan.md | ui-design.md | test-strategy.md | contracts-plan.md | Estado |
|----|----------|-----------------|--------------|-----------------|-------------------|--------|
| AC-CP01-1 | Crear perfil de promotor | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-2 | Vinculacion UserId + FanProfileId | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-3 | EsActivo = true al crear | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-4 | PromotorWallet creada automaticamente | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-5 | Un perfil por UserId | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-6 | Validacion URLs con Zod en Admin | NO EXISTE | NO EXISTE | NO EXISTE | updatePromotorSchema cubre validacion URL | PARCIAL |
| AC-CP01-7 | Selector de tipos de promotor | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin (solo Landing) | N/A - Landing |
| AC-CP01-8 | Edicion de perfil en Admin | NO EXISTE | NO EXISTE | NO EXISTE | UpdatePromotorRequest + mappers definidos | NO CUBIERTO |
| AC-CP01-9 | Tipo Promotor no editable en Admin | NO EXISTE | NO EXISTE | NO EXISTE | UpdatePromotorRequest no incluye tipoPromotorId | PARCIAL (contrato ok, UI no planificada) |
| AC-CP01-10 | Desactivacion con baja de programas | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin | N/A - Backend |
| AC-CP01-11 | Schemas Zod de shared usados en Admin | NO EXISTE | NO EXISTE | NO EXISTE | updatePromotorSchema y tipos definidos en shared | PARCIAL (shared listo, consumo Admin no planificado) |
| AC-CP01-12 | Redireccion al login sin sesion | NO EXISTE | NO EXISTE | NO EXISTE | No aplica Admin (ruta es /promotor/registro en Landing) | N/A - Landing |

**Leyenda:**
- CUBIERTO: Requisito completamente implementado en plan
- PARCIAL: Requisito parcialmente cubierto (contratos listos pero implementacion no planificada)
- NO CUBIERTO: Requisito no mencionado en ningun plan de Admin
- N/A: No aplica a este proyecto/target

### 3.2 Responsabilidades Admin segun feature-spec.md

| Responsabilidad | Cobertura en Planes Admin | Estado |
|----------------|--------------------------|--------|
| Vista de perfil de promotor en dashboard (si el usuario tambien es promotor) | Sin plan | NO CUBIERTO |
| Mostrar estado del perfil (activo/inactivo) | Sin plan | NO CUBIERTO |
| Acceso rapido a la edicion | Sin plan | NO CUBIERTO |
| Tipo de Promotor como campo solo lectura (AC-CP01-9) | Contrato UpdatePromotorRequest no incluye tipoPromotorId | PARCIAL |
| Reutilizar updatePromotorSchema de shared (AC-CP01-11) | Schema definido en shared/contracts-plan.md | PARCIAL |

### 3.3 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive design en Admin (Next.js 14) | Sin plan | NO CUBIERTO |
| NFR-02 | Autenticacion Bearer JWT requerida en todas las rutas Admin | Layout de dashboard existente tiene guard de auth | CUBIERTO (patron existente) |
| NFR-03 | TypeScript strict mode (sin any) | Sin plan especifico | CUBIERTO (regla global del proyecto) |
| NFR-04 | shadcn/ui como libreria de componentes | Sin plan especifico | CUBIERTO (patron existente en Admin) |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-C01 | AC-CP01-8 | No existe `frontend-plan.md` para Admin. No hay plan de ruta, componentes ni hooks para la edicion del perfil de promotor en el dashboard administrativo. | Alto | Crear `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md` con: ruta `/crowdpromotion/promotor/perfil`, componente `PromotorPerfilPage`, hook `usePromotor`, hook `useUpdatePromotor`, servicio `promotor.service.ts` |
| GAP-C02 | AC-CP01-9 | No existe `ui-design.md` para Admin. No hay definicion del comportamiento visual del campo Tipo de Promotor como solo lectura en la vista de edicion del dashboard. | Alto | Crear `plans/cp-perfil-promotor/frontend-admin/ui-design.md` especificando el componente para campo read-only (Badge o Input disabled), y diferenciando la UI del formulario de creacion (Landing) vs edicion (Admin/Landing) |
| GAP-C03 | AC-CP01-11 | No existe `test-strategy.md` para Admin. Sin plan de tests no hay garantia de que el schema `updatePromotorSchema` importado de shared se use correctamente en el formulario de edicion de Admin. | Alto | Crear `plans/cp-perfil-promotor/frontend-admin/test-strategy.md` con: tests del hook `useUpdatePromotor`, tests del componente `PromotorEditForm` (campo tipoPromotor como read-only), test de integracion del flujo de edicion completo |
| GAP-C04 | Responsabilidad Admin general | No existe ninguna planificacion del modulo Crowdpromotion en `src/admin`. La estructura de directorios, rutas Next.js y navegacion del sidebar no han sido planificadas. | Alto | Incluir en `frontend-plan.md`: nueva entrada en sidebar, nueva ruta `(dashboard)/crowdpromotion/promotor/perfil/page.tsx`, guard que detecta si el usuario tiene perfil de promotor |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-M01 | AC-CP01-8 | Los mappers `mapUpdateFormToRequest` y `mapPromotorToUpdateForm` estan definidos en `shared/contracts-plan.md` pero no hay plan que especifique como el componente Admin los consume para pre-rellenar el formulario con datos del perfil existente. | Medio | Documentar en `frontend-plan.md` la integracion: `usePromotor()` -> `mapPromotorToUpdateForm(data)` -> `useForm defaultValues` -> submit -> `mapUpdateFormToRequest(formData)` -> `useUpdatePromotor()` |
| GAP-M02 | AC-CP01-8 | La gestion del estado del promotor (activo/inactivo) debe mostrarse en el dashboard Admin, pero no hay plan para el componente de visualizacion ni para la accion de desactivacion desde Admin. El flujo secundario de desactivacion menciona "perfil de promotor" sin especificar si Admin lo soporta. | Medio | Aclarar en feature-spec o en `ui-design.md` si Admin debe exponer la desactivacion o solo la edicion. Planificar componente `PromotorStatusBadge` usando `PromotorEstado` del shared |
| GAP-M03 | AC-CP01-11 | El hook de datos `usePromotor` (GET /promotor/me) no esta planificado para Admin. Este hook es necesario tanto para mostrar el perfil como para pre-rellenar el formulario de edicion. | Medio | Agregar a `frontend-plan.md`: hook `usePromotor` en `src/admin/src/hooks/use-promotor.ts` usando `QUERY_KEYS.crowdpromotion.promotor.me` y `API_ROUTES.crowdpromotion.promotor.me` del shared |
| GAP-M04 | AC-CP01-6 | No hay plan que especifique como los errores de validacion del backend (codigos 1013 para URL invalida) se muestran en el formulario de edicion de Admin. El mapeo de errores esta definido en shared (`PROMOTOR_ERROR_MESSAGES`) pero la integracion con el formulario Admin no esta planificada. | Medio | Documentar en `frontend-plan.md` el patron de manejo de errores del servidor: captura del error en `onError` del mutation, uso de `getPromotorErrorMessage(errorCode)` del shared, display con `toast.error()` (patron existente en Admin) |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-m01 | Navegacion | No esta planificado si el promotor accede a su perfil desde el sidebar principal del dashboard o desde una seccion dedicada de Crowdpromotion. La entrada de sidebar para el modulo Crowdpromotion no esta definida para Admin. | Bajo | Definir en `ui-design.md` la ubicacion del acceso: nueva seccion en sidebar "Crowdpromotion" o entrada bajo la seccion de perfil existente |
| GAP-m02 | Estado vacio | No hay plan para el estado donde el usuario no es promotor: que ve en Admin si intenta acceder a la ruta de perfil de promotor sin tener perfil creado. La ruta de creacion es en Landing (`/promotor/registro`), no en Admin. | Bajo | Documentar en `ui-design.md` el estado vacio: banner o card informativa con enlace a la Landing (`/promotor/registro`) para crear el perfil |
| GAP-m03 | Loading states | No hay plan para los estados de carga del formulario de edicion en Admin (skeleton, spinner en boton submit). El patron existe en `perfil/page.tsx` (Artista) pero no esta replicado para Promotor. | Bajo | Referenciar en `frontend-plan.md` el patron existente en `src/admin/src/app/(dashboard)/perfil/page.tsx` como base para los loading states del promotor |
| GAP-m04 | FA-05 en Admin | El flujo alternativo FA-05 (error de red -> toast de error, mantener datos del formulario) no esta planificado para el formulario de edicion de Admin. | Bajo | Documentar en `test-strategy.md` el test: simular error de red en `useUpdatePromotor`, verificar toast de error y que los datos del formulario persisten |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|-----------------|------|--------|
| AC-CP01-8 (edicion en Admin) | No existe test-strategy.md | - | NO CUBIERTO |
| AC-CP01-9 (tipo no editable en Admin) | No existe test-strategy.md | - | NO CUBIERTO |
| AC-CP01-11 (reutilizacion schema Zod en Admin) | No existe test-strategy.md | - | NO CUBIERTO |
| Hook usePromotor (GET /me) | No existe test-strategy.md | - | NO CUBIERTO |
| Hook useUpdatePromotor (PUT /me) | No existe test-strategy.md | - | NO CUBIERTO |
| PromotorEditForm - campo tipoPromotor readonly | No existe test-strategy.md | - | NO CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Tipo | Razon |
|----------|---------------|------|-------|
| AC-CP01-8 | `use-promotor.test.ts` - test GET /promotor/me | Unit (hook) | Verificar carga de datos y pre-relleno del formulario |
| AC-CP01-8 | `use-promotor.test.ts` - test PUT /promotor/me | Unit (hook) | Verificar mutation, invalidacion cache y toast de exito |
| AC-CP01-9 | `PromotorEditForm.test.tsx` - campo tipoPromotor disabled | Unit (component) | Verificar que el campo no es interactivo y muestra el valor actual |
| AC-CP01-11 | `PromotorEditForm.test.tsx` - validacion Zod | Unit (component) | Verificar que updatePromotorSchema bloquea URLs invalidas |
| AC-CP01-8 | `PromotorPerfilPage.test.tsx` - flujo completo | Integration | Verificar carga, edicion y guardado del perfil en Admin |
| FA-05 | `PromotorPerfilPage.test.tsx` - error de red | Integration | Verificar toast de error y persistencia de datos |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada en ui-design.md | Componentes | Estado |
|-----------------|---------------------------|-------------|--------|
| Vista/formulario perfil de promotor en dashboard | No | - | NO CUBIERTO |
| Estado del perfil (activo/inactivo) visible | No | - | NO CUBIERTO |
| Acceso rapido a edicion desde dashboard | No | - | NO CUBIERTO |
| Estado vacio (usuario sin perfil de promotor) | No | - | NO CUBIERTO |

### Estados de UI

| Estado | Requerido | Planificado | Estado |
|--------|-----------|-------------|--------|
| Loading (cargando perfil) | Si | No | NO CUBIERTO |
| Error (perfil no encontrado - 404) | Si | No | NO CUBIERTO |
| Success (perfil guardado - toast) | Si | No | NO CUBIERTO |
| Empty (usuario sin perfil de promotor) | Si | No | NO CUBIERTO |
| Campo Tipo Promotor read-only | Si (AC-CP01-9) | No | NO CUBIERTO |
| Badge estado activo/inactivo | Si | No | NO CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Crear frontend-plan.md para Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md`
   - Cambio: Definir estructura de directorios en `src/admin/src/app/(dashboard)/crowdpromotion/promotor/`, componentes necesarios, hooks (`usePromotor`, `useUpdatePromotor`), servicio `promotor.service.ts`, y patron de integracion con los mappers del shared (`mapPromotorToUpdateForm`, `mapUpdateFormToRequest`)

2. **Crear ui-design.md para Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/ui-design.md`
   - Cambio: Especificar el layout de la pantalla de perfil de promotor en Admin, el campo Tipo de Promotor como solo lectura (Input disabled o Badge, no Select), el badge de estado activo/inactivo usando `PromotorEstado` del shared, y el estado vacio con enlace a Landing para crear perfil

3. **Crear test-strategy.md para Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/test-strategy.md`
   - Cambio: Definir tests unitarios para hooks (`use-promotor.test.ts`), tests de componente para `PromotorEditForm` (readonly del campo tipo), y tests de integracion del flujo completo de edicion incluyendo error handling

4. **Planificar la entrada en sidebar del Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md`
   - Cambio: Definir si se agrega una nueva seccion "Crowdpromotion" en el sidebar o si el acceso al perfil de promotor aparece bajo la seccion de perfil existente. El sidebar actual (`src/admin/src/components/layout/sidebar.tsx`) debe ser actualizado

### Acciones Sugeridas (Mayor)

1. **Definir patron de consumo de shared en Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md`
   - Cambio: Documentar explicitamente que el formulario de edicion de Admin usa `updatePromotorSchema` importado de `@shared/schemas`, `UpdatePromotorFormData` de `@shared/types`, y los mappers `mapPromotorToUpdateForm` y `mapUpdateFormToRequest` de `@shared/utils`. Esto cumple AC-CP01-11 de forma demostrable

2. **Aclarar alcance de desactivacion en Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/ui-design.md`
   - Cambio: Definir si la desactivacion del perfil de promotor es accesible desde Admin (con dialog de confirmacion similar al patron en Landing) o si Admin es solo de lectura/edicion. El feature-spec no lo especifica para Admin, pero el impacto sobre programas activos es significativo

3. **Definir guard para ruta de promotor en Admin**
   - Archivo: `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md`
   - Cambio: Dado que no todos los usuarios Admin son promotores, definir el comportamiento cuando el usuario accede a la ruta de promotor sin tener perfil: mostrar estado vacio con CTA o redirigir a Landing

### Nice to Have (Menor)

1. **Definir componente PromotorStatusBadge reutilizable**
   - El tipo `PromotorEstado` y la funcion `getPromotorEstado` ya estan en el shared. Planificar un componente en `src/admin/src/components/crowdpromotion/PromotorStatusBadge.tsx` reutilizable en futuras features del modulo (US-CP-02, US-CP-03)

2. **Referencia al patron de perfil de Artista existente**
   - El patron de la pagina de perfil de artista en `src/admin/src/app/(dashboard)/perfil/page.tsx` (carga con useQuery, loading skeleton, formulario con hook form) es directamente replicable para el promotor. Documentar esta referencia en el plan

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [ ] Todos los criterios de aceptacion de Admin tienen cobertura (AC-CP01-9 y AC-CP01-11 no cubiertos)
- [ ] Flujos de usuario completos planificados (flujo de edicion de perfil no planificado)
- [ ] Casos de error cubiertos (error handling no planificado)

### UI/UX
- [ ] Todas las screens planificadas (ninguna screen de promotor planificada para Admin)
- [ ] Estados de interaccion definidos (loading, error, success, empty no definidos)
- [ ] Responsive design considerado (no planificado)
- [ ] Accesibilidad validada (no planificada)

### Testing
- [ ] Tests para criterios criticos (no existe test-strategy.md)
- [ ] Tests de integracion para flujos (no planificados)
- [ ] Cobertura objetivo definida (no definida)

### Shared Integration
- [x] Types TypeScript definidos en shared (contracts-plan.md cubre esto)
- [x] Schemas Zod definidos en shared (contracts-plan.md cubre esto)
- [x] Mappers definidos en shared (contracts-plan.md cubre esto)
- [ ] Consumo de shared documentado en plan Admin (no documentado)

---

## 9. Inventario de Archivos a Crear (cuando se elabore el plan)

Basado en los patrones existentes en `src/admin` y los requisitos de la feature:

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── crowdpromotion/
│           └── promotor/
│               └── perfil/
│                   └── page.tsx              <- Vista principal perfil promotor
├── components/
│   └── crowdpromotion/
│       ├── PromotorEditForm.tsx              <- Formulario edicion (tipoPromotor read-only)
│       └── PromotorStatusBadge.tsx           <- Badge activo/inactivo
├── hooks/
│   └── use-promotor.ts                       <- usePromotor + useUpdatePromotor
└── services/
    └── promotor.service.ts                   <- Llamadas a API
```

---

## 10. Conclusion

**Score Final:** 29%

**Veredicto:** RECHAZADO

**Detalle del score:**
- Requisitos Admin directos (AC-CP01-9, AC-CP01-11): 0% cubiertos en planes Admin
- Responsabilidades Admin del feature-spec (vista perfil, estado, edicion): 0% planificadas
- Cobertura de tests Admin: 0%
- Infraestructura shared reutilizable (tipos, schemas, mappers): 100% cubierta por contracts-plan.md
- Patrones existentes de Admin aplicables (auth guard, RHF, TanStack Query, shadcn/ui): disponibles pero no conectados al plan

**La causa raiz del rechazo** no es un fallo de diseno sino la **ausencia total de planes de implementacion Admin**. El plan de shared (`contracts-plan.md`) esta bien ejecutado y proporciona toda la infraestructura necesaria (tipos, schemas Zod, mappers, constantes, error messages). Lo que falta es el plan especifico de Admin que consuma esa infraestructura.

**Proximo Paso:** RECHAZADO - Crear los tres archivos de plan faltantes antes de proceder a implementacion:
1. `plans/cp-perfil-promotor/frontend-admin/frontend-plan.md`
2. `plans/cp-perfil-promotor/frontend-admin/ui-design.md`
3. `plans/cp-perfil-promotor/frontend-admin/test-strategy.md`

Una vez creados y con score >= 90%, proceder a implementacion en `src/admin`.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-25
