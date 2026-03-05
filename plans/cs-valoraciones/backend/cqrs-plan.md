# Plan CQRS: cs-valoraciones

**Fecha:** 2026-02-21
**Modulo:** Crowdsourcing
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response | Auth |
|-----------|------|---------|----------|------|
| Crear valoracion sobre acuerdo completado | Command | `CreateValoracionCommand` | `ServiceResponse<ValoracionCreatedResultDto>` | Participante del acuerdo |
| Obtener valoraciones recibidas por usuario | Query | `GetValoracionesByUserQuery` | `ServiceResponse<ValoracionesUsuarioDto>` | Cualquier usuario autenticado |

---

## 2. Constante Nueva en ServiceResponseMessageType

**Archivo a modificar:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

Agregar al final de la seccion Business Rules (4000-4999), despues de `BusinessRule_NoRelacionConDestinatario = "4016"`:

```csharp
public const string BusinessRule_DuplicateAction = "4017";  // Ya existe valoracion del mismo autor para este acuerdo
```

**Constantes existentes a reutilizar en esta feature (NO crear nuevas):**

| Constante | Codigo | Uso |
|-----------|--------|-----|
| `Validation_Required` | `"1001"` | Puntuacion == 0, AcuerdoId vacio, UserId vacio |
| `Validation_MaxLength` | `"1002"` | Comentario > 1000 chars |
| `Validation_InvalidRange` | `"1009"` | Puntuacion fuera del rango 1-5 |
| `NotFound_Entity` | `"2000"` | Usuario no encontrado en Identity |
| `NotFound_Acuerdo` | `"2011"` | acuerdoId no existe en DB |
| `Auth_Unauthorized` | `"3001"` | Token JWT invalido/expirado |
| `Auth_Forbidden` | `"3002"` | Usuario no es participante del acuerdo |
| `BusinessRule_InvalidState` | `"4014"` | Acuerdo no esta en estado Completado |
| `BusinessRule_DuplicateAction` | `"4017"` | **NUEVA** - Ya existe valoracion para ese (AcuerdoId, UserIdAutor) |
| `Internal_UnexpectedError` | `"5000"` | Excepcion no controlada |
| `Created` | `"0001"` | Valoracion creada exitosamente |

---

## 3. Commands

### 3.1 CreateValoracionCommand

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Commands/CreateValoracionCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo (REGLA 2 CQRS)

**Implementa:** `IRequest<ServiceResponse<ValoracionCreatedResultDto>>`

#### Command

| Propiedad | Tipo | Origen | Requerido | Descripcion |
|-----------|------|--------|-----------|-------------|
| `AcuerdoId` | `Guid` | Path param (`[FromRoute]`) | Si | ID del acuerdo. Asignado por el controller |
| `UserId` | `string` | JWT claim | Si | Identity UserId del autor. Extraido del token por el controller |
| `Puntuacion` | `int` | Body | Si | Puntuacion de 1 a 5 estrellas |
| `Comentario` | `string?` | Body | No | Comentario opcional, max 1000 chars |

```csharp
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateValoracionCommand : IRequest<ServiceResponse<ValoracionCreatedResultDto>>
{
    // Del path param - asignado por el controller desde [FromRoute]
    public Guid AcuerdoId { get; set; }

    // Del JWT claim - asignado por el controller via ClaimTypes.NameIdentifier
    public string UserId { get; set; } = string.Empty;

    // Del request body
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}
```

#### Handler

**Dependencias:**

| Dependencia | Interfaz | Motivo |
|-------------|----------|--------|
| `_valoracionService` | `IValoracionCrowdsourcingService` | Crear valoracion, verificar unicidad |
| `_acuerdoService` | `IAcuerdoCrowdsourcingService` | Obtener acuerdo para validar estado y participacion |
| `_artistaService` | `IArtistaService` | Resolver ArtistaId por UserId para determinar participante/valorado |
| `_validator` | `IValidator<CreateValoracionCommand>` | Validacion de formato de entrada |
| `_logger` | `ILogger<CreateValoracionCommandHandler>` | Logging obligatorio en try-catch |

**Flujo completo del Handle:**

```
1.  Validar con IValidator<CreateValoracionCommand> (formato de entrada)
    └── Si invalido -> retornar ServiceResponse con validationResult.GetServiceResponseMessages()

2.  Obtener acuerdo via _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
    └── Si acuerdo == null -> ValidateExtensions.NotFoundServiceResponse(NotFound_Acuerdo)

3.  Resolver participacion del usuario:
    a. artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
    b. esArtista = artista != null && acuerdo.ArtistaId == artista.Id
    c. esProveedor = acuerdo.UserIdProveedor == request.UserId
    └── Si !esArtista && !esProveedor -> ValidateExtensions.ForbiddenServiceResponse(Auth_Forbidden)

4.  Verificar que acuerdo.EstadoAcuerdoId == EstadoAcuerdoConstants.Completado
    └── Si no -> ServiceResponse con BusinessRule_InvalidState

5.  Verificar unicidad via _valoracionService.ExisteValoracionAsync(request.AcuerdoId, request.UserId, ct)
    └── Si existe -> ServiceResponse con BusinessRule_DuplicateAction

6.  Determinar UserIdValorado (logica de negocio - NUNCA desde el cliente):
    - Si esArtista -> UserIdValorado = acuerdo.UserIdProveedor
    - Si esProveedor -> UserIdValorado = userId del artista del acuerdo
      (necesita resolver el UserId del Artista: artistaDelAcuerdo = await _artistaService.GetByIdAsync(acuerdo.ArtistaId, ct))

7.  Construir la entidad ValoracionCrowdsourcing:
    entity.Id = Guid.NewGuid()
    entity.AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId)
    entity.UserIdAutor = request.UserId
    entity.UserIdValorado = userIdValorado (calculado en paso 6)
    entity.Puntuacion = (byte)request.Puntuacion
    entity.Comentario = request.Comentario
    entity.TipoValoracionId = null  (nullable en MVP)
    entity.FechaCreacion = DateTime.UtcNow

8.  Persistir via _valoracionService.CreateAsync(entity, ct)

9.  Construir ValoracionCreatedResultDto:
    dto.Id = entity.Id
    dto.Puntuacion = (int)entity.Puntuacion
    dto.Comentario = entity.Comentario
    dto.FechaCreacion = entity.FechaCreacion

10. Retornar ServiceResponse exitoso con Data y Message "Valoracion enviada. Gracias por tu feedback."
    con HttpStatusCode = System.Net.HttpStatusCode.Created
```

