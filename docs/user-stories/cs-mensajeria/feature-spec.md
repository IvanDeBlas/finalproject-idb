# Feature: Mensajeria entre Partes

> **ID:** cs-mensajeria
> **User Story:** US-CS-05
> **Status:** proposed
> **Priority:** Media
> **Sprint:** TBD

---

## Descripcion

Esta feature introduce el canal de comunicacion directa entre artistas y profesionales dentro del modulo Crowdsourcing. Permite que ambas partes intercambien mensajes de texto con adjuntos opcionales (URL) en el contexto de una necesidad o de un acuerdo activo, sin salir de la plataforma y sin perder el hilo de la conversacion.

El modelo de mensajeria esta basado en conversaciones contextualizadas: cada `ConversacionCrowdsourcing` esta vinculada o bien a una `NecesidadCrowdsourcing` o bien a un `AcuerdoCrowdsourcing`, lo que garantiza que los mensajes siempre tienen un contexto de negocio claro. Las conversaciones sobre acuerdos se crean automaticamente cuando el artista acepta una propuesta (operacion definida en US-CS-04); las conversaciones sobre necesidades se inician manualmente por cualquiera de las dos partes que comparten una relacion (propuesta enviada). No se permiten conversaciones duplicadas para el mismo contexto entre las mismas partes.

El MVP no incluye notificaciones en tiempo real (WebSocket). La actualizacion de mensajes se realiza mediante polling cada 10 segundos o con refresh manual del chat. El seguimiento de mensajes no leidos se expone mediante badges en el listado de conversaciones y en el icono de mensajeria del navbar, de forma que el usuario siempre sabe cuantos mensajes tiene pendientes de leer. Al abrir una conversacion, todos los mensajes no leidos del interlocutor se marcan automaticamente como leidos.

---

## User Story

**Como** artista o profesional participante de una necesidad o acuerdo,
**Quiero** comunicarme directamente con la otra parte mediante conversaciones con mensajes de texto y adjuntos,
**Para** aclarar dudas, negociar condiciones, coordinar el trabajo y no perder mensajes importantes.

---

## Flujo Principal

### 1. Iniciar Conversacion sobre una Necesidad

1. Artista o profesional navega al detalle de una necesidad o propuesta donde tiene relacion con la otra parte.
2. El usuario hace click en "Iniciar conversacion".
3. El sistema verifica que existe una relacion entre los usuarios (propuesta enviada o acuerdo activo).
4. El sistema verifica que no existe ya una conversacion para ese contexto entre esas partes.
5. El sistema muestra formulario con campo "Asunto" (max 200 caracteres, obligatorio).
6. El usuario completa el asunto y confirma.
7. El sistema crea `ConversacionCrowdsourcing` con `FechaCreacion = ahora` y `FechaUltimoMensaje = null`.
8. El sistema redirige al usuario a la vista de chat de la conversacion recien creada.

### 2. Abrir Conversacion desde el Listado

1. Usuario accede a `/crowdsourcing/mensajes`.
2. El sistema muestra el listado de conversaciones en las que participa, ordenadas por `FechaUltimoMensaje` descendente.
3. Cada fila del listado muestra: nombre de la otra parte, asunto, contexto (titulo de la necesidad o acuerdo), preview del ultimo mensaje (max 80 chars), fecha del ultimo mensaje, y badge numerico de mensajes no leidos.
4. El usuario puede filtrar por contexto: Todas, Sobre Necesidades, Sobre Acuerdos.
5. El usuario hace click en una conversacion.
6. El sistema abre la vista de chat con los mensajes en orden cronologico (mas antiguos arriba, mas recientes abajo).
7. El sistema ejecuta automaticamente `PATCH /conversaciones/{id}/marcar-leidos`, marcando como leidos todos los mensajes del interlocutor.

### 3. Enviar Mensaje

1. Usuario esta en la vista de chat de una conversacion.
2. El usuario escribe el mensaje en el campo de texto (min 1 char, max 5000 chars).
3. Opcionalmente, el usuario puede incluir una URL adjunta (debe ser URL valida).
4. El usuario hace click en "Enviar".
5. El sistema crea `MensajeCrowdsourcing` con `Leido = false` y `FechaLeido = null`.
6. El sistema actualiza `FechaUltimoMensaje` de la conversacion con el timestamp del nuevo mensaje.
7. El mensaje aparece inmediatamente en el chat (propio a la derecha del layout).

---

## Flujos Secundarios

### FS-01: Conversacion sobre Acuerdo (Creacion Automatica)

