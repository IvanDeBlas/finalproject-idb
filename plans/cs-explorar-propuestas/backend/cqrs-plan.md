# Plan CQRS: cs-explorar-propuestas

**Fecha:** 2026-02-17
**Modulo:** Crowdsourcing
**Feature:** cs-explorar-propuestas (US-CS-03)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Listar necesidades publicas | Query | `GetNecesidadesPublicasQuery` | `ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>` |
| Ver detalle de necesidad publica | Query | `GetNecesidadPublicaByIdQuery` | `ServiceResponse<NecesidadPublicaDto>` |
| Enviar propuesta a necesidad | Command | `CreatePropuestaCommand` | `ServiceResponse<PropuestaCreatedResultDto>` |
| Listar mis propuestas | Query | `GetMisPropuestasQuery` | `ServiceResponse<PaginatedResponse<MiPropuestaListDto>>` |
| Retirar propuesta propia | Command | `RetirarPropuestaCommand` | `ServiceResponse<RetirarPropuestaResultDto>` |

---

## 2. Constantes Nuevas en ServiceResponseMessageType

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

Las constantes existentes llegan hasta `4002`. Agregar al final de cada seccion:

```
// NotFound (2000-2999) - AGREGAR:
public const string NotFound_Propuesta = "2010";

// Business Rules (4000-4999) - AGREGAR:
public const string BusinessRule_AlreadyProposed         = "4003";
public const string BusinessRule_CannotProposeSelf       = "4004";
public const string BusinessRule_PropuestaNotRetirable   = "4005";
public const string BusinessRule_NoProfessionalProfile   = "4006";
```

**Tambien crear:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/EstadoPropuestaConstants.cs`

```
public static class EstadoPropuestaConstants
{
    public const int Pendiente = 1;
    public const int Aceptada  = 2;
    public const int Rechazada = 3;
    public const int Retirada  = 4;
}
```

Esto elimina los magic numbers (1, 4, etc.) actualmente hardcodeados en services y repositories.

---

## 3. Queries

### 3.1 GetNecesidadesPublicasQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetNecesidadesPublicasQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo

**No requiere Validator:** query de lectura con parametros todos opcionales (paginacion usa defaults).

#### Query

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | Para excluir necesidades del artista autenticado |
| `TipoNecesidadId` | `int?` | No | Query param | Filtro por tipo de necesidad |
| `ModalidadTrabajoId` | `int?` | No | Query param `modalidad` | Filtro por modalidad (1=Presencial, 2=Remoto, 3=Hibrido) |
| `PresupuestoMin` | `decimal?` | No | Query param | Presupuesto minimo del filtro de rango |
| `PresupuestoMax` | `decimal?` | No | Query param | Presupuesto maximo del filtro de rango |
| `Pais` | `string?` | No | Query param | Filtro por pais de ubicacion |
| `Search` | `string?` | No | Query param | Busqueda en titulo y descripcion |
| `OrderBy` | `string?` | No | Query param (default: `"recientes"`) | Orden: `recientes`, `mayor-presupuesto`, `fecha-limite` |
| `Page` | `int` | No | Query param (default: 1) | Pagina actual |
| `PageSize` | `int` | No | Query param (default: 12, max: 50) | Items por pagina |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>>`

#### Handler

**Dependencias del constructor:**

| Dependencia | Tipo | Patron |
|-------------|------|--------|
| `_service` | `INecesidadCrowdsourcingService` | Para obtener listado paginado publico |
| `_artistaService` | `IArtistaService` | Para resolver `ArtistaId` del usuario (para excluir propias) y nombres de artistas en listado |
| `_mapper` | `IMapper` | Para mapear entidades a DTOs |
| `_logger` | `ILogger<GetNecesidadesPublicasQueryHandler>` | Para logging de errores |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Flujo del Handle:**

```
1. Resolver ArtistaId del usuario (nullable):
   artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   artistaId = artista?.Id   // puede ser null si el usuario no tiene artista

2. Construir filtro:
   filtro = new NecesidadesPublicasFiltro(
       TiposNecesidadIds: request.TipoNecesidadId.HasValue ? [request.TipoNecesidadId.Value] : null,
       ModalidadTrabajoId: request.ModalidadTrabajoId,
       PresupuestoMin: request.PresupuestoMin,
       PresupuestoMax: request.PresupuestoMax,
       Pais: request.Pais,
       Search: request.Search,
       OrderBy: request.OrderBy ?? "recientes",
       Page: request.Page,
       PageSize: Math.Min(request.PageSize, 50)
   )

3. Obtener listado via service:
   (items, totalCount) = await _service.GetPublicasPaginatedAsync(filtro, artistaId, ct)

4. Mapear entidades a DTOs:
   dtos = _mapper.Map<List<NecesidadPublicaListDto>>(items)

5. Para cada dto en dtos:
   - Asignar TipoNecesidadNombre, ModalidadTrabajoNombre, MonedaNombre desde navigation props del entity
   - Asignar ArtistaNombre: await _artistaService.GetByIdAsync(item.ArtistaId, ct)?.NombreArtistico
     (el cache request-scoped en IArtistaService evita queries duplicados para el mismo artista)
   - Calcular EsUrgente:
     dto.EsUrgente = item.FechaLimitePropuestas.HasValue
         && item.FechaLimitePropuestas.Value <= DateTime.UtcNow.AddDays(3)
   - Asignar NumeroPropuestas desde el COUNT de Propuestas no-Retiradas

6. Construir PaginatedResponse:
   paginatedResponse = new PaginatedResponse<NecesidadPublicaListDto>
   {
       Items = dtos,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize
   }

7. Retornar:
   ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>
   {
       Data = paginatedResponse,
       Messages = [{ Message = "Necesidades obtenidas exitosamente", HttpStatusCode = OK }]
   }

8. Catch:
   _logger.LogError(ex, "Error getting necesidades publicas for user {UserId}", request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse(...)
```

**Notas de implementacion:**

- La exclusion de necesidades propias se delega al repositorio via `artistaId?` en el filtro (el repo aplica `WHERE ArtistaId != artistaId` cuando no es null).
- Usar `ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>` en el catch (patron existente en el proyecto).
- `EsUrgente` se calcula en el Handler, NO en el repositorio, para mantener la logica de negocio en la capa Application.
- Los nombres de maestras (TipoNecesidadNombre, ModalidadTrabajoNombre, etc.) se obtienen de navigation properties del entity si estan disponibles via Include, o se asignan desde maestras en memoria.

---

### 3.2 GetNecesidadPublicaByIdQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetNecesidadPublicaByIdQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo

**No requiere Validator:** query de lectura; si la necesidad no existe, el handler retorna NotFound directamente.

#### Query

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `Id` | `Guid` | Si | Route param | ID de la necesidad |
| `UserId` | `string` | Si | JWT claim `sub` | Para calcular `yaPropuso`, `esPropietario`, `tienePerfilProfesional` |

**Implementa:** `IRequest<ServiceResponse<NecesidadPublicaDto>>`

#### Handler

**Dependencias del constructor:**

| Dependencia | Tipo | Patron |
|-------------|------|--------|
| `_necesidadService` | `INecesidadCrowdsourcingService` | Para obtener la necesidad publica (con cache) |
| `_propuestaService` | `IPropuestaCrowdsourcingService` | Para calcular `yaPropuso` (con cache) |
| `_artistaService` | `IArtistaService` | Para calcular `esPropietario` y datos publicos del artista (con cache) |
| `_perfilService` | `IPerfilProfesionalService` | Para calcular `tienePerfilProfesional` (con cache) |
| `_mapper` | `IMapper` | Para mapear entidad a DTO |
| `_logger` | `ILogger<GetNecesidadPublicaByIdQueryHandler>` | Para logging de errores |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Flujo del Handle:**

