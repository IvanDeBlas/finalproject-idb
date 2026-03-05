# Validacion QA: cs-valoraciones (Landing)

**Fecha:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (AC + FA + NFR) | 22 |
| Cubiertos (Shared plan cubre contratos) | 14 |
| Parcialmente Cubiertos | 5 |
| No Cubiertos | 3 |
| **Score de Cobertura** | **63.6%** |

> **Nota metodologica:** Se validan los planes disponibles: `contracts-plan.md` (shared) y los documentos fuente `contracts.md` y `ui-ux.md`. Los planes `frontend-plan.md` y `test-strategy.md` **NO han sido generados todavia** para el target `frontend-landing`. Esta es la causa principal del score bajo y de la mayoria de los gaps. El score refleja el estado actual de planificacion, no una deficiencia de la feature.

**Estado:** REQUIERE CAMBIOS

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

#### Criterios de Aceptacion Funcionales

| ID | Criterio | Tipo | Proyectos |
|----|----------|------|-----------|
| AC-CS06-1 | Solo valorar acuerdos en estado Completado. Backend retorna 400 con BusinessRule_InvalidState. | Funcional | Backend + Landing |
| AC-CS06-2 | Una valoracion por acuerdo por participante. 400 en segundo intento. Constraint unico en BD. | Funcional | Backend + Landing |
| AC-CS06-3 | Puntuacion 1-5 con hover effect visual. No permite valores fuera de rango. | Funcional | Landing |
| AC-CS06-4 | Mensaje incentivo visible. Comentario no obligatorio. Envio sin comentario valido. | Funcional | Landing |
| AC-CS06-5 | Inmutable tras envio. Toast exacto: "Valoracion enviada. Gracias por tu feedback." Read-only post-envio. | Funcional | Backend + Landing |
| AC-CS06-6 | Cualquier usuario autenticado consulta valoraciones via GET /api/crowdsourcing/usuarios/{userId}/valoraciones. | Funcional | Backend |
| AC-CS06-7 | Resumen con AVG 1 decimal, total, distribucion histograma (GROUP BY). | Funcional | Backend |
| AC-CS06-8 | Ordenacion fecha DESC. Paginacion page/pageSize. Respuesta incluye totalCount, page, pageSize. | Funcional | Backend + Landing |
| AC-CS06-9 | Empty state "Este usuario aun no tiene valoraciones". | Funcional | Landing |

#### Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Acuerdo no completado | CTA no visible |
| FA-02 | Ya valoro | Read-only |
| FA-03 | Sin valoraciones | Mensaje empty |
| FA-04 | Segunda valoracion via API | 400 |
| FA-05 | Puntuacion fuera de rango | 400 + frontend impide |
| FA-06 | Comentario > 1000 | 400 + contador + bloquea envio |

#### Requisitos No Funcionales

| ID | Criterio | Tipo |
|----|----------|------|
| NFR-01 | Query resumen SQL (AVG, COUNT, GROUP BY), no en memoria | Performance |
| NFR-02 | Paginacion con Skip/Take en query, no cargar todos en memoria | Performance |
| NFR-03 | IRequestCacheService para compartir lectura del acuerdo entre validator y handler | Performance |
| NFR-04 | GET resumen + pagina 1 responde en menos de 300ms | Performance |
| NFR-05 | UserIdValorado determinado en backend desde JWT, no aceptado del cliente | Seguridad |
| NFR-06 | No existe endpoint PATCH/PUT de valoracion | Seguridad |
| NFR-07 | Responsive design (mobile < 768px, tablet 768-1024px, desktop > 1024px) | UX |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio Resumido | contracts-plan (shared) | ui-ux.md | frontend-plan.md | test-strategy.md | Estado |
|----|-------------------|-------------------------|----------|------------------|------------------|--------|
| AC-CS06-1 | Solo valorar acuerdos Completados | Schema `createValoracionSchema`, errores `4007`/`BusinessRule_InvalidState` en `VALORACION_ERROR_MESSAGES`. `ESTADO_ACUERDO.COMPLETADO` ya en constants. | Card no se renderiza si `estado != Completado`. FA-01 documentado. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-2 | Una valoracion por acuerdo | `VALORACION_ERROR_MESSAGES['4014']` y key semantico `VALORACION_DUPLICATE`. Error code `4014` definido en `contracts.md`. | FA-02: ya valorado -> read-only documentado. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-3 | Puntuacion 1-5 con hover effect | Schema `.gte(1).lte(5)`. Type `CreateValoracionRequest.puntuacion`. Error `1014` en `VALORACION_ERROR_MESSAGES`. | `StarRating` especificado con hover `scale-110` + amber, `role="radiogroup"`. Animaciones definidas. Breakpoints responsive definidos. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-4 | Mensaje incentivo + comentario opcional | Schema `comentario: z.string().max(1000).optional()`. Type `puntuacion: number` (no opcional). | Mensaje "Tu comentario ayuda a otros artistas/profesionales" especificado. Label "(opcional)" documentado. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-5 | Inmutable + toast exacto + read-only | `getValoracionErrorMessage` y toast desde `messages[0].message`. `API_ROUTES.crowdsourcing.valoraciones.create`. | Toast "Valoracion enviada. Gracias por tu feedback." especificado. Card read-only con borde verde especificado. Transicion `animate-in fade-in-0 slide-in-from-bottom-2` documentada. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-6 | Cualquier usuario autenticado consulta valoraciones | `API_ROUTES.crowdsourcing.valoraciones.byUser`. Type `ValoracionesUsuario`. `QUERY_KEYS.crowdsourcing.valoraciones.byUserId`. | Seccion de valoraciones en perfil documentada. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-7 | Resumen AVG, total, histograma | Types `ValoracionResumen` con `puntuacionMedia: number | null`, `totalValoraciones: number`, `distribucion: {5,4,3,2,1}`. Numeric literal keys garantizan las 5 claves. | `RatingHistogram` con 5 barras. Card resumen con puntuacion media en texto grande. Animacion de barras 500ms. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-8 | Orden fecha DESC + paginacion | `valoracionesQuerySchema` con `page` y `pageSize`. Type `PaginatedResult<T>` con `totalCount`, `page`, `pageSize`. `ValoracionesUsuario.valoraciones: PaginatedResult<ValoracionListItem>`. | Controles de paginacion especificados (prev/next/numeros). Scroll suave al cambiar pagina. Paginacion simplificada en mobile. | NO GENERADO | NO GENERADO | PARCIAL |
| AC-CS06-9 | Empty state con texto exacto | `getValoracionErrorMessage`. Logica de `totalValoraciones === 0` en `ValoracionResumen`. | `EmptyValoraciones` con texto exacto "Este usuario aun no tiene valoraciones" + subtexto "Completa un acuerdo para recibir tu primera valoracion". Icono `<Star>` en gris. | NO GENERADO | NO GENERADO | PARCIAL |

