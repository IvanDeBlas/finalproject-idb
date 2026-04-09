# Plan CQRS: cp-inscripcion-programa

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-inscripcion-programa (US-CP-03)
**Depende de:** US-CP-01 (cp-perfil-promotor), US-CP-02 (cp-programas-promocion)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Archivo | Request | Response |
|-----------|------|---------|---------|----------|
| Explorar programas | Query | `ExplorarProgramasQuery.cs` | `ExplorarProgramasQuery` | `ServiceResponse<ExplorarProgramasResultDto>` |
| Solicitar inscripcion | Command | `SolicitarInscripcionCommand.cs` | `SolicitarInscripcionCommand` | `ServiceResponse<InscripcionCreadaDto>` |
| Aprobar inscripcion | Command | `AprobarInscripcionCommand.cs` | `AprobarInscripcionCommand` | `ServiceResponse<InscripcionAprobadaDto>` |
| Rechazar inscripcion | Command | `RechazarInscripcionCommand.cs` | `RechazarInscripcionCommand` | `ServiceResponse<InscripcionRechazadaDto>` |
| Bloquear promotor | Command | `BloquearInscripcionCommand.cs` | `BloquearInscripcionCommand` | `ServiceResponse<InscripcionBloqueadaDto>` |
| Dar de baja promotor | Command | `DarDeBajaInscripcionCommand.cs` | `DarDeBajaInscripcionCommand` | `ServiceResponse<InscripcionDadaDeBajaDto>` |
| Listar inscripciones del programa | Query | `GetInscripcionesProgramaQuery.cs` | `GetInscripcionesProgramaQuery` | `ServiceResponse<InscripcionListResultDto>` |
| Mis inscripciones (promotor) | Query | `GetMisInscripcionesQuery.cs` | `GetMisInscripcionesQuery` | `ServiceResponse<MisInscripcionesResultDto>` |

**Nota de arquitectura:** Todos los Commands/Queries de esta feature usan `IInscripcionService` como servicio primario. Este service nuevo encapsula toda la logica de persistencia de inscripciones y la resolucion de identidad (PromotorId, ArtistaId). Los servicios `IPromoProgramaService` y `IPromotorService` se usan solo como servicios de lectura auxiliares para validaciones cruzadas donde ya existe el metodo implementado.

---

## 2. Prerequisitos de Dominio

Antes de implementar cualquier Command/Query, los siguientes cambios de dominio deben estar aplicados (detallados en `hexagonal-architecture.md`):

### 2.1 Constantes nuevas en ServiceResponseMessageType

**Archivo a MODIFICAR:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

```
// NotFound (2000-2999) - AGREGAR:
public const string NotFound_Inscripcion = "2020";

// Business Rules (4000-4999) - AGREGAR:
public const string BusinessRule_InscripcionAlreadyExists = "4021";
public const string BusinessRule_InscripcionBloqueada = "4022";
public const string BusinessRule_PromotorInactivo = "4023";
public const string BusinessRule_ProgramaInactivo = "4024";
public const string BusinessRule_InscripcionEstadoInvalido = "4025";
public const string BusinessRule_NoEsPropietarioPrograma = "4026";
```

### 2.2 Nuevo IInscripcionService

**Archivo a CREAR:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IInscripcionService.cs`

Metodos requeridos por los handlers:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetPromotorByUserIdAsync` | `Promotor?` | Resolucion de identidad del promotor (con cache) |
| `GetArtistaIdByUserIdAsync` | `ArtistaId?` | Resolucion de identidad del artista (con cache, reutiliza patron existente) |
| `GetProgramasActivosAsync` | `(IReadOnlyList<PromoPrograma>, int)` | Catalogo paginado con filtros para explorar |
| `GetByPromotorYProgramaAsync` | `PromoProgramaPromotor?` | Verificar inscripcion existente (cache por request) |
| `CreateAsync` | `Guid` | Crear nueva inscripcion |
| `GetByIdAsync` | `PromoProgramaPromotor?` | Obtener inscripcion sin datos de promotor |
| `GetByIdWithPromotorAsync` | `PromoProgramaPromotor?` | Obtener inscripcion con datos del promotor (sin cache; lectura fresca) |
| `AprobarAsync` | `void` | Persistir aprobacion (EsAprobado=true, CodigoReferido, UrlReferido) |
| `RechazarAsync` | `void` | Eliminar fisicamente la inscripcion |
| `BloquearAsync` | `void` | Persistir bloqueo (EsBloqueado=true) |
| `DarDeBajaAsync` | `void` | Persistir baja (FechaBaja=now, EsAprobado=false) |
| `GetInscripcionesPorProgramaAsync` | `(IReadOnlyList<PromoProgramaPromotor>, int)` | Lista paginada de inscripciones de un programa |
| `GetMisInscripcionesAsync` | `(IReadOnlyList<PromoProgramaPromotor>, int)` | Lista paginada de inscripciones del promotor (con Programa.Tareas) |
| `CodigoReferidoExistsAsync` | `bool` | Verificar unicidad del codigo generado |

**Nota:** La generacion del CodigoReferido y UrlTrackingPersonalizada ocurre en el Handler, no en el Service. El Handler llama a `CodigoReferidoExistsAsync` para verificar unicidad y luego llama a `AprobarAsync` con los valores ya calculados.

---

## 3. Commands

### 3.1 SolicitarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/SolicitarInscripcionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<InscripcionCreadaDto>>`

| Propiedad | Tipo | Nullable | Origen | Descripcion |
|-----------|------|----------|--------|-------------|
| `ProgramaId` | `Guid` | No | Path param `programaId` | Id del programa donde se solicita inscripcion |
| `UserId` | `string` | No | JWT claim `sub` | Para resolver PromotorId del promotor autenticado |

#### Handler: SolicitarInscripcionCommandHandler

**Implementa:** `IRequestHandler<SolicitarInscripcionCommand, ServiceResponse<InscripcionCreadaDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `IValidator<SolicitarInscripcionCommand> _validator`
- `ILogger<SolicitarInscripcionCommandHandler> _logger`

**Flujo paso a paso:**

```
1. await _validator.ValidateAsync(request, ct)
   - Si invalido -> return ServiceResponse con validationResult.GetServiceResponseMessages()

2. Resolver Promotor: var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, ct)
   - Si null -> ValidateExtensions.NotFoundServiceResponse<InscripcionCreadaDto>(
       "No tienes un perfil de promotor",
       ServiceResponseMessageType.NotFound_Promotor)  // "2015"

3. Verificar Promotor.EsActivo == true
   - Si false -> ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
       "Tu perfil de promotor esta desactivado",
       ServiceResponseMessageType.BusinessRule_PromotorInactivo)  // "4023"

4. Resolver PromoPrograma: var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   [NOTA: Usar el metodo de IPromoProgramaService reutilizando el service existente para obtener el programa]
   - Si null -> ValidateExtensions.NotFoundServiceResponse<InscripcionCreadaDto>(
       "El programa de promocion no existe",
       ServiceResponseMessageType.NotFound_PromoPrograma)  // "2019"

5. Verificar programa.EsActivo == true
   - Si false -> ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
       "El programa de promocion no esta activo",
       ServiceResponseMessageType.BusinessRule_ProgramaInactivo)  // "4024"