La conversacion vinculada a un acuerdo no se crea manualmente. Es creada automaticamente como parte del `AceptarPropuestaCommand` (US-CS-04), con `Asunto` derivado del titulo del acuerdo. El artista y el profesional pueden acceder a esta conversacion desde el detalle del acuerdo (via `ConversacionLink`) o desde el listado de mensajes en `/crowdsourcing/mensajes`.

### FS-02: Badge de No Leidos en Navbar

El navbar principal muestra un icono de mensajeria con un badge que refleja el total de mensajes no leidos del usuario en todas sus conversaciones. Este badge se actualiza con cada peticion al listado de conversaciones (campo `totalNoLeidos` en la respuesta). Existe un endpoint ligero `GET /api/crowdsourcing/conversaciones/no-leidos` que retorna solo el conteo total, pensado para refrescar el badge de forma independiente al listado completo.

### FS-03: Paginacion de Mensajes (Scroll Inverso)

Los mensajes se paginan de forma inversa: la primera pagina contiene los mensajes mas recientes. Para ver mensajes mas antiguos el usuario hace scroll hacia arriba, lo que carga la pagina siguiente. El `pageSize` por defecto es 50 mensajes.

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Conversacion ya existe para el mismo contexto entre las mismas partes | El sistema no crea una nueva. Redirige al usuario a la conversacion existente. |
| FA-02 | Usuario intenta iniciar conversacion sin tener relacion con el destinatario | Accion no disponible. Backend retorna 403 Forbidden. |
| FA-03 | Usuario accede a `/crowdsourcing/mensajes` sin conversaciones activas | Se muestra empty state: "No tienes conversaciones activas". |
| FA-04 | URL adjunta no es una URL valida | Error de validacion en el formulario de envio. El mensaje no se crea. |
| FA-05 | Usuario tercero intenta enviar mensaje en una conversacion de la que no es participante | Backend retorna 403 Forbidden. |
| FA-06 | Contenido del mensaje vacio o supera 5000 caracteres | Error de validacion. El mensaje no se crea. |
| FA-07 | Asunto de conversacion vacio o supera 200 caracteres | Error de validacion. La conversacion no se crea. |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CS05-1 | Un artista puede iniciar una conversacion con un profesional que envio propuesta a su necesidad. El formulario requiere un asunto (max 200 chars). La conversacion se crea con `FechaCreacion = ahora` y `FechaUltimoMensaje = null`. | Backend + Landing |
| AC-CS05-2 | No se permiten conversaciones duplicadas entre las mismas partes para el mismo contexto (misma necesidad o mismo acuerdo). El backend valida unicidad antes de crear. | Backend |
| AC-CS05-3 | Si ya existe una conversacion para el contexto solicitado, el sistema redirige al usuario a la conversacion existente en lugar de crear una nueva. | Backend + Landing |
| AC-CS05-4 | Solo los dos participantes de una conversacion pueden enviar mensajes en ella. Un tercer usuario autenticado que llame al endpoint de envio recibe 403 Forbidden. | Backend |
| AC-CS05-5 | Los mensajes se muestran en orden cronologico con layout de chat: mensajes propios alineados a la derecha, mensajes del interlocutor alineados a la izquierda. | Landing |
| AC-CS05-6 | Al abrir una conversacion, el sistema ejecuta automaticamente la operacion de marcar como leidos todos los mensajes no leidos del interlocutor (PATCH marcar-leidos). Los mensajes del remitente propio no se marcan. | Backend + Landing |
| AC-CS05-7 | El listado de conversaciones muestra badge numerico de no leidos por conversacion y se ordena por `FechaUltimoMensaje` descendente. Las conversaciones con mensajes no leidos tienen indicador visual destacado. | Backend + Landing |
| AC-CS05-8 | El listado de conversaciones puede filtrarse por contexto: Todas, Sobre Necesidades, Sobre Acuerdos. El filtro se aplica via query param en el endpoint de listado. | Backend + Landing |
| AC-CS05-9 | El navbar muestra un icono de mensajeria con badge total de mensajes no leidos del usuario en todas sus conversaciones. El badge se actualiza con la respuesta del listado de conversaciones (`totalNoLeidos`). | Landing |
| AC-CS05-10 | Un mensaje con URL adjunta valida se crea correctamente. Un mensaje con URL adjunta invalida retorna error de validacion. Un mensaje sin URL adjunta se crea correctamente. | Backend |
| AC-CS05-11 | El contenido del mensaje debe tener minimo 1 caracter y maximo 5000 caracteres. Mensajes fuera de estos limites retornan error de validacion. | Backend |
| AC-CS05-12 | El listado de conversaciones esta paginado (query params `page` y `pageSize`). Los mensajes dentro de una conversacion estan paginados de forma inversa (mas recientes primero, mas antiguos al hacer scroll hacia arriba). | Backend |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar Commands CQRS: `CreateConversacionCommand`, `SendMensajeCommand`, `MarcarLeidosCommand`. Queries: `GetConversacionesQuery` (listado con filtro y paginacion, incluye count de no leidos), `GetMensajesQuery` (paginacion inversa), `GetNoLeidosTotalQuery` (endpoint ligero para badge del navbar). Validators: `CreateConversacionValidator` (asunto, destinatario, unicidad de contexto), `SendMensajeValidator` (contenido, URL opcional). Nuevo `IConversacionCrowdsourcingService` con logica de negocio: verificacion de relacion, unicidad de conversacion, marcado de leidos, actualizacion de `FechaUltimoMensaje`. Nuevo `IMensajeCrowdsourcingService`. Nuevas entidades `ConversacionCrowdsourcing` y `MensajeCrowdsourcing` (la tabla `ConversacionCrowdsourcing` puede estar ya creada por US-CS-04). Nuevas migraciones si las tablas no existen. Autorizacion por participante en todos los endpoints. | ALTO |
| **Shared** | Definir TypeScript types para DTOs: `ConversacionResumenDto`, `ConversacionDetalleDto`, `MensajeDto`. Tipos para requests: `CreateConversacionRequest`, `SendMensajeRequest`. Schemas Zod: `createConversacionSchema` (asunto max 200), `sendMensajeSchema` (contenido min 1 max 5000, urlAdjunto URL valida opcional). Constants: `FILTRO_CONVERSACION` ('todas', 'necesidades', 'acuerdos'). Nuevas `QUERY_KEYS` (conversaciones, mensajes, no-leidos) y `API_ROUTES` para los 5 endpoints de esta feature. | MEDIO |
| **Landing** | Implementar pagina `MensajesPage` en ruta `/crowdsourcing/mensajes`. Implementar pagina o layout `ChatPage` en ruta `/crowdsourcing/mensajes/:id`. Componentes: `ConversacionList` (listado paginado con filtro), `ConversacionItem` (fila con avatar, asunto, contexto, preview, fecha, badge no leidos), `ChatView` (vista de mensajes con scroll inverso), `MensajeItem` (burbuja con alineacion segun `esPropio`), `EnviarMensajeForm` (campo de texto + adjunto URL + boton enviar), `FiltroConversacion` (botones Todas / Necesidades / Acuerdos), `NavbarMensajesBadge` (icono + badge total no leidos). Hooks: `useConversaciones` (useQuery con filtro y paginacion), `useMensajes` (useQuery con paginacion inversa, polling cada 10 segundos), `useCreateConversacion` (useMutation), `useSendMensaje` (useMutation, invalida cache de mensajes y conversaciones), `useMarcarLeidos` (useMutation, ejecutado al montar ChatView), `useNoLeidosTotal` (useQuery, para badge del navbar). Services: `conversacion.service.ts`, `mensaje.service.ts`. Implementar polling de 10 segundos en `useMensajes` para el MVP (sin WebSocket). | ALTO |
| **Admin** | No involucrado. Toda la mensajeria ocurre en la Landing. Los profesionales y artistas acceden al chat desde la Landing. | BAJO |