### 3.2 Flujos Alternativos

| ID | Condicion | contracts-plan (shared) | ui-ux.md | frontend-plan.md | test-strategy.md | Estado |
|----|-----------|-------------------------|----------|------------------|------------------|--------|
| FA-01 | Acuerdo no completado -> CTA oculto | `ESTADO_ACUERDO.COMPLETADO = 2` en constants. Error `4007` en `VALORACION_ERROR_MESSAGES`. | Estado "Acuerdo no completado": card completo no se renderiza. | NO GENERADO | NO GENERADO | PARCIAL |
| FA-02 | Ya valoro -> read-only | `VALORACION_ERROR_MESSAGES['4014']`. `VALORACION_DUPLICATE` key semantico. | `ValoracionReadOnly` documentado. Borde verde semitransparente. FA-02 descrito en feature-spec. | NO GENERADO | NO GENERADO | PARCIAL |
| FA-03 | Sin valoraciones -> empty state | `totalValoraciones: number` en `ValoracionResumen`. | `EmptyValoraciones` especificado con texto exacto. | NO GENERADO | NO GENERADO | PARCIAL |
| FA-04 | Segunda valoracion via API -> 400 | Error `4014` definido. `BusinessRule_DuplicateAction = "4014"` en contracts.md. | No aplicable (es comportamiento backend). | NO GENERADO | NO GENERADO | CUBIERTO (backend via contracts) |
| FA-05 | Puntuacion fuera de rango -> 400 + impide | Schema `gte(1).lte(5)`. Error `1014` en `VALORACION_ERROR_MESSAGES`. `Validation_InvalidRange = "1014"` en contracts.md. | `StarRating` no permite valores fuera del rango. | NO GENERADO | NO GENERADO | PARCIAL |
| FA-06 | Comentario > 1000 -> 400 + contador | Schema `z.string().max(1000)`. Key `VALORACION_COMENTARIO_MAX`. Error `1002` en contracts.md. | Contador caracteres "{n} / 1000" siempre visible. Ambar >900, rojo >1000. Boton disabled al exceder. | NO GENERADO | NO GENERADO | PARCIAL |

### 3.3 Requisitos No Funcionales

