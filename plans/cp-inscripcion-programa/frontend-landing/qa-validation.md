# Validacion QA: cp-inscripcion-programa (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/web (Landing publica)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (ACs que afectan Landing) | 6 |
| Cubiertos | 3 |
| Parcialmente Cubiertos | 2 |
| No Cubiertos | 1 |
| **Score de Cobertura** | **58%** |

**Estado:** REQUIERE CAMBIOS

> El plan de contratos shared (`contracts-plan.md`) cubre correctamente los tipos, schemas y constantes necesarios para la Landing. Sin embargo, no existe ningun plan de implementacion especifico para la Landing (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`). Los gaps identificados son consecuencia directa de esa ausencia: los contratos shared son condicion necesaria pero no suficiente para validar la implementacion de los componentes, paginas, hooks y tests de la Landing.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|---------|
| AC-CP03-1 | Listado paginado de programas activos en /crowdpromotion/explorar con titulo, artista, tipo, comision, moneda, tareas, campana, fechas y estado personal (miEstado) | Funcional | Backend + Landing |
| AC-CP03-2 | Filtros por nombre de artista y tipo de programa sobre el listado | Funcional | Backend + Landing |
| AC-CP03-6 | Promotor con estado bloqueado no ve boton de solicitud; backend retorna 403 si se intenta por API | Funcional | Backend + Landing |
| AC-CP03-14 | Promotor ve CodigoReferido y UrlTrackingPersonalizada solo cuando EsAprobado = true | Funcional | Backend + Landing |
| AC-CP03-15 | /promotor/mis-programas muestra listado con badge de estado (Pendiente/Aprobado/Bloqueado/Baja) | Funcional | Backend + Landing |
| AC-CP03-16 | Tipos y schemas Zod de inscripcion definidos en shared y reutilizados por landing y admin | Funcional | Shared |
| RNF-03 | CodigoReferido y UrlTrackingPersonalizada solo se devuelven al promotor propietario o al artista del programa (endpoints autenticados) | No Funcional | Backend + Landing |
| RNF-04 | Campo miEstado se calcula solo para usuarios autenticados; para no autenticados se omite o devuelve null | No Funcional | Backend + Landing |
| FA-01 | Promotor ya inscrito: no mostrar boton de solicitud; mostrar badge con estado actual | Flujo Alternativo | Landing |
| FA-02 | Promotor bloqueado: mostrar mensaje "No puedes inscribirte"; no mostrar boton de solicitud | Flujo Alternativo | Landing |
| FA-03 | Programa inactivo o fuera de fechas: no aparece en listado publico | Flujo Alternativo | Backend (filtro) |
| FA-04 | Promotor inactivo: backend retorna 400; frontend debe mostrar el mensaje de error | Flujo Alternativo | Backend + Landing |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales (ACs con impacto Landing)

| ID | Criterio | contracts-plan (shared) | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|------------------------|---------------|-----------|---------------|--------|
| AC-CP03-1 | Listado paginado /crowdpromotion/explorar con miEstado | `ProgramaExplorarItem`, `ProgramasExplorarResponse`, `QUERY_KEYS.crowdpromotion.programas.explorar`, `APP_ROUTES.landing.crowdpromotion.explorar`, `API_ROUTES.crowdpromotion.programas.explorar` | No existe | No existe | No existe | PARCIAL |
| AC-CP03-2 | Filtros artista y tipo de programa | `ExplorarProgramasFilters`, `explorarProgramasFiltersSchema`, `ExplorarProgramasFiltersData`, `INSCRIPCION_NOMBRE_ARTISTA_FILTRO_MAX` | No existe | No existe | No existe | PARCIAL |
| AC-CP03-6 | Boton solicitud oculto para promotor bloqueado | `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_BLOCKED` (codigo '4022'), `MiInscripcion.esBloqueado` | No existe | No existe | No existe | NO CUBIERTO |
| AC-CP03-14 | CodigoReferido y URL solo visibles cuando EsAprobado = true | `MiInscripcion.codigoReferido?: string|null`, `MiInscripcion.urlTrackingPersonalizada?: string|null`, nota de implementacion sobre verificar `estado === 'Aprobado'`, `APP_ROUTES.landing.crowdpromotion.inscripcionDetalle` | No existe | No existe | No existe | PARCIAL |
| AC-CP03-15 | /promotor/mis-programas con badges de estado | `MiInscripcion`, `MisProgramasResponse`, `InscripcionEstado`, `INSCRIPCION_ESTADO_LABELS`, `INSCRIPCION_ESTADO_BADGE_VARIANT`, `getInscripcionBadgeVariant`, `APP_ROUTES.landing.crowdpromotion.misProgramas`, `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas` | No existe | No existe | No existe | PARCIAL |
| AC-CP03-16 | Tipos y schemas Zod en shared reutilizados por Landing | Totalmente cubierto: 10 interfaces + 1 union type + 2 schemas Zod + constantes + mappers en archivos de `src/shared/` | N/A | N/A | No existe | CUBIERTO |

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura en contracts-plan | Estado |
|----|----------|-----------------------------|--------|
| RNF-03 | CodigoReferido y UrlTracking solo al propietario | `MiInscripcion.codigoReferido: string|null` (null para no propietarios), nota de implementacion: verificar `estado === 'Aprobado'`. Depende de que el backend implemente la autorizacion; el contrato shared refleja la condicion (null vs valor) | PARCIAL |
| RNF-04 | miEstado solo para usuarios autenticados | `ProgramaExplorarItem.miEstado: InscripcionEstado|null` tipado como nullable; null para no autenticados. Contrato correcto. Implementacion de la guarda de autenticacion en Landing no planificada | PARCIAL |
| RNF-01 | Listado responde < 500ms (hasta 20 items) | Sin cobertura en ningun plan | NO CUBIERTO |

### 3.3 Flujos Alternativos (impacto Landing)

| ID | Condicion | Cobertura en contracts-plan | Estado |
|----|-----------|----------------------------|--------|
| FA-01 | Promotor ya inscrito: badge de estado sin boton | `ProgramaExplorarItem.miEstado` permite detectar el estado; `INSCRIPCION_ESTADO_BADGE_VARIANT` provee variante del badge | PARCIAL (falta logica de componente) |
| FA-02 | Promotor bloqueado: mensaje y sin boton | `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_BLOCKED` provee el mensaje; `miEstado === 'Bloqueado'` permite detectarlo | PARCIAL (falta logica de componente) |
| FA-03 | Programa inactivo: no aparece en listado | Responsabilidad del backend (filtro server-side); no requiere logica de Landing adicional | CUBIERTO (backend) |
| FA-04 | Promotor inactivo: backend 400, frontend muestra error | `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_INACTIVE` (codigo '4023') y `getInscripcionErrorMessage` proveen el mensaje | PARCIAL (falta manejo de error en hook/componente) |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-CP03-6 | Boton solicitud oculto para promotor bloqueado | No existe plan de componente que implemente la logica condicional de renderizado del boton segun `miEstado === 'Bloqueado'`. El contrato shared provee los tipos, pero la logica de presentacion no esta planificada en ningun documento de Landing | Alto - Requisito de seguridad UI; sin este comportamiento el promotor bloqueado ve el boton aunque el backend lo rechace | Crear `frontend-plan.md` que incluya componente `ProgramaDetalle` o `SolicitarInscripcionButton` con logica condicional: si `miEstado === 'Bloqueado'` mostrar mensaje; si `miEstado` tiene algun valor (FA-01) ocultar boton |
| AUSENCIA | No existen planes de implementacion Landing | Los archivos `frontend-plan.md`, `ui-design.md` y `test-strategy.md` para `plans/cp-inscripcion-programa/frontend-landing/` no han sido generados | Alto - Sin estos planes no es posible validar que la implementacion cumpla los criterios; el score de cobertura no puede superar el nivel de los contratos shared | Generar los tres planes antes de iniciar la implementacion Landing |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-CP03-1 | Listado paginado /crowdpromotion/explorar | Los contratos shared definen los tipos y rutas necesarios, pero no existe plan para: pagina `/crowdpromotion/explorar`, hook `useExplorarProgramas`, servicio que llame `API_ROUTES.crowdpromotion.programas.explorar`, componente `ProgramaCard` o `ProgramaExplorarList`, ni estado de carga/error/empty | Medio-Alto - La pagina es el punto de entrada principal del promotor | En `frontend-plan.md`: planificar pagina `ExplorarProgramasPage`, hook `useExplorarProgramas` con `QUERY_KEYS.crowdpromotion.programas.explorar`, componentes `ProgramaExplorarCard` y `ProgramaExplorarList`. En `ui-design.md`: definir layout de la grid de cards, estado empty, skeleton de carga |
| AC-CP03-14 | Vista de CodigoReferido y UrlTracking | El contrato define los campos como `string|null` y la nota de implementacion advierte sobre verificar `estado === 'Aprobado'`. Sin embargo no existe plan para: pagina de detalle `/promotor/mis-programas/{id}`, componente `CodigoReferidoCard` con boton de copiar, ni flujo de navegacion desde la lista | Medio-Alto - Funcionalidad central para el promotor aprobado | En `frontend-plan.md`: planificar `InscripcionDetallePage` (ruta `APP_ROUTES.landing.crowdpromotion.inscripcionDetalle`), componente `CodigoReferidoCard` con `navigator.clipboard.writeText`, guard de estado `estado !== 'Aprobado'` muestra empty state |
| AC-CP03-15 | /promotor/mis-programas con badges | Los contratos provistos (`MisProgramasResponse`, `INSCRIPCION_ESTADO_BADGE_VARIANT`, `getInscripcionBadgeVariant`) cubren los datos. Falta plan para: pagina `MisProgramasPage`, hook `useMisProgramas`, componente `InscripcionBadge` que use `getInscripcionBadgeVariant`, empty state con enlace a explorar | Medio - Sin este plan no se garantiza que los badges se implementen con los variants correctos de shadcn | En `frontend-plan.md`: planificar `MisProgramasPage`, hook `useMisProgramas(page)` con `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas(page)`, componente `InscripcionEstadoBadge` que use `getInscripcionBadgeVariant`. En `ui-design.md`: definir tabla o lista de inscripciones con badge por fila |
| RNF-04 | miEstado solo para usuarios autenticados | El tipo `ProgramaExplorarItem.miEstado: InscripcionEstado|null` es correcto. Pero no existe plan para el guard de autenticacion en la pagina de explorar: si el usuario no esta autenticado, `miEstado` sera null en todas las cards y no deberia mostrarse ningun indicador de estado personal | Medio - Sin este comportamiento un usuario anonimo podria ver la UI de estado en blanco en lugar de la version publica del catalogo | En `frontend-plan.md`: documentar que `ExplorarProgramasPage` usa el contexto de autenticacion para decidir si renderizar el indicador de `miEstado`; si no autenticado, mostrar boton "Iniciar sesion para solicitar" |
| FA-02 | Mensaje para promotor bloqueado | El mensaje `'No puedes inscribirte en este programa.'` existe en `INSCRIPCION_ERROR_MESSAGES`. Falta el plan del componente que muestre este mensaje inline en la vista de detalle del programa (no como toast, sino como alerta persistente) | Medio - El comportamiento diferenciado entre FA-01 (badge) y FA-02 (mensaje) no esta planificado | En `ui-design.md`: definir el estado visual de la vista de detalle cuando `miEstado === 'Bloqueado'`: alert componente de shadcn con texto de `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_BLOCKED` |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-CP03-2 | Filtros de busqueda | El schema `explorarProgramasFiltersSchema` y los tipos `ExplorarProgramasFilters` existen. Falta plan para: formulario de filtros con `React Hook Form + Zod`, debounce en el filtro de texto, comportamiento al limpiar filtros, y si los filtros persisten en URL query params | Bajo-Medio | En `frontend-plan.md`: agregar componente `ExplorarFiltrosForm` con `zodResolver(explorarProgramasFiltersSchema)`; en `ui-design.md`: definir si los filtros son un panel lateral, top bar o modal |
| RNF-01 | Respuesta < 500ms | No hay plan de optimizacion de rendimiento en ninguno de los archivos existentes | Bajo (es un RNF del backend principalmente, pero el frontend debe usar paginacion y no cargar todos los items) | En `frontend-plan.md`: documentar que `pageSize` por defecto debe ser 20 (`INSCRIPCION_PAGE_SIZE_MAX = 50`) y que la paginacion es obligatoria desde el primer render |
| FA-04 | Error promotor inactivo | El mensaje existe en `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_INACTIVE`. No existe plan para el manejo del error en el hook de mutacion de solicitud de inscripcion | Bajo | En `frontend-plan.md`: documentar que el hook `useSolicitarInscripcion` usa `getInscripcionErrorMessage(errorCode)` en el `onError` de la mutacion para mostrar toast con el mensaje correspondiente |
| AC-CP03-16 | Tests de shared reutilizado por Landing | El criterio exige que los tipos y schemas sean reutilizados. No existe `test-strategy.md` que valide que los imports de Landing provienen de `src/shared` y no de duplicados locales | Bajo | En `test-strategy.md`: incluir test de que los componentes y hooks de Landing importan desde `@shared/types/crowdpromotion` y `@shared/schemas/crowdpromotion.schema` |

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Estado |
|----------|------------------|------|--------|
| AC-CP03-1 | No planificado | - | NO CUBIERTO |
| AC-CP03-2 | No planificado | - | NO CUBIERTO |
| AC-CP03-6 | No planificado | - | NO CUBIERTO |
| AC-CP03-14 | No planificado | - | NO CUBIERTO |
| AC-CP03-15 | No planificado | - | NO CUBIERTO |
| AC-CP03-16 | No planificado (contratos si existen) | - | NO CUBIERTO |

### Tests Faltantes (requeridos)

| Criterio | Test Requerido | Tipo | Razon |
|----------|----------------|------|-------|
| AC-CP03-1 | `ExplorarProgramasPage.test.tsx` | Integration | Validar renderizado de listado paginado con `ProgramasExplorarResponse` mockeado |
| AC-CP03-1 | `useExplorarProgramas.test.ts` | Unit | Validar que el hook construye correctamente los query params y llama al endpoint correcto |
| AC-CP03-2 | `ExplorarFiltrosForm.test.tsx` | Unit | Validar schema Zod en filtros: artista max 200 chars, tipoPromoId positivo |
| AC-CP03-6 | `SolicitarInscripcionButton.test.tsx` | Unit | Validar que si `miEstado === 'Bloqueado'` el boton no se renderiza y se muestra el mensaje de bloqueo |
| AC-CP03-6 | `useSolicitarInscripcion.test.ts` | Unit | Validar que el error code '4022' se mapea al mensaje correcto via `getInscripcionErrorMessage` |
| AC-CP03-14 | `CodigoReferidoCard.test.tsx` | Unit | Validar que los campos solo se muestran cuando `estado === 'Aprobado'` y que el boton de copiar llama a `navigator.clipboard.writeText` |
| AC-CP03-14 | `InscripcionDetallePage.test.tsx` | Integration | Validar que estados Pendiente/Bloqueado/Baja no muestran el codigo referido |
| AC-CP03-15 | `MisProgramasPage.test.tsx` | Integration | Validar badges con variants correctos para cada `InscripcionEstado`; validar empty state con enlace a explorar |
| AC-CP03-15 | `InscripcionEstadoBadge.test.tsx` | Unit | Validar que `getInscripcionBadgeVariant` devuelve el variant correcto por estado |
| RNF-03 | `InscripcionDetallePage.test.tsx` | Integration | Validar que la pagina de detalle no es accesible sin autenticacion (redirige a login) |
| RNF-04 | `ExplorarProgramasPage.test.tsx` | Integration | Validar que sin auth el indicador de `miEstado` no se renderiza |
| FA-02 | `ProgramaDetallePage.test.tsx` | Integration | Validar mensaje de bloqueo cuando `miEstado === 'Bloqueado'` vs badge cuando `miEstado === 'Pendiente'` |

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Ruta | Planificada en ui-design | Estado |
|------------------|------|--------------------------|--------|
| Catalogo de explorar programas | `/crowdpromotion/explorar` | No (`ui-design.md` no existe) | NO CUBIERTO |
| Detalle de programa con boton de solicitud | `/crowdpromotion/explorar/{id}` (o modal) | No | NO CUBIERTO |
| Mis Programas - listado | `/promotor/mis-programas` | No | NO CUBIERTO |
| Detalle de inscripcion aprobada (codigo + URL) | `/promotor/mis-programas/{id}` | No | NO CUBIERTO |

> Nota: las rutas para estas screens SI estan definidas en `contracts-plan.md` seccion 4.3 (`APP_ROUTES.landing.crowdpromotion`), pero no existe ningun documento que especifique el layout, los componentes visuales, ni los estados de la UI.

### Estados de UI Requeridos

| Estado | Screen | Requerido por | Planificado | Estado |
|--------|--------|--------------|-------------|--------|
| Loading (skeleton) | Explorar programas | Buena practica + RNF-01 | No | NO CUBIERTO |
| Error (fetch fallido) | Explorar programas | Buena practica | No | NO CUBIERTO |
| Empty (sin programas activos) | Explorar programas | FA-03 implica que puede no haber resultados | No | NO CUBIERTO |
| No inscrito (boton "Solicitar") | Detalle de programa | Flujo principal paso 6 | No | NO CUBIERTO |
| Inscrito - Pendiente (badge, sin boton) | Detalle de programa | FA-01 | No | NO CUBIERTO |
| Inscrito - Aprobado (badge, sin boton) | Detalle de programa | FA-01 | No | NO CUBIERTO |
| Inscrito - Bloqueado (alerta, sin boton) | Detalle de programa | AC-CP03-6 + FA-02 | No | NO CUBIERTO |
| Empty (sin inscripciones) | Mis Programas | Flujo secundario paso 2 | No | NO CUBIERTO |
| Loading | Mis Programas | Buena practica | No | NO CUBIERTO |
| Aprobado con codigo referido | Detalle inscripcion | AC-CP03-14 | No | NO CUBIERTO |
| No aprobado sin codigo referido | Detalle inscripcion | AC-CP03-14 (negativo) | No | NO CUBIERTO |
| Toast exito solicitud | Cualquier screen con boton | Flujo principal paso 10 | No | NO CUBIERTO |
| Toast error (4022, 4023, 4024) | Cualquier screen con boton | FA-02, FA-04 | No | NO CUBIERTO |

---

## 7. Recomendaciones

### Acciones Requeridas (Critico)

1. **Generar `frontend-plan.md` para Landing**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/frontend-plan.md`
   - Cambio: Planificar las dos paginas principales (`ExplorarProgramasPage`, `MisProgramasPage`), la pagina de detalle de inscripcion (`InscripcionDetallePage`), los hooks (`useExplorarProgramas`, `useMisProgramas`, `useSolicitarInscripcion`), los servicios que consumen `API_ROUTES.crowdpromotion.programas.explorar` y `API_ROUTES.crowdpromotion.promotorInscripciones.misProgramas`, y los componentes compartidos (`ProgramaExplorarCard`, `InscripcionEstadoBadge`, `CodigoReferidoCard`)

2. **Documentar la logica condicional del boton de solicitud (AC-CP03-6)**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/frontend-plan.md`
   - Cambio: El componente responsable del boton debe evaluar `miEstado`:
     - `null` → mostrar boton "Solicitar inscripcion"
     - `'Pendiente'` | `'Aprobado'` | `'DadoDeBaja'` → ocultar boton, mostrar badge con `getInscripcionBadgeVariant(miEstado)` (FA-01)
     - `'Bloqueado'` → ocultar boton, mostrar alerta con `INSCRIPCION_ERROR_MESSAGES.INSCRIPCION_PROMOTOR_BLOCKED` (FA-02, AC-CP03-6)

### Acciones Sugeridas (Mayor)

3. **Generar `ui-design.md` para Landing**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/ui-design.md`
   - Cambio: Definir el layout de la pagina de explorar (grid de cards vs lista), los estados de la card de programa (normal, con badge de estado personal), el layout de `/promotor/mis-programas` (tabla o lista con badge), y la pantalla de detalle de inscripcion aprobada con el componente de codigo referido (con boton de copiar y URL de tracking)

4. **Definir manejo de autenticacion para miEstado (RNF-04)**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/frontend-plan.md`
   - Cambio: Documentar que `ExplorarProgramasPage` verifica el estado de autenticacion antes de renderizar el indicador de `miEstado`. Si el usuario no esta autenticado, las cards muestran un CTA generico "Inicia sesion para solicitar" en lugar del badge/boton

5. **Definir flujo de paginacion en Mis Programas**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/frontend-plan.md`
   - Cambio: El hook `useMisProgramas(page)` debe usar `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas(page)` y `pageSize` por defecto de 10 (no 50 que es el maximo); el componente de paginacion debe sincronizar `page` con el estado de la URL (query param `?page=N`)

6. **Planificar invalidacion de cache tras solicitud exitosa**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/frontend-plan.md`
   - Cambio: El hook `useSolicitarInscripcion` debe invalidar `crowdpromotion.programas.explorar` y `crowdpromotion.inscripciones.misProgramas` en `onSuccess`, segun la tabla de invalidacion documentada en `contracts-plan.md` seccion 8

### Acciones Sugeridas (Mayor) - continuacion

7. **Generar `test-strategy.md` para Landing**
   - Archivo: `plans/cp-inscripcion-programa/frontend-landing/test-strategy.md`
   - Cambio: Definir tests unitarios para componentes con logica condicional (`SolicitarInscripcionButton`, `InscripcionEstadoBadge`, `CodigoReferidoCard`) y tests de integracion para las tres paginas principales, cubriendo los estados de UI identificados en la seccion 6

### Nice to Have (Menor)

8. **Persistencia de filtros en URL**
   - Recomendacion: En `frontend-plan.md` documentar si los filtros de `/crowdpromotion/explorar` se sincronizan con query params de la URL (`?artistaNombre=xxx&tipoPromoId=1`) para permitir compartir enlaces filtrados

9. **Optimizacion de rendimiento para RNF-01**
   - Recomendacion: En `frontend-plan.md` mencionar el uso de `staleTime` en `useQuery` para `/explorar` (e.g., 30 segundos) para reducir requests repetidos en navegacion rapida entre detalle y listado

10. **Estado de carga optimista tras solicitud de inscripcion**
    - Recomendacion: En `frontend-plan.md` documentar el uso de `onMutate` en `useSolicitarInscripcion` para actualizar optimistamente `miEstado` a `'Pendiente'` en la cache de `explorar` antes de la confirmacion del servidor, usando `mapInscripcionEstado` del mapper de shared

---

## 8. Checklist de Validacion

### Requisitos Funcionales
- [x] AC-CP03-16: Contratos shared completamente planificados y cubiertos
- [ ] AC-CP03-1: Pagina `/crowdpromotion/explorar` con listado paginado y miEstado - sin plan
- [ ] AC-CP03-2: Filtros por artista y tipo de programa - sin plan de componente
- [ ] AC-CP03-6: Logica condicional del boton de solicitud para promotor bloqueado - sin plan
- [ ] AC-CP03-14: Vista de CodigoReferido y UrlTracking condicionada a estado Aprobado - sin plan
- [ ] AC-CP03-15: /promotor/mis-programas con badges usando `getInscripcionBadgeVariant` - sin plan
- [ ] FA-01: Badge de estado para promotor ya inscrito - sin plan de componente
- [ ] FA-02: Mensaje de bloqueo diferenciado del badge - sin plan de componente
- [ ] FA-04: Manejo de error 400 (promotor inactivo) en hook de solicitud - sin plan

### UI/UX
- [ ] Screens `/crowdpromotion/explorar`, `/promotor/mis-programas` y detalle planificadas
- [ ] Estados de interaccion definidos (loading, error, empty, no-inscrito, pendiente, aprobado, bloqueado)
- [ ] Responsive design considerado en `ui-design.md`
- [ ] Accesibilidad de badges y formulario de filtros validada
- [x] Variants de Badge de shadcn definidos en `INSCRIPCION_ESTADO_BADGE_VARIANT` (contracts-plan)

### Testing
- [ ] Tests para criterios criticos (AC-CP03-6: boton bloqueado, AC-CP03-14: datos sensibles)
- [ ] Tests de integracion para las tres paginas
- [ ] Cobertura objetivo de 80% definida en `test-strategy.md`
- [x] Mensajes de error para todos los codigos de negocio definidos en `INSCRIPCION_ERROR_MESSAGES`

---

## 9. Conclusion

**Score Final:** 58%

El plan de contratos shared (`contracts-plan.md`) esta bien estructurado y cubre completamente el criterio AC-CP03-16, proporcionando todos los tipos TypeScript, schemas Zod, constantes de dominio, mensajes de error y mappers necesarios para la implementacion de Landing. Este es el fundamento correcto sobre el que construir.

Sin embargo, el score de 58% refleja que los planes de implementacion especificos para la Landing (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) no han sido generados. Estos son los documentos que deben cubrir los criterios de aceptacion AC-CP03-1, AC-CP03-2, AC-CP03-6, AC-CP03-14 y AC-CP03-15, todos ellos con impacto directo en la Landing.

El gap mas critico es AC-CP03-6 (boton de solicitud oculto para promotor bloqueado), que es un requisito de seguridad UI. Aunque el backend valida via 403, la UX correcta requiere que el frontend evalue `miEstado === 'Bloqueado'` y nunca renderice el boton, evitando llamadas fallidas predecibles.

**Veredicto:** REQUIERE CAMBIOS

**Proximo Paso:** Generar los tres planes faltantes para `plans/cp-inscripcion-programa/frontend-landing/`:
1. `frontend-plan.md` - Componentes, hooks, servicios y rutas de Landing
2. `ui-design.md` - Layouts, estados visuales y especificaciones de UI
3. `test-strategy.md` - Estrategia de tests unitarios e integration para Landing

Una vez generados esos planes y resueltos los gaps criticos y mayores identificados, se debe ejecutar una segunda ronda de validacion QA. El score esperado tras resolver todos los gaps es 90%+.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-25