---

## Entidades Involucradas

### ConversacionCrowdsourcing (nueva o ya creada por US-CS-04)

Representa el canal de comunicacion entre dos participantes en el contexto de una necesidad o un acuerdo.

**Campos clave:**
- `Id` (Guid, PK)
- `NecesidadId` (Guid, nullable, FK -> NecesidadCrowdsourcing) - Contexto necesidad
- `AcuerdoId` (Guid, nullable, FK -> AcuerdoCrowdsourcing) - Contexto acuerdo
- `UserIdCreador` (string, FK -> Identity.User) - Usuario que inicio la conversacion
- `UserIdDestinatario` (string, FK -> Identity.User) - Usuario receptor
- `Asunto` (string, NOT NULL, max 200)
- `FechaCreacion` (DateTime, NOT NULL)
- `FechaUltimoMensaje` (DateTime, nullable, se actualiza al enviar cada mensaje)

**Reglas de negocio:**
- Debe tener exactamente uno de `NecesidadId` o `AcuerdoId` (no ambos, no ninguno)
- Unicidad: no puede existir dos conversaciones con los mismos `UserIdCreador`, `UserIdDestinatario` y el mismo `NecesidadId` o `AcuerdoId`
- Si `AcuerdoId` no es null, la conversacion fue creada automaticamente por US-CS-04

### MensajeCrowdsourcing (nueva)