```
1. Construir strongly-typed ID:
   necesidadId = new NecesidadCrowdsourcingId(request.Id)

2. Obtener necesidad publica (solo Abiertas y no expiradas):
   entity = await _necesidadService.GetPublicaByIdAsync(necesidadId, ct)
   // GetPublicaByIdAsync usa cache key "necesidad:publica:{id.Value}"
   if (entity == null):
       return ValidateExtensions.NotFoundServiceResponse<NecesidadPublicaDto>(
           "Necesidad no encontrada",
           ServiceResponseMessageType.NotFound_Necesidad)

3. Calcular campos calculados en PARALELO (o secuencial con cache):
   a. yaPropuso:
      yaPropuso = await _propuestaService.ExistePropuestaAsync(necesidadId, request.UserId, ct)
      // Usa cache "propuesta:existe:{necesidadId.Value}:{userId}"

   b. esPropietario:
      artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
      // Usa cache request-scoped en IArtistaService
      esPropietario = artista != null && entity.ArtistaId == artista.Id

   c. tienePerfilProfesional:
      tienePerfilProfesional = await _perfilService.ExistsByUserIdAsync(request.UserId, ct)
      // Usa cache "perfilprofesional:exists:user:{userId}"

4. Obtener datos publicos del artista propietario:
   artistaPropietario = await _artistaService.GetByIdAsync(entity.ArtistaId, ct)
   // IArtistaService ya tiene GetByIdAsync con cache

5. Mapear entidad a DTO:
   dto = _mapper.Map<NecesidadPublicaDto>(entity)

6. Asignar campos calculados y nombres de maestras (NO mapeados via AutoMapper):
   dto.YaPropuso              = yaPropuso
   dto.EsPropietario          = esPropietario
   dto.TienePerfilProfesional = tienePerfilProfesional
   dto.TipoNecesidadNombre    = (desde navigation property o servicio de maestras)
   dto.EstadoNecesidadNombre  = "Abierta"  // siempre Abierta en este endpoint
   dto.ModalidadTrabajoNombre = (desde navigation property)
   dto.MonedaNombre           = (desde navigation property)
   dto.NumeroPropuestas       = entity.Propuestas.Count(p => p.EstadoPropuestaId != EstadoPropuestaConstants.Retirada)
   dto.Artista = new ArtistaPublicoDto
   {
       Id              = artistaPropietario?.Id.Value ?? entity.ArtistaId.Value,
       NombreArtistico = artistaPropietario?.NombreArtistico ?? string.Empty,
       ImagenUrl       = artistaPropietario?.ImagenPerfilUrl
   }

7. Retornar:
   ServiceResponse<NecesidadPublicaDto>
   {
       Data = dto,
       Messages = [{ Message = "Necesidad obtenida exitosamente", HttpStatusCode = OK }]
   }

8. Catch:
   _logger.LogError(ex, "Error getting necesidad publica {NecesidadId}", request.Id)
   return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadPublicaDto>(...)
```

**Notas de implementacion:**

- Los tres campos calculados (`yaPropuso`, `esPropietario`, `tienePerfilProfesional`) se calculan usando services con cache request-scoped (ADR-006). Si el Validator de CreatePropuesta ya cargo los mismos datos en la misma request, el cache devuelve sin query a BD.
- `NumeroPropuestas` en el DTO cuenta propuestas activas (excluye `EstadoPropuestaId == 4` usando `EstadoPropuestaConstants.Retirada`).
- El nombre del estado es siempre "Abierta" porque el service ya filtra por `EstadoNecesidadId == 1`.

---

### 3.3 GetMisPropuestasQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetMisPropuestasQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo

**No requiere Validator:** query de lectura; siempre filtra por UserId del token JWT.

#### Query

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | Filtra propuestas del usuario autenticado |
| `EstadoPropuestaId` | `int?` | No | Query param `estado` | Filtro por estado (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) |
| `Page` | `int` | No | Query param (default: 1) | Pagina actual |
| `PageSize` | `int` | No | Query param (default: 10, max: 50) | Items por pagina |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<MiPropuestaListDto>>>`

#### Handler

**Dependencias del constructor:**

| Dependencia | Tipo | Patron |
|-------------|------|--------|
| `_service` | `IPropuestaCrowdsourcingService` | Para obtener propuestas paginadas del usuario |
| `_mapper` | `IMapper` | Para mapear entidades a DTOs |
| `_logger` | `ILogger<GetMisPropuestasQueryHandler>` | Para logging de errores |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Flujo del Handle:**

```
1. Obtener propuestas paginadas del service:
   (items, totalCount) = await _service.GetByUserIdPaginatedAsync(
       request.UserId,
       request.EstadoPropuestaId,
       request.Page,
       Math.Min(request.PageSize, 50),
       ct)
   // El service delega al repository con Include de Necesidad, Artista y Acuerdos

2. Mapear entidades a DTOs:
   dtos = _mapper.Map<List<MiPropuestaListDto>>(items)

3. Para cada item + dto en zip(items, dtos):
   - dto.NecesidadTitulo  = item.Necesidad?.Titulo ?? string.Empty
   - dto.ArtistaNombre    = (resuelto via Necesidad navigation -> ArtistaId -> IArtistaService)
     // NOTA: El repository hace Include(p => p.Necesidad) pero no Include(artista del necesidad)
     // porque Artista esta en UserAccessContext. Se resuelve via IArtistaService con cache.
     artista = await _artistaService.GetByIdAsync(item.Necesidad.ArtistaId, ct)  // (necesita IArtistaService)
     dto.ArtistaNombre = artista?.NombreArtistico ?? string.Empty
   - dto.MonedaNombre     = (desde navigation prop de MaestraMoneda si esta incluida, o hardcoded "EUR"/"USD" segun MonedaId)
   - dto.EstadoPropuestaNombre = (desde MaestraEstadoPropuesta - puede ser via constantes o navigation prop)
   - dto.AcuerdoId        = item.Acuerdos.FirstOrDefault()?.Id.Value   // null si no hay acuerdo

4. Construir PaginatedResponse:
   paginatedResponse = new PaginatedResponse<MiPropuestaListDto>
   {
       Items = dtos,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize
   }

5. Retornar:
   ServiceResponse<PaginatedResponse<MiPropuestaListDto>>
   {
       Data = paginatedResponse,
       Messages = [{ Message = "Propuestas obtenidas exitosamente", HttpStatusCode = OK }]
   }

