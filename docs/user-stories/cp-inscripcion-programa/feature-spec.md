# Feature: Inscripcion en Programa

> **ID:** cp-inscripcion-programa
> **User Story:** US-CP-03
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature implementa el ciclo completo de inscripcion de un promotor a un programa de promocion: desde que el promotor explora el catalogo publico de programas activos hasta que el artista aprueba, rechaza o bloquea su solicitud. Es el mecanismo de control de calidad del modulo Crowdpromotion: los artistas eligen quienes representan su campana, y solo los promotores aprobados reciben las herramientas de tracking (codigo referido y URL personalizada) necesarias para comenzar a difundir.

El flujo de inscripcion tiene dos actores con vistas separadas: el promotor interactua desde la Landing publica (explorar programas, solicitar inscripcion, ver mis programas) y el artista gestiona las solicitudes desde el Admin dashboard (aprobar, rechazar, bloquear, dar de baja). La generacion del codigo referido y la URL de tracking ocurre automaticamente en el momento de la aprobacion, construida server-side a partir del `CodigoTrackingBase` del programa y un identificador corto unico por inscripcion.

La feature tiene un impacto transversal: involucra al Backend para la logica de negocio y generacion de codigos, a la Landing para las vistas del promotor, al Admin para la gestion por parte del artista, y a Shared para los tipos y schemas que ambos frontends comparten. Depende directamente de que exista un perfil de promotor activo (US-CP-01) y de que haya programas publicados por artistas (US-CP-02).

---

## User Story

**Como** promotor registrado
**Quiero** explorar los programas de promocion disponibles, solicitar mi inscripcion en los que me interesen, y recibir un codigo referido unico y URL de tracking personalizada al ser aprobado por el artista
**Para** comenzar a difundir la campana con enlaces rastreables que acrediten mis resultados

---

## Actores

| Actor | Descripcion |
|-------|-------------|
| Promotor | Usuario con perfil de promotor activo (EsActivo = true) |
| Artista | Propietario del programa de promocion, gestiona las solicitudes |

---

## Precondiciones

- Promotor tiene perfil activo (EsActivo = true, establecido en US-CP-01)
- Existen programas de promocion activos creados por artistas (US-CP-02)

---

## Flujo Principal: Explorar y Solicitar Inscripcion (Promotor)

```
1. Promotor autenticado accede a /crowdpromotion/explorar en la Landing
2. Sistema muestra listado paginado de programas activos con su estado personal (no inscrito / pendiente / aprobado / bloqueado)
3. Promotor aplica filtros opcionales: artista, tipo de programa, comision
4. Promotor hace click en un programa para ver el detalle: descripcion, comisiones, tareas disponibles
5. Si el promotor ya esta inscrito → sistema muestra su estado actual sin boton de solicitud (FA-01 / FA-02)
6. Si el promotor no esta inscrito → sistema muestra boton "Solicitar inscripcion"
7. Promotor hace click en "Solicitar inscripcion"
8. Backend valida: promotor activo, programa activo, sin inscripcion previa, sin bloqueo previo
9. Backend crea PromoProgramaPromotor con estado pendiente (EsAprobado = false, EsBloqueado = false)
10. Frontend muestra toast: "Solicitud enviada al artista"
11. El programa aparece ahora con estado "Pendiente de aprobacion" en el listado del promotor
```

---

## Flujo Secundario: Gestionar Solicitudes (Artista)

```
1. Artista autenticado accede al detalle de un programa en el Admin (/crowdpromotion/programas/{id})
2. Sistema muestra la pestana "Solicitudes pendientes" con los promotores pendientes de revision
3. Artista ve el perfil del promotor (nombre publico, tipo, redes sociales)
4. Artista elige una accion:
   a. Aprobar: Backend marca EsAprobado = true, genera CodigoReferido y UrlTrackingPersonalizada
   b. Rechazar: Backend elimina fisicamente el registro PromoProgramaPromotor
   c. Bloquear: Backend marca EsBloqueado = true; el promotor no puede re-solicitar
5. Frontend muestra toast con el resultado de la accion
```

---

## Flujo Secundario: Dar de Baja a Promotor Aprobado (Artista)

```
1. Artista accede a la lista de promotores aprobados en el detalle del programa
2. Artista hace click en "Dar de baja" para un promotor aprobado
3. Sistema muestra dialogo de confirmacion
4. Artista confirma
5. Backend establece FechaBaja = now y EsAprobado = false en el registro
6. El CodigoReferido se desactiva (el promotor pierde acceso a la URL de tracking)
7. Frontend muestra toast: "Promotor dado de baja"
```

---

## Flujo Secundario: Mis Programas (Promotor)