| ID | Criterio | contracts-plan (shared) | ui-ux.md | Estado |
|----|----------|-------------------------|----------|--------|
| NFR-01 | AVG/COUNT/GROUP BY en SQL | N/A (backend) | N/A | CUBIERTO (documentado en feature-spec y contracts.md) |
| NFR-02 | Paginacion con Skip/Take | `PaginatedResult<T>`. `valoracionesQuerySchema` con page/pageSize. | Controles de paginacion con disabled states durante carga. | CUBIERTO |
| NFR-03 | IRequestCacheService en backend | N/A (backend) | N/A | CUBIERTO (documentado en contracts.md seccion backend) |
| NFR-04 | < 300ms respuesta | N/A (backend) | N/A | CUBIERTO (definido como NFR en feature-spec) |
| NFR-05 | UserIdValorado desde JWT en backend | `CreateValoracionRequest` no incluye campo `userIdValorado`. El body solo tiene `puntuacion` + `comentario`. | N/A | CUBIERTO |
| NFR-06 | No existe PATCH/PUT | N/A (backend) | `ValoracionReadOnly` sin controles de edicion. | CUBIERTO |
| NFR-07 | Responsive design | N/A | Breakpoints mobile/tablet/desktop documentados con wireframes. Estrellas `size="md"` en mobile para tap. Histograma en columna unica en mobile. Paginacion simplificada en mobile. | CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-01 | Todos los AC y FA de Landing | `frontend-plan.md` no ha sido generado. Sin plan de componentes, hooks ni service para Landing. | **Alto** - No se puede iniciar implementacion de componentes sin este plan. | Generar `plans/cs-valoraciones/frontend-landing/frontend-plan.md` con: lista de componentes (`StarRating`, `StarDisplay`, `ValoracionForm`, `ValoracionReadOnly`, `ValoracionesSection`, `ValoracionListItem`, `RatingHistogram`, `RatingBadge`, `EmptyValoraciones`), hooks (`useCreateValoracion`, `useValoracionesByUser`), service (`valoracion.service.ts`) y logica condicional en `AcuerdoDetallePage`. |
| GAP-02 | Todos los AC | `test-strategy.md` no ha sido generado. Sin estrategia de tests para Landing. | **Alto** - No hay criterio de done en testing. Los AC de validacion, estados UI y flujos de error no tienen tests planificados. | Generar `plans/cs-valoraciones/frontend-landing/test-strategy.md` con tests unitarios de `StarRating` (hover, click, teclado), `ValoracionForm` (submit, validacion), tests de integracion para `useCreateValoracion` y `useValoracionesByUser`, y E2E para los flujos completos. |
| GAP-03 | AC-CS06-5 (toast exacto) | El texto exacto del toast ("Valoracion enviada. Gracias por tu feedback.") viene de `messages[0].message` del backend. El plan shared documenta el mecanismo pero no define explicitamente como el hook extrae el mensaje. Si la respuesta cambia el campo `message`, el toast mostraria texto incorrecto. | **Alto** - AC-CS06-5 es un criterio de aceptacion con texto exacto. | En `frontend-plan.md`, especificar que `useCreateValoracion` extrae `response.messages[0].message` para el toast y define un fallback hardcodeado si `messages` esta vacio. Agregar un test unitario que verifique que el texto del toast es exactamente el especificado. |

### 4.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-04 | AC-CS06-3 (hover effect) | El componente `StarRating` con hover effect esta especificado en `ui-ux.md` pero no existe todavia. El plan shared no incluye la libreria a usar (custom vs libreria externa). El riesgo de `shadcn/ui` sin StarRating nativo esta documentado en feature-spec pero no resuelto en un plan. | **Medio** - Decision tecnica pendiente que afecta la implementacion. | En `frontend-plan.md`, registrar la decision: implementar `StarRating` custom con Tailwind + Lucide `<Star>` icon (enfoque recomendado para MVP dado el control total y sin dependencias extras). Documentar la especificacion del state interno `hoverValue`. |
| GAP-05 | AC-CS06-8 (paginacion Landing) | El schema `valoracionesQuerySchema` esta definido en shared pero no hay hook que lo use. El plan no especifica como se mantiene el estado de pagina actual en el componente `ValoracionesSection` (URL params vs state local). | **Medio** - Sin decision tomada el implementador podria elegir patron inconsistente con el resto del proyecto. | En `frontend-plan.md`, especificar que `useValoracionesByUser` recibe `params: ValoracionesQueryParams` como argumento, que el estado de paginacion se mantiene con `useState` local en `ValoracionesSection` (no en URL para MVP), y que al cambiar de pagina se invalida el query y hace scroll suave al inicio de la seccion. |
| GAP-06 | Invalidacion de cache post-valoracion | `contracts-plan.md` identifica el problema: `userIdValorado` no viene en la respuesta 201, por lo que el hook no puede invalidar `valoraciones.byUserId(userIdValorado)` automaticamente. Este gap se documenta pero no se resuelve. | **Medio** - Sin solucion definida, las valoraciones del perfil del otro usuario no se actualizarian en tiempo real tras crear una valoracion. | En `frontend-plan.md`, definir la solucion: el componente padre `AcuerdoDetallePage` conoce los participantes del acuerdo y puede pasar `userIdValorado` al hook `useCreateValoracion(acuerdoId, userIdValorado)`. Especificar las dos invalidaciones: `valoraciones.byUserId(userIdValorado)` y `acuerdos.byId(acuerdoId)`. |
| GAP-07 | AC-CS06-6 + proteccion de rutas | La ruta `/profesionales/:userId` (seccion valoraciones) requiere auth. El plan shared no incluye la proteccion de ruta en el frontend. La tabla "Proteccion de Rutas Frontend" en contracts.md la documenta pero no hay plan de implementacion. | **Medio** - Las rutas sin proteccion exponen la seccion de valoraciones a usuarios no autenticados, contradiciendo AC-CS06-6. | En `frontend-plan.md`, verificar que `AcuerdoDetallePage` y la pagina de perfil de profesional ya tienen `PrivateRoute` (o equivalente en la app). Si no, agregar la proteccion como tarea explicita. |

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| GAP-08 | NFR Badge en tarjetas | El badge de puntuacion media en tarjetas de propuestas y listados de profesionales esta definido en `ui-ux.md` (Pantalla 3) pero no hay plan de en qué componentes existentes se integrara ni qué query se usara para obtener el dato. | **Bajo** - El badge es un nice-to-have para MVP segun la feature-spec (seccion landing notas). | En `frontend-plan.md`, listar los componentes existentes donde se integraria `RatingBadge` (p.ej. tarjeta de propuesta en el listado de propuestas de una necesidad, cabecera de perfil profesional) y especificar si se hace una llamada independiente por usuario o si el dato viene incluido en el DTO de la entidad. |
| GAP-09 | Tipo `ValoracionDto` vs `ValoracionCreatedResult` | El archivo `ui-ux.md` define el tipo `ValoracionDto` (interfaz de referencia local al documento) con propiedades ligeramente distintas al `ValoracionCreatedResult` del plan shared. `ValoracionDto.comentario` es `string | null` en ui-ux.md vs `string?` (undefined) en el contracts-plan. Esta inconsistencia menor podria causar confusion durante la implementacion. | **Bajo** - Solo es un riesgo de confusion en el implementador, no un error funcional. | En `frontend-plan.md`, referenciar explicitamente los types de `src/shared/types/crowdsourcing.ts` como fuente de verdad y marcar los tipos definidos en `ui-ux.md` como "solo referencia, usar los del shared". El tipo canonico de `autorImagenUrl` es `string | null` (no `undefined`), segun nota critica del contracts-plan. |
| GAP-10 | `ValoracionAcuerdoState` en ui-ux.md | `ui-ux.md` define `ValoracionAcuerdoState { yaValoro: boolean; valoracionExistente: ValoracionDto | null }` como type de referencia pero este type no esta incluido en `contracts-plan.md` (shared). Si se necesita que el backend devuelva este estado, debe incluirse en el DTO del acuerdo o consultarse por separado. | **Bajo** - El contracts-plan aclara que la condicion "ya valorado" se puede verificar con la respuesta 400 (codigo 4014) o via campo `miValoracion` del acuerdo, siendo esto ultimo opcional en MVP. | En `frontend-plan.md`, decidir si `AcuerdoCrowdsourcingDetailDto` incluye un campo `miValoracion: ValoracionCreatedResult | null`. Si no, documentar que el estado "ya valorado" se gestiona localmente con `useState` tras recibir la respuesta exitosa del POST. |