6. Catch:
   _logger.LogError(ex, "Error getting propuestas for user {UserId}", request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<MiPropuestaListDto>>(...)
```

**Notas de implementacion:**

- Se requiere `IArtistaService` como dependencia adicional para resolver `ArtistaNombre` (cross-module, igual que en `GetNecesidadesPublicasQuery`).
- Si el repository ya hace Include de la entidad `Artista` via un mecanismo cross-context, se puede omitir la llamada a `IArtistaService`. Dado que la tabla `Artista` esta en `UserAccessContext` y no en `CrowdsourcingContext`, la opcion segura es usar `IArtistaService` con cache.
- `EstadoPropuestaNombre` se puede mapear desde un diccionario local en el Handler usando `EstadoPropuestaConstants` (ej: `EstadoPropuestaConstants.Pendiente -> "Pendiente"`) para evitar un query adicional a maestras.

---

## 4. Commands

### 4.1 CreatePropuestaCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Commands/CreatePropuestaCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo

**Requiere Validator:** `CreatePropuestaCommandValidator` (ver seccion 5.1)

#### Command

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `NecesidadId` | `Guid` | Si | Route param `necesidadId` | Necesidad a la que se propone |
| `UserId` | `string` | Si | JWT claim `sub` | Asignado automaticamente desde token |
| `PrecioPropuesto` | `decimal` | Si | Request body | Debe ser > 0 |
| `MonedaId` | `int` | Si | Request body | FK a MaestraMoneda |
| `DiasEstimados` | `int?` | No | Request body | Opcional; si presente, debe ser > 0 y <= 365 |
| `MensajePropuesta` | `string` | Si | Request body | Min 20 chars, max 2000 chars |

**Implementa:** `IRequest<ServiceResponse<PropuestaCreatedResultDto>>`

**Nota sobre PerfilProfesionalId:** No se incluye en el Command. El Handler lo resuelve via `IPerfilProfesionalService.GetByUserIdAsync(UserId)` antes de crear la entidad. Si no existe PerfilProfesional, el Validator ya habrá fallado antes (ver seccion 5.1), pero el handler tambien verifica como segunda linea de defensa.

#### Handler

**Dependencias del constructor:**

| Dependencia | Tipo | Patron |
|-------------|------|--------|
| `_service` | `IPropuestaCrowdsourcingService` | Para persistir la propuesta |
| `_necesidadService` | `INecesidadCrowdsourcingService` | Para obtener NecesidadTitulo (via cache del validator) |
| `_perfilService` | `IPerfilProfesionalService` | Para obtener PerfilProfesionalId del usuario (via cache del validator) |
| `_mapper` | `IMapper` | Para mapear Command -> Entidad |
| `_validator` | `IValidator<CreatePropuestaCommand>` | Para validacion |
| `_logger` | `ILogger<CreatePropuestaCommandHandler>` | Para logging de errores |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Flujo del Handle:**

```
1. Validar request:
   validationResult = await _validator.ValidateAsync(request, ct)
   if (!validationResult.IsValid):
       return new ServiceResponse<PropuestaCreatedResultDto>
       {
           Messages = validationResult.GetServiceResponseMessages()
       }

2. Obtener PerfilProfesional del usuario (del cache del validator):
   perfil = await _perfilService.GetByUserIdAsync(request.UserId, ct)
   // Si el validator ya verifico ExistsByUserIdAsync, este GetByUserIdAsync sera un cache HIT
   if (perfil == null):
       // Segunda linea de defensa (el validator ya debe haberlo capturado)
       return new ServiceResponse<PropuestaCreatedResultDto>
       {
           Messages = [new() { Message = "Debes crear un perfil profesional para enviar propuestas",
                               ErrorCode = ServiceResponseMessageType.BusinessRule_NoProfessionalProfile }]
       }

3. Mapear Command a entidad:
   entity = _mapper.Map<PropuestaCrowdsourcing>(request)
   // AutoMapper solo mapea PrecioPropuesto, MonedaId, DiasEstimados, MensajePropuesta

4. Asignar campos que NO mapea AutoMapper (logica de negocio en el Handler):
   entity.Id               = PropuestaCrowdsourcingId.CreateNew()
   entity.NecesidadId      = new NecesidadCrowdsourcingId(request.NecesidadId)
   entity.UserId           = request.UserId
   entity.PerfilProfesionalId = perfil.Id   // PerfilProfesionalId obtenido del service
   entity.EstadoPropuestaId   = EstadoPropuestaConstants.Pendiente   // 1
   entity.FechaCreacion       = DateTime.UtcNow
   entity.FechaActualizacion  = null
   entity.MotivoRechazo       = null

5. Persistir via service:
   id = await _service.CreateAsync(entity, ct)
   // El service hace SaveChanges via repository

6. Obtener NecesidadTitulo para el DTO (del cache del validator):
   necesidad = await _necesidadService.GetPublicaByIdAsync(
       new NecesidadCrowdsourcingId(request.NecesidadId), ct)
   // Cache HIT "necesidad:publica:{id}" si el validator ya lo cargo

7. Construir DTO de resultado:
   resultDto = new PropuestaCreatedResultDto
   {
       Id                   = id.Value,
       NecesidadTitulo      = necesidad?.Titulo ?? string.Empty,
       PrecioPropuesto      = entity.PrecioPropuesto,
       EstadoPropuestaNombre = "Pendiente",
       FechaCreacion        = entity.FechaCreacion
   }

8. Retornar exito:
   ServiceResponse<PropuestaCreatedResultDto>
   {
       Data = resultDto,
       Messages = [new() {
           Message = "Propuesta enviada correctamente. El artista sera notificado.",
           HttpStatusCode = HttpStatusCode.Created
       }]
   }
   // USAR ServiceResponseMessageType.Created ("0001") via HttpStatusCode.Created

9. Catch:
   _logger.LogError(ex, "Error creating propuesta for necesidad {NecesidadId} by user {UserId}",
       request.NecesidadId, request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse<PropuestaCreatedResultDto>(
       "Error inesperado al enviar propuesta",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

**Notas de implementacion:**

- `EstadoPropuestaConstants.Pendiente` (= 1) en lugar del magic number `1`.
- El campo `PropuestaCreatedResultDto.EstadoPropuestaNombre` se asigna directamente con `"Pendiente"` (valor conocido al crear) en lugar de hacer un query a maestras.
- `entity.PerfilProfesionalId` es un `PerfilProfesionalId` (strongly-typed ID). Asignar correctamente: `perfil.Id` ya es de tipo `PerfilProfesionalId`.

---

### 4.2 RetirarPropuestaCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Commands/RetirarPropuestaCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo

**Requiere Validator:** `RetirarPropuestaCommandValidator` (ver seccion 5.2)

#### Command

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `Id` | `Guid` | Si | Route param | ID de la propuesta a retirar |
| `UserId` | `string` | Si | JWT claim `sub` | Para validar ownership en el handler |

**Implementa:** `IRequest<ServiceResponse<RetirarPropuestaResultDto>>`

**Nota:** No tiene Request Body. El controller solo toma `id` del route y `UserId` del JWT.

#### Handler

**Dependencias del constructor:**

| Dependencia | Tipo | Patron |
|-------------|------|--------|
| `_service` | `IPropuestaCrowdsourcingService` | Para obtener propuesta (cache) y ejecutar retiro |
| `_validator` | `IValidator<RetirarPropuestaCommand>` | Para validar existencia y estado |
| `_logger` | `ILogger<RetirarPropuestaCommandHandler>` | Para logging de errores y warning de ownership |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Flujo del Handle:**

```
1. Validar request (existencia + estado Pendiente):
   validationResult = await _validator.ValidateAsync(request, ct)
   if (!validationResult.IsValid):
       return new ServiceResponse<RetirarPropuestaResultDto>
       {
           Messages = validationResult.GetServiceResponseMessages()
       }

2. Obtener propuesta del cache (validator ya la cargo):
   propuestaId = new PropuestaCrowdsourcingId(request.Id)
   propuesta   = await _service.GetByIdAsync(propuestaId, ct)
   // Cache HIT "propuesta:{id.Value}" si el validator ya lo cargo
   if (propuesta == null):
       // Proteccion defensiva (el validator ya habrá fallado)
       return ValidateExtensions.NotFoundServiceResponse<RetirarPropuestaResultDto>(
           "Propuesta no encontrada",
           ServiceResponseMessageType.NotFound_Propuesta)

3. Verificar ownership (logica en Handler, retorna 403):
   if (propuesta.UserId != request.UserId):
       _logger.LogWarning(
           "Unauthorized attempt to retirar propuesta {PropuestaId} by user {UserId}. Owner is {OwnerId}",
           request.Id, request.UserId, propuesta.UserId)
       return ValidateExtensions.ForbiddenServiceResponse<RetirarPropuestaResultDto>(
           "No tienes permiso para retirar esta propuesta",
           ServiceResponseMessageType.Auth_Forbidden)
   // Auth_Forbidden = "3002"

4. Ejecutar retiro via service:
   retirado = await _service.RetirarAsync(propuestaId, ct)
   // El service cambia EstadoPropuestaId = EstadoPropuestaConstants.Retirada (4)
   // y FechaActualizacion = DateTime.UtcNow
   // Hace SaveChanges via repository

5. Construir DTO de resultado:
   resultDto = new RetirarPropuestaResultDto
   {
       Id                   = request.Id,
       EstadoPropuestaNombre = "Retirada"
   }

6. Retornar exito:
   ServiceResponse<RetirarPropuestaResultDto>
   {
       Data = resultDto,
       Messages = [new() {
           Message = "Propuesta retirada correctamente",
           HttpStatusCode = HttpStatusCode.OK
       }]
   }
   // USAR ServiceResponseMessageType.Updated ("0002") via HttpStatusCode

7. Catch:
   _logger.LogError(ex, "Error retracting propuesta {PropuestaId} by user {UserId}",
       request.Id, request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse<RetirarPropuestaResultDto>(
       "Error inesperado al retirar propuesta",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

**Notas de implementacion:**

- La verificacion de ownership (paso 3) se hace en el Handler (NO en el Validator) para retornar HTTP 403 en el controller, siguiendo el patron de `GetNecesidadByIdQueryHandler` que tambien verifica ownership en el Handler.
- El Validator verifica existencia y estado (400/404 segun el caso). El Handler verifica ownership (403).
- `EstadoPropuestaConstants.Retirada` (= 4) en lugar del magic number `4`.

---

## 5. Validators

### 5.1 CreatePropuestaCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Validators/CreatePropuestaCommandValidator.cs`

**Dependencias del constructor:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `_propuestaService` | `IPropuestaCrowdsourcingService` | Verificar unicidad (no propuesta activa previa para NecesidadId + UserId) |
| `_necesidadService` | `INecesidadCrowdsourcingService` | Verificar que necesidad existe, esta Abierta y no expirada |
| `_perfilService` | `IPerfilProfesionalService` | Verificar que usuario tiene PerfilProfesional activo |
| `_artistaService` | `IArtistaService` | Verificar que usuario no es el artista propietario de la necesidad |

**Constructor:** Todos con `?? throw new ArgumentNullException(nameof(X))`

**Nota de caching (ADR-006):** Todas las llamadas a services en el validator usan `IRequestCacheService` dentro de los services. Cuando el Handler llame a los mismos metodos, seran Cache HITs sin queries adicionales a BD.

**Reglas de validacion (en orden de ejecucion recomendado):**

#### Validaciones de campo simples (sincronas, primero):

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `PrecioPropuesto` | `.GreaterThan(0)` | "El precio propuesto debe ser mayor a 0" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| `MonedaId` | `.NotEmpty()` | "La moneda es obligatoria" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `MonedaId` | `.GreaterThan(0)` | "La moneda no es valida" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| `DiasEstimados` | `.GreaterThan(0).When(x => x.DiasEstimados.HasValue)` | "Los dias estimados deben ser mayor a 0" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| `DiasEstimados` | `.LessThanOrEqualTo(365).When(x => x.DiasEstimados.HasValue)` | "El tiempo estimado no puede superar los 365 dias" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| `MensajePropuesta` | `.NotEmpty()` | "El mensaje de propuesta es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `MensajePropuesta` | `.MinimumLength(20)` | "El mensaje debe tener al menos 20 caracteres" | `ServiceResponseMessageType.Validation_MinLength` ("1011") |
| `MensajePropuesta` | `.MaximumLength(2000)` | "El mensaje no puede superar los 2000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |

#### Validaciones asincronas (MustAsync, en orden):

| Objetivo | Regla MustAsync | Mensaje | ErrorCode (Constante) |
|----------|-----------------|---------|----------------------|
| Necesidad existe y abierta | `RuleFor(x => x.NecesidadId).MustAsync(...)` verifica: necesidad != null, `EstadoNecesidadId == 1`, `FechaLimitePropuestas IS NULL OR >= DateTime.Today` | "La necesidad no existe, no esta abierta o ha expirado" | `ServiceResponseMessageType.NotFound_Necesidad` ("2009") |
| Usuario tiene PerfilProfesional | `RuleFor(x => x).MustAsync(...)` llama `_perfilService.ExistsByUserIdAsync(command.UserId, ct)` | "Debes crear un perfil profesional para enviar propuestas" | `ServiceResponseMessageType.BusinessRule_NoProfessionalProfile` ("4006") |
| Usuario no es propietario | `RuleFor(x => x).MustAsync(...)` obtiene `necesidad` del cache, obtiene `artista` del `_artistaService.GetByUserIdAsync(UserId)`, verifica `artista == null OR necesidad.ArtistaId != artista.Id` | "No puedes enviar propuesta a tu propia necesidad" | `ServiceResponseMessageType.BusinessRule_CannotProposeSelf` ("4004") |
| No propuesta activa previa | `RuleFor(x => x).MustAsync(...)` llama `_propuestaService.ExistePropuestaAsync(NecesidadId, UserId, ct)` y retorna `!existe` | "Ya tienes una propuesta enviada para esta necesidad" | `ServiceResponseMessageType.BusinessRule_AlreadyProposed` ("4003") |

**Pseudo-codigo del validator:**

```
public CreatePropuestaCommandValidator(
    IPropuestaCrowdsourcingService propuestaService,
    INecesidadCrowdsourcingService necesidadService,
    IPerfilProfesionalService perfilService,
    IArtistaService artistaService)
{
    // Con ?? throw en todas las dependencias

    // --- Validaciones de campo (sincronas) ---
    RuleFor(x => x.PrecioPropuesto)
        .GreaterThan(0)
        .WithMessage("El precio propuesto debe ser mayor a 0")
        .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

    RuleFor(x => x.MonedaId)
        .NotEmpty()
        .WithMessage("La moneda es obligatoria")
        .WithErrorCode(ServiceResponseMessageType.Validation_Required)
        .GreaterThan(0)
        .WithMessage("La moneda no es valida")
        .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

    RuleFor(x => x.DiasEstimados)
        .GreaterThan(0)
        .When(x => x.DiasEstimados.HasValue)
        .WithMessage("Los dias estimados deben ser mayor a 0")
        .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

    RuleFor(x => x.DiasEstimados)
        .LessThanOrEqualTo(365)
        .When(x => x.DiasEstimados.HasValue)
        .WithMessage("El tiempo estimado no puede superar los 365 dias")
        .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

    RuleFor(x => x.MensajePropuesta)
        .NotEmpty()
        .WithMessage("El mensaje de propuesta es obligatorio")
        .WithErrorCode(ServiceResponseMessageType.Validation_Required)
        .MinimumLength(20)
        .WithMessage("El mensaje debe tener al menos 20 caracteres")
        .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
        .MaximumLength(2000)
        .WithMessage("El mensaje no puede superar los 2000 caracteres")
        .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

    // --- Validaciones asincronas ---

    // 1. Necesidad existe, Abierta y no expirada
    RuleFor(x => x.NecesidadId)
        .MustAsync(async (necesidadId, ct) =>
        {
            var necesidad = await _necesidadService.GetPublicaByIdAsync(
                new NecesidadCrowdsourcingId(necesidadId), ct);
            return necesidad != null;
            // GetPublicaByIdAsync ya filtra EstadoNecesidadId == 1 y no expiradas
        })
        .WithMessage("La necesidad no existe, no esta abierta o ha expirado")
        .WithErrorCode(ServiceResponseMessageType.NotFound_Necesidad);

    // 2. Usuario tiene PerfilProfesional activo
    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            return await _perfilService.ExistsByUserIdAsync(command.UserId, ct);
        })
        .WithMessage("Debes crear un perfil profesional para enviar propuestas")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_NoProfessionalProfile);

    // 3. Usuario NO es el artista propietario de la necesidad
    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            var necesidad = await _necesidadService.GetPublicaByIdAsync(
                new NecesidadCrowdsourcingId(command.NecesidadId), ct);
            // Cache HIT: mismo resultado que la validacion 1
            if (necesidad == null) return true;  // ya fallara en validacion 1

            var artista = await _artistaService.GetByUserIdAsync(command.UserId, ct);
            return artista == null || necesidad.ArtistaId != artista.Id;
        })
        .WithMessage("No puedes enviar propuesta a tu propia necesidad")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_CannotProposeSelf);

    // 4. No existe propuesta activa previa (unicidad)
    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            var yaPropuso = await _propuestaService.ExistePropuestaAsync(
                new NecesidadCrowdsourcingId(command.NecesidadId),
                command.UserId,
                ct);
            return !yaPropuso;
        })
        .WithMessage("Ya tienes una propuesta enviada para esta necesidad")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_AlreadyProposed);
}
```

---

### 5.2 RetirarPropuestaCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Validators/RetirarPropuestaCommandValidator.cs`

**Dependencias del constructor:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `_propuestaService` | `IPropuestaCrowdsourcingService` | Verificar existencia y estado de la propuesta |

**Constructor:** Con `?? throw new ArgumentNullException(nameof(propuestaService))`

**Nota de separacion de responsabilidades:**
- El Validator verifica **existencia** (retorna 404) y **estado Pendiente** (retorna 400).
- El Handler verifica **ownership** (retorna 403), siguiendo el patron establecido en `GetNecesidadByIdQueryHandler`.

**Reglas de validacion:**

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `Id` | `.NotEmpty()` | "El ID de la propuesta es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `command` (nivel objeto) | `MustAsync`: propuesta existe en BD | "La propuesta no fue encontrada" | `ServiceResponseMessageType.NotFound_Propuesta` ("2010") |
| `command` (nivel objeto) | `MustAsync`: propuesta tiene `EstadoPropuestaId == EstadoPropuestaConstants.Pendiente` | "Solo se pueden retirar propuestas en estado Pendiente" | `ServiceResponseMessageType.BusinessRule_PropuestaNotRetirable` ("4005") |

**Pseudo-codigo del validator:**

```
public RetirarPropuestaCommandValidator(IPropuestaCrowdsourcingService propuestaService)
{
    _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));

    RuleFor(x => x.Id)
        .NotEmpty()
        .WithMessage("El ID de la propuesta es obligatorio")
        .WithErrorCode(ServiceResponseMessageType.Validation_Required);

    // Verificar existencia
    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            var propuesta = await _propuestaService.GetByIdAsync(
                new PropuestaCrowdsourcingId(command.Id), ct);
            return propuesta != null;
        })
        .WithMessage("La propuesta no fue encontrada")
        .WithErrorCode(ServiceResponseMessageType.NotFound_Propuesta);

    // Verificar estado Pendiente (solo si existe)
    RuleFor(x => x)
        .MustAsync(async (command, ct) =>
        {
            var propuesta = await _propuestaService.GetByIdAsync(
                new PropuestaCrowdsourcingId(command.Id), ct);
            // Cache HIT: mismo objeto que la validacion anterior
            return propuesta == null
                || propuesta.EstadoPropuestaId == EstadoPropuestaConstants.Pendiente;
        })
        .WithMessage("Solo se pueden retirar propuestas en estado Pendiente")
        .WithErrorCode(ServiceResponseMessageType.BusinessRule_PropuestaNotRetirable);
}
```

**Nota sobre implementacion alternativa:** Las dos validaciones asincronas pueden combinarse en un solo `MustAsync` que determina el mensaje segun si la propuesta no existe o si no esta en estado Pendiente. Sin embargo, FluentValidation no soporta mensajes condicionales en un mismo `MustAsync` de forma idiomatica. La version con dos reglas separadas es mas clara y el cache garantiza que `GetByIdAsync` solo va a BD una vez.

---

## 6. DTOs a Crear

**Ubicacion:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/`

| Archivo | Estado | Descripcion |
|---------|--------|-------------|
| `NecesidadPublicaListDto.cs` | CREAR | Item del listado publico paginado |
| `NecesidadPublicaDto.cs` | CREAR | Detalle completo con campos calculados |
| `ArtistaPublicoDto.cs` | CREAR | Sub-DTO embebido en NecesidadPublicaDto |
| `PropuestaCreatedResultDto.cs` | CREAR | Respuesta al crear propuesta (POST 201) |
| `MiPropuestaListDto.cs` | CREAR | Item del listado "Mis propuestas" |
| `RetirarPropuestaResultDto.cs` | CREAR | Respuesta al retirar propuesta (PATCH 200) |

### Propiedades de cada DTO

#### NecesidadPublicaListDto

| Propiedad | Tipo | Asignado por |
|-----------|------|--------------|
| `Id` | `Guid` | AutoMapper (`Id.Value`) |
| `Titulo` | `string` | AutoMapper |
| `Descripcion` | `string?` | AutoMapper (truncado a 150 por el repository) |
| `TipoNecesidadId` | `int` | AutoMapper |
| `TipoNecesidadNombre` | `string` | Handler (navigation prop o service de maestras) |
| `PresupuestoMin` | `decimal?` | AutoMapper |
| `PresupuestoMax` | `decimal?` | AutoMapper |
| `MonedaId` | `int?` | AutoMapper |
| `MonedaNombre` | `string?` | Handler (navigation prop) |
| `ModalidadTrabajoId` | `int` | AutoMapper |
| `ModalidadTrabajoNombre` | `string` | Handler (navigation prop) |
| `UbicacionCiudad` | `string?` | AutoMapper |
| `UbicacionPais` | `string?` | AutoMapper |
| `ArtistaNombre` | `string` | Handler (via `IArtistaService.GetByIdAsync`) |
| `FechaCreacion` | `DateTime` | AutoMapper |
| `FechaLimitePropuestas` | `DateTime?` | AutoMapper |
| `EsUrgente` | `bool` | Handler (calculado: `FechaLimitePropuestas <= UtcNow.AddDays(3)`) |
| `NumeroPropuestas` | `int` | Handler (COUNT propuestas no-Retiradas) |

#### NecesidadPublicaDto

Incluye todos los campos de `NecesidadPublicaListDto` mas:

| Propiedad adicional | Tipo | Asignado por |
|---------------------|------|--------------|
| `EstadoNecesidadId` | `int` | AutoMapper |
| `EstadoNecesidadNombre` | `string` | Handler (siempre `"Abierta"`) |
| `FechaInicioPrevista` | `DateTime?` | AutoMapper |
| `Artista` | `ArtistaPublicoDto` | Handler (via `IArtistaService`) |
| `YaPropuso` | `bool` | Handler (via `IPropuestaCrowdsourcingService.ExistePropuestaAsync`) |
| `EsPropietario` | `bool` | Handler (comparar `ArtistaId` de necesidad con artista del UserId) |
| `TienePerfilProfesional` | `bool` | Handler (via `IPerfilProfesionalService.ExistsByUserIdAsync`) |

*Nota: `Descripcion` en este DTO NO esta truncada (es el texto completo de la necesidad)*

#### ArtistaPublicoDto

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del artista |
| `NombreArtistico` | `string` | Nombre artistico publico |
| `ImagenUrl` | `string?` | URL de imagen; null si no tiene |

#### PropuestaCreatedResultDto

| Propiedad | Tipo | Asignado por |
|-----------|------|--------------|
| `Id` | `Guid` | Handler (del `PropuestaCrowdsourcingId.Value`) |
| `NecesidadTitulo` | `string` | Handler (de `INecesidadCrowdsourcingService.GetPublicaByIdAsync`) |
| `PrecioPropuesto` | `decimal` | AutoMapper |
| `EstadoPropuestaNombre` | `string` | Handler (siempre `"Pendiente"` al crear) |
| `FechaCreacion` | `DateTime` | Handler (de `entity.FechaCreacion`) |

#### MiPropuestaListDto

| Propiedad | Tipo | Asignado por |
|-----------|------|--------------|
| `Id` | `Guid` | AutoMapper (`Id.Value`) |
| `NecesidadTitulo` | `string` | Handler (`entity.Necesidad.Titulo`) |
| `ArtistaNombre` | `string` | Handler (via `IArtistaService.GetByIdAsync`) |
| `PrecioPropuesto` | `decimal` | AutoMapper |
| `MonedaId` | `int` | AutoMapper |
| `MonedaNombre` | `string` | Handler (navigation prop o diccionario local) |
| `EstadoPropuestaId` | `int` | AutoMapper |
| `EstadoPropuestaNombre` | `string` | Handler (diccionario `EstadoPropuestaConstants` -> string) |
| `FechaCreacion` | `DateTime` | AutoMapper |
| `FechaActualizacion` | `DateTime?` | AutoMapper |
| `AcuerdoId` | `Guid?` | Handler (`entity.Acuerdos.FirstOrDefault()?.Id.Value`) |

#### RetirarPropuestaResultDto

| Propiedad | Tipo | Asignado por |
|-----------|------|--------------|
| `Id` | `Guid` | Handler (de `request.Id`) |
| `EstadoPropuestaNombre` | `string` | Handler (siempre `"Retirada"`) |

---

## 7. AutoMapper Profiles

### 7.1 NecesidadCrowdsourcingProfile (AMPLIAR)

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/NecesidadCrowdsourcingProfile.cs`

Agregar dentro del constructor del profile existente:

```
// Entity -> NecesidadPublicaListDto
CreateMap<NecesidadCrowdsourcing, NecesidadPublicaListDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())    // Handler lo asigna
    .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore()) // Handler lo asigna
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())           // Handler lo asigna
    .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())          // Handler via IArtistaService
    .ForMember(dest => dest.EsUrgente, opt => opt.Ignore())              // Handler calcula logica de negocio
    .ForMember(dest => dest.NumeroPropuestas, opt => opt.Ignore());      // Handler COUNT propuestas activas

// Entity -> NecesidadPublicaDto
CreateMap<NecesidadCrowdsourcing, NecesidadPublicaDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore())    // Handler lo asigna
    .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore())  // Handler: siempre "Abierta"
    .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore()) // Handler lo asigna
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())           // Handler lo asigna
    .ForMember(dest => dest.NumeroPropuestas, opt => opt.Ignore())       // Handler COUNT propuestas activas
    .ForMember(dest => dest.Artista, opt => opt.Ignore())                // Handler via IArtistaService
    .ForMember(dest => dest.YaPropuso, opt => opt.Ignore())              // Handler campo calculado
    .ForMember(dest => dest.EsPropietario, opt => opt.Ignore())          // Handler campo calculado
    .ForMember(dest => dest.TienePerfilProfesional, opt => opt.Ignore()); // Handler campo calculado
```

### 7.2 PropuestaCrowdsourcingProfile (AMPLIAR)

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/PropuestaCrowdsourcingProfile.cs`

Agregar dentro del constructor del profile existente (conservar el mapping `PropuestaCrowdsourcing -> PropuestaCrowdsourcingDto` existente):

```
// Command -> Entity (para crear propuesta)
CreateMap<CreatePropuestaCommand, PropuestaCrowdsourcing>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())                    // Handler: PropuestaCrowdsourcingId.CreateNew()
    .ForMember(dest => dest.NecesidadId, opt => opt.Ignore())           // Handler: new NecesidadCrowdsourcingId(request.NecesidadId)
    .ForMember(dest => dest.UserId, opt => opt.Ignore())                // Handler: desde request.UserId
    .ForMember(dest => dest.PerfilProfesionalId, opt => opt.Ignore())   // Handler: desde IPerfilProfesionalService
    .ForMember(dest => dest.EstadoPropuestaId, opt => opt.Ignore())     // Handler: EstadoPropuestaConstants.Pendiente
    .ForMember(dest => dest.MotivoRechazo, opt => opt.Ignore())         // Handler: null al crear
    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())         // Handler: DateTime.UtcNow
    .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())    // Handler: null al crear
    .ForMember(dest => dest.Necesidad, opt => opt.Ignore())             // Navigation property
    .ForMember(dest => dest.Acuerdos, opt => opt.Ignore());             // Navigation property

