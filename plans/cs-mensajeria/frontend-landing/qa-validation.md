# Validacion QA: cs-mensajeria (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)
**Target:** src/web (Landing publica - Vite + React)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Criterios de Aceptacion | 12 |
| Cubiertos | 8 |
| Parcialmente Cubiertos | 4 |
| No Cubiertos | 0 |
| Total Flujos Alternativos | 7 |
| FA Cubiertos | 5 |
| FA Parcialmente Cubiertos | 2 |
| FA No Cubiertos | 0 |
| **Score de Cobertura (AC)** | **83%** |
| **Score de Cobertura (FA)** | **86%** |
| **Score Global** | **84%** |

**Estado: REQUIERE CAMBIOS**

Los planes definen con alto detalle la arquitectura de tipos (shared/contracts-plan.md) y la especificacion UI/UX (ui-ux.md). Sin embargo, el plan de implementacion frontend (`frontend-plan.md`) aun no ha sido generado, lo que impide validar la cobertura de componentes React, hooks y servicios de forma directa. La validacion se realiza usando `contracts.md` y `ui-ux.md` como proxy de los planes de frontend pendientes.

**Gaps criticos identificados:** 1 (polling en listado vs. especificacion)
**Gaps mayores identificados:** 5
**Gaps menores identificados:** 6

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Proyecto |
|----|----------|------|---------|
| AC-CS05-1 | Iniciar conversacion con asunto (max 200 chars). Conversacion creada con FechaCreacion=ahora y FechaUltimoMensaje=null | Funcional | Backend + Landing |
| AC-CS05-2 | No se permiten conversaciones duplicadas; backend valida unicidad | Funcional | Backend |
| AC-CS05-3 | Si ya existe conversacion, sistema redirige al usuario a la existente | Funcional | Backend + Landing |
| AC-CS05-4 | Solo los dos participantes pueden enviar mensajes; tercer usuario recibe 403 | Funcional | Backend |
| AC-CS05-5 | Mensajes en orden cronologico; propios a la derecha, ajenos a la izquierda | Funcional | Landing |
| AC-CS05-6 | Al abrir conversacion, PATCH automatico marcar-leidos; mensajes del remitente propio no se marcan | Funcional | Backend + Landing |
| AC-CS05-7 | Listado con badge numerico de no leidos por conversacion, ordenado por FechaUltimoMensaje DESC | Funcional | Backend + Landing |
| AC-CS05-8 | Filtro por contexto (Todas / Necesidades / Acuerdos) via query param | Funcional | Backend + Landing |
| AC-CS05-9 | Navbar con badge total de no leidos actualizado con totalNoLeidos de la respuesta | Funcional | Landing |
| AC-CS05-10 | URL adjunta valida: mensaje creado OK. URL invalida: error de validacion | Funcional | Backend |
| AC-CS05-11 | Contenido min 1, max 5000 caracteres; fuera de limites retorna error | Funcional | Backend |
| AC-CS05-12 | Listado paginado; mensajes paginados de forma inversa (mas recientes primero) | Funcional | Backend + Landing |

**NFR-01** | GET listado retorna < 300ms con COUNT de no leidos | No Funcional | Backend |
**NFR-02** | Polling MVP con refetchInterval 10s en useMensajes | No Funcional | Landing |
**NFR-03** | PageSize default listado: 20; mensajes: 50, orden cronologico inverso | No Funcional | Backend + Landing |
**NFR-04** | Validacion de participante en backend en todos los endpoints | No Funcional | Backend |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales (Criterios de Aceptacion)