---

## 5. Validacion de Contratos

### 5.1 Alineacion DTOs TypeScript vs C#

| DTO TypeScript (contracts-plan) | DTO C# (contracts.md) | Alineacion | Nota |
|---------------------------------|-----------------------|------------|------|
| `CreateValoracionRequest` | `CreateValoracionRequestDto` | ALINEADO | `puntuacion: number` / `int Puntuacion`. `comentario?: string` / `string? Comentario`. Semantica identica. |
| `ValoracionCreatedResult` | `ValoracionCreatedResultDto` | ALINEADO | `id: string` / `Guid Id`. `puntuacion: number` / `int Puntuacion`. `comentario?: string` / `string? Comentario`. `fechaCreacion: string` / `DateTime FechaCreacion`. La conversion DateTime->ISO string es responsabilidad del serializador. |
| `ValoracionResumen` | `ValoracionResumenDto` | ALINEADO | `puntuacionMedia: number | null` / `decimal? PuntuacionMedia`. `totalValoraciones: number` / `int TotalValoraciones`. `distribucion: {5,4,3,2,1}` / `Dictionary<int, int> Distribucion`. Garantia de 5 claves documentada en ambos. |
| `ValoracionListItem` | `ValoracionListItemDto` | ALINEADO | Todos los campos mapeados. `autorImagenUrl: string | null` / `string? AutorImagenUrl`. Consistente. |
| `PaginatedResult<T>` | `PaginatedResult<T>` | ALINEADO | `items: T[]` / `List<T> Items`. `totalCount` / `TotalCount`. `page` / `Page`. `pageSize` / `PageSize`. |
| `ValoracionesUsuario` | `ValoracionesUsuarioDto` | ALINEADO | `resumen: ValoracionResumen` / `ValoracionResumenDto Resumen`. `valoraciones: PaginatedResult<ValoracionListItem>` / `PaginatedResult<ValoracionListItemDto> Valoraciones`. |

**Resultado:** Todos los DTOs del contracts-plan estan alineados con los C# definidos en contracts.md. No se identificaron discrepancias en nombres de campo o tipos.

### 5.2 Alineacion Schemas Zod vs FluentValidation