// Entity -> PropuestaCreatedResultDto
CreateMap<PropuestaCrowdsourcing, PropuestaCreatedResultDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.NecesidadTitulo, opt => opt.Ignore())       // Handler: desde necesidad cargada
    .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore()); // Handler: siempre "Pendiente"

// Entity -> MiPropuestaListDto
CreateMap<PropuestaCrowdsourcing, MiPropuestaListDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.NecesidadTitulo, opt => opt.Ignore())        // Handler: entity.Necesidad.Titulo
    .ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())          // Handler: via IArtistaService
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())           // Handler: navigation prop o lookup
    .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore())  // Handler: diccionario de constantes
    .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore());             // Handler: entity.Acuerdos.FirstOrDefault()?.Id.Value

// Entity -> RetirarPropuestaResultDto
CreateMap<PropuestaCrowdsourcing, RetirarPropuestaResultDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore()); // Handler: siempre "Retirada"
```

---

## 8. Estructura de Archivos a Crear

```
src/api/Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       ├── ServiceResponseMessageType.cs              MODIFICAR - agregar 5 constantes
│       └── EstadoPropuestaConstants.cs                CREAR
│
└── WePlayRises.Crowdsourcing.Application/
    ├── Dtos/
    │   ├── NecesidadPublicaListDto.cs                 CREAR
    │   ├── NecesidadPublicaDto.cs                     CREAR
    │   ├── ArtistaPublicoDto.cs                       CREAR
    │   ├── PropuestaCreatedResultDto.cs               CREAR
    │   ├── MiPropuestaListDto.cs                      CREAR
    │   └── RetirarPropuestaResultDto.cs               CREAR
    │
    ├── Features/
    │   └── Propuestas/
    │       ├── Commands/
    │       │   ├── CreatePropuestaCommand.cs          CREAR (Command + Handler en mismo archivo)
    │       │   └── RetirarPropuestaCommand.cs         CREAR (Command + Handler en mismo archivo)
    │       ├── Queries/
    │       │   ├── GetNecesidadesPublicasQuery.cs     CREAR (Query + Handler en mismo archivo)
    │       │   ├── GetNecesidadPublicaByIdQuery.cs    CREAR (Query + Handler en mismo archivo)
    │       │   └── GetMisPropuestasQuery.cs           CREAR (Query + Handler en mismo archivo)
    │       └── Validators/
    │           ├── CreatePropuestaCommandValidator.cs CREAR
    │           └── RetirarPropuestaCommandValidator.cs CREAR
    │
    └── Mapping/
        ├── NecesidadCrowdsourcingProfile.cs           MODIFICAR - agregar 2 mappings
        └── PropuestaCrowdsourcingProfile.cs            MODIFICAR - agregar 4 mappings
