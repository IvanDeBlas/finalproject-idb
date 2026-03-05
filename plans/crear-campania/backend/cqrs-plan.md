# Plan CQRS: Crear Campaña de Crowdfunding

**Fecha:** 2026-02-12
**Modulo:** Crowdfunding
**Feature:** crear-campania (US-02)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Crear campaña | Command | CreateCampaniaCommand | ServiceResponse\<CampaniaDto\> |
| Actualizar campaña | Command | UpdateCampaniaCommand | ServiceResponse\<bool\> |
| Publicar campaña | Command | PublishCampaniaCommand | ServiceResponse\<PublishCampaniaResponse\> |
| Obtener por ID | Query | GetCampaniaByIdQuery | ServiceResponse\<CampaniaDto\> |
| Listar campañas públicas | Query | GetAllCampaniasQuery | ServiceResponse\<IEnumerable\<CampaniaListDto\>\> |
| Listar mis campañas | Query | GetMisCampaniasQuery | ServiceResponse\<IEnumerable\<CampaniaListDto\>\> |

---

## 2. Commands

### 2.1 CreateCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/CreateCampaniaCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Estado actual:** ✅ EXISTE - Requiere refinamiento

#### Command

| Propiedad | Type | Requerido | Default | Notas |
|-----------|------|-----------|---------|-------|
| ArtistaId | Guid | Si | - | Extraído del token JWT en Controller |
| ProyectoArtisticoId | Guid? | No | null | Opcional para MVP |
| Titulo | string | Si | - | Max 200 caracteres |
| Subtitulo | string? | No | null | Max 300 caracteres |
| DescripcionCorta | string? | No | null | Max 500 caracteres |
| VideoPrincipalUrl | string? | No | null | URL válida, max 500 |
| ImagenPrincipalUrl | string? | No | null | URL válida, max 500 |
| MonedaId | int | Si | 1 | 1 = EUR |
| ImporteObjetivo | decimal | Si | - | > 0 |
| ImporteMinimo | decimal? | No | null | > 0, <= ImporteObjetivo |
| TipoFinanciacionId | int | Si | - | 1 = Todo o Nada, 2 = Flexible |
| PermiteAportacionesAnonimas | bool | No | false | - |
| PermitePropinas | bool | No | false | - |
| FechaInicio | DateTime? | No | null | < FechaFin |
| FechaFin | DateTime? | No | null | > FechaInicio |