6. Verificar inscripcion existente:
   var inscripcionExistente = await _inscripcionService.GetByPromotorYProgramaAsync(
       promotor.Id, new PromoProgramaId(request.ProgramaId), ct)
   - Si existe y EsBloqueado == true -> ValidateExtensions.ForbiddenServiceResponse<InscripcionCreadaDto>(
       "No puedes inscribirte en este programa",
       ServiceResponseMessageType.BusinessRule_InscripcionBloqueada)  // "4022"
   - Si existe y EsBloqueado == false -> ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
       "Ya tienes una inscripcion en este programa",
       ServiceResponseMessageType.BusinessRule_InscripcionAlreadyExists)  // "4021"

7. Crear entidad:
   var inscripcion = new PromoProgramaPromotor
   {
       Id = Guid.NewGuid(),
       ProgramaId = new PromoProgramaId(request.ProgramaId),
       PromotorId = promotor.Id,
       EsAprobado = false,
       EsBloqueado = false,
       EsActivo = true,
       FechaInscripcion = DateTime.UtcNow
   }

8. var id = await _inscripcionService.CreateAsync(inscripcion, ct)

9. var dto = _mapper.Map<InscripcionCreadaDto>(inscripcion)
   dto.ProgramaTitulo = programa.Titulo
   dto.Id = id

10. _logger.LogInformation("Inscripcion created for PromotorId {PromotorId} in ProgramaId {ProgramaId}",
        promotor.Id, request.ProgramaId)

11. return new ServiceResponse<InscripcionCreadaDto>
    {
        Data = dto,
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Inscripcion solicitada", HttpStatusCode = HttpStatusCode.Created }
        }
    }

12. catch (Exception ex):
    _logger.LogError(ex, "Error creating inscripcion for UserId {UserId} ProgramaId {ProgramaId}",
        request.UserId, request.ProgramaId)
    return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionCreadaDto>(
        "Error inesperado al solicitar la inscripcion",
        ServiceResponseMessageType.Internal_UnexpectedError)
```

**Nota importante:** El handler necesita acceso a `IPromoProgramaService` para `GetByIdWithFullDetailAsync` o un metodo ligero. Revisar si `IInscripcionService` encapsula este metodo o si se inyecta `IPromoProgramaService` adicionalmente. Segun `api-contracts.md` seccion 6, `IInscripcionService.GetProgramasActivosAsync` es el metodo de exploracion, pero para este handler se necesita `GetByIdAsync` del programa. Opciones:
- Opcion A (recomendada): Agregar `GetProgramaByIdAsync(PromoProgramaId, ct)` a `IInscripcionService`
- Opcion B: Inyectar tambien `IPromoProgramaService` en el handler

---

### 3.2 AprobarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/AprobarInscripcionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<InscripcionAprobadaDto>>`

| Propiedad | Tipo | Nullable | Origen | Descripcion |
|-----------|------|----------|--------|-------------|
| `ProgramaId` | `Guid` | No | Path param `programaId` | Id del programa para verificar ownership |
| `InscripcionId` | `Guid` | No | Path param `inscripcionId` | Id del PromoProgramaPromotor a aprobar |
| `UserId` | `string` | No | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

#### Handler: AprobarInscripcionCommandHandler

**Implementa:** `IRequestHandler<AprobarInscripcionCommand, ServiceResponse<InscripcionAprobadaDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `IValidator<AprobarInscripcionCommand> _validator`
- `ILogger<AprobarInscripcionCommandHandler> _logger`

**Flujo paso a paso:**

```
1. await _validator.ValidateAsync(request, ct)
   - Si invalido -> return ServiceResponse con errores de validacion

2. Resolver ArtistaId: var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionAprobadaDto>(
       "No tienes un perfil de artista",
       ServiceResponseMessageType.NotFound_Artista)  // "2016"

3. Resolver PromoPrograma:
   [Misma nota que en SolicitarInscripcion sobre como obtener el programa]
   var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   - Si null -> NotFoundServiceResponse<InscripcionAprobadaDto>(
       "El programa de promocion no existe",
       ServiceResponseMessageType.NotFound_PromoPrograma)  // "2019"

4. Verificar ownership: programa.ArtistaId != artistaId.Value
   - Si no coincide -> ValidateExtensions.ForbiddenServiceResponse<InscripcionAprobadaDto>(
       "No tienes permiso para gestionar este programa",
       ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma)  // "4026"

5. Resolver inscripcion con datos de promotor:
   var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionAprobadaDto>(
       "La inscripcion no existe",
       ServiceResponseMessageType.NotFound_Inscripcion)  // "2020"

6. Verificar estado pendiente: inscripcion.EsAprobado == false && inscripcion.EsBloqueado == false && inscripcion.FechaBaja == null
   - Si no cumple -> ValidateExtensions.BadRequestServiceResponse<InscripcionAprobadaDto>(
       "La inscripcion no esta en estado pendiente",
       ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido)  // "4025"

7. Generar CodigoReferido con logica de reintento (logica de negocio en el Handler):
   var codigoBase = string.IsNullOrEmpty(programa.CodigoTrackingBase)
       ? request.ProgramaId.ToString("N")[..8]
       : programa.CodigoTrackingBase;

   string codigoReferido = null!;
   for (int intento = 0; intento < 3; intento++)
   {
       var shortId = Guid.NewGuid().ToString("N")[..5];
       var candidato = $"{codigoBase}-{shortId}";
       if (!await _inscripcionService.CodigoReferidoExistsAsync(candidato, ct))
       {
           codigoReferido = candidato;
           break;
       }
   }
   Si codigoReferido es null tras 3 intentos:
   _logger.LogError("Unable to generate unique CodigoReferido for InscripcionId {Id}", request.InscripcionId)
   return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionAprobadaDto>(
       "No se pudo generar un codigo unico",
       ServiceResponseMessageType.Internal_UnexpectedError)

8. Construir UrlTrackingPersonalizada:
   string? urlTracking = null;
   if (!string.IsNullOrEmpty(programa.UrlLanding))
   {
       urlTracking = $"{programa.UrlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={codigoBase}&ref={codigoReferido}";
   }

9. Actualizar entidad en memoria (logica de negocio en Handler):
   inscripcion.EsAprobado = true;
   inscripcion.CodigoReferido = codigoReferido;
   inscripcion.UrlReferido = urlTracking;

10. Persistir: await _inscripcionService.AprobarAsync(inscripcion, ct)

11. Construir DTO:
    var dto = _mapper.Map<InscripcionAprobadaDto>(inscripcion)
    dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty

12. _logger.LogInformation(
        "Inscripcion {InscripcionId} aprobada. CodigoReferido: {Codigo}",
        inscripcion.Id, codigoReferido)

13. return new ServiceResponse<InscripcionAprobadaDto>
    {
        Data = dto,
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Inscripcion aprobada", ErrorCode = ServiceResponseMessageType.Updated }
        }
    }

14. catch (Exception ex):
    _logger.LogError(ex, "Error approving inscripcion {InscripcionId} for UserId {UserId}",
        request.InscripcionId, request.UserId)
    return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionAprobadaDto>(
        "Error inesperado al aprobar la inscripcion",
        ServiceResponseMessageType.Internal_UnexpectedError)
```

---

### 3.3 RechazarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/RechazarInscripcionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<InscripcionRechazadaDto>>`