```

---

## 9. Estrategia de Caching (IRequestCacheService)

Siguiendo ADR-006, el cache request-scoped evita queries duplicados entre Validator y Handler dentro de la misma request HTTP.

### Flujo de cache en CreatePropustaCommand (la request mas compleja)

```
Request HTTP: POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas
│
├── CreatePropuestaCommandValidator.ValidateAsync()
│   ├── Regla 1 (necesidadId):
│   │   -> INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId)
│   │      -> _requestCache.GetOrAddAsync("necesidad:publica:{id}")   [Cache MISS -> DB]
│   │
│   ├── Regla 2 (UserId tiene perfil):
│   │   -> IPerfilProfesionalService.ExistsByUserIdAsync(userId)
│   │      -> _requestCache.GetOrAddAsync("perfilprofesional:exists:user:{userId}")  [Cache MISS -> DB]
│   │
│   ├── Regla 3 (no es propietario):
│   │   -> INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId)
│   │      -> _requestCache.GetOrAddAsync("necesidad:publica:{id}")   [Cache HIT - sin DB]
│   │   -> IArtistaService.GetByUserIdAsync(userId)                    [Cache MISS o HIT]
│   │
│   └── Regla 4 (unicidad):
│       -> IPropuestaCrowdsourcingService.ExistePropuestaAsync(necesidadId, userId)
│          -> _requestCache.GetOrAddAsync("propuesta:existe:{necesidadId}:{userId}")  [Cache MISS -> DB]
│
└── CreatePropuestaCommandHandler.Handle()
    ├── IPerfilProfesionalService.GetByUserIdAsync(userId)
    │   -> _requestCache.GetOrAddAsync("perfilprofesional:user:{userId}")  [Cache MISS -> DB]
    │      (diferente key a "exists:user:{userId}", pero si ya se cargo el objeto se puede reusar)
    │
    └── INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId)
        -> _requestCache.GetOrAddAsync("necesidad:publica:{id}")   [Cache HIT - sin DB]