| ID | Criterio (Resumen) | contracts-plan (shared) | ui-ux.md | contracts.md (proxy backend) | Estado |
|----|-------------------|-------------------------|----------|-------------------------------|--------|
| AC-CS05-1 | Iniciar conversacion con asunto max 200 chars | `createConversacionSchema` (asunto .min(1).max(200)), `CreateConversacionRequest`, `IniciarConversacionDialog` | Pantalla 4: Dialog con input asunto, contador 0/200, validacion en tiempo real. Estado Success: toast + navegar a mensajes/{id} | POST /api/crowdsourcing/conversaciones, errores 1001/1002 para asunto, 201 con conversacion creada | CUBIERTO |
| AC-CS05-2 | No permite conversaciones duplicadas | `MENSAJERIA_ERROR_MESSAGES['4015']` | Estado FA-01 en Dialog: Toast info + navegar a conversacion existente | Error 400 con errorCode 4015 al duplicar. Indice unico filtrado en EF Core | CUBIERTO (backend via contracts) |
| AC-CS05-3 | Redireccion a conversacion existente | `getMensajeriaErrorMessage`, `CONVERSACION_DUPLICADA`, nota sobre deteccion de 4015 en `useCreateConversacion` | Estado "Error - Conversacion ya existe (FA-01)": Toast info + navegar a existente | Error 4015 en POST /conversaciones | PARCIAL - El contrato indica que backend NO devuelve el conversacionId en el cuerpo del error 4015. El frontend solo puede navegar al listado, no directamente al chat existente. |
| AC-CS05-4 | Solo participantes pueden enviar (403 para terceros) | `API_ROUTES.crowdsourcing.conversaciones.mensajes` | No aplica UI directamente (es validacion backend) | POST /mensajes retorna 403 si UserId no es participante | CUBIERTO (backend via contracts) |
| AC-CS05-5 | Mensajes en orden cronologico, propios derecha / ajenos izquierda | Tipos `Mensaje.esPropio`, `MensajeDto` | Pantalla 2: `MessageBubble` con `esPropio`, burbuja propia gradiente rosa-purpura a la derecha, ajena azul a la izquierda. `DateSeparator` entre dias | GET /mensajes retorna `esPropio` calculado en backend | CUBIERTO |
| AC-CS05-6 | PATCH automatico marcar-leidos al abrir conversacion | `API_ROUTES.crowdsourcing.conversaciones.marcarLeidos`, `MarcarLeidosResponse`, `QUERY_KEYS.conversaciones.noLeidos` | Pantalla 2: Interaccion "Abrir la pagina -> PATCH automatico a /marcar-leidos al montar el componente". Badge de no leidos en navbar se actualiza | PATCH /marcar-leidos, marca mensajes donde UserIdRemitente != token y Leido=false | CUBIERTO |
| AC-CS05-7 | Listado con badge no leidos por conversacion, ordenado FechaUltimoMensaje DESC | `ConversacionListItem.mensajesNoLeidos`, `ConversacionListResponse.totalNoLeidos`, `QUERY_KEYS.conversaciones.lista` | Pantalla 1: Badge rosa por fila (mensajesNoLeidos > 0), fondo diferenciado #1e2a42, ordenado por FechaUltimoMensaje. Badge total en encabezado | GET /conversaciones ordena por FechaUltimoMensaje DESC, incluye mensajesNoLeidos y totalNoLeidos | CUBIERTO |
| AC-CS05-8 | Filtro por contexto via query param | `FiltroConversacion` union type, `FILTRO_CONVERSACION` const, `QUERY_KEYS.conversaciones.lista(filtro)` | Pantalla 1: Tabs Todas/Necesidades/Acuerdos, cambio de tab recarga lista con parametro contexto | GET /conversaciones?contexto=necesidades/acuerdos/todas | CUBIERTO |
| AC-CS05-9 | Navbar badge total no leidos con totalNoLeidos | `NoLeidosCountResponse.totalNoLeidos`, `QUERY_KEYS.conversaciones.noLeidos`, `MENSAJERIA_POLLING_INTERVAL_MS` | Pantalla 3: NavbarMensajesIcon con badge rosa, oculto si 0, "99+" si > 99, refetch 60s, invalidacion tras marcar-leidos | GET /conversaciones/no-leidos retorna totalNoLeidos | PARCIAL - La ui-ux.md indica refetchInterval de 60s para el navbar, mientras que contracts.md especifica 10s. Inconsistencia entre documentos. |
| AC-CS05-10 | URL adjunta valida OK; URL invalida retorna error | `createMensajeSchema` (urlAdjunto .url().optional().or(z.literal(''))), `MENSAJE_URL_INVALIDA` | Pantalla 2: Campo URL con validacion en tiempo real, error "Debe ser una URL valida" | POST /mensajes retorna 400 errorCode 1013 para URL invalida | CUBIERTO |
| AC-CS05-11 | Contenido min 1, max 5000 chars | `createMensajeSchema` (contenido .min(1).max(5000)), `MENSAJE_CONTENIDO_VACIO`, `MENSAJE_CONTENIDO_MAX` | Pantalla 2: Boton Enviar disabled si vacio, contador 4000+ chars, borde rojo y error si > 5000 | POST /mensajes retorna 400 errorCode 1001/1002 | CUBIERTO |
| AC-CS05-12 | Listado paginado; mensajes paginacion inversa | `QUERY_KEYS.conversaciones.mensajes(id, page)`, `MensajeListResponse`, `ConversacionListResponse` con page/pageSize | Pantalla 1: Boton "Cargar mas". Pantalla 2: Boton "Cargar mensajes anteriores" (scroll hacia arriba) | GET /conversaciones?page=N&pageSize=N. GET /mensajes?page=N (pageSize default 50) | PARCIAL - ui-ux.md describe mensajes en orden ASC con "cargar anteriores" al hacer scroll. El contracts.md especifica que backend retorna en ASC. No se especifica si el frontend invierte el array o si el scroll inverso es scroll-to-top. Falta plan explicito de implementacion del scroll inverso. |

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura en Contratos/UI | Estado |
|----|----------|--------------------------|--------|
| NFR-01 | GET listado < 300ms con COUNT SQL | contracts.md: AsNoTracking, COUNT SQL en query. Nota de implementacion de GetConversacionesQuery | CUBIERTO (backend) |
| NFR-02 | Polling 10s en useMensajes | `MENSAJERIA_POLLING_INTERVAL_MS = 10000`, contracts.md nota frontend punto 3 (`useMensajes` con polling) | CUBIERTO en shared. Falta frontend-plan.md que especifique `refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS` en el hook |
| NFR-03 | PageSize default: listado 20, mensajes 50 | contracts.md: GET /conversaciones default pageSize 20, max 50. GET /mensajes default pageSize 50 | CUBIERTO (backend via contracts) |
| NFR-04 | Validacion de participante en todos los endpoints | contracts.md: Autorizacion por endpoint documentada para los 6 endpoints | CUBIERTO (backend via contracts) |

---

## 4. Matriz de Trazabilidad - Flujos Alternativos