| Propiedad | Tipo | Nullable | Origen | Descripcion |
|-----------|------|----------|--------|-------------|
| `ProgramaId` | `Guid` | No | Path param `programaId` | Id del programa para verificar ownership |
| `InscripcionId` | `Guid` | No | Path param `inscripcionId` | Id del PromoProgramaPromotor a rechazar (se elimina fisicamente) |
| `UserId` | `string` | No | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

#### Handler: RechazarInscripcionCommandHandler

**Implementa:** `IRequestHandler<RechazarInscripcionCommand, ServiceResponse<InscripcionRechazadaDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `IValidator<RechazarInscripcionCommand> _validator`
- `ILogger<RechazarInscripcionCommandHandler> _logger`

**Flujo paso a paso:**

```
1. await _validator.ValidateAsync(request, ct)
   - Si invalido -> return ServiceResponse con errores

2. Resolver ArtistaId: var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionRechazadaDto>(
       "No tienes un perfil de artista",
       ServiceResponseMessageType.NotFound_Artista)

3. Resolver PromoPrograma:
   var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   - Si null -> NotFoundServiceResponse<InscripcionRechazadaDto>(
       "El programa de promocion no existe",
       ServiceResponseMessageType.NotFound_PromoPrograma)

4. Verificar ownership: programa.ArtistaId != artistaId.Value
   - Si no coincide -> ForbiddenServiceResponse<InscripcionRechazadaDto>(
       "No tienes permiso para gestionar este programa",
       ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma)

5. Resolver inscripcion con datos de promotor:
   var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionRechazadaDto>(
       "La inscripcion no existe",
       ServiceResponseMessageType.NotFound_Inscripcion)

6. Verificar estado pendiente: inscripcion.EsAprobado == false && inscripcion.EsBloqueado == false && inscripcion.FechaBaja == null
   - Si no cumple -> BadRequestServiceResponse<InscripcionRechazadaDto>(
       "Solo se pueden rechazar inscripciones en estado pendiente",
       ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido)

7. Guardar nombre del promotor ANTES de eliminar (para el DTO de respuesta):
   var promotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty
   var inscripcionId = inscripcion.Id

8. Eliminar fisicamente:
   await _inscripcionService.RechazarAsync(request.InscripcionId, ct)

9. _logger.LogInformation(
       "Inscripcion {InscripcionId} rechazada y eliminada para Promotor {PromotorNombre}",
       inscripcionId, promotorNombre)

10. return new ServiceResponse<InscripcionRechazadaDto>
    {
        Data = new InscripcionRechazadaDto
        {
            InscripcionId = inscripcionId,
            PromotorNombre = promotorNombre
        },
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Inscripcion rechazada", ErrorCode = ServiceResponseMessageType.Deleted }
        }
    }

11. catch (Exception ex):
    _logger.LogError(ex, "Error rejecting inscripcion {InscripcionId} for UserId {UserId}",
        request.InscripcionId, request.UserId)
    return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionRechazadaDto>(...)
```

**Nota critica:** El DTO de respuesta se construye manualmente (NO via mapper) porque la entidad ya fue eliminada antes de mapear. El nombre del promotor se guarda en variable local antes del `RechazarAsync`.

---

### 3.4 BloquearInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/BloquearInscripcionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<InscripcionBloqueadaDto>>`

| Propiedad | Tipo | Nullable | Origen | Descripcion |
|-----------|------|----------|--------|-------------|
| `ProgramaId` | `Guid` | No | Path param `programaId` | Id del programa para verificar ownership |
| `InscripcionId` | `Guid` | No | Path param `inscripcionId` | Id del PromoProgramaPromotor a bloquear |
| `UserId` | `string` | No | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

#### Handler: BloquearInscripcionCommandHandler

**Implementa:** `IRequestHandler<BloquearInscripcionCommand, ServiceResponse<InscripcionBloqueadaDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `IValidator<BloquearInscripcionCommand> _validator`
- `ILogger<BloquearInscripcionCommandHandler> _logger`

**Flujo paso a paso:**

```
1. await _validator.ValidateAsync(request, ct)
   - Si invalido -> return ServiceResponse con errores

2. Resolver ArtistaId: var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionBloqueadaDto>(..., NotFound_Artista)

3. Resolver PromoPrograma:
   var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   - Si null -> NotFoundServiceResponse<InscripcionBloqueadaDto>(..., NotFound_PromoPrograma)

4. Verificar ownership: programa.ArtistaId != artistaId.Value
   - Si no coincide -> ForbiddenServiceResponse<InscripcionBloqueadaDto>(..., BusinessRule_NoEsPropietarioPrograma)

5. Resolver inscripcion con datos de promotor:
   var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionBloqueadaDto>(..., NotFound_Inscripcion)

6. Verificar que no este ya bloqueada: inscripcion.EsBloqueado == true
   - Si ya bloqueada -> BadRequestServiceResponse<InscripcionBloqueadaDto>(
       "Este promotor ya esta bloqueado en este programa",
       ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido)

7. Aplicar logica de negocio en Handler:
   inscripcion.EsBloqueado = true;
   if (inscripcion.EsAprobado)
   {
       inscripcion.EsAprobado = false;  // Revocar aprobacion al bloquear
   }

8. Persistir: await _inscripcionService.BloquearAsync(inscripcion, ct)

9. var dto = _mapper.Map<InscripcionBloqueadaDto>(inscripcion)
   dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty

10. _logger.LogInformation(
        "Promotor {PromotorNombre} bloqueado en InscripcionId {InscripcionId}",
        dto.PromotorNombre, inscripcion.Id)

11. return new ServiceResponse<InscripcionBloqueadaDto>
    {
        Data = dto,
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Promotor bloqueado", ErrorCode = ServiceResponseMessageType.Updated }
        }
    }

12. catch (Exception ex):
    _logger.LogError(ex, "Error blocking inscripcion {InscripcionId} for UserId {UserId}", ...)
    return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionBloqueadaDto>(...)
```

---

### 3.5 DarDeBajaInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/DarDeBajaInscripcionCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<InscripcionDadaDeBajaDto>>`

| Propiedad | Tipo | Nullable | Origen | Descripcion |
|-----------|------|----------|--------|-------------|
| `ProgramaId` | `Guid` | No | Path param `programaId` | Id del programa para verificar ownership |
| `InscripcionId` | `Guid` | No | Path param `inscripcionId` | Id del PromoProgramaPromotor a dar de baja |
| `UserId` | `string` | No | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

#### Handler: DarDeBajaInscripcionCommandHandler

**Implementa:** `IRequestHandler<DarDeBajaInscripcionCommand, ServiceResponse<InscripcionDadaDeBajaDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `IValidator<DarDeBajaInscripcionCommand> _validator`
- `ILogger<DarDeBajaInscripcionCommandHandler> _logger`

**Flujo paso a paso:**

```
1. await _validator.ValidateAsync(request, ct)
   - Si invalido -> return ServiceResponse con errores

2. Resolver ArtistaId: var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionDadaDeBajaDto>(..., NotFound_Artista)

3. Resolver PromoPrograma:
   var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   - Si null -> NotFoundServiceResponse<InscripcionDadaDeBajaDto>(..., NotFound_PromoPrograma)

4. Verificar ownership: programa.ArtistaId != artistaId.Value
   - Si no coincide -> ForbiddenServiceResponse<InscripcionDadaDeBajaDto>(..., BusinessRule_NoEsPropietarioPrograma)