| Campo | FluentValidation (contracts.md) | Zod (contracts-plan) | Alineacion | Nota |
|-------|---------------------------------|----------------------|------------|------|
| `puntuacion` | `.NotNull().InclusiveBetween(1,5)`. ErrorCodes `1001` + `1014`. | `.number({required_error}).int().gte(1).lte(5)`. | ALINEADO | Zod usa `.int()` adicional para rechazar decimales (ej: 3.5). Apropiado. |
| `comentario` | `.MaximumLength(1000).When(!IsNullOrEmpty)`. ErrorCode `1002`. | `.string().max(1000).optional()`. | ALINEADO | Zod `.optional()` permite `undefined`. Backend acepta `null`. Diferencia menor sin impacto funcional. |
| `page` | Validado en Query handler (>= 1). | `.number().int().gte(1).optional().default(1)`. | ALINEADO | El default en Zod es conveniente para construir query strings. |
| `pageSize` | Validado en Query handler (1-50). | `.number().int().gte(1).lte(50).optional().default(10)`. | ALINEADO | Limite max 50 coincide con documentacion en contracts.md. |

**Resultado:** Los schemas Zod replican fielmente las reglas de FluentValidation. No hay divergencias.

### 5.3 Alineacion Constantes de Error

| Codigo | Backend (contracts.md) | Frontend (contracts-plan + error-messages.ts) | Estado |
|--------|------------------------|----------------------------------------------|--------|
| `1001` | `Validation_Required` - Puntuacion no enviada | Existe en `ERROR_CODE_MESSAGES["1001"]` | CUBIERTO |
| `1002` | `Validation_MaxLength` - Comentario > 1000 | Existe en `ERROR_CODE_MESSAGES["1002"]` | CUBIERTO |
| `1014` | `Validation_InvalidRange` - Puntuacion fuera 1-5 | Pendiente de agregar en `ERROR_CODE_MESSAGES` (solo en `VALORACION_ERROR_MESSAGES` del nuevo bloque) | PARCIAL |
| `4007` | `BusinessRule_InvalidState` - Acuerdo no completado | Existe en `ERROR_CODE_MESSAGES["4007"]` pero con mensaje de campania. Override correcto en `VALORACION_ERROR_MESSAGES`. `getValoracionErrorMessage` resuelve correctamente. | CUBIERTO (via override) |
| `4014` | `BusinessRule_DuplicateAction` - Ya valorado | Pendiente de agregar en `ERROR_CODE_MESSAGES` (solo en `VALORACION_ERROR_MESSAGES`) | PARCIAL |
| `2011` | `NotFound_Acuerdo` | Existe en `ERROR_CODE_MESSAGES["2011"]`. Override especifico en `VALORACION_ERROR_MESSAGES`. | CUBIERTO |
| `3001` | `Auth_Unauthorized` | Existe en `ERROR_CODE_MESSAGES["3001"]`. | CUBIERTO |
| `3002` | `Auth_Forbidden` - No es participante | Existe en `ERROR_CODE_MESSAGES["3002"]`. Override especifico en `VALORACION_ERROR_MESSAGES`. | CUBIERTO |
| `5000` | `Internal_UnexpectedError` | Existe en `ERROR_CODE_MESSAGES["5000"]`. | CUBIERTO |
| `2000` | `NotFound_Entity` - Usuario no encontrado | Existe en `ERROR_CODE_MESSAGES["2000"]`. | CUBIERTO |

**Nota sobre QUERY_KEYS:** El objeto `QUERY_KEYS.crowdsourcing` en `src/shared/constants/index.ts` actualmente **NO incluye** la clave `valoraciones`. El contracts-plan especifica agregarla entre `acuerdos` y `conversaciones`. Esta es una modificacion pendiente de implementar.

**Nota sobre API_ROUTES:** El objeto `API_ROUTES.crowdsourcing` en `src/shared/constants/index.ts` actualmente **NO incluye** la clave `valoraciones`. El contracts-plan especifica agregarla entre `entregables` y `conversaciones`. Esta es una modificacion pendiente de implementar.

---

## 6. Validacion de UI/UX

### 6.1 Screens Requeridas vs Planificadas

| Screen Requerida (feature-spec) | Planificada en ui-ux.md | Componentes Definidos | Estado |
|---------------------------------|------------------------|-----------------------|--------|
| Formulario de valoracion en AcuerdoDetallePage | SI - Pantalla 1 | `ValoracionForm`, `StarRating`, contador chars, boton submit | CUBIERTO |
| Vista read-only post-envio | SI - Pantalla 1 (estado "Ya valorado") | `ValoracionReadOnly` (nombrado `ValoracionEnviada` en ui-ux) | CUBIERTO |
| Seccion de valoraciones en perfil de usuario | SI - Pantalla 2 | `ValoracionesSection`, `ValoracionResumenCard`, `RatingHistogram`, `ValoracionListItem`, `EmptyValoraciones`, controles paginacion | CUBIERTO |
| Badge en tarjetas de propuestas y profesionales | SI - Pantalla 3 | `RatingBadge` (2 variantes: compact y medium) | CUBIERTO |

### 6.2 Estados de UI