**Manejo especial de race condition (segunda linea de defensa):**

Si el handler pasa la verificacion en memoria pero la BD viola el constraint unico, EF lanza `DbUpdateException`. El handler captura este caso especifico antes del catch general y lo convierte en `BusinessRule_DuplicateAction` (no en `Internal_UnexpectedError`).

```csharp
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND (ver arriba)
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateValoracionCommandHandler
    : IRequestHandler<CreateValoracionCommand, ServiceResponse<ValoracionCreatedResultDto>>
{
    private readonly IValoracionCrowdsourcingService _valoracionService;
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<CreateValoracionCommand> _validator;
    private readonly ILogger<CreateValoracionCommandHandler> _logger;

    // CRITICO: ?? throw para TODAS las dependencias
    public CreateValoracionCommandHandler(
        IValoracionCrowdsourcingService valoracionService,
        IAcuerdoCrowdsourcingService acuerdoService,
        IArtistaService artistaService,
        IValidator<CreateValoracionCommand> validator,
        ILogger<CreateValoracionCommandHandler> logger)
    {
        _valoracionService = valoracionService
            ?? throw new ArgumentNullException(nameof(valoracionService));
        _acuerdoService = acuerdoService
            ?? throw new ArgumentNullException(nameof(acuerdoService));
        _artistaService = artistaService
            ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator
            ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ValoracionCreatedResultDto>> Handle(
        CreateValoracionCommand request,
        CancellationToken ct)
    {
        try
        {
            // Paso 1: Validacion de formato
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Paso 2: Obtener acuerdo
            var acuerdo = await _acuerdoService.GetByIdAsync(
                new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ValoracionCreatedResultDto>(
                    "Acuerdo no encontrado",
                    ServiceResponseMessageType.NotFound_Acuerdo);
            }

            // Paso 3: Resolver participacion
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var esArtista = artista != null && acuerdo.ArtistaId == artista.Id;
            var esProveedor = acuerdo.UserIdProveedor == request.UserId;

            if (!esArtista && !esProveedor)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to create valoracion on acuerdo {AcuerdoId} without being a participant",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<ValoracionCreatedResultDto>(
                    "No eres participante de este acuerdo",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            // Paso 4: Verificar estado Completado
            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Completado)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Solo se puede valorar acuerdos completados",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_InvalidState
                        }
                    }
                };
            }

            // Paso 5: Verificar unicidad (primera linea de defensa)
            var yaValoro = await _valoracionService.ExisteValoracionAsync(
                request.AcuerdoId, request.UserId, ct);
            if (yaValoro)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Ya has dejado una valoracion para este acuerdo",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_DuplicateAction
                        }
                    }
                };
            }

            // Paso 6: Determinar UserIdValorado (logica de negocio en el Handler)
            string userIdValorado;
            if (esArtista)
            {
                // El artista valora al proveedor
                userIdValorado = acuerdo.UserIdProveedor;
            }
            else
            {
                // El proveedor valora al artista
                // Necesitamos el UserId del artista, que obtemos via IArtistaService
                var artistaDelAcuerdo = await _artistaService.GetByIdAsync(acuerdo.ArtistaId, ct);
                if (artistaDelAcuerdo == null)
                {
                    _logger.LogError(
                        "ArtistaId {ArtistaId} not found when resolving UserIdValorado for acuerdo {AcuerdoId}",
                        acuerdo.ArtistaId, request.AcuerdoId);
                    return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionCreatedResultDto>(
                        "Error al resolver el usuario valorado",
                        ServiceResponseMessageType.Internal_UnexpectedError);
                }
                userIdValorado = artistaDelAcuerdo.UserId;
            }

            // Paso 7: Construir entidad (logica de negocio en el Handler, NO en el Service)
            var entity = new ValoracionCrowdsourcing
            {
                Id = Guid.NewGuid(),
                AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId),
                UserIdAutor = request.UserId,
                UserIdValorado = userIdValorado,
                Puntuacion = (byte)request.Puntuacion,
                Comentario = request.Comentario,
                TipoValoracionId = null,    // Nullable en MVP
                FechaCreacion = DateTime.UtcNow
            };

            // Paso 8: Persistir via Service (Service delega al Repository - NUNCA DbContext en Handler)
            await _valoracionService.CreateAsync(entity, ct);

            // Paso 9: Construir DTO de resultado
            var resultDto = new ValoracionCreatedResultDto
            {
                Id = entity.Id,
                Puntuacion = (int)entity.Puntuacion,
                Comentario = entity.Comentario,
                FechaCreacion = entity.FechaCreacion
            };

            // Paso 10: Retornar ServiceResponse exitoso con Created
            return new ServiceResponse<ValoracionCreatedResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Valoracion enviada. Gracias por tu feedback.",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (DbUpdateException dbEx)
            when (dbEx.InnerException?.Message.Contains("UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor") == true)
        {
            // Segunda linea de defensa: race condition - dos requests simultaneas del mismo usuario
            _logger.LogWarning(dbEx,
                "Constraint unicidad violado al crear valoracion (race condition). AcuerdoId={AcuerdoId}, UserId={UserId}",
                request.AcuerdoId, request.UserId);
            return new ServiceResponse<ValoracionCreatedResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Ya has dejado una valoracion para este acuerdo",
                        ErrorCode = ServiceResponseMessageType.BusinessRule_DuplicateAction
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating valoracion. AcuerdoId={AcuerdoId}, UserId={UserId}",
                request.AcuerdoId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionCreatedResultDto>(
                "Error inesperado al crear la valoracion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

## 4. Queries

### 4.1 GetValoracionesByUserQuery

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Queries/GetValoracionesByUserQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo (REGLA 2 CQRS)

**Implementa:** `IRequest<ServiceResponse<ValoracionesUsuarioDto>>`

#### Query

| Propiedad | Tipo | Origen | Default | Descripcion |
|-----------|------|--------|---------|-------------|
| `UserId` | `string` | Path param | - | Identity UserId del usuario consultado |
| `Page` | `int` | Query param | 1 | Numero de pagina, 1-based |
| `PageSize` | `int` | Query param | 10 | Items por pagina, max 50 |

```csharp
public class GetValoracionesByUserQuery : IRequest<ServiceResponse<ValoracionesUsuarioDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

