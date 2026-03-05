# Plan CQRS: Gestionar Necesidades de Crowdsourcing

**Fecha:** 2026-02-16
**Modulo:** Crowdsourcing
**Feature:** cs-gestionar-necesidades (US-CS-02)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Crear Necesidad | Command | CreateNecesidadCommand | ServiceResponse<NecesidadCreateResultDto> |
| Actualizar Necesidad | Command | UpdateNecesidadCommand | ServiceResponse<NecesidadUpdateResultDto> |
| Cerrar Necesidad | Command | CerrarNecesidadCommand | ServiceResponse<CerrarNecesidadResultDto> |
| Listar Mis Necesidades | Query | GetMisNecesidadesQuery | ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>> |
| Obtener Detalle | Query | GetNecesidadByIdQuery | ServiceResponse<NecesidadCrowdsourcingDto> |

---

## 2. Commands

### 2.1 CreateNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/CreateNecesidadCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command
```csharp
public class CreateNecesidadCommand : IRequest<ServiceResponse<NecesidadCreateResultDto>>
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public Guid ProyectoArtisticoId { get; set; }

    // Asignado desde JWT en Controller
    public Guid ArtistaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<NecesidadCreateResultDto>>`

#### Handler

**Dependencias:**
- `INecesidadCrowdsourcingService` - Para persistencia (con UoW)
- `IMapper` - Para mapping Command -> Entity
- `IValidator<CreateNecesidadCommand>` - Para validacion
- `ILogger<CreateNecesidadCommandHandler>` - Para logging

**Flujo:**
1. **Validar request** con FluentValidation
2. Si invalido, retornar ServiceResponse con errores
3. **Mapear Command -> NecesidadCrowdsourcing** (entity)
4. **Aplicar logica de negocio:**
   - Establecer `EstadoNecesidadId = 1` (Abierta)
   - Establecer `FechaCreacion = DateTime.UtcNow`
   - Convertir IDs a Strongly Typed IDs
5. **Persistir via Service.CreateAsync()** (incluye UoW commit)
6. **Mapear entity -> NecesidadCreateResultDto**
7. **Retornar ServiceResponse exitoso** con DTO
8. Try-catch con logging para errores inesperados

**Ejemplo de implementacion:**
```csharp
public class CreateNecesidadCommandHandler : IRequestHandler<CreateNecesidadCommand, ServiceResponse<NecesidadCreateResultDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateNecesidadCommand> _validator;
    private readonly ILogger<CreateNecesidadCommandHandler> _logger;

    public CreateNecesidadCommandHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        IValidator<CreateNecesidadCommand> validator,
        ILogger<CreateNecesidadCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadCreateResultDto>> Handle(
        CreateNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<NecesidadCreateResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Mapear Command -> Entity
            var entity = _mapper.Map<NecesidadCrowdsourcing>(request);

            // 3. Logica de negocio
            entity.Id = NecesidadCrowdsourcingId.CreateNew();
            entity.ArtistaId = new ArtistaId(request.ArtistaId);
            entity.ProyectoArtisticoId = new ProyectoArtisticoId(request.ProyectoArtisticoId);
            entity.EstadoNecesidadId = 1; // Abierta
            entity.FechaCreacion = DateTime.UtcNow;

            // 4. Persistir via Service
            var id = await _service.CreateAsync(entity, ct);

            // 5. Mapear entity -> DTO resultado
            var resultDto = new NecesidadCreateResultDto
            {
                Id = id.Value,
                Titulo = entity.Titulo,
                EstadoNecesidadId = entity.EstadoNecesidadId,
                EstadoNecesidadNombre = "Abierta",
                FechaCreacion = entity.FechaCreacion
            };

            // 6. Retornar exito
            return new ServiceResponse<NecesidadCreateResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad publicada correctamente",
                        ErrorCode = ServiceResponseMessageType.Created,
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating NecesidadCrowdsourcing for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadCreateResultDto>(
                "Error inesperado al crear necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

### 2.2 UpdateNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/UpdateNecesidadCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command
```csharp
public class UpdateNecesidadCommand : IRequest<ServiceResponse<NecesidadUpdateResultDto>>
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }

    // Asignado desde JWT en Controller para validar ownership
    public Guid ArtistaId { get; set; }
}
```

**Nota:** TipoNecesidadId y ProyectoArtisticoId NO son editables (inmutables)

**Implementa:** `IRequest<ServiceResponse<NecesidadUpdateResultDto>>`

#### Handler

**Dependencias:**
- `INecesidadCrowdsourcingService` - Para obtener entity existente y actualizar
- `IMapper` - Para mapping Command -> Entity
- `IValidator<UpdateNecesidadCommand>` - Para validacion (incluye estado=Abierta)
- `ILogger<UpdateNecesidadCommandHandler>` - Para logging

**Flujo:**
1. **Validar request** (incluye validacion de estado=Abierta en Validator)
2. Si invalido, retornar ServiceResponse con errores
3. **Obtener entidad existente** via Service.GetByIdAsync()
4. Si no existe, retornar NOT_FOUND
5. **Actualizar propiedades editables** (todos excepto TipoNecesidadId y ProyectoArtisticoId)
6. **Establecer FechaActualizacion = DateTime.UtcNow**
7. **Persistir via Service.UpdateAsync()** (incluye UoW commit)
8. **Mapear entity -> NecesidadUpdateResultDto**
9. **Retornar ServiceResponse exitoso**
10. Try-catch con logging

**Ejemplo:**
```csharp
public class UpdateNecesidadCommandHandler : IRequestHandler<UpdateNecesidadCommand, ServiceResponse<NecesidadUpdateResultDto>>
{
    // ... dependencias (igual que Create)