| Estado | Requerido por feature-spec | Definido en ui-ux.md | Implementado en plans | Estado |
|--------|---------------------------|----------------------|-----------------------|--------|
| Acuerdo no completado (card oculto) | SI - FA-01 | SI | NO GENERADO | PARCIAL |
| Loading skeleton | SI | SI - con clases exactas de skeleton | NO GENERADO | PARCIAL |
| Formulario pendiente (default) | SI | SI - especificacion completa de estilos | NO GENERADO | PARCIAL |
| Puntuacion seleccionada | SI | SI - estrellas amber + texto "{n} de 5" | NO GENERADO | PARCIAL |
| Submitting con spinner | SI | SI - spinner `animate-spin` + inputs disabled | NO GENERADO | PARCIAL |
| Success (toast + read-only) | SI - AC-CS06-5 | SI - toast + `animate-in fade-in-0 slide-in-from-bottom-2` | NO GENERADO | PARCIAL |
| Error API (toast error) | SI | SI - toast "No se pudo enviar la valoracion. Intenta de nuevo." | NO GENERADO | PARCIAL |
| Already rated (read-only con borde verde) | SI - FA-02 | SI - borde `border-[#10b981]/40` + icono check | NO GENERADO | PARCIAL |
| Empty state en perfil | SI - FA-03 + AC-CS06-9 | SI - texto exacto + icono estrella gris + subtexto | NO GENERADO | PARCIAL |
| Error API en seccion perfil | SI | SI - `<Alert>` con boton "Reintentar" | NO GENERADO | PARCIAL |
| Cargando nueva pagina | SI | SI - spinner centrado + controles disabled | NO GENERADO | PARCIAL |

### 6.3 Interacciones

| Interaccion | Requerida | Definida en ui-ux.md | Implementada en plan | Estado |
|-------------|-----------|----------------------|----------------------|--------|
| Hover sobre estrella N ilumina 1..N | SI - AC-CS06-3 | SI - amber `#f59e0b` + `scale-110` + 100ms | NO GENERADO | PARCIAL |
| Salida hover vuelve a estado previo | SI | SI - transicion 100ms | NO GENERADO | PARCIAL |
| Click estrella fija puntuacion | SI | SI - `shadow-glow` amber + texto numérico | NO GENERADO | PARCIAL |
| Teclado en StarRating (Tab, flechas, Enter) | SI - Accesibilidad | SI - `role="radiogroup"`, flechas izq/der, Enter/Space | NO GENERADO | PARCIAL |
| Contador caracteres en tiempo real | SI - FA-06 | SI - cambio a ambar >900, rojo >1000 | NO GENERADO | PARCIAL |
| Boton submit disabled sin puntuacion | SI | SI | NO GENERADO | PARCIAL |
| Boton submit disabled si comentario > 1000 | SI - FA-06 | SI | NO GENERADO | PARCIAL |
| Click pagina en seccion perfil | SI - AC-CS06-8 | SI - fetch nueva pagina + scroll suave | NO GENERADO | PARCIAL |
| Hover ValoracionListItem | SI | SI - borde `#334155` -> `#475569` en 150ms | NO GENERADO | PARCIAL |

### 6.4 Accesibilidad

| Requisito | Definido en ui-ux.md | Estado |
|-----------|----------------------|--------|
| StarRating como `role="radiogroup"` con `aria-label` | SI | CUBIERTO en ui-ux.md |
| Cada estrella `aria-label="{n} estrella(s)"` + `aria-pressed` | SI | CUBIERTO en ui-ux.md |
| Navegacion teclado con flechas izq/der | SI | CUBIERTO en ui-ux.md |
| StarDisplay con `role="img"` + `aria-label` + estrellas `aria-hidden` | SI | CUBIERTO en ui-ux.md |
| Labels asociados a inputs con `htmlFor` | SI | CUBIERTO en ui-ux.md |
| Mensajes de error con `role="alert"` + `aria-live="polite"` | SI | CUBIERTO en ui-ux.md |
| Campo con error `aria-invalid="true"` + `aria-describedby` | SI | CUBIERTO en ui-ux.md |
| `aria-busy="true"` durante submitting | SI | CUBIERTO en ui-ux.md |
| Toast con `role="status"` | SI | CUBIERTO en ui-ux.md |
| Controles paginacion con `aria-label` | SI | CUBIERTO en ui-ux.md |
| Contraste texto principal > 10:1 | SI - `#ffffff` sobre `#0f1729` | CUBIERTO en ui-ux.md |
| Contraste texto secundario > 4.5:1 | SI - `#94a3b8` sobre `#0f1729` | CUBIERTO en ui-ux.md |

**Resultado:** La especificacion de accesibilidad en `ui-ux.md` es completa y cubre todos los requisitos WCAG AA relevantes. No se identificaron gaps de accesibilidad en la especificacion.

---

## 7. Validacion de Tests

### 7.1 Tests Requeridos por Criterios de Aceptacion