#### Handler

**Dependencias:**

| Dependencia | Interfaz | Motivo |
|-------------|----------|--------|
| `_valoracionService` | `IValoracionCrowdsourcingService` | Verificar usuario, obtener resumen y listado paginado |
| `_artistaService` | `IArtistaService` | Resolver AutorNombre: NombreArtistico si el autor es artista |
| `_perfilService` | `IPerfilProfesionalService` | Resolver AutorNombre: Titulo si el autor es profesional |
| `_validator` | `IValidator<GetValoracionesByUserQuery>` | Validacion de parametros de entrada |
| `_logger` | `ILogger<GetValoracionesByUserQueryHandler>` | Logging obligatorio en try-catch |

**Nota sobre AutoMapper en este handler:** El handler NO usa AutoMapper para el mapeo de `ValoracionListItemDto` porque los campos `AutorNombre`, `AutorImagenUrl` y `AcuerdoTituloInterno` requieren consultar otros servicios. El service devuelve directamente `PaginatedResponse<ValoracionListItemDto>` con la proyeccion ya resuelta, segun la interfaz del servicio.

**Flujo completo del Handle:**

```
1.  Validar con IValidator<GetValoracionesByUserQuery>
    └── Si invalido -> retornar ServiceResponse con validationResult.GetServiceResponseMessages()

2.  Verificar que el usuario existe:
    _valoracionService.UsuarioExisteAsync(query.UserId, ct)
    └── Si false -> ValidateExtensions.NotFoundServiceResponse(NotFound_Entity)

3.  Calcular pageSize efectivo: Math.Min(query.PageSize, 50)

4.  Obtener resumen estadistico:
    resumenDto = await _valoracionService.GetResumenByUserIdAsync(query.UserId, ct)
    (AVG, COUNT, GROUP BY ejecutados en SQL - nunca en memoria)

5.  Obtener listado paginado:
    valoracionesPaginadas = await _valoracionService.GetByUserIdPagedAsync(
        query.UserId, query.Page, pageSize, ct)
    (El service/repository ya proyecta AutorNombre, AutorImagenUrl, AcuerdoTituloInterno)
    (SKIP/TAKE + ORDER BY FechaCreacion DESC ejecutados en SQL)

6.  Componer ValoracionesUsuarioDto:
    dto.Resumen = resumenDto
    dto.Valoraciones = valoracionesPaginadas

7.  Retornar ServiceResponse exitoso con Data
```

**Nota sobre resolucion de AutorNombre en GetByUserIdPagedAsync:**
La resolucion de `AutorNombre` y `AutorImagenUrl` se hace en el Service (no en el repository ni en AutoMapper). El `IValoracionCrowdsourcingService.GetByUserIdPagedAsync` recibe las entidades del repository y luego resuelve el nombre del autor usando `IArtistaService` y `IPerfilProfesionalService` inyectados en el service. Esto sigue el patron observado en `GetMensajesQueryHandler` donde se usa un `nombreCache` local para evitar queries N+1.