| ID | Condicion | contracts-plan (shared) | ui-ux.md | contracts.md | Estado |
|----|-----------|-------------------------|----------|--------------|--------|
| FA-01 | Conversacion ya existe para mismo contexto | `CONVERSACION_DUPLICADA`, deteccion de errorCode 4015 en `useCreateConversacion` | Pantalla 4: Toast info "Ya existe..." + navegar a conversacion existente | Error 400 con errorCode 4015. Nota: backend NO devuelve conversacionId en el error | PARCIAL - Redireccion al chat existente imposible directamente (falta conversacionId en error). Solo se puede ir al listado. |
| FA-02 | Usuario sin relacion con destinatario | `CONVERSACION_NO_RELACION`, codigo 4016 en error messages | Pantalla 4: Toast error "No puedes iniciar..." Dialog permanece abierto | Error 403 con errorCode 4016 (segun contracts notas, aunque el endpoint define 403/3002 generico) | PARCIAL - Inconsistencia: contracts.md tabla de errores dice 403/3002 para "No tienes relacion", pero el nuevo codigo 4016 es BusinessRule. Necesita alineacion. |
| FA-03 | Sin conversaciones activas | Empty state en UI planificado | Pantalla 1: Icono MessageSquare + "No tienes conversaciones activas" centrado | GET /conversaciones retorna items vacios, no error | CUBIERTO |
| FA-04 | URL adjunta invalida | `createMensajeSchema` valida URL, `MENSAJE_URL_INVALIDA` | Pantalla 2: Error en campo URL "Debe ser una URL valida", mensaje no se envia | POST /mensajes retorna 400/1013 | CUBIERTO |
| FA-05 | Tercero intenta enviar mensaje | No aplica al frontend directamente; gestionado por 403 del backend | UI no tiene accion especial; el 403 se maneja con toast de error generico | POST /mensajes retorna 403/3002 si no es participante | CUBIERTO |
| FA-06 | Contenido vacio o > 5000 chars | `createMensajeSchema` con .min(1).max(5000) | Pantalla 2: Boton disabled si vacio, borde rojo + error si > 5000 | POST /mensajes retorna 400/1001 o 400/1002 | CUBIERTO |
| FA-07 | Asunto vacio o > 200 chars | `createConversacionSchema` con .min(1).max(200) | Pantalla 4: Borde rojo + "El asunto es obligatorio" o "Maximo 200 caracteres" | POST /conversaciones retorna 400/1001 o 400/1002 | CUBIERTO |

---

## 5. Analisis de Gaps

### 5.1 Gaps Criticos

| ID | Criterio Afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-C01 | AC-CS05-9 / NFR-02 | **Inconsistencia en intervalo de polling**: `contracts.md` (seccion SignalR y notas frontend) especifica `refetchInterval: 10000` para `useConversaciones`. La `ui-ux.md` (seccion Interacciones de Pantalla 1) especifica "Polling automatico cada 30 segundos" para el listado, y la seccion de Pantalla 3 especifica refetchInterval de 60s para el badge. Tres valores distintos para el mismo recurso (10s, 30s, 60s). | Alto - La implementacion del frontend podria no cumplir con la especificacion de rendimiento y UX si usa el valor incorrecto | Unificar el contrato: (a) `useConversaciones` con 30s (no hay mensajeria activa en la lista), (b) `useMensajes` con 10s (chat activo), (c) `useNoLeidosCount` con 60s via `refetchInterval`. Actualizar `contracts.md` para distinguir los tres intervalos y agregar constantes separadas `MENSAJERIA_LISTA_POLLING_INTERVAL_MS = 30000` y `MENSAJERIA_NAVBAR_POLLING_INTERVAL_MS = 60000`. |

### 5.2 Gaps Mayores