| Criterio | Test Requerido | Tipo | Planificado en test-strategy.md | Estado |
|----------|----------------|------|---------------------------------|--------|
| AC-CS06-1 | No renderizar formulario si acuerdo != Completado | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-2 | Mostrar read-only si ya valoro (respuesta 400 con 4014) | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-3 | StarRating: hover ilumina estrellas 1..N | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-3 | StarRating: click fija valor | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-3 | StarRating: no permite valor 0 o > 5 | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-4 | ValoracionForm muestra mensaje incentivo | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-4 | Envio sin comentario es valido (schema) | Unit | NO GENERADO | NO CUBIERTO |
| AC-CS06-5 | Toast muestra texto exacto tras envio exitoso | Integration | NO GENERADO | NO CUBIERTO |
| AC-CS06-5 | Card cambia a read-only tras envio sin reload | Integration | NO GENERADO | NO CUBIERTO |
| AC-CS06-8 | Paginacion: cambio de pagina carga nuevos items | Integration | NO GENERADO | NO CUBIERTO |
| AC-CS06-9 | Empty state con texto exacto cuando totalValoraciones === 0 | Unit | NO GENERADO | NO CUBIERTO |
| FA-01 | CTA oculto si acuerdo no completado | Unit | NO GENERADO | NO CUBIERTO |
| FA-05 | Schema rechaza puntuacion < 1 y > 5 | Unit (schema) | NO GENERADO | NO CUBIERTO |
| FA-06 | Contador muestra rojo y bloquea submit > 1000 chars | Unit | NO GENERADO | NO CUBIERTO |
| FA-06 | Schema rechaza comentario > 1000 chars | Unit (schema) | NO GENERADO | NO CUBIERTO |

### 7.2 Tests Faltantes por Tipo

| Tipo de Test | Razon de Necesidad | Prioridad |
|-------------|-------------------|-----------|
| Unit - `StarRating.test.tsx` | Componente critico y custom, hover + click + teclado | Alta |
| Unit - `ValoracionForm.test.tsx` | Submit, validaciones, disabled states | Alta |
| Unit - `createValoracionSchema.test.ts` | Validaciones de puntuacion y comentario | Alta |
| Unit - `ValoracionesSection.test.tsx` | Empty state, loading, error, paginacion | Alta |
| Integration - `useCreateValoracion.test.ts` | Mutation, toast, invalidacion cache, read-only | Alta |
| Integration - `useValoracionesByUser.test.ts` | Query con paginacion, estados | Media |
| E2E - Flujo completo valoracion | Completar acuerdo -> valorar -> verificar toast y read-only | Alta |
| E2E - Ver perfil con valoraciones | Ver seccion valoraciones con histograma y lista | Media |
| E2E - Empty state en perfil sin valoraciones | Verificar mensaje exacto | Media |

---

## 8. Recomendaciones

### Acciones Requeridas (Critico)

1. **Generar `frontend-plan.md` para Landing**
   - Archivo: `plans/cs-valoraciones/frontend-landing/frontend-plan.md`
   - Cambio: Crear plan completo con todos los componentes, hooks y servicios detallados en `feature-spec.md` seccion "Frontend (Landing)" y `ui-ux.md` seccion "Componentes Personalizados Necesarios". Incluir la decision de implementacion de `StarRating` custom (Tailwind + Lucide) vs libreria externa. Definir la solucion para el `userIdValorado` en la invalidacion de cache.

2. **Generar `test-strategy.md` para Landing**
   - Archivo: `plans/cs-valoraciones/frontend-landing/test-strategy.md`
   - Cambio: Crear estrategia de tests con todos los tests unitarios, de integracion y E2E listados en la seccion 7 de este informe. Incluir los escenarios de AC-CS06-5 (texto exacto del toast) como test de criterio critico.

3. **Implementar modificaciones pending en shared**
   - Archivos: `src/shared/constants/index.ts`, `src/shared/utils/error-messages.ts`, `src/shared/types/crowdsourcing.ts`, `src/shared/schemas/crowdsourcing.schema.ts`
   - Cambio: Aplicar todas las modificaciones definidas en `contracts-plan.md`. Actualmente ninguna ha sido implementada: `QUERY_KEYS.crowdsourcing.valoraciones`, `API_ROUTES.crowdsourcing.valoraciones`, interfaces de valoraciones, schemas Zod, y bloque `VALORACION_ERROR_MESSAGES` son todos pendientes.

### Acciones Sugeridas (Mayor)

4. **Definir decision sobre `ValoracionAcuerdoState`**
   - Archivo: `frontend-plan.md` (cuando se genere)
   - Cambio: Decidir si `AcuerdoCrowdsourcingDetailDto` incluye `miValoracion: ValoracionCreatedResult | null` (requiere cambio en backend response) o si el estado "ya valorado" se gestiona localmente en el frontend con `useState` tras el POST exitoso. Documentar la decision y sus implicaciones.

5. **Resolver ambiguedad de integracion de RatingBadge**
   - Archivo: `frontend-plan.md` (cuando se genere)
   - Cambio: Especificar en qué componentes existentes se integrara `RatingBadge` y si requiere una llamada adicional a `GET /api/crowdsourcing/usuarios/{userId}/valoraciones` por cada card o si el dato se incluye en los DTOs de propuesta/profesional. Para MVP, la opcion de llamada adicional es aceptable dado el volumen bajo.