**Alternativa analizada:** Dado que el service de infraestructura deberia retornar entidades (REGLA 4 CQRS) y la logica de enriquecimiento pertenece a la capa Application, esta resolucion se puede hacer en el Handler con el mismo patron de `nombreCache` que usa `GetMensajesQueryHandler`. Ver la seccion 8.3 del presente plan para el diseno alternativo donde el handler mismo resuelve los nombres.

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetValoracionesByUserQuery : IRequest<ServiceResponse<ValoracionesUsuarioDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetValoracionesByUserQueryHandler
    : IRequestHandler<GetValoracionesByUserQuery, ServiceResponse<ValoracionesUsuarioDto>>
{
    private readonly IValoracionCrowdsourcingService _valoracionService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IValidator<GetValoracionesByUserQuery> _validator;
    private readonly ILogger<GetValoracionesByUserQueryHandler> _logger;

    // CRITICO: ?? throw para TODAS las dependencias
    public GetValoracionesByUserQueryHandler(
        IValoracionCrowdsourcingService valoracionService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IValidator<GetValoracionesByUserQuery> validator,
        ILogger<GetValoracionesByUserQueryHandler> logger)
    {
        _valoracionService = valoracionService
            ?? throw new ArgumentNullException(nameof(valoracionService));
        _artistaService = artistaService
            ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService
            ?? throw new ArgumentNullException(nameof(perfilService));
        _validator = validator
            ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ValoracionesUsuarioDto>> Handle(
        GetValoracionesByUserQuery request,
        CancellationToken ct)
    {
        try
        {
            // Paso 1: Validacion de parametros
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ValoracionesUsuarioDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Paso 2: Verificar que el usuario existe en Identity
            var usuarioExiste = await _valoracionService.UsuarioExisteAsync(request.UserId, ct);
            if (!usuarioExiste)
            {
                return ValidateExtensions.NotFoundServiceResponse<ValoracionesUsuarioDto>(
                    "Usuario no encontrado",
                    ServiceResponseMessageType.NotFound_Entity);
            }

            // Paso 3: Aplicar limite de pageSize
            var pageSize = Math.Min(request.PageSize, 50);

            // Paso 4: Obtener resumen estadistico (AVG + COUNT + GROUP BY en SQL)
            var resumenDto = await _valoracionService.GetResumenByUserIdAsync(request.UserId, ct);

            // Paso 5: Obtener listado paginado con proyeccion de AutorNombre (SKIP/TAKE en SQL)
            var valoracionesPaginadas = await _valoracionService.GetByUserIdPagedAsync(
                request.UserId, request.Page, pageSize, ct);

            // Paso 6: Componer DTO raiz
            var responseDto = new ValoracionesUsuarioDto
            {
                Resumen = resumenDto,
                Valoraciones = valoracionesPaginadas
            };

            // Paso 7: Retornar ServiceResponse exitoso
            return new ServiceResponse<ValoracionesUsuarioDto>
            {
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error getting valoraciones for user {UserId}. Page={Page}, PageSize={PageSize}",
                request.UserId, request.Page, request.PageSize);
            return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionesUsuarioDto>(
                "Error inesperado al obtener las valoraciones",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

## 5. Validators

### 5.1 CreateValoracionCommandValidator

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Validators/CreateValoracionCommandValidator.cs`

**Nota critica:** Las validaciones de regla de negocio (estado del acuerdo, participacion, unicidad) NO van en el validator. Van en el Handler. El validator solo valida el formato de los datos de entrada.

```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;

public class CreateValoracionCommandValidator : AbstractValidator<CreateValoracionCommand>
{
    // CRITICO: Sin dependencias -> no hay IService inyectado aqui
    // Las validaciones de negocio (existencia acuerdo, participacion, unicidad) van en el Handler
    public CreateValoracionCommandValidator()
    {
        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El identificador del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .GreaterThan(0)
            .WithMessage("La puntuacion es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .InclusiveBetween(1, 5)
            .WithMessage("La puntuacion debe ser entre 1 y 5")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.Comentario)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comentario))
            .WithMessage("El comentario no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
```

**Tabla de reglas del validator:**

| Campo | Regla | Mensaje | ErrorCode Constante | Codigo |
|-------|-------|---------|---------------------|--------|
| `AcuerdoId` | `NotEmpty()` | "El identificador del acuerdo es obligatorio" | `Validation_Required` | `"1001"` |
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` | `"1001"` |
| `Puntuacion` | `GreaterThan(0)` | "La puntuacion es obligatoria" | `Validation_Required` | `"1001"` |
| `Puntuacion` | `InclusiveBetween(1, 5)` | "La puntuacion debe ser entre 1 y 5" | `Validation_InvalidRange` | `"1009"` |
| `Comentario` | `MaximumLength(1000)` cuando no vacio | "El comentario no puede superar los 1000 caracteres" | `Validation_MaxLength` | `"1002"` |

---

### 5.2 GetValoracionesByUserQueryValidator

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Validators/GetValoracionesByUserQueryValidator.cs`

```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;

public class GetValoracionesByUserQueryValidator : AbstractValidator<GetValoracionesByUserQuery>
{
    public GetValoracionesByUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El numero de pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("El tamano de pagina debe ser entre 1 y 50")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

**Tabla de reglas del validator:**

| Campo | Regla | Mensaje | ErrorCode Constante | Codigo |
|-------|-------|---------|---------------------|--------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` | `"1001"` |
| `Page` | `GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `Validation_InvalidRange` | `"1009"` |
| `PageSize` | `InclusiveBetween(1, 50)` | "El tamano de pagina debe ser entre 1 y 50" | `Validation_InvalidRange` | `"1009"` |

---

## 6. AutoMapper Profile

### 6.1 ValoracionProfile

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/ValoracionProfile.cs`

**Nota critica sobre conversion de tipos:** La entidad `ValoracionCrowdsourcing` tiene `Puntuacion` de tipo `byte` (eficiente para rango 1-5). Los DTOs y el Command usan `int`. El Profile debe hacer la conversion explicita con cast.

**Nota critica sobre campos calculados:** `AutorNombre`, `AutorImagenUrl` y `AcuerdoTituloInterno` en `ValoracionListItemDto` NO se mapean via AutoMapper porque requieren consultar `IArtistaService` y `IPerfilProfesionalService`. Estos campos se marcan como `Ignore()` en el Profile y se resuelven en el service o handler.

```csharp
using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class ValoracionProfile : Profile
{
    public ValoracionProfile()
    {
        // Command -> Entity (para CreateValoracionCommand)
        // Los campos Id, AcuerdoId, UserIdValorado, TipoValoracionId y FechaCreacion
        // se asignan manualmente en el Handler (logica de negocio)
        CreateMap<CreateValoracionCommand, ValoracionCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())
            .ForMember(dest => dest.UserIdAutor, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserIdValorado, opt => opt.Ignore())
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (byte)src.Puntuacion))
            .ForMember(dest => dest.TipoValoracionId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Acuerdo, opt => opt.Ignore());

        // Entity -> ValoracionCreatedResultDto (respuesta del POST exitoso)
        // Conversion byte -> int para Puntuacion
        CreateMap<ValoracionCrowdsourcing, ValoracionCreatedResultDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion));

        // Entity -> ValoracionListItemDto (proyeccion parcial para el listado paginado)
        // AutorNombre, AutorImagenUrl y AcuerdoTituloInterno se ignoran aqui;
        // se proyectan manualmente en el Service al construir el listado paginado
        CreateMap<ValoracionCrowdsourcing, ValoracionListItemDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion))
            .ForMember(dest => dest.AutorNombre, opt => opt.Ignore())
            .ForMember(dest => dest.AutorImagenUrl, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoTituloInterno, opt => opt.Ignore());
    }
}
```

**Tabla de mappings:**

| Source | Destination | Tipo de mapping | Notas |
|--------|-------------|-----------------|-------|
| `CreateValoracionCommand` | `ValoracionCrowdsourcing` | Command -> Entity | Ignorar Id, AcuerdoId, UserIdValorado, TipoValoracionId, FechaCreacion, Acuerdo. `UserIdAutor` viene de `UserId`. Conversion `int -> byte` en Puntuacion |
| `ValoracionCrowdsourcing` | `ValoracionCreatedResultDto` | Entity -> DTO (POST result) | Conversion `byte -> int` en Puntuacion. Resto mapea directamente |
| `ValoracionCrowdsourcing` | `ValoracionListItemDto` | Entity -> DTO (listado) | AutorNombre, AutorImagenUrl, AcuerdoTituloInterno ignorados y proyectados manualmente |

---

## 7. DTOs de Respuesta

### 7.1 ValoracionCreatedResultDto

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionCreatedResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Identificador unico de la valoracion creada |
| `Puntuacion` | `int` | Puntuacion enviada (1-5). Viene de byte en la entidad |
| `Comentario` | `string?` | Comentario opcional enviado |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion |

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionCreatedResultDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

### 7.2 ValoracionResumenDto

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionResumenDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PuntuacionMedia` | `decimal?` | AVG(Puntuacion) con 1 decimal. Null si TotalValoraciones == 0 |
| `TotalValoraciones` | `int` | COUNT(*) total de valoraciones recibidas |
| `Distribucion` | `Dictionary<int, int>` | Histograma. Claves 1..5 siempre presentes aunque el valor sea 0 |

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionResumenDto
{
    /// <summary>
    /// AVG(Puntuacion) con 1 decimal. Null si TotalValoraciones == 0.
    /// Calculado en SQL via AVG, nunca en memoria.
    /// </summary>
    public decimal? PuntuacionMedia { get; set; }

    /// <summary>
    /// COUNT(*) total de valoraciones recibidas por el usuario.
    /// </summary>
    public int TotalValoraciones { get; set; }

    /// <summary>
    /// Histograma de distribucion. Claves 1..5 siempre presentes aunque el valor sea 0.
    /// Calculado con GROUP BY Puntuacion COUNT(*) en la query SQL.
    /// </summary>
    public Dictionary<int, int> Distribucion { get; set; } = new();
}
```

### 7.3 ValoracionListItemDto

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionListItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Identificador unico de la valoracion |
| `Puntuacion` | `int` | Puntuacion en estrellas (1-5) |
| `Comentario` | `string?` | Comentario opcional |
| `AutorNombre` | `string` | NombreArtistico si artista, Titulo si profesional. Resuelto en service |
| `AutorImagenUrl` | `string?` | URL imagen de perfil del autor. Null si no tiene |
| `AcuerdoTituloInterno` | `string` | TituloInterno del acuerdo al que pertenece |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion |

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionListItemDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }

    /// <summary>
    /// NombreArtistico si el autor tiene perfil Artista, Titulo del PerfilProfesional
    /// si el autor es profesional. Proyectado manualmente en el Service/Handler,
    /// NO en AutoMapper. Patron igual a MensajeDto.RemitenteNombre en GetMensajesQuery.
    /// </summary>
    public string AutorNombre { get; set; } = null!;

    /// <summary>
    /// URL de imagen de perfil. Null si el autor no tiene imagen configurada.
    /// </summary>
    public string? AutorImagenUrl { get; set; }

    /// <summary>
    /// TituloInterno del AcuerdoCrowdsourcing al que pertenece esta valoracion.
    /// </summary>
    public string AcuerdoTituloInterno { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }
}
```