5. Resolver inscripcion con datos de promotor:
   var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionDadaDeBajaDto>(..., NotFound_Inscripcion)

6. Verificar estado aprobado: inscripcion.EsAprobado == true && inscripcion.FechaBaja == null
   - Si no cumple -> BadRequestServiceResponse<InscripcionDadaDeBajaDto>(
       "Esta inscripcion no esta en estado aprobado",
       ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido)

7. Aplicar logica de negocio en Handler:
   var fechaBaja = DateTime.UtcNow;
   inscripcion.FechaBaja = fechaBaja;
   inscripcion.EsAprobado = false;

8. Persistir: await _inscripcionService.DarDeBajaAsync(inscripcion, ct)

9. var dto = _mapper.Map<InscripcionDadaDeBajaDto>(inscripcion)
   dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty

10. _logger.LogInformation(
        "Promotor {PromotorNombre} dado de baja en InscripcionId {InscripcionId}",
        dto.PromotorNombre, inscripcion.Id)

11. return new ServiceResponse<InscripcionDadaDeBajaDto>
    {
        Data = dto,
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Promotor dado de baja", ErrorCode = ServiceResponseMessageType.Updated }
        }
    }

12. catch (Exception ex):
    _logger.LogError(ex, "Error giving baja to inscripcion {InscripcionId} for UserId {UserId}", ...)
    return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionDadaDeBajaDto>(...)
```

---

## 4. Queries

### 4.1 ExplorarProgramasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/ExplorarProgramasQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries`

#### Query

**Implementa:** `IRequest<ServiceResponse<ExplorarProgramasResultDto>>`

| Propiedad | Tipo | Nullable | Valor por defecto | Descripcion |
|-----------|------|----------|-------------------|-------------|
| `UserId` | `string` | No | - | Para resolver PromotorId y calcular MiEstado |
| `ArtistaNombre` | `string?` | Si | `null` | Filtro contains case-insensitive por nombre artistico |
| `TipoPromoId` | `int?` | Si | `null` | Filtro por tipo de programa |
| `Page` | `int` | No | `1` | Pagina actual |
| `PageSize` | `int` | No | `10` | Elementos por pagina (min 1, max 50) |

**Nota:** No tiene Validator. La normalizacion de Page/PageSize (Math.Max, Math.Min) ocurre en el controller antes de construir el query.

#### Handler: ExplorarProgramasQueryHandler

**Implementa:** `IRequestHandler<ExplorarProgramasQuery, ServiceResponse<ExplorarProgramasResultDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `ILogger<ExplorarProgramasQueryHandler> _logger`

**Diccionarios de lookup estaticos (readonly):**
```
TipoPromoNombres: { 1: "Referral", 2: "Afiliado", 3: "Influencer", 4: "Mixto" }
MonedaNombres: { 1: "EUR", 2: "USD" }
```

**Flujo paso a paso:**

```
1. Resolver Promotor: var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<ExplorarProgramasResultDto>(
       "No tienes un perfil de promotor",
       ServiceResponseMessageType.NotFound_Promotor)

2. Obtener programas activos paginados:
   var (items, totalCount) = await _inscripcionService.GetProgramasActivosAsync(
       request.ArtistaNombre, request.TipoPromoId, request.Page, request.PageSize, promotor.Id, ct)

3. Para cada programa en items:
   a. Mapear: var dto = _mapper.Map<ProgramaExplorarItemDto>(programa)
   b. Resolver TipoPromoNombre: TipoPromoNombres.TryGetValue(programa.TipoPromoId, out var tipoNombre) -> dto.TipoPromoNombre
   c. Resolver MonedaNombre: MonedaNombres.TryGetValue(programa.MonedaId ?? 0, out var monedaNombre) -> dto.MonedaNombre
   d. Calcular NumeroTareas: dto.NumeroTareas = programa.Tareas?.Count(t => t.EsActivo) ?? 0
   e. Calcular MiEstado desde la inscripcion incluida en GetProgramasActivosAsync:
      [IInscripcionService.GetProgramasActivosAsync incluye la inscripcion del promotor para cada programa]
      La inscripcion del promotor se incluye en el PromoPrograma via Include(p => p.Promotores.Where(pp => pp.PromotorId == promotorId))
      - Si no hay inscripcion del promotor -> dto.MiEstado = null
      - Si EsBloqueado -> dto.MiEstado = "Bloqueado"
      - Else If FechaBaja != null -> dto.MiEstado = "DadoDeBaja"
      - Else If EsAprobado -> dto.MiEstado = "Aprobado"
      - Else -> dto.MiEstado = "Pendiente"
   f. ArtistaNombre: si el service devuelve el nombre del artista en los datos auxiliares, asignar;
      si no, se necesita una segunda consulta cruzada via IInscripcionService

4. Construir resultado:
   var resultDto = new ExplorarProgramasResultDto
   {
       Items = dtoItems,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize,
       TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
   }

5. return new ServiceResponse<ExplorarProgramasResultDto> { Data = resultDto }