6. **Agregar `ESTADO_ACUERDO_NOMBRES` o usar ID numerico explicitamente**
   - Archivo: `src/shared/constants/index.ts`
   - Cambio: El plan shared referencia `estadoAcuerdoNombre == 'Completado'` en algunos comentarios de `contracts-plan.md`, pero `ESTADO_ACUERDO.COMPLETADO` ya existe con valor numerico `2`. En `frontend-plan.md`, especificar que la condicion de visibilidad del formulario usa `acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO` (numerico), no comparacion por nombre string.

### Nice to Have (Menor)

7. **Unificar tipo `ValoracionDto` de ui-ux.md con shared**
   - En `ui-ux.md` se referencian tipos locales de documentacion. Agregar una nota al inicio de la seccion "Tipos TypeScript de Referencia" en `ui-ux.md` indicando que son solo de referencia para el documento y que la fuente de verdad es `src/shared/types/crowdsourcing.ts`.

8. **Documentar decision de `PaginatedResult<T>` como tipo global**
   - El contracts-plan sugiere mover `PaginatedResult<T>` a una seccion de tipos genericos si se reutiliza. Si `cs-mensajeria` o futuras features lo usan, refactorizar a `src/shared/types/common.ts` con una tarea de deuda tecnica.

9. **Agregar `VALORACION_PUNTUACION_MAX` y `VALORACION_COMENTARIO_MAX_LENGTH` a constants**
   - Agregar en `src/shared/constants/index.ts` dentro de la seccion `VALIDATION`:
   ```typescript
   VALORACION_PUNTUACION_MIN: 1,
   VALORACION_PUNTUACION_MAX: 5,
   VALORACION_COMENTARIO_MAX: 1000,
   ```
   - Esto permite que el componente `ValoracionForm` importe los limites desde constants en lugar de tenerlos hardcodeados.

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [x] Criterios de aceptacion AC-CS06-1 a AC-CS06-9 identificados
- [x] Flujos alternativos FA-01 a FA-06 identificados
- [ ] Todos los criterios tienen cobertura de implementacion en plans -> PENDIENTE (falta frontend-plan.md)
- [ ] Flujos de usuario completos planificados -> PENDIENTE
- [x] Casos de error cubiertos en shared (schemas, error messages, constants)

### Contratos Shared
- [x] DTOs TypeScript alineados con DTOs C#
- [x] Schemas Zod alineados con FluentValidation
- [x] Constants de error alineadas con ServiceResponseMessageType
- [ ] Modificaciones aplicadas en archivos shared -> PENDIENTE de implementacion
- [ ] `QUERY_KEYS.crowdsourcing.valoraciones` agregado -> PENDIENTE
- [ ] `API_ROUTES.crowdsourcing.valoraciones` agregado -> PENDIENTE

### UI/UX
- [x] Todas las screens planificadas en ui-ux.md (3 pantallas)
- [x] Estados de interaccion definidos en ui-ux.md
- [x] Responsive design documentado (mobile/tablet/desktop)
- [x] Accesibilidad WCAG AA especificada completamente
- [x] Design tokens definidos (colores, tipografia, espaciado, bordes)
- [x] Animaciones especificadas con duraciones exactas
- [ ] Implementacion de componentes planificada -> PENDIENTE (falta frontend-plan.md)

### Testing
- [ ] Tests para criterios criticos planificados -> PENDIENTE (falta test-strategy.md)
- [ ] Tests de integracion para flujos planificados -> PENDIENTE
- [ ] Cobertura objetivo definida -> PENDIENTE
- [ ] E2E tests para flujos principales planificados -> PENDIENTE

---

## 10. Conclusion

### Score Final: 63.6%

**Distribucion del score:**
- Contratos Shared (types, schemas, constants, error messages): 95% cubiertos en el plan, 0% implementados todavia en codigo
- UI/UX: 95% especificado en ui-ux.md
- Frontend Plan (componentes, hooks, servicios): 0% (plan no generado)
- Test Strategy: 0% (plan no generado)

**Veredicto:** REQUIERE CAMBIOS

**Analisis:** El score de 63.6% refleja que los **fundamentos del contrato** (tipos, schemas, errores, rutas de API) estan completamente especificados y alineados entre backend y frontend, y que la **especificacion UI/UX** es exhaustiva y de alta calidad. El score no alcanza el umbral de aprobacion (90%) porque los **planes de implementacion de Landing** (`frontend-plan.md` y `test-strategy.md`) no han sido generados todavia. Esta es una limitacion de planificacion, no de la feature en si.

**Proximo Paso:** Generar los planes faltantes para resolver los gaps criticos (GAP-01 y GAP-02) y luego re-ejecutar esta validacion.

---

### Orden de Resolucion Recomendado

1. Implementar modificaciones pending en shared (contracts-plan.md ya define exactamente que cambiar)
2. Generar `frontend-plan.md` para Landing (componentes, hooks, service, integracion en paginas existentes)
3. Generar `test-strategy.md` para Landing
4. Re-ejecutar validacion QA esperando score >= 90%

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-21