| ID | Criterio Afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-M01 | AC-CS05-3 / FA-01 | **conversacionId no incluido en error 4015**: El backend retorna 400/4015 cuando la conversacion ya existe, pero el contrato no incluye el `conversacionId` de la conversacion existente en el cuerpo del error. La `ui-ux.md` promete navegar al chat existente, pero el frontend no puede obtener el ID para hacer esa navegacion directa. La nota en `contracts-plan.md` indica que "la navegacion seria a la lista de conversaciones". | Medio - La UX prometida (redireccion directa al chat) no se puede implementar con el contrato actual. El usuario llega al listado y debe hacer un click adicional. | Modificar el contrato del error 4015 para incluir `existingConversacionId: string` en el body del error 400, o cambiar la respuesta a 200 con el DTO de la conversacion existente. Actualizar `contracts.md`, `contracts-plan.md` y `ui-ux.md` de forma consistente. |
| GAP-M02 | FA-02 | **Inconsistencia en error code para "sin relacion"**: `contracts.md` tabla de errores de POST /conversaciones usa `403 / 3002` para "No tienes relacion con este destinatario". El `contracts-plan.md` y `error-messages.ts` usan `4016 / BusinessRule_NoRelacionConDestinatario`. La `feature-spec.md` solo dice "Backend retorna 403 Forbidden". Hay tres definiciones distintas del mismo error. | Medio - El frontend necesita saber el HTTP status y el errorCode exacto para distinguir "sin relacion" de "sin autenticacion". Si el backend implementa 4016 como 400 (no 403), el handler de errores del frontend falla. | Alinear el contrato: dado que "no tener relacion" es una regla de negocio (no autenticacion), deberia ser 400/4016. Actualizar `contracts.md` para cambiar de 403/3002 a 400/4016 en POST /conversaciones. Actualizar `feature-spec.md` para reflejar esto. |
| GAP-M03 | AC-CS05-12 | **Implementacion del scroll inverso no especificada**: `contracts.md` nota de GetMensajesQuery dice "Ordenar por FechaCreacion ASC". `ui-ux.md` muestra mensajes mas antiguos arriba y mas recientes abajo (correcto), con "Cargar mensajes anteriores" al hacer scroll hacia arriba. `feature-spec.md` dice "paginacion de forma inversa (mas recientes primero)". Hay contradiccion entre "ASC en backend" y "mas recientes primero" del spec. El frontend-plan.md (no generado) deberia aclarar si: (a) el backend retorna ASC y el frontend muestra page 1 al fondo del scroll, o (b) el backend retorna DESC y el frontend invierte el array. | Medio - Sin claridad, la implementacion del frontend podria ordenar mal los mensajes o tener un scroll position inesperado al cargar mensajes anteriores. | Aclarar en `contracts.md`: "GET /mensajes retorna en orden ASC (FechaCreacion). La pagina 1 contiene los mensajes mas recientes (offset desde el final). El frontend muestra la pagina 1 al fondo del scroll y carga paginas adicionales (2, 3...) que contienen mensajes mas antiguos hacia arriba." Actualizar la nota de implementacion GetMensajesQuery en `contracts.md`. |
| GAP-M04 | AC-CS05-9 | **`useNoLeidosCount` vs `totalNoLeidos` de lista**: `feature-spec.md` FS-02 y AC-CS05-9 dicen que el badge se actualiza con `totalNoLeidos` del listado de conversaciones. `ui-ux.md` Pantalla 3 dice que el badge hace fetch a GET `/api/crowdsourcing/conversaciones?pageSize=1` y extrae `totalNoLeidos`. Pero `contracts.md` define un endpoint dedicado `GET /no-leidos` para el badge. El shared plan define `useNoLeidosCount()` apuntando a `QUERY_KEYS.conversaciones.noLeidos`. Son tres mecanismos distintos para el mismo dato. | Medio - El frontend podria implementar el badge con el endpoint equivocado, generando requests innecesarios o datos inconsistentes. | Definir claramente en `contracts.md` que el navbar badge usa `GET /no-leidos` (endpoint dedicado) con polling de 60s, y que `totalNoLeidos` del listado es un valor secundario que solo se usa cuando el listado ya esta cargado. Actualizar `ui-ux.md` Pantalla 3 para eliminar la referencia a `?pageSize=1` y usar el endpoint dedicado. |
| GAP-M05 | - | **Ausencia de frontend-plan.md**: No existe el plan de implementacion de componentes React (`frontend-plan.md`) para la feature. Los componentes, hooks y servicios estan documentados en `ui-ux.md` (como especificacion UI) y en `contracts.md` (como notas de implementacion), pero sin un plan de arquitectura formal que incluya: estructura de carpetas, dependencias entre componentes, estrategia de state management, manejo de errores globales y criterios de cobertura de tests. | Medio - El desarrollador debe inferir la arquitectura desde multiples documentos. Mayor riesgo de inconsistencias en la implementacion. | Generar `frontend-plan.md` antes de iniciar la implementacion. Debe incluir: (a) estructura de carpetas en `src/web/src/features/crowdsourcing/mensajeria/`, (b) listado de componentes con su jerarquia, (c) hooks y sus dependencias, (d) servicios `conversacion.service.ts` y `mensaje.service.ts`, (e) integracion con el router existente, (f) estrategia de invalidacion de cache. |

### 5.3 Gaps Menores

| ID | Criterio Afectado | Gap | Impacto | Recomendacion |
|----|-------------------|-----|---------|---------------|
| GAP-N01 | AC-CS05-5 | **Separadores de fecha no mencionados en contracts.md**: `ui-ux.md` define el componente `DateSeparator` entre mensajes de dias distintos. El contrato no especifica si el backend devuelve agrupacion por fecha o si es responsabilidad del frontend. | Bajo - El frontend debe implementar agrupacion por fecha usando `fechaCreacion` de cada mensaje. | Agregar nota en `contracts.md` (Frontend notas): "El `DateSeparator` entre dias se calcula en frontend comparando `fechaCreacion` de mensajes consecutivos. No requiere datos adicionales del backend." |
| GAP-N02 | AC-CS05-7 | **Timestamp relativo no especificado**: `ui-ux.md` muestra "hace 5 min", "ayer", o fecha en las filas del listado, pero no especifica la libreria ni la logica de formateo (ej: date-fns, dayjs, o custom). El shared plan no incluye utilidades de formato de fecha. | Bajo - Inconsistencia visual posible si cada componente implementa su propio formateo. | Agregar en `src/shared/utils/` una funcion `formatRelativeTime(dateString: string): string` o especificar en `contracts-plan.md` que se use `date-fns/formatDistanceToNow` o equivalente ya disponible en el proyecto. |
| GAP-N03 | AC-CS05-1 | **Punto de entrada desde detalle de necesidad no planificado**: `ui-ux.md` Pantalla 5 define el boton "Iniciar conversacion" en el detalle de una necesidad y el enlace "Ver conversacion" cuando ya existe. Este es un cambio a pantallas existentes que no tiene un plan de modificacion. | Bajo - Riesgo de olvidar integrar los puntos de entrada en las pantallas existentes de necesidades y acuerdos. | Crear checklist de integracion en el `frontend-plan.md` que incluya: (a) componente `IniciarConversacionButton` en la pagina de detalle de necesidad, (b) enlace "Ver conversacion del acuerdo" en la pagina de detalle de acuerdo (con badge de no leidos). |
| GAP-N04 | AC-CS05-6 | **Race condition entre marcar-leidos y primer render de mensajes**: El PATCH marcar-leidos se invoca "al montar el componente" pero los mensajes se cargan con un GET asincronico. Si el GET termina antes que el PATCH, los mensajes se muestran como no leidos brevemente. | Bajo - Parpadeo visual de badges en el primer render. | Agregar nota en `frontend-plan.md`: "Invocar `useMarcarLeidos` en paralelo con `useMensajes` al montar ChatView. No esperar al PATCH para mostrar los mensajes (no bloquear el render). El badge se actualizara cuando el PATCH complete e invalide la cache." |
| GAP-N05 | AC-CS05-12 | **Estrategia de paginacion del listado no completamente definida**: `ui-ux.md` dice "Click 'Cargar mas': agregar resultados al final de la lista (no reemplazar)". Esto implica estado acumulativo (infinite query o state local). El shared plan no especifica si usar `useInfiniteQuery` de TanStack Query o `useState` para acumulacion manual. | Bajo - Puede generar implementacion inconsistente con el patron del resto del proyecto. | Especificar en `frontend-plan.md` si usar `useInfiniteQuery` (patron correcto para "load more") o `useQuery` con page state y acumulacion manual. Verificar si otras features del proyecto ya usan `useInfiniteQuery` como referencia. |
| GAP-N06 | AC-CS05-9 | **`ConversacionDetalleDto` no incluida en shared plan**: `ui-ux.md` define la interface `ConversacionDetalleDto` con `{ id, asunto, nombreOtraParte, imagenOtraParte, contextoTipo, contextoTitulo }`. Esta interface no esta incluida en `contracts-plan.md` ni en `contracts.md` (que no define un DTO de detalle de conversacion, solo de listado y mensaje). | Bajo - El componente `ChatHeader` necesita este tipo pero no tiene un DTO de backend correspondiente. | Evaluar si `ConversacionDetalleDto` se puede derivar del primer item de `ConversacionListResponse` (usando el id de la URL) o si requiere un nuevo endpoint `GET /conversaciones/{id}`. Si se usa el listado como fuente, agregar nota en `contracts-plan.md`. Si se necesita un endpoint de detalle, agregarlo a `contracts.md`. |