6. catch (Exception ex):
   _logger.LogError(ex, "Error exploring programas for UserId {UserId}", request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse<ExplorarProgramasResultDto>(...)
```

**Decision de diseno sobre ArtistaNombre:** Segun `hexagonal-architecture.md` seccion 4.4, se sigue Opcion A: el repositorio devuelve `PromoPrograma` con sus datos y el handler resuelve el nombre del artista con una consulta cross-context. `IInscripcionService.GetProgramasActivosAsync` puede incluir el `ArtistaId` en el resultado; el handler llama a un metodo `GetArtistaNombreByIdAsync` adicional en el service o resuelve directamente. Documentar en la implementacion cuantas queries cross-context se realizan.

---

### 4.2 GetInscripcionesProgramaQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/GetInscripcionesProgramaQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries`

#### Query

**Implementa:** `IRequest<ServiceResponse<InscripcionListResultDto>>`

| Propiedad | Tipo | Nullable | Valor por defecto | Descripcion |
|-----------|------|----------|-------------------|-------------|
| `ProgramaId` | `Guid` | No | - | Id del programa cuyas inscripciones se listan |
| `UserId` | `string` | No | - | Para resolver ArtistaId y verificar ownership |
| `Estado` | `string?` | Si | `null` | Filtro: `Pendiente`, `Aprobado`, `Bloqueado`, `DadoDeBaja`. Sin valor: todos |
| `Page` | `int` | No | `1` | Pagina actual |
| `PageSize` | `int` | No | `20` | Elementos por pagina (max 50) |

**Nota:** No tiene Validator propio. Validacion en el controller.

#### Handler: GetInscripcionesProgramaQueryHandler

**Implementa:** `IRequestHandler<GetInscripcionesProgramaQuery, ServiceResponse<InscripcionListResultDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `ILogger<GetInscripcionesProgramaQueryHandler> _logger`

**Diccionarios de lookup estaticos (readonly):**
```
TipoPromotorNombres: { 1: "Fan Embajador", 2: "Influencer", 3: "Medio / Blog", 4: "Profesional Marketing" }
```

**Flujo paso a paso:**

```
1. Resolver ArtistaId: var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<InscripcionListResultDto>(..., NotFound_Artista)

2. Resolver PromoPrograma:
   var programa = await _inscripcionService.GetProgramaByIdAsync(new PromoProgramaId(request.ProgramaId), ct)
   - Si null -> NotFoundServiceResponse<InscripcionListResultDto>(..., NotFound_PromoPrograma)

3. Verificar ownership: programa.ArtistaId != artistaId.Value
   - Si no coincide -> ForbiddenServiceResponse<InscripcionListResultDto>(..., BusinessRule_NoEsPropietarioPrograma)

4. Obtener inscripciones paginadas con filtro:
   var (items, totalCount) = await _inscripcionService.GetInscripcionesPorProgramaAsync(
       new PromoProgramaId(request.ProgramaId), request.Estado, request.Page, request.PageSize, ct)

5. Para cada inscripcion en items:
   a. var dto = _mapper.Map<InscripcionListItemDto>(inscripcion)
   b. Asignar datos del promotor desde inscripcion.Promotor (ya incluido en la query del repositorio):
      dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty
      dto.PromotorId = inscripcion.Promotor?.Id.Value ?? Guid.Empty
      dto.PromotorEmailContacto = inscripcion.Promotor?.EmailContacto
      dto.PromotorUrlInstagram = inscripcion.Promotor?.UrlInstagram
      dto.PromotorUrlTikTok = inscripcion.Promotor?.UrlTikTok
      dto.PromotorUrlSitioWeb = inscripcion.Promotor?.UrlSitioWeb
      if (TipoPromotorNombres.TryGetValue(inscripcion.Promotor?.TipoPromotorId ?? 0, out var tipoNombre))
          dto.TipoPromotorNombre = tipoNombre
   c. Calcular Estado (prioridad segun maquina de estados):
      dto.Estado = inscripcion.EsBloqueado ? "Bloqueado"
                 : inscripcion.FechaBaja != null ? "DadoDeBaja"
                 : inscripcion.EsAprobado ? "Aprobado"
                 : "Pendiente"
   d. CodigoReferido solo si aprobado: dto.CodigoReferido = inscripcion.EsAprobado ? inscripcion.CodigoReferido : null

6. var resultDto = new InscripcionListResultDto
   {
       Items = dtoItems,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize,
       TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
   }

7. return new ServiceResponse<InscripcionListResultDto> { Data = resultDto }

8. catch (Exception ex):
   _logger.LogError(ex, "Error getting inscripciones for ProgramaId {ProgramaId} UserId {UserId}", ...)
   return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionListResultDto>(...)
```

---

### 4.3 GetMisInscripcionesQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/GetMisInscripcionesQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries`

#### Query

**Implementa:** `IRequest<ServiceResponse<MisInscripcionesResultDto>>`

| Propiedad | Tipo | Nullable | Valor por defecto | Descripcion |
|-----------|------|----------|-------------------|-------------|
| `UserId` | `string` | No | - | Para resolver PromotorId del promotor autenticado |
| `Page` | `int` | No | `1` | Pagina actual |
| `PageSize` | `int` | No | `10` | Elementos por pagina (max 50) |

**Nota:** No tiene Validator propio.

#### Handler: GetMisInscripcionesQueryHandler

**Implementa:** `IRequestHandler<GetMisInscripcionesQuery, ServiceResponse<MisInscripcionesResultDto>>`

**Dependencias (con `?? throw` en constructor):**
- `IInscripcionService _inscripcionService`
- `IMapper _mapper`
- `ILogger<GetMisInscripcionesQueryHandler> _logger`

**Diccionarios de lookup estaticos (readonly):**
```
TipoPromoNombres: { 1: "Referral", 2: "Afiliado", 3: "Influencer", 4: "Mixto" }
MonedaNombres: { 1: "EUR", 2: "USD" }
TipoEventoPromoNombres: { 1: "Click", 2: "PageView", 3: "Share", 4: "Post", 5: "Signup", 6: "Backing" }
```

**Flujo paso a paso:**

```
1. Resolver Promotor: var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, ct)
   - Si null -> NotFoundServiceResponse<MisInscripcionesResultDto>(..., NotFound_Promotor)

2. Obtener inscripciones paginadas (incluye Programa.Tareas activas):
   var (items, totalCount) = await _inscripcionService.GetMisInscripcionesAsync(
       promotor.Id, request.Page, request.PageSize, ct)

3. Para cada inscripcion en items:
   a. var dto = _mapper.Map<MiInscripcionDto>(inscripcion)

   b. Datos del Programa (desde inscripcion.Programa incluido):
      dto.ProgramaTitulo = inscripcion.Programa?.Titulo ?? string.Empty
      dto.ProgramaId = inscripcion.ProgramaId.Value
      if (TipoPromoNombres.TryGetValue(inscripcion.Programa?.TipoPromoId ?? 0, out var tipoNombre))
          dto.TipoPromoNombre = tipoNombre
      dto.ImporteComisionPorcentaje = inscripcion.Programa?.ImporteComisionPorcentaje
      dto.ImporteComisionFija = inscripcion.Programa?.ImporteComisionFija
      if (MonedaNombres.TryGetValue(inscripcion.Programa?.MonedaId ?? 0, out var monedaNombre))
          dto.MonedaNombre = monedaNombre
      [ArtistaNombre requiere cross-context query similar al ExplorarProgramas handler]

   c. Calcular Estado:
      dto.Estado = inscripcion.EsBloqueado ? "Bloqueado"
                 : inscripcion.FechaBaja != null ? "DadoDeBaja"
                 : inscripcion.EsAprobado ? "Aprobado"
                 : "Pendiente"

   d. CodigoReferido y UrlTracking: SOLO si aprobado activo (no dado de baja):
      bool esAprobadoActivo = inscripcion.EsAprobado && inscripcion.FechaBaja == null;
      dto.CodigoReferido = esAprobadoActivo ? inscripcion.CodigoReferido : null
      dto.UrlTrackingPersonalizada = esAprobadoActivo ? inscripcion.UrlReferido : null

   e. Tareas: SOLO si aprobado activo:
      if (esAprobadoActivo)
      {
          dto.Tareas = inscripcion.Programa?.Tareas
              ?.Where(t => t.EsActivo)
              .Select(tarea => {
                  var tareaDto = _mapper.Map<TareaResumenDto>(tarea)
                  if (TipoEventoPromoNombres.TryGetValue(tarea.TipoEventoPromoId, out var eventoNombre))
                      tareaDto.TipoEventoPromoNombre = eventoNombre
                  if (MonedaNombres.TryGetValue(tarea.MonedaId ?? 0, out var monNombre))
                      tareaDto.MonedaNombre = monNombre
                  return tareaDto
              }).ToList() ?? new List<TareaResumenDto>()
      }
      else
      {
          dto.Tareas = new List<TareaResumenDto>()
      }

4. var resultDto = new MisInscripcionesResultDto
   {
       Items = dtoItems,
       TotalCount = totalCount,
       Page = request.Page,
       PageSize = request.PageSize,
       TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
   }

5. return new ServiceResponse<MisInscripcionesResultDto> { Data = resultDto }

6. catch (Exception ex):
   _logger.LogError(ex, "Error getting mis-inscripciones for UserId {UserId}", request.UserId)
   return ValidateExtensions.InternalServerErrorServiceResponse<MisInscripcionesResultDto>(...)
```

---

## 5. Validators

Los Commands de esta feature no tienen body de request (todos los parametros vienen de la ruta o del token). Los Validators son ligeros: solo validan la presencia de los identificadores. La logica de negocio (promotor activo, programa activo, estado de inscripcion, ownership) se valida en el Handler.

### 5.1 SolicitarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/SolicitarInscripcionCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators`

**Extiende:** `AbstractValidator<SolicitarInscripcionCommand>`

**No tiene dependencias inyectadas** (no requiere acceso a base de datos via service).

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| `UserId` | `NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty()` (Guid no vacio: `Must(id => id != Guid.Empty)`) | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |

---

### 5.2 AprobarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/AprobarInscripcionCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators`

**Extiende:** `AbstractValidator<AprobarInscripcionCommand>`

**No tiene dependencias inyectadas.**

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| `UserId` | `NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `ProgramaId` | `Must(id => id != Guid.Empty)` | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `InscripcionId` | `Must(id => id != Guid.Empty)` | "El identificador de la inscripcion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

---

### 5.3 RechazarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/RechazarInscripcionCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators`

**Extiende:** `AbstractValidator<RechazarInscripcionCommand>`

**No tiene dependencias inyectadas.**

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| `UserId` | `NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `ProgramaId` | `Must(id => id != Guid.Empty)` | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `InscripcionId` | `Must(id => id != Guid.Empty)` | "El identificador de la inscripcion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

---

### 5.4 BloquearInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/BloquearInscripcionCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators`

**Extiende:** `AbstractValidator<BloquearInscripcionCommand>`

**No tiene dependencias inyectadas.**

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| `UserId` | `NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `ProgramaId` | `Must(id => id != Guid.Empty)` | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `InscripcionId` | `Must(id => id != Guid.Empty)` | "El identificador de la inscripcion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

---

### 5.5 DarDeBajaInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/DarDeBajaInscripcionCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators`

**Extiende:** `AbstractValidator<DarDeBajaInscripcionCommand>`

**No tiene dependencias inyectadas.**

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| `UserId` | `NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `ProgramaId` | `Must(id => id != Guid.Empty)` | "El identificador del programa es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `InscripcionId` | `Must(id => id != Guid.Empty)` | "El identificador de la inscripcion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

---

## 6. DTOs

### 6.1 DTOs del promotor

#### ProgramaExplorarItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaExplorarItemDto.cs`

| Propiedad | Tipo C# | Nullable |
|-----------|---------|----------|
| `Id` | `Guid` | No |
| `Titulo` | `string` | No (`null!`) |
| `ArtistaNombre` | `string` | No (`null!`) - asignado en handler |
| `TipoPromoId` | `int` | No |
| `TipoPromoNombre` | `string` | No (`null!`) - asignado en handler |
| `ImporteComisionPorcentaje` | `decimal?` | Si |
| `ImporteComisionFija` | `decimal?` | Si |
| `MonedaNombre` | `string?` | Si - asignado en handler |
| `NumeroTareas` | `int` | No - calculado en handler |
| `CampaniaTitulo` | `string?` | Si |
| `FechaInicio` | `DateTime?` | Si |
| `FechaFin` | `DateTime?` | Si |
| `MiEstado` | `string?` | Si - calculado en handler (`"Pendiente"`, `"Aprobado"`, `"Bloqueado"`, `"DadoDeBaja"`, o `null` si no inscrito) |

#### ExplorarProgramasResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ExplorarProgramasResultDto.cs`

| Propiedad | Tipo C# |
|-----------|---------|
| `Items` | `List<ProgramaExplorarItemDto>` |
| `TotalCount` | `int` |
| `Page` | `int` |
| `PageSize` | `int` |
| `TotalPages` | `int` |

#### InscripcionCreadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionCreadaDto.cs`

| Propiedad | Tipo C# | Origen |
|-----------|---------|--------|
| `Id` | `Guid` | Id del PromoProgramaPromotor creado |
| `ProgramaId` | `Guid` | Mapeado desde `ProgramaId.Value` |
| `ProgramaTitulo` | `string` | Ignorado en mapper; asignado en handler desde `Programa.Titulo` |
| `EsAprobado` | `bool` | Siempre `false` |
| `EsBloqueado` | `bool` | Siempre `false` |
| `FechaAlta` | `DateTime` | Mapeado desde `FechaInscripcion` |

#### MiInscripcionDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MiInscripcionDto.cs`

| Propiedad | Tipo C# | Nullable | Asignacion |
|-----------|---------|----------|------------|
| `Id` | `Guid` | No | Mapper |
| `ProgramaId` | `Guid` | No | Handler desde `ProgramaId.Value` |
| `ProgramaTitulo` | `string` | No | Handler desde `Programa.Titulo` |
| `ArtistaNombre` | `string` | No | Handler via cross-context |
| `TipoPromoNombre` | `string` | No | Handler via diccionario |
| `ImporteComisionPorcentaje` | `decimal?` | Si | Handler desde `Programa` |
| `ImporteComisionFija` | `decimal?` | Si | Handler desde `Programa` |
| `MonedaNombre` | `string?` | Si | Handler via diccionario |
| `EsAprobado` | `bool` | No | Mapper |
| `EsBloqueado` | `bool` | No | Mapper |
| `CodigoReferido` | `string?` | Si | Handler: solo si `EsAprobado && FechaBaja == null` |
| `UrlTrackingPersonalizada` | `string?` | Si | Handler: solo si aprobado activo; de `UrlReferido` |
| `FechaAlta` | `DateTime` | No | Mapper desde `FechaInscripcion` |
| `FechaBaja` | `DateTime?` | Si | Mapper |
| `Estado` | `string` | No | Handler calculado |
| `Tareas` | `List<TareaResumenDto>` | No (lista vacia si no aprobado) | Handler |

#### TareaResumenDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareaResumenDto.cs`

| Propiedad | Tipo C# | Nullable |
|-----------|---------|----------|
| `Id` | `Guid` | No |
| `Titulo` | `string` | No |
| `Descripcion` | `string?` | Si |
| `TipoEventoPromoNombre` | `string` | No - Handler via diccionario |
| `ImporteRecompensa` | `decimal?` | Si |
| `MonedaNombre` | `string?` | Si - Handler via diccionario |
| `EsRepetible` | `bool` | No |
| `MaxRepeticiones` | `int?` | Si |
| `Orden` | `int` | No |

#### MisInscripcionesResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisInscripcionesResultDto.cs`

| Propiedad | Tipo C# |
|-----------|---------|
| `Items` | `List<MiInscripcionDto>` |
| `TotalCount` | `int` |
| `Page` | `int` |
| `PageSize` | `int` |
| `TotalPages` | `int` |

---

### 6.2 DTOs del artista

#### InscripcionAprobadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionAprobadaDto.cs`

| Propiedad | Tipo C# | Asignacion |
|-----------|---------|------------|
| `Id` | `Guid` | Mapper |
| `PromotorNombre` | `string` | Handler desde `Promotor.NombrePublico` |
| `EsAprobado` | `bool` | Mapper (siempre `true`) |
| `EsBloqueado` | `bool` | Mapper (siempre `false`) |
| `CodigoReferido` | `string` | Mapper desde `CodigoReferido` |
| `UrlTrackingPersonalizada` | `string?` | Mapper desde `UrlReferido` |

#### InscripcionRechazadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionRechazadaDto.cs`

| Propiedad | Tipo C# | Asignacion |
|-----------|---------|------------|
| `InscripcionId` | `Guid` | Handler (guardado antes del DELETE) |
| `PromotorNombre` | `string` | Handler (guardado antes del DELETE) |

**Nota:** Este DTO se construye manualmente en el handler sin AutoMapper porque la entidad ya fue eliminada.

#### InscripcionBloqueadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionBloqueadaDto.cs`

| Propiedad | Tipo C# | Asignacion |
|-----------|---------|------------|
| `Id` | `Guid` | Mapper |
| `PromotorNombre` | `string` | Handler desde `Promotor.NombrePublico` |
| `EsBloqueado` | `bool` | Mapper (siempre `true`) |
| `EsAprobado` | `bool` | Mapper (siempre `false`) |

#### InscripcionDadaDeBajaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionDadaDeBajaDto.cs`

| Propiedad | Tipo C# | Asignacion |
|-----------|---------|------------|
| `Id` | `Guid` | Mapper |
| `PromotorNombre` | `string` | Handler desde `Promotor.NombrePublico` |
| `EsAprobado` | `bool` | Mapper (siempre `false`) |
| `FechaBaja` | `DateTime` | Mapper |

#### InscripcionListItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionListItemDto.cs`

| Propiedad | Tipo C# | Nullable | Asignacion |
|-----------|---------|----------|------------|
| `Id` | `Guid` | No | Mapper |
| `PromotorId` | `Guid` | No | Handler desde `Promotor.Id.Value` |
| `PromotorNombre` | `string` | No | Handler desde `Promotor.NombrePublico` |
| `TipoPromotorNombre` | `string` | No | Handler via diccionario |
| `PromotorEmailContacto` | `string?` | Si | Handler desde `Promotor.EmailContacto` |
| `PromotorUrlInstagram` | `string?` | Si | Handler desde `Promotor.UrlInstagram` |
| `PromotorUrlTikTok` | `string?` | Si | Handler desde `Promotor.UrlTikTok` |
| `PromotorUrlSitioWeb` | `string?` | Si | Handler desde `Promotor.UrlSitioWeb` |
| `EsAprobado` | `bool` | No | Mapper |
| `EsBloqueado` | `bool` | No | Mapper |
| `CodigoReferido` | `string?` | Si | Handler: solo si `EsAprobado == true` |
| `FechaAlta` | `DateTime` | No | Mapper desde `FechaInscripcion` |
| `FechaBaja` | `DateTime?` | Si | Mapper |
| `Estado` | `string` | No | Handler calculado |

#### InscripcionListResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionListResultDto.cs`

| Propiedad | Tipo C# |
|-----------|---------|
| `Items` | `List<InscripcionListItemDto>` |
| `TotalCount` | `int` |
| `Page` | `int` |
| `PageSize` | `int` |
| `TotalPages` | `int` |

---

## 7. AutoMapper Profile

### 7.1 InscripcionProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/InscripcionProfile.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Mapping`

**Extiende:** `Profile`

Todos los campos que el handler asigna manualmente deben estar con `.ForMember(... opt.Ignore())`.

#### Mapeos a definir:

**1. PromoProgramaPromotor -> InscripcionCreadaDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.ProgramaId, opt => opt.MapFrom(src => src.ProgramaId.Value))
.ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
.ForMember(dest => dest.ProgramaTitulo, opt => opt.Ignore())  // Handler asigna
```

**2. PromoProgramaPromotor -> InscripcionAprobadaDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.MapFrom(src => src.UrlReferido))
.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())  // Handler asigna
```