```
1. Promotor autenticado accede a /promotor/mis-programas en la Landing
2. Si no tiene inscripciones → sistema muestra empty state con enlace a /crowdpromotion/explorar
3. Si tiene inscripciones → sistema muestra listado con badge de estado por cada inscripcion (Pendiente / Aprobado / Bloqueado)
4. Promotor hace click en una inscripcion aprobada
5. Sistema muestra el codigo referido y la URL de tracking con boton de copiado
6. Sistema muestra las tareas del programa para ese promotor
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Promotor ya tiene inscripcion en el programa (cualquier estado) | No mostrar boton de solicitud; mostrar badge con el estado actual |
| FA-02 | Promotor tiene estado bloqueado en ese programa | Mostrar mensaje: "No puedes inscribirte en este programa"; no mostrar boton de solicitud |
| FA-03 | Programa inactivo o fuera de fechas de vigencia | No aparece en el listado publico de explorar |
| FA-04 | Promotor con perfil desactivado (EsActivo = false) intenta solicitar inscripcion | Backend retorna 400 Bad Request con mensaje descriptivo |
| FA-05 | Artista aprueba un promotor pero el programa se desactivo entre tanto | Se permite la aprobacion; la URL generada no sera funcional hasta que el programa vuelva a estar activo |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP03-1 | Un promotor autenticado puede ver un listado paginado de programas activos en /crowdpromotion/explorar; el listado muestra titulo, artista, tipo, comision, moneda, numero de tareas, campana vinculada, fechas y su estado personal | Backend + Landing |
| AC-CP03-2 | El promotor puede filtrar el listado por nombre de artista y tipo de programa | Backend + Landing |
| AC-CP03-3 | Al solicitar inscripcion, se crea un registro PromoProgramaPromotor con EsAprobado = false y EsBloqueado = false vinculado al PromotorId y ProgramaId correctos | Backend |
| AC-CP03-4 | La combinacion PromotorId + ProgramaId es unica en BD; un segundo intento de inscripcion en el mismo programa retorna error 400 con mensaje descriptivo | Backend |
| AC-CP03-5 | Un promotor con perfil inactivo (EsActivo = false) recibe error 400 al intentar solicitar inscripcion | Backend |
| AC-CP03-6 | Un promotor con estado bloqueado en un programa no puede re-solicitar; el frontend oculta el boton de solicitud y el backend retorna 403 si se intenta por API | Backend + Landing |
| AC-CP03-7 | El artista ve la lista de solicitudes pendientes en el detalle de cada programa de su propiedad | Backend + Admin |
| AC-CP03-8 | Al aprobar una solicitud, el backend genera automaticamente CodigoReferido con el patron {CodigoTrackingBase}-{ShortId} y UrlTrackingPersonalizada con parametros UTM correctos; ambos se persisten en BD | Backend |
| AC-CP03-9 | El CodigoReferido generado es unico entre todas las inscripciones; dos promotores distintos en el mismo programa tienen codigos diferentes | Backend |
| AC-CP03-10 | La UrlTrackingPersonalizada sigue el patron: {UrlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={CodigoTrackingBase}&ref={CodigoReferido} | Backend |
| AC-CP03-11 | El artista puede rechazar una solicitud pendiente; el registro PromoProgramaPromotor se elimina fisicamente de BD | Backend |
| AC-CP03-12 | El artista puede bloquear un promotor; el registro PromoProgramaPromotor queda con EsBloqueado = true; el promotor no puede re-solicitar | Backend |
| AC-CP03-13 | El artista puede dar de baja a un promotor aprobado; el registro queda con FechaBaja = now y EsAprobado = false | Backend |
| AC-CP03-14 | El promotor ve su CodigoReferido y UrlTrackingPersonalizada unicamente cuando su inscripcion tiene EsAprobado = true | Backend + Landing |
| AC-CP03-15 | En /promotor/mis-programas el promotor ve todas sus inscripciones con badge de estado (Pendiente / Aprobado / Bloqueado); las inscripciones dadas de baja muestran estado Baja | Backend + Landing |
| AC-CP03-16 | Los tipos y schemas Zod de inscripcion estan definidos en shared y son reutilizados por landing y admin | Shared |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar los endpoints de inscripcion en el modulo Crowdpromotion: GET explorar programas, POST solicitar inscripcion, PATCH aprobar, PATCH rechazar, PATCH bloquear, PATCH dar-de-baja y GET mis-programas. Logica de generacion de CodigoReferido (patron CodigoTrackingBase + ShortId unico) y UrlTrackingPersonalizada construida server-side. Validaciones de negocio: promotor activo, programa activo, unicidad de inscripcion, no-rebloqueo. Aplicar migracion de EF Core para agregar campos EsAprobado y EsBloqueado a PromoProgramaPromotor. | ALTO |
| **Landing** | Implementar pagina /crowdpromotion/explorar con listado paginado y filtros. Implementar vista de detalle de programa con boton de solicitud condicional segun estado del promotor. Implementar pagina /promotor/mis-programas con listado de inscripciones, badges de estado y vista de codigo referido + URL de tracking con copiado al portapapeles. | ALTO |
| **Admin** | Implementar la pestana "Solicitudes pendientes" en el detalle de programa del artista. Implementar la lista de promotores aprobados con la accion "Dar de baja". Implementar los botones de Aprobar, Rechazar y Bloquear con dialogo de confirmacion para las acciones destructivas. | ALTO |
| **Shared** | Definir tipos TypeScript para PromoProgramaPromotorDto, InscripcionEstado (union type: 'Pendiente' | 'Aprobado' | 'Bloqueado' | 'Baja'), ExploracionProgramaDto (catalogo publico con miEstado). Definir schema Zod para la solicitud de inscripcion. Agregar QUERY_KEYS para explorar-programas y mis-programas. | MEDIO |

---

## Reglas de Negocio

| Regla | Descripcion |
|-------|-------------|
| RN-01 | Un promotor solo puede tener una inscripcion activa o historica por programa; la combinacion PromotorId + ProgramaId debe ser unica en BD |
| RN-02 | Solo los promotores con EsActivo = true pueden solicitar inscripciones |
| RN-03 | Solo los programas con EsActivo = true y dentro de sus fechas de vigencia aparecen en el catalogo publico |
| RN-04 | Un promotor bloqueado (EsBloqueado = true) en un programa no puede re-solicitar inscripcion en ese programa aunque su perfil general este activo |
| RN-05 | El CodigoReferido y la UrlTrackingPersonalizada se generan server-side en el momento de la aprobacion; no son indicados por el cliente |
| RN-06 | Al rechazar una solicitud el registro se elimina fisicamente; el promotor podra volver a solicitar en el futuro |
| RN-07 | Al bloquear, el registro persiste con EsBloqueado = true para impedir re-solicitudes futuras |
| RN-08 | La baja de un promotor aprobado no elimina el registro ni el codigo; establece FechaBaja y EsAprobado = false, dejando trazabilidad historica |
| RN-09 | El CodigoReferido generado debe ser unico en la tabla; debe reintentarse la generacion si se produce colision |

---

## Cambios de Modelo de Dominio Requeridos

La entidad `PromoProgramaPromotor` existente en el codebase NO incluye los campos `EsAprobado` ni `EsBloqueado`, que la User Story requiere para gestionar el flujo de aprobacion. La entidad actual usa `EsActivo` como unico indicador de estado. Antes de implementar los endpoints es necesario:

**PromoProgramaPromotor - campos a agregar:**
- `public bool EsAprobado { get; set; }` (default: false; indica que el artista ha aprobado la solicitud)
- `public bool EsBloqueado { get; set; }` (default: false; indica que el artista ha bloqueado al promotor en este programa)

**Interpretacion de estados derivados del modelo:**

| EsAprobado | EsBloqueado | FechaBaja | Estado visible |
|------------|-------------|-----------|----------------|
| false | false | null | Pendiente |
| true | false | null | Aprobado |
| false | true | null | Bloqueado |
| false | false | valor | Baja |

**Campo UrlReferido existente:** el campo `UrlReferido` ya existe en la entidad y se usara para almacenar la `UrlTrackingPersonalizada`. No se agrega un campo nuevo; se reutiliza con la semantica definida en la US.

Todos los cambios son aditivos (columnas bool con valor default false) y no rompen registros existentes. Se debe generar y aplicar una nueva migracion de EF Core.

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity activo con JWT: el PromotorId y el ArtistaId se extraen del claim del token en cada handler
- Tabla `PromoProgramaPromotor` con indice unico compuesto sobre (ProgramaId, PromotorId)
- Campos `CodigoTrackingBase` y `UrlLanding` existentes en `PromoPrograma` (agregados en US-CP-02); son necesarios para la construccion del codigo referido y la URL de tracking
- Libreria o utilidad para generacion de ShortId alfanumerico (puede ser nanoid en Node o un helper de Guid truncado en .NET)

### De otras features

- **cp-perfil-promotor (US-CP-01):** debe estar implementada; provee la entidad `Promotor`, el campo `EsActivo` y los endpoints de perfil que el Admin consume para mostrar datos del promotor al artista en la pantalla de solicitudes
- **cp-programas-promocion (US-CP-02):** debe estar implementada; provee la entidad `PromoPrograma` con los campos `EsActivo`, `CodigoTrackingBase`, `UrlLanding` y `FechaInicio/Fin` que esta feature necesita para el catalogo y para generar los codigos

---

## Requisitos No Funcionales

| ID | Requisito |
|----|-----------|
| RNF-01 | El listado de explorar programas debe responder en menos de 500ms para paginas de hasta 20 items |
| RNF-02 | La generacion del CodigoReferido en el handler de aprobacion debe completarse en la misma transaccion que la actualizacion del registro; si la generacion falla, la aprobacion no se persiste |
| RNF-03 | El CodigoReferido y la UrlTrackingPersonalizada son datos sensibles del promotor; solo deben devolverse en endpoints autenticados y al promotor propietario o al artista del programa |
| RNF-04 | El campo `miEstado` en el catalogo publico solo se calcula cuando hay usuario autenticado; para usuarios no autenticados el campo se omite o devuelve null |

---

## Referencia

- User Story completa: [docs/product/US-CP-03-inscripcion-programa.md](../../product/US-CP-03-inscripcion-programa.md)
- Feature spec cp-perfil-promotor: [docs/user-stories/cp-perfil-promotor/feature-spec.md](../cp-perfil-promotor/feature-spec.md)
- Feature spec cp-programas-promocion: [docs/user-stories/cp-programas-promocion/feature-spec.md](../cp-programas-promocion/feature-spec.md)