    public async Task<ServiceResponse<NecesidadUpdateResultDto>> Handle(
        UpdateNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar (incluye estado=Abierta)
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<NecesidadUpdateResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Obtener entidad existente
            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var entity = await _service.GetByIdAsync(necesidadId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<NecesidadUpdateResultDto>(
                    "Necesidad no encontrada",
                    ServiceResponseMessageType.NotFound_Necesidad);
            }

            // 3. Actualizar propiedades editables
            entity.Titulo = request.Titulo;
            entity.Descripcion = request.Descripcion;
            entity.ModalidadTrabajoId = request.ModalidadTrabajoId;
            entity.PresupuestoMin = request.PresupuestoMin;
            entity.PresupuestoMax = request.PresupuestoMax;
            entity.MonedaId = request.MonedaId;
            entity.UbicacionCiudad = request.UbicacionCiudad;
            entity.UbicacionPais = request.UbicacionPais;
            entity.FechaLimitePropuestas = request.FechaLimitePropuestas;
            entity.FechaInicioPrevista = request.FechaInicioPrevista;
            entity.FechaActualizacion = DateTime.UtcNow;

            // 4. Persistir
            await _service.UpdateAsync(entity, ct);

            // 5. Mapear resultado
            var resultDto = new NecesidadUpdateResultDto
            {
                Id = entity.Id.Value,
                Titulo = entity.Titulo,
                EstadoNecesidadId = entity.EstadoNecesidadId,
                EstadoNecesidadNombre = "Abierta",
                FechaActualizacion = entity.FechaActualizacion
            };

            // 6. Retornar exito
            return new ServiceResponse<NecesidadUpdateResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad actualizada correctamente",
                        ErrorCode = ServiceResponseMessageType.Updated
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating NecesidadCrowdsourcing {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadUpdateResultDto>(
                "Error inesperado al actualizar necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

### 2.3 CerrarNecesidadCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Commands/CerrarNecesidadCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command
```csharp
public class CerrarNecesidadCommand : IRequest<ServiceResponse<CerrarNecesidadResultDto>>
{
    public Guid Id { get; set; }
    public string? Motivo { get; set; }

    // Asignado desde JWT en Controller para validar ownership
    public Guid ArtistaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<CerrarNecesidadResultDto>>`

#### Handler

**Dependencias:**
- `INecesidadCrowdsourcingService` - Para cerrar necesidad (incluye rechazo propuestas)
- `IMapper` - No usado (no hay mapping complejo)
- `IValidator<CerrarNecesidadCommand>` - Para validacion (incluye estado Abierta/EnProgreso)
- `ILogger<CerrarNecesidadCommandHandler>` - Para logging

**Flujo:**
1. **Validar request** (incluye validacion de estado Abierta/EnProgreso en Validator)
2. Si invalido, retornar ServiceResponse con errores
3. **Llamar Service.CerrarAsync(necesidadId, motivo, ct)** - **TRANSACCION ATOMICA**
   - Service actualiza necesidad (estado=Cerrada, motivo, fecha)
   - Service rechaza propuestas pendientes
   - Service hace UoW.CommitAsync() (todo o nada)
4. Service retorna numero de propuestas rechazadas
5. **Crear CerrarNecesidadResultDto**
6. **Retornar ServiceResponse exitoso**
7. Try-catch con logging

**Ejemplo:**
```csharp
public class CerrarNecesidadCommandHandler : IRequestHandler<CerrarNecesidadCommand, ServiceResponse<CerrarNecesidadResultDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IValidator<CerrarNecesidadCommand> _validator;
    private readonly ILogger<CerrarNecesidadCommandHandler> _logger;

    public CerrarNecesidadCommandHandler(
        INecesidadCrowdsourcingService service,
        IValidator<CerrarNecesidadCommand> validator,
        ILogger<CerrarNecesidadCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CerrarNecesidadResultDto>> Handle(
        CerrarNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            // 1. Validar
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CerrarNecesidadResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Cerrar necesidad (transaccion atomica: necesidad + propuestas)
            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var propuestasRechazadas = await _service.CerrarAsync(necesidadId, request.Motivo, ct);

            // 3. Crear resultado
            var resultDto = new CerrarNecesidadResultDto
            {
                Id = request.Id,
                EstadoNecesidadNombre = "Cerrada",
                PropuestasRechazadas = propuestasRechazadas
            };

            // 4. Retornar exito
            var mensaje = propuestasRechazadas > 0
                ? $"Necesidad cerrada correctamente. Se han rechazado {propuestasRechazadas} propuestas pendientes."
                : "Necesidad cerrada correctamente.";

            return new ServiceResponse<CerrarNecesidadResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = mensaje,
                        ErrorCode = ServiceResponseMessageType.Updated
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing NecesidadCrowdsourcing {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<CerrarNecesidadResultDto>(
                "Error inesperado al cerrar necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

**IMPORTANTE:** El Service maneja la transaccion atomica completa (necesidad + propuestas).

---

## 3. Queries

### 3.1 GetMisNecesidadesQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Queries/GetMisNecesidadesQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
```csharp
public class GetMisNecesidadesQuery : IRequest<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>>
{
    public Guid ArtistaId { get; set; }
    public int? EstadoNecesidadId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
```

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>>`

#### Handler

**Dependencias:**
- `INecesidadCrowdsourcingService` - Para obtener listado paginado
- `IMapper` - Para mapping Entity -> ListDto
- `ILogger<GetMisNecesidadesQueryHandler>` - Para logging

**Flujo:**
1. **Llamar Service.GetByArtistaIdPaginatedAsync()** con filtros (estado, search) y paginacion
2. Service retorna (Items, TotalCount)
3. **Mapear cada entidad -> NecesidadCrowdsourcingListDto** con AutoMapper
4. **Crear PaginatedResponse** con items, totalCount, page, pageSize, totalPages
5. **Retornar ServiceResponse exitoso**
6. Try-catch con logging

**Ejemplo:**
```csharp
public class GetMisNecesidadesQueryHandler : IRequestHandler<GetMisNecesidadesQuery, ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMisNecesidadesQueryHandler> _logger;

    public GetMisNecesidadesQueryHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        ILogger<GetMisNecesidadesQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>> Handle(
        GetMisNecesidadesQuery request,
        CancellationToken ct)
    {
        try
        {
            var artistaId = new ArtistaId(request.ArtistaId);

            var (items, totalCount) = await _service.GetByArtistaIdPaginatedAsync(
                artistaId,
                request.EstadoNecesidadId,
                request.Search,
                request.Page,
                request.PageSize,
                ct);

            var dtos = _mapper.Map<List<NecesidadCrowdsourcingListDto>>(items);

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var paginatedResponse = new PaginatedResponse<NecesidadCrowdsourcingListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>
            {
                Data = paginatedResponse,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidades obtenidas exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidades for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>(
                "Error inesperado al obtener necesidades",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

### 3.2 GetNecesidadByIdQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Queries/GetNecesidadByIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
```csharp
public class GetNecesidadByIdQuery : IRequest<ServiceResponse<NecesidadCrowdsourcingDto>>
{
    public Guid Id { get; set; }

    // Para validar ownership
    public Guid ArtistaId { get; set; }
}
```

**Implementa:** `IRequest<ServiceResponse<NecesidadCrowdsourcingDto>>`

#### Handler

**Dependencias:**
- `INecesidadCrowdsourcingService` - Para obtener entidad con propuestas
- `IMapper` - Para mapping Entity -> DTO completo
- `ILogger<GetNecesidadByIdQueryHandler>` - Para logging

**Flujo:**
1. **Llamar Service.GetByIdWithDetailsAsync()** (incluye propuestas)
2. Si null, retornar NOT_FOUND
3. **Validar ownership:** entity.ArtistaId == request.ArtistaId
4. Si no coincide, retornar FORBIDDEN
5. **Mapear entity -> NecesidadCrowdsourcingDto** (incluye propuestas nested)
6. **Retornar ServiceResponse exitoso**
7. Try-catch con logging

**Ejemplo:**
```csharp
public class GetNecesidadByIdQueryHandler : IRequestHandler<GetNecesidadByIdQuery, ServiceResponse<NecesidadCrowdsourcingDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetNecesidadByIdQueryHandler> _logger;

    public GetNecesidadByIdQueryHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        ILogger<GetNecesidadByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadCrowdsourcingDto>> Handle(
        GetNecesidadByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var entity = await _service.GetByIdWithDetailsAsync(necesidadId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<NecesidadCrowdsourcingDto>(
                    "Necesidad no encontrada",
                    ServiceResponseMessageType.NotFound_Necesidad);
            }

            // Validar ownership
            if (entity.ArtistaId.Value != request.ArtistaId)
            {
                _logger.LogWarning(
                    "Unauthorized access attempt to necesidad {NecesidadId} by artista {ArtistaId}",
                    request.Id, request.ArtistaId);

                return ValidateExtensions.ForbiddenServiceResponse<NecesidadCrowdsourcingDto>(
                    "No tienes permiso para ver esta necesidad",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var dto = _mapper.Map<NecesidadCrowdsourcingDto>(entity);

            return new ServiceResponse<NecesidadCrowdsourcingDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad obtenida exitosamente",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidad {NecesidadId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadCrowdsourcingDto>(
                "Error inesperado al obtener necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

---

## 4. Validators

### 4.1 CreateNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/CreateNecesidadCommandValidator.cs`

**CRITICO:** Usar `ServiceResponseMessageType.X` constants, NO strings literales

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| Titulo | NotEmpty | El título es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| Titulo | MinLength(5) | El título debe tener al menos 5 caracteres | `ServiceResponseMessageType.Validation_MinLength` |
| Titulo | MaxLength(200) | El título no puede superar los 200 caracteres | `ServiceResponseMessageType.Validation_MaxLength` |
| Descripcion | MaxLength(4000) | La descripción no puede superar los 4000 caracteres | `ServiceResponseMessageType.Validation_MaxLength` |
| TipoNecesidadId | NotEmpty | El tipo de necesidad es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| ModalidadTrabajoId | NotEmpty | La modalidad de trabajo es obligatoria | `ServiceResponseMessageType.Validation_Required` |
| PresupuestoMin | GreaterThanOrEqualTo(0) | El presupuesto mínimo no puede ser negativo | `ServiceResponseMessageType.Validation_InvalidRange` |
| PresupuestoMax | GreaterThanOrEqualTo(PresupuestoMin) | El presupuesto máximo debe ser mayor o igual al mínimo | `ServiceResponseMessageType.Validation_InvalidRange` |
| MonedaId | NotEmpty (si presupuesto presente) | La moneda es obligatoria cuando se especifica presupuesto | `ServiceResponseMessageType.Validation_Required` |
| UbicacionCiudad | NotEmpty (si modalidad 1 o 3) | La ubicación (ciudad) es obligatoria para modalidad Presencial o Híbrida | `ServiceResponseMessageType.Validation_Required` |
| UbicacionPais | NotEmpty (si modalidad 1 o 3) | La ubicación (país) es obligatoria para modalidad Presencial o Híbrida | `ServiceResponseMessageType.Validation_Required` |
| FechaLimitePropuestas | GreaterThan(DateTime.UtcNow.Date) | La fecha límite debe ser posterior a hoy | `ServiceResponseMessageType.Validation_InvalidDate` |
| FechaInicioPrevista | GreaterThanOrEqualTo(DateTime.UtcNow.Date) | La fecha de inicio debe ser igual o posterior a hoy | `ServiceResponseMessageType.Validation_InvalidDate` |
| ProyectoArtisticoId | NotEmpty | El proyecto artístico es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| ProyectoArtisticoId | MustAsync (existe + ownership) | El proyecto artístico no existe o no te pertenece | `ServiceResponseMessageType.Auth_Forbidden` |

**Ejemplo de implementacion:**
```csharp
public class CreateNecesidadCommandValidator : AbstractValidator<CreateNecesidadCommand>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IProyectoArtisticoService _proyectoService;

    public CreateNecesidadCommandValidator(
        INecesidadCrowdsourcingService necesidadService,
        IProyectoArtisticoService proyectoService)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _proyectoService = proyectoService ?? throw new ArgumentNullException(nameof(proyectoService));

        // Titulo
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El título es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(5)
            .WithMessage("El título debe tener al menos 5 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El título no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // Descripcion
        RuleFor(x => x.Descripcion)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrEmpty(x.Descripcion))
            .WithMessage("La descripción no puede superar los 4000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // TipoNecesidadId
        RuleFor(x => x.TipoNecesidadId)
            .NotEmpty()
            .WithMessage("El tipo de necesidad es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // ModalidadTrabajoId
        RuleFor(x => x.ModalidadTrabajoId)
            .NotEmpty()
            .WithMessage("La modalidad de trabajo es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // Presupuesto
        RuleFor(x => x.PresupuestoMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PresupuestoMin.HasValue)
            .WithMessage("El presupuesto mínimo no puede ser negativo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PresupuestoMax)
            .GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0)
            .When(x => x.PresupuestoMax.HasValue && x.PresupuestoMin.HasValue)
            .WithMessage("El presupuesto máximo debe ser mayor o igual al mínimo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        // Moneda requerida si hay presupuesto
        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .When(x => x.PresupuestoMin.HasValue || x.PresupuestoMax.HasValue)
            .WithMessage("La moneda es obligatoria cuando se especifica presupuesto")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // Ubicacion requerida para Presencial (1) o Hibrido (3)
        RuleFor(x => x.UbicacionCiudad)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicación (ciudad) es obligatoria para modalidad Presencial o Híbrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.UbicacionCiudad))
            .WithMessage("La ubicación (ciudad) no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.UbicacionPais)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicación (país) es obligatoria para modalidad Presencial o Híbrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.UbicacionPais))
            .WithMessage("La ubicación (país) no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // Fechas
        RuleFor(x => x.FechaLimitePropuestas)
            .GreaterThan(DateTime.UtcNow.Date)
            .When(x => x.FechaLimitePropuestas.HasValue)
            .WithMessage("La fecha límite debe ser posterior a hoy")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        RuleFor(x => x.FechaInicioPrevista)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.FechaInicioPrevista.HasValue)
            .WithMessage("La fecha de inicio debe ser igual o posterior a hoy")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        // ProyectoArtisticoId (existe + ownership)
        RuleFor(x => x.ProyectoArtisticoId)
            .NotEmpty()
            .WithMessage("El proyecto artístico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MustAsync(async (command, proyectoId, ct) =>
            {
                var proyecto = await _proyectoService.GetByIdAsync(new ProyectoArtisticoId(proyectoId), ct);
                return proyecto != null && proyecto.ArtistaId.Value == command.ArtistaId;
            })
            .WithMessage("El proyecto artístico no existe o no te pertenece")
            .WithErrorCode(ServiceResponseMessageType.Auth_Forbidden);
    }
}
```

**IMPORTANTE:** IProyectoArtisticoService debe usar RequestCache para evitar query duplicado en Handler.

---

### 4.2 UpdateNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/UpdateNecesidadCommandValidator.cs`

Similar a CreateNecesidadCommandValidator pero con validaciones adicionales:

| Campo adicional | Regla | Mensaje | ErrorCode (Constant) |
|----------------|-------|---------|----------------------|
| Necesidad completa | MustAsync (existe + estado=Abierta + ownership) | Solo se pueden editar necesidades en estado Abierta | `ServiceResponseMessageType.BusinessRule_NecesidadNotEditable` |

**Ejemplo:**
```csharp
public class UpdateNecesidadCommandValidator : AbstractValidator<UpdateNecesidadCommand>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;

    public UpdateNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));

        // ... mismas validaciones de campos que Create (excepto ProyectoArtisticoId y TipoNecesidadId)

        // Validacion critica: estado = Abierta (1) + ownership
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await _necesidadService.GetByIdAsync(new NecesidadCrowdsourcingId(command.Id), ct);
                return necesidad != null
                    && necesidad.EstadoNecesidadId == 1  // Abierta
                    && necesidad.ArtistaId.Value == command.ArtistaId;
            })
            .WithMessage("Solo se pueden editar necesidades en estado Abierta")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }
}
```

---

### 4.3 CerrarNecesidadCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Necesidades/Validators/CerrarNecesidadCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| Motivo | MaxLength(500) | El motivo no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` |
| Necesidad completa | MustAsync (existe + estado in (1,2) + ownership) | Solo se pueden cerrar necesidades en estado Abierta o En Progreso | `ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable` |

**Ejemplo:**
```csharp
public class CerrarNecesidadCommandValidator : AbstractValidator<CerrarNecesidadCommand>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;

    public CerrarNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));

        // Motivo
        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Motivo))
            .WithMessage("El motivo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // Validacion critica: estado in (Abierta, EnProgreso) + ownership
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await _necesidadService.GetByIdAsync(new NecesidadCrowdsourcingId(command.Id), ct);
                return necesidad != null
                    && (necesidad.EstadoNecesidadId == 1 || necesidad.EstadoNecesidadId == 2)  // Abierta o EnProgreso
                    && necesidad.ArtistaId.Value == command.ArtistaId;
            })
            .WithMessage("Solo se pueden cerrar necesidades en estado Abierta o En Progreso")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }
}
```

---

## 5. DTOs

### 5.1 NecesidadCrowdsourcingListDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingListDto.cs`

```csharp
public class NecesidadCrowdsourcingListDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public int NumeroPropuestas { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

**Uso:** Listado paginado en GetMisNecesidadesQuery

---

### 5.2 NecesidadCrowdsourcingDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCrowdsourcingDto.cs`

```csharp
public class NecesidadCrowdsourcingDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public string TipoNecesidadNombre { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int ModalidadTrabajoId { get; set; }
    public string ModalidadTrabajoNombre { get; set; } = null!;
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public string ProyectoArtisticoNombre { get; set; } = null!;
    public List<PropuestaCrowdsourcingDto> Propuestas { get; set; } = new();
}
```

**Uso:** Detalle completo en GetNecesidadByIdQuery

---

### 5.3 PropuestaCrowdsourcingDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/PropuestaCrowdsourcingDto.cs`

```csharp
public class PropuestaCrowdsourcingDto
{
    public Guid Id { get; set; }
    public Guid ProfesionalId { get; set; }
    public string ProfesionalNombre { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public int MonedaId { get; set; }
    public int? TiempoEstimadoDias { get; set; }
    public string Mensaje { get; set; } = null!;
    public int EstadoPropuestaId { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

**Uso:** Nested en NecesidadCrowdsourcingDto

---

### 5.4 NecesidadCreateResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadCreateResultDto.cs`

```csharp
public class NecesidadCreateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

**Uso:** Response de CreateNecesidadCommand

---

### 5.5 NecesidadUpdateResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadUpdateResultDto.cs`

```csharp
public class NecesidadUpdateResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int EstadoNecesidadId { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public DateTime? FechaActualizacion { get; set; }
}
```

**Uso:** Response de UpdateNecesidadCommand

---

### 5.6 CerrarNecesidadResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CerrarNecesidadResultDto.cs`

```csharp
public class CerrarNecesidadResultDto
{
    public Guid Id { get; set; }
    public string EstadoNecesidadNombre { get; set; } = null!;
    public int PropuestasRechazadas { get; set; }
}
```

**Uso:** Response de CerrarNecesidadCommand

---

## 6. AutoMapper Profile

### 6.1 NecesidadCrowdsourcingProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/NecesidadCrowdsourcingProfile.cs`

```csharp
using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class NecesidadCrowdsourcingProfile : Profile
{
    public NecesidadCrowdsourcingProfile()
    {
        // Command -> Entity (para crear)
        CreateMap<CreateNecesidadCommand, NecesidadCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaId, opt => opt.Ignore()) // Se asigna en Handler
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.Ignore()) // Se asigna en Handler
            .ForMember(dest => dest.EstadoNecesidadId, opt => opt.Ignore()) // Se asigna en Handler (1=Abierta)
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()); // Se asigna en Handler

        // Entity -> ListDto
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.NumeroPropuestas, opt => opt.Ignore()); // Se calcula en Repository o Handler

        // Entity -> DTO completo
        CreateMap<NecesidadCrowdsourcing, NecesidadCrowdsourcingDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ProyectoArtisticoId, opt => opt.MapFrom(src => src.ProyectoArtisticoId.Value))
            .ForMember(dest => dest.EstadoNecesidadNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.TipoNecesidadNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.ModalidadTrabajoNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore()) // Se asigna manualmente o via Include
            .ForMember(dest => dest.ProyectoArtisticoNombre, opt => opt.Ignore()) // Se asigna manualmente
            .ForMember(dest => dest.Propuestas, opt => opt.MapFrom(src => src.Propuestas)); // Mapeo de coleccion
    }
}
```

**Nota:** Las propiedades "Nombre" de maestras se pueden ignorar en mapping y asignar manualmente en Handler, O incluir navegaciones en query EF Core y mapear automaticamente.

---

### 6.2 PropuestaCrowdsourcingProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/PropuestaCrowdsourcingProfile.cs`

```csharp
using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class PropuestaCrowdsourcingProfile : Profile
{
    public PropuestaCrowdsourcingProfile()
    {
        CreateMap<PropuestaCrowdsourcing, PropuestaCrowdsourcingDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ProfesionalId, opt => opt.MapFrom(src => src.ProfesionalId.Value))
            .ForMember(dest => dest.ProfesionalNombre, opt => opt.Ignore()) // Se asigna via Include o manual
            .ForMember(dest => dest.EstadoPropuestaNombre, opt => opt.Ignore()); // Se asigna via Include o manual
    }
}
```

---

## 7. Flujo de Datos Completo

### 7.1 CreateNecesidadCommand

```
1. Controller recibe CreateNecesidadRequest
2. Controller extrae ArtistaId desde JWT
3. Controller crea CreateNecesidadCommand + asigna ArtistaId
4. Controller -> MediatR.Send(command)
5. Handler recibe command
6. Handler -> Validator.ValidateAsync(command)
   └─> Validator -> ProyectoArtisticoService.GetByIdAsync(proyectoId)
       └─> Service -> RequestCache.GetOrAddAsync()
           └─> Cache MISS -> Repository -> DB
7. Si validacion falla, retornar ServiceResponse con errores
8. Handler -> Mapper.Map<NecesidadCrowdsourcing>(command)
9. Handler aplica logica negocio (estado=Abierta, fechas, strongly typed IDs)
10. Handler -> Service.CreateAsync(entity)
    └─> Service -> Repository.AddAsync(entity)
    └─> Service -> UnitOfWork.CommitAsync()
11. Handler -> Mapear entity -> NecesidadCreateResultDto
12. Handler retorna ServiceResponse exitoso
13. Controller retorna 201 Created con Location header
```

---

### 7.2 UpdateNecesidadCommand

```
1. Controller recibe UpdateNecesidadRequest
2. Controller extrae ArtistaId desde JWT
3. Controller crea UpdateNecesidadCommand + asigna ArtistaId
4. Controller -> MediatR.Send(command)
5. Handler recibe command
6. Handler -> Validator.ValidateAsync(command)
   └─> Validator -> NecesidadService.GetByIdAsync(id) - Valida estado=Abierta + ownership
       └─> Service -> RequestCache.GetOrAddAsync()
           └─> Cache MISS -> Repository -> DB
7. Si validacion falla, retornar ServiceResponse con errores
8. Handler -> Service.GetByIdAsync(id) - Cache HIT (sin DB)
9. Handler actualiza propiedades editables de entity
10. Handler -> Service.UpdateAsync(entity)
    └─> Service -> Repository.UpdateAsync(entity)
    └─> Service -> UnitOfWork.CommitAsync()
11. Handler -> Mapear entity -> NecesidadUpdateResultDto
12. Handler retorna ServiceResponse exitoso
13. Controller retorna 200 OK
```

**BENEFICIO RequestCache:** Query DB solo 1 vez (en Validator). Handler reutiliza desde cache.

---

### 7.3 CerrarNecesidadCommand

```
1. Controller recibe CerrarNecesidadRequest
2. Controller extrae ArtistaId desde JWT
3. Controller crea CerrarNecesidadCommand + asigna ArtistaId
4. Controller -> MediatR.Send(command)
5. Handler recibe command
6. Handler -> Validator.ValidateAsync(command)
   └─> Validator -> NecesidadService.GetByIdAsync(id) - Valida estado in (Abierta, EnProgreso) + ownership
       └─> Service -> RequestCache.GetOrAddAsync()
           └─> Cache MISS -> Repository -> DB
7. Si validacion falla, retornar ServiceResponse con errores
8. Handler -> Service.CerrarAsync(necesidadId, motivo)
   └─> Service inicia transaccion atomica (UoW):
       └─> Service.GetByIdAsync(id) -> Repository
       └─> Service actualiza necesidad (estado=Cerrada, motivo, fecha)
       └─> Service -> PropuestaService.RechazarPropuestasPendientesAsync(necesidadId)
           └─> PropuestaService -> PropuestaRepository.GetPendientesByNecesidadIdAsync()
           └─> PropuestaService actualiza propuestas (estado=Rechazada, fecha)
           └─> PropuestaService -> PropuestaRepository.UpdateManyAsync()
       └─> Service -> UnitOfWork.CommitAsync() - ATOMICO (todo o nada)
       └─> Service retorna num propuestas rechazadas
9. Handler crea CerrarNecesidadResultDto con count
10. Handler retorna ServiceResponse exitoso
11. Controller retorna 200 OK
```

**CRITICO:** Transaccion atomica via UoW. Si falla rechazo de propuestas, necesidad NO se cierra (rollback).

---

### 7.4 GetMisNecesidadesQuery

```
1. Controller recibe query params (estado, search, page, pageSize)
2. Controller extrae ArtistaId desde JWT
3. Controller crea GetMisNecesidadesQuery + asigna ArtistaId
4. Controller -> MediatR.Send(query)
5. Handler recibe query
6. Handler -> Service.GetByArtistaIdPaginatedAsync(artistaId, filtros, paginacion)
   └─> Service -> Repository.GetByArtistaIdPaginatedAsync()
       └─> Repository query con filtros (WHERE, LIKE) + paginacion (SKIP, TAKE)
       └─> Repository retorna (Items, TotalCount)
7. Handler -> Mapper.Map<List<NecesidadCrowdsourcingListDto>>(items)
8. Handler crea PaginatedResponse (items, totalCount, page, pageSize, totalPages)
9. Handler retorna ServiceResponse exitoso
10. Controller retorna 200 OK
```

---

### 7.5 GetNecesidadByIdQuery

```
1. Controller recibe id (path param)
2. Controller extrae ArtistaId desde JWT
3. Controller crea GetNecesidadByIdQuery + asigna ArtistaId
4. Controller -> MediatR.Send(query)
5. Handler recibe query
6. Handler -> Service.GetByIdWithDetailsAsync(id)
   └─> Service -> Repository.GetByIdWithDetailsAsync()
       └─> Repository query con Include(Propuestas)
       └─> Repository retorna entity completa
7. Si null, retornar NOT_FOUND
8. Handler valida ownership (entity.ArtistaId == query.ArtistaId)
9. Si no coincide, retornar FORBIDDEN
10. Handler -> Mapper.Map<NecesidadCrowdsourcingDto>(entity)
11. Handler retorna ServiceResponse exitoso
12. Controller retorna 200 OK
```

---

## 8. Archivos a Crear

```
Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/
├── Dtos/
│   ├── NecesidadCrowdsourcingListDto.cs          [CREAR]
│   ├── NecesidadCrowdsourcingDto.cs              [CREAR]
│   ├── PropuestaCrowdsourcingDto.cs              [CREAR]
│   ├── NecesidadCreateResultDto.cs               [CREAR]
│   ├── NecesidadUpdateResultDto.cs               [CREAR]
│   └── CerrarNecesidadResultDto.cs               [CREAR]
│
├── Features/Necesidades/
│   ├── Commands/
│   │   ├── CreateNecesidadCommand.cs             [CREAR - Command + Handler]
│   │   ├── UpdateNecesidadCommand.cs             [CREAR - Command + Handler]
│   │   └── CerrarNecesidadCommand.cs             [CREAR - Command + Handler]
│   │
│   ├── Queries/
│   │   ├── GetMisNecesidadesQuery.cs             [CREAR - Query + Handler]
│   │   └── GetNecesidadByIdQuery.cs              [CREAR - Query + Handler]
│   │
│   └── Validators/
│       ├── CreateNecesidadCommandValidator.cs    [CREAR]
│       ├── UpdateNecesidadCommandValidator.cs    [CREAR]
│       └── CerrarNecesidadCommandValidator.cs    [CREAR]
│
└── Mapping/
    ├── NecesidadCrowdsourcingProfile.cs          [CREAR]
    └── PropuestaCrowdsourcingProfile.cs          [CREAR]
```

**Total:** 16 archivos nuevos

---

## 9. Patrones Importantes

### 9.1 Logica de Negocio en Handler

**Handler contiene:**
- Validacion (via FluentValidation)
- Logica de negocio (calculos, reglas, transformaciones)
- Orquestacion de llamadas a Services
- Mapping Command/Query -> Entity -> DTO

**Service contiene:**
- SOLO persistencia (CRUD via Repository + UoW)
- Transacciones atomicas (UoW)
- Cache (RequestCache)
- NO logica de negocio

---

### 9.2 ServiceResponse siempre con Constants

```csharp
// Exito - USAR CONSTANTS
return new ServiceResponse<T>
{
    Data = result,
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Necesidad creada",
            ErrorCode = ServiceResponseMessageType.Created
        }
    }
};

// Error de validacion
return new ServiceResponse<T>
{
    Messages = validationResult.GetServiceResponseMessages()
};

// Error de negocio - USAR CONSTANTS
return new ServiceResponse<T>
{
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Solo se pueden editar necesidades en estado Abierta",
            ErrorCode = ServiceResponseMessageType.BusinessRule_NecesidadNotEditable
        }
    }
};
```

---

### 9.3 Validators con ServiceResponseMessageType

**CRITICO:** NO usar strings literales

```csharp
// CORRECTO
RuleFor(x => x.Titulo)
    .NotEmpty()
    .WithMessage("El título es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);

// INCORRECTO
RuleFor(x => x.Titulo)
    .NotEmpty()
    .WithMessage("El título es obligatorio")
    .WithErrorCode("VALIDATION_REQUIRED"); // NO HACER
```

---

### 9.4 Handlers con Try-Catch + Logging

```csharp
public async Task<ServiceResponse<T>> Handle(TCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validar
        // 2. Logica de negocio
        // 3. Persistir
        // 4. Retornar exito
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating NecesidadCrowdsourcing");
        return ValidateExtensions.InternalServerErrorServiceResponse<T>(
            "Error inesperado",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

---

### 9.5 Constructor con ?? throw

```csharp
public CreateNecesidadCommandHandler(
    INecesidadCrowdsourcingService service,
    IMapper mapper,
    IValidator<CreateNecesidadCommand> validator,
    ILogger<CreateNecesidadCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**CRITICO:** Validar TODAS las dependencias con `?? throw new ArgumentNullException`.

---

## 10. Checklist

- [ ] Commands implementan IRequest<ServiceResponse<T>>
- [ ] Queries implementan IRequest<ServiceResponse<T>>
- [ ] Handler en MISMO archivo que Command/Query
- [ ] Handler inyecta Service, Mapper, Validator, Logger
- [ ] **Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias**
- [ ] Handler NUNCA inyecta DbContext
- [ ] Validators en carpeta `Validators/` separada
- [ ] **Validators usan `ServiceResponseMessageType.X` (NO strings literales)**
- [ ] **Handlers usan `ServiceResponseMessageType.X` en respuestas**
- [ ] Try-catch con logging en Handlers
- [ ] Services retornan Entidades, Handlers mapean a DTOs
- [ ] RequestCache evita queries duplicados Validator/Handler
- [ ] Transacciones atomicas con UoW (cerrar necesidad)
- [ ] AutoMapper profiles separados por entidad
- [ ] DTOs con propiedades non-nullable correctamente marcadas
- [ ] Validacion condicional (ubicacion, moneda, estado)

---

## 11. Siguiente Paso Sugerido

**Orden de implementacion:**

1. **DTOs** (6 archivos) - Crear todos los DTOs de Request/Response
2. **Commands** (3 archivos) - Crear Commands con Handlers en mismo archivo
3. **Queries** (2 archivos) - Crear Queries con Handlers en mismo archivo
4. **Validators** (3 archivos) - Crear validadores con todas las reglas
5. **AutoMapper Profiles** (2 archivos) - Crear mappings
6. **Testing** - Unit tests de Validators y Handlers

**Comandos para verificar:**

```bash
# Compilar Application
cd src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application
dotnet build

# Ejecutar tests
cd ../../../../../../tests/Crowdsourcing.Application.Tests
dotnet test
```

**Dependencias requeridas antes de implementar:**

- [ ] `INecesidadCrowdsourcingService` con metodos: GetByIdAsync, GetByIdWithDetailsAsync, GetByArtistaIdPaginatedAsync, CreateAsync, UpdateAsync, CerrarAsync (ver hexagonal-architecture.md)
- [ ] `IPropuestaCrowdsourcingService` con metodo: RechazarPropuestasPendientesAsync
- [ ] `IProyectoArtisticoService` con metodo: GetByIdAsync (para validar ownership)
- [ ] `ServiceResponseMessageType` con constantes: Validation_MinLength (1011), Validation_InvalidDate (1012), NotFound_Necesidad (2009), BusinessRule_NecesidadNotEditable (4001), BusinessRule_NecesidadNotCloseable (4002)
- [ ] Entidad `NecesidadCrowdsourcing` con campo `MotivoCierre` (ver hexagonal-architecture.md)

---

**Fin del Plan CQRS**