**3. PromoProgramaPromotor -> InscripcionBloqueadaDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())  // Handler asigna
```

**4. PromoProgramaPromotor -> InscripcionDadaDeBajaDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())  // Handler asigna
.ForMember(dest => dest.FechaBaja, opt => opt.MapFrom(src => src.FechaBaja!.Value))
```

**5. PromoProgramaPromotor -> InscripcionListItemDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
// Todos los campos de Promotor -> Ignore() (handler asigna desde Include):
.ForMember(dest => dest.PromotorId, opt => opt.Ignore())
.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())
.ForMember(dest => dest.TipoPromotorNombre, opt => opt.Ignore())
.ForMember(dest => dest.PromotorEmailContacto, opt => opt.Ignore())
.ForMember(dest => dest.PromotorUrlInstagram, opt => opt.Ignore())
.ForMember(dest => dest.PromotorUrlTikTok, opt => opt.Ignore())
.ForMember(dest => dest.PromotorUrlSitioWeb, opt => opt.Ignore())
.ForMember(dest => dest.Estado, opt => opt.Ignore())
.ForMember(dest => dest.CodigoReferido, opt => opt.Ignore())
```

**6. PromoProgramaPromotor -> MiInscripcionDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
.ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))
// Campos que el handler asigna condicionalmente o desde Programa:
.ForMember(dest => dest.ProgramaId, opt => opt.Ignore())
.ForMember(dest => dest.ProgramaTitulo, opt => opt.Ignore())
.ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
.ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
.ForMember(dest => dest.ImporteComisionPorcentaje, opt => opt.Ignore())
.ForMember(dest => dest.ImporteComisionFija, opt => opt.Ignore())
.ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
.ForMember(dest => dest.CodigoReferido, opt => opt.Ignore())
.ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.Ignore())
.ForMember(dest => dest.Estado, opt => opt.Ignore())
.ForMember(dest => dest.Tareas, opt => opt.Ignore())
```