### 7.4 ValoracionesUsuarioDto

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionesUsuarioDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Resumen` | `ValoracionResumenDto` | Estadisticas agregadas |
| `Valoraciones` | `PaginatedResponse<ValoracionListItemDto>` | Listado paginado. Usa el `PaginatedResponse<T>` existente |

```csharp
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class ValoracionesUsuarioDto
{
    public ValoracionResumenDto Resumen { get; set; } = null!;

    /// <summary>
    /// Usa el PaginatedResponse[T] existente en el modulo (NO crear nuevo PaginatedResult[T]).
    /// Incluye TotalPages calculado automaticamente.
    /// </summary>
    public PaginatedResponse<ValoracionListItemDto> Valoraciones { get; set; } = null!;
}
```

**CRITICO:** Usar `PaginatedResponse<T>` (el tipo que ya existe en `Crowdsourcing.Application/Dtos/PaginatedResponse.cs`), NO crear un nuevo `PaginatedResult<T>`.

---

## 8. Interface del Servicio

### 8.1 IValoracionCrowdsourcingService

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IValoracionCrowdsourcingService.cs`

```csharp
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Interfaces.Services;

public interface IValoracionCrowdsourcingService
{
    /// <summary>
    /// Crea una nueva valoracion. El servicio hace SaveChanges via repositorio.
    /// Retorna el Guid del Id de la valoracion creada.
    /// </summary>
    Task<Guid> CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct);

    /// <summary>
    /// Verifica si ya existe una valoracion del mismo autor para el mismo acuerdo.
    /// Implementa cache de request para evitar query duplicada entre validator y handler.
    /// </summary>
    Task<bool> ExisteValoracionAsync(Guid acuerdoId, string userIdAutor, CancellationToken ct);

    /// <summary>
    /// Verifica si el userId existe en Identity. Usado por el handler del GET.
    /// </summary>
    Task<bool> UsuarioExisteAsync(string userId, CancellationToken ct);

    /// <summary>
    /// Calcula el resumen estadistico: AVG(Puntuacion), COUNT(*) y GROUP BY Puntuacion.
    /// El calculo se realiza en SQL, nunca en memoria.
    /// Retorna ValoracionResumenDto directamente (el service proyecta el DTO).
    /// </summary>
    Task<ValoracionResumenDto> GetResumenByUserIdAsync(string userId, CancellationToken ct);

    /// <summary>
    /// Retorna listado paginado de valoraciones recibidas por el usuario.
    /// Ordenado por FechaCreacion DESC. SKIP/TAKE ejecutado en SQL.
    /// El service proyecta AutorNombre, AutorImagenUrl y AcuerdoTituloInterno
    /// usando IArtistaService y IPerfilProfesionalService.
    /// </summary>
    Task<PaginatedResponse<ValoracionListItemDto>> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct);
}
```

**NOTA IMPORTANTE:** A diferencia de otras interfaces del modulo donde el service retorna entidades, `IValoracionCrowdsourcingService` retorna DTOs (`ValoracionResumenDto`, `PaginatedResponse<ValoracionListItemDto>`) para los metodos de lectura. Esto se justifica porque:

1. `GetResumenByUserIdAsync` devuelve datos calculados (AVG, COUNT, GROUP BY) que no corresponden a ninguna entidad del dominio. Es necesario proyectar directamente a DTO.
2. `GetByUserIdPagedAsync` requiere enriquecimiento con datos de otros servicios (IArtistaService, IPerfilProfesionalService). Este enriquecimiento se centraliza en el service para mantener el handler limpio.

Esta es una excepcion documentada a la REGLA 4 (Services retornan entidades), justificada por la naturaleza de consulta agregada.

---

## 9. Flujo de Datos Completo

### 9.1 CreateValoracion (POST)