---

## 6. Validacion de Tests

> **Nota:** El archivo `test-strategy.md` no existe en los planes generados para esta feature. La validacion de tests se realiza contra los criterios de aceptacion para identificar los tests requeridos.

### Cobertura de Criterios en Tests (Tests Requeridos)

| Criterio | Test Requerido | Tipo | Estado |
|----------|----------------|------|--------|
| AC-CS05-1 | `IniciarConversacionDialog - submit valido crea conversacion y navega` | Integration | NO CUBIERTO - falta test-strategy.md |
| AC-CS05-1 | `createConversacionSchema - validacion asunto vacio` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-1 | `createConversacionSchema - validacion asunto > 200 chars` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-3 | `useCreateConversacion - error 4015 navega a listado` | Unit (Hook) | NO CUBIERTO |
| AC-CS05-5 | `MessageBubble - mensaje propio renderiza a la derecha` | Unit (Component) | NO CUBIERTO |
| AC-CS05-5 | `MessageBubble - mensaje ajeno renderiza a la izquierda` | Unit (Component) | NO CUBIERTO |
| AC-CS05-6 | `ChatView - al montar invoca useMarcarLeidos automaticamente` | Integration | NO CUBIERTO |
| AC-CS05-7 | `ConversacionRow - muestra badge si mensajesNoLeidos > 0` | Unit (Component) | NO CUBIERTO |
| AC-CS05-7 | `ConversacionRow - oculta badge si mensajesNoLeidos === 0` | Unit (Component) | NO CUBIERTO |
| AC-CS05-8 | `ConversacionList - cambio de filtro recarga lista con contexto correcto` | Integration | NO CUBIERTO |
| AC-CS05-9 | `NavbarMensajesIcon - muestra badge cuando totalNoLeidos > 0` | Unit (Component) | NO CUBIERTO |
| AC-CS05-9 | `NavbarMensajesIcon - muestra "99+" cuando totalNoLeidos > 99` | Unit (Component) | NO CUBIERTO |
| AC-CS05-9 | `NavbarMensajesIcon - oculta badge cuando totalNoLeidos === 0` | Unit (Component) | NO CUBIERTO |
| AC-CS05-10 | `createMensajeSchema - URL adjunta invalida retorna error` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-10 | `createMensajeSchema - URL adjunta valida pasa validacion` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-10 | `createMensajeSchema - sin URL adjunta pasa validacion` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-11 | `createMensajeSchema - contenido vacio retorna error` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-11 | `createMensajeSchema - contenido > 5000 chars retorna error` | Unit (Zod) | NO CUBIERTO |
| AC-CS05-12 | `ConversacionList - click "Cargar mas" agrega resultados sin reemplazar` | Integration | NO CUBIERTO |
| FA-01 | `IniciarConversacionDialog - error 4015 muestra toast y navega` | Integration | NO CUBIERTO |
| FA-03 | `ConversacionList - empty state cuando items es vacio` | Unit (Component) | NO CUBIERTO |
| FA-04 | `MessageInput - URL invalida muestra error y bloquea envio` | Unit (Component) | NO CUBIERTO |
| FA-07 | `IniciarConversacionDialog - asunto vacio bloquea submit` | Unit (Component) | NO CUBIERTO |

### Tests de Schemas Zod (Prioritarios - pueden hacerse sin frontend-plan.md)

Los schemas Zod en `src/shared/schemas/crowdsourcing.schema.ts` son independientes del frontend y pueden testearse inmediatamente:

| Schema | Test | Razon de Prioridad |
|--------|------|--------------------|
| `createConversacionSchema` | 6 casos (vacio, max 200, UUID invalido, destinatario requerido) | Bloquea AC-CS05-1 y FA-07 |
| `createMensajeSchema` | 6 casos (vacio, max 5000, URL valida, URL invalida, sin URL) | Bloquea AC-CS05-10, AC-CS05-11 y FA-04/FA-06 |