**7. PromoPrograma -> ProgramaExplorarItemDto**
```
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
.ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => src.FechaInicio))
.ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => src.FechaFin))
// Todos los campos calculados -> Ignore():
.ForMember(dest => dest.ArtistaNombre, opt => opt.Ignore())
.ForMember(dest => dest.TipoPromoNombre, opt => opt.Ignore())
.ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
.ForMember(dest => dest.NumeroTareas, opt => opt.Ignore())
.ForMember(dest => dest.CampaniaTitulo, opt => opt.Ignore())
.ForMember(dest => dest.MiEstado, opt => opt.Ignore())
```

**8. PromoTarea -> TareaResumenDto**
```
.ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())  // Handler via diccionario
.ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())  // Handler via diccionario
```

---

## 8. Estructura de Archivos

```
src/api/Modules/Crowdpromotion/
└── WePlayRises.Crowdpromotion.Application/
    ├── Features/
    │   └── Inscripcion/
    │       ├── Commands/
    │       │   ├── SolicitarInscripcionCommand.cs      (Command + Handler)
    │       │   ├── AprobarInscripcionCommand.cs        (Command + Handler)
    │       │   ├── RechazarInscripcionCommand.cs       (Command + Handler)
    │       │   ├── BloquearInscripcionCommand.cs       (Command + Handler)
    │       │   └── DarDeBajaInscripcionCommand.cs      (Command + Handler)
    │       ├── Queries/
    │       │   ├── ExplorarProgramasQuery.cs           (Query + Handler)
    │       │   ├── GetInscripcionesProgramaQuery.cs    (Query + Handler)
    │       │   └── GetMisInscripcionesQuery.cs         (Query + Handler)
    │       └── Validators/
    │           ├── SolicitarInscripcionCommandValidator.cs
    │           ├── AprobarInscripcionCommandValidator.cs
    │           ├── RechazarInscripcionCommandValidator.cs
    │           ├── BloquearInscripcionCommandValidator.cs
    │           └── DarDeBajaInscripcionCommandValidator.cs
    ├── Dtos/
    │   ├── ProgramaExplorarItemDto.cs
    │   ├── ExplorarProgramasResultDto.cs
    │   ├── InscripcionCreadaDto.cs
    │   ├── InscripcionAprobadaDto.cs
    │   ├── InscripcionRechazadaDto.cs
    │   ├── InscripcionBloqueadaDto.cs
    │   ├── InscripcionDadaDeBajaDto.cs
    │   ├── InscripcionListItemDto.cs
    │   ├── InscripcionListResultDto.cs
    │   ├── MiInscripcionDto.cs
    │   ├── TareaResumenDto.cs
    │   └── MisInscripcionesResultDto.cs
    ├── Interfaces/
    │   └── Services/
    │       └── IInscripcionService.cs                   (NUEVO)
    └── Mapping/
        └── InscripcionProfile.cs                        (NUEVO)
```