```
ValoracionesController.CreateValoracion()
  |  Extrae UserId del JWT (ClaimTypes.NameIdentifier)
  |  Asigna command.AcuerdoId = acuerdoId (path param)
  |  Asigna command.UserId = userId (JWT claim)
  |  _mediator.Send(command)
  v
CreateValoracionCommandHandler.Handle()
  |
  +--> CreateValoracionCommandValidator.ValidateAsync()  [1. Formato]
  |      AcuerdoId NotEmpty (1001)
  |      UserId NotEmpty (1001)
  |      Puntuacion > 0 (1001)
  |      Puntuacion 1-5 (1009)
  |      Comentario MaxLength(1000) si presente (1002)
  |
  +--> IAcuerdoCrowdsourcingService.GetByIdAsync()       [2. Acuerdo existe]
  |      └── Si null: NotFound 2011
  |
  +--> IArtistaService.GetByUserIdAsync()                [3. Resolver participacion]
  |      esArtista = artista != null && acuerdo.ArtistaId == artista.Id
  |      esProveedor = acuerdo.UserIdProveedor == request.UserId
  |      └── Si ninguno: Forbidden 3002
  |
  +--> Verificar EstadoAcuerdo == Completado             [4. Estado]
  |      └── Si no: BusinessRule_InvalidState 4014
  |
  +--> IValoracionCrowdsourcingService.ExisteValoracionAsync()  [5. Unicidad]
  |      └── Si existe: BusinessRule_DuplicateAction 4017
  |
  +--> Calcular UserIdValorado (logica de negocio)       [6. UserIdValorado]
  |      Si esArtista: UserIdValorado = acuerdo.UserIdProveedor
  |      Si esProveedor: IArtistaService.GetByIdAsync(acuerdo.ArtistaId).UserId
  |
  +--> Construir ValoracionCrowdsourcing (entity)        [7. Build entity]
  |      Id = Guid.NewGuid()
  |      AcuerdoId = new AcuerdoCrowdsourcingId(...)
  |      UserIdAutor = request.UserId
  |      UserIdValorado = calculado
  |      Puntuacion = (byte)request.Puntuacion
  |      FechaCreacion = DateTime.UtcNow
  |
  +--> IValoracionCrowdsourcingService.CreateAsync()     [8. Persistir]
  |      Service -> Repository.AddAsync() -> SaveChangesAsync()
  |
  +--> Construir ValoracionCreatedResultDto              [9. DTO resultado]
  |
  +--> Retornar ServiceResponse<ValoracionCreatedResultDto>  [10. Response]
          Message: "Valoracion enviada. Gracias por tu feedback."
          HttpStatusCode: Created (201)

  CATCH DbUpdateException (constraint UQ_AcuerdoId_UserIdAutor):
    └── LogWarning + BusinessRule_DuplicateAction 4017 (race condition)

  CATCH Exception:
    └── LogError + Internal_UnexpectedError 5000
```

### 9.2 GetValoracionesByUser (GET)

```
ValoracionesController.GetValoracionesByUser()
  |  page, pageSize de query params (defaults: 1, 10)
  |  _mediator.Send(query)
  v
GetValoracionesByUserQueryHandler.Handle()
  |
  +--> GetValoracionesByUserQueryValidator.ValidateAsync()  [1. Formato]
  |      UserId NotEmpty (1001)
  |      Page >= 1 (1009)
  |      PageSize 1..50 (1009)
  |
  +--> IValoracionCrowdsourcingService.UsuarioExisteAsync()  [2. Usuario existe]
  |      └── Si false: NotFound 2000
  |
  +--> pageSize = Math.Min(request.PageSize, 50)             [3. Limitar pageSize]
  |
  +--> IValoracionCrowdsourcingService.GetResumenByUserIdAsync()  [4. Resumen]
  |      SQL: AVG(Puntuacion), COUNT(*), GROUP BY Puntuacion
  |      Retorna ValoracionResumenDto
  |      Si no hay valoraciones: PuntuacionMedia=null, Total=0, Distribucion={1:0,...5:0}
  |
  +--> IValoracionCrowdsourcingService.GetByUserIdPagedAsync()    [5. Listado paginado]
  |      SQL: WHERE UserIdValorado=userId ORDER BY FechaCreacion DESC
  |           SKIP((page-1)*pageSize) TAKE(pageSize)
  |      Service enriquece AutorNombre: IArtistaService o IPerfilProfesionalService
  |      Retorna PaginatedResponse<ValoracionListItemDto>
  |
  +--> Construir ValoracionesUsuarioDto                      [6. Componer DTO]
  |      Resumen = resumenDto
  |      Valoraciones = valoracionesPaginadas
  |
  +--> Retornar ServiceResponse<ValoracionesUsuarioDto>      [7. Response]

  CATCH Exception:
    └── LogError + Internal_UnexpectedError 5000
```

---

## 10. Controller

### 10.1 ValoracionesController

**Archivo:**
`src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/ValoracionesController.cs`

**Patron:** Identico a `AcuerdosCrowdsourcingController` con `GetUserId()` via `ClaimTypes.NameIdentifier` y discriminacion de errores por `ErrorCode`.

```csharp
using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing")]
[Authorize]
public class ValoracionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ValoracionesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// POST api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones
    /// Crear valoracion sobre un acuerdo completado. Solo participantes del acuerdo.
    /// </summary>
    [HttpPost("acuerdos/{acuerdoId:guid}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateValoracion(
        [FromRoute] Guid acuerdoId,
        [FromBody] CreateValoracionCommand command,
        CancellationToken ct)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.UserId = userId;

        var response = await _mediator.Send(command, ct);

        if (response.HasErrors)
        {
            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Acuerdo))
                return NotFound(response);

            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden))
                return StatusCode(StatusCodes.Status403Forbidden, response);

            return BadRequest(response);
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET api/crowdsourcing/usuarios/{userId}/valoraciones
    /// Obtener valoraciones recibidas por un usuario. Cualquier usuario autenticado.
    /// </summary>
    [HttpGet("usuarios/{userId}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionesUsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionesUsuarioDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetValoracionesByUser(
        [FromRoute] string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetValoracionesByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query, ct);

        if (response.HasErrors)
        {
            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Entity))
                return NotFound(response);

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        return Ok(response);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
```