---

## 7. Validacion de UI/UX

### Pantallas Requeridas vs Planificadas

| Pantalla Requerida | Planificada en ui-ux.md | Componentes Principales | Estado |
|-------------------|------------------------|------------------------|--------|
| Listado de conversaciones (`/crowdsourcing/mensajes`) | Si (Pantalla 1) | `ConversacionList`, `ConversacionRow`, `FiltroConversacion` | CUBIERTO |
| Vista de chat (`/crowdsourcing/mensajes/:id`) | Si (Pantalla 2) | `ChatView`, `ChatHeader`, `MessageBubble`, `DateSeparator`, `MessageInput` | CUBIERTO |
| Badge en Navbar (componente global) | Si (Pantalla 3) | `NavbarMensajesIcon` | CUBIERTO |
| Dialog iniciar conversacion | Si (Pantalla 4) | `IniciarConversacionDialog` | CUBIERTO |
| Puntos de entrada en necesidades/acuerdos | Si (Pantalla 5) | `IniciarConversacionButton` | CUBIERTO (especificacion) |
| Pagina de detalle de conversacion (header con info) | Parcial (incluida en Pantalla 2) | `ChatHeader` con `ConversacionDetalleDto` | PARCIAL - Ver GAP-N06 |

### Estados de UI

| Estado | Requerido | Planificado en ui-ux.md | Status |
|--------|-----------|------------------------|--------|
| Loading listado | Si | Skeleton 5 filas `animate-pulse` | CUBIERTO |
| Error listado | Si | Alert con borde rojo + boton Reintentar | CUBIERTO |
| Empty listado | Si | Icono MessageSquare + texto "No tienes conversaciones activas" | CUBIERTO |
| Default listado | Si | Lista ordenada, badges, filtros | CUBIERTO |
| Loading mas (listado) | Si | Spinner `Loader2` centrado | CUBIERTO |
| Loading chat inicial | Si | Skeleton cabecera + 5 burbujas alternadas | CUBIERTO |
| Error carga chat | Si | Alert + Reintentar | CUBIERTO |
| Sin mensajes en chat | Si | Texto centrado "Aun no hay mensajes" | CUBIERTO |
| Enviando mensaje | Si | Mensaje optimista + spinner + input disabled | CUBIERTO |
| Error al enviar | Si | Toast error + icono AlertCircle en mensaje | CUBIERTO |
| Polling activo | Si | Sin indicador visual, animate-in para nuevos mensajes | CUBIERTO |
| Dialog submitting | Si | Spinner en boton + inputs disabled | CUBIERTO |
| Error conversacion ya existe | Si | Toast info + navegar (parcialmente - ver GAP-M01) | PARCIAL |
| Error sin relacion | Si | Toast error + dialog permanece | PARCIAL (ver GAP-M02) |
| Badge navbar con > 99 | Si | "99+" | CUBIERTO |
| Badge navbar oculto | Si | `hidden` cuando totalNoLeidos === 0 | CUBIERTO |

### Accesibilidad

| Requisito WCAG | Especificado | Implementacion en ui-ux.md | Estado |
|----------------|-------------|---------------------------|--------|
| Contraste 4.5:1 minimo | Si | Texto principal #ffffff sobre #0f1729 (>10:1). Texto secundario #94a3b8 sobre #0f1729 (>4.5:1) | CUBIERTO |
| Focus ring visible | Si | `focus-visible:ring-2 focus-visible:ring-[#a855f7]` en todos los elementos interactivos | CUBIERTO |
| Labels en formularios | Si | `<Label>` con `htmlFor` en todos los inputs | CUBIERTO |
| ARIA badge no leidos | Si | `aria-label="{n} mensajes no leidos"` en badges | CUBIERTO |
| ARIA live region mensajes | Si | `aria-live="polite"` en area de mensajes | CUBIERTO |
| role="alert" en errores | Si | Mensajes de error y toasts con `role="alert"` | CUBIERTO |
| Boton enviar aria-label | Si | `aria-label="Enviar mensaje"` cuando solo icono | CUBIERTO |
| Navegacion por teclado | Si | Tab, Enter para enviar, Shift+Enter para salto de linea | CUBIERTO |
| Alt text en avatares | Si | `alt="{nombre de la persona}"` en AvatarImage | CUBIERTO |
| aria-busy en carga | Si | `aria-busy="true"` en area de mensajes durante carga | CUBIERTO |

### Responsive Design

| Breakpoint | Especificado | Cambios Definidos | Estado |
|------------|-------------|-------------------|--------|
| Mobile (< 768px) | Si | Padding reducido, chat full-width, cabecera condensada, `h-[calc(100vh-200px)]` | CUBIERTO |
| Tablet (768px-1024px) | Si | `max-w-2xl`, `h-[calc(100vh-280px)]` | CUBIERTO |
| Desktop (> 1024px) | Si | `max-w-3xl`, `h-[calc(100vh-320px)]`, MVP una sola vista | CUBIERTO |

---

## 8. Recomendaciones

### Acciones Requeridas (Critico - bloquea APROBADO)