**Implementa:** `IRequest<ServiceResponse<CampaniaDto>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly IValidator<CreateCampaniaCommand> _validator;
private readonly ILogger<CreateCampaniaCommandHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public CreateCampaniaCommandHandler(
    ICampaniaService service,
    IMapper mapper,
    IValidator<CreateCampaniaCommand> validator,
    ILogger<CreateCampaniaCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<CampaniaDto>> Handle(CreateCampaniaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validar con FluentValidation
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ServiceResponse<CampaniaDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Mapear Command -> Entity
        var entity = _mapper.Map<CampaniaCrowdfunding>(request);

        // 3. Establecer valores automáticos (lógica de negocio en Handler)
        entity.Id = CampaniaCrowdfundingId.CreateNew();
        entity.EstadoCampaniaId = 1; // BORRADOR
        entity.ImportePledgedActual = 0;
        entity.FechaCreacion = DateTime.UtcNow;
        entity.Borrado = false;

        // 4. Persistir via Service
        var id = await _service.CreateAsync(entity, ct);

        // 5. Obtener entidad creada para mapear a DTO
        var createdEntity = await _service.GetByIdAsync(id, ct);

        // 6. Mapear Entity -> DTO
        var dto = _mapper.Map<CampaniaDto>(createdEntity);

        // 7. Retornar éxito - USAR CONSTANTS
        return new ServiceResponse<CampaniaDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campaña creada correctamente",
                    ErrorCode = ServiceResponseMessageType.Created
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear campaña para Artista {ArtistaId}", request.ArtistaId);
        return new ServiceResponse<CampaniaDto>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al crear campaña",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

### 2.2 UpdateCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/UpdateCampaniaCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Estado actual:** ✅ EXISTE - Requiere refinamiento

#### Command

| Propiedad | Type | Requerido | Notas |
|-----------|------|-----------|-------|
| Id | Guid | Si | Debe coincidir con URL param |
| ArtistaId | Guid | Si (interno) | Extraído del token, NO del body |
| Titulo | string? | No | Max 200, solo si presente |
| Subtitulo | string? | No | Max 300, solo si presente |
| DescripcionCorta | string? | No | Max 500, solo si presente |
| VideoPrincipalUrl | string? | No | URL válida, solo si presente |
| ImagenPrincipalUrl | string? | No | URL válida, solo si presente |
| ImporteObjetivo | decimal? | No | > 0, solo si presente |
| ImporteMinimo | decimal? | No | > 0, <= ImporteObjetivo, solo si presente |
| TipoFinanciacionId | int? | No | > 0, solo si presente |
| PermiteAportacionesAnonimas | bool? | No | - |
| PermitePropinas | bool? | No | - |
| FechaInicio | DateTime? | No | - |
| FechaFin | DateTime? | No | > FechaInicio, solo si ambos presentes |

**Implementa:** `IRequest<ServiceResponse<bool>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly IValidator<UpdateCampaniaCommand> _validator;
private readonly ILogger<UpdateCampaniaCommandHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public UpdateCampaniaCommandHandler(
    ICampaniaService service,
    IMapper mapper,
    IValidator<UpdateCampaniaCommand> validator,
    ILogger<UpdateCampaniaCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<bool>> Handle(UpdateCampaniaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validar con FluentValidation
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ServiceResponse<bool>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Obtener campaña existente
        var campaniaId = new CampaniaCrowdfundingId(request.Id);
        var existingCampania = await _service.GetByIdAsync(campaniaId, ct);

        // 3. Validar que existe
        if (existingCampania == null)
        {
            return new ServiceResponse<bool>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campaña no encontrada",
                        ErrorCode = ServiceResponseMessageType.NotFound_Campania
                    }
                }
            };
        }

        // 4. Validar ownership (lógica de negocio en Handler)
        if (existingCampania.ArtistaId.Value != request.ArtistaId)
        {
            _logger.LogWarning(
                "Artista {ArtistaId} intentó editar campaña {CampaniaId} que no le pertenece",
                request.ArtistaId,
                request.Id);

            return new ServiceResponse<bool>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "No tienes permiso para editar esta campaña",
                        ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                    }
                }
            };
        }

        // 5. Validar estado (solo BORRADOR puede editarse)
        if (existingCampania.EstadoCampaniaId != 1)
        {
            return new ServiceResponse<bool>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Solo se pueden editar campañas en estado borrador",
                        ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
                    }
                }
            };
        }

        // 6. Actualizar solo campos presentes (patch parcial)
        if (request.Titulo != null)
            existingCampania.Titulo = request.Titulo;

        if (request.Subtitulo != null)
            existingCampania.Subtitulo = request.Subtitulo;

        if (request.DescripcionCorta != null)
            existingCampania.DescripcionCorta = request.DescripcionCorta;

        if (request.VideoPrincipalUrl != null)
            existingCampania.VideoPrincipalUrl = request.VideoPrincipalUrl;

        if (request.ImagenPrincipalUrl != null)
            existingCampania.ImagenPrincipalUrl = request.ImagenPrincipalUrl;

        if (request.ImporteObjetivo.HasValue)
            existingCampania.ImporteObjetivo = request.ImporteObjetivo.Value;

        if (request.ImporteMinimo.HasValue)
            existingCampania.ImporteMinimo = request.ImporteMinimo;

        if (request.TipoFinanciacionId.HasValue)
            existingCampania.TipoFinanciacionId = request.TipoFinanciacionId.Value;

        if (request.PermiteAportacionesAnonimas.HasValue)
            existingCampania.PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas.Value;

        if (request.PermitePropinas.HasValue)
            existingCampania.PermitePropinas = request.PermitePropinas.Value;

        if (request.FechaInicio.HasValue)
            existingCampania.FechaInicio = request.FechaInicio;

        if (request.FechaFin.HasValue)
            existingCampania.FechaFin = request.FechaFin;

        // 7. Establecer FechaActualizacion
        existingCampania.FechaActualizacion = DateTime.UtcNow;

        // 8. Persistir via Service
        await _service.UpdateAsync(existingCampania, ct);

        // 9. Retornar éxito - USAR CONSTANTS
        return new ServiceResponse<bool>
        {
            Data = true,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campaña actualizada correctamente",
                    ErrorCode = ServiceResponseMessageType.Updated
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar campaña {CampaniaId}", request.Id);
        return new ServiceResponse<bool>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al actualizar campaña",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

### 2.3 PublishCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/PublishCampaniaCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Estado actual:** ❌ NO EXISTE - Crear nuevo

#### Command

| Propiedad | Type | Requerido | Notas |
|-----------|------|-----------|-------|
| Id | Guid | Si | ID de la campaña a publicar |
| ArtistaId | Guid | Si (interno) | Extraído del token, NO del body |

**Implementa:** `IRequest<ServiceResponse<PublishCampaniaResponse>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IValidator<PublishCampaniaCommand> _validator;
private readonly ILogger<PublishCampaniaCommandHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public PublishCampaniaCommandHandler(
    ICampaniaService service,
    IValidator<PublishCampaniaCommand> validator,
    ILogger<PublishCampaniaCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<PublishCampaniaResponse>> Handle(PublishCampaniaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validar con FluentValidation (solo Id y ArtistaId)
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ServiceResponse<PublishCampaniaResponse>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Obtener campaña existente
        var campaniaId = new CampaniaCrowdfundingId(request.Id);
        var campania = await _service.GetByIdAsync(campaniaId, ct);

        // 3. Validar que existe
        if (campania == null)
        {
            return new ServiceResponse<PublishCampaniaResponse>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campaña no encontrada",
                        ErrorCode = ServiceResponseMessageType.NotFound_Campania
                    }
                }
            };
        }

        // 4. Validar ownership
        if (campania.ArtistaId.Value != request.ArtistaId)
        {
            _logger.LogWarning(
                "Artista {ArtistaId} intentó publicar campaña {CampaniaId} que no le pertenece",
                request.ArtistaId,
                request.Id);

            return new ServiceResponse<PublishCampaniaResponse>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "No tienes permiso para publicar esta campaña",
                        ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                    }
                }
            };
        }

        // 5. Validar estado (solo BORRADOR puede publicarse)
        if (campania.EstadoCampaniaId != 1)
        {
            return new ServiceResponse<PublishCampaniaResponse>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Solo se pueden publicar campañas en estado borrador",
                        ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
                    }
                }
            };
        }

        // 6. Validar campos requeridos para publicación (lógica de negocio en Handler)
        var errors = new List<ServiceResponseMessage>();

        if (string.IsNullOrEmpty(campania.Titulo))
        {
            errors.Add(new ServiceResponseMessage
            {
                Message = "El título es obligatorio para publicar",
                ErrorCode = ServiceResponseMessageType.Validation_Required
            });
        }

        if (campania.ImporteObjetivo <= 0)
        {
            errors.Add(new ServiceResponseMessage
            {
                Message = "El importe objetivo es obligatorio y debe ser mayor a 0",
                ErrorCode = ServiceResponseMessageType.Validation_InvalidAmount
            });
        }

        if (campania.MonedaId <= 0)
        {
            errors.Add(new ServiceResponseMessage
            {
                Message = "La moneda es obligatoria",
                ErrorCode = ServiceResponseMessageType.Validation_Required
            });
        }

        if (campania.TipoFinanciacionId <= 0)
        {
            errors.Add(new ServiceResponseMessage
            {
                Message = "El tipo de financiación es obligatorio",
                ErrorCode = ServiceResponseMessageType.Validation_Required
            });
        }

        if (campania.FechaFin == null || campania.FechaFin < DateTime.UtcNow.AddDays(7))
        {
            errors.Add(new ServiceResponseMessage
            {
                Message = "La campaña debe durar mínimo 7 días desde hoy",
                ErrorCode = ServiceResponseMessageType.Validation_InvalidDate
            });
        }

        if (errors.Any())
        {
            return new ServiceResponse<PublishCampaniaResponse>
            {
                Messages = errors
            };
        }

        // 7. Actualizar entidad (transición de estado)
        campania.EstadoCampaniaId = 2; // PUBLICADA
        campania.FechaPublicacion = DateTime.UtcNow;

        // Si FechaInicio es null, establecerla ahora
        if (campania.FechaInicio == null)
        {
            campania.FechaInicio = DateTime.UtcNow;
        }

        campania.FechaActualizacion = DateTime.UtcNow;

        // 8. Persistir via Service
        await _service.UpdateAsync(campania, ct);

        // 9. Crear response DTO
        var response = new PublishCampaniaResponse
        {
            Id = campania.Id.Value,
            EstadoCampaniaId = campania.EstadoCampaniaId,
            FechaPublicacion = campania.FechaPublicacion.Value,
            Message = "Campaña publicada exitosamente"
        };

        // 10. Retornar éxito - USAR CONSTANTS
        _logger.LogInformation(
            "Campaña {CampaniaId} publicada por Artista {ArtistaId}",
            campania.Id.Value,
            request.ArtistaId);

        return new ServiceResponse<PublishCampaniaResponse>
        {
            Data = response,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campaña publicada exitosamente",
                    ErrorCode = ServiceResponseMessageType.Success
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al publicar campaña {CampaniaId}", request.Id);
        return new ServiceResponse<PublishCampaniaResponse>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al publicar campaña",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

## 3. Queries

### 3.1 GetCampaniaByIdQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaByIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Estado actual:** ✅ EXISTE - Verificar que siga patrón correcto

#### Query

| Propiedad | Type | Requerido |
|-----------|------|-----------|
| Id | Guid | Si |

**Implementa:** `IRequest<ServiceResponse<CampaniaDto>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly ILogger<GetCampaniaByIdQueryHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public GetCampaniaByIdQueryHandler(
    ICampaniaService service,
    IMapper mapper,
    ILogger<GetCampaniaByIdQueryHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<CampaniaDto>> Handle(GetCampaniaByIdQuery request, CancellationToken ct)
{
    try
    {
        // 1. Llamar a Service (usa cache si está disponible)
        var campaniaId = new CampaniaCrowdfundingId(request.Id);
        var entity = await _service.GetByIdAsync(campaniaId, ct);

        // 2. Si no existe, retornar NOT_FOUND - USAR CONSTANTS
        if (entity == null)
        {
            return new ServiceResponse<CampaniaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campaña no encontrada",
                        ErrorCode = ServiceResponseMessageType.NotFound_Campania
                    }
                }
            };
        }

        // 3. Mapear Entity -> DTO
        var dto = _mapper.Map<CampaniaDto>(entity);

        // 4. Retornar éxito - USAR CONSTANTS
        return new ServiceResponse<CampaniaDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campaña encontrada",
                    ErrorCode = ServiceResponseMessageType.Success
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener campaña {CampaniaId}", request.Id);
        return new ServiceResponse<CampaniaDto>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al obtener campaña",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

### 3.2 GetAllCampaniasQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetAllCampaniasQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Estado actual:** ✅ EXISTE - Requiere refinamiento para filtros y paginación

#### Query

| Propiedad | Type | Requerido | Default | Notas |
|-----------|------|-----------|---------|-------|
| SearchTerm | string? | No | null | Buscar en Titulo, Subtitulo, DescripcionCorta |
| ArtistaId | Guid? | No | null | Filtrar por artista |
| EstadoCampaniaId | int? | No | 2 (PUBLICADA) | Por defecto solo públicas |
| PageNumber | int? | No | 1 | > 0 |
| PageSize | int? | No | 10 | 1-50 |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>`

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly ILogger<GetAllCampaniasQueryHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public GetAllCampaniasQueryHandler(
    ICampaniaService service,
    IMapper mapper,
    ILogger<GetAllCampaniasQueryHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<IEnumerable<CampaniaListDto>>> Handle(GetAllCampaniasQuery request, CancellationToken ct)
{
    try
    {
        // 1. Establecer default: solo campañas PUBLICADAS (estado 2) si no se especifica
        var estadoFiltro = request.EstadoCampaniaId ?? 2;

        // 2. Obtener todas las campañas del Service
        var entities = await _service.GetAllAsync(ct);

        // 3. Aplicar filtros (lógica en Handler)
        var filtered = entities.AsQueryable();

        // Filtro por estado
        filtered = filtered.Where(c => c.EstadoCampaniaId == estadoFiltro);

        // Filtro por artista (opcional)
        if (request.ArtistaId.HasValue)
        {
            var artistaId = new ArtistaId(request.ArtistaId.Value);
            filtered = filtered.Where(c => c.ArtistaId == artistaId);
        }

        // Filtro de búsqueda (opcional)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLowerInvariant();
            filtered = filtered.Where(c =>
                (c.Titulo != null && c.Titulo.ToLowerInvariant().Contains(searchLower)) ||
                (c.Subtitulo != null && c.Subtitulo.ToLowerInvariant().Contains(searchLower)) ||
                (c.DescripcionCorta != null && c.DescripcionCorta.ToLowerInvariant().Contains(searchLower))
            );
        }

        // 4. Aplicar paginación
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = Math.Min(request.PageSize ?? 10, 50); // Máximo 50

        var paginatedEntities = filtered
            .OrderByDescending(c => c.FechaCreacion)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // 5. Mapear List<Entity> -> List<CampaniaListDto>
        var dtos = _mapper.Map<IEnumerable<CampaniaListDto>>(paginatedEntities);

        // 6. Retornar éxito - USAR CONSTANTS
        return new ServiceResponse<IEnumerable<CampaniaListDto>>
        {
            Data = dtos,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campañas encontradas",
                    ErrorCode = ServiceResponseMessageType.Success
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al listar campañas");
        return new ServiceResponse<IEnumerable<CampaniaListDto>>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al listar campañas",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

### 3.3 GetMisCampaniasQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Estado actual:** ❌ NO EXISTE - Crear nuevo

#### Query

| Propiedad | Type | Requerido | Default | Notas |
|-----------|------|-----------|---------|-------|
| ArtistaId | Guid | Si (interno) | - | Extraído del token, NO del query string |
| EstadoCampaniaId | int? | No | null | Filtrar por estado (opcional, NO aplicar default) |
| PageNumber | int? | No | 1 | > 0 |
| PageSize | int? | No | 10 | 1-50 |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>`

**NOTA:** Este endpoint retorna TODAS las campañas del artista, incluidos BORRADORES.

#### Handler

**Dependencias:**
```csharp
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly ILogger<GetMisCampaniasQueryHandler> _logger;

// CRITICO: Constructor con ?? throw para TODAS las dependencias
public GetMisCampaniasQueryHandler(
    ICampaniaService service,
    IMapper mapper,
    ILogger<GetMisCampaniasQueryHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo:**
```csharp
public async Task<ServiceResponse<IEnumerable<CampaniaListDto>>> Handle(GetMisCampaniasQuery request, CancellationToken ct)
{
    try
    {
        // 1. Obtener campañas del artista (usa cache si disponible)
        var artistaId = new ArtistaId(request.ArtistaId);
        var entities = await _service.GetByArtistaIdAsync(artistaId, ct);

        // 2. Aplicar filtros (lógica en Handler)
        var filtered = entities.AsQueryable();

        // Filtro por estado (opcional, NO aplicar default)
        if (request.EstadoCampaniaId.HasValue)
        {
            filtered = filtered.Where(c => c.EstadoCampaniaId == request.EstadoCampaniaId.Value);
        }

        // 3. Aplicar paginación
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = Math.Min(request.PageSize ?? 10, 50); // Máximo 50

        var paginatedEntities = filtered
            .OrderByDescending(c => c.FechaCreacion)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // 4. Mapear List<Entity> -> List<CampaniaListDto>
        var dtos = _mapper.Map<IEnumerable<CampaniaListDto>>(paginatedEntities);

        // 5. Retornar éxito - USAR CONSTANTS
        return new ServiceResponse<IEnumerable<CampaniaListDto>>
        {
            Data = dtos,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Campañas encontradas",
                    ErrorCode = ServiceResponseMessageType.Success
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al listar campañas del Artista {ArtistaId}", request.ArtistaId);
        return new ServiceResponse<IEnumerable<CampaniaListDto>>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Error inesperado al listar campañas",
                    ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                }
            }
        };
    }
}
```

---

## 4. Validators

### 4.1 CreateCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/CreateCampaniaCommandValidator.cs`