```

### Claves de cache definidas en Services

| Cache Key | Metodo | Modulo |
|-----------|--------|--------|
| `"necesidad:publica:{id.Value}"` | `INecesidadCrowdsourcingService.GetPublicaByIdAsync` | Crowdsourcing |
| `"propuesta:{id.Value}"` | `IPropuestaCrowdsourcingService.GetByIdAsync` | Crowdsourcing |
| `"propuesta:existe:{necesidadId.Value}:{userId}"` | `IPropuestaCrowdsourcingService.ExistePropuestaAsync` | Crowdsourcing |
| `"perfilprofesional:user:{userId}"` | `IPerfilProfesionalService.GetByUserIdAsync` | UserAccess |
| `"perfilprofesional:exists:user:{userId}"` | `IPerfilProfesionalService.ExistsByUserIdAsync` | UserAccess |

---

## 10. Flujo de Datos Completo por Operacion

### GET /api/crowdsourcing/necesidades

```
NecesidadesCrowdsourcingController.GetPublicas(query params)
  -> userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
  -> GetNecesidadesPublicasQuery { UserId, filtros, page, pageSize }
  -> GetNecesidadesPublicasQueryHandler.Handle(query, ct)
     -> IArtistaService.GetByUserIdAsync(userId) -> artista? (para excluir propias)
     -> INecesidadCrowdsourcingService.GetPublicasPaginatedAsync(filtro, artista?.Id, ct)
        -> INecesidadCrowdsourcingRepository.GetPublicasPaginatedAsync(filtro, artistaId?, ct)
           -> EF: WHERE EstadoNecesidadId = 1
                  AND (FechaLimitePropuestas IS NULL OR FechaLimitePropuestas > UtcNow)
                  AND (artistaId IS NULL OR ArtistaId != artistaId)
                  + filtros opcionales + ORDER BY + SKIP/TAKE
                  AsNoTracking()
     -> _mapper.Map<List<NecesidadPublicaListDto>>(items)
     -> Para cada item: resolver ArtistaNombre via IArtistaService (con cache), calcular EsUrgente, NumeroPropuestas
     -> PaginatedResponse<NecesidadPublicaListDto>
     -> ServiceResponse { Data = paginatedResponse }
  -> Controller: Ok(response)