1. **Unificar intervalos de polling en los tres documentos**
   - Archivos: `docs/user-stories/cs-mensajeria/contracts.md`, `docs/user-stories/cs-mensajeria/ui-ux.md`, `plans/cs-mensajeria/shared/contracts-plan.md`
   - Cambio: Definir tres constantes separadas: `MENSAJERIA_LISTA_POLLING_INTERVAL_MS = 30000` (listado), `MENSAJERIA_CHAT_POLLING_INTERVAL_MS = 10000` (chat activo), `MENSAJERIA_NAVBAR_POLLING_INTERVAL_MS = 60000` (badge). Actualizar `MENSAJERIA_POLLING_INTERVAL_MS` actual o eliminarla. Actualizar la seccion de interacciones de Pantalla 1 en ui-ux.md para decir "30 segundos" consistente con la UX.

### Acciones Sugeridas (Mayor - resolver antes de implementar frontend)

2. **Incluir conversacionId en el error 4015**
   - Archivo: `docs/user-stories/cs-mensajeria/contracts.md`
   - Cambio: Modificar el cuerpo del error 400/4015 para incluir `{ "errorCode": "4015", "conversacionId": "uuid" }` en `messages[0]` o en un campo `data`. Ejemplo:
     ```json
     {
       "data": { "conversacionId": "f2a3b4c5-..." },
       "messages": [{ "message": "Ya existe una conversacion para este contexto", "errorCode": "4015" }]
     }
     ```
   - Actualizar `contracts-plan.md` nota sobre manejo del error 4015 para leer el `conversacionId` del cuerpo del error y navegar directamente al chat.

3. **Alinear codigo de error "sin relacion" a 400/4016**
   - Archivo: `docs/user-stories/cs-mensajeria/contracts.md` tabla de errores de POST /conversaciones
   - Cambio: Cambiar la fila `403 / 3002 / "No tienes relacion con este destinatario"` por `400 / 4016 / "No tienes relacion con este destinatario"`. Actualizar `feature-spec.md` FA-02 para decir "400 Bad Request" en lugar de "403 Forbidden".

4. **Aclarar el mecanismo de paginacion inversa de mensajes**
   - Archivo: `docs/user-stories/cs-mensajeria/contracts.md` (nota de implementacion GetMensajesQuery)
   - Cambio: Agregar aclaracion: "La pagina 1 contiene los N mensajes mas recientes (implementar con `Skip((totalCount - pageSize * page))` o equivalent). El frontend recibe mensajes en ASC y los muestra con el mas reciente al fondo del scroll. Al solicitar pagina 2, el frontend prepend los mensajes mas antiguos al inicio de la lista y mantiene la posicion de scroll."

5. **Generar frontend-plan.md**
   - Archivo: `plans/cs-mensajeria/frontend-landing/frontend-plan.md` (nuevo)
   - Cambio: Crear el plan con: estructura de carpetas (`src/web/src/features/crowdsourcing/mensajeria/`), jerarquia de componentes, hooks (`useConversaciones`, `useMensajes`, `useNoLeidosCount`, `useCreateConversacion`, `useEnviarMensaje`, `useMarcarLeidos`) con sus `refetchInterval` correctos, servicios (`conversacion.service.ts`, `mensaje.service.ts`), integracion con el router y estrategia de invalidacion de cache.

6. **Resolver ConversacionDetalleDto**
   - Archivo: `docs/user-stories/cs-mensajeria/contracts.md`
   - Cambio: Definir si el `ChatHeader` obtiene los datos de la conversacion de: (a) la cache del listado de conversaciones (usando el id de la URL para encontrar el item), o (b) un nuevo endpoint `GET /api/crowdsourcing/conversaciones/{id}` que retorne `ConversacionDetalleDto`. Opcion (a) es preferible para el MVP (no requiere endpoint adicional).

### Nice to Have (Menor)

7. **Agregar utilidad `formatRelativeTime` en shared**
   - Archivo: `src/shared/utils/` (nuevo archivo o modificar `date-utils.ts`)
   - Cambio: Funcion `formatRelativeTime(dateString: string): string` usando `date-fns` para formatear timestamps del listado ("hace 5 min", "ayer", fecha larga).

8. **Agregar nota sobre `DateSeparator` en contracts.md**
   - Archivo: `docs/user-stories/cs-mensajeria/contracts.md` seccion Frontend
   - Cambio: "El componente `DateSeparator` agrupa mensajes por dia usando `fechaCreacion` de cada `MensajeDto`. No requiere datos adicionales del backend."

9. **Crear checklist de integracion en pantallas existentes**
   - Archivo: `plans/cs-mensajeria/frontend-landing/frontend-plan.md`
   - Cambio: Agregar seccion "Modificaciones a pantallas existentes" con: (a) agregar `IniciarConversacionButton` en `NecesidadDetailPage`, (b) agregar enlace "Ver conversacion del acuerdo" en `AcuerdoDetailPage`.

10. **Documentar estrategia de paginacion del listado (infinite vs manual)**
    - Archivo: `plans/cs-mensajeria/frontend-landing/frontend-plan.md`
    - Cambio: Especificar uso de `useInfiniteQuery` de TanStack Query para el listado de conversaciones (patron "load more") y para la carga de mensajes anteriores.

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [x] AC-CS05-1: Formulario de inicio de conversacion con validacion de asunto (shared + ui-ux)
- [x] AC-CS05-2: Unicidad de conversacion (backend via contracts.md)
- [ ] AC-CS05-3: Redireccion directa al chat existente (GAP-M01: conversacionId no en error 4015)
- [x] AC-CS05-4: 403 para terceros (backend via contracts.md)
- [x] AC-CS05-5: Layout de chat con esPropio (shared types + ui-ux)
- [x] AC-CS05-6: PATCH automatico marcar-leidos (shared constants + ui-ux interacciones)
- [x] AC-CS05-7: Badge no leidos y orden FechaUltimoMensaje (shared types + ui-ux)
- [x] AC-CS05-8: Filtro por contexto via query param (shared constants + ui-ux tabs)
- [ ] AC-CS05-9: Badge navbar con intervalo de polling correcto (GAP-C01: inconsistencia 10s/30s/60s)
- [x] AC-CS05-10: Validacion URL adjunta (shared schema + ui-ux)
- [x] AC-CS05-11: Validacion contenido min/max (shared schema + ui-ux)
- [ ] AC-CS05-12: Paginacion inversa claramente implementada (GAP-M03: mecanismo no definido)