---

## 11. Tabla de Errores por Endpoint

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

| HTTP Status | ErrorCode | Constante | Mensaje | Capa que lo genera |
|-------------|-----------|-----------|---------|-------------------|
| 400 | `"1001"` | `Validation_Required` | "La puntuacion es obligatoria" | Validator |
| 400 | `"1001"` | `Validation_Required` | "El identificador del acuerdo es obligatorio" | Validator |
| 400 | `"1001"` | `Validation_Required` | "El identificador de usuario es obligatorio" | Validator |
| 400 | `"1009"` | `Validation_InvalidRange` | "La puntuacion debe ser entre 1 y 5" | Validator |
| 400 | `"1002"` | `Validation_MaxLength` | "El comentario no puede superar los 1000 caracteres" | Validator |
| 400 | `"4014"` | `BusinessRule_InvalidState` | "Solo se puede valorar acuerdos completados" | Handler |
| 400 | `"4017"` | `BusinessRule_DuplicateAction` | "Ya has dejado una valoracion para este acuerdo" | Handler (verificacion + catch DbUpdateException) |
| 401 | `"3001"` | `Auth_Unauthorized` | "Token no valido o expirado" | Middleware JWT |
| 403 | `"3002"` | `Auth_Forbidden` | "No eres participante de este acuerdo" | Handler |
| 404 | `"2011"` | `NotFound_Acuerdo` | "Acuerdo no encontrado" | Handler |
| 500 | `"5000"` | `Internal_UnexpectedError` | "Error inesperado al crear la valoracion" | Handler catch |

### GET /api/crowdsourcing/usuarios/{userId}/valoraciones

| HTTP Status | ErrorCode | Constante | Mensaje | Capa que lo genera |
|-------------|-----------|-----------|---------|-------------------|
| 400 | `"1001"` | `Validation_Required` | "El identificador de usuario es obligatorio" | Validator |
| 400 | `"1009"` | `Validation_InvalidRange` | "El numero de pagina debe ser mayor o igual a 1" | Validator |
| 400 | `"1009"` | `Validation_InvalidRange` | "El tamano de pagina debe ser entre 1 y 50" | Validator |
| 401 | `"3001"` | `Auth_Unauthorized` | "Token no valido o expirado" | Middleware JWT |
| 404 | `"2000"` | `NotFound_Entity` | "Usuario no encontrado" | Handler |
| 500 | `"5000"` | `Internal_UnexpectedError` | "Error inesperado al obtener las valoraciones" | Handler catch |

---

## 12. Estructura de Archivos a Crear

```
src/api/Modules/Crowdsourcing/
│
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs          [MODIFICAR] Agregar BusinessRule_DuplicateAction = "4017"
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── ValoracionCreatedResultDto.cs           [NUEVO]
│   │   ├── ValoracionResumenDto.cs                 [NUEVO]
│   │   ├── ValoracionListItemDto.cs                [NUEVO]
│   │   └── ValoracionesUsuarioDto.cs               [NUEVO]
│   │   (PaginatedResponse<T> ya existe - NO crear nuevo)
│   │
│   ├── Features/
│   │   └── Valoraciones/
│   │       ├── Commands/
│   │       │   └── CreateValoracionCommand.cs      [NUEVO] Command + Handler en mismo archivo
│   │       ├── Queries/
│   │       │   └── GetValoracionesByUserQuery.cs   [NUEVO] Query + Handler en mismo archivo
│   │       └── Validators/
│   │           ├── CreateValoracionCommandValidator.cs   [NUEVO]
│   │           └── GetValoracionesByUserQueryValidator.cs [NUEVO]
│   │
│   ├── Interfaces/
│   │   └── Services/
│   │       └── IValoracionCrowdsourcingService.cs  [NUEVO]
│   │
│   └── Mapping/
│       └── ValoracionProfile.cs                    [NUEVO]
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        └── ValoracionesController.cs               [NUEVO]
```

---

## 13. Decisiones de Arquitectura y Justificaciones

### 13.1 IMapper NO se usa en CreateValoracionCommandHandler

El `CreateValoracionCommandHandler` NO inyecta `IMapper`. La entidad `ValoracionCrowdsourcing` se construye manualmente en el Handler porque:

1. Varios campos (`AcuerdoId`, `UserIdValorado`, `FechaCreacion`) se calculan en el handler con logica de negocio.
2. La conversion `Guid -> AcuerdoCrowdsourcingId` (StronglyTypedId) no es trivial para AutoMapper sin configuracion extra.
3. El `ValoracionCreatedResultDto` se construye directamente desde la entidad ya creada (4 campos simples).

El Profile `ValoracionProfile` define los mappings pero el handler los usa via `new ValoracionCrowdsourcing { ... }` manual. El Profile si se necesita para futuras operaciones de listado si el Handler decidiera usarlo.

### 13.2 IArtistaService se usa en el Handler para resolver UserIdValorado

La logica de determinar quien es el `UserIdValorado` es logica de negocio pura y pertenece al Handler (REGLA 7 CQRS). El Service de infraestructura solo persiste la entidad ya construida. El `GetByIdAsync` de `IArtistaService` ya existente resuelve el `UserId` del artista del acuerdo.

### 13.3 Resolucion de AutorNombre centralizada en el Service

Para `GetByUserIdPagedAsync`, el service de infraestructura inyecta `IArtistaService` y `IPerfilProfesionalService` para resolver `AutorNombre` y `AutorImagenUrl`. Esto es una excepcion al patron "el service solo delega al repository" y se justifica porque:

1. El enriquecimiento es parte de la proyeccion de datos para el listado (no es logica de negocio).
2. El patron ya existe en el modulo: `GetMensajesQueryHandler` resuelve `RemitenteNombre` en el Handler con un `nombreCache` local para evitar N+1.
3. Si el handler resolviera los nombres, necesitaria inyectar `IArtistaService` y `IPerfilProfesionalService` ademas de `IValoracionCrowdsourcingService`, aumentando el numero de dependencias del handler.