**Estado actual:** ✅ EXISTE - Verificar que use Constants y tenga todas las reglas

**CRITICO:** Usar `ServiceResponseMessageType.X` constants, NO strings literales

| Campo | Regla | Mensaje | ErrorCode (Constant) | Condición |
|-------|-------|---------|----------------------|-----------|
| ArtistaId | NotEmpty | El ArtistaId es obligatorio | `ServiceResponseMessageType.Validation_Required` | Siempre |
| Titulo | NotEmpty | El título es obligatorio | `ServiceResponseMessageType.Validation_Required` | Siempre |
| Titulo | MaxLength(200) | El título no puede superar los 200 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Siempre |
| Subtitulo | MaxLength(300) | El subtítulo no puede superar los 300 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si no es null/empty |
| DescripcionCorta | MaxLength(500) | La descripción corta no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si no es null/empty |
| VideoPrincipalUrl | MaxLength(500) | La URL del video no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si no es null/empty |
| VideoPrincipalUrl | Must(BeValidUrl) | La URL del video no es válida | `ServiceResponseMessageType.Validation_InvalidUrl` | Si no es null/empty |
| ImagenPrincipalUrl | MaxLength(500) | La URL de la imagen no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si no es null/empty |
| ImagenPrincipalUrl | Must(BeValidUrl) | La URL de la imagen no es válida | `ServiceResponseMessageType.Validation_InvalidUrl` | Si no es null/empty |
| MonedaId | GreaterThan(0) | La moneda es obligatoria | `ServiceResponseMessageType.Validation_Required` | Siempre |
| ImporteObjetivo | GreaterThan(0) | El importe objetivo debe ser mayor a 0 | `ServiceResponseMessageType.Validation_InvalidAmount` | Siempre |
| ImporteMinimo | GreaterThan(0) | El importe mínimo debe ser mayor a 0 | `ServiceResponseMessageType.Validation_InvalidAmount` | Si tiene valor |
| ImporteMinimo | LessThanOrEqualTo(ImporteObjetivo) | El importe mínimo no puede ser mayor al importe objetivo | `ServiceResponseMessageType.Validation_InvalidRange` | Si tiene valor |
| TipoFinanciacionId | GreaterThan(0) | El tipo de financiación es obligatorio | `ServiceResponseMessageType.Validation_Required` | Siempre |
| FechaFin | GreaterThan(FechaInicio) | La fecha de fin debe ser posterior a la fecha de inicio | `ServiceResponseMessageType.Validation_InvalidDate` | Si ambos tienen valor |