```

### GET /api/crowdsourcing/necesidades/{id} (vista profesional)

```
NecesidadesCrowdsourcingController.GetById(id)
  -> userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
  -> GetNecesidadPublicaByIdQuery { Id = id, UserId = userId }
  -> GetNecesidadPublicaByIdQueryHandler.Handle(query, ct)
     -> INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId, ct)   [cache]
        -> Si null: NotFound (2009)
     -> IPropuestaCrowdsourcingService.ExistePropuestaAsync(necesidadId, userId, ct)  [cache]
     -> IArtistaService.GetByUserIdAsync(userId, ct)  [cache]
     -> esPropietario = artista != null && entity.ArtistaId == artista.Id
     -> IPerfilProfesionalService.ExistsByUserIdAsync(userId, ct)  [cache]
     -> IArtistaService.GetByIdAsync(entity.ArtistaId, ct)  [cache]
     -> _mapper.Map<NecesidadPublicaDto>(entity)
     -> Asignar: YaPropuso, EsPropietario, TienePerfilProfesional, Artista, nombres de maestras
     -> ServiceResponse { Data = dto }
  -> Controller: Si EsPropietario -> invocar GetNecesidadByIdQuery (vista artista)
                 Si no -> Ok(response) con NecesidadPublicaDto
```

### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas

```
NecesidadesCrowdsourcingController.CreatePropuesta(necesidadId, body)
  -> userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
  -> CreatePropuestaCommand { NecesidadId, UserId, PrecioPropuesto, MonedaId, DiasEstimados, MensajePropuesta }
  -> CreatePropuestaCommandHandler.Handle(command, ct)
     -> _validator.ValidateAsync(command, ct)
        -> Validaciones de campo: precio, moneda, dias, mensaje
        -> MustAsync: necesidad existe y abierta (via cache)
        -> MustAsync: usuario tiene PerfilProfesional (via cache)
        -> MustAsync: usuario no es propietario (via cache)
        -> MustAsync: no propuesta activa previa (via cache)
        -> Si invalido: ServiceResponse con errores de validacion
     -> IPerfilProfesionalService.GetByUserIdAsync(userId, ct)  [cache HIT]
     -> _mapper.Map<PropuestaCrowdsourcing>(command)
     -> Asignar: Id, NecesidadId, UserId, PerfilProfesionalId, EstadoPropuestaId, FechaCreacion
     -> IPropuestaCrowdsourcingService.CreateAsync(entity, ct)
        -> IPropuestaCrowdsourcingRepository.AddAsync(entity, ct)
           -> EF: Add + SaveChangesAsync
     -> INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId, ct)  [cache HIT]
     -> PropuestaCreatedResultDto { Id, NecesidadTitulo, PrecioPropuesto, "Pendiente", FechaCreacion }
     -> ServiceResponse { Data = resultDto, Messages = [{ "0001" Created }] }
  -> Controller: 201 Created
```

### GET /api/crowdsourcing/propuestas/mis-propuestas

```
PropuestasCrowdsourcingController.GetMisPropuestas(estado, page, pageSize)
  -> userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
  -> GetMisPropuestasQuery { UserId, EstadoPropuestaId, Page, PageSize }
  -> GetMisPropuestasQueryHandler.Handle(query, ct)
     -> IPropuestaCrowdsourcingService.GetByUserIdPaginatedAsync(userId, estadoId, page, pageSize, ct)
        -> IPropuestaCrowdsourcingRepository.GetByUserIdPaginatedAsync(...)
           -> EF: WHERE UserId = userId
                  AND (EstadoPropuestaId = estadoId si presente)
                  Include(p => p.Necesidad)
                  Include(p => p.Acuerdos)
                  ORDER BY FechaCreacion DESC
                  AsNoTracking()
     -> _mapper.Map<List<MiPropuestaListDto>>(items)
     -> Para cada item: asignar NecesidadTitulo, ArtistaNombre (via IArtistaService), MonedaNombre, EstadoPropuestaNombre, AcuerdoId
     -> PaginatedResponse<MiPropuestaListDto>
     -> ServiceResponse { Data = paginatedResponse }
  -> Controller: Ok(response)
```

### PATCH /api/crowdsourcing/propuestas/{id}/retirar

```
PropuestasCrowdsourcingController.Retirar(id)
  -> userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
  -> RetirarPropuestaCommand { Id, UserId }
  -> RetirarPropuestaCommandHandler.Handle(command, ct)
     -> _validator.ValidateAsync(command, ct)
        -> RuleFor(Id).NotEmpty()
        -> MustAsync: propuesta existe (via cache "propuesta:{id}")
        -> MustAsync: propuesta.EstadoPropuestaId == Pendiente (via cache HIT)
        -> Si invalido: ServiceResponse con errores (404 o 400 segun ErrorCode)
     -> IPropuestaCrowdsourcingService.GetByIdAsync(propuestaId, ct)  [cache HIT]
     -> Si propuesta.UserId != request.UserId:
        -> LogWarning + return Forbidden (3002)
     -> IPropuestaCrowdsourcingService.RetirarAsync(propuestaId, ct)
        -> Cambia EstadoPropuestaId = 4 (Retirada)
        -> Actualiza FechaActualizacion = UtcNow
        -> IPropuestaCrowdsourcingRepository.UpdateAsync(entity, ct)
           -> EF: Update + SaveChangesAsync
     -> RetirarPropuestaResultDto { Id, EstadoPropuestaNombre = "Retirada" }
     -> ServiceResponse { Data = resultDto, Messages = [{ "0002" Updated }] }
  -> Controller: Ok(response)