Representa un mensaje individual dentro de una conversacion.

**Campos clave:**
- `Id` (Guid, PK)
- `ConversacionId` (Guid, FK -> ConversacionCrowdsourcing)
- `UserIdRemitente` (string, FK -> Identity.User)
- `Contenido` (string, NOT NULL, min 1, max 5000)
- `UrlAdjunto` (string, nullable, URL valida)
- `Leido` (bool, default false)
- `FechaLeido` (DateTime, nullable, se registra al marcar como leido)
- `FechaCreacion` (DateTime, NOT NULL)

---

## Requisitos No Funcionales

### Performance
- El GET del listado de conversaciones debe retornar en menos de 300ms, incluyendo el COUNT de no leidos por conversacion.
- El conteo de no leidos por conversacion debe calcularse a nivel SQL (COUNT con WHERE), no en memoria.
- El endpoint ligero `GET /no-leidos` para el badge del navbar debe retornar en menos de 100ms.
- Evitar el problema N+1 en el listado: cargar el preview del ultimo mensaje y el count de no leidos en la misma query SQL.
- Usar `IRequestCacheService` para compartir datos entre validator y handler dentro de la misma request (evitar queries duplicados de verificacion de participante).

### Polling MVP
- El hook `useMensajes` en la Landing implementa polling con `refetchInterval: 10000` (10 segundos) de TanStack Query mientras el chat esta en primer plano.
- El hook `useConversaciones` puede usar el mismo mecanismo o depender del refresh al volver al listado.
- El badge del navbar se actualiza con cada peticion al listado de conversaciones (no requiere polling independiente en MVP).

### Paginacion
- Listado de conversaciones: `pageSize` por defecto 20.
- Mensajes de una conversacion: `pageSize` por defecto 50, orden cronologico inverso (mas recientes primero). El frontend invierte el orden para mostrar los mas antiguos arriba.

### Seguridad
- Validar en backend que el usuario autenticado es participante de la conversacion en TODOS los endpoints de mensajes y marcar-leidos.
- No exponer mensajes ni metadatos de conversaciones a usuarios que no son participantes.
- La verificacion de relacion (propuesta enviada o acuerdo activo) se hace en backend al crear una conversacion.

---

## Dependencias

### Tecnicas
- **Modulo Crowdsourcing**: `NecesidadCrowdsourcing` (FK de contexto), `AcuerdoCrowdsourcing` (FK de contexto)
- **Modulo UserAccess**: Entidad `Artista` y `PerfilProfesional` (para mostrar nombre e imagen de la otra parte en el listado)
- **Identity**: `UserIdCreador`, `UserIdDestinatario`, `UserIdRemitente` son claves foraneas a `AspNetUsers`
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType`, `IRequestCacheService`
- **Shared**: Types, schemas Zod, constants y QUERY_KEYS definidos en esta feature

### De otras User Stories
- **US-CS-04** (cs-acuerdos-entregables): Prerequisito directo. La entidad `ConversacionCrowdsourcing` puede estar ya creada por US-CS-04 (se crea automaticamente al aceptar una propuesta). La tabla debe existir en BD antes de implementar esta feature. El `conversacionId` en el detalle del acuerdo es el punto de entrada al chat para conversaciones sobre acuerdos.
- **US-CS-02** (cs-gestionar-necesidades): Prerequisito de contexto. Las conversaciones sobre necesidades requieren que existan necesidades y propuestas enviadas que establezcan la relacion entre artista y profesional.

---

## Out of Scope

- Notificaciones push o email cuando llega un nuevo mensaje
- WebSocket o Server-Sent Events para actualizacion en tiempo real (MVP: polling)
- Envio de archivos adjuntos directamente (MVP: solo URLs externas)
- Mensajes de grupo (MVP: conversaciones siempre entre dos participantes)
- Borrado o edicion de mensajes enviados
- Reacciones o respuestas anidadas a mensajes
- Busqueda de mensajes por contenido
- Moderacion de mensajes por parte del Admin
- Historial de mensajes exportable

---

## Referencia

- User Story completa: [docs/product/US-CS-05-mensajeria.md](../../product/US-CS-05-mensajeria.md)
- US-CS-04 Acuerdos y Entregables: [docs/user-stories/cs-acuerdos-entregables/feature-spec.md](../cs-acuerdos-entregables/feature-spec.md)
- US-CS-02 Gestionar Necesidades: [docs/user-stories/cs-gestionar-necesidades/feature-spec.md](../cs-gestionar-necesidades/feature-spec.md)
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
- CLAUDE.md: Patron CQRS, reglas de validacion, ServiceResponse