**Método auxiliar BeValidUrl:**
```csharp
private static bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url))
        return true;

    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Estructura del Validator:**
```csharp
using WePlayRises.Crowdfunding.Domain.Constants;  // CRITICO: Importar Constants

public class CreateCampaniaCommandValidator : AbstractValidator<CreateCampaniaCommand>
{
    private readonly ICampaniaService _service;

    // CRITICO: Constructor con ?? throw
    public CreateCampaniaCommandValidator(ICampaniaService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));

        // CRITICO: Usar ServiceResponseMessageType.X en vez de strings literales
        RuleFor(x => x.ArtistaId)
            .NotEmpty()
            .WithMessage("El ArtistaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El título es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(200)
            .WithMessage("El título no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // ... resto de reglas siguiendo el patrón
    }
}
```

---

### 4.2 UpdateCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/UpdateCampaniaCommandValidator.cs`

**Estado actual:** ✅ EXISTE - Verificar que use Constants

**CRITICO:** Usar `ServiceResponseMessageType.X` constants, NO strings literales

| Campo | Regla | Mensaje | ErrorCode (Constant) | Condición |
|-------|-------|---------|----------------------|-----------|
| Id | NotEmpty | El Id es obligatorio | `ServiceResponseMessageType.Validation_Required` | Siempre |
| Titulo | MaxLength(200) | El título no puede superar los 200 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si presente |
| Subtitulo | MaxLength(300) | El subtítulo no puede superar los 300 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si presente |
| DescripcionCorta | MaxLength(500) | La descripción corta no puede superar los 500 caracteres | `ServiceResponseMessageType.Validation_MaxLength` | Si presente |
| VideoPrincipalUrl | Must(BeValidUrl) | La URL del video no es válida | `ServiceResponseMessageType.Validation_InvalidUrl` | Si presente |
| ImagenPrincipalUrl | Must(BeValidUrl) | La URL de la imagen no es válida | `ServiceResponseMessageType.Validation_InvalidUrl` | Si presente |
| ImporteObjetivo | GreaterThan(0) | El importe objetivo debe ser mayor a 0 | `ServiceResponseMessageType.Validation_InvalidAmount` | Si presente |
| ImporteMinimo | GreaterThan(0) | El importe mínimo debe ser mayor a 0 | `ServiceResponseMessageType.Validation_InvalidAmount` | Si presente |
| ImporteMinimo | LessThanOrEqualTo(ImporteObjetivo) | El importe mínimo no puede ser mayor al importe objetivo | `ServiceResponseMessageType.Validation_InvalidRange` | Si presente |
| TipoFinanciacionId | GreaterThan(0) | El tipo de financiación debe ser válido | `ServiceResponseMessageType.Validation_Required` | Si presente |
| FechaFin | GreaterThan(FechaInicio) | La fecha de fin debe ser posterior a la fecha de inicio | `ServiceResponseMessageType.Validation_InvalidDate` | Si ambos presentes |