### UI/UX
- [x] Todas las 5 pantallas/componentes principales planificados en ui-ux.md
- [x] Estados de interaccion definidos (loading, error, empty, success, optimistic)
- [x] Responsive design con 3 breakpoints especificados
- [x] Accesibilidad WCAG AA documentada (contraste, focus ring, ARIA, teclado)
- [x] Design tokens definidos (colores, tipografia, espaciado, bordes para mensajeria)
- [x] Animaciones y transiciones especificadas
- [ ] ConversacionDetalleDto sin endpoint backend definido (GAP-N06)
- [ ] Puntos de entrada en pantallas existentes sin plan de modificacion (GAP-N03)

### Testing
- [ ] test-strategy.md no generado
- [ ] Ningun test unitario planificado para schemas Zod (pueden generarse inmediatamente)
- [ ] Ningun test de componentes planificado
- [ ] Ningun test de integracion planificado para flujos criticos
- [ ] Cobertura objetivo no definida para esta feature

### Contratos Shared
- [x] 10 interfaces TypeScript definidas en contracts-plan.md
- [x] 2 union types definidos (`ContextoConversacion`, `FiltroConversacion`)
- [x] 2 schemas Zod con reglas alineadas al backend
- [x] QUERY_KEYS, API_ROUTES y APP_ROUTES definidos
- [x] Constantes de mensajeria (`MENSAJERIA_POLLING_INTERVAL_MS`, `FILTRO_CONVERSACION`)
- [x] Error messages con codigos numericos y claves semanticas
- [ ] Constantes de polling inconsistentes entre documentos (GAP-C01)
- [ ] `ConversacionDetalleDto` ausente del shared plan (GAP-N06)

---

## 10. Conclusion

**Score Final: 84%** (calculado sobre 12 AC + 7 FA = 19 criterios. Cubiertos: 13, Parciales: 6 = 13 + 6*0.5 = 16 / 19 = 84%)

**Veredicto: REQUIERE CAMBIOS**

Los planes disponibles (`contracts.md`, `contracts-plan.md` y `ui-ux.md`) cubren con alto nivel de detalle los aspectos de UI/UX y la capa de tipos compartidos. La especificacion de contratos es exhaustiva (6 endpoints documentados, 9 DTOs C#, 10 types TypeScript, 2 schemas Zod, constantes y error messages).

Sin embargo, para proceder a la implementacion frontend se requiere:

**Bloqueante (resolver antes de implementar):**
1. Resolver la inconsistencia de intervalos de polling (GAP-C01) entre los tres documentos.
2. Generar el `frontend-plan.md` con la arquitectura de componentes y hooks (GAP-M05).
3. Alinear el error code de "sin relacion" (FA-02) entre `contracts.md` y `contracts-plan.md` (GAP-M02).

**Recomendado (mejora significativa de UX):**
4. Incluir `conversacionId` en el error 4015 para habilitar redireccion directa al chat existente (GAP-M01).
5. Aclarar el mecanismo de paginacion inversa en el contrato del backend (GAP-M03).
6. Resolver la fuente de datos para `ConversacionDetalleDto` del `ChatHeader` (GAP-N06).

**Proximo Paso:** Resolver gaps criticos y mayores (especialmente GAP-C01 y GAP-M05), luego generar `frontend-plan.md` y `test-strategy.md`. Una vez resueltos, el plan puede alcanzar un score >= 90% (APROBADO).

---

## Apendice: Resumen de Gaps

| ID | Severidad | Criterio | Descripcion Corta |
|----|-----------|----------|-------------------|
| GAP-C01 | Critico | AC-CS05-9 | Inconsistencia de intervalos de polling (10s / 30s / 60s) entre documentos |
| GAP-M01 | Mayor | AC-CS05-3 | conversacionId no incluido en error 4015; redireccion directa imposible |
| GAP-M02 | Mayor | FA-02 | Inconsistencia en error code "sin relacion": 403/3002 vs 400/4016 |
| GAP-M03 | Mayor | AC-CS05-12 | Mecanismo de paginacion inversa de mensajes sin definicion clara |
| GAP-M04 | Mayor | AC-CS05-9 | Tres mecanismos distintos para el badge del navbar |
| GAP-M05 | Mayor | Todos | Ausencia de frontend-plan.md con arquitectura de componentes |
| GAP-N01 | Menor | AC-CS05-5 | DateSeparator sin nota en contracts sobre logica de calculo |
| GAP-N02 | Menor | AC-CS05-7 | Formato de timestamp relativo sin libreria ni funcion especificada |
| GAP-N03 | Menor | AC-CS05-1 | Puntos de entrada en pantallas existentes sin plan de modificacion |
| GAP-N04 | Menor | AC-CS05-6 | Race condition potencial entre marcar-leidos y primer render |
| GAP-N05 | Menor | AC-CS05-12 | Estrategia de paginacion (infinite query vs acumulacion manual) sin definir |
| GAP-N06 | Menor | - | ConversacionDetalleDto sin DTO backend ni endpoint definido |

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-18