---

## 9. Imports Requeridos por Archivo

**Todos los archivos de Commands y Queries requieren:**
```csharp
using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;  // CRITICO: ServiceResponseMessageType
using WePlayRises.Crowdpromotion.Domain.Model;
```

**Todos los Validators requieren:**
```csharp
using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;  // CRITICO: ServiceResponseMessageType
```

**InscripcionProfile requiere:**
```csharp
using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;
```

---

## 10. Patrones de Negocio en Handlers

### 10.1 Calculo del Estado de Inscripcion

Este calculo aparece en tres handlers (GetInscripcionesProgramaQuery, GetMisInscripciones, ExplorarProgramas). La prioridad es siempre la misma:

```
string CalcularEstado(PromoProgramaPromotor inscripcion)
{
    if (inscripcion.EsBloqueado) return "Bloqueado";
    if (inscripcion.FechaBaja != null) return "DadoDeBaja";
    if (inscripcion.EsAprobado) return "Aprobado";
    return "Pendiente";
}
```

Se puede extraer como metodo privado estatico en cada handler para no duplicar logica.

### 10.2 Generacion de CodigoReferido

Ocurre SOLO en `AprobarInscripcionCommandHandler`. La logica de reintento es responsabilidad del Handler:

```
// En AprobarInscripcionCommandHandler.Handle():
var codigoBase = string.IsNullOrEmpty(programa.CodigoTrackingBase)
    ? request.ProgramaId.ToString("N")[..8]
    : programa.CodigoTrackingBase;

string? codigoReferido = null;
const int maxIntentos = 3;
for (int intento = 0; intento < maxIntentos && codigoReferido == null; intento++)
{
    var shortId = Guid.NewGuid().ToString("N")[..5];
    var candidato = $"{codigoBase}-{shortId}";
    if (!await _inscripcionService.CodigoReferidoExistsAsync(candidato, ct))
        codigoReferido = candidato;
}
if (codigoReferido == null)
    return InternalServerError("No se pudo generar un codigo unico");
```

### 10.3 Construccion de UrlTrackingPersonalizada

Ocurre SOLO en `AprobarInscripcionCommandHandler.Handle()` tras generar el codigo:

```
string? urlTracking = null;
if (!string.IsNullOrEmpty(programa.UrlLanding))
{
    urlTracking = $"{programa.UrlLanding}?utm_source=weplay&utm_medium=referral" +
                  $"&utm_campaign={codigoBase}&ref={codigoReferido}";
}
```

### 10.4 Resolucion de Nombre del Artista (Cross-Context)

Para `ExplorarProgramasQuery` y `GetMisInscripcionesQuery`, el campo `ArtistaNombre` viene del modulo UserAccess. El patron ya establecido en `PromoProgramaService.GetArtistaIdByUserIdAsync` usa `SqlQueryRaw` o similar. `IInscripcionService` debe exponer un metodo `GetArtistaNombreByIdAsync(ArtistaId id, CancellationToken ct)` para que el handler pueda resolver el nombre sin depender del contexto de UserAccess directamente.

---

## 11. Checklist de Implementacion

### Commands

- [ ] `SolicitarInscripcionCommand.cs` - Command + Handler en mismo archivo
- [ ] `AprobarInscripcionCommand.cs` - Command + Handler en mismo archivo
- [ ] `RechazarInscripcionCommand.cs` - Command + Handler en mismo archivo
- [ ] `BloquearInscripcionCommand.cs` - Command + Handler en mismo archivo
- [ ] `DarDeBajaInscripcionCommand.cs` - Command + Handler en mismo archivo

### Queries

- [ ] `ExplorarProgramasQuery.cs` - Query + Handler en mismo archivo
- [ ] `GetInscripcionesProgramaQuery.cs` - Query + Handler en mismo archivo
- [ ] `GetMisInscripcionesQuery.cs` - Query + Handler en mismo archivo

### Validators

- [ ] `SolicitarInscripcionCommandValidator.cs` - en carpeta `Validators/`
- [ ] `AprobarInscripcionCommandValidator.cs` - en carpeta `Validators/`
- [ ] `RechazarInscripcionCommandValidator.cs` - en carpeta `Validators/`
- [ ] `BloquearInscripcionCommandValidator.cs` - en carpeta `Validators/`
- [ ] `DarDeBajaInscripcionCommandValidator.cs` - en carpeta `Validators/`

### DTOs (12 archivos)

- [ ] `ProgramaExplorarItemDto.cs`
- [ ] `ExplorarProgramasResultDto.cs`
- [ ] `InscripcionCreadaDto.cs`
- [ ] `InscripcionAprobadaDto.cs`
- [ ] `InscripcionRechazadaDto.cs`
- [ ] `InscripcionBloqueadaDto.cs`
- [ ] `InscripcionDadaDeBajaDto.cs`
- [ ] `InscripcionListItemDto.cs`
- [ ] `InscripcionListResultDto.cs`
- [ ] `MiInscripcionDto.cs`
- [ ] `TareaResumenDto.cs`
- [ ] `MisInscripcionesResultDto.cs`

### Mapping y Service Interface

- [ ] `InscripcionProfile.cs` - en `Mapping/`
- [ ] `IInscripcionService.cs` - en `Interfaces/Services/`

### Reglas CQRS verificadas

- [ ] Todos los Handlers retornan `ServiceResponse<T>`
- [ ] Handler + Command/Query en MISMO archivo
- [ ] Ninguna dependencia de `CrowdpromotionContext` en Handlers
- [ ] `?? throw new ArgumentNullException` en TODOS los constructores de Handlers
- [ ] Validators usan `ServiceResponseMessageType.X` (NO strings literales)
- [ ] Handlers usan `ServiceResponseMessageType.X` en respuestas de error
- [ ] Try-catch con `_logger.LogError` en todos los Handlers
- [ ] Validators en carpeta `Validators/` separada
- [ ] Validators NO tienen dependencias de IInscripcionService (logica de negocio en Handler)
- [ ] `RechazarInscripcionCommandHandler` guarda datos ANTES del DELETE fisico
- [ ] Generacion de CodigoReferido con 3 reintentos en `AprobarInscripcionCommandHandler`
- [ ] Estado de inscripcion calculado en Handler con prioridad: Bloqueado > DadoDeBaja > Aprobado > Pendiente
- [ ] CodigoReferido y UrlTrackingPersonalizada SOLO expuestos en estado aprobado activo (`EsAprobado == true && FechaBaja == null`)