**NOTA:** Validaciones de negocio (ownership, estado BORRADOR) se hacen en Handler, NO en Validator.

---

### 4.3 PublishCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/PublishCampaniaCommandValidator.cs`

**Estado actual:** ❌ NO EXISTE - Crear nuevo

**CRITICO:** Usar `ServiceResponseMessageType.X` constants, NO strings literales

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| Id | NotEmpty | El Id es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| ArtistaId | NotEmpty | El ArtistaId es obligatorio | `ServiceResponseMessageType.Validation_Required` |

**Estructura del Validator:**
```csharp
using WePlayRises.Crowdfunding.Domain.Constants;  // CRITICO: Importar Constants

public class PublishCampaniaCommandValidator : AbstractValidator<PublishCampaniaCommand>
{
    // CRITICO: Constructor sin inyección de Service (validaciones simples)
    public PublishCampaniaCommandValidator()
    {
        // CRITICO: Usar ServiceResponseMessageType.X en vez de strings literales
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El Id es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ArtistaId)
            .NotEmpty()
            .WithMessage("El ArtistaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

**NOTA:** Validaciones de campos requeridos para publicación (Titulo, ImporteObjetivo, FechaFin) se hacen en Handler, NO en Validator.

---

## 5. DTOs

### 5.1 CampaniaDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaDto.cs`

