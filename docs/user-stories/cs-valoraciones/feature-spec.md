# Feature: Valoraciones Bidireccionales

> **ID:** cs-valoraciones
> **User Story:** US-CS-06
> **Status:** proposed
> **Priority:** Baja
> **Sprint:** TBD

---

## Descripcion

Esta feature cierra el ciclo de reputacion del modulo crowdsourcing permitiendo que artistas y profesionales se valoren mutuamente al finalizar un acuerdo de trabajo. Cada parte puede dejar una puntuacion de 1 a 5 estrellas con un comentario opcional, y esa valoracion queda visible publicamente en el perfil del usuario valorado. Las valoraciones son inmutables: una vez enviadas no pueden editarse, lo que garantiza la integridad del sistema de reputacion de la plataforma.

La visibilidad de valoraciones es publica para cualquier usuario autenticado. El perfil de un profesional muestra puntuacion media (con 1 decimal), contador total e histograma de distribucion, ademas del listado paginado de valoraciones ordenado por fecha descendente. Esta informacion aparece tambien como badge en los listados de propuestas y perfiles de profesionales, apoyando la toma de decisiones de los artistas al evaluar candidatos.

Esta feature depende directamente de US-CS-04 (Acuerdos y Entregables) porque el sistema de valoraciones solo se activa sobre acuerdos en estado `Completado`. No tiene impacto en el dashboard Admin ya que toda la interaccion ocurre en la Landing publica.

---

## User Story

**Como** artista o profesional que ha completado un acuerdo de trabajo,
**Quiero** dejar una valoracion (puntuacion 1-5 y comentario opcional) a la otra parte y poder consultar las valoraciones de cualquier usuario,
**Para** compartir mi experiencia, contribuir a la reputacion en la plataforma y ayudar a otros a tomar decisiones informadas.

---

## Flujo Principal

### 1. Dejar Valoracion

```
1. Usuario (artista o profesional) accede al detalle de un acuerdo en estado Completado
2. El sistema verifica si el usuario ya dejo valoracion para ese acuerdo
3. Si no valoro, muestra el CTA y formulario: puntuacion de 1-5 estrellas con hover effect
   y campo de comentario opcional (max 1000 chars) con mensaje incentivo
4. Usuario selecciona puntuacion y opcionalmente escribe comentario
5. Usuario hace click en "Enviar valoracion"
6. El sistema ejecuta CreateValoracionCommand:
   - Valida que el acuerdo esta en estado Completado
   - Valida que el usuario es participante del acuerdo
   - Valida que el usuario no ha valorado previamente ese acuerdo
   - Determina automaticamente quien es el UserIdValorado (la otra parte)
   - Persiste ValoracionCrowdsourcing con FechaCreacion
7. El sistema muestra toast: "Valoracion enviada. Gracias por tu feedback."
8. El formulario se reemplaza por la vista de solo lectura de la valoracion enviada
```

### 2. Ver Valoraciones de un Usuario