```

---

## 11. Dependencias Cross-Module

Esta feature introduce la dependencia `Crowdsourcing.Application -> UserAccess`:

```
Crowdsourcing.Application.Features.Propuestas
  -> IArtistaService      (UserAccess.Application, ya usada en otros Handlers de Crowdsourcing)
  -> IPerfilProfesionalService  (UserAccess.Application, NUEVA - ver hexagonal-architecture.md)
```

**IPerfilProfesionalService** es nuevo y debe crearse en `UserAccess.Application/Interfaces/Services/`. Ver el plan de hexagonal-architecture.md para la implementacion en infraestructura.

---

## 12. Imports Necesarios en cada Archivo

### Commands y Queries

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;        // ValidateExtensions
using WePlayRises.BuildingBlocks.Kernel.Http.Response;    // ServiceResponse, PaginatedResponse
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;          // ServiceResponseMessageType, EstadoPropuestaConstants
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;  // IPerfilProfesionalService, IArtistaService
```

### Validators

```csharp
using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;          // ServiceResponseMessageType, EstadoPropuestaConstants
using WePlayRises.UserAccess.Application.Interfaces.Services;  // IPerfilProfesionalService, IArtistaService
```

---

## 13. Orden de Implementacion

El orden recomendado para minimizar dependencias de compilacion:

```
Fase 1 - Domain y Constants (sin dependencias)
  1. ServiceResponseMessageType.cs          MODIFICAR - agregar 5 constantes
  2. EstadoPropuestaConstants.cs            CREAR

Fase 2 - DTOs (solo dependen de tipos primitivos)
  3. ArtistaPublicoDto.cs                   CREAR
  4. NecesidadPublicaListDto.cs             CREAR
  5. NecesidadPublicaDto.cs                 CREAR (depende de ArtistaPublicoDto)
  6. PropuestaCreatedResultDto.cs           CREAR
  7. MiPropuestaListDto.cs                  CREAR
  8. RetirarPropuestaResultDto.cs           CREAR

Fase 3 - AutoMapper Profiles (dependen de DTOs y entidades)
  9. NecesidadCrowdsourcingProfile.cs       MODIFICAR - agregar 2 mappings
  10. PropuestaCrowdsourcingProfile.cs       MODIFICAR - agregar 4 mappings

Fase 4 - UserAccess (necesario para Validators y Handlers)
  11. IPerfilProfesionalService.cs          CREAR (UserAccess.Application)
  [Ver hexagonal-architecture.md para implementacion]

Fase 5 - Validators (dependen de Services e interfaces)
  12. CreatePropuestaCommandValidator.cs    CREAR
  13. RetirarPropuestaCommandValidator.cs   CREAR

Fase 6 - Commands y Queries (dependen de Validators y Services)
  14. GetNecesidadesPublicasQuery.cs        CREAR (sin validator)
  15. GetNecesidadPublicaByIdQuery.cs       CREAR (sin validator)
  16. GetMisPropuestasQuery.cs              CREAR (sin validator)
  17. CreatePropuestaCommand.cs             CREAR (usa CreatePropuestaCommandValidator)
  18. RetirarPropuestaCommand.cs            CREAR (usa RetirarPropuestaCommandValidator)
```

---

## 14. Patrones del Proyecto a Respetar

### 14.1 ServiceResponse con HttpStatusCode (NO ErrorCode directo)

Basado en los archivos existentes (`CreateNecesidadCommand.cs`, `GetNecesidadByIdQuery.cs`), el proyecto usa `HttpStatusCode` en lugar de `ErrorCode` directamente en los `ServiceResponseMessage` de exito:

```csharp
// CORRECTO para este proyecto (patron observado en codigo existente):
return new ServiceResponse<T>
{
    Data = resultDto,
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Propuesta enviada correctamente. El artista sera notificado.",
            HttpStatusCode = System.Net.HttpStatusCode.Created
        }
    }
};

// Para errores, usar ValidateExtensions (patron existente):
return ValidateExtensions.NotFoundServiceResponse<T>(
    "Necesidad no encontrada",
    ServiceResponseMessageType.NotFound_Necesidad);

return ValidateExtensions.ForbiddenServiceResponse<T>(
    "No tienes permiso",
    ServiceResponseMessageType.Auth_Forbidden);

return ValidateExtensions.InternalServerErrorServiceResponse<T>(
    "Error inesperado",
    ServiceResponseMessageType.Internal_UnexpectedError);
```

### 14.2 StronglyTypedIds

```csharp
// Crear nuevo ID:
entity.Id = PropuestaCrowdsourcingId.CreateNew();

// Convertir desde Guid:
entity.NecesidadId = new NecesidadCrowdsourcingId(request.NecesidadId);
entity.ArtistaId   = new ArtistaId(request.ArtistaId);

// Extraer Guid:
dto.Id = entity.Id.Value;
```

### 14.3 Namespace del proyecto

```csharp
namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;
namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Validators;
```

---

## 15. Checklist de Verificacion

### Commands y Queries
- [ ] `GetNecesidadesPublicasQuery` + Handler en MISMO archivo (sin validator)
- [ ] `GetNecesidadPublicaByIdQuery` + Handler en MISMO archivo (sin validator)
- [ ] `GetMisPropuestasQuery` + Handler en MISMO archivo (sin validator)
- [ ] `CreatePropuestaCommand` + Handler en MISMO archivo
- [ ] `RetirarPropuestaCommand` + Handler en MISMO archivo
- [ ] Todos los Commands/Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Todos los Handlers tienen `ILogger<T>` inyectado
- [ ] Handlers NUNCA inyectan DbContext (usan Services)
- [ ] Constructores con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Try-catch con `_logger.LogError` en TODOS los Handlers
- [ ] Usar `ValidateExtensions.NotFoundServiceResponse`, `ForbiddenServiceResponse`, `InternalServerErrorServiceResponse` (patron existente)
- [ ] `EstadoPropuestaConstants` usados en lugar de magic numbers (1, 4, etc.)

### Validators
- [ ] `CreatePropuestaCommandValidator` en carpeta `Validators/` separada
- [ ] `RetirarPropuestaCommandValidator` en carpeta `Validators/` separada
- [ ] Cada regla tiene `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- [ ] ErrorCodes usan constantes de `ServiceResponseMessageType` (NO strings literales)
- [ ] Validaciones asincronas usan `.MustAsync()`
- [ ] Constructor de validators con `?? throw new ArgumentNullException`
- [ ] Ownership verificado en Handler (NO en Validator) para retornar 403 correcto

### AutoMapper
- [ ] `NecesidadCrowdsourcingProfile` ampliado con 2 mappings nuevos
- [ ] `PropuestaCrowdsourcingProfile` ampliado con 4 mappings nuevos
- [ ] Campos calculados y nombres de maestras marcados con `.Ignore()` (se asignan en Handler)
- [ ] `Id.Value` mapeado correctamente para StronglyTypedIds

### DTOs
- [ ] 6 DTOs nuevos creados en `Application/Dtos/`
- [ ] `NecesidadPublicaDto` incluye `YaPropuso`, `EsPropietario`, `TienePerfilProfesional`
- [ ] `MiPropuestaListDto` incluye `AcuerdoId?` (para US-CS-04)
- [ ] `PropuestaCreatedResultDto` incluye todos los campos del contrato

### Constants
- [ ] 5 nuevas constantes en `ServiceResponseMessageType.cs` (2010, 4003, 4004, 4005, 4006)
- [ ] `EstadoPropuestaConstants.cs` creado con 4 valores
- [ ] NO se usan strings literales para ErrorCodes

### Cross-Module
- [ ] `IPerfilProfesionalService` planificada en UserAccess.Application (ver hexagonal-architecture.md)
- [ ] `IArtistaService.GetByUserIdAsync` ya existe y es usable
- [ ] Cache request-scoped planificado para todas las llamadas cross-service en la misma request