**Estado actual:** ✅ EXISTE (completo)

DTO completo con todas las propiedades de la entidad para respuestas detalladas (GET by ID).

---

### 5.2 CampaniaListDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaListDto.cs`

**Estado actual:** ✅ EXISTE (completo)

DTO simplificado para listados (GET all, mis campañas).

---

### 5.3 CreateCampaniaRequest

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CreateCampaniaRequest.cs`

**Estado actual:** ❌ NO EXISTE - Crear nuevo

**NOTA:** Este DTO NO incluye `ArtistaId` (se extrae del token en Controller).

```csharp
public class CreateCampaniaRequest
{
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; } = 1; // Default EUR
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int TipoFinanciacionId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

---

### 5.4 UpdateCampaniaRequest

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/UpdateCampaniaRequest.cs`

**Estado actual:** ❌ NO EXISTE - Crear nuevo

**NOTA:** Este DTO NO incluye `ArtistaId` (se extrae del token en Controller).

```csharp
public class UpdateCampaniaRequest
{
    public Guid Id { get; set; } // Requerido, debe coincidir con URL param
    // Todos los demás campos opcionales
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal? ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoFinanciacionId { get; set; }
    public bool? PermiteAportacionesAnonimas { get; set; }
    public bool? PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

---

### 5.5 PublishCampaniaResponse

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PublishCampaniaResponse.cs`

**Estado actual:** ❌ NO EXISTE - Crear nuevo

```csharp
public class PublishCampaniaResponse
{
    public Guid Id { get; set; }
    public int EstadoCampaniaId { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public string Message { get; set; } = null!;
}
```

---

## 6. AutoMapper Profiles

### 6.1 CampaniaProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CampaniaProfile.cs`

**Estado actual:** ✅ EXISTE - Verificar que tenga todos los mappings necesarios

**Mappings requeridos:**

| Source | Destination | Notas | Estado |
|--------|-------------|-------|--------|
| CampaniaCrowdfunding | CampaniaDto | Conversión StronglyTypedIds -> Guid | ✅ Verificar |
| CampaniaCrowdfunding | CampaniaListDto | DTO simplificado | ✅ Verificar |
| CreateCampaniaCommand | CampaniaCrowdfunding | Conversión Guid -> StronglyTypedIds | ✅ Verificar |
| UpdateCampaniaCommand | CampaniaCrowdfunding | NO necesario (se actualiza campo por campo en Handler) | ❌ No requerido |

**Transformaciones críticas con StronglyTypedIds:**
```csharp
// Entity -> DTO (conversión de StronglyTypedIds a Guid)
CreateMap<CampaniaCrowdfunding, CampaniaDto>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.ArtistaId,
               opt => opt.MapFrom(src => src.ArtistaId.Value))
    .ForMember(dest => dest.ProyectoArtisticoId,
               opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue
                   ? src.ProyectoArtisticoId.Value.Value
                   : (Guid?)null));

// Command -> Entity (conversión de Guid a StronglyTypedIds)
CreateMap<CreateCampaniaCommand, CampaniaCrowdfunding>()
    .ForMember(dest => dest.ArtistaId,
               opt => opt.MapFrom(src => new ArtistaId(src.ArtistaId)))
    .ForMember(dest => dest.ProyectoArtisticoId,
               opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue
                   ? new ProyectoArtisticoId(src.ProyectoArtisticoId.Value)
                   : (ProyectoArtisticoId?)null))
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.EstadoCampaniaId, opt => opt.Ignore())
    .ForMember(dest => dest.ImportePledgedActual, opt => opt.Ignore())
    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
    .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
    .ForMember(dest => dest.Rewards, opt => opt.Ignore())
    .ForMember(dest => dest.Pedidos, opt => opt.Ignore());
```

---

## 7. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/Campanias/
│   ├── Commands/
│   │   └── PublishCampaniaCommand.cs                     # NUEVO - Command + Handler
│   ├── Queries/
│   │   └── GetMisCampaniasQuery.cs                       # NUEVO - Query + Handler
│   └── Validators/
│       └── PublishCampaniaCommandValidator.cs            # NUEVO
└── Dtos/
    ├── CreateCampaniaRequest.cs                          # NUEVO - DTO para request (sin ArtistaId)
    ├── UpdateCampaniaRequest.cs                          # NUEVO - DTO para request (sin ArtistaId)
    └── PublishCampaniaResponse.cs                        # NUEVO - Response específico
```

---

## 8. Archivos a Refinar (ya existen)

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/Campanias/
│   ├── Commands/
│   │   ├── CreateCampaniaCommand.cs                      # REFINAR - Handler con lógica completa
│   │   └── UpdateCampaniaCommand.cs                      # REFINAR - Agregar validación ownership + estado
│   ├── Queries/
│   │   ├── GetCampaniaByIdQuery.cs                       # VERIFICAR - Debe seguir patrón correcto
│   │   └── GetAllCampaniasQuery.cs                       # REFINAR - Agregar filtros + paginación
│   └── Validators/
│       ├── CreateCampaniaCommandValidator.cs             # VERIFICAR - Usar Constants
│       └── UpdateCampaniaCommandValidator.cs             # VERIFICAR - Usar Constants
└── Mapping/
    └── CampaniaProfile.cs                                # VERIFICAR - Todos los mappings necesarios
```

---

## 9. Patrones Arquitectónicos Críticos

### 9.1 Lógica de Negocio en Handlers

**CORRECTO:**
```csharp
// Handler contiene:
// - Validación (via Validator)
// - Lógica de negocio (ownership, estado, transformaciones)
// - Orquestación de llamadas a services

// CreateCampaniaCommandHandler
entity.EstadoCampaniaId = 1; // BORRADOR
entity.ImportePledgedActual = 0;
entity.FechaCreacion = DateTime.UtcNow;
```

**Service contiene:**
```csharp
// Service contiene SOLO:
// - Persistencia (CRUD via repository)
// - Caching
// - NO lógica de negocio

public async Task<CampaniaCrowdfundingId> CreateAsync(CampaniaCrowdfunding campania, CancellationToken ct)
{
    campania.FechaCreacion = DateTime.UtcNow;  // Solo timestamp
    return await _repository.AddAsync(campania, ct);
}
```

---

### 9.2 ServiceResponse siempre con Constants

**CORRECTO:**
```csharp
// Éxito - USAR CONSTANTS
return new ServiceResponse<CampaniaDto>
{
    Data = dto,
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Campaña creada correctamente",
            ErrorCode = ServiceResponseMessageType.Created
        }
    }
};

// Error de validación
return new ServiceResponse<CampaniaDto>
{
    Messages = validationResult.GetServiceResponseMessages()
};

// Error de negocio - USAR CONSTANTS
return new ServiceResponse<bool>
{
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Solo se pueden editar campañas en estado borrador",
            ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
        }
    }
};
```

**INCORRECTO:**
```csharp
// NUNCA usar strings literales
ErrorCode = "CAMPANIA_NOT_DRAFT"  // NO HACER
```

---

### 9.3 Constructor con ?? throw

**CORRECTO:**
```csharp
public CreateCampaniaCommandHandler(
    ICampaniaService service,
    IMapper mapper,
    IValidator<CreateCampaniaCommand> validator,
    ILogger<CreateCampaniaCommandHandler> logger)
{
    _service = service ?? throw new ArgumentNullException(nameof(service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**INCORRECTO:**
```csharp
public CreateCampaniaCommandHandler(
    ICampaniaService service,
    IMapper mapper)
{
    _service = service;  // NUNCA - falta ?? throw
    _mapper = mapper;    // NUNCA - falta ?? throw
}
```

---

### 9.4 Caching en Services (ADR-006)

**Flujo con caching:**
```
1. Validator llama Service.GetByIdAsync(id)
   └─> Service consulta RequestCache
       └─> Cache MISS -> Repository -> DB
       └─> Almacena resultado en cache

2. Handler llama Service.GetByIdAsync(id)
   └─> Service consulta RequestCache
       └─> Cache HIT -> Retorna inmediato (sin DB)
```

**Beneficio:** Validator y Handler comparten datos sin queries duplicados.

---

## 10. Checklist de Implementación

### Commands

- [ ] **CreateCampaniaCommand** - Handler + Command en MISMO archivo
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Handler inyecta Service, Mapper, Validator, Logger (NO DbContext)
- [ ] Lógica de negocio en Handler (EstadoCampaniaId = 1, ImportePledgedActual = 0)
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<CampaniaDto>`
- [ ] Usa `ServiceResponseMessageType.Created` (NO strings literales)

---

- [ ] **UpdateCampaniaCommand** - Handler + Command en MISMO archivo
- [ ] Constructor con `?? throw` para todas las dependencias
- [ ] Validar ownership (ArtistaId del token == ArtistaId de la campaña)
- [ ] Validar estado (solo BORRADOR puede editarse)
- [ ] Actualización parcial (patch) de campos
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<bool>`
- [ ] Usa `ServiceResponseMessageType.Updated`, `Auth_Forbidden`, `BusinessRule_CampaniaNotDraft`

---

- [ ] **PublishCampaniaCommand** - Handler + Command en MISMO archivo (NUEVO)
- [ ] Constructor con `?? throw` para todas las dependencias
- [ ] Validar ownership
- [ ] Validar estado (solo BORRADOR puede publicarse)
- [ ] Validar campos requeridos (Titulo, ImporteObjetivo, FechaFin >= Now + 7 días)
- [ ] Transición de estado (EstadoCampaniaId = 2, FechaPublicacion = Now)
- [ ] Si FechaInicio == null, establecer FechaInicio = Now
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<PublishCampaniaResponse>`
- [ ] Usa `ServiceResponseMessageType.Success`, `Validation_Required`, `BusinessRule_CampaniaNotDraft`

---

### Queries

- [ ] **GetCampaniaByIdQuery** - Handler + Query en MISMO archivo
- [ ] Constructor con `?? throw` para todas las dependencias
- [ ] Handler llama Service.GetByIdAsync() (usa cache)
- [ ] Si null, retornar `NotFound_Campania`
- [ ] Mapear Entity -> DTO
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<CampaniaDto>`

---

- [ ] **GetAllCampaniasQuery** - Handler + Query en MISMO archivo
- [ ] Constructor con `?? throw` para todas las dependencias
- [ ] Default EstadoCampaniaId = 2 (PUBLICADA) si no se especifica
- [ ] Filtros: SearchTerm, ArtistaId, EstadoCampaniaId
- [ ] Paginación: PageNumber, PageSize (máximo 50)
- [ ] OrderByDescending(FechaCreacion)
- [ ] Mapear List<Entity> -> List<CampaniaListDto>
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<IEnumerable<CampaniaListDto>>`

---

- [ ] **GetMisCampaniasQuery** - Handler + Query en MISMO archivo (NUEVO)
- [ ] Constructor con `?? throw` para todas las dependencias
- [ ] Handler llama Service.GetByArtistaIdAsync() (usa cache)
- [ ] NO aplicar default de estado (retorna TODAS las campañas del artista)
- [ ] Filtro opcional: EstadoCampaniaId
- [ ] Paginación: PageNumber, PageSize (máximo 50)
- [ ] Try-catch con logging
- [ ] Retorna `ServiceResponse<IEnumerable<CampaniaListDto>>`

---

### Validators

- [ ] **CreateCampaniaCommandValidator** - Verificar que existe
- [ ] Constructor con `?? throw` si inyecta dependencias
- [ ] TODAS las reglas usan `ServiceResponseMessageType.X` (NO strings literales)
- [ ] TODAS las reglas tienen `.WithMessage()` Y `.WithErrorCode()`
- [ ] Método auxiliar `BeValidUrl()` para validar URLs

---

- [ ] **UpdateCampaniaCommandValidator** - Verificar que existe
- [ ] Constructor con `?? throw` si inyecta dependencias
- [ ] TODAS las reglas usan `ServiceResponseMessageType.X` (NO strings literales)
- [ ] TODAS las reglas tienen `.WithMessage()` Y `.WithErrorCode()`

---

- [ ] **PublishCampaniaCommandValidator** - Crear nuevo
- [ ] Constructor simple (sin inyecciones)
- [ ] Validar solo Id y ArtistaId NotEmpty
- [ ] Usa `ServiceResponseMessageType.Validation_Required`

---

### DTOs

- [ ] **CreateCampaniaRequest** - Crear nuevo (sin ArtistaId)
- [ ] **UpdateCampaniaRequest** - Crear nuevo (sin ArtistaId, campos opcionales)
- [ ] **PublishCampaniaResponse** - Crear nuevo

---

### AutoMapper

- [ ] **CampaniaProfile** - Verificar que existe
- [ ] Mapping: CampaniaCrowdfunding -> CampaniaDto (con conversión StronglyTypedIds)
- [ ] Mapping: CampaniaCrowdfunding -> CampaniaListDto
- [ ] Mapping: CreateCampaniaCommand -> CampaniaCrowdfunding (con conversión Guid -> StronglyTypedIds)
- [ ] ForMember con Ignore para campos calculados (Id, EstadoCampaniaId, FechaCreacion, etc.)

---

## 11. Siguiente Paso Sugerido

Después de implementar este plan CQRS, el siguiente paso es:

1. **Controller (WebApi Layer):**
   - Crear `CreateCampaniaRequest` y `UpdateCampaniaRequest` DTOs
   - Refinar `CampaniasController` con extracción de `ArtistaId` del token
   - Agregar endpoint `POST /api/campanias/{id}/publicar`
   - Agregar endpoint `GET /api/campanias/mis-campanias`

2. **Testing:**
   - Unit tests para Validators
   - Unit tests para Handlers
   - Integration tests para endpoints

3. **Frontend (Admin Dashboard):**
   - Wizard de 4 pasos para crear campaña
   - Vista previa de campaña
   - Botón "Publicar"
   - Listado "Mis Campañas"

---

**Puntos clave del plan:**
- Todos los Handlers con `?? throw new ArgumentNullException` para TODAS las dependencias
- TODAS las respuestas usan `ServiceResponseMessageType.X` constants (NO strings literales)
- Lógica de negocio en Handlers (ownership, estado, transiciones)
- Services solo persistencia + caching
- Validators con `.WithMessage()` Y `.WithErrorCode(ServiceResponseMessageType.X)`
- Validators en carpeta `Validators/` separada
- Handler + Command/Query en MISMO archivo
- Try-catch con logging en todos los Handlers

---

**Fin del plan CQRS**