```
1. Usuario (cualquier usuario autenticado) accede al perfil de otro usuario
2. El sistema ejecuta GetValoracionesByUserQuery para ese userId
3. Si el usuario no tiene valoraciones, se muestra mensaje de estado vacio
4. Si tiene valoraciones, se muestra:
   a. Resumen: puntuacion media con 1 decimal, total de valoraciones, histograma 1-5
   b. Listado paginado ordenado por FechaCreacion descendente
      - Cada item muestra: puntuacion en estrellas, nombre del autor, titulo del acuerdo, fecha y comentario
5. El usuario puede navegar entre paginas del listado
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Acuerdo no esta en estado Completado | CTA de valoracion no visible en la seccion de valoraciones del detalle del acuerdo |
| FA-02 | El usuario ya dejo valoracion para ese acuerdo | Se muestra la valoracion existente en modo solo lectura; el formulario no aparece |
| FA-03 | Usuario consultado no tiene valoraciones | Se muestra mensaje: "Este usuario aun no tiene valoraciones" |
| FA-04 | Intento de crear segunda valoracion via API directa | Backend retorna 400 con error de regla de negocio (duplicado) |
| FA-05 | Puntuacion fuera del rango 1-5 | Backend retorna 400 con error de validacion; frontend impide la seleccion invalida |
| FA-06 | Comentario supera 1000 caracteres | Backend retorna 400; frontend muestra contador de caracteres y bloquea el envio |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto | Metodo de Prueba |
|----|----------|----------|------------------|
| AC-CS06-1 | Solo se puede crear una valoracion sobre un acuerdo en estado `Completado`. Si el acuerdo esta en otro estado, el backend retorna 400 con errorCode de regla de negocio. | Backend + Landing | Intentar POST /valoraciones sobre un acuerdo en estado Activo; verificar 400. Intentar sobre un acuerdo Completado como participante; verificar 201. |
| AC-CS06-2 | Cada participante solo puede dejar una valoracion por acuerdo. El segundo intento retorna 400. Existe constraint unico en BD (AcuerdoId + UserIdAutor). | Backend + Landing | Enviar valoracion exitosa. Intentar una segunda valoracion para el mismo acuerdo con el mismo usuario; verificar 400. Verificar constraint unico en migracion. |
| AC-CS06-3 | La puntuacion es de 1 a 5 estrellas con seleccion visual con hover effect. El componente de estrellas no permite valores fuera del rango. | Landing | Verificar que al pasar el cursor por las estrellas se aplica el efecto visual. Verificar que solo se pueden seleccionar valores del 1 al 5. |
| AC-CS06-4 | El formulario de valoracion muestra el mensaje de incentivo: "Tu comentario ayuda a otros artistas/profesionales". El comentario no es obligatorio; se puede enviar una valoracion sin el. | Landing | Renderizar el formulario; verificar que el texto del incentivo aparece. Enviar valoracion sin comentario; verificar que se crea correctamente. |
| AC-CS06-5 | Una vez enviada la valoracion no puede editarse. El formulario se reemplaza por vista de solo lectura. El toast muestra exactamente: "Valoracion enviada. Gracias por tu feedback." | Backend + Landing | Enviar valoracion; verificar toast con texto exacto. Verificar que la seccion pasa a solo lectura. Verificar que no existe endpoint PATCH/PUT de valoracion. |
| AC-CS06-6 | Cualquier usuario autenticado puede consultar las valoraciones de cualquier otro usuario via GET /api/crowdsourcing/usuarios/{userId}/valoraciones. | Backend | Autenticarse como usuario que no es participante del acuerdo. GET valoraciones de otro usuario; verificar 200 con datos correctos. |
| AC-CS06-7 | La respuesta incluye resumen con puntuacion media calculada a 1 decimal (AVG en query SQL), total de valoraciones y distribucion por estrellas (histograma con GROUP BY puntuacion). | Backend | Crear 5 valoraciones con puntuaciones [5, 5, 4, 3, 2]. Consultar resumen; verificar media=3.8, total=5, distribucion={5:2, 4:1, 3:1, 2:1, 1:0}. |
| AC-CS06-8 | Las valoraciones del listado estan ordenadas por FechaCreacion descendente. El endpoint soporta paginacion via parametros `page` y `pageSize`. La respuesta incluye `totalCount`, `page` y `pageSize`. | Backend + Landing | Crear 15 valoraciones. Consultar pagina 1 con pageSize=10; verificar 10 items ordenados por fecha desc. Consultar pagina 2; verificar 5 items restantes. |
| AC-CS06-9 | Si el usuario no tiene valoraciones, la seccion muestra el mensaje: "Este usuario aun no tiene valoraciones" (estado vacio). | Landing | Acceder a perfil de usuario sin valoraciones; verificar que aparece el mensaje de estado vacio en lugar del resumen y listado. |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar `CreateValoracionCommand` + Handler (con validaciones de negocio: estado acuerdo, participante, unicidad). Implementar `GetValoracionesByUserQuery` + Handler (con resumen AVG/COUNT/GROUP BY y listado paginado). Validator `CreateValoracionValidator`. Nuevo `IValoracionCrowdsourcingService`. Nuevo `IValoracionCrowdsourcingRepository`. Migracion para tabla `ValoracionesCrowdsourcing` con constraint unico (AcuerdoId + UserIdAutor). Nuevo controller `ValoracionesController` con los 2 endpoints. AutoMapper profile `ValoracionProfile`. | ALTO |
| **Shared** | Definir TypeScript types: `ValoracionDto` (id, puntuacion, comentario, autorNombre, autorImagenUrl, acuerdoTituloInterno, fechaCreacion), `ValoracionResumenDto` (puntuacionMedia, totalValoraciones, distribucion), `ValoracionesByUserDto` (resumen + listado paginado), `CreateValoracionRequest` (puntuacion, comentario). Schema Zod: `createValoracionSchema`. Constants: actualizar `QUERY_KEYS` con claves para valoraciones. Actualizar `API_ROUTES` con las 2 rutas nuevas. | MEDIO |
| **Landing** | Implementar seccion de valoracion en `AcuerdoDetallePage`: CTA o formulario segun si ya valoro, vista de solo lectura post-envio. Componente `StarRating` con hover effect (custom o libreria, no nativo de shadcn). Componente `ValoracionForm` (StarRating + textarea + mensaje incentivo + boton enviar). Componente `ValoracionEnviada` (solo lectura). Componente `ValoracionesSection` en perfil de usuario: resumen (media + total + histograma), listado paginado `ValoracionCard`. Componente `ValoracionEmptyState`. Badge de puntuacion media en tarjetas de propuestas y listados de profesionales. Hooks: `useCreateValoracion`, `useValoracionesByUser`. Services: `valoracion.service.ts`. | ALTO |
| **Admin** | No involucrado. Toda la interaccion de valoraciones ocurre en la Landing. | BAJO |

---

## Entidades Involucradas

### ValoracionCrowdsourcing (nueva)

Registra la valoracion que un participante deja sobre la otra parte al completar un acuerdo.

**Campos clave:**
- `Id` (Guid, PK)
- `AcuerdoId` (Guid, FK -> AcuerdoCrowdsourcing, NOT NULL)
- `UserIdAutor` (string, FK -> Identity.User, NOT NULL) - Usuario que emite la valoracion
- `UserIdValorado` (string, FK -> Identity.User, NOT NULL) - Usuario que recibe la valoracion
- `Puntuacion` (int, NOT NULL, CHECK 1-5) - Puntuacion en estrellas
- `TipoValoracionId` (int, nullable, FK -> MaestraTipoValoracion) - Artista->Profesional o Profesional->Artista
- `Comentario` (string, nullable, max 1000) - Texto de la valoracion
- `FechaCreacion` (DateTime, NOT NULL)

**Constraint unico en BD:**
- `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor` - Garantiza una sola valoracion por participante por acuerdo

**Relaciones:**
- `Acuerdo` (navigation property -> AcuerdoCrowdsourcing)

**Reglas de negocio:**
- Solo se puede crear sobre un `AcuerdoCrowdsourcing` en estado `Completado`
- Solo puede crear la valoracion un participante del acuerdo (ArtistaId o UserIdProveedor)
- El `UserIdValorado` se determina automaticamente en el backend (la otra parte del acuerdo)
- No existe operacion de edicion ni eliminacion

### AcuerdoCrowdsourcing (existente, relacion nueva)

Ya introducida en US-CS-04. En esta feature se añade la relacion de navegacion:
- `Valoraciones` (collection -> ValoracionCrowdsourcing) - Ya prevista en el modelo de US-CS-04

---

## API Endpoints

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

Crear valoracion sobre un acuerdo completado. Solo participantes del acuerdo.

**Auth:** Participante del acuerdo (artista o profesional)

**Request:**
```json
{
  "puntuacion": 5,
  "comentario": "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles."
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "puntuacion": 5,
    "comentario": "Excelente trabajo, muy profesional y puntual...",
    "fechaCreacion": "2026-03-16T10:00:00Z"
  },
  "messages": [
    { "message": "Valoracion enviada. Gracias por tu feedback.", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Acuerdo no esta en estado Completado (`BusinessRule_InvalidState`)
- `400 Bad Request` - El usuario ya dejo valoracion para este acuerdo (`BusinessRule_DuplicateAction`)
- `400 Bad Request` - Validacion de puntuacion (fuera de rango 1-5) o comentario (supera 1000 chars)
- `403 Forbidden` - El usuario no es participante del acuerdo
- `404 Not Found` - Acuerdo no existe

---

### GET /api/crowdsourcing/usuarios/{userId}/valoraciones

Obtener valoraciones recibidas de un usuario con resumen estadistico y listado paginado.

**Auth:** Cualquier usuario autenticado

**Query params:** `page` (default: 1), `pageSize` (default: 10)

**Response 200 OK:**
```json
{
  "data": {
    "resumen": {
      "puntuacionMedia": 4.5,
      "totalValoraciones": 12,
      "distribucion": {
        "5": 7,
        "4": 3,
        "3": 1,
        "2": 1,
        "1": 0
      }
    },
    "valoraciones": {
      "items": [
        {
          "id": "guid",
          "puntuacion": 5,
          "comentario": "Excelente trabajo, muy profesional y puntual...",
          "autorNombre": "Los Rockeros",
          "autorImagenUrl": "https://...",
          "acuerdoTituloInterno": "Mezcla EP Los Rockeros",
          "fechaCreacion": "2026-03-16T10:00:00Z"
        }
      ],
      "totalCount": 12,
      "page": 1,
      "pageSize": 10
    }
  },
  "messages": []
}
```

**Errores:**
- `404 Not Found` - Usuario no existe

---

## Validaciones

### CreateValoracionValidator

```csharp
RuleFor(x => x.Puntuacion)
    .InclusiveBetween(1, 5)
    .WithMessage("La puntuacion debe ser entre 1 y 5")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

RuleFor(x => x.Comentario)
    .MaximumLength(1000)
    .When(x => !string.IsNullOrEmpty(x.Comentario))
    .WithMessage("El comentario no puede superar los 1000 caracteres")
    .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

// Business rule: acuerdo debe estar en estado Completado y usuario debe ser participante sin valoracion previa
// Se validan en el Handler/Service usando IRequestCacheService para no duplicar queries
```

---

## Requisitos No Funcionales

### Performance
- La query de resumen (AVG, COUNT, GROUP BY) se ejecuta a nivel SQL, no en memoria
- La paginacion se implementa con `Skip/Take` en la query (no cargar todos en memoria)
- Usar `IRequestCacheService` para compartir la lectura del acuerdo entre el validator y el handler en la misma request (evitar query duplicado)
- El GET de valoraciones con resumen y pagina 1 debe responder en menos de 300ms

### Seguridad
- El `UserIdValorado` se determina siempre en el backend comparando el userId del JWT con los participantes del acuerdo; no se acepta del cliente
- El backend valida en todos los casos que el usuario del JWT es participante del acuerdo antes de crear la valoracion
- No existe endpoint de edicion ni eliminacion de valoraciones (integridad del sistema de reputacion)

### UX
- El componente StarRating muestra hover effect al pasar el cursor por las estrellas antes de seleccionar
- Contador de caracteres visible en el textarea del comentario
- El mensaje de incentivo "Tu comentario ayuda a otros artistas/profesionales" aparece junto al campo de comentario
- Toast inmediato con el texto exacto tras enviar correctamente
- El formulario pasa a solo lectura tras el envio exitoso (sin reload de pagina)
- El badge de puntuacion media (ej: "4.5 (12)") se muestra en tarjetas de propuestas y listados de profesionales

### Mantenibilidad
- Maestra opcional `MaestraTipoValoracion` (ArtistaAProfesional, ProfesionalAArtista) para filtros futuros
- No se pre-calcula ni almacena la puntuacion media en ninguna tabla (se calcula con AVG en cada query en MVP)
- Constraint unico a nivel de BD como segunda linea de defensa ademas de la validacion en el handler

---

## Dependencias

### Tecnicas (dentro del proyecto)
- **AcuerdoCrowdsourcing**: Entidad principal de la que depende la valoracion. Debe existir la tabla y el repositorio antes de esta US. La navegacion `Valoraciones` ya esta prevista en el modelo de US-CS-04.
- **Identity.User**: Referenciado por `UserIdAutor` y `UserIdValorado`. Se usa el UserId del JWT para identificar al autor.
- **Artista**: Necesario para obtener el `NombreArtistico` del autor en el listado de valoraciones cuando el autor es un artista.
- **PerfilProfesional**: Necesario para obtener el nombre del profesional en el listado cuando el autor es un profesional.
- **Maestras nuevas**: `MaestraTipoValoracion` (opcional en MVP, el campo TipoValoracionId es nullable).
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType` (requiere constante `BusinessRule_DuplicateAction` si no existe), `IRequestCacheService`.
- **Shared**: Types, schemas Zod, QUERY_KEYS y API_ROUTES actualizados con las rutas de valoraciones.

### De otras User Stories
- **US-CS-04** (cs-acuerdos-entregables): Prerequisito directo e imprescindible. La entidad `AcuerdoCrowdsourcing` y su estado `Completado` deben existir. Sin acuerdos completados no hay valoraciones posibles.

---

## Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Condicion de carrera: dos requests simultaneas del mismo usuario crean dos valoraciones para el mismo acuerdo antes de que el constraint lo impida | Baja | Medio | Constraint unico en BD (AcuerdoId + UserIdAutor) actua como segunda linea de defensa. Si ambas requests pasan la validacion en memoria, la segunda falla con excepcion de BD que el handler captura y convierte en 400. |
| Componente StarRating con hover effect: shadcn/ui no incluye uno nativo, requiere libreria externa o componente custom | Alta | Bajo | Evaluar libreria ligera como `react-rating` o implementar custom con Tailwind en el Sprint. Documentar la decision en el codigo. |
| La puntuacion media calculada por AVG puede ser lenta si un usuario acumula muchas valoraciones | Baja | Bajo | En MVP el volumen de valoraciones por usuario es bajo. Si escala, se puede agregar un indice en `UserIdValorado` o pre-calcular la media. Documentado como deuda tecnica. |
| Usuarios intentan valorar a la misma parte (a si mismos o a un tercero) manipulando el request | Baja | Medio | El backend calcula `UserIdValorado` a partir del JWT y los participantes del acuerdo, nunca desde el body del request. El cliente no puede elegir a quien valora. |

---

## Notas de Implementacion

### Backend

- Crear Commands/Queries en `Modules/Crowdsourcing/Crowdsourcing.Application/Features/Valoraciones/`
  - Commands: `CreateValoracionCommand.cs` (Command + Handler en mismo archivo)
  - Queries: `GetValoracionesByUserQuery.cs` (Query + Handler en mismo archivo)
  - Validators: `CreateValoracionValidator.cs`
- Implementar `ValoracionCrowdsourcingService : IValoracionCrowdsourcingService` con metodos:
  - `CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct)`
  - `ExisteValoracionAsync(Guid acuerdoId, string userIdAutor, CancellationToken ct)`
  - `GetResumenByUserIdAsync(string userId, CancellationToken ct)` (AVG + COUNT + GROUP BY)
  - `GetByUserIdPagedAsync(string userId, int page, int pageSize, CancellationToken ct)`
- Implementar `ValoracionCrowdsourcingRepository : IValoracionCrowdsourcingRepository` con los metodos anteriores
- `CreateValoracionCommandHandler` usa `IRequestCacheService` para obtener el acuerdo una sola vez (compartido con el validator)
- La logica de determinar `UserIdValorado` se implementa en el Handler comparando `command.UserId` con `acuerdo.ArtistaId` y `acuerdo.UserIdProveedor`
- AutoMapper Profile: `ValoracionProfile.cs`
- Controller: `ValoracionesController.cs` con los 2 endpoints
- Migracion: `AddValoracionesCrowdsourcing` que crea la tabla con el constraint unico

### Frontend (Landing)

- Seccion de valoraciones en `AcuerdoDetallePage.tsx` (ya existente en US-CS-04):
  - Si acuerdo.estadoAcuerdoId != Completado: no renderizar la seccion
  - Si el usuario ya valoro: renderizar `ValoracionEnviada` (solo lectura)
  - Si no ha valorado: renderizar `ValoracionForm`
- Nuevo componente `StarRating.tsx` con hover effect: implementar custom o usar libreria ligera
- Nuevo componente `ValoracionesSection.tsx` en perfil de usuario:
  - `ValoracionResumen.tsx` (media, total, histograma de barras con Tailwind)
  - `ValoracionCard.tsx` (estrellas, autor, titulo acuerdo, fecha, comentario)
  - `ValoracionEmptyState.tsx`
- Badge de puntuacion media: agregar al componente de tarjeta de propuesta y de listado de profesionales
- Hooks: `useCreateValoracion.ts` (useMutation, invalida query de valoraciones del acuerdo), `useValoracionesByUser.ts` (useQuery con paginacion)
- Services: `valoracion.service.ts` con los 2 metodos de API

### Testing

- **Unit tests Backend:**
  - `CreateValoracionCommandHandlerTests` (acuerdo completado exitoso, acuerdo no completado, ya valorado, no participante)
  - `CreateValoracionValidatorTests` (puntuacion invalida, comentario maximo)
  - `GetValoracionesByUserQueryHandlerTests` (con valoraciones, sin valoraciones, calculo de media)
- **Integration tests:**
  - Flujo completo: completar acuerdo -> artista valora profesional -> profesional valora artista -> verificar resumen actualizado
  - Verificar que un tercero puede consultar las valoraciones pero no crearlas
  - Verificar que el constraint unico impide segunda valoracion
- **E2E tests:**
  - Completar acuerdo y enviar valoracion como artista (toast + paso a solo lectura)
  - Ver sección de valoraciones en perfil con resumen e histograma
  - Verificar empty state en usuario sin valoraciones

---

## Out of Scope (No incluido en esta feature)

- Edicion o eliminacion de valoraciones una vez enviadas
- Respuesta del valorado a una valoracion recibida (replica de reseña)
- Valoraciones de acuerdos cancelados (solo acuerdos completados)
- Notificaciones push o email cuando se recibe una valoracion
- Moderacion o reporte de valoraciones por parte del Admin
- Pre-calculo de puntuacion media almacenado en la entidad del profesional (post-MVP si hay problemas de performance)
- Ordenacion alternativa del listado (por puntuacion, por relevancia)
- Filtracion del listado por tipo de valoracion (artista a profesional vs profesional a artista)
- Valoraciones visibles para usuarios no autenticados

---

## Referencia

- User Story completa: [docs/product/US-CS-06-valoraciones.md](../../product/US-CS-06-valoraciones.md)
- US-CS-04 Acuerdos y Entregables: [docs/user-stories/cs-acuerdos-entregables/feature-spec.md](../cs-acuerdos-entregables/feature-spec.md)
- CLAUDE.md: Patron CQRS, reglas de validacion, ServiceResponse
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