**Alternativa equivalente:** El handler resuelve `AutorNombre` con el mismo patron de `nombreCache` que `GetMensajesQueryHandler`, iterando sobre las entidades retornadas por el service. En ese caso, `IValoracionCrowdsourcingService.GetByUserIdPagedAsync` retornaria `(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)` (entidades) en lugar de DTOs. Ambas opciones son validas. El plan actual centraliza en el service para mantener el handler mas simple.

### 13.4 Constraint unico y race condition

El handler verifica unicidad en el paso 5 (primera linea de defensa). Si dos requests simultaneas pasan ese check, EF lanzara `DbUpdateException` al violar el constraint `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor`. El handler captura este caso especifico antes del catch general, retornando `BusinessRule_DuplicateAction` (400) en lugar de `Internal_UnexpectedError` (500). Patron identico al usado en `CreateConversacionCommandHandler`.

---

## 14. Checklist de Implementacion

### Commands y Queries
- [ ] `CreateValoracionCommand` implementa `IRequest<ServiceResponse<ValoracionCreatedResultDto>>`
- [ ] `GetValoracionesByUserQuery` implementa `IRequest<ServiceResponse<ValoracionesUsuarioDto>>`
- [ ] Handler + Command en MISMO archivo (`CreateValoracionCommand.cs`)
- [ ] Handler + Query en MISMO archivo (`GetValoracionesByUserQuery.cs`)

### Constructores (REGLA 8 CQRS)
- [ ] `CreateValoracionCommandHandler` usa `?? throw new ArgumentNullException` para TODAS las dependencias (5 dependencias)
- [ ] `GetValoracionesByUserQueryHandler` usa `?? throw new ArgumentNullException` para TODAS las dependencias (5 dependencias)
- [ ] `ValoracionesController` usa `?? throw new ArgumentNullException` para `IMediator`

### Handlers (REGLAS 3, 7 CQRS)
- [ ] Handler NUNCA inyecta `CrowdsourcingContext` ni `DbContext` de ningun tipo
- [ ] Logica de `UserIdValorado` esta en el Handler (no en Service ni Repository)
- [ ] `EstadoAcuerdoConstants.Completado` usado para verificar estado (no magic number)
- [ ] Try-catch con `_logger.LogError` en ambos Handlers
- [ ] Catch especifico de `DbUpdateException` (race condition) en `CreateValoracionCommandHandler`
- [ ] `ValidateExtensions.NotFoundServiceResponse` usado para 404
- [ ] `ValidateExtensions.ForbiddenServiceResponse` usado para 403
- [ ] `ValidateExtensions.InternalServerErrorServiceResponse` usado para 500

### Validators (REGLAS 5, 9 CQRS)
- [ ] Validators en carpeta `Validators/` separada
- [ ] TODOS los `RuleFor` tienen `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- [ ] Constantes de `ServiceResponseMessageType` (NO strings literales "1001", "1009", etc.)
- [ ] `Validation_InvalidRange = "1009"` usado para Puntuacion fuera de rango (NO crear "1014")
- [ ] `BusinessRule_DuplicateAction = "4017"` agregada al `ServiceResponseMessageType.cs`

### Responses (REGLA 1 CQRS)
- [ ] Todos los retornos son `ServiceResponse<T>`
- [ ] Validacion retorna `ServiceResponse` (no `throw ValidationException`)
- [ ] Message de exito: `"Valoracion enviada. Gracias por tu feedback."` con `HttpStatusCode = Created`
- [ ] `ServiceResponseMessageType.Created` disponible para responses exitosos (ya existe "0001")

### AutoMapper
- [ ] `ValoracionProfile` registrado en el modulo
- [ ] Conversion `int -> byte` para Puntuacion en Command -> Entity
- [ ] Conversion `byte -> int` para Puntuacion en Entity -> DTO
- [ ] `AutorNombre`, `AutorImagenUrl`, `AcuerdoTituloInterno` marcados como `Ignore()` en el Profile

### DTOs
- [ ] 4 nuevos DTOs creados: `ValoracionCreatedResultDto`, `ValoracionResumenDto`, `ValoracionListItemDto`, `ValoracionesUsuarioDto`
- [ ] `PaginatedResponse<T>` existente reutilizado en `ValoracionesUsuarioDto` (NO crear nuevo)
- [ ] `IValoracionCrowdsourcingService` creada en `Application/Interfaces/Services/`

### Controller
- [ ] `ValoracionesController` con ruta base `api/crowdsourcing`
- [ ] POST en `acuerdos/{acuerdoId:guid}/valoraciones`
- [ ] GET en `usuarios/{userId}/valoraciones`
- [ ] `GetUserId()` via `User.FindFirstValue(ClaimTypes.NameIdentifier)`
- [ ] Discriminacion de errores por `ErrorCode` para retornar 404, 403, 400, 500
- [ ] `[Authorize]` en el controller
- [ ] `ProducesResponseType` para todos los status codes posibles

### Constante Nueva
- [ ] `BusinessRule_DuplicateAction = "4017"` agregada a `ServiceResponseMessageType.cs`
- [ ] NO agregar nueva constante `Validation_InvalidRange_Puntuacion` (usar la existente "1009")

---

## 15. Siguiente Paso Sugerido

Con este plan CQRS completo, los siguientes pasos de implementacion en orden son:

1. Agregar constante `BusinessRule_DuplicateAction = "4017"` en `ServiceResponseMessageType.cs`
2. Crear los 4 DTOs en `Application/Dtos/`
3. Crear `IValoracionCrowdsourcingService` en `Application/Interfaces/Services/`
4. Crear `ValoracionProfile` en `Application/Mapping/`
5. Crear `CreateValoracionCommandValidator` en `Features/Valoraciones/Validators/`
6. Crear `GetValoracionesByUserQueryValidator` en `Features/Valoraciones/Validators/`
7. Crear `CreateValoracionCommand.cs` (Command + Handler) en `Features/Valoraciones/Commands/`
8. Crear `GetValoracionesByUserQuery.cs` (Query + Handler) en `Features/Valoraciones/Queries/`
9. Crear `ValoracionesController` en `WebApi/Controllers/`
10. Implementar `ValoracionCrowdsourcingService` e `IValoracionCrowdsourcingService` en `Infra/` (segun plan hexagonal)
